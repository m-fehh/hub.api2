using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Hub.Infrastructure.Database.Models;

namespace Hub.Domain.Entities.OrgStructure
{
    [Table("OrganizationalStructureConfig")]
    public class OrganizationalStructureConfig : BaseEntity, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        public virtual long OrganizationalStructureId { get; set; }

        [ForeignKey(nameof(OrganizationalStructureId))]
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }

        public virtual long OrgStructConfigDefaultId { get; set; }

        [ForeignKey(nameof(OrgStructConfigDefaultId))]
        public virtual OrgStructConfigDefault Config { get; set; }

        [Required]
        [StringLength(300)]
        public virtual string Value { get; set; }

        public virtual DateTime? CreationUTC { get; set; }

        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
