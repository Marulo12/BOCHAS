using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class EstadoAlquiler
    {
        public EstadoAlquiler()
        {
            AlquilerCancha = new HashSet<AlquilerCancha>();
        }

        public int Id { get; set; }
        public byte[] Nombre { get; set; }
        public string Descripcion { get; set; }

        public ICollection<AlquilerCancha> AlquilerCancha { get; set; }
    }
}
