using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System;
using Es.Riam.Gnoss.CL.Notificacion;
using AspNetCoreGeneratedDocument;
using Es.Riam.Gnoss.Web.MVC.Models.Notificaciones;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Microsoft.Extensions.Logging;
namespace Gnoss.Web.Open.Controllers
{
    public class BandejaNotificacionesPersonalizadasController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public BandejaNotificacionesPersonalizadasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<BandejaNotificacionesPersonalizadasController> logger, ILoggerFactory loggerFactory)
           : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EsPaginaEdicion = true;
            base.OnActionExecuting(filterContext);
        }

        protected override void CargarTituloPagina()
        {
            TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "NOTIFICACIONESPERSO");

            base.CargarTituloPagina();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }
            //ViewBag.applicationServerKey = mConfigService.ObtenerVapidPublicKey();
            List<NotificacionesModel> notificaciones = ControladorNotificaciones.ObtenerNotificacionesSinLeer(IdentidadActual.PerfilID);
            string grafo = mControladorBase.UsuarioActual.UsuarioID.ToString();
            string parametrosAdicionales = null;
            Guid identidadBusqueda = mControladorBase.UsuarioActual.IdentidadID;

            if (RequestParams("organizacion") == "true")
            {
                grafo = IdentidadActual.OrganizacionID.Value.ToString();
                identidadBusqueda = IdentidadActual.IdentidadOrganizacion.Clave;
            }

            if (EsEcosistemaSinMetaProyecto && ProyectoVirtual != null)
            {
                parametrosAdicionales = "proyectoVirtualID=" + ProyectoVirtual.Clave + "|" + parametrosAdicionales;
            }

            string variableLeerMaster = "primeraCargaDeFacetas = false;";
            if (mControladorBase.UsuarioActual.UsarMasterParaLectura)
            {
                variableLeerMaster += " bool_usarMasterParaLectura = true;";
            }
            insertarScriptBuscador("divFac", "divRel", "divNumResultadosBusqueda", "panListadoFiltrosPulgarcito", "divRel", "divFiltros", grafo, "navegadorBusqueda", "", "", parametrosAdicionales, "", "", "", variableLeerMaster, identidadBusqueda, ProyectoSeleccionado.Clave, null, (short)TipoBusqueda.Mensajes, null, null);

            return View(notificaciones);
        }

        [HttpGet]
        public IActionResult VistaExtendida(string claveCache)
        {
            GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            NotificacionesModel notificacion;
            try
            {
                NotificationHtml not = (NotificationHtml)gnossCacheCL.ObtenerObjetoDeCache(claveCache, typeof(NotificationHtml));
                notificacion = new NotificacionesModel(not.html,not.perfilID,not.fechaNotificacion,not.leida);
            }
            catch (Exception)
            {

                NotificacionDefault not = (NotificacionDefault)gnossCacheCL.ObtenerObjetoDeCache(claveCache, typeof(NotificacionDefault));
                notificacion = new NotificacionesModel(not.contenidoNotificacion, not.idPerfil, not.fechaNotificacion, not.leida,not.urlNotificacion);
            }
            
            
            if (notificacion != null)
            {
                return PartialView("_VistaExtendida", notificacion);
            }
            return NotFound();
        }

        [HttpPost]
        public void deleteRead(List<string> notificacionesLeidas)
        {
            foreach (var item in notificacionesLeidas)
            {
                ControladorNotificaciones.EliminarNotificacionesLeidas(item.ToString());
            }
        }
    }
}
