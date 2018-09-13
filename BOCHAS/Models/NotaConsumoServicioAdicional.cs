using System;
using System.Collections.Generic;

namespace BOCHAS.Models
{
    public partial class NotaConsumoServicioAdicional
    {
        public int Id { get; set; }
        public int? IdNumeroAlquiler { get; set; }
        public int? IdNumeroClase { get; set; }
        public int IdServicioAdicional { get; set; }
        public int Cantidad { get; set; }

        public AlquilerCancha IdNumeroAlquilerNavigation { get; set; }
        public ClaseParticular IdNumeroClaseNavigation { get; set; }
        public ServiciosAdicionales IdServicioAdicionalNavigation { get; set; }
    }
}
