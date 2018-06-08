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
    public class CanchasController : Controller
    {
        private readonly BOCHASContext _context;

        public CanchasController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: Canchas
        public async Task<IActionResult> Index()
        {
            var bOCHASContext = _context.Cancha.Include(c => c.IdEstadoCnchaNavigation).Include(c => c.IdTipoMaterialNavigation);
            return View(await bOCHASContext.ToListAsync());
        }

        // GET: Canchas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancha = await _context.Cancha
                .Include(c => c.IdEstadoCnchaNavigation)
                .Include(c => c.IdTipoMaterialNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cancha == null)
            {
                return NotFound();
            }

            return View(cancha);
        }

        // GET: Canchas/Create
        public IActionResult Create()
        {
            ViewData["IdEstadoCncha"] = new SelectList(_context.EstadoCancha, "Id", "Nombre");
            ViewData["IdTipoMaterial"] = new SelectList(_context.TipoMaterial, "Id", "Nombre");
            return View();
        }

        // POST: Canchas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,Nombre,Descripcion,IdTipoMaterial,IdEstadoCncha")] Cancha cancha)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cancha);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEstadoCncha"] = new SelectList(_context.EstadoCancha, "Id", "Nombre", cancha.IdEstadoCncha);
            ViewData["IdTipoMaterial"] = new SelectList(_context.TipoMaterial, "Id", "Nombre", cancha.IdTipoMaterial);
            return View(cancha);
        }

        // GET: Canchas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancha = await _context.Cancha.SingleOrDefaultAsync(m => m.Id == id);
            if (cancha == null)
            {
                return NotFound();
            }
            ViewData["IdEstadoCncha"] = new SelectList(_context.EstadoCancha, "Id", "Nombre", cancha.IdEstadoCncha);
            ViewData["IdTipoMaterial"] = new SelectList(_context.TipoMaterial, "Id", "Nombre", cancha.IdTipoMaterial);
            return View(cancha);
        }

        // POST: Canchas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,Nombre,Descripcion,IdTipoMaterial,IdEstadoCncha")] Cancha cancha)
        {
            if (id != cancha.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cancha);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CanchaExists(cancha.Id))
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
            ViewData["IdEstadoCncha"] = new SelectList(_context.EstadoCancha, "Id", "Nombre", cancha.IdEstadoCncha);
            ViewData["IdTipoMaterial"] = new SelectList(_context.TipoMaterial, "Id", "Nombre", cancha.IdTipoMaterial);
            return View(cancha);
        }

        // GET: Canchas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancha = await _context.Cancha
                .Include(c => c.IdEstadoCnchaNavigation)
                .Include(c => c.IdTipoMaterialNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cancha == null)
            {
                return NotFound();
            }

            return View(cancha);
        }

        // POST: Canchas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cancha = await _context.Cancha.SingleOrDefaultAsync(m => m.Id == id);
            _context.Cancha.Remove(cancha);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CanchaExists(int id)
        {
            return _context.Cancha.Any(e => e.Id == id);
        }
    }
}
