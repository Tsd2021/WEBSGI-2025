using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WEBSGI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
     name: "Default",
     url: "{controller}/{action}/{id}",
     defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
 );
            // En RouteConfig.cs (si usas rutas personalizadas)
            routes.MapRoute(
                name: "ViewDocument",
                url: "Documentos/ViewDocument/{id}",
                defaults: new { controller = "Documentos", action = "ViewDocument" }
            );

        }
    }
}
