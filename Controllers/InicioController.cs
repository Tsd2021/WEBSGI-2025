using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBSGI.Models;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class InicioController : Controller
    {
        private readonly RepositorioUsuarios _repositorioUsuario;
        private readonly RepositorioDocumentos _repositorioDocumentos;
        private readonly RepositorioLog _repositorioLog;
        private readonly RepositorioNoConformidad _repositorioNoConformidades;

        public InicioController()
        {
            _repositorioUsuario = new RepositorioUsuarios(); 
            _repositorioDocumentos = new RepositorioDocumentos();
            _repositorioLog = new RepositorioLog();
            _repositorioNoConformidades = new RepositorioNoConformidad();
        }

        public InicioController(RepositorioUsuarios repositorioUsuario, RepositorioDocumentos repositorioDocumentos, 
            RepositorioLog repositorioLog, RepositorioNoConformidad repositorioNoConformidad)
        {
            _repositorioUsuario = repositorioUsuario;
            _repositorioDocumentos = repositorioDocumentos;
            _repositorioLog = repositorioLog;
            _repositorioNoConformidades = repositorioNoConformidad;
        }
        public ActionResult Index()
        {
            Usuarios usu = (Usuarios)Session["Usuario"];
            string nombre = usu.Nombre;
            ViewBag.Nombre = nombre;
            Usuarios usuario = _repositorioUsuario.BuscarUNO(usu.Numero);

            string nombreCompleto = usu.Nombre;

            string[] partesNombre = nombreCompleto.ToLower().Split(' ');
            string primerNombre = "";

            if (partesNombre.Length > 0 && !string.IsNullOrEmpty(partesNombre[0]))
            {
                // Tomar el primer nombre y capitalizar la primera letra
                primerNombre = char.ToUpper(partesNombre[0][0]) + partesNombre[0].Substring(1);
            }

            Session["NOMBREUSUARIO"] = primerNombre;

            int totalRegistros = _repositorioLog.TotalRegistros();  // Total de registros en Log
            int documentosActivos = _repositorioDocumentos.TotalDocumentosActivos();
            int totalRegistros3Dias = _repositorioLog.TotalRegistros3DIAS();

            // Paginación de los registros
            //Usuarios usuario2 = (Usuarios)Session["Usuario"];
            List<NoConformidad> lista = _repositorioNoConformidades.TraerNoConformidades();

            if (usuario.Nivel == "ADMINISTRADOR")
            {
                // Obtener la primera no conformidad abierta con fecha de cierre próxima o vencida
                NoConformidad unaNc = _repositorioNoConformidades.AbiertaEnFechaCierre(lista);

                if (unaNc != null)
                {
                    DateTime hoy = DateTime.Now.Date; // Fecha actual sin hora
                    if (unaNc.CIERRE.HasValue)
                    {
                        DateTime fechaCierre = unaNc.CIERRE.Value;
                        TimeSpan diferencia = fechaCierre - hoy; // Asegúrate de restar la fecha actual de la fecha de cierre

                        // Si la diferencia está entre 1 y 3 días, es una advertencia
                        if (diferencia.TotalDays <= 3 && diferencia.TotalDays > 0)
                        {
                            TempData["Mensaje"] = $"La no conformidad {unaNc.NUMERO} esta cercana a su fecha de cierre ({fechaCierre.ToShortDateString()}).";

                            TempData["TipoMensaje"] = "warning";
                        }
                        // Si la fecha de cierre ya pasó
                        else if (diferencia.TotalDays <= 0)
                        {
                            TempData["Mensaje"] = $"La no conformidad {unaNc.NUMERO} ya ha excedido su fecha de cierre ({fechaCierre.ToShortDateString()}).";
                            TempData["TipoMensaje"] = "warning"; // "warning" o "error", dependiendo del caso
                        }
                    }

                }
            }

            // Obtener los registros paginados desde la base de datos
            List<Dictionary<string, object>> logs = _repositorioLog.ObtenerRegistrosUltimoDia();

            // Calcular el número total de páginas

            // Pasar los resultados a la vista
            ViewBag.Registros = logs;
            ViewBag.TotalRegistros = totalRegistros;
          
         
            ViewBag.DocumentosActivos = documentosActivos;
            ViewBag.TotalRegistros3Dias = totalRegistros3Dias;

            return View();
        }

        public ActionResult GetLogs()
        {
            try
            {
                var logs = _repositorioLog.ObtenerRegistrosUltimoDia(); 

         
                return PartialView("_LogsModal", logs);  
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetLogs: " + ex.Message);
                return new HttpStatusCodeResult(500, "Error en el servidor");
            }
        }








    }
}