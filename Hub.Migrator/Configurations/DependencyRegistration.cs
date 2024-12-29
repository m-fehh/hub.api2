using Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;
using Hub.Infrastructure.DependencyInjection.Interfaces;

public class DependencyRegistration : IDependencyConfiguration
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<DbMigrationService>().As<IHostedService>().SingleInstance();
        builder.RegisterType<TenantProvider>().As<ITenantProvider>().SingleInstance();
    }

    public int Order => 2;
}
