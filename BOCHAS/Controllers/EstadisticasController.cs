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
    [Authorize]
    public class EstadisticasController : Controller
    {
        private readonly BOCHASContext _context;

        public EstadisticasController(BOCHASContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult ComparacionServicioMensual()
        {
            var alquiler = _context.AlquilerCancha.Where(a => a.FechaReserva.Value.Year == DateTime.Now.Year && a.FechaReserva.Value.Month == DateTime.Now.Month && a.IdEstado == 4).Count();
            var Clases = _context.ClaseParticular.Where(c => c.FechaReserva.Year == DateTime.Now.Year && c.FechaReserva.Month == DateTime.Now.Month && c.HoraFinReal != null).Count();

            var resultado = new { alquiler, Clases };

            return Json(resultado);
        }

        public async Task<JsonResult> GraficoProyeccionClases()
        {
            var real = await (from a in _context.ClaseParticular where a.HoraFinReal != null group a by a.FechaReserva.Month into g select new { mes = g.Key.ToString(), cantidad = g.Count() }).ToArrayAsync();
            var proy = await (from a in _context.ClaseParticular group a by a.FechaReserva.Month into g select new { mes = g.Key.ToString(), cantidad = g.Count() }).ToArrayAsync();
            var R = new[] { real, proy };
            return Json(R);
        }


    }
}
