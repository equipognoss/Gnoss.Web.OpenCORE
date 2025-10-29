using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Gnoss.Web.Open.Controllers.Administracion
{
    public class AdministrarFlujosController : ControllerAdministrationWeb
    {
        #region Miembros

        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        private AdministrarFlujosViewModel mPaginaModel = null;

        #endregion

        #region Constructores

        public AdministrarFlujosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarFlujosController> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos Web

        [HttpGet]
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarFlujos } })]
        public IActionResult Index()
        {
            CargarPermisosAdministracionComunidadEnViewBag();
            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "estructura edicion edicionPaginas listado no-max-width-container workflows";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_Flujos;

            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTRUCTURA");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("COMADMINCOMUNIDAD", "ADMINISTRARFLUJOS");

            return View(PaginaModel);
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarFlujos } })]
        public ActionResult EditarFlujo(Guid pFlujoID)
        {
            try
            {
                ControladorFlujos controladorFlujos = new ControladorFlujos(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorFlujos>(), mLoggerFactory);
                FlujoViewModel modelo = controladorFlujos.CargarFlujo(pFlujoID);
                return PartialView("_modal-views/_edit-flujo", modelo);
            }         
            catch(Exception ex){
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR(UtilIdiomas.GetText("DEVTOOLS", "ERRORCARGARFORMULARIO"));
            }
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarFlujos } })]
        public ActionResult GuardarFlujo(FlujoViewModel pModelo)
        {
            try
            {
                ControladorFlujos controladorFlujos = new ControladorFlujos(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorFlujos>(), mLoggerFactory);

                string error = controladorFlujos.ComprobarErrores(pModelo);

                if (!string.IsNullOrEmpty(error))
                {
                    string errorFormateado = "";
                    if (error.Contains("ERRORFLUJOEXISTENTESOBRETIPO"))
                    {
                        string mensaje = error.Split(":")[0];
                        string tipo = error.Split(":")[1];
                        if (tipo.Contains("@"))
                        {
                            tipo = UtilCadenas.ObtenerTextoDeIdioma(tipo, IdiomaPorDefecto, null);
                        }
                        else
                        {
                            tipo = UtilIdiomas.GetText("DEVTOOLS", tipo);
                        }
                        errorFormateado = UtilIdiomas.GetText("DEVTOOLS", mensaje, tipo);
                    }
                    else
                    {
                        errorFormateado = UtilIdiomas.GetText("DEVTOOLS", error);
                    }

                    return GnossResultERROR(errorFormateado);
                }

                controladorFlujos.GuardarFlujo(pModelo);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR("Ha surgido un error durante el guardado de flujo");
            }

            return GnossResultOK();
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarFlujos } })]
        public ActionResult EliminarFlujo(Guid pFlujoID)
        {
            try
            {
                ControladorFlujos controladorFlujos = new ControladorFlujos(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorFlujos>(), mLoggerFactory);
                controladorFlujos.EliminarFlujo(pFlujoID);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR("");
            }
            return GnossResultOK();
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarFlujos } })]
        public ActionResult VerFlujo(Guid pFlujoID)
        {
            ControladorFlujos controladorFlujos = new ControladorFlujos(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorFlujos>(), mLoggerFactory);
            
            FlujoViewModel modelo = controladorFlujos.CargarFlujo(pFlujoID);
            return PartialView("_partial-views/_flujo-chart", modelo);
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarFlujos } })]
        public ActionResult EliminarEstado(Guid pEstadoID, Guid pFlujoID)
        {
            string error = "";
            try
            {
                FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
                error = flujosCN.PuedoEliminarEstado(pEstadoID, pFlujoID);
                if (!string.IsNullOrEmpty(error))
                {
                    string errorFormateado = UtilIdiomas.GetText("DEVTOOLS", error);
                    if (error.Contains("AFECTA"))
                    {
                        errorFormateado = UtilIdiomas.GetText("DEVTOOLS", error.Split(":")[0], UtilIdiomas.GetText("DEVTOOLS", error.Split(":")[1]));
                    }
                    return GnossResultERROR(errorFormateado);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR("");
            }
            return GnossResultOK("");
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarFlujos } })]
        public ActionResult EliminarTransicion(Guid pTransicionID, Guid pFlujoID)
        {
            string error = "";
            try
            {
                FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
                error = flujosCN.PuedoEliminarTransicion(pTransicionID, pFlujoID);
                if (!string.IsNullOrEmpty(error))
                {
                    string nombreError = error.Split(":")[0];
                    string tipoRecurso = error.Split(":")[1];
                    if (tipoRecurso.Contains("Recurso"))
                    {
                        tipoRecurso = string.Join(", ", flujosCN.ObtenerOntologiasNombreFlujo(pFlujoID));
                    }
                    return GnossResultERROR(UtilIdiomas.GetText("DEVTOOLS", nombreError, tipoRecurso));
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR();
            }
            return GnossResultOK();
        }
        #endregion

        #region Metodos privados

        private AdministrarFlujosViewModel CargarModelo()
        {
            AdministrarFlujosViewModel modelo = new AdministrarFlujosViewModel();
            modelo.IdiomaPorDefecto = IdiomaPorDefecto;
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

            modelo.Idiomas = paramCL.ObtenerListaIdiomasDictionary();

            if (!ParametrosGeneralesRow.IdiomasDisponibles)
            {
                modelo.Idiomas = paramCL.ObtenerListaIdiomasDictionary().Where(i => i.Key.Equals(IdiomaPorDefecto)).ToDictionary();
            }

            ControladorFlujos controladorFlujos = new ControladorFlujos(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorFlujos>(), mLoggerFactory);
            modelo.ListaFlujos = controladorFlujos.CargarFlujosProyecto();

            DataWrapperDocumentacion dwDocumentacion = new DataWrapperDocumentacion();
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            docCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dwDocumentacion, true, false, false);
            ViewBag.DiccionarioOntologias =  dwDocumentacion.ListaDocumento.ToDictionary(k => k.DocumentoID, k => k.Enlace.Replace(".owl",""));

            return modelo;
        }

        #endregion

        #region Propiedades

        private AdministrarFlujosViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = CargarModelo();
                }
                return mPaginaModel;
            }
        }

        #endregion
    }
}
