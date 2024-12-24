using Hub.TenantOrchestration.TenantSupport;

namespace Hub.TenantOrchestration.Persistency
{
    internal class DbSchemaAwareModelCacheKeyFactory : IModelCacheKeyFactory
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
