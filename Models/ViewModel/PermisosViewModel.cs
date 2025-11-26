using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models.ViewModel
{
    public class PermisosViewModel
    {
        public List<PuestoModel> Puestos { get; set; }

        public PermisosViewModel()
        {
            Puestos = new List<PuestoModel>();
        }
    }

    public class PuestoModel
    {
        public int ID { get; set; }
        public string PUESTO { get; set; }
    }
}
