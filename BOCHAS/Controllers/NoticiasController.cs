using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace BOCHAS.Controllers
{   [Authorize]
    public class NoticiasController : Controller
    {
        private readonly BOCHASContext _context;
        private IHostingEnvironment _hostingEnv;
        public NoticiasController(BOCHASContext context, IHostingEnvironment hosting)
        {
            _context = context;
            _hostingEnv = hosting;
        }

        [HttpPost]
        public IActionResult CrearNoticia( string titulo, string descripcion, IFormFile ImageFile)
        {
            try
            {              
                var filename = ContentDispositionHeaderValue.Parse(ImageFile.ContentDisposition).FileName.Trim('"');
                var targetDirectory = Path.Combine(_hostingEnv.WebRootPath, string.Format("Images\\Noticias\\" ));
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                var savePath = Path.Combine(targetDirectory, filename);
                ImageFile.CopyToAsync(new FileStream(savePath, FileMode.Create));           
                
                    Noticias noti = new Noticias();
                    noti.Titulo = titulo;
                    noti.Descripcion = descripcion;
                    noti.Url = "http://" +  Url.ActionContext.HttpContext.Request.Host.Value + "/images/Noticias/" + filename;
                    noti.Activo = true;
                    _context.Noticias.Add(noti);
                    if (_context.SaveChanges() == 1)
                    {
                        return RedirectToAction("Index", "Home", "");
                    }
                    else
                    {
                        TempData["Mensaje"] = "Error en la operacion";
                        return RedirectToAction("Index", "Home", "");
                    }                            
            }
            catch
            {
                TempData["Mensaje"] = "Ingrese una imagen";
                return RedirectToAction("AgregarImagenPerfil");
            }
        }

        public async Task<JsonResult> ConocerNoticias()
        {
            var noticia = _context.Noticias.Where(n => n.Activo == true).OrderByDescending(o => o.Titulo).ToListAsync();

            return Json(await noticia);
        }

        public JsonResult BajadeNoticia(int id)
        {
            var noti = _context.Noticias.SingleOrDefault(n=>n.Id ==  id);
          
            noti.Activo = false;
            _context.Noticias.Update(noti);

            if (_context.SaveChanges() == 1)
            {                                          
               
                return Json("OK");
            }
            return Json("ERROR");
        }

    }
}
