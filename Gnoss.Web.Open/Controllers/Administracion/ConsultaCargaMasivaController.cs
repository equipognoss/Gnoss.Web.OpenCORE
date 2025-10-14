using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Carga;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
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
using System.Collections.Generic;
using System.Linq;


namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Consulta de las cargas masivas
    /// </summary>
    public class ConsultaCargaMasivaController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ConsultaCargaMasivaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<ConsultaCargaMasivaController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros

        private const int NUMERO_CARGAS_MOSTRADAS = 15;
        private ConsultaCargaMasivaViewModel mPaginaModel = null;
        private const short CERRADA = 1;
        private const short ABIERTA = 0;
        public const string CARGAABIERTA = "Abierta";
        public const string CARGACERRADA = "Cerrada en curso";
        public const string CARGACOMPLETADA = "Cerrada Completada";

        #endregion

        #region Métodos de evento
        /// <summary>
        /// Carga index de la pagina
        /// </summary>
        /// <returns>Actionresult</returns>
        [HttpGet]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.ConsultarCargasMasivas } })]
		public ActionResult Index()
        {

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "edicion no-max-width-container grafo-de-conocimiento edicionObjetos";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_CargaMasiva;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "GRAFODECONOCIMIENTO");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "CONSULTACARGASMASIVAS");

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            mPaginaModel = CargarModelo();
            return View(mPaginaModel);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Carga el modelo de datos
        /// </summary>
        /// <returns>Modelo de consulta de la carga masiva</returns>
        private ConsultaCargaMasivaViewModel CargarModelo()
        {
            ConsultaCargaMasivaViewModel consulta = new ConsultaCargaMasivaViewModel();
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            List<Carga> listaCargas = proyCN.ObtenerCargasMasivasPorIdentidadID(IdentidadActual.Clave);
            consulta.ListaInformacion = new List<InformacionMostrar>();

            foreach(Carga carga in listaCargas.Take(NUMERO_CARGAS_MOSTRADAS))
            {
                InformacionMostrar info = new InformacionMostrar();
                info.NombreCarga = carga.Nombre;
                info.FechaCarga = carga.FechaAlta.ToString();

                List<CargaPaquete> listaPaquetes = proyCN.ObtenerPaquetesPorIDCarga(carga.CargaID);
                

                List<CargaPaquete> paquetesProcesados = listaPaquetes.Where(x => x.Estado == (short)EstadoPaquete.Correcto).ToList();
                info.NumParquetesProcesados = paquetesProcesados.Count;

                info.IDCarga = carga.CargaID.ToString();

                if (carga.Estado == ABIERTA)
                {
                    info.EstadoCarga = CARGAABIERTA;
                    info.NumPaquetes = -1;
                }
                else if (carga.Estado == CERRADA)
                {
                    info.NumPaquetes = listaPaquetes.Count;
                    info.EstadoCarga = CARGACERRADA;
                    if (info.NumPaquetes == info.NumParquetesProcesados)
                    {
                        info.EstadoCarga = CARGACOMPLETADA;
                    }
                }

                if (listaPaquetes.Any(x => !string.IsNullOrEmpty(x.Error)))
                {
                    info.EstadoCarga = "ERROR en la carga";
                    List<CargaPaquete> listaPaquetesConError = listaPaquetes.Where(x => !string.IsNullOrEmpty(x.Error)).ToList();
                    info.NumPaquetesConError = listaPaquetesConError.Count;
                    info.ErrorCarga = $"Carga con error: {listaPaquetesConError.FirstOrDefault().Error}";
                }

                consulta.ListaInformacion.Add(info);
            }

            return consulta;
        }

        #endregion
    }
} 