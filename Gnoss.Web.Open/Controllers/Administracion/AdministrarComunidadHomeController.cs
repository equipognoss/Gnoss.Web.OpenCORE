using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.Extensions.Hosting;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Modelo de la página de administrar comunidad Home
    /// </summary>
    public class AdministrarComunidadHomeModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ListaIdiomas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ListaIdiomasPlataforma { get; set; }
        /// <summary>
        ///       
    }

    public class AdministrarComunidadHomeSectionModel
    {
        public string HomeSection{get;set;}    
    }


    /// <summary>
    /// Controller de administrar comunidad Home
    /// </summary>
    public partial class AdministrarComunidadHomeController : ControllerBaseWeb
    {
        public AdministrarComunidadHomeController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        ///private AdministrarComunidadHomeModel mPaginaModel = null;
        /// <summary>
        /// 
        /// </summary>
        //private ParametroGeneralDS mParametrosGeneralesDS;
        ///private GestorParametroGeneral mParametrosGeneralesDS;
        /// <summary>
        /// 
        /// </summary>
        ///private ParametroGeneral mFilaParametrosGenerales = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Home;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_Home;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderTitle = "Home";

            return View();
        }

        /// <summary>
        /// Devolver la vista parcial o sección para la Home de DevTools dependiendo el parámetro de entrada obtenido al haber hecho
        /// click en el menú de navegación de la Home
        /// </summary>
        /// <returns></returns>
        [HttpPost]        
        public ActionResult LoadHomeSection(AdministrarComunidadHomeSectionModel pModel)
        {
            
            ActionResult homePartialView = View();

            // Cargar permisos de Comunidad en ViewBag
            CargarPermisosAdministracionComunidadEnViewBag();

            switch (pModel.HomeSection)
            {
                case "comunidad":
                    homePartialView = GnossResultHtml("../AdministrarComunidadHome/_partial-views/_home-community-section" , null);
                                      
                    break;
                case "estructura":
                    homePartialView = GnossResultHtml("../AdministrarPestanyas/_partial-views/_home-community-section", null);
                    break;

                case "configuracion":
                    homePartialView = GnossResultHtml("../AdministrarAplicaciones/_partial-views/_home-community-section", null);                    
                    break;

                case "grafo-de-conocimiento":
                    homePartialView = GnossResultHtml("../AdministrarObjetosConocimiento/_partial-views/_home-community-section", null);
                    break;

                case "insights":
                    homePartialView = GnossResultHtml("../AdministrarPestanyas/_partial-views/_home-community-section-insights", null);
                    break;

                case "apariencia":
                    homePartialView = GnossResultHtml("../AdministrarVistas/_partial-views/_home-community-section", null);
                    break;

                case "integracionContinua":
                    homePartialView = GnossResultHtml("../AdministrarIntegracionContinua/_partial-views/_home-community-section", null);
                    break;

                default:
                    homePartialView = GnossResultHtml("../AdministrarComunidadHome/_partial-views/_home-not-found-section", null);
                    break;
            }
            // Devolver la sección de la home correspondiente
            return homePartialView;
        }
        

        public ActionResult LoadPageSection(string administrarSeccion)
        {

            ActionResult homePartialView = View();

            // Añadir clase para el body del Layout (Anchura contenida)
            ViewBag.BodyClassPestanya = "edicion max-width-container";

            // Cargar permisos de Comunidad en ViewBag
            CargarPermisosAdministracionComunidadEnViewBag();

            switch (administrarSeccion)
            {
                case "comunidad":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_Home;
                    homePartialView = GnossResultHtml("../AdministrarComunidadHome/Index_ComunidadSection", null);
                    break;
                case "estructura":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_Home;
                    // Establecer el título para el header de DevTool 
                    homePartialView = GnossResultHtml("../AdministrarPestanyas/Index_EstructuraSection", null);
                    break;

                case "configuracion":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Home;
                    // Establecer el título para el header de DevTool                  
                    homePartialView = GnossResultHtml("../AdministrarAplicaciones/Index_ConfiguracionSection", null);
                    break;

                case "grafo-conocimiento":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_Home;
                    // Establecer el título para el header de DevTool  
                    homePartialView = GnossResultHtml("../AdministrarObjetosConocimiento/Index_GrafoConocimientoSection", null);
                    break;

                case "descubrimiento-analisis":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.DescubrimientoAnalisis;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.DescubrimientoAnalisis_Home;
                    // Establecer el título para el header de DevTool             
                    homePartialView = GnossResultHtml("../AdministrarPestanyas/Index_DescubrimientoAnalisisSection", null);                                                                               
                    break;

                case "apariencia":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Apariencia;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Apariencia_Home;
                    // Establecer el título para el header de DevTool          
                    homePartialView = GnossResultHtml("../AdministrarVistas/Index_AparienciaSection", null);
                    break;

                case "integracion-continua":
                    homePartialView = GnossResultHtml("../AdministrarIntegracionContinua/Index_IntegracionContinuaSection", null);
                    break;

                default:
                    homePartialView = GnossResultHtml("../AdministrarComunidadHome/_partial-views/_home-not-found-section", null);
                    break;
            }
            // Devolver la sección de la home correspondiente
            return homePartialView;
        }
    }
}