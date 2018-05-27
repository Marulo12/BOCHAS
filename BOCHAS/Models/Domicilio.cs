using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Domicilio
    {
        public Domicilio()
        {
            Persona = new HashSet<Persona>();
        }

        public int Id { get; set; }
        public int IdBarrio { get; set; }
        public int Numero { get; set; }
        public string Calle { get; set; }
        public int IdLocalidad { get; set; }
        public string Departamento { get; set; }
        public int? Piso { get; set; }

        public Barrio IdBarrioNavigation { get; set; }
        public Localidad IdLocalidadNavigation { get; set; }
        public ICollection<Persona> Persona { get; set; }
    }
}
