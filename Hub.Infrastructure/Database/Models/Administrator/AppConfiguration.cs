using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Infrastructure.Database.Models.Administrator
{
    [Table("AppConfigurations")]
    public class AppConfiguration : BaseEntity
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public virtual string AppCode { get; set; }

        [Required]
        [MaxLength(200)]
        public virtual string Name { get; set; }

        public virtual string? SchemaId { get; set; }

        [Required]
        public virtual string? Environment { get; set; }

        public virtual bool Inactive { get; set; }

        [Required]
        [MaxLength(50)]
        public virtual string MinVersionIOS { get; set; }

        [Required]
        [MaxLength(50)]
        public virtual string MinVersionAndroid { get; set; }

        [Required]
        [MaxLength(50)]
        public virtual string LatestVersionIOS { get; set; }

        [Required]
        [MaxLength(50)]
        public virtual string LatestVersionAndroid { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string InfoAppStoreIOS { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string InfoAppStoreAndroid { get; set; }
    }
}
