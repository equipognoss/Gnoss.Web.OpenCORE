using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gnoss.Web.Middlewares
{
    public class GnossMiddleware
    {
        private IHostingEnvironment mEnv;
        private readonly RequestDelegate _next;
        private ConfigService mConfigService;
        public static bool RecalculandoRutas { get; set; }
        private static ConcurrentDictionary<string, Guid?> ProyectoIDPorNombreCorto = new ConcurrentDictionary<string, Guid?>();
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

        private void ComprobarTrazaHabilitada(Es.Riam.Gnoss.AD.EntityModel.EntityContext pEntityContext, LoggingService pLoggingService, RedisCacheWrapper pRedisCacheWrapper, HttpContext pHttpContext, bool pForzarComprobacion = false)
        {
            if (pForzarComprobacion || LoggingService.HoraComprobacionCache == null || LoggingService.HoraComprobacionCache.AddSeconds(LoggingService.TiempoDuracionComprobacion) < DateTime.Now)
            {
                GnossCacheCL gnossCacheCL = new GnossCacheCL(pEntityContext, pLoggingService, pRedisCacheWrapper, mConfigService, null, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
                bool? trazaHabilitada = gnossCacheCL.ObtenerDeCache($"traza_5.0.0_{pHttpContext.Request.Host}") as bool?;

                if (trazaHabilitada.HasValue && trazaHabilitada.Value)
                {
                    LoggingService.TrazaHabilitada = true;
                    //LoggingService.TiempoMinPeticion = (int)tiempoMin;
                }
                else
                {
                    LoggingService.TrazaHabilitada = false;
                }

                LoggingService.HoraComprobacionCache = DateTime.Now;
            }
        }
        private void RegistrarRutas(Es.Riam.Gnoss.AD.EntityModel.EntityContext pEntityContext, HttpContext pHttpContextAccessor, RouteConfig pRouteConfig, LoggingService pLoggingService, IServicesUtilVirtuosoAndReplication pServicesUtilVirtuosoAndReplication)
        {
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(pEntityContext, pLoggingService, null, mConfigService, pServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
            Dictionary<string, string> listaIdiomasPlataforma = paramCL.ObtenerListaIdiomasDictionary();
            int indiceComunidad = 2;
            string url = $"{pHttpContextAccessor.Request.Scheme}://{pHttpContextAccessor.Request.Host}{pHttpContextAccessor.Request.Path}";
            pRouteConfig.RegisterRoutesIdioma(RouteConfig.RouteBuilder, paramCL.ObtenerListaIdiomas());
            //if (new Uri(url).Segments.Length > 1 && new Uri(url).Segments[1].Length > 1)
            //{
            //    string idiomaPeticion = new Uri(url).Segments[1].Trim('/');

            //    if (listaIdiomasPlataforma.ContainsKey(idiomaPeticion))
            //    {
            //        pRouteConfig.RegisterRoutesIdioma(RouteConfig.RouteBuilder, idiomaPeticion);
            //        indiceComunidad = 3;
            //    }
            //}

            Dictionary<string, string> listaIdiomasCargados = new Dictionary<string, string>();


            foreach (string idioma in RouteConfig.IdiomasRegistrados)
            {
                listaIdiomasCargados.Add(idioma, listaIdiomasPlataforma[idioma]);
            }

            Guid? proyectoID = null;
            if (new Uri(url).Segments.Length > indiceComunidad && new Uri(url).Segments[indiceComunidad].Length > 1)
            {
                ProyectoCN proyCN = new ProyectoCN(pEntityContext, pLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                string nombreCortoComunidad = new Uri(url).Segments[indiceComunidad];

                if (ProyectoIDPorNombreCorto.ContainsKey(nombreCortoComunidad))
                {
                    proyectoID = ProyectoIDPorNombreCorto[nombreCortoComunidad];
                }
                else
                {

                    proyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(nombreCortoComunidad);
                    if (proyectoID.Value.Equals(Guid.Empty))
                    {
                        proyectoID = null;
                    }
                    ProyectoIDPorNombreCorto.TryAdd(nombreCortoComunidad, proyectoID);
                }

                if (!proyectoID.HasValue)
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(pEntityContext, pLoggingService, mConfigService, pServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    proyectoID = mConfigService.ObtenerProyectoConexion();
                    if (proyectoID == null || proyectoID.Equals(Guid.Empty))
                    {

                        string valor = paramCN.ObtenerParametroAplicacionSeaContenidoParametro(url).FirstOrDefault();

                        if (!string.IsNullOrEmpty(valor))
                        {
                            proyectoID = new Guid(valor);
                        }

                    }
                }
            }

            if (proyectoID.HasValue)
            {
                if (!RouteConfig.ListaProyectosRutas.Contains(proyectoID.Value))
                {
                    //pRouteConfig.RegistrarRutasPestanyas(RouteConfig.RouteBuilder, listaIdiomasCargados, proyectoID.Value);
                }
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
