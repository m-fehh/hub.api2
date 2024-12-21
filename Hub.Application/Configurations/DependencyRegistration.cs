using Autofac;
using Hub.Infrastructure.DependencyInjection.Interfaces;

namespace Hub.Application.Configurations
{
    public class DependencyRegistration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder) { }

        public int Order
        {
            get { return 1; }
        }
    }
}
