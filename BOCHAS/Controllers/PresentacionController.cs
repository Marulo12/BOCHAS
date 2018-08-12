using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BOCHAS.Controllers
{
    public class PresentacionController : Controller
    {
        public IActionResult Presentacion()
        {
            return View();
        }
    }
}