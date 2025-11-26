using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class Normas
    {

        public int ID { get; set; }
        public string NORMA { get; set; }
        public string DESCRIPCION { get; set; }

        public Normas()
        { }

        public Normas(int id, string norma, string descripcion)
        {
            ID = id;
            NORMA = norma;
            DESCRIPCION = descripcion;
        }
    }
}