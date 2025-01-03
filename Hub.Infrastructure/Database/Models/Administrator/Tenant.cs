using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Infrastructure.Database.Models.Administrator
{
    [Table("Tenants")]
    public class Tenant : BaseEntity, ITenantInfo
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string Subdomain { get; set; }

        public virtual bool Inative { get; set; }

        [Required]
        public virtual string Culture { get; set; }

        public virtual string? ConnectionString { get; set; }

        [NotMapped]
        public string? Schema { get; set; }
    }
}
