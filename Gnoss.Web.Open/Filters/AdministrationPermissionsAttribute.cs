using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace Gnoss.Web.Open.Filters
{
    /// <summary>
    /// Permisos necesarios para acceder a una acción de administración de comunidad o del ecosistema.
    /// El ámbito de la petición se obtiene únicamente de la ruta definida en el routemap, se controlan los parámetros enviados por el cliente.
    /// Si la acción no declara permisos para el ámbito de la petición, se deniega el acceso.
    /// </summary>
    public class AdministrationPermissionsAttribute : TypeFilterAttribute
    {
        public AdministrationPermissionsAttribute() : base(typeof(AdministrationPermissionsFilter))
        {
            Arguments = [this];
        }

        /// <summary>
        /// Permisos de administración de comunidad que dan acceso a la acción en el ámbito de comunidad
        /// </summary>
        public PermisoComunidad[] Community { get; set; } = [];

        /// <summary>
        /// Permisos de contenidos de comunidad que dan acceso a la acción en el ámbito de comunidad
        /// </summary>
        public PermisoContenidos[] Contents { get; set; } = [];

        /// <summary>
        /// Permisos de administración del ecosistema que dan acceso a la acción en el ámbito del ecosistema
        /// </summary>
        public PermisoEcosistema[] Ecosystem { get; set; } = [];

        private sealed class AdministrationPermissionsFilter : PermisosPaginasUsuariosAttribute
        {
            private const string EcosystemParameter = "ecosistema";

            private readonly AdministrationPermissionsAttribute mPermissions;

            public AdministrationPermissionsFilter(AdministrationPermissionsAttribute pPermissions, LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ILogger<AdministrationPermissionsFilter> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, logger, loggerFactory, false)
            {
                mPermissions = pPermissions;
            }

            protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
            {
                HttpRequest request = pFilterContext.HttpContext.Request;
                string? routeEcosystemValue = request.RouteValues[EcosystemParameter] as string;

                // Se controla que no se altere el ámbito definido por la ruta
                if (ClientChangesEcosystemValue(request, routeEcosystemValue))
                {
                    pFilterContext.Result = new RedirectResult($"/error?errorCode=404&lang={request.RouteValues["lang"]}");
                    return;
                }

                bool tienePermiso;
                if (bool.TryParse(routeEcosystemValue, out bool esEcosistema) && esEcosistema)
                {
                    tienePermiso = HasPermission(TipoDePermiso.Ecosistema, mPermissions.Ecosystem.Select(permiso => (ulong)permiso).ToArray());
                }
                else
                {
                    tienePermiso = HasPermission(TipoDePermiso.Comunidad, mPermissions.Community.Select(permiso => (ulong)permiso).ToArray()) || HasPermission(TipoDePermiso.Contenidos, mPermissions.Contents.Select(permiso => (ulong)permiso).ToArray());
                }

                if (!tienePermiso)
                {
                    RedirectWithoutPermission(pFilterContext);
                }
            }

            private static bool ClientChangesEcosystemValue(HttpRequest pRequest, string? pRouteEcosystemValue)
            {
                if (pRequest.Query.TryGetValue(EcosystemParameter, out StringValues queryValue) && !string.Equals(queryValue.ToString(), pRouteEcosystemValue, StringComparison.Ordinal))
                {
                    return true;
                }

                return pRequest.HasFormContentType && pRequest.Form.TryGetValue(EcosystemParameter, out StringValues formValue) && !string.Equals(formValue.ToString(), pRouteEcosystemValue, StringComparison.Ordinal);
            }
        }
    }
}
