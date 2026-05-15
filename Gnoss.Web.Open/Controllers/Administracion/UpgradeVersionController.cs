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
using System;

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
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarTraducciones, (ulong)PermisoComunidad.GestionarPesosAutocompletado, (ulong)PermisoComunidad.AccederAlEstadoDeLosServicios, (ulong)PermisoComunidad.GestionarEventosExternos, (ulong)PermisoComunidad.AccederAEstadisticasDeLaComunidad, (ulong)PermisoComunidad.ConsultarCargasMasivas, (ulong)PermisoComunidad.GestionarIntegracionContinua, (ulong)PermisoContenidos.CrearFaceta, (ulong)PermisoComunidad.EjecutarReprocesadosDeRecursos, (ulong)PermisoComunidad.AccesoSparqlEndpoint, (ulong)PermisoComunidad.AccederAlFTP, (ulong)PermisoComunidad.GestionarCache, (ulong)PermisoComunidad.DescargarConfiguracionOAuth, (ulong)PermisoComunidad.GestionarTrazas, (ulong)PermisoComunidad.GestionarAsistentes, (ulong)PermisoComunidad.GestionarAplicacionesEspecificas } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarTraduccionesEcosistema, (ulong)PermisoEcosistema.GestionarEventosExternosEcosistema, (ulong)PermisoEcosistema.AdministrarIntegracionContinua } })]
        public IActionResult Index(string metodo)
        {
            EliminarPersonalizacionVistas();
            CargarSeccionViewBag(metodo);
            
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            return View();
        }
        [NonAction]
        private void CargarSeccionViewBag(string pMetodo)
        {
            switch (pMetodo)
            {

                case "ADMINISTRARTRADUCTORECOSISTEMA":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Traductor;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("COMADMINCOMUNIDAD", "ADMINISTRARTRADUCTOR");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    ViewBag.isInEcosistemaPlatform = "true";
                    break;
                case "ADMINISTRAREVENTOSEXTERNOSECOSISTEMA":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_EventosExternos;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
                    ViewBag.HeaderTitle = "Eventos Externos";
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    ViewBag.isInEcosistemaPlatform = "true";
                    break;
                case "ADMINISTRARDESCARGARCONFIGURACIONECOSISTEMA":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Descargar_Configuraciones;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACIONGENERAL");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "DESCARGACONFIGECOSISTEMA") ;
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    ViewBag.isInEcosistemaPlatform = "true";
                    break;
                case "ADMINISTRARCONFIGURACIONPESOBUSQUEDAS":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_PesosBusqueda;
                    break;
                case "ADMINISTRARESTADOSISTEMAS":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_EstadoSistemas;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACIONGENERAL");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTADODELOSSERVICIOS");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRAREVENTOSEXTERNOS":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_EventosExternos;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
                    ViewBag.HeaderTitle = "Eventos Externos";
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRARMATOMO":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Matomo;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTADISTICASDELACOMUNIDAD");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "CONSULTACARGAMASIVA":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_CargaMasiva;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "GRAFODECONOCIMIENTO");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "CONSULTACARGASMASIVAS");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRARINTEGRACIONCONTINUA":
                case "CONFIGURARINTEGRACIONCONTINUAREPOSITORIO":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.IntegracionContinua;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.IntegracionContinua_AdministrarIntegracionContinua;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "INTEGRACIONCONTINUA");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("COMADMINCOMUNIDAD", "ADMINISTRARINTEGRACIONCONTINUA");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "CONFIGURARINTEGRACIONCONTINUAINCIAL":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.IntegracionContinua;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.IntegracionContinua_Home;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "INTEGRACIONCONTINUA");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("COMADMINCOMUNIDAD", "ADMINISTRARINTEGRACIONCONTINUA");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "DIAGNOSTICOPROBLEMAS":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Ayuda;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Diagnostico_Problemas;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "AYUDAS");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "DIAGNOSTICOPROBLEMAS");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "REPROCESADORECURSOS":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Mantenimiento;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Mantenimiento_ReprocesadoRecursos;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "MANTENIMIENTO");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "REPROCESADORECURSOS");
                    break;
                case "TAREASFONDO":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Mantenimiento;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Mantenimiento_TareasFondo;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "MANTENIMIENTO");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "TAREASSEGUNDOPLANO");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRARAPLICACIONESESPECIFICAS":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_Paginas;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTRUCTURA");
                    break;
                case "ADMINISTRARCONSULTASPARQL":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_SparQL;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "GRAFODECONOCIMIENTO");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "SPARQL");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "CONFIGURAFTP":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_AccesoFTP;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ACCESOFTP");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRARCACHE":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Cache;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACIONGENERAL");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ADMINISTRARCACHE");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRARDESCARGARCONFIGURACION":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Descargar_Configuraciones;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACIONGENERAL");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "DESCARGACONFIG");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRARTRAZAS":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Trazas;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACIONGENERAL");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ADMINISTRACIONDETRAZAS");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRARTRADUCTOR":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Traductor;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("COMADMINCOMUNIDAD", "ADMINISTRARTRADUCTOR");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
                case "ADMINISTRARASISTENTES":
                    ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
                    ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_Asistentes;
                    ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "COMUNIDAD");
                    ViewBag.HeaderTitle = UtilIdiomas.GetText("COMADMINCOMUNIDAD", "ADMINISTRARASISTENTES");
                    ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
                    break;
            }

        }
    }
}
