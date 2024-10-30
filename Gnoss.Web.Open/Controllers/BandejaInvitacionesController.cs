using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Usuarios;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Invitacion;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
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
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class BandejaInvitacionesController : ControllerBaseWeb
    {
        public BandejaInvitacionesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
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
            TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "INVITACIONES");

            base.CargarTituloPagina();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            //bool notificacionesAgrupadas = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.NotificacionesAgrupadas.ToString() + "'").Length > 0 && bool.Parse((string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.NotificacionesAgrupadas.ToString() + "'")[0]["Valor"]);
            List<AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.NotificacionesAgrupadas.ToString())).ToList();
            bool notificacionesAgrupadas = busqueda.Count > 0 && bool.Parse(busqueda.First().Valor);
            if (notificacionesAgrupadas)
            {
                return new RedirectResult(Request.Path.ToString().Replace(GnossUrlsSemanticas.GetURLBandejaInvitaciones(BaseURL, UtilIdiomas, IdentidadActual), GnossUrlsSemanticas.GetURLBandejaNotificaciones(BaseURL, UtilIdiomas, IdentidadActual)));
            }

            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            ControladorAmigos.ResetearContadorNuevasInvitaciones(IdentidadActual.PerfilID);

            if (IdentidadActual.IdentidadOrganizacion != null && EsIdentidadActualAdministradorOrganizacion)
            {
                ControladorAmigos.ResetearContadorNuevasInvitaciones(IdentidadActual.IdentidadOrganizacion.PerfilID);
            }

            string grafo = mControladorBase.UsuarioActual.UsuarioID.ToString();

            Guid identidadBusqueda = mControladorBase.UsuarioActual.IdentidadID;

            if (RequestParams("organizacion") == "true")
            {
                grafo = IdentidadActual.OrganizacionID.Value.ToString();
                identidadBusqueda = IdentidadActual.IdentidadOrganizacion.Clave;
            }

            string parametrosAdicionales = "";

            if (EsEcosistemaSinMetaProyecto && ProyectoVirtual != null)
            {
                parametrosAdicionales = "proyectoVirtualID=" + ProyectoVirtual.Clave + "|" + parametrosAdicionales;
            }

            string javascriptAdicional = "primeraCargaDeFacetas = false;";

            insertarScriptBuscador("divFac", "divRel", "divNumResultadosBusqueda", "panListadoFiltrosPulgarcito", "divRel", "divFiltros", grafo, "navegadorBusqueda", "", "", parametrosAdicionales, "", "", "", javascriptAdicional, identidadBusqueda, ProyectoSeleccionado.Clave, null, (short)TipoBusqueda.Invitaciones, null, null);

            NotificationViewModel modelo = new NotificationViewModel();
            modelo.UrlActionAcceptInvitation = GnossUrlsSemanticas.GetURLBandejaInvitaciones(BaseURL, UtilIdiomas, IdentidadActual) + "/accept";
            modelo.UrlActionRejectInvitation = GnossUrlsSemanticas.GetURLBandejaInvitaciones(BaseURL, UtilIdiomas, IdentidadActual) + "/reject";
            modelo.UrlActionMarkReadComment = GnossUrlsSemanticas.GetURLBandejaComentarios(BaseURL, UtilIdiomas, IdentidadActual) + "/markRead";

            return View(modelo);
        }
    }
}
