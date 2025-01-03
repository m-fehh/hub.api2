namespace Hub.Infrastructure.Database.Interfaces
{
    public interface ITenantInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Subdomain { get; set; }
        public bool Inative { get; set; }
        public string Culture { get; set; }
        public string ConnectionString { get; set; }
        public string Schema { get; set; }
    }
}
