using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBSGI.Models;
using WEBSGI.Models.ViewModel;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class PermisosController : Controller
    {
        private readonly RepositorioUsuarios _repositorioUsuarios;

        public PermisosController() 
        {
            _repositorioUsuarios = new RepositorioUsuarios();

        }
        public PermisosController(RepositorioUsuarios repositorioUsuario)
        {
            _repositorioUsuarios = repositorioUsuario;
        }


        [HttpGet]
        public ActionResult Index()
        {
            
           List<Puestos> puestos = _repositorioUsuarios.ObtenerTodosLosPuestos();

            var model = new PuestosViewModel
            {
                Puestos = puestos
            };

            return View(model);
        }
        [HttpGet]
        public JsonResult BuscarPermisosPorPuesto(int idPuesto)
        {
            var permisos = _repositorioUsuarios.BuscarPorIdPuesto(idPuesto);
            return Json(permisos, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult VerificarPermiso(int acceso)
        {
            if (Session["PUESTO"] == null || string.IsNullOrEmpty(Session["PUESTO"].ToString()))
            {
                return Json(new { success = false, message = "Sesión no válida. Inicie sesión nuevamente." }, JsonRequestBehavior.AllowGet);
            }

            int idPuesto = Convert.ToInt32(Session["PUESTO"]);
            bool tienePermiso = _repositorioUsuarios.VerificarPermiso(idPuesto, acceso);

            return Json(new { success = true, permiso = tienePermiso }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GuardarPermisos(int idPuesto, List<int> permisos)
        {
            try
            {
                _repositorioUsuarios.EliminarPorIdPuesto(idPuesto);

                foreach (var acceso in permisos)
                {
                    _repositorioUsuarios.AgregarPermiso(idPuesto, acceso);
                }

                return Json(new { success = true, message = "Permisos guardados correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar permisos: " + ex.Message });
            }
        }

    }
}
