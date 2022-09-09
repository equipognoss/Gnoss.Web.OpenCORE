using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;


namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador de la página de de modificar la consultas SPARQL de la tabla ProyectoSearchPersonalizado
    /// </summary>
    public class AdministrarParametrosBusquedaPersonalizadosController : ControllerBaseWeb
    {
        public AdministrarParametrosBusquedaPersonalizadosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
            proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, servicesUtilVirtuosoAndReplication);
        }

        #region Miembros

        private AdministrarParametrosBusquedaPersonalizadosViewModel mPaginaModel = null;
        private ProyectoCN proyCN;

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

            mPaginaModel = CargarModelo();
            return View(mPaginaModel);
        }
        /// <summary>
        /// Guardar
        /// </summary>
        /// <param name="ListaPestanyas"></param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(List<ParametroBusquedaPersonalizadoModel> ListaPestanyas)
        {
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
            try
            {
                Guid organizacionID = proyCN.ObtenerOrganizacionIDAPartirDeProyectoID(ProyectoSeleccionado.Clave);
                proyCN.ActualizarParametrosBusquedaPersonalizados(organizacionID, ProyectoSeleccionado.Clave, ListaPestanyas);
                if (iniciado)
                {
                    HttpResponseMessage resultado = InformarCambioAdministracion("SearchPersonalizado", JsonConvert.SerializeObject(ListaPestanyas, Formatting.Indented));
                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);
                proyCL.Dispose();
                mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }
            
        }       
        #endregion

        #region Métodos públicos 

        private AdministrarParametrosBusquedaPersonalizadosViewModel CargarModelo()
        {
            AdministrarParametrosBusquedaPersonalizadosViewModel adModCon = new AdministrarParametrosBusquedaPersonalizadosViewModel();
            List <ProyectoSearchPersonalizado> listaProyectos = new List<ProyectoSearchPersonalizado>();
            listaProyectos = proyCN.ObtenerProyectosSearchPersonalizado(ProyectoSeleccionado.Clave);

            List<ParametroBusquedaPersonalizadoModel> listaAPasar = new List<ParametroBusquedaPersonalizadoModel>();

            foreach(ProyectoSearchPersonalizado proy in listaProyectos)
            {
                ParametroBusquedaPersonalizadoModel param = new ParametroBusquedaPersonalizadoModel();
                param.NombreParametro = proy.NombreFiltro;
                param.WhereParametro = proy.WhereSPARQL;
                param.OrderByParametro = proy.OrderBySPARQL;
                param.WhereFacetaParametro = proy.WhereFacetasSPARQL;

                listaAPasar.Add(param);
            }
            adModCon.ListaParametros = listaAPasar;
            return adModCon;
        }

        #endregion
    }
}