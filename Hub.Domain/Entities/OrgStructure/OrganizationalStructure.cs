using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Hub.Infrastructure.Architecture.Logger.Interfaces;

namespace Hub.Domain.Entities.OrgStructure
{
    [Table("OrganizationalStructure")]
    public class OrganizationalStructure : BaseEntity, ILogableEntity, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(10)]
        public virtual string Abbrev { get; set; }


        [Required]
        [StringLength(150)]
        public virtual string Description { get; set; }

        public virtual bool Inactive { get; set; }

        public virtual bool IsRoot { get; set; }

        public virtual bool IsLeaf { get; set; }

        public virtual bool IsDomain { get; set; }

        public virtual long? FatherId { get; set; }

        [ForeignKey(nameof(FatherId))]
        public virtual OrganizationalStructure Father { get; set; }

        public virtual ICollection<OrganizationalStructure> Childrens { get; set; }

        [MaxLength(100)]
        public virtual string? ExternalCode { get; set; }

        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
