using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para adminstrar los servicios externos de la plataforma
    /// </summary>
    public class AdministrarServiciosExternosController : ControllerBaseWeb
    {
        public AdministrarServiciosExternosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        private AdministrarServiciosExternosViewModel mPaginaModel = null;

        #endregion

        #region Métodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "configuracion edicionServiciosExternos edicion no-max-width-container ";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_ServiciosExternos;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "SERVICIOSEXTERNOS");

            mPaginaModel = CargarModelo();
            return View(mPaginaModel);           
        }


        /// <summary>
        /// Método para solicitar una nueva "row" para crear un nuevo servicio externo. Creará una nueva fila para que vía JS una vez se haya creado se pulse en "Editar"
        /// para añadir las características
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult CargarNuevoItem()
        {           
            ServiceNameModel serviceName = new ServiceNameModel();
            serviceName.Deleted = false;
            serviceName.Nueva = true;
            
            return PartialView("_partial-views/_list-item-external-service", serviceName);
        }



        /// <summary>
        /// Guardar la Url
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult GuardarUrl(AdministrarServiciosExternosViewModel pOptions)
        {
            try
            {
                string serviceName = pOptions.ServiceName;
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                if (serviceName.Contains("{ServiceName}"))
                {
                    proyCN.GuardarParamentroAplicacion("UrlBaseService", serviceName);
                }
                else
                {
                    throw new Exception("No se ha podido guardar la Url, falta {ServiceName}");
                }
                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }
            
        }
        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(List<ServiceNameModel> listaPestanyas)
        {
            try
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                foreach(ServiceNameModel pestanya in listaPestanyas)
                {
                    if (pestanya.Nueva && !pestanya.Deleted)
                    {
                        if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                        {
                            EcosistemaServicioExterno eco = new EcosistemaServicioExterno();
                            eco.NombreServicio = pestanya.NombreServicio;
                            eco.UrlServicio = pestanya.UrlServicio;
                            proyCN.ActualizarServiceNameEcosistema(eco);
                        }
                        else
                        {
                            ProyectoServicioExterno proy = new ProyectoServicioExterno();
                            proy.OrganizacionID = proyCN.ObtenerOrganizacionIDAPartirDeProyectoID(ProyectoSeleccionado.Clave);
                            proy.ProyectoID = ProyectoSeleccionado.Clave;
                            proy.NombreServicio = pestanya.NombreServicio;
                            proy.UrlServicio = pestanya.UrlServicio;
                            proyCN.ActualizarServiceNameProyecto(proy);
                        }
                    }
                    if (pestanya.Deleted)
                    {
                        if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                        {
                            EcosistemaServicioExterno eco = new EcosistemaServicioExterno();
                            eco.NombreServicio = pestanya.NombreServicio;
                            eco.UrlServicio = pestanya.UrlServicio;
                            proyCN.EliminarEcosistemaServicioExterno(eco);
                        }
                        else
                        {
                            ProyectoServicioExterno proy = new ProyectoServicioExterno();
                            proy.NombreServicio = pestanya.NombreServicio;
                            proy.UrlServicio = pestanya.UrlServicio;
                            proy.ProyectoID = ProyectoSeleccionado.Clave;
                            proy.OrganizacionID = proyCN.ObtenerOrganizacionIDAPartirDeProyectoID(ProyectoSeleccionado.Clave);
                            proyCN.EliminarProyectoServicioExterno(proy);
                        }
                    }
                }
                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }

        }
        
        #endregion

        #region Métodos

        public AdministrarServiciosExternosViewModel CargarModelo()
        {
            AdministrarServiciosExternosViewModel adServiceName = new AdministrarServiciosExternosViewModel();            
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            // Lista de servicios del proyecto
            adServiceName.ListaServicios = new List<ServiceNameModel>();

            string serviceName = proyCN.ObtenerParametroAplicacion("UrlBaseService");
            if (string.IsNullOrEmpty(serviceName))
            {
                if (EsUsuarioAdministradorProyecto)
                {
                    adServiceName.EsAdministrador = true;
                }
                else
                {
                    adServiceName.EsAdministrador = false;
                }
            }
            else
            {
                adServiceName.EstaConfigurado = true;
                adServiceName.ServiceName = serviceName;
                if (ProyectoSeleccionado.Equals(ProyectoAD.MetaProyecto))
                {
                    List<EcosistemaServicioExterno> listaServicios = new List<EcosistemaServicioExterno>();
                    listaServicios = proyCN.ObtenerEcosistemaServicioExterno();
                    ServiceNameModel servicio = new ServiceNameModel();
                    List<ServiceNameModel> listaAPasar = new List<ServiceNameModel>();

                    foreach (EcosistemaServicioExterno eco in listaServicios)
                    {
                        servicio.NombreServicio = eco.NombreServicio;
                        servicio.UrlServicio = eco.UrlServicio;
                        listaAPasar.Add(servicio);
                    }
                    adServiceName.ListaServicios = listaAPasar;
                }
                else
                {
                    List<ProyectoServicioExterno> listaServicios = new List<ProyectoServicioExterno>();
                    listaServicios = proyCN.ObtenerProyectoServicioExterno(ProyectoSeleccionado.Clave);
                    List<ServiceNameModel> listaAPasar = new List<ServiceNameModel>();
                    foreach (ProyectoServicioExterno proyecto in listaServicios)
                    {
                        ServiceNameModel servicio = new ServiceNameModel();
                        servicio.NombreServicio = proyecto.NombreServicio;
                        servicio.UrlServicio = proyecto.UrlServicio;
                        listaAPasar.Add(servicio);
                    }
                    adServiceName.ListaServicios = listaAPasar;
                }
            }                     
            return adServiceName;
        }

        #endregion
    }
}