using Hub.Infrastructure.Database.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hub.Domain.Persistence
{
    public class DbSchemaAwareModelCacheKeyFactory : IModelCacheKeyFactory
    {
        private readonly ITenantProvider _tenantProvider;

        public DbSchemaAwareModelCacheKeyFactory(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        public object Create(DbContext context, bool designTime)
        {
            return Tuple.Create(context.GetType(), _tenantProvider.DbSchemaName, designTime);
        }
    }
}
