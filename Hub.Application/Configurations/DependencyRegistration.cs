using Autofac;
using Hub.Application.Hangfire;
using Hub.Application.Services;
using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture.Security.Interfaces;
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

            builder.RegisterType<PortalUserFingerprintService>().As<ICrudService<PortalUserFingerprint>>();
            builder.RegisterType<PortalUserFingerprintService>().AsSelf();

            builder.RegisterType<PortalUserPassHistoryService>().As<ICrudService<PortalUserPassHistory>>().AsSelf();
            builder.RegisterType<PortalUserPassHistoryService>().AsSelf();

            builder.RegisterType<UserProfileControlAccessService>().As<IUserProfileControlAccessService>();
            builder.RegisterType<UserProfileControlAccessService>().AsSelf();

            builder.RegisterType<UserService>().As<ICrudService<PortalUser>>().As<ISecurityProvider>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<LoginService>().AsSelf();

            #endregion

        }

        public int Order => 1;
    }
}
