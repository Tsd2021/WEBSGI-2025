using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class DocumentosVisibilidad
    {
        public int ID { get; set; }
        public int IDDOCUMENTO { get; set; }
        public int IDPUESTO { get; set; }
        public string PUESTO { get; set; }

        public DocumentosVisibilidad()
        {

        }

        public DocumentosVisibilidad(int id, int iddoc, int idpuesto, string puesto)
        {
            ID = id;
            IDDOCUMENTO = iddoc;
            IDPUESTO = idpuesto;
            PUESTO = puesto;
        }
    }
}