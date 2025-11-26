using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class Log
    {
        public int ID { get; set; }
        public string USUARIO { get; set; }
        public string ARCHIVO_MOVIMIENTO { get; set; }
        public string IPPUBLICA { get; set; }
        public string IPPRIVADA { get; set; }
        public DateTime FECHA { get; set; }

        public Log()
        { }

        public Log(int id, string usuario, string archivo, string ippublica, string ipprivada, DateTime fecha)
        {
            ID = id;
            USUARIO = usuario;
            ARCHIVO_MOVIMIENTO = archivo;
            IPPUBLICA = ippublica;
            IPPRIVADA = ipprivada;
            FECHA = fecha;
        }

    }
}