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
        public int Hora { set; get; }
        public DayOfWeek Dia { set; get; }
        public int Cantidad { set; get; }
        public string FecD { set; get; }
        public string FecH { set; get; }

    }
    public class RepoJugador
    {
        [Key]
        public string DNI { set; get; }
        public string Nombre { set; get; }
        public int Cantidad { set; get; }
        public string FecD { set; get; }
        public string FecH { set; get; }
    }
    public class RepoReservas
    {
       public List<AlquilerCancha> Reservas { set; get; }
        public string FecD { set; get; }
        public string FecH { set; get; }
    }
    
         public class RepoIngresosDiarios
    {
        public IOrderedEnumerable<Cobro> Cobros { set; get; }
        public string FecD { set; get; }
        public string FecH { set; get; }
    }

    public class RepoClases
    {
        public IOrderedEnumerable<ClaseParticular> Clases { set; get; }
        public string FecD { set; get; }
        public string FecH { set; get; }
    }
    public class RepoCobroClasesIndividual
    {
        public Cobro Clases { set; get; }
        public int Nclase { set; get; }
       
    }
    public class RepoCobroReservasIndividual
    {
        public Cobro Reservas { set; get; }
        public int Nreserva { set; get; }

    }
}
