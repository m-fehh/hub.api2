using Hub.Domain.Enums;
using Hub.Infrastructure.Architecture.Logger.Interfaces;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities
{
    [Table("ProfileGroup")]
    public class ProfileGroup : BaseEntity, IProfileGroup, ILogableEntity
    {
        public ProfileGroup()
        {
            PasswordExpirationDays = EPasswordExpirationDays.Ninety;
        }

        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Name { get; set; }

        [NotMapped]
        public virtual ICollection<IAccessRule> Rules { get; set; }

        public virtual long OwnerOrgStructId { get; set; }

        [ForeignKey(nameof(OwnerOrgStructId))]
        public virtual OrganizationalStructure OwnerOrgStruct { get; set; }

        [Required]
        public virtual bool Administrator { get; set; }

        public virtual EPasswordExpirationDays PasswordExpirationDays { get; set; }

        [Required]
        public virtual bool AllowMultipleAccess { get; set; }
    }
}
