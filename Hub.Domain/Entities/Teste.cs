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
    public class Teste
    {
        [Key]
        public int Fuck { get; set; }
    }
}
