using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class DbMigrationService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ITenantProvider _tenantProvider;
    private readonly IServiceProvider _serviceProvider;

    public DbMigrationService(IHostApplicationLifetime hostApplicationLifetime, ITenantProvider tenantProvider, IServiceProvider serviceProvider)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _tenantProvider = tenantProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao migrar banco de dados: {ex.Message}");
            throw;
        }

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
                    try
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<EntityDbContext>();
                        await dbContext.Database.MigrateAsync();

                        Console.WriteLine($"Migração concluída para o tenant {tenant.Name} (schema: {_tenantProvider.DbSchemaName})");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao migrar o tenant {tenant.Name}: {ex.Message}");
                        throw;
                    }
                }
            }
        }
    }
}
