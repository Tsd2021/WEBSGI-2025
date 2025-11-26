using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models.ViewModel
{
    public class NoConformidadViewModel
    {

        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string Origen { get; set; }
        public IEnumerable<string> ListaOrigen { get; set; }  // Por ejemplo, para el dropdown
        public int Total { get; set; }

        // Propiedades para la edición/alta (los campos del panel)
        public int? ID { get; set; }
        public string Numero { get; set; }
        public DateTime? Fecha { get; set; }
        public string ISO { get; set; }
        public string Tipo { get; set; }
        public string RefiereASector { get; set; }
        public string Procedimiento { get; set; }
        public string OrigenDetalle { get; set; }
        public string Descripcion { get; set; }
        public string Causa { get; set; }
        public string AccionInmediata { get; set; }
        public string AccionCorrectiva { get; set; }
        public string Responsable { get; set; }
        public DateTime? PlazoCierre { get; set; }
        public string CerradaEnFecha { get; set; }
        public DateTime? Cierre { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }

        // Lista de no conformidades para la tabla
        public IEnumerable<NoConformidad> NoConformidades { get; set; }
        public int Acceso { get; set; }  // 4 = solo ver, 5 = puede modificar

    }
}