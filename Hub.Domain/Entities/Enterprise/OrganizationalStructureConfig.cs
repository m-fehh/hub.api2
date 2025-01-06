using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Models.Tenant;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities.Enterprise
{
    [Table("OrganizationalStructureConfig")]
    public class OrganizationalStructureConfig : BaseEntity, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        public virtual long OrganizationalStructureId { get; set; }

        [ForeignKey(nameof(OrganizationalStructureId))]
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }

        public virtual long ConfigId { get; set; }

        [ForeignKey(nameof(ConfigId))]
        public virtual OrgStructConfigDefault Config { get; set; }

        [Required]
        [StringLength(300)]
        public virtual string Value { get; set; }



        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
