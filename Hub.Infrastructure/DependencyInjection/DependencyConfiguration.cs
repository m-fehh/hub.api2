﻿using Autofac;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure.MultiTenant.Interfaces;
using Hub.Infrastructure.MultiTenant;
using Hub.Infrastructure.Database.Interfaces;

namespace Hub.Infrastructure.DependencyInjection
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<ConnectionStringBaseConfigurator>().AsSelf().SingleInstance();
            builder.RegisterType<TenantManager>().As<ITenantManager>().SingleInstance();
            builder.RegisterType<MigrationManager>().As<IMigrationManager>().SingleInstance();

            builder.RegisterType<DefaultOrmConfiguration>().As<IOrmConfiguration>().SingleInstance();
            builder.RegisterType<DefaultOrmConfiguration>().AsSelf().SingleInstance();
        }

        public int Order
        {
            get { return -1; }
        }

    }
}