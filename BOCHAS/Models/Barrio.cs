using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Models
{
    public class Barrio
    {
        public int Id { set; get; }
        [Required]
        public string Nombre { set; get; }
        
        public int IdLocalidad { set; get; }
    }
}
