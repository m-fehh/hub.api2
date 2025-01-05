using Hub.Infrastructure.Architecture.Logger.Interfaces;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Hub.Infrastructure.Database.Models;
using Hub.Domain.Enums;
using System;
using TimeZoneConverter;

namespace Hub.Domain.Entities
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

        public virtual string StateRegistration { get; set; }

        public virtual string MunicipalRegistration { get; set; }

        public virtual ETaxRegimeCategory TaxRegimeCode { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string SocialName { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string CommercialName { get; set; }

        [Required]
        [StringLength(15)]
        public virtual string PostalCode { get; set; }

        [StringLength(15)]
        public virtual string OpeningTime { get; set; }        
        
        [StringLength(15)]
        public virtual string ClosingTime { get; set; }

        public virtual DateTime? SystemStartDate { get; set; }

        public virtual DateTime? SystemEndDate { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string Description { get; set; }

        public virtual double? AddressLat { get; set; }

        public virtual double? AddressLng { get; set; }

        [StringLength(120)]
        public virtual string Timezone { get; set; }

        public virtual int? TimezoneOffset { get; set; }

        public virtual string GetTimezone()
        {
            return TZConvert.WindowsToIana(Timezone);
        }

        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
