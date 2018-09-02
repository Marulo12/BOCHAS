using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;

namespace BOCHAS.APIS
{
    [Produces("application/json")]
    
    public class AlquilerCanchasController : Controller
    {
        private readonly BOCHASContext _context;

        public AlquilerCanchasController(BOCHASContext context)
        {
            _context = context;
        }

        [HttpGet("api/AlquilerCanchas/ComprobarDisponibilidad/{fecR}/{hd}/{hh}")]
        public JsonResult ComprobarDisponibilidad(string fecR, string hd, string hh)
        {
            var cancha = (from c in _context.Cancha join e in _context.EstadoCancha on c.IdEstadoCancha equals e.Id where e.Id == 1 || e.Id == 2 select new { Id = c.Id }).ToList();
            int[] idCanchasDisponibles = new int[cancha.Count()];
            List<int> IdCanchas = new List<int>();
            foreach (var c in cancha)
            {


                var agenda = _context.Agenda.Where(a => a.Fecha == Convert.ToDateTime(fecR) && a.IdCancha == c.Id).OrderBy(a => a.HoraDesde).ToList();

                if (agenda.Count() == 0)
                {
                    IdCanchas.Add(c.Id);
                }
                else
                {
                    if (agenda.Where(a => TimeSpan.Parse(hd) >= a.HoraDesde && TimeSpan.Parse(hh) <= a.HoraHasta).ToList().Count() > 0)
                    {

                    }

                    if (agenda.Where(a => TimeSpan.Parse(hd) == a.HoraDesde).ToList().Count() > 0)
                    {

                    }
                    else
                    {
                        if (TimeSpan.Parse(hd) < agenda[0].HoraDesde && TimeSpan.Parse(hh) <= agenda[0].HoraDesde)
                        {

                            IdCanchas.Add(c.Id);

                            continue;
                        }
                        if (TimeSpan.Parse(hd) >= agenda[agenda.Count() - 1].HoraHasta)
                        {

                            IdCanchas.Add(c.Id);
                            continue;
                        }

                        for (int ii = 0; ii < agenda.Count(); ii++)
                        {
                            if (TimeSpan.Parse(hd) == agenda[ii].HoraHasta)
                            {

                                if (TimeSpan.Parse(hh) <= agenda[ii + 1].HoraDesde)
                                {
                                    IdCanchas.Add(c.Id);
                                    break;
                                }

                            }
                            else
                            {
                                if (TimeSpan.Parse(hd) >= agenda[ii].HoraHasta && TimeSpan.Parse(hh) <= agenda[ii + 1].HoraDesde)
                                {
                                    IdCanchas.Add(c.Id);
                                    break;
                                }
                            }


                        }

                    }

                }

            }


            if (IdCanchas.Count > 0)
            {
                List<Cancha> Lcancha = new List<Cancha>();
                foreach (var i in IdCanchas)
                {
                    if (i > 0)
                    {
                        var canchas = _context.Cancha.Where(a => a.Id == i).SingleOrDefault();
                        Cancha c = new Cancha();
                        c.Id = canchas.Id;
                        c.Numero = canchas.Numero;
                        c.Nombre = canchas.Nombre;
                        c.Descripcion = canchas.Descripcion;
                        Lcancha.Add(c);
                    }
                }
                return Json(Lcancha);

            }
            else
            {
                return Json("VACIO");
            }

        }

        public class RR {
            public string fecR { get; set; }
            public string hd { get; set; }
            public string hh { get; set; }
            public string[] Canchas { get; set; }
            public string Usuario { get; set; }
        }


        [HttpPost("api/AlquilerCanchas/RegistrarReserva")]
        public JsonResult RegistrarReserva([FromBody] RR reserva)
        {

            try
            {
                int ResultadoAlquiler = 0;
                DateTime fechaPedido = DateTime.Now.Date;
                DateTime fechaReserva = Convert.ToDateTime(reserva.fecR).Date;
                TimeSpan HoraDesde = TimeSpan.Parse(reserva.hd);
                TimeSpan HoraHasta = TimeSpan.Parse(reserva.hh);

                int idCliente = (from u in _context.Usuario join p in _context.Persona on u.Id equals p.IdUsuario where u.Nombre == reserva.Usuario && p.Tipo == "JUGADOR" && p.FechaBaja == null select u).SingleOrDefault().Id;


                AlquilerCancha al = new AlquilerCancha();
                al.FechaPedido = fechaPedido;
                al.FechaReserva = fechaReserva;
                al.IdCliente = idCliente;
                al.IdEstado = 2;
                _context.AlquilerCancha.Add(al);

                if (_context.SaveChanges() == 1)
                {
                    int IdAlquiler = _context.AlquilerCancha.Max(a => a.Numero);
                    ResultadoAlquiler = IdAlquiler;
                    foreach (var id in reserva.Canchas)
                    {
                        DetalleAlquilerCancha DetA = new DetalleAlquilerCancha();
                        DetA.IdAlquilerCancha = IdAlquiler;
                        DetA.IdCancha = Convert.ToInt32(id);
                        DetA.HoraReservaDesde = HoraDesde;
                        DetA.HoraReservaHasta = HoraHasta;
                        DetA.IdEstadoDetalle = 2;
                        _context.DetalleAlquilerCancha.Add(DetA);
                        _context.SaveChanges();
                        Agenda ag = new Agenda();
                        ag.Fecha = Convert.ToDateTime(al.FechaReserva).Date;
                        ag.IdAlquilerCancha = al.Numero;
                        ag.IdCancha = Convert.ToInt32(id);
                        ag.HoraDesde = HoraDesde;
                        ag.HoraHasta = HoraHasta;
                        _context.Agenda.Add(ag);
                        _context.SaveChanges();
                    }


                }


                return Json(Ok());
            }
            catch { return Json(NotFound()); }

        }

