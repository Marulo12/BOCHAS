﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;

namespace BOCHAS.Controllers
{
    public class PersonasController : Controller
    {
        private readonly BOCHASContext _context;

        public PersonasController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: Personas
        public IActionResult Index()
        {
            return View();
        }
        public async Task<JsonResult> MostrarEmpleados(string filtro)
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
                                Cargo = c.Nombre

                            });

            if (!string.IsNullOrEmpty(filtro))
            {
                Empleado = Empleado.Where(p => p.Nombre.Contains(filtro) || p.Apellido.Contains(filtro));
            }

            return Json(await Empleado.ToListAsync());
        }
        public IActionResult RegistrarEmpleado()
        {

            return View();
        }
        [HttpPost]
        public JsonResult New(string Nombre, string Apellido, string TipoDoc, string Numero, string Mail, string Telefono, string Localidad, string Barrio, string usuario, string Contra, string Calle, string Cargo, string ncalle)
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
                    _context.Domicilio.Add(dom);
                    if (_context.SaveChanges() == 0)
                    {
                        return Json("ERROR");
                    }
                    var IdDom = _context.Domicilio.Max(i => i.Id);

                    //crea usuario
                    Usuario us = new Usuario();
                    us.Nombre = usuario;
                    us.Contraseña = Contra;
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
                    per.NroDocumento = Convert.ToInt32(Numero);
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

        public bool ExistePersona(string documento)
        {
            if (_context.Persona.Where(p => p.NroDocumento == Convert.ToInt32(documento)).Count() >= 1)
            {
                return true;
            }
            return false;
        }

        public async Task<JsonResult> ConocerDomicilio(string IdPersona)
        {
            int idP = Convert.ToInt32(IdPersona);
            var IdDomicilio = (from p in _context.Persona where p.Id == idP select new { IdDomicilio = p.IdDomicilio }).ToList();

            var Domicilio = (from d in _context.Domicilio join l in _context.Localidad on d.IdLocalidad equals l.Id join b in _context.Barrio on d.IdBarrio equals b.Id join p in _context.Persona on d.Id equals p.IdDomicilio join u in _context.Usuario on p.IdUsuario equals u.Id where d.Id == Convert.ToInt32(IdDomicilio[0].IdDomicilio) select new { barrio = b.Nombre, localidad = l.Nombre, numero = d.Numero, calle = d.Calle, usuario = u.Nombre, contra = u.Contraseña });
            return Json(await Domicilio.ToListAsync());
        }

        [HttpPost]

        public async  Task<IActionResult> Baja(string id)
        { int Id = Convert.ToInt32(id);
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.Id == Id );
            persona.FechaBaja = DateTime.Now;

            _context.Persona.Update(persona);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Personas","");
        }


        public IActionResult EditarEmpleado(int id)
        {
            var per = _context.Persona.Include(d => d.IdDomicilioNavigation).Include(u => u.IdUsuarioNavigation).Include(t => t.IdTipoDocumentoNavigation).Include(d => d.IdDomicilioNavigation.IdLocalidadNavigation).Include(d => d.IdDomicilioNavigation.IdBarrioNavigation).SingleOrDefault(p => p.Id == id);

            return PartialView(per);
        }
        public JsonResult MostrarBarrios(string IdLocalidad)
        {
            int IdL = Convert.ToInt32(IdLocalidad);
            var barrios = (from b in _context.Barrio where b.IdLocalidad == IdL select b).ToList();

            return Json(barrios);

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
        // GET: Personas/Create
        public IActionResult Create()
        {


            return View();
        }

        // POST: Personas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre,Apellido,Mail,Telefono,NumeroDocumento,Tipo,IdTipoDocumento,Id_Domicilio,Id_Usuario,Fecha_Baja")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                _context.Add(persona);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(persona);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Mail,Telefono,NumeroDocumento,Tipo,IdTipoDocumento,Id_Domicilio,Id_Usuario,FechaBaja")] Persona persona)
        {
            if (id != persona.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(persona);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(persona.Id))
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
            return View(persona);
        }

        // GET: Personas/Delete/5
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

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.Id == id);
            _context.Persona.Remove(persona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
            return _context.Persona.Any(e => e.Id == id);
        }
       
    }
   
}
