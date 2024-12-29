using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;

namespace Hub.Migrator.Services
{
    public static class TenantServiceExtensions
    {
        public static IServiceCollection AddTenantSupport(this IServiceCollection services)
        {
            services.AddSingleton<ITenantProvider, TenantProvider>();
            return services;
        }
    }
}
