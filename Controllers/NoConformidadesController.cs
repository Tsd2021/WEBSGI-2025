using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using MathNet.Numerics.Distributions;
using WEBSGI.Models;
using WEBSGI.Repositorio;

namespace WEBSGI.Controllers
{
    public class NoConformidadesController : Controller
    {
        private readonly RepositorioDocumentos _repositorioDocumentos;
        private readonly RepositorioLog _repositorioLog;
        private readonly RepositorioNoConformidad _repositorioNoConformidades;

        public NoConformidadesController()
        {
            _repositorioLog = new RepositorioLog(); 

            _repositorioDocumentos = new RepositorioDocumentos();
            _repositorioNoConformidades = new RepositorioNoConformidad();
        }

        public NoConformidadesController(RepositorioDocumentos repositorioDocumentos, RepositorioLog repositorioLog, RepositorioNoConformidad repositorioNoConformidad)
        {
            _repositorioDocumentos = repositorioDocumentos;
            _repositorioLog = repositorioLog;
            _repositorioNoConformidades = repositorioNoConformidad;
        }
        // GET: NoConformidades
        public ActionResult Index()
        {

           

            List<NoConformidad> lista = _repositorioNoConformidades.TraerNoConformidades();

            ViewBag.Total = lista.Count();
            ViewBag.Abiertas = lista.Count(x => x.ESTADO.ToUpper() == "ABIERTA");
            ViewBag.Cerradas = lista.Count(x => x.ESTADO.ToUpper() == "CERRADA");

            return View(lista);
        }


        public ActionResult Buscar(DateTime fecha1, DateTime fecha2)
        {
            if(fecha1 == null|| fecha2 == null)
            {
                return View();
            }
            else
            {
               var lista = _repositorioNoConformidades.TraerNoConformidadesFiltradas(fecha1, fecha2);
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
        public ActionResult Editar(int id)
        {

            NoConformidad nc = _repositorioNoConformidades.Buscar(id);

            Log LL = new Log();
            int numero = Convert.ToInt32(Session["NUMERO"]);
            Usuarios usuario = _repositorioLog.BuscarUNO(numero);
            LL.USUARIO = usuario.Usuario;
            LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ", vío la No Conformidad N" + nc.NUMERO;
            LL.FECHA = DateTime.Now;
            LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
            _repositorioLog.Agregar(LL);

            List<Normas> normas = _repositorioNoConformidades.TraerNromas();
            if (nc == null)
            {
                return HttpNotFound();
            }
            var idPuesto = (int)Session["PUESTOID"];
            var puesto = Session["PUESTO"]?.ToString();
           
         
            var normaSeleccionada = normas.FirstOrDefault(n => n.NORMA == nc.ISO);

            ViewBag.Normas = new SelectList(normas, "Id", "Norma", normaSeleccionada?.ID);

            return View(nc);
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


                if(model.CERRADAENFECHA == "" || model.CERRADAENFECHA == null)
                {
                    model.CERRADAENFECHA = "";
                } 
                if(model.OBSERVACIONES == "" || model.OBSERVACIONES == null)
                {
                    model.OBSERVACIONES = "";
                }
                if(model.RESPONSABLE == "" || model.RESPONSABLE == null)
                {
                    model.RESPONSABLE = "";
                }
                model.TIPO = "NOCONFORMIDAD";

                int ret = _repositorioNoConformidades.Modificar(model, model.ID);
                if (ret != 0)
                {
                    TempData["Mensaje"] = "No Conformidad actualizada correctamente.";
                    TempData["TipoMensaje"] = "success";
                    ViewBag.Normas = new SelectList(_repositorioNoConformidades.TraerNromas(), "Id", "Norma", NormaId);

                    Log LL = new Log();
                    int numero = Convert.ToInt32(Session["NUMERO"]);
                    Usuarios usuario = _repositorioLog.BuscarUNO(numero);
                    LL.USUARIO = usuario.Usuario;
                    LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ", Editor la No Conformidad N" + model.NUMERO;
                    LL.FECHA = DateTime.Now;
                    LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
                    _repositorioLog.Agregar(LL);

                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["Mensaje"] = "No se pudo actualizar la No Conformidad.";
                    TempData["TipoMensaje"] = "error";
                }
            }
            return View("Index", model);
        }

        // Acción para crear una nueva No Conformidad (GET)
        public ActionResult NuevaNoConformidad()
        {

            List<Normas> normas = _repositorioNoConformidades.TraerNromas();
            ViewBag.Normas = new SelectList(normas, "Id", "Norma"); // Asegura que los valores sean correctos

            return View();
        }

        // Acción POST para crear la No Conformidad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuevaNoConformidad(NoConformidad model, int NormaId)
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


                model.CERRADAENFECHA = string.IsNullOrEmpty(model.CERRADAENFECHA) ? "" : model.CERRADAENFECHA;
                model.OBSERVACIONES = string.IsNullOrEmpty(model.OBSERVACIONES) ? "" : model.OBSERVACIONES;
                model.RESPONSABLE = string.IsNullOrEmpty(model.RESPONSABLE) ? "" : model.RESPONSABLE;

                model.TIPO = "NOCONFORMIDAD";

                int ret = _repositorioNoConformidades.Agregar(model);
                if (ret != 0)
                {
                    Log LL = new Log();
                    int numero = Convert.ToInt32(Session["NUMERO"]);
                    Usuarios usuario = _repositorioLog.BuscarUNO(numero);
                    LL.USUARIO = usuario.Usuario;
                    LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ", Agregó una nueva No Conformidad N" + model.NUMERO;
                    LL.FECHA = DateTime.Now;
                    LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
                    _repositorioLog.Agregar(LL);

                    TempData["Mensaje"] = "No Conformidad creada correctamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo crear la No Conformidad.");
                }
            }
            ViewBag.Normas = new SelectList(_repositorioNoConformidades.TraerNromas(), "Id", "Norma", NormaId);

            return View(model);
        }
    }
}