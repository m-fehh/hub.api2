using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hub.Infrastructure.Database.Models.Tenant;
using Hub.Infrastructure.Database.Interfaces;

namespace Hub.Domain.Entities.Users
{
    [Table("Person")]
    public class Person : BaseEntity, IEntityOrgStructBased, IEntityOrgStructOwned, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Name { get; set; }

        [StringLength(100)]
        public virtual string Document { get; set; }

        public virtual long OwnerOrgStructId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(OwnerOrgStructId))]
        public virtual OrganizationalStructure OwnerOrgStruct { get; set; }

        [NotMapped]
        public virtual ICollection<OrganizationalStructure> OrganizationalStructures { get; set; }

        [StringLength(100)]
        public virtual string ExternalCode { get; set; }

        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }


        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
