using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class DetalleAlquilerCancha
    {
        public DetalleAlquilerCancha()
        {
            DetalleCobro = new HashSet<DetalleCobro>();
        }

        public int Id { get; set; }
        public TimeSpan HoraReservaDesde { get; set; }
        public int IdCancha { get; set; }
        public TimeSpan HoraReservaHasta { get; set; }
        public int IdAlquilerCancha { get; set; }
        public int IdEstadoDetalle { get; set; }

        public AlquilerCancha IdAlquilerCanchaNavigation { get; set; }
        public Cancha IdCanchaNavigation { get; set; }
        public EstadoDetalleAlquiler IdEstadoDetalleNavigation { get; set; }
        public ICollection<DetalleCobro> DetalleCobro { get; set; }
    }
}
