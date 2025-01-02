using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities
{
    [Table("PortalUser")]
    public class PortalUser : BaseEntity, IUserAccount, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public virtual string Email { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Login { get; set; }

        [IgnoreLog]
        [Required]
        [StringLength(50)]
        public virtual string Password { get; set; }

        public virtual long? PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }

        public virtual long ProfileId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(ProfileId))]
        public virtual IProfileGroup Profile { get; set; }

        [IgnoreLog]
        [StringLength(50)]
        public virtual string? TempPassword { get; set; }


        [IgnoreLog]
        public virtual bool ChangingPass { get; set; }
        
        public virtual bool Inactive { get; set; }

        [StringLength(50)]
        public virtual string? Keyword { get; set; }

        public virtual string? IpAddress { get; set; }

        public virtual DateTime? LastAccessDate { get; set; }

        public virtual DateTime? LastPasswordRecoverRequestDate { get; set; }

        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }
    }
}
