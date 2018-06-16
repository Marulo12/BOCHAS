using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOCHAS.Hubs
{[HubName("chat")]
    public class Chat: Hub
    {
        public void join()
        {
            Clients.All.join($"{Context.ConnectionId } se conecto");
        }
    }
}
