﻿using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.Seguridad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;

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
    public class AdministrarPestanyasController : ControllerBaseWeb
    {
        IServiceScopeFactory mScopedFactory;

        public AdministrarPestanyasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IServiceScopeFactory serviceProvider, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
            mScopedFactory = serviceProvider;
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
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult Index()
        {
           
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "estructura edicion edicionPaginas listado no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_Paginas;

            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTRUCTURA");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONPAGINAS", "PAGINAS");
			ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
			List<FacetaModel> listaFacetas = conFacetas.CargarListadoFacetas();
            ViewBag.ListadoFacetasComunidad = listaFacetas;
			return View(PaginaModel);
        }

        /// <summary>
        /// Nueva Pestaña
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaPestanya(short TipoPestanya, string nameonto)
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE);

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
            }
            //TFG Fran
            bool esPestanyaDasboard = TipoPestanya == (short)TipoPestanyaMenu.Dashboard;
            if (esPestanyaDasboard)
            {
                pestanya.OpcionesDashboard = new List<TabModel.DashboardTabModel>();
            }

            ViewBag.ContenidoMultiIdioma = (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma));
            pestanya.ListaIdiomasDisponibles = new List<string>();
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

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
            ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService,mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
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
            ControladorFacetas conFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            List<FacetaModel> listaFacetas = conFacetas.CargarListadoFacetas();
            AdministrarFacetasPestanyasModel model = new AdministrarFacetasPestanyasModel();
            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, false);
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
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
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
            ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE, iniciado);
            string errores = contrPest.ComprobarErrores(ListaPestanyas);
            if (string.IsNullOrEmpty(errores))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;
                try
                {
                    List<TabModel> paginaModelPestanyas = PaginaModel.ListaPestanyas;
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);
                    contrPest.GuardarPestanyas(ListaPestanyas);
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
        #endregion

        #region Metodos

        private void CargarListaOntologias()
        {
            Dictionary<string, string> ListaOntologias = new Dictionary<string, string>();

            foreach (OntologiaProyecto filaOntologia in GestionProyectos.DataWrapperProyectos.ListaOntologiaProyecto)
            {
                ListaOntologias.Add(filaOntologia.OntologiaProyecto1, UtilCadenas.ObtenerTextoDeIdioma(filaOntologia.NombreOnt, UtilIdiomas.LanguageCode, IdiomaPorDefecto));
            }

            ViewBag.ListaOntologias = ListaOntologias;
        }

        #endregion

        #region Propiedades

        private AdministrarPestanyasViewModel PaginaModel
        {
            get
            {

                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarPestanyasViewModel();
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

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

                    ControladorPestanyas contrPest = new ControladorPestanyas(GestionProyectos.ListaProyectos[ProyectoSeleccionado.Clave], ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE);

                    foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya in GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu)
                    {
                        //mPaginaModel.ListaPestanyas.Add(CargarPestanya(filaPestanya));
                        TabModel pestanya = contrPest.CargarPestanya(filaPestanya);

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
                        
                        mPaginaModel.ListaPestanyas.Add(pestanya);
                    }
                    //Cargamos las paginas de búsqueda que se puedan crear
                    List<string> listaOntologiasBusqueda = new List<string>();
					foreach (OntologiaProyecto ontologiaProyecto in GestionProyectos.DataWrapperProyectos.ListaOntologiaProyecto.Where(item => item.EsBuscable))
                    {
                        string rdfType = $"rdf:type={ontologiaProyecto.NombreOnt}";
						if(!mPaginaModel.ListaPestanyas.Any(item => item.OpcionesBusqueda != null && !string.IsNullOrWhiteSpace(item.OpcionesBusqueda.CampoFiltro) && item.OpcionesBusqueda.CampoFiltro.Equals(rdfType)))
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
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerProyectoPorID(ProyectoSeleccionado.Clave);
                    dataWrapperProyecto.Merge(proyCN.ObtenerFiltrosOrdenesDeProyecto(ProyectoSeleccionado.Clave));

                    proyCN.Dispose();

                    mGestionProyectos = new GestionProyecto(dataWrapperProyecto, mLoggingService, mEntityContext);
                }

                return mGestionProyectos;
            }
        }

        #endregion
    }
}
