using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace WEBSGI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        //        ✔ Si Session["Usuario"] es null, el usuario es redirigido a la página de Login.
        //        ✔ Permite el acceso a Login/Login sin problemas.
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
          

        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
