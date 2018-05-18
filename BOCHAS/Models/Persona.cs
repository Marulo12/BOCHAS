using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Models
{
    public class Persona
    {
        public int Id { set; get; }
        [Required(ErrorMessage ="No cargo el Nombre")]
        public string nombre { set; get; }
        [Required(ErrorMessage = "No cargo el Apellido")]
        public string Apellido { set; get; }
        [Required(ErrorMessage = "No cargo el Mail")]
        [DataType(DataType.EmailAddress)]
        public string Mail { set; get; }

        [Required(ErrorMessage = "No cargo el telefono")]
        public string Telefono { set; get; }
        [Required(ErrorMessage = "No cargo el Numero de Documento")]
        public int NroDocumento { set; get; }
        [Required]
        public string Tipo { set; get; }
        [Required]
        public int IdTipoDocumento { set; get; }
        [Required]
        public int Id_Domicilio { set; get; }
        
        public int Id_Usuario { set; get; }
        public DateTime? Fecha_Baja { set; get; }

    }
    
}
