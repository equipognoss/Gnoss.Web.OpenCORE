using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;


namespace Es.Riam.Gnoss.Web.MVC.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class PermisosPaginasUsuariosAttribute : BaseActionFilterAttribute
    {

        private ControladorBase mControladorBase;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private RedisCacheWrapper mRedisCacheWrapper;
        private VirtuosoAD mVirtuosoAD;

        public PermisosPaginasUsuariosAttribute(string pPermisoPaginaNecesario, TipoPaginaAdministracion pTipoPaginaAdministracion, LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor)
        {
            TipoPaginaAdministracion = pTipoPaginaAdministracion;
            PermisoPaginaNecesario = pPermisoPaginaNecesario;
            mLoggingService = loggingService;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAD;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFilterContext"></param>
        protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
        {

            //si no esta habilitada la pagina de administracion en la comunidad
            if (!EstaActivadaPaginaDeAdministracion())
            {
                Redireccionar(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto) + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRARCOMUNIDADGENERAL"), pFilterContext);
            }

            bool tienePermisoPagina = mControladorBase.ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID);
            if (!tienePermisoPagina)
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null);
                tienePermisoPagina = proyCN.TienePermisoUsuarioEnPagina(mControladorBase.ProyectoSeleccionado.FilaProyecto.OrganizacionID, mControladorBase.ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, TipoPaginaAdministracion);
                proyCN.Dispose();
            }


            if (!tienePermisoPagina)
            {
                string urlRedirectBase = mControladorBase.BaseURLIdioma + mControladorBase.UrlPerfil;
                if (mControladorBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                {
                    urlRedirectBase = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto) + "/";
                }

                //redirección a la home
                Redireccionar(urlRedirectBase, pFilterContext);
            }
        }

        private bool EstaActivadaPaginaDeAdministracion()
        {
            if (!string.IsNullOrEmpty(PermisoPaginaNecesario) && !mControladorBase.ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
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
    }
}