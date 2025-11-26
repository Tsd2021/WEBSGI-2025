using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEBSGI.Models.ViewModel
{
    public class DocumentoViewModel
    {
        public Documentos Documento { get; set; }
        public string Carpeta { get; set; }
        public int SelectedCarpeta { get; set; }
        public IEnumerable<SelectListItem> Carpetas { get; set; }

        public string Tipo { get; set; }
        public int SelectedTipo { get; set; }
        public IEnumerable<SelectListItem> Tipos { get; set; }

        public int IdSelectedEstado { get; set; }
        public string SelectedEstado { get; set; }
        public IEnumerable<SelectListItem> Estados { get; set; }

        public IEnumerable<SelectListItem> Puestos { get; set; }
        public List<string> PuestosSeleccionados { get; set; } // ✅ Lista de IDs seleccionados


        public string Revision { get; set; }

        public string Titulo { get; set; }

        public string Observaciones { get; set; }
        public IEnumerable<WEBSGI.Models.DocumentosCarpeta> FolderTree { get; set; }

        public byte[] Archivo { get; set; }
        public int SelectedFolderId { get; set; }

    }
}