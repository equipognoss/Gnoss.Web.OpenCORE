using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using System.Text;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Gnoss.Web.Open.Filters;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{

    /// <summary>
    /// Controlador para adminstrar las opciones avanzadas de la plataforma
    /// </summary>
    public class AdministrarOpcionesAvanzadasPlataformaController : ControllerAdministrationWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarOpcionesAvanzadasPlataformaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarOpcionesAvanzadasPlataformaController> logger, ILoggerFactory loggerFactory)
             : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            _appLifetime = appLifetime;
        }

        private static string[] LISTA_IDIOMAS = { "es", "en", "pt", "ca", "eu", "gl", "fr", "de", "it" };

        #region Miembros

        private List<ParametroAplicacion> mParametroAplicacion;
        private AdministrarOpcionesAvanzadasPlataformaViewModel mPaginaModel;
        private Dictionary<string, string> mListaParametrosAplicacion;
        private readonly IHostApplicationLifetime _appLifetime;

		private GestorDocumental mGestorDocumentalOntologiasPlataforma;
        private AdministrarPlantillasOntologicasViewModel mDescargaClasesModelPlataforma;
		#endregion

        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarLaConfiguracionPlataforma} })]
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
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarLaConfiguracionPlataforma } })]
		public ActionResult Guardar(AdministrarOpcionesAvanzadasPlataformaViewModel Options)
        {
            GuardarLogAuditoria();
            try
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

                if (!string.IsNullOrEmpty(Options.DominioPaginasAdministracion) && !(Uri.TryCreate(Options.DominioPaginasAdministracion, UriKind.Absolute, out Uri uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
                {
                    return GnossResultERROR($"El dominio \"{Options.DominioPaginasAdministracion}\" para las páginas de administración no tiene un formato válido. Ejemplo de formato: https://administracion-testing.gnoss.com");
                }

                bool esNecesarioReiniciar = GuardarOpciones(Options);

                InvalidarCaches();

                if (iniciado)
                {

                    HttpResponseMessage response = InformarCambioAdministracion("OpcionesAvanzadasEcosistema", JsonConvert.SerializeObject(Options, Formatting.Indented));
                    if (!response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new ExcepcionWeb("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }

                if (esNecesarioReiniciar)
                {
                    return GnossResultOK("shutdown");
                }
                else
                {
                    return GnossResultOK();
                }
            }
            catch (Exception ex)
            {
                return GnossResultERROR(ex.Message);
            }

        }

        /// <summary>
        /// Añadir en la vista los idiomas personalizados
        /// </summary>
        /// <param name="customLanguage"> Idiomas personalizados a añadir en la vista </param>
        /// <returns></returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarLaConfiguracionPlataforma } })]
		public ActionResult AddCustomLanguage(string customLanguage)
        {
            return PartialView("_partial-views/_language-row", customLanguage + "|custom");
        }

		#region Métodos de carga de datos

		
		/// <summary>
		/// 
		/// </summary>
		private GestorDocumental GestorDocumentalOntologiasPlataforma
		{
			get
			{
				if (mGestorDocumentalOntologiasPlataforma == null)
				{
					mGestorDocumentalOntologiasPlataforma = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

					DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
					docCL.ObtenerBaseRecursosProyecto(mGestorDocumentalOntologiasPlataforma.DataWrapperDocumentacion, mControladorBase.UsuarioActual.ProyectoID, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.UsuarioID);

					DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
					docCN.ObtenerOntologiasPlataforma(mGestorDocumentalOntologiasPlataforma.DataWrapperDocumentacion);
					docCN.Dispose();

					mGestorDocumentalOntologiasPlataforma.CargarDocumentos();
				}
				return mGestorDocumentalOntologiasPlataforma;
			}
		}


		private void CargarModelo()
        {
            mPaginaModel = new AdministrarOpcionesAvanzadasPlataformaViewModel();

            //Parámetros string
            mPaginaModel.CodigoGoogleAnalyticsProyecto = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CodigoGoogleAnalyticsProyecto);
            mPaginaModel.DominiosPermitidosCORS = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DominiosPermitidosCORS);
            mPaginaModel.ConexionEntornoPreproduccion = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ConexionEntornoPreproduccion);
            mPaginaModel.CorreoSolicitudes = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CorreoSolicitudes);
            mPaginaModel.CorreoSugerencias = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CorreoSugerencias);
            mPaginaModel.DominiosSinPalco = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DominiosSinPalco);
            mPaginaModel.HashTagEntorno = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.HashTagEntorno);

            mPaginaModel.IdiomasBase = LISTA_IDIOMAS;
            string idiomas = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.Idiomas);
            if (string.IsNullOrEmpty(idiomas))
            {
                idiomas = string.Join("&&&", mConfigService.ObtenerListaIdiomasDictionary().Select(item => $"{item.Key}|{item.Value}").ToList());
            }

            mPaginaModel.Idiomas = idiomas;

            CargarModeloIdiomasPersonalizados(idiomas);

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

            string urlsPropiasProyecto = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlsPropiasProyecto);
            mPaginaModel.UrlProyectosPublicos = ControladorProyecto.ObtenerUrlProyectosPorTipoAcceso(urlsPropiasProyecto, TipoAcceso.Publico);
            mPaginaModel.UrlProyectosPrivados = ControladorProyecto.ObtenerUrlProyectosPorTipoAcceso(urlsPropiasProyecto, TipoAcceso.Privado);

            mPaginaModel.DuracionCookieUsuario = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DuracionCookieUsuario);
            mPaginaModel.ExtensionesImagenesCMSMultimedia = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ExtensionesImagenesCMSMultimedia);
            mPaginaModel.ExtensionesDocumentosCMSMultimedia = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ExtensionesDocumentosCMSMultimedia);
            mPaginaModel.ipFTP = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ipFTP);
            mPaginaModel.UrlContent = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlContent);
            mPaginaModel.UrlIntragnoss = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlIntragnoss);
            mPaginaModel.UrlIntragnossServicios = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlIntragnossServicios);
            mPaginaModel.UrlBaseService = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlBaseService);
            mPaginaModel.DominioPaginasAdministracion = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DominioPaginasAdministracion);
            mPaginaModel.ScriptGoogleAnalytics = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ScriptGoogleAnalytics);
            mPaginaModel.ComunidadesExcluidaPersonalizacion = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion);
            mPaginaModel.LoginUnicoUsuariosExcluidos = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos);
            mPaginaModel.PasosAsistenteCreacionComunidad = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.PasosAsistenteCreacionComunidad);
            mPaginaModel.GrafoMetaBusquedaRecursos = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaRecursos);
            mPaginaModel.GrafoMetaBusquedaPerYOrg = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaPerYOrg);
            mPaginaModel.GrafoMetaBusquedaComunidades = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaComunidades);

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

        // Carga los idiomas personalizas en el modelo
        private void CargarModeloIdiomasPersonalizados(string pIdiomas)
        {
            StringBuilder idiomasPersonalizados = new StringBuilder();

            string[] idiomas = pIdiomas.Split("&&&");
            foreach (string idioma in idiomas)
            {
                string clave = idioma.Substring(0, 2);
                if (!LISTA_IDIOMAS.Contains(clave))
                {
                    idiomasPersonalizados.Append($"{idioma}&&&");
                }
            }
            if (!string.IsNullOrEmpty(idiomasPersonalizados.ToString()))
            {
                // ELiminar las tres ultimos caracteres
                mPaginaModel.IdiomasPersonalizados = idiomasPersonalizados.ToString().Substring(0, idiomasPersonalizados.Length - 3);
            }
        }

        #endregion

        #region Métodos de guardado

        private bool GuardarOpciones(AdministrarOpcionesAvanzadasPlataformaViewModel pOptions)
        {
            bool esNecesarioReiniciar = false;
            try
            {
                bool recalcularIdiomas = false;
                string idiomasAntiguos = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.Idiomas);
                if (string.Compare(idiomasAntiguos, pOptions.Idiomas) != 0)
                {
                    recalcularIdiomas = true;
                }

                PasarDatosADataSet(pOptions);

                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                parametroAplicacionCN.ActualizarConfiguracionGnoss();

                if (recalcularIdiomas)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                    new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory).VersionarCacheLocal(Guid.Empty);

                    paramCL.InvalidarCacheIdiomas();
                    UtilIdiomas mUtilIdiomas = new UtilIdiomas(IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);
                    mUtilIdiomas.VaciarDiccionarioLenguajes();

                    // ¿Se quiere reiniciar ahora la aplicacion?
                    if (pOptions.ReiniciarAplicacion)
                    {
                        esNecesarioReiniciar = true;
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError($"Ha habido un error al guardar las configuraciones. ERROR: {ex}", mlogger);
                throw;
            }

            return esNecesarioReiniciar;
        }

        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarLaConfiguracionPlataforma } })]
		public void Shutdown()
        {
            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            Guid clave = Guid.NewGuid();
            proyectoCL.AgregarObjetoCache(NombresCL.CLAVEREINICIO, clave, 3600);
            _appLifetime.StopApplication();
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
                    throw new ExcepcionWeb("La UrlBaseService no contiene {ServiceName}");
                }
            }

            if (!string.IsNullOrEmpty(pOptions.Idiomas))
            {
                GuardarParametroString(TiposParametrosAplicacion.Idiomas, pOptions.Idiomas);
            }
            else
            {
                throw new ExcepcionWeb("Debe haber al menos un idioma seleccionado");
            }

            GuardarParametroString(TiposParametrosAplicacion.CodigoGoogleAnalyticsProyecto, pOptions.CodigoGoogleAnalyticsProyecto);
            GuardarParametroString(TiposParametrosAplicacion.DominiosPermitidosCORS, pOptions.DominiosPermitidosCORS);
            GuardarParametroString(TiposParametrosAplicacion.ConexionEntornoPreproduccion, pOptions.ConexionEntornoPreproduccion);
            GuardarParametroString(TiposParametrosAplicacion.Copyright, pOptions.Copyright);
            GuardarParametroString(TiposParametrosAplicacion.CorreoSolicitudes, pOptions.CorreoSolicitudes);
            GuardarParametroString(TiposParametrosAplicacion.CorreoSugerencias, pOptions.CorreoSugerencias);
            GuardarParametroString(TiposParametrosAplicacion.DominiosEmailLoginRedesSociales, pOptions.DominiosEmailLoginRedesSociales);
            GuardarParametroString(TiposParametrosAplicacion.UrlsPropiasProyecto, ControladorProyecto.GenerarUrlsPropiasProyecto(pOptions.UrlProyectosPublicos, pOptions.UrlProyectosPrivados));
            GuardarParametroString(TiposParametrosAplicacion.DominiosSinPalco, pOptions.DominiosSinPalco);
            GuardarParametroString(TiposParametrosAplicacion.GoogleRecaptchaSecret, pOptions.GoogleRecaptchaSecret);
            GuardarParametroString(TiposParametrosAplicacion.HashTagEntorno, pOptions.HashTagEntorno);
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
            GuardarParametroString(TiposParametrosAplicacion.DominioPaginasAdministracion, pOptions.DominioPaginasAdministracion);
            GuardarParametroString(TiposParametrosAplicacion.ScriptGoogleAnalytics, pOptions.ScriptGoogleAnalytics);
            GuardarParametroString(TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion, pOptions.ComunidadesExcluidaPersonalizacion);
            GuardarParametroString(TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos, pOptions.LoginUnicoUsuariosExcluidos);
            GuardarParametroString(TiposParametrosAplicacion.PasosAsistenteCreacionComunidad, pOptions.PasosAsistenteCreacionComunidad);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaRecursos, pOptions.GrafoMetaBusquedaRecursos);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaPerYOrg, pOptions.GrafoMetaBusquedaPerYOrg);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaComunidades, pOptions.GrafoMetaBusquedaComunidades);

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
            ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
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
            EntityContext context = mEntityContext;
            ParametroAplicacion filaParametro = context.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(pNombreParametro));
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
                }
            }
            else if (filaParametro != null)
            {
                // El valor es null, elimino el parámetro
                context.ParametroAplicacion.Remove(filaParametro);
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
                    mParametroAplicacion = mEntityContext.ParametroAplicacion.ToList();
                }
                return mParametroAplicacion;
            }
        }

        #endregion
    }
}