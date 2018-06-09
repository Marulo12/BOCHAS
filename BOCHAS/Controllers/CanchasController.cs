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
{ [Authorize]
    public class CanchasController : Controller
    {
        private readonly BOCHASContext _context;

        public CanchasController(BOCHASContext context)
        {
            _context = context;
        }

      
        public async Task<IActionResult> Index()
        {
            var bOCHASContext = _context.Cancha.Include(c => c.IdEstadoCnchaNavigation).Include(c => c.IdTipoMaterialNavigation).Where(c=>c.IdEstadoCncha != 3);
            return View(await bOCHASContext.ToListAsync());
        }
            
        
        public IActionResult NuevaCancha()
        {
            ViewData["IdEstadoCncha"] = new SelectList(_context.EstadoCancha, "Id", "Nombre");
            ViewData["IdTipoMaterial"] = new SelectList(_context.TipoMaterial.OrderBy(tm=> tm.Nombre), "Id", "Nombre");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,Nombre,Descripcion,IdTipoMaterial,IdEstadoCncha")] Cancha cancha)
        {
            if (ModelState.IsValid)
            {
                cancha.IdEstadoCncha = 2;
                _context.Add(cancha);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEstadoCncha"] = new SelectList(_context.EstadoCancha, "Id", "Nombre", cancha.IdEstadoCncha);
            ViewData["IdTipoMaterial"] = new SelectList(_context.TipoMaterial, "Id", "Nombre", cancha.IdTipoMaterial);
            return View(cancha);
        }

        // GET: Canchas/Edit/5
        public async Task<IActionResult> EditarCancha(int? id)
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
            return PartialView(cancha);
        }

       
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

       

        
        [HttpPost]
       
        public async Task<IActionResult> BajadeCancha(string id)
        {
            int Id = Convert.ToInt32(id);
            var cancha = await _context.Cancha.SingleOrDefaultAsync(m => m.Id == Id);
            cancha.IdEstadoCncha = 3;
            _context.Cancha.Update(cancha);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CanchaExists(int id)
        {
            return _context.Cancha.Any(e => e.Id == id);
        }
    }
}
