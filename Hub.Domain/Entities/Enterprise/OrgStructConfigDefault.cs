using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities.Enterprise
{
    [Table("OrgStructConfigDefault")]
    public class OrgStructConfigDefault : BaseEntity
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(300)]
        public virtual string DefaultValue { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string ConfigType { get; set; }

        public virtual bool ApplyToRoot { get; set; }

        public virtual bool ApplyToDomain { get; set; }

        public virtual bool ApplyToLeaf { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string GroupName { get; set; }

        [StringLength(150)]
        public virtual string? SearchName { get; set; }

        [StringLength(150)]
        public virtual string? SearchExtraCondition { get; set; }

        [MaxLength(int.MaxValue)]
        public virtual string? Options { get; set; }

        [StringLength(150)]
        public virtual string? Legend { get; set; }

        public virtual int? MaxLength { get; set; }

        public virtual int? OrderConfig { get; set; }

        public virtual long? OrgStructConfigDefaultDependencyId { get; set; }


        [ForeignKey(nameof(OrgStructConfigDefaultDependencyId))]
        public virtual OrgStructConfigDefault OrgStructConfigDefaultDependency { get; set; }
    }
}
