using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;

namespace Hub.Migrator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTenantSupport(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITenantProvider, TenantProvider>();
            services.Configure<TenantConfigurationOptions>(configuration.GetSection(TenantConfigurationOptions.ConfigKey));

            return services;
        }
    }
}
