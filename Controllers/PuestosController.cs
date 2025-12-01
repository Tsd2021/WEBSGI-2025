using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Org.BouncyCastle.Tls;
using WEBSGI.Models;
using WEBSGI.Models.ViewModel;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class PuestosController : Controller
    {
        private readonly RepositorioDocumentos _repositorioDocumentos;
        private readonly RepositorioLog _repositorioLog;
        private readonly RepositorioNoConformidad _repositorioNoConformidades;
        private readonly RepositorioConfigOrganizacion _repositorioConfigOrganizacion;
        private readonly RepositorioUsuarios _repositorioUsuarios;
        public PuestosController()
        {
            _repositorioLog = new RepositorioLog();
            _repositorioConfigOrganizacion = new RepositorioConfigOrganizacion();
            _repositorioDocumentos = new RepositorioDocumentos();
            _repositorioNoConformidades = new RepositorioNoConformidad();
            _repositorioUsuarios = new RepositorioUsuarios();
        }

        public PuestosController(
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
        // GET: Puestos
        public ActionResult Index()
        {
            var puestos = _repositorioUsuarios.ObtenerTodosLosPuestos()
                .Select(p => new PuestosViewModel
                {
                    ID = p.ID,
                    PUESTO = p.PUESTO,
                    DESCRIPCION = p.DESCRIPCION,
                    FUNCIONES = p.FUNCIONES,
                    PERFIL = p.PERFIL,
                    REQUISITOS = p.REQUISITOS
                }).ToList();

            return View(puestos);
        }


        [HttpPost]
        public JsonResult GuardarPuesto(string Nombre, string Descripcion, string Funciones, string Perfil, string Requisitos)
        {
            var numeroSesion = Session["numero"]?.ToString();
            if (numeroSesion != "20044")
            {
                return Json(new { success = false, message = "No tienes permisos para realizar esta acción." });
            }
            try
            {
                var nuevoPuesto = new Puestos
                {
                    PUESTO = Nombre,
                    DESCRIPCION = Descripcion,
                    FUNCIONES = Funciones,
                    PERFIL = Perfil,
                    REQUISITOS = Requisitos
                };
                nuevoPuesto.PUESTOPADRE = "";
                nuevoPuesto.IDPUESTOPADRE = 0;

                _repositorioUsuarios.Agregar(nuevoPuesto);  

                return Json(new { success = true, message = "Puesto guardado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar el puesto: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Editar(int id)
        {
            var puesto = _repositorioUsuarios.BuscarPuesto(id);  // Método para obtener los datos del puesto por su ID

            if (puesto == null)
            {
                return HttpNotFound();

            }

            var viewModel = new PuestosViewModel
            {
                ID = puesto.ID,
                PUESTO = puesto.PUESTO,
                IDPUESTOPADRE = 0,
                PUESTOPADRE = "",
                DESCRIPCION = puesto.DESCRIPCION,
                FUNCIONES = puesto.FUNCIONES,
                PERFIL = puesto.PERFIL,
                REQUISITOS = puesto.REQUISITOS
            };

            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Editar(PuestosViewModel viewModel)
        {
            bool esCamillo = Session["numero"]?.ToString() == "20044";

            if (esCamillo)
            {
                var puesto = new Puestos
                {
                    ID = viewModel.ID,
                    PUESTOPADRE = "",
                    IDPUESTOPADRE = 0,
                    PUESTO = viewModel.PUESTO,
                    DESCRIPCION = viewModel.DESCRIPCION,
                    FUNCIONES = viewModel.FUNCIONES,
                    PERFIL = viewModel.PERFIL,
                    REQUISITOS = viewModel.REQUISITOS
                };

                if (_repositorioUsuarios.Modificar(puesto) != 0)
                {
                    TempData["SwalMessage"] = "Puesto actualizado correctamente.";
                    TempData["SwalType"] = "success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["SwalMessage"] = "No se pudo actualizar el puesto.";
                    TempData["SwalType"] = "error";
                }
            }
            else
            {
                TempData["SwalMessage"] = "No tiene permisos";
                TempData["SwalType"] = "error";
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }



        public ActionResult Filtrar(string filtro)
        {
            var puestos = _repositorioUsuarios.ObtenerTodosLosPuestos()
                .Where(p => string.IsNullOrEmpty(filtro) || p.PUESTO.ToLower().Contains(filtro.ToLower()))
                .Select(p => new PuestosViewModel
                {
                    ID = p.ID,
                    PUESTO = p.PUESTO,
                    DESCRIPCION = p.DESCRIPCION,
                    FUNCIONES = p.FUNCIONES,
                    PERFIL = p.PERFIL,
                    REQUISITOS = p.REQUISITOS
                }).ToList();

            return PartialView("_ListaPuestos", puestos);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(PuestosViewModel viewModel)
        {
            bool esCamillo = Session["numero"]?.ToString() == "20044";

            if (!esCamillo)
            {
                TempData["SwalMessage"] = "No tiene permisos para crear puestos.";
                TempData["SwalType"] = "error";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                // Volvés a la vista con errores
                return View(viewModel);
            }

            var nuevoPuesto = new Puestos
            {
                PUESTO = viewModel.PUESTO,
                DESCRIPCION = viewModel.DESCRIPCION,
                FUNCIONES = viewModel.FUNCIONES,
                PERFIL = viewModel.PERFIL,
                REQUISITOS = viewModel.REQUISITOS,
                PUESTOPADRE = "",
                IDPUESTOPADRE = 0
            };

            var resultado = _repositorioUsuarios.Agregar(nuevoPuesto);

            if (resultado != 0)
            {
                TempData["SwalMessage"] = "Puesto creado correctamente.";
                TempData["SwalType"] = "success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["SwalMessage"] = "No se pudo crear el puesto.";
                TempData["SwalType"] = "error";
                return View(viewModel);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            bool esCamillo = Session["numero"]?.ToString() == "20044";

            if (!esCamillo)
            {
                TempData["SwalMessage"] = "No tiene permisos para eliminar puestos.";
                TempData["SwalType"] = "error";
                return RedirectToAction("Index");
            }

            var resultado = _repositorioUsuarios.Eliminar(id);

            if (resultado > 0)
            {
                TempData["SwalMessage"] = "Puesto eliminado correctamente.";
                TempData["SwalType"] = "success";
            }
            else
            {
                TempData["SwalMessage"] = "No se pudo eliminar el puesto.";
                TempData["SwalType"] = "error";
            }

            return RedirectToAction("Index");
        }

    }
}