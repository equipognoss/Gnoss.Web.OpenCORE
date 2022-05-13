using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.CL.Usuarios;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.ExportarImportar;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.MVC;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.Organizador.Correo;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.Controles.Suscripcion;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Gnoss.Web.MVC;
using Es.Riam.Interfaces;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using Es.Riam.Web.Util;
using Gnoss.Web.App_Start;
using Gnoss.Web.Services;
using Gnoss.Web.Services.VirtualPathProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using static Es.Riam.Gnoss.Web.Controles.ControladorBase;
using Es.Riam.InterfacesOpen;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public abstract class ControllerBaseWeb : ControllerBaseGnoss
    {

        private ControladorDocumentacion mControladorDocumentacion;
        protected EntityContextBASE mEntityContextBASE;
        private ControladorCorreo mControladorCorreo;
        private ControladorAmigos mControladorAmigos;
        private ControladorGrupos mControladorGrupos;
        private UtilidadesVirtuoso mUtilidadesVirtuoso;
        private ControladorDocumentoMVC mControladorDocumentoMVC;
        private SemCmsController mSemCmsController;
        private ControladorProyecto mControladorProyecto;
        private ControladorSuscripciones mControladorSuscripciones;
        private ControladorTraducciones mControladorTraducciones;
        private UtilServiciosFacetas mUtilServiciosFacetas;
        private ControladorMapasCharts mControladorMapasCharts;
        private Web.Controles.Administracion.ControladorAdministrarVistas mControladorAdministrarVistas;
        private UtilGeneral mUtilGeneral;
        protected IHostingEnvironment mEnv;
        protected IActionContextAccessor mActionContextAccessor;
        protected IServiceScopeFactory mServiceScopeFactory;
        protected IOAuth mOAuth;


        /// <summary>
        /// Indica si la pagina es visible en una comunidad privada
        /// </summary>
        private bool mPaginaVisibleEnPrivada = false;

        protected ControllerBaseWeb(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(httpContextAccessor, entityContext, loggingService, configService, redisCacheWrapper, virtuosoAD, gnossCache, viewEngine, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication)
        {
            mActionContextAccessor = actionContextAccessor;
            mEnv = env;
            mEntityContextBASE = entityContextBASE;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mOAuth = oAuth;
            mRouteConfig = new RouteConfig(entityContext, loggingService, configService, redisCacheWrapper, virtuosoAD);
        }

        #region Miembros

        private static readonly HttpClient mHttpClient = new HttpClient();

        /// <summary>
        /// Devueve o establece el tipo de doc sem en el que estamos
        /// </summary>
        private string mTipoDocSem;

        /// <summary>
        /// Modelo del perfil.
        /// </summary>
        private UserProfileModel mPerfil;

        /// <summary>
        /// Modelo del formulario de registro
        /// </summary>
        private AutenticationModel mFormularioRegistro;

        private bool? mMostrarDatosDemograficosPerfil = null;

        /// <summary>ObtenerNombresVistas
        /// Contiene el booleano que dice si el usuario es administrador de alguna clase.
        /// </summary>
        private string mEsAdministradorDeAlgunaClase;

        private string mUrlPaginaConParametros;

        private DataWrapperPais mPaisDW;

        /// <summary>
        /// Ruta del fichero de configuracion si y solo si su fichero config es otro q el de por defecto (se utiliza para servicios con hilos)
        /// </summary>
        private string mFicheroConfiguracionBD = string.Empty;

        private bool? mCVUnicoPorPerfil = null;

        /// <summary>
        /// Indica si está cargado en JS de gráfias de google
        /// </summary>
        private bool mJSGraficasGoogleCargado = false;

        private string mMetaRobots = "all";

        /// <summary>
        /// Filas del dataset de proyectometarobots del proyecto actual.
        /// </summary>
        //protected ParametroGeneralDS.ProyectoMetaRobotsRow[] mProyectoMetaRobotsRows;
        protected List<ProyectoMetaRobots> mProyectoMetaRobotsRows;

        /// <summary>
        /// Indica si se ha realizado un Transfer a la página 404
        /// </summary>
        private bool mRedireccionadoA404 = false;

        /// <summary>
        /// Ontología
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// Lista de enlaces multi-idioma
        /// </summary>
        private Dictionary<string, KeyValuePair<bool, string>> mListaEnlacesMultiIdioma = null;

        /// <summary>
        /// Titulo de la página
        /// </summary>
        private string mTituloPagina = null;

        /// <summary>
        /// Url Canónica.
        /// </summary>
        private string mUrlCanonical = string.Empty;

        private ControladorProyectoMVC mControladorProyectoMVC = null;

        private bool mEvitarRedireccionHTTP = false;

        PermisosPaginasAdministracionViewModel mPermisosPaginasAdministracion;

        private static DateTime? UltimaInvalidacionVistasPorError;

        private UtilServicios mUtilServicios;

        private ControladorIdentidades mControladorIdentidades;

        private RouteConfig mRouteConfig;

        #endregion

        #region Metodos

        /// <summary>
        /// Obtiene un token para que el usuario se pueda loguear
        /// </summary>
        public string TokenLoginUsuario
        {
            get
            {
                if (Session.Get("tokenCookie") == null)
                {
                    string ticks = DateTime.Now.Ticks.ToString();
                    Session.Set("tokenCookie", ticks);
                }
                return Session.GetString("tokenCookie");
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller.ToString().StartsWith("Es.Riam.Gnoss.Web.MVC.Controllers.Administracion."))
            {
                TipoPagina = TiposPagina.Administacion;
            }
            /*ProyectoAD.MetaOrganizacion = mControladorBase.OrganizacionGnoss;
            ProyectoAD.MetaProyecto = mControladorBase.ProyectoGnoss;*/
            ObtenerCookieSesionUsuarioActiva();

            ObtenerCookieIdiomaActual(filterContext);

            if (Session.Get("Usuario") == null || (Session.Get<GnossIdentity>("Usuario")).EsUsuarioInvitado)
            {
                //Carga la sesión desde la cookie si existe, si no existe redirige a la página de solicitar cookie al servicio de login para verificar si el usuario se había logueado antes en otro dominio
                filterContext.Result = mControladorBase.SolicitarCookieLoginUsuario(UsuarioActual, TokenLoginUsuario);
                if (filterContext.Result != null)
                {
                    return;
                }
                // TODO desacoplar

                //si es petición Oauth, hay que validar el usuario
                if (EsPeticionOAuth)
                {    
                    mOAuth.ObtenerOAuth(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.FilaProyecto.ProyectoID);
                }

                if (HayValidacionRecaptcha)
                {
                    filterContext.Result = ValidarRecaptcha();
                    if (filterContext.Result != null)
                    {
                        return;
                    }
                }
            }

            if (Session.Get("Usuario") == null)
            {
                //Si después de lo anterior no hay sesión de usuario, el usuario no se ha logueado en ningún dominio, le conecto como invitado
                CrearUsuarioInvitado();
            }
            else
            {
                //La sesión ya estaba iniciada, recupero el usuario
                mControladorBase.RecuperarUsuarioDeSesion(UsuarioActual);
            }



            // Si el usuario tiene que ir al Master, 
          if (mControladorBase.UsuarioActual.UsarMasterParaLectura)
            {
                mControladorBase.AgregarObjetoAPeticionActual("UsarMasterParaLectura", true);
            }

            if (filterContext.Controller is PeticionesAJAXController)
            {
                //Si es una llamada al controlador PeticionesAJAX, no es necesaria la carga del ViewBag, ni las invalidaciones que se hacen.
                //Con esta comprobación, agilizamos mucho las llamadas a "CargarNumElementos" y liberamos al servidor
                mLoggingService.AgregarEntrada("TiemposMVC_OnActionExecuting_PeticionesAJAX");
                return;
            }

            base.OnActionExecuting(filterContext);

            mLoggingService.AgregarEntrada("TiemposMVC_OnActionExecuting_Inicio");

            if (ProyectoSeleccionado == null)
            {
                if (!(filterContext.Controller is ErrorController))
                {
                    filterContext.Result = Redirect("/error?errorCode=404&lang=" + RequestParams("lang") + "&urlAcceso=" + "/comunidad/" + RequestParams("nombreProy"));
                }
                return;
            }

            //Cambio con las comprobaciones en caché con el Proyecto Padre de Ecosistema.
            //El valor de proyectoIDPadre sera el del padre si esta conigurado el parametro y es el mismo que tiene en la fila de proyecto, si no sera el valor del proyecto seleccionado.
            Guid proyectoIDPadre = Guid.Empty;
            string valorParametroAplicacionEcosistemaPadre = "";
            ParametroProyecto.TryGetValue("ComunidadPadreEcosistemaID", out valorParametroAplicacionEcosistemaPadre);
            if (ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.HasValue && !string.IsNullOrEmpty(valorParametroAplicacionEcosistemaPadre) && ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.ToString().Equals(valorParametroAplicacionEcosistemaPadre))
            {
                proyectoIDPadre = ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.Value;
            }
            else
            {
                proyectoIDPadre = ProyectoSeleccionado.Clave;
            }
            string nombreCortoProyectoPadre = "";
            if (ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.HasValue && !string.IsNullOrEmpty(valorParametroAplicacionEcosistemaPadre) && ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.ToString().Equals(valorParametroAplicacionEcosistemaPadre))
            {
                ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                nombreCortoProyectoPadre = proyectoCL.ObtenerNombreCortoProyecto(ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.Value);
                proyectoCL.Dispose();
            }
            else
            {
                nombreCortoProyectoPadre = ProyectoSeleccionado.NombreCorto;
            }


            UtilServicios.ComprobacionCambiosCachesLocales(proyectoIDPadre);

            ComprobacionInvalidarVistasLocales();

            ComprobacionInvalidarTraduccionesLocales();

            bool actualizarRutasPestanyas = UtilServicios.ComprobacionCambiosRutasPestanyas(/*ProyectoSeleccionado.Clave*/proyectoIDPadre);

            bool recalcularRutas = UtilServicios.ComprobacionCambiosRedirecciones();

            if (recalcularRutas)
            {
                mLoggingService.GuardarLogError("recalculando rutas...");
                try
                {
                    //MvcApplication.RecalculandoRutas = true;
                    // TODO Javier mover a middleware
                    //mRouteConfig.RecalcularTablaRutas();
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                }
                finally
                {
                    // TODO Javier 
                    //MvcApplication.RecalculandoRutas = false;
                }
            }
            else if (actualizarRutasPestanyas || !RouteConfig.ListaProyectosRutas.Contains(proyectoIDPadre))
            {
                ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                //invalidar rutas viejas y registrar nuevas
                List<string> listaRutasPestanyasInvalidarDeCache = (List<string>)proyectoCL.ObtenerObjetoDeCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_INVALIDAR + /*ProyectoSeleccionado.Clave*/proyectoIDPadre, true);

                List<string> listaRutasPestanyasRegistrarDeCache = (List<string>)proyectoCL.ObtenerObjetoDeCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_REGISTRAR + /*ProyectoSeleccionado.Clave*/proyectoIDPadre, true);

                if (listaRutasPestanyasInvalidarDeCache != null)
                {
                    //mRouteConfig.InvalidarRutasPestanyasProyecto(/*ProyectoSeleccionado.Clave*/proyectoIDPadre, /*ProyectoSeleccionado.NombreCorto*/ nombreCortoProyectoPadre, listaRutasPestanyasInvalidarDeCache);
                    RouteValueTransformer.EliminarRutasProyecto(ProyectoSeleccionado.NombreCorto);
                }

                if (listaRutasPestanyasRegistrarDeCache != null)
                {
                    CachedRouter<Guid>.RECALCULAR_RUTAS = true;
                    //TODO Javi rutas comentar
                    // mRouteConfig.RegistrarRutasPestanyas(RouteConfig.RouteBuilder, mConfigService.ObtenerListaIdiomasDictionary(), proyectoIDPadre, listaRutasPestanyasRegistrarDeCache);   
                }
                proyectoCL.Dispose();
            }

            ViewBag.ShowError = ControladorBase.ShowErrors;

            ViewBag.UrlLoadResourceActions = BaseURLIdioma;
            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                ViewBag.UrlLoadResourceActions = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto /*nombreCortoProyectoPadre*/);
            }
            ViewBag.UrlLoadResourceActions = ViewBag.UrlLoadResourceActions + "/load-resource-actions";

            if (ProyectoPrincipalUnico.Equals(ProyectoAD.MetaProyecto))
            {
                ViewBag.UrlComunidadPrincipal = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, null);
            }
            else
            {
                ViewBag.UrlComunidadPrincipal = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoPrincipal);
            }

            ViewBag.UrlServicioContextos = UrlServicioContextos;



            ViewBag.Perfil = Perfil;

            ViewBag.IdentidadActual = CargarDatosIdentidadActual();

            if (string.IsNullOrEmpty(RequestParams("callback")) && !ViewBag.ControllerName.Equals("Widget"))
            {
                CargarPermisosComunidad(Comunidad);
            }

            ViewBag.ControladorProyectoMVC = ControladorProyectoMVC;

            //Comprobamos las redirecciones despues de cargar la identidad actual y el proyecto
            ComprobarRedirecciones(filterContext);
            if (filterContext.Result != null)
            {
                //Si tiene que hacer una redireccion, no le dejamos seguir con el metodo
                return;
            }

            ViewBag.NombreProyectoEcosistema = NombreProyectoEcosistema;
            ViewBag.NombreEspacioPersonal = UtilCadenas.ObtenerTextoDeIdioma(EspacioPersonal, UtilIdiomas.LanguageCode, null);

            if (string.IsNullOrEmpty(RequestParams("callback")))
            {
                ControllerHeadBaseWeb controllerHead = new ControllerHeadBaseWeb(this, mLoggingService, mConfigService, mEntityContext, mHttpContextAccessor, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);

                controllerHead.CargarDatosHead();
            }

            ControllerCabeceraBase controllerCabecera = new ControllerCabeceraBase(this, mHttpContextAccessor, mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mGnossCache);
            controllerCabecera.CargarDatosCabecera();
            ((HeaderModel)(ViewBag.Cabecera)).MultiLingualLinks = ListaEnlacesMultiIdioma;
            ViewBag.UrlCanonical = UrlCanonical;

            if (ParametroProyecto.ContainsKey(ParametroAD.RobotsComunidad))
            {
                MetaRobots = ParametroProyecto[ParametroAD.RobotsComunidad];
            }
            AsignarMetaRobots(MetaRobots);

            ViewBag.TituloPagina = TituloPagina;
            ViewBag.GeneradorURLs = GnossUrlsSemanticas;
            ViewBag.ProyectoConexion = mConfigService.ObtenerProyectoConexion();
            ViewBag.TokenLoginUsuario = TokenLoginUsuario;
            ViewBag.UrlServicioLogin = mControladorBase.UrlServicioLogin;
            ViewBag.LoggingService = mLoggingService;
            //TODO Javier ViewBag.SessionTimeout = obtener el session timeout
            ViewBag.applicationServerKey = mConfigService.ObtenerVapidPublicKey();
            IncluirAvisoCookies(filterContext);

            ObtenerUrlSiguientePasoAsistenteNuevaComunidad();

            mLoggingService.AgregarEntrada("TiemposMVC_OnActionExecuting_Fin");
        }

        /*protected UtilIntegracionContinua UtilIntegracionContinua
        {
            get
            {
                if (mUtilIntegracionContinua == null)
                {
                    mUtilIntegracionContinua = new UtilIntegracionContinua(mLoggingService, mEntityContext, mConfigService);
                }
                return mUtilIntegracionContinua;
            }
        }*/

        protected Web.Controles.Administracion.ControladorAdministrarVistas ControladorAdministrarVistas
        {
            get
            {
                if (mControladorAdministrarVistas == null)
                {
                    mControladorAdministrarVistas = new Web.Controles.Administracion.ControladorAdministrarVistas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorAdministrarVistas;
            }
        }

        protected UtilGeneral UtilGeneral
        {
            get
            {
                if (mUtilGeneral == null)
                {
                    mUtilGeneral = new UtilGeneral();
                }
                return mUtilGeneral;
            }
        }

        protected ControladorTraducciones ControladorTraducciones
        {
            get
            {
                if (mControladorTraducciones == null)
                {
                    mControladorTraducciones = new ControladorTraducciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorTraducciones;
            }
        }

        protected ControladorMapasCharts ControladorMapasCharts
        {
            get
            {
                if (mControladorMapasCharts == null)
                {
                    mControladorMapasCharts = new ControladorMapasCharts(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorMapasCharts;
            }
        }

        protected ControladorDocumentoMVC ControladorDocumentoMVC
        {
            get
            {
                if (mControladorDocumentoMVC == null)
                {
                    mControladorDocumentoMVC = new ControladorDocumentoMVC(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorDocumentoMVC;
            }
        }

        protected ControladorProyecto ControladorProyecto
        {
            get
            {
                if (mControladorProyecto == null)
                {
                    mControladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorProyecto;
            }
        }

        protected UtilServiciosFacetas UtilServiciosFacetas
        {
            get
            {
                if (mUtilServiciosFacetas == null)
                {
                    mUtilServiciosFacetas = new UtilServiciosFacetas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
                return mUtilServiciosFacetas;
            }
        }

        protected ControladorSuscripciones ControladorSuscripciones
        {
            get
            {
                if (mControladorSuscripciones == null)
                {
                    mControladorSuscripciones = new ControladorSuscripciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorSuscripciones;
            }
        }

        protected ControladorAmigos ControladorAmigos
        {
            get
            {
                if (mControladorAmigos == null)
                {
                    mControladorAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorAmigos;
            }
        }

        protected UtilidadesVirtuoso UtilidadesVirtuoso
        {
            get
            {
                if (mUtilidadesVirtuoso == null)
                {
                    mUtilidadesVirtuoso = new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
                return mUtilidadesVirtuoso;
            }
        }

        protected ControladorGrupos ControladorGrupos
        {
            get
            {
                if (mControladorGrupos == null)
                {
                    mControladorGrupos = new ControladorGrupos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorGrupos;
            }
        }

        protected ControladorCorreo ControladorCorreo
        {
            get
            {
                if (mControladorCorreo == null)
                {
                    mControladorCorreo = new ControladorCorreo(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorCorreo;
            }
        }

        public ControladorDocumentacion ControladorDocumentacion
        {
            get
            {
                if (mControladorDocumentacion == null)
                {
                    mControladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorDocumentacion;
            }
        }

        protected ControladorIdentidades ControladorIdentidades
        {
            get
            {
                if (mControladorIdentidades == null)
                {
                    mControladorIdentidades = new ControladorIdentidades(new GestionIdentidades(new DataWrapperIdentidad(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication), mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorIdentidades;
            }
        }

        protected SemCmsController ObtenerSemCmsController(Documento pDocumento)
        {

            if (mSemCmsController == null)
            {
                mSemCmsController = new SemCmsController(new SemanticResourceModel(), new Ontologia(), pDocumento, new List<ElementoOntologia>(), ProyectoSeleccionado, IdentidadActual, UtilIdiomas, BaseURL, BaseURLIdioma, BaseURLContent, BaseURLStatic, UrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);
            }
            return mSemCmsController;

        }

        protected UtilServicios UtilServicios
        {
            get
            {
                if (mUtilServicios == null)
                {
                    mUtilServicios = new UtilServicios(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mServicesUtilVirtuosoAndReplication);
                }
                return mUtilServicios;
            }
        }

        /// <summary>
        /// Comprueba si tiene que invalidar las vistas locales
        /// </summary>
        public void ComprobacionInvalidarVistasLocales()
        {
            if (UtilServicios.ComprobacionInvalidarVistasLocales(ProyectoSeleccionado.PersonalizacionID, mControladorBase.PersonalizacionEcosistemaID))
            {
                mLoggingService.AgregarEntrada("Borramos las vistas del VirtualPathProvider");
                BDVirtualPath.LimpiarListasRutasVirtuales();
            }

            //Refrescamos la cache de las vistas de la comunidad, si se han cambiado las vistas
            Dictionary<string, string> diccionarioRefrescoCacheLocal = mRedisCacheWrapper.Cache.Get(GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + ProyectoSeleccionado.PersonalizacionID) as Dictionary<string, string>;
            Dictionary<string, string> diccionarioRefrescoCacheLocalEcosistema = mRedisCacheWrapper.Cache.Get(GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + mControladorBase.PersonalizacionEcosistemaID) as Dictionary<string, string>;

            string claveActualizacion = "";
            if (diccionarioRefrescoCacheLocal != null)
            {
                claveActualizacion += diccionarioRefrescoCacheLocal["ClaveActualizacion"];
            }
            if (diccionarioRefrescoCacheLocalEcosistema != null)
            {
                claveActualizacion += diccionarioRefrescoCacheLocalEcosistema["ClaveActualizacion"];
            }

            string claveActualizacionProyecto = mRedisCacheWrapper.Cache.Get(GnossCacheCL.CLAVE_REFRESCO_CACHE_VISTAS + ProyectoSeleccionado.Clave) as string;

            if (!string.IsNullOrEmpty(claveActualizacion) && (string.IsNullOrEmpty(claveActualizacionProyecto) || !claveActualizacionProyecto.Equals(claveActualizacion)))
            {
                mRedisCacheWrapper.Cache.Set(GnossCacheCL.CLAVE_REFRESCO_CACHE_VISTAS + ProyectoSeleccionado.Clave, claveActualizacion, DateTime.Now.AddYears(1));

                if (!string.IsNullOrEmpty(claveActualizacionProyecto))
                {
                    BaseCL.ActualizarCacheDependencyCacheLocal(ProyectoSeleccionado.Clave, claveActualizacion);
                }
            }

        }

        /// <summary>
        /// Comprueba si tiene que invalidar las traduciones locales
        /// </summary>
        public void ComprobacionInvalidarTraduccionesLocales()
        {
            if (UtilServicios.ComprobacionInvalidarTraduccionesLocales(ProyectoSeleccionado.PersonalizacionID, mControladorBase.PersonalizacionEcosistemaID))
            {
                string dominio = new HostingEnvironment().ApplicationName;
                string dominioConfig = mConfigService.ObtenerDominio();
                if (!string.IsNullOrEmpty(dominioConfig))
                {
                    dominio = dominioConfig;
                }

                if (!string.IsNullOrEmpty(dominio) && dominio.Contains("depuracion.net"))
                {
                    dominio = "";
                }

                UtilIdiomas.CargarTextosPersonalizadosDominio(dominio, mControladorBase.PersonalizacionEcosistemaID);
            }
        }

        // public Dictionary<string, string> ObtenerParametrosLoginExterno(TipoRedSocialLogin pTipoRedSocial, Dictionary<string, string> pParametrosProyecto, ParametroAplicacionDS pParametrosAplicacion)
        public Dictionary<string, string> ObtenerParametrosLoginExterno(TipoRedSocialLogin pTipoRedSocial, Dictionary<string, string> pParametrosProyecto, List<AD.EntityModel.ParametroAplicacion> pParametrosAplicacion)
        {
            string key = "login" + pTipoRedSocial.ToString();
            List<ParametroAplicacion> busqueda = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro == key).ToList();
            string parametros = "";
            if (pParametrosProyecto.ContainsKey(key))
            {
                parametros = pParametrosProyecto[key];
            }
            else
            {
                if (busqueda.Count > 0)
                {
                    parametros = busqueda.FirstOrDefault().Valor;
                }
            }

            string[] listaParametros = parametros.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> parametrosLogin = new Dictionary<string, string>();
            foreach (string param in listaParametros)
            {
                string clave = param.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string valor = param.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[1];
                parametrosLogin.Add(clave, valor);
            }

            return parametrosLogin;
        }

        private void ComprobarRedirecciones(ActionExecutingContext pFilterContext)
        {
            string urlPropiaProyecto = "";

            if (!BaseURL.Contains("depuracion") && !BaseURL.Contains("localhost"))
            {
                if (ProyectoVirtual.FilaProyecto.URLPropia != null)
                {
                    urlPropiaProyecto = ProyectoVirtual.UrlPropia(IdiomaUsuario);
                }
            }

            //Si ya se ha declarado una redirección, no seguimos comprobando, porque la vamos a sobreescribir.
            if (pFilterContext.Result == null)
            {
                //Si el dominio es un proyecto con url propia, compruebo que el usuario no se está saliendo del proyecto
                ComprobarRedireccionAMyGnoss(urlPropiaProyecto, pFilterContext);
                mLoggingService.AgregarEntrada("Comprobada ComprobarRedireccionAMyGnoss");
            }
            if (pFilterContext.Result == null)
            {
                //Comprueba si el usuario está intentando acceder a un proyecto con url propia, en cuyo caso le redirecciona a esa url
                ComprobarRedireccionProyectoConUrlPropia(urlPropiaProyecto, pFilterContext);

                mLoggingService.AgregarEntrada("Comprobada ComprobarRedireccionProyectoConUrlPropia");
            }
            if (pFilterContext.Result == null)
            {
                //Comprueba si el usuario tiene acceso al proyecto. Si no es así le redirecciono a la home de la comunidad
                ComprobarRedireccionHomeProyecto(pFilterContext);

                mLoggingService.AgregarEntrada("Comprobada ComprobarRedireccionHomeProyecto");
            }
            if (pFilterContext.Result == null)
            {
                //Comprueba condiciones especiales en la url.
                ComprobarCondicionesUrl(pFilterContext);
            }
            if (pFilterContext.Result == null)
            {
                //Comprueba condiciones especiales en la url.
                ComprobarCaducidadPassword(pFilterContext);
            }

            if (pFilterContext.Result == null)
            {
                ComprobarRedireccionesUsuario(pFilterContext);
            }

            if (pFilterContext.Result == null && this.Request.Path.ToString().EndsWith("/login") && Request.Headers.ContainsKey("Referer"))
            {
                // Si redirige a una página de login, agrega al final el redirect a la página de la que viene.
                string urlRefferer = new Uri(Request.Headers["Referer"].ToString()).AbsolutePath;

                if (!string.IsNullOrEmpty(urlRefferer) && !string.IsNullOrEmpty(urlRefferer.TrimEnd('/')) && !urlRefferer.EndsWith("/login") && !urlRefferer.Contains("redirect") && !urlRefferer.EndsWith("/home") && !Request.Headers["Referer"].Equals(Comunidad.Url))
                {
                    string redirect = ObtenerUrlRedirect(ref urlRefferer);
                    pFilterContext.Result = Redirect(this.Request.Path.ToString() + redirect);
                }
            }
        }

        protected RouteConfig RouteConfig
        {
            get
            {
                if (mRouteConfig == null)
                {
                    mRouteConfig = new RouteConfig(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD);
                }
                return mRouteConfig;
            }
        }

        /// <summary>
        /// Comprueba las condiciones especiales de la url.
        /// </summary>
        private void ComprobarCondicionesUrl(ActionExecutingContext pFilterContext)
        {
            if (Request.Headers.ContainsKey("redirectSiIdentInv") && mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                pFilterContext.Result = Redirect(Request.Headers["redirectSiIdentInv"]);
            }

            if (Request.Headers.ContainsKey("redirectSiIdentInvConProyPriORev") && mControladorBase.UsuarioActual.EsIdentidadInvitada && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado))
            {
                pFilterContext.Result = Redirect(Request.Headers["redirectSiIdentInvConProyPriORev"]);
            }

            if (Request.Headers.ContainsKey("redirectSiPerNOInv") && mControladorBase.UsuarioActual.PersonaID != UsuarioAD.Invitado)
            {
                pFilterContext.Result = Redirect(Request.Headers["redirectSiPerNOInv"]);
            }

            if (Request.Headers.ContainsKey("redirectSioSi"))
            {
                pFilterContext.Result = Redirect(Request.Headers["redirectSioSi"]);
            }

            if (Request.Headers.ContainsKey("statusCode"))
            {
                if (Request.Headers["statusCode"] == "301")
                {
                    pFilterContext.Result = this.RedirectPermanent(this.Request.Path.ToString());
                }
                else if (Request.Headers["statusCode"] != "403")
                {
                    int codigo = 200;
                    int.TryParse(Request.Headers["statusCode"], out codigo);
                    Response.StatusCode = codigo;
                }
            }

            if (Request.Headers.ContainsKey("redirectPermanente"))
            {
                pFilterContext.Result = this.RedirectPermanent(Request.Headers["redirectPermanente"]);
            }
        }

        /// <summary>
        /// Comprueba las condiciones especiales de la url.
        /// </summary>
        private void ComprobarCaducidadPassword(ActionExecutingContext pFilterContext)
        {
            if (ParametroProyecto.ContainsKey(ParametroAD.CaducidadPassword) && !mControladorBase.UsuarioActual.EsIdentidadInvitada && !this.GetType().Name.Equals("CambiarPasswordController"))
            {
                // Si el proyecto tiene caducidad de password y el usuario pertenece al proyecto,
                // comprobamos cuándo ha cambiado el usuario su contraseña por última vez
                int diasCaducidad = 0;
                if (int.TryParse(ParametroProyecto[ParametroAD.CaducidadPassword], out diasCaducidad))
                {
                    if (!mControladorBase.UsuarioActual.FechaCambioPassword.HasValue)
                    {
                        // El usuario no había entrado nunca en este proyecto desde que se estableció la política de password. 
                        // Le establecemos la fecha actual como fecha de cambio para que a partir de este momento, 
                        // su password tenga fecha de caducidad
                        UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        usuarioCN.EstablecerCaducidadPasswordUsuario(mControladorBase.UsuarioActual.UsuarioID);
                        mControladorBase.UsuarioActual.FechaCambioPassword = DateTime.Now;
                    }
                    else
                    {
                        DateTime fechaCaducaPasswordUsuario = mControladorBase.UsuarioActual.FechaCambioPassword.Value.AddDays(diasCaducidad);
                        if (DateTime.Now > fechaCaducaPasswordUsuario)
                        {
                            pFilterContext.Result = Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "CAMBIARPASSWORD") + "?expired=true&transferto=" + System.Net.WebUtility.UrlEncode(pFilterContext.HttpContext.Request.Path.ToString()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Comprueba las redirecciones del usuario
        /// </summary>
        private void ComprobarRedireccionesUsuario(ActionExecutingContext pFilterContext)
        {
            //Obtener la URL si es que tuviera de redirección.
            UsuarioCL usuarioCL = new UsuarioCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            string redirect = usuarioCL.ObtenerRedireccionUsuario(mControladorBase.UsuarioActual.UsuarioID);
            if (redirect != null)
            {
                redirect = redirect.ToLower();
            }
            usuarioCL.Dispose();

            //string url = UriHelper.GetEncodedUrl(Request);
            string url = $"{mConfigService.Scheme}{Request.Host}{Request.Path}{Request.QueryString}";
            /*if (mConfigService.PeticionHttps())
            {
                url.Replace("http://", "https://");
            }*/
            if (!string.IsNullOrEmpty(redirect) && !url.Contains(redirect) && !url.Contains("paso-registro?url=") && !url.Contains("externalservice") && !(url.ToLower().Contains("cmspagina") || url.Contains("recargarcomponentecms") || url.Contains("condiciones-uso") || url.Contains("desconectar") || url.Contains("politica-privacidad") || url.Contains("condiciones-uso") || url.Contains("politica-de-cookies")))
            {
                redirect = redirect.ToLower();
                pFilterContext.Result = Redirect(redirect);
            }

            if (mControladorBase.UsuarioActual != null && !mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                //Comprobar la cookie para ver si este usuario esta logeado.
                AD.EntityModel.ParametroAplicacion busquedaLoginUnicoPorUsuario = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.LoginUnicoPorUsuario));
                bool loginUnicoPorUsuario = busquedaLoginUnicoPorUsuario != null && busquedaLoginUnicoPorUsuario.Valor.Equals("1");
                AD.EntityModel.ParametroAplicacion busquedaDesconexionUsuarios = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.DesconexionUsuarios));
                bool desconexionUsuarios = busquedaDesconexionUsuarios != null && busquedaDesconexionUsuarios.Valor.Equals("1");
                AD.EntityModel.ParametroAplicacion filaExcluidos = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos));
                List<string> sessionesPermitidasUsuario = null;

                if (loginUnicoPorUsuario && desconexionUsuarios && (filaExcluidos == null || !filaExcluidos.Valor.ToLower().Contains(mControladorBase.UsuarioActual.UsuarioID.ToString())))
                {
                    // La plataforma está configurada para que los usuarios sólo puedan estar logueados en un sólo navegador al mismo tiempo
                    sessionesPermitidasUsuario = mGnossCache.ObtenerDeCache($"{ControladorBase.SESION_UNICA_POR_USUARIO}_List_{mControladorBase.UsuarioActual.UsuarioID}", true) as List<string>;

                    //Si no tiene nada en la cache, implica que no se ha hecho login o que le han desconectado.
                    if (sessionesPermitidasUsuario == null)
                    {
                        //Desconectar
                        //string urlDesconexion = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoConexion)}/{ UtilIdiomas.GetText("URLSEM", "LOGIN")}?desconectado";
                        //string query = "?dominio=" + BaseURL + $"/&eliminar=true&redirect={UrlEncode(urlDesconexion)}";
                        //string urlRedirect = UtilUsuario.UrlServicioLogin + "/eliminarCookie.aspx" + query;
                        string urlRedirect = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoConexion)}/{UtilIdiomas.GetText("URLSEM", "DESCONECTAR")}?redirect={UrlEncode($"{UtilIdiomas.GetText("URLSEM", "LOGIN")}?desconectado=true")}";
                        // El sessionID actual es distinto al sessionID con el que inició sesión previamente. Le redirijo
                        if (!Request.Path.ToString().Contains($"{UtilIdiomas.GetText("URLSEM", "DESCONECTAR")}"))
                        {
                            pFilterContext.Result = Redirect($"{urlRedirect}");
                        }
                    }

                }
            }

        }

        /// <summary>
        /// Comprueba si el usuaio tiene acceso al proyecto al que intenta acceder. Si no es así, le redirecciono a la home del proyecto
        /// </summary>
        private void ComprobarRedireccionHomeProyecto(ActionExecutingContext pFilterContext)
        {
            //Obtengo la URL a la que el usuario intenta acceder
            string url = pFilterContext.HttpContext.Request.Path.ToString();

            if (pFilterContext.Controller is PeticionesAJAXController || pFilterContext.Controller is ErrorController || pFilterContext.Controller is LoadingController || pFilterContext.Controller is CkeditorController || pFilterContext.Controller is CrearCookieController || pFilterContext.Controller is EliminarCookieController)
            {
                return;
            }

            string pathFisico = "";

            try
            {
                //pathFisico = this.Request.PhysicalPath;
                pathFisico = mEnv.WebRootFileProvider.GetFileInfo(Request.Path).PhysicalPath;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            if (string.IsNullOrEmpty(pathFisico) || (!System.IO.File.Exists(pathFisico)))
            {
                if ((ProyectoSeleccionado != null))// && (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto)))
                {
                    //#if !DEBUG
                    ////Quitar el aceptar cookies.
                    //if (!BaseURL.Contains("depuracion.net") && ProyectoSeleccionado.FilaProyecto.URLPropia != null && !url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "ACEPTARCOOKIES")))
                    //{
                    //    //Si el proyecto es privado, compruebo que ha llegado por https
                    //    if (ProyectoSeleccionado.UrlPropia(IdiomaUsuario).StartsWith("https://") && !Request.Scheme.Equals("https"))
                    //    {
                    //        //Le llevo a la misma página pero por https
                    //        pFilterContext.Result = this.RedirectPermanent(UtilDominios.CambiarHttpAHttpsEnUrl(this.Request.Path.ToString()));
                    //    }
                    //    else if (ProyectoSeleccionado.UrlPropia(IdiomaUsuario).StartsWith("http://") && !Request.Scheme.Equals("http"))
                    //    {
                    //        pFilterContext.Result = this.Redirect(UtilDominios.CambiarHttpsAHttpEnUrl(this.Request.Path.ToString()));
                    //    }
                    //}

                    if (Session.Get("Usuario") != null)
                    {
                        bool estaUsuarioEnProyecto = ParticipaUsuarioEnProyecto();

                        // David: Comprobar que si el proyecto esta cerrado o cerrado temporalmente y el usuario no es administrador
                        //        no pueda entrar y vaya al inicio de la comunidad
                        bool comunidadEstadoCerrado = (ProyectoSeleccionado.Estado == (short)EstadoProyecto.Cerrado || ProyectoSeleccionado.Estado == (short)EstadoProyecto.CerradoTemporalmente) && !ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID);

                        if (((!IdentidadActual.FilaIdentidad.ProyectoID.Equals(ProyectoSeleccionado.Clave)) || (comunidadEstadoCerrado) || (!estaUsuarioEnProyecto) || (mControladorBase.UsuarioActual.EsIdentidadInvitada)) && !PaginaVisibleEnPrivada)
                        {
                            if (comunidadEstadoCerrado || (ProyectoSeleccionado != null && !ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Publico) && !ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Restringido)) || estaUsuarioEnProyecto)
                            {
                                //Si es una comunidad privada/reservada pero es la página de olvidé mi contraseña, debe poder accederse sin redirección al Login.
                                if (!url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "OLVIDEPASSWORD")) && !url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "COMPLETARREGISTRO")) && !url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "CAMBIARPASSWORD")) && !url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "ESTABLECERPASSWORD")) && !url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "ACEPTARCOOKIES")) && !url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "POLITICACOOKIES")) && !url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "CONDICIONESUSO")) && !url.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "POLITICAPRIVACIDAD")))
                                {
                                    //if ((!url.Contains("anyadirGnoss.aspx?addToGnoss=")) && (url.Contains("?")))
                                    //{
                                    //    url = url.Remove(url.IndexOf('?'));
                                    //}
                                    url = url.Replace("\n", "").Replace("\t", "");

                                    if (url.Contains(mControladorBase.HTTP))
                                    {
                                        url = url.Remove(0, mControladorBase.HTTP.Length);
                                        url = url.Remove(0, url.IndexOf("/"));
                                    }

                                    bool perfilCambiado = false;
                                    bool estaOrganizacionEnProy = false;

                                    if (IdentidadActual.OrganizacionID.HasValue)
                                    {
                                        List<AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value) && !perfil.PersonaID.HasValue).ToList();
                                        if (filasPerfil.Count > 0)
                                        {
                                            Guid perfilOrgID = filasPerfil.First().PerfilID;
                                            estaOrganizacionEnProy = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(identidad => identidad.ProyectoID.Equals(IdentidadActual.FilaIdentidad.ProyectoID) && identidad.Tipo == 3 && identidad.PerfilID.Equals(perfilOrgID));
                                        }
                                    }

                                    bool esRSS = this.Request.QueryString.ToString().StartsWith("?rss") && this.Request.QueryString.ToString().Contains("&tiporss=");

                                    if ((estaUsuarioEnProyecto) && (mControladorBase.UsuarioActual.EsIdentidadInvitada) && (!IdentidadActual.OrganizacionID.HasValue || !EsIdentidadActualAdministradorOrganizacion || estaOrganizacionEnProy || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado))
                                    {
                                        //El usuario tiene acceso con otro perfil, y no es administrador de la organización (o lo es pero la comunidad es privada o reservada), le cambio a él
                                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                        GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadDePersonaEnProyecto(ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.PersonaID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                                        identidadCN.Dispose();

                                        if (gestorIdentidades != null)
                                        {
                                            //Obtengo la fila de la identidad con la que tiene acceso al proyecto
                                            //"ProyectoID = '" + ProyectoSeleccionado.Clave + "' AND IdentidadID <> '" + mControladorBase.UsuarioActual.IdentidadID.ToString() + "' AND Tipo <> 3 AND FechaBaja IS NULL AND FechaExpulsion IS NULL"
                                            List<AD.EntityModel.Models.IdentidadDS.Identidad> filas = gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(ProyectoSeleccionado.Clave) && identidad.IdentidadID.Equals(mControladorBase.UsuarioActual.IdentidadID) && identidad.Tipo != 3 && !identidad.FechaBaja.HasValue && !identidad.FechaExpulsion.HasValue).ToList();

                                            if ((filas != null) && (filas.Count > 0))
                                            {
                                                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = filas.First();

                                                mControladorBase.CambiarPerfilUsuario(filaIdentidad);
                                                perfilCambiado = true;
                                            }
                                        }
                                    }

                                    if (!esRSS && !EstaEnLoginComunidad && !perfilCambiado)
                                    {
                                        //El usuario no tiene acceso a este proyecto, le llevo a la home de la comunidad
                                        string redirect = ObtenerUrlRedirect(ref url);

                                        //if (!MaximoRedireccionesExcedidas())
                                        {
                                            string urlComunidadLogin = BaseURLIdioma + "/" + UtilIdiomas.GetText("URLSEM", "LOGIN");
                                            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                                            {
                                                urlComunidadLogin = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);

                                                if (mControladorBase.UsuarioActual.EsUsuarioInvitado || !ProyectoSeleccionado.EsPublico || !ParametrosGeneralesRow.HomeVisible)
                                                {
                                                    urlComunidadLogin += "/" + UtilIdiomas.GetText("URLSEM", "LOGIN");
                                                }
                                                else
                                                {
                                                    redirect = "";
                                                }
                                            }

                                            //Si el redirect ya contiene esta cadena, significa que ya se le ha redireccionado a login (ya estamos en esa página, no hay que volver a redireccionar)
                                            if (!redirect.Contains(urlComunidadLogin) && (!redirect.Contains("/login/redirect")) && (!redirect.EndsWith("/login")))
                                            {
                                                if (!this.GetType().Name.Equals("LogoutController") && !this.GetType().Name.Equals("LoadingController"))
                                                {
                                                    redirect = "";
                                                }

                                                //Realiza la redirección
                                                pFilterContext.Result = Redirect(urlComunidadLogin + redirect);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            string requestUrl = this.Request.Path.ToString();
            if (requestUrl.IndexOf('?') > 0)
            {
                requestUrl = requestUrl.Remove(requestUrl.IndexOf('?'));
            }

            //Comprobamos si el proyecto está en definición.
            bool permisoProyCerrado = (ProyectoSeleccionado.Estado == (short)EstadoProyecto.CerradoTemporalmente && !ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID));
            bool permisoProyDefinicion = mControladorBase.UsuarioActual != null && mControladorBase.UsuarioActual.EsIdentidadInvitada && ProyectoSeleccionado.Estado == (short)EstadoProyecto.Definicion && string.IsNullOrEmpty(mControladorBase.RequestParams("peticionComID"));
            bool permisoProyPrivado = mControladorBase.UsuarioActual != null && mControladorBase.UsuarioActual.EsIdentidadInvitada && !ProyectoSeleccionado.EsPublico && string.IsNullOrEmpty(mControladorBase.RequestParams("peticionComID"));
            bool paginaPermitidaInvitado = requestUrl.Contains("/login") || requestUrl.Contains("/registro-tutor") || requestUrl.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "OLVIDEPASSWORD")) || requestUrl.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "COMPLETARREGISTRO")) || requestUrl.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "CAMBIARPASSWORD")) || requestUrl.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "ESTABLECERPASSWORD")) || requestUrl.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "ACEPTARCOOKIES")) || requestUrl.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "POLITICACOOKIES")) || requestUrl.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "CONDICIONESUSO")) || requestUrl.Contains(mControladorBase.UtilIdiomas.GetText("URLSEM", "POLITICAPRIVACIDAD")) || requestUrl.Contains("&token") || PaginaVisibleEnPrivada;

            //Si 
            if ((permisoProyCerrado || permisoProyDefinicion || permisoProyPrivado) && !paginaPermitidaInvitado)
            {
                string urlRedirect = BaseURLIdioma + "/home";
                if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                {
                    urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto);
                    if (!ParametrosGeneralesRow.HomeVisible)
                    {
                        urlRedirect += "/login";
                    }

                    string redirect = ObtenerUrlRedirect(ref url);
                    if (!string.IsNullOrEmpty(redirect))
                    {
                        urlRedirect += redirect;
                    }
                }

                //Evitamos que se entre en un bucle infinito...
                if (!string.IsNullOrEmpty(urlRedirect) && !requestUrl.Equals(urlRedirect))
                {
                    pFilterContext.Result = Redirect(urlRedirect);
                }
            }
        }

        /// <summary>
        /// Carga las identidades completas (identidad,persona y organizacion) a partir de una lista de guids de identidad
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidad</param>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        /// <param name="pDataWrapperPersona">Dataset de personas</param>
        /// <param name="pOrganizacionDW">Dataset de organizaciones</param>
        public void ObtenerIdentidadesPorID(List<Guid> pListaIdentidades, DataWrapperIdentidad pDataWrapperIdentidad, DataWrapperPersona pDataWrapperPersona, DataWrapperOrganizacion pOrganizacionDW)
        {
            if (mFicheroConfiguracionBD == string.Empty)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pDataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesPorID(pListaIdentidades, false));
                identidadCN.Dispose();


                if (pDataWrapperPersona != null)
                {
                    PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    //pDataWrapperPersona.Merge(persCN.ObtenerPersonasIdentidadesCargadas(pIdentidadDS), true);
                    pDataWrapperPersona.Merge(persCN.ObtenerPersonasIdentidadesCargadas(pDataWrapperIdentidad));
                    persCN.Dispose();
                }
            }
            else
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pDataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesPorID(pListaIdentidades, false));
                identidadCN.Dispose();



                if (pDataWrapperPersona != null)
                {
                    PersonaCN persCN = new PersonaCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    //pDataWrapperPersona.Merge(persCN.ObtenerPersonasIdentidadesCargadas(pIdentidadDS), true);
                    pDataWrapperPersona.Merge(persCN.ObtenerPersonasIdentidadesCargadas(pDataWrapperIdentidad));
                    persCN.Dispose();
                }
            }
        }

        private string ObtenerUrlRedirect(ref string pUrl)
        {
            string redirect = "";

            if (pUrl.Contains(BaseURLSinHTTP))
            {
                pUrl = pUrl.Remove(0, pUrl.IndexOf(BaseURLSinHTTP) + BaseURLSinHTTP.Length);
            }

            if (pUrl.Length > 3 && !pUrl.Equals("/home"))
            {
                if (!pUrl.StartsWith("/"))
                {
                    if (pUrl.StartsWith("http"))
                    {
                        pUrl = System.Net.WebUtility.UrlEncode(pUrl);
                    }
                    pUrl = "/" + pUrl;
                }

                //Redirige al login de gnoss genérico
                redirect = "/redirect" + pUrl;

            }

            return redirect;
        }

        private bool ParticipaUsuarioEnProyecto()
        {
            return mControladorBase.UsuarioActual != null && !mControladorBase.UsuarioActual.EsIdentidadInvitada && !mControladorBase.EstaUsuarioBloqueadoProyecto;
        }

        /// <summary>
        /// Comprueba si el usuario está intentando acceder a un proyecto con url propia, en cuyo caso le redirecciona a esa url
        /// </summary>
        private void ComprobarRedireccionProyectoConUrlPropia(string pUrlPropiaProyecto, ActionExecutingContext pFilterContext)
        {

            if (!string.IsNullOrEmpty(pUrlPropiaProyecto) && mControladorBase.DominoAplicacion != null && !mControladorBase.DominoAplicacion.Equals(ControladorBase.ObtenerDominioUrl(new Uri(pUrlPropiaProyecto), false)))
            {
                //Obtengo la URL a la que el usuario intenta acceder
                string url = this.Request.Path.ToString();

                if (!url.ToLower().Contains("visualizardocumento") && !url.ToLower().Contains("download-file") && !url.ToLower().Contains("visualizarontologia.aspx") && !(this.GetType().Name.Equals("LoadingController")) && !(this.GetType().Name.Equals("CkeditorController")) && !(this.GetType().Name.Equals("AceptarCookieController")) && !(this.GetType().Name.Equals("CrearCookieController")) && !(this.GetType().Name.Equals("EliminarCookieController")))
                {
                    if (!url.ToLower().Contains("anyadirgnoss.aspx?addtognoss=") && (url.Contains("?")))
                    {
                        url = url.Remove(url.IndexOf('?'));
                    }
                    url = url.Replace("\n", "").Replace("\t", "");

                    if (url.Contains(mControladorBase.HTTP))
                    {
                        url = url.Remove(0, mControladorBase.HTTP.Length);
                        url = url.Remove(0, url.IndexOf("/"));
                    }

                    if (url.Contains(BaseURLSinHTTP))
                    {
                        url = url.Remove(0, url.IndexOf(BaseURLSinHTTP) + BaseURLSinHTTP.Length);
                    }
                    pUrlPropiaProyecto = UtilDominios.DomainToPunyCode(pUrlPropiaProyecto);


                    //if (!MaximoRedireccionesExcedidas())
                    {
                        //redirigimos a la URL del proyecto 
                        pFilterContext.Result = RedirectPermanent(pUrlPropiaProyecto + url);
                    }
                }
            }
        }

        /// <summary>
        /// Si el dominio hace referencia a una comunidad con url propia, compruebo que el usuario no se esté saliendo de la comunidad hacia otra comunidad o hacia MyGNOSS. Si es así, le redirigo a la url de mygnoss o de la comunidad en su caso.
        /// </summary>
        private void ComprobarRedireccionAMyGnoss(string pUrlPropiaProyecto, ActionExecutingContext pFilterContext)
        {
            //Comprobamos si el dominio al que vamos corresponde a un dominio diferente
            bool esUnDominioDiferente = (!string.IsNullOrEmpty(pUrlPropiaProyecto) && mControladorBase.DominoAplicacion != null && !mControladorBase.DominoAplicacion.Equals(ControladorBase.ObtenerDominioUrl(new Uri(pUrlPropiaProyecto), false)));
            if ((ProyectoSeleccionado != null) && (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto)) && esUnDominioDiferente && !(this.GetType().Name.Equals("LoadingController")) && !(this.GetType().Name.Equals("CkeditorController")) && !(this.GetType().Name.Equals("AceptarCookieController")) && !(this.GetType().Name.Equals("CrearCookieController")) && !(this.GetType().Name.Equals("EliminarCookieController")))
            {
                mLoggingService.AgregarEntrada("Entra en ComprobarRedireccionAMyGnoss");

                pFilterContext.Result = Redirect(ProyectoSeleccionado.UrlPropia(IdiomaUsuario) + Request.Path);
            }
        }

        private UserIdentityModel CargarDatosIdentidadActual()
        {
            UserIdentityModel identidad = CargarDatosIdentidad(IdentidadActual);

            if (identidad.IsGuestIdentity && ProyectoSeleccionado.FilaProyecto.TipoAcceso == (short)TipoAcceso.Restringido)
            {
                if (new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).TieneUsuarioSolicitudPendienteEnProyecto(mControladorBase.UsuarioActual.UsuarioID, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, IdentidadActual.PerfilID))
                {
                    identidad.CommunityRequestStatus = UserIdentityModel.CommunityRequestStatusEnum.RequestPending;
                }
                else if (new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).TieneUsuarioSolicitudPendienteEnProyecto(mControladorBase.UsuarioActual.UsuarioID, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave))
                {
                    identidad.CommunityRequestStatus = UserIdentityModel.CommunityRequestStatusEnum.RequestedWithAnotherProfile;
                }
            }

            identidad.IdentityGroups = CargarGruposIdentidadActual();

            CargarDatosExtraIdentidad(identidad);

            return identidad;
        }

        private List<GroupCardModel> CargarGruposIdentidadActual()
        {
            List<GroupCardModel> listaGrupos = new List<GroupCardModel>();

            if (ListaGruposIdentidadActual.Count > 0)
            {
                listaGrupos = ControladorProyectoMVC.ObtenerGruposPorID(ListaGruposIdentidadActual).Values.ToList();
            }

            return listaGrupos;
        }

        private UserProfileModel CargarDatosPerfil()
        {
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

            UserProfileModel perfil = (UserProfileModel)identidadCL.ObtenerPerfilMVC(IdentidadActual.PerfilID);

            if (perfil == null)
            {
                perfil = new UserProfileModel();

                Guid identidadID = IdentidadActual.Clave;
                Identidad identidadFoto = IdentidadActual;
                string identidadNombreCorto = IdentidadActual.NombreCorto;
                string identidadNombreCompuesto = IdentidadActual.NombreCompuesto();
                string identidadNombreCortoOrg = "";

                if (ProyectoVirtual.Clave == ProyectoAD.MetaProyecto || mControladorBase.UsuarioActual.EsIdentidadInvitada)
                {
                    if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
                    {
                        identidadID = IdentidadActual.PerfilUsuario.IdentidadMyGNOSS.Clave;
                        identidadFoto = IdentidadActual.PerfilUsuario.IdentidadMyGNOSS;
                        identidadNombreCorto = IdentidadActual.NombreCorto;
                        identidadNombreCompuesto = IdentidadActual.NombreCompuesto();
                    }
                }
                else if (IdentidadActual.TrabajaConOrganizacion && !IdentidadActual.EsIdentidadProfesor)
                {
                    identidadNombreCorto = IdentidadActual.IdentidadOrganizacion.NombreCorto;
                    identidadNombreCompuesto = IdentidadActual.NombreCompuesto();
                }

                perfil.Key = IdentidadActual.PerfilID;
                perfil.Url = UrlPerfil; //Despues se construye en el idioma indicado
                perfil.Name = identidadNombreCorto;
                perfil.NameOrg = identidadNombreCortoOrg;
                perfil.PersonName = IdentidadActual.NombreCorto;
                perfil.CompleteProfileName = identidadNombreCompuesto;
                perfil.ExtraData = new Dictionary<string, string>();
                perfil.ExtraDataIdentities = new Dictionary<Guid, Dictionary<string, string>>();
                perfil.BornDate = IdentidadActual.Persona.Fecha;

                if (IdentidadActual.Persona != null)
                {
                    if (!string.IsNullOrEmpty(IdentidadActual.Persona.ValorDocumentoAcreditativo))
                    {
                        perfil.DNI = IdentidadActual.Persona.ValorDocumentoAcreditativo;
                    }

                    perfil.PaisID = IdentidadActual.Persona.PaisID;
                    perfil.PersonName = IdentidadActual.Persona.Nombre;
                }

                perfil.Communities = new List<UserProfileModel.ProfileCommunitiesModel>();

                if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
                { return perfil; }

                string fotoPerfil = "/" + UtilArchivos.ContentImagenes + identidadFoto.UrlImagen;
                if (!string.IsNullOrEmpty(fotoPerfil) && !fotoPerfil.Contains("sinfoto") && !fotoPerfil.Contains("anonimo"))
                {
                    perfil.Foto = fotoPerfil;
                }
                //perfil.FotoPerfil = "/" + UtilArchivos.ContentImagenes + identidadFoto.UrlImagen;


                if (IdentidadActual.IdentidadPersonalMyGNOSS != null && IdentidadActual.PerfilUsuario.Clave == IdentidadActual.IdentidadPersonalMyGNOSS.PerfilUsuario.Clave && !IdentidadActual.IdentidadPersonalMyGNOSS.TrabajaConOrganizacion)
                {
                    perfil.TypeProfile = ProfileType.Personal;
                }
                else if (IdentidadActual.IdentidadProfesorMyGnoss != null && IdentidadActual.PerfilUsuario.Clave == IdentidadActual.IdentidadProfesorMyGnoss.PerfilUsuario.Clave)
                {
                    perfil.TypeProfile = ProfileType.Teacher;
                }
                else
                {
                    perfil.IsClassProfile = IdentidadActual.TrabajaConClase;
                    perfil.TypeProfile = ProfileType.ProfessionalPersonal;
                }

                bool esAdministradorOrg = false;
                bool esEditorOrg = false;

                if (!IdentidadActual.TrabajaConClase && IdentidadActual.OrganizacionID != null)
                {
                    esAdministradorOrg = mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, (Guid)IdentidadActual.OrganizacionID);
                    esEditorOrg = mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion, (Guid)IdentidadActual.OrganizacionID);
                }
                perfil.OrganizationMenuAvailable = (esAdministradorOrg || esEditorOrg);

                perfil.IsAdministrator = esAdministradorOrg;

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                AD.EncapsuladoDatos.DataWrapperProyecto dataSetComunidades = proyCN.ObtenerProyectosParticipaPerfil(IdentidadActual.PerfilID);

                List<Guid> ListaProyectosSinRegistro = proyCN.ObtenerListaIDsProyectosSinRegistroObligatorio();
                proyCN.Dispose();
                //Proyecto.NombreCorto, Proyecto.Nombre, Proyecto.TipoAcceso, Identidad.NumConnexiones
                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoNumConexiones fila in dataSetComunidades.ListaProyectoNumConexiones)
                {
                    //Comprobar que la clave del proyecto no este en la tabla ProyectoSinRegistroObligatorio.
                    bool estaEnTablaSinRegistroObligatorio = false;

                    Guid clave = fila.ProyectoID;
                    string nombreCorto = fila.NombreCorto;
                    string nombre = fila.Nombre;
                    short tipoAcceso = fila.TipoAcceso;
                    int numConnexiones = fila.NumConexiones;
                    short tipoProyecto = fila.TipoProyecto;

                    estaEnTablaSinRegistroObligatorio = EstaEnListaSinRegistroObligatorio(clave, ListaProyectosSinRegistro);
                    if (!estaEnTablaSinRegistroObligatorio)
                    {
                        UserProfileModel.ProfileCommunitiesModel comunidad = new UserProfileModel.ProfileCommunitiesModel();
                        comunidad.Key = clave;
                        comunidad.Name = UtilCadenas.ObtenerTextoDeIdioma(nombre, UtilIdiomas.LanguageCode, IdiomaPorDefecto);
                        comunidad.Url = nombreCorto;
                        comunidad.Type = tipoAcceso;
                        comunidad.NumberOfConnections = numConnexiones;
                        comunidad.ProyectType = (CommunityModel.TypeProyect)tipoProyecto;
                        perfil.Communities.Add(comunidad);
                    }


                }

                CargarDatosExtraPerfil(perfil);

                perfil.IsClassAdministrator = EsAdministradorDeAlgunaClase;

                identidadCL.AgregarPerfilMVC(IdentidadActual.PerfilID, perfil);
            }

            if (perfil.OrganizationMenuAvailable)
            {
                perfil.OrganizationPersonalSpaceUrl = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, true);
                perfil.AdminOrganizationUsersUrl = GnossUrlsSemanticas.GetUrlAdministrarOrg(BaseURLIdioma, UtilIdiomas, UrlPerfil, UtilIdiomas.GetText("URLSEM", "USUARIOS"));
            }

            CargarPerfilesPerfil(perfil);

            List<string> listaNombresCortosProyectos = new List<string>();
            foreach (UserProfileModel.ProfileCommunitiesModel comunidad in perfil.Communities)
            {
                listaNombresCortosProyectos.Add(comunidad.Url);
            }

            Dictionary<string, string> listaNombresCortosUrl = mControladorBase.UrlsSemanticas.ObtenerURLComunidades(UtilIdiomas, BaseURLIdioma, listaNombresCortosProyectos);

            foreach (UserProfileModel.ProfileCommunitiesModel comunidad in perfil.Communities)
            {
                comunidad.Url = listaNombresCortosUrl[comunidad.Url];
            }

            //perfil = JsonConvert.DeserializeObject<UserProfileModel>(ReemplazarClavesCacheHTML(JsonConvert.SerializeObject(perfil)));

            return perfil;
        }

        private bool EstaEnListaSinRegistroObligatorio(Guid pProyectoID, List<Guid> pListaProyectoSinRegistroObligatorio)
        {
            bool estaEnLista = false;
            foreach (Guid proyectoIDSinRegistroObligatorio in pListaProyectoSinRegistroObligatorio)
            {
                if (pProyectoID.Equals(proyectoIDSinRegistroObligatorio))
                {
                    return true;
                }

            }
            return estaEnLista;
        }


        private void CargarDatosExtraPerfil(UserProfileModel pPerfil)
        {
            //Obtiene de BBDD los datosextra de las identidades que no estaban caché y los procesa para almacenarlos en caché
            MVCCN mvcCN = new MVCCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<AD.EntityModel.Models.ProyectoDS.DatoExtraFichasdentidad> listaDatosExtra = null;
            try
            {
                listaDatosExtra = mvcCN.ObtenerDatosExtraFichasIdentidadesPorPerfilesID(IdentidadActual.GestorIdentidades.ListaPerfiles.Keys.ToList());
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, " 20160209 Error mal controlado.");
            }

            if (listaDatosExtra != null)
            {
                foreach (AD.EntityModel.Models.ProyectoDS.DatoExtraFichasdentidad datoExtra in listaDatosExtra)
                {
                    Guid identID = datoExtra.IdentidadID;

                    string titulo = datoExtra.Titulo;
                    string opcion = datoExtra.Opcion;
                    Guid proyectoID = datoExtra.ProyectoID;

                    if (!pPerfil.ExtraData.ContainsKey(titulo) && proyectoID.Equals(Guid.Empty))
                    {
                        pPerfil.ExtraData.Add(titulo, opcion);
                    }

                    if (!pPerfil.ExtraDataIdentities.ContainsKey(proyectoID))
                    {
                        pPerfil.ExtraDataIdentities.Add(proyectoID, new Dictionary<string, string>());
                    }

                    if (!pPerfil.ExtraDataIdentities[proyectoID].ContainsKey(titulo))
                    {
                        pPerfil.ExtraDataIdentities[proyectoID].Add(titulo, opcion);
                    }
                }
            }
        }

        private void CargarDatosExtraIdentidad(UserIdentityModel pIdentidad)
        {
            if (Perfil.ExtraDataIdentities != null && Perfil.ExtraDataIdentities.ContainsKey(ProyectoSeleccionado.Clave))
            {
                pIdentidad.ExtraData = Perfil.ExtraDataIdentities[ProyectoSeleccionado.Clave];
            }
            pIdentidad.ExtraDataCommunities = Perfil.ExtraDataIdentities;
        }

        private void CargarPerfilesPerfil(UserProfileModel pPerfil)
        {
            pPerfil.UserProfiles = new List<UserProfileModel>();
            //"ProyectoID = '" + ProyectoAD.MetaProyecto + "' AND Tipo <> " + (short)TiposIdentidad.Organizacion, "NumConnexiones desc"
            List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidadMyGnossPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !identidad.Tipo.Equals((short)TiposIdentidad.Organizacion)).OrderByDescending(identidad => identidad.NumConnexiones).ToList();

            foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidadMyGnossPerfil in filasIdentidadMyGnossPerfil)
            {
                AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(filaIdentidadMyGnossPerfil.PerfilID));

                if (filaPerfil.PersonaID.HasValue)
                {
                    string nombre = filaPerfil.NombrePerfil;

                    UserProfileModel perfilExt = new UserProfileModel();

                    Guid identidadMyGnossPerfil = filaIdentidadMyGnossPerfil.IdentidadID;

                    string urlFoto = filaIdentidadMyGnossPerfil.Foto;

                    if (filaPerfil.OrganizacionID.HasValue)
                    {
                        nombre = filaPerfil.NombreOrganizacion;

                        AD.EntityModel.Models.IdentidadDS.Perfil filaPerfilOrg = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.OrganizacionID.Equals(filaPerfil.OrganizacionID) && !perfil.PersonaID.HasValue);
                        AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidadMyGnossOrg = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.FirstOrDefault(identidad => identidad.PerfilID.Equals(filaPerfilOrg.PerfilID) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto));

                        if (filaIdentidadMyGnossOrg != null)
                        {
                            identidadMyGnossPerfil = filaIdentidadMyGnossOrg.IdentidadID;
                            urlFoto = filaIdentidadMyGnossOrg.Foto;
                        }
                    }

                    if (!string.IsNullOrEmpty(urlFoto) && !urlFoto.Contains("sinfoto") && !urlFoto.Contains("anonimo"))
                    {
                        perfilExt.Foto = "/" + UtilArchivos.ContentImagenes + urlFoto;
                    }

                    perfilExt.Name = nombre;
                    perfilExt.Key = filaPerfil.PerfilID;

                    string urlIdent = "/";
                    if (!string.IsNullOrEmpty(filaPerfil.NombreCortoOrg))
                    {
                        urlIdent = "/" + UtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + filaPerfil.NombreCortoOrg + "/";
                    }
                    else if (IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Count(identidad => identidad.Tipo.Equals((short)ProfileType.Teacher) && identidad.PerfilID.Equals(filaPerfil.PerfilID)) > 0)
                    {
                        urlIdent = "/" + UtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + filaPerfil.NombreCortoUsu + "/";
                    }
                    perfilExt.Url = BaseURLIdioma + urlIdent + UtilIdiomas.GetText("URLSEM", "HOME");

                    if (IdentidadActual.IdentidadPersonalMyGNOSS != null && IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID == filaPerfil.PerfilID && !IdentidadActual.IdentidadPersonalMyGNOSS.TrabajaConOrganizacion)
                    {
                        perfilExt.TypeProfile = ProfileType.Personal;
                    }
                    else if (IdentidadActual.IdentidadProfesorMyGnoss != null && IdentidadActual.IdentidadProfesorMyGnoss.PerfilUsuario.Clave == filaPerfil.PerfilID)
                    {
                        perfilExt.TypeProfile = ProfileType.Teacher;
                    }
                    else
                    {
                        perfilExt.TypeProfile = ProfileType.ProfessionalPersonal;
                    }

                    pPerfil.UserProfiles.Add(perfilExt);
                }
            }
        }

        private AutenticationModel CargarFormularioRegistro()
        {
            AutenticationModel registro = null;

            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                registro = (AutenticationModel)proyCL.ObtenerFormularioRegistroMVC(ProyectoSeleccionado.Clave);
                RegistroController registroController = new RegistroController(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth);
                if (registro == null)
                {
                    registro = new AutenticationModel();
                    registro.TypePage = AutenticationModel.TypeAutenticationPage.Registro;
                    registroController.CargarCamposRegistroProyecto(registro);
                    registroController.CargarCamposExtraRegistro(registro);
                    registroController.CargarEdadMinimaRegistro(registro);
                    registroController.CargarClausulasRegistro(registro);
                    proyCL.AgregarFormularioRegistroMVC(ProyectoSeleccionado.Clave, registro);
                }
                proyCL.Dispose();

                if (registro != null && Request.HasFormContentType && Request.Form.Count == 0)
                {
                    registroController.EstablecerSeguridad(registro, Session, Response);
                }
            }

            return registro;
        }

        protected void CargarPersonalizacion(Guid pProyectoID, Guid pPersonalizacionEcosistemaID, bool pComunidadExcluidaPersonalizacionEcosistema)
        {
            CommunityModel comunidad = new CommunityModel();

            VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(pProyectoID, pPersonalizacionEcosistemaID, pComunidadExcluidaPersonalizacionEcosistema);

            comunidad.ListaPersonalizaciones = new List<string>();
            comunidad.ListaPersonalizacionesEcosistema = new List<string>();

            if (vistaVirtualDW.ListaVistaVirtual.Count > 0)
            {
                Guid personalizacionProyecto = Guid.Empty;
                if (vistaVirtualDW.ListaVistaVirtualProyecto.Count > 0)
                {
                    personalizacionProyecto = ((VistaVirtualProyecto)vistaVirtualDW.ListaVistaVirtualProyecto.FirstOrDefault()).ProyectoID;
                }

                if (personalizacionProyecto != Guid.Empty)
                {
                    foreach (VistaVirtual filaVistaVirtual in vistaVirtualDW.ListaVistaVirtual.Where(item => item.PersonalizacionID.Equals(personalizacionProyecto.ToString())))
                    {
                        comunidad.ListaPersonalizaciones.Add(filaVistaVirtual.TipoPagina);
                    }
                    foreach (VistaVirtualRecursos filaVistaVirtualRecurso in vistaVirtualDW.ListaVistaVirtualRecursos.Where(item => item.PersonalizacionID.Equals(personalizacionProyecto.ToString())))
                    {
                        comunidad.ListaPersonalizaciones.Add("/Views/FichaRecurso_" + filaVistaVirtualRecurso.RdfType + "/Index.cshtml");
                    }
                    foreach (VistaVirtualCMS filaVistaVirtualCMS in vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(personalizacionProyecto.ToString())))
                    {
                        comunidad.ListaPersonalizaciones.Add("/Views/CMSPagina/" + filaVistaVirtualCMS.PersonalizacionComponenteID + ".cshtml");
                    }
                    foreach (VistaVirtualGadgetRecursos filaVistaVirtualGadgetRecursos in vistaVirtualDW.ListaVistaVirtualGadgetRecursos.Where(item => item.PersonalizacionID.Equals(personalizacionProyecto.ToString())))
                    {
                        comunidad.ListaPersonalizaciones.Add("/Views/Shared/" + filaVistaVirtualGadgetRecursos.PersonalizacionComponenteID + ".cshtml");
                    }
                }
                if (mControladorBase.PersonalizacionEcosistemaID != Guid.Empty)
                {
                    foreach (VistaVirtual filaVistaVirtual in vistaVirtualDW.ListaVistaVirtual.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID.ToString())))
                    {
                        comunidad.ListaPersonalizacionesEcosistema.Add(filaVistaVirtual.TipoPagina);
                    }
                    foreach (VistaVirtualRecursos filaVistaVirtualRecurso in vistaVirtualDW.ListaVistaVirtualRecursos.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID.ToString())))
                    {
                        comunidad.ListaPersonalizacionesEcosistema.Add("/Views/FichaRecurso_" + filaVistaVirtualRecurso.RdfType + "/Index.cshtml");
                    }
                    foreach (VistaVirtualCMS filaVistaVirtualCMS in vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID.ToString())))
                    {
                        comunidad.ListaPersonalizacionesEcosistema.Add("/Views/CMSPagina/" + filaVistaVirtualCMS.PersonalizacionComponenteID + ".cshtml");
                    }
                    foreach (VistaVirtualGadgetRecursos filaVistaVirtualGadgetRecursos in vistaVirtualDW.ListaVistaVirtualGadgetRecursos.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID.ToString())))
                    {
                        comunidad.ListaPersonalizacionesEcosistema.Add("/Views/Shared/" + filaVistaVirtualGadgetRecursos.PersonalizacionComponenteID + ".cshtml");
                    }
                }
            }
            ViewBag.Comunidad = comunidad;

            string controllerName = this.ToString();
            controllerName = controllerName.Substring(controllerName.LastIndexOf('.') + 1);
            controllerName = controllerName.Substring(0, controllerName.IndexOf("Controller"));
            ViewBag.ControllerName = controllerName;

            ViewBag.CMSActivado = ParametrosGeneralesRow.CMSDisponible;
            ViewBag.VistasActivadas = ProyectoSeleccionado.PersonalizacionID != Guid.Empty;
        }

        protected virtual void CargarEnlacesMultiIdioma()
        {
            Dictionary<string, string> listaIdiomas = mConfigService.ObtenerListaIdiomasDictionary();
            Dictionary<string, KeyValuePair<bool, string>> listaEnlacesMultiIdioma = new Dictionary<string, KeyValuePair<bool, string>>();
            string urlMultiIdiomaPaginaActualAux = UrlMultiIdiomaPaginaActual;
            string[] parametros = urlMultiIdiomaPaginaActualAux.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string parametro in parametros)
            {
                if (parametro.StartsWith("{") && parametro.EndsWith("}"))
                {
                    string nombreParametro = parametro.Substring(1, parametro.Length - 2);
                    string valorParametro = RequestParams(nombreParametro);
                    if (!string.IsNullOrEmpty(nombreParametro))
                    {
                        urlMultiIdiomaPaginaActualAux = urlMultiIdiomaPaginaActualAux.Replace(parametro, valorParametro);
                    }
                }
            }
            foreach (string idioma in listaIdiomas.Keys)
            {
                Recursos.UtilIdiomas utilIdiomasActual = new Recursos.UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService);

                string rutaActual = BaseURL;
                bool obtenerIdioma = true;
                if (!ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto))
                {
                    obtenerIdioma = false;
                    //rutaActual = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(utilIdiomasActual, BaseURLIdioma, ProyectoVirtual.NombreCorto);
                    rutaActual = ObtenerUrlComunidadParaIdioma(idioma);
                }
                rutaActual += "/" + RouteConfig.ConvertirUrlAIdioma(urlMultiIdiomaPaginaActualAux, idioma, obtenerIdioma);
                listaEnlacesMultiIdioma.Add(idioma, new KeyValuePair<bool, string>(true, rutaActual));
            }

            mListaEnlacesMultiIdioma = listaEnlacesMultiIdioma;
        }

        private string ObtenerUrlComunidadParaIdioma(string pIdioma)
        {
            Dictionary<string, string> urlPorIdioma = UtilCadenas.ObtenerTextoPorIdiomas(ProyectoVirtual.FilaProyecto.URLPropia);
            string url = "";


            if (urlPorIdioma.Any(item => item.Key.Equals(pIdioma)))
            {
                url = urlPorIdioma[pIdioma];
            }
            else
            {
                if (urlPorIdioma.Count == 0)
                {
                    url = ProyectoVirtual.FilaProyecto.URLPropia;
                    if (!IdiomaPorDefecto.Equals(pIdioma))
                    {
                        url += $"/{pIdioma}";
                    }
                }
                else
                {
                    url = urlPorIdioma.First().Value;
                    url += $"/{pIdioma}";
                }
            }

            //if (!ParametroProyecto.ContainsKey(ParametroAD.ProyectoSinNombreCortoEnURL))
            //{
            //    url += $"/{UtilIdiomas.GetText("URLSEM", "COMUNIDAD")}/{ProyectoVirtual.NombreCorto}";
            //}

            return url;
        }

        protected virtual void CargarTituloPagina()
        {
            if (!ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                if (!string.IsNullOrEmpty(mTituloPagina))
                {
                    mTituloPagina = mTituloPagina + " - ";
                }
                if (!string.IsNullOrEmpty(ProyectoVirtual.FilaProyecto.NombrePresentacion))
                {
                    mTituloPagina = mTituloPagina + UtilCadenas.ObtenerTextoDeIdioma(ProyectoVirtual.FilaProyecto.NombrePresentacion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }
                else
                {
                    mTituloPagina = mTituloPagina + UtilCadenas.ObtenerTextoDeIdioma(ProyectoVirtual.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }
            }
            if (BaseURL.Contains("gnoss."))
            {
                if (!string.IsNullOrEmpty(mTituloPagina))
                {
                    mTituloPagina = mTituloPagina + " - ";
                }
                mTituloPagina = mTituloPagina + "GNOSS";
            }
        }

        protected virtual void ObtenerUrlCanonica()
        {
            string urlCanonicalTemp = "";
            if (ListaEnlacesMultiIdioma != null && ListaEnlacesMultiIdioma.ContainsKey(UtilIdiomas.LanguageCode))
            {
                string urlPaginaActualMultiLingual = ListaEnlacesMultiIdioma[UtilIdiomas.LanguageCode].Value;

                if (urlPaginaActualMultiLingual != UrlPaginaConParametros)
                {
                    if (!string.IsNullOrEmpty(ParametrosGeneralesRow.IdiomaDefecto))
                    {
                        urlCanonicalTemp = ListaEnlacesMultiIdioma[ParametrosGeneralesRow.IdiomaDefecto].Value;
                    }

                    if (string.IsNullOrEmpty(urlCanonicalTemp))
                    {
                        foreach (string idioma in ListaEnlacesMultiIdioma.Keys)
                        {
                            if (ListaEnlacesMultiIdioma[idioma].Key)
                            {
                                urlCanonicalTemp = ListaEnlacesMultiIdioma[idioma].Value;
                                break;
                            }
                        }
                    }
                    mUrlCanonical = urlCanonicalTemp;
                }
            }
        }

        public Dictionary<string, KeyValuePair<bool, string>> ListaEnlacesMultiIdioma
        {
            get
            {
                if (mListaEnlacesMultiIdioma == null)
                {
                    CargarEnlacesMultiIdioma();
                }
                return mListaEnlacesMultiIdioma;
            }
            set
            {
                mListaEnlacesMultiIdioma = value;
            }
        }

        public string TituloPagina
        {
            get
            {
                if (mTituloPagina == null)
                {
                    CargarTituloPagina();
                }
                return mTituloPagina;
            }
            set
            {
                mTituloPagina = value;
            }
        }

        public string UrlCanonical
        {
            get
            {
                if (string.IsNullOrEmpty(mUrlCanonical))
                {
                    ObtenerUrlCanonica();
                }
                return mUrlCanonical;
            }
            set
            {
                mUrlCanonical = value;
            }
        }

        protected List<CategoryModel> CargarTesauroPorBR(Guid pBaseRecursosID)
        {
            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperTesauro tesauroDW = tesauroCN.ObtenerTesauroPorBaseRecursosID(pBaseRecursosID);

            GestionTesauro gestorTesauro = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);

            return CargarTesauroPorGestorTesauro(gestorTesauro);
        }

        protected List<CategoryModel> CargarTesauroPerfil()
        {
            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperTesauro tesauroDW = tesauroCN.ObtenerTesauroUsuario(mControladorBase.UsuarioActual.UsuarioID);

            GestionTesauro gestorTesauro = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);
            AD.EntityModel.ParametroAplicacion filaParametroAplicacion = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == TiposParametrosAplicacion.DuracionCookieUsuario);
            //ParametroAplicacionDS.ParametroAplicacionRow filaParametroAplicacion = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro("UsarSoloCategoriasPrivadasEnEspacioPersonal");
            if (filaParametroAplicacion != null && (filaParametroAplicacion.Valor.Equals("1") || filaParametroAplicacion.Valor.Equals("true")))
            {
                gestorTesauro.EliminarCategoriasPublicasSiEsMetaEspacioGNOSS(UtilIdiomas.LanguageCode);
            }

            return CargarTesauroPorGestorTesauro(gestorTesauro);
        }

        /// <summary>
        /// Carga el modelo de tesauro a partir de un gestor de Tesaro.
        /// </summary>
        /// <param name="pGestorTesauro">Gestor de tesaro</param>
        /// <returns>Modelo de tesauro a partir de un gestor de Tesaro</returns>
        protected List<CategoryModel> CargarTesauroPorGestorTesauro(GestionTesauro pGestorTesauro)
        {
            List<CategoryModel> listaCategoriasTesauro = new List<CategoryModel>();

            foreach (CategoriaTesauro catTes in pGestorTesauro.ListaCategoriasTesauro.Values)
            {
                CategoryModel categoriaTesauro = CargarCategoria(catTes);
                listaCategoriasTesauro.Add(categoriaTesauro);
            }

            return listaCategoriasTesauro;
        }

        protected override CommunityModel CargarDatosComunidad()
        {
            CommunityModel comunidad = base.CargarDatosComunidad();
            bool cache = true;
            //Si comunidad.Administrators == null significa que el modelo no estaba en caché, tengo que cargar el resto de propiedades que necesita
            if ((comunidad.Administrators == null && cache) || (comunidad.Administrators.Count > 0 && string.IsNullOrEmpty(comunidad.Administrators.FirstOrDefault().NombreCortoUsuario) && string.IsNullOrEmpty(comunidad.Administrators.FirstOrDefault().NombreCortoOrganizacion)))
            {
                cache = false;
                comunidad.IsGnossOrganiza = EsGnossOrganiza;

                comunidad.CookieNotice = AvisoCookie;

                comunidad.ExternalUrlCookieNotice = UrlAvisoCookieExterno;

                comunidad.Copyright = TextoCopyright;
                comunidad.ClausesRegister = ClausulasRegistro;

                comunidad.ContactLink = EnlaceContacto;

                comunidad.BetaProject = ProyectoEnBeta;

                comunidad.CompactedMenu = MenuCompactado;

                comunidad.ProjectLoginConfiguration = ProyectoLoginConfiguracion;

                ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                List<Identidad> ListaAdministradores = controladorProyecto.CargarAdministradoresProyecto(ProyectoSeleccionado);

                List<Guid> listaIds = new List<Guid>();
                foreach (Identidad identidad in ListaAdministradores)
                {
                    if (!listaIds.Contains(identidad.Clave))
                    {
                        listaIds.Add(identidad.Clave);
                    }
                }

                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Dictionary<Guid, string> listaFotosIds = identCN.ObtenerSiListaIdentidadesTienenFoto(listaIds);
                identCN.Dispose();

                comunidad.Administrators = new List<ProfileModel>();
                foreach (Identidad identidad in ListaAdministradores)
                {
                    ProfileModel usuario = new ProfileModel();
                    usuario.NamePerson = identidad.Nombre();
                    usuario.UrlPerson = mControladorBase.UrlsSemanticas.GetURLPerfilPersonaOOrgEnProyecto(BaseURLIdioma, UtilIdiomas, UrlPerfil, identidad, ProyectoSeleccionado.NombreCorto);
                    usuario.NombreCortoOrganizacion = identidad.PerfilUsuario.FilaPerfil.NombreCortoOrg;
                    usuario.NombreCortoUsuario = identidad.PerfilUsuario.FilaPerfil.NombreCortoUsu;
                    string urlImagen = "";
                    string urlFoto = listaFotosIds[identidad.Clave];

                    if (!string.IsNullOrEmpty(urlFoto) && urlFoto.ToLower() != "sinfoto" && !urlFoto.ToLower().Contains("anonimo35_peque"))
                    {
                        if (!urlFoto.ToLower().StartsWith("/" + UtilArchivos.ContentImagenes))
                        {
                            urlFoto = "/" + UtilArchivos.ContentImagenes + urlFoto;
                        }
                        urlImagen = urlFoto;
                    }

                    usuario.UrlFoto = urlImagen;
                    usuario.TypeProfile = (ProfileType)identidad.FilaIdentidad.Tipo;

                    comunidad.Administrators.Add(usuario);
                }

                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.AgregarComunidadMVC(ProyectoSeleccionado.Clave, comunidad);
            }
            //No se actualizan las urls por el idioma ya que se guarda en cache la primera url creada con el idioma que se hubiera creado en ese momento por lo que hay que aztualizar las urls al coger de cache
            if (comunidad.Administrators != null && cache)
            {
                foreach (var admin in comunidad.Administrators)
                {
                    admin.UrlPerson = mControladorBase.UrlsSemanticas.GetURLPerfilPersonaOOrgEnProyecto(BaseURLIdioma, UtilIdiomas, UrlPerfil, ProyectoSeleccionado.NombreCorto, admin.NombreCortoUsuario, admin.NombreCortoOrganizacion, admin.TypeProfile);
                }
            }
            comunidad.Tabs = CargarPestañasMenuComunidad();

            comunidad.ModelMetabusqueda = CargarPestañasMetabusqueda();

            comunidad.ModelMetabusquedaJson = JsonConvert.SerializeObject(comunidad.ModelMetabusqueda);

            comunidad.RdfTypesExcluidos = JsonConvert.SerializeObject(CargarRDFSAExcluir());

            if (string.IsNullOrEmpty(RequestParams("callback")) && !ViewBag.ControllerName.Equals("Widget"))
            {
                CargarPermisosComunidad(comunidad);
            }

            ProcesarPrivacidadPestanyasMenuComunidad(comunidad.Tabs);

            if (ProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
            {
                ProcesarPestanyasComunidad(comunidad.Tabs, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

                BuscarPestanyaActiva(comunidad);
            }

            if (ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.HasValue)
            {
                comunidad.ParentKey = ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.Value;
            }

            return comunidad;
        }

        private List<string> CargarRDFSAExcluir()
        {
            List<string> lista = new List<string>();
            //Simple
            foreach (AD.EntityModel.Models.Faceta.OntologiaProyecto filaDoc in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaOntologiaProyecto)
            {
                if (!filaDoc.EsBuscable)
                {
                    lista.Add(filaDoc.Namespace);
                }

            }
            return lista;

        }


        /// <summary>
        /// Pinta la comunidad.
        /// </summary>
        private List<CommunityModel.PestaniaModelMetabusqueda> CargarPestañasMetabusqueda()
        {
            List<int> listasTipoPorDefecto = new List<int> { 2, 3, 4, 5, 6, 12 };
            List<CommunityModel.PestaniaModelMetabusqueda> listaPestanyas = new List<CommunityModel.PestaniaModelMetabusqueda>();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaBusqueda filaPestanyaBusqueda in ProyectoVirtual.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda.Where(proyectoPestanyaMenu => proyectoPestanyaMenu.ProyectoPestanyaMenu.ProyectoID.Equals(ProyectoSeleccionado.FilaProyecto.ProyectoID) && !listasTipoPorDefecto.Contains((int)proyectoPestanyaMenu.ProyectoPestanyaMenu.TipoPestanya)).ToList())
            {
                if (filaPestanyaBusqueda.ProyectoPestanyaMenu.Visible && filaPestanyaBusqueda.ProyectoPestanyaMenu.Activa)
                {
                    CommunityModel.PestaniaModelMetabusqueda pestaniaModel = new CommunityModel.PestaniaModelMetabusqueda();
                    pestaniaModel.Nombre = UtilCadenas.ObtenerTextoDeIdioma(filaPestanyaBusqueda.ProyectoPestanyaMenu.Nombre, UtilIdiomas.LanguageCode, "es");
                    pestaniaModel.CampoFiltro = filaPestanyaBusqueda.CampoFiltro;
                    pestaniaModel.PestanyaID = filaPestanyaBusqueda.PestanyaID;
                    pestaniaModel.TipoBusqueda = 0;
                    listaPestanyas.Add(pestaniaModel);
                }
            }
            /*
            Recursos = 2,
            Preguntas = 3,
            Debates = 4,
            Encuestas = 5,
            PersonasYOrganizaciones = 6,
            BusquedaAvanzada = 12,
            */

            /*
            /// <summary>
            /// Recursos
            /// </summary>
            Recursos = 0,
                /// <summary>
                /// Debates
                /// </summary>
                Debates,
                /// <summary>
                /// Preguntas
                /// </summary>
                Preguntas,
                /// <summary>
                /// Encuestas
                /// </summary>
                Encuestas,
                /// <summary>
                /// Dafos
                /// </summary>
                Dafos,
                /// <summary>
                /// Personas y organizaciones
                /// </summary>
                PersonasYOrganizaciones,
                /// <summary>
                /// Búsqueda avanzada
                /// </summary>
                BusquedaAvanzada
                */

            
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya in ProyectoVirtual.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proyPestanyaMenu => proyPestanyaMenu.PestanyaPadreID == null && listasTipoPorDefecto.Contains(proyPestanyaMenu.TipoPestanya)).OrderBy(proyPestanyaMenu => proyPestanyaMenu.Orden).ToList())
            {
                CommunityModel.PestaniaModelMetabusqueda pestaniaModel = new CommunityModel.PestaniaModelMetabusqueda();
                if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Recursos))
                {
                    pestaniaModel.Nombre = "Recursos";
                    pestaniaModel.TipoBusqueda = (int)TipoBusqueda.Recursos;
                    pestaniaModel.PestanyaID = filaPestanya.PestanyaID;
                }
                else if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Preguntas))
                {
                    pestaniaModel.Nombre = "Preguntas";
                    pestaniaModel.TipoBusqueda = (int)TipoBusqueda.Preguntas;
                    pestaniaModel.PestanyaID = filaPestanya.PestanyaID;
                }
                else if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Debates))
                {
                    pestaniaModel.Nombre = "Debates";
                    pestaniaModel.TipoBusqueda = (int)TipoBusqueda.Debates;
                    pestaniaModel.PestanyaID = filaPestanya.PestanyaID;
                }
                else if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Encuestas))
                {
                    pestaniaModel.Nombre = "Encuestas";
                    pestaniaModel.TipoBusqueda = (int)TipoBusqueda.Encuestas;
                    pestaniaModel.PestanyaID = filaPestanya.PestanyaID;
                }
                else if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.PersonasYOrganizaciones))
                {
                    pestaniaModel.Nombre = "PersonasYOrganizaciones";
                    pestaniaModel.TipoBusqueda = (int)TipoBusqueda.PersonasYOrganizaciones;
                    pestaniaModel.PestanyaID = filaPestanya.PestanyaID;
                }
                else if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.BusquedaAvanzada))
                {
                    pestaniaModel.Nombre = "BusquedaAvanzada";
                    pestaniaModel.TipoBusqueda = (int)TipoBusqueda.BusquedaAvanzada;
                    pestaniaModel.PestanyaID = filaPestanya.PestanyaID;
                }

                listaPestanyas.Add(pestaniaModel);
            }
            return listaPestanyas;
        }


        /// <summary>
        /// Pinta la comunidad.
        /// </summary>
        private List<CommunityModel.TabModel> CargarPestañasMenuComunidad()
        {
            return CargarPestanyas(ProyectoVirtual.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proyPestanyaMenu => proyPestanyaMenu.PestanyaPadreID == null).OrderBy(proyPestanyaMenu => proyPestanyaMenu.Orden).ToList());//("PestanyaPadreID IS NULL", "Orden ASC"));
        }

        private List<CommunityModel.TabModel> CargarPestanyas(List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> pPestanyas)
        {
            List<CommunityModel.TabModel> listaPestanyas = new List<CommunityModel.TabModel>();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya in pPestanyas)
            {
                CommunityModel.TabModel pestanya = new CommunityModel.TabModel();
                pestanya.Active = false;
                pestanya.Key = filaPestanya.PestanyaID;
                pestanya.Visible = filaPestanya.Visible;

                KeyValuePair<string, string> nameUrl = ObtenerNameUrlPestanya(filaPestanya);
                pestanya.Name = nameUrl.Key;
                pestanya.Url = nameUrl.Value;
                pestanya.OpenInNewWindow = filaPestanya.NuevaPestanya;

                List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> proyectosPestanyaMenu = ProyectoVirtual.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proyPestanyaMenu => proyPestanyaMenu.PestanyaPadreID.Equals(filaPestanya.PestanyaID)).ToList();
                if (proyectosPestanyaMenu.Count > 0)
                {
                    proyectosPestanyaMenu = proyectosPestanyaMenu.OrderBy(proyPestanyaMenu => proyPestanyaMenu.Orden).ToList();
                    pestanya.SubTab = CargarPestanyas(proyectosPestanyaMenu);
                }

                listaPestanyas.Add(pestanya);
            }
            return listaPestanyas;
        }

        public void ObtenerNameUrlPestanyaPorTipo(TipoPestanyaMenu pTipoPestanya, out string pNombre, out string pUrl)
        {
            pNombre = string.Empty;
            pUrl = string.Empty;

            switch (pTipoPestanya)
            {
                case TipoPestanyaMenu.Home:
                    pNombre = UtilIdiomas.GetText("COMMON", "HOME");
                    break;
                case TipoPestanyaMenu.Indice:
                    pNombre = UtilIdiomas.GetText("COMMON", "INDICE");
                    pUrl = UtilIdiomas.GetText("URLSEM", "INDICE");
                    break;
                case TipoPestanyaMenu.Recursos:
                    pNombre = UtilIdiomas.GetText("COMMON", "BASERECURSOS");
                    pUrl = UtilIdiomas.GetText("URLSEM", "RECURSOS");
                    break;
                case TipoPestanyaMenu.Preguntas:
                    pNombre = UtilIdiomas.GetText("COMMON", "PREGUNTAS");
                    pUrl = UtilIdiomas.GetText("URLSEM", "PREGUNTAS");
                    break;
                case TipoPestanyaMenu.Debates:
                    pNombre = UtilIdiomas.GetText("COMMON", "DEBATES");
                    pUrl = UtilIdiomas.GetText("URLSEM", "DEBATES");
                    break;
                case TipoPestanyaMenu.Encuestas:
                    pNombre = UtilIdiomas.GetText("COMMON", "ENCUESTAS");
                    pUrl = UtilIdiomas.GetText("URLSEM", "ENCUESTAS");
                    break;
                case TipoPestanyaMenu.PersonasYOrganizaciones:
                    pNombre = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES");
                    if (ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionExpandida || ProyectoVirtual.TipoProyecto == TipoProyecto.Universidad20 || ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionPrimaria)
                    {
                        pNombre = UtilIdiomas.GetText("COMMON", "PROFESORESYALUMNOS");
                    }
                    pUrl = UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES");
                    break;
                case TipoPestanyaMenu.AcercaDe:
                    pNombre = UtilIdiomas.GetText("COMMON", "ACERCADE");
                    pUrl = UtilIdiomas.GetText("URLSEM", "ACERCADE");
                    break;
                case TipoPestanyaMenu.Comunidades:
                    pNombre = UtilIdiomas.GetText("COMMON", "COMUNIDADES");
                    pUrl = UtilIdiomas.GetText("URLSEM", "COMUNIDADES");
                    break;
                case TipoPestanyaMenu.BusquedaAvanzada:
                    pNombre = UtilIdiomas.GetText("BUSCADORFACETADO", "TODALACOMUNIDAD");
                    pUrl = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    break;
            }
        }

        public KeyValuePair<string, string> ObtenerNameUrlPestanya(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pFilaPestanya, bool pObtenerMultiIdioma = false)
        {
            KeyValuePair<string, string> nameUrl = new KeyValuePair<string, string>();
            if (pFilaPestanya != null)
            {
                string name = string.Empty;
                string url = string.Empty;

                if (!pObtenerMultiIdioma)
                {
                    ObtenerNameUrlPestanyaPorTipo((TipoPestanyaMenu)pFilaPestanya.TipoPestanya, out name, out url);
                }
                else
                {
                    ObtenerNameUrlMultiIdiomaPestanyaPorTipo((TipoPestanyaMenu)pFilaPestanya.TipoPestanya, out name, out url);
                }

                if (!string.IsNullOrEmpty(pFilaPestanya.Nombre))
                {
                    name = pFilaPestanya.Nombre;
                }
                if (!string.IsNullOrEmpty(pFilaPestanya.Ruta))
                {
                    url = pFilaPestanya.Ruta;
                    if (pFilaPestanya.TipoPestanya == (short)TipoPestanyaMenu.EnlaceExterno)
                    {
                        url = "enlace_externo_" + url;
                    }
                }
                if (string.IsNullOrEmpty(url))
                {
                    url = "";
                }

                if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(url))
                {
                    nameUrl = new KeyValuePair<string, string>(name, url);
                }
            }
            return nameUrl;
        }

        public void ObtenerNameUrlMultiIdiomaPestanyaPorTipo(TipoPestanyaMenu pTipoPestanya, out string pNombre, out string pUrl)
        {
            ControladorPestanyas.ObtenerNameUrlMultiIdiomaPestanyaPorTipo(pTipoPestanya, out pNombre, out pUrl);
        }

        private void ProcesarPrivacidadPestanyasMenuComunidad(List<CommunityModel.TabModel> pTabs)
        {
            foreach (CommunityModel.TabModel tab in pTabs)
            {
                if (ProyectoVirtual.ListaPestanyasMenu.ContainsKey(tab.Key))
                {
                    ProyectoPestanyaMenu pestanya = ProyectoVirtual.ListaPestanyasMenu[tab.Key];
                    if (tab.Visible && !pestanya.VisibleSinAcceso && pestanya.Privacidad != TipoPrivacidadPagina.Normal)
                    {
                        if (pestanya.Privacidad == TipoPrivacidadPagina.Lectores)
                        {
                            if (IdentidadActual == null || mControladorBase.UsuarioActual.EsIdentidadInvitada)
                            {
                                tab.Visible = false;
                            }
                            else
                            {
                                bool perfilConPermiso = pestanya.ListaRolIdentidad.ContainsKey(IdentidadActual.PerfilID);
                                if (!perfilConPermiso)
                                {
                                    bool perfilEnGrupoConPermiso = false;
                                    foreach (Guid idGrupoIdentidad in ListaGruposPerfilEnProyectoVirtual)
                                    {
                                        if (pestanya.ListaRolGrupoIdentidades.ContainsKey(idGrupoIdentidad))
                                        {
                                            perfilEnGrupoConPermiso = true;
                                            break;
                                        }
                                    }
                                    if (!perfilEnGrupoConPermiso)
                                    {
                                        tab.Visible = false;
                                    }
                                }
                            }
                        }
                        else if (pestanya.Privacidad == TipoPrivacidadPagina.Especial)
                        {
                            if (IdentidadActual != null && mControladorBase.UsuarioActual.EsIdentidadInvitada && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido))
                            {
                                tab.Visible = false;
                            }
                        }
                    }
                }
                if (tab.SubTab != null)
                {
                    ProcesarPrivacidadPestanyasMenuComunidad(tab.SubTab);
                }
            }
        }

        private void ProcesarPestanyasComunidad(List<CommunityModel.TabModel> pListaPestanyas, string pLanguageCode, string pIdiomaDefecto)
        {
            List<CommunityModel.TabModel> pestanyasAEliminar = new List<CommunityModel.TabModel>();
            foreach (CommunityModel.TabModel pestanya in pListaPestanyas)
            {
                if (pestanya.Key != Guid.Empty)
                {
                    ProyectoPestanyaMenu pestanyaActual = ProyectoVirtual.ListaPestanyasMenu[pestanya.Key];
                    pestanya.Name = UtilCadenas.ObtenerTextoDeIdioma(pestanya.Name, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    bool externo = false;
                    if (pestanya.Url.StartsWith("enlace_externo_"))
                    {
                        pestanya.Url = pestanya.Url.Replace("enlace_externo_", "");
                        externo = true;
                    }
                    pestanya.Url = UtilCadenas.ObtenerTextoDeIdioma(pestanya.Url, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    if (!externo)
                    {
                        string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto);
                        if (!string.IsNullOrEmpty(pestanya.Url))
                        {
                            pestanya.Url = urlComunidad + "/" + pestanya.Url;
                        }
                        else
                        {
                            pestanya.Url = urlComunidad;
                        }
                    }

                    if (pestanya.SubTab != null)
                    {
                        ProcesarPestanyasComunidad(pestanya.SubTab, pLanguageCode, pIdiomaDefecto);
                    }
                    if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma))
                    {
                        if (!pestanyaActual.ListaIdiomasDisponibles(mConfigService.ObtenerListaIdiomasDictionary()).Contains(UtilIdiomas.LanguageCode))
                        {
                            pestanyasAEliminar.Add(pestanya);
                        }
                    }
                }
            }
            foreach (CommunityModel.TabModel pestaña in pestanyasAEliminar)
            {
                pListaPestanyas.Remove(pestaña);
            }

        }

        private void BuscarPestanyaActiva(CommunityModel pComunidad)
        {
            string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto);
            urlComunidad = urlComunidad.Replace("https","").Replace("http", "");
            string absoluteUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}").AbsoluteUri.Replace("https", "").Replace("http", "");
            string urlCompleta = absoluteUri.Replace(urlComunidad, "");
            var pestanyaActiva = MarcarPestanyaActiva(pComunidad.Tabs, urlCompleta);

            while (pestanyaActiva == null && urlCompleta.Contains('&'))
            {
                urlCompleta = urlCompleta.Substring(0, urlCompleta.LastIndexOf('&'));
                pestanyaActiva = MarcarPestanyaActiva(pComunidad.Tabs, urlCompleta);
            }

            if (pestanyaActiva == null && urlCompleta.Contains('?'))
            {
                urlCompleta = urlCompleta.Substring(0, urlCompleta.LastIndexOf('?'));
                pestanyaActiva = MarcarPestanyaActiva(pComunidad.Tabs, urlCompleta);
            }

            //string pagActual = HttpUtility.UrlDecode(UrlPagina);
            //pestanyaActiva = MarcarPestanyaActiva(pComunidad.Tabs, pagActual);
        }


        private CommunityModel.TabModel MarcarPestanyaActiva(List<CommunityModel.TabModel> pListaPestanyas, string pRuta)
        {
            CommunityModel.TabModel pestanyaActiva = null;
            pestanyaActiva = pListaPestanyas.Find(pestanya => !string.IsNullOrEmpty(pestanya.Url) && pestanya.Url.EndsWith(pRuta));
            if (pestanyaActiva == null)
            {
                foreach (CommunityModel.TabModel pestanya in pListaPestanyas)
                {
                    if (pestanya.SubTab != null)
                    {
                        pestanyaActiva = MarcarPestanyaActiva(pestanya.SubTab, pRuta);
                        if (pestanyaActiva != null)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                pestanyaActiva.Active = true;
            }
            return pestanyaActiva;
        }

        private void CargarPermisosComunidad(CommunityModel pComunidad)
        {
            CommunityModel.PermissionsModel permisosComunidad = new CommunityModel.PermissionsModel();

            permisosComunidad.CreateWidget = false;
            permisosComunidad.CreateResource = false;
            permisosComunidad.Invite = false;
            permisosComunidad.MaxMembersExceeded = false;
            permisosComunidad.EditBio = false;
            permisosComunidad.Manage = false;
            permisosComunidad.ManageCMS = false;
            permisosComunidad.CreateGroup = false;
            permisosComunidad.ManageRequestAccess = false;
            permisosComunidad.ManageRequestGroup = false;
            permisosComunidad.SendNewsletter = false;
            permisosComunidad.ManageRSSFeeds = false;
            permisosComunidad.LeaveCommunity = false;

            permisosComunidad.DocumentPermissions = new List<ResourceModel.DocumentType>();

            permisosComunidad.OntologyPermissionsNameUrls = new SortedDictionary<string, KeyValuePair<string, string>>();
            ;

            if (ParticipaUsuarioEnProyecto())
            {
                if (ProyectoVirtual.TipoAcceso != TipoAcceso.Privado && ProyectoVirtual.TipoAcceso != TipoAcceso.Reservado)
                {
                    permisosComunidad.CreateWidget = true;
                }

                bool tieneCategorias = true;

                if (ProyectoVirtual.Estado == (short)EstadoProyecto.Definicion)
                {
                    TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    tieneCategorias = tesauroCL.ObtenerTesauroDeProyecto(ProyectoVirtual.Clave).ListaCategoriaTesauro.Count > 0;
                    tesauroCL.Dispose();
                }

                if (tieneCategorias)
                {

                    foreach (TiposDocumentacion tipoDoc in ListaPermisosDocumentos)
                    {
                        switch (tipoDoc)
                        {
                            case TiposDocumentacion.Encuesta:
                                permisosComunidad.CreatePoll = true;
                                break;
                            case TiposDocumentacion.Debate:
                                permisosComunidad.CreateDebate = true;
                                break;
                            case TiposDocumentacion.Pregunta:
                                permisosComunidad.CreateQuestion = true;
                                break;
                            case TiposDocumentacion.Blog:
                            case TiposDocumentacion.DafoProyecto:
                                break;
                            default:
                                permisosComunidad.DocumentPermissions.Add((ResourceModel.DocumentType)(short)tipoDoc);
                                break;
                        }
                    }

                    if (ProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
                    {
                        Guid proyectoOntologiasID = ProyectoVirtual.Clave;

                        if (ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
                        {
                            proyectoOntologiasID = new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
                        }

                        DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        GestorDocumental gestorDocumental = new GestorDocumental(docCL.ObtenerOntologiasProyecto(proyectoOntologiasID, true), mLoggingService, mEntityContext);

                        List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = gestorDocumental.DataWrapperDocumentacion.ListaDocumento.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList();

                        //Simple
                        foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in filasDoc)
                        {
                            if (ComprobarPermisoEnOntologiaDeProyectoEIdentidad(filaDoc.DocumentoID))
                            {
                                Dictionary<string, string> listaPropiedades = UtilCadenas.ObtenerPropiedadesDeTexto(filaDoc.NombreElementoVinculado);
                                bool documentoVirtual = false;
                                if (listaPropiedades.ContainsKey(PropiedadesOntologia.urlservicio.ToString()))
                                {
                                    documentoVirtual = true;
                                }

                                ////Comprobamos si hay una ontología de otro proyecto. En el momento en el que se puedan compartir ontologías entre proyectos, habrá que cambiar esta comprobación, ya que se cumplirá cuando no debería.
                                //if (filaDoc.ProyectoID != mControladorBase.UsuarioActual.ProyectoID)
                                //{
                                //    mHayOntologiaOtroProyecto = true;
                                //}

                                string tituloOntologia = UtilCadenas.ObtenerTextoDeIdioma(filaDoc.Titulo, UtilIdiomas.LanguageCode, "es");

                                Identidad IdentidadOrganizacion = null;
                                if (!string.IsNullOrEmpty(RequestParams("organizacion")))
                                {
                                    IdentidadOrganizacion = IdentidadActual.IdentidadOrganizacion;
                                }

                                if (listaPropiedades.ContainsKey(PropiedadesOntologia.cargasmultiples.ToString()) && listaPropiedades[PropiedadesOntologia.cargasmultiples.ToString()] == "true")
                                {
                                    permisosComunidad.OntologyPermissionsNameUrls.Add(tituloOntologia, new KeyValuePair<string, string>(mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumento(BaseURLIdioma, UtilIdiomas, ProyectoVirtual.NombreCorto, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), (IdentidadOrganizacion != null), documentoVirtual), mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumentoMultiple(BaseURLIdioma, UtilIdiomas, ProyectoVirtual.NombreCorto, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), (IdentidadOrganizacion != null))));
                                }
                                else
                                {
                                    permisosComunidad.OntologyPermissionsNameUrls.Add(tituloOntologia, new KeyValuePair<string, string>(mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumento(BaseURLIdioma, UtilIdiomas, ProyectoVirtual.NombreCorto, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), (IdentidadOrganizacion != null), documentoVirtual), null));
                                }
                            }
                        }
                    }

                    bool tienePermisoVariosTiposRecursos = permisosComunidad.DocumentPermissions.Count > 1;
                    bool tienePermisoRecursoNoSemantico = permisosComunidad.DocumentPermissions.Count == 1 && !permisosComunidad.DocumentPermissions[0].Equals(ResourceModel.DocumentType.Semantico);
                    bool tienePermisoOntologias = permisosComunidad.DocumentPermissions.Count == 1 && permisosComunidad.DocumentPermissions[0].Equals(ResourceModel.DocumentType.Semantico) && permisosComunidad.OntologyPermissionsNameUrls.Count > 0;
                    permisosComunidad.CreateResource = (tienePermisoVariosTiposRecursos || tienePermisoRecursoNoSemantico || tienePermisoOntologias);
                    //permisosComunidad.CreateResource = (permisosComunidad.DocumentPermissions.Count > 0 || tienePermisoOntologias);
                }

                if (ParametrosGeneralesVirtualRow.InvitacionesDisponibles || ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    permisosComunidad.Invite = true;

                    if ((ProyectoVirtual.Estado == (short)EstadoProyecto.Definicion) && ContadoresProyecto.NumeroMiembros > 4)
                    {
                        permisosComunidad.MaxMembersExceeded = true;
                    }
                }

                permisosComunidad.Subscribe = true;

                if (IdentidadActual.Tipo != TiposIdentidad.ProfesionalCorporativo && IdentidadActual.Tipo != TiposIdentidad.Organizacion && ParametrosGeneralesVirtualRow.BiosCortas && !ProyectoVirtual.EsCatalogo && ProyectoPrincipalUnico == ProyectoAD.MetaProyecto)
                {
                    permisosComunidad.EditBio = true;
                }

                if (EsUsuarioAdministradorProyectoVirtual)
                {
                    permisosComunidad.Manage = true;
                    permisosComunidad.CreateGroup = true;

                    if (ParametrosGeneralesVirtualRow.CMSDisponible)
                    {
                        permisosComunidad.ManageCMS = true;
                    }
                }
                else if (EsIdentidadActualSupervisorProyecto && ParametrosGeneralesVirtualRow.CMSDisponible && PermisosPaginasAdministracion.AdministracionPaginasPermitido)
                {
                    permisosComunidad.ManageCMS = true;
                }

                SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (ProyectoVirtual.TipoAcceso == TipoAcceso.Restringido)
                {
                    if (EsUsuarioAdministradorProyectoVirtual || (ParametrosGeneralesVirtualRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))
                    {
                        permisosComunidad.ManageRequestAccess = true;
                        pComunidad.SolicitudesPendientes = solicitudCN.ObtenerNumeroSolicitudesAccesoProyectoPorProyecto(mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.ProyectoID);
                    }
                }

                if (ProyectoVirtual.Estado != (short)EstadoProyecto.Definicion && ProyectoVirtual.Estado != (short)EstadoProyecto.CerradoTemporalmente)
                {
                    if (EsUsuarioAdministradorProyectoVirtual || (ParametrosGeneralesVirtualRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))
                    {
                        permisosComunidad.ManageRequestGroup = true;

                        pComunidad.SolicitudesGrupoPendientes = solicitudCN.ObtenerNumeroSolicitudesPertenecerGruposDeProyecto(mControladorBase.UsuarioActual.ProyectoID);
                    }
                }

                solicitudCN.Dispose();

                if ((IdentidadActual.ModoParticipacion == TiposIdentidad.Personal) || (IdentidadActual.ModoParticipacion == TiposIdentidad.Profesor))
                {
                    permisosComunidad.LeaveCommunity = true;
                }
            }

            if (EsUsuarioAdministradorProyectoVirtual)
            {
                if (ProyectoVirtual.Estado != (short)EstadoProyecto.Definicion && ProyectoVirtual.Estado != (short)EstadoProyecto.CerradoTemporalmente)
                {
                    permisosComunidad.SendNewsletter = true;
                }
            }

            pComunidad.Permissions = permisosComunidad;
        }

        /// <summary>
        /// Carga todo lo necesario para que se conecte el usuario invitado y agrega a la sesión el usuario invitado
        /// </summary>
        private void CrearUsuarioInvitado()
        {
            mControladorBase.CrearUsuarioInvitado();
        }

        /// <summary>
        /// Obtiene los parámetros extra para una búsqueda con proyecto origen.
        /// </summary>
        /// <returns></returns>
        public string ObtenerParametrosExtraBusquedaConProyOrigen()
        {
            return mControladorBase.ObtenerParametrosExtraBusquedaConProyOrigen();
        }


        /// <summary>
        /// Inserta el Script encargado de llamar a los servicios de carga de facetas y de resultados
        /// </summary>
        /// <param name="pPanFacetasID">ID del panel de facetas</param>
        /// <param name="pPanResultadosID">ID del panel de resultados</param>
        /// <param name="pDivNumResultadosBusquedaID">ID del div del nñumero de resultados</param>
        /// <param name="pPanListadoFiltrosPulgarcitoID">ID del panel de los filtros</param>
        /// <param name="pUpdResultadosID"></param>
        /// <param name="pDivFiltrosID"></param>
        /// <param name="pGrafo"></param>
        /// <param name="pNavegadorID"></param>
        /// <param name="pInstruccionesExtra">Otras instrucciones Javascript que se quieran incluir en este bloque</param>
        /// <param name="pSuplementoFiltros">Suplemento que se añade después de # en todos los filtros (Ej: En contribuciones: #contribuciones|sioc_t:Tag=web)</param>
        /// <param name="pParametrosAdiccionales">Patámetros adiccionales para ésta búsqueda (no intruducidos por el usuario)</param>
        /// <param name="pIdentidadOrganizacion">Identidad de organización para hacer la petición en la BR de organización</param>
        protected void insertarScriptBuscador(string pPanFacetasID, string pPanResultadosID, string pDivNumResultadosBusquedaID, string pPanListadoFiltrosPulgarcitoID, string pUpdResultadosID, string pDivFiltrosID, string pGrafo, string pNavegadorID, string pInstruccionesExtra, string pSuplementoFiltros, string pParametrosAdiccionales, string pOrdenPorDefecto, string pOrdenEnSearch, string pFiltroContexto, string pMasJavascript, Guid pIdentidadBusqueda, Guid pProyectoID, string pUbicacionBusqueda, short pTipoBusqueda, AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaBusqueda pFilaPestanya, Guid? pIdentidadOrganizacion, bool pEsVistaMapa = false, Guid? pTokenAfinidad = null)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("origenAutoCompletar = '" + OrigenAutoCompletarPagina + "';");

            string urlComunidad = "";
            if (Comunidad != null)
            {
                urlComunidad = Comunidad.Url;
            }
            sb.AppendLine("var urlComunidad = '" + urlComunidad + "';");

            if (!pTipoBusqueda.Equals((short)TipoBusqueda.Contribuciones))
            {
                sb.AppendLine("var urlCargarAccionesRecursos = '" + ViewBag.UrlLoadResourceActions + "';");
            }

            sb.AppendLine("var panFacetas = '" + pPanFacetasID + "';");
            sb.AppendLine("var panResultados = '" + pPanResultadosID + "';");
            sb.AppendLine("var numResultadosBusq = '" + pDivNumResultadosBusquedaID + "';");
            sb.AppendLine("var panFiltrosPulgarcito = '" + pPanListadoFiltrosPulgarcitoID + "';");
            sb.AppendLine("var updResultados = '" + pUpdResultadosID + "';");
            sb.AppendLine("var divFiltros = '" + pDivFiltrosID + "';");

            sb.AppendLine("var ubicacionBusqueda = '" + pUbicacionBusqueda + "';");//TODO

            sb.AppendLine("var grafo = '" + pGrafo + "';");

            if (pIdentidadOrganizacion.HasValue)
            {
                sb.AppendLine("var identOrg = '" + pIdentidadOrganizacion.Value + "';");
            }

            bool verTodasPersonas = AdministradorQuiereVerTodasLasPersonas;

            if (AdministradorQuiereVerTodasLasPersonas ||
                (pTipoBusqueda.Equals((short)TipoBusqueda.Contribuciones) && (!IdentidadActual.OrganizacionID.HasValue && !string.IsNullOrEmpty(RequestParams("nombreCortoPerfil")) && RequestParams("nombreCortoPerfil").Equals(IdentidadActual.PerfilUsuario.NombreCortoUsu))) ||
                (pTipoBusqueda.Equals((short)TipoBusqueda.Contribuciones) && IdentidadActual.OrganizacionID.HasValue && EsIdentidadActualAdministradorOrganizacion && RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true")) ||
                 pTipoBusqueda.Equals((short)TipoBusqueda.EditarRecursosPerfil))
            {
                verTodasPersonas = true;
            }

            sb.AppendLine("var adminVePersonas = '" + verTodasPersonas.ToString() + "';");
            sb.AppendLine("var tipoBusqeda = " + (short)pTipoBusqueda + ";");
            sb.AppendLine("var idNavegadorBusqueda = '" + pNavegadorID + "';");

            sb.AppendLine("var ordenPorDefecto = '" + pOrdenPorDefecto + "';");
            sb.AppendLine("var ordenEnSearch = '" + pOrdenEnSearch + "';");
            sb.AppendLine("var filtroContexto = \"" + pFiltroContexto + "\";");

            sb.AppendLine("var tiempoEsperaResultados = 0;");

            if (!string.IsNullOrEmpty(pSuplementoFiltros) && !pSuplementoFiltros.EndsWith("|"))
            {
                pSuplementoFiltros += "|";
            }
            sb.AppendLine("var suplementoFiltros = '" + pSuplementoFiltros + "';");

            if (!string.IsNullOrEmpty(pParametrosAdiccionales))
            {
                //sb.AppendLine("var parametros_adiccionales = '" + Uri.EscapeDataString(pParametrosAdiccionales) + "';");
                sb.AppendLine("var parametros_adiccionales = '" + pParametrosAdiccionales + "';");
            }
            else
            {
                sb.AppendLine("var parametros_adiccionales = '';");
            }

            string mostrarFacetas = "true";
            string mostrarCajaBusqueda = "true";

            if (pFilaPestanya != null)
            {
                mostrarFacetas = pFilaPestanya.MostrarFacetas.ToString().ToLower();
                mostrarCajaBusqueda = pFilaPestanya.MostrarCajaBusqueda.ToString().ToLower();
            }

            sb.AppendLine("var mostrarFacetas = " + mostrarFacetas + ";");

            sb.AppendLine("var mostrarCajaBusqueda = " + mostrarCajaBusqueda + ";");

            //if (pInstruccionesExtra.Contains("txtBusquedaInt"))
            //{
            //    ((GnossMasterPage)this.Master).BodyReference.Attributes.Add("onload", "if(txtBusquedaInt != null && document.getElementById(txtBusquedaInt) != null && document.getElementById(txtBusquedaInt).value == ''){ document.getElementById(txtBusquedaInt).focus();}");
            //}

            string codigoRegistrarVisita = mControladorBase.CodigoCompletoGoogleAnalytics;

            string googleAnalyticsActivo = "false";

            if (!string.IsNullOrEmpty(codigoRegistrarVisita) && !string.IsNullOrEmpty(pInstruccionesExtra) && pInstruccionesExtra.Contains("FinalizarMontarResultados"))
            {
                mControladorBase.CodigoGoogleAnalyticsBusquedaEstablecido = true;

                //vuelvo a establecer el código de google analytics porque si no en las páginas de búsqueda se hacen dos peticiones a google analytics
                ViewBag.GoogleAnalytics = mControladorBase.CodigoCompletoGoogleAnalytics;

                googleAnalyticsActivo = "true";

                //cojo sólo la parte del registro de la visita
                codigoRegistrarVisita = codigoRegistrarVisita.Substring(codigoRegistrarVisita.IndexOf("<script type=\"text/javascript\">")).Replace("<script type=\"text/javascript\">", "");
                //sustituyo la función pageTracker._trackPageview(); para pasarle como parámetro la url de la página a la que está accediendo
                codigoRegistrarVisita = codigoRegistrarVisita.Replace("pageTracker._trackPageview();", "pageTracker._trackPageview(window.location.pathname + window.location.hash);");

                sb.AppendLine("function RegistrarVisitaGoogleAnalytics() ");
                sb.AppendLine("{");
                sb.AppendLine("if (typeof _gat !== \"undefined\" && _gat !== null) {");
                sb.AppendLine(codigoRegistrarVisita);
                sb.AppendLine("}}");
            }

            //Esta instrucción debe ser la última, las variables tienen que estar todas definidas arriba
            if (!string.IsNullOrEmpty(pInstruccionesExtra))
            {
                sb.AppendLine(pInstruccionesExtra);
            }

            sb.AppendLine("var googleAnalyticsActivo = " + googleAnalyticsActivo + ";");
            sb.AppendLine(pMasJavascript);

            bool omitirCargaInicialFacetasResultados = false;
            if (mControladorBase.ProyectoPestanyaActual != null && mControladorBase.ProyectoPestanyaActual.FilaProyectoPestanyaBusqueda != null)
            {
                omitirCargaInicialFacetasResultados = mControladorBase.ProyectoPestanyaActual.FilaProyectoPestanyaBusqueda.OmitirCargaInicialFacetasResultados;
            }
            //mFilaPestanyaBusqueda.OmitirCargaInicialFacetasResultados

            if (!string.IsNullOrEmpty(pPanFacetasID) && !pEsVistaMapa && pTipoBusqueda != (short)TipoBusqueda.Mensajes && pTipoBusqueda != (short)TipoBusqueda.Comentarios && pTipoBusqueda != (short)TipoBusqueda.Invitaciones && pTipoBusqueda != (short)TipoBusqueda.Notificaciones && (pTipoBusqueda != (short)TipoBusqueda.PersonasYOrganizaciones || !mControladorBase.UsuarioActual.EsUsuarioInvitado) && !omitirCargaInicialFacetasResultados)
            {
                string tokenAfinidad = "";
                if (pTokenAfinidad != null)
                {
                    tokenAfinidad = pTokenAfinidad.Value.ToString();
                }

                sb.AppendLine("$(document).ready(function () { MontarFacetas(filtrosDeInicio , true, 2, '#" + pPanFacetasID + "', null, '" + tokenAfinidad + "'); });");
            }
            else if (omitirCargaInicialFacetasResultados)
            {
                sb.AppendLine("primeraCargaDeFacetas = false;");
            }

            ViewBag.JSExtra = sb.ToString();
        }

        /// <summary>
        /// Redirecciona a pagina no encontrada.
        /// </summary>
        /// <returns>Redirección a la página 404</returns>
        public ActionResult RedireccionarAPaginaNoEncontrada()
        {
            return RedireccionarAPaginaNoEncontrada("200");
        }

        /// <summary>
        /// Devuelve un RedirectResult a la Url pasada como parámetro 
        /// </summary>
        /// <param name="pUrl">Url a la que redireccionar</param>
        /// <returns></returns>
        public ActionResult DevolverActionResult(string pUrl)
        {
            return GnossResultNoLogin(pUrl);
        }

        /// <summary>
        /// Devuelve el ActionResult de la vista seleccionada
        /// </summary>
        /// <param name="pViewName">Vista a cargar</param>
        /// <returns></returns>
        public ActionResult DevolverVista(string pViewName)
        {
            return View(pViewName);
        }

        /// <summary>
        /// Devuelve el ActionResult de la vista seleccionada
        /// </summary>
        /// <param name="pViewName">Vista a cargar</param>
        /// <param name="pModel">Model para pasar a la vista</param>
        /// <returns></returns>
        public ActionResult DevolverVista(string pViewName, object pModel)
        {
            return View(pViewName, pModel);
        }

        /// <summary>
        /// Redirecciona a pagina no encontrada.
        /// </summary>
        /// <returns>Redirección a la página 404</returns>
        public ActionResult RedireccionarAPaginaNoEncontrada(string pStatusCode)
        {
            if (!RedireccionadoA404)
            {
                RedireccionadoA404 = true;

                //string urlTransfer = "";

                //if (ProyectoSeleccionado.NombreCorto != RouteConfig.NombreProyectoSinNombreCorto)
                //{
                //    urlTransfer += "/" + UtilIdiomas.GetText("URLSEM", "COMUNIDAD") + "/" + ProyectoSeleccionado.NombreCorto;
                //}
                //urlTransfer += "/error?errorCode=404&statuscode=" + pStatusCode + "&urlOriginal=" + HttpContext.Context.Request.Path.ToString();
                //if (!UtilIdiomas.LanguageCode.Equals("es"))
                //{
                //    urlTransfer = "/" + UtilIdiomas.LanguageCode + urlTransfer;
                //}
                //HttpContext.Server.TransferRequest(urlTransfer, true);

                //return View("~/Views/Error/Error404.cshtml", ErrorController.Error404(this, false));                
                return ViewError404(new ErrorController(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth).Error404(this, false));
            }
            return new EmptyResult();
        }

        public void CargarJSGraficasGoogle()
        {
            if (!mJSGraficasGoogleCargado)
            {
                ViewBag.ListaJS.Add("//www.google.com/jsapi?key=" /* TODO Javier + Conexion.ObtenerParametro("Config/services.config", "config/claveApiGoogle", false)*/);
                mJSGraficasGoogleCargado = true;
            }
        }

        protected void CargarGeneradorURLs()
        {
            string urlProyecto = "";

            if ((ProyectoSeleccionado != null) && (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto)))
            {
                urlProyecto = ProyectoSeleccionado.NombreCorto;
            }

            string urlPagConParametros = UrlPagina;

            if (!string.IsNullOrEmpty(this.Request.GetDisplayUrl()) && this.Request.GetDisplayUrl().Contains("?"))
            {
                urlPagConParametros += this.Request.GetDisplayUrl().Substring(this.Request.GetDisplayUrl().IndexOf("?"));

                if (urlPagConParametros.Contains("?rdf&"))
                {
                    urlPagConParametros = urlPagConParametros.Replace("?rdf&", "?");
                }
                else if (urlPagConParametros.Contains("?rdf"))
                {
                    urlPagConParametros = urlPagConParametros.Replace("?rdf", "");
                }
                else if (urlPagConParametros.Contains("&rdf"))
                {
                    urlPagConParametros = urlPagConParametros.Replace("&rdf", "");
                }
            }

            GestionOWLGnoss.GeneradorUrls = new GnossGeneradorUrlsRDF(UtilIdiomas, BaseURLIdioma, BaseURLContent, UrlPerfil, urlProyecto, UrlPagina, urlPagConParametros, null, UrlsSemanticas, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor);
        }

        /// <summary>
        /// Codifica una cadena de caractéres para poder añadirla a una URL
        /// </summary>
        /// <param name="pTexto">Texto a codificar</param>
        /// <returns></returns>
        public string UrlEncode(string pTexto)
        {
            string textoEncoded = "";
            if (!string.IsNullOrEmpty(pTexto))
            {
                // Los espacios se transforman en '+', y al hacer un decode, no se transforman en un espacio, se siguen quedando en '+'
                // Si se sustituye por %20, el decode sí que se hace bien. 

                // Lo que hacemos aquí es sustituir el espacio por un texto comodín para que no lo reemplace por el signo '+'
                // y luego transofrmar ese texto comodín por un %20

                textoEncoded = pTexto.Replace(" ", "--ESPACIO--");
                textoEncoded = System.Net.WebUtility.UrlEncode(textoEncoded);
                textoEncoded = textoEncoded.Replace("--ESPACIO--", "%20");
            }
            return textoEncoded;
        }


        /// <summary>
        /// Obtiene el gestor tesauro de la BR actual.
        /// </summary>
        /// <returns>Gestor tesauro de la BR actual</returns>
        protected GestionTesauro CargarGestorTesauroBRActual()
        {
            TesauroCN tesCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperTesauro tesDW = null;

            if (RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true"))
            {
                tesDW = tesCN.ObtenerTesauroOrganizacion(IdentidadActual.OrganizacionPerfil.Clave);
            }
            else
            {
                tesDW = tesCN.ObtenerTesauroUsuario(mControladorBase.UsuarioActual.UsuarioID);
            }

            tesCN.Dispose();

            GestionTesauro gestorTes = new GestionTesauro(tesDW, mLoggingService, mEntityContext);
            AD.EntityModel.ParametroAplicacion filaParametroAplicacion = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == "UsarSoloCategoriasPrivadasEnEspacioPersonal");
            //ParametroAplicacionDS.ParametroAplicacionRow filaParametroAplicacion = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro("UsarSoloCategoriasPrivadasEnEspacioPersonal");
            if (filaParametroAplicacion != null && (filaParametroAplicacion.Valor.Equals("1") || filaParametroAplicacion.Valor.Equals("true")))
            {
                //TODO Juan : ¿Eliminar tambien las categorias hijas?
                gestorTes.EliminarCategoriasPublicasSiEsMetaEspacioGNOSS(UtilIdiomas.LanguageCode);
            }

            short i = 0;
            foreach (CategoriaTesauro catPadrePrimerNivel in gestorTes.ListaCategoriasTesauroPrimerNivel.Values)
            {
                catPadrePrimerNivel.FilaCategoria.Orden = i;
                i++;
            }

            foreach (CategoriaTesauro catPadre in gestorTes.ListaCategoriasTesauro.Values)
            {
                if (!gestorTes.ListaCategoriasTesauroPrimerNivel.ContainsKey(catPadre.Clave))
                {
                    catPadre.FilaCategoria.Orden = 0;
                }

                short j = 0;
                foreach (IElementoGnoss catHija in catPadre.Hijos)
                {
                    ((CategoriaTesauro)catHija).FilaAgregacion.Orden = j;
                    j++;
                }
            }

            return gestorTes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public bool ParticipaUsuarioActualEnProyecto(Guid pProyectoID)
        {
            if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(identidad => identidad.ProyectoID.Equals(pProyectoID) && identidad.Tipo != 3);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Guid IdentidadUsuarioActualEnProyecto(Guid pProyectoID)
        {
            if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                //Problema al traer la identidad de Ana, trae 2 identidades, la de organización y la suya personal fallando al tratar de obtener la PersonaID de la organización más adelante.
                //Bug original: https://redprivada.gnoss.com/comunidad/soporteserviciognoss/recurso/Autores-de-Papertoy-no-dirigen-bien-a-mis-museos/3308b8e3-ef3c-40c7-ac8f-eacf8d9ac906
                List<AD.EntityModel.Models.IdentidadDS.Identidad> drArray = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(pProyectoID)).ToList();
                if (drArray.Count > 1)
                {
                    AD.EntityModel.Models.IdentidadDS.Identidad filaIDentidad = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.FirstOrDefault(identidad => identidad.ProyectoID.Equals(pProyectoID) && !identidad.Tipo.Equals(3));
                    if (filaIDentidad != null)
                    {
                        return filaIDentidad.IdentidadID;
                    }

                }
                else
                {
                    if (drArray[0] != null)
                    {
                        return drArray[0].IdentidadID;
                    }
                }

            }
            return Guid.Empty;
        }

        /// <summary>
        /// Carga los dataset de persona y organizaciones a partir de un dataset de identidades
        /// </summary>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        /// <param name="pDataWrapperPersona">DataWrapperPersona de personas que queremos cargar</param>
        /// <param name="pOrganizacionDW">Dataset de organizaciones que queremos cargar</param>
        public void ObtenerPersonasYOrgDeIdentidades(DataWrapperIdentidad pDataWrapperIdentidad, DataWrapperPersona pDataWrapperPersona, DataWrapperOrganizacion pOrganizacionDW)
        {
            ObtenerPersonasYOrgDeIdentidades(pDataWrapperIdentidad, pDataWrapperPersona, pOrganizacionDW, false);
        }

        /// <summary>
        /// Carga los dataset de persona y organizaciones a partir de un dataset de identidades
        /// </summary>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        /// <param name="pDataWarpperPersona">Dataset de personas que queremos cargar</param>
        /// <param name="pOrganizacionDW">Dataset de organizaciones que queremos cargar</param>
        /// <param name="pCargaLigera">TRUE para usar cargas ligeras, FALSE en caso contrario</param>
        public void ObtenerPersonasYOrgDeIdentidades(DataWrapperIdentidad pDataWrapperIdentidad, DataWrapperPersona pDataWarpperPersona, DataWrapperOrganizacion pOrganizacionDW, bool pCargaLigera)
        {
            List<Guid> listaOrganizaciones = new List<Guid>();
            List<Guid> listaPersonas = new List<Guid>();
            OrganizacionCN orgCN;
            PersonaCN persCN;

            foreach (AD.EntityModel.Models.IdentidadDS.Perfil fila in pDataWrapperIdentidad.ListaPerfil)
            {
                if (fila.OrganizacionID.HasValue && !listaOrganizaciones.Contains(fila.OrganizacionID.Value))
                {
                    listaOrganizaciones.Add(fila.OrganizacionID.Value);
                }
                if (fila.PersonaID.HasValue && !listaPersonas.Contains(fila.PersonaID.Value))
                {
                    listaPersonas.Add(fila.PersonaID.Value);
                }
            }

            if (mFicheroConfiguracionBD == string.Empty)
            {
                orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                orgCN = new OrganizacionCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                persCN = new PersonaCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            if (!pCargaLigera)
            {
                pOrganizacionDW.Merge(orgCN.ObtenerOrganizacionesPorID(listaOrganizaciones));
                pDataWarpperPersona.Merge(persCN.ObtenerPersonasPorID(listaPersonas));
            }
            else
            {
                pOrganizacionDW.Merge(orgCN.ObtenerOrganizacionesPorIDCargaLigera(listaOrganizaciones));
                pDataWarpperPersona.ListaPersona = pDataWarpperPersona.ListaPersona.Union(persCN.ObtenerPersonasPorIDCargaLigera(listaPersonas)).ToList();
            }

            persCN.Dispose();
            orgCN.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void EliminarPersonalizacionVistas()
        {
            Comunidad.VersionJS = null;
            Comunidad.VersionCSS = null;

            ViewBag.Personalizacion = string.Empty;
            ViewBag.PersonalizacionEcosistema = string.Empty;
            ViewBag.PersonalizacionLayout = string.Empty;

            //RouteValueTransformer.RecalcularRutasProyecto(ProyectoSeleccionado.FilaProyecto.ProyectoID, mEntityContext);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void CargarPermisosAdministracionComunidadEnViewBag()
        {
            if (string.IsNullOrEmpty(RequestParams("new-community-wizard")))
            {
                // El parámetro new-community-wizard indica que está en el asistente de creación de comunidad
                // En ese caso, no sacamos el menú de adminstración para que sigua los pasos del asistente. 
                ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            }
            //TODO FRAN: No se debe hacer en el controllerBase.
            Comunidad.IntegracionContinuaActivada = false;
            Comunidad.IntContActivadaSinRamaEnUso = false;
            Comunidad.UsuarioDadoAltaIntCont = true;
            Comunidad.EntornoEsPre = false;
            Comunidad.EntornoBloqueado = false;
            Comunidad.EntornoEsPro = false;

            //TODO FRAN: No se debe hacer en el controllerBase.
            if (HayIntegracionContinua)
            {
                Comunidad.IntegracionContinuaActivada = true;
                Comunidad.RamaActualGit = RamaEnUsoGit;
                Comunidad.VersionRamaRelease = VersionRama;

                if (String.Equals(Comunidad.RamaActualGit, "") || (String.Equals(Comunidad.RamaActualGit, "develop")))
                {
                    if (!EntornoActualEsPreproduccion && !EntornoActualEsPruebas)
                    {
                        Comunidad.IntContActivadaSinRamaEnUso = false;
                    }
                    else
                    {
                        Comunidad.IntContActivadaSinRamaEnUso = true;
                    }
                }
                if (!UsuarioEstaDadoDeAltaEnIntCont)
                {
                    Comunidad.UsuarioDadoAltaIntCont = false;
                }
                if (EntornoActualEsPreproduccion)
                {
                    Comunidad.EntornoEsPre = true;
                }

                if (!Comunidad.EntornoEsPre && !EntornoActualEsPruebas)
                {
                    Comunidad.EntornoEsPro = true;
                }

                if (EntornoActualBloqueado)
                {
                    Comunidad.EntornoBloqueado = true;
                }
            }
        }

        private PermisosPaginasAdministracionViewModel CargarPermisosAdministracionComunidad()
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            bool esAdministradorProyecto = mControladorBase.ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID);

            bool esAdministradorMetaProyecto = proyCN.EsUsuarioAdministradorProyecto(mControladorBase.UsuarioActual.UsuarioID, ProyectoAD.MetaProyecto);

            List<TipoPaginaAdministracion> listaPermisos = new List<TipoPaginaAdministracion>();
            bool tienePermisosSupervisor = false;
            if (!esAdministradorProyecto)
            {
                List<PermisosPaginasUsuarios> listaPermisosPagina = proyCN.ObtenerPermisosPaginasProyectoUsuarioID(mControladorBase.ProyectoSeleccionado.FilaProyecto.OrganizacionID, mControladorBase.ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID);

                foreach (short pag in listaPermisosPagina.Select(fila => fila.Pagina).ToList())
                {
                    listaPermisos.Add((TipoPaginaAdministracion)pag);
                }

                tienePermisosSupervisor = listaPermisos.Count > 0;
            }

            proyCN.Dispose();

            //Inicializamos el objeto con todos los permisos a false
            PermisosPaginasAdministracionViewModel permisosPaginasModel = new PermisosPaginasAdministracionViewModel();

            if (esAdministradorProyecto || tienePermisosSupervisor)
            {
                //CargarPermisos
                permisosPaginasModel.CMSActivado = ParametrosGeneralesRow.CMSDisponible;

                permisosPaginasModel.AdministracionSemanticaPermitido = obtenerParametroBooleano("AdministracionSemanticaPermitido");
                permisosPaginasModel.AdministracionPaginasPermitido = obtenerParametroBooleano("AdministracionPaginasPermitido");
                permisosPaginasModel.AdministracionVistasPermitido = obtenerParametroBooleano("AdministracionVistasPermitido");
                permisosPaginasModel.AdministracionDesarrolladoresPermitido = obtenerParametroBooleano("AdministracionDesarrolladoresPermitido");

                permisosPaginasModel.AdministracionEventosDisponible = ParametrosGeneralesRow.EventosDisponibles;
                permisosPaginasModel.AdministracionIntegracionContinua = !string.IsNullOrEmpty(UrlApiIntegracionContinua);
                // Hacer peticion al api de despliegues para comprobar que esta iniciado.
                if (!string.IsNullOrEmpty(UrlApiIntegracionContinua))
                {
                    try
                    {
                        bool iniciado = false;
                        iniciado = mUtilServicioIntegracionContinua.EstaEnBD(ProyectoSeleccionado.NombreCorto, UrlApiIntegracionContinua);
                        permisosPaginasModel.AdministracionIntegracionContinuaIniciada = iniciado;
                    }
                    catch (Exception ex)
                    {
                        permisosPaginasModel.AdministracionIntegracionContinuaIniciada = false;
                    }
                }

                permisosPaginasModel.EsMetaAdministrador = esAdministradorMetaProyecto;
                permisosPaginasModel.VistasActivadas = ProyectoSeleccionado.PersonalizacionID != Guid.Empty;
                permisosPaginasModel.EsAdministradorProyecto = esAdministradorProyecto;

                if (tienePermisosSupervisor)
                {
                    permisosPaginasModel.PaginasPermisosUsuarios = listaPermisos;
                }
            }

            return permisosPaginasModel;
        }

        /// <summary>
        /// Obtiene la cookie de SesionUsuarioActiva
        /// </summary>
        /// <returns></returns>
        protected void ObtenerCookieSesionUsuarioActiva()
        {
            CookieOptions options = new CookieOptions();

            //establezco la validez inicial de la cookie que será de 1 día

            DateTime caduca = DateTime.UtcNow.AddDays(1);
            List<AD.EntityModel.ParametroAplicacion> filas = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.DuracionCookieUsuario).ToList();
            //DataRow[] filas = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.DuracionCookieUsuario + "'");
            if (filas != null && filas.Count > 0)
            {
                string duracion = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.DuracionCookieUsuario).FirstOrDefault().Valor;
                if (!string.IsNullOrEmpty(duracion))
                {
                    string letra = duracion.Substring(duracion.Length - 1).ToLower();
                    string digitos = duracion.Substring(0, duracion.Length - 1);
                    int cantidad;
                    if (int.TryParse(digitos, out cantidad) && cantidad > 0)
                    {
                        switch (letra)
                        {
                            case "d":
                                caduca = DateTime.UtcNow.AddDays(cantidad);
                                break;
                            case "h":
                                caduca = DateTime.UtcNow.AddHours(cantidad);
                                break;
                            case "m":
                                caduca = DateTime.UtcNow.AddMinutes(cantidad);
                                break;
                            default:
                                caduca = DateTime.UtcNow.AddDays(1);
                                break;
                        }
                    }
                }
            }
            options.Expires = caduca;
            Response.Cookies.Append("SesionUsuarioActiva", caduca.ToString("yyyy/MM/dd HH:mm:ss"), options);
        }

        /// <summary>
        /// Obtiene la cookie de SesionUsuarioActiva
        /// </summary>
        /// <returns></returns>
        private void ObtenerCookieIdiomaActual(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is PeticionesAJAXController || filterContext.Controller is CMSPaginaController)
            {
                return;
            }

            string idioma = RequestParams("lang");

            if (!string.IsNullOrEmpty(idioma))
            {
                Response.Cookies.Append("IdiomaActual", idioma, new CookieOptions { Expires = DateTime.UtcNow.AddDays(30) });
            }
        }

        #endregion

        #region Propiedades
        public bool EsAdministracionEcosistema
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("ecosistema")) && RequestParams("ecosistema").Equals("true"))
                {
                    ViewBag.PersonalizacionAdministracionEcosistema = mControladorBase.PersonalizacionEcosistemaID;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public Guid ProyectoSeleccionadoError404
        {
            get
            {
                return mControladorBase.ProyectoSeleccionadoError404;
            }
            set
            {
                mControladorBase.ProyectoSeleccionadoError404 = value;
            }
        }

        /// <summary>
        /// Obtiene el proyecto seleccionado
        /// </summary>
        public string UrlSearchProyecto
        {
            get
            {
                return mControladorBase.UrlSearchProyecto;
            }
            set
            {
                mControladorBase.UrlSearchProyecto = value;
            }
        }

        public string RequestUrl
        {
            get
            {
                return UriHelper.GetEncodedUrl(mHttpContextAccessor.HttpContext.Request);
            }
        }

        public UserProfileModel Perfil
        {
            get
            {
                if (mPerfil == null)
                {
                    mPerfil = CargarDatosPerfil();
                    string idioma = "";
                    if (UtilIdiomas.LanguageCode != IdiomaPorDefecto)
                    {
                        idioma = "/" + UtilIdiomas.LanguageCode;
                    }

                    mPerfil.Url = UrlPrincipal + idioma + UrlPerfil;
                    mPerfil.UrlViewProfile = mControladorBase.UrlsSemanticas.GetURLPerfilDeIdentidad(BaseURL, ProyectoSeleccionado.NombreCorto, UtilIdiomas, IdentidadActual);
                }
                return mPerfil;
            }
        }

        public AutenticationModel FormularioRegistro
        {
            get
            {
                if (mFormularioRegistro == null)
                {
                    mFormularioRegistro = CargarFormularioRegistro();
                }
                return mFormularioRegistro;
            }
        }

        /// <summary>
        /// Obtiene el proyecto externo en el que se está haciendo la búsqueda, o NULL si se hace en el proyecto actual.
        /// </summary>
        public Proyecto ProyectoOrigenBusqueda
        {
            get
            {
                return mControladorBase.ProyectoOrigenBusqueda;
            }
        }

        /// <summary>
        /// Nombre del proyecto actual o del de OrigenBusqueda si hay.
        /// </summary>
        public string NombreProyBusquedaOActual
        {
            get
            {
                return mControladorBase.NombreProyBusquedaOActual;
            }
        }

        /// <summary>
        /// Obtiene si el dominio actual es el dominio de las comunidades por defecto (http://comunidades.gnoss.com)
        /// </summary>
        public bool EsDominioComunidades
        {
            get
            {
                return mControladorBase.EsDominioComunidades;
            }
        }

        /// <summary>
        /// Obtiene el idioma del usuario
        /// </summary>
        public string IdiomaUsuario
        {
            get
            {
                return mControladorBase.IdiomaUsuario;
            }
            set
            {
                mControladorBase.IdiomaUsuario = value;
            }
        }

        /// <summary>
        /// Obtiene la URL de presentación para este sitio web
        /// </summary>
        public string UrlPresentacion
        {
            get
            {
                return mControladorBase.UrlPresentacion;
            }
        }

        /// <summary>
        /// Obtiene el proyecto al que se conecta siempre la aplicación
        /// </summary>
        public Guid? ProyectoConexionID
        {
            get
            {
                return mControladorBase.ProyectoConexionID;
            }
        }

        /// <summary>
        /// Obtiene el nombre corto del proyecto al que se conecta por defecto siempre la aplicación
        /// </summary>
        public string NombreCortoProyectoConexion
        {
            get
            {
                return mControladorBase.NombreCortoProyectoConexion;
            }
        }

        /// <summary>
        /// Obtiene el nombre corto del proyecto al que se conecta por defecto siempre la aplicación
        /// </summary>
        public string NombreCortoProyectoPrincipal
        {
            get
            {
                return mControladorBase.NombreCortoProyectoPrincipal;
            }
        }

        /// <summary>
        /// Indica si debe pintar la cabecera simplificada
        /// </summary>
        public TipoCabeceraProyecto TipoCabecera
        {
            get
            {
                return mControladorBase.TipoCabecera;
            }
        }

        /// <summary>
        /// Indica si hay que pintar la ficha de recursos de inevery o la normal.
        /// </summary>
        public bool PintarFichaRecInevery
        {
            get
            {
                return mControladorBase.PintarFichaRecInevery;
            }
        }

        /// <summary>
        /// Obtiene si los enlaces tienen que apuntar al perfil global en la comunidad principal
        /// </summary>
        public bool PerfilGlobalEnComunidadPrincipal
        {
            get
            {
                return mControladorBase.PerfilGlobalEnComunidadPrincipal;
            }
        }

        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string UrlServicioContextos
        {
            get
            {
                return mControladorBase.UrlServicioContextos;
            }
        }

        /// <summary>
        /// Devueve o establece el tipo de documento semantico en el que estamos
        /// </summary>
        public string TipoDocSem
        {
            get
            {
                return mTipoDocSem;
            }
            set
            {
                mTipoDocSem = value;
            }
        }

        /// <summary>
        /// Devueve o establece el tipo de pagina en la que estamos
        /// </summary>
        public TiposPagina TipoPagina
        {
            get
            {
                return mControladorBase.TipoPagina;
            }
            set
            {
                mControladorBase.TipoPagina = value;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin bandeja de suscripciones
        /// </summary>
        public bool EsEcosistemaSinBandejaSuscripciones
        {
            get
            {
                return mControladorBase.EsEcosistemaSinBandejaSuscripciones;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin contactos
        /// </summary>
        public bool EsEcosistemaSinContactos
        {
            get
            {
                return mControladorBase.EsEcosistemaSinContactos;
            }
        }


        /// <summary>
        /// Obtiene si se puede editar multiple
        /// </summary>
        public bool EsEdicionMultiple
        {
            get
            {
                return mControladorBase.EsEdicionMultiple;
            }
        }

        #region Configuración AutoCompletar

        /// <summary>
        /// Facetas para autocompletar la comunidad.
        /// </summary>
        public string FacetasProyAutoCompBuscadorCom
        {
            get
            {
                return mControladorBase.FacetasProyAutoCompBuscadorCom;
            }
        }

        /// <summary>
        /// Obtiene el origen del autocompletar según el tipo de página.
        /// </summary>
        public string OrigenAutoCompletarPagina
        {
            get
            {
                return mControladorBase.OrigenAutoCompletarPagina;
            }
        }

        #endregion

        public string EspacioPersonal
        {
            get
            {
                return mControladorBase.EspacioPersonal;
            }
        }

        public string NombreProyectoEcosistema
        {
            get
            {
                return mControladorBase.NombreProyectoEcosistema;
            }
        }

        public TipoBusqueda VariableTipoBusqueda { get; set; }

        public bool AdministradorQuiereVerTodasLasPersonas
        {
            get
            {
                return ((!string.IsNullOrEmpty(RequestParams("admin"))) && (PuedeVerTodasLasPersonas || (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))));
            }
        }

        public bool PuedeVerTodasLasPersonas
        {
            get
            {
                return (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto) && VariableTipoBusqueda.Equals(TipoBusqueda.PersonasYOrganizaciones) && mControladorBase.UsuarioActual.EstaAutorizado((ulong)Capacidad.General.CapacidadesPersonas.VerTODASpersonas));
            }
        }

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos.
        /// </summary>
        public List<TiposDocumentacion> ListaPermisosDocumentos
        {
            get
            {
                if (ParametroProyectoVirtual.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
                {
                    return mControladorBase.ListaPermisosDocumentosDeProyecto(new Guid(ParametroProyectoVirtual[ParametroAD.ProyectoIDPatronOntologias]));
                }
                else
                {
                    return mControladorBase.ListaPermisosDocumentos;
                }
            }
        }

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos.
        /// </summary>
        public bool ComprobarPermisoEnOntologiaDeProyectoEIdentidad(Guid pDocumentoID)
        {
            if (mControladorBase.ProyectoVirtual.FilaProyecto.ProyectoSuperiorID.HasValue)
            {
                return mControladorBase.ComprobarPermisoEnOntologiaDeProyectoEIdentidad(mControladorBase.ProyectoVirtual.FilaProyecto.ProyectoSuperiorID.Value, pDocumentoID, true);
            }
            else
            {
                bool devolver = mControladorBase.ComprobarPermisoEnOntologiaDeProyectoEIdentidad(mControladorBase.UsuarioActual.ProyectoID, pDocumentoID, false);
                Guid proyectoOntologiasID = ProyectoVirtual.Clave;
                if (!devolver && ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
                {
                    proyectoOntologiasID = new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
                    devolver = mControladorBase.ComprobarPermisoEnOntologiaDeProyectoEIdentidad(proyectoOntologiasID, pDocumentoID, false);
                }
                return devolver;
            }
        }

        public bool EsUsuarioAdministradorProyecto
        {
            get
            {
                return mControladorBase.EsUsuarioAdministradorProyecto;
            }
        }

        public bool EsUsuarioAdministradorProyectoVirtual
        {
            get
            {
                return mControladorBase.EsUsuarioAdministradorProyectoVirtual;
            }
        }

        /// <summary>
        /// FilaProy con los contadores del proyecto actual.
        /// </summary>
        public AD.EntityModel.Models.ProyectoDS.Proyecto ContadoresProyecto
        {
            get
            {
                return mControladorBase.ContadoresProyecto;
            }
        }

        /// <summary>
        /// HTML personalizado para el Login.
        /// </summary>
        public string ProyectoLoginConfiguracion
        {
            get
            {
                return mControladorBase.ProyectoLoginConfiguracion;
            }
        }

        /// <summary>
        /// Devuelve si la organización es de gnossOrganiza
        /// </summary>
        public bool EsGnossOrganiza
        {
            get
            {
                return mControladorBase.EsGnossOrganiza;
            }
        }

        public bool AvisoCookie
        {
            get
            {
                return ParametrosGeneralesVirtualRow.AvisoCookie;
            }
        }

        public string UrlAvisoCookieExterno
        {
            get
            {
                string urlAvisoCookie = "";
                //Cargamos los enlaces de la política de cookies y su texto
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                DataWrapperUsuario usuarioDW = proyCL.ObtenerPoliticaCookiesProyecto(ProyectoVirtual.Clave);
                proyCL.Dispose();

                if (usuarioDW.ListaClausulaRegistro.Count > 0)
                {
                    string texto = usuarioDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesUrlPagina)).Select(item => item.Texto).FirstOrDefault();

                    Uri urlCookies = null;
                    if (Uri.TryCreate(texto, UriKind.Absolute, out urlCookies))
                    {
                        if (Uri.IsWellFormedUriString(texto, UriKind.RelativeOrAbsolute))
                        {
                            //Se trata de una URL
                            urlAvisoCookie = urlCookies.ToString();
                        }
                        else
                        {
                            urlAvisoCookie = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "POLITICACOOKIES");
                        }
                    }
                    else
                    {
                        urlAvisoCookie = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "POLITICACOOKIES");
                    }
                }

                return urlAvisoCookie;
            }
        }

        public string TextoCopyright
        {
            get
            {

                ParametroAplicacion busqueda = GestorParametroAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == "copyright");
                ParametroAplicacion busqueda2 = GestorParametroAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == TiposParametrosAplicacion.Copyright);
                string textoCopyright = "";
                //if (!string.IsNullOrEmpty(ParametrosGeneralesVirtualRow.Copyright) || (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro='copyright'").Length > 0 && !string.IsNullOrEmpty(((ParametroAplicacionDS.ParametroAplicacionRow)(ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro='" + TiposParametrosAplicacion.Copyright + "'")[0])).Valor)))
                if (!string.IsNullOrEmpty(ParametrosGeneralesVirtualRow.Copyright) || (busqueda != null && !string.IsNullOrEmpty(((GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.Copyright).FirstOrDefault())).Valor)))
                {
                    //if ((busqueda.Count > 0 && !string.IsNullOrEmpty(((ParametroAplicacionDS.ParametroAplicacionRow)(ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro='" + TiposParametrosAplicacion.Copyright + "'")[0])).Valor)))
                    if ((busqueda != null && busqueda2 != null && !string.IsNullOrEmpty(busqueda2.Valor)))
                    {
                        //textoCopyright = ((ParametroAplicacionDS.ParametroAplicacionRow)(ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro='" + TiposParametrosAplicacion.Copyright + "'")[0])).Valor;
                        textoCopyright = busqueda2.Valor;
                    }
                    else
                    {
                        textoCopyright = ParametrosGeneralesVirtualRow.Copyright;
                    }
                }

                return textoCopyright;
            }
        }

        public bool ClausulasRegistro
        {
            get
            {
                return ParametrosGeneralesVirtualRow.ClausulasRegistro;
            }
        }

        public string EnlaceContacto
        {
            get
            {
                return ParametrosGeneralesVirtualRow.EnlaceContactoPiePagina;
            }
        }

        public string ProyectoEnBeta
        {
            get
            {
                string proyectoEnBeta = "";
                if (!ProyectoSeleccionado.EsCatalogo)
                {


                    bool proyEnBeta = (GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro == "UrlsPropiasProyecto").FirstOrDefault().Valor).Contains($"={ProyectoSeleccionado.UrlPropia(IdiomaUsuario)}");
                    //bool proyEnBeta = ((DataRow)(ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'UrlsPropiasProyecto'")[0]))["Valor"].ToString().Contains("=" + ProyectoSeleccionado.UrlPropia);

                    if ((proyEnBeta && mControladorBase.UsuarioActual != null && !mControladorBase.UsuarioActual.EsUsuarioInvitado) || ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                    {
                        proyectoEnBeta = "beta";
                    }

                    if (proyEnBeta && mControladorBase.UsuarioActual != null && mControladorBase.UsuarioActual.EsUsuarioInvitado)
                    {
                        proyectoEnBeta = "beta GNOSS";
                    }
                }

                return proyectoEnBeta;
            }
        }

        public bool MenuCompactado
        {
            get
            {
                return !ParametrosGeneralesVirtualRow.ComunidadGNOSS;
            }
        }

        public bool EsAdministradorDeAlgunaClase
        {
            get
            {
                if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
                {
                    return false;
                }
                else if (string.IsNullOrEmpty(mEsAdministradorDeAlgunaClase))
                {
                    OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mEsAdministradorDeAlgunaClase = orgCN.ComprobarUsuarioAdministraAlgunaClase(mControladorBase.UsuarioActual.UsuarioID).ToString();
                    orgCN.Dispose();
                }

                return mEsAdministradorDeAlgunaClase == true.ToString();
            }
        }

        /// <summary>
        /// Indica si debe aceptar automáticamente los registros en el ecosistema o mantenerlos en espera hasta que se acepte la solicitud por el administrador
        /// </summary>
        public bool RegistroAutomaticoEcosistema
        {
            get
            {
                return mControladorBase.RegistroAutomaticoEcosistema;
            }
        }

        public bool RegistroAutomáticoEnComunidad
        {
            get
            {
                return mControladorBase.RegistroAutomaticoEnComunidad;
            }
        }

        public bool EsIdentidadActualAdministradorOrganizacion
        {
            get
            {
                return IdentidadActual.OrganizacionID.HasValue && ChequeoSeguridad.ComprobarCapacidadEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, (Guid)IdentidadActual.OrganizacionID.Value);
            }
        }

        public bool EsIdentidadActualSupervisorProyecto
        {
            get
            {
                return ChequeoSeguridad.ComprobarCapacidadEnProyecto((ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos);
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public DataWrapperExportacionBusqueda ExportacionBusquedaDW
        {
            get
            {
                return mControladorBase.ExportacionBusquedaDW;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public string UrlPaginaConParametros
        {
            get
            {
                if (mUrlPaginaConParametros == null)
                {
                    mUrlPaginaConParametros = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}").AbsoluteUri;
                }
                return mUrlPaginaConParametros;
            }
        }

        public string UrlMultiIdiomaPaginaActual
        {
            get
            {
                string url = "";
                if (mConfigService.PeticionHttps())
                {
                    url = $"https://{Request.Path}";
                }
                else
                {
                    url =  $"http://{Request.Path}";
                }

                RouteValueDictionary ruta = mActionContextAccessor.ActionContext.RouteData.Values;

                string[] partesUrl = url.Split("/");
                foreach (string parteUrl in partesUrl)
                {
                    if (ruta.Values.Contains(parteUrl))
                    {
                        string claveDiccionario = "{" + ruta.Where(x => x.Value.Equals(parteUrl)).Select(x => x.Key).First() + "}";
                        if (!claveDiccionario.Equals("{lang}"))
                        {
                            url = url.Replace("/" + parteUrl + "/", "/" + claveDiccionario + "/");
                        }
                    }
                }

                if (url.StartsWith("/"))
                {
                    url = url.Remove(0,1);
                }
               
                if (RouteConfig.ListaRutasURLName.ContainsKey(url))
                {
                    url = RouteConfig.ListaRutasURLName[url];

                    url = url.Substring(url.IndexOf('/') + 1);

                    if (url.EndsWith("CMSPagina"))
                    {
                        url = url.Substring(0, url.Length - "CMSPagina".Length);
                    }
                    while (url.EndsWith("/"))
                    {
                        url = url.Substring(0, url.Length - 1);
                    }
                }
                return (url+Request.QueryString);
            }
        }

        /// <summary>
        /// Obtiene o establece si la petición actual ha sido transferida a la página 404
        /// </summary>
        public bool RedireccionadoA404
        {
            get
            {
                return mRedireccionadoA404;
            }
            set
            {
                mRedireccionadoA404 = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si la pagina es visible en privada
        /// </summary>
        public virtual bool PaginaVisibleEnPrivada
        {
            get
            {
                return mPaginaVisibleEnPrivada;
            }
            set
            {
                this.mPaginaVisibleEnPrivada = value;
            }
        }

        /// <summary>
        /// Indica si la petición es OAuth o no.
        /// </summary>
        public bool EsPeticionOAuth
        {
            get
            {
                return !(this.Request.Path.ToString().ToLower().Contains("enviartwitter.aspx")) && ((Request.Headers.ContainsKey("oauth_token")) || (Request.Headers.ContainsKey("Authorization") && Request.Headers["Authorization"].Contains("oauth_token=")));
            }
        }

        /// <summary>
        /// Indica si la petición es OAuth o no.
        /// </summary>
        public bool HayValidacionRecaptcha
        {
            get
            {
                //return !string.IsNullOrEmpty(Request["g-recaptcha-response"]);
                return Request.Query.ContainsKey("g-recaptcha-response");
            }
        }

        public bool EstaEnLoginComunidad
        {
            get
            {
                return (this.GetType().Name.Equals("RegistroController") || this.GetType().Name.Equals("PeticionesAJAXController"));
            }
        }

        /// <summary>
        /// Obtiene la ontología de GNOSS
        /// </summary>
        protected Ontologia OntologiaGnoss
        {
            get
            {
                if (mOntologia == null)
                {
                    mOntologia = new OntologiaGnoss(UtilGnoss.OntologiaGnoss, mEntityContext);
                    mOntologia.LeerOntologia();
                }
                return mOntologia;
            }
        }

        public bool EsPeticionRDF
        {
            get
            {
                if (Request.Host.ToString().Contains("depuracion.net") && Request.QueryString.ToString().EndsWith("?rdfDepuracion"))
                {
                    return true;
                }
                if (Request.ContentType == "application/rdf+xml")
                {
                    return true;
                }
                else if (Request.QueryString.ToString().EndsWith("?rdf") || Request.QueryString.ToString().EndsWith("&rdf") || Request.QueryString.ToString().Contains("?rdf&") || Request.QueryString.ToString().Contains("&rdf&"))
                {
                    if (EsBot)
                    {
                        return true;
                    }
                    else if (Request.Headers.ContainsKey("Referer") && !Request.Headers["Referer"].ToString().Equals(string.Empty))
                    {
                        //Si viene del mismo dominio, le permito descargarse el RDF
                        return new Uri(Request.Headers["Referer"]).Host.Equals(Request.Host.Value);
                    }
                }

                return false;
            }
        }

        public bool EsPeticionRSS
        {
            get
            {
                return (Request.QueryString.ToString().EndsWith("?rss") || Request.QueryString.ToString().EndsWith("&rss") || Request.QueryString.ToString().Contains("?rss&") || Request.QueryString.ToString().Contains("&rss&"));
            }
        }

        /// <summary>
        /// Incio del título de las páginas.
        /// </summary>
        public string InicioTituloPagEcosistema
        {
            get
            {
                string inicioTitulo = "GNOSS -";

                if (ProyectoVirtual != null && ProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
                {
                    if (string.IsNullOrEmpty(ProyectoVirtual.FilaProyecto.NombrePresentacion))
                    {
                        inicioTitulo = ProyectoVirtual.Nombre + " -";
                    }
                    else
                    {
                        inicioTitulo = ProyectoVirtual.FilaProyecto.NombrePresentacion + " -";
                    }
                }

                return inicioTitulo;
            }
        }

        /// <summary>
        /// Indica si permite el usuario sin login.
        /// </summary>
        public bool UsuarioSinLoginPermitido
        {
            get
            {
                return (this.GetType().Name.Equals("EditarRecursoController") && RequestParams("virtual") != null);
            }
        }

        /// <summary>
        /// Verdad si la página actual tiene algún control de edición (editar recurso, agregar comentario, enviar mensaje...)
        /// </summary>
        public bool EsPaginaEdicion
        {
            get;
            set;
        }

        /// <summary>
        /// Controlador para cargar modelos del ProyectoSeleccionado pra la IdentidadActual
        /// </summary>
        public ControladorProyectoMVC ControladorProyectoMVC
        {
            get
            {
                if (mControladorProyectoMVC == null)
                {
                    List<string> BaseURLsContent = new List<string>();
                    BaseURLsContent.Add(BaseURLContent);
                    mControladorProyectoMVC = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLsContent, BaseURLStatic, ProyectoSeleccionado, Guid.Empty, ParametrosGeneralesRow, IdentidadActual, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorProyectoMVC;
            }
        }

        /// <summary>
        /// Obtiene los permisos de las páginas que puede administrar el usuario actual
        /// </summary>
        protected PermisosPaginasAdministracionViewModel PermisosPaginasAdministracion
        {
            get
            {
                if (mPermisosPaginasAdministracion == null)
                {
                    mPermisosPaginasAdministracion = CargarPermisosAdministracionComunidad();
                }

                return mPermisosPaginasAdministracion;
            }
        }

        #endregion

        #region MetaRobots

        public void AsignarMetaRobots(string pMetaRobots)
        {
            if (ViewBag.ListaMetas == null)
            {
                ViewBag.ListaMetas = new List<KeyValuePair<string, string>>();
            }

            if (ViewBag.ListaMetasComplejas == null)
            {
                ViewBag.ListaMetasComplejas = new List<Dictionary<string, string>>();
            }


            KeyValuePair<string, string>? metaAntigua = null;
            KeyValuePair<string, string> metaNueva = new KeyValuePair<string, string>("robots", pMetaRobots.ToLower());
            foreach (KeyValuePair<string, string> meta in (List<KeyValuePair<string, string>>)ViewBag.ListaMetas)
            {
                if (meta.Key == "robots")
                {
                    metaAntigua = meta;
                }
            }
            if (!metaAntigua.HasValue)
            {
                ((List<KeyValuePair<string, string>>)ViewBag.ListaMetas).Add(metaNueva);
            }
            else
            {
                if (ObtenerValorMeta(metaNueva.Value) > ObtenerValorMeta(metaAntigua.Value.Value))
                {
                    ((List<KeyValuePair<string, string>>)ViewBag.ListaMetas).Remove(metaAntigua.Value);
                    ((List<KeyValuePair<string, string>>)ViewBag.ListaMetas).Add(metaNueva);
                }
            }
        }

        private int ObtenerValorMeta(string pMeta)
        {
            //0 "ALL"
            //1 "INDEX, FOLLOW"
            //2 "INDEX, NOFOLLOW"
            //3 "NOINDEX, FOLLOW"
            //4 "NOINDEX, NOFOLLOW"
            pMeta = pMeta.ToLower().Trim();
            if (pMeta == "all")
            {
                return 0;
            }
            int metaInt = 0;
            if (int.TryParse(pMeta, out metaInt))
            {
                return metaInt;
            }
            string[] metas = pMeta.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<string> metasAux = new List<string>();
            foreach (string meta in metas)
            {
                metasAux.Add(meta.Trim());
            }
            if (metasAux.Count == 2)
            {
                if (metasAux.Contains("index") && metasAux.Contains("follow"))
                {
                    return 1;
                }
                else if (metasAux.Contains("index") && metasAux.Contains("nofollow"))
                {
                    return 2;
                }
                else if (metasAux.Contains("noindex") && metasAux.Contains("follow"))
                {
                    return 3;
                }
                else if (metasAux.Contains("noindex") && metasAux.Contains("nofollow"))
                {
                    return 4;
                }
            }
            throw new Exception("El valor de la meta: '" + pMeta + "' no es válido");
        }

        /// <summary>
        /// Obtiene la metaetiqueta "robots"
        /// </summary>
        private string MetaRobots
        {
            get
            {
                if (mMetaRobots.Equals("all"))
                {

                    string metaRobots = mConfigService.ObtenerRobots();
                    if (!string.IsNullOrEmpty(metaRobots))
                    {
                        mMetaRobots = metaRobots;
                    }

                    if (ProyectoMetaRobotsRows != null && ProyectoMetaRobotsRows.Any(filaMetaRobots => filaMetaRobots.Tipo.Equals((short)TipoPaginaMetaRobots.Pagina)))
                    {
                        return ProyectoMetaRobotsRows.FirstOrDefault(filaMetaRobots => filaMetaRobots.Tipo.Equals((short)TipoPaginaMetaRobots.Pagina)).Content;
                    }
                }
                return mMetaRobots;
            }
            set
            {
                mMetaRobots = value;
            }
        }

        /// <summary>
        /// Obtiene la fila del proyectometarobots
        /// </summary>
        protected List<ProyectoMetaRobots> ProyectoMetaRobotsRows
        {
            get
            {
                if (mProyectoMetaRobotsRows == null)
                {
                    mProyectoMetaRobotsRows = mControladorBase.ObtenerFilasProyectoMetaRobots(ProyectoSeleccionado.Clave);
                }
                return mProyectoMetaRobotsRows;
            }
        }
        #endregion

        private bool obtenerParametroBooleano(string pNombreParametro, bool pValorPorDefecto = false)
        {
            if (ParametroProyecto.ContainsKey(pNombreParametro))
            {
                if (ParametroProyecto[pNombreParametro] == "1" || ParametroProyecto[pNombreParametro] == "true")
                {
                    return true;
                }
                else if (ParametroProyecto[pNombreParametro] == "0" || ParametroProyecto[pNombreParametro] == "false")
                {
                    return false;
                }
            }
            return pValorPorDefecto;
        }

        private Guid ObtenerUsuarioIDOAuth()
        {
            Guid usuarioID = Guid.Empty;
            Stopwatch sw = null;
            try
            {
                string urlPeticionOauth = Es.Riam.Util.UtilOAuth.ObtenerUrlGetDePeticionOAuth(mHttpContextAccessor.HttpContext.Request);



                CallOauthService servicioOauth = new CallOauthService(mConfigService);
                sw = LoggingService.IniciarRelojTelemetria();
                usuarioID = servicioOauth.ObtenerUsuarioAPartirDeUrl(urlPeticionOauth, Request.Method);
                mLoggingService.AgregarEntradaDependencia($"Obtener usuario OAuth. Url: {urlPeticionOauth}. UsuarioID: {usuarioID} ", false, "ObtenerUsuarioIDOAuth", sw, true);

            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia("Error al obtener el usuarioID del servicio OAuth", false, "ObtenerUsuarioIDOAuth", sw, false);
                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }

            return usuarioID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        protected new PartialViewResult PartialView(string viewName)
        {
            if (TienePersonalizacion())
            {
                string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, viewName);

                return base.PartialView(viewName + personalizacion);
            }
            return base.PartialView(viewName);
        }

        protected new internal ViewResult View()
        {
            if (TienePersonalizacion())
            {
                string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, "Index");

                return base.View(personalizacion);
            }
            return base.View();
        }

        //protected internal ViewResult View(string viewName)
        protected new internal ViewResult View(string viewName)
        {
            if (TienePersonalizacion())
            {
                string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, viewName);

                return base.View(viewName + personalizacion);
            }
            if (ViewBag.ControllerName.ToLower() == "ficharecurso")
            {
                return base.View();
            }
            else
            {
                return base.View(viewName);
            }
        }

        //protected internal ViewResult View(object model)
        protected new internal ActionResult View(object model)
        {
            //Para Que solo lo permita desde desarrollo
            if (Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json") && !Comunidad.EntornoEsPro && !Comunidad.EntornoEsPre)
            {
                //Serializamos todo con el memoryStream para que no de error el compilador de vistas.
                //Se corrige el error de no poder depurar con el compilador algunas páginas del CMS
                //Usar la versión 2.4.0.0 del compilador para que funcione

                MemoryStream memStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memStream, model);
                string modelSerialized = string.Empty;

                memStream.Seek(0, SeekOrigin.Begin);

                using (MemoryStream output = new MemoryStream())
                using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(output, System.IO.Compression.CompressionMode.Compress))
                {
                    memStream.CopyTo(deflateStream);
                    deflateStream.Close();

                    modelSerialized = Convert.ToBase64String(output.ToArray());
                }

                return Content(SerializeViewData(modelSerialized), "application/json");

                //JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                //{
                //    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                //    TypeNameHandling = TypeNameHandling.All
                //};
                //string json = JsonConvert.SerializeObject(model, jsonSerializerSettings);

                //return Content(SerializeViewData(json), "application/json");
            }
            else
            {
                if (TienePersonalizacion())
                {
                    string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, "Index");

                    return base.View("Index" + personalizacion, model);
                }
                return base.View(model);
            }
        }

        protected new internal ActionResult View(string viewName, object model)
        {
            if (Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json") && !Comunidad.EntornoEsPro && !Comunidad.EntornoEsPre)
            {
                //JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                //{
                //    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                //    TypeNameHandling = TypeNameHandling.All
                //};
                //string json = JsonConvert.SerializeObject(model, jsonSerializerSettings);

                MemoryStream memStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memStream, model);
                string json = string.Empty;

                memStream.Seek(0, SeekOrigin.Begin);

                using (MemoryStream output = new MemoryStream())
                using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(output, System.IO.Compression.CompressionMode.Compress))
                {
                    memStream.CopyTo(deflateStream);
                    deflateStream.Close();

                    json = Convert.ToBase64String(output.ToArray());
                }

                return Content(SerializeViewData(json), "application/json");
            }
            else
            {
                if (TienePersonalizacion())
                {
                    string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, viewName);

                    return base.View(viewName + personalizacion, model);
                }
                return base.View(viewName, model);
            }
        }

        protected internal ActionResult ViewError404(object model)
        {
            if (Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json") && !Comunidad.EntornoEsPro && !Comunidad.EntornoEsPre)
            {
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memStream, model);
                string json = string.Empty;

                memStream.Seek(0, SeekOrigin.Begin);

                using (MemoryStream output = new MemoryStream())
                using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(output, System.IO.Compression.CompressionMode.Compress))
                {
                    memStream.CopyTo(deflateStream);
                    deflateStream.Close();

                    json = Convert.ToBase64String(output.ToArray());
                }

                return Content(SerializeViewData(json), "application/json");
            }
            else
            {
                if (TienePersonalizacion())
                {
                    string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, "Error404");
                    return base.View("~/Views/Error/Error404" + personalizacion + ".cshtml", model);
                }
                return base.View("~/Views/Error/Error404.cshtml", model);
            }
        }

        protected internal ActionResult ViewCMSPagina(object model)
        {
            if (Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json") && !Comunidad.EntornoEsPro && !Comunidad.EntornoEsPre)
            {
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memStream, model);
                string json = string.Empty;

                memStream.Seek(0, SeekOrigin.Begin);

                using (MemoryStream output = new MemoryStream())
                using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(output, System.IO.Compression.CompressionMode.Compress))
                {
                    memStream.CopyTo(deflateStream);
                    deflateStream.Close();

                    json = Convert.ToBase64String(output.ToArray());
                }

                return Content(SerializeViewData(json), "application/json");
            }
            else
            {
                if (TienePersonalizacion())
                {
                    string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, "Index");

                    return base.View("~/Views/CMSPagina/Index" + personalizacion + ".cshtml", model);
                }
                return base.View("~/Views/CMSPagina/Index.cshtml", model);
            }
        }

        protected new internal ActionResult ViewFichaRecurso(string viewName, object model)
        {
            if (Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json") && !Comunidad.EntornoEsPro && !Comunidad.EntornoEsPre)
            {
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memStream, model);
                string json = string.Empty;

                memStream.Seek(0, SeekOrigin.Begin);

                using (MemoryStream output = new MemoryStream())
                using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(output, System.IO.Compression.CompressionMode.Compress))
                {
                    memStream.CopyTo(deflateStream);
                    deflateStream.Close();

                    json = Convert.ToBase64String(output.ToArray());
                }
                return Content(SerializeViewData(json), "application/json");
            }
            else
            {
                if (TienePersonalizacion())
                {
                    string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, (string)ViewBag.ControllerName + "_" + viewName, "Index");
                    if (!string.IsNullOrEmpty(personalizacion))
                    {
                        //Hay personalización para este tipo de recurso
                        personalizacion = viewName + personalizacion;
                    }
                    else
                    {
                        //No hay personalización por tipo, busco personalización para la vista genérica
                        personalizacion = MultiViewResult.ComprobarPersonalizacion(this, "Index");
                    }

                    if (!string.IsNullOrEmpty(personalizacion))
                    {
                        return base.View(personalizacion, model);
                    }
                }
                return base.View(model);
            }
        }

        /// <summary>
        /// Añade a un json pasado como parámetro, el json correspondiente a serializar el ViewData
        /// </summary>
        private string SerializeViewData(string json)
        {
            ViewBag.ControladorProyectoMVC = null;
            UtilIdiomasSerializable aux = ViewBag.UtilIdiomas.GetUtilIdiomas();
            ViewBag.UtilIdiomas = aux;

            JsonSerializerSettings jsonSerializerSettingsVB = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full
            };
            Dictionary<string, object> dic = ViewData.Where(k => !k.Key.Equals("LoggingService")).ToDictionary(k => k.Key, v => v.Value);
            string jsonViewData = JsonConvert.SerializeObject(dic, jsonSerializerSettingsVB);

            return json + "{ComienzoJsonViewData}" + jsonViewData;
        }

        /*TODO Javier middleware
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            filterContext.Controller.ViewBag.Error = filterContext.Exception.Message;
            filterContext.Controller.ViewBag.ErrorTrace = filterContext.Exception.StackTrace;

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Error/Error500.cshtml",
                ViewData = filterContext.Controller.ViewData
            };
            filterContext.ExceptionHandled = true;

            string mensajeError = $" ERROR:  {filterContext.Exception.Message}\r\nStackTrace: {filterContext.Exception.StackTrace}";

            //GuardarLogError(mensajeError);
            mLoggingService.GuardarLogError(filterContext.Exception); //Enviar Excepción

            if (filterContext.Exception.StackTrace.Contains(" ASP._Page_Views") && !filterContext.Exception.StackTrace.Contains(" ASP._Page_Views_Shared_Layout") && !filterContext.Exception.StackTrace.Contains(" ASP._Page_Views_Error") && !filterContext.Exception.StackTrace.Contains(" ASP._Page_Views_Shared_Head__HojasDeEstilo"))
            {
                string directorioLogErroresVistas = Server.MapPath("/ErroresVistas");

                // Es posible que haya un error en las vistas, guardo el log e intento invalidarlas
                if (Directory.Exists(directorioLogErroresVistas))
                {
                    mLoggingService.GuardarLogError(mLoggingService.DevolverCadenaError(filterContext.Exception, "1.0.0.0"), $"{directorioLogErroresVistas}\\errorvista_{DateTime.Now.ToString("yyyyMMdd")}.log");
                }

                // Si la última vez que se invalidaron vistas por error fué hace menos de 30 minutos, no vuelvo a invalidar las vistas. El error probablemente no estará en la invalidación, hay otro tipo de error. 
                if (!UltimaInvalidacionVistasPorError.HasValue || DateTime.Now.Subtract(UltimaInvalidacionVistasPorError.Value).TotalMinutes > 30)
                {
                    UltimaInvalidacionVistasPorError = DateTime.Now;

                    //InvalidarVistasLocales();
                    EntityContext context = mEntityContext;
                    List<ParametroAplicacion> listaParametroAplicacion = context.ParametroAplicacion.Where(parametro => parametro.Parametro == "CorreoErroresVistas").ToList();
                    if (listaParametroAplicacion.Count > 0)
                    {
                        GestionNotificaciones.EnviarCorreoError(mensajeError, $"en vista {this.Version} {mControladorBase.DominoAplicacion}", listaParametroAplicacion);
                        //GestionNotificaciones.EnviarCorreoError(mensajeError, $"en vista {this.Version} {UtilUsuario.DominoAplicacion} {Server.MachineName}", lParametroAplicacion,ParametrosAplicacionDS);
                    }
                }
            }
        }*/

        /// <summary>
        /// Incluye si es necesario el aviso de Cookies en el modelo ViewBag.
        /// </summary>
        private void IncluirAvisoCookies(ActionExecutingContext pFilterContext)
        {
            if (!EsBot && (ParametrosGeneralesVirtualRow.AvisoCookie || ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies)))
            {
                string nombreCookie = string.Empty;

                if (ParametroProyecto != null && ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies) && !string.IsNullOrEmpty(ParametroProyecto[ParametroAD.NombrePoliticaCookies]))
                {
                    nombreCookie = ParametroProyecto[ParametroAD.NombrePoliticaCookies] + mControladorBase.DominoAplicacion;
                }
                else
                {
                    nombreCookie = "cookieAviso" + mControladorBase.DominoAplicacion;
                }

                Dictionary<string, string> cookieAviso = UtilCookies.FromLegacyCookieString(Request.Cookies[nombreCookie]);
                bool actualizarCookie = false;
                if (cookieAviso == null || cookieAviso.Count == 0 || !cookieAviso.ContainsKey("aceptada"))
                {
                    if (cookieAviso != null && !cookieAviso.ContainsKey("aceptada"))
                    {
                        Response.Cookies.Append(nombreCookie, "", new CookieOptions { Expires = DateTime.Now.AddDays(-1), Domain = mControladorBase.DominoAplicacion, HttpOnly = true, Secure = true });
                    }
                    else
                    {
                        cookieAviso = new Dictionary<string, string>();
                        cookieAviso.Add("aceptada", "false");
                        actualizarCookie = true;
                    }                    
                }
                else
                {
                    if (mControladorBase.ParametroProyectoEcosistema != null && mControladorBase.ParametroProyectoEcosistema.ContainsKey(ParametroAD.NombrePoliticaCookies) && !string.IsNullOrEmpty(mControladorBase.ParametroProyectoEcosistema[ParametroAD.NombrePoliticaCookies]))
                    {
                        nombreCookie = mControladorBase.ParametroProyectoEcosistema[ParametroAD.NombrePoliticaCookies] + mControladorBase.DominoAplicacion;
                    }
                    else
                    {
                        nombreCookie = "cookieAviso" + mControladorBase.DominoAplicacion;
                    }
                    cookieAviso = UtilCookies.FromLegacyCookieString(Request.Cookies[nombreCookie]);

                    if (cookieAviso == null)
                    {
                        cookieAviso = new Dictionary<string, string>();
                        cookieAviso.Add("aceptada", "false");
                        actualizarCookie = true;
                    }
                }

                if (cookieAviso == null || !cookieAviso.ContainsKey("aceptada") || cookieAviso["aceptada"] == "false")
                {
                    ViewBag.CookiesWarning = "Cookies no aceptadas";
                }

                if (actualizarCookie && pFilterContext.Result != null)
                {
                    Response.Cookies.Append(nombreCookie, UtilCookies.ToLegacyCookieString(cookieAviso), new CookieOptions { Expires = DateTime.Now.AddYears(1), Domain = mControladorBase.DominoAplicacion, HttpOnly = true, Secure = true });
                }
            }
        }

        protected void GuardarXmlCambiosAdministracion()
        {
            try
            {
                ControladorProyectoPintarEstructuraXML controladorEstructuraXML = new ControladorProyectoPintarEstructuraXML(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.FilaProyecto.ProyectoID, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);
                XmlDocument xml = controladorEstructuraXML.PintarEstructuraXML();

                MemoryStream memoryStream = new MemoryStream();
                xml.Save(memoryStream);

                //create new Bite Array
                byte[] biteArray = new byte[memoryStream.Length];
                //Set pointer to the beginning of the stream
                memoryStream.Position = 0;
                //Read the entire stream
                memoryStream.Read(biteArray, 0, (int)memoryStream.Length);

                //Subimos el fichero al servidor
                GestionDocumental gd = new GestionDocumental(mLoggingService);
                gd.Url = UrlServicioWebDocumentacion;

                gd.AdjuntarDocumentoADirectorio(biteArray, "Configuracion/" + ProyectoSeleccionado.Clave, ProyectoSeleccionado.Clave.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".xml");
            }
            catch (Exception ex)
            {
                GuardarLogError("Error al guardar el XML de configuración del Proyecto " + ex.Message);
            }
        }

        /// <summary>
        /// Envia una peticion al api de integracioncontinua de entornos, si esta configurado para este proyecto
        /// </summary>
        /// <param name="pTipoCambio"></param>
        /// <param name="pObjeto"></param>
        protected HttpResponseMessage InformarCambioAdministracion(string pTipoCambio, string pObjeto)
        {
            HttpResponseMessage resultado = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && !string.IsNullOrEmpty(EntornoIntegracionContinua))
                {
                    pObjeto = EncapsularRutas(pObjeto);
                    if (!pTipoCambio.Contains("Facetas"))
                    {
                        pObjeto = EncapsularRutas(pObjeto);
                    }
                    string peticion = UrlApiIntegracionContinua + "/integracion/upload-changes";
                    string requestParameters = $"User={mControladorBase.UsuarioActual.UsuarioID}&Project={ProyectoSeleccionado.Clave}&Environment={EntornoIntegracionContinua}&Type={pTipoCambio}&Content={System.Net.WebUtility.UrlEncode(pObjeto)}";
                    StringContent content = new StringContent(requestParameters, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = mHttpClient.PostAsync(peticion, content);
                    return response.Result;
                }
            }
            catch (Exception ex)
            {
                GuardarLogError("Error al enviar petición a UrlServicioIntegracionEntornos\n" + ex.Message);
                GuardarLogError("Error al enviar petición a UrlServicioIntegracionEntornos\n Tipo de cambio " + pTipoCambio + " y el objeto " + pObjeto);
                throw;
            }
            return resultado;
        }

        /// <summary>
        /// Envia una peticion al api de integracioncontinua de entornos, si esta configurado para este proyecto
        /// </summary>
        /// <param name="pTipoCambio"></param>
        /// <param name="pNombreVista"></param>
        /// <param name="pHtml"></param>
        protected HttpResponseMessage InformarCambioAdministracionVistas(string pTipoCambio, string pNombreVista, string pHtml)
        {
            HttpResponseMessage resultado = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string objeto = "";
            try
            {
                if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && !string.IsNullOrEmpty(EntornoIntegracionContinua))
                {

                    pHtml = EncapsularRutas(pHtml);
                    objeto = JsonConvert.SerializeObject(new KeyValuePair<string, string>(pNombreVista, pHtml));

                    string peticion = UrlApiIntegracionContinua + "/integracion/upload-changes";
                    string requestParameters = $"User={mControladorBase.UsuarioActual.UsuarioID}&Project={ProyectoSeleccionado.Clave}&Environment={EntornoIntegracionContinua}&Type={pTipoCambio}&Content={HttpUtility.UrlEncode(objeto)}";
                    StringContent content = new StringContent(requestParameters, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = mHttpClient.PostAsync(peticion, content);
                    return response.Result;
                }
            }
            catch (Exception ex)
            {
                GuardarLogError("Error al enviar petición a UrlServicioIntegracionEntornos\n" + ex.Message);
                GuardarLogError("Error al enviar petición a UrlServicioIntegracionEntornos\n Tipo de cambio " + pTipoCambio + " y el objeto " + objeto);
                throw;
            }
            return resultado;
        }

        /// <summary>
        /// Envia una peticion al api de integracioncontinua de entornos, si esta configurado para este proyecto
        /// </summary>
        /// <param name="pTipoCambio"></param>
        /// <param name="pObjeto"></param>
        /// <param name="pNombreArchivo"></param>
        protected HttpResponseMessage InformarCambioAdministracionCMS(string pTipoCambio, string pObjeto, string pNombreArchivo)
        {
            HttpResponseMessage resultado = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && !string.IsNullOrEmpty(EntornoIntegracionContinua))
                {
                    string peticion = UrlApiIntegracionContinua + "/integracion/upload-changes";
                    string requestParameters = $"User={mControladorBase.UsuarioActual.UsuarioID}&Project={ProyectoSeleccionado.Clave}&Environment={EntornoIntegracionContinua}&Type={pTipoCambio}&RelativePath={pNombreArchivo}&Content={System.Net.WebUtility.UrlEncode(pObjeto)}";
                    StringContent content = new StringContent(requestParameters, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = mHttpClient.PostAsync(peticion, content);
                    return response.Result;
                }
            }
            catch (Exception ex)
            {
                GuardarLogError("Error al enviar petición a UrlServicioIntegracionEntornos\n" + ex.Message);
            }
            return resultado;
        }

        private UtilIdiomasFactory mUtilIdiomasFactory;
        protected UtilIdiomasFactory UtilIdiomasFactory
        {
            get
            {
                if (mUtilIdiomasFactory == null)
                {
                    mUtilIdiomasFactory = new UtilIdiomasFactory(mLoggingService, mEntityContext, mConfigService);
                }
                return mUtilIdiomasFactory;
            }
        }

        protected string DesencapsularRutasFichero(string contenido)
        {
            return DesencapsularRutas(contenido);
        }


        protected string EncapsularRutas(string contenido)
        {
            Dictionary<string, string> replaces = new Dictionary<string, string>();

            foreach (string idioma in mConfigService.ObtenerListaIdiomas())
            {
                string baseUrlIdioma = ProyectoSeleccionado.UrlPropia(IdiomaUsuario);

                UtilIdiomas utilIdiomasAux = UtilIdiomasFactory.ObtenerUtilIdiomas(idioma);

                if (utilIdiomasAux.LanguageCode != "es")
                {
                    baseUrlIdioma = baseUrlIdioma + "/" + idioma;
                }

                string urlIdioma = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(utilIdiomasAux, baseUrlIdioma, ProyectoSeleccionado.FilaProyecto.NombreCorto);

                replaces.Add(urlIdioma, "[%%%_URL_PROJECT_" + idioma.ToUpper() + "_%%%]");
            }

            replaces.Add("/" + ProyectoSeleccionado.FilaProyecto.NombreCorto, "[%%%_SHORT_NAME_PROJECT_%%%]");
            if (!replaces.ContainsKey(ProyectoSeleccionado.UrlPropia(IdiomaUsuario)))
            {
                replaces.Add(ProyectoSeleccionado.UrlPropia(IdiomaUsuario), "[%%%_URL_PROJECT_%%%]");
            }
            replaces.Add(mControladorBase.BaseURLPersonalizacion, "[%%%_URL_PERSONALIZACION_%%%]");
            replaces.Add(mControladorBase.BaseURLStatic, "[%%%_URL_STATIC_%%%]");
            replaces.Add(ProyectoSeleccionado.FilaProyecto.ProyectoID.ToString(), "[%%%_PROYECTO_ID_%%%]");
            if (!replaces.ContainsKey(mControladorBase.BaseURLContent))
            {
                replaces.Add(mControladorBase.BaseURLContent, "[%%%_URL_CONTENT_%%%]");
            }

            foreach (string original in replaces.Keys)
            {
                contenido = contenido.Replace(original, replaces[original]);
                if (replaces[original].StartsWith("[%%%_URL_"))
                {
                    contenido = Regex.Replace(contenido, System.Net.WebUtility.UrlEncode(original), replaces[original].Replace("[%%%_URL_", "[%%%_ENCODED_URL_"), RegexOptions.IgnoreCase);
                }
            }

            return contenido;
        }

        protected string DesencapsularRutas(string contenido)
        {
            Dictionary<string, string> replaces = new Dictionary<string, string>();

            foreach (string idioma in mConfigService.ObtenerListaIdiomas())
            {
                string baseUrlIdioma = ProyectoSeleccionado.FilaProyecto.URLPropia;

                UtilIdiomas utilIdiomasAux = UtilIdiomasFactory.ObtenerUtilIdiomas(idioma);

                if (utilIdiomasAux.LanguageCode != "es")
                {
                    baseUrlIdioma = baseUrlIdioma + "/" + idioma;
                }

                string urlIdioma = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(utilIdiomasAux, baseUrlIdioma, ProyectoSeleccionado.FilaProyecto.NombreCorto);

                replaces.Add("[%%%_URL_PROJECT_" + idioma.ToUpper() + "_%%%]", urlIdioma);
            }

            replaces.Add("[%%%_SHORT_NAME_PROJECT_%%%]", "/" + ProyectoSeleccionado.FilaProyecto.NombreCorto);
            replaces.Add("[%%%_URL_PROJECT_%%%]", ProyectoSeleccionado.FilaProyecto.URLPropia);
            replaces.Add("[%%%_URL_PERSONALIZACION_%%%]", mControladorBase.BaseURLPersonalizacion);
            replaces.Add("[%%%_URL_CONTENT_%%%]", mControladorBase.BaseURLContent);
            replaces.Add("[%%%_URL_STATIC_%%%]", mControladorBase.BaseURLStatic);
            replaces.Add("[%%%_PROYECTO_ID_%%%]", ProyectoSeleccionado.FilaProyecto.ProyectoID.ToString());

            foreach (string reemplazo in replaces.Keys)
            {
                contenido = contenido.Replace(reemplazo, replaces[reemplazo]);
                if (reemplazo.StartsWith("[%%%_URL_"))
                {
                    contenido = contenido.Replace(reemplazo.Replace("[%%%_URL_", "[%%%_ENCODED_URL_"), System.Net.WebUtility.UrlEncode(replaces[reemplazo]));
                }
            }

            return contenido;
        }

        private ActionResult ValidarRecaptcha()
        {
            ActionResult resultado = null;

            if (HayValidacionRecaptcha)
            {
                EntityContext context = mEntityContext;
                ParametroAplicacion filaParametroGoogleRecaptchaSecret = context.ParametroAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.GoogleRecaptchaSecret).FirstOrDefault();
                //ParametroAplicacionDS.ParametroAplicacionRow filaParametroGoogleRecaptchaSecret = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.GoogleRecaptchaSecret);
                string mensaje = "Hay Validación Recaptcha";
                if (filaParametroGoogleRecaptchaSecret != null)
                {
                    List<string> listaTokensValidados = new List<string>();
                    if (Session.TryGetValue("TokenRecaptchaValidados", out _))
                    {
                        listaTokensValidados = Session.Get<List<string>>("TokenRecaptchaValidados");
                    }

                    try
                    {
                        string response = Request.Headers["g-recaptcha-response"];
                        CaptchaResponse captchaResponse = null;

                        if (!string.IsNullOrEmpty(response))
                        {
                            if (listaTokensValidados.Contains(response))
                            {
                                // El token ya había sido validado para este usuario, no lo valido contra google de nuevo porque fallaría
                                return resultado;
                            }
                            //secret that was generated in key value pair
                            string secret = filaParametroGoogleRecaptchaSecret.Valor;
                            mensaje += "\r\nHay fila google recaptcha secret: " + secret;

                            WebClient client = new WebClient();
                            string reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
                            client.Dispose();

                            mensaje += "\r\nPetición OK: " + reply;

                            captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);
                            mensaje += "\r\nDeserialización OK";
                        }


                        if (captchaResponse != null && captchaResponse.Success)
                        {
                            listaTokensValidados.Add(response);
                            Session.Set("TokenRecaptchaValidados", listaTokensValidados);
                        }
                        else
                        {
                            //when response is false check for the error message
                            mensaje += "\r\nNo funcionó, devuelvo error: " + response;
                            resultado = Content("{\"Status\":\"Error\", \"Message\":\"" + UtilIdiomas.GetText("REGISTRO", "ERRORRECAPTCHA") + "\", \"Html\":null,\"UrlRedirect\":null}");
                            HttpContext.Response.ContentType = "application/json";
                        }
                    }
                    catch (Exception ex)
                    {
                        mensaje += "\r\nError: " + ex.Message + "\r\nPila: " + ex.StackTrace;
                    }
                }

                if (resultado != null)
                {
                    GuardarLogError(mensaje);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la URL a la que hay que redireccionar al usuario recien logueado
        /// </summary>
        /// <returns></returns>
        public string ObtenerUrlHomeConectado()
        {
            string urlHomeConectado = "";
            EntityContext context = mEntityContext;
            ParametroAplicacion filaParametro = context.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == TiposParametrosAplicacion.UrlHomeConectado);

            //ParametroAplicacionDS.ParametroAplicacionRow filaParametro = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.UrlHomeConectado);

            if (filaParametro != null && !string.IsNullOrEmpty(filaParametro.Valor) && !filaParametro.Valor.ToLower().Equals("##comunidadorigen##"))
            {
                if (filaParametro.Valor.StartsWith("http://") || filaParametro.Valor.StartsWith("https://"))
                {
                    urlHomeConectado = filaParametro.Valor;
                }
                else if (UtilIdiomas.ObtenerDiccionarioDePagina("URLSEM").ContainsValue(filaParametro.Valor))
                {
                    urlHomeConectado = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto)}/{UtilIdiomas.GetText("URLSEM", filaParametro.Valor)}";
                }
                else
                {
                    urlHomeConectado = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto)}/{filaParametro.Valor}";
                }
            }

            return urlHomeConectado;
        }

        /// <summary>
        /// Obtiene la url siguiente para el asistente de creación de comunidad
        /// Tambien introduce en ViewBab.UrlSiguienteAsistente la url actual, por si se quiere volver al paso anterior
        /// </summary>
        /// <returns></returns>
        protected string ObtenerUrlSiguientePasoAsistenteNuevaComunidad()
        {
            string redirect = null;
            if (!string.IsNullOrEmpty(RequestParams("new-community-wizard")))
            {
                int paso;
                if (int.TryParse(RequestParams("new-community-wizard"), out paso))
                {
                    EntityContext context = mEntityContext;
                    AD.EntityModel.ParametroAplicacion filaParametroPasos = context.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == TiposParametrosAplicacion.PasosAsistenteCreacionComunidad);
                    //ParametroAplicacionDS.ParametroAplicacionRow filaParametroPasos = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.PasosAsistenteCreacionComunidad);

                    if (filaParametroPasos != null && !string.IsNullOrEmpty(filaParametroPasos.Valor))
                    {
                        string[] pasos = filaParametroPasos.Valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (pasos.Length > paso)
                        {
                            string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                            redirect = urlComunidad + "/" + UtilIdiomas.GetText("URLSEM", pasos[paso]);
                            paso++;
                            if (pasos.Length > paso)
                            {
                                redirect += "?new-community-wizard=" + paso;
                            }

                            ViewBag.UrlSiguienteAsistente = redirect;
                        }
                    }
                }
            }

            return redirect;
        }

        /// <summary>
        /// Indica si deben mostrar los datos demograficos del perfil
        /// </summary>
        public bool MostrarDatosDemograficosPerfil
        {
            get
            {
                if (!mMostrarDatosDemograficosPerfil.HasValue)
                {
                    mMostrarDatosDemograficosPerfil = true;
                    EntityContext context = mEntityContext;
                    List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.DatosDemograficosPerfil).ToList();
                    if (busqueda.Count > 0)
                    {
                        mMostrarDatosDemograficosPerfil = bool.Parse(busqueda.FirstOrDefault().Valor);
                    }
                }
                return mMostrarDatosDemograficosPerfil.Value;
            }
        }

        /// <summary>
        /// Obtiene si el CV va a ser unico por perfil
        /// </summary>
        public bool CVUnicoPorPerfil
        {
            get
            {
                if (!mCVUnicoPorPerfil.HasValue)
                {
                    EntityContext context = mEntityContext;
                    List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.CVUnicoPorPerfil).ToList();
                    mCVUnicoPorPerfil = busqueda.Count > 0 && bool.Parse(busqueda.FirstOrDefault().Valor);
                    //mCVUnicoPorPerfil = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.CVUnicoPorPerfil.ToString() + "'").Length > 0 && bool.Parse((string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.CVUnicoPorPerfil.ToString() + "'")[0]["Valor"]);
                }
                return mCVUnicoPorPerfil.Value;
            }
        }


        /// <summary>
        /// Url de la ontología de curriculum.
        /// </summary>
        public string UrlOntologiaCurriculum
        {
            get
            {
                return BaseURLFormulariosSem + "/Ontologia/Curriculum.owl#";
            }
        }

        public DataWrapperPais PaisesDW
        {
            get
            {
                if (mPaisDW == null)
                {
                    PaisCL paisCL = new PaisCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mPaisDW = (DataWrapperPais)paisCL.ObtenerPaisesProvincias();
                }
                return mPaisDW;
            }
        }

        /// <summary>
        /// Devuelve la URL del servicio de documentación.
        /// </summary>
        public string UrlServicioWebDocumentacion
        {
            get
            {
                return mControladorBase.UrlServicioWebDocumentacion;
            }
        }

        /// <summary>
        /// Indica si hay que subir los recursos a GoogleDrive
        /// </summary>
        public bool TieneGoogleDriveConfigurado
        {
            get
            {
                return mControladorBase.TieneGoogleDriveConfigurado;
            }
        }
    }

    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }

    /// <summary>
    /// Identifica la acción que está realizando la carga
    /// </summary>
    public enum TipoAccionCarga
    {
        ObtenerEtiquetas = 0,
        GuardarAdjuntos = 1,
        ServicioReplicacionProcesado = 2,
        ServicioBaseProcesado = 3,
        ServicioProcesadoTareas = 4,

        ServicioReplicacionBorrado_CapturaImg = 5,
        ServicioBaseBorrado_CapturaImg = 6,
        ServicioReplicacionProcesado_CapturaImg = 7,
        ServicioBaseProcesado_CapturaImg = 8,

        EliminarRecursoDelModeloAcido = 9,
        ServicioReplicacion_RecursoBorrado = 10,
        ServicioBase_RecursoBorrado = 11,
        ServicioApiRecursos_AgregarRecurso = 12,
        ServicioApiRecursos_ModificarRecurso = 13,

        Web_GuardarRDFRecursoSemantico_ServicioExterno = 14
    }
}
