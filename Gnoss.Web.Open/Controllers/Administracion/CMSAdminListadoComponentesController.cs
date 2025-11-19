using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Enumeración para distinguir tipos de Componetes disponibles para el CMS
    /// </summary>
    public enum CMSComponentType
    {
        /// <summary>
        /// Representa un HTML plano
        /// </summary>
        HTML = 0,
        /// <summary>
        /// Representa un HTML destacado
        /// </summary>
        Important = 1,
        /// <summary>
        /// Representa un listado dinamico
        /// </summary>
        DinamicList = 2,
        /// <summary>
        /// Representa un listado estático
        /// </summary>
        StaticList = 3,
        /// <summary>
        /// Representa la actividad reciente
        /// </summary>
        RecentActivity = 4,
        /// <summary>
        /// Representa un grupo de componentes
        /// </summary>
        ComponentsGroup = 5,
        /// <summary>
        /// Representa una sección del tesauro de la comunidad
        /// </summary>
        Thesaurus = 6,
        ///// <summary>
        ///// Representa los recursos destacados de la comunidad
        ///// </summary>
        //ImportantResources = 7,
        /// <summary>
        /// Representa los datos de la comunidad, numero de recursos y personas y organizaciones
        /// </summary>
        CommunityData = 8,
        /// <summary>
        /// Representa los usuarios recomendados
        /// </summary>
        RecomendedUsers = 9,
        /// <summary>
        /// Representa una caja de buscador
        /// </summary>
        SearchBox = 10,
        ///// <summary>
        ///// Representa los recursos destacados estaticos de la comunidad
        ///// </summary>
        //StaticImportantResources = 11,
        ///// <summary>
        ///// Representa un refcurso destacado
        ///// </summary>
        //ImportantResource = 12,
        /// <summary>
        /// Representa una faceta
        /// </summary>
        Facet = 13,
        /// <summary>
        /// ListadoUsuarios
        /// </summary>
        UsersList = 14,
        /// <summary>
        /// ListadoProyectos
        /// </summary>
        ProyectsList = 15,
        /// <summary>
        /// ResumenPerfil
        /// </summary>
        ProfileSummary = 16,
        /// <summary>
        /// MasVistos
        /// </summary>
        MostViewed = 17,
        /// <summary>
        /// EnvioCorreo
        /// </summary>
        MailDelivery = 18,
        /// <summary>
        /// PreguntaTIC
        /// </summary>
        TICQuestion = 19,
        /// <summary>
        /// Menu
        /// </summary>
        Menu = 20,
        /// <summary>
        /// Buscador
        /// </summary>
        Searcher = 21,
        /// <summary>
        /// BuscadorSPARQL
        /// </summary>
        SPARQLSearcher = 22,
        /// <summary>
        /// UltimosRecursosVisitados
        /// </summary>
        LastViewedResources = 23,
        /// <summary>
        /// Ficha descripción documento
        /// </summary>
        DocumentDescriptionTab = 24,
        /// <summary>
        /// MasVistos en x dias
        /// </summary>
        MostViewedInXDays = 25,
        /// <summary>
        /// ConsultaSPARQL
        /// </summary>
        SPARQLQuery = 26,
        /// <summary>
        /// ConsultaSQLSERVER
        /// </summary>
        SQLSERVERQuery = 27
    }

    /// <summary>
    /// CMSComponentList Administration View Model
    /// </summary>
    public class CMSComponentListViewModel
    {
        /// <summary>
        /// Indica si la vista es la de selección de componente
        /// </summary>
        public bool IsSelectionView;
        /// <summary>
        /// Contiene los resultados de la búsqueda de componentes
        /// </summary>
        public ResultadoModel Result;
        /// <summary>
        /// Contiene las facetas de la búsqueda de componentes
        /// </summary>
        public List<FacetModel> FacetList;
        /// <summary>
        /// Lista de filtros para la búsqueda de componentes
        /// </summary>
        public List<FacetItemModel> FilterList;
        /// <summary>
        /// Lista con los componentes disponibles para crear
        /// </summary>
        public Dictionary<CMSComponentType, string> AvailableComponentsList;
        /// <summary>
        /// Identificador del bloque contenedor del componente
        /// </summary>
        public Guid CMSContainerBlockID;
    }

    public class CMSAdminListadoComponentesController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public CMSAdminListadoComponentesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<CMSAdminDisenioController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros

        private CMSComponentListViewModel mPaginaModel = null;

        #endregion

        #region Métodos públicos
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.VerComponenteCMS, (ulong)PermisoContenidos.EliminarComponenteCMS, (ulong)PermisoContenidos.CrearComponenteCMS, (ulong)PermisoContenidos.EditarComponenteCMS } })]
        public ActionResult Index(string tipoComponente, string idBloqueContenedor, string pagina, string search, string numusos)
        {
            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "estructura edicion edicionComponentes listado no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_Componentes;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTRUCTURA");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONPAGINAS", "LISTADOCOMPONENTESCMS");

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            CargarPermisosCMSViewBag();

            if (!ParametrosGeneralesRow.CMSDisponible)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADGENERAL"));
            }

            NumPagina = 1;
            TipoComponente = -1;
            Busqueda = "";
            NumUsos = -1;

            RecogerParametros(tipoComponente, idBloqueContenedor, pagina, search, numusos);
            return View(PaginaModel);
        }

        #endregion

        #region Propiedades

        private CMSComponentListViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new CMSComponentListViewModel();

                    if (BloqueID != Guid.Empty)
                    {
                        mPaginaModel.IsSelectionView = true;
                        mPaginaModel.CMSContainerBlockID = BloqueID;
                    }

                    CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
                    GestionCMS gestorCMSConFiltros = null;
                    if (TipoComponente == -1)
                    {
                        gestorCMSConFiltros = new GestionCMS(CMSCN.ObtenerComponentesCMSDeProyecto(ProyectoSeleccionado.Clave, Busqueda), mLoggingService, mEntityContext);
                    }
                    else
                    {
                        gestorCMSConFiltros = new GestionCMS(CMSCN.ObtenerComponentesCMSDeProyectoDelTipoEspecificado(ProyectoSeleccionado.Clave, (TipoComponenteCMS)TipoComponente, Busqueda), mLoggingService, mEntityContext);
                    }

                    GestionCMS gestorCMSSinFiltros = new GestionCMS(CMSCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);

                    List<TipoComponenteCMS> componentesDisponibles = new List<TipoComponenteCMS>();
                    foreach (TipoComponenteCMS tipoComponenteCMS in UtilComponentes.ListaComponentesPublicos)
                    {
                        if (!componentesDisponibles.Contains(tipoComponenteCMS))
                        {
                            componentesDisponibles.Add(tipoComponenteCMS);
                        }
                    }
                    foreach (TipoComponenteCMS tipoComponenteCMS in gestorCMSSinFiltros.ListaComponentesPrivadosProyecto)
                    {
                        if (!componentesDisponibles.Contains(tipoComponenteCMS))
                        {
                            componentesDisponibles.Add(tipoComponenteCMS);
                        }
                    }

                    Dictionary<Guid, int> componenteApariciones = new Dictionary<Guid, int>();
                    //Inicializamos
                    foreach (CMSComponente componente in gestorCMSConFiltros.ListaComponentes.Values)
                    {
                        componenteApariciones.Add(componente.Clave, 0);
                    }

                    //Cargamos los componentes que aparecen en los bloques
                    foreach (CMSBloque bloque in gestorCMSSinFiltros.ListaBloques.Values)
                    {
                        if (!bloque.Borrador)
                        {
                            foreach (CMSComponente componente in bloque.Componentes.Values)
                            {
                                if (componenteApariciones.ContainsKey(componente.Clave))
                                {
                                    componenteApariciones[componente.Clave]++;
                                }
                            }
                        }
                    }

                    //Cargamos los componentes que aparecen dentro de otros componentes
                    foreach (CMSBloque bloque in gestorCMSSinFiltros.ListaBloques.Values)
                    {
                        foreach (CMSComponente componente in bloque.Componentes.Values)
                        {
                            if (componente is CMSComponenteGrupoComponentes)
                            {
                                foreach (Guid idComponente in ((CMSComponenteGrupoComponentes)componente).ListaGuids)
                                {
                                    if (componenteApariciones.ContainsKey(idComponente))
                                    {
                                        componenteApariciones[idComponente]++;
                                    }
                                }
                            }
                        }
                    }

                    //Num apariciones-- numComponentes
                    SortedDictionary<int, int> numeroDeUsos = new SortedDictionary<int, int>();
                    foreach (CMSComponente componente in gestorCMSConFiltros.ListaComponentes.Values)
                    {
                        int numApariciones = componenteApariciones[componente.Clave];
                        if (!numeroDeUsos.ContainsKey(numApariciones))
                        {
                            numeroDeUsos.Add(numApariciones, 0);
                        }
                        numeroDeUsos[numApariciones]++;
                    }

                    mPaginaModel.Result = new ResultadoModel();
                    mPaginaModel.FacetList = new List<FacetModel>();

                    //Faceta Tipo de componente
                    FacetModel facetaTipoComponente = new FacetModel();
                    mPaginaModel.FacetList.Add(facetaTipoComponente);
                    facetaTipoComponente.Name = "Tipo de componente";
                    facetaTipoComponente.FacetItemList = new List<FacetItemModel>();

                    //Faceta Número de usos
                    FacetModel facetaUsos = new FacetModel();
                    mPaginaModel.FacetList.Add(facetaUsos);
                    facetaUsos.Name = "Número de usos";
                    facetaUsos.FacetItemList = new List<FacetItemModel>();

                    Dictionary<TipoComponenteCMS, int> diccionarioFacetasTiposComponentes = new Dictionary<TipoComponenteCMS, int>();
                    int cont = 0;
                    int i = 1;

                    CorregirFechaActualizacionComponentesAntiguos();
                    foreach (CMSComponente componente in gestorCMSConFiltros.ListaComponentes.Values.OrderByDescending(item => item.FilaComponente.FechaUltimaActualizacion.Value))
                    {
                        if (NumUsos == -1 || (componenteApariciones.ContainsKey(componente.Clave) && componenteApariciones[componente.Clave] == NumUsos))
                        {
                            #region Componentes

                            if (i > (NumPagina - 1) * 10 && i <= (NumPagina) * 10)
                            {
                                if (mPaginaModel.Result.ListaResultados == null)
                                {
                                    mPaginaModel.Result.ListaResultados = new List<ObjetoBuscadorModel>();
                                }

                                CMSEditComponentModel ficha = new CMSEditComponentModel();
                                ficha.Title = componente.Nombre;
                                ficha.Key = componente.Clave;
                                ficha.CMSComponentType = componente.TipoComponenteCMS;
                                ficha.Versionado = CMSCN.ObtenerVersionesComponenteCMS(componente.Clave).Count > 0;
                                if (componente.FilaComponente.FechaUltimaActualizacion.HasValue)
                                {
                                    ficha.EditionDate = componente.FilaComponente.FechaUltimaActualizacion.Value;
                                }   
                                ficha.Activo = componente.Activo;
                                ficha.PermisoLectura = true;

                                if (componente.FilaComponente.EstadoID.HasValue)
                                {
                                    UtilFlujos utilFlujos = new UtilFlujos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilFlujos>(), mLoggerFactory);
                                    ficha.PermisoLectura = utilFlujos.IdentidadTienePermisoLecturaEnEstado(componente.FilaComponente.EstadoID.Value, IdentidadActual.Clave, Guid.Empty);
                                }
                                
                                mPaginaModel.Result.ListaResultados.Add(ficha);
                            }
                            i++;
                            cont++;

                            #endregion

                            //Tipo de componente
                            if (!diccionarioFacetasTiposComponentes.ContainsKey(componente.TipoComponenteCMS))
                            {
                                diccionarioFacetasTiposComponentes.Add(componente.TipoComponenteCMS, 0);
                            }
                            diccionarioFacetasTiposComponentes[componente.TipoComponenteCMS]++;
                        }
                    }

                    CMSCN.Dispose();
                                                            
                    #region Facetas                            
                    
                    //Tipo de componente
                    foreach (TipoComponenteCMS tipoComp in diccionarioFacetasTiposComponentes.Keys)
                    {
                        int numComponentes = diccionarioFacetasTiposComponentes[tipoComp];
                        if (numComponentes > 0)
                        {
                            FacetItemModel facetaItem = new FacetItemModel();
                            facetaItem.Name = TipoComponenteToString(tipoComp);
                            facetaItem.Number = numComponentes;
                            facetaItem.Filter = "tipoComponente=" + (int)tipoComp;
                            if (TipoComponente == (int)tipoComp)
                            {
                                facetaItem.Selected = true;
                            }
                            facetaTipoComponente.FacetItemList.Add(facetaItem);
                        }
                    }

                    //Número de usos
                    foreach (int numeroUsos in numeroDeUsos.Keys)
                    {
                        int numComponentes = numeroDeUsos[numeroUsos];
                        FacetItemModel facetaItem = new FacetItemModel();
                        facetaItem.Name = numeroUsos.ToString();
                        facetaItem.Number = numComponentes;
                        facetaItem.Filter = "numusos=" + numeroUsos + "";
                        if (NumUsos == numeroUsos)
                        {
                            facetaItem.Selected = true;
                        }
                        facetaUsos.FacetItemList.Add(facetaItem);
                    }

                    //Crear componentes
                    // Permitir crear componentes incluso si se están buscando componentes (TipoComponente == 0
                    //if (TipoComponente == -1)
                    //{
                        mPaginaModel.AvailableComponentsList = new Dictionary<CMSComponentType, string>();

                        foreach (TipoComponenteCMS tipoComp in Enum.GetValues(typeof(TipoComponenteCMS)))
                        {
                            if (componentesDisponibles.Contains(tipoComp))
                            {
                                //se mapea el tipo de componente para la vista
                                CMSComponentType tipoCMScomp = (CMSComponentType)((int)tipoComp);
                                mPaginaModel.AvailableComponentsList.Add(tipoCMScomp, UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_" + tipoComp));
                            }
                        }
                    //}

                    #endregion

                    #region Caja de búsqueda  y filtros

                    mPaginaModel.FilterList = new List<FacetItemModel>();

                    //Busqueda
                    if (!string.IsNullOrEmpty(Busqueda))
                    {
                        FacetItemModel facetaItem = new FacetItemModel();
                        facetaItem.Name = Busqueda;
                        facetaItem.Filter = "search=" + Busqueda;
                        mPaginaModel.FilterList.Add(facetaItem);
                    }
                    //Tipo Componente
                    if (TipoComponente > -1)
                    {
                        FacetItemModel facetaItem = new FacetItemModel();
                        //facetaItem.Name = ((TipoComponenteCMS)TipoComponente).ToString(); TipoComponenteToString
                        facetaItem.Name = TipoComponenteToString((TipoComponenteCMS)TipoComponente);
                        facetaItem.Filter = "tipoComponente=" + TipoComponente;
                        mPaginaModel.FilterList.Add(facetaItem);
                    }
                    //NumUsos
                    if (NumUsos > -1)
                    {
                        FacetItemModel facetaItem = new FacetItemModel();
                        facetaItem.Name = NumUsos.ToString();
                        facetaItem.Filter = "numusos=" + NumUsos;
                        mPaginaModel.FilterList.Add(facetaItem);
                    }

                    #endregion

                    mPaginaModel.Result.NumeroPaginaActual = NumPagina;
                    mPaginaModel.Result.NumeroResultadosPagina = 10;
                    mPaginaModel.Result.NumeroResultadosTotal = cont;
                }
                
                return mPaginaModel;
            }
            set { mPaginaModel = value; }
        }

        public string TipoComponenteToString(TipoComponenteCMS pTipoComponenteCMS)
        {
            
            switch (pTipoComponenteCMS)
            {
                case TipoComponenteCMS.HTML:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_HTML");
                case TipoComponenteCMS.Destacado:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_DESTACADO");
                case TipoComponenteCMS.ListadoDinamico:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_LISTADODINAMICO");
                case TipoComponenteCMS.ListadoEstatico:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_LISTADOESTATICO");
                case TipoComponenteCMS.ActividadReciente:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_ACTIVIDADRECIENTE");
                case TipoComponenteCMS.GrupoComponentes:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_GRUPOCOMPONENTES");
                case TipoComponenteCMS.Tesauro:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_TESAURO");
                case TipoComponenteCMS.DatosComunidad:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_DATOSCOMUNIDAD");
                case TipoComponenteCMS.UsuariosRecomendados:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_USUARIOSRECOMENDADOS");
                case TipoComponenteCMS.CajaBuscador:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_CAJABUSCADOR");
                case TipoComponenteCMS.Faceta:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_FACETA");
                case TipoComponenteCMS.ListadoUsuarios:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_LISTADOUSUARIOS");
                case TipoComponenteCMS.ListadoProyectos:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_LISTADOPROYECTOS");
                case TipoComponenteCMS.ResumenPerfil:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_RESUMENPERFIL");
                case TipoComponenteCMS.MasVistos:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_MASVISTOS");
                case TipoComponenteCMS.EnvioCorreo:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_ENVIOCORREO");
                case TipoComponenteCMS.PreguntaTIC:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_PREGUNTATIC");
                case TipoComponenteCMS.Menu:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_MENU");
                case TipoComponenteCMS.Buscador:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_BUSCADOR");
                case TipoComponenteCMS.BuscadorSPARQL:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_BUSCADORSPARQL");
                case TipoComponenteCMS.UltimosRecursosVisitados:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_ULTIMOSRECURSOSVISITADOS");
                case TipoComponenteCMS.ConsultaSPARQL:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_CONSULTASPARQL");
                case TipoComponenteCMS.ConsultaSQLSERVER:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_CONSULTASQLSERVER");
                case TipoComponenteCMS.ListadoPorParametros:
                    return UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_LISTADOPORPARAMETROS");
            }
            return pTipoComponenteCMS.ToString();
        }

        private int TipoComponente { get; set; }
        private Guid BloqueID { get; set; }
        private int NumPagina { get; set; }
        private string Busqueda { get; set; }
        private int NumUsos { get; set; }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// A partir de la versi�n 5.6.0 es necesario que los CMSComponentes tenga FechaActualizacion, este campo antes no era necesario as� que actualizamos los componentes antiguos
        /// para que tambi�n tengan, si no fallar�n las instrucciones siguientes.
        /// </summary>
        private void CorregirFechaActualizacionComponentesAntiguos()
        {
            if (mEntityContext.CMSComponente.Any(item => !item.FechaUltimaActualizacion.HasValue))
            {
                List<AD.EntityModel.Models.CMS.CMSComponente> listaComponentesSinFecha = mEntityContext.CMSComponente.Where(item => !item.FechaUltimaActualizacion.HasValue).ToList();
                foreach (AD.EntityModel.Models.CMS.CMSComponente cmsComponenteSinFecha in listaComponentesSinFecha)
                {
                    cmsComponenteSinFecha.FechaUltimaActualizacion = DateTime.Now;
                }

                mEntityContext.SaveChanges();
            }
        }

		private void CargarPermisosCMSViewBag()
		{
			UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
			ViewBag.VerComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.VerComponenteCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.CrearComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.CrearComponenteCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.EliminarComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarComponenteCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.ModificarComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EditarComponenteCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.RestaurarVersionComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.RestaurarVersionCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.EliminarVersionComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarVersionCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
		}

		private void RecogerParametros(string tipoComponente, string idBloqueContenedor, string pagina, string search, string numusos)
        {
            if (!string.IsNullOrEmpty(pagina))
            {
                int pag = int.Parse(pagina);
                if (pag > 0)
                {
                    NumPagina = pag;
                }
            }
            if (!string.IsNullOrEmpty(tipoComponente))
            {
                int tipo = int.Parse(tipoComponente);
                if (Enum.IsDefined(typeof(TipoComponenteCMS), tipo))
                {
                    TipoComponente = tipo;
                }
            }
            if (!string.IsNullOrEmpty(idBloqueContenedor))
            {
                Guid bloqueID = Guid.Empty;
                if (Guid.TryParse(idBloqueContenedor, out bloqueID) && !bloqueID.Equals(Guid.Empty))
                {
                    BloqueID = bloqueID;
                }
            }
            if (!string.IsNullOrEmpty(search))
            {
                Busqueda = search;
            }
            if (!string.IsNullOrEmpty(numusos))
            {
                int usos = int.Parse(numusos);
                if (usos >= 0)
                {
                    NumUsos = usos;
                }
            }
        }

        #endregion
    }
} 