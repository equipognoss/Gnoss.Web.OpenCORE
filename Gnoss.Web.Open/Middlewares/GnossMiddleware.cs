using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC;
using Es.Riam.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gnoss.Web.Middlewares
{
    public class GnossMiddleware
    {
        private IHostingEnvironment mEnv;
        private readonly RequestDelegate _next;
        private ConfigService mConfigService;
        public static bool RecalculandoRutas { get; set; }
        
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        public GnossMiddleware(RequestDelegate next, IHostingEnvironment env, ConfigService configService, ILogger<GnossMiddleware> logger, ILoggerFactory loggerFactory)
        {
            _next = next;
            mEnv = env;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context, LoggingService loggingService, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, UtilTelemetry utilTelemetry, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, RouteConfig routeConfig, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            ControladorBase controladorBase = new ControladorBase(loggingService, mConfigService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);
            Application_BeginRequest(entityContext, context, routeConfig, loggingService, redisCacheWrapper);
            AddHeaders(context);
            await _next(context);
            Application_PostRequestHandlerExecute(context, servicesUtilVirtuosoAndReplication, gnossCache);
            Application_EndRequest(context, loggingService, controladorBase, utilTelemetry);
        }

        protected void Application_BeginRequest(EntityContext pEntityContext, HttpContext pHttpContextAccessor, RouteConfig pRouteConfig, LoggingService pLoggingService, RedisCacheWrapper pRedisCacheWrapper)
        {
            pLoggingService.AgregarEntrada("TiemposMVC_Application_BeginRequest_INICIO");
            pLoggingService.AgregarEntrada("TiemposMVC_Application_BeginRequest_INICIO_TRAZA_COMPROBADA");
        }

        protected void AddHeaders(HttpContext pHttpContextAccessor)
        {
            if (!pHttpContextAccessor.Response.Headers.ContainsKey("Content-Security-Policy") && !string.IsNullOrEmpty(mConfigService.GetConfigContentSecurityPolocy()))
            {
                pHttpContextAccessor.Response.Headers.Add("Content-Security-Policy", mConfigService.GetConfigContentSecurityPolocy());
            }
            if (!pHttpContextAccessor.Response.Headers.ContainsKey("Strict-Transport-Security"))
            {
                pHttpContextAccessor.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            }
        }

        protected void Application_PostRequestHandlerExecute(HttpContext pHttpContextAccessor, IServicesUtilVirtuosoAndReplication pServicesUtilVirtuosoAndReplication, GnossCache pGnossCache)
        {
            try
            {
                Guid? proyInvalidarCache = null;
                if (proyInvalidarCache.HasValue)
                {
                    pGnossCache.AgregarObjetoCache(GnossCacheCL.CLAVE_REFRESCO_CACHE_LOCAL + proyInvalidarCache.Value, Guid.NewGuid());
                }
            }
            catch
            {
                // Si no se puede guardar en caché no hace falta hacer nada
            }
        }

        protected void Application_EndRequest(HttpContext pHttpContextAccessor, LoggingService pLoggingService, ControladorBase pControladorBase, UtilTelemetry pUtilTelemetry)
        {
            try
            {
                string listaObjetosColaReplicacionFichero = null;//TODO Javier  (string)UtilPeticion.ObtenerObjetoDePeticion("ColaReplicacionFichero");
                pLoggingService.AgregarEntrada("TiemposMVC_Application_EndRequest");
                pLoggingService.GuardarTraza(ObtenerRutaTraza(pHttpContextAccessor, pControladorBase));
                if (!string.IsNullOrEmpty(listaObjetosColaReplicacionFichero))
                {
                    listaObjetosColaReplicacionFichero = listaObjetosColaReplicacionFichero.Substring(0, listaObjetosColaReplicacionFichero.Length - 1);

                    listaObjetosColaReplicacionFichero = "{ \"listaReplicacion\": [" + listaObjetosColaReplicacionFichero + "]}";

                    string ruta = Path.Combine(mEnv.WebRootPath, pHttpContextAccessor.Request.Path, "FicherosReplicacion");

                    if (!Directory.Exists(ruta))
                    {
                        Directory.CreateDirectory(ruta);
                    }

                    ruta += $"{Path.DirectorySeparatorChar}web_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.txt";

                    //Añado el error al fichero
                    using (StreamWriter sw = new StreamWriter(ruta, true, System.Text.Encoding.Default))
                    {
                        sw.Write(listaObjetosColaReplicacionFichero);
                    }
                }
            }
            catch (Exception) { }
        }

        protected string ObtenerRutaTraza(HttpContext pHttpContextAccessor, ControladorBase pControladorBase)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "trazas");

            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }

            ruta = Path.Combine(ruta, $"traza_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");

            return ruta;
        }

        public string DominoAplicacion(HttpContextAccessor pHttpContextAccessor)
        {

            string host = pHttpContextAccessor.HttpContext.Request.Host.ToString();

            if ((!host.ToLower().Trim().Equals("2003server")) && (!host.ToLower().Trim().Equals("localhost")))
            {
                if (host.StartsWith("www."))
                {
                    host = host.Remove(0, 3);
                }
                else if (!host.StartsWith("."))
                {
                    host = "." + host;
                }

                return host;
            }

            return null;

        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGnossMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GnossMiddleware>();
        }
        public static IApplicationBuilder UseErrorGnossMidleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorMiddleware>();
        }
    }

}