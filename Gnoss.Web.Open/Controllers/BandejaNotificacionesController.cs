using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
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
using System;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class BandejaNotificacionesController : ControllerBaseWeb
    {
        public BandejaNotificacionesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EsPaginaEdicion = true;
            base.OnActionExecuting(filterContext);
        }

        protected override void CargarTituloPagina()
        {
            TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "NOTIFICACIONES");

            base.CargarTituloPagina();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            ControladorDocumentacion.ResetearContadorNuevosComentarios(IdentidadActual.PerfilID);
            ControladorAmigos.ResetearContadorNuevasInvitaciones(IdentidadActual.PerfilID);

            if (IdentidadActual.IdentidadOrganizacion != null && EsIdentidadActualAdministradorOrganizacion)
            {
                ControladorDocumentacion.ResetearContadorNuevosComentarios(IdentidadActual.IdentidadOrganizacion.PerfilID);
                ControladorAmigos.ResetearContadorNuevasInvitaciones(IdentidadActual.IdentidadOrganizacion.PerfilID);
            }

            string grafo = mControladorBase.UsuarioActual.UsuarioID.ToString();

            string parametrosAdicionales = "Comentario;gnoss:PerfilID=gnoss:" + IdentidadActual.PerfilID.ToString().ToUpper();
            string javascriptAdicional = "primeraCargaDeFacetas = false;";
            if (mControladorBase.UsuarioActual.UsarMasterParaLectura)
            {
                javascriptAdicional += " bool_usarMasterParaLectura = true;";
            }

            Guid identidadBusqueda = mControladorBase.UsuarioActual.IdentidadID;
            if (RequestParams("organizacion") == "true")
            {
                grafo = IdentidadActual.OrganizacionID.Value.ToString();
                identidadBusqueda = IdentidadActual.IdentidadOrganizacion.Clave;
            }

                
            insertarScriptBuscador("divFac", "divRel", "divNumResultadosBusqueda", "panListadoFiltrosPulgarcito", "divRel", "divFiltros", grafo, "navegadorBusqueda", "", "", parametrosAdicionales, "", "", "", javascriptAdicional, identidadBusqueda, ProyectoSeleccionado.Clave, null, (short)TipoBusqueda.Notificaciones, null, null);


            NotificationViewModel modelo = new NotificationViewModel();
            modelo.UrlActionAcceptInvitation = GnossUrlsSemanticas.GetURLBandejaInvitaciones(BaseURL, UtilIdiomas, IdentidadActual) + "/accept";
            modelo.UrlActionRejectInvitation = GnossUrlsSemanticas.GetURLBandejaInvitaciones(BaseURL, UtilIdiomas, IdentidadActual) + "/reject";
            modelo.UrlActionMarkReadComment = GnossUrlsSemanticas.GetURLBandejaComentarios(BaseURL, UtilIdiomas, IdentidadActual) + "/markRead";

            return View(modelo);
        }        
    }
}
