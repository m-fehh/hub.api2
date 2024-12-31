using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;

namespace Hub.Domain.Persistence
{
    public class MigrationsTenantProvider : ITenantProvider
    {
        public string? CurrentTenant { get; set; }

        public string DbSchemaName { get; set; }

        public string ConnectionString { get; set; }

        public List<AdmClient> Tenants { get; set; }
    }

}
