using Autofac;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure.MultiTenant;
using Hub.Infrastructure.MultiTenant.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hub.Infrastructure.DependencyInjection
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<TenantProvider>().As<ITenantProvider>().SingleInstance();

            builder.RegisterType<DbSchemaAwareModelCacheKeyFactory>().As<IModelCacheKeyFactory>().SingleInstance();
            builder.RegisterType<DbSchemaAwareSqlServerMigrationsSqlGenerator>().As<IMigrationsSqlGenerator>().SingleInstance();

        }

        public int Order
        {
            get { return -1; }
        }

    }
}
