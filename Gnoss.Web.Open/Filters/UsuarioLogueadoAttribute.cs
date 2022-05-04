using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using static Es.Riam.Gnoss.Util.Seguridad.Capacidad.General;

namespace Es.Riam.Gnoss.Web.MVC.Filters
{

    public enum RolesUsuario
    {
        Logueado = 0,
        MiembroComunidad = 1,
        SupervisorComunidad = 2,
        Diseñador = 3,
        AdministradorComunidad = 4,
        AdministradorOrganizacion = 5,
        AdministradorMetaProyecto = 6
    }

    public class UsuarioLogueadoAttribute : BaseActionFilterAttribute
    {
        private ControladorBase mControladorBase;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private RedisCacheWrapper mRedisCacheWrapper;
        private VirtuosoAD mVirtuosoAD;

        public UsuarioLogueadoAttribute(RolesUsuario pRol, LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor)
        {
            mLoggingService = loggingService;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAD;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null);
            Rol = pRol;
            //Rol = RolesUsuario.Logueado;
        }

        public RolesUsuario Rol
        {
            get;
            set;
        }

        protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
        {
            
            ControllerBaseWeb controlador = (ControllerBaseWeb)Controlador(pFilterContext);

            string urlRedirectBase = mControladorBase.BaseURLIdioma + mControladorBase.UrlPerfil;
            if (mControladorBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                urlRedirectBase = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto) + "/";
            }

            if (!controlador.UsuarioSinLoginPermitido)
            {
                if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
                {
                    //login comunidad
                    string urlRedireccion = urlRedirectBase + mControladorBase.UtilIdiomas.GetText("URLSEM", "LOGIN") + mControladorBase.UrlParaLoginConRedirect;
                    Redireccionar(urlRedireccion, pFilterContext);
                    //HttpContext.Current.Response.Redirect(urlRedireccion);
                }
                else if (Rol.Equals(RolesUsuario.MiembroComunidad) && (mControladorBase.UsuarioActual.EsIdentidadInvitada || mControladorBase.EstaUsuarioBloqueadoProyecto))
                {
                    //redirección a la home
                    Redireccionar(urlRedirectBase, pFilterContext);
                }
            }

            if (Rol.Equals(RolesUsuario.SupervisorComunidad))
            {
                if (!EsIdentidadActualSupervisorProyecto && !mControladorBase.ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    //redirección a la home
                    Redireccionar(urlRedirectBase, pFilterContext);
                }
            }

            if (Rol.Equals(RolesUsuario.AdministradorComunidad))
            {
                if (!mControladorBase.ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    //redirección a la home de la comunidad
                    Redireccionar(urlRedirectBase, pFilterContext);
                }
            }

            if (Rol.Equals(RolesUsuario.AdministradorOrganizacion))
            {
                if (mControladorBase.IdentidadActual.OrganizacionID != null && !mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)CapacidadesAdministrador.AdministrarGeneral, mControladorBase.IdentidadActual.OrganizacionID.Value))
                {
                    //redirección a la home
                    Redireccionar(urlRedirectBase, pFilterContext);
                }
            }

            if (Rol.Equals(RolesUsuario.AdministradorMetaProyecto))
            {
                Es.Riam.Gnoss.Logica.ServiciosGenerales.ProyectoCN proyCN = new Logica.ServiciosGenerales.ProyectoCN(mEntityContext, mLoggingService, mConfigService, null);
                bool esAdministradorMetaProyecto = proyCN.EsUsuarioAdministradorProyecto(mControladorBase.UsuarioActual.UsuarioID, ProyectoAD.MetaProyecto);
                proyCN.Dispose();
                if (!esAdministradorMetaProyecto)
                {
                    //redirección a la home
                    Redireccionar(urlRedirectBase, pFilterContext);
                }
            }
        }

        private bool EsIdentidadActualSupervisorProyecto
        {
            get
            {
                return new ChequeoSeguridad(mControladorBase.UsuarioActual).ComprobarCapacidadEnProyecto((ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos);
            }
        }
    }
}