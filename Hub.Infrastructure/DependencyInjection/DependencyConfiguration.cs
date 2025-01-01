﻿using Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Autofac.Core;
using Hub.Infrastructure.Autofac.Interfaces;
using Autofac.Core.Registration;
using Hub.Infrastructure.Generator;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Autofac.Builders;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture.Cache.Interfaces;
using Hub.Infrastructure.Architecture.DistributedLock;
using Hub.Infrastructure.Architecture.DistributedLock.Interfaces;
using Hub.Infrastructure.Architecture.Logger;
using Hub.Infrastructure.Architecture.Logger.Interfaces;
using Hub.Infrastructure.Architecture.Localization.Interfaces;
using Hub.Infrastructure.Architecture.Localization;
using Hub.Infrastructure.Autofac;
using FMK.Core.Nominator;
using Hub.Infrastructure.Nominator.Interfaces;

namespace Hub.Infrastructure.DependencyInjection
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultOrmConfiguration>().As<IOrmConfiguration>().SingleInstance();
            builder.RegisterType<DefaultOrmConfiguration>().AsSelf().SingleInstance();

            builder.RegisterType<ConnectionStringBaseConfigurator>().AsSelf().SingleInstance();

            builder.RegisterType<EngineInitializationParametersBuilder>().AsImplementedInterfaces();
            builder.RegisterType<EngineInitializationParametersBuilder>().AsSelf();

            builder.RegisterType<TenantLifeTimeScope>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DefaultTenantManager>().As<ITenantManager>().SingleInstance();

            builder.RegisterType<IgnoreLogScope>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<Repository>().AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).OnActivating(ActivingRepository);

            builder.RegisterType<LogManager>().As<ILogManager>().SingleInstance();

            builder.RegisterType<RedisService>().AsSelf().SingleInstance();
            builder.RegisterType<RedisService>().As<IRedisService>().SingleInstance();
            builder.RegisterType<CacheManager>().AsSelf().SingleInstance();

            builder.RegisterType<IgnoreLogScope>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<IgnoreModificationControl>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<RandomGeneration>().As<IRandomGeneration>().SingleInstance();

            builder.RegisterType<RedLockManager>().AsSelf().SingleInstance();
            builder.RegisterType<RedLockManager>().As<ILockManager>().SingleInstance();

            builder.RegisterType<DefaultLocalizationProvider>().As<ILocalizationProvider>().AsSelf();

            builder.RegisterType<EntityNameProvider>().As<IEntityNameProvider>().AsSelf();

            builder.RegisterType<NominatorManager>().As<INominatorManager>().SingleInstance();
        }

        void ActivingRepository(IActivatingEventArgs<object> e)
        {
            var typeToLookup = e.Instance.GetType().GetGenericArguments()[0];

            if (typeToLookup.IsInterface)
            {
                try
                {
                    var foundEntry = e.Context.Resolve(typeToLookup);

                    if (foundEntry != null)
                    {
                        ((ISetType)e.Instance).SetType(foundEntry.GetType());
                    }
                }
                catch (ComponentNotRegisteredException) { }

            }
        }

        public int Order => -1;
    }
}
