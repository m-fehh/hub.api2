using Hub.Infrastructure.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Domain.Entities
{
    [Table("FuckYou")]
    public class Teste : BaseEntity
    {
        [Key]
        public override long Id { get; set; }
    }
}
