using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
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
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class AdministrarServiciosWebController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarServiciosWebController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarServiciosWebController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

		// GET: AdministrarServiciosWeb
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarServiciosExternos } })]
		public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            AdministrarServiciosWebViewModel model = new AdministrarServiciosWebViewModel();
            ObtenerServiciosWeb(model, ProyectoSeleccionado.Clave);

            return View(model);
        }

        private void ObtenerServiciosWeb (AdministrarServiciosWebViewModel pModel, Guid pProyectoID)
        {
            List<ProyectoServicioWeb> listaServicios = new List<ProyectoServicioWeb>();
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
            listaServicios = parametroCN.ObtenerProyectoServicioWeb(pProyectoID);
            pModel.ServiciosWeb = listaServicios;
        }

        /// <summary>
        /// Nueva Pestaña
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarServiciosExternos } })]
		public ActionResult NuevoServicio()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            ProyectoServicioWeb servicio = new ProyectoServicioWeb();
            servicio.Nombre = Guid.NewGuid().ToString();
            return PartialView("_NuevoServicio", servicio);
        }
        
        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarServiciosExternos } })]
		public ActionResult Guardar(List<ProyectoServicioWeb> ListaServicios)
        {
            GuardarLogAuditoria();
            //Es necesario comprobar todos los datos para insertarlos en la BBDD
            ActualizarBBDD(ListaServicios);


            return GnossResultOK();
        }

        public void ActualizarBBDD(List<ProyectoServicioWeb> pListaServicios)
        {
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
            parametroCN.GuardarFilasProyectoServicioWeb(pListaServicios, ProyectoSeleccionado.Clave);
        }
    }
}
