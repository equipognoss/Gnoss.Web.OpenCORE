using DocumentFormat.OpenXml.Bibliography;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Flujos;
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
using Es.Riam.Gnoss.Elementos.Comentario;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.ExportarImportar;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.ExportarImportar.Exportadores;
using Es.Riam.Gnoss.Logica.Comentario;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Logica.Voto;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.ControlesMVC;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.FicherosRecursos;
using Es.Riam.Gnoss.Web.MVC.Models.Flujos;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Open.Model;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Gnoss.Web.Controllers;
using Gnoss.Web.Open.Filters;
using Gnoss.Web.Open.Models.FichaRecurso;
using Google.Protobuf.Compiler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Universal.Common.Extensions;
using static Es.Riam.Gnoss.Web.MVC.Models.ResourceModel;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [Serializable]
    public class FichaRecursoController : ControllerBaseWeb
    {
        private IRDFSearch mRDFSearch;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public FichaRecursoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IRDFSearch rDFSearch, IOAuth oAuth, IHostApplicationLifetime appLifetime, IPublishEvents publishEvents, IAvailableServices availableServices, ILogger<FichaRecursoController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mRDFSearch = rDFSearch;
            mIPublishEvents = publishEvents;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros

        private static List<string> mListaDominiosSinPalco = null;

        private ResourceViewModel paginaModel = new ResourceViewModel();

        /// <summary>
        /// Documento actual.
        /// </summary>
        private DocumentoWeb mDocumento = null;

        /// <summary>
        /// Documento actual está versionado
        /// </summary>
        private bool mDocumentoVersionado = false;

        /// <summary>
        /// Guid de la ultima version del documento actual
        /// </summary>
        private Guid mDocumentoUltimaVersionID = Guid.Empty;

        /// <summary>
        /// Guid del documento original
        /// </summary>
        private Guid mDocumentoOriginalVersionID = Guid.Empty;

        /// <summary>
        /// Identificador de documento.
        /// </summary>
        private GestorDocumental mGestorDocumental;

        /// <summary>
        /// Generador plantillasOWL
        /// </summary>
        private SemCmsController mGenPlantillasOWL;

        private KeyValuePair<string, string>? mPestanyaRecurso = null;

        /// <summary>
        /// ID del usuario que está viendo el recurso publicado en el espacio personal de otra persona.
        /// </summary>
        private Guid? mUsuarioVeRecursoOtroPerfilID;

        /// <summary>
        /// Token del usuario de la peticion
        /// </summary>
        private string tokenSP;

        /// <summary>
        /// Token del usuario creador del documento
        /// </summary>
        private string tokenSPAdminDocument;
        private IPublishEvents mIPublishEvents;

        #endregion

        #region Controlador

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ComprobarDocumentoVersionado();

            if (filterContext.Result == null)
            {
                ComprobarRedirecciones(filterContext);
            }

            if (filterContext.Result != null)
            {
                return;
            }

            // Cargamos el gestor documental y el documento actual
            if (mGestorDocumental == null)
            {
                CargarInicial();
            }
            if (mDocumento == null)
            {
                CargaInicialDocumento();
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Comprueba si existe el documento en el proyecto 
        /// </summary>
        /// <param name="pFilterContext"></param>
        private void ComprobarRedirecciones(ActionExecutingContext pFilterContext)
        {
            try
            {
                if (pFilterContext.Result == null)
                {
                    ComprobarExisteDocumento(pFilterContext);
                }
                if (pFilterContext.Result == null)
                {
                    ComprobarPeticion(pFilterContext);
                }
                if (pFilterContext.Result == null)
                {
                    ComprobarSesion(pFilterContext);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
            }
        }

        /// <summary>
        /// Asigna valores a las propiedades del controlador para determinar si el documento de la peticion está versionado,
        /// cual es el documentoID de la ultima version y cual es el documentoID de la version original en el caso de que el 
        /// documento esté versionado.
        /// </summary>
        private void ComprobarDocumentoVersionado()
        {
            if (DocumentoVersionID != Guid.Empty)
            {
                mDocumentoVersionado = true;
                mDocumentoUltimaVersionID = DocumentoVersionID;
                mDocumentoOriginalVersionID = DocumentoID;
            }
            else
            {
            using (DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory))
            {
                try
                {
                        Guid ultimaVersion = docCN.ObtenerUltimaVersionDeDocumento(DocumentoID);

                        if (ultimaVersion.Equals(Guid.Empty))
                        {
                        return;
                        }

                    mDocumentoVersionado = true;
                        mDocumentoUltimaVersionID = ultimaVersion;
                        mDocumentoOriginalVersionID = DocumentoID;
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                }
            }
        }
        }

        /// <summary>
        /// Comprobamos la existencia de la ultima version del documento solicitado
        /// En el caso de que esté eliminado no mostraremos sus versiones.
        /// </summary>
        /// <param name="pFilterContext"></param>
        private void ComprobarExisteDocumento(ActionExecutingContext pFilterContext)
        {
            try
            {
                // Si esta versionado mDocumentoUltimaVersionID deberia tener valor por lo que comprobamos si no está eliminado.
                // Si no tiene valor es que es un documento sin versionar previo al cambio de versionado o un recurso creado
                // por la api.
                Guid ultimaVersionDocumentoID = mDocumentoUltimaVersionID != Guid.Empty ? mDocumentoUltimaVersionID : DocumentoID;

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                Guid pProyectoID = ProyectoSeleccionado.Clave;
                bool esPeticionConVersionID = HttpContext.Request.RouteValues.ContainsKey("versionDocID");

                // Comprobamos si el documento está compartido. Si ese es el caso la comprobacion de su
                // existencia será con el proyecto id original.
                if (docCN.EstaDocumentoCompartidoEnProyecto(ultimaVersionDocumentoID, pProyectoID))
                {
                    pProyectoID = docCN.ObtenerProyectoIDPorDocumentoID(ultimaVersionDocumentoID);
                }

                if (!docCN.ExisteDocumentoEnProyecto(pProyectoID, ultimaVersionDocumentoID))
                {
                    pFilterContext.Result = RedireccionarAPaginaNoEncontrada();
                }

                if (esPeticionConVersionID && (DocumentoVersionID == Guid.Empty || !docCN.ExisteDocumentoEnProyecto(pProyectoID, DocumentoVersionID)))
                {
                    DataWrapperDocumentacion dwDocumentacion = docCN.ObtenerDocumentoPorID(DocumentoID);
                    DocumentoWeb documentoWeb = new DocumentoWeb(dwDocumentacion.ListaDocumento.First(), new GestorDocumental(dwDocumentacion, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory));

                    pFilterContext.Result = GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, documentoWeb, false));
                }

                docCN.Dispose();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
            }
        }

        /// <summary>
        /// Comprobamos que la peticion está bien formada. Si solo se indica un GUID, tiene que ser el Id original del recurso.
        /// </summary>
        /// <param name="pFilterContext"></param>
        private void ComprobarPeticion(ActionExecutingContext pFilterContext)
        {
            try
            {
                if (DocumentoVersionado && DocumentoVersionID == Guid.Empty)
                {
                    if (DocumentoID != mDocumentoOriginalVersionID && pFilterContext.ActionDescriptor.RouteValues["action"].Equals("Index"))
                    {
                        pFilterContext.Result = RedireccionarAPaginaNoEncontrada();
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
            }
        }

        /// <summary>
        /// Comprobamos que no se ha perdido la sesion, en caso de haberlo hecho, se le redirige al servicio de login para
        /// verificar si el usuario estaba logueado.
        /// </summary>
        /// <param name="pFilterContext"></param>
        private void ComprobarSesion(ActionExecutingContext pFilterContext)
        {
            if (Session.Get<GnossIdentity>("Usuario") == null || (Session.Get<GnossIdentity>("Usuario")).EsUsuarioInvitado)
            {
                pFilterContext.Result = mControladorBase.SolicitarCookieLoginUsuario(UsuarioActual, TokenLoginUsuario);
            }
        }
        #endregion

        #region Comentarios

        /// <summary>
        /// Acción para eliminar un cometario
        /// </summary>
        /// <param name="comentarioID">Comentario a eliminar</param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult DeleteCommentDetached(Guid comentarioID)
        {
            try
            {
                //Seguridad : 
                /*
                 * Si tiene permisos para eliminar el comentario
                 * Si el comentario no tiene respuestas o las respuestas son del creador del comentario o es el administrador del proyecto
                */

                CargarComentarios();

                Comentario comentario = GestorDocumental.GestionComentarios.ListaComentarios[comentarioID];

                bool respuestasMismoAutor = ComprobarAutorHijos(comentario, comentario.FilaComentario.IdentidadID);
                bool tieneRespuestas = (comentario.Hijos.Count > 0);

                if ((!tieneRespuestas || respuestasMismoAutor || ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID)) && comentario.TienePermisosIdentidadEliminar(IdentidadActual, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion))
                {
                    List<Comentario> comentariosEliminados = new List<Comentario>();

                    if (comentario.Hijos.Count > 0)
                    {
                        comentariosEliminados = EliminarComentariosHijos(comentario, Documento);
                        comentario.LimpiarHijos();
                    }

                    GestorDocumental.GestionComentarios.EliminarComentario(comentario, Documento);
                    comentariosEliminados.Add(comentario);
                }

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" ComentarioID: {comentarioID}");
                throw;
            }
        }

        /// <summary>
        /// Acción para eliminar un cometario
        /// </summary>
        /// <param name="comentarioID">Comentario a eliminar</param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult DeleteComment(Guid comentarioID)
        {
            try
            {
                ComentarioCN comentarioCN = new ComentarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ComentarioCN>(), mLoggerFactory);
                AD.EntityModel.Models.Comentario.Comentario comentario = comentarioCN.ObtenerComentarioPorID(comentarioID);
                Comentario elementoComentario = new Comentario(comentario, new GestionComentarios(new DataWrapperComentario(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionComentarios>(), mLoggerFactory), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<Comentario>(), mLoggerFactory);

                if (comentario != null)
                {
                    List<AD.EntityModel.Models.Comentario.Comentario> listaComentariosEliminar = new List<AD.EntityModel.Models.Comentario.Comentario>();
                    listaComentariosEliminar.Add(comentario);
                    listaComentariosEliminar.AddRange(comentarioCN.ObtenerTodosComentariosHijosDeComentarios(listaComentariosEliminar));

                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) || elementoComentario.TienePermisosIdentidadEliminar(IdentidadActual, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || EsAutorDeComentariosHijos(listaComentariosEliminar))
                    {
                        MarcarListaComentariosComoEliminados(listaComentariosEliminar);

                        comentarioCN.ActualizarComentarioEntity();

                        ActualizarNumeroComentariosRecurso(listaComentariosEliminar);
                    }
                    else
                    {
                        throw new ExcepcionWeb("No puedes eliminar un comentario con respuestas que no sean tuyas.");
                    }
                }
                else
                {
                    throw new ExcepcionWeb("No existe ningún comentario con ese ID");
                }

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}\r\nComentarioID: {comentarioID}", mlogger);
                throw;
            }
        }


        /// <summary>
        /// Marca todos los comentarios recibidos por parametro como eliminados
        /// </summary>
        /// <param name="pListaComentariosEliminar"></param>
        private void MarcarListaComentariosComoEliminados(List<AD.EntityModel.Models.Comentario.Comentario> pListaComentariosEliminar)
        {
            foreach (AD.EntityModel.Models.Comentario.Comentario comentario in pListaComentariosEliminar)
            {
                comentario.Eliminado = true;
            }
        }

        /// <summary>
        /// Comprueba si el usuario actual es el autor de todos los comentarios pasados por parametro
        /// </summary>
        /// <param name="pListaComentarios"></param>
        /// <returns></returns>
        private bool EsAutorDeComentariosHijos(List<AD.EntityModel.Models.Comentario.Comentario> pListaComentarios)
        {
            foreach (AD.EntityModel.Models.Comentario.Comentario comentario in pListaComentarios)
            {
                if (!IdentidadActual.ListaTodosIdentidadesDeIdentidad.Contains(comentario.IdentidadID))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Acción para editar un comentario
        /// </summary>
        /// <param name="comentarioID">Comentario a editar</param>
        /// <param name="Description">Descripción editada del comentario</param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult EditComment(Guid comentarioID, string Description)
        {
            try
            {
                //Seguridad : 
                /*
                 * Si la comunidad permite comentarios o el documento es una pregunta
                 * Si el documento el la ultima versión y permite comentarios
                 * Si tiene permisos para editar el comentario
                 * Si el comentario no tiene respuestas o las respuestas son del creador del comentario
                */

                CargarComentarios();

                Comentario comentario = GestorDocumental.GestionComentarios.ListaComentarios[comentarioID];
                string textoComentario = HttpUtility.UrlDecode(Description);

                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);

                bool respuestasMismoAutor = ComprobarAutorHijos(comentario, comentario.FilaComentario.IdentidadID);
                bool tieneRespuestas = (comentario.Hijos.Count > 0);

                if ((!tieneRespuestas || respuestasMismoAutor) && comentario.TienePermisosEdicionIdentidad(IdentidadActual, EsIdentidadActualAdministradorOrganizacion) && MostrarControlAgregarComentario && (ComunidadPermiteComentarios || Documento.TipoDocumentacion == TiposDocumentacion.Pregunta))
                {
                    mEntityContext.Comentario.First(item => item.ComentarioID.Equals(comentarioID)).Descripcion = textoComentario;
                    GuardarComentarios();
                }

                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.InvalidarComentariosRecursoMVC(DocumentoID, ProyectoSeleccionado.Clave);
                docCL.InvalidarEventosRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);
                docCL.Dispose();

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}\r\nComentarioID: {comentarioID} Description: {Description}", mlogger);
                throw;
            }
        }

        /// <summary>
        /// Acción de responder un comentario
        /// </summary>
        /// <param name="Description">Descripción de la respuesta al comentario</param>
        /// <param name="comentarioID">Comentario al que vamos a responder</param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult ReplyComment(string Description, Guid comentarioID)
        {
            return CrearComentario(Description, comentarioID);
        }

        /// <summary>
        /// Acción de crear un comentario
        /// </summary>
        /// <param name="Description">Descripción del comentario que vamos a crear</param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult CreateComment(string Description)
        {
            return CrearComentario(Description, Guid.Empty);
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult GetComments(int pInicio)
        {
            try
            {
                List<CommentModel> listaComentarios = CargarListaComentarios(pInicio);

                MultiViewResult result = new MultiViewResult(this, mViewEngine);

                int i = 0;
                foreach (CommentModel fichaComentario in listaComentarios)
                {
                    result.AddView("_FichaComentario", $"fichaComentariio_{i}", fichaComentario);
                    i++;
                }

                return result;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                throw;
            }
        }

        /// <summary>
        /// Acción para votar positivamente un comentario
        /// </summary>
        /// <param name="comentarioID">Comentario a votar</param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult VoteCommentPositive(Guid comentarioID)
        {
            try
            {
                CargarComentarios();

                Comentario comentario = GestorDocumental.GestionComentarios.ListaComentarios[comentarioID];

                //Seguridad : No puede votar dos veces, borramos el voto anterior
                List<AD.EntityModel.Models.Voto.Voto> filasVotos = comentario.GestorComentarios.GestorVotos.VotoDW.ListaVotos.Where(item => item.IdentidadID.Equals(IdentidadActual.Clave) && item.ElementoID.Equals(comentarioID)).ToList();
                if (filasVotos.Count > 0)
                {
                    comentario.ListaVotos.Remove(filasVotos[0].VotoID);

                    AD.EntityModel.Models.Comentario.VotoComentario filaVotoCom = comentario.GestorComentarios.ComentarioDW.ListaVotoComentario.Find(item => item.VotoID.Equals(filasVotos[0].VotoID));
                    comentario.GestorComentarios.ComentarioDW.ListaVotoComentario.Remove(filaVotoCom);
                    mEntityContext.EliminarElemento(filaVotoCom);

                    comentario.GestorComentarios.GestorVotos.VotoDW.ListaVotos.Remove(filasVotos[0]);
                    mEntityContext.EliminarElemento(filasVotos[0]);
                }
                GestorDocumental.GestionComentarios.GestorVotos.AgregarVoto(VotoDocumento.Positivo, IdentidadActual.Clave, comentario.Clave, comentario.FilaComentario.IdentidadID, comentario);
                GuardarVotosComentarios();

                List<CommentModel> listaComentarios = CargarListaComentarios();

                MultiViewResult result = new MultiViewResult(this, mViewEngine);

                int i = 0;
                foreach (CommentModel fichaComentario in listaComentarios)
                {
                    result.AddView("_FichaComentario", $"fichaComentariio_{i}", fichaComentario);
                    i++;
                }

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);

                return result;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" ComentarioID: {comentarioID}");
                throw;
            }
        }

        /// <summary>
        /// Acción para votar negativamente un comentario
        /// </summary>
        /// <param name="comentarioID">Comentario a votar</param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult VoteCommentNegative(Guid comentarioID)
        {
            try
            {
                CargarComentarios();

                Comentario comentario = GestorDocumental.GestionComentarios.ListaComentarios[comentarioID];

                //Seguridad : No puede votar dos veces, borramos el voto anterior
                List<AD.EntityModel.Models.Voto.Voto> filasVotos = comentario.GestorComentarios.GestorVotos.VotoDW.ListaVotos.Where(item => item.IdentidadID.Equals(IdentidadActual.Clave) && item.ElementoID.Equals(comentarioID)).ToList();
                if (filasVotos.Count > 0)
                {
                    comentario.ListaVotos.Remove(filasVotos[0].VotoID);
                    AD.EntityModel.Models.Comentario.VotoComentario filaVotoCom = comentario.GestorComentarios.ComentarioDW.ListaVotoComentario.Find(item => item.VotoID.Equals(filasVotos[0].VotoID));

                    mEntityContext.EliminarElemento(filaVotoCom);
                    comentario.GestorComentarios.ComentarioDW.ListaVotoComentario.Remove(filaVotoCom);


                    mEntityContext.EliminarElemento(filasVotos[0]);
                    comentario.GestorComentarios.GestorVotos.VotoDW.ListaVotos.Remove(filasVotos[0]);
                }

                GestorDocumental.GestionComentarios.GestorVotos.AgregarVoto(VotoDocumento.Negativo, IdentidadActual.Clave, comentario.Clave, comentario.FilaComentario.IdentidadID, comentario);
                GuardarVotosComentarios();

                List<CommentModel> listaComentarios = CargarListaComentarios();

                MultiViewResult result = new MultiViewResult(this, mViewEngine);

                int i = 0;
                foreach (CommentModel fichaComentario in listaComentarios)
                {
                    result.AddView("_FichaComentario", $"fichaComentariio_{i}", fichaComentario);
                    i++;
                }

                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.InvalidarComentariosRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);

                return result;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" ComentarioID: {comentarioID}");
                throw;
            }
        }

        /// <summary>
        /// Acción para votar negativamente un comentario
        /// </summary>
        /// <param name="comentarioID">Comentario a votar</param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult DeleteVoteComment(Guid comentarioID)
        {
            try
            {
                CargarComentarios();

                Comentario comentario = GestorDocumental.GestionComentarios.ListaComentarios[comentarioID];

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);

                //Seguridad : No puede votar dos veces, borramos el voto anterior
                List<AD.EntityModel.Models.Voto.Voto> filasVotos = comentario.GestorComentarios.GestorVotos.VotoDW.ListaVotos.Where(item => item.IdentidadID.Equals(IdentidadActual.Clave) && item.ElementoID.Equals(comentarioID)).ToList();

                if (filasVotos.Count > 0)
                {
                    comentario.ListaVotos.Remove(filasVotos[0].VotoID);
                    AD.EntityModel.Models.Comentario.VotoComentario filaVotoCom = comentario.GestorComentarios.ComentarioDW.ListaVotoComentario.Find(item => item.VotoID.Equals(filasVotos[0].VotoID));

                    mEntityContext.EliminarElemento(filaVotoCom);
                    comentario.GestorComentarios.ComentarioDW.ListaVotoComentario.Remove(filaVotoCom);

                    mEntityContext.EliminarElemento(filasVotos[0]);
                    comentario.GestorComentarios.GestorVotos.VotoDW.ListaVotos.Remove(filasVotos[0]);
                }

                GuardarVotosComentarios();

                List<CommentModel> listaComentarios = CargarListaComentarios();

                MultiViewResult result = new MultiViewResult(this, mViewEngine);

                int i = 0;
                foreach (CommentModel fichaComentario in listaComentarios)
                {
                    result.AddView("_FichaComentario", $"fichaComentariio_{i}", fichaComentario);
                    i++;
                }

                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.InvalidarComentariosRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);

                return result;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" ComentarioID: {comentarioID}");
                throw;
            }
        }

        /// <summary>
        /// Acción para bloquear los comentarios de un recurso
        /// </summary>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LockComments()
        {
            try
            {
                //Hay que hacer esta carga porque si no, no comprueba bien los permisos de edición.
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);

                //Seguridad : Si no es borrador y tiene permisos para bloquear y desbloquear
                if (!Documento.EsBorrador && Documento.TienePermisosIdentidadBloquearDesbloquearComentarios(IdentidadActual, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) && ProyectoOrigenBusquedaID == Guid.Empty && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonBloquearComentariosDoc))
                {
                    return AccionRecurso_Comentarios_Bloquear(true);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }
            return GnossResultERROR();
        }

        /// <summary>
        /// Acción para desbloquear los comentarios de un recurso
        /// </summary>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult UnlockComments()
        {
            try
            {
                //Hay que hacer esta carga porque si no, no comprueba bien los permisos de edición.
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);

                //Seguridad : Si no es borrador y tiene permisos para bloquear y desbloquear
                if (!Documento.EsBorrador && Documento.TienePermisosIdentidadBloquearDesbloquearComentarios(IdentidadActual, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) && ProyectoOrigenBusquedaID == Guid.Empty && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonBloquearComentariosDoc))
                {
                    return AccionRecurso_Comentarios_Bloquear(false);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }
            return GnossResultERROR();
        }

        /// <summary>
        /// Bloquea los comentarios en una pregunta
        /// </summary>
        /// <returns>Código de respuesta para actualizar mediante javascript el control</returns>
        private ActionResult AccionRecurso_Comentarios_Bloquear(bool bloquear)
        {
            Guid proyectoActuacionID = ProyectoSeleccionado.Clave;

            if (ProyectoOrigenBusquedaID != Guid.Empty)
            {
                proyectoActuacionID = ProyectoOrigenBusquedaID;
            }

            if (!Documento.FilaDocumento.UltimaVersion)
            {
                return GnossResultERROR("No es la ultima versión");
            }
            if (bloquear && !Documento.PermiteComentarios)
            {
                return GnossResultERROR("Los comentarios ya estan bloqueados");
            }
            else if (!bloquear && Documento.PermiteComentarios)
            {
                return GnossResultERROR("Los comentarios ya estan desbloqueados");
            }

            List<Guid> listaProyectosCambioEstado = new List<Guid>();

            if (Documento.FilaDocumentoWebVinBR != null)
            {
                if (Documento.ProyectoID == proyectoActuacionID)
                {
                    bool permiteComentario = !Documento.FilaDocumentoWebVinBR.PermiteComentarios;

                    foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR in Documento.FilaDocumento.DocumentoWebVinBaseRecursos)
                    {
                        filaDocVinBR.PermiteComentarios = permiteComentario;
                        listaProyectosCambioEstado.Add(Documento.GestorDocumental.ObtenerProyectoID(filaDocVinBR.BaseRecursosID));
                    }
                }
                else
                {
                    Documento.FilaDocumentoWebVinBR.PermiteComentarios = !Documento.FilaDocumentoWebVinBR.PermiteComentarios;
                    listaProyectosCambioEstado.Add(proyectoActuacionID);
                }
                mEntityContext.SaveChanges();
            }
            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, proyectoActuacionID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            if (Documento.Comentarios.Count == 0)
            {
                if (Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                {
                    facetadoCN.ModificarEstadoPreguntaDebate(proyectoActuacionID.ToString(), Documento.Clave.ToString(), "sin respuestas");
                }
                if (Documento.TipoDocumentacion == TiposDocumentacion.Debate)
                {
                    facetadoCN.ModificarEstadoPreguntaDebate(proyectoActuacionID.ToString(), Documento.Clave.ToString(), "sin comentarios");
                }
            }


            if (Documento.PermiteComentarios)
            {
                if (Documento.Comentarios.Count > 0)
                {
                    if (Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                    {
                        facetadoCN.ModificarEstadoPreguntaDebate(proyectoActuacionID.ToString(), Documento.Clave.ToString(), "abierta");

                    }
                    if (Documento.TipoDocumentacion == TiposDocumentacion.Debate)
                    {
                        facetadoCN.ModificarEstadoPreguntaDebate(proyectoActuacionID.ToString(), Documento.Clave.ToString(), "abierto");
                    }
                    if (Documento.TipoDocumentacion == TiposDocumentacion.Encuesta)
                    {
                        facetadoCN.ModificarEstadoPreguntaDebate(proyectoActuacionID.ToString(), Documento.Clave.ToString(), "abierta");
                    }
                }
            }
            else
            {
                if (Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                {
                    facetadoCN.ModificarEstadoPreguntaDebate(proyectoActuacionID.ToString(), Documento.Clave.ToString(), "cerrada");
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.Debate)
                {
                    facetadoCN.ModificarEstadoPreguntaDebate(proyectoActuacionID.ToString(), Documento.Clave.ToString(), "cerrado");
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.Encuesta)
                {
                    facetadoCN.ModificarEstadoPreguntaDebate(proyectoActuacionID.ToString(), Documento.Clave.ToString(), "cerrada");
                }
            }

            foreach (Guid proyectoID in listaProyectosCambioEstado)
            {
                if (!proyectoID.Equals(ProyectoAD.MetaProyecto))
                {
                    ControladorDocumentacion controlador = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);

                    controlador.NotificarCambioEstadoPreguntaDebate(Documento.Clave, proyectoID, (Documento.ProyectoID == proyectoID), Documento.TipoDocumentacion, mAvailableServices);
                }

                #region Actualizar cola GnossLIVE

                int tipo;
                switch (Documento.TipoDocumentacion)
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

                if (proyectoID == ProyectoAD.MetaProyecto)
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

                ControladorDocumentacion.ActualizarGnossLive(proyectoID, Documento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);


                if (proyectoID != ProyectoAD.MetaProyecto)
                {
                    ControladorDocumentacion.ActualizarGnossLive(proyectoID, IdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad(proyectoID), AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);
                }

                #endregion
            }

            //Invalidamos la cache del recurso para que se recargue con el campo "AllowComments"
            ControladorDocumentacion.BorrarCacheControlFichaRecursos(Documento.Clave);

            facetadoCN.Dispose();

            return GnossResultOK();
        }



        private ActionResult CrearComentario(string pDescripcion, Guid pComentarioPadre)
        {
            try
            {
                //Seguridad : 
                /*
                 * Si la comunidad permite comentarios o el documento es una pregunta
                 * Si el documento el la ultima versión y permite comentarios
                */
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);

                CargarComentarios();

                Comentario comentario = null;
                string textoComentario = HttpUtility.UrlDecode(pDescripcion);
                textoComentario = UtilCadenas.LimpiarInyeccionCodigo(textoComentario);

                if (MostrarControlAgregarComentario && (ComunidadPermiteComentarios || Documento.TipoDocumentacion == TiposDocumentacion.Pregunta))
                {
                    ProyectoCN ProyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    string NombreProy = "";
                    if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                    {
                        NombreProy = ProyectoSeleccionado.NombreCorto;
                    }

                    string enlace = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, NombreProy, UrlPerfil, Documento, false);

                    string urlMetaproyecto = ProyectoSeleccionado.UrlPropia(IdiomaUsuario);

                    if (!EsEcosistemaSinMetaProyecto && !ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                    {
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                        urlMetaproyecto = proyCL.ObtenerURLPropiaProyecto(ProyectoAD.MetaProyecto, IdiomaUsuario);
                        if (string.IsNullOrEmpty(urlMetaproyecto))
                        {
                            urlMetaproyecto = UrlIntragnoss;
                        }
                    }

                    if (!urlMetaproyecto.EndsWith("/"))
                    {
                        urlMetaproyecto += "/";
                    }


                    #region Cargamos las identidades de los editores en el proyectoactual

                    List<Guid> listaPerfiles = new List<Guid>();
                    foreach (Elementos.Documentacion.EditorRecurso editor in Documento.ListaPerfilesEditores.Values)
                    {
                        listaPerfiles.Add(editor.FilaEditor.PerfilID);
                    }

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    GestorDocumental.GestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesDePerfilesEnProyecto(listaPerfiles, ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCN.Dispose();

                    List<Guid> listaIdentidades = new List<Guid>();
                    foreach (Guid identidadID in GestorDocumental.GestorIdentidades.ListaIdentidades.Keys)
                    {
                        listaIdentidades.Add(identidadID);
                    }

                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                    GestorDocumental.GestorIdentidades.GestorPersonas = new GestionPersonas(personaCN.ObtenerPersonasPorIdentidad(listaIdentidades), mLoggingService, mEntityContext);
                    personaCN.Dispose();

                    #endregion

                    comentario = GestorDocumental.GestionComentarios.AgregarComentarioDocumento(textoComentario, IdentidadActual, Documento, ProyectoSeleccionado.Clave, enlace, urlMetaproyecto, ProyectoSeleccionado, UtilIdiomas.LanguageCode, mAvailableServices, EsEcosistemaSinMetaProyecto);
                    comentario.URLFotoIdentidad = IdentidadActual.UrlImagen;

                    if (pComentarioPadre != Guid.Empty)
                    {
                        comentario.Padre = GestorDocumental.GestionComentarios.ListaComentarios[pComentarioPadre];
                        comentario.FilaComentario.ComentarioSuperiorID = pComentarioPadre;
                    }

                    if (IdentidadActual.ModoParticipacion == TiposIdentidad.Personal || IdentidadActual.ModoParticipacion == TiposIdentidad.Organizacion)
                    {
                        comentario.FilaComentario.NombreAutor = IdentidadActual.Nombre(IdentidadActual.Clave);
                    }
                    else
                    {
                        comentario.FilaComentario.NombreAutor = IdentidadActual.PerfilUsuario.NombrePersonaEnOrganizacion;
                    }
                    comentario.FilaComentario.TipoPerfil = (short)IdentidadActual.ModoParticipacion;
                    if (IdentidadActual.PerfilUsuario.FilaPerfil.NombreOrganizacion != null)
                    {
                        comentario.FilaComentario.NombreOrganizacion = IdentidadActual.PerfilUsuario.FilaPerfil.NombreOrganizacion;
                    }
                    if (IdentidadActual.ModoParticipacion != TiposIdentidad.Personal && IdentidadActual.ModoParticipacion != TiposIdentidad.Profesor && IdentidadActual.PerfilUsuario.FilaPerfil.OrganizacionID != Guid.Empty)
                    {
                        comentario.FilaComentario.OrganizacionPerfil = IdentidadActual.PerfilUsuario.FilaPerfil.OrganizacionID;
                    }

                    GuardarComentarioAgregado(comentario);

                    ModificarComentarios modelo = new ModificarComentarios(ProyectoSeleccionado.Clave, Documento.Clave, comentario.Clave, pComentarioPadre, IdentidadActual.Persona.Clave, comentario.Fecha);


                    mIPublishEvents.PublishComments(modelo);


                    List<CommentModel> listaComentarios = CargarListaComentarios();

                    MultiViewResult result = new MultiViewResult(this, mViewEngine);

                    int i = 0;
                    foreach (CommentModel fichaComentario in listaComentarios)
                    {
                        result.AddView("_FichaComentario", $"fichaComentariio_{i}", fichaComentario);
                        i++;
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" ComentarioPadreID: {pComentarioPadre} Descripcion: {pDescripcion}");
            }

            return GnossResultERROR();
        }

        private void CargarComentarios()
        {
            if (GestorDocumental.GestionComentarios == null || (GenPlantillasOWL != null && !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarComentarios))
            {
                ComentarioCN comentCN = new ComentarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ComentarioCN>(), mLoggerFactory);
                DataWrapperComentario comentarioDW = comentCN.ObtenerComentariosDeDocumento(Documento.VersionOriginalID, ProyectoSeleccionado.Clave);
                comentCN.Dispose();

                GestorDocumental.GestionComentarios = new GestionComentariosDocumento(comentarioDW, GestorDocumental, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionComentariosDocumento>(), mLoggerFactory);

                VotoCN votoCN = new VotoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
                DataWrapperVoto votoComentariosDW = votoCN.ObtenerVotosComentariosDocumentoPorID(Documento.VersionOriginalID);

                if (GestorDocumental.GestionComentarios.GestorVotos == null)
                {
                    GestorDocumental.GestionComentarios.GestorVotos = new GestorVotosComentario(votoComentariosDW, GestorDocumental.GestionComentarios, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestorVotosComentario>(), mLoggerFactory);
                }
                else
                {
                    GestorDocumental.GestionComentarios.GestorVotos.VotoDW.Merge(votoComentariosDW);
                }
                votoCN.Dispose();
            }
        }

        private List<CommentModel> CargarListaComentarios(int pInicio = 0)
        {
            ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
            List<CommentModel> listaComentariosDoc = null;

            if (!Documento.EsBorrador && ParametrosGeneralesRow.ComentariosDisponibles && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarComentarios))
            {
                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                listaComentariosDoc = (List<CommentModel>)docCL.ObtenerComentariosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);

                if (listaComentariosDoc == null)
                {
                    CargarComentarios();
                    listaComentariosDoc = CargarFichasComentarios(Guid.Empty);
                    docCL.AgregarComentariosRecursoMVC(DocumentoID, ProyectoVirtual.Clave, listaComentariosDoc);
                }

                if (listaComentariosDoc != null && !mControladorBase.UsuarioActual.EsIdentidadInvitada)
                {
                    CargarComentarios();
                    listaComentariosDoc = CargarFichasComentarios(Guid.Empty, pInicio);
                    CargarDatosExtraFichaComentarios(listaComentariosDoc);
                }

                CargarIdentidadesComentarios(listaComentariosDoc);
            }
            return listaComentariosDoc;
        }

        private void CargarIdentidadesComentarios(List<CommentModel> pListaComentarios)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            ObtenerIdentidadesKeyDeFichasComentario(pListaComentarios, listaIdentidades);

            List<string> BaseURLsContent = new List<string>();
            BaseURLsContent.Add(BaseURLContent);
            string nombreSem = PestanyaRecurso.Value;
            string baseUrlBusqueda = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, EsIdentidadBROrg, nombreSem) + "/";

            Dictionary<Guid, ProfileModel> listaIdentidadesPublicadores = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdentidades);

            AgregarPublicadorFichaComentarios(pListaComentarios, listaIdentidadesPublicadores);
        }

        private void AgregarPublicadorFichaComentarios(List<CommentModel> pListaComentarios, Dictionary<Guid, ProfileModel> pListaIdentidadesPublicadores)
        {
            foreach (CommentModel fichaComentario in pListaComentarios)
            {
                if (pListaIdentidadesPublicadores.ContainsKey(fichaComentario.PublisherCard.Key))
                {
                    fichaComentario.PublisherCard = pListaIdentidadesPublicadores[fichaComentario.PublisherCard.Key];
                }

                if (fichaComentario.Replies != null)
                {
                    AgregarPublicadorFichaComentarios(fichaComentario.Replies, pListaIdentidadesPublicadores);
                }
            }
        }

        private void ObtenerIdentidadesKeyDeFichasComentario(List<CommentModel> pListaComentarios, List<Guid> pListaIdentidades)
        {
            foreach (CommentModel fichaComentario in pListaComentarios)
            {
                if (!pListaIdentidades.Contains(fichaComentario.PublisherCard.Key))
                {
                    pListaIdentidades.Add(fichaComentario.PublisherCard.Key);
                }

                if (fichaComentario.Replies != null)
                {
                    ObtenerIdentidadesKeyDeFichasComentario(fichaComentario.Replies, pListaIdentidades);
                }
            }
        }

        private void CargarDatosExtraFichaComentarios(List<CommentModel> pListaComentarios)
        {
            List<CommentModel> listaComentariosEliminar = new List<CommentModel>();
            foreach (CommentModel fichaComentario in pListaComentarios)
            {
                if (!Documento.GestorDocumental.GestionComentarios.ListaComentarios.ContainsKey(fichaComentario.Key))
                {
                    listaComentariosEliminar.Add(fichaComentario);
                    continue;
                }
                Comentario comentario = Documento.GestorDocumental.GestionComentarios.ListaComentarios[fichaComentario.Key];

                string urlDocumento = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false);
                string urlComentario = $"{urlDocumento}/{UtilIdiomas.GetText("URLSEM", "COMENTARIO")}/{comentario.Clave}";

                bool estaVotadoPositivo = false;
                bool estaVotadoNegativo = false;
                bool esPropioAutor = false;

                foreach (AD.EntityModel.Models.Voto.Voto voto in comentario.ListaVotos.Values)
                {
                    if (voto.Voto1 > 0)
                    {
                        if (IdentidadActual.Clave == voto.IdentidadID)
                        {
                            estaVotadoPositivo = true;
                        }
                    }
                    else if (voto.Voto1 < 0)
                    {
                        if (IdentidadActual.Clave == voto.IdentidadID)
                        {
                            estaVotadoNegativo = true;
                        }
                    }
                }
                if (IdentidadActual.ListaTodosIdentidadesDeIdentidad.Contains(comentario.FilaComentario.IdentidadID))
                {
                    esPropioAutor = true;
                }

                ////////////////////////////////////////////////////
                /////////  Este trozo va a desaparecer  ////////////
                ////////////////////////////////////////////////////

                fichaComentario.Votes = new VotesModel();
                fichaComentario.Votes.NumVotes = (int)comentario.GestorComentarios.GestorVotos.ObtenerMedia(comentario);

                fichaComentario.Votes.AllowNegativeVotes = ParametrosGeneralesRow.PermitirVotacionesNegativas;

                fichaComentario.Votes.UrlVotePositive = urlComentario + "/vote-positive";
                fichaComentario.Votes.UrlVoteNegative = urlComentario + "/vote-negative";
                fichaComentario.Votes.UrlDeleteVote = urlComentario + "/delete-vote";

                ////////////////////////////////////////////////////

                fichaComentario.Votes.IsVotedPositive = estaVotadoPositivo;
                fichaComentario.Votes.IsVotedNegative = estaVotadoNegativo;
                fichaComentario.Votes.IsOwnedAuthor = esPropioAutor;

                fichaComentario.Actions = new CommentModel.ActionsModel();

                bool hijosMismoAutor = ComprobarAutorHijos(comentario, comentario.FilaComentario.IdentidadID);
                bool tieneHijos = (comentario.Hijos.Count > 0);


                if ((!tieneHijos || hijosMismoAutor || ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID)) && comentario.TienePermisosIdentidadEliminar(IdentidadActual, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion))
                {
                    fichaComentario.Actions.Delete = true;
                    fichaComentario.Actions.UrlDelete = urlComentario + "/delete";
                }

                if (!tieneHijos && comentario.TienePermisosEdicionIdentidad(IdentidadActual, EsIdentidadActualAdministradorOrganizacion) && MostrarControlAgregarComentario && (ComunidadPermiteComentarios || Documento.TipoDocumentacion == TiposDocumentacion.Pregunta))
                {
                    fichaComentario.Actions.Edit = true;
                    fichaComentario.Actions.UrlEdit = urlComentario + "/edit";
                }

                if (MostrarControlAgregarComentario && ComunidadPermiteComentarios)
                {
                    fichaComentario.Actions.Reply = true;
                    fichaComentario.Actions.UrlReply = urlComentario + "/reply";
                }

                fichaComentario.Actions.UrlVotePositive = urlComentario + "/vote-positive";
                fichaComentario.Actions.UrlVoteNegative = urlComentario + "/vote-negative";
                fichaComentario.Actions.UrlDeleteVote = urlComentario + "/delete-vote";

                if (fichaComentario.Replies != null && fichaComentario.Replies.Count > 0)
                {
                    CargarDatosExtraFichaComentarios(fichaComentario.Replies);
                }
            }

            if (listaComentariosEliminar.Count > 0)
            {
                foreach (CommentModel fichaComentario in listaComentariosEliminar)
                {
                    pListaComentarios.Remove(fichaComentario);
                }

                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.InvalidarComentariosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                docCL.InvalidarEventosRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);
                docCL.Dispose();
            }

        }

        private List<CommentModel> CargarFichasComentarios(Guid pComentarioSupID, int pInicio = 0)
        {
            List<CommentModel> listaComentarios = new List<CommentModel>();

            int pLimite = 100;
            if (pInicio != 0) pLimite = 20;

            List<AD.EntityModel.Models.Comentario.Comentario> filasComentarios;
            if (pComentarioSupID != Guid.Empty)
            {
                filasComentarios = Documento.GestorDocumental.GestionComentarios.ComentarioDW.ListaComentario.Where(item => item.ComentarioSuperiorID.HasValue && item.ComentarioSuperiorID.Value.Equals(pComentarioSupID) && item.Eliminado.Equals(false)).OrderByDescending(item => item.Fecha).ToList();
            }
            else
            {
                filasComentarios = Documento.GestorDocumental.GestionComentarios.ComentarioDW.ListaComentario.Where(item => !item.ComentarioSuperiorID.HasValue && item.Eliminado.Equals(false)).OrderByDescending(item => item.Fecha).Skip(pInicio).Take(pLimite).ToList();
            }

            List<Guid> listaCreadoresComentarios = new List<Guid>();
            Dictionary<Guid, Guid> comentarioCreador = new Dictionary<Guid, Guid>();

            foreach (AD.EntityModel.Models.Comentario.Comentario filaComentario in filasComentarios)
            {
                Comentario comentario = Documento.GestorDocumental.GestionComentarios.ListaComentarios[filaComentario.ComentarioID];
                CommentModel fichaComentario = new CommentModel();
                fichaComentario.Key = comentario.Clave;
                fichaComentario.Title = comentario.Nombre;
                fichaComentario.PublishDate = comentario.Fecha;

                fichaComentario.PublisherCard = CargarPublicadorComentario(comentario);
                fichaComentario.PublisherCard = new ProfileModel();
                fichaComentario.PublisherCard.Key = comentario.FilaComentario.IdentidadID;

                string urlDocumento = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false);
                string urlComentario = urlDocumento + "/" + UtilIdiomas.GetText("URLSEM", "COMENTARIO") + "/" + comentario.Clave;

                fichaComentario.Votes = new VotesModel();
                fichaComentario.Votes.NumVotes = (int)comentario.GestorComentarios.GestorVotos.ObtenerMedia(comentario);

                fichaComentario.Votes.IsVotedPositive = false;
                fichaComentario.Votes.IsVotedNegative = false;
                fichaComentario.Votes.IsOwnedAuthor = false;
                fichaComentario.Votes.AllowNegativeVotes = ParametrosGeneralesRow.PermitirVotacionesNegativas;

                fichaComentario.Votes.UrlVotePositive = urlComentario + "/vote-positive";
                fichaComentario.Votes.UrlVoteNegative = urlComentario + "/vote-negative";
                fichaComentario.Votes.UrlDeleteVote = urlComentario + "/delete-vote";

                fichaComentario.Replies = CargarFichasComentarios(comentario.Clave);

                listaComentarios.Add(fichaComentario);
            }
            return listaComentarios;
        }

        /// <summary>
        /// Comprueba si todas las respuestas de un comentario estan realizadas por la misma persona que escribio el comentario
        /// </summary>
        /// <param name="pComentario">Comentario</param>
        /// <param name="pAutor">GUID del autor</param>
        /// <returns>True si todas las respuestas han sido escritas por el autor del comentario</returns>
        private bool ComprobarAutorHijos(Comentario pComentario, Guid pAutor)
        {
            if (pComentario.Hijos.Count == 0)
            {
                //El comentario no tiene respuestas
                if (pComentario.FilaComentario.IdentidadID == pAutor)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (pComentario.FilaComentario.IdentidadID != pAutor)
            {
                //El comentario tiene respuestas, pero su autor no es el mismo
                return false;
            }
            else
            {
                //El comentario tiene respuestas, pero su autor concuerda
                int i = 0;
                bool mismoAutor = true;

                while ((i < pComentario.Hijos.Count) && mismoAutor)
                {
                    mismoAutor = ComprobarAutorHijos((Es.Riam.Gnoss.Elementos.Comentario.Comentario)pComentario.Hijos[i], pAutor);
                    i++;
                }

                return mismoAutor;
            }
        }

        private ProfileModel CargarPublicadorComentario(Comentario pComentario)
        {
            ProfileModel fichaPublicador = mGnossCache.ObtenerDeCacheLocal($"IdentidadParaComentarios_{pComentario.FilaComentario.IdentidadID}") as ProfileModel;
            if (fichaPublicador != null)
            {
                return fichaPublicador;
            }

            fichaPublicador = new ProfileModel();

            #region URLFoto

            string urlFoto = string.Empty;
            TiposIdentidad tipoIdentidad = TiposIdentidad.Personal;

            if (Documento.GestorDocumental.GestorIdentidades != null && Documento.GestorDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(pComentario.FilaComentario.IdentidadID))
            {
                Identidad ident = Documento.GestorDocumental.GestorIdentidades.ListaIdentidades[pComentario.FilaComentario.IdentidadID];

                urlFoto = ident.UrlImagen;

                tipoIdentidad = ident.Tipo;
            }
            else
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                urlFoto = identCN.ObtenerSiIdentidadTieneFoto(pComentario.FilaComentario.IdentidadID);
                identCN.Dispose();

                tipoIdentidad = (TiposIdentidad)(short)pComentario.FilaComentario.TipoPerfil;
            }

            if (!string.IsNullOrEmpty(urlFoto) && !urlFoto.Equals("sinfoto"))
            {
                fichaPublicador.UrlFoto = "/" + UtilArchivos.ContentImagenes + urlFoto;
            }

            #endregion

            fichaPublicador.TypeProfile = (ProfileType)tipoIdentidad;

            #region Enlace y nombre

            Identidad identAutorComent = null;

            if (Documento.GestorDocumental.GestorIdentidades != null && Documento.GestorDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(pComentario.FilaComentario.IdentidadID))
            {
                identAutorComent = Documento.GestorDocumental.GestorIdentidades.ListaIdentidades[pComentario.FilaComentario.IdentidadID];
            }
            else
            {
                #region Cargamos la identidad del autor del comentario en el gestor

                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                GestionIdentidades gestionMisIdentidades = new GestionIdentidades(idenCN.ObtenerIdentidadPorID(pComentario.FilaComentario.IdentidadID, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                idenCN.Dispose();
                identAutorComent = gestionMisIdentidades.ListaIdentidades[pComentario.FilaComentario.IdentidadID];

                #endregion
            }
            if (GestorDocumental.DataWrapperDocumentacion != null)
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoComentario> filaDocComent = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoComentario.Where(item => item.ComentarioID.Equals(pComentario.Clave)).ToList();

                string nombreProyecto = "";
                string urlPersona = "";

                if (!string.IsNullOrEmpty(ProyectoSeleccionado.NombreCorto) && (ProyectoSeleccionado.Clave != ProyectoPrincipalUnico || !PerfilGlobalEnComunidadPrincipal))
                {
                    nombreProyecto = ProyectoSeleccionado.NombreCorto;
                }

                if (filaDocComent.Count > 0 && filaDocComent[0].ProyectoID == mControladorBase.UsuarioActual.ProyectoID)
                {
                    urlPersona = mControladorBase.UrlsSemanticas.GetURLPerfilPersonaOOrgEnProyecto(BaseURLIdioma, UtilIdiomas, UrlPerfil, identAutorComent, nombreProyecto);
                }
                else
                {
                    urlPersona = mControladorBase.UrlsSemanticas.GetURLPerfilPersonaOOrg(BaseURLIdioma, UtilIdiomas, UrlPerfil, identAutorComent);
                }
            }

            string nombre = "";

            if (pComentario.NombreAutor != null && pComentario.NombreAutor != Es.Riam.Gnoss.Elementos.Comentario.Comentario.NombreUsuarioActual)
            {
                if (GestorDocumental.GestorIdentidades == null || !GestorDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(pComentario.FilaComentario.IdentidadID) || GestorDocumental.GestorIdentidades.ListaIdentidades[pComentario.FilaComentario.IdentidadID].Tipo != TiposIdentidad.Profesor)
                {
                    nombre = pComentario.NombreAutor;
                }
                else
                {
                    nombre = GestorDocumental.GestorIdentidades.ListaIdentidades[pComentario.FilaComentario.IdentidadID].Nombre(IdentidadActual.Clave);
                }
            }
            else if (pComentario.NombreAutor != null && pComentario.NombreAutor == Es.Riam.Gnoss.Elementos.Comentario.Comentario.NombreUsuarioActual)
            {
                nombre = IdentidadActual.Nombre(IdentidadActual.Clave);
            }
            #endregion

            fichaPublicador.NamePerson = nombre;
            //TODO Migrar EF
            fichaPublicador.UrlPerson = mControladorBase.UrlsSemanticas.GetURLPerfilPersonaOOrg(BaseURLIdioma, UtilIdiomas, UrlPerfil, identAutorComent); // urlPersona; urlPersona originariamente cambiar cuando este migrado a EF TODO Migrar EF
            mGnossCache.AgregarObjetoCacheLocal(ProyectoSeleccionado.Clave, $"IdentidadParaComentarios_{pComentario.FilaComentario.IdentidadID}", fichaPublicador, true, DateTime.Now.AddHours(1));

            return fichaPublicador;
        }

        /// <summary>
        /// Elimina todos los hijos de un comentario
        /// </summary>
        /// <param name="pComentario">Comentario</param>
        /// <param name="pDocumento">Documento</param>
        private List<Comentario> EliminarComentariosHijos(Comentario pComentario, Documento pDocumento)
        {
            List<Comentario> hijosEliminados = new List<Comentario>();

            foreach (Comentario hijo in pComentario.Hijos)
            {
                if (hijo.Hijos.Count > 0)
                {
                    hijosEliminados.AddRange(EliminarComentariosHijos(hijo, pDocumento));
                    hijo.LimpiarHijos();
                }
                Guid hijoID = hijo.Clave;
                GestorDocumental.GestionComentarios.EliminarComentario(hijo, pDocumento);

                hijosEliminados.Add(hijo);
            }

            return hijosEliminados;
        }

        /// <summary>
        /// Guardar los votos
        /// </summary>
        private void GuardarVotosComentarios()
        {
            mEntityContext.SaveChanges();

            DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            docCL.InvalidarComentariosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
            docCL.InvalidarEventosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
            docCL.Dispose();
        }

        /// <summary>
        /// Guardar los comentarios
        /// </summary>
        private void GuardarComentarios()
        {
            if (GestorDocumental.DataWrapperDocumentacion != null)
            {
                mEntityContext.SaveChanges();
            }
        }

        private void GuardarComentarioAgregado(Comentario pComentario)
        {
            GuardarComentarios();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            try
            {
                //Actualizo el número de comentarios del documento
                //Se suma UNO, Deberian Contarse los comentarios que hay.
                docCN.ActualizarNumeroComentariosDocumento(Documento.Clave);
                docCN.Dispose();

                ControladorDocumentacion.AgregarComentarioModeloBaseSimple(pComentario.Clave, ProyectoSeleccionado.Clave, 1, PrioridadBase.Alta, mAvailableServices);

                TipoLive tipo = TipoLive.Recurso;

                if (Documento.TipoDocumentacion == TiposDocumentacion.Debate)
                {
                    tipo = TipoLive.Debate;
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                {
                    tipo = TipoLive.Pregunta;
                }

                ControladorDocumentacion.ActualizarGnossLive(ProyectoSeleccionado.Clave, pComentario.Clave, AccionLive.ComentarioAgregado, (int)tipo, PrioridadLive.Alta, mAvailableServices);

            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
            finally
            {
                if (docCN != null)
                {
                    docCN.Dispose();
                }
            }

            if (Documento.Comentarios.Count == 0 && (Documento.TipoDocumentacion == TiposDocumentacion.Debate || Documento.TipoDocumentacion == TiposDocumentacion.Pregunta))
            {
                //Es el primer comentario, cambio el estado de la pregunta o debate
                ControladorDocumentacion controlador = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                controlador.NotificarCambioEstadoPreguntaDebate(Documento.Clave, ProyectoSeleccionado.Clave, ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Publico) || ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Restringido), Documento.TipoDocumentacion, mAvailableServices);
            }

            DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            docCL.InvalidarFichaRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
            docCL.InvalidarComentariosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
            docCL.InvalidarEventosRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);
            docCL.Dispose();
        }

        private void ActualizarNumeroComentariosRecurso(List<AD.EntityModel.Models.Comentario.Comentario> pListaComentarios)
        {
            try
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                docCN.ActualizarNumeroComentariosDocumento(Documento.Clave);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                throw ex;
            }
        }

        private void GuardarComentariosEliminadosOld(List<Comentario> pListaComentarios)
        {
            GuardarComentarios();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            try
            {
                foreach (Comentario comentario in pListaComentarios)
                {
                    //Actualizo el número de comentarios del documento
                    //Se resta UNO, Deberian Contarse los comentarios que hay.
                    docCN.ActualizarNumeroComentariosDocumento(Documento.Clave);

                    ControladorDocumentacion.AgregarComentarioModeloBaseSimple(comentario.Clave, ProyectoSeleccionado.Clave, 1, PrioridadBase.Alta, true, mAvailableServices);

                    TipoLive tipo = TipoLive.Recurso;

                    switch (Documento.TipoDocumentacion)
                    {
                        case TiposDocumentacion.Debate:
                            tipo = TipoLive.Debate;
                            break;
                        case TiposDocumentacion.Pregunta:
                            tipo = TipoLive.Pregunta;
                            break;
                    }

                    ControladorDocumentacion.ActualizarGnossLive(ProyectoSeleccionado.Clave, comentario.Clave, AccionLive.ComentarioEliminado, (int)tipo, PrioridadLive.Alta, mAvailableServices);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
            finally
            {
                if (docCN != null)
                {
                    docCN.Dispose();
                }
            }

            if (Documento.Comentarios.Count == 0 && (Documento.TipoDocumentacion == TiposDocumentacion.Debate || Documento.TipoDocumentacion == TiposDocumentacion.Pregunta))
            {
                //Han eliminado el único comentario, cambio el estado de la pregunta o debate
                ControladorDocumentacion controlador = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                controlador.NotificarCambioEstadoPreguntaDebate(Documento.Clave, ProyectoSeleccionado.Clave, ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Publico) || ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Restringido), Documento.TipoDocumentacion, mAvailableServices);
            }

            DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            docCL.InvalidarFichaRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
            docCL.InvalidarComentariosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
            docCL.InvalidarEventosRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);
            docCL.Dispose();
        }

        #endregion

        #region Votos

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LoadVoters()
        {
            if (GestorDocumental.DataWrapperDocumentacion != null)
            {
                try
                {

                    //Seguridad : Usuario conectado y participando en la comunidad
                    //Seguridad : Que las votaciones esten disponibles, que tenga votos y que se puedan ver los votos
                    bool tieneVotos = GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Any(item => item.DocumentoID.Equals(Documento.FilaDocumento.DocumentoID) && item.ProyectoID.Value.Equals(ProyectoSeleccionado.Clave));

                    if (ParametrosGeneralesRow.VotacionesDisponibles && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarVotosDoc) && ParametrosGeneralesRow.VerVotaciones && tieneVotos)
                    {
                        return GnossResultHtml("_FichaVotantes", CargarFichaVotos(true));
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                }
            }

            return GnossResultERROR();
        }

        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult VotePositive()
        {
            try
            {
                if (Request.Method.Equals("GET"))
                {
                    return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, IdentidadActual.EsOrganizacion));
                }
                else if (Request.Method.Equals("POST"))
                {
                    //Seguridad : Que las votaciones esten disponibles
                    if (ParametrosGeneralesRow.VotacionesDisponibles && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarVotosDoc))
                    {
                        if (Documento.GestorDocumental.GestorVotos == null)
                        {
                            VotoCN votoCN = new VotoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
                            Documento.GestorDocumental.GestorVotos = new GestionVotosDocumento(votoCN.ObtenerVotosDocumentoPorID(Documento.Clave), Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionVotosDocumento>(), mLoggerFactory);
                            votoCN.Dispose();
                        }

                        List<AD.EntityModel.Models.Voto.Voto> filasVotos = Documento.GestorDocumental.GestorVotos.VotoDW.ListaVotos.Where(item => item.IdentidadID.Equals(IdentidadActual.Clave)).ToList();

                        if (GestorDocumental.DataWrapperDocumentacion != null)
                        {
                            //Eliminamos el voto anterior (si lo hay)
                            if (filasVotos.Count > 0)
                            {
                                foreach (AD.EntityModel.Models.Voto.Voto filaVoto in filasVotos)
                                {
                                    List<AD.EntityModel.Models.Documentacion.VotoDocumento> filasVotoDoc = Documento.GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Where(item => item.VotoID.Equals(filaVoto.VotoID)).ToList();
                                    foreach (AD.EntityModel.Models.Documentacion.VotoDocumento filaVotoDoc in filasVotoDoc)
                                    {
                                        Documento.GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Remove(filaVotoDoc);
                                        mEntityContext.EliminarElemento(filaVotoDoc);
                                    }

                                    Documento.ListaVotos.Remove(filaVoto.VotoID);

                                    mEntityContext.EliminarElemento(filaVoto);
                                    Documento.GestorDocumental.GestorVotos.VotoDW.ListaVotos.Remove(filaVoto);
                                }
                            }
                        }


                        Documento.GestorDocumental.GestorVotos.RecargarVotos();

                        //Agregamos el voto nuevo
                        AD.EntityModel.Models.Voto.Voto voto = Documento.GestorDocumental.GestorVotos.AgregarVoto(VotoDocumento.Positivo, mControladorBase.UsuarioActual.IdentidadID, Documento.Clave, Documento.CreadorID, Documento, ProyectoSeleccionado.Clave);

                        //Actualiza el número de votos del recurso.
                        Documento.FilaDocumentoWebVinBR.NumeroVotos = (int)Documento.GestorDocumental.GestorVotos.ObtenerMediaProyecto(Documento, ProyectoSeleccionado.Clave);
                        Documento.FilaDocumento.Valoracion = Documento.GestorDocumental.GestorVotos.ObtenerMedia(Documento);
                        if (GestorDocumental.DataWrapperDocumentacion != null)
                        {
                            mEntityContext.SaveChanges();
                        }


                        if (voto != null)
                        {
                            int tipo;

                            switch (Documento.TipoDocumentacion)
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

                            //Se envia una fila al Live para actualizar la valoración de los documentos.
                            ControladorDocumentacion.ActualizarGnossLive(ProyectoSeleccionado.Clave, voto.VotoID, AccionLive.Votado, tipo, PrioridadLive.Alta, mAvailableServices);
                        }

                        //Actualización Offline a partir de un servicio UDP
                        //Llamada asincrona para actualizar la popularidad del recurso:
                        ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("Votos", Documento.Clave, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.IdentidadID, Documento.CreadorID);

                        DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                        docCL.InvalidarFichaRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                        docCL.InvalidarEventosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                        docCL.Dispose();

                        return GnossResultHtml("_FichaVotos", CargarFichaVotos(false));
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }
            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult VoteNegative()
        {
            try
            {
                //Seguridad : Que las votaciones esten disponibles
                if (ParametrosGeneralesRow.VotacionesDisponibles && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarVotosDoc))
                {
                    if (Documento.GestorDocumental.GestorVotos == null)
                    {
                        VotoCN votoCN = new VotoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
                        Documento.GestorDocumental.GestorVotos = new GestionVotosDocumento(votoCN.ObtenerVotosDocumentoPorID(Documento.Clave), Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionVotosDocumento>(), mLoggerFactory);
                        votoCN.Dispose();
                    }

                    List<AD.EntityModel.Models.Voto.Voto> filasVotos = Documento.GestorDocumental.GestorVotos.VotoDW.ListaVotos.Where(item => item.IdentidadID.Equals(IdentidadActual.Clave)).ToList();

                    //Eliminamos el voto anterior (si lo hay)
                    if (filasVotos.Count > 0)
                    {
                        if (GestorDocumental.DataWrapperDocumentacion != null)
                        {
                            foreach (AD.EntityModel.Models.Voto.Voto filaVoto in filasVotos)
                            {
                                List<AD.EntityModel.Models.Documentacion.VotoDocumento> filasVotoDoc = Documento.GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Where(item => item.VotoID.Equals(filaVoto.VotoID)).ToList();
                                foreach (AD.EntityModel.Models.Documentacion.VotoDocumento filaVotoDoc in filasVotoDoc)
                                {
                                    Documento.GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Remove(filaVotoDoc);
                                    mEntityContext.VotoDocumento.Remove(filaVotoDoc);
                                }

                                Documento.ListaVotos.Remove(filaVoto.VotoID);

                                mEntityContext.EliminarElemento(filaVoto);
                                Documento.GestorDocumental.GestorVotos.VotoDW.ListaVotos.Remove(filaVoto);
                            }
                        }

                    }

                    Documento.GestorDocumental.GestorVotos.RecargarVotos();

                    //Agregamos el voto nuevo
                    AD.EntityModel.Models.Voto.Voto voto = Documento.GestorDocumental.GestorVotos.AgregarVoto(VotoDocumento.Negativo, mControladorBase.UsuarioActual.IdentidadID, Documento.Clave, Documento.CreadorID, Documento, ProyectoSeleccionado.Clave);


                    Documento.FilaDocumentoWebVinBR.NumeroVotos = (int)Documento.GestorDocumental.GestorVotos.ObtenerMediaProyecto(Documento, ProyectoSeleccionado.Clave);
                    Documento.FilaDocumento.Valoracion = Documento.GestorDocumental.GestorVotos.ObtenerMedia(Documento);
                    if (GestorDocumental.DataWrapperDocumentacion != null)
                    {
                        mEntityContext.SaveChanges();
                        if (voto != null)
                        {
                            //Procesado en el servicio live Extra de manera Offline
                            Documento.FilaDocumento.Valoracion = Documento.GestorDocumental.GestorVotos.ObtenerMedia(Documento);
                            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                            Documento.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ActualizarValoracionDocumento(Documento.Clave));
                            docCN.Dispose();
                        }
                    }


                    //Actualización Offline a partir de un servicio UDP
                    //Llamada asincrona para actualizar la popularidad del recurso:
                    ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("Votos", Documento.Clave, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.IdentidadID, Documento.CreadorID);

                    DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    docCL.InvalidarFichaRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                    docCL.InvalidarEventosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                    docCL.Dispose();

                    return GnossResultHtml("_FichaVotos", CargarFichaVotos(false));
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }
            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult DeleteVote()
        {
            try
            {
                //Seguridad : Que las votaciones esten disponibles
                if (ParametrosGeneralesRow.VotacionesDisponibles && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarVotosDoc))
                {
                    if (Documento.GestorDocumental.GestorVotos == null)
                    {
                        VotoCN votoCN = new VotoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
                        Documento.GestorDocumental.GestorVotos = new GestionVotosDocumento(votoCN.ObtenerVotosDocumentoPorID(Documento.Clave), Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionVotosDocumento>(), mLoggerFactory);
                        votoCN.Dispose();
                    }

                    List<AD.EntityModel.Models.Voto.Voto> filasVotos = Documento.GestorDocumental.GestorVotos.VotoDW.ListaVotos.Where(item => item.IdentidadID.Equals(IdentidadActual.Clave)).ToList();
                    if (GestorDocumental.DataWrapperDocumentacion != null)
                    {
                        if (filasVotos.Count > 0)
                        {
                            foreach (AD.EntityModel.Models.Voto.Voto filaVoto in filasVotos)
                            {
                                List<AD.EntityModel.Models.Documentacion.VotoDocumento> filasVotoDoc = Documento.GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Where(item => item.VotoID.Equals(filaVoto.VotoID)).ToList();
                                foreach (AD.EntityModel.Models.Documentacion.VotoDocumento filaVotoDoc in filasVotoDoc)
                                {
                                    Documento.GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Remove(filaVotoDoc);
                                    mEntityContext.EliminarElemento(filaVotoDoc);

                                }

                                Documento.ListaVotos.Remove(filaVoto.VotoID);

                                mEntityContext.EliminarElemento(filaVoto);
                                Documento.GestorDocumental.GestorVotos.VotoDW.ListaVotos.Remove(filaVoto);
                            }
                        }

                        Documento.GestorDocumental.GestorVotos.RecargarVotos();

                        Documento.FilaDocumentoWebVinBR.NumeroVotos = (int)Documento.GestorDocumental.GestorVotos.ObtenerMediaProyecto(Documento, ProyectoSeleccionado.Clave);
                        mEntityContext.SaveChanges();

                    }
                    //Eliminamos el voto anterior (si lo hay)


                    //Actualización Offline a partir de un servicio UDP
                    //Llamada asincrona para actualizar la popularidad del recurso:
                    ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("Votos", Documento.Clave, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.IdentidadID, Documento.CreadorID);

                    DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    docCL.InvalidarFichaRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                    docCL.InvalidarEventosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                    docCL.Dispose();

                    return GnossResultHtml("_FichaVotos", CargarFichaVotos(false));
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }
            return GnossResultERROR();
        }

        private VotesModel CargarFichaVotos(bool pCargarFichaAmpliada)
        {
            if (GestorDocumental.DataWrapperDocumentacion != null)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                docCN.ObtenerVotosDocumento(Documento.Clave, Documento.GestorDocumental.DataWrapperDocumentacion);
                docCN.Dispose();
            }


            if (Documento.GestorDocumental.GestorVotos == null || this.Documento.FilaDocumento.VotoDocumento.Count > 0)
            {
                VotoCN votoCN = new VotoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
                Documento.GestorDocumental.GestorVotos = new GestionVotosDocumento(votoCN.ObtenerVotosDocumentoPorID(Documento.Clave), Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionVotosDocumento>(), mLoggerFactory);
                votoCN.Dispose();
            }
            Documento.GestorDocumental.GestorVotos.RecargarVotos();

            VotesModel fichaVotos = new VotesModel();

            if (Documento.ListaVotosPorComunidad(ProyectoSeleccionado.Clave).Count > 0)
            {
                //Cargamos los votos positivos
                #region Cargamos los votos

                List<Guid> listaIdentidadesPositivas = new List<Guid>();
                List<Guid> listaIdentidadesNegativas = new List<Guid>();
                List<Guid> listaIdentidades = new List<Guid>();

                int numVotosMostrar = 6;
                int numVoto = 0;
                foreach (AD.EntityModel.Models.Voto.Voto voto in Documento.ListaVotosPorComunidad(ProyectoSeleccionado.Clave).Values)
                {
                    if (voto.Voto1 > 0)
                    {
                        listaIdentidadesPositivas.Add(voto.IdentidadID);
                    }
                    else if (voto.Voto1 < 0)
                    {
                        listaIdentidadesNegativas.Add(voto.IdentidadID);
                    }
                    listaIdentidades.Add(voto.IdentidadID);

                    numVoto++;
                    if (numVoto == numVotosMostrar)
                    {
                        break;
                    }
                }

                #endregion

                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                GestionIdentidades gestorIden = new GestionIdentidades(idenCN.ObtenerIdentidadesPorID(listaIdentidades, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                idenCN.Dispose();

                #region Votos

                if (listaIdentidades.Count > 0)
                {
                    fichaVotos.Voters = new List<VotesModel.VoterModel>();
                    foreach (Guid identidadID in listaIdentidades)
                    {
                        if (gestorIden.ListaIdentidades.ContainsKey(identidadID))
                        {
                            Identidad identidadVotador = gestorIden.ListaIdentidades[identidadID];
                            int voto = 1;
                            if (!listaIdentidadesPositivas.Contains(identidadID))
                            {
                                voto = -1;
                            }

                            VotesModel.VoterModel fichaVotante = new VotesModel.VoterModel();
                            fichaVotante.Name = identidadVotador.Nombre(identidadID);

                            if (pCargarFichaAmpliada)
                            {
                                string urlIdentidad = "";
                                if (identidadVotador.FilaIdentidad.ProyectoID == ProyectoSeleccionado.Clave)
                                {
                                    if (ProyectoSeleccionado.NombreCorto != null && ProyectoSeleccionado.NombreCorto != "")
                                    {
                                        urlIdentidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/";
                                    }
                                    else if (UrlPerfil != null)
                                    {
                                        urlIdentidad = BaseURLIdioma + UrlPerfil;
                                    }

                                    if (identidadVotador.NombreOrganizacion != "")
                                    {
                                        urlIdentidad += mControladorBase.UrlsSemanticas.ObtenerURLOrganizacionOClase(UtilIdiomas, identidadVotador.OrganizacionID.Value) + "/" + identidadVotador.PerfilUsuario.NombreCortoOrg + "/";
                                    }

                                    if (identidadVotador.Tipo == TiposIdentidad.ProfesionalCorporativo && IdentidadActual.OrganizacionID.HasValue && identidadVotador.OrganizacionID == IdentidadActual.OrganizacionID)
                                    {
                                        urlIdentidad += UtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + identidadVotador.PerfilUsuario.NombreCortoUsu;
                                    }
                                    else if (identidadVotador.Tipo != TiposIdentidad.ProfesionalCorporativo)
                                    {
                                        urlIdentidad += UtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + identidadVotador.PerfilUsuario.NombreCortoUsu;
                                    }
                                    fichaVotante.Url = urlIdentidad;
                                }

                                string urlFotoIdentidad = identidadVotador.FilaIdentidad.Foto;
                                string urlImagen = "";
                                if (!string.IsNullOrEmpty(urlFotoIdentidad) && !urlFotoIdentidad.Equals("sinfoto"))
                                {
                                    urlImagen = BaseURLContent + "/" + UtilArchivos.ContentImagenes + urlFotoIdentidad;
                                }
                                else if (identidadVotador.TrabajaConOrganizacion)
                                {
                                    urlImagen = BaseURLContent + "/imgagenes/Organizaciones/anonimo_peque.png";
                                }
                                else
                                {
                                    urlImagen = BaseURLContent + "/imagenes/Personas/anonimo_peque.png";
                                }

                                fichaVotante.Image = urlImagen;
                            }
                            fichaVotante.Vote = voto;

                            fichaVotos.Voters.Add(fichaVotante);
                        }
                    }
                }
                #endregion
            }

            if (!pCargarFichaAmpliada)
            {
                bool estaVotadoPositivo = false;
                bool estaVotadoNegativo = false;

                int numVotosPositivos = 0;
                int numVotosNegativos = 0;

                bool esPropioAutor = false;

                foreach (AD.EntityModel.Models.Voto.Voto voto in Documento.ListaVotosPorComunidad(ProyectoSeleccionado.Clave).Values)
                {
                    if (voto.IdentidadID != UsuarioAD.Invitado)
                    {
                        if (voto.Voto1 > 0)
                        {
                            numVotosPositivos++;
                            if (IdentidadActual.Clave == voto.IdentidadID)
                            {
                                estaVotadoPositivo = true;
                            }
                        }
                        else if (voto.Voto1 < 0)
                        {
                            numVotosNegativos++;
                            if (IdentidadActual.Clave == voto.IdentidadID)
                            {
                                estaVotadoNegativo = true;
                            }
                        }
                    }
                }
                if (IdentidadActual.ListaTodosIdentidadesDeIdentidad.Contains(Documento.CreadorID))
                {
                    esPropioAutor = true;
                }

                if (GestorDocumental.DataWrapperDocumentacion != null)
                {
                    bool tieneVotos = GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Any(item => item.Equals(Documento.FilaDocumento.DocumentoID) && item.ProyectoID.Value.Equals(ProyectoSeleccionado.Clave));

                    bool montarControlVotantes = false;

                    if (!mControladorBase.UsuarioActual.EsIdentidadInvitada && ParametrosGeneralesRow.VotacionesDisponibles && ParametrosGeneralesRow.VerVotaciones && tieneVotos)
                    {
                        montarControlVotantes = true;
                    }
                    fichaVotos.ShowVoters = montarControlVotantes;
                }


                fichaVotos.NumPositiveVotes = numVotosPositivos;
                fichaVotos.NumNegativeVotes = numVotosNegativos;
                fichaVotos.IsVotedPositive = estaVotadoPositivo;
                fichaVotos.IsVotedNegative = estaVotadoNegativo;
                fichaVotos.IsOwnedAuthor = esPropioAutor;

                fichaVotos.AllowNegativeVotes = ParametrosGeneralesRow.PermitirVotacionesNegativas;

                if (Documento.FilaDocumentoWebVinBR != null)
                {
                    fichaVotos.NumVotes = Documento.FilaDocumentoWebVinBR.NumeroVotos;
                }

                string urlDocumento = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false);

                fichaVotos.UrlVotePositive = urlDocumento + "/vote-positive";
                fichaVotos.UrlVoteNegative = urlDocumento + "/vote-negative";
                fichaVotos.UrlDeleteVote = urlDocumento + "/delete-vote";
            }

            return fichaVotos;
        }

        #endregion

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LoadHistory()
        {
            try
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                List<AD.EntityModel.Models.Documentacion.VersionDocumento> listaVersionesDocumento = docCN.ObtenerVersionesPorDocumentoOriginalID(Documento.VersionOriginalID);
                Dictionary<Guid,ResourceModel> recursos = ControladorProyectoMVC.ObtenerRecursosPorID(listaVersionesDocumento.Select(x => x.DocumentoID).ToList(), "", null, false);

                docCN.Dispose();

                List<VersionViewModel> versiones = new List<VersionViewModel>();
                List<AD.EntityModel.Models.Documentacion.VersionDocumento> parentVersions = CalcularVersionesPadre(listaVersionesDocumento);
                
                foreach (AD.EntityModel.Models.Documentacion.VersionDocumento versionDocumento in parentVersions)
                {
                    
                    VersionViewModel version = new VersionViewModel
                    {
                        Number = versionDocumento.Version,
                        Title = recursos[versionDocumento.DocumentoID].Title,
                        Publisher = recursos[versionDocumento.DocumentoID].Publisher.NamePerson,
                        PublishDate = recursos[versionDocumento.DocumentoID].PublishDate,
                        VersionId = versionDocumento.DocumentoID,
                        StatusId = versionDocumento.EstadoID,
                        IsImprovement = versionDocumento.EsMejora,
                        VersionStatus = (EstadoVersion)versionDocumento.EstadoVersion,
                        Url = recursos[versionDocumento.DocumentoID].VersionCardLink,
                        UrlPreview = recursos[versionDocumento.DocumentoID].UrlPreview,
                        UrlLoadActionRestoreVersion = recursos[versionDocumento.DocumentoID].ListActions.UrlLoadActionRestoreVersion,
                        UrlLoadActionDeleteVersion = recursos[versionDocumento.DocumentoID].ListActions.UrlLoadActionDeleteVersion,
                        IsLastVersion = versionDocumento.EstadoVersion == (short)EstadoVersion.Vigente,
                        IsSemantic = recursos[versionDocumento.DocumentoID].TypeDocument == DocumentType.Semantico,
                        IsServerFile = recursos[versionDocumento.DocumentoID].TypeDocument == DocumentType.FicheroServidor,
                        ImprovementSubversions = CalcularSubversiones(versionDocumento.MejoraID,listaVersionesDocumento, recursos)
                    };

                    versiones.Add(version);
                }


                HistoryViewModel model = new HistoryViewModel();
                model.Versions = versiones;
                model.LastVersionStatus = model.Versions.First(x => x.IsLastVersion).StatusId;
                model.ImprovementStatus = model.Versions.FirstOrDefault(x => x.IsImprovement && x.VersionStatus == EstadoVersion.Pendiente)?.StatusId;
                model.HasWorkflow = model.LastVersionStatus != null;
                model.HasActiveImprovement = model.Versions.Any(x => x.IsImprovement && x.VersionStatus == EstadoVersion.Pendiente);
                model.IsActiveImprovement = model.Versions.Any(x => x.VersionId == DocumentoVersionID && x.VersionStatus == EstadoVersion.Pendiente);


                return GnossResultHtml("../FichaRecurso/_modal-views/_history", model);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                return GnossResultERROR("No se pudo cargar el historial");
            }
        }

        [HttpGet]
        [TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult DownloadSharepoint(string RecursoID)
        {
            /*ActionResult redireccion = FuncionalidadSharepoint();
            if (redireccion != null)
            {
                return redireccion;
            }*/
            string enlace = Documento.Enlace;
            string[] auxEnlace = enlace.Split("|||");
            string extension = string.Empty;
            string nombre = string.Empty;
            if (auxEnlace.Length > 1)
            {
                string nombreCompleto = auxEnlace[1];
                extension = Path.GetExtension(nombreCompleto);
                nombre = Path.GetFileNameWithoutExtension(nombreCompleto);
            }
            else
            {
                ActionResult redireccion = FuncionalidadSharepoint(true);
                if (redireccion != null)
                {
                    return redireccion;
                }
                SharepointController spController = new SharepointController(RecursoID, mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);
                spController.Token = tokenSP;
                extension = spController.ExtraerExtensionFicheroAPI(enlace);
                nombre = spController.ExtraerNombreSinExtension(enlace);
                DocumentacionCN documentoCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dwDocumentacion = documentoCN.ObtenerDocumentoPorID(new Guid(RecursoID));
                dwDocumentacion.ListaDocumento[0].Enlace = $"{enlace}|||{nombre}{extension}";
                AD.EntityModel.Models.Documentacion.Documento doc = dwDocumentacion.ListaDocumento[0];

                mEntityContext.Documento.Update(doc);
                mEntityContext.SaveChanges();
                //El tipo se guarda en caché, así que la borramos
                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.InvalidarFichaRecursoMVC(new Guid(RecursoID), (Guid)dwDocumentacion.ListaDocumento[0].ProyectoID);
            }


            GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
            gd.Url = UrlServicioWebDocumentacion;
            byte[] bytesFichero = gd.ObtenerDocumento(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, Documento.OrganizacionID, Documento.ProyectoID, new Guid(RecursoID), extension);
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();

            return File(bytesFichero, provider.Mappings[extension], $"{nombre}{extension}");
        }

        [HttpGet, HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        [TypeFilter(typeof(PermisosRecursos), Arguments = new object[] { new ulong[] { (ulong)PermisoRecursos.EditarRecursoTipoEnlace } })]
        public ActionResult DesvincularSharepoint(string pDocumentoID)
        {
            try
            {
                DocumentacionCN documentoCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dwDocumentacion = documentoCN.ObtenerDocumentoPorID(new Guid(pDocumentoID));
                ParametroAplicacionCN parametroCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrive = parametroCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                bool esDocumentoSharepoint = UtilCadenas.EsEnlaceSharepoint(Documento.Enlace, oneDrive);
                string sharepointConfigurado = parametroCN.ObtenerParametroAplicacion("SharepointClientID");
                SharepointController spController = new SharepointController(Documento.Clave.ToString(), mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);
                string nombre = string.Empty;

                if (string.IsNullOrEmpty(sharepointConfigurado))
                {
                    esDocumentoSharepoint = false;
                }
                if (esDocumentoSharepoint)
                {
                    try
                    {
                        ActionResult redirect = FuncionalidadSharepoint();
                        if (redirect != null)
                        {
                            return redirect;
                        }
                        spController.Token = tokenSP;
                        spController.TokenCreadorRecurso = tokenSPAdminDocument;
                        nombre = spController.ObtenerNombreFichero(dwDocumentacion.ListaDocumento[0].Enlace);
                        bool eliminacionCorrecta = spController.EliminarDocumentoDeSharepoint(Documento.Enlace);
                        if (!eliminacionCorrecta)
                        {
                            mLoggingService.GuardarLogError($"No se ha podido eliminar el documento del recurso {Documento.Clave} de SharePoint al tratar de desvincular el recurso. Enlace del documento: {Documento.Enlace}", mlogger);
                            return GnossResultERROR($"Error al eliminar el documento de SharePoint al desvincular el recurso");
                        }
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, $"Error al eliminar el documento {Documento.Clave} de SharePoint al intentar desvincular el recurso. Enlace del documento: {Documento.Enlace}", mlogger);
                        return GnossResultERROR($"Error al eliminar el documento de SharePoint al desvincular el recurso");
                    }
                }

                //cambiamos el Tipo de Hipervínculo a Adjunto

                dwDocumentacion.ListaDocumento[0].Tipo = (short)TiposDocumentacion.FicheroServidor;

                //Obtenemos el nombre del fichero para ponerlo como Enlace y cambiamos la fecha de modificacion

                dwDocumentacion.ListaDocumento[0].Enlace = nombre;
                dwDocumentacion.ListaDocumento[0].FechaModificacion = DateTime.Now;
                AD.EntityModel.Models.Documentacion.Documento doc = dwDocumentacion.ListaDocumento[0];
                mEntityContext.Documento.Update(doc);
                mEntityContext.SaveChanges();

                //El tipo se guarda en caché, así que la borramos
                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.InvalidarFichaRecursoMVC(new Guid(pDocumentoID), (Guid)dwDocumentacion.ListaDocumento[0].ProyectoID);

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                return GnossResultERROR(ex.Message);
            }
        }

        [HttpGet, HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult VincularDocumentoSharepoint(string pEnlace, string pDocumentoID)
        {
            try
            {
                // Comprobamos si esta configurado SharePoint
                ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string sharepointConfigurado = paramCN.ObtenerParametroAplicacion("SharepointClientID");

                if (string.IsNullOrEmpty(sharepointConfigurado))
                {
                    return GnossResultERROR("SharePoint no esta vinculado para este entorno.");
                }

                ActionResult redireccion = FuncionalidadSharepoint(false, pEnlace);

                if (redireccion != null)
                {
                    return redireccion;
                }

                // Comprobamos si el enlace pasado es valido
                string oneDrivePermitido = paramCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);

                if (!UtilCadenas.EsEnlaceSharepoint(pEnlace, oneDrivePermitido))
                {
                    return GnossResultERROR("El enlace seleccionado no corresponde a SharePoint/OneDrive o no esta permitido enlazar documentos con OneDrive");
                }

                // Cambiamos el tipo de documento de Adjunto a Hipervinculo
                DocumentacionCN documentoCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dwDocumentacion = documentoCN.ObtenerDocumentoPorID(new Guid(pDocumentoID));
                dwDocumentacion.ListaDocumento[0].Tipo = (short)TiposDocumentacion.Hipervinculo;
                dwDocumentacion.ListaDocumento[0].FechaModificacion = DateTime.Now;

                // Guardarmos el fichero de SharePoint en el servidor
                SharepointController spController = new SharepointController(pDocumentoID, mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);
                spController.Token = tokenSP;
                string nombreFichero = spController.GuardarArchivoDesdeAPI(pEnlace);

                if (!pEnlace.Contains("|||"))
                {
                    dwDocumentacion.ListaDocumento[0].Enlace = $"{pEnlace}|||{nombreFichero}";
                }
                else
                {
                    dwDocumentacion.ListaDocumento[0].Enlace = pEnlace;
                }

                AD.EntityModel.Models.Documentacion.Documento doc = dwDocumentacion.ListaDocumento[0];
                mEntityContext.Documento.Update(doc);
                mEntityContext.SaveChanges();

                // El tipo de documento se guarda en cache, asi que la borramos
                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.InvalidarFichaRecursoMVC(new Guid(pDocumentoID), (Guid)doc.ProyectoID);

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                return GnossResultERROR($"Ha ocurrido un error al tratar de vincular el documento con SharePoint: {ex.Message}");
            }
        }

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LoadSendLinkAction()
        {
            try
            {
                string rutaEnlace = ViewBag.UrlPagina;
                rutaEnlace = rutaEnlace.Substring(0, rutaEnlace.LastIndexOf("/load-action"));
                if (UsuarioActual.EsUsuarioInvitado)
                {
                    if (EsBot)
                    {
                        return RedirectPermanent(rutaEnlace);
                    }
                    else
                    {
                        return new RedirectResult($"{GnossUrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto)}/{UtilIdiomas.GetText("URLSEM", "LOGIN")}/redirect/{new Uri((string)ViewBag.UrlPagina).AbsolutePath}");
                    }
                }

                ControladorAmigos contrAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory);
                contrAmigos.CargarAmigos(IdentidadActual, true, mAvailableServices);

                SendLinkViewModel paginaModel = new SendLinkViewModel();

                paginaModel.LinkUrl = rutaEnlace;
                paginaModel.LinkName = Documento.Titulo;

                return GnossResultHtml("../FichaRecurso/_modal-views/_send", paginaModel);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                throw;
            }
        }

        #region Vinculados

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LoadLinkedResources(int page = 1)
        {
            try
            {
                //Solo Pintado
                ResourceModel FichaRecurso = new ResourceModel();
                FichaRecurso.Key = Documento.Clave;

                int numRecursosRelaccionados = 0;
                FichaRecurso.RelatedResources = CargarListaVinculados(page, out numRecursosRelaccionados);
                FichaRecurso.NumRelatedResources = numRecursosRelaccionados;
                ViewBag.UrlPagina = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false);

                return GnossResultHtml("_FichaVinculados", FichaRecurso);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                throw;
            }
        }

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LinkResource(string UrlResourceLink)
        {
            try
            {
                //Seguridad : Si no es borrador, es la ultima version
                if (!Documento.EsBorrador && Documento.FilaDocumento.UltimaVersion && ProyectoOrigenBusquedaID == Guid.Empty && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonVincularDoc))
                {
                    return AccionRecurso_Vincular_Aceptar(UrlResourceLink);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" UrlResourceLink: {UrlResourceLink}");
            }
            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult UnLinkResource(Guid ResourceUnLinkKey)
        {
            //Seguridad : Dentro
            return AccionRecurso_DesVincular_Aceptar(ResourceUnLinkKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult ReportPage(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "REPORTAR_RECURSO_ERROR") + UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "REPORTAR_RECURSO_DESCRIPCION_VACIA"));
                }

                //Seguridad : Si no es borrador, es la ultima version
                if (!Documento.EsBorrador && Documento.FilaDocumento.UltimaVersion && ProyectoOrigenBusquedaID == Guid.Empty)
                {
                    EnviarReporte(message);

                    return GnossResultOK(UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "REPORTAR_RECURSO_OK"));
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, " ReportPage ");
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "REPORTAR_RECURSO_ERROR") + UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "REPORTAR_RECURSO_REPETIR_ACCION"));
            }
            return GnossResultERROR(UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "REPORTAR_RECURSO_ERROR"));
        }

        private void EnviarReporte(string message)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            List<string> listaEmailsAdministradores = proyCN.ObtenerEmailsAdministradoresDeProyecto(ProyectoSeleccionado.Clave);

            if (listaEmailsAdministradores.Count > 0)
            {
                string nombreIdentidad = IdentidadActual.Nombre(IdentidadActual.Clave);
                string urlIdentidad = mControladorBase.UrlsSemanticas.GetURLPerfilDeIdentidad(BaseURL, ProyectoSeleccionado.NombreCorto, UtilIdiomas, IdentidadActual);

                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                GestionNotificaciones GestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);

                string nombreEnlace = Documento.Titulo;
                string urlEnlace = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false);

                GestorNotificaciones.AgregarNotificacionReporteRecurso(listaEmailsAdministradores, message, nombreIdentidad, urlIdentidad, nombreEnlace, urlEnlace, UtilIdiomas.LanguageCode, ProyectoSeleccionado);
                notificacionCN.ActualizarNotificacion(mAvailableServices);
                notificacionCN.Dispose();
            }
        }


        /// <summary>
        /// Cuenta una descarga en un recurso concreto
        /// </summary>
        /// <returns></returns>
        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult DownloadCounter()
        {
            try
            {
                Guid? baseRecursosID = CargarBaseRecursos();
                if (baseRecursosID.HasValue)
                {
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    docCN.ActualizarNumeroDescargasDocumento(DocumentoID, baseRecursosID.Value);

                    DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    ResourceModel resourceModel = docCL.ObtenerFichaRecursoMVC(DocumentoID, ProyectoSeleccionado.Clave);

                    if (resourceModel != null)
                    {
                        resourceModel.NumDownloads = Documento.FilaDocumentoWebVinBRExtra.NumeroDescargas + 1;
                        docCL.AgregarFichaRecursoMVC(DocumentoID, ProyectoSeleccionado.Clave, resourceModel);
                    }

                    return GnossResultOK((Documento.FilaDocumentoWebVinBRExtra.NumeroDescargas + 1).ToString());
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return GnossResultERROR();
        }

        private List<ResourceModel> CargarListaVinculados(int pPagina, out int pNumVinculados)
        {
            pNumVinculados = 0;

            int inicio = ((pPagina - 1) * 5) + 1;
            int fin = 10;
            if (pPagina > 1)
            {
                fin = ((pPagina - 1) * 5) + 5;
            }

            Dictionary<Guid, List<Guid>> listaClavesRecursosProyecto = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, Elementos.ServiciosGenerales.Proyecto> listaProyectosRecursos = new Dictionary<Guid, Elementos.ServiciosGenerales.Proyecto>();
            List<ParametroGeneral> listaParametrosGeneralesProyectos = new List<ParametroGeneral>();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            if (docCN.TieneDocumentoDocumentosVinculados(Documento.Clave))
            {
                listaClavesRecursosProyecto = docCN.ObtenerListaDocumentosVinculadosDocumento(GestorDocumental.DataWrapperDocumentacion, ProyectoSeleccionado.Clave, Documento.Clave, IdentidadActual.PerfilID, inicio, fin, out pNumVinculados);

            }
            docCN.Dispose();

            List<ResourceModel> listaVinculados = new List<ResourceModel>();

            if (listaClavesRecursosProyecto.Any())
            {
                List<string> BaseURLsContent = new List<string>();
                BaseURLsContent.Add(BaseURLContent);

                if (listaClavesRecursosProyecto.Count > 1 || !listaClavesRecursosProyecto.First().Equals(ProyectoSeleccionado.Clave))
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    GestionProyecto gestProy = new GestionProyecto(proyCN.ObtenerProyectosPorID(listaClavesRecursosProyecto.Keys.ToList()), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                    listaProyectosRecursos = gestProy.ListaProyectos;

                    ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCN>(), mLoggerFactory);
                    listaParametrosGeneralesProyectos = paramCN.ObtenerParametrosGeneralesDeListaDeProyectos(listaClavesRecursosProyecto.Keys.ToList());
                }

                Dictionary<Guid, ResourceModel> listaRecursosCache = new Dictionary<Guid, ResourceModel>();

                foreach (Guid proyectoID in listaClavesRecursosProyecto.Keys)
                {
                    Elementos.ServiciosGenerales.Proyecto proy = ProyectoSeleccionado;
                    ParametroGeneral paramGenerales = ParametrosGeneralesRow;

                    string nombreSem = PestanyaRecurso.Value;

                    if (!proyectoID.Equals(ProyectoSeleccionado.Clave))
                    {
                        proy = listaProyectosRecursos[proyectoID];
                        paramGenerales = listaParametrosGeneralesProyectos.FirstOrDefault(fila => fila.ProyectoID.Equals(proyectoID));

                        nombreSem = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    }

                    string urlBusqueda = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, EsIdentidadBROrg, nombreSem) + "/";

                    Guid proyectoOrigenID = ObtenerProyectoOrigenFichaRecurso(Documento.Clave, ProyectoSeleccionado.Clave);

                    ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLsContent, BaseURLStatic, proy, proyectoOrigenID, paramGenerales, IdentidadActual, mControladorBase.EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);

                    listaRecursosCache = listaRecursosCache.Concat(controladorMVC.ObtenerRecursosPorID(listaClavesRecursosProyecto[proyectoID], urlBusqueda, null, false)).ToDictionary(k => k.Key, v => v.Value);
                }

                foreach (ResourceModel vinculado in listaRecursosCache.Values)
                {
                    if (!mControladorBase.UsuarioActual.EsIdentidadInvitada && Documento.GestorDocumental.ListaDocumentosWeb.ContainsKey(vinculado.Key))
                    {
                        DocumentoWeb documentoVinc = Documento.GestorDocumental.ListaDocumentosWeb[vinculado.Key];
                        if (GestorDocumental.TienePermisosIdentidadDesvincularRecursos(Documento, documentoVinc, IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion))
                        {
                            if (vinculado.Actions == null)
                            {
                                vinculado.Actions = new ActionsModel();
                            }
                            vinculado.Actions.UnLinkUp = true;

                            vinculado.ListActions.UrlUnLinkResource = vinculado.CompletCardLink + "/unlink-resource";
                            vinculado.ListActions.UrlLoadLinkedResources = vinculado.CompletCardLink + "/load-linked-resources";
                        }
                    }
                    listaVinculados.Add(vinculado);
                }
            }

            return listaVinculados;
        }

        private Guid ObtenerProyectoOrigenFichaRecurso(Guid pDocumentoID, Guid pProyectoActualID)
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

        private GnossResult AccionRecurso_Vincular_Aceptar(string pUrlVincular)
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
                    bool urlEncontrada = false;
                    foreach (string urlPropia in listaUrlPropia)
                    {
                        if (urlDoc.Contains(urlPropia))
                        {
                            urlEncontrada = true;
                            break;
                        }
                    }

                    //Si no se comparte desde una url propia ni se comparte desde gnoss
                    if (!urlEncontrada && !urlDoc.Contains(BaseURLSinHTTP))
                    {
                        throw new Exception("Mala url");
                    }

                    string id = urlDoc.Substring(urlDoc.LastIndexOf("/") + 1);
                    Guid documentoVincID = new Guid(id);

                    string urlComunidad = urlDoc.Substring(0, urlDoc.LastIndexOf("/"));//gnoss.com/comunidad/nombre_com/recurso/nombre_recurso
                    urlComunidad = urlComunidad.Substring(0, urlComunidad.LastIndexOf("/"));//gnoss.com/comunidad/nombre_com/recurso
                    urlComunidad = urlComunidad.Substring(0, urlComunidad.LastIndexOf("/"));//gnoss.com/comunidad/nombre_com
                    urlComunidad = urlComunidad.Substring(urlComunidad.LastIndexOf("/") + 1);//nombre_com

                    Guid proyID = proyCN.ObtenerProyectoIDPorNombre(urlComunidad);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();
                    docCN.ObtenerDocumentoPorIDCargarTotal(documentoVincID, docDW, true, false, null);
                    // Hay que vincularlo con el documentoid de la ultima version si es posible
                    Guid documentoUlimaVersionID;
                    if (docDW.ListaVersionDocumento.Count > 0)
                    {
                        documentoUlimaVersionID = docDW.ListaVersionDocumento.OrderByDescending(item => item.Version).FirstOrDefault().DocumentoID;
                    }
                    else
                    {
                        //En caso de que el documento no tenga versiones
                        documentoUlimaVersionID = docDW.ListaDocumento.FirstOrDefault().DocumentoID;
                    }


                    DataWrapperDocumentacion docUltimaVersionDW = new DataWrapperDocumentacion();
                    docCN.ObtenerDocumentoPorIDCargarTotal(documentoUlimaVersionID, docUltimaVersionDW, true, false, null);
                    documentoVincID = documentoUlimaVersionID != Guid.Empty ? documentoUlimaVersionID : documentoVincID;
                    bool estaDocumentoVinculado = docCN.EstaVinculadoDocumento(DocumentoID, documentoVincID);
                    docCN.Dispose();

                    if (documentoVincID == Documento.Clave)
                    {
                        mensajeError = UtilIdiomas.GetText("VINCULACIONDOCUMENTACION", "ERRORDOCVINCMISMO");
                    }
                    else if (docUltimaVersionDW.ListaDocumento.First().Eliminado || !docUltimaVersionDW.ListaDocumento.First().UltimaVersion || docUltimaVersionDW.ListaDocumento.First().Borrador || !docUltimaVersionDW.ListaDocumentoWebVinBaseRecursos.Any(doc => !doc.Eliminado))
                    {
                        mensajeError = UtilIdiomas.GetText("VINCULACIONDOCUMENTACION", "ERRORDOCELIMINADO");
                    }
                    else
                    {
                        if (estaDocumentoVinculado)
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
                            GestorDocumental.VincularDocumentos(DocumentoID, documentoVincID, identidadVinculacion);

                            mEntityContext.SaveChanges();

                            ControladorDocumentacion.ActualizarGnossLivePopularidad(proyID, mControladorBase.UsuarioActual.IdentidadID, documentoVincID, AccionLive.VincularRecursoaRecurso, (int)TipoLive.Miembro, (int)TipoLive.Recurso, true, PrioridadLive.Alta, mAvailableServices);

                            try
                            {
                                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                                docCL.InvalidarVinculadosRecursoMVC(documentoVincID, ProyectoVirtual.Clave);
                                docCL.InvalidarVinculadosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                                docCL.Dispose();

                                return GnossResultOK();
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError($"{ex.Message}\\n{ex.StackTrace}");
                                throw;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    mensajeError = UtilIdiomas.GetText("VINCULACIONDOCUMENTACION", "ERRORURLMALA", NombreProyectoEcosistema);
                }
            }

            return GnossResultERROR(mensajeError);
        }

        private GnossResult AccionRecurso_DesVincular_Aceptar(Guid pDocVinculadoID)
        {
            try
            {
                //Seguridad : si Tiene Permisos la Identidad para Desvincular estos Recursos
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                Guid docVinculadoUltimaVersion = docCN.ObtenerUltimaVersionDeDocumento(pDocVinculadoID);

                DataWrapperDocumentacion dwDocumentacion = new DataWrapperDocumentacion();
                docCN.ObtenerDocumentoPorIDCargarTotal(docVinculadoUltimaVersion, dwDocumentacion, true, true, null);
                dwDocumentacion.Merge(docCN.ObtenerVinculacionesRecurso(docVinculadoUltimaVersion));

                GestorDocumental gesDoc = new GestorDocumental(dwDocumentacion, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

                DocumentoWeb documentoVinc = gesDoc.ListaDocumentosWeb[docVinculadoUltimaVersion];

                if (GestorDocumental.TienePermisosIdentidadDesvincularRecursos(documentoVinc, Documento, IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion))
                {
                    //DesVincular
                    bool correcto = docCN.DesvincularRecursos(DocumentoID, docVinculadoUltimaVersion);

                    if (correcto)
                    {
                        DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                        docCL.InvalidarVinculadosRecursoMVC(docVinculadoUltimaVersion, ProyectoVirtual.Clave);
                        docCL.InvalidarVinculadosRecursoMVC(DocumentoID, ProyectoVirtual.Clave);
                        docCL.Dispose();

                        return GnossResultOK();
                    }
                }

                docCN.Dispose();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" ResourceUnLinkKey: {pDocVinculadoID}");
            }
            return GnossResultERROR();
        }

        #endregion

        #region Compartir, Guardar espacio personal

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult AddToPersonalSpace(string[] categoriesList)
        {
            try
            {
                // Seguridad : 
                /*
                 * Si no es borrador y es la ultima version
                 * Si el documento permite compartir y la comunidad permite compartir y no es el metaproyecto
                 * Si no es debate, ni pregunta, ni encuesta, ni documento semantico, ni newsletter
                 * Si no esta ya compartido en mi base de recursos
                */
                if (!Documento.EsBorrador && Documento.FilaDocumento.UltimaVersion && Documento.CompartirPermitido && ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && ParametrosGeneralesRow.CompartirRecursosPermitido && (!Documento.FilaDocumentoWebVinBR.PrivadoEditores || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)))
                {
                    if (Documento.TipoDocumentacion != TiposDocumentacion.Debate && Documento.TipoDocumentacion != TiposDocumentacion.Pregunta && Documento.TipoDocumentacion != TiposDocumentacion.Encuesta && Documento.TipoDocumentacion != TiposDocumentacion.Newsletter)
                    {
                        DataWrapperDocumentacion docDSAux = new DataWrapperDocumentacion();
                        DocumentacionCN docCNAux = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                        docCNAux.ObtenerBaseRecursosUsuario(docDSAux, mControladorBase.UsuarioActual.UsuarioID);
                        if (!Documento.BaseRecursos.Contains(docDSAux.ListaBaseRecursosUsuario.First().BaseRecursosID))
                        {
                            Dictionary<Guid, List<Guid>> listaBasesRecursos = new Dictionary<Guid, List<Guid>>();

                            List<Guid> listaCategorias = new List<Guid>();
                            foreach (string cat in categoriesList)
                            {
                                if (cat != "")
                                {
                                    listaCategorias.Add(new Guid(cat));
                                }
                            }

                            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                            Guid brPersonal = docCN.ObtenerBaseRecursosIDUsuario(mControladorBase.UsuarioActual.UsuarioID);

                            listaBasesRecursos.Add(brPersonal, listaCategorias);

                            return AccionRecurso_Compartir_Aceptar(listaBasesRecursos, new List<Guid>(), new List<Guid>(), new List<Guid>(), new List<Guid>(), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (categoriesList != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(categoriesList);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion AddToPersonalSpace");
                }
                GuardarLogError(ex, $" categoriesList: {modeloDatos}");
            }

            return new EmptyResult();
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult AddToPersonalSpacePrivate()
        {
            try
            {
                // Seguridad : 
                /*
                 * Si no es borrador y es la ultima version
                 * Si el documento permite compartir y la comunidad permite compartir y no es el metaproyecto
                 * Si no es debate, ni pregunta, ni encuesta, ni documento semantico, ni newsletter
                 * Si no esta ya compartido en mi base de recursos
                */
                if (!Documento.EsBorrador && Documento.FilaDocumento.UltimaVersion && Documento.CompartirPermitido && ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && ParametrosGeneralesRow.CompartirRecursosPermitido && (!Documento.FilaDocumentoWebVinBR.PrivadoEditores || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)))
                {
                    if (Documento.TipoDocumentacion != TiposDocumentacion.Debate && Documento.TipoDocumentacion != TiposDocumentacion.Pregunta && Documento.TipoDocumentacion != TiposDocumentacion.Encuesta && Documento.TipoDocumentacion != TiposDocumentacion.Newsletter)
                    {
                        DataWrapperDocumentacion docDWAux = new DataWrapperDocumentacion();
                        DocumentacionCN docCNAux = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                        docCNAux.ObtenerBaseRecursosUsuario(docDWAux, mControladorBase.UsuarioActual.UsuarioID);
                        if (!Documento.BaseRecursos.Contains(docDWAux.ListaBaseRecursosUsuario.First().BaseRecursosID))
                        {
                            Dictionary<Guid, List<Guid>> listaBasesRecursos = new Dictionary<Guid, List<Guid>>();

                            List<CategoryModel> listaTesouroPerfil = CargarTesauroPerfil();

                            List<Guid> listaCategorias = new List<Guid>();
                            foreach (CategoryModel cat in listaTesouroPerfil)
                            {
                                listaCategorias.Add(cat.Key);
                            }

                            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                            Guid brPersonal = docCN.ObtenerBaseRecursosIDUsuario(mControladorBase.UsuarioActual.UsuarioID);

                            listaBasesRecursos.Add(brPersonal, listaCategorias);

                            return AccionRecurso_Compartir_Aceptar(listaBasesRecursos, new List<Guid>(), new List<Guid>(), new List<Guid>(), new List<Guid>(), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, " categoriesList: Private");
            }

            return new EmptyResult();
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult AddCategoryToPersonalSpace(string categoryName, Guid parentCategoryID)
        {
            try
            {
                TesauroCN tesCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
                GestionTesauro gestorTesauroBRPersonal = new GestionTesauro(tesCN.ObtenerTesauroUsuario(mControladorBase.UsuarioActual.UsuarioID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);

                //Comprobamos si el nombre está repetido
                bool nombreRepetido = false;
                List<IElementoGnoss> catReps = null;
                catReps = gestorTesauroBRPersonal.ListaCategoriasTesauro[parentCategoryID].Hijos;
                foreach (IElementoGnoss catTes in catReps)
                {
                    if (((CategoriaTesauro)catTes).Nombre[IdiomaUsuario].ToLower() == categoryName.ToLower())
                    {
                        nombreRepetido = true;
                    }
                }

                if (nombreRepetido)
                {
                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_NOMBRE_DIFERENTE"));
                }
                else if (string.IsNullOrEmpty(categoryName))
                {
                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_NOMBRE"));
                }

                Guid IDNuevaCategoria = Guid.NewGuid();
                CategoriaTesauro categoriaPadre = gestorTesauroBRPersonal.ListaCategoriasTesauro[parentCategoryID];
                CategoriaTesauro categoriaNueva = gestorTesauroBRPersonal.AgregarSubcategoria(categoriaPadre, categoryName.Trim());
                gestorTesauroBRPersonal.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaInferiorID.Equals(categoriaNueva.Clave)).FirstOrDefault().CategoriaInferiorID = IDNuevaCategoria;
                gestorTesauroBRPersonal.TesauroDW.ListaCategoriaTesauro.Where(item => item.CategoriaTesauroID.Equals(categoriaNueva.Clave)).FirstOrDefault().CategoriaTesauroID = IDNuevaCategoria;

                tesCN.ActualizarTesauro();

                return LoadAction("add-personal-space", IDNuevaCategoria);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" categoryName: {categoryName} parentCategoryID: {parentCategoryID}");
                throw;
            }
        }

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult Share(string[] Categories, string[] Editors, string[] Readers, bool Private)
        {
            try
            {
                // Seguridad : 
                /*
                    * Si no es borrador y es la ultima version
                    * Si el documento permite compartir y la comunidad permite compartir
                    * Si no es debate, ni pregunta, ni encuesta, ni documento semantico, ni newsletter
                */
                bool esOntologia = (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Ontologia) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.OntologiaSecundaria));

                if (!Documento.EsBorrador && (Documento.FilaDocumento.UltimaVersion || esOntologia) && Documento.CompartirPermitido && ParametrosGeneralesRow.CompartirRecursosPermitido && (!Documento.FilaDocumentoWebVinBR.PrivadoEditores || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)))
                {
                    if (Documento.TipoDocumentacion != TiposDocumentacion.Debate && Documento.TipoDocumentacion != TiposDocumentacion.Pregunta && Documento.TipoDocumentacion != TiposDocumentacion.Encuesta && Documento.TipoDocumentacion != TiposDocumentacion.Newsletter)
                    {
                        Dictionary<Guid, List<Guid>> listaBasesRecursos = new Dictionary<Guid, List<Guid>>();
                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                        foreach (string brCat in Categories)
                        {
                            string[] brCatSplit = brCat.Split(new string[] { "$$" }, StringSplitOptions.None);
                            string br = brCatSplit[0];
                            string[] cats = brCatSplit[1].Split(',');
                            List<Guid> listaCategorias = new List<Guid>();
                            foreach (string cat in cats)
                            {
                                if (cat != "")
                                {
                                    listaCategorias.Add(new Guid(cat));
                                }
                            }

                            if (!Guid.TryParse(br, out Guid brID))
                            {
                                Guid proyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(br);
                                brID = proyCN.ObtenerBaseRecursosProyectoPorProyectoID(proyectoID);
                            }

                            listaBasesRecursos.Add(brID, listaCategorias);
                        }
                        proyCN.Dispose();

                        List<Guid> listaEditores = new List<Guid>();
                        List<Guid> listaGrupoEditores = new List<Guid>();
                        if (Editors != null)
                        {
                            foreach (string editor in Editors)
                            {
                                if (!editor.Equals(string.Empty))
                                {
                                    if (editor.StartsWith("g_"))
                                    {
                                        Guid grupoID = Guid.Empty;
                                        if (Guid.TryParse(editor.Substring(2), out grupoID))
                                        {
                                            listaGrupoEditores.Add(grupoID);
                                        }
                                    }
                                    else
                                    {
                                        listaEditores.Add(new Guid(editor));
                                    }
                                }
                            }
                        }

                        List<Guid> listaLectores = new List<Guid>();
                        List<Guid> listaGrupoLectores = new List<Guid>();
                        if (Readers != null)
                        {
                            foreach (string lector in Readers)
                            {
                                if (!lector.Equals(string.Empty))
                                {
                                    if (lector.StartsWith("g_"))
                                    {
                                        Guid grupoID = Guid.Empty;
                                        if (Guid.TryParse(lector.Substring(2), out grupoID))
                                        {
                                            listaGrupoLectores.Add(grupoID);
                                        }
                                    }
                                    else
                                    {
                                        listaLectores.Add(new Guid(lector));
                                    }
                                }
                            }
                        }

                        return AccionRecurso_Compartir_Aceptar(listaBasesRecursos, listaEditores, listaGrupoEditores, listaLectores, listaGrupoLectores, Private);
                    }
                }
            }
            catch (Exception ex)
            {
                string modeloCategories = "";
                string modeloEditors = "";
                string modeloReaders = "";
                try
                {
                    if (Categories != null)
                    {
                        modeloCategories = JsonConvert.SerializeObject(Categories);
                    }
                    if (Editors != null)
                    {
                        modeloEditors = JsonConvert.SerializeObject(Editors);
                    }
                    if (Readers != null)
                    {
                        modeloReaders = JsonConvert.SerializeObject(Readers);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion Share");
                }
                GuardarLogError(ex, $" Categories: {modeloCategories} Editors: {modeloEditors} Readers: {modeloReaders} Private: {Private}");
            }
            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult Unshare(Guid BR, Guid ProyectID, Guid OrganizationID)
        {
            try
            {
                //Seguridad : Que la identidad actual tenga permisos para eliminar el recurso en esta Base de recursos
                if (Documento.TienePermisosIdentidadEliminarRecursoEnBR(IdentidadActual, BR, ProyectoSeleccionado, UsuarioActual.UsuarioID, UsuarioActual.ProyectoID, EsIdentidadActualAdministradorOrganizacion, EsIdentidadActualSupervisorProyecto))
                {
                    DesCompartirRecurso(BR, ProyectID, OrganizationID);
                }
                return GnossResultOK();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" BR: {BR} ProyectID: {ProyectID} OrganizationID: {OrganizationID}");
                throw;
            }
        }

        private GnossResult AccionRecurso_Compartir_Aceptar(Dictionary<Guid, List<Guid>> pListaBasesRecursos, List<Guid> pListaEditores, List<Guid> pListaGrupoEditores, List<Guid> pListaLectores, List<Guid> pListaGrupoLectores, bool pVisibleSoloEditores)
        {
            try
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                IdentidadActual.GestorIdentidades.SetDataWrapperIdentidad(identidadCN.ObtenerPerfilesDePersona(IdentidadActual.PersonaID.Value, true, IdentidadActual.Clave, true));
                GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;

                foreach (Guid editorID in pListaEditores)
                {
                    if (!Documento.ListaPerfilesEditoresSinLectores.ContainsKey(editorID))
                    {
                        GestorDocumental.AgregarEditorARecurso(Documento.Clave, editorID);
                    }
                }
                foreach (Guid grupoEditorID in pListaGrupoEditores)
                {
                    if (!Documento.ListaGruposEditoresSinLectores.ContainsKey(grupoEditorID))
                    {
                        GestorDocumental.AgregarGrupoEditorARecurso(Documento.Clave, grupoEditorID);
                    }
                }
                foreach (Guid lectorID in pListaLectores)
                {
                    if (!Documento.ListaPerfilesEditores.ContainsKey(lectorID))
                    {
                        GestorDocumental.AgregarLectorARecurso(Documento.Clave, lectorID);
                    }
                }
                foreach (Guid grupoLectorID in pListaGrupoLectores)
                {
                    if (!Documento.ListaGruposEditores.ContainsKey(grupoLectorID))
                    {
                        GestorDocumental.AgregarGrupoLectorARecurso(Documento.Clave, grupoLectorID);
                    }
                }

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

                AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocWebVinBROrigen = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.FirstOrDefault(doc => doc.DocumentoID.Equals(Documento.Clave) && doc.BaseRecursosID.Equals(GestorDocumental.BaseRecursosIDActual));

                List<Guid> listaProyectosActualizarNumProy = new List<Guid>();

                bool esOntologia = (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Ontologia) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.OntologiaSecundaria));

                foreach (Guid baseRecursosID in pListaBasesRecursos.Keys)
                {
                    if (pListaBasesRecursos[baseRecursosID].Count > 0 || esOntologia)
                    {
                        List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

                        TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);

                        GestionTesauro gestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroPorBaseRecursosID(baseRecursosID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);

                        gestorTesauro.CargarCategorias();
                        gestorTesauro.CargarGestor();

                        List<CategoriaTesauro> categoriasNoMarcadas = gestorTesauro.ComprobarCategoriasObligatoriasMarcadas(pListaBasesRecursos[baseRecursosID]);

                        if (Documento.TipoDocumentacion != TiposDocumentacion.Semantico && Documento.TipoDocumentacion != TiposDocumentacion.Ontologia && Documento.TipoDocumentacion != TiposDocumentacion.OntologiaSecundaria && (pListaBasesRecursos[baseRecursosID].Count == 0 || categoriasNoMarcadas.Count > 0))
                        {
                            string error = null;

                            switch (Documento.TipoDocumentacion)
                            {
                                case TiposDocumentacion.Pregunta:
                                    error = $"errorCategories|{UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA_PREGUNTA")}";
                                    break;
                                case TiposDocumentacion.Debate:
                                    error = $"errorCategories|{UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA_DEBATE")}";
                                    break;
                                case TiposDocumentacion.Encuesta:
                                    error = $"errorCategories|{UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA_ENCUESTA")}";
                                    break;
                                default:
                                    if (categoriasNoMarcadas.Count > 0)
                                    {
                                        error = $"errorCategories|{UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA_OBLIGATORIA", categoriasNoMarcadas[0].Nombre[IdiomaUsuario])}";
                                    }
                                    else
                                    {
                                        error = $"errorCategories|{UtilIdiomas.GetText("PERFILBASESUBIRRECURSO", "ERROR_DCATEGORIA")}";
                                    }
                                    break;
                            }

                            return GnossResultERROR(error);
                        }

                        foreach (Guid catTes in pListaBasesRecursos[baseRecursosID])
                        {
                            listaCategorias.Add(gestorTesauro.ListaCategoriasTesauro[catTes]);
                        }

                        Guid proyectoID;
                        Guid identidadID;
                        Guid organizacionID;

                        proyCN.ObtenerDatosPorBaseRecursosPersona(baseRecursosID, IdentidadActual.PersonaID.Value, out proyectoID, out identidadID, out organizacionID);

                        if (identidadID == Guid.Empty)
                        {
                            // Se está compartiendo el recurso en la Base de Recursos del Usuario
                            identidadID = IdentidadActual.IdentidadPersonalMyGNOSS.Clave;

                            if (IdentidadActual.IdentidadPersonalMyGNOSS.OrganizacionID.HasValue)
                            {
                                organizacionID = IdentidadActual.IdentidadPersonalMyGNOSS.OrganizacionID.Value;
                            }
                        }

                        List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(baseRecursosID) && doc.Eliminado && doc.DocumentoID.Equals(Documento.Clave)).ToList();
                        bool modificando = false;

                        if (filaDocVinBR.Count > 0)
                        {
                            filaDocVinBR[0].Eliminado = false;
                            filaDocVinBR[0].IdentidadPublicacionID = identidadID;
                            filaDocVinBR[0].FechaPublicacion = DateTime.Now;
                            modificando = true;

                            if (proyectoID != Guid.Empty)
                            {
                                GestorDocumental.AgregarComparticionDocumentoHistorial(Documento, proyectoID, identidadID);
                            }
                            else
                            {
                                GestorDocumental.AgregarComparticionDocumentoHistorial(Documento, ProyectoAD.MetaProyecto, identidadID);
                            }
                        }
                        else if (!Documento.BaseRecursos.Contains(baseRecursosID))
                        {
                            //Si ya esta compartido en esta BR no lo hago
                            GestorDocumental.VincularDocumentoABaseRecursos(Documento, baseRecursosID, TipoPublicacion.Compartido, identidadID, false);

                            if (proyectoID != Guid.Empty)
                            {
                                GestorDocumental.AgregarComparticionDocumentoHistorial(Documento, proyectoID, identidadID);
                            }
                            else
                            {
                                GestorDocumental.AgregarComparticionDocumentoHistorial(Documento, ProyectoAD.MetaProyecto, identidadID);
                            }
                        }

                        if (proyectoID != Guid.Empty)
                        {
                            listaProyectosActualizarNumProy.Add(proyectoID);
                        }
                        else if (!listaProyectosActualizarNumProy.Contains(ProyectoAD.MetaProyecto))
                        {
                            listaProyectosActualizarNumProy.Add(ProyectoAD.MetaProyecto);
                        }


                        if (!modificando)
                        {
                            GestorDocumental.VincularDocumentoACategorias(listaCategorias, Documento, baseRecursosID, proyectoID, identidadID);
                        }
                        else
                        {
                            List<CategoriaTesauro> listaCategoriasCompartir = new List<CategoriaTesauro>();

                            foreach (CategoriaTesauro categoria in listaCategorias)
                            {
                                List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasDocVinAgCat = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.BaseRecursosID.Equals(baseRecursosID) && doc.DocumentoID.Equals(Documento.Clave) && doc.CategoriaTesauroID.Equals(categoria.Clave)).ToList();

                                if (filasDocVinAgCat.Count == 0)
                                {
                                    listaCategoriasCompartir.Add(categoria);
                                }
                            }
                            GestorDocumental.VincularDocumentoACategorias(listaCategoriasCompartir, Documento, baseRecursosID, proyectoID, identidadID);
                        }

                        AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocWebVinBRDestino = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Find(doc => doc.DocumentoID.Equals(Documento.Clave) && doc.BaseRecursosID.Equals(baseRecursosID));

                        filaDocWebVinBRDestino.PrivadoEditores = filaDocWebVinBROrigen.PrivadoEditores || pVisibleSoloEditores;
                        filaDocWebVinBRDestino.TipoPublicacion = (short)TipoPublicacion.Compartido;

                        if (identidadID == IdentidadActual.IdentidadMyGNOSS.Clave)
                        {
                            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                            controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);
                        }
                        else
                        {
                            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);

                            // Identidad proyecto actual
                            controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);

                            if (IdentidadActual.GestorIdentidades.ListaIdentidades.ContainsKey(identidadID))
                            {
                                // Identidad proyecto destino
                                controladorPersonas.ActivoEnComunidad(IdentidadActual.GestorIdentidades.ListaIdentidades[identidadID], mAvailableServices);
                            }
                        }

                        //InsertarScriptBuscador en la tabla ProyectoConfigExtraSem
                        if (esOntologia && Documento.TipoDocumentacion.Equals(TiposDocumentacion.OntologiaSecundaria))
                        {
                            ProyectoConfigExtraSem filaConfigExtraSem = mEntityContext.ProyectoConfigExtraSem.Where(proy => proy.ProyectoID.Equals(ProyectoSeleccionado.Clave) && proy.UrlOntologia.Equals(Documento.Enlace) && proy.SourceTesSem.Equals(Documento.Clave.ToString())).FirstOrDefault();
                            if (filaConfigExtraSem != null)
                            {
                                ProyectoConfigExtraSem proyectoConfigExtraSem = new ProyectoConfigExtraSem();
                                proyectoConfigExtraSem.ProyectoID = proyectoID;
                                proyectoConfigExtraSem.UrlOntologia = Documento.Enlace;
                                proyectoConfigExtraSem.SourceTesSem = Documento.Clave.ToString();
                                proyectoConfigExtraSem.Tipo = (short)TipoConfigExtraSemantica.EntidadSecundaria;
                                proyectoConfigExtraSem.Nombre = Documento.Titulo;
                                proyectoConfigExtraSem.Editable = true;
                                mEntityContext.ProyectoConfigExtraSem.Add(proyectoConfigExtraSem);
                            }
                        }
                    }
                }

                GuardarCompartir();

                foreach (Guid proyectoID in listaProyectosActualizarNumProy)
                {
                    if (proyectoID != ProyectoAD.MetaProyecto)
                    {
                        proyCN.ActualizarNumeroDocumentacion(proyectoID);
                    }

                    #region Actualizar cola GnossLIVE

                    int tipo;
                    switch (Documento.TipoDocumentacion)
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

                    if (proyectoID == ProyectoAD.MetaProyecto)
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

                    ControladorDocumentacion.ActualizarGnossLive(proyectoID, Documento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);


                    if (proyectoID != ProyectoAD.MetaProyecto)
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

                    ControladorDocumentacion.ActualizarGnossLive(proyectoID, Documento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);

                    if (proyectoID != ProyectoAD.MetaProyecto)
                    {
                        ControladorDocumentacion.ActualizarGnossLive(proyectoID, IdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad(proyectoID), AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);
                    }
                    #endregion

                    ControladorDocumentacion.AgregarRecursoModeloBaseSimple(Documento.Clave, proyectoID, (short)Documento.TipoDocumentacion, PrioridadBase.Alta, mAvailableServices);
                }

                if (!Documento.FilaDocumentoWebVinBR.PrivadoEditores)
                {
                    GestionProyecto gestProy = new GestionProyecto(proyCN.ObtenerProyectosPorIDsCargaLigera(listaProyectosActualizarNumProy), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);

                    List<Elementos.ServiciosGenerales.Proyecto> proyectosEnvioTwitter = new List<Elementos.ServiciosGenerales.Proyecto>(gestProy.ListaProyectos.Values);

                    //Enviar mensaje a twitter
                    ControladorDocumentacion.EnviarEnlaceATwitterDeComunidad(IdentidadActual, Documento, proyectosEnvioTwitter, BaseURLIdioma, UtilIdiomas, UrlPerfil, mAvailableServices);
                }
                proyCN.Dispose();

                ControladorDocumentacion.BorrarCacheControlFichaRecursos(Documento.Clave);

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }
        }

        /// <summary>
        /// Descomparte un recurso.
        /// </summary>
        /// <param name="baseRecursoID"></param>
        /// <param name="proyectoID"></param>
        /// <param name="organizacionID"></param>
        private void DesCompartirRecurso(Guid baseRecursoID, Guid proyectoID, Guid organizacionID)
        {
            if (Documento.BaseRecursos.Count < 2)
            {
                GestorDocumental.EliminarDocumentoLogicamente(Documento.GestorDocumental.ListaDocumentos[Documento.Clave]);
            }
            else
            {
                GestorDocumental.DesVincularDocumentoDeBaseRecursos(Documento.Clave, baseRecursoID, proyectoID, IdentidadActual.Clave);
            }

            List<Guid> listaProyectosActualizarNumRec = new List<Guid>();

            if (GestorDocumental.ListaDocumentos[Documento.Clave].FilaDocumento.Borrador == false)
            {
                listaProyectosActualizarNumRec.Add(proyectoID);
            }

            mEntityContext.SaveChanges();
            if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && Documento.FilaDocumento.Eliminado)
            {
                ControladorDocumentacion.BorrarRDFDeDocumentoEliminado(Documento.Clave, Documento.FilaDocumento.ElementoVinculadoID.Value, UrlIntragnoss, false, proyectoID);
            }

            #region Actualizar cola GnossLIVE

            int tipo;

            switch (Documento.TipoDocumentacion)
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
            if (proyectoID.Equals(Guid.Empty))
            {
                proyectoID = ProyectoAD.MetaProyecto;
            }
            if (proyectoID == ProyectoAD.MetaProyecto)
            {
                infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
            }

            ControladorDocumentacion.ActualizarGnossLive(proyectoID, Documento.VersionOriginalID, AccionLive.Eliminado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);
            ControladorDocumentacion.ActualizarGnossLive(proyectoID, Documento.ObtenerPublicadorEnBR(baseRecursoID), AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);

            #endregion

            Documento doc = Documento.GestorDocumental.ListaDocumentos[Documento.Clave];

            ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(doc, IdentidadActual, true);
            ControladorDocumentacion controDoc = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
            Guid docID = Documento.VersionOriginalID != Documento.Clave ? Documento.VersionOriginalID : Documento.Clave;
            controDoc.ActualizarNumRecCatTesDeTesauro(true, Guid.Empty, organizacionID, proyectoID, docID, mAvailableServices, true);

            ControladorDocumentacion.BorrarCacheControlFichaRecursos(Documento.Clave);

            try
            {
                FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, true, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                facetadoCN.ModificarVotosNegativo(proyectoID.ToString(), Documento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Find(docu => docu.DocumentoID.Equals(Documento.Clave) && docu.BaseRecursosID.Equals(baseRecursoID)).IdentidadPublicacionID.ToString().ToUpper(), "recursos");
                facetadoCN.Dispose();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
        }

        /// <summary>
        /// Guarda la documentación
        /// </summary>
        public void GuardarCompartir()
        {
            DocumentacionCN docuCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            docuCN.ActualizarDocumentacion();
            docuCN.Dispose();
            docuCN = null;
        }

        #endregion

        #region Añadir Categorias y Etiquetas

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult AddTags(string[] Tags)
        {
            try
            {
                //Seguridad :
                /*
                    * Si no es borrador y es ultima version
                    * Si no iene permisos de edicion o es una encuesta
                    * Si no esta bloqueado por otro usuario o (es administrador del proyecto y tiene permisos para editar el recurso)
                */
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                if (!Documento.EsBorrador && Documento.FilaDocumento.UltimaVersion /*&& (Documento.TipoDocumentacion == TiposDocumentacion.Encuesta)*/ && ProyectoOrigenBusquedaID == Guid.Empty && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonAgregarEtiquetasDoc))
                {
                    return AccionRecurso_AgregarTags_Aceptar(Tags);
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (Tags != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(Tags);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion AddTags");
                }
                GuardarLogError(ex, $" Tags: {modeloDatos}");
            }

            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult AddCategories(string[] Categories)
        {
            try
            {
                //Seguridad :
                /*
                    * Si no es borrador y es ultima version
                    * Si no iene permisos de edicion o es una encuesta
                    * Si no esta bloqueado por otro usuario o (es administrador del proyecto y tiene permisos para editar el recurso)
                */
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                if (!Documento.EsBorrador && Documento.FilaDocumento.UltimaVersion && ProyectoOrigenBusquedaID == Guid.Empty && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonAgregarCategoriaDoc))
                {
                    return AccionRecurso_AgregarCategorias_Aceptar(Categories);
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (Categories != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(Categories);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion AddCategories");
                }
                GuardarLogError(ex, $" Categories: {modeloDatos}");
            }

            return GnossResultERROR();
        }

        /// <summary>
        /// Se añadiran nuevos tags al documento seleccionado.
        /// </summary>
        /// <param name="pListaTags">Lista de tags</param>
        private ActionResult AccionRecurso_AgregarTags_Aceptar(string[] pListaTags)
        {
            try
            {
                if (pListaTags.Length == 0)
                {
                    return GnossResultERROR("0");
                }

                bool eliminandoTags = false;
                List<string> listaOldTagsEliminar = new List<string>();
                if (!string.IsNullOrEmpty(Documento.Tags))
                {
                    List<string> listaOldTags = Documento.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    listaOldTagsEliminar = (from tagOld in listaOldTags where !(from tagNew in pListaTags select tagNew).Contains(tagOld) select tagOld).ToList();

                    //Solo el administrador puede eliminar tags
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) && listaOldTagsEliminar.Count > 0)
                    {
                        eliminandoTags = true;
                    }
                }

                if (Documento.TienePermisosIdentidadEditarTagsYCategoriasEnBR(IdentidadActual, Documento.GestorDocumental.BaseRecursosIDActual, UsuarioActual.ProyectoID, EsIdentidadActualSupervisorProyecto, EsAdministrador(IdentidadActual)))
                {
                    Documento.Tags = "";
                }
                else
                {
                    Documento.Tags += ",";
                }

                List<string> listaTagAgregador = new List<string>();
                foreach (string tag in pListaTags)
                {
                    //Agrego los tags nuevos
                    Documento.Tags += tag + ",";
                    listaTagAgregador.Add(tag);
                }
                Documento.Tags = Documento.Tags.Trim(',');
                //Añado al historial de documento el tratamiento de sus tags.
                GestorDocumental.AgregarEliminarTagsHistorial(Documento, listaTagAgregador, AccionHistorialDocumento.Agregar, ProyectoSeleccionado.Clave, IdentidadActual.Clave);

                mEntityContext.SaveChanges();

                ControladorDocumentacion.BorrarCacheControlFichaRecursos(Documento.Clave);

                ControladorDocumentacion controDoc = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                controDoc.NotificarModificarTagsSearchRecursoComunidadesCompartido(Documento, eliminandoTags, PrioridadBase.Alta, mAvailableServices);

                //Actualizamos la cache y el live
                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                List<Guid> listaProyectosActualizar = new List<Guid>();
                listaProyectosActualizar.Add(ProyectoSeleccionado.Clave);
                int tipoDoc = -1;
                int tipoElemento = (int)TipoLive.Recurso;
                if (Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                {
                    tipoDoc = (int)TiposDocumentacion.Pregunta;
                    tipoElemento = (int)TipoLive.Pregunta;
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.Debate)
                {
                    tipoDoc = (int)TiposDocumentacion.Debate;
                    tipoElemento = (int)TipoLive.Debate;
                }
                docCL.BorrarPrimerosRecursosSiEstaRecurso(Documento.Clave, listaProyectosActualizar, tipoDoc);
                //TODO : Borrar cache busqueda
                docCL.Dispose();

                string infoExtra = null;

                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                {
                    infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                }

                ControladorDocumentacion.ActualizarGnossLive(ProyectoSeleccionado.Clave, Documento.Clave, AccionLive.Editado, tipoElemento, PrioridadLive.Alta, infoExtra, mAvailableServices);

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                // Identidad proyecto actual
                controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR("1");
            }
        }

        /// <summary>
        /// Las nuevas categorías agregadas al documento se añadirán a él.
        /// </summary>
        /// <param name="sender">El que envia la petición</param>
        /// <param name="e">Argumentos</param>
        private ActionResult AccionRecurso_AgregarCategorias_Aceptar(string[] pListaCatSeleccionadas)
        {
            try
            {
                if (pListaCatSeleccionadas.Length == 0)
                {
                    return GnossResultERROR("0");
                }

                if (GestorDocumental.GestorTesauro == null)
                {
                    CargarTesauro();
                }

                //Obtengo la categorías nuevas
                List<CategoriaTesauro> listaCategoriasNuevas = new List<CategoriaTesauro>();
                List<CategoriaTesauro> listaCategoriasEliminadas = new List<CategoriaTesauro>();
                List<Guid> listaCatSeleccionadas = new List<Guid>();

                foreach (string cat in pListaCatSeleccionadas)
                {
                    if (cat != "" && !listaCatSeleccionadas.Contains(new Guid(cat)))
                    {
                        listaCatSeleccionadas.Add(new Guid(cat));
                    }
                }

                if (listaCatSeleccionadas.Count > 0)
                {
                    Guid identidadCreador = mControladorBase.UsuarioActual.IdentidadID;

                    if (EsIdentidadBROrg)
                    {
                        identidadCreador = IdentidadOrganizacionBROrg.Clave;
                    }

                    if (Documento.TienePermisosIdentidadEditarTagsYCategoriasEnBR(IdentidadActual, Documento.GestorDocumental.BaseRecursosIDActual, ProyectoSeleccionado.Clave, EsIdentidadActualSupervisorProyecto, EsAdministrador(IdentidadActual)))
                    {
                        //Solo borraremos categorias si somos editor del documento.
                        foreach (Guid categoriaID in Documento.Categorias.Keys)
                        {
                            if (!listaCatSeleccionadas.Contains(categoriaID) && listaCategoriasEliminadas.Count < Documento.Categorias.Count)
                            {
                                CategoriaTesauro categoria = Documento.Categorias[categoriaID];
                                listaCategoriasEliminadas.Add(categoria);
                            }
                        }
                    }

                    foreach (Guid categoriaID in listaCatSeleccionadas)
                    {
                        CategoriaTesauro categoria = Documento.GestorDocumental.GestorTesauro.ListaCategoriasTesauro[categoriaID];

                        if (!Documento.Categorias.ContainsKey(categoriaID) && !listaCategoriasNuevas.Contains(categoria))
                        {
                            listaCategoriasNuevas.Add(categoria);
                        }
                    }
                    Documento.GestorDocumental.VincularDocumentoACategorias(listaCategoriasNuevas, Documento, identidadCreador, ProyectoSeleccionado.Clave);
                    Documento.GestorDocumental.DesvincularDocumentoDeCategorias(Documento, listaCategoriasEliminadas, identidadCreador, ProyectoSeleccionado.Clave);
                }
                else
                {
                    throw new Exception();
                }

                Guardar(new List<Guid>());

                if (listaCatSeleccionadas.Count > 0)
                {
                    ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(Documento, IdentidadActual, true);

                    bool eliminandoCategorias = false;
                    //solo el administrador puede eliminar categorías
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) && listaCategoriasEliminadas.Count > 0)
                    {
                        eliminandoCategorias = true;
                    }

                    UtilidadesVirtuoso.GuardarEdicionCategoriasRecursoEnGrafoBusqueda(Documento, mControladorBase.UsuarioActual.ProyectoID, eliminandoCategorias, UrlIntragnoss, PrioridadBase.Alta, mAvailableServices);

                    int tipo;

                    switch (Documento.TipoDocumentacion)
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

                    if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto)
                    {
                        infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                    }

                    ControladorDocumentacion.ActualizarGnossLive(mControladorBase.UsuarioActual.ProyectoID, Documento.Clave, AccionLive.Editado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);

                    ControladorDocumentacion.BorrarCacheControlFichaRecursos(Documento.Clave);
                }

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                // Identidad proyecto actual
                controladorPersonas.ActivoEnComunidad(IdentidadActual, mAvailableServices);

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR("1");
            }
        }

        #endregion

        #region Añadir Categorias y Etiquetas

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult AddMetaTitle(string MetaTitle)
        {
            try
            {
                //Seguridad :
                /*
                    * Si no es borrador y es ultima version
                    * Si no iene permisos de edicion o es una encuesta
                    * Si no esta bloqueado por otro usuario o (es administrador del proyecto y tiene permisos para editar el recurso)
                */
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                if (!Documento.EsBorrador && Documento.FilaDocumento.UltimaVersion && ProyectoOrigenBusquedaID == Guid.Empty && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonAgregarCategoriaDoc))
                {
                    return GuardarMetaTitle(MetaTitle);
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (MetaTitle != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(MetaTitle);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion AddMetaTitle");
                }
                GuardarLogError(ex, " Categories: " + modeloDatos);
            }

            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult AddMetaDescripcion(string MetaDescripcion)
        {
            try
            {
                //Seguridad :
                /*
                    * Si no es borrador y es ultima version
                    * Si no iene permisos de edicion o es una encuesta
                    * Si no esta bloqueado por otro usuario o (es administrador del proyecto y tiene permisos para editar el recurso)
                */
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                if (!Documento.EsBorrador && Documento.FilaDocumento.UltimaVersion && ProyectoOrigenBusquedaID == Guid.Empty && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonAgregarCategoriaDoc))
                {
                    return GuardarMetaDescripcion(MetaDescripcion);
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (MetaDescripcion != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(MetaDescripcion);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion AddMetaTitle");
                }
                GuardarLogError(ex, " Categories: " + modeloDatos);
            }

            return GnossResultERROR();
        }

        /// <summary>
        /// Guardar el metaTitulo
        /// </summary>
        /// <param name="sender">El que envia la petición</param>
        /// <param name="e">Argumentos</param>
        private ActionResult GuardarMetaTitle(string pMetaTitle)
        {
            try
            {
                AD.EntityModel.Models.Documentacion.DocumentoMetaDatos doc = GestorDocumental.AgregarMetaTitulo(Documento, pMetaTitle);

                //Actualizamos la cache y el live
                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.AgregarMetaDatos(Documento.Clave, doc);
                docCL.Dispose();

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR("1");
            }
        }

        /// <summary>
        /// Guardar el metaTitulo
        /// </summary>
        /// <param name="sender">El que envia la petición</param>
        /// <param name="e">Argumentos</param>
        private ActionResult GuardarMetaDescripcion(string pMetaDescripcion)
        {
            try
            {
                AD.EntityModel.Models.Documentacion.DocumentoMetaDatos doc = GestorDocumental.AgregarMetaDescripcion(Documento, pMetaDescripcion);
                //Actualizamos la cache y el live
                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.AgregarMetaDatos(Documento.Clave, doc);
                docCL.Dispose();

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR("1");
            }
        }

        #endregion

        #region Restaurar Version

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult RestoreVersion()
        {
            try
            {
                //Seguridad :
                /*
                 * Si no es la ultima version
                 * Si tiene permisos para editar el recurso
                 * Si no es una wiki o (Si es una wiki tiene que ser supervisor y que no este protegido)
                 */
                bool tienePermiso = TienePermisoRestaurarVersion();
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                if ((Documento.TipoDocumentacion != TiposDocumentacion.Wiki || (!Documento.FilaDocumento.Protegido && EsIdentidadActualSupervisorProyecto)) && !Documento.FilaDocumento.UltimaVersion && (Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)) && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonRestaurarVersionDoc) || tienePermiso)
                {
                    if (Documento.FilaDocumento.VersionDocumento.EsMejora && ComprobarRestaurarMejora())
                    {
                        return AccionRecurso_Restaurar_Mejora_Aceptar();
                    }
                    else
                    {
                        return AccionRecurso_Restaurar_Aceptar();
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult DeleteVersion()
        {
            try
            {
                //Seguridad :
                /*
                 * Si no es la ultima version
                 * Si tiene permisos para editar el recurso
                 * Si no es una wiki o (Si es una wiki tiene que ser supervisor y que no este protegido)
                 */
                bool tienePermiso = TienePermisoEliminarVersion();
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                if ((Documento.TipoDocumentacion != TiposDocumentacion.Wiki || (!Documento.FilaDocumento.Protegido && EsIdentidadActualSupervisorProyecto)) && !Documento.FilaDocumento.UltimaVersion && (Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)) && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonRestaurarVersionDoc) || tienePermiso)
                {
                    return AccionRecurso_Eliminar_Aceptar();
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return GnossResultERROR();
        }


        private bool TienePermisoRestaurarVersion()
        {
            bool tienePermiso = false;
            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            switch (Documento.TipoDocumentacion)
            {
                case TiposDocumentacion.Nota:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionNota, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Hipervinculo:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionEnlace, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.ReferenciaADoc:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionReferencia, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Video:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionEnlace, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos) || utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.FicheroServidor:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Encuesta:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionEncuesta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Debate:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionDebate, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Pregunta:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionPregunta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Semantico:
                    tienePermiso = utilPermisos.IdentidadTienePermisoRecursoSemantico(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoPermisoRecursosSemanticos.RestaurarVersion, Documento.ElementoVinculadoID);
                    break;
            }

            return tienePermiso;
        }

        private bool TienePermisoEliminarVersion()
        {
            bool tienePermiso = false;
            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            switch (Documento.TipoDocumentacion)
            {
                case TiposDocumentacion.Nota:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionNota, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Hipervinculo:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionEncuesta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.ReferenciaADoc:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionReferencia, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Video:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionEnlace, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos) || utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.FicheroServidor:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Encuesta:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionEncuesta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Debate:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionDebate, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Pregunta:
                    tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionPregunta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Semantico:
                    tienePermiso = utilPermisos.IdentidadTienePermisoRecursoSemantico(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoPermisoRecursosSemanticos.EliminarVersion, Documento.ElementoVinculadoID);
                    break;
            }

            return tienePermiso;
        }

        private bool ComprobarRestaurarMejora()
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            bool resultado = docCN.ComprobarDocumentoTieneMejoraPendiente(Documento.FilaDocumento.VersionDocumento.DocumentoOriginalID, (Guid)Documento.FilaDocumento.VersionDocumento.MejoraID);
            docCN.Dispose();
            return resultado;
        }
        private bool CopiarAdjuntosRecursosRestauracion(DocumentoWeb pDocUltimaVersion, Documento pDocVersionRestaurada, bool pEsMejora, ControladorDocumentacion pControladroDocumentacion)
        {
            bool correcto = true;
            if (pDocUltimaVersion.TipoDocumentacion == TiposDocumentacion.Semantico)
            {
                correcto = pControladroDocumentacion.CrearNuevaVersionDocumentoRDF(Documento, pDocVersionRestaurada, mAvailableServices, pEsMejora);
            }
            else if (pDocUltimaVersion.EsFicheroDigital)
            {
                correcto = pControladroDocumentacion.DuplicarDocumentoFisicamente(Documento, pDocVersionRestaurada, IdentidadOrganizacionBROrg, (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto), UsuarioActual);
                if (!pEsMejora)
                {
                    pControladroDocumentacion.CapturarImagenWeb(pDocVersionRestaurada.Clave, false, PrioridadColaDocumento.Alta, mAvailableServices);
                }
            }
            return correcto;
        }

        private ActionResult AccionRecurso_Restaurar_Aceptar()
        {
            try
            {
                DocumentoWeb docUltimaVersion = new DocumentoWeb(Documento.UltimaVersion.FilaDocumento, Documento.GestorDocumental);

                ControladorDocumentacion controlador = new ControladorDocumentacion(Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                controlador.CargarNecesarioParaDuplicacion(docUltimaVersion.Clave, true);
                controlador.CargarNecesarioParaDuplicacion(Documento.Clave, true);

                CargarGestorVotosRestaurar(docUltimaVersion.Clave);

                Documento nuevaVersionRestaurada = Documento.GestorDocumental.RestaurarVersion(Documento, docUltimaVersion, IdentidadRestaurar);

                ComprobarNuevaVersionAgregada(nuevaVersionRestaurada);

                bool correcto = CopiarAdjuntosRecursosRestauracion(docUltimaVersion, nuevaVersionRestaurada, false, controlador);

                if (correcto)
                {
                    mEntityContext.SaveChanges();

                    ActualizarLive(docUltimaVersion, nuevaVersionRestaurada);

                    ControladorDocumentacion controDoc = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);

                    ActualizarBase(controDoc, docUltimaVersion, nuevaVersionRestaurada);

                    ControladorDocumentacion.BorrarCacheControlFichaRecursos(docUltimaVersion.Clave);

                    return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, nuevaVersionRestaurada, false).Replace(nuevaVersionRestaurada.Clave.ToString(), nuevaVersionRestaurada.VersionOriginalID.ToString()));
                }
                else
                {
                    return GnossResultERROR(UtilIdiomas.GetText("HISTORIALVERSIONES", "ERRORRESTAURARVERSION"));
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR();
            }
        }
        private ActionResult AccionRecurso_Restaurar_Mejora_Aceptar()
        {
            try
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                DocumentoWeb documentoUltimaVersionMejora = new DocumentoWeb(docCN.ObtenerUltimaVersionDocumentoMejora(Documento.FilaDocumento.VersionDocumento.DocumentoOriginalID, (Guid)Documento.FilaDocumento.VersionDocumento.MejoraID), Documento.GestorDocumental);
                docCN.Dispose();

                ControladorDocumentacion controlador = new ControladorDocumentacion(Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                controlador.CargarNecesarioParaDuplicacion(documentoUltimaVersionMejora.Clave, true);
                controlador.CargarNecesarioParaDuplicacion(Documento.Clave, true);

                CargarGestorVotosRestaurar(documentoUltimaVersionMejora.Clave);

                Documento nuevaVersionRestaurada = Documento.GestorDocumental.RestaurarVersion(Documento, documentoUltimaVersionMejora,IdentidadRestaurar, true);

                ComprobarNuevaVersionAgregada(nuevaVersionRestaurada);

                bool correcto = CopiarAdjuntosRecursosRestauracion(documentoUltimaVersionMejora, nuevaVersionRestaurada, true, controlador);

                if (correcto)
                {
                    mEntityContext.SaveChanges();

                    return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, nuevaVersionRestaurada, false).Replace(nuevaVersionRestaurada.Clave.ToString(), $"{nuevaVersionRestaurada.VersionOriginalID.ToString()}/{nuevaVersionRestaurada.Clave}"));
                }
                else
                {
                    return GnossResultERROR(UtilIdiomas.GetText("HISTORIALVERSIONES", "ERRORRESTAURARVERSION"));
                }
            }
            catch(Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR();
            }
        }
        private void CargarGestorVotosRestaurar(Guid pDocumentoID)
        {
            if (Documento.GestorDocumental.GestorVotos == null)
            {
                VotoCN votoCN = new VotoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
                Documento.GestorDocumental.GestorVotos = new GestionVotosDocumento(votoCN.ObtenerVotosDocumentoPorID(pDocumentoID), Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionVotosDocumento>(), mLoggerFactory);
                votoCN.Dispose();
            }
        }
        private void ComprobarNuevaVersionAgregada(Documento pDocuemtoNuevaVersion)
        {
            // David: Añadir el documento a la lista de documentos de la página
            if (!Documento.GestorDocumental.ListaDocumentos.ContainsKey(pDocuemtoNuevaVersion.Clave))
            {
                Documento.GestorDocumental.ListaDocumentos.Add(pDocuemtoNuevaVersion.Clave, pDocuemtoNuevaVersion);
            }
        }
        private ActionResult AccionRecurso_Eliminar_Aceptar()
        {
            try
            {
                Documento.GestorDocumental.EliminarVersionDocumento(Documento);

                // Si es un recurso semántico hay que borrar los rdf
                if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    ControladorDocumentacion.EliminarVersionDocumentoRDF(Documento.Clave);
                }

                ControladorDocumentacion.InsertarEnColaProcesarFicherosRecursosModificadosOEliminados(Documento.Clave, TipoEventoProcesarFicherosRecursos.BorradoPersistente, mAvailableServices);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR();
            }
            return GnossResultOK();
        }
        #endregion
        #region Mejoras de version
        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public IActionResult ApplyImprovement()
        {
            try
            {
                //Seguridad :
                /*
                 * Si no es la ultima version
                 * Si tiene permisos para editar el recurso
                 * Si no es una wiki o (Si es una wiki tiene que ser supervisor y que no este protegido)
                 */
                DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                AD.EntityModel.Models.Documentacion.VersionDocumento versionDocumento = documentacionCN.ObtenerVersionPorVersionID(DocumentoVersionID);
                bool tienePermiso = TienePermisoAprobarMejora(versionDocumento);
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                if (tienePermiso)
                {
                    return AccionRecurso_Aplicar_Mejora(versionDocumento);
                }
                return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "ERRORPERMISOACEPTARMEJORA"));
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
        public IActionResult CancelImprovement()
        {
            try
            {
                DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                AD.EntityModel.Models.Documentacion.VersionDocumento versionDocumento = documentacionCN.ObtenerVersionPorVersionID(DocumentoVersionID);
                bool tienePermiso = TienePermisoCancelarMejora(versionDocumento);
                if (tienePermiso)
                {
                    return AccionRecurso_Cancelar_Mejora(versionDocumento);
                }
                return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "ERRORPERMISOCANCELARMEJORA"));
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return GnossResultERROR();
        }

		[HttpPost, TypeFilter(typeof(AccesoRecursoAttribute))]
		public GnossResult StartImprovement()
		{
			try
			{
                bool tienePermiso = TienePermisoIniciarMejora();
                if (tienePermiso)
                {					
					return AccionRecurso_Iniciar_Mejora();
				}
				return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "ERRORPERMISOINICIARMEJORA"));				
			}
			catch (Exception ex)
			{
				mLoggingService.GuardarLogError(ex, mlogger);
				return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "ERRORINICIARMEJORA"));
			}
		}

        private bool TienePermisoAprobarMejora(AD.EntityModel.Models.Documentacion.VersionDocumento pVersionDocumento)
        {
            bool permitirAprobarMejora = false;
            //Comprobar que el estado es el final
            //Comprobar que el usuario tiene permisos de edición en el estado final
            //Comprobar que la versión este pendiente
            if (pVersionDocumento != null && pVersionDocumento.EsMejora && ((EstadoVersion)pVersionDocumento.EstadoVersion).Equals(EstadoVersion.Pendiente) && pVersionDocumento.EstadoID.HasValue)
            {
                FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
                bool esEstadoFinal = flujosCN.EsEstadoFinal(pVersionDocumento.EstadoID.Value);
                bool permisosEdicionEnEstado = flujosCN.ComprobarIdentidadTienePermisoEdicionEnEstado(pVersionDocumento.EstadoID.Value, IdentidadActual.FilaIdentidad.IdentidadID, pVersionDocumento.DocumentoID);
                if (esEstadoFinal && permisosEdicionEnEstado)
                {
                    permitirAprobarMejora = true;
                }
            }

            return permitirAprobarMejora;
        }

        private bool TienePermisoCancelarMejora(AD.EntityModel.Models.Documentacion.VersionDocumento pVersionDocumento)
        {
            bool permitirAprobarMejora = false;
            //Comprobar que el usuario tiene permisos de edición en el estado
            //Comprobar que la versión este pendiente
            if (pVersionDocumento != null && pVersionDocumento.EsMejora && ((EstadoVersion)pVersionDocumento.EstadoVersion).Equals(EstadoVersion.Pendiente) && pVersionDocumento.EstadoID.HasValue)
            {
                FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
                bool permisosEdicionEnEstado = flujosCN.ComprobarIdentidadTienePermisoEdicionEnEstado(pVersionDocumento.EstadoID.Value, IdentidadActual.FilaIdentidad.IdentidadID, pVersionDocumento.DocumentoID);
                if (permisosEdicionEnEstado)
                {
                    permitirAprobarMejora = true;
                }
            }

            return permitirAprobarMejora;
        }

        private bool TienePermisoIniciarMejora()
        {
			// Comprobar que el usuario tiene permisos de edición en el estado            
			// Comprobar que el estado permita mejora
            // Comprobar que el estado sea final
			// Comprobar que no esté ya en otro proceso de mejora
            
			bool permitirIniciarMejora = false;
            EstadoModel estado = ObtenerEstadoDocumento();

			if (estado != null && !ComprobarSiDocumentoEstaSiendoMejorado() && estado.EsFinal && estado.PermiteMejora)
            {
				FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
				permitirIniciarMejora = flujosCN.ComprobarIdentidadTienePermisoEdicionEnEstado(estado.EstadoID, IdentidadActual.FilaIdentidad.IdentidadID, DocumentoID);
            }

            return permitirIniciarMejora;
		}

        private bool ComprobarSiDocumentoEstaSiendoMejorado()
        {
            if (DocumentoVersionID == Guid.Empty)
            {
			    List<AD.EntityModel.Models.Documentacion.VersionDocumento> versiones = mGestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.Where(item => item.DocumentoOriginalID.Equals(DocumentoID) && item.EsMejora && item.EstadoVersion.Equals((short)EstadoVersion.Pendiente)).ToList();
			    if (versiones.Count > 0)
			    {
				    return true;
			    }
			}			

			return false;
		}

		private GnossResult AccionRecurso_Iniciar_Mejora()
        {
            try
            {
                // Si no tiene versiones, se crea una para evitar errores al cargar el historial
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                if (!docCN.ComprobarDocumentoTieneVersiones(Documento.FilaDocumento.VersionDocumento.DocumentoOriginalID))
                {
                    GestorDocumental.CrearPrimeraVersionDocumento(Documento);
                    mEntityContext.SaveChanges();
                }
                docCN.Dispose();
                Elementos.Documentacion.Documento docMejora = GestorDocumental.CrearNuevaVersionDocumento(Documento, IdentidadActual, pEsMejora: true);
				ControladorDocumentacion controladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                controladorDocumentacion.IniciarMejoraSobreDocumento(Documento, docMejora, UsuarioActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto);

				return GnossResultUrl($"{mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, UtilCadenas.EliminarCaracteresUrlSem(Documento.Titulo), Documento.VersionOriginalID, Documento.ElementoVinculadoID, false)}/{docMejora.Clave}");
			}
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "ERRORINICIARMEJORA"));
            }
        }

        private GnossResult AccionRecurso_Aplicar_Mejora(AD.EntityModel.Models.Documentacion.VersionDocumento pVersionDocumento)
        {
            //Obtener la última versión del documento original en la tabla documento
            //marcar la version como no ultima
            //Cambiar ultima versión del documento
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            try
            {
                ControladorDocumentacion controDoc = new ControladorDocumentacion(Documento.GestorDocumental, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                documentacionCN.IniciarTransaccion();
                AD.EntityModel.Models.Documentacion.Documento documentoPrevio = documentacionCN.ObtenerUltimaVersionDocumento(pVersionDocumento.DocumentoOriginalID);
                if (documentoPrevio == null)
                {
                    documentoPrevio = documentacionCN.ObtenerDocumentoPorID(pVersionDocumento.DocumentoOriginalID).ListaDocumento.FirstOrDefault();
                }
                AD.EntityModel.Models.Documentacion.Documento documentoMejorado = documentacionCN.ObtenerDocumentoPorID(pVersionDocumento.DocumentoID).ListaDocumento.First();
                documentoPrevio.UltimaVersion = false;
                documentoMejorado.UltimaVersion = true;

                AD.EntityModel.Models.Documentacion.VersionDocumento versionAnterior = documentacionCN.ObtenerVersionPorVersionID(documentoPrevio.DocumentoID);
                documentacionCN.CambiarEstadoVersionesDeMejoraAprobada(pVersionDocumento);
                if (versionAnterior != null)
                {
                    versionAnterior.EstadoVersion = (short)EstadoVersion.Historico;
                }

                DataWrapperDocumentacion dataWrapperDocumentacionDocPrevio = new DataWrapperDocumentacion();
                dataWrapperDocumentacionDocPrevio.ListaDocumento.Add(documentoPrevio);
                DataWrapperDocumentacion dataWrapperDocumentacionDocMejorado = new DataWrapperDocumentacion();
                dataWrapperDocumentacionDocMejorado.ListaDocumento.Add(documentoMejorado);
                DocumentoWeb documentoWebPrevio = new DocumentoWeb(documentoPrevio, new GestorDocumental(dataWrapperDocumentacionDocPrevio, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory));
                DocumentoWeb documentoWebMejorado = new DocumentoWeb(documentoMejorado, new GestorDocumental(dataWrapperDocumentacionDocMejorado, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory));
                documentoWebMejorado.GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;

				if (documentoWebPrevio.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    controDoc.AplicarMejoraDocumentoRDF(documentoWebPrevio, documentoWebMejorado, mAvailableServices);
                }

                mEntityContext.SaveChanges();
                documentacionCN.TerminarTransaccion(true);

                ActualizarLive(documentoWebPrevio, documentoWebMejorado);

                ActualizarBase(controDoc, documentoWebPrevio, documentoWebMejorado);

                ControladorDocumentacion.BorrarCacheControlFichaRecursos(documentoWebPrevio.Clave);
                ControladorDocumentacion.BorrarCacheControlFichaRecursos(documentoWebMejorado.Clave);


                GuardarLogAuditoria();
                DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);
                string url = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, documentoWebMejorado, false).Replace(documentoWebMejorado.Clave.ToString(), pVersionDocumento.DocumentoOriginalID.ToString());
                gestorNotificaciones.EnviarCorreoAvisoAplicarMejora(pVersionDocumento.MejoraID.Value, documentoPrevio.DocumentoID, Documento.ProyectoID, url, documentoPrevio.Titulo, IdentidadActual.Nombre());
                notificacionCN.ActualizarNotificacion(mAvailableServices);
                return GnossResultUrl(url);
            }
            catch (Exception ex)
            {
                documentacionCN.TerminarTransaccion(false);
                GuardarLogError(ex);
                return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "ERRORRESACEPTARMEJORA"));
            }
        }

        private void ActualizarLive(Documento documentoWebPrevio, Documento documentoWebMejorado)
        {
            int tipo;
            switch (documentoWebPrevio.TipoDocumentacion)
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

            foreach (Guid baseRecurso in documentoWebPrevio.BaseRecursos)
            {
                Guid proyecto = documentoWebPrevio.GestorDocumental.ObtenerProyectoID(baseRecurso);

                string infoExtra = null;

                if (proyecto == ProyectoAD.MetaProyecto)
                {
                    infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                }

                ControladorDocumentacion.ActualizarGnossLive(proyecto, documentoWebPrevio.Clave, AccionLive.Eliminado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);
                ControladorDocumentacion.ActualizarGnossLive(proyecto, documentoWebMejorado.Clave, AccionLive.Agregado, tipo, PrioridadLive.Alta, mAvailableServices);
            }
        }

        private void ActualizarBase(ControladorDocumentacion controDoc, Documento documentoWebPrevio, Documento documentoWebMejorado)
        {
            // No es necesario actualizar el base si es un recurso semántico ya lo hacemos durante la 
            // restauracion del RDF
            if (documentoWebPrevio.TipoDocumentacion != TiposDocumentacion.Semantico)
            {
                controDoc.ActualizarNumRecCatTesDeTesauroDeRecursoEliminado(documentoWebPrevio, mAvailableServices);

                controDoc.ActualizarNumRecCatTesDeTesauro(Guid.Empty, documentoWebMejorado.Clave, false, UsuarioActual.OrganizacionID, UsuarioActual.ProyectoID, mAvailableServices);
            }

            if (documentoWebMejorado.FilaDocumento.DocumentoWebVinBaseRecursos.Count > 1)
            {
                controDoc.mActualizarTodosProyectosCompartido = true;
                controDoc.NotificarModificarTagsRecurso(documentoWebMejorado.Clave, mAvailableServices);
            }
        }


        private GnossResult AccionRecurso_Cancelar_Mejora(AD.EntityModel.Models.Documentacion.VersionDocumento pVersionDocumento)
        {
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            try
            {
                documentacionCN.IniciarTransaccion();

                AD.EntityModel.Models.Documentacion.Documento ultimaVersion = documentacionCN.ObtenerUltimaVersionDocumento(pVersionDocumento.DocumentoOriginalID);
                documentacionCN.EliminarVersionesDeMejora(pVersionDocumento.MejoraID.Value);

                DataWrapperDocumentacion dataWrapperDocumentacionDocUltimaVersion = new DataWrapperDocumentacion();
                dataWrapperDocumentacionDocUltimaVersion.ListaDocumento.Add(ultimaVersion);
                DocumentoWeb documentoWebUltimaVersion = new DocumentoWeb(ultimaVersion, new GestorDocumental(dataWrapperDocumentacionDocUltimaVersion, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory));

                ControladorDocumentacion.InsertarEnColaProcesarFicherosRecursosModificadosOEliminados(pVersionDocumento.DocumentoID, TipoEventoProcesarFicherosRecursos.BorradoPersistente, mAvailableServices);
                mEntityContext.SaveChanges();
                documentacionCN.TerminarTransaccion(true);
                GuardarLogAuditoria();

                DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);
                string url = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, documentoWebUltimaVersion, false).Replace(documentoWebUltimaVersion.Clave.ToString(), pVersionDocumento.DocumentoOriginalID.ToString());

                gestorNotificaciones.EnviarCorreoAvisoCancelarMejora(pVersionDocumento.MejoraID.Value, ultimaVersion.DocumentoID, Documento.ProyectoID, url, ultimaVersion.Titulo, IdentidadActual.Nombre());
                notificacionCN.ActualizarNotificacion(mAvailableServices);

                return GnossResultUrl(url);
            }
            catch (Exception ex)
            {
                documentacionCN.TerminarTransaccion(false);
                GuardarLogError(ex);
                return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "ERRORRESCANCELARMEJORA"));
            }

        }
        #endregion
        #region Eliminar

        [HttpPost]
        [TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult Delete()
        {
            try
            {
                //Seguridad : 
                /*
                 * Si es la ultima version
                 * Si no esta protegido o lo ha protegido el usuario actual o el usuario actual es supervisor
                 * Si no es una wiki o el usuario actual es supervisor
                 * Si tiene permisos para editarlo o para eliminarlo en esta Base de recursos
                */
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                bool bloqueadoPorOtroUsuario = Documento.FilaDocumento.IdentidadProteccionID.HasValue && !Documento.FilaDocumento.IdentidadProteccionID.Equals(mControladorBase.UsuarioActual.IdentidadID);
                UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
                bool permisoEliminar = false;
                switch (Documento.TipoDocumentacion)
                {
                    case TiposDocumentacion.Hipervinculo:
                        permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarRecursoTipoEnlace, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                        break;
                    case TiposDocumentacion.Nota:
                        permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarNota, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                        break;
                    case TiposDocumentacion.FicheroServidor:
                        permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarRecursoTipoAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                        break;
                    case TiposDocumentacion.Encuesta:
                        permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarEncuesta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                        break;
                    case TiposDocumentacion.ReferenciaADoc:
                        permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarRecursoTipoReferenciaADocumentoFisico, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                        break;
                    case TiposDocumentacion.Pregunta:
                        permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarPregunta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                        break;
                    case TiposDocumentacion.Debate:
                        permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarDebate, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                        break;
                    case TiposDocumentacion.Semantico:
                        permisoEliminar = utilPermisos.IdentidadTienePermisoRecursoSemantico(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoPermisoRecursosSemanticos.Eliminar, Documento.ElementoVinculadoID);
                        break;
                }
                if (Documento.FilaDocumento.UltimaVersion && (!Documento.FilaDocumento.Protegido || !bloqueadoPorOtroUsuario || EsIdentidadActualSupervisorProyecto) && (!Documento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki) || EsIdentidadActualSupervisorProyecto) && (Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || Documento.TienePermisosIdentidadEliminarRecursoEnBR(IdentidadActual, Documento.GestorDocumental.BaseRecursosIDActual, ProyectoSeleccionado, UsuarioActual.UsuarioID, UsuarioActual.ProyectoID, EsIdentidadActualAdministradorOrganizacion, EsIdentidadActualSupervisorProyecto) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID) || permisoEliminar) && (GenPlantillasOWL == null || (!GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDoc && (string.IsNullOrEmpty(GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDocCondicion) || !GenPlantillasOWL.CumpleRecursoCondicion(GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDocCondicion, IdentidadActual.Clave)))))
                {
                    #region Eliminar de SharePoint
                    ParametroAplicacionCN parametroCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    string oneDrive = parametroCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                    bool esDocumentoSharepoint = UtilCadenas.EsEnlaceSharepoint(Documento.Enlace, oneDrive);
                    string sharepointConfigurado = parametroCN.ObtenerParametroAplicacion("SharepointClientID");
                    if (string.IsNullOrEmpty(sharepointConfigurado))
                    {
                        esDocumentoSharepoint = false;
                    }
                    if (esDocumentoSharepoint)
                    {
                        try
                        {
                            ActionResult redirect = FuncionalidadSharepoint();
                            if (redirect != null)
                            {
                                return redirect;
                            }
                            SharepointController spController = new SharepointController(Documento.Clave.ToString(), mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);
                            spController.Token = tokenSP;
                            spController.TokenCreadorRecurso = tokenSPAdminDocument;
                            bool eliminacionCorrecta = spController.EliminarDocumentoDeSharepoint(Documento.Enlace);
                            if (!eliminacionCorrecta)
                            {
                                mLoggingService.GuardarLogError($"No se ha podido eliminar el documento del recurso {Documento.Clave} de SharePoint. Enlace del documento: {Documento.Enlace}", mlogger);
                                return GnossResultERROR($"Error al eliminar el documento de SharePoint");
                            }
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, $"Error al eliminar el documento {Documento.Clave} de SharePoint. Enlace del documento: {Documento.Enlace}", mlogger);
                            return GnossResultERROR($"Error al eliminar el documento de SharePoint");
                        }
                    }

                    #endregion

                    #region Espacio BRs personales

                    Stopwatch sw = null;
                    try
                    {
                        double espacioArchivo = 0;

                        if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                        {
                            if (Documento.EsFicheroDigital)
                            {
                                if (Documento.TipoDocumentacion == TiposDocumentacion.FicheroServidor)
                                {
                                    GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                                    gd.Url = UrlServicioWebDocumentacion;
                                    sw = LoggingService.IniciarRelojTelemetria();
                                    if (!EsIdentidadBROrg)
                                    {
                                        espacioArchivo = gd.ObtenerEspacioDocumentoDeBaseRecursosUsuario(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, Documento.Clave, Path.GetExtension(Documento.Enlace));
                                    }
                                    else
                                    {
                                        espacioArchivo = gd.ObtenerEspacioDocumentoDeBaseRecursosOrganizacion(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacionBROrg.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), Documento.Clave, Path.GetExtension(Documento.Enlace));
                                    }
                                    mLoggingService.AgregarEntradaDependencia("Obtener espacio de la base de recursos del usuario", false, "FichaRecursoController.Delete", sw, true);
                                }
                                else if (Documento.TipoDocumentacion == TiposDocumentacion.Imagen)
                                {
                                    ServicioImagenes sI = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                                    sI.Url = UrlIntragnossServicios;
                                    sw = LoggingService.IniciarRelojTelemetria();
                                    if (!EsIdentidadBROrg)
                                    {
                                        espacioArchivo = sI.ObtenerEspacioImagenDocumentoPersonal(Documento.Clave.ToString(), ".jpg", mControladorBase.UsuarioActual.PersonaID);
                                    }
                                    else
                                    {
                                        espacioArchivo = sI.ObtenerEspacioImagenDocumentoOrganizacion(Documento.Clave.ToString(), ".jpg", (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacionBROrg.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                                    }
                                    mLoggingService.AgregarEntradaDependencia("Comprobar si imagen es documento personal", false, "FichaRecursoController.Delete", sw, true);
                                }
                                else if (Documento.TipoDocumentacion == TiposDocumentacion.Video)
                                {
                                    sw = LoggingService.IniciarRelojTelemetria();

                                    ServicioVideos sV = new ServicioVideos(mConfigService, mLoggingService, mLoggerFactory.CreateLogger<ServicioVideos>(), mLoggerFactory);
                                    if (!EsIdentidadBROrg)
                                    {
                                        espacioArchivo = sV.ObtenerEspacioVideoPersonal(Documento.Clave, mControladorBase.UsuarioActual.PersonaID);
                                    }
                                    else
                                    {
                                        espacioArchivo = sV.ObtenerEspacioVideoPersonal(Documento.Clave, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacionBROrg.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                                    }

                                    mLoggingService.AgregarEntradaDependencia("Comprobar si vídeo está en espacio personal", false, "FichaRecursoController.Delete", sw, true);
                                }
                            }
                        }
                        if (espacioArchivo != 0)
                        {
                            Documento.GestorDocumental.EspacioActualBaseRecursos = Documento.GestorDocumental.EspacioActualBaseRecursos - espacioArchivo;

                            if (Documento.GestorDocumental.EspacioActualBaseRecursos < 0)
                            {
                                Documento.GestorDocumental.EspacioActualBaseRecursos = 0;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        mLoggingService.AgregarEntradaDependencia("Error al borrar recurso", false, "FichaRecursoController.Delete", sw, false);
                    }

                    #endregion

                    List<Guid> listaProyectosActualNumRec = new List<Guid>();
                    Dictionary<Guid, Guid> listaBRsConProyectos = new Dictionary<Guid, Guid>();

                    if (Documento.FilaDocumento.Borrador == false)
                    {
                        if (Documento.TipoDocumentacion != TiposDocumentacion.Hipervinculo)
                        {
                            foreach (Guid baseRecurso in Documento.BaseRecursos)
                            {
                                Guid proyectoID = Documento.GestorDocumental.ObtenerProyectoID(baseRecurso);
                                listaProyectosActualNumRec.Add(proyectoID);
                                listaBRsConProyectos.Add(baseRecurso, proyectoID);
                            }
                        }
                        else
                        {
                            listaProyectosActualNumRec.Add(mControladorBase.UsuarioActual.ProyectoID);
                            listaBRsConProyectos.Add(Documento.GestorDocumental.BaseRecursosIDActual, mControladorBase.UsuarioActual.ProyectoID);
                        }
                    }

                    //Si esta en mas de una base de recursos, lo eliminamos solamente de la base de recursos actual
                    if (Documento.BaseRecursos.Count() > 1 && Documento.CompartidoEnProyectoActual(ProyectoSeleccionado.Clave))
                    {
                        Documento.GestorDocumental.DesVincularDocumentoDeBaseRecursos(Documento.Clave, Documento.GestorDocumental.BaseRecursosIDActual, mControladorBase.UsuarioActual.ProyectoID, IdentidadActual.Clave);
                    }
                    else
                    {
                        Documento.GestorDocumental.EliminarDocumentoLogicamente(Documento);
                        Documento.GestorDocumental.AgregarEliminarCategoriaTesauroHistorial(Documento, Guid.Empty, AccionHistorialDocumento.Eliminar, mControladorBase.UsuarioActual.ProyectoID, IdentidadActual.Clave);
                    }

                    // Hay que desvincular las categorias con todos las versiones del documento actual.
                    List<Guid> documentoIDs = GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.Select(doc => doc.DocumentoID).ToList();
                    documentoIDs.Add(Documento.VersionOriginalID);

                    foreach (Guid docID in GestorDocumental.ListaDocumentosWeb.Keys)
                    {
                        // Nos aseguramos de que solo usamos documentos que tengan que ver con el recurso.
                        if (!documentoIDs.Contains(docID) && docID != Documento.VersionOriginalID)
                        {
                            continue;
                        }
                        DocumentoWeb aux = GestorDocumental.ListaDocumentosWeb[docID];
                        aux.GestorDocumental.DesvincularDocumentoDeCategorias(aux, aux.CategoriasTesauro.Values.ToList(), aux.CreadorID, ProyectoSeleccionado.Clave);
                    }

                    #region Llamada a servio externo para notificar eliminación recurso

                    if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                    {
                        if (Documento.FilaDocumento.ElementoVinculadoID.HasValue)
                        {
                            if (!Documento.GestorDocumental.ListaDocumentos.ContainsKey(Documento.FilaDocumento.ElementoVinculadoID.Value))
                            {
                                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                                Documento.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerDocumentoPorID(Documento.FilaDocumento.ElementoVinculadoID.Value));
                                docCN.Dispose();

                                Documento.GestorDocumental.CargarDocumentos(false);
                            }
                            Dictionary<string, string> listaPropiedades = UtilCadenas.ObtenerPropiedadesDeTexto(Documento.GestorDocumental.ListaDocumentos[Documento.FilaDocumento.ElementoVinculadoID.Value].FilaDocumento.NombreElementoVinculado);

                            if (listaPropiedades.ContainsKey(PropiedadesOntologia.urlservicioElim.ToString()))
                            {
                                string urlServElim = ControladorDocumentacion.ObtenerUrlServicioExternoSEMCMS(listaPropiedades[PropiedadesOntologia.urlservicioElim.ToString()], ProyectoSeleccionado.Clave);
                                return NotificarServicioExternoBorradoRecurso(urlServElim);
                            }
                        }
                    }

                    #endregion

                    Documento.FilaDocumento.FechaModificacion = DateTime.Now;
                    Guardar(listaProyectosActualNumRec);

                    if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                    {
                        ControladorDocumentacion.BorrarRDFDeDocumentoEliminado(Documento.VersionOriginalID, Documento.FilaDocumento.ElementoVinculadoID.Value, UrlIntragnoss, false, ProyectoSeleccionado.Clave);
                    }

                    #region Actualizar cola GnossLIVE

                    int tipo;

                    switch (Documento.TipoDocumentacion)
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

                    foreach (Guid baseRecursosID in listaBRsConProyectos.Keys)
                    {
                        string infoExtra = null;

                        if (listaBRsConProyectos[baseRecursosID] == ProyectoAD.MetaProyecto)
                        {
                            infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                        }

                        ControladorDocumentacion.ActualizarGnossLive(listaBRsConProyectos[baseRecursosID], Documento.Clave, AccionLive.Eliminado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);
                        ControladorDocumentacion.ActualizarGnossLive(listaBRsConProyectos[baseRecursosID], Documento.ObtenerPublicadorEnBR(baseRecursosID), AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);
                    }

                    #endregion

                    if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MyGnoss))
                    {
                        FacetadoCN facetadoCN_Master = null;
                        string grafoBorrar = IdentidadActual.PerfilID.ToString();

                        if (EsIdentidadBROrg)
                        {
                            grafoBorrar = IdentidadActual.OrganizacionID.Value.ToString();
                        }

                        try
                        {
                            facetadoCN_Master = new FacetadoCN("acid", UrlIntragnoss, "", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                            facetadoCN_Master.BorrarRecurso(grafoBorrar, Documento.Clave);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, mlogger);
                        }
                        finally
                        {
                            if (facetadoCN_Master != null)
                            {
                                facetadoCN_Master.Dispose();
                            }
                        }

                        FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mControladorBase.UsuarioActual.ProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                        facetadoCN.BorrarRecurso(grafoBorrar, Documento.Clave);
                        facetadoCN.Dispose();
                    }
                    else
                    {
                        // TODO: Se elimina aquí y posteriormente se vuelve a eliminar dentro del método ActualizarNumRecCatTesDeTesauroDeRecursoEliminado de abajo. hacer que solo se pase 1 vez por el base...
                        // Bug 9325 - Buscar en el código este bug. Se quita esto de aquí porque en la función ActualizarNumRecCatTesDeTesauroDeRecursoEliminado de abajo se inserta una fila en la cola del base.
                        //ControladorDocumentacion.EliminarRecursoModeloBaseSimple(Documento.Clave, ProyectoSeleccionado.Clave, (short)Documento.TipoDocumentacion);
                    }

                    if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                    {
                        ProyectoSeleccionado.FilaProyecto.NumeroRecursos--;
                    }
                    if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo)
                    {
                        ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(Documento, IdentidadActual, true);
                    }

                    ControladorDocumentacion controDoc = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                    //Si el documento que se borra es un recurso normal, ni debate, ni pregunta, ni encuesta.... se tiene que omitir la eliminación de la caché.
                    controDoc.ActualizarNumRecCatTesDeTesauroDeRecursoEliminado(Documento, mAvailableServices);
                    PublicarModificarEliminarRecurso publicarModificarEliminarRecursoModel = new PublicarModificarEliminarRecurso(ProyectoSeleccionado.Clave, Documento.Clave, mControladorBase.UsuarioActual.UsuarioID, Documento.Fecha);
                    mIPublishEvents.PublishResource(publicarModificarEliminarRecursoModel, "Eliminar");
                    string nombreSem = PestanyaRecurso.Value;

                    string nombreCortoProy = "";

                    if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                    {
                        nombreCortoProy = ProyectoSeleccionado.NombreCorto;
                    }

                    // Hay que eliminar todos los archivos que tenga sus versiones anteriores
                    foreach (Guid docID in documentoIDs)
                    {
                        ControladorDocumentacion.InsertarEnColaProcesarFicherosRecursosModificadosOEliminados(docID, TipoEventoProcesarFicherosRecursos.BorradoPersistente, mAvailableServices);
                    }

                    // Hay que eliminar logicamente los documentos que sean versiones anteriores.
                    foreach (Documento doc in GestorDocumental.ListaDocumentos.Values.Where(doc => documentoIDs.Contains(doc.Clave)))
                    {
                        if (doc.Clave.Equals(Documento.Clave)) continue;
                        GestorDocumental.EliminarDocumentoLogicamente(doc);
                    }
                    mEntityContext.SaveChanges();

                    return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, nombreCortoProy, UrlPerfil, EsIdentidadBROrg, nombreSem));
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }
            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult DeleteSelective(string[] SharedCommunities)
        {
            try
            {
                //Seguridad : 
                /*
                 * Si es la ultima version
                 * Si no esta protegido o lo ha protegido el usuario actual o el usuario actual es supervisor
                 * Si no es una wiki o el usuario actual es supervisor
                 * Si tiene permisos para editarlo
                */
                ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                bool bloqueadoPorOtroUsuario = Documento.FilaDocumento.IdentidadProteccionID.HasValue && !Documento.FilaDocumento.IdentidadProteccionID.Equals(mControladorBase.UsuarioActual.IdentidadID);

                if (Documento.FilaDocumento.UltimaVersion && (!Documento.FilaDocumento.Protegido || !bloqueadoPorOtroUsuario || EsIdentidadActualSupervisorProyecto) && (!Documento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki) || EsIdentidadActualSupervisorProyecto) && (Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)) && (GenPlantillasOWL == null || (!GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDoc && (string.IsNullOrEmpty(GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDocCondicion) || !GenPlantillasOWL.CumpleRecursoCondicion(GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDocCondicion, IdentidadActual.Clave)))))
                {
                    List<Guid> listaBaseRecursosChequeadas = new List<Guid>();

                    foreach (string brID in SharedCommunities)
                    {
                        if (!string.IsNullOrEmpty(brID))
                        {
                            Guid baseRecursosID = new Guid(brID);
                            if (Documento.BaseRecursos.Contains(baseRecursosID))
                            {
                                listaBaseRecursosChequeadas.Add(baseRecursosID);
                            }
                        }
                    }

                    if (listaBaseRecursosChequeadas.Count == Documento.BaseRecursos.Count)
                    {
                        GestorDocumental.EliminarDocumentoLogicamente(Documento);
                    }
                    else
                    {
                        foreach (Guid baseRecursosID in listaBaseRecursosChequeadas)
                        {
                            GestorDocumental.DesVincularDocumentoDeBaseRecursos(Documento.Clave, baseRecursosID, GestorDocumental.ObtenerProyectoID(baseRecursosID), IdentidadActual.Clave);
                        }
                    }

                    List<Guid> listaProyectosActualizarNumRec = new List<Guid>();
                    if (Documento.FilaDocumento.Borrador == false)
                    {
                        foreach (Guid baseRecursosID in listaBaseRecursosChequeadas)
                        {
                            Guid proyectoID = GestorDocumental.ObtenerProyectoID(baseRecursosID);
                            listaProyectosActualizarNumRec.Add(proyectoID);

                            #region Actualizar cola GnossLIVE

                            int tipo;
                            switch (Documento.TipoDocumentacion)
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

                            if (proyectoID == ProyectoAD.MetaProyecto)
                            {
                                infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                            }

                            ControladorDocumentacion.ActualizarGnossLive(proyectoID, Documento.Clave, AccionLive.Eliminado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);
                            ControladorDocumentacion.ActualizarGnossLive(proyectoID, Documento.ObtenerPublicadorEnBR(baseRecursosID), AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta, mAvailableServices);

                            #endregion
                        }
                    }

                    Documento.FilaDocumento.FechaModificacion = DateTime.Now;
                    Guardar(listaProyectosActualizarNumRec);
                    ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(Documento, IdentidadActual, true);

                    if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && Documento.FilaDocumento.Eliminado)
                    {
                        ControladorDocumentacion.BorrarRDFDeDocumentoEliminado(Documento.Clave, Documento.FilaDocumento.ElementoVinculadoID.Value, UrlIntragnoss, false, UsuarioActual.ProyectoID);
                    }

                    ControladorDocumentacion controDoc = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                    controDoc.ActualizarNumRecCatTesDeTesauroDeRecursoEliminado(Documento, mAvailableServices);

                    ControladorDocumentacion.BorrarCacheControlFichaRecursos(Documento.Clave);

                    return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, EsIdentidadBROrg));
                }
            }
            catch (Exception ex)
            {
                string modeloDatos = "";
                try
                {
                    if (SharedCommunities != null)
                    {
                        modeloDatos = JsonConvert.SerializeObject(SharedCommunities);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion DeleteSelective");
                }
                GuardarLogError(ex, " Categories: " + modeloDatos);
            }
            return GnossResultERROR();
        }

        /// <summary>
        /// Guarda el documento en la base de datos.
        /// </summary>
        /// <param name="pDocumentacionDS">DataSet de documentación</param>
        /// <param name="pListaProyectosActualizarNumRec">Lista con los proyectos a los que hay que acutulizarles el Num de Recursos</param>
        protected void Guardar(List<Guid> pListaProyectosActualizarNumRec)
        {
            DataWrapperTesauro tesauroGuardarDW = null;

            if (GestorDocumental.GestorTesauro != null)
            {
                tesauroGuardarDW = GestorDocumental.GestorTesauro.TesauroDW;
            }

            mEntityContext.SaveChanges();
            try
            {
                //Borramos la cache de las comunidades afectadas
                DocumentacionCL documentacionCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                List<Guid> listaProyectosActualYProyectosRelacionados = new List<Guid>();
                listaProyectosActualYProyectosRelacionados.AddRange(pListaProyectosActualizarNumRec);
                foreach (Guid proyecto in pListaProyectosActualizarNumRec)
                {
                    int tipoDoc = -1;
                    if (Documento.TipoDocumentacion == TiposDocumentacion.Debate || Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                    {
                        tipoDoc = (int)Documento.TipoDocumentacion;
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

        /// <summary>
        /// Notifica a un servicio externo que se ha borrado un recurso.
        /// </summary>
        /// <param name="pUrlServicio">Url del servicio de borrado</param>
        /// <returns>TRUE si ha ido bien, FALSE en caso contrario</returns>
        private ActionResult NotificarServicioExternoBorradoRecurso(string pUrlServicio)
        {
            string entidadID = null;
            string respuesta = null;
            Stopwatch sw = null;

            try
            {
                mLoggingService.AgregarEntrada("Inicio NotificarServicioExternoBorradoRecurso con url '" + pUrlServicio + "'");

                //Obtenemos el RDF del recurso para extraer la entidad principal:
                ControladorDocumentacion contrDoc = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                Ontologia ontologia = null;
                string rdfTexto = null;
                string nombreOntologia = null;
                List<ElementoOntologia> instanciasPrincipales = ObtenerSemCmsController(Documento).ObtenerEntidadesPrincipalesRecursoDeBD(Documento, Documento.ElementoVinculadoID, BaseURLFormulariosSem, UrlIntragnoss, UtilIdiomas, ProyectoSeleccionado, out nombreOntologia, out ontologia, out rdfTexto, null);
                byte[] rdfArrayBytes = Encoding.UTF8.GetBytes(rdfTexto);

                mLoggingService.AgregarEntrada("Tras obtener instanciasPrincipales");

                entidadID = instanciasPrincipales[0].Uri;

                #region Llamamos al servicio:

                sw = LoggingService.IniciarRelojTelemetria();

                Dictionary<string, string> parametros = new Dictionary<string, string>();
                parametros.Add("ResourceID", Documento.Clave.ToString());
                parametros.Add("RDFBytes", Convert.ToBase64String(rdfArrayBytes));
                parametros.Add("UserID", mControladorBase.UsuarioActual.UsuarioID.ToString());
                parametros.Add("CommunityShortName", ProyectoSeleccionado.NombreCorto);
                parametros.Add("UserLanguaje", UtilIdiomas.LanguageCode);

                respuesta = UtilWeb.HacerPeticionPost(pUrlServicio, parametros);
                JsonExtServResponse jsonRes = JsonConvert.DeserializeObject<JsonExtServResponse>(respuesta);

                mLoggingService.AgregarEntradaDependencia($"Enviar al servicio de quitar publicacion '{pUrlServicio}' el documento '{Documento.Clave}' con entidad principal '{entidadID}'", false, "NotificarServicioExternoBorradoRecurso", sw, true);

                return GestionarRespuestaServicioExterno(jsonRes);

                #endregion
            }
            catch (Exception ex)
            {
                try
                {
                    string fichero = Path.Combine(mEnv.WebRootPath, mHttpContextAccessor.HttpContext.Request.Path, "logs", "errorIntegServicioEliminacion.log");
                    ControladorBase.GuardarLogSize("Error Integración Serv Eliminar externo '" + pUrlServicio + "', con email '" + IdentidadActual.Persona.FilaPersona.Email + "', entidadID='" + entidadID + "'" + Environment.NewLine + ex.ToString(), fichero);

                    mLoggingService.AgregarEntradaDependencia($"Error Integracion Servicio Eliminar externo '{pUrlServicio}' con email '{IdentidadActual.Persona.FilaPersona.Email}' y entidad principal '{entidadID}'", false, "NotificarServicioExternoBorradoRecurso", sw, false);
                }
                catch (Exception ex2)
                {
                    mLoggingService.GuardarLogError(ex2, mlogger);
                }

                throw;
            }
        }

        #endregion

        #region Certificar

        public ActionResult Certify(Guid CertificationID)
        {
            try
            {
                //Seguridad : 
                /*
                    * Si es la ultima version
                    * Si tiene permiso para certificar recursos
                    * Si permite la comunidad Certificaciones
                    * Si el proyecto no es el MetaProyecto
                    * Si la ontologia permite Certificaciones
                */
                UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
                bool tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.CertificarRecurso, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                if (Documento.FilaDocumento.UltimaVersion && tienePermiso && ParametrosGeneralesRow.PermitirCertificacionRec && ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonCertificarDoc))
                {
                    return AccionRecurso_Certificar_Aceptar(CertificationID);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" CertificationID: {CertificationID}");
            }
            return GnossResultERROR();
        }

        /// <summary>
        /// Se ejecuta al certificar un recurso
        /// </summary>
        /// <param name="pFuncion">Función de los argumentos del callback</param>
        private ActionResult AccionRecurso_Certificar_Aceptar(Guid pCertificacionID)
        {
            try
            {
                Guid idCertificadoNuevo = pCertificacionID;
                Guid idCertificadoAntiguo = Guid.Empty;

                if (Documento.FilaDocumentoWebVinBR.NivelCertificacionID.HasValue)
                {
                    idCertificadoAntiguo = Documento.FilaDocumentoWebVinBR.NivelCertificacionID.Value;
                }

                AccionHistorialDocumento accion = AccionHistorialDocumento.CertificarDoc;
                if (idCertificadoNuevo != Guid.Empty)
                {
                    Documento.FilaDocumentoWebVinBR.NivelCertificacionID = idCertificadoNuevo;
                    Documento.FilaDocumentoWebVinBR.FechaCertificacion = DateTime.Now;
                }
                else
                {
                    Documento.FilaDocumentoWebVinBR.NivelCertificacionID = null;
                    Documento.FilaDocumentoWebVinBR.FechaCertificacion = null;
                    accion = AccionHistorialDocumento.EliminarCertificacionDoc;
                }

                Documento.GestorDocumental.AgregarEliminarCategoriaTesauroHistorial(Documento, Guid.Empty, accion, mControladorBase.UsuarioActual.ProyectoID, UsuarioActual.ProyectoID);

                if (idCertificadoAntiguo != idCertificadoNuevo)
                {
                    int tipoLive = (int)TipoLive.Recurso;

                    if (Documento.TipoDocumentacion == TiposDocumentacion.Debate)
                    {
                        tipoLive = (int)TipoLive.Debate;
                    }
                    else if (Documento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                    {
                        tipoLive = (int)TipoLive.Pregunta;
                    }

                    string infoExtra = null;

                    if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto)
                    {
                        infoExtra = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                    }

                    ControladorDocumentacion.ActualizarGnossLive(mControladorBase.UsuarioActual.ProyectoID, Documento.Clave, AccionLive.RecursoCertificado, tipoLive, PrioridadLive.Alta, infoExtra, mAvailableServices);
                    if (!Documento.FilaDocumentoWebVinBR.NivelCertificacionID.HasValue)
                    {
                        ControladorDocumentacion.ActualizarGnossLivePopularidad(mControladorBase.UsuarioActual.ProyectoID, Documento.Clave, idCertificadoAntiguo, AccionLive.RecursoDesCertificado, (int)TipoLive.Recurso, (int)TipoLive.Recurso, false, PrioridadLive.Alta, mAvailableServices);
                    }

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    docCN.ActualizarDocumentacion();
                    docCN.Dispose();



                    ControladorDocumentacion controDoc = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                    controDoc.ActualizarNumRecCatTesDeTesauro(false, Guid.Empty, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.ProyectoID, Documento.Clave, mAvailableServices);
                    ControladorDocumentacion.AgregarRecursoModeloBaseSimple(Documento.Clave, ProyectoSeleccionado.Clave, (short)Documento.TipoDocumentacion, PrioridadBase.Alta, mAvailableServices);

                    DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    docCL.InvalidarFichaRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);
                    docCL.InvalidarEventosRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);
                    docCL.Dispose();

                }
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }

            return GnossResultOK();
        }

        #endregion

        #region Enviar Newsletter

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult SendNewsletter(string Language)
        {
            try
            {
                //Seguridad :
                /*
                * Si el tipo es una newsletter y no es borrador
                * si el proyecto es distinto al metaproyecto
                * Si eres Administrador del proyecto actual
                */
                if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && Documento.TipoDocumentacion == TiposDocumentacion.Newsletter && ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) && !Documento.EsBorrador)
                {
                    string idioma = Language;
                    return AccionRecurso_EnviarNewsletter_Aceptar(idioma);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" Language: {Language}");
            }
            return GnossResultERROR();
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult SendNewsletterGroups(string Language, string[] Groups)
        {
            try
            {
                //Seguridad :
                /*
                * Si el tipo es una newsletter y no es borrador
                * si el proyecto es distinto al metaproyecto
                * Si eres supervisor
                * Si la comunidad tiene grupos, solo a los grupos de la comunidad
                */
                if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && Documento.TipoDocumentacion == TiposDocumentacion.Newsletter && ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) && !Documento.EsBorrador)
                {
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGruposDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCN.Dispose();

                    if (gestorIdentidades.ListaGrupos.Count > 0)
                    {
                        string idioma = Language;

                        List<Guid> listaGrupos = new List<Guid>();
                        foreach (string grupo in Groups)
                        {
                            try
                            {
                                Guid grupoID = new Guid(grupo);
                                if (!listaGrupos.Contains(grupoID) && gestorIdentidades.ListaGrupos.ContainsKey(grupoID))
                                {
                                    listaGrupos.Add(grupoID);
                                }
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError(ex.ToString());
                            }
                        }

                        if (listaGrupos.Any())
                        {
                            return AccionRecurso_EnviarNewsletterGrupos_Aceptar(idioma, listaGrupos);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string modeloGroups = "";
                try
                {
                    if (Groups != null)
                    {
                        modeloGroups = JsonConvert.SerializeObject(Groups);
                    }
                }
                catch (Exception ex2)
                {
                    GuardarLogError(ex2, "Error al intentar serializar el modelo de datos de la accion SendNewsletterGroups");
                }
                GuardarLogError(ex, $" Groups: {modeloGroups} Language: {Language}");
            }

            return GnossResultERROR();
        }

        /// <summary>
        /// Se ejecuta al seleccionar 'SI' al eliminar un recurso
        /// </summary>
        /// <param name="pIdioma">Clave del idioma</param>
        protected ActionResult AccionRecurso_EnviarNewsletter_Aceptar(string pIdioma)
        {
            try
            {
                ControladorDocumentacion.EnviarCorreoNewsLetter(Documento, pIdioma, null, IdentidadActual.Clave, mAvailableServices);
                return GnossResultOK(UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "NEWSLETTEROK"));
            }
            catch (Exception ex)
            {
                GuardarLogError(ex.ToString());
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "NEWSLETTERKO"));
            }
        }

        /// <summary>
        /// Se ejecuta al seleccionar 'SI' al eliminar un recurso
        /// </summary>
        /// <param name="pIdioma">Idioma de la newsletter</param>
        /// <param name="pArg">Argumentos</param>
        protected ActionResult AccionRecurso_EnviarNewsletterGrupos_Aceptar(string pIdioma, List<Guid> listaGrupos)
        {
            try
            {
                ControladorDocumentacion.EnviarCorreoNewsLetter(Documento, pIdioma, listaGrupos, IdentidadActual.Clave, mAvailableServices);
                return GnossResultOK(UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "NEWSLETTEROK"));
            }
            catch (Exception ex)
            {
                GuardarLogError(ex.ToString());
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "NEWSLETTERKO"));
            }
        }

        #endregion

        #region Grafo Recurso

        [TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LoadGraph()
        {
            try
            {
                if (Request.Method.Equals("GET"))
                {
                    return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, IdentidadActual.EsOrganizacion));
                }
                else if (Request.Method.Equals("POST"))
                {
                    string urlintragnoss = RequestParams("urlintragnoss");
                    Guid documentoID = new Guid(RequestParams("docID"));

                    string propEnlace = RequestParams("propEnlace");
                    int nodosLimiteNivel = int.Parse(RequestParams("nodosLimiteNivel"));
                    string extra = RequestParams("extra");
                    string tipoRecurso = RequestParams("tipoRecurso");
                    string grafoDbpedia = ParametrosAplicacionDS.FirstOrDefault(item => item.Parametro.Equals(TiposParametrosAplicacion.URIGrafoDbpedia))?.Valor;
                    string idioma = mControladorBase.UsuarioActual.Idioma;
                    if (string.IsNullOrEmpty(idioma))
                    {
                        if (!string.IsNullOrEmpty(mControladorBase.IdiomaUsuario))
                        {
                            idioma = mControladorBase.IdiomaUsuario;
                        }
                        else
                        {
                            idioma = mControladorBase.IdiomaPorDefecto;
                        }
                    }

                    FacetadoCN facCN = new FacetadoCN(urlintragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    string datos = facCN.ObtenerRelacionesGrafoGraficoDeDocumento(ProyectoSeleccionado.Clave.ToString(), documentoID, propEnlace, nodosLimiteNivel, extra, idioma, tipoRecurso, grafoDbpedia);
                    facCN.Dispose();

                    return GnossResultOK(datos);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                throw;
            }
        }

        [TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult CallbackGraph()
        {
            try
            {
                if (Request.Method.Equals("GET"))
                {
                    return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, IdentidadActual.EsOrganizacion));
                }
                else if (Request.Method.Equals("POST"))
                {
                    try
                    {
                        Guid ontologia = Guid.Empty;

                        if (!string.IsNullOrEmpty(RequestParams("ontologia")))
                        {
                            ontologia = new Guid(RequestParams("ontologia"));
                        }

                        string propConexion = RequestParams("propConexion");
                        string tipoEntidad = RequestParams("tipoEntidad");
                        string subTiposEntidad = RequestParams("subTiposEntidad");


                        string respuesta = ControladorDocumentacion.ObtenerNombrePropiedadGrafoGraficoOntologia(ontologia, ProyectoSeleccionado.Clave, propConexion, tipoEntidad, subTiposEntidad, mControladorBase.UsuarioActual.Idioma);

                        if (respuesta == null)
                        {
                            respuesta = "";
                        }

                        return GnossResultOK(respuesta);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, mlogger);
                    }

                    return GnossResultERROR();
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                throw;
            }
        }

        #endregion

        #region Acciones SEMCMS

        [TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult ActionSemCms()
        {
            if (Request.Method.Equals("GET"))
            {
                return RedirectPermanent(Request.Path.ToString().Replace("/actionsemcms", ""));
            }

            try
            {
                if (Request.Method.Equals("GET"))
                {
                    return Redirect(this.Url.ToString().Replace("/actionsemcms", ""));
                }

                string accionID = RequestParams("ActionID");
                string entidadID = RequestParams("ActionEntityID");

                if (mGenPlantillasOWL.Ontologia.ConfiguracionPlantilla.Acciones.ContainsKey(accionID))
                {
                    ActionResult respuesta = null;

                    foreach (AccionSemCms.Accion accionInt in mGenPlantillasOWL.Ontologia.ConfiguracionPlantilla.Acciones[accionID].Acciones)
                    {
                        if (accionInt.TipoAccion == AccionSemCms.TipoAccion.EnviarServExterno)
                        {
                            respuesta = LLamarServicioExternoAccionSemCms(accionInt.UrlServExterno, mGenPlantillasOWL.ResourceRDF, entidadID);
                        }
                    }

                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
        }

        /// <summary>
        /// Llama a un servicio externos a gnoss para una acción del SEMCMS.
        /// </summary>
        /// <param name="pUrlServicio">Url del servicio</param>
        /// <param name="pRDFRecurso">RDF del recurso</param>
        /// <param name="pEntidadAccionID">ID de la entidad sobre la que se hace la acción</param>
        /// <returns>Respuesta del servicio externo</returns>
        private ActionResult LLamarServicioExternoAccionSemCms(string pUrlServicio, string pRDFRecurso, string pEntidadAccionID)
        {
            pUrlServicio = ControladorDocumentacion.ObtenerUrlServicioExternoSEMCMS(pUrlServicio, ProyectoSeleccionado.Clave);
            byte[] rdfArrayBytes = Encoding.UTF8.GetBytes(pRDFRecurso);

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("ResourceID", Documento.Clave.ToString());
            parametros.Add("UserID", mControladorBase.UsuarioActual.UsuarioID.ToString());
            parametros.Add("CommunityShortName", ProyectoSeleccionado.NombreCorto);
            parametros.Add("RDFBytes", Convert.ToBase64String(rdfArrayBytes));
            parametros.Add("SelectedEntities", pEntidadAccionID);
            parametros.Add("UserLanguaje", UtilIdiomas.LanguageCode);

            string conexionAfinidadVirtuoso = mServicesUtilVirtuosoAndReplication.ConexionAfinidad;

            string tokenAfinidadPeticion = Guid.NewGuid().ToString();

            //if (string.IsNullOrEmpty(conexionAfinidadVirtuoso))
            //{
            //    conexionAfinidadVirtuoso = (string)mGnossCache.ObtenerDeCache("conexionAfinidadVirtuoso_" + tokenAfinidadPeticion);
            //    if (string.IsNullOrEmpty(conexionAfinidadVirtuoso))
            //    {
            //        conexionAfinidadVirtuoso = mConfigService.ObtenerVirtuosoEscritura().Value;
            //    }
            //}

            Dictionary<string, string> cabeceras = null;
            if (!string.IsNullOrEmpty(conexionAfinidadVirtuoso))
            {
                // Añadir cabecera afinidad
                cabeceras = new Dictionary<string, string>();
                cabeceras.Add("X-Request-ID", tokenAfinidadPeticion);
                mGnossCache.AgregarObjetoCache("conexionAfinidadVirtuoso_" + tokenAfinidadPeticion, conexionAfinidadVirtuoso, 86400);
            }

            string respuesta = UtilWeb.HacerPeticionPost(pUrlServicio, parametros, cabeceras);
            JsonExtServResponse jResponse = JsonConvert.DeserializeObject<JsonExtServResponse>(respuesta);

            return GestionarRespuestaServicioExterno(jResponse);
        }

        /// <summary>
        /// gestiona una respuesta de un servicio externo.
        /// </summary>
        /// <param name="pJResponse">Respuesta</param>
        /// <returns>Acción en función de la respuesta</returns>
        private ActionResult GestionarRespuestaServicioExterno(JsonExtServResponse pJResponse)
        {
            if (pJResponse.Status == 0)
            {
                return GnossResultERROR(pJResponse.Message);
            }
            else
            {
                if (pJResponse.Action == null)
                {
                    return GnossResultOK(pJResponse.Message);
                }
                else if (pJResponse.Action.RedirectCommunityHome)
                {
                    return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
                else if (pJResponse.Action.RedirectResource)
                {
                    return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, EsIdentidadBROrg));
                }
                else
                {
                    return GnossResultUrl(pJResponse.Action.RedirectUrl);
                }
            }
        }

        #endregion

        #region Selector Entidad SEMCMS

        [TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LoadMoreEntitiesSelector()
        {
            if (HttpContext.Request.Method != "POST")
            {
                return Index();
            }

            try
            {
                string propiedad = RequestParams("propiedad");
                string entidad = RequestParams("entidad");
                int inicioPag = 0;
                int.TryParse(RequestParams("inicioPag"), out inicioPag);

                if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    SemanticPropertyModel propModel = ObtenerSemCmsController(Documento).ObtenerPropiedadControladorSemCMS(Documento, ProyectoSeleccionado, IdentidadActual, BaseURLFormulariosSem, UtilIdiomas, BaseURL, BaseURLIdioma, BaseURLContent, BaseURLStatic, UrlIntragnoss, VistaPersonalizada, ParametroProyecto, entidad, propiedad, inicioPag);

                    if (propModel.SpecificationProperty.SelectorEntidad != null)
                    {
                        if (propModel.SpecificationProperty.SelectorEntidad.VistaPersonalizadaPaginacion != null)
                        {
                            return GnossResultHtml(propModel.SpecificationProperty.SelectorEntidad.VistaPersonalizadaPaginacion, propModel);
                        }
                        else
                        {
                            return GnossResultHtml("SemCms/_PropiedadOntoSelectorEntidadLectura", propModel);
                        }
                    }
                    else
                    {
                        if (propModel.SpecificationProperty.VistaPersonalizadaPaginacion != null)
                        {
                            return GnossResultHtml(propModel.SpecificationProperty.VistaPersonalizadaPaginacion, propModel);
                        }
                        else
                        {
                            return GnossResultHtml("SemCms/_PropiedadOntoObject", propModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return GnossResultERROR(null);
        }

        #endregion

        #region Cargar Panel Accion...

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accion"></param>
        /// <param name="idNuevaCategoria">id de la nueva categoria creada (opcional)</param>
        /// <returns></returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad }), TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult LoadAction(string accion, Guid? idNuevaCategoria = null)
        {
            try
            {
                if (!Request.Method.Equals("POST"))
                {
                    string urlRedirect = Request.Path.ToString();
                    urlRedirect = urlRedirect.Substring(0, urlRedirect.IndexOf("/load-action/"));
                    return RedirectPermanent(urlRedirect);
                }

                List<string> BaseURLsContent = new List<string>();
                BaseURLsContent.Add(BaseURLContent);
                string nombreSem = PestanyaRecurso.Value;
                string baseUrlBusqueda = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, EsIdentidadBROrg, nombreSem) + "/";
                List<Guid> listaRecursosID = new List<Guid>();
                listaRecursosID.Add(DocumentoID);
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

                Guid proyectoOrigenID = ObtenerProyectoOrigenFichaRecurso(Documento.Clave, ProyectoSeleccionado.Clave);

                // TODO: Ajustar propiedad para que tenga en cuenta el proyectoOrigenID

                ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLsContent, BaseURLStatic, ProyectoSeleccionado, proyectoOrigenID, ParametrosGeneralesRow, IdentidadActual, mControladorBase.EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);
                Dictionary<Guid, ResourceModel> listaRecursos = controladorMVC.ObtenerRecursosPorID(listaRecursosID, baseUrlBusqueda, espacioPersonal, null, false);

                paginaModel.Resource = listaRecursos[DocumentoID];

                if (accion == "delete")
                {
                    //Solo pintado
                    ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);
                    paginaModel.IsDocumentEditor = (Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)) && Documento.BaseRecursos.Count > 1;
                }
                else if (accion == "delete-selective")
                {
                    //Solo pintado
                    paginaModel.Resource.Shareds = CargarBRCompartidasParaEliminar();
                }
                else if (accion == "certify")
                {
                    //Solo pintado
                    if (Documento.FilaDocumentoWebVinBR.NivelCertificacionID.HasValue)
                    {
                        ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                        ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion.Union(proyectoCL.ObtenerNivelesCertificacionRecursosProyecto(ProyectoSeleccionado.Clave).ListaNivelCertificacion).ToList();
                        paginaModel.Resource.Certification = new KeyValuePair<Guid, string>(Documento.FilaDocumentoWebVinBR.NivelCertificacionID.Value, ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion.FirstOrDefault(nivelCert => nivelCert.NivelCertificacionID.Equals(Documento.FilaDocumentoWebVinBR.NivelCertificacionID)).Descripcion);
                    }
                    Comunidad.ListaCertificaciones = CargarListaCertificaciones();
                }
                else if (accion == "restore-version")
                {
                    //Solo pintado
                    paginaModel.Resource.LastVersion = Documento.Clave;
                    if (!Documento.UltimaVersionID.Equals(Guid.Empty))
                    {
                        paginaModel.Resource.LastVersion = Documento.UltimaVersionID;
                    }
                }
                else if (accion == "add-tags")
                {
                    //Solo pintado
                    paginaModel.AllowEditTags = Documento.TienePermisosIdentidadEditarTagsYCategoriasEnBR(IdentidadActual, Documento.GestorDocumental.BaseRecursosIDActual, UsuarioActual.ProyectoID, EsIdentidadActualSupervisorProyecto, EsIdentidadActualAdministradorOrganizacion);
                    paginaModel.Resource.UrlSearch = CargarUrlBusqueda();
                    paginaModel.Resource.Tags = CargarEtiquetas();
                }
                else if (accion == "add-categories")
                {
                    //Solo pintado
                    paginaModel.AllowEditCategories = Documento.TienePermisosIdentidadEditarTagsYCategoriasEnBR(IdentidadActual, Documento.GestorDocumental.BaseRecursosIDActual, UsuarioActual.ProyectoID, EsIdentidadActualSupervisorProyecto, EsIdentidadActualAdministradorOrganizacion);

                    paginaModel.Resource.UrlSearch = CargarUrlBusqueda();

                    paginaModel.Resource.Categories = CargarCategoriasTesauro();

                    if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                    {
                        if (GestorDocumental.GestorTesauro == null)
                        {
                            CargarTesauro();
                        }

                        List<CategoryModel> listaCategorias = new List<CategoryModel>();

                        foreach (CategoriaTesauro catTes in GestorDocumental.GestorTesauro.ListaCategoriasTesauro.Values)
                        {
                            CategoryModel categoriaTesauro = new CategoryModel();
                            categoriaTesauro.Key = catTes.Clave;
                            categoriaTesauro.Name = catTes.FilaCategoria.Nombre;
                            categoriaTesauro.LanguageName = catTes.FilaCategoria.Nombre;

                            listaCategorias.Add(categoriaTesauro);
                        }
                        paginaModel.Categories = listaCategorias;
                    }
                    else
                    {
                        paginaModel.Categories = Comunidad.Categories;
                    }
                }
                else if (accion == "share" || accion == "duplicate")
                {
                    //Solo pintado
                    Dictionary<Guid, KeyValuePair<string, string>> listaBasesDeRecursos = new Dictionary<Guid, KeyValuePair<string, string>>();
                    Dictionary<string, KeyValuePair<string, string>> listaComunidadesUsuario = new Dictionary<string, KeyValuePair<string, string>>();
                    CargarBasesRecursosCompartir(out listaBasesDeRecursos, out listaComunidadesUsuario);

                    paginaModel.ResourceBases = listaBasesDeRecursos;
                    paginaModel.UserComunities = listaComunidadesUsuario;

                    bool esOntologia = (paginaModel.Resource.TypeDocument.Equals(DocumentType.Ontologia) || paginaModel.Resource.TypeDocument.Equals(DocumentType.OntologiaSecundaria));

                    if (listaBasesDeRecursos != null && listaBasesDeRecursos.Count > 0 && !esOntologia)
                    {
                        paginaModel.Categories = CargarTesauroPorBR(listaBasesDeRecursos.First().Key);
                    }
                    else
                    {
                        paginaModel.Categories = new List<CategoryModel>();
                    }


                    paginaModel.IsDocumentEditor = new AccionesRecurso(this, mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mHttpContextAccessor, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AccionesRecurso>(), mLoggerFactory).TienePermisosEditarDoc(Documento, IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, GenPlantillasOWL, false);
                }
                else if (accion == "share-change")
                {
                    //Solo pintado
                    Guid brSeleccionada = new Guid(RequestParams("SelectedBR"));

                    MultiViewResult result = new MultiViewResult(this, mViewEngine);

                    bool esOntologia = (paginaModel.Resource.TypeDocument.Equals(DocumentType.Ontologia) || paginaModel.Resource.TypeDocument.Equals(DocumentType.OntologiaSecundaria));

                    if (!esOntologia)
                    {
                        paginaModel.Categories = CargarTesauroPorBR(brSeleccionada);
                    }
                    else
                    {
                        paginaModel.Categories = new List<CategoryModel>();
                    }

                    result.AddView("_FichaCategoriaTesauroArbol", "categorias", paginaModel);

                    result.AddContent("", "ProyPrivado");

                    return result;
                }
                else if (accion == "add-personal-space")
                {
                    //Solo pintado
                    paginaModel.NewCategory = idNuevaCategoria ?? Guid.Empty;
                    paginaModel.Categories = CargarTesauroPerfil();
                    paginaModel.PersonalSpace = UtilCadenas.ObtenerTextoDeIdioma(EspacioPersonal, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }
                else if (accion == "lock-unlock-comments")
                {
                    paginaModel.Resource.AllowComments = ParametrosGeneralesRow.ComentariosDisponibles;
                    if (Documento.FilaDocumentoWebVinBR != null)
                    {
                        paginaModel.Resource.AllowComments = (Documento.FilaDocumentoWebVinBR.PermiteComentarios && ParametrosGeneralesRow.ComentariosDisponibles);
                    }
                }
                else if (accion == "add-metatitle" || accion == "add-metadescription")
                {
                    DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    AD.EntityModel.Models.Documentacion.DocumentoMetaDatos docMetas = docCL.ObtenerMetaDatos(Documento.FilaDocumento.DocumentoID);
                    docCL.Dispose();
                    if (docMetas != null)
                    {
                        if (accion == "add-metatitle" && !string.IsNullOrEmpty(docMetas.MetaTitulo))
                        {
                            paginaModel.Resource.MetaTitle = docMetas.MetaTitulo;
                        }
                        else if (accion == "add-metadescription" && !string.IsNullOrEmpty(docMetas.MetaDescripcion))
                        {
                            paginaModel.Resource.MetaDescription = docMetas.MetaDescripcion;
                        }
                        else
                        {
                            paginaModel.Resource.MetaTitle = "";
                            paginaModel.Resource.MetaDescription = "";
                        }
                    }
                }
                else if (accion == "transition")
                {
                    Guid transicionID = new Guid(RequestParams("pTransicionID"));
                    paginaModel.Transition = transicionID;
                }
                else if (accion == "transition-history")
                {
                    paginaModel.Resource.Estado = ObtenerEstadoDocumento();
                    if (paginaModel.Resource.Estado != null)
                    {
                        UtilFlujos utilFlujos = new UtilFlujos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilFlujos>(), mLoggerFactory);
                        paginaModel.Resource.HistorialTransiciones = utilFlujos.ObtenerHistorialDeTransiciones(Documento.Clave, TipoContenidoFlujo.Recurso, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    }
                }
                else if (accion == "load-modal-apply-improvement")
                {
                    // TODO: Rellenar las acciones
                }
                else if (accion == "load-modal-cancel-improvement")
                {
                    // TODO: Rellenar las acciones
                }

                paginaModel.Action = accion;
                ParametroAplicacionCN parametroCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrive = parametroCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                paginaModel.Resource.EsEnlaceSharepoint = UtilCadenas.EsEnlaceSharepoint(Documento.Enlace, oneDrive);
                string sharepointConfigurado = parametroCN.ObtenerParametroAplicacion("SharepointClientID");
                if (string.IsNullOrEmpty(sharepointConfigurado))
                {
                    paginaModel.Resource.EsEnlaceSharepoint = false;
                }

                return PartialView("_FichaAcciones", paginaModel);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $" accion: {accion} idNuevaCategoria: {idNuevaCategoria}");
                throw;
            }
        }

        private List<SharedBRModel> CargarBRCompartidasParaEliminar()
        {
            List<Guid> ProyectosID = new List<Guid>();
            foreach (Guid baseRecursos in Documento.BaseRecursos)
            {
                ProyectosID.Add(Documento.GestorDocumental.ObtenerProyectoID(baseRecursos));
            }
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto proyectosCompartidosDWP = proyCN.ObtenerProyectosPorIDsCargaLigera(ProyectosID);
            proyCN.Dispose();

            List<SharedBRModel> listaBRCompartidas = new List<SharedBRModel>();

            foreach (Guid baseRecursosID in Documento.BaseRecursos)
            {
                bool puedeEliminarla = false;
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDocWeb = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(baseRecursosID) && doc.DocumentoID.Equals(Documento.Clave)).ToList();

                puedeEliminarla = Documento.TienePermisosIdentidadEliminarRecursoEnBR(IdentidadActual, baseRecursosID, ProyectoSeleccionado, UsuarioActual.UsuarioID, UsuarioActual.ProyectoID, EsIdentidadActualAdministradorOrganizacion, EsIdentidadActualSupervisorProyecto);

                if (puedeEliminarla)
                {
                    Guid proyectoID = Documento.GestorDocumental.ObtenerProyectoID(baseRecursosID);

                    AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = proyectosCompartidosDWP.ListaProyecto.FirstOrDefault(proy => proy.ProyectoID.Equals(proyectoID));

                    SharedBRModel brCompartida = new SharedBRModel();
                    brCompartida.Key = baseRecursosID;
                    brCompartida.Name = UtilCadenas.ObtenerTextoDeIdioma(filaProy.Nombre, IdiomaUsuario, IdiomaPorDefecto);
                    brCompartida.ProyectKey = filaProy.ProyectoID;

                    listaBRCompartidas.Add(brCompartida);
                }
            }

            return listaBRCompartidas;
        }

        private Dictionary<Guid, string> CargarListaCertificaciones()
        {
            Dictionary<Guid, string> ListaCertificaciones = new Dictionary<Guid, string>();

            if (NivelesCertificacionProyecto.ListaNivelCertificacion.Count > 0)
            {
                foreach (NivelCertificacion fila in NivelesCertificacionProyecto.ListaNivelCertificacion.OrderBy(nivelCert => nivelCert.Orden).ToList())
                {
                    ListaCertificaciones.Add(fila.NivelCertificacionID, fila.Descripcion);
                }
            }

            return ListaCertificaciones;
        }

        /// <summary>
        /// Carga las Bases de Recursos de el usuario.
        /// </summary>
        public void CargarBasesRecursosCompartir(out Dictionary<Guid, KeyValuePair<string, string>> listaBasesDeRecursos, out Dictionary<string, KeyValuePair<string, string>> listaComunidadesUsuario)
        {
            listaBasesDeRecursos = new Dictionary<Guid, KeyValuePair<string, string>>();
            listaComunidadesUsuario = new Dictionary<string, KeyValuePair<string, string>>();

            #region Inspecciono las BR donde ya está compartido el recurso

            bool recursoPrivadoParaEditores = Documento.FilaDocumentoWebVinBR.PrivadoEditores;

            //Obtener bases de recursos donde esta compartido el documento
            List<Guid> listaBRYaCompartido = new List<Guid>();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki))
            {
                listaBRYaCompartido = docCN.ObtenerBREstaCompartidoWikiPorNombre(Documento.Titulo);
            }
            else
            {
                listaBRYaCompartido = docCN.ObtenerBREstaCompartidoDocPorID(Documento.Clave);
            }

            docCN.Dispose();

            #endregion

            TiposDocumentacion tipoDocumento = Documento.TipoDocumentacion;

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto dataWrapperProyecto = proyectoCN.ObtenerProyectosUsuarioPuedeCompartirRecurso(mControladorBase.UsuarioActual.UsuarioID, tipoDocumento);
            GestionProyecto gestorProyectos = new GestionProyecto(dataWrapperProyecto, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
            proyectoCN.Dispose();

            Dictionary<Identidad, string> listaIdentidadNombrePerfil = new Dictionary<Identidad, string>();
            SortedDictionary<string, List<SortedDictionary<string, Identidad>>> listaPerfilIdentProy = new SortedDictionary<string, List<SortedDictionary<string, Identidad>>>();
            SortedDictionary<string, Identidad> listaIdentidadProyPERFILPERSONAL = new SortedDictionary<string, Identidad>();
            SortedDictionary<string, Identidad> listaIdentidadProyPERFILPROFESOR = new SortedDictionary<string, Identidad>();
            SortedDictionary<string, Identidad> listaIdentidadProyPERFILORGANIZACION = new SortedDictionary<string, Identidad>();

            #region Cargo listas identidades proyectos perfil

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerPerfilesDePersona(mControladorBase.UsuarioActual.PersonaID, true, mControladorBase.UsuarioActual.IdentidadID, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            int contadorListaIdentidadProyPERFILORGANIZACION = 0;
            foreach (Perfil perfil in gestorIdentidades.ListaPerfiles.Values)
            {
                bool BRorganizacionConPermiso = (!perfil.PersonaID.HasValue && perfil.FilaRelacionPerfil != null && (Guid)UtilReflection.GetValueReflection(perfil.FilaRelacionPerfil, "OrganizacionID") == perfil.OrganizacionID && (mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion, (Guid)UtilReflection.GetValueReflection(perfil.FilaRelacionPerfil, "OrganizacionID")) || mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, (Guid)UtilReflection.GetValueReflection(perfil.FilaRelacionPerfil, "OrganizacionID"))));

                if ((perfil.PersonaID.HasValue && (Guid)UtilReflection.GetValueReflection(perfil.FilaRelacionPerfil, "PersonaID") == mControladorBase.UsuarioActual.PersonaID) || BRorganizacionConPermiso)
                {
                    SortedDictionary<string, Identidad> listaIdentidadProy = new SortedDictionary<string, Identidad>();

                    foreach (Identidad identidad in perfil.Hijos)
                    {
                        if (gestorProyectos.ListaProyectos.ContainsKey(identidad.FilaIdentidad.ProyectoID))
                        {
                            //Daba fallo de key duplicada en dictionary.
                            if (!listaIdentidadProy.ContainsKey(gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto.NombreCorto))
                            {
                                //Probar guardar por id en vez de por nombre...
                                listaIdentidadProy.Add(gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto.NombreCorto, identidad);
                            }
                            if (identidad.ModoParticipacion.Equals(TiposIdentidad.ProfesionalCorporativo))
                            {
                                listaIdentidadNombrePerfil.Add(identidad, identidad.NombreOrganizacion);
                            }
                            else if (identidad.ModoParticipacion == TiposIdentidad.Organizacion)
                            {
                                if (identidad.FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto)
                                {
                                    listaIdentidadProyPERFILORGANIZACION.Add(UtilIdiomas.GetText("ANYADIRGNOSS", "BRORGANIZACION") + perfil.NombreOrganizacion + contadorListaIdentidadProyPERFILORGANIZACION, identidad);
                                    contadorListaIdentidadProyPERFILORGANIZACION++;
                                }
                            }
                            else
                            {
                                listaIdentidadNombrePerfil.Add(identidad, identidad.Nombre());
                            }
                        }
                    }

                    if (!perfil.OrganizacionID.HasValue || (perfil.PersonaID.HasValue && perfil.IdentidadMyGNOSS != null && perfil.IdentidadMyGNOSS.IdentidadPersonalMyGNOSS == null))
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
                    else
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

            #endregion

            #region Cargamos listaProyIDBRID

            List<Guid> listaProyCompartirID = new List<Guid>();
            foreach (Guid proyID in gestorProyectos.ListaProyectos.Keys)
            {
                if (recursoPrivadoParaEditores)
                {
                    Elementos.ServiciosGenerales.Proyecto proy = gestorProyectos.ListaProyectos[proyID];
                    if (proy.TipoAcceso == TipoAcceso.Publico || proy.TipoAcceso == TipoAcceso.Restringido)
                    {
                        continue;
                    }
                }
                listaProyCompartirID.Add(proyID);
            }
            Dictionary<Guid, Guid> listaProyIDBRID = new Dictionary<Guid, Guid>();

            if (listaProyCompartirID.Count > 0)
            {
                DocumentacionCN documCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                listaProyIDBRID = documCN.ObtenerBasesRecursosIDporProyectosID(listaProyCompartirID);
                documCN.Dispose();
            }

            #endregion

            #region Cargamos listaOrgIDBRID y OrganizacionEsClase

            List<Guid> listaOrgCompartirID = new List<Guid>();
            foreach (Identidad identidad in listaIdentidadProyPERFILORGANIZACION.Values)
            {
                listaOrgCompartirID.Add((Guid)identidad.OrganizacionID);
            }
            Dictionary<Guid, Guid> listaOrgIDBRID = new Dictionary<Guid, Guid>();

            if (listaOrgCompartirID.Count > 0)
            {
                DocumentacionCN docum2CN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                listaOrgIDBRID = docum2CN.ObtenerBasesRecursosIDporOrganizacionesID(listaOrgCompartirID);
                docum2CN.Dispose();
            }

            #endregion

            //Agrego las BRs en proyectos con perfil personal:
            foreach (Identidad identidad in listaIdentidadProyPERFILPERSONAL.Values)
            {
                if (identidad.FilaIdentidad.ProyectoID != ProyectoAD.MetaProyecto && listaProyIDBRID.ContainsKey(identidad.FilaIdentidad.ProyectoID))
                {
                    Guid baseRecursosID = listaProyIDBRID[identidad.FilaIdentidad.ProyectoID];
                    if (!listaBasesDeRecursos.ContainsKey(baseRecursosID))
                    {
                        if (!listaBRYaCompartido.Contains(baseRecursosID))
                        {
                            AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto;
                            bool esBaseRecursosPublica = filaProy.TipoAcceso == (short)TipoAcceso.Publico || filaProy.TipoAcceso == (short)TipoAcceso.Restringido;

                            string tipoBaseRecursos = "private";
                            if (esBaseRecursosPublica) { tipoBaseRecursos = "public"; }

                            string nombreProy = filaProy.Nombre;
                            string nombre = UtilCadenas.ObtenerTextoDeIdioma(nombreProy, IdiomaUsuario, IdiomaPorDefecto) + " - " + UtilIdiomas.GetText("ANYADIRGNOSS", "PERSONAL");
                            KeyValuePair<string, string> datosBR = new KeyValuePair<string, string>(nombre, tipoBaseRecursos);
                            listaBasesDeRecursos.Add(baseRecursosID, datosBR);

                            string urlProyecto = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, filaProy.NombreCorto);
                            listaComunidadesUsuario.Add(urlProyecto, datosBR);
                        }
                    }
                }
            }

            //Agrego las BRs de los proyectos con perfil de profesor
            foreach (Identidad identidad in listaIdentidadProyPERFILPROFESOR.Values)
            {
                if (identidad.FilaIdentidad.ProyectoID != ProyectoAD.MetaProyecto && listaProyIDBRID.ContainsKey(identidad.FilaIdentidad.ProyectoID))
                {
                    Guid baseRecursosID = listaProyIDBRID[identidad.FilaIdentidad.ProyectoID];
                    if (!listaBasesDeRecursos.ContainsKey(baseRecursosID))
                    {
                        if (!listaBRYaCompartido.Contains(baseRecursosID))
                        {
                            AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto;
                            bool esBaseRecursosPublica = filaProy.TipoAcceso == (short)TipoAcceso.Publico || filaProy.TipoAcceso == (short)TipoAcceso.Restringido;

                            string tipoBaseRecursos = "private";
                            if (esBaseRecursosPublica) { tipoBaseRecursos = "public"; }

                            string nombreProy = UtilCadenas.ObtenerTextoDeIdioma(filaProy.Nombre, IdiomaUsuario, IdiomaPorDefecto);
                            string nombre = UtilCadenas.ObtenerTextoDeIdioma(nombreProy, IdiomaUsuario, IdiomaPorDefecto) + " - " + UtilIdiomas.GetText("ANYADIRGNOSS", "PROFESOR");
                            KeyValuePair<string, string> datosBR = new KeyValuePair<string, string>(nombre, tipoBaseRecursos);
                            listaBasesDeRecursos.Add(baseRecursosID, datosBR);

                            string urlProyecto = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, filaProy.NombreCorto);
                            listaComunidadesUsuario.Add(urlProyecto, datosBR);
                        }
                    }
                }
            }

            //Agrego las BRs de los proyectos en los que NO estoy con perfil PERSONAL
            foreach (List<SortedDictionary<string, Identidad>> listaDeListas in listaPerfilIdentProy.Values)
            {
                foreach (SortedDictionary<string, Identidad> listIdent in listaDeListas)
                {
                    foreach (Identidad identidad in listIdent.Values)
                    {
                        if (identidad.FilaIdentidad.ProyectoID != ProyectoAD.MetaProyecto && listaProyIDBRID.ContainsKey(identidad.FilaIdentidad.ProyectoID))
                        {
                            Guid baseRecursosID = listaProyIDBRID[identidad.FilaIdentidad.ProyectoID];
                            if (!listaBasesDeRecursos.ContainsKey(baseRecursosID))
                            {
                                if (!listaBRYaCompartido.Contains(baseRecursosID))
                                {
                                    AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto;
                                    bool esBaseRecursosPublica = filaProy.TipoAcceso == (short)TipoAcceso.Publico || filaProy.TipoAcceso == (short)TipoAcceso.Restringido;

                                    string tipoBaseRecursos = "private";
                                    if (esBaseRecursosPublica) { tipoBaseRecursos = "public"; }

                                    string nombreProy = UtilCadenas.ObtenerTextoDeIdioma(filaProy.Nombre, IdiomaUsuario, IdiomaPorDefecto);
                                    string nombre = UtilCadenas.ObtenerTextoDeIdioma(nombreProy, IdiomaUsuario, IdiomaPorDefecto) + " - " + listaIdentidadNombrePerfil[identidad];
                                    KeyValuePair<string, string> datosBR = new KeyValuePair<string, string>(nombre, tipoBaseRecursos);
                                    listaBasesDeRecursos.Add(baseRecursosID, datosBR);

                                    string urlProyecto = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, filaProy.NombreCorto);
                                    listaComunidadesUsuario.Add(urlProyecto, datosBR);
                                }
                            }
                        }
                    }
                }
            }

            //Agrego las BRs de organizaciones:
            foreach (Identidad identidad in listaIdentidadProyPERFILORGANIZACION.Values)
            {
                Guid baseRecursosID = listaOrgIDBRID[(Guid)identidad.OrganizacionID];
                if (!listaBasesDeRecursos.ContainsKey(baseRecursosID))
                {
                    if (!listaBRYaCompartido.Contains(baseRecursosID))
                    {
                        AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = gestorProyectos.ListaProyectos[identidad.FilaIdentidad.ProyectoID].FilaProyecto;

                        string nombre = UtilIdiomas.GetText("ANYADIRGNOSS", "BRORGANIZACION");
                        KeyValuePair<string, string> datosBR = new KeyValuePair<string, string>(nombre, "org");
                        listaBasesDeRecursos.Add(baseRecursosID, datosBR);

                        string urlProyecto = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, filaProy.NombreCorto);
                        listaComunidadesUsuario.Add(urlProyecto, datosBR);
                    }
                }
            }
        }

        #endregion
        private bool RecursoPuedeSerEditado(bool pBloqueadoPorOtroUsuario)
        {
            if (pBloqueadoPorOtroUsuario || !Documento.FilaDocumento.UltimaVersion || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki))
            {
                return false;
            }
            else if (!Documento.EsBorrador && Documento.TipoDocumentacion.Equals(TiposDocumentacion.Encuesta) && Documento.FilaDocumento.DocumentoRespuestaVoto.Count > 0)
            {
                return false;
            }
            else if (UsuarioActual.EsIdentidadInvitada)
            {
                return false;
            }
            else if (GenPlantillasOWL != null && GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEditarDoc)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void CargarAccionesPermitidasRecurso(ResourceModel pFichaRecurso)
        {
            ControladorDocumentoMVC.CargarIdentidadesLectoresEditores(Documento, GestorDocumental, ProyectoSeleccionado.Clave);

            paginaModel.PersonalSpace = UtilCadenas.ObtenerTextoDeIdioma(EspacioPersonal, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

            ActionsModel fichaAcciones = new ActionsModel();

            if (ComprobarSiDocumentoEstaSiendoMejorado())
            {
				FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
				bool tienePermisoEdicionEstado = flujosCN.ComprobarIdentidadTienePermisoEdicionEnEstado(pFichaRecurso.Estado.EstadoID, IdentidadActual.Clave, Documento.VersionOriginalID);
				fichaAcciones.AddCategories = false;
                fichaAcciones.AddTags = false;
                fichaAcciones.AddToMyPersonalSpace = true;
                fichaAcciones.BlockComments = false;
                fichaAcciones.Certify = false;
                fichaAcciones.CreateVersion = false;
                fichaAcciones.Delete = true;
                fichaAcciones.Duplicate = false;
                fichaAcciones.Edit = false;
                fichaAcciones.EditImprovement = tienePermisoEdicionEstado;
                fichaAcciones.LinkUp = false;
                fichaAcciones.Restore = false;
				fichaAcciones.SendLink = true;
				fichaAcciones.SendNewsletter = true;
                fichaAcciones.SendNewsletterGroups = true;                
                fichaAcciones.Share = true;
                fichaAcciones.Transition = true;
                fichaAcciones.UnLinkUp = false;
                fichaAcciones.ViewHistory = true;

				pFichaRecurso.Actions = fichaAcciones;

				return;
            }			

            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            bool permisoEditar = true;
            bool permisoEliminar = true;
            bool permisoRestaurarVersion = true;
            bool permisoEliminarVersion = true;
            bool permisoCertificar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.CertificarRecurso, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);

            switch (Documento.TipoDocumentacion)
            {
                case TiposDocumentacion.Hipervinculo:
                    permisoEditar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EditarRecursoTipoEnlace, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarRecursoTipoEnlace, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoRestaurarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionEnlace, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionEnlace, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Nota:
                    permisoEditar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EditarNota, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarNota, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoRestaurarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionNota, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionNota, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.FicheroServidor:
                    permisoEditar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EditarRecursoTipoAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarRecursoTipoAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoRestaurarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionAdjunto, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Encuesta:
                    permisoEditar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EditarEncuesta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarEncuesta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoRestaurarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionEncuesta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionEncuesta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.ReferenciaADoc:
                    permisoEditar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EditarRecursoTipoReferenciaADocumentoFisico, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarRecursoTipoReferenciaADocumentoFisico, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoRestaurarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionReferencia, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionReferencia, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Pregunta:
                    permisoEditar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EditarPregunta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarPregunta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoRestaurarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionPregunta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionPregunta, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Debate:
                    permisoEditar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EditarDebate, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminar = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarDebate, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoRestaurarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.RestaurarVersionDebate, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    permisoEliminarVersion = utilPermisos.IdentidadTienePermiso((ulong)PermisoRecursos.EliminarVersionDebate, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Recursos);
                    break;
                case TiposDocumentacion.Semantico:
                    permisoEditar = utilPermisos.IdentidadTienePermisoRecursoSemantico(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoPermisoRecursosSemanticos.Modificar, Documento.ElementoVinculadoID);
                    permisoEliminar = utilPermisos.IdentidadTienePermisoRecursoSemantico(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoPermisoRecursosSemanticos.Eliminar, Documento.ElementoVinculadoID);
                    permisoRestaurarVersion = utilPermisos.IdentidadTienePermisoRecursoSemantico(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoPermisoRecursosSemanticos.RestaurarVersion, Documento.ElementoVinculadoID);
                    permisoEliminarVersion = utilPermisos.IdentidadTienePermisoRecursoSemantico(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoPermisoRecursosSemanticos.EliminarVersion, Documento.ElementoVinculadoID);
                    break;
            }
            fichaAcciones.AddToMyPersonalSpace = true;
            fichaAcciones.Restore = !Documento.FilaDocumento.UltimaVersion && permisoRestaurarVersion;
            fichaAcciones.AddCategories = permisoEditar;
            fichaAcciones.AddTags = permisoEditar;
            fichaAcciones.Share = true;
            fichaAcciones.Duplicate = false;
            fichaAcciones.CreateVersion = true;
            fichaAcciones.ViewHistory = Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID) || permisoRestaurarVersion || permisoEliminarVersion;
            fichaAcciones.BlockComments = true;
            fichaAcciones.SendLink = true;
            fichaAcciones.SendNewsletter = true;
            fichaAcciones.SendNewsletterGroups = true;
            fichaAcciones.LinkUp = true;
            fichaAcciones.Certify = permisoCertificar;
            fichaAcciones.Delete = true;
            fichaAcciones.EditImprovement = false;

            #region Compartir a BR Personal o a otra comunidad

            if (Documento.EsBorrador || !Documento.CompartirPermitido || (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && !ParametrosGeneralesRow.CompartirRecursosPermitido) || (Documento.FilaDocumentoWebVinBR.PrivadoEditores && !ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)) || !Documento.FilaDocumento.UltimaVersion)
            {
                fichaAcciones.AddToMyPersonalSpace = false;
                fichaAcciones.Share = false;
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Debate || Documento.TipoDocumentacion == TiposDocumentacion.Pregunta || Documento.TipoDocumentacion == TiposDocumentacion.Encuesta || Documento.TipoDocumentacion == TiposDocumentacion.Newsletter)
            {
                fichaAcciones.AddToMyPersonalSpace = false;
                fichaAcciones.Share = false;
            }
            else
            {
                if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                {
                    if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
                    {
                        DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                        DocumentacionCN docCNAux = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                        docCNAux.ObtenerBaseRecursosUsuario(dataWrapperDocumentacion, mControladorBase.UsuarioActual.UsuarioID);
                        if (Documento.BaseRecursos.Contains(dataWrapperDocumentacion.ListaBaseRecursosUsuario.First().BaseRecursosID))
                        {
                            fichaAcciones.AddToMyPersonalSpace = false;
                        }
                        docCNAux.Dispose();
                    }
                }
                else
                {
                    fichaAcciones.AddToMyPersonalSpace = false;
                }
            }

            #endregion


            if (mControladorBase.UsuarioActual.EsIdentidadInvitada || mControladorBase.EstaUsuarioBloqueadoProyecto)
            {
                fichaAcciones.AddToMyPersonalSpace = false;
                fichaAcciones.AddCategories = false;
                fichaAcciones.AddTags = false;
                fichaAcciones.Duplicate = false;
                fichaAcciones.Share = false;
                fichaAcciones.Edit = false;
                fichaAcciones.CreateVersion = false;
                fichaAcciones.ViewHistory = false;
                fichaAcciones.BlockComments = false;
                fichaAcciones.SendNewsletter = false;
                fichaAcciones.SendNewsletterGroups = false;
                fichaAcciones.LinkUp = false;
                fichaAcciones.Certify = false;
                fichaAcciones.Delete = false;
                fichaAcciones.SendLink = false;
                fichaAcciones.Transition = false;
            }
            else
            {
                #region Restaurar Versión

                if (!(Documento.TipoDocumentacion != TiposDocumentacion.Wiki || (Documento.TipoDocumentacion == TiposDocumentacion.Wiki && !Documento.FilaDocumento.Protegido && EsIdentidadActualSupervisorProyecto)))
                {
                    fichaAcciones.Restore = false;
                }
                else if (!Documento.FilaDocumento.UltimaVersion && (Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)))
                {
                    fichaAcciones.Restore = true;
                    if (Documento.TipoDocumentacion == TiposDocumentacion.Wiki && Documento.UltimaVersion.FilaDocumento.Protegido)
                    {
                        fichaAcciones.Restore = false;
                    }
                }

                #endregion

                #region Agregar categorías y etiquetas

                bool bloqueadoPorOtroUsuario = Documento.FilaDocumento.IdentidadProteccionID.HasValue && !Documento.FilaDocumento.IdentidadProteccionID.Equals(mControladorBase.UsuarioActual.IdentidadID) && !IdentidadActual.GestorIdentidades.ListaIdentidades.ContainsKey(Documento.FilaDocumento.IdentidadProteccionID.Value);

                if (Documento.EsBorrador || !Documento.FilaDocumento.UltimaVersion || ProyectoOrigenBusquedaID != Guid.Empty)
                {
                    fichaAcciones.AddCategories = false;
                    fichaAcciones.AddTags = false;
                }else if (Documento.FilaDocumentoWebVinBR != null && Documento.FilaDocumentoWebVinBR.TipoPublicacion == (short)TipoPublicacion.CompartidoAutomatico)
                {
                    //Si es compartido automáticamente no se le pueden editar las categorías en esta comunidad.
                    fichaAcciones.AddCategories = false;
                }
                else
                {
                    if (!bloqueadoPorOtroUsuario && (Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)))
                    {
                        fichaAcciones.AddCategories = true;
                        fichaAcciones.AddTags = true;
                    }
                }  

                #endregion

                #region Editar
                fichaAcciones.Edit = permisoEditar && RecursoPuedeSerEditado(bloqueadoPorOtroUsuario);

                string urlEditar = "";

                if (Documento.TipoDocumentacion != TiposDocumentacion.Semantico)
                {
                    urlEditar = mControladorBase.UrlsSemanticas.GetURLBaseRecursosEditarDocumento(BaseURLIdioma, UtilIdiomas, NombreProyEdicionRecurso, UrlPerfil, Documento, 1, EsIdentidadBROrg);
                }
                else
                {
                    bool esDocVirtual = false;
                    if (Documento.GestorDocumental.ListaDocumentos.ContainsKey(Documento.ElementoVinculadoID))
                    {
                        Dictionary<string, string> listaPropiedades = UtilCadenas.ObtenerPropiedadesDeTexto(Documento.GestorDocumental.ListaDocumentos[Documento.ElementoVinculadoID].NombreEntidadVinculada);
                        if (listaPropiedades.ContainsKey(PropiedadesOntologia.urlservicio.ToString()))
                        {
                            esDocVirtual = true;
                        }
                    }
                    if (esDocVirtual)
                    {
                        urlEditar = mControladorBase.UrlsSemanticas.GetURLBaseRecursosVerDocumentoCreado(BaseURLIdioma, UtilIdiomas, NombreProyEdicionRecurso, UrlPerfil, Documento.Clave, Documento.FilaDocumento.ElementoVinculadoID.ToString(), true, EsIdentidadBROrg, esDocVirtual);
                    }
                    else
                    {
                        urlEditar = mControladorBase.UrlsSemanticas.GetURLBaseRecursosVersionarDocumentoCreado(BaseURLIdioma, UtilIdiomas, NombreProyEdicionRecurso, UrlPerfil, Documento.Clave, Documento.FilaDocumento.ElementoVinculadoID.ToString(), Guid.NewGuid(), EsIdentidadBROrg);
                    }
                }

                paginaModel.UrlEdit = urlEditar;

                paginaModel.UrlTransition = $"{mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false)}/transition";

                #endregion

                #region CrearVersion

                if (Documento.EsBorrador || !Documento.FilaDocumento.UltimaVersion || !Documento.PermiteVersiones || !mControladorBase.UsuarioActual.ProyectoID.Equals(Documento.ProyectoID) || (Documento.TipoDocumentacion == TiposDocumentacion.Wiki && Documento.FilaDocumento.Protegido && !EsIdentidadActualSupervisorProyecto) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Encuesta) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Pregunta) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Debate) || (!Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) && !ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID) && !permisoEditar))
                {
                    fichaAcciones.CreateVersion = false;
                }

                if (Documento.TipoDocumentacion != TiposDocumentacion.Semantico)
                {
                    paginaModel.UrlNewVersion = mControladorBase.UrlsSemanticas.GetURLBaseRecursosEditarDocumento(BaseURLIdioma, UtilIdiomas, NombreProyEdicionRecurso, UrlPerfil, Documento, 1, EsIdentidadBROrg);
                }

                #endregion

                #region Historial

                if (Documento.EsBorrador || !Documento.PermiteVersiones || Documento.Version == 0 || !Documento.FilaDocumento.UltimaVersion || (!Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) && !ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID)) && !permisoRestaurarVersion && !permisoEliminarVersion)
                {
                    fichaAcciones.ViewHistory = false;
                }

                paginaModel.UrlHistorial = mControladorBase.UrlsSemanticas.GetURLHistorialDocumento(BaseURLIdioma, UtilIdiomas, UrlPerfil, NombreProyEdicionRecurso, Documento.NombreSem(UtilIdiomas.LanguageCode), Documento.Clave, EsIdentidadBROrg);

                #endregion

                #region Bloquear Comentarios
                if (Documento.EsBorrador || !Documento.FilaDocumento.UltimaVersion || !Documento.TienePermisosIdentidadBloquearDesbloquearComentarios(IdentidadActual, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || ProyectoOrigenBusquedaID != Guid.Empty)
                {
                    fichaAcciones.BlockComments = false;
                }
                #endregion

                #region Enviar Newsletter

                if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && Documento.TipoDocumentacion == TiposDocumentacion.Newsletter && ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) && !Documento.EsBorrador)
                {
                    fichaAcciones.SendNewsletter = true;
                }
                else
                {
                    fichaAcciones.SendNewsletter = false;
                }
                #endregion

                #region Enviar Newsletter Grupos

                if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && Documento.TipoDocumentacion == TiposDocumentacion.Newsletter && EsIdentidadActualSupervisorProyecto && !Documento.EsBorrador)
                {
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGruposDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCN.Dispose();

                    if (gestorIdentidades.ListaGrupos.Count > 0)
                    {
                        fichaAcciones.SendNewsletterGroups = true;
                    }
                    else
                    {
                        fichaAcciones.SendNewsletterGroups = false;
                    }
                }
                else
                {
                    fichaAcciones.SendNewsletterGroups = false;
                }
                #endregion

                #region Vincular

                if (Documento.EsBorrador || !Documento.FilaDocumento.UltimaVersion || ProyectoOrigenBusquedaID != Guid.Empty)
                {
                    fichaAcciones.LinkUp = false;
                }
                #endregion

                #region Certificar

                if (!Documento.FilaDocumento.UltimaVersion)
                {
                    fichaAcciones.Certify = false;
                }
                #endregion

                #region Eliminar

                if (!(Documento.FilaDocumento.UltimaVersion && (!Documento.FilaDocumento.Protegido || !bloqueadoPorOtroUsuario || EsIdentidadActualSupervisorProyecto) && (!Documento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki) || EsIdentidadActualSupervisorProyecto) && (Documento.TienePermisosEdicionIdentidad(IdentidadActual, IdentidadOrganizacionBROrg, ProyectoSeleccionado, UsuarioActual.UsuarioID, EsIdentidadActualAdministradorOrganizacion) || Documento.TienePermisosIdentidadEliminarRecursoEnBR(IdentidadActual, Documento.GestorDocumental.BaseRecursosIDActual, ProyectoSeleccionado, UsuarioActual.UsuarioID, UsuarioActual.ProyectoID, EsIdentidadActualAdministradorOrganizacion, EsIdentidadActualSupervisorProyecto) || ControladorDocumentacion.EsEditorPerfilDeDocumento(IdentidadActual.PerfilID, Documento, true, UsuarioActual.UsuarioID) || permisoEliminar)))
                {
                    fichaAcciones.Delete = false;
                }
                #endregion

                #region Duplicar

                // Si tiene permisos para duplicar y se puede compartir el recurso y no es borrador y no es semántico
                if (Documento.FilaDocumento.UltimaVersion && Documento.TipoDocumentacion != TiposDocumentacion.Semantico && !Documento.EsBorrador && Documento.CompartirPermitido && ParametroProyecto.ContainsKey(ParametroAD.DuplicarRecursosDisponible) && bool.Parse(ParametroProyecto[ParametroAD.DuplicarRecursosDisponible]))
                {
                    fichaAcciones.Duplicate = true;
                }

                #endregion
            }

            if (GenPlantillasOWL != null)
            {
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonRestaurarVersionDoc)
                {
                    fichaAcciones.Restore = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonAgregarCategoriaDoc)
                {
                    fichaAcciones.AddCategories = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonAgregarEtiquetasDoc)
                {
                    fichaAcciones.AddTags = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEditarDoc)
                {
                    fichaAcciones.Edit = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonCrearVersionDoc)
                {
                    fichaAcciones.CreateVersion = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonHistorialDoc)
                {
                    fichaAcciones.ViewHistory = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonBloquearComentariosDoc)
                {
                    fichaAcciones.BlockComments = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEnviarEnlaceDoc)
                {
                    fichaAcciones.SendLink = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonVincularDoc)
                {
                    fichaAcciones.LinkUp = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonCertificarDoc)
                {
                    fichaAcciones.Certify = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarCompartirEspecioPersonal)
                {
                    fichaAcciones.AddToMyPersonalSpace = false;
                }
                if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDoc || (!string.IsNullOrEmpty(GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDocCondicion) && GenPlantillasOWL.CumpleRecursoCondicion(GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarBotonEliminarDocCondicion, UsuarioActual.IdentidadID)))
                {
                    fichaAcciones.Delete = false;
                }
            }

            pFichaRecurso.AllowComments = ParametrosGeneralesRow.ComentariosDisponibles;
            if (Documento.FilaDocumentoWebVinBR != null)
            {
                pFichaRecurso.AllowComments = (Documento.FilaDocumentoWebVinBR.PermiteComentarios && ParametrosGeneralesRow.ComentariosDisponibles);
            }

            #region Flujos

            if (pFichaRecurso.Estado != null)
            {
                FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
                bool tienePermisoEdicionEstado = flujosCN.ComprobarIdentidadTienePermisoEdicionEnEstado(pFichaRecurso.Estado.EstadoID, IdentidadActual.Clave, Documento.VersionOriginalID);
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                bool puedeSerEditado = !docCN.ComprobarDocumentoTieneVersiones(Documento.VersionOriginalID) || Documento.FilaDocumento.VersionDocumento.EstadoVersion == (short) EstadoVersion.Vigente || Documento.FilaDocumento.VersionDocumento.EstadoVersion == (short)EstadoVersion.Pendiente;
                docCN.Dispose();
                if (tienePermisoEdicionEstado && puedeSerEditado)
                {
                    fichaAcciones.Edit = true;
                    fichaAcciones.AddCategories = true;
                    fichaAcciones.AddTags = true;
                }
                else
                {
                    fichaAcciones.Edit = false;
                    fichaAcciones.AddCategories = false;
                    fichaAcciones.AddTags = false;
                    fichaAcciones.BlockComments = true;
                    pFichaRecurso.AllowComments = false;
                    fichaAcciones.Delete = false;
                    fichaAcciones.AddToMyPersonalSpace = false;
                    fichaAcciones.Share = false;
                    fichaAcciones.Duplicate = false;
                    fichaAcciones.ViewHistory = true;
                }

                fichaAcciones.EditImprovement = fichaAcciones.Edit;

				bool esMejora = false;
				using (DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory))
				{
					esMejora = documentacionCN.ComprobarSiDocumentoEsUnaMejora(DocumentoVersionID);
				}
				if (esMejora)
				{
                    fichaAcciones.BlockComments = true;
                    pFichaRecurso.AllowComments = false;
                    fichaAcciones.Delete = false;
                    fichaAcciones.AddToMyPersonalSpace = false;
                    fichaAcciones.Share = false;                    
                    fichaAcciones.Duplicate = false;
                    fichaAcciones.ViewHistory = true;
				}

                fichaAcciones.Transition = true;
            }

            #endregion

            pFichaRecurso.Actions = fichaAcciones;
        }

        private ActionResult ActionCallback()
        {
            string[] action = RequestParams("callback").Split('|');

            if (action[0].ToLower() == "CargarGadgets".ToLower())
            {
                //Seguridad : Dentro
                return Index_Gadgets();
            }
            else if (action[0].ToLower() == "paginadorgadget")
            {
                //Seguridad : Dentro
                Guid gadgetID = new Guid(RequestParams("gadgetid"));
                int numPagina = int.Parse(RequestParams("numPagina"));

                GadgetController gadgetController = new GadgetController(this, mHttpContextAccessor, mLoggingService, mGnossCache, mConfigService, mVirtuosoAD, mEntityContext, mRedisCacheWrapper, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mEnv, mAvailableServices, mLoggerFactory.CreateLogger<GadgetController>(), mLoggerFactory);
                return gadgetController.CargarGadgetRecurso(gadgetID, numPagina, Documento, GenPlantillasOWL, PestanyaRecurso.Value);
            }
            else if (!mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                if (action[0].ToLower() == "compartidos")
                {
                    if (action[1].ToLower() == "CargarCompartidos".ToLower())
                    {
                        if (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarCompartidoDoc)
                        {
                            ResourceModel FichaRecurso = new ResourceModel();
                            FichaRecurso.Key = Documento.Clave;
                            FichaRecurso.Title = Documento.Nombre;

                            FichaRecurso.Shareds = CargarBRCompartidas();

                            FichaRecurso.CompletCardLink = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false);
                            ControladorProyectoMVC.EstablecerUrlAccionesReecurso(FichaRecurso, ProyectoSeleccionado.NombreCorto);

                            return PartialView("_FichaCompartidos", FichaRecurso);
                        }
                    }
                }
                else if (action[0].ToLower() == "encuesta" && Documento.TipoDocumentacion == TiposDocumentacion.Encuesta)
                {
                    //Seguridad : No le dejamos votar dos veces
                    ResourceModel FichaRecurso = new ResourceModel();
                    FichaRecurso.Key = Documento.Clave;
                    FichaRecurso.Title = Documento.Nombre;

                    if (FichaRecurso.Poll == null)
                    {
                        FichaRecurso.Poll = new PollModel();
                    }

                    bool verResultadosEncuesta = false;
                    if (GestorDocumental.DataWrapperDocumentacion != null)
                    {
                        bool yaVotado = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRespuestaVoto.Any(item => item.IdentidadID.Equals(IdentidadActual.Clave));

                        if (action[1].ToLower() == "votar" && action[2] != null)
                        {
                            Guid respuestaID = new Guid(action[2]);

                            if (respuestaID != Guid.Empty && !yaVotado)
                            {
                                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                                docCL.InvalidarFichaRecursoMVC(Documento.Clave, ProyectoVirtual.Clave);
                                docCL.Dispose();

                                GestorDocumental.AgregarVotoARespuestaDeRecurso(Documento.Clave, respuestaID, IdentidadActual.Clave);
                                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                                docCN.ActualizarDocumentacion();
                                docCN.Dispose();

                                ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("Votos", Documento.Clave, ProyectoVirtual.Clave, IdentidadActual.Clave, Documento.CreadorID);

                                yaVotado = true;
                            }
                        }
                        else if (action[1].ToLower() == "verresultados")
                        {
                            verResultadosEncuesta = true;
                        }

                        FichaRecurso.Poll.PollOptions = new List<PollModel.PollOptionsModel>();

                        foreach (RespuestaRecurso opcion in Documento.ListaRespuestas.Values)
                        {
                            PollModel.PollOptionsModel opcionEncuesta = new PollModel.PollOptionsModel();
                            opcionEncuesta.Key = opcion.FilaRespuesta.RespuestaID;
                            opcionEncuesta.Name = opcion.FilaRespuesta.Descripcion;
                            opcionEncuesta.NumberOfVotes = opcion.FilaRespuesta.NumVotos;
                            FichaRecurso.Poll.PollOptions.Add(opcionEncuesta);
                        }

                        FichaRecurso.Poll.Voted = yaVotado;

                        FichaRecurso.Poll.ViewPollResults = verResultadosEncuesta || yaVotado;

                        return PartialView("_FichaEncuesta", FichaRecurso);
                    }
                }
            }

            return new EmptyResult();
        }

        private ActionResult Index_Gadgets()
        {
            MultiViewResult result = new MultiViewResult(this, mViewEngine);

            if (RequestParams("gadgetsID") != null)
            {
                List<Guid> listaGadgetsSinCargar = new List<Guid>();
                List<GadgetModel> listaGadgets = new List<GadgetModel>();

                try
                {
                    string[] gadgets = RequestParams("gadgetsID").Split(',');
                    foreach (string gadget in gadgets)
                    {
                        if (gadget != "")
                        {
                            listaGadgetsSinCargar.Add(new Guid(gadget));
                        }
                        if (listaGadgetsSinCargar.Count >= 3)
                        {
                            break;
                        }
                    }

                    GadgetController gadgetController = new GadgetController(this, mHttpContextAccessor, mLoggingService, mGnossCache, mConfigService, mVirtuosoAD, mEntityContext, mRedisCacheWrapper, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mEnv, mAvailableServices, mLoggerFactory.CreateLogger<GadgetController>(), mLoggerFactory);
                    listaGadgets = gadgetController.CargarListaGadgetsRecurso(false, listaGadgetsSinCargar, Documento, GenPlantillasOWL, PestanyaRecurso.Value, false);
                }
                catch
                { }
                finally
                {
                    foreach (Guid gadgetID in listaGadgetsSinCargar)
                    {
                        GadgetModel gadgetModel = listaGadgets.Find(gadget => gadget.Key == gadgetID);

                        if (gadgetModel != null)
                        {
                            if (gadgetModel is GadgetResourceListModel)
                            {
                                if (((GadgetResourceListModel)gadgetModel).Resources != null && ((GadgetResourceListModel)gadgetModel).Resources.Count > 0)
                                {
                                    result.AddView("../Shared/ControlesMVC/_FichaGadget", $"FichaGadget_{gadgetID}", gadgetModel);
                                }
                                else
                                {
                                    result.AddContent("", $"FichaGadget_{gadgetID}");
                                }
                            }
                            else if (gadgetModel is GadgetCMSModel)
                            {
                                if (((GadgetCMSModel)gadgetModel).CMSComponent != null)
                                {
                                    result.AddView("../Shared/ControlesMVC/_FichaGadget", $"FichaGadget_{gadgetID}", gadgetModel);
                                }
                                else
                                {
                                    result.AddContent("", $"FichaGadget_{gadgetID}");
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        [TypeFilter(typeof(AccesoRecursoAttribute))]
        public ActionResult Index()
        {
            try
            {
                if (!string.IsNullOrEmpty(RequestParams("callback")))
                {
                    return ActionCallback();
                }

                if (!ListaEnlacesMultiIdioma.Values.Any(par => par.Key.Equals(true)))
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
                else if (!ListaEnlacesMultiIdioma[UtilIdiomas.LanguageCode].Key)
                {
                    AsignarMetaRobots("NOINDEX, FOLLOW");
                    return DevolverVista("../Shared/IndexNoIdioma", ((HeaderModel)(ViewBag.Cabecera)).MultiLingualLinks);
                }

                if (Documento == null || Documento.FilaDocumento.Eliminado || Documento.FilaDocumento.Borrador)
                {
                    // Si la identidad actual o la identidad personal no coinciden con la identidad de creacción del recurso, redirigir a la página no encontrada.
                    if (!(IdentidadActual.Clave == Documento.FilaDocumento.CreadorID || (IdentidadActual.IdentidadPersonalMyGNOSS != null && IdentidadActual.IdentidadPersonalMyGNOSS.Clave == Documento.FilaDocumento.CreadorID)))
                    {
                        return RedireccionarAPaginaNoEncontrada();
                    }
                }

                if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Ontologia) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.OntologiaSecundaria))
                {
                    return Redirect(Comunidad.Url);
                }

                if (EsPeticionRDF)
                {
                    return CargarRDF();
                }

                string nombreSem = PestanyaRecurso.Value;
                string nombreProy = "";
                KeyValuePair<Guid?, bool> baseRecursosPersonaID = new KeyValuePair<Guid?, bool>(null, false);

                if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                {
                    nombreProy = ProyectoSeleccionado.NombreCorto;
                }
                else
                {
                    baseRecursosPersonaID = ObtenerBaseRecursosPersonalRecurso();
                }

                string baseUrlBusqueda = $"{mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, nombreProy, UrlPerfil, EsIdentidadBROrg, nombreSem)}/";
				AD.EntityModel.Models.Documentacion.VersionDocumento versionDocumento = mGestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.FirstOrDefault(item => item.DocumentoID.Equals(Documento.Clave));
				List<Guid> listaRecursosID = new List<Guid>();
                listaRecursosID.Add(Documento.Clave);
                Dictionary<Guid, ResourceModel> diccRecursos = ControladorProyectoMVC.ObtenerRecursosPorIDSinProcesarIdioma(listaRecursosID, baseUrlBusqueda, baseRecursosPersonaID, Documento.FilaDocumento.ProyectoID, pEsFichaRecurso: true, pEsMejora: versionDocumento.EsMejora);
                if (!diccRecursos.ContainsKey(Documento.Clave))
                {
					return Redirect(Comunidad.Url);
				}
				paginaModel.Resource = diccRecursos[Documento.Clave];
                bool recursoObtenidoDeCache = true;

                paginaModel.Resource.LastVersion = Documento.UltimaVersionID;
                paginaModel.Resource.Description = ModificarVideoSiNoHayCookiesYoutube(paginaModel.Resource.Description);

                if (!paginaModel.Resource.FullyLoaded)
                {
                    recursoObtenidoDeCache = false;
                    paginaModel.Resource.FullyLoaded = true;
                    paginaModel.Resource.Version = Documento.Version;

                    paginaModel.Resource.NumDownloads = Documento.FilaDocumento.NumeroTotalDescargas;

                    if (Documento.FilaDocumentoWebVinBR.NivelCertificacionID.HasValue)
                    {
                        ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                        ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion.Union(proyectoCL.ObtenerNivelesCertificacionRecursosProyecto(ProyectoSeleccionado.Clave).ListaNivelCertificacion).ToList();
                        paginaModel.Resource.Certification = new KeyValuePair<Guid, string>(Documento.FilaDocumentoWebVinBR.NivelCertificacionID.Value, ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion.FirstOrDefault(nivelCert => nivelCert.NivelCertificacionID.Equals(Documento.FilaDocumentoWebVinBR.NivelCertificacionID)).Descripcion);
                    }
                    CargarAutoresEditoresLectores(paginaModel.Resource);

                    cargarEnlaceDescargaRecurso(paginaModel.Resource);

                    if (ParametrosAplicacionDS.Any(item => item.Parametro.Equals(TiposParametrosAplicacion.LecturaAumentada) && item.Valor.Equals("1")))
                    {
                        AD.EntityModel.Models.Documentacion.DocumentoLecturaAumentada lecturaAumentadaDocumento = mEntityContext.DocumentoLecturaAumentada.FirstOrDefault(item => item.DocumentoID.Equals(Documento.Clave));
                        if (lecturaAumentadaDocumento != null && lecturaAumentadaDocumento.Validada)
                        {
                            paginaModel.Resource.AugmentedTitle = lecturaAumentadaDocumento.TituloAumentado;
                            paginaModel.Resource.AugmentedDescription = lecturaAumentadaDocumento.DescripcionAumentada;
                        }
                    }

                    DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    docCL.AgregarFichaRecursoMVC(Documento.Clave, ProyectoVirtual.Clave, paginaModel.Resource);
                    docCL.Dispose();
                }

                //Permitir externos leer el fichero y descargarlo
                Guid token = Guid.Empty;
                if (ParametroProyecto.ContainsKey(ParametroAD.LecturaFichero) && ParametroProyecto[ParametroAD.LecturaFichero].Equals("1") && paginaModel.Resource.TypeDocument == DocumentType.FicheroServidor && (paginaModel.Resource.NameImage == "xls" || paginaModel.Resource.NameImage == "documento" || paginaModel.Resource.NameImage == "presentacion" || paginaModel.Resource.NameImage == "pdf"))
                {
                    SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SolicitudCN>(), mLoggerFactory);
                    token = Guid.NewGuid();
                    solicitudCN.GuardarTokenAccesoAPILogin(token, "Documento");
                    paginaModel.Resource.UrlDocument += $"&token={token.ToString()}";
                }

                foreach (CategoryModel categoria in paginaModel.Resource.Categories)
                {
                    TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);
                    GestionTesauro gestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);

                    if (gestorTesauro.ListaCategoriasTesauro.ContainsKey(categoria.Key))
                    {
                        CategoriaTesauro catTesauro = gestorTesauro.ListaCategoriasTesauro[categoria.Key];
                        categoria.Name = UtilCadenas.ObtenerTextoDeIdioma(catTesauro.FilaCategoria.Nombre, UtilIdiomas.LanguageCode, "es");
                    }
                    categoria.Lang = UtilIdiomas.LanguageCode;
                }

                if (!string.IsNullOrEmpty(paginaModel.Resource.RdfType))
                {
                    string rdfTypeName = "";
                    List<OntologiaProyecto> filasOntologias = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaOntologiaProyecto.Where(onto => onto.OntologiaProyecto1.Equals(paginaModel.Resource.RdfType)).ToList();
                    if (filasOntologias.Count > 0)
                    {
                        rdfTypeName = UtilCadenas.ObtenerTextoDeIdioma(filasOntologias.First().NombreOnt, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    }

                    paginaModel.Resource.RdfTypeName = rdfTypeName;
                }
                else
                {
                    paginaModel.Resource.RdfTypeName = paginaModel.Resource.NameImage;

                    switch (paginaModel.Resource.TypeDocument)
                    {
                        case DocumentType.Audio:
                        case DocumentType.AudioBrightcove:
                        case DocumentType.AudioTOP:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCAUDIO");
                            break;
                        case DocumentType.Blog:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCBLOG");
                            break;
                        case DocumentType.DafoProyecto:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDAFO");
                            break;
                        case DocumentType.Debate:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDEBATE");
                            break;
                        case DocumentType.Encuesta:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCENCUESTA");
                            break;
                        case DocumentType.EntradaBlog:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCENTRADABLOG");
                            break;
                        case DocumentType.EntradaBlogTemporal:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCENTRADABLOGTEMP");
                            break;
                        case DocumentType.FicheroServidor:
                            switch (paginaModel.Resource.NameImage)
                            {
                                case "video":
                                    paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCVIDEO");
                                    break;
                                case "audio":
                                    paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCAUDIO");
                                    break;
                                case "documento":
                                    paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCTEXTO");
                                    break;
                                case "presentacion":
                                    paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCPRESENTACION");
                                    break;
                                case "xls":
                                    paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCEXCEL");
                                    break;
                                case "pdf":
                                    paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCPDF");
                                    break;
                                case "zip":
                                    paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCARCHIVOCOMPRIMIDO");
                                    break;
                            }
                            break;
                        case DocumentType.Hipervinculo:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCHIPERVINCULO");
                            ParametroAplicacionCN parametroCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                            string oneDrive = parametroCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                            paginaModel.Resource.EsEnlaceSharepoint = UtilCadenas.EsEnlaceSharepoint(Documento.Enlace, oneDrive);
                            string sharepointConfigurado = parametroCN.ObtenerParametroAplicacion("SharepointClientID");
                            if (string.IsNullOrEmpty(sharepointConfigurado))
                            {
                                paginaModel.Resource.EsEnlaceSharepoint = false;
                            }
                            else
                            {
                                string[] enlaceAux = Documento.Enlace.Split("|||");
                                if (enlaceAux.Length > 1)
                                {
                                    paginaModel.Resource.UrlDocument = Documento.Enlace.Split("|||")[0];
                                }
                                else
                                {
                                    paginaModel.Resource.UrlDocument = Documento.Enlace;
                                }
                            }
                            break;
                        case DocumentType.Imagen:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCIMAGEN");
                            break;
                        case DocumentType.ImagenWiki:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCIMGWIKI");
                            break;
                        case DocumentType.Newsletter:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCNEWSLETTER");
                            break;
                        case DocumentType.Nota:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCNOTA");
                            break;
                        case DocumentType.Pregunta:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCPREGUNTA");
                            break;
                        case DocumentType.Video:
                        case DocumentType.VideoBrightcove:
                        case DocumentType.VideoTOP:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCVIDEO");
                            break;
                        case DocumentType.Wiki:
                        case DocumentType.WikiTemporal:
                            paginaModel.Resource.RdfTypeName = UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCWIKI");
                            break;
                    }
                }

                //Está fuera de la cache porque serializa HTML y da error
                paginaModel.Resource.Licence = ObtenerLicencia();

                if (recursoObtenidoDeCache)
                {
                    // Si no se ha obtenido de caché, esta carga ya se ha hecho
                    CargarAutoresEditoresLectores(paginaModel.Resource);
                }

                paginaModel.Resource.Shareds = CargarBRCompartidas();

                string url = CargarUrlVistaPrevia(Documento);

                if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo && !Documento.EsVideoIncrustado && !Documento.EsPresentacionIncrustada)
                {
                    if (!Documento.FilaDocumento.VersionFotoDocumento.HasValue || (Documento.FilaDocumento.VersionFotoDocumento.HasValue && Documento.FilaDocumento.VersionFotoDocumento.Value < 0) || (Documento.FilaDocumento.VersionFotoDocumento.HasValue && Documento.FilaDocumento.VersionFotoDocumento.Value > 1000))
                    {
                        paginaModel.Resource.UrlPreview = "";
                    }
                }
                else if (!string.IsNullOrEmpty(url))
                {
                    paginaModel.Resource.UrlPreview = url;
                }

                #region Modificamos propiedades para la presentacion (despues de guardar en BBDD)
                if (GenPlantillasOWL != null)
                {
                    if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarEtiquetasDoc)
                    {
                        if (paginaModel.Resource.Tags != null)
                        {
                            paginaModel.Resource.Tags.Clear();
                        }
                    }
                    if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarCategoriasDoc)
                    {
                        if (paginaModel.Resource.Categories != null)
                        {
                            paginaModel.Resource.Categories.Clear();
                        }
                    }
                    if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarCompartidoEnDoc)
                    {
                        paginaModel.Resource.SocialNetworks = null;
                    }
                    if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarLicenciaDoc)
                    {
                        paginaModel.Resource.Licence = null;
                    }
                    if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarVersionDoc)
                    {
                        paginaModel.Resource.Version = 0;
                    }
                    if (GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarPublicadorDoc)
                    {
                        paginaModel.Resource.Publisher = null;
                    }
                }
                if (ProyectoSeleccionado.EsCatalogo && !ParametrosGeneralesRow.MostrarPersonasEnCatalogo)
                {
                    paginaModel.Resource.Publisher = null;
                }
                if (GenPlantillasOWL != null && GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarUtilsDoc)
                {
                    paginaModel.HideUtilsResource = true;
                }
                if (ParametrosGeneralesRow.VotacionesDisponibles && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarVotosDoc))
                {
                    paginaModel.Resource.AllowVotes = true;
                }
                if (GenPlantillasOWL != null && GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarVisitasDoc)
                {
                    paginaModel.HideWebsiteVisits = true;
                }
                if (GenPlantillasOWL != null && GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarCompartidoDoc)
                {
                    paginaModel.HideSharedCommunities = true;
                }
                if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Encuesta))
                {
                    if (paginaModel.Resource.Poll == null)
                    {
                        paginaModel.Resource.Poll = new PollModel();
                    }

                    paginaModel.Resource.Poll.PollOptions = new List<PollModel.PollOptionsModel>();

                    foreach (RespuestaRecurso opcion in Documento.ListaRespuestas.Values)
                    {
                        PollModel.PollOptionsModel opcionEncuesta = new PollModel.PollOptionsModel();
                        opcionEncuesta.Key = opcion.FilaRespuesta.RespuestaID;
                        opcionEncuesta.Name = opcion.FilaRespuesta.Descripcion;
                        opcionEncuesta.NumberOfVotes = opcion.FilaRespuesta.NumVotos;
                        paginaModel.Resource.Poll.PollOptions.Add(opcionEncuesta);
                    }
                }

                #endregion

                paginaModel.Resource.AllowComments = ParametrosGeneralesRow.ComentariosDisponibles;

                if (Documento.FilaDocumentoWebVinBR != null)
                {
                    paginaModel.Resource.AllowComments = (Documento.FilaDocumentoWebVinBR.PermiteComentarios && ParametrosGeneralesRow.ComentariosDisponibles);
                }

                if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Encuesta))
                {
                    if (paginaModel.Resource.Poll == null)
                    {
                        paginaModel.Resource.Poll = new PollModel();
                    }
                    if (GestorDocumental.DataWrapperDocumentacion != null)
                    {
                        bool yaVotado = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRespuestaVoto.Any(item => item.IdentidadID.Equals(IdentidadActual.Clave));

                        paginaModel.Resource.Poll.Voted = yaVotado;

                        paginaModel.Resource.Poll.ViewPollResults = yaVotado;
                    }
                }

				paginaModel.Resource.Title = UtilCadenas.ObtenerTextoDeIdioma(paginaModel.Resource.Title, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
				paginaModel.Resource.Description = UtilCadenas.ObtenerTextoDeIdioma(paginaModel.Resource.Description, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

				string rdfType = Documento.TipoDocumentacion.ToString();
                if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    rdfType = Path.GetFileNameWithoutExtension(GestorDocumental.ListaDocumentos[Documento.ElementoVinculadoID].Enlace);
                    
                    paginaModel.Resource.Description = ModificarVideoSiNoHayCookiesYoutube(paginaModel.Resource.Description);

                    if (Documento.GestorDocumental.ListaDocumentos.ContainsKey(paginaModel.Resource.ItemLinked) && Documento.GestorDocumental.ListaDocumentos[paginaModel.Resource.ItemLinked].FilaDocumento.VersionFotoDocumento.HasValue && Documento.GestorDocumental.ListaDocumentos[paginaModel.Resource.ItemLinked].FilaDocumento.VersionFotoDocumento > 0)
                    {
                        paginaModel.Resource.ItemLinkedFotoVersion = Documento.GestorDocumental.ListaDocumentos[paginaModel.Resource.ItemLinked].FilaDocumento.VersionFotoDocumento.Value;
                    }
                }

                paginaModel.Resource.SocialNetworks = CargarListaRedesSociales(paginaModel.Resource);
                paginaModel.Resource.Comments = CargarListaComentarios();

                int numRecursosRelaccionados = 0;
                paginaModel.Resource.RelatedResources = CargarListaVinculados(1, out numRecursosRelaccionados);
                paginaModel.Resource.NumRelatedResources = numRecursosRelaccionados;

                paginaModel.Resource.Graphs = CargarGrafosRecurso();
                paginaModel.UrlIntragnoss = UrlIntragnoss;

                paginaModel.Resource.AIGeneratedTranslation = ComprobarSiDocumentoEstaTraducidoConIAEnIdiomaActual();

                try
                {
                    GadgetController gadgetController = new GadgetController(this, IdentidadActual, mHttpContextAccessor, mLoggingService, mGnossCache, mConfigService, mVirtuosoAD, mEntityContext, mRedisCacheWrapper, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GadgetController>(), mLoggerFactory);
                    paginaModel.Resource.Gadgets = gadgetController.CargarListaGadgetsRecurso(true, null, Documento, GenPlantillasOWL, PestanyaRecurso.Value);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex.ToString());
                }

                ViewBag.NombreUrlPestanya = PestanyaRecurso.Value;
                ViewBag.NombrePestanya = PestanyaRecurso.Key;
                ViewBag.RecursoEspacioPersonal = Documento.ProyectoID.Equals(ProyectoAD.MetaProyecto);

                MarcarPestanyaActiva(Comunidad.Tabs, ViewBag.NombreUrlPestanya);

                CambiarIndexacionDelRecurso();

                paginaModel.Resource.Estado = ObtenerEstadoDocumento();
                
                if (versionDocumento != null)
                {
                    paginaModel.Resource.IsImprovement = versionDocumento.EsMejora;
                    paginaModel.Resource.VersionState = (EstadoVersion)versionDocumento.EstadoVersion;
                }


				paginaModel.Resource.IsInProcessOfImprovement = ComprobarSiDocumentoEstaSiendoMejorado();				

                if (paginaModel.Resource.Estado != null)
                {
                    UtilFlujos utilFlujos = new UtilFlujos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilFlujos>(), mLoggerFactory);
                    paginaModel.Resource.HistorialTransiciones = utilFlujos.ObtenerHistorialDeTransiciones(Documento.Clave, TipoContenidoFlujo.Recurso, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }

                if (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarAccionesDoc)
                {
                    CargarAccionesPermitidasRecurso(paginaModel.Resource);
                }

                if (ParametrosGeneralesRow.VotacionesDisponibles && (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarVotosDoc))
                {
                    paginaModel.Resource.Votes = CargarFichaVotos(false);
                }

                string tituloPaginaSemantica = "";
                if (GenPlantillasOWL != null && GenPlantillasOWL.SemanticResourceModel != null && !string.IsNullOrEmpty(GenPlantillasOWL.SemanticResourceModel.PageTitle))
                {
                    tituloPaginaSemantica = System.Text.RegularExpressions.Regex.Replace(HttpUtility.UrlDecode(GenPlantillasOWL.SemanticResourceModel.PageTitle), "<.*?>", string.Empty);
                }

                AgregarMetasFichaRecurso(Documento, tituloPaginaSemantica);
                ViewBag.URLRDF = ViewBag.UrlPagina + "?rdf";

                //Si se trata de un video brightcove no se actualiza aqui, sino que se actualizará al reproducir el video
                if (Documento.TipoDocumentacion != TiposDocumentacion.VideoBrightcove)
                {
                    ActualizarConsultaRecurso();
                }
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                //Si es recurso tipo enlace de sharepoint
                if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo && UtilCadenas.EsEnlaceSharepoint(Documento.Enlace, oneDrivePermitido))
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    string sharepointConfigurado = paramCN.ObtenerParametroAplicacion("SharepointClientID");
                    if (!string.IsNullOrEmpty(sharepointConfigurado))
                    {
                        ActionResult redirect = FuncionalidadSharepoint();
                        if (redirect != null)
                        {
                            return redirect;
                        }
                        SharepointController spController = new SharepointController(Documento.Clave.ToString(), mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth, _appLifetime, mAvailableServices, mLoggerFactory.CreateLogger<SharepointController>(), mLoggerFactory);
                        spController.Token = tokenSP;
                        spController.TokenCreadorRecurso = tokenSPAdminDocument;
                        bool estaAlineadoConSharepoint = spController.ComprobarSiEstaAlineadoConSharepoint(Documento.Enlace);
                        paginaModel.Resource.EstaAlineadoConSharepoint = estaAlineadoConSharepoint;
                    }
                }

                paginaModel.AllowPalco = false;
                if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo)
                {
                    string dominioRecurso = new Uri(paginaModel.Resource.UrlDocument).Host.Replace("www.", "");
                    paginaModel.AllowPalco = !ListaDominiosSinPalco.Contains(dominioRecurso);
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    paginaModel.AllowPalco = true;
                }

                CargarJavascriptExtra();

                ViewBag.FormularioRegistro = CargarFormularioRegistro(mAvailableServices);

                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("recurso_id", DocumentoID.ToString()));
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("recurso_version_id", Documento.Clave.ToString()));
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("txtRecurso", UtilIdiomas.GetText("URLSEM", "RECURSO")));
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("desvincularSharepointMetodo", UtilIdiomas.GetText("URLSEM", "DESVINCULARSHAREPOINT")));
                if (Documento.FilaDocumento.FechaModificacion.HasValue)
                {
                    ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("recurso_modification_date", Documento.FilaDocumento.FechaModificacion.Value.ToString("yyyy/MM/dd HH:mm")));
                }

                return ViewFichaRecurso(rdfType, paginaModel);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Genera la lectura aumentada para un recurso
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateAugmentedReading()
        {
            try
            {
                AnnotationResult annotation = new UtilSherlock(new UtilWeb(mHttpContextAccessor)).AnnotationSherlock(Documento.Titulo, Documento.Descripcion, 0.7f, pAugmentedText: true, pPreserveHTML: true, pTopics: true);

                AD.EntityModel.Models.Documentacion.DocumentoLecturaAumentada lecturaAumentada = mEntityContext.DocumentoLecturaAumentada.FirstOrDefault(item => item.DocumentoID.Equals(Documento.Clave));
                bool borrarCache = false;
                if (lecturaAumentada != null)
                {
                    lecturaAumentada.Validada = false;
                    borrarCache = true;
                }
                else
                {
                    lecturaAumentada = new AD.EntityModel.Models.Documentacion.DocumentoLecturaAumentada()
                    {
                        DocumentoID = Documento.Clave,
                        Validada = false
                    };
                    mEntityContext.DocumentoLecturaAumentada.Add(lecturaAumentada);
                }
                lecturaAumentada.TituloAumentado = annotation.augmentedTextTitle;
                lecturaAumentada.DescripcionAumentada = annotation.augmentedTextDescription;
                lecturaAumentada.EntitiesInfo = annotation.entitiesInfo;
                lecturaAumentada.TopicsInfo = annotation.topicsInfo;

                mEntityContext.SaveChanges();

                if (borrarCache)
                {
                    DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    docCL.InvalidarFichaRecursoMVC(Documento.Clave, ProyectoSeleccionado.Clave);
                }

                var result = new { AugmentedTitle = annotation.augmentedTextTitle, AugmentedDescription = annotation.augmentedTextDescription };
                return Json(result);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Genera la lectura aumentada para un recurso
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoRecursoAttribute), Arguments = new object[] { RolesAccesoRecurso.Editor })]
        public ActionResult SaveAugmentedReading(string AugmentedText, string AugmentedDescription)
        {
            try
            {
                AD.EntityModel.Models.Documentacion.DocumentoLecturaAumentada lecturaAumentada = mEntityContext.DocumentoLecturaAumentada.FirstOrDefault(item => item.DocumentoID.Equals(Documento.Clave));
                if (lecturaAumentada == null)
                {
                    lecturaAumentada = new AD.EntityModel.Models.Documentacion.DocumentoLecturaAumentada()
                    {
                        DocumentoID = Documento.Clave,
                        Validada = true
                    };
                    mEntityContext.DocumentoLecturaAumentada.Add(lecturaAumentada);
                }
                else
                {
                    lecturaAumentada.Validada = true;
                }

                if (!string.IsNullOrEmpty(AugmentedText))
                {
                    lecturaAumentada.TituloAumentado = AugmentedText;
                }
                else
                {
                    lecturaAumentada.TituloAumentado = null;
                }
                if (!string.IsNullOrEmpty(AugmentedDescription))
                {
                    if (!string.IsNullOrEmpty(lecturaAumentada.TopicsInfo) && !string.IsNullOrEmpty(lecturaAumentada.EntitiesInfo))
                    {
                        List<SherlockEntityInfoModel> litaTopicsSherlock = JsonConvert.DeserializeObject<List<SherlockEntityInfoModel>>(lecturaAumentada.TopicsInfo);
                        List<SherlockEntityInfoModel> litaEntitySherlock = JsonConvert.DeserializeObject<List<SherlockEntityInfoModel>>(lecturaAumentada.EntitiesInfo);
                        foreach (SherlockEntityInfoModel topic in litaTopicsSherlock.ToList())
                        {
                            if (!AugmentedDescription.Contains(topic.WikipediaUri))
                            {
                                litaTopicsSherlock.Remove(topic);
                            }
                        }
                        foreach (SherlockEntityInfoModel entity in litaEntitySherlock.ToList())
                        {
                            if (!AugmentedDescription.Contains(entity.WikipediaUri))
                            {
                                litaEntitySherlock.Remove(entity);
                            }
                        }
                        lecturaAumentada.EntitiesInfo = JsonConvert.SerializeObject(litaEntitySherlock);
                        lecturaAumentada.TopicsInfo = JsonConvert.SerializeObject(litaTopicsSherlock);
                    }
                    lecturaAumentada.DescripcionAumentada = AugmentedDescription;
                }
                else
                {
                    lecturaAumentada.DescripcionAumentada = null;
                }

                //Insertar Rabbit ColaTagsComunidadesLInkedData
                ControladorDocumentacion controladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                controladorDocumentacion.InsertLinkedDataRabbit(Documento.ProyectoID, Documento.TipoDocumentacion.ToString(), lecturaAumentada, mAvailableServices);

                mEntityContext.SaveChanges();

                DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                docCL.InvalidarFichaRecursoMVC(Documento.Clave, ProyectoSeleccionado.Clave);

                return Content("OK");
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                return new StatusCodeResult(500);
            }
        }

        private ActionResult FuncionalidadSharepoint(bool pDescargando = false, string pEnlace = "")
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
                if (!string.IsNullOrEmpty(pEnlace))
                {
                    enlace = pEnlace;
                }
                if (string.IsNullOrEmpty(enlace))
                {
                    if (Session.Get("EnlaceDocumentoAgregar") != null)
                    {
                        enlace = Session.GetString("EnlaceDocumentoAgregar");
                    }
                }
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                string oneDrivePermitido = parametroAplicacionCN.ObtenerParametroAplicacion(ParametroAD.PermitirEnlazarDocumentosOneDrive);
                if (!string.IsNullOrEmpty(enlace) && UtilCadenas.EsEnlaceSharepoint(enlace, oneDrivePermitido))
                {
                    string urlRedirect = $"{urlServicioLogin}/LoginSharepoint?urlInicio={RequestUrl}&usuario={UsuarioActual.UsuarioID}";

                    // Guardamos la url de inicio en una cookie
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(1);
                    mHttpContextAccessor.HttpContext.Response.Cookies.Append("urlInicio", RequestUrl, cookieOptions);

                    // Comprobamos en BD si existe un token para el usuario
                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);


                    Guid usuarioID = UsuarioActual.UsuarioID;
                    Guid usuarioIDCreador = identidadCN.ObtenerUsuarioIDConIdentidadID(Documento.FilaDocumento.CreadorID);
                    // Si el usuario es solo lector, comprobar con el token del creador.
                    if ((paginaModel.Resource != null && !paginaModel.Resource.Actions.Edit) || pDescargando)
                    {
                        usuarioID = identidadCN.ObtenerUsuarioIDConIdentidadID(Documento.FilaDocumento.CreadorID);
                        urlRedirect = $"{urlServicioLogin}/LoginSharepoint?urlInicio={RequestUrl}&usuario={usuarioIDCreador}";
                    }
                    string tokenUsuario = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.Sharepoint, usuarioID);
                    string refreshToken = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.SharepointRefresh, usuarioID);
                    // Generar y comprobar token para usuario normal.
                    if (!string.IsNullOrEmpty(tokenUsuario))
                    {
                        // Si existe en la bd comprobamos si el token sigue siendo valido
                        bool tokenEsValido = sharepointController.ComprobarValidezToken(tokenUsuario);
                        if (!tokenEsValido)
                        {
                            // Si existe pero no es valido solicitamos uno nuevo llamando al servicio de login
                            //return Redirect(urlRedirect);
                            tokenUsuario = sharepointController.RenovarToken(refreshToken, usuarioID);
                            if (string.IsNullOrEmpty(tokenUsuario))
                            {
                                return Redirect(urlRedirect);
                            }
                            sharepointController.Token = tokenUsuario;
                            tokenSP = tokenUsuario;
                        }
                        else
                        {
                            // Si existe lo guardamos para usarlo y que nos permita descargar
                            sharepointController.Token = tokenUsuario;
                            tokenSP = tokenUsuario;
                        }
                    }
                    else
                    {
                        // Si no existe generamos uno nuevo llamando al servicio de login
                        return Redirect(urlRedirect);
                    }

                    // Generar y comprobar token para usuario Creador del recurso.
                    if (!usuarioID.Equals(usuarioIDCreador))
                    {
                        tokenUsuario = usuarioCN.ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin.Sharepoint, usuarioID);

                        // Generar y comprobar token para usuario normal.
                        if (!string.IsNullOrEmpty(tokenUsuario))
                        {
                            // Si existe en la bd comprobamos si el token sigue siendo valido
                            bool tokenEsValido = sharepointController.ComprobarValidezToken(tokenUsuario);
                            if (!tokenEsValido)
                            {
                                // Si existe pero no es valido solicitamos uno nuevo llamando al servicio de login
                                //return Redirect(urlRedirect);
                                tokenUsuario = sharepointController.RenovarToken(refreshToken, usuarioID);
                                if (string.IsNullOrEmpty(tokenUsuario))
                                {
                                    return Redirect(urlRedirect);
                                }
                                sharepointController.Token = tokenUsuario;
                                tokenSPAdminDocument = tokenUsuario;
                                tokenSP = tokenUsuario;
                            }
                            else
                            {
                                // Si existe lo guardamos para usarlo y que nos permita descargar
                                sharepointController.Token = tokenUsuario;
                                tokenSPAdminDocument = tokenUsuario;
                                tokenSP = tokenUsuario;
                            }
                        }
                        else
                        {
                            // Si no existe generamos uno nuevo llamando al servicio de login
                            return Redirect(urlRedirect);
                        }
                    }
                }
                return null;
            }
            return null;
        }

        private void CargarJavascriptExtra()
        {
            //TODO Juan: Mirar este error -> ERROR: Method not found: '!!0[] System.Array.Empty()'.
            try
            {
                //Si no la contiene o si la contiene y es igual a 1, entonces llamar al servicio de etiquetado automático.
                if (!ParametroProyecto.ContainsKey("PintarEnlacesLODEtiquetasEnProyecto") || (ParametroProyecto.ContainsKey("PintarEnlacesLODEtiquetasEnProyecto") && ParametroProyecto["PintarEnlacesLODEtiquetasEnProyecto"].Equals("1")))
                {
                    string baseEnlaceTag = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, false, PestanyaRecurso.Value);
                    if (PestanyaRecurso.Key == UtilIdiomas.GetClaveText("URLSEM", "BUSQUEDAAVANZADA"))
                    {
                        baseEnlaceTag = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    }

                    StringBuilder sbTraerComp = new StringBuilder();
                    sbTraerComp.AppendLine("");
                    if (Documento.FilaDocumento.Tags == null)
                    {
                        Documento.FilaDocumento.Tags = "";
                    }
                    sbTraerComp.AppendLine($"ObtenerEntidadesLOD('{mConfigService.ObtenerUrlServicioEtiquetas()}/EtiquetadoLOD/', '{baseEnlaceTag}', '{Documento.Clave.ToString()}', '{Documento.FilaDocumento.Tags.Replace("'", "\\'")}', '{UtilIdiomas.LanguageCode}')");
                    sbTraerComp.AppendLine("");

                    ViewBag.JSExtra += sbTraerComp.ToString();
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }
        }

        #region RDF

        /// <summary>
        /// Carga el RDF del documento
        /// </summary>
        public ActionResult CargarRDF()
        {
            try
            {
                //base.CargarRDF(pPestanyaActual);
                CargarGeneradorURLs();

                if (RequestParams("docID") != null)
                {
                    List<string> tipoElementoExportar = new List<string>();
                    tipoElementoExportar.Add(TipoElementoGnoss.Documento);
                    tipoElementoExportar.Add(TipoElementoGnoss.Debate);
                    tipoElementoExportar.Add(TipoElementoGnoss.Pregunta);
                    tipoElementoExportar.Add(TipoElementoGnoss.ComentarioSioc);
                    tipoElementoExportar.Add(TipoElementoGnoss.CategoriasTesauroSkos);
                    tipoElementoExportar.Add(TipoElementoGnoss.TagSioc);
                    tipoElementoExportar.Add(TipoElementoGnoss.ComunidadSioc);
                    tipoElementoExportar.Add(TipoElementoGnoss.PerfilPersonaFoaf);
                    tipoElementoExportar.Add(TipoElementoGnoss.PerfilOrganizacionFoaf);

                    CargarComentarios();

                    ExportadorWiki exportadorWiki = new ExportadorWiki(OntologiaGnoss, IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, new Metagnoss.ExportarImportar.UtilSemCms(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UtilSemCms>(), mLoggerFactory), mServicesUtilVirtuosoAndReplication, mVirtuosoAD, mLoggerFactory.CreateLogger<ExportadorWiki>(), mLoggerFactory);
                    exportadorWiki.IdentidadActual = IdentidadActual;
                    exportadorWiki.ProyectoSeleccionado = ProyectoSeleccionado;
                    string tipo = TipoElementoGnoss.Documento;

                    if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Debate))
                    {
                        tipo = TipoElementoGnoss.Debate;
                    }
                    else if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Pregunta))
                    {
                        tipo = TipoElementoGnoss.Pregunta;
                    }
                    else if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Semantico))
                    {
                        if (!Documento.GestorDocumental.ListaDocumentos.ContainsKey(Documento.ElementoVinculadoID))
                        {
                            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                            Documento.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerDocumentoPorID(Documento.ElementoVinculadoID));
                            docCN.Dispose();

                            Documento.GestorDocumental.CargarDocumentos(false);
                        }

                        string nombreOntologia = GestorDocumental.ListaDocumentos[Documento.ElementoVinculadoID].FilaDocumento.Enlace;
                        exportadorWiki.URL_ONTOLOGIA_DOCSEM = BaseURLFormulariosSem + "/Ontologia/" + nombreOntologia + "#";
                        Dictionary<string, List<EstiloPlantilla>> listaEstilos;
                        byte[] arrayOnto = null;

                        Guid xmlID = ControladorDocumentacion.ObtenerIDXmlOntologia(Documento.ElementoVinculadoID, ProyectoSeleccionado.Clave, null);
                        Dictionary<string, string> listaPropiedades = UtilCadenas.ObtenerPropiedadesDeTexto(Documento.GestorDocumental.ListaDocumentos[Documento.ElementoVinculadoID].FilaDocumento.NombreElementoVinculado);

                        if (listaPropiedades.ContainsKey(PropiedadesOntologia.xmlTroceado.ToString()) && listaPropiedades[PropiedadesOntologia.xmlTroceado.ToString()].ToLower() == "true")
                        {
                            bool obtenidoVirtuoso = false;
                            string rdfTexto = ObtenerSemCmsController(Documento).ObtenerRDFDocumento(Documento.Clave, Documento.FilaDocumento.ProyectoID.Value, GestionOWL.NAMESPACE_ONTO_GNOSS, exportadorWiki.URL_ONTOLOGIA_DOCSEM, nombreOntologia, null, UrlIntragnoss, false, Documento.FilaDocumento.UltimaVersion, out obtenidoVirtuoso);
                            string urlEntPrincipal = SemCmsController.ObtenerTipoEntidadPrincipalRDF(rdfTexto);
                            string claveOnto = Documento.ElementoVinculadoID + urlEntPrincipal;
                            exportadorWiki.CLAVE_ONTO_TROCEADA = claveOnto;

                            if (!EstiloPlantilla.OntologiasTrozosCargadas.ContainsKey(claveOnto) || EstiloPlantilla.OntologiasTrozosCargadas[claveOnto].Key != xmlID)
                            {
                                arrayOnto = ControladorDocumentacion.ObtenerOntologiaFraccionada(Documento.ElementoVinculadoID, nombreOntologia, urlEntPrincipal, out listaEstilos, ProyectoSeleccionado.Clave, null);
                            }
                            else
                            {
                                ControladorDocumentacion.CargarEstilosOntologiaFraccionadoSegunEntidad(Documento.ElementoVinculadoID, nombreOntologia, nombreOntologia.Replace(".", "") + ":" + urlEntPrincipal, out listaEstilos, ProyectoSeleccionado.Clave, null);
                            }
                        }
                        else
                        {
                            if (!EstiloPlantilla.OntologiasCargadas.ContainsKey(Documento.ElementoVinculadoID) || EstiloPlantilla.OntologiasCargadas[Documento.ElementoVinculadoID].Key != xmlID)
                            {
                                arrayOnto = ControladorDocumentacion.ObtenerOntologia(Documento.ElementoVinculadoID, out listaEstilos, ProyectoSeleccionado.Clave);
                            }
                            else
                            {
                                ControladorDocumentacion.CargarEstilosOntologia(Documento.ElementoVinculadoID, out listaEstilos, ProyectoSeleccionado.Clave);
                            }
                        }

                        exportadorWiki.ARRAY_ONTOLOGIA_DOCSEM = arrayOnto;
                        exportadorWiki.NOMBRE_ONTOLOGIA_DOCSEM = nombreOntologia;
                        exportadorWiki.URLINTRAGNOSS = UrlIntragnoss;
                        exportadorWiki.CONFIG_XML_DOCSEM = listaEstilos;
                        mRDFSearch.mUrlOntologiaPlantillaRDF = exportadorWiki.URL_ONTOLOGIA_DOCSEM;
                        exportadorWiki.XML_ID = xmlID;

                        EstiloPlantillaConfigGen estiloPlantillaConfigGen = (EstiloPlantillaConfigGen)listaEstilos["[" + LectorXmlConfig.NodoConfigGen + "]"][0];

                        if (estiloPlantillaConfigGen.Namespace != null)
                        {
                            exportadorWiki.NAMESPACE_DOCSEM = ((EstiloPlantillaConfigGen)listaEstilos["[" + LectorXmlConfig.NodoConfigGen + "]"][0]).Namespace;
                            mRDFSearch.mNamespacePlantillaRDF = exportadorWiki.NAMESPACE_DOCSEM;
                        }

                        if (EsBot && estiloPlantillaConfigGen.IndexRobots != null && estiloPlantillaConfigGen.IndexRobots.ToUpper().Replace(" ", "").Equals("NOINDEX,NOFOLLOW"))
                        {
                            if (this.Request.QueryString.ToString().ToLower().EndsWith("?rdf"))
                            {
                                return Redirect(UrlPagina);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }

                    #region Recursos relacionados y contextos

                    try
                    {
                        ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                        DataWrapperProyecto dataWrapperProyecto = proyectoCL.ObtenerGadgetsProyecto(ProyectoSeleccionado.Clave);
                        proyectoCL.Dispose();

                        if (dataWrapperProyecto.ListaProyectoGadget.Count > 0)
                        {
                            #region Rec relacionados

                            //string recursosPagActual = null;
                            if (dataWrapperProyecto.ListaProyectoGadget.Where(proy => proy.TipoUbicacion.Equals((short)(TipoUbicacionGadget.FichaRecursoComunidad)) && proy.Visible == true && proy.Tipo.Equals((short)TipoGadget.RecursosRelacionados)).ToList().Count > 0)
                            {
                                //int resultpag;
                                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);

                                List<Guid> listaIDs = (List<Guid>)docCL.ObtenerRelacionadosRecursoMVC(DocumentoID, ProyectoSeleccionado.Clave);

                                if (listaIDs == null)
                                {
                                    listaIDs = new List<Guid>();

                                    int tablaProyectoId = ProyectoSeleccionado.FilaProyecto.TablaBaseProyectoID;

                                    string nombreTablaCOMUNIDADES = "COMUNIDAD_000000000000000".Substring(0, 25 - tablaProyectoId.ToString().Length) + tablaProyectoId.ToString();

                                    FacetadoDS facetadoDS = new FacetadoDS();

                                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, ProyectoSeleccionado.Clave.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                                    facetadoCN.InformacionOntologias = InformacionOntologias;

                                    string tags = "";
                                    if (Documento.ListaTagsSoloLectura.Count > 0)
                                    {
                                        foreach (string tag in Documento.ListaTagsSoloLectura)
                                        {
                                            tags += "\"" + tag.Replace("\"", "'").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ") + "\",";
                                        }
                                        tags = tags.Substring(0, tags.Length - 1);
                                    }

                                    string categorias = "";
                                    if (Documento.Categorias.Count > 0)
                                    {
                                        foreach (Guid categoriaID in Documento.Categorias.Keys)
                                        {
                                            categorias += "gnoss:" + categoriaID.ToString().ToUpper() + ",";
                                        }
                                        categorias = categorias.Substring(0, categorias.Length - 1);
                                    }

                                    facetadoCN.ObtenerRecursosRelacionadosNuevo(ProyectoSeleccionado.Clave.ToString(), Documento.Clave.ToString(), facetadoDS, 0, ParametrosGeneralesRow.NumeroRecursosRelacionados * 2, tags, categorias, ProyectoSeleccionado.TipoProyecto == TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso, PestanyaRecurso.Value);

                                    if (facetadoDS.Tables["RecurosRelacionados"].Rows.Count > 0)
                                    {
                                        foreach (DataRow myrow in facetadoDS.Tables["RecurosRelacionados"].Rows)
                                        {
                                            string recursoRelacionado = (string)myrow[0];
                                            recursoRelacionado = recursoRelacionado.Substring(recursoRelacionado.IndexOf("gnoss") + 6);

                                            listaIDs.Add(new Guid(recursoRelacionado));
                                        }
                                    }
                                    docCL.AgregarRelacionadosRecursoMVC(Documento.Clave, ProyectoSeleccionado.Clave, listaIDs);

                                }

                                docCL.Dispose();

                                string baseUrlBusqueda = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");

                                Dictionary<Guid, ResourceModel> listaFichasRecursos = ControladorProyectoMVC.ObtenerRecursosPorID(listaIDs, baseUrlBusqueda, null, false);

                                foreach (ResourceModel recursoRelacionado in listaFichasRecursos.Values)
                                {
                                    Documento.UrlRecursosRelacionadosRDF.Add(recursoRelacionado.CompletCardLink);
                                }
                            }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        GuardarLogError(ex.ToString());
                    }

                    #endregion

                    ElementoOntologia documento = new ElementoOntologiaGnoss(OntologiaGnoss.GetEntidadTipo(tipo));
                    UtilImportarExportar.ObtenerID(documento, Documento.FilaDocumento, Documento);
                    exportadorWiki.ObtenerEntidad(documento, Documento, true, Documento.GestorDocumental);
                    mRDFSearch.mUrlIntragnoss = UrlIntragnoss;
                    return File(mRDFSearch.MontarRDF(documento, tipoElementoExportar, OntologiaGnoss), "application/rdf+xml");
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                throw;
            }
        }

        #endregion

        #region Metas

        public void AgregarMetasFichaRecurso(DocumentoWeb pDocumento, string pTituloPaginaConfiguradoXMLOntologia)
        {
            if (ViewBag.ListaMetas == null)
            {
                ViewBag.ListaMetas = new List<ViewMetaData>();
            }

            //Nuevo MetaDescripcion metatitulo
            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            AD.EntityModel.Models.Documentacion.DocumentoMetaDatos docMetas = docCL.ObtenerMetaDatos(pDocumento.FilaDocumento.DocumentoID);
            docCL.Dispose();
            if (docMetas != null)
            {
                if (string.IsNullOrEmpty(docMetas.MetaTitulo))
                {
                    AgregarMetaTitulo(pTituloPaginaConfiguradoXMLOntologia);
                }
                else
                {
                    AgregarMetaTitulo(docMetas.MetaTitulo);
                    ViewBag.TituloPagina = docMetas.MetaTitulo;
                }
                if (string.IsNullOrEmpty(docMetas.MetaDescripcion))
                {
                    AgregarMetaDescripcion(pDocumento.Descripcion);
                }
                else
                {
                    AgregarMetaDescripcion(docMetas.MetaDescripcion);
                }
            }
            else
            {
                AgregarMetaDescripcion(pDocumento.Descripcion);
                AgregarMetaTitulo(pTituloPaginaConfiguradoXMLOntologia);
            }

            AgregarMetaUrl(pDocumento);
            AgregarMetaImagen(pDocumento);
            AgregarMetaSite(ProyectoSeleccionado.Nombre);

            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "twitter:card", ContentAttributeValue = "summary" });
            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "twitter:domain", ContentAttributeValue = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto) });

            AgregarMetasFormularioSemantico();
        }

        private void AgregarMetasFormularioSemantico()
        {
            if (ViewBag.ListaMetasComplejas == null)
            {
                ViewBag.ListaMetasComplejas = new List<Dictionary<string, string>>();
            }

            if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && GenPlantillasOWL != null)
            {
                Dictionary<string, List<Dictionary<string, string>>> metaEtiquetasHTMLOntologia = mGenPlantillasOWL.Ontologia.ConfiguracionPlantilla.MetasHTMLOntologia;

                if (metaEtiquetasHTMLOntologia != null && metaEtiquetasHTMLOntologia.Count > 0)
                {
                    //cargamos las metaetiquetas genéricas
                    if (metaEtiquetasHTMLOntologia.ContainsKey(string.Empty))
                    {
                        ViewBag.ListaMetasComplejas.AddRange(metaEtiquetasHTMLOntologia[string.Empty]);
                    }

                    //cargamos las metaetiquetas del idioma de navegación del usuario
                    if (metaEtiquetasHTMLOntologia.ContainsKey(UtilIdiomas.LanguageCode))
                    {
                        ViewBag.ListaMetasComplejas.AddRange(metaEtiquetasHTMLOntologia[UtilIdiomas.LanguageCode]);
                    }
                    else
                    {
                        //cargamos las metaetiquetas del idioma por defecto de la comunidad
                        if (!(ParametrosGeneralesRow.IdiomaDefecto == null) && metaEtiquetasHTMLOntologia.ContainsKey(ParametrosGeneralesRow.IdiomaDefecto))
                        {
                            ViewBag.ListaMetasComplejas.AddRange(metaEtiquetasHTMLOntologia[ParametrosGeneralesRow.IdiomaDefecto]);
                        }
                    }
                }
            }
        }

        private void AgregarMetaSite
            (string pNombreProyecto)
        {
            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "property", AttributeValue = "og:site_name", ContentAttributeValue = pNombreProyecto });

            if (ProyectoVirtual.FilaProyecto.TieneTwitter)
            {
                string usuarioTwitter = ProyectoVirtual.FilaProyecto.UsuarioTwitter;
                if (!usuarioTwitter.StartsWith("@"))
                {
                    usuarioTwitter = $"@{usuarioTwitter}";
                }

                ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "twitter:site", ContentAttributeValue = usuarioTwitter });
            }
        }

        private void AgregarMetaImagen(DocumentoWeb pDocumento)
        {
            string urlImagen = "";
            if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico)
            {
                string imagen = ControladorDocumentacion.ObtenerNombreImagenPrincipalRecSem(pDocumento, BaseURLContent);

                if (imagen != null)
                {
                    urlImagen = imagen;
                }
            }
            else if (pDocumento.FilaDocumento.VersionFotoDocumento.HasValue && Documento.FilaDocumento.VersionFotoDocumento > 0)
            {
                string extension = ".jpg";
                if (pDocumento.NombreEntidadVinculada == "Wiki2" && pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Imagen) && pDocumento.Enlace.Contains('.'))
                {
                    extension = pDocumento.Enlace.Substring(pDocumento.Enlace.LastIndexOf('.'));
                }

                string fileName = System.Net.WebUtility.UrlEncode(pDocumento.Clave.ToString().ToLower()) + extension;
                urlImagen = $"{BaseURLContent}/{UtilArchivos.ContentImagenesEnlaces}/{UtilArchivos.DirectorioDocumento(pDocumento.Clave)}/{fileName}?{pDocumento.FilaDocumento.VersionFotoDocumento}";
            }

            if (string.IsNullOrEmpty(urlImagen))
            {
                bool escomunidadEducativa = ProyectoVirtual.TipoProyecto == TipoProyecto.Universidad20 || ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionExpandida || ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionPrimaria;

                if (!escomunidadEducativa && !string.IsNullOrEmpty(ParametrosGeneralesVirtualRow.CoordenadasSup))
                {
                    urlImagen = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesProyectos}/{ProyectoVirtual.Clave.ToString().ToLower()}.png";
                    if (!(ParametrosGeneralesVirtualRow.VersionFotoImagenSupGrande == null))
                    {
                        urlImagen += $"?v={ParametrosGeneralesVirtualRow.VersionFotoImagenSupGrande.ToString()}";
                    }
                }
            }

            if (!string.IsNullOrEmpty(urlImagen))
            {
                ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "property", AttributeValue = "og:image", ContentAttributeValue = urlImagen });
                ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "twitter:image", ContentAttributeValue = urlImagen });
            }
        }

        private void AgregarMetaUrl(DocumentoWeb pDocumento)
        {
            string urlFicha = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "/", pDocumento, false);

            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "property", AttributeValue = "og:url", ContentAttributeValue = urlFicha });
            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "twitter:url", ContentAttributeValue = urlFicha });
        }

        private void AgregarMetaTitulo(string pTituloPaginaConfiguradoXMLOntologia)
        {
            string tituloPagina = pTituloPaginaConfiguradoXMLOntologia;

            if (string.IsNullOrEmpty(tituloPagina))
            {
                tituloPagina = UtilCadenas.ObtenerTextoDeIdioma(mDocumento.Titulo, IdiomaUsuario, IdiomaPorDefecto);
            }

            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "property", AttributeValue = "og:title", ContentAttributeValue = tituloPagina });
            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "twitter:title", ContentAttributeValue = tituloPagina });
        }

        /// <summary>
        /// Agrega el meta de descripción en el idioma en el que está navegando el usuario. Si la descripción no tiene ese idioma agrega el castellano
        /// </summary>
        /// <param name="pDescripcion">Descripción</param>
        public void AgregarMetaDescripcion(string pDescripcion)
        {
            string descripcion = UtilCadenas.HtmlDecode(UtilCadenas.EliminarHtmlDeTexto(pDescripcion)).Trim();

            descripcion = UtilCadenas.ObtenerTextoDeIdioma(descripcion, UtilIdiomas.LanguageCode, "es");

            // Si hay saltos de línea, los quito para el meta description. 
            descripcion = descripcion.Replace("\r\n", " ").Replace("\n", " ");

            if (descripcion.Length > 200)
            {
                // 1- Buscar el primer punto a partir de carácter nº 200. Si no contiene punto, devolver "..."                               
                int positionNextPoint = descripcion.Substring(200, descripcion.Length - 200).IndexOf(".");
                // 2- Asignar a descripción la nueva hasta el punto encontrado siempre que haya alguno en los siguientes 200 caractéres.
                if (positionNextPoint > 1 && positionNextPoint < 200)
                {
                    descripcion = descripcion.Substring(0, positionNextPoint + 201);
                }
                else
                {
                    descripcion = descripcion.Substring(0, 200) + "...";
                }
            }

            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "description", ContentAttributeValue = descripcion });
            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "property", AttributeValue = "og:description", ContentAttributeValue = descripcion });
            ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "twitter:description", ContentAttributeValue = descripcion });
        }

        #endregion

        /// <summary>
        /// Actualiza el nº de consultas al documento actual.
        /// </summary>
        private void ActualizarConsultaRecurso()
        {
            if (Session.Get("DocumentoVisto") == null)
            {
                Session.Set("DocumentoVisto", new List<Guid>());
            }

            if ((!(Session.Get<List<Guid>>("DocumentoVisto")).Contains(DocumentoID)) && (!EsBot))
            {
                try
                {
                    List<Guid> documentosVistos = Session.Get<List<Guid>>("DocumentoVisto");
                    documentosVistos.Add(DocumentoID);
                    Session.Set("DocumentoVisto", documentosVistos);

                    //Actualización Offline a partir de un servicio UDP
                    //Llamada asincrona para actualizar la popularidad del recurso:
                    ControladorDocumentacion.LlamadaUDP_ServicioSocketsOffline("Visitas", Documento.Clave, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.IdentidadID, Documento.CreadorID);

                    if (!mControladorBase.UsuarioActual.EsIdentidadInvitada)
                    {
                        // Actualizo el contador del usuario
                        // TODO: Mover al servicio de sockets offline
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        identidadCN.ActualizarContadorIdentidad(IdentidadActual.Clave, IdentidadAD.CONTADOR_NUMERO_VISITAS);
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                }
            }
        }

        /// <summary>
        /// Maca la pestaña activa del menú
        /// </summary>
        /// <param name="pListaPestanyas"></param>
        /// <param name="pRuta"></param>
        /// <returns></returns>
        private CommunityModel.TabModel MarcarPestanyaActiva(List<CommunityModel.TabModel> pListaPestanyas, string pRuta)
        {
            CommunityModel.TabModel pestanyaActiva = null;
            pestanyaActiva = pListaPestanyas.Find(pestanya => pestanya.Url != null && pestanya.Url.EndsWith(pRuta));
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

        private void CambiarIndexacionDelRecurso()
        {
            if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && !string.IsNullOrEmpty(GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.IndexRobots))
            {
                AsignarMetaRobots(mGenPlantillasOWL.Ontologia.ConfiguracionPlantilla.IndexRobots);
            }
            else if (Documento.FilaDocumentoWebVinBR != null && !Documento.FilaDocumentoWebVinBR.IndexarRecurso)
            {
                AsignarMetaRobots("NOINDEX, FOLLOW");
            }
        }

        private bool ComprobarSiDocumentoEstaTraducidoConIAEnIdiomaActual()
        {
            using (DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory))
			{
                return docCN.ComprobarSiDocumentoEstaTraducidoConIAEnIdioma(Documento.Clave, UtilIdiomas.LanguageCode);
            }                
        }

        private List<GrafoRecurso> CargarGrafosRecurso()
        {
            List<GrafoRecurso> listaGrafosRecurso = new List<GrafoRecurso>();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            DataWrapperProyecto proyGrafosDWP = proyCL.ObtenerGrafosProyecto(ProyectoSeleccionado.Clave);

            if (proyGrafosDWP.ListaProyectoGrafoFichaRec.Count > 0)
            {
                string version = "1.0.0.0";
                if (!string.IsNullOrEmpty(Version))
                {
                    version = Version;
                }
                version = $"?v={version.Replace('.', '_')}";

                string urlBusqueda = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto)}/";
                string urlBusquedaDbpedia = null;
                string uridbpedia = ParametrosAplicacionDS.FirstOrDefault(item => item.Parametro.Equals(TiposParametrosAplicacion.URIGrafoDbpedia))?.Valor;
                Guid idGrafoDbpedia;
                if (!string.IsNullOrEmpty(uridbpedia) && Guid.TryParse(uridbpedia, out idGrafoDbpedia))
                {
                    string nombreCortoComunidadGrafoDbpedia = proyCL.ObtenerNombreCortoProyecto(idGrafoDbpedia);
                    urlBusquedaDbpedia = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, nombreCortoComunidadGrafoDbpedia)}/";
                }

                string tipoRecurso = FacetadoAD.BUSQUEDA_RECURSOS;

                if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    tipoRecurso = Documento.GestorDocumental.ListaDocumentos[Documento.ElementoVinculadoID].Enlace;
                    tipoRecurso = tipoRecurso.Substring(0, tipoRecurso.LastIndexOf("."));
                }

                foreach (ProyectoGrafoFichaRec filaProyGrafo in proyGrafosDWP.ListaProyectoGrafoFichaRec)
                {
                    string pestana = null;
                    Dictionary<string, string> listaExtras = UtilCadenas.ObtenerPropiedadesDeTexto(filaProyGrafo.Extra);

                    if (listaExtras.ContainsKey(ConfiguracionGrafoFichaRec.PestanyaBusqueda.ToString()))
                    {
                        pestana = listaExtras[ConfiguracionGrafoFichaRec.PestanyaBusqueda.ToString()];

                        Guid pestanyaId;
                        if (Guid.TryParse(pestana, out pestanyaId))
                        {
                            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                            DataWrapperProyecto proyDWP = proyectoCL.ObtenerPestanyasProyecto(ProyectoSeleccionado.Clave);
                            AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = proyDWP.ListaProyectoPestanyaMenu.FirstOrDefault(proy => proy.PestanyaID.Equals(pestanyaId));
                            if (filaPestanya != null)
                            {
                                pestana = ObtenerNameUrlPestanya(filaPestanya).Value;
                                pestana = UtilCadenas.ObtenerTextoDeIdioma(pestana, UtilIdiomas.LanguageCode, IdiomaPorDefecto);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(pestana))
                    {
                        pestana = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    }

                    GrafoRecurso grafoRecurso = new GrafoRecurso();
                    grafoRecurso.PropEnlace = filaProyGrafo.PropEnlace;
                    grafoRecurso.NodosLimiteNivel = filaProyGrafo.NodosLimiteNivel.Value;
                    grafoRecurso.Extra = filaProyGrafo.Extra;
                    grafoRecurso.UrlBusqueda = urlBusqueda + pestana;
                    grafoRecurso.UrlBusquedaGrafo = urlBusquedaDbpedia + pestana;
                    grafoRecurso.TipoRecurso = tipoRecurso;

                    listaGrafosRecurso.Add(grafoRecurso);
                }
            }

            return listaGrafosRecurso;
        }

        /// <summary>
        /// Si el recurso está compartido, devuelve la URL canónica al recurso original
        /// </summary>
        /// <returns>URL del recurso original al compartido.</returns>
        protected override void ObtenerUrlCanonica()
        {
            AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = ProyectoSeleccionado.FilaProyecto;
            bool recursoCompartido = false;
            string urlFichaCompleta = string.Empty;

            if (Documento == null || Documento.TipoDocumentacion.Equals(TiposDocumentacion.Ontologia) || Documento.TipoDocumentacion.Equals(TiposDocumentacion.OntologiaSecundaria))
            {
                return;
            }

            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            string urlCanonica = documentacionCN.ObtenerDocumentoUrlCanonica(DocumentoID);

            if (!string.IsNullOrEmpty(urlCanonica))
            {
                UrlCanonical = urlCanonica;
            }
            else if (Documento.Compartido)
            {
                // Obtener el origen del recurso.
                Guid baseRecursosID = Documento.FilaDocumentoWebVinBR_Publicador.BaseRecursosID;
                Guid proyectoID = Documento.GestorDocumental.ObtenerProyectoID(baseRecursosID);

                if (!proyectoID.Equals(Guid.Empty) && !proyectoID.Equals(ProyectoAD.MetaProyecto))
                {
                    // Compartido en una Base de Recursos de proyecto.
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);

                    // Recurso compartido y con el recurso original eliminado.
                    if (Documento.FilaDocumentoWebVinBR_Publicador.Eliminado)
                    {
                        filaProy = ObtenerPrimerProyectoPublicoCompartidoRecurso();
                    }
                    else
                    {
                        filaProy = proyCL.ObtenerFilaProyecto(proyectoID);
                    }

                    string nombreCortoProy = "";

                    if (filaProy != null)
                    {
                        nombreCortoProy = filaProy.NombreCorto;
                    }

                    urlFichaCompleta = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, nombreCortoProy, UrlPerfil, Documento, false);
                }
                else
                {
                    urlFichaCompleta = ObtenerUrlCanonicaRecursoPublicadoBRPersonal();
                }

                bool urlCambiada = !Request.Path.ToString().Equals(urlFichaCompleta);
                if (recursoCompartido || urlCambiada)
                {
                    UrlCanonical = urlFichaCompleta;
                }
            }
            else
            {
                base.ObtenerUrlCanonica();
            }
        }

        private string ObtenerUrlCanonicaRecursoPublicadoBRPersonal()
        {
            AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = ProyectoSeleccionado.FilaProyecto;
            bool hayQueLlevarAMyGnoss = false;
            string urlFichaCompleta = string.Empty;
            if (GestorDocumental.DataWrapperDocumentacion != null)
            {
                // Compartido en una Base de Recursos personal.
                if (Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count > 0)
                {
                    // Persona
                    AD.EntityModel.Models.PersonaDS.Persona filaPersona = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory).ObtenerPersonaPorIdentidadCargaLigera(Documento.CreadorID);

                    bool documentoEnCatPublica = false;
                    if (filaPersona.UsuarioID.HasValue)
                    {
                        documentoEnCatPublica = ObtenerSiDocumentoBRUsuarioEstaEnCatPublica(filaPersona.UsuarioID.Value);
                    }

                    // Comprobar la visibilidad del perfil en mygnoss, ¿es visible por los bots / usuarios invitados?
                    ConfiguracionGnossPersona filaConfigPersona = filaPersona.ConfiguracionGnossPersona;
                    if (documentoEnCatPublica && filaPersona.EsBuscableExternos && filaConfigPersona != null && filaConfigPersona.VerRecursosExterno)
                    {
                        // Obtenemos la URL del recurso en la ficha del perfil
                        hayQueLlevarAMyGnoss = true;
                    }
                    else
                    {
                        // Obtenemos la URL del primer sitio en el que se ha compartido el recurso.
                        filaProy = ObtenerPrimerProyectoPublicoCompartidoRecurso();
                    }
                }
                else if (Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count > 0)
                {
                    // Organizacion
                    DataWrapperOrganizacion orgDS = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory).ObtenerOrganizacionDeIdentidad(Documento.CreadorID);
                    bool organizacionBuscablePorExternos = false;
                    if (orgDS.ListaOrganizacion.Count > 0)
                    {
                        AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrg = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory).ObtenerOrganizacionDeIdentidad(Documento.CreadorID).ListaOrganizacion.FirstOrDefault();
                        ConfiguracionGnossOrg filasConfigOrganizacion = filaOrg.ConfiguracionGnossOrg;

                        if (filasConfigOrganizacion != null)
                        {
                            organizacionBuscablePorExternos = filaOrg.EsBuscableExternos && filasConfigOrganizacion.VerRecursosExterno;
                        }
                    }
                    AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR = Documento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Find(doc => doc.IdentidadPublicacionID.Equals(Documento.CreadorID) && doc.TipoPublicacion.Equals((short)TipoPublicacion.Publicado));

                    AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion filaBROrg = Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Where(item => item.BaseRecursosID.Equals((filaDocVinBR).BaseRecursosID)).FirstOrDefault();

                    bool documentoEnCatPublica = ObtenerSiDocumentoBROrganizacionEstaEnCatPublica(filaBROrg.OrganizacionID);

                    if (documentoEnCatPublica && organizacionBuscablePorExternos)
                    {
                        hayQueLlevarAMyGnoss = true;
                    }
                    else
                    {
                        // Obtenemos la URL del primer sitio en el que se ha compartido el recurso.
                        filaProy = ObtenerPrimerProyectoPublicoCompartidoRecurso();
                    }
                }
            }

            //Si hay identidad habrá que llevar con esa:
            if (hayQueLlevarAMyGnoss)
            {
                GestionIdentidades gestorIdentAux = new GestionIdentidades(new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory).ObtenerIdentidadPorID(Documento.CreadorID, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                urlFichaCompleta = GnossUrlsSemanticas.GetURLBaseRecursosFichaMyGnoss(BaseURLIdioma, UtilIdiomas, UrlPerfil, gestorIdentAux.ListaIdentidades[Documento.CreadorID], Documento);

                gestorIdentAux.Dispose();
            }
            else
            {
                if (filaProy != null)
                {
                    urlFichaCompleta = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, filaProy.NombreCorto, UrlPerfil, Documento, false);
                }
                else
                {
                    urlFichaCompleta = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, Documento, false);
                }
            }
            return urlFichaCompleta;
        }

        private bool ObtenerSiDocumentoBRUsuarioEstaEnCatPublica(Guid pUsuarioID)
        {
            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
            GestionTesauro gestionTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroUsuario(pUsuarioID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            tesauroCN.Dispose();

            Guid clavePublica = (gestionTesauro.TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroPublicoID.Value;
            bool documentoEnCatPublica = false;
            foreach (CategoriaTesauro categoria in Documento.Categorias.Values)
            {
                if (categoria.PadreNivelRaiz.Clave == clavePublica)
                {
                    documentoEnCatPublica = true;
                    break;
                }
            }

            return documentoEnCatPublica;
        }

        private bool ObtenerSiDocumentoBROrganizacionEstaEnCatPublica(Guid pOrganizacionID)
        {
            bool documentoEnCatPublica = false;

            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
            Documento.GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroOrganizacion(pOrganizacionID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            tesauroCN.Dispose();

            Guid clavePublica = (Documento.GestorDocumental.GestorTesauro.TesauroDW.ListaTesauroOrganizacion.FirstOrDefault()).CategoriaTesauroPublicoID.Value;
            Documento.RecargarCategoriasTesauro();
            foreach (CategoriaTesauro categoria in Documento.Categorias.Values)
            {
                if (categoria.PadreNivelRaiz.Clave == clavePublica)
                {
                    documentoEnCatPublica = true;
                    break;
                }
            }

            return documentoEnCatPublica;
        }

        private AD.EntityModel.Models.ProyectoDS.Proyecto ObtenerPrimerProyectoPublicoCompartidoRecurso()
        {
            AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = ProyectoSeleccionado.FilaProyecto;
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);

            //Cuidado con los recursos compartidos en una base de recursos privada
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasBR = Documento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.Eliminado == false && doc.PrivadoEditores == false && !doc.TipoPublicacion.Equals((short)TipoPublicacion.Publicado)).OrderByDescending(doc => doc.FechaPublicacion).ToList();

            bool encontrado = false;
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos fila in filasBR)
            {
                Guid baseRecursosID = fila.BaseRecursosID;
                Guid proyID = Documento.GestorDocumental.ObtenerProyectoID(baseRecursosID);

                filaProy = proyCL.ObtenerFilaProyecto(proyID);

                if (!proyID.Equals(Guid.Empty) && proyID != ProyectoAD.MetaProyecto && proyID != Guid.Empty && filaProy.Estado != (short)EstadoProyecto.Cerrado && filaProy.Estado != (short)EstadoProyecto.CerradoTemporalmente && filaProy.TipoAcceso != (short)TipoAcceso.Privado && filaProy.TipoAcceso != (short)TipoAcceso.Reservado)
                {
                    encontrado = true;
                    break;
                }
            }

            proyCL.Dispose();

            if (encontrado)
            {
                return filaProy;
            }
            else
            {
                return null;
            }
        }

        private void cargarEnlaceDescargaRecurso(ResourceModel pFichaRecurso)
        {
            #region Enlace a la descarga del recurso

            bool AbrirEnNuevaVentana = false;
            string enlaceFichaCompleta = "";
            string baseUrl = UtilDominios.ObtenerDominioUrl(ProyectoSeleccionado.UrlPropia(UtilIdiomas.LanguageCode), true);

            if (Documento.TipoDocumentacion != TiposDocumentacion.Video && Documento.TipoDocumentacion != TiposDocumentacion.ReferenciaADoc && Documento.TipoDocumentacion != TiposDocumentacion.Nota && Documento.TipoDocumentacion != TiposDocumentacion.Newsletter && Documento.TipoDocumentacion != TiposDocumentacion.Semantico)
            {
                if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo)
                {
                    AbrirEnNuevaVentana = true;
                    enlaceFichaCompleta = Documento.Enlace;

                    if ((enlaceFichaCompleta.Length < 5) || (!enlaceFichaCompleta.Substring(0, 4).Equals("http")))
                    {
                        enlaceFichaCompleta = $"http://{enlaceFichaCompleta}";
                    }
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    string nombreProyAux = "";

                    if (OrganizacionPublicadorID == Guid.Empty && PersonaPublicadorID == Guid.Empty)
                    {
                        nombreProyAux = ProyectoVirtual.NombreCorto;
                    }
                    enlaceFichaCompleta = mControladorBase.UrlsSemanticas.GetURLBaseRecursosVerDocumentoCreado(BaseURLIdioma, UtilIdiomas, nombreProyAux, UrlPerfil, Documento.Clave, Documento.FilaDocumento.ElementoVinculadoID.ToString(), false, IdentidadActual.OrganizacionID != null);
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.FicheroServidor || Documento.TipoDocumentacion == TiposDocumentacion.Imagen)
                {
                    string tipo = ControladorDocumentacion.ObtenerTipoEntidadAdjuntarDocumento(Documento.TipoEntidadVinculada);
                    string extension = Path.GetExtension(Documento.NombreDocumento).ToLower();

                    if (IdiomaUsuario != IdiomaPorDefecto)
                    {
                        baseUrl = $"{baseUrl}/{IdiomaUsuario}";
                    }

                    if (extension == ".swf")
                    {
                        AbrirEnNuevaVentana = true;
                    }

                    if (Documento.TipoEntidadVinculada == TipoEntidadVinculadaDocumento.Web && PersonaPublicadorID != Guid.Empty && !Documento.Compartido)
                    {
                        enlaceFichaCompleta = $"{baseUrl}/download-file?tipo={tipo}&doc={Documento.Clave}&nombre={System.Net.WebUtility.UrlEncode(Documento.NombreDocumento)}&ext={extension}&personaID={PersonaPublicadorID.ToString()}&ID={mControladorBase.UsuarioActual.IdentidadID}";
                    }
                    else if (Documento.TipoEntidadVinculada == TipoEntidadVinculadaDocumento.Web && OrganizacionPublicadorID != Guid.Empty && !Documento.Compartido)
                    {
                        enlaceFichaCompleta = $"{baseUrl}/download-file?tipo={tipo}&org={Documento.FilaDocumento.OrganizacionID}&doc={Documento.Clave}&nombre={System.Net.WebUtility.UrlEncode(Documento.NombreDocumento)}&ext={extension}&ID={mControladorBase.UsuarioActual.IdentidadID}";
                    }
                    else
                    {
                        if (Documento.ProyectoID != ProyectoAD.MetaProyecto)
                        {
                            string proyectoID = "";

                            if (Documento.FilaDocumento.ProyectoID.HasValue)
                            {
                                proyectoID = $"&proy={Documento.FilaDocumento.ProyectoID.Value.ToString()}";
                            }
                            enlaceFichaCompleta = $"{baseUrl}/download-file?tipo={tipo}&org={Documento.FilaDocumento.OrganizacionID}{proyectoID}&doc={Documento.Clave}&nombre={System.Net.WebUtility.UrlEncode(Documento.NombreDocumento)}&ext={extension}&ID={mControladorBase.UsuarioActual.IdentidadID}&proyectoID={ProyectoSeleccionado.Clave}";
                        }
                        else//Es un recurso publicado en MyGnoss y mostrado en una comunidad:
                        {
                            if (Documento.GestorDocumental.GestorIdentidades != null && Documento.GestorDocumental.GestorIdentidades.ListaIdentidades[Documento.CreadorID].PerfilUsuario.FilaPerfil.PersonaID.HasValue)
                            {
                                enlaceFichaCompleta = $"{baseUrl}/download-file?tipo={tipo}&doc={Documento.Clave}&nombre={System.Net.WebUtility.UrlEncode(Documento.NombreDocumento)}&ext={extension}&personaID={Documento.GestorDocumental.GestorIdentidades.ListaIdentidades[Documento.CreadorID].PerfilUsuario.FilaPerfil.PersonaID.ToString()}";
                            }
                            else
                            {
                                enlaceFichaCompleta = $"{baseUrl}/download-file?tipo={tipo}&org={Documento.FilaDocumento.OrganizacionID}&doc={Documento.Clave}&nombre={System.Net.WebUtility.UrlEncode(Documento.NombreDocumento)}&ext={extension}";
                            }

                            if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                            {
                                enlaceFichaCompleta += $"&proyectoID={ProyectoSeleccionado.Clave}";
                            }
                        }
                    }
                }
            }

            if ((Documento.EsPresentacionIncrustada || Documento.EsVideoIncrustado) && Documento.TipoDocumentacion != TiposDocumentacion.VideoBrightcove && Documento.TipoDocumentacion != TiposDocumentacion.AudioBrightcove && Documento.TipoDocumentacion != TiposDocumentacion.VideoTOP && Documento.TipoDocumentacion != TiposDocumentacion.AudioTOP)
            {
                AbrirEnNuevaVentana = true;
                enlaceFichaCompleta = Documento.Enlace;
            }

            #endregion

            if (enlaceFichaCompleta != "")
            {
                try
                {
                    string extension = Path.GetExtension(Documento.NombreDocumento).ToLower();

                    if (extension == ".swf" && Documento.TipoDocumentacion != TiposDocumentacion.Hipervinculo)
                    {
                        pFichaRecurso.UrlPreview = enlaceFichaCompleta;

                        enlaceFichaCompleta = enlaceFichaCompleta.Replace("download-file", "VisualizarFlash");
                    }
                }
                catch (Exception)
                {
                }
            }

            pFichaRecurso.UrlDocument = enlaceFichaCompleta;
            pFichaRecurso.OpenInNewWindow = AbrirEnNuevaVentana;
        }

        private string CargarUrlVistaPreviaMini(Documento pDocumento)
        {
            string urlImagen = "";

            if (pDocumento != null && (pDocumento.TipoDocumentacion == TiposDocumentacion.Hipervinculo || pDocumento.TipoDocumentacion == TiposDocumentacion.Video || pDocumento.TipoDocumentacion == TiposDocumentacion.VideoBrightcove || pDocumento.TipoDocumentacion == TiposDocumentacion.VideoTOP || pDocumento.TipoDocumentacion == TiposDocumentacion.FicheroServidor || pDocumento.TipoDocumentacion == TiposDocumentacion.Nota))
            {
                string fileName = $"{System.Net.WebUtility.UrlEncode(pDocumento.Clave.ToString().ToLower())}.jpg";
                if (pDocumento.FilaDocumento.VersionFotoDocumento.HasValue && pDocumento.FilaDocumento.VersionFotoDocumento > 0)
                {
                    urlImagen = $"{BaseURLContent}/{UtilArchivos.ContentImagenesEnlaces}/{UtilArchivos.DirectorioDocumento(pDocumento.Clave)}/{fileName}?{pDocumento.FilaDocumento.VersionFotoDocumento}";
                }
            }
            else if (pDocumento != null && (pDocumento.TipoDocumentacion == TiposDocumentacion.Imagen))
            {
                string extension = ".jpg";
                if (pDocumento.NombreEntidadVinculada == "Wiki2")
                {
                    extension = pDocumento.Enlace.Substring(pDocumento.Enlace.LastIndexOf('.'));
                }

                string fileName = $"{System.Net.WebUtility.UrlEncode(pDocumento.Clave.ToString().ToLower())}_peque.{extension}";
                if (pDocumento.FilaDocumento.VersionFotoDocumento.HasValue && pDocumento.FilaDocumento.VersionFotoDocumento > 0)
                {
                    urlImagen = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesDocumentos}/Miniatura/{UtilArchivos.DirectorioDocumento(pDocumento.Clave)}/{fileName}?{pDocumento.FilaDocumento.VersionFotoDocumento}";
                }
            }
            else if (pDocumento != null && (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico) && !string.IsNullOrEmpty(pDocumento.FilaDocumento.NombreCategoriaDoc))
            {
                string fileName = pDocumento.FilaDocumento.NombreCategoriaDoc;

                if (fileName.Contains(".jpg"))
                {
                    if (fileName.Contains("|"))
                    {
                        fileName = fileName.Split('|')[0];
                    }

                    return $"{BaseURLContent}/{fileName.Substring(fileName.LastIndexOf(",") + 1).Replace(".jpg", $"_{fileName.Substring(0, fileName.IndexOf(","))}.jpg")}";
                }
            }

            return urlImagen;
        }

        private string CargarUrlVistaPrevia(Documento pDocumento)
        {
            string urlVistaPrevia = "";

            if (pDocumento.TipoDocumentacion == TiposDocumentacion.Video || pDocumento.TipoDocumentacion == TiposDocumentacion.Imagen || pDocumento.EsPresentacionIncrustada || pDocumento.EsVideoIncrustado || pDocumento.EsAudioIncrustado || (pDocumento.TipoDocumentacion == TiposDocumentacion.FicheroServidor && pDocumento.Extension.ToLower() == ".swf"))
            {
                if (pDocumento.EsVideoIncrustado)
                {
                    string enlace = pDocumento.Enlace;

                    if (enlace.StartsWith("http://")) { enlace = enlace.Substring(7); }
                    if (enlace.StartsWith("https://")) { enlace = enlace.Substring(8); }
                    if (enlace.StartsWith("www.")) { enlace = enlace.Substring(4); }

                    if (enlace.StartsWith("youtube.com") || enlace.StartsWith("youtu.be"))
                    {
                        string v = "";
                        if (enlace.StartsWith("youtu.be"))
                        {
                            v = enlace.Replace("youtu.be", "").Trim('/');
                        }
                        else
                        {
                            v = HttpUtility.ParseQueryString(new Uri(pDocumento.Enlace).Query).Get("v");
                        }

                        //si el enlace viene de youtube y el parámetro v no es null
                        if (!string.IsNullOrEmpty(v))
                        {
                            string parametrosExtra = "?rel=0";

                            if (ParametroProyecto.ContainsKey(ParametroAD.ParametrosExtraYoutube))
                            {
                                string parametrosExtraConfig = ParametroProyecto[ParametroAD.ParametrosExtraYoutube];

                                if (parametrosExtraConfig.StartsWith("?") || parametrosExtraConfig.StartsWith("&"))
                                {
                                    parametrosExtraConfig = parametrosExtraConfig.Substring(1);
                                }

                                if (!string.IsNullOrEmpty(parametrosExtraConfig))
                                {
                                    parametrosExtra += $"&{parametrosExtraConfig}";
                                }
                            }

                            urlVistaPrevia = $"//www.youtube.com/embed/{v}{parametrosExtra}";

                            urlVistaPrevia = ModificarVideoSiNoHayCookiesYoutube(urlVistaPrevia);
                        }
                    }
                    else if (enlace.StartsWith("vimeo.com"))
                    {
                        string v = (new Uri(pDocumento.Enlace)).AbsolutePath;
                        int idVideo;
                        int inicio = v.LastIndexOf("/");
                        bool exito = int.TryParse(v.Substring(inicio + 1, v.Length - inicio - 1), out idVideo);

                        if (exito)
                        {
                            urlVistaPrevia = $"//player.vimeo.com/video/{idVideo}";
                        }
                    }
                    else if (pDocumento.TipoDocumentacion == TiposDocumentacion.VideoBrightcove)
                    {
                        #region Brightcove

                        //if (!Documento.FilaDocumento.IsNombreCategoriaDocNull() && Documento.FilaDocumento.NombreCategoriaDoc.StartsWith("<object"))
                        //{
                        //    divObjeto.InnerHtml = Documento.FilaDocumento.NombreCategoriaDoc;
                        //}
                        //else
                        //{
                        //    ParametroGeneralDS.ParametroGeneralRow ParametrosGeneralesProyecto = ControladorProyecto.ObtenerFilaParametrosGeneralesDeProyecto(Documento.ProyectoID);

                        //    string idReproductor = ParametrosGeneralesProyecto.BrightcoveReproductorID;
                        //    string idVideo = Documento.Enlace;

                        //    string fotoPendiente = "<img height=\"336\" src=\"" + BaseURLStatic + "/img/brightcove_espera.jpg\">";
                        //    string fotoError = "<img height=\"336\" src=\"" + BaseURLStatic + "/img/brightcove_error.jpg\">";

                        //    if (!string.IsNullOrEmpty(idVideo))
                        //    {
                        //        string videoBrightcove = "";
                        //        videoBrightcove += "<!-- Start of Brightcove Player -->";
                        //        videoBrightcove += "<div style=\"display:none\">";
                        //        videoBrightcove += "</div>";
                        //        videoBrightcove += "<script language=\"JavaScript\" type=\"text/javascript\" src=\"http://admin.brightcove.com/js/BrightcoveExperiences.js\"></script>";
                        //        if (RequestUrl.ToLower().StartsWith("https"))
                        //        {
                        //            videoBrightcove += "<script src=\"https://admin.brightcove.com/js/APIModules_all.js\"></script>";
                        //        }
                        //        else
                        //        {
                        //            videoBrightcove += "<script src=\"http://admin.brightcove.com/js/APIModules_all.js\"></script>";
                        //        }
                        //        videoBrightcove += "<object id=\"myExperience\" class=\"BrightcoveExperience\">";
                        //        videoBrightcove += "<param name=\"bgcolor\" value=\"#FFFFFF\" />";
                        //        videoBrightcove += "<param name=\"width\" value=\"582\" />";
                        //        videoBrightcove += "<param name=\"height\" value=\"336\" />";
                        //        videoBrightcove += "<param name=\"playerID\" value=\"" + idReproductor + "\" />";
                        //        videoBrightcove += "<param name=\"isVid\" value=\"true\" />";
                        //        videoBrightcove += "<param name=\"isUI\" value=\"true\" />";
                        //        videoBrightcove += "<param name=\"@videoPlayer\"  value=\"" + idVideo + "\" />";
                        //        videoBrightcove += "</object>";
                        //        videoBrightcove += "<!-- End of Brightcove Player -->";
                        //        divObjeto.InnerHtml = videoBrightcove;
                        //    }
                        //    else
                        //    {
                        //        try
                        //        {
                        //            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService);
                        //            DocumentacionDS docDS = docCN.ObtenerDocumentoTokenBrightcovePorID(Documento.Clave);
                        //            if (docDS.DocumentoTokenBrightcove[0].Estado == (short)EstadoSubidaVideo.Fallido)
                        //            {
                        //                divObjeto.InnerHtml = fotoError;

                        //            }
                        //            else
                        //            {
                        //                divObjeto.InnerHtml = fotoPendiente;
                        //            }
                        //        }
                        //        catch (Exception)
                        //        {
                        //            divObjeto.InnerHtml = fotoPendiente;
                        //        }
                        //    }
                        //}
                        #endregion
                    }
                    else if (pDocumento.TipoDocumentacion == TiposDocumentacion.VideoTOP)
                    {
                        if (!string.IsNullOrEmpty(pDocumento.Enlace))
                        {
                            urlVistaPrevia = $"TOP:OK:{pDocumento.Enlace}";
                        }
                        else
                        {
                            try
                            {
                                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                                DataWrapperDocumentacion docDW = docCN.ObtenerDocumentoTokenTOPPorID(pDocumento.Clave);
                                if (docDW.ListaDocumentoTokenTOP.FirstOrDefault().Estado == (short)EstadoSubidaVideo.Fallido)
                                {
                                    urlVistaPrevia = "TOP:Error";
                                }
                                else
                                {
                                    urlVistaPrevia = "TOP:Pendiente";
                                }
                            }
                            catch (Exception)
                            {
                                urlVistaPrevia = "TOP:Pendiente";
                            }
                        }
                    }
                    else if (enlace.StartsWith("ted.com/talks/") || enlace.StartsWith("tedxtalks.ted.com/video/"))
                    {
                        if (enlace.StartsWith("ted.com/talks/"))
                        {
                            if (enlace.EndsWith(".html"))
                            {
                                enlace = $"{enlace}.html";
                            }

                            urlVistaPrevia = $"https://embed-ssl{enlace}";
                        }
                        else if (enlace.StartsWith("tedxtalks.ted.com/video/"))
                        {
                            urlVistaPrevia = $"http://{enlace}/player?layout=&read_more=1";
                        }
                    }
                }
                else if (pDocumento.EsAudioIncrustado)
                {
                    if (pDocumento.TipoDocumentacion == TiposDocumentacion.AudioBrightcove)
                    {
                        #region Brightcove audio
                        //    ParametroGeneralDS.ParametroGeneralRow ParametrosGeneralesProyecto = ControladorProyecto.ObtenerFilaParametrosGeneralesDeProyecto(Documento.ProyectoID);

                        //    string idReproductor = ParametrosGeneralesProyecto.BrightcoveReproductorID;
                        //    string idVideo = Documento.Enlace;

                        //    string fotoPendiente = "<img height=\"336\" src=\"" + BaseURLStatic + "/img/brightcove_audio_espera.jpg\">";
                        //    string fotoError = "<img height=\"336\" src=\"" + BaseURLStatic + "/img/brightcove_error.jpg\">";

                        //    if (!string.IsNullOrEmpty(idVideo))
                        //    {
                        //        string videoBrightcove = "";
                        //        videoBrightcove += "<!-- Start of Brightcove Player -->";
                        //        videoBrightcove += "<div style=\"display:none\">";
                        //        videoBrightcove += "</div>";
                        //        videoBrightcove += "<script language=\"JavaScript\" type=\"text/javascript\" src=\"http://admin.brightcove.com/js/BrightcoveExperiences.js\"></script>";
                        //        if (RequestUrl.ToLower().StartsWith("https"))
                        //        {
                        //            videoBrightcove += "<script src=\"https://admin.brightcove.com/js/APIModules_all.js\"></script>";
                        //        }
                        //        else
                        //        {
                        //            videoBrightcove += "<script src=\"http://admin.brightcove.com/js/APIModules_all.js\"></script>";
                        //        }
                        //        videoBrightcove += "<object id=\"myExperience\" class=\"BrightcoveExperience\">";
                        //        videoBrightcove += "<param name=\"bgcolor\" value=\"#FFFFFF\" />";
                        //        videoBrightcove += "<param name=\"width\" value=\"600\" />";
                        //        videoBrightcove += "<param name=\"height\" value=\"60\" />";
                        //        videoBrightcove += "<param name=\"playerID\" value=\"" + idReproductor + "\" />";
                        //        videoBrightcove += "<param name=\"isVid\" value=\"true\" />";
                        //        videoBrightcove += "<param name=\"isUI\" value=\"true\" />";
                        //        videoBrightcove += "<param name=\"@videoPlayer\"  value=\"" + idVideo + "\" />";
                        //        videoBrightcove += "</object>";
                        //        videoBrightcove += "<!-- End of Brightcove Player -->";
                        //        divObjeto.InnerHtml = videoBrightcove;
                        //    }
                        //    else
                        //    {
                        //        try
                        //        {
                        //            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService);
                        //            DocumentacionDS docDS = docCN.ObtenerDocumentoTokenBrightcovePorID(Documento.Clave);
                        //            if (docDS.DocumentoTokenBrightcove[0].Estado == (short)EstadoSubidaVideo.Fallido)
                        //            {
                        //                divObjeto.InnerHtml = fotoError;
                        //            }
                        //            else
                        //            {
                        //                divObjeto.InnerHtml = fotoPendiente;
                        //            }
                        //        }
                        //        catch (Exception)
                        //        {
                        //            divObjeto.InnerHtml = fotoPendiente;
                        //        }
                        //    }
                        #endregion
                    }
                    else if (pDocumento.TipoDocumentacion == TiposDocumentacion.AudioTOP)
                    {
                        if (!string.IsNullOrEmpty(pDocumento.Enlace))
                        {
                            urlVistaPrevia = $"TOP:OK:{pDocumento.Enlace}";
                        }
                        else
                        {
                            try
                            {
                                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                                DataWrapperDocumentacion docDS = docCN.ObtenerDocumentoTokenTOPPorID(pDocumento.Clave);
                                if (docDS.ListaDocumentoTokenTOP.FirstOrDefault().Estado == (short)EstadoSubidaVideo.Fallido)
                                {
                                    urlVistaPrevia = "TOP:Error";
                                }
                                else
                                {
                                    urlVistaPrevia = "TOP:Pendiente";
                                }
                            }
                            catch (Exception)
                            {
                                urlVistaPrevia = "TOP:Pendiente";
                            }
                        }
                    }
                }
                else if (pDocumento.EsPresentacionIncrustada)
                {
                    string enlaceSlideshare = pDocumento.Enlace;
                    if (enlaceSlideshare.Contains("?"))
                    {
                        enlaceSlideshare = enlaceSlideshare.Substring(0, enlaceSlideshare.IndexOf("?"));
                    }

                    string ruta = $"https://www.slideshare.net/api/oembed/2?url={enlaceSlideshare}";

                    try
                    {
                        XmlDocument docXml = new XmlDocument();
                        //Problema para los TLS
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                        docXml.Load(ruta);

                        string id = docXml.SelectSingleNode("oembed/slideshow-id").InnerText;
                        urlVistaPrevia = $"https://www.slideshare.net/slideshow/embed_code/{id}";
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, mlogger);
                    }
                }
                else if (pDocumento.TipoDocumentacion == TiposDocumentacion.Imagen)
                {
                    string extension = ".jpg";
                    if (pDocumento.NombreEntidadVinculada == "Wiki2")
                    {
                        extension = pDocumento.Enlace.Substring(pDocumento.Enlace.LastIndexOf('.'));
                    }

                    string versionFoto = "";
                    if (Documento.FilaDocumento.VersionFotoDocumento.HasValue && Documento.FilaDocumento.VersionFotoDocumento.Value > 0)
                    {
                        versionFoto += $"?v={Documento.FilaDocumento.VersionFotoDocumento.Value}";
                    }

                    if (PersonaPublicadorID != Guid.Empty)
                    {
                        urlVistaPrevia = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesDocumentos}/{UtilArchivos.ContentImagenesPersonas}/{PersonaPublicadorID.ToString().ToLower()}/{pDocumento.Clave.ToString().ToLower()}{extension}{versionFoto}";
                    }
                    else if (OrganizacionPublicadorID != Guid.Empty)
                    {
                        urlVistaPrevia = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesDocumentos}/{UtilArchivos.ContentImagenesOrganizaciones}/{OrganizacionPublicadorID.ToString().ToLower()}/{pDocumento.Clave.ToString().ToLower()}{extension}{versionFoto}";
                    }
                    else
                    {
                        urlVistaPrevia = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesDocumentos}/{UtilArchivos.DirectorioDocumento(pDocumento.Clave)}/{pDocumento.Clave.ToString().ToLower()}{extension}{versionFoto}";
                    }
                }
                else if (Documento.EsFicheroDigital)
                {
                    urlVistaPrevia = BaseURLContent + "/Videos/" + Documento.Enlace;
                }
            }

            return urlVistaPrevia;
        }

        private string ObtenerLicencia()
        {
            string htmlLicencia = "";

            if (Documento.PermiteLicencia)
            {
                if (Documento.FilaDocumento.Licencia != null)
                {
                    string licencia = Documento.FilaDocumento.Licencia;

                    /* La imagen de Copyright ahora estará después del texto */
                    string Html_Comun = "@2@ <a rel=\"license\" target=\"_blank\" href=\"@0@\" class=\"right10\"><img alt=\"Licencia Creative Commons\" style=\"border-width:0;vertical-align:middle;\" src=\"@1@\" /></a>";

                    string Img_Licencia_00 = "https://licensebuttons.net/l/by/3.0/88x31.png";
                    string Img_Licencia_01 = "https://licensebuttons.net/l/by-sa/3.0/88x31.png";
                    string Img_Licencia_02 = "https://licensebuttons.net/l/by-nd/3.0/88x31.png";
                    string Img_Licencia_10 = "https://licensebuttons.net/l/by-nc/3.0/88x31.png";
                    string Img_Licencia_11 = "https://licensebuttons.net/l/by-nc-sa/3.0/88x31.png";
                    string Img_Licencia_12 = "https://licensebuttons.net/l/by-nc-nd/3.0/88x31.png";

                    string html = Html_Comun;
                    string codigoLicencia = "";

                    if (licencia == "00")
                    {
                        html = html.Replace("@1@", Img_Licencia_00);
                        codigoLicencia = "by";
                    }
                    else if (licencia == "01")
                    {
                        html = html.Replace("@1@", Img_Licencia_01);
                        codigoLicencia = "by-sa";
                    }
                    else if (licencia == "02")
                    {
                        html = html.Replace("@1@", Img_Licencia_02);
                        codigoLicencia = "by-nd";
                    }
                    else if (licencia == "10")
                    {
                        html = html.Replace("@1@", Img_Licencia_10);
                        codigoLicencia = "by-nc";
                    }
                    else if (licencia == "11")
                    {
                        html = html.Replace("@1@", Img_Licencia_11);
                        codigoLicencia = "by-nc-sa";
                    }
                    else if (licencia == "12")
                    {
                        html = html.Replace("@1@", Img_Licencia_12);
                        codigoLicencia = "by-nc-nd";
                    }

                    string urlCC = UtilIdiomas.GetText("LICENCIASDOCUMENTOS", "URLCREATIVE", codigoLicencia);

                    htmlLicencia = html.Replace("@2@", $" {UtilIdiomas.GetText("LICENCIASDOCUMENTOS", "CONTENIDOLICENCIADO", urlCC)}").Replace("@0@", urlCC);
                }
                else
                {
                    if (!Documento.FilaDocumento.CreadorEsAutor && Documento.TipoDocumentacion != TiposDocumentacion.Semantico && (Documento.FilaDocumento.CompartirPermitido || ProyectoSeleccionado == null || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido))
                    {
                        htmlLicencia = UtilIdiomas.GetText("LICENCIASDOCUMENTOS", "RESPETOORIGINAL");
                    }
                }
            }
            else if (Documento.FilaDocumento.CompartirPermitido || ProyectoSeleccionado == null || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido)
            {
                if (Documento.TipoDocumentacion == TiposDocumentacion.Video)
                {
                    Uri uriPrueba;
                    if (!string.IsNullOrEmpty(Documento.Enlace) && Uri.TryCreate(Documento.Enlace, UriKind.Absolute, out uriPrueba))
                    {
                        htmlLicencia = UtilIdiomas.GetText("LICENCIASDOCUMENTOS", "RESPETOORIGINAL");
                    }
                }
                else if (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo)
                {
                    htmlLicencia = UtilIdiomas.GetText("LICENCIASDOCUMENTOS", "RESPETOORIGINAL");
                }
            }

            return htmlLicencia;
        }

        /// <summary>
        /// Agrega el ul con las categorías del recurso.
        /// </summary>
        /// <returns>Ul con las categorías del recurso</returns>
        public string CargarUrlBusqueda()
        {
            string nombreSem = PestanyaRecurso.Value;
            string urlBusqueda = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, false, nombreSem);
            return urlBusqueda;
        }

        /// <summary>
        /// Agrega el ul con las categorías del recurso.
        /// </summary>
        /// <returns>Ul con las categorías del recurso</returns>
        public List<CategoryModel> CargarCategoriasTesauro()
        {
            if (GestorDocumental.GestorTesauro == null)
            {
                CargarTesauro();
            }

            List<CategoryModel> listaCategorias = null;

            if (Documento.CategoriasTesauro.Count > 0)
            {
                listaCategorias = new List<CategoryModel>();

                foreach (CategoriaTesauro catTes in Documento.CategoriasTesauro.Values)
                {
                    CategoryModel categoriaTesauro = new CategoryModel();
                    categoriaTesauro.Key = catTes.Clave;
                    categoriaTesauro.Name = catTes.FilaCategoria.Nombre;
                    categoriaTesauro.LanguageName = catTes.FilaCategoria.Nombre;
                    listaCategorias.Add(categoriaTesauro);
                }
            }

            return listaCategorias;
        }

        /// <summary>
        /// Carga las etiquetas
        /// </summary>
        /// <returns></returns>
        public List<string> CargarEtiquetas()
        {
            List<string> listaEtiquetas = null;

            if (Documento.ListaTagsSoloLectura.Count > 0)
            {
                listaEtiquetas = new List<string>();

                foreach (string tag in Documento.ListaTagsSoloLectura)
                {
                    if (tag != "")
                    {
                        listaEtiquetas.Add(tag);
                    }
                }
            }

            return listaEtiquetas;
        }

        private List<SharedBRModel> CargarBRCompartidas()
        {
            List<SharedBRModel> listaBRCompartidas = new List<SharedBRModel>();

            List<Guid> ProyectosID = new List<Guid>();
            var listaBaseRecursos = Documento.BaseRecursos.Take(20);
            foreach (Guid baseRecursos in listaBaseRecursos)
            {
                ProyectosID.Add(Documento.GestorDocumental.ObtenerProyectoID(baseRecursos));
            }
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto proyectosCompartidosDWP = proyCN.ObtenerProyectosPorIDsCargaLigera(ProyectosID);

            List<Guid> proyectosParticipaUsuario = proyCN.ObtenerIdProyectosParticipaPersona(UsuarioActual.PersonaID);

            proyCN.Dispose();

            foreach (Guid baseRecursos in listaBaseRecursos)
            {
                Guid proyectoID = Documento.GestorDocumental.ObtenerProyectoID(baseRecursos);
                Guid organizacionID = Documento.GestorDocumental.ObtenerOrganizacionID(baseRecursos);

                if (proyectoID != ProyectoSeleccionado.Clave || (proyectoID == ProyectoAD.MetaProyecto && ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto))
                {
                    if (GestorDocumental.DataWrapperDocumentacion != null)
                    {
                        List<AD.EntityModel.Models.Documentacion.BaseRecursosUsuario> filaBRUsu = Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Where(item => item.BaseRecursosID.Equals(baseRecursos)).ToList();

                        if ((proyectoID == ProyectoAD.MetaProyecto && (filaBRUsu.Count == 0 || filaBRUsu[0].UsuarioID == mControladorBase.UsuarioActual.UsuarioID)) || proyectoID != ProyectoAD.MetaProyecto)
                        {
                            //Entra si estando en la comunidad la BR no es de MyGnoss o, si está en MyGnoss o no es una BR de usuario,
                            //o si lo es, pertenece al usuario conectado.
                            AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = proyectosCompartidosDWP.ListaProyecto.FirstOrDefault(proy => proy.ProyectoID.Equals(proyectoID));

                            if (proyectoID != ProyectoAD.MetaProyecto && filaProy.Estado != (short)EstadoProyecto.Cerrado && filaProy.Estado != (short)EstadoProyecto.CerradoTemporalmente)
                            {
                                //Si no es my gnoss y el proyecto no esta cerrado entra
                                bool tieneAccesoComunidad = true;

                                bool proyPrivado = filaProy.TipoAcceso.Equals((short)TipoAcceso.Privado) || filaProy.TipoAcceso.Equals((short)TipoAcceso.Reservado);

                                if (proyPrivado)
                                {
                                    if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
                                    {
                                        tieneAccesoComunidad = false;
                                    }
                                    else
                                    {
                                        //Es una comunidad privada o reservada, solo se muestra la comunidad si participa en ella
                                        //tieneAccesoComunidad = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(identidad => identidad.ProyectoID.Equals(proyectoID) && identidad.Tipo != (short)TiposIdentidad.Organizacion);

                                        tieneAccesoComunidad = proyectosParticipaUsuario.Any(proyecto => proyecto.Equals(proyectoID));
                                    }
                                }

                                if (tieneAccesoComunidad)
                                {
                                    SharedBRModel brComunidad = new SharedBRModel();
                                    brComunidad.Key = baseRecursos;
                                    brComunidad.ProyectKey = filaProy.ProyectoID;
                                    brComunidad.OrganizationKey = ProyectoAD.MetaOrganizacion;
                                    brComunidad.Name = UtilCadenas.ObtenerTextoDeIdioma(filaProy.Nombre, IdiomaUsuario, IdiomaPorDefecto);
                                    brComunidad.Url = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, filaProy.NombreCorto, UrlPerfil, Documento, false);
                                    brComunidad.Private = proyPrivado;
                                    brComunidad.DeleteAvailable = (Documento.TienePermisosIdentidadEliminarRecursoEnBR(IdentidadActual, baseRecursos, ProyectoSeleccionado, UsuarioActual.UsuarioID, UsuarioActual.ProyectoID, EsIdentidadActualAdministradorOrganizacion, EsIdentidadActualSupervisorProyecto));

                                    listaBRCompartidas.Add(brComunidad);
                                }
                            }
                            else
                            {
                                if (!mControladorBase.UsuarioActual.EsIdentidadInvitada)
                                {
                                    List<AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion> filasBROrg = Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Where(item => item.BaseRecursosID.Equals(baseRecursos)).ToList();

                                    bool tieneAcceso = false;

                                    if (filasBROrg.Count > 0 && IdentidadActual.OrganizacionID.HasValue && IdentidadActual.OrganizacionID.Value == filasBROrg[0].OrganizacionID)
                                    {
                                        tieneAcceso = true;
                                    }

                                    if (filasBROrg.Count > 0 && tieneAcceso)
                                    {
                                        if (Documento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.FirstOrDefault(item => item.BaseRecursosID.Equals(baseRecursos) && item.DocumentoID.Equals(DocumentoID)).TipoPublicacion != 0)
                                        {
                                            SharedBRModel brOrganizacion = new SharedBRModel();
                                            brOrganizacion.Key = baseRecursos;
                                            brOrganizacion.ProyectKey = ProyectoAD.MetaProyecto;
                                            brOrganizacion.OrganizationKey = filasBROrg[0].OrganizacionID;
                                            brOrganizacion.Name = UtilIdiomas.GetText("ANYADIRGNOSS", "BRORGANIZACION") + IdentidadActual.PerfilUsuario.NombreOrganizacion;
                                            brOrganizacion.Url = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, true);
                                            brOrganizacion.Private = false;
                                            brOrganizacion.DeleteAvailable = (Documento.TienePermisosIdentidadEliminarRecursoEnBR(IdentidadActual, baseRecursos, ProyectoSeleccionado, UsuarioActual.UsuarioID, UsuarioActual.ProyectoID, EsIdentidadActualAdministradorOrganizacion, EsIdentidadActualSupervisorProyecto));

                                            listaBRCompartidas.Add(brOrganizacion);
                                        }
                                    }
                                    else if (Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Any(item => item.BaseRecursosID.Equals(baseRecursos)))
                                    {
                                        if (Documento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.FirstOrDefault(item => item.BaseRecursosID.Equals(baseRecursos) && item.DocumentoID.Equals(Documento.Clave)).TipoPublicacion != 0)
                                        {
                                            SharedBRModel brPersonal = new SharedBRModel();
                                            brPersonal.Key = baseRecursos;
                                            brPersonal.Name = UtilIdiomas.GetText("ANYADIRGNOSS", "BRPERSONAL");
                                            brPersonal.Url = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, false);
                                            brPersonal.Private = false;
                                            brPersonal.DeleteAvailable = (Documento.TienePermisosIdentidadEliminarRecursoEnBR(IdentidadActual, baseRecursos, ProyectoSeleccionado, UsuarioActual.UsuarioID, UsuarioActual.ProyectoID, EsIdentidadActualAdministradorOrganizacion, EsIdentidadActualSupervisorProyecto));

                                            listaBRCompartidas.Add(brPersonal);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return listaBRCompartidas;
        }


        /// <summary>
        /// Pinta el compartir en
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> CargarListaRedesSociales(ResourceModel pResource)
        {
            Dictionary<string, Dictionary<string, string>> listaRedesSociales = null;

            if (!Documento.EsBorrador && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido))
            {
                listaRedesSociales = new Dictionary<string, Dictionary<string, string>>();

                string linkPagina = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false);

                string descripcion = System.Net.WebUtility.UrlEncode(UtilCadenas.EliminarHtmlDeTexto(pResource.Description));

                if (descripcion.Length > 500)
                {
                    descripcion = descripcion.Substring(0, 500) + "...";
                }

                #region Lectura Acciones Eventos Redes Sociales

                Dictionary<TipoProyectoEventoAccion, string> listaEventos = ProyectoSeleccionado.ListaTipoProyectoEventoAccion;
                string accionTwitter = string.Empty;
                string accionFacebook = string.Empty;
                string accionLinkedIn = string.Empty;
                string accionReddit = string.Empty;
                string accionBlogger = string.Empty;
                if (listaEventos != null && listaEventos.Count > 0)
                {
                    //comprobación existencia evento lectura recurso                
                    if (listaEventos.ContainsKey(TipoProyectoEventoAccion.CompartirContenidoTwitter))
                    {
                        accionTwitter = listaEventos[TipoProyectoEventoAccion.CompartirContenidoTwitter];
                    }

                    if (listaEventos.ContainsKey(TipoProyectoEventoAccion.CompartirContenidoFacebook))
                    {
                        accionFacebook = listaEventos[TipoProyectoEventoAccion.CompartirContenidoFacebook];
                    }

                    if (listaEventos.ContainsKey(TipoProyectoEventoAccion.CompartirContenidoLinkedIn))
                    {
                        accionLinkedIn = listaEventos[TipoProyectoEventoAccion.CompartirContenidoLinkedIn];
                    }

                    if (listaEventos.ContainsKey(TipoProyectoEventoAccion.CompartirContenidoReddit))
                    {
                        accionReddit = listaEventos[TipoProyectoEventoAccion.CompartirContenidoReddit];
                    }

                    if (listaEventos.ContainsKey(TipoProyectoEventoAccion.CompartirContenidoBlogger))
                    {
                        accionBlogger = listaEventos[TipoProyectoEventoAccion.CompartirContenidoBlogger];
                    }
                }

                #endregion Eventos Redes Sociales

                string titulo = System.Net.WebUtility.UrlEncode(System.Text.RegularExpressions.Regex.Replace(HttpUtility.UrlDecode(pResource.Title), "<.*?>", string.Empty));

                #region Twitter

                string urlTweet = $"https://twitter.com/intent/tweet?original_referer={linkPagina}&text={titulo}&tw_p=tweetbutton&url={linkPagina}";

                Dictionary<string, string> listaAtributosTwitter = new Dictionary<string, string>();
                listaAtributosTwitter.Add("onclick", "MostrarPopUpCentrado('" + urlTweet + "',895,525);" + accionTwitter);

                listaRedesSociales.Add("Twitter", listaAtributosTwitter);

                #endregion

                #region Facebook

                string urlFacebook = "https://www.facebook.com/sharer/sharer.php?u=" + linkPagina;

                Dictionary<string, string> listaAtributosFacebook = new Dictionary<string, string>();
                listaAtributosFacebook.Add("onclick", $"MostrarPopUpCentrado('{urlFacebook}',895,525);{accionFacebook}");
                listaAtributosFacebook.Add("class", "servicio");
                listaAtributosFacebook.Add("id", "fb");

                listaRedesSociales.Add("Facebook", listaAtributosFacebook);

                #endregion

                #region Linkedin

                string urlLinkedin = $"http://www.linkedin.com/shareArticle?mini=true&url={linkPagina}&title={titulo}&summary={descripcion}";

                Dictionary<string, string> listaAtributosLinkedin = new Dictionary<string, string>();
                listaAtributosLinkedin.Add("onclick", $"MostrarPopUpCentrado('{urlLinkedin}',895,525);{accionLinkedIn}");

                listaRedesSociales.Add("Linkedin", listaAtributosLinkedin);

                #endregion

                #region Reddit

                string urlReddit = $"http://www.reddit.com/submit?url={linkPagina}&title={titulo}";

                Dictionary<string, string> listaAtributosReddit = new Dictionary<string, string>();
                listaAtributosReddit.Add("onclick", $"MostrarPopUpCentrado('{urlReddit}',895,525);{accionReddit}");

                listaRedesSociales.Add("Reddit", listaAtributosReddit);

                #endregion

                #region Blogger

                string urlBlogger = $"http://www.blogger.com/blog_this.pyra?t={descripcion}&u={linkPagina}&n={titulo}";

                Dictionary<string, string> listaAtributosBlogger = new Dictionary<string, string>();
                listaAtributosBlogger.Add("onclick", $"MostrarPopUpCentrado('{urlBlogger}',895,525);{accionBlogger}");

                listaRedesSociales.Add("Blogger", listaAtributosBlogger);

                #endregion

            }

            return listaRedesSociales;
        }

        #region Flujos

        private EstadoModel ObtenerEstadoDocumento()
        {
            if (Documento.Estado.HasValue)
            {
                UtilFlujos utilFlujos = new UtilFlujos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilFlujos>(), mLoggerFactory);
                return utilFlujos.ObtenerEstadoDeContenido(Documento.Estado.Value, IdentidadActual.Clave, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }

            return null;
        }

        private EstadoModel ObtenerEstadoUltimaVersionDocumento(Guid pDocumentoID)
        {			
            FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
            Guid estadoID = flujosCN.ObtenerEstadoIDDeContenido(pDocumentoID, TipoContenidoFlujo.Recurso);
            flujosCN.Dispose();
            if (estadoID != Guid.Empty)
            {
				UtilFlujos utilFlujos = new UtilFlujos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilFlujos>(), mLoggerFactory);
                return utilFlujos.ObtenerEstadoDeContenido(estadoID, IdentidadActual.Clave, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
			}

            return null;
		}

        public ActionResult RealizarTransicionEstado(Guid pTransicionID, string pComentario)
        {
            try
            {
                EstadoModel estadoDocumento = ObtenerEstadoDocumento();
                bool tienePermiso = estadoDocumento.Transiciones.Any(t => t.TransicionID.Equals(pTransicionID));
                // TODO FRAN: ¿Permisos creador?
                if (tienePermiso /*|| Documento.CreadorID.Equals(IdentidadActual.Clave)*/)
                {
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, Documento.ProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                    DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                    GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);

                    pComentario = pComentario ?? "";
                    // Cambiar estado en base de datos y añadir al historial
                    Guid estadoDestino = flujosCN.ObtenerEstadoDestinoTransicion(pTransicionID);
                    docCN.CambiarEstadoDocumento(Documento.Clave, estadoDestino);
                    flujosCN.GuardarHistorialTransicionDocumento(Documento.Clave, pTransicionID, pComentario, IdentidadActual.Clave);

                    // Cambiar estado en Virtuoso                    
                    facetadoCN.ModificarEstadoDeContenido(Documento.ProyectoID, estadoDocumento.EstadoID, estadoDestino, Documento.Clave);

                    // Enviar correo de aviso a lectores y editores de los estados de origen y destino
                    // TODO FRAN: ¿Enviar correo al creador?					
                    string urlDocumento = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false);
                    gestorNotificaciones.EnviarCorreoAvisoCambioDeEstado(pTransicionID, Documento.ProyectoID, pComentario, urlDocumento, Documento.Titulo, IdentidadActual.Nombre());
                    notificacionCN.ActualizarNotificacion(mAvailableServices);
                    // Actualizar GnossLive
                    ActualizarGnossLiveTransicionEstado(estadoDocumento.EstadoID, estadoDestino);
                    // Invalidar ficha del recurso
                    ControladorDocumentacion.BorrarCacheControlFichaRecursos(Documento.UltimaVersionID);

                    notificacionCN.Dispose();
                    facetadoCN.Dispose();
                    flujosCN.Dispose();
                    docCN.Dispose();
                }

                return GnossResultOK(UtilIdiomas.GetText("FLUJOS", "TRANSICIONREALIZADA"));
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "TRANSICIONERROR"));
            }
        }

        #endregion

        #region Cargas

        private Guid? CargarBaseRecursos()
        {
            mGestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);

            if (ProyectoOrigenBusquedaID != Guid.Empty)
            {
                docCL.ObtenerBaseRecursosProyecto(mGestorDocumental.DataWrapperDocumentacion, ProyectoOrigenBusquedaID, Guid.Empty, UsuarioActual.UsuarioID);
            }
            else if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
            {
                if (UsuarioVeRecursoOtroPerfilID != Guid.Empty)
                {
                    docCN.ObtenerBaseRecursosUsuario(mGestorDocumental.DataWrapperDocumentacion, UsuarioVeRecursoOtroPerfilID);
                }
                else if (!EsIdentidadBROrg)
                {
                    docCN.ObtenerBaseRecursosUsuario(mGestorDocumental.DataWrapperDocumentacion, mControladorBase.UsuarioActual.UsuarioID);
                }
                else
                {
                    docCL.ObtenerBaseRecursosOrganizacion(mGestorDocumental.DataWrapperDocumentacion, (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacionBROrg.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), ProyectoSeleccionado.Clave);
                }
            }
            else
            {
                docCL.ObtenerBaseRecursosProyecto(mGestorDocumental.DataWrapperDocumentacion, ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, UsuarioActual.UsuarioID);
            }

            docCN.Dispose();
            docCL.Dispose();

            if (mGestorDocumental.DataWrapperDocumentacion.ListaBaseRecursos.Count > 0)
            {
                return mGestorDocumental.BaseRecursosIDActual;
            }

            return null;
        }

        /// <summary>
        /// Realiza la carga inicial de los gestores para que se cargue el recurso
        /// </summary>
        private void CargarInicial()
        {
            if (CargarBaseRecursos().HasValue)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                if (DocumentoVersionado)
                {
                    Guid documentoUltimaVersionID = DocumentoVersionID != Guid.Empty ? DocumentoVersionID : mDocumentoUltimaVersionID;
                    docCN.ObtenerDocumentoPorIDCargarTotal(documentoUltimaVersionID, mGestorDocumental.DataWrapperDocumentacion, true, true, mGestorDocumental.BaseRecursosIDActual);

                    // Si es un recurso anterior al versionado de los recursos se mostrará su unico documento
                    if (mDocumentoOriginalVersionID != Guid.Empty)
                    {
                        docCN.ObtenerDocumentoPorIDCargarTotal(mDocumentoOriginalVersionID, mGestorDocumental.DataWrapperDocumentacion, true, true, mGestorDocumental.BaseRecursosIDActual);
                        // Cargar todos los documentos de las verisones que no estén cargados aun.
                        foreach (Guid docID in mGestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.Where(doc => !mGestorDocumental.ListaDocumentos.ContainsKey(doc.DocumentoID)).Select(doc => doc.DocumentoID).ToList())
                        {
                            docCN.ObtenerDocumentoPorIDCargarTotal(docID, mGestorDocumental.DataWrapperDocumentacion, true, true, mGestorDocumental.BaseRecursosIDActual);
                        }
                    }
                }
                else
                {
                    docCN.ObtenerDocumentoPorIDCargarTotal(DocumentoID, mGestorDocumental.DataWrapperDocumentacion, true, true, mGestorDocumental.BaseRecursosIDActual);
                }

                //Cargo el gestor de tesauro, lo necesitaremos para la comprobación de seguirdad.
                CargarTesauro();
                docCN.Dispose();
            }

            mGestorDocumental.CargarDocumentos(false);
        }
        private void CargaInicialDocumento()
        {
            Guid docId = Guid.Empty;
            try
            {
                if (DocumentoVersionado)
                {
                    docId = DocumentoVersionID != Guid.Empty ? DocumentoVersionID : mDocumentoUltimaVersionID;
                }
                else
                {
                    docId = DocumentoID;
                }

                if (!(GestorDocumental.ListaDocumentos[docId] is DocumentoWeb))
                {
                    mDocumento = new DocumentoWeb(GestorDocumental.ListaDocumentos[docId].FilaDocumento, GestorDocumental);
                }
                else
                {
                    mDocumento = (DocumentoWeb)GestorDocumental.ListaDocumentos[docId];
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, $"Se ha producido un error al cargar los datos del documento actual\nID: {docId}");
            }
        }

        /// <summary>
        /// Realiza la carga inicial de los gestores para que se cargue el recurso
        /// </summary>
        private void CargarTesauro()
        {
            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                if (ProyectoOrigenBusquedaID != Guid.Empty)
                {
                    GestorDocumental.GestorTesauro = new GestionTesauro(new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory).ObtenerTesauroDeProyecto(ProyectoOrigenBusquedaID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                }
                else
                {
                    GestorDocumental.GestorTesauro = new GestionTesauro(new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory).ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                }
            }
            else
            {
                TesauroCN tesCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);

                if (UsuarioVeRecursoOtroPerfilID != Guid.Empty)
                {
                    GestorDocumental.GestorTesauro = new GestionTesauro(tesCN.ObtenerTesauroUsuario(UsuarioVeRecursoOtroPerfilID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                }
                else if (EsIdentidadBROrg)
                {
                    GestorDocumental.GestorTesauro = new GestionTesauro(tesCN.ObtenerTesauroOrganizacion(IdentidadOrganizacionBROrg.OrganizacionID.Value), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                }
                else
                {
                    GestorDocumental.GestorTesauro = new GestionTesauro(tesCN.ObtenerTesauroUsuario(mControladorBase.UsuarioActual.UsuarioID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                }

                tesCN.Dispose();
            }
        }

        /// <summary>
        /// Genarador plantilla para la ficha.
        /// </summary>
        private SemCmsController GenPlantillasOWL
        {
            get
            {
                if (Documento != null && Documento.TipoDocumentacion == TiposDocumentacion.Semantico && mGenPlantillasOWL == null)
                {
                    CargarSemCmsController(true);
                }
                return mGenPlantillasOWL;
            }
        }

        /// <summary>
        /// Carga el controlador de SEM CMS.
        /// </summary>
        public void CargarSemCmsController(bool pUsarAfinidad = false)
        {
            try
            {
                mGenPlantillasOWL = ObtenerSemCmsController(Documento).ObtenerControladorSemCMS(Documento, ProyectoSeleccionado, IdentidadActual, BaseURLFormulariosSem, UtilIdiomas, BaseURL, BaseURLIdioma, BaseURLContent, BaseURLStatic, UrlIntragnoss, VistaPersonalizada, ParametroProyecto, Request.Query["paramsemcms"], pUsarAfinidad);
                paginaModel.SemanticFrom = mGenPlantillasOWL.SemanticResourceModel;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex.ToString());
                throw;
            }
        }


        public KeyValuePair<Guid?, bool> ObtenerBaseRecursosPersonalRecurso()
        {
            Guid brID = Guid.Empty;
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            bool org = false;

            if (RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true"))
            {
                brID = docCN.ObtenerBaseRecursosIDOrganizacion(IdentidadActual.OrganizacionPerfil.Clave);
                org = true;
            }
            else if (UsuarioVeRecursoOtroPerfilID != Guid.Empty)
            {
                brID = docCN.ObtenerBaseRecursosIDUsuario(UsuarioVeRecursoOtroPerfilID);
            }
            else
            {
                brID = docCN.ObtenerBaseRecursosIDUsuario(mControladorBase.UsuarioActual.UsuarioID);
            }

            docCN.Dispose();
            return new KeyValuePair<Guid?, bool>(brID, org);
        }

        #endregion

        protected override void CargarEnlacesMultiIdioma()
        {
            List<string> listaIdiomasDisponible = null;
            if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma) && GenPlantillasOWL != null)
            {
                string nombreProp = ParametroProyecto[ParametroAD.PropiedadContenidoMultiIdioma];
                if (nombreProp.StartsWith("dce:"))
                {
                    nombreProp = nombreProp.Replace("dce:", "dc:");
                }
                if (nombreProp.Contains(":") && GenPlantillasOWL.Ontologia.NamespacesDefinidosInv.ContainsKey(nombreProp.Split(':')[0]))
                {
                    nombreProp = GenPlantillasOWL.Ontologia.NamespacesDefinidosInv[nombreProp.Split(':')[0]] + nombreProp.Substring(nombreProp.IndexOf(":") + 1);
                }
                Propiedad propIdioma = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(nombreProp, null, GenPlantillasOWL.Entidades);

                listaIdiomasDisponible = new List<string>();
                if (propIdioma != null)
                {
                    foreach (string idioma in propIdioma.ValoresUnificados.Keys)
                    {
                        listaIdiomasDisponible.Add(idioma.ToLower());
                    }
                }
            }

            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
            Dictionary<string, string> listaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
            Dictionary<string, KeyValuePair<bool, string>> listaEnlacesMultiIdioma = new Dictionary<string, KeyValuePair<bool, string>>();
            foreach (string idioma in listaIdiomas.Keys)
            {
                if (!ParametrosGeneralesRow.IdiomasDisponibles && idioma != IdiomaPorDefecto)
                {
                    continue;
                }
                Guid docID = Guid.Empty;

                if (Guid.TryParse(RequestParams("docID"), out docID))
                {
                    bool disponible = (listaIdiomasDisponible == null) || listaIdiomasDisponible.Contains(idioma.ToLower());
                    UtilIdiomas utilIdiomasActual = new Recursos.UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<Recursos.UtilIdiomas>(), mLoggerFactory);

                    if (Documento != null)
                    {
                        string rutaActual = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, utilIdiomasActual, ProyectoSeleccionado.NombreCorto, UrlPerfil, Documento, false).Replace(Documento.Clave.ToString(), Documento.VersionOriginalID.ToString());
                        listaEnlacesMultiIdioma.Add(idioma, new KeyValuePair<bool, string>(disponible, rutaActual));
                    }
                }
            }
            ListaEnlacesMultiIdioma = listaEnlacesMultiIdioma;
        }

        protected override void CargarTituloPagina()
        {
            if (Documento == null)
            {
                return;
            }

            if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && !string.IsNullOrEmpty(GenPlantillasOWL.SemanticResourceModel.PageTitle))
            {
                TituloPagina = UtilCadenas.ObtenerTextoDeIdioma(GenPlantillasOWL.SemanticResourceModel.PageTitle, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }
            else
            {
                TituloPagina = UtilCadenas.ObtenerTextoDeIdioma(Documento.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }

            base.CargarTituloPagina();
        }

        private void CargarAutoresEditoresLectores(ResourceModel pFichaRecurso)
        {

            CargarAutores(pFichaRecurso);

            bool mostrarEditores = (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarEditoresDoc);
            ControladorDocumentoMVC.CargarEditoresLectores(paginaModel.Resource, GestorDocumental, Documento, IdentidadActual, mControladorBase.UsuarioActual.EsIdentidadInvitada, mostrarEditores, UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado, ProyectoVirtual);
        }

        private void CargarAutores(ResourceModel pFichaRecurso)
        {
            if (GenPlantillasOWL == null || !GenPlantillasOWL.Ontologia.ConfiguracionPlantilla.OcultarAutoresDoc)
            {
                if (Documento.Autor != null)
                {
                    string[] autores = Documento.Autor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    string nombreSem = PestanyaRecurso.Value;

                    if (autores.Length > 0)
                    {
                        pFichaRecurso.Authors = new Dictionary<string, string>();

                        foreach (string autor in autores)
                        {
                            string urlBusquedaAutor = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, ProyectoVirtual.NombreCorto, UrlPerfil, EsIdentidadBROrg, nombreSem) + "/autor/" + autor.Trim();

                            if (!pFichaRecurso.Authors.ContainsKey(autor))
                            {
                                pFichaRecurso.Authors.Add(autor, urlBusquedaAutor);
                            }
                        }
                    }
                }
            }
        }

        private bool ComprobarPermisosLecturaRecurso(Guid pDocumentoID)
        {
            bool resultado = false;

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {

                using (DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory))
                {
                    List<Guid> listaProyectos = new List<Guid>();
                    listaProyectos.Add(ProyectoSeleccionado.Clave);
                    resultado = documentacionCN.TieneUsuarioAccesoADocumentoEnProyecto(listaProyectos, pDocumentoID, IdentidadActual.PerfilID, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, false, !mControladorBase.UsuarioActual.EsIdentidadInvitada);
                }
            }
            else
            {
                resultado = ComprobarMyGnoss(resultado);
            }
            return resultado;
        }

        private string ModificarVideoSiNoHayCookiesYoutube(string pEnlace)
        {
            if ((Request.Cookies["redes sociales"] == null || Request.Cookies["redes sociales"].Equals("no")) && pEnlace.Contains("www.youtube.com/embed"))
            {
                return pEnlace.Replace("www.youtube.com/embed", "www.youtube-nocookie.com/embed");
            }

            return pEnlace;
        }

        private bool ComprobarMyGnoss(bool resultado)
        {
            if (GestorDocumental.DataWrapperDocumentacion != null)
            {
                if (Documento.FilaDocumentoWebVinBR.BaseRecursosID == Documento.GestorDocumental.BaseRecursosIDActual && !Documento.FilaDocumentoWebVinBR.Eliminado)
                {
                    if (UsuarioVeRecursoOtroPerfilID != Guid.Empty)
                    {//Usuario está viendo el perfil de otro. Hay que comprobar que el recurso está categorizado publico en la BR de ese usuario:
                        Guid catPublicaUsuario = Documento.GestorDocumental.GestorTesauro.CategoriaPublicaID;

                        foreach (CategoriaTesauro categoria in Documento.CategoriasTesauro.Values)
                        {
                            if (categoria.PadreNivelRaiz.Clave == catPublicaUsuario)
                            {
                                resultado = true;
                                break;
                            }
                        }
                    }
                    else if (EsIdentidadBROrg)
                    {

                        List<AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion> filasBROrg = Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Where(item => item.BaseRecursosID.Equals(Documento.GestorDocumental.BaseRecursosIDActual)).ToList();

                        if (filasBROrg.Count > 0)
                        {
                            Guid orgID = (filasBROrg[0]).OrganizacionID;

                            if (mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion, orgID) || mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, orgID))
                            {
                                resultado = true;
                            }
                        }
                    }
                    else if (Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Any(item => item.BaseRecursosID.Equals(Documento.GestorDocumental.BaseRecursosIDActual) && item.UsuarioID.Equals(mControladorBase.UsuarioActual.UsuarioID)))
                    {
                        resultado = true;
                    }
                }
            }

            return resultado;
        }

        private void ActualizarGnossLiveTransicionEstado(Guid pEstadoOrigen, Guid pEstadoDestino)
        {
            FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
            int tipo;
            switch (Documento.TipoDocumentacion)
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

            if (Documento.ProyectoID == ProyectoAD.MetaProyecto)
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

            bool privacidadCambiada = flujosCN.ComprobarEstadoEsPublico(pEstadoOrigen) != flujosCN.ComprobarEstadoEsPublico(pEstadoDestino);

            if (privacidadCambiada)
            {
                infoExtra += Constantes.PRIVACIDAD_CAMBIADA;

                ControladorDocumentacion.ActualizarGnossLive(Documento.ProyectoID, Documento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Media, null, mAvailableServices);
                ControladorDocumentacion.ActualizarGnossLive(Documento.ProyectoID, Documento.Clave, AccionLive.Editado, tipo, PrioridadLive.Media, infoExtra, mAvailableServices);
            }
            else
            {
                ControladorDocumentacion.ActualizarGnossLive(Documento.ProyectoID, Documento.Clave, AccionLive.Agregado, tipo, PrioridadLive.Alta, infoExtra, mAvailableServices);
            }
        }

        private List<AD.EntityModel.Models.Documentacion.VersionDocumento> CalcularVersionesPadre(List<AD.EntityModel.Models.Documentacion.VersionDocumento> listaVersionesDocumento)
        {
            return listaVersionesDocumento.Where(x => x.MejoraID == null || x.EstadoVersion == (short)EstadoVersion.Vigente || x.EstadoVersion == (short)EstadoVersion.Pendiente).ToList();
        }

        private List<VersionViewModel> CalcularSubversiones(Guid? mejoraID, List<AD.EntityModel.Models.Documentacion.VersionDocumento> listaVersionesDocumento, Dictionary<Guid, ResourceModel> recursos)
        {
            List<VersionViewModel> subversiones = new List<VersionViewModel>();
            foreach (AD.EntityModel.Models.Documentacion.VersionDocumento versionDocumento in listaVersionesDocumento.Where(x => x.MejoraID == mejoraID))
            {
                VersionViewModel version = new VersionViewModel
                {
                    Number = versionDocumento.Version,
                    Title = recursos[versionDocumento.DocumentoID].Title,
                    Publisher = recursos[versionDocumento.DocumentoID].Publisher.NamePerson,
                    PublishDate = recursos[versionDocumento.DocumentoID].PublishDate,
                    VersionId = versionDocumento.DocumentoID,
                    StatusId = versionDocumento.EstadoID,
                    IsImprovement = versionDocumento.EsMejora,
                    VersionStatus = (EstadoVersion)versionDocumento.EstadoVersion,
                    Url = recursos[versionDocumento.DocumentoID].VersionCardLink,
                    UrlPreview = recursos[versionDocumento.DocumentoID].UrlPreview,
                    UrlLoadActionRestoreVersion = recursos[versionDocumento.DocumentoID].ListActions.UrlLoadActionRestoreVersion,
                    UrlLoadActionDeleteVersion = recursos[versionDocumento.DocumentoID].ListActions.UrlLoadActionDeleteVersion,
                    IsLastVersion = versionDocumento.EstadoVersion == (short)EstadoVersion.Vigente,
                    IsSemantic = recursos[versionDocumento.DocumentoID].TypeDocument == DocumentType.Semantico,
                    IsServerFile = recursos[versionDocumento.DocumentoID].TypeDocument == DocumentType.FicheroServidor
                };

                subversiones.Add(version);
            }

            return subversiones;
        }

        #region Propiedades

        /// <summary>
        /// Devuelve el documento actual.
        /// </summary>
        private Guid DocumentoID
        {
            get
            {
                return new Guid(RequestParams("docID"));
            }
        }
        private Guid DocumentoVersionID
        {
            get
            {
                Guid versionDocID;
                Guid.TryParse(RequestParams("versionDocID"), out versionDocID);
                return versionDocID;
            }
            set
            {

            }
        }

        /// <summary>
        /// Devuelve el documento actual.
        /// </summary>
        public DocumentoWeb Documento
        {
            get
            {
                if (mDocumento == null)
                {
                    Guid docID = Guid.Empty;

                    if (Guid.TryParse(RequestParams("docID"), out docID))
                    {
                        if (GestorDocumental.ListaDocumentos.ContainsKey(DocumentoID))
                        {

                            if (!(GestorDocumental.ListaDocumentos[DocumentoID] is DocumentoWeb))
                            {
                                mDocumento = new DocumentoWeb(GestorDocumental.ListaDocumentos[DocumentoID].FilaDocumento, GestorDocumental);
                            }
                            else
                            {
                                mDocumento = (DocumentoWeb)GestorDocumental.ListaDocumentos[DocumentoID];
                            }

                            if (!mDocumento.FilaDocumento.UltimaVersion && mDocumento.UltimaVersion == null)
                            {
                                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                                mDocumento.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerVersionesDocumentoPorID(DocumentoID));
                                docCN.Dispose();
                                mDocumento.GestorDocumental.CargarDocumentosWeb();
                            }
                        }
                    }
                }

                return mDocumento;
            }
        }

        private bool DocumentoVersionado
        {
            get
            {
                return mDocumentoVersionado;
            }
        }

        /// <summary>
        /// Devuelve el gestor de documentos.
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
        /// Indica la identidad de la BR actual es la de una organización.
        /// </summary>
        public bool EsIdentidadBROrg
        {
            get
            {
                return !string.IsNullOrEmpty(RequestParams("organizacion"));
            }
        }

        /// <summary>
        /// Devuelve la identidad de organización si estamos en la BR de Org. NULL si no.
        /// </summary>
        public Identidad IdentidadOrganizacionBROrg
        {
            get
            {
                if (EsIdentidadBROrg)
                {
                    return IdentidadActual.IdentidadOrganizacion;
                }

                return null;
            }
        }

        /// <summary>
        /// Obtiene la pestaña seleccionada
        /// </summary>
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

        /// <summary>
        /// Nombre del proyecto para la edición del recurso.
        /// </summary>
        public string NombreProyEdicionRecurso
        {
            get
            {
                string nomProyEditar = "";

                if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                {
                    nomProyEditar = NombreProyBusquedaOActual;
                }

                if (ProyectoOrigenBusquedaID != Guid.Empty && Documento.FilaDocumento.ProyectoID == ProyectoSeleccionado.Clave)
                {
                    nomProyEditar = ProyectoSeleccionado.NombreCorto;
                }

                return nomProyEditar;
            }
        }

        /// <summary>
        /// Obtenemos el id de publicador persona
        /// </summary>
        public Guid PersonaPublicadorID
        {
            get
            {
                Guid perpublicadorID = Guid.Empty;

                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                {
                    if (!EsIdentidadBROrg)
                    {
                        perpublicadorID = mControladorBase.UsuarioActual.PersonaID;
                    }
                }
                else
                {
                    if (Documento.ProyectoID != ProyectoAD.MetaProyecto)
                    {
                        perpublicadorID = Guid.Empty;
                    }
                    else
                    {
                        AD.EntityModel.Models.PersonaDS.Persona filaPersona = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory).ObtenerPersonaPorIdentidadCargaLigera(Documento.CreadorID);
                        if (filaPersona != null)
                        {
                            perpublicadorID = filaPersona.PersonaID;
                        }
                    }
                }

                if (Documento.TipoEntidadVinculada != TipoEntidadVinculadaDocumento.Web)
                {
                    perpublicadorID = Guid.Empty;
                }
                return perpublicadorID;
            }
        }

        /// <summary>
        /// Obtenemos el id de publicador Organizacion
        /// </summary>
        public Guid OrganizacionPublicadorID
        {
            get
            {
                Guid orgPublicadorID = Guid.Empty;

                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                {
                    if (EsIdentidadBROrg)
                    {
                        orgPublicadorID = (Guid)UtilReflection.GetValueReflection(IdentidadOrganizacionBROrg.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID");
                    }
                }

                if (Documento.TipoEntidadVinculada != TipoEntidadVinculadaDocumento.Web)
                {
                    orgPublicadorID = Guid.Empty;
                }
                return orgPublicadorID;
            }
        }

        private bool MostrarControlAgregarComentario
        {
            get
            {
                return Documento.PermiteComentarios && Documento.FilaDocumento.UltimaVersion && ProyectoOrigenBusquedaID == Guid.Empty;
            }
        }

        private bool ComunidadPermiteComentarios
        {
            get
            {
                return ParametrosGeneralesVirtualRow.ComentariosDisponibles;
            }
        }

        private Identidad IdentidadRestaurar
        {
            get
            {
                Identidad identidadRestaurar = IdentidadActual;

                if (EsIdentidadBROrg)
                {
                    identidadRestaurar = IdentidadOrganizacionBROrg;
                }
                else if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto && IdentidadActual.Tipo != TiposIdentidad.Personal)
                {
                    identidadRestaurar = IdentidadActual.IdentidadPersonalMyGNOSS;
                }
                return identidadRestaurar;
            }
        }

        /// <summary>
        /// Indica si la vista que se va a usar para el controlador es personalizada o no.
        /// </summary>
        private bool VistaPersonalizada
        {
            get
            {
                if (!string.IsNullOrEmpty((string)ViewBag.Personalizacion) && Comunidad != null)
                {
                    return ControladorDocumentoMVC.ComprobarVistaPersonalizadaDocumento(Documento, Comunidad, (string)ViewBag.ControllerName);
                }

                return false;
            }
        }


        /// <summary>
        /// Obtiene los niveles de certificación de un proyecto
        /// </summary>
        protected DataWrapperProyecto NivelesCertificacionProyecto
        {
            get
            {
                if ((!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto)) && (ParametrosGeneralesRow.PermitirCertificacionRec))
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    return proyCL.ObtenerNivelesCertificacionRecursosProyecto(ProyectoSeleccionado.Clave);
                }
                return null;
            }
        }

        /// <summary>
        /// ID del usuario que está viendo el recurso publicado en el espacio personal de otra persona.
        /// </summary>
        public Guid UsuarioVeRecursoOtroPerfilID
        {
            get
            {
                if (mUsuarioVeRecursoOtroPerfilID == null)
                {
                    mUsuarioVeRecursoOtroPerfilID = Guid.Empty;

                    if (RequestParams("VerRecursosPerfil") != null && RequestParams("VerRecursosPerfil").Equals("true"))
                    {
                        string nombreUsu = RequestParams("nombreCortoPerfil");

                        if (!string.IsNullOrEmpty(nombreUsu))
                        {
                            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                            Guid? usuarioID = usuCN.ObtenerUsuarioIDPorNombreCorto(nombreUsu);
                            usuCN.Dispose();

                            if (usuarioID.HasValue)
                            {
                                mUsuarioVeRecursoOtroPerfilID = usuarioID.Value;
                            }
                        }
                    }
                }

                return mUsuarioVeRecursoOtroPerfilID.Value;
            }
        }

        private List<string> ListaDominiosSinPalco
        {
            get
            {
                if (mListaDominiosSinPalco == null)
                {
                    mListaDominiosSinPalco = new List<string>();
                    List<ParametroAplicacion> filas = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("DominiosSinPalco")).ToList();
                    if (filas.Count > 0)
                    {
                        string[] dominiosSinPalco = filas.First().Valor.ToString().Split('|');
                        foreach (string dominioSinPalco in dominiosSinPalco)
                        {
                            string dominio = dominioSinPalco.Replace("http://", "").Replace("https://", "").Replace("www.", "");
                            if (dominio.IndexOf('/') > -1)
                            {
                                dominio = dominio.Substring(0, dominio.IndexOf('/'));
                            }
                            if (!mListaDominiosSinPalco.Contains(dominio) && !string.IsNullOrEmpty(dominio))
                            {
                                mListaDominiosSinPalco.Add(dominio);
                            }
                        }
                    }
                }
                return mListaDominiosSinPalco;
            }
        }

        #endregion
    }
}
