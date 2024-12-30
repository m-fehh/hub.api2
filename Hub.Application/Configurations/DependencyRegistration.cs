using Autofac;
using Hub.Application.Hangfire;
using Hub.Infrastructure.DependencyInjection.Interfaces;

namespace Hub.Application.Configurations
{
    public class DependencyRegistration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder) 
        {
            builder.RegisterType<HangfireStartup>().AsSelf().SingleInstance();

        }

        public int Order => 1;
    }
}
