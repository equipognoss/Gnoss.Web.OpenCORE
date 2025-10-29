using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Sitemaps;
using Es.Riam.Util;
using Gnoss.Web.Open.Properties;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Es.Riam.Gnoss.Elementos.Amigos;

namespace Es.Riam.Gnoss.Web.MVC
{

    public class RouteConfig
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private RedisCacheWrapper mRedisCacheWrapper;
        private VirtuosoAD mVirtuosoAD;
        private UtilIdiomasFactory mUtilIdiomasFactory;
        private Regex mRegExprRoutes;
        protected Elementos.ServiciosGenerales.Proyecto mProyecto;
        protected string mIdiomaUsuario;
        protected IHttpContextAccessor mHttpContextAccessor;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        //private IHttpContextAccessor mHttpContextAccessor;
        //private GnossCache mGnossCache;
        //private EntityContextBASE mEntityContextBASE;
        //private ControladorBase mControladorBase;
        public static IRouteBuilder RouteBuilder { get; set; }

        public RouteConfig(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ILogger<RouteConfig> logger, ILoggerFactory loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mVirtuosoAD = virtuosoAD;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            //mHttpContextAccessor = httpContextAccessor;
            //mGnossCache = gnossCache;
            //mEntityContextBASE = entityContextBASE;
            mRedisCacheWrapper = redisCacheWrapper;
            mUtilIdiomasFactory = new UtilIdiomasFactory(mLoggingService, mEntityContext, mConfigService, redisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomasFactory>(), mLoggerFactory);
            mHttpContextAccessor = httpContextAccessor;
            //mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor);
            mRegExprRoutes = new Regex("(@#@\\$[^@#@$]*,\\|,[^@#@$]*\\$@#@)");
        }

        public static string IdiomaPrincipalDominio = "";

        private static bool mRecalculandoRutasPaginas = false;

        /// <summary>
        /// Lista de correspondencias entre rutas y name
        /// </summary>
        private static Dictionary<string, string> mListaRutasURLName = new Dictionary<string, string>();

        /// <summary>
        /// Lista de las idiomas que se han cargado sus URLs
        /// </summary>
        private static List<string> mListaIdiomasCargados = new List<string>();

        /// <summary>
        /// Lista de correspondencias entre rutas y name
        /// </summary>
        public static Dictionary<string, string> ListaRutasURLName
        {
            get { return mListaRutasURLName; }
        }

        /// <summary>
        /// Lista de las idiomas que se han cargado sus URLs
        /// </summary>
        public static List<string> ListaIdiomasCargados
        {
            get { return mListaIdiomasCargados; }
        }

        /// <summary>
        /// Nombre del proyecto sin nombrecorto (no hace falta poner comunidad/nombrecorto)
        /// </summary>
        private static string mNombreProyectoSinNombreCorto = null;
        public Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado
        {
            get
            {
                if (mProyecto == null)
                {
                    string nombreCortoProyecto = null;
                    Guid proyectoID = Guid.Empty;

                    if (RequestParams("proyectoID") != null)
                    {
                        proyectoID = new Guid(RequestParams("proyectoID"));
                    }
                    else if (RequestParams("nombreProy") != null)
                    {
                        nombreCortoProyecto = RequestParams("nombreProy");
                    }
                    else if (RequestParams("NombreCortoComunidad") != null)
                    {
                        nombreCortoProyecto = RequestParams("NombreCortoComunidad");
                    }
                    else if (RequestParams("proy") != null)
                    {
                        proyectoID = new Guid(RequestParams("proy"));
                    }
                    else if (RequestParams("pProyectoID") != null)
                    {
                        Guid.TryParse(RequestParams("pProyectoID").Replace("\"", ""), out proyectoID);
                    }
                    else
                    {
                        proyectoID = ProyectoAD.MetaProyecto;
                    }

                    if (!string.IsNullOrEmpty(RequestParams("ecosistema")) && RequestParams("ecosistema").Equals("true"))
                    {
                        proyectoID = ProyectoAD.MetaProyecto;
                        nombreCortoProyecto = "mygnoss";
                    }

                    if (mProyecto == null || (nombreCortoProyecto != mProyecto.NombreCorto && mProyecto.Clave != proyectoID))
                    {
                        ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);

                        if (!string.IsNullOrEmpty(nombreCortoProyecto))
                        {
                            proyectoID = proyectoCL.ObtenerProyectoIDPorNombreCorto(nombreCortoProyecto);
                        }

                        GestionProyecto gestorProyecto = new GestionProyecto(proyectoCL.ObtenerProyectoPorID(proyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);

                        if (gestorProyecto.ListaProyectos.Count > 0 && gestorProyecto.ListaProyectos.ContainsKey(proyectoID))
                        {
                            mProyecto = gestorProyecto.ListaProyectos[proyectoID];
                        }

                        if (mProyecto == null)
                        {
                            return null;
                        }
                        
                    }
                    return mProyecto;
                }
                return mProyecto;
            }
            set
            {
                mProyecto = value;
            }
        }

        public string RequestParams(string pParametro)
        {
            string valorParametro = null;

            try
            {
                if(mHttpContextAccessor != null && mHttpContextAccessor.HttpContext != null && mHttpContextAccessor.HttpContext.Request != null)
                {
                    if (mHttpContextAccessor.HttpContext.Request.Query.ContainsKey(pParametro))
                    {
                        valorParametro = mHttpContextAccessor.HttpContext.Request.Query[pParametro];
                    }
                    else if (mHttpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(pParametro))
                    {
                        valorParametro = mHttpContextAccessor.HttpContext.Request.RouteValues[pParametro] as string;

                    }
                    else if (mHttpContextAccessor.HttpContext.Request.HasFormContentType && mHttpContextAccessor.HttpContext.Request.Form != null && mHttpContextAccessor.HttpContext.Request.Form.ContainsKey(pParametro))
                    {
                        valorParametro = mHttpContextAccessor.HttpContext.Request.Form[pParametro];
                    }
                }               
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex.Message, mlogger);
            }

