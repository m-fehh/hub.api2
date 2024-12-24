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

        // Configura o Entity Framework e os esquemas para os tenants
        public void Configure()
        {
            string connectionString = GetConnectionString();

            // Captura todos os esquemas (tenants) disponíveis
            var schemas = GetAvailableSchemas(connectionString);

            // Inicializa o TenantManager com os schemas capturados
            _tenantManager.InitializeTenants(schemas);

            // Configura cada esquema (tenant) no DbContext
            foreach (var schema in schemas)
            {
                ConfigureTenantSchema(schema);
            }
        }

        // Configura o DbContext para o tenant especificado
        private void ConfigureTenantSchema(string schema)
        {
            // Configura o schema atual para o tenant
            _tenantManager.SetCurrentSchema(schema);
        }


        // Retorna a string de conexão
        private string GetConnectionString()
        {
            return Engine.ConnectionString("default"); 
        }

        // Obtém os esquemas disponíveis no banco de dados
        private List<string> GetAvailableSchemas(string connectionString)
        {
            var schemas = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Consulta para buscar os esquemas a partir da tabela ADM.Tenants
                var command = new SqlCommand("SELECT DISTINCT [Schema] FROM ADM.Tenants WHERE IsActive = 1", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Adiciona o schema retornado à lista
                        schemas.Add(reader.GetString(0));
                    }
                }
            }

            return schemas;
        }
    }
}
