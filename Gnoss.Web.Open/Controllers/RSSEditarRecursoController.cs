using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class RSSEditarRecursoViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Titulo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Descripcion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Enlace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ThesaurusEditorModel Tesauro { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Guid> CategoriasSeleccionadas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> TagsTitulo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Compartir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Autores { get; set; }
    }
}