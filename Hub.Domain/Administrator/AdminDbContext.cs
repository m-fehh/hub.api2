using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Models.Administrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hub.Domain.Administrator
{
    public class AdminDbContext : DbContext
    {
        public string ConnectionString => Engine.ConnectionString("adm");
        public string Schema => "admin";

        #region TABLES

        public DbSet<Tenant> Tenants { get; set; } = null!;

        public DbSet<TenantBinding> TenantBindings { get; set; } = null!;

        public DbSet<AppConfiguration> AppConfigurations { get; set; } = null!;

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Usa o schema específico para cada tenant na configuração de migração
            optionsBuilder.UseSqlServer(ConnectionString, o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schema));

            // Ignora o aviso de alterações pendentes no modelo
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<string>().HaveMaxLength(255);

            // Configura todas as propriedades do tipo Enum para serem armazenadas como string
            configurationBuilder.Properties<Enum>().HaveConversion<string>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura o schema do tenant
            modelBuilder.HasDefaultSchema(Schema);

            base.OnModelCreating(modelBuilder);
        }
    }
}


//add-migration "XXX" -Context AdminDbContext'.
//Update-Database -Context AdminDbContext