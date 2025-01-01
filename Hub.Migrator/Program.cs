using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using System.Reflection;
using Hub.Infrastructure.Database;
using Hub.Domain.Persistence;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.Architecture;
using Hub.Migrator.Seeders;
using Hub.Application.Services;

var builder = Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        var basePath = Directory.GetCurrentDirectory(); 
        Console.WriteLine($"Config Base Path: {basePath}"); 
        configBuilder.SetBasePath(basePath);
        configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        foreach (var item in configBuilder.Build().AsEnumerable().Where(c => c.Key.StartsWith("Settings:")))
        {
            Environment.SetEnvironmentVariable(item.Key.Replace("Settings:", ""), item.Value);
        }
    });

builder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    Engine.Initialize(
        executingAssembly: Assembly.GetExecutingAssembly(),
        dependencyRegistrars: new List<IDependencyConfiguration>
        {
            new DependencyRegistration(),
        },
        containerBuilder: containerBuilder,
        csb: new ConnectionStringBaseVM()
        {
            ConnectionStringBaseSchema = "sch",
        }
    );
});


builder.ConfigureServices((hostContext, services) =>
{
    services.AddTenantSupport();
    services.AddEntityFrameworkSqlServer<EntityDbContext>();
    services.AddHostedService<DbMigrationService>();

    // Registrar os seeders automaticamente
    var seederTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(ISeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

    foreach (var seederType in seederTypes)
    {
        services.AddTransient(seederType);
    }

    // Registrar o SeederExecutor para orquestrar os seeders
    services.AddTransient<SeederExecutor>();
});


var app = builder.Build();

// Configura o container principal do Engine com o Autofac Root
Engine.SetContainer((IContainer)app.Services.GetAutofacRoot());

// Inicia a execução da aplicação e o serviço de migração
await app.RunAsync();
