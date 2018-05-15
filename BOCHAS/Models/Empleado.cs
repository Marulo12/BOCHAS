using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Models
{
    public class Empleado
    {
        [Key]
        public int IdPersona { set; get; }
        [Required]
        public int IdCargo { set; get; }
        public bool Activo { set; get; }
        [Required]
        public DateTime FechaComienzo { set; get; }
    }
}
