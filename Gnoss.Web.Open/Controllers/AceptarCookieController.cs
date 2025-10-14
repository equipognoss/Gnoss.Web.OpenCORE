using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Web.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class AceptarCookieController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AceptarCookieController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AceptarCookieController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        [HttpPost]
        public ActionResult Index()
        {
            string nombreCookie = string.Empty;

            if (ParametroProyecto != null && ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies) && !string.IsNullOrEmpty(ParametroProyecto[ParametroAD.NombrePoliticaCookies]))
            {
                nombreCookie = ParametroProyecto[ParametroAD.NombrePoliticaCookies] + mControladorBase.DominoAplicacion;
            }
            else if (ExisteNombrePoliticaCookiesMetaproyecto)
            {
                nombreCookie = NombrePoliticaCookiesMetaproyecto + mControladorBase.DominoAplicacion;
            }
            else
            {
                nombreCookie = "cookieAviso" + mControladorBase.DominoAplicacion;
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            try
            {
                value = UtilCookies.FromLegacyCookieString(Request.Cookies[nombreCookie], mEntityContext);
            }
            catch
            {
                Response.Cookies.Delete(nombreCookie);
            }

            if (value.ContainsKey("aceptada"))
            {
                value["aceptada"] = "true";
            }
            else
            {
                value.Add("aceptada", "true");
            }

            
            // Opciones cookie para evitar problemas en Safari (iOS) y al depurar
            CookieOptions cookieOptions = new CookieOptions { Expires = DateTime.Now.AddYears(1)};
            if (!mControladorBase.DominoAplicacion.Contains("depuracion.net") && mConfigService.PeticionHttps()) {
                cookieOptions.HttpOnly = true;
                cookieOptions.Secure = true;
            }            

            Response.Cookies.Append(nombreCookie, UtilCookies.ToLegacyCookieString(value, mEntityContext), cookieOptions);

            foreach(PersonalizacionCategoriaCookieModel cookie in ListaPersonalizacionCategoriaCookieModel)
            {
                string nombreCookieLista = UrlEncode(cookie.CategoriaCookie.NombreCorto.ToLower());
                if (!Request.Cookies.ContainsKey(nombreCookieLista))
                {
                    Response.Cookies.Append(nombreCookieLista, "yes", cookieOptions);
                }
            }

            return new EmptyResult();
        }
    }
}