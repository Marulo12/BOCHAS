using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class TipoJugador
    {
        public TipoJugador()
        {
            Jugador = new HashSet<Jugador>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public ICollection<Jugador> Jugador { get; set; }
    }
}
