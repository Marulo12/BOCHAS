using BOCHAS.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Controllers
{
    public class ClaseParticularsController : Controller
    {
        private readonly BOCHASContext _context;

        public ClaseParticularsController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: ClaseParticulars
        public IActionResult Index()
        {
            ViewData["IdJugador"] = new SelectList(_context.Persona.Where(p => p.Tipo == "JUGADOR" && p.FechaBaja == null && _context.Jugador.Where(j => j.IdPersona == p.Id && j.IdTipoJugador == 2).Count() > 0).Select(x => new { Id = x.Id, persona = x.Nombre + " " + x.Apellido + ", Nro doc: " + x.NroDocumento }), "Id", "persona");
            ViewData["IdProfesor"] = new SelectList(_context.Persona.Where(p => p.FechaBaja == null && p.Tipo == "EMPLEADO").Select(x => new { Id = x.Id, persona = x.Nombre + " " + x.Apellido + ", Nro doc: " + x.NroDocumento }), "Id", "persona");
            return View();
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


        [HttpPost]
        public JsonResult RegistrarClase(int IdJugador, int IdProfesor, DateTime FechaReserva, TimeSpan HoraInicio, TimeSpan HoraFin, int IdCancha, string Obs)
        {

            try
            {

                ClaseParticular Cp = new ClaseParticular();
                Cp.FechaReserva = FechaReserva;
                Cp.HoraFinPrevista = HoraFin;
                Cp.HoraInicioPrevista = HoraInicio;
                Cp.IdCancha = IdCancha;
                Cp.IdJugador = IdJugador;
                Cp.IdProfesor = IdProfesor;
                Cp.Observacion = Obs;
                _context.ClaseParticular.Add(Cp);

                if (_context.SaveChanges() == 1)
                {
                    int IdClaseParticular = _context.ClaseParticular.Max(a => a.Id);


                    Agenda Ag = new Agenda();
                    Ag.IdClasesParticulares = IdClaseParticular;
                    Ag.IdCancha = Convert.ToInt32(IdCancha);
                    Ag.Fecha = FechaReserva;
                    Ag.HoraDesde = HoraInicio;
                    Ag.HoraHasta = HoraFin;
                    _context.Agenda.Add(Ag);
                    _context.SaveChanges();
                    return Json("EXITO");
                }
                else
                {
                    return Json("ERROR");
                }

            }
            catch { return Json("ERROR"); }

        }


        public IActionResult ConsultarClases()
        {
            ViewData["IdJugador"] = new SelectList(_context.Persona.Where(p => p.Tipo == "JUGADOR" && p.FechaBaja == null && _context.Jugador.Where(j => j.IdPersona == p.Id && j.IdTipoJugador == 2).Count() > 0).Select(x => new { Id = x.Id, persona = x.Nombre + " " + x.Apellido + ", Nro doc: " + x.NroDocumento }), "Id", "persona");
            return View();
        }

        public async Task<IActionResult> MostrarClases(int IdJugador , DateTime? fechaD , DateTime? fechaH)
        {
            if (fechaD != null && fechaH != null)
            {
                var clases = await _context.ClaseParticular.Include(c => c.IdCanchaNavigation).Include(c => c.IdProfesorNavigation).Where(c => c.IdJugador == IdJugador && c.FechaReserva >= fechaD && c.FechaReserva <= fechaH).ToListAsync();
                return PartialView(clases);
            }
            else
            {
                var clases = await _context.ClaseParticular.Include(c=>c.IdCanchaNavigation).Include(c=>c.IdProfesorNavigation).Where(c => c.IdJugador == IdJugador).ToListAsync();
                return PartialView(clases);
            }
            
        }

        public async Task<IActionResult> VerDetalle(int id)
        {            
                var clases = await _context.ClaseParticular.Include(c => c.IdCanchaNavigation).Include(c => c.IdProfesorNavigation).Where(c => c.Id == id).SingleOrDefaultAsync();
                return PartialView(clases);            
        }

        public IActionResult ComenzarClase(int Nclase)
        {
            
               var clase = _context.ClaseParticular.Where(c => c.Id == Nclase).SingleOrDefault();
            clase.HoraInicioReal = TimeSpan.Parse( DateTime.Now.ToString("HH:mm"));
            clase.FechaRealRealizacion = DateTime.Now.Date;
            _context.ClaseParticular.Update(clase);
            if (_context.SaveChanges() == 1)
            {
                var cancha = _context.Cancha.Where(c => c.Id == clase.IdCancha).SingleOrDefault();
                cancha.IdEstadoCancha = 1;
                _context.Cancha.Update(cancha);
                _context.SaveChanges();
                TempData["Resultado"] = "COMENZADO";
                return RedirectToAction("ConsultarClases");
            }
            else
            {
                TempData["Resultado"] = "NO";
                return RedirectToAction("ConsultarClases");
            }
        }

        public IActionResult CancelarClase(int Nclase) {

            var clase = _context.ClaseParticular.Where(c => c.Id == Nclase).SingleOrDefault();
            
            clase.FechaCancelacion = DateTime.Now.Date;
            _context.ClaseParticular.Update(clase);
            if (_context.SaveChanges() == 1)
            {
                Agenda ag = _context.Agenda.Where(a => a.IdClasesParticulares == Nclase).SingleOrDefault();
                _context.Agenda.Remove(ag);
                if (_context.SaveChanges() == 1)
                {
                    try
                    {
                        var persona = _context.ClaseParticular.Where(c => c.Id == Nclase).Include(c => c.IdJugadorNavigation).SingleOrDefault();
                        var mensaje = new MimeMessage();
                        mensaje.From.Add(new MailboxAddress("BOCHAS PADEL", "bochaspadel@gmail.com"));
                        mensaje.To.Add(new MailboxAddress("Alumno", persona.IdJugadorNavigation.Mail));
                        mensaje.Subject = "Cancelacion de Reserva";
                        mensaje.Body = new TextPart("plain") { Text = "Buenos dias  Sr/a. " + persona.IdJugadorNavigation.Apellido + " se realizo la cancelacion de la clase particular para la fecha" + persona.FechaReserva.Date.ToString("dd/MM/yyyy") + ", Saludos." };
                        using (var cliente = new SmtpClient())
                        {
                            cliente.Connect("smtp.gmail.com", 587, false);
                            cliente.Authenticate("bochaspadel@gmail.com", "bochas2018");
                            cliente.Send(mensaje);
                            cliente.Disconnect(true);
                        }
                        TempData["Resultado"] = "Cancelado";
                        return RedirectToAction("ConsultarClases");
                    }
                    catch
                    {
                        TempData["Resultado"] = "NoMail";
                        return RedirectToAction("ConsultarClases");
                    }

                }
                else
                {
                    TempData["Resultado"] = "NO";
                    return RedirectToAction("ConsultarClases");
                }
                
            }
            else
            {
                TempData["Resultado"] = "NO";
                return RedirectToAction("ConsultarClases");
            }
        }

        public IActionResult FinalizarClase(int Nclase) {
            var clase = _context.ClaseParticular.Where(c => c.Id == Nclase).SingleOrDefault();
            clase.HoraFinReal = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
            
            _context.ClaseParticular.Update(clase);
            if (_context.SaveChanges() == 1)
            {
                Agenda ag = _context.Agenda.Where(a => a.IdClasesParticulares == Nclase).SingleOrDefault();
                _context.Agenda.Remove(ag);
                var cancha = _context.Cancha.Where(c => c.Id == clase.IdCancha).SingleOrDefault();
                cancha.IdEstadoCancha = 2;
                _context.Cancha.Update(cancha);
                
                _context.SaveChanges();
                TempData["Resultado"] = "FINALIZADO";
                TempData["NClaseFinalizada"] = Nclase;
                return RedirectToAction("ConsultarClases");
            }
            else
            {
                TempData["Resultado"] = "NO";
                return RedirectToAction("ConsultarClases");
            }
        }

    }
}
