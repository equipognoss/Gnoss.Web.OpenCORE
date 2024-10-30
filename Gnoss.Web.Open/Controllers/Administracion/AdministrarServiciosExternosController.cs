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
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para adminstrar los servicios externos de la plataforma
    /// </summary>
    public class AdministrarServiciosExternosController : ControllerBaseWeb
    {
        public AdministrarServiciosExternosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Constantes

        private const string URL_BASE_SERVICE = "UrlBaseService";
        private const string SERVICE_NAME = "{ServiceName}";

        #endregion

        #region Miembros

        private AdministrarServiciosExternosViewModel mPaginaModel = null;

        #endregion

        #region Métodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
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
            GuardarLogAuditoria();
            try
            {
                string serviceName = pOptions.ServiceName;
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                if (serviceName.Contains(SERVICE_NAME))
                {
                    proyCN.ActualizarParametroAplicacion(URL_BASE_SERVICE, serviceName);
                }
                else
                {
                    //throw new Exception($"No se ha podido guardar la Url, falta {SERVICE_NAME}");
                    throw new Exception(UtilIdiomas.GetText("DEVTOOLS", "NOSEHAPODIDOGUARDARLAURL", SERVICE_NAME));
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
                if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                {
                    ActualizarServiciosExternosEcosistema(listaPestanyas, proyCN);
                }
                else
                {
                    ActualizarServiciosExternosProyecto(listaPestanyas, proyCN);
                }

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }
        }

        /// <summary>
        /// Actualizamos los servicios externos configurados en el proyecto en relación al modelo enviado por la vista
        /// </summary>
        /// <param name="pListaServiciosExternos">Lista de servicios definidos en la vista</param>
        /// <param name="pProyCN">ProyectoCN para acceder a la base de datos</param>
        private void ActualizarServiciosExternosProyecto(List<ServiceNameModel> pListaServiciosExternos, ProyectoCN pProyCN)
        {
            List<ProyectoServicioExterno> listaProyectoServicioExterno = pProyCN.ObtenerProyectoServicioExterno(ProyectoSeleccionado.Clave);
            IEnumerable<ServiceNameModel> serviciosAgregar = pListaServiciosExternos.Where(item => !listaProyectoServicioExterno.Any(servicio => servicio.NombreServicio == item.NombreServicio) && !item.Deleted);

            //Obtenemos los servicios que no estan entre los guardados, servicios con el nombre viejo (esto s ehace porque no hay una clave que los identifique)
            List<ProyectoServicioExterno> serviciosEliminar = listaProyectoServicioExterno.Where(item => !pListaServiciosExternos.Any(servicio => servicio.NombreServicio == item.NombreServicio)).ToList();

            // Añadimos los que si existen pero se han marcado para eliminar
            serviciosEliminar.AddRange(listaProyectoServicioExterno.Where(item => pListaServiciosExternos.Any(servicio => servicio.NombreServicio == item.NombreServicio && servicio.Deleted)));

            foreach (ServiceNameModel servicio in serviciosAgregar)
            {
                ProyectoServicioExterno proyectoServicioExterno = new ProyectoServicioExterno();
                proyectoServicioExterno.NombreServicio = servicio.NombreServicio;
                proyectoServicioExterno.UrlServicio = servicio.UrlServicio;
                proyectoServicioExterno.OrganizacionID = ProyectoAD.MetaOrganizacion;
                proyectoServicioExterno.ProyectoID = ProyectoSeleccionado.Clave;
                pProyCN.AgregarProyectoServicioExterno(proyectoServicioExterno);
            }

            foreach (ProyectoServicioExterno servicio in serviciosEliminar)
            {
                pProyCN.EliminarProyectoServicioExterno(servicio);
            }

            pProyCN.Actualizar();
        }

        /// <summary>
        /// Actualizamos los servicios externos configurados en el ecosistema en relación al modelo enviado por la vista
        /// </summary>
        /// <param name="pListaServiciosExternos">Lista de servicios definidos en la vista</param>
        /// <param name="pProyCN">ProyectoCN para acceder a la base de datos</param>
        private void ActualizarServiciosExternosEcosistema(List<ServiceNameModel> pListaServiciosExternos, ProyectoCN pProyCN)
        {
            List<EcosistemaServicioExterno> listaEcosistemaServicioExterno = pProyCN.ObtenerEcosistemaServicioExterno();
            IEnumerable<ServiceNameModel> serviciosAgregar
                = pListaServiciosExternos.Where(item => !listaEcosistemaServicioExterno.Any(servicio => servicio.NombreServicio == item.NombreServicio) && !item.Deleted);

            ///Obtenemos los servicios que no estan entre los guardados, servicios con el nombre viejo (esto s ehace porque no hay una clave que los identifique)
            List<EcosistemaServicioExterno> serviciosEliminar = listaEcosistemaServicioExterno.Where(item => !pListaServiciosExternos.Any(servicio => servicio.NombreServicio == item.NombreServicio)).ToList();
            
            /// Añadimos los que si existen pero se han marcado para eliminar
            serviciosEliminar.AddRange(listaEcosistemaServicioExterno.Where(item => pListaServiciosExternos.Any(servicio => servicio.NombreServicio == item.NombreServicio && servicio.Deleted)));

            foreach (ServiceNameModel servicio in serviciosAgregar)
            {
                EcosistemaServicioExterno eco = new EcosistemaServicioExterno();
                eco.NombreServicio = servicio.NombreServicio;
                eco.UrlServicio = servicio.UrlServicio;                
                pProyCN.AgregarServicioExternoEcosistema(eco);                
            }

            foreach (EcosistemaServicioExterno servicio in serviciosEliminar)
            {
                pProyCN.EliminarEcosistemaServicioExterno(servicio);
            }

            pProyCN.Actualizar();
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