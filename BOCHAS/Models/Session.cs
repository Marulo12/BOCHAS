using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Session
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public int Origen { get; set; }

        public Usuario IdUsuarioNavigation { get; set; }
    }
}
