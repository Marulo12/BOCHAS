﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;

using System.Diagnostics;

namespace BOCHAS.Controllers
{
    [Authorize(Roles = "Empleado")]
    public class PersonasController : Controller
    {
        private readonly BOCHASContext _context;
        private IHostingEnvironment _hostingEnv;
        public PersonasController(BOCHASContext context , IHostingEnvironment hosting)
        {
            _hostingEnv = hosting;
            _context = context;
          
        }

        
        public IActionResult ConsultarEmpleado()
        {
            return View();
        }
        public IActionResult ConsultarJugador()
        {
            return View();
        }
        public async Task<JsonResult> MostrarEmpleados()
        {

            var Empleado = (from p in _context.Persona
                            from e in _context.Empleado
                            from c in _context.Cargo

                            where p.Id == e.IdPersona && e.IdCargo == c.Id && p.Tipo.Contains("EMPLEADO") && p.FechaBaja == null
                            select new
                            {
                                Id = p.Id,
                                Mail = p.Mail,
                                Nombre = p.Nombre,
                                Apellido = p.Apellido,
                                Documento = p.NroDocumento,
                                Cargo = c.Nombre,
                                Telefono = p.Telefono

                            }).OrderBy(u=>u.Nombre).OrderBy(u=>u.Apellido);
            
            return Json(await Empleado.ToListAsync());
        }
        public async Task<JsonResult> MostrarJugadores()
        {

            var Jugador = (from p in _context.Persona                                                 
                            where  p.Tipo.Contains("JUGADOR") && p.FechaBaja == null 
                            select  new 
                            {
                                
                                Id = p.Id,
                                Mail = p.Mail,
                                Nombre = p.Nombre,
                                Apellido = p.Apellido,
                                Documento = p.NroDocumento,
                               
                                Telefono = p.Telefono

                            }).OrderBy(u => u.Nombre).OrderBy(u => u.Apellido);
            
            return Json(await Jugador.ToListAsync());
        }
        public IActionResult RegistrarEmpleado()
        {

            return View();
        }
        public IActionResult RegistrarJugador()
        {

            return View();
        }
        [HttpPost]
        public JsonResult NewEmpleado(string Nombre, string Apellido, string TipoDoc, string Numero, string Mail, string Telefono, string Localidad, string Barrio, string usuario, string Contra, string Calle, string Cargo, string ncalle , string dpto , string piso)
        {
            try
            {
                //Verificar si existe esa persona
                if (ExistePersona(Numero) == false)
                {
                    // crea domicilio
                    Domicilio dom = new Domicilio();
                    dom.IdBarrio = Convert.ToInt32(Barrio);
                    dom.Numero = Convert.ToInt32(ncalle);
                    dom.IdLocalidad = Convert.ToInt32(Localidad);
                    dom.Calle = Calle;
                    
                        dom.Departamento = dpto;
                    if (!string.IsNullOrEmpty(piso))
                    {
                        dom.Piso = Convert.ToInt32(piso);
                    }
                    _context.Domicilio.Add(dom);
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }
                    var IdDom = _context.Domicilio.Max(i => i.Id);

                    //crea usuario
                    Usuario us = new Usuario();
                    us.Nombre = usuario;
                    using (MD5 md5Hash = MD5.Create())
                    {

                        string hash = Encriptador.GetMd5Hash(md5Hash, Contra);
                        us.Contraseña = hash;
                    }
                    _context.Usuario.Add(us);
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }
                    var IdUs = _context.Usuario.Max(i => i.Id);
                     
                    // crear persona
                    Persona per = new Persona();
                    per.IdTipoDocumento = Convert.ToInt32(TipoDoc);
                    per.IdDomicilio = IdDom;
                    per.IdUsuario = IdUs;
                    per.Mail = Mail;
                    per.Nombre = Nombre;
                    per.NroDocumento = Numero;
                    per.Telefono = Telefono;
                    per.Tipo = "EMPLEADO";
                    per.Apellido = Apellido;
                    
                    _context.Persona.Add(per);
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }
                    var IdPer = _context.Persona.Max(i => i.Id);

                    //Crear Empleado
                    Empleado em = new Empleado();
                    em.Activo = true;
                    em.FechaComienzo = DateTime.Now.Date;
                    em.IdCargo = Convert.ToInt32(Cargo);
                    em.IdPersona = IdPer;
                    _context.Empleado.Add(em);
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }

