﻿using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities.Users
{
    [Table("PortalUserPassHistory")]
    public class PortalUserPassHistory : BaseEntity
    {
        [Key]
        public override long Id { get; set; }

        public virtual long PortalUserId { get; set; }

        [ForeignKey(nameof(PortalUserId))]
        public virtual PortalUser PortalUser { get; set; }

        [Required]
        public virtual string Password { get; set; }

        [Required]
        public virtual DateTime CreationUTC { get; set; }

        public virtual DateTime? ExpirationUTC { get; set; }
    }
}
