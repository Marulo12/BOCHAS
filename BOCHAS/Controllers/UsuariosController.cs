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
using Microsoft.AspNetCore.Authorization;

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
                    var tipojugador = _context.Jugador.Where(j => j.IdPersona == usuario[0].Id && j.IdTipoJugador == 2).Count();
                    if (tipojugador > 0)
                    {
                        return RedirectToAction("IndexJugadores", "Home");
                    }
                    else
                    {
                        TempData["Mensaje"] = "El usuario no tiene cuenta de jugador";
                        return RedirectToAction("Index");
                    }
                    
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

      public JsonResult VerificarContraseña(string contraactual)
        {
            var Usuario = _context.Usuario.Where(u => u.Nombre == HttpContext.User.Identity.Name && u.Contraseña == contraactual).ToList().Count();

            if (Usuario > 0)
            {
                return Json("OK");
            }
            else
            {
                return Json("False");
            }
        }
        [Authorize]
        public async Task<IActionResult> ConocerPerfil()
        {
            var Perfil = _context.Persona.Include(p => p.IdDomicilioNavigation).Include(p => p.IdUsuarioNavigation).Include(p => p.IdTipoDocumentoNavigation).Include(p => p.IdDomicilioNavigation.IdLocalidadNavigation).Include(p => p.IdDomicilioNavigation.IdBarrioNavigation).Where(p => p.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name).SingleOrDefaultAsync();

            return PartialView(await Perfil);
        }
        public JsonResult CambiarContraseña(string contraactual,string contranueva)
        {
            var Usuario = _context.Usuario.Where(u => u.Nombre == HttpContext.User.Identity.Name && u.Contraseña == contraactual).SingleOrDefault();

            Usuario.Contraseña = contranueva;
            _context.Update(Usuario);
            if (_context.SaveChanges()==1)
            {
                return Json("OK");
            }
            else
            {
                return Json("False");
            }
        }



    }
}
