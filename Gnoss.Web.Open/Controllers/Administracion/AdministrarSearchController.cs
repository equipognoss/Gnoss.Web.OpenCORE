using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para administrar las opciones de ayuda de búsqueda 
    /// </summary>
    public class AdministrarSearchController : ControllerBaseWeb
    {
        public AdministrarSearchController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
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
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
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
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(AdministrarSearchViewModel pOptions)
        {
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
                string[] trozosAuto = autocompletar.Split(',');
                for (int i = 0; i < trozosAuto.Length; i++)
                {
                    listaAutocompletar.Add(trozosAuto[i]);
                }
            }
            

            string txtLibre = pOptions.TagsTxtLibre;
            List<string> listaTxtLibre = new List<string>();
            if (!string.IsNullOrEmpty(txtLibre))
            {
                string[] trozosTxt = txtLibre.Split(',');
                for (int i = 0; i < trozosTxt.Length; i++)
                {
                    listaTxtLibre.Add(trozosTxt[i]);
                }
            }
            

            try
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
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
        /// <summary>
        /// Recargar Autocompletar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult RegenerarAutocompletar(AdministrarSearchViewModel pOptions)
        {
            try
            {
                BaseRecursosComunidadDS baeRecursoDS = new BaseRecursosComunidadDS();
                BaseRecursosComunidadDS.ColaTagsComunidadesRow rabbitMQ = baeRecursoDS.ColaTagsComunidades.NewColaTagsComunidadesRow();
                baeRecursoDS.Dispose();

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                rabbitMQ.TablaBaseProyectoID = proyCN.ObtenerTablaBaseProyectoIDProyectoPorID(ProyectoSeleccionado.Clave);
                rabbitMQ.Tags = "##GENERAR_TODOS_RECURSOS##1##GENERAR_TODOS_RECURSOS##";
                rabbitMQ.Tipo = 0;
                rabbitMQ.Estado = 101;
                rabbitMQ.FechaPuestaEnCola = DateTime.Now;
                rabbitMQ.Prioridad = 1;
                rabbitMQ.EstadoTags = 0;

                InsertarColaTagsComunidades(rabbitMQ);

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
                throw;
            }

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

            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<string> listaConfigAutocompletar = new List<string>();
            List<string> listaConfigSearch = new List<string>();

            listaConfigAutocompletar = paramCN.ObtenerConfigAutocompletar(ProyectoSeleccionado.Clave);

            listaConfigSearch = paramCN.ObtenerConfigSearch(ProyectoSeleccionado.Clave);

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

            string valoresInicialesAuto = string.Empty;
            foreach (string valor in listaConfigAutocompletar)
            {
                valoresInicialesAuto = valoresInicialesAuto + "," + valor;
            }
            adSearch.TagsAutocompletar = valoresInicialesAuto;

            string valoresInicialesSearch = string.Empty;
            foreach (string valor in listaConfigSearch)
            {
                valoresInicialesSearch = valoresInicialesSearch + "," + valor;
            }
            adSearch.TagsTxtLibre = valoresInicialesSearch;

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
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, colaRabbit, mLoggingService, mConfigService, exchange, colaRabbit))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pFilaCola.ItemArray));
                }
            }
        }

        #endregion
    }
}