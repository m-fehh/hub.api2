using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using System.ComponentModel.DataAnnotations;
using Hub.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities.Users
{
    [Table("Person")]
    public class Person : BaseEntity
    {
        public Person()
        {
            QrCodeInfo = Guid.NewGuid().ToString();
        }

        [Key]
        public override long Id { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Name { get; set; }

        [StringLength(100)]
        public virtual string Document { get; set; }

        public virtual long DocumentTypeId { get; set; }


        [ForeignKey(nameof(DocumentTypeId))]
        public virtual DocumentType DocumentType { get; set; }

        public virtual EGender? Gender { get; set; }

        public virtual DateTime? BirthDate { get; set; }

        [StringLength(100)]
        public virtual string Keyword { get; set; }

        [StringLength(50)]
        public virtual string QrCodeInfo { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUserUpdateUTC { get; set; }
    }
}
