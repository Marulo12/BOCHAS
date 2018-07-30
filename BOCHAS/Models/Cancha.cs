using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Cancha
    {
        public Cancha()
        {
            Agenda = new HashSet<Agenda>();
            DetalleAlquilerCancha = new HashSet<DetalleAlquilerCancha>();
        }

        public int Id { get; set; }
        public int Numero { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoMaterial { get; set; }
        public int IdEstadoCancha { get; set; }

        public EstadoCancha IdEstadoCanchaNavigation { get; set; }
        public TipoMaterial IdTipoMaterialNavigation { get; set; }
        public ICollection<Agenda> Agenda { get; set; }
        public ICollection<DetalleAlquilerCancha> DetalleAlquilerCancha { get; set; }
    }
}
