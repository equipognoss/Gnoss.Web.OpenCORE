using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Universal.Common.Extensions;
using static Es.Riam.Util.UtilWeb;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para administrar las opciones de ayuda de búsqueda 
    /// </summary>
    public class AdministrarSearchController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarSearchController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarSearchController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros
        private AdministrarSearchViewModel mPaginaModel = null;

        #endregion

        #region Métodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarSugerenciasDeBusqueda } })]
		public ActionResult Index()
        {
            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "configuracion sugerencias-busqueda max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.DescubrimientoAnalisis;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.DescubrimientoAnalisis_Sugerencias_de_busqueda;                                 
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "DESCUBRIMIENTOYANALISIS");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONPAGINAS", "SUGERENCIASDEBUSQUEDA");            

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            mPaginaModel = CargarModelo();
            return View(mPaginaModel);
        }
        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarSugerenciasDeBusqueda } })]
		public ActionResult Guardar(AdministrarSearchViewModel pOptions)
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
            string autocompletar = pOptions.TagsAutocompletar;
            List<string> listaAutocompletar = new List<string>();
            if (!string.IsNullOrEmpty(autocompletar))
            {
                string[] trozosAuto = autocompletar.Split(',', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < trozosAuto.Length; i++)
                {
                    listaAutocompletar.Add(trozosAuto[i]);
                }
            }
            

            string txtLibre = pOptions.TagsTxtLibre;
            List<string> listaTxtLibre = new List<string>();
            if (!string.IsNullOrEmpty(txtLibre))
            {
                string[] trozosTxt = txtLibre.Split(',', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < trozosTxt.Length; i++)
                {
                    listaTxtLibre.Add(trozosTxt[i]);
                }
            }
            

            try
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
                paramCN.ActualizarConfigAutocompletar(ProyectoSeleccionado.Clave, listaAutocompletar);
                paramCN.ActualizarConfigSearch(ProyectoSeleccionado.Clave, listaTxtLibre);

                //cambios para IC             
                if (iniciado)
                {
                    ConfigSearchModel configSearchModel = new ConfigSearchModel();
                    configSearchModel.ConfigSearch = txtLibre;
                    configSearchModel.ConfigAutocompletar = autocompletar;
                    HttpResponseMessage resultado = InformarCambioAdministracion("ConfigSearch", JsonConvert.SerializeObject(configSearchModel, Formatting.Indented));
                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }
                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
                throw;  
            }
        }

        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarSugerenciasDeBusqueda } })]
		public ActionResult VerificarPrefijo(string propertyValue,string identidad, string organizacion, string proyecto, string lista)
        {
            HttpRequest pRequest = null;
            string re = @"^[^\r\n]+@@@[^\r\n]+$";
            string url= $"{mConfigService.ObtenerUrlServicio("autocompletar")}/AutoCompletarOntologia";
            string data = $"q={propertyValue.ToLower()}&lista={lista}&identidad={identidad}&organizacion={organizacion}&proyecto={proyecto}&pIdentidadID={identidad}";
            string respuesta = "";

            if (propertyValue.Contains("@"))
            {
                if (Regex.IsMatch(propertyValue,re))
                {
                    respuesta = WebRequest(Metodo.POST, url, data, pRequest);
                    if (!respuesta.Equals("") && respuesta.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Where(item => item.Equals(propertyValue.Split("@@@").Last())).Count() > 0)
                    {
                        return GnossResultOK();
                    }
                    
                }

                return GnossResultERROR("");

            }
            else
            {
                respuesta = WebRequest(Metodo.POST, url, data, pRequest);
                if (!respuesta.Equals("") && respuesta.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Where(item => item.Equals(propertyValue)).Count() > 0)
                {
                    return GnossResultOK();
                }
            }

            return GnossResultERROR();                
        }

        #endregion

        #region Métodos

        private AdministrarSearchViewModel CargarModelo()
        {
            AdministrarSearchViewModel adSearch = new AdministrarSearchViewModel();
            adSearch.PropiedadesParaAutocompletar = string.Empty;
            adSearch.PropiedadesParaTxtLibre = string.Empty;
            adSearch.TagsAutocompletar = string.Empty;
            adSearch.TagsTxtLibre = string.Empty;

            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
            List<string> listaConfigAutocompletar = paramCN.ObtenerConfigAutocompletar(ProyectoSeleccionado.Clave);
            List<string> listaConfigSearch = paramCN.ObtenerConfigSearch(ProyectoSeleccionado.Clave);

            for (int i = 0; i < listaConfigAutocompletar.Count; i++)
            {
                for (int j = 0; j < listaConfigSearch.Count; j++)
                {
                    if (listaConfigAutocompletar[i].Equals(listaConfigSearch[j]))
                    {
                        listaConfigSearch.Remove(listaConfigSearch[j]);
                    }
                }
            }

            StringBuilder valoresInicialesAuto = new StringBuilder();
            foreach (string valor in listaConfigAutocompletar)
            {
                valoresInicialesAuto.Append($"{valor},");
            }
            adSearch.TagsAutocompletar = valoresInicialesAuto.ToString();

            StringBuilder valoresInicialesSearch = new StringBuilder();
            foreach (string valor in listaConfigSearch)
            {
                valoresInicialesSearch.Append($"{valor},");
            }
            adSearch.TagsTxtLibre = valoresInicialesSearch.ToString();

            return adSearch;
        }

        /// <summary>
        /// Inserta en la cola de Rabbit los tags de las comunidades
        /// </summary>
        /// <param name="pFilaCola">Parámetro de la fila para la cola</param>
        public void InsertarColaTagsComunidades(BaseRecursosComunidadDS.ColaTagsComunidadesRow pFilaCola)
        {
            string exchange = "";
            string colaRabbit = "ColaTagsComunidadesGeneradorAutocompletar";

            if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, colaRabbit, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, exchange, colaRabbit))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pFilaCola.ItemArray));
                }
            }
        }

        #endregion
    }
}