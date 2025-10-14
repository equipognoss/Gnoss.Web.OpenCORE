using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    /// <summary>
    /// Modelo de la pagina de logout
    /// </summary>
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public partial class LoadingModel
    {
        public string UrlLogo { get; set; }
        public string NombreComunidad { get; set; }
        public bool ShowPowered { get; set; }

        public string UrlRedireccion { get; set; }
    }

    public class LoadingController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public LoadingController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<LoadingController> logger, ILoggerFactory loggerFactory)
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
            if (Request.Scheme.Equals("https") && 
                (Request.Headers.ContainsKey("Referer") == false|| !Request.Headers["Referer"].Equals(Request.Path.ToString().Replace("https://", "http://")))) // Compruebo que no se está en un bucle de redireccionamiento
            {
                // Esta página está pensada para eliminar las cookies de los sitios a los que has entrado sin HTTPS
                // No tiene sentido cargarla con https porque el iframe fallaría
                return Redirect(Request.Path.ToString().Replace("https://", "http://"));
            }

            HttpContext.Response.Headers.Add("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");

            //Elimino del servicio de login la cookie actual para que no vuelva a loguearle

            string urlLogo = "";
            string nombre = "GNOSS";
            bool showPowered = false;

            if (EsGnossOrganiza)
            {
                urlLogo = "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesProyectos + "/" + mControladorBase.ProyectoConexion.ToString().ToLower() + ".png";
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
                //ParametroGeneralDS.ParametroGeneralRow filaParametroGeneral = UtilUsuario.ObtenerFilaParametrosGeneralesDeProyecto(proyectoid);
                ParametroGeneral filaParametroGeneral = mControladorBase.ObtenerFilaParametrosGeneralesDeProyecto(proyectoid).ListaParametroGeneral[0];

                if (!(filaParametroGeneral.VersionFotoImagenSupGrande==null))
                {
                    urlLogo = "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesProyectos + "/" + proyectoid.ToString().ToLower() + ".png?v=" + filaParametroGeneral.VersionFotoImagenSupGrande;
                    showPowered = true;
                }

                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                nombre = proyCL.ObtenerNombreDeProyectoID(proyectoid);
            }

            string query = string.Format("?dominio={0}/", BaseURL);

            if ((HttpContext.Session.Get("Usuario") != null) && (!HttpContext.Session.Get<GnossIdentity>("Usuario").EsUsuarioInvitado))
            {
                query = string.Format("?usuarioID={0}&{1}", HttpContext.Session.Get<GnossIdentity>("Usuario").UsuarioID, query.Substring(1));
            }

            Response.WriteAsync("<IFRAME style='WIDTH:1px;HEIGHT:1px' src='" + mControladorBase.UrlServicioLogin.Replace("https://", "http://") + "/eliminarCookie" + query + "' frameBorder='0' onload='document.location=\"" + UrlRedireccion + "\"'></IFRAME>");

            LoadingModel paginaModel = new LoadingModel();
            paginaModel.UrlLogo = urlLogo;
            paginaModel.NombreComunidad = nombre;
            paginaModel.ShowPowered = showPowered;
            paginaModel.UrlRedireccion = UrlRedireccion;

            return View(paginaModel);
        }

        #region Propiedades


        private string UrlRedireccion
        {
            get
            {
                string url = "";

                if (!string.IsNullOrEmpty(RequestParams("redirect")))
                {
                    url = HttpUtility.UrlDecode(RequestParams("redirect"));
                }
                else if (!mControladorBase.ProyectoPrincipal.Equals(ProyectoAD.MetaProyecto))
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    url = proyCL.ObtenerURLPropiaProyecto(mControladorBase.ProyectoPrincipal, IdiomaUsuario);
                }
                else if (ProyectoConexionID.HasValue && !ProyectoConexionID.Value.Equals(ProyectoAD.MetaProyecto))
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    url = proyCL.ObtenerURLPropiaProyecto(ProyectoConexionID.Value, IdiomaUsuario);
                }

                if (string.IsNullOrEmpty(url))
                {
                    url = $"{Request.Scheme}://{Request.Host}";
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
