using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Models
{
    public class Usuario
    {
        public int Id { set; get; }
        [Required(ErrorMessage = "No cargo el Usuario ")]
        public string Nombre { set; get; }
        [Required(ErrorMessage = "No cargo la contraseña ")]
        [StringLength(7), DataType(DataType.Password)]
        public string Contraseña { set; get; }
    }
}
