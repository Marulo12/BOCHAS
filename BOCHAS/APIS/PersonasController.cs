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
using System.Net.Http;

namespace BOCHAS.APIS
{
    [Produces("application/json", "multipart/form-data", "application/x-www-form-urlencoded", "application/octet-stream", "image/svg+xml")]
    

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

        
        [HttpPut("api/Personas/ModificarJugador")]
        public JsonResult PutPersona([FromBody] Persona persona)
        {
            var personaBase = _context.Persona.Where(p => p.IdUsuario == persona.IdUsuario && p.FechaBaja == null && p.Tipo == "JUGADOR").SingleOrDefault();
            personaBase.Nombre = persona.Nombre;
            personaBase.Apellido = persona.Apellido;
            personaBase.IdTipoDocumento = persona.IdTipoDocumento;
            personaBase.Mail = persona.Mail;
            personaBase.Telefono = persona.Telefono;
            personaBase.NroDocumento = persona.NroDocumento;

            _context.Persona.Update(personaBase);
            if (_context.SaveChanges() == 1)
            {
                return Json(Ok());
            }
            else
            {
                return Json(NotFound());
            }
        }

        
        [HttpPut("api/Personas/ModificarPass")]
        public JsonResult ModificarPass([FromBody] ModContra contranueva)
        {
            var Usuario = _context.Usuario.Where(u => u.Persona.Any(p => p.IdUsuario == contranueva.IdJugador && p.FechaBaja == null && p.Tipo=="JUGADOR")).SingleOrDefault();
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


        [HttpPost("api/Personas/SubirImagen")]
        public JsonResult SubirImagen(  IFormFile file)
        {
            string Usuario = HttpContext.Request.Query["Usuario"].ToString();
            try
            {
                var persona = _context.Persona.SingleOrDefault(p => p.IdUsuarioNavigation.Nombre == Usuario && p.FechaBaja == null);
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var uploads = Path.Combine(_hostingEnv.WebRootPath, string.Format("Images\\perfiles\\jugadores\\" + Usuario));
                if (file.Length > 0)
                {
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    {
                        file.CopyToAsync(fileStream);
                    }
                    persona.Imagen = filename;
                    _context.Persona.Update(persona);
                    _context.SaveChanges();
                }

                return Json(Ok());
            }
            catch {
                return Json(NotFound());
            }
        }
        

      
    }
}