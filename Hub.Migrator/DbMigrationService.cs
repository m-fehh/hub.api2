using Microsoft.EntityFrameworkCore;
using Hub.Infrastructure.Architecture;
using Hub.Migrator.Seeders;

public class DbMigrationService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbMigrationService> _logger;

    public DbMigrationService(IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider, ILogger<DbMigrationService> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
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
        await Singleton<LoopTenantManager>.Instance.LoopTenants("MigrateAsync", async () =>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<EntityDbContext>();
                    await dbContext.Database.MigrateAsync();

                    await ExecuteSeedDataAsync(dbContext);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao migrar o banco de dados.");
                }
            }
        },
        (logType, message) =>
        {
            if (logType.Equals("info", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation(message);
            }
            else if (logType.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError(message);
            }
        });
    }


    private async Task ExecuteSeedDataAsync(EntityDbContext dbContext)
    {
        var seederExecutor = new SeederExecutor(dbContext);
        await seederExecutor.RunSeedersAsync();
    }
}
