﻿using Hub.Domain.Entities;
using Hub.Domain.Entities.Enterprise;
using Hub.Domain.Entities.Logs;
using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Hub.Domain.Entities.Enterprise.Incorporation;
using Hub.Infrastructure.Architecture;

public class EntityDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    #region CONFIGURATIONS SYSTEM 

    public DbSet<DocumentType> DocumentTypes { get; set; } = null!;

    public DbSet<ProfileGroup> ProfileGroups { get; set; } = null!;

    public DbSet<AccessRule> AccessRules { get; set; } = null!;

    #endregion

    #region USERS

    public DbSet<PortalUser> PortalUsers { get; set; } = null!;

    public DbSet<PortalUserSetting> PortalUserSettings { get; set; } = null!;

    public DbSet<PortalUserPassHistory> PortalUserPassHistories { get; set; } = null!;

    public DbSet<PortalUserFingerprint> PortalUserFingerprints { get; set; } = null!;

    public DbSet<Person> Persons { get; set; } = null!;

    public DbSet<UserRole> UserRoles { get; set; } = null!;

    #endregion

    #region LOGS

    public DbSet<Log> Logs { get; set; } = null!;

    public DbSet<LogField> LogFields { get; set; } = null!;

    #endregion

    #region INCORPORATION

    public DbSet<IncorporationEstablishment> IncorporationEstablishments { get; set; } = null!;

    public DbSet<IncorporationEstablishmentConfig> IncorporationEstablishmentConfigs { get; set; } = null!;

    #endregion

    #region ENTERPRISE

    public DbSet<OrganizationalStructure> OrganizationalStructures { get; set; } = null!;

    public DbSet<OrganizationalStructureConfig> OrganizationalStructureConfigs { get; set; } = null!;

    public DbSet<OrgStructConfigDefault> OrgStructConfigDefaults { get; set; } = null!;

    public DbSet<Establishment> Establishments { get; set; } = null!;

    #endregion

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

        configurationBuilder.Properties<string>().HaveMaxLength(255);

        // Configura todas as propriedades do tipo Enum para serem armazenadas como string
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configura o schema do tenant
        modelBuilder.HasDefaultSchema(_tenantProvider.DbSchemaName);

        ConfigureEntities(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configura as entidades relacionadas ao domínio de usuários.
    /// </summary>
    /// <param name="modelBuilder">Instância do ModelBuilder.</param>
    private void ConfigureEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PortalUser>()
            .HasOne(p => p.UserRole)
            .WithMany()
            .HasForeignKey(p => p.UserRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<IncorporationEstablishment>()
            .HasOne(e => e.OrganizationalStructure)
            .WithMany()
            .HasForeignKey(e => e.OrganizationalStructureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<IncorporationEstablishmentConfig>()
            .HasOne(e => e.OrganizationalStructure)
            .WithMany()
            .HasForeignKey(e => e.OrganizationalStructureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

//add-migration "XXX" -Context EntityDbContext
//Update-Database -Context EntityDbContext