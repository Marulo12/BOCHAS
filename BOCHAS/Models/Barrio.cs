using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Barrio
    {
        public Barrio()
        {
            Domicilio = new HashSet<Domicilio>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdLocalidad { get; set; }

        public Localidad IdLocalidadNavigation { get; set; }
        public ICollection<Domicilio> Domicilio { get; set; }
    }
}
