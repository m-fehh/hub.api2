namespace Hub.Infrastructure.MultiTenant.Interfaces
{
    public interface ITenantManager
    {
        void InitializeTenants(List<string> schemas); // Inicializa os tenants (schemas) disponíveis
        void SetCurrentSchema(string schema); // Configura o schema atual para o tenant logado
        string GetCurrentSchema(); // Obtém o schema configurado
        List<string> GetAllSchemas(); // Retorna a lista de todos os schemas
    }
}
