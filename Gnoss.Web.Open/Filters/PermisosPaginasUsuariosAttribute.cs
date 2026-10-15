using DocumentFormat.OpenXml.Wordprocessing;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Es.Riam.Gnoss.Web.MVC.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class PermisosPaginasUsuariosAttribute : BaseActionFilterAttribute
    {

        protected ControladorBase mControladorBase;
        protected LoggingService mLoggingService;
        protected ConfigService mConfigService;
        protected EntityContext mEntityContext;
        protected RedisCacheWrapper mRedisCacheWrapper;
        protected VirtuosoAD mVirtuosoAD;
        protected IHttpContextAccessor mHttpContextAccessor;
        protected ILogger mLogger;
        protected ILoggerFactory mLoggerFactory;
        public PermisosPaginasUsuariosAttribute(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ILogger<PermisosPaginasUsuariosAttribute> logger, ILoggerFactory loggerFactory, bool pComprobarPermisoUsuarioNoAdministrador = false)
        {
            ComprobarPermisoUsuarioNoAdministrador = pComprobarPermisoUsuarioNoAdministrador;
            mLoggingService = loggingService;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAD;
            mLogger = logger;
            mLoggerFactory = loggerFactory;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null, mLoggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFilterContext"></param>
        protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
        {
            bool tienePermisoPagina = HasPermission(TipoDePermisoRequerido, PermisosRequeridosParaAcceso);

			if (!tienePermisoPagina && !ComprobarPermisoUsuarioNoAdministrador)
            {
                RedirectWithoutPermission(pFilterContext);
            }
        }

        /// <summary>
        /// Comprueba si el usuario actual tiene alguno de los permisos indicados del tipo indicado
        /// </summary>
        /// <param name="pPermissionType">Tipo de permiso a comprobar</param>
        /// <param name="pPermissions">Permisos que dan acceso. Si no hay ninguno, no se concede el acceso</param>
        /// <returns>True si el usuario tiene alguno de los permisos</returns>
        protected bool HasPermission(TipoDePermiso pPermissionType, ulong[] pPermissions)
        {
			if (pPermissions == null || pPermissions.Length == 0)
			{
				return false;
			}

			UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
			foreach (ulong permisoRequerido in pPermissions)
			{
				bool tienePermiso;
				if (pPermissionType.Equals(TipoDePermiso.Ecosistema))
				{
					tienePermiso = utilPermisos.UsuarioTienePermisoAdministracionEcosistema(permisoRequerido, mControladorBase.UsuarioActual.UsuarioID);
				}
				else
				{
					tienePermiso = utilPermisos.IdentidadTienePermiso(permisoRequerido, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, pPermissionType);
				}

				if (tienePermiso)
				{
					return true;
				}
			}

			return false;
		}

        /// <summary>
        /// Redirige al usuario sin permisos a la home de la comunidad o a su perfil si está en el ecosistema
        /// </summary>
        /// <param name="pFilterContext">Contexto del filtro</param>
        protected void RedirectWithoutPermission(ActionExecutingContext pFilterContext)
        {
            string urlRedirectBase = mControladorBase.BaseURLIdioma + mControladorBase.UrlPerfil;
            if (mControladorBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                urlRedirectBase = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto) + "/";
            }

            Redireccionar(urlRedirectBase, pFilterContext);
        }

        private bool EstaActivadaPaginaDeAdministracion()
        {
            if (!string.IsNullOrEmpty(PermisoPaginaNecesario) && !mControladorBase.ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                Dictionary<string, string> parametroProyecto = proyectoCL.ObtenerParametrosProyecto(mControladorBase.ProyectoSeleccionado.Clave);
                proyectoCL.Dispose();

                if (!parametroProyecto.ContainsKey(PermisoPaginaNecesario) || parametroProyecto[PermisoPaginaNecesario] != "1")
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PermisoPaginaNecesario { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TipoPaginaAdministracion TipoPaginaAdministracion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ComprobarPermisoUsuarioNoAdministrador { get; set; }

		public TipoDePermiso TipoDePermisoRequerido { get; set; }

		public ulong[] PermisosRequeridosParaAcceso { get; set; }
    }
}