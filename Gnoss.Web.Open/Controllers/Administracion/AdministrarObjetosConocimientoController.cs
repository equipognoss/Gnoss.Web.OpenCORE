using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using OntologiaAClase;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AdministrarObjetosConocimientoViewModel
    {
        public Dictionary<string, string> ListaIdiomas { get; set; }

        public string IdiomaPorDefecto { get; set; }

        public Dictionary<string, string> ListaOntologiasCom { get; set; }

        public List<ObjetoConocimientoModel> ListaObjetosConocimiento { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarObjetosConocimientoController : ControllerBaseWeb
    {
        public AdministrarObjetosConocimientoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IMassiveOntologyToClass massiveOntologyToClass)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
            mMassiveOntologyToClass = massiveOntologyToClass;
        }

        #region Miembros

        /// <summary>
        /// Modelo a devolver para la vista de administrar objetos de conocimiento.
        /// </summary>
        private AdministrarObjetosConocimientoViewModel mPaginaModel = null;

        /// <summary>
        /// Modelo que contiene los datos de las entidades secundarias
        /// </summary>
        private ComAdminSemanticElemModel mSecondaryEntityModel;

        /// <summary>
        /// Modelo utilizado para descargar las clases generadas automáticamente 
        /// </summary>
        private AdministrarPlantillasOntologicasViewModel mDescargaClasesModel = null;

        /// <summary>
        /// DataWrapper del proyecto con la configuración semántica.
        /// </summary>
        private DataWrapperProyecto mProyectoConfigSemDataWrapperProyecto;

        /// <summary>
        /// Data
        /// </summary>
        private DataWrapperProyecto mProyectoEntidadSecundariaDW;

        /// <summary>
        /// Gestor documental
        /// </summary>
        private GestorDocumental mGestorDocumental;

        /// <summary>
        /// Interfaz utilizada para generar las clases
        /// </summary>
        private IMassiveOntologyToClass mMassiveOntologyToClass;

        /// <summary>
        /// Lista con los grafos simples para autocompletar configurados en el XML.
        /// </summary>
        private List<string> mGrafosSimplesAutocompletarConfig;

        /// <summary>
        /// Diccionario con las propiedades que puede tener la ontología
        /// </summary>
        private Dictionary<string, string> mPropiedadesOntologia;

        /// <summary>
        /// Lista de documentos con imagenes
        /// </summary>
        private Dictionary<Guid, bool> mListaDocumentosConRecursos;

        /// <summary>
        /// Controlador de edición del SEMCMS.
        /// </summary>
        private SemCmsController mSemController;

        /// <summary>
        /// La ontología cuyos elementos se están editando
        /// </summary>
        private Ontologia mOntologiaEntidadSecundaria;

        #endregion

        #region Variables privadas

        /// <summary>
        /// Xml del documento
        /// </summary>
        private XmlDocument documentoXml;

        /// <summary>
        /// Identificador del proyecto actual
        /// </summary>
        private Guid proyectoID;

        /// <summary>
        /// Nombre corto del proyecto actual
        /// </summary>
        private string nombreCortoProyecto;
        #endregion

        #region Metodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Index()
        {
            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "grafo-de-conocimiento edicion edicionObjetos no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_ObjetosConocimiento_Ontologias;
            // Establecer el título para el header de DevTools                       
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "GRAFODECONOCIMIENTO");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "OBJETOSDECONOCIMIENTO/ONTOLOGIAS");

            // Establecer en el ViewBag el idioma por defecto
            ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View(PaginaModel);
        }

        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult CrearLoadModal()
        {
            // Creación objeto de conocimiento "vacío" para el mostrado del modal 
            ObjetoConocimientoModel objetoConocimiento = new ObjetoConocimientoModel();
            return GnossResultHtml("_modal-views/_objetoConocimiento-new-item", objetoConocimiento);
        }

        /// <summary>
        /// Crea la ontología a partir de los ficheros proporcionados
        /// </summary>
        /// <param name="Ontologia">Ontología a crear</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult CrearOntologia(EditOntologyViewModel Ontologia)
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

            Guid documentoID = Guid.NewGuid();
            Ontologia.Description = "Descripcion";
            Ontologia.Name = Ontologia.OWL.FileName;

            bool AgregadoCSS = false;
            bool AgregadoIMG = false;
            bool AgregadoJS = false;
            bool transaccionIniciada = false;
            string error = string.Empty;
            Documento doc = null;
            if (ValidarAgregacionEdicionOnto(Ontologia, true))
            {
                try
                {
                    ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                    string rutaFichero = GuardarArchivos(Ontologia, documentoID, ref AgregadoCSS, ref AgregadoIMG, ref AgregadoJS, ref error);

                    if (!string.IsNullOrEmpty(error))
                    {
                        return GnossResultERROR(error);
                    }

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    if (rutaFichero != null && docCN.ExisteOtraOntologiaEnProyecto(ProyectoSeleccionado.Clave, rutaFichero, documentoID))
                    {
                        docCN.Dispose();
                        return GnossResultERROR(UtilIdiomas.GetText("COMADMIN", "ERRORONTOUSADA", rutaFichero));
                    }
                    docCN.Dispose();

                    string nombreOnto = Ontologia.Name.Trim();
                    string descripcionOnto = HttpUtility.UrlDecode(Ontologia.Description);

                    if (rutaFichero != null)
                    {
                        GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
                        controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                        TiposDocumentacion tipoDoc = TiposDocumentacion.Ontologia;

                        if (!Ontologia.Principal)
                        {
                            tipoDoc = TiposDocumentacion.OntologiaSecundaria;

                            if (!Ontologia.NoUseXML)//Hemos subido XML a la entidad secundaria, generico o no.
                            {
                                ProyectoConfigExtraSem filaConfigSem = new ProyectoConfigExtraSem();
                                filaConfigSem.ProyectoID = ProyectoSeleccionado.Clave;
                                filaConfigSem.UrlOntologia = rutaFichero;
                                filaConfigSem.SourceTesSem = documentoID.ToString();
                                filaConfigSem.Tipo = (short)TipoConfigExtraSemantica.EntidadSecundaria;
                                filaConfigSem.Nombre = nombreOnto;
                                filaConfigSem.Editable = true;
                                ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Add(filaConfigSem);
                                if (!mEntityContext.ProyectoConfigExtraSem.Any(proy => proy.ProyectoID.Equals(filaConfigSem.ProyectoID) && proy.UrlOntologia.Equals(filaConfigSem.UrlOntologia) && proy.SourceTesSem.Equals(filaConfigSem.SourceTesSem)))
                                {
                                    mEntityContext.ProyectoConfigExtraSem.Add(filaConfigSem);
                                }
                            }
                        }

                        if (mGrafosSimplesAutocompletarConfig != null)
                        {
                            foreach (string grafoSimple in mGrafosSimplesAutocompletarConfig)
                            {
                                ProyectoConfigExtraSem filaConfigSem = new ProyectoConfigExtraSem();
                                filaConfigSem.ProyectoID = ProyectoSeleccionado.Clave;
                                filaConfigSem.UrlOntologia = rutaFichero;
                                filaConfigSem.SourceTesSem = grafoSimple;
                                filaConfigSem.Tipo = (short)TipoConfigExtraSemantica.GrafoSimple;
                                filaConfigSem.Nombre = nombreOnto;
                                filaConfigSem.Editable = true;
                                ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Add(filaConfigSem);
                                if (!mEntityContext.ProyectoConfigExtraSem.Any(proy => proy.ProyectoID.Equals(filaConfigSem.ProyectoID) && proy.UrlOntologia.Equals(filaConfigSem.UrlOntologia) && proy.SourceTesSem.Equals(filaConfigSem.SourceTesSem)))
                                {
                                    mEntityContext.ProyectoConfigExtraSem.Add(filaConfigSem);
                                }
                            }
                        }

                        doc = GestorDocumental.AgregarDocumento(rutaFichero, nombreOnto, descripcionOnto, null, tipoDoc, TipoEntidadVinculadaDocumento.Web, Guid.Empty, true, false, false, null, false, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.IdentidadID);

                        //Cambiar identificador del documento
                        if (GestorDocumental.ListaDocumentos.ContainsKey(doc.Clave))
                        {
                            GestorDocumental.ListaDocumentos.Remove(doc.Clave);
                            GestorDocumental.ListaDocumentos.Add(documentoID, doc);
                        }
                        else
                        {
                            GestorDocumental.ListaDocumentos.Add(documentoID, doc);
                        }
                        doc.FilaDocumento.DocumentoID = documentoID;

                        //Agrego la comunidad a la que pertenece el documento:
                        doc.FilaDocumento.ProyectoID = mControladorBase.UsuarioActual.ProyectoID;

                        //Pongo la ultima versión del documento a false para que no aparezca en las listas:
                        doc.FilaDocumento.UltimaVersion = false;

                        GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
                        controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                        ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        GestorDocumental.VincularDocumentoABaseRecursos(doc, proyectoCN.ObtenerBaseRecursosProyectoPorProyectoID(mControladorBase.UsuarioActual.ProyectoID), TipoPublicacion.Compartido, mControladorBase.UsuarioActual.IdentidadID, false);

                        if (AgregadoCSS || AgregadoIMG || AgregadoJS)
                        {
                            if (AgregadoIMG)
                            {
                                doc.FilaDocumento.NombreCategoriaDoc = $"240,{UtilArchivos.ContentOntologias}/Archivos/{documentoID.ToString().Substring(0, 3)}/{documentoID}.jpg";
                            }
                            doc.FilaDocumento.VersionFotoDocumento = 1;
                        }

                        if (mPropiedadesOntologia != null)
                        {
                            foreach (string propiedad in mPropiedadesOntologia.Keys)
                            {
                                doc.FilaDocumento.NombreElementoVinculado += $"{propiedad}={mPropiedadesOntologia[propiedad]}|||";
                            }
                        }
                        Ontologia.Protected = doc.FilaDocumento.Protegido;
                        Ontologia.OntologyProperties = doc.FilaDocumento.NombreElementoVinculado;

                        try
                        {
                            mEntityContext.NoConfirmarTransacciones = true;
                            transaccionIniciada = proyAD.IniciarTransaccion(true);
                            GuardarCambios();

                            if (iniciado)
                            {
                                HttpResponseMessage resultado = AdministrarIntegracionContinua(Ontologia, documentoID);

                                if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                                {
                                    throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                                }
                            }

                            if (transaccionIniciada)
                            {
                                mEntityContext.TerminarTransaccionesPendientes(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (transaccionIniciada)
                            {
                                proyAD.TerminarTransaccion(false);
                            }
                            GuardarLogError(ex.ToString());
                            return GnossResultERROR(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex.ToString());
                    return GnossResultERROR(ex.Message);
                }
            }

            ObjetoConocimientoModel objetoConocimientoModel = null;

            ControladorObjetosConocimiento contrObjetosConocim = new ControladorObjetosConocimiento(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            if (Ontologia.Principal)
            {
                objetoConocimientoModel = contrObjetosConocim.NuevoObjetoConocimento(Ontologia.Name);
                objetoConocimientoModel.DocumentoID = documentoID;
                objetoConocimientoModel.EsObjetoPrimario = true;
                contrObjetosConocim.AgregarPermisosAdministradorOntologia(documentoID);
            }
            else
            {
                objetoConocimientoModel = new ObjetoConocimientoModel();
                objetoConocimientoModel.DocumentoID = documentoID;
                objetoConocimientoModel.EsObjetoPrimario = false;
                objetoConocimientoModel.Ontologia = Ontologia.Name.Replace(".owl", string.Empty);
                objetoConocimientoModel.Name = objetoConocimientoModel.Ontologia;
                objetoConocimientoModel.GrafoActual = Ontologia.Name;
            }           

            return PartialView("_EdicionObjetoConocimiento", objetoConocimientoModel);
        }

        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult DownloadVersion(Guid? ontologyID, string copyName, string ontologyName)
        {
            if (!string.IsNullOrEmpty(copyName) && ontologyID.HasValue)
            {
                try
                {
                    return DescargarArchivo(ontologyID.Value, copyName, ontologyName);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    return Content("ERROR: Ha habido un error al obtener el historial");
                }
            }

            return Content("Parámetros de descarga incorrectos");
        }

        /// <summary>
        /// Guarda los cambios realizados al editar el objeto de conocimiento
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult GuardarEdicionObjetoConocimiento(EditarObjetoConocimientoYOntologiaModel ObjetoConocimiento)
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

            ControladorObjetosConocimiento contrObjetosConocim = new ControladorObjetosConocimiento(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            try
            {
                // Guardar cambios en Ontologia
                if (ObjetoConocimiento.OntologiaModificada)
                {
                    GuardarEdicionOntologia(ObjetoConocimiento.Ontologia, iniciado);
                }
                //Guardar cambios en Objeto de conocimiento
                if (ObjetoConocimiento.ObjetoConocimientoModificado)
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);
                    contrObjetosConocim.GuardarObjetoConocimiento(ObjetoConocimiento.ObjetoConocimiento);

                    if (iniciado)
                    {
                        HttpResponseMessage resultado = InformarCambioAdministracion("ObjetosConocimiento", JsonConvert.SerializeObject(ObjetoConocimiento.ObjetoConocimiento, Newtonsoft.Json.Formatting.Indented));

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }
                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }

            }
            catch (Exception ex)
            {
                if (transaccionIniciada)
                {
                    proyAD.TerminarTransaccion(false);
                }

                return GnossResultERROR(ex.Message);
            }

            contrObjetosConocim.InvalidarCaches(UrlIntragnoss);

            return GnossResultOK();
        }

        /// <summary>
        /// Método para eliminar un objeto de conocimiento. Se pasará el guid y la ontología para permitir el borrado correcto.
        /// </summary>
        /// <param name="DocumentoId">Guid de la ontología/oc del que se desea eliminar</param>
        /// <param name="Id">Id del objeto de conocimiento del que se desea proceder al borrado</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult EliminarObjetoConocimiento(Guid DocumentoId, string Id)
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

            ControladorObjetosConocimiento contrObjetosConocim = new ControladorObjetosConocimiento(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            string nombreOntologia = $"{Id}.owl";
            bool esPrincipal = false;
            ObjetoConocimientoModel objetoConocimiento = contrObjetosConocim.CargarObjetoConocimiento(nombreOntologia);
            
            if(objetoConocimiento != null)
            {
                objetoConocimiento.Deleted = true;
                esPrincipal = true;
            }
            
            EditOntologyViewModel ontologyBorrar = contrObjetosConocim.EliminarObjetoConocimientoOntologia(DocumentoId, Id, iniciado, esPrincipal);

            if (iniciado)
            {
                HttpResponseMessage resultado = InformarCambioAdministracion("Ontologias", JsonConvert.SerializeObject(ontologyBorrar, Newtonsoft.Json.Formatting.Indented));

                if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                {
                    throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                }

                resultado = InformarCambioAdministracion("ObjetosConocimiento", JsonConvert.SerializeObject(objetoConocimiento, Newtonsoft.Json.Formatting.Indented));
                if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                {
                    throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                }
            }

            return GnossResultOK();
        }

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult DownloadClasses()
        {
            Dictionary<string, string> dicPref = new Dictionary<string, string>();
            Dictionary<string, KeyValuePair<Ontologia, byte[]>> diccionarioOntologias = new Dictionary<string, KeyValuePair<Ontologia, byte[]>>();
            List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();
            foreach (var ontologiaPrimaria in DescargaClasesModel.Templates)
            {
                if (!ontologiaPrimaria.OntologyName.Equals("dbpedia"))
                {
                    byte[] bytesOntologias = ObtenerOntologia(ontologiaPrimaria.OntologyID);
                    byte[] bytesXmlOntologia = ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);
                    Ontologia ontologia = new Ontologia(bytesOntologias);
                    ontologia.LeerOntologia();

                    foreach (string key in ontologia.NamespacesDefinidos.Keys)
                    {
                        if (!dicPref.ContainsKey(key))
                        {
                            dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
                        }
                    }
                    diccionarioOntologias.Add(ontologiaPrimaria.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
                }
            }
            foreach (var secondaryOntology in DescargaClasesModel.SecondaryTemplates)
            {
                byte[] bytesOntologias = ObtenerOntologia(secondaryOntology.OntologyID);
                byte[] bytesXmlOntologia = ObtenerXmlOntologia(secondaryOntology.OntologyID);
                Ontologia ontologia = new Ontologia(bytesOntologias);
                ontologia.LeerOntologia();
                foreach (string key in ontologia.NamespacesDefinidos.Keys)
                {
                    if (!dicPref.ContainsKey(key))
                    {
                        dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
                    }
                }
                diccionarioOntologias.Add(secondaryOntology.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
            }

            try
            {
                string directorio = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(directorio);
                List<string> nombresOntologias = DescargaClasesModel.Templates.Select(p => p.OntologyName).Union(DescargaClasesModel.SecondaryTemplates.Select(p => p.OntologyName)).ToList();
                GenerarClaseYVista claseYvista = new GenerarClaseYVista(directorio, ProyectoSeleccionado.NombreCorto, ProyectoSeleccionado.Clave, nombresOntologias, dicPref, diccionarioOntologias, false, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mMassiveOntologyToClass);
                claseYvista.CrearObjetos(diccionarioOntologias);
                claseYvista.CrearRelaciones();

                foreach (var ontologiaPrincipal in DescargaClasesModel.Templates)
                {
                    try
                    {
                        byte[] bytesOntologia = ObtenerOntologia(ontologiaPrincipal.OntologyID);
                        byte[] bytesXmlOntologia = ObtenerXmlOntologia(ontologiaPrincipal.OntologyID);

                        Ontologia ontologia = new Ontologia(bytesOntologia);
                        OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaPrincipal.OntologyName, ontologia, bytesXmlOntologia, true, listaIdiomas, directorio);
                        claseYvista.CrearClases(contenedorOntologia);
                        claseYvista.CrearVistas(contenedorOntologia, ProyectoSeleccionado.NombreCorto);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"En la ontologia {UtilCadenas.ObtenerTextoDeIdioma(ontologiaPrincipal.Name, IdiomaUsuario, IdiomaPorDefecto)}, {ex.Message}", ex);
                    }
                }
                foreach (var ontologiaSecundaria in DescargaClasesModel.SecondaryTemplates)
                {
                    byte[] bytesOntologia = ObtenerOntologia(ontologiaSecundaria.OntologyID);
                    byte[] bytesXmlOntologia = null;
                    if (ontologiaSecundaria.HasXmlFile)
                    {
                        bytesXmlOntologia = ObtenerXmlOntologia(ontologiaSecundaria.OntologyID);
                    }

                    Ontologia ontologia = new Ontologia(bytesOntologia);
                    OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaSecundaria.OntologyName, ontologia, bytesXmlOntologia, false, listaIdiomas, directorio);

                    claseYvista.CrearClases(contenedorOntologia);
                }
                claseYvista.GenerarPaqueteCSPROJ(ProyectoSeleccionado.NombreCorto);

                //Generar clases
                //Comprimirlo
                DirectoryInfo directoryPrincipal = new DirectoryInfo(directorio);
                DirectoryInfo[] directories = directoryPrincipal.GetDirectories();

                string nombrefichero = string.Empty;
                foreach (DirectoryInfo dir in directories)
                {
                    if (dir.Name.Contains("ClasesYVistas_"))
                    {
                        nombrefichero = dir.Name;
                    }
                }

                string pZipPath = Path.Combine(directorio, "comprimido.zip");
                string folderPath = Path.Combine(directorio, nombrefichero);
                ZipFile.CreateFromDirectory(folderPath, pZipPath);

                byte[] bytes = System.IO.File.ReadAllBytes(pZipPath);
                Thread.Sleep(1000);

                try
                {
                    Directory.Delete(directorio, true);
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        Directory.Delete(directorio, true);
                    }
                    catch
                    {
                        GuardarLogError("Fallo al intentar borrra el fichero temporar de la carpeta: " + directorio);
                    }
                }
                return File(bytes, "application/zip", $"{ProyectoSeleccionado.NombreCorto}_ClassesAndViews.zip");
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex.Message);
                Response.StatusCode = 500;
                return GnossResultERROR(ex.Message);
            }
        }

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult DownloadClassesJava()
        {
            Dictionary<string, string> dicPref = new Dictionary<string, string>();
            Dictionary<string, KeyValuePair<Ontologia, byte[]>> diccionarioOntologias = new Dictionary<string, KeyValuePair<Ontologia, byte[]>>();
            List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();
            foreach (var ontologiaPrimaria in DescargaClasesModel.Templates)
            {
                if (!ontologiaPrimaria.OntologyName.Equals("dbpedia"))
                {
                    byte[] bytesOntologias = ObtenerOntologia(ontologiaPrimaria.OntologyID);
                    byte[] bytesXmlOntologia = ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);
                    Ontologia ontologia = new Ontologia(bytesOntologias);
                    ontologia.LeerOntologia();

                    foreach (string key in ontologia.NamespacesDefinidos.Keys)
                    {
                        if (!dicPref.ContainsKey(key))
                        {
                            dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
                        }
                    }

                    diccionarioOntologias.Add(ontologiaPrimaria.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
                }
            }
            foreach (var secondaryOntology in DescargaClasesModel.SecondaryTemplates)
            {
                byte[] bytesOntologias = ObtenerOntologia(secondaryOntology.OntologyID);
                byte[] bytesXmlOntologia = ObtenerXmlOntologia(secondaryOntology.OntologyID);
                Ontologia ontologia = new Ontologia(bytesOntologias);
                ontologia.LeerOntologia();
                foreach (string key in ontologia.NamespacesDefinidos.Keys)
                {
                    if (!dicPref.ContainsKey(key))
                    {
                        dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
                    }
                }

                diccionarioOntologias.Add(secondaryOntology.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
            }

            try
            {
                string directorio = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "ClasesYVistas");
                Directory.CreateDirectory(directorio);
                List<string> nombresOntologias = DescargaClasesModel.Templates.Select(p => p.OntologyName).Union(DescargaClasesModel.SecondaryTemplates.Select(p => p.OntologyName)).ToList();

                GenerarClaseYVista claseYvista = new GenerarClaseYVista(directorio, ProyectoSeleccionado.NombreCorto, ProyectoSeleccionado.Clave, nombresOntologias, dicPref, diccionarioOntologias, true, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mMassiveOntologyToClass);
                claseYvista.CrearObjetos(diccionarioOntologias);
                claseYvista.CrearRelaciones();

                string ruta = Path.Combine(directorio, "src", "main", "java");
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                foreach (var ontologiaPrimaria in DescargaClasesModel.Templates)
                {
                    try
                    {
                        byte[] bytesOntologia = ObtenerOntologia(ontologiaPrimaria.OntologyID);
                        byte[] bytesXmlOntologia = ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);

                        Ontologia ontologia = new Ontologia(bytesOntologia);
                        OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaPrimaria.OntologyName, ontologia, bytesXmlOntologia, true, listaIdiomas, ruta);

                        claseYvista.CrearClasesJava(contenedorOntologia);
                        claseYvista.CrearVistas(contenedorOntologia, ProyectoSeleccionado.NombreCorto);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, $"error en la ontología {ontologiaPrimaria.OntologyName}");
                        throw new Exception(ex.Message);
                    }
                }
                foreach (var ontologiaSecundaria in DescargaClasesModel.SecondaryTemplates)
                {
                    byte[] bytesOntologia = ObtenerOntologia(ontologiaSecundaria.OntologyID);
                    byte[] bytesXmlOntologia = null;
                    if (ontologiaSecundaria.HasXmlFile)
                    {
                        bytesXmlOntologia = ObtenerXmlOntologia(ontologiaSecundaria.OntologyID);
                    }

                    Ontologia ontologia = new Ontologia(bytesOntologia);
                    OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaSecundaria.OntologyName, ontologia, bytesXmlOntologia, false, listaIdiomas, directorio);
                    claseYvista.CrearClasesJava(contenedorOntologia);
                }

                CrearPomXml(directorio, ProyectoSeleccionado.NombreCorto);

                DirectoryInfo directoryPrincipal = new DirectoryInfo(directorio);
                DirectoryInfo[] directories = directoryPrincipal.GetDirectories();

                string pZipPath = Path.Combine(directorio.Replace("ClasesYVistas", string.Empty), "comprimido.zip");
                ZipFile.CreateFromDirectory(directorio, pZipPath);

                byte[] bytes = System.IO.File.ReadAllBytes(pZipPath);
                Thread.Sleep(1000);

                try
                {
                    Directory.Delete(directorio, true);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        Directory.Delete(directorio, true);
                    }
                    catch
                    {
                        GuardarLogError($"Fallo al intentar borrra el fichero temporar de la carpeta: {directorio}");
                    }
                }

                return File(bytes, "application/zip", $"{ProyectoSeleccionado.NombreCorto}_ClassesAndViews.zip");
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                return GnossResultERROR("Error al generar las clases");
            }
        }

        /// <summary>
        /// Nuevo Contexto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevoSubTipo()
        {
            EliminarPersonalizacionVistas();

            return PartialView("_FichaSubTipo", new KeyValuePair<string, string>(string.Empty, "Nuevo Sub-Tipo"));
        }

        /// <summary>
        /// Nuevo Contexto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaPropiedad()
        {
            EliminarPersonalizacionVistas();

            ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();
            propiedad.Propiedad = "Nueva propiedad";

            return PartialView("_FichaPropiedad", propiedad);
        }

        /// <summary>
        /// Nuevo Contexto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaPropiedadPersonalizada()
        {
            EliminarPersonalizacionVistas();
            ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel propiedad = new ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel();
            propiedad.Identificador = "Identificador";
            propiedad.Select = "select distinct ?s ?rdftype";
            propiedad.Where = "where\n{\n  ?s a ?rdftype.\n  <FILTER_RESOURCE_IDS>\n}";
            return PartialView("_FichaPropiedadPersonalizado", propiedad);
        }

        /// <summary>
        /// Carga todos los elementos pertenecientes a la entidad secundaria indicada
        /// </summary>
        /// <param name="Grafo">Grafo de la entidad secundaria indicada</param>
        /// <returns>Devuelve vista con todos los elementos pertenecientes a la entidad secundaria indicada<returns>
        public ActionResult CargarElementosEntidadSecundaria(string Grafo)
        {
            EliminarPersonalizacionVistas();

            CargarInicial_EntSecund();
            CargarInstanciasOntologiaSecundaria(Grafo);

            return PartialView("_partial-views/_ontology-secondary-list", mSecondaryEntityModel);
        }

        /// <summary>
        /// Devuelve la vista con las propiedades correspondientes de los elementos de la ontología a la cual estamos añadiendo uno nuevo
        /// </summary>
        /// <param name="Grafo">Grafo del elemento a cargar</param>
        /// <returns>Vista con las propiedades del tipo de la entidad secundaria vacías</returns>
        public ActionResult NuevoElementoEntidadSecundaria(string Grafo)
        {
            EliminarPersonalizacionVistas();

            CargarInicial_EntSecund();
            CrearEditarIntanciaOntologiaSecundaria(Grafo, true);

            // return PartialView("_partial-views/_ontology-secondary-list-detail-item", mSecondaryEntityModel); *@
            // Indicar que el item se va a editar y no es nuevo
            ViewBag.isNewCreation = "true";
            return PartialView("_partial-views/_ontology-secondary-list-item", mSecondaryEntityModel);            
        }

        /// <summary>
        /// Devuelve la vista con las propiedades del elemento y sus valores para poder editarlo
        /// </summary>
        /// <param name="Grafo">Grafo del elemento a mostrar</param>
        /// <param name="SujetoEntidad">Sujeto del elemento a mostrar</param>
        /// <returns>Vista con las propiedades del elemento de la entidad secundaria y sus datos</returns>
        public ActionResult EditarElementoEntidadSecundaria(string Grafo, string SujetoEntidad)
        {
            EliminarPersonalizacionVistas();

            CargarInicial_EntSecund();
            CrearEditarIntanciaOntologiaSecundaria(Grafo, false, SujetoEntidad);
            
            // Indicar que el item se va a editar y no es nuevo            
            ViewBag.isNewCreation = "false";
            return PartialView("_partial-views/_ontology-secondary-list-detail-item", mSecondaryEntityModel);
        }

        /// <summary>
        /// Aplica los cambios sobre el elemento añadido o modificado
        /// </summary>
        /// <param name="SujetoEntidad">Sujeto de la entidad a modificar</param>
        /// <param name="Grafo">Grafo al que pertenece la entidad</param>
        /// <param name="ElementoNuevo">Si es un elemento nuevo o uno modificado</param>
        /// <param name="Rdf">Valor del rdf del elemento</param>
        /// <returns></returns>
        public ActionResult GuardarElementoEntidadSecundaria(string SujetoEntidad, string Grafo, bool ElementoNuevo, string Rdf)
        {
            string tildes = "[áéíóúöüÁÉÍÓÚÖÜ]";
            if (!string.IsNullOrEmpty(SujetoEntidad))
            {
                bool tieneTildes = Regex.IsMatch(SujetoEntidad, tildes);
                if (tieneTildes)
                {
                    return GnossResultERROR("No se pueden poner acentos");
                }
            }

            return GuardarIntanciaOntologiaSecundaria(SujetoEntidad, Grafo, ElementoNuevo, Rdf);
        }

        public ActionResult EliminarElementoEntidadSecundaria(string Grafo, string SujetoEntidad)
        {
            EliminarPersonalizacionVistas();

            CargarInicial_EntSecund();
            try
            {
                EliminarIntanciasOntologiaSecundaria(Grafo, SujetoEntidad);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(ex.Message))
                {
                    return GnossResultERROR("Ha ocurrido un error al eliminar el elemento");
                }
                else
                {
                    return GnossResultERROR(ex.Message);
                }                
            }

            return GnossResultOK("El elemento se ha borrado correctamente");
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult HistorialFicheros(Guid ontoID)
        {
            OntologicalTemplatesAdministrationViewModel viewModel = new OntologicalTemplatesAdministrationViewModel() { OntologyID = ontoID };

            try
            {
                List<string> listadoCopias = CargarHistorial(ontoID);

                if (listadoCopias == null || listadoCopias.Count == 0)
                {
                    return GnossResultERROR("No hay versiones de esta ontologia");
                }

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                viewModel.OntologyName = docCN.ObtenerEnlaceDocumentoPorDocumentoID(ontoID);
                if (viewModel.OntologyName.Contains("."))
                {
                    viewModel.OntologyName = viewModel.OntologyName.Substring(0, viewModel.OntologyName.LastIndexOf("."));
                }
                docCN.Dispose();

                viewModel.VersionList = listadoCopias;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                return GnossResultERROR("Ha habido un error al obtener el historial");
            }

            return PartialView("_partial-views/_ontology-history-file-items", viewModel);
        }

        #endregion

        #region Métodos privados

        /// <summary>
        /// Guarda una instancia secundaría.
        /// </summary>
        /// <returns>Acción resultado</returns>
        private ActionResult GuardarIntanciaOntologiaSecundaria(string pSujeto, string pGrafo, bool pElementoNuevo, string pRdf)
        {
            CrearEditarIntanciaOntologiaSecundaria(pGrafo, pElementoNuevo, pSujeto);
            List<ElementoOntologia> entidadesGuardar = mSemController.RecogerValoresRdf(HttpUtility.HtmlDecode(pRdf), null);

            if (string.IsNullOrEmpty(pSujeto))
            {
                return GnossResultERROR("El Sujeto no puede estar vacío.");
            }

            if (pElementoNuevo)
            {
                entidadesGuardar[0].ID = pSujeto;
            }

            Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(pGrafo, ProyectoSeleccionado.Clave);

            try
            {
                ControladorDocumentacion.GuardarRDFEntidadSecundaria(entidadesGuardar, UrlIntragnoss, pGrafo, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, docOnto, !pElementoNuevo);
            }
            catch (Exception ex)
            {
                return GnossResultERROR(ex.Message);
            }

            CargarInstanciasOntologiaSecundaria(pGrafo);

            if (!mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables.ContainsKey(entidadesGuardar[0].Uri))
            {
                mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables.Add(entidadesGuardar[0].Uri, entidadesGuardar[0].Uri);
            }

            if (!string.IsNullOrEmpty(mOntologiaEntidadSecundaria.ConfiguracionPlantilla.PropiedadTitulo.Key))
            {
                Propiedad propRep = entidadesGuardar[0].ObtenerPropiedad(mOntologiaEntidadSecundaria.ConfiguracionPlantilla.PropiedadTitulo.Key);

                if (propRep != null)
                {
                    string repres = propRep.ObtenerPrimerValorDeIdiomaOSinEl(UtilIdiomas.LanguageCode);
                    mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables[entidadesGuardar[0].Uri] = repres;
                }
            }

            mSecondaryEntityModel.SecondaryEntities.SemanticResourceModel = null;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();

            // Pasar en el ViewBag el sujeto necesario para la vista 
            ViewBag.SujetoEnt = mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables.Keys;                        
            return PartialView("_partial-views/_ontology-secondary-list", mSecondaryEntityModel);
        }

        private bool GuardarEdicionOntologia(EditOntologyViewModel Ontologia, bool pIniciado)
        {
            Guid documentoID = Ontologia.OntologyID;
            Ontologia.Description = HttpUtility.UrlDecode(Ontologia.Description);
            Ontologia.Principal = GestorDocumental.ListaDocumentos[documentoID].TipoDocumentacion.Equals(TiposDocumentacion.Ontologia);

            bool AgregadoCSS = false;
            bool AgregadoIMG = false;
            bool AgregadoJS = false;
            string error = string.Empty;

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            if (ValidarAgregacionEdicionOnto(Ontologia, false))
            {
                try
                {
                    ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                    string rutaFichero = GuardarArchivos(Ontologia, documentoID, ref AgregadoCSS, ref AgregadoIMG, ref AgregadoJS, ref error);

                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception(error);
                    }

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                    if (rutaFichero != null && docCN.ExisteOtraOntologiaEnProyecto(ProyectoSeleccionado.Clave, rutaFichero, documentoID))
                    {
                        docCN.Dispose();
                        return false;
                    }

                    docCN.Dispose();

                    string nombreOnto = Ontologia.Name.Trim();
                    string descripcionOnto = HttpUtility.UrlDecode(Ontologia.Description);

                    GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Titulo = nombreOnto;
                    GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Descripcion = descripcionOnto;

                    string rutaActualizarDocsOnto = null;

                    if (Ontologia.EditIMG)
                    {
                        if (AgregadoIMG)
                        {
                            GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc = $"240,{UtilArchivos.ContentOntologias}/Archivos/{documentoID.ToString().Substring(0, 3)}/{documentoID}.jpg";
                            rutaActualizarDocsOnto = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc;

                            if (!GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento.HasValue)
                            {
                                GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento = 1;
                            }
                            else
                            {
                                GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento = Math.Abs(GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento.Value) + 1;
                            }
                        }
                        else if (GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc != null)
                        {
                            rutaActualizarDocsOnto = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc;
                            GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc = null;
                        }
                    }

                    if ((AgregadoCSS || AgregadoJS) && !AgregadoIMG)
                    {
                        if (!GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento.HasValue)
                        {
                            GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento = 1;
                        }
                        else
                        {
                            GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento = Math.Abs(GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento.Value) + 1;
                        }
                    }

                    if (Ontologia.Principal || (!Ontologia.Principal && Ontologia.EditXML))
                    {
                        List<ProyectoConfigExtraSem> listaProyectoConfigExtraSemBorrar2 = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace) && proy.Tipo.Equals((short)TipoConfigExtraSemantica.EntidadSecundaria)).ToList();
                        foreach (ProyectoConfigExtraSem fila in listaProyectoConfigExtraSemBorrar2)
                        {
                            mEntityContext.EliminarElemento(fila);

                            ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Remove(fila);
                        }
                    }

                    if (!Ontologia.Principal && Ontologia.EditXML) //Se ha pulsado el botón para interacturar con el archivo de configuración.
                    {
                        if (!Ontologia.NoUseXML)//Hemos subido XML a la entidad secundaria, generico o no.
                        {
                            ProyectoConfigExtraSem filaConfigSem = new ProyectoConfigExtraSem();
                            filaConfigSem.ProyectoID = ProyectoSeleccionado.Clave;
                            filaConfigSem.UrlOntologia = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace;
                            filaConfigSem.SourceTesSem = documentoID.ToString();
                            filaConfigSem.Tipo = (short)TipoConfigExtraSemantica.EntidadSecundaria;
                            filaConfigSem.Nombre = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Titulo;
                            filaConfigSem.Editable = true;
                            ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Add(filaConfigSem);
                            mEntityContext.ProyectoConfigExtraSem.Add(filaConfigSem);
                        }
                    }
                    List<ProyectoConfigExtraSem> listaProyectoConfigExtraSemBorrar = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace) && proy.Tipo.Equals((short)TipoConfigExtraSemantica.GrafoSimple)).ToList();
                    foreach (ProyectoConfigExtraSem fila in listaProyectoConfigExtraSemBorrar)
                    {
                        mEntityContext.EliminarElemento(fila);
                        ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Remove(fila);
                    }

                    if (mGrafosSimplesAutocompletarConfig != null)
                    {
                        foreach (string grafoSimple in mGrafosSimplesAutocompletarConfig)
                        {
                            ProyectoConfigExtraSem filaConfigSem = new ProyectoConfigExtraSem();
                            filaConfigSem.ProyectoID = ProyectoSeleccionado.Clave;
                            filaConfigSem.UrlOntologia = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace;
                            filaConfigSem.SourceTesSem = grafoSimple;
                            filaConfigSem.Tipo = (short)TipoConfigExtraSemantica.GrafoSimple;
                            filaConfigSem.Nombre = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Titulo;
                            filaConfigSem.Editable = true;
                            ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Add(filaConfigSem);
                            if (!(mEntityContext.ProyectoConfigExtraSem.Any(proy => proy.ProyectoID.Equals(filaConfigSem.ProyectoID) && proy.UrlOntologia.Equals(filaConfigSem.UrlOntologia) && proy.SourceTesSem.Equals(filaConfigSem.SourceTesSem))))
                            {
                                mEntityContext.ProyectoConfigExtraSem.Add(filaConfigSem);
                            }
                        }
                    }

                    if (mPropiedadesOntologia != null)
                    {
                        List<string> listaPropsOnto = null;
                        Dictionary<string, string> dicPropsOnto = new Dictionary<string, string>();
                        string propsOnto = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreElementoVinculado;

                        if (!string.IsNullOrEmpty(propsOnto))
                        {
                            listaPropsOnto = new List<string>(propsOnto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries));
                            foreach (string prop in listaPropsOnto)
                            {
                                if (prop.Contains("="))
                                {
                                    string[] valores = prop.Split(new char[] { '=' });
                                    dicPropsOnto.Add(valores[0], valores[1]);
                                }
                            }

                            //quitamos las propiedades de la Ontología que ya no están configuradas
                            //mPropiedadesOntologia contiene las configuradas en el Xml y dicPropsOnto son las obtenidas en BD
                            if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.urlservicio.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.urlservicio.ToString()))
                            {
                                dicPropsOnto.Remove(PropiedadesOntologia.urlservicio.ToString());
                            }
                            if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.urlserviciocomplementario.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.urlserviciocomplementario.ToString()))
                            {
                                dicPropsOnto.Remove(PropiedadesOntologia.urlserviciocomplementario.ToString());
                            }
                            if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString()))
                            {
                                dicPropsOnto.Remove(PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString());
                            }
                            if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.urlservicioElim.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.urlservicioElim.ToString()))
                            {
                                dicPropsOnto.Remove(PropiedadesOntologia.urlservicioElim.ToString());
                            }
                            if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.enviarRdfAntiguo.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.enviarRdfAntiguo.ToString()))
                            {
                                dicPropsOnto.Remove(PropiedadesOntologia.enviarRdfAntiguo.ToString());
                            }
                        }

                        //Guardo las propiedades configuradas en el Xml
                        foreach (string propiedad in mPropiedadesOntologia.Keys)
                        {
                            if (!dicPropsOnto.ContainsKey(propiedad))
                            {
                                dicPropsOnto.Add(propiedad, mPropiedadesOntologia[propiedad]);
                            }
                            else
                            {
                                dicPropsOnto[propiedad] = mPropiedadesOntologia[propiedad];
                            }
                        }

                        GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreElementoVinculado = string.Empty;
                        foreach (string propiedadOnto in dicPropsOnto.Keys)
                        {
                            GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreElementoVinculado += $"{propiedadOnto}={dicPropsOnto[propiedadOnto]}|||";
                        }
                    }

                    Ontologia.Protected = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Protegido;
                    Ontologia.OntologyProperties = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreElementoVinculado;

                    try
                    {
                        mEntityContext.NoConfirmarTransacciones = true;
                        transaccionIniciada = proyAD.IniciarTransaccion(true);

                        GuardarCambios();

                        if (pIniciado)
                        {
                            HttpResponseMessage resultado = AdministrarIntegracionContinua(Ontologia, documentoID);
                            if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                            {
                                throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                            }
                        }

                        if (transaccionIniciada)
                        {
                            mEntityContext.TerminarTransaccionesPendientes(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (transaccionIniciada)
                        {
                            proyAD.TerminarTransaccion(false);
                        }
                        GuardarLogError(ex.ToString());
                        return false;
                    }

                    if (rutaActualizarDocsOnto != null)
                    {
                        docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        docCN.ActualizarFotoDocumentosDeOntologia(documentoID, rutaActualizarDocsOnto, !AgregadoIMG);
                        docCN.Dispose();
                    }

                    DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    docCL.GuardarOntologia(documentoID, null);
                    docCL.GuardarIDXmlOntologia(documentoID, Guid.NewGuid());
                    docCL.Dispose();

                    mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);

                    return true;
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex.ToString());
                    throw new Exception(ex.Message,ex);
                }
            }

            return true;
        }

        /*
        /// <summary>
        /// Crea un nuevo objeto de conocimiento
        /// </summary>
        /// <returns>ActionResult</returns>        
        private ObjetoConocimientoModel NuevoObjetoConocimento(string ontology)
        {
            EliminarPersonalizacionVistas();

            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, true, true);
            documentacionCN.Dispose();

            AD.EntityModel.Models.Documentacion.Documento filaDoc = dataWrapperDocumentacion.ListaDocumento.FirstOrDefault(doc => (doc.Tipo == (short)TiposDocumentacion.Ontologia || doc.Tipo == (short)TiposDocumentacion.OntologiaSecundaria) && doc.Enlace.ToLower().Equals(ontology.ToLower()));
            Dictionary<string, List<EstiloPlantilla>> listaEstilos = new Dictionary<string, List<EstiloPlantilla>>();
            byte[] arrayOnto = ControladorDocumentacion.ObtenerOntologia(filaDoc.DocumentoID, out listaEstilos, filaDoc.ProyectoID.Value, null, null, false);
            Ontologia ontologia = new Ontologia(arrayOnto, true);
            ontologia.LeerOntologia();

            ObjetoConocimientoModel objetoConocimiento = null;
            if (filaDoc != null)
            {
                objetoConocimiento = new ObjetoConocimientoModel();
                objetoConocimiento.Ontologia = ontology.Replace(".owl", string.Empty);
                objetoConocimiento.Name = filaDoc.Titulo.Replace(".owl", string.Empty);
                objetoConocimiento.EsCreacion = true;
                objetoConocimiento.CachearDatosSemanticos = true;
                objetoConocimiento.EsBuscable = true;
                objetoConocimiento.GrafoActual = filaDoc.Enlace;
                objetoConocimiento.Subtipos = new Dictionary<string, string>();
                objetoConocimiento.PresentacionListado = new ObjetoConocimientoModel.PresentacionModel();
                objetoConocimiento.PresentacionListado.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();
                objetoConocimiento.PresentacionMosaico = new ObjetoConocimientoModel.PresentacionModel();
                objetoConocimiento.PresentacionMosaico.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();
                objetoConocimiento.PresentacionMapa = new ObjetoConocimientoModel.PresentacionModel();
                objetoConocimiento.PresentacionMapa.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();
                objetoConocimiento.PresentacionRelacionados = new ObjetoConocimientoModel.PresentacionModel();
                objetoConocimiento.PresentacionRelacionados.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();
                objetoConocimiento.PresentacionPersonalizado = new ObjetoConocimientoModel.PresentacionPersonalizadoModel();
                objetoConocimiento.PresentacionPersonalizado.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel>();

                if (!string.IsNullOrEmpty(filaDoc.NombreCategoriaDoc) && filaDoc.NombreCategoriaDoc.Contains(".jpg"))
                {
                    objetoConocimiento.Image = filaDoc.NombreCategoriaDoc.Split(',')[1].Replace(".jpg", "_" + filaDoc.NombreCategoriaDoc.Split(',')[0] + ".jpg");
                }

                Dictionary<string, string> namespaces = ontologia.NamespacesDefinidos;
                string valor = string.Empty;
                string namespaceExtra = string.Empty;
                string namespacesFinal = string.Empty;
                foreach (string clave in namespaces.Keys)
                {
                    namespaces.TryGetValue(clave, out valor);
                    if (!valor.Equals("rdf") && !valor.Equals("xsd") && !valor.Equals("rdfs") && !valor.Equals("owl") && !valor.Equals("base") && !string.IsNullOrEmpty(valor))
                    {
                        namespaceExtra = $"{valor}:{clave}";
                        if (string.IsNullOrEmpty(namespacesFinal))
                        {
                            namespacesFinal = $"{namespaceExtra}";
                        }
                        else
                        {
                            namespacesFinal = $"{namespacesFinal}|{namespaceExtra}";
                        }
                    }
                }
                objetoConocimiento.NamespaceExtra = namespacesFinal;
                objetoConocimiento.Namespace = objetoConocimiento.Name;
                GuardarObjetoConocimiento(objetoConocimiento, filaDoc.DocumentoID);
            }

            return objetoConocimiento;
        }
        */

        /// <summary>
        /// Agrega el archivo de plantilla y el de su estilo a Gnoss.
        /// </summary>
        /// <returns>El nombre del archivo ontológico reemplazador</returns>
        protected string GuardarArchivos(EditOntologyViewModel Ontologia, Guid DocumentoID, ref bool AgregadoCSS, ref bool AgregadoIMG, ref bool AgregadoJS, ref string Error)
        {
            Stopwatch sw = null;
            try
            {
                FileInfo archivoInfo1 = null;

                if (Ontologia.OWL != null)
                {
                    archivoInfo1 = new FileInfo(Ontologia.OWL.FileName);
                }

                FileInfo archivoInfo2 = null;
                string extensionArchivo2 = null;

                if (!Ontologia.GenericCSS)
                {
                    if (Ontologia.CSS != null)
                    {
                        archivoInfo2 = new FileInfo(Ontologia.CSS.FileName);
                    }
                }
                else if (Ontologia.EditCSS)
                {
                    extensionArchivo2 = ".css";
                }

                FileInfo archivoInfo3 = null;
                byte[] buffer3 = null;
                string extensionArchivo3 = null;
                bool generarXMLDef = false;

                if (!Ontologia.GenericXML && !Ontologia.NoUseXML)
                {
                    if (Ontologia.XML != null)
                    {
                        archivoInfo3 = new FileInfo(Ontologia.XML.FileName);
                    }
                }
                else if (Ontologia.EditXML && Ontologia.GenericXML && !Ontologia.NoUseXML)
                {
                    generarXMLDef = true;
                    extensionArchivo3 = ".xml";
                }

                FileInfo archivoInfo4 = null;
                byte[] buffer4 = null;
                string extensionArchivo4 = null;

                if (!Ontologia.GenericIMG)
                {
                    if (Ontologia.IMG != null)
                    {
                        archivoInfo4 = new FileInfo(Ontologia.IMG.FileName);
                    }
                }
                else if (Ontologia.EditIMG)
                {
                    extensionArchivo4 = ".jpg";
                }

                FileInfo archivoInfo5 = null;
                string extensionArchivo5 = null;

                if (!Ontologia.GenericJS)
                {
                    if (Ontologia.JS != null)
                    {
                        archivoInfo5 = new FileInfo(Ontologia.JS.FileName);
                    }
                }
                else if (Ontologia.EditJS)
                {
                    extensionArchivo5 = ".js";
                }

                string extensionArchivo1 = null;

                if (archivoInfo1 != null)
                {
                    extensionArchivo1 = Path.GetExtension(archivoInfo1.Name).ToLower();
                }

                if (archivoInfo2 != null)
                {
                    extensionArchivo2 = Path.GetExtension(archivoInfo2.Name).ToLower();
                }

                if (archivoInfo3 != null)
                {
                    extensionArchivo3 = Path.GetExtension(archivoInfo3.Name).ToLower();
                }

                if (archivoInfo4 != null)
                {
                    extensionArchivo4 = Path.GetExtension(archivoInfo4.Name).ToLower();
                }

                if (archivoInfo5 != null)
                {
                    extensionArchivo5 = Path.GetExtension(archivoInfo5.Name).ToLower();
                }

                //Obtengo lo primero el buffer del archivo para no perderlo luego mediante un cierre de Stream.
                byte[] buffer1 = null;
                if (archivoInfo1 != null)
                {
                    using (BinaryReader reader1 = new BinaryReader(Ontologia.OWL.OpenReadStream()))
                    {
                        buffer1 = reader1.ReadBytes((int)Ontologia.OWL.Length);
                        Ontologia.FicheroOWL = Convert.ToBase64String(buffer1);
                    }
                }

                if (generarXMLDef)
                {
                    string nombreOntologia = null;
                    if (archivoInfo1 != null)
                    {
                        nombreOntologia = archivoInfo1.Name;
                    }
                    else
                    {
                        //Si no hemos reemplazo el archivo ontológico, devolvemos el enlace que ya poseia el doc:
                        nombreOntologia = GestorDocumental.ListaDocumentos[DocumentoID].Enlace;
                    }

                    nombreOntologia = nombreOntologia.Substring(0, nombreOntologia.LastIndexOf("."));

                    //Agrego namespaces y urls:
                    string urlOntologia = $"{BaseURLFormulariosSem}/Ontologia/{nombreOntologia}#";

                    GestionOWL gestorOWL = new GestionOWL();
                    gestorOWL.UrlOntologia = urlOntologia;
                    gestorOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_GNOSS;

                    //Obtengo la ontología:
                    byte[] arrayOntologia = null;

                    if (archivoInfo1 != null)
                    {
                        arrayOntologia = buffer1;
                    }
                    else
                    {
                        arrayOntologia = ControladorDocumentacion.ObtenerOntologia(DocumentoID, ProyectoSeleccionado.Clave);
                    }

                    //Leo la ontología:
                    Ontologia ontologia = new Ontologia(arrayOntologia, true);
                    ontologia.LeerOntologia();
                    ontologia.IdiomaUsuario = IdiomaUsuario;
                    buffer3 = ControladorDocumentacion.GenerarArchivoConfiguracionPlantillaOntologiaGenerico(nombreOntologia, ontologia, !Ontologia.Principal);
                }


                bool archivoOWLValido = string.IsNullOrEmpty(extensionArchivo1) || (extensionArchivo1.ToLower() == ".owl" && ControladorDocumentacion.ComprobarBuenFormatoPlantillaOWL(buffer1));
                bool archivoXMLValido = string.IsNullOrEmpty(extensionArchivo3) || extensionArchivo3.ToLower() == ".xml";
                bool archivoCSSValido = string.IsNullOrEmpty(extensionArchivo2) || extensionArchivo2.ToLower() == ".css";
                bool archivoIMGValido = string.IsNullOrEmpty(extensionArchivo4) || extensionArchivo4.ToLower() == ".jpg" || extensionArchivo4.ToLower() == ".png" || extensionArchivo4.ToLower() == ".gif" || extensionArchivo4.ToLower() == ".jpeg";
                bool archivoJSValido = extensionArchivo5 == null || extensionArchivo5.ToLower() == ".js";

                if (archivoOWLValido && archivoCSSValido && archivoXMLValido && archivoIMGValido && archivoJSValido)
                {
                    byte[] buffer2 = null;

                    if (archivoInfo2 != null)
                    {
                        if (!Ontologia.GenericCSS)
                        {
                            using (BinaryReader reader2 = new BinaryReader(Ontologia.CSS.OpenReadStream()))
                            {
                                buffer2 = reader2.ReadBytes((int)Ontologia.CSS.Length);
                                Ontologia.FicheroCSS = Convert.ToBase64String(buffer2);
                                reader2.Close();
                            }
                        }
                        else
                        {
                            extensionArchivo2 = ".css";
                        }

                        AgregadoCSS = true;
                    }

                    if (archivoInfo3 != null)
                    {
                        if (!Ontologia.GenericXML)
                        {
                            using (BinaryReader reader3 = new BinaryReader(Ontologia.XML.OpenReadStream()))
                            {
                                buffer3 = reader3.ReadBytes((int)Ontologia.XML.Length);
                                Ontologia.FicheroXML = Convert.ToBase64String(buffer3);
                                reader3.Close();
                                if (!ComprobarXMLConfiguracion(buffer3, ref Error))
                                {
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            using (BinaryReader reader3 = new BinaryReader(new StreamReader(archivoInfo3.FullName).BaseStream))
                            {
                                buffer3 = reader3.ReadBytes((int)archivoInfo3.Length);
                                Ontologia.FicheroXML = Convert.ToBase64String(buffer3);
                                reader3.Close();
                                if (!ComprobarXMLConfiguracion(buffer3, ref Error))
                                {
                                    return null;
                                }
                            }
                        }
                    }

                    if (archivoInfo4 != null)
                    {
                        if (!Ontologia.GenericIMG)
                        {
                            using (BinaryReader reader4 = new BinaryReader(Ontologia.IMG.OpenReadStream()))
                            {
                                buffer4 = reader4.ReadBytes((int)Ontologia.IMG.Length);
                                Ontologia.FicheroIMG = Convert.ToBase64String(buffer4);
                                reader4.Close();
                            }

                            Image imagen = Image.Load(new MemoryStream(buffer4));

                            if (extensionArchivo4 != ".jpg")
                            {
                                MemoryStream ms = new MemoryStream();
                                imagen.SaveAsJpeg(ms);
                                buffer4 = ms.ToArray();
                                extensionArchivo4 = ".jpg";
                            }

                            if (imagen.Height > 240 || imagen.Width > 240)
                            {
                                Image imagenPeque = UtilImages.AjustarImagen(imagen, 240, 240);
                                MemoryStream ms = new MemoryStream();
                                imagenPeque.SaveAsJpeg(ms);
                                buffer4 = ms.ToArray();
                                imagenPeque.Dispose();
                            }

                            imagen.Dispose();
                            AgregadoIMG = true;
                        }
                        else
                        {
                            extensionArchivo4 = ".jpg";
                        }
                    }

                    byte[] buffer5 = null;

                    if (archivoInfo5 != null)
                    {
                        if (!Ontologia.GenericJS)
                        {
                            using (BinaryReader reader5 = new BinaryReader(Ontologia.JS.OpenReadStream()))
                            {
                                buffer5 = reader5.ReadBytes((int)Ontologia.JS.Length);
                                Ontologia.FicheroJS = Convert.ToBase64String(buffer5);
                                reader5.Close();
                            }
                        }
                        else
                        {
                            extensionArchivo5 = ".js";
                        }

                        AgregadoJS = true;
                    }

                    #region Guardado archivos

                    int resultado = 0;

                    if (archivoInfo1 != null || extensionArchivo2 != null || buffer3 != null || buffer4 != null || extensionArchivo5 != null)
                    {

                        if (archivoInfo1 != null)
                        {
                            sw = LoggingService.IniciarRelojTelemetria();
                            //Además lo guardamos en la Web:
                            CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
                            fileService.GuardarOntologia(buffer1, DocumentoID);

                            resultado = 1;
                            mLoggingService.AgregarEntradaDependencia("Subir ontologia", false, "GuardarArchivos", sw, true);
                        }

                        if (buffer3 != null)//Se comprueba que el fichero xml no sea nulo
                        {
                            documentoXml = new XmlDocument();
                            MemoryStream ms = new MemoryStream(buffer3);
                            documentoXml.Load(ms);
                            proyectoID = mControladorBase.UsuarioActual.ProyectoID;
                            nombreCortoProyecto = ProyectoSeleccionado.NombreCorto;
                            if (Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE.ContainsKey(OntologiaID))
                            {
                                Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE[OntologiaID] = buffer3;
                            }
                            else
                            {
                                Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE.Add(OntologiaID, buffer3);
                            }
                            Dictionary<string, List<string>> selectores = new Dictionary<string, List<string>>();
                            selectores = ObtenerSelectores(buffer1);
                            foreach (string grafo in selectores.Keys)
                            {
                                foreach (string selector in selectores[grafo])
                                {
                                    FacetaEntidadesExternas facetaEntidad = new FacetaEntidadesExternas();
                                    facetaEntidad.Grafo = grafo;
                                    facetaEntidad.BuscarConRecursividad = true;
                                    facetaEntidad.EsEntidadSecundaria = false;
                                    facetaEntidad.EntidadID = $"{UrlIntragnoss}items/{selector}";
                                    facetaEntidad.OrganizacionID = UsuarioActual.OrganizacionID;
                                    facetaEntidad.ProyectoID = UsuarioActual.ProyectoID;
                                    AniadirFacetaEntidad(facetaEntidad);
                                }
                            }

                        }
                        if (archivoInfo1 == null || resultado == 1)
                        {
                            if (archivoInfo2 != null)
                            {
                                CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
                                fileService.GuardarCSSOntologia(buffer2, DocumentoID, $"{Path.DirectorySeparatorChar}Archivos{Path.DirectorySeparatorChar}{DocumentoID.ToString().Substring(0, 3)}{Path.DirectorySeparatorChar}", extensionArchivo2);
                            }

                            if (buffer3 != null)
                            {
                                CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
                                fileService.GuardarXmlOntologia(buffer3, DocumentoID);
                            }

                            if (buffer4 != null)
                            {
                                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                                servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");

                                servicioImagenes.AgregarImagenADirectorioOntologia(buffer4, $"Archivos{Path.DirectorySeparatorChar}{DocumentoID.ToString().Substring(0, 3)}", $"{DocumentoID}_240", extensionArchivo4);
                            }

                            if (archivoInfo5 != null)
                            {
                                CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
                                fileService.GuardarCSSOntologia(buffer5, DocumentoID, $"{Path.DirectorySeparatorChar}Archivos{Path.DirectorySeparatorChar}{DocumentoID.ToString().Substring(0, 3)}{Path.DirectorySeparatorChar}", extensionArchivo5);

                            }

                            resultado = 1;
                        }
                    }
                    else
                    {
                        //Para que no entre por el switch y devuelva null:
                        resultado = 1000;
                    }

                    switch (resultado)
                    {
                        case 0:
                            {
                                Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC");
                                break;
                            }
                        case 1:
                            {
                                string nuevoEnlace = null;

                                if (archivoInfo1 != null)
                                {
                                    nuevoEnlace = archivoInfo1.Name.ToLower();
                                }
                                else
                                {
                                    //Si no hemos reemplazo el archivo ontológico, devolvemos el enlace que ya poseia el doc:
                                    nuevoEnlace = GestorDocumental.ListaDocumentos[DocumentoID].Enlace;
                                }
                                return nuevoEnlace;
                            }
                        case 2:
                            {
                                Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSEGURIDAD");
                                break;
                            }
                        case 3:
                            {
                                Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSIZE");
                                break;
                            }
                    }

                    #endregion
                }
                else if ((extensionArchivo1 == null || extensionArchivo1.ToLower() == ".owl") && (extensionArchivo2 == null || extensionArchivo2.ToLower() == ".css") && (extensionArchivo3 == null || extensionArchivo3.ToLower() == ".xml") && (extensionArchivo5 == null || extensionArchivo5.ToLower() == ".js"))
                {
                    //Lo que ha fallado es la comprobación del buen formato del OWL
                    Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCFORMATOARCHIVOOWL", NombreProyectoEcosistema);
                }
                else
                {
                    Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCFORMATO");
                }
            }
            catch (ExcepcionGeneral ex)
            {
                //La ontología está mal construida 
                Error = ex.Message;
                throw;
            }
            catch (Exception)
            {
                //Se han producido errores al guardar el documento en el servidor
                Error = "Se produjo un error al subir el documento, inténtelo de nuevo más tarde.";
                throw;
            }
            return null;
        }

        /// <summary>
        /// Descarga el archivo de la ontología indicado
        /// </summary>
        /// <param name="pOntologiaID"></param>
        /// <param name="pVersion"></param>
        /// <param name="pOntologyName"></param>
        /// <returns></returns>
        private ActionResult DescargarArchivo(Guid pOntologiaID, string pVersion, string pOntologyName)
        {
            if (!string.IsNullOrEmpty(pVersion))
            {
                byte[] bytesArchivo = ObtenerOntologia(pOntologiaID, pVersion);

                return File(bytesArchivo, "application/text", pVersion.Replace(pOntologiaID.ToString(), pOntologyName));
            }

            return Content("No se ha encontrado el archivo");
        }


        /// <summary>
        /// Guarda los objetos de conocimiento
        /// </summary>
        /// <returns>ActionResult</returns>
        private void GuardarObjetoConocimiento(ObjetoConocimientoModel pObjetoConocimiento, Guid pOntologiaID)
        {
            ControladorObjetosConocimiento contrObjetosConocim = new ControladorObjetosConocimiento(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            string errores = contrObjetosConocim.ComprobarErroresObjetoConocimiento(pObjetoConocimiento);

            if (string.IsNullOrEmpty(errores))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;

                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);
                    contrObjetosConocim.AgregarObjetoConocimientoNuevo(pOntologiaID, pObjetoConocimiento);
                    if (HayIntegracionContinua)
                    {
                        HttpResponseMessage resultado = InformarCambioAdministracion("ObjetosConocimiento", JsonConvert.SerializeObject(pObjetoConocimiento, Newtonsoft.Json.Formatting.Indented));

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                    {
                        proyCN.ActualizarProyectos();
                    }

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch (Exception ex)
                {
                    if (transaccionIniciada)
                    {
                        proyAD.TerminarTransaccion(false);
                    }

                    throw new Exception(ex.Message);
                }

                contrObjetosConocim.InvalidarCaches(UrlIntragnoss);
            }
            else
            {
                throw new Exception(errores);
            }
        }

        /*
        /// <summary>
        /// Añade permisos para editar la ontología recien creada al administrador
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        private void AgregarPermisosAdministradorOntologia(Guid pDocumentoID)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            
            if (!proyectoCN.ExisteTipoDocDispRolUsuarioProySemantico(ProyectoSeleccionado.FilaProyecto.ProyectoID))
            {
                TipoDocDispRolUsuarioProy permisoEditDocsSemasAdmin = new TipoDocDispRolUsuarioProy();
                permisoEditDocsSemasAdmin.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                permisoEditDocsSemasAdmin.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;                
                permisoEditDocsSemasAdmin.TipoDocumento = (short)TiposDocumentacion.Semantico;
                permisoEditDocsSemasAdmin.RolUsuario = (short)TipoRolUsuario.Administrador;

                mEntityContext.TipoDocDispRolUsuarioProy.Add(permisoEditDocsSemasAdmin);
            }

            if(!proyectoCN.ExisteTipoOntoDispRolUsuarioProy(ProyectoSeleccionado.FilaProyecto.ProyectoID, pDocumentoID))
            {
                TipoOntoDispRolUsuarioProy permisoEditOntoAdmin = new TipoOntoDispRolUsuarioProy();
                permisoEditOntoAdmin.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                permisoEditOntoAdmin.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                permisoEditOntoAdmin.OntologiaID = pDocumentoID;
                permisoEditOntoAdmin.RolUsuario = (short)TipoRolUsuario.Administrador;

                mEntityContext.TipoOntoDispRolUsuarioProy.Add(permisoEditOntoAdmin);
            }

            mEntityContext.SaveChanges();
        }
        */

        /// <summary>
        /// Guarda el documento en la base de datos.
        /// </summary>
        private void GuardarCambios()
        {
            mEntityContext.SaveChanges();

            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCL.InvalidarOntologiasProyecto(ProyectoSeleccionado.Clave);

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarOntologiasEcosistema();
        }

        private byte[] ObtenerOntologia(Guid pOntologiaID, string pVersion = null)
        {
            CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
            byte[] bytesArchivo = null;
            if (string.IsNullOrEmpty(pVersion))
            {
                bytesArchivo = fileService.ObtenerOntologiaBytes(pOntologiaID);
            }
            else
            {
                bytesArchivo = fileService.DescargarVersionBytes(pOntologiaID, pVersion);
            }
            return bytesArchivo;
        }

        private byte[] ObtenerXmlOntologia(Guid pOntologiaID)
        {
            CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
            byte[] bytesArchivo = fileService.ObtenerXmlOntologiaBytes(pOntologiaID);
            return bytesArchivo;
        }

        private void CrearPomXml(string pDirectorio, string pNombreCortoProyecto)
        {
            string nombreFichero = Path.Combine(pDirectorio, "pom.xml");
            if (!System.IO.File.Exists(nombreFichero))
            {
                System.IO.File.WriteAllText(nombreFichero, EscribirPomXml(pNombreCortoProyecto));
            }
        }

        private string EscribirPomXml(string pNombreCortoProyecto)
        {
            StringBuilder clase = new StringBuilder();
            string versionApiWrapperJava = "0.0.2";

            clase.AppendLine("<project xmlns=\"http://maven.apache.org/POM/4.0.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://maven.apache.org/POM/4.0.0 https://maven.apache.org/xsd/maven-4.0.0.xsd\">");
            clase.AppendLine($"\t<modelVersion>4.0.0</modelVersion>  <!-- Por defecto mantener así -->");
            clase.AppendLine($"\t<groupId>com.arquitecturaJavaGnoss</groupId>  <!-- Introducir grupo al que pertenece el proyecto maven. Lo has indicado al crear el proyecto -->");
            clase.AppendLine($"\t<artifactId>Clases{pNombreCortoProyecto}</artifactId>  <!-- Introducir nombre que le has dado al proyecto. Lo has indicado al crear el proyecto -->");
            clase.AppendLine($"\t<version>1</version>  <!-- Versión inicial -->");
            clase.AppendLine();
            clase.AppendLine($"\t<properties>");
            clase.AppendLine();
            clase.AppendLine($"\t<project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>");
            clase.AppendLine($"\t\t<maven.compiler.source>1.8</maven.compiler.source>");
            clase.AppendLine($"\t\t<maven.compiler.target>1.8</maven.compiler.target>");
            clase.AppendLine($"\t</properties>");
            clase.AppendLine();
            clase.AppendLine($"\t<dependencies>");
            clase.AppendLine($"\t\t<dependency>");
            clase.AppendLine($"\t\t\t<groupId>io.github.equipognoss</groupId>");
            clase.AppendLine($"\t\t\t<artifactId>Gnoss-ApiWrapper-Java</artifactId>");
            clase.AppendLine($"\t\t\t<version>{versionApiWrapperJava}</version>");
            clase.AppendLine($"\t\t</dependency>");
            clase.AppendLine("\t\t<dependency>");
            clase.AppendLine("\t\t\t<groupId>com.microsoft.azure</groupId>");
            clase.AppendLine("\t\t\t<artifactId>applicationinsights-web-auto</artifactId>");
            clase.AppendLine("\t\t\t<!-- or applicationinsights-web for manual web filter registration -->");
            clase.AppendLine("\t\t\t<!-- or applicationinsights-core for bare API -->");
            clase.AppendLine("\t\t\t<version>2.6.2</version>");
            clase.AppendLine("\t\t</dependency>");
            clase.AppendLine($"\t</dependencies>");
            clase.AppendLine("</project>");

            return clase.ToString();
        }

        private HttpResponseMessage AdministrarIntegracionContinua(EditOntologyViewModel Ontologia, Guid documentoID)
        {
            HttpResponseMessage resultado = new HttpResponseMessage(HttpStatusCode.BadRequest);
            if (HayIntegracionContinua)
            {
                //Para la IC 
                Documento doc = GestorDocumental.ListaDocumentos[documentoID];
                string nombreOnto = doc.Enlace.Replace(".owl", "");
                CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
                if (Ontologia.FicheroOWL == null)
                {

                    Ontologia.FicheroOWL = fileService.ObtenerOntologia(documentoID, true);

                }
                if (Ontologia.FicheroCSS == null)
                {
                    Ontologia.FicheroCSS = fileService.DescargarCSSOntologia(documentoID, ".css", true);
                }
                if (Ontologia.FicheroIMG == null)
                {
                    ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                    servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");
                    byte[] buffer = servicioImagenes.ObtenerImagenDeDirectorioOntologia($"Archivos{Path.DirectorySeparatorChar}{documentoID.ToString().Substring(0, 3)}", $"{documentoID}_240", ".jpg");
                    if (buffer != null && buffer.Any())
                    {
                        Ontologia.FicheroIMG = Convert.ToBase64String(buffer);
                    }
                }
                if (Ontologia.FicheroJS == null)
                {

                    Ontologia.FicheroJS = Ontologia.FicheroCSS = fileService.DescargarCSSOntologia(documentoID, ".js", true);

                }
                if (Ontologia.FicheroXML == null)
                {
                    Ontologia.FicheroXML = fileService.ObtenerXmlOntologia(documentoID, true);

                }
                //Esto es para notificar las ontologias en la IC correctamente.
                Ontologia.NameOWL = nombreOnto;
                return InformarCambioAdministracion("Ontologias", JsonConvert.SerializeObject(Ontologia, Newtonsoft.Json.Formatting.Indented));
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Comprueba la correción del XML de configuración.
        /// </summary>
        /// <param name="pArray">Array del XML</param>
        /// <returns>TRUE si todo va bien, FALSE si no</returns>
        private bool ComprobarXMLConfiguracion(byte[] pArray, ref string Error)
        {
            Error = "";
            try
            {
                LectorXmlConfig lector = new LectorXmlConfig(Guid.Empty, ProyectoSeleccionado.Clave, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD);
                Dictionary<string, List<EstiloPlantilla>> listaEstilos = lector.ObtenerConfiguracionXml(pArray, ProyectoSeleccionado.Clave);

                mGrafosSimplesAutocompletarConfig = ((EstiloPlantillaConfigGen)listaEstilos["[ConfiguracionGeneral]"][0]).GrafosSimplesAutocompletar;
                mPropiedadesOntologia = ((EstiloPlantillaConfigGen)listaEstilos["[ConfiguracionGeneral]"][0]).PropiedadesOntologia;
                return true;
            }
            catch (Exception ex)
            {
                Error = UtilIdiomas.GetText("COMADMIN", "XMLCONFIGURACIONINCORRECTO") + ": " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Devuelve los selectores del archivo XML
        /// </summary> 
        /// <returns>La clave es el grafo y el valor la lista de selectores del grafo</returns>
        private Dictionary<string, List<string>> ObtenerSelectores(byte[] pOntologia)
        {
            string clase = string.Empty;
            Dictionary<string, List<string>> dicSelectores = new Dictionary<string, List<string>>();
            List<string> vSelectores = new List<string>();
            if (EspefEntidad != null)
            {

                XmlNodeList listaSeleccionEntidad = EspefPropiedad.SelectNodes($"Propiedad/SeleccionEntidad");
                foreach (XmlNode seleccionEntidad in listaSeleccionEntidad)
                {
                    XmlNode nodoGrafo = seleccionEntidad.SelectSingleNode("Grafo");
                    if (nodoGrafo != null)
                    {
                        string nombreGrafo = nodoGrafo.InnerText;

                        List<string> listaSelectoresNombre = new List<string>();
                        XmlNode selector = seleccionEntidad.SelectSingleNode("UrlTipoEntSolicitada");
                        if (selector == null)
                        {
                            Ontologia ontologia = null;
                            if (pOntologia == null)
                            {
                                DocumentacionCN documentacionCn = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                Guid ontologiaID = documentacionCn.ObtenerOntologiaAPartirNombre(ProyectoSeleccionado.Clave, nombreGrafo);
                                byte[] ontologiArrray = ControladorDocumentacion.ObtenerOntologia(ontologiaID, ProyectoSeleccionado.Clave);
                                ontologia = new Ontologia(ontologiArrray, true);
                            }
                            else
                            {
                                ontologia = new Ontologia(pOntologia, true);
                            }

                            ontologia.LeerOntologia();
                            List<ElementoOntologia> entidadesPrincipal = GestionOWL.ObtenerElementosContenedorSuperior(ontologia.Entidades);
                            ElementoOntologia entidadPrincipal = null;
                            if (entidadesPrincipal != null && entidadesPrincipal.Count > 0)
                            {
                                entidadPrincipal = entidadesPrincipal.First();
                                string tipoEntidad = entidadPrincipal.TipoEntidadRelativo;
                                if (!listaSelectoresNombre.Contains(tipoEntidad))
                                {
                                    listaSelectoresNombre.Add(tipoEntidad);
                                }
                            }
                        }
                        else if (selector != null)
                        {
                            string[] sel = selector.InnerText.Split(',');
                            foreach (string selNombre in sel)
                            {
                                string nombre = "";
                                if (selNombre != null && selNombre.Contains("#"))
                                {
                                    nombre = selNombre.Substring(selNombre.LastIndexOf("#") + 1);
                                }
                                else if (selNombre != null && selNombre.Contains("/"))
                                {
                                    nombre = selNombre.Substring(selNombre.LastIndexOf("/") + 1);
                                }
                                else
                                {
                                    nombre = selNombre;
                                }

                                if (!listaSelectoresNombre.Contains(nombre))
                                {
                                    listaSelectoresNombre.Add(nombre);
                                }
                            }
                        }

                        if (dicSelectores.ContainsKey(nombreGrafo))
                        {
                            vSelectores = dicSelectores[nombreGrafo];
                            vSelectores = vSelectores.Union(listaSelectoresNombre).ToList();
                            dicSelectores[nombreGrafo] = vSelectores;
                        }
                        else
                        {
                            dicSelectores.Add(nombreGrafo, listaSelectoresNombre);
                        }
                    }
                }
            }
            return dicSelectores;
        }

        /// <summary>
        /// Añade a la base de datos la faceta entidad externa pasada por parámetro
        /// </summary>
        /// <param name="facetaEntidad">Faceta entidad externa a añadir a la web</param>
        private void AniadirFacetaEntidad(FacetaEntidadesExternas facetaEntidad)
        {
            if (!mEntityContext.FacetaEntidadesExternas.Any(facetaEntidadExterna => facetaEntidadExterna.OrganizacionID.Equals(facetaEntidad.OrganizacionID) && facetaEntidadExterna.ProyectoID.Equals(facetaEntidad.ProyectoID) && facetaEntidadExterna.EntidadID.Equals(facetaEntidad.EntidadID)) && !mEntityContext.FacetaEntidadesExternas.Local.Any(facetaEntidadExterna => facetaEntidadExterna.OrganizacionID.Equals(facetaEntidad.OrganizacionID) && facetaEntidadExterna.ProyectoID.Equals(facetaEntidad.ProyectoID) && facetaEntidadExterna.EntidadID.Equals(facetaEntidad.EntidadID)))
            {
                mEntityContext.FacetaEntidadesExternas.Add(facetaEntidad);
            }
        }

        private PlantillaOntologicaViewModel ObtenerTemplate(Documento pDocumento)
        {
            PlantillaOntologicaViewModel template = new PlantillaOntologicaViewModel();
            template.OntologyID = pDocumento.Clave;
            template.IsSecondaryOntology = pDocumento.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria;

            template.Name = pDocumento.Titulo;
            template.OntologyName = pDocumento.Enlace.Replace(".owl", "");
            template.Description = pDocumento.Descripcion;
            if (!string.IsNullOrEmpty(pDocumento.FilaDocumento.NombreCategoriaDoc) && pDocumento.FilaDocumento.NombreCategoriaDoc.Contains(".jpg"))
            {
                template.Image = pDocumento.FilaDocumento.NombreCategoriaDoc.Split(',')[1].Replace(".jpg", $"_{pDocumento.FilaDocumento.NombreCategoriaDoc.Split(',')[0]}.jpg");
            }
            template.Protected = pDocumento.FilaDocumento.Protegido;
            if (ListaDocumentosConRecursos.ContainsKey(pDocumento.Clave))
            {
                template.HasResources = true;
            }


            bool cargasMultiplesDisponibles = false;
            Dictionary<string, string> propiedades = UtilCadenas.ObtenerPropiedadesDeTexto(pDocumento.NombreEntidadVinculada);
            if (propiedades.ContainsKey(PropiedadesOntologia.cargasmultiples.ToString()) && propiedades[PropiedadesOntologia.cargasmultiples.ToString()] == "true")
            {
                cargasMultiplesDisponibles = true;
            }
            template.AllowMasiveUpload = cargasMultiplesDisponibles;

            template.HasXmlFile = true;

            if (pDocumento.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria)
            {
                List<ProyectoConfigExtraSem> filas = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(pDocumento.Enlace) && proy.Tipo.Equals((short)TipoConfigExtraSemantica.EntidadSecundaria)).ToList();
                if (filas == null || filas.Count == 0)
                {
                    template.HasXmlFile = false;
                }
            }

            return template;
        }

        /// <summary>
        /// Valida si los datos de documento son correctos o no.
        /// </summary>
        /// <param name="Ontologia"></param>
        /// <param name="pComprobarFileUpdates">Indica si se debe comprobar o no los fileUpdates</param>
        /// <returns>True si los datos son correctos, false en caso contario.</returns>
        private bool ValidarAgregacionEdicionOnto(EditOntologyViewModel Ontologia, bool pComprobarFileUpdates)
        {
            if (string.IsNullOrEmpty(Ontologia.Name))
            {
                return false;
            }

            if (!pComprobarFileUpdates)
            {
                return true;
            }

            if (Ontologia.OWL == null)
            {
                return false;
            }
            if (Ontologia.XML == null && !Ontologia.GenericXML)
            {
                if (Ontologia.Principal || !Ontologia.NoUseXML)
                {
                    return false;
                }
            }
            if (Ontologia.CSS == null && !Ontologia.GenericCSS)
            {
                return false;
            }
            if (Ontologia.IMG == null && !Ontologia.GenericIMG)
            {
                return false;
            }
            if (Ontologia.JS == null && !Ontologia.GenericJS)
            {
                return false;
            }

            return true;
        }


        private List<string> CargarHistorial(Guid pOntologiaID)
        {
            List<string> listadoCopias = null;

            CallFileService fileService = new CallFileService(mConfigService, mLoggingService);

            string[] archivos = fileService.ObtenerHistorialOntologia(pOntologiaID);

            if (archivos != null && archivos.Length > 0)
            {
                listadoCopias = new List<string>(archivos);
            }


            return listadoCopias;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Ontología actual
        /// </summary>
        private Guid OntologiaID
        {
            get
            {
                Guid ontologiaSeleccionada = Guid.Empty;
                if (!string.IsNullOrEmpty(RequestParams("ontoID")))
                {
                    Guid.TryParse(RequestParams("ontoID"), out ontologiaSeleccionada);
                }
                return ontologiaSeleccionada;
            }
        }

        private Dictionary<Guid, bool> ListaDocumentosConRecursos
        {
            get
            {
                if (mListaDocumentosConRecursos == null)
                {
                    List<Guid> listaDocs = new List<Guid>(GestorDocumental.ListaDocumentos.Keys);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mListaDocumentosConRecursos = docCN.ComprobarSiOntologiaTieneRecursos(listaDocs);
                    docCN.Dispose();
                }
                return mListaDocumentosConRecursos;
            }
        }

        /// <summary>
        /// Espef entidad definida como etiqueta en el archivo de configuración de la ontología
        /// </summary>
        private XmlNode EspefEntidad
        {
            get
            {
                XmlNodeList listaespef = null;
                if (documentoXml.DocumentElement != null)
                {
                    listaespef = documentoXml.DocumentElement.SelectNodes("EspefEntidad");
                    if (listaespef != null)
                    {
                        if (listaespef.Count > 1)
                        {
                            foreach (XmlNode espef in listaespef)
                            {
                                if (espef.Attributes.Count > 0)
                                {
                                    if (espef.Attributes[0].InnerText.Equals(proyectoID))
                                    {
                                        return espef;
                                    }
                                    else if (espef.Attributes[0].InnerText.Equals(nombreCortoProyecto))
                                    {
                                        return espef;
                                    }
                                }
                            }
                            foreach (XmlNode espef in listaespef)
                            {
                                if (espef.Attributes.Count < 1)
                                {
                                    return espef;
                                }
                            }
                        }
                    }
                }
                if (listaespef != null)
                {
                    return listaespef.Item(0);
                }
                else { return null; }
            }
        }

        /// <summary>
        /// Carga las instacias principales de una ontología secundaría.
        /// </summary>
        private void CargarInstanciasOntologiaSecundaria(string pGrafo)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mProyectoEntidadSecundariaDW = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(ProyectoSeleccionado.Clave);

            ProyectoConfigExtraSem filaConfig = mProyectoEntidadSecundariaDW.ListaProyectoConfigExtraSem.FirstOrDefault(proy => proy.UrlOntologia.Equals(pGrafo));

            Guid ontologiaID = new Guid(filaConfig.SourceTesSem);
            mOntologiaEntidadSecundaria = ObtenerObjetoOntologia(ontologiaID);

            List<ElementoOntologia> entidadesPrinc = GestionOWL.ObtenerElementosContenedorSuperiorOHerencias(mOntologiaEntidadSecundaria.Entidades);
            mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables = new SortedDictionary<string, string>();
            FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            if (string.IsNullOrEmpty(mOntologiaEntidadSecundaria.ConfiguracionPlantilla.PropiedadTitulo.Key))
            {
                foreach (string sujeto in facCN.ObjeterSujetosDePropiedadPorValor(pGrafo, FacetadoAD.RDF_TYPE, new List<string>(new string[] { entidadesPrinc[0].TipoEntidad })))
                {
                    mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables.Add(sujeto, sujeto);
                }
            }
            else
            {
                FacetadoDS facDS = facCN.ObtenerRDFXMLSelectorEntidadFormulario(pGrafo, null, null, entidadesPrinc[0].TipoEntidad, new List<string>(new string[] { mOntologiaEntidadSecundaria.ConfiguracionPlantilla.PropiedadTitulo.Key }));

                foreach (DataRow fila in facDS.Tables[0].Rows)
                {
                    string sujeto = (string)fila[0];
                    if (!mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables.ContainsKey(sujeto))
                    {
                        mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables.Add(sujeto, (string)fila[2]);
                    }
                    else if (fila.Table.Columns.Count > 3 && !fila.IsNull(3) && (string)fila[3] == UtilIdiomas.LanguageCode)
                    {
                        mSecondaryEntityModel.SecondaryEntities.SecondaryInstancesEditables[sujeto] = (string)fila[2];
                    }
                }

            }

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mSecondaryEntityModel.SecondaryEntities.SecondaryOntologyNameSelected = docCN.ObtenerTituloDocumentoPorID(ontologiaID);
            docCN.Dispose();

            facCN.Dispose();
        }

        private void CargarInicial_EntSecund()
        {
            if (mSecondaryEntityModel == null)
            {
                mSecondaryEntityModel = new ComAdminSemanticElemModel();
            }

            mSecondaryEntityModel.PageType = ComAdminSemanticElemModel.ComAdminSemanticElemPage.SecondaryEntitiesEdition;
            mSecondaryEntityModel.SecondaryEntities = new ComAdminEditSecondaryEntities();
            mSecondaryEntityModel.SecondaryEntities.SecondaryEntitiesEditables = new Dictionary<string, string>();

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mProyectoEntidadSecundariaDW = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(ProyectoSeleccionado.Clave);
            Guid proyectoOntologiasID = Guid.Empty;
            //Obtener las del proyecto padre.
            if (ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
            {
                proyectoOntologiasID = new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
            }
            if (proyectoOntologiasID != Guid.Empty)
            {
                mProyectoEntidadSecundariaDW.ListaProyectoConfigExtraSem.AddRange(proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(proyectoOntologiasID).ListaProyectoConfigExtraSem);
            }

            proyCN.Dispose();

            foreach (ProyectoConfigExtraSem filaConfig in mProyectoEntidadSecundariaDW.ListaProyectoConfigExtraSem.Where(proy => proy.Tipo.Equals((short)TipoConfigExtraSemantica.EntidadSecundaria)).ToList())
            {
                if (!mSecondaryEntityModel.SecondaryEntities.SecondaryEntitiesEditables.ContainsKey(filaConfig.UrlOntologia))
                {
                    mSecondaryEntityModel.SecondaryEntities.SecondaryEntitiesEditables.Add(filaConfig.UrlOntologia, UtilCadenas.ObtenerTextoDeIdioma(filaConfig.Nombre, UtilIdiomas.LanguageCode, null));
                }
            }

            foreach (ProyectoConfigExtraSem filaConfig in mProyectoEntidadSecundariaDW.ListaProyectoConfigExtraSem.Where(proy => proy.Tipo.Equals((short)TipoConfigExtraSemantica.TesauroSemantico)).ToList())
            {
                mSecondaryEntityModel.SecondaryEntities.SecondaryEntitiesEditables.Remove(filaConfig.UrlOntologia);
            }
        }

        /// <summary>
        /// Carga el formulario de edición para crear una instación de una ontología secundaria.
        /// </summary>
        private void CrearEditarIntanciaOntologiaSecundaria(string pGrafo, bool pCreandoInstancia, string pSujetoEntidad = "")
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mProyectoEntidadSecundariaDW = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(ProyectoSeleccionado.Clave);

            ProyectoConfigExtraSem filaConfig = mProyectoEntidadSecundariaDW.ListaProyectoConfigExtraSem.FirstOrDefault(proy => proy.UrlOntologia.Equals(pGrafo));

            Guid ontologiaID = new Guid(filaConfig.SourceTesSem);
            Ontologia ontologia = ObtenerObjetoOntologia(ontologiaID);

            SemCmsController.ApanyarRepeticionPropiedades(ontologia.ConfiguracionPlantilla, ontologia.Entidades);
            mSecondaryEntityModel = new ComAdminSemanticElemModel();
            mSecondaryEntityModel.SecondaryEntities = new ComAdminEditSecondaryEntities();
            mSecondaryEntityModel.SecondaryEntities.SemanticResourceModel = new SemanticResourceModel();

            List<ElementoOntologia> instanciaPinc = null;
            mSecondaryEntityModel.SecondaryEntities.CreatingNewInstance = pCreandoInstancia;

            if (!pCreandoInstancia)
            {
                instanciaPinc = ObtenerInstanciasRDFDeEntidad(pGrafo, ontologia, pSujetoEntidad);
            }

            mSemController = new SemCmsController(mSecondaryEntityModel.SecondaryEntities.SemanticResourceModel, ontologia, Guid.Empty, instanciaPinc, ProyectoSeleccionado, IdentidadActual, UtilIdiomas, BaseURL, BaseURLIdioma, BaseURLContent, BaseURLStatic, UrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);

            try
            {
                mSemController.ObtenerModeloSemCMSEdicion(IdentidadActual.Clave);
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(mSecondaryEntityModel.SecondaryEntities.SemanticResourceModel.AdminGenerationError) || !ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    throw;
                }
            }

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mSecondaryEntityModel.SecondaryEntities.SecondaryOntologyNameSelected = docCN.ObtenerTituloDocumentoPorID(ontologiaID);
            docCN.Dispose();
        }

        /// <summary>
        /// Elimina una serie de instancias de la ontología secundaria.
        /// </summary>
        private void EliminarIntanciasOntologiaSecundaria(string pGrafo, string pSujeto)
        {
            CargarInstanciasOntologiaSecundaria(pGrafo);
            Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(pGrafo, ProyectoSeleccionado.Clave);
            FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            string idHasEntidadPrincipal = pSujeto;
            List<string> sujetos = facCN.ObtenerSujetosConObjetoDePropiedad(pGrafo, idHasEntidadPrincipal, "http://gnoss/hasEntidad");

            if (sujetos.Count > 0 && sujetos[0].Contains("entidadsecun_"))
            {
                idHasEntidadPrincipal = sujetos[0].Substring(sujetos[0].IndexOf("entidadsecun_"));
            }
            else
            {
                if (idHasEntidadPrincipal.Contains("/"))
                {
                    idHasEntidadPrincipal = idHasEntidadPrincipal.Substring(idHasEntidadPrincipal.LastIndexOf("/") + 1);
                }

                if (idHasEntidadPrincipal.Contains("#"))
                {
                    idHasEntidadPrincipal = idHasEntidadPrincipal.Substring(idHasEntidadPrincipal.LastIndexOf("#") + 1);
                }

                idHasEntidadPrincipal = $"entidadsecun_{idHasEntidadPrincipal.ToLower()}";
            }

            //Comprobamos en cada proyecto donde está compartida la ontología si se usa la entidad:
            foreach (Guid proyectoID in docOnto.ListaProyectos)
            {
                FacetadoDS facDS = facCN.ObtenerTripletasConObjeto(proyectoID.ToString(), pSujeto.ToLower());

                foreach (DataRow fila in facDS.Tables[0].Rows)
                {
                    if ((string)fila[1] != "http://gnoss/hasEntidad")
                    {
                        string error = UtilIdiomas.GetText("COMADMIN", "ENTIDADSECUNDNOBORRABLE", pSujeto, pGrafo);
                        throw new Exception(error);
                    }
                }

                facDS.Dispose();
            }

            ControladorDocumentacion.BorrarRDFDeVirtuoso(idHasEntidadPrincipal, pGrafo, UrlIntragnoss, "acid", ProyectoSeleccionado.Clave, true);

            foreach (Guid proyectoID in docOnto.ListaProyectos)
            {
                ControladorDocumentacion.BorrarRDFDeVirtuoso(idHasEntidadPrincipal, proyectoID.ToString().ToLower(), UrlIntragnoss, "acid", ProyectoSeleccionado.Clave, true);
                facCN.BorrarTripleta(proyectoID.ToString().ToLower(), $"<{UrlIntragnoss}{pGrafo.ToLower()}>", "<http://gnoss/hasEntidad>", $"<{UrlIntragnoss}{idHasEntidadPrincipal}>", true);
            }
        }

        /// <summary>
        /// Obtiene las instancias de la entidad secundaria seleccionada.
        /// </summary>
        /// <returns></returns>
        private List<ElementoOntologia> ObtenerInstanciasRDFDeEntidad(string pGrafo, Ontologia pOntologia, string pSujetoEntidad)
        {
            FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            List<string> sujetos = facCN.ObtenerSujetosConObjetoDePropiedad(pGrafo, pSujetoEntidad, "http://gnoss/hasEntidad");
            facCN.Dispose();

            if (sujetos.Count > 0 && sujetos[0].Contains("entidadsecun_"))
            {
                pSujetoEntidad = sujetos[0].Substring(sujetos[0].IndexOf("entidadsecun_"));
            }
            else
            {
                pSujetoEntidad = "entidadsecun_" + pSujetoEntidad.Substring(pSujetoEntidad.LastIndexOf("/") + 1).ToLower();
            }

            GestionOWL gestorOWL = new GestionOWL();
            gestorOWL.UrlOntologia = $"{BaseURLFormulariosSem}/Ontologia/{pGrafo}#"; ;
            gestorOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_GNOSS;

            MemoryStream buffer = new MemoryStream(ControladorDocumentacion.ObtenerRDFDeVirtuoso(pSujetoEntidad, pGrafo, UrlIntragnoss, pGrafo, gestorOWL.NamespaceOntologia, pOntologia, null, false));

            if (buffer == null)
            {
                string error = $"El recurso {pSujetoEntidad} no tiene datos en virtuoso.";
                throw new Exception(error);
            }

            StreamReader reader = new StreamReader(buffer);
            string rdfTexto = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            List<ElementoOntologia> instanciasPrincipales = null;

            try
            {
                instanciasPrincipales = gestorOWL.LeerFicheroRDF(pOntologia, rdfTexto, true);
            }
            catch (Exception ex)
            {
                string error = $"El RDF del recurso {pSujetoEntidad} no es correcto: {Environment.NewLine}{ex.Message}";
                throw;
            }

            return instanciasPrincipales;
        }

        /// <summary>
        /// Obtiene y lee una ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de ontología</param>
        /// <returns>Ontología leida</returns>
        private Ontologia ObtenerObjetoOntologia(Guid pOntologiaID)
        {
            Ontologia ontologia = null;
            Dictionary<string, List<EstiloPlantilla>> listaEstilos = null;
            byte[] arrayOnto = null;

            try
            {
                arrayOnto = ControladorDocumentacion.ObtenerOntologia(pOntologiaID, out listaEstilos, ProyectoSeleccionado.Clave);
            }
            catch (Exception ex)
            {
                string mensajeError = $"Error al leer el XML de la ontología con ID {pOntologiaID}:{Environment.NewLine}{ex}";
                GuardarLogErrorAJAX(mensajeError);
                throw;
            }

            if (arrayOnto == null)
            {
                string mensajeError = "No ha sido posible gererar el formulario porque el array de la ontología es nulo";
                throw new Exception(mensajeError);
            }

            try
            {
                ontologia = new Ontologia(arrayOnto, true);
                ontologia.LeerOntologia();
                ontologia.EstilosPlantilla = listaEstilos;
                ontologia.IdiomaUsuario = IdiomaUsuario;
                ontologia.OntologiaID = pOntologiaID;
            }
            catch (Exception ex)
            {
                string mensajeError = $"La ontología con ID {pOntologiaID} no es correcta:{Environment.NewLine}{ex}";
                GuardarLogErrorAJAX(mensajeError);
                throw;
            }

            if (ontologia.ConfiguracionPlantilla.ListaIdiomas.Count == 0)
            {
                ontologia.ConfiguracionPlantilla.ListaIdiomas.Add(IdiomaUsuario);
            }

            return ontologia;
        }

        /// <summary>
        /// Espef propiedad definida como etiqueta en el archivo de configuración de la ontología
        /// </summary>
        private XmlNode EspefPropiedad
        {
            get
            {
                XmlNodeList listaEspefPropiedad = null;
                if (documentoXml.DocumentElement != null)
                {
                    listaEspefPropiedad = documentoXml.DocumentElement.SelectNodes("EspefPropiedad");
                    if (listaEspefPropiedad != null)
                    {
                        if (listaEspefPropiedad.Count > 1)
                        {
                            foreach (XmlNode espef in listaEspefPropiedad)
                            {

                                if (espef.Attributes.Count > 0)
                                {
                                    if (espef.Attributes[0].InnerText.Equals(proyectoID))
                                    {
                                        return espef;
                                    }
                                    else if (espef.Attributes[0].InnerText.Equals(nombreCortoProyecto))
                                    {
                                        return espef;
                                    }
                                }
                            }
                            foreach (XmlNode espef in listaEspefPropiedad)
                            {
                                if (espef.Attributes.Count < 1)
                                {
                                    return espef;
                                }
                            }
                        }
                    }
                }
                if (listaEspefPropiedad != null)
                {
                    return listaEspefPropiedad.Item(0);
                }
                else { return null; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private GestorDocumental GestorDocumental
        {
            get
            {
                if (mGestorDocumental == null)
                {
                    mGestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext);

                    DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    docCL.ObtenerBaseRecursosProyecto(mGestorDocumental.DataWrapperDocumentacion, mControladorBase.UsuarioActual.ProyectoID, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.UsuarioID);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    docCN.ObtenerOntologiasProyecto(mControladorBase.UsuarioActual.ProyectoID, mGestorDocumental.DataWrapperDocumentacion, true, true, false, true);
                    docCN.Dispose();

                    mGestorDocumental.CargarDocumentos();
                }
                return mGestorDocumental;
            }
        }

        /// <summary>
        /// DataWrapper del proyecto actual
        /// </summary>
        private DataWrapperProyecto ProyectoConfigSemDataWrapperProyecto
        {
            get
            {
                if (mProyectoConfigSemDataWrapperProyecto == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mProyectoConfigSemDataWrapperProyecto = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(ProyectoSeleccionado.Clave);
                    proyCN.Dispose();
                }
                return mProyectoConfigSemDataWrapperProyecto;
            }

        }

        private AdministrarObjetosConocimientoViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarObjetosConocimientoViewModel();

                    if (ParametrosGeneralesRow.IdiomasDisponibles)
                    {
                        mPaginaModel.ListaIdiomas = mConfigService.ObtenerListaIdiomasDictionary();
                    }
                    else
                    {
                        mPaginaModel.ListaIdiomas = new Dictionary<string, string>();
                        mPaginaModel.ListaIdiomas.Add(IdiomaPorDefecto, mConfigService.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
                    }

                    mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;
                    mPaginaModel.ListaOntologiasCom = new Dictionary<string, string>();

                    DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                    DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Guid proyectoIDPatronOntologias = Guid.Empty;
                    if (ParametroProyecto.ContainsKey("ProyectoIDPatronOntologias"))
                    {
                        Guid.TryParse(ParametroProyecto["ProyectoIDPatronOntologias"], out proyectoIDPatronOntologias);
                        documentacionCN.ObtenerOntologiasProyecto(proyectoIDPatronOntologias, dataWrapperDocumentacion, false, true, true);
                    }

                    documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, true, true);
                    documentacionCN.Dispose();

                    List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = dataWrapperDocumentacion.ListaDocumento.Where(doc => doc.Tipo == (short)TiposDocumentacion.Ontologia || doc.Tipo == (short)TiposDocumentacion.OntologiaSecundaria && !doc.Eliminado).ToList();

                    mPaginaModel.ListaObjetosConocimiento = new List<ObjetoConocimientoModel>();

                    ControladorObjetosConocimiento contrObjetosConocim = new ControladorObjetosConocimiento(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                    foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in filasDoc)
                    {
                        var enlace = filaDoc.Enlace.Replace(".owl", "");
                        if (!mPaginaModel.ListaOntologiasCom.Any(x => string.Compare(x.Key, enlace, true) == 0))
                        {
                            mPaginaModel.ListaOntologiasCom.Add(enlace, filaDoc.Titulo);
                        }
                        ObjetoConocimientoModel objetoConocimiento = contrObjetosConocim.CargarObjetoConocimiento(filaDoc);

                        if (objetoConocimiento != null && !mPaginaModel.ListaObjetosConocimiento.Any(o => o.Ontologia == objetoConocimiento.Ontologia))
                        {
                            mPaginaModel.ListaObjetosConocimiento.Add(objetoConocimiento);
                        }
                    }
                }

                mPaginaModel.ListaObjetosConocimiento = mPaginaModel.ListaObjetosConocimiento.OrderByDescending(item => item.EsObjetoPrimario).ThenBy(item => item.Name).ToList();

                return mPaginaModel;
            }
        }

        private AdministrarPlantillasOntologicasViewModel DescargaClasesModel
        {
            get
            {
                if (mDescargaClasesModel == null)
                {
                    mDescargaClasesModel = new AdministrarPlantillasOntologicasViewModel();
                    mDescargaClasesModel.Templates = new List<PlantillaOntologicaViewModel>();
                    mDescargaClasesModel.SecondaryTemplates = new List<PlantillaOntologicaViewModel>();

                    foreach (Documento doc in GestorDocumental.ListaDocumentos.Values)
                    {
                        if (doc.TipoDocumentacion == TiposDocumentacion.Ontologia)
                        {
                            mDescargaClasesModel.Templates.Add(ObtenerTemplate(doc));
                        }
                        else if (doc.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria)
                        {
                            mDescargaClasesModel.SecondaryTemplates.Add(ObtenerTemplate(doc));
                        }
                    }

                    mDescargaClasesModel.SelectedOntology = new PlantillaOntologicaViewModel();

                    Guid ontologiaSeleccionada = OntologiaID;

                    if (!ontologiaSeleccionada.Equals(Guid.Empty) && GestorDocumental.ListaDocumentos.ContainsKey(ontologiaSeleccionada))
                    {
                        mDescargaClasesModel.SelectedOntology = ObtenerTemplate(GestorDocumental.ListaDocumentos[ontologiaSeleccionada]);
                    }

                }
                return mDescargaClasesModel;
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarPlantillasOntologicasViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public List<PlantillaOntologicaViewModel> Templates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PlantillaOntologicaViewModel> SecondaryTemplates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PlantillaOntologicaViewModel SelectedOntology { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PlantillaOntologicaViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSecondaryOntology { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OntologyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Protected { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool HasResources { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AllowMasiveUpload { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool HasXmlFile { get; set; }

    }
    public class OntologicalTemplatesAdministrationViewModel
    {
        public Guid OntologyID { get; set; }
        public List<string> VersionList { get; set; }
        public string OntologyName { get; set; }
    }
}
