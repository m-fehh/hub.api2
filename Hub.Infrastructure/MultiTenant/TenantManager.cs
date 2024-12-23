using Hub.Infrastructure.MultiTenant.Interfaces;

namespace Hub.Infrastructure.MultiTenant
{
    public class TenantManager : ITenantManager
    {
        private string _currentSchema;
        private List<string> _availableSchemas;

        // Inicializa os schemas disponíveis
        public void InitializeTenants(List<string> schemas)
        {
            _availableSchemas = schemas;
        }

        // Configura o schema do tenant corrente
        public void SetCurrentSchema(string schema)
        {
            _currentSchema = schema;
        }

        // Obtém o schema do tenant corrente
        public string GetCurrentSchema()
        {
            return _currentSchema;
        }

        // Obtém todos os schemas disponíveis
        public List<string> GetAvailableSchemas()
        {
            return _availableSchemas;
        }
    }
}
