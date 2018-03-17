using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using System.Configuration;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(sfAdmin.Startup))]

namespace sfAdmin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    //EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888            

            var redisConnection = ConfigurationManager.ConnectionStrings["RedisCache"].ConnectionString;
            GlobalHost.DependencyResolver.UseRedis(new RedisScaleoutConfiguration(redisConnection, "SignalR"));

            Global._sfAppLogger.Debug("Admin Application Started.");
        }
    }
}
