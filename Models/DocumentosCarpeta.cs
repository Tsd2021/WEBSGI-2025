using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class DocumentosCarpeta
    {
        public int ID { get; set; }
        public string CARPETA { get; set; }
        public int TIPO { get; set; }
        public int IDCARPETA { get; set; }

        public DocumentosCarpeta()
        { }

        public DocumentosCarpeta(int id, string carpeta, int tipo, int idcarpeta)
        {
            ID = id;
            CARPETA = carpeta;
            TIPO = tipo;
            IDCARPETA = idcarpeta;
        }

    }
}