using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace sfSuperAdmin.Controllers
{
    [HubName("RTMessageHub")]
    public class RTMessageHub : Hub
    {
        public void Register()
        {
            PublishMessage("{\"message\":\"welcome\"}");
        }

        public void PublishMessage(string message)
        {
            Clients.All.onReceivedMessage(message);
        }
    }
}