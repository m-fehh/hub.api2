using Autofac;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using System.Reflection;

namespace Hub.Infrastructure.Architecture
{
    public class EngineInitializationParameters
    {
        public Assembly ExecutingAssembly { get; set; }
        public IList<IDependencyConfiguration> DependencyRegistrators { get; set; } = new List<IDependencyConfiguration>();
        public ConnectionStringBaseVM ConnectionStringBase { get; set; }
        public ContainerBuilder ContainerBuilder { get; set; }

        public EngineInitializationParameters() { }

        public EngineInitializationParameters(Assembly executingAssembly)
        {
            ExecutingAssembly = executingAssembly;
        }
    }
}
