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
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarPesosAutocompletado } })]
        public IActionResult IndexAdministrarPesos()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOADMINISTRARPESOS");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_PesosBusqueda;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.AccederAlEstadoDeLosServicios } })]
        public IActionResult IndexAdministrarEstadoSistemas()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOESTADOSISTEMAS");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_EstadoSistemas;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarEventosExternos } })]
        public IActionResult IndexAdministrarEventosExternos()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOEVENTOSEXTERNOS");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_EventosExternos;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.AccederAEstadisticasDeLaComunidad } })]
        public IActionResult IndexAdministrarEstadisticasComunidad()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOESTADISITICASCOMUNIDAD");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Matomo;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.ConsultarCargasMasivas } })]
        public IActionResult IndexConsultaCargaMasiva()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOCARGAMASIVA");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_CargaMasiva;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarIntegracionContinua } })]
        public IActionResult IndexAdministrarIntegracionContinua()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOINEGRACIONCONTINUA");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.IntegracionContinua_AdministrarIntegracionContinua;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.IntegracionContinua;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.CrearFaceta} })]
        public IActionResult IndexDiagnosticoProblemas()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIODIAGNOSTICODEPROBLEMAS");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Diagnostico_Problemas;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Ayuda;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.EjecutarReprocesadosDeRecursos } })]
        public IActionResult IndexReprocesadoRecursos()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOREPROCESADORECURSOS");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Mantenimiento_ReprocesadoRecursos;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Mantenimiento;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.EjecutarReprocesadosDeRecursos } })]
        public IActionResult IndexMonitorizarTareas()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOMONITORIZACIONDETAREAS");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Mantenimiento_TareasFondo;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Mantenimiento;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.AdministrarIntegracionContinua } })]
        public IActionResult IndexAdministrarApliacionesEspecificas()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOAPLICACIONESESPECIFICAS");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.IntegracionContinua_DesplegarWeb;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.IntegracionContinua;

            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.AccesoSparqlEndpoint } })]
        public IActionResult IndexAccesoSparql()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOCONSULTASSPARQL");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_SparQL;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.AccederAlFTP } })]
        public IActionResult IndexFTP()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOFTP");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_AccesoFTP;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarCache } })]
        public IActionResult IndexAdministracionCache()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOCACHE");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Cache;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.DescargarConfiguracionOAuth } })]
        public IActionResult IndexDescargaConfiguraciones()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIODESCARGACONFIGURACIONES");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Descargar_Configuraciones;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        /// <summary>
        /// Muestra la página de subir a premium
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarTrazas } })]
        public IActionResult IndexTrazas()
        {
            string textoServicio = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOTRAZAS");
            string textoMostrar = ObtenerTextoEnterprise(textoServicio);
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Trazas;
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View("Index", textoMostrar);
        }

        private UtilIdiomas UtilIdiomas
        {
            get
            {
                if (mUtilIdiomas == null)
                {
                    mUtilIdiomas = new UtilIdiomas(IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);
                }
                return mUtilIdiomas;
            } 
        }

        [NonAction]
        private string ObtenerTextoEnterprise(string pTextoServicio)
        {
            string textoEnterprise = UtilIdiomas.GetText("DEVTOOLS", "ADQUIRIRVERSIONENTERPRISE");
            string textoMostrar = textoEnterprise.Replace("[SERVICIO]", pTextoServicio);
            return textoMostrar;
        }
    }
}
