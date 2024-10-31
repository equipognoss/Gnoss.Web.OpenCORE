using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class AdministrarServiciosWebController : ControllerBaseWeb
    {
        public AdministrarServiciosWebController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        // GET: AdministrarServiciosWeb
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
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
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            listaServicios = parametroCN.ObtenerProyectoServicioWeb(pProyectoID);
            pModel.ServiciosWeb = listaServicios;
        }

        /// <summary>
        /// Nueva Pestaña
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
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
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult Guardar(List<ProyectoServicioWeb> ListaServicios)
        {
            GuardarLogAuditoria();
            //Es necesario comprobar todos los datos para insertarlos en la BBDD
            ActualizarBBDD(ListaServicios);


            return GnossResultOK();
        }

        public void ActualizarBBDD(List<ProyectoServicioWeb> pListaServicios)
        {
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            parametroCN.GuardarFilasProyectoServicioWeb(pListaServicios, ProyectoSeleccionado.Clave);
        }
    }
}
