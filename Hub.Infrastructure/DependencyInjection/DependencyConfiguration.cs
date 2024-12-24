using Autofac;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.MultiTenant;
using Hub.Infrastructure.MultiTenant.Interfaces;

namespace Hub.Infrastructure.DependencyInjection
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultOrmConfiguration>().As<IOrmConfiguration>().SingleInstance();
            builder.RegisterType<DefaultOrmConfiguration>().AsSelf().SingleInstance();

            builder.RegisterType<TenantProvider>().As<ITenantProvider>().SingleInstance();
        }

        public int Order
        {
            get { return -1; }
        }

    }
}
