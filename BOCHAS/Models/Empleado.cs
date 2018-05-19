using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Empleado
    {
        public int IdPersona { get; set; }
        public int IdCargo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaComienzo { get; set; }

        public Cargo IdCargoNavigation { get; set; }
        public Persona IdPersonaNavigation { get; set; }
    }
}
