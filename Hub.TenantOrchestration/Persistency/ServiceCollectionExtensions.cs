using Hub.TenantOrchestration.TenantSupport;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.TenantOrchestration.Persistency
{
    public static class ServiceCollectionExtensions
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
