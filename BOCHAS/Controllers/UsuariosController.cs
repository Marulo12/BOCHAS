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
            var usuario = _context.Usuario.Where(u => u.Nombre == Usuario && u.Contraseña == Contra).ToList();

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
                  //  ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = true,
                    //IssuedUtc = <DateTimeOffset>
                    // The time at which the authentication ticket was issued.
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                var persona = _context.Persona.Where(p => p.IdUsuario == usuario[0].Id).ToList();
                RegistrarIngresoSession( Convert.ToInt32( persona[0].IdUsuario));
                if (persona[0].Tipo == "EMPLEADO")
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

        public  void RegistrarIngresoSession(int IdUsuario)
        {
            Session Entrada = new Session();
            Entrada.IdUsuario = IdUsuario;
            Entrada.FechaInicio = DateTime.Now.Date;
            Entrada.HoraInicio = (TimeSpan) DateTime.Now.TimeOfDay;                      
            _context.Add(Entrada);
          _context.SaveChanges();
        }

        public async Task<IActionResult> Logout()
        {
            
            RegistrarSalidaSession(HttpContext.User.Identity.Name);
            await HttpContext.SignOutAsync(
    CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Usuarios","");
        }

        public  void RegistrarSalidaSession( string Usuario)
        {
            var Salida = (from p in _context.Session join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == Usuario && p.Id == _context.Session.Max(s => s.Id) select p).ToList() ;
            Salida[0].HoraFin =(TimeSpan) DateTime.Now.TimeOfDay;
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

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Contraseña")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.SingleOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Contraseña")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.SingleOrDefaultAsync(m => m.Id == id);
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}
