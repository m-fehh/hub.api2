using Hub.Infrastructure.Database.Models;

namespace Hub.Infrastructure.Database.Interfaces
{
    public interface ITenantProvider
    {
        string? CurrentTenant { get; }

        string DbSchemaName { get; }

        string ConnectionString { get; }

        public List<AdmClient> Tenants { get; }
    }
}
