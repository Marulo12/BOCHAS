using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOCHAS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BOCHAS.Hubs
{[HubName("chat")]
    public class Chat: Hub
    {
        BOCHASContext bd = new BOCHASContext();
        public void join(string user)
        {
           
            var users = (from u in bd.Usuario join s in bd.Session on u.Id equals s.IdUsuario join p in bd.Persona on u.Id equals p.IdUsuario where p.Tipo=="JUGADOR" &&  s.FechaInicio==DateTime.Now.Date && s.FechaFin == null select new { us=u.Nombre }).ToList().Distinct();
            Clients.All.join(users);
        }
        public void ReservasJugador()
        {
            var reservaJ = (from a in bd.AlquilerCancha join p in bd.Persona on a.IdCliente equals p.IdUsuario where a.IdEmpleado == null && a.IdEstado == 2 && a.FechaReserva >= DateTime.Now.Date select new { Numero = a.Numero, Nombre = p.Nombre, Apellido = p.Apellido }).ToList();                                
            Clients.All.ReservasJugador(reservaJ);
        }
    }
}
