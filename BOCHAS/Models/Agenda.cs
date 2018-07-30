using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Agenda
    {
        public int Id { get; set; }
        public int? IdAlquilerCancha { get; set; }
        public int IdTorneo { get; set; }
        public int? IdClasesParticulares { get; set; }
        public DateTime Fecha { get; set; }
        public int IdCancha { get; set; }
        public TimeSpan HoraDesde { get; set; }
        public TimeSpan HoraHasta { get; set; }

        public AlquilerCancha IdAlquilerCanchaNavigation { get; set; }
        public Cancha IdCanchaNavigation { get; set; }
    }
}
