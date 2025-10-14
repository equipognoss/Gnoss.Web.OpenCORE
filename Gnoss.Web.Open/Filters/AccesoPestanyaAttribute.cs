using System;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Web.MVC.Filters
{
    /// <summary>
    /// El controlador que tenga este filtro, está permitiendo que un usuario no logueado o que no sea miembro de la comunidad puede cargar esta acción
    /// </summary>
    public class AccesoPestanyaAttribute : BaseActionFilterAttribute
    {
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ControladorBase mControladorBase;
        private EntityContext mEntityContext;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AccesoPestanyaAttribute(EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, GnossCache gnossCache, ILogger<AccesoPestanyaAttribute> logger, ILoggerFactory loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null, mLoggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);
        }

        protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
        {
            if (mControladorBase.ProyectoSeleccionado == null)
            {
                pFilterContext.Result = new RedirectResult("/error?errorCode=404&lang=" + mControladorBase.RequestParams("lang") + "&urlAcceso=" + "/comunidad/" + mControladorBase.RequestParams("nombreProy"));
                return;
            }

            ProyectoPestanyaMenu pestanya = mControladorBase.ProyectoPestanyaActual;

            if (pestanya != null && !pestanya.Activa)
            {
                //Saco la página 404
                pFilterContext.Result = Controlador(pFilterContext).RedireccionarAPaginaNoEncontrada();
            }

            if (UsuarioSinAcceso(pestanya, pFilterContext))
            {
                if (!string.IsNullOrEmpty(pestanya.HTMLAlternativo))
                {
                    pFilterContext.Result = Controlador(pFilterContext).DevolverVista("../Shared/IndexHTMLLibre", pestanya.HTMLAlternativo);
                }
                else
                {
                    //Si no está logueado le saco la página de login
                    //Si el usuario está logueado le saco la página 404                    
                    if (mControladorBase.UsuarioActual != null && mControladorBase.UsuarioActual.EsUsuarioInvitado)
                    {
                        string scheme = "http";
                        if (mConfigService.PeticionHttps())
                        {
							scheme = "https";
						}
                        string urlActual = $"{scheme}://{pFilterContext.HttpContext.Request.Host}{pFilterContext.HttpContext.Request.Path}";
                        string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto);
                        string urlRedirect = urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "LOGIN") + "/redirect";
						urlRedirect = urlActual.Replace(urlComunidad, urlRedirect);
						//Redirijo a login
						pFilterContext.Result = new RedirectResult(urlRedirect);
                    }
                    else
                    {
                        //Saco la página 404
                        pFilterContext.Result = Controlador(pFilterContext).RedireccionarAPaginaNoEncontrada();
					}
				}
			}

			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
			if (pestanya != null && !pestanya.ListaIdiomasDisponibles(paramCL.ObtenerListaIdiomasDictionary()).Contains(mControladorBase.IdiomaUsuario))
            {
                pFilterContext.Result = Controlador(pFilterContext).DevolverVista("../Shared/IndexNoIdioma", ((HeaderModel)(Controlador(pFilterContext).ViewBag.Cabecera)).MultiLingualLinks);
            }
        }

        private bool UsuarioSinAcceso(ProyectoPestanyaMenu pPestanya, ActionExecutingContext pFilterContext)
        {
            bool usuarioSinAcceso = false;

            if (pPestanya == null)
            {
                return false;
                //if (!this.Controlador(pFilterContext).Request.Method.Equals("GET"))
                //{
                //    usuarioSinAcceso = true;
                //}
            }
            else
            {
                switch (pPestanya.Privacidad)
                {
                    case TipoPrivacidadPagina.Especial:
                        if (mControladorBase.ProyectoSeleccionado.TipoAcceso != TipoAcceso.Privado && mControladorBase.ProyectoSeleccionado.TipoAcceso != TipoAcceso.Reservado)
                        {
                            //publicas    
                            if (mControladorBase.UsuarioActual == null || mControladorBase.UsuarioActual.EsIdentidadInvitada)
                            {
                                usuarioSinAcceso = true;
                            }
                        }
                        break;
                    case TipoPrivacidadPagina.Lectores:
                        bool identidadActualEsPerfilEditor = false;
                        foreach (Guid perfilID in pPestanya.ListaRolIdentidad.Keys)
                        {
                            if (mControladorBase.IdentidadActual != null && mControladorBase.IdentidadActual.PerfilID == perfilID)
                            {
                                identidadActualEsPerfilEditor = true;
                            }
                        }

                        if (!identidadActualEsPerfilEditor)
                        {
                            bool identidadActualPerteneceAAlgunGrupo = false;
                            if (mControladorBase.IdentidadActual != null)
                            {
                                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                                mControladorBase.IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerGruposParticipaIdentidad(mControladorBase.IdentidadActual.Clave, true));
                                if (mControladorBase.IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalCorporativo || mControladorBase.IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalPersonal)
                                {
                                    mControladorBase.IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerGruposParticipaIdentidad(mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, true));
                                }
                                mControladorBase.IdentidadActual.GestorIdentidades.CargarGrupos();
                                identidadCN.Dispose();

                                foreach (Guid grupoID in pPestanya.ListaRolGrupoIdentidades.Keys)
                                {
                                    if (mControladorBase.IdentidadActual.GestorIdentidades.ListaGrupos.ContainsKey(grupoID))
                                    {
                                        identidadActualPerteneceAAlgunGrupo = true;
                                    }
                                }
                            }

                            if (!identidadActualPerteneceAAlgunGrupo)
                            {
                                usuarioSinAcceso = true;
                            }
                        }
                        break;
                }
            }
            return usuarioSinAcceso;
        }
    }
}