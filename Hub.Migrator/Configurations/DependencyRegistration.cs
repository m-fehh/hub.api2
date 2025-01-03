using Autofac;
using Hub.Domain.Administrator;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models.Administrator;
using Hub.Infrastructure.Database.MultiTenant;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Microsoft.EntityFrameworkCore;

public class DependencyRegistration : IDependencyConfiguration
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<DbMigrationService>().As<IHostedService>().SingleInstance();
        builder.RegisterType<TenantProvider>().As<ITenantProvider>().SingleInstance();

        builder.RegisterType<AdminDbContext>().As<DbContext>().InstancePerLifetimeScope();
    }

    public int Order => 2;
}
