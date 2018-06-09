using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class EstadoCancha
    {
        public EstadoCancha()
        {
            Cancha = new HashSet<Cancha>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public ICollection<Cancha> Cancha { get; set; }
    }
}
