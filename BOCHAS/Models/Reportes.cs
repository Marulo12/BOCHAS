using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Models
{
    public class Reportes
    {
    }
    public class CanchasEfectivas
    {
        [Key]
        public int Id { set; get; }
        public string Nombre { set; get; }
        public int Cantidad { set; get; }
        public string servicio { set; get; }
    }
    public class RepoJugador
    {
        [Key]
        public string DNI { set; get; }
        public string Nombre { set; get; }
        public int Cantidad { set; get; }
    }
}
