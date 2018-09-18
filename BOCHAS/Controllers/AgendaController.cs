using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace BOCHAS.Controllers
{
    [Authorize]
    public class AgendaController : Controller
    {
       
        private readonly BOCHASContext _context;

        public AgendaController(BOCHASContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> MostrarHorariosOcupados(string fecR)
        {
           
            ViewData["fecha"] = fecR;
            var agenda = _context.Agenda.Include(a => a.IdCanchaNavigation).Where(a => a.Fecha == Convert.ToDateTime(fecR).Date).OrderBy(a => a.HoraDesde);
            return PartialView( await agenda.ToListAsync());
        }

        public JsonResult ArmarAgenda()
        {
            EliminarReservascaducadas();
            var agenda = (from a in _context.Agenda select new {id= a.Id , title="Cancha N°: " + a.IdCanchaNavigation.Numero , allDay = false , start=a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraDesde) , end = a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraHasta), backgroundColor = ColorXTipodeServicio(a.IdAlquilerCancha, a.IdTorneo , a.IdClasesParticulares) }).ToList();
            return Json(agenda);
        }

        public JsonResult ArmarAgendaXCancha(int cancha)
        {
            EliminarReservascaducadas();
            if (cancha > 0)
            {
                var agenda = (from a in _context.Agenda where a.IdCancha == cancha select new { id = a.Id, title = "Nombre:" + a.IdCanchaNavigation.Nombre + " Numero:" + a.IdCanchaNavigation.Numero + " Descripcion:" + a.IdCanchaNavigation.Descripcion, allDay = false, start = a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraDesde), end = a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraHasta), backgroundColor = ColorXTipodeServicio(a.IdAlquilerCancha, a.IdTorneo, a.IdClasesParticulares) }).ToList();
                return Json(agenda);
            }
            else
            {
                var agenda = (from a in _context.Agenda  select new { id = a.Id, title = "Nombre:" + a.IdCanchaNavigation.Nombre + " Numero:" + a.IdCanchaNavigation.Numero + " Descripcion:" + a.IdCanchaNavigation.Descripcion, allDay = false, start = a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraDesde), end = a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraHasta), backgroundColor = ColorXTipodeServicio(a.IdAlquilerCancha, a.IdTorneo, a.IdClasesParticulares) }).ToList();
                return Json(agenda);
            }
            
         
        }

        public void EliminarReservascaducadas()
        {
            var agenda = _context.Agenda.Where(a => a.Fecha < DateTime.Now.Date).ToList();
            foreach (var a in agenda)
            {
                _context.Agenda.Remove(a);
                _context.SaveChanges();
            }
            
        }
        public string ColorXTipodeServicio(int? reserva , int? torneo , int? clases)
        { string color = "";
            if (reserva != null)
            {
                color = "CadetBlue";
            }
            if (torneo != null)
            {
                color = "orangered";
            }
            if (clases != null)
            {
                color = "dimgrey";
            }
            return color;
        }

        public async Task<IActionResult> MostrarEvento(string evento) {
            int even = Convert.ToInt32(evento);
            var eventos = _context.Agenda.Include(a => a.IdAlquilerCanchaNavigation).Include(a=>a.IdClasesParticularesNavigation).Include(a=>a.IdAlquilerCanchaNavigation.IdClienteNavigation).Include(a=>a.IdAlquilerCanchaNavigation.IdEmpleadoNavigation).Include(a => a.IdClasesParticularesNavigation.IdJugadorNavigation).Include(a => a.IdClasesParticularesNavigation.IdProfesorNavigation).Include(a => a.IdCanchaNavigation).Where(a => a.Id == even).SingleOrDefaultAsync();

            return PartialView(await eventos);

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
