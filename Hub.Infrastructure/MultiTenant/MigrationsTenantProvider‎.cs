using Hub.Infrastructure.MultiTenant.Interfaces;

namespace Hub.Infrastructure.MultiTenant
{
    public class MigrationsTenantProvider : ITenantProvider
    {
        public string? CurrentTenant => null;

        public string DbSchemaName => "adm";

        public string ConnectionString => Engine.ConnectionString("default");

        public IDisposable BeginScope(string tenant)
        {
            throw new NotImplementedException();
        }
    }
}
