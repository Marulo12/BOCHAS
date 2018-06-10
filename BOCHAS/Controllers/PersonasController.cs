using System;
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

namespace BOCHAS.Controllers
{
    [Authorize]
    public class PersonasController : Controller
    {
        private readonly BOCHASContext _context;

        public PersonasController(BOCHASContext context)
        {
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
        public async Task<IActionResult> BajaJugador(string id)
        {
            int Id = Convert.ToInt32(id);
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.Id == Id);
            persona.FechaBaja = DateTime.Now;
            
            _context.Persona.Update(persona);
            await _context.SaveChangesAsync();

            return RedirectToAction("ConsultarJugador", "Personas", "");
        }

        public IActionResult EditarEmpleado(int id)
        {
            var per = _context.Persona.Include(d => d.IdDomicilioNavigation).Include(u => u.IdUsuarioNavigation).Include(t => t.IdTipoDocumentoNavigation).Include(d => d.IdDomicilioNavigation.IdLocalidadNavigation).Include(d => d.IdDomicilioNavigation.IdBarrioNavigation).Include(p=>p.Empleado).SingleOrDefault(p => p.Id == id);

            return PartialView(per);
        }
        public IActionResult EditarJugador(int id)
        {
            var per = _context.Persona.Include(d => d.IdDomicilioNavigation).Include(u => u.IdUsuarioNavigation).Include(t => t.IdTipoDocumentoNavigation).Include(d => d.IdDomicilioNavigation.IdLocalidadNavigation).Include(d => d.IdDomicilioNavigation.IdBarrioNavigation).SingleOrDefault(p => p.Id == id);

            return PartialView(per);
        }
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
        .Include(i=>i.Empleado)
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

            var Jugador = _context.Jugador.Include(p => p.IdPersonaNavigation).Include(p => p.IdPersonaNavigation.IdDomicilioNavigation).Include(d => d.IdPersonaNavigation.IdDomicilioNavigation.IdBarrioNavigation).Include(d => d.IdPersonaNavigation.IdDomicilioNavigation.IdLocalidadNavigation).Include(u => u.IdPersonaNavigation.IdUsuarioNavigation).Where( e => e.IdPersonaNavigation.Tipo == "JUGADOR" && e.IdPersonaNavigation.FechaBaja != null);

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
      
    }
   
}
