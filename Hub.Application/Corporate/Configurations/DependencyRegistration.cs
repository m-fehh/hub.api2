using Autofac;
using Hub.Application.Corporate.Hangfire;
using Hub.Application.Corporate.Interfaces;
using Hub.Application.Corporate.Manager;
using Hub.Application.Services;
using Hub.Application.Services.Enterprise;
using Hub.Application.Services.Enterprise.Incorporation;
using Hub.Application.Services.Users;
using Hub.Domain.Administrator;
using Hub.Domain.Entities;
using Hub.Domain.Entities.Enterprise;
using Hub.Domain.Entities.Enterprise.Incorporation;
using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Architecture.Security.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models.Tenant;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Corporate.Configurations
{
    public class DependencyRegistration : IDependencyConfiguration
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<AdminDbContext>().As<DbContext>().InstancePerLifetimeScope();

            builder.RegisterType<HangfireStartup>().AsSelf().SingleInstance();
            builder.RegisterType<CurrentTenantProvider>().AsSelf().SingleInstance();
            builder.RegisterType<PortalCacheManager>().AsSelf().SingleInstance();
            builder.RegisterType<PortalCacheRequest>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<TimezoneManager>().As<ICurrentTimezone>().SingleInstance();

            #region SERVICES TENANTS

            builder.RegisterType<AccessRuleService>().As<IOrchestratorService<AccessRule>>();
            builder.RegisterType<AccessRuleService>().AsSelf();

            builder.RegisterType<DocumentTypeService>().As<IOrchestratorService<DocumentType>>();
            builder.RegisterType<DocumentTypeService>().AsSelf();

            builder.RegisterType<ProfileGroupService>().As<IOrchestratorService<ProfileGroup>>();
            builder.RegisterType<ProfileGroupService>().AsSelf();

            builder.RegisterType<OrganizationalStructureService>().As<IOrchestratorService<OrganizationalStructure>>();
            builder.RegisterType<OrganizationalStructureService>().AsSelf();
            builder.RegisterType<OrganizationalStructureService>().As<IOrgStructBasedService>();
            builder.RegisterType<OrganizationalStructureService>().As<IOrganizationalStructureService>();

            builder.RegisterType<OrganizationalStructureConfigService>().As<IOrchestratorService<OrganizationalStructureConfig>>();
            builder.RegisterType<OrganizationalStructureConfigService>().AsSelf();

            builder.RegisterType<EstablishmentService>().As<IOrchestratorService<Establishment>>();
            builder.RegisterType<EstablishmentService>().AsSelf();

            builder.RegisterType<IncorporationEstablishmentService>().As<IOrchestratorService<IncorporationEstablishment>>();
            builder.RegisterType<IncorporationEstablishmentService>().AsSelf();

            builder.RegisterType<IncorporationEstablishmentConfigService>().As<IOrchestratorService<IncorporationEstablishmentConfig>>();
            builder.RegisterType<IncorporationEstablishmentConfigService>().AsSelf();

            builder.RegisterType<LogService>().AsSelf();
            builder.RegisterType<LoginService>().AsSelf();

            builder.RegisterType<PersonService>().As<IOrchestratorService<Person>>();
            builder.RegisterType<PersonService>().AsSelf();

            builder.RegisterType<PortalUserFingerprintService>().As<IOrchestratorService<PortalUserFingerprint>>();
            builder.RegisterType<PortalUserFingerprintService>().AsSelf();

            builder.RegisterType<PortalUserPassHistoryService>().As<IOrchestratorService<PortalUserPassHistory>>().AsSelf();
            builder.RegisterType<PortalUserPassHistoryService>().AsSelf();

            builder.RegisterType<PortalUserSettingService>().As<IOrchestratorService<PortalUserSetting>>();
            builder.RegisterType<PortalUserSettingService>().As<IUserSettingManager>();

            builder.RegisterType<UserKeywordService>().AsSelf();

            builder.RegisterType<UserProfileControlAccessService>().As<IUserProfileControlAccessService>();
            builder.RegisterType<UserProfileControlAccessService>().AsSelf();

            builder.RegisterType<UserService>().As<IOrchestratorService<PortalUser>>().As<ISecurityProvider>().AsSelf().InstancePerLifetimeScope();

            #endregion

        }

        public int Order => 1;
    }
}
