using Hub.Infrastructure;
using Hub.Infrastructure.Database.Interfaces;

namespace Hub.Domain.Persistence
{
    public class MigrationsTenantProvider : ITenantProvider
    {
        public string? CurrentTenant => null;

        public string DbSchemaName => "dbo";

        public string ConnectionString => Engine.ConnectionString("default");

        public IDisposable BeginScope(string tenant)
        {
            throw new NotImplementedException();
        }
    }
}
