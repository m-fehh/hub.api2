using Hub.Infrastructure.Architecture.Logger.Interfaces;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Hub.Domain.Enums;
using TimeZoneConverter;
using Hub.Infrastructure.Database.Models.Tenant;

namespace Hub.Domain.Entities.Enterprise
{
    [Table("Establishment")]
    public class Establishment : BaseEntity, ILogableEntity, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        public virtual long OrganizationalStructureId { get; set; }

        [ForeignKey(nameof(OrganizationalStructureId))]
        public virtual OrganizationalStructure OrganizationalStructure { get; set; }

        [Required]
        [StringLength(20)]
        public virtual string CNPJ { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string SocialName { get; set; }

        [StringLength(500)]
        public virtual string? CommercialName { get; set; }

        [StringLength(15)]
        public virtual string? CommercialAbbrev { get; set; }

        [StringLength(15)]
        public virtual string? OpeningTime { get; set; }

        public virtual string? TimezoneIdentifier { get; set; }

        public virtual int? TimeZoneDifference { get; set; }

        [StringLength(15)]
        public virtual string? ClosingTime { get; set; }

        public virtual EEstablishmentClassifier EstablishmentClassifier { get; set; }

        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }

        public virtual string GetTimezone()
        {
            return TZConvert.WindowsToIana(TimezoneIdentifier);
        }
    }
}
