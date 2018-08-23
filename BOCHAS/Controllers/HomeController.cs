using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BOCHAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;


namespace BOCHAS.Controllers
{
   
    public class HomeController : Controller
    {
       
        private readonly BOCHASContext _context;
        public HomeController(BOCHASContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Empleado"), AutoValidateAntiforgeryToken]
        public IActionResult Index()
        {           
            return View();
        }

        [Authorize(Roles = "Jugador"), AutoValidateAntiforgeryToken]
        public IActionResult IndexJugadores()
        {
            return View();
        }
      
         
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
