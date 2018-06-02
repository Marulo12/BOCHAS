using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BOCHAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace BOCHAS.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly BOCHASContext _context;
        public HomeController(BOCHASContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult IndexJugadores()
        {
            return View();
        }
        public IActionResult NuevoJugador()
        {
          
            return View();

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
                    us.Contraseña = Contra;
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
                    per.NroDocumento = Convert.ToInt32(Numero);
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
        public JsonResult MostrarTipoJugador()
        {
            var tipo = _context.TipoJugador.ToList();
            return Json(tipo);

        }
        public bool ExistePersona(string documento)
        {
            if (_context.Persona.Where(p => p.NroDocumento == Convert.ToInt32(documento) && p.FechaBaja == null).Count() >= 1)
            {
                return true;
            }
            return false;
        }
        public async Task<JsonResult> ConocerDomicilio(string IdPersona)
        {
            int idP = Convert.ToInt32(IdPersona);
            var IdDomicilio = (from p in _context.Persona where p.Id == idP select new { IdDomicilio = p.IdDomicilio }).ToList();

            var Domicilio = (from d in _context.Domicilio join l in _context.Localidad on d.IdLocalidad equals l.Id join b in _context.Barrio on d.IdBarrio equals b.Id join p in _context.Persona on d.Id equals p.IdDomicilio join u in _context.Usuario on p.IdUsuario equals u.Id where d.Id == Convert.ToInt32(IdDomicilio[0].IdDomicilio) select new { barrio = b.Nombre, localidad = l.Nombre, numero = d.Numero, calle = d.Calle, usuario = u.Nombre, contra = u.Contraseña, Dpto = d.Departamento, Piso = d.Piso });
            return Json(await Domicilio.ToListAsync());
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
        public async Task<JsonResult> MostrarTipoDocumento()
        {

            return Json(await _context.TipoDocumento.ToListAsync());

        }
        public async Task<JsonResult> MostrarLocalidades()
        {

            return Json(await _context.Localidad.ToListAsync());

        }
        /*    public IActionResult Error()
            {
               return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            */
    }
}
