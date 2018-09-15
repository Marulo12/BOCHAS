﻿using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class AlquilerCancha
    {
        public AlquilerCancha()
        {
            Agenda = new HashSet<Agenda>();
            DetalleAlquilerCancha = new HashSet<DetalleAlquilerCancha>();
            DetalleCobro = new HashSet<DetalleCobro>();
            NotaConsumoServicioAdicional = new HashSet<NotaConsumoServicioAdicional>();
        }

        public int Numero { get; set; }
        public DateTime? FechaPedido { get; set; }
        public int? IdEmpleado { get; set; }
        public int IdCliente { get; set; }
        public int IdEstado { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public DateTime? FechaReserva { get; set; }
        public int? IdCobro { get; set; }

        public Usuario IdClienteNavigation { get; set; }
        public Cobro IdCobroNavigation { get; set; }
        public Usuario IdEmpleadoNavigation { get; set; }
        public EstadoAlquiler IdEstadoNavigation { get; set; }
        public ICollection<Agenda> Agenda { get; set; }
        public ICollection<DetalleAlquilerCancha> DetalleAlquilerCancha { get; set; }
        public ICollection<DetalleCobro> DetalleCobro { get; set; }
        public ICollection<NotaConsumoServicioAdicional> NotaConsumoServicioAdicional { get; set; }
    }
}
