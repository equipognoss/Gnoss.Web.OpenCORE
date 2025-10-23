using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.Seguridad;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Gnoss.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using VDS.RDF.Writing;
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.TabModel;
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.TabModel.SearchTabModel;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// ViewModel de la página de administrar pestañas
    /// </summary>
    [Serializable]
    public class AdministrarPestanyasViewModel
    {
        /// <summary>
        /// Lista de idiomas de la plataforma
        /// </summary>
        public Dictionary<string, string> ListaIdiomas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ContenidoMultiIdioma { get; set; }
        /// <summary>
        /// Idioma por defecto de la comunidad
        /// </summary>
        public string IdiomaPorDefecto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool CMSDisponible { get; set; }
        /// <summary>
        /// Lista de pestañas de la comunidad
        /// </summary>
        public List<TabModel> ListaPestanyas { get; set; }
        public List<string> ListaPestanyasOntologia { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarPestanyasController : ControllerAdministrationWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarPestanyasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices pAvailableService, ILogger<AdministrarPestanyasController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, pAvailableService, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarPestanyasViewModel mPaginaModel = null;

        /// <summary>
        /// 
        /// </summary>
        private GestionProyecto mGestionProyectos = null;
        #endregion

        #region Metodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.VerPagina, (ulong)PermisoContenidos.CrearPagina, (ulong)PermisoContenidos.EditarPagina, (ulong)PermisoContenidos.EliminarPagina, (ulong)PermisoContenidos.PublicarPagina } })]
        public ActionResult Index()
        {

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            CargarPermisosAdministrarPaginas();

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "estructura edicion edicionPaginas listado no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_Paginas;

            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTRUCTURA");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONPAGINAS", "PAGINAS");
            ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorFacetas>(), mLoggerFactory);
            List<FacetaModel> listaFacetas = conFacetas.CargarListadoFacetas();
            ViewBag.ListadoFacetasComunidad = listaFacetas;

            return View(PaginaModel);
        }

        /// <summary>
        /// Nueva Pestaña
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.CrearPagina } })]
        public ActionResult NuevaPestanya(short TipoPestanya, string nameonto)
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            CargarPermisosAdministrarPaginas();

            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory);

            bool existePestanyaDelMismoTipo = contrPest.ExistePestanyaDelMismoTipo(TipoPestanya);

            TipoPestanyaMenu tipoPestMenu = (TipoPestanyaMenu)TipoPestanya;

            if (existePestanyaDelMismoTipo && (tipoPestMenu == TipoPestanyaMenu.Home || tipoPestMenu == TipoPestanyaMenu.Indice || tipoPestMenu == TipoPestanyaMenu.Recursos || tipoPestMenu == TipoPestanyaMenu.Preguntas || tipoPestMenu == TipoPestanyaMenu.Debates || tipoPestMenu == TipoPestanyaMenu.Encuestas || tipoPestMenu == TipoPestanyaMenu.PersonasYOrganizaciones || tipoPestMenu == TipoPestanyaMenu.AcercaDe || tipoPestMenu == TipoPestanyaMenu.BusquedaAvanzada))
            {
                return GnossResultERROR(UtilIdiomas.GetText("COMADMINPESTANYAS", "ERRORTIPOREPETIDO"));
            }

            TabModel pestanya = new TabModel();
            pestanya.Active = true;
            pestanya.Nueva = true;
            pestanya.Key = Guid.NewGuid();
            pestanya.Type = tipoPestMenu;
            pestanya.Visible = true;
            pestanya.Privacidad = 0;
            pestanya.ParentTabKey = Guid.Empty;
            pestanya.ListaFacetas = new List<TabModel.FacetasTabModel>();
            // Idioma por defecto de la comunidad para la página            
            pestanya.IdiomaPorDefecto = IdiomaPorDefecto;

            pestanya.ClassCSSBody = "";

            if (tipoPestMenu.Equals(TipoPestanyaMenu.Home))
            {
                pestanya.HomeCMS = new TabModel.HomeCMTabSModel();
                pestanya.HomeCMS.HomeTodosUsuarios = false;
                pestanya.HomeCMS.HomeMiembros = false;
                pestanya.HomeCMS.HomeNoMiembros = false;
            }

            if (tipoPestMenu == TipoPestanyaMenu.CMS || tipoPestMenu == TipoPestanyaMenu.BusquedaSemantica || tipoPestMenu == TipoPestanyaMenu.EnlaceInterno || tipoPestMenu == TipoPestanyaMenu.EnlaceExterno || tipoPestMenu == TipoPestanyaMenu.Dashboard)
            {
                //A las pestañas de cms, busqueda semantica o enlaces, debemos ponerles un nombre por defecto
                if (string.IsNullOrEmpty(nameonto))
                {
                    pestanya.Name = tipoPestMenu.ToString();
                }
                else
                {
                    pestanya.Name = $"{char.ToUpper(nameonto[0])}{nameonto.Substring(1)}";
                    pestanya.Url = nameonto;
                }
                pestanya.EsNombrePorDefecto = false;
                pestanya.EsUrlPorDefecto = false;
            }
            else
            {
                string name, url;
                ObtenerNameUrlMultiIdiomaPestanyaPorTipo((TipoPestanyaMenu)TipoPestanya, out name, out url);
                pestanya.Name = name;
                pestanya.Url = url;

                pestanya.EsNombrePorDefecto = true;
                pestanya.EsUrlPorDefecto = true;
            }

            bool esPestanyaBusqueda =
                TipoPestanya == (short)TipoPestanyaMenu.Recursos ||
                TipoPestanya == (short)TipoPestanyaMenu.Preguntas ||
                TipoPestanya == (short)TipoPestanyaMenu.Debates ||
                TipoPestanya == (short)TipoPestanyaMenu.Encuestas ||
                TipoPestanya == (short)TipoPestanyaMenu.PersonasYOrganizaciones ||
                TipoPestanya == (short)TipoPestanyaMenu.BusquedaSemantica ||
                TipoPestanya == (short)TipoPestanyaMenu.BusquedaAvanzada;

            if (esPestanyaBusqueda)
            {
                pestanya.OpcionesBusqueda = contrPest.CargarOpcionesBusquedaPorDefecto((TipoPestanyaMenu)TipoPestanya, nameonto);
                pestanya.ListaExportaciones = new List<TabModel.ExportacionSearchTabModel>();
                pestanya.OpcionesDashboard = new List<TabModel.DashboardTabModel>();
                pestanya.OpcionesBusqueda.ListaSearchPersonalizado = new List<SearchPersonalizadoTabModel>();
            }
            //TFG Fran
            bool esPestanyaDasboard = TipoPestanya == (short)TipoPestanyaMenu.Dashboard;
            if (esPestanyaDasboard)
            {
                pestanya.OpcionesDashboard = new List<TabModel.DashboardTabModel>();
            }

            ViewBag.ContenidoMultiIdioma = (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma));
            pestanya.ListaIdiomasDisponibles = new List<string>();
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

            if (ParametrosGeneralesRow.IdiomasDisponibles)
            {
                ViewBag.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
            }
            else
            {
                ViewBag.ListaIdiomas = new Dictionary<string, string>();
                ViewBag.ListaIdiomas.Add(IdiomaPorDefecto, paramCL.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
            }

            return PartialView("_NuevaPestanya", pestanya);
        }

        /// <summary>
        /// Nueva Exportacion
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaExportacion()
        {
            EliminarPersonalizacionVistas();

            TabModel.ExportacionSearchTabModel exportacion = new TabModel.ExportacionSearchTabModel();
            exportacion.Key = Guid.NewGuid();
            exportacion.Nombre = UtilIdiomas.GetText("COMADMINPESTANYAS", "NUEVAEXPORTACION");
            exportacion.GruposPermiso = new Dictionary<Guid, string>();
            exportacion.ListaPropiedades = new List<TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel>();

            return PartialView("_FichaExportacion", exportacion);
        }

        /// <summary>
        /// Agregar Faceta
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult AgregarFaceta()
        {
            EliminarPersonalizacionVistas();
            ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorFacetas>(), mLoggerFactory);
            List<FacetaModel> listaFacetas = conFacetas.CargarListadoFacetas();
            AdministrarFacetasEnPestanyasViewModel model = new AdministrarFacetasEnPestanyasViewModel();


            model.ListadoFacetas = listaFacetas;
            model.FacetasPestanya = new TabModel.FacetasTabModel();
            return PartialView("_SeleccionFaceta", model);
        }
        /// <summary>
        /// Cargar Faceta
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult CargarFacetas(Guid pPestanyaID)
        {
            EliminarPersonalizacionVistas();
            ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorFacetas>(), mLoggerFactory);
            List<FacetaModel> listaFacetas = conFacetas.CargarListadoFacetas();
            AdministrarFacetasPestanyasModel model = new AdministrarFacetasPestanyasModel();
            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory, false);
            model.ListadoFacetas = listaFacetas;

            List<FacetaObjetoConocimientoProyectoPestanya> facetasPestanya = contrPest.ObtenerFacetaObjetoConocimientoProyectoPestanya(pPestanyaID);
            List<TabModel.FacetasTabModel> lst = new List<TabModel.FacetasTabModel>();
            foreach (FacetaObjetoConocimientoProyectoPestanya item in facetasPestanya)
            {
                TabModel.FacetasTabModel fcTM = new TabModel.FacetasTabModel();
                fcTM.ClavePestanya = item.PestanyaID;
                fcTM.Faceta = item.Faceta;
                fcTM.ObjetoConocimiento = item.ObjetoConocimiento;
                lst.Add(fcTM);
            }
            model.FacetasPestanyas = lst;
            return PartialView("_VerFacetas", model);
        }

        /// <summary>
        /// Nueva Propiedad Exportacion
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaPropiedadExportacion()
        {
            EliminarPersonalizacionVistas();

            TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel propiedadExportacion = new TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel();
            propiedadExportacion.Nombre = UtilIdiomas.GetText("COMADMINPESTANYAS", "NUEVAPROPIEDAD");
            propiedadExportacion.Ontologia = null;
            propiedadExportacion.Propiedad = "";
            propiedadExportacion.DatoExtraPropiedad = "";

            CargarListaOntologias();

            return PartialView("_FichaExportacionPropiedad", propiedadExportacion);
        }

        /// <summary>
        /// Nuevo filtro orden
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevoFiltroOrden()
        {
            EliminarPersonalizacionVistas();

            TabModel.SearchTabModel.FiltroOrden filtroOrden = new TabModel.SearchTabModel.FiltroOrden();
            filtroOrden.Nombre = UtilIdiomas.GetText("COMADMINPESTANYAS", "NUEVOFILTRO");
            filtroOrden.Filtro = "";
            filtroOrden.Consulta = "";
            filtroOrden.OrderBy = "";

            return PartialView("_FichaFiltroOrden", filtroOrden);
        }

        /// <summary>
        /// Nuevo Asistente
        /// TFG FRAN
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevoAsistente()
        {
            EliminarPersonalizacionVistas();

            TabModel.DashboardTabModel asistente = new TabModel.DashboardTabModel();
            asistente.Nombre = "Nuevo gráfico";
            asistente.OpcionesDatasets = new List<TabModel.DashboardTabModel.DatasetTabModel>();
            asistente.AsisID = Guid.NewGuid();

            return PartialView("_FichaAsistente", asistente);
        }
        /// <summary>
        /// Nuevo Dataset
        /// TFG FRAN
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevoDataset()
        {
            EliminarPersonalizacionVistas();

            TabModel.DashboardTabModel.DatasetTabModel dataset = new TabModel.DashboardTabModel.DatasetTabModel();
            dataset.Nombre = "Nuevo Dataset";
            dataset.DatasetID = Guid.NewGuid();

            return PartialView("_FichaDataset", dataset);
        }

        [HttpPost]
        public ActionResult NuevaPropiedad()
        {
            EliminarPersonalizacionVistas();

            TabModel.DashboardTabModel.DatasetTabModel dataset = new TabModel.DashboardTabModel.DatasetTabModel();
            dataset.Nombre = "Nuevo Dataset";
            dataset.DatasetID = Guid.NewGuid();

            return PartialView("_FichaDatasetNoAgrupacion", dataset);
        }

        [HttpPost]
        public string CargarResultados(string select, string where, string groupby, string orderby, string limit, string idChart, string tipoContexto)
        {

            CargadorResultados cargadorResultados = new CargadorResultados();
            cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();

            Guid pProyectoID = ProyectoSeleccionado.Clave;
            Guid pIdentidadID = mControladorBase.UsuarioActual.IdentidadID;
            string pUrlPaginaBusqueda = this.Request.GetDisplayUrl();
            bool pUsarMasterParaLectura = mControladorBase.UsuarioActual.UsarMasterParaLectura;
            bool pIdentidadInvitada = !mControladorBase.UsuarioActual.EsIdentidadInvitada;
            bool pAdminQuiereVerTodasPersonas = AdministradorQuiereVerTodasLasPersonas;
            TipoBusqueda pTipoBusqueda = VariableTipoBusqueda;
            string pGrafo = ProyectoSeleccionado.Clave.ToString();

            string argumentos = "";
            string pArgumentos = argumentos.Replace("\"", "%2522");
            bool pEsPrimeraCarga = string.IsNullOrEmpty(argumentos);
            string pLanguageCode = UtilIdiomas.LanguageCode;
            Guid pTokenAfinidad = Guid.NewGuid();


            //parametros
            string pParametroAdicional = "";
            pParametroAdicional = pParametroAdicional + "busquedaTipoChart=" + idChart + "&busquedaTipoDashboardSelect=" + select;
            string whereCompleto = "";
            whereCompleto = whereCompleto + where;
            if (!string.IsNullOrEmpty(groupby))
            {
                whereCompleto = whereCompleto + "))) " + groupby;
            }
            else
            {
                whereCompleto = whereCompleto + ")))";
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                whereCompleto = whereCompleto + ")))" + orderby;
            }
            else
            {
                whereCompleto = whereCompleto + ")))";
            }
            if (!string.IsNullOrEmpty(limit) && int.Parse(limit) < 20)
            {
                whereCompleto = whereCompleto + ")))" + limit;
            }
            else
            {
                whereCompleto = whereCompleto + ")))20";
            }
            pParametroAdicional = pParametroAdicional + "&busquedaTipoDashboardWhere=" + whereCompleto;
            tipoContexto = tipoContexto.Replace("\'", "");
            pParametroAdicional = pParametroAdicional + "&" + tipoContexto;
            pParametroAdicional = pParametroAdicional.Replace("=", "%3D");
            pParametroAdicional = pParametroAdicional.Replace("&", "%7C");
            pParametroAdicional = pParametroAdicional.Replace(":", "%3A");
            pParametroAdicional = pParametroAdicional.Replace(")", "%29");


            string Resultados = cargadorResultados.CargarResultados(pProyectoID, pIdentidadID, pIdentidadInvitada, pUrlPaginaBusqueda, pUsarMasterParaLectura, pAdminQuiereVerTodasPersonas, pTipoBusqueda, pGrafo, pParametroAdicional, pArgumentos, pEsPrimeraCarga, pLanguageCode, -1, "", pTokenAfinidad, Request);
            return Resultados;
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.CrearPagina, (ulong)PermisoContenidos.EditarPagina, (ulong)PermisoContenidos.EliminarPagina } })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Guardar(List<TabModel> ListaPestanyas)
        {
            GuardarLogAuditoria();
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
            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory, iniciado);
            string errores = contrPest.ComprobarErrores(ListaPestanyas);
            if (string.IsNullOrEmpty(errores))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = false;
                try
                {
                    List<TabModel> paginaModelPestanyas = PaginaModel.ListaPestanyas;
                    List<TabModel> pestanyasEliminadasOrdenadas = new List<TabModel>();
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    foreach (TabModel pestanya in ListaPestanyas)
                    {
                        short ordenViejo = mEntityContext.ProyectoPestanyaMenu.Where(x => x.PestanyaID.Equals(pestanya.Key)).Select(x => x.Orden).FirstOrDefault();
                        if (pestanya.Deleted || pestanya.Order != ordenViejo)
                        {
                            pestanyasEliminadasOrdenadas.Add(pestanya);
                        }
                    }

                    contrPest.GuardarPestanyas(pestanyasEliminadasOrdenadas);
                    if (iniciado)
                    {
                        ListaPestanyas = ModificarOrdenPestanyas(paginaModelPestanyas, ListaPestanyas);

                        HttpResponseMessage resultado = InformarCambioAdministracion("Pestanyas", JsonConvert.SerializeObject(ListaPestanyas, Formatting.Indented));
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
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    }
                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(false);
                    }
                    return GnossResultERROR(e.Message);
                }
                catch (Exception ex)
                {
                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(false);
                        //proyAD.TerminarTransaccion(false);
                    }
                }

                contrPest.CrearFilasPropiedadesIntegracionContinua(ListaPestanyas.Where(pestanya => pestanya.Deleted || pestanya.Modified).ToList());

                if (EntornoActualEsPruebas && iniciado)
                {
                    //con esto funciona para PRE
                    //contrPest.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaPestanyas.ToList(), UrlApiDesplieguesEntornoSiguiente);

                    contrPest.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaPestanyas.ToList(), UrlApiEntornoSeleccionado("pre"), UsuarioActual.UsuarioID);
                    contrPest.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaPestanyas.ToList(), UrlApiEntornoSeleccionado("pro"), UsuarioActual.UsuarioID);
                }

                contrPest.InvalidarCaches(UrlIntragnoss);
                //RouteValueTransformer.RecalcularRutasProyecto(ProyectoSeleccionado.FilaProyecto.NombreCorto, mConfigService, mScopedFactory);
                return GnossResultOK();
            }
            else
            {
                return GnossResultERROR(errores);
            }
        }
        /// <summary>
        /// Guardar una pestanya
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.CrearPagina, (ulong)PermisoContenidos.EditarPagina } })]
        public ActionResult GuardarPestanya(List<TabModel> ListaPestanyas)
        {
            TabModel Pestanya = ListaPestanyas[0];
            GuardarLogAuditoria();
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
            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory, iniciado);
            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
            bool transaccionIniciada = false;
            try
            {
                List<TabModel> paginaModelPestanyas = PaginaModel.ListaPestanyas;
                mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);
                contrPest.GuardarPestanya(Pestanya);
                if (iniciado)
                {

                    HttpResponseMessage resultado = InformarCambioAdministracion("Pestanyas", JsonConvert.SerializeObject(Pestanya, Formatting.Indented));
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
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                }
                if (transaccionIniciada)
                {
                    mEntityContext.TerminarTransaccionesPendientes(false);
                }
                return GnossResultERROR(e.Message);
            }
            catch (Exception ex)
            {
                if (transaccionIniciada)
                {
                    mEntityContext.TerminarTransaccionesPendientes(false);
                    //proyAD.TerminarTransaccion(false);
                }
            }

            contrPest.CrearFilasPropiedadesIntegracionContinuaPestanya(Pestanya);

            if (EntornoActualEsPruebas && iniciado)
            {
                //con esto funciona para PRE
                //contrPest.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaPestanyas.ToList(), UrlApiDesplieguesEntornoSiguiente);

                contrPest.ModificarFilasIntegracionContinuaEntornoSiguientePestanya(Pestanya, UrlApiEntornoSeleccionado("pre"), UsuarioActual.UsuarioID);
                contrPest.ModificarFilasIntegracionContinuaEntornoSiguientePestanya(Pestanya, UrlApiEntornoSeleccionado("pro"), UsuarioActual.UsuarioID);
            }

            contrPest.InvalidarCaches(UrlIntragnoss);
            //RouteValueTransformer.RecalcularRutasProyecto(ProyectoSeleccionado.FilaProyecto.NombreCorto, mConfigService, mScopedFactory);
            return GnossResultOK();
        }

        [HttpPost]
        public ActionResult CerrarModalSinGuardar(TabModel model)
        {
            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            bool tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EditarPagina, IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);

            if (tienePermiso)
            {
                PartialView("_modal-views/_CloseModal", model);
            }

            return GnossResultOK();
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.RestaurarVersionPagina, (ulong)PermisoContenidos.VerPagina } })]
        public ActionResult CompararVersionConfigPestanya(string documentosComparar, bool pRestaurar = false)
        {
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

            if (ParametrosGeneralesRow.IdiomasDisponibles)
            {
                ViewBag.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
            }
            else
            {
                ViewBag.ListaIdiomas = new Dictionary<string, string>();
                ViewBag.ListaIdiomas.Add(IdiomaPorDefecto, paramCL.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
            }

            ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
            string[] param = documentosComparar.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            ViewBag.Restaurando = pRestaurar;
            ViewBag.PermisosPaginas = PermisosPaginasAdministracion;

            ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorFacetas>(), mLoggerFactory);
            ViewBag.ListadoFacetasComunidad = conFacetas.CargarListadoFacetas();

            List<TabModel> listaPestanyasAComparar = CargarComparadorConfigPestanyas(Guid.Parse(param[0]), Guid.Parse(param[1]));

            return PartialView("_comparator-views/_comparator", listaPestanyasAComparar);
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.RestaurarVersionPagina } })]
        public ActionResult RestaurarVersion(Guid pVersionID, string pComentario = null)
        {
            bool iniciado = false;

            try
            {
                iniciado = HayIntegracionContinua;

                ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory, iniciado);

                TabModel pestanyaRestaurada = contrPest.RestaurarPagina(pVersionID, pComentario ?? "");
                if (iniciado)
                {
                    HttpResponseMessage resultado = InformarCambioAdministracion("Pestanyas", JsonConvert.SerializeObject(pestanyaRestaurada, Formatting.Indented));
                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }

                return GnossResultOK(UtilIdiomas.GetText("DEVTOOLS", "RESTAURARVERSIONCONFIGPAGINA"));
            }
            catch (Exception ex)
            {
                mlogger.LogError($"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}");
                return GnossResultERROR(UtilIdiomas.GetText("DEVTOOLS", "ERRORRESTAURARVERSIONCONFIGPAGINA"));
            }
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.EliminarVersionPagina } })]
        public ActionResult EliminarVersion(Guid pPestanyaID, Guid pVersionID)
        {
            try
            {
                ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory, HayIntegracionContinua);

                contrPest.EliminarVersionPagina(pPestanyaID, pVersionID);

                return GnossResultOK(UtilIdiomas.GetText("DEVTOOLS", "VERSIONCONFIGPAGINAELIMINADO"));
            }
            catch (Exception ex)
            {
                mlogger.LogError($"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}");
                return GnossResultERROR(UtilIdiomas.GetText("DEVTOOLS", "ERRORELIMINARVERSIONCONFIGPAGINA"));
            }
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.VerPagina } })]
        public ActionResult CargarHistorial(Guid pPestanyaID)
        {
            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            ViewBag.RestaurarVersionPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.RestaurarVersionPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            ViewBag.EliminarVersionPaginasPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarVersionPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);

            List<AdministrarPaginaVersionViewModel> modelo = CargarVersionesPestanya(pPestanyaID);

            return PartialView("_modal-views/_history", modelo);
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.RestaurarVersionPagina, (ulong)PermisoContenidos.EditarPagina } })]
        public ActionResult AddCommentVersion(Guid pVersionID)
        {
            try
            {
                return PartialView("_partial-views/_add-comment-restore", pVersionID);
            }
            catch (Exception ex)
            {
                mlogger.LogError($"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}");
                return GnossResultERROR(UtilIdiomas.GetText("DEVTOOLS", "ERRORCARGAFORMULARIO"));
            }
        }

        private List<TabModel> ModificarOrdenPestanyas(List<TabModel> pPaginaModelPestanyas, List<TabModel> pListaPestanyas)
        {
            List<TabModel> listaPestanyasDevolver = new List<TabModel>();
            foreach (TabModel pestanya in pListaPestanyas)
            {
                TabModel pestanyaNueva = pestanya;
                if (!pestanya.Deleted && !pestanya.Modified)
                {
                    pestanyaNueva = pPaginaModelPestanyas.Where(pesta => pesta.Key.Equals(pestanya.Key)).FirstOrDefault();
                    if (pestanyaNueva != null)
                    {
                        if (pestanyaNueva.OpcionesBusqueda != null)
                        {
                            pestanyaNueva.OpcionesBusqueda.ValoresPorDefecto = true;
                        }
                        //Solo modificar las que se han cambiado de orden
                        if (pestanyaNueva.Order != pestanya.Order)
                        {
                            pestanyaNueva.Order = pestanya.Order;
                            pestanyaNueva.Modified = true;
                        }
                    }
                    else
                    {
                        pestanyaNueva = pestanya;
                    }
                }

                if (pestanyaNueva.ClassCSSBody == null)
                {
                    pestanyaNueva.ClassCSSBody = "";
                }
                listaPestanyasDevolver.Add(pestanyaNueva);
            }
            return listaPestanyasDevolver;
        }

        /// <summary>
        /// Carga la previsualización de las facetas y resultados de una página de búsqueda.
        /// </summary>
        /// <param name="pPaginaId">Identificador de la página de búsqueda.</param>
        /// <returns>Vista parcial con todas las facetas que afectan a la página de búsqueda, la consulta que realiza el servicio de facetado para obtenerlas y la consulta que realiza el servicio de resultados para conseguir la primera página de resultados.</returns>
        [HttpPost]
        public IActionResult CargarPrevisualizacionPestanya([FromForm] string pPaginaId)
        {
            ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorFacetas>(), mLoggerFactory);
            List<FacetaModel> listaFacetas = conFacetas.CargarListadoFacetas();
            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory);

            //Obtenemos la página seleccionada
            TabModel pestanya = contrPest.CargarPestanya(GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where((p) => p.PestanyaID == new Guid(pPaginaId)).FirstOrDefault());

            pestanya.ConsultasDeFacetas = new Dictionary<string, string>();
            if (pestanya.OpcionesBusqueda != null)
            {
                //Obtenemos la lista de los filtros aplicados a la página
                string[] listaFiltros = pestanya.OpcionesBusqueda.CampoFiltro.Split("|", StringSplitOptions.RemoveEmptyEntries);
                List<string> objetosConocimientoAplicables = new List<string>();

                //Obtenemos los objetos de conocimiento aplicados a la página a partir de los filtros
                foreach (string filtro in listaFiltros)
                {
                    if (filtro.Contains("rdf:type"))
                    {
                        objetosConocimientoAplicables.Add(filtro.Replace("rdf:type=", string.Empty));
                    }
                }

                //Se construyen los parámetros adicionales para la petición que obtiene la consulta a travñes de los servicios de facetas y resultados
                string parametrosAdicionales = $"PestanyaActualID={pestanya.Key}|";
                if (pestanya.Type != TipoPestanyaMenu.Recursos)
                {
                    parametrosAdicionales += $"{pestanya.OpcionesBusqueda.CampoFiltro}|";
                }

                foreach (FiltroOrden filtroOrden in pestanya.OpcionesBusqueda.FiltrosOrden)
                {
                    parametrosAdicionales += $"ordenarPor={filtroOrden.Filtro}|";
                }

                CargadorFacetas cargadorFacetas = new CargadorFacetas();
                cargadorFacetas.Url = mConfigService.ObtenerUrlServicioFacetas();

                // Por cada objeto de conocimiento asociado a la página se obtienen las facetas que le afectan
                foreach (string objetoConocimiento in objetosConocimientoAplicables)
                {
                    List<FacetaModel> facetasAplicables = listaFacetas.Where(fac => fac.ObjetosConocimiento.Contains(objetoConocimiento)).ToList();

                    // Por cada faceta, se obtiene su consulta
                    foreach (FacetaModel faceta in facetasAplicables)
                    {
                        string claveNombreFaceta = $"{UtilCadenas.ObtenerTextoDeIdioma(faceta.Name, IdiomaUsuario, IdiomaPorDefecto)} - {faceta.ClaveFaceta}";

                        if (!pestanya.ConsultasDeFacetas.ContainsKey(claveNombreFaceta))
                        {
                            try
                            {
                                string queryFaceta = cargadorFacetas.ObtenerConsulta(ProyectoSeleccionado.Clave, !mControladorBase.UsuarioActual.EsIdentidadInvitada, mControladorBase.UsuarioActual.EsUsuarioInvitado, mControladorBase.UsuarioActual.IdentidadID, "", "Meta", UtilIdiomas.LanguageCode, AdministradorQuiereVerTodasLasPersonas, VariableTipoBusqueda, -1, !string.IsNullOrEmpty(faceta.ClaveFacetaYReprocidad) ? faceta.ClaveFacetaYReprocidad : faceta.ClaveFaceta, ProyectoSeleccionado.Clave.ToString(), $"{parametrosAdicionales}NumElementosFaceta=10000|", string.Empty, null, mControladorBase.UsuarioActual.UsarMasterParaLectura, null, null, null, Request);
                                if (!string.IsNullOrEmpty(queryFaceta))
                                {
                                    //Decodificar la consulta
                                    queryFaceta = System.Text.RegularExpressions.Regex.Unescape(queryFaceta.Remove(queryFaceta.Length - 2).Remove(0, 2));
                                    pestanya.ConsultasDeFacetas.Add(claveNombreFaceta, queryFaceta);
                                }
                                else
                                {
                                    pestanya.ConsultasDeFacetas.Add(claveNombreFaceta, "Error al recuperar la consulta");
                                }
                            }
                            catch
                            {
                                pestanya.ConsultasDeFacetas.Add(claveNombreFaceta, "Error al recuperar la consulta");
                            }
                        }
                    }
                }

                try
                {
                    CargadorResultados cargadorResultados = new CargadorResultados(); // Se obtiene la consulta realizada para obtener la primera página de resultados
                    cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();
                    string queryResultados = cargadorResultados.ObtenerConsulta(ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.IdentidadID, mControladorBase.UsuarioActual.EsUsuarioInvitado, pestanya.Url, mControladorBase.UsuarioActual.UsarMasterParaLectura, AdministradorQuiereVerTodasLasPersonas, TipoBusqueda.Recursos, ProyectoSeleccionado.Clave.ToString(), parametrosAdicionales, "|pagina=1", false, UtilIdiomas.LanguageCode, 1, "", false, null, Request);
                    if (!string.IsNullOrEmpty(queryResultados))
                    {
                        queryResultados = System.Text.RegularExpressions.Regex.Unescape(queryResultados.Remove(queryResultados.Length - 1).Remove(0, 2)); // Decodificar la consulta
                        pestanya.ConsultaDeResultados = queryResultados;
                    }
                }
                catch
                {
                    //Si falla la petición no se carga la consulta de resultados, en la vista se pintará un mensaje indicando que no hay recursos que mostrar.
                }
            }

            return PartialView("_PrevisualizacionPaginas", pestanya);
        }


        #endregion

        #region Metodos        

        private void CargarPermisosAdministrarPaginas()
        {
            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            ViewBag.VerPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.VerPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            ViewBag.CrearPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.CrearPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            ViewBag.EliminarPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            ViewBag.ModificarPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EditarPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            ViewBag.PublicarPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.PublicarPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            ViewBag.RestaurarVersionPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.RestaurarVersionPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            ViewBag.EliminarVersionPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarVersionPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
        }


        private void CargarListaOntologias()
        {
            Dictionary<string, string> ListaOntologias = new Dictionary<string, string>();

            foreach (OntologiaProyecto filaOntologia in GestionProyectos.DataWrapperProyectos.ListaOntologiaProyecto)
            {
                ListaOntologias.Add(filaOntologia.OntologiaProyecto1, UtilCadenas.ObtenerTextoDeIdioma(filaOntologia.NombreOnt, UtilIdiomas.LanguageCode, IdiomaPorDefecto));
            }

            ViewBag.ListaOntologias = ListaOntologias;
        }

        private List<SearchPersonalizadoTabModel> CargarListaSearchPersonalizado(Guid pPestanyaID)
        {

            List<SearchPersonalizadoTabModel> listaSearch = new List<SearchPersonalizadoTabModel>();

            foreach (ProyectoSearchPersonalizado proyectoSearch in GestionProyectos.DataWrapperProyectos.ListaProyectoSearchPersonalizado)
            {
                bool seleccionada = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda.Any(pestanya => pestanya.PestanyaID.Equals(pPestanyaID) && pestanya.SearchPersonalizado != null && pestanya.SearchPersonalizado.Equals(proyectoSearch.NombreFiltro));
                SearchPersonalizadoTabModel searchPersonalizadoTabModel = new SearchPersonalizadoTabModel();
                searchPersonalizadoTabModel.SearchPersonalizado = proyectoSearch.NombreFiltro;
                searchPersonalizadoTabModel.EstaActivo = seleccionada;
                listaSearch.Add(searchPersonalizadoTabModel);
            }
            return listaSearch;

        }
        public Dictionary<string, Dictionary<string, bool>> CompararCamposMultiIdiomaPestanyas(TabModel pComparar1, TabModel pComparar2)
        {
            Dictionary<string, Dictionary<string, bool>> resultado = new Dictionary<string, Dictionary<string, bool>>();

            resultado.Add("title", ComparadorCamposMultiIdioma(pComparar1.Name, pComparar2.Name));
            resultado.Add("url", ComparadorCamposMultiIdioma(pComparar1.Url, pComparar2.Url));
            resultado.Add("metaDescription", ComparadorCamposMultiIdioma(pComparar1.MetaDescription, pComparar2.MetaDescription));
            resultado.Add("defaultText", ComparadorCamposMultiIdioma(pComparar1.OpcionesBusqueda.TextoDefectoBuscador, pComparar2.OpcionesBusqueda.TextoDefectoBuscador));
            return resultado;
        }
        public Dictionary<string, bool> ComparadorCamposMultiIdioma(string pValue1, string pValue2)
        {
            Dictionary<string, bool> resultado = new Dictionary<string, bool>();
            using (ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory))
            {
                Dictionary<string, bool> diccAux = new Dictionary<string, bool>();
                foreach (string idioma in paramCL.ObtenerListaIdiomas())
                {
                    string value1 = UtilCadenas.ObtenerTextoDeIdioma(pValue1, idioma, null) ?? "";
                    string value2 = UtilCadenas.ObtenerTextoDeIdioma(pValue2, idioma, null) ?? "";
                    resultado.Add(idioma, !value1.Equals(value2));
                }
            }

            return resultado;
        }
        public Dictionary<string, bool> CompararCamposPestanyas(TabModel pComparar1, TabModel pComparar2)
        {
            Dictionary<string, bool> resultado = new Dictionary<string, bool>();
            resultado.Add("type", !pComparar1.Type.Equals(pComparar2.Type));
            resultado.Add("openNewWindows", pComparar1.OpenInNewWindow != pComparar2.OpenInNewWindow);
            resultado.Add("classCssBody", !pComparar1.ClassCSSBody.Equals(pComparar2.ClassCSSBody));
            resultado.Add("visible", pComparar1.Visible != pComparar2.Visible);
            resultado.Add("active", pComparar1.Active != pComparar2.Active);
            resultado.Add("privacy", pComparar1.Privacidad != pComparar2.Privacidad);
            resultado.Add("visibleUserUnauthorized", pComparar1.VisibleSinAcceso != pComparar2.VisibleSinAcceso);
            resultado.Add("alternativeHtml", !pComparar1.HtmlAlternativoPrivacidad.Equals(pComparar2.HtmlAlternativoPrivacidad));
            resultado.Add("privacyProfiles", !pComparar1.PrivacidadPerfiles.Keys.All(pComparar2.PrivacidadPerfiles.Keys.Contains));
            resultado.Add("privacyGroups", !pComparar1.PrivacidadGrupos.Keys.All(pComparar2.PrivacidadGrupos.Keys.Contains));
            resultado.Add("activeHome", (pComparar1.HomeCMS != null && (pComparar1.HomeCMS.HomeTodosUsuarios || pComparar1.HomeCMS.HomeMiembros || pComparar1.HomeCMS.HomeNoMiembros)) != (pComparar2.HomeCMS != null && (pComparar2.HomeCMS.HomeTodosUsuarios || pComparar2.HomeCMS.HomeMiembros || pComparar2.HomeCMS.HomeNoMiembros)));
            resultado.Add("filters", !pComparar1.OpcionesBusqueda.CampoFiltro.Equals(pComparar2.OpcionesBusqueda.CampoFiltro));
            resultado.Add("filterOrder", pComparar1.OpcionesBusqueda.FiltrosOrden.Count != pComparar2.OpcionesBusqueda.FiltrosOrden.Count);
            resultado.Add("customSearch", !pComparar1.OpcionesBusqueda.ListaSearchPersonalizado.Select(item => item.SearchPersonalizado).ToList().SequenceEqual(pComparar2.OpcionesBusqueda.ListaSearchPersonalizado.Select(item => item.SearchPersonalizado).ToList()));
            resultado.Add("listView", pComparar1.OpcionesBusqueda.OpcionesVistas.VistaListado != pComparar2.OpcionesBusqueda.OpcionesVistas.VistaListado);
            resultado.Add("listMosaic", pComparar1.OpcionesBusqueda.OpcionesVistas.VistaMosaico != pComparar2.OpcionesBusqueda.OpcionesVistas.VistaMosaico);
            resultado.Add("listMap", pComparar1.OpcionesBusqueda.OpcionesVistas.VistaMapa != pComparar2.OpcionesBusqueda.OpcionesVistas.VistaMapa);
            resultado.Add("listGraph", pComparar1.OpcionesBusqueda.OpcionesVistas.VistaGrafico != pComparar2.OpcionesBusqueda.OpcionesVistas.VistaGrafico);
            resultado.Add("defaultView", pComparar1.OpcionesBusqueda.OpcionesVistas.VistaPorDefecto != pComparar2.OpcionesBusqueda.OpcionesVistas.VistaPorDefecto);
            resultado.Add("numResults", pComparar1.OpcionesBusqueda.NumeroResultados != pComparar2.OpcionesBusqueda.NumeroResultados);
            resultado.Add("showFacets", pComparar1.OpcionesBusqueda.MostrarFacetas != pComparar2.OpcionesBusqueda.MostrarFacetas);
            resultado.Add("groupFacets", pComparar1.OpcionesBusqueda.AgruparFacetasPorTipo != pComparar2.OpcionesBusqueda.AgruparFacetasPorTipo);
            resultado.Add("proySearch", !pComparar1.OpcionesBusqueda.ProyectoOrigenBusqueda.Equals(pComparar2.OpcionesBusqueda.ProyectoOrigenBusqueda));
            resultado.Add("hideResults", pComparar1.OpcionesBusqueda.OcultarResultadosSinFiltros != pComparar2.OpcionesBusqueda.OcultarResultadosSinFiltros);
            resultado.Add("textHideResults", !pComparar1.OpcionesBusqueda.TextoBusquedaSinResultados.Equals(pComparar2.OpcionesBusqueda.TextoBusquedaSinResultados));
            resultado.Add("ignorePrivacy", pComparar1.OpcionesBusqueda.IgnorarPrivacidadEnBusqueda != pComparar2.OpcionesBusqueda.IgnorarPrivacidadEnBusqueda);
            resultado.Add("skipFirstLoad", pComparar1.OpcionesBusqueda.OmitirCargaInicialFacetasResultados != pComparar2.OpcionesBusqueda.OmitirCargaInicialFacetasResultados);
            resultado.Add("relationMandatory", !pComparar1.OpcionesBusqueda.RelacionMandatory.Equals(pComparar2.OpcionesBusqueda.RelacionMandatory));
            resultado.Add("exportDiff", pComparar1.ListaExportaciones.Count != pComparar2.ListaExportaciones.Count);
            resultado.Add("orderDiff", pComparar1.OpcionesBusqueda.FiltrosOrden.Count != pComparar2.OpcionesBusqueda.FiltrosOrden.Count);
            resultado.Add("facetDiff", pComparar1.ListaFacetas.Count != pComparar2.ListaFacetas.Count);
            return resultado;
        }

        private List<TabModel> CargarComparadorConfigPestanyas(Guid pConfigPestanya1, Guid pConfigPestanya2)
        {
            List<TabModel> resultado = new List<TabModel>();

            List<ProyectoPestanyaMenuVersionPaginaModel> configPestanyas = ControladorPestanyas.ObtenerListaVersionesComparador(new List<Guid> { pConfigPestanya1, pConfigPestanya2 });

            foreach (ProyectoPestanyaMenuVersionPaginaModel version in configPestanyas)
            {
                TabModel pestanyaRestaurar = JsonConvert.DeserializeObject<TabModel>(version.ModeloJSON);
                pestanyaRestaurar.ClassCSSBody = pestanyaRestaurar.ClassCSSBody ?? "";
                pestanyaRestaurar.HtmlAlternativoPrivacidad = pestanyaRestaurar.HtmlAlternativoPrivacidad ?? "";
                pestanyaRestaurar.PrivacidadPerfiles = pestanyaRestaurar.PrivacidadPerfiles ?? new Dictionary<Guid, string>();
                pestanyaRestaurar.PrivacidadGrupos = pestanyaRestaurar.PrivacidadGrupos ?? new Dictionary<Guid, string>();
                pestanyaRestaurar.OpcionesBusqueda = pestanyaRestaurar.OpcionesBusqueda ?? new SearchTabModel();
                pestanyaRestaurar.OpcionesBusqueda.OpcionesVistas = pestanyaRestaurar.OpcionesBusqueda.OpcionesVistas ?? new ViewsSearchTabModel();
                pestanyaRestaurar.OpcionesBusqueda.CampoFiltro = pestanyaRestaurar.OpcionesBusqueda.CampoFiltro ?? "";
                pestanyaRestaurar.OpcionesBusqueda.ListaSearchPersonalizado = pestanyaRestaurar.OpcionesBusqueda.ListaSearchPersonalizado ?? new List<SearchPersonalizadoTabModel>();
                pestanyaRestaurar.OpcionesBusqueda.FiltrosOrden = pestanyaRestaurar.OpcionesBusqueda.FiltrosOrden ?? new List<FiltroOrden>();
                pestanyaRestaurar.OpcionesBusqueda.RelacionMandatory = pestanyaRestaurar.OpcionesBusqueda.RelacionMandatory ?? "";
                pestanyaRestaurar.OpcionesBusqueda.TextoBusquedaSinResultados = pestanyaRestaurar.OpcionesBusqueda.TextoBusquedaSinResultados ?? "";
                pestanyaRestaurar.VersionPagina = ControladorPestanyas.ObtenerListaVersionesSinModelo(version.PestanyaID).OrderBy(item => item.Fecha).ToList();
                pestanyaRestaurar.Version = pestanyaRestaurar.VersionPagina.FindIndex(item => item.VersionID.Equals(version.VersionID)) + 1;
                pestanyaRestaurar.VersionActual = pestanyaRestaurar.VersionPagina.Last().VersionID.Equals(version.VersionID);
                pestanyaRestaurar.VersionID = version.VersionID;
                pestanyaRestaurar.Comparando = true;
                pestanyaRestaurar.ListaExportaciones = pestanyaRestaurar.ListaExportaciones ?? new List<ExportacionSearchTabModel>();
                pestanyaRestaurar.OpcionesDashboard = pestanyaRestaurar.OpcionesDashboard ?? new List<DashboardTabModel>();

                resultado.Add(pestanyaRestaurar);
            }
            TabModel comparar1 = resultado[0];
            TabModel comparar2 = resultado[1];
            comparar1.ComparacionesMultiIdioma = CompararCamposMultiIdiomaPestanyas(comparar1, comparar2);
            comparar1.Comparaciones = CompararCamposPestanyas(comparar1, comparar2);
            comparar2.ComparacionesMultiIdioma = comparar1.ComparacionesMultiIdioma;
            comparar2.Comparaciones = comparar1.Comparaciones;

            return resultado;
        }

        private List<AdministrarPaginaVersionViewModel> CargarVersionesPestanya(Guid pPestanyaID)
        {
            List<AdministrarPaginaVersionViewModel> modelo = new List<AdministrarPaginaVersionViewModel>();
            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory);

            List<ProyectoPestanyaMenuVersionPaginaModel> versionesPagina = contrPest.ObtenerListaVersionesSinModelo(pPestanyaID).OrderBy(item => item.Fecha).ToList();
            using (IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory))
            {
                int contador = 1;
                foreach (ProyectoPestanyaMenuVersionPaginaModel versionPaginaCMS in versionesPagina)
                {
                    AdministrarPaginaVersionViewModel versionPagina = new AdministrarPaginaVersionViewModel(null, versionPaginaCMS.VersionID, contador, versionesPagina.Last().VersionID.Equals(versionPaginaCMS.VersionID), versionPaginaCMS.Fecha, versionPaginaCMS.Comentario, identidadCN.ObtenerNombreDeIdentidad(versionPaginaCMS.IdentidadID));
                    modelo.Add(versionPagina);
                    contador++;
                }
            }
            return modelo;
        }

        #endregion

        #region Propiedades

        private AdministrarPestanyasViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorFacetas>(), mLoggerFactory);
                    List<FacetaModel> listaFacetas = conFacetas.CargarListadoFacetas();

                    mPaginaModel = new AdministrarPestanyasViewModel();
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

                    if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma))
                    {
                        mPaginaModel.ContenidoMultiIdioma = true;
                    }

                    if (ParametrosGeneralesRow.IdiomasDisponibles)
                    {
                        mPaginaModel.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
                    }
                    else
                    {
                        mPaginaModel.ListaIdiomas = new Dictionary<string, string>();
                        mPaginaModel.ListaIdiomas.Add(IdiomaPorDefecto, paramCL.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
                    }

                    mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;

                    mPaginaModel.CMSDisponible = ParametrosGeneralesRow.CMSDisponible;

                    CargarListaOntologias();

                    mPaginaModel.ListaPestanyas = new List<TabModel>();

                    ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, mAvailableServices, mLoggerFactory.CreateLogger<ControladorPestanyas>(), mLoggerFactory);

                    foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya in GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu)
                    {
                        TabModel pestanya = contrPest.CargarPestanya(filaPestanya);
                        pestanya.VersionPagina = contrPest.ObtenerListaVersionesSinModelo(pestanya.Key);
                        if (pestanya.Type.Equals(TipoPestanyaMenu.CMS) || (pestanya.Type.Equals(TipoPestanyaMenu.Home) && (pestanya.HomeCMS.HomeTodosUsuarios || pestanya.HomeCMS.HomeMiembros || pestanya.HomeCMS.HomeNoMiembros)))
                        {
                            CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
                            pestanya.PaginaCMSVersionado = CMSCN.ObtenerVersionesEstructuraPaginaCMS(pestanya.Key).Count > 0;
                        }
                        if (string.IsNullOrEmpty(pestanya.Name) || string.IsNullOrEmpty(pestanya.Url))
                        {
                            KeyValuePair<string, string> nameUrl = ObtenerNameUrlPestanya(filaPestanya, true);

                            if (pestanya.EsNombrePorDefecto)
                            {
                                pestanya.Name = nameUrl.Key;
                            }
                            if (pestanya.EsUrlPorDefecto)
                            {
                                pestanya.Url = nameUrl.Value;
                            }
                        }

                        if (pestanya.OpcionesBusqueda != null)
                        {
                            pestanya.OpcionesBusqueda.ListaSearchPersonalizado = CargarListaSearchPersonalizado(filaPestanya.PestanyaID);
                        }

                        mPaginaModel.ListaPestanyas.Add(pestanya);
                    }

                    //Cargamos las paginas de búsqueda que se puedan crear
                    List<string> listaOntologiasBusqueda = new List<string>();
                    foreach (OntologiaProyecto ontologiaProyecto in GestionProyectos.DataWrapperProyectos.ListaOntologiaProyecto.Where(item => item.EsBuscable))
                    {
                        string rdfType = $"rdf:type={ontologiaProyecto.NombreOnt}";
                        if (!mPaginaModel.ListaPestanyas.Any(item => item.OpcionesBusqueda != null && !string.IsNullOrWhiteSpace(item.OpcionesBusqueda.CampoFiltro) && item.OpcionesBusqueda.CampoFiltro.Equals(rdfType)))
                        {
                            listaOntologiasBusqueda.Add(ontologiaProyecto.NombreOnt);
                        }
                    }
                    mPaginaModel.ListaPestanyasOntologia = listaOntologiasBusqueda;
                }

                return mPaginaModel;
            }
        }

        private Dictionary<string, string> ListaOntologias
        {
            get
            {
                Dictionary<string, string> mListaOntologias = null;

                if (mListaOntologias == null)
                {
                    mListaOntologias = new Dictionary<string, string>();

                    foreach (OntologiaProyecto filaOntologia in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaOntologiaProyecto)
                    {
                        mListaOntologias.Add(filaOntologia.OntologiaProyecto1.ToLower(), UtilCadenas.ObtenerTextoDeIdioma(filaOntologia.NombreOnt, UtilIdiomas.LanguageCode, IdiomaPorDefecto));
                    }
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_RECURSOS.ToLower(), UtilIdiomas.GetText("COMMON", "RECURSOS"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_PREGUNTAS.ToLower(), UtilIdiomas.GetText("COMMON", "PREGUNTAS"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_DEBATES.ToLower(), UtilIdiomas.GetText("COMMON", "DEBATES"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_ENCUESTAS.ToLower(), UtilIdiomas.GetText("COMMON", "ENCUESTAS"));
                    if (!mListaOntologias.ContainsKey(FacetadoAD.BUSQUEDA_PERSONA.ToLower()))
                    {
                        mListaOntologias.Add(FacetadoAD.BUSQUEDA_PERSONA.ToLower(), UtilIdiomas.GetText("COMMON", "PERSONAS"));
                    }
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_ORGANIZACION.ToLower(), UtilIdiomas.GetText("COMMON", "ORGANIZACIONES"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_CLASE.ToLower(), UtilIdiomas.GetText("CONFIGURACIONFACETADO", "CLASE"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_GRUPO.ToLower(), UtilIdiomas.GetText("GRUPO", "GRUPOS"));
                }
                return mListaOntologias;
            }
        }

        private GestionProyecto GestionProyectos
        {
            get
            {
                if (mGestionProyectos == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerProyectoPorID(ProyectoSeleccionado.Clave);
                    dataWrapperProyecto.Merge(proyCN.ObtenerFiltrosOrdenesDeProyecto(ProyectoSeleccionado.Clave));

                    proyCN.Dispose();

                    mGestionProyectos = new GestionProyecto(dataWrapperProyecto, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                }

                return mGestionProyectos;
            }
        }

        #endregion
    }
}
