using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hub.Domain.Entities.OrganizationalStructure
{
    [Table("OrganizationalStructureConfig")]
    public class OrganizationalStructureConfig : BaseEntity, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        public virtual long OrganizationalStructureId { get; set; }

        [ForeignKey(nameof(OrganizationalStructureId))]
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }

        [Required]
        [StringLength(300)]
        public virtual string Value { get; set; }

        public virtual DateTime? CreationUTC { get; set; }

        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
