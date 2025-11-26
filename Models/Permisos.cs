using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class Permisos
    {
        public int ID { get; set; }
        public int ACCESO { get; set; }
        public int IDPUESTO { get; set; }
        public string PUESTO { get; set; }

        public Permisos()
        {

        }

        public Permisos(int id, int acceso, int idpuesto, string puesto)
        {
            ID = id;
            ACCESO = acceso;
            IDPUESTO = idpuesto;
            PUESTO = puesto;
        }


        public class PermisosAcceso
        {
            public const int DocumentosListados = 1;
            public const int NoConformidades = 4;
            public const int AccionesCorrectivas = 5;
            public const int OportunidadMejora = 11;


            // Método para obtener la descripción de un acceso
            //public string ObtenerDescripcion(int acceso)
            //{
            //    switch (acceso)
            //    {
            //        case 1: return "DOCUMENTOS LISTADOS";
            //        case 4: return "NO CONFORMIDADES VER";
            //        case 5: return "ACCIONES CORRECTIVAS";
            //        case 11: return "OPORTUNIDAD DE MEJORA";
            //        default: return "Desconocido";

            //    }
            //}
        }

        }
}