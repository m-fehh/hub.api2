using Autofac;
using Hub.Infrastructure.DependencyInjection.Interfaces;

namespace Hub.Infrastructure.DependencyInjection
{
    public class DependencyRegistration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder) { }

        public int Order
        {
            get { return -1; }
        }

    }
}