        [HttpGet("api/AlquilerCanchas/ListadoReservasPorJugador/{Usuario}")]
        [HttpGet("api/AlquilerCanchas/ListadoReservasPorJugador/{Usuario}/{Dias}")]
        public async Task<JsonResult> ListadoReservasPorJugador([FromRoute] string Usuario, int? Dias)
        {
            int idCliente = (from u in _context.Usuario join p in _context.Persona on u.Id equals p.IdUsuario where u.Nombre == Usuario && p.Tipo == "JUGADOR" && p.FechaBaja == null select u).SingleOrDefault().Id;
            if (Dias == null)
            {
                var alquiler = (from a in _context.AlquilerCancha join u in _context.Usuario on a.IdCliente equals u.Id join e in _context.EstadoAlquiler on a.IdEstado equals e.Id where u.Id == idCliente select new { Numero = a.Numero, FechaPedido = a.FechaPedido.Value.Day + "/" + a.FechaPedido.Value.Month + "/" + a.FechaPedido.Value.Year, FechaReserva = a.FechaReserva.Value.Day + "/" + a.FechaReserva.Value.Month + "/" + a.FechaReserva.Value.Year, Estado = e.Nombre, IdEstado = e.Id }).ToListAsync();
                return Json(await alquiler);
            }
            else
            {
                DateTime FechaLimite = DateTime.Now.Date.AddDays((double)Dias);
                if (Dias > 0)
                {
                   
                  
                    var alquiler = (from a in _context.AlquilerCancha join u in _context.Usuario on a.IdCliente equals u.Id join e in _context.EstadoAlquiler on a.IdEstado equals e.Id where u.Id == idCliente && a.FechaReserva >= DateTime.Now.Date && a.FechaReserva <= FechaLimite select new { Numero = a.Numero, FechaPedido = a.FechaPedido.Value.Day + "/" + a.FechaPedido.Value.Month + "/" + a.FechaPedido.Value.Year, FechaReserva = a.FechaReserva.Value.Day + "/" + a.FechaReserva.Value.Month + "/" + a.FechaReserva.Value.Year, Estado = e.Nombre, IdEstado = e.Id }).ToListAsync();
                    return Json(await alquiler);
                }
                else
                {

                 
                    var alquiler = (from a in _context.AlquilerCancha join u in _context.Usuario on a.IdCliente equals u.Id join e in _context.EstadoAlquiler on a.IdEstado equals e.Id where u.Id == idCliente && a.FechaReserva <= DateTime.Now.Date && a.FechaReserva >= FechaLimite select new { Numero = a.Numero, FechaPedido = a.FechaPedido.Value.Day + "/" + a.FechaPedido.Value.Month + "/" + a.FechaPedido.Value.Year, FechaReserva = a.FechaReserva.Value.Day + "/" + a.FechaReserva.Value.Month + "/" + a.FechaReserva.Value.Year, Estado = e.Nombre, IdEstado = e.Id }).ToListAsync();
                    return Json(await alquiler);
                }
                
            }
            
        }
       
        [HttpGet("api/AlquilerCanchas/DetalleReserva/{Numero}")]
        public async Task<JsonResult> DetalleReserva([FromRoute] int Numero)
        {
            int num = Numero;
            
            var detalle = (from d in _context.DetalleAlquilerCancha join c in _context.Cancha on d.IdCancha equals c.Id join e in _context.EstadoDetalleAlquiler on d.IdEstadoDetalle equals e.Id where d.IdAlquilerCancha == num select new {IdCancha= c.Id , NumCancha = c.Numero , Nombre = c.Nombre , Descripcion = c.Descripcion , HoraDesde = d.HoraReservaDesde.Hours + ":" + d.HoraReservaDesde.Minutes , HoraHasta = d.HoraReservaHasta.Hours + ":" + d.HoraReservaHasta.Minutes , Estado = e.Nombre , IdEstadoDetalleReserva = e.Id }).ToListAsync();
            return Json(await detalle);
        }



    }
}