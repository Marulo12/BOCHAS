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
    public class CobroController : Controller
    {
        private readonly BOCHASContext _context;

        public CobroController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: Cobro
        public async Task<IActionResult> Index()
        {
            var bOCHASContext = _context.Cobro.Include(c => c.IdMedioPagoNavigation).Include(c => c.IdTarjetaNavigation).Include(c => c.IdUsuarioNavigation);
            return View(await bOCHASContext.ToListAsync());
        }

        // GET: Cobro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cobro = await _context.Cobro
                .Include(c => c.IdMedioPagoNavigation)
                .Include(c => c.IdTarjetaNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .SingleOrDefaultAsync(m => m.Numero == id);
            if (cobro == null)
            {
                return NotFound();
            }

            return View(cobro);
        }

        // GET: Cobro/Create
        public IActionResult RegistrarCobro(int? Numero , string servicio)
        {
            ViewData["Numero"] = Numero;
            ViewData["servicio"] = servicio;
            ViewData["IdMedioPago"] = new SelectList(_context.MediodePago, "Id", "Nombre");
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta, "Id", "Nombre");
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Contraseña");
            return View();
        }

        // POST: Cobro/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Numero,Fecha,IdMedioPago,MontoTotal,IdUsuario,NroCupon,IdTarjeta")] Cobro cobro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cobro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdMedioPago"] = new SelectList(_context.MediodePago, "Id", "Nombre", cobro.IdMedioPago);
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta, "Id", "Nombre", cobro.IdTarjeta);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Contraseña", cobro.IdUsuario);
            return View(cobro);
        }

        // GET: Cobro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cobro = await _context.Cobro.SingleOrDefaultAsync(m => m.Numero == id);
            if (cobro == null)
            {
                return NotFound();
            }
            ViewData["IdMedioPago"] = new SelectList(_context.MediodePago, "Id", "Nombre", cobro.IdMedioPago);
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta, "Id", "Nombre", cobro.IdTarjeta);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Contraseña", cobro.IdUsuario);
            return View(cobro);
        }

        // POST: Cobro/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Numero,Fecha,IdMedioPago,MontoTotal,IdUsuario,NroCupon,IdTarjeta")] Cobro cobro)
        {
            if (id != cobro.Numero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cobro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CobroExists(cobro.Numero))
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
            ViewData["IdMedioPago"] = new SelectList(_context.MediodePago, "Id", "Nombre", cobro.IdMedioPago);
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta, "Id", "Nombre", cobro.IdTarjeta);
            ViewData["IdUsuario"] = new SelectList(_context.Usuario, "Id", "Contraseña", cobro.IdUsuario);
            return View(cobro);
        }

        // GET: Cobro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cobro = await _context.Cobro
                .Include(c => c.IdMedioPagoNavigation)
                .Include(c => c.IdTarjetaNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .SingleOrDefaultAsync(m => m.Numero == id);
            if (cobro == null)
            {
                return NotFound();
            }

            return View(cobro);
        }

        // POST: Cobro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cobro = await _context.Cobro.SingleOrDefaultAsync(m => m.Numero == id);
            _context.Cobro.Remove(cobro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CobroExists(int id)
        {
            return _context.Cobro.Any(e => e.Numero == id);
        }
    }
}
