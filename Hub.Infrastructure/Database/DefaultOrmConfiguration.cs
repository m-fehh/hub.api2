using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.MultiTenant.Interfaces;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

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
            _tenantManager.SetCurrentSchema(schema);

            // Aqui, você pode adicionar a lógica de configuração das tabelas para o schema corrente.
            // O código do OnModelCreating da sua classe ApplicationDbContext
            // deve incluir a configuração do esquema para cada tenant.
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

                // Consulta para buscar os schemas a partir da tabela ADM.Tenants
                var command = new SqlCommand("SELECT DISTINCT Schema FROM ADM.Tenants WHERE IsActive = 1", connection);

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


        // Cria migrações para todos os esquemas (tenants)
        public void CreateMigrationsForAllSchemas()
        {
            var schemas = GetAvailableSchemas(GetConnectionString());

            foreach (var schema in schemas)
            {
                // Configura o tenant corrente
                _tenantManager.SetCurrentSchema(schema);

                // Adiciona a migração para o esquema atual
                string migrationName = $"Migration{DateTime.Now:yyyyMMddHHmm}";
                RunMigrationCommand(migrationName);
            }
        }

        // Executa o comando dotnet ef migrations add para o esquema corrente
        private void RunMigrationCommand(string migrationName)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"ef migrations add {migrationName} --context ApplicationDbContext --output-dir Migrations/{migrationName} --project ../Hub.Api",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(processStartInfo);
            process.WaitForExit();
        }

        // Aplica as migrações para todos os esquemas (tenants)
        public void ApplyMigrationsForAllSchemas()
        {
            var schemas = GetAvailableSchemas(GetConnectionString());

            foreach (var schema in schemas)
            {
                // Configura o tenant corrente
                _tenantManager.SetCurrentSchema(schema);

                // Aplica a migração para o esquema atual
                ApplyMigrationForSchema();
            }
        }

        // Aplica a migração para o esquema atual
        private void ApplyMigrationForSchema()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "ef database update --context ApplicationDbContext",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(processStartInfo);
            process.WaitForExit();
        }
    }
}
