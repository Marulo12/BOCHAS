using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOCHAS.Models;

namespace BOCHAS.Hubs
{[HubName("chat")]
    public class Chat: Hub
    {
        public void join(string user)
        {
            BOCHASContext bd = new BOCHASContext();
            var users = (from u in bd.Usuario join s in bd.Session on u.Id equals s.IdUsuario join p in bd.Persona on u.Id equals p.IdUsuario where p.Tipo=="JUGADOR" &&  s.FechaInicio==DateTime.Now.Date && s.FechaFin == null select new { us=u.Nombre }).ToList().Distinct();
            Clients.All.join(users);
        }
    }
}
