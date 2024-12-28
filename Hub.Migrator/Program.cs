using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hub.Domain.Persistence;
using Hub.Domain;
using Hub.Infrastructure;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Migrator;
using System.Reflection;

var builder = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        // Define o diret�rio base para configurar os arquivos de configura��o compartilhados
        configBuilder.SetBasePath(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
        configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Define as vari�veis de ambiente com base nas configura��es que come�am com "Settings:"
        foreach (var item in configBuilder.Build().AsEnumerable().Where(c => c.Key.StartsWith("Settings:")))
        {
            Environment.SetEnvironmentVariable(item.Key.Replace("Settings:", ""), item.Value);
        }
    });

builder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Inicializa o mecanismo do Hub com Autofac
    Engine.Initialize(
        executingAssembly: Assembly.GetExecutingAssembly(),
        dependencyRegistrars: new List<IDependencyConfiguration>
        {
            // Adicione suas configura��es personalizadas de depend�ncia aqui
        },
        containerBuilder: containerBuilder
    );
});

// Ap�s a inicializa��o do Engine, configuramos os servi�os
builder.ConfigureServices((hostContext, services) =>
{
    var configuration = hostContext.Configuration;

    // Adiciona suporte a tenants
    services.AddTenantSupport(configuration);

    // Configura o Entity Framework com SQL Server
    services.AddEntityFrameworkSqlServer<EntityDbContext>();

    // Adiciona o HostedService para migra��o de banco de dados
    services.AddHostedService<DbMigratorHostedService>();
});

var app = builder.Build();

// Configura o container principal do Engine com o Autofac Root
Engine.SetContainer((IContainer)app.Services.GetAutofacRoot());

await app.RunAsync();
