using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class ClaseParticular
    {
        public ClaseParticular()
        {
            Agenda = new HashSet<Agenda>();
        }

        public int Id { get; set; }
        public int IdJugador { get; set; }
        public int IdProfesor { get; set; }
        public DateTime FechaReserva { get; set; }
        public TimeSpan HoraInicioPrevista { get; set; }
        public TimeSpan HoraFinPrevista { get; set; }
        public TimeSpan? HoraInicioReal { get; set; }
        public TimeSpan? HoraFinReal { get; set; }
        public DateTime? FechaRealRealizacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public int? IdCobro { get; set; }
        public string Observacion { get; set; }
        public int IdCancha { get; set; }

        public Cancha IdCanchaNavigation { get; set; }
        public Cobro IdCobroNavigation { get; set; }
        public Persona IdJugadorNavigation { get; set; }
        public Persona IdProfesorNavigation { get; set; }
        public ICollection<Agenda> Agenda { get; set; }
    }
}
