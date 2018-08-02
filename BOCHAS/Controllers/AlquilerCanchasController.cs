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
        public async Task<IActionResult> Index()
        {
            var bOCHASContext = _context.AlquilerCancha.Include(a => a.IdClienteNavigation).Include(a => a.IdEmpleadoNavigation).Include(a => a.IdEstadoNavigation);
            return View(await bOCHASContext.ToListAsync());
        }

        public IActionResult NuevaReserva()
        {

            var empleado = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(u => u.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name && u.Tipo == "EMPLEADO" && u.FechaBaja == null).SingleOrDefault();
            ViewData["empleado"] = empleado.Nombre + " " + empleado.Apellido;

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


      

       
    }
}
