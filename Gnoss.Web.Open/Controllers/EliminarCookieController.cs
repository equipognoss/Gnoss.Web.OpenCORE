using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
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
using Serilog.Core;
using System;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class EliminarCookieController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public EliminarCookieController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<EliminarCookieController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;
        }

        // GET: EliminarCookie
        public ActionResult Index()
        {
            GnossIdentity usuario = Session.Get<GnossIdentity>("Usuario");

            // Comprueba si se ha enviado esta petición para conectar al mismo usuario que ya estaba conectado 
            // Si es el usuario que se acaba de conectar es el mismo que ya estaba conectado, no elimino cookies
            Guid usuarioNuevoID;
            bool usuarioYaConectado = (usuario != null && !string.IsNullOrEmpty(Request.Headers["usuarioID"]) && Guid.TryParse(Request.Headers["usuarioID"], out usuarioNuevoID) && usuario.UsuarioID.Equals(usuarioNuevoID));
            
            if (!usuarioYaConectado)
            {
                //Cabeceras para poder recibir cookies de terceros
                HttpContext.Response.Headers.Add("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");

                //Elimino las cookies
                Session.Clear();

                //Elimino las cookies
                mControladorBase.ExpirarCookies(ExisteNombrePoliticaCookiesMetaproyecto, NombrePoliticaCookiesMetaproyecto, usuario);

                //Si el usuario esta conectandose, le creamos la cookie de logueado
                if (Request.Headers.ContainsKey("login"))
                {
                    mControladorBase.CrearCookieLogueado();
                }
            }

            if (Request.Headers.ContainsKey("redirect"))
            {
                string redirect = Request.Headers["redirect"];
                if (redirect.Contains("?") && redirect.ToLower().Contains("crearcookie"))
                {
                    //Para la página crearcookie.aspx, la query viene encriptada y hay que hacer un url encode
                    int indiceQuery = redirect.IndexOf('?') + 1;
                    redirect = redirect.Substring(0, indiceQuery) + System.Net.WebUtility.UrlEncode(redirect.Substring(indiceQuery, redirect.Length - indiceQuery));
                }
                return Redirect(redirect);
            }

            return new EmptyResult();
        }
    }
}