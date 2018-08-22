using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class EstadoDetalleAlquiler
    {
        public EstadoDetalleAlquiler()
        {
            DetalleAlquilerCancha = new HashSet<DetalleAlquilerCancha>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public ICollection<DetalleAlquilerCancha> DetalleAlquilerCancha { get; set; }
    }
}
