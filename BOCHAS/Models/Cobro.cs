using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Cobro
    {
        public Cobro()
        {
            AlquilerCancha = new HashSet<AlquilerCancha>();
            ClaseParticular = new HashSet<ClaseParticular>();
            DetalleCobro = new HashSet<DetalleCobro>();
        }

        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public int IdMedioPago { get; set; }
        public decimal? MontoTotal { get; set; }
        public int IdUsuario { get; set; }
        public int? NroCupon { get; set; }
        public int? IdTarjeta { get; set; }

        public MediodePago IdMedioPagoNavigation { get; set; }
        public Tarjeta IdTarjetaNavigation { get; set; }
        public Usuario IdUsuarioNavigation { get; set; }
        public ICollection<AlquilerCancha> AlquilerCancha { get; set; }
        public ICollection<ClaseParticular> ClaseParticular { get; set; }
        public ICollection<DetalleCobro> DetalleCobro { get; set; }
    }
}
