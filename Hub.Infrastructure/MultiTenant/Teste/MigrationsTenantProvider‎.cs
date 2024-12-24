namespace Hub.Infrastructure.MultiTenant.Teste
{
    public class MigrationsTenantProvider : ITenantProvider
    {
        public string? CurrentTenant => null;

        public string DbSchemaName => "adm";

        public string ConnectionString => "Persist Security Info=True;Integrated Security=true;Server=.;Database=MultiTenantSample;";

        public IDisposable BeginScope(string tenant)
        {
            throw new NotImplementedException();
        }
    }
}
