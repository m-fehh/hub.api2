using Hub.Infrastructure.Database.Entity.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.Database.Entity;

namespace Hub.Domain.Entities
{
    [Table("AccessRule")]
    public class AccessRule : BaseEntity, IAccessRule
    {
        [Key]
        public override long Id { get; set; }

        public virtual long? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        [JsonConverter(typeof(ConcreteTypeConverter<AccessRule>))]
        public virtual IAccessRule Parent { get; set; } 


        [Required]
        [StringLength(150)]
        public virtual string KeyName { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Description { get; set; }

        [IgnoreLog]
        public virtual string? Tree { get; set; }
    }
}
