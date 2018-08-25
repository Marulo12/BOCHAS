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

     
     
        [HttpGet]
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
        [HttpPost]
       
        public JsonResult RegistrarCobroReserva(int Nreserva, DateTime Fecha,int MedioPago,decimal MontoTotal,int NroCupon,int IdTarjeta,decimal MontoServicio,DetalleCobro Servicio,DetalleCobro[] ServiciosAdicionales )
        {
            int empleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;

            Cobro cobro = new Cobro();
            cobro.Fecha = Fecha.Date;
            cobro.IdMedioPago = MedioPago;
            cobro.MontoTotal = MontoTotal;
            cobro.IdUsuario = empleado;
            _context.Cobro.Add(cobro);

            if (_context.SaveChanges() == 1)
            {
                int numeroCobro = _context.Cobro.Max(c => c.Numero);
                DetalleCobro dc = new DetalleCobro();
                dc = Servicio;
                dc.IdNumeroCobro = numeroCobro;
                _context.DetalleCobro.Add(dc);
                _context.SaveChanges();

                foreach (var d in ServiciosAdicionales)
                {
                    DetalleCobro dca = new DetalleCobro();
                    dca = d;
                    dca.IdNumeroCobro = numeroCobro;
                    _context.DetalleCobro.Add(dca);
                    _context.SaveChanges();
                }
                
                var reserva = _context.AlquilerCancha.Where(a => a.Numero == Nreserva).SingleOrDefault();
                reserva.IdCobro = numeroCobro;
                _context.AlquilerCancha.Update(reserva);
                _context.SaveChanges();

                TempData["Respuesta"] = "Cobro";
                return Json(numeroCobro);
            }
            else
            {
                TempData["Respuesta"] = "NO";
                return Json("ERROR");
            }

        }

        public IActionResult MostrarCobros()
        {
            return View();
        }
        public async Task<IActionResult> ConsultarCobros(DateTime? fecD, DateTime? fecH)
        {
            if (fecD != null || fecH != null)
            {
                var cobro = _context.Cobro.Include(c => c.AlquilerCancha).Include(c => c.DetalleCobro).Include(c => c.IdMedioPagoNavigation).Include(c => c.IdTarjetaNavigation).Include(c => c.IdUsuarioNavigation).Where(c => c.Fecha >= fecD.Value.Date && c.Fecha <= fecH.Value.Date).ToListAsync();
                return PartialView(await cobro);
            }
            else
            {
                var cobro = _context.Cobro.Include(c => c.AlquilerCancha).Include(c => c.DetalleCobro).Include(c => c.IdMedioPagoNavigation).Include(c => c.IdTarjetaNavigation).Include(c => c.IdUsuarioNavigation).ToListAsync();
                return PartialView(await cobro);

            }



        }
    }
}
