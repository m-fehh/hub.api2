namespace Hub.Infrastructure.Database.Interfaces
{
    public interface ITenantInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Subdomain { get; set; }
        public bool IsActive { get; set; }
        public string DefaultCulture { get; set; }
        public string ConnectionString { get; set; }
    }
}
