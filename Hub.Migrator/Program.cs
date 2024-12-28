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
    // Inicializa o mecanismo do Hub com Autofac
    Engine.Initialize(
        executingAssembly: Assembly.GetExecutingAssembly(),
        dependencyRegistrars: new List<IDependencyConfiguration>
        {
            // Adicione suas configurações personalizadas de dependência aqui
        },
        containerBuilder: containerBuilder
    );
});

// Após a inicialização do Engine, configuramos os serviços
builder.ConfigureServices((hostContext, services) =>
{
    var configuration = hostContext.Configuration;

    // Adiciona suporte a tenants
    services.AddTenantSupport(configuration);

    // Configura o Entity Framework com SQL Server
    services.AddEntityFrameworkSqlServer<EntityDbContext>();

    // Adiciona o HostedService para migração de banco de dados
    services.AddHostedService<DbMigratorHostedService>();
});

var app = builder.Build();

// Configura o container principal do Engine com o Autofac Root
Engine.SetContainer((IContainer)app.Services.GetAutofacRoot());

await app.RunAsync();
