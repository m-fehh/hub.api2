using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hub.Domain.Persistence
{
    public static class DbContextServiceExtensions
    {
        public static IServiceCollection AddEntityFrameworkSqlServer<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddSingleton<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>();
            services.AddScoped<IMigrationsSqlGenerator, DbSchemaAwareSqlServerMigrationsSqlGenerator>();
            //services.TryAddSingleton<ITenantProvider, TenantProvider>();


            services.AddEntityFrameworkSqlServer().AddDbContext<TContext>((sp, options) => options.UseInternalServiceProvider(sp));
            return services;
        }
    }
}
