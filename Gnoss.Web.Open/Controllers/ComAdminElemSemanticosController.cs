using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    /// <summary>
    /// Controlador para la página de administrar elementos semánticos de la comunidad.
    /// </summary>
    public class ComAdminElemSemanticosController : ControllerBaseWeb
    {
        #region Miembros

        /// <summary>
        /// Modelo de la administración de elementos semánticos.
        /// </summary>
        private ComAdminSemanticElemModel mModelAdmin;

        /// <summary>
        /// Mensaje para el administrador.
        /// </summary>
        private string mMensajeAdmin;

        #region Tesauro semántico

        /// <summary>
        /// Modelo de edición del tesauro.
        /// </summary>
        private EditSemanticThesaurusModel mEditTesModel;

        /// <summary>
        /// DataSet de proyecto con los datos de los tesauros  semánticos cargados.
        /// </summary>
        private DataWrapperProyecto mProyTesSemDS;

        /// <summary>
        /// Entidades del tesauro semántico.
        /// </summary>
        private List<ElementoOntologia> mEntidadesTesSem;

        /// <summary>
        /// IDs con las categorías que deben pintarse expandidas.
        /// </summary>
        private List<string> mCategoriasExpandidasStringIDs = new List<string>();

        /// <summary>
        /// IDs con las categorías que deben pintarse expandidas.
        /// </summary>
        private List<Guid> mCategoriasExpandidasIDs = new List<Guid>();

        /// <summary>
        /// IDs y tres objetos: modelo de categorías de tesauro semántico, Entidad de Categoría y nivel de la categoría.
        /// </summary>
        private Dictionary<string, object[]> mCategoriasModeloTesSem;

        /// <summary>
        /// Propiedades extra que tienen las categorías de un tesauro.
        /// </summary>
        private Dictionary<string, Propiedad> mPropiedadesExtraCategorias;

        #endregion

        #region Entidades secundarias

        /// <summary>
        /// Modelo de edición del tesauro.
        /// </summary>
        private EditSecondaryEntityModel mEditEntSecModel;

        /// <summary>
        /// Ontología secundaría que se está editando.
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// Controlador de edición del SEMCMS.
        /// </summary>
        private SemCmsController mSemController;

        #endregion

        #region Grafos Simples

        /// <summary>
        /// Modelo de edición de grafos simples.
        /// </summary>
        private EditSimpleGraphModel mEditGrafoSimpleModel;

        #endregion

        #endregion

        public ComAdminElemSemanticosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Acciones

        /// <summary>
        /// Acción inicial del controlador.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Index()
        {
            ActionResult redireccion = ComprobarRedirecciones();

            if (redireccion != null)
            {
                return redireccion;
            }

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            mModelAdmin = new ComAdminSemanticElemModel();

            if (RequestParams("tessem") == "true")
            {
                CargarInicial_TesSem();
            }
            else if (RequestParams("entsecund") == "true")
            {
                CargarInicial_EntSecund();
            }
            else if (RequestParams("grafosimple") == "true")
            {
                CargarInicial_GrafoSimple();
            }
            else
            {
                mModelAdmin.PageType = ComAdminSemanticElemModel.ComAdminSemanticElemPage.OntologiesAdmin;
            }

            return View(mModelAdmin);
        }

        /// <summary>
        /// Acción de eliminar un tesauro
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Ontologia"></param>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult EliminarTesauro(string Source, string Ontologia)
        {
            EliminarPersonalizacionVistas();

            try
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                mProyTesSemDS = proyCN.ObtenerTesaurosSemanticosConfigEdicionDeProyecto(ProyectoSeleccionado.Clave);
                //FindByProyectoIDUrlOntologiaSourceTesSem(ProyectoSeleccionado.Clave, Ontologia, Source);
                ProyectoConfigExtraSem filaConfig = mProyTesSemDS.ListaProyectoConfigExtraSem.FirstOrDefault(proy => proy.ProyectoID.Equals(ProyectoSeleccionado.Clave) && proy.UrlOntologia.Equals(Ontologia) && proy.SourceTesSem.Equals(Source));
                mProyTesSemDS.ListaProyectoConfigExtraSem.Remove(filaConfig);
                mEntityContext.EliminarElemento(filaConfig);
                proyCN.ActualizarProyectos();
                proyCN.Dispose();

                FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
                facetadoCL.Dispose();

                return GnossResultOK();
            }
            catch
            {
                return GnossResultERROR();
            }
        }

        /// <summary>
        /// Acción de añadir un tesauro
        /// </summary>
        /// <param name="Nombre"></param>
        /// <param name="Idiomas"></param>
        /// <param name="Ontologia"></param>
        /// <param name="Prefijo"></param>
        /// <param name="Source"></param>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult AddTesauro(string Nombre, string Idiomas, string Ontologia, string Prefijo, string Source)
        {
            EliminarPersonalizacionVistas();

            if (string.IsNullOrEmpty(Ontologia))
            {
                return GnossResultERROR("ONTOLOGIA_EMPTY");
            }
            else if (string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Prefijo) || string.IsNullOrEmpty(Source))
            {
                return GnossResultERROR("CAMPOS_EMPTY");
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mProyTesSemDS = proyCN.ObtenerTesaurosSemanticosConfigEdicionDeProyecto(ProyectoSeleccionado.Clave);
            if (mProyTesSemDS.ListaProyectoConfigExtraSem.FirstOrDefault(proy => proy.ProyectoID.Equals(ProyectoSeleccionado.Clave) && proy.UrlOntologia.Equals(Ontologia) && proy.SourceTesSem.Equals(Source)) != null)
            {
                return GnossResultERROR("SOURCE_REPETIDO");
            }

            ProyectoConfigExtraSem filaConfig = new ProyectoConfigExtraSem();
            filaConfig.ProyectoID = ProyectoSeleccionado.Clave;
            filaConfig.Tipo = 0;
            filaConfig.Editable = true;
            filaConfig.Nombre = Nombre;
            filaConfig.SourceTesSem = Source;
            filaConfig.PrefijoTesSem = Prefijo;
            filaConfig.Idiomas = Idiomas;
            filaConfig.UrlOntologia = Ontologia;

            mProyTesSemDS.ListaProyectoConfigExtraSem.Add(filaConfig);
            if (mEntityContext.ProyectoConfigExtraSem.FirstOrDefault(proy => proy.ProyectoID.Equals(filaConfig.ProyectoID) && proy.UrlOntologia.Equals(filaConfig.UrlOntologia) && proy.SourceTesSem.Equals(filaConfig.SourceTesSem)) == null)
            {
                mEntityContext.ProyectoConfigExtraSem.Add(filaConfig);
            }

            proyCN.ActualizarProyectos();
            proyCN.Dispose();

            Tuple<string, string, string> datosFicha = new Tuple<string, string, string>(UtilCadenas.ObtenerTextoDeIdioma(filaConfig.Nombre, UtilIdiomas.LanguageCode, null), filaConfig.UrlOntologia, filaConfig.SourceTesSem);

            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();

            return PartialView("_EditarTesSemFicha", datosFicha);
        }

        /// <summary>
        /// Acción de modificar el tesauro.
        /// </summary>
        /// <param name="pModel">Modelo de la acción</param>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult EditarTesauro(EditSemanticThesaurusModel pModel)
        {
            EliminarPersonalizacionVistas();
            try
            {
                mEditTesModel = pModel;
                mEditTesModel.ActionsBackUp = HttpUtility.UrlDecode(mEditTesModel.ActionsBackUp);
                mEditTesModel.CategoryExtraPropertiesValues = HttpUtility.UrlDecode(mEditTesModel.CategoryExtraPropertiesValues);
                mEditTesModel.ExtraSemanticPropertiesValuesBK = HttpUtility.UrlDecode(mEditTesModel.ExtraSemanticPropertiesValuesBK);
                ActionResult redireccion = ComprobarRedirecciones();

                if (redireccion != null)
                {
                    return redireccion;
                }

                CargarInicial_TesSem();

                if (!mModelAdmin.SemanticThesaurus.SemanticThesaurusEditables.ContainsKey(new KeyValuePair<string, string>(pModel.OntologyUrl, pModel.SourceSemanticThesaurus)))
                {
                    return GnossResultERROR();
                }

                CargarTesauroBD();
                CargarTesauro();

                EjecutarAccionesBackup((mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.SaveThesaurus));

                if (mEditTesModel.ActionsBackUp == null)
                {
                    mEditTesModel.ActionsBackUp = "";
                }

                if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.LoadThesaurus)
                {
                    return PartialView("_AccionesTesSem", mModelAdmin);
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.CreateCategory)
                {
                    return CrearCategoria();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.ReNameCategory)
                {
                    return RenombrarCategoria();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.PrepareMoveCategories)
                {
                    return PrepararMoverCategorias();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.MoveCategories)
                {
                    return MoverCategorias();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.PrepareOrderCategories)
                {
                    //return PrepararOrdenarCategorias();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.OrderCategories)
                {
                    //return OrdenarCategorias();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.PrepareDeleteCategories)
                {
                    return PrepararEliminarCategorias();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.DeleteCategories)
                {
                    return EliminarCategorias();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.EditExtraProperties)
                {
                    return EditarPropiedadesExtraCategoria();
                }
                else if (mEditTesModel.EditAction == EditSemanticThesaurusModel.Action.SaveThesaurus)
                {
                    //return GuardarCategorias();
                    return GnossResultOK();
                }

                return GnossResultERROR();
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.ToString());
                if (string.IsNullOrEmpty(mMensajeAdmin))
                {
                    return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS") + ".");
                }
                else
                {
                    return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS") + ":" + Environment.NewLine + mMensajeAdmin);
                }
            }
        }

        /// <summary>
        /// Acción de modificar el tesauro.
        /// </summary>
        /// <param name="pModel">Modelo de la acción</param>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult EditarEntSec(EditSecondaryEntityModel pModel)
        {
            try
            {
                mEditEntSecModel = pModel;
                ActionResult redireccion = ComprobarRedirecciones();

                if (redireccion != null)
                {
                    return redireccion;
                }

                CargarInicial_EntSecund();

                if (!mModelAdmin.SecondaryEntities.SecondaryEntitiesEditables.ContainsKey(pModel.OntologyUrl))
                {
                    return GnossResultERROR();
                }

                if (mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.LoadInstances)
                {
                    CargarInstanciasOntologiaSecundaria();
                    return PartialView("_EditarEntSecInstancias", mModelAdmin);
                }
                else if (mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.CreateNewInstance)
                {
                    CrearEditarIntanciaOntologiaSecundaria();
                    return PartialView("_EditarEntSecInstancias", mModelAdmin);
                }
                else if (mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.EditInstance)
                {
                    CrearEditarIntanciaOntologiaSecundaria();
                    return PartialView("_EditarEntSecInstancias", mModelAdmin);
                }
                else if (mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.DeleteInstance)
                {
                    EliminarIntanciasOntologiaSecundaria();
                    return PartialView("_EditarEntSecInstancias", mModelAdmin);
                }
                else if (mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.SaveNewInstance || mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.SaveInstance)
                {
                    string tildes = "[áéíóúöüÁÉÍÓÚÖÜ]";
                    if (pModel.EntitySubject != null)
                    {
                        bool tilde = Regex.IsMatch(pModel.EntitySubject, tildes);
                        if (tilde)
                        {
                            return GnossResultERROR("No se pueden poner acentos");
                        }
                    }
                    return GuardarIntanciaOntologiaSecundaria();
                }

                return GnossResultERROR();
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.ToString());
                if (string.IsNullOrEmpty(mMensajeAdmin))
                {
                    return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS") + ".");
                }
                else
                {
                    return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS") + ":" + Environment.NewLine + mMensajeAdmin);
                }
            }
        }

        /// <summary>
        /// Acción de modificar el tesauro.
        /// </summary>
        /// <param name="pModel">Modelo de la acción</param>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult EditarGrafoSimple(EditSimpleGraphModel pModel)
        {
            try
            {
                mEditGrafoSimpleModel = pModel;
                ActionResult redireccion = ComprobarRedirecciones();

                if (redireccion != null)
                {
                    return redireccion;
                }

                CargarInicial_GrafoSimple();

                if (!mModelAdmin.SimpleGraphs.SimpleGraphsEditables.ContainsKey(pModel.Graph))
                {
                    return GnossResultERROR();
                }

                if (mEditGrafoSimpleModel.EditAction == EditSimpleGraphModel.Action.LoadInstances)
                {
                    CargarInstanciasGrafoSimple();
                    return PartialView("_EditarGrafosSimplesInstancias", mModelAdmin);
                }
                else if (mEditGrafoSimpleModel.EditAction == EditSimpleGraphModel.Action.CreateNewInstance)
                {
                    CrearIntanciaGrafoSimple();
                    return PartialView("_EditarGrafosSimplesInstancias", mModelAdmin);
                }
                else if (mEditGrafoSimpleModel.EditAction == EditSimpleGraphModel.Action.DeleteInstance)
                {
                    EliminarIntanciasGrafoSimple();
                    return PartialView("_EditarGrafosSimplesInstancias", mModelAdmin);
                }

                return GnossResultERROR();
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.ToString());
                if (string.IsNullOrEmpty(mMensajeAdmin))
                {
                    return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS") + ".");
                }
                else
                {
                    return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS") + ":" + Environment.NewLine + mMensajeAdmin);
                }
            }
        }

        #endregion

        #region Metodos auxiliares Editar Tesauro Semántico

        /// <summary>
        /// Acción de mover categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult EliminarCategorias()
        {
            List<string> catSelecc = new List<string>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                catSelecc.Add(catTesID);
            }

            if (catSelecc.Count == 0)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            if (string.IsNullOrEmpty(mEditTesModel.SelectedCategory))
            {
                return GnossResultERROR();
            }

            if (catSelecc.Count != 1)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            string categoriaSustitutaID = mEditTesModel.SelectedCategory;


            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, (short)EditThesaurusPersonalSpaceModel.Action.DeleteCategories, "|_|", categoriaSustitutaID.ToString());

            foreach (string idCatEliminar in catSelecc)
            {
                mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|_|", idCatEliminar.ToString());
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|,|");

            EliminarCategoriasTesauro(false, categoriaSustitutaID, catSelecc);

            mCategoriasExpandidasStringIDs.Add(categoriaSustitutaID);

            CargarTesauro();
            mModelAdmin.SemanticThesaurus.ActionsBackUp = mEditTesModel.ActionsBackUp;
            mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();
            return PartialView("_AccionesTesSem", mModelAdmin);
        }

        /// <summary>
        /// Eliminar una serie de categorías del tesauro.
        /// </summary>
        /// <param name="pGuardarEnBD">Indica si hay que llevar los cambios a la BD</param>
        /// <param name="pIDCategoriaVincular">Catetgoría nueva a la que se vincularán los elementos</param>
        /// <param name="pListaCategoriasAEliminar">Lista de categorías a eliminar</param>
        private void EliminarCategoriasTesauro(bool pGuardarEnBD, string pIDCategoriaVincular, List<string> pListaCategoriasAEliminar)
        {
            CategoryModel catPadreM = (CategoryModel)mCategoriasModeloTesSem[pIDCategoriaVincular][0];
            List<string> pathPadre = GenerarPathPadresCategorias(catPadreM);

            foreach (string catID in pListaCategoriasAEliminar)
            {
                CategoryModel catHijaM = (CategoryModel)mCategoriasModeloTesSem[catID][0];
                ElementoOntologia catHija = (ElementoOntologia)mCategoriasModeloTesSem[catID][1];

                if (catHijaM.ParentCategoryKey == Guid.Empty)
                {
                    mEntidadesTesSem[0].ObtenerPropiedad(EstiloPlantilla.Member_TesSem).LimpiarValor(catHija.ID);
                }
                else
                {
                    ((ElementoOntologia)mCategoriasModeloTesSem[catHijaM.ParentCategoryStringKey][1]).ObtenerPropiedad(EstiloPlantilla.Narrower_TesSem).LimpiarValor(catHija.ID);
                    catHija.ObtenerPropiedad(EstiloPlantilla.Broader_TesSem).LimpiarValor();
                }
            }

            if (pGuardarEnBD)
            {
                Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(mEditTesModel.OntologyUrl, ProyectoSeleccionado.Clave);

                foreach (string catEliminar in pListaCategoriasAEliminar)
                {
                    ControladorDocumentacion.EliminarCategoriaTesauroSemantico(UrlIntragnoss + mEditTesModel.OntologyUrl, null, UrlIntragnoss, BaseURLFormulariosSem, catEliminar, pathPadre.ToArray(), docOnto, null, false, ProyectoSeleccionado.Clave);
                }
            }
        }

        /// <summary>
        /// Prepara la acción de mover categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult PrepararEliminarCategorias()
        {
            mModelAdmin.SemanticThesaurus.CategoryNamesToDelete = new List<string>();
            List<string> catSelecc = new List<string>();
            List<Guid> catGuidSelecc = new List<Guid>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                CategoryModel categoriaM = (CategoryModel)mCategoriasModeloTesSem[catTesID][0];
                catGuidSelecc.Add(categoriaM.Key);
                catSelecc.Add(catTesID);
                mCategoriasExpandidasStringIDs.Add(catTesID);
                mModelAdmin.SemanticThesaurus.CategoryNamesToDelete.Add(categoriaM.Name);

                if (((ElementoOntologia)mCategoriasModeloTesSem[catTesID][1]).ObtenerPropiedad(EstiloPlantilla.Narrower_TesSem).ValoresUnificados.Count > 0)
                {
                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_HOJA_ELIMINAR"));
                }
            }

            if (catSelecc.Count != 1)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            mModelAdmin.SemanticThesaurus.ParentCategoriesForDeleteCategories = new Dictionary<string, string>();
            mModelAdmin.SemanticThesaurus.ParentCategoriesForDeleteCategories.Add("", UtilIdiomas.GetText("PERFILBASE", "SELECCIONACAT"));

            foreach (ElementoOntologia entidadPrinc in mEntidadesTesSem)
            {
                Propiedad propMember = entidadPrinc.ObtenerPropiedad(EstiloPlantilla.Member_TesSem);
                foreach (ElementoOntologia categoria in CategoriasTesauroSemOrdenadasTesSem(new List<ElementoOntologia>(propMember.ValoresUnificados.Values)))
                {
                    CargarCategoriasHijasOmitiendoCatEnLista(categoria, string.Empty, catSelecc, mModelAdmin.SemanticThesaurus.ParentCategoriesForDeleteCategories);
                }
            }

            //CargarTesauro(); NO CARGAR, CAMBIARÍAN IDs
            mModelAdmin.SemanticThesaurus.ThesaurusEditorModel.SelectedCategories = catGuidSelecc;
            mModelAdmin.SemanticThesaurus.ActionsBackUp = mEditTesModel.ActionsBackUp;
            mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();
            return PartialView("_AccionesTesSem", mModelAdmin);
        }

        /// <summary>
        /// Acción de mover categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult MoverCategorias()
        {
            List<string> catSelecc = new List<string>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                catSelecc.Add(catTesID);
            }

            if (catSelecc.Count == 0)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            if (string.IsNullOrEmpty(mEditTesModel.SelectedCategory))
            {
                return GnossResultERROR();
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, (short)mEditTesModel.EditAction, "|_|", mEditTesModel.SelectedCategory);


            List<Guid> IDCatSeleccionadas = new List<Guid>();

            foreach (string catTesID in catSelecc)
            {
                mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|_|", catTesID.ToString());
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, "|,|");

            MoverCategoriasTesauro(false, mEditTesModel.SelectedCategory, catSelecc);

            mCategoriasExpandidasStringIDs.Add(mEditTesModel.SelectedCategory);

            CargarTesauro();

            mModelAdmin.SemanticThesaurus.ActionsBackUp = mEditTesModel.ActionsBackUp;
            mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();
            return PartialView("_AccionesTesSem", mModelAdmin);
        }

        /// <summary>
        /// Mueve una serie de categorías del tesauro.
        /// </summary>
        /// <param name="pGuardarEnBD">Indica si hay que llevar los cambios a la BD</param>
        /// <param name="pCategoriaPadre">Nueva categoría padre</param>
        /// <param name="pCategoriasAMover">Categorías que movemos</param>
        private void MoverCategoriasTesauro(bool pGuardarEnBD, string pCategoriaPadre, List<string> pCategoriasAMover)
        {
            List<string> pathPadre = null;
            CategoryModel catPadreM = null;

            if (pCategoriaPadre == "[RAIZ]")
            {
                pathPadre = new List<string>();
            }
            else
            {
                catPadreM = (CategoryModel)mCategoriasModeloTesSem[pCategoriaPadre][0];
                pathPadre = GenerarPathPadresCategorias(catPadreM);
            }

            foreach (string catID in pCategoriasAMover)
            {
                CategoryModel catHijaM = (CategoryModel)mCategoriasModeloTesSem[catID][0];
                ElementoOntologia catHija = (ElementoOntologia)mCategoriasModeloTesSem[catID][1];

                if (catHijaM.ParentCategoryKey == Guid.Empty)
                {
                    mEntidadesTesSem[0].ObtenerPropiedad(EstiloPlantilla.Member_TesSem).LimpiarValor(catHija.ID);
                }
                else
                {
                    ((ElementoOntologia)mCategoriasModeloTesSem[catHijaM.ParentCategoryStringKey][1]).ObtenerPropiedad(EstiloPlantilla.Narrower_TesSem).LimpiarValor(catHija.ID);
                    catHija.ObtenerPropiedad(EstiloPlantilla.Broader_TesSem).LimpiarValor();
                }

                catHija.ObtenerPropiedad(EstiloPlantilla.Symbol_TesSem).LimpiarValor();


                if (pCategoriaPadre == "[RAIZ]")
                {
                    catHijaM.ParentCategoryKey = Guid.Empty;
                    catHijaM.ParentCategoryStringKey = null;

                    mEntidadesTesSem[0].ObtenerPropiedad(EstiloPlantilla.Member_TesSem).AgregarValor(catHija);
                    catHija.ObtenerPropiedad(EstiloPlantilla.Symbol_TesSem).AgregarValor("1");
                }
                else
                {
                    if (catPadreM != null)
                    {
                        catHijaM.ParentCategoryKey = catPadreM.Key;
                        catHijaM.ParentCategoryStringKey = catPadreM.StringKey;

                        ElementoOntologia catPadre = (ElementoOntologia)mCategoriasModeloTesSem[catHijaM.ParentCategoryStringKey][1];
                        catPadre.ObtenerPropiedad(EstiloPlantilla.Narrower_TesSem).AgregarValor(catHija);
                        catHija.ObtenerPropiedad(EstiloPlantilla.Broader_TesSem).AgregarValor(catPadre);
                        catHija.ObtenerPropiedad(EstiloPlantilla.Symbol_TesSem).AgregarValor((pathPadre.Count + 1).ToString());
                    }
                }
            }

            if (pGuardarEnBD)
            {
                Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(mEditTesModel.OntologyUrl, ProyectoSeleccionado.Clave);

                foreach (string catMover in pCategoriasAMover)
                {
                    ControladorDocumentacion.MoverCategoriaTesauroSemantico(UrlIntragnoss + mEditTesModel.OntologyUrl, null, UrlIntragnoss, catMover, pathPadre.ToArray(), docOnto, null, false, ProyectoSeleccionado.Clave);
                }
            }
        }

        /// <summary>
        /// Genera una lista con la jerarquia de padres de una categoría.
        /// </summary>
        /// <param name="pCatPadre">Categoría</param>
        /// <returns>Lista con la jerarquia de padres de una categoría</returns>
        private List<string> GenerarPathPadresCategorias(CategoryModel pCatPadre)
        {
            List<string> pathPadre = null;

            if (pCatPadre.ParentCategoryKey != Guid.Empty)
            {
                pathPadre = GenerarPathPadresCategorias((CategoryModel)mCategoriasModeloTesSem[pCatPadre.ParentCategoryStringKey][0]);
            }
            else
            {
                pathPadre = new List<string>();
            }

            pathPadre.Add(pCatPadre.StringKey);

            return pathPadre;
        }

        /// <summary>
        /// Prepara la acción de mover categorías.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult PrepararMoverCategorias()
        {
            mModelAdmin.SemanticThesaurus.CategoryNamesToMove = new List<string>();
            List<string> catSelecc = new List<string>();
            List<Guid> catGuidSelecc = new List<Guid>();

            foreach (string catTesID in mEditTesModel.SelectedCategories.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                CategoryModel categoriaM = (CategoryModel)mCategoriasModeloTesSem[catTesID][0];
                catGuidSelecc.Add(categoriaM.Key);
                catSelecc.Add(catTesID);
                mCategoriasExpandidasStringIDs.Add(catTesID);
                mModelAdmin.SemanticThesaurus.CategoryNamesToMove.Add(categoriaM.Name);
            }

            if (catSelecc.Count != 1)
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT"));
            }

            mModelAdmin.SemanticThesaurus.ParentCategoriesForMoveCategories = new Dictionary<string, string>();
            mModelAdmin.SemanticThesaurus.ParentCategoriesForMoveCategories.Add("[RAIZ]", UtilIdiomas.GetText("TESAURO", "INDICE"));

            foreach (ElementoOntologia entidadPrinc in mEntidadesTesSem)
            {
                Propiedad propMember = entidadPrinc.ObtenerPropiedad(EstiloPlantilla.Member_TesSem);
                foreach (ElementoOntologia categoria in CategoriasTesauroSemOrdenadasTesSem(new List<ElementoOntologia>(propMember.ValoresUnificados.Values)))
                {
                    CargarCategoriasHijasOmitiendoCatEnLista(categoria, string.Empty, catSelecc, mModelAdmin.SemanticThesaurus.ParentCategoriesForMoveCategories);
                }
            }

            //CargarTesauro(); NO CARGAR, CAMBIARÍAN IDs
            mModelAdmin.SemanticThesaurus.ThesaurusEditorModel.SelectedCategories = catGuidSelecc;
            mModelAdmin.SemanticThesaurus.ActionsBackUp = mEditTesModel.ActionsBackUp;
            mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();
            return PartialView("_AccionesTesSem", mModelAdmin);
        }

        /// <summary>
        /// Gestiona el cambio de nombre de una categoría.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult RenombrarCategoria()
        {
            string catSeleccID = mEditTesModel.SelectedCategory;

            if (string.IsNullOrEmpty(mEditTesModel.NewCategoryName))
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_NOMBRE"));
            }

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, ((short)mEditTesModel.EditAction).ToString(), "|_|", catSeleccID.ToString(), "|_|", mEditTesModel.NewCategoryName, "|,|");

            RenombrarCategoria(false, catSeleccID, mEditTesModel.NewCategoryName);
            mCategoriasExpandidasStringIDs.Add(catSeleccID);
            CargarTesauro();

            mModelAdmin.SemanticThesaurus.ActionsBackUp = mEditTesModel.ActionsBackUp;
            mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();
            return PartialView("_AccionesTesSem", mModelAdmin);
        }

        /// <summary>
        /// Renombra una categoria.
        /// </summary>
        /// <param name="pGuardarEnBD">Indica si hay que llevar los cambios a la BD</param>
        /// <param name="pIdCategoria">ID de la categoría</param>
        /// <param name="pNombreCategoria">Nombre de la categoría</param>
        private void RenombrarCategoria(bool pGuardarEnBD, string pIdCategoria, string pNombreCategoria)
        {
            ElementoOntologia categoriaSel = (ElementoOntologia)mCategoriasModeloTesSem[pIdCategoria][1];
            Propiedad propNombre = categoriaSel.ObtenerPropiedad(EstiloPlantilla.PrefLabel_TesSem);
            propNombre.LimpiarValor();

            if (pNombreCategoria.Contains("|||") && pNombreCategoria.Contains("@"))
            {
                Dictionary<string, string> idiomaValor = UtilCadenas.ObtenerTextoPorIdiomas(pNombreCategoria);

                foreach (string idioma in idiomaValor.Keys)
                {
                    propNombre.AgregarValorConIdioma(idiomaValor[idioma], idioma);
                }
            }
            else
            {
                propNombre.AgregarValor(pNombreCategoria);
            }

            if (pGuardarEnBD)
            {
                Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(mEditTesModel.OntologyUrl, ProyectoSeleccionado.Clave);
                ControladorDocumentacion.RenombrarCategoriaTesauroSemantico(UrlIntragnoss + mEditTesModel.OntologyUrl, UrlIntragnoss, pIdCategoria, pNombreCategoria, docOnto, false, ProyectoSeleccionado.Clave);
                docOnto.GestorDocumental.Dispose();
            }
        }

        /// <summary>
        /// Gestiona el cambio de las propiedades extra de una categoría.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult EditarPropiedadesExtraCategoria()
        {
            string catSeleccID = mEditTesModel.SelectedCategory;

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, ((short)mEditTesModel.EditAction).ToString(), "|_|", catSeleccID, "|_|", mEditTesModel.CategoryExtraPropertiesValues, "|,|");

            if (!string.IsNullOrEmpty(mEditTesModel.CategoryExtraPropertiesValues))
            {
                if (mEditTesModel.ExtraSemanticPropertiesValuesBK.Contains(catSeleccID + "|"))
                {
                    string extraAux = mEditTesModel.ExtraSemanticPropertiesValuesBK.Substring(0, mEditTesModel.ExtraSemanticPropertiesValuesBK.IndexOf(catSeleccID + "|"));
                    mEditTesModel.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK.Substring(mEditTesModel.ExtraSemanticPropertiesValuesBK.IndexOf(catSeleccID + "|"));
                    mEditTesModel.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK.Substring(mEditTesModel.ExtraSemanticPropertiesValuesBK.IndexOf("[|||]") + 5);
                    mEditTesModel.ExtraSemanticPropertiesValuesBK = extraAux + mEditTesModel.ExtraSemanticPropertiesValuesBK;
                }

                mEditTesModel.ExtraSemanticPropertiesValuesBK = string.Concat(mEditTesModel.ExtraSemanticPropertiesValuesBK, catSeleccID, "|", mEditTesModel.CategoryExtraPropertiesValues, "[|||]");
            }

            EditarPropiedadesExtraCategoria(false, catSeleccID, mEditTesModel.CategoryExtraPropertiesValues);
            mCategoriasExpandidasStringIDs.Add(catSeleccID);
            CargarTesauro();

            mModelAdmin.SemanticThesaurus.ActionsBackUp = mEditTesModel.ActionsBackUp;
            mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();
            return PartialView("_AccionesTesSem", mModelAdmin);
        }

        /// <summary>
        /// Edita las propiedades extra de una categoria.
        /// </summary>
        /// <param name="pGuardarEnBD">Indica si hay que llevar los cambios a la BD</param>
        /// <param name="pIdCategoria">ID de la categoría</param>
        /// <param name="pCategoryExtraPropertiesValues">Valor de las propiedades extra de la categoría</param>
        private void EditarPropiedadesExtraCategoria(bool pGuardarEnBD, string pIdCategoria, string pCategoryExtraPropertiesValues)
        {
            ElementoOntologia categoriaSel = (ElementoOntologia)mCategoriasModeloTesSem[pIdCategoria][1];
            List<string> propsExtra = ExtraerValoresExtraPropsCategorias(categoriaSel, pCategoryExtraPropertiesValues);

            if (pGuardarEnBD)
            {
                Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(mEditTesModel.OntologyUrl, ProyectoSeleccionado.Clave);
                ControladorDocumentacion.EditarPropiedadesExtraCategoriaTesauroSemantico(UrlIntragnoss + mEditTesModel.OntologyUrl, UrlIntragnoss, pIdCategoria, categoriaSel, propsExtra, docOnto, false, ProyectoSeleccionado.Clave);
                docOnto.GestorDocumental.Dispose();
            }
        }

        /// <summary>
        /// Gestiona la creación de una nueva categoría controlando que no exista ya otra con ese nombre y añadiendo la acción a la lista de acciones.
        /// </summary>
        /// <returns>Acción de respuesta</returns>
        private ActionResult CrearCategoria()
        {
            if (string.IsNullOrEmpty(mEditTesModel.NewCategoryName) || string.IsNullOrEmpty(mEditTesModel.SelectedCategory) || string.IsNullOrEmpty(mEditTesModel.NewCategoryIdentifier))
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_NOMBRE"));
            }

            Uri uriAux = null;
            if (mEditTesModel.NewCategoryIdentifier.Contains(" ") || mEditTesModel.NewCategoryIdentifier.Contains(":") || mEditTesModel.NewCategoryIdentifier.Contains("/") || mEditTesModel.NewCategoryIdentifier.Contains("|") || !Uri.TryCreate(UrlIntragnoss + mEditTesModel.NewCategoryIdentifier, UriKind.RelativeOrAbsolute, out uriAux))
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "FORMATOURIINCORRECTO", mEditTesModel.NewCategoryIdentifier));
            }

            ProyectoConfigExtraSem filaConfig = ObtenerFilaConfigExtraSemActual();
            string prefijo = filaConfig.PrefijoTesSem;

            if (prefijo[prefijo.Length - 1] != '_')
            {
                prefijo += "_";
            }

            string identificadorNuevaCat = UrlIntragnoss + "items/" + prefijo + mEditTesModel.NewCategoryIdentifier;
            //FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD);
            //List<string> propiedades = new List<string>();
            //propiedades.Add(EstiloPlantilla.Identifier_TesSem);//identifier
            //FacetadoDS dataSetCategorias = facCN.ObtenerValoresPropiedadesEntidad(mEditTesModel.OntologyUrl, identificadorNuevaCat, propiedades);
            //facCN.Dispose();

            //if (dataSetCategorias.Tables[0].Rows.Count > 0)
            if (mCategoriasModeloTesSem.ContainsKey(identificadorNuevaCat))
            {
                return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_ID_DIFERENTE"));
            }

            //dataSetCategorias.Dispose();

            mEditTesModel.ActionsBackUp = string.Concat(mEditTesModel.ActionsBackUp, ((short)mEditTesModel.EditAction).ToString(), "|_|", mEditTesModel.SelectedCategory, "|_|", mEditTesModel.NewCategoryName, "|_|", identificadorNuevaCat, "|_|", mEditTesModel.CategoryExtraPropertiesValues, "|,|");

            if (!string.IsNullOrEmpty(mEditTesModel.CategoryExtraPropertiesValues))
            {
                mEditTesModel.ExtraSemanticPropertiesValuesBK = string.Concat(mEditTesModel.ExtraSemanticPropertiesValuesBK, identificadorNuevaCat, "|", mEditTesModel.CategoryExtraPropertiesValues, "[|||]");
            }

            CrearCategoriaTesauro(mEditTesModel.SelectedCategory, identificadorNuevaCat, mEditTesModel.NewCategoryName.Trim(), false, prefijo, mEditTesModel.CategoryExtraPropertiesValues);
            mCategoriasExpandidasStringIDs.Add(identificadorNuevaCat);
            CargarTesauro();

            mModelAdmin.SemanticThesaurus.ActionsBackUp = mEditTesModel.ActionsBackUp;
            mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = mEditTesModel.ExtraSemanticPropertiesValuesBK;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();
            return PartialView("_AccionesTesSem", mModelAdmin);
        }

        /// <summary>
        /// Crea una categoría en el tesauro
        /// </summary>
        /// <param name="pIdCategoriaPadre">Id de la categoría padre en la que se crea la nueva categoría</param>
        /// <param name="pIdNuevaCategoria">Id de la nueva categoría</param>
        /// <param name="pNombreCategoria">Nombre de la nueva categoría</param>
        /// <param name="pGuardarEnBD">Indica si se debe guardar en la BD</param>
        private void CrearCategoriaTesauro(string pIdCategoriaPadre, string pIdNuevaCategoria, string pNombreCategoria, bool pGuardarEnBD, string pPrefijoCategoria, string pExtraPropertiesValues)
        {
            ElementoOntologia nuevaCat = mEntidadesTesSem[0].Ontologia.GetEntidadTipo(EstiloPlantilla.Concept_TesSem, true);
            nuevaCat.ID = pIdNuevaCategoria.Substring(pIdNuevaCategoria.LastIndexOf("/") + 1);

            if (pIdCategoriaPadre == "[RAIZ]")
            {
                Propiedad member = mEntidadesTesSem[0].ObtenerPropiedad(EstiloPlantilla.Member_TesSem);
                member.ListaValores.Add(nuevaCat.ID, nuevaCat);
                mEntidadesTesSem[0].EntidadesRelacionadas.Add(nuevaCat);

                nuevaCat.ObtenerPropiedad(EstiloPlantilla.Symbol_TesSem).AgregarValor("1");
            }
            else
            {
                ElementoOntologia categoriaPadre = (ElementoOntologia)mCategoriasModeloTesSem[pIdCategoriaPadre][1];
                Propiedad propHijo = categoriaPadre.ObtenerPropiedad(EstiloPlantilla.Narrower_TesSem);
                propHijo.ListaValores.Add(nuevaCat.ID, nuevaCat);
                categoriaPadre.EntidadesRelacionadas.Add(nuevaCat);

                nuevaCat.ObtenerPropiedad(EstiloPlantilla.Broader_TesSem).AgregarValor(pIdCategoriaPadre);
                nuevaCat.ObtenerPropiedad(EstiloPlantilla.Symbol_TesSem).AgregarValor(((int)mCategoriasModeloTesSem[pIdCategoriaPadre][2] + 1).ToString());
            }

            //nuevaCat.ObtenerPropiedad(EstiloPlantilla.Identifier_TesSem).AgregarValor(pIdNuevaCategoria.Substring(pIdNuevaCategoria.LastIndexOf("_") + 1));
            nuevaCat.ObtenerPropiedad(EstiloPlantilla.Identifier_TesSem).AgregarValor(pIdNuevaCategoria.Substring(pIdNuevaCategoria.LastIndexOf(pPrefijoCategoria) + pPrefijoCategoria.Length));

            if (pNombreCategoria.Contains("|||") && pNombreCategoria.Contains("@"))
            {
                Dictionary<string, string> idiomaValor = UtilCadenas.ObtenerTextoPorIdiomas(pNombreCategoria);
                Propiedad propNombre = nuevaCat.ObtenerPropiedad(EstiloPlantilla.PrefLabel_TesSem);

                foreach (string idioma in idiomaValor.Keys)
                {
                    propNombre.AgregarValorConIdioma(idiomaValor[idioma], idioma);
                }
            }
            else
            {
                nuevaCat.ObtenerPropiedad(EstiloPlantilla.PrefLabel_TesSem).AgregarValor(pNombreCategoria);
            }

            nuevaCat.ObtenerPropiedad(EstiloPlantilla.Source_TesSem).AgregarValor(mEditTesModel.SourceSemanticThesaurus);

            List<string> propsExtra = ExtraerValoresExtraPropsCategorias(nuevaCat, pExtraPropertiesValues);

            if (pGuardarEnBD)
            {
                List<string> padres = new List<string>();

                if (pIdCategoriaPadre != "[RAIZ]")
                {
                    padres.Add(pIdCategoriaPadre);
                }

                Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(mEditTesModel.OntologyUrl, ProyectoSeleccionado.Clave);
                ControladorDocumentacion.CrearCategoriaTesauroSemantico(UrlIntragnoss + mEditTesModel.OntologyUrl, UrlIntragnoss, nuevaCat, padres, docOnto, false, propsExtra, ProyectoSeleccionado.Clave);
                docOnto.GestorDocumental.Dispose();
            }
        }

        /// <summary>
        /// Extrae los valores de las propiedades extra que tengan las categorías.
        /// </summary>
        /// <param name="pCategorias">Categoría editada</param>
        /// <param name="pExtraPropertiesValues">Valores de las propiedades extra</param>
        /// <returns>Nombre de las propiedades extra</returns>
        private List<string> ExtraerValoresExtraPropsCategorias(ElementoOntologia pCategorias, string pExtraPropertiesValues)
        {
            if (string.IsNullOrEmpty(pExtraPropertiesValues))
            {
                return null;
            }

            List<string> propsExtra = new List<string>();

            foreach (string propValor in pExtraPropertiesValues.Split(new string[] { "[||]" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string propSem = propValor.Split('|')[0];
                Propiedad propiedad = pCategorias.ObtenerPropiedad(propSem);

                if (propiedad != null)
                {
                    if (!propsExtra.Contains(propSem))
                    {
                        propsExtra.Add(propSem);
                        propiedad.LimpiarValor();
                    }

                    string valor = propValor.Substring(propValor.IndexOf("|") + 1);

                    if (valor.EndsWith("|||") && valor.Contains("@"))
                    {
                        string idioma = valor.Substring(valor.LastIndexOf("@") + 1);
                        idioma = idioma.Replace("|||", "");
                        valor = valor.Substring(0, valor.LastIndexOf("@"));

                        if (!string.IsNullOrEmpty(valor))
                        {
                            propiedad.AgregarValorConIdioma(valor, idioma);
                        }
                    }
                    else if (!string.IsNullOrEmpty(valor))
                    {
                        propiedad.AgregarValor(valor);
                    }
                }
            }

            return propsExtra;
        }

        /// <summary>
        /// Ejecuta todas las acciones que se van realizando sobre el tesauro anteriormente.
        /// </summary>
        /// <param name="pGuardarEnBD">Indica si se debe guardar en la BD</param>
        private void EjecutarAccionesBackup(bool pGuardarEnBD)
        {
            //Si hay acciones a realizar, se realizan
            if (!string.IsNullOrEmpty(mEditTesModel.ActionsBackUp))
            {
                ProyectoConfigExtraSem filaConfig = ObtenerFilaConfigExtraSemActual();
                string prefijo = filaConfig.PrefijoTesSem;

                if (prefijo[prefijo.Length - 1] != '_')
                {
                    prefijo += "_";
                }

                //Obtenemos las acciones
                string[] acciones = mEditTesModel.ActionsBackUp.Split(new string[] { "|,|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string accion in acciones)
                {
                    string[] accion2 = accion.Split(new string[] { "|_|" }, StringSplitOptions.RemoveEmptyEntries);
                    short tipoAccion = short.Parse(accion2[0]);

                    if ((short)EditThesaurusPersonalSpaceModel.Action.CreateCategory == tipoAccion)
                    {//crear
                        string idPadre = accion2[1];
                        string idNuevaCategoria = accion2[3];
                        string nombreNuevaCat = accion2[2];
                        string extraPropValores = null;

                        if (accion2.Length > 4)
                        {
                            extraPropValores = accion2[4];
                        }

                        CrearCategoriaTesauro(idPadre, idNuevaCategoria, nombreNuevaCat.Trim(), pGuardarEnBD, prefijo, extraPropValores);
                        CargarTesauro();
                    }
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.ReNameCategory == tipoAccion)
                    {//cambiarnombre
                        string idCategoria = accion2[1];
                        string nombreCategoria = accion2[2];
                        RenombrarCategoria(pGuardarEnBD, idCategoria, nombreCategoria);
                    }
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.MoveCategories == tipoAccion)
                    {//MoverCategorias
                        string idNuevoPadre = accion2[1];
                        List<string> idCat = new List<string>();
                        for (int i = 2; i < accion2.Length; i++)
                        {
                            idCat.Add(accion2[i]);
                        }
                        MoverCategoriasTesauro(pGuardarEnBD, idNuevoPadre, idCat);
                    }
                    //NO BORRAR, EN BREVE SE VA A EVOLUCIONAR LA PÁGINA PARA QUE USE ÉSTO
                    //else if ((short)EditThesaurusPersonalSpaceModel.Action.OrderCategories == tipoAccion)
                    //{//OrdenarCategoria
                    //    int ordenDestino = int.Parse(accion2[1]);
                    //    List<Guid> idCategoriasOrdenar = new List<Guid>();
                    //    for (int i = 2; i < accion2.Length; i++)
                    //    {
                    //        idCategoriasOrdenar.Add(new Guid(accion2[i]));
                    //    }
                    //    //OrdenarCategoriasTesauro(ordenDestino, idCategoriasOrdenar);
                    //}
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.DeleteCategories == tipoAccion)
                    {//EliminarCategoria                        
                        string idCategoriaVincular = accion2[1];
                        List<string> idCategorias = new List<string>();
                        for (int i = 2; i < accion2.Length; i++)
                        {
                            idCategorias.Add(accion2[i]);
                        }
                        EliminarCategoriasTesauro(pGuardarEnBD, idCategoriaVincular, idCategorias);
                    }
                    else if ((short)EditThesaurusPersonalSpaceModel.Action.EditExtraProperties == tipoAccion)
                    {//cambiarnombre
                        string idCategoria = accion2[1];
                        string extraProp = accion2[2];
                        EditarPropiedadesExtraCategoria(pGuardarEnBD, idCategoria, extraProp);
                    }
                }
            }
        }

        /// <summary>
        /// Establece diversos parámetros del tesauro.
        /// </summary>
        private void EstablecerParametrosTesauro()
        {
            ProyectoConfigExtraSem filaConfig = ObtenerFilaConfigExtraSemActual();
            mModelAdmin.SemanticThesaurus.SemThesaurusLanguajes = new Dictionary<string, string>();

            Dictionary<string, string> idiomasEntorno = mConfigService.ObtenerListaIdiomasDictionary();

            if (!string.IsNullOrEmpty(filaConfig.Idiomas))
            {
                foreach (string idioma in filaConfig.Idiomas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (idiomasEntorno.ContainsKey(idioma))
                    {
                        mModelAdmin.SemanticThesaurus.SemThesaurusLanguajes.Add(idioma, idiomasEntorno[idioma]);
                    }
                }
            }

            mModelAdmin.SemanticThesaurus.OntologyUrl = mEditTesModel.OntologyUrl;
            mModelAdmin.SemanticThesaurus.SourceSemanticThesaurus = mEditTesModel.SourceSemanticThesaurus;
        }

        /// <summary>
        /// Obtiene la fila de configuración de elemento semánticos que se está usando ahora.
        /// </summary>
        /// <returns>Fila de configuración de elemento semánticos que se está usando ahora</returns>
        private ProyectoConfigExtraSem ObtenerFilaConfigExtraSemActual()
        {
            //Select("UrlOntologia='" + mEditTesModel.OntologyUrl + "' AND SourceTesSem='" + mEditTesModel.SourceSemanticThesaurus + "'")[0];
            ProyectoConfigExtraSem filaConfig = mProyTesSemDS.ListaProyectoConfigExtraSem.FirstOrDefault(proy => proy.UrlOntologia.Equals(mEditTesModel.OntologyUrl) && proy.SourceTesSem.Equals(mEditTesModel.SourceSemanticThesaurus));

            return filaConfig;
        }

        /// <summary>
        /// Carga el tesauro del perfil de la base de datos.
        /// </summary>
        private void CargarTesauroBD()
        {
            try
            {
                mEntidadesTesSem = ControladorDocumentacion.ObtenerEntidadesTesauroSemantico(mEditTesModel.OntologyUrl, mEditTesModel.SourceSemanticThesaurus, UrlIntragnoss, ProyectoSeleccionado.Clave, BaseURLFormulariosSem, UtilIdiomas.LanguageCode);
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.ToString());
                mMensajeAdmin = "La ontología del tesauro semántico no es correcta.";
                throw new Exception(mMensajeAdmin);
            }
        }

        private void ExtraerPropiedadesExtraCategoria()
        {
            ElementoOntologia entidadCat = mEntidadesTesSem[0].Ontologia.GetEntidadTipo(EstiloPlantilla.Concept_TesSem, false);
            mPropiedadesExtraCategorias = new Dictionary<string, Propiedad>();

            foreach (Propiedad propiedad in entidadCat.Propiedades)
            {
                if (propiedad.Nombre != EstiloPlantilla.Identifier_TesSem && propiedad.Nombre != EstiloPlantilla.Source_TesSem && propiedad.Nombre != EstiloPlantilla.Broader_TesSem && propiedad.Nombre != EstiloPlantilla.Narrower_TesSem && propiedad.Nombre != EstiloPlantilla.PrefLabel_TesSem && propiedad.Nombre != EstiloPlantilla.Symbol_TesSem)
                {
                    mPropiedadesExtraCategorias.Add(propiedad.Nombre, propiedad);
                    propiedad.ElementoOntologia = entidadCat;
                }
            }

            mModelAdmin.SemanticThesaurus.ThesaurusEditorModel.ExtraPropertiesCategories = mPropiedadesExtraCategorias;
        }

        /// <summary>
        /// Carga el tesauro del perfil.
        /// </summary>
        private void CargarTesauro()
        {
            try
            {
                mModelAdmin.SemanticThesaurus.ThesaurusEditorModel = new ThesaurusEditorModel();
                mModelAdmin.SemanticThesaurus.ThesaurusEditorModel.HideTreeListSelector = true;
                ExtraerPropiedadesExtraCategoria();
                mModelAdmin.SemanticThesaurus.ThesaurusEditorModel.ThesaurusCategories = CargarTesauroPorTesauroSemantico(mEntidadesTesSem);
                mModelAdmin.SemanticThesaurus.ThesaurusEditorModel.SelectedCategories = new List<Guid>();
                mModelAdmin.SemanticThesaurus.ThesaurusEditorModel.ExpandedCategories = mCategoriasExpandidasIDs;
                //DeshabilidatarCategoriasEspeciales();
                //RecargarComboPadresParaCreacionCategoria();
                EstablecerParametrosTesauro();
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.ToString());
                mMensajeAdmin = "La ontología del tesauro semántico no es correcta.";
                throw new Exception(mMensajeAdmin);
            }
        }

        /// <summary>
        /// Genera un modelo de tesauro a partir de las entidaes de un tesauro semántico.
        /// </summary>
        /// <param name="pEntidadesTesSem">Entidaes de un tesauro semántico</param>
        /// <returns>Modelo de tesauro a partir de las entidaes de un tesauro semántico</returns>
        private List<CategoryModel> CargarTesauroPorTesauroSemantico(List<ElementoOntologia> pEntidadesTesSem)
        {
            List<CategoryModel> categoryModel = new List<CategoryModel>();
            mCategoriasModeloTesSem = new Dictionary<string, object[]>();
            mModelAdmin.SemanticThesaurus.ParentCategoriesForCreateNewsCategories = new Dictionary<string, string>();
            mModelAdmin.SemanticThesaurus.ParentCategoriesForCreateNewsCategories.Add("[RAIZ]", UtilIdiomas.GetText("TESAURO", "INDICE"));
            mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = "";

            foreach (ElementoOntologia entidadPrinc in pEntidadesTesSem)
            {
                short orden = 0;
                Propiedad propMember = entidadPrinc.ObtenerPropiedad(EstiloPlantilla.Member_TesSem);
                foreach (ElementoOntologia categoria in CategoriasTesauroSemOrdenadasTesSem(new List<ElementoOntologia>(propMember.ValoresUnificados.Values)))
                {
                    CargarCategoriaTesSem(categoria, orden, 1, null, categoryModel);
                    CargarCategoriasHijasOmitiendoCatEnLista(categoria, string.Empty, null, mModelAdmin.SemanticThesaurus.ParentCategoriesForCreateNewsCategories);
                    orden++;
                }
            }

            return categoryModel;
        }

        /// <summary>
        /// Devuelve una lista de categorías semánticas ordenadas por su identificador.
        /// </summary>
        /// <param name="pCategorias">lista de categorías semánticas</param>
        /// <returns>Lista de categorías semánticas ordenadas por su identificador</returns>
        private List<ElementoOntologia> CategoriasTesauroSemOrdenadasTesSem(List<ElementoOntologia> pCategorias)
        {
            SortedDictionary<string, List<ElementoOntologia>> nuevaLista = new SortedDictionary<string, List<ElementoOntologia>>();

            foreach (ElementoOntologia elem in pCategorias)
            {
                string orden = elem.ObtenerPropiedad(EstiloPlantilla.Identifier_TesSem).PrimerValorPropiedad;
                if (!nuevaLista.ContainsKey(orden))
                {
                    nuevaLista.Add(orden, new List<ElementoOntologia>());
                }

                nuevaLista[orden].Add(elem);
            }

            List<ElementoOntologia> elementos = new List<ElementoOntologia>();

            foreach (List<ElementoOntologia> elems in nuevaLista.Values)
            {
                elementos.AddRange(elems);
            }

            return elementos;
        }

        /// <summary>
        /// Crea un categoría semántica en el modelo para las vistas.
        /// </summary>
        /// <param name="pCategoria">Categoría semántica</param>
        /// <param name="pOrden">Orden</param>
        /// <param name="pNivel">Nivel en el tesauro</param>
        /// <param name="pCategoriaPadre">ID de la categoría padre</param>
        /// <param name="categoryModel">Lista para las categorías del modelo</param>
        private void CargarCategoriaTesSem(ElementoOntologia pCategoria, short pOrden, int pNivel, string pCategoriaPadre, List<CategoryModel> categoryModel)
        {
            CategoryModel categoriaTesauro = new CategoryModel();
            categoriaTesauro.Key = Guid.NewGuid();
            categoriaTesauro.StringKey = pCategoria.Uri;
            categoriaTesauro.Name = pCategoria.ObtenerPropiedad(EstiloPlantilla.PrefLabel_TesSem).ObtenerPrimerValorDeIdiomaOSinEl(UtilIdiomas.LanguageCode) + " (" + pCategoria.ObtenerPropiedad(EstiloPlantilla.Identifier_TesSem).PrimerValorPropiedad + ")";
            categoriaTesauro.LanguageName = pCategoria.ObtenerPropiedad(EstiloPlantilla.PrefLabel_TesSem).ObtenerPrimerValorDeIdiomaOSinEl(UtilIdiomas.LanguageCode) + " (" + pCategoria.ObtenerPropiedad(EstiloPlantilla.Identifier_TesSem).PrimerValorPropiedad + ")";
            categoriaTesauro.Order = pOrden;

            categoryModel.Add(categoriaTesauro);
            mCategoriasModeloTesSem.Add(pCategoria.Uri, new object[] { categoriaTesauro, pCategoria, pNivel });

            if (!string.IsNullOrEmpty(pCategoriaPadre))
            {
                categoriaTesauro.ParentCategoryStringKey = pCategoriaPadre;
                categoriaTesauro.ParentCategoryKey = ((CategoryModel)mCategoriasModeloTesSem[pCategoriaPadre][0]).Key;
            }

            AgregarPropiedadesExtraCategorias(pCategoria);

            Propiedad propHijos = pCategoria.ObtenerPropiedad(EstiloPlantilla.Narrower_TesSem);
            if (propHijos.ValoresUnificados.Count > 0)
            {
                short orden = 0;

                foreach (ElementoOntologia elem in CategoriasTesauroSemOrdenadasTesSem(new List<ElementoOntologia>(propHijos.ValoresUnificados.Values)))
                {
                    CargarCategoriaTesSem(elem, orden, (pNivel + 1), categoriaTesauro.StringKey, categoryModel);
                    orden++;
                }
            }

            if (mCategoriasExpandidasStringIDs.Contains(pCategoria.Uri))
            {
                ExpandirCategoriaYPadres(categoriaTesauro);
            }
        }

        /// <summary>
        /// Agrega las propiedades extra de una categoría a la variable que las guarda.
        /// </summary>
        /// <param name="pCategoria">Categoría</param>
        private void AgregarPropiedadesExtraCategorias(ElementoOntologia pCategoria)
        {
            if (mPropiedadesExtraCategorias.Count == 0)
            {
                return;
            }

            string valores = "";

            foreach (string prop in mPropiedadesExtraCategorias.Keys)
            {
                Propiedad propiedad = pCategoria.ObtenerPropiedad(prop);

                if (propiedad.ListaValoresIdioma.Count > 0)
                {
                    foreach (string idioma in propiedad.ListaValoresIdioma.Keys)
                    {
                        foreach (string valor in propiedad.ListaValoresIdioma[idioma].Keys)
                        {
                            valores += prop + "|" + valor + "@" + idioma + "|||[||]";
                        }
                    }
                }
                else if (propiedad.ValoresUnificados.Count > 0)
                {
                    foreach (string valor in propiedad.ValoresUnificados.Keys)
                    {
                        string val = valor;

                        if (propiedad.Tipo == TipoPropiedad.ObjectProperty && !val.StartsWith("http"))
                        {
                            val = "http://" + val;
                        }

                        valores += prop + "|" + val + "[||]";
                    }
                }
            }

            if (!string.IsNullOrEmpty(valores))
            {
                mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK = string.Concat(mModelAdmin.SemanticThesaurus.ExtraSemanticPropertiesValuesBK, pCategoria.Uri, "|", valores, "[|||]");
            }
        }

        /// <summary>
        /// Recarga el combo de categorías padres para la creación de nuevas.
        /// </summary>
        /// <param name="pCategoria">Categoría</param>
        /// <param name="pEspacios">Cadena de espacios según nivel de la categoría</param>
        /// <param name="pListaCategoriasOmitir">Lista de categorías a omitir de la lista final</param>
        /// <param name="pListaCats">Lista final para el combo</param>
        private void CargarCategoriasHijasOmitiendoCatEnLista(ElementoOntologia pCategoria, string pEspacios, List<string> pListaCategoriasOmitir, Dictionary<string, string> pListaCats)
        {
            if (pListaCategoriasOmitir == null || !pListaCategoriasOmitir.Contains(pCategoria.Uri))
            {
                pListaCats.Add(pCategoria.Uri, pEspacios + pCategoria.ObtenerPropiedad(EstiloPlantilla.PrefLabel_TesSem).ObtenerPrimerValorDeIdiomaOSinEl(UtilIdiomas.LanguageCode));

                pEspacios += UtilCadenas.HtmlDecode("&nbsp;&nbsp;&nbsp;");

                foreach (ElementoOntologia categoria in CategoriasTesauroSemOrdenadasTesSem(new List<ElementoOntologia>(pCategoria.ObtenerPropiedad(EstiloPlantilla.Narrower_TesSem).ValoresUnificados.Values)))
                {
                    CargarCategoriasHijasOmitiendoCatEnLista(categoria, pEspacios, pListaCategoriasOmitir, pListaCats);
                }
            }
        }

        private void ExpandirCategoriaYPadres(CategoryModel pCategoriaTesauro)
        {
            mCategoriasExpandidasIDs.Add(pCategoriaTesauro.Key);

            if (pCategoriaTesauro.ParentCategoryKey != Guid.Empty)
            {
                ExpandirCategoriaYPadres((CategoryModel)mCategoriasModeloTesSem[pCategoriaTesauro.ParentCategoryStringKey][0]);
            }
        }

        ///// <summary>
        ///// Recarga el combo de categorías padres para la creación de nuevas.
        ///// </summary>
        //private void RecargarComboPadresParaCreacionCategoria()
        //{
        //    mModelAdmin.SemanticThesaurus.ParentCategoriesForCreateNewsCategories = new Dictionary<string, string>();

        //    //foreach (CategoriaTesauro catTes in mGestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
        //    //{
        //    //    cargarCategoriasHijas(catTes, string.Empty, mAdminCatModel.ParentCategoriesForCreateNewsCategories);
        //    //}
        //}

        /// <summary>
        /// Carga inicial de la edición de tesauros semánticos.
        /// </summary>
        private void CargarInicial_TesSem()
        {
            if (mModelAdmin == null)
            {
                mModelAdmin = new ComAdminSemanticElemModel();
            }

            mModelAdmin.PageType = ComAdminSemanticElemModel.ComAdminSemanticElemPage.SemanticThesaurusEdition;
            mModelAdmin.SemanticThesaurus = new ComAdminEditSemanticThesaurus();
            mModelAdmin.SemanticThesaurus.SemanticThesaurusEditables = new Dictionary<KeyValuePair<string, string>, string>();

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mProyTesSemDS = proyCN.ObtenerTesaurosSemanticosConfigEdicionDeProyecto(ProyectoSeleccionado.Clave);
            proyCN.Dispose();

            foreach (ProyectoConfigExtraSem filaConfig in mProyTesSemDS.ListaProyectoConfigExtraSem.Where(proy => proy.Editable == true))
            {
                mModelAdmin.SemanticThesaurus.SemanticThesaurusEditables.Add(new KeyValuePair<string, string>(filaConfig.UrlOntologia, filaConfig.SourceTesSem), UtilCadenas.ObtenerTextoDeIdioma(filaConfig.Nombre, UtilIdiomas.LanguageCode, null));
            }

            mModelAdmin.SemanticThesaurus.ListaOntologias = new Dictionary<string, string>();

            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, documentacionDW, false, true, true);
            documentacionCN.Dispose();

            List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = documentacionDW.ListaDocumento.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)).ToList();

            foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in filasDoc)
            {
                mModelAdmin.SemanticThesaurus.ListaOntologias.Add(filaDoc.Enlace, filaDoc.Titulo);
            }
        }

        /// <summary>
        /// Comprueba si hay que redireccionar y si es así devuelve la redirección.
        /// </summary>
        /// <returns>Redirección resultante</returns>
        private ActionResult ComprobarRedirecciones()
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada || !ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
            {
                return Redirect(BaseURLIdioma);
            }

            return null;
        }

        #endregion

        #region Métodos auxiliares Editar Entidades Secundarias

        /// <summary>
        /// Elimina una serie de instancias de la ontología secundaria.
        /// </summary>
        private void EliminarIntanciasOntologiaSecundaria()
        {
            CargarInstanciasOntologiaSecundaria();
            Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(mEditEntSecModel.OntologyUrl, ProyectoSeleccionado.Clave);
            FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            foreach (string sujeto in mEditEntSecModel.SelectedInstances.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string idHasEntidadPrincipal = sujeto;
                List<string> sujetos = facCN.ObtenerSujetosConObjetoDePropiedad(mEditEntSecModel.OntologyUrl, idHasEntidadPrincipal, "http://gnoss/hasEntidad");

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

                    idHasEntidadPrincipal = "entidadsecun_" + idHasEntidadPrincipal.ToLower();
                }

                //Comprobamos en cada proyecto donde está compartida la ontología si se usa la entidad:
                foreach (Guid proyectoID in docOnto.ListaProyectos)
                {
                    FacetadoDS facDS = facCN.ObtenerTripletasConObjeto(proyectoID.ToString(), sujeto.ToLower());

                    foreach (DataRow fila in facDS.Tables[0].Rows)
                    {
                        if ((string)fila[1] != "http://gnoss/hasEntidad")
                        {
                            mMensajeAdmin = UtilIdiomas.GetText("COMADMIN", "ENTIDADSECUNDNOBORRABLE", sujeto, mEditEntSecModel.OntologyUrl);
                            throw new Exception(mMensajeAdmin);
                        }
                    }

                    facDS.Dispose();
                }

                ControladorDocumentacion.BorrarRDFDeVirtuoso(idHasEntidadPrincipal, mEditEntSecModel.OntologyUrl, UrlIntragnoss, "acid", ProyectoSeleccionado.Clave, true);

                foreach (Guid proyectoID in docOnto.ListaProyectos)
                {
                    ControladorDocumentacion.BorrarRDFDeVirtuoso(idHasEntidadPrincipal, proyectoID.ToString().ToLower(), UrlIntragnoss, "acid", ProyectoSeleccionado.Clave, true);
                    facCN.BorrarTripleta(proyectoID.ToString().ToLower(), "<" + UrlIntragnoss + mEditEntSecModel.OntologyUrl.ToLower() + ">", "<http://gnoss/hasEntidad>", "<" + UrlIntragnoss + idHasEntidadPrincipal + ">", true);
                }

                if (mModelAdmin.SecondaryEntities.SecondaryInstancesEditables.ContainsKey(sujeto))
                {
                    mModelAdmin.SecondaryEntities.SecondaryInstancesEditables.Remove(sujeto);
                }
            }

            facCN.Dispose();
        }

        /// <summary>
        /// Guarda una instancia secundaría.
        /// </summary>
        /// <returns>Acción resultado</returns>
        private ActionResult GuardarIntanciaOntologiaSecundaria()
        {
            bool creandoNuevo = (mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.SaveNewInstance);
            CrearEditarIntanciaOntologiaSecundaria();
            List<ElementoOntologia> entidadesGuardar = mSemController.RecogerValoresRdf(HttpUtility.HtmlDecode(mEditEntSecModel.RdfValue), null);
            string nuevaSecundaria = "";
            if (entidadesGuardar.Count > 0 && entidadesGuardar[0].Propiedades[0].PrimerValorPropiedad != null)
            {
                if (entidadesGuardar[0].Propiedades.Count > 0)
                {
                    nuevaSecundaria = entidadesGuardar[0].Propiedades[0].PrimerValorPropiedad;
                    string tildes = "[áéíóúöüÁÉÍÓÚÖÜ]";
                    bool tilde = Regex.IsMatch(nuevaSecundaria, tildes);
                    if (tilde)
                    {
                        return GnossResultERROR("No se pueden poner acentos");
                    }
                }
            }

            if (string.IsNullOrEmpty(mEditEntSecModel.EntitySubject))
            {
                return GnossResultERROR("El Sujeto no puede ser vacío.");
            }

            if (creandoNuevo)
            {
                entidadesGuardar[0].ID = mEditEntSecModel.EntitySubject;
            }

            Documento docOnto = ControladorDocumentacion.ObtenerOntologiaDeEntidadSecundaria(mEditEntSecModel.OntologyUrl, ProyectoSeleccionado.Clave);

            try
            {
                ControladorDocumentacion.GuardarRDFEntidadSecundaria(entidadesGuardar, UrlIntragnoss, mEditEntSecModel.OntologyUrl, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, docOnto, !creandoNuevo);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Ya existe una entidad secundaria con"))
                {
                    mMensajeAdmin = ex.Message;
                }

                throw;
            }

            CargarInstanciasOntologiaSecundaria();

            if (!mModelAdmin.SecondaryEntities.SecondaryInstancesEditables.ContainsKey(entidadesGuardar[0].Uri))
            {
                mModelAdmin.SecondaryEntities.SecondaryInstancesEditables.Add(entidadesGuardar[0].Uri, entidadesGuardar[0].Uri);
            }

            if (!string.IsNullOrEmpty(mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key))
            {
                Propiedad propRep = entidadesGuardar[0].ObtenerPropiedad(mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key);

                if (propRep != null)
                {
                    string repres = propRep.ObtenerPrimerValorDeIdiomaOSinEl(UtilIdiomas.LanguageCode);
                    mModelAdmin.SecondaryEntities.SecondaryInstancesEditables[entidadesGuardar[0].Uri] = repres;
                }
            }

            mModelAdmin.SecondaryEntities.SemanticResourceModel = null;
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheTesauroFaceta(ProyectoSeleccionado.Clave);
            facetadoCL.Dispose();
            return PartialView("_EditarEntSecInstancias", mModelAdmin);
        }

        /// <summary>
        /// Carga el formulario de edición para crear una instación de una ontología secundaria.
        /// </summary>
        private void CrearEditarIntanciaOntologiaSecundaria()
        {
            bool crearnuevaInstancia = (mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.CreateNewInstance || mEditEntSecModel.EditAction == EditSecondaryEntityModel.Action.SaveNewInstance);

            ProyectoConfigExtraSem filaConfig = mProyTesSemDS.ListaProyectoConfigExtraSem.FirstOrDefault(proy => proy.UrlOntologia.Equals(mEditEntSecModel.OntologyUrl));//("UrlOntologia='" + mEditEntSecModel.OntologyUrl + "'")[0];
            Guid ontologiaID = new Guid(filaConfig.SourceTesSem);
            mOntologia = ObtenerOntologia(ontologiaID);
            SemCmsController.ApañarRepeticionPropiedades(mOntologia.ConfiguracionPlantilla, mOntologia.Entidades);
            mModelAdmin.SecondaryEntities.SemanticResourceModel = new SemanticResourceModel();

            List<ElementoOntologia> instanciaPinc = null;
            mModelAdmin.SecondaryEntities.CreatingNewInstance = crearnuevaInstancia;

            if (!crearnuevaInstancia)
            {
                instanciaPinc = ObtenerInstanciasRDFDeEntidad();
            }

            mSemController = new SemCmsController(mModelAdmin.SecondaryEntities.SemanticResourceModel, mOntologia, Guid.Empty, instanciaPinc, ProyectoSeleccionado, IdentidadActual, UtilIdiomas, BaseURL, BaseURLIdioma, BaseURLContent, BaseURLStatic, UrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);

            if (mOntologia.ConfiguracionPlantilla.MultiIdioma && !(ParametrosGeneralesRow.IdiomaDefecto == null) && !string.IsNullOrEmpty(ParametrosGeneralesRow.IdiomaDefecto))
            {//Es multiidioma:
                mSemController.IdiomaDefecto = ParametrosGeneralesRow.IdiomaDefecto;
                mSemController.IdiomasDisponibles = mConfigService.ObtenerListaIdiomasDictionary();
            }

            try
            {
                mSemController.ObtenerModeloSemCMSEdicion(IdentidadActual.Clave);
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(mModelAdmin.SecondaryEntities.SemanticResourceModel.AdminGenerationError) || !ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    throw;
                }
            }

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mModelAdmin.SecondaryEntities.SecondaryOntologyNameSelected = docCN.ObtenerTituloDocumentoPorID(ontologiaID);
            docCN.Dispose();
        }

        /// <summary>
        /// Obtiene las instancias de la entidad secundaria seleccionada.
        /// </summary>
        /// <returns></returns>
        private List<ElementoOntologia> ObtenerInstanciasRDFDeEntidad()
        {
            string idHasEntidadPrincipal = mEditEntSecModel.SelectedInstances.Split(',')[0];
            FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            List<string> sujetos = facCN.ObtenerSujetosConObjetoDePropiedad(mEditEntSecModel.OntologyUrl, idHasEntidadPrincipal, "http://gnoss/hasEntidad");
            facCN.Dispose();

            if (sujetos.Count > 0 && sujetos[0].Contains("entidadsecun_"))
            {
                idHasEntidadPrincipal = sujetos[0].Substring(sujetos[0].IndexOf("entidadsecun_"));
            }
            else
            {
                idHasEntidadPrincipal = "entidadsecun_" + idHasEntidadPrincipal.Substring(idHasEntidadPrincipal.LastIndexOf("/") + 1).ToLower();
            }

            GestionOWL gestorOWL = new GestionOWL();
            gestorOWL.UrlOntologia = BaseURLFormulariosSem + "/Ontologia/" + mEditEntSecModel.OntologyUrl + "#"; ;
            gestorOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_GNOSS;

            byte[] rdfVirtuoso = ControladorDocumentacion.ObtenerRDFDeVirtuoso(idHasEntidadPrincipal, mEditEntSecModel.OntologyUrl, UrlIntragnoss, mEditEntSecModel.OntologyUrl, gestorOWL.NamespaceOntologia, mOntologia, null, false);

            if (rdfVirtuoso.Length == 0)
            {
                mMensajeAdmin = "El recurso " + idHasEntidadPrincipal + " no tiene datos en virtuoso.";
                throw new ExcepcionGeneral(mMensajeAdmin);
            }

            MemoryStream buffer = new MemoryStream(rdfVirtuoso);
            
            StreamReader reader = new StreamReader(buffer);
            string rdfTexto = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            List<ElementoOntologia> instanciasPrincipales = null;

            try
            {
                instanciasPrincipales = gestorOWL.LeerFicheroRDF(mOntologia, rdfTexto, true);
            }
            catch (Exception ex)
            {
                mMensajeAdmin = "El RDF del recurso " + idHasEntidadPrincipal + " no es correcto: " + Environment.NewLine + ex.Message;
                throw;
            }

            return instanciasPrincipales;
        }

        private bool EsOracle()
        {
            string tipoBD = mConfigService.ObtenerTipoBD();

            if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("1"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Carga las instacias principales de una ontología secundaría.
        /// </summary>
        private void CargarInstanciasOntologiaSecundaria()
        {
            ProyectoConfigExtraSem filaConfig = mProyTesSemDS.ListaProyectoConfigExtraSem.FirstOrDefault(proy => proy.UrlOntologia.Equals(mEditEntSecModel.OntologyUrl));
            Guid ontologiaID = new Guid();
            if (EsOracle())
            {
                ontologiaID = new Guid(Gnoss.Util.General.UtilOracle.FormatearGuid(new Guid(filaConfig.SourceTesSem), true));
            }
            else
            {
                ontologiaID = new Guid(filaConfig.SourceTesSem);
            }

            mOntologia = ObtenerOntologia(ontologiaID);

            List<ElementoOntologia> entidadesPrinc = GestionOWL.ObtenerElementosContenedorSuperiorOHerencias(mOntologia.Entidades);

            mModelAdmin.SecondaryEntities.SecondaryInstancesEditables = new SortedDictionary<string, string>();
            FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            if (string.IsNullOrEmpty(mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key))
            {
                foreach (string sujeto in facCN.ObjeterSujetosDePropiedadPorValor(mEditEntSecModel.OntologyUrl, FacetadoAD.RDF_TYPE, new List<string>(new string[] { entidadesPrinc[0].TipoEntidad })))
                {
                    mModelAdmin.SecondaryEntities.SecondaryInstancesEditables.Add(sujeto, sujeto);
                }
            }
            else
            {
                FacetadoDS facDS = facCN.ObtenerRDFXMLSelectorEntidadFormulario(mEditEntSecModel.OntologyUrl, null, null, entidadesPrinc[0].TipoEntidad, new List<string>(new string[] { mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key }));

                foreach (DataRow fila in facDS.Tables[0].Rows)
                {
                    string sujeto = (string)fila[0];
                    if (!mModelAdmin.SecondaryEntities.SecondaryInstancesEditables.ContainsKey(sujeto))
                    {
                        mModelAdmin.SecondaryEntities.SecondaryInstancesEditables.Add(sujeto, (string)fila[2]);
                    }
                    else if (fila.Table.Columns.Count > 3 && !fila.IsNull(3) && (string)fila[3] == UtilIdiomas.LanguageCode)
                    {
                        mModelAdmin.SecondaryEntities.SecondaryInstancesEditables[sujeto] = (string)fila[2];
                    }
                }

            }

            facCN.Dispose();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mModelAdmin.SecondaryEntities.SecondaryOntologyNameSelected = docCN.ObtenerTituloDocumentoPorID(ontologiaID);
            docCN.Dispose();
        }

        /// <summary>
        /// Obtiene y lee una ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de ontología</param>
        /// <returns>Ontología leida</returns>
        private Ontologia ObtenerOntologia(Guid pOntologiaID)
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
                mMensajeAdmin = "Error al leer el XML de la ontología con ID " + pOntologiaID + ":" + Environment.NewLine + ex.ToString();
                GuardarLogErrorAJAX(mMensajeAdmin);
                throw;
            }

            if (arrayOnto == null)
            {
                mMensajeAdmin = "No ha sido posible gererar el formulario porque el array de la ontología es nulo";
                throw new Exception(mMensajeAdmin);
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
                mMensajeAdmin = "La ontología con ID " + pOntologiaID + " no es correcta:" + Environment.NewLine + ex.ToString();
                GuardarLogErrorAJAX(mMensajeAdmin);
                throw;
            }

            if (ontologia.ConfiguracionPlantilla.ListaIdiomas.Count == 0)
            {
                ontologia.ConfiguracionPlantilla.ListaIdiomas.Add(IdiomaUsuario);
            }

            return ontologia;
        }

        /// <summary>
        /// Carga inicial de la edición de entidades secundarias.
        /// </summary>
        private void CargarInicial_EntSecund()
        {
            if (mModelAdmin == null)
            {
                mModelAdmin = new ComAdminSemanticElemModel();
            }

            mModelAdmin.PageType = ComAdminSemanticElemModel.ComAdminSemanticElemPage.SecondaryEntitiesEdition;
            mModelAdmin.SecondaryEntities = new ComAdminEditSecondaryEntities();
            mModelAdmin.SecondaryEntities.SecondaryEntitiesEditables = new Dictionary<string, string>();

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mProyTesSemDS = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(ProyectoSeleccionado.Clave);
            Guid proyectoOntologiasID = Guid.Empty;
            //Obtener las del proyecto padre.
            if (ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
            {
                proyectoOntologiasID = new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
            }
            if (proyectoOntologiasID != Guid.Empty)
            {
                mProyTesSemDS.ListaProyectoConfigExtraSem.AddRange(proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(proyectoOntologiasID).ListaProyectoConfigExtraSem);
            }
            proyCN.Dispose();

            foreach (ProyectoConfigExtraSem filaConfig in mProyTesSemDS.ListaProyectoConfigExtraSem.Where(proy => proy.Tipo.Equals((short)TipoConfigExtraSemantica.EntidadSecundaria)).ToList())
            {
                if (!mModelAdmin.SecondaryEntities.SecondaryEntitiesEditables.ContainsKey(filaConfig.UrlOntologia))
                {
                    mModelAdmin.SecondaryEntities.SecondaryEntitiesEditables.Add(filaConfig.UrlOntologia, UtilCadenas.ObtenerTextoDeIdioma(filaConfig.Nombre, UtilIdiomas.LanguageCode, null));
                }
            }

            foreach (ProyectoConfigExtraSem filaConfig in mProyTesSemDS.ListaProyectoConfigExtraSem.Where(proy => proy.Tipo.Equals((short)TipoConfigExtraSemantica.TesauroSemantico)).ToList())
            {
                mModelAdmin.SecondaryEntities.SecondaryEntitiesEditables.Remove(filaConfig.UrlOntologia);
            }
        }

        #endregion

        #region Métodos auxiliares de grafos simples

        /// <summary>
        /// Elimina una instancia de un grafo simple.
        /// </summary>
        private void EliminarIntanciasGrafoSimple()
        {
            Dictionary<string, List<string>> valoresGrafos = new Dictionary<string, List<string>>();
            valoresGrafos.Add(mEditGrafoSimpleModel.Graph.ToLower(), new List<string>());

            foreach (string sujeto in mEditGrafoSimpleModel.SelectedInstances.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                valoresGrafos[mEditGrafoSimpleModel.Graph.ToLower()].Add(sujeto);
            }

            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCN.BorrarValoresGrafos(valoresGrafos, 0);
            facetadoCN.Dispose();

            CargarInstanciasGrafoSimple();
        }

        /// <summary>
        /// Crea una instancia en un grafo simple.
        /// </summary>
        private void CrearIntanciaGrafoSimple()
        {
            if (string.IsNullOrEmpty(mEditGrafoSimpleModel.NewElement))
            {
                throw new Exception("El nuevo elemento no puede estar vacío");
            }

            Dictionary<string, List<string>> valoresGrafos = new Dictionary<string, List<string>>();
            valoresGrafos.Add(mEditGrafoSimpleModel.Graph.ToLower(), new List<string>(new string[] { mEditGrafoSimpleModel.NewElement }));

            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCN.InsertarValoresGrafos(valoresGrafos, 0);
            facetadoCN.Dispose();

            CargarInstanciasGrafoSimple();
        }

        /// <summary>
        /// Carga las instacias principales de una ontología secundaría.
        /// </summary>
        private void CargarInstanciasGrafoSimple()
        {
            mModelAdmin.SimpleGraphs.SimpleGraphsInstancesEditables = new List<string>();
            FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            List<string> propsBus = new List<string>();
            propsBus.Add("http://gnoss/has" + mEditGrafoSimpleModel.Graph.ToLower());
            FacetadoDS facDS = facCN.ObtenerRDFXMLSelectorEntidadFormulario(mEditGrafoSimpleModel.Graph, null, propsBus[0], null, propsBus);
            facCN.Dispose();

            foreach (DataRow fila in facDS.Tables[0].Rows)
            {
                if (!mModelAdmin.SimpleGraphs.SimpleGraphsInstancesEditables.Contains((string)fila[2]))
                {
                    mModelAdmin.SimpleGraphs.SimpleGraphsInstancesEditables.Add((string)fila[2]);
                }
            }

            mModelAdmin.SimpleGraphs.SimpleGraphsInstancesEditables.Sort();
            facDS.Dispose();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mModelAdmin.SimpleGraphs.SimpleGraphsNameSelected = mEditGrafoSimpleModel.Graph;
            docCN.Dispose();
        }

        /// <summary>
        /// Carga inicial de la edición de entidades secundarias.
        /// </summary>
        private void CargarInicial_GrafoSimple()
        {
            if (mModelAdmin == null)
            {
                mModelAdmin = new ComAdminSemanticElemModel();
            }

            mModelAdmin.PageType = ComAdminSemanticElemModel.ComAdminSemanticElemPage.SimpleGraphsEdition;
            mModelAdmin.SimpleGraphs = new ComAdminEditSimpleGraphs();
            mModelAdmin.SimpleGraphs.SimpleGraphsEditables = new Dictionary<string, string>();

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mProyTesSemDS = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(ProyectoSeleccionado.Clave);
            proyCN.Dispose();

            foreach (ProyectoConfigExtraSem filaConfig in mProyTesSemDS.ListaProyectoConfigExtraSem.Where(proy => proy.Tipo.Equals((short)TipoConfigExtraSemantica.GrafoSimple)).ToList())
            {
                if (!mModelAdmin.SimpleGraphs.SimpleGraphsEditables.ContainsKey(filaConfig.SourceTesSem))
                {
                    mModelAdmin.SimpleGraphs.SimpleGraphsEditables.Add(filaConfig.SourceTesSem, filaConfig.SourceTesSem + " (" + UtilCadenas.ObtenerTextoDeIdioma(filaConfig.Nombre, UtilIdiomas.LanguageCode, null) + ")");
                }
            }
        }

        #endregion

    }
}
