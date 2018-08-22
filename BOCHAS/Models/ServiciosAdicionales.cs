using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class ServiciosAdicionales
    {
        public ServiciosAdicionales()
        {
            DetalleServicios = new HashSet<DetalleServicios>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }

        public ICollection<DetalleServicios> DetalleServicios { get; set; }
    }
}
