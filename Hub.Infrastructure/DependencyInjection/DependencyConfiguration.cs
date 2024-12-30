using Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Logger;
using Autofac.Core;
using Hub.Infrastructure.Autofac.Interfaces;
using Autofac.Core.Registration;
using Hub.Infrastructure.Logger.Interfaces;
using Hub.Infrastructure.Cache;
using Hub.Infrastructure.Cache.Interfaces;

namespace Hub.Infrastructure.DependencyInjection
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultOrmConfiguration>().As<IOrmConfiguration>().SingleInstance();
            builder.RegisterType<DefaultOrmConfiguration>().AsSelf().SingleInstance();

            builder.RegisterType<ConnectionStringBaseConfigurator>().AsSelf().SingleInstance();

            builder.RegisterType<TenantLifeTimeScope>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DefaultTenantManager>().As<ITenantManager>().SingleInstance();

            builder.RegisterType<IgnoreLogScope>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).OnActivating(ActivingRepository);
            builder.RegisterType<LogManager>().As<ILogManager>().SingleInstance();

            builder.RegisterType<RedisService>().AsSelf().SingleInstance();
            builder.RegisterType<RedisService>().As<IRedisService>().SingleInstance();

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
