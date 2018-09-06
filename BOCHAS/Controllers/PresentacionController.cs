using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOCHAS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BOCHAS.Controllers
{
    public class PresentacionController : Controller
    {
        public IActionResult Presentacion()
        {
            return View();
        }

        public async Task<JsonResult> noticias()
        {
            BOCHASContext bd = new BOCHASContext();
            var noticias = await bd.Noticias.Where(a => a.Activo == true).ToListAsync();
            return Json(noticias);
        }
    }
}