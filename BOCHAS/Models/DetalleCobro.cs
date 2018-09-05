﻿using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class DetalleCobro
    {
        public int Id { get; set; }
        public int? IdServicio { get; set; }
        public decimal Monto { get; set; }
        public int IdNumeroCobro { get; set; }
        public int Cantidad { get; set; }
        public int? IdServiciosAdicionales { get; set; }
        public double? TotalHoras { get; set; }

        public Cobro IdNumeroCobroNavigation { get; set; }
        public Servicio IdServicioNavigation { get; set; }
        public ServiciosAdicionales IdServiciosAdicionalesNavigation { get; set; }
    }
}
