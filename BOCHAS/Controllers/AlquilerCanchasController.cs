 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;

namespace BOCHAS.Controllers
{
    public class AlquilerCanchasController : Controller
    {
        private readonly BOCHASContext _context;

        public AlquilerCanchasController(BOCHASContext context)
        {
            _context = context;
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

               bool mismaHD = false;
               Boolean salirBuclePadre = false;
               foreach (var c in cancha)
               {
                   mismaHD = false;              
                   salirBuclePadre = false;
                   var agenda = _context.Agenda.Where(a => a.Fecha == Convert.ToDateTime(fecR) && a.IdCancha == c.Id).OrderBy(a => a.HoraDesde).ToList();

                   if (agenda.Count() == 0)
                   {

                       IdCanchas.Add(c.Id);
                   }
                   else
                   {
                       for (int i = 0; i < agenda.Count(); i++)
                       {
                           if (agenda[i].HoraDesde == TimeSpan.Parse(hd))
                           {
                               mismaHD = true;
                           }

                       }
                       if (mismaHD)
                       {
                           continue;
                       }
                       else
                       {
                           for (int ii = 0; ii < agenda.Count(); ii++)
                           {
                               if (TimeSpan.Parse(hd) == agenda[ii].HoraHasta)
                               {
                                   if (ii == agenda.Count() - 1)
                                   {

                                       IdCanchas.Add(c.Id);
                                       salirBuclePadre = true;
                                       break;
                                   }
                                   if (TimeSpan.Parse(hh) <= agenda[ii + 1].HoraDesde)
                                   {

                                       IdCanchas.Add(c.Id);
                                       salirBuclePadre = true;
                                       break;

                                   }

                               }
                                       else{
                                        if (TimeSpan.Parse(hd) < agenda[0].HoraDesde && TimeSpan.Parse(hh) <= agenda[0].HoraDesde)
                                        {

                                            IdCanchas.Add(c.Id);
                                            salirBuclePadre = true;
                                            break;
                                        }
                                        if (TimeSpan.Parse(hd) >= agenda[agenda.Count()-1].HoraHasta)
                                        {

                                            IdCanchas.Add(c.Id);
                                            salirBuclePadre = true;
                                            break;
                                        }


                                        if (TimeSpan.Parse(hd) >= agenda[ii].HoraHasta && TimeSpan.Parse(hh) <= agenda[ii + 1].HoraDesde)
                                        {

                                            IdCanchas.Add(c.Id);
                                            salirBuclePadre = true;
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





        // GET: AlquilerCanchas
        public async Task<IActionResult> Index()
        {
            var bOCHASContext = _context.AlquilerCancha.Include(a => a.IdClienteNavigation).Include(a => a.IdEmpleadoNavigation).Include(a => a.IdEstadoNavigation);
            return View(await bOCHASContext.ToListAsync());
        }

        // GET: AlquilerCanchas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquilerCancha = await _context.AlquilerCancha
                .Include(a => a.IdClienteNavigation)
                .Include(a => a.IdEmpleadoNavigation)
                .Include(a => a.IdEstadoNavigation)
                .SingleOrDefaultAsync(m => m.Numero == id);
            if (alquilerCancha == null)
            {
                return NotFound();
            }

            return View(alquilerCancha);
        }

        // GET: AlquilerCanchas/Create
        public IActionResult Create()
        {
            ViewData["IdCliente"] = new SelectList(_context.Usuario, "Id", "Contraseña");
            ViewData["IdEmpleado"] = new SelectList(_context.Usuario, "Id", "Contraseña");
            ViewData["IdEstado"] = new SelectList(_context.EstadoAlquiler, "Id", "Id");
            return View();
        }

        // POST: AlquilerCanchas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Numero,FechaPedido,IdEmpleado,IdCliente,IdEstado,FechaCancelacion,FechaReserva,Cobro")] AlquilerCancha alquilerCancha)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alquilerCancha);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCliente"] = new SelectList(_context.Usuario, "Id", "Contraseña", alquilerCancha.IdCliente);
            ViewData["IdEmpleado"] = new SelectList(_context.Usuario, "Id", "Contraseña", alquilerCancha.IdEmpleado);
            ViewData["IdEstado"] = new SelectList(_context.EstadoAlquiler, "Id", "Id", alquilerCancha.IdEstado);
            return View(alquilerCancha);
        }

        // GET: AlquilerCanchas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquilerCancha = await _context.AlquilerCancha.SingleOrDefaultAsync(m => m.Numero == id);
            if (alquilerCancha == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Usuario, "Id", "Contraseña", alquilerCancha.IdCliente);
            ViewData["IdEmpleado"] = new SelectList(_context.Usuario, "Id", "Contraseña", alquilerCancha.IdEmpleado);
            ViewData["IdEstado"] = new SelectList(_context.EstadoAlquiler, "Id", "Id", alquilerCancha.IdEstado);
            return View(alquilerCancha);
        }

        // POST: AlquilerCanchas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Numero,FechaPedido,IdEmpleado,IdCliente,IdEstado,FechaCancelacion,FechaReserva,Cobro")] AlquilerCancha alquilerCancha)
        {
            if (id != alquilerCancha.Numero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alquilerCancha);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlquilerCanchaExists(alquilerCancha.Numero))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCliente"] = new SelectList(_context.Usuario, "Id", "Contraseña", alquilerCancha.IdCliente);
            ViewData["IdEmpleado"] = new SelectList(_context.Usuario, "Id", "Contraseña", alquilerCancha.IdEmpleado);
            ViewData["IdEstado"] = new SelectList(_context.EstadoAlquiler, "Id", "Id", alquilerCancha.IdEstado);
            return View(alquilerCancha);
        }

        // GET: AlquilerCanchas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquilerCancha = await _context.AlquilerCancha
                .Include(a => a.IdClienteNavigation)
                .Include(a => a.IdEmpleadoNavigation)
                .Include(a => a.IdEstadoNavigation)
                .SingleOrDefaultAsync(m => m.Numero == id);
            if (alquilerCancha == null)
            {
                return NotFound();
            }

            return View(alquilerCancha);
        }

        // POST: AlquilerCanchas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alquilerCancha = await _context.AlquilerCancha.SingleOrDefaultAsync(m => m.Numero == id);
            _context.AlquilerCancha.Remove(alquilerCancha);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool AlquilerCanchaExists(int id)
        {
            return _context.AlquilerCancha.Any(e => e.Numero == id);
        }
    }
}
