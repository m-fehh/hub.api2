using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hub.Infrastructure;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using System.Reflection;
using Hub.Infrastructure.Database;
using Hub.Domain.Persistence;
using Hub.Infrastructure.Extensions;

var builder = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        // Define o diretório base para configurar os arquivos de configuração compartilhados
        configBuilder.SetBasePath(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
        configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Define as variáveis de ambiente com base nas configurações que começam com "Settings:"
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
});

var app = builder.Build();

// Configura o container principal do Engine com o Autofac Root
Engine.SetContainer((IContainer)app.Services.GetAutofacRoot());

// Inicia a execução da aplicação e o serviço de migração
await app.RunAsync();
