using Autofac;
using Hub.Domain.Persistence;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;

public class DependencyRegistration : IDependencyConfiguration
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<DbMigrationService>().As<IHostedService>().SingleInstance();
        builder.RegisterType<TenantProvider>().As<ITenantProvider>().SingleInstance();
    }

    public int Order => 2;
}
