using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Routes;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Gnoss.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class ErrorController : ControllerBaseWeb
    {
        //private RouteConfig mRouteConfig;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ErrorController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<ErrorController> logger, ILoggerFactory loggerFactory)
             : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            //mRouteConfig = routeConfig;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region Cargamos el proyecto seleccionado
            ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            Guid? idProyecto = mConfigService.ObtenerProyectoConexion();
            if (idProyecto == null || idProyecto.Equals(Guid.Empty))
            {
                string valor = paramCN.ObtenerParametroAplicacion(ProyectoSeleccionado.UrlPropia(IdiomaUsuario));

                if (!string.IsNullOrEmpty(valor))
                {
                    idProyecto = new Guid(valor);
                }
            }
            if (idProyecto.HasValue && (RequestParams("proyectoID") == null || RequestParams("proyID") == null))
            {
                base.ProyectoSeleccionadoError404 = idProyecto.Value;
            }
            if (HttpContext.Request.Path.ToString().Contains("error?errorCode=404&404;"))
            {
                string[] argsUrl404 = new Uri(HttpContext.Request.Path).OriginalString.ToString().Split(new string[] { "error?errorCode=404&404;" }, StringSplitOptions.RemoveEmptyEntries);

                string UrlOriginal = argsUrl404[1];

                UtilIdiomas = ObtenerIdiomaUrl(UrlOriginal);                

                string nompreCortoProy = "";
                string urlCom=$"/{UtilIdiomas.GetText("URLSEM", "COMUNIDAD")}/";
                if (UrlOriginal.Contains(urlCom))
                {
                    nompreCortoProy = UrlOriginal.Substring(UrlOriginal.IndexOf(urlCom)+urlCom.Length);
                    nompreCortoProy = nompreCortoProy.Trim('/');
                    if (nompreCortoProy.IndexOf('/') > 0)
                    {
                        nompreCortoProy = nompreCortoProy.Substring(0, nompreCortoProy.IndexOf('/'));
                    }
                }
                //TODO Javier -> mover el RouterConfig  un middleware
                //else if (!string.IsNullOrEmpty(mRouteConfig.NombreProyectoSinNombreCorto))
                //{
                //    nompreCortoProy = mRouteConfig.NombreProyectoSinNombreCorto;
                //}

                if (!string.IsNullOrEmpty(nompreCortoProy))
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    base.ProyectoSeleccionadoError404 = proyectoCL.ObtenerProyectoIDPorNombreCorto(nompreCortoProy);
                    UtilIdiomas = null;
                    proyectoCL.Dispose();
                }
            }
            #endregion
            
            base.OnActionExecuting(filterContext);           
        }


        public ActionResult Index()
        {
            string statusCode = RequestParams("errorCode");
            switch (statusCode)
            {
                case "500":
                    Error500ViewModel error500Model = new Error500ViewModel();
                    error500Model.ShowError = mConfigService.ObtenerShow500Error();
                    return View("Error500", error500Model);
                case "404":
                    return  View("Error404", Error404(this, false));
                case "410":
                    return View("Error404", Error404(this, true));
            }
            return View("Error404", Error404(this, false));
        }

        public Error404ViewModel Error404(ControllerBaseWeb pControlador, bool pPermanente)
        {
            string tituloInt = pControlador.UtilIdiomas.GetText("404", "TITULO");

            #region StatusCode
            ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            Guid? idProyecto = mConfigService.ObtenerProyectoConexion();
            if (idProyecto == null || idProyecto.Equals(Guid.Empty))
            {
                string valor = paramCN.ObtenerParametroAplicacion(ProyectoSeleccionado.UrlPropia(IdiomaUsuario));

                if (!string.IsNullOrEmpty(valor))
                {
                    idProyecto = new Guid(valor);
                }
            }
            if ((idProyecto.HasValue && (pControlador.RequestParams("proyectoID") == null)) || pControlador.Request.Headers.ContainsKey("gone"))
            {
                pControlador.Response.StatusCode = 200;
                //pControlador.Response.Status = "200 OK";
            }
            else if (pControlador.Request.Headers.ContainsKey("statuscode"))
            {
                int statusCode = 0;
                string status = string.Empty;

                switch (pControlador.Request.Headers["statuscode"])
                {
                    case "200":
                        statusCode = 200;
                        status = "200 OK";
                        break;
                    case "403":
                        statusCode = 403;
                        status = "403 Forbidden";
                        //tituloPagina = UtilIdiomas.GetText("404", "TITULOPAGINARECURSOPRIVADO");
                        tituloInt = pControlador.UtilIdiomas.GetText("404", "TITULORECURSOPRIVADO");

                        if (pControlador.EsEcosistemaSinMetaProyecto)
                        {
                            // ICM-3 - NO mostrar el título de Página no encontrada cuando sea una página 403.
                            tituloInt = pControlador.UtilIdiomas.GetText("404", "TITULOPAGINARECURSOPRIVADO");
                        }
                        break;
                    default:
                        statusCode = 200;
                        status = "200 OK";
                        break;
                }

                pControlador.Response.StatusCode = statusCode;
                //pControlador.Response.Status = status;
            }
            else
            {
                pControlador.Response.StatusCode = 200;
                //pControlador.Response.Status = "200 OK";
            }
            #endregion

            pControlador.AsignarMetaRobots("NOINDEX, NOFOLLOW");

            Error404ViewModel modeloError404 = new Error404ViewModel();
            modeloError404.Tittle = tituloInt;
            modeloError404.Error = MensajeError(pControlador);
            modeloError404.ResourcesList = new List<ResourceModel>();

            #region Comprobamos si hay que mostrar recursos relacionados
            string[] delimiter = { "/" };
            //string[] trozos = new Uri(pControlador.Request.Path).ToString().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] trozos = pControlador.Request.Path.ToString().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            Guid docID;
            string tempDocID = trozos[trozos.Length - 1];

            if (tempDocID.Contains("?"))
            {
                tempDocID = tempDocID.Substring(0, tempDocID.IndexOf("?"));
            }
            //Si es un ID, se trata de la url de recurso:
            if (Guid.TryParse(tempDocID, out docID))
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                docCN.ObtenerDocumentoPorIDCargarTotal(docID, dataWrapperDocumentacion, true, false, null);

                if (dataWrapperDocumentacion.ListaDocumento.Count > 0)
                {
                    //Si el recurso existió
                    AD.EntityModel.Models.Documentacion.Documento filaDocumento = dataWrapperDocumentacion.ListaDocumento.Where(doc => doc.DocumentoID.Equals(docID)).ToList().FirstOrDefault();
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filaDocumentoWebAgCatTesauro = dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(docID)).ToList();

                    string[] delimiterComma = { "," };
                    string[] tags = filaDocumento.Tags.Split(delimiterComma, StringSplitOptions.RemoveEmptyEntries);

                    List<Guid> cats = new List<Guid>();
                    foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro docWebAgCatTesauro in filaDocumentoWebAgCatTesauro)
                    {
                        cats.Add(docWebAgCatTesauro.CategoriaTesauroID);
                    }
                    modeloError404.ResourcesList = CargarRecursosRelacionados(pControlador, tags, cats.ToArray());
                }
                else
                {
                    //Si el recurso no existe con ese GUID
                    string titulo = trozos[trozos.Length - 2];
                    string[] delimiterGuion = { "-" };
                    string[] trozosTitulo = titulo.Split(delimiterGuion, StringSplitOptions.RemoveEmptyEntries);
                    List<Guid> listaVacia = new List<Guid>();
                    modeloError404.ResourcesList = CargarRecursosRelacionados(pControlador, trozosTitulo, listaVacia.ToArray());
                }
            }
            #endregion

            return modeloError404;
        }

        private ActionResult Error500()
        {
            return new EmptyResult();
        }

        #region Métodos auxiliares

        private UtilIdiomas ObtenerIdiomaUrl(string pRequestUrl)
        {
            string[] delimiter = { "/" };
            string[] trozos = pRequestUrl.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            if (trozos.Length >= 2 && trozos[2].Length == 2)
            {
                IdiomaUsuario = trozos[2];
            }

            return UtilIdiomas;
        }

        /// <summary>
        /// Comprueba la URL a la que intentaba acceder el usuario para personalizar el mensaje que se le da al usuario
        /// </summary>
        private static string MensajeError(ControllerBaseWeb pControlador)
        {           
            string emailSugerencias = "";
            List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = pControlador.ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("CorreoSugerencias")).ToList();
            //if (((DataRow[])pControlador.ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'CorreoSugerencias'")).Length > 0)
            if (busqueda.Count > 0)
            {
                // emailSugerencias = "mailto:" + pControlador.ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'CorreoSugerencias'")[0][1].ToString();
                emailSugerencias = "mailto:" + busqueda.First().Valor;
            }
            string mensaje = pControlador.UtilIdiomas.GetText("404", "ERRORGENERICO", pControlador.UtilIdiomas.GetText("404", "ERRORPORDEFECTO"), emailSugerencias);

            if (pControlador.Request.Headers.ContainsKey("statuscode") && pControlador.Request.Headers["statuscode"] == "403")
            {
                mensaje = pControlador.UtilIdiomas.GetText("404", "RECURSOPRIVADO");
            }

            return mensaje;
        }

        /// <summary>
        /// Monta la lista de las últimas preguntas planteadas
        /// </summary>
        private List<ResourceModel> CargarRecursosRelacionados(ControllerBaseWeb pControlador, string[] pTags, Guid[] pCats)
        {
            if (pControlador.EsEcosistemaSinMetaProyecto && pControlador.Request.Headers.ContainsKey("statuscode") && pControlador.Request.Headers["statuscode"] != "403")
            {
                Stopwatch sw = null;
                try
                {
                    string tags = "";
                    if (pTags.Length > 0)
                    {
                        foreach (string tag in pTags)
                        {
                            tags += "\"" + UtilCadenas.PasarAUtf8(tag.Replace("\"", "'").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ")) + "\",";
                        }
                        tags = tags.Substring(0, tags.Length - 1);
                    }

                    string categorias = "";
                    if (pCats.Length > 0)
                    {
                        foreach (Guid categoriaID in pCats)
                        {
                            categorias += "gnoss:" + categoriaID.ToString().ToUpper() + ",";
                        }
                        categorias = categorias.Substring(0, categorias.Length - 1);
                    }

                    string filtroContextoSelect = "(1* count(?o1) )";
                    string filtroContextoWhere = " {?s ?p ?o1 filter (?p in (sioc_t:Tag)   and ?o1  in (" + tags + ")) } UNION {?s ?p ?o1 filter (?p in (skos:ConceptID)   and ?o1  in (" + categorias + ")) } ";
                    string FiltrosOrden = "null";
                    string filtroContexto = System.Net.WebUtility.UrlEncode("Recursos relacionados|||" + filtroContextoSelect + "|||" + filtroContextoWhere + "|||" + FiltrosOrden);

                    CargadorResultados cargadorResultados = new CargadorResultados();
                    cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();
                    Guid identidadActualID = UsuarioAD.Invitado;
                    bool esUsuarioInvitado = false;

                    sw = LoggingService.IniciarRelojTelemetria();
                    var resultado = cargadorResultados.CargarResultadosContexto(pControlador.ProyectoSeleccionado.Clave, "pagina=1", false, pControlador.UtilIdiomas.LanguageCode, TipoBusqueda.BusquedaAvanzada, 5, pControlador.ProyectoSeleccionado.Clave.ToString(), "", filtroContexto, pControlador.EsBot, false, "", "", "", false, "", identidadActualID, esUsuarioInvitado);
                    mLoggingService.AgregarEntradaDependencia("Llamar al servicio de resultados (pagina de error)", false, "CargarRecursosRelacionados", sw, true);

                    return resultado;
                }
                catch (Exception)
                {
                    mLoggingService.AgregarEntradaDependencia("Error al llamar al servicio de resultados (pagina de error)", false, "CargarRecursosRelacionados", sw, false);
                }
            }
            return null;
        }

        #endregion

    }
}
