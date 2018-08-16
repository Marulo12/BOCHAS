﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;

namespace BOCHAS.APIS
{
    [Produces("application/json")]
    [Route("api/Personas")]
    public class PersonasController : Controller
    {
        private readonly BOCHASContext _context;

        public PersonasController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: api/Personas
        [HttpGet]
        public IEnumerable<Persona> GetPersona()
        {
            return _context.Persona;
        }

        // GET: api/Personas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJugador([FromRoute] int id)
        {
            var Jugador = (from p in _context.Persona join d in _context.Domicilio on p.IdDomicilio equals d.Id join t in _context.TipoDocumento on p.IdTipoDocumento equals t.Id join b in _context.Barrio on d.IdBarrio equals b.Id join l in _context.Localidad on d.IdLocalidad equals l.Id 
                           where p.Tipo.Contains("JUGADOR") && p.FechaBaja == null && p.Id == id
                           select new
                           {

                               Id = p.Id,
                               Mail = p.Mail,
                               Nombre = p.Nombre,
                               Apellido = p.Apellido,
                               Documento = p.NroDocumento,
                               TipoDoc = t.Nombre,
                               Telefono = p.Telefono,
                               Localidad = l.Nombre,
                               Barrio = b.Nombre

                           }).OrderBy(p => p.Nombre).OrderBy(p => p.Apellido);

            return Json(await Jugador.ToListAsync());

           
        }

        // PUT: api/Personas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersona([FromRoute] int id, [FromBody] Persona persona)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != persona.Id)
            {
                return BadRequest();
            }

            _context.Entry(persona).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Personas
      

        private bool PersonaExists(int id)
        {
            return _context.Persona.Any(e => e.Id == id);
        }
    }
}