using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Infrastructure.Extensions
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
