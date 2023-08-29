using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    public class AdministrarSeoGooglePlataformaController : ControllerBaseWeb
    {
        private AdministrarSeoGooglePlataformaViewModel mPaginaModel = null;
        private bool mConfigDeParametroAplicacion = false;
        private string mMetaRobots = "all";
        public AdministrarSeoGooglePlataformaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
           : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }
        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_SEO;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "POSICIONAMIENTOYANALITICA");

            return View(PaginaModel);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(AdministrarSeoGooglePlataformaViewModel pParametros)
        {
            ControladorSeoGoogle contrSeoGoogle = new ControladorSeoGoogle(ProyectoSeleccionado, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

            pParametros.CodigoGoogleAnalytics = HttpUtility.UrlDecode(pParametros.CodigoGoogleAnalytics);
            pParametros.ScriptGoogleAnalytics = HttpUtility.UrlDecode(pParametros.ScriptGoogleAnalytics);

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            try
            {
                mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);

                contrSeoGoogle.GuardarConfiguracionSeoGooglePlataforma(pParametros);

                if (transaccionIniciada)
                {
                    mEntityContext.TerminarTransaccionesPendientes(true);
                }
            }
            catch (Exception ex)
            {
                if (transaccionIniciada)
                {
                    proyAD.TerminarTransaccion(false);
                }

                return GnossResultERROR(ex.Message);
            }
            contrSeoGoogle.InvalidarCaches();
            return GnossResultOK();
        }

        private AdministrarSeoGooglePlataformaViewModel PaginaModel
        {
            get
            {

                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarSeoGooglePlataformaViewModel();

                    mPaginaModel.RobotsBusqueda = RobotsComunidad;
                    mPaginaModel.ConfiguracionEnParametroAplicacion = mConfigDeParametroAplicacion;
                    mPaginaModel.RobotsEnConfig = mConfigService.ObtenerRobots();
                    mPaginaModel.CodigoGoogleAnalytics = CodigoGoogleAnalytics;
                    mPaginaModel.ScriptGoogleAnalytics = ScriptGoogleAnalytics;
                    mPaginaModel.ScriptPorDefecto = ScriptPorDefecto;

                }
                return mPaginaModel;
            }
        }

        private string RobotsComunidad
        {
            get
            {
                mConfigDeParametroAplicacion = false;
                
                ParametroAplicacion parametroAplicacion = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.RobotsComunidad)).FirstOrDefault();
                if (parametroAplicacion != null)
                {
                    mConfigDeParametroAplicacion = true;
                    mMetaRobots = parametroAplicacion.Valor;
                    return mMetaRobots;
                }

                string metaRobots = mConfigService.ObtenerRobots();
                if (!string.IsNullOrEmpty(metaRobots))
                {
                    mMetaRobots = metaRobots;
                    return mMetaRobots;
                }

                if (ProyectoMetaRobotsRows != null && ProyectoMetaRobotsRows.Any(filaMetaRobots => filaMetaRobots.Tipo.Equals((short)TipoPaginaMetaRobots.Pagina)))
                {
                    return ProyectoMetaRobotsRows.FirstOrDefault(filaMetaRobots => filaMetaRobots.Tipo.Equals((short)TipoPaginaMetaRobots.Pagina)).Content;
                }
                return mMetaRobots;
            }
            set
            {
                mMetaRobots = value;
            }
        }

        private string CodigoGoogleAnalytics
        {
            get
            {
                //comprobar si esta en ParametroAplicacion
                ParametroAplicacion busqueda = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals("CodigoGoogleAnalytics")).FirstOrDefault();
                if (busqueda != null)
                {
                    return busqueda.Valor;
                }
                return "";
            }
        }

        private string ScriptGoogleAnalytics
        {
            get
            {
                //comprobar si esta en ParametroAplicacion
                ParametroAplicacion busqueda = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals("ScriptGoogleAnalytics")).FirstOrDefault();

                if (busqueda != null)
                {
                    return busqueda.Valor;
                }
                return "";
            }
        }

        private string ScriptPorDefecto
        {
            get
            {
                string mScript = "var peticionGARetenida = false; (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){(i[r].q = i[r].q ||[]).push(arguments)},i [r].l=1*new Date(); a=s.createElement(o),m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a, m)})(window, document,'script','https://www.google-analytics.com/analytics.js','ga'); ga('create', '@@codigoga@@', 'auto'); var cname = \"cookieAviso.\" + document.location.host.replace(\"www.\",\"\"); var name = cname + \"=\"; var ca = document.cookie.split(';'); var valor=\"\"; for (var i = 0; i < ca.length; i++){var c = ca[i]; while (c.charAt(0) == ' '){c = c.substring(1);}if (c.indexOf(name) == 0){valor=c.substring(name.length, c.length);}} if (valor.toLowerCase() == \"3gv3jeLqUkVdjqrfSUK0MA%3d%3d\".toLowerCase()){ga('send', 'pageview');}else{peticionGARetenida = true;}; ";
                return mScript;
            }
        }

        private void InvalidarCachesParametroAplicacion()
        {
            ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            parametroAplicacionCL.InvalidarCacheParametrosAplicacion();
        }
    }
}