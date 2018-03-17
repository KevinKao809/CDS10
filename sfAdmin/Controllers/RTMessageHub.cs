using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using sfAdmin.Models;
using Newtonsoft.Json.Linq;

namespace sfAdmin.Controllers
{
    [HubName("RTMessageHub")]
    public class RTMessageHub : Hub
    {
        public async Task Register(string CompanyId)
        {
            string clientOrigin = Context.Headers["Origin"];
            string hubHost = Context.Headers["Host"];
            if (clientOrigin!=null && clientOrigin.Contains("//"))
                clientOrigin = clientOrigin.Substring(clientOrigin.IndexOf("//")+2);            

            if (clientOrigin!=null && clientOrigin != hubHost)
            {
                Global._sfAppLogger.Warn("Client Origin: " + clientOrigin);
                Global._sfAppLogger.Warn("Check Allow Domain ...");
                string allowDomain = "";
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper(false, int.Parse(CompanyId));
                    allowDomain = await apiHelper.callAPIService("GET", Global._companyAllowDomainEndPoint, null);
                    allowDomain = allowDomain.Trim('"');
                }
                catch (Exception ex)
                {
                    Global._sfAppLogger.Error("Company Allow Domain API Exception: " + ex.Message + "," + ex.InnerException.Message);
                    return;
                }

                if (allowDomain != "*")
                {
                    if (!allowDomain.Contains(clientOrigin))
                    {
                        Global._sfAppLogger.Warn("Unauthorized. Allow Domain (" + allowDomain + "); Request Domain (" + clientOrigin + ")");
                        return;
                    }
                }
                Global._sfAppLogger.Warn("Authorized. Allow Domain (" + allowDomain + "); Request Domain (" + clientOrigin + ")");
            }
            await Groups.Add(Context.ConnectionId, CompanyId);
            PublishMessageByCompanyId(CompanyId, "{\"topic\":\"welcome\"}");
        }

        private void PublishMessageByCompanyId(string CompanyId, string message)
        {
            Clients.Group(CompanyId).onReceivedMessage(message);
        }
    }
}