using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;

namespace Hub.Infrastructure.Database.MultiTenant
{
    public class TenantProvider : ITenantProvider
    {
        private readonly Dictionary<string, AdmClient> _tenantConfigurations;

        public string? CurrentTenant => Engine.Resolve<TenantLifeTimeScope>().CurrentTenantName;

        public TenantProvider()
        {
            var tenants = Singleton<ConfigurationTenant>.Instance.Mapeamentos[0].ConfigurationTenants;

            _tenantConfigurations = tenants.ToDictionary(tenant => tenant.Subdomain, tenant => new AdmClient
            {
                Id = tenant.TenantId,
                Name = tenant.TenantName,
                Subdomain = tenant.Subdomain,
                ConnectionString = tenant.ConnectionString ?? Engine.ConnectionString("default"),
                DefaultCulture = tenant.Culture,
                Schema = tenant.SchemaDefault
            });
        }

        public string? DbSchemaName => GetTenantConfiguration()?.Schema ?? null;

        public string? ConnectionString => GetTenantConfiguration()?.ConnectionString ?? null;

        public List<AdmClient> Tenants => _tenantConfigurations.Values.ToList();

        private AdmClient? GetTenantConfiguration()
        {
            if (CurrentTenant != null && _tenantConfigurations.TryGetValue(CurrentTenant, out var config))
            {
                return config;
            }

            return null; 
        }
    }
}
