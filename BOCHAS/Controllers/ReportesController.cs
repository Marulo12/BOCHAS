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
using System.ComponentModel.DataAnnotations;

namespace BOCHAS.Controllers
{[AllowAnonymous]
    public class ReportesController : Controller
    {
        private readonly BOCHASContext _context;

        public ReportesController(BOCHASContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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
                var cobro = _context.Cobro.Include(c => c.DetalleCobro).Include(c => c.DetalleCobro).Include(c => c.IdUsuarioNavigation).Include(c => c.IdMedioPagoNavigation).Where(c => c.Numero == NCobro).SingleOrDefault();
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

        public IActionResult ReporteReservas(DateTime FecD, DateTime FecH)
        {
            try
            {
                var reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Include(a => a.IdClienteNavigation).Include(a => a.IdClienteNavigation.Persona).Include(a => a.IdEmpleadoNavigation).Include(a => a.IdCobroNavigation).Include(a => a.IdEstadoNavigation).Where(a => a.FechaReserva >= FecD && a.FechaReserva <= FecH).ToList();
                TempData["fecD"] = FecD.Date.ToString("dd/MM/yyyy");
                TempData["fecH"] = FecH.Date.ToString("dd/MM/yyyy");
                return new ViewAsPdf("ReporteReservas", reserva) {//FileName = "ReporteReservas.pdf" 
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                    CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

                };
            }
            catch
            {
                return NotFound();
            }

        }


        public IActionResult ReporteJugadoresFrecuentes(DateTime FecD, DateTime FecH)
        {
            List<RepoJugador> repo = new List<RepoJugador>();
            var repos = (from a in _context.AlquilerCancha join u in _context.Usuario on a.IdCliente equals u.Id join p in _context.Persona on u.Id equals p.IdUsuario where a.FechaReserva >= FecD.Date && a.FechaReserva <= FecH.Date group p by p.NroDocumento into g select new { Nombre = g.ToList()[0].Nombre + " " + g.ToList()[0].Apellido, DNI = g.ToList()[0].NroDocumento, Cantidad = g.Count() }).ToList().OrderByDescending(a => a.Cantidad);

            foreach (var i in repos)
            {
                RepoJugador r = new RepoJugador();
                r.DNI = i.DNI;
                r.Nombre = i.Nombre;
                r.Cantidad = i.Cantidad;
                repo.Add(r);
            }
            TempData["fecD"] = FecD.Date.ToString("dd/MM/yyyy");
            TempData["fecH"] = FecH.Date.ToString("dd/MM/yyyy");
            return new ViewAsPdf("ReporteJugadoresFrecuentes", repo)
            {

                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };
        }
        public IActionResult ReporteIngresosDiarios(DateTime FecD, DateTime FecH)
        {

            var cobro = _context.Cobro.Include(a => a.IdMedioPagoNavigation).Include(a => a.IdTarjetaNavigation).Include(a => a.IdUsuarioNavigation).Include(a => a.IdUsuarioNavigation.Persona).Include(a => a.DetalleCobro).Where(a => a.Fecha >= FecD.Date && a.Fecha <= FecH.Date).ToList().OrderBy(d => d.Fecha);
            TempData["fecD"] = FecD.Date.ToString("dd/MM/yyyy");
            TempData["fecH"] = FecH.Date.ToString("dd/MM/yyyy");
            return new ViewAsPdf("ReporteIngresosDiarios", cobro)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };

        }

        public IActionResult ReporteClasesParticulares(DateTime FecD, DateTime FecH)
        {

            var clase = _context.ClaseParticular.Include(a => a.IdCanchaNavigation).Include(a => a.IdJugadorNavigation).Include(a => a.IdCobroNavigation).Include(a => a.IdProfesorNavigation).Where(a => a.FechaReserva >= FecD.Date && a.FechaReserva <= FecH.Date).ToList().OrderBy(d => d.FechaReserva);
            TempData["fecD"] = FecD.Date.ToString("dd/MM/yyyy");
            TempData["fecH"] = FecH.Date.ToString("dd/MM/yyyy");
            return new ViewAsPdf("ReporteClasesParticulares", clase)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };

        }

        public IActionResult ReporteCanchas(DateTime FecD, DateTime FecH)
        {
            List<CanchasEfectivas> lc = new List<CanchasEfectivas>();
            var CanchasReservas = (from d in _context.DetalleAlquilerCancha join c in _context.Cancha on d.IdCancha equals c.Id join a in _context.AlquilerCancha on d.IdAlquilerCancha equals a.Numero where a.IdEstado == 4 && d.IdEstadoDetalle == 4 && a.FechaReserva >= FecD.Date && a.FechaReserva <= FecH.Date group c by c.Id into g select new { Nombre = "Numero:" + g.ToList()[0].Numero + ", Nombre:" + g.ToList()[0].Nombre , Cantidad = g.Count() , Id = g.Key }).ToList();
            foreach (var i in CanchasReservas)
            {
                CanchasEfectivas ce = new CanchasEfectivas();
                ce.Id = i.Id;
                ce.Nombre = i.Nombre;
                ce.Cantidad = i.Cantidad;
                ce.servicio = "ALQUILER";
                lc.Add(ce);
            }

            var CanchasClases = (from d in _context.ClaseParticular join c in _context.Cancha on d.IdCancha equals c.Id  where d.FechaReserva >= FecD.Date && d.FechaReserva <= FecH.Date && d.HoraFinReal != null group c by c.Id into g select new { Nombre = "Numero:" + g.ToList()[0].Numero + ", Nombre:" + g.ToList()[0].Nombre, Cantidad = g.Count(), Id = g.Key }).ToList();
            foreach (var i in CanchasClases)
            {
                CanchasEfectivas ce = new CanchasEfectivas();
                ce.Id = i.Id;
                ce.Nombre = i.Nombre;
                ce.Cantidad = i.Cantidad;
                ce.servicio = "CLASES";
                lc.Add(ce);
            }

            TempData["fecD"] = FecD.Date.ToString("dd/MM/yyyy");
            TempData["fecH"] = FecH.Date.ToString("dd/MM/yyyy");
            return new ViewAsPdf("ReporteCanchas", lc)
            {
                //PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };

        }

        public IActionResult ReporteServicios()
        {
            
            return new ViewAsPdf("ReporteServicios")
            {
                
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };

        }


    }
}
