using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBSGI.Models;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class OportunidadMejoraController : Controller
    {
        private readonly RepositorioDocumentos _repositorioDocumentos;
        private readonly RepositorioLog _repositorioLog;
        private readonly RepositorioNoConformidad _repositorioNoConformidades;

        public OportunidadMejoraController()
        {
            _repositorioLog = new RepositorioLog();

            _repositorioDocumentos = new RepositorioDocumentos();
            _repositorioNoConformidades = new RepositorioNoConformidad();
        }

        public OportunidadMejoraController(RepositorioDocumentos repositorioDocumentos, RepositorioLog repositorioLog, RepositorioNoConformidad repositorioNoConformidad)
        {
            _repositorioDocumentos = repositorioDocumentos;
            _repositorioLog = repositorioLog;
            _repositorioNoConformidades = repositorioNoConformidad;
        }
        // GET: OportunidadMejora
        public ActionResult Index()
        {
            List<NoConformidad> lista = _repositorioNoConformidades.TraerOportunidadMejoras();

            ViewBag.Total = lista.Count();
            ViewBag.Abiertas = lista.Count(x => x.ESTADO.ToUpper() == "ABIERTA");
            ViewBag.Cerradas = lista.Count(x => x.ESTADO.ToUpper() == "CERRADA");

            var ultimas5 = lista
                .OrderByDescending(x => x.FECHA)
                .Take(5)
                .ToList();

            return View(ultimas5);
        }


        public ActionResult Buscar(DateTime fecha1, DateTime fecha2)
        {
            if (fecha1 == null || fecha2 == null)
            {
                return View();
            }
            else
            {
                var lista = _repositorioNoConformidades.TraerOportunidadesMejoraFiltradas(fecha1, fecha2);
                ViewBag.Total = lista.Count();
                ViewBag.Abiertas = lista.Count(x => x.ESTADO.ToUpper() == "ABIERTA");
                ViewBag.Cerradas = lista.Count(x => x.ESTADO.ToUpper() == "CERRADA");
                return View("Index",lista);
            }
        
        }
        public ActionResult ObtenerNoConformidad(int id)
        {
            NoConformidad nc = _repositorioNoConformidades.Buscar(id);
              return View(nc);
        }

        public ActionResult Editar (int id)
        {

            NoConformidad nc = _repositorioNoConformidades.Buscar(id);
            List<Normas> normas = _repositorioNoConformidades.TraerNromas();

            Log LL = new Log();
            int numero = Convert.ToInt32(Session["NUMERO"]);
            Usuarios usuario = _repositorioLog.BuscarUNO(numero);
            LL.USUARIO = usuario.Usuario;
            LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ", Entro a la Oportunidad de Mejora N" + nc.NUMERO;
            LL.FECHA = DateTime.Now;
            LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
            _repositorioLog.Agregar(LL);


            if (nc == null)
            {
                return HttpNotFound();
            }
            var normaSeleccionada = normas.FirstOrDefault(n => n.NORMA == nc.ISO);

            ViewBag.Normas = new SelectList(normas, "Id", "Norma", normaSeleccionada?.ID);

            return View(nc);

        }
        private DateTime ConvertirFecha(DateTime? fecha)
        {
            if (fecha.HasValue)
            {
                return fecha.Value.Date; // Devuelve la fecha con la hora a 00:00:00
            }
            else
            {
                return DateTime.MinValue; // O cualquier otro valor predeterminado que desees
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarNoConformidad(NoConformidad model, int NormaId)
        {
            if (ModelState.IsValid)
            {
                Normas normaSeleccionada = _repositorioNoConformidades.TraerNromas()
                                   .FirstOrDefault(n => n.ID == NormaId);

                if (normaSeleccionada != null)
                {
                    model.ISO = normaSeleccionada.NORMA; // Asignamos el número ISO
                }

                model.FECHA = ConvertirFecha(model.FECHA);
                model.PLAZOCIERRE = ConvertirFecha(model.PLAZOCIERRE);
                model.ACCIONPLAZOCIERRE = ConvertirFecha(model.PLAZOCIERRE);

                if (model.CERRADAENFECHA == "" || model.CERRADAENFECHA == null)
                {
                    model.CERRADAENFECHA = "";
                }
                if (model.OBSERVACIONES == "" || model.OBSERVACIONES == null)
                {
                    model.OBSERVACIONES = "";
                }
                if (model.RESPONSABLE == "" || model.RESPONSABLE == null)
                {
                    model.RESPONSABLE = "";
                }
                if(model.CAUSA == "" || model.CAUSA == null)
                {
                    model.CAUSA = "";
                }
                if(model.ACCIONCORRECTIVA == "" || model.ACCIONCORRECTIVA == null)
                {
                    model.ACCIONCORRECTIVA = "";
                }
                if (model.NC == "" || model.NC == null)
                {
                    model.NC = "";
                }

                model.TIPO = "OPCIONMEJORA";

                model.CIERRE = ConvertirFecha(model.PLAZOCIERRE);
                int ret = _repositorioNoConformidades.Modificar(model, model.ID);
                if (ret != 0)
                {

                    Log LL = new Log();
                    int numero = Convert.ToInt32(Session["NUMERO"]);
                    Usuarios usuario = _repositorioLog.BuscarUNO(numero);
                    LL.USUARIO = usuario.Usuario;
                    LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ", Editor la Oportunidad de Mejora N" + model.NUMERO;
                    LL.FECHA = DateTime.Now;
                    LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
                    _repositorioLog.Agregar(LL);


                    TempData["Mensaje"] = "Oportunidad Mejora actualizada correctamente.";
                    TempData["TipoMensaje"] = "success";
                    ViewBag.Normas = new SelectList(_repositorioNoConformidades.TraerNromas(), "Id", "Norma", NormaId);

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensaje"] = "No se pudo actualizar la Oportunidad Mejora.";
                    TempData["TipoMensaje"] = "error";
                }
            }
            return View("Editar", model);
        }
        // Acción para crear una nueva No Conformidad (GET)
        public ActionResult NuevaOportunidadMejora()
        {
            List<Normas> normas = _repositorioNoConformidades.TraerNromas();
            ViewBag.Normas = new SelectList(normas, "Id", "Norma"); // Asegura que los valores sean correctos
            return View();
        }

        // Acción POST para crear la No Conformidad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuevaOportunidadMejora(NoConformidad model,int NormaId)
        {
            if (ModelState.IsValid)
            {
                Normas normaSeleccionada = _repositorioNoConformidades.TraerNromas()
                                     .FirstOrDefault(n => n.ID == NormaId);

                if (normaSeleccionada != null)
                {
                    model.ISO = normaSeleccionada.NORMA; // Asignamos el número ISO
                }

                model.FECHA = ConvertirFecha(model.FECHA);
                model.PLAZOCIERRE = ConvertirFecha(model.PLAZOCIERRE);
                model.ACCIONPLAZOCIERRE = ConvertirFecha(model.PLAZOCIERRE);

                model.ACCIONCORRECTIVA = string.IsNullOrEmpty(model.ACCIONCORRECTIVA) ? "" : model.ACCIONCORRECTIVA;
                model.CERRADAENFECHA = string.IsNullOrEmpty(model.CERRADAENFECHA) ? "" : model.CERRADAENFECHA;
                model.OBSERVACIONES = string.IsNullOrEmpty(model.OBSERVACIONES) ? "" : model.OBSERVACIONES;
                model.RESPONSABLE = string.IsNullOrEmpty(model.RESPONSABLE) ? "" : model.RESPONSABLE;
                model.CAUSA = string.IsNullOrEmpty(model.CAUSA) ? "" : model.CAUSA;
                model.TIPO = "OPCIONMEJORA";


                int ret = _repositorioNoConformidades.Agregar(model);
                if (ret != 0)
                {
                    Log LL = new Log();
                    int numero = Convert.ToInt32(Session["NUMERO"]);
                    Usuarios usuario = _repositorioLog.BuscarUNO(numero);
                    LL.USUARIO = usuario.Usuario;
                    LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ", Agregó la Oportunidad de Mejora N" + model.NUMERO;
                    LL.FECHA = DateTime.Now;
                    LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
                    _repositorioLog.Agregar(LL);


                    TempData["Mensaje"] = "Oportunidad de Mejora creada correctamente.";
                    TempData["MensajeTipo"] = "success";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo crear la Oportunidad de Mejora.");
                }
            }
            ViewBag.Normas = new SelectList(_repositorioNoConformidades.TraerNromas(), "Id", "Norma", NormaId);


            return View(model);
        }


    }
}