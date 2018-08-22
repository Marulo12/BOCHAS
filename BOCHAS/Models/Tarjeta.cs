using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Tarjeta
    {
        public Tarjeta()
        {
            Cobro = new HashSet<Cobro>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string TipoTarjeta { get; set; }

        public ICollection<Cobro> Cobro { get; set; }
    }
}
