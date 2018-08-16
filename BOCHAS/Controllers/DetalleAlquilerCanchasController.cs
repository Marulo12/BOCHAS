using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace BOCHAS.Controllers
{
    public class DetalleAlquilerCanchasController : Controller
    {
        private readonly BOCHASContext _context;

        public DetalleAlquilerCanchasController(BOCHASContext context)
        {
            _context = context;
        }

        public IActionResult ComenzarCancha(int NDreserva)
        {
            var detalle = _context.DetalleAlquilerCancha.Where(d => d.Id == NDreserva).SingleOrDefault();
            var reserva = _context.AlquilerCancha.Include(a => a.DetalleAlquilerCancha).Where(a => a.Numero == detalle.IdAlquilerCancha).SingleOrDefault();
            reserva.IdEstado = 3;
            reserva.IdEmpleado = (from p in _context.Persona join u in _context.Usuario on p.IdUsuario equals u.Id where u.Nombre == HttpContext.User.Identity.Name && p.Tipo == "EMPLEADO" && p.FechaBaja == null select u).SingleOrDefault().Id;
            _context.AlquilerCancha.Update(reserva);
            if (_context.SaveChanges() == 1)
            {
                detalle.IdEstadoDetalle = 3;

                var cancha = _context.Cancha.Where(c => c.Id == detalle.IdCancha).SingleOrDefault();
                cancha.IdEstadoCancha = 1;
                _context.DetalleAlquilerCancha.Update(detalle);
                _context.Cancha.Update(cancha);
                _context.SaveChanges();
                TempData["Respuesta"] = "COMENZADO";
                return RedirectToAction("ConsultarReservas", "AlquilerCanchas", "");
            }

            else
            {
                TempData["Respuesta"] = "NO";
                return RedirectToAction("ConsultarReservas", "AlquilerCanchas", "");
            }
        }

        public IActionResult CancelarCancha(int NDreserva)
        {                      
            var detalle = _context.DetalleAlquilerCancha.Where(d => d.Id == NDreserva).SingleOrDefault();
            var reserva = _context.AlquilerCancha.Include(a => a.IdClienteNavigation.Persona).Include(a => a.DetalleAlquilerCancha).Where(a => a.Numero == detalle.IdAlquilerCancha).SingleOrDefault();
            int nreserva = reserva.Numero;
            DateTime? freserva = reserva.FechaReserva;
            string apellido = reserva.IdClienteNavigation.Persona.SingleOrDefault().Apellido;
            string mail = reserva.IdClienteNavigation.Persona.SingleOrDefault().Mail;
            int cancha = _context.Cancha.Where(c => c.Id == detalle.IdCancha).SingleOrDefault().Numero;
            detalle.IdEstadoDetalle = 5;
            var agenda = _context.Agenda.Where(a => a.IdAlquilerCancha == detalle.IdAlquilerCancha && a.IdCancha == detalle.IdCancha).SingleOrDefault();
            _context.DetalleAlquilerCancha.Update(detalle);
            _context.Agenda.Remove(agenda);
            _context.SaveChanges();
              //if (_context.SaveChanges() == 1)
             // {
                  int cantidadCanceladas = 0;
                  foreach (var r in reserva.DetalleAlquilerCancha)
                  {
                      if (r.IdEstadoDetalle == 5) {
                          cantidadCanceladas++;
                      }
                  }
                  if (cantidadCanceladas == reserva.DetalleAlquilerCancha.Count)
                  {
                      reserva.IdEstado = 5;
                      _context.AlquilerCancha.Update(reserva);
                      _context.SaveChanges();
                  }
                  try
                  {
                      var mensaje = new MimeMessage();
                      mensaje.From.Add(new MailboxAddress("BOCHAS PADEL", "bochaspadel@gmail.com"));
                      mensaje.To.Add(new MailboxAddress("Jugador", mail));
                      mensaje.Subject = "Cancelacion de Reserva";
                      mensaje.Body = new TextPart("plain") { Text = "Buenos dias  Sr/a. " + apellido + " se realizo la cancelacion en la reserva N°" + nreserva + " de la fecha " + freserva.Value.Date.ToString("dd/MM/yyyy") + " la cancha N°" + cancha + ", Saludos." };
                      using (var cliente = new SmtpClient())
                      {
                          cliente.Connect("smtp.gmail.com", 587, false);
                          cliente.Authenticate("bochaspadel@gmail.com", "bochas2018");
                          cliente.Send(mensaje);
                          cliente.Disconnect(true);
                      }
                      TempData["Respuesta"] = "Cancelado";
                      return RedirectToAction("ConsultarReservas", "AlquilerCanchas", "");
                  }
                  catch
                  {
                      TempData["Respuesta"] = "NoMail";
                      return RedirectToAction("ConsultarReservas", "AlquilerCanchas", "");
                  }

          //    }
            //  else
          //    {
          //        TempData["Respuesta"] = "NO";
          //        return RedirectToAction("ConsultarReservas", "AlquilerCanchas", "");
          //    }


        }

    }


}

