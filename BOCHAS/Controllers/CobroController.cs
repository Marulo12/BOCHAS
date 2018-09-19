using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

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
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta.ToList().Select(t=> new { Id=t.Id , Nombre= t.Nombre + "-" + t.TipoTarjeta }), "Id", "Nombre");            
            return View();
        }
        [HttpGet]
        // GET: Cobro/Create
        public IActionResult CobroClase(int Numero)
        {
            int canchas = _context.ClaseParticular.Where(c=>c.Id == Numero).ToList().Count();
            var horas = _context.ClaseParticular.Where(d => d.Id == Numero).SingleOrDefault().HoraFinPrevista - _context.ClaseParticular.Where(d => d.Id == Numero).SingleOrDefault().HoraInicioPrevista;
            var empleado = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(u => u.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name && u.Tipo == "EMPLEADO" && u.FechaBaja == null).SingleOrDefault();
            ViewData["canchas"] = canchas;
            ViewData["horas"] = horas;
            ViewData["empleado"] = empleado.Nombre + " " + empleado.Apellido;
            ViewData["Numero"] = Numero;
            ViewData["IdMedioPago"] = new SelectList(_context.MediodePago, "Id", "Nombre");
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta.ToList().Select(t => new { Id = t.Id, Nombre = t.Nombre + "-" + t.TipoTarjeta }), "Id", "Nombre");
            return View();
        }
        public async Task<JsonResult> ListadoServiciosAdicionales() {

            var listado = await _context.ServiciosAdicionales.Where(l=>l.Activo == true).ToListAsync();
            return Json(listado);
        }

        private bool CobroExists(int id)
        {
            return _context.Cobro.Any(e => e.Numero == id);
        }
        [HttpPost]
       
        public JsonResult RegistrarCobroReserva(int Nreserva, DateTime Fecha,int MedioPago,decimal MontoTotal,int? NroCupon,int? IdTarjeta,decimal MontoServicio,DetalleCobro Servicio,DetalleCobro[] ServiciosAdicionales )
        {
            int empleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;

            Cobro cobro = new Cobro();
            cobro.Fecha = Fecha.Date;
            cobro.IdMedioPago = MedioPago;
            cobro.MontoTotal = MontoTotal;
            cobro.IdUsuario = empleado;
            if (MedioPago == 2)
            {
                cobro.IdTarjeta = IdTarjeta;
                cobro.NroCupon = NroCupon;
            }
            _context.Cobro.Add(cobro);

            if (_context.SaveChanges() == 1)
            {
                int numeroCobro = _context.Cobro.Max(c => c.Numero);
                DetalleCobro dc = new DetalleCobro();              
                dc = Servicio;               
                dc.IdNumeroCobro = numeroCobro;
                dc.IdNumeroServicioAlquiler = Nreserva;
                _context.DetalleCobro.Add(dc);
                _context.SaveChanges();

                foreach (var d in ServiciosAdicionales)
                {
                    DetalleCobro dca = new DetalleCobro();
                    dca = d;
                    dca.IdNumeroCobro = numeroCobro;
                    dca.IdNumeroServicioAlquiler = Nreserva;
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
        [HttpPost]
        public JsonResult RegistrarCobroClase(int Nclase, DateTime Fecha, int MedioPago, decimal MontoTotal, int? NroCupon, int? IdTarjeta, decimal MontoServicio, DetalleCobro Servicio, DetalleCobro[] ServiciosAdicionales)
        {
            int empleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;

            Cobro cobro = new Cobro();
            cobro.Fecha = Fecha.Date;
            cobro.IdMedioPago = MedioPago;
            cobro.MontoTotal = MontoTotal;
            cobro.IdUsuario = empleado;
            if (MedioPago == 2)
            {
                cobro.IdTarjeta = IdTarjeta;
                cobro.NroCupon = NroCupon;
            }
            _context.Cobro.Add(cobro);

            if (_context.SaveChanges() == 1)
            {
                int numeroCobro = _context.Cobro.Max(c => c.Numero);
                DetalleCobro dc = new DetalleCobro();
                dc = Servicio;
                dc.IdNumeroCobro = numeroCobro;
                dc.IdNumeroServicioClases = Nclase;
                _context.DetalleCobro.Add(dc);
                _context.SaveChanges();

                foreach (var d in ServiciosAdicionales)
                {
                    DetalleCobro dca = new DetalleCobro();
                    dca = d;
                    dca.IdNumeroCobro = numeroCobro;
                    dca.IdNumeroServicioClases = Nclase;
                    _context.DetalleCobro.Add(dca);
                    _context.SaveChanges();
                }

                var clase = _context.ClaseParticular.Where(a => a.Id == Nclase).SingleOrDefault();
                clase.IdCobro = numeroCobro;
                _context.ClaseParticular.Update(clase);
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


        public IActionResult CobroManual() {

           
            var empleado = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(u => u.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name && u.Tipo == "EMPLEADO" && u.FechaBaja == null).SingleOrDefault();
            ViewData["empleado"] = empleado.Nombre + " " + empleado.Apellido;
            ViewData["IdMedioPago"] = new SelectList(_context.MediodePago, "Id", "Nombre");
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta.ToList().Select(t => new { Id = t.Id, Nombre = t.Nombre + "-" + t.TipoTarjeta }), "Id", "Nombre");
            return View();
        }
        public IActionResult CobroManualClases()
        {


            var empleado = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(u => u.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name && u.Tipo == "EMPLEADO" && u.FechaBaja == null).SingleOrDefault();
            ViewData["empleado"] = empleado.Nombre + " " + empleado.Apellido;
            ViewData["IdMedioPago"] = new SelectList(_context.MediodePago, "Id", "Nombre");
            ViewData["IdTarjeta"] = new SelectList(_context.Tarjeta.ToList().Select(t => new { Id = t.Id, Nombre = t.Nombre + "-" + t.TipoTarjeta }), "Id", "Nombre");
            return View();
        }

        public async Task<JsonResult> TraerPorReserva(int IdReserva)
        {

            var servicio = await _context.Servicio.Where(s => s.Id == 1).SingleOrDefaultAsync();
            decimal canchas = await _context.DetalleAlquilerCancha.Where(d => d.IdAlquilerCancha == IdReserva && d.IdEstadoDetalle == 4).CountAsync();
            TimeSpan horas =_context.DetalleAlquilerCancha.Where(d => d.IdAlquilerCancha == IdReserva && d.IdEstadoDetalle == 4).ToList()[0].HoraReservaHasta - _context.DetalleAlquilerCancha.Where(d => d.IdAlquilerCancha == IdReserva && d.IdEstadoDetalle == 4).ToList()[0].HoraReservaDesde;
            var total = Convert.ToDecimal(servicio.Precio) * Convert.ToDecimal(horas.TotalHours) * canchas;

            Subtotal sub = new Subtotal();
            sub.canchas = canchas;
            sub.horas = Convert.ToDecimal(horas.TotalHours);
            sub.precio = servicio.Precio;
            sub.servicio = servicio.Nombre;
            sub.total =  total;

            return Json(sub);
        }



        public async Task<JsonResult> ConsultarClasesPendientedeCobro(int idJugador)
        {

            var servicio = await _context.Servicio.Where(s => s.Id == 2).SingleOrDefaultAsync();
           
            var clases = await (from c in _context.ClaseParticular join p in _context.Persona on c.IdJugador equals p.Id where c.HoraFinReal != null && c.IdCobro == null && c.IdJugador == idJugador select new { numero = c.Id, servicio = servicio.Nombre, precio = servicio.Precio.ToString("N"), canchas = 1, horas = c.HoraFinPrevista.TotalHours - c.HoraInicioPrevista.TotalHours, total = servicio.Precio * (Convert.ToDecimal(c.HoraFinPrevista.TotalHours) - Convert.ToDecimal(c.HoraInicioPrevista.TotalHours)) }).ToListAsync();
            return Json(clases);
        }


        class Subtotal
        {
            public string servicio { set; get; }
            public decimal precio { set; get; }
            public decimal horas { set; get; }
            public decimal canchas { set; get; }
            public decimal total { set; get; }
        }



        [HttpPost]

        public JsonResult RegistrarCobroReservaManual(int[] Nreserva, DateTime Fecha, int MedioPago, decimal MontoTotal, int? NroCupon, int? IdTarjeta, decimal MontoServicio, DetalleCobro[] Servicio, DetalleCobro[] ServiciosAdicionales)
        {
            int empleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;

            Cobro cobro = new Cobro();
            cobro.Fecha = Fecha.Date;
            cobro.IdMedioPago = MedioPago;
            cobro.MontoTotal = MontoTotal;
            cobro.IdUsuario = empleado;
            if (MedioPago == 2)
            {
                cobro.IdTarjeta = IdTarjeta;
                cobro.NroCupon = NroCupon;
            }
            _context.Cobro.Add(cobro);

            if (_context.SaveChanges() == 1)
            {
                int numeroCobro = _context.Cobro.Max(c => c.Numero);
                foreach (var serv in Servicio)
                {
                    DetalleCobro dc = new DetalleCobro();
                    dc = serv;
                    dc.IdNumeroCobro = numeroCobro;
                    _context.DetalleCobro.Add(dc);
                    _context.SaveChanges();
                }
                               
                foreach (var d in ServiciosAdicionales)
                {
                    DetalleCobro dca = new DetalleCobro();
                    dca = d;
                    dca.IdNumeroCobro = numeroCobro;
                    _context.DetalleCobro.Add(dca);
                    _context.SaveChanges();
                }
                foreach (var res in Nreserva)
                {
                    var reserva = _context.AlquilerCancha.Where(a => a.Numero == res).SingleOrDefault();
                    reserva.IdCobro = numeroCobro;
                    _context.AlquilerCancha.Update(reserva);
                    _context.SaveChanges();
                }
                

                TempData["Respuesta"] = "Cobro";
                return Json(numeroCobro);
            }
            else
            {
                TempData["Respuesta"] = "NO";
                return Json("ERROR");
            }

        }


        public async Task<JsonResult> ConsultarCobroMensual()
        {
            var cobro = await _context.Cobro.Where(c => c.Fecha.Year == DateTime.Now.Year && c.Fecha.Month == DateTime.Now.Month).CountAsync();

            return Json(cobro);
        }
        [HttpPost]

        public JsonResult RegistrarCobroClaseManual(int[] Nrclase, DateTime Fecha, int MedioPago, decimal MontoTotal, int? NroCupon, int? IdTarjeta, decimal MontoServicio, DetalleCobro[] Servicio, DetalleCobro[] ServiciosAdicionales)
        {
            int empleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;

            Cobro cobro = new Cobro();
            cobro.Fecha = Fecha.Date;
            cobro.IdMedioPago = MedioPago;
            cobro.MontoTotal = MontoTotal;
            cobro.IdUsuario = empleado;
            if (MedioPago == 2)
            {
                cobro.IdTarjeta = IdTarjeta;
                cobro.NroCupon = NroCupon;
            }
            _context.Cobro.Add(cobro);

            if (_context.SaveChanges() == 1)
            {
                int numeroCobro = _context.Cobro.Max(c => c.Numero);
                foreach (var serv in Servicio)
                {
                    DetalleCobro dc = new DetalleCobro();
                    dc = serv;
                    dc.IdNumeroCobro = numeroCobro;
                    _context.DetalleCobro.Add(dc);
                    _context.SaveChanges();
                }

                foreach (var d in ServiciosAdicionales)
                {
                    DetalleCobro dca = new DetalleCobro();
                    dca = d;
                    dca.IdNumeroCobro = numeroCobro;
                    _context.DetalleCobro.Add(dca);
                    _context.SaveChanges();
                }
                foreach (var res in Nrclase)
                {
                    var clase = _context.ClaseParticular.Where(a => a.Id == res).SingleOrDefault();
                    clase.IdCobro = numeroCobro;
                    _context.ClaseParticular.Update(clase);
                    _context.SaveChanges();
                }


                TempData["Respuesta"] = "Cobro";
                return Json(numeroCobro);
            }
            else
            {
                TempData["Respuesta"] = "NO";
                return Json("ERROR");
            }

        }

        public JsonResult InfoVentas() {

            decimal? diario = 0;
            decimal? mensual = 0;
            decimal? anual = 0;
            foreach (var i in _context.Cobro.Where(c=>c.Fecha == DateTime.Now.Date).ToList())
            {
                diario += i.MontoTotal;
            }
            foreach (var i in _context.Cobro.Where(c => c.Fecha.Month == DateTime.Now.Month && c.Fecha.Year == DateTime.Now.Year).ToList())
            {
                mensual += i.MontoTotal;
            }
            foreach (var i in _context.Cobro.Where(c =>  c.Fecha.Year == DateTime.Now.Year).ToList())
            {
                anual += i.MontoTotal;
            }

            var lista = new { diario = diario, mensual = mensual, anual = anual };

            return Json(lista);
        }

        public async Task< JsonResult> GraficoSemanal()
        {
            var actual = await(from a in _context.Cobro where a.Fecha.Year == DateTime.Now.Year && a.Fecha.Month == DateTime.Now.Month group a by a.Fecha.DayOfWeek into g select new { dia = g.Key.ToString(), precio = g.Sum(a=>a.MontoTotal) }).ToArrayAsync();
            var anterior = await (from a in _context.Cobro where a.Fecha.Year == DateTime.Now.Year && a.Fecha.Month == DateTime.Now.Month - 1 group a by a.Fecha.DayOfWeek into g select new { dia = g.Key.ToString(), precio = g.Sum(a => a.MontoTotal) }).ToArrayAsync();
            var R = new[] { actual, anterior };
            return Json(R);
        }

    }
}
