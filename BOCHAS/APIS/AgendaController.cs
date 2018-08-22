using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;

namespace BOCHAS.APIS
{
    [Produces("application/json")]
    
    public class AgendaController : Controller
    {
        private readonly BOCHASContext _context;

        public AgendaController(BOCHASContext context)
        {
            _context = context;
        }

        //api/Agenda/datetime con formato yyyy-MM-dd
       
        [HttpGet("api/Agenda/VerHorariosOcupados/{fecR}")]
        public JsonResult VerHorariosOcupados([FromRoute] DateTime fecR)
        {

            var agenda = (from a in _context.Agenda join c in _context.Cancha on a.IdCancha equals c.Id where a.Fecha == fecR.Date select new { Numero = c.Numero, Nombre = c.Nombre, Descripcion = c.Descripcion, HoraDesde = a.HoraDesde, HoraHasta = a.HoraHasta }).ToList();
            return Json(agenda);
        }









    
    }
}