using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    /// <summary>
    /// Modelo de la pagina de logout
    /// </summary>
    public partial class LogoutModel
    {
        public string UrlLogo { get; set; }
        public string NombreComunidad { get; set; }
        public bool ShowPowered { get; set; }

        public string UrlIframe { get; set; }
        public string UrlRedirect { get; set; }
    }

    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class LogoutController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        public LogoutController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<LogoutController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Obtiene o establece si el usuario invitado puede ver esta página
        /// </summary>
        public override bool PaginaVisibleEnPrivada
        {
            get
            {
                return true;
            }
        }

        public ActionResult Index()
        {
            HttpContext.Response.Headers.Add("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");

            //Elimino del servicio de login la cookie actual para que no vuelva a loguearle

            string urlLogo = "";
            string nombre = "GNOSS";
            bool showPowered = false;

            if (EsGnossOrganiza)
            {
                urlLogo = $"/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesProyectos}/{mControladorBase.ProyectoConexion.ToString().ToLower()}.png";
            }

            Guid proyectoid = ProyectoAD.MetaProyecto;

            if (!ProyectoPrincipalUnico.Equals(ProyectoAD.MetaProyecto))
            {
                proyectoid = ProyectoPrincipalUnico;
            }
            else if (!mControladorBase.ProyectoPrincipal.Equals(ProyectoAD.MetaProyecto))
            {
                proyectoid = mControladorBase.ProyectoPrincipal;
            }
            else if (ProyectoConexionID.HasValue && !ProyectoConexionID.Equals(ProyectoAD.MetaProyecto))
            {
                proyectoid = ProyectoConexionID.Value;
            }

            if (!proyectoid.Equals(ProyectoAD.MetaProyecto))
            {
                ParametroGeneral filaParametroGeneral = mControladorBase.ObtenerFilaParametrosGeneralesDeProyecto(proyectoid).ListaParametroGeneral[0];

                if (filaParametroGeneral.VersionFotoImagenSupGrande != null)
                {
                    urlLogo = $"/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesProyectos}/{proyectoid.ToString().ToLower()}.png?v={filaParametroGeneral.VersionFotoImagenSupGrande}";
                    showPowered = true;
                }

                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                nombre = proyCL.ObtenerNombreDeProyectoID(proyectoid);
            }

            GnossIdentity usuario = Session.Get<GnossIdentity>("Usuario");
            if (usuario != null && !usuario.EsUsuarioInvitado)
            {
                IdentidadCL identCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                identCL.EliminarCacheGestorTodasIdentidadesUsuario(usuario.UsuarioID, usuario.PersonaID);
                identCL.EliminarCacheIdentidadActualUsuario(usuario.UsuarioID);
            }

            string query = $"?dominio={BaseURL}&eliminar=true&proyecto={ProyectoSeleccionado.Clave}&idioma={IdiomaUsuario}";

            ParametroAplicacion busqueda = ParametrosAplicacionDS.Find(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.LoginUnicoPorUsuario));
            bool loginUnicoPorUsuario = busqueda != null && busqueda.Valor.Equals("1");

            if (loginUnicoPorUsuario && usuario != null)
            {
                mGnossCache.InvalidarDeCache($"{ControladorBase.SESION_UNICA_POR_USUARIO}_List_{usuario.UsuarioID}", true);
            }

            Session.Clear();

            //Elimino las cookies
            mControladorBase.ExpirarCookies(ExisteNombrePoliticaCookiesMetaproyecto, NombrePoliticaCookiesMetaproyecto, usuario);

            LogoutModel paginaModel = new LogoutModel();
            paginaModel.UrlLogo = urlLogo;
            paginaModel.NombreComunidad = nombre;
            paginaModel.ShowPowered = showPowered;

            paginaModel.UrlIframe = $"{mControladorBase.UrlServicioLogin}/eliminarCookie{query}";
            paginaModel.UrlRedirect = UrlRedireccion;

            return View(paginaModel);
        }


        public ActionResult EliminarSesion()
        {
            //Elimino del servicio de login la cookie actual para que no vuelva a loguearle
            if (Request.Query.ContainsKey("token"))
            {
                Guid usuarioIDToken = Guid.Parse(Session.Get(Request.Query["token"]).ToString());

                ParametroAplicacion busqueda = ParametrosAplicacionDS.Find(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.LoginUnicoPorUsuario));

                bool loginUnicoPorUsuario = busqueda != null && busqueda.Valor.Equals("1");

                if (loginUnicoPorUsuario)
                {
                    mGnossCache.InvalidarDeCache($"{ControladorBase.SESION_UNICA_POR_USUARIO}_List_{usuarioIDToken}", true);
                }

                Session.Clear();
            }
            string url = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoConexion)}/{UtilIdiomas.GetText("URLSEM", "LOGIN")}?eliminarsesion=true";
            return Redirect(url);
        }

        #region Propiedades

        private string UrlRedireccion
        {
            get
            {
                string url = "";

                if (!mControladorBase.ProyectoPrincipal.Equals(ProyectoAD.MetaProyecto))
                {
                    url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoPrincipal);
                }
                else if (ProyectoConexionID.HasValue && !ProyectoConexionID.Value.Equals(ProyectoAD.MetaProyecto))
                {
                    url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoConexion);
                }
                else
                {
                    if (string.IsNullOrEmpty(RequestParams("redirect")))
                    {
                        string urlPaginaPresentadion = mConfigService.ObtenerUrlPaginaPresentacion();
                        if (!string.IsNullOrEmpty(urlPaginaPresentadion))
                        {
                            url = ProyectoSeleccionado.UrlPropia(IdiomaUsuario);
                        }
                        else if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                        {
                            url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                        }
                        else
                        {
                            url = $"{BaseURLIdioma}/{UtilIdiomas.GetText("URLSEM", "HOME")}";
                        }
                    }
                    else
                    {
                        url = $"{BaseURLIdioma}/{RequestParams("redirect")}";
                    }
                }

                return url;
            }
        }

        public string DominoAplicacionIdioma
        {
            get
            {
                return mControladorBase.DominoAplicacionConHTTP + IdiomaUsuarioDistintoPorDefecto;
            }
        }

        #endregion
    }
}
