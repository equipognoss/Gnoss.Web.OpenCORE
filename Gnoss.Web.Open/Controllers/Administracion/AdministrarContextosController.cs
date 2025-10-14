using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AdministrarContextosViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ListaIdiomas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IdiomaPorDefecto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool CMSDisponible { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AdministracionSemanticaPermitido { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ContextoModel> ListaGadgets { get; set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarContextosController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarContextosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarContextosController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarContextosViewModel mPaginaModel = null;

        private DataWrapperProyecto mDataWrapperProyecto = null;

        #endregion

        #region Metodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarInformacionContextual } })]
		public ActionResult Index()
        {

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "estructura edicion edicionPaginas no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.DescubrimientoAnalisis;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.DescubrimientoAnalisis_Informacion_Contextual;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "DESCUBRIMIENTOYANALISIS");            
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "INFORMACIONCONTEXTUAL");

            // Establecer en el ViewBag el idioma por defecto
            ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            return View(PaginaModel);
        }

        /// <summary>
        /// Nuevo Contexto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarInformacionContextual } })]
		public ActionResult NuevoGadget(TipoGadget TipoGadget)
        {
            EliminarPersonalizacionVistas();

            ContextoModel gadget = new ContextoModel();
            gadget.Key = Guid.NewGuid();
            gadget.Name = UtilIdiomas.GetText("COMADMINCONTEXTOS", "NUEVOGADGET", TipoGadget.ToString());
            gadget.Clases = "";
            gadget.Visible = false;
            gadget.TipoGadget = TipoGadget;
            gadget.FiltrosDestino = "";

            if (TipoGadget.Equals(TipoGadget.RecursosContextos))
            {
                ContextoModel.ContextModel contexto = new ContextoModel.ContextModel();

                contexto.ShortName = "";
                contexto.ComunidadOrigen = "";
                contexto.FiltrosOrigen = "";
                contexto.RelacionOrigenDestino = "";
                contexto.NumResultados = 5;
                contexto.OrdenResultados = "";
                contexto.Imagen = 1;
                contexto.MostrarEnlaceOriginal = false;
                contexto.MostrarVerMas = true;
                contexto.AbrirEnPestanyaNueva = false;
                contexto.NamespacesExtra = "";
                contexto.ResultadosExcluir = "";

                gadget.Contexto = contexto;
            }
            else if (TipoGadget.Equals(TipoGadget.CMS))
            {
                CargarListaComponentesCMS();
                gadget.Contenido = Guid.Empty.ToString();
            }

            return PartialView("_EdicionContexto", gadget);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarInformacionContextual } })]
		[TypeFilter(typeof(AccesoIntegracionAttribute))]
		public ActionResult Guardar(List<ContextoModel> ListaGadgets)
        {
            GuardarLogAuditoria();
            bool iniciado = false;
            try
            {
                iniciado = HayIntegracionContinua;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                return GnossResultERROR("Contacte con el administrador del Proyecto, no es posible atender la petición.");
            }

            ControladorContextos contrContex = new ControladorContextos(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorContextos>(), mLoggerFactory);
            
            string errores = contrContex.ComprobarErrores(ListaGadgets);
            if (string.IsNullOrEmpty(errores))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    contrContex.GuardarGadgets(ListaGadgets);

                    if (iniciado)
                    {
                        foreach (ContextoModel contexto in ListaGadgets)
                        {
                            if (contexto.Contenido == null)
                            {
                                contexto.Contenido = "";
                            }

                            if (contexto.Clases == null)
                            {
                                contexto.Clases = "";
                            }

                            if (contexto.TipoGadget == TipoGadget.RecursosContextos)
                            {
                                contexto.Ajax = true;
                            }
                        }

                        HttpResponseMessage resultado = InformarCambioAdministracion("Gadgets", JsonConvert.SerializeObject(ListaGadgets, Formatting.Indented));

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch (Exception ex)
                {
                    if (transaccionIniciada)
                    {
                        proyAD.TerminarTransaccion(false);
                    }

                    return GnossResultERROR(ex.Message);
                }

                contrContex.CrearFilasPropiedadesIntegracionContinua(ListaGadgets);

                if (EntornoActualEsPruebas && iniciado)
                {
                    //contrContex.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaGadgets, UrlApiDesplieguesEntornoSiguiente);
                    contrContex.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaGadgets, UrlApiEntornoSeleccionado("pre"), UsuarioActual.UsuarioID);
                    contrContex.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaGadgets, UrlApiEntornoSeleccionado("pro"), UsuarioActual.UsuarioID);
                }

                contrContex.InvalidarCaches();


                return GnossResultOK();
            }
            else
            {
                return GnossResultERROR(errores);
            }
        }



        #endregion

        private void CargarListaComponentesCMS()
        {
            ViewBag.ListaComponentesCMS = new Dictionary<Guid, string>();

            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
            DataWrapperCMS cmsDW = cmsCN.ObtenerComponentesCMSDeProyecto(ProyectoSeleccionado.Clave);
            cmsCN.Dispose();

            foreach (AD.EntityModel.Models.CMS.CMSComponente filaComponente in cmsDW.ListaCMSComponente)
            {
                if (!ViewBag.ListaComponentesCMS.ContainsKey(filaComponente.ComponenteID))
                {
                    ViewBag.ListaComponentesCMS.Add(filaComponente.ComponenteID, filaComponente.Nombre);
                }
            }
        }

        #region Propiedades

        private DataWrapperProyecto DataWrapperProyecto
        {
            get
            {
                if (mDataWrapperProyecto == null)
                {
                    mDataWrapperProyecto = new DataWrapperProyecto();
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    proyCN.ObtenerGadgetsProyecto(ProyectoSeleccionado.Clave, mDataWrapperProyecto);
                }
                return mDataWrapperProyecto;
            }
        }

        private AdministrarContextosViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarContextosViewModel();

					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

					mPaginaModel.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();

                    mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;

                    mPaginaModel.CMSDisponible = ParametrosGeneralesRow.CMSDisponible;

                    mPaginaModel.AdministracionSemanticaPermitido = ParametroProyecto.ContainsKey("AdministracionSemanticaPermitido") && ParametroProyecto["AdministracionSemanticaPermitido"] == "1";

                    if (mPaginaModel.CMSDisponible)
                    {
                        CargarListaComponentesCMS();

                        if (((Dictionary<Guid, string>)ViewBag.ListaComponentesCMS).Count == 0)
                        {
                            mPaginaModel.CMSDisponible = false;
                        }
                    }

                    mPaginaModel.ListaGadgets = new List<ContextoModel>();

                    ControladorContextos contrContex = new ControladorContextos(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorContextos>(), mLoggerFactory);

                    foreach (ProyectoGadget filaGadget in DataWrapperProyecto.ListaProyectoGadget.Where(gadget => gadget.TipoUbicacion == (short)TipoUbicacionGadget.FichaRecursoComunidad))
                    {
                        ContextoModel gadget = contrContex.CargarGadget(filaGadget);

                        mPaginaModel.ListaGadgets.Add(gadget);
                    }
                }
                return mPaginaModel;
            }
        }

        #endregion
    }
}
