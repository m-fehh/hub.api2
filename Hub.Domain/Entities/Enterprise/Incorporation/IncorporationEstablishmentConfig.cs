using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Models.Tenant;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities.Enterprise.Incorporation
{
    [Table("IncorporationEstablishmentConfig")]
    public class IncorporationEstablishmentConfig : BaseEntity
    {
        [Key]
        public override long Id { get; set; }

        public virtual long IncorporationEstablishmentId { get; set; }

        [ForeignKey(nameof(IncorporationEstablishmentId))]
        public virtual IncorporationEstablishment IncorporationEstablishment { get; set; }

        public virtual long OrganizationalStructureId { get; set; }

        [ForeignKey(nameof(OrganizationalStructureId))]
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }

        public virtual long ConfigId { get; set; }

        [ForeignKey(nameof(ConfigId))]
        public virtual OrgStructConfigDefault Config { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Value { get; set; }
    }
}
