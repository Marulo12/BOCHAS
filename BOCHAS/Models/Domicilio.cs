using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Models
{
    public class Domicilio
    {
        public int Id { set; get; }
        
        [Required(ErrorMessage = "No cargo el Barrio")]
        public int IdBarrio { set; get; }
        [Required(ErrorMessage = "No cargo el Numero ")]
        
        public int Numero { set; get; }
        [Required(ErrorMessage = "No cargo la Calle ")]
        public string Calle { set; get; }
        [Required(ErrorMessage = "No cargo la Localidad ")]
        public int IdLocalidad { set; get; }
    }
}
