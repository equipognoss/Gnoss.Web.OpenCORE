using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;


namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador de la página de de modificar la consultas SPARQL de la tabla ProyectoSearchPersonalizado
    /// </summary>
    public class AdministrarParametrosBusquedaPersonalizadosController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarParametrosBusquedaPersonalizadosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarParametrosBusquedaPersonalizadosController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
        }

        #region Miembros

        private AdministrarParametrosBusquedaPersonalizadosViewModel mPaginaModel = null;
        private ProyectoCN proyCN;

        #endregion

        #region Métodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarParametrosDeBusquedaPersonalizados } })]
		public ActionResult Index()
        {

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "grafo-de-conocimiento edicion edicionObjetos no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.DescubrimientoAnalisis;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.DescubrimientoAnalisis_Personalizacion_Consulta_Busqueda;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "DESCUBRIMIENTOYANALISIS");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "PARAMETROSDEBUSQUEDAPERSONALIZADOS");

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            mPaginaModel = CargarModelo();
            return View(mPaginaModel);
        }

        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarParametrosDeBusquedaPersonalizados } })]
		public ActionResult CrearLoadModal()
        {
            // Creo un parámetro "nuevo/vacío" para la construcción correcta posterioro del modal
            ParametroBusquedaPersonalizadoModel param = new ParametroBusquedaPersonalizadoModel();
            return GnossResultHtml("_partial-views/_edit-parametro-busqueda", param);
        }


        /// <summary>
        /// Guardar
        /// </summary>
        /// <param name="ListaPestanyas"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarParametrosDeBusquedaPersonalizados } })]
		[TypeFilter(typeof(AccesoIntegracionAttribute))]
		public ActionResult Guardar(List<ParametroBusquedaPersonalizadoModel> ListaPestanyas)
        {
            GuardarLogAuditoria();
            bool iniciado = false;
            try
            {
                iniciado = HayIntegracionContinua;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                return GnossResultERROR("Contacte con el administrador del Proyecto, no es posible atender la petición.");
            }
            try
            {
                Guid organizacionID = proyCN.ObtenerOrganizacionIDAPartirDeProyectoID(ProyectoSeleccionado.Clave);
                proyCN.ActualizarParametrosBusquedaPersonalizados(organizacionID, ProyectoSeleccionado.Clave, ListaPestanyas);
                if (iniciado)
                {
                    HttpResponseMessage resultado = InformarCambioAdministracion("SearchPersonalizado", JsonConvert.SerializeObject(ListaPestanyas, Formatting.Indented));
                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);
                proyCL.Dispose();
                mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }
            
        }       
        #endregion

        #region Métodos públicos 

        private AdministrarParametrosBusquedaPersonalizadosViewModel CargarModelo()
        {
            AdministrarParametrosBusquedaPersonalizadosViewModel adModCon = new AdministrarParametrosBusquedaPersonalizadosViewModel();
            List <ProyectoSearchPersonalizado> listaProyectos = new List<ProyectoSearchPersonalizado>();
            listaProyectos = proyCN.ObtenerProyectosSearchPersonalizado(ProyectoSeleccionado.Clave);

            List<ParametroBusquedaPersonalizadoModel> listaAPasar = new List<ParametroBusquedaPersonalizadoModel>();

            foreach(ProyectoSearchPersonalizado proy in listaProyectos)
            {
                ParametroBusquedaPersonalizadoModel param = new ParametroBusquedaPersonalizadoModel();
                param.NombreParametro = proy.NombreFiltro;
                param.WhereParametro = proy.WhereSPARQL;
                param.OrderByParametro = proy.OrderBySPARQL;
                param.WhereFacetaParametro = proy.WhereFacetasSPARQL;

                listaAPasar.Add(param);
            }
            adModCon.ListaParametros = listaAPasar;
            return adModCon;
        }

        #endregion
    }
}