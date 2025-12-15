using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Documentacion.AddToGnoss;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.Documentacion.AddToGnossControles;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.FicherosRecursos;
using Es.Riam.Gnoss.Web.MVC.Models.Flujos;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Open.Model;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Es.Riam.Util.AnalisisSintactico;
using Es.Riam.Web.Util;
using Ganss.Xss;
using Gnoss.Web.Controllers;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Universal.Common.Net.Http;
using static Es.Riam.Gnoss.Web.Controles.ControladorBase;
using static Es.Riam.Gnoss.Web.MVC.Models.SelectResourceModel;
using Editor = Es.Riam.Gnoss.Elementos.Documentacion;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    /// <summary>
    /// Controlador para editar recurso.
    /// </summary>
    public class EditarRecursoController : ControllerBaseWeb
    {
        #region Constantes

        public const string COLA_MINIATURA = "ColaMiniatura";
        public const string EXCHANGE = "";
        private IPublishEvents mIPublishEvents;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        public EditarRecursoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IPublishEvents publishEvents, IAvailableServices availableServices, ILogger<EditarRecursoController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mIPublishEvents = publishEvents;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros

        /// <summary>
        /// Separador de cadena del GoogleDrive documentoID
        /// </summary>
        public const string ID_GOOGLE = "##idgoogle##";

        /// <summary>
        /// Devuelve o establece la identidad de la organización si la tiene.
        /// </summary>
        private Identidad mIdentidadOrganizacion;

        /// <summary>
        /// Devuelve la identidad actual ya sea personal o de organizacion.
        /// </summary>
        private Identidad mIdentidadCrearVersion;

        /// <summary>
        /// Devuelve o establece el gestor de documentos
        /// </summary>
        private GestorDocumental mGestorDocumental;

        /// <summary>
        /// Modelo de edición de recursos.
        /// </summary>
        private EditResourceModel mEditRecCont;

        private KeyValuePair<string, string>? mPestanyaRecurso = null;
        /// <summary>
        /// Identificador de documento.
        /// </summary>
        private Guid mDocumentoID;

        /// <summary>
        /// Identificador temporal para el duplicado de un documento.
        /// </summary>
        private Guid mDocumentoDuplicadoID;

        /// <summary>
        /// Documento autoguardado de la wiki actual.
        /// </summary>
        private DocumentoWeb mDocumentoAutoGuardadoWiki;

        /// <summary>
        /// Devuelve o establece la categoría de documentación seleccionada.
        /// </summary>
        private Guid mCatDocumentacionSeleccionada;

        /// <summary>
        /// Texto con el error que se ha producido, NULL si no hay error.
        /// </summary>
        private string mErrorDocumento;

        /// <summary>
        /// Modelo para guardar un recurso.
        /// </summary>
        private DocumentEditionModel mModelSaveRec;


        /// <summary>
        /// Tipo de documento seleccionado.
        /// </summary>
        private TiposDocumentacion mTipoDocumento;

        /// <summary>
        /// Link del recurso (Url, ubicación, hiperenlace).
        /// </summary>
        private string mNombreDocumento;


        /// <summary>
        /// ID de la ontologia a la que se vincula el recurso semántico.
        /// </summary>
        private Guid mOntologiaID;

        /// <summary>
        /// Id del nuevo documneto del que se va a hacer una versión.
        /// </summary>
        private Guid? mDocumentoVersionID;

        /// <summary>
        /// Url de la ontología.
        /// </summary>
        private string mUrlOntologia;

        /// <summary>
        /// Namespace de la ontología.
        /// </summary>
        private string mNamespaceOntologia;

        /// <summary>
        /// Ontología del recurso semántico.
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// Lista con las entidades listas para guardar.
        /// </summary>
        private List<ElementoOntologia> mEntidadesGuardar;

        /// <summary>
        /// Controlador de SEMCMS.
        /// </summary>
        private SemCmsController mSemController;

        /// <summary>
        /// Indica si se ha encargado la captura del recurso al servicio de procesado de tareas.
        /// </summary>
        private bool mCapturaSercicioEncargada;

        /// <summary>
        /// Indica si se está editando un recurso de la carga masiva.
        /// </summary>
        private bool mEditandoRecursoCargaMasiva;

        /// <summary>
        /// Datos del recurso editado de la carga masiva.
        /// </summary>
        private string[] mDatosRecursoEditadoCargaMasiva;

        /// <summary>
        /// Rdf del recurso editado de la carga masiva.
        /// </summary>
        private string mRdfRecursoEditadoCargaMasiva;

        /// <summary>
        /// Propiedades de tipo texto configuradas para la ontología actual.
        /// </summary>
        private Dictionary<string, string> mPropiedadesTextoOntologia;

        /// <summary>
        /// Cookie temporal que almacena algunos parámetros que se pasan a anyadir a gnoss.
        /// </summary>
        private Dictionary<string, string> mCookieAnyadirGnoss;

        /// <summary>
        /// Obtiene o establece el título del recurso para el Add to Gnoss
        /// </summary>
        private string mTituloAddTo;

        /// <summary>
        /// Obtiene o establece la descripción del recurso para el Add to Gnoss
        /// </summary>
        private string mDescripcionAddTo;

        /// <summary>
        /// Obtiene o establece los tags del recurso para el Add to Gnoss
        /// </summary>
        private string mTagsAddTo;

        /// <summary>
        /// Indica que el add to GNOSS actual es un envío desde metaGNOSS.
        /// </summary>
        private bool mEnvioMetaGnossAddToGnoss;

        /// <summary>
        /// Indica que es un archivo local Add To Gnoss.
        /// </summary>
        private bool mEsArchivoLocalAddToGnoss;

        /// <summary>
        /// Lista con los gestores de Add To Gnoss.
        /// </summary>
        private Dictionary<Guid, GestorAddToGnoss> mGestoresAddToGnoss;

        /// <summary>
        /// Bases de recursos y sus categorías seleccionadas para el Add To Gnoss.
        /// </summary>
        private Dictionary<Guid, List<Guid>> mBRsCatsAddToGnoss;

        /// <summary>
        /// Documentos extra para guardar.
        /// </summary>
        private List<Documento> mDocumentosExtraGuardar;

        /// <summary>
        /// IDs de entidades externas editables y su ID de documento que el corresponde.
        /// </summary>
        private Dictionary<string, EntidadExtEditableDoc> mEntidadesExtEditablesDocID;

        /// <summary>
        /// Indica si el recurso que se está guardando ha cambiado de borrador a público.
        /// </summary>
        private bool mCambioDeBorradoAPublicado;

        private List<TripleWrapper> mListaTriplesSemanticos;

        /// <summary>
        /// Cadena para meter el parámetro OtrosArgumentos en el BASE.
        /// </summary>
        private string mOtrosArgumentosBase;

        /// <summary>
        /// Indica si se está creando una versión del documento.
        /// </summary>
        private bool mCreandoVersion;

        /// <summary>
        /// Indica si ya se han insertado los triples del recurso en el grafo de búsqueda
        /// </summary>
        private bool mInsertadoEnGrafoBusqueda;



        #endregion

        #region Métodos
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EsPaginaEdicion = true;

            mCookieAnyadirGnoss = null;
            try
            {
                mCookieAnyadirGnoss = UtilCookies.FromLegacyCookieString(Request.Cookies["anyadirGnoss"], mEntityContext);
            }
            catch
            {
                Response.Cookies.Delete("anyadirGnoss");
            }
            if (mCookieAnyadirGnoss == null && (RequestParams("titl") != null || RequestParams("descp") != null))
            {
                //crea la cookie con los parámetros de la petición
                mCookieAnyadirGnoss = new Dictionary<string, string>();
                mCookieAnyadirGnoss["titl"] = System.Net.WebUtility.UrlEncode(TituloAddTo);
                mCookieAnyadirGnoss["descp"] = System.Net.WebUtility.UrlEncode(DescripcionAddTo);
                mCookieAnyadirGnoss["tags"] = System.Net.WebUtility.UrlEncode(TagsAddTo);
                mCookieAnyadirGnoss["verAddTo"] = RequestParams("verAddTo");
                Response.Cookies.Append("anyadirGnoss", UtilCookies.ToLegacyCookieString(mCookieAnyadirGnoss, mEntityContext), new CookieOptions { Expires = DateTime.Now.AddMinutes(20) });
            }

            ViewBag.EsPaginaEdicionRecurso = true;

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Método inicial.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index(bool pDevolverVista = true)
        {
            mCookieAnyadirGnoss = null;
            try
            {
                mCookieAnyadirGnoss = UtilCookies.FromLegacyCookieString(Request.Cookies["anyadirGnoss"], mEntityContext);
            }
            catch
            {
                Response.Cookies.Delete("anyadirGnoss");
            }

            mLoggingService.AgregarEntrada("TiemposMVC_IndexEditarRecurso_Inicio");

            ActionResult redireccion = null;

            TipoPagina = TiposPagina.BaseRecursos;

            mEditRecCont = new EditResourceModel();
            mEditRecCont.TabName = UtilIdiomas.GetText("COMMON", "RECURSOS");
            mEditRecCont.UrlPestanya = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, false);

            if (RequestParams("subirRecurso") != null)
            {
                redireccion = Index_SubirRecurso();
            }
            else if (RequestParams("subirRecursoPart2") != null)
            {
                redireccion = Index_SubirRecursoPart2();
            }
            else if (RequestParams("editarRecurso") != null)
            {
                if (EstaCreandoVersionEnlace())
                {
                    redireccion = FuncionalidadSharepoint();
                    if (redireccion != null)
                    {
                        return redireccion;
                    }
                }

                redireccion = Index_ModificarRecurso();

                if (EsEdicionMultiple && (Documento.TipoDocumentacion.Equals(TiposDocumentacion.FicheroServidor) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Nota)))
                {
                    mEditRecCont.ModifyResourceModel.EditAttachedAvailable = true;
                    mEditRecCont.ModifyResourceModel.EditFileAvailable = true;
                    mEditRecCont.ModifyResourceModel.EdicionUnificada = true;
                }
            }
            else if (RequestParams("subidaUnificada") != null)
            {
                redireccion = Index_SubirRecursoPart2();
                mEditRecCont.ModifyResourceModel.EditAttachedAvailable = true;
                mEditRecCont.ModifyResourceModel.EditFileAvailable = true;
                mEditRecCont.ModifyResourceModel.SubidaUnificada = true;
            }
            else if (CreandoFormSem || EditandoFormSem || CargaMasivaFormSem)
            {
                ListaNombresDocumento = null;
                redireccion = Index_ModificarRecursoSemantico();
            }
            else if (AñadiendoAGnoss)
            {
                redireccion = Index_AnyadirGnoss();
            }

            if (redireccion != null)
            {
                return redireccion;
            }

            if (pDevolverVista)
            {
                return View(mEditRecCont);
            }
            return null;
        }

        #region Subida Recurso Entero

        /// <summary>
        /// Método inicial.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index_SubidaRecursoEntero()
        {
            mEditRecCont.TypePage = EditResourceModel.TypePageEditResource.CreateResourceComplete;

            CreateResourceModel subirRecModel = new CreateResourceModel();

            mLoggingService.AgregarEntrada("TiemposMVC_IndexEditarRecurso_SubidaRecursoEntero_Inicio");

            mEditRecCont.CreateResourceModel = subirRecModel;

            #region !IsPostBack

            string urlRedirec = CargarInicial_SubirRecurso();

            if (urlRedirec != null)
            {
                return Redirect(urlRedirec);
            }

            TituloPagina = UtilIdiomas.GetText("PERFILBASESUBIR", "TITULOPAGINA");

            #region Ontologia

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                Guid proyectoOntologiasID = mControladorBase.UsuarioActual.ProyectoID;

                if (ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
                {
                    proyectoOntologiasID = new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
                }

                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                DataWrapperDocumentacion docOntoDW = docCL.ObtenerOntologiasProyecto(proyectoOntologiasID, true);

                GestorDocumental.DataWrapperDocumentacion.Merge(docOntoDW);

                List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumento.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList();

                SortedDictionary<string, KeyValuePair<string, string>> nombreUrlsOntologia = new SortedDictionary<string, KeyValuePair<string, string>>();
                mEditRecCont.CreateResourceModel.OntologyNameUrls = nombreUrlsOntologia;

                //Simple
                foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in filasDoc.Where(item => ComprobarPermisoEnOntologiaDeProyectoEIdentidad(item.DocumentoID)))
                {
                    Dictionary<string, string> listaPropiedades = UtilCadenas.ObtenerPropiedadesDeTexto(filaDoc.NombreElementoVinculado);
                    bool documentoVirtual = false;
                    if (listaPropiedades.ContainsKey(PropiedadesOntologia.urlservicio.ToString()))
                    {
                        documentoVirtual = true;
                    }

                    string tituloOntologia = UtilCadenas.ObtenerTextoDeIdioma(filaDoc.Titulo, UtilIdiomas.LanguageCode, "es");

                    if (listaPropiedades.ContainsKey(PropiedadesOntologia.cargasmultiples.ToString()) && listaPropiedades[PropiedadesOntologia.cargasmultiples.ToString()] == "true")
                    {
                        nombreUrlsOntologia.Add(tituloOntologia, new KeyValuePair<string, string>(mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumento(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), (IdentidadOrganizacion != null), documentoVirtual), mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumentoMultiple(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), (IdentidadOrganizacion != null))));
                    }
                    else
                    {
                        nombreUrlsOntologia.Add(tituloOntologia, new KeyValuePair<string, string>(mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumento(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), IdentidadOrganizacion != null, documentoVirtual), null));
                    }
                }
            }

            GestorDocumental.CargarDocumentos(false);

            #endregion

            #endregion

            #region Comprobar rol usuario para permitir ciertas subidas

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && ProyectoSeleccionado.FilaProyecto.Estado != (short)EstadoProyecto.Abierto && ProyectoSeleccionado.FilaProyecto.Estado != (short)EstadoProyecto.Definicion)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                CargarListaRecursos_SubirRecurso(true, ListaPermisosDocumentos, subirRecModel);
            }
            else
            {
                CargarListaRecursos_SubirRecurso(false, new List<TiposDocumentacion>(), subirRecModel);
            }

            #endregion

            return null;
        }

        #endregion


        #region Subir Recurso

        /// <summary>
        /// Método inicial.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index_SubirRecurso()
        {
            mLoggingService.AgregarEntrada("TiemposMVC_IndexEditarRecurso_SubirRecurso_Inicio");

            mEditRecCont.TypePage = EditResourceModel.TypePageEditResource.CreateResource;

            CreateResourceModel subirRecModel = new CreateResourceModel();
            mEditRecCont.CreateResourceModel = subirRecModel;

            #region !IsPostBack

            string urlRedirec = CargarInicial_SubirRecurso();

            if (urlRedirec != null)
            {
                return Redirect(urlRedirec);
            }

            TituloPagina = UtilIdiomas.GetText("PERFILBASESUBIR", "TITULOPAGINA");

            #region Ontologia

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                Guid proyectoOntologiasID = mControladorBase.UsuarioActual.ProyectoID;

                if (ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
                {
                    proyectoOntologiasID = new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
                }

                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                DataWrapperDocumentacion docOntoDW = docCL.ObtenerOntologiasProyecto(proyectoOntologiasID, true);

                GestorDocumental.DataWrapperDocumentacion.Merge(docOntoDW);

                List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumento.Where(documento => documento.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList();

                SortedDictionary<string, KeyValuePair<string, string>> nombreUrlsOntologia = new SortedDictionary<string, KeyValuePair<string, string>>();
                mEditRecCont.CreateResourceModel.OntologyNameUrls = nombreUrlsOntologia;

                //Simple
                foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in filasDoc.Where(item => ComprobarPermisoEnOntologiaDeProyectoEIdentidad(item.DocumentoID)))
                {
                    Dictionary<string, string> listaPropiedades = UtilCadenas.ObtenerPropiedadesDeTexto(filaDoc.NombreElementoVinculado);
                    bool documentoVirtual = false;
                    if (listaPropiedades.ContainsKey(PropiedadesOntologia.urlservicio.ToString()))
                    {
                        documentoVirtual = true;
                    }

                    string tituloOntologia = UtilCadenas.ObtenerTextoDeIdioma(filaDoc.Titulo, UtilIdiomas.LanguageCode, "es");

                    if (listaPropiedades.ContainsKey(PropiedadesOntologia.cargasmultiples.ToString()) && listaPropiedades[PropiedadesOntologia.cargasmultiples.ToString()] == "true")
                    {
                        nombreUrlsOntologia.Add(tituloOntologia, new KeyValuePair<string, string>(mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumento(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), (IdentidadOrganizacion != null), documentoVirtual), mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumentoMultiple(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), (IdentidadOrganizacion != null))));
                    }
                    else
                    {
                        nombreUrlsOntologia.Add(tituloOntologia, new KeyValuePair<string, string>(mControladorBase.UrlsSemanticas.GetURLBaseRecursosCrearDocumento(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Guid.NewGuid(), filaDoc.DocumentoID.ToString(), (IdentidadOrganizacion != null), documentoVirtual), null));
                    }
                }
            }

            GestorDocumental.CargarDocumentos(false);

            #endregion

            #endregion

            #region Comprobar rol usuario para permitir ciertas subidas

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && ProyectoSeleccionado.FilaProyecto.Estado != (short)EstadoProyecto.Abierto && ProyectoSeleccionado.FilaProyecto.Estado != (short)EstadoProyecto.Definicion)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                CargarListaRecursos_SubirRecurso(true, ListaPermisosDocumentos, subirRecModel);
            }
            else
            {
                CargarListaRecursos_SubirRecurso(false, new List<TiposDocumentacion>(), subirRecModel);
            }

            #endregion

            return null;
        }

        [HttpGet, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult ComprobarTokenSharepointAntesGuardar(string pLink, int pTypeResourceSelected, bool pSkipRepeat, string pUrlPaginaSubir)
        {
            try
            {
                if (!string.IsNullOrEmpty(pLink))
                {
                    Environment.SetEnvironmentVariable("Link", pLink);
                }

                if (!string.IsNullOrEmpty(pUrlPaginaSubir))
                {
                    Environment.SetEnvironmentVariable("UrlPaginaSubir", pUrlPaginaSubir);
                    Environment.SetEnvironmentVariable("SkipRepeat", $"{pSkipRepeat}");
                }
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (UtilCadenas.EsEnlaceSharepoint(pLink, oneDrivePermitido))
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    string sharepointConfigurado = paramCN.ObtenerParametroAplicacion("SharepointClientID");
                    if (!string.IsNullOrEmpty(sharepointConfigurado))
                    {
                        ActionResult redireccion = FuncionalidadSharepointComprobarTokenAntesSubirRecurso(pLink);
                        if (redireccion != null)
                        {
                            return redireccion;
                        }
                    }
                }

                string link = Environment.GetEnvironmentVariable("Link");
                bool skipRepeat = bool.Parse(Environment.GetEnvironmentVariable("SkipRepeat"));
                Environment.SetEnvironmentVariable("Link", null);
                Environment.SetEnvironmentVariable("SkipRepeat", null);
                Environment.SetEnvironmentVariable("UrlPaginaSubir", null);
                SelectResourceModel model = new SelectResourceModel();
                model.Link = link;
                model.SkipRepeat = skipRepeat;
                model.TypeResourceSelected = TypeResource.Link;

                return SubirRecurso(model);
            }
            catch
            {
                return GnossResultERROR("Error al ir a la página de crear recurso");
            }

        }

        /// <summary>
        /// Acción de seleccionar un tipo de recurso para subir.
        /// </summary>
        /// <param name="pModel">Modelo de selección de recurso</param>
        /// <returns>Resultado de la acción</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult SubirRecurso(SelectResourceModel pModel)
        {
            try
            {
                //Llamo a Index para que se genere el modelo y luego comprobar si hay permisos para hacer la acción:
                Index(false);

                if (pModel.TypeResourceSelected == TypeResource.Note)
                {
                    return lbSiguienteNota_Click_SubirRecurso();
                }
                else if (pModel.TypeResourceSelected == TypeResource.Link)
                {
                    return lbSiguienteURL_Click_SubirRecurso(pModel);
                }
                else if (pModel.TypeResourceSelected == TypeResource.DocumentReference)
                {
                    return lbSiguienteReferencia_Click_SubirRecurso(pModel);
                }
                else if (pModel.TypeResourceSelected == TypeResource.Wiki)
                {
                    return lbSiguienteWiki_Click_SubirRecurso(pModel);
                }
                else if (pModel.TypeResourceSelected == TypeResource.File)
                {
                    return lbSiguienteArchivo_Click_SubirRecurso(pModel);
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion SubirRecurso");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
                throw;
            }
        }

        private ActionResult ComprobarTienePermisosAccionRecursoSemantico(TipoPermisoRecursosSemanticos pTipoPermiso)
        {
            if (!ComprobarPermisosAccionRecursoSemantico(pTipoPermiso))
            {
                return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosInicio(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, IdentidadOrganizacion != null));
            }

            return null;
        }

        private bool ComprobarPermisosAccionRecursoSemantico(TipoPermisoRecursosSemanticos pTipoPermiso)
        {
            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            return utilPermisos.IdentidadTienePermisoRecursoSemantico(mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, pTipoPermiso, mOntologiaID);
        }

        private ActionResult ComprobarTienePermisosAccionRecurso(PermisoRecursos pTipoPermiso)
        {
            if (!ComprobarPermisosAccionRecurso(pTipoPermiso))
            {
                return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosInicio(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, IdentidadOrganizacion != null));
            }

            return null;
        }

        private bool ComprobarPermisosAccionRecurso(PermisoRecursos pTipoPermiso)
        {
            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            bool tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)pTipoPermiso, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);

            return tienePermiso;
        }

        #region Nota

        /// <summary>
        /// Click en nota.
        /// </summary>
        /// <param name="pModel">Modelo de selección de recurso</param>
        /// <returns>Acción resultado</returns>
        private ActionResult lbSiguienteNota_Click_SubirRecurso()
        {
            if (!mEditRecCont.CreateResourceModel.NoteAvailable)
            {
                return new EmptyResult();
            }

            ActionResult result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearNota);
            if (result != null)
            {
                return result;
            }

            Guid documentoID = Guid.NewGuid();
            Session.SetString("EnlaceDocumentoAgregar", "Nota");

            return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, ((short)TiposDocumentacion.Nota).ToString(), null, (IdentidadOrganizacion != null)));
        }

        #endregion

        #region Link

        /// <summary>
        /// Click en link.
        /// </summary>
        /// <param name="pModel">Modelo de selección de recurso</param>
        /// <returns>Acción resultado</returns>
        private ActionResult lbSiguienteURL_Click_SubirRecurso(SelectResourceModel pModel)
        {
            ActionResult result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearRecursoTipoEnlace);
            if (result != null)
            {
                return result;
            }
            ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
            if (!UtilCadenas.EsEnlaceSharepoint(pModel.Link, oneDrivePermitido) && !mEditRecCont.CreateResourceModel.LinkAvailable)
            {
                return new EmptyResult();
            }

            string url = pModel.Link;
            if (!string.IsNullOrEmpty(url))
            {
                Guid documentoID = Guid.NewGuid();

                if (!pModel.SkipRepeat)
                {
                    ActionResult htmlPanelRepe = ComprobarRepeticionEnlaceRecurso("", url, TiposDocumentacion.Hipervinculo, null);

                    if (htmlPanelRepe != null)
                    {
                        return htmlPanelRepe;
                    }
                }
                Session.SetString("EnlaceDocumentoAgregar", url);

                string urlRedireccion = mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, ((short)TiposDocumentacion.Hipervinculo).ToString(), null, (IdentidadOrganizacion != null));

                return GnossResultUrl(urlRedireccion);
            }
            else
            {
                return GnossResultERROR();
            }
        }

        #endregion

        #region Referencia Documento Físico

        /// <summary>
        /// Click en referencia a documento físico.
        /// </summary>
        /// <param name="pModel">Modelo de selección de recurso</param>
        /// <returns>Acción resultado</returns>
        protected ActionResult lbSiguienteReferencia_Click_SubirRecurso(SelectResourceModel pModel)
        {
            ActionResult result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearRecursoTipoReferenciaADocumentoFisico);
            if (result != null)
            {
                return result;
            }

            if (!mEditRecCont.CreateResourceModel.DocumentReferenceAvailable)
            {
                return new EmptyResult();
            }

            string url = pModel.Link;

            if (url != "")
            {
                if (!pModel.SkipRepeat)
                {
                    ActionResult htmlPanelRepe = ComprobarRepeticionEnlaceRecurso("", url, TiposDocumentacion.ReferenciaADoc, null);

                    if (htmlPanelRepe != null)
                    {
                        return htmlPanelRepe;
                    }
                }

                Guid documentoID = Guid.NewGuid();
                Session.SetString("EnlaceDocumentoAgregar", url);
                string urlRedireccion = mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, ((short)TiposDocumentacion.ReferenciaADoc).ToString(), null, (IdentidadOrganizacion != null));

                return GnossResultUrl(urlRedireccion);
            }
            else
            {
                return GnossResultERROR();
            }
        }

        #endregion

        #region Artículos Wiki

        /// <summary>
        /// Click en artículo wiki.
        /// </summary>
        /// <param name="pModel">Modelo de selección de recurso</param>
        /// <returns>Acción resultado</returns>
        protected ActionResult lbSiguienteWiki_Click_SubirRecurso(SelectResourceModel pModel)
        {
            if (!mEditRecCont.CreateResourceModel.WikiAvailable)
            {
                return new EmptyResult();
            }

            string artWiki = pModel.Link;
            if (artWiki == "")
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "NOMBREVACIO"));
            }
            else
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                Guid documentoID = docCN.ObtenerDocumentoWikiPorNombreyProyecto(artWiki, mControladorBase.UsuarioActual.ProyectoID);

                if (documentoID.Equals(Guid.Empty))
                {
                    Session.SetString("EnlaceDocumentoAgregar", artWiki);
                    docCN.Dispose();
                    return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Guid.NewGuid(), ((short)TiposDocumentacion.Wiki).ToString(), null, (IdentidadOrganizacion != null)));
                }
                else
                {
                    DataWrapperDocumentacion dataWrapperDocumentacion = docCN.ObtenerDocumentoPorID(documentoID);
                    GestorDocumental GestDoc = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
                    docCN.Dispose();

                    string urlDoc = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, NombreProy, UrlPerfil, GestDoc.ListaDocumentos[documentoID], (IdentidadOrganizacion != null));

                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "DOCENCONTRADO", artWiki, urlDoc));
                }

            }
        }

        #endregion

        #region Archivo

        /// <summary>
        /// Click en archivo.
        /// </summary>
        /// <param name="pModel">Modelo de selección de recurso</param>
        /// <returns>Acción resultado</returns>
        protected ActionResult lbSiguienteArchivo_Click_SubirRecurso(SelectResourceModel pModel)
        {
            if (!mEditRecCont.CreateResourceModel.FileAvailable)
            {
                return new EmptyResult();
            }

            ActionResult result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearRecursoTipoAdjunto);
            if (result != null)
            {
                return result;
            }

            if (pModel.SkipRepeat)
            {
                string[] extraArchivo = pModel.ExtraFile.Split('|');

                Guid documentoID = new Guid(extraArchivo[0]);
                string tipoDoc = extraArchivo[1];
                string tamanoArch = extraArchivo[2];
                string enlace = extraArchivo[3];

                Session.SetString("EnlaceDocumentoAgregar", enlace);

                return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, tipoDoc, tamanoArch, (IdentidadOrganizacion != null)));

            }
            else if (pModel.File != null && !string.IsNullOrEmpty(pModel.FileName))
            {
                return AgregarArchivo_SubirRecurso(pModel);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Agrega un Archivo a Gnoss
        /// </summary>
        /// <param name="pModel">Modelo de selección de recurso</param>
        /// <returns>Resultado de la acción</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        protected ActionResult AgregarArchivo_SubirRecurso(SelectResourceModel pModel)
        {
            if (pModel.File != null)
            {
                Stopwatch sw = null;
                try
                {
                    string fileName = pModel.FileName;
                    TipoArchivo tipoArchivo = UtilArchivos.ObtenerTipoArchivo(fileName);
                    TiposDocumentacion tipoDocumentoGnoss = TiposDocumentacion.FicheroServidor;

                    byte[] buffer1;

                    using (BinaryReader reader1 = new BinaryReader(pModel.File.OpenReadStream()))
                    {
                        buffer1 = reader1.ReadBytes((int)pModel.File.Length);
                        reader1.Close();
                    }

                    int resultado = 0;
                    double tamanoBytes = buffer1.Length;
                    double tamanoArchivoMB = tamanoBytes / 1024 / 1024;

                    FileInfo archivoInfo = new FileInfo(fileName);
                    string extensionArchivo = Path.GetExtension(archivoInfo.Name).ToLower();
                    Guid documentoID = Guid.NewGuid();

                    if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto && GestorDocumental.EspacioActualBaseRecursos + tamanoArchivoMB > GestorDocumental.EspacioMaximoBaseRecursos)
                    {
                        resultado = 4;
                    }
                    else
                    {
                        if (tipoArchivo == TipoArchivo.Audio || tipoArchivo == TipoArchivo.Video)
                        {
                            if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto || ListaPermisosDocumentos.Contains(TiposDocumentacion.Video))
                            {
                                ServicioVideos servicioVideos = new ServicioVideos(mConfigService, mLoggingService, mLoggerFactory.CreateLogger<ServicioVideos>(), mLoggerFactory);
                                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                                {
                                    if (IdentidadOrganizacion == null)
                                    {
                                        resultado = servicioVideos.AgregarVideoPersonal(buffer1, extensionArchivo, documentoID, mControladorBase.UsuarioActual.PersonaID);
                                    }
                                    else
                                    {
                                        resultado = servicioVideos.AgregarVideoOrganizacion(buffer1, extensionArchivo, documentoID, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                                    }
                                }
                                else
                                {
                                    resultado = servicioVideos.AgregarVideo(buffer1, extensionArchivo, documentoID);
                                }
                            }
                            else
                            {
                                resultado = 100;
                            }
                            tipoDocumentoGnoss = TiposDocumentacion.Video;
                        }
                        else if (tipoArchivo == TipoArchivo.Imagen)
                        {
                            string extensionArchivoReducido = extensionArchivo;
                            if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto || ListaPermisosDocumentos.Contains(TiposDocumentacion.Imagen))
                            {
                                Image imagen = Image.Load(new MemoryStream(buffer1));

                                int anchoMaximo = 582;

                                float ancho = imagen.Width;
                                float alto = imagen.Height;
                                if (ancho > anchoMaximo)
                                {
                                    ancho = anchoMaximo;
                                    alto = (imagen.Height * ancho) / imagen.Width;
                                }

                                Image imagenPeque = UtilImages.AjustarImagen(imagen, ancho, alto);

                                byte[] bufferReducido = UtilImages.ImageToBytePng(imagenPeque);

                                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                                servicioImagenes.Url = UrlIntragnossServicios;
                                bool correcto = false;

                                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                                {
                                    if (IdentidadOrganizacion == null)
                                    {
                                        correcto = servicioImagenes.AgregarImagenDocumentoPersonal(bufferReducido, documentoID.ToString(), extensionArchivoReducido, mControladorBase.UsuarioActual.PersonaID);
                                    }
                                    else
                                    {
                                        correcto = servicioImagenes.AgregarImagenDocumentoOrganizacion(bufferReducido, documentoID.ToString(), extensionArchivoReducido, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                                    }
                                }
                                else
                                {
                                    correcto = servicioImagenes.AgregarImagenADirectorio(bufferReducido, Path.Combine(UtilArchivos.ContentImagenesDocumentos, UtilArchivos.DirectorioDocumento(documentoID)), documentoID.ToString(), extensionArchivoReducido);
                                }

                                if (correcto)
                                {
                                    resultado = 1;

                                    GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                                    gd.Url = UrlServicioWebDocumentacion;

                                    sw = LoggingService.IniciarRelojTelemetria();
                                    if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                                    {
                                        if (IdentidadOrganizacion == null)
                                        {
                                            gd.AdjuntarDocumentoABaseRecursosUsuario(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, documentoID, extensionArchivo);
                                        }
                                        else
                                        {
                                            gd.AdjuntarDocumentoABaseRecursosOrganizacion(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), documentoID, extensionArchivo);
                                        }
                                    }
                                    else
                                    {
                                        gd.AdjuntarDocumento(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.ProyectoID, documentoID, extensionArchivo);
                                    }
                                    mLoggingService.AgregarEntradaDependencia("Adjuntar archivo al gestor documental", false, "AgregarArchivo_SubirRecurso", sw, true);
                                }
                            }
                            else
                            {
                                resultado = 100;
                            }
                            tipoDocumentoGnoss = TiposDocumentacion.Imagen;
                        }
                        else if (tipoArchivo == TipoArchivo.Otros)
                        {
                            if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto || ListaPermisosDocumentos.Contains(TiposDocumentacion.FicheroServidor))
                            {
                                string idAuxGestorDocumental = "";

                                //Subimos el fichero al servidor
                                GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                                gd.Url = UrlServicioWebDocumentacion;
                                sw = LoggingService.IniciarRelojTelemetria();

                                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                                {
                                    if (IdentidadOrganizacion == null)
                                    {
                                        idAuxGestorDocumental = gd.AdjuntarDocumentoABaseRecursosUsuario(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, documentoID, extensionArchivo);
                                    }
                                    else
                                    {
                                        idAuxGestorDocumental = gd.AdjuntarDocumentoABaseRecursosOrganizacion(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), documentoID, extensionArchivo);
                                    }
                                }
                                else
                                {
                                    TipoEntidadVinculadaDocumento tipoEntidaVinDoc = TipoEntidadVinculadaDocumento.Web;

                                    string tipoEntidadTexto = ControladorDocumentacion.ObtenerTipoEntidadAdjuntarDocumento(tipoEntidaVinDoc);
                                    idAuxGestorDocumental = gd.AdjuntarDocumento(buffer1, tipoEntidadTexto, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.ProyectoID, documentoID, extensionArchivo);
                                }

                                if (!idAuxGestorDocumental.ToUpper().Equals("ERROR"))
                                {
                                    resultado = 1;
                                }

                                mLoggingService.AgregarEntradaDependencia("Adjuntar archivo al gestor documental", false, "AgregarArchivo_SubirRecurso", sw, true);
                            }
                            else
                            {
                                resultado = 100;
                            }
                            tipoDocumentoGnoss = TiposDocumentacion.FicheroServidor;
                        }
                    }

                    switch (resultado)
                    {
                        case 0:
                            {
                                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
                            }
                        case 1:
                            {
                                ActionResult panRep = ComprobarRepeticionEnlaceRecurso("", archivoInfo.Name, TiposDocumentacion.FicheroServidor, documentoID.ToString() + "|" + ((short)tipoDocumentoGnoss).ToString() + "|" + tamanoArchivoMB.ToString() + "|" + archivoInfo.Name);

                                if (panRep != null)
                                {
                                    return panRep;
                                }

                                Session.SetString("EnlaceDocumentoAgregar", archivoInfo.Name);

                                return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, ((short)tipoDocumentoGnoss).ToString(), tamanoArchivoMB.ToString(), (IdentidadOrganizacion != null)));

                            }
                        case 2:
                            {
                                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSEGURIDAD"));
                            }
                        case 3:
                            {
                                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSIZE"));
                            }
                        case 4:
                            {
                                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSIZEMAXIMO"));
                            }
                        case 100:
                            {
                                if (tipoDocumentoGnoss == TiposDocumentacion.FicheroServidor)
                                {
                                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCNOPERMITIDOFICHSER"));
                                }
                                else if (tipoDocumentoGnoss == TiposDocumentacion.Imagen)
                                {
                                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCNOPERMITIDOIMAGEN"));
                                }
                                else if (tipoDocumentoGnoss == TiposDocumentacion.Video)
                                {
                                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCNOPERMITIDOMULTIMEDIA"));
                                }
                                break;
                            }
                    }

                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    //Se han producido errores al guardar el documento en el servidor
                    mLoggingService.AgregarEntradaDependencia("Error al adjuntar archivo al gestor documental", false, "AgregarArchivo_SubirRecurso", sw, false);
                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
                }
            }

            return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
        }

        #endregion

        /// <summary>
        /// Realiza la carga inicial de los gestores para que funcione la pantalla.
        /// </summary>
        /// <returns>NULL si va bien o URL a la que hay que redirigir si el usuario no esta donde debe</returns>
        private string CargarInicial_SubirRecurso()
        {
            if (!string.IsNullOrEmpty(RequestParams("organizacion")) && !mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID")) && !mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID")))
            {
                return mControladorBase.UrlsSemanticas.GetURLBaseRecursosInicio(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, IdentidadOrganizacion != null);
            }

            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return BaseURLIdioma;
            }

            //Carga la base de recursos del usuario:
            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            GestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

            if (IdentidadOrganizacion == null)
            {
                docCL.ObtenerBaseRecursosProyecto(GestorDocumental.DataWrapperDocumentacion, mControladorBase.UsuarioActual.ProyectoID, mControladorBase.UsuarioActual.OrganizacionID, UsuarioActual.UsuarioID);
            }
            else
            {
                docCL.ObtenerBaseRecursosOrganizacion(GestorDocumental.DataWrapperDocumentacion, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), ProyectoAD.MetaProyecto);
            }

            docCL.Dispose();

            return null;
        }

        /// <summary>
        /// Carga los combos con datos
        /// </summary>
        /// <param name="pExaminarPermisos">TRUE si debe examinar los permisos, FALSE en caso contrario</param>
        /// <param name="pListaTiposDocPermitidos">Lista de tipos de documentación permitidos</param>
        /// <param name="subirRecModel">Modelo de subir recurso</param>
        private void CargarListaRecursos_SubirRecurso(bool pExaminarPermisos, List<TiposDocumentacion> pListaTiposDocPermitidos, CreateResourceModel pSubirRecModel)
        {
            if (!pExaminarPermisos || (pListaTiposDocPermitidos.Contains(TiposDocumentacion.FicheroServidor) || pListaTiposDocPermitidos.Contains(TiposDocumentacion.Imagen) || pListaTiposDocPermitidos.Contains(TiposDocumentacion.Video)))
            {
                pSubirRecModel.FileAvailable = true;
            }

            if (!pExaminarPermisos || pListaTiposDocPermitidos.Contains(TiposDocumentacion.ReferenciaADoc))
            {
                pSubirRecModel.DocumentReferenceAvailable = true;
            }

            if (!pExaminarPermisos || pListaTiposDocPermitidos.Contains(TiposDocumentacion.Hipervinculo))
            {
                pSubirRecModel.LinkAvailable = true;
            }

            if (pListaTiposDocPermitidos.Contains(TiposDocumentacion.VideoBrightcove))
            {
                pSubirRecModel.BrightcoveVideoAvailable = true;

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();

                Guid tokenID = Guid.NewGuid();
                Guid documentoID = Guid.NewGuid();

                AD.EntityModel.Models.Documentacion.DocumentoTokenBrightcove docTokenBrightcove = new AD.EntityModel.Models.Documentacion.DocumentoTokenBrightcove();
                docTokenBrightcove.TokenID = tokenID;
                docTokenBrightcove.ProyectoID = ProyectoSeleccionado.Clave;
                docTokenBrightcove.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                docTokenBrightcove.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                docTokenBrightcove.FechaCreacion = DateTime.Now;
                docTokenBrightcove.Estado = (short)EstadoSubidaVideo.Espera;

                docDW.ListaDocumentoTokenBrightcove.Add(docTokenBrightcove);
                mEntityContext.DocumentoTokenBrightcove.Add(docTokenBrightcove);
                docCN.ActualizarDocumentacion();
                docCN.Dispose();

                #region Iframe & JS Brightcove

                #region Iframe
                string errorSubida = UtilIdiomas.GetText("PERFILBASESUBIR", "ERROR_DVIDEO");
                string errorTipo = UtilIdiomas.GetText("PERFILBASESUBIR", "ERROR_DVIDEO_TIPO");
                string urlRedireccion = BaseURL + "/brightcoveok.aspx";

                pSubirRecModel.SrcIframeBrightcove = $"{mConfigService.ObtenerUrlServicioBrightcove()}?token={tokenID}&urlRedireccion={urlRedireccion}&btSiguiente={UtilIdiomas.GetText("PERFILBASESUBIR", "SIGUIENTE")}&errorSubida={errorSubida}&errorTipo={errorTipo}";

                #endregion

                #region JavaScript

                pSubirRecModel.UrlVideoIframe = mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, ((short)TiposDocumentacion.VideoBrightcove).ToString(), null, (IdentidadOrganizacion != null));
                pSubirRecModel.UrlAudioIframe = mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, ((short)TiposDocumentacion.AudioBrightcove).ToString(), null, (IdentidadOrganizacion != null));

                #endregion

                #endregion
            }

            if (pListaTiposDocPermitidos.Contains(TiposDocumentacion.VideoTOP))
            {
                pSubirRecModel.TOPVideoAvailable = true;

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();

                Guid tokenID = Guid.NewGuid();
                Guid documentoID = Guid.NewGuid();

                AD.EntityModel.Models.Documentacion.DocumentoTokenTOP docTokenTOP = new AD.EntityModel.Models.Documentacion.DocumentoTokenTOP();
                docTokenTOP.TokenID = tokenID;
                docTokenTOP.ProyectoID = ProyectoSeleccionado.Clave;
                docTokenTOP.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                docTokenTOP.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                docTokenTOP.FechaCreacion = DateTime.Now;
                docTokenTOP.Estado = (short)EstadoSubidaVideo.Espera;

                docDW.ListaDocumentoTokenTOP.Add(docTokenTOP);
                mEntityContext.DocumentoTokenTOP.Add(docTokenTOP);
                docCN.ActualizarDocumentacion();
                docCN.Dispose();

                #region Iframe & JS TOP

                #region Iframe
                string errorSubida = UtilIdiomas.GetText("PERFILBASESUBIR", "ERROR_DVIDEO");
                string errorTipo = UtilIdiomas.GetText("PERFILBASESUBIR", "ERROR_DVIDEO_TIPO");
                string urlRedireccion = BaseURL + "/TOPok.aspx";

                pSubirRecModel.SrcIframeTOP = $"{mConfigService.ObtenerUrlServicioTOP()}?token={tokenID}&urlRedireccion={urlRedireccion}&btSiguiente={UtilIdiomas.GetText("PERFILBASESUBIR", "SIGUIENTE")}&errorSubida={errorSubida}&errorTipo={errorTipo}";

                #endregion

                #region JavaScript

                pSubirRecModel.UrlVideoIframe = mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, ((short)TiposDocumentacion.VideoTOP).ToString(), null, (IdentidadOrganizacion != null));
                pSubirRecModel.UrlAudioIframe = mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubirRecurso(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoID, ((short)TiposDocumentacion.AudioTOP).ToString(), null, (IdentidadOrganizacion != null));

                #endregion

                #endregion
            }

            if (!pExaminarPermisos || pListaTiposDocPermitidos.Contains(TiposDocumentacion.Nota))
            {
                pSubirRecModel.NoteAvailable = true;
            }

            if (!mControladorBase.UsuarioActual.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (!pExaminarPermisos || pListaTiposDocPermitidos.Contains(TiposDocumentacion.Wiki)) && ParametrosGeneralesRow.WikiDisponible)
            {
                pSubirRecModel.WikiAvailable = true;
            }

            if (pSubirRecModel.OntologyNameUrls != null && pSubirRecModel.OntologyNameUrls.Count > 0)
            {
                pSubirRecModel.SemanticResourceAvailable = true;
            }
        }

        #endregion

        #region Subir Recurso Parte 2

        /// <summary>
        /// Método inicial de subir recurso parte 2.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index_SubirRecursoPart2()
        {
            mLoggingService.AgregarEntrada("TiemposMVC_Index_SubirRecursoPart2_Inicio");

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && ProyectoSeleccionado.FilaProyecto.Estado != (short)EstadoProyecto.Abierto && ProyectoSeleccionado.FilaProyecto.Estado != (short)EstadoProyecto.Definicion)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            string urlredirect = RecogerParametros_SubirRecursoPart2();

            if (RequestParams("subidaUnificada") != "true" && urlredirect != null)
            {
                return Redirect(urlredirect);
            }

            CargarInicial_SubirRecursoPart2();
            ActionResult redireccion = ComprobarRedirecciones_SubirRecursoPart2();

            if (redireccion != null)
            {
                return redireccion;
            }

            #region Configuración visibilidad

            mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = true;
            mEditRecCont.ModifyResourceModel.EditTitleAvailable = true;
            mEditRecCont.ModifyResourceModel.EditDescriptionAvailable = true;
            mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = false;
            mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = true;
            mEditRecCont.ModifyResourceModel.ShareAvailable = true;
            mEditRecCont.ModifyResourceModel.SelectorEditionAvailable = true;
            mEditRecCont.ModifyResourceModel.DocumentEditionModel.ModificationProtectionAvailable = true;

            if (mTipoDocumento == TiposDocumentacion.Pregunta)
            {
                TipoPagina = TiposPagina.Preguntas;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASE", "SUBIRPREGUNTA");
                mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = false;
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "SUBIRPREGUNTA");
            }
            else if (mTipoDocumento == TiposDocumentacion.Encuesta)
            {
                TipoPagina = TiposPagina.Encuestas;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASE", "SUBIRENCUESTA");
                mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = false;
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "SUBIRENCUESTA");
                mEditRecCont.ModifyResourceModel.EditPollAnswersAvailable = true;
                mEditRecCont.ModifyResourceModel.EditDescriptionAvailable = false;
                mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = false;
            }
            else if (mTipoDocumento == TiposDocumentacion.Debate)
            {
                TipoPagina = TiposPagina.Debates;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASE", "SUBIRDEBATE");
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "SUBIRDEBATE");
                mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = false;
                mEditRecCont.ModifyResourceModel.ShareAvailable = false;
                mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = false;
                mEditRecCont.ModifyResourceModel.SelectorEditionAvailable = false;
            }
            else if (mTipoDocumento == TiposDocumentacion.Newsletter)
            {
                TipoPagina = TiposPagina.BaseRecursos;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("COMMON", "SUBIRNEWSLETTER");
                TituloPagina = UtilIdiomas.GetText("COMMON", "SUBIRNEWSLETTER");


                mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = false;
            }
            else
            {
                TipoPagina = TiposPagina.BaseRecursos;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASE", "SUBIRRECURSO");
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "SUBIRRECURSO");
            }

            if (EsComunidad && (!mTipoDocumento.Equals(TiposDocumentacion.Wiki) && !mTipoDocumento.Equals(TiposDocumentacion.Pregunta) && !mTipoDocumento.Equals(TiposDocumentacion.Encuesta)))
            {
                mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = true;
            }

            bool puedeSeleccionarAbierto = ComprobarIdentidadPuedeSeleccionarVisibilidadAbierto(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);
            mEditRecCont.ModifyResourceModel.CommunityReadersAvailable = true;
            mEditRecCont.ModifyResourceModel.VisibilityMembersCommunity = !ProyectoSeleccionado.EsPublico || ParametroProyecto.ContainsKey("GruposPermitidosSeleccionarPrivacidadRecursoAbierto");
            mEditRecCont.ModifyResourceModel.OpenResourcesAvailable = ProyectoSeleccionado.EsPublico && puedeSeleccionarAbierto;

            if (ParametrosGeneralesRow.PermitirRecursosPrivados)
            {
                if (EsComunidad)
                {
                    mEditRecCont.ModifyResourceModel.PrivateResourcesAvailable = true;
                }
                else
                {
                    mEditRecCont.ModifyResourceModel.PrivateResourcesAvailable = false;
                    mEditRecCont.ModifyResourceModel.CommunityReadersAvailable = false;
                    mEditRecCont.ModifyResourceModel.OpenResourcesAvailable = false;
                }
            }
            else
            {
                mEditRecCont.ModifyResourceModel.PrivateResourcesAvailable = false;
            }

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && !ParametrosGeneralesRow.CompartirRecursosPermitido)
            {
                mEditRecCont.ModifyResourceModel.ShareAvailable = false;
            }

            #endregion

            #region Recuperar autoguardo

            if (mTipoDocumento == TiposDocumentacion.Wiki)
            {
                if (DocumentoAutoGuardadoWiki != null) //Solo si hay algo que recuperar
                {
                    if (RequestParams("recuperarAutoGuardado") == null)
                    {
                        mEditRecCont.ModifyResourceModel.DocumentEditionModel.AutosaveAvailable = true;
                        mEditRecCont.ModifyResourceModel.DocumentEditionModel.UrlRecoverAutosave = ViewBag.UrlPagina + "?recuperarAutoGuardado=" + HttpUtility.HtmlEncode(mNombreDocumento);
                    }
                    else
                    {
                        CargarDatosDocumento_ModificarRecurso(mEditRecCont.ModifyResourceModel.DocumentEditionModel);
                    }
                }
            }

            #endregion

            UsersSelectorModel selecEditModel = new UsersSelectorModel();
            mEditRecCont.ModifyResourceModel.UsersSelectorEditionModel = selecEditModel;

            UsersSelectorModel selecLectModel = new UsersSelectorModel();
            mEditRecCont.ModifyResourceModel.UsersSelectorReadingModel = selecLectModel;

            #region Título predefinido

            mEditRecCont.ModifyResourceModel.DocumentEditionModel.Draft = true;

            if (mTipoDocumento.Equals(TiposDocumentacion.Wiki))
            {
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.Title = mNombreDocumento;
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.Draft = false;
            }
            else if (mTipoDocumento == TiposDocumentacion.FicheroServidor || mTipoDocumento == TiposDocumentacion.Video || mTipoDocumento == TiposDocumentacion.Imagen)
            {
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.Title = mNombreDocumento;

                if (Path.GetExtension(mNombreDocumento) != "")
                {
                    mEditRecCont.ModifyResourceModel.DocumentEditionModel.Title = mNombreDocumento.Replace(System.IO.Path.GetExtension(mNombreDocumento), "");
                }
            }

            #endregion

            #region Autoría documento

            mEditRecCont.ModifyResourceModel.CopyrightAvailable = !(mTipoDocumento.Equals(TiposDocumentacion.Wiki) || mTipoDocumento.Equals(TiposDocumentacion.Pregunta) || mTipoDocumento.Equals(TiposDocumentacion.Debate) || mTipoDocumento.Equals(TiposDocumentacion.Encuesta));

            #endregion

            #region Selector de categoría del tesauro

            PrepararTesauro_SubirRecursoPart2();

            #endregion

            #region Protección de documentos

            mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = (mTipoDocumento != TiposDocumentacion.EntradaBlog && mTipoDocumento != TiposDocumentacion.Hipervinculo && mTipoDocumento != TiposDocumentacion.ReferenciaADoc && mTipoDocumento != TiposDocumentacion.Ontologia && mTipoDocumento != TiposDocumentacion.Wiki && EsComunidad);

            #endregion

            #region Permitir Compartir

            mEditRecCont.ModifyResourceModel.DocumentEditionModel.ShareAllowed = (EsComunidad && (ProyectoSeleccionado.FilaProyecto.TipoAcceso != (short)TipoAcceso.Privado && ProyectoSeleccionado.FilaProyecto.TipoAcceso != (short)TipoAcceso.Reservado) && mTipoDocumento != TiposDocumentacion.Debate && mTipoDocumento != TiposDocumentacion.Encuesta);

            #endregion

            #region Propiedad intelectual

            mEditRecCont.ModifyResourceModel.DocumentEditionModel.ActualIdentityIsCreator = true;
            mEditRecCont.ModifyResourceModel.DocumentEditionModel.AllowsLicense = (mTipoDocumento == TiposDocumentacion.Nota || mTipoDocumento == TiposDocumentacion.FicheroServidor || mTipoDocumento == TiposDocumentacion.Semantico || mTipoDocumento == TiposDocumentacion.Newsletter || mTipoDocumento == TiposDocumentacion.VideoBrightcove || mTipoDocumento == TiposDocumentacion.AudioBrightcove || mTipoDocumento == TiposDocumentacion.VideoTOP || mTipoDocumento == TiposDocumentacion.AudioTOP || mTipoDocumento == TiposDocumentacion.Imagen);

            if (mEditRecCont.ModifyResourceModel.DocumentEditionModel.AllowsLicense)
            {
                PrepararLicencia();
            }

            #endregion

            if (mTipoDocumento == TiposDocumentacion.Encuesta)
            {
                mEditRecCont.ModifyResourceModel.EditPollAnswersAvailable = true;
                mEditRecCont.ModifyResourceModel.PollAnswersModel = new PollAnswersModel();
            }

            return null;
        }

        /// <summary>
        /// Comprueba si la identidad tiene permiso para configurar la visibilidad abierto
        /// </summary>
        /// <returns>True si no existen grupos o, si existen, la identidad pertenece a alguno de ellos</returns>
        private bool ComprobarIdentidadPuedeSeleccionarVisibilidadAbierto(Guid pIdentidadID, Guid pIdentidadEnMyGnossID)
        {
            bool gruposVisibilidadConfigurados = ParametroProyecto.ContainsKey("GruposPermitidosSeleccionarPrivacidadRecursoAbierto");
            bool pertenceAGrupo = false;
            if (gruposVisibilidadConfigurados)
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                string[] grupos = ParametroProyecto["GruposPermitidosSeleccionarPrivacidadRecursoAbierto"].Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string grupo in grupos)
                {
                    pertenceAGrupo = identCN.ParticipaIdentidadEnGrupo(pIdentidadID, new Guid(grupo)) || identCN.ParticipaIdentidadEnGrupo(pIdentidadEnMyGnossID, new Guid(grupo));
                    if (pertenceAGrupo)
                    {
                        break;
                    }
                }

                identCN.Dispose();
            }
            return !gruposVisibilidadConfigurados || pertenceAGrupo;
        }

        /// <summary>
        /// Hace la carga inicial para la segunda parte de subir recurso.
        /// </summary>
        public void CargarInicial_SubirRecursoPart2()
        {
            CargaInicial();
            CargaInicialTesauro();

            if (mTipoDocumento == TiposDocumentacion.Wiki)
            {
                CargarDocumentosTemporalesAutoguardados_SubirRecursoPart2();
            }
        }

        /// <summary>
        /// Recoge los parámetros de la página.
        /// </summary>
        public string RecogerParametros_SubirRecursoPart2()
        {
            ObtenerTipoDocumentoPeticion();
            if (mEditRecCont == null)
            {
                mEditRecCont = new EditResourceModel();
            }

            mEditRecCont.TypePage = EditResourceModel.TypePageEditResource.CreateResource2;
            ModifyResourceModel modelModRec = new ModifyResourceModel();
            modelModRec.EditResourceModel = mEditRecCont;
            mEditRecCont.ModifyResourceModel = modelModRec;
            mEditRecCont.ModifyResourceModel.EditPropertiesAvailable = true;
            modelModRec.DocumentEditionModel = new DocumentEditionModel();
            modelModRec.DocumentType = (ResourceModel.DocumentType)mTipoDocumento;

            string strDocumentoID = RequestParams("documentoID");
            if (!string.IsNullOrEmpty(strDocumentoID))
            {
                Guid.TryParse(strDocumentoID, out mDocumentoID);
            }

            if (RequestParams("docID") != null && mDocumentoID.Equals(Guid.Empty))
            {
                mDocumentoID = new Guid(RequestParams("docID"));
            }

            if (mTipoDocumento.Equals(TiposDocumentacion.Hipervinculo) || mTipoDocumento.Equals(TiposDocumentacion.FicheroServidor) || mTipoDocumento.Equals(TiposDocumentacion.Imagen) || mTipoDocumento.Equals(TiposDocumentacion.Video) || mTipoDocumento.Equals(TiposDocumentacion.Audio) || mTipoDocumento.Equals(TiposDocumentacion.VideoTOP) || mTipoDocumento.Equals(TiposDocumentacion.AudioTOP))
            {
                //si es enlace o adjunto es necesario darle el documentoID de la url porque el enlace o fichero ya se ha vinculado a ese ID
                modelModRec.DocumentEditionModel.Key = mDocumentoID;
            }

            if (Session.Get("EnlaceDocumentoAgregar") != null && !mTipoDocumento.Equals(TiposDocumentacion.Debate) && !mTipoDocumento.Equals(TiposDocumentacion.Pregunta) && !mTipoDocumento.Equals(TiposDocumentacion.Encuesta))
            {
                mNombreDocumento = Session.GetString("EnlaceDocumentoAgregar");
                modelModRec.DocumentEditionModel.Link = mNombreDocumento;

                if (Session.Get("idGoogleDrive") != null)
                {
                    string googleID = Session.GetString("idGoogleDrive");
                    Session.Remove("idGoogleDrive");
                    FileInfo informacionNombre = new FileInfo(mNombreDocumento);
                    modelModRec.DocumentEditionModel.Link = ExtraerNombreFicheroSinExtension(informacionNombre.Name) + ID_GOOGLE + googleID + informacionNombre.Extension;
                }
            }
            else if (RequestParams("recuperarAutoGuardado") != null && mTipoDocumento == TiposDocumentacion.Wiki)
            {
                mNombreDocumento = RequestParams("recuperarAutoGuardado");
            }

            return null;
        }

        /// <summary>
        /// Prepara el tesauro para la segunda parte de subir recurso.
        /// </summary>
        private void PrepararTesauro_SubirRecursoPart2()
        {
            ParametroAplicacion filaParametroAplicacion = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("UsarSoloCategoriasPrivadasEnEspacioPersonal"));
            if (filaParametroAplicacion != null && (filaParametroAplicacion.Valor.Equals("1") || filaParametroAplicacion.Valor.Equals("true")))
            {
                GestorDocumental.GestorTesauro.EliminarCategoriasPublicasSiEsMetaEspacioGNOSS(UtilIdiomas.LanguageCode);
            }

            mEditRecCont.ModifyResourceModel.ThesaurusEditorModel = new ThesaurusEditorModel();
            mEditRecCont.ModifyResourceModel.ThesaurusEditorModel.ThesaurusCategories = CargarTesauroPorGestorTesauro(GestorDocumental.GestorTesauro);

            ComprobarCategoriasDeshabilitadas(GestorDocumental.GestorTesauro, mEditRecCont.ModifyResourceModel.ThesaurusEditorModel);

            if (mTipoDocumento == TiposDocumentacion.Wiki && RequestParams("recuperarAutoGuardado") != null)
            {
                List<Guid> listaCatModelSeleccionadas = new List<Guid>();

                foreach (CategoriaTesauro categoria in DocumentoAutoGuardadoWiki.Categorias.Values)
                {
                    CategoryModel categoriaTesauro = new CategoryModel();
                    categoriaTesauro.Key = categoria.Clave;
                    categoriaTesauro.Name = categoria.FilaCategoria.Nombre;
                    categoriaTesauro.LanguageName = categoria.FilaCategoria.Nombre;
                    categoriaTesauro.Lang = UtilIdiomas.LanguageCode;

                    listaCatModelSeleccionadas.Add(categoria.Clave);
                }

                mEditRecCont.ModifyResourceModel.ThesaurusEditorModel.SelectedCategories = listaCatModelSeleccionadas;
            }
            else
            {
                mEditRecCont.ModifyResourceModel.ThesaurusEditorModel.SelectedCategories = new List<Guid>();

                if (mEditandoRecursoCargaMasiva && !string.IsNullOrEmpty(mDatosRecursoEditadoCargaMasiva[6]))
                {
                    char[] separador = { ',' };
                    foreach (string categoriaID in mDatosRecursoEditadoCargaMasiva[6].Split(separador, StringSplitOptions.RemoveEmptyEntries).Where(item => GestorDocumental.GestorTesauro.ListaCategoriasTesauro.ContainsKey(new Guid(item))))
                    {
                        mEditRecCont.ModifyResourceModel.ThesaurusEditorModel.SelectedCategories.Add(new Guid(categoriaID));
                    }
                }
            }
        }

        private static void ComprobarCategoriasDeshabilitadas(GestionTesauro pGestorTesauro, ThesaurusEditorModel pThesaurusEditorModel)
        {
            if (pThesaurusEditorModel.DisabledCategories == null)
            {
                pThesaurusEditorModel.DisabledCategories = new List<Guid>();
            }

            foreach (CategoriaTesauro catTesauro in pGestorTesauro.ListaCategoriasTesauro.Values.Where(categoria => !categoria.EsSeleccionable))
            {
                pThesaurusEditorModel.DisabledCategories.Add(catTesauro.Clave);
            }
        }

        /// <summary>
        /// Comprueba si hay que redireccionar y si es así devuelve la redirección.
        /// </summary>
        /// <returns>Redirección resultante</returns>
        private ActionResult ComprobarRedirecciones_SubirRecursoPart2()
        {
            ActionResult result = null;
            switch (mTipoDocumento)
            {
                case TiposDocumentacion.Hipervinculo:
                    result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearRecursoTipoEnlace);
                    break;
                case TiposDocumentacion.Nota:
                    result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearNota);
                    break;
                case TiposDocumentacion.FicheroServidor:
                    result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearRecursoTipoAdjunto);
                    break;
                case TiposDocumentacion.ReferenciaADoc:
                    result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearRecursoTipoReferenciaADocumentoFisico);
                    break;
                case TiposDocumentacion.Pregunta:
                    result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearPregunta);
                    break;
                case TiposDocumentacion.Encuesta:
                    result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearEncuesta);
                    break;
                case TiposDocumentacion.Debate:
                    result = ComprobarTienePermisosAccionRecurso(PermisoRecursos.CrearDebate);
                    break;
                case TiposDocumentacion.Semantico:
                    result = ComprobarTienePermisosAccionRecursoSemantico(TipoPermisoRecursosSemanticos.Crear);
                    break;
            }
            bool tienePermiso = result == null;

            if (result != null)
            {
                return result;
            }


            if (!string.IsNullOrEmpty(RequestParams("organizacion")) && IdentidadActual.IdentidadOrganizacion != null)
            {
                if (!mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion, IdentidadActual.OrganizacionID.Value) && !mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, IdentidadActual.OrganizacionID.Value))
                {
                    return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosInicio(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, true));
                }
            }

            if (!mControladorBase.UsuarioActual.EsIdentidadInvitada && ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && !ListaPermisosDocumentos.Contains(mTipoDocumento) && (mTipoDocumento != TiposDocumentacion.Newsletter || !ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID)))
            {
                if (!tienePermiso)
                {
                    return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosInicio(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, IdentidadActual.OrganizacionID != null));
                }
            }

            if (!mControladorBase.UsuarioActual.EsIdentidadInvitada && mTipoDocumento == TiposDocumentacion.Semantico && !ComprobarPermisoEnOntologiaDeProyectoEIdentidad(mOntologiaID))
            {
                if (!tienePermiso)
                {
                    return Redirect(BaseURLIdioma);
                }
            }

            if (mControladorBase.UsuarioActual.EsIdentidadInvitada && (!FormSemVirtual || !PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.permitirUsuNoLogueado.ToString())))
            {
                return Redirect(BaseURLIdioma);
            }

            return null;
        }

        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult CrearTokenTOP()
        {
            Guid tokenID = Guid.NewGuid();

            string strDocumentoID = RequestParams("documentoID");
            if (!string.IsNullOrEmpty(strDocumentoID))
            {
                Guid.TryParse(strDocumentoID, out mDocumentoID);
            }

            if (mDocumentoID.Equals(Guid.Empty))
            {
                mDocumentoID = new Guid(RequestParams("docID"));
            }

            if (!mDocumentoID.Equals(Guid.Empty))
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();

                AD.EntityModel.Models.Documentacion.DocumentoTokenTOP docTokenTOP = new AD.EntityModel.Models.Documentacion.DocumentoTokenTOP();
                docTokenTOP.TokenID = tokenID;
                docTokenTOP.ProyectoID = ProyectoSeleccionado.Clave;
                docTokenTOP.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                docTokenTOP.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                docTokenTOP.DocumentoID = mDocumentoID;
                docTokenTOP.FechaCreacion = DateTime.Now;
                docTokenTOP.Estado = (short)EstadoSubidaVideo.Espera;


                docDW.ListaDocumentoTokenTOP.Add(docTokenTOP);
                docCN.ActualizarDocumentacion();
                docCN.Dispose();
            }

            string errorSubida = UtilIdiomas.GetText("PERFILBASESUBIR", "ERROR_DVIDEO");
            string errorTipo = UtilIdiomas.GetText("PERFILBASESUBIR", "ERROR_DVIDEO_TIPO");
            string urlRedireccion = BaseURL + "/TOPok.aspx";

            string urlTop = $"{mConfigService.ObtenerUrlServicioTOP()}?token={tokenID}&urlRedireccion={urlRedireccion}&btSiguiente={UtilIdiomas.GetText("PERFILBASESUBIR", "SIGUIENTE")}&errorSubida={errorSubida}&errorTipo={errorTipo}";

            return GnossResultOK(urlTop);
        }

        /// <summary>
        /// Médodo para crear un recurso.
        /// </summary>
        /// <param name="pModel">Argumentos</param>
        /// <param name="pRealizarCargaInicial">Indica si ya se ha cargado anteriormente</param>
        /// <returns>Respuesta acción</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult CrearRecurso_SubirRecursoPart2(DocumentEditionModel pModel, bool pRealizarCargaInicial = true)
        {
            mModelSaveRec = pModel;
            RecogerParametros_SubirRecursoPart2();
            mNombreDocumento = pModel.Link;//Lo establezco ya, que la consulta para saber si hay autoguardado lo usa.

            if (pRealizarCargaInicial)
            {
                CargarInicial_SubirRecursoPart2();
            }

            if (Duplicando)
            {
                mDocumentoID = mDocumentoDuplicadoID;
            }

            if (pModel.AutoSave.HasValue && pModel.AutoSave.Value)
            {
                return AutoGuardadoWiki_SubirRecursoPart2();
            }

            ActionResult redireccion = ComprobarRedirecciones_SubirRecursoPart2();

            if (redireccion != null)
            {
                return redireccion;
            }

            if (!pModel.SkipRepeat)
            {
                ActionResult htmlPanelRepe = ComprobarRepeticionEnlaceRecurso_SubirRecursoPart2();

                if (htmlPanelRepe != null)
                {
                    return htmlPanelRepe;
                }
            }

            List<Guid> categoriasSeleccionadas = RecogerCategoriasSeleccionadas();
            bool borrador = mModelSaveRec.Draft;

            string mensError = ValidarRecurso(mModelSaveRec.Draft, categoriasSeleccionadas);

            if (!string.IsNullOrEmpty(mensError))
            {
                return GnossResultERROR(mensError);
            }

            if (!borrador)
            {
                if (Session.Get("DocumentoVisto") == null)
                {
                    Session.Set<List<Guid>>("DocumentoVisto", new List<Guid>());
                }
                List<Guid> documentosVistos = Session.Get<List<Guid>>("DocumentoVisto");
                documentosVistos.Add(mDocumentoID);
                Session.Set("DocumentoVisto", documentosVistos);
            }

            #region Eliminar documento autoguardado

            //Elimino el posible documento temporal autoguardado.
            if (DocumentoAutoGuardadoWiki != null)
            {
                GestorDocumental.EliminarDocumentoWeb(DocumentoAutoGuardadoWiki);
            }

            #endregion

            //Aqui haremos la peticion para Vincular los Recursos, En CrearDocumentoEnModeloAcido.
            Documento doc = CrearDocumentoEnModeloAcido(mDocumentoID, categoriasSeleccionadas);

            if (SubidaUnificada && doc.TipoDocumentacion == TiposDocumentacion.FicheroServidor)
            {
                ReemplazarArchivos_ModificarRecurso();
            }

            CrearDocumentoEnModeloLive(doc);

            if (doc.TipoDocumentacion == TiposDocumentacion.Hipervinculo || doc.TipoDocumentacion == TiposDocumentacion.Nota || doc.TipoDocumentacion == TiposDocumentacion.VideoBrightcove || doc.TipoDocumentacion == TiposDocumentacion.VideoTOP || doc.EsVideoIncrustado || doc.EsPresentacionIncrustada)
            {
                ControladorDocumentacion.CapturarImagenWeb(doc.Clave, true, PrioridadColaDocumento.Alta, mAvailableServices);
            }
            else if (doc.TipoDocumentacion == TiposDocumentacion.Imagen)
            {
                ControladorDocumentacion.CapturarImagenWeb(doc.Clave, false, PrioridadColaDocumento.Alta, mAvailableServices);
            }
            else if (doc.TipoDocumentacion == TiposDocumentacion.FicheroServidor)
            {
                //Trata de capturar la imagen de la descripción si la hay.
                ControladorDocumentacion.CapturarImagenWeb(doc.Clave, false, PrioridadColaDocumento.Alta, mAvailableServices);
            }

            if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MyGnoss))
            {
                ControladorDocumentacion.AgregarRecursoModeloBaseSimple(doc.Clave, ProyectoSeleccionado.Clave, doc.FilaDocumento.Tipo, null, ",##enlaces##" + ExtraerTexto(mModelSaveRec.TagsLinks) + "##enlaces##", PrioridadBase.Alta, mAvailableServices);
                mInsertadoEnGrafoBusqueda = true;
            }

            mOtrosArgumentosBase = ",##enlaces##" + ExtraerTexto(mModelSaveRec.TagsLinks) + "##enlaces##";

            CrearDocumentoAccionesExtra(doc);

            return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, doc, IdentidadOrganizacion != null) + "?created");
        }

        /// <summary>
        /// Crea un documento en el modelo ácido.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento que se va a crear</param>
        /// <param name="pCategoriasSeleccionadas">Categorías del tesauro seleccionadas</param>
        /// <returns>Documento creado</returns>
        private Documento CrearDocumentoEnModeloAcido(Guid pDocumentoID, List<Guid> pCategoriasSeleccionadas)
        {
            if (GestorDocumental.GestorIdentidades == null)
            {
                GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
            }
            else
            {
                GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(IdentidadActual.GestorIdentidades.DataWrapperIdentidad);
                GestorDocumental.GestorIdentidades.RecargarHijos();
            }
            ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorIdentidades>(), mLoggerFactory);
            controladorIdentidades.CompletarCargaIdentidad(IdentidadActual.Clave);

            #region Documento Tipo documento

            Documento doc = null;
            string titulo = UtilCadenas.EliminarHtmlDeTexto(mModelSaveRec.Title.Trim());
            string descripcion = HttpUtility.UrlDecode(mModelSaveRec.Description);
            string tags = UtilCadenas.EliminarHtmlDeTexto(mModelSaveRec.Tags?.Trim());
            string rutaFichero = null;
            Guid elementoVinculadoID = Guid.Empty;

            if (mTipoDocumento == TiposDocumentacion.FicheroServidor || mTipoDocumento == TiposDocumentacion.Imagen || mTipoDocumento == TiposDocumentacion.ReferenciaADoc || mTipoDocumento == TiposDocumentacion.Semantico || mTipoDocumento == TiposDocumentacion.Nota || mTipoDocumento == TiposDocumentacion.Newsletter || mTipoDocumento == TiposDocumentacion.Pregunta || mTipoDocumento == TiposDocumentacion.Debate || mTipoDocumento == TiposDocumentacion.Encuesta)
            {
                if (pDocumentoID != Guid.Empty)
                {
                    if (mTipoDocumento == TiposDocumentacion.Semantico)
                    {
                        mNombreDocumento = UtilCadenas.EliminarCaracteresUrlSem(mModelSaveRec.Title) + ".rdf";
                        elementoVinculadoID = mOntologiaID;
                    }

                    //Añadimos el documento a la definición y a los dataset que corresponda
                    rutaFichero = mNombreDocumento;
                }
            }
            else if (mTipoDocumento == TiposDocumentacion.Video)
            {
                if (pDocumentoID != Guid.Empty)
                {
                    //Añadimos el documento a la definición y a los dataset que corresponda
                    int indicepunto = mNombreDocumento.LastIndexOf(".");
                    string documentoSinExtension;

                    if (indicepunto != -1)
                    {
                        documentoSinExtension = mNombreDocumento.Substring(0, indicepunto);
                    }
                    else
                    {
                        documentoSinExtension = mNombreDocumento;
                    }

                    //Ponemos los autores a false para que no se ponga un enlace a todos los autores
                    rutaFichero = documentoSinExtension + ".flv";
                }
            }
            else if (mTipoDocumento == TiposDocumentacion.VideoBrightcove || mTipoDocumento == TiposDocumentacion.AudioBrightcove || mTipoDocumento == TiposDocumentacion.VideoTOP || mTipoDocumento == TiposDocumentacion.AudioTOP)
            {
                if (pDocumentoID != Guid.Empty)
                {
                    rutaFichero = RequestParams("size");
                }
            }
            else//Es enlace
            {
                rutaFichero = mNombreDocumento.Trim();
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (UtilCadenas.EsEnlaceSharepoint(rutaFichero, oneDrivePermitido))
                {
                    AD.EntityModel.Models.Documentacion.Documento docAux = new AD.EntityModel.Models.Documentacion.Documento();
                    docAux.Enlace = rutaFichero;
                    FuncionalidadSharepoint(docAux);
                    rutaFichero = docAux.Enlace;
                }

            }

            if (rutaFichero == null)
            {
                if (mTipoDocumento != TiposDocumentacion.Newsletter && mTipoDocumento != TiposDocumentacion.Encuesta && mTipoDocumento != TiposDocumentacion.Pregunta && mTipoDocumento != TiposDocumentacion.Debate && mTipoDocumento != TiposDocumentacion.VideoTOP && mTipoDocumento != TiposDocumentacion.AudioTOP && mTipoDocumento == TiposDocumentacion.AudioBrightcove && mTipoDocumento == TiposDocumentacion.VideoBrightcove)
                {
                    throw new ExcepcionWeb("Enlace vacío");
                }
                else
                {
                    rutaFichero = "";
                }
            }
            doc = GestorDocumental.AgregarDocumento(rutaFichero, titulo, descripcion, tags, mTipoDocumento, TipoEntidadVinculadaDocumento.Web, elementoVinculadoID, mModelSaveRec.ShareAllowed, mModelSaveRec.Draft, mModelSaveRec.CreatorIsAuthor, null, false, UsuarioActual.OrganizacionID, UsuarioActual.IdentidadID);

            //Cambiar identificador del documento
            if (GestorDocumental.ListaDocumentos.ContainsKey(doc.Clave))
            {
                GestorDocumental.ListaDocumentos.Remove(doc.Clave);
                GestorDocumental.ListaDocumentos.Add(pDocumentoID, doc);
            }
            else
            {
                GestorDocumental.ListaDocumentos.Add(pDocumentoID, doc);
            }
            doc.FilaDocumento.DocumentoID = pDocumentoID;

            #endregion

            //Agrego la comunidad a la que pertenece el documento:
            doc.FilaDocumento.ProyectoID = mControladorBase.UsuarioActual.ProyectoID;

            if (IdentidadOrganizacion != null)
            {
                doc.FilaDocumento.CreadorID = IdentidadOrganizacion.Clave;
                doc.FilaDocumento.OrganizacionID = (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID");
            }
            else if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto && IdentidadActual.Tipo != TiposIdentidad.Personal)
            {
                if (IdentidadActual.IdentidadPersonalMyGNOSS != null)
                {
                    doc.FilaDocumento.CreadorID = IdentidadActual.IdentidadPersonalMyGNOSS.Clave;
                }
                else
                {
                    doc.FilaDocumento.CreadorID = IdentidadActual.IdentidadMyGNOSS.Clave;
                }
            }

            List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

            foreach (Guid clave in pCategoriasSeleccionadas)
            {
                try
                {
                    CategoriaTesauro categoria = GestorDocumental.GestorTesauro.ListaCategoriasTesauro[clave];
                    listaCategorias.Add(categoria);
                }
                catch (Exception ex)
                {
                    if (mTipoDocumento == TiposDocumentacion.Semantico)
                    {
                        GuardarMensajeErrorAdmin("En el XML de la ontología hay configurada una categoría del tesauro que no pertenece al de esta comunidad.", ex);
                        GuardarLogError("En el XML de la ontología hay configurada una categoría del tesauro que no pertenece al de esta comunidad.");
                    }
                    throw;
                }
            }

            if (IdentidadOrganizacion != null)
            {
                controladorIdentidades.CompletarCargaIdentidad(IdentidadOrganizacion.Clave);

                GestorDocumental.AgregarDocumento(listaCategorias, doc, IdentidadOrganizacion.Clave, mModelSaveRec.ResourceVisibility == ResourceVisibility.SpecificReaders || mModelSaveRec.ResourceVisibility == ResourceVisibility.OnlyEditors, UsuarioActual.ProyectoID);
            }
            else if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto && IdentidadActual.Tipo != TiposIdentidad.Personal)
            {
                Guid identidad;

                if (IdentidadActual.IdentidadPersonalMyGNOSS != null)
                {
                    identidad = IdentidadActual.IdentidadPersonalMyGNOSS.Clave;
                }
                else
                {
                    identidad = IdentidadActual.IdentidadMyGNOSS.Clave;
                }

                controladorIdentidades.CompletarCargaIdentidad(identidad);
                GestorDocumental.AgregarDocumento(listaCategorias, doc, identidad, mModelSaveRec.ResourceVisibility == ResourceVisibility.SpecificReaders || mModelSaveRec.ResourceVisibility == ResourceVisibility.OnlyEditors, UsuarioActual.ProyectoID);
            }
            else
            {
                controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                GestorDocumental.AgregarDocumento(listaCategorias, doc, mControladorBase.UsuarioActual.IdentidadID, mModelSaveRec.ResourceVisibility == ResourceVisibility.SpecificReaders || mModelSaveRec.ResourceVisibility == ResourceVisibility.OnlyEditors, UsuarioActual.ProyectoID);
            }

            //Asignamos el doc de nuevo porque se crea una nueva instancia en los métodos anteriores:
            doc = GestorDocumental.ListaDocumentos[doc.Clave];

            if (CreandoFormSem || !EditandoRecurso)
            {
                GestorDocumental.CrearPrimeraVersionDocumento(doc);
            }

            ExtraerCambiosBasicos(doc);
            GuardarDatosAutoriaDocumento(doc);

            bool privacidadCambiada = false;
            List<Guid> listaEditoresEliminados;
            List<Guid> listaGruposEditoresEliminados;
            GuardarDatosEditores(doc, out privacidadCambiada, out listaEditoresEliminados, out listaGruposEditoresEliminados);
            GuardarDatosRespuestasEncuesta(doc);

            #region Tamaño archivo

            if (RequestParams("size") != null && !EsComunidad)
            {
                double sizeArchivo = double.Parse(RequestParams("size"));
                GestorDocumental.EspacioActualBaseRecursos = GestorDocumental.EspacioActualBaseRecursos + sizeArchivo;
            }

            #endregion

            #region Licencia propiedad intelectual

            if (!string.IsNullOrEmpty(mModelSaveRec.License) && mModelSaveRec.CreatorIsAuthor && (mModelSaveRec.ShareAllowed || (EsComunidad && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido))))
            {
                doc.FilaDocumento.Licencia = mModelSaveRec.License;
            }

            #endregion

            GuardarImagenPrincipalRecSemantico(doc, mOntologia);

            List<Guid> listaProyectosAcuNumRec = new List<Guid>();

            if (EsComunidad && !mModelSaveRec.Draft)
            {
                listaProyectosAcuNumRec.Add(mControladorBase.UsuarioActual.ProyectoID);
            }

            //Insertar Vinculados.
            if (!string.IsNullOrEmpty(RequestParams("Vinculado")) && !RequestParams("Vinculado").Equals("SinModificacion"))
            {
                List<string> listaVinculados = UtilCadenas.SepararTexto(RequestParams("Vinculado"));

                foreach (string vinculado in listaVinculados)
                {
                    AnyadirVinculado(vinculado, pDocumentoID);
                }
            }

            AgregarEstadoDocumento(doc.FilaDocumento);

            GuardarEnBD_SubirRecursoPart2(listaProyectosAcuNumRec);

            ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(doc, IdentidadActual, true);

            if (!mModelSaveRec.Draft)
            {
                try
                {
                    //Actualización Offline a partir de un servicio UDP
                    //Llamada asincrona para actualizar la popularidad del recurso:
                    ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("recursos", doc.Clave, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.IdentidadID, doc.CreadorID);

                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                }
            }

            //Agregada variable de sesión para el registro del evento Subir Recurso
            Session.Set("EventoSubirRecurso", true);

            return doc;
        }

        private Guid ObtenerProyectoOrigenFichaRecurso(Guid pProyectoActualID)
        {
            Guid proyectoOrigenID = Guid.Empty;

            // Obtenemos si existe el recurso pasado para el proyectoActual
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            Guid baseRecursosID = docCN.ObtenerBaseRecursosIDProyecto(pProyectoActualID);

            //Si el recurso esta en la propia comunidad no hacemos nada
            if ((Documento.FilaDocumentoWebVinBR != null && Documento.FilaDocumentoWebVinBR.BaseRecursosID != baseRecursosID) || (Documento.FilaDocumentoWebVinBR_Publicador != null && Documento.FilaDocumentoWebVinBR_Publicador.BaseRecursosID != baseRecursosID) || (Documento.FilaDocumentoWebVinBR_SinCompartir != null && Documento.FilaDocumentoWebVinBR_SinCompartir.BaseRecursosID != baseRecursosID))
            {
                // El recurso no está publicado en el proyecto actual y tampoco está compartido en él.
                proyectoOrigenID = ProyectoOrigenBusquedaID;
            }

            docCN.Dispose();

            return proyectoOrigenID;
        }

        public bool EsIdentidadBROrg
        {
            get
            {
                return !string.IsNullOrEmpty(RequestParams("organizacion"));
            }
        }

        private KeyValuePair<string, string> PestanyaRecurso
        {
            get
            {
                if (mPestanyaRecurso == null)
                {
                    mPestanyaRecurso = ControladorDocumentoMVC.ObtenerPestanyaRecurso(Documento, UtilIdiomas, ParametrosGeneralesRow.IdiomaDefecto, ProyectoSeleccionado);
                }


                return mPestanyaRecurso.Value;
            }
        }
        private Guid DocumentoID
        {
            get
            {
                return new Guid(RequestParams("docID"));
            }
        }

        /// <summary>
        /// Mostrar las url de los documentos Vinculados
        /// </summary>
        /// <param name="pVincular">pUrlVincular</param>
        private List<string> MostrarVinculado()
        {
            List<string> listaStringVinculados = new List<string>();

            List<Guid> listaGuidVinculados = Documento.DocumentosVinculados;

            List<string> BaseURLsContent = new List<string>();
            BaseURLsContent.Add(BaseURLContent);
            string nombreSem = PestanyaRecurso.Value;
            string baseUrlBusqueda = $"{mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, EsIdentidadBROrg, nombreSem)}/";
            EspacioPersonal espacioPersonal = Controles.Controladores.EspacioPersonal.No;

            if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
            {
                if (EsIdentidadBROrg)
                {
                    espacioPersonal = Controles.Controladores.EspacioPersonal.Organizacion;
                }
                else
                {
                    espacioPersonal = Controles.Controladores.EspacioPersonal.Usuario;
                }
            }
            Guid proyectoOrigenID = ObtenerProyectoOrigenFichaRecurso(ProyectoSeleccionado.Clave);
            ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLsContent, BaseURLStatic, ProyectoSeleccionado, proyectoOrigenID, ParametrosGeneralesRow, IdentidadActual, mControladorBase.EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);

            Dictionary<Guid, ResourceModel> listaRecursos = controladorMVC.ObtenerRecursosPorID(listaGuidVinculados, baseUrlBusqueda, espacioPersonal, null, false);

            foreach (ResourceModel vinculado in listaRecursos.Values)
            {
                if (vinculado != null)
                {
                    listaStringVinculados.Add(vinculado.CompletCardLink);
                }
            }

            return listaStringVinculados;
        }

        /// <summary>
        /// Borra los recursos vinculados a este documento.
        /// </summary>
        /// <param name="pDocumentoID">pDocumentoID</param>
        private void BorrarVinculados(Guid pDocumentoID)
        {
            List<Guid> listaVinculados = VinculadosEnBD(pDocumentoID);

            foreach (Guid vinculado in listaVinculados)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                docCN.ObtenerDocumentoPorIDCargarTotal(vinculado, GestorDocumental.DataWrapperDocumentacion, true, false, null);
                GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerVinculacionesRecurso(vinculado));
                docCN.Dispose();

                GestorDocumental.DesVincularDocumentos(vinculado, pDocumentoID);
            }
        }

        /// <summary>
        /// Obtiene la lista de los Vinculados en BD
        /// </summary>
        /// <param name="pDocumentoID">pDocumentoID</param>
        private List<Guid> VinculadosEnBD(Guid pDocumentoID)
        {
            List<Guid> listaGuidBorrar = new List<Guid>();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoVincDoc filaVincDoc in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoVincDoc.Where(item => item.DocumentoID.Equals(pDocumentoID)))
            {
                listaGuidBorrar.Add(filaVincDoc.DocumentoVincID);
            }

            return listaGuidBorrar;
        }

        /// <summary>
        /// Vincula el Documento que estamos creando con el que ha introducido el usuario.
        /// </summary>
        /// <param name="pVincular">pUrlVincular</param>
        private void AnyadirVinculado(string pUrlVincular, Guid pDocumentoID)
        {
            string mensajeError = "";
            string urlDoc = pUrlVincular;

            if (urlDoc.Contains('?'))
            {
                urlDoc = urlDoc.Substring(0, urlDoc.IndexOf('?'));
            }

            if (urlDoc != "")
            {
                try
                {
                    //Buscamos si se esta compartiendo desde una comunidad con url propia
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    List<string> listaUrlPropia = proyCN.ObtenerUrlPropiasProyectos();
                    bool urlEncontrada = listaUrlPropia.Any(item => urlDoc.Contains(item));

                    //Si no se comparte desde una url propia ni se comparte desde gnoss
                    if (!urlEncontrada && !urlDoc.Contains(BaseURLSinHTTP))
                    {
                        throw new ExcepcionWeb("Mala url");
                    }

                    string id = urlDoc.Substring(urlDoc.LastIndexOf("/") + 1);
                    Guid documentoVincID = new Guid(id);

                    string urlComunidad = urlDoc.Substring(0, urlDoc.LastIndexOf("/"));//gnoss.com/comunidad/nombre_com/recurso/nombre_recurso
                    urlComunidad = urlComunidad.Substring(0, urlComunidad.LastIndexOf("/"));//gnoss.com/comunidad/nombre_com/recurso
                    urlComunidad = urlComunidad.Substring(0, urlComunidad.LastIndexOf("/"));//gnoss.com/comunidad/nombre_com
                    urlComunidad = urlComunidad.Substring(urlComunidad.LastIndexOf("/") + 1);//nombre_com

                    Guid proyID = proyCN.ObtenerProyectoIDPorNombre(urlComunidad);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                    docCN.ObtenerDocumentoPorIDCargarTotal(documentoVincID, dataWrapperDocumentacion, true, false, null);
                    dataWrapperDocumentacion.Merge(docCN.ObtenerVinculacionesRecurso(documentoVincID));
                    docCN.Dispose();

                    if (documentoVincID == Documento.Clave)
                    {
                        mensajeError = UtilIdiomas.GetText("VINCULACIONDOCUMENTACION", "ERRORDOCVINCMISMO");
                    }
                    else if (dataWrapperDocumentacion.ListaDocumento[0].Eliminado || !dataWrapperDocumentacion.ListaDocumento[0].UltimaVersion || dataWrapperDocumentacion.ListaDocumento[0].Borrador || !dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Any(docWebVin => !docWebVin.Eliminado))
                    {
                        mensajeError = UtilIdiomas.GetText("VINCULACIONDOCUMENTACION", "ERRORDOCELIMINADO");
                    }
                    else
                    {
                        if (dataWrapperDocumentacion.ListaDocumentoVincDoc.Any(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.DocumentoVincID.Equals(documentoVincID)))
                        {
                            mensajeError = UtilIdiomas.GetText("VINCULACIONDOCUMENTACION", "ERRORDOCYAVINC");
                        }
                        else
                        {
                            Guid identidadVinculacion = IdentidadActual.Clave;
                            if (identidadVinculacion == Guid.Empty)
                            {
                                throw new ExcepcionGeneral("Hay datos malos en identidad");
                            }

                            //Lo vinculamos:
                            GestorDocumental.VincularDocumentos(pDocumentoID, documentoVincID, identidadVinculacion);

                            ControladorDocumentacion.ActualizarGnossLivePopularidad(proyID, mControladorBase.UsuarioActual.IdentidadID, documentoVincID, AccionLive.VincularRecursoaRecurso, (int)TipoLive.Miembro, (int)TipoLive.Recurso, true, PrioridadLive.Alta, mAvailableServices);
                            try
                            {
                                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                                docCL.InvalidarVinculadosRecursoMVC(documentoVincID, ProyectoVirtual.Clave);
                                docCL.InvalidarVinculadosRecursoMVC(pDocumentoID, ProyectoVirtual.Clave);
                                docCL.Dispose();
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError(ex.Message + "\\n" + ex.StackTrace);
                                throw;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    mensajeError = UtilIdiomas.GetText("VINCULACIONDOCUMENTACION", "ERRORURLMALA", NombreProyectoEcosistema);
                }

                if (!string.IsNullOrEmpty(mensajeError))
                {
                    mLoggingService.GuardarLogError(mensajeError, mlogger);
                }
            }
        }

        /// <summary>
        /// Crea un documento en el modelo Live.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        private void CrearDocumentoEnModeloLive(Documento pDocumento)
        {
            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && !mModelSaveRec.Draft && mTipoDocumento != TiposDocumentacion.Encuesta)
            {
                int tipo;
                switch (mTipoDocumento)
                {
                    case TiposDocumentacion.Debate:
                        tipo = (int)TipoLive.Debate;
                        break;
                    case TiposDocumentacion.Pregunta:
                        tipo = (int)TipoLive.Pregunta;
                        break;
                    default:
                        tipo = (int)TipoLive.Recurso;
                        break;
                }

                string infoExtra = null;

                if (pDocumento.FilaDocumento.ProyectoID == ProyectoAD.MetaProyecto)
                {
                    if (IdentidadActual.IdentidadPersonalMyGNOSS != null)
                    {
                        infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                    }
                    else
                    {
                        infoExtra = IdentidadActual.IdentidadMyGNOSS.PerfilID.ToString();
                    }
                }

                ControladorDocumentacion.ActualizarGnossLive(pDocumento.FilaDocumento.ProyectoID.Value, pDocumento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);
                ControladorDocumentacion.ActualizarGnossLive(pDocumento.FilaDocumento.ProyectoID.Value, mControladorBase.UsuarioActual.IdentidadID, AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);
            }
        }

        /// <summary>
        /// Realiza acciones extra tras crear el documento.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        private void CrearDocumentoAccionesExtra(Documento pDocumento)
        {
            if (EsComunidad && !mModelSaveRec.Draft)
            {
                if (ProyectoSeleccionado != null)
                {
                    ProyectoSeleccionado.FilaProyecto.NumeroRecursos++;
                }

                if (ProyectoSeleccionado.FilaProyecto.TieneTwitter && !pDocumento.FilaDocumentoWebVinBR.PrivadoEditores)
                {
                    ControladorDocumentacion.EnviarEnlaceATwitterDeComunidad(IdentidadActual, pDocumento, ProyectoSeleccionado, BaseURLIdioma, UtilIdiomas, mAvailableServices);
                }
            }

            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);
        }

        /// <summary>
        /// Comprueba la repetición del enlace de un recurso.
        /// </summary>
        /// <returns>Acción respuesta</returns>
        private ActionResult ComprobarRepeticionEnlaceRecurso_SubirRecursoPart2()
        {
            string enlace = "";

            if (mTipoDocumento != TiposDocumentacion.Nota && mTipoDocumento != TiposDocumentacion.Newsletter && mTipoDocumento != TiposDocumentacion.Pregunta && mTipoDocumento != TiposDocumentacion.Encuesta && mTipoDocumento != TiposDocumentacion.Debate)
            {
                enlace = mNombreDocumento;
            }

            string funcionVuelta = "GuardarRecursoRepetido";

            if (mModelSaveRec.Draft)
            {
                funcionVuelta = "GuardarRecursoRepetido_Borrador";
            }

            string titulo = mModelSaveRec.Title;

            if (enlace != "" || titulo != "")
            {
                return ComprobarRepeticionEnlaceRecurso(titulo, enlace, null, funcionVuelta);
            }

            return null;
        }

        /// <summary>
        /// Guarda el documento en la base de datos.
        /// </summary>
        /// <param name="pListaProyectosActuNumRec">Lista con los recursos que se deben actualizar</param>
        private void GuardarEnBD_SubirRecursoPart2(List<Guid> pListaProyectosActuNumRec)
        {
            if (!pListaProyectosActuNumRec.Contains(ProyectoSeleccionado.Clave))
            {
                pListaProyectosActuNumRec.Add(ProyectoSeleccionado.Clave);
            }

            mEntityContext.SaveChanges();

            try
            {
                //Borramos la cache de las comunidades afectadas
                DocumentacionCL documentacionCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                List<Guid> listaProyectosActualYProyectosRelacionados = new List<Guid>();
                listaProyectosActualYProyectosRelacionados.AddRange(pListaProyectosActuNumRec);
                foreach (Guid proyecto in pListaProyectosActuNumRec)
                {
                    int tipoDoc = -1;
                    if (mTipoDocumento == TiposDocumentacion.Debate || mTipoDocumento == TiposDocumentacion.Pregunta)
                    {
                        tipoDoc = (int)mTipoDocumento;
                    }
                    documentacionCL.BorrarPrimerosRecursos(proyecto, tipoDoc);
                }

                //Borramos la cache de recursos relacionados de las comunidades afectadas, asi como todas las relacionadas
                documentacionCL.BorrarRecursosRelacionados(listaProyectosActualYProyectosRelacionados);
                documentacionCL.Dispose();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
        }

        #region AutoGuardado

        /// <summary>
        /// Auto guarda la información del formulario.
        /// </summary>
        /// <param name="sender">El que produce el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private ActionResult AutoGuardadoWiki_SubirRecursoPart2()
        {
            if (mTipoDocumento != TiposDocumentacion.Wiki)
            {
                throw new ExcepcionGeneral("No es Wiki");
            }

            List<Guid> categoriasSeleccionadas = RecogerCategoriasSeleccionadas();

            DocumentoWeb documentoARevisar = DocumentoAutoGuardadoWiki;

            if (HayCambiosSinGuardarParaAutoGuardado(documentoARevisar, categoriasSeleccionadas))
            {
                string mensError = ValidarRecurso(true, categoriasSeleccionadas);

                if (!string.IsNullOrEmpty(mensError))
                {
                    return GnossResultERROR(mensError);
                }

                GuardarRecursoAutoguardado(categoriasSeleccionadas);
            }

            return Content("");
        }

        /// <summary>
        /// Carga los documentos que han sido autoguardados a partir del que se está editando.
        /// </summary>
        private void CargarDocumentosTemporalesAutoguardados_SubirRecursoPart2()
        {
            if (mTipoDocumento == TiposDocumentacion.Wiki)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dataWrapperDocumentacionDocsTemporales = docCN.ObtenerDocumentosTemporalesDeDocumentoPorNombre(mNombreDocumento, mControladorBase.UsuarioActual.IdentidadID, TiposDocumentacion.WikiTemporal);

                if (dataWrapperDocumentacionDocsTemporales.ListaDocumento.Count > 0 && !GestorDocumental.ListaDocumentos.ContainsKey(dataWrapperDocumentacionDocsTemporales.ListaDocumento[0].DocumentoID))
                {
                    docCN.ObtenerDocumentoPorIDCargarTotal(dataWrapperDocumentacionDocsTemporales.ListaDocumento[0].DocumentoID, dataWrapperDocumentacionDocsTemporales, true, true, GestorDocumental.BaseRecursosIDActual);

                    GestorDocumental.DataWrapperDocumentacion.Merge(dataWrapperDocumentacionDocsTemporales);
                    //Es importante crear el documento con la fila del gestor y no con la del dataSet temporal, sino habrá futuros errores:
                    Documento documento = new Documento(GestorDocumental.DataWrapperDocumentacion.ListaDocumento.FirstOrDefault(doc => doc.DocumentoID.Equals(dataWrapperDocumentacionDocsTemporales.ListaDocumento[0].DocumentoID)), GestorDocumental);
                    GestorDocumental.ListaDocumentos.Add(documento.Clave, documento);

                    //Convierto el documento en documentoWeb
                    DocumentoAutoGuardadoWiki = new DocumentoWeb(documento.FilaDocumento, GestorDocumental);
                }
                else if (dataWrapperDocumentacionDocsTemporales.ListaDocumento.Count > 0)
                {
                    DocumentoAutoGuardadoWiki = GestorDocumental.ObtenerDocumentoWeb(GestorDocumental.ListaDocumentos[dataWrapperDocumentacionDocsTemporales.ListaDocumento[0].DocumentoID]);
                }

                docCN.Dispose();
            }
        }

        #endregion

        #endregion
        #region Editar Recurso
        /// <summary>
        /// Método inicial de modificar recurso.
        /// </summary>
        /// <returns>Acción resultante</returns>
        private ActionResult Index_ModificarRecurso()
        {
            mLoggingService.AgregarEntrada("TiemposMVC_Index_ModificarRecurso_Inicio");

            CargarInicial_ModificarRecurso();

            if (!Duplicando)
            {
                ActionResult redireccion = ComprobarRedirecciones_ModificarRecurso();

                if (redireccion != null)
                {
                    return redireccion;
                }
            }

            mEditRecCont.TypePage = EditResourceModel.TypePageEditResource.EditResource;
            ModifyResourceModel modelModRec = new ModifyResourceModel();
            modelModRec.EditResourceModel = mEditRecCont;
            mEditRecCont.ModifyResourceModel = modelModRec;

            modelModRec.DocumentType = (ResourceModel.DocumentType)Documento.TipoDocumentacion;
            mTipoDocumento = Documento.TipoDocumentacion;

            bool recursoTipo2Visible = false;
            bool recursoTipo1Visible = false;
            bool panSeleccionarRecursoVisible = true;
            bool panParte2Visible = true;
            bool recursoTipo0Visible = false;

            if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo || Documento.EsPresentacionIncrustada || (Documento.EsVideoIncrustado && Documento.TipoDocumentacion != TiposDocumentacion.VideoBrightcove && Documento.TipoDocumentacion != TiposDocumentacion.VideoTOP))
            {
                recursoTipo2Visible = true;
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.ReferenciaADoc)
            {
                recursoTipo1Visible = true;
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Nota || Documento.TipoDocumentacion == TiposDocumentacion.Newsletter || Documento.TipoDocumentacion == TiposDocumentacion.Pregunta || Documento.TipoDocumentacion == TiposDocumentacion.Debate || Documento.TipoDocumentacion == TiposDocumentacion.Encuesta)
            {
                panSeleccionarRecursoVisible = false;
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
            {
                //No mostramos el reemplazar:
                panSeleccionarRecursoVisible = false;
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.VideoBrightcove || Documento.TipoDocumentacion == TiposDocumentacion.AudioBrightcove)
            {
                panSeleccionarRecursoVisible = false;
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.VideoTOP || Documento.TipoDocumentacion == TiposDocumentacion.AudioTOP)
            {
                panSeleccionarRecursoVisible = true;

                #region !IsPostBack

                Guid tokenID = Guid.NewGuid();
                mEditRecCont.ModifyResourceModel.TOPTokenID = tokenID.ToString();
                mEditRecCont.ModifyResourceModel.TOPDocType = (short)Documento.TipoDocumentacion;

                string errorSubida = UtilIdiomas.GetText("PERFILBASESUBIR", "ERROR_DVIDEO");
                string errorTipo = UtilIdiomas.GetText("PERFILBASESUBIR", "ERROR_DVIDEO_TIPO");
                string urlRedireccion = BaseURL + "/TOPok.aspx";
                mEditRecCont.ModifyResourceModel.TOPIframeSRC = $"{mConfigService.ObtenerUrlServicioTOP()}?token={tokenID}&urlRedireccion={urlRedireccion}&btSiguiente={UtilIdiomas.GetText("PERFILBASESUBIR", "SIGUIENTE")}&errorSubida={errorSubida}&errorTipo={errorTipo}&edicion=true";

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion docDS = new DataWrapperDocumentacion();

                AD.EntityModel.Models.Documentacion.DocumentoTokenTOP docTokenTOP = new AD.EntityModel.Models.Documentacion.DocumentoTokenTOP();
                docTokenTOP.TokenID = tokenID;
                docTokenTOP.ProyectoID = ProyectoSeleccionado.Clave;
                docTokenTOP.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                docTokenTOP.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                docTokenTOP.FechaCreacion = DateTime.Now;
                docTokenTOP.DocumentoID = Documento.Clave;
                docTokenTOP.Estado = (short)EstadoSubidaVideo.Espera;

                docDS.ListaDocumentoTokenTOP.Add(docTokenTOP);
                docCN.ActualizarDocumentacion();
                docCN.Dispose();

                #endregion
            }
            else
            {
                recursoTipo0Visible = true;
            }

            if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki))
            {
                panSeleccionarRecursoVisible = false;
            }

            //para obtener los Vinculados
            if (Documento.DocumentosVinculados.Count != 0)
            {
                mEditRecCont.ModifyResourceModel.Vinculados = MostrarVinculado();
            }

            mEditRecCont.ModifyResourceModel.EditAttachedAvailable = panSeleccionarRecursoVisible;
            mEditRecCont.ModifyResourceModel.EditPropertiesAvailable = panParte2Visible;
            mEditRecCont.ModifyResourceModel.EditFileAvailable = recursoTipo0Visible;
            mEditRecCont.ModifyResourceModel.EditLocationAvailable = recursoTipo1Visible;
            mEditRecCont.ModifyResourceModel.EditUrlAvailable = recursoTipo2Visible;

            mEditRecCont.ModifyResourceModel.DocumentEditionModel = new DocumentEditionModel();
            mEditRecCont.ModifyResourceModel.DocumentEditionModel.Key = mDocumentoID;

            CargarDatosDocumento_ModificarRecurso(mEditRecCont.ModifyResourceModel.DocumentEditionModel);

            #region Recuperar autoguardo

            if (Documento.TipoDocumentacion == TiposDocumentacion.Wiki && DocumentoAutoGuardadoWiki != null && RequestParams("recuperarAutoGuardado") == null)
            {
                modelModRec.DocumentEditionModel.AutosaveAvailable = true;
                modelModRec.DocumentEditionModel.UrlRecoverAutosave = ViewBag.UrlPagina + "?recuperarAutoGuardado=true";
            }

            #endregion

            #region Selector categoria del tesauro

            PrepararTesauro_ModificarRecurso();

            #endregion

            #region Autoría documento

            mEditRecCont.ModifyResourceModel.CopyrightAvailable = !(Documento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Pregunta) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Debate) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Encuesta));

            #endregion

            mEditRecCont.ModifyResourceModel.CreatingVersion = (RequestParams("version") != null && RequestParams("version") == "true");
            mEditRecCont.ModifyResourceModel.IsImprovement = ComprobarSiSeEstaEditandoUnaMejora(mDocumentoID);
			mEditRecCont.ModifyResourceModel.DocumentEditionModel.Draft = Documento.FilaDocumento.Borrador;

            #region Editores

            UsersSelectorModel selecEditModel = new UsersSelectorModel();
            mEditRecCont.ModifyResourceModel.UsersSelectorEditionModel = selecEditModel;

            UsersSelectorModel selecLectModel = new UsersSelectorModel();
            mEditRecCont.ModifyResourceModel.UsersSelectorReadingModel = selecLectModel;

            CargarEditoresRecurso(mEditRecCont.ModifyResourceModel.UsersSelectorEditionModel, mEditRecCont.ModifyResourceModel.UsersSelectorReadingModel);

            #endregion

            #region Protección de documentos

            if (Documento.PermiteVersiones && EsComunidad && Documento.TipoDocumentacion != TiposDocumentacion.Pregunta && (Documento.TipoDocumentacion != TiposDocumentacion.Wiki || EsIdentidadActualSupervisorProyecto))
            {
                mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = true;

                mEditRecCont.ModifyResourceModel.DocumentEditionModel.Protected = Documento.FilaDocumento.Protegido;
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.ModificationProtectionAvailable = true;

                if (Documento.FilaDocumento.Protegido)
                {
                    string[] parametros = new string[2];

                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                    AD.EntityModel.Models.PersonaDS.Persona filaPersona = null;
                    if (Documento.FilaDocumento.IdentidadProteccionID.HasValue)
                    {
                        filaPersona = personaCN.ObtenerPersonaPorIdentidadCargaLigera(Documento.FilaDocumento.IdentidadProteccionID.Value);
                    }

                    if (filaPersona != null)
                    {
                        parametros[0] = filaPersona.Nombre + " " + filaPersona.Apellidos;
                    }

                    parametros[1] = "";
                    if (Documento.FilaDocumento.FechaProteccion.HasValue)
                    {
                        parametros[1] = Documento.FilaDocumento.FechaProteccion.Value.ToShortDateString();
                    }

                    personaCN.Dispose();

                    mEditRecCont.ModifyResourceModel.DocumentEditionModel.ProtectionInfo = new KeyValuePair<string, string>(parametros[0], parametros[1]);

                    if (Documento.TipoDocumentacion != TiposDocumentacion.Wiki || !EsIdentidadActualSupervisorProyecto)
                    {
                        mEditRecCont.ModifyResourceModel.DocumentEditionModel.ModificationProtectionAvailable = ((Documento.FilaDocumento.IdentidadProteccionID.HasValue && Documento.FilaDocumento.IdentidadProteccionID.Value.Equals(mControladorBase.UsuarioActual.IdentidadID)));
                    }
                }
            }

            #endregion

            #region Desconocido

            mEditRecCont.ModifyResourceModel.UrlCancelButton = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Documento, IdentidadOrganizacion != null);

            if ((Documento.TipoDocumentacion == TiposDocumentacion.FicheroServidor || Documento.TipoDocumentacion == TiposDocumentacion.Imagen) && !Documento.EsPresentacionIncrustada && !Documento.Enlace.Equals(string.Empty))
            {
                ControladorDocumentacion controlador = new ControladorDocumentacion(GestorDocumental, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.UrlDownloadAttached = controlador.CrearEnlaceDocumento(Documento, IdentidadOrganizacion, UsuarioActual);
            }

            #endregion

            #region Visibilidad botones

            mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = (Documento.TipoDocumentacion != TiposDocumentacion.Newsletter && Documento.TipoDocumentacion != TiposDocumentacion.Encuesta && Documento.TipoDocumentacion != TiposDocumentacion.Debate);

            if (Documento.TipoDocumentacion == TiposDocumentacion.Newsletter)
            {
                mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = false;
            }

            #endregion

            #region Addin de Gnoss para Office

            string nombreDocTemporal = null;
            Guid? idDocTemporal = null;

            //A entrado a esta página mediante el addin de Gnoss para Office, compruebo que se ha subido bien el documento.
            string cookieTemporal = Request.Cookies["nombreDocTemporal_" + mControladorBase.DominoAplicacion];
            if ((RequestParams("idTemp") != null) && (cookieTemporal != null) && (!string.IsNullOrEmpty(cookieTemporal)))
            {
                try
                {
                    nombreDocTemporal = cookieTemporal;
                    idDocTemporal = new Guid(RequestParams("idTemp"));
                }
                catch (Exception ex)
                {
                    GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                }
            }

            //Ha entrado a esta página mediante el addin de Gnoss para Office, compruebo que se ha subido bien el documento.
            if ((idDocTemporal != null) && (nombreDocTemporal != null))
            {
                Stopwatch sw = null;
                try
                {
                    mEditRecCont.ModifyResourceModel.EditFileAvailable = false;
                    mEditRecCont.ModifyResourceModel.UploadedAttachedNameByAddin = "(" + nombreDocTemporal + ")";
                    GestionDocumental gestorDocumentalWS = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                    gestorDocumentalWS.Url = UrlServicioWebDocumentacion;
                    sw = LoggingService.IniciarRelojTelemetria();
                    byte[] buffer1 = gestorDocumentalWS.ObtenerRecursoTemporal(idDocTemporal.Value, Path.GetExtension(nombreDocTemporal));
                    mLoggingService.AgregarEntradaDependencia("Obtener recurso temporal", false, "Index_MofificarRecurso", sw, true);

                    Session.Set("nombreFuReemplazado", nombreDocTemporal);
                    Session.Set("fuReemplazado", buffer1);
                }
                catch (Exception ex)
                {
                    mLoggingService.AgregarEntradaDependencia("Error al adjuntar archivo al gestor documental", false, "Index_MofificarRecurso", sw, false);
                    GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                }
            }

            #endregion

            #region Visibilidad panel Privacidad y Politicas de seguridad

            mEditRecCont.ModifyResourceModel.ShareAvailable = (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto || ParametrosGeneralesRow.CompartirRecursosPermitido);

            #endregion

            #region Propiedad intelectual

            mEditRecCont.ModifyResourceModel.DocumentEditionModel.AllowsLicense = Documento.PermiteLicencia;

            if (Documento.PermiteLicencia)
            {
                PrepararLicencia();
            }

            #endregion

            //Ponemos el título de la página
            if (Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
            {
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "EDITAPREGUNTATITULO");
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Encuesta)
            {
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "EDITAENCUESTATITULO");
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Debate)
            {
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "EDITADEBATETITULO");
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Newsletter)
            {
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "EDITANEWSLETTERTITULO");
            }
            else
            {
                TituloPagina = UtilIdiomas.GetText("PERFILBASE", "EDITARECURSOTITULO");
            }

            #region encuestas

            if (Documento.TipoDocumentacion == TiposDocumentacion.Encuesta)
            {
                mEditRecCont.ModifyResourceModel.EditPollAnswersAvailable = true;
                mEditRecCont.ModifyResourceModel.PollAnswersModel = new PollAnswersModel();

                //En la porimera carga carga las respuestas de BD 
                foreach (RespuestaRecurso respuesta in Documento.ListaRespuestas.Values.OrderBy(res => res.FilaRespuesta.Orden))
                {
                    mEditRecCont.ModifyResourceModel.PollAnswersModel.Answers.Add(respuesta.FilaRespuesta.Descripcion);
                }
            }

            #endregion

            #region tipo de documento

            mEditRecCont.ModifyResourceModel.EditTitleAvailable = true;
            mEditRecCont.ModifyResourceModel.EditDescriptionAvailable = true;

            if (Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
            {
                TipoPagina = TiposPagina.Preguntas;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASE", "EDITAPREGUNTATITULO");
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Encuesta)
            {
                if (EsComunidad)
                {
                    TipoPagina = TiposPagina.Encuestas;
                    mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASERECURSOEDITAR", "EDITARENCUESTA");
                }

                mEditRecCont.ModifyResourceModel.EditDescriptionAvailable = false;
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Debate)
            {
                if (EsComunidad)
                {
                    TipoPagina = TiposPagina.Debates;
                    mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASE", "EDITADEBATETITULO");
                }
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Newsletter)
            {
                TipoPagina = TiposPagina.BaseRecursos;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASE", "EDITANEWSLETTERTITULO");

                #region En la primera carga igualamos la temporal a la actual

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentoNewsletterPorDocumentoID(mDocumentoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

                if (gestorDoc.DataWrapperDocumentacion.ListaDocumentoNewsLetter.Count == 1)
                {
                    gestorDoc.DataWrapperDocumentacion.ListaDocumentoNewsLetter[0].NewsletterTemporal = gestorDoc.DataWrapperDocumentacion.ListaDocumentoNewsLetter[0].Newsletter;
                }

                docCN.ActualizarDocumentacion();
                docCN.Dispose();

                #endregion
            }
            else
            {
                if (EsComunidad)
                {
                    TipoPagina = TiposPagina.BaseRecursos;
                    mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASE", "EDITARECURSOTITULO");
                }
            }

            #endregion
            return null;
        }

        /// <summary>
        /// Acción de subir archivo adjunto.
        /// </summary>
        /// <param name="pModel">Modelo de subir archivo adjunto</param>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult AdjuntarArchivo(AttachmentModel pPaginaModel)
        {
            try
            {
                ActionResult redireccion = null;

                if (EditandoRecurso)
                {
                    CargarInicial_ModificarRecurso();
                    redireccion = ComprobarRedirecciones_ModificarRecurso();
                }
                else if (EditandoFormSem || CreandoFormSem || CargaMasivaFormSem)
                {
                    CargarInicial_ModificarRecursoSemantico();
                    redireccion = ComprobarRedirecciones_ModificarRecursoSemantico();

                    if (redireccion == null)
                    {
                        if (!CargaMasivaFormSem)
                        {
                            return GuardarArchivo_ModificarRecursoSemantico(pPaginaModel);
                        }
                        else
                        {
                            return GuardarArchivo_CargaMasiva_ModificarRecursoSemantico(pPaginaModel);
                        }
                    }
                }
                else
                {
                    RecogerParametros_SubirRecursoPart2();
                    CargarInicial_SubirRecursoPart2();
                    redireccion = ComprobarRedirecciones_SubirRecursoPart2();
                }

                if (redireccion != null)
                {
                    return redireccion;
                }

                if (!pPaginaModel.IsNewsLetter)
                {
                    return GuardarArchivoTemporal_ModificarRecurso(pPaginaModel);
                }
                else
                {
                    return SubirArchivoNewsLetter_ModificarRecurso(pPaginaModel);
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pPaginaModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pPaginaModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion AdjuntarArchivo");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
                throw;
            }
        }

        /// <summary>
        /// Acción de eliminar el autoguardado de un recurso.
        /// </summary>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult EliminarAutoGuardado()
        {
            try
            {
                ActionResult redireccion = null;

                if (EditandoRecurso)
                {
                    CargarInicial_ModificarRecurso();
                    redireccion = ComprobarRedirecciones_ModificarRecurso();
                }
                else
                {
                    RecogerParametros_SubirRecursoPart2();
                    CargarInicial_SubirRecursoPart2();
                    redireccion = ComprobarRedirecciones_SubirRecursoPart2();
                }

                if (redireccion != null)
                {
                    return redireccion;
                }

                return EliminarAutoGuardadoWiki_ModificarRecurso();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
            }
        }

        public bool ComprobarPermisoEdicionEstado(Guid pEstadoID, Guid pDocumentoID)
        {
			FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
			return flujosCN.ComprobarIdentidadTienePermisoEdicionEnEstado(pEstadoID, IdentidadActual.Clave, pDocumentoID);
        }

        public bool ComprobarPermisoGeneralEditarRecurso(TiposDocumentacion pTipoDocumento)
        {
            switch (Documento.TipoDocumentacion)
            {
                case TiposDocumentacion.Hipervinculo:
                    return ComprobarPermisosAccionRecurso(PermisoRecursos.EditarRecursoTipoEnlace);
                case TiposDocumentacion.Nota:
                    return ComprobarPermisosAccionRecurso(PermisoRecursos.EditarNota);
                case TiposDocumentacion.FicheroServidor:
                    return ComprobarPermisosAccionRecurso(PermisoRecursos.EditarRecursoTipoAdjunto);
                case TiposDocumentacion.ReferenciaADoc:
                    return ComprobarPermisosAccionRecurso(PermisoRecursos.EditarRecursoTipoReferenciaADocumentoFisico);
                case TiposDocumentacion.Pregunta:
                    return ComprobarPermisosAccionRecurso(PermisoRecursos.EditarPregunta);
                case TiposDocumentacion.Encuesta:
                    return ComprobarPermisosAccionRecurso(PermisoRecursos.EditarEncuesta);
                case TiposDocumentacion.Debate:
                    return ComprobarPermisosAccionRecurso(PermisoRecursos.EditarDebate);
                case TiposDocumentacion.Semantico:
                    return ComprobarPermisosAccionRecursoSemantico(TipoPermisoRecursosSemanticos.Modificar);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Comprueba si hay que hacer alguna redirección a otra página.
        /// </summary>
        /// <returns>Resultado de la acción</returns>
        private ActionResult ComprobarRedirecciones_ModificarRecurso()
        {
            if (Documento.Estado.HasValue && ComprobarPermisoEdicionEstado(Documento.Estado.Value, Documento.VersionOriginalID))
            {
                return null;
            }

            bool permisoEditar = ComprobarPermisoGeneralEditarRecurso(Documento.TipoDocumentacion);

            if (!string.IsNullOrEmpty(RequestParams("organizacion")) && (IdentidadActual.OrganizacionID == null || IdentidadActual.OrganizacionID != IdentidadOrganizacion.OrganizacionID.Value))
            {
                return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosInicio(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, IdentidadOrganizacion != null));
            }

            if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki) && Documento.FilaDocumento.Protegido && !EsIdentidadActualSupervisorProyecto)
            {
                return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Documento, (IdentidadOrganizacion != null)));
            }

            if (!Documento.FilaDocumento.UltimaVersion)
            {
                return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Documento, (IdentidadOrganizacion != null)));
            }

            if (!ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID) && !Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadActual.IdentidadOrganizacion, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsAdministrador(IdentidadActual)) && !permisoEditar)
            {
                return Redirect(BaseURLIdioma);
            }

            //no se puede editar un recuso encuesta
            if (Documento.TipoDocumentacion == TiposDocumentacion.Encuesta && !Documento.EsBorrador && Documento.FilaDocumento.DocumentoRespuestaVoto.Count > 0)
            {
                return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Documento, (IdentidadOrganizacion != null)));
            }

            if (mControladorBase.UsuarioActual.EsIdentidadInvitada && (!FormSemVirtual || !PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.permitirUsuNoLogueado.ToString())))
            {
                return Redirect(BaseURLIdioma);
            }

            if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && mOntologiaID == Guid.Empty)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                string nombreEntVinc = docCN.ObtenerNombreElementoVinculadoDocumento(Documento.ElementoVinculadoID);
                docCN.Dispose();
                bool docVirtual = false;

                if (!string.IsNullOrEmpty(nombreEntVinc))
                {
                    mPropiedadesTextoOntologia = UtilCadenas.ObtenerPropiedadesDeTexto(nombreEntVinc);
                    docVirtual = (PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.urlservicio.ToString()));
                }

                return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosVerDocumentoCreado(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Documento.Clave, Documento.ElementoVinculadoID.ToString(), true, (IdentidadOrganizacion != null), docVirtual));
            }

            return null;
        }

        #region Métodos callback

        /// <summary>
        /// Médodo para eliminar el autoguardado de un recurso wiki.
        /// </summary>
        /// <param name="pArgs">Argumentos</param>
        /// <returns>Respuesta del callback</returns>
        private ActionResult EliminarAutoGuardadoWiki_ModificarRecurso()
        {
            try
            {
                if (DocumentoAutoGuardadoWiki != null)
                {
                    GestorDocumental.EliminarDocumentoWeb(DocumentoAutoGuardadoWiki);
                    Guardar_ModificarRecurso(new List<Guid>());
                    return Content("OK");
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex.ToString());
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
            }

            return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
        }

        /// <summary>
        /// Médodo para subir un archivo de newLetter modificar recurso.
        /// </summary>
        /// <param name="pModel">Modelo para subir un adjunto</param>
        /// <returns>Respuesta del callback</returns>
        private ActionResult SubirArchivoNewsLetter_ModificarRecurso(AttachmentModel pModel)
        {
            if (pModel.File != null)
            {
                try
                {
                    #region leer Archivo

                    byte[] buffer1;

                    using (BinaryReader reader1 = new BinaryReader(pModel.File.OpenReadStream()))
                    {
                        buffer1 = reader1.ReadBytes((int)pModel.File.Length);
                        reader1.Close();
                    }

                    MemoryStream ms = new MemoryStream(buffer1);

                    StreamReader objReader = new StreamReader(ms);

                    string sLine = "";
                    StringBuilder txtNewsletter = new StringBuilder();

                    while (sLine != null)
                    {
                        sLine = objReader.ReadLine();
                        if (sLine != null)
                            txtNewsletter.Append(sLine);
                    }

                    objReader.Close();
                    objReader.Dispose();

                    #endregion

                    #region Guardar Newsletter

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                    GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentoNewsletterPorDocumentoID(mDocumentoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

                    if (gestorDoc.DataWrapperDocumentacion.ListaDocumentoNewsLetter.Count == 1)
                    {
                        gestorDoc.DataWrapperDocumentacion.ListaDocumentoNewsLetter.FirstOrDefault().NewsletterTemporal = txtNewsletter.ToString();
                    }
                    else
                    {
                        gestorDoc.AgregarNewsletterDocumento(mDocumentoID, "", txtNewsletter.ToString());
                    }

                    docCN.ActualizarDocumentacion();
                    gestorDoc.Dispose();

                    #endregion

                    return Content("OK");
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    //Se han producido errores al guardar el documento en el servidor
                    return GnossResultERROR();
                }
            }

            return GnossResultERROR();
        }

        /// <summary>
        /// Médodo para subir un archivo de modificar recurso.
        /// </summary>
        /// <param name="pModel">Modelo de subir archivo adjunto</param>
        /// <returns>Respuesta del callback</returns>
        private ActionResult GuardarArchivoTemporal_ModificarRecurso(AttachmentModel pModel)
        {
            if (pModel.File != null)
            {
                try
                {
                    string nombreCompleto = pModel.FileName;
                    string extension = Path.GetExtension(nombreCompleto).ToLower();
                    string nombre = Path.GetFileNameWithoutExtension(nombreCompleto).ToLower();

                    byte[] buffer1;

                    using (BinaryReader reader1 = new BinaryReader(pModel.File.OpenReadStream()))
                    {
                        buffer1 = reader1.ReadBytes((int)pModel.File.Length);
                        reader1.Close();
                    }

                    int num = GuardarArchivoTemporal_ModificarRecurso_GestorDocumental(buffer1, nombre, extension);

                    if (num != 1)
                    {
                        return GnossResultERROR();
                    }

                    return Content("OK");
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    //Se han producido errores al guardar el documento en el servidor
                    return GnossResultERROR();
                }
            }

            return GnossResultERROR();
        }
        private HashSet<string> ModificarTriplesAdjuntosRecursoSemantico(DocumentEditionModel pModel, Identidad pIdentidadOrganizacion, bool pMasterComunidad)
        {
            HashSet<string> rutasAdjuntos = new HashSet<string>();
            if (EditandoFormSem && pModel != null)
            {
                string pattern = $@">([^<]*({UtilArchivos.ContentImagenesSemanticas}|{UtilArchivos.ContentDocumentosSem}|{UtilArchivos.ContentDocLinks}|{UtilArchivos.ContentVideosSemanticos})[^<]*)<";
                Regex regex = new Regex(pattern);
                MatchCollection resultados = regex.Matches(pModel.RdfValue);

                foreach (Match resultado in resultados)
                {
                    string rutaArchivo = resultado.Groups[1].Value;

                    string nuevaRuta = ControladorDocumentacion.ReemplazarRutaAdjuntoSemantico(rutaArchivo, pModel.Key, DocumentoVersionID);

                    rutasAdjuntos.Add(Path.GetDirectoryName(rutaArchivo));

                    pModel.RdfValue = pModel.RdfValue.Replace(rutaArchivo, nuevaRuta);
                }
            }
            return rutasAdjuntos;
        }

        private int GuardarArchivoTemporal_ModificarRecurso_GestorDocumental(byte[] pFichero, string pNombre, string pExtension)
        {
            //Subimos el fichero al servidor
            GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
            gd.Url = UrlServicioWebDocumentacion;

            string idAuxGestorDocumental = "";

            idAuxGestorDocumental = gd.AdjuntarDocumentoWebTemporal(pFichero, IdentidadActual.Clave, pNombre, pExtension);

            return idAuxGestorDocumental.ToUpper().Equals("ERROR") ? 0 : 1;
        }

        /// <summary>
        /// Médodo para guardar un recurso.
        /// </summary>
        /// <param name="pModel">Argumentos</param>
        /// <returns>Respuesta acción</returns>
        private ActionResult GuardarRecurso_ModificarRecurso(DocumentEditionModel pModel)
        {
            mModelSaveRec = pModel;

            if (pModel.AutoSave.HasValue && pModel.AutoSave.Value)
            {
                return AutoGuardadoWiki_ModificarRecurso();
            }

            #region Versiones por concurrencia

            Documento nuevoDoc = null;

            if (pModel.RedirectionByConcurrency.HasValue && pModel.RedirectionByConcurrency.Value)
            {
                ActionResult redireccion = null;
                try
                {
                    redireccion = GuardarRecurso_ModificarRecurso(false, Documento, false);
                }
                catch (Exception ex)
                {
                    GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    redireccion = GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
                }
                finally
                {
                    DocumentacionCN docCNFin = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    docCNFin.FinalizarEdicionRecurso(Documento.Clave);
                    docCNFin.Dispose();
                }

                return redireccion;
            }

            if (pModel.CreateVersionByConcurrency.HasValue && pModel.CreateVersionByConcurrency.Value)
            {
                #region concurrencia sobreescribir

                nuevoDoc = CrearVersionDocumento_ModificarRecurso();

                DocumentacionCN docAuxCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion mDocDWPreGuardado = docAuxCN.ObtenerVersionesDocumentoPorID(Documento.Clave);
                List<Guid> listaDocIDs = new List<Guid>();

                foreach (AD.EntityModel.Models.Documentacion.VersionDocumento filaVersionDoc in mDocDWPreGuardado.ListaVersionDocumento.Where(item => !listaDocIDs.Contains(item.DocumentoID)))
                {
                    listaDocIDs.Add(filaVersionDoc.DocumentoID);
                }

                if (listaDocIDs.Count > 0)
                {
                    mDocDWPreGuardado.Merge(docAuxCN.ObtenerDocumentosPorID(listaDocIDs));
                }

                foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in mDocDWPreGuardado.ListaDocumento)
                {
                    if (!filaDoc.DocumentoID.Equals(nuevoDoc.Clave))
                    {
                        filaDoc.UltimaVersion = false;
                    }
                }

                //La última versión que hemos creado no tiene la numeración correcta, revisamos como debe estar
                int ultimaVersion = 0;
                if (mDocDWPreGuardado.ListaVersionDocumento.Count > 0)
                {
                    ultimaVersion = mDocDWPreGuardado.ListaVersionDocumento.OrderByDescending(version => version.Version).FirstOrDefault().Version;

                }
                GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.OrderByDescending(versionDoc => versionDoc.Version).First().Version = ultimaVersion + 1;

                docAuxCN.Dispose();

                ActionResult redireccion = null;
                try
                {
                    redireccion = GuardarRecurso_ModificarRecurso(true, nuevoDoc, true);
                }
                catch (Exception ex)
                {
                    GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    redireccion = GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
                }
                finally
                {
                    DocumentacionCN docCNFin = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    docCNFin.FinalizarEdicionRecurso(Documento.Clave);
                    docCNFin.Dispose();
                }

                return redireccion;

                #endregion
            }

            #endregion

            if (!pModel.SkipRepeat)
            {
                ActionResult htmlPanelRepe = ComprobarRepeticionEnlaceRecurso_ModificarRecurso();

                if (htmlPanelRepe != null)
                {
                    return htmlPanelRepe;
                }
            }

            bool creandoVersion = false;

            if (RequestParams("version") != null)
            {
                bool.TryParse(RequestParams("version").ToLower(), out creandoVersion);
            }

            //Compruebo si ha habido un error de concurrencia
            bool versionCreadaPorConcurrencia = false;
            Guid? nuevaVersionDocID = null;

            ControladorDocumentacion contrDoc = new ControladorDocumentacion(Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
            ErroresConcurrencia errorConcurrencia = contrDoc.ComprobarConcurrenciaDocumento(Documento, IdentidadActual, out nuevaVersionDocID);

            if (creandoVersion && errorConcurrencia == ErroresConcurrencia.NoConcurrencia)
            {
                if (Documento.EsFicheroDigital && string.IsNullOrEmpty(pModel.TemporalFileName) && Session.Get("nombreFuReemplazado") == null)
                {
                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DENLACE"));
                }
                else
                {
                    nuevoDoc = CrearVersionDocumento_ModificarRecurso();
                }
            }

            if (errorConcurrencia == ErroresConcurrencia.NoConcurrencia)
            {
                ActionResult redireccion = null;
                try
                {
                    redireccion = GuardarRecurso_ModificarRecurso(creandoVersion, nuevoDoc, versionCreadaPorConcurrencia);
                }
                catch (Exception ex)
                {
                    GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    redireccion = GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
                }

                return redireccion;
            }
            else if (errorConcurrencia != ErroresConcurrencia.NoConcurrencia)
            {
                DocumentacionCN DocumentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                DocumentacionCN.ObtenerHistorialDocumentoPorID(Documento.Clave, dataWrapperDocumentacion);
                DocumentacionCN.Dispose();

                Guid identidadUltimaModificacion = Guid.Empty;

                Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.HistorialDocumento historialDocumento = dataWrapperDocumentacion.ListaHistorialDocumento.OrderByDescending(historial => historial.Fecha).FirstOrDefault();

                if (historialDocumento != null)
                {
                    identidadUltimaModificacion = historialDocumento.IdentidadID;
                }

                return MostrarPanelConcurrencia_ModificarRecurso(identidadUltimaModificacion);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Guarda un recurso
        /// </summary>
        /// <param name="pCreandoVersion">TRUE si se está creando una nueva versión, FALSE en caso contrario</param>
        /// <param name="pNuevoDoc">Nuevo documento creado</param>
        /// <param name="pVersionCreadaPorConcurrencia">Indica si la versión ha sido creada por concurrencia o no</param>
        /// <returns>Acción resultado</returns>
        private ActionResult GuardarRecurso_ModificarRecurso(bool pCreandoVersion, Documento pNuevoDoc, bool pVersionCreadaPorConcurrencia)
        {
            List<Guid> listaPerfiles = new List<Guid>();
            listaPerfiles.AddRange(Documento.ListaPerfilesEditores.Keys);
            List<Guid> categoriasSeleccionadas = RecogerCategoriasSeleccionadas();
            bool esMejora = ComprobarSiSeEstaEditandoUnaMejora(pNuevoDoc.Clave);

            Guid docAntiguoID = mDocumentoID;
            bool borrador = mModelSaveRec.Draft;

            //Si hay varios editores y se ha reemplazado el documento, compruebo si el usuario quiere crear nueva versión
            if (pNuevoDoc == null && mModelSaveRec.CreateVersionByReplaceAttachment && !Documento.FilaDocumento.Protegido)
            {
                pNuevoDoc = CrearVersionDocumento_ModificarRecurso();
            }

            int numVisitasAnteriorVersion = 0;
            if (pCreandoVersion)
            {
                numVisitasAnteriorVersion = Documento.FilaDocumentoWebVinBRExtra.NumeroConsultas;
            }

            if (pNuevoDoc != null)
            {
                mDocumentoID = pNuevoDoc.Clave;

                if (!GestorDocumental.ListaDocumentos.ContainsKey(mDocumentoID))
                {
                    GestorDocumental.ListaDocumentos.Add(mDocumentoID, pNuevoDoc);
                }

            }

            if (pCreandoVersion)
            {
                Documento.FilaDocumentoWebVinBRExtra.NumeroDescargas = numVisitasAnteriorVersion;
                Documento.FilaDocumentoWebVinBRExtra.NumeroConsultas = numVisitasAnteriorVersion;
            }

            string mensError = ValidarRecurso(borrador, categoriasSeleccionadas);

            if (!string.IsNullOrEmpty(mensError))
            {
                return GnossResultERROR(mensError);
            }


            try
            {
                KeyValuePair<bool, bool> parCapturaWeb_GeneraImgMini = ExtraerReemplazoArchivo_ModificarRecurso();
                string enlaceNuevoAux;
                ActionResult redireccion = FuncionalidadSharepointCrearVersion(mDocumentoID, Documento.FilaDocumento.Enlace, out enlaceNuevoAux);
                if (redireccion != null)
                {
                    return redireccion;
                }
                ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string sharepointConfigurado = paramCN.ObtenerParametroAplicacion("SharepointClientID");
                string oneDrivePermitido = paramCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (!string.IsNullOrEmpty(sharepointConfigurado) && UtilCadenas.EsEnlaceSharepoint(enlaceNuevoAux, oneDrivePermitido))
                {
                    Documento.Enlace = enlaceNuevoAux;
                    Documento.FilaDocumento.Enlace = enlaceNuevoAux;
                }

                if (mErrorDocumento != null)
                {
                    return GnossResultERROR(mErrorDocumento);
                }

                mCambioDeBorradoAPublicado = (Documento.FilaDocumento.Borrador && !borrador);
                bool privacidadCambiada = false;
                List<Guid> listaEditoresEliminados;
                List<Guid> listaGruposEditoresEliminados;

                GuardarVinculados(Documento.Clave, pCreandoVersion);
                GuardarRecursoModeloAcido(categoriasSeleccionadas, pCreandoVersion, pVersionCreadaPorConcurrencia, out privacidadCambiada, out listaEditoresEliminados, out listaGruposEditoresEliminados);

                if (!esMejora)
                {
					GuardarRecursoModeloLive(privacidadCambiada, pCreandoVersion, mCambioDeBorradoAPublicado, docAntiguoID, Documento, listaEditoresEliminados, listaGruposEditoresEliminados);
					GuardarRecursoModeloBase(pCreandoVersion, mCambioDeBorradoAPublicado, docAntiguoID, Documento);
				}
                

                if (parCapturaWeb_GeneraImgMini.Key)
                {
                    ControladorDocumentacion.CapturarImagenWeb(Documento.Clave, false, PrioridadColaDocumento.Alta, mAvailableServices);
                }

                if (parCapturaWeb_GeneraImgMini.Value)
                {
                    ControladorDocumentacion.CapturarImagenWeb(Documento.Clave, false, PrioridadColaDocumento.Alta, mAvailableServices);
                }

                if (pCreandoVersion)
                {
                    ControladorDocumentacion.BorrarCacheControlFichaRecursos(docAntiguoID);
                }
                else
                {
                    ControladorDocumentacion.BorrarCacheControlFichaRecursos(Documento.Clave);
                }

                //TO-DO : Solo se han añadido acciones externas del ecosistema para la edición de un recurso, hay que añadir el desarrollo para el resto de acciones (crear y eliminar)
                // y hay que añadir las acciones externas del proyecto
                ControladorDocumentacion.AccionEnServicioExternoEcosistema(TipoAccionExterna.EditarRecurso, GestorParametroAplicacion, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, Documento.Clave, Documento.Titulo, Documento.Descripcion, Documento.TipoDocumentacion);

                ControladorDocumentacion.InsertarEnColaProcesarFicherosRecursosModificadosOEliminados(Documento.Clave, TipoEventoProcesarFicherosRecursos.Modificado, mAvailableServices);

                string urlRedirect = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Documento, (IdentidadOrganizacion != null)).Replace(Documento.Clave.ToString(), Documento.VersionOriginalID.ToString());
                if (esMejora)
                {
                    urlRedirect = $"{mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, UtilCadenas.EliminarCaracteresUrlSem(Documento.Titulo), Documento.VersionOriginalID, Documento.ElementoVinculadoID, false)}/{pNuevoDoc.Clave}";
                }
                if (pCreandoVersion)
                {
                    urlRedirect += "?versioned";
                }
                else
                {
                    urlRedirect += "?modified";
                }

                return GnossResultUrl(urlRedirect);
            }
            catch (HttpException)
            {
                throw;
            }
            catch (Exception ex)
            {
                GuardarLogError($" ERROR: {ex.Message}\r\nStackTrace: {ex.StackTrace}");
            }

            return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
        }


        private void GuardarVinculados(Guid pDocumentoID, bool pCreandoVersion)
        {
            if (!pCreandoVersion)
            {
                if (!string.IsNullOrEmpty(RequestParams("Vinculado")))
                {
                    if (!RequestParams("Vinculado").Equals("SinModificacion"))
                    {
                        List<string> listaVinculados = UtilCadenas.SepararTexto(RequestParams("Vinculado"));

                        List<Guid> listaVinculadosBD = VinculadosEnBD(pDocumentoID);
                        if (listaVinculadosBD != null)
                        {
                            listaVinculados = BorrarVinculadosBD(listaVinculados, listaVinculadosBD);
                        }
                        foreach (string vinculado in listaVinculados)
                        {
                            AnyadirVinculado(vinculado, pDocumentoID);
                        }
                    }
                }
                else
                {
                    BorrarVinculados(pDocumentoID);
                }
            }
        }

        private List<string> BorrarVinculadosBD(List<string> plistaVinculados, List<Guid> plistaVinculadosBD)
        {
            List<string> listaVinculadosFinal = new List<string>();
            foreach (string VinculadoFormulario in plistaVinculados)
            {
                string id = VinculadoFormulario.Substring(VinculadoFormulario.LastIndexOf("/") + 1);
                Guid documentoVincID = new Guid(id);

                if (plistaVinculadosBD.Contains(documentoVincID))
                {
                    plistaVinculadosBD.Remove(documentoVincID);
                }
                else
                {
                    listaVinculadosFinal.Add(VinculadoFormulario);
                }
            }
            foreach (Guid VinculadoBD in plistaVinculadosBD)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                docCN.ObtenerDocumentoPorIDCargarTotal(VinculadoBD, GestorDocumental.DataWrapperDocumentacion, true, false, null);
                GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerVinculacionesRecurso(VinculadoBD));
                docCN.Dispose();

                GestorDocumental.DesVincularDocumentos(VinculadoBD, Documento.Clave);
            }

            return listaVinculadosFinal;
        }

        /// <summary>
        /// Guarda el recurso en el modelo ácido de GNOSS.
        /// </summary>
        /// <param name="pCategoriasSeleccionadas">Categorías del tesauro seleccionadas</param>
        /// <param name="pCreandoVersion">Indica si se está creando versión</param>
        /// <param name="pVersionCreadaPorConcurrencia">Indica si se ha crado una versión por concurrencia</param>
        /// <param name="pPrivacidadCambiada">Indica si la privacidad del recurso ha cambiado en esta edición</param>
        private void GuardarRecursoModeloAcido(List<Guid> pCategoriasSeleccionadas, bool pCreandoVersion, bool pVersionCreadaPorConcurrencia, out bool pPrivacidadCambiada, out List<Guid> pListaEditoresEliminados, out List<Guid> pListaGruposEditoresEliminados)
        {
            bool debePublicarEnTwitter = false;
            bool borrador = mModelSaveRec.Draft;

            if (!string.IsNullOrEmpty(Documento.Enlace) && mEntityContext.Entry(Documento.FilaDocumento).State.Equals(DataRowState.Modified) && !(mEntityContext.Entry(Documento.FilaDocumento).OriginalValues["Enlace"] is DBNull) && !Documento.Enlace.Equals(mEntityContext.Entry(Documento.FilaDocumento).OriginalValues["Enlace"]))
            {
                // Ha cambiado el enlace al documento, compruebo si ha dejado de ser una presentación incrustada o un vído incrustado
                string enlaceOriginal = (string)mEntityContext.Entry(Documento).OriginalValues["Enlace"];

                if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Video) && !Documento.EsVideoIncrustado && Documento.CompruebaUrlEsVideo(enlaceOriginal))
                {
                    // Ha dejado de ser un vídeo, cambio el tipo del documento a enlace
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.Hipervinculo;
                }
                if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.FicheroServidor) && !Documento.EsPresentacionIncrustada && Documento.CompruebaUrlEsPresentacion(enlaceOriginal))
                {
                    // Ha dejado de ser una presentación de slideshare, cambio el tipo del documento a enlace
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.Hipervinculo;
                }
            }

            bool eraEnlace = Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo || Documento.EsVideoIncrustado || Documento.EsPresentacionIncrustada;

            ExtraerCambiosBasicos(Documento);

            List<Guid> listaProyectosActualNumRec = new List<Guid>();

            if (EsComunidad && Documento.FilaDocumento.Borrador && !borrador)
            {
                foreach (Guid baseRecurso in Documento.BaseRecursos)
                {
                    Guid proyectoID = GestorDocumental.ObtenerProyectoID(baseRecurso);
                    listaProyectosActualNumRec.Add(proyectoID);
                }

                if (ProyectoSeleccionado != null)
                {
                    ProyectoSeleccionado.FilaProyecto.NumeroRecursos++;
                }

                if (ProyectoSeleccionado.FilaProyecto.TieneTwitter)
                {
                    debePublicarEnTwitter = true;
                }
            }

            if (Documento.FilaDocumento.Borrador && !borrador)
            {
                DateTime fechaCreacion = DateTime.Now;

                Documento.FilaDocumento.FechaCreacion = fechaCreacion;

                GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Find(doc => doc.DocumentoID.Equals(Documento.Clave) && doc.BaseRecursosID.Equals(GestorDocumental.BaseRecursosIDActual)).FechaPublicacion = fechaCreacion;

                if (Session.Get("DocumentoVisto") == null)
                {
                    Session.Set("DocumentoVisto", new List<Guid>());
                }
                List<Guid> documentosVistos = Session.Get<List<Guid>>("DocumentoVisto");
                documentosVistos.Add(mDocumentoID);
                Session.Set("DocumentoVisto", documentosVistos);
            }

            if (Documento.FilaDocumento.Borrador && !borrador)
            {
                Documento.FilaDocumento.Borrador = borrador;
            }

            #region TipoDocumentacion

            //Si Antes era vimeo/youtube/enlace
            if (eraEnlace)
            {
                if (Documento.TipoDocumentacion == TiposDocumentacion.VideoBrightcove)
                {
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.VideoBrightcove;
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.AudioBrightcove)
                {
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.AudioBrightcove;
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.VideoTOP)
                {
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.VideoTOP;
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.AudioTOP)
                {
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.AudioTOP;
                }
                else if (Documento.EsVideoIncrustado)
                {
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.Video;
                }
                else if (Documento.EsPresentacionIncrustada)
                {
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.FicheroServidor;
                }
                else if (!string.IsNullOrEmpty(Documento.Enlace))
                {
                    Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.Hipervinculo;
                }
            }

            #endregion

            GuardarDatosCategorias(pCategoriasSeleccionadas, Documento);
            GuardarDatosAutoriaDocumento(Documento);

            bool privacidadCambiada = false;
            pListaEditoresEliminados = new List<Guid>();
            pListaGruposEditoresEliminados = new List<Guid>();
            GuardarDatosEditores(Documento, out privacidadCambiada, out pListaEditoresEliminados, out pListaGruposEditoresEliminados);

            GuardarDatosRespuestasEncuesta(Documento);
            GuardarImagenPrincipalRecSemantico(Documento, mOntologia);

            AgregarEstadoDocumento(Documento.FilaDocumento);

            #region Licencia propiedad intelectual

            if (Documento.PermiteLicencia && Documento.CreadorID.Equals(IdentidadActual.Clave))
            {
                if (mModelSaveRec.CreatorIsAuthor && (mModelSaveRec.ShareAllowed || Documento.BaseRecursos.Count > 1 || (EsComunidad && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido))))
                {
                    Documento.FilaDocumento.Licencia = mModelSaveRec.License;
                }
                else
                {
                    Documento.FilaDocumento.Licencia = null;
                }
            }

            #endregion

            #region Eliminar documento autoguardado

            //Elimino el posible documento temporal autoguardado.
            if (DocumentoAutoGuardadoWiki != null && GestorDocumental.ListaDocumentos.ContainsKey(DocumentoAutoGuardadoWiki.Clave))
            {
                GestorDocumental.EliminarDocumentoWeb(DocumentoAutoGuardadoWiki);
            }

            #endregion

            Documento.FilaDocumento.FechaModificacion = DateTime.Now;
            Documento.FilaDocumento.CompartirPermitido = mModelSaveRec.ShareAllowed;

            if (!pCreandoVersion && !pVersionCreadaPorConcurrencia)
            {
                GestorDocumental.AgregarGuardadoDocumentoHistorial(Documento, UsuarioActual);
            }

            Guardar_ModificarRecurso(listaProyectosActualNumRec);

            #region Actualizamos la cache

            try
            {
                //Borramos la cache de las comunidades afectadas
                DocumentacionCL documentacionCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                //Borramos la cache de recursos relacionados de las comunidades afectadas, asi como todas las relacionadas
                documentacionCL.BorrarRecursosRelacionados(listaProyectosActualNumRec);
                documentacionCL.Dispose();
            }
            catch (Exception ex)
            {
                GuardarLogError($" ERROR: {ex.Message}\r\nStackTrace: {ex.StackTrace}");
            }

            #endregion

            #region Privacidad

            //Solo hay que comprobarlo para el MetaProyecto, sino se suben los recursos en la home de la comunidad a la primera posición tras editarlos.
            if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                bool docPublico = Documento.FilaDocumento.Publico;
                ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(Documento, IdentidadActual, true);

                if (docPublico != Documento.FilaDocumento.Publico)
                {
                    privacidadCambiada = true;
                }
            }

            #endregion

            if (privacidadCambiada)
            {
                //Hay que limpiar la caché de los perfiles que tienen recursos privados
                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                if (!borrador && Documento.TipoDocumentacion != TiposDocumentacion.Encuesta)
                {
                    docCL.InvalidarPerfilesConRecursosPrivados(ProyectoSeleccionado.Clave);
                }
                docCL.Dispose();
            }

            if (Documento.FilaDocumentoWebVinBR.PrivadoEditores)
            {
                debePublicarEnTwitter = false;
            }

            if (debePublicarEnTwitter)
            {
                ControladorDocumentacion.EnviarEnlaceATwitterDeComunidad(IdentidadActual, Documento, ProyectoSeleccionado, BaseURLIdioma, UtilIdiomas, mAvailableServices);
            }

            pPrivacidadCambiada = privacidadCambiada;
        }

        private void AgregarEstadoDocumento(AD.EntityModel.Models.Documentacion.Documento pDocumento)
        {
            if (!pDocumento.EstadoID.HasValue)
            {
                FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
                Guid? estadoID = null;
                switch (pDocumento.Tipo)
                {
                    case (short)TiposDocumentacion.Hipervinculo:
                        estadoID = flujosCN.ObtenerEstadoInicialDeTipoContenido(ProyectoSeleccionado.Clave, TiposContenidos.Link);
                        break;
                    case (short)TiposDocumentacion.Nota:
                        estadoID = flujosCN.ObtenerEstadoInicialDeTipoContenido(ProyectoSeleccionado.Clave, TiposContenidos.Nota);
                        break;
                    case (short)TiposDocumentacion.FicheroServidor:
                        estadoID = flujosCN.ObtenerEstadoInicialDeTipoContenido(ProyectoSeleccionado.Clave, TiposContenidos.Adjunto);
                        break;
                    case (short)TiposDocumentacion.Video:
                        estadoID = flujosCN.ObtenerEstadoInicialDeTipoContenido(ProyectoSeleccionado.Clave, TiposContenidos.Video);
                        break;
                    case (short)TiposDocumentacion.Encuesta:
                        estadoID = flujosCN.ObtenerEstadoInicialDeTipoContenido(ProyectoSeleccionado.Clave, TiposContenidos.Encuesta);
                        break;
                    case (short)TiposDocumentacion.Debate:
                        estadoID = flujosCN.ObtenerEstadoInicialDeTipoContenido(ProyectoSeleccionado.Clave, TiposContenidos.Debate);
                        break;
                    case (short)TiposDocumentacion.Semantico:
                        ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                        string nombreOntologia = proyectoCN.ObtenerNombreOntologiaProyectoPorOntologiaID(pDocumento.ElementoVinculadoID.Value);
                        Guid flujoID = flujosCN.ObtenerFlujoIDDeOntologia(ProyectoSeleccionado.Clave, nombreOntologia);
                        estadoID = flujosCN.ObtenerEstadoInicialDeTipoContenido(ProyectoSeleccionado.Clave, TiposContenidos.RecursoSemantico, flujoID);
                        break;
                }

                if (estadoID.HasValue)
                {
                    pDocumento.EstadoID = estadoID.Value;
                    if (Documento != null && Documento.FilaDocumento != null)
                    {
                        Documento.FilaDocumento.EstadoID = estadoID.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Guarda el recruso actual en el modelo Live.
        /// </summary>
        /// <param name="pPrivacidadCambiada">Indica si la privacidad del recurso ha cambiado en esta edición</param>
        /// <param name="pCreandoVersion">Indica si se está creando versión</param>
        /// <param name="pCambioDeBorradoAPublicado">Indica si el recurso a cambiado de borrador a publicado</param>
        /// <param name="pDocAntiguoID">ID del documento antiguo, si se ha creado versión</param>
        /// <param name="pDocumento">Documento que se está guardando</param>
        private void GuardarRecursoModeloLive(bool pPrivacidadCambiada, bool pCreandoVersion, bool pCambioDeBorradoAPublicado, Guid pDocAntiguoID, Documento pDocumento, List<Guid> pListaEditoresEliminados, List<Guid> pListaGruposEditoresEliminados)
        {
            if (!mModelSaveRec.Draft && pDocumento.TipoDocumentacion != TiposDocumentacion.Encuesta)
            {
                foreach (Guid baseRecurso in pDocumento.BaseRecursos)
                {
                    Guid proyecto = GestorDocumental.ObtenerProyectoID(baseRecurso);

                    int tipo;
                    switch (pDocumento.TipoDocumentacion)
                    {
                        case TiposDocumentacion.Debate:
                            tipo = (int)TipoLive.Debate;
                            break;
                        case TiposDocumentacion.Pregunta:
                            tipo = (int)TipoLive.Pregunta;
                            break;
                        default:
                            tipo = (int)TipoLive.Recurso;
                            break;
                    }

                    string infoExtra = null;

                    if (proyecto == ProyectoAD.MetaProyecto)
                    {
                        if (IdentidadActual.IdentidadPersonalMyGNOSS != null)
                        {
                            infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                        }
                        else
                        {
                            infoExtra = IdentidadActual.IdentidadMyGNOSS.PerfilID.ToString();
                        }
                    }

                    if (pPrivacidadCambiada)
                    {
                        infoExtra = ObtenerCampoInfoExtraLive(pListaEditoresEliminados, pListaGruposEditoresEliminados);
                    }

                    if (pCreandoVersion)
                    {
                        ControladorDocumentacion.ActualizarGnossLive(proyecto, pDocAntiguoID, AccionLive.Eliminado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);

                        if (pPrivacidadCambiada)
                        {
                            ControladorDocumentacion.ActualizarGnossLive(proyecto, pDocumento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Media, null, mAvailableServices);
                            ControladorDocumentacion.ActualizarGnossLive(proyecto, pDocumento.Clave, AccionLive.Editado, tipo, PrioridadLive.Media, infoExtra, mAvailableServices);
                        }
                        else
                        {
                            ControladorDocumentacion.ActualizarGnossLive(proyecto, pDocumento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Media, infoExtra, mAvailableServices);
                        }
                    }
                    else if (pCambioDeBorradoAPublicado)
                    {
                        ControladorDocumentacion.ActualizarGnossLive(proyecto, pDocumento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);
                        ControladorDocumentacion.ActualizarGnossLive(proyecto, mControladorBase.UsuarioActual.IdentidadID, AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);
                    }
                    else
                    {
                        ControladorDocumentacion.ActualizarGnossLive(proyecto, pDocumento.Clave, AccionLive.Editado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);
                    }
                }
            }
        }

        private static string ObtenerCampoInfoExtraLive(List<Guid> pListaEditoresEliminadosIDs, List<Guid> pListaGruposEditoresEliminadosIDs)
        {
            string infoExtra = Constantes.PRIVACIDAD_CAMBIADA;
            //Obtener los editores que se han quitado del recurso si hay alguno.
            infoExtra += ObtenerInfoExtraEditores(pListaEditoresEliminadosIDs, Constantes.EDITOR_ELIMINADO);

            //Obtener los grupos que se han quitado como editores del recurso si hay alguno.
            infoExtra += ObtenerInfoExtraEditores(pListaGruposEditoresEliminadosIDs, Constantes.GRUPO_EDITORES_ELIMINADO);

            return infoExtra;
        }

        private static string ObtenerInfoExtraEditores(List<Guid> pListaIDs, string pCadenaConcatenarInfoExtra)
        {
            StringBuilder infoExtra = new StringBuilder();
            if (pListaIDs.Count > 0)
            {
                infoExtra.Append(pCadenaConcatenarInfoExtra);

                foreach (Guid editorID in pListaIDs)
                {
                    infoExtra.Append($"{editorID}|");
                }

                UtilCadenas.EliminarUltimosCaracteresStringBuilder(infoExtra, '|');

                infoExtra.Append(pCadenaConcatenarInfoExtra);
            }

            return infoExtra.ToString();
        }

        /// <summary>
        /// Guarda el recurso actual en el modelo Base.
        /// </summary>
        /// <param name="pCreandoVersion">Indica si se está creando versión</param>
        /// <param name="pCambioDeBorradoAPublicado">Indica si el recurso a cambiado de borrador a publicado</param>
        /// <param name="pDocAntiguoID">ID del documento antiguo, si se ha creado versión</param>
        /// <param name="pDocumento">Documento que se está guardando</param>
        private void GuardarRecursoModeloBase(bool pCreandoVersion, bool pCambioDeBorradoAPublicado, Guid pDocAntiguoID, Documento pDocumento)
        {
            if (!pCreandoVersion)
            {
                foreach (Guid proyectoID in pDocumento.ListaProyectos)
                {
                    if (proyectoID.Equals(pDocumento.ProyectoID) && !proyectoID.Equals(ProyectoAD.MyGnoss))
                    {
                        mOtrosArgumentosBase = $",##enlaces##{ExtraerTexto(mModelSaveRec.TagsLinks)}##enlaces##";
                    }
                    else
                    {
                        PrioridadBase prioridad = PrioridadBase.Resto;
                        if (proyectoID.Equals(ProyectoAD.MyGnoss))
                        {
                            //para el espacio personal la prioridad debe ser alta
                            prioridad = PrioridadBase.Alta;

                            //está editando un recurso en el espacio personal, va a ir al Base y no hace falta insertarlo directamente en el grafo de busqueda
                            if (ProyectoSeleccionado.Clave.Equals(proyectoID))
                            {
                                mInsertadoEnGrafoBusqueda = true;
                            }
                        }

                        ControladorDocumentacion.AgregarRecursoModeloBaseSimple(pDocumento.Clave, proyectoID, pDocumento.FilaDocumento.Tipo, null, ",##enlaces##" + ExtraerTexto(mModelSaveRec.TagsLinks) + "##enlaces##", prioridad, mAvailableServices);
                    }
                }

                if (pCambioDeBorradoAPublicado)
                {
                    try
                    {
                        //Actualización Offline a partir de un servicio UDP
                        //Llamada asincrona para actualizar la popularidad del recurso:
                        ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("recursos", pDocumento.Clave, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.IdentidadID, pDocumento.CreadorID);
                    }
                    catch (Exception ex)
                    {
                        GuardarLogError($" ERROR: {ex.Message}\r\nStackTrace: {ex.StackTrace}");
                    }
                }
            }
            else
            {
                foreach (Guid proyectoID in pDocumento.ListaProyectos)
                {
                    if (proyectoID.Equals(pDocumento.ProyectoID) && !proyectoID.Equals(ProyectoAD.MyGnoss))
                    {
                        mOtrosArgumentosBase = $",##enlaces##{ExtraerTexto(mModelSaveRec.TagsLinks)}##enlaces##";
                        mCreandoVersion = true;
                    }
                    else
                    {
                        PrioridadBase prioridad = PrioridadBase.Resto;
                        if (proyectoID.Equals(ProyectoAD.MyGnoss))
                        {
                            //para el espacio personal la prioridad debe ser alta
                            prioridad = PrioridadBase.Alta;

                            //está versionando un recurso en el espacio personal, va a ir al Base y no hace falta insertarlo directamente en el grafo de busqueda
                            if (ProyectoSeleccionado.Clave.Equals(proyectoID))
                            {
                                mInsertadoEnGrafoBusqueda = true;
                            }
                        }

                        ControladorDocumentacion.AgregarRecursoEliminadoModeloBaseSimple(pDocAntiguoID, proyectoID, pDocumento.FilaDocumento.Tipo, prioridad, mAvailableServices);
                        ControladorDocumentacion.AgregarRecursoModeloBaseSimple(pDocumento.Clave, proyectoID, pDocumento.FilaDocumento.Tipo, null, ",##enlaces##" + ExtraerTexto(mModelSaveRec.TagsLinks) + "##enlaces##", prioridad, mAvailableServices);
                    }
                }
            }
        }

        /// <summary>
        /// Guarda el documento en la base de datos.
        /// </summary>
        /// <param name="pListaProyectosActuNumRec">Lista con los identificadores de los proyectos a los que hay que modificar su número de recursos</param>
        /// <param name="pVersionCreadaPorConcurrencia">Verdad si la versión se ha creado por que ha habido concurrencia</param>
        private void Guardar_ModificarRecurso(List<Guid> pListaProyectosActuNumRec)
        {
            if (!pListaProyectosActuNumRec.Contains(ProyectoSeleccionado.Clave))
            {
                pListaProyectosActuNumRec.Add(ProyectoSeleccionado.Clave);
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Auto guarda la información del formulario.
        /// </summary>
        /// <param name="sender">El que produce el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private ActionResult AutoGuardadoWiki_ModificarRecurso()
        {
            if (Documento.TipoDocumentacion != TiposDocumentacion.Wiki)
            {
                throw new ExcepcionWeb("No es Wiki");
            }

            List<Guid> categoriasSeleccionadas = RecogerCategoriasSeleccionadas();

            DocumentoWeb documentoARevisar;
            if (DocumentoAutoGuardadoWiki != null)
            {
                documentoARevisar = DocumentoAutoGuardadoWiki;
            }
            else
            {
                documentoARevisar = Documento;
            }

            if (HayCambiosSinGuardarParaAutoGuardado(documentoARevisar, categoriasSeleccionadas))
            {
                string mensError = ValidarRecurso(true, categoriasSeleccionadas);

                if (!string.IsNullOrEmpty(mensError))
                {
                    return GnossResultERROR(mensError);
                }

                GuardarRecursoAutoguardado(categoriasSeleccionadas);
            }

            return Content("");
        }

        #region Recuperación del AutoGuardado

        /// <summary>
        /// Carga los documentos que han sido autoguardados a partir del que se está editando.
        /// </summary>
        private void CargarDocumentosTemporalesAutoguardados_ModificarRecurso()
        {
            if (Documento.TipoDocumentacion == TiposDocumentacion.Wiki)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dataWrapperDocumentacionDocsTemporales = docCN.ObtenerDocumentosTemporalesDeDocumento(mDocumentoID, mControladorBase.UsuarioActual.IdentidadID, TiposDocumentacion.WikiTemporal);

                if (dataWrapperDocumentacionDocsTemporales.ListaDocumento.Count > 0 && !GestorDocumental.ListaDocumentos.ContainsKey(dataWrapperDocumentacionDocsTemporales.ListaDocumento[0].DocumentoID))
                {
                    docCN.ObtenerDocumentoPorIDCargarTotal(dataWrapperDocumentacionDocsTemporales.ListaDocumento[0].DocumentoID, dataWrapperDocumentacionDocsTemporales, true, true, GestorDocumental.BaseRecursosIDActual);

                    GestorDocumental.DataWrapperDocumentacion.Merge(dataWrapperDocumentacionDocsTemporales);

                    //Es importante crear el documento con la fila del gestor y no con la del dataSet temporal, sino habrá futuros errores:                    
                    Documento documento = new Documento(GestorDocumental.DataWrapperDocumentacion.ListaDocumento.FirstOrDefault(doc => doc.DocumentoID.Equals(dataWrapperDocumentacionDocsTemporales.ListaDocumento[0].DocumentoID)), GestorDocumental);
                    GestorDocumental.ListaDocumentos.Add(documento.Clave, documento);

                    //Convierto el documento en documentoWeb
                    DocumentoAutoGuardadoWiki = new DocumentoWeb(documento.FilaDocumento, GestorDocumental);
                }
                else if (dataWrapperDocumentacionDocsTemporales.ListaDocumento.Count > 0)
                {
                    DocumentoAutoGuardadoWiki = GestorDocumental.ObtenerDocumentoWeb(GestorDocumental.ListaDocumentos[dataWrapperDocumentacionDocsTemporales.ListaDocumento[0].DocumentoID]);
                }

                docCN.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// Extrae el archivo para reemplazar el actual.
        /// </summary>
        /// <returns>Par de booleanos que indican: El 1º que se debe hacer la captura web del recurso y el 2º que se debe generar la mini-imagen del recurso</returns>
        private KeyValuePair<bool, bool> ExtraerReemplazoArchivo_ModificarRecurso()
        {
            bool capturarImagenURL = false;
            bool generarImagenPequeDoc = false;

            #region Reemplazar documento

            if (Documento.EsFicheroDigital)
            {
                if (!string.IsNullOrEmpty(NombreArchivoTemporal) || (Session.Get("nombreFuReemplazado") != null))
                {
                    string nombreArchivo = "";

                    if (!string.IsNullOrEmpty(NombreArchivoTemporal))
                    {
                        nombreArchivo = NombreArchivoTemporal;
                    }
                    else
                    {
                        nombreArchivo = Session.GetString("nombreFuReemplazado");
                    }
                    TipoArchivo tipoRecursoNuevo = UtilArchivos.ObtenerTipoArchivo(nombreArchivo);

                    if (tipoRecursoNuevo == TipoArchivo.Audio || tipoRecursoNuevo == TipoArchivo.Video)
                    {
                        Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.Video;
                    }
                    else if (tipoRecursoNuevo == TipoArchivo.Imagen)
                    {
                        Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.Imagen;
                        generarImagenPequeDoc = true;
                    }
                    else if (tipoRecursoNuevo == TipoArchivo.Otros)
                    {
                        Documento.FilaDocumento.Tipo = (short)TiposDocumentacion.FicheroServidor;
                    }

                    string nuevoEnlace = ReemplazarArchivos_ModificarRecurso();

                    if (!string.IsNullOrEmpty(nuevoEnlace))
                    {
                        if (nuevoEnlace.StartsWith("KO|"))
                        {
                            mErrorDocumento = nuevoEnlace;
                            return new KeyValuePair<bool, bool>(false, false);
                        }

                        Documento.Enlace = nuevoEnlace;
                    }
                    else
                    {
                        throw new ExcepcionWeb("No se ha subido archivo");
                    }
                }

                //Ahora se debe obtener la imagen de la descripción.
                if (Documento.TipoDocumentacion == TiposDocumentacion.FicheroServidor)
                {
                    generarImagenPequeDoc = true;
                }
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo || ((Documento.TipoDocumentacion == TiposDocumentacion.Video && Documento.EsVideoIncrustado) || (Documento.TipoDocumentacion == TiposDocumentacion.FicheroServidor && Documento.EsPresentacionIncrustada)))
            {
                capturarImagenURL = true;
                Documento.Enlace = ExtraerTexto(mModelSaveRec.Link);
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Nota)
            {
                capturarImagenURL = true;
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.ReferenciaADoc)
            {
                Documento.Enlace = ExtraerTexto(mModelSaveRec.Link);
            }

            #endregion

            return new KeyValuePair<bool, bool>(capturarImagenURL, generarImagenPequeDoc);
        }

        /// <summary>
        /// Agrega un Archivo a Gnoss
        /// </summary>
        /// <returns>El nombre del archivo reemplazador</returns>
        private string ReemplazarArchivos_ModificarRecurso()
        {
            if (!string.IsNullOrEmpty(NombreArchivoTemporal) || (Session.Get("fuReemplazado") != null))
            {
                Stopwatch sw = null;
                try
                {
                    TipoArchivo tipoArchivo = TipoArchivo.Otros;
                    string idAuxGestorDocumental = "";
                    GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                    gd.Url = UrlServicioWebDocumentacion;

                    byte[] buffer1 = null;
                    FileInfo archivoInfo = null;

                    if (!string.IsNullOrEmpty(NombreArchivoTemporal))
                    {
                        buffer1 = CargarArchivoTemporal_ModificarRecurso();
                        tipoArchivo = UtilArchivos.ObtenerTipoArchivo(NombreArchivoTemporal);
                        archivoInfo = new FileInfo(NombreArchivoTemporal);
                    }
                    else if (Session.Get("fuReemplazado") != null)
                    {
                        buffer1 = Session.Get("fuReemplazado");
                        tipoArchivo = UtilArchivos.ObtenerTipoArchivo(Session.GetString("nombreFuReemplazado"));
                        archivoInfo = new FileInfo(Session.GetString("nombreFuReemplazado"));
                    }
                    Session.Remove("fuReemplazado");
                    Session.Remove("nombreFuReemplazado");

                    if (buffer1 == null || buffer1.Length == 0)
                    {
                        mLoggingService.GuardarLog("El fichero que se intenta subir es vacio", mlogger);
                        throw new ExcepcionGeneral("El fichero que se esta intentado subir es vacio");
                    }
                    double tamanoBytes = buffer1.Length;
                    double tamanoArchivoMB = ((tamanoBytes / 1024) / 1024);
                    double tamanoAntiguoArchivoMB = 0;

                    string extensionArchivo = System.IO.Path.GetExtension(archivoInfo.Name).ToLower();

                    int resultado = 0;
                    string googleID = null;


                    if (tipoArchivo == TipoArchivo.Audio || tipoArchivo == TipoArchivo.Video)
                    {
                        ServicioVideos servicioVideos = new ServicioVideos(mConfigService, mLoggingService, mLoggerFactory.CreateLogger<ServicioVideos>(), mLoggerFactory);

                        if (!EsComunidad)
                        {
                            if (IdentidadOrganizacion == null)
                            {
                                tamanoAntiguoArchivoMB = servicioVideos.ObtenerEspacioVideoPersonal(mDocumentoID, mControladorBase.UsuarioActual.PersonaID);
                                resultado = servicioVideos.AgregarVideoPersonal(buffer1, extensionArchivo, mDocumentoID, mControladorBase.UsuarioActual.PersonaID);
                            }
                            else
                            {
                                tamanoAntiguoArchivoMB = servicioVideos.ObtenerEspacioVideoOrganizacion(mDocumentoID, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                                resultado = servicioVideos.AgregarVideoOrganizacion(buffer1, extensionArchivo, mDocumentoID, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                            }
                        }
                        else
                        {
                            resultado = servicioVideos.AgregarVideo(buffer1, extensionArchivo, mDocumentoID);
                        }
                    }
                    else if (tipoArchivo == TipoArchivo.Imagen)
                    {
                        Image imagen = Image.Load(buffer1);

                        int anchoMaximo = 582;

                        float ancho = imagen.Width;
                        float alto = imagen.Height;
                        if (ancho > anchoMaximo)
                        {
                            ancho = anchoMaximo;
                            alto = (imagen.Height * ancho) / imagen.Width;
                        }

                        Image imagenPeque = UtilImages.AjustarImagen(imagen, ancho, alto);
                        byte[] bufferReducido = UtilImages.ImageToBytePng(imagenPeque);

                        ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                        servicioImagenes.Url = UrlIntragnossServicios;
                        bool correcto = false;

                        if (!EsComunidad)
                        {
                            if (IdentidadOrganizacion == null)
                            {
                                tamanoAntiguoArchivoMB = servicioImagenes.ObtenerEspacioImagenDocumentoPersonal(mDocumentoID.ToString(), extensionArchivo, mControladorBase.UsuarioActual.PersonaID);
                                correcto = servicioImagenes.AgregarImagenDocumentoPersonal(bufferReducido, mDocumentoID.ToString(), extensionArchivo, mControladorBase.UsuarioActual.PersonaID);
                            }
                            else
                            {
                                tamanoAntiguoArchivoMB = servicioImagenes.ObtenerEspacioImagenDocumentoOrganizacion(mDocumentoID.ToString(), extensionArchivo, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                                correcto = servicioImagenes.AgregarImagenDocumentoOrganizacion(bufferReducido, mDocumentoID.ToString(), extensionArchivo, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                            }
                        }
                        else
                        {
                            correcto = servicioImagenes.AgregarImagenADirectorio(bufferReducido, UtilArchivos.ContentImagenesDocumentos + "/" + UtilArchivos.DirectorioDocumento(mDocumentoID), mDocumentoID.ToString(), extensionArchivo);
                        }

                        Documento.NombreEntidadVinculada = "Wiki2";

                        if (correcto)
                        {
                            resultado = 0;

                            if (!Documento.FilaDocumento.VersionFotoDocumento.HasValue)
                            {
                                Documento.FilaDocumento.VersionFotoDocumento = 1;
                            }
                            else
                            {
                                Documento.FilaDocumento.VersionFotoDocumento = Math.Abs(Documento.FilaDocumento.VersionFotoDocumento.Value) + 1;
                            }

                            sw = LoggingService.IniciarRelojTelemetria();

                            if (!EsComunidad)
                            {
                                if (IdentidadOrganizacion == null)
                                {
                                    idAuxGestorDocumental = gd.AdjuntarDocumentoABaseRecursosUsuario(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, mDocumentoID, extensionArchivo);
                                }
                                else
                                {
                                    idAuxGestorDocumental = gd.AdjuntarDocumentoABaseRecursosOrganizacion(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), mDocumentoID, extensionArchivo);
                                }
                            }
                            else
                            {
                                idAuxGestorDocumental = gd.AdjuntarDocumento(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.ProyectoID, mDocumentoID, extensionArchivo);
                            }
                            if (!idAuxGestorDocumental.ToUpper().Equals("ERROR"))
                            {
                                resultado = 1;
                            }
                            mLoggingService.AgregarEntradaDependencia("Adjuntar archivo al gestor documental", false, "ReemplazarArchivos_MofificarRecurso", sw, true);
                        }
                    }
                    else if (tipoArchivo == TipoArchivo.Otros)
                    {
                        //si tiene google drive: 
                        if (TieneGoogleDriveConfigurado)
                        {
                            try
                            {
                                string[] separadores = { ID_GOOGLE };
                                string[] nombreDoc = Documento.Enlace.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                                if (nombreDoc.Length > 1)
                                {
                                    googleID = nombreDoc[1];

                                    FileInfo informacionNombre = new FileInfo(googleID);
                                    googleID = ExtraerNombreFicheroSinExtension(informacionNombre.Name);
                                    Path.GetExtension(NombreArchivoTemporal);
                                    ExtraerNombreFicheroSinExtension(NombreArchivoTemporal);
                                }
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError(ex.ToString());
                                resultado = 0;
                            }
                        }
                        else
                        {
                            //Subimos el fichero al servidor
                            gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                            gd.Url = UrlServicioWebDocumentacion;

                            sw = LoggingService.IniciarRelojTelemetria();

                            if (!EsComunidad)
                            {
                                if (IdentidadOrganizacion == null)
                                {
                                    tamanoAntiguoArchivoMB = gd.ObtenerEspacioDocumentoDeBaseRecursosUsuario(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, mDocumentoID, extensionArchivo);
                                    idAuxGestorDocumental = gd.AdjuntarDocumentoABaseRecursosUsuario(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, mDocumentoID, extensionArchivo);
                                }
                                else
                                {
                                    tamanoAntiguoArchivoMB = gd.ObtenerEspacioDocumentoDeBaseRecursosOrganizacion(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), mDocumentoID, extensionArchivo);
                                    idAuxGestorDocumental = gd.AdjuntarDocumentoABaseRecursosOrganizacion(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), mDocumentoID, extensionArchivo);
                                }
                            }
                            else
                            {
                                string tipoEntidadTexto = ControladorDocumentacion.ObtenerTipoEntidadAdjuntarDocumento(Documento.TipoEntidadVinculada);

                                idAuxGestorDocumental = gd.AdjuntarDocumento(buffer1, tipoEntidadTexto, Documento.OrganizacionID, Documento.ProyectoID, mDocumentoID, extensionArchivo);
                            }
                            if (!idAuxGestorDocumental.ToUpper().Equals("ERROR"))
                            {
                                resultado = 1;
                            }
                            mLoggingService.AgregarEntradaDependencia("Adjuntar archivo al gestor documental", false, "ReemplazarArchivos_MofificarRecurso", sw, true);
                        }
                    }
                    #region Tamaño archivo

                    if (!EsComunidad)
                    {
                        GestorDocumental.EspacioActualBaseRecursos = GestorDocumental.EspacioActualBaseRecursos - tamanoAntiguoArchivoMB + tamanoArchivoMB;
                        if (GestorDocumental.EspacioActualBaseRecursos < 0)
                        {
                            GestorDocumental.EspacioActualBaseRecursos = 0;
                        }
                    }

                    #endregion

                    string error = null;

                    switch (resultado)
                    {
                        case 0:
                            {
                                error = "KO|" + UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC");
                                break;
                            }
                        case 1:
                            {
                                //si no tiene google drive: 
                                if (TieneGoogleDriveConfigurado && Documento.TipoDocumentacion == TiposDocumentacion.FicheroServidor)
                                {
                                    string extension = "";
                                    int punto = Documento.NombreDocumento.LastIndexOf(".");

                                    if (punto != -1)
                                    {
                                        extension = Documento.NombreDocumento.Substring(punto);
                                    }
                                    //Eliminar anterior
                                    if (extension.ToLower() != extensionArchivo.ToLower())
                                    {
                                        sw = LoggingService.IniciarRelojTelemetria();
                                        if (!EsComunidad)
                                        {
                                            if (IdentidadOrganizacion == null)
                                            {
                                                gd.BorrarDocumentoDeBaseRecursosUsuario(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, Documento.Clave, extension);
                                            }
                                            else
                                            {
                                                gd.BorrarDocumentoDeBaseRecursosOrganizacion(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), Documento.Clave, extension);
                                            }
                                        }
                                        else
                                        {
                                            gd.BorrarDocumento(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, Documento.OrganizacionID, Documento.ProyectoID, Documento.Clave, extension);
                                        }
                                        mLoggingService.AgregarEntradaDependencia("Borrar documento del gestor documental", false, "ReemplazarArchivos_MofificarRecurso", sw, true);
                                    }
                                }

                                string nuevoEnlace = archivoInfo.Name;

                                if (!string.IsNullOrEmpty(googleID))
                                {
                                    FileInfo informacionNombre = new FileInfo(NombreArchivoTemporal);
                                    nuevoEnlace = ExtraerNombreFicheroSinExtension(informacionNombre.Name) + ID_GOOGLE + googleID + extensionArchivo;
                                }

                                if (Documento.TipoDocumentacion == TiposDocumentacion.Video)
                                {
                                    nuevoEnlace = Documento.Clave + archivoInfo.Extension;
                                }
                                return nuevoEnlace;
                            }
                        case 2:
                            {
                                error = $"KO|{UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSEGURIDAD")}";
                                break;
                            }
                        case 3:
                            {
                                error = $"KO|{UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSIZE")}";
                                break;
                            }
                    }

                    return error;
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex.ToString());

                    //Se han producido errores al guardar el documento en el servidor
                    throw;
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene el archivo temporal previamente subido.
        /// </summary>
        /// <returns>Bytes del archivo temporal previamente subido</returns>
        private byte[] CargarArchivoTemporal_ModificarRecurso()
        {
            string txtHackArchivo = mModelSaveRec.TemporalFileName;

            if (!string.IsNullOrEmpty(txtHackArchivo))
            {
                #region Obtenemos el fichero

                string extension = Path.GetExtension(txtHackArchivo).ToLower();
                string nombre = Path.GetFileNameWithoutExtension(txtHackArchivo).ToLower();

                //Obtenemos el fichero del servidor
                GestionDocumental gestorDoc = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                gestorDoc.Url = UrlServicioWebDocumentacion;

                byte[] archivoTemporal = gestorDoc.ObtenerDocumentoWebTemporal(IdentidadActual.Clave, nombre, extension);

                return archivoTemporal;

                #endregion
            }

            return new byte[0];
        }

        /// <summary>
        /// Muestra el panel de concurrencia.
        /// </summary>
        /// <param name="pIdentidadID">ID de la identidad que causa la concurrencia</param>
        /// <returns>Acción respuesta</returns>
        private ActionResult MostrarPanelConcurrencia_ModificarRecurso(Guid pIdentidadID)
        {
            UploadResourceReplayPanelModel subRecPanRepModel = new UploadResourceReplayPanelModel();
            subRecPanRepModel.CanRepeatResource = true;
            subRecPanRepModel.RepeatedResourceType = 6;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerIdentidadPorID(pIdentidadID, false);
            identidadCN.Dispose();

            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = dataWrapperIdentidad.ListaIdentidad.FirstOrDefault();
            AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = dataWrapperIdentidad.ListaPerfil.FirstOrDefault();

            string nombre = filaPerfil.NombrePerfil;

            if ((filaIdentidad.Tipo > 0) && (filaIdentidad.Tipo < 4))
            {
                //Trabaja con una organización, tengo que mostrar el nombre de la organización
                if ((filaIdentidad.Tipo.Equals(1)) || (filaIdentidad.Tipo.Equals(2) && IdentidadActual.OrganizacionID.HasValue && IdentidadActual.OrganizacionID.Value.Equals(filaPerfil.OrganizacionID)))
                {
                    nombre += $" {ConstantesDeSeparacion.SEPARACION_CONCATENADOR} {filaPerfil.NombreOrganizacion}";
                }
                else
                {
                    nombre = filaPerfil.NombreOrganizacion;
                }
            }

            subRecPanRepModel.ProfileConcurrencyName = UtilIdiomas.GetText("PERFILBASE", "SOBRESCRIBIRCAMBIOS", nombre);
            subRecPanRepModel.CreateVersionIfConcurrency = Documento.PermiteVersiones;

            return PartialView("_SubirRecurso_PanelRepeticion", subRecPanRepModel);
        }

        /// <summary>
        /// Crea una nueva versión del documento y la pone como documento principal de la página
        /// </summary>
        /// <returns>Versión creada o NULL en caso de errores</returns>
        private Documento CrearVersionDocumento_ModificarRecurso()
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerVinculacionesRecurso(Documento.Clave, true));

            Documento nuevoDoc = null;
            bool esMejora = ComprobarSiSeEstaEditandoUnaMejora(mDocumentoID);
			nuevoDoc = GestorDocumental.CrearNuevaVersionDocumento(Documento, IdentidadCrearVersion, pEsMejora: esMejora);

            if (nuevoDoc != null)
            {
                if (!GestorDocumental.ListaDocumentos.ContainsKey(nuevoDoc.Clave))
                {
                    GestorDocumental.ListaDocumentos.Add(nuevoDoc.Clave, nuevoDoc);
                }
                Documento.FilaDocumento.FechaModificacion = DateTime.Now;
            }
            return nuevoDoc;
        }

        /// <summary>
        /// Comprueba la repetición del enlace de un recurso.
        /// </summary>
        /// <returns>Acción respuesta</returns>
        private ActionResult ComprobarRepeticionEnlaceRecurso_ModificarRecurso()
        {
            bool rbLectoresComunidad = (mModelSaveRec.ResourceVisibility == ResourceVisibility.CommunityMembers);
            bool rbAbierto = (mModelSaveRec.ResourceVisibility == ResourceVisibility.Open);

            bool recursoDejaSerPrivado = (Documento.FilaDocumentoWebVinBR.PrivadoEditores && (rbLectoresComunidad || rbAbierto));

            string funcionVuelta = "GuardarRecursoRepetido";

            if (Documento.EsBorrador)
            {
                funcionVuelta = "GuardarRecursoRepetido_Borrador";
            }

            if (!recursoDejaSerPrivado)
            {
                string enlace = "";
                string titulo = mModelSaveRec.Title;

                if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo || Documento.TipoDocumentacion == TiposDocumentacion.ReferenciaADoc)
                {
                    enlace = mModelSaveRec.Link;
                }
                else if ((Documento.TipoDocumentacion == TiposDocumentacion.FicheroServidor || Documento.TipoDocumentacion == TiposDocumentacion.Imagen || Documento.TipoDocumentacion == TiposDocumentacion.Video) && !string.IsNullOrEmpty(mModelSaveRec.TemporalFileName))
                {
                    enlace = mModelSaveRec.TemporalFileName.Substring(36);
                }

                if ((enlace != "" && enlace != Documento.Enlace) || (titulo != "" && titulo != Documento.Titulo))
                {
                    if (enlace == Documento.Enlace)
                    {
                        enlace = "";
                    }

                    if (titulo == Documento.Titulo)
                    {
                        titulo = "";
                    }

                    return ComprobarRepeticionEnlaceRecurso(titulo, enlace, null, funcionVuelta);
                }
            }
            else
            {
                return ComprobarRepeticionEnlaceRecurso(null, null, Documento.TipoDocumentacion, funcionVuelta);
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Realiza la carga inicial de los gestores para que funcione la pantalla.
        /// </summary>
        /// <returns>NULL si va bien o URL a la que hay que redirigir si el usuario no esta donde debe</returns>
        private void CargarInicial_ModificarRecurso()
        {
            CargaInicial();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            docCN.ObtenerDocumentoPorIDCargarTotal(mDocumentoID, GestorDocumental.DataWrapperDocumentacion, true, true, GestorDocumental.BaseRecursosIDActual);
            GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerVinculacionesRecurso(mDocumentoID));

            GestorDocumental.CargarDocumentos(false);

            mTipoDocumento = Documento.TipoDocumentacion;

            CargaInicialTesauro();

            List<Guid> listaIdentidadesURLSem = new List<Guid>();

            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(item => item.IdentidadPublicacionID.HasValue && !listaIdentidadesURLSem.Contains(item.IdentidadPublicacionID.Value)))
            {
                listaIdentidadesURLSem.Add(filaDocVinBR.IdentidadPublicacionID.Value);
            }

            if (!listaIdentidadesURLSem.Contains(IdentidadActual.Clave))
            {
                listaIdentidadesURLSem.Add(IdentidadActual.Clave);
            }

            GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerEditoresDocumento(Documento.Clave));

            List<Guid> listaPerfilesURlSem = new List<Guid>();

            //Cargo las identidades de los editores
            foreach (Editor.EditorRecurso editor in Documento.ListaPerfilesEditores.Values)
            {
                if ((!listaPerfilesURlSem.Contains(editor.FilaEditor.PerfilID)) && (editor.IdentidadEnProyectoActual(ProyectoSeleccionado.Clave) == null || GestorDocumental.GestorIdentidades == null || !GestorDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(editor.IdentidadEnProyectoActual(ProyectoSeleccionado.Clave).Clave)))
                {
                    listaPerfilesURlSem.Add(editor.FilaEditor.PerfilID);
                }
            }
            if (listaIdentidadesURLSem.Count > 0 || listaPerfilesURlSem.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperIdentidad identDW = new DataWrapperIdentidad();

                if (listaIdentidadesURLSem.Count > 0)
                {
                    identDW.Merge(identidadCN.ObtenerIdentidadesPorID(listaIdentidadesURLSem, false));
                }

                if (listaPerfilesURlSem.Count > 0)
                {
                    identDW.Merge(identidadCN.ObtenerIdentidadesDePerfiles(listaPerfilesURlSem));
                }

                GestionIdentidades gestorIdent = new GestionIdentidades(identDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();

                if (GestorDocumental.GestorIdentidades == null)
                {
                    GestorDocumental.GestorIdentidades = gestorIdent;
                }
                else
                {
                    GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(gestorIdent.DataWrapperIdentidad);
                    GestorDocumental.GestorIdentidades.RecargarHijos();
                }
            }

            //Cargo los grupos de los editores
            List<Guid> listaGrupos = new List<Guid>();
            foreach (Guid claveGrupoEditor in Documento.ListaGruposEditores.Values.Select(item => item.Clave).Where(item => !listaGrupos.Contains(item)))
            {
                listaGrupos.Add(claveGrupoEditor);
            }

            if (listaGrupos.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperIdentidad identDW = identidadCN.ObtenerGruposPorIDGrupo(listaGrupos);

                GestionIdentidades gestorIdent = new GestionIdentidades(identDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();

                if (GestorDocumental.GestorIdentidades == null)
                {
                    GestorDocumental.GestorIdentidades = gestorIdent;
                }
                else
                {
                    GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(gestorIdent.DataWrapperIdentidad);
                    GestorDocumental.GestorIdentidades.RecargarHijos();
                }
            }

            Documento.RecargarCategoriasTesauro();
            docCN.Dispose();

            #region Recuperar autoguardo

            if (Documento.TipoDocumentacion == TiposDocumentacion.Wiki)
            {
                CargarDocumentosTemporalesAutoguardados_ModificarRecurso();
            }

            #endregion
        }

        /// <summary>
        /// Carga los datos actuales del documento en la página.
        /// </summary>
        /// <param name="pDocEditModel">Modelo de documento de edición</param>
        private void CargarDatosDocumento_ModificarRecurso(DocumentEditionModel pDocEditModel)
        {
            Documento docDatos = Documento;

            if (RequestParams("recuperarAutoGuardado") != null)
            {
                docDatos = DocumentoAutoGuardadoWiki;
            }

            pDocEditModel.Title = docDatos.Titulo;
            pDocEditModel.Description = docDatos.Descripcion;

            pDocEditModel.Tags = docDatos.FilaDocumento.Tags;

            if (Session.Get("nombreFuReemplazado") != null)
            {
                pDocEditModel.Link = Session.GetString("nombreFuReemplazado");
            }
            else
            {
                pDocEditModel.Link = docDatos.Enlace;
            }

            pDocEditModel.ShareAllowed = (docDatos.FilaDocumento.CompartirPermitido && docDatos.TipoDocumentacion != TiposDocumentacion.Debate);

            pDocEditModel.Authors = "";

            if (!string.IsNullOrEmpty(docDatos.FilaDocumento.Autor))
            {
                if (docDatos.FilaDocumento.CreadorEsAutor)
                {
                    if (docDatos.FilaDocumento.Autor.Contains(","))
                    {
                        int inicio = docDatos.FilaDocumento.Autor.IndexOf(",") + 1;
                        int longitud = docDatos.FilaDocumento.Autor.Length - inicio;
                        pDocEditModel.Authors = docDatos.FilaDocumento.Autor.Substring(inicio, longitud).Trim();
                    }
                }
                else
                {
                    pDocEditModel.Authors = docDatos.FilaDocumento.Autor;
                }
            }

            pDocEditModel.ActualIdentityIsCreator = (Documento == null || Documento.FilaDocumento.CreadorID == IdentidadActual.Clave);

            if (Documento != null)
            {
                pDocEditModel.CreatorIsAuthor = Documento.FilaDocumento.CreadorEsAutor;
            }
            else
            {
                pDocEditModel.CreatorIsAuthor = docDatos.FilaDocumento.CreadorEsAutor;
            }

            //Tags título autoGenerados
            int numDescartados = 0;
            foreach (string tag in AnalizadorSintactico.ObtenerTagsFrase(docDatos.Titulo, out numDescartados))
            {
                string tagLimpio = ControladorDocumentacion.LimpiarPalabraParaTagGeneradoSegunTitulo(tag).ToLower();
                if (docDatos.ListaTagsSoloLectura.Contains(tagLimpio))
                {
                    pDocEditModel.AutomaticTagsTitle += $"[&]{tagLimpio}[&]";
                }
            }

            pDocEditModel.SharedDocumentJustInPrivateProject = ((Documento == null || Documento.BaseRecursos.Count == 1) && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado));

        }

        /// <summary>
        /// Prepara el control de tesauro.
        /// </summary>
        private void PrepararTesauro_ModificarRecurso()
        {
            Documento docDatos = Documento;

            if (Documento.TipoDocumentacion == TiposDocumentacion.Wiki && RequestParams("recuperarAutoGuardado") != null)
            {
                docDatos = DocumentoAutoGuardadoWiki;
            }

            mEditRecCont.ModifyResourceModel.ThesaurusEditorModel = new ThesaurusEditorModel();
            mEditRecCont.ModifyResourceModel.ThesaurusEditorModel.ThesaurusCategories = CargarTesauroPorGestorTesauro(GestorDocumental.GestorTesauro);

            ComprobarCategoriasDeshabilitadas(GestorDocumental.GestorTesauro, mEditRecCont.ModifyResourceModel.ThesaurusEditorModel);

            List<Guid> listaCatModelSeleccionadas = new List<Guid>();

            foreach (CategoriaTesauro categoria in docDatos.Categorias.Values)
            {
                if (mEntityContext.Entry(categoria.FilaCategoria).State != EntityState.Deleted)
                {
                    CategoryModel categoriaTesauro = new CategoryModel();
                    categoriaTesauro.Key = categoria.Clave;
                    categoriaTesauro.Name = categoria.FilaCategoria.Nombre;
                    categoriaTesauro.LanguageName = categoria.FilaCategoria.Nombre;
                    categoriaTesauro.Lang = UtilIdiomas.LanguageCode;

                    listaCatModelSeleccionadas.Add(categoria.Clave);
                }
            }

            mEditRecCont.ModifyResourceModel.ThesaurusEditorModel.SelectedCategories = listaCatModelSeleccionadas;
        }

        /// <summary>
        /// Acción para comprobar edición de un recurso.
        /// </summary>
        /// <returns></returns>
        [HttpGet, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public string ComprobarEdicion()
        {
            string urlFicha = string.Empty;

            string strDocId = RequestParams("documentoID");
            if (string.IsNullOrEmpty(strDocId))
            {
                strDocId = RequestParams("docID");
            }

            string strOntoId = RequestParams("ontologiaID");
            Guid? ontologiaID = null;

            if (!string.IsNullOrEmpty(strDocId))
            {
                Guid documentoID = new Guid(strDocId);

                if (!documentoID.Equals(Guid.Empty))
                {
                    if (!string.IsNullOrEmpty(strOntoId))
                    {
                        ontologiaID = new Guid(strOntoId);
                    }

                    List<Guid> listaDocumentos = new List<Guid>() { documentoID };
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    List<AD.EntityModel.Models.Documentacion.Documento> listaDocumentoDW = docCN.ObtenerDocumentosPorIDSoloDocumento(listaDocumentos);

                    foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in listaDocumentoDW)
                    {
                        if (filaDoc.FechaModificacion > DateTime.Now.AddMinutes(-1))
                        {
                            urlFicha = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, ontologiaID.ToString(), documentoID, ontologiaID, IdentidadActual.EsOrganizacion);
                        }
                    }
                }
            }

            return urlFicha;
        }

        #endregion

        #region Recurso Semántico

        /// <summary>
        /// Método inicial de modificar recurso Semántico.
        /// </summary>
        /// <returns>Acción resultante</returns>
        private ActionResult Index_ModificarRecursoSemantico()
        {
            mLoggingService.AgregarEntrada("TiemposMVC_Index_ModificarRecursoSemantico");
            TipoPagina = TiposPagina.BaseRecursos;

            CargarInicial_ModificarRecursoSemantico();
            ActionResult redireccion = ComprobarRedirecciones_ModificarRecursoSemantico();

            if (redireccion != null)
            {
                return redireccion;
            }

            ModifyResourceModel modelModRec = new ModifyResourceModel();
            modelModRec.EditResourceModel = mEditRecCont;
            mEditRecCont.ModifyResourceModel = modelModRec;

            if (EditandoFormSem)
            {
                mEditRecCont.TypePage = EditResourceModel.TypePageEditResource.EditSemanticResource;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASESUBIR", "EDITARRECURSOCREADO");
            }
            else
            {
                mEditRecCont.TypePage = EditResourceModel.TypePageEditResource.CreateSemanticResource;
                mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("PERFILBASESUBIR", "CREARRECURSO");
            }

            mTipoDocumento = TiposDocumentacion.Semantico;
            modelModRec.DocumentType = ResourceModel.DocumentType.Semantico;
            mEditRecCont.ModifyResourceModel.DocumentEditionModel = new DocumentEditionModel();
            mEditRecCont.ModifyResourceModel.DocumentEditionModel.Key = mDocumentoID;
            mEditRecCont.ModifyResourceModel.DocumentEditionModel.ActualIdentityIsCreator = true;
            if (CreandoFormSem || CargaMasivaFormSem)
            {
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.Key = Guid.Empty;
            }

            mEditRecCont.ModifyResourceModel.EditPropertiesAvailable = true;
            mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = false;
            mEditRecCont.ModifyResourceModel.DocumentEditionModel.ModificationProtectionAvailable = true;//Para que no se pinte en este caso.


            mEditRecCont.ModifyResourceModel.DocumentEditionModel.SharedDocumentJustInPrivateProject = ((Documento == null || Documento.BaseRecursos.Count == 1) && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado));

            mEditRecCont.ModifyResourceModel.SemanticResourceModel = new SemanticResourceModel();
            mEditRecCont.ModifyResourceModel.SemanticResourceModel.MassiveResourceLoad = CargaMasivaFormSem;
            mEditRecCont.ModifyResourceModel.SemanticResourceModel.EditingMassiveResourceLoad = mEditandoRecursoCargaMasiva;

            mEditRecCont.ModifyResourceModel.CreatingVersion = CreandoVersionFormSem;
            mEditRecCont.ModifyResourceModel.IsImprovement = ComprobarSiSeEstaEditandoUnaMejora(mDocumentoID);
			mEditRecCont.ModifyResourceModel.DocumentEditionModel.Draft = Documento == null || Documento.FilaDocumento.Borrador;
            mEditRecCont.ModifyResourceModel.SemanticResourceModel.VirtualForm = FormSemVirtual;

            if (EditandoFormSem)
            {
                CargarDatosDocumento_ModificarRecursoSemantico();
            }
            else if (mEditandoRecursoCargaMasiva)
            {
                CargarDatosDocumento_CargaMasiva_ModificarRecursoSemantico();
            }

            #region Editores

            UsersSelectorModel selecEditModel = new UsersSelectorModel();
            mEditRecCont.ModifyResourceModel.UsersSelectorEditionModel = selecEditModel;

            UsersSelectorModel selecLectModel = new UsersSelectorModel();
            mEditRecCont.ModifyResourceModel.UsersSelectorReadingModel = selecLectModel;

            CargarEditoresRecurso(mEditRecCont.ModifyResourceModel.UsersSelectorEditionModel, mEditRecCont.ModifyResourceModel.UsersSelectorReadingModel);

            #endregion

            CrearFormulario_ModificarRecursoSemantico();

            PrepararTesauro_ModificarRecursoSemantico();

            if (FormSemVirtual)
            {
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.SemCmsContainsTitleAndDescription = true;
                mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = false;
                mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = false;
                mEditRecCont.ModifyResourceModel.CopyrightAvailable = false;
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.Draft = false;
            }

            if (mEditRecCont.ModifyResourceModel.SemanticResourceModel.SemCmsContainsTitleAndDescription)
            {
                mEditRecCont.ModifyResourceModel.EditTitleAvailable = false;
                mEditRecCont.ModifyResourceModel.EditDescriptionAvailable = false;
            }
            else
            {
                mEditRecCont.ModifyResourceModel.EditTitleAvailable = true;
                mEditRecCont.ModifyResourceModel.EditDescriptionAvailable = true;
            }

            string url = "";

            if (!EditandoFormSem) //Si estamos creando un recurso, le llevamos a la home
            {
                //Creando Recurso
                url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreProy);
            }
            else //Si estamos editando un recurso, le llevamos al recurso
            {
                //Editando Recurso
                url = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, NombreProy, "", Documento, (IdentidadOrganizacion != null));
            }

            mEditRecCont.ModifyResourceModel.UrlCancelButton = url;

            if (CargaMasivaFormSem)
            {
                mEditRecCont.ModifyResourceModel.UrlGoHomeButton = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreProy);
            }

            #region Propiedad intelectual

            mEditRecCont.ModifyResourceModel.DocumentEditionModel.AllowsLicense = true;
            PrepararLicencia();

            #endregion

            mEditRecCont.ModifyResourceModel.ShareAvailable = !(ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && !ParametrosGeneralesRow.CompartirRecursosPermitido);

            mEditRecCont.ModifyResourceModel.SemanticResourceModel.JcropAvailable = (mOntologia != null && mOntologia.ConfiguracionPlantilla.HayJcrop);

            return null;
        }

        /// <summary>
        /// Realiza la carga inicial para modificar un recurso semántico.
        /// </summary>
        public void CargarInicial_ModificarRecursoSemantico()
        {
            mOntologiaID = new Guid(RequestParams("ontologiaID"));
            mTipoDocumento = TiposDocumentacion.Semantico;

            if (EditandoFormSem)
            {
                CargarInicial_ModificarRecurso();
            }
            else
            {
                CargaInicial();
                CargaInicialTesauro();
            }

            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            Guid proyectoOntologiasID;

            if (!EditandoFormSem)
            {
                proyectoOntologiasID = mControladorBase.UsuarioActual.ProyectoID;
            }
            else
            {
                proyectoOntologiasID = GestorDocumental.ListaDocumentos[mDocumentoID].ProyectoID;
            }

            if (ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
            {
                proyectoOntologiasID = new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
            }

            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            DataWrapperDocumentacion docOntoDW = docCL.ObtenerOntologiasProyecto(proyectoOntologiasID, true);

            GestorDocumental.DataWrapperDocumentacion.Merge(docOntoDW);

            documentacionCN.Dispose();
            GestorDocumental.CargarDocumentos(false);
        }

        /// <summary>
        /// Comprueba si hay que redireccionar realizando la acción de modificar un recurso semántico.
        /// </summary>
        /// <returns></returns>
        private ActionResult ComprobarRedirecciones_ModificarRecursoSemantico()
        {
            ActionResult redireccion = null;

            if (!EditandoFormSem)
            {
                redireccion = ComprobarRedirecciones_SubirRecursoPart2();
            }
            else
            {
                redireccion = ComprobarRedirecciones_ModificarRecurso();
            }

            if (redireccion != null)
            {
                return redireccion;
            }

            //Comprobamos si se trata de un documento virtual
            if (FormSemVirtual == string.IsNullOrEmpty(UrlServicio) || (CargaMasivaFormSem && !GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.NombreElementoVinculado.Contains("cargasmultiples=true")))
            {
                return RedireccionarAPaginaNoEncontrada();
            }

            return null;
        }

        /// <summary>
        /// Carga los datos actuales del documento en la página.
        /// </summary>
        public void CargarDatosDocumento_ModificarRecursoSemantico()
        {
            CargarDatosDocumento_ModificarRecurso(mEditRecCont.ModifyResourceModel.DocumentEditionModel);

            //Imagen principal
            if (GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento.NombreCategoriaDoc != null && !GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento.NombreCategoriaDoc.Trim().Equals("") && GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento.NombreCategoriaDoc.ToLower().Contains(UtilArchivos.ContentImagenes.ToLower() + "/") && !GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento.NombreCategoriaDoc.ToLower().Contains("," + UtilArchivos.ContentOntologias.ToLower() + "/") && !GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento.NombreCategoriaDoc.ToLower().Contains("/" + UtilArchivos.ContentImgCapSemanticasAntiguo + "/") && !GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento.NombreCategoriaDoc.ToLower().Contains("/" + UtilArchivos.ContentImgCapSemanticas + "/"))
            {
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.ImageRepresentativeValue = GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento.NombreCategoriaDoc.Substring(GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento.NombreCategoriaDoc.LastIndexOf(",") + 1);

                if (mEditRecCont.ModifyResourceModel.SemanticResourceModel.ImageRepresentativeValue.Contains("|"))
                {
                    mEditRecCont.ModifyResourceModel.SemanticResourceModel.ImageRepresentativeValue = mEditRecCont.ModifyResourceModel.SemanticResourceModel.ImageRepresentativeValue.Split('|')[0];
                }

                mEditRecCont.ModifyResourceModel.SemanticResourceModel.ImageRepresentativeValue = $"doc,{mEditRecCont.ModifyResourceModel.SemanticResourceModel.ImageRepresentativeValue}|";
            }
        }

        /// <summary>
        /// Carga los datos del recurso del carga masiva que se está editando en la página.
        /// Parametros:
        /// 0: ID recurso, 1: Archivo, 2: Título, 3: Descripción, 4: Tags, 5: RDF, 6: Categorías, 7: CreatorIsAuthor, 8: Authors, 9: ImageRepresentativeValue,
        /// 10: License, 11: SpecificResourceEditors,12: ResourceEditors, 13: ResourceVisibility,14: ResourceReaders,15: ShareAllowed
        /// </summary>
        public void CargarDatosDocumento_CargaMasiva_ModificarRecursoSemantico()
        {
            mRdfRecursoEditadoCargaMasiva = mDatosRecursoEditadoCargaMasiva[5];

            DocumentEditionModel docEditModel = mEditRecCont.ModifyResourceModel.DocumentEditionModel;

            docEditModel.Key = new Guid(mDatosRecursoEditadoCargaMasiva[0]);
            mEditRecCont.ModifyResourceModel.SemanticResourceModel.EditingMassiveResourceID = docEditModel.Key;
            docEditModel.Title = mDatosRecursoEditadoCargaMasiva[2];
            docEditModel.Description = mDatosRecursoEditadoCargaMasiva[3];
            docEditModel.Tags = mDatosRecursoEditadoCargaMasiva[4];
            docEditModel.ShareAllowed = bool.Parse(mDatosRecursoEditadoCargaMasiva[15]);
            docEditModel.Authors = mDatosRecursoEditadoCargaMasiva[8];
            docEditModel.ActualIdentityIsCreator = true;
            docEditModel.CreatorIsAuthor = bool.Parse(mDatosRecursoEditadoCargaMasiva[7]);

            //Tags título autoGenerados
            int numDescartados = 0;
            foreach (string tag in AnalizadorSintactico.ObtenerTagsFrase(docEditModel.Title, out numDescartados))
            {
                string tagLimpio = ControladorDocumentacion.LimpiarPalabraParaTagGeneradoSegunTitulo(tag).ToLower();
                if (docEditModel.Tags.Contains(tagLimpio + ","))
                {
                    docEditModel.AutomaticTagsTitle += $"[&]{tagLimpio}[&]";
                }
            }

            docEditModel.SharedDocumentJustInPrivateProject = ((Documento == null || Documento.BaseRecursos.Count == 1) && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado));

            if (!string.IsNullOrEmpty(mDatosRecursoEditadoCargaMasiva[13]))
            {
                ResourceVisibility recVisibility = (ResourceVisibility)short.Parse(mDatosRecursoEditadoCargaMasiva[13]);
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.PrivateEditors = (recVisibility == ResourceVisibility.OnlyEditors || recVisibility == ResourceVisibility.SpecificReaders);
                bool puedeSeleccionarAbierto = ComprobarIdentidadPuedeSeleccionarVisibilidadAbierto(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);
                mEditRecCont.ModifyResourceModel.OpenResourcesAvailable = ProyectoSeleccionado.EsPublico && puedeSeleccionarAbierto;
                mEditRecCont.ModifyResourceModel.VisibilityMembersCommunity = (recVisibility == ResourceVisibility.CommunityMembers || !ProyectoSeleccionado.EsPublico || !puedeSeleccionarAbierto);
            }
        }

        /// <summary>
        /// Carga los editores de un recurso temporal de una carga masiva.
        /// </summary>
        /// <param name="pSelectorEditores">Modelo de selector de editores del recurso</param>
        /// <param name="pSelectorLectores">Modelo de selector de lectores del recurso</param>
        private void CargarEditoresRecursoTemp_CargaMasiva(UsersSelectorModel pSelectorEditores, UsersSelectorModel pSelectorLectores)
        {
            string txtEditores = mDatosRecursoEditadoCargaMasiva[12];
            string txtLectores = mDatosRecursoEditadoCargaMasiva[14];

            string perfilesTexto = txtEditores + txtLectores;
            string[] separadores = { " & ", "," };
            string[] perfilesYgrupos = perfilesTexto.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

            List<Guid> listaPerfilesURlSem = new List<Guid>();
            List<Guid> listaGrupos = new List<Guid>();

            foreach (string editorID in perfilesYgrupos)
            {
                if (editorID.StartsWith("g_"))
                {
                    if (!listaGrupos.Contains(new Guid(editorID.Replace("g_", ""))))
                    {
                        listaGrupos.Add(new Guid(editorID.Replace("g_", "")));
                    }
                }
                else
                {
                    if (!listaPerfilesURlSem.Contains(new Guid(editorID)))
                    {
                        listaPerfilesURlSem.Add(new Guid(editorID));
                    }
                }
            }

            //Cargo editores
            if (listaPerfilesURlSem.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperIdentidad identDw = new DataWrapperIdentidad();

                if (listaPerfilesURlSem.Count > 0)
                {
                    identDw.Merge(identidadCN.ObtenerIdentidadesDePerfiles(listaPerfilesURlSem));
                }

                GestionIdentidades gestorIdent = new GestionIdentidades(identDw, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();

                if (GestorDocumental.GestorIdentidades == null)
                {
                    GestorDocumental.GestorIdentidades = gestorIdent;
                }
                else
                {
                    GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(gestorIdent.DataWrapperIdentidad);
                    GestorDocumental.GestorIdentidades.RecargarHijos();
                }
            }

            //Cargo los grupos de los editores
            if (listaGrupos.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperIdentidad identDW = identidadCN.ObtenerGruposPorIDGrupo(listaGrupos);

                GestionIdentidades gestorIdent = new GestionIdentidades(identDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();

                if (GestorDocumental.GestorIdentidades == null)
                {
                    GestorDocumental.GestorIdentidades = gestorIdent;
                }
                else
                {
                    GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(gestorIdent.DataWrapperIdentidad);
                    GestorDocumental.GestorIdentidades.RecargarHijos();
                }
            }

            foreach (string editorID in perfilesYgrupos)
            {
                if (editorID.StartsWith("g_"))
                {
                    Guid grupoID = new Guid(editorID.Replace("g_", ""));
                    if (!GestorDocumental.GestorIdentidades.ListaGrupos.ContainsKey(grupoID))
                    {
                        continue;
                    }

                    string nombreGrupo = GestorDocumental.GestorIdentidades.ListaGrupos[grupoID].Nombre;
                    bool esEditor = (txtEditores.Contains(editorID));

                    if (esEditor)
                    {
                        pSelectorEditores.SelectedGroupsList.Add(grupoID, nombreGrupo);
                    }
                    else
                    {
                        pSelectorLectores.SelectedGroupsList.Add(grupoID, nombreGrupo);
                    }
                }
                else
                {
                    Guid claveEditor = new Guid(editorID);
                    string nombreEditor = null;

                    if (!GestorDocumental.GestorIdentidades.ListaPerfiles.ContainsKey(claveEditor))
                    {
                        continue;
                    }

                    Guid identidadID = GestorDocumental.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(claveEditor)).Select(x => x.IdentidadID).FirstOrDefault();

                    if (!identidadID.Equals(Guid.Empty))
                    {
                        nombreEditor = ObtenerNombreIdentidad(GestorDocumental.GestorIdentidades.ListaIdentidades[identidadID]);
                    }


                    bool esEditor = (txtEditores.Contains(editorID));

                    if (nombreEditor != null)
                    {
                        if (esEditor)
                        {
                            pSelectorEditores.SelectedProfilesList.Add(claveEditor, nombreEditor);
                        }
                        else
                        {
                            pSelectorLectores.SelectedProfilesList.Add(claveEditor, nombreEditor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Crea el formulario semántico.
        /// </summary>
        private void CrearFormulario_ModificarRecursoSemantico()
        {
            string nombreOntologia = GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.Enlace;
            mNamespaceOntologia = GestionOWL.NAMESPACE_ONTO_GNOSS;
            mUrlOntologia = $"{BaseURLFormulariosSem}/Ontologia/{nombreOntologia}#";

            Dictionary<string, List<EstiloPlantilla>> listaEstilos = null;
            byte[] arrayOnto = null;

            try
            {
                arrayOnto = ControladorDocumentacion.ObtenerOntologia(mOntologiaID, out listaEstilos, ProyectoSeleccionado.Clave);
            }
            catch (Exception ex)
            {
                GuardarMensajeErrorAdmin($"Error al leer el XML de la ontología con nombre {nombreOntologia} y con ID {mOntologiaID}:", ex);
                throw;
            }

            if (arrayOnto == null)
            {
                throw new ExcepcionGeneral("No ha sido posible gererar el formulario porque el array de la ontología es nulo");
            }

            try
            {
                mOntologia = new Ontologia(arrayOnto, true);
                mOntologia.LeerOntologia();
                mOntologia.EstilosPlantilla = listaEstilos;
                mOntologia.IdiomaUsuario = IdiomaUsuario;
                mOntologia.OntologiaID = mOntologiaID;
            }
            catch (Exception ex)
            {
                GuardarMensajeErrorAdmin($"La ontología {nombreOntologia} con ID {mOntologiaID} no es correcta:", ex);
                throw;
            }

            if (mOntologia.ConfiguracionPlantilla.ListaIdiomas.Count == 0)
            {
                mOntologia.ConfiguracionPlantilla.ListaIdiomas.Add(IdiomaUsuario);
            }

            mEditRecCont.ModifyResourceModel.CopyrightAvailable = !mOntologia.ConfiguracionPlantilla.OcultarAutoria;
            mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = true;

            if (!EditandoFormSem && mOntologia.ConfiguracionPlantilla.GruposEditoresFijos != null)
            {
                mEditRecCont.ModifyResourceModel.SelectorEditionAvailable = false;

                mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = (mOntologia.ConfiguracionPlantilla.TipoVisiblidadEdicionRec == null);
            }

            if (mOntologia.ConfiguracionPlantilla.GruposEditoresPrivacidad != null)
            {
                string idsEditores = ObtenerIDsGrupoPorNombreCorto(mOntologia.ConfiguracionPlantilla.GruposEditoresPrivacidad).Replace("g_", "");
                string[] listaIDGruposEditarPrivacidad = idsEditores.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.Clave, true));
                if (IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalCorporativo || IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalPersonal)
                {
                    IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.IdentidadMyGNOSS.Clave, true));
                }

                IdentidadActual.GestorIdentidades.CargarGrupos();
                identidadCN.Dispose();

                bool identidadActualPerteneceAAlgunGrupo = false;
                foreach (string grupoID in listaIDGruposEditarPrivacidad.Where(item => IdentidadActual.GestorIdentidades.ListaGrupos.ContainsKey(new Guid(item))))
                {
                    identidadActualPerteneceAAlgunGrupo = true;
                }

                if (!identidadActualPerteneceAAlgunGrupo)
                {
                    mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = false;
                    mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = false;
                }
            }

            if (mOntologia.ConfiguracionPlantilla.OcultarBloquePrivacidadSeguridadEdicion)
            {
                mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = false;
            }

            if (mOntologia.ConfiguracionPlantilla.OcultarBloqueCompartirEdicion)
            {
                mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = false;
            }

            if (mOntologia.ConfiguracionPlantilla.OcultarBloquePropiedadIntelectualEdicion)
            {
                mEditRecCont.ModifyResourceModel.CopyrightAvailable = false;
            }

            if (CargaMasivaFormSem && mOntologia.ConfiguracionPlantilla.PropiedadArchivoCargaMasiva.Key == null)
            {
                foreach (string nombreEstiloPlan in mOntologia.EstilosPlantilla.Keys)
                {
                    foreach (EstiloPlantilla estiloPlan in mOntologia.EstilosPlantilla[nombreEstiloPlan])
                    {
                        if (estiloPlan is EstiloPlantillaEspecifProp && ((EstiloPlantillaEspecifProp)estiloPlan).TieneValor_TipoCampo && (((EstiloPlantillaEspecifProp)estiloPlan).TipoCampo == TipoCampoOntologia.Archivo || ((EstiloPlantillaEspecifProp)estiloPlan).TipoCampo == TipoCampoOntologia.Imagen || ((EstiloPlantillaEspecifProp)estiloPlan).TipoCampo == TipoCampoOntologia.ArchivoLink))
                        {
                            mOntologia.ConfiguracionPlantilla.PropiedadArchivoCargaMasiva = new KeyValuePair<string, string>(((EstiloPlantillaEspecifProp)estiloPlan).NombreRealPropiedad, ((EstiloPlantillaEspecifProp)estiloPlan).NombreEntidad);
                            break;
                        }
                    }

                    if (mOntologia.ConfiguracionPlantilla.PropiedadArchivoCargaMasiva.Key != null)
                    {
                        break;
                    }
                }
            }

            Dictionary<string, List<string>> propiedadesOmitirPintado = new Dictionary<string, List<string>>();

            if (mOntologia.ConfiguracionPlantilla.HayEntidadesSeleccEditables)
            {
                ObtenerSemCmsController(mDocumentoID).FusionarOntologiasYXMLEntExtEditables(mOntologia, ProyectoSeleccionado.Clave, propiedadesOmitirPintado, ParametroProyecto);
            }

            SemCmsController.ApanyarRepeticionPropiedades(mOntologia.ConfiguracionPlantilla, mOntologia.Entidades);
            List<ElementoOntologia> instanciasPrincipales = null;

            if (EditandoFormSem)
            {
                string rdfTexto = null;
                instanciasPrincipales = ObtenerRdfDeDocumento_ModificarRecursoSemantico(out rdfTexto);
                mSemController = new SemCmsController(mEditRecCont.ModifyResourceModel.SemanticResourceModel, mOntologia, Documento, instanciasPrincipales, ProyectoSeleccionado, IdentidadActual, UtilIdiomas, BaseURL, BaseURLIdioma, BaseURLContent, BaseURLStatic, UrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SemCmsController>(), mLoggerFactory);
                mSemController.ResourceRDF = rdfTexto;
            }
            else
            {
                //Comprobamos que el documento no existe ya:
                if (Documento != null)
                {
                    throw new ExcepcionGeneral("El documento ya existe");
                }

                Guid docFormID = mDocumentoID;


                if (mEditandoRecursoCargaMasiva)
                {
                    docFormID = mEditRecCont.ModifyResourceModel.DocumentEditionModel.Key;
                    string rdfTexto = null;
                    instanciasPrincipales = ObtenerRdfDeDocumento_ModificarRecursoSemantico(out rdfTexto);
                    mSemController.ResourceRDF = rdfTexto;
                }

                mSemController = new SemCmsController(mEditRecCont.ModifyResourceModel.SemanticResourceModel, mOntologia, docFormID, instanciasPrincipales, ProyectoSeleccionado, IdentidadActual, UtilIdiomas, BaseURL, BaseURLIdioma, BaseURLContent, BaseURLStatic, UrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SemCmsController>(), mLoggerFactory);
            }

            if (mOntologia.ConfiguracionPlantilla.HayEntidadesSeleccEditables)
            {
                string imgRepValue = mEditRecCont.ModifyResourceModel.SemanticResourceModel.ImageRepresentativeValue;
                ObtenerSemCmsController(mDocumentoID).FuncionarRDFsEntidadExternaEditable(mSemController.Entidades, mOntologia, ProyectoSeleccionado.Clave, mDocumentoID, BaseURLFormulariosSem, UrlIntragnoss, out mEntidadesExtEditablesDocID, IdentidadActual.Persona.FilaPersona, ProyectoSeleccionado.FilaProyecto, ref imgRepValue);
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.ImageRepresentativeValue = imgRepValue;
            }

            if (mOntologia.ConfiguracionPlantilla.MultiIdioma && ParametrosGeneralesRow.IdiomaDefecto != null && !string.IsNullOrEmpty(ParametrosGeneralesRow.IdiomaDefecto))
            {//Es multiidioma:
                ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                mSemController.IdiomaDefecto = ParametrosGeneralesRow.IdiomaDefecto;
                mSemController.IdiomasDisponibles = paramCL.ObtenerListaIdiomasDictionary();
            }

            if (GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.VersionFotoDocumento.HasValue && GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.VersionFotoDocumento > 0)
            {
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.OntologyCSS = BaseURLContent + "/" + UtilArchivos.ContentOntologias + "/Archivos/" + mOntologiaID.ToString().Substring(0, 3) + "/" + mOntologiaID + ".css?v=" + GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.VersionFotoDocumento;
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.OntologyJS = BaseURLContent + "/" + UtilArchivos.ContentOntologias + "/Archivos/" + mOntologiaID.ToString().Substring(0, 3) + "/" + mOntologiaID + ".js?v=" + GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.VersionFotoDocumento;
            }

            mSemController.EditandoRecursoCargaMasiva = mEditandoRecursoCargaMasiva;
            mSemController.PropiedadesOmitirPintado = propiedadesOmitirPintado;
            mSemController.EntidadesExtEditablesDocID = mEntidadesExtEditablesDocID;

            if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma))
            {
                mSemController.PropiedadIdiomaBusquedaComunidad = ParametroProyecto[Es.Riam.Gnoss.AD.Parametro.ParametroAD.PropiedadContenidoMultiIdioma];
            }

            try
            {
                mSemController.ObtenerModeloSemCMSEdicion(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave));
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(mEditRecCont.ModifyResourceModel.SemanticResourceModel.AdminGenerationError) || !ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Obtiene el RDF del documento de la BD, y devuele las entidades pricipales del mismo.
        /// </summary>
        /// <param name="rdfTexto">Texto con el RDF del recurso</param>
        /// <returns>Lista con las entidades pricipales leidas del archivo RDF leido de la BD y generado</returns>
        private List<ElementoOntologia> ObtenerRdfDeDocumento_ModificarRecursoSemantico(out string pRdfTexto)
        {
            GestionOWL gestorOWL = new GestionOWL();
            gestorOWL.UrlOntologia = mUrlOntologia;
            gestorOWL.NamespaceOntologia = mNamespaceOntologia;

            string rdfTexto = null;

            if (GestorDocumental.ListaDocumentos.ContainsKey(mDocumentoID))
            {
                #region Obtengo RDF almacenado del recurso

                GestorDocumental.RdfDS = ControladorDocumentacion.ObtenerRDFDeBDRDF(mDocumentoID, Documento.FilaDocumento.ProyectoID.Value);

                if (GestorDocumental.RdfDS.RdfDocumento.Count > 0)
                {
                    rdfTexto = Documento.RdfSemantico;

                    if (!rdfTexto.Contains(mNamespaceOntologia + ":") && !rdfTexto.Contains("xmlns:" + mNamespaceOntologia + "="))
                    {
                        try
                        {
                            if (rdfTexto.Contains("=\"" + mUrlOntologia + "\""))
                            {
                                string namespaceGuardado = rdfTexto.Substring(0, rdfTexto.IndexOf("=\"" + mUrlOntologia + "\""));
                                namespaceGuardado = namespaceGuardado.Substring(namespaceGuardado.LastIndexOf("xmlns:") + 6);
                                rdfTexto = rdfTexto.Replace("xmlns:" + namespaceGuardado + "=", "xmlns:" + mNamespaceOntologia + "=");
                                rdfTexto = rdfTexto.Replace("<" + namespaceGuardado + ":", "<" + mNamespaceOntologia + ":").Replace("</" + namespaceGuardado + ":", "</" + mNamespaceOntologia + ":");
                            }
                            else
                            {
                                //Que vaya a virtuoso
                                rdfTexto = null;
                            }
                        }
                        catch (Exception)
                        {//Que vaya a virtuoso
                            rdfTexto = null;
                        }
                    }
                }

                if (rdfTexto == null)
                {
                    if (GestorDocumental.RdfDS.RdfDocumento.Count == 0)
                    {
                        GestorDocumental.RdfDS = null;
                    }

                    byte[] rdfVirtuoso = ControladorDocumentacion.ObtenerRDFDeVirtuoso(mDocumentoID, GestorDocumental.ListaDocumentos[mOntologiaID].Enlace, UrlIntragnoss, mUrlOntologia, mNamespaceOntologia, mOntologia);

                    if (rdfVirtuoso.Length == 0)
                    {
                        GuardarMensajeErrorAdmin("El recurso " + Documento.Clave + " no tiene datos en virtuoso.", null);
                        throw new ExcepcionGeneral("El recurso " + Documento.Clave + " no tiene datos en virtuoso.");
                    }

                    MemoryStream buffer = new MemoryStream(rdfVirtuoso);


                    StreamReader reader = new StreamReader(buffer);
                    rdfTexto = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }

                #endregion
            }
            else //Obtengo el documento del RDF temporal
            {
                rdfTexto = mRdfRecursoEditadoCargaMasiva;
            }

            List<ElementoOntologia> instanciasPrincipales = null;

            try
            {
                instanciasPrincipales = gestorOWL.LeerFicheroRDF(mOntologia, rdfTexto, true);

                if (mEditandoRecursoCargaMasiva)
                {
                    if (mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key != null)
                    {
                        Propiedad propTit = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key, mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Value, instanciasPrincipales);
                        propTit.LimpiarValor();
                        propTit.DarValor(UtilCadenas.ObtenerTextoDeIdioma(mEditRecCont.ModifyResourceModel.DocumentEditionModel.Title, UtilIdiomas.LanguageCode, null), null);
                    }

                    if (mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key != null)
                    {
                        Propiedad propDesc = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key, mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value, instanciasPrincipales);
                        propDesc.LimpiarValor();
                        propDesc.DarValor(UtilCadenas.ObtenerTextoDeIdioma(mEditRecCont.ModifyResourceModel.DocumentEditionModel.Description, UtilIdiomas.LanguageCode, null), null);
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarMensajeErrorAdmin("El RDF del recurso " + mDocumentoID + " no es correcto. ", ex);
                throw;
            }

            pRdfTexto = rdfTexto;
            return instanciasPrincipales;
        }

        /// <summary>
        /// Prepara el tesauro del recurso semántico.
        /// </summary>
        public void PrepararTesauro_ModificarRecursoSemantico()
        {
            if (FormSemVirtual)
            {
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.ThesaurusCategoryNotRequired = true;
            }
            else if (EditandoFormSem && Documento.FilaDocumentoWebVinBR.TipoPublicacion == (short)TipoPublicacion.CompartidoAutomatico)
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                string nombreCortoProyPubli = proyCL.ObtenerNombreCortoProyecto(Documento.ProyectoID);
                proyCL.Dispose();

                mEditRecCont.ModifyResourceModel.SemanticResourceModel.ThesaurusCategoryNotRequired = true;
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.OtherCommunityEditCategoriesUrl = mControladorBase.UrlsSemanticas.GetURLBaseRecursosVerDocumentoCreado(BaseURLIdioma, UtilIdiomas, nombreCortoProyPubli, UrlPerfil, Documento.Clave, mOntologiaID.ToString(), true, (IdentidadOrganizacion != null));
            }
            else if (!mOntologia.ConfiguracionPlantilla.CategorizacionTesauroGnossNoObligatoria && !mOntologia.ConfiguracionPlantilla.OcultarTesauro)
            {
                if (mOntologia.ConfiguracionPlantilla.CategoriasPorDefecto != null)
                {
                    try
                    {
                        foreach (string categoria in mOntologia.ConfiguracionPlantilla.CategoriasPorDefecto)
                        {
                            Guid categoriaID = Guid.Empty;
                            Guid.TryParse(categoria, out categoriaID);

                            if (categoriaID == Guid.Empty)
                            {
                                categoriaID = GestorDocumental.GestorTesauro.ObtenerCategoriasIDPorNombre(categoria).FirstOrDefault();
                            }

                            foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauro fila in GestorDocumental.GestorTesauro.TesauroDW.ListaCategoriaTesauro.Where(item => item.CategoriaTesauroID.Equals(categoriaID)).ToList())
                            {
                                GestorDocumental.GestorTesauro.TesauroDW.ListaCategoriaTesauro.Remove(fila);
                                mEntityContext.EliminarElemento(fila);
                            }

                            foreach (AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro fila in GestorDocumental.GestorTesauro.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaInferiorID.Equals(categoriaID) || item.CategoriaSuperiorID.Equals(categoriaID)))
                            {
                                GestorDocumental.GestorTesauro.TesauroDW.ListaCatTesauroAgCatTesauro.Remove(fila);
                                mEntityContext.EliminarElemento(fila);
                            }
                        }

                        GestorDocumental.GestorTesauro.CargarCategorias();
                    }
                    catch (Exception ex)
                    {
                        GuardarMensajeErrorAdmin("La sección 'CategoriasPorDefecto' tiene alguna categoría mal configurada.", ex);
                        throw;
                    }
                }

                if (EditandoFormSem)
                {
                    PrepararTesauro_ModificarRecurso();
                }
                else
                {
                    PrepararTesauro_SubirRecursoPart2();
                }
            }
            else
            {
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.ThesaurusCategoryNotRequired = true;
            }
        }

        /// <summary>
        /// Guarda el mensaje de error para el administrador.
        /// </summary>
        /// <param name="pMensaje">Mensaje</param>
        /// <param name="ex">Excepción</param>
        private void GuardarMensajeErrorAdmin(string pMensaje, Exception ex)
        {
            if (string.IsNullOrEmpty(mEditRecCont.ModifyResourceModel.SemanticResourceModel.AdminGenerationError))
            {
                mEditRecCont.ModifyResourceModel.SemanticResourceModel.AdminGenerationError = pMensaje;

                if (ex != null)
                {
                    mEditRecCont.ModifyResourceModel.SemanticResourceModel.AdminGenerationError += Environment.NewLine + ex.Message;
                }
            }
        }

        /// <summary>
        /// Médodo para subir archivos a una carga masiva del SEMCMS.
        /// </summary>
        /// <param name="pModel">Modelo de subir archivo adjunto</param>
        /// <returns>Respuesta del callback</returns>
        private ActionResult GuardarArchivo_CargaMasiva_ModificarRecursoSemantico(AttachmentModel pModel)
        {
            if (pModel.File != null)
            {
                string directorio = Path.Combine(Request.Path, "documentosvirtuales/cargasmasivas/", mDocumentoID.ToString());

                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                string valorArch = pModel.FileName;

                if (valorArch.Contains("."))
                {
                    valorArch = valorArch.Substring(0, valorArch.LastIndexOf("."));
                    valorArch += "_" + Guid.NewGuid() + pModel.FileName.Substring(pModel.FileName.LastIndexOf("."));
                }
                else
                {
                    valorArch += "_" + Guid.NewGuid();
                }

                byte[] buffer1;

                using (BinaryReader reader1 = new BinaryReader(pModel.File.OpenReadStream()))
                {
                    buffer1 = reader1.ReadBytes((int)pModel.File.Length);
                    reader1.Close();
                }

                System.IO.File.WriteAllBytes(directorio + valorArch, buffer1);

                return Content(pModel.FileName);
            }

            return GnossResultERROR("");
        }

        /// <summary>
        /// Médodo para subir un archivo al SEMCMS.
        /// </summary>
        /// <param name="pModel">Modelo de subir archivo adjunto</param>
        /// <returns>Respuesta del callback</returns>
        private ActionResult GuardarArchivo_ModificarRecursoSemantico(AttachmentModel pModel)
        {
            if (pModel.File != null)
            {
                try
                {
                    InicializarModeloParaAccionSemCms(null);
                    string[] extraSemCms = pModel.ExtraSemCms.Split('|');

                    string fileUploadID = extraSemCms[1];
                    string especialID = extraSemCms[2];

                    int propImagen = 0;
                    int indexDocID = 3;
                    string idioma = null;

                    if (fileUploadID == "videofileUpLoad")
                    {
                        propImagen = 1;

                    }
                    else if (fileUploadID == "archivofileUpLoad" || fileUploadID == "archivoLinkfileUpLoad")
                    {
                        string valorProp = especialID;

                        if (extraSemCms[3].Contains("@"))
                        {
                            idioma = extraSemCms[3].Substring(extraSemCms[3].IndexOf("@") + 1) + "/";
                            extraSemCms[3] = extraSemCms[3].Substring(0, extraSemCms[3].IndexOf("@"));
                        }

                        if (fileUploadID == "archivofileUpLoad")
                        {
                            propImagen = 2;
                        }
                        else
                        {
                            propImagen = 3;
                        }

                        if (valorProp.Contains("."))
                        {
                            valorProp = valorProp.Substring(0, valorProp.LastIndexOf("."));

                            if (propImagen == 3)
                            {
                                valorProp = UtilCadenas.EliminarCaracteresUrlSem(valorProp);
                            }
                            else
                            {
                                valorProp = UtilCadenas.EliminarCaracetresInvalidosNombreArchivo(valorProp);
                            }

                            valorProp += "_" + extraSemCms[3] + especialID.Substring(especialID.LastIndexOf("."));
                        }
                        else
                        {
                            if (propImagen == 3)
                            {
                                valorProp = UtilCadenas.EliminarCaracteresUrlSem(valorProp);
                            }
                            else
                            {
                                valorProp = UtilCadenas.EliminarCaracetresInvalidosNombreArchivo(valorProp);
                            }

                            valorProp += "_" + extraSemCms[3];
                        }

                        especialID = idioma + valorProp;
                        indexDocID = 4;
                    }

                    byte[] buffer1;

                    using (BinaryReader reader1 = new BinaryReader(pModel.File.OpenReadStream()))
                    {
                        buffer1 = reader1.ReadBytes((int)pModel.File.Length);
                        reader1.Close();
                    }

                    string extensionArchivo = System.IO.Path.GetExtension(new FileInfo(pModel.FileName).Name);
                    string propiedad = extraSemCms[0];

                    string entidad = propiedad.Substring(propiedad.IndexOf(",") + 1);
                    string propiedadNombre = propiedad.Substring(0, propiedad.IndexOf(","));
                    bool usarJCROP = mOntologia.ConfiguracionPlantilla.EsPropiedadUsarJcrop(propiedadNombre, entidad);

                    Guid documentoID = DocumentoVersionID != Guid.Empty ? DocumentoVersionID : mDocumentoID;

                    if (extraSemCms.Length > indexDocID)
                    {
                        documentoID = new Guid(extraSemCms[indexDocID]);
                    }

                    if (usarJCROP && propImagen == 0)
                    {
                        ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                        servicioImagenes.Url = UrlIntragnossServicios;

                        bool correcto = servicioImagenes.AgregarImagenADirectorio(buffer1, Path.Combine(UtilArchivos.ContentImagenesDocumentos, UtilArchivos.ContentImagenesSemanticas, "temp", UtilArchivos.DirectorioDocumento(documentoID)), especialID, extensionArchivo);

                        if (!correcto)
                        {
                            throw new ExcepcionGeneral("Archivo no subido");
                        }
                    }
                    else
                    {
                        if (!AgregarArchivoAServicio_SemCms(documentoID, propImagen, especialID, buffer1, extensionArchivo, propiedad))
                        {
                            throw new ExcepcionGeneral("Archivo no subido");
                        }
                    }

                    string respuesta = "OK";

                    if (propImagen == 0 && mOntologia.ConfiguracionPlantilla.PropiedadesOpenSeaDragon != null && mOntologia.ConfiguracionPlantilla.PropiedadesOpenSeaDragon.Contains(new KeyValuePair<string, string>(propiedadNombre, entidad)))
                    {
                        Image imagen = Image.Load(buffer1);
                        respuesta = "OpenSeaDragon|" + imagen.Width + "|" + imagen.Height;
                        imagen.Dispose();
                    }
                    else if (propImagen == 3 || propImagen == 2) //Devolvemos el nombre definitivo del archivo.
                    {
                        respuesta = especialID;

                        if (!string.IsNullOrEmpty(idioma) && propImagen == 2)
                        {
                            respuesta = respuesta.Replace(idioma, "");
                        }
                    }

                    return Content(respuesta);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    //Se han producido errores al guardar el documento en el servidor
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        return GnossResultERROR(ex.Message);
                    }
                    else
                    {
                        return GnossResultERROR();
                    }
                }
            }

            return GnossResultERROR();
        }

        /// <summary>
        /// Agrega un archivo subido al servicio correcto.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pPropImagen">Indica de que tipo es el archivo subido</param>
        /// <param name="pEspecialID">ID del archivo</param>
        /// <param name="pBuffer1">Buffer con el archivo</param>
        /// <param name="pExtensionArchivo">Extensión del archivo</param>
        /// <param name="pPropiedad">Propiedad del archivo</param>
        /// <returns>TRUE si ha ido bien, FALSE en caso contrario</returns>
        private bool AgregarArchivoAServicio_SemCms(Guid pDocumentoID, int pPropImagen, string pEspecialID, byte[] pBuffer1, string pExtensionArchivo, string pPropiedad)
        {
            bool correcto = false;

            if (FormSemVirtual)
            {
                string ruta = Path.Combine(mEnv.WebRootPath, Request.Path, $"documentosvirtuales/{mOntologiaID}/{pDocumentoID}");

                if (pPropImagen == 3 || pPropImagen == 0)
                {
                    ruta = Path.Combine(mEnv.WebRootPath, Request.Path, $"documentosvirtuales/{mOntologiaID}/{UtilArchivos.DirectorioDocumento(pDocumentoID)}");
                }

                if (pPropImagen == 2 || pPropImagen == 3)
                {
                    pExtensionArchivo = "";
                }

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                System.IO.File.WriteAllBytes(ruta + pEspecialID + pExtensionArchivo, pBuffer1);

                return true;
            }
            else if (pPropImagen == 0)
            {
                Image imagen = Image.Load(new MemoryStream(pBuffer1));

                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                servicioImagenes.Url = UrlIntragnossServicios;

                correcto = servicioImagenes.AgregarImagenADirectorio(pBuffer1, Path.Combine(UtilArchivos.ContentImagenesDocumentos, UtilArchivos.ContentImagenesSemanticas, UtilArchivos.DirectorioDocumento(pDocumentoID)), pEspecialID, pExtensionArchivo);

                if (correcto)
                {
                    string entidad = pPropiedad.Substring(pPropiedad.IndexOf(",") + 1);
                    pPropiedad = pPropiedad.Substring(0, pPropiedad.IndexOf(","));

                    entidad = ElementoOntologia.ObtenerTiposEntidadLimpiaDeApanioRepeticiones(entidad);

                    if (mOntologia.ConfiguracionPlantilla.EsPropiedadImagenMini(pPropiedad, entidad))
                    {
                        correcto = CapturarImgMiniPropiedad_SemCms(pPropiedad, entidad, imagen, pEspecialID, servicioImagenes, pDocumentoID, pExtensionArchivo);
                    }
                }

                imagen.Dispose();
            }
            else if (pPropImagen == 1)
            {
                ServicioVideos servicioVideos = new ServicioVideos(mConfigService, mLoggingService, mLoggerFactory.CreateLogger<ServicioVideos>(), mLoggerFactory);
                correcto = servicioVideos.AgregarVideoSemantico(pBuffer1, pExtensionArchivo, pDocumentoID, new Guid(pEspecialID)) == 1;
            }
            else if (pPropImagen == 2)
            {
                string entidad = pPropiedad.Substring(pPropiedad.IndexOf(",") + 1);
                pPropiedad = pPropiedad.Substring(0, pPropiedad.IndexOf(","));
                string idioma = "";

                if (pEspecialID.Contains("/"))
                {
                    idioma = Path.DirectorySeparatorChar + pEspecialID.Substring(0, pEspecialID.IndexOf("/"));
                    pEspecialID = pEspecialID.Substring(pEspecialID.IndexOf("/") + 1);
                }

                GestionDocumental gestionDoc = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                gestionDoc.Url = UrlServicioWebDocumentacion;

                string directorio = Path.Combine(UtilArchivos.ContentDocumentosSem, UtilArchivos.DirectorioDocumento(pDocumentoID)) + idioma;
                Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                string idAuxGestorDocumental = gestionDoc.AdjuntarDocumentoADirectorio(pBuffer1, directorio, pEspecialID.Substring(0, pEspecialID.LastIndexOf(".")), pEspecialID.Substring(pEspecialID.LastIndexOf(".")));
                correcto = (!idAuxGestorDocumental.ToUpper().Equals("ERROR"));
                mLoggingService.AgregarEntradaDependencia("Agregar archivo a gestor documental", false, "AgregarArchivoAServcicio_SemCMS", sw, true);

                ElementoOntologia entidadArchivo = mOntologia.GetEntidadTipo(entidad, false);
                entidadArchivo.ObtenerPropiedad(pPropiedad);
            }
            else if (pPropImagen == 3)
            {
                correcto = EnviarArchivoServicioDocumentosLink(pBuffer1, pDocumentoID, pEspecialID.Substring(0, pEspecialID.LastIndexOf(".")), pExtensionArchivo);
            }

            return correcto;
        }

        private bool EnviarArchivoServicioDocumentosLink(byte[] pBytes, Guid pDocumentoID, string pNombre, string pExtension)
        {
            bool exito = false;
            try
            {
                string peticion = $"{UrlIntragnossServicios}/DocumentosLink/add-document?pDocumentoID={pDocumentoID}&pNombre={pNombre}&pExtension={pExtension}";

                string respuesta = WebRequest("POST", peticion, pBytes);


                exito = respuesta.Equals("true");
            }
            catch (Exception ex)
            {
                GuardarLogError(ex.Message + " \r\nPila: " + ex.StackTrace);
                ServicioVideos servicioDocsLink = new ServicioVideos(mConfigService, mLoggingService, mLoggerFactory.CreateLogger<ServicioVideos>(), mLoggerFactory);
                exito = servicioDocsLink.AgregarDocumento(pBytes, pDocumentoID, pNombre, pExtension);
            }

            return exito;
        }

        /// <summary>
        /// Realiza la captura mini de una imagen.
        /// </summary>
        /// <param name="pPropiedad">Propiedad de la imagen a capturar</param>
        /// <param name="pEntidad">Entidad que se está cargando</param>
        /// <param name="pImagen">Imagen a miniaturizar</param>
        /// <param name="pEspecialID">ID de la imagen</param>
        /// <param name="pServicioImagenes">Servicio de imágenes</param>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pExtensionArchivo">Extension del documento</param>
        /// <returns>TRUE si la captura se ha realizado correctamente, FALSE si no</returns>
        private bool CapturarImgMiniPropiedad_SemCms(string pPropiedad, string pEntidad, Image pImagen, string pEspecialID, ServicioImagenes pServicioImagenes, Guid pDocumentoID, string pExtensionArchivo)
        {
            bool correcto = true;
            ImagenMini imagenMini = mOntologia.ConfiguracionPlantilla.ObtenerTamanioPropiedadImagenMini(pPropiedad, pEntidad);

            foreach (KeyValuePair<int, int> sizes in imagenMini.Tamanios)
            {
                byte[] buffer1 = null;

                int ancho = sizes.Key;
                int alto = sizes.Value;
                int dimension = ancho;

                if (dimension == -1)
                {
                    dimension = alto;
                }

                //No hay tipo configurado para redimensionar
                if (!imagenMini.Tipo.ContainsKey(ancho))
                {
                    buffer1 = UtilImages.RedimensionarAnchoAlto(ancho, alto, pImagen);
                }
                else if (imagenMini.Tipo[ancho] == "RecorteCuadrado")
                {
                    buffer1 = UtilImages.RealizarRecorteCuadrado(dimension, pImagen);
                }

                if (buffer1 != null)
                {
                    correcto = pServicioImagenes.AgregarImagenADirectorio(buffer1, Path.Combine(UtilArchivos.ContentImagenesDocumentos, UtilArchivos.ContentImagenesSemanticas, UtilArchivos.DirectorioDocumento(pDocumentoID)), $"{pEspecialID}_{dimension}", pExtensionArchivo);
                }

                if (!correcto)
                {
                    break;
                }
            }

            return correcto;
        }

        /// <summary>
        /// Médodo para guardar un recurso.
        /// </summary>
        /// <param name="pModel">Argumentos</param>
        /// <returns>Respuesta acción</returns>
        private ActionResult GuardarRecurso_ModificarRecursoSemantico(DocumentEditionModel pModel)
        {
            InicializarModeloParaAccionSemCms(pModel);

            GestionarArchivosGoogleDrive();
            AjustarVisiblidadYEditoresSegunXml();
            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            docCL.InvalidarContextoRecursoMVC(pModel.Key, ProyectoSeleccionado.Clave);
            if (mErrorDocumento != null)
            {
                return GnossResultERROR(mErrorDocumento);
            }

            if (!FormSemVirtual)
            {
                return GuardarRDFRecursoSemantico();
            }
            else
            {
                return GuardarRDFRecursoSemantico_FormSemVirtual();
            }
        }

        /// <summary>
        /// Inicializa los modelos para realizar una acción sobre el SEMCMS.
        /// </summary>
        /// <param name="pModel">Argumentos</param>
        private void InicializarModeloParaAccionSemCms(DocumentEditionModel pModel)
        {
            mModelSaveRec = pModel;
            mEditRecCont = new EditResourceModel();
            mEditRecCont.ModifyResourceModel = new ModifyResourceModel();
            mEditRecCont.ModifyResourceModel.SemanticResourceModel = new SemanticResourceModel();
            mEditRecCont.ModifyResourceModel.SemanticResourceModel.MassiveResourceLoad = CargaMasivaFormSem;
            mEditRecCont.ModifyResourceModel.SemanticResourceModel.EditingMassiveResourceLoad = mEditandoRecursoCargaMasiva;
            mTipoDocumento = TiposDocumentacion.Semantico;
            mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = true;
            CrearFormulario_ModificarRecursoSemantico();
        }

        /// <summary>
        /// Guarda el RDF de un recurso semántico.
        /// </summary>
        /// <returns>Acción a realizar</returns>
        private ActionResult GuardarRDFRecursoSemantico()
        {
            List<ElementoOntologia> EntidadesDocAntiguas = mSemController.Entidades;
            string rdfDocAntiguo = mSemController.ResourceRDF;

            if (mOntologia.ConfiguracionPlantilla.HayEntidadesSeleccEditables)
            {
                RecuperarEntidadesExtEditablesDocID();
            }

            HashSet<string> rutasAdjuntos = ModificarTriplesAdjuntosRecursoSemantico(mModelSaveRec, IdentidadOrganizacion, (ProyectoSeleccionado.Clave != ProyectoAD.MyGnoss));

            mEntidadesGuardar = mSemController.RecogerValoresRdf(mModelSaveRec.RdfValue, mModelSaveRec.EntityIDRegisterInfo);

            if (CargaMasivaFormSem && !GuardandoRecursoCargaMasiva)
            {
                AgregarPropiedadArchivoTemporal(mEntidadesGuardar);

                if (mErrorDocumento != null)
                {
                    return GnossResultERROR(mErrorDocumento);
                }
            }

            AD.EntityModel.Models.Documentacion.ColaDocumento colaDocumentoRow = GuardarDatosPropiedadesEspeciales(mEntidadesGuardar);

            List<Guid> categoriasSeleccionadas = RecogerCategoriasSeleccionadas();

            string mensError = ValidarRecurso(mModelSaveRec.Draft, categoriasSeleccionadas);

            if (!string.IsNullOrEmpty(mensError))
            {
                return GnossResultERROR(mensError);
            }

            ActionResult repeticion = ComprobarRepeticionesConfiguradas(mEntidadesGuardar);

            if (repeticion != null)
            {
                return repeticion;
            }

            foreach(string rutaAdjunto in rutasAdjuntos)
            {
                bool correcto = ControladorDocumentacion.CopiarAdjuntoDocumentoSemantico(rutaAdjunto, mModelSaveRec.Key, DocumentoVersionID);

                if (!correcto)
                {
                    throw new Exception($"No se han podido duplicar los archivos para el nuevo documento.\n DocumentoID original: {mModelSaveRec.Key}\n DocumentoID Version: {DocumentoVersionID}");
                }
            }

            if (CargaMasivaFormSem && GuardandoRecursoCargaMasiva)
            {
                if (mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key != null)
                {
                    Propiedad propTit = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key, mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Value, mEntidadesGuardar);
                    propTit.LimpiarValor();
                    propTit.DarValor("[tituloTemporalCargaMasivagnoss]", null);
                }

                if (mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key != null)
                {
                    Propiedad propDesc = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key, mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value, mEntidadesGuardar);
                    propDesc.LimpiarValor();
                    propDesc.DarValor("[descripcionTemporalCargaMasivagnoss]", null);
                }
            }

            if (mOntologia.ConfiguracionPlantilla.HayEntidadesSeleccEditables)
            {
                ExtraerOntologiasExternasEntExtEditables(mEntidadesGuardar);
                BorrarRecsOntologiasExternasEntExtEditablesDescartados();
            }

            Stream streamRDF = ObtenerRDF();

            streamRDF.Position = 0;
            string ficheroRDF = new StreamReader(streamRDF).ReadToEnd();

            if (CargaMasivaFormSem)
            {
                ActionResult resultadoCargaMasiva = GuardadoCargaMasivaFormSem(ficheroRDF);

                return resultadoCargaMasiva;
            }

            bool servicioExterno = (PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.urlserviciocomplementario.ToString()) || PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString())) && !mModelSaveRec.Draft;

            string infoExtra = null;
            Guid docIDAuxiliar = CreandoVersion ? Documento.VersionOriginalID : mDocumentoID;

            bool esMejora = ComprobarSiSeEstaEditandoUnaMejora(DocumentoID);
            if (!esMejora)
            {
				mListaTriplesSemanticos = ControladorDocumentacion.GuardarRDFEnVirtuoso(mEntidadesGuardar, GestorDocumental.ListaDocumentos[mOntologiaID].Enlace, UrlIntragnoss, "", mControladorBase.UsuarioActual.ProyectoID, docIDAuxiliar.ToString(), false, infoExtra, mModelSaveRec.Draft, false, (short)PrioridadBase.Alta);
			}
            
            try
            {
                ControladorDocumentacion.GuardarRDFEnBDRDF(ficheroRDF, DocumentoVersionID == Guid.Empty ? mDocumentoID : DocumentoVersionID, mControladorBase.UsuarioActual.ProyectoID, GestorDocumental.RdfDS);
                ControladorDocumentacion.GuardarRDFEnBDRdfHistorico(ficheroRDF, DocumentoVersionID == Guid.Empty ? mDocumentoID : DocumentoVersionID, IdentidadCrearVersion.Clave);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $"Error al guardar modificaciones en BD RDF o BD RdfHistorico. Se van a revertir los cambios en virtuoso del recurso {mDocumentoID}");

                if (!esMejora)
                {
					if (!EditandoFormSem)
					{
						ControladorDocumentacion.BorrarRDFDeVirtuoso(mDocumentoID, GestorDocumental.ListaDocumentos[mOntologiaID].Enlace, UrlIntragnoss, false, ProyectoSeleccionado.Clave);
					}
					else
					{
						ControladorDocumentacion.GuardarRDFEnVirtuoso(EntidadesDocAntiguas, GestorDocumental.ListaDocumentos[mOntologiaID].Enlace, UrlIntragnoss, "", mControladorBase.UsuarioActual.ProyectoID, Documento.VersionOriginalID.ToString(), false, infoExtra, Documento.FilaDocumento.Borrador, false, (short)PrioridadBase.Alta);
					}
				}                

                if (ex.Message.Contains("PRIMARY KEY"))
                {
                    GuardarMensajeErrorAdmin("El ID con el que está guardando el recurso está siendo usado, vuelva a la página de añadir nuevo recurso e intentelo de nuevo.", ex);
                }

                throw;
            }

            if (!esMejora)
            {
				GuardarValoresInsertarEnGrafosAuxiliares();
			}
            

            ActionResult result = null;
            try
            {
                result = GuardarRecursoSemanticoenBD(categoriasSeleccionadas);

                CrearEventoColaMiniatura(colaDocumentoRow);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $"Error al guardar modificaciones en BD Ácida. Se van a revertir los cambios en Virtuoso y BD RDF del recurso {mDocumentoID}");

                try
                {
                    if (!EditandoFormSem)
                    {
                        if (!esMejora)
                        {
							ControladorDocumentacion.BorrarRDFDeVirtuoso(mDocumentoID, GestorDocumental.ListaDocumentos[mOntologiaID].Enlace, UrlIntragnoss, false, ProyectoSeleccionado.Clave);
						}                        
                        ControladorDocumentacion.BorrarRDFDeBDRDF(mDocumentoID);
                    }
                    else
                    {
						if (!esMejora)
						{
							ControladorDocumentacion.GuardarRDFEnVirtuoso(EntidadesDocAntiguas, GestorDocumental.ListaDocumentos[mOntologiaID].Enlace, UrlIntragnoss, "", mControladorBase.UsuarioActual.ProyectoID, mDocumentoID.ToString(), false, infoExtra, Documento.FilaDocumento.Borrador, false, (short)PrioridadBase.Alta);
						}						
                        ControladorDocumentacion.GuardarRDFEnBDRDF(rdfDocAntiguo, mDocumentoID, Documento.FilaDocumento.ProyectoID.Value, GestorDocumental.RdfDS);
                    }
                }
                catch
                {
                    //No se puede hacer nada, ya se ha guardado el error
                }

                throw;
            }

            //Guardado en grafo de búsqueda. AñadirAGnoss lo hace el BASE
            string rdfConfiguradoRecursoNoSemantico = "";
            if (!Documento.TipoDocumentacion.Equals(TiposDocumentacion.Semantico))
            {
                rdfConfiguradoRecursoNoSemantico = ObtenerRdfRecursoNoSemantico();
            }
            
            if (!esMejora)
            {
				UtilidadesVirtuoso.GuardarRecursoEnGrafoBusqueda(Documento, true, mDocumentosExtraGuardar, ProyectoSeleccionado, mListaTriplesSemanticos, mOntologia, GestorDocumental.GestorTesauro, rdfConfiguradoRecursoNoSemantico, mCreandoVersion, Documento.VersionOriginalID, UrlIntragnoss, mOtrosArgumentosBase, PrioridadBase.Alta, mAvailableServices);
				mInsertadoEnGrafoBusqueda = true;
			}            

            if (servicioExterno)
            {
                string urlServComple = null;

                if (PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.urlserviciocomplementario.ToString()))
                {
                    urlServComple = PropiedadesTextoOntologia[PropiedadesOntologia.urlserviciocomplementario.ToString()];
                }
                else
                {
                    urlServComple = PropiedadesTextoOntologia[PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString()];
                }

                urlServComple = ControladorDocumentacion.ObtenerUrlServicioExternoSEMCMS(urlServComple, ProyectoSeleccionado.Clave);

                try
                {
                    if (PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.urlserviciocomplementario.ToString()))
                    {
                        GuardarRdfEnServicioExterno_Asincrono(streamRDF, urlServComple);
                    }
                    else
                    {
                        bool editandoForm = EditandoFormSem && !mCambioDeBorradoAPublicado;
                        string rdfAntiguo = null;

                        if (PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.enviarRdfAntiguo.ToString()))
                        {
                            rdfAntiguo = mSemController.ResourceRDF;
                        }
                        JsonExtServResponse respuesta = GuardarRdfEnServicioExterno(streamRDF, urlServComple, mOntologiaID, mDocumentoID, editandoForm, UtilIdiomas.LanguageCode, IdentidadActual, mModelSaveRec, ProyectoSeleccionado, rdfAntiguo);
                        if (respuesta.Status == 1)
                        {
                            if (respuesta.Action != null)
                            {
                                if (respuesta.Action.RedirectCommunityHome)
                                {
                                    return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                                }
                                else if (respuesta.Action.RedirectResource)
                                {
                                    return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, mOntologiaID.ToString(), mDocumentoID, mOntologiaID, false) + "?modified");
                                }
                                else if (respuesta.Action.RedirectSpecificResource != Guid.Empty)
                                {
                                    return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, mOntologiaID.ToString(), respuesta.Action.RedirectSpecificResource, mOntologiaID, false) + "?modified");
                                }
                                else if (!string.IsNullOrEmpty(respuesta.Action.RedirectUrl))
                                {
                                    return GnossResultUrl(respuesta.Action.RedirectUrl);
                                }
                            }
                            else
                            {
                                return GnossResultOK(respuesta.Message);
                            }
                        }

                        if (respuesta.Action != null && !string.IsNullOrEmpty(respuesta.Action.RunJavaScript))
                        {
                            return GnossResultERROR("javascript=" + respuesta.Action.RunJavaScript + "|||mensaje=" + respuesta.Message);
                        }
                        else
                        {
                            return GnossResultERROR(respuesta.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError("Error al llamar al servicio externo '" + urlServComple + "': " + ex.ToString());
                    throw;
                }
            }

            ControladorDocumentacion.InsertarEnColaProcesarFicherosRecursosModificadosOEliminados(Documento.Clave, TipoEventoProcesarFicherosRecursos.Modificado, mAvailableServices);

            return result;
        }

        private void CrearEventoColaMiniatura(AD.EntityModel.Models.Documentacion.ColaDocumento pColaDocumentoRow)
        {

            try
            {
                InsertarFilaEnColaMiniatura(pColaDocumentoRow);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla ColaUsuarios", mlogger);
                GestorDocumental.DataWrapperDocumentacion.ListaColaDocumento.Add(pColaDocumentoRow);
                mEntityContext.ColaDocumento.Add(pColaDocumentoRow);
            }
        }

        /// <summary>
        /// Recupera la variable 'EntidadesExtEditablesDocID' según la edición del recurso.
        /// </summary>
        private void RecuperarEntidadesExtEditablesDocID()
        {
            Dictionary<Guid, string> docsIDAntiguos = new Dictionary<Guid, string>();

            foreach (EntidadExtEditableDoc entExtEdit in mEntidadesExtEditablesDocID.Values)
            {
                if (!entExtEdit.NuevoDoc && !docsIDAntiguos.ContainsKey(entExtEdit.DocumentoID))
                {
                    docsIDAntiguos.Add(entExtEdit.DocumentoID, entExtEdit.EntidadID);
                }
            }

            mEntidadesExtEditablesDocID = new Dictionary<string, EntidadExtEditableDoc>();

            foreach (string propEntExt in mModelSaveRec.SubOntologiesExtInfo.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string propEnt = propEntExt.Substring(0, propEntExt.IndexOf("|"));
                string datospropEntExt = propEntExt.Substring(propEntExt.IndexOf("|") + 1);
                string propiedad = propEnt.Split(',')[1];
                string entidad = propEnt.Split(',')[0];

                foreach (string entidadControl in datospropEntExt.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] datosEnt = entidadControl.Split(',');
                    if (datosEnt[0] != "-1")
                    {
                        EntidadExtEditableDoc entExtEditDoc = new EntidadExtEditableDoc();
                        entExtEditDoc.EntidadID = datosEnt[2];
                        entExtEditDoc.DocumentoID = new Guid(datosEnt[1]);
                        entExtEditDoc.NumValorPropiedad = int.Parse(datosEnt[0]);
                        entExtEditDoc.Propiedad = propiedad;
                        entExtEditDoc.TipoEntidad = entidad;
                        entExtEditDoc.NuevoDoc = (!docsIDAntiguos.ContainsKey(entExtEditDoc.DocumentoID));
                        string claveID = entExtEditDoc.EntidadID;

                        if (string.IsNullOrEmpty(claveID))
                        {
                            claveID = datosEnt[1];
                        }

                        if (docsIDAntiguos.ContainsKey(entExtEditDoc.DocumentoID))
                        {
                            docsIDAntiguos.Remove(entExtEditDoc.DocumentoID);
                        }

                        mEntidadesExtEditablesDocID.Add(claveID, entExtEditDoc);
                    }
                }
            }

            mSemController.EntidadesIDProhibidoUsar = new List<string>();

            foreach (Guid docID in docsIDAntiguos.Keys)
            {
                EntidadExtEditableDoc entExtEditDoc = new EntidadExtEditableDoc();
                entExtEditDoc.EntidadID = docsIDAntiguos[docID];
                entExtEditDoc.DocumentoID = docID;
                string claveID = entExtEditDoc.EntidadID;
                entExtEditDoc.Eliminado = true;

                if (string.IsNullOrEmpty(claveID))
                {
                    claveID = docID.ToString();
                }

                mEntidadesExtEditablesDocID.Add(claveID, entExtEditDoc);
                mSemController.EntidadesIDProhibidoUsar.Add(entExtEditDoc.EntidadID);
            }
        }

        /// <summary>
        /// Extrae las entidades de las ontologías externas de las entidades externas editables.
        /// </summary>
        /// <param name="pEntidades">Entidades que se van a guardar</param>
        private void ExtraerOntologiasExternasEntExtEditables(List<ElementoOntologia> pEntidades)
        {
            foreach (ElementoOntologia entidad in pEntidades)
            {
                foreach (Propiedad prop in entidad.Propiedades)
                {
                    if (prop.Tipo == TipoPropiedad.ObjectProperty)
                    {
                        if (prop.EspecifPropiedad.SelectorEntidad != null)
                        {
                            if (prop.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Edicion")
                            {
                                List<ElementoOntologia> entidadesValor = new List<ElementoOntologia>(prop.ValoresUnificados.Values);
                                foreach (ElementoOntologia entidadExt in entidadesValor)
                                {
                                    prop.LimpiarValor(entidadExt);
                                    GuardarEntidadExternaDePropSeleccEntExtEditable(prop, entidadExt, entidad);

                                    prop.AgregarValor(entidadExt.ID);
                                }

                                prop.Rango = "";
                            }
                        }
                        else
                        {
                            ExtraerOntologiasExternasEntExtEditables(entidad.EntidadesRelacionadas);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Borra los recursos externos editables anteriormente vinculados al recurso actual.
        /// </summary>
        private void BorrarRecsOntologiasExternasEntExtEditablesDescartados()
        {
            foreach (EntidadExtEditableDoc entExtEdi in mEntidadesExtEditablesDocID.Values)
            {
                if (string.IsNullOrEmpty(entExtEdi.EntidadID) || !entExtEdi.Eliminado)
                {
                    continue;
                }

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                docCN.ObtenerDocumentoPorIDCargarTotal(entExtEdi.DocumentoID, GestorDocumental.DataWrapperDocumentacion, true, false, null);
                docCN.Dispose();
                GestorDocumental.CargarDocumentos(false);

                Documento doc = GestorDocumental.ListaDocumentos[entExtEdi.DocumentoID];
                GestorDocumental.EliminarDocumentoLogicamente(doc);
                doc.FilaDocumento.FechaModificacion = DateTime.Now;
                ControladorDocumentacion.BorrarRDFDeDocumentoEliminado(doc.Clave, doc.FilaDocumento.ElementoVinculadoID.Value, UrlIntragnoss, false, ProyectoSeleccionado.Clave);

                foreach (Guid proyID in doc.ListaProyectos)
                {
                    ControladorDocumentacion.ActualizarGnossLive(proyID, doc.Clave, AccionLive.Eliminado, (int)TipoLive.Recurso, PrioridadLive.Alta, mAvailableServices);
                    ControladorDocumentacion.ActualizarGnossLive(proyID, IdentidadActual.Clave, AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);
                }

                ControladorDocumentacion.EliminarRecursoModeloBaseSimple(doc.Clave, ProyectoSeleccionado.Clave, (short)doc.TipoDocumentacion, mAvailableServices);
            }
        }

        /// <summary>
        /// Incluye la entidad actual en la entidad externa editable reciproca.
        /// </summary>
        /// <param name="pPropiedad">Propiedad con selector externo editable</param>
        /// <param name="pEntidad">Entidad actual</param>
        /// <param name="pEntidadExt">Entidad externa editable reciproca</param>
        private static void IncluirEntidadEnEntidadExternaEditableReciproca(Propiedad pPropiedad, ElementoOntologia pEntidad, ElementoOntologia pEntidadExt)
        {
            if (!string.IsNullOrEmpty(pPropiedad.EspecifPropiedad.SelectorEntidad.PropiedadEdicionReciproca) && !string.IsNullOrEmpty(pPropiedad.EspecifPropiedad.SelectorEntidad.EntidadEdicionReciproca))
            {
                Propiedad prop = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pPropiedad.EspecifPropiedad.SelectorEntidad.PropiedadEdicionReciproca, pPropiedad.EspecifPropiedad.SelectorEntidad.EntidadEdicionReciproca, pEntidadExt);

                if (prop != null && !prop.ValoresUnificados.ContainsKey(pEntidad.ID))
                {
                    prop.AgregarValor(pEntidad.ID);
                }
            }
        }

        /// <summary>
        /// Guarda una entidad externa editable.
        /// </summary>
        /// <param name="pPropiedad">Propiedad con selector externo editable</param>
        /// <param name="pEntidad">Entidad a guardar</param>
        /// <param name="pEntidadRecursoVinc">Entidad del recurso que estamos editando que se relaciona con la entidad externa editable</param>
        private void GuardarEntidadExternaDePropSeleccEntExtEditable(Propiedad pPropiedad, ElementoOntologia pEntidad, ElementoOntologia pEntidadRecursoVinc)
        {
            Guid documentoID = Guid.Empty;
            bool editandoRec = false;
            Guid nuevoDocumentoID = Guid.NewGuid();

            if (EditandoFormSem && mEntidadesExtEditablesDocID.ContainsKey(pEntidad.ID) && !mEntidadesExtEditablesDocID[pEntidad.ID].Eliminado)
            {
                documentoID = mEntidadesExtEditablesDocID[pEntidad.ID].DocumentoID;
                mEntidadesExtEditablesDocID.Remove(pEntidad.ID);//Lo borro para saber que está agregado
                editandoRec = true;
            }
            else
            {
                foreach (string entExt in mEntidadesExtEditablesDocID.Keys)
                {
                    if (mEntidadesExtEditablesDocID[entExt].NuevoDoc)
                    {
                        documentoID = mEntidadesExtEditablesDocID[entExt].DocumentoID;
                        mEntidadesExtEditablesDocID.Remove(entExt);
                        break;
                    }
                }
            }

            if (documentoID == Guid.Empty)
            {
                throw new ExcepcionWeb("Se va a crear un recurso con Guid.Empty");
            }

            SemCmsController.ReempazarDocumentoIDEntidad(mDocumentoID, documentoID, pEntidad);

            if (pPropiedad.EspecifPropiedad.SelectorEntidad.Reciproca)
            {
                IncluirEntidadEnEntidadExternaEditableReciproca(pPropiedad, pEntidadRecursoVinc, pEntidad);
            }

            string enlaceOnto = pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo;

            if (enlaceOnto.Contains("/"))
            {
                enlaceOnto = enlaceOnto.Substring(enlaceOnto.LastIndexOf("/") + 1);
            }

            if (enlaceOnto.LastIndexOf("#") == (enlaceOnto.Length - 1))
            {
                enlaceOnto = enlaceOnto.Substring(0, enlaceOnto.Length - 1);
            }


            string infoExtra = ObtenerInfoExtraBaseDocumento(documentoID, (short)TiposDocumentacion.Semantico, ProyectoSeleccionado.Clave, PrioridadBase.Alta, 0, -1);

            List<ElementoOntologia> entidadesGuardar = new List<ElementoOntologia>();
            entidadesGuardar.Add(pEntidad);

            GestionOWL gestionOWL = new GestionOWL();
            gestionOWL.UrlOntologia = BaseURLFormulariosSem + "/Ontologia/" + enlaceOnto + "#";
            gestionOWL.NamespaceOntologia = mNamespaceOntologia;

            Stream streamRDF = gestionOWL.PasarOWL(null, mOntologia.OntologiasExternas[pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo], entidadesGuardar, null, null);

            streamRDF.Position = 0;
            string ficheroRDF = new StreamReader(streamRDF).ReadToEnd();

            mListaTriplesSemanticos = ControladorDocumentacion.GuardarRDFEnVirtuoso(entidadesGuardar, enlaceOnto, UrlIntragnoss, "", mControladorBase.UsuarioActual.ProyectoID, documentoID.ToString(), false, infoExtra, mModelSaveRec.Draft, false, (short)PrioridadBase.Alta);

            try
            {
                if (!EditandoFormSem)
                {
                    ControladorDocumentacion.GuardarRDFEnBDRDF(ficheroRDF, documentoID, mControladorBase.UsuarioActual.ProyectoID, GestorDocumental.RdfDS);
                    ControladorDocumentacion.GuardarRDFEnBDRdfHistorico(ficheroRDF, documentoID, IdentidadActual.Clave);
                }
                else
                {
                    if (editandoRec)
                    {
                        ControladorDocumentacion.GuardarRDFEnBDRDF(ficheroRDF, nuevoDocumentoID, Documento.FilaDocumento.ProyectoID.Value, GestorDocumental.RdfDS);
                        ControladorDocumentacion.GuardarRDFEnBDRdfHistorico(ficheroRDF, nuevoDocumentoID, IdentidadActual.Clave);
                    }
                    else
                    {
                        ControladorDocumentacion.GuardarRDFEnBDRDF(ficheroRDF, documentoID, Documento.FilaDocumento.ProyectoID.Value, null);
                        ControladorDocumentacion.GuardarRDFEnBDRdfHistorico(ficheroRDF, documentoID, IdentidadActual.Clave);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("PRIMARY KEY"))
                {
                    GuardarMensajeErrorAdmin("El ID con el que está guardando el recurso está siendo usado, vuelva a la página de añadir nuevo recurso e intentelo de nuevo.", ex);
                }

                throw;
            }

            Documento documentoEntExt = GuardarDocumentoBDEntidadExternaDePropSeleccEntExtEditable(documentoID, nuevoDocumentoID, mOntologia.OntologiasExternas[pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo], entidadesGuardar, editandoRec, pPropiedad);

            //Guardado en grafo de búsqueda
            string rdfConfiguradoRecursoNoSemantico = "";
            if (!documentoEntExt.TipoDocumentacion.Equals(TiposDocumentacion.Semantico))
            {
                rdfConfiguradoRecursoNoSemantico = ObtenerRdfRecursoNoSemantico();
            }

            UtilidadesVirtuoso.GuardarRecursoEnGrafoBusqueda(documentoEntExt, true, mDocumentosExtraGuardar, ProyectoSeleccionado, mListaTriplesSemanticos, mOntologia, GestorDocumental.GestorTesauro, rdfConfiguradoRecursoNoSemantico, editandoRec, documentoEntExt.VersionOriginalID, UrlIntragnoss, mOtrosArgumentosBase, PrioridadBase.Alta, mAvailableServices);
        }

        /// <summary>
        /// Guarda en Base de datos el recurso correspondiente a una entidad externa editable.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento de la entidad</param>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pEntidades">Entidades principales a guardar</param>
        /// <param name="pEditandoRec">Indica si se está editando el recurso</param>
        /// <param name="pPropiedad">Propiedad con selector externo editable</param>
        private Documento GuardarDocumentoBDEntidadExternaDePropSeleccEntExtEditable(Guid pDocumentoID, Guid pDocumentoVersionID, Ontologia pOntologia, List<ElementoOntologia> pEntidades, bool pEditandoRec, Propiedad pPropiedad)
        {
            if (GestorDocumental.GestorIdentidades == null)
            {
                GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
            }
            else
            {
                GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(IdentidadActual.GestorIdentidades.DataWrapperIdentidad);
                GestorDocumental.GestorIdentidades.RecargarHijos();
            }
            ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorIdentidades>(), mLoggerFactory);
            controladorIdentidades.CompletarCargaIdentidad(IdentidadActual.Clave);

            string titulo = null;
            string descripcion = null;

            if (!string.IsNullOrEmpty(pOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key))
            {
                titulo = ObtenerTituloODescripcionDePropiedadSEMCMS(pOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key, pOntologia.ConfiguracionPlantilla.PropiedadTitulo.Value, pEntidades);
            }

            if (string.IsNullOrEmpty(titulo))
            {
                titulo = pEntidades[0].ID;
            }

            if (!string.IsNullOrEmpty(pOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key))
            {
                descripcion = ObtenerTituloODescripcionDePropiedadSEMCMS(pOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key, pOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value, pEntidades);
            }

            string tags = UtilCadenas.EliminarHtmlDeTexto(mModelSaveRec.Tags?.Trim());

            if (mDocumentosExtraGuardar == null)
            {
                mDocumentosExtraGuardar = new List<Documento>();
            }

            Documento doc = null;
            if (!pEditandoRec)
            {
                titulo = UtilCadenas.EliminarHtmlDeTexto(titulo.Trim());
                string rutaFichero = UtilCadenas.EliminarCaracteresUrlSem(titulo) + ".rdf";
                Guid elementoVinculadoID = pOntologia.OntologiaID;
                doc = GestorDocumental.AgregarDocumento(rutaFichero, titulo, descripcion, tags, mTipoDocumento, TipoEntidadVinculadaDocumento.Web, elementoVinculadoID, mModelSaveRec.ShareAllowed, mModelSaveRec.Draft, mModelSaveRec.CreatorIsAuthor, null, false, UsuarioActual.OrganizacionID, UsuarioActual.IdentidadID);

                //Cambiar identificador del documento
                if (GestorDocumental.ListaDocumentos.ContainsKey(doc.Clave))
                {
                    GestorDocumental.ListaDocumentos.Remove(doc.Clave);
                    GestorDocumental.ListaDocumentos.Add(pDocumentoID, doc);
                }
                else
                {
                    GestorDocumental.ListaDocumentos.Add(pDocumentoID, doc);
                }
                doc.FilaDocumento.DocumentoID = pDocumentoID;

                //Agrego la comunidad a la que pertenece el documento:
                doc.FilaDocumento.ProyectoID = mControladorBase.UsuarioActual.ProyectoID;

                if (IdentidadOrganizacion != null)
                {
                    doc.FilaDocumento.CreadorID = IdentidadOrganizacion.Clave;
                    doc.FilaDocumento.OrganizacionID = (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID");
                }
                else if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto && IdentidadActual.Tipo != TiposIdentidad.Personal)
                {
                    if (IdentidadActual.IdentidadPersonalMyGNOSS != null)
                    {
                        doc.FilaDocumento.CreadorID = IdentidadActual.IdentidadPersonalMyGNOSS.Clave;
                    }
                    else
                    {
                        doc.FilaDocumento.CreadorID = IdentidadActual.IdentidadMyGNOSS.Clave;
                    }
                }

                List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

                if (IdentidadOrganizacion != null)
                {
                    controladorIdentidades.CompletarCargaIdentidad(IdentidadOrganizacion.Clave);

                    GestorDocumental.AgregarDocumento(listaCategorias, doc, IdentidadOrganizacion.Clave, mModelSaveRec.ResourceVisibility == ResourceVisibility.SpecificReaders || mModelSaveRec.ResourceVisibility == ResourceVisibility.OnlyEditors, UsuarioActual.ProyectoID);
                }
                else if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto && IdentidadActual.Tipo != TiposIdentidad.Personal)
                {
                    Guid identidad;

                    if (IdentidadActual.IdentidadPersonalMyGNOSS != null)
                    {
                        identidad = IdentidadActual.IdentidadPersonalMyGNOSS.Clave;
                    }
                    else
                    {
                        identidad = IdentidadActual.IdentidadMyGNOSS.Clave;
                    }

                    controladorIdentidades.CompletarCargaIdentidad(identidad);
                    GestorDocumental.AgregarDocumento(listaCategorias, doc, identidad, mModelSaveRec.ResourceVisibility == ResourceVisibility.SpecificReaders || mModelSaveRec.ResourceVisibility == ResourceVisibility.OnlyEditors, UsuarioActual.ProyectoID);
                }
                else
                {
                    controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                    GestorDocumental.AgregarDocumento(listaCategorias, doc, mControladorBase.UsuarioActual.IdentidadID, mModelSaveRec.ResourceVisibility == ResourceVisibility.SpecificReaders || mModelSaveRec.ResourceVisibility == ResourceVisibility.OnlyEditors, UsuarioActual.ProyectoID);
                }

                //Asignamos el doc de nuevo porque se crea una nueva instancia en los métodos anteriores:
                doc = GestorDocumental.ListaDocumentos[doc.Clave];

                bool privacidadCambiada = false;
                List<Guid> listaEditoresEliminados;
                List<Guid> listaGruposEditoresEliminados;
                GuardarDatosEditores(doc, out privacidadCambiada, out listaEditoresEliminados, out listaGruposEditoresEliminados);


                GuardarImagenPrincipalRecSemantico(doc, mOntologia.OntologiasExternas[pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo]);

                mDocumentosExtraGuardar.Add(doc);
            }
            else
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                GestorDocumental gesDoc = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

                docCN.ObtenerDocumentoPorIDCargarTotal(pDocumentoID, gesDoc.DataWrapperDocumentacion, true, false, null);
                gesDoc.CargarDocumentos(false);

                Guid documentoUltimaVersionID = gesDoc.ListaDocumentos[pDocumentoID].UltimaVersionID;
                docCN.ObtenerDocumentoPorIDCargarTotal(documentoUltimaVersionID, gesDoc.DataWrapperDocumentacion, true, true, null);
                gesDoc.CargarDocumentos(false);

                foreach (var docOnto in GestorDocumental.ListaDocumentos.Where(doc => doc.Value.TipoDocumentacion.Equals(TiposDocumentacion.Ontologia)).ToList())
                {
                    gesDoc.ListaDocumentos.Add(docOnto.Key, docOnto.Value);
                }

                doc = gesDoc.ListaDocumentos[documentoUltimaVersionID];

                if (CreandoVersionFormSem)
                {
                    bool esMejora = ComprobarSiSeEstaEditandoUnaMejora(mDocumentoID);
					doc = gesDoc.CrearNuevaVersionDocumento(doc, IdentidadCrearVersion, pEsMejora: esMejora);
                }

                doc.Titulo = titulo;
                doc.Descripcion = descripcion;
                GuardarTagsDocumento(doc);
                doc.FilaDocumento.FechaModificacion = DateTime.Now;
                doc.FilaDocumento.Borrador = mModelSaveRec.Draft;

                mEntityContext.SaveChanges();
                docCN.Dispose();

                GuardarImagenPrincipalRecSemantico(doc, mOntologia.OntologiasExternas[pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo]);

                mDocumentosExtraGuardar.Add(doc);
            }

            return doc;
        }

        /// <summary>
        /// Guarda una carga masiva de recursos.
        /// </summary>
        /// <param name="pRutaRdf">Ruta del fichero RDF de la carga masiva</param>
        /// <returns>Acción a realizar</returns>
        private ActionResult GuardadoCargaMasivaFormSem(string rdfTexto)
        {
            if (!GuardandoRecursoCargaMasiva)
            {
                string parametrosCargaMas = ObtenerParametrosBKCargaMasiva(Guid.Empty, null, rdfTexto);
                return GnossResultOK(parametrosCargaMas);
            }
            else
            {
                string sep = "[-|-]";
                string trozo1 = mModelSaveRec.MassiveResourceLoadInfo.Substring(0, mModelSaveRec.MassiveResourceLoadInfo.IndexOf(mModelSaveRec.EditingMassiveResourceID + sep));
                string trozo2 = mModelSaveRec.MassiveResourceLoadInfo.Substring(mModelSaveRec.MassiveResourceLoadInfo.IndexOf(mModelSaveRec.EditingMassiveResourceID + sep));
                string archivoRecEdit = trozo2.Substring(trozo2.IndexOf(sep) + sep.Length);
                archivoRecEdit = archivoRecEdit.Substring(0, archivoRecEdit.IndexOf(sep));
                trozo2 = trozo2.Substring(trozo2.IndexOf("<|||||>"));

                string parametrosCargaMas = ObtenerParametrosBKCargaMasiva(mModelSaveRec.EditingMassiveResourceID, archivoRecEdit, rdfTexto);

                parametrosCargaMas = trozo1 + parametrosCargaMas + trozo2;

                return GnossResultOK(parametrosCargaMas);
            }
        }

        /// <summary>
        /// Obtiene los parametros de la carga masiva separados por '[-|-]'.
        /// </summary>
        /// <param name="pRecursoID">ID del recurso masivo</param>
        /// <param name="pArchivo">Nombre del adjunto del recurso masivo</param>
        /// <param name="pRdf">RDF del recurso masivo</param>
        /// <returns>Parametros de la carga masiva separados por '[-|-]'</returns>
        private string ObtenerParametrosBKCargaMasiva(Guid pRecursoID, string pArchivo, string pRdf)
        {
            string sep = "[-|-]";
            string licencia = null;

            if (!string.IsNullOrEmpty(mModelSaveRec.License) && mModelSaveRec.CreatorIsAuthor && (mModelSaveRec.ShareAllowed || (EsComunidad && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido))))
            {
                licencia = mModelSaveRec.License;
            }

            return string.Concat(pRecursoID, sep, pArchivo, sep, mModelSaveRec.Title, sep, mModelSaveRec.Description, sep, mModelSaveRec.Tags, sep, pRdf, sep, mModelSaveRec.SelectedCategories, sep, mModelSaveRec.CreatorIsAuthor, sep, mModelSaveRec.Authors, sep, mModelSaveRec.ImageRepresentativeValue, sep, licencia, sep, mModelSaveRec.SpecificResourceEditors, sep, mModelSaveRec.ResourceEditors, sep, (short)mModelSaveRec.ResourceVisibility, sep, mModelSaveRec.ResourceReaders, sep, mModelSaveRec.ShareAllowed, sep);
        }


        /// <summary>
        /// Guarda el RDF de un recurso semántico que es formulario virtual.
        /// </summary>
        /// <returns>Acción a realizar</returns>
        private ActionResult GuardarRDFRecursoSemantico_FormSemVirtual()
        {
            mEntidadesGuardar = mSemController.RecogerValoresRdf(mModelSaveRec.RdfValue, mModelSaveRec.EntityIDRegisterInfo);

            Stream streamRDF = ObtenerRDF();

            streamRDF.Position = 0;

            string rdfAntiguo = null;

            if (PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.enviarRdfAntiguo.ToString()))
            {
                rdfAntiguo = mSemController.ResourceRDF;
            }

            JsonExtServResponse respuesta = GuardarRdfEnServicioExterno(streamRDF, UrlServicio, mOntologiaID, mDocumentoID, EditandoFormSem, UtilIdiomas.LanguageCode, IdentidadActual, mModelSaveRec, ProyectoSeleccionado, rdfAntiguo);
            string directorio = Path.Combine(mEnv.WebRootPath, Request.Path, $"documentosvirtuales/{mOntologiaID}/{mDocumentoID}");

            if (respuesta.Status == 1)
            {
                if (respuesta.Action != null)
                {
                    if (respuesta.Action.RedirectCommunityHome)
                    {
                        if (Directory.Exists(directorio))
                        {
                            Directory.Delete(directorio, true);
                        }

                        return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                    }
                    else if (respuesta.Action.RedirectResource)
                    {
                        if (Directory.Exists(directorio))
                        {
                            Directory.Delete(directorio, true);
                        }

                        return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, mOntologiaID.ToString(), mDocumentoID, mOntologiaID, false) + "?modified");
                    }
                    else if (respuesta.Action.RedirectSpecificResource != Guid.Empty)
                    {
                        if (Directory.Exists(directorio))
                        {
                            Directory.Delete(directorio, true);
                        }

                        return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, mOntologiaID.ToString(), respuesta.Action.RedirectSpecificResource, mOntologiaID, false) + "?modified");
                    }
                    else if (!string.IsNullOrEmpty(respuesta.Action.RedirectUrl))
                    {
                        if (Directory.Exists(directorio))
                        {
                            Directory.Delete(directorio, true);
                        }

                        return GnossResultUrl(respuesta.Action.RedirectUrl);
                    }
                }
                else
                {
                    if (Directory.Exists(directorio))
                    {
                        Directory.Delete(directorio, true);
                    }

                    return GnossResultOK(respuesta.Message);
                }
            }

            string codigoJS = "FinPeticionFormFan();";

            if (respuesta.Action != null && !string.IsNullOrEmpty(respuesta.Action.RunJavaScript))
            {
                codigoJS += respuesta.Action.RunJavaScript;
            }

            return GnossResultERROR("javascript=" + codigoJS + "|||mensaje=" + respuesta.Message);
        }

        /// <summary>
        /// Envía el RDF a un servicio externo para que opere con el.
        /// </summary>
        /// <param name="pStreamRDF">Ruta del fichero que contiene el RDF del recurso a guardar</param>
        /// <param name="pUrlServicio">Url del servicio externo</param>
        /// <returns>Respuesta del servicio externo</returns>
        private void GuardarRdfEnServicioExterno_Asincrono(Stream pStreamRDF, string pUrlServicio)
        {
            try
            {
                bool editandoForm = EditandoFormSem && !mCambioDeBorradoAPublicado;

                // Recojo primero los parámetros que no puedo obtener dentro de un hilo
                Proyecto proyActual = ProyectoSeleccionado;
                Identidad identidadActualUsuario = IdentidadActual;

                string rdfAntiguo = null;

                if (PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.enviarRdfAntiguo.ToString()))
                {
                    rdfAntiguo = mSemController.ResourceRDF;
                }

                string conexionAfinidadVirtuoso = mServicesUtilVirtuosoAndReplication.ConexionAfinidad;

                string tokenAfinidadPeticion = Guid.NewGuid().ToString();

                //lanzo la ejecución en un nuevo hilo                                
                Task.Factory.StartNew(() => GuardarRdfEnServicioExterno(pStreamRDF, pUrlServicio, mOntologiaID, mDocumentoID, editandoForm, UtilIdiomas.LanguageCode, identidadActualUsuario, mModelSaveRec, proyActual, rdfAntiguo, conexionAfinidadVirtuoso, tokenAfinidadPeticion));
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
        }

        /// <summary>
        /// Envía el RDF a un servicio externo para que opere con el.
        /// </summary>
        /// <param name="pStreamRDF">Ruta del fichero que contiene el RDF del recurso a guardar</param>
        /// <param name="pUrlServicio">Url del servicio externo</param>
        /// <param name="pOntologiaID">ID de ontología</param>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pEditandoRec">Indica si se está editando</param>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <param name="pIdioma">Idioma actual</param>
        /// <param name="pModelSaveRec">Modelo del guardado de recurso</param>
        /// <param name="pProyectoActual">Proyecto actual</param>
        /// <param name="pRdfAntiguo">RDF antiguo del recurso que se está editando</param>
        /// <param name="pConexionAfinidadVirtuoso"></param>
        /// <param name="pTokenAfinidadPeticion"></param>
        /// <returns>Respuesta del servicio externo</returns>
        private JsonExtServResponse GuardarRdfEnServicioExterno(Stream pStreamRDF, string pUrlServicio, Guid pOntologiaID, Guid pDocumentoID, bool pEditandoRec, string pIdioma, Identidad pIdentidadActual, DocumentEditionModel pModelSaveRec, Proyecto pProyectoActual, string pRdfAntiguo, string pConexionAfinidadVirtuoso = null, string pTokenAfinidadPeticion = null)
        {
            //Leemos el fichero RDF y llamamos al servicio pasandole como parametro el RDF
            byte[] rdfArrayBytes = StreamToByte(pStreamRDF);

            mLoggingService.AgregarEntrada("Enviando al servicio de publicacion '" + pUrlServicio + "' " + rdfArrayBytes.Length + " bytes del archivo en memoria");

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("ResourceID", pDocumentoID.ToString());
            parametros.Add("RDFBytes", Convert.ToBase64String(rdfArrayBytes));
            parametros.Add("OntologyID", pOntologiaID.ToString());
            parametros.Add("EditResource", pEditandoRec.ToString());
            parametros.Add("UserID", pIdentidadActual.Persona.FilaPersona.UsuarioID.ToString());
            parametros.Add("UserLanguaje", pIdioma);
            parametros.Add("UserEmail", pIdentidadActual.Persona.FilaPersona.Email);
            parametros.Add("Tags", pModelSaveRec.Tags);
            parametros.Add("CommunityShortName", pProyectoActual.NombreCorto);

            if (!string.IsNullOrEmpty(pRdfAntiguo))
            {
                byte[] rdfArrayBytesAnt = Encoding.UTF8.GetBytes(pRdfAntiguo);
                parametros.Add("OldRDFBytes", Convert.ToBase64String(rdfArrayBytesAnt));
            }

            string conexionAfinidadVirtuoso = pConexionAfinidadVirtuoso;
            string tokenAfinidadPeticion = pTokenAfinidadPeticion;

            Dictionary<string, string> cabeceras = null;
            if (!string.IsNullOrEmpty(conexionAfinidadVirtuoso))
            {
                // Añadir cabecera afinidad
                cabeceras = new Dictionary<string, string>();
                cabeceras.Add("X-Request-ID", tokenAfinidadPeticion);
            }

            string respuesta = UtilWeb.HacerPeticionPost(pUrlServicio, parametros, cabeceras);
            return JsonConvert.DeserializeObject<JsonExtServResponse>(respuesta);
        }

        /// <summary>
        /// Obtener el RDF del recurso
        /// </summary>
        /// <returns>Ruta del fichero temporal</returns>
        private Stream ObtenerRDF()
        {
            GestionOWL gestionOWL = new GestionOWL();
            gestionOWL.UrlOntologia = mUrlOntologia;
            gestionOWL.NamespaceOntologia = mNamespaceOntologia;
            return gestionOWL.PasarOWL(null, mOntologia, mEntidadesGuardar, null, null);
        }

        private static byte[] StreamToByte(Stream input)
        {
            input.Position = 0;
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Guarda la parte acida del recurso semántico.
        /// </summary>
        /// <param name="pCategoriasSeleccionadas">Categorías seleccionadas</param>
        /// <returns>Acción resultado del guardado</returns>
        private ActionResult GuardarRecursoSemanticoenBD(List<Guid> pCategoriasSeleccionadas)
        {
            Documento doc = null;
            Guid docAntiguoID = mDocumentoID;
			bool esMejora = ComprobarSiSeEstaEditandoUnaMejora(mDocumentoID);
			if (!EditandoFormSem)
            {
                doc = CrearDocumentoEnModeloAcido(mDocumentoID, pCategoriasSeleccionadas);
                if (!esMejora)
                {
					CrearDocumentoEnModeloLive(doc);					
				}
				CrearDocumentoAccionesExtra(doc);

				mOtrosArgumentosBase = ",##enlaces####enlaces##";

                if (mDocumentosExtraGuardar != null)
                {
                    foreach (Documento docExtra in mDocumentosExtraGuardar)
                    {
                        if (!esMejora)
                        {
							CrearDocumentoEnModeloLive(docExtra);
						}                        
                        CrearDocumentoAccionesExtra(docExtra);
                    }
                }
                string urlRedirect = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, doc, (IdentidadOrganizacion != null));
                if (esMejora)
                {
					urlRedirect = $"{mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, UtilCadenas.EliminarCaracteresUrlSem(doc.Titulo), Documento.VersionOriginalID, Documento.ElementoVinculadoID, false)}/{doc.Clave}";
				}
				return GnossResultUrl(urlRedirect);
            }
            else
            {
                if (CreandoVersionFormSem)
                {                    
					doc = GestorDocumental.CrearNuevaVersionDocumento(Documento, IdentidadCrearVersion, DocumentoVersionID, pEsMejora: esMejora);

                    // A partir de aqui se trata con el nuevo documento creado ya que luego al invocar la propoedad Documento
                    // necesitamos el nuevo documento versionado.
                    mDocumentoID = doc.Clave;

                    if (!GestorDocumental.ListaDocumentos.ContainsKey(mDocumentoID))
                    {
                        DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                        docCN.ObtenerDocumentoPorIDCargarTotal(mDocumentoID, GestorDocumental.DataWrapperDocumentacion, true, true, null);
                        docCN.Dispose();

                        GestorDocumental.CargarDocumentos(false);
                    }
                }

                mCambioDeBorradoAPublicado = Documento.FilaDocumento.Borrador && !mModelSaveRec.Draft;
                bool privacidadCambiada = false;
                List<Guid> listaEditoresEliminados;
                List<Guid> listaGruposEditoresEliminados;
                GuardarRecursoModeloAcido(pCategoriasSeleccionadas, CreandoVersionFormSem, false, out privacidadCambiada, out listaEditoresEliminados, out listaGruposEditoresEliminados);
                if (!esMejora)
                {
					GuardarRecursoModeloLive(privacidadCambiada, CreandoVersionFormSem, mCambioDeBorradoAPublicado, docAntiguoID, Documento, listaEditoresEliminados, listaGruposEditoresEliminados);
					GuardarRecursoModeloBase(CreandoVersionFormSem, mCambioDeBorradoAPublicado, docAntiguoID, Documento);
				}
                

                ControladorDocumentacion.BorrarCacheControlFichaRecursos(docAntiguoID);

                if (mDocumentosExtraGuardar != null && !esMejora)
                {
                    foreach (Documento docExtra in mDocumentosExtraGuardar)
                    {
                        Guid docIDAntiguoAux = docExtra.VersionAnterior != null ? docExtra.VersionAnterior.Clave : Guid.Empty;
                        bool versionadoDocumentoExtra = docIDAntiguoAux != Guid.Empty;
                        GuardarRecursoModeloLive(false, versionadoDocumentoExtra, false, docIDAntiguoAux, docExtra, new List<Guid>(), new List<Guid>());
                        GuardarRecursoModeloBase(versionadoDocumentoExtra, false, docIDAntiguoAux, docExtra);
                        ControladorDocumentacion.BorrarCacheControlFichaRecursos(docExtra.Clave);
                    }
                }

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                // Identidad proyecto actual
                controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);
                
                string urlRedirect = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Documento, (IdentidadOrganizacion != null)).Replace(Documento.Clave.ToString(), Documento.VersionOriginalID.ToString());
				if (esMejora)
                {
                    urlRedirect = $"{mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, UtilCadenas.EliminarCaracteresUrlSem(Documento.Titulo), Documento.VersionOriginalID, Documento.ElementoVinculadoID, false)}/{doc.Clave}";
                }
				return GnossResultUrl(urlRedirect);
            }
        }

        /// <summary>
        /// Guarda en los grafos auxiliares los nuevos valores.
        /// </summary>
        private void GuardarValoresInsertarEnGrafosAuxiliares()
        {
            try
            {
                //Guardamos los campos de los grafos:
                if (mSemController.ValoresGrafoAutocompletar.Count > 0)
                {
                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mControladorBase.UsuarioActual.ProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    facetadoCN.InsertarValoresGrafos(mSemController.ValoresGrafoAutocompletar, 0);
                    facetadoCN.Dispose();
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Recogemos la información extra que se deberá procesar en el servicio módulo base tras la ejecución de las querys de virtuoso.
        /// </summary>
        /// <returns>Cadena de parámetros necesarios para que el servicio de replicación inserte en el módulo base.</returns>
        public string ObtenerInfoExtraBaseDocumento(long pEstadoCargaID)
        {
            StringBuilder infoExtra = new StringBuilder();

            if (!EditandoFormSem)
            {
                //Es un documento nuevo.
                infoExtra = new StringBuilder(ObtenerInfoExtraBaseDocumento(mDocumentoID, (short)TiposDocumentacion.Semantico, ProyectoSeleccionado.Clave, PrioridadBase.Alta, 0, pEstadoCargaID));
            }
            else
            {
                if (!CreandoVersionFormSem)
                {
                    foreach (Guid proyectoID in Documento.ListaProyectos)
                    {
                        infoExtra.Append($"{ObtenerInfoExtraBaseDocumento(Documento.Clave, Documento.FilaDocumento.Tipo, Documento.ProyectoID, PrioridadBase.Alta, 0, pEstadoCargaID)}|;|%|;|");
                    }
                }
                else
                {
                    foreach (Guid proyectoID in Documento.ListaProyectos)
                    {
                        //Convertir a string
                        infoExtra.Append($"{ObtenerInfoExtraBaseDocumento(DocumentoID, Documento.FilaDocumento.Tipo, Documento.ProyectoID, PrioridadBase.Alta, 1, pEstadoCargaID)}|;|%|;|{ObtenerInfoExtraBaseDocumento(Documento.Clave, Documento.FilaDocumento.Tipo, Documento.ProyectoID, PrioridadBase.Alta, 0, pEstadoCargaID)}|;|%|;|");
                    }
                }
            }

            return infoExtra.ToString();
        }

        /// <summary>
        /// Método que transforma la petición para agregar al servicio módulo base en un string con los parámetros que necesita el servicio de replicación para enviar la solicitud.
        /// </summary>
        /// <param name="pDocumentoID">DocumentoID que se ha creado/editado</param>
        /// <param name="pTipoDoc">Tipo de documento que se ha creado.</param>
        /// <param name="pProyectoID">Proyecto donde se ha creado el documento.</param>
        /// <param name="pPrioridadBase">Prioridad para procesarlo por el servicio modulo base.</param>
        /// <param name="pAccion">Acción a realizar sobre el recurso, 0 agregar, 1 eliminar</param>
        /// <returns>Cadena de parámetros necesarios para que el servicio de replicación inserte en el módulo base.</returns>
        private string ObtenerInfoExtraBaseDocumento(Guid pDocumentoID, short pTipoDoc, Guid pProyectoID, PrioridadBase pPrioridadBase, int pAccion, long pEstadoCargaID)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            proyCL.Dispose();

            string tempString = "";

            //TablaBaseProyectoID
            tempString += id + "|";

            //Tag
            tempString += $"{Constantes.ID_TAG_DOCUMENTO}{pDocumentoID}{Constantes.ID_TAG_DOCUMENTO},{Constantes.TIPO_DOC}{pTipoDoc}{Constantes.TIPO_DOC}|";

            //Tipo de acción (0 agregado) (1 eliminado)
            tempString += pAccion + "|";

            //Prioridad de procesado por el servicio base.
            tempString += (short)pPrioridadBase + "|";

            if (pEstadoCargaID != -1)
            {
                tempString += pEstadoCargaID + "|";
                tempString += (short)TipoAccionCarga.ServicioBaseProcesado + "|";
            }

            return tempString;
        }

        /// <summary>
        /// Agrega la propiedad archivo temporal.
        /// </summary>
        /// <param name="pListaEntidadesPrinc">Lista de entidades de la ontología</param>
        private void AgregarPropiedadArchivoTemporal(List<ElementoOntologia> pListaEntidadesPrinc)
        {
            if (mOntologia.ConfiguracionPlantilla.PropiedadArchivoCargaMasiva.Key == null)
            {
                mErrorDocumento = UtilIdiomas.GetText("CREARDOCUMENTO", "ERRORONTONOTIENEARCHIVO");
                return;
            }

            Propiedad propAgValor = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadArchivoCargaMasiva.Key, mOntologia.ConfiguracionPlantilla.PropiedadArchivoCargaMasiva.Value, pListaEntidadesPrinc);

            if (propAgValor == null)
            {
                mErrorDocumento = UtilIdiomas.GetText("CREARDOCUMENTO", "ERRORENTNOTIENEARCHIVO");
                return;
            }

            propAgValor.LimpiarValor();
            propAgValor.DarValor("[archivoTemporalCargaMasivagnoss]", null);

            if (mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key != null)
            {
                propAgValor = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key, mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Value, pListaEntidadesPrinc);

                propAgValor.LimpiarValor();
                propAgValor.DarValor("[tituloTemporalCargaMasivagnoss]", null);
            }

            if (mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key != null)
            {

                propAgValor = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key, mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value, pListaEntidadesPrinc);


                propAgValor.LimpiarValor();
                propAgValor.DarValor("[descripcionTemporalCargaMasivagnoss]", null);
            }
        }

        /// <summary>
        /// Guarda los datos de las propiedades especiales.
        /// </summary>
        /// <param name="pEntidadesPrinc">Lista entidades pricipales</param>
        private AD.EntityModel.Models.Documentacion.ColaDocumento GuardarDatosPropiedadesEspeciales(List<ElementoOntologia> pEntidadesPrinc)
        {
            string nombrePropTitulo = mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key;
            string nombrePropDescripcion = mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key;
            List<KeyValuePair<string, string>> urlCaptura = mOntologia.ConfiguracionPlantilla.PropiedadImagenFromURL;
            List<KeyValuePair<string, string>> openSeaDragon = mOntologia.ConfiguracionPlantilla.PropiedadesOpenSeaDragon;

            #region NombrePropTitulo

            if (nombrePropTitulo != null)
            {
                try
                {
                    mModelSaveRec.Title = ObtenerTituloODescripcionDePropiedadSEMCMS(nombrePropTitulo, mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Value, pEntidadesPrinc);
                }
                catch (Exception ex)
                {
                    GuardarMensajeErrorAdmin("El título del recurso está mal configurado en el XML.", ex);
                    throw new ExcepcionWeb("El título del recurso está mal configurado en el XML.");
                }

                if (mModelSaveRec.Title == "")
                {
                    GuardarMensajeErrorAdmin("Titulo del recurso no introducido.", null);
                    throw new ExcepcionWeb("Titulo del recurso no introducido.");
                }
            }

            #endregion

            #region NombrePropDescripcion

            if (nombrePropDescripcion != null)
            {
                try
                {
                    // Se hace encode porque luego se hace un decode de la descripción, y en los forumlarios semánticos viene sin hacer encode
                    mModelSaveRec.Description = System.Net.WebUtility.UrlEncode(ObtenerTituloODescripcionDePropiedadSEMCMS(nombrePropDescripcion, mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value, pEntidadesPrinc));
                }
                catch (Exception ex)
                {
                    GuardarMensajeErrorAdmin("La descripción del recurso está mal configurada en el XML.", ex);
                    throw new ExcepcionWeb("La descripción del recurso está mal configurada en el XML.");
                }
            }

            #endregion

            #region UrlCaptura

            //Si hay url de captura enviamos al Thumbnail Generator una notificación para que genere la miniatura.
            if (urlCaptura != null)
            {
                try
                {
                    Propiedad propUrlCaptura = null;

                    foreach (KeyValuePair<string, string> propCap in urlCaptura)
                    {
                        foreach (ElementoOntologia entidadPrinc in pEntidadesPrinc)
                        {
                            propUrlCaptura = GestionOWL.ObtenerPropiedadDeTipoEntidad(propCap.Key, propCap.Value, entidadPrinc);

                            if (propUrlCaptura != null)
                            {
                                break;
                            }
                        }

                        if (propUrlCaptura != null && propUrlCaptura.ValoresUnificados.Count > 0)
                        {
                            break;
                        }
                    }

                    //Cargar la url de la web de la que hay que obtener el pantallazo y enviarselo al servicio de procesado de tareas.
                    if (propUrlCaptura != null && propUrlCaptura.ValoresUnificados.Count > 0)
                    {
                        string urlObtenerCaptura = "";
                        if (propUrlCaptura.ValoresUnificados.Any())
                        {
                            urlObtenerCaptura = propUrlCaptura.ValoresUnificados.First().Key;
                        }

                        //Si es un campo de tipo descripción, obtenemos la primera imagen de la descripción.
                        if (propUrlCaptura.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Tiny)
                        {
                            urlObtenerCaptura = ObtenerUrlsDeDescripcion(urlObtenerCaptura);
                        }

                        if (!string.IsNullOrEmpty(urlObtenerCaptura))
                        {
                            string tamañoCapturas = "240";
                            string urlValorPropCap = $"{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesDocumentos}/{UtilArchivos.ContentImgCapSemanticas}/{UtilArchivos.DirectorioDocumento(mDocumentoID)}/{Guid.NewGuid().ToString().ToLower()}.jpg";

                            if (ParametroProyecto.ContainsKey(ParametroAD.CaputurasImgSize))
                            {
                                tamañoCapturas = ParametroProyecto[ParametroAD.CaputurasImgSize].Split(',')[0];
                            }

                            if (mOntologia.ConfiguracionPlantilla.PropiedadImagenRepre.Key != null)
                            {
                                string tamañosPropConfig = mOntologia.ConfiguracionPlantilla.ObtenerTamaniosTextoPropiedadImagenMini(mOntologia.ConfiguracionPlantilla.PropiedadImagenRepre.Key, mOntologia.ConfiguracionPlantilla.PropiedadImagenRepre.Value);

                                if (!string.IsNullOrEmpty(tamañosPropConfig))
                                {
                                    if (tamañosPropConfig[tamañosPropConfig.Length - 1] == ',')
                                    {
                                        tamañosPropConfig = tamañosPropConfig.Substring(0, tamañosPropConfig.Length - 1);
                                    }

                                    tamañoCapturas = tamañosPropConfig;
                                }

                                Propiedad propImagenRep = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadImagenRepre.Key, mOntologia.ConfiguracionPlantilla.PropiedadImagenRepre.Value, pEntidadesPrinc);
                                if (propImagenRep != null && propImagenRep.ValoresUnificados.Count == 0)
                                {
                                    urlValorPropCap = $"{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesDocumentos}/{UtilArchivos.ContentImagenesSemanticas}/{UtilArchivos.DirectorioDocumento(mDocumentoID)}/{Guid.NewGuid().ToString().ToLower()}.jpg";

                                    if (propImagenRep.FunctionalProperty)
                                    {
                                        propImagenRep.UnicoValor = new KeyValuePair<string, ElementoOntologia>(urlValorPropCap, null);
                                    }
                                    else
                                    {
                                        propImagenRep.ListaValores.Add(urlValorPropCap, null);
                                    }
                                }
                            }

                            //Enviar al servicio de procesado de tareas para que obtenga la imagen del recurso?
                            string infoExtra = urlObtenerCaptura + "|" + urlValorPropCap + "|" + tamañoCapturas;
                            //Pasarle a Lorena la url de la web, el tamaño de las miniaturas int[], rutaRec con la ruta de la imagen.

                            if (GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.NombreCategoriaDoc != null && GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.NombreCategoriaDoc.Contains("class=")) //La ontología tiene icono
                            {
                                infoExtra += "|class=" + GestorDocumental.ListaDocumentos[mOntologiaID].FilaDocumento.NombreCategoriaDoc.Split('=')[1];
                            }

                            AD.EntityModel.Models.Documentacion.ColaDocumento colaDocumentoRow = new AD.EntityModel.Models.Documentacion.ColaDocumento();
                            colaDocumentoRow.DocumentoID = mDocumentoID;
                            colaDocumentoRow.AccionRealizada = 0; //Agregar
                            colaDocumentoRow.Estado = 0; // Espera
                            colaDocumentoRow.FechaEncolado = DateTime.Now;
                            colaDocumentoRow.Prioridad = 0; //Prioridad con la que se va a procesar el recurso.
                            colaDocumentoRow.InfoExtra = infoExtra;

                            mCapturaSercicioEncargada = true;

                            return colaDocumentoRow;
                        }
                    }
                }
                catch (Exception ex)
                {
                    GuardarMensajeErrorAdmin("La Url de la que se debe hacer la captura del recurso está mal configurada en el XML.", ex);
                    throw new ExcepcionWeb("La Url de la que se debe hacer la captura del recurso está mal configurada en el XML.");
                }
            }

            #endregion

            #region OpenSeaDragon

            if (openSeaDragon != null && !string.IsNullOrEmpty(mModelSaveRec.OpenSeaDragonInfo))
            {
                try
                {
                    foreach (KeyValuePair<string, string> propCap in openSeaDragon)
                    {
                        List<Propiedad> propiedades = EstiloPlantilla.ObtenerTodasInstanciasPropiedadACualquierNivelPorNombre(propCap.Key, propCap.Value, pEntidadesPrinc);

                        foreach (Propiedad propiedad in propiedades)
                        {
                            foreach (string valor in propiedad.ValoresUnificados.Keys.Where(item => mModelSaveRec.OpenSeaDragonInfo.Contains($"{item}|")))
                            {
                                string widthImg = mModelSaveRec.OpenSeaDragonInfo.Substring(mModelSaveRec.OpenSeaDragonInfo.IndexOf(valor + "|") + valor.Length + 1);
                                widthImg = widthImg.Substring(0, widthImg.IndexOf("|||"));
                                string heightImg = widthImg.Split('|')[1];
                                widthImg = widthImg.Split('|')[0];

                                Propiedad propAncho = propiedad.ElementoOntologia.ObtenerPropiedad(propiedad.EspecifPropiedad.OpenSeaDragon.Key);
                                Propiedad propAlto = propiedad.ElementoOntologia.ObtenerPropiedad(propiedad.EspecifPropiedad.OpenSeaDragon.Value);

                                if (propAncho == null || propAlto == null)
                                {
                                    throw new ExcepcionWeb("La propiedad alto o acho configuradas no pertenece a la entidad que contiene la propiedad de la imagen.");
                                }

                                propAncho.LimpiarValor();
                                propAncho.AgregarValor(widthImg);

                                propAlto.LimpiarValor();
                                propAlto.AgregarValor(heightImg);

                                AD.EntityModel.Models.Documentacion.ColaDocumento colaDocumentoRow = new AD.EntityModel.Models.Documentacion.ColaDocumento();
                                colaDocumentoRow.DocumentoID = mDocumentoID;
                                colaDocumentoRow.AccionRealizada = 0; //Agregar
                                colaDocumentoRow.Estado = 0; // Espera
                                colaDocumentoRow.FechaEncolado = DateTime.Now;
                                colaDocumentoRow.Prioridad = 0; //Prioridad con la que se va a procesar el recurso.
                                colaDocumentoRow.InfoExtra = "OpenSeaDragon|" + propiedad.PrimerValorPropiedad;

                                return colaDocumentoRow;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    GuardarMensajeErrorAdmin("El procesado de imagenes OpenSeaDragon está mal configurada en el XML.", ex);
                    throw new ExcepcionWeb("El procesado de imagenes OpenSeaDragon está mal configurada en el XML:" + Environment.NewLine + ex.ToString());
                }
            }
            #endregion

            return null;
        }

        /// <summary>
        /// Se encarga de añadir en RabbitMQ a la colaMiniatura el elemento ColaDocumento
        /// </summary>
        /// <param name="pColaDocumento">Elemento a añadir a la ColaMinitatura</param>
        private void InsertarFilaEnColaMiniatura(AD.EntityModel.Models.Documentacion.ColaDocumento pColaDocumento)
        {
            if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN) && mAvailableServices.CheckIfServiceIsAvailable(mAvailableServices.GetBackServiceCode(Interfaces.InterfacesOpen.BackgroundService.Thumbnail), ServiceType.Background))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_MINIATURA, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, EXCHANGE, COLA_MINIATURA))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pColaDocumento));
                }
            }
        }

        /// <summary>
        /// Obtiene el título o la descripción de una propiedad configurada en el XML como título o descripción.
        /// </summary>
        /// <param name="pPropiedad">Nombre de la propiedad</param>
        /// <param name="pEntidad">Tipo de entidad de la propiedad</param>
        /// <param name="pEntidades">Entidades</param>
        /// <returns>Valor para el título o la descripción de una propiedad configurada en el XML como título o descripción</returns>
        private string ObtenerTituloODescripcionDePropiedadSEMCMS(string pPropiedad, string pEntidad, List<ElementoOntologia> pEntidades)
        {
            StringBuilder titulo = new StringBuilder();
            Propiedad propTitulo = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pPropiedad, pEntidad, pEntidades);

            if (propTitulo.ListaValoresIdioma.Count > 0)
            {
                bool agregadoIdiomDef = false;

                if (mSemController.IdiomaDefecto != null && propTitulo.ListaValoresIdioma.ContainsKey(mSemController.IdiomaDefecto)) //Añado 1º el idioma por defecto.
                {
                    foreach (string valorIdio in propTitulo.ListaValoresIdioma[mSemController.IdiomaDefecto].Keys)
                    {
                        titulo.Append($"{valorIdio}@{mSemController.IdiomaDefecto}|||");
                    }

                    agregadoIdiomDef = true;
                }

                foreach (string idioma in propTitulo.ListaValoresIdioma.Keys)
                {
                    if (!agregadoIdiomDef || idioma != mSemController.IdiomaDefecto)
                    {
                        foreach (string valorIdio in propTitulo.ListaValoresIdioma[idioma].Keys)
                        {
                            titulo.Append($"{valorIdio}@{idioma}|||");
                        }
                    }
                }
            }
            else
            {
                if (propTitulo.UnicoValor.Key != null)
                {
                    titulo = new StringBuilder(propTitulo.UnicoValor.Key);
                }
                else
                {
                    if (propTitulo.ListaValores.Any())
                    {
                        titulo = new StringBuilder(propTitulo.ListaValores.First().Key);
                    }
                }
            }
            if (!propTitulo.EspecifPropiedad.PermitirScript)
            {
                titulo = new StringBuilder(UtilCadenas.LimpiarInyeccionCodigo(titulo.ToString()));
            }
            return titulo.ToString();
        }

        /// <summary>
        /// Método para obtener la primera URL de una descripción pasada como parámetro
        /// </summary>
        /// <param name="pDescripcion">Descripción</param>
        /// <returns>La primera URL de la descripción</returns>
        private string ObtenerUrlsDeDescripcion(string pDescripcion)
        {
            string urlImagen = "";
            int caracteractual = 0;

            while (caracteractual >= 0)
            {
                if (pDescripcion.Length > caracteractual && pDescripcion.Substring(caracteractual).Contains("<img"))
                {
                    caracteractual = pDescripcion.IndexOf("<img", caracteractual);
                    int finImg = pDescripcion.IndexOf(">", caracteractual) - caracteractual + 1;
                    string foto = pDescripcion.Substring(caracteractual, finImg);

                    if (foto.Contains("src=\""))
                    {
                        int inicioSRC = foto.IndexOf("src=\"") + 5;
                        string ruta = foto.Substring(inicioSRC, foto.Substring(inicioSRC).IndexOf("\""));
                        urlImagen = ruta;
                        break;
                    }
                    else if (foto.Contains("src='"))
                    {
                        int inicioSRC = foto.IndexOf("src=\"") + 5;
                        string ruta = foto.Substring(inicioSRC, foto.Substring(inicioSRC).IndexOf("\""));
                        urlImagen = ruta;
                        break;
                    }
                    caracteractual = caracteractual + 1;
                }
                else
                {
                    caracteractual = -1;
                }
            }

            if (!urlImagen.StartsWith("http"))
            {
                urlImagen = UrlIntragnoss + urlImagen;
            }

            //urlImagen
            return urlImagen;
        }

        /// <summary>
        /// Gestiona los archivos de google drive del recurso.
        /// </summary>
        private void GestionarArchivosGoogleDrive()
        {
            bool nombreArchivoCambiado = false;

            if (ListaNombresDocumento.Count > 0)
            {
                string rdf = mModelSaveRec.RdfValue;

                foreach (string nombreOriginal in ListaNombresDocumento.Keys)
                {
                    string nodoArchivo = "<" + ListaNombresDocumento[nombreOriginal].Value + ">";
                    string nodoFinArchivo = "</" + ListaNombresDocumento[nombreOriginal].Value + ">";

                    if (rdf.Contains(nodoArchivo + nombreOriginal + "<"))
                    {
                        rdf = rdf.Replace(nodoArchivo + nombreOriginal + "<", nodoArchivo + ListaNombresDocumento[nombreOriginal].Key + "<");
                        nombreArchivoCambiado = true;
                    }
                    else
                    {

                        if (rdf.Contains(nodoArchivo))
                        {
                            if (rdf.IndexOf(nodoArchivo).Equals(rdf.LastIndexOf(nodoArchivo)))
                            {
                                //Solo hay un nodo archivo, sustituyo su contenido por el nombre del archivo con el código de google drive
                                int indiceInicio = rdf.IndexOf(nodoArchivo);
                                int indiceFin = rdf.IndexOf(nodoFinArchivo);
                                rdf = rdf.Substring(0, indiceInicio) + nodoArchivo + ListaNombresDocumento[nombreOriginal].Key + rdf.Substring(indiceFin);
                                nombreArchivoCambiado = true;
                            }
                        }
                        else
                        {
                            //El archivo ha sido eliminado
                            nombreArchivoCambiado = true;
                        }
                    }
                }

                mModelSaveRec.RdfValue = rdf;
            }

            if (!nombreArchivoCambiado && ListaNombresDocumento.Count > 0)
            {
                StringBuilder mensaje = new StringBuilder($"ERRVINDRIVE: No se ha podido vincular el nombre del documento con el ID en Google Drive. \r\nrdf: {mModelSaveRec.RdfValue}");

                foreach (string nombreOriginal in ListaNombresDocumento.Keys)
                {
                    mensaje.AppendLine($"{nombreOriginal}:{ListaNombresDocumento[nombreOriginal].Key}");
                }

                GuardarLogError(mensaje.ToString());
            }
        }

        /// <summary>
        /// Ajusta la visibilidad de los editores y lectores según lo configurado en el xml.
        /// </summary>
        private void AjustarVisiblidadYEditoresSegunXml()
        {
            try
            {
                if (EditandoFormSem)
                {
                    return;
                }

                if (mOntologia.ConfiguracionPlantilla.TipoVisiblidadEdicionRec != null)
                {
                    mModelSaveRec.ResourceReaders = "";
                    mModelSaveRec.ResourceVisibility = ResourceVisibility.OnlyEditors;

                    if (mOntologia.ConfiguracionPlantilla.TipoVisiblidadEdicionRec == "Abierto")
                    {
                        mModelSaveRec.ResourceVisibility = ResourceVisibility.Open;
                    }
                    else if (mOntologia.ConfiguracionPlantilla.TipoVisiblidadEdicionRec == "Miembros")
                    {
                        mModelSaveRec.ResourceVisibility = ResourceVisibility.CommunityMembers;
                    }
                    else if (mOntologia.ConfiguracionPlantilla.TipoVisiblidadEdicionRec == "Editores")
                    {
                        mModelSaveRec.ResourceVisibility = ResourceVisibility.OnlyEditors;
                    }
                    else if (mOntologia.ConfiguracionPlantilla.TipoVisiblidadEdicionRec == "Lectores")
                    {
                        mModelSaveRec.ResourceVisibility = ResourceVisibility.SpecificReaders;
                        string idsLectores = ObtenerIDsGrupoPorNombreCorto(mOntologia.ConfiguracionPlantilla.GruposLectoresFijos);
                        mModelSaveRec.ResourceReaders = idsLectores;
                    }
                }

                if (mOntologia.ConfiguracionPlantilla.GruposEditoresFijos != null)
                {
                    if (mModelSaveRec.ResourceEditors == null)
                    {
                        mModelSaveRec.ResourceEditors = string.Empty;
                    }


                    if (mOntologia.ConfiguracionPlantilla.GruposEditoresFijos.Count == 0)
                    {
                        mModelSaveRec.SpecificResourceEditors = false;
                    }
                    else
                    {
                        mModelSaveRec.SpecificResourceEditors = true;
                        string idsEditores = ObtenerIDsGrupoPorNombreCorto(mOntologia.ConfiguracionPlantilla.GruposEditoresFijos);
                        mModelSaveRec.ResourceEditors += idsEditores;
                    }
                }
            }
            catch (Exception ex)
            {
                mErrorDocumento = UtilIdiomas.GetText("CONTROLESCVSEM", "GUARDADOFALLO");
                GuardarLogError("Grupos mal configurados: " + ex.ToString());
            }
        }

        /// <summary>
        /// Comprueba las repeticiones configuradas en el XML.
        /// </summary>
        /// <param name="pEntidadesPrinc">Lista entidades pricipales</param>
        private ActionResult ComprobarRepeticionesConfiguradas(List<ElementoOntologia> pEntidadesPrinc)
        {
            StringBuilder mensajeRepecionNoContinua = new StringBuilder();
            StringBuilder mensajeRepecion = new StringBuilder();

            if (mOntologia.ConfiguracionPlantilla.PropsComprobarRepeticion.Count > 0)
            {
                FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                foreach (KeyValuePair<string, string> claveRep in mOntologia.ConfiguracionPlantilla.PropsComprobarRepeticion.Keys)
                {
                    Propiedad propiedad = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(claveRep.Key, claveRep.Value, pEntidadesPrinc);

                    if (propiedad != null && propiedad.ValoresUnificados.Count > 0)
                    {
                        List<string> listaSujetos = facCN.ObjeterSujetosDePropiedadPorValor(GestorDocumental.ListaDocumentos[mOntologiaID].Enlace, propiedad.NombreFormatoUri, new List<string>(propiedad.ValoresUnificados.Keys));

                        if (listaSujetos.Count > 0)
                        {
                            List<Guid> listaDocumentos = new List<Guid>();

                            foreach (string sujeto in listaSujetos.Where(item => item.Contains("_")))
                            {
                                string sujetoAux = sujeto.Substring(0, sujeto.LastIndexOf("_"));
                                sujetoAux = sujetoAux.Substring(sujetoAux.LastIndexOf("_") + 1);
                                Guid docAuxID;
                                if (Guid.TryParse(sujetoAux, out docAuxID))
                                {
                                    listaDocumentos.Add(docAuxID);
                                }
                            }

                            if (listaDocumentos.Count > 0)
                            {
                                List<AD.EntityModel.Models.Documentacion.Documento> listaDocumentosDW = docCN.ObtenerDocumentosPorIDSoloDocumento(listaDocumentos);

                                foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in listaDocumentosDW)
                                {
                                    if ((!EditandoFormSem || filaDoc.DocumentoID != mDocumentoID) && !filaDoc.Eliminado && filaDoc.UltimaVersion && (mOntologia.ConfiguracionPlantilla.PropsComprobarRepeticion[claveRep].Value == 0 || filaDoc.ProyectoID == ProyectoSeleccionado.Clave))
                                    {
                                        if (filaDoc.ProyectoID == ProyectoSeleccionado.Clave)
                                        {
                                            if (mOntologia.ConfiguracionPlantilla.PropsComprobarRepeticion[claveRep].Key)
                                            {
                                                mensajeRepecionNoContinua.Append(UtilIdiomas.GetText("CREARDOCUMENTO", "EXISTE_PROP_DOC_REPE_NO_CONTI_COM", mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, UtilCadenas.EliminarCaracteresUrlSem(filaDoc.Titulo), filaDoc.DocumentoID, mOntologiaID, IdentidadOrganizacion != null), ObtenerValorPropiedadParaMensajeRepeticion(propiedad), ObtenerNombrePropiedadParaMensajeRepeticion(propiedad), UtilCadenas.ObtenerTextoDeIdioma(filaDoc.Titulo, mControladorBase.IdiomaUsuario, null)));
                                            }
                                            else
                                            {
                                                mensajeRepecion.Append(UtilIdiomas.GetText("CREARDOCUMENTO", "EXISTE_PROP_DOC_REPE_COM", mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, UtilCadenas.EliminarCaracteresUrlSem(filaDoc.Titulo), filaDoc.DocumentoID, mOntologiaID, IdentidadOrganizacion != null), ObtenerValorPropiedadParaMensajeRepeticion(propiedad), ObtenerNombrePropiedadParaMensajeRepeticion(propiedad), UtilCadenas.ObtenerTextoDeIdioma(filaDoc.Titulo, mControladorBase.IdiomaUsuario, null)));
                                            }
                                        }
                                        else
                                        {
                                            if (mOntologia.ConfiguracionPlantilla.PropsComprobarRepeticion[claveRep].Key)
                                            {
                                                mensajeRepecionNoContinua.Append(UtilIdiomas.GetText("CREARDOCUMENTO", "EXISTE_PROP_DOC_REPE_NO_CONTI", GnossUrlsSemanticas.GetURLBaseRecursosRecursoInvitadoConIDS(BaseURLIdioma, UrlPerfil, UtilIdiomas, UtilCadenas.EliminarCaracteresUrlSem(filaDoc.Titulo), filaDoc.DocumentoID), ObtenerValorPropiedadParaMensajeRepeticion(propiedad), ObtenerNombrePropiedadParaMensajeRepeticion(propiedad), UtilCadenas.ObtenerTextoDeIdioma(filaDoc.Titulo, mControladorBase.IdiomaUsuario, null)));
                                            }
                                            else
                                            {
                                                mensajeRepecion.Append(UtilIdiomas.GetText("CREARDOCUMENTO", "EXISTE_PROP_DOC_REPE", GnossUrlsSemanticas.GetURLBaseRecursosRecursoInvitadoConIDS(BaseURLIdioma, UrlPerfil, UtilIdiomas, UtilCadenas.EliminarCaracteresUrlSem(filaDoc.Titulo), filaDoc.DocumentoID), ObtenerValorPropiedadParaMensajeRepeticion(propiedad), ObtenerNombrePropiedadParaMensajeRepeticion(propiedad), UtilCadenas.ObtenerTextoDeIdioma(filaDoc.Titulo, mControladorBase.IdiomaUsuario, null)));
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                facCN.Dispose();
                docCN.Dispose();
            }

            if ((mensajeRepecion.Length > 0 && !mModelSaveRec.SkipSemanticPropertyRepeat) || mensajeRepecionNoContinua.Length > 0)
            {
                return GnossResultOK($"mensajeRepecion={mensajeRepecion.ToString()}||mensajeRepecionNoContinua={mensajeRepecionNoContinua.ToString()}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene el valor de una propiedad formateado para el mensaje de repetición de una propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Valor de una propiedad formateado para el mensaje de repetición de una propiedad</returns>
        private static string ObtenerValorPropiedadParaMensajeRepeticion(Propiedad pPropiedad)
        {
            string val = "";

            foreach (string valor in pPropiedad.ValoresUnificados.Keys)
            {
                val += valor + ", ";
            }

            if (val != "")
            {
                val = val.Substring(0, val.Length - 2);
            }

            return val;
        }

        /// <summary>
        /// Obtiene el nombre de la propiedad para el mensaje de repetición de una propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Nombre de la propiedad para el mensaje de repetición de una propiedad</returns>
        private static string ObtenerNombrePropiedadParaMensajeRepeticion(Propiedad pPropiedad)
        {
            string posibleNombrePropiedad = pPropiedad.EspecifPropiedad.NombrePropiedad(false);
            if (!string.IsNullOrEmpty(posibleNombrePropiedad))
            {
                return posibleNombrePropiedad;
            }
            else
            {
                return pPropiedad.Nombre.Replace("_", " ");
            }
        }

        /// <summary>
        /// Obtiene el nombre de una categoría de tesauro semántico según el idioma.
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetas</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPropNombre">Propiedad nombre de las categorías</param>
        /// <param name="pIdiomaUsu">Idioma del usuario</param>
        /// <returns>Nombre de una categoría de tesauro semántico según el idioma</returns>
        public static string ObtenerNombreCatTesSem(FacetadoDS pFacetadoDS, string pSujeto, string pPropNombre, string pIdiomaUsu)
        {
            DataRow[] filas = pFacetadoDS.Tables[0].Select("s='" + pSujeto + "' AND p='" + pPropNombre + "'");

            if (filas.Length > 0)
            {
                if (filas.Length > 1 && pFacetadoDS.Tables[0].Columns.Count > 3 && !string.IsNullOrEmpty(pIdiomaUsu))
                {
                    DataRow filaSinIdioma = null;

                    foreach (DataRow fila in filas)
                    {
                        if (!fila.IsNull(3) && !string.IsNullOrEmpty((string)fila[3]))
                        {
                            if ((string)fila[3] == pIdiomaUsu)
                            {
                                return (string)fila[2];
                            }
                        }
                        else
                        {
                            filaSinIdioma = fila;
                        }
                    }

                    if (filaSinIdioma != null)
                    {
                        return (string)filaSinIdioma[2];
                    }
                }

                return (string)filas[0][2];
            }

            return null;
        }

        /// <summary>
        /// Acción de traer los hijos de una categoría del tesauro semántico seleccionada. Sólo aplica a formularios semánticos.
        /// </summary>
        /// <param name="pModel">Modelo de traer los hijos de una categoría del tesauro semántico seleccionada</param>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult TraerHijosTesSem(SemanticThesaurusCategoryChildren pModel)
        {
            try
            {
                FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                FacetadoDS facetadoDS = facCN.ObtenerCatHijasCatTesSemanticoFormulario(pModel.Graph, pModel.CategoryUri, pModel.RequestedProperty);
                facCN.Dispose();

                Dictionary<string, List<KeyValuePair<string, string>>> entidades = new Dictionary<string, List<KeyValuePair<string, string>>>();
                SortedDictionary<string, List<string>> entidadesOrdenadas = new SortedDictionary<string, List<string>>();

                foreach (DataRow fila in facetadoDS.Tables[0].Rows)
                {
                    string sujeto = (string)fila[0];
                    string predicado = (string)fila[1];
                    string objeto = (string)fila[2];
                    if (entidades.ContainsKey(sujeto))
                    {
                        entidades[sujeto].Add(new KeyValuePair<string, string>(predicado, objeto));
                    }
                    else
                    {
                        List<KeyValuePair<string, string>> valorProp = new List<KeyValuePair<string, string>>();
                        valorProp.Add(new KeyValuePair<string, string>(predicado, objeto));
                        entidades.Add(sujeto, valorProp);
                    }

                    if (predicado == pModel.CategoryIdProperty)
                    {
                        if (!entidadesOrdenadas.ContainsKey(objeto))
                        {
                            entidadesOrdenadas.Add(objeto, new List<string>());
                        }

                        entidadesOrdenadas[objeto].Add(sujeto);
                    }
                }

                facetadoDS.Dispose();

                StringBuilder categoriasHijasYNombres = new StringBuilder();

                List<string> entitiesWithChildren = new List<string>();
                Dictionary<string, string> editionEntitiesValues = new Dictionary<string, string>();

                SemCmsController.GenerarDatosTesauroSemanticoCategoriasEdicion(entidades, entidadesOrdenadas, pModel.PropertyName, pModel.RequestedProperty, facetadoDS, IdiomaUsuario, editionEntitiesValues, entitiesWithChildren);

                foreach (string catId in editionEntitiesValues.Keys)
                {
                    categoriasHijasYNombres.Append($"{catId}|||{editionEntitiesValues[catId]}|||");

                    if (entitiesWithChildren.Contains(catId))
                    {
                        categoriasHijasYNombres.Append("1");
                    }
                    else
                    {
                        categoriasHijasYNombres.Append("0");
                    }

                    categoriasHijasYNombres.Append("[|||]");
                }

                string nombresCatNuevas = "";

                foreach (DataRow fila in facetadoDS.Tables[0].Select("p='" + pModel.PropertyName + "'"))
                {
                    string sujeto = (string)fila[0];
                    if (!nombresCatNuevas.Contains(sujeto + "|"))
                    {
                        string nombreCat = ObtenerNombreCatTesSem(facetadoDS, sujeto, pModel.PropertyName, IdiomaUsuario);
                        nombresCatNuevas += sujeto + "|" + nombreCat + "|||";
                    }
                }
                categoriasHijasYNombres.Append($"[[|||]]{nombresCatNuevas}");

                return GnossResultOK(categoriasHijasYNombres.ToString());
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion TraerHijosTesSem");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
            }
        }

        /// <summary>
        /// Acción de aceptar el recorte de un Jcrop.
        /// </summary>
        /// <param name="pModel">Modelo de aceptar Jcrop</param>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult AceptarJcrop(SaveJcrop pModel)
        {
            try
            {
                CargarInicial_ModificarRecursoSemantico();
                ActionResult redireccion = ComprobarRedirecciones_ModificarRecursoSemantico();

                if (redireccion != null)
                {
                    return redireccion;
                }

                InicializarModeloParaAccionSemCms(null);

                string ruta = pModel.ImgSrc;
                ruta = ruta.Substring(0, ruta.IndexOf('?'));

                string extensionArchivo = ruta.Substring(ruta.LastIndexOf('.'));

                ruta = ruta.Substring(ruta.IndexOf(UtilArchivos.ContentImagenesDocumentos)).Replace(extensionArchivo, "");

                string especialID = ruta.Substring(ruta.LastIndexOf("/") + 1);

                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                servicioImagenes.Url = UrlIntragnossServicios;

                byte[] bytesImagen = servicioImagenes.ObtenerImagen(ruta, extensionArchivo);
                byte[] bytesImagenCortada = UtilImages.CropImageFile(bytesImagen, pModel.Width, pModel.Height, pModel.XCoord, pModel.YCoord, extensionArchivo.Substring(1));

                string propiedad = pModel.Extra;

                AgregarArchivoAServicio_SemCms(mDocumentoID, 0, especialID, bytesImagenCortada, extensionArchivo, propiedad);

                servicioImagenes.BorrarImagenDeDirectorio(ruta + extensionArchivo);

                return GnossResultOK("OK");
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion AceptarJcrop");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
            }

            return GnossResultERROR();
        }

        /// <summary>
        /// Acción de obtener los selectores dependientes de otro selectores de entidades.
        /// </summary>
        /// <param name="pModel">Modelo de selección dependiente</param>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult ObtenerSelectoresEntDependientes(GetDependentSelectorsEntities pModel)
        {
            try
            {
                CargarInicial_ModificarRecursoSemantico();
                ActionResult redireccion = ComprobarRedirecciones_ModificarRecursoSemantico();

                if (redireccion != null)
                {
                    return redireccion;
                }

                InicializarModeloParaAccionSemCms(null);

                string datos = mSemController.ObtenerDatosSelectoresEntidadDependientes(pModel.PropertyName, pModel.EntityType, pModel.PropertyValue);

                return GnossResultOK(datos);
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion ObtenerSelectoresEntDependientes");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
                return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }
        }

        /// <summary>
        /// Acción de aceptar los archivos de la carga masiva.
        /// </summary>
        /// <param name="pModel">Modelo de aceptar los archivos de la carga masiva</param>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult AceptarArchivosCargaMasiva(SaveMasiveLoadFiles pModel)
        {
            try
            {
                CargarInicial_ModificarRecursoSemantico();
                ActionResult redireccion = ComprobarRedirecciones_ModificarRecursoSemantico();

                if (redireccion != null)
                {
                    return redireccion;
                }

                string sep = "[-|-]";
                string directorio = Path.Combine(mEnv.WebRootPath, Request.Path, $"documentosvirtuales/cargasmasivas/{mDocumentoID}");
                string infoDocsAux = "";
                string txtHackCargaMasiva = pModel.InfoFiles + sep + "<|||||>";
                string[] docPlantilla = txtHackCargaMasiva.Split(new string[] { sep }, StringSplitOptions.None);

                DirectoryInfo directorioArchivos = new DirectoryInfo(directorio);
                foreach (string fileName in directorioArchivos.GetFiles().Select(item => item.Name))
                {
                    Guid recursoID = Guid.NewGuid();
                    infoDocsAux = string.Concat(infoDocsAux, recursoID, sep);//0 Guid
                    infoDocsAux = string.Concat(infoDocsAux, fileName, sep);//1 Nombre fichero
                    string nombreArc = fileName;

                    if (nombreArc.Contains("."))
                    {
                        nombreArc = nombreArc.Substring(0, nombreArc.LastIndexOf("."));
                    }

                    nombreArc = nombreArc.Substring(0, nombreArc.LastIndexOf("_"));

                    string titulo = nombreArc;
                    infoDocsAux = string.Concat(infoDocsAux, titulo, sep);//2 Título

                    string descripcion = "";

                    infoDocsAux = string.Concat(infoDocsAux, descripcion, sep);//3 Descripción

                    string tags = ObtenerEtiquetasAutomaticas(nombreArc, descripcion, ProyectoSeleccionado.Clave);
                    infoDocsAux = string.Concat(infoDocsAux, tags, sep);//4 Tags

                    for (int i = 5; i < docPlantilla.Length; i++)
                    {
                        infoDocsAux = string.Concat(infoDocsAux, docPlantilla[i], sep);
                    }

                    infoDocsAux = infoDocsAux.Substring(0, infoDocsAux.Length - sep.Length);
                }

                txtHackCargaMasiva = string.Concat(txtHackCargaMasiva, infoDocsAux);

                if (!string.IsNullOrEmpty(infoDocsAux))
                {
                    return GnossResultOK(txtHackCargaMasiva);
                }
                else
                {
                    return GnossResultERROR(UtilIdiomas.GetText("CREARDOCUMENTO", "ERRORNOARCHIVOADJUNTO"));
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion AceptarArchivosCargaMasiva");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
                return GnossResultERROR(UtilIdiomas.GetText("CREARDOCUMENTO", "ERRORNOARCHIVOADJUNTO"));
            }
        }

        /// <summary>
        /// Acción de editar un recurso de la carga masiva.
        /// </summary>
        /// <param name="pModel">Modelo de editar un recurso de la carga masiva</param>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult EditarRecursoCargaMasiva(SaveMasiveLoadFiles pModel)
        {
            try
            {
                TipoPagina = TiposPagina.BaseRecursos;

                mEditRecCont = new EditResourceModel();
                mEditRecCont.TabName = UtilIdiomas.GetText("COMMON", "RECURSOS");
                mEditRecCont.UrlPestanya = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, false);

                mEditandoRecursoCargaMasiva = true;
                mDatosRecursoEditadoCargaMasiva = pModel.InfoFiles.Split(new string[] { "[-|-]" }, StringSplitOptions.None);
                ActionResult redireccion = Index_ModificarRecursoSemantico();

                if (redireccion != null)
                {
                    return redireccion;
                }

                return GnossResultHtml("_ModificarRecurso", mEditRecCont.ModifyResourceModel);
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion EditarRecursoCargaMasiva");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
                return GnossResultERROR(UtilIdiomas.GetText("CONTROLESCVSEM", "GUARDADOFALLO"));
            }
        }

        /// <summary>
        /// Acción de publicar los recursos de la carga masiva.
        /// </summary>
        /// <param name="pModel">Modelo de editar un recurso de la carga masiva</param>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult PublicarCargaMasiva(SaveMasiveLoadFiles pModel)
        {
            try
            {
                CargarInicial_ModificarRecursoSemantico();
                ActionResult redireccion = ComprobarRedirecciones_ModificarRecursoSemantico();

                if (redireccion != null)
                {
                    return redireccion;
                }

                InicializarModeloParaAccionSemCms(null);

                #region Variables previas

                StringBuilder recursosCorrectos = new StringBuilder(pModel.InfoFilesAlreadyPublished);
                int recursoIncorrectos = 0;
                Propiedad propArchivo = null;

                foreach (ElementoOntologia entidad in mOntologia.Entidades)
                {
                    propArchivo = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(mOntologia.ConfiguracionPlantilla.PropiedadArchivoCargaMasiva.Key, mOntologia.ConfiguracionPlantilla.PropiedadArchivoCargaMasiva.Value, entidad);

                    if (propArchivo != null)
                    {
                        break;
                    }
                }

                if (GestorDocumental.GestorIdentidades == null)
                {
                    GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
                }
                else
                {
                    GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(IdentidadActual.GestorIdentidades.DataWrapperIdentidad);
                    GestorDocumental.GestorIdentidades.RecargarHijos();
                }
                ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorIdentidades>(), mLoggerFactory);
                controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                string directorio = Path.Combine(mEnv.WebRootPath, Request.Path, $"documentosvirtuales/cargasmasivas/{mDocumentoID}");

                #endregion

                string arg = pModel.InfoFiles.Substring(pModel.InfoFiles.IndexOf("<|||||>") + 7);//Quito la plantilla
                foreach (string infoRec in arg.Split(new string[] { "<|||||>" }, StringSplitOptions.RemoveEmptyEntries))
                {

                    mDatosRecursoEditadoCargaMasiva = infoRec.Split(new string[] { "[-|-]" }, StringSplitOptions.None);
                    Guid recursoID = new Guid(mDatosRecursoEditadoCargaMasiva[0]);
                    string nombreFichero = mDatosRecursoEditadoCargaMasiva[1];

                    if (recursosCorrectos.ToString().Contains(recursoID.ToString()))
                    {
                        continue;//Ya publicado
                    }

                    try
                    {
                        ObtenerModeloSalvarRecurso_CargaMasiva();

                        #region Archivo

                        ListaNombresDocumento = null;
                        int propImagen = 0;
                        string especialID = nombreFichero;
                        string nombreEspecialRdf = nombreFichero;

                        if (propArchivo != null && propArchivo.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Video)
                        {
                            propImagen = 1;
                            especialID = Guid.NewGuid().ToString();
                            nombreEspecialRdf = $"videossemanticos/{recursoID}/{especialID}.flv";
                        }
                        else if (propArchivo != null && propArchivo.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Archivo)
                        {
                            propImagen = 2;
                        }
                        else if (propArchivo != null && propArchivo.EspecifPropiedad.TipoCampo == TipoCampoOntologia.ArchivoLink)
                        {
                            propImagen = 3;
                            nombreEspecialRdf = $"{UtilArchivos.ContentDocLinks}/{UtilArchivos.DirectorioDocumento(recursoID)}/{especialID}";
                        }
                        else
                        {
                            especialID = Guid.NewGuid().ToString();
                            nombreEspecialRdf = $"{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesDocumentos}/{UtilArchivos.ContentImagenesSemanticas}/{recursoID}/{especialID}.jpg";
                        }

                        byte[] buffer1 = System.IO.File.ReadAllBytes(directorio + nombreFichero);
                        string extensionArchivo = "";

                        if (nombreFichero.Contains("."))
                        {
                            extensionArchivo = nombreFichero.Substring(nombreFichero.LastIndexOf(".")).ToLower();
                        }

                        string propEnt = propArchivo.Nombre + "," + propArchivo.Dominio[0];

                        if (!AgregarArchivoAServicio_SemCms(recursoID, propImagen, especialID, buffer1, extensionArchivo, propEnt))
                        {
                            throw new ExcepcionGeneral("Fallo al subir archivos:" + propImagen + ", " + especialID + ", " + buffer1.Length + ", " + extensionArchivo);
                        }

                        #region Google Drive

                        if (ListaNombresDocumento.ContainsKey(nombreEspecialRdf))
                        {
                            nombreEspecialRdf = ListaNombresDocumento[nombreEspecialRdf].Key;
                        }
                        else if (ListaNombresDocumento.Count > 0)
                        {
                            StringBuilder mensaje = new StringBuilder($"ERRVINDRIVE: No se ha podido vincular el nombre del documento con el ID en Google Drive. \r\nrdf: {mModelSaveRec.RdfValue}");
                            foreach (string nombreOriginal in ListaNombresDocumento.Keys)
                            {
                                mensaje.AppendLine($"{nombreOriginal}:{ListaNombresDocumento[nombreOriginal].Key}");
                            }

                            GuardarLogError(mensaje.ToString());
                        }

                        #endregion

                        mModelSaveRec.RdfValue = mModelSaveRec.RdfValue.Replace("[archivoTemporalCargaMasivagnoss]", nombreEspecialRdf).Replace(mDocumentoID.ToString(), recursoID.ToString());

                        if (mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key != null)
                        {
                            mModelSaveRec.RdfValue = mModelSaveRec.RdfValue.Replace("[tituloTemporalCargaMasivagnoss]", UtilCadenas.ObtenerTextoDeIdioma(mModelSaveRec.Title, UtilIdiomas.LanguageCode, null));
                        }

                        if (mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key != null)
                        {
                            mModelSaveRec.RdfValue = mModelSaveRec.RdfValue.Replace("[descripcionTemporalCargaMasivagnoss]", UtilCadenas.ObtenerTextoDeIdioma(mModelSaveRec.Description, UtilIdiomas.LanguageCode, null));
                        }

                        List<Guid> categoriasSeleccionadas = RecogerCategoriasSeleccionadas();

                        string mensError = ValidarRecurso(mModelSaveRec.Draft, categoriasSeleccionadas);

                        if (!string.IsNullOrEmpty(mensError))
                        {
                            throw new ExcepcionWeb(mensError);
                        }

                        #endregion

                        #region Guardar RDF

                        //Recogemos la información extra que se deberá procesar en el servicio módulo base tras la ejecución de las querys de virtuoso.
                        string infoExtra = ObtenerInfoExtraBaseDocumento(recursoID, (short)TiposDocumentacion.Semantico, ProyectoSeleccionado.Clave, PrioridadBase.Alta, 0, -1);

                        GestionOWL gestorOWL = new GestionOWL();
                        gestorOWL.UrlOntologia = mUrlOntologia;
                        gestorOWL.NamespaceOntologia = mNamespaceOntologia;
                        List<ElementoOntologia> instanciasPrincipales = gestorOWL.LeerFicheroRDF(mOntologia, mModelSaveRec.RdfValue, true);

                        mListaTriplesSemanticos = ControladorDocumentacion.GuardarRDFEnVirtuoso(instanciasPrincipales, GestorDocumental.ListaDocumentos[mOntologiaID].Enlace, UrlIntragnoss, "", mControladorBase.UsuarioActual.ProyectoID, recursoID.ToString(), false, infoExtra, false, false, (short)PrioridadBase.Alta);

                        try
                        {
                            ControladorDocumentacion.GuardarRDFEnBDRDF(mModelSaveRec.RdfValue, recursoID, mControladorBase.UsuarioActual.ProyectoID, null);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("PRIMARY KEY"))
                            {
                                GuardarMensajeErrorAdmin("El ID con el que está guardando el recurso está siendo usado, vuelva a la página de añadir nuevo recurso e intentelo de nuevo.", ex);
                            }

                            throw;
                        }

                        GuardarValoresInsertarEnGrafosAuxiliares();

                        #endregion

                        Guid bkDocID = mDocumentoID;
                        mDocumentoID = recursoID;
                        GuardarRecursoSemanticoenBD(categoriasSeleccionadas);
                        mDocumentoID = bkDocID;

                        recursosCorrectos.Append($"{recursoID},{mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, GestorDocumental.ListaDocumentos[recursoID], IdentidadOrganizacion != null)}&");
                    }
                    catch (Exception ex)
                    {
                        recursoIncorrectos++;
                        GuardarLogError(ex.Message + "\n" + ex.StackTrace);
                    }
                }

                if (recursoIncorrectos > 0)
                {
                    return GnossResultOK("KO|" + UtilIdiomas.GetText("CONTROLESCVSEM", "ERRORPUBLIRECMAS") + "|" + recursosCorrectos);
                }
                else
                {
                    try
                    {
                        //Intento borrar el directorio temporal:
                        DirectoryInfo directorioInf = new DirectoryInfo(directorio);
                        directorioInf.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLog("No es posible borrar el fichero temporal" + ex.Message, mlogger);
                    }

                    return GnossResultOK("OK|" + UtilIdiomas.GetText("CONTROLESCVSEM", "GUARDADOOK") + "|" + recursosCorrectos);
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion PublicarCargaMasiva");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
                return GnossResultERROR(UtilIdiomas.GetText("CONTROLESCVSEM", "GUARDADOFALLO"));
            }
        }

        /// <summary>
        /// Obtiene el modelo para salvar un recurso de la carga masiva.
        /// </summary>
        private void ObtenerModeloSalvarRecurso_CargaMasiva()
        {
            mModelSaveRec = new DocumentEditionModel();
            mModelSaveRec.Title = mDatosRecursoEditadoCargaMasiva[2];
            mModelSaveRec.Description = mDatosRecursoEditadoCargaMasiva[3];
            mModelSaveRec.Tags = mDatosRecursoEditadoCargaMasiva[4];
            mModelSaveRec.RdfValue = mDatosRecursoEditadoCargaMasiva[5];
            mModelSaveRec.SelectedCategories = mDatosRecursoEditadoCargaMasiva[6];
            mModelSaveRec.CreatorIsAuthor = bool.Parse(mDatosRecursoEditadoCargaMasiva[7]);
            mModelSaveRec.Authors = mDatosRecursoEditadoCargaMasiva[8];
            mModelSaveRec.ImageRepresentativeValue = mDatosRecursoEditadoCargaMasiva[9];
            mModelSaveRec.SpecificResourceEditors = bool.Parse(mDatosRecursoEditadoCargaMasiva[11]);
            mModelSaveRec.ResourceEditors = mDatosRecursoEditadoCargaMasiva[12];
            mModelSaveRec.ResourceVisibility = (ResourceVisibility)short.Parse(mDatosRecursoEditadoCargaMasiva[13]);
            mModelSaveRec.ResourceReaders = mDatosRecursoEditadoCargaMasiva[14];
            mModelSaveRec.ShareAllowed = bool.Parse(mDatosRecursoEditadoCargaMasiva[15]);

            if (!string.IsNullOrEmpty(mDatosRecursoEditadoCargaMasiva[10]) && mModelSaveRec.CreatorIsAuthor && (mModelSaveRec.ShareAllowed || (EsComunidad && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido))))
            {
                mModelSaveRec.License = mDatosRecursoEditadoCargaMasiva[2];
            }
        }

        /// <summary>
        /// Obtiene las etiquetas automáticas de un recurso a partir del título y descripción.
        /// </summary>
        /// <param name="pTitulo">Título</param>
        /// <param name="pDescripcion">Descripción</param>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Cadena de caracteres con las etiquetas separadas por comas</returns>
        private string ObtenerEtiquetasAutomaticas(string pTitulo, string pDescripcion, Guid pProyectoID)
        {
            try
            {
                CallEtiquetadoAutomaticoService servicioEtiAuto = new CallEtiquetadoAutomaticoService(mConfigService);

                if (pTitulo == null)
                {
                    pTitulo = "";
                }

                if (pDescripcion == null)
                {
                    pDescripcion = "";
                }

                string etiquetas = servicioEtiAuto.SeleccionarEtiquetasDesdeServicio(pTitulo, pDescripcion, pProyectoID.ToString());
                return etiquetas;
            }
            catch (Exception ex)
            {
                GuardarLogError("Error en etiquetado automático: " + ex.ToString());
            }

            return null;
        }

        /// <summary>
        /// Request an url with an oauth sign
        /// </summary>
        /// <param name="httpMethod">Http method (GET, POST, PUT...)</param>
        /// <param name="url">Url to make the request</param>
        /// <param name="postData">(Optional) Post data to send in the body request</param>
        /// <param name="contentType">(Optional) Content type of the postData</param>
        /// <param name="acceptHeader">(Optional) Accept header</param>
        /// <returns>Response of the server</returns>
        private static string WebRequest(string httpMethod, string url, byte[] byteData)
        {
            string result = "";

            HttpResponseMessage response = null;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("UserAgent", UtilWeb.GenerarUserAgent());
            if (httpMethod == "POST")
            {
                HttpContent contentData = null;
                if (byteData != null)
                {
                    ByteArrayContent bytes = new ByteArrayContent(byteData);
                    contentData = new MultipartFormDataContent();
                    ((MultipartFormDataContent)contentData).Add(bytes, "pBytes", "pBytes");
                }
                contentData.Headers.Add("UserAgent", UtilWeb.GenerarUserAgent());
                response = client.PostAsync($"{url}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;

            }
            else
            {
                client.DefaultRequestHeaders.Add("UserAgent", UtilWeb.GenerarUserAgent());
                response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }

            return result;
        }

        /// <summary>
        /// Realizamos un recorte cuadrado de la imagen pasada por parámetro del tamaño pasado por parámetro
        /// </summary>
        /// <param name="pRecorte">Tamaño que tendrá el recorte de la imagen</param>
        /// <param name="pImagenOriginal">Imagen que queremos recortar</param>
        /// <returns>Array de bytes con el contenido de la imagen recortada</returns>
        private byte[] RealizarRecorteCuadrado(int pRecorte, Image pImagenOriginal)
        {
            Image imagenPeque = UtilImages.RecortarImagenACuadrada(pImagenOriginal, pRecorte);

            if (imagenPeque.Width > pImagenOriginal.Width || imagenPeque.Height > pImagenOriginal.Height)
            {
                imagenPeque = pImagenOriginal;
            }

            return UtilImages.ImageToBytePng(imagenPeque);
        }

        /// <summary>
        /// Redimensionamos una imagen a el Ancho y el Alto pasados por parametros. En caso de que uno sea -1 se redimensionará la imagen en función
        /// del otro parámetro manteniendo la relación de aspecto
        /// </summary>
        /// <param name="pAncho">Ancho deseado para la redimensión</param>
        /// <param name="pAlto">Alto deseado para la redimensión</param>
        /// <param name="pImagenOriginal">Imagen que queremos redimensionar</param>
        /// <returns>Array de bytes con el contenido de la imagen redimensionada</returns>
        /// <exception cref="InvalidDataException">Devolvemos InvalidDataException en caso de que ambos parámetros sean -1</exception>
        private static byte[] RedimensionarMiniaturaAnchoAlto(int pAncho, int pAlto, Image pImagenOriginal)
        {
            byte[] bytesImagenRedimensionada = null;
            if (pAncho == -1 && pAlto == -1)
            {
                throw new InvalidDataException("No se ha configurado ni el alto ni el ancho del recorte, hay que configurar al menos un valor");
            }
            float proporcion = 0;

            //No está configurado el ancho, redimensionamos en función del alto
            if (pAncho == -1)
            {
                proporcion = (float)pImagenOriginal.Height / pImagenOriginal.Width;
                pAncho = (int)(proporcion * pAlto);
            }

            //No está configurado el alto, redimensionamos en función del ancho
            if (pAlto == -1)
            {
                proporcion = (float)pImagenOriginal.Height / pImagenOriginal.Width;
                pAlto = (int)(proporcion * pAncho);
            }

            pImagenOriginal.Mutate(x => x.Resize(pAncho, pAlto));
            using (var ms = new MemoryStream())
            {
                pImagenOriginal.Save(ms, PngFormat.Instance);
                bytesImagenRedimensionada = ms.ToArray();
            }

            return bytesImagenRedimensionada;
        }

        #endregion

        #region Añadir a GNOSS

        /// <summary>
        /// Método inicial de subir recurso parte 2.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index_AnyadirGnoss()
        {
            RecogerParametros_AnyadirGnoss();

            ActionResult redireccion = ComprobarRedirecciones_AnyadirGnoss(false);

            if (redireccion != null)
            {
                return redireccion;
            }

            ExpirarCookie();

            AjustarVisibilidad_AnyadirGnoss();

            if (mEditRecCont.TypePage.Equals(EditResourceModel.TypePageEditResource.AddToGnossResource))
            {
                mEditRecCont.ModifyResourceModel.AddToGnossShareSites = CargarBaseRecursosUsuario_AnyadirGnoss(false);
            }
            else
            {
                CargarBaseRecursosUsuario_AnyadirComunidad();
            }

            mEditRecCont.ModifyResourceModel.ThesaurusEditorModel = PrepararTesauro_AnyadirGnoss(mEditRecCont.ModifyResourceModel.AddToGnossShareSites.Keys.ToArray()[0]);

            return null;
        }

        /// <summary>
        /// Carga el control de tesauro de una base de recursos de Add To Gnoss.
        /// </summary>
        /// <param name="pModel">Modelo para la acción</param>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult GetThesauroBRAddToGnoss(GetThesauroAddToGnoss pModel)
        {
            try
            {
                ActionResult redireccion = ComprobarRedirecciones_AnyadirGnoss(true);

                if (redireccion != null)
                {
                    return redireccion;
                }

                ThesaurusEditorModel thesaurusEditorModel = PrepararTesauro_AnyadirGnoss(pModel.ResourceSpaceID);

                return GnossResultHtml("EditorTesauro/_EditorTesauro", thesaurusEditorModel);
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion GetThesauroBRAddToGnoss");
                }

                GuardarLogError(ex, " ModeloDatos: " + modeloDatos);
                return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }
        }

        /// <summary>
        /// Crea el recurso de Add To Gnoss.
        /// </summary>
        /// <param name="pModel">Modelo de guardado de recurso</param>
        /// <returns>Acción resultado de la creación del recurso</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        private ActionResult CrearRecurso_AnyadirGnoss(DocumentEditionModel pModel)
        {
            mModelSaveRec = pModel;

            CargarBaseRecursosUsuario_AnyadirGnoss(true);
            mBRsCatsAddToGnoss = RecogerBRsYCategoriasSeleccionadas_AnyadirGnoss();

            if (ComprobarRepetidos && !pModel.SkipRepeat)
            {
                ActionResult htmlPanelRepe = ComprobarRepeticionEnlaceRecurso_SubirRecursoPart2();

                if (htmlPanelRepe != null)
                {
                    return htmlPanelRepe;
                }
            }

            string mensError = ValidarRecurso_AnyadirGnoss(mBRsCatsAddToGnoss);

            if (!string.IsNullOrEmpty(mensError))
            {
                return GnossResultERROR(mensError);
            }

            Guid baseRecursosPersonal = Guid.Empty;
            List<Guid> listaGuidProyectosEnvioTwitter = new List<Guid>();

            foreach (Guid brID in mBRsCatsAddToGnoss.Keys)
            {
                if (mGestoresAddToGnoss[brID].GestorDocumental == null)
                {
                    mGestoresAddToGnoss[brID].GestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
                }

                if (mGestoresAddToGnoss[brID].GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursos.Count == 0)
                {
                    //Recupero la base de recursos
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    if (mGestoresAddToGnoss[brID].EsBaseRecursosPersonal)
                    {
                        docCN.ObtenerBaseRecursosUsuario(mGestoresAddToGnoss[brID].GestorDocumental.DataWrapperDocumentacion, mControladorBase.UsuarioActual.UsuarioID);
                    }
                    else if (mGestoresAddToGnoss[brID].EsBaseRecursosOrganizacion)
                    {
                        docCN.ObtenerBaseRecursosOrganizacion(mGestoresAddToGnoss[brID].GestorDocumental.DataWrapperDocumentacion, mGestoresAddToGnoss[brID].OrganizacionBRID);
                    }
                    else
                    {
                        docCN.ObtenerBaseRecursosProyecto(mGestoresAddToGnoss[brID].GestorDocumental.DataWrapperDocumentacion, mGestoresAddToGnoss[brID].ProyectoBRID, mGestoresAddToGnoss[brID].OrganizacionBRID, UsuarioActual.UsuarioID);
                    }
                    docCN.Dispose();
                }

                if (mGestoresAddToGnoss[brID].EsBaseRecursosPersonal)
                {
                    baseRecursosPersonal = mGestoresAddToGnoss[brID].GestorDocumental.BaseRecursosIDActual;
                }
            }

            bool documentoAgregado = false;
            Documento doc = null;
            Guid documentoID = Guid.NewGuid();

            if (!string.IsNullOrEmpty(RequestParams("idtemporal")))
            {
                documentoID = new Guid(RequestParams("idtemporal"));
            }

            Guid identidadRedireccion = Guid.Empty;
            DataWrapperDocumentacion docDefinitivoDW = new DataWrapperDocumentacion();

            //Guardo los datos:
            if (baseRecursosPersonal != Guid.Empty)
            {
                ControladorDocumentacion.RecargarGestorAddToGnoss(mGestoresAddToGnoss[baseRecursosPersonal], UsuarioActual.UsuarioID);
                doc = CrearDocumentoEnModeloAcido_AnyadirGnoss(documentoID, mBRsCatsAddToGnoss[baseRecursosPersonal], baseRecursosPersonal);
                documentoAgregado = true;
                identidadRedireccion = mGestoresAddToGnoss[baseRecursosPersonal].IdentidadEnProyecto;
                mGestoresAddToGnoss[baseRecursosPersonal].GestorDocumental.GestorTesauro = null;
            }

            List<Guid> listaProyectosActualizarNumRec = new List<Guid>();

            foreach (Guid brID in mBRsCatsAddToGnoss.Keys)
            {
                ControladorDocumentacion.RecargarGestorAddToGnoss(mGestoresAddToGnoss[brID], UsuarioActual.UsuarioID);
                if (mGestoresAddToGnoss[brID].GestorDocumental.BaseRecursosIDActual != baseRecursosPersonal)
                {
                    if (!documentoAgregado)
                    {
                        doc = CrearDocumentoEnModeloAcido_AnyadirGnoss(documentoID, mBRsCatsAddToGnoss[brID], brID);
                        documentoAgregado = true;
                        identidadRedireccion = mGestoresAddToGnoss[brID].IdentidadEnProyecto;
                    }
                    else
                    {
                        if (GestorDocumental.GestorIdentidades == null)
                        {
                            GestorDocumental.GestorIdentidades = this.IdentidadActual.GestorIdentidades;
                        }
                        else
                        {
                            GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(this.IdentidadActual.GestorIdentidades.DataWrapperIdentidad);
                            GestorDocumental.GestorIdentidades.RecargarHijos();
                        }

                        ControladorIdentidades controladorIdentidades = new ControladorIdentidades(this.IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorIdentidades>(), mLoggerFactory);
                        controladorIdentidades.CompletarCargaIdentidad(mGestoresAddToGnoss[brID].IdentidadEnProyecto);

                        GestorDocumental.CompartirRecursoEnVariasComunidades(doc, mGestoresAddToGnoss[brID]);
                    }

                    //Compruebo si la comunidad tiene twitter
                    if (mGestoresAddToGnoss[brID].TieneTwitter)
                    {
                        listaGuidProyectosEnvioTwitter.Add(mGestoresAddToGnoss[brID].ProyectoBRID);
                    }

                    if (!listaProyectosActualizarNumRec.Contains(mGestoresAddToGnoss[brID].ProyectoBRID))
                    {
                        listaProyectosActualizarNumRec.Add(mGestoresAddToGnoss[brID].ProyectoBRID);
                    }
                }
                mGestoresAddToGnoss[brID].GestorDocumental.GestorTesauro = null;
                docDefinitivoDW.Merge(GestorDocumental.DataWrapperDocumentacion);
            }

            if (doc.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count > 0)
            {
                docDefinitivoDW.ListaDocumento.Find(docu => docu.DocumentoID.Equals(doc.Clave)).OrganizacionID = doc.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion[0].OrganizacionID;
            }
            string error = null;

            if (mEsArchivoLocalAddToGnoss)
            {
                // David: Subir el archivo al servidor
                error = AgregarArchivo_AnyadirGnoss(doc);
            }

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }

            mEntityContext.SaveChanges();

            if (doc.TipoDocumentacion == TiposDocumentacion.Hipervinculo || doc.TipoDocumentacion == TiposDocumentacion.Nota || doc.TipoDocumentacion == TiposDocumentacion.VideoBrightcove || doc.TipoDocumentacion == TiposDocumentacion.VideoTOP || doc.EsVideoIncrustado || doc.EsPresentacionIncrustada)
            {
                ControladorDocumentacion.CapturarImagenWeb(doc.Clave, true, PrioridadColaDocumento.Alta, mAvailableServices);
            }

            GestorDocumental gestorDocumentalSuperAux = new GestorDocumental(docDefinitivoDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
            gestorDocumentalSuperAux.CargarDocumentos(false);

            if (baseRecursosPersonal != Guid.Empty)
            {
                GestorAddToGnoss gestAddTo = mGestoresAddToGnoss[baseRecursosPersonal];
                ControladorDocumentacion.RecargarGestorAddToGnoss(gestAddTo, UsuarioActual.UsuarioID);
                gestorDocumentalSuperAux.GestorTesauro = gestAddTo.GestorDocumental.GestorTesauro;
                gestAddTo.GestorDocumental.GestorTesauro = null;
            }
            else
            {
                gestorDocumentalSuperAux.GestorTesauro = new GestionTesauro(new DataWrapperTesauro(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            }

            ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(gestorDocumentalSuperAux.ListaDocumentos[documentoID], IdentidadActual, true);

            foreach (Guid brID in mBRsCatsAddToGnoss.Keys)
            {
                try
                {
                    string infoExtra = null;

                    if (mGestoresAddToGnoss[brID].ProyectoBRID == ProyectoAD.MetaProyecto)
                    {
                        infoExtra = mGestoresAddToGnoss[brID].PerfilEnProyecto.ToString();
                    }

                    ControladorDocumentacion.ActualizarGnossLive(mGestoresAddToGnoss[brID].ProyectoBRID, doc.Clave, AccionLive.Agregado, (int)TipoLive.Recurso, PrioridadLive.Alta, infoExtra, mAvailableServices);
                    ControladorDocumentacion.ActualizarGnossLive(mGestoresAddToGnoss[brID].ProyectoBRID, mGestoresAddToGnoss[brID].IdentidadEnProyecto, AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);

                    ControladorDocumentacion.AgregarRecursoModeloBaseSimple(doc.Clave, mGestoresAddToGnoss[brID].ProyectoBRID, doc.FilaDocumento.Tipo, null, ",##enlaces##" + ExtraerTexto(mModelSaveRec.TagsLinks) + "##enlaces##", Es.Riam.Gnoss.AD.BASE_BD.PrioridadBase.Alta, mAvailableServices);

                    //Actualización Offline a partir de un servicio UDP
                    //Llamada asincrona para actualizar la popularidad del recurso:
                    ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("recursos", doc.Clave, mGestoresAddToGnoss[brID].ProyectoBRID, mGestoresAddToGnoss[brID].IdentidadEnProyecto, doc.CreadorID);

                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                }
            }

            //Agregada variable de sesión para el registro del evento Subir Recurso
            Session.Set("EventoSubirRecurso", true);

            if (listaGuidProyectosEnvioTwitter.Count > 0 && doc.FilaDocumento.Publico)
            {
                List<Proyecto> listaProyectosEnvioTwitter = new List<Proyecto>();

                ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                DataWrapperProyecto proyectoDS = new DataWrapperProyecto();

                foreach (Guid proyectoID in listaGuidProyectosEnvioTwitter)
                {
                    proyectoDS.Merge(proyectoCL.ObtenerProyectoPorID(proyectoID));
                }

                GestionProyecto gestProy = new GestionProyecto(proyectoDS, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                foreach (Proyecto proy in gestProy.ListaProyectos.Values)
                {
                    listaProyectosEnvioTwitter.Add(proy);
                }

                gestProy.Dispose();

                //Enviar mensaje a twitter
                ControladorDocumentacion.EnviarEnlaceATwitterDeComunidad(IdentidadActual, doc, listaProyectosEnvioTwitter, BaseURLIdioma, UtilIdiomas, UrlPerfil, mAvailableServices);
            }

            string urlDeVuelta = "";

            if (mEsArchivoLocalAddToGnoss)
            {
                string perfilUrl = "";
                string comUrl = "";

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                GestionIdentidades gestIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(identidadRedireccion, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();

                Identidad identidad = gestIdentidades.ListaIdentidades[identidadRedireccion];

                //Redirecciono a la ficha del recurso
                if (baseRecursosPersonal != Guid.Empty)
                {
                    perfilUrl = "/";
                    urlDeVuelta = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, "", perfilUrl, doc, false);
                }
                else
                {
                    if (identidad.FilaIdentidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))
                    {
                        //Redirecciono a la ficha del recurso subida a una BR de organización o a la personal
                        perfilUrl = $"/{mControladorBase.UrlsSemanticas.ObtenerURLOrganizacionOClase(UtilIdiomas, identidad.OrganizacionID.Value)}/{identidad.PerfilUsuario.NombreCortoOrg}/";
                    }
                    else
                    {
                        //Redirecciono a la ficha del recurso subida a una BR de comunidad
                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                        comUrl = proyCN.ObtenerNombreCortoProyecto(identidad.FilaIdentidad.ProyectoID);
                        proyCN.Dispose();
                    }

                    urlDeVuelta = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, comUrl, perfilUrl, doc, false);
                }

                urlDeVuelta += "?created";
            }
            else
            {
                urlDeVuelta = mModelSaveRec.Link;
            }

            gestorDocumentalSuperAux.Dispose();
            doc.GestorDocumental.Dispose();

            return GnossResultUrl(urlDeVuelta);
        }

        /// <summary>
        /// Agrega el archivo temporal de añadir a gnoss a la BR del usuario.
        /// </summary>
        /// <param name="pDocumento">Documento que se está creando</param>
        /// <returns>NULL si va bien o el ERROR que se ha producido</returns>
        private string AgregarArchivo_AnyadirGnoss(Documento pDocumento)
        {
            GestionDocumental gd = null;
            int resultado = 0;

            try
            {
                //Subimos el fichero al servidor
                gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                gd.Url = UrlServicioWebDocumentacion;
                byte[] bufferRecurso = gd.ObtenerRecursoTemporal(pDocumento.Clave, Path.GetExtension(mModelSaveRec.Link));

                double tamanoBytes = bufferRecurso.Length;
                double tamanoArchivoMB = ((tamanoBytes / 1024) / 1024);
                string extensionArchivo = System.IO.Path.GetExtension(mModelSaveRec.Link).ToLower();
                bool brProyecto = pDocumento.ProyectoID != ProyectoAD.MetaProyecto;
                Guid organizacionBRID = Guid.Empty;

                // David: Comprobar si el recurso se ha subido a una base de recursos de organización
                if (pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count > 0)
                {
                    organizacionBRID = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion[0].OrganizacionID;
                }

                if (!brProyecto && pDocumento.GestorDocumental.EspacioActualBaseRecursos + tamanoArchivoMB > pDocumento.GestorDocumental.EspacioMaximoBaseRecursos)
                {
                    resultado = 4;
                }
                else
                {
                    if (pDocumento.TipoDocumentacion == TiposDocumentacion.Video)
                    {
                        //Subimos el fichero al servidor

                        ServicioVideos servicioVideos = new ServicioVideos(mConfigService, mLoggingService, mLoggerFactory.CreateLogger<ServicioVideos>(), mLoggerFactory);
                        if (brProyecto)
                        {
                            resultado = servicioVideos.AgregarVideo(bufferRecurso, extensionArchivo, pDocumento.Clave);
                        }
                        else
                        {
                            if (organizacionBRID != Guid.Empty)
                            {
                                resultado = servicioVideos.AgregarVideoOrganizacion(bufferRecurso, extensionArchivo, pDocumento.Clave, organizacionBRID);
                            }
                            else
                            {
                                resultado = servicioVideos.AgregarVideoPersonal(bufferRecurso, extensionArchivo, pDocumento.Clave, mControladorBase.UsuarioActual.PersonaID);
                            }
                        }

                    }
                    else if (pDocumento.TipoDocumentacion == TiposDocumentacion.Imagen)
                    {
                        Image imagen = Image.Load(new MemoryStream(bufferRecurso));

                        int anchoMaximo = 582;

                        float ancho = imagen.Width;
                        float alto = imagen.Height;
                        if (ancho > anchoMaximo)
                        {
                            ancho = anchoMaximo;
                            alto = (imagen.Height * ancho) / imagen.Width;
                        }

                        Image imagenPeque = UtilImages.AjustarImagen(imagen, ancho, alto);

                        byte[] bufferReducido = UtilImages.ImageToBytePng(imagenPeque);

                        ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                        servicioImagenes.Url = UrlIntragnossServicios;
                        bool correcto = false;

                        if (brProyecto)
                        {
                            correcto = servicioImagenes.AgregarImagenADirectorio(bufferReducido, UtilArchivos.ContentImagenesDocumentos + "/" + UtilArchivos.DirectorioDocumento(pDocumento.Clave), pDocumento.Clave.ToString(), extensionArchivo);
                        }
                        else
                        {
                            if (organizacionBRID != Guid.Empty)
                            {
                                correcto = servicioImagenes.AgregarImagenDocumentoOrganizacion(bufferReducido, pDocumento.Clave.ToString(), extensionArchivo, organizacionBRID);

                            }
                            else
                            {
                                correcto = servicioImagenes.AgregarImagenDocumentoPersonal(bufferReducido, pDocumento.Clave.ToString(), extensionArchivo, mControladorBase.UsuarioActual.PersonaID);
                            }
                        }

                        if (correcto)
                        {
                            resultado = 1;
                        }
                    }
                    else if (pDocumento.TipoDocumentacion == TiposDocumentacion.FicheroServidor)
                    {
                        string idAuxGestorDocumental = "";

                        if (brProyecto)
                        {
                            idAuxGestorDocumental = gd.AdjuntarDocumento(bufferRecurso, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, pDocumento.FilaDocumento.OrganizacionID, pDocumento.FilaDocumento.ProyectoID.Value, pDocumento.Clave, extensionArchivo);
                        }
                        else
                        {
                            if (organizacionBRID != Guid.Empty)
                            {
                                idAuxGestorDocumental = gd.AdjuntarDocumentoABaseRecursosOrganizacion(bufferRecurso, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, organizacionBRID, pDocumento.Clave, extensionArchivo);
                            }
                            else
                            {
                                idAuxGestorDocumental = gd.AdjuntarDocumentoABaseRecursosUsuario(bufferRecurso, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, pDocumento.Clave, extensionArchivo);
                            }
                        }

                        if (!idAuxGestorDocumental.ToUpper().Equals("ERROR"))
                        {
                            resultado = 1;
                        }
                    }
                }

                switch (resultado)
                {
                    case 0:
                        {
                            return UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC");
                        }
                    case 2:
                        {
                            return UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSEGURIDAD");
                        }
                    case 3:
                        {
                            return UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSIZE");
                        }
                    case 4:
                        {
                            return UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSIZEMAXIMO");
                        }
                    case 1:
                        {
                            // David: Eliminar la ruta temporald el archivo
                            gd.BorrarRecursoTemporal(pDocumento.Clave, Path.GetExtension(mModelSaveRec.Link));
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                //Se han producido errores al guardar el documento en el servidor
                return UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC") + " - " + ex.Message;
            }

            return null;
        }

        /// <summary>
        /// Crea un documento en el modelo ácido.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento que se va a crear</param>
        /// <param name="pCategoriasSeleccionadas">Categorías del tesauro seleccionadas</param>
        /// <param name="pBaseRecursosID">Base de recursos en la que se debe agregar el recurso</param>
        /// <returns>Documento creado</returns>
        private Documento CrearDocumentoEnModeloAcido_AnyadirGnoss(Guid pDocumentoID, List<Guid> pCategoriasSeleccionadas, Guid pBaseRecursosID)
        {
            GestorDocumental = mGestoresAddToGnoss[pBaseRecursosID].GestorDocumental;
            if (GestorDocumental.GestorIdentidades == null)
            {
                GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
            }
            else
            {
                GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(IdentidadActual.GestorIdentidades.DataWrapperIdentidad);
                GestorDocumental.GestorIdentidades.RecargarHijos();
            }
            ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorIdentidades>(), mLoggerFactory);
            controladorIdentidades.CompletarCargaIdentidad(mGestoresAddToGnoss[pBaseRecursosID].IdentidadEnProyecto);

            #region Documento Tipo documento

            Documento doc = null;
            string titulo = UtilCadenas.EliminarHtmlDeTexto(mModelSaveRec.Title.Trim());
            string descripcion = mModelSaveRec.Description;
            string tags = UtilCadenas.EliminarHtmlDeTexto(mModelSaveRec.Tags.Trim());
            string rutaFichero = null;
            Guid elementoVinculadoID = Guid.Empty;

            if (mTipoDocumento == TiposDocumentacion.FicheroServidor || mTipoDocumento == TiposDocumentacion.Imagen || mTipoDocumento == TiposDocumentacion.ReferenciaADoc || mTipoDocumento == TiposDocumentacion.Semantico || mTipoDocumento == TiposDocumentacion.Nota || mTipoDocumento == TiposDocumentacion.Newsletter || mTipoDocumento == TiposDocumentacion.Pregunta || mTipoDocumento == TiposDocumentacion.Debate || mTipoDocumento == TiposDocumentacion.Encuesta)
            {
                if (pDocumentoID != Guid.Empty)
                {
                    //Añadimos el documento a la definición y a los dataset que corresponda
                    rutaFichero = mNombreDocumento;
                }
            }
            else//Es enlace
            {
                rutaFichero = mModelSaveRec.Link.Trim();
            }

            if (rutaFichero == null)
            {
                if (mTipoDocumento != TiposDocumentacion.Newsletter && mTipoDocumento != TiposDocumentacion.Encuesta && mTipoDocumento != TiposDocumentacion.Pregunta && mTipoDocumento != TiposDocumentacion.Debate)
                {
                    throw new ExcepcionWeb("Enlace vacío");
                }
                else
                {
                    rutaFichero = "";
                }
            }

            doc = GestorDocumental.AgregarDocumento(rutaFichero, titulo, descripcion, tags, mTipoDocumento, TipoEntidadVinculadaDocumento.Web, elementoVinculadoID, mModelSaveRec.ShareAllowed, mModelSaveRec.Draft, mModelSaveRec.CreatorIsAuthor, null, false, UsuarioActual.OrganizacionID, UsuarioActual.IdentidadID);

            //Cambiar identificador del documento
            if (GestorDocumental.ListaDocumentos.ContainsKey(doc.Clave))
            {
                GestorDocumental.ListaDocumentos.Remove(doc.Clave);
                GestorDocumental.ListaDocumentos.Add(pDocumentoID, doc);
            }
            else
            {
                GestorDocumental.ListaDocumentos.Add(pDocumentoID, doc);
            }
            doc.FilaDocumento.DocumentoID = pDocumentoID;

            #endregion

            //Agrego la comunidad e indentidad a la que pertenece el documento:
            doc.FilaDocumento.ProyectoID = mGestoresAddToGnoss[pBaseRecursosID].ProyectoBRID;
            doc.FilaDocumento.CreadorID = mGestoresAddToGnoss[pBaseRecursosID].IdentidadEnProyecto;

            List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

            foreach (Guid clave in pCategoriasSeleccionadas)
            {
                CategoriaTesauro categoria = GestorDocumental.GestorTesauro.ListaCategoriasTesauro[clave];
                listaCategorias.Add(categoria);
            }

            GestorDocumental.AgregarDocumento(listaCategorias, doc, mGestoresAddToGnoss[pBaseRecursosID].IdentidadEnProyecto, pBaseRecursosID, false, UsuarioActual.UsuarioID);

            //Asignamos el doc de nuevo porque se crea una nueva instancia en los métodos anteriores:
            doc = GestorDocumental.ListaDocumentos[doc.Clave];

            ExtraerCambiosBasicos(doc);
            GuardarDatosAutoriaDocumento(doc);

            //Agrego el usuario actual como editor
            GestorDocumental.AgregarEditorARecurso(doc.Clave, mGestoresAddToGnoss[pBaseRecursosID].PerfilEnProyecto);

            if (!string.IsNullOrEmpty(mModelSaveRec.License) && mModelSaveRec.CreatorIsAuthor && (mModelSaveRec.ShareAllowed))
            {
                doc.FilaDocumento.Licencia = mModelSaveRec.License;
            }

            return doc;
        }

        /// <summary>
        /// Valida si los datos de documento son correctos o no.
        /// </summary>
        /// <param name="pCategoriasSelec">Lista con las BRs y categorías seleccionadas</param>
        /// <returns>NULL si los datos son correctos, Texto con el error en caso contario.</returns>
        private string ValidarRecurso_AnyadirGnoss(Dictionary<Guid, List<Guid>> pCategoriasSelec)
        {
            string titulo = UtilCadenas.EliminarHtmlDeTexto(ExtraerTexto(mModelSaveRec.Title).Trim());
            string descripcion = HttpUtility.HtmlDecode(ExtraerTexto(mModelSaveRec.Description));
            string tags = UtilCadenas.EliminarHtmlDeTexto(ExtraerTexto(mModelSaveRec.Tags).Trim());

            if (mTipoDocumento == TiposDocumentacion.Hipervinculo && (ExtraerTexto(mModelSaveRec.Link).Trim().Equals(string.Empty) || !Uri.IsWellFormedUriString(ExtraerTexto(mModelSaveRec.Link).Trim(), UriKind.Absolute)))
            {
                return UtilIdiomas.GetText("ANYADIRGNOSS", "ERORR_URL_MALA");
            }

            if (string.IsNullOrEmpty(titulo))
            {
                return UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DTITULO");
            }

            if (string.IsNullOrEmpty(descripcion))
            {
                return UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DDESCRIPCION");
            }

            if (string.IsNullOrEmpty(tags))
            {
                return UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DTAG");
            }

            if (pCategoriasSelec.Count == 0)
            {
                return UtilIdiomas.GetText("ANYADIRGNOSS", "ERORR_NO_BR");
            }

            if (!string.IsNullOrEmpty(mModelSaveRec.License))
            {
                foreach (Guid br in pCategoriasSelec.Keys)
                {
                    if (mGestoresAddToGnoss[br].LicenciaProyectoPorDefecto != null && mModelSaveRec.CreatorIsAuthor && mModelSaveRec.License != mGestoresAddToGnoss[br].LicenciaProyectoPorDefecto)
                    {
                        return UtilIdiomas.GetText("CONTROLESDOCUMENTACION", "COMPARTIRERRORDISTLICENCIAENPROY", ObtenerCodigoCCSegunCodigoLicencia(mGestoresAddToGnoss[br].LicenciaProyectoPorDefecto), mGestoresAddToGnoss[br].NombreProyecto);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene el código de Creative Commons a partir del código de la licencia del documento.
        /// </summary>
        /// <param name="pLicencia">Licencia del documento</param>
        /// <returns>Código de Creative Commons a partir del código de la licencia del documento</returns>
        private static string ObtenerCodigoCCSegunCodigoLicencia(string pLicencia)
        {
            if (pLicencia == "00")
            {
                return "by";
            }
            else if (pLicencia == "01")
            {
                return "by-sa";
            }
            else if (pLicencia == "02")
            {
                return "by-nd";
            }
            else if (pLicencia == "10")
            {
                return "by-nc";
            }
            else if (pLicencia == "11")
            {
                return "by-nc-sa";
            }
            else if (pLicencia == "12")
            {
                return "by-nc-nd";
            }

            return null;
        }

        /// <summary>
        /// Recoge las BRs y las categorías seleccionadas de cada una de ellas.
        /// </summary>
        /// <returns>BRs y las categorías seleccionadas de cada una de ellas</returns>
        private Dictionary<Guid, List<Guid>> RecogerBRsYCategoriasSeleccionadas_AnyadirGnoss()
        {
            Dictionary<Guid, List<Guid>> brCats = new Dictionary<Guid, List<Guid>>();

            DocumentacionCN documCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            Guid baseRecursosID = documCN.ObtenerBaseRecursosIDProyecto(ProyectoSeleccionado.Clave);
            documCN.Dispose();

            foreach (string brCat in mModelSaveRec.SelectedCategories.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid br;
                if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                {
                    br = new Guid(brCat.Split('|')[0]);
                    brCats.Add(br, new List<Guid>());

                    foreach (string cat in brCat.Split('|')[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        brCats[br].Add(new Guid(cat));
                    }
                }
                else
                {
                    br = baseRecursosID;
                    brCats.Add(br, new List<Guid>());

                    foreach (string cat in brCat.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        brCats[br].Add(new Guid(cat));
                    }
                }

                if (brCats[br].Count == 0)
                {
                    brCats.Remove(br);
                }
                else
                {
                    mGestoresAddToGnoss[br].EstaAgregado = true;
                }
            }

            return brCats;
        }

        private void AjustarVisibilidad_AnyadirGnoss()
        {
            mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = false;
            mEditRecCont.ModifyResourceModel.EditTitleAvailable = true;
            mEditRecCont.ModifyResourceModel.EditDescriptionAvailable = true;
            mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = false;
            mEditRecCont.ModifyResourceModel.ResourcePropertiesAvailable = true;
            mEditRecCont.ModifyResourceModel.ShareAvailable = true;
            mEditRecCont.ModifyResourceModel.SelectorEditionAvailable = false;
            mEditRecCont.ModifyResourceModel.DocumentEditionModel.ModificationProtectionAvailable = true;
            mEditRecCont.ModifyResourceModel.ResourceProtectionAvailable = false;
            mEditRecCont.ModifyResourceModel.DocumentEditionModel.ShareAllowed = true;

            TipoPagina = TiposPagina.BaseRecursos;
            mEditRecCont.ModifyResourceModel.CurrentSiteMenuPulg = UtilIdiomas.GetText("ANYADIRGNOSS", "TIT_ANYADIRGNOSS");
            ViewBag.TituloPagina = UtilIdiomas.GetText("ANYADIRGNOSS", "TIT_ANYADIRGNOSS");
        }

        /// <summary>
        /// Recoge los parámetro para añadir a GNOSS.
        /// </summary>
        private void RecogerParametros_AnyadirGnoss()
        {
            if (mEditRecCont == null)
            {
                mEditRecCont = new EditResourceModel();
            }

            if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                mEditRecCont.TypePage = EditResourceModel.TypePageEditResource.AddToGnossResource;
            }
            else
            {
                mEditRecCont.TypePage = EditResourceModel.TypePageEditResource.AddToCommunityResource;
            }

            ModifyResourceModel modelModRec = new ModifyResourceModel();
            modelModRec.EditResourceModel = mEditRecCont;
            mEditRecCont.ModifyResourceModel = modelModRec;
            mEditRecCont.ModifyResourceModel.EditPropertiesAvailable = true;
            modelModRec.DocumentEditionModel = new DocumentEditionModel();
            modelModRec.DocumentEditionModel.ActualIdentityIsCreator = true;
            modelModRec.CopyrightAvailable = true;

            mTipoDocumento = TiposDocumentacion.Hipervinculo;

            if (RequestParams("addToGnoss") != null || mCookieAnyadirGnoss != null)
            {
                if (RequestParams("archivolocal") != null)
                {
                    mEsArchivoLocalAddToGnoss = bool.Parse(RequestParams("archivolocal"));
                }

                if (mEsArchivoLocalAddToGnoss)
                {
                    mTipoDocumento = TiposDocumentacion.FicheroServidor;
                    modelModRec.DocumentEditionModel.AllowsLicense = true;
                }

                mNombreDocumento = RequestParams("addToGnoss");
                modelModRec.DocumentEditionModel.Link = mNombreDocumento;

                Dictionary<string, string> simbolosCaracteresnNopermitidos = new Dictionary<string, string>(AddToGnoss.SimbolosCaracteresnNopermitidos);

                foreach (string simbolo in simbolosCaracteresnNopermitidos.Keys)
                {
                    TituloAddTo = TituloAddTo.Replace(simbolo, simbolosCaracteresnNopermitidos[simbolo]);
                }

                foreach (string simbolo in simbolosCaracteresnNopermitidos.Keys)
                {
                    DescripcionAddTo = DescripcionAddTo.Replace(simbolo, simbolosCaracteresnNopermitidos[simbolo]);
                }

                modelModRec.DocumentEditionModel.Title = TituloAddTo;
                modelModRec.DocumentEditionModel.Description = DescripcionAddTo.Trim().Replace("\n", "<p>");

                if (string.IsNullOrEmpty(mNombreDocumento) && mModelSaveRec != null)
                {
                    mNombreDocumento = mModelSaveRec.Link;
                }
            }
            else if (mModelSaveRec != null)
            {
                mNombreDocumento = mModelSaveRec.Link;
            }

            modelModRec.DocumentType = (ResourceModel.DocumentType)mTipoDocumento;

            if (mEsArchivoLocalAddToGnoss)
            {
                bool hayActualizacionHerramienta = true;

                //Compruebo si hay actualizaciones para las herramientas GNOSS del usuario
                if (RequestParams("versionHerramienta") != null && !HayActualizacionesDisponibles(RequestParams("versionHerramienta")))
                {
                    hayActualizacionHerramienta = false;
                }

                if (hayActualizacionHerramienta)
                {
                    mEditRecCont.ModifyResourceModel.NewVersionMessageAddToGnossOfficeAvailable = UtilIdiomas.GetText("ACTUALIZACIONES", "ACTUALIZACIONESDISPONIBLES", BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "ACTUALIZACIONESHERRAMIENTASGNOSS")).Replace("p>", "div>");
                }
            }
            else if (mCookieAnyadirGnoss != null)
            {
                string versionAddTo = mCookieAnyadirGnoss["verAddTo"];

                if (versionAddTo == null || (versionAddTo != "" && !AddToGnoss.EsVersionActual(versionAddTo)))
                {
                    mEditRecCont.ModifyResourceModel.NewVersionMessageAddToGnossAvailable = UtilIdiomas.GetText("ACTUALIZACIONES", "ACTUALIZACIONADDTOGNOSSNAVGDISP", BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "HERRAMIENTASGNOSS"));
                }
            }
        }

        /// <summary>
        /// Comprueba si hay una versión más actual del Add to GNOSS
        /// </summary>
        /// <param name="pVersionProducto">Versión del producto</param>
        /// <returns>TRUE si hay actualizaciones disponibles, FALSE en caso contrario</returns>
        private bool HayActualizacionesDisponibles(string pVersionProducto)
        {
            if (!string.IsNullOrEmpty(pVersionProducto))
            {
                char[] separadores = { '.' };
                string[] version = pVersionProducto.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                string[] versionActual = (ParametrosAplicacionDS.Find(parametro => parametro.Parametro.Equals("usarHTTPSParaDominioPrincipal")).Valor).Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                int indice = 0;

                foreach (string numero in version)
                {
                    int numeroVersionUsuario = int.Parse(numero);
                    int numeroVersionActual = int.Parse(versionActual[indice]);

                    if (numeroVersionActual > numeroVersionUsuario)
                    {
                        return true;
                    }
                    indice++;
                }
            }
            return false;
        }

        /// <summary>
        /// Carga las Base de recursos disponibles para el usuario donde podrá compartir el recurso add to Gnoss.
        /// </summary>
        /// <param name="pCargarGestoresAddToGnoss">Indica si se deben cargar los gestores Add To Gnoss</param>
        private Dictionary<Guid, string> CargarBaseRecursosUsuario_AnyadirGnoss(bool pCargarGestoresAddToGnoss)
        {
            TiposDocumentacion tipoRecurso = TiposDocumentacion.Hipervinculo;

            if (pCargarGestoresAddToGnoss)
            {
                mGestoresAddToGnoss = new Dictionary<Guid, GestorAddToGnoss>();
            }

            if (mEsArchivoLocalAddToGnoss)
            {
                TipoArchivo tipoArchivo = UtilArchivos.ObtenerTipoArchivo(mNombreDocumento);
                tipoRecurso = TiposDocumentacion.FicheroServidor;

                if (tipoArchivo == TipoArchivo.Audio || tipoArchivo == TipoArchivo.Video)
                {
                    tipoRecurso = TiposDocumentacion.Video;
                }
                else if (tipoArchivo == TipoArchivo.Imagen)
                {
                    tipoRecurso = TiposDocumentacion.Imagen;
                }
            }

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto dataWrapperProyecto = null;

            if (mEnvioMetaGnossAddToGnoss)
            {
                dataWrapperProyecto = proyectoCN.ObtenerProyectoPorID(ProyectoAD.MetaGNOSS);
            }
            else
            {
                dataWrapperProyecto = proyectoCN.ObtenerProyectosUsuarioPuedeCompartirRecurso(mControladorBase.UsuarioActual.UsuarioID, tipoRecurso);
            }

            proyectoCN.Dispose();
            GestionProyecto gestorProyectos = new GestionProyecto(dataWrapperProyecto, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);

            List<Guid> listaProyectosConTwitter = new List<Guid>();

            foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in dataWrapperProyecto.ListaProyecto.Where(proy => proy.TieneTwitter == true))
            {
                if ((filaProy.TieneTwitter))
                {
                    listaProyectosConTwitter.Add(filaProy.ProyectoID);
                }
            }

            Dictionary<Identidad, string> listaIdentidadNombrePerfil = new Dictionary<Identidad, string>();
            SortedDictionary<string, List<SortedDictionary<string, Identidad>>> listaPerfilIdentProy = new SortedDictionary<string, List<SortedDictionary<string, Identidad>>>();
            SortedDictionary<string, Identidad> listaIdentidadProyPERFILPERSONAL = new SortedDictionary<string, Identidad>();
            SortedDictionary<string, Identidad> listaIdentidadProyPERFILPROFESOR = new SortedDictionary<string, Identidad>();
            SortedDictionary<string, Identidad> listaIdentidadProyPERFILORGANIZACION = new SortedDictionary<string, Identidad>();

            foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.PersonaID.Equals(IdentidadActual.PersonaID)).ToList())
            {
                Perfil perfil = IdentidadActual.GestorIdentidades.ListaPerfiles[filaPerfil.PerfilID];

                bool BRorganizacionConPermiso = (!perfil.PersonaID.HasValue && (Guid)UtilReflection.GetValueReflection(perfil.FilaRelacionPerfil, "OrganizacionID") == perfil.OrganizacionID && (mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion, (Guid)UtilReflection.GetValueReflection(perfil.FilaRelacionPerfil, "OrganizacionID")) || mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, (Guid)UtilReflection.GetValueReflection(perfil.FilaRelacionPerfil, "OrganizacionID"))));

                //Si el perfil es del usuario actual y si es perfil de organización, solo si es administrador
                if ((perfil.PersonaID.HasValue && (Guid)UtilReflection.GetValueReflection(perfil.FilaRelacionPerfil, "PersonaID") == mControladorBase.UsuarioActual.PersonaID) || BRorganizacionConPermiso)
                {
                    SortedDictionary<string, Identidad> listaIdentidadProy = new SortedDictionary<string, Identidad>();

                    foreach (Identidad identidad in perfil.Hijos)
                    {
                        if (gestorProyectos.ListaProyectos.ContainsKey(identidad.FilaIdentidad.ProyectoID) && (!identidad.Clave.Equals(UsuarioAD.Invitado)))
                        {
                            //Comprobamos que no hay proyectos con el mismo nombre...
                            if (!listaIdentidadProy.ContainsKey(gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto.Nombre))
                            {
                                listaIdentidadProy.Add(gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto.Nombre, identidad);
                            }

                            if (identidad.ModoParticipacion.Equals(TiposIdentidad.ProfesionalCorporativo))
                            {
                                listaIdentidadNombrePerfil.Add(identidad, identidad.NombreOrganizacion);
                            }
                            else if (identidad.ModoParticipacion == TiposIdentidad.Organizacion)
                            {
                                if (identidad.FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto)
                                {
                                    listaIdentidadProyPERFILORGANIZACION.Add(UtilIdiomas.GetText("ANYADIRGNOSS", "BRORGANIZACION") + perfil.NombreOrganizacion, identidad);
                                }
                            }
                            else
                            {
                                listaIdentidadNombrePerfil.Add(identidad, identidad.Nombre(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave)));
                            }
                        }
                    }

                    if (!perfil.OrganizacionID.HasValue)
                    {
                        if (perfil.IdentidadMyGNOSS != null)
                        {
                            if (perfil.IdentidadMyGNOSS.Tipo == TiposIdentidad.Profesor)
                            {
                                listaIdentidadProyPERFILPROFESOR = listaIdentidadProy;
                            }
                            else
                            {
                                listaIdentidadProyPERFILPERSONAL = listaIdentidadProy;
                            }
                        }
                    }
                    else if (perfil.PersonaID.HasValue)
                    {
                        if (listaPerfilIdentProy.ContainsKey(perfil.Nombre))
                        {
                            listaPerfilIdentProy[perfil.Nombre].Add(listaIdentidadProy);
                        }
                        else
                        {
                            List<SortedDictionary<string, Identidad>> listaDeLista = new List<SortedDictionary<string, Identidad>>();
                            listaDeLista.Add(listaIdentidadProy);
                            listaPerfilIdentProy.Add(perfil.Nombre, listaDeLista);
                        }
                    }
                }
            }

            #region Cargamos listaProyIDBRID y licenciadefecto

            List<Guid> listaProyCompartirID = new List<Guid>();
            foreach (Guid proyID in gestorProyectos.ListaProyectos.Keys)
            {
                listaProyCompartirID.Add(proyID);
            }
            Dictionary<Guid, Guid> listaProyIDBRID = new Dictionary<Guid, Guid>();
            Dictionary<Guid, string> listaProyIDLiceciaDefecto = new Dictionary<Guid, string>();

            if (listaProyCompartirID.Count > 0)
            {
                DocumentacionCN documCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                listaProyIDBRID = documCN.ObtenerBasesRecursosIDporProyectosID(listaProyCompartirID);
                documCN.Dispose();

                ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCN>(), mLoggerFactory);
                listaProyIDLiceciaDefecto = paramCN.ObtenerLicenciasporProyectosID(listaProyCompartirID);
                paramCN.Dispose();
            }

            #endregion

            Dictionary<Guid, string> addToGnossShareSites = new Dictionary<Guid, string>();

            //Agrego la BR personal
            foreach (Identidad identidad in listaIdentidadProyPERFILPERSONAL.Values)
            {
                if (identidad.FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto && gestorProyectos.ListaProyectos.ContainsKey(identidad.FilaIdentidad.ProyectoID))
                {
                    Guid baseRecursosID = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory).ObtenerBaseRecursosIDUsuario(mControladorBase.UsuarioActual.UsuarioID);

                    if (pCargarGestoresAddToGnoss)
                    {
                        GestorAddToGnoss gestorAddtoGnoss = new GestorAddToGnoss();
                        gestorAddtoGnoss.EsBaseRecursosPersonal = true;
                        gestorAddtoGnoss.EsBaseRecursosOrganizacion = false;
                        gestorAddtoGnoss.IdentidadEnProyecto = identidad.Clave;
                        gestorAddtoGnoss.ProyectoBRID = ProyectoAD.MetaProyecto;
                        gestorAddtoGnoss.OrganizacionBRID = ProyectoAD.MetaOrganizacion;
                        gestorAddtoGnoss.PerfilEnProyecto = identidad.PerfilID;
                        mGestoresAddToGnoss.Add(baseRecursosID, gestorAddtoGnoss);
                    }
                    else
                    {
                        addToGnossShareSites.Add(baseRecursosID, UtilIdiomas.GetText("ANYADIRGNOSS", "BRPERSONAL"));
                    }
                    break;
                }
            }

            //Agrego las BRs en proyectos con perfil personal
            foreach (Identidad identidad in listaIdentidadProyPERFILPERSONAL.Values)
            {
                if (identidad.FilaIdentidad.ProyectoID != ProyectoAD.MetaProyecto && gestorProyectos.ListaProyectos.ContainsKey(identidad.FilaIdentidad.ProyectoID))
                {
                    Guid baseRecursosID = listaProyIDBRID[identidad.FilaIdentidad.ProyectoID];
                    AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto;

                    if (pCargarGestoresAddToGnoss)
                    {
                        GestorAddToGnoss gestorAddtoGnoss = new GestorAddToGnoss();
                        gestorAddtoGnoss.EsBaseRecursosPersonal = false;
                        gestorAddtoGnoss.EsBaseRecursosOrganizacion = false;
                        gestorAddtoGnoss.IdentidadEnProyecto = identidad.Clave;
                        gestorAddtoGnoss.ProyectoBRID = identidad.FilaIdentidad.ProyectoID;
                        gestorAddtoGnoss.OrganizacionBRID = identidad.FilaIdentidad.OrganizacionID;
                        gestorAddtoGnoss.PerfilEnProyecto = identidad.PerfilID;
                        gestorAddtoGnoss.EsBaseRecursosPublica = filaProy.TipoAcceso == (short)TipoAcceso.Publico || filaProy.TipoAcceso == (short)TipoAcceso.Restringido;
                        gestorAddtoGnoss.NombreProyecto = filaProy.Nombre;

                        string licenciaPorDefecto = listaProyIDLiceciaDefecto[filaProy.ProyectoID];

                        if (!string.IsNullOrEmpty(licenciaPorDefecto))
                        {
                            gestorAddtoGnoss.LicenciaProyectoPorDefecto = licenciaPorDefecto;
                        }

                        gestorAddtoGnoss.TieneTwitter = listaProyectosConTwitter.Contains(gestorAddtoGnoss.ProyectoBRID);

                        mGestoresAddToGnoss.Add(baseRecursosID, gestorAddtoGnoss);
                    }
                    else
                    {
                        string nombreProy = filaProy.Nombre;
                        addToGnossShareSites.Add(baseRecursosID, nombreProy + " - " + UtilIdiomas.GetText("ANYADIRGNOSS", "PERSONAL"));
                    }
                }
            }

            //Agrego las BRs de los proyectos con perfil de profesor
            foreach (Identidad identidad in listaIdentidadProyPERFILPROFESOR.Values)
            {
                if (identidad.FilaIdentidad.ProyectoID != ProyectoAD.MetaProyecto && gestorProyectos.ListaProyectos.ContainsKey(identidad.FilaIdentidad.ProyectoID))
                {
                    Guid baseRecursosID = listaProyIDBRID[identidad.FilaIdentidad.ProyectoID];
                    AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto;

                    if (pCargarGestoresAddToGnoss)
                    {
                        GestorAddToGnoss gestorAddtoGnoss = new GestorAddToGnoss();
                        gestorAddtoGnoss.EsBaseRecursosPersonal = false;
                        gestorAddtoGnoss.EsBaseRecursosOrganizacion = false;
                        gestorAddtoGnoss.IdentidadEnProyecto = identidad.Clave;
                        gestorAddtoGnoss.ProyectoBRID = identidad.FilaIdentidad.ProyectoID;
                        gestorAddtoGnoss.OrganizacionBRID = identidad.FilaIdentidad.OrganizacionID;
                        gestorAddtoGnoss.PerfilEnProyecto = identidad.PerfilID;
                        gestorAddtoGnoss.EsBaseRecursosPublica = filaProy.TipoAcceso == (short)TipoAcceso.Publico || filaProy.TipoAcceso == (short)TipoAcceso.Restringido;
                        gestorAddtoGnoss.NombreProyecto = filaProy.Nombre;

                        string licenciaPorDefecto = listaProyIDLiceciaDefecto[filaProy.ProyectoID];

                        if (!string.IsNullOrEmpty(licenciaPorDefecto))
                        {
                            gestorAddtoGnoss.LicenciaProyectoPorDefecto = licenciaPorDefecto;
                        }

                        gestorAddtoGnoss.TieneTwitter = listaProyectosConTwitter.Contains(gestorAddtoGnoss.ProyectoBRID);

                        mGestoresAddToGnoss.Add(baseRecursosID, gestorAddtoGnoss);
                    }
                    else
                    {
                        string nombreProy = filaProy.Nombre;
                        addToGnossShareSites.Add(baseRecursosID, nombreProy + " - " + UtilIdiomas.GetText("ANYADIRGNOSS", "PROFESOR"));
                    }
                }
            }

            foreach (List<SortedDictionary<string, Identidad>> listaDeListas in listaPerfilIdentProy.Values)
            {
                foreach (SortedDictionary<string, Identidad> listIdent in listaDeListas)
                {
                    foreach (Identidad identidad in listIdent.Values)
                    {
                        if (identidad.FilaIdentidad.ProyectoID != ProyectoAD.MetaProyecto && gestorProyectos.ListaProyectos.ContainsKey(identidad.FilaIdentidad.ProyectoID))
                        {
                            Guid baseRecursosID = listaProyIDBRID[identidad.FilaIdentidad.ProyectoID];
                            AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto;

                            if (pCargarGestoresAddToGnoss)
                            {
                                GestorAddToGnoss gestorAddtoGnoss = new GestorAddToGnoss();
                                gestorAddtoGnoss.EsBaseRecursosPersonal = false;
                                gestorAddtoGnoss.EsBaseRecursosOrganizacion = false;
                                gestorAddtoGnoss.IdentidadEnProyecto = identidad.Clave;
                                gestorAddtoGnoss.ProyectoBRID = identidad.FilaIdentidad.ProyectoID;
                                gestorAddtoGnoss.OrganizacionBRID = identidad.FilaIdentidad.OrganizacionID;
                                gestorAddtoGnoss.PerfilEnProyecto = identidad.PerfilID;
                                gestorAddtoGnoss.EsBaseRecursosPublica = filaProy.TipoAcceso == (short)TipoAcceso.Publico || filaProy.TipoAcceso == (short)TipoAcceso.Restringido;
                                gestorAddtoGnoss.NombreProyecto = filaProy.Nombre;

                                string licenciaPorDefecto = listaProyIDLiceciaDefecto[filaProy.ProyectoID];

                                if (!string.IsNullOrEmpty(licenciaPorDefecto))
                                {
                                    gestorAddtoGnoss.LicenciaProyectoPorDefecto = licenciaPorDefecto;
                                }

                                gestorAddtoGnoss.TieneTwitter = listaProyectosConTwitter.Contains(gestorAddtoGnoss.ProyectoBRID);

                                if (!mGestoresAddToGnoss.ContainsKey(baseRecursosID))
                                {
                                    mGestoresAddToGnoss.Add(baseRecursosID, gestorAddtoGnoss);
                                }
                            }
                            else
                            {
                                string nombreProy = filaProy.Nombre;
                                if (!addToGnossShareSites.ContainsKey(baseRecursosID))
                                {
                                    addToGnossShareSites.Add(baseRecursosID, nombreProy + " - " + listaIdentidadNombrePerfil[identidad]);
                                }
                            }
                        }
                    }
                }
            }

            //Agrego las BRs de organizaciones
            foreach (Identidad identidad in listaIdentidadProyPERFILORGANIZACION.Values)
            {
                if (!new OrganizacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCL>(), mLoggerFactory).ComprobarOrganizacionEsClase((Guid)identidad.OrganizacionID))
                {
                    Guid baseRecursosID = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory).ObtenerBaseRecursosIDOrganizacion((Guid)identidad.OrganizacionID);

                    if (pCargarGestoresAddToGnoss)
                    {
                        GestorAddToGnoss gestorAddtoGnoss = new GestorAddToGnoss();
                        gestorAddtoGnoss.EsBaseRecursosPersonal = false;
                        gestorAddtoGnoss.EsBaseRecursosOrganizacion = true;
                        gestorAddtoGnoss.IdentidadEnProyecto = identidad.Clave;
                        gestorAddtoGnoss.ProyectoBRID = ProyectoAD.MetaProyecto;
                        gestorAddtoGnoss.OrganizacionBRID = (Guid)identidad.OrganizacionID;
                        gestorAddtoGnoss.PerfilEnProyecto = identidad.PerfilID;
                        mGestoresAddToGnoss.Add(baseRecursosID, gestorAddtoGnoss);
                    }
                    else
                    {
                        string nombreOrganizacion = identidad.PerfilUsuario.NombreOrganizacion;
                        addToGnossShareSites.Add(baseRecursosID, UtilIdiomas.GetText("ANYADIRGNOSS", "BRORGANIZACION") + nombreOrganizacion);
                    }
                }
            }

            return addToGnossShareSites;
        }

        /// <summary>
        /// Carga las Base de recursos del usuario en la Comunidad.
        /// </summary>
        /// <param name="pCargarGestoresAddToGnoss">Indica si se deben cargar los gestores Add To Gnoss</param>
        private void CargarBaseRecursosUsuario_AnyadirComunidad()
        {
            if (mGestoresAddToGnoss == null)
            {
                mGestoresAddToGnoss = new Dictionary<Guid, GestorAddToGnoss>();
            }

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            GestionProyecto gestorProyectos = new GestionProyecto(proyectoCN.ObtenerProyectoPorID(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
            proyectoCN.Dispose();
            gestorProyectos.CargarGestor();
            Proyecto proyecto = gestorProyectos.ListaProyectos[ProyectoSeleccionado.Clave];

            DocumentacionCN documCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            Guid baseRecursosID = documCN.ObtenerBaseRecursosIDProyecto(ProyectoSeleccionado.Clave);
            documCN.Dispose();

            GestorAddToGnoss gestorAddtoGnoss = new GestorAddToGnoss();
            gestorAddtoGnoss.EsBaseRecursosPersonal = false;
            gestorAddtoGnoss.EsBaseRecursosOrganizacion = false;
            gestorAddtoGnoss.IdentidadEnProyecto = IdentidadActual.Clave;
            gestorAddtoGnoss.ProyectoBRID = IdentidadActual.FilaIdentidad.ProyectoID;
            gestorAddtoGnoss.OrganizacionBRID = IdentidadActual.FilaIdentidad.OrganizacionID;
            gestorAddtoGnoss.PerfilEnProyecto = IdentidadActual.PerfilID;
            gestorAddtoGnoss.EsBaseRecursosPublica = proyecto.TipoAcceso.Equals((short)TipoAcceso.Publico) || proyecto.TipoAcceso.Equals((short)TipoAcceso.Restringido);
            gestorAddtoGnoss.NombreProyecto = proyecto.Nombre;

            ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCN>(), mLoggerFactory);
            List<Guid> listaProyLic = new List<Guid>();
            listaProyLic.Add(ProyectoSeleccionado.Clave);
            Dictionary<Guid, string> listaProyIDLiceciaDefecto = paramCN.ObtenerLicenciasporProyectosID(listaProyLic);
            paramCN.Dispose();

            string licenciaPorDefecto = listaProyIDLiceciaDefecto[ProyectoSeleccionado.Clave];

            if (!string.IsNullOrEmpty(licenciaPorDefecto))
            {
                gestorAddtoGnoss.LicenciaProyectoPorDefecto = licenciaPorDefecto;
            }

            gestorAddtoGnoss.TieneTwitter = proyecto.FilaProyecto.TieneTwitter;

            if (!mGestoresAddToGnoss.ContainsKey(baseRecursosID))
            {
                mGestoresAddToGnoss.Add(baseRecursosID, gestorAddtoGnoss);

                if (mEditRecCont.ModifyResourceModel.AddToGnossShareSites == null)
                {
                    mEditRecCont.ModifyResourceModel.AddToGnossShareSites = new Dictionary<Guid, string>();
                }

                mEditRecCont.ModifyResourceModel.AddToGnossShareSites.Add(baseRecursosID, proyecto.Nombre + " - " + IdentidadActual.PerfilUsuario.NombrePerfil);
            }
        }

        /// <summary>
        /// Prepara el tesauro para añadir a Gnoss.
        /// </summary>
        /// <param name="pBRActualID">ID de la base de recursos actual</param>
        private ThesaurusEditorModel PrepararTesauro_AnyadirGnoss(Guid pBRActualID)
        {
            TesauroCN tesCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
            GestionTesauro gestorTes = new GestionTesauro(tesCN.ObtenerTesauroPorBaseRecursosID(pBRActualID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            tesCN.Dispose();

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Guid proyectoIDBR = proyCN.ObtenerProyectoIDPorBaseRecursos(pBRActualID);
            proyCN.Dispose();

            if (proyectoIDBR != Guid.Empty)
            {
                TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);
                gestorTes.TesauroDW.Merge(tesauroCL.ObtenerCategoriasPermitidasPorTipoRecurso(gestorTes.TesauroActualID, proyectoIDBR));
                tesauroCL.Dispose();

                gestorTes.EliminarCategoriasNoPermitidasPorTipoDoc((short)mTipoDocumento, null);
            }

            ParametroAplicacion filaParametroAplicacion = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("UsarSoloCategoriasPrivadasEnEspacioPersonal"));
            if (filaParametroAplicacion != null && (filaParametroAplicacion.Valor.Equals("1") || filaParametroAplicacion.Valor.Equals("true")))
            {
                gestorTes.EliminarCategoriasPublicasSiEsMetaEspacioGNOSS(UtilIdiomas.LanguageCode);
            }

            ThesaurusEditorModel thesaurusEditorModel = new ThesaurusEditorModel();
            thesaurusEditorModel.ThesaurusCategories = CargarTesauroPorGestorTesauro(gestorTes);
            thesaurusEditorModel.SelectedCategories = new List<Guid>();

            ComprobarCategoriasDeshabilitadas(gestorTes, thesaurusEditorModel);

            gestorTes.Dispose();

            return thesaurusEditorModel;
        }

        /// <summary>
        /// Comprueba si hay que redireccionar y si es así devuelve la redirección.
        /// </summary>
        /// <param name="pEsAccion">Indica si se está ejecutando una acción</param>
        /// <returns>Redirección resultante</returns>
        private ActionResult ComprobarRedirecciones_AnyadirGnoss(bool pEsAccion)
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return Redirect(BaseURLIdioma);
            }

            if ((IdentidadActual.IdentidadPersonalMyGNOSS.Clave != IdentidadActual.Clave && mEditRecCont.TypePage != Es.Riam.Gnoss.Web.MVC.Models.EditResourceModel.TypePageEditResource.AddToCommunityResource) || Request.QueryString.ToString().Contains("\n") || Request.QueryString.ToString().Contains("\t"))
            {
                string titulo = TituloAddTo;
                string descripcion = DescripcionAddTo;
                string tags = TagsAddTo;
                string version = null;

                if (RequestParams("verAddTo") != null)
                {
                    version = RequestParams("verAddTo");
                }

                if (RequestParams("archivolocal") == null)
                {
                    string urlAddToGnoss = $"{UtilIdiomas.GetText("URLSEM", "ANYADIRAGNOSS")}?ID={IdentidadActual.IdentidadPersonalMyGNOSS.Clave}&titl={titulo}&addToGnoss={RequestParams("addToGnoss")}&descp={descripcion}&tags={tags}";

                    if (version != null)
                    {
                        urlAddToGnoss += "&verAddTo=" + version;
                    }
                    return Redirect(urlAddToGnoss);
                }
                else
                {
                    string parametroMetagnoss = "";

                    if (RequestParams("metagnoss") != null)
                    {
                        parametroMetagnoss = "&metagnoss=" + RequestParams("metagnoss");
                    }



                    return Redirect(UtilIdiomas.GetText("URLSEM", "ANYADIRAGNOSS") + "?ID=" + IdentidadActual.IdentidadPersonalMyGNOSS.Clave.ToString() + "&titl=" + titulo + "&addToGnoss=" + RequestParams("addToGnoss") + "&verAddTo=" + AddToGnoss.VERSION_ACTUAL + "&descp=" + descripcion + "&tags=" + tags + "&archivolocal=true&idtemporal=" + RequestParams("idtemporal") + parametroMetagnoss);
                }
            }

            if ((RequestParams("metagnoss") != null) && (RequestParams("metagnoss").ToLower().Equals("true")))
            {
                mEnvioMetaGnossAddToGnoss = true;

                UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                bool estaEnMetagnoss = usuCN.EstaUsuarioEnProyecto(mControladorBase.UsuarioActual.UsuarioID, ProyectoAD.MetaGNOSS);
                usuCN.Dispose();

                if (!estaEnMetagnoss)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    string nombreCortoMetagnoss = proyCN.ObtenerNombreCortoProyecto(ProyectoAD.MetaGNOSS);
                    proyCN.Dispose();

                    string url = Request.Path.ToString().ToLower();

                    if (url.Contains(BaseURLSinHTTP.ToLower()))
                    {
                        url = url.Remove(0, BaseURLSinHTTP.Length + 1);//con el +1 quito tambien la primera '/'
                    }
                    Session.Set("redirectAddToGnoss", url);
                    return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, nombreCortoMetagnoss));
                }
            }

            if (!pEsAccion && RequestParams("addToGnoss") == null && mCookieAnyadirGnoss == null)
            {
                return Redirect(BaseURLIdioma);
            }

            return null;
        }

        /// <summary>
        /// Hace expirar la cookie de añadir a gnoss
        /// </summary>
        private void ExpirarCookie()
        {
            if (mCookieAnyadirGnoss != null)
            {
                Response.Cookies.Append("anyadirGnoss", null, new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            }
        }

        #endregion

        #region Comunes

        /// <summary>
        /// Extrae un parámetro de la petición actual o Devuelve Vacío si no está.
        /// </summary>
        /// <param name="pParametro">Nombre de parámetro</param>
        /// <returns>Parámetro de la petición actual o Devuelve Vacío si no está</returns>
        private static string ExtraerTexto(string pParametro)
        {
            if (pParametro != null)
            {
                return pParametro;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Valida si los datos de documento son correctos o no.
        /// </summary>
        /// <param name="pBorrador">Indica si el documento es un borrador o no</param>
        /// <param name="pCategoriasSelec">Lista con las categorías seleccionadas</param>
        /// <returns>NULL si los datos son correctos, Texto con el error en caso contario.</returns>
        private string ValidarRecurso(bool pBorrador, List<Guid> pCategoriasSelec)
        {
            if (Documento != null)
            {
                mTipoDocumento = Documento.TipoDocumentacion;
            }

            string titulo = UtilCadenas.EliminarHtmlDeTexto(ExtraerTexto(mModelSaveRec.Title).Trim());
            string descripcion = HttpUtility.UrlDecode(ExtraerTexto(mModelSaveRec.Description));
            string tags = UtilCadenas.EliminarHtmlDeTexto(ExtraerTexto(mModelSaveRec.Tags).Trim());

            string regexpresion = "^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\\:\\/\\/)?([\\w\\.\\-]+(\\:[\\w\\.\\&%\\$\\-]+)*@)?((([^\\s\\(\\)\\<\\>\\\\\\\"\\.\\[\\]\\,@;:]+)(\\.[^\\s\\(\\)\\<\\>\\\\\\\"\\.\\[\\]\\,@;:]+)*(\\.[a-zA-Z]*))|((([01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}([01]?\\d{1,2}|2[0-4]\\d|25[0-5])))(\\b\\:(6553[0-5]|655[0-2]\\d|65[0-4]\\d{2}|6[0-4]\\d{3}|[1-5]\\d{4}|[1-9]\\d{0,3}|0)\\b)?((\\/[^\\/][\\w\\.\\,\\?\\'\\\\\\/\\+&%\\$#\\=~_\\-@:]*)*[^\\.\\,\\?\\\"\\'\\(\\)\\[\\]!;<>{}\\s\\x7F-\\xFF])?)$";
            Regex reg = new Regex(regexpresion);

            if (Documento != null && Documento.TipoDocumentacion != TiposDocumentacion.Nota && Documento.TipoDocumentacion != TiposDocumentacion.Pregunta && Documento.TipoDocumentacion != TiposDocumentacion.Encuesta && Documento.TipoDocumentacion != TiposDocumentacion.Debate && Documento.TipoDocumentacion != TiposDocumentacion.Newsletter && Documento.TipoDocumentacion != TiposDocumentacion.Wiki && Documento.TipoDocumentacion != TiposDocumentacion.AudioBrightcove && Documento.TipoDocumentacion != TiposDocumentacion.VideoBrightcove && Documento.TipoDocumentacion != TiposDocumentacion.VideoTOP && Documento.TipoDocumentacion != TiposDocumentacion.AudioTOP && Documento.Enlace.Equals(string.Empty) && (string.IsNullOrEmpty(mModelSaveRec.TemporalFileName) || string.IsNullOrEmpty(NombreArchivoTemporal)) && (string.IsNullOrEmpty(mModelSaveRec.Link)) && (Session.Get("nombreFuReemplazado") == null))
            {
                return "errorEnlace|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DENLACE");
            }
            else if (Documento == null && mTipoDocumento != TiposDocumentacion.Nota && mTipoDocumento != TiposDocumentacion.Pregunta && mTipoDocumento != TiposDocumentacion.Encuesta && mTipoDocumento != TiposDocumentacion.Debate && mTipoDocumento != TiposDocumentacion.Newsletter && mTipoDocumento != TiposDocumentacion.Wiki && mTipoDocumento != TiposDocumentacion.AudioBrightcove && mTipoDocumento != TiposDocumentacion.VideoBrightcove && mTipoDocumento != TiposDocumentacion.VideoTOP && mTipoDocumento != TiposDocumentacion.AudioTOP && mTipoDocumento != TiposDocumentacion.Semantico && string.IsNullOrEmpty(mModelSaveRec.Link))
            {
                return "errorEnlace|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DENLACE");
            }
            else if (mTipoDocumento == TiposDocumentacion.Hipervinculo && (ExtraerTexto(mModelSaveRec.Link).Trim().Equals(string.Empty) || !reg.IsMatch(ExtraerTexto(mModelSaveRec.Link).Trim())))
            {
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (!UtilCadenas.EsEnlaceSharepoint(mModelSaveRec.Link, oneDrivePermitido))
                {
                    return "errorEnlace|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DWEB");
                }
            }
            else if ((mTipoDocumento == TiposDocumentacion.ReferenciaADoc) && (ExtraerTexto(mModelSaveRec.Link).Trim().Equals(string.Empty)))
            {
                return "errorEnlace|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DUBICACION");
            }
            if (string.IsNullOrEmpty(titulo))
            {
                return "errorTitulo|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DTITULO");
            }

            if (mTipoDocumento != TiposDocumentacion.Semantico || mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key == null)
            {
                if (mModelSaveRec.Description != null)
                {
                    if (!pBorrador && mTipoDocumento != TiposDocumentacion.Encuesta && mTipoDocumento != TiposDocumentacion.Newsletter && string.IsNullOrEmpty(descripcion))
                    {
                        return "errorDescription|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DDESCRIPCION");
                    }
                }
                else
                {
                    if (!pBorrador && mTipoDocumento != TiposDocumentacion.Encuesta && (Documento == null || Documento.Descripcion.Equals(string.Empty)) && (Documento != null || mTipoDocumento != TiposDocumentacion.Newsletter || mModelSaveRec.NewsletterManual))
                    {
                        return "errorDescription|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DDESCRIPCION");
                    }
                }
            }

            if ((!pBorrador && !CargaMasivaFormSem && tags.Equals(string.Empty) && mTipoDocumento != TiposDocumentacion.Semantico) || (mTipoDocumento == TiposDocumentacion.Semantico && !mOntologia.ConfiguracionPlantilla.EtiquetacionGnossNoObligatoria && tags.Equals(string.Empty)))
            {
                return "errorTags|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DTAG");
            }

            if (mTipoDocumento == TiposDocumentacion.Encuesta)
            {
                int numRespuestas = 0;
                foreach (string respuesta in ExtraerTexto(mModelSaveRec.PollResponses).Split(new string[] { "[[&]]" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (respuesta != "")
                    {
                        numRespuestas++;
                    }
                }

                if (!pBorrador && numRespuestas < 2)
                {
                    return "errorRespuestas|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DOSRESPUESTAS");
                }
            }

            if (mTipoDocumento == TiposDocumentacion.Newsletter && !pBorrador && string.IsNullOrEmpty(descripcion) && mModelSaveRec.NewsletterManual)
            {
                return "errorDescription|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DDESCRIPCION");
            }

            List<CategoriaTesauro> categoriasNoMarcadas = GestorDocumental.GestorTesauro.ComprobarCategoriasObligatoriasMarcadas(pCategoriasSelec);

            if (!pBorrador && (mTipoDocumento != TiposDocumentacion.Semantico || !mOntologia.ConfiguracionPlantilla.CategorizacionTesauroGnossNoObligatoria) && (pCategoriasSelec.Count == 0 || categoriasNoMarcadas.Count > 0))
            {
                string error = null;

                switch (mTipoDocumento)
                {
                    case TiposDocumentacion.Pregunta:
                        error = "errorCategories|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA_PREGUNTA");
                        break;
                    case TiposDocumentacion.Debate:
                        error = "errorCategories|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA_DEBATE");
                        break;
                    case TiposDocumentacion.Encuesta:
                        error = "errorCategories|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA_ENCUESTA");
                        break;
                    default:
                        if (categoriasNoMarcadas.Count > 0)
                        {
                            error = "errorCategories|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA_OBLIGATORIA", categoriasNoMarcadas[0].Nombre[IdiomaUsuario]);
                        }
                        else
                        {
                            error = "errorCategories|" + UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA");
                        }
                        break;
                }

                return error;
            }

            return null;
        }

        /// <summary>
        /// Recoge las categorías seleccionadas.
        /// </summary>
        private List<Guid> RecogerCategoriasSeleccionadas()
        {
            List<Guid> listaCategorias = new List<Guid>();

            string txtHackDesplegableSelCat = mModelSaveRec.SelectedCategories;
            if (string.IsNullOrEmpty(txtHackDesplegableSelCat))
            {
                if (GestorDocumental.GestorTesauro.TesauroDW.ListaTesauroUsuario.Count > 0)
                {
                    AD.EntityModel.Models.Tesauro.TesauroUsuario fila = GestorDocumental.GestorTesauro.TesauroDW.ListaTesauroUsuario.FirstOrDefault();
                    listaCategorias.Add(fila.CategoriaTesauroPrivadoID.Value);
                }
            }
            else
            {
                char[] separador = { ',' };
                foreach (string categoriaID in txtHackDesplegableSelCat.Split(separador, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (GestorDocumental.GestorTesauro == null)
                    {
                        TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);
                        GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(mControladorBase.UsuarioActual.ProyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                        tesauroCL.Dispose();
                    }
                    if (GestorDocumental.GestorTesauro.ListaCategoriasTesauro.ContainsKey(new Guid(categoriaID)))
                    {
                        listaCategorias.Add(new Guid(categoriaID));
                    }
                }
            }

            if (mTipoDocumento == TiposDocumentacion.Semantico)
            {
                if (EditandoFormSem && Documento.FilaDocumentoWebVinBR.TipoPublicacion == (short)TipoPublicacion.CompartidoAutomatico)
                {
                    if (GestorDocumental.GestorTesauro == null)
                    {
                        TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);
                        GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(mControladorBase.UsuarioActual.ProyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                        tesauroCL.Dispose();

                        Documento.RecargarCategoriasTesauro();
                    }

                    //Hay que dejar las categorías intactas:
                    foreach (Guid categoriaID in Documento.Categorias.Keys)
                    {
                        listaCategorias.Add(categoriaID);
                    }
                }
                else if (mOntologia.ConfiguracionPlantilla.CategoriasPorDefecto != null)
                {
                    try
                    {
                        foreach (string categoria in mOntologia.ConfiguracionPlantilla.CategoriasPorDefecto)
                        {
                            Guid categoriaID;
                            Guid.TryParse(categoria, out categoriaID);

                            if (categoriaID == Guid.Empty)
                            {
                                categoriaID = GestorDocumental.GestorTesauro.ObtenerCategoriasIDPorNombre(categoria)[0];
                            }

                            if (!listaCategorias.Contains(categoriaID))
                            {
                                listaCategorias.Add(categoriaID);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GuardarMensajeErrorAdmin("La sección 'CategoriasPorDefecto' tiene alguna categoría mal configurada.", ex);
                        throw new ExcepcionWeb("La sección 'CategoriasPorDefecto' tiene alguna categoría mal configurada:" + Environment.NewLine + ex.ToString());
                    }
                }
            }

            return listaCategorias;
        }

        /// <summary>
        /// Carga los editores del recurso actual.
        /// </summary>
        /// <param name="pSelectorEditores">Modelo de selector de editores del recurso</param>
        /// <param name="pSelectorLectores">Modelo de selector de lectores del recurso</param>
        private void CargarEditoresRecurso(UsersSelectorModel pSelectorEditores, UsersSelectorModel pSelectorLectores)
        {
            if (Documento != null && Documento.TipoDocumentacion == TiposDocumentacion.Debate)
            {
                pSelectorEditores.TextInfoSelectUsers = UtilIdiomas.GetText("PERFILBASERECURSOEDITAR", "INFOEDITORESDEBATES");
                pSelectorLectores.TextInfoSelectUsers = UtilIdiomas.GetText("PERFILBASERECURSOEDITAR", "INFOEDITORESDEBATES");
            }
            else
            {
                pSelectorEditores.TextInfoSelectUsers = UtilIdiomas.GetText("PERFILBASERECURSOEDITAR", "INFOEDITORES");
                pSelectorLectores.TextInfoSelectUsers = UtilIdiomas.GetText("PERFILBASERECURSOEDITAR", "INFOLECTORES");
            }

            pSelectorEditores.TextSelectUsers = UtilIdiomas.GetText("USUARIOS", "Editores") + ": ";
            pSelectorLectores.TextSelectUsers = UtilIdiomas.GetText("PERFILBASERECURSOEDITAR", "LECTORES") + ": ";

            if (!Duplicando)
            {
                // No es necesario cargar los editores / lectores en la comunidad destino si se está duplicando el recurso.
                if (Documento != null)
                {
                    foreach (Editor.EditorRecurso editor in Documento.ListaPerfilesEditores.Values)
                    {
                        Identidad identidadEditor = editor.IdentidadEnProyectoActual(ProyectoSeleccionado.Clave);

                        if (identidadEditor == null)
                        {
                            identidadEditor = editor.IdentidadEnCualquierProyecto;
                        }

                        string nombreEditor = ObtenerNombreIdentidad(identidadEditor);

                        if (!string.IsNullOrEmpty(nombreEditor))
                        {
                            if (editor.FilaEditor.Editor)
                            {
                                pSelectorEditores.SelectedProfilesList.Add(editor.Clave, nombreEditor);
                            }
                            else
                            {
                                pSelectorLectores.SelectedProfilesList.Add(editor.Clave, nombreEditor);
                            }
                        }
                    }

                    using (IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory))
                    {
                        pSelectorEditores.DocumentCreatorProfileId = identidadCN.ObtenerPerfilIDDeIdentidadID(Documento.CreadorID);
                        pSelectorLectores.DocumentCreatorProfileId = identidadCN.ObtenerPerfilIDDeIdentidadID(Documento.CreadorID);
                    }

                    Dictionary<Guid, string> orgIDNombreOrg = new Dictionary<Guid, string>();

                    foreach (GrupoEditorRecurso grupoEditor in Documento.ListaGruposEditores.Values)
                    {
                        string nombreGrupo = grupoEditor.Nombre;

                        if (grupoEditor.FilaGrupoIdentidadOrganizacion != null)
                        {
                            if (!orgIDNombreOrg.ContainsKey(grupoEditor.FilaGrupoIdentidadOrganizacion.OrganizacionID))
                            {
                                OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                                AD.EntityModel.Models.OrganizacionDS.Organizacion organizacion = orgCN.ObtenerNombreOrganizacionPorID(grupoEditor.FilaGrupoIdentidadOrganizacion.OrganizacionID);
                                string nombreOrgGrupo = organizacion.Nombre;
                                orgCN.Dispose();

                                orgIDNombreOrg.Add(grupoEditor.FilaGrupoIdentidadOrganizacion.OrganizacionID, nombreOrgGrupo);
                            }

                            nombreGrupo += " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + orgIDNombreOrg[grupoEditor.FilaGrupoIdentidadOrganizacion.OrganizacionID];
                        }

                        if (grupoEditor.FilaGrupoEditor.Editor)
                        {
                            pSelectorEditores.SelectedGroupsList.Add(grupoEditor.Clave, nombreGrupo);
                        }
                        else
                        {
                            pSelectorLectores.SelectedGroupsList.Add(grupoEditor.Clave, nombreGrupo);
                        }
                    }
                }
                else if (mEditandoRecursoCargaMasiva)
                {
                    CargarEditoresRecursoTemp_CargaMasiva(pSelectorEditores, pSelectorLectores);
                }
            }

            mEditRecCont.ModifyResourceModel.CommunityReadersAvailable = true;
            bool puedeSeleccionarAbierto = ComprobarIdentidadPuedeSeleccionarVisibilidadAbierto(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);
            mEditRecCont.ModifyResourceModel.OpenResourcesAvailable = ProyectoSeleccionado.EsPublico && puedeSeleccionarAbierto;

            if (ParametrosGeneralesRow.PermitirRecursosPrivados)
            {
                if (EsComunidad)
                {
                    mEditRecCont.ModifyResourceModel.PrivateResourcesAvailable = true;
                }
                else
                {
                    mEditRecCont.ModifyResourceModel.PrivateResourcesAvailable = false;
                    mEditRecCont.ModifyResourceModel.CommunityReadersAvailable = false;
                    mEditRecCont.ModifyResourceModel.OpenResourcesAvailable = false;
                }
            }
            else
            {
                mEditRecCont.ModifyResourceModel.PrivateResourcesAvailable = false;
            }

            if (!mEditandoRecursoCargaMasiva)
            {
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.PrivateEditors = (Documento != null && Documento.FilaDocumentoWebVinBR != null && Documento.FilaDocumentoWebVinBR.PrivadoEditores);
                mEditRecCont.ModifyResourceModel.VisibilityMembersCommunity = ((Documento != null && ((VisibilidadDocumento)Documento.FilaDocumento.Visibilidad).Equals(VisibilidadDocumento.PrivadoMiembrosComunidad)) || !ProyectoSeleccionado.EsPublico || !puedeSeleccionarAbierto);
            }

            mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable = (!mTipoDocumento.Equals(TiposDocumentacion.Wiki) && !mTipoDocumento.Equals(TiposDocumentacion.Pregunta) && !mTipoDocumento.Equals(TiposDocumentacion.Newsletter) && !mTipoDocumento.Equals(TiposDocumentacion.Encuesta));
            mEditRecCont.ModifyResourceModel.SelectorEditionAvailable = true;

            if (mTipoDocumento.Equals(TiposDocumentacion.Debate))
            {
                mEditRecCont.ModifyResourceModel.SelectorEditionAvailable = false;
                pSelectorLectores.TextSelectUsers = UtilIdiomas.GetText("PERFILBASERECURSOEDITAR", "PARTICIPANTES");
            }

            bool creandoVersion = false;

            if (RequestParams("version") != null)
            {
                bool.TryParse(RequestParams("version").ToString(), out creandoVersion);
            }

            if (Documento != null && Documento.ListaPerfilesEditores.Count > 1 && !creandoVersion && !Documento.FilaDocumento.Protegido)
            {
                mEditRecCont.ModifyResourceModel.MultipleEditors = true;
            }
        }

        /// <summary>
        /// Obtiene el nombre de un identidad
        /// </summary>
        /// <param name="pIdentidad"></param>
        /// <returns></returns>
        private string ObtenerNombreIdentidad(Identidad pIdentidad)
        {
            if (pIdentidad != null)
            {
                if (!pIdentidad.ModoPersonal)
                {
                    // David: Si la identidad actual es miembro de la organización, se muestar el nombre de la persona
                    if (pIdentidad.OrganizacionID.HasValue && IdentidadActual.OrganizacionID.HasValue && pIdentidad.OrganizacionID.Equals(IdentidadActual.OrganizacionID))
                    {
                        return pIdentidad.NombreOrganizacion + " (" + pIdentidad.PerfilUsuario.NombrePersonaEnOrganizacion + ")";
                    }
                    else
                    {
                        return pIdentidad.Nombre(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave));
                    }
                }
                else
                {
                    return pIdentidad.Nombre(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave));
                }
            }

            return null;
        }

        /// <summary>
        /// Comprueba la repetición del enlace de un recurso.
        /// </summary>
        /// <param name="pEnlace">Enlace</param>
        /// <param name="pTipoDoc">Tipo de documento</param>
        /// <param name="pExtraArchivo">Exta para el archivo</param>
        /// <returns>Acción respuesta</returns>
        private ActionResult ComprobarRepeticionEnlaceRecurso(string pTitulo, string pEnlace, TiposDocumentacion? pTipoDoc, string pExtraArchivo)
        {
            if (pTipoDoc.HasValue)
            {
                if (pTitulo == null && pEnlace == null) //Es aviso de repetición
                {
                    UploadResourceReplayPanelModel subRecPanRepModel = new UploadResourceReplayPanelModel();
                    subRecPanRepModel.CanRepeatResource = true;
                    subRecPanRepModel.ExtraFile = pExtraArchivo;

                    if (pTipoDoc == TiposDocumentacion.Debate)
                    {
                        subRecPanRepModel.RepeatedResourceType = 5;
                    }
                    else
                    {
                        subRecPanRepModel.RepeatedResourceType = 4;
                    }

                    return PartialView("_SubirRecurso_PanelRepeticion", subRecPanRepModel);
                }
            }

            string enlaceRepeticion = pEnlace;

            if (pTipoDoc == TiposDocumentacion.Hipervinculo && enlaceRepeticion.IndexOf("http://") != 0)
            {
                enlaceRepeticion = "http://" + enlaceRepeticion;
            }

            Guid documentoRepetidoID;
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            int codigo = 0;

            if (!AñadiendoAGnoss)
            {
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (UtilCadenas.EsEnlaceSharepoint(enlaceRepeticion, oneDrivePermitido))
                {
                    DataWrapperDocumentacion dwDocumentacion = new DataWrapperDocumentacion();
                    docCN.ObtenerBaseRecursosProyecto(dwDocumentacion, ProyectoSeleccionado.FilaProyecto.ProyectoID);

                    if (!EditandoRecurso)
                    {
                        CargaInicial();
                    }

                }
                codigo = docCN.DocumentoRepetidoTituloEnlace(pTitulo, enlaceRepeticion, Guid.Empty, GestorDocumental.BaseRecursosIDActual, out documentoRepetidoID);
            }
            else
            {
                if (mBRsCatsAddToGnoss == null)
                {
                    mEditRecCont.ModifyResourceModel.AddToGnossShareSites = CargarBaseRecursosUsuario_AnyadirGnoss(true);
                    mBRsCatsAddToGnoss = RecogerBRsYCategoriasSeleccionadas_AnyadirGnoss();
                }

                codigo = docCN.DocumentoRepetidoTituloEnlaceEnVariasBRs(pTitulo, enlaceRepeticion, Guid.Empty, new List<Guid>(mBRsCatsAddToGnoss.Keys), out documentoRepetidoID);
            }

            docCN.Dispose();

            if (codigo != 0)
            {
                UploadResourceReplayPanelModel subRecPanRepModel = new UploadResourceReplayPanelModel();
                subRecPanRepModel.CanRepeatResource = (codigo == 1);
                subRecPanRepModel.ExtraFile = pExtraArchivo;
                subRecPanRepModel.AddToGnossPage = AñadiendoAGnoss;

                #region Panel Repetición

                if (codigo > 1)
                {
                    docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    subRecPanRepModel.RepeatedResourceName = docCN.ObtenerTituloDocumentoPorID(documentoRepetidoID);

                    //El error es de url repetida, no puede crear el recurso
                    subRecPanRepModel.RepeatedResourceUrl = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Es.Riam.Gnoss.Elementos.Documentacion.Documento.ObtenerNombreSemantico(subRecPanRepModel.RepeatedResourceName, UtilIdiomas.LanguageCode), documentoRepetidoID, null, (IdentidadOrganizacion != null));
                }
                else
                {
                    subRecPanRepModel.RepetitionLink = enlaceRepeticion;

                    if (pTipoDoc.HasValue)
                    {
                        if (pTipoDoc == TiposDocumentacion.ReferenciaADoc)
                        {
                            subRecPanRepModel.RepeatedResourceType = 0;
                        }
                        else if (pTipoDoc == TiposDocumentacion.Hipervinculo)
                        {
                            subRecPanRepModel.RepeatedResourceType = 1;
                        }
                        else if (pTipoDoc == TiposDocumentacion.FicheroServidor)
                        {
                            subRecPanRepModel.RepeatedResourceType = 2;
                        }
                    }
                    else //Aviso de recurso genérico:
                    {
                        subRecPanRepModel.RepeatedResourceType = 3;
                    }
                }

                return PartialView("_SubirRecurso_PanelRepeticion", subRecPanRepModel);

                #endregion
            }

            return null;
        }

        /// <summary>
        /// Devuelve el nombre del documento sin la extensión.
        /// </summary>
        /// <param name="pNombreFichero">Nombre fichero</param>
        /// <returns>String con nombre fichero</returns>
        private static string ExtraerNombreFicheroSinExtension(string pNombreFichero)
        {
            return pNombreFichero.Substring(0, pNombreFichero.LastIndexOf('.'));
        }

        /// <summary>
        /// Guarda los datos de los editores y actualiza la parte del Live correspondiente a ello.
        /// </summary>
        /// <param name="pDocumento">Documento para guardar editores</param>
        private void GuardarDatosEditores(Documento pDocumento, out bool pPrivacidadCambiada, out List<Guid> pListaEditoresEliminados, out List<Guid> pListaGruposEditoresEliminados)
        {
            pPrivacidadCambiada = false;
            pListaEditoresEliminados = new List<Guid>();
            pListaGruposEditoresEliminados = new List<Guid>();

            List<Guid> listaIdsEditores = new List<Guid>();
            List<Guid> listaIdsGruposEditores = new List<Guid>();

            bool permisoEdicionEstado = pDocumento.FilaDocumento.EstadoID.HasValue && ComprobarPermisoEdicionEstado(pDocumento.FilaDocumento.EstadoID.Value, pDocumento.VersionOriginalID);

			if (((!EditandoRecurso && !EditandoFormSem) || pDocumento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacion, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsAdministrador(IdentidadActual)) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, pDocumento, true, UsuarioActual.UsuarioID) || ComprobarPermisoGeneralEditarRecurso(pDocumento.TipoDocumentacion) || permisoEdicionEstado) && (mOntologia == null || !EditandoFormSem || (mEditRecCont.ModifyResourceModel.SetPermissionsEditionAvailable)))
            {
                List<Guid> listaEditoresOriginal = new List<Guid>(pDocumento.ListaPerfilesEditores.Keys);
                List<Guid> listaGruposEditoresOriginal = null;

                if (Documento != null)
                {
                    listaGruposEditoresOriginal = new List<Guid>(Documento.ListaGruposEditores.Keys);
                }
                else
                {
                    listaGruposEditoresOriginal = new List<Guid>();
                }

                pDocumento.ListaPerfilesEditores.Clear();
                pDocumento.ListaPerfilesEditoresSinLectores.Clear();
                pDocumento.ListaGruposEditores.Clear();
                pDocumento.ListaGruposEditoresSinLectores.Clear();
                GestorDocumental.LimpiarEditores(pDocumento.Clave);
                GestorDocumental.LimpiarGruposEditores(pDocumento.Clave);

                bool privadoEditores = true;
                bool privadoMiembroscomunidadCambiado = false;
                if (mModelSaveRec.ResourceVisibility == ResourceVisibility.CommunityMembers || mModelSaveRec.ResourceVisibility == ResourceVisibility.Open)
                {
                    privadoEditores = false;
                    if (
                        (mModelSaveRec.ResourceVisibility == ResourceVisibility.CommunityMembers && Documento != null && Documento.FilaDocumento.Visibilidad != 2) ||
                        (mModelSaveRec.ResourceVisibility != ResourceVisibility.CommunityMembers && Documento != null && Documento.FilaDocumento.Visibilidad == 2))
                    {
                        privadoMiembroscomunidadCambiado = true;
                    }

                    if (ProyectoSeleccionado.EsPublico && mModelSaveRec.ResourceVisibility == ResourceVisibility.CommunityMembers)
                    {
                        pDocumento.FilaDocumento.Visibilidad = (short)VisibilidadDocumento.PrivadoMiembrosComunidad;
                    }
                    else if (mModelSaveRec.ResourceVisibility == ResourceVisibility.Open || (!ProyectoSeleccionado.EsPublico && mModelSaveRec.ResourceVisibility == ResourceVisibility.CommunityMembers))
                    {
                        pDocumento.FilaDocumento.Visibilidad = (short)VisibilidadDocumento.Todos;
                    }
                }
                pPrivacidadCambiada = (pDocumento.FilaDocumentoWebVinBR.PrivadoEditores != privadoEditores) || privadoMiembroscomunidadCambiado;
                pDocumento.FilaDocumentoWebVinBR.PrivadoEditores = privadoEditores;
                string[] perfiles = null;
                string[] separadores = { " & ", "," };

                if (mModelSaveRec.SpecificResourceEditors)
                {
                    perfiles = ExtraerTexto(mModelSaveRec.ResourceEditors).Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string id in perfiles)
                    {
                        if (id.StartsWith("g_"))
                        {
                            //Es grupo
                            try
                            {
                                Guid grupoID = new Guid(id.Replace("g_", ""));
                                if (!listaIdsGruposEditores.Contains(grupoID))
                                {
                                    listaIdsGruposEditores.Add(grupoID);
                                }

                                if (!pDocumento.ListaGruposEditores.ContainsKey(grupoID))
                                {
                                    GestorDocumental.AgregarGrupoEditorARecurso(pDocumento.Clave, grupoID);
                                }
                                else if (!pDocumento.ListaGruposEditores[grupoID].FilaGrupoEditor.Editor)
                                {
                                    pDocumento.ListaGruposEditores[grupoID].FilaGrupoEditor.Editor = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                            }
                        }
                        else
                        {
                            try
                            {
                                Guid perfilID = new Guid(id);
                                if (!listaIdsEditores.Contains(perfilID))
                                {
                                    listaIdsEditores.Add(perfilID);
                                }

                                if (!pDocumento.ListaPerfilesEditores.ContainsKey(perfilID))
                                {
                                    GestorDocumental.AgregarEditorARecurso(pDocumento.Clave, perfilID);
                                }
                                else if (!pDocumento.ListaPerfilesEditores[perfilID].FilaEditor.Editor)
                                {
                                    pDocumento.ListaPerfilesEditores[perfilID].FilaEditor.Editor = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                            }
                        }
                    }
                }
                else
                {
                    if (!listaIdsEditores.Contains(IdentidadActual.PerfilID))
                    {
                        listaIdsEditores.Add(IdentidadActual.PerfilID);
                    }

                    if (!pDocumento.ListaPerfilesEditores.ContainsKey(IdentidadActual.PerfilID))
                    {
                        GestorDocumental.AgregarEditorARecurso(pDocumento.Clave, IdentidadActual.PerfilID);
                    }
                }

                if (privadoEditores && mModelSaveRec.ResourceVisibility == ResourceVisibility.SpecificReaders)
                {
                    perfiles = ExtraerTexto(mModelSaveRec.ResourceReaders).Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string id in perfiles)
                    {
                        if (id.StartsWith("g_"))
                        {
                            //Es grupo
                            try
                            {
                                Guid grupoID = new Guid(id.Replace("g_", ""));
                                if (!listaIdsGruposEditores.Contains(grupoID))
                                {
                                    listaIdsGruposEditores.Add(grupoID);
                                }

                                if (!pDocumento.ListaGruposEditores.ContainsKey(grupoID))
                                {
                                    GestorDocumental.AgregarGrupoLectorARecurso(pDocumento.Clave, grupoID);
                                }
                                else if (pDocumento.ListaGruposEditores[grupoID].FilaGrupoEditor.Editor)
                                {
                                    pDocumento.ListaGruposEditores[grupoID].FilaGrupoEditor.Editor = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                            }
                        }
                        else
                        {
                            try
                            {
                                Guid perfilID = new Guid(id);
                                if (!listaIdsEditores.Contains(perfilID))
                                {
                                    listaIdsEditores.Add(perfilID);
                                }

                                if (!pDocumento.ListaPerfilesEditores.ContainsKey(perfilID))
                                {
                                    GestorDocumental.AgregarLectorARecurso(pDocumento.Clave, perfilID);
                                }
                                else if (pDocumento.ListaPerfilesEditores[perfilID].FilaEditor.Editor)
                                {
                                    pDocumento.ListaPerfilesEditores[perfilID].FilaEditor.Editor = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                            }
                        }
                    }
                }

                if (!EditandoRecurso && !EditandoFormSem)
                {
                    if (!listaIdsEditores.Contains(IdentidadActual.PerfilID))
                    {
                        listaIdsEditores.Add(IdentidadActual.PerfilID);
                    }

                    if (!pDocumento.ListaPerfilesEditores.ContainsKey(IdentidadActual.PerfilID))
                    {
                        GestorDocumental.AgregarEditorARecurso(pDocumento.Clave, IdentidadActual.PerfilID);
                    }
                }

                pPrivacidadCambiada = pPrivacidadCambiada || listaEditoresOriginal.Count != pDocumento.ListaPerfilesEditores.Count;

                if (!pPrivacidadCambiada)
                {
                    foreach (Guid perfilID in pDocumento.ListaPerfilesEditores.Keys)
                    {
                        if (!listaEditoresOriginal.Contains(perfilID))
                        {
                            pPrivacidadCambiada = true;
                            break;
                        }
                    }

                    if (Documento != null)
                    {
                        foreach (Guid grupoID in Documento.ListaGruposEditores.Keys)
                        {
                            if (!listaGruposEditoresOriginal.Contains(grupoID))
                            {
                                pPrivacidadCambiada = true;
                                break;
                            }
                        }
                    }
                }

                List<Guid> listaEditoresActuales = new List<Guid>(pDocumento.ListaPerfilesEditores.Keys);

                foreach (Guid perfilID in listaEditoresActuales)
                {
                    if (!listaIdsEditores.Contains(perfilID))
                    {
                        GestorDocumental.QuitarEditorDeRecurso(pDocumento.ListaPerfilesEditores[perfilID]);
                    }
                }

                List<Guid> listaGruposEditoresActuales = new List<Guid>(pDocumento.ListaGruposEditores.Keys);
                if (Documento != null)
                {
                    foreach (Guid grupoID in listaGruposEditoresActuales)
                    {
                        if (!listaIdsGruposEditores.Contains(grupoID))
                        {
                            GestorDocumental.QuitarGrupoEditorDeRecurso(Documento.ListaGruposEditores[grupoID]);
                            pPrivacidadCambiada = true;
                        }
                    }
                }

                foreach (Guid perfilEditorEliminadoID in listaEditoresOriginal)
                {
                    if (!listaEditoresOriginal.Contains(perfilEditorEliminadoID))
                    {
                        GestorDocumental.QuitarGrupoEditorDeRecurso(pDocumento.ListaGruposEditores[perfilEditorEliminadoID]);
                    }
                }

                foreach (Guid grupoEliminadoID in listaGruposEditoresOriginal)
                {
                    if (!listaIdsGruposEditores.Contains(grupoEliminadoID))
                    {
                        pListaGruposEditoresEliminados.Add(grupoEliminadoID);
                    }
                }

                //Cargar los editores en el gestor identidad 
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                pDocumento.GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesDePerfilesEnProyecto(listaIdsEditores, mControladorBase.UsuarioActual.ProyectoID));
                pDocumento.GestorDocumental.GestorIdentidades.RecargarHijos();
                identCN.Dispose();
            }
        }

        /// <summary>
        /// Guarda las categorías seleccionadas.
        /// </summary>
        /// <param name="pCategoriasSeleccionadas">IDs de la categorías seleccionadas</param>
        private void GuardarDatosCategorias(List<Guid> pCategoriasSeleccionadas, DocumentoWeb pDocumento)
        {
            List<CategoriaTesauro> listaCategoriasNuevas = new List<CategoriaTesauro>();
            List<CategoriaTesauro> listaCategoriasEliminadas = new List<CategoriaTesauro>();

            foreach (Guid categoriaID in pCategoriasSeleccionadas)
            {
                if (!pDocumento.Categorias.ContainsKey(categoriaID))
                {
                    CategoriaTesauro categoria = pDocumento.GestorDocumental.GestorTesauro.ListaCategoriasTesauro[categoriaID];
                    listaCategoriasNuevas.Add(categoria);
                }
            }

            foreach (CategoriaTesauro categoria in pDocumento.Categorias.Values)
            {
                if (!pCategoriasSeleccionadas.Contains(categoria.Clave))
                {
                    listaCategoriasEliminadas.Add(categoria);
                }
            }

            pDocumento.GestorDocumental.VincularDocumentoACategorias(listaCategoriasNuevas, pDocumento, IdentidadActual.Clave, ProyectoSeleccionado.Clave);
            pDocumento.GestorDocumental.DesvincularDocumentoDeCategorias(pDocumento, listaCategoriasEliminadas, IdentidadActual.Clave, ProyectoSeleccionado.Clave);

            pDocumento.RecargarCategoriasTesauro();
        }

        /// <summary>
        /// Guarda los datos de las respuestas del recurso encuesta.
        /// </summary>
        /// <param name="pDocumento">Documento para actualizar autoría</param>
        private void GuardarDatosRespuestasEncuesta(Documento pDocumento)
        {
            if (pDocumento.TipoDocumentacion == TiposDocumentacion.Encuesta)
            {
                List<RespuestaRecurso> listaRespuestasAnt = new List<RespuestaRecurso>(pDocumento.ListaRespuestas.Values);

                foreach (RespuestaRecurso respuesta in listaRespuestasAnt)
                {
                    GestorDocumental.QuitarRespuestaDeRecurso(respuesta);
                }

                listaRespuestasAnt.Clear();
                short orden = 1;

                foreach (string respuesta in ExtraerTexto(mModelSaveRec.PollResponses).Split(new string[] { "[[&]]" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    GestorDocumental.AgregarRespuestaARecurso(pDocumento.Clave, Guid.NewGuid(), respuesta, orden);
                    orden++;
                }
            }
        }

        private bool EstaCreandoVersionEnlace()
        {
            bool creandoVersion = false;

            if (RequestParams("version") != null)
            {
                bool.TryParse(RequestParams("version").ToLower(), out creandoVersion);
            }

            return creandoVersion && mTipoDocumento == TiposDocumentacion.Hipervinculo && (!string.IsNullOrEmpty(RequestParams("documentoID")) || !string.IsNullOrEmpty(RequestParams("docID")));
        }

        private ActionResult FuncionalidadSharepoint(AD.EntityModel.Models.Documentacion.Documento pDoc = null)
        {
            ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            string sharepointConfigurado = paramCN.ObtenerParametroAplicacion("SharepointClientID");
            if (!string.IsNullOrEmpty(sharepointConfigurado))
            {
                string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();
                string documentoID = RequestParams("documentoID");
                if (string.IsNullOrEmpty(documentoID))
                {
                    documentoID = RequestParams("docID");
                }
                SharepointController sharepointController = new SharepointController(documentoID, mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                string enlace = docCN.ObtenerEnlaceDocumentoPorDocumentoID(new Guid(documentoID));

                if (string.IsNullOrEmpty(enlace) && Session.Get("EnlaceDocumentoAgregar") != null)
                {
                    enlace = Session.GetString("EnlaceDocumentoAgregar");
                }
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (!string.IsNullOrEmpty(enlace) && UtilCadenas.EsEnlaceSharepoint(enlace, oneDrivePermitido))
                {
                    string urlRedirect = $"{urlServicioLogin}/LoginSharepoint?urlInicio={RequestUrl}&usuario={UsuarioActual.UsuarioID}";

                    //Guardamos la url de inicio en una cookie
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddHours(1);
                    mHttpContextAccessor.HttpContext.Response.Cookies.Append("urlInicio", RequestUrl, cookieOptions);

                    //Comprobamos en BD si existe un token para el usuario
                    Guid personaID = (Guid)IdentidadActual.PerfilUsuario.PersonaID;
                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    Guid usuarioID = (Guid)personaCN.ObtenerUsuarioIDDePersonaID(personaID);
                    string tokenUsuario = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.Sharepoint, usuarioID);
                    string refreshToken = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.SharepointRefresh, usuarioID);
                    if (!string.IsNullOrEmpty(tokenUsuario))
                    {
                        //Si existe comprobamos si sigue siendo valido
                        bool tokenEsValido = sharepointController.ComprobarValidezToken(tokenUsuario);
                        if (!tokenEsValido)
                        {
                            tokenUsuario = sharepointController.RenovarToken(refreshToken, usuarioID);
                            if (string.IsNullOrEmpty(tokenUsuario))
                            {
                                return Redirect(urlRedirect);
                            }
                            sharepointController.Token = tokenUsuario;
                            string nombreFichero = sharepointController.GuardarArchivoDesdeAPI(enlace);
                            if (pDoc != null)
                            {
                                pDoc.Enlace = $"{pDoc.Enlace}|||{nombreFichero}";
                            }
                        }
                        else
                        {
                            //Si es valido, guardamos el archivo
                            sharepointController.Token = tokenUsuario;
                            string nombreFichero = sharepointController.GuardarArchivoDesdeAPI(enlace);
                            if (pDoc != null)
                            {
                                pDoc.Enlace = $"{pDoc.Enlace}|||{nombreFichero}";
                            }
                        }
                    }
                    else
                    {
                        //Si no existe, generamos uno nuevo llamando al servicio de login
                        return Redirect(urlRedirect);
                    }
                }

                return null;
            }
            return null;
        }

        private ActionResult FuncionalidadSharepointCrearVersion(Guid docID, string enlace, out string enlaceNuevo)
        {
            ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            string sharepointConfigurado = paramCN.ObtenerParametroAplicacion("SharepointClientID");
            enlaceNuevo = enlace;
            if (!string.IsNullOrEmpty(sharepointConfigurado))
            {
                string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();

                SharepointController sharepointController = new SharepointController(docID.ToString(), mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);

                if (string.IsNullOrEmpty(enlace) && Session.Get("EnlaceDocumentoAgregar") != null)
                {
                    enlace = Session.GetString("EnlaceDocumentoAgregar");
                }
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (!string.IsNullOrEmpty(enlace) && UtilCadenas.EsEnlaceSharepoint(enlace, oneDrivePermitido))
                {
                    string urlRedirect = $"{urlServicioLogin}/LoginSharepoint?urlInicio={RequestUrl}&usuario={UsuarioActual.UsuarioID}";

                    //Guardamos la url de inicio en una cookie       
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(1);
                    mHttpContextAccessor.HttpContext.Response.Cookies.Append("urlInicio", RequestUrl, cookieOptions);

                    //Comprobamos en BD si existe un token para el usuario
                    Guid personaID = (Guid)IdentidadActual.PerfilUsuario.PersonaID;
                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    Guid usuarioID = (Guid)personaCN.ObtenerUsuarioIDDePersonaID(personaID);
                    string tokenUsuario = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.Sharepoint, usuarioID);
                    string refreshToken = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.SharepointRefresh, usuarioID);
                    if (!string.IsNullOrEmpty(tokenUsuario))
                    {
                        //si existe en la bd comprobamos si el token sigue siendo valido
                        bool tokenEsValido = sharepointController.ComprobarValidezToken(tokenUsuario);
                        if (!tokenEsValido)
                        {
                            tokenUsuario = sharepointController.RenovarToken(refreshToken, usuarioID);
                            if (string.IsNullOrEmpty(tokenUsuario))
                            {
                                return Redirect(urlRedirect);
                            }
                            sharepointController.Token = tokenUsuario;
                            string nombreFichero = sharepointController.GuardarArchivoDesdeAPI(enlace);
                            if (!enlace.Contains("|||"))
                            {
                                enlaceNuevo = $"{enlace}|||{nombreFichero}";
                            }
                        }
                        else
                        {
                            //si existe y es valido guardamos el archivo
                            sharepointController.Token = tokenUsuario;
                            string nombreFichero = sharepointController.GuardarArchivoDesdeAPI(enlace);
                            if (!enlace.Contains("|||"))
                            {
                                enlaceNuevo = $"{enlace}|||{nombreFichero}";
                            }
                        }
                    }
                    else
                    {
                        //si no existe generamos uno nuevo llamando al servicio de login
                        return Redirect(urlRedirect);
                    }
                }
                return null;
            }
            return null;

        }

        private ActionResult FuncionalidadSharepointComprobarToken()
        {
            ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            string sharepointConfigurado = paramCN.ObtenerParametroAplicacion("SharepointClientID");
            if (!string.IsNullOrEmpty(sharepointConfigurado))
            {
                string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();
                string documentoID = RequestParams("documentoID");
                if (string.IsNullOrEmpty(documentoID))
                {
                    documentoID = RequestParams("docID");
                }
                SharepointController sharepointController = new SharepointController(documentoID, mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                string enlace = docCN.ObtenerEnlaceDocumentoPorDocumentoID(new Guid(documentoID));

                if (string.IsNullOrEmpty(enlace) && Session.Get("EnlaceDocumentoAgregar") != null)
                {
                    enlace = Session.GetString("EnlaceDocumentoAgregar");
                }

                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (!string.IsNullOrEmpty(enlace) && UtilCadenas.EsEnlaceSharepoint(enlace, oneDrivePermitido))
                {
                    string urlRedirect = $"{urlServicioLogin}/LoginSharepoint?urlInicio={RequestUrl}&usuario={UsuarioActual.UsuarioID}";

                    // guardamos la url de inicio en una cookie
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(1);
                    mHttpContextAccessor.HttpContext.Response.Cookies.Append("urlInicio", RequestUrl, cookieOptions);

                    //comprobamos en BD si existe un token para el usuario
                    Guid personaID = (Guid)IdentidadActual.PerfilUsuario.PersonaID;
                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    Guid usuarioID = (Guid)personaCN.ObtenerUsuarioIDDePersonaID(personaID);
                    string tokenUsuario = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.Sharepoint, usuarioID);
                    string tokenRefresh = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.SharepointRefresh, usuarioID);
                    if (!string.IsNullOrEmpty(tokenUsuario))
                    {
                        //si existe en la bd comprobamos si el token sigue siendo valido
                        bool tokenEsValido = sharepointController.ComprobarValidezToken(tokenUsuario);
                        if (!tokenEsValido)
                        {
                            tokenUsuario = sharepointController.RenovarToken(tokenRefresh, usuarioID);
                            if (string.IsNullOrEmpty(tokenUsuario))
                            {
                                return Redirect(urlRedirect);
                            }
                            sharepointController.Token = tokenUsuario;
                        }
                        else
                        {
                            //si existe lo guardamos para usarlo y que nos permita descargar
                            sharepointController.Token = tokenUsuario;
                        }
                    }
                    else
                    {
                        //si no existe generamos uno nuevo llamando al servicio de login
                        return Redirect(urlRedirect);
                    }
                }
                return null;
            }
            return null;
        }

        private ActionResult FuncionalidadSharepointComprobarTokenAntesSubirRecurso(string enlace)
        {
            ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            string sharepointConfigurado = paramCN.ObtenerParametroAplicacion("SharepointClientID");
            if (!string.IsNullOrEmpty(sharepointConfigurado))
            {
                string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();
                string documentoID = RequestParams("documentoID");
                if (string.IsNullOrEmpty(documentoID))
                {
                    documentoID = RequestParams("docID");
                }
                SharepointController sharepointController = new SharepointController(documentoID, mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);

                if (string.IsNullOrEmpty(enlace) && Session.Get("EnlaceDocumentoAgregar") != null)
                {
                    enlace = Session.GetString("EnlaceDocumentoAgregar");
                }
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (!string.IsNullOrEmpty(enlace) && UtilCadenas.EsEnlaceSharepoint(enlace, oneDrivePermitido))
                {
                    string urlRedirect = $"{urlServicioLogin}/LoginSharepoint?urlInicio={RequestUrl}&usuario={UsuarioActual.UsuarioID}";

                    // guardamos la url de inicio en una cookie
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(1);
                    mHttpContextAccessor.HttpContext.Response.Cookies.Append("urlInicio", RequestUrl, cookieOptions);

                    //comprobamos en BD si existe un token para el usuario
                    Guid personaID = (Guid)IdentidadActual.PerfilUsuario.PersonaID;
                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    Guid usuarioID = (Guid)personaCN.ObtenerUsuarioIDDePersonaID(personaID);
                    string tokenUsuario = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.Sharepoint, usuarioID);
                    string tokenRefresh = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.SharepointRefresh, usuarioID);
                    if (!string.IsNullOrEmpty(tokenUsuario))
                    {
                        //si existe en la bd comprobamos si el token sigue siendo valido
                        bool tokenEsValido = sharepointController.ComprobarValidezToken(tokenUsuario);
                        if (!tokenEsValido)
                        {
                            tokenUsuario = sharepointController.RenovarToken(tokenRefresh, usuarioID);
                            if (string.IsNullOrEmpty(tokenUsuario))
                            {
                                return Redirect(urlRedirect);
                            }
                            sharepointController.Token = tokenUsuario;
                        }
                        else
                        {
                            //si existe lo guardamos para usarlo y que nos permita descargar
                            sharepointController.Token = tokenUsuario;
                        }
                    }
                    else
                    {
                        //si no existe generamos uno nuevo llamando al servicio de login
                        return Redirect(urlRedirect);
                    }
                }
                return null;
            }
            return null;

        }

        /// <summary>
        /// Guarda la imagen principal del recurso semántico.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        private void GuardarImagenPrincipalRecSemantico(Documento pDocumento, Ontologia pOntologia)
        {
            if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico)
            {
                try
                {
                    string docImgRep = "doc,";

                    if (pDocumento.Clave != mDocumentoID)
                    {
                        docImgRep = pDocumento.Clave.ToString() + ",";
                    }

                    if (!string.IsNullOrEmpty(mModelSaveRec.ImageRepresentativeValue) && mModelSaveRec.ImageRepresentativeValue.Contains(docImgRep) && pOntologia.ConfiguracionPlantilla.PropiedadImagenRepre.Key != null)
                    {
                        string imagen = mModelSaveRec.ImageRepresentativeValue.Substring(mModelSaveRec.ImageRepresentativeValue.IndexOf(docImgRep) + docImgRep.Length);
                        imagen = imagen.Substring(0, imagen.IndexOf("|"));

                        pDocumento.FilaDocumento.NombreCategoriaDoc = pOntologia.ConfiguracionPlantilla.ObtenerTamaniosTextoPropiedadImagenMini(pOntologia.ConfiguracionPlantilla.PropiedadImagenRepre.Key, pOntologia.ConfiguracionPlantilla.PropiedadImagenRepre.Value) + imagen;

                        if (GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].FilaDocumento.NombreCategoriaDoc != null && GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].FilaDocumento.NombreCategoriaDoc.Contains("class=")) //La ontología tiene icono
                        {
                            pDocumento.FilaDocumento.NombreCategoriaDoc += "|class=" + GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].FilaDocumento.NombreCategoriaDoc.Split('=')[1];
                        }
                    }
                    else if (!mCapturaSercicioEncargada || pDocumento.Clave != mDocumentoID)
                    {
                        if (GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].FilaDocumento.NombreCategoriaDoc != null) //La ontología tiene icono
                        {
                            pDocumento.FilaDocumento.NombreCategoriaDoc = GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].FilaDocumento.NombreCategoriaDoc;
                        }
                        else
                        {
                            pDocumento.FilaDocumento.NombreCategoriaDoc = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    GuardarMensajeErrorAdmin("Error en la configuración de la imagen principal del recurso.", ex);
                    GuardarLogError("Error en la configuración de la imagen principal del recurso. " + ex.ToString());
                    throw;
                }
            }
        }

        /// <summary>
        /// Guarda los datos de la autoría del recurso.
        /// </summary>
        /// <param name="pDocumento">Documento para actualizar autoría</param>
        private void GuardarDatosAutoriaDocumento(Documento pDocumento)
        {
            string autorPropio = "";
            if (mModelSaveRec.CreatorIsAuthor && pDocumento.CreadorID.Equals(IdentidadActual.Clave))
            {
                pDocumento.FilaDocumento.CreadorEsAutor = true;
                if (IdentidadOrganizacion == null)
                {
                    TiposIdentidad tipoIdentidadPublicador = IdentidadActual.Tipo;
                    if (tipoIdentidadPublicador.Equals(TiposIdentidad.ProfesionalCorporativo))
                    {
                        //Si participa en modo corporativo, establezco solo el nombre de la organización
                        autorPropio = IdentidadActual.NombreOrganizacion;
                    }
                    else if (tipoIdentidadPublicador.Equals(TiposIdentidad.ProfesionalPersonal))
                    {
                        //Si participa en modo profesional-personal, pongo su nombre junto al de la organización
                        autorPropio = IdentidadActual.NombreOrganizacion + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + IdentidadActual.PerfilUsuario.FilaPerfil.NombrePerfil;
                    }
                    else
                    {
                        autorPropio = IdentidadActual.PerfilUsuario.FilaPerfil.NombrePerfil;
                    }
                }
                else
                {
                    autorPropio = IdentidadOrganizacion.Nombre(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave));
                }
            }
            else if (!mModelSaveRec.CreatorIsAuthor)
            {
                autorPropio = ExtraerTexto(mModelSaveRec.Authors).Trim();
                pDocumento.FilaDocumento.CreadorEsAutor = false;
                pDocumento.FilaDocumento.Autor = autorPropio;
            }

            if (ExtraerTexto(mModelSaveRec.Authors).Trim() != "" && pDocumento.FilaDocumento.CreadorEsAutor)
            {
                autorPropio = autorPropio + ", " + ExtraerTexto(mModelSaveRec.Authors).Trim();
            }

            if (!string.IsNullOrEmpty(autorPropio))
            {
                pDocumento.FilaDocumento.Autor = autorPropio;
            }

        }

        /// <summary>
        /// Prepara la licencia para la edición.
        /// </summary>
        private void PrepararLicencia()
        {
            LicenseEditorModel editLicModel = new LicenseEditorModel();
            mEditRecCont.ModifyResourceModel.LicenseEditorModel = editLicModel;
            editLicModel.EcosystemProjectName = NombreProyectoEcosistema;

            if (ParametrosGeneralesRow.MensajeLicenciaPorDefecto != null)
            {
                editLicModel.MessageDefaultLicense = UtilCadenas.ObtenerTextoDeIdioma(ParametrosGeneralesRow.MensajeLicenciaPorDefecto, UtilIdiomas.LanguageCode, null);
            }

            if (ParametrosGeneralesRow.LicenciaPorDefecto != null)
            {
                editLicModel.DefaultLicense = ParametrosGeneralesRow.LicenciaPorDefecto;
            }

            if (Documento != null && Documento.FilaDocumento.Licencia != null)
            {
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.License = Documento.FilaDocumento.Licencia;
                editLicModel.License = Documento.FilaDocumento.Licencia;

                #region Licencia No Editable

                bool licenciaNoEditable = false;

                if (Documento.BaseRecursos.Count > 1) //Compruebo que se pueda editar la licencia:
                {
                    foreach (Guid proyectoID in Documento.ListaProyectos)
                    {
                        if (proyectoID != ProyectoSeleccionado.Clave && !string.IsNullOrEmpty(ControladorProyecto.ObtenerFilaParametrosGeneralesDeProyecto(proyectoID).LicenciaPorDefecto))
                        {
                            licenciaNoEditable = true;
                            break;
                        }
                    }
                }

                editLicModel.NotEditable = licenciaNoEditable;

                #endregion
            }
            else if (mEditandoRecursoCargaMasiva && !string.IsNullOrEmpty(mDatosRecursoEditadoCargaMasiva[10]))
            {
                mEditRecCont.ModifyResourceModel.DocumentEditionModel.License = mDatosRecursoEditadoCargaMasiva[10];
                editLicModel.License = mDatosRecursoEditadoCargaMasiva[10];
            }
        }

        [HttpGet, HttpPost]
        [TypeFilter(typeof(PermisosRecursos), Arguments = new object[] { new ulong[] { (ulong)PermisoRecursos.EditarRecursoTipoEnlace } })]
        public ActionResult GenerarVersionRapida(string pDocumentoID)
        {
            try
            {
                Guid documentoID = new Guid(pDocumentoID);
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                docCN.ObtenerDocumentoPorIDCargarTotal(documentoID, dataWrapperDocumentacion, true, false, null);
                GestorDocumental gestDoc = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
                docCN.Dispose();

                Documento documentoAntiguo = gestDoc.ListaDocumentos[new Guid(pDocumentoID)];
                mDocumentoID = documentoID;
                GestorDocumental = gestDoc;

                ActionResult redirect = FuncionalidadSharepointComprobarToken();
                if (redirect != null)
                {
                    string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();
                    string url = $"{urlServicioLogin}/LoginSharepoint?urlInicio={RequestUrl}&usuario={UsuarioActual.UsuarioID}";
                    return GnossResultERROR(url);
                }

                //Guardamos el recurso en base de datos
                Documento documentoNuevaVersion = CrearVersionDocumento_ModificarRecurso();
                DocumentoWeb documentoWeb = new DocumentoWeb(documentoNuevaVersion.FilaDocumento, gestDoc);

                #region Gestion Virtuoso

                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> listaDocumentoWebVinBaseRecursos = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(item => item.DocumentoID.Equals(documentoAntiguo.FilaDocumento.DocumentoID)).ToList();
                //Eliminamos de virtuoso los datos de la versión anterior
                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos baseRecurso in listaDocumentoWebVinBaseRecursos)
                {
                    string infoExtra = null;
                    Guid proyectoID = ProyectoSeleccionado.Clave;
                    if (proyectoID == ProyectoAD.MetaProyecto)
                    {
                        infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                    }

                    ControladorDocumentacion.ActualizarGnossLive(proyectoID, documentoAntiguo.Clave, AccionLive.Eliminado, (int)TipoLive.Recurso, PrioridadLive.Alta, infoExtra, mAvailableServices);
                    ControladorDocumentacion.ActualizarGnossLive(proyectoID, documentoAntiguo.ObtenerPublicadorEnBR(baseRecurso.BaseRecursosID), AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);
                    ControladorDocumentacion.EliminarRecursoModeloBaseSimple(documentoAntiguo.Clave, proyectoID, (short)documentoAntiguo.TipoDocumentacion, mAvailableServices);
                }

                ControladorDocumentacion controDoc = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                controDoc.ActualizarNumRecCatTesDeTesauroDeRecursoEliminado(documentoAntiguo, mAvailableServices);

                //Enviamos los datos de la nueva versión del recurso a Rabbit para guardar en Virtuoso
                foreach (Guid brID in listaDocumentoWebVinBaseRecursos.Select(item => item.BaseRecursosID))
                {
                    try
                    {
                        string infoExtra = null;

                        if (mGestoresAddToGnoss == null)
                        {
                            mGestoresAddToGnoss = new Dictionary<Guid, GestorAddToGnoss>();
                            GestorAddToGnoss gestorAddtoGnoss = new GestorAddToGnoss();
                            gestorAddtoGnoss.IdentidadEnProyecto = IdentidadActual.Clave;
                            gestorAddtoGnoss.ProyectoBRID = IdentidadActual.FilaIdentidad.ProyectoID;
                            mGestoresAddToGnoss.Add(brID, gestorAddtoGnoss);
                        }
                        if (mGestoresAddToGnoss[brID].ProyectoBRID == ProyectoAD.MetaProyecto)
                        {
                            infoExtra = mGestoresAddToGnoss[brID].PerfilEnProyecto.ToString();
                        }

                        ControladorDocumentacion.ActualizarGnossLive(mGestoresAddToGnoss[brID].ProyectoBRID, documentoNuevaVersion.Clave, AccionLive.Agregado, (int)TipoLive.Recurso, PrioridadLive.Alta, infoExtra, mAvailableServices);
                        ControladorDocumentacion.ActualizarGnossLive(mGestoresAddToGnoss[brID].ProyectoBRID, mGestoresAddToGnoss[brID].IdentidadEnProyecto, AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);

                        ControladorDocumentacion.AgregarRecursoModeloBaseSimple(documentoNuevaVersion.Clave, mGestoresAddToGnoss[brID].ProyectoBRID, documentoNuevaVersion.FilaDocumento.Tipo, null, ",##enlaces##" + ExtraerTexto(/*mModelSaveRec.TagsLinks*/"") + "##enlaces##", PrioridadBase.Alta, mAvailableServices);

                        //Actualización Offline a partir de un servicio UDP
                        //Llamada asincrona para actualizar la popularidad del recurso:
                        ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("recursos", documentoNuevaVersion.Clave, mGestoresAddToGnoss[brID].ProyectoBRID, mGestoresAddToGnoss[brID].IdentidadEnProyecto, documentoNuevaVersion.CreadorID);

                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, mlogger);
                    }
                }

                #endregion

                string urlRedirect = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, documentoWeb, (IdentidadOrganizacion != null));
                urlRedirect += "?versioned";
                string enlaceNuevo;
                ActionResult redireccion = FuncionalidadSharepointCrearVersion(documentoNuevaVersion.FilaDocumento.DocumentoID, Documento.FilaDocumento.Enlace, out enlaceNuevo);
                if (redireccion != null)
                {
                    string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();
                    urlRedirect = $"{urlServicioLogin}/LoginSharepoint?urlInicio={RequestUrl}&usuario={UsuarioActual.UsuarioID}";
                    return GnossResultERROR(urlRedirect);
                }
                return GnossResultOK(urlRedirect);
            }
            catch (Exception ex)
            {
                return GnossResultERROR(ex.Message);
            }
        }

        /// <summary>
        /// Acción de guardar recurso.
        /// </summary>
        /// <param name="pPaginaModel">Modelo de subir archivo adjunto</param>
        /// <returns>Acción resultado</returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult GuardarRecurso(DocumentEditionModel pPaginaModel)
        {
            StringBuilder traza = new StringBuilder();

            try
            {
                ObtenerTipoDocumentoPeticion();
                traza.Append("Antes de LimpiarInyeccionCodigo");
                pPaginaModel.Description = HttpUtility.UrlEncode(LimpiarInyeccionCodigoSegunTipoRecurso(HttpUtility.UrlDecode(pPaginaModel.Description)));
                traza.Append("Antes de TratarImagenesDescripcion");
                TratarImagenesDescripcion(pPaginaModel);
                ActionResult respuesta;


                if (EditandoRecurso && !Duplicando)
                {
                    traza.Append("EditandoRecurso && !Duplicando");
                    traza.Append("Antes de CargarInicial_ModificarRecurso");
                    CargarInicial_ModificarRecurso();

                    if (EsEdicionMultiple)
                    {
                        if ((Documento.TipoDocumentacion.Equals(TiposDocumentacion.FicheroServidor) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Nota)) && !mTipoDocumento.Equals(TiposDocumentacion.Nota))
                        {
                            Documento.FilaDocumento.Tipo = (short)mTipoDocumento;
                        }
                    }

                    traza.Append("Antes de ComprobarRedirecciones_ModificarRecurso");
                    ActionResult redireccion = ComprobarRedirecciones_ModificarRecurso();
                    //ProyectoSeleccionado.Clave, Documento.Clave, comentario.Clave, pComentarioPadre, IdentidadActual.Usuario.Clave, comentario.Fecha
                    if (redireccion != null)
                    {
                        respuesta = redireccion;
                    }

                    traza.Append("Antes de GuardarRecurso_ModificarRecurso");
                    respuesta = GuardarRecurso_ModificarRecurso(pPaginaModel);
                    PublicarModificarEliminarRecurso modelo = new PublicarModificarEliminarRecurso(ProyectoSeleccionado.Clave, Documento.Clave, IdentidadActual.Persona.UsuarioID, DateTime.Now);
                    mIPublishEvents.PublishResource(modelo, "Modificar");
                }
                else if (EditandoRecurso && Duplicando)
                {
                    traza.Append("EditandoRecurso && Duplicando");
                    traza.Append("Antes de CargarInicial_ModificarRecurso");
                    mDocumentoDuplicadoID = Guid.NewGuid();

                    CargarInicial_ModificarRecurso();

                    traza.Append("Antes de DuplicarRecurso_CompletarDatosModel");
                    DuplicarRecurso_CompletarDatosModel(pPaginaModel);

                    traza.Append("Antes de CrearRecurso_SubirRecursoPart2");
                    respuesta = CrearRecurso_SubirRecursoPart2(pPaginaModel);
                    PublicarModificarEliminarRecurso modelo = new PublicarModificarEliminarRecurso(ProyectoSeleccionado.Clave, Documento.Clave, IdentidadActual.Persona.UsuarioID, DateTime.Now);
                    mIPublishEvents.PublishResource(modelo, "Modificar");

                }
                else if (EditandoFormSem || CreandoFormSem || CargaMasivaFormSem)
                {
                    traza.Append("EditandoFormSem || CreandoFormSem || CargaMasivaFormSem");
                    traza.Append("Antes de CargarInicial_ModificarRecursoSemantico");
                    CargarInicial_ModificarRecursoSemantico();
                    traza.Append("Antes de ComprobarRedirecciones_ModificarRecursoSemantico");

                    if (CreandoFormSem)
                    {
                        //Comprobamos que el documento no existe ya:
                        ActionResult redireccionExiste = ComprobarExisteRecursoEnCreacion(mDocumentoID);
                        if (redireccionExiste != null)
                        {
                            return redireccionExiste;
                        }
                    }

                    ActionResult redireccion = ComprobarRedirecciones_ModificarRecursoSemantico();

                    if (redireccion != null)
                    {
                        respuesta = redireccion;
                    }

                    traza.Append("Antes de GuardarRecurso_ModificarRecursoSemantico");
                    respuesta = GuardarRecurso_ModificarRecursoSemantico(pPaginaModel);
                }
                else if (AñadiendoAGnoss)
                {
                    traza.Append("AñadiendoAGnoss");
                    traza.Append("Antes de CrearRecurso_AnyadirGnoss");
                    respuesta = CrearRecurso_AnyadirGnoss(pPaginaModel);
                }
                else
                {
                    //Comprobamos que el documento no existe ya:
                    CargarInicial_SubirRecursoPart2();
                    ActionResult redireccionExiste = ComprobarExisteRecursoEnCreacion(mDocumentoID);

                    if (redireccionExiste != null)
                    {
                        return redireccionExiste;
                    }

                    traza.Append("!AñadiendoAGnoss");
                    traza.Append("Antes de CrearRecurso_SubirRecursoPart2");

                    respuesta = CrearRecurso_SubirRecursoPart2(pPaginaModel, false);
                    PublicarModificarEliminarRecurso modelo = new PublicarModificarEliminarRecurso(ProyectoSeleccionado.Clave, pPaginaModel.Key, IdentidadActual.Persona.UsuarioID, DateTime.Now);
                    mIPublishEvents.PublishResource(modelo, "Crear");
                }

                if (!AñadiendoAGnoss && !FormSemVirtual && !mInsertadoEnGrafoBusqueda && Documento != null && (!(respuesta is GnossResult) || !((GnossResult)respuesta).result.Status.Equals(GnossResult.GnossStatus.Error.ToString())))
                {
                    traza.Append("!AñadiendoAGnoss && !FormSemVirtual && !mInsertadoEnGrafoBusqueda");
                    //Guardado en grafo de búsqueda. AñadirAGnoss lo hace el BASE
                    string rdfConfiguradoRecursoNoSemantico = "";
                    if (!Documento.TipoDocumentacion.Equals(TiposDocumentacion.Semantico))
                    {
                        traza.Append("Antes de ObtenerRdfRecursoNoSemantico");
                        rdfConfiguradoRecursoNoSemantico = ObtenerRdfRecursoNoSemantico();
                    }

                    traza.Append("Después de ObtenerRdfRecursoNoSemantico");
                    Guid? docOriginalID = null;
                    if (Documento.VersionOriginalID != Guid.Empty)
                    {
                        docOriginalID = Documento.VersionOriginalID;
                    }
                    else
                    {
                        docOriginalID = DocumentoID;
                    }                    
                   
                    if (!ComprobarSiSeEstaEditandoUnaMejora(Documento.Clave))
                    {
						traza.Append("Antes de GuardarRecursoEnGrafoBusqueda");
						UtilidadesVirtuoso.GuardarRecursoEnGrafoBusqueda(Documento, true, mDocumentosExtraGuardar, ProyectoSeleccionado, mListaTriplesSemanticos, mOntologia, GestorDocumental.GestorTesauro, rdfConfiguradoRecursoNoSemantico, mCreandoVersion, docOriginalID, UrlIntragnoss, mOtrosArgumentosBase, PrioridadBase.Alta, mAvailableServices);
						traza.Append("Despues de GuardarRecursoEnGrafoBusqueda");
					}
                    
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (pPaginaModel != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(pPaginaModel);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion GuardarRecurso");
                }

                GuardarLogError(ex, "Traza: " + traza.ToString() + " ModeloDatos: " + modeloDatos);
                if (mEditRecCont != null && mEditRecCont.ModifyResourceModel.SemanticResourceModel != null && !string.IsNullOrEmpty(mEditRecCont.ModifyResourceModel.SemanticResourceModel.AdminGenerationError) && ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    return GnossResultERROR(mEditRecCont.ModifyResourceModel.SemanticResourceModel.AdminGenerationError);
                }
                else
                {
                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
                }
            }
        }

        /// <summary>
        /// Devuelve el texto pasado por parámetro después de limpiar la posible inyección de código. En función del tipo de recurso
        /// se será más o menos permisivo a la hora de borrar el texto a limpiar
        /// </summary>
        /// <param name="pTexto">Texto del recurso a limpiar</param>
        /// <returns></returns>
        private string LimpiarInyeccionCodigoSegunTipoRecurso(string pTexto)
        {
            if (mTipoDocumento == TiposDocumentacion.Newsletter)
            {
                HtmlSanitizer htmlSanitizer = new HtmlSanitizer();
                htmlSanitizer.AllowedTags.Add("style");
                return UtilCadenas.LimpiarInyeccionCodigo(pTexto, htmlSanitizer);
            }
            else
            {
                return UtilCadenas.LimpiarInyeccionCodigo(pTexto);
            }
        }

        /// <summary>
        /// Se obtiene el tipo de documento a partir de los parámetros de la petición y se inicializa la variable miembro con la información obtenida.
        /// </summary>
        private void ObtenerTipoDocumentoPeticion()
        {
            short tipoDoc = (short)TiposDocumentacion.Nota;
            if (!string.IsNullOrEmpty(Request.Query["Tipo"]))
            {
                short.TryParse(Request.Query["Tipo"], out tipoDoc);
            }
            else if (RequestParams("tipo") != null)
            {
                short.TryParse(RequestParams("tipo"), out tipoDoc);
            }
            else if (!string.IsNullOrEmpty(Request.Query["TipoFicheroEdit"]))
            {
                short.TryParse(Request.Query["TipoFicheroEdit"], out tipoDoc);
            }

            mTipoDocumento = (TiposDocumentacion)tipoDoc;
        }

        private GnossResult ComprobarExisteRecursoEnCreacion(Guid pDocumentoID)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            DataWrapperDocumentacion dataWrapperDocumentacion = docCN.ObtenerDocumentoPorID(pDocumentoID);
            GestorDocumental gestDoc = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
            docCN.Dispose();

            if (gestDoc.ListaDocumentos.ContainsKey(pDocumentoID))
            {
                Documento doc = gestDoc.ListaDocumentos[pDocumentoID];
                string urlDoc = mControladorBase.UrlsSemanticas.GetURLBaseRecursosEditarDocumento(BaseURL, UtilIdiomas, NombreProy, UrlPerfil, doc, 0, (IdentidadOrganizacion != null));
                string titulo = UtilCadenas.ObtenerTextoDeIdioma(doc.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCEXISTE", titulo, urlDoc));
            }

            return null;
        }

        /// <summary>
        /// Obtiene el rdf configurado para un tipo de recurso en un proyecto
        /// </summary>
        /// <returns></returns>
        private string ObtenerRdfRecursoNoSemantico()
        {
            string rdfConfiguradoRecursoNoSemantico = "";
            ParametroGeneralCN paramGralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCN>(), mLoggerFactory);
            List<ProyectoRDFType> filaProyectoRdfType = paramGralCN.ObtenerProyectoRDFType(ProyectoSeleccionado.Clave, (short)Documento.TipoDocumentacion);
            paramGralCN.Dispose();
            if (filaProyectoRdfType.Count > 0)
            {
                rdfConfiguradoRecursoNoSemantico = filaProyectoRdfType[0].RdfType;
            }
            return rdfConfiguradoRecursoNoSemantico;
        }

        private void DuplicarRecurso_CompletarDatosModel(DocumentEditionModel pPaginaModel)
        {
            // ParametrosAplicacionDS.ParametroAplicacion.FindByParametro("GoogleDrive") != null

            if (mTipoDocumento == TiposDocumentacion.FicheroServidor || mTipoDocumento == TiposDocumentacion.Imagen || mTipoDocumento == TiposDocumentacion.Video)
            {
                // Cargar el documento anterior y cargar los datos necesarios
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dataWrapperDocumentacion = docCN.ObtenerDocumentoPorID(mDocumentoID);
                docCN.Dispose();

                AD.EntityModel.Models.Documentacion.Documento filaDocumento = dataWrapperDocumentacion.ListaDocumento.Where(doc => doc.DocumentoID.Equals(mDocumentoID)).ToList().FirstOrDefault();

                byte[] byteArray;
                string nombre = filaDocumento.Enlace;
                string ext = System.IO.Path.GetExtension(filaDocumento.Enlace).ToLower();

                int resultado = 0;

                string idAuxGestorDocumental = "";
                GestionDocumental gestorDocumental = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                gestorDocumental.Url = mConfigService.ObtenerUrlServicioDocumental();

                byteArray = gestorDocumental.ObtenerDocumento(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, ProyectoSeleccionado.FilaProyecto.OrganizacionID, filaDocumento.ProyectoID.Value, mDocumentoID, ext);


                // TODO: Google Drive

                if (mTipoDocumento == TiposDocumentacion.FicheroServidor)
                {
                    idAuxGestorDocumental = gestorDocumental.AdjuntarDocumento(byteArray, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, mDocumentoDuplicadoID, ext);
                }
                else if (mTipoDocumento == TiposDocumentacion.Imagen)
                {
                    ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                    servicioImagenes.Url = UrlIntragnossServicios;

                    bool correcto = false;
                    correcto = servicioImagenes.AgregarImagenADirectorio(byteArray, UtilArchivos.ContentImagenesDocumentos + "/" + UtilArchivos.DirectorioDocumento(mDocumentoDuplicadoID), mDocumentoDuplicadoID.ToString(), ext);

                    if (correcto)
                    {
                        resultado = 0;
                        idAuxGestorDocumental = gestorDocumental.AdjuntarDocumento(byteArray, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, mDocumentoDuplicadoID, ext);

                        if (!idAuxGestorDocumental.ToUpper().Equals("ERROR"))
                        {
                            resultado = 1;
                        }
                    }
                }
                else if (mTipoDocumento == TiposDocumentacion.Video)
                {
                    // TODO: No se usan los vídeos, se consideran archivos de tipo servidor.

                    ServicioVideos servicioVideos = new ServicioVideos(mConfigService, mLoggingService, mLoggerFactory.CreateLogger<ServicioVideos>(), mLoggerFactory);
                    resultado = servicioVideos.AgregarVideo(byteArray, ext, mDocumentoDuplicadoID);
                }

                pPaginaModel.Link = nombre + ext;
            }
        }

        private bool ComprobarSiSeEstaEditandoUnaMejora(Guid pDocumentoID)
        {
            bool esMejora = false;

            using (DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory))
            {
                esMejora = documentacionCN.ComprobarSiDocumentoEsUnaMejora(pDocumentoID);
            } 

            return esMejora;
        }

		/// <summary>
		/// Añade título y texto alternativo a las imágenes contenidas en la descripción del recurso
		/// </summary>
		/// <param name="pPaginaModel">Modelo de subir archivo adjunto</param>
		/// <returns>Modelo de subir archivo adjunto con las imágenes de la descripción modificadas</returns>
		private DocumentEditionModel TratarImagenesDescripcion(DocumentEditionModel pPaginaModel)
        {
            string descripcion = HttpUtility.UrlDecode(ExtraerTexto(pPaginaModel.Description));
            string descripcionParcial = "";
            string marcaInicioImg = "<img ";
            string marcaFinImg = ">";

            while (descripcion.Contains(marcaInicioImg))
            {
                int inicioImg = descripcion.IndexOf(marcaInicioImg);
                int finImg = descripcion.Substring(inicioImg).IndexOf(marcaFinImg) + inicioImg;
                string imagen = descripcion.Substring(descripcion.IndexOf(marcaInicioImg), finImg - inicioImg + marcaFinImg.Length);
                imagen = AñadirAtributosImagen(imagen, pPaginaModel.Title);
                descripcionParcial += descripcion.Substring(0, inicioImg) + imagen;
                descripcion = descripcion.Substring(finImg + marcaFinImg.Length);
            }

            if (!string.IsNullOrEmpty(descripcion))
            {
                descripcionParcial += descripcion;
            }

            pPaginaModel.Description = System.Net.WebUtility.UrlEncode(descripcionParcial);
            return pPaginaModel;
        }

        /// <summary>
        /// Añade los atributos Alt y Title a la imagen si ésta no los contiene definidos
        /// </summary>
        /// <param name="pImagen">html de la imagen en la que se van a insertar los atributos</param>
        /// <param name="pTituloRecurso">título del recurso que será el valor de los atributos</param>
        /// <returns>html de la imagen con los atributos añadidos</returns>
        private static string AñadirAtributosImagen(string pImagen, string pTituloRecurso)
        {
            pTituloRecurso = UtilCadenas.EliminarHtmlDeTexto(HttpUtility.UrlDecode(pTituloRecurso));
            string marcaImg = "<img";

            if (pImagen.Contains(" alt=\"\""))
            {
                pImagen = pImagen.Replace("alt=\"\"", "alt=\"" + pTituloRecurso + "\"");
            }
            else if (!pImagen.Contains(" alt="))
            {
                pImagen = pImagen.Substring(0, marcaImg.Length) + " alt=\"" + pTituloRecurso + "\"" + pImagen.Substring(marcaImg.Length);
            }

            if (pImagen.Contains(" title=\"\""))
            {
                pImagen = pImagen.Replace("title=\"\"", "title=\"" + pTituloRecurso + "\"");
            }
            else if (!pImagen.Contains(" title="))
            {
                pImagen = pImagen.Substring(0, marcaImg.Length) + " title=\"" + pTituloRecurso + "\"" + pImagen.Substring(marcaImg.Length);
            }

            return pImagen;
        }

        /// <summary>
        /// Extrae los cambios básicos del documento.
        /// </summary>
        /// <param name="pDocumento">Documento del que hay que extraer los cambios</param>
        private void ExtraerCambiosBasicos(Documento pDocumento)
        {
            string titulo = UtilCadenas.EliminarHtmlDeTexto(ExtraerTexto(mModelSaveRec.Title).Trim());
            string descripcion = HttpUtility.UrlDecode(ExtraerTexto(mModelSaveRec.Description));

            pDocumento.Titulo = titulo;

            if (!mModelSaveRec.ProtectDocumentProtected.HasValue)
            {
                // Protección de documentos. Los borradores nunca se protegen
                if (!pDocumento.FilaDocumento.IdentidadProteccionID.HasValue && mModelSaveRec.Protected && !pDocumento.EsBorrador)
                {
                    pDocumento.FilaDocumento.Protegido = true;
                    pDocumento.FilaDocumento.IdentidadProteccionID = mControladorBase.UsuarioActual.IdentidadID;
                    pDocumento.FilaDocumento.FechaProteccion = DateTime.Now;
                }
                else if ((!mModelSaveRec.Protected) || pDocumento.EsBorrador)
                {
                    pDocumento.FilaDocumento.Protegido = false;
                    pDocumento.FilaDocumento.IdentidadProteccionID = null;
                    pDocumento.FilaDocumento.FechaProteccion = null;
                }
            }
            else
            {
                // Proteger la nueva versión creada del documento
                if (mModelSaveRec.ProtectDocumentProtected.Value && !pDocumento.EsBorrador)
                {
                    pDocumento.FilaDocumento.Protegido = true;
                    pDocumento.FilaDocumento.IdentidadProteccionID = mControladorBase.UsuarioActual.IdentidadID;
                    pDocumento.FilaDocumento.FechaProteccion = DateTime.Now;
                }
            }

            pDocumento.Descripcion = descripcion;

            if (pDocumento.TipoDocumentacion == TiposDocumentacion.Newsletter)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion docDW = docCN.ObtenerDocumentoNewsletterPorDocumentoID(pDocumento.Clave);

                if (mModelSaveRec.NewsletterManual)
                {
                    if (docDW.ListaDocumentoNewsLetter.Count == 1)
                    {
                        AD.EntityModel.Models.Documentacion.DocumentoNewsletter documentoNewsLetter = docDW.ListaDocumentoNewsLetter.FirstOrDefault();
                        mEntityContext.EliminarElemento(documentoNewsLetter);
                        GestorDocumental.DataWrapperDocumentacion.ListaDocumentoNewsLetter.Remove(documentoNewsLetter);
                        GestorDocumental.DataWrapperDocumentacion.Merge(docDW);
                    }
                }
                else
                {
                    pDocumento.Descripcion = "";
                    if (docDW.ListaDocumentoNewsLetter.Count == 1)
                    {
                        docDW.ListaDocumentoNewsLetter.FirstOrDefault().Newsletter = docDW.ListaDocumentoNewsLetter.FirstOrDefault().NewsletterTemporal;
                        docDW.ListaDocumentoNewsLetter.FirstOrDefault().NewsletterTemporal = "";
                        GestorDocumental.DataWrapperDocumentacion.Merge(docDW);
                    }
                }
                docCN.Dispose();
            }


            #region HistorialDocumento y Tags

            if (EditandoRecurso || EditandoFormSem)
            {
                GuardarTagsDocumento(pDocumento);
            }

            #endregion
        }

        /// <summary>
        /// Guarda los tags del documento.
        /// </summary>
        /// <param name="pDocumento">Documento en el que hay que guardar los tags</param>
        private void GuardarTagsDocumento(Documento pDocumento)
        {
            string tags = UtilCadenas.EliminarHtmlDeTexto(ExtraerTexto(mModelSaveRec.Tags).Trim());

            //Registro los tags añadidos y eliminados para agregarlos al historial.
            List<string> listaTagsTemp = UtilCadenas.SepararTexto(tags);
            List<string> listaTagAgregador = new List<string>();
            List<string> listaTagEliminados = new List<string>();
            List<string> nuevaListaTags = new List<string>();
            foreach (string tag in listaTagsTemp)
            {
                string tagLimpio = tag.Replace("http://", "").Replace("https://", "");
                if (tagLimpio.Contains("/"))
                {
                    tagLimpio = tagLimpio.Substring(0, tagLimpio.IndexOf('/'));
                }

                if (!nuevaListaTags.Contains(tagLimpio))
                {
                    nuevaListaTags.Add(tagLimpio);
                }

                if (!pDocumento.ListaTagsSoloLectura.Contains(tagLimpio))
                {
                    listaTagAgregador.Add(tagLimpio);
                }
            }
            foreach (string tag in pDocumento.ListaTagsSoloLectura)
            {
                if (!nuevaListaTags.Contains(tag))
                {
                    listaTagEliminados.Add(tag);
                }
            }
            //Añado al historial de documento el tratamiento de sus tags.
            GestorDocumental.AgregarEliminarTagsHistorial(pDocumento, listaTagEliminados, AccionHistorialDocumento.Eliminar, ProyectoSeleccionado.Clave, IdentidadActual.Clave);
            GestorDocumental.AgregarEliminarTagsHistorial(pDocumento, listaTagAgregador, AccionHistorialDocumento.Agregar, ProyectoSeleccionado.Clave, IdentidadActual.Clave);

            pDocumento.ListaTagsSoloLectura = nuevaListaTags;
            pDocumento.Tags = UtilCadenas.CadenaFormatoTexto(nuevaListaTags);
        }

        /// <summary>
        /// Comprueba si hay cambios para autoguardarlos si es así.
        /// </summary>
        /// <param name="pDocumentoARevisar">Documento temporal si lo hay o Documento actual</param>
        /// <param name="pCategoriasSeleccionadas">Lista de categorías</param>
        /// <returns>TRUE si hay cambios para autoguardarlos si es así, FALSE</returns>
        private bool HayCambiosSinGuardarParaAutoGuardado(DocumentoWeb pDocumentoARevisar, List<Guid> pCategoriasSeleccionadas)
        {
            if (pDocumentoARevisar == null)
            {//Estamos subiendo un recurso nuevo, solo hay que mirar si tiene rellenos los campo mínimos.
                if (string.IsNullOrEmpty(mModelSaveRec.Title))
                {
                    return false;
                }

                return (pCategoriasSeleccionadas.Count > 0 || !string.IsNullOrEmpty(mModelSaveRec.Description) || !string.IsNullOrEmpty(mModelSaveRec.Tags));
            }

            if (pDocumentoARevisar.Titulo != ExtraerTexto(mModelSaveRec.Title))
            {
                return true;
            }
            else if (pDocumentoARevisar.Descripcion != HttpUtility.HtmlDecode(ExtraerTexto(mModelSaveRec.Description)).Trim())
            {
                return true;
            }

            //Miro si hay diferencia en los tags:
            List<string> listaTagsActual = UtilCadenas.SepararTexto(ExtraerTexto(mModelSaveRec.Tags));

            if (listaTagsActual.Count != pDocumentoARevisar.ListaTagsSoloLectura.Count)
            {
                return true;
            }
            else
            {
                foreach (string tag in pDocumentoARevisar.ListaTagsSoloLectura)
                {
                    if (!listaTagsActual.Contains(tag))
                    {
                        return true;
                    }
                }
            }

            //Miro si hay diferencia en las categorías:
            if (pCategoriasSeleccionadas.Count != pDocumentoARevisar.Categorias.Count)
            {
                return true;
            }
            else
            {
                foreach (Guid categoriaID in pDocumentoARevisar.Categorias.Keys)
                {
                    if (!pCategoriasSeleccionadas.Contains(categoriaID))
                    {
                        return true;
                    }
                }
            }

            if (mModelSaveRec.ShareAllowed != pDocumentoARevisar.FilaDocumento.CompartirPermitido)
            {
                return true;
            }
            else if (mModelSaveRec.Protected != pDocumentoARevisar.FilaDocumento.Protegido)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Guarda el recurso en la base de datos como temporal.
        /// </summary>
        /// <param name="pCategoriasSeleccionadas">Lista de categorías</param>
        protected void GuardarRecursoAutoguardado(List<Guid> pCategoriasSeleccionadas)
        {
            string titulo = UtilCadenas.EliminarHtmlDeTexto(ExtraerTexto(mModelSaveRec.Title).Trim());
            string descripcion = HttpUtility.HtmlDecode(ExtraerTexto(mModelSaveRec.Description));
            string tags = UtilCadenas.EliminarHtmlDeTexto(ExtraerTexto(mModelSaveRec.Tags).Trim());

            if (DocumentoAutoGuardadoWiki == null)
            {
                if (Documento != null)
                {
                    mNombreDocumento = Documento.Enlace;
                }

                if (GestorDocumental.GestorIdentidades == null)
                {
                    GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
                }
                else
                {
                    GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(IdentidadActual.GestorIdentidades.DataWrapperIdentidad);
                    GestorDocumental.GestorIdentidades.RecargarHijos();
                }
                ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorIdentidades>(), mLoggerFactory);
                controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                //Creo el nuevo documento, con entidad vinculada temporal y ElementoVinculadoID el documento del que se está haciendo el autoguardado
                Documento doc = GestorDocumental.AgregarDocumento(mNombreDocumento, titulo, descripcion, tags, TiposDocumentacion.WikiTemporal, TipoEntidadVinculadaDocumento.Temporal, mDocumentoID, mModelSaveRec.ShareAllowed, true, mModelSaveRec.CreatorIsAuthor, Es.Riam.Util.UtilCadenas.CadenaFormatoTexto(Es.Riam.Util.UtilCadenas.SepararTexto(ExtraerTexto(mModelSaveRec.Authors))), false, UsuarioActual.OrganizacionID, UsuarioActual.IdentidadID);

                //Agrego la comunidad a la que pertenece el documento:
                doc.FilaDocumento.ProyectoID = mControladorBase.UsuarioActual.ProyectoID;

                List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

                foreach (Guid clave in pCategoriasSeleccionadas)
                {
                    listaCategorias.Add(GestorDocumental.GestorTesauro.ListaCategoriasTesauro[clave]);
                }

                if (IdentidadOrganizacion != null)
                {
                    GestorDocumental.AgregarDocumento(listaCategorias, doc, IdentidadOrganizacion.Clave, !(mModelSaveRec.ResourceVisibility == ResourceVisibility.CommunityMembers || mModelSaveRec.ResourceVisibility == ResourceVisibility.Open), UsuarioActual.ProyectoID);
                }
                else
                {
                    GestorDocumental.AgregarDocumento(listaCategorias, doc, mControladorBase.UsuarioActual.IdentidadID, !(mModelSaveRec.ResourceVisibility == ResourceVisibility.CommunityMembers || mModelSaveRec.ResourceVisibility == ResourceVisibility.Open), UsuarioActual.ProyectoID);
                }
                doc.FilaDocumento.TipoEntidad = (short)TipoEntidadVinculadaDocumento.Temporal;

                GestorDocumental.BorrarHistorialDocumento(doc);
            }
            else
            {
                DocumentoWeb documentoAutoguardado = DocumentoAutoGuardadoWiki;

                documentoAutoguardado.Titulo = titulo;
                documentoAutoguardado.Descripcion = descripcion;
                documentoAutoguardado.ListaTagsSoloLectura = UtilCadenas.SepararTexto(tags);
                documentoAutoguardado.Tags = tags;

                GuardarDatosCategorias(pCategoriasSeleccionadas, documentoAutoguardado);

                documentoAutoguardado.FilaDocumento.CompartirPermitido = mModelSaveRec.ShareAllowed;
            }

            Guardar_ModificarRecurso(new List<Guid>());
        }

        /// <summary>
        /// Obtiene un string con los IDs de los grupos separados por ,.
        /// </summary>
        /// <param name="pNombreCortoGrupos">Nombre corto de los grupos</param>
        /// <returns>String con los IDs de los grupos separados por &</returns>
        private string ObtenerIDsGrupoPorNombreCorto(Dictionary<string, List<string>> pNombreCortoGrupos)
        {
            StringBuilder grupos = new StringBuilder();
            IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

            foreach (string nombreorganizacion in pNombreCortoGrupos.Keys)
            {
                if (string.IsNullOrEmpty(nombreorganizacion))
                {
                    List<Guid> gruposIDs = idenCN.ObtenerGruposIDPorNombreCortoYProyecto(pNombreCortoGrupos[nombreorganizacion], ProyectoSeleccionado.Clave);

                    foreach (Guid grupoID in gruposIDs)
                    {
                        grupos.Append($"g_{grupoID},");
                    }
                }
                else
                {
                    OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                    Guid organizacionID = orgCN.ObtenerOrganizacionesIDPorNombre(nombreorganizacion);
                    orgCN.Dispose();

                    List<Guid> gruposIDs = idenCN.ObtenerGruposIDPorNombreCortoYOrganizacion(pNombreCortoGrupos[nombreorganizacion], organizacionID);

                    foreach (Guid grupoID in gruposIDs)
                    {
                        grupos.Append($"g_{grupoID},");
                    }
                }

            }

            return grupos.ToString();
        }

        /// <summary>
        /// Carga inicial común para subir un recurso nuevo o modificar uno antiguo.
        /// </summary>
        public void CargaInicial()
        {
            string strDocumentoID = RequestParams("documentoID");
            if (!string.IsNullOrEmpty(strDocumentoID))
            {
                Guid.TryParse(strDocumentoID, out mDocumentoID);
            }

            if (RequestParams("docID") != null)
            {
                if (mDocumentoID.Equals(Guid.Empty))
                {
                    mDocumentoID = new Guid(RequestParams("docID"));
                }
            }

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);

            GestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

            if (IdentidadOrganizacion == null)
            {
                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                {
                    docCN.ObtenerBaseRecursosUsuario(GestorDocumental.DataWrapperDocumentacion, mControladorBase.UsuarioActual.UsuarioID);
                }
                else
                {
                    docCL.ObtenerBaseRecursosProyecto(GestorDocumental.DataWrapperDocumentacion, ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, UsuarioActual.UsuarioID);
                }
            }
            else
            {
                docCL.ObtenerBaseRecursosOrganizacion(GestorDocumental.DataWrapperDocumentacion, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), ProyectoSeleccionado.Clave);
            }

            docCN.Dispose();
            docCL.Dispose();
        }

        /// <summary>
        /// Carga inicial del tesauro común para subir un recurso nuevo o modificar uno antiguo.
        /// </summary>
        public void CargaInicialTesauro()
        {
            //Cargamos el tesauro
            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
            if (IdentidadOrganizacion == null && !EsComunidad)
            {
                GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroUsuario(mControladorBase.UsuarioActual.UsuarioID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            }
            else if (EsComunidad)
            {
                TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);
                GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(mControladorBase.UsuarioActual.ProyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);

                GestorDocumental.GestorTesauro.TesauroDW.Merge(tesauroCL.ObtenerCategoriasPermitidasPorTipoRecurso(GestorDocumental.GestorTesauro.TesauroActualID, mControladorBase.UsuarioActual.ProyectoID));
                tesauroCL.Dispose();
            }
            else
            {
                GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroOrganizacion((Guid)UtilReflection.GetValueReflection(IdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID")), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            }
            tesauroCN.Dispose();

            if (EsComunidad)
            {
                GestorDocumental.GestorTesauro.EliminarCategoriasNoPermitidasPorTipoDoc((short)mTipoDocumento, null);
            }

            //Borramos las categorías públicas del gestor puesto que no tienen que mostrarse.
            ParametroAplicacion filaParametroAplicacion = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("UsarSoloCategoriasPrivadasEnEspacioPersonal"));
            if (filaParametroAplicacion != null && (filaParametroAplicacion.Valor.Equals("1") || filaParametroAplicacion.Valor.Equals("true")))
            {
                GestorDocumental.GestorTesauro.EliminarCategoriasPublicasSiEsMetaEspacioGNOSS(UtilIdiomas.LanguageCode);
            }
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece la identidad de la organización si la tiene.
        /// </summary>
        private Identidad IdentidadOrganizacion
        {
            get
            {
                if (mIdentidadOrganizacion == null && !string.IsNullOrEmpty(RequestParams("organizacion")))
                {
                    mIdentidadOrganizacion = IdentidadActual.IdentidadOrganizacion;
                }

                return mIdentidadOrganizacion;
            }
        }

        private Identidad IdentidadCrearVersion
        {
            get
            {
                if (mIdentidadCrearVersion == null)
                {
                    mIdentidadCrearVersion = IdentidadActual;

                    if (IdentidadOrganizacion != null)
                    {
                        mIdentidadCrearVersion = IdentidadOrganizacion;
                    }
                    else if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto && IdentidadActual.Tipo != TiposIdentidad.Personal)
                    {
                        if (IdentidadActual.IdentidadPersonalMyGNOSS != null)
                        {
                            mIdentidadCrearVersion = IdentidadActual.IdentidadPersonalMyGNOSS;
                        }
                        else
                        {
                            mIdentidadCrearVersion = IdentidadActual.IdentidadMyGNOSS;
                        }
                    }
                }
                return mIdentidadCrearVersion;
            }
        }

        /// <summary>
        /// Devuelve o establece el gestor de documentos
        /// </summary>
        private GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
            set
            {
                mGestorDocumental = value;
            }
        }

        /// <summary>
        /// Nombre corto del proyecto actual o Vacío si es MyGnoss.
        /// </summary>
        private string NombreProy
        {
            get
            {
                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                {
                    return "";
                }
                else
                {
                    return ProyectoSeleccionado.NombreCorto;
                }
            }
        }

        /// <summary>
        /// Devuelve el documento actual.
        /// </summary>
        private DocumentoWeb Documento
        {
            get
            {
                if (GestorDocumental != null && GestorDocumental.ListaDocumentos != null && GestorDocumental.ListaDocumentos.ContainsKey(mDocumentoID))
                {
                    if (!(GestorDocumental.ListaDocumentos[mDocumentoID] is DocumentoWeb))
                    {
                        return new DocumentoWeb(GestorDocumental.ListaDocumentos[mDocumentoID].FilaDocumento, GestorDocumental);
                    }
                    else
                    {
                        return (DocumentoWeb)GestorDocumental.ListaDocumentos[mDocumentoID];
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Indica si estamos en una comunidad o no.
        /// </summary>
        public bool EsComunidad
        {
            get
            {
                return (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto);
            }
        }

        /// <summary>
        /// Documento autoguardado de la wiki actual.
        /// </summary>
        private DocumentoWeb DocumentoAutoGuardadoWiki
        {
            get
            {
                return mDocumentoAutoGuardadoWiki;
            }
            set
            {
                mDocumentoAutoGuardadoWiki = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la categoría de documentación seleccionada.
        /// </summary>
        public Guid CatDocumentacionSeleccionada
        {
            get
            {
                return mCatDocumentacionSeleccionada;
            }
            set
            {
                mCatDocumentacionSeleccionada = value;
            }
        }

        /// <summary>
        /// Nombre del archivo temporal subido.
        /// </summary>
        public String NombreArchivoTemporal
        {
            get
            {
                if (string.IsNullOrEmpty(mModelSaveRec.TemporalFileName))
                {
                    return "";
                }
                else
                {
                    return mModelSaveRec.TemporalFileName.Substring(36);
                }
            }
        }

        /// <summary>
        /// Indica si se está editando el recurso
        /// </summary>
        public bool EditandoRecurso
        {
            get
            {
                return (RequestParams("editarRecurso") != null);
            }
        }
        public bool CreandoVersion
        {
            get
            {
                return RequestParams("version") != null;
            }
        }

        /// <summary>
        /// Indica si se está editando el recurso
        /// </summary>
        public bool SubidaUnificada
        {
            get
            {
                return (RequestParams("subidaUnificada") != null);
            }
        }

        /// <summary>
        /// Indica si se está editando el recurso
        /// </summary>
        public bool Duplicando
        {
            get
            {
                return (RequestParams("duplicate") != null && bool.Parse(RequestParams("duplicate")));
            }
        }

        /// <summary>
        /// Indica si se está creando un recurso semántico.
        /// </summary>
        public bool CreandoFormSem
        {
            get
            {
                return (RequestParams("crearformsem") != null);
            }
        }

        /// <summary>
        /// Indica si se está editando un recurso semántico.
        /// </summary>
        public bool EditandoFormSem
        {
            get
            {
                return (RequestParams("editarformsem") != null);
            }
        }

        /// <summary>
        /// Indica si se está haciendo una carga masiva de formularios semánticos.
        /// </summary>
        public bool CargaMasivaFormSem
        {
            get
            {
                return (RequestParams("cargamultiple") != null);
            }
        }

        /// <summary>
        /// Indica si se subiendo un documento de una plantilla virtual.
        /// </summary>
        public bool FormSemVirtual
        {
            get
            {
                return (RequestParams("virtual") != null);
            }
        }

        /// <summary>
        /// Indica si se está editando un recurso semántico.
        /// </summary>
        public bool CreandoVersionFormSem
        {
            get
            {
                return (DocumentoVersionID != Guid.Empty);
            }
        }

        /// <summary>
        /// Documento original del que se va a hacer una versión.
        /// </summary>
        public Guid DocumentoVersionID
        {
            get
            {
                if (!mDocumentoVersionID.HasValue)
                {
                    if (RequestParams("crearVersion") != null && RequestParams("crearVersion") != Guid.Empty.ToString())
                    {
                        mDocumentoVersionID = new Guid(RequestParams("crearVersion"));
                    }
                    else
                    {
                        mDocumentoVersionID = Guid.Empty;
                    }
                }

                return mDocumentoVersionID.Value;
            }
        }

        /// <summary>
        /// Indica si se está guardando un recurso de la carga masiva.
        /// </summary>
        public bool GuardandoRecursoCargaMasiva
        {
            get
            {
                return (mModelSaveRec.EditingMassiveResourceID != Guid.Empty);
            }
        }

        /// <summary>
        /// Indica si se está añadiendo a Gnoss.
        /// </summary>
        public bool AñadiendoAGnoss
        {
            get
            {
                return (RequestParams("anyadirgnoss") != null);
            }
        }

        /// <summary>
        /// Lista con los nombre de los documentos mapeados subidos a google Drive.
        /// </summary>
        private Dictionary<string, KeyValuePair<string, string>> ListaNombresDocumento
        {
            get
            {
                if (Session.Get<Dictionary<string, KeyValuePair<string, string>>>("ListaNombresDocumento") == null)
                {
                    Session.Set("ListaNombresDocumento", new Dictionary<string, KeyValuePair<string, string>>());
                }

                return Session.Get<Dictionary<string, KeyValuePair<string, string>>>("ListaNombresDocumento");
            }
            set
            {
                Session.Set("ListaNombresDocumento", value);
            }
        }

        /// <summary>
        /// Url del servicio externo al que se llama si estamos subiendo o editando un documento de una plantilla virtual.
        /// </summary>
        private string UrlServicio
        {
            get
            {
                string url = "";

                if (PropiedadesTextoOntologia.ContainsKey(PropiedadesOntologia.urlservicio.ToString()))
                {
                    url = ControladorDocumentacion.ObtenerUrlServicioExternoSEMCMS(PropiedadesTextoOntologia[PropiedadesOntologia.urlservicio.ToString()], ProyectoSeleccionado.Clave);
                }
                return url;
            }
        }

        /// <summary>
        /// Propiedades de tipo texto configuradas para la ontología actual.
        /// </summary>
        private Dictionary<string, string> PropiedadesTextoOntologia
        {
            get
            {
                if (mPropiedadesTextoOntologia == null)
                {
                    mPropiedadesTextoOntologia = UtilCadenas.ObtenerPropiedadesDeTexto(GestorDocumental.ListaDocumentos[mOntologiaID].NombreEntidadVinculada);
                }

                return mPropiedadesTextoOntologia;
            }
        }

        /// <summary>
        /// Obtiene o establece el título del recurso para el Add to Gnoss
        /// </summary>
        private string TituloAddTo
        {
            get
            {
                if (string.IsNullOrEmpty(mTituloAddTo))
                {
                    mTituloAddTo = "";

                    if (mCookieAnyadirGnoss != null && !string.IsNullOrEmpty(mCookieAnyadirGnoss["titl"]))
                    {
                        mTituloAddTo = HttpUtility.UrlDecode(mCookieAnyadirGnoss["titl"]);
                    }
                    else if (Request.Query.ContainsKey("titl"))
                    {
                        mTituloAddTo = RequestParams("titl");
                    }

                    mTituloAddTo = mTituloAddTo.Replace("\n", "").Replace("\t", "");
                }
                return mTituloAddTo;
            }
            set
            {
                mTituloAddTo = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la descripción del recurso para el Add to Gnoss
        /// </summary>
        private string DescripcionAddTo
        {
            get
            {
                if (string.IsNullOrEmpty(mDescripcionAddTo))
                {
                    mDescripcionAddTo = "";

                    if (mCookieAnyadirGnoss != null && !string.IsNullOrEmpty(mCookieAnyadirGnoss["descp"]))
                    {
                        mDescripcionAddTo = HttpUtility.UrlDecode(mCookieAnyadirGnoss["descp"]);
                    }
                    else if (Request.Query.ContainsKey("descp"))
                    {
                        mDescripcionAddTo = RequestParams("descp");
                    }

                    mDescripcionAddTo = mDescripcionAddTo.Replace("\n", "").Replace("\t", "");
                }
                return mDescripcionAddTo;
            }
            set
            {
                mDescripcionAddTo = value;
            }
        }

        /// <summary>
        /// Obtiene o establece los tags del recurso para el Add to Gnoss
        /// </summary>
        private string TagsAddTo
        {
            get
            {
                if (string.IsNullOrEmpty(mTagsAddTo))
                {
                    mTagsAddTo = "";

                    if (mCookieAnyadirGnoss != null && !string.IsNullOrEmpty(mCookieAnyadirGnoss["tags"]))
                    {
                        mTagsAddTo = HttpUtility.UrlDecode(mCookieAnyadirGnoss["tags"]);
                    }
                    else if (Request.Query.ContainsKey("tags"))
                    {
                        mTagsAddTo = RequestParams("tags");
                    }

                    mTagsAddTo = mTagsAddTo.Replace("\n", "").Replace("\t", "");
                }
                return mTagsAddTo;
            }
            set
            {
                mTagsAddTo = value;
            }
        }

        #endregion

    }
}
