using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class AdministrarMapasChartsController : ControllerBaseWeb
    {
        public AdministrarMapasChartsController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult AdministrarMapa()
        {
            AdministrarMapaViewModel modelo = ControladorMapasCharts.LoadMapFromBBDD();

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "configuracion descubrimiento-analisis-mapa max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.DescubrimientoAnalisis;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.DescubrimientoAnalisis_Mapa;
            // Establecer el título para el header de DevTools                       
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "DESCUBRIMIENTOYANALISIS");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONPAGINAS", "MAPA");
           
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            return View("_EditarMapa", modelo);
        }

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult AdministrarChart()
        {
            AdministrarChartsViewModel modelo = ControladorMapasCharts.LoadChartsFromBBDD();
            modelo.IdiomaUsuario = IdiomaUsuario;
            modelo.ListaCharts = modelo.ListaCharts.OrderBy(x => x.Orden).ToList();

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "edicionPaginas configuracion descubrimiento-analisis-mapa no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.DescubrimientoAnalisis;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.DescubrimientoAnalisis_Graficos;
            // Establecer el título para el header de DevTools                       
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "DESCUBRIMIENTOYANALISIS");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ADMINISTRARGRAFICOS");			


			// Establecer en el ViewBag el idioma por defecto
			ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;

			EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
			//return View("_EditarCharts", modelo);
			return View("Index", modelo);
		}

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult GuardarMapa(AdministrarMapaViewModel pMapa)
        {
            GuardarLogAuditoria();
            GnossResult resultado = new GnossResult("Se han guardado con exito", GnossResult.GnossStatus.OK, mViewEngine);
            try
            {
                ControladorMapasCharts.SaveMap(pMapa);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                resultado = new GnossResult("Erro al guardar los charts", GnossResult.GnossStatus.Error, mViewEngine);
            }
            return resultado;
        }

        /// <summary>
        /// Método para realizar el guardado de los gráficos
        /// </summary>
        /// <param name="pCharts">Chart o gráfico con los datos que se desea guardar</param>
        /// <param name="ChartViewInfoOrderList">Lista con los gráficos existentes en la comunidad donde se incluye el id del gráfico y su posición actual</param>
        /// <returns></returns>

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult GuardarChart(ChartViewModel pCharts, List<ChartViewInfoOrder> ChartViewInfoOrderList)
        {
            GuardarLogAuditoria();          

            GnossResult resultado = new GnossResult("Se han guardado con exito", GnossResult.GnossStatus.OK, mViewEngine);
            try
            {
                ControladorMapasCharts.SaveChart(pCharts, ChartViewInfoOrderList);

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
                if (iniciado)
                {
                    List<ChartViewModel> graficos = ControladorMapasCharts.LoadChartsFromBBDD().ListaCharts;
                    if (pCharts.Eliminada)
                    {
                        if (!graficos.Contains(pCharts))
                        {
                            graficos.Add(pCharts);
                        }
                    }

                    HttpResponseMessage response = InformarCambioAdministracion("Graficos", JsonConvert.SerializeObject(graficos, Formatting.Indented));
                    if (!response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                resultado = new GnossResult("Error al guardar los charts", GnossResult.GnossStatus.Error, mViewEngine);
            }

            return resultado;
        }

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult CrearFila()
        {
            ChartViewModel chart = new ChartViewModel();
            chart.ChartID = Guid.NewGuid().ToString();
            chart.Nombre = $"NuevoComponente@{IdiomaUsuario}";
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			Dictionary<string, string> idiomas = paramCL.ObtenerListaIdiomasDictionary();
            ViewData["0"] = idiomas.FirstOrDefault(x => string.Compare(x.Key, IdiomaUsuario, true) == 0);
            ViewData["1"] = paramCL.ObtenerListaIdiomasDictionary();


			// Establecer en el ViewBag el idioma por defecto
			ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
            // Indicar que es un nuevo gráfico a crear
            ViewBag.isNewGraph = "true";
			//return PartialView("_EditarChart", chart);
			return PartialView("_partial-views/_graphListItem", chart);
        }

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult EliminarChart(string ChartID)
        {
            List<ChartViewModel> graficos = null;

            bool iniciado = false;

            try
            {
                iniciado = HayIntegracionContinua;
                if (iniciado)
                {
                    graficos = ControladorMapasCharts.LoadChartsFromBBDD().ListaCharts;
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                return GnossResultERROR("Contacte con el administrador del Proyecto, no es posible atender la petición.");
            }
            FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperFacetas facetaDW = facetaCN.ObtenerDatosChartProyecto(ProyectoAD.MyGnoss, ProyectoSeleccionado.Clave, new Guid(ChartID));
            
            FacetaConfigProyChart chartEliminar = facetaDW.ListaFacetaConfigProyChart.FirstOrDefault();

            if(chartEliminar != null) 
            {
                mEntityContext.FacetaConfigProyChart.Remove(chartEliminar);
                mEntityContext.SaveChanges();
            }

            facetaCN.Dispose();
           
            if (iniciado)
            {
                ChartViewModel grafico = graficos.Where(x => x.ChartID.ToLower().Equals(ChartID.ToLower())).FirstOrDefault();
                if (grafico != null)
                {
                    grafico.Eliminada = true;
                }              
                
                HttpResponseMessage response = InformarCambioAdministracion("Graficos", JsonConvert.SerializeObject(graficos, Formatting.Indented));
                if (!response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                }
            }

            return GnossResultOK();
        }
    }
}