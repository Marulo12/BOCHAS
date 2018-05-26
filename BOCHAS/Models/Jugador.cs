using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Jugador
    {
        public int IdPersona { get; set; }
        public int IdTipoJugador { get; set; }

        public Persona IdPersonaNavigation { get; set; }
        public TipoJugador IdTipoJugadorNavigation { get; set; }
    }
}
