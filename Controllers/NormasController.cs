using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBSGI.Models;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class NormasController : Controller
    {

 
        private readonly RepositorioConfigOrganizacion _repositorioConfigOrganizacion;
        public NormasController()
        {
       
            _repositorioConfigOrganizacion = new RepositorioConfigOrganizacion();
           
        }

        public NormasController(
          
            RepositorioConfigOrganizacion repositorioConfigOrganizacion)
        {
            
            _repositorioConfigOrganizacion = repositorioConfigOrganizacion;
        }

        // GET: Normas
        public ActionResult Index()
        {
            List<Normas> lista = _repositorioConfigOrganizacion.TraerNormas();
            return View(lista);
        }

        // POST: Normas/Guardar

        [HttpPost]
        public JsonResult Guardar(string descripcion, string norma)
        {
            Normas normaF = new Normas();
            normaF.DESCRIPCION = descripcion;
            normaF.NORMA = norma;
            int resultado = _repositorioConfigOrganizacion.Agregar(normaF);
            return Json(new { success = resultado > 0, message = resultado > 0 ? "Norma agregada." : "Error al agregar la norma." });
        }

        // POST: Actualizar norma
        [HttpPost]
        public JsonResult Actualizar(int id, string descripcion, string normaTxt)
        {
            Normas norma = new Normas();
            norma.ID = id;
            norma.DESCRIPCION = descripcion;
            norma.NORMA = normaTxt;
            int resultado = _repositorioConfigOrganizacion.Modificar(norma);
            return Json(new { success = resultado > 0, message = resultado > 0 ? "Norma actualizada." : "Error al actualizar la norma." });
        }

        // POST: Eliminar norma
        [HttpPost]
        public JsonResult Eliminar(int id)
        {
            int resultado = _repositorioConfigOrganizacion.Eliminar(id);
            return Json(new { success = resultado > 0, message = resultado > 0 ? "Norma eliminada." : "Error al eliminar la norma." });
        }

    }
}