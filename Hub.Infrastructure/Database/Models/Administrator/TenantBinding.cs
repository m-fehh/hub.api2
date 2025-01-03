using Hub.Infrastructure.Database.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Infrastructure.Database.Models.Administrator
{
    [Table("TenantBinding")]
    public class TenantBinding : BaseEntity
    {
        [Key]
        public override long Id { get; set; } 

        public virtual long TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public virtual Tenant Tenant { get; set; }

        [NotMapped]
        public string TenantName => Tenant?.Name;

        public virtual string Url { get; set; }
    }
}
