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
{
    public class ServiciosAdicionalesController : Controller
    {
        private readonly BOCHASContext _context;

        public ServiciosAdicionalesController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: ServiciosAdicionales
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiciosAdicionales.ToListAsync());
        }

        // GET: ServiciosAdicionales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciosAdicionales = await _context.ServiciosAdicionales
                .SingleOrDefaultAsync(m => m.Id == id);
            if (serviciosAdicionales == null)
            {
                return NotFound();
            }

            return View(serviciosAdicionales);
        }

        // GET: ServiciosAdicionales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiciosAdicionales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Precio")] ServiciosAdicionales serviciosAdicionales)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviciosAdicionales);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviciosAdicionales);
        }

        // GET: ServiciosAdicionales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciosAdicionales = await _context.ServiciosAdicionales.SingleOrDefaultAsync(m => m.Id == id);
            if (serviciosAdicionales == null)
            {
                return NotFound();
            }
            return View(serviciosAdicionales);
        }

        // POST: ServiciosAdicionales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio")] ServiciosAdicionales serviciosAdicionales)
        {
            if (id != serviciosAdicionales.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    serviciosAdicionales.Precio = Convert.ToDecimal(serviciosAdicionales.Precio);
                    _context.Update(serviciosAdicionales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiciosAdicionalesExists(serviciosAdicionales.Id))
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
            return View(serviciosAdicionales);
        }

        // GET: ServiciosAdicionales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciosAdicionales = await _context.ServiciosAdicionales
                .SingleOrDefaultAsync(m => m.Id == id);
            if (serviciosAdicionales == null)
            {
                return NotFound();
            }

            return View(serviciosAdicionales);
        }

        // POST: ServiciosAdicionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviciosAdicionales = await _context.ServiciosAdicionales.SingleOrDefaultAsync(m => m.Id == id);
            _context.ServiciosAdicionales.Remove(serviciosAdicionales);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiciosAdicionalesExists(int id)
        {
            return _context.ServiciosAdicionales.Any(e => e.Id == id);
        }
    }
}
