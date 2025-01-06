using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Hub.Infrastructure.Database.Models.Tenant;
using Hub.Infrastructure.Database.Interfaces;

namespace Hub.Domain.Entities.Users
{
    [Table("UserRole")]
    public class UserRole : BaseEntity, IEntityOrgStructOwned, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string Description { get; set; }

        public virtual long OwnerOrgStructId { get; set; }

        [ForeignKey(nameof(OwnerOrgStructId))]
        public virtual OrganizationalStructure OwnerOrgStruct { get; set; }

        [Required]
        public virtual bool Inactive { get; set; }

        public virtual string? ExternalCode { get; set; }

        public virtual DateTime? CreationUTC { get; set; }

        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
