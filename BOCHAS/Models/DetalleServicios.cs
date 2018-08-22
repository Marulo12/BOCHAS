using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class DetalleServicios
    {
        public int Id { get; set; }
        public int IdServiciosAdicionales { get; set; }
        public int Cantidad { get; set; }
        public int IdServicio { get; set; }

        public Servicio IdServicioNavigation { get; set; }
        public ServiciosAdicionales IdServiciosAdicionalesNavigation { get; set; }
    }
}
