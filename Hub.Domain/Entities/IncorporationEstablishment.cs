using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities
{
    [Table("IncorporationEstablishment")]
    public class IncorporationEstablishment : BaseEntity
    {
        public override long Id { get; set; }

        public virtual long EstablishmentId { get; set; }

        [ForeignKey(nameof(EstablishmentId))]
        public virtual Establishment Establishment { get; set; }

        public virtual long OrganizationalStructureId { get; set; }

        [ForeignKey(nameof(OrganizationalStructureId))]
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }

        public virtual long PortalUserId { get; set; }

        [ForeignKey(nameof(PortalUserId))]
        public virtual PortalUser PortalUser { get; set; }

        [Required]
        [StringLength(20)]
        public virtual string CNPJ { get; set; }

        public virtual DateTime IncorporationDate { get; set; }
    }
}
