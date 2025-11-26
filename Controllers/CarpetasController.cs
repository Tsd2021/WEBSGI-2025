using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBSGI.Models;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class CarpetasController : Controller
    {
        // GET: Carpetas

        private readonly RepositorioDocumentos _repositorioDocumentos;
        private readonly RepositorioLog _repositorioLog;

        // Constructor sin parámetros requerido por MVC
        public CarpetasController()
        {
            _repositorioLog = new RepositorioLog(); // Crear instancia manualmente

            _repositorioDocumentos = new RepositorioDocumentos(); // Crear instancia manualmente
        }

        // Constructor con inyección de dependencias
        public CarpetasController(RepositorioDocumentos repositorioDocumentos, RepositorioLog repositorioLog)
        {
            _repositorioDocumentos = repositorioDocumentos;
            _repositorioLog = repositorioLog;
        }

        private List<int> GetFolderParentChain(int folderId, List<WEBSGI.Models.DocumentosCarpeta> folders)
        {
            List<int> chain = new List<int>();
            // Creamos un diccionario para búsquedas rápidas
            var folderDict = folders.ToDictionary(f => f.ID, f => f);
            int currentId = folderId;

            // Recorremos mientras exista el folder y no lleguemos a la raíz (asumamos que raíz es cuando IDCARPETA == 0)
            while (folderDict.ContainsKey(currentId))
            {
                chain.Add(currentId);
                int parentId = folderDict[currentId].IDCARPETA;
                if (parentId == 0)
                    break;
                currentId = parentId;
            }
            chain.Reverse(); // Para que la ruta vaya de la raíz a la carpeta seleccionada
            return chain;
        }

        public ActionResult Index(int? selectedId)
        {
            // Si no se selecciona una carpeta, se limpia la selección
            if (!selectedId.HasValue || selectedId.Value == 0)
            {
                Session["SelectedFolderIdCarpetas"] = null;
            }
            else
            {
                Session["SelectedFolderIdCarpetas"] = selectedId.Value;
            }

            // Obtenemos la estructura completa de carpetas
            var fullFolderTree = _repositorioDocumentos.TraerCarpetasEnteras();

            // Si hay una carpeta seleccionada, armamos la cadena de ids
            if (Session["SelectedFolderIdCarpetas"] != null)
            {
                int folderId = Convert.ToInt32(Session["SelectedFolderIdCarpetas"]);
                List<int> folderChain = GetFolderParentChain(folderId, fullFolderTree);
                ViewBag.FolderChain = folderChain;

                // Cargamos los documentos de la carpeta seleccionada
                string puesto = Session["PUESTO"] as string;
                string tipoUsuario = Session["NIVEL"] as string;


                List<Documentos> docs = _repositorioDocumentos.TraerDocumentosPorCarpeta(folderId, puesto, tipoUsuario); // aca se hace la consulta para la visibilidad segun el puesto. :D
                ViewBag.Documentos = docs;
            }
            else
            {
                ViewBag.Documentos = null;
                ViewBag.FolderChain = new List<int>(); // Para evitar problemas en la vista
            }

            return View(fullFolderTree);
        }

        [HttpPost]
        public ActionResult AgregarCarpeta(string nombre, int parentId)
        {
            try
            {
                DocumentosCarpeta carpeta = new DocumentosCarpeta();
                carpeta.CARPETA = nombre;
                carpeta.IDCARPETA = parentId;
                carpeta.TIPO = 1;

                int ret = _repositorioDocumentos.Agregar(carpeta);

                if (ret != 0)
                {
                    // Traer los datos del árbol actualizado
                    var carpetas = _repositorioDocumentos.TraerCarpetasEnteras();
                    var treeData = carpetas.Select(c => new {
                        id = c.ID.ToString(),
                        parent = (c.IDCARPETA == 0 ? "#" : c.IDCARPETA.ToString()),
                        text = c.CARPETA ?? "Sin Nombre",
                        icon = "bi bi-folder-fill"
                    }).ToList();

                    return Json(new { success = true, treeData = treeData });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo agregar la carpeta." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        [HttpPost]
        public ActionResult ActualizarRutaCarpeta(int id, string nuevoNombre)
        {
            try
            {
                DocumentosCarpeta carpeta = new DocumentosCarpeta();
                carpeta.ID = id;
                carpeta.CARPETA = nuevoNombre;

                _repositorioDocumentos.Modificar(carpeta);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CrearSubCarpeta(int parentId, string nombre)
        {
            try
            {
                DocumentosCarpeta carpeta = new DocumentosCarpeta();
                carpeta.CARPETA = nombre;
                carpeta.IDCARPETA = parentId;
                carpeta.TIPO = 2;
                if(_repositorioDocumentos.Agregar(carpeta) != 0)
                {
                    return Json(new { success = true});

                }
                else
                {
                    return Json(new { success = false, message = "No se ingresó, verifique." });

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            try
            {
                //Tipo 2 son todoas las carpetas que tienen sub carpetas, las carpetas Raiz son tipo 1... 
                if (_repositorioDocumentos.TieneSubCarpetasActivas(id, 2).ID != 0)
                {
                    return Json(new { success = false, message = "No se puede borrar una carpeta que tiene Documentos activos." });
                }
                else
                {
                    if(_repositorioDocumentos.Eliminar(id) != 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "No se pudo eliminar la carpeta." });
                    }
                }
                

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

                throw;
            }
        }

    }



}
