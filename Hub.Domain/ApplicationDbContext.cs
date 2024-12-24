//using Microsoft.EntityFrameworkCore;
//using Hub.Domain.Entities;
//using Hub.Infrastructure.MultiTenant.Interfaces;

//namespace Hub.Domain
//{
//    public class ApplicationDbContext : DbContext
//    {
//        private readonly ITenantManager _tenantManager;

//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantManager tenantManager)
//            : base(options)
//        {
//            _tenantManager = tenantManager;
//        }

//        public DbSet<Tenant> Tenants { get; set; }

//        // Configura o modelo para cada esquema (tenant) dinamicamente
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // Configura tabelas para o schema de admin (ADM)
//            ConfigureAdminSchema(modelBuilder);

//            // Obtém todos os esquemas de tenants disponíveis
//            var schemas = _tenantManager.GetAvailableSchemas();

//            // Para cada schema de tenant, garante que o esquema existe e configura as tabelas de tenant
//            foreach (var schema in schemas)
//            {
//                // Garante que o esquema do tenant exista no banco de dados
//                EnsureSchemaExists(schema);

//                // Configura as entidades para o schema de cada tenant
//                ConfigureTenantSchema(modelBuilder, schema);
//            }
//        }

//        // Método que configura as tabelas para o schema de admin (ADM)
//        private void ConfigureAdminSchema(ModelBuilder modelBuilder)
//        {
//            const string adminSchema = "ADM"; // O nome do schema de admin

//            // Garante que o schema de admin existe no banco de dados
//            EnsureSchemaExists(adminSchema);

//            modelBuilder.Entity<Tenant>(entity =>
//            {
//                // Configura a tabela "Tenants" para o schema de admin
//                entity.ToTable("Tenants", adminSchema);
//                entity.HasKey(e => e.Id);
//                entity.Property(e => e.CreationUTC).HasDefaultValueSql("GETUTCDATE()");
//                entity.Property(e => e.LastUpdateUTC).HasDefaultValueSql("GETUTCDATE()");
//            });
//        }

//        // Método que configura as tabelas para o schema de cada tenant
//        private void ConfigureTenantSchema(ModelBuilder modelBuilder, string schema)
//        {
//            //    modelBuilder.Entity<Tenant>(entity =>
//            //    {
//            //        // Configura a tabela "Tenants" para o schema do tenant
//            //        entity.ToTable("Tenants", schema);
//            //        entity.HasKey(e => e.Id);
//            //        entity.Property(e => e.CreationUTC).HasDefaultValueSql("GETUTCDATE()");
//            //        entity.Property(e => e.LastUpdateUTC).HasDefaultValueSql("GETUTCDATE()");
//            //    });
//        }

//        // Método que verifica se o esquema existe e, caso não, cria o esquema no banco de dados
//        private void EnsureSchemaExists(string schema)
//        {
//            using (var connection = this.Database.GetDbConnection())
//            {
//                connection.Open();
//                var command = connection.CreateCommand();

//                // Comando SQL para verificar a existência do esquema
//                command.CommandText = $@"
//                    IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = '{schema}')
//                    BEGIN
//                        EXEC('CREATE SCHEMA [{schema}]');
//                    END";

//                // Executa o comando
//                command.ExecuteNonQuery();
//            }
//        }

//        // Método que muda o schema atual para o tenant ativo
//        public void SetTenantSchema(string schema)
//        {
//            _tenantManager.SetCurrentSchema(schema);
//        }
//    }
//}


using Microsoft.EntityFrameworkCore;
using Hub.Domain.Entities;

namespace Hub.Domain
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }

        // Configura o modelo para cada esquema (tenant) dinamicamente
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
