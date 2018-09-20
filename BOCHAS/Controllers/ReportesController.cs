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
using Microsoft.AspNetCore.Http;

namespace BOCHAS.Controllers
{[Authorize]
    public class ReportesController : Controller
    {
        private readonly BOCHASContext _context;

        
        public ReportesController(BOCHASContext context )
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
                var cobro = _context.Cobro.Include(c => c.DetalleCobro).Include(c => c.IdUsuarioNavigation).Include(c => c.IdMedioPagoNavigation).Where(c => c.Numero == NCobro).SingleOrDefault();
                return new ViewAsPdf("ReporteCobroReserva", cobro);
            }
            catch
            {
                return NotFound();
            }

        }
        public IActionResult ReporteCobroReservaIndividual(int NCobro, int NReserva)
        {
            try
            {
               
               
                var cobro = _context.Cobro.Include(c => c.DetalleCobro).Include(c => c.IdUsuarioNavigation).Include(c => c.IdMedioPagoNavigation).Where(c => c.Numero == NCobro).SingleOrDefault();
                RepoCobroReservasIndividual rp = new RepoCobroReservasIndividual();
                rp.Reservas = cobro;
                rp.Nreserva = NReserva;
                return new ViewAsPdf("ReporteCobroReservaIndividual", rp);
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
        public IActionResult ReporteCobroClaseIndividual(int NCobro , int NClase)
        {
            try
            {
                
                var cobro = _context.Cobro.Include(c => c.DetalleCobro).Include(c => c.DetalleCobro).Include(c => c.IdUsuarioNavigation).Include(c => c.IdMedioPagoNavigation).Where(c => c.Numero == NCobro).SingleOrDefault();
                RepoCobroClasesIndividual clases = new RepoCobroClasesIndividual();
                clases.Clases = cobro;
                clases.Nclase = NClase;
                return new ViewAsPdf("ReporteCobroClaseIndividual", clases);
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
                RepoReservas repoR = new RepoReservas();
                var reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Include(a => a.IdClienteNavigation).Include(a => a.IdClienteNavigation.Persona).Include(a => a.IdEmpleadoNavigation).Include(a => a.IdCobroNavigation).Include(a => a.IdEstadoNavigation).Where(a => a.FechaReserva >= FecD && a.FechaReserva <= FecH).ToList();
                repoR.Reservas = reserva;
                repoR.FecD = FecD.ToString("dd/MM/yyyy");
                repoR.FecH = FecH.ToString("dd/MM/yyyy");
                return new ViewAsPdf("ReporteReservas", repoR) {//FileName = "ReporteReservas.pdf" 
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
                r.FecD = FecD.ToString("dd/MM/yyyy");
                r.FecH = FecH.ToString("dd/MM/yyyy");
                repo.Add(r);
            }
          
            return new ViewAsPdf("ReporteJugadoresFrecuentes", repo)
            {

                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };
        }
        public IActionResult ReporteIngresosDiarios(DateTime FecD, DateTime FecH)
        {
            
          
            var cobro = _context.Cobro.Include(a => a.IdMedioPagoNavigation).Include(a => a.IdTarjetaNavigation).Include(a => a.IdUsuarioNavigation).Include(a => a.IdUsuarioNavigation.Persona).Include(a => a.DetalleCobro).Where(a => a.Fecha >= FecD.Date && a.Fecha <= FecH.Date).ToList().OrderBy(d => d.Fecha);
            RepoIngresosDiarios repo = new RepoIngresosDiarios();
            repo.Cobros = cobro;
            repo.FecD = FecD.ToString("dd/MM/yyyy");
            repo.FecH = FecH.ToString("dd/MM/yyyy");
            return new ViewAsPdf("ReporteIngresosDiarios", repo)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };

        }

        public IActionResult ReporteClasesParticulares(DateTime FecD, DateTime FecH)
        {
            HttpContext.Session.SetString("FecD", FecD.ToString("dd/MM/yyyy"));
            HttpContext.Session.SetString("FecH", FecH.ToString("dd/MM/yyyy"));
            var clase = _context.ClaseParticular.Include(a => a.IdCanchaNavigation).Include(a => a.IdJugadorNavigation).Include(a => a.IdCobroNavigation).Include(a => a.IdProfesorNavigation).Where(a => a.FechaReserva >= FecD.Date && a.FechaReserva <= FecH.Date).ToList().OrderBy(d => d.FechaReserva);
            RepoClases clases = new RepoClases();
            clases.Clases = clase;
            clases.FecD = FecD.ToString("dd/MM/yyyy");
            clases.FecH = FecH.ToString("dd/MM/yyyy");
            return new ViewAsPdf("ReporteClasesParticulares", clases)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };

        }

        public IActionResult ReporteCanchas(DateTime FecD, DateTime FecH)
        {
            
            List<CanchasEfectivas> lc = new List<CanchasEfectivas>();
            var CanchasReservas = (from d in _context.DetalleAlquilerCancha  join c in _context.Cancha on d.IdCancha equals c.Id join a in _context.AlquilerCancha on d.IdAlquilerCancha equals a.Numero where a.IdEstado == 4 && d.IdEstadoDetalle == 4 && a.FechaReserva >= FecD.Date && a.FechaReserva <= FecH.Date group c by new { dias= a.FechaReserva.Value.DayOfWeek , horas= d.HoraReservaDesde.Hours }  into g select new { Hora = g.Key.horas , dia = g.Key.dias , Cantidad = g.Count() , Id = g.Key }).ToList();


            foreach (var i in CanchasReservas)
            {
                CanchasEfectivas ce = new CanchasEfectivas();
                ce.Cantidad = i.Cantidad;
                ce.Hora = i.Hora;
                ce.Dia =  i.dia;
                ce.FecD = FecD.ToString("dd/MM/yyyy");
                ce.FecH = FecH.ToString("dd/MM/yyyy");
                lc.Add(ce);
            }

           

           
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
