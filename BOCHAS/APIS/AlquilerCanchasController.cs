using BOCHAS.Hubs;
using BOCHAS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.APIS
{
    [Produces("application/json")]

    public class AlquilerCanchasController : Controller
    {
        private readonly BOCHASContext _context;
        private readonly IHubContext<Chat> _hubContext;
        public AlquilerCanchasController(BOCHASContext context, IHubContext<Chat> hubContext)
        {
            _hubContext = hubContext;
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

            List<Cancha> Lcancha = new List<Cancha>();
            if (IdCanchas.Count > 0)
            {

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

            }
            return Json(Lcancha);

        }

        public class RR
        {
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
                var reservaJ = (from a in _context.AlquilerCancha join p in _context.Persona on a.IdCliente equals p.IdUsuario where a.IdEmpleado == null && a.IdEstado == 2 select new { Numero = a.Numero, Nombre = p.Nombre, Apellido = p.Apellido }).ToList();

                _hubContext.Clients.All.ReservasJugador(reservaJ);
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

            var detalle = (from d in _context.DetalleAlquilerCancha join c in _context.Cancha on d.IdCancha equals c.Id join e in _context.EstadoDetalleAlquiler on d.IdEstadoDetalle equals e.Id where d.IdAlquilerCancha == num select new { IdCancha = c.Id, NumCancha = c.Numero, Nombre = c.Nombre, Descripcion = c.Descripcion, HoraDesde = d.HoraReservaDesde, HoraHasta = d.HoraReservaHasta, Estado = e.Nombre, IdEstadoDetalleReserva = e.Id }).ToListAsync();
            return Json(await detalle);
        }


        [HttpGet("api/AlquilerCanchas/Noticias")]
        public async Task<JsonResult> Noticias()
        {
            var noticias = _context.Noticias.Where(n => n.Activo == true).ToListAsync();
            return Json(await noticias);
        }



        [HttpGet("api/AlquilerCanchas/sugerirCanchas/{fecR}/{hd}")]
        public JsonResult sugerirCanchas(string fecR, string hd)
        {
            var cancha = (from c in _context.Cancha join e in _context.EstadoCancha on c.IdEstadoCancha equals e.Id where e.Id == 1 || e.Id == 2 select c).ToList();

            List<Sugerencias> SCanchas = new List<Sugerencias>();

            int totalhoras = TimeSpan.Parse(hd).Hours;


            foreach (var c in cancha)
            {
                TimeSpan horadesde = TimeSpan.Parse(hd);
                TimeSpan horahasta = new TimeSpan();
                TimeSpan t2 = new TimeSpan(1, 0, 0);
                var agenda = _context.Agenda.Where(a => a.Fecha == Convert.ToDateTime(fecR) && a.IdCancha == c.Id).ToList();
                if (agenda.Count() == 0)
                {
                    Sugerencias s = new Sugerencias();
                    s.id = c.Id;
                    s.nombre = c.Nombre;
                    s.numero = c.Numero;
                    s.descripcion = c.Descripcion;
                    s.horadesde = horadesde.ToString();
                    s.horahasta = horadesde.Add(t2).ToString();
                    SCanchas.Add(s);
                    continue;
                }
                for (int i = 0; i < 24 - totalhoras; i++)
                {
                   
                    horadesde = horadesde.Add(new TimeSpan(i, 0, 0));
                    horahasta = horadesde.Add(t2);

                    if (horadesde.Hours <= 24 && horadesde.Days == 0 && horahasta.Hours <= 24 && horahasta.Days == 0)
                    {

                        for (int ag = 0; ag < agenda.Count(); ag++)
                        {
                            if (ag == agenda.Count() - 1)
                            {
                                if (horadesde >= agenda[ag].HoraHasta)
                                {
                                    Sugerencias s = new Sugerencias();
                                    s.id = c.Id;
                                    s.nombre = c.Nombre;
                                    s.numero = c.Numero;
                                    s.descripcion = c.Descripcion;
                                    s.horadesde = horadesde.ToString();
                                    s.horahasta = horahasta.ToString();
                                    SCanchas.Add(s);

                                }
                                if (horahasta <= agenda[ag].HoraDesde)
                                {
                                    Sugerencias s = new Sugerencias();
                                    s.id = c.Id;
                                    s.nombre = c.Nombre;
                                    s.numero = c.Numero;
                                    s.descripcion = c.Descripcion;
                                    s.horadesde = horadesde.ToString();
                                    s.horahasta = horahasta.ToString();
                                    SCanchas.Add(s);


                                }

                            }
                            else
                            {


                                if (horadesde >= agenda[ag].HoraDesde && horadesde <= agenda[ag].HoraHasta && horahasta <= agenda[ag + 1].HoraDesde)
                                {
                                    Sugerencias s = new Sugerencias();
                                    s.id = c.Id;
                                    s.nombre = c.Nombre;
                                    s.numero = c.Numero;
                                    s.descripcion = c.Descripcion;
                                    s.horadesde = horadesde.ToString();
                                    s.horahasta = horahasta.ToString();
                                    SCanchas.Add(s);
                                    break;
                                }

                            }





                        }

                    }

                }



            }


            return Json(SCanchas);



        }

        public class Sugerencias { public int id { set; get; } public int numero { set; get; } public string nombre { set; get; } public string descripcion { set; get; } public string horadesde { set; get; } public string horahasta { set; get; } }









    }
}