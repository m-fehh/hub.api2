using Hub.Infrastructure.MultiTenant.Interfaces;

namespace Hub.Infrastructure.MultiTenant
{
    public class TenantManager : ITenantManager
    {
        private List<string> _schemas = new();
        private string _currentSchema;

        public void InitializeTenants(List<string> schemas)
        {
            _schemas = schemas;
        }

        public void SetCurrentSchema(string schema)
        {
            if (!_schemas.Contains(schema))
            {
                throw new ArgumentException($"Schema '{schema}' não encontrado na lista de tenants disponíveis.");
            }

            _currentSchema = schema;
        }

        public string GetCurrentSchema()
        {
            if (string.IsNullOrEmpty(_currentSchema))
            {
                throw new InvalidOperationException("Nenhum schema foi configurado.");
            }

            return _currentSchema;
        }

        public List<string> GetAllSchemas()
        {
            return _schemas;
        }
    }
}
