using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Seguridad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Gnoss.Web.Open.Filters
{
    public class LimitarPeticionesAdministracion : BaseActionFilterAttribute
    {
        private const int VENTANA_TIEMPO = 600;
        private const int MAX_PETICIONES = 50;
        private const int TIEMPO_BLOQUEADO = 20;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private RedisCacheWrapper mRedisCacheWrapper;
        private VirtuosoAD mVirtuosoAD;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        private int mMaximoPeticiones;
        private int mVentanaDeTiempo;
        private int mTiempoBloqueado;
        private ControladorBase mControladorBase;

        public LimitarPeticionesAdministracion(int pMaximoPeticiones, int pVentanaDeTiempo, int pTiempoBloqueado, LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ILogger<UsuarioLogueadoAttribute> logger, ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAD;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mMaximoPeticiones = pMaximoPeticiones < 1 ? MAX_PETICIONES : pMaximoPeticiones;
            mVentanaDeTiempo = pVentanaDeTiempo < 1 ? VENTANA_TIEMPO : pVentanaDeTiempo;
            mTiempoBloqueado = pTiempoBloqueado < 1 ? TIEMPO_BLOQUEADO : pTiempoBloqueado;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null, mLoggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);
        }

        protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
        {
            try
            {
                using (SeguridadCL seguridadCL = new SeguridadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null, mLoggerFactory.CreateLogger<SeguridadCL>(), mLoggerFactory))
                {
                    string ip = ObtenerIP(pFilterContext.HttpContext);

                    bool puedeRealizarPeticiones = seguridadCL.ComprobarSiSeSuperaLimiteDePeticionesAdministracion(mMaximoPeticiones, mVentanaDeTiempo, mTiempoBloqueado, ip, mControladorBase.IdentidadActual.Clave);
                    if (!puedeRealizarPeticiones)
                    {
                        pFilterContext.Result = new ContentResult
                        {
                            StatusCode = 429,
                            Content = """{"error": "Too Many Requests"}""",
                            ContentType = "application/json"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error al comprobar el limite de peticiones. Los límites configurados son: Ventana de tiempo -> {mVentanaDeTiempo}, Número máximo de peticiones -> {mMaximoPeticiones}, Tiempo de bloqueo -> {mTiempoBloqueado}", mlogger);
                pFilterContext.Result = new ContentResult
                {
                    StatusCode = 500,
                    Content = """{"error": "Internal Server Error"}""",
                    ContentType = "application/json"
                };
            }
        }

        private static string ObtenerIP(HttpContext pHttpContext)
        {
            string ip = pHttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
            {
                return ip.Split(',')[0].Trim();
            }

            ip = pHttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            return ip;
        }
    }
}
