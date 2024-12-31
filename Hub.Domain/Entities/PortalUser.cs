using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Hub.Domain.Entities
{
    public class PortalUser : BaseEntity, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string Name { get; set; }

        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
