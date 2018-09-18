using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class HorariosProfesor
    {
        public int Id { get; set; }
        public TimeSpan? HoraDesde { get; set; }
        public TimeSpan? HoraHasta { get; set; }
        public int? IdProfesor { get; set; }

        public string Turno { get; set; }
        public Empleado IdProfesorNavigation { get; set; }
    }
}
