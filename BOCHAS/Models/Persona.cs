using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Persona
    {
        public Persona()
        {
            Jugador = new HashSet<Jugador>();
        }

        public int Id { get; set; }
        public string NroDocumento { get; set; }
        public int IdTipoDocumento { get; set; }
        public int IdDomicilio { get; set; }
        public int? IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string Mail { get; set; }
        public string Telefono { get; set; }
        public string Tipo { get; set; }
        public string Imagen { get; set; }

        public Domicilio IdDomicilioNavigation { get; set; }
        public TipoDocumento IdTipoDocumentoNavigation { get; set; }
        public Usuario IdUsuarioNavigation { get; set; }
        public Empleado Empleado { get; set; }
        public ICollection<Jugador> Jugador { get; set; }
    }
}
