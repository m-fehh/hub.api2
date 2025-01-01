using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hub.Domain.Entities
{
    [Table("AccessRule")]
    public class AccessRule : BaseEntity, IAccessRule
    {
        [Key]
        public override long Id { get; set; }

        public virtual long ParentId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(ParentId))]
        public virtual IAccessRule Parent { get; set; } 


        [Required]
        [StringLength(150)]
        public virtual string KeyName { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Description { get; set; }

        [IgnoreLog]
        public virtual string Tree { get; set; }
    }
}
