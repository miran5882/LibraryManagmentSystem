using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LibraryManagementSystem
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable CORS with specific origins (for testing)
            var cors = new EnableCorsAttribute(
                origins: "http://localhost:44305",
                headers: "*",
                methods: "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
   