using Autofac;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using System.Reflection;

namespace Hub.Infrastructure.Autofac
{
    public class EngineInitializationParameters
    {
        public Assembly ExecutingAssembly { get; set; }
        public IList<IDependencyConfiguration> DependencyRegistrators { get; set; } = new List<IDependencyConfiguration>();
        public ContainerBuilder ContainerBuilder { get; set; }

        public EngineInitializationParameters() { }

        public EngineInitializationParameters(Assembly executingAssembly)
        {
            ExecutingAssembly = executingAssembly;
        }
    }
}
