using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using Microsoft.AspNetCore.Authorization;

namespace BOCHAS.Controllers
{  [Authorize]
    public class AlquilerCanchasController : Controller
    {
        private readonly BOCHASContext _context;

        public AlquilerCanchasController(BOCHASContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NuevaReserva()
        {

            var empleado = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(u => u.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name && u.Tipo == "EMPLEADO" && u.FechaBaja == null).SingleOrDefault();
            ViewData["empleado"] = empleado.Nombre + " " + empleado.Apellido;

            return View();

        }
        public IActionResult NuevaReservaJugador()
        {         
            return View();

        }
        [HttpPost]
        public JsonResult RegistrarReserva(string fecR,string hd,string hh,string[] Canchas ,string Cliente)
        {
         
          try {
                int ResultadoAlquiler = 0;
                DateTime fechaPedido = DateTime.Now.Date;
                DateTime fechaReserva = Convert.ToDateTime(fecR).Date;
            TimeSpan HoraDesde = TimeSpan.Parse(hd);
                TimeSpan HoraHasta = TimeSpan.Parse(hh);
            int idPersona = Convert.ToInt32(Cliente);
            int idCliente = (from u in _context.Usuario join p in _context.Persona on u.Id equals p.IdUsuario where p.Id == idPersona && p.Tipo == "JUGADOR" select u).SingleOrDefault().Id;
            int empleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;
            
            AlquilerCancha al = new AlquilerCancha();
                al.FechaPedido = fechaPedido;
                al.FechaReserva = fechaReserva;
                al.IdCliente = idCliente;
                al.IdEmpleado = empleado;
                al.IdEstado = 2;
                   _context.AlquilerCancha.Add(al);
               
                if (_context.SaveChanges() == 1)
                {
                    int IdAlquiler = _context.AlquilerCancha.Max(a=>a.Numero);
                    ResultadoAlquiler = IdAlquiler;
                    foreach (var id in Canchas)
                    {
                        DetalleAlquilerCancha DetA = new DetalleAlquilerCancha();
                        DetA.IdAlquilerCancha = IdAlquiler;
                        DetA.IdCancha = Convert.ToInt32(id);
                        DetA.HoraReservaDesde = HoraDesde;
                        DetA.HoraReservaHasta = HoraHasta;
                        _context.DetalleAlquilerCancha.Add(DetA);
                        if (_context.SaveChanges() == 1)
                        {
                            Agenda Ag = new Agenda();
                            Ag.IdAlquilerCancha = IdAlquiler;
                            Ag.IdCancha = Convert.ToInt32(id);
                            Ag.Fecha = fechaReserva;
                            Ag.HoraDesde = HoraDesde;
                            Ag.HoraHasta = HoraHasta;
                            _context.Agenda.Add(Ag);
                            _context.SaveChanges();

                        
                        }




                    }
                }

                return Json(ResultadoAlquiler);
           }
          catch { return Json("ERROR"); }
       
        }
        [HttpPost]
        public JsonResult RegistrarReservaJugador(string fecR, string hd, string hh, string[] Canchas)
        {

            try
            {
                int ResultadoAlquiler = 0;
                DateTime fechaPedido = DateTime.Now.Date;
                DateTime fechaReserva = Convert.ToDateTime(fecR).Date;
                TimeSpan HoraDesde = TimeSpan.Parse(hd);
                TimeSpan HoraHasta = TimeSpan.Parse(hh);
                
                int idCliente = (from u in _context.Usuario join p in _context.Persona on u.Id equals p.IdUsuario where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "JUGADOR" && p.FechaBaja == null select u).SingleOrDefault().Id;
                

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
                    foreach (var id in Canchas)
                    {
                        DetalleAlquilerCancha DetA = new DetalleAlquilerCancha();
                        DetA.IdAlquilerCancha = IdAlquiler;
                        DetA.IdCancha = Convert.ToInt32(id);
                        DetA.HoraReservaDesde = HoraDesde;
                        DetA.HoraReservaHasta = HoraHasta;
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

                return Json(ResultadoAlquiler);
            }
            catch { return Json("ERROR"); }

        }

        public JsonResult MostrarCanchas(string fecR, string hd, string hh)
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
                       // continue;
                    }

                    if (agenda.Where(a => TimeSpan.Parse(hd) == a.HoraDesde).ToList().Count() > 0)
                    {
                      //  continue;
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
                                /*if (ii == agenda.Count() - 1)
                                {
                                    IdCanchas.Add(c.Id);
                                    break;
                                }*/
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


        public async Task<IActionResult> MisReservas()
        {
            int idCliente = (from u in _context.Usuario join p in _context.Persona on u.Id equals p.IdUsuario where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "JUGADOR" && p.FechaBaja == null select u).SingleOrDefault().Id;
            var alquiler = _context.AlquilerCancha.Include(a=>a.IdEstadoNavigation).Where(a => a.IdCliente == idCliente && (a.IdEstado == 1 || a.IdEstado == 2)).OrderBy(a=>a.Numero).ToListAsync();

            return View(await alquiler);
        }

        public async Task<IActionResult> VerDetalleMiReserva(string numero)
        { int num = Convert.ToInt32(numero);
            var detalle = _context.DetalleAlquilerCancha.Include(d=>d.IdAlquilerCanchaNavigation).Include(d=>d.IdCanchaNavigation).Where(d => d.IdAlquilerCancha == num).ToListAsync();
            return PartialView(await detalle);
        }

        public async Task<IActionResult> ConsultarReservas()
        { 
            var alquiler = _context.AlquilerCancha.Include(a => a.IdEstadoNavigation).Include(a => a.IdClienteNavigation.Persona).Include(a=>a.IdEmpleadoNavigation.Persona).Include(a=>a.IdClienteNavigation).Include(a=>a.IdEmpleadoNavigation).Where(a=>a.FechaReserva >= DateTime.Now.Date && a.IdEstado != 5).OrderByDescending(a => a.Numero).ToListAsync();

            return View(await alquiler);
        }

        public IActionResult ConfirmarReserva(int Nreserva)
        {
 
            var reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Where(a => a.Numero == Nreserva).SingleOrDefault();
                reserva.IdEstado = 2;
                reserva.IdEmpleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;
                _context.AlquilerCancha.Update(reserva);
                if (_context.SaveChanges() == 1)
                {
                    foreach (var r in reserva.DetalleAlquilerCancha)
                    {
                        Agenda ag = new Agenda();
                        ag.Fecha = Convert.ToDateTime(reserva.FechaReserva).Date;
                        ag.IdAlquilerCancha = reserva.Numero;
                        ag.IdCancha = r.IdCancha;
                        ag.HoraDesde = r.HoraReservaDesde;
                        ag.HoraHasta = r.HoraReservaHasta;
                        _context.Agenda.Add(ag);
                        _context.SaveChanges();
                    }
                    TempData["Respuesta"] = "SI";
                    return RedirectToAction("ConsultarReservas");
                }
                else
                {
                TempData["Respuesta"] = "NO";
                return RedirectToAction("ConsultarReservas");
            }
            }

        public IActionResult CancelarReserva(int Nreserva)
        {
            var reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Where(a => a.Numero == Nreserva).SingleOrDefault();

            if (reserva.IdEstado == 1)
            {
                reserva.IdEstado = 5;
                reserva.FechaCancelacion = DateTime.Now.Date;
                _context.AlquilerCancha.Update(reserva);
                _context.SaveChanges();
                TempData["Respuesta"] = "Cancelado";
                return RedirectToAction("ConsultarReservas");
            }

            
            if (reserva.IdEstado == 2)
            {
                var ag = _context.Agenda.Where(a => a.IdAlquilerCancha == Nreserva).ToList();
                foreach (var a in ag)
                {
                    _context.Agenda.Remove(a);
                    _context.SaveChanges();

                }

                reserva.IdEstado = 5;
                reserva.FechaCancelacion = DateTime.Now.Date;
                _context.AlquilerCancha.Update(reserva);
                if (_context.SaveChanges() == 1)

                {
                   
                    TempData["Respuesta"] = "Cancelado";
                    return RedirectToAction("ConsultarReservas");

                }
              
  
            }

            TempData["Respuesta"] = "NO";
            return RedirectToAction("ConsultarReservas");
        }
        public IActionResult ComenzarReserva(int Nreserva)
        {

            var reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Where(a => a.Numero == Nreserva).SingleOrDefault();
            reserva.IdEstado = 3;
            reserva.IdEmpleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;
            _context.AlquilerCancha.Update(reserva);
            if (_context.SaveChanges() == 1)
            {
                foreach (var r in reserva.DetalleAlquilerCancha)
                {
                    var cancha = _context.Cancha.Where(c => c.Id == r.IdCancha).SingleOrDefault();
                    cancha.IdEstadoCancha = 1;
                    _context.Cancha.Update(cancha);
                    _context.SaveChanges();
                }
                TempData["Respuesta"] = "COMENZADO";
                return RedirectToAction("ConsultarReservas");
            }
            else
            {
                TempData["Respuesta"] = "NO";
                return RedirectToAction("ConsultarReservas");
            }
        }
        public IActionResult ConsultaReservaParticular(string Jugador)
        {
            int IdJ = Convert.ToInt32(Jugador);
            var Reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Include(a => a.IdClienteNavigation).Include(a => a.IdClienteNavigation.Persona).Include(a => a.IdEstadoNavigation).Where(a => a.IdClienteNavigation.Persona.Any(p => p.Id ==IdJ));
            
           
            return PartialView(Reserva.ToList());
        }

        public  IActionResult ReporteReserva(int Nreserva)
        {
            var reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Include(a => a.IdClienteNavigation).Include(a => a.IdClienteNavigation.Persona).Include(a => a.IdEmpleadoNavigation).Include(a => a.IdEstadoNavigation).Where(a => a.Numero == Nreserva).SingleOrDefault();
            return new Rotativa.AspNetCore.ViewAsPdf("ReporteReserva", reserva);
        }



        }

        }

       
    

