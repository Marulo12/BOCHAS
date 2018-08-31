using BOCHAS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System;

namespace BOCHAS.APIS
{
    [Produces("application/json", "application/octet-stream", "image/svg+xml")]
    

    public class PersonasController : Controller
    {
        private readonly BOCHASContext _context;
        private IHostingEnvironment _hostingEnv;
        public PersonasController(BOCHASContext context , IHostingEnvironment host)
        {
            _context = context;
            _hostingEnv = host;
        }

        // GET: api/Personas
        [HttpGet]
        public IEnumerable<Persona> GetPersona()
        {
            return _context.Persona;
        }

        // GET: api/Personas/5
        [HttpGet("api/Personas/GetJugador/{id}")]
        public async Task<IActionResult> GetJugador([FromRoute] int id)
        {
            var Jugador = (from p in _context.Persona
                           join d in _context.Domicilio on p.IdDomicilio equals d.Id
                           join t in _context.TipoDocumento on p.IdTipoDocumento equals t.Id
                           join b in _context.Barrio on d.IdBarrio equals b.Id
                           join l in _context.Localidad on d.IdLocalidad equals l.Id
                           join u in _context.Usuario on p.IdUsuario equals u.Id 
                           where p.Tipo.Contains("JUGADOR") && p.FechaBaja == null && u.Id == id
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
                               Barrio = b.Nombre,
                               Calle = d.Calle,
                               Ncalle = d.Numero,
                               IdTipoDoc = t.Id,
                               IdBarrio = b.Id,
                               IdLocalidad = l.Id,
                               Imagen = HttpContext.Request.Host + "/images/perfiles/jugadores/" + u.Nombre + "/" + p.Imagen,
                               Usuario = u.Nombre

        }).OrderBy(p => p.Nombre).OrderBy(p => p.Apellido);

            return Json(await Jugador.SingleOrDefaultAsync());


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

        
        [HttpPut("api/Personas/ModificarPass")]
        public JsonResult ModificarPass([FromBody] ModContra contranueva)
        {
            var Usuario = _context.Usuario.Where(u => u.Persona.Any(p => p.Id == contranueva.IdJugador)).SingleOrDefault();
            string hashNuevo = "";
            using (MD5 md5Hash = MD5.Create())
            {
                hashNuevo = Encriptador.GetMd5Hash(md5Hash, contranueva.contranueva);
            }
            Usuario.Contraseña = hashNuevo;
            _context.Update(Usuario);
            if (_context.SaveChanges() == 1)
            {
                return Json(Ok());
            }
            else
            {
                return Json(NotFound());
            }
        }


        private bool PersonaExists(int id)
        {
            return _context.Persona.Any(e => e.Id == id);
        }


        public class ModContra
        {
            public int IdJugador { get; set; }
            public string contranueva
            {
                get; set;
            }
        }

        [Produces("image/svg+xml", "application/octet-stream")]
        [HttpPost("api/Personas/SubirImagenJugador/{Usuario}")]
        public IActionResult SubirImagenJugador( [FromRoute] string Usuario   , [FromBody]  IFormFile ImageFile)
        {           
           try
           {

                
              //  FileInfo file = new FileInfo(img.ImageFile);
                
                var persona = (from u in _context.Usuario join p in _context.Persona on u.Id equals p.IdUsuario where u.Nombre == Usuario && p.Tipo == "JUGADOR" && p.FechaBaja == null select p).SingleOrDefault();
                var filename = ContentDispositionHeaderValue.Parse(ImageFile.ContentDisposition).FileName.Trim('"');
                var targetDirectory = Path.Combine(_hostingEnv.WebRootPath, string.Format("Images\\perfiles\\jugadores\\" + HttpContext.User.Identity.Name + "\\"));
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                var savePath = Path.Combine(targetDirectory, filename);
                ImageFile.CopyTo(new FileStream(savePath, FileMode.Create));
                persona.Imagen = filename;
                _context.Persona.Update(persona);
                _context.SaveChanges();
                
               return Ok();
           }
            catch
            {
                
                return NotFound();
            }


        }
    }
}