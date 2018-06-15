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

        
       
    }
}
