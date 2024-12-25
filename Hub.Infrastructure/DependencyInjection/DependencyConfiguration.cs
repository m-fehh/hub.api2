﻿using Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.DependencyInjection.Interfaces;

namespace Hub.Infrastructure.DependencyInjection
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultOrmConfiguration>().As<IOrmConfiguration>().SingleInstance();
            builder.RegisterType<DefaultOrmConfiguration>().AsSelf().SingleInstance();

        }

        public int Order
        {
            get { return -1; }
        }

    }
}
