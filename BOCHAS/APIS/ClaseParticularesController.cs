using System;
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
   
    public class ClaseParticularesController : Controller
    {
        private readonly BOCHASContext _context;

        public ClaseParticularesController(BOCHASContext context)
        {
            _context = context;
        }

        // GET: api/ClaseParticulares
        [HttpGet]
        public IEnumerable<ClaseParticular> GetClaseParticular()
        {
            return _context.ClaseParticular;
        }

        [HttpGet("api/ClaseParticulares/MostrarClases/{Usuario}")]
        public JsonResult MostrarClasesParticulares([FromRoute] string Usuario)
        {

            var usuario = _context.Usuario.Where(u => u.Nombre == Usuario).SingleOrDefault();
            var persona = _context.Persona.Where(p=>p.IdUsuario == usuario.Id).SingleOrDefault();
              var clases =  (from c in _context.ClaseParticular join can in _context.Cancha on c.IdCancha equals can.Id join p in _context.Persona on c.IdProfesor equals p.Id where c.IdJugador == persona.Id select new { Profesor = p.Nombre, DocProfesor = p.NroDocumento, IdClase = c.Id, FechaReserva = c.FechaReserva.Date.ToString("dd/MM/yyyy"), HoraDesde = c.HoraInicioPrevista, HoraHasta = c.HoraFinPrevista, IdCancha = can.Id, NombreCancha = can.Nombre, NroCancha = can.Numero , fechaC=c.FechaCancelacion}).ToList();
          //  var clases = _context.ClaseParticular.Include(c => c.IdCanchaNavigation).Where(c => c.IdJugador == persona.Id).ToList();
            return Json(clases);
        }
        // GET: api/ClaseParticulares/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClaseParticular([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var claseParticular = await _context.ClaseParticular.SingleOrDefaultAsync(m => m.Id == id);

            if (claseParticular == null)
            {
                return NotFound();
            }

            return Ok(claseParticular);
        }

        // PUT: api/ClaseParticulares/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClaseParticular([FromRoute] int id, [FromBody] ClaseParticular claseParticular)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != claseParticular.Id)
            {
                return BadRequest();
            }

            _context.Entry(claseParticular).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClaseParticularExists(id))
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

        // POST: api/ClaseParticulares
        [HttpPost]
        public async Task<IActionResult> PostClaseParticular([FromBody] ClaseParticular claseParticular)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ClaseParticular.Add(claseParticular);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClaseParticular", new { id = claseParticular.Id }, claseParticular);
        }

        // DELETE: api/ClaseParticulares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaseParticular([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var claseParticular = await _context.ClaseParticular.SingleOrDefaultAsync(m => m.Id == id);
            if (claseParticular == null)
            {
                return NotFound();
            }

            _context.ClaseParticular.Remove(claseParticular);
            await _context.SaveChangesAsync();

            return Ok(claseParticular);
        }

        private bool ClaseParticularExists(int id)
        {
            return _context.ClaseParticular.Any(e => e.Id == id);
        }
    }
}