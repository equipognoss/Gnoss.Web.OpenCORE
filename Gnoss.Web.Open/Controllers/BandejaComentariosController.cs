using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class BandejaComentariosController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public BandejaComentariosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<BandejaComentariosController> logger, ILoggerFactory loggerFactory)
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
            TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "COMENTARIOS");

            base.CargarTituloPagina();
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.NotificacionesAgrupadas.ToString())).ToList();
            bool notificacionesAgrupadas = busqueda.Count > 0 && bool.Parse(busqueda.First().Valor);
            if (notificacionesAgrupadas)
            {
                return new RedirectResult(Request.Path.ToString().Replace(GnossUrlsSemanticas.GetURLBandejaComentarios(BaseURL, UtilIdiomas, IdentidadActual),  GnossUrlsSemanticas.GetURLBandejaNotificaciones(BaseURL, UtilIdiomas, IdentidadActual)));   
            }


            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            } 
            
            ControladorDocumentacion.ResetearContadorNuevosComentarios(IdentidadActual.PerfilID);
            
            if (IdentidadActual.IdentidadOrganizacion != null && EsIdentidadActualAdministradorOrganizacion)
            {
                ControladorDocumentacion.ResetearContadorNuevosComentarios(IdentidadActual.IdentidadOrganizacion.PerfilID);
            }

            string grafo = mControladorBase.UsuarioActual.UsuarioID.ToString();
            string parametrosAdicionales = "gnoss:PerfilID=gnoss:" + IdentidadActual.PerfilID.ToString().ToUpper();

            string variableLeerMaster = "primeraCargaDeFacetas = false;";
            if (mControladorBase.UsuarioActual.UsarMasterParaLectura)
            {
                variableLeerMaster += " bool_usarMasterParaLectura = true;";
            }

            if (EsEcosistemaSinMetaProyecto && ProyectoVirtual != null)
            {
                parametrosAdicionales = "proyectoVirtualID=" + ProyectoVirtual.Clave + "|" + parametrosAdicionales;
            }

            insertarScriptBuscador("divFac", "divRel", "divNumResultadosBusqueda", "panListadoFiltrosPulgarcito", "divRel", "divFiltros", grafo, "navegadorBusqueda", "", "", parametrosAdicionales, "", "", "", variableLeerMaster, Guid.Empty, ProyectoSeleccionado.Clave, null, (short)TipoBusqueda.Comentarios, null, null);

            NotificationViewModel modelo = new NotificationViewModel();
            modelo.UrlActionAcceptInvitation = GnossUrlsSemanticas.GetURLBandejaInvitaciones(BaseURL, UtilIdiomas, IdentidadActual) + "/accept";
            modelo.UrlActionRejectInvitation = GnossUrlsSemanticas.GetURLBandejaInvitaciones(BaseURL, UtilIdiomas, IdentidadActual) + "/reject";
            modelo.UrlActionMarkReadComment = GnossUrlsSemanticas.GetURLBandejaComentarios(BaseURL, UtilIdiomas, IdentidadActual) +"/markRead";

            return View(modelo);
        }

        [HttpPost]
        public ActionResult MarkRead(string listComments)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            List<Guid> listaComentarioSeleccionados = new List<Guid>();

            if (listComments != "")
            {
                char[] separador = { ',' };
                foreach (string comentarioSeleccionado in listComments.Split(separador, StringSplitOptions.RemoveEmptyEntries))
                {
                    listaComentarioSeleccionados.Add(new Guid(comentarioSeleccionado));
                }
                MarcarComentariosLeidos(listaComentarioSeleccionados);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaCorreosSeleccionados"></param>
        private void MarcarComentariosLeidos(List<Guid> pListaComentariosSeleccionados)
        {
            FacetadoCN facetadoCN = new FacetadoCN("acidHome", UrlIntragnoss, "", ReplicacionAD.COLA_REPLICACION_MASTER_HOME, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<LiveCN>(), mLoggerFactory);
            Dictionary<Guid, bool> listaComentariosConLeido = facetadoCN.ObtenerLeidoListaComentarios(mControladorBase.UsuarioActual.UsuarioID, pListaComentariosSeleccionados);

            foreach (Guid comentarioID in pListaComentariosSeleccionados)
            {
                if (!listaComentariosConLeido.ContainsKey(comentarioID) || !listaComentariosConLeido[comentarioID]) //Si no está ya leido hay que disminuir los contadores.
                {
                    mControladorBase.UsuarioActual.UsarMasterParaLectura = true;

                    facetadoCN.ModificarPendienteLeerALeido(mControladorBase.UsuarioActual.UsuarioID.ToString(), comentarioID.ToString());

                    liveCN.DisminuirContadorComentariosLeidos(IdentidadActual.PerfilID);
                }
            }
            facetadoCN.Dispose();
            liveCN.Dispose();
        }
    }
}
