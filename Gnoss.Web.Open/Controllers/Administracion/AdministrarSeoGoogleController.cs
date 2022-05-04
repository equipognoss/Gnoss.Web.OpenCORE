using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
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
    public class AdministrarSeoGoogleController : ControllerBaseWeb
    {
        private AdministrarSeoGoogleViewModel mPaginaModel = null;
        private bool mConfigDeParametroProyecto = false;
        private bool mConfigGoogleDeParametroProyecto = false;
        private bool mConfigScriptDeParametroProyecto = false;
        private string mMetaRobots = "all";

        public AdministrarSeoGoogleController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }
        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View(PaginaModel);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(AdministrarSeoGoogleViewModel pParametros)
        {

            ControladorSeoGoogle contrSeoGoogle = new ControladorSeoGoogle(ProyectoSeleccionado, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mHttpContextAccessor);

            //GuardarXmlCambiosAdministracion();

            pParametros.CodigoGoogleAnalytics = HttpUtility.UrlDecode(pParametros.CodigoGoogleAnalytics);
            pParametros.ScriptGoogleAnalytics = HttpUtility.UrlDecode(pParametros.ScriptGoogleAnalytics);

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            try
            {
                mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);

                contrSeoGoogle.GuardarConfiguracionSeoGoogle(pParametros);

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

        private AdministrarSeoGoogleViewModel PaginaModel
        {
            get
            {

                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarSeoGoogleViewModel();
                    mPaginaModel.ValorRobotsBusqueda = MetaRobots;
                    mPaginaModel.RobotsBusqueda = RobotsComunidad;
                    mPaginaModel.ConfiguracionEnParametroProyecto = mConfigDeParametroProyecto;
                    mPaginaModel.CodigoGoogleAnalytics = CodigoGoogleAnalytics;
                    mPaginaModel.ConfigGoogleEnParametroProyecto = mConfigGoogleDeParametroProyecto;
                    mPaginaModel.ScriptGoogleAnalytics = ScriptGoogleAnalytics;
                    mPaginaModel.ConfigScriptEnParametroProyecto = mConfigScriptDeParametroProyecto;
                    mPaginaModel.ScriptPorDefecto = ScriptPorDefecto;
                }
                return mPaginaModel;
            }
        }

        private string CodigoGoogleAnalytics
        {
            get
            {
                ControladorSeoGoogle contrSeoGoogle = new ControladorSeoGoogle(ProyectoSeleccionado, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mHttpContextAccessor);
                //string valorCodigo = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "CodigoGoogleAnalytics");
                //comprobar si esta en ParametroProyecto

                string valorCodigo = contrSeoGoogle.FilaParametrosGenerales.CodigoGoogleAnalytics;
                if (!string.IsNullOrEmpty(valorCodigo))
                {
                    mConfigGoogleDeParametroProyecto = true;
                    return valorCodigo;
                }
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
                ControladorSeoGoogle contrSeoGoogle = new ControladorSeoGoogle(ProyectoSeleccionado, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mHttpContextAccessor);
                //comprobar si esta en ParametroProyecto
                //string valorScript = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "ScriptGoogleAnalytics");
                string valorScript = contrSeoGoogle.FilaParametrosGenerales.ScriptGoogleAnalytics;
                if (!string.IsNullOrEmpty(valorScript))
                {
                    mConfigScriptDeParametroProyecto = true;
                    return valorScript;
                }
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
        /// <summary>
        /// Obtiene la metaetiqueta "robots"
        /// </summary>
        private string MetaRobots
        {
            get
            {
                //if (mMetaRobots.Equals("all")) { 
                //ParametroProyecto
                mConfigDeParametroProyecto = false;
                string valorRobotsParametroProyecto = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.RobotsComunidad);
                if (!string.IsNullOrEmpty(valorRobotsParametroProyecto))
                {
                    mConfigDeParametroProyecto = true;
                    mMetaRobots = ParametroProyecto[ParametroAD.RobotsComunidad];
                    return mMetaRobots;
                }

                //ParametroAplicacion
                List<ParametroAplicacion> busqueda = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.RobotsComunidad)).ToList();
                ParametroAplicacion parametroAplicacion = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.RobotsComunidad)).FirstOrDefault();
                if (parametroAplicacion != null)
                {
                    string valor = null;
                    //mMetaRobots = parametroAplicacion.Valor;
                    if (parametroAplicacion.Valor.Equals("all"))
                    {
                        valor = "1";
                    }
                    else
                    {
                        valor = "0";
                    }
                    return valor;
                }

                //fichero project.config
                
                string metaRobots = mConfigService.ObtenerRobots();
                if (!string.IsNullOrEmpty(metaRobots))
                {
                    mConfigDeParametroProyecto = true;
                    string valor = metaRobots;
                    if (metaRobots.Equals("all"))
                    {
                        valor = "1";
                    }
                    else
                    {
                        valor = "0";
                    }
                    return valor;
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

        private string RobotsComunidad
        {
            get
            {
                //ParametroAplicacion
                List<ParametroAplicacion> busqueda = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.RobotsComunidad)).ToList();
                ParametroAplicacion parametroAplicacion = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.RobotsComunidad)).FirstOrDefault();
                if (parametroAplicacion != null)
                {
                    mMetaRobots = parametroAplicacion.Valor;
                    return mMetaRobots;
                }

                //fichero project.config
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
    }
}