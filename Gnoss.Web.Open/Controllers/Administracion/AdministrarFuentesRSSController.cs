using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{

    /// <summary>
    /// 
    /// </summary>
    public class FuenteViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Perioicidad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short NumPerioicidad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, string> Editores{ get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool TagsDeCategorias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool TagsDeTitulo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ThesaurusEditorModel Tesauro { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Guid> CategoriasSeleccionadas{ get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Compartir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ObtenerResumen { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarFuentesRSSController : ControllerBaseWeb
    {
        public AdministrarFuentesRSSController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            List<FuenteViewModel>  listaFuentes = new List<FuenteViewModel>();

            return View(listaFuentes);
        }


        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult New()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Validar(string url, string user, string password)
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            FuenteViewModel fuente = new FuenteViewModel();
            fuente.Key = Guid.Empty;
            fuente.Url = url;

            fuente.NumPerioicidad = 1;

            fuente.Editores = new Dictionary<Guid, string>();

            fuente.Tesauro = new ThesaurusEditorModel();
            fuente.Tesauro.ThesaurusCategories = Comunidad.Categories;
            //ComprobarCategoriasDeshabilitadas(GestorDocumental.GestorTesauro);

            fuente.Tesauro.SelectedCategories = new List<Guid>();

            return PartialView("_PanelEdicion", fuente);
        }

    }
}