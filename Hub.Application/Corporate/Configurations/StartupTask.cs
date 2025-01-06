using Hub.Application.Corporate.Mapper;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Localization;
using Hub.Infrastructure.Architecture.Tasks.Interfaces;

namespace Hub.Application.Corporate.Configurations
{
    public class StartupTask : IStartupTask
    {
        public void Execute()
        {
            Engine.RegisterAutoMapperStartup(new DefaultMapperConfig());
            Engine.RegisterAutoMapperStartup(new LogMapperConfig());

            LocalizationAttributeBootstrap.Initialize();
        }

        public int Order => 10;
    }
}
