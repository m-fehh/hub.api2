using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;
using Hub.Infrastructure.Database.Models.Administrator;

namespace Hub.Infrastructure.Database.MultiTenant
{
    public class TenantProvider : ITenantProvider
    {
        private readonly Dictionary<string, Tenant> _tenantConfigurations;

        public string? CurrentTenant => Engine.Resolve<TenantLifeTimeScope>().CurrentTenantName;

        public TenantProvider()
        {
            var tenants = Singleton<ConfigurationTenant>.Instance.Mapeamentos[0].ConfigurationTenants;

            _tenantConfigurations = tenants.ToDictionary(tenant => tenant.Subdomain, tenant => new Tenant
            {
                Id = tenant.TenantId,
                Name = tenant.TenantName,
                Subdomain = tenant.Subdomain,
                ConnectionString = tenant.ConnectionString ?? Engine.ConnectionString("default"),
                Culture = tenant.Culture,
                Schema = tenant.SchemaDefault
            });
        }

        public string? DbSchemaName => GetTenantConfiguration()?.Schema ?? null;

        public string? ConnectionString => GetTenantConfiguration()?.ConnectionString ?? null;

        public List<Tenant> Tenants => _tenantConfigurations.Values.ToList();

        private Tenant? GetTenantConfiguration()
        {
            if (CurrentTenant != null && _tenantConfigurations.TryGetValue(CurrentTenant, out var config))
            {
                return config;
            }

            return null; 
        }
    }
}
