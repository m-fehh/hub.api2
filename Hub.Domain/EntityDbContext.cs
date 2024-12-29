using Hub.Domain.Entities;
using Hub.Infrastructure.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;

public class EntityDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Teste> Testes { get; set; } = null!;

    public EntityDbContext(DbContextOptions options, ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Usa o schema específico para cada tenant na configuração de migração
        optionsBuilder.UseSqlServer(_tenantProvider.ConnectionString, o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, _tenantProvider.DbSchemaName));

        // Ignora o aviso de alterações pendentes no modelo
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<string>()
            .HaveMaxLength(255);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configura o schema do tenant
        modelBuilder.HasDefaultSchema(_tenantProvider.DbSchemaName);

        base.OnModelCreating(modelBuilder);

        ConfigureCustomer(modelBuilder);
    }

    private static void ConfigureCustomer(ModelBuilder builder)
    {
        builder.Entity<Customer>(b =>
        {
            var table = b.ToTable("Customers");

            table.Property(p => p.CustomerId).ValueGeneratedOnAdd();
            table.HasKey(p => p.CustomerId).HasName("PK_CustomerId");
        });
    }
}
