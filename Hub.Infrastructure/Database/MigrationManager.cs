using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.MultiTenant;
using System.Diagnostics;

namespace Hub.Infrastructure.Database
{
    public class MigrationManager : IMigrationManager
    {
        private readonly TenantManager _tenantManager;
        private readonly string _connectionString;

        public MigrationManager()
        {
            // Resolve TenantManager uma vez para evitar resoluções repetidas
            _tenantManager = Engine.Resolve<TenantManager>();
            _connectionString = Engine.ConnectionString("default");
        }

        // Cria migrações para todos os esquemas (tenants)
        public void CreateMigrationsForAllSchemas()
        {
            var schemas = GetAvailableSchemas();

            foreach (var schema in schemas)
            {
                // Configura o tenant corrente
                _tenantManager.SetCurrentSchema(schema);

                // Adiciona a migração para o esquema atual
                string migrationName = $"Migration{DateTime.Now:yyyyMMddHHmm}";
                CreateMigrationForSchema(migrationName);
            }
        }

        // Cria uma migração para um esquema específico
        public void CreateMigrationForSchema(string migrationName)
        {
            try
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
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start the migration process.");
                }

                // Captura a saída do processo
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Migration command failed: {error}");
                }

                Console.WriteLine(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running migration command: {ex.Message}");
            }
        }

        // Aplica as migrações para todos os esquemas (tenants)
        public void ApplyMigrationsForAllSchemas()
        {
            var schemas = GetAvailableSchemas();

            foreach (var schema in schemas)
            {
                // Configura o tenant corrente
                _tenantManager.SetCurrentSchema(schema);

                // Aplica a migração para o esquema atual
                ApplyMigrationForSchema(schema);
            }
        }

        // Aplica a migração para um esquema específico
        public void ApplyMigrationForSchema(string schemaName)
        {
            try
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
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start the update process.");
                }

                // Captura a saída do processo
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Database update command failed: {error}");
                }

                Console.WriteLine(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migration: {ex.Message}");
            }
        }

        // Método para obter a string de conexão
        private string GetConnectionString()
        {
            return _connectionString;
        }

        // Método para obter os schemas disponíveis
        private List<string> GetAvailableSchemas()
        {
            return _tenantManager.GetAvailableSchemas();
        }
    }
}
