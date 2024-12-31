using Autofac;
using Hub.Application.Hangfire;
using Hub.Application.Services;
using Hub.Domain.Entities;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure.Web.Interfaces;

namespace Hub.Application.Configurations
{
    public class DependencyRegistration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder) 
        {
            builder.RegisterType<HangfireStartup>().AsSelf().SingleInstance();


            #region SERVICES 

            builder.RegisterType<DocumentTypeService>().As<ICrudService<DocumentType>>();
            builder.RegisterType<DocumentTypeService>().AsSelf(); 

            #endregion

        }

        public int Order => 1;
    }
}
