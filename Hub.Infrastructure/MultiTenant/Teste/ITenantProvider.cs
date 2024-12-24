namespace Hub.Infrastructure.MultiTenant.Teste
{
    public interface ITenantProvider
    {
        string? CurrentTenant { get; }

        string DbSchemaName { get; }
        string ConnectionString { get; }

        IDisposable BeginScope(string tenant);
    }
}
