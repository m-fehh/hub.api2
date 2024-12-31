using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Internacionalization.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities
{
    [Table("DocumentType")]
    public class DocumentType : BaseEntity
    {
        [Key]
        public override long Id { get; set; }

        [Required]  
        [StringLength(50)]
        public virtual string Abbrev { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Mask { get; set; }

        [Required]
        public virtual long? MinLength { get; set; }

        [Required]
        public virtual long? MaxLength { get; set; }

        [Required]  
        public virtual ESpecialDocumentValidation? SpecialDocumentValidation { get; set; }

        [Required]  
        public virtual bool Inactive { get; set; }
    }
}