            return valorParametro;
        }

        /// <summary>
        /// Obtiene el idioma del usuario
        /// </summary>
        public string IdiomaUsuario
        {
            get
            {
                if (mIdiomaUsuario == null)
                {
                    mIdiomaUsuario = RequestParams("lang");
                }
                return mIdiomaUsuario;
            }
            set
            {
                mIdiomaUsuario = value;
            }
        }

        /// <summary>
        /// Nombre del proyecto sin nombrecorto (no hace falta poner comunidad/nombrecorto)
        /// </summary>
        public string NombreProyectoSinNombreCorto
        {
            get
            {
                if (mNombreProyectoSinNombreCorto == null)
                {
                    //Obtenemos el ProyectoSinNombreCorto para registrar las URLs de forma diferente
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    Guid? idProyectoPrincipal = mConfigService.ObtenerProyectoConexion();
                    if (idProyectoPrincipal == null || idProyectoPrincipal.Equals(Guid.Empty))
                    {
                        if(ProyectoSeleccionado != null)
                        {
                            string valor = paramCN.ObtenerParametroAplicacion(ProyectoSeleccionado.UrlPropia(IdiomaUsuario));

                            if (!string.IsNullOrEmpty(valor))
                            {
                                idProyectoPrincipal = new Guid(valor);
                            }
                        }
                    }
                    if (idProyectoPrincipal.HasValue)
                    {
                        ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
                        Dictionary<string, string> listaParametros = parametroCN.ObtenerParametrosProyecto(idProyectoPrincipal.Value);
                        bool proyectoSinURL = listaParametros.ContainsKey(ParametroAD.ProyectoSinNombreCortoEnURL) && listaParametros[ParametroAD.ProyectoSinNombreCortoEnURL] == "1";
                        parametroCN.Dispose();
                        if (proyectoSinURL)
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mNombreProyectoSinNombreCorto = proyCN.ObtenerNombreCortoProyecto(idProyectoPrincipal.Value);
                            proyCN.Dispose();
                        }
                    }
                    if (mNombreProyectoSinNombreCorto == null)
                    {
                        mNombreProyectoSinNombreCorto = "";
                    }
                }
                return mNombreProyectoSinNombreCorto;
            }
        }

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro ComunidadPadreEcosistemaID (comunidad/nombrecorto)
        /// </summary>
        private static string mNombreProyectoPadreEcositema = null;

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro NombreCortoProyectoPadreEcosistema (comunidad/nombrecorto)
        /// </summary>
        private static string mNombreCortoProyectoPadreEcosistema = null;

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro ComunidadPadreEcosistemaID (comunidad/nombrecorto)
        /// </summary>
        public string NombreProyectoPadreEcositema
        {
            get
            {
                if (mNombreProyectoPadreEcositema == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                    List<AD.EntityModel.ParametroAplicacion> parametrosAplicacionDS = paramCL.ObtenerParametrosAplicacionPorContext();
                    ParametroAplicacion ComunidadPadreEcosistemaID = parametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("ComunidadPadreEcosistemaID"));
                    paramCL.Dispose();
                    if (ComunidadPadreEcosistemaID != null)
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mNombreProyectoPadreEcositema = proyCN.ObtenerNombreCortoProyecto(Guid.Parse(ComunidadPadreEcosistemaID.Valor));
                            proyCN.Dispose();
                        }
                        catch
                        {
                            GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.");
                            mNombreProyectoPadreEcositema = "";
                        }
                    }
                    if (mNombreProyectoPadreEcositema == null)
                    {
                        mNombreProyectoPadreEcositema = "";
                    }
                }
                return mNombreProyectoPadreEcositema;
            }
        }

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro NombreCortoProyectoPadreEcosistema (comunidad/nombrecorto)
        /// </summary>
        public string NombreCortoProyectoPadreEcositema
        {
            get
            {
                if (mNombreProyectoPadreEcositema == null)
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    string NombreCortoEcosistema = paramCN.ObtenerParametroAplicacion("NombreCortoProyectoPadreEcositema");
                    paramCN.Dispose();
                    if (!string.IsNullOrEmpty(NombreCortoEcosistema))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mNombreProyectoPadreEcositema = NombreCortoEcosistema;
                            proyCN.Dispose();
                        }
                        catch
                        {
                            GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.");
                            mNombreProyectoPadreEcositema = "";
                        }
                    }
                    if (mNombreProyectoPadreEcositema == null)
                    {
                        mNombreProyectoPadreEcositema = "";
                    }
                }
                return mNombreProyectoPadreEcositema;
            }
        }

        /// <summary>
        /// ProyectoID padre del ecosistema (comunidad/nombrecorto)
        /// </summary>
        private static Guid? mPadreEcosistemaProyectoID = null;

        /// <summary>
        /// Nombre del proyecto padre del ecosistema (comunidad/nombrecorto)
        /// </summary>
        public Guid? PadreEcosistemaProyectoID
        {
            get
            {
                if (mPadreEcosistemaProyectoID == null)
                {

                    if (!string.IsNullOrEmpty(NombreCortoProyectoPadreEcositema))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreCortoProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.");
                            mPadreEcosistemaProyectoID = Guid.Empty;
                        }
                    }

                    if (!string.IsNullOrEmpty(NombreProyectoPadreEcositema) && (mPadreEcosistemaProyectoID == null || (mPadreEcosistemaProyectoID != null && mPadreEcosistemaProyectoID.Value.Equals(Guid.Empty))))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.");
                            mPadreEcosistemaProyectoID = Guid.Empty;
                        }
                    }

                    if (mPadreEcosistemaProyectoID == null)
                    {
                        mPadreEcosistemaProyectoID = Guid.Empty;
                    }
                }
                return mPadreEcosistemaProyectoID;
            }
        }


        public static List<string> IdiomasRegistrados = new List<string>();

        private static List<Guid> mListaProyectosRutas = new List<Guid>();

        public static List<Guid> ListaProyectosRutas
        {
            get
            {
                return mListaProyectosRutas;
            }
        }

        /// <summary>
        /// Registra las rutas de la aplicacion
        /// </summary>
        /// <param name="routes"></param>
        public void RegisterRoutes(IRouteBuilder routes)
        {
            RouteBuilder = routes;
            mLoggingService.AgregarEntrada("Inicio mapeo rutas");
            string rutaEjecucionWeb = mConfigService.ObtenerRutaEjecucionWeb();
            ProcesarURL(RouteBuilder, $"{rutaEjecucionWeb}crearCookie", "CrearCookie", IdiomaPrincipalDominio);
            ProcesarURL(RouteBuilder, $"{rutaEjecucionWeb}eliminarCookie", "EliminarCookie", IdiomaPrincipalDominio);
            ProcesarURL(RouteBuilder, $"{rutaEjecucionWeb}RecargarComponenteCMS", "CMSPagina", IdiomaPrincipalDominio);
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

			Dictionary<string, string> listaIdiomas = paramCL.ObtenerListaIdiomasDictionary();

            mLoggingService.AgregarEntrada("Mapeo rutas - Registro Redirect/Home");
            //Si no coniene el idioma español, necesitamos registrar esta ruta para que funcione el dominio
            if (!listaIdiomas.ContainsKey(IdiomaPrincipalDominio))
            {
                ProcesarURL(RouteBuilder, $"{rutaEjecucionWeb}", "Redirect/Home", IdiomaPrincipalDominio);
            }

            RouteBuilder.MapRoute(
                       name: "RedireccionComunitaItaliano",
                       template: "it/comunità/{*datosextra}",
                       defaults: new { controller = "Redirect", action = "RedireccionarComunitaItaliano" });

            /*RouteBuilder.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");*/
            RegisterSitemap(routes);

            RegisterRoutesIdioma(RouteBuilder, paramCL.ObtenerListaIdiomas());

            mLoggingService.AgregarEntrada("Fin mapeo rutas");
        }

        private void RegisterSitemap(IRouteBuilder routes)
        {
            string dominio = "";
            string dominioConfig = mConfigService.ObtenerUrlBase();
            if (!string.IsNullOrEmpty(dominioConfig))
            {
                dominio = dominioConfig;
            }

            var sitemap = mEntityContext.SitemapsIndex.FirstOrDefault(item => item.Dominio.Equals(dominio));
            if (sitemap != null)
            {

                routes.MapRoute(
                           name: "Sitemap",
                           template: "sitemap.xml",
                           defaults: new { controller = "Sitemap", action = "Index" });

                List<Sitemaps> sitemaps = mEntityContext.Sitemaps.Where(item => item.Dominio.Equals(dominio)).ToList();
                foreach (var name in sitemaps)
                {
                    routes.MapRoute(
                        name: name.SitemapIndexName,
                        template: name.SitemapIndexName,
                        defaults: new { controller = "Sitemap", action = "Index" });
                }
            }
        }

        private static bool RecalculandoRutas = false;

        public void RegisterRoutesIdioma(IRouteBuilder routes, List<string> pIdiomas)
        {
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
            // Si está configurado, la web en lugar de ejecutarse en https://testing.gnoss.com/comunidad/testing3 se ejecutará en https://testing.gnoss.com/SUBRUTA/comunidad/testing3
            string rutaEjecucionWeb = mConfigService.ObtenerRutaEjecucionWeb();

			foreach (string idiomaConfig in pIdiomas)
            {
                if (!IdiomasRegistrados.Contains(idiomaConfig))
                {
                    Dictionary<string, string> listaIdiomasPlataforma = paramCL.ObtenerListaIdiomasDictionary();
                    if (!listaIdiomasPlataforma.ContainsKey(idiomaConfig))
                    {
                        return;
                    }

                    if (RecalculandoRutas)
                    {
                        int tiempoMaximo = 30000;
                        int tiempoTranscurrido = 0;
                        while (RecalculandoRutas && tiempoTranscurrido < tiempoMaximo)
                        {
                            mLoggingService.AgregarEntrada("RecalculandoRutas. Espero 1000 ms");
                            Thread.Sleep(1000);
                            tiempoTranscurrido += 1000;
                        }
                        if (IdiomasRegistrados.Contains(idiomaConfig))
                        {
                            return;
                        }
                    }
                    RecalculandoRutas = true;

                    mLoggingService.AgregarEntrada("Inicio mapeo rutas " + idiomaConfig);

                    Dictionary<string, string> listaIdiomas = new Dictionary<string, string>();
                    listaIdiomas.Add(idiomaConfig, idiomaConfig);

                    if (string.IsNullOrEmpty(NombreProyectoSinNombreCorto))
                    {
                        //Registramos la home, luego se registraran el resto de paginas
                        foreach (string idioma in listaIdiomas.Keys)
                        {
                            ProcesarURL(routes, $"{rutaEjecucionWeb}", "Redirect/Home", idioma);
                        }
                    }

                    //Obtener la comunidad Padre de Ecosistema.
                    //if (!string.IsNullOrEmpty(NombreProyectoPadreEcositema))
                    //{
                    //    foreach (string idioma in listaIdiomas.Keys)
                    //    {
                    //        ProcesarURL(routes, "", "comunidad/{nombreCorto}/Redirect/Home", idioma);
                    //    }
                    //}

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Cargar RouteMap");
                    XmlDocument xmlRoute = new XmlDocument();
                    xmlRoute.Load(new StringReader(Resources.routemap));

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Obtener Listas Rutas");
                    //Lista de urls del metaproyecto, con identidad personal y de organización
                    Dictionary<string, string> listaUrlsMetaProyecto = new Dictionary<string, string>();
                    ObtenerListaRutas(xmlRoute, listaUrlsMetaProyecto, "metaproyecto");

                    Dictionary<string, string> listaUrlsMetaAdministrador = new Dictionary<string, string>();
                    ObtenerListaRutas(xmlRoute, listaUrlsMetaAdministrador, "metaAdministrador");

                    //Lista de urls del metaproyecto, solo con identidad de organización
                    Dictionary<string, string> listaUrlsSoloOrg = new Dictionary<string, string>();
                    ObtenerListaRutas(xmlRoute, listaUrlsSoloOrg, "organizacion");

                    //Lista de urls comunes a la comunidad y al metaproyecto, con identidad personal y de organización
                    Dictionary<string, string> listaUrls = new Dictionary<string, string>();
                    ObtenerListaRutas(xmlRoute, listaUrls, "comun");

                    //Lista de urls de una comunidad
                    Dictionary<string, string> listaUrlsCom = new Dictionary<string, string>();
                    ObtenerListaRutas(xmlRoute, listaUrlsCom, "comunidad");

                    //Lista de urls de ficha de recurso de una comunidad
                    Dictionary<string, string> listaUrlsFichaRecursoCom = new Dictionary<string, string>();
                    ObtenerListaRutas(xmlRoute, listaUrlsFichaRecursoCom, "fichaRecurso");

                    xmlRoute = null;

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Registrar Rutas Recursos");
                    RegistrarRutasRecursos(routes, listaUrlsFichaRecursoCom, listaIdiomas, null);

                    // Las rutas se iran registrando a medida que los usuarios entren en los proyectos
                    //LoggingService.AgregarEntrada("Mapeo rutas - Antes Registrar Rutas Pestanyas");
                    //RegistrarRutasPestanyas(routes, listaIdiomas, null);

                    // Descomentar estas líneas para depurar CRFP. Sustituir a2952703-a9ed-4471-a7dd-5ebcab9607c1 por el ID del proyecto que se necesite depurar
                    // RegistrarRutasPestanyas(routes, listaIdiomas, new Guid("e9fd8d5e-7e8b-4e7d-9782-b1f93a4583d6"));
                    // RegistrarRutasPestanyas(routes, listaIdiomas, new Guid("a2952703-a9ed-4471-a7dd-5ebcab9607c1"));


                    //Registrar los Proyectos que tienen padres
                    //RegistrarRutasPestanyas(routes, listaIdiomas, null, null, PadreEcosistemaProyectoID.HasValue);

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Registrar Rutas MetaAdministrador");
                    foreach (string url in listaUrlsMetaAdministrador.Keys)
                    {
                        foreach (string idioma in listaIdiomas.Keys)
                        {
                            ProcesarURL(routes, $"{rutaEjecucionWeb}{url}", listaUrlsMetaAdministrador[url], idioma);
                        }
                    }

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Registrar Rutas RutasComunes - Metaproyecto");
                    if (string.IsNullOrEmpty(NombreProyectoSinNombreCorto))
                    {
                        foreach (string url in listaUrls.Keys)
                        {
                            foreach (string idioma in listaIdiomas.Keys)
                            {
                                ProcesarURL(routes, $"{rutaEjecucionWeb}{url}", listaUrls[url], idioma);
                                ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,IDENTIDAD$@#@/{{nombreOrgRewrite}}/{url}", listaUrls[url], idioma);
                            }
                        }                        
                    }
                    // registrar las rutas del metaproyecto
                    foreach (string url in listaUrlsMetaProyecto.Keys)
                    {
                        if (!url.Equals("@#@$URLSEM,|,HOME$@#@") || string.IsNullOrEmpty(NombreProyectoSinNombreCorto))
                        {
                            foreach (string idioma in listaIdiomas.Keys)
                            {
                                
                                ProcesarURL(routes, $"{rutaEjecucionWeb}{url}", listaUrlsMetaProyecto[url], idioma);
                                ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,IDENTIDAD$@#@/{{nombreOrgRewrite}}/{url}", listaUrlsMetaProyecto[url], idioma);
                            }
                        }
                        else
                        {                           
                            foreach (string idioma in listaIdiomas.Keys)
                            {
                                ProcesarURL(routes, $"{rutaEjecucionWeb}{url}", listaUrlsMetaProyecto[url], idioma);
                                ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,IDENTIDAD$@#@/{{nombreOrgRewrite}}/{url}", listaUrlsMetaProyecto[url], idioma);
                            }
                        }
                    }

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Registrar Rutas Solo Organizacion");
                    foreach (string url in listaUrlsSoloOrg.Keys)
                    {
                        foreach (string idioma in listaIdiomas.Keys)
                        {
                            ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,IDENTIDAD$@#@/{{nombreOrgRewrite}}/{url}", listaUrlsSoloOrg[url], idioma);
                        }
                    }

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Registrar RedireccionProyectoSinNombreCorto");
                    //Si la comunidad está configurada sin nombrecorto
                    if (!string.IsNullOrEmpty(NombreProyectoSinNombreCorto))
                    {
                        foreach (string idioma in listaIdiomas.Keys)
                        {
                            ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,COMUNIDAD$@#@/{NombreProyectoSinNombreCorto}/{{*datosextra}}", "Redirect/TablaRedireccionamiento", idioma);
                            ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,COMUNIDAD$@#@/{NombreProyectoSinNombreCorto}", "Redirect/TablaRedireccionamiento", idioma);
                        }
                    }

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Registrar Rutas Comunidad");
                    foreach (string url in listaUrlsCom.Keys)
                    {
                        foreach (string idioma in listaIdiomas.Keys)
                        {
                            if (!string.IsNullOrEmpty(NombreProyectoSinNombreCorto))
                            {
                                ProcesarURL(routes, $"{rutaEjecucionWeb}{url}", listaUrlsCom[url] + "?nombreProy=" + NombreProyectoSinNombreCorto, idioma);
                            }
                            ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,COMUNIDAD$@#@/{{nombreProy}}/{url}", listaUrlsCom[url], idioma);
                        }
                    }

                    mLoggingService.AgregarEntrada("Mapeo rutas - Antes Registrar RutasComunes - Proyectos");
                    foreach (string url in listaUrls.Keys)
                    {
                        foreach (string idioma in listaIdiomas.Keys)
                        {
                            if (!string.IsNullOrEmpty(NombreProyectoSinNombreCorto))
                            {
                                ProcesarURL(routes, $"{rutaEjecucionWeb}{url}", listaUrls[url] + "?nombreProy=" + NombreProyectoSinNombreCorto, idioma);
								ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,IDENTIDAD$@#@/{{nombreOrgRewrite}}/{url}", listaUrls[url], idioma);
							}
                            ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,COMUNIDAD$@#@/{{nombreProy}}/{url}", listaUrls[url], idioma);						
						}
                    }

                    RegistrarRutaSoloGuid(routes, listaIdiomas);

                    mLoggingService.AgregarEntrada("Fin mapeo rutas " + pIdiomas);

                    IdiomasRegistrados.Add(idiomaConfig);
                    RecalculandoRutas = false;
                }
            }
        }

        private static readonly object bloqueoRecalculoRutas = new object();
        private static bool recalcularRutas = false;

        public void RecalcularTablaRutas()
        {
            try
            {
                mLoggingService.GuardarLogError("RecalcularTablaRutas", mlogger);
                recalcularRutas = true;
                lock (bloqueoRecalculoRutas)
                {
                    if (recalcularRutas)
                    {
                        RouteBuilder.Routes.Clear();
                        IdiomasRegistrados.Clear();
                        RegistrarRutasRedireccionamiento(RouteBuilder);
                        RegisterRoutes(RouteBuilder);
                        // Establezco esta variable a false por si había algún otro proceso bloqueado esperando para recalcular rutas
                        // Ya las ha recalculado este proceso, los que estaban esperando a que se liberase el proceso no hace falta que las recalculen de nuevo. 
                        recalcularRutas = false;
                    }
                }

            }
            catch (Exception ex)
            {
                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                throw;
            }
        }

        public void RegistrarRutasRedireccionamiento(IRouteBuilder pRoutes)
        {
            try
            {
                string dominio = "";
                string dominioConfig = mConfigService.ObtenerDominio();
                if (!string.IsNullOrEmpty(dominioConfig))
                {
                    dominio = dominioConfig;
                }

                mLoggingService.GuardarLogError("RegistrarRutasRedireccionamiento. Domminio: " + dominio, mlogger);

                if (!string.IsNullOrEmpty(dominio))
                {
                    try
                    {
                        RegistrarRutasRedireccionamientoModeloEF(pRoutes, dominio);
                    }
                    catch (Exception ex)
                    {
                        GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    }

                }
            }
            catch (Exception ex)
            {
                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }

        private void RegistrarRutasRedireccionamientoModeloEF(IRouteBuilder pRoutes, string dominio)
        {
            mLoggingService.AgregarEntrada("RegistrarRutasRedireccionamientoModeloEF - INICIO");

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            List<RedireccionRegistroRuta> filasRedirecciones = proyCN.ObtenerRedireccionRegistroRutaPorDominio(dominio, false);
            proyCN.Dispose();
            int i = pRoutes.Routes.Count;

            mLoggingService.AgregarEntrada("Antes del registro rutas RedireccionRegistroRuta. Número redirecciones: " + filasRedirecciones.Count);

            if (filasRedirecciones != null && filasRedirecciones.Count > 0)
            {
                foreach (RedireccionRegistroRuta redireccion in filasRedirecciones)
                {
                    mLoggingService.AgregarEntrada("Registrando la redireccion: " + redireccion.RedireccionID + " del dominio: " + redireccion.Dominio);

                    //contador para hacer unico el nombre. Es interno, yo no lo uso para nada
                    i++;
                    RouteValueDictionary constraints = new RouteValueDictionary();

                    pRoutes.MapRoute(
                        name: "Redireccion" + i,
                        template: redireccion.UrlOrigen,
                        defaults: new { controller = "Redirect", action = "RedireccionamientoModeloEF", redireccionID = redireccion.RedireccionID },
                        constraints: constraints);
                }
            }
            mLoggingService.AgregarEntrada("RegistrarRutasRedireccionamientoModeloEF - FIN");
        }



        private static readonly object registrPestanyaLock = new object();

        /// <summary>
        /// Obtiene las rutas de las pestañas de una o todas las comunidades
        /// </summary>
        /// <param name="routes">RouteCollection</param>
        /// <param name="listaIdiomas">Lista de idiomas de la aplicacion para mapear las rutas en todos los idiomas</param>
        /// <param name="pProyectoID">Proyecto del que vamos a obtener las rutas de las pestañas, si es null carga de todos los proyectos</param>
        public void RegistrarRutasPestanyas(IRouteBuilder routes, Dictionary<string, string> listaIdiomas, Guid? pProyectoID, List<string> pRutasPestanyasRegistrar = null, bool pRegistroRutasEcosistema = false)
        {
            //Si cambiamos una pestaña en base de datos, entrar en la url ".../comunidad/.../administrar-comunidad-general?recalcularPestanyas=true" esto recalcula todas las pestañas de esta comunidad.
            //Si se cambia una pestaña con el XML, se recalculan solas

            //if (!pProyectoID.HasValue)
            //{
            //    return;
            //}
            //else if (!mListaProyectosRutas.Contains(pProyectoID.Value))
            //{
            //    mListaProyectosRutas.Add(pProyectoID.Value);
            //}

            lock (registrPestanyaLock)
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                ////Posible cambio en las rutas: Comunidad padre
                //Proyecto filaProyecto = proyCL.ObtenerFilaProyecto(pProyectoID.Value);
                ////El valor de proyectoIDPadre sera el del padre si esta conigurado el parametro y es el mismo que tiene en la fila de proyecto, si no sera el valor del proyecto seleccionado.
                //Guid proyectoIDPadre = filaProyecto.ProyectoSuperiorID.HasValue ? filaProyecto.ProyectoSuperiorID.Value : Guid.Empty;
                //bool proyectoHijoEcosistema = EsHijoEcosistemaProyecto(proyectoIDPadre);
                //try
                //{
                //    if (PadreEcosistemaProyectoID.HasValue)
                //    {
                //        if (!proyectoIDPadre.Equals(PadreEcosistemaProyectoID.Value))
                //        {
                //            proyectoIDPadre = pProyectoID.Value;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    proyectoIDPadre = pProyectoID.Value;
                //}


                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                DataWrapperProyecto dataWrapperProyectoPestanyas = new DataWrapperProyecto();
                //Obtener los proyecto que no tienen proyectoSuperiorID

                if (pRegistroRutasEcosistema)
                {
                    proyCN.ObtenerPestanyasProyecto(PadreEcosistemaProyectoID.Value, dataWrapperProyectoPestanyas, true);
                }
                else
                {
                    proyCN.ObtenerPestanyasProyecto(pProyectoID, dataWrapperProyectoPestanyas, true);
                }

                proyCN.Dispose();

                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya in dataWrapperProyectoPestanyas.ListaProyectoPestanyaMenu)
                {
                    string nombreCorto = proyCL.ObtenerNombreCortoProyecto(filaPestanya.ProyectoID);

                    if (!mListaProyectosRutas.Contains(filaPestanya.ProyectoID))
                    {
                        mListaProyectosRutas.Add(filaPestanya.ProyectoID);
                    }

                    TipoPestanyaMenu tipoPestanya = (TipoPestanyaMenu)filaPestanya.TipoPestanya;

                    string nombreCortoProy = nombreCorto;
                    string pagina = string.Empty;
                    string ruta = filaPestanya.Ruta;
                    /*if (!mListaProyectosRutas.Contains(filaPestanya.ProyectoID))
                    {
                        mListaProyectosRutas.Add(filaPestanya.ProyectoID);
                    }*/

                    if (ruta.Contains("?"))
                    {
                        ruta = ruta.Substring(0, ruta.IndexOf("?"));
                    }
                    string rutaAuxInt = ruta;

                    if (filaPestanya.Activa && (pRutasPestanyasRegistrar == null || pRutasPestanyasRegistrar.Contains(rutaAuxInt)))
                    {
                        bool esPaginaBusqueda = false;
                        switch (tipoPestanya)
                        {
                            case TipoPestanyaMenu.Home:
                                pagina = "HomeComunidad?PestanyaID=" + filaPestanya.PestanyaID;
                                break;
                            case TipoPestanyaMenu.Indice:
                                pagina = "Indice?PestanyaID=" + filaPestanya.PestanyaID;
                                break;
                            case TipoPestanyaMenu.Recursos:
                                pagina = "Busqueda?Recursos=true?PestanyaID=" + filaPestanya.PestanyaID;
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.Preguntas:
                                pagina = "Busqueda?preguntas=true?PestanyaID=" + filaPestanya.PestanyaID;
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.Debates:
                                pagina = "Busqueda?debates=true?PestanyaID=" + filaPestanya.PestanyaID;
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.Encuestas:
                                pagina = "Busqueda?encuestas=true?PestanyaID=" + filaPestanya.PestanyaID;
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.PersonasYOrganizaciones:
                                pagina = "Busqueda?perorg=true?PestanyaID=" + filaPestanya.PestanyaID;
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.AcercaDe:
                                pagina = "AcercaDeComunidad?PestanyaID=" + filaPestanya.PestanyaID;
                                break;
                            case TipoPestanyaMenu.CMS:
                                pagina = "CMSPagina?PestanyaID=" + filaPestanya.PestanyaID;
                                break;
                            case TipoPestanyaMenu.BusquedaSemantica:
                                pagina = "Busqueda?semanticos=true?PestanyaID=" + filaPestanya.PestanyaID;
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.BusquedaAvanzada:
                                pagina = "Busqueda?Meta=true?PestanyaID=" + filaPestanya.PestanyaID;
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.EnlaceInterno:
                                break;
                            case TipoPestanyaMenu.EnlaceExterno:
                                break;
                        }


                        if (!string.IsNullOrEmpty(pagina))
                        {
                            pagina += "?nombreProy=" + nombreCortoProy;

                            bool error = false;
                            if (ruta.StartsWith("http://") || ruta.StartsWith("https://"))
                            {
                                error = true;
                            }
                            else
                            {
                                UtilIdiomasFactory utilIdiomasFactory = new UtilIdiomasFactory(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomasFactory>(), mLoggerFactory);
                                foreach (string idioma in listaIdiomas.Keys)
                                {
                                    UtilIdiomas utilIdiomasActual = utilIdiomasFactory.ObtenerUtilIdiomas(idioma);

                                    try
                                    {
                                        string rutaAux = UtilCadenas.ObtenerTextoDeIdioma(ruta, idioma, null).Trim();

                                        if (pRegistroRutasEcosistema)
                                        {
                                            rutaAux = "@#@$URLSEM,|,COMUNIDAD$@#@/{nombreProy}/" + rutaAux;
                                        }
                                        else if (string.IsNullOrEmpty(NombreProyectoSinNombreCorto) || NombreProyectoSinNombreCorto != nombreCortoProy)
                                        {
                                            rutaAux = "@#@$URLSEM,|,COMUNIDAD$@#@/" + nombreCortoProy + "/" + rutaAux;
                                        }

                                        if (esPaginaBusqueda)
                                        {
                                            rutaAux += "/{*searchInfo}";
                                        }

                                        ProcesarURL(routes, rutaAux, pagina, idioma);
                                    }
                                    catch (Exception)
                                    {
                                        //GuardarLogError(ex.StackTrace);
                                        error = true;
                                    }
                                }
                            }

                            if (error)
                            {
                                mLoggingService.GuardarLogError($"La ruta '{nombreCortoProy}/{ruta}' NO es una ruta valida o no esta bien configurada", mlogger);
                            }
                        }
                    }
                }

                //invalidamos caché
                if (pProyectoID.HasValue)
                {
                    // ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD);
                    proyCL.InvalidarPestanyasProyecto(pProyectoID.Value);
                    proyCL.InvalidarFilaProyecto(pProyectoID.Value);
                    proyCL.Dispose();
                }
            }
        }

        private void RegistrarRutasRecursos(IRouteBuilder routes, Dictionary<string, string> listaUrlsFichaRecursoCom, Dictionary<string, string> listaIdiomas, Guid? pProyectoID)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Dictionary<string, Dictionary<Guid, string>> diccionarioProyectoOntologias = proyCN.ObtenerNombresCortosProyectosConNombresCortosOntologias(pProyectoID);
            proyCN.Dispose();
            string rutaEjecucionWeb = mConfigService.ObtenerRutaEjecucionWeb();
            List<string> tiposRecursoFijos = new List<string>();
            tiposRecursoFijos.Add("@#@$URLSEM,|,RECURSO$@#@");
            tiposRecursoFijos.Add("@#@$URLSEM,|,PREGUNTA$@#@");
            tiposRecursoFijos.Add("@#@$URLSEM,|,DEBATE$@#@");
            tiposRecursoFijos.Add("@#@$URLSEM,|,ENCUESTA$@#@");

            foreach (string url in listaUrlsFichaRecursoCom.Keys)
            {
                string pagina = listaUrlsFichaRecursoCom[url];

                if (string.IsNullOrEmpty(NombreProyectoSinNombreCorto))
                {
                    string urlBRProcesar = "@#@$URLSEM,|,RECURSO$@#@/" + url;
                    string urlOrgProcesar = "@#@$URLSEM,|,RECURSOORG$@#@/" + url;

                    foreach (string idioma in listaIdiomas.Keys)
                    {
                        ProcesarURL(routes, $"{rutaEjecucionWeb}{urlBRProcesar}", pagina, idioma);
                        ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,IDENTIDAD$@#@/{{nombreOrgRewrite}}/{urlBRProcesar}", pagina, idioma);
                        ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,IDENTIDAD$@#@/{{nombreOrgRewrite}}/{urlOrgProcesar}", pagina + "?organizacion=true", idioma);

                        //Registramos la url de un recurso en un perfil que no es el tuyo
                        ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,PERSONA$@#@/{{nombreCortoPerfil}}/{urlBRProcesar}", pagina + "?VerRecursosPerfil=true", idioma);
                        ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,IDENTIDAD$@#@/{{nombreOrgRewrite}}/@#@$URLSEM,|,PERSONA$@#@/{{nombreCortoPerfil}}/{urlBRProcesar}", pagina + "?VerRecursosPerfil=true", idioma);
                    }
                }

                foreach (string tipoRecurso in tiposRecursoFijos)
                {
                    foreach (string idioma in listaIdiomas.Keys)
                    {
                        string urlProcesar = tipoRecurso + "/" + url;

                        if (!string.IsNullOrEmpty(NombreProyectoSinNombreCorto))
                        {
                            ProcesarURL(routes, $"{rutaEjecucionWeb}{urlProcesar}", pagina + "?nombreProy=" + NombreProyectoSinNombreCorto, idioma);
                        }
                        ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,COMUNIDAD$@#@/{{nombreProy}}/{urlProcesar}", pagina, idioma);
                    }
                }

                string ruta = url;
                foreach (string nombreCortoProyecto in diccionarioProyectoOntologias.Keys)
                {
                    string paginaConProy = pagina + "?nombreProy=" + nombreCortoProyecto;
                    foreach (string ontologia in diccionarioProyectoOntologias[nombreCortoProyecto].Values)
                    {
                        foreach (string idioma in listaIdiomas.Keys)
                        {
                            string rutaAux = UtilCadenas.ObtenerTextoDeIdioma(ontologia, idioma, null);
                            try
                            {
                                if (string.IsNullOrEmpty(NombreProyectoSinNombreCorto) || NombreProyectoSinNombreCorto != nombreCortoProyecto)
                                {
                                    ProcesarURL(routes, $"{rutaEjecucionWeb}@#@$URLSEM,|,COMUNIDAD$@#@/{nombreCortoProyecto}/{rutaAux}/{ruta}", paginaConProy, idioma);
                                }
                                else
                                {
                                    ProcesarURL(routes, $"{rutaEjecucionWeb}{rutaAux}/{ruta}", paginaConProy, idioma);
                                }
                            }
                            catch
                            {
                                mLoggingService.GuardarLog($"La ruta '{rutaEjecucionWeb}{nombreCortoProyecto}/{rutaAux}/{ruta}' NO es una ruta valida o no esta bien configurada", mlogger);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene la lista de rutas de una seccion del fichero RouteMap.xml
        /// </summary>
        /// <param name="xmlRoute">Fichero RouteMap.xml</param>
        /// <param name="listaUrls">Lista en la que vamos a guardar las urls leídas</param>
        /// <param name="seccion">Sección de la que vamos a leer las rutas</param>
        private static void ObtenerListaRutas(XmlDocument xmlRoute, Dictionary<string, string> listaUrls, string seccion)
        {
            XmlNodeList listaNodos = xmlRoute.SelectNodes("routeMap/seccion[@name='" + seccion + "']/pagina");
            foreach (XmlNode nodo in listaNodos)
            {
                string urlMapeo = nodo["key"].InnerText.Replace("@#@@", "@#@$").Replace("@@#@", "$@#@");

                listaUrls.Add(urlMapeo, nodo["value"].InnerText);
            }
        }

        /// <summary>
        /// Convierte una url en multi-idioma al idioma deseado
        /// </summary>
        /// <param name="url">Url en multi-idioma</param>
        /// <param name="idioma">Idioma deseado</param>
        /// <returns>Devuelve una url convertida al idioma deseado</returns>
        public string ConvertirUrlAIdioma(string url, string idioma)
        {
            return ConvertirUrlAIdioma(url, idioma, true);
        }

        /// <summary>
        /// Convierte una url en multi-idioma al idioma deseado
        /// </summary>
        /// <param name="url">Url en multi-idioma</param>
        /// <param name="idioma">Idioma deseado</param>
        /// <param name="pObtenerIdioma"></param>
        /// <returns>Devuelve una url convertida al idioma deseado</returns>
        public string ConvertirUrlAIdioma(string url, string idioma, bool pObtenerIdioma)
        {
            UtilIdiomas UtilIdiomas = mUtilIdiomasFactory.ObtenerUtilIdiomas(idioma);

            string urlMap = url.Trim('/');
            if (idioma != IdiomaPrincipalDominio && pObtenerIdioma)
            {
                string rutaEjecucionWeb = mConfigService.ObtenerRutaEjecucionWeb();
                if (rutaEjecucionWeb == url)
                {
                    urlMap = rutaEjecucionWeb;
                }

                if (!string.IsNullOrEmpty(rutaEjecucionWeb))
                {
                    urlMap = $"{rutaEjecucionWeb}{idioma}/{urlMap.Replace(rutaEjecucionWeb, "")}";
                }
                else
                {
                    urlMap = $"{idioma}/{urlMap}";
                }               
            }

            while (urlMap.Contains("@#@$"))
            {
                List<string> lista = new List<string>();

                MatchCollection collection = mRegExprRoutes.Matches(urlMap);

                if (collection.Count == 0)
                {
                    break;
                }

                foreach (Match match in collection)
                {
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        string clave = match.Groups[i].Value;
                        if (!lista.Contains(clave))
                        {
                            urlMap = urlMap.Replace(clave, UtilIdiomas.GetTextClaveHTML(clave).Trim('/'));
                            lista.Add(clave);
                        }
                    }
                }
            }

            return urlMap;
        }

        /// <summary>
        /// Procesar una url que solamente contiene un guid
        /// </summary>
        /// <param name="routes">RouteCollection</param>
        /// <param name="listaIdiomas">Lista de idiomas</param>
        private void RegistrarRutaSoloGuid(IRouteBuilder routes, Dictionary<string, string> listaIdiomas)
        {
            RouteValueDictionary constraints = new RouteValueDictionary();
            constraints.Add("RecursoID", @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

            foreach (string idioma in listaIdiomas.Keys)
            {
                string url = "/{RecursoID}";
                string urlMap = ConvertirUrlAIdioma(url, idioma);

                routes.MapRoute(
                    name: "sologuid_" + idioma,
                    template: urlMap,
                    defaults: new { controller = "Redirect", action = "Recurso", redirect301 = "true" },
                    constraints: constraints
                );
            }
        }

        /// <summary>
        /// Procesar una url leida del fichero RouteMap.xml
        /// </summary>
        /// <param name="routes">RouteCollection</param>
        /// <param name="url">Url que vamos a procesar</param>
        /// <param name="urlDestino">Pagina o controlador de destino </param>
        /// <param name="idioma">Idioma en el que vamos a procesar la url</param>
        /// <returns>Devuelve la ruta procesada</returns>
        private void ProcesarURL(IRouteBuilder routes, string url, string urlDestino, string idioma)
        {
            try
            {
                string urlMap = ConvertirUrlAIdioma(url, idioma);

                string[] argsUrlDestino = urlDestino.Split('?');

                RouteValueDictionary parametros = new RouteValueDictionary();
                parametros.Add("lang", idioma);
                if (argsUrlDestino.Length > 1)
                {
                    bool esArgumento = false;
                    foreach (string arg in argsUrlDestino)
                    {
                        if (!esArgumento) { esArgumento = true; continue; }
                        string paramKey = arg.Substring(0, arg.IndexOf('='));
                        string paramValue = arg.Substring(arg.IndexOf('=') + 1);
                        parametros.Add(paramKey, paramValue);
                    }
                }

                //if (urlDestino.Contains(".aspx"))
                //{
                //    routes.MapPageRoute(idioma + "/" + url,
                //    urlMap,
                //    "~/" + argsUrlDestino[0],
                //    true,
                //    parametros);

                //    return routes[routes.Count - 1];
                //}
                //else
                {
                    string[] argsPagina = argsUrlDestino[0].Split('/');
                    string argControler = argsPagina[0];
                    string argAction = "Index";
                    if (argsPagina.Length > 1)
                    {
                        argAction = argsPagina[1];
                    }

                    parametros.Add("controller", argControler);
                    parametros.Add("action", argAction);
                    routes.MapRoute(
                        name: idioma + "/" + url,
                        template: urlMap,
                        defaults: parametros
                    );

                    if (mListaRutasURLName.ContainsKey(urlMap))
                    {
                        mListaRutasURLName[urlMap] = idioma + "/" + url;
                    }
                    else
                    {
                        mListaRutasURLName.Add(urlMap, idioma + "/" + url);
                    }

                    //return routes.Routes.ElementAt(routes.Routes.Count - 1);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }

            //return routes.Routes.ElementAt(routes.Routes.Count - 1);
        }

        public bool EsHijoEcosistemaProyecto(Guid pProyectoIDPadre)
        {
            if (PadreEcosistemaProyectoID.HasValue)
            {
                if (pProyectoIDPadre.Equals(PadreEcosistemaProyectoID.Value))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        private void GuardarLogError(string pError)
        {
            mLoggingService.GuardarLogError(pError, mlogger);
        }
    }
}