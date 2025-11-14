using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gnoss.Web.Open.Controllers.Administracion
{
    public class UpgradeVersionController : ControllerAdministrationWeb
    {
        private UtilIdiomas mUtilIdiomas;
        public UpgradeVersionController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<ControllerAdministrationWeb> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
        }

        /// <summary>
		/// Muestra la página de subir a premium
		/// </summary>
		/// <returns>ActionResult</returns>
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarPesosAutocompletado, (ulong)PermisoComunidad.AccederAlEstadoDeLosServicios, (ulong)PermisoComunidad.GestionarEventosExternos, (ulong)PermisoComunidad.AccederAEstadisticasDeLaComunidad, (ulong)PermisoComunidad.ConsultarCargasMasivas, (ulong)PermisoComunidad.GestionarIntegracionContinua, (ulong)PermisoContenidos.CrearFaceta, (ulong)PermisoComunidad.EjecutarReprocesadosDeRecursos, (ulong)PermisoComunidad.EjecutarReprocesadosDeRecursos, (ulong)PermisoEcosistema.AdministrarIntegracionContinua, (ulong)PermisoComunidad.AccesoSparqlEndpoint, (ulong)PermisoComunidad.AccederAlFTP, (ulong)PermisoComunidad.GestionarCache, (ulong)PermisoComunidad.DescargarConfiguracionOAuth, (ulong)PermisoComunidad.GestionarTrazas } })]
        public IActionResult Index()
        {
            EliminarPersonalizacionVistas();
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_PesosBusqueda;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View();
        }
    }
}
