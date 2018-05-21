using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Localidad
    {
        public Localidad()
        {
            Barrio = new HashSet<Barrio>();
            Domicilio = new HashSet<Domicilio>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }

        public ICollection<Barrio> Barrio { get; set; }
        public ICollection<Domicilio> Domicilio { get; set; }
    }
}
