using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Cargo
    {
        public Cargo()
        {
            Empleado = new HashSet<Empleado>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public ICollection<Empleado> Empleado { get; set; }
    }
}
