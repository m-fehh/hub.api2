using Hub.Infrastructure.Database.Interfaces;

namespace Hub.Infrastructure.Database.Models
{
    public class AdmClient : ITenantInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Subdomain { get; set; }
        public bool IsActive { get; set; } = true;
        public string DefaultCulture { get; set; }
        public string ConnectionString { get; set; }
        public string Schema { get; set; }
    }
}
