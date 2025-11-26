using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBSGI.Models.ViewModel;
using WEBSGI.Models;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class TiposDocumentosController : Controller
    {

        private readonly RepositorioDocumentos _repositorioDocumentos;
        private readonly RepositorioLog _repositorioLog;

        // Constructor sin parámetros requerido por MVC
        public TiposDocumentosController()
        {
            _repositorioLog = new RepositorioLog(); // Crear instancia manualmente

            _repositorioDocumentos = new RepositorioDocumentos(); // Crear instancia manualmente
        }

        // Constructor con inyección de dependencias
        public TiposDocumentosController(RepositorioDocumentos repositorioDocumentos, RepositorioLog repositorioLog)
        {
            _repositorioDocumentos = repositorioDocumentos;
            _repositorioLog = repositorioLog;
        }
        // GET: TiposDocumentos
      

        public ActionResult Index()
        {
            List<DocumentosTipo> tipos = _repositorioDocumentos.TraerDocumentoTipo();
            return View(tipos);
        }

        [HttpPost]
        public ActionResult AddTipo(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
            {
                return Json(new { success = false, message = "El tipo es requerido." });
            }

            try
            {
               
                string tipoUpper = tipo.ToUpper();

                DocumentosTipo documentosTipo = new DocumentosTipo();
                documentosTipo.TIPO = tipoUpper;
                _repositorioDocumentos.AgregarTipo(documentosTipo);


                return Json(new { success = true, message = "Tipo agregado correctamente." });
            }
            catch (Exception ex)
            {
                // Loguear error, etc.
                return Json(new { success = false, message = "Error al agregar: " + ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UpdateTipo(int id, string tipo)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(tipo))
            {
                return Json(new { success = false, message = "Datos inválidos." });
            }

            try
            {
                // Convertir a mayúsculas
                string tipoUpper = tipo.ToUpper();

                DocumentosTipo documentosTipo = new DocumentosTipo();
                documentosTipo.ID = id;
                documentosTipo.TIPO = tipoUpper;

                _repositorioDocumentos.ModificarTipo(documentosTipo);

                return Json(new { success = true, message = "Tipo actualizado correctamente." });
            }
            catch (Exception ex)
            {
                // Loguear error, etc.
                return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
            }
        }


        [HttpPost]
        public ActionResult DeleteTipo(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Id inválido." });
            }

            try
            {
                
                _repositorioDocumentos.EliminarTipo(id);



                return Json(new { success = true, message = "Tipo eliminado correctamente." });
            }
            catch (Exception ex)
            {
                // Loguear error, etc.
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }

    }
}