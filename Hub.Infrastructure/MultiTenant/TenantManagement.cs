using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Hub.Infrastructure.MultiTenant;
using Hub.Infrastructure.MultiTenant.Interfaces;

namespace Hub.Infrastructure.MultiTenant
{
    public static class TenantManagement // Classe agora é estática
    {
        // Método de extensão para adicionar suporte a Tenant
        public static IServiceCollection AddTenantSupport(this IServiceCollection services)
        {
            services.AddSingleton<ITenantProvider, TenantProvider>();

            // Chamada ao método privado para buscar os tenants do banco de dados
            var tenants = GetActiveTenants();

            // Substituindo a configuração pela obtenção direta do banco de dados
            services.AddSingleton(new TenantConfigurationOptions
            {
                Tenants = tenants
            });

            return services;
        }

        // Método de extensão para adicionar suporte a Entity Framework com SQL Server
        public static IServiceCollection AddEntityFrameworkSqlServer<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddSingleton<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>();
            services.AddScoped<IMigrationsSqlGenerator, DbSchemaAwareSqlServerMigrationsSqlGenerator>();

            services.TryAddSingleton<ITenantProvider, TenantProvider>();

            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<TContext>((sp, options) =>
                    options.UseInternalServiceProvider(sp)
                );

            return services;
        }

        // Método privado que busca os tenants ativos do banco de dados
        private static List<TenantConfigurationOptions.Tenant> GetActiveTenants()
        {
            string connectionStringAdmin = Engine.ConnectionString("adm");

            var tenants = new List<TenantConfigurationOptions.Tenant>();

            if (!string.IsNullOrEmpty(connectionStringAdmin))
            {
                using (var connection = new SqlConnection(connectionStringAdmin))
                {
                    var query = @"
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

                    tenants = connection.Query<TenantConfigurationOptions.Tenant>(query).ToList();
                }
            }

            return tenants;
        }
    }
}
