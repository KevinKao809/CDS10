using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace sfAPIService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                 name: "ExternalApi",
                 routeTemplate: "cdstudio/{action}/{id}",
                 defaults: new { controller = "ExternalApi", id = RouteParameter.Optional }
             );

            config.Routes.MapHttpRoute(
                 name: "DeviceApi",
                 routeTemplate: "device-api/{action}/{id}",
                 defaults: new { controller = "DeviceApi", id = RouteParameter.Optional }
             );

            config.Routes.MapHttpRoute(
                name: "AdminApi",
                routeTemplate: "admin-api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
