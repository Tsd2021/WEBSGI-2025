using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class Puestos
    {
        public int ID { get; set; }

        public string PUESTO { get; set; }
        public int IDPUESTOPADRE { get; set; }
        public string PUESTOPADRE { get; set; }
        public string DESCRIPCION { get; set; }
        public string FUNCIONES { get; set; }
        public string PERFIL { get; set; }
        public string REQUISITOS { get; set; }

        public Puestos()
        { }

        public Puestos(int id, string puesto, int idpuestopadre, string puestopadre, string descripcion, string funciones, string perfil, string requisitos)
        {
            ID = id;
            PUESTO = puesto;
            IDPUESTOPADRE = idpuestopadre;
            PUESTOPADRE = puestopadre;
            DESCRIPCION = descripcion;
            FUNCIONES = funciones;
            PERFIL = perfil;
            REQUISITOS = requisitos;
        }
    }
}