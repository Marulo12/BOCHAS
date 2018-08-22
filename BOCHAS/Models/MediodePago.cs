using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class MediodePago
    {
        public MediodePago()
        {
            Cobro = new HashSet<Cobro>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }

        public ICollection<Cobro> Cobro { get; set; }
    }
}
