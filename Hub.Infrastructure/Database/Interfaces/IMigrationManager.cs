namespace Hub.Infrastructure.Database.Interfaces
{
    public interface IMigrationManager
    {
        /// <summary>
        /// Cria as migrações para todos os esquemas (tenants).
        /// </summary>
        void CreateMigrationsForAllSchemas();

        /// <summary>
        /// Aplica as migrações para todos os esquemas (tenants).
        /// </summary>
        void ApplyMigrationsForAllSchemas();

        /// <summary>
        /// Cria uma migração para um esquema específico.
        /// </summary>
        /// <param name="migrationName">Nome da migração.</param>
        void CreateMigrationForSchema(string migrationName);

        /// <summary>
        /// Aplica a migração para um esquema específico.
        /// </summary>
        void ApplyMigrationForSchema(string schemaName);
    }
}
