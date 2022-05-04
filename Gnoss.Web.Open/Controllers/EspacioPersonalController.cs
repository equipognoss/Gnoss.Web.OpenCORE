using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using static Es.Riam.Gnoss.Web.Controles.ControladorBase;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class EspacioPersonalController : ControllerBaseWeb
    {
        #region Constantes
        private const string LETRAS_CON_ACENTOS = "âáàäêéèëîíìïôóòöûúüùñÂÁÀÄÊÉÈËÎÍÌÏÔÓÒÖÛÚÙÜÑçÇ";
        private const string LETRAS_SIN_ACENTOS = "aaaaeeeeiiiioooouuuunAAAAEEEEIIIIOOOOUUUUNcC";
        private const string SIGNOS_ELIMINAR_SEARCH = ",.;¿?!¡:";
        #endregion

        #region Miembros

        /// <summary>
        /// Modelo del espacio personal.
        /// </summary>
        private PersonalSpaceModel mPerSpaceModel;

        /// <summary>
        /// Grafo de búsqueda.
        /// </summary>
        private string mGrafo;

        /// <summary>
        /// Identidad de la organización en el caso de que haya.
        /// </summary>
        private Identidad mIdentidadOrganizacion;

        /// <summary>
        /// Contiene la URL del servicio web de documentación.
        /// </summary>
        private string mUrlServicioWebDocumentacion;

        #endregion

        public EspacioPersonalController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Propiedades

        /// <summary>
        /// Identidad de la organización en el caso de que haya.
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

        #endregion

        #region Métodos

        /// <summary>
        /// Método inicial.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index()
        {
            mPerSpaceModel = new PersonalSpaceModel();

            ActionResult redireccion = ComprobarRedirecciones();

            if (redireccion != null)
            {
                return redireccion;
            }

            ObtenerGrafoBusqueda();

            CargarComponerPanelSidebar();
            
            CargarTitulosYMenus();
            redireccion = ObtenerResultados();

            if (redireccion != null)
            {
                return redireccion;
            }
            // Permitir añadir recursos desde EspacioPersonal
            ViewBag.Comunidad.Permissions.CreateResource = true;
            ViewBag.EsPaginaEspacioPersonal = true;

            return View(mPerSpaceModel);
        }

        /// <summary>
        /// Acción de editar categorías del recurso.
        /// </summary>
        /// <param name="pModel">Modelo de la acción</param>
        /// <returns>Resultado de la acción</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult EditarCategorias(EditCategoriesPersonalSpaceModel pModel)
        {
            try
            {
                ActionResult redireccion = ComprobarRedirecciones();

                if (redireccion != null)
                {
                    return redireccion;
                }

                GestionTesauro gestorTes = CargarGestorTesauroBRActual();

                ThesaurusEditorModel modeloTesauro = new ThesaurusEditorModel();
                modeloTesauro.ThesaurusCategories = CargarTesauroPorGestorTesauro(gestorTes);
                List<Guid> listaCatModelSeleccionadas = new List<Guid>();

                string[] recursos = pModel.SelectedResources.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (recursos.Length == 1)
                {
                    DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    docCN.ObtenerDocumentoWebPorIDWEB(new Guid(recursos[0]), dataWrapperDocumentacion);
                    

                    Guid baseRecursosID = Guid.Empty;
                    if (RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true"))
                    {
                        baseRecursosID = docCN.ObtenerBaseRecursosIDOrganizacion(IdentidadActual.OrganizacionPerfil.Clave);
                    }
                    else
                    {
                        baseRecursosID = docCN.ObtenerBaseRecursosIDUsuario(mControladorBase.UsuarioActual.UsuarioID);
                    }

                    docCN.Dispose();

                    foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocAgCatTes in dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(docWebAg=>docWebAg.BaseRecursosID.Equals(baseRecursosID)))
                    {
                        CategoriaTesauro categoria = gestorTes.ListaCategoriasTesauro[filaDocAgCatTes.CategoriaTesauroID];

                        if (mEntityContext.Entry(categoria.FilaCategoria).State != EntityState.Deleted)
                        {
                            CategoryModel categoriaTesauro = new CategoryModel();
                            categoriaTesauro.Key = categoria.Clave;
                            categoriaTesauro.Name = categoria.FilaCategoria.Nombre;
                            categoriaTesauro.LanguageName = categoria.FilaCategoria.Nombre;

                            listaCatModelSeleccionadas.Add(categoria.Clave);
                        }
                    }
                }
                else if (recursos.Length == 0)
                {
                    return GnossResultERROR();
                }

                modeloTesauro.SelectedCategories = listaCatModelSeleccionadas;

                // Nuevo Front return GnossResultHtml("EditorTesauro/_EditorTesauro", modeloTesauro);
                return GnossResultHtml("EditorTesauro/_modal-views/_EditorTesauroInModalView", modeloTesauro);
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.Message);
            }

            return GnossResultERROR();
        }

        /// <summary>
        /// Acción de aceptar las categorías del recurso.
        /// </summary>
        /// <param name="pModel">Modelo de la acción</param>
        /// <returns>Resultado de la acción</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult AceptarCategorias(EditCategoriesPersonalSpaceModel pModel)
        {
            try
            {
                ActionResult redireccion = ComprobarRedirecciones();

                if (redireccion != null)
                {
                    return redireccion;
                }

                string[] recursos = pModel.SelectedResources.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] categorias = pModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (recursos.Length > 0 && categorias.Length > 0)
                {
                    List<CategoriaTesauro> listaCategoriasEliminadas = new List<CategoriaTesauro>();
                    List<CategoriaTesauro> listaCategoriasNuevas = new List<CategoriaTesauro>();
                    List<Guid> listaCatSeleccionadas = new List<Guid>();

                    foreach (string categoriaID in categorias)
                    {
                        if (!listaCatSeleccionadas.Contains(new Guid(categoriaID)))
                        {
                            listaCatSeleccionadas.Add(new Guid(categoriaID));
                        }
                    }

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

                    if (RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true"))
                    {
                        docCN.ObtenerBaseRecursosOrganizacion(dataWrapperDocumentacion, IdentidadActual.OrganizacionPerfil.Clave);
                    }
                    else
                    {
                        docCN.ObtenerBaseRecursosUsuario(dataWrapperDocumentacion, mControladorBase.UsuarioActual.UsuarioID);
                    }

                    GestorDocumental gestorDoc = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext);
                    gestorDoc.GestorTesauro = CargarGestorTesauroBRActual();

                    foreach (string documentoTextoID in recursos)
                    {
                        Guid documentoID = new Guid(documentoTextoID);
                        docCN.ObtenerDocumentoPorIDCargarTotal(documentoID, dataWrapperDocumentacion, true, true, null);
                        gestorDoc.CargarDocumentos(false);

                        if ((gestorDoc.ListaDocumentos.ContainsKey(documentoID)) && (gestorDoc.ListaDocumentos[documentoID].TipoEntidadVinculada == TipoEntidadVinculadaDocumento.Web))
                        {
                            DocumentoWeb documentoWeb = new DocumentoWeb(gestorDoc.ListaDocumentos[documentoID].FilaDocumento, gestorDoc, mLoggingService);

                            if (listaCatSeleccionadas.Count > 0)
                            {
                                Guid identidadCreador = mControladorBase.UsuarioActual.IdentidadID;

                                if (IdentidadOrganizacion != null)
                                {
                                    identidadCreador = IdentidadOrganizacion.Clave;
                                }

                                foreach (Guid categoriaID in listaCatSeleccionadas)
                                {
                                    if (!documentoWeb.Categorias.ContainsKey(categoriaID))
                                    {
                                        CategoriaTesauro categoria = gestorDoc.GestorTesauro.ListaCategoriasTesauro[categoriaID];
                                        listaCategoriasNuevas.Add(categoria);
                                    }
                                }

                                foreach (CategoriaTesauro categoria in documentoWeb.Categorias.Values)
                                {
                                    if (!listaCatSeleccionadas.Contains(categoria.Clave))
                                    {
                                        listaCategoriasEliminadas.Add(categoria);
                                    }
                                }

                                gestorDoc.VincularDocumentoACategorias(listaCategoriasNuevas, documentoWeb, UsuarioActual.IdentidadID, UsuarioActual.ProyectoID);
                                gestorDoc.DesvincularDocumentoDeCategorias(documentoWeb, listaCategoriasEliminadas, UsuarioActual.IdentidadID, UsuarioActual.ProyectoID);
                                ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(documentoWeb, IdentidadActual, false);
                                ControladorDocumentacion.AgregarRecursoModeloBaseSimple(documentoID, ProyectoSeleccionado.Clave, (short)documentoWeb.TipoDocumentacion, PrioridadBase.Alta);
                            }
                            else
                            {
                                throw new Exception(UtilIdiomas.GetText("CONTROLESDOCUMENTACION", "ERRORSELECCIONCAT"));
                            }
                        }
                    }

                    docCN.Dispose();

                    mEntityContext.SaveChanges();

                    #region Actualizar cola GnossLIVE y BASE

                    string tripletas = "";

                    foreach (string documentoTextoID in recursos)
                    {
                        Guid documentoID = new Guid(documentoTextoID);
                        string infoExtra = null;

                        if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto)
                        {
                            infoExtra = IdentidadActual.PerfilID.ToString();
                        }

                        ControladorDocumentacion.ActualizarGnossLive(mControladorBase.UsuarioActual.ProyectoID, documentoID, AccionLive.Editado, (int)TipoLive.Recurso, false, "live", PrioridadLive.Alta, infoExtra);
                        ControladorDocumentacion.BorrarCacheControlFichaRecursos(documentoID);

                        string sujeto = "<http://gnoss/" + documentoID.ToString().ToUpper() + "> ";
                        string predicado = "<http://www.w3.org/2004/02/skos/core#ConceptID> ";
                        foreach (Guid catID in listaCatSeleccionadas)
                        {
                            string objeto = "<http://gnoss/" + catID.ToString().ToUpper() + "> .";
                            tripletas += FacetadoAD.GenerarTripleta(sujeto, predicado, objeto);
                        }
                    }

                    try
                    {
                        FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, IdentidadActual.PerfilID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        facCN.InsertaTripletas(IdentidadActual.PerfilID.ToString(), tripletas, (short)PrioridadBase.Alta, false);
                        facCN.Dispose();
                    }
                    catch(Exception ex)
                    {
                        GuardarLogErrorAJAX("Error al guardar directamente en virtuoso: " + ex.ToString());
                    }

                    #endregion

                    gestorDoc.Dispose();
                }

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.Message);
            }

            return GnossResultERROR();
        }

        /// <summary>
        /// Acción de eliminar recursos seleccionados.
        /// </summary>
        /// <param name="pModel">Modelo de la acción</param>
        /// <returns>Resultado de la acción</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult EliminarRecursos(EditCategoriesPersonalSpaceModel pModel)
        {
            try
            {
                ActionResult redireccion = ComprobarRedirecciones();

                if (redireccion != null)
                {
                    return redireccion;
                }

                string[] recursos = pModel.SelectedResources.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (recursos.Length > 0)
                {
                    mControladorBase.UsuarioActual.UsarMasterParaLectura = true;

                    List<Guid> recursosID = new List<Guid>();

                    foreach (string recurso in recursos)
                    {
                        if (recurso != "")
                        {
                            recursosID.Add(new Guid(recurso));
                        }
                    }

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

                    if (RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true"))
                    {
                        docCN.ObtenerBaseRecursosOrganizacion(dataWrapperDocumentacion, IdentidadActual.OrganizacionPerfil.Clave);
                    }
                    else
                    {
                        docCN.ObtenerBaseRecursosUsuario(dataWrapperDocumentacion, mControladorBase.UsuarioActual.UsuarioID);
                    }

                    if (recursosID != null && recursosID.Count > 0)
                    {
                        dataWrapperDocumentacion.Merge(docCN.ObtenerDocumentosPorID(recursosID));
                    }

                    GestorDocumental gestorDoc = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext);
                    docCN.Dispose();

                    foreach (Guid recurso in recursosID)
                    {
                        Documento doc = gestorDoc.ListaDocumentos[recurso];

                        //Eliminando recursos mientras estamos viendo el perfil
                        try
                        {
                            double espacioArchivo = 0;

                            if (doc.EsFicheroDigital)
                            {
                                if (doc.TipoDocumentacion == TiposDocumentacion.FicheroServidor)
                                {
                                    GestionDocumental gd = new GestionDocumental(mLoggingService);
                                    gd.Url = UrlServicioWebDocumentacion;

                                    espacioArchivo = gd.ObtenerEspacioDocumentoDeBaseRecursosUsuario(TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.PersonaID, recurso, Path.GetExtension(doc.Enlace));
                                    
                                }
                                else if (doc.TipoDocumentacion == TiposDocumentacion.Imagen)
                                {
                                    ServicioImagenes sI = new ServicioImagenes(mLoggingService);
                                    sI.Url = UrlIntragnossServicios;

                                    espacioArchivo = sI.ObtenerEspacioImagenDocumentoPersonal(recurso.ToString(), ".jpg", mControladorBase.UsuarioActual.PersonaID);
                                }
                                else if (doc.TipoDocumentacion == TiposDocumentacion.Video)
                                {

                                    CallInterntService sV = new CallInterntService(mConfigService);

                                    espacioArchivo = sV.ObtenerEspacioVideoPersonal(recurso, mControladorBase.UsuarioActual.PersonaID);

                                }
                            }

                            if (espacioArchivo != 0)
                            {
                                gestorDoc.EspacioActualBaseRecursos = gestorDoc.EspacioActualBaseRecursos - espacioArchivo;

                                if (gestorDoc.EspacioActualBaseRecursos < 0)
                                {
                                    gestorDoc.EspacioActualBaseRecursos = 0;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        Dictionary<Guid, Guid> listaBRsConProyectos = new Dictionary<Guid, Guid>();

                        Guid brActual = gestorDoc.BaseRecursosIDActual;
                        listaBRsConProyectos.Add(brActual, mControladorBase.UsuarioActual.ProyectoID);


                        if (doc.BaseRecursos.Count < 2 && doc.FilaDocumentoWebVinBR_Publicador != null && doc.FilaDocumentoWebVinBR_Publicador.IdentidadPublicacionID.Equals(IdentidadActual.Clave))
                        {
                            //Si solo existe en una base de recursos y el publicador del recurso soy yo con mi identidad del metaproyecto, se trata de un recurso publicado en mi espacio personal. Por lo tanto no se desvincula, sino que se elimina.
                            gestorDoc.EliminarDocumentoLogicamente(doc);
                        }
                        else
                        {
                            gestorDoc.DesVincularDocumentoDeBaseRecursos(recurso, brActual, mControladorBase.UsuarioActual.ProyectoID, UsuarioActual.IdentidadID);
                        }
                        doc.FilaDocumento.FechaModificacion = DateTime.Now;


                        #region Actualizar cola GnossLIVE

                        int tipo;

                        switch (doc.TipoDocumentacion)
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
                                infoExtra = IdentidadActual.PerfilID.ToString();
                            }

                            ControladorDocumentacion.ActualizarGnossLive(listaBRsConProyectos[baseRecursosID], doc.Clave, AccionLive.Eliminado, tipo, false, "live", PrioridadLive.Alta, infoExtra);
                            ControladorDocumentacion.ActualizarGnossLive(listaBRsConProyectos[baseRecursosID], doc.ObtenerPublicadorEnBR(baseRecursosID), AccionLive.RecursoAgregado, (int)TipoLive.Miembro, PrioridadLive.Alta);
                        }

                        #endregion

                        if (doc.TipoDocumentacion == TiposDocumentacion.Hipervinculo)
                        {
                            ControladorDocumentacion.EstablecePrivacidadRecursoEnMetaBuscador(doc, IdentidadActual, false);
                        }

                        mEntityContext.SaveChanges();

                        ControladorDocumentacion.BorrarCacheControlFichaRecursos(doc.Clave);

                        try
                        {
                            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                            facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, TipoBusqueda.EditarRecursosPerfil.ToString());
                        }
                        catch (Exception)
                        {
                        }
                        
                        FacetadoCN facetadoCN = null;
                        try
                        {
                            string grafoBorrar = IdentidadActual.PerfilID.ToString();

                            if (IdentidadOrganizacion != null)
                            {
                                grafoBorrar = IdentidadOrganizacion.OrganizacionID.Value.ToString();
                            }

                            facetadoCN = new FacetadoCN(UrlIntragnoss, mControladorBase.UsuarioActual.ProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                            facetadoCN.BorrarRecurso(grafoBorrar, doc.Clave);
                        }
                        catch (Exception ex) { mLoggingService.GuardarLogError(ex); }
                        finally {
                            if (facetadoCN != null)
                                facetadoCN.Dispose();
                        }
                    }
                }

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.Message);
            }

            return GnossResultERROR();
        }

        #region  Internos

        /// <summary>
        /// Carga los títulos de la página y los menús.
        /// </summary>
        private void CargarTitulosYMenus()
        {
            string textoBusqueda = "";
            if (RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true"))
            {
                if (IdentidadActual.OrganizacionID.HasValue)
                {
                    //administrador organizacion
                    textoBusqueda = UtilIdiomas.GetText("PERFIL", "ESPACIOCORPORATIVODE", IdentidadActual.OrganizacionPerfil.Nombre);
                    //this.lblMisRecursos.Text = GetText("PERFIL", "ESPACIOCORPORATIVODE", IdentidadActual.OrganizacionPerfil.Nombre);
                }
                else
                {
                    //this.lblMisRecursos.Text = GetText("PERFIL", "MIESPACIOPERSONAL");
                    textoBusqueda = UtilCadenas.ObtenerTextoDeIdioma(EspacioPersonal, UtilIdiomas.LanguageCode, null);
                }
            }
            else
            {
                //this.lblMisRecursos.Text = GetText("PERFIL", "MIESPACIOPERSONAL");
                textoBusqueda = UtilCadenas.ObtenerTextoDeIdioma(EspacioPersonal, UtilIdiomas.LanguageCode, null);
            }

            mPerSpaceModel.PageTitle = textoBusqueda;

            string titulo = "";
            if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                titulo = ProyectoSeleccionado.Nombre + " - ";
            }

            TituloPagina = titulo + InicioTituloPagEcosistema + textoBusqueda;

            mPerSpaceModel.AddNewResourceUrl = mControladorBase.UrlsSemanticas.GetURLBaseRecursosSubir(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, (IdentidadOrganizacion != null));
            mPerSpaceModel.AdminCategoriesUrl = GnossUrlsSemanticas.GetURLAdministrarPerfilCategorias(BaseURLIdioma, UrlPerfil, UtilIdiomas, (RequestParams("organizacion") != null));
        }

        /// <summary>
        /// Obtiene los resultados de la búsqueda.
        /// </summary>
        private ActionResult ObtenerResultados()
        {
            mPerSpaceModel.SearchViewModel = new SearchViewModel();
            TipoBusqueda tipoBusqueda = TipoBusqueda.EditarRecursosPerfil;

            string argumentos = "";
            string separadorArgumentos = "";
            //string parametroadicional = "";
            string parametrosAdicionales = null;

            //if (EsEcosistemaSinMetaProyecto && ProyectoVirtual != null)
            //{
            //    parametroadicional = "proyectoVirtualID=" + ProyectoVirtual.Clave + "|" + parametrosAdicionales;
            //}

            string ordenPorDefecto = "gnoss:hasfechapublicacion";
            string ordenEnSearch = "gnoss:relevancia";

            TipoPagina = TiposPagina.BaseRecursos;

            string filtroPag = "";

            if (!string.IsNullOrEmpty(RequestParams("categoria")))
            {
                string categoriaID = RequestParams("categoria");
                filtroPag = "skos:ConceptID=gnoss:" + HttpUtility.UrlDecode(categoriaID).ToUpper();
                argumentos += separadorArgumentos + filtroPag;
                separadorArgumentos = "|";
            }
            else if (!string.IsNullOrEmpty(RequestParams("searchInfo")))
            {
                string[] filtro = RequestParams("searchInfo").Split('/');

                if (filtro[0].ToLower() == UtilIdiomas.GetText("URLSEM", "CATEGORIA").ToLower())
                {
                    filtroPag = "skos:ConceptID=gnoss:" + HttpUtility.UrlDecode(filtro[2]).ToUpper();
                    argumentos += separadorArgumentos + filtroPag;
                    separadorArgumentos = "|";
                }
                else if (filtro[0].ToLower() == UtilIdiomas.GetText("URLSEM", "TAG").ToLower())
                {
                    filtroPag = "sioc_t:Tag=" + HttpUtility.UrlDecode(filtro[1]);
                    argumentos += separadorArgumentos + filtroPag;
                    separadorArgumentos = "|";
                }
                else
                {
                    FacetaCN tablasCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    string nombreRealFaceta = tablasCN.ObtenerNombreFacetaRedireccion(filtro[0]);

                    if (nombreRealFaceta != null && nombreRealFaceta != "")
                    {
                        if (filtro[0].Equals("autor"))
                        {
                            filtroPag = nombreRealFaceta + "=" + HttpUtility.UrlDecode(filtro[1]);
                        }
                        else
                        {
                            filtroPag = nombreRealFaceta + "=" + HttpUtility.UrlDecode(filtro[0]);
                        }

                        argumentos += separadorArgumentos + filtroPag;
                        separadorArgumentos = "|";
                    }
                }
            }
            
            if (EsEcosistemaSinMetaProyecto && ProyectoVirtual != null)
            {
                parametrosAdicionales = "proyectoVirtualID=" + ProyectoVirtual.Clave + "|" + parametrosAdicionales;
            }

            //string javascriptAdicional = "primeraCargaDeFacetas = false;";

            string instruccionesExtra = "var filtroDePag = ''; var filtrosDeInicio = ''; filtrosPeticionActual = '';";

            if (!string.IsNullOrEmpty(argumentos))
            {
                instruccionesExtra = "var textoCategoria='" + UtilIdiomas.GetText("URLSEM", "CATEGORIA") + "'; var textoBusqAvaz='" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA") + "'; var textoComunidad='" + UtilIdiomas.GetText("URLSEM", "COMUNIDAD") + "'; var filtroDePag = '" + filtroPag.Replace("'", "\\'") + "'; var filtrosDeInicio = decodeURIComponent('" + UrlEncode(argumentos) + "'); filtrosPeticionActual = filtrosDeInicio; ";
            }

            string filtroContexto = "";
            if (RequestParams("filtroContexto") != null)
            {
                filtroContexto = RequestParams("filtroContexto");
            }

            instruccionesExtra += "$(document).ready(function () {FinalizarMontarResultados(decodeURIComponent('" + UrlEncode(parametrosAdicionales) + "'), '', 1, '#' + panResultados); enlazarJavascriptFacetas = true; FinalizarMontarFacetas(); });";

            string ubicacionBusqueda = "MyGnoss";

            Guid identidadBusqueda = mControladorBase.UsuarioActual.IdentidadID;
            Guid? identidadOrg = null;

            if (IdentidadOrganizacion != null)
            {
                identidadBusqueda = IdentidadOrganizacion.Clave;
                identidadOrg = identidadBusqueda;
            }

            insertarScriptBuscador("panFacetas", "panResultados", "panNumResultados", "panListadoFiltros", "panResultados", "panFiltros", mGrafo, "navegadorBusqueda", instruccionesExtra, "panNavegador", parametrosAdicionales, ordenPorDefecto, ordenEnSearch, filtroContexto, "", Guid.Empty, ProyectoSeleccionado.Clave, ubicacionBusqueda, (short)tipoBusqueda, null, identidadOrg);

            string rawUrl = this.Request.GetDisplayUrl();
            bool hayParametrosBusqueda = false;

            if (!string.IsNullOrEmpty(this.Request.GetDisplayUrl()))
            {
                rawUrl = Request.GetDisplayUrl();

                //bug7595
                rawUrl = rawUrl.Replace("+", "%2B");
                rawUrl = HttpUtility.UrlDecode(rawUrl);

                //Detectado que puede estar doblemente codificado al haber más de un filtro, visto en InternetExplorer 8.
                for (int i = 0; i < rawUrl.Split('&').Length; i++)
                {
                    if (UtilCadenas.PasarAUtf8(UtilCadenas.PasarAANSI(rawUrl)) == rawUrl)
                    {
                        rawUrl = UtilCadenas.PasarAANSI(rawUrl);
                    }
                }
            }

            if (!string.IsNullOrEmpty(rawUrl) && rawUrl.Contains("?"))
            {
                //hay más parámetros, los añado a los argumentos
                string queryString = rawUrl.Substring(rawUrl.IndexOf('?') + 1);

                char[] separadores = { '&' };

                string[] filtros = queryString.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                foreach (string filtro in filtros)
                {
                    argumentos += separadorArgumentos + RemoverSignosSearch(filtro);
                    separadorArgumentos = "|";
                    hayParametrosBusqueda = true;
                }
            }

            bool primeraCarga = string.IsNullOrEmpty(argumentos);

            CargarResultadosYFacetas(mPerSpaceModel.SearchViewModel, ProyectoSeleccionado.Clave, ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto), !mControladorBase.UsuarioActual.EsIdentidadInvitada, mControladorBase.UsuarioActual.EsUsuarioInvitado, identidadBusqueda, argumentos, primeraCarga, true, tipoBusqueda, /*mControladorBase.DominoAplicacionConHTTP + rawUrl*/ "", mGrafo, parametrosAdicionales, hayParametrosBusqueda, ubicacionBusqueda, UtilIdiomas.LanguageCode, mControladorBase.UsuarioActual.UsarMasterParaLectura);

            return null;
        }

        private void CargarResultadosYFacetas(SearchViewModel pModelo, Guid pProyectoID, bool pEsMyGnoss, bool pEstaEnProyecto, bool EsUsuarioInvitado, Guid pIdentidadID, string pArgumentos, bool pEsPrimeraCarga, bool pAdminQuiereVerTodasPersonas, TipoBusqueda pTipoBusqueda, string pUrlPaginaBusqueda, string pGrafo, string pParametroAdicional, bool pHayParametrosBusqueda, string pUbicacionBusqueda, string pLanguageCode, bool pUsarMasterParaLectura)
        {
            Stopwatch sw = null;
            CargadorResultados cargadorResultados = new CargadorResultados();
            cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();

            CargadorFacetas cargadorFacetas = new CargadorFacetas();
            cargadorFacetas.Url = mConfigService.ObtenerUrlServicioFacetas();

            Thread hiloResultados = new Thread(() =>
            {
                try
                {
                    sw = LoggingService.IniciarRelojTelemetria();
                    string jsonResultados = cargadorResultados.CargarResultados(pProyectoID, pIdentidadID, EsUsuarioInvitado, pUrlPaginaBusqueda, pUsarMasterParaLectura, pAdminQuiereVerTodasPersonas, pTipoBusqueda, pGrafo, pParametroAdicional, pArgumentos, pEsPrimeraCarga, pLanguageCode, -1, "", null, Request);
                    mLoggingService.AgregarEntradaDependencia("Llamar al servicio de resultados", false, "EspacioPersonalController.CargarResultadosYFacetas", sw, true);
                    try
                    {
                        KeyValuePair<int, string> respuestaResultados = JsonConvert.DeserializeObject<KeyValuePair<int, string>>(jsonResultados);

                        //numResultados = respuestaResultados.Key;
                        pModelo.HTMLResourceList = respuestaResultados.Value;
                    }
                    catch
                    {
                        pModelo.HTMLResourceList = jsonResultados;
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        mLoggingService.AgregarEntradaDependencia("Error al llamar al servicio de resultados", false, "EspacioPersonalController.CargarResultadosYFacetas", sw, false);
                        GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    }
                    catch (Exception) { } 
                }
            });

            Thread hiloFacetas = new Thread(() =>
            {
                try
                {
                    sw = LoggingService.IniciarRelojTelemetria();
                    pModelo.HTMLFaceted = cargadorFacetas.CargarFacetas(pProyectoID, pEstaEnProyecto, EsUsuarioInvitado, pIdentidadID, pArgumentos, pUbicacionBusqueda, (!pHayParametrosBusqueda), pLanguageCode, pAdminQuiereVerTodasPersonas, pTipoBusqueda, 1, null, pGrafo, pParametroAdicional, "", pUrlPaginaBusqueda, pUsarMasterParaLectura, null, Request);
                    mLoggingService.AgregarEntradaDependencia("Llamar al servicio de facetas", false, "EspacioPersonalController.CargarResultadosYFacetas", sw, true);
                }
                catch (Exception ex)
                {
                    try
                    {
                        mLoggingService.AgregarEntradaDependencia("Error al llamar al servicio de facetas", false, "EspacioPersonalController.CargarResultadosYFacetas", sw, false);
                        GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    }
                    catch (Exception) { }
                }
            });

            mLoggingService.AgregarEntrada("Llamada a los servicios de resultados y facetas");
            //Lanzamos y esperamos a las facetas y los resultados
            if (hiloResultados != null)
            {
                hiloResultados.Start();
            }
            hiloFacetas.Start();
            if (hiloResultados != null)
            {
                hiloResultados.Join();
            }
            hiloFacetas.Join();
            mLoggingService.AgregarEntrada("Fin llamada a los servicios de resultados y facetas");
        }

        /// <summary>
        /// Redirecciona a la página indicada permatentemente.
        /// </summary>
        /// <param name="pUrl">Página</param>
        /// <returns>Acción redirección</returns>
        private ActionResult RedirigirAPaginaCon301(string pUrl)
        {
            return new RedirectResult(pUrl, true);
        }

        /// <summary>
        /// Carga los componentes del panel siderbar.
        /// </summary>
        private void CargarComponerPanelSidebar()
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            
            if (RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true"))
            {
                docCN.ObtenerBaseRecursosOrganizacion(dataWrapperDocumentacion, IdentidadActual.OrganizacionPerfil.Clave);
            }
            else
            {
                docCN.ObtenerBaseRecursosUsuario(dataWrapperDocumentacion, mControladorBase.UsuarioActual.UsuarioID);
            }

            docCN.Dispose();
            GestorDocumental gestDoc = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext);

            mPerSpaceModel.UsedMegaBytes = (decimal)Math.Round(new decimal(gestDoc.EspacioActualBaseRecursos), 2);
            mPerSpaceModel.FreeMegaBytes = (decimal)Math.Round(new decimal((gestDoc.EspacioMaximoBaseRecursos - gestDoc.EspacioActualBaseRecursos)), 2);

            gestDoc.Dispose();
            FacetadoDS facetDS = new FacetadoDS();
            //ObtenerGrafoBusqueda();

            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, true, "perfil/" + mGrafo, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCN.ObtieneNumeroResultados(facetDS, "RecursosBusqueda", new Dictionary<string, List<string>>(), new List<string>(), new List<string>(), TiposAlgoritmoTransformacion.Ninguno, UsuarioActual.ProyectoID, IdentidadActual.EsIdentidadInvitada, UsuarioActual.EsUsuarioInvitado, IdentidadActual.Clave);
            facetadoCN.Dispose();

            int numeroResultados = 0;
            if ((facetDS.Tables.Contains("NResultadosBusqueda")) && (facetDS.Tables["NResultadosBusqueda"].Rows.Count > 0))
            {
                object resultado = facetDS.Tables["NResultadosBusqueda"].Rows[0][0];
                if (resultado != null)
                {
                    int.TryParse(resultado.ToString(), out numeroResultados);
                }
            }

            facetDS.Dispose();

            mPerSpaceModel.TotalNumberResults = numeroResultados;
        }

        /// <summary>
        /// Obtiene el grafo de búsqueda.
        /// </summary>
        private void ObtenerGrafoBusqueda()
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion").Equals("true"))
            {
                mGrafo = IdentidadActual.OrganizacionPerfil.Clave.ToString();
            }
            else
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                Guid identidadID = Guid.Empty;

                if (RequestParams("nombreCortoPerfil") != null)
                {
                    string nombreCortoOrg = "";
                    if (RequestParams("nombreCortoOrganizacion") != null)
                    {
                        nombreCortoOrg = RequestParams("nombreCortoOrganizacion");
                    }
                    identidadID = identidadCN.ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(RequestParams("nombreCortoPerfil"), ProyectoSeleccionado.Clave, nombreCortoOrg, false)[0];
                }
                else if (RequestParams("nombreCortoOrganizacion") != null)
                {
                    identidadID = identidadCN.ObtenerIdentidadIDDeOrganizacionEnProyecto(RequestParams("nombreCortoOrganizacion"), ProyectoSeleccionado.Clave);
                }

                if (!identidadID.Equals(Guid.Empty))
                {
                    List<Guid> ListaIdentidades = new List<Guid>();
                    ListaIdentidades.Add(identidadID);
                    mGrafo = identidadCN.ObtenerPerfilesDeIdentidades(ListaIdentidades)[0].ToString();
                }
                else
                {
                    if (IdentidadActual.Tipo == TiposIdentidad.Personal || IdentidadActual.IdentidadPersonalMyGNOSS == null)
                    {
                        mGrafo = IdentidadActual.PerfilID.ToString();
                    }
                    //else if (IdentidadActual.Tipo == TiposIdentidad.ProfesionalPersonal)
                    //{
                        //mGrafo = IdentidadActual.PerfilID.ToString();
                    //}
                    else
                    {
                        mGrafo = IdentidadActual.IdentidadPersonalMyGNOSS.PerfilID.ToString();
                    }
                }

                identidadCN.Dispose();
            }
        }

        /// <summary>
        /// Comprueba si hay que hacer alguna redirección a otra página.
        /// </summary>
        /// <returns>Resultado de la acción</returns>
        private ActionResult ComprobarRedirecciones()
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return Redirect(BaseURLIdioma + "/home");
            }

            if (IdentidadOrganizacion != null && (IdentidadActual.OrganizacionID == null || IdentidadActual.OrganizacionID != IdentidadOrganizacion.OrganizacionID.Value || !EsIdentidadActualAdministradorOrganizacion))
            {
                return Redirect(mControladorBase.UrlsSemanticas.GetURLBaseRecursosInicio(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, IdentidadOrganizacion != null));
            }

            return null;
        }

        private static string RemoverSignosSearch(string pTexto)
        {
            StringBuilder textoSearch = new StringBuilder(pTexto.Length);
            int indiceCaracterAcento;
            foreach (char caracter in pTexto)
            {
                indiceCaracterAcento = LETRAS_CON_ACENTOS.IndexOf(caracter);

                if (SIGNOS_ELIMINAR_SEARCH.IndexOf(caracter) > -1)
                {
                    textoSearch.Append(" ");
                }
                else if (LETRAS_CON_ACENTOS.IndexOf(caracter) > -1)
                {

                    textoSearch.Append(LETRAS_SIN_ACENTOS.Substring(indiceCaracterAcento, 1));
                }
                else
                {
                    textoSearch.Append(caracter);
                }
            }
            return textoSearch.ToString();
        }

        #endregion

        #endregion
    }
}