                    return Json("OK");
                }
                else
                {
                    return Json("EXISTE");
                }
            }
            catch { return Json("ERROR"); }
        }
        [HttpPost]
        public JsonResult NewJugador(string Nombre, string Apellido, string TipoDoc, string Numero, string Mail, string Telefono, string Localidad, string Barrio, string usuario, string Contra, string Calle, List<string> TipoJugador, string ncalle, string dpto, string piso)
        {
            try
            {
                //Verificar si existe esa persona
                if (ExistePersona(Numero) == false)
                {
                    // crea domicilio
                    Domicilio dom = new Domicilio();
                    dom.IdBarrio = Convert.ToInt32(Barrio);
                    dom.Numero = Convert.ToInt32(ncalle);
                    dom.IdLocalidad = Convert.ToInt32(Localidad);
                    dom.Calle = Calle;

                    dom.Departamento = dpto;
                    if (!string.IsNullOrEmpty(piso))
                    {
                        dom.Piso = Convert.ToInt32(piso);
                    }
                    _context.Domicilio.Add(dom);
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }
                    var IdDom = _context.Domicilio.Max(i => i.Id);

                    //crea usuario
                    Usuario us = new Usuario();
                    us.Nombre = usuario;
                    using (MD5 md5Hash = MD5.Create())
                    {
                       
                        string hash = Encriptador.GetMd5Hash(md5Hash, Contra);
                        us.Contraseña = hash;
                    }
                    _context.Usuario.Add(us);
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }
                    var IdUs = _context.Usuario.Max(i => i.Id);

                    // crea persona
                    Persona per = new Persona();
                    per.IdTipoDocumento = Convert.ToInt32(TipoDoc);
                    per.IdDomicilio = IdDom;
                    per.IdUsuario = IdUs;
                    per.Mail = Mail;
                    per.Nombre = Nombre;
                    per.NroDocumento = Numero;
                    per.Telefono = Telefono;
                    per.Tipo = "JUGADOR";
                    per.Apellido = Apellido;

                    _context.Persona.Add(per);
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }
                    var IdPer = _context.Persona.Max(i => i.Id);

                    //Crea Jugador
                    foreach (var j in TipoJugador)
                    {
                        Jugador ju = new Jugador();

                        ju.IdPersona = IdPer;
                        ju.IdTipoJugador = Convert.ToInt32(j.ToString());
                        _context.Jugador.Add(ju);
                    }
                    
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }

                    return Json("OK");
                }
                else
                {
                    return Json("EXISTE");
                }
            }
            catch { return Json("ERROR"); }
        }
        public bool ExistePersona(string documento)
        {
            if (_context.Persona.Where(p => p.NroDocumento == documento && p.FechaBaja == null).Count() >= 1 )
            {
                return true;
            }
            return false;
        }

        public async Task<JsonResult> ConocerDomicilio(string IdPersona)
        {
            int idP = Convert.ToInt32(IdPersona);
            var IdDomicilio = (from p in _context.Persona where p.Id == idP select new { IdDomicilio = p.IdDomicilio }).ToList();

            var Domicilio = (from d in _context.Domicilio join l in _context.Localidad on d.IdLocalidad equals l.Id join b in _context.Barrio on d.IdBarrio equals b.Id join p in _context.Persona on d.Id equals p.IdDomicilio join u in _context.Usuario on p.IdUsuario equals u.Id where d.Id == Convert.ToInt32(IdDomicilio[0].IdDomicilio) select new { barrio = b.Nombre, localidad = l.Nombre, numero = d.Numero, calle = d.Calle, usuario = u.Nombre, contra = u.Contraseña , Dpto = d.Departamento , Piso = d.Piso});
            return Json(await Domicilio.ToListAsync());
        }

        [HttpPost]

        public async  Task<IActionResult> BajaEmpleado(string id, string Motivo)
        { int Id = Convert.ToInt32(id);
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.Id == Id );
            persona.FechaBaja = DateTime.Now;
            var empleado = await _context.Empleado.SingleOrDefaultAsync(m => m.IdPersona == Id);
            empleado.Activo = false;
            empleado.MotivoBaja = Motivo;
            _context.Empleado.Update(empleado);
            _context.Persona.Update(persona);
            await _context.SaveChangesAsync();

            return RedirectToAction("ConsultarEmpleado", "Personas","");
        }
        [HttpPost]
        public async Task<JsonResult> BajaJugador(string id)
        {
            int Id = Convert.ToInt32(id);
            bool servicio_activo = false;
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.Id == Id);
            var reservas = await _context.AlquilerCancha.Where(a => a.IdClienteNavigation.Persona.SingleOrDefault().Id == persona.Id && (a.IdEstado == 2 || a.IdEstado == 3)).ToListAsync();
            var clases = await _context.ClaseParticular.Where(c => c.IdJugador == persona.Id && c.HoraFinReal == null && c.FechaCancelacion == null).ToListAsync();
            if (reservas.Count > 0 || clases.Count > 0)
            {
                servicio_activo = true;
            }

            if (!servicio_activo)
            {
                persona.FechaBaja = DateTime.Now;
                _context.Persona.Update(persona);
                await _context.SaveChangesAsync();
                
                return Json("BAJA");
            }
            else
            {
                
                return Json("NOBAJA");
            }
          

            
        }

        public IActionResult EditarEmpleado(int id)
        {
            var per = _context.Persona.Include(d => d.IdDomicilioNavigation).Include(u => u.IdUsuarioNavigation).Include(t => t.IdTipoDocumentoNavigation).Include(d => d.IdDomicilioNavigation.IdLocalidadNavigation).Include(d => d.IdDomicilioNavigation.IdBarrioNavigation).Include(p=>p.Empleado).Include(i=>i.Empleado.IdCargoNavigation).SingleOrDefault(p => p.Id == id);
            ViewData["Cargos"] = new SelectList(_context.Cargo, "Id", "Nombre");
            return PartialView(per);
        }
        public IActionResult EditarJugador(int id)
        {
            var per = _context.Persona.Include(d => d.IdDomicilioNavigation).Include(u => u.IdUsuarioNavigation).Include(t => t.IdTipoDocumentoNavigation).Include(d => d.IdDomicilioNavigation.IdLocalidadNavigation).Include(d => d.IdDomicilioNavigation.IdBarrioNavigation).SingleOrDefault(p => p.Id == id);
           
            return PartialView(per);
        }
        [AllowAnonymous]
        public JsonResult MostrarBarrios(string IdLocalidad)
        {
            try
            {
                int IdL = Convert.ToInt32(IdLocalidad);
                var barrios = (from b in _context.Barrio where b.IdLocalidad == IdL select b).ToList();
                return Json(barrios);
            }
            catch
            {
                return Json("");
            }
            

        }
        public async Task<JsonResult> MostrarLocalidades()
        {

            return Json(await _context.Localidad.ToListAsync());

        }

        public async Task<JsonResult> MostrarTipoDocumento()
        {

            return Json(await _context.TipoDocumento.ToListAsync());

        }
        public async Task<JsonResult> MostrarCargos()
        {

            return Json(await _context.Cargo.ToListAsync());

        }

        public JsonResult ValidarUsuario(string usuario)
        {
            var Usuario = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(u => u.IdUsuarioNavigation.Nombre == usuario && u.FechaBaja == null).Count();
            if (Usuario > 0)
            {
                return Json("False");

            }
            else
            {
                return Json("OK");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJugador(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var PersonaToUpdate = await _context.Persona
        .Include(i => i.IdDomicilioNavigation)
        .Include(i => i.IdUsuarioNavigation)                    
        .SingleOrDefaultAsync(m => m.Id == id);
            
            if (await TryUpdateModelAsync<Persona>(
                PersonaToUpdate))
            {
               
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "No se pudo guardar los cambios, verifique los datos..");
                }
                return RedirectToAction(nameof(ConsultarJugador));
            }
           
            return Redirect("ConsultarJugador");
        }

        public async Task<IActionResult> EditEmpleado(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var PersonaToUpdate = await _context.Persona
        .Include(i => i.IdDomicilioNavigation)
        .Include(i => i.IdUsuarioNavigation)
        .Include(i=>i.Empleado).Include(i=>i.Empleado.IdCargoNavigation)
        .SingleOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync<Persona>(
                PersonaToUpdate))
            {

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "No se pudo guardar los cambios, verifique los datos..");
                }
                return RedirectToAction(nameof(ConsultarEmpleado));
            }

            return Redirect("ConsultarEmpleado");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .SingleOrDefaultAsync(m => m.Id == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }
        public JsonResult MostrarTipoJugador()
        {
            var tipo = _context.TipoJugador.ToList();
            return Json(tipo);

        }
        public async Task<IActionResult> MostrarEmpleadoBajas()
        {

            var Empleado = _context.Empleado.Include(p => p.IdPersonaNavigation).Include(p => p.IdPersonaNavigation.IdDomicilioNavigation).Include(d => d.IdPersonaNavigation.IdDomicilioNavigation.IdBarrioNavigation).Include(d => d.IdPersonaNavigation.IdDomicilioNavigation.IdLocalidadNavigation).Include(u => u.IdPersonaNavigation.IdUsuarioNavigation).Where(e => e.Activo == false && e.IdPersonaNavigation.Tipo =="EMPLEADO");

            return View(await Empleado.ToListAsync());
        }

        public async Task<IActionResult> MostrarJugadorBajas()
        {

            var Jugador = _context.Persona.Include(p => p.IdDomicilioNavigation).Include(d => d.IdDomicilioNavigation.IdBarrioNavigation).Include(d => d.IdDomicilioNavigation.IdLocalidadNavigation).Include(u => u.IdUsuarioNavigation).Where( e => e.Tipo == "JUGADOR" && e.FechaBaja != null);

            return View(await Jugador.ToListAsync());
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.Id == id);
            _context.Persona.Remove(persona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ConsultarEmpleado));
        }

        private bool PersonaExists(int id)
        {
            return _context.Persona.Any(e => e.Id == id);
        }
        public async Task<JsonResult> ConocerTipoJugador(string id)
        {
            var tipoJugador = (from p in _context.Persona join j in _context.Jugador on p.Id equals j.IdPersona join tj in _context.TipoJugador on j.IdTipoJugador equals tj.Id where p.Id == Convert.ToInt32(id) select new { Nombre = tj.Nombre });
            return Json( await tipoJugador.ToListAsync());
        }
        [HttpPost]
        public JsonResult AgregarTipoJugador(string IdPersona , string tipoJugador)
        {
            var existeJugadorxTipo = _context.Jugador.Where(j => j.IdPersona == Convert.ToInt32(IdPersona) && j.IdTipoJugador == Convert.ToInt32(tipoJugador)).ToList().Count();
            if (existeJugadorxTipo > 0)
            {
                return Json("False");
            }
            else
            {
                Jugador jugador = new Jugador();
                jugador.IdPersona = Convert.ToInt32(IdPersona);
                jugador.IdTipoJugador = Convert.ToInt32(tipoJugador);
                _context.Jugador.Add(jugador);
                _context.SaveChanges(); 
                return Json("OK");
            }
           

        }
        public IActionResult AgregarImagenPerfil()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult AgregarImagenPerfilJugador()
        {
            return View();
        }
        [HttpPost,AllowAnonymous]
       
        public IActionResult SubirImagenJugador(IFormFile ImageFile)
        {
           
            try
            {
                var persona = _context.Persona.SingleOrDefault(p => p.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name && p.FechaBaja == null);
                var filename = ContentDispositionHeaderValue.Parse(ImageFile.ContentDisposition).FileName.Trim('"');
                var targetDirectory = Path.Combine(_hostingEnv.WebRootPath, string.Format("Images\\perfiles\\jugadores\\" + HttpContext.User.Identity.Name + "\\"));
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                string[] picList = Directory.GetFiles(targetDirectory);
                foreach (string f in picList)
                {
                    System.IO.File.Delete(f);
                }
                var savePath = Path.Combine(targetDirectory, filename);

                using (FileStream d = new FileStream(savePath, FileMode.Create))
                {
                    ImageFile.CopyTo(d);
                    d.Close();
                }
                
              
                persona.Imagen = filename;
                _context.Persona.Update(persona);
                _context.SaveChanges();
                
                return RedirectToAction("IndexJugadores", "Home", "");
           }
            catch
            {
                TempData["Mensaje"] ="Ingrese una imagen";
                return RedirectToAction("AgregarImagenPerfilJugador","Personas","");
            }


        }
        [HttpPost]
        public IActionResult SubirImagenEmpleados(IFormFile ImageFile)
        {
            try
            {
                var persona = _context.Persona.SingleOrDefault(p => p.IdUsuarioNavigation.Nombre == HttpContext.User.Identity.Name && p.FechaBaja == null);
                var filename = ContentDispositionHeaderValue.Parse(ImageFile.ContentDisposition).FileName.Trim('"');
                var targetDirectory = Path.Combine(_hostingEnv.WebRootPath, string.Format("Images\\perfiles\\empleados\\" + HttpContext.User.Identity.Name + "\\"));
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                string[] picList = Directory.GetFiles(targetDirectory);
                foreach (string f in picList)
                {
                    System.IO.File.Delete(f);
                }
                var savePath = Path.Combine(targetDirectory, filename);

                using (FileStream d = new FileStream(savePath, FileMode.Create))
                {
                    ImageFile.CopyTo(d);
                    d.Close();
                }

                persona.Imagen = filename;
                _context.Persona.Update(persona);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home", "");
            }
            catch {
                TempData["Mensaje"] = "Ingrese una imagen";
                return RedirectToAction("AgregarImagenPerfil");
            }


            }


        public IActionResult HorariosEmpleados()
        {
            ViewData["IdProfesor"] = new SelectList(_context.Persona.Where(p => p.FechaBaja == null && p.Tipo == "EMPLEADO").Select(x => new { Id = x.Id, persona = x.Nombre + " " + x.Apellido + ", Nro doc: " + x.NroDocumento }), "Id", "persona");
            return View();
        }
        [HttpPost]
        public JsonResult RegistrarHorariosProfe( HorariosProfesor[] turno)
        {
            try
            {
                foreach (var t in turno)
                {
                    var HoraProfe =  _context.HorariosProfesor.Where(h => h.IdProfesor == t.IdProfesor && h.Turno == t.Turno).SingleOrDefault();
                    if (HoraProfe != null)
                    {
                        HoraProfe.HoraDesde = t.HoraDesde;
                        HoraProfe.HoraHasta = t.HoraHasta;
                        _context.HorariosProfesor.Update(HoraProfe);
                         _context.SaveChanges();
                    }
                    else
                    {
                        HorariosProfesor horario = new HorariosProfesor();
                        horario.IdProfesor = t.IdProfesor;
                        horario.Turno = t.Turno;
                        horario.HoraDesde = t.HoraDesde;
                        horario.HoraHasta = t.HoraHasta;

                        _context.HorariosProfesor.Add(horario);
                         _context.SaveChanges();
                    }

                }


                return Json("OK");
            }
            catch {
                return Json("ERROR");
            }
        }

        public async Task <JsonResult> BuscarHorariosProfe(int profesor)
        {
            var horas =await _context.HorariosProfesor.Where(p => p.IdProfesor == profesor).ToListAsync();
            return Json(horas);
        }
        [AllowAnonymous]
        public async Task< IActionResult> ModificarPerfil(string Usuario)
        {
            var persona = await  _context.Persona.Include(p=>p.IdTipoDocumentoNavigation).Include(p=>p.IdDomicilioNavigation).Include(p=>p.IdDomicilioNavigation.IdBarrioNavigation).Include(p => p.IdDomicilioNavigation.IdLocalidadNavigation).Include(p=>p.IdUsuarioNavigation).Where(p => p.IdUsuarioNavigation.Nombre == Usuario && p.FechaBaja == null && p.Tipo == "JUGADOR").SingleOrDefaultAsync();
            ViewData["Localidad"] = new SelectList(_context.Localidad,"Id","Nombre");
            ViewData["Barrio"] = new SelectList(_context.Barrio, "Id", "Nombre");
            ViewData["TipoDoc"] = new SelectList(_context.TipoDocumento, "Id", "Nombre");
            return View(persona);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ModificarPerfil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var PersonaToUpdate = await _context.Persona
        .Include(i => i.IdDomicilioNavigation)
        .Include(i => i.IdUsuarioNavigation)
        .SingleOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync<Persona>(
                PersonaToUpdate))
            {

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    TempData["Respuesta"] = "ERROR";
                    return RedirectToAction("IndexJugadores", "Home", "");
                }
               
            }
            TempData["Respuesta"] = "OK";
            return RedirectToAction("IndexJugadores","Home","");          
        }
    }
   
}
