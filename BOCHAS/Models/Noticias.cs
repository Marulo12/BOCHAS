using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class Noticias
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Url { get; set; }
        public bool Activo { get; set; }
    }
}
