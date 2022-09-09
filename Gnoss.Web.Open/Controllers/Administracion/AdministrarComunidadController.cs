using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controller de administrar comunidad
    /// </summary>
    public class AdministrarComunidadController : ControllerBaseWeb
    {

        public AdministrarComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        /// <summary>
        /// Pagina principal donde se muestran todas las paginas de administración de la comunidad.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();

            PermisosPaginasAdministracionViewModel paginaModel = PermisosPaginasAdministracion;

            if (paginaModel.EsAdministradorProyecto)
            {
                return View(paginaModel);
            }
            else
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                bool esAdministradorMetaProyecto = proyCN.EsUsuarioAdministradorProyecto(mControladorBase.UsuarioActual.UsuarioID, ProyectoAD.MetaProyecto);

                if (esAdministradorMetaProyecto)
                {
                    //Redirigir a página AdministrarOpcionesMetaAdministrador
                    return Redirect(Comunidad.Url + "/" + UtilIdiomas.GetText("URLSEM", "ADMINMETAADMINOPTIONS"));
                }
                else
                {
                    //Redirigir home Proyecto
                    return Redirect(Comunidad.Url);
                }
            }
        }

        /// <summary>
        /// Obtiene o establece si el usuario invitado puede ver esta página
        /// </summary>
        public override bool PaginaVisibleEnPrivada
        {
            get
            {
                return true;
            }
        }

    }
}
