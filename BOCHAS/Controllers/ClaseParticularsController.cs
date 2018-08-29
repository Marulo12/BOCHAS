using BOCHAS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

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

    }
}
