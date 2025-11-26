using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using WEBSGI.Controllers;
using ActionFilterAttribute = System.Web.Mvc.ActionFilterAttribute;

namespace WEBSGI.Models
{
    public class VerificarSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Excluir la acción Logout para que no sea interceptada por el filtro
            if (filterContext.ActionDescriptor.ActionName.Equals("Logout", StringComparison.OrdinalIgnoreCase))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            var usuario = (Usuarios)HttpContext.Current.Session["Usuario"];

            if (usuario == null || usuario.NecesitaCambiarContrasena == true)
            {
                // Si no es el controlador de Login, redirige a Login
                if (!(filterContext.Controller is LoginController))
                {
                    filterContext.Result = new RedirectResult("~/Login/Login");
                    return;
                }
            }
            else
            {
                // Si el usuario está logueado y accede al Login, redirige a Inicio
                if (filterContext.Controller is LoginController)
                {
                    filterContext.Result = new RedirectResult("~/Inicio/Index");
                    return;
                }
                // Pasa el usuario al ViewBag para que esté disponible en las vistas
                filterContext.Controller.ViewBag.User = usuario;
            }

            base.OnActionExecuting(filterContext);
        }
    }

}