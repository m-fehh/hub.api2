using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

namespace Hub.Infrastructure.MultiTenant.Teste
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTenantSupport(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITenantProvider, TenantProvider>();

            // Configuração correta usando a seção do configuration
            services.Configure<TenantConfigurationOptions>(opts =>
                configuration.GetSection(TenantConfigurationOptions.ConfigKey).Bind(opts)
            );

            return services;
        }

    }

    public static class ServiceCollectionInternalExtensions
    {
        public static IServiceCollection AddEntityFrameworkSqlServer<TContext>(this IServiceCollection services)
            where TContext : DbContext
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
    }

}
