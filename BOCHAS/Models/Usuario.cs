using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            Persona = new HashSet<Persona>();
            Session = new HashSet<Session>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Contraseña { get; set; }

        public ICollection<Persona> Persona { get; set; }
      public ICollection<Session> Session { get; set; }
    }
}
