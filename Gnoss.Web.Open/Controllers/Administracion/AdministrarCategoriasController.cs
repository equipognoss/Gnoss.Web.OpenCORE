using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Tesauro;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Universal.Common.Extensions;
using Universal.Common;
using VDS.RDF.Query.Expressions.Functions.XPath.Cast;
using Microsoft.AspNetCore.Http.Extensions;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Microsoft.Extensions.Hosting;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
	/// <summary>
	/// Enumeración que contien las posibles acciones que se pueden realizar sobre categorías.
	/// </summary>
	public enum AccionConCategorias
	{
		CrearCategoria = 0,
		MoverCategoria = 1,
		EliminarCategoria = 2,
		AceptarSugerenciaCat = 3,
		RechazarSugerenciaCat = 4,
		OrdenarCategoria = 5,
		CambiarNombreCategoria = 6,
		EliminarCategoriaYMoverSoloHuerfanos = 7,
		SeleccionarUnicoIdioma = 8,
		CompartirCategoria = 9
	}

	/// <summary>
	/// Modelo de la página de administrar categorías
	/// </summary>
	[Serializable]
	public class AdministrarCategoriasViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		public ThesaurusEditorModel Thesaurus { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool MultiLanguaje { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string IdiomaDefecto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string IdiomaTesauro { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Action { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PasosRealizados { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public KeyValuePair<Guid, Dictionary<string, string>> Categoria { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool ExistenRecursosNoHuerfanos { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public KeyValuePair<Guid, string> ComunidadCompartir { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<CategoryModel> CategoriasCompartir { get; set; }
	}

	/// <summary>
	/// Tipos de acción de la página de administrar categorías
	/// </summary>
	public enum TypesAccion
	{
		/// <summary>
		/// Crear una categoría
		/// </summary>
		create,
		/// <summary>
		/// Cambiar el nombre a una categoría
		/// </summary>
		change_name,
		/// <summary>
		/// Mover una categoría
		/// </summary>
		move,
		/// <summary>
		/// Ordenar una categoría
		/// </summary>
		order,
		/// <summary>
		/// Eliminar una categoría
		/// </summary>
		delete,
		/// <summary>
		/// Compartir una categoría
		/// </summary>
		share
	}

	/// <summary>
	/// Controller de administrar categorías
	/// </summary>
	public class AdministrarCategoriasController : ControllerBaseWeb
	{

		public AdministrarCategoriasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
			: base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
		{
		}

		#region Miembros

		/// <summary>
		/// 
		/// </summary>
		private AdministrarCategoriasViewModel mPaginaModel = null;
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

		/// <summary>
		/// Diccionario con categorias a eliminar para actualizar el Base (todos los recursos y dafos se vinculan a la nueva categoria)
		/// </summary>
		private Dictionary<Guid, List<Guid>> mEliminarCategoriasActualizarBaseTodo;

		/// <summary>
		/// Diccionario con categorias a eliminar para actualizar el Base (solo los recursos y dafos huerfanos se vinculan a la nueva categoria)
		/// </summary>
		private Dictionary<Guid, List<Guid>> mEliminarCategoriasActualizarBaseSoloHuerfanos;

		/// <summary>
		/// Diccionario con categorias a mover para actualizar el Base la categoria clave sera la nueva padre de la lista de categorías
		/// </summary>
		private Dictionary<Guid, List<Guid>> mMoverCategoriasActualizarBase;

		/// <summary>
		/// 
		/// </summary>
		private DataWrapperDocumentacion mDocumentacionDW;

		private DataWrapperDocumentacion mDataWrapperDocumentacion;
		/// <summary>
		/// 
		/// </summary>
		private DataWrapperSuscripcion mSuscripcionDW;
		/// <summary>
		/// 
		/// </summary>
		private DataWrapperProyecto mDataWrapperProyecto;
		/// <summary>
		/// 
		/// </summary>
		private Guid mBaseRecursosID = Guid.Empty;
		/// <summary>
		/// 
		/// </summary>
		private List<CategoryModel> mCategoriasSugeridas;
		/// <summary>
		/// 
		/// </summary>
		private GestionTesauro mGestorTesauro = null;

		#endregion

		/// <summary>
		/// Muestra la pagina de administrar categorías
		/// </summary>
		/// <returns>ActionResult</returns>
		[TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Tesauro })]
		public ActionResult Index()
		{
			EliminarPersonalizacionVistas();
			CargarPermisosAdministracionComunidadEnViewBag();

			// Añadir clase para el body del Layout
			ViewBag.BodyClassPestanya = "edicionCategorias";
			ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
			ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_Categorias;
			// Establecer el título para el header de DevTools                      
			ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTRUCTURA");
			ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONBASICA", "CATEGORIAS");

			// Activar la visualización del icono de la documentación de la sección
			ViewBag.showDocumentationByDefault = "true";
			// Indica si sólo se desea visualizar la documentación por secciones (Ej: Mostrar la documentación en el modal con el contenido desplegado/plegado)
			ViewBag.showDocumentationSection = "true";


			short i = 0;
			foreach (CategoriaTesauro cat in GestorTesauro.ListaCategoriasTesauroPrimerNivel.Values.OrderBy(cat => cat.FilaCategoria.Orden))
			{
				cat.FilaCategoria.Orden = i;
				if (GestorTesauro.TesauroActualID != cat.FilaCategoria.TesauroID)
				{
					mEntityContext.AceptarCambios(cat.FilaCategoria);
					AD.EntityModel.Models.Tesauro.CatTesauroCompartida catCompartida = GestorTesauro.TesauroDW.ListaCatTesauroCompartida.Where(item => item.TesauroOrigenID.Equals(cat.FilaCategoria.TesauroID) && item.CategoriaOrigenID.Equals(cat.FilaCategoria.CategoriaTesauroID) && item.TesauroDestinoID.Equals(GestorTesauro.TesauroActualID)).FirstOrDefault();
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
		/// Devolver la vista modal para ser insertada en un modal de bootstrap. Preguntará al usuario si desea cancelar la edición de multilenguaje de categorías        
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public ActionResult LoadDisableMultilenguage()
		{
			ActionResult partialView = View();
			partialView = GnossResultHtml("../AdministrarCategorias/_modal-views/_disable-multilenguage-categories", null);

			// Devolver la vista modal
			return partialView;
		}




		/// <summary>
		/// Muestra el panel para realizar la acción solicitada
		/// </summary>
		/// <returns>ActionResult</returns>
		[TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Tesauro })]
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
					IElementoGnoss padre = GestorTesauro.ListaCategoriasTesauro[CategoriasSeleccionadasIDs[0]].Padre;

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

					PaginaModel.ComunidadCompartir = new KeyValuePair<Guid, string>(idComunidad, nombreComunidad);

					TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
					DataWrapperTesauro tesauroDW = tesauroCL.ObtenerTesauroDeProyecto(idComunidad);
					GestionTesauro gesTesauro = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);

					PaginaModel.CategoriasCompartir = CargarTesauroPorGestorTesauro(gesTesauro);
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
						EliminarCategoriasTesauro(Guid.Empty, false);

						CategoriasSeleccionadasIDs = new List<Guid>();

						RecargarPagniaModel();

						//Devolvemos un multiView con una sola vista para poder identificarla desde javascript
						MultiViewResult multiView = new MultiViewResult(this, mViewEngine);
						multiView.AddView("_Tesauro", "tesauro", PaginaModel);
						return multiView;
					}
					else
					{

						TesauroCN tesCN2 = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
						bool existenRecursosNoHuerfanos = tesCN2.ObtenerSiExistenElementosNoHuerfanos(GestorTesauro.TesauroActualID, listaCatSeleccionadasEHijos);
						tesCN2.Dispose();

						PaginaModel.ExistenRecursosNoHuerfanos = existenRecursosNoHuerfanos;
					}
				}
				else if (typeAction == TypesAccion.share.ToString() && ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
				{
					ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
					DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerProyectosParticipaUsuario(mControladorBase.UsuarioActual.UsuarioID);

					Dictionary<Guid, string> listaProyectos = new Dictionary<Guid, string>();

					foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in dataWrapperProyecto.ListaProyecto)
					{
						listaProyectos.Add(filaProy.ProyectoID, filaProy.Nombre);
					}

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
		[TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Tesauro })]
		public ActionResult EjecutarAccion(string typeAction)
		{
			GuardarLogAuditoria();
			PaginaModel.Action = typeAction;

			switch (typeAction)
			{
				case "create":
					return CrearCategoria_Event();
				case "change-name":
					return CambiarNombre_Event();
				case "move":
					return MoverCategoria_Event();
				case "order":
					return OrdenarCategoria_Event();
				case "delete":
					return EliminarCategoria_Event();
				case "multilanguaje":
					return CambiarIdioma_Event();
				case "onlylanguaje":
					return IdiomaUnico_Event();
				case "acept-category":
					return AceptarSolicitudCategoria_Event();
				case "reject-category":
					return RechazarSolicitudCategoria_Event();
				case "share":
					return CompartirCategoria_Event();
			}

			return GnossResultERROR();
		}

		/// <summary>
		/// Guardar los cambios
		/// </summary>
		/// <returns>ActionResult</returns>
		[TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Tesauro })]
		public ActionResult Guardar()
		{
			GuardarLogAuditoria();
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

			List<Object> categoriasModificadas = mEntityContext.ObtenerElementosModificados(typeof(AD.EntityModel.Models.Tesauro.CategoriaTesauro));

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
					mLoggingService.GuardarLogError(ex);
				}
			}

			List<object> categoriasEliminadas = mEntityContext.ObtenerElementosEliminados(typeof(AD.EntityModel.Models.Tesauro.CategoriaTesauro));

			List<Guid> listaCategoriasEliminadas = new List<Guid>();
			if ((categoriasEliminadas != null) && (categoriasEliminadas.Count > 0))
			{
				foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCat in categoriasEliminadas)
				{
					if (!listaCategoriasEliminadas.Contains(mEntityContext.ObtenerValorOriginalDeObjeto<Guid>(filaCat, nameof(filaCat.CategoriaTesauroID))))
					{
						listaCategoriasEliminadas.Add(mEntityContext.ObtenerValorOriginalDeObjeto<Guid>(filaCat, nameof(filaCat.CategoriaTesauroID)));
					}
				}
			}

			mEntityContext.SaveChanges();

			ControladorDocumentacion.AgregarEliminacionCategoriasModeloBase(EliminarCategoriasActualizarBaseTodo, mControladorBase.UsuarioActual.ProyectoID, true, PrioridadBase.Baja);
			ControladorDocumentacion.AgregarEliminacionCategoriasModeloBase(EliminarCategoriasActualizarBaseSoloHuerfanos, mControladorBase.UsuarioActual.ProyectoID, false, PrioridadBase.Baja);
			ControladorDocumentacion.AgregarMoverCategoriasModeloBase(MoverCategoriasActualizarBase, mControladorBase.UsuarioActual.ProyectoID, PrioridadBase.Baja);

			TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			tesauroCL.InvalidarCacheDeTesauroDeProyecto(ProyectoSeleccionado.Clave);
			tesauroCL.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);
			tesauroCL.Dispose();

			ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
			proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
			proyCL.Dispose();

			FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
			facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "");
			facetadoCL.Dispose();
			ControladorFacetas contrFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, null, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
			contrFacetas.InvalidarCaches(UrlIntragnoss);
			PaginaModel.PasosRealizados = "";

			return PartialView("_Tesauro", PaginaModel);

		}

        /// <summary>
        /// Carga el modal para crear una subcategoría dentro de otra
        /// </summary>
        /// <param name="categoriaPadreID">Id de la categoría padre de la cual se va a crear su hija</param>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Tesauro })]
		public ActionResult CargarModalCrearCategoriaEnCategoria(Guid categoriaPadreID)
		{
            PintarModalCrearSubCategoria model = new PintarModalCrearSubCategoria();

            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			model.NombreCategoriaPadre = tesauroCN.ObtenerNombreCategoriaPorID(categoriaPadreID, IdiomaUsuario);
			model.CategoriaId = categoriaPadreID;
            model.IdiomaTesauro = UtilIdiomas.LanguageCode;
			model.MultiLanguaje = bool.Parse(RequestParams("multiLanguage"));


            if (model.MultiLanguaje && !string.IsNullOrEmpty(RequestParams("IdiomaSeleccionado")))
            {
                model.IdiomaTesauro = RequestParams("IdiomaSeleccionado");
            }

            if (!string.IsNullOrEmpty(RequestParams("multiLanguage")))
            {
                model.MultiLanguaje = bool.Parse(RequestParams("multiLanguage"));
            }
            else
            {
                model.MultiLanguaje = false;
                foreach (CategoriaTesauro cat in GestorTesauro.ListaCategoriasTesauro.Values)
                {
                    foreach (string idioma in paramCL.ObtenerListaIdiomas())
                    {
                        if (cat.FilaCategoria.Nombre.Contains("@" + idioma.ToString()))
                        {
                            model.MultiLanguaje = true;
                            break;
                        }
                        else if (idioma.Contains("-"))
                        {
                            string idiomaAux = idioma.Substring(0, idioma.IndexOf('-'));
                            if (cat.FilaCategoria.Nombre.Contains("@" + idiomaAux.ToString()))
                            {
                                model.MultiLanguaje = true;
                                break;
                            }
                        }
                    }
                }
            }

            return PartialView("_modal-views/_add-categories-in-category", model);
		}

		/// <summary>
		/// Acción de crear una categoría
		/// </summary>
		/// <returns>Devuelve el tesauro modificado</returns>
		private ActionResult CrearCategoria_Event()
		{
			GuardarLogAuditoria();
			bool nombreRepetido = false;

			Guid guidPadre = new Guid(RequestParams("parentKey"));
			string nombre = RequestParams("name");


			Dictionary<string, string> listaCatIdiomas = new Dictionary<string, string>();

			string[] arrayCatIdiomas = nombre.Split(new string[] { "$$$" }, StringSplitOptions.None);

			foreach (string catIdioma in arrayCatIdiomas)
			{
				string[] arrayCat = catIdioma.Split(new string[] { "@@@" }, StringSplitOptions.None);

				listaCatIdiomas.Add(arrayCat[0], arrayCat[1]);
			}

			if (guidPadre != Guid.Empty)
			{
				foreach (CategoriaTesauro catTes in GestorTesauro.ListaCategoriasTesauro[guidPadre].Hijos)
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
			else
			{
				foreach (CategoriaTesauro catTes in GestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
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
				Guid IDNuevaCategoria = Guid.NewGuid();

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

				PaginaModel.PasosRealizados += ((short)AccionConCategorias.CrearCategoria).ToString() + "|_|" + guidPadre.ToString() + "|_|" + nombreCategoria.Trim() + "|_|" + IDNuevaCategoria + "|,|";

				CrearCategoriaTesauro(guidPadre, IDNuevaCategoria, nombreCategoria.Trim());

				if (guidPadre != Guid.Empty)
				{
					List<Guid> catSel = CategoriasExpandidasIDs;
					catSel.Add(guidPadre);
					CategoriasExpandidasIDs = catSel;
				}

				RecargarPagniaModel();

				return PartialView("_Tesauro", PaginaModel);
				//return UtilAJAX.AgregarJavaScripADevolucionCallBack(UtilAJAX.RefrescarControlesCallBack(true, TipoPeticionAjax.Normal, panTesauro, PanParaSelectorCategorias, panTxtAcciones, txtAccionesTesauroHack), "SlideUp();LimpiarCamposCrearCategoriaTesauro();");
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
		/// Acción de cambiar el nombre a una categoría
		/// </summary>
		/// <returns>Devuelve el tesauro modificado</returns>
		private ActionResult CambiarNombre_Event()
		{
			GuardarLogAuditoria();
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

			foreach (IElementoGnoss catTes in GestorTesauro.ListaCategoriasTesauro[categoriaSeleccionada].Padre.Hijos)
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
		/// Acción de mover una categoría
		/// </summary>
		/// <returns>Devuelve el tesauro modificado</returns>
		private ActionResult MoverCategoria_Event()
		{
			GuardarLogAuditoria();
			Guid guidPadre = new Guid(RequestParams("parentKey"));

			PaginaModel.PasosRealizados += ((short)AccionConCategorias.MoverCategoria).ToString() + "|_|" + guidPadre.ToString();

			List<Guid> IDCatSeleccionadas = new List<Guid>();

			foreach (Guid catTesID in CategoriasSeleccionadasIDs)
			{
				PaginaModel.PasosRealizados += "|_|" + catTesID.ToString();
				IDCatSeleccionadas.Add(catTesID);
			}
			PaginaModel.PasosRealizados += "|,|";

			MoverCategoriasTesauro(guidPadre, CategoriasSeleccionadasIDs);

			if (guidPadre != Guid.Empty)
			{
				List<Guid> catSel = CategoriasExpandidasIDs;
				catSel.Add(guidPadre);
				CategoriasExpandidasIDs = catSel;
			}

			RecargarPagniaModel();

			return PartialView("_Tesauro", PaginaModel);
			//return UtilAJAX.AgregarJavaScripADevolucionCallBack(UtilAJAX.RefrescarControlesCallBack(true, TipoPeticionAjax.Normal, panTesauro, PanParaSelectorCategorias, panTxtAcciones, txtAccionesTesauroHack), "SlideUp();");
		}

		/// <summary>
		/// Acción de ordenar una categoría
		/// </summary>
		/// <returns>Devuelve el tesauro modificado</returns>
		private ActionResult OrdenarCategoria_Event()
		{
			//Guid categoriaPadre = new Guid(RequestParams("parentKey"));
			string ordenDestino = RequestParams("newOrderCategory");

			PaginaModel.PasosRealizados += $"{(short)AccionConCategorias.OrdenarCategoria}|_|{ordenDestino}";

			foreach (Guid guidCatSel in CategoriasSeleccionadasIDs)
			{
				PaginaModel.PasosRealizados += $"|_|{guidCatSel}";
			}

			PaginaModel.PasosRealizados += "|,|";

			OrdenarCategoriasTesauro(Int32.Parse(ordenDestino), CategoriasSeleccionadasIDs);

			RecargarPagniaModel();

			return PartialView("_Tesauro", PaginaModel);
		}

		/// <summary>
		/// Acción de eliminar una categoría
		/// </summary>
		/// <returns>Devuelve el tesauro modificado</returns>
		private ActionResult EliminarCategoria_Event()
		{
			GuardarLogAuditoria();
			Guid guidCatSup = new Guid(RequestParams("parentKey"));

			string accion = RequestParams("moveTo");

			EliminarCategoriasTesauro(guidCatSup, (accion == "HUERFANOS"));

			//error = UtilIdiomas.GetText("COMADMINCATEGORIAS", "GUARDAROBLIGADO");

			CategoriasSeleccionadasIDs = new List<Guid>();

			RecargarPagniaModel();
			TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			tesauroCL.InvalidarCacheDeTesauroDeProyecto(ProyectoSeleccionado.Clave);
			tesauroCL.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);
			tesauroCL.Dispose();

			ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
			proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
			proyCL.Dispose();

			FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
			facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "");
			facetadoCL.Dispose();
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
		/// Acción de aceptar una categoría sugerida
		/// </summary>
		/// <returns>Devuelve el tesauro modificado</returns>
		private ActionResult AceptarSolicitudCategoria_Event()
		{
			Guid guidCatSugerencia = new Guid(RequestParams("CategoryKey"));

			List<AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia> filasCatSug = GestorTesauro.TesauroDW.ListaCategoriaTesauroSugerencia.Where(item => item.SugerenciaID.Equals(guidCatSugerencia)).ToList();

			if (filasCatSug.Count > 0)
			{
				bool nombreRepetido = false;

				if (filasCatSug[0].CategoriaTesauroPadreID.HasValue)
				{
					foreach (IElementoGnoss catTes in GestorTesauro.ListaCategoriasTesauro[filasCatSug[0].CategoriaTesauroPadreID.Value].Hijos)
					{
						if (((CategoriaTesauro)catTes).Nombre[IdiomaUsuario].ToLower() == filasCatSug[0].Nombre.ToLower())
						{
							nombreRepetido = true;
						}
					}
				}
				else
				{
					foreach (IElementoGnoss catTes in GestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
					{
						if (((CategoriaTesauro)catTes).Nombre[IdiomaUsuario].ToLower() == filasCatSug[0].Nombre.ToLower())
						{
							nombreRepetido = true;
						}
					}
				}
				if (!nombreRepetido)
				{
					Guid IDNuevaCategoria = Guid.NewGuid();
					PaginaModel.PasosRealizados += ((short)AccionConCategorias.AceptarSugerenciaCat).ToString() + "|_|" + guidCatSugerencia.ToString() + "|_|" + IDNuevaCategoria.ToString() + "|,|";
					AceptarSugerenciaCategoria(guidCatSugerencia, IDNuevaCategoria);

					CategoryModel catSugerida = PaginaModel.Thesaurus.SuggestedThesaurusCategories.Find(cat => cat.Key == guidCatSugerencia);
					PaginaModel.Thesaurus.SuggestedThesaurusCategories.Remove(catSugerida);
					RecargarPagniaModel();
					TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
					tesauroCL.InvalidarCacheDeTesauroDeProyecto(ProyectoSeleccionado.Clave);
					tesauroCL.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);
					tesauroCL.Dispose();

					ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
					proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
					proyCL.Dispose();

					FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
					facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "");
					facetadoCL.Dispose();
					return PartialView("_Tesauro", PaginaModel);
				}
				else
				{
					return GnossResultERROR(UtilIdiomas.GetText("PERFILBASE", "ERROR_CAT_DEBE_SER_NOMBRE_DIFERENTE"));
				}
			}

			return GnossResultERROR();
		}

		/// <summary>
		/// Acción de rechazar una categoría sugerida
		/// </summary>
		/// <returns>Devuelve el tesauro modificado</returns>
		private ActionResult RechazarSolicitudCategoria_Event()
		{
			Guid guidCatSugerencia = new Guid(RequestParams("CategoryKey"));

			List<AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia> filasCatSug = GestorTesauro.TesauroDW.ListaCategoriaTesauroSugerencia.Where(item => item.SugerenciaID.Equals(guidCatSugerencia)).ToList();

			if (filasCatSug.Count > 0)
			{
				filasCatSug[0].Estado = (short)EstadoSugerenciaCatTesauro.Rechazada;
				PaginaModel.PasosRealizados += ((short)AccionConCategorias.RechazarSugerenciaCat).ToString() + "|_|" + guidCatSugerencia.ToString() + "|,|";
				RechazarSugerenciaCategoriaDefinitiva(guidCatSugerencia);


				CategoryModel catSugerida = PaginaModel.Thesaurus.SuggestedThesaurusCategories.Find(cat => cat.Key == guidCatSugerencia);
				PaginaModel.Thesaurus.SuggestedThesaurusCategories.Remove(catSugerida);

				RecargarPagniaModel();
				TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
				tesauroCL.InvalidarCacheDeTesauroDeProyecto(ProyectoSeleccionado.Clave);
				tesauroCL.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);
				tesauroCL.Dispose();

				ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
				proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
				proyCL.Dispose();

				FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
				facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "");
				facetadoCL.Dispose();
				return PartialView("_Tesauro", PaginaModel);
			}

			return GnossResultERROR();
		}

		/// <summary>
		/// Acción de compartir una categoría
		/// </summary>
		/// <returns>Devuelve el tesauro modificado</returns>
		private ActionResult CompartirCategoria_Event()
		{
			GuardarLogAuditoria();
			Guid idComunidad = new Guid(RequestParams("ComunidadCompartirKey"));
			Guid idCategoria = new Guid(RequestParams("CategoriaCompartirKey"));

			CategoriaTesauro catTesauro = GestorTesauro.ListaCategoriasTesauroPrimerNivel.OrderBy(cat => cat.Value.FilaCategoria.Orden).Last().Value;

			TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			Guid tesauroOrigenID = tesauroCN.ObtenerIDTesauroDeProyecto(idComunidad);
			tesauroCN.Dispose();

			CompartirCategoria(idCategoria, tesauroOrigenID, (short)(catTesauro.FilaCategoria.Orden + 1));

			PaginaModel.PasosRealizados += ((short)AccionConCategorias.CompartirCategoria).ToString() + "|_|" + idCategoria.ToString() + "|_|" + tesauroOrigenID.ToString() + "|_|" + (catTesauro.FilaCategoria.Orden + 1).ToString() + "|,|";

			RecargarPagniaModel();

			return PartialView("_Tesauro", PaginaModel);
		}

		/// <summary>
		/// Crea la fila de la categoria compartida
		/// </summary>
		/// <param name="pCategoriaID">Categoria tesauro compartida</param>
		/// <param name="pTeauroID">Tesauro de la categoria compartida</param>
		/// <param name="pOrden">Orden en el que se mete la categoria compartida</param>
		private void CompartirCategoria(Guid pCategoriaID, Guid pTeauroID, short pOrden)
		{
			AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaCatTesauroCompartida = new AD.EntityModel.Models.Tesauro.CatTesauroCompartida();
			filaCatTesauroCompartida.TesauroOrigenID = pTeauroID;
			filaCatTesauroCompartida.CategoriaOrigenID = pCategoriaID;
			filaCatTesauroCompartida.TesauroDestinoID = GestorTesauro.TesauroActualID;
			filaCatTesauroCompartida.Orden = pOrden;
			GestorTesauro.TesauroDW.ListaCatTesauroCompartida.Add(filaCatTesauroCompartida);

			TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			DataWrapperTesauro tesauroTempDW = tesauroCN.ObtenerTesauroPorID(pTeauroID);

			Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCatTesauro = tesauroTempDW.ListaCategoriaTesauro.FirstOrDefault(item => item.TesauroID.Equals(pTeauroID) && item.CategoriaTesauroID.Equals(pCategoriaID));
			filaCatTesauro.Orden = pOrden;
			GestorTesauro.TesauroDW.ListaCategoriaTesauro.Add(filaCatTesauro);

			CategoriasCompartidasIDs.Add(pCategoriaID);

			GestorTesauro.RecargarGestor();
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
				CategoriaTesauro categoriaPadre = GestorTesauro.ListaCategoriasTesauro[pIdCategoriaPadre];
				categoriaNueva = GestorTesauro.AgregarSubcategoria(categoriaPadre, pNombreCategoria.Trim());
				(GestorTesauro.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaInferiorID.Equals(categoriaNueva.Clave)).FirstOrDefault()).CategoriaInferiorID = pIdNuevaCategoria;

			}
			else
			{
				categoriaNueva = GestorTesauro.AgregarCategoriaPrimerNivel(pNombreCategoria.Trim());
			}
			(GestorTesauro.TesauroDW.ListaCategoriaTesauro.Where(item => item.CategoriaTesauroID.Equals(categoriaNueva.Clave)).FirstOrDefault()).CategoriaTesauroID = pIdNuevaCategoria;

			GestorTesauro.RecargarGestor();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pOrdenOriginal"></param>
		/// <param name="pCatID"></param>
		/// <param name="pCatTesauroSupID"></param>
		private void RestarOrdenCategoriasMover(short pOrdenOriginal, Guid pCatID, Guid pCatTesauroSupID)
		{
			int ordenDestino = GestorTesauro.ListaCategoriasTesauro[pCatID].FilaAgregacion.Orden;
			var sql = GestorTesauro.TesauroDW.ListaCatTesauroCompartida.Where(item => !item.CategoriaSupDestinoID.HasValue);

			if (!pCatTesauroSupID.Equals(Guid.Empty))
			{
				sql = sql.Where(item => item.CategoriaSupDestinoID.Equals(pCatTesauroSupID));
			}

			AD.EntityModel.Models.Tesauro.CatTesauroCompartida[] catCompHermanas = sql.Where(item => !item.CategoriaOrigenID.Equals(pCatID)).ToArray();

			foreach (AD.EntityModel.Models.Tesauro.CatTesauroCompartida catTesHermana in catCompHermanas)
			{
				if (pOrdenOriginal < catTesHermana.Orden)
				{
					catTesHermana.Orden = (short)(catTesHermana.Orden - 1);
					GestorTesauro.ListaCategoriasTesauro[catTesHermana.CategoriaOrigenID].FilaCategoria.Orden = catTesHermana.Orden;
					//GestorTesauro.ListaCategoriasTesauro[catTesHermana.CategoriaOrigenID].FilaCategoria.AcceptChanges();
					mEntityContext.AceptarCambios(GestorTesauro.ListaCategoriasTesauro[catTesHermana.CategoriaOrigenID].FilaCategoria);
				}
			}

			if (!pCatTesauroSupID.Equals(Guid.Empty))
			{
				List<IElementoGnoss> catHermanas = GestorTesauro.ListaCategoriasTesauro[pCatTesauroSupID].Hijos;

				foreach (CategoriaTesauro catTesHermana in catHermanas)
				{
					if (!CategoriasCompartidasIDs.Contains(catTesHermana.Clave) && pOrdenOriginal < catTesHermana.FilaAgregacion.Orden && ordenDestino > catTesHermana.FilaAgregacion.Orden)
					{
						catTesHermana.FilaAgregacion.Orden = (short)(catTesHermana.FilaAgregacion.Orden - 1);
						catTesHermana.FilaCategoria.Orden = catTesHermana.FilaAgregacion.Orden;
					}
					else if (!CategoriasCompartidasIDs.Contains(catTesHermana.Clave) && pOrdenOriginal > catTesHermana.FilaAgregacion.Orden && ordenDestino <= catTesHermana.FilaAgregacion.Orden)
					{
						catTesHermana.FilaAgregacion.Orden = (short)(catTesHermana.FilaAgregacion.Orden + 1);
						catTesHermana.FilaCategoria.Orden = catTesHermana.FilaAgregacion.Orden;
					}
				}
			}
			else
			{
				foreach (CategoriaTesauro catTesRaiz in GestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
				{
					if (!CategoriasCompartidasIDs.Contains(catTesRaiz.Clave) && pOrdenOriginal < catTesRaiz.FilaCategoria.Orden && ordenDestino > catTesRaiz.FilaCategoria.Orden)
					{
						catTesRaiz.FilaCategoria.Orden = (short)(catTesRaiz.FilaCategoria.Orden - 1);
					}
					else if (!CategoriasCompartidasIDs.Contains(catTesRaiz.Clave) && pOrdenOriginal > catTesRaiz.FilaCategoria.Orden && ordenDestino <= catTesRaiz.FilaCategoria.Orden)
					{
						catTesRaiz.FilaCategoria.Orden = (short)(catTesRaiz.FilaCategoria.Orden + 1);
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCategoriaPadre"></param>
		/// <returns></returns>
		private short ObtenerNuevoOrdenMover(Guid pCategoriaPadre)
		{
			var sql = GestorTesauro.TesauroDW.ListaCatTesauroCompartida.Where(item => !item.CategoriaSupDestinoID.HasValue);

			if (!pCategoriaPadre.Equals(Guid.Empty))
			{
				sql = sql.Where(item => item.CategoriaSupDestinoID.Equals(pCategoriaPadre));
			}

			short nuevoOrden = 0;
			AD.EntityModel.Models.Tesauro.CatTesauroCompartida[] catCompHermanas = sql.OrderByDescending(item => item.Orden).ToArray();
			if (catCompHermanas.Length > 0)
			{
				nuevoOrden = (short)(catCompHermanas[0].Orden + 1);
			}

			if (pCategoriaPadre != Guid.Empty)
			{
				List<IElementoGnoss> catHermanas = GestorTesauro.ListaCategoriasTesauro[pCategoriaPadre].Hijos;
				foreach (CategoriaTesauro catTesHermana in catHermanas)
				{
					if (!CategoriasCompartidasIDs.Contains(catTesHermana.Clave) && catTesHermana.FilaAgregacion.Orden >= nuevoOrden)
					{
						nuevoOrden = (short)(catTesHermana.FilaAgregacion.Orden + 1);
					}
				}
			}
			else
			{
				foreach (CategoriaTesauro catTesRaiz in GestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
				{
					if (!CategoriasCompartidasIDs.Contains(catTesRaiz.Clave) && catTesRaiz.FilaCategoria.Orden >= nuevoOrden)
					{
						nuevoOrden = (short)(catTesRaiz.FilaCategoria.Orden + 1);
					}
				}
			}
			return nuevoOrden;
		}

		/// <summary>
		/// Mueve una serie de categorías del tesauro.
		/// </summary>
		private void MoverCategoriasTesauro(Guid pCategoriaPadre, List<Guid> pCategoriasAMover)
		{
			if (CategoriasCompartidasIDs.Contains(pCategoriaPadre))
			{
				return;
			}
			foreach (Guid guidCat in pCategoriasAMover)
			{
				if (CategoriasCompartidasIDs.Contains(guidCat))
				{
					AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaCatCompartida = GestorTesauro.TesauroDW.ListaCatTesauroCompartida.Where(item => item.CategoriaOrigenID.Equals(guidCat)).FirstOrDefault();
					CategoriaTesauro categoriaTesauro = GestorTesauro.ListaCategoriasTesauro[filaCatCompartida.CategoriaOrigenID];


					Guid catTesauroSupID = Guid.Empty;
					if (filaCatCompartida.CategoriaSupDestinoID.HasValue)
					{
						catTesauroSupID = filaCatCompartida.CategoriaSupDestinoID.Value;
					}

					bool categoriaPadreDistintaActual = pCategoriaPadre != catTesauroSupID;

					if (categoriaTesauro.Padre is CategoriaTesauro && categoriaPadreDistintaActual)
					{
						categoriaTesauro.Padre.Hijos.Remove(categoriaTesauro);
						GestorTesauro.DesasignarSubcategoriaDeCategoria(categoriaTesauro, (CategoriaTesauro)categoriaTesauro.Padre);

						GestorTesauro.RecargarGestor();
					}

					if (pCategoriaPadre != Guid.Empty && categoriaPadreDistintaActual)
					{
						GestorTesauro.AgregarSubcategoriaACategoria(categoriaTesauro, GestorTesauro.ListaCategoriasTesauro[pCategoriaPadre]);
					}

					if (pCategoriaPadre.Equals(Guid.Empty))
					{
						filaCatCompartida.CategoriaSupDestinoID = null;
					}
					else
					{
						filaCatCompartida.CategoriaSupDestinoID = pCategoriaPadre;
					}

					RestarOrdenCategoriasMover(filaCatCompartida.Orden, guidCat, catTesauroSupID);

					filaCatCompartida.Orden = ObtenerNuevoOrdenMover(pCategoriaPadre);
					categoriaTesauro.FilaCategoria.Orden = filaCatCompartida.Orden;

					mEntityContext.AceptarCambios(categoriaTesauro.FilaCategoria);
					mEntityContext.AceptarCambios(categoriaTesauro.FilaAgregacion);

					GestorTesauro.RecargarGestor();
				}
				else
				{
					bool categoriaPadreDistintaActual = true;

					if (GestorTesauro.ListaCategoriasTesauro[guidCat].Padre is CategoriaTesauro)
					{
						if (((CategoriaTesauro)GestorTesauro.ListaCategoriasTesauro[guidCat].Padre).Clave == pCategoriaPadre)
						{
							categoriaPadreDistintaActual = false;
						}
					}
					else if (pCategoriaPadre == Guid.Empty)
					{
						categoriaPadreDistintaActual = false;
					}

					//Ordenamos elementos no raíz si la categoria a mover no es raiz
					short ordenOriginal = 0;
					if (GestorTesauro.ListaCategoriasTesauro[guidCat].FilaAgregacion != null)
					{
						ordenOriginal = GestorTesauro.ListaCategoriasTesauro[guidCat].FilaAgregacion.Orden;
					}
					else
					{
						ordenOriginal = GestorTesauro.ListaCategoriasTesauro[guidCat].FilaCategoria.Orden;
					}

					Guid clave = Guid.Empty;
					if (GestorTesauro.ListaCategoriasTesauro[guidCat].Padre is CategoriaTesauro)
					{
						clave = ((CategoriaTesauro)(GestorTesauro.ListaCategoriasTesauro[guidCat].Padre)).Clave;
					}

					if (GestorTesauro.ListaCategoriasTesauro[guidCat].Padre is CategoriaTesauro && categoriaPadreDistintaActual)
					{
						GestorTesauro.ListaCategoriasTesauro[guidCat].Padre.Hijos.Remove(GestorTesauro.ListaCategoriasTesauro[guidCat]);
						GestorTesauro.DesasignarSubcategoriaDeCategoria(GestorTesauro.ListaCategoriasTesauro[guidCat], (CategoriaTesauro)GestorTesauro.ListaCategoriasTesauro[guidCat].Padre);

						GestorTesauro.RecargarGestor();
					}

					if (pCategoriaPadre != Guid.Empty && categoriaPadreDistintaActual)
					{
						GestorTesauro.AgregarSubcategoriaACategoria(GestorTesauro.ListaCategoriasTesauro[guidCat], GestorTesauro.ListaCategoriasTesauro[pCategoriaPadre]);
					}
				}
			}

			if (MoverCategoriasActualizarBase.ContainsKey(pCategoriaPadre))
			{
				foreach (Guid catID in pCategoriasAMover)
				{
					if (!MoverCategoriasActualizarBase[pCategoriaPadre].Contains(catID))
					{
						MoverCategoriasActualizarBase[pCategoriaPadre].Add(catID);
					}
				}
			}
			else
			{
				MoverCategoriasActualizarBase.Add(pCategoriaPadre, pCategoriasAMover);
			}
		}

		/// <summary>
		/// Eliminar una serie de categorías del tesauro.
		/// </summary>
		private void EliminarCategoriasTesauro(Guid pCategoriaSustitutaID, bool pMoverSoloLoJusto)
		{
			List<Guid> listaCatEliminarIDs = new List<Guid>();

			foreach (Guid catTeID in CategoriasSeleccionadasIDs)
			{
				AgregarCategoriaEHijosALista(GestorTesauro.ListaCategoriasTesauro[catTeID], listaCatEliminarIDs);
			}

			if (pMoverSoloLoJusto)
			{
				PaginaModel.PasosRealizados += ((short)AccionConCategorias.EliminarCategoriaYMoverSoloHuerfanos).ToString() + "|_|" + pCategoriaSustitutaID.ToString();
			}
			else
			{
				PaginaModel.PasosRealizados += ((short)AccionConCategorias.EliminarCategoria).ToString() + "|_|" + pCategoriaSustitutaID.ToString();
			}
			foreach (Guid idCatEliminar in listaCatEliminarIDs)
			{
				PaginaModel.PasosRealizados += "|_|" + idCatEliminar.ToString();
			}
			PaginaModel.PasosRealizados += "|,|";

			EliminarCategoriasTesauro(pCategoriaSustitutaID, listaCatEliminarIDs, pMoverSoloLoJusto);
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
			bool tieneRecursosAsociados = tesCN.EstanVinculadasCategoriasTesauro(GestorTesauro.TesauroActualID, pListaCategoriasAEliminar);
			tesCN.Dispose();
			if (tieneRecursosAsociados)
			{
				MoverDependenciasCategoriasAOtras(pIDCategoriaVincularm, pListaCategoriasAEliminar, pMoverSoloLoJusto);
			}

			//Eliminamos las preferencias
			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			DataWrapperProyecto.Merge(proyectoCN.ObtenerPreferenciasProyectoPorID(ProyectoSeleccionado.Clave));
			proyectoCN.Dispose();


			foreach (Guid IDCatAEliminar in pListaCategoriasAEliminar)
			{
				List<PreferenciaProyecto> filas = DataWrapperProyecto.ListaPreferenciaProyecto.Where(pref => pref.CategoriaTesauroID.Equals(IDCatAEliminar)).ToList();
				if (filas.Count == 1)
				{
					DataWrapperProyecto.ListaPreferenciaProyecto.Remove(filas.First());
					mEntityContext.EliminarElemento(filas.First());
				}

				CategoriaTesauro catTe = GestorTesauro.ListaCategoriasTesauro[IDCatAEliminar];
				if (!catTe.EstaEliminado)
				{
					GestorTesauro.EliminarCategoriaEHijos(catTe);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCategoria"></param>
		/// <param name="pListaCategoriaID"></param>
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
		/// Acepta una sugerencia de tesauro.
		/// </summary>
		/// <param name="pSugerenciaID">Identificador de la sugerencia.</param>
		private void AceptarSugerenciaCategoria(Guid pSugerenciaID, Guid pNuevaCategoria)
		{
			CategoriaTesauro categoriaNueva = null;

			AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia[] filasCatSug = GestorTesauro.TesauroDW.ListaCategoriaTesauroSugerencia.Where(item => item.SugerenciaID.Equals(pSugerenciaID)).ToArray();

			if (!filasCatSug[0].CategoriaTesauroPadreID.HasValue)
			{
				categoriaNueva = GestorTesauro.AgregarCategoriaPrimerNivel(filasCatSug[0].Nombre.Trim());
			}
			else
			{
				CategoriaTesauro categoriaPadre = GestorTesauro.ListaCategoriasTesauro[filasCatSug[0].CategoriaTesauroPadreID.Value];
				categoriaNueva = GestorTesauro.AgregarSubcategoria(categoriaPadre, filasCatSug[0].Nombre.Trim());
				GestorTesauro.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaInferiorID.Equals(categoriaNueva.Clave)).FirstOrDefault().CategoriaInferiorID = pNuevaCategoria;
			}
			
			GestorTesauro.TesauroDW.ListaCategoriaTesauro.Where(item => item.CategoriaTesauroID.Equals(categoriaNueva.Clave)).FirstOrDefault().CategoriaTesauroID = pNuevaCategoria;

			filasCatSug[0].CategoriaTesauroAceptadaID = pNuevaCategoria;
			filasCatSug[0].Estado = (short)EstadoSugerenciaCatTesauro.Aceptada;

			mEntityContext.SaveChanges();

			TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			tesauroCL.InvalidarCacheDeTesauroDeProyecto(ProyectoSeleccionado.Clave);
		}

		/// <summary>
		/// Rechaza una sugerencia de tesauro.
		/// </summary>
		/// <param name="pSugerenciaID">Identificador de la sugerencia.</param>
		private void RechazarSugerenciaCategoriaDefinitiva(Guid pSugerenciaID)
		{
			AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia[] filasCatSug = GestorTesauro.TesauroDW.ListaCategoriaTesauroSugerencia.Where(item => item.SugerenciaID.Equals(pSugerenciaID)).ToArray();

			if (filasCatSug.Length > 0)
			{
				filasCatSug[0].Estado = (short)EstadoSugerenciaCatTesauro.Rechazada;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pOrden"></param>
		/// <param name="pListaCategorias"></param>
		private void OrdenarCategoriasTesauro(int pOrden, List<Guid> pListaCategorias)
		{
			Guid categoriaPadre = Guid.Empty;

			List<CategoriaTesauro> listaCategoriasSeleccionadas = new List<CategoriaTesauro>();
			foreach (Guid idCategoria in pListaCategorias)
			{
				listaCategoriasSeleccionadas.Add(GestorTesauro.ListaCategoriasTesauro[idCategoria]);
			}

			//Almacenamos los ordenes antiguos de las categorias que vamos a ordenar
			List<short> ordenesAntiguos = new List<short>();
			foreach (CategoriaTesauro catTes in listaCategoriasSeleccionadas)
			{
				if (CategoriasCompartidasIDs.Contains(catTes.Clave))
				{
					ordenesAntiguos.Add(catTes.FilaCategoria.Orden);
					AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaCatComp = GestorTesauro.TesauroDW.ListaCatTesauroCompartida.Where(item => item.CategoriaOrigenID.Equals(catTes.Clave)).FirstOrDefault();

					if (filaCatComp.CategoriaSupDestinoID.HasValue)
					{
						categoriaPadre = filaCatComp.CategoriaSupDestinoID.Value;
					}
				}
				else if (GestorTesauro.ListaCategoriasTesauroPrimerNivel.ContainsKey(catTes.Clave))
				{
					ordenesAntiguos.Add(catTes.FilaCategoria.Orden);
					categoriaPadre = Guid.Empty;
				}
				else
				{
					ordenesAntiguos.Add(catTes.FilaAgregacion.Orden);
					categoriaPadre = ((CategoriaTesauro)catTes.Padre).Clave;
				}
			}
			var listaCategoriasHemanas = GestorTesauro.TesauroDW.ListaCatTesauroCompartida.Where(item => !item.CategoriaSupDestinoID.HasValue);
			if (!categoriaPadre.Equals(Guid.Empty))
			{
				listaCategoriasHemanas = listaCategoriasHemanas.Where(item => item.CategoriaSupDestinoID.Equals(categoriaPadre));
			}

			short cont = 0;

			foreach (AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaTesCompartida in listaCategoriasHemanas)
			{
				filaTesCompartida.Orden = ObtenerOrdenOrdenar(pListaCategorias.Contains(filaTesCompartida.CategoriaOrigenID), ordenesAntiguos, filaTesCompartida.Orden, pOrden, cont);
				GestorTesauro.ListaCategoriasTesauro[filaTesCompartida.CategoriaOrigenID].FilaCategoria.Orden = filaTesCompartida.Orden;
				mEntityContext.AceptarCambios(GestorTesauro.ListaCategoriasTesauro[filaTesCompartida.CategoriaOrigenID].FilaCategoria);
			}

			if (categoriaPadre != Guid.Empty)
			{
				List<IElementoGnoss> catHermanas = GestorTesauro.ListaCategoriasTesauro[categoriaPadre].Hijos;
				foreach (CategoriaTesauro catTesHermana in catHermanas)
				{
					if (!CategoriasCompartidasIDs.Contains(catTesHermana.Clave))
					{
						catTesHermana.FilaAgregacion.Orden = ObtenerOrdenOrdenar(pListaCategorias.Contains(catTesHermana.Clave), ordenesAntiguos, catTesHermana.FilaAgregacion.Orden, pOrden, cont);
						catTesHermana.FilaCategoria.Orden = catTesHermana.FilaAgregacion.Orden;
					}
				}
			}
			else
			{
				foreach (CategoriaTesauro catTesRaiz in GestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
				{
					if (!CategoriasCompartidasIDs.Contains(catTesRaiz.Clave))
					{
						catTesRaiz.FilaCategoria.Orden = ObtenerOrdenOrdenar(pListaCategorias.Contains(catTesRaiz.Clave), ordenesAntiguos, catTesRaiz.FilaCategoria.Orden, pOrden, cont);
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pEstaSeleccionada"></param>
		/// <param name="pOrdenesAntiguos"></param>
		/// <param name="pOrdenCatSeleccionada"></param>
		/// <param name="pOrden"></param>
		/// <param name="pCont"></param>
		/// <returns></returns>
		private short ObtenerOrdenOrdenar(bool pEstaSeleccionada, List<short> pOrdenesAntiguos, short pOrdenCatSeleccionada, int pOrden, short pCont)
		{
			if (!pEstaSeleccionada)
			{
				short cambioOrden = 0;

				foreach (short ordenAntiguo in pOrdenesAntiguos)
				{
					if ((ordenAntiguo > pOrdenCatSeleccionada && pOrden <= pOrdenCatSeleccionada) || (ordenAntiguo == pOrdenCatSeleccionada && pOrden <= pOrdenCatSeleccionada))
					{
						cambioOrden++;
					}
					if (ordenAntiguo < pOrdenCatSeleccionada && pOrden >= pOrdenCatSeleccionada)
					{
						cambioOrden--;
					}
				}
				return (short)(pOrdenCatSeleccionada + cambioOrden);
			}
			else
			{
				return (short)pOrden;
			}
		}

		/// <summary>
		/// Cambia el nombre a una categoría del tesauro.
		/// </summary>
		private void CambiarNombreCategoriasTesauro(Guid pIdCategoria, string pNombreCategoria)
		{
			GestorTesauro.ListaCategoriasTesauro[pIdCategoria].FilaCategoria.Nombre = pNombreCategoria;
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
			DataWrapperDocumentacion dataWrapperDocumentacion = docCN.ObtenerVinculacionDocumentosDeCategoriasTesauro(pListaCategoriasParaSustituirIDs, GestorTesauro.TesauroActualID);
			docCN.Dispose();

			List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.ToList();
			foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocAgTes in listaDocumentoWebAgCatTesauro)
			{
				if (mEntityContext.Entry(filaDocAgTes).State != EntityState.Deleted)
				{
					//Si la categoría a la que pertenece el recurso hay q sustituirla y no es la sustituta hay que proceder con ella
					if (pListaCategoriasParaSustituirIDs.Contains(filaDocAgTes.CategoriaTesauroID) && filaDocAgTes.CategoriaTesauroID != pCategoriaSustitutaID)
					{
						mEntityContext.EliminarElemento(filaDocAgTes);
						dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Remove(filaDocAgTes);
						//si pMoverSoloLoJusto y tiene otras categorías que no se van a borrar, se borra la vinculación con esta categoría
						//o ya está vinculado con la categoría destino
						if (!((pMoverSoloLoJusto && dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Any(doc => doc.DocumentoID.Equals(filaDocAgTes.DocumentoID) && doc.BaseRecursosID.Equals(filaDocAgTes.BaseRecursosID) && !pListaCategoriasParaSustituirIDs.Contains(doc.BaseRecursosID))) || dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Any(doc => doc.DocumentoID.Equals(filaDocAgTes.DocumentoID) && doc.BaseRecursosID.Equals(filaDocAgTes.BaseRecursosID) && doc.CategoriaTesauroID.Equals(pCategoriaSustitutaID))))
						{
							AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro nuevaCategoria = new AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro();
							nuevaCategoria.BaseRecursosID = filaDocAgTes.BaseRecursosID;
							nuevaCategoria.CategoriaTesauroID = pCategoriaSustitutaID;
							nuevaCategoria.DocumentoID = filaDocAgTes.DocumentoID;
							nuevaCategoria.Fecha = filaDocAgTes.Fecha;
							nuevaCategoria.TesauroID = filaDocAgTes.TesauroID;

							mEntityContext.DocumentoWebAgCatTesauro.Add(nuevaCategoria);
							dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Add(nuevaCategoria);
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

			#region Muevo las suscripciones de las categorias

			SuscripcionCN suscripCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			DataWrapperSuscripcion suscripDW = suscripCN.ObtenerVinculacionesSuscripcionesDeCategoriasTesauro(pListaCategoriasParaSustituirIDs, GestorTesauro.TesauroActualID);
			suscripCN.Dispose();

			List<AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip> listaCategoriaTesVinSuscripBorrar = suscripDW.ListaCategoriaTesVinSuscrip.ToList();
			foreach (AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip filaSuscripAgTes in listaCategoriaTesVinSuscripBorrar)
			{
				if (mEntityContext.Entry(filaSuscripAgTes).State != EntityState.Deleted)
				{
					suscripDW.ListaCategoriaTesVinSuscrip.Remove(filaSuscripAgTes);
					mEntityContext.EliminarElemento(filaSuscripAgTes);
				}
			}
			SuscripcionDW.Merge(suscripDW);

			#endregion

			#region Muevo los proyectos de las categorias

			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			DataWrapperProyecto dataWrapperProyecto = proyectoCN.ObtenerVinculacionProyectosDeCategoriasTesauro(pListaCategoriasParaSustituirIDs, GestorTesauro.TesauroActualID);
			proyectoCN.Dispose();
			List<ProyectoAgCatTesauro> listaProyectoAgCatTesauroBorrar = dataWrapperProyecto.ListaProyectoAgCatTesauro.ToList();
			foreach (ProyectoAgCatTesauro filaProyAgTes in listaProyectoAgCatTesauroBorrar)
			{
				if (mEntityContext.Entry(filaProyAgTes).State != EntityState.Deleted)
				{
					if ((DataWrapperProyecto.ListaProyectoAgCatTesauro.Any(proy => proy.ProyectoID.Equals(filaProyAgTes.ProyectoID) && proy.CategoriaTesauroID.Equals(pCategoriaSustitutaID)) || dataWrapperProyecto.ListaProyectoAgCatTesauro.Any(proy => proy.ProyectoID.Equals(filaProyAgTes.ProyectoID) && proy.CategoriaTesauroID.Equals(pCategoriaSustitutaID))) && filaProyAgTes.CategoriaTesauroID != pCategoriaSustitutaID)
					{
						DataWrapperProyecto.ListaProyectoAgCatTesauro.Remove(filaProyAgTes);
						dataWrapperProyecto.ListaProyectoAgCatTesauro.Remove(filaProyAgTes);
						mEntityContext.EliminarElemento(filaProyAgTes);
					}
					else
					{
						filaProyAgTes.CategoriaTesauroID = pCategoriaSustitutaID;
					}
				}
			}
			DataWrapperProyecto.Merge(dataWrapperProyecto);

			#endregion

			#region Muevo los dafos de las categorias

			//DafoProyectoCN dafoCN = new DafoProyectoCN();
			//DafoProyectoDS dafoDS = dafoCN.ObtenerVinculacionDafosDeCategoriasTesauro(pListaCategoriasParaSustituirIDs, GestorTesauro.TesauroActualID);
			//docCN.Dispose();

			//foreach (DafoProyectoDS.DafoProyectoAgCatTesauroRow filaDafoAgTes in dafoDS.DafoProyectoAgCatTesauro)
			//{
			//    if (filaDafoAgTes.RowState != DataRowState.Deleted)
			//    {
			//        //Si la categoría a la que pertenece el recurso hay q sustituirla y no es la sustituta hay que proceder con ella
			//        if (pListaCategoriasParaSustituirIDs.Contains(filaDafoAgTes.CategoriaTesauroID) && filaDafoAgTes.CategoriaTesauroID != pCategoriaSustitutaID)
			//        {
			//            string categoriasEliminadas = "";
			//            int num = 0;
			//            foreach (Guid catID in pListaCategoriasParaSustituirIDs)
			//            {
			//                if (num > 0)
			//                {
			//                    categoriasEliminadas += " AND ";
			//                }
			//                categoriasEliminadas += " CategoriaTesauroID<>'" + catID + "'";
			//                num++;
			//            }

			//            //si pMoverSoloLoJusto y tiene otras categorías que no se van a borrar, se borra la vinculación con esta categoría
			//            //o ya está vinculado con la categoría destino
			//            if (pMoverSoloLoJusto && dafoDS.DafoProyectoAgCatTesauro.Select("DafoID='" + filaDafoAgTes.DafoID + "' AND CategoriaTesauroID='" + pCategoriaSustitutaID + "'").Length > 0)
			//            {
			//                filaDafoAgTes.Delete();
			//            }
			//            else
			//            {
			//                //si no se cambia (porque es la única)
			//                filaDafoAgTes.CategoriaTesauroID = pCategoriaSustitutaID;
			//            }
			//        }
			//    }
			//}
			//DafoProyectoDS.Merge(dafoDS);

			#endregion

			pListaCategoriasParaSustituirIDs.Remove(pCategoriaSustitutaID);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pAcciones"></param>
		private void EjecutarAcciones(string pAcciones)
		{
			if (!string.IsNullOrEmpty(pAcciones))
			{
				string[] acciones = pAcciones.Split(new string[] { "|,|" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string accion in acciones)
				{
					string[] accion2 = accion.Split(new string[] { "|_|" }, StringSplitOptions.RemoveEmptyEntries);
					switch (accion2[0])
					{
						case "0"://crear
							Guid idPadre = new Guid(accion2[1]);
							Guid idNuevaCategoria = new Guid(accion2[3]);
							string nombreNuevaCat = accion2[2];
							CrearCategoriaTesauro(idPadre, idNuevaCategoria, nombreNuevaCat.Trim());
							break;
						case "1"://MoverCategorias
							Guid idNuevoPadre = new Guid(accion2[1]);
							List<Guid> idCat = new List<Guid>();
							for (int i = 2; i < accion2.Length; i++)
							{
								idCat.Add(new Guid(accion2[i]));
							}
							MoverCategoriasTesauro(idNuevoPadre, idCat);
							break;
						case "2"://EliminarCategoria                        
							Guid idCategoriaVincular = new Guid(accion2[1]);
							List<Guid> idCategorias = new List<Guid>();
							for (int i = 2; i < accion2.Length; i++)
							{
								idCategorias.Add(new Guid(accion2[i]));
							}
							EliminarCategoriasTesauro(idCategoriaVincular, idCategorias, false);
							break;
						case "7"://EliminarCategoriaYMoverSoloHuerfanos                        
							Guid idCategoriaVincular2 = new Guid(accion2[1]);
							List<Guid> idCategorias2 = new List<Guid>();
							for (int i = 2; i < accion2.Length; i++)
							{
								idCategorias2.Add(new Guid(accion2[i]));
							}
							EliminarCategoriasTesauro(idCategoriaVincular2, idCategorias2, true);
							break;
						case "3"://aceptarsugerencia                        
							Guid idCategoriaSugerida = new Guid(accion2[1]);
							Guid idNuevaCategoriaAceptar = new Guid(accion2[2]);
							AceptarSugerenciaCategoria(idCategoriaSugerida, idNuevaCategoriaAceptar);
							break;
						case "4"://RechazarSugerenciaCat
							Guid idCategoriaSugeridaARechazar = new Guid(accion2[1]);
							RechazarSugerenciaCategoriaDefinitiva(idCategoriaSugeridaARechazar);
							break;
						case "5"://OrdenarCategoria
							int ordenDestino = int.Parse(accion2[1]);
							List<Guid> idCategoriasOrdenar = new List<Guid>();
							for (int i = 2; i < accion2.Length; i++)
							{
								idCategoriasOrdenar.Add(new Guid(accion2[i]));
							}
							OrdenarCategoriasTesauro(ordenDestino, idCategoriasOrdenar);
							break;
						case "6"://cambiarnombre
							Guid idCategoria = new Guid(accion2[1]);
							string nombreCategoria = accion2[2];
							CambiarNombreCategoriasTesauro(idCategoria, nombreCategoria);
							break;
						case "8"://seleccionar único idioma
							string claveIdioma = accion2[1];
							CambiarIdiomaTesauro(claveIdioma);
							break;
						case "9"://compartirCategoria
							Guid categoriaCompartirID = new Guid(accion2[1]);
							Guid tesauroCompartirID = new Guid(accion2[2]);
							short ordenCategoria = short.Parse(accion2[3]);
							CompartirCategoria(categoriaCompartirID, tesauroCompartirID, ordenCategoria);
							break;
					}
					GestorTesauro.RecargarGestor();
				}
			}

		}


		//TODO CORE: Invalidar cache de categorias cuando se crear/editan/borran nuevas categorías desde Administración



		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCategoriaTesauro"></param>
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

		#region Propiedades

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
		/// Diccionario con categorias a mover para actualizar el Base la categoria clave sera la nueva padre de la lista de categorías
		/// </summary>
		private Dictionary<Guid, List<Guid>> MoverCategoriasActualizarBase
		{
			get
			{
				if (mMoverCategoriasActualizarBase == null)
				{
					mMoverCategoriasActualizarBase = new Dictionary<Guid, List<Guid>>();
				}

				return mMoverCategoriasActualizarBase;
			}
			set
			{
				mMoverCategoriasActualizarBase = value;
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

		/// <summary>
		/// 
		/// </summary>
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

		/// <summary>
		/// 
		/// </summary>
		private List<CategoryModel> CategoriasSugeridas
		{
			get
			{
				if (mCategoriasSugeridas == null)
				{
					mCategoriasSugeridas = new List<CategoryModel>();

					foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia filaCatSug in GestorTesauro.TesauroDW.ListaCategoriaTesauroSugerencia)
					{
						if (mEntityContext.Entry(filaCatSug).State != EntityState.Deleted)
						{
							if (filaCatSug.Estado == (short)EstadoSugerenciaCatTesauro.Espera)
							{
								CategoryModel cat = new CategoryModel();
								cat.Key = filaCatSug.SugerenciaID;

								cat.Name = filaCatSug.Nombre;
								cat.LanguageName = filaCatSug.Nombre;

								if (!filaCatSug.CategoriaTesauroPadreID.HasValue)
								{
									cat.ParentCategoryKey = Guid.Empty;
								}
								else
								{
									cat.ParentCategoryKey = filaCatSug.CategoriaTesauroPadreID.Value;
								}
								mCategoriasSugeridas.Add(cat);
							}
						}
					}
				}
				return mCategoriasSugeridas;
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

			foreach (CategoriaTesauro catTes in GestorTesauro.ListaCategoriasTesauroPrimerNivel.Values)
			{
				CategoryModel categoriaTesauro = CargarCategoria(catTes, pIdiomaTesauro);
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
		private AdministrarCategoriasViewModel PaginaModel
		{
			get
			{
				if (mPaginaModel == null)
				{
					mPaginaModel = new AdministrarCategoriasViewModel();

					mPaginaModel.Thesaurus = new ThesaurusEditorModel();

					mPaginaModel.PasosRealizados = RequestParams("PasosRealizados");

					TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
					GestorTesauro.TesauroDW.Merge(tesauroCN.ObtenerSugerenciasCatDeUnTesauro(GestorTesauro.TesauroDW.ListaTesauro.FirstOrDefault().TesauroID));
					tesauroCN.Dispose();

					GestorTesauro.CargarCategoriasSugerencia();

					EjecutarAcciones(mPaginaModel.PasosRealizados);

					mPaginaModel.Thesaurus.ThesaurusCategories = ObtenerListaCategorias();

					mPaginaModel.Thesaurus.SuggestedThesaurusCategories = CategoriasSugeridas;

					mPaginaModel.Thesaurus.ExpandedCategories = CategoriasExpandidasIDs;
					mPaginaModel.Thesaurus.SelectedCategories = CategoriasSeleccionadasIDs;
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
							foreach (string idioma in paramCL.ObtenerListaIdiomas())
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

		/// <summary>
		/// Obtiene o establece el dataset de documentación
		/// </summary>
		public DataWrapperDocumentacion DataWrapperDocumentacion
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
		/// Obtiene o establece el dataset de suscripciones
		/// </summary>
		public DataWrapperSuscripcion SuscripcionDW
		{
			get
			{
				if (mSuscripcionDW == null)
				{
					mSuscripcionDW = new DataWrapperSuscripcion();
				}
				return mSuscripcionDW;
			}
			set
			{
				mSuscripcionDW = value;
			}
		}

		/// <summary>
		/// Obtiene o establece el dataset de proyectos
		/// </summary>
		public DataWrapperProyecto DataWrapperProyecto
		{
			get
			{
				if (mDataWrapperProyecto == null)
				{
					mDataWrapperProyecto = new DataWrapperProyecto();
				}
				return mDataWrapperProyecto;
			}
			set
			{
				mDataWrapperProyecto = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected GestionTesauro GestorTesauro
		{
			get
			{
				if (mGestorTesauro == null)
				{
					TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
					DataWrapperTesauro dataWrapperTesauro = tesauroCN.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);

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
		/// 
		/// </summary>
		/// <param name="pTesauroID"></param>
		/// <param name="pCategoriaID"></param>
		/// <param name="pTesauroOrigen"></param>
		/// <param name="pTesauroDestino"></param>
		private void ImportarCategoriaCompartidaTesauro(Guid pTesauroID, Guid pCategoriaID, DataWrapperTesauro pTesauroOrigen, DataWrapperTesauro pTesauroDestino)
		{
			AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCatTesauro = pTesauroOrigen.ListaCategoriaTesauro.FirstOrDefault(item => item.TesauroID.Equals(pTesauroID) && item.CategoriaTesauroID.Equals(pCategoriaID));
			pTesauroDestino.ListaCategoriaTesauro.Add(filaCatTesauro);

			List<AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro> filasCatTesauro = pTesauroOrigen.ListaCatTesauroAgCatTesauro.Where(item => item.TesauroID.Equals(pTesauroID) && item.CategoriaSuperiorID.Equals(pCategoriaID)).ToList();

			foreach (AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro filaCatTesAgCatTes in filasCatTesauro)
			{
				pTesauroDestino.ListaCatTesauroAgCatTesauro.Add(filaCatTesAgCatTes);
				ImportarCategoriaCompartidaTesauro(pTesauroID, filaCatTesAgCatTes.CategoriaInferiorID, pTesauroOrigen, pTesauroDestino);
			}

		}

		/// <summary>
		/// 
		/// </summary>
		private Guid BaseRecursosID
		{
			get
			{
				if (mBaseRecursosID.Equals(Guid.Empty))
				{
					DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
					DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

					DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
					docCL.ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, UsuarioActual.UsuarioID);
					docCN.Dispose();

					mBaseRecursosID = dataWrapperDocumentacion.ListaBaseRecursosProyecto.FirstOrDefault().BaseRecursosID;
				}
				return mBaseRecursosID;
			}
		}

		#endregion
	}
}
