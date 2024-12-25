using System.Reflection;
using Hub.Migrator;
using Hub.Domain.Persistence;
using Hub.Domain;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configBuilder =>
    {
        // need to set base path to make it work with shared appsettings files
        configBuilder.SetBasePath(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTenantSupport(hostContext.Configuration);
        services.AddEntityFrameworkSqlServer<EntityDbContext>();
        services.AddHostedService<DbMigratorHostedService>();
    })
    .Build();

await host.RunAsync();
