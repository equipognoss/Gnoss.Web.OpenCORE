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
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Modelo de la página de administrar comunidad Home
    /// </summary>
    public class AdministrarDocumentacionModel
    {
        /// <summary>
        /// 
        /// </summary>
        // public Dictionary<string, string> ListaIdiomas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        // public Dictionary<string, string> ListaIdiomasPlataforma { get; set; }
        /// <summary>
        ///       
    }

    /// <summary>
    /// Controller de administrar configuración inicial del proyecto.
    /// </summary>
    public partial class AdministrarDocumentacionController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarDocumentacionController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarDocumentacionController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
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
        /// Método index que devuelve la vista de configuración inicial de la comunidad/proyecto
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            var seccionDevTools = RequestParams("section");
            ViewResult view = View();

            // SecciónDevTools de la que se desea la documentación            
            if (RequestParams("section") != null)
            {
                AdministracionSeccionesDevTools.SeccionesDevTools currentSection;
                if (Enum.TryParse(RequestParams("section"), out currentSection))
                {
                    switch (currentSection) {
                        case AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad:
                            // Documentación padre de Información General de la Comunidad
                            view = View("Index_InformacionGeneral.cshtml");
                            // Establecer el título según el parámetro pasado vía URL
                            ViewBag.HeaderTitle = "Administración de Comunidad";
                            break;
                    }
                }          
            }
            
            ViewBag.BodyClassPestanya = "meta-administrador documentationPage";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Documentacion;
            // Tener en cuenta el parámetro pasado vía URL
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Documentacion_Comunidad;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "DOCUMENTACION");
            

            // Activar la visualización del icono de la documentación de la sección
            ViewBag.showDocumentationByDefault = "true";

            return View();
        }
    }
}