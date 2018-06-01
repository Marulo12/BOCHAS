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


        public IActionResult NuevoJugador()
        {
          

            return View();

        }
        [HttpPost]
        public IActionResult NewJugador(string Nombre, string Apellido, string TipoDoc, string Numero, string Mail, string Telefono, string Localidad, string Barrio, string usuario, string Contra, string Calle, string ncalle, string dpto, string piso)
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
                   
                        Jugador ju = new Jugador();

                        ju.IdPersona = IdPer;
                        ju.IdTipoJugador = 1;
                        _context.Jugador.Add(ju);
                    

                    if (_context.SaveChanges() == 0)
                    {
                        return RedirectToAction("Index", "Usuarios", "");
                    }

                    return RedirectToAction("Index","Usuarios","");
                }
                else
                {
                    return RedirectToAction("Index", "Usuarios", "");
                }
            }
            catch { return RedirectToAction("Index", "Usuarios", ""); }
        }
        public bool ExistePersona(string documento)
        {
            if (_context.Persona.Where(p => p.NroDocumento == Convert.ToInt32(documento) && p.FechaBaja == null).Count() >= 1)
            {
                return true;
            }
            return false;
        }
        /*    public IActionResult Error()
            {
               return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            */
    }
}
