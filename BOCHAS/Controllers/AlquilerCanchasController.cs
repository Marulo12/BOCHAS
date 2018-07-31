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

        public JsonResult MostrarCanchas(string fecha, string hd, string hh)
        {
            DateTime fechaR = Convert.ToDateTime(fecha);
            TimeSpan HD = Convert.ToDateTime(hd).TimeOfDay;
            TimeSpan HH = Convert.ToDateTime(hh).TimeOfDay;
            var cancha = (from c in _context.Cancha join e in _context.EstadoCancha on c.IdEstadoCancha equals e.Id where e.Id == 1 || e.Id == 2 select new { Id = c.Id }).ToList();
            int[] idCanchasDisponibles = new int[cancha.Count()];
            int count = 0;

            foreach (var c in cancha)
            {
                Boolean salirBuclePadre = false;
                var agenda = _context.Agenda.Where(a => a.Fecha == fechaR && a.IdCancha == c.Id).OrderByDescending(a => a.HoraDesde).ToList();
              

                if (agenda.Count() == 0)
                {
                    idCanchasDisponibles[count] = c.Id;
                    count++;
                   
                }
                else
                {
                    for (int i = 0; i < agenda.Count(); i++)
                    {
                        if (agenda[i].HoraDesde == HD)
                        {
                            break;
                        }
                        else
                        {
                            for (int ii = 0; ii < agenda.Count(); ii++)
                            {
                                if (HD == agenda[ii].HoraHasta)
                                {
                                    if (HH <= agenda[ii + 1].HoraDesde)
                                    {
                                        idCanchasDisponibles[count] = c.Id;
                                        count++;
                                        salirBuclePadre = true;
                                        break;

                                    }
                                    else
                                    {
                                        salirBuclePadre = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (HD > agenda[ii].HoraHasta && HD <= agenda[ii + 1].HoraHasta)
                                    {
                                        idCanchasDisponibles[count] = c.Id;
                                        count++;
                                        salirBuclePadre = true;
                                        break;
                                    }
                                }

                                if (salirBuclePadre)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

            }
            if (idCanchasDisponibles.Length != 0)
            {
                List<Cancha> Lcancha = new List<Cancha>();
                for (int j = 0; j < idCanchasDisponibles.Length; j++)
                {
                    var canchas = _context.Cancha.Where(a => a.Id == idCanchasDisponibles[j]).SingleOrDefault();
                    Lcancha.Add(canchas);
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
