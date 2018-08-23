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

     
     

        // GET: Cobro/Create
        public IActionResult CobroReserva(int Numero )
        {
            int canchas = _context.DetalleAlquilerCancha.Where(d => d.IdAlquilerCancha == Numero && d.IdEstadoDetalle == 4).ToList().Count();
            var horas = _context.DetalleAlquilerCancha.Where(d => d.IdAlquilerCancha == Numero).ToList()[0].HoraReservaHasta - _context.DetalleAlquilerCancha.Where(d => d.IdAlquilerCancha == Numero).ToList()[0].HoraReservaDesde;
            var empleado = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(u => u.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name && u.Tipo == "EMPLEADO" && u.FechaBaja == null).SingleOrDefault();
            ViewData["canchas"] = canchas;
            ViewData["horas"] = horas;
            ViewData["empleado"] = empleado.Nombre + " " + empleado.Apellido;
            ViewData["Numero"] = Numero;            
            ViewData["IdMedioPago"] = new SelectList(_context.MediodePago, "Id", "Nombre");
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta, "Id", "Nombre");            
            return View();
        }

        public async Task<JsonResult> ListadoServiciosAdicionales() {

            var listado = await _context.ServiciosAdicionales.ToListAsync();
            return Json(listado);
        }

        private bool CobroExists(int id)
        {
            return _context.Cobro.Any(e => e.Numero == id);
        }
    }
}
