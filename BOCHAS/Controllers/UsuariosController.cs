using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace BOCHAS.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly BOCHASContext _context;

        public UsuariosController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ValidarUsuario(string Usuario, string Contra)
        {
            // = _context.Usuario.Where(u => u.Nombre == Usuario && u.Contraseña == Contra).ToList();
            var usuario = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(p => p.FechaBaja == null && p.IdUsuarioNavigation.Nombre == Usuario && p.IdUsuarioNavigation.Contraseña == Contra).ToList();
            if (usuario.Count >= 1)
            {
                var claims = new List<Claim>
              {
               new Claim(ClaimTypes.Name,Usuario)

              };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                
                RegistrarIngresoSession(Convert.ToInt32(usuario[0].IdUsuario));
                if (usuario[0].Tipo == "EMPLEADO")
                {

                    return RedirectToAction("Index", "Home", "");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Mensaje"] = "Usuario Incorrecto";
                return RedirectToAction("Index");
            }
        }

        public void RegistrarIngresoSession(int IdUsuario)
        {
            Session Entrada = new Session();
            Entrada.IdUsuario = IdUsuario;
            Entrada.FechaInicio = DateTime.Now.Date;
            Entrada.HoraInicio = (TimeSpan)DateTime.Now.TimeOfDay;
            _context.Add(Entrada);
            _context.SaveChanges();
        }

        public async Task<IActionResult> Logout()
        {

            RegistrarSalidaSession(HttpContext.User.Identity.Name);
            await HttpContext.SignOutAsync(
    CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Usuarios", "");
        }

        public void RegistrarSalidaSession(string Usuario)
        {
            var Salida = (from p in _context.Session join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == Usuario && p.Id == _context.Session.Max(s => s.Id) select p).ToList();
            Salida[0].HoraFin = (TimeSpan)DateTime.Now.TimeOfDay;
            Salida[0].FechaFin = DateTime.Now.Date;
            _context.Session.Update(Salida[0]);
            _context.SaveChanges();
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .SingleOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

      

      
    }
}
