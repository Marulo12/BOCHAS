using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class TipoDocumento
    {
        public TipoDocumento()
        {
            Persona = new HashSet<Persona>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public ICollection<Persona> Persona { get; set; }
    }
}
