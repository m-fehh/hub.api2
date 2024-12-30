using Autofac;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Autofac.Interfaces;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using System.Reflection;

namespace Hub.Infrastructure.Autofac.Builders
{
    public class EngineInitializationParametersBuilder : IBuilder<EngineInitializationParameters>
    {
        private readonly EngineInitializationParameters _obj = new EngineInitializationParameters();

        public EngineInitializationParametersBuilder(Assembly executingAssembly)
        {
            _obj.ExecutingAssembly = executingAssembly;
        }

        //public EngineInitializationParametersBuilder AddStartupTask(IStartupTask startupTask)
        //{
        //    _obj.StartupTasks.Add(startupTask);
        //    return this;
        //}

        public EngineInitializationParametersBuilder AddDependencyRegistrator(IDependencyConfiguration dependencyRegistrator)
        {
            _obj.DependencyRegistrators.Add(dependencyRegistrator);
            return this;
        }

        //public EngineInitializationParametersBuilder WithNameProvider(INhNameProvider nameProvider)
        //{
        //    _obj.NameProvider = nameProvider;
        //    return this;
        //}

        public EngineInitializationParametersBuilder WithConnectionStringBase(ConnectionStringBaseVM csb)
        {
            _obj.ConnectionStringBase = csb;
            return this;
        }

        public EngineInitializationParametersBuilder WithContainerBuilder(ContainerBuilder containerBuilder)
        {
            _obj.ContainerBuilder = containerBuilder;
            return this;
        }

        public EngineInitializationParameters Build()
        {
            return _obj;
        }
    }
}
