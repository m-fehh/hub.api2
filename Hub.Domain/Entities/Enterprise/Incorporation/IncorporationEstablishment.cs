using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Models.Tenant;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities.Enterprise.Incorporation
{
    [Table("IncorporationEstablishment")]
    public class IncorporationEstablishment : BaseEntity
    {
        [Key]
        public override long Id { get; set; }

        public virtual long EstablishmentId { get; set; }

        [ForeignKey(nameof(EstablishmentId))]

        public virtual Establishment Establishment { get; set; }

        public virtual long OrganizationalStructureId { get; set; }

        [ForeignKey(nameof(OrganizationalStructureId))]
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }

        public virtual long UserId { get; set; }

        [ForeignKey(nameof(UserId))]

        public virtual PortalUser User { get; set; }

        public virtual string CNPJ { get; set; }

        public virtual DateTime IncorporationDate { get; set; }
    }
}
