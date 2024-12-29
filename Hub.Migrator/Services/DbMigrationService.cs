using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database;
using Microsoft.Extensions.Logging;

public class DbMigrationService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ITenantProvider _tenantProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbMigrationService> _logger;

    public DbMigrationService(
        IHostApplicationLifetime hostApplicationLifetime,
        ITenantProvider tenantProvider,
        IServiceProvider serviceProvider,
        ILogger<DbMigrationService> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _tenantProvider = tenantProvider;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando migração do banco de dados...");
            await MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao migrar banco de dados.");
            throw;
        }

        _logger.LogInformation("Migração concluída com sucesso.");
        _hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task MigrateAsync()
    {
        var tenants = _tenantProvider.Tenants;

        foreach (var tenant in tenants)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (Engine.BeginLifetimeScope(tenant.Subdomain))
                {
                    _logger.LogInformation($"Iniciando migração para o tenant {tenant.Name}...");

                    try
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<EntityDbContext>();
                        await dbContext.Database.MigrateAsync();

                        _logger.LogInformation($"Migração concluída para o tenant {tenant.Name} (schema: {_tenantProvider.DbSchemaName})");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erro ao migrar o tenant {tenant.Name}");
                    }
                }
            }
        }
    }
}
