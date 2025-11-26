using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models.ViewModel
{
    public class PuestosViewModel
    {
        public int ID { get; set; }
        public string PUESTO { get; set; }
        public string DESCRIPCION { get; set; }
        public string FUNCIONES { get; set; }
        public string PERFIL { get; set; }
        public string REQUISITOS { get; set; }

        public int IDPUESTOPADRE { get; set; }
        public string PUESTOPADRE { get; set; }

        public List<Puestos> Puestos { get; set; }

    }
}