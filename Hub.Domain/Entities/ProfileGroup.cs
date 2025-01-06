using Hub.Domain.Enums;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities
{
    [Table("ProfileGroup")]
    public class ProfileGroup : BaseEntity, IProfileGroup
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
        [JsonConverter(typeof(ConcreteListTypeConverter<IAccessRule, AccessRule>))]
        public virtual ICollection<IAccessRule> Rules { get; set; }

        [Required]
        public virtual bool Administrator { get; set; }

        public virtual EPasswordExpirationDays PasswordExpirationDays { get; set; }

        [Required]
        public virtual bool AllowMultipleAccess { get; set; }
    }
}
