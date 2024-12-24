using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hub.Infrastructure.MultiTenant;

namespace Hub.Infrastructure.MultiTenant
{
    public static class TenantManagement
    {
        public static IServiceCollection AddTenantSupport(this IServiceCollection services)
        {
            var tenants = GetActiveTenants();

            services.AddSingleton(new TenantConfigurationOptions
            {
                Tenants = tenants
            });

            return services;
        }

        public static IServiceCollection AddEntityFrameworkSqlServer<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddEntityFrameworkSqlServer().AddDbContext<TContext>((sp, options) =>options.UseInternalServiceProvider(sp));
            return services;
        }

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
