using Autofac;
using Hub.Application.Hangfire;
using Hub.Application.Services;
using Hub.Domain.Administrator;
using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Configurations
{
    public class DependencyRegistration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder) 
        {
            builder.RegisterType<HangfireStartup>().AsSelf().SingleInstance();
            builder.RegisterType<HubTenantNameProvider>().AsSelf().SingleInstance();

            #region SERVICES TENANTS

            builder.RegisterType<DocumentTypeService>().As<IOrchestratorService<DocumentType>>();
            builder.RegisterType<DocumentTypeService>().AsSelf();

            builder.RegisterType<PortalUserFingerprintService>().As<IOrchestratorService<PortalUserFingerprint>>();
            builder.RegisterType<PortalUserFingerprintService>().AsSelf();

            builder.RegisterType<PortalUserPassHistoryService>().As<IOrchestratorService<PortalUserPassHistory>>().AsSelf();
            builder.RegisterType<PortalUserPassHistoryService>().AsSelf();

            builder.RegisterType<AccessRuleService>().As<IOrchestratorService<AccessRule>>();
            builder.RegisterType<AccessRuleService>().AsSelf();

            builder.RegisterType<UserProfileControlAccessService>().As<IUserProfileControlAccessService>();
            builder.RegisterType<UserProfileControlAccessService>().AsSelf();

            builder.RegisterType<UserService>().As<IOrchestratorService<PortalUser>>().As<ISecurityProvider>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<LoginService>().AsSelf();

            builder.RegisterType<AdminDbContext>().As<DbContext>().InstancePerLifetimeScope();

            #endregion

        }

        public int Order => 1;
    }
}
