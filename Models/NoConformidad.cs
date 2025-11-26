using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class NoConformidad
    {
        public int ID { get; set; }
        public string NUMERO { get; set; }
        public DateTime? FECHA { get; set; }
        public string ISO { get; set; }
        public string NC { get; set; }
        public string REFIEREASECTOR { get; set; }
        public string PROCEDIMIENTO { get; set; }
        public string ORIGEN { get; set; }
        public string DESCRIPCION { get; set; }
        public string CAUSA { get; set; }
        public string CAUSAACCIONINMEDIATA { get; set; }
        public string RESPONSABLE { get; set; }
        public DateTime? PLAZOCIERRE { get; set; }
        public string CERRADAENFECHA { get; set; }
        public string ACCIONCORRECTIVA { get; set; }
        public string ACCIONRESPONSABLE { get; set; }
        public DateTime? ACCIONPLAZOCIERRE { get; set; }
        public string ACCIONCERRADAENFECHA { get; set; }
        public string ESTADO { get; set; }
        public DateTime? CIERRE { get; set; }
        public string OBSERVACIONES { get; set; }
        public String TIPO { get; set; }

        public NoConformidad()
        { }

        public NoConformidad(int id, string numero, DateTime fecha, string iso, string nc, string refiere, string precedimiento, string origen, string descripcion, string causa, string causaaccioninmediata, string responsable, DateTime plazocierre, string cerradaenfecha, string accioncorrectiva, string accionresponsable, DateTime accionplazocierre, string accioncerradaenfecha, string estado, DateTime cierre, string OBS)
        {
            ID = id;
            NUMERO = numero;
            FECHA = fecha;
            ISO = iso;
            NC = nc;
            REFIEREASECTOR = refiere;
            PROCEDIMIENTO = precedimiento;
            ORIGEN = origen;
            DESCRIPCION = descripcion;
            CAUSA = causa;
            CAUSAACCIONINMEDIATA = causaaccioninmediata;
            RESPONSABLE = responsable;
            PLAZOCIERRE = plazocierre;
            CERRADAENFECHA = cerradaenfecha;
            ACCIONCORRECTIVA = accioncorrectiva;
            ACCIONRESPONSABLE = accionresponsable;
            ACCIONPLAZOCIERRE = accionplazocierre;
            ACCIONCERRADAENFECHA = accioncerradaenfecha;
            ESTADO = estado;
            CIERRE = cierre;
            OBSERVACIONES = OBS;
        }
        public NoConformidad(int id, string numero, DateTime fecha, string iso, string nc, string refiere, string precedimiento, string origen, string descripcion, string causa, string causaaccioninmediata, string responsable, DateTime plazocierre, string cerradaenfecha, string accioncorrectiva, string accionresponsable, DateTime accionplazocierre, string accioncerradaenfecha, string estado, DateTime cierre, string OBS, string tTipo)
        {
            ID = id;
            NUMERO = numero;
            FECHA = fecha;
            ISO = iso;
            NC = nc;
            REFIEREASECTOR = refiere;
            PROCEDIMIENTO = precedimiento;
            ORIGEN = origen;
            DESCRIPCION = descripcion;
            CAUSA = causa;
            CAUSAACCIONINMEDIATA = causaaccioninmediata;
            RESPONSABLE = responsable;
            PLAZOCIERRE = plazocierre;
            CERRADAENFECHA = cerradaenfecha;
            ACCIONCORRECTIVA = accioncorrectiva;
            ACCIONRESPONSABLE = accionresponsable;
            ACCIONPLAZOCIERRE = accionplazocierre;
            ACCIONCERRADAENFECHA = accioncerradaenfecha;
            ESTADO = estado;
            CIERRE = cierre;
            OBSERVACIONES = OBS;
            TIPO = tTipo;
        }



       
    }
}