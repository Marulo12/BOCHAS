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
using Microsoft.AspNetCore.SignalR;
using BOCHAS.Hubs;
using MimeKit;
using MailKit.Net.Smtp;
using System.Drawing;


namespace BOCHAS.APIS
{
    [Produces("application/json", "multipart/form-data", "application/x-www-form-urlencoded", "application/octet-stream", "image/svg+xml")]
    

    public class PersonasController : Controller
    {
        private readonly BOCHASContext _context;
        private IHostingEnvironment _hostingEnv;
        private readonly IHubContext<Chat> _hubContext;
        public PersonasController(BOCHASContext context , IHostingEnvironment host , IHubContext<Chat> _hub)
        {
            _context = context;
            _hostingEnv = host;
            _hubContext = _hub;
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


        

        public class imagenes {
            public string img { set; get; }
            public string nomImagen { set; get; }
            public int IdJugador { set; get; }
        }
        [HttpPost("api/Personas/SubirImagen")]
        public JsonResult SubirImagen([FromBody] imagenes image)
        {
           
            try
            {
                var persona = _context.Persona.Include(p=>p.IdUsuarioNavigation).SingleOrDefault(p => p.IdUsuario == image.IdJugador && p.FechaBaja == null);
                var imageDataByteArray = Convert.FromBase64String(image.img);
               

                var uploads = Path.Combine(_hostingEnv.WebRootPath, string.Format("Images\\perfiles\\jugadores\\" + persona.IdUsuarioNavigation.Nombre));
                if (imageDataByteArray.Length > 0)
                {
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    string[] picList = Directory.GetFiles(uploads);
                    foreach (string f in picList)
                    {
                        System.IO.File.Delete(f);
                    }

                    System.IO.File.WriteAllBytes(uploads + @"/" + image.nomImagen, imageDataByteArray);
                    persona.Imagen = image.nomImagen;
                    _context.Persona.Update(persona);
                    _context.SaveChanges();
                }

                return Json(Ok());
          }
          catch
            {
               return Json(NotFound());
           }




        }








        public class Users {
            public string Usuario { set; get; }
            public string Contra { set; get; }
        }
        [HttpPost("api/Personas/Login")]
        public async Task<JsonResult> Login([FromBody] Users U)
        {
            try
            {
                string hash = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    hash = Encriptador.GetMd5Hash(md5Hash, U.Contra);
                }
                var usuario = await _context.Persona.Include(p => p.IdUsuarioNavigation).Where(p => p.FechaBaja == null && p.IdUsuarioNavigation.Nombre == U.Usuario && p.IdUsuarioNavigation.Contraseña == hash && p.Tipo == "JUGADOR").SingleOrDefaultAsync();

                if (usuario != null)
                {
                    RegistrarIngresoSession(Convert.ToInt32(usuario.IdUsuario));
                    var IdJugador = new { IdUsuario = usuario.Id };
                    return Json(IdJugador);
                }
                else
                {
                    var IdJugador = new { IdUsuario = 0 };
                    return Json(IdJugador);

                }
            }
            catch  {
                var IdJugador = new { IdUsuario = -1 };
                return Json(IdJugador); }
        }

        public void RegistrarIngresoSession(int IdUsuario)
        {
            ComprobarSesionesAnteriores(IdUsuario);
               Session Entrada = new Session();
            Entrada.IdUsuario = IdUsuario;
            Entrada.FechaInicio = DateTime.Now.Date;
            Entrada.Origen = 1;
            Entrada.HoraInicio = (TimeSpan)DateTime.Now.TimeOfDay;
            _context.Add(Entrada);
           
            _context.SaveChanges();
            NotificarSession();

        }

        [HttpGet("api/Personas/Logout/{Usuario}")]
        public JsonResult Logout(string Usuario)
        {
            try
            {
                RegistrarSalidaSession(Usuario);
                NotificarSession();

            }
            catch
            {

                return Json(NotFound());
            }
           
            return Json(Ok());

        }

        public void RegistrarSalidaSession(string Usuario)
        {
            var SalidaMax = (from p in _context.Session join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == Usuario && p.FechaFin == null select p).Max(p => p.Id);
            var Salida = _context.Session.SingleOrDefault(s => s.Id == SalidaMax);
            Salida.HoraFin = (TimeSpan)DateTime.Now.TimeOfDay;
            Salida.FechaFin = DateTime.Now.Date;
            _context.Session.Update(Salida);
            _context.SaveChanges();
        }
        public void NotificarSession()
        {
            var users = (from u in _context.Usuario join s in _context.Session on u.Id equals s.IdUsuario join p in _context.Persona on u.Id equals p.IdUsuario where p.Tipo == "JUGADOR" && s.FechaInicio == DateTime.Now.Date && s.FechaFin == null select new { us = u.Nombre }).ToList().Distinct();
            _hubContext.Clients.All.join(users);
        }

        public void ComprobarSesionesAnteriores(int IdUsuario)
        {
            try
            {
                var sesiones = _context.Session.Where(s => s.IdUsuario == IdUsuario && (s.HoraFin == null || s.FechaFin == null)).ToList();
                if (sesiones.Count > 0)
                {
                    foreach (var i in sesiones)
                    {
                        i.HoraFin = DateTime.Now.TimeOfDay;
                        i.FechaFin = i.FechaInicio;
                        _context.Session.Update(i);
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {

            }
        }

     
        [HttpGet("api/Personas/ResetearContraseña/{mail}")]
        public JsonResult ResetearContraseña(string mail)
        {
            try
            {
                Random rand = new Random();
                int Clave = rand.Next(1000, 5000);
                var persona = _context.Persona.Include(p => p.IdUsuarioNavigation).Where(p => p.Mail == mail && p.Tipo=="JUGADOR" && p.FechaBaja == null).SingleOrDefault();
                string nuevaClave = persona.IdUsuarioNavigation.Nombre + Convert.ToString(Clave);
                string hashNuevo = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    hashNuevo = Encriptador.GetMd5Hash(md5Hash, nuevaClave);
                }
                persona.IdUsuarioNavigation.Contraseña = hashNuevo;
                _context.Persona.Update(persona);
                _context.SaveChanges();
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress("BOCHAS PADEL", "bochaspadel@gmail.com"));
                mensaje.To.Add(new MailboxAddress("Jugador", persona.Mail));               
                mensaje.Subject = "Blanqueo de clave";
                mensaje.Body = new TextPart("plain") { Text = "Buenos dias  Sr/a. " + persona.Apellido + " le comentamos que se reseteo su clave, las misma ahora es = " + nuevaClave + ", por su seguridad cambiela por una personal, Saludos." };
                using (var cliente = new SmtpClient())
                {
                    cliente.Connect("smtp.gmail.com", 587, false);
                    cliente.Authenticate("bochaspadel@gmail.com", "bochas2018");
                    cliente.Send(mensaje);
                    cliente.Disconnect(true);
                }
                return Json(Ok());
           }
            catch
            {
                return Json(NotFound());
            }

        }

    }
}