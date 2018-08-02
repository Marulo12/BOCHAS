﻿using System;
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
            var agenda = _context.Agenda.Include(a => a.IdCanchaNavigation).Where(a => a.Fecha == Convert.ToDateTime(fecR)).OrderBy(a => a.HoraDesde);
            return PartialView( await agenda.ToListAsync());
        }

        public JsonResult ArmarAgenda()
        {
            Random rnd = new Random();
            string[] colores = new string[11];
            colores[0] = "red";
            colores[1] = "blue";
            colores[2] = "black";
            colores[3] = "green";
            colores[4] = "BurlyWood";
            colores[6] = "CadetBlue";
            colores[7] = "DarkSalmon";
            colores[8] = "DarkSlateGrey";
            colores[9] = "FireBrick";
            colores[10] = "Indigo";
            var agenda = (from a in _context.Agenda select new { title="Nombre:"+a.IdCanchaNavigation.Nombre + " Numero:" + a.IdCanchaNavigation.Numero + " Descripcion:" + a.IdCanchaNavigation.Descripcion, allDay = false , start=a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraDesde) , end = a.Fecha.Date.ToString("yyyy-MM-dd") + "T" + Convert.ToString(a.HoraHasta), backgroundColor = colores[rnd.Next(0,10)] }).ToList();
            return Json(agenda);
        }
      
    }
}
