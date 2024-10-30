using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class AdminCatEspacioPersonalController : ControllerBaseWeb
    {
        #region Miembros

        /// <summary>
        /// Modelo de la página.
        /// </summary>
        private AdminCatEspacioPersonalModel mAdminCatModel;

        /// <summary>
        /// Identidad de la organización en el caso de que haya.
        /// </summary>
        private Identidad mIdentidadOrganizacion;

        /// <summary>
        /// Gestor de tesauro
        /// </summary>
        private GestionTesauro mGestorTesauro;

        /// <summary>
        /// Modelo de la acción de edición del tesauro.
        /// </summary>
        private EditThesaurusPersonalSpaceModel mEditTesModel;

        /// <summary>
        /// IDs con las categorías que deben pintarse expandidas.
        /// </summary>
        private List<Guid> mCategoriasExpandidasIDs = new List<Guid>();

        /// <summary>
        /// Diccionario con categorias a mover para actualizar el Base la categoria clave sera la nueva padre de la lista de categorías
        /// </summary>
        private Dictionary<Guid, List<Guid>> mMoverCategoriasActualizarBase;

        /// <summary>
        /// DataSet de documentación
        /// </summary>
        private DataWrapperDocumentacion mDocumentacionDW;
        /// <summary>
        /// DataWrapper de documentación
        /// </summary>
        private DataWrapperDocumentacion mDataWrapperDocumentacion;

        /// <summary>
        /// Diccionario con categorias a eliminar para actualizar el Base (todos los recursos y dafos se vinculan a la nueva categoria)
        /// </summary>
        private Dictionary<Guid, List<Guid>> mEliminarCategoriasActualizarBaseTodo;

        /// <summary>
        /// Diccionario con categorias a eliminar para actualizar el Base (solo los recursos y dafos huerfanos se vinculan a la nueva categoria)
        /// </summary>
        private Dictionary<Guid, List<Guid>> mEliminarCategoriasActualizarBaseSoloHuerfanos;

        /// <summary>
        /// IDs de los documentos afectados por la edición de las categorías.
        /// </summary>
        private List<Guid> mDocumentosAfectadosID;

        #endregion


        public AdminCatEspacioPersonalController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
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
        

        /// <summary>
        /// Obtiene o establece el dataset de documentación
        /// </summary>
        private DataWrapperDocumentacion DataWrapperDocumentacion
        {
            get
            {
                if (mDataWrapperDocumentacion == null)
                {
                    mDataWrapperDocumentacion = new DataWrapperDocumentacion();
                }
                return mDataWrapperDocumentacion;
            }
            set
            {
                mDataWrapperDocumentacion = value;
            }
        }

        /// <summary>
        /// Dictionary con categorías para actualizar en el base (el primer Guid es la categoría destino, Guid.Empty si no hay, y la lista de categorías son las categorías a eliminar)
        /// </summary>
        private Dictionary<Guid, List<Guid>> EliminarCategoriasActualizarBaseSoloHuerfanos
        {
            get
            {
                if (mEliminarCategoriasActualizarBaseSoloHuerfanos == null)
                {
                    mEliminarCategoriasActualizarBaseSoloHuerfanos = new Dictionary<Guid, List<Guid>>();
                }

                return mEliminarCategoriasActualizarBaseSoloHuerfanos;
            }
            set
            {
                mEliminarCategoriasActualizarBaseSoloHuerfanos = value;
            }
        }

        /// <summary>
        /// Dictionary con categorías para actualizar en el base (el primer Guid es la categoría destino, Guid.Empty si no hay, y la lista de categorías son las categorías a eliminar)
        /// </summary>
        private Dictionary<Guid, List<Guid>> EliminarCategoriasActualizarBaseTodo
        {
            get
            {
                if (mEliminarCategoriasActualizarBaseTodo == null)
                {
                    mEliminarCategoriasActualizarBaseTodo = new Dictionary<Guid, List<Guid>>();
                }

                return mEliminarCategoriasActualizarBaseTodo;
            }
            set
            {
                mEliminarCategoriasActualizarBaseTodo = value;
            }
        }

        /// <summary>
        /// IDs de los documentos afectados por la edición de las categorías.
        /// </summary>
        private List<Guid> DocumentosAfectadosID
        {
            get
            {
                if (mDocumentosAfectadosID == null)
                {
                    mDocumentosAfectadosID = new List<Guid>();
                }

                return mDocumentosAfectadosID;
            }
        }

        #endregion

        #region Métodos acción

        /// <summary>
        /// Método inicial.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index()
        {
            mAdminCatModel = new AdminCatEspacioPersonalModel();

            ActionResult redireccion = ComprobarRedirecciones();

            if (redireccion != null)
            {
                return redireccion;
            }

            CargarUrls();
            CargarTesauroBD();
            CargarTesauro();

            return View(mAdminCatModel);
        }

        /// <summary>
        /// Método inicial. En este caso, la información es devuelta para ser gestionada en una vista modal.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult LoadActionModalView()
        {
            mAdminCatModel = new AdminCatEspacioPersonalModel();

            ActionResult redireccion = ComprobarRedirecciones();

            if (redireccion != null)
            {
                return redireccion;
            }

            CargarUrls();
            CargarTesauroBD();
            CargarTesauro();

            // Devolver la vista parcial con el modelo de datos correspondiente            
            return GnossResultHtml("../EspacioPersonal/_modal-views/_manage-categories", mAdminCatModel);
        }

        /// <summary>
        /// Acción de modificar el tesauro.
        /// </summary>
        /// <param name="pModel">Modelo de la acción</param>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult ModificarTesauro(EditThesaurusPersonalSpaceModel pModel)
        {
            try
            {
                ActionResult redireccion = ComprobarRedirecciones();

                if (redireccion != null)
                {
                    return redireccion;
                }

                mAdminCatModel = new AdminCatEspacioPersonalModel();
                CargarTesauroBD();
                mEditTesModel = pModel;
                EjecutarAccionesBackup();

                if (mEditTesModel.ActionsBackUp == null)
                {
                    mEditTesModel.ActionsBackUp = "";
                }

                if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.CreateCategory)
                {
                    return CrearCategoria();
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.ReNameCategory)
                {
                    return RenombrarCategoria();
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.PrepareMoveCategories)
                {
                    return PrepararMoverCategorias();
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.MoveCategories)
                {
                    return MoverCategorias();
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.PrepareOrderCategories)
                {
                    return PrepararOrdenarCategorias();
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.OrderCategories)
                {
                    return OrdenarCategorias();
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.PrepareDeleteCategories)
                {
                    return PrepararEliminarCategorias();
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.DeleteCategories)
                {
                    return EliminarCategorias(false);
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.DeleteCategoriesAndMoveOnlyOrphansResources)
                {
                    return EliminarCategorias(true);
                }
                else if (mEditTesModel.EditAction == EditThesaurusPersonalSpaceModel.Action.SaveThesaurus)
                {
                    return GuardarCategorias();
                }
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.ToString());
            }

            return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
        }

        #endregion

        #region Métodos auxiliares

        /// <summary>
        /// Acción de guardar los cambios de categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult GuardarCategorias()
        {
            List<object> categoriasModificadas = mEntityContext.ObtenerElementosModificados(typeof(AD.EntityModel.Models.Tesauro.CategoriaTesauro)); 
                
            Dictionary<string, string> listaModificadas = new Dictionary<string, string>();
            
            if ((categoriasModificadas != null) && (categoriasModificadas.Count > 0))
            {
                try
                {
                    //Intento actualiar el modelo base
                    foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCat in categoriasModificadas)
                    {
                        string nombreOriginal = mEntityContext.ObtenerValorOriginalDeObjeto<string>(filaCat, nameof(filaCat.Nombre)).Trim().ToLower();
                        string nombreActual = filaCat.Nombre.Trim().ToLower();

                        if (!listaModificadas.ContainsKey(nombreOriginal))
                        {
                            listaModificadas.Add(nombreOriginal, nombreActual);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                }
            }

            mEntityContext.SaveChanges();

            ActualizarCategoriasModeloBaseUsuario();

            mAdminCatModel.ActionsBackUp = "";
            mGestorTesauro = new GestionTesauro(mGestorTesauro.TesauroDW, mLoggingService, mEntityContext);
            CargarTesauro();

            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Actualiza el modelo base para los documentos y dafos modificados.
        /// </summary>
        private void ActualizarCategoriasModeloBaseUsuario()
        {
            if (mMoverCategoriasActualizarBase == null)
            {
                mMoverCategoriasActualizarBase = new Dictionary<Guid, List<Guid>>();
            }

            ControladorDocumentacion.AgregarEliminacionCategoriasModeloBaseUsuario(EliminarCategoriasActualizarBaseTodo, true, PrioridadBase.Alta);
            ControladorDocumentacion.AgregarEliminacionCategoriasModeloBaseUsuario(EliminarCategoriasActualizarBaseSoloHuerfanos, false, PrioridadBase.Alta);
            ControladorDocumentacion.AgregarMoverCategoriasModeloBaseUsuario(mMoverCategoriasActualizarBase, PrioridadBase.Alta);

            foreach (Guid documentoID in DocumentosAfectadosID)
            {
                ControladorDocumentacion.BorrarCacheControlFichaRecursos(documentoID);
            }
        }

        /// <summary>
        /// Acción de mover categorías.
        /// </summary>
        /// <param name="pMoverSoloLoJusto">Indica si lo se debe mover los recursos que se queden huerfanos</param>
        /// <returns>Acción de respuesta</returns>
        private ActionResult EliminarCategorias(bool pMoverSoloLoJusto)
        {
            List<Guid> catSelecc = new List<Guid>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid catID = new Guid(catTesID);
                catSelecc.Add(catID);
            }

            if (catSelecc.Count == 0)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            if (mEditTesModel.SelectedCategory == Guid.Empty)
            {
                return GnossResultERROR();
            }

            List<Guid> listaCatEliminarIDs = new List<Guid>();

            foreach (Guid catTeID in catSelecc)
            {
                AgregarCategoriaEHijosALista(mGestorTesauro.ListaCategoriasTesauro[catTeID], listaCatEliminarIDs);
            }

            Guid categoriaSustitutaID = mEditTesModel.SelectedCategory;

            if (pMoverSoloLoJusto)
            {
                mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, (short)EditThesaurusPersonalSpaceModel.Action.DeleteCategoriesAndMoveOnlyOrphansResources, "|_|", categoriaSustitutaID.ToString());
            }
            else
            {
                mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, (short)EditThesaurusPersonalSpaceModel.Action.DeleteCategories, "|_|", categoriaSustitutaID.ToString());
            }

            foreach (Guid idCatEliminar in listaCatEliminarIDs)
            {
                mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|_|", idCatEliminar.ToString());
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp,"|,|");

            EliminarCategoriasTesauro(categoriaSustitutaID, listaCatEliminarIDs, pMoverSoloLoJusto);

            ExpandirTesauroHastaCategoria(categoriaSustitutaID);

            mGestorTesauro = new GestionTesauro(mGestorTesauro.TesauroDW, mLoggingService, mEntityContext);
            CargarTesauro();
            mAdminCatModel.ActionsBackUp = mEditTesModel.ActionsBackUp;

            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Eliminar una serie de categorías del tesauro.
        /// </summary>
        /// <param name="pIDCategoriaVincularm">Catetgoría nueva a la que se vincularán los elementos</param>
        /// <param name="pListaCategoriasAEliminar">Lista de categorías a eliminar</param>
        /// <param name="pMoverSoloLoJusto">Indica si solo se debe recatigarizar aquellas cosas que de lo contrario quedarían huérfanas</param>
        private void EliminarCategoriasTesauro(Guid pIDCategoriaVincularm, List<Guid> pListaCategoriasAEliminar, bool pMoverSoloLoJusto)
        {
            TesauroCN tesCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool tieneRecursosAsociados = tesCN.EstanVinculadasCategoriasTesauro(mGestorTesauro.TesauroActualID, pListaCategoriasAEliminar);
            tesCN.Dispose();

            if (tieneRecursosAsociados)
            {
                MoverDependenciasCategoriasAOtras(pIDCategoriaVincularm, pListaCategoriasAEliminar, pMoverSoloLoJusto);
            }

            foreach (Guid IDCatAEliminar in pListaCategoriasAEliminar)
            {
                CategoriaTesauro catTe = mGestorTesauro.ListaCategoriasTesauro[IDCatAEliminar];
                if (!catTe.EstaEliminado)
                {
                    mGestorTesauro.EliminarCategoriaEHijos(catTe);
                }
            }
        }

        /// <summary>
        /// Mueve los elementos de las catorías que se van a eliminar a otra categoría pasada por parámetro (todos si pMoverSoloLoJusto==false y solo aquellos que se quedarían huérfanos en caso contrario)
        /// </summary>
        /// <param name="pCategoriaSustitutaID">Identificador de la categoría a la que hay que mover todo</param>
        /// <param name="pListaCategoriasParaSustituirIDs">Lista de identificadores de categorías que se eliminarán</param>
        private void MoverDependenciasCategoriasAOtras(Guid pCategoriaSustitutaID, List<Guid> pListaCategoriasParaSustituirIDs, bool pMoverSoloLoJusto)
        {
            #region Muevo los documentos de las categorías

            if (!pListaCategoriasParaSustituirIDs.Contains(pCategoriaSustitutaID))
            {
                pListaCategoriasParaSustituirIDs.Add(pCategoriaSustitutaID);
            }

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperDocumentacion dataWrapperDocumentacion = docCN.ObtenerVinculacionDocumentosDeCategoriasTesauro(pListaCategoriasParaSustituirIDs, mGestorTesauro.TesauroActualID);
            docCN.Dispose();
            List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauroBorrar = dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocAgTes in listaDocumentoWebAgCatTesauroBorrar)
            {
                if (mEntityContext.Entry(filaDocAgTes).State != EntityState.Deleted)
                {
                    //Si la categoría a la que pertenece el recurso hay q sustituirla y no es la sustituta hay que proceder con ella
                    if (pListaCategoriasParaSustituirIDs.Contains(filaDocAgTes.CategoriaTesauroID) && filaDocAgTes.CategoriaTesauroID != pCategoriaSustitutaID)
                    {
                        if (!DocumentosAfectadosID.Contains(filaDocAgTes.DocumentoID))
                        {
                            DocumentosAfectadosID.Add(filaDocAgTes.DocumentoID);
                        }

                        string categoriasEliminadas = "";
                        int num = 0;
                        foreach (Guid catID in pListaCategoriasParaSustituirIDs)
                        {
                            if (num > 0)
                            {
                                categoriasEliminadas += " AND ";
                            }
                            categoriasEliminadas += " CategoriaTesauroID<>'" + catID + "'";
                            num++;
                        }

                        //si pMoverSoloLoJusto y tiene otras categorías que no se van a borrar, se borra la vinculación con esta categoría
                        //o ya está vinculado con la categoría destino
                        //Select("DocumentoID='" + filaDocAgTes.DocumentoID + "' AND BaseRecursosID='" + filaDocAgTes.BaseRecursosID + "' AND CategoriaTesauroID='" + pCategoriaSustitutaID + "'").Length
                        if ((pMoverSoloLoJusto && dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Any(doc=>doc.DocumentoID.Equals(filaDocAgTes.DocumentoID) && doc.BaseRecursosID.Equals(filaDocAgTes.BaseRecursosID) && !pListaCategoriasParaSustituirIDs.Contains(doc.CategoriaTesauroID))) || dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Any(doc=>doc.DocumentoID.Equals(filaDocAgTes.DocumentoID) && doc.BaseRecursosID.Equals(filaDocAgTes.BaseRecursosID) && doc.CategoriaTesauroID.Equals(pCategoriaSustitutaID)))
                        {
                            mEntityContext.EliminarElemento(filaDocAgTes);
                            dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Remove(filaDocAgTes);
                        }
                        else
                        {
                            //si no se cambia (porque es la única)
                            filaDocAgTes.CategoriaTesauroID = pCategoriaSustitutaID;
                        }
                    }
                }
            }

            if (pMoverSoloLoJusto)
            {
                EliminarCategoriasActualizarBaseSoloHuerfanos.Add(pCategoriaSustitutaID, pListaCategoriasParaSustituirIDs);
            }
            else
            {
                EliminarCategoriasActualizarBaseTodo.Add(pCategoriaSustitutaID, pListaCategoriasParaSustituirIDs);
            }


            DataWrapperDocumentacion.Merge(dataWrapperDocumentacion);

            #endregion

            pListaCategoriasParaSustituirIDs.Remove(pCategoriaSustitutaID);
        }

        /// <summary>
        /// Prepara la acción de mover categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult PrepararEliminarCategorias()
        {
            mAdminCatModel.CategoryNamesToDelete = new List<string>();
            List<Guid> catSelecc = new List<Guid>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid catID = new Guid(catTesID);
                catSelecc.Add(catID);
            }

            List<Guid> listaAux = catSelecc;
            List<Guid> listaAux2 = new List<Guid>();
            foreach (Guid idCategoria in listaAux)
            {
                AgregarCategoriaEHijosALista(mGestorTesauro.ListaCategoriasTesauro[idCategoria], listaAux2);
            }

            catSelecc = listaAux2;

            if (catSelecc.Count == 0)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            TesauroCN tesCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool tieneRecursosAsociados = tesCN.EstanVinculadasCategoriasTesauro(mGestorTesauro.TesauroActualID, catSelecc);
            tesCN.Dispose();

            if (!tieneRecursosAsociados)
            {
                //Seleccionamos el padre del primer elemento para luego expandir el tesaro:
                mEditTesModel.SelectedCategory = ((CategoriaTesauro)mGestorTesauro.ListaCategoriasTesauro[catSelecc[0]].Padre).Clave;
                return EliminarCategorias(false);
            }

            foreach (Guid catID in catSelecc)
            {
                ExpandirTesauroHastaCategoria(catID);
                mAdminCatModel.CategoryNamesToDelete.Add(mGestorTesauro.ListaCategoriasTesauro[catID].Nombre[IdiomaUsuario]);
            }

            mAdminCatModel.ParentCategoriesForDeleteCategories = new Dictionary<Guid, string>();
            mAdminCatModel.ParentCategoriesForDeleteCategories.Add(Guid.Empty, UtilIdiomas.GetText("PERFILBASE", "SELECCIONACAT"));

            foreach (CategoriaTesauro catTes in mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
            {
                if (!catSelecc.Contains(catTes.Clave))
                {
                    CargarCategoriasHijasOmitiendoCatEnLista(catTes, string.Empty, catSelecc, mAdminCatModel.ParentCategoriesForDeleteCategories);
                }
            }

            TesauroCN tesCN2 = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mAdminCatModel.ResourcesOfCategoriesDeletingAreNotOrphans = tesCN2.ObtenerSiExistenElementosNoHuerfanos(mGestorTesauro.TesauroActualID, catSelecc);
            tesCN2.Dispose();

            mAdminCatModel.ActionsBackUp = mEditTesModel.ActionsBackUp;

            CargarTesauro();
            mAdminCatModel.ThesaurusEditorModel.SelectedCategories = catSelecc;
            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Agrega una categoría y todos sus hijos a una lista.
        /// </summary>
        /// <param name="pCategoria">Categoría</param>
        /// <param name="pListaCategoriaID">Lista con la categoría y sus hijos</param>
        private void AgregarCategoriaEHijosALista(CategoriaTesauro pCategoria, List<Guid> pListaCategoriaID)
        {
            foreach (IElementoGnoss catTeHija in pCategoria.Hijos)
            {
                AgregarCategoriaEHijosALista((CategoriaTesauro)catTeHija, pListaCategoriaID);
            }

            if (!pListaCategoriaID.Contains(pCategoria.Clave))
            {
                pListaCategoriaID.Add(pCategoria.Clave);
            }
        }

        /// <summary>
        /// Acción de ordenar categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult OrdenarCategorias()
        {
            List<Guid> catSelecc = new List<Guid>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid catID = new Guid(catTesID);
                catSelecc.Add(catID);
            }

            if (catSelecc.Count == 0)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }


            short ordenDestino;
            //Si las categorias seleccionadas son del primer nivel
            if (mGestorTesauro.ListaCategoriasTesauroPrimerNivel.ContainsKey(catSelecc[0]))
            {
                //Obtenemos el orden de destino
                if (mEditTesModel.SelectedCategory == Guid.Empty)
                {
                    ordenDestino = -1;
                }
                else
                {
                    ordenDestino = (short)(mGestorTesauro.ListaCategoriasTesauro[mEditTesModel.SelectedCategory].FilaCategoria.Orden);
                }
            }//Si no son del primer nivel
            else
            {
                if (mEditTesModel.SelectedCategory == Guid.Empty)
                {
                    ordenDestino = -1;
                }
                else
                {
                    ordenDestino = (short)mGestorTesauro.ListaCategoriasTesauro[mEditTesModel.SelectedCategory].FilaAgregacion.Orden;
                }
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, (short)mEditTesModel.EditAction, "|_|", ordenDestino.ToString());

            foreach (Guid guidCatSel in catSelecc)
            {
                mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|_|", guidCatSel.ToString());
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|,|");

            OrdenarCategoriasTesauro(ordenDestino, catSelecc);

            if (mEditTesModel.SelectedCategory != Guid.Empty)
            {
                ExpandirTesauroHastaCategoria(mEditTesModel.SelectedCategory);
            }
            else
            {
                ExpandirTesauroHastaCategoria(catSelecc[0]);
            }

            mGestorTesauro = new GestionTesauro(mGestorTesauro.TesauroDW, mLoggingService, mEntityContext);
            CargarTesauro();
            mAdminCatModel.ActionsBackUp = mEditTesModel.ActionsBackUp;

            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Ordena una serie de categorías
        /// </summary>
        /// <param name="pOrden">Nuevo orden de las categorías</param>
        /// <param name="pListaCategorias">Categorías a ordenar</param>
        private void OrdenarCategoriasTesauro(int pOrden, List<Guid> pListaCategorias)
        {

            //Si las categorías seleccionadas son del primer nivel
            if (mGestorTesauro.ListaCategoriasTesauroPrimerNivel.ContainsKey(pListaCategorias[0]))
            {
                List<CategoriaTesauro> listaCategoriasSeleccionadas = new List<CategoriaTesauro>();
                foreach (Guid idCategoria in pListaCategorias)
                {
                    listaCategoriasSeleccionadas.Add(mGestorTesauro.ListaCategoriasTesauro[idCategoria]);
                }

                //Almacenamos los ordenes antiguos de las categorias que vamos a ordenar
                List<short> ordenesAntiguos = new List<short>();
                foreach (CategoriaTesauro catTes in listaCategoriasSeleccionadas)
                {
                    ordenesAntiguos.Add(catTes.FilaCategoria.Orden);
                }
                short cont = 0;

                //Procedemos a ordenar
                foreach (CategoriaTesauro catTes in mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
                {
                    if (!listaCategoriasSeleccionadas.Contains(catTes))
                    {
                        short cambioOrden = 0;

                        foreach (short ordenAntiguo in ordenesAntiguos)
                        {
                            if (ordenAntiguo > catTes.FilaCategoria.Orden && pOrden < catTes.FilaCategoria.Orden)
                            {
                                cambioOrden++;
                            }
                            if (ordenAntiguo < catTes.FilaCategoria.Orden && pOrden >= catTes.FilaCategoria.Orden)
                            {
                                cambioOrden--;
                            }
                        }
                        catTes.FilaCategoria.Orden = (short)(catTes.FilaCategoria.Orden + cambioOrden);
                    }
                    else
                    {
                        short cambioOrden = 0;

                        foreach (short ordenAntiguo in ordenesAntiguos)
                        {
                            if (ordenAntiguo < pOrden)
                            {
                                cambioOrden--;
                            }
                        }
                        cont++;
                        catTes.FilaCategoria.Orden = (short)(pOrden + cont + cambioOrden);
                    }
                }
            }
            else//Si las categorías seleccionadas no son del primer nivel
            {
                List<CategoriaTesauro> listaCategoriasSeleccionadas = new List<CategoriaTesauro>();
                foreach (Guid idCategoria in pListaCategorias)
                {
                    listaCategoriasSeleccionadas.Add(mGestorTesauro.ListaCategoriasTesauro[idCategoria]);
                }

                //Almacenamos los ordenes antiguos de las categorias que vamos a ordenar
                List<short> ordenesAntiguos = new List<short>();

                foreach (CategoriaTesauro catTes in listaCategoriasSeleccionadas)
                {
                    ordenesAntiguos.Add(catTes.FilaAgregacion.Orden);
                }
                short cont = 0;
                List<IElementoGnoss> categoriasNivel = mGestorTesauro.ListaCategoriasTesauro[pListaCategorias[0]].Padre.Hijos;

                //Procedemos a ordenar
                foreach (IElementoGnoss catTes in categoriasNivel)
                {
                    if (!listaCategoriasSeleccionadas.Contains((CategoriaTesauro)catTes))
                    {
                        short cambioOrden = 0;

                        foreach (short ordenAntiguo in ordenesAntiguos)
                        {
                            if (ordenAntiguo > ((CategoriaTesauro)catTes).FilaAgregacion.Orden && pOrden < ((CategoriaTesauro)catTes).FilaAgregacion.Orden)
                            {
                                cambioOrden++;
                            }
                            if (ordenAntiguo < ((CategoriaTesauro)catTes).FilaAgregacion.Orden && pOrden >= ((CategoriaTesauro)catTes).FilaAgregacion.Orden)
                            {
                                cambioOrden--;
                            }
                        }
                        ((CategoriaTesauro)catTes).FilaAgregacion.Orden = (short)(((CategoriaTesauro)catTes).FilaAgregacion.Orden + cambioOrden);
                    }
                    else
                    {
                        short cambioOrden = 0;

                        foreach (short ordenAntiguo in ordenesAntiguos)
                        {
                            if (ordenAntiguo < pOrden)
                            {
                                cambioOrden--;
                            }
                        }
                        cont++;
                        ((CategoriaTesauro)catTes).FilaAgregacion.Orden = (short)(pOrden + cont + cambioOrden);
                    }
                }
            }
        }

        /// <summary>
        /// Prepara la acción de ordenar categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult PrepararOrdenarCategorias()
        {
            mAdminCatModel.CategoryNamesToOrder = new List<string>();
            List<Guid> catSelecc = new List<Guid>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid catID = new Guid(catTesID);
                catSelecc.Add(catID);
                ExpandirTesauroHastaCategoria(catID);
                mAdminCatModel.CategoryNamesToOrder.Add(mGestorTesauro.ListaCategoriasTesauro[catID].Nombre[IdiomaUsuario]);
            }

            if (catSelecc.Count == 0)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            mAdminCatModel.ParentCategoriesForOrderCategories = new Dictionary<Guid, string>();

            if (!mGestorTesauro.ListaCategoriasTesauroPrimerNivel.ContainsValue((CategoriaTesauro)(mGestorTesauro.ListaCategoriasTesauro[catSelecc[0]])))
            {
                CategoriaTesauro catPadre = (CategoriaTesauro)(mGestorTesauro.ListaCategoriasTesauro[catSelecc[0]].Padre);
                mAdminCatModel.ParentCategoriesForOrderCategories.Add(Guid.Empty, catPadre.Nombre[IdiomaUsuario]);

                foreach (CategoriaTesauro catTes in catPadre.Hijos)
                {
                    if (!catSelecc.Contains(catTes.Clave))
                    {
                        mAdminCatModel.ParentCategoriesForOrderCategories.Add(catTes.Clave, catTes.Nombre[IdiomaUsuario]);
                    }
                }
            }
            else
            {
                foreach (CategoriaTesauro catTes in mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
                {
                    if (!catSelecc.Contains(catTes.Clave))
                    {
                        mAdminCatModel.ParentCategoriesForOrderCategories.Add(catTes.Clave, catTes.Nombre[IdiomaUsuario]);
                    }
                }
            }

            mAdminCatModel.ActionsBackUp = mEditTesModel.ActionsBackUp;

            CargarTesauro();
            mAdminCatModel.ThesaurusEditorModel.SelectedCategories = catSelecc;
            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Acción de mover categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult MoverCategorias()
        {
            List<Guid> catSelecc = new List<Guid>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid catID = new Guid(catTesID);
                catSelecc.Add(catID);
            }

            if (catSelecc.Count == 0)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            if (mEditTesModel.SelectedCategory == Guid.Empty)
            {
                return GnossResultERROR();
            }

            CategoriaTesauro categoriaPadreNueva = mGestorTesauro.ListaCategoriasTesauro[mEditTesModel.SelectedCategory];

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, (short)mEditTesModel.EditAction, "|_|", categoriaPadreNueva.Clave.ToString());
            

            List<Guid> IDCatSeleccionadas = new List<Guid>();

            foreach (Guid catTesID in catSelecc)
            {
                mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|_|", catTesID.ToString());
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|,|");

            MoverCategoriasTesauro(categoriaPadreNueva.Clave, catSelecc);

            ExpandirTesauroHastaCategoria(categoriaPadreNueva.Clave);

            mGestorTesauro = new GestionTesauro(mGestorTesauro.TesauroDW, mLoggingService, mEntityContext);
            CargarTesauro();
            mAdminCatModel.ActionsBackUp = mEditTesModel.ActionsBackUp;

            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Mueve una serie de categorías del tesauro.
        /// </summary>
        /// <param name="pCategoriaPadre">Nueva categoría padre</param>
        /// <param name="pCategoriasAMover">Categorías que movemos</param>
        private void MoverCategoriasTesauro(Guid pCategoriaPadre, List<Guid> pCategoriasAMover)
        {
            foreach (Guid guidCat in pCategoriasAMover)
            {
                if (mGestorTesauro.ListaCategoriasTesauro[guidCat].Padre is CategoriaTesauro)
                {
                    //Ordenamos elementos no raíz si la categoria a mover no es raiz
                    short ordenOriginal = mGestorTesauro.ListaCategoriasTesauro[guidCat].FilaAgregacion.Orden;
                    List<IElementoGnoss> catHermanas = mGestorTesauro.ListaCategoriasTesauro[guidCat].Padre.Hijos;

                    foreach (IElementoGnoss catTesHermana in catHermanas)
                    {
                        if (ordenOriginal < ((CategoriaTesauro)catTesHermana).FilaAgregacion.Orden)
                        {
                            ((CategoriaTesauro)catTesHermana).FilaAgregacion.Orden = (short)(((CategoriaTesauro)catTesHermana).FilaAgregacion.Orden - 1);
                        }
                    }
                    mGestorTesauro.ListaCategoriasTesauro[guidCat].Padre.Hijos.Remove(mGestorTesauro.ListaCategoriasTesauro[guidCat]);
                    mGestorTesauro.DesasignarSubcategoriaDeCategoria(mGestorTesauro.ListaCategoriasTesauro[guidCat], (CategoriaTesauro)mGestorTesauro.ListaCategoriasTesauro[guidCat].Padre);
                    mGestorTesauro = new GestionTesauro(mGestorTesauro.TesauroDW, mLoggingService, mEntityContext);
                }
                else
                {
                    //Ordenamos los elementos Raiz si la categoria a mover es raiz               
                    short ordenCategoriaAMover = mGestorTesauro.ListaCategoriasTesauro[guidCat].FilaCategoria.Orden;

                    foreach (CategoriaTesauro catTesRaiz in mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
                    {
                        if (mGestorTesauro.ListaCategoriasTesauro[guidCat].Equals(catTesRaiz))
                        {
                            catTesRaiz.FilaCategoria.Orden = 0;
                        }
                        else if (ordenCategoriaAMover < catTesRaiz.FilaCategoria.Orden)
                        {
                            catTesRaiz.FilaCategoria.Orden = (short)(catTesRaiz.FilaCategoria.Orden - 1);
                        }
                    }
                }

                if (pCategoriaPadre != Guid.Empty)
                {
                    mGestorTesauro.AgregarSubcategoriaACategoria(mGestorTesauro.ListaCategoriasTesauro[guidCat], mGestorTesauro.ListaCategoriasTesauro[pCategoriaPadre]);

                    mGestorTesauro.ListaCategoriasTesauro[guidCat].FilaAgregacion.Orden = (short)(mGestorTesauro.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaSuperiorID.Equals(((CategoriaTesauro)mGestorTesauro.ListaCategoriasTesauro[guidCat].Padre).Clave)).Count() - 1);
                }
                else
                {
                    //Agregar orden si se mueve a raíz
                    mGestorTesauro.ListaCategoriasTesauro[guidCat].FilaCategoria.Orden = (short)(mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Count - 1);
                }
            }

            if (mMoverCategoriasActualizarBase == null)
            {
                mMoverCategoriasActualizarBase = new Dictionary<Guid, List<Guid>>();
            }

            mMoverCategoriasActualizarBase.Add(pCategoriaPadre, pCategoriasAMover);
        }

        /// <summary>
        /// Prepara la acción de mover categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult PrepararMoverCategorias()
        {
            mAdminCatModel.CategoryNamesToMove = new List<string>();
            List<Guid> catSelecc = new List<Guid>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid catID = new Guid(catTesID);
                catSelecc.Add(catID);
                ExpandirTesauroHastaCategoria(catID);
                mAdminCatModel.CategoryNamesToMove.Add(mGestorTesauro.ListaCategoriasTesauro[catID].Nombre[IdiomaUsuario]);
            }

            if (catSelecc.Count == 0)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            mAdminCatModel.ParentCategoriesForMoveCategories = new Dictionary<Guid, string>();

            foreach (CategoriaTesauro catTes in mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
            {
                if (!catSelecc.Contains(catTes.Clave))
                {
                    CargarCategoriasHijasOmitiendoCatEnLista(catTes, string.Empty, catSelecc, mAdminCatModel.ParentCategoriesForMoveCategories);
                }
            }

            mAdminCatModel.ActionsBackUp = mEditTesModel.ActionsBackUp;

            CargarTesauro();
            mAdminCatModel.ThesaurusEditorModel.SelectedCategories = catSelecc;
            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Gestiona el cambio de nombre de una categoría.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult RenombrarCategoria()
        {
            Guid catSeleccID = mEditTesModel.SelectedCategory;
            bool nombreRepetido = false;

            foreach (IElementoGnoss catTes in mGestorTesauro.ListaCategoriasTesauro[catSeleccID].Padre.Hijos)
            {
                if (((CategoriaTesauro)catTes).Nombre[IdiomaUsuario].ToLower() == mEditTesModel.NewCategoryName.ToLower())
                {
                    nombreRepetido = true;
                }
            }

            if (mEditTesModel.NewCategoryName.ToLower() == mGestorTesauro.ListaCategoriasTesauro[catSeleccID].Nombre[IdiomaUsuario].ToLower())
            {
                nombreRepetido = false;
            }

            if (nombreRepetido)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_NOMBRE_DIFERENTE"));
            }
            else if (string.IsNullOrEmpty(mEditTesModel.NewCategoryName))
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_NOMBRE"));
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, ((short)mEditTesModel.EditAction).ToString(), "|_|", catSeleccID.ToString(), "|_|", mEditTesModel.NewCategoryName, "|,|");

            mGestorTesauro.ListaCategoriasTesauro[catSeleccID].FilaCategoria.Nombre = mEditTesModel.NewCategoryName;

            mGestorTesauro = new GestionTesauro(mGestorTesauro.TesauroDW, mLoggingService, mEntityContext);
            mAdminCatModel.ActionsBackUp = mEditTesModel.ActionsBackUp;
            ExpandirTesauroHastaCategoria(catSeleccID);

            CargarTesauro();

            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Gestiona la creación de una nueva categoría controlando que no exista ya otra con ese nombre y añadiendo la acción a la lista de acciones.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult CrearCategoria()
        {
            bool nombreRepetido = false;

            Guid guidPadre = mEditTesModel.SelectedCategory;
            List<IElementoGnoss> catReps = null;

            if (guidPadre != Guid.Empty)
            {
                catReps = mGestorTesauro.ListaCategoriasTesauro[guidPadre].Hijos;
            }
            else
            {
                catReps = new List<IElementoGnoss>(mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Values);
            }

            foreach (IElementoGnoss catTes in catReps)
            {
                if (((CategoriaTesauro)catTes).Nombre[IdiomaUsuario].ToLower() == mEditTesModel.NewCategoryName.ToLower())
                {
                    nombreRepetido = true;
                }
            }

            if (nombreRepetido)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_NOMBRE_DIFERENTE"));
            }
            else if (string.IsNullOrEmpty(mEditTesModel.NewCategoryName))
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_NOMBRE"));
            }

            Guid IDNuevaCategoria = Guid.NewGuid();
            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, ((short)mEditTesModel.EditAction).ToString(), "|_|", guidPadre.ToString(), "|_|", mEditTesModel.NewCategoryName, "|_|", IDNuevaCategoria, "|,|");
            CrearCategoriaTesauro(guidPadre, IDNuevaCategoria, mEditTesModel.NewCategoryName.Trim());

            ExpandirTesauroHastaCategoria(guidPadre);

            mGestorTesauro = new GestionTesauro(mGestorTesauro.TesauroDW, mLoggingService, mEntityContext);
            mAdminCatModel.ActionsBackUp = mEditTesModel.ActionsBackUp;

            CargarTesauro();

            return GnossResultHtml("_AdminTesauro", mAdminCatModel);
        }

        /// <summary>
        /// Expande el tesauro hasta una categoría.
        /// </summary>
        /// <param name="pCategoriaID">ID de la categoría a expandir</param>
        private void ExpandirTesauroHastaCategoria(Guid pCategoriaID)
        {
            List<Guid> catSel = mCategoriasExpandidasIDs;

            if (!catSel.Contains(pCategoriaID))
            {
                catSel.Add(pCategoriaID);
            }

            while (mGestorTesauro.ListaCategoriasTesauro[pCategoriaID].PadreNivelRaiz != mGestorTesauro.ListaCategoriasTesauro[pCategoriaID])
            {
                pCategoriaID = ((CategoriaTesauro)(mGestorTesauro.ListaCategoriasTesauro[pCategoriaID].Padre)).Clave;
                if (!catSel.Contains(pCategoriaID))
                {
                    catSel.Add(pCategoriaID);
                }
            }
            mCategoriasExpandidasIDs = catSel;
        }

        /// <summary>
        /// Crea una categoría en el tesauro
        /// </summary>
        /// <param name="pIdCategoriaPadre">Id de la categoría padre en la que se crea la nueva categoría</param>
        /// <param name="pIdNuevaCategoria">Id de la nueva categoría</param>
        /// <param name="pNombreCategoria">Nombre de la nueva categoría</param>
        private void CrearCategoriaTesauro(Guid pIdCategoriaPadre, Guid pIdNuevaCategoria, string pNombreCategoria)
        {
            CategoriaTesauro categoriaNueva = null;
            if (pIdCategoriaPadre != Guid.Empty)
            {
                CategoriaTesauro categoriaPadre = mGestorTesauro.ListaCategoriasTesauro[pIdCategoriaPadre];
                categoriaNueva = mGestorTesauro.AgregarSubcategoria(categoriaPadre, pNombreCategoria.Trim());
                ((mGestorTesauro.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaInferiorID.Equals(categoriaNueva.Clave)).FirstOrDefault())).CategoriaInferiorID = pIdNuevaCategoria;
            }
            else
            {
                categoriaNueva = mGestorTesauro.AgregarCategoriaPrimerNivel(pNombreCategoria.Trim());
            }

            (mGestorTesauro.TesauroDW.ListaCategoriaTesauro.Where(item => item.CategoriaTesauroID.Equals(categoriaNueva.Clave)).FirstOrDefault()).CategoriaTesauroID = pIdNuevaCategoria;
        }

        /// <summary>
        /// Ejecuta todas las acciones que se van realizando sobre el tesauro anteriormente.
        /// </summary>
        private void EjecutarAccionesBackup()
        {
            //Si hay acciones a realizar, se realizan
            if (!string.IsNullOrEmpty(mEditTesModel.ActionsBackUp))
            {
                //Obtenemos las acciones
                string[] acciones = mEditTesModel.ActionsBackUp.Split(new string[] { "|,|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string accion in acciones)
                {
                    string[] accion2 = accion.Split(new string[] { "|_|" }, StringSplitOptions.RemoveEmptyEntries);
                    short tipoAccion = short.Parse(accion2[0]);

                    if ((short)EditThesaurusPersonalSpaceModel.Action.CreateCategory == tipoAccion)
                    {//crear
                        Guid idPadre = new Guid(accion2[1]);
                        Guid idNuevaCategoria = new Guid(accion2[3]);
                        string nombreNuevaCat = accion2[2];
                        CrearCategoriaTesauro(idPadre, idNuevaCategoria, nombreNuevaCat.Trim());
                    }
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.ReNameCategory == tipoAccion)
                    {//cambiarnombre
                        Guid idCategoria = new Guid(accion2[1]);
                        string nombreCategoria = accion2[2];
                        mGestorTesauro.ListaCategoriasTesauro[idCategoria].FilaCategoria.Nombre = nombreCategoria;
                    }
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.MoveCategories == tipoAccion)
                    {//MoverCategorias
                        Guid idNuevoPadre = new Guid(accion2[1]);
                        List<Guid> idCat = new List<Guid>();
                        for (int i = 2; i < accion2.Length; i++)
                        {
                            idCat.Add(new Guid(accion2[i]));
                        }
                        MoverCategoriasTesauro(idNuevoPadre, idCat);
                    }
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.OrderCategories == tipoAccion)
                    {//OrdenarCategoria
                        int ordenDestino = int.Parse(accion2[1]);
                        List<Guid> idCategoriasOrdenar = new List<Guid>();
                        for (int i = 2; i < accion2.Length; i++)
                        {
                            idCategoriasOrdenar.Add(new Guid(accion2[i]));
                        }
                        OrdenarCategoriasTesauro(ordenDestino, idCategoriasOrdenar);
                    }
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.DeleteCategories == tipoAccion)
                    {//EliminarCategoria                        
                        Guid idCategoriaVincular = new Guid(accion2[1]);
                        List<Guid> idCategorias = new List<Guid>();
                        for (int i = 2; i < accion2.Length; i++)
                        {
                            idCategorias.Add(new Guid(accion2[i]));
                        }
                        EliminarCategoriasTesauro(idCategoriaVincular, idCategorias, false);
                    }
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.DeleteCategoriesAndMoveOnlyOrphansResources == tipoAccion)
                    {//EliminarCategoriaYMoverSoloHuerfanos                        
                        Guid idCategoriaVincular2 = new Guid(accion2[1]);
                        List<Guid> idCategorias2 = new List<Guid>();
                        for (int i = 2; i < accion2.Length; i++)
                        {
                            idCategorias2.Add(new Guid(accion2[i]));
                        }
                        EliminarCategoriasTesauro(idCategoriaVincular2, idCategorias2, true);
                    }

                    mGestorTesauro = new GestionTesauro(mGestorTesauro.TesauroDW, mLoggingService, mEntityContext);
                }
            }
        }

        /// <summary>
        /// Carga el tesauro del perfil de la base de datos.
        /// </summary>
        private void CargarTesauroBD()
        {
            mGestorTesauro = CargarGestorTesauroBRActual();
        }

        /// <summary>
        /// Carga el tesauro del perfil.
        /// </summary>
        private void CargarTesauro()
        {
            mAdminCatModel.ThesaurusEditorModel = new ThesaurusEditorModel();
            mAdminCatModel.ThesaurusEditorModel.HideTreeListSelector = true;
            mAdminCatModel.ThesaurusEditorModel.ThesaurusCategories = CargarTesauroPorGestorTesauro(mGestorTesauro);
            mAdminCatModel.ThesaurusEditorModel.SelectedCategories = new List<Guid>();
            mAdminCatModel.ThesaurusEditorModel.ExpandedCategories = mCategoriasExpandidasIDs;
            DeshabilidatarCategoriasEspeciales();
            RecargarComboPadresParaCreacionCategoria();
        }

        /// <summary>
        /// Carga las urls de la página.
        /// </summary>
        private void CargarUrls()
        {
            mAdminCatModel.BackUrl = mControladorBase.UrlsSemanticas.GetURLBaseRecursos(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, (IdentidadOrganizacion != null));
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

        /// <summary>
        /// Deshabilita las categorías del tesauro especiales.
        /// </summary>
        private void DeshabilidatarCategoriasEspeciales()
        {
            mAdminCatModel.ThesaurusEditorModel.DisabledCategories = new List<Guid>();

            mAdminCatModel.ThesaurusEditorModel.DisabledCategories.Add(mGestorTesauro.CategoriaPublicaID);
            mAdminCatModel.ThesaurusEditorModel.DisabledCategories.Add(mGestorTesauro.CategoriaPrivadaID);

            if (RequestParams("organizacion") == null)
            {
                mAdminCatModel.ThesaurusEditorModel.DisabledCategories.Add(mGestorTesauro.CategoriaImagenesID);
                mAdminCatModel.ThesaurusEditorModel.DisabledCategories.Add(mGestorTesauro.CategoriaVideosID);
            }
        }

        /// <summary>
        /// Recarga el combo de categorías padres para la creación de nuevas.
        /// </summary>
        private void RecargarComboPadresParaCreacionCategoria()
        {
            mAdminCatModel.ParentCategoriesForCreateNewsCategories = new Dictionary<Guid, string>();

            foreach (CategoriaTesauro catTes in mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
            {
                CargarCategoriasHijas(catTes, string.Empty, mAdminCatModel.ParentCategoriesForCreateNewsCategories);
            }
        }

        /// <summary>
        /// Recarga el combo de categorías padres para la creación de nuevas.
        /// </summary>
        /// <param name="pCategoria">Categoría</param>
        /// <param name="pEspacios">Cadena de espacios según nivel de la categoría</param>
        /// <param name="pListaCats">Lista de categorías a añadir</param>
        private void CargarCategoriasHijas(CategoriaTesauro pCategoria, string pEspacios, Dictionary<Guid, string> pListaCats)
        {
            CargarCategoriasHijasOmitiendoCatEnLista(pCategoria, pEspacios, null, pListaCats);
        }

        /// <summary>
        /// Recarga el combo de categorías padres para la creación de nuevas.
        /// </summary>
        /// <param name="pCategoria">Categoría</param>
        /// <param name="pEspacios">Cadena de espacios según nivel de la categoría</param>
        private void CargarCategoriasHijasOmitiendoCatEnLista(CategoriaTesauro pCategoria, string pEspacios, List<Guid> pListaCategoriasOmitir, Dictionary<Guid, string> pListaCats)
        {
            pListaCats.Add(pCategoria.Clave, pEspacios + pCategoria.Nombre[IdiomaUsuario]);

            pEspacios += UtilCadenas.HtmlDecode("&nbsp;&nbsp;&nbsp;");

            foreach (CategoriaTesauro categoria in pCategoria.Hijos)
            {
                if (pListaCategoriasOmitir == null || !pListaCategoriasOmitir.Contains(categoria.Clave))
                {
                    CargarCategoriasHijasOmitiendoCatEnLista(categoria, pEspacios, pListaCategoriasOmitir, pListaCats);
                }
            }
        }

        #endregion
    }
}
