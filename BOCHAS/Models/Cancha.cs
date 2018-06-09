using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Cancha
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoMaterial { get; set; }
        public int IdEstadoCncha { get; set; }

        public EstadoCancha IdEstadoCnchaNavigation { get; set; }
        public TipoMaterial IdTipoMaterialNavigation { get; set; }
    }
}
