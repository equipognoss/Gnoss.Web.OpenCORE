using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador de la página para administrar la preferencia del proyecto
    /// </summary>
    public class AdministrarPreferenciaProyectoController : ControllerBaseWeb
    {
        public AdministrarPreferenciaProyectoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        private AdministrarPreferenciaProyectoViewModel mPaginaModel = null;
        /// <summary>
        /// 
        /// </summary>
        private string mIdiomaDefecto;
        /// <summary>
        /// 
        /// </summary>
        private List<Guid> mCategoriasSeleccionadasIDs;
        /// <summary>
        /// 
        /// </summary>
        private List<Guid> mCategoriasCompartidasIDs;
        /// <summary>
        /// 
        /// </summary>
        private List<Guid> mCategoriasExpandidasIDs;
        private GestionTesauro mGestorTesauro = null;

        #endregion

        #region Métodos de evento
        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            short i = 0;
            foreach (CategoriaTesauro cat in GestorTesauro.ListaCategoriasTesauroPrimerNivel.Values.OrderBy(cat => cat.FilaCategoria.Orden))
            {
                cat.FilaCategoria.Orden = i;
                if (GestorTesauro.TesauroActualID != cat.FilaCategoria.TesauroID)
                {
                    mEntityContext.AceptarCambios(cat.FilaCategoria);
                    AD.EntityModel.Models.Tesauro.CatTesauroCompartida catCompartida = GestorTesauro.TesauroDW.ListaCatTesauroCompartida.Where(item => item.TesauroOrigenID.Equals(cat.FilaCategoria.TesauroID) && item.CategoriaOrigenID.Equals(cat.FilaCategoria) && item.TesauroDestinoID.Equals(GestorTesauro.TesauroActualID)).FirstOrDefault();
                    catCompartida.Orden = i;
                }
                else
                {
                    ReOrdenarCategorias(cat);
                }
                i++;
            }
            GestorTesauro.RecargarGestor();

            try
            {
                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                tesauroCN.ActualizarTesauro();
                tesauroCN.Dispose();
            }
            finally
            {
                TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                tesauroCL.InvalidarCacheDeTesauroDeProyecto(ProyectoSeleccionado.Clave);
                tesauroCL.Dispose();
            }

            return View(PaginaModel);
        }
        /// <summary>
        /// Muestra el panel para realizar la acción solicitada
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { Models.ViewModels.TipoPaginaAdministracion.Tesauro })]
        public ActionResult MostrarAccion(string typeAction)
        {
            typeAction = typeAction.Replace('-', '_');

            string error = string.Empty;

            if (CategoriasSeleccionadasIDs.Count > 1)
            {
                if (typeAction == TypesAccion.change_name.ToString())
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT_UNA_CAMBIARNOMBRE");
                }
                else if (typeAction == TypesAccion.order.ToString())
                {
                    bool sonHermanos = true;
                    Interfaces.IElementoGnoss padre = GestorTesauro.ListaCategoriasTesauro[CategoriasSeleccionadasIDs[0]].Padre;

                    foreach (Guid catTesID in CategoriasSeleccionadasIDs)
                    {
                        sonHermanos = padre.Hijos.Contains(GestorTesauro.ListaCategoriasTesauro[catTesID]);
                        if (!sonHermanos)
                        {
                            error = UtilIdiomas.GetText("COMADMINCATEGORIAS", "ERROR_CAT_DEBE_SER_MISMO_NIVEL");
                            break;
                        }
                    }
                }
                else if (typeAction == TypesAccion.delete.ToString())
                {
                    foreach (Guid catID in CategoriasSeleccionadasIDs)
                    {
                        if (CategoriasCompartidasIDs.Contains(catID))
                        {
                            error = UtilIdiomas.GetText("PERFILBASE", "ERROR_ELIMINAR_CAT_COMPARTIDA");
                            break;
                        }
                    }
                }
            }
            else if (CategoriasSeleccionadasIDs.Count == 0)
            {
                if (typeAction == TypesAccion.change_name.ToString())
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT_CAMBIARNOMBRE");
                }
                else if (typeAction == TypesAccion.order.ToString())
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT_ORDENAR");
                }
                else if (typeAction == TypesAccion.move.ToString())
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT_MOVER");
                }
                else if (typeAction == TypesAccion.delete.ToString())
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_SELECT_ELIMINAR");
                }
            }
            else if (CategoriasCompartidasIDs.Contains(CategoriasSeleccionadasIDs[0]))
            {
                if (typeAction == TypesAccion.change_name.ToString())
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAMBIARNOMBRE_CAT_COMPARTIDA");
                }
                else if (typeAction == TypesAccion.delete.ToString())
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_ELIMINAR_CAT_COMPARTIDA");
                }
            }
            if (string.IsNullOrEmpty(error))
            {
                PaginaModel.Action = typeAction;

                if (typeAction == "share_load")
                {
                    Guid idComunidad = new Guid(RequestParams("ComunidadCompartirKey"));
                    string nombreComunidad = Perfil.Communities.Find(comunidad => comunidad.Key == idComunidad).Name;

                    //mPaginaModel.ComunidadCompartir = new KeyValuePair<Guid, string>(idComunidad, nombreComunidad);

                    TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperTesauro tesauroDW = tesauroCL.ObtenerTesauroDeProyecto(idComunidad);
                    GestionTesauro gesTesauro = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);

                    //mPaginaModel.CategoriasCompartir = CargarTesauroPorGestorTesauro(gesTesauro);
                }
                else if (typeAction == TypesAccion.change_name.ToString())
                {
                    PaginaModel.Categoria = new KeyValuePair<Guid, Dictionary<string, string>>(CategoriasSeleccionadasIDs.First(), GestorTesauro.ListaCategoriasTesauro[CategoriasSeleccionadasIDs.First()].Nombre);
                }
                else if (typeAction == TypesAccion.delete.ToString())
                {
                    List<Guid> listaCatSeleccionadasEHijos = new List<Guid>();

                    foreach (Guid catID in CategoriasSeleccionadasIDs)
                    {
                        AgregarCategoriaEHijosALista(GestorTesauro.ListaCategoriasTesauro[catID], listaCatSeleccionadasEHijos);
                    }

                    TesauroCN tesCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    bool tieneRecursosAsociados = tesCN.EstanVinculadasCategoriasTesauro(GestorTesauro.TesauroActualID, listaCatSeleccionadasEHijos);

                    if (!tieneRecursosAsociados)
                    {
                        //EliminarCategoriasTesauro(Guid.Empty, false);

                        CategoriasSeleccionadasIDs = new List<Guid>();

                        RecargarPagniaModel();

                        //Devolvemos un multiView con una sola vista para poder identificarla desde javascript
                        Controles.MultiViewResult multiView = new Controles.MultiViewResult(this, mViewEngine);
                        multiView.AddView("_Tesauro", "tesauro", PaginaModel);
                        return multiView;
                    }
                    else
                    {

                        TesauroCN tesCN2 = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        bool existenRecursosNoHuerfanos = tesCN2.ObtenerSiExistenElementosNoHuerfanos(GestorTesauro.TesauroActualID, listaCatSeleccionadasEHijos);
                        tesCN2.Dispose();

                        //mPaginaModel.ExistenRecursosNoHuerfanos = existenRecursosNoHuerfanos;
                    }
                }
                else if (typeAction == TypesAccion.share.ToString() && ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    //DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerProyectosParticipaUsuario(AD.EntityModel.Models.UsuarioDS.Usuario.UsuarioID);

                    Dictionary<Guid, string> listaProyectos = new Dictionary<Guid, string>();

                    //foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in dataWrapperProyecto.ListaProyecto)
                    //{
                    //    listaProyectos.Add(filaProy.ProyectoID, filaProy.Nombre);
                    //}

                    ViewBag.ListaProyectosUsuario = listaProyectos;
                }

                return PartialView("_Acciones", PaginaModel);
            }
            else
            {
                return GnossResultERROR(error);
            }
        }

        /// <summary>
        /// Ejecuta la acción solicitada
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { Models.ViewModels.TipoPaginaAdministracion.Tesauro })]
        public ActionResult EjecutarAccion(string typeAction)
        {
            PaginaModel.Action = typeAction;

            switch (typeAction)
            {
                case "change-name":
                    return CambiarNombre_Event();
                case "multilanguaje":
                    return CambiarIdioma_Event();
                case "onlylanguaje":
                    return IdiomaUnico_Event();
            }

            return GnossResultERROR();
        }
        /// <summary>
        /// Guardar los cambios
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { Models.ViewModels.TipoPaginaAdministracion.Tesauro })]
        public ActionResult Guardar()
        {
            try
            {
                if (!PaginaModel.MultiLanguaje)
                {
                    foreach (CategoriaTesauro categoriaTesauro in GestorTesauro.ListaCategoriasTesauro.Values)
                    {
                        if (!CategoriasCompartidasIDs.Contains(categoriaTesauro.Clave) && categoriaTesauro.FilaCategoria.Nombre != categoriaTesauro.Nombre[UtilIdiomas.LanguageCode.ToString()])
                        {
                            categoriaTesauro.FilaCategoria.Nombre = categoriaTesauro.Nombre[UtilIdiomas.LanguageCode.ToString()];
                        }
                    }
                }
                List<Guid> categoriasSeleccionadas = CategoriasSeleccionadasIDs;
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                proyCN.ActualizarTablaPreferenciaProyecto(categoriasSeleccionadas, ProyectoSeleccionado.Clave);

                TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                tesauroCL.InvalidarCacheDeTesauroDeProyecto(ProyectoSeleccionado.Clave);
                tesauroCL.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);
                tesauroCL.Dispose();

                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
                proyCL.Dispose();

                CL.Facetado.FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "");
                facetadoCL.Dispose();

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }
        }
        /// <summary>
        /// Acción de cambiar el idioma del tesauro
        /// </summary>
        /// <returns>Devuelve el tesauro modificado</returns>
        private ActionResult CambiarIdioma_Event()
        {
            RecargarPagniaModel();

            return PartialView("_Tesauro", PaginaModel);
        }
        /// <summary>
        /// Acción de desactivar el multiidioma
        /// </summary>
        /// <returns>Devuelve el tesauro modificado</returns>
        private ActionResult IdiomaUnico_Event()
        {
            PaginaModel.PasosRealizados += ((short)AccionConCategorias.SeleccionarUnicoIdioma).ToString() + "|_|" + IdiomaDefecto + "|,|";
            CambiarIdiomaTesauro(IdiomaDefecto);

            RecargarPagniaModel();

            return PartialView("_Tesauro", PaginaModel);
        }
        #endregion

        #region Métodos privados
        /// <summary>
        /// Acción de cambiar el nombre a una categoría
        /// </summary>
        /// <returns>Devuelve el tesauro modificado</returns>
        private ActionResult CambiarNombre_Event()
        {
            bool nombreRepetido = false;

            Guid categoriaSeleccionada = new Guid(RequestParams("categoryKey"));
            string nombre = RequestParams("name");

            Dictionary<string, string> listaCatIdiomas = new Dictionary<string, string>();

            string[] arrayCatIdiomas = nombre.Split(new string[] { "$$$" }, StringSplitOptions.None);

            foreach (string catIdioma in arrayCatIdiomas)
            {
                string[] arrayCat = catIdioma.Split(new string[] { "@@@" }, StringSplitOptions.None);

                listaCatIdiomas.Add(arrayCat[0], arrayCat[1]);
            }

            foreach (Interfaces.IElementoGnoss catTes in GestorTesauro.ListaCategoriasTesauro[categoriaSeleccionada].Padre.Hijos)
            {
                if (((CategoriaTesauro)catTes).Clave != categoriaSeleccionada)
                {
                    foreach (string idioma in listaCatIdiomas.Keys)
                    {
                        if (((CategoriaTesauro)catTes).Nombre[idioma].ToLower() == listaCatIdiomas[idioma].ToLower())
                        {
                            nombreRepetido = true;
                        }
                    }
                }
            }

            bool faltaNombre = (bool.Parse(RequestParams("multiLanguage")) && string.IsNullOrEmpty(listaCatIdiomas["es"])) || (listaCatIdiomas.ContainsKey(IdiomaDefecto) && string.IsNullOrEmpty(listaCatIdiomas[IdiomaDefecto]));

            bool caracterNoPermitido = false;

            foreach (string idioma in listaCatIdiomas.Keys)
            {
                if (listaCatIdiomas[idioma].Contains("@"))
                {
                    caracterNoPermitido = true;
                    break;
                }
            }

            if (!nombreRepetido && !faltaNombre && !caracterNoPermitido)
            {
                string nombreCategoria = "";

                foreach (string idioma in listaCatIdiomas.Keys)
                {
                    if (!string.IsNullOrEmpty(listaCatIdiomas[idioma]))
                    {
                        if (!string.IsNullOrEmpty(nombreCategoria))
                        {
                            nombreCategoria += "|||";
                        }
                        nombreCategoria += listaCatIdiomas[idioma] + "@" + idioma;
                    }
                }

                PaginaModel.PasosRealizados += ((short)AccionConCategorias.CambiarNombreCategoria).ToString() + "|_|" + categoriaSeleccionada.ToString() + "|_|" + nombreCategoria + "|,|";

                CambiarNombreCategoriasTesauro(categoriaSeleccionada, nombreCategoria);

                RecargarPagniaModel();

                return PartialView("_Tesauro", PaginaModel);
                //return UtilAJAX.AgregarJavaScripADevolucionCallBack(UtilAJAX.RefrescarControlesCallBack(true, TipoPeticionAjax.Normal, panTesauro, PanParaSelectorCategorias, panTxtAcciones, txtAccionesTesauroHack), "SlideUp();LimpiarCamposCambiarNombreCategoriaTesauro();");

            }
            else
            {
                string error = "";

                if (nombreRepetido)
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_NOMBRE_DIFERENTE");
                }
                else if (faltaNombre)
                {
                    if (listaCatIdiomas.Count > 1)
                    {
                        error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_NOMBRE*");
                    }
                    else
                    {
                        error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_NOMBRE");
                    }

                }
                else if (caracterNoPermitido)
                {
                    error = UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_NOPERMITIDO");
                }
                //lblError.Text = error;

                //return UtilAJAX.RefrescarControlCallBack(panError, lblError, true, TipoPeticionAjax.Normal);

                return GnossResultERROR(error);
            }
        }
        /// <summary>
        /// Cambia el nombre a una categoría del tesauro.
        /// </summary>
        private void CambiarNombreCategoriasTesauro(Guid pIdCategoria, string pNombreCategoria)
        {
            GestorTesauro.ListaCategoriasTesauro[pIdCategoria].FilaCategoria.Nombre = pNombreCategoria;
        }
        private void ReOrdenarCategorias(CategoriaTesauro pCategoriaTesauro)
        {
            pCategoriaTesauro.CargarSubcategorias();
            short i = 0;
            foreach (CategoriaTesauro catHija in pCategoriaTesauro.SubCategorias.OrderBy(cat => cat.FilaAgregacion.Orden))
            {
                catHija.FilaAgregacion.Orden = i;
                if (GestorTesauro.TesauroActualID != catHija.FilaCategoria.TesauroID)
                {
                    mEntityContext.AceptarCambios(catHija.FilaAgregacion);
                    AD.EntityModel.Models.Tesauro.CatTesauroCompartida catCompartida = GestorTesauro.TesauroDW.ListaCatTesauroCompartida.FirstOrDefault(item => item.TesauroOrigenID.Equals(catHija.FilaCategoria.TesauroID) && item.CategoriaOrigenID.Equals(catHija.FilaCategoria.CategoriaTesauroID) && item.TesauroDestinoID.Equals(GestorTesauro.TesauroActualID));
                    catCompartida.Orden = i;
                }
                else
                {
                    ReOrdenarCategorias(catHija);
                }
                i++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RecargarPagniaModel()
        {
            string idiomaTesauro = IdiomaDefecto;

            if (PaginaModel.MultiLanguaje && !string.IsNullOrEmpty(RequestParams("IdiomaSeleccionado")))
            {
                idiomaTesauro = RequestParams("IdiomaSeleccionado");
            }

            PaginaModel.Thesaurus.ThesaurusCategories = ObtenerListaCategorias(idiomaTesauro);

            PaginaModel.Thesaurus.ExpandedCategories = CategoriasExpandidasIDs;
            PaginaModel.Thesaurus.SelectedCategories = CategoriasSeleccionadasIDs;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCategoria"></param>
        /// <param name="pListaCategoriaID"></param>
        private void AgregarCategoriaEHijosALista(CategoriaTesauro pCategoria, List<Guid> pListaCategoriaID)
        {
            foreach (Interfaces.IElementoGnoss catTeHija in pCategoria.Hijos)
            {
                AgregarCategoriaEHijosALista((CategoriaTesauro)catTeHija, pListaCategoriaID);
            }

            if (!pListaCategoriaID.Contains(pCategoria.Clave))
            {
                pListaCategoriaID.Add(pCategoria.Clave);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pIdiomaTesauro"></param>
        /// <returns></returns>
        private List<CategoryModel> ObtenerListaCategorias(string pIdiomaTesauro = null)
        {
            List<CategoryModel> listaCategoriasTesauro = new List<CategoryModel>();

            foreach (CategoriaTesauro catTes in GestorTesauro.ListaCategoriasTesauro.Values)
            {
                CategoryModel categoriaTesauro = CargarCategoria(catTes);
                if (!string.IsNullOrEmpty(pIdiomaTesauro))
                {
                    categoriaTesauro.Lang = pIdiomaTesauro;
                }
                listaCategoriasTesauro.Add(categoriaTesauro);
            }

            return listaCategoriasTesauro;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTesauroID"></param>
        /// <param name="pCategoriaID"></param>
        /// <param name="pTesauroOrigen"></param>
        /// <param name="pTesauroDestino"></param>
        private void ImportarCategoriaCompartidaTesauro(Guid pTesauroID, Guid pCategoriaID, DataWrapperTesauro pTesauroOrigen, DataWrapperTesauro pTesauroDestino)
        {
            //.FindByTesauroIDCategoriaTesauroID(pTesauroID, pCategoriaID);
            Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCatTesauro = pTesauroOrigen.ListaCategoriaTesauro.FirstOrDefault(item => item.TesauroID.Equals(pTesauroID) && item.CategoriaTesauroID.Equals(pCategoriaID));
            pTesauroDestino.ListaCategoriaTesauro.Add(filaCatTesauro);

            List<Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro> filasCatTesauro = pTesauroOrigen.ListaCatTesauroAgCatTesauro.Where(item => item.TesauroID.Equals(pTesauroID) && item.CategoriaSuperiorID.Equals(pCategoriaID)).ToList();
            //"TesauroID = '" + pTesauroID + "' AND CategoriaSuperiorID = '" + pCategoriaID + "'");

            foreach (AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro filaCatTesAgCatTes in filasCatTesauro)
            {
                pTesauroDestino.ListaCatTesauroAgCatTesauro.Add(filaCatTesAgCatTes);
                ImportarCategoriaCompartidaTesauro(pTesauroID, filaCatTesAgCatTes.CategoriaInferiorID, pTesauroOrigen, pTesauroDestino);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pClaveIdioma"></param>
        protected void CambiarIdiomaTesauro(string pClaveIdioma)
        {
            foreach (CategoriaTesauro categoria in GestorTesauro.ListaCategoriasTesauro.Values)
            {
                if (!CategoriasCompartidasIDs.Contains(categoria.Clave))
                {
                    categoria.FilaCategoria.Nombre = categoria.Nombre[pClaveIdioma];
                }
            }
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Propiedad que que gestiona el Tesauro
        /// </summary>
        protected GestionTesauro GestorTesauro
        {
            get
            {
                if (mGestorTesauro == null)
                {
                    TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperTesauro dataWrapperTesauro = tesauroCN.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);
                    // TesauroDS tesauroDS = tesauroCN.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);

                    if (dataWrapperTesauro.ListaCatTesauroCompartida.Count > 0)
                    {
                        List<Guid> listaTesauros = new List<Guid>();
                        foreach (AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaCatTesComp in dataWrapperTesauro.ListaCatTesauroCompartida)
                        {
                            Guid tesauroID = filaCatTesComp.TesauroOrigenID;
                            if (!listaTesauros.Contains(tesauroID))
                            {
                                listaTesauros.Add(tesauroID);
                            }
                        }
                        DataWrapperTesauro tesauroTempDW = tesauroCN.ObtenerTesauroPorListaIDs(listaTesauros);

                        foreach (AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaCatTesComp in dataWrapperTesauro.ListaCatTesauroCompartida.OrderByDescending(item => item.Orden))
                        {
                            AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCatTesauro = tesauroTempDW.ListaCategoriaTesauro.Where(item => item.TesauroID.Equals(filaCatTesComp.TesauroOrigenID) && item.CategoriaTesauroID.Equals(filaCatTesComp.CategoriaOrigenID)).FirstOrDefault();

                            //FindByTesauroIDCategoriaTesauroID(filaCatTesComp.TesauroOrigenID, filaCatTesComp.CategoriaOrigenID);
                            filaCatTesauro.Orden = filaCatTesComp.Orden;
                            mEntityContext.AceptarCambios(filaCatTesauro);
                            dataWrapperTesauro.ListaCategoriaTesauro.Add(filaCatTesauro);

                            if (filaCatTesComp.CategoriaSupDestinoID.HasValue)
                            {
                                AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro filaCatTesCompAgCatTes = new AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro();
                                filaCatTesCompAgCatTes.TesauroID = filaCatTesComp.TesauroOrigenID;
                                filaCatTesCompAgCatTes.CategoriaSuperiorID = filaCatTesComp.CategoriaSupDestinoID.Value;
                                filaCatTesCompAgCatTes.CategoriaInferiorID = filaCatTesComp.CategoriaOrigenID;
                                filaCatTesCompAgCatTes.Orden = filaCatTesComp.Orden;

                                dataWrapperTesauro.ListaCatTesauroAgCatTesauro.Add(filaCatTesCompAgCatTes);
                            }


                            List<AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro> filasCatTesauro = tesauroTempDW.ListaCatTesauroAgCatTesauro.Where(item => item.TesauroID.Equals(filaCatTesComp.TesauroOrigenID) && item.CategoriaSuperiorID.Equals(filaCatTesComp.CategoriaOrigenID)).ToList();

                            foreach (AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro filaCatTesAgCatTes in filasCatTesauro)
                            {
                                dataWrapperTesauro.ListaCatTesauroAgCatTesauro.Add(filaCatTesAgCatTes);
                                ImportarCategoriaCompartidaTesauro(filaCatTesComp.TesauroOrigenID, filaCatTesAgCatTes.CategoriaInferiorID, tesauroTempDW, dataWrapperTesauro);
                            }
                        }
                    }

                    mGestorTesauro = new GestionTesauro(dataWrapperTesauro, mLoggingService, mEntityContext);
                }
                return mGestorTesauro;
            }
        }
        /// <summary>
        /// Obtiene o establece la lista de categorías seleccionadas en la pantalla
        /// </summary>
        private List<Guid> CategoriasSeleccionadasIDs
        {
            get
            {
                if (mCategoriasSeleccionadasIDs == null)
                {
                    mCategoriasSeleccionadasIDs = new List<Guid>();

                    string categorias = RequestParams("CategoriasSeleccionadas");
                    if (!string.IsNullOrEmpty(categorias))
                    {
                        string[] IDstexto = RequestParams("CategoriasSeleccionadas").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        foreach (string id in IDstexto)
                        {
                            mCategoriasSeleccionadasIDs.Add(new Guid(id));
                        }
                    }
                }
                return mCategoriasSeleccionadasIDs;
            }
            set
            {
                mCategoriasSeleccionadasIDs = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de categorías expandidas en la pantalla
        /// </summary>
        public List<Guid> CategoriasCompartidasIDs
        {
            get
            {
                if (mCategoriasCompartidasIDs == null)
                {
                    mCategoriasCompartidasIDs = new List<Guid>();

                    foreach (AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaCatTesComp in GestorTesauro.TesauroDW.ListaCatTesauroCompartida)
                    {
                        mCategoriasCompartidasIDs.Add(filaCatTesComp.CategoriaOrigenID);
                    }
                }
                return mCategoriasCompartidasIDs;
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de categorías expandidas en la pantalla
        /// </summary>
        public List<Guid> CategoriasExpandidasIDs
        {
            get
            {
                if (mCategoriasExpandidasIDs == null)
                {
                    mCategoriasExpandidasIDs = new List<Guid>();
                    string categorias = RequestParams("CategoriasExpandidas");
                    if (!string.IsNullOrEmpty(categorias))
                    {
                        string[] IDstexto = RequestParams("CategoriasExpandidas").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        foreach (string id in IDstexto)
                        {
                            mCategoriasExpandidasIDs.Add(new Guid(id));
                        }
                    }
                }
                return mCategoriasExpandidasIDs;
            }
            set
            {
                mCategoriasExpandidasIDs = value;
            }
        }
        private string IdiomaDefecto
        {
            get
            {
                if (string.IsNullOrEmpty(mIdiomaDefecto))
                {
                    if (string.IsNullOrEmpty(ParametrosGeneralesRow.IdiomaDefecto))
                    {
                        mIdiomaDefecto = "es";
                    }
                    else
                    {
                        mIdiomaDefecto = ParametrosGeneralesRow.IdiomaDefecto.Trim();
                    }
                }
                return mIdiomaDefecto;
            }
        }
        private AdministrarPreferenciaProyectoViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarPreferenciaProyectoViewModel();

                    mPaginaModel.Thesaurus = new ThesaurusEditorModel();

                    mPaginaModel.PasosRealizados = RequestParams("PasosRealizados");

                    TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestorTesauro.TesauroDW.Merge(tesauroCN.ObtenerSugerenciasCatDeUnTesauro(GestorTesauro.TesauroDW.ListaTesauro.FirstOrDefault().TesauroID));
                    tesauroCN.Dispose();

                    GestorTesauro.CargarCategoriasSugerencia();

                    //EjecutarAcciones(mPaginaModel.PasosRealizados);

                    mPaginaModel.Thesaurus.ThesaurusCategories = ObtenerListaCategorias();

                    //mPaginaModel.Thesaurus.SuggestedThesaurusCategories = CategoriasSugeridas;

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mPaginaModel.Thesaurus.ExpandedCategories = CategoriasExpandidasIDs;
                    mPaginaModel.Thesaurus.SelectedCategories = proyCN.ObtenerCategoriasSeleccionadas(ProyectoSeleccionado.Clave);
                    mPaginaModel.Thesaurus.SharedCategories = CategoriasCompartidasIDs;

                    mPaginaModel.IdiomaDefecto = IdiomaDefecto;

                    if (!string.IsNullOrEmpty(RequestParams("multiLanguage")))
                    {
                        mPaginaModel.MultiLanguaje = bool.Parse(RequestParams("multiLanguage"));
                    }
                    else
                    {
                        mPaginaModel.MultiLanguaje = false;
                        foreach (CategoriaTesauro cat in GestorTesauro.ListaCategoriasTesauro.Values)
                        {
                            
                            foreach (string idioma in mConfigService.ObtenerListaIdiomas())
                            {
                                if (cat.FilaCategoria.Nombre.Contains("@" + idioma.ToString()))
                                {
                                    mPaginaModel.MultiLanguaje = true;
                                    break;
                                }
                                else if (idioma.Contains("-"))
                                {
                                    string idiomaAux = idioma.Substring(0, idioma.IndexOf('-'));
                                    if (cat.FilaCategoria.Nombre.Contains("@" + idiomaAux.ToString()))
                                    {
                                        mPaginaModel.MultiLanguaje = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    mPaginaModel.IdiomaTesauro = UtilIdiomas.LanguageCode;

                    if (mPaginaModel.MultiLanguaje && !string.IsNullOrEmpty(RequestParams("IdiomaSeleccionado")))
                    {
                        mPaginaModel.IdiomaTesauro = RequestParams("IdiomaSeleccionado");
                    }

                }
                return mPaginaModel;
            }
        }
        #endregion

    }
}