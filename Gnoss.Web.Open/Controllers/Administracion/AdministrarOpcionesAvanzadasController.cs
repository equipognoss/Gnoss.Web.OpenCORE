using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
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
using System.Net;
using System.Net.Http;
using System.Web;
using System.Xml;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para administrar las opciones avanzadas de la comunidad
    /// </summary>
    public class AdministrarOpcionesAvanzadasController : ControllerBaseWeb
    {
        public AdministrarOpcionesAvanzadasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        private AdministrarOpcionesAvanzadasViewModel mPaginaModel = null;

        #endregion

        #region Metodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
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
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Guardar(AdministrarOpcionesAvanzadasViewModel Options)
        {
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

            ControladorOpcionesAvanzadas contrOpcionesAvanzadas = new ControladorOpcionesAvanzadas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor);

            GuardarXmlCambiosAdministracion();

            //Options.CodigoGoogleAnalytics = HttpUtility.UrlDecode(Options.CodigoGoogleAnalytics);
            //Options.ScriptGoogleAnalytics = HttpUtility.UrlDecode(Options.ScriptGoogleAnalytics);

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            try
            {
                mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);

                contrOpcionesAvanzadas.GuardarOpcionesAvanzadas(Options);
                if (iniciado)
                {
                    CorreoIntegracion(Options);

                    if (Options.GruposVisibilidadAbierto == null) { Options.GruposVisibilidadAbierto = new Dictionary<Guid, string>(); }

                    if (Options.PestanyasSeleccionadas.Value.Equals(Guid.Empty)) { Options.PestanyasSeleccionadas = null; }

                    //if (Options.RobotsBusqueda == "") { Options.RobotsBusqueda = null; }

                    HttpResponseMessage resultado = InformarCambioAdministracion("OpcionesAvanzadas", JsonConvert.SerializeObject(Options, Newtonsoft.Json.Formatting.Indented));

                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }

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

            contrOpcionesAvanzadas.InvalidarCaches();

            return GnossResultOK();
        }

        private void CorreoIntegracion(AdministrarOpcionesAvanzadasViewModel options)
        {
            if(string.IsNullOrEmpty(options.ConfiguracionCorreo.Email) && string.IsNullOrEmpty(options.ConfiguracionCorreo.SMTP) && string.IsNullOrEmpty(options.ConfiguracionCorreo.User) && string.IsNullOrEmpty(options.ConfiguracionCorreo.Password))
            {
                options.ConfiguracionCorreo = null;
            }
        }

        #endregion

        #region Propiedades

        private AdministrarOpcionesAvanzadasViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    ControladorOpcionesAvanzadas contrOpcionesAvanzadas = new ControladorOpcionesAvanzadas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor);
                    mPaginaModel = contrOpcionesAvanzadas.CargarOpcionesAvanzadas();

                    //Twitter
                    mPaginaModel.UrlEnvioTwitter = "";
                    if (Conexion.UsarVariablesEntorno)
                    {
                        mPaginaModel.UrlEnvioTwitter = Environment.GetEnvironmentVariable("TwitterCallbackURL");
                    }
                    else if (System.IO.File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/GnossRedesSociales.config"))
                    {
                        XmlDocument documentoXml = new XmlDocument();
                        documentoXml.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/GnossRedesSociales.config");
                        mPaginaModel.UrlEnvioTwitter = documentoXml.GetElementsByTagName("TwitterCallbackURL").Item(0).InnerText;
                    }
                    else
                    {
                        mPaginaModel.UrlEnvioTwitter = BaseURL + "/EnviarTwitter.aspx";
                    }
                    mPaginaModel.UrlEnvioTwitter += "?solicitaracceso=true&force_login=true&cuentaComunidad=" + System.Net.WebUtility.UrlEncode(ProyectoSeleccionado.Clave.ToString());


                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mPaginaModel.PestanyasDeBusqueda = proyCN.ObtenerPestanyasProyectoNombre(ProyectoSeleccionado.Clave);
                    mPaginaModel.ProyectosConOntologias = proyCN.ObtenerProyectosConOntologiasAdministraUsuario(mControladorBase.UsuarioActual.UsuarioID);
                    proyCN.Dispose();
                }
                return mPaginaModel;
            }
        }

        #endregion
    }
}
