using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace BOCHAS.Controllers
{[AllowAnonymous]
    public class ReportesController : Controller
    {
        private readonly BOCHASContext _context;

        public ReportesController(BOCHASContext context)
        {
            _context = context;
        }

        public IActionResult ReporteReserva(int Nreserva)
        {
            try
            {
                var reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Include(a => a.IdClienteNavigation).Include(a => a.IdClienteNavigation.Persona).Include(a => a.IdEmpleadoNavigation).Include(a => a.IdEstadoNavigation).Where(a => a.Numero == Nreserva).SingleOrDefault();
                return new ViewAsPdf("ReporteReserva", reserva);
            }
            catch
            {
                return NotFound();
            }

        }
        public IActionResult ReporteClase(int Nclase)
        {
            try
            {
                var clase = _context.ClaseParticular.Include(a => a.IdCanchaNavigation).Include(a => a.IdJugadorNavigation).Include(a => a.IdProfesorNavigation).Where(a => a.Id == Nclase).SingleOrDefault();
                return new ViewAsPdf("ReporteClase", clase);
            }
            catch
            {
                return NotFound();
            }

        }
        public IActionResult ReporteCobroReserva(int NCobro)
        {
            try
            {
                var cobro = _context.Cobro.Include(c=>c.DetalleCobro).Include(c=>c.DetalleCobro).Include(c=>c.IdUsuarioNavigation).Include(c=>c.IdMedioPagoNavigation).Where(c=>c.Numero == NCobro ).SingleOrDefault() ;
                return new ViewAsPdf("ReporteCobroReserva", cobro);
            }
            catch
            {
                return NotFound();
            }

        }
        public IActionResult ReporteCobroClase(int NCobro)
        {
            try
            {
                var cobro = _context.Cobro.Include(c => c.DetalleCobro).Include(c => c.DetalleCobro).Include(c => c.IdUsuarioNavigation).Include(c => c.IdMedioPagoNavigation).Where(c => c.Numero == NCobro).SingleOrDefault();
                return new ViewAsPdf("ReporteCobroClase", cobro);
            }
            catch
            {
                return NotFound();
            }

        }
    }
}
