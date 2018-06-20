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
using System.Security.Cryptography;

using Microsoft.AspNetCore.SignalR;
using BOCHAS.Hubs;

namespace BOCHAS.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly BOCHASContext _context;
        private readonly IHubContext<Chat> _hubContext;
        public UsuariosController(BOCHASContext context, IHubContext<Chat> hubContext)
        {
            _hubContext = hubContext;
            _context = context;           
        }

        // GET: Usuarios
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ValidarUsuario(string Usuario, string Contra)
        {
            string hash = "";
            using (MD5 md5Hash = MD5.Create())
            {
                 hash =Encriptador.GetMd5Hash(md5Hash, Contra);
            }
                var usuario = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(p => p.FechaBaja == null && p.IdUsuarioNavigation.Nombre == Usuario && p.IdUsuarioNavigation.Contraseña == hash).ToList();
            if (usuario.Count >= 1)
            {
                var claims = new List<Claim>
              {
               new Claim(ClaimTypes.Name,usuario[0].IdUsuarioNavigation.Nombre)
               
              };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                };                            
                RegistrarIngresoSession(Convert.ToInt32(usuario[0].IdUsuario));
                if (usuario[0].Tipo == "EMPLEADO")
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Empleado"));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "Home", "");
                }                              
                else
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Jugador"));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("IndexJugadores", "Home", "");

                }
                
            }
            else
            {
                TempData["Mensaje"] = "Usuario Incorrecto";
                return RedirectToAction("Index");
            }
        }

        public  void RegistrarIngresoSession(int IdUsuario)
        {
            Session Entrada = new Session();
            Entrada.IdUsuario = IdUsuario;
            Entrada.FechaInicio = DateTime.Now.Date;
            Entrada.Origen = 1;
            Entrada.HoraInicio = (TimeSpan)DateTime.Now.TimeOfDay;
            _context.Add(Entrada);
            _context.SaveChanges();
                       
        }

       
        public async Task<IActionResult> Logout()
        {
            try {
                RegistrarSalidaSession(HttpContext.User.Identity.Name);
                NotificarSalidaSession();
                           
            } catch {

                NotFound();
            }
                await HttpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Usuarios", "");
                   
        }

        public void RegistrarSalidaSession(string Usuario)
        {
            var SalidaMax = (from p in _context.Session join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == Usuario && p.FechaFin == null select p).Max(p=>p.Id);
            var Salida = _context.Session.SingleOrDefault(s => s.Id == SalidaMax);
            Salida.HoraFin = (TimeSpan)DateTime.Now.TimeOfDay;
            Salida.FechaFin = DateTime.Now.Date;
            _context.Session.Update(Salida);
            _context.SaveChanges();
        }
        public void NotificarSalidaSession()
        {
            var users = (from u in _context.Usuario join s in _context.Session on u.Id equals s.IdUsuario join p in _context.Persona on u.Id equals p.IdUsuario where p.Tipo == "JUGADOR" && s.FechaInicio == DateTime.Now.Date && s.FechaFin == null select new { us = u.Nombre }).ToList().Distinct();
            _hubContext.Clients.All.join(users);
        }

      public JsonResult VerificarContraseña(string contraactual)
        {
            string hash = "";
            using (MD5 md5Hash = MD5.Create())
            {
                hash = Encriptador.GetMd5Hash(md5Hash, contraactual);
            }
            var Usuario = _context.Usuario.Where(u => u.Nombre == HttpContext.User.Identity.Name && u.Contraseña == hash).ToList().Count();

            if (Usuario > 0)
            {
                return Json("OK");
            }
            else
            {
                return Json("False");
            }
        }
        [Microsoft.AspNetCore.Authorization.AuthorizeAttribute]
        
        public async Task<IActionResult> ConocerPerfil()
        {
            var Perfil = _context.Persona.Include(p => p.IdDomicilioNavigation).Include(p => p.IdUsuarioNavigation).Include(p => p.IdTipoDocumentoNavigation).Include(p => p.IdDomicilioNavigation.IdLocalidadNavigation).Include(p => p.IdDomicilioNavigation.IdBarrioNavigation).Where(p => p.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name).SingleOrDefaultAsync();

            return PartialView(await Perfil);
        }
        public JsonResult CambiarContraseña(string contraactual,string contranueva)
        {
            string hash = "";
            using (MD5 md5Hash = MD5.Create())
            {
                hash = Encriptador.GetMd5Hash(md5Hash, contraactual);
            }
            var Usuario = _context.Usuario.Where(u => u.Nombre == HttpContext.User.Identity.Name && u.Contraseña == hash).SingleOrDefault();

            string hashNuevo = "";
            using (MD5 md5Hash = MD5.Create())
            {
                hashNuevo = Encriptador.GetMd5Hash(md5Hash, contranueva);
            }

            Usuario.Contraseña = hashNuevo;
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

        public  JsonResult PermisosNavBar()
        {
            

            var jugador = (from j in _context.Jugador join p in _context.Persona on j.IdPersona equals p.Id join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.FechaBaja == null select new { jugador = j.IdTipoJugador }).ToList();


            return  Json( jugador);
        }

    }
}
