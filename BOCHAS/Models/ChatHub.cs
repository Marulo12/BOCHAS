using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace BOCHAS.Models
{
    public class ChatHub: Hub
    {
       
        public async Task UsuariosConectados(string user)
        {
            BOCHASContext bd = new BOCHASContext();
            var usuario = (from u in bd.Usuario join s in bd.Session on u.Id equals s.IdUsuario where s.FechaFin != null && s.FechaInicio == DateTime.Now.Date select  new  { user = u.Nombre }).ToList().Distinct();
            await Clients.All.SendAsync("UsuariosConectado", usuario);
        }
    }
}
