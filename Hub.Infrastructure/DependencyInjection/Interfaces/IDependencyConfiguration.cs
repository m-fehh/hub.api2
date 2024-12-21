using Autofac;

namespace Hub.Infrastructure.DependencyInjection.Interfaces
{
    public interface IDependencyConfiguration
    {
        void Register(ContainerBuilder builder);

        int Order { get; }
    }
}
