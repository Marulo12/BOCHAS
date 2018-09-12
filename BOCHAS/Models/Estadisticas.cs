using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Models
{
    public class Estadisticas
    {
        [Key]
        public int Id { set; get; }
        public string Nombre { set; get; }
    }
}
