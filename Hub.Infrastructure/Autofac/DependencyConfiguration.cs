using Autofac;

namespace Hub.Infrastructure.Autofac
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder) { }

        public int Order
        {
            get { return -1; }
        }

    }
}
