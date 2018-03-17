using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Script.Serialization;
using sfAPIService.Providers;
using sfShareLib;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: OwinStartup(typeof(sfAPIService.Startup))]
namespace sfAPIService
{
    public class Startup
    {
        //public static NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();
        static sfLogLevel logLevel = (sfLogLevel)Enum.Parse(typeof(sfLogLevel), ConfigurationManager.AppSettings["sfLogLevel"]);
        public static sfLog _sfAppLogger = new sfLog(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"], ConfigurationManager.AppSettings["sfLogStorageContainerApp"], logLevel);

        public static JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(2),
                Provider = new OAuthProviders(),
                
            };
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());  
        }
        
    }    

}