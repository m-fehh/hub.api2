using Hub.Application.Configurations.Mapper;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Tasks.Interfaces;

namespace Hub.Application.Configurations
{
    public class StartupTask : IStartupTask
    {
        public void Execute()
        {
            Engine.RegisterAutoMapperStartup(new MapperConfig());
            //Engine.RegisterAutoMapperStartup(new LogMapperConfig());

            //LocalizationAttributeBootstrap.Initialize();
        }

        public int Order
        {
            get { return 10; }
        }
    }
}
