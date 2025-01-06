using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hub.Domain.Entities.Users
{
    public class PortalUserFingerprint : BaseEntity, IModificationControl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id { get; set; }

        [Required]
        public virtual long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual PortalUser PortalUser { get; set; }

        [MaxLength(255)]
        public virtual string OS { get; set; }

        [MaxLength(255)]
        public virtual string BrowserName { get; set; }

        [MaxLength(1024)]
        public virtual string BrowserInfo { get; set; }

        public virtual double? Lat { get; set; }

        public virtual double? Lng { get; set; }

        [MaxLength(255)]
        public virtual string IpAddress { get; set; }

        public virtual DateTime? CreationUTC { get; set; }

        public virtual DateTime? LastUpdateUTC { get; set; }

        public virtual bool? CookieEnabled { get; set; }
    }
}
