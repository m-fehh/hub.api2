using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hub.Domain.Entities.Users
{
    [Table("PortalUserSettings")]
    public class PortalUserSetting : BaseEntity
    {
        [Key]
        public override long Id { get; set; }

        public virtual long PortalUserId { get; set; }

        [ForeignKey(nameof(PortalUserId))]
        public virtual PortalUser PortalUser { get; set; }

        [Required]
        [MaxLength(150)]
        public virtual string Name { get; set; }

        [Required]
        [MaxLength(4000)]
        public virtual string Value { get; set; }

    }
}
