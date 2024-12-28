namespace Hub.Infrastructure.Database.MultiTenant
{
    public class TenantConfigurationOptions
    {
        public class Tenant
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public bool IsActive { get; set; }
            public string DefaultCulture { get; set; }
            public string Schema { get; set; }
        }

        public ICollection<Tenant> Tenants { get; set; }
    }
}