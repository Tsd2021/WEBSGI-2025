using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class DocumentosTipo
    {
        public int ID { get; set; }
        public string TIPO { get; set; }

        public DocumentosTipo()
        { }

        public DocumentosTipo(int id, string tipo)
        {
            ID = id;
            TIPO = tipo;
        }
    }
}