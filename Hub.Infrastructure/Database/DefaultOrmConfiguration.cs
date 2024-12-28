using Dapper;
using Hub.Infrastructure;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using static Hub.Infrastructure.Database.MultiTenant.TenantConfigurationOptions;

public class DefaultOrmConfiguration : IOrmConfiguration
{
    private readonly IOptions<TenantConfigurationOptions> _tenantConfigOptions;

    public DefaultOrmConfiguration(IOptions<TenantConfigurationOptions> tenantConfigOptions)
    {
        _tenantConfigOptions = tenantConfigOptions;
    }

    public async void Configure()
    {
        var tenantOptions = _tenantConfigOptions.Value;

        using (var connection = new SqlConnection(Engine.ConnectionString("adm")))
        {
            const string query = @"
                SELECT 
                    Id, 
                    Name, 
                    ConnectionString, 
                    IsActive, 
                    DefaultCulture
                FROM 
                    Tenants
                WHERE 
                    IsActive = 1";

            var tenants = (await connection.QueryAsync<Tenant>(query)).ToList();

            // Atualiza as opções com os tenants carregados
            tenantOptions.Tenants = tenants;
        }
    }
}
