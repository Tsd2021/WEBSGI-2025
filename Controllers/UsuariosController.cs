using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBSGI.Models;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly RepositorioDocumentos _repositorioDocumentos;
        private readonly RepositorioLog _repositorioLog;
        private readonly RepositorioNoConformidad _repositorioNoConformidades;
        private readonly RepositorioConfigOrganizacion _repositorioConfigOrganizacion;
        private readonly RepositorioUsuarios _repositorioUsuarios;
        public UsuariosController()
        {
            _repositorioLog = new RepositorioLog();
            _repositorioConfigOrganizacion = new RepositorioConfigOrganizacion();
            _repositorioDocumentos = new RepositorioDocumentos();
            _repositorioNoConformidades = new RepositorioNoConformidad();
            _repositorioUsuarios = new RepositorioUsuarios();
        }

        public UsuariosController(
            RepositorioDocumentos repositorioDocumentos,
            RepositorioLog repositorioLog,
            RepositorioNoConformidad repositorioNoConformidad,
            RepositorioConfigOrganizacion repositorioConfigOrganizacion,
            RepositorioUsuarios repositorioUsuarios)
        {
            _repositorioDocumentos = repositorioDocumentos;
            _repositorioLog = repositorioLog;
            _repositorioNoConformidades = repositorioNoConformidad;
            _repositorioConfigOrganizacion = repositorioConfigOrganizacion;
            _repositorioUsuarios = repositorioUsuarios;
        }

        // GET: Usuarios
        public ActionResult Index()
        {
            List<Usuarios> tipos = _repositorioConfigOrganizacion.TraerUsuarios();

            return View(tipos);
        }


        [HttpPost]
        public JsonResult ActualizarNivel(int id, string nivel)
        {
            try
            {
                _repositorioUsuarios.ActualizarNivelUsuario(id, nivel);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}