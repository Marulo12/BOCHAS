﻿using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class ServiciosAdicionales
    {
        public ServiciosAdicionales()
        {
            DetalleCobro = new HashSet<DetalleCobro>();
            NotaConsumoServicioAdicional = new HashSet<NotaConsumoServicioAdicional>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool? Activo { get; set; }

        public ICollection<DetalleCobro> DetalleCobro { get; set; }
        public ICollection<NotaConsumoServicioAdicional> NotaConsumoServicioAdicional { get; set; }
    }
}
