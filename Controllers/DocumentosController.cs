
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBSGI.Models;
using WEBSGI.Models.ViewModel;
using WEBSGI.Repositorio;
using Spire.Doc;
using Spire.Xls;
using FileFormat = Spire.Doc.FileFormat;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;





namespace WEBSGI.Controllers
{
    public class DocumentosController : Controller
    {
        private readonly RepositorioDocumentos _repositorioDocumentos;
        private readonly RepositorioLog _repositorioLog;
        public DocumentosController()
        {
            _repositorioLog = new RepositorioLog(); // Crear instancia manualmente

            _repositorioDocumentos = new RepositorioDocumentos(); // Crear instancia manualmente
        }

        public DocumentosController(RepositorioDocumentos repositorioDocumentos, RepositorioLog repositorioLog)
        {
            _repositorioDocumentos = repositorioDocumentos;
            _repositorioLog = repositorioLog;
        }

        private List<string> ObtenerRutaCarpeta(string rutaCarpeta)
        {
            if (string.IsNullOrWhiteSpace(rutaCarpeta))
                return new List<string>();

            // Separa por '/' y elimina elementos vacíos
            return rutaCarpeta.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public ActionResult Buscar(string titulo, string estado)
        {
            Log LL = new Log();
            int numero = Convert.ToInt32(Session["NUMERO"]);
            Usuarios usuario = _repositorioLog.BuscarUNO(numero);
            LL.USUARIO = usuario.Usuario;
            LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ",Buscó el archivo (" + titulo + ")";
            LL.FECHA = DateTime.Now;
            LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
            _repositorioLog.Agregar(LL);

            // Normalizamos el texto de búsqueda antes de pasarlo a la base de datos
            string tituloNormalizado = NormalizarTexto(titulo);

            // Llamamos al repositorio pasándole el texto normalizado
            var documentos = _repositorioDocumentos.TraerDocumentos(tituloNormalizado, estado);

            var carpetas = _repositorioDocumentos.TraerCarpetasEnteras(); // Obtenemos todas las carpetas

            string rutaCarpeta = documentos.Any() ? documentos.First().CARPETA : "";
            ViewBag.Documentos = documentos;
            ViewBag.Carpetas = carpetas;
            ViewBag.CarpetaRuta = ObtenerRutaCarpeta(rutaCarpeta);

            return View("Index", carpetas);
        }

        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;

            // Convertir a minúsculas
            texto = texto.ToLower();

            // Eliminar los tildes
            texto = texto.Normalize(NormalizationForm.FormD);
            texto = new string(texto.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
            texto = texto.Normalize(NormalizationForm.FormC);

            return texto;
        }

        public ActionResult Index(int? selectedId)
        {
            // Obtenemos la estructura completa de carpetas
            var fullFolderTree = _repositorioDocumentos.TraerCarpetasEnteras();

            string puesto = Session["PUESTO"] as string;
            string tipoUsuario = Session["NIVEL"] as string;

            // Si hay una carpeta seleccionada, procesamos normalmente
            if (selectedId.HasValue && selectedId.Value != 0)
            {
                Session["SelectedFolderId"] = selectedId.Value;
                int folderId = selectedId.Value;
                List<int> folderChain = GetFolderParentChain(folderId, fullFolderTree);
                ViewBag.FolderChain = folderChain;

                // Cargamos los documentos de la carpeta seleccionada
                List<Documentos> docs = _repositorioDocumentos.TraerDocumentosPorCarpeta(folderId, puesto, tipoUsuario);
                ViewBag.Documentos = docs;
            }
            else
            {
                Session["SelectedFolderId"] = null;

                // NUEVA LÓGICA: Si no hay carpeta seleccionada, verificamos los documentos del usuario
                // Traer todos los documentos asociados al puesto del usuario
                List<Documentos> documentosUsuario = _repositorioDocumentos.TraerDocumentosPorPuesto(puesto, tipoUsuario);

                // Si son menos de 20, los mostramos directamente
                if (documentosUsuario != null && documentosUsuario.Count < 30)
                {
                    ViewBag.Documentos = documentosUsuario;
                    ViewBag.MensajeArchivos = $"Mostrando {documentosUsuario.Count} archivos asociados a su puesto.";
                }
                else
                {
                    ViewBag.Documentos = null;
                }

                ViewBag.FolderChain = new List<int>(); 
            }

            return View(fullFolderTree);
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
        private List<string> GetFolderRoute(int folderId, List<WEBSGI.Models.DocumentosCarpeta> folders)
        {
            var route = new List<string>();

            // Diccionario para búsqueda rápida
            var folderDict = folders.ToDictionary(f => f.ID, f => f);

            // Si no existe el folderId en el diccionario, retornamos vacío
            if (!folderDict.ContainsKey(folderId))
            {
                return route; // O podrías retornar una lista con "Carpeta no encontrada"
            }

            int currentId = folderId;
            var visited = new HashSet<int>(); // Para evitar ciclos

            while (folderDict.ContainsKey(currentId) && !visited.Contains(currentId))
            {
                visited.Add(currentId);

                var folder = folderDict[currentId];
                // Agrega el nombre de la carpeta (o "Raíz" si quieres forzarlo cuando IDCARPETA=0)
                route.Add(folder.CARPETA ?? "Sin nombre");

                // Si llegamos a la raíz (IDCARPETA == 0), rompemos
                if (folder.IDCARPETA == 0)
                    break;

                currentId = folder.IDCARPETA;
            }

            // Invierte la lista para que vaya de la raíz a la subcarpeta
            route.Reverse();
            return route;
        }
        [HttpGet]
        public ActionResult GetFolderRoute(int folderId)
        {
            Session["SelectedFolderId"] = folderId;

            var fullFolders = _repositorioDocumentos.TraerCarpetasEnteras();
            var routeNames = GetFolderRoute(folderId, fullFolders); // Tu método para construir la ruta
            string rutaCompleta = string.Join(" / ", routeNames);
            return Json(new { folderRoute = rutaCompleta }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NuevoDocumentoPartial()
        {
            try
            {

                int selectedFolderId = Session["SelectedFolderId"] != null ? Convert.ToInt32(Session["SelectedFolderId"]) : 0;


                var fullFolderTree = _repositorioDocumentos.TraerCarpetasEnteras();
                string folderRoute = "";

                if (selectedFolderId != 0)
                {
                    List<string> routeNames = GetFolderRoute(selectedFolderId, fullFolderTree);
                    folderRoute = string.Join(" / ", routeNames);
                }

               
                ViewBag.FolderRoute = folderRoute;
                Session["CarpetaRuta"] = folderRoute;
                var viewModel = new DocumentoViewModel
                {
                    Documento = new Documentos { IDCARPETA = selectedFolderId },
                    Carpetas = _repositorioDocumentos.TraerCarpetas().Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CARPETA }),
                    Tipos = _repositorioDocumentos.TraerDocumentoTipo().Select(t => new SelectListItem { Value = t.ID.ToString(), Text = t.TIPO }),
                    Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "ALTA" },
                new SelectListItem { Value = "2", Text = "BAJA" }
            },
                    Puestos = _repositorioDocumentos.TraerPuestos().Select(p => new SelectListItem { Value = p.ID.ToString(), Text = p.PUESTO })
                };

                return PartialView("_NuevoDocumento", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en NuevoDocumentoPartial: " + ex.Message);
                return new HttpStatusCodeResult(500, "Error interno en el servidor.");
            }
        }

        [HttpPost]
        public ActionResult NuevoDocumentoPartial(DocumentoViewModel model, HttpPostedFileBase fileUpload)
        {
            try
            {
                if (fileUpload != null)
                {
                    Log LL = new Log();
                    int numero = Convert.ToInt32(Session["NUMERO"]);
                    Usuarios usuario = _repositorioLog.BuscarUNO(numero);
                    LL.USUARIO = usuario.Usuario;
                   
                    LL.ARCHIVO_MOVIMIENTO = "Crea Documento " + fileUpload.FileName;
                    LL.FECHA = DateTime.Now;
                    LL.IPPRIVADA = Session["IPPRIVADA"].ToString();
                    LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
                    _repositorioLog.Agregar(LL);

                    int idCarpeta = Convert.ToInt32(Session["SelectedFolderId"]);

                    Documentos carpetaRuta = _repositorioDocumentos.TraerCarpetaRutaPorIdCarpeta(idCarpeta);
                    DocumentosTipo tipo = _repositorioDocumentos.BuscarTipo(model.SelectedTipo);
                    string estado = "";
                    if (model.SelectedEstado == "1")
                    {
                        estado = "ALTA";
                    }
                    else
                    {
                        estado = "BAJA";
                    }
                    var nuevoDocumento = new Documentos
                    {
                        REVISION = model.Documento.REVISION,
                        TITULO = model.Documento.TITULO,

                        IDCARPETA = idCarpeta,
                        CARPETA = carpetaRuta.CARPETA,
                        IDESTADO = int.Parse(model.SelectedEstado),
                     
                        ESTADO = estado,
                        IDTIPO = model.SelectedTipo,
                        TIPO = tipo.TIPO,
                        OBSERVACIONES = model.Documento.OBSERVACIONES
                    };

                    if (_repositorioDocumentos.Agregar(nuevoDocumento, fileUpload) != 0)
                    {
                        int ID = _repositorioDocumentos.buscarULTIMO().ID;

                        _repositorioDocumentos.EliminarVisibilidad(ID);

                        if (model.PuestosSeleccionados != null && model.PuestosSeleccionados.Any())
                        {
                            foreach (var puestoId in model.PuestosSeleccionados)
                            {
                                var visibilidad = new DocumentosVisibilidad
                                {
                                    IDDOCUMENTO = ID,
                                    IDPUESTO = Convert.ToInt32(puestoId),
                                    PUESTO = ""  // O elimina esta línea si no es requerida
                                };
                                _repositorioDocumentos.Agregar(visibilidad);
                            }
                        }


                    }
                    else
                    {
                        // VER COMO MANEJAR MSG ERRORES
                        TempData["Error"] = "NO SE INGRESO, VERIFIQUE !!";
                        return RedirectToAction("Index");
                    }
                    return Json(new { success = true, message = "Ingresado correctamente" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Seleccione un archivo" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false, message = "Error, verifique" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ResetNavigation()
        {
            Session["NIVELCARPETACONTAR"] = 0;
            Session["IDBUSCARCARPETA"] = null;
            Session["TEXTO1"] = "";
            Session["TEXTO2"] = "";
            Session["TEXTO3"] = "";
            Session["TEXTO4"] = "";
            Session["TEXTO5"] = "";
            return RedirectToAction("Index");
        }

        public ActionResult DocumentoCRUD(int id)
        {
              

            // Busca el documento a editar
            Documentos documento = _repositorioDocumentos.BuscarUno(id);




            if (documento == null)
            {
                return RedirectToAction("Index");
            }

            // Prepara el combo de carpetas
            var carpetasSelectList = _repositorioDocumentos.TraerCarpetas()
                                .Select(c => new SelectListItem
                                {
                                    Text = c.CARPETA,
                                    Value = c.ID.ToString(),
                                    Selected = (c.ID == documento.IDCARPETA)
                                });

            // Obtén la estructura completa de carpetas para el tree view
            var fullFolderTree = _repositorioDocumentos.TraerCarpetasEnteras();

            int selectedFolderId = Session["SelectedFolderId"] != null ? Convert.ToInt32(Session["SelectedFolderId"]) : 0;



            string folderRoute = "Raíz";

            if (selectedFolderId != 0)
            {
                List<string> routeNames = GetFolderRoute(selectedFolderId, fullFolderTree);
                folderRoute = string.Join(" / ", routeNames);
            }

            Console.WriteLine("Ruta reconstruida: " + folderRoute);
            ViewBag.FolderRoute = folderRoute;

            var tipos = _repositorioDocumentos.TraerDocumentoTipo()
                                .Select(t => new SelectListItem
                                {
                                    Text = t.TIPO,
                                    Value = t.ID.ToString(),
                                    Selected = (t.ID == documento.IDTIPO)
                                });

            var estados = new List<SelectListItem>
    {
        new SelectListItem { Text = "ALTA", Value = "ALTA", Selected = documento.ESTADO.ToUpper() == "ALTA" },
        new SelectListItem { Text = "BAJA", Value = "BAJA", Selected = documento.ESTADO.ToUpper() == "BAJA" }
    };

            var visibilidades = _repositorioDocumentos.BuscarPorIdDocumento(documento.ID);
            var listaPuestos = _repositorioDocumentos.TraerPuestos();
            var puestos = listaPuestos.Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.PUESTO,
                Selected = visibilidades.Any(v => v.IDPUESTO == p.ID)
            }).ToList();




            // Armar el view model y asignar FolderTree
            DocumentoViewModel model = new DocumentoViewModel
            {
                Documento = documento,
                Carpetas = carpetasSelectList,
                FolderTree = fullFolderTree,  // Asignación del árbol completo de carpetas
                SelectedTipo = documento.IDTIPO,
                Tipos = tipos,
                SelectedEstado = documento.ESTADO,
                Estados = estados,
                Puestos = puestos,
                Archivo = documento.ARCHIVO,

            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditDocumentPartial(DocumentoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos" });
            }

            var documento = _repositorioDocumentos.BuscarUno(model.Documento.ID);
            if (documento == null)
            {
                return Json(new { success = false, message = "Documento no encontrado" });
            }

            documento.IDCARPETA = model.SelectedFolderId;
            var carpeta = _repositorioDocumentos.TraerCarpetaRutaPorIdCarpeta(model.SelectedFolderId);
            documento.CARPETA = carpeta?.CARPETA ?? "Sin nombre";

            documento.REVISION = model.Documento.REVISION;
            documento.IDTIPO = model.SelectedTipo;
            documento.TIPO = _repositorioDocumentos.BuscarTipo(model.SelectedTipo).TIPO;
            documento.TITULO = model.Documento.TITULO;
            documento.ESTADO = model.SelectedEstado;
            documento.OBSERVACIONES = model.Documento.OBSERVACIONES;

            int ID = documento.ID;

            if (_repositorioDocumentos.Modificar(documento, ID) != 0)
            {
                // ✅ Aquí procesamos el archivo subido
                var file = Request.Files["fileUpload"];
                if (file != null && file.ContentLength > 0)
                {
                    _repositorioDocumentos.AGREGARIMAGEN(ID, file);
                }

                return Json(new { success = true, message = "Documento actualizado correctamente." });
            }

            return Json(new { success = false, message = "Error al guardar los cambios." });
        }



        public ActionResult ViewDocument(int id, bool convert = false)
        {
            try
            {
                Console.WriteLine($"📥 Solicitando documento ID: {id}");

                // 1. Obtener conexión y leer el documento
                using (SqlConnection connection = BD.obtenerConexion())
                using (SqlCommand command = new SqlCommand("SELECT ARCHIVO, NOMBREARCHIVO FROM CDOCUMENTOS WHERE ID = @ID", connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        if (!reader.Read()) return HttpNotFound("Documento no encontrado");
                        if (reader.IsDBNull(0)) return HttpNotFound("Archivo no existe");

                        // 2. Leer archivo desde la BD
                        byte[] fileBytes;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[64 * 1024];
                            long offset = 0, bytesRead;
                            do
                            {
                                bytesRead = reader.GetBytes(0, offset, buffer, 0, buffer.Length);
                                ms.Write(buffer, 0, (int)bytesRead);
                                offset += bytesRead;
                            } while (bytesRead > 0);
                            fileBytes = ms.ToArray();
                        }

                        string fileName = reader.GetString(1);
                        string fileExtension = Path.GetExtension(fileName)?.TrimStart('.').ToLower() ?? "";
                        Console.WriteLine($"📄 Archivo obtenido: {fileName} ({fileBytes.Length} bytes)");

                        // 3. Conversión si es necesario
                        if (convert && (fileExtension == "doc" || fileExtension == "docx" || fileExtension == "odt" || fileExtension == "xls" || fileExtension == "xlsx" || fileExtension == "ods"))
                        {
                            Console.WriteLine($"🔄 Iniciando conversión de {fileExtension} a PDF...");
                            fileBytes = ConvertToPdf(fileBytes, fileExtension);
                            Console.WriteLine("✅ Conversión exitosa.");
                            fileName = Path.GetFileNameWithoutExtension(fileName) + ".pdf";
                        }

                        Log LL = new Log();
                        Usuarios usuario = (Usuarios)Session["Usuario"];
                        LL.USUARIO = usuario.Usuario;
                        LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ", Vio el archivo : " + fileName;
                        LL.FECHA = DateTime.Now;
                        LL.IPPRIVADA = Session["IPPRIVADA"].ToString();
                        LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
                        _repositorioLog.Agregar(LL);


                        // 4. Retornar el archivo
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        return File(fileBytes, "application/pdf", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Error crítico: {ex.Message}");
                return new HttpStatusCodeResult(500, "Error interno");
            }
        }

        private byte[] ConvertToPdf(byte[] fileBytes, string fileExtension)
        {
            try
            {
                Console.WriteLine($"📄 Cargando archivo ({fileExtension}) para conversión...");
                using (MemoryStream inputStream = new MemoryStream(fileBytes))
                using (MemoryStream outputStream = new MemoryStream())
                {
                    if (fileExtension == "doc" || fileExtension == "docx" || fileExtension == "odt")
                    {
                        // Conversión de Word / ODT
                        Document doc = new Document();
                        doc.LoadFromStream(inputStream, FileFormat.Auto);
                        doc.SaveToStream(outputStream, FileFormat.PDF);
                    }
                    else if (fileExtension == "xls" || fileExtension == "xlsx" || fileExtension == "ods")
                    {
                        // Conversión de Excel / ODS
                        Workbook workbook = new Workbook();
                        workbook.LoadFromStream(inputStream);
                        workbook.SaveToStream(outputStream, Spire.Xls.FileFormat.PDF);
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Tipo de archivo no soportado.");
                        return fileBytes;
                    }

                    Console.WriteLine("✅ PDF generado correctamente.");
                    return outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en conversión a PDF: {ex.Message}");
                return fileBytes; // Devuelve el archivo original si la conversión falla
            }
        }




        [HttpPost]
        public ActionResult EliminarDocumentoElegido(int id)
        {
            string nombre = _repositorioDocumentos.TraerDocumentoEstadoPorId(id).NOMBREARCHIVO;
            if (_repositorioDocumentos.EliminarDocumento(id) != 0)
            {

                Log LL = new Log();
                Usuarios usuario = (Usuarios)Session["Usuario"];
                LL.USUARIO = usuario.Usuario;
              
                LL.ARCHIVO_MOVIMIENTO = "Elimina Documento " + nombre;
                LL.FECHA = DateTime.Now;
                LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
                _repositorioLog.Agregar(LL);
                _repositorioDocumentos.EliminarVisibilidad(id);
              
            }
            bool exito = true; // resultado de la eliminación
            string mensaje = exito ? "Eliminado correctamente" : "Error al eliminar";
            return Json(new { success = exito, message = mensaje });
        }


        [HttpGet]
        public ActionResult DescargarDocumento(int id)
        {
            try
            {
                // 1️⃣ Validar sesión y rol
                if (Session["ROL"] == null)
                    return new HttpUnauthorizedResult("Debe iniciar sesión.");
                if (Session["ROL"].ToString().ToUpper() != "ADMINISTRADOR")
                    return new HttpUnauthorizedResult("No tiene permisos para descargar este archivo.");

                // 2️⃣ Leer el archivo desde la base de datos
                using (SqlConnection connection = BD.obtenerConexion())
                using (SqlCommand command = new SqlCommand("SELECT ARCHIVO, NOMBREARCHIVO FROM CDOCUMENTOS WHERE ID = @ID", connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        if (!reader.Read()) return HttpNotFound("Documento no encontrado");
                        if (reader.IsDBNull(0)) return HttpNotFound("Archivo no existe en la base de datos");

                        // 3️⃣ Extraer bytes
                        byte[] fileBytes;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[64 * 1024];
                            long offset = 0, bytesRead;
                            do
                            {
                                bytesRead = reader.GetBytes(0, offset, buffer, 0, buffer.Length);
                                ms.Write(buffer, 0, (int)bytesRead);
                                offset += bytesRead;
                            } while (bytesRead > 0);
                            fileBytes = ms.ToArray();
                        }

                        string fileName = reader.GetString(1);
                        string contentType = MimeMapping.GetMimeMapping(fileName);

                        // 4️⃣ Log de descarga
                        var log = new Log
                        {
                            USUARIO = Session["USUARIO"]?.ToString(),
                            ARCHIVO_MOVIMIENTO = $"Descargó el documento: {fileName}",
                            IPPUBLICA = Session["IPPUBLICA"]?.ToString(),
                            IPPRIVADA = Session["IPPRIVADA"]?.ToString(),
                            FECHA = DateTime.Now
                        };
                        _repositorioLog.Agregar(log);

                        // 5️⃣ Devolver el archivo como descarga
                        return File(fileBytes, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Error al descargar documento ID {id}: {ex.Message}");
                return new HttpStatusCodeResult(500, "Error interno al descargar el archivo.");
            }
        }







    }
}



