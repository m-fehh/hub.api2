//using Autofac;
//using Autofac.Extensions.DependencyInjection;
//using Hub.Infrastructure;
//using System.Reflection;

//IHost host = Host.CreateDefaultBuilder(args)
//    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
//    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
//    {
//        Engine.Initialize(
//            executingAssembly: Assembly.GetExecutingAssembly(),
//            containerBuilder: containerBuilder
//        );
//    })
//    .Build();


//await host.RunAsync();


using Hub.DbMigration;
using Hub.Domain;
using System.Reflection;
using Hub.Infrastructure.MultiTenant;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configBuilder =>
    {
        // need to set base path to make it work with shared appsettings files
        configBuilder.SetBasePath(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTenantSupport();
        services.AddEntityFrameworkSqlServer<DatabaseContext>();
        services.AddHostedService<DbMigratorHostedService>();
    })
    .Build();

await host.RunAsync();