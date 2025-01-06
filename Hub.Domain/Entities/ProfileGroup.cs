﻿using Hub.Infrastructure.Database.Entity;
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
        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Name { get; set; }

        [JsonConverter(typeof(ConcreteListTypeConverter<IAccessRule, AccessRule>))]
        public virtual ICollection<IAccessRule> Rules { get; set; }

        [Required]
        public virtual bool Administrator { get; set; }

        [Required]
        public virtual bool AllowMultipleAccess { get; set; }
    }
}
