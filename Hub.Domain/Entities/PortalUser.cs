using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities
{
    [Table("PortalUser")]
    public class PortalUser : BaseEntity, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        //[Required]
        //[StringLength(100)]
        //public virtual string Name { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Login { get; set; }

        [IgnoreLog]
        [Required]
        [StringLength(50)]
        public virtual string Password { get; set; }

        [IgnoreLog]
        [StringLength(50)]
        public virtual string? TempPassword { get; set; }

        public virtual bool Inactive { get; set; }

        [IgnoreLog]
        public virtual bool ChangingPass { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Keyword { get; set; }


        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
