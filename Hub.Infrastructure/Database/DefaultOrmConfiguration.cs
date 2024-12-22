using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.MultiTenant.Interfaces;
using Microsoft.Data.SqlClient;

namespace Hub.Infrastructure.Database
{
    public class DefaultOrmConfiguration : IOrmConfiguration
    {
        private readonly ITenantManager _tenantManager;

        public DefaultOrmConfiguration(ITenantManager tenantManager)
        {
            _tenantManager = tenantManager;
        }

        public void Configure()
        {
            string connectionString = GetConnectionString();

            // Captura todos os schemas (tenants) disponíveis
            var schemas = GetAvailableSchemas(connectionString);

            // Inicializa o TenantManager com os schemas capturados
            _tenantManager.InitializeTenants(schemas);
        }

        public void ConfigureTenant(string tenantSchema, ConnectionStringBaseVM config)
        {
            // Configura o schema do tenant logado
            _tenantManager.SetCurrentSchema(tenantSchema);
        }

        private string GetConnectionString()
        {
            return Engine.ConnectionString("DefaultConnection");
        }

        private List<string> GetAvailableSchemas(string connectionString)
        {
            var schemas = new List<string>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        schemas.Add(reader.GetString(0));
                    }
                }
            }
            return schemas;
        }
    }
}