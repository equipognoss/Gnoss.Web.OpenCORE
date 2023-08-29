using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
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
    public class AdministrarMapasChartsController : ControllerBaseWeb
    {
        public AdministrarMapasChartsController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        // GET: AdministrarMapasCharts
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
        public ActionResult AdministrarChart()
        {
            AdministrarChartsViewModel modelo = ControladorMapasCharts.LoadChartsFromBBDD();
            modelo.IdiomaUsuario = IdiomaUsuario;
            modelo.ListaCharts = modelo.ListaCharts.OrderBy(x => x.Orden).ToList();
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            return View("_EditarCharts", modelo);
        }
        public ActionResult GuardarMapa(AdministrarMapaViewModel pMapa)
        {
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
        [HttpPost]
        public ActionResult GuardarChart(List<ChartViewModel> pCharts)
        {
            GnossResult resultado = new GnossResult("Se han guardado con exito", GnossResult.GnossStatus.OK, mViewEngine);
            try
            {
                ControladorMapasCharts.SaveListCharts(pCharts);
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                resultado = new GnossResult("Erro al guardar los charts", GnossResult.GnossStatus.Error, mViewEngine);
            }
            return resultado;
        }
        public ActionResult CrearFila()
        {
            ChartViewModel chart = new ChartViewModel();
            chart.ChartID = Guid.NewGuid().ToString();
            chart.Nombre = $"NuevoComponente@{IdiomaUsuario}";
            Dictionary<string, string> idiomas = mConfigService.ObtenerListaIdiomasDictionary();
            ViewData["0"] = idiomas.FirstOrDefault(x => string.Compare(x.Key, IdiomaUsuario, true) == 0);
            ViewData["1"] = mConfigService.ObtenerListaIdiomasDictionary();
            return PartialView("_EditarChart", chart);
        }
    }
}