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
            var agenda = (from a in _context.Agenda select new { title="Nombre:"+a.IdCanchaNavigation.Nombre + " Numero:" + a.IdCanchaNavigation.Numero + " Descripcion:" + a.IdCanchaNavigation.Descripcion, allDay = false , start=a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraDesde) , end = a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraHasta), backgroundColor = ColorXTipodeServicio(a.IdAlquilerCancha, a.IdTorneo , a.IdClasesParticulares) }).ToList();
            return Json(agenda);
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
      
    }
}
