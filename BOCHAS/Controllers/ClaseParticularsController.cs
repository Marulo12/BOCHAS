using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;

namespace BOCHAS.Controllers
{
    public class ClaseParticularsController : Controller
    {
        private readonly BOCHASContext _context;

        public ClaseParticularsController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: ClaseParticulars
        public  IActionResult Index()
        {
            ViewData["IdJugador"] = new SelectList(_context.Persona.Where(p=>p.Tipo =="JUGADOR" && p.FechaBaja == null &&  _context.Jugador.Where(j=>j.IdPersona == p.Id && j.IdTipoJugador == 2).Count() > 0).Select( x => new { Id = x.Id , persona = x.Nombre + " " + x.Apellido + ", Nro doc: " + x.NroDocumento  }), "Id", "persona");
            ViewData["IdProfesor"] = new SelectList(_context.Persona.Where(p=>p.FechaBaja == null && p.Tipo =="EMPLEADO").Select(x => new { Id = x.Id, persona = x.Nombre + " " + x.Apellido + ", Nro doc: " + x.NroDocumento }), "Id", "persona");
            return View();
        }

    }
}
