using Hub.Domain.Common;
using Hub.Domain.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities
{
    [Table("Tenants", Schema = "Adm")]
    public class Tenant : BaseEntity, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        [Required]
        public virtual string Name { get; set; }

        public virtual string Subdomain { get; set; }

        public virtual string Schema { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string CultureName { get; set; }

        public virtual DateTime? CreationUTC { get; set; }

        public virtual  DateTime? LastUpdateUTC { get; set; }
    }
}
