using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class Documentos
    {
        public int ID { get; set; }
        public string TITULO { get; set; }
        public string REVISION { get; set; }
        public int IDCARPETA { get; set; }
        public string CARPETA { get; set; }
        public int IDTIPO { get; set; }
        public string TIPO { get; set; }
        public int IDESTADO { get; set; }
        public string ESTADO { get; set; }

       
        public string OBSERVACIONES { get; set; }
        public byte[] ARCHIVO { get; set; }
        public string NOMBREARCHIVO { get; set; }


        public Documentos()
        { }

        public Documentos(int id, string titulo, string revision, int idcarpeta, string carpeta, int idtipo, string tipo, int idestado, string estado, string obs,byte[] archivo, string nombrearchivo)
        {
            ID = id;
            TITULO = titulo;
            REVISION = revision;
            IDCARPETA = idcarpeta;
            CARPETA = carpeta;
            IDTIPO = idtipo;
            TIPO = tipo;
            IDESTADO = idestado;
            ESTADO = estado;
            OBSERVACIONES = obs;
            ARCHIVO = archivo;
            NOMBREARCHIVO = nombrearchivo;
          
        }

    }
}