using Hub.Infrastructure.Database.Models;
using Hub.Infrastructure.Database.Models.Administrator;

namespace Hub.Infrastructure.Database.Interfaces
{
    public interface ITenantProvider
    {
        string? CurrentTenant { get; }

        string DbSchemaName { get; }

        string ConnectionString { get; }

        public List<Tenant> Tenants { get; }
    }
}
