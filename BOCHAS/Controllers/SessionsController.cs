using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using Microsoft.AspNetCore.Authorization;
using BOCHAS.Hubs;

namespace BOCHAS.Controllers
{
    [Authorize(Roles = "Empleado")]
    public class SessionsController : Controller
    {
        private readonly BOCHASContext _context;
       
        public SessionsController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public  IActionResult Index()
        {
           
            return View();
        }

        
        public async Task<IActionResult> MostrarSessiones(string fechadesde, string fechahasta)
        {
            if (fechadesde == null && fechahasta == null)
            {
                return NotFound();
            }

            var session = await _context.Session.Include(s => s.IdUsuarioNavigation).Where(s =>s.FechaInicio >= Convert.ToDateTime(fechadesde) && s.FechaInicio <= Convert.ToDateTime(fechahasta)).OrderByDescending(u=>u.FechaInicio).OrderByDescending(u=>u.HoraInicio).ToListAsync();
            if (session == null)
            {
                return NotFound();
            }

            return PartialView(session);
        }


        public async Task<JsonResult> MostrarTotalSesiones() {

            return Json(await _context.Session.Where(s=>s.FechaInicio.Month == DateTime.Now.Month).CountAsync());

        }

        public JsonResult MostrarSesionesMensuales()
        {
            // var sesiones = _context.Session.Where(s=>s.FechaInicio.Month == DateTime.Now.Month && s.FechaInicio.Year == DateTime.Now.Year).ToList();
            var sesiones = _context.Session.ToList();
            int Monday = 0; int Tuesday = 0; int Wednesday = 0; int Thursday = 0; int Friday = 0; int Saturday = 0; int Sunday = 0;

            foreach (var s in sesiones)
            {
                switch (s.FechaInicio.DayOfWeek.ToString())
                {
                    case "Monday":
                        Monday++;
                        break;
                    case "Tuesday":
                        Tuesday++;
                        break;
                    case "Wednesday":
                        Wednesday++;
                        break;
                    case "Thursday":
                        Thursday++;
                        break;
                    case "Friday":
                        Friday++;
                        break;
                    case "Saturday":
                        Saturday++;
                        break;
                    case "Sunday":
                        Sunday++;
                        break;
                }
            }

            int[] ls = new int[] { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
            return Json(ls.ToList());
        }

    }
}
