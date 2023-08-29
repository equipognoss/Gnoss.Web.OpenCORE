using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
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
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{

    /// <summary>
    /// Controlador para adminstrar las opciones avanzadas de la plataforma
    /// </summary>
    public class AdministrarOpcionesAvanzadasPlataformaController : ControllerBaseWeb
    {
        public AdministrarOpcionesAvanzadasPlataformaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
             : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        private List<ParametroAplicacion> mParametroAplicacion;
        private AdministrarOpcionesAvanzadasPlataformaViewModel mPaginaModel;
        private Dictionary<string, string> mListaParametrosAplicacion;

        #endregion

        #region Métodos Web

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();

            // Añadir clases para el body del Layout
            ViewBag.BodyClassPestanya = "meta-administrador platformSettingsPage";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_OpcionesAvanzadasPlataforma;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACIONDELAPLATAFORMA");
            // Indicar que está administrando el ecosistema
            ViewBag.isInEcosistemaPlatform = "true";

            CargarPermisosAdministracionComunidadEnViewBag();
            return View(PaginaModel);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
        public ActionResult Guardar(AdministrarOpcionesAvanzadasPlataformaViewModel Options)
        {
            try
            {
                GuardarOpciones(Options);

                InvalidarCaches();

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                return GnossResultERROR(ex.Message);
            }

        }

        #endregion

        #region Métodos de carga de datos

        private void CargarModelo()
        {
            mPaginaModel = new AdministrarOpcionesAvanzadasPlataformaViewModel();

            //Parámetros string
            mPaginaModel.CodigoGoogleAnalyticsProyecto = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CodigoGoogleAnalyticsProyecto);
            mPaginaModel.ConexionEntornoPreproduccion = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ConexionEntornoPreproduccion);
            mPaginaModel.CorreoSolicitudes = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CorreoSolicitudes);
            mPaginaModel.CorreoSugerencias = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CorreoSugerencias);
            mPaginaModel.DominiosSinPalco = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DominiosSinPalco);
            mPaginaModel.HashTagEntorno = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.HashTagEntorno);
            mPaginaModel.Idiomas = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.Idiomas);
            mPaginaModel.LoginFacebook = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginFacebook);
            mPaginaModel.LoginGoogle = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginGoogle);
            mPaginaModel.LoginTwitter = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginTwitter);
            mPaginaModel.NombreEspacioPersonal = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.NombreEspacioPersonal);
            mPaginaModel.Copyright = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.Copyright);
            mPaginaModel.VisibilidadPerfil = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.VisibilidadPerfil);
            mPaginaModel.OntologiasNoLive = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.OntologiasNoLive);
            mPaginaModel.ImplementationKey = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ImplementationKey);
            mPaginaModel.UrlHomeConectado = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlHomeConectado);
            mPaginaModel.GoogleRecaptchaSecret = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GoogleRecaptchaSecret);
            mPaginaModel.DominiosEmailLoginRedesSociales = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DominiosEmailLoginRedesSociales);
            mPaginaModel.UrlsPropiasProyecto = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlsPropiasProyecto);
            mPaginaModel.DuracionCookieUsuario = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DuracionCookieUsuario);
            mPaginaModel.ExtensionesImagenesCMSMultimedia = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ExtensionesImagenesCMSMultimedia);
            mPaginaModel.ExtensionesDocumentosCMSMultimedia = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ExtensionesDocumentosCMSMultimedia);

            mPaginaModel.ipFTP = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ipFTP);
            mPaginaModel.UrlContent = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlContent);
            mPaginaModel.UrlIntragnoss = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlIntragnoss);
            mPaginaModel.UrlIntragnossServicios = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlIntragnossServicios);
            mPaginaModel.UrlBaseService = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlBaseService);
            mPaginaModel.ScriptGoogleAnalytics = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ScriptGoogleAnalytics);
            mPaginaModel.ComunidadesExcluidaPersonalizacion = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion);
            mPaginaModel.LoginUnicoUsuariosExcluidos = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos);
            mPaginaModel.PasosAsistenteCreacionComunidad = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.PasosAsistenteCreacionComunidad);
            mPaginaModel.GrafoMetaBusquedaRecursos = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaRecursos);
            mPaginaModel.GrafoMetaBusquedaPerYOrg = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaPerYOrg);
            mPaginaModel.GrafoMetaBusquedaComunidades = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaComunidades);

            //mPaginaModel.IPRecargarComponentes = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.IPRecargarComponentes);




            //Parámetros booleanos
            mPaginaModel.EcosistemaSinBandejaSuscripciones = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones, true);
            mPaginaModel.EcosistemaSinContactos = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EcosistemaSinContactos, true);
            mPaginaModel.VersionFotoDocumentoNegativo = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.VersionFotoDocumentoNegativo);
            mPaginaModel.CVUnicoPorPerfil = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.CVUnicoPorPerfil);
            mPaginaModel.DatosDemograficosPerfil = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.DatosDemograficosPerfil, true);
            mPaginaModel.EcosistemaSinMetaproyecto = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EcosistemaSinMetaProyecto);
            mPaginaModel.PanelMensajeImportarContactos = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.PanelMensajeImportarContactos, true);
            mPaginaModel.PerfilGlobalEnComunidadPrincipal = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.PerfilGlobalEnComunidadPrincipal);
            mPaginaModel.PestanyaImportarContactosCorreo = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.PestanyaImportarContactosCorreo, true);
            mPaginaModel.RegistroAutomaticoEcosistema = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.RegistroAutomaticoEcosistema, true);
            mPaginaModel.SeguirEnTodaLaActividad = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.SeguirEnTodaLaActividad);
            mPaginaModel.EcosistemaSinHomeUsuario = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EcosistemaSinHomeUsuario, true);
            mPaginaModel.MostrarGruposIDEnHtml = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.MostrarGruposIDEnHtml);
            mPaginaModel.UsarSoloCategoriasPrivadasEnEspacioPersonal = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.UsarSoloCategoriasPrivadasEnEspacioPersonal);
            mPaginaModel.NotificacionesAgrupadas = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.NotificacionesAgrupadas);
            mPaginaModel.RecibirNewsletterDefecto = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.RecibirNewsletterDefecto);
            mPaginaModel.PerfilPersonalDisponible = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.PerfilPersonalDisponible, true);
            mPaginaModel.GenerarGrafoContribuciones = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.GenerarGrafoContribuciones, true);
            mPaginaModel.MantenerSesionActiva = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.MantenerSesionActiva, true);
            mPaginaModel.NoEnviarCorreoSeguirPerfil = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.NoEnviarCorreoSeguirPerfil, false);
			mPaginaModel.LoginUnicoPorUsuario = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginUnicoPorUsuario, true);
            mPaginaModel.EnviarNotificacionesDeSuscripciones = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones, true);


            //Parámetros Int
            mPaginaModel.EdadLimiteRegistroEcosistema = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.EdadLimiteRegistroEcosistema);
            mPaginaModel.SegundosMaxSesionBloqueada = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.SegundosMaxSesionBloqueada);
            mPaginaModel.TamanioPoolRedis = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.TamanioPoolRedis);
            mPaginaModel.UbicacionLogs = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.UbicacionLogs);
            mPaginaModel.UbicacionTrazas = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.UbicacionTrazas);

            mPaginaModel.puertoFTP = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.puertoFTP);
            mPaginaModel.VersionJSEcosistema = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.VersionJSEcosistema);
            mPaginaModel.VersionCSSEcosistema = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.VersionCSSEcosistema);
            mPaginaModel.AceptacionComunidadesAutomatica = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.AceptacionComunidadesAutomatica);
            mPaginaModel.TipoCabecera = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.TipoCabecera);
            mPaginaModel.TamanioPoolRedis = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.TamanioPoolRedis);
            mPaginaModel.usarHTTPSParaDominioPrincipal = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.usarHTTPSParaDominioPrincipal);
            mPaginaModel.CargarIdentidadesDeProyectosPrivadosComoAmigos = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos);


        }

        #endregion

        #region Métodos de guardado

        private void GuardarOpciones(AdministrarOpcionesAvanzadasPlataformaViewModel pOptions)
        {
            try
            {
                bool recalcularIdiomas = false;
                string idiomasAntiguos = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.Idiomas);
                if (string.Compare(idiomasAntiguos, pOptions.Idiomas) != 0)
                {
                    recalcularIdiomas = true;
                }

                PasarDatosADataSet(pOptions);

                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                parametroAplicacionCN.ActualizarConfiguracionGnoss();

                if (recalcularIdiomas)
                {
                    new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication).VersionarCacheLocal(Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private void PasarDatosADataSet(AdministrarOpcionesAvanzadasPlataformaViewModel pOptions)
        {
            // string
            if (!string.IsNullOrEmpty(pOptions.UrlBaseService))
            {
                if (pOptions.UrlBaseService.Contains("{ServiceName}"))
                {
                    GuardarParametroString(TiposParametrosAplicacion.UrlBaseService, pOptions.UrlBaseService);
                }
                else
                {
                    throw new Exception("La UrlBaseService no contiene {ServiceName}");
                }
            }

            GuardarParametroString(TiposParametrosAplicacion.CodigoGoogleAnalyticsProyecto, pOptions.CodigoGoogleAnalyticsProyecto);
            GuardarParametroString(TiposParametrosAplicacion.ConexionEntornoPreproduccion, pOptions.ConexionEntornoPreproduccion);
            GuardarParametroString(TiposParametrosAplicacion.Copyright, pOptions.Copyright);
            GuardarParametroString(TiposParametrosAplicacion.CorreoSolicitudes, pOptions.CorreoSolicitudes);
            GuardarParametroString(TiposParametrosAplicacion.CorreoSugerencias, pOptions.CorreoSugerencias);
            GuardarParametroString(TiposParametrosAplicacion.DominiosEmailLoginRedesSociales, pOptions.DominiosEmailLoginRedesSociales);
            if (!string.IsNullOrEmpty(pOptions.UrlsPropiasProyecto))
            {
				GuardarParametroString(TiposParametrosAplicacion.UrlsPropiasProyecto, pOptions.UrlsPropiasProyecto);
			}
            GuardarParametroString(TiposParametrosAplicacion.DominiosSinPalco, pOptions.DominiosSinPalco);
            GuardarParametroString(TiposParametrosAplicacion.GoogleRecaptchaSecret, pOptions.GoogleRecaptchaSecret);
            GuardarParametroString(TiposParametrosAplicacion.HashTagEntorno, pOptions.HashTagEntorno);
            GuardarParametroString(TiposParametrosAplicacion.Idiomas, pOptions.Idiomas);
            GuardarParametroString(TiposParametrosAplicacion.ImplementationKey, pOptions.ImplementationKey);
            GuardarParametroString(TiposParametrosAplicacion.LoginFacebook, pOptions.LoginFacebook);
            GuardarParametroString(TiposParametrosAplicacion.LoginGoogle, pOptions.LoginGoogle);
            GuardarParametroString(TiposParametrosAplicacion.LoginTwitter, pOptions.LoginTwitter);
            GuardarParametroString(TiposParametrosAplicacion.NombreEspacioPersonal, pOptions.NombreEspacioPersonal);
            GuardarParametroString(TiposParametrosAplicacion.OntologiasNoLive, pOptions.OntologiasNoLive);
            GuardarParametroString(TiposParametrosAplicacion.UrlHomeConectado, pOptions.UrlHomeConectado);
            GuardarParametroString(TiposParametrosAplicacion.VisibilidadPerfil, pOptions.VisibilidadPerfil);
            GuardarParametroString(TiposParametrosAplicacion.DuracionCookieUsuario, pOptions.DuracionCookieUsuario);
            GuardarParametroString(TiposParametrosAplicacion.ExtensionesImagenesCMSMultimedia, pOptions.ExtensionesImagenesCMSMultimedia);
            GuardarParametroString(TiposParametrosAplicacion.ExtensionesDocumentosCMSMultimedia, pOptions.ExtensionesDocumentosCMSMultimedia);


            GuardarParametroString(TiposParametrosAplicacion.ipFTP, pOptions.ipFTP);
            GuardarParametroString(TiposParametrosAplicacion.UrlContent, pOptions.UrlContent);
            GuardarParametroString(TiposParametrosAplicacion.UrlIntragnoss, pOptions.UrlIntragnoss);
            GuardarParametroString(TiposParametrosAplicacion.UrlIntragnossServicios, pOptions.UrlIntragnossServicios);

            GuardarParametroString(TiposParametrosAplicacion.ScriptGoogleAnalytics, pOptions.ScriptGoogleAnalytics);
            GuardarParametroString(TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion, pOptions.ComunidadesExcluidaPersonalizacion);
            GuardarParametroString(TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos, pOptions.LoginUnicoUsuariosExcluidos);
            GuardarParametroString(TiposParametrosAplicacion.PasosAsistenteCreacionComunidad, pOptions.PasosAsistenteCreacionComunidad);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaRecursos, pOptions.GrafoMetaBusquedaRecursos);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaPerYOrg, pOptions.GrafoMetaBusquedaPerYOrg);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaComunidades, pOptions.GrafoMetaBusquedaComunidades);


            //GuardarParametroString(TiposParametrosAplicacion.IPRecargarComponentes, pOptions.IPRecargarComponentes);







            // int
            GuardarParametroString(TiposParametrosAplicacion.EdadLimiteRegistroEcosistema, pOptions.EdadLimiteRegistroEcosistema > 0 ? pOptions.EdadLimiteRegistroEcosistema.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.SegundosMaxSesionBloqueada, pOptions.SegundosMaxSesionBloqueada > 0 ? pOptions.SegundosMaxSesionBloqueada.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.UbicacionLogs, pOptions.UbicacionLogs > 0 ? pOptions.UbicacionLogs.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.UbicacionTrazas, pOptions.UbicacionTrazas > 0 ? pOptions.UbicacionTrazas.ToString() : null);

            GuardarParametroString(TiposParametrosAplicacion.puertoFTP, pOptions.puertoFTP > 0 ? pOptions.puertoFTP.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.VersionJSEcosistema, pOptions.VersionJSEcosistema > 0 ? pOptions.VersionJSEcosistema.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.VersionCSSEcosistema, pOptions.VersionCSSEcosistema > 0 ? pOptions.VersionCSSEcosistema.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.AceptacionComunidadesAutomatica, pOptions.AceptacionComunidadesAutomatica > 0 ? pOptions.AceptacionComunidadesAutomatica.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.TipoCabecera, pOptions.TipoCabecera > 0 ? pOptions.TipoCabecera.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.TamanioPoolRedis, pOptions.TamanioPoolRedis > 0 ? pOptions.TamanioPoolRedis.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.usarHTTPSParaDominioPrincipal, pOptions.usarHTTPSParaDominioPrincipal > 0 ? pOptions.usarHTTPSParaDominioPrincipal.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos, pOptions.CargarIdentidadesDeProyectosPrivadosComoAmigos > 0 ? pOptions.CargarIdentidadesDeProyectosPrivadosComoAmigos.ToString() : null);



            // bool
            GuardarParametroBooleano(TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones, pOptions.EcosistemaSinBandejaSuscripciones, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.EcosistemaSinContactos, pOptions.EcosistemaSinContactos, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.VersionFotoDocumentoNegativo, pOptions.VersionFotoDocumentoNegativo);
            GuardarParametroBooleano(TiposParametrosAplicacion.CVUnicoPorPerfil, pOptions.CVUnicoPorPerfil);
            GuardarParametroBooleano(TiposParametrosAplicacion.DatosDemograficosPerfil, pOptions.DatosDemograficosPerfil, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.EcosistemaSinMetaProyecto, pOptions.EcosistemaSinMetaproyecto);
            GuardarParametroBooleano(TiposParametrosAplicacion.PanelMensajeImportarContactos, pOptions.PanelMensajeImportarContactos, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.PerfilGlobalEnComunidadPrincipal, pOptions.PerfilGlobalEnComunidadPrincipal);
            GuardarParametroBooleano(TiposParametrosAplicacion.PestanyaImportarContactosCorreo, pOptions.PestanyaImportarContactosCorreo, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.RegistroAutomaticoEcosistema, pOptions.RegistroAutomaticoEcosistema, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.SeguirEnTodaLaActividad, pOptions.SeguirEnTodaLaActividad);
            GuardarParametroBooleano(TiposParametrosAplicacion.MostrarGruposIDEnHtml, pOptions.MostrarGruposIDEnHtml);
            GuardarParametroBooleano(TiposParametrosAplicacion.UsarSoloCategoriasPrivadasEnEspacioPersonal, pOptions.UsarSoloCategoriasPrivadasEnEspacioPersonal);
            GuardarParametroBooleano(TiposParametrosAplicacion.EcosistemaSinHomeUsuario, pOptions.EcosistemaSinHomeUsuario, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.NotificacionesAgrupadas, pOptions.NotificacionesAgrupadas);
            GuardarParametroBooleano(TiposParametrosAplicacion.RecibirNewsletterDefecto, pOptions.RecibirNewsletterDefecto);
            GuardarParametroBooleano(TiposParametrosAplicacion.PerfilPersonalDisponible, pOptions.PerfilPersonalDisponible, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.GenerarGrafoContribuciones, pOptions.GenerarGrafoContribuciones, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.MantenerSesionActiva, pOptions.MantenerSesionActiva, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.NoEnviarCorreoSeguirPerfil, pOptions.NoEnviarCorreoSeguirPerfil, false);
            GuardarParametroBooleano(TiposParametrosAplicacion.LoginUnicoPorUsuario, pOptions.LoginUnicoPorUsuario, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones, pOptions.EnviarNotificacionesDeSuscripciones, true);

        }

        private void InvalidarCaches()
        {
            ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            parametroAplicacionCL.InvalidarCacheParametrosAplicacion();
            parametroAplicacionCL.Dispose();
            mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
            mGnossCache.VersionarCacheLocal(Guid.Empty);
        }

        /// <summary>
        /// Guarda un parámetro booleano en el data set de parametro aplicacion
        /// </summary>
        /// <param name="pNombreParametro">Nombre del parámetro</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pValorPorDefecto">Valor por defecto</param>
        /// <param name="pValorFalse">String para el valor a guardar si el valor es True</param>
        /// <param name="pValorTrue">String para el valor a guardar si el valor es False</param>
        private void GuardarParametroBooleano(string pNombreParametro, bool pValor, bool pValorPorDefecto = false, string pValorTrue = "true", string pValorFalse = "false")
        {
            if (pValor != pValorPorDefecto)
            {
                GuardarParametroString(pNombreParametro, pValor ? pValorTrue : pValorFalse);
            }
            else
            {
                GuardarParametroString(pNombreParametro, null);
            }
        }

        /// <summary>
        /// Guarda un parámetro string en el data set de parámetro aplicación
        /// </summary>
        /// <param name="pNombreParametro">Nombre del parámetro</param>
        /// <param name="pValor">Valor</param>
        private void GuardarParametroString(string pNombreParametro, string pValor)
        {
            //ParametroAplicacionDS.ParametroAplicacionRow filaParametro = ParametroAplicacionDeBaseDeDatosDS.ParametroAplicacion.FindByParametro(pNombreParametro);
            //AD.EntityModel.ParametroAplicacion filaParametro = ParametroAplicacion.Find(parametro => parametro.Parametro.Equals(pNombreParametro));
            EntityContext context = mEntityContext;
            AD.EntityModel.ParametroAplicacion filaParametro = context.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(pNombreParametro));
            if (!string.IsNullOrEmpty(pValor))
            {
                if (filaParametro != null && !filaParametro.Valor.Equals(pValor))
                {
                    // El parametro existe, lo modifico
                    filaParametro.Valor = pValor;
                }
                else if (filaParametro == null)
                {
                    // La fila no existe, la creo
                    context.ParametroAplicacion.Add(new ParametroAplicacion(pNombreParametro, pValor));
                    //ParametroAplicacionDeBaseDeDatosDS.ParametroAplicacion.AddParametroAplicacionRow(pNombreParametro, pValor);
                }
            }
            else if (filaParametro != null)
            {
                // El valor es null, elimino el parámetro
                context.ParametroAplicacion.Remove(filaParametro);
                //filaParametro.Delete();
            }
        }

        #endregion

        #region Propiedades

        private AdministrarOpcionesAvanzadasPlataformaViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    CargarModelo();
                }

                return mPaginaModel;
            }
        }

        private Dictionary<string, string> ListaParametrosAplicacion
        {
            get
            {
                if (mListaParametrosAplicacion == null)
                {
                    mListaParametrosAplicacion = ParametroAplicacion.ToDictionary(parametro => parametro.Parametro, parametro => parametro.Valor);
                    //mListaParametrosAplicacion = ParametroAplicacionDeBaseDeDatosDS.ParametroAplicacion.Select(filaParametro => new { filaParametro.Parametro, filaParametro.Valor }).ToDictionary(parametro => parametro.Parametro, parametro => parametro.Valor);
                }
                return mListaParametrosAplicacion;
            }
        }

        private List<ParametroAplicacion> ParametroAplicacion
        {

            get
            {
                if (mParametroAplicacion == null)
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mParametroAplicacion = mEntityContext.ParametroAplicacion.ToList();
                }
                return mParametroAplicacion;
            }
        }
        #endregion
    }
}