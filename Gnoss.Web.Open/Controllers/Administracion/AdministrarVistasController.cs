using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Web.Util;
using Gnoss.Web.Open.Filters;
using Gnoss.Web.Services.VirtualPathProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    public class AdministrarVistasController : ControllerAdministrationWeb
    {
        private readonly string pathResourceDefault;
        private readonly string pathListResourcesDefault;
        private readonly string pathGroupComponentsDefault;

        private const string comandoTraduccion = "Html.Translate(\"";
        private readonly ManageViewsViewModel paginaModel = new ManageViewsViewModel();
        private DataWrapperVistaVirtual mVistaVirtualDW;

        private static List<string> listaVistasResultados;
        private static List<string> listaVistasFacetas;
        private readonly string mViews;
        private const string VIEWS_DIRECTORY = "Views";
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarVistasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarVistasController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mViews = "Views";
            pathResourceDefault = $"/{VIEWS_DIRECTORY}/CMSPagina/ListadoRecursos/Vistas/";
            pathListResourcesDefault = $"/{VIEWS_DIRECTORY}/CMSPagina/ListadoRecursos/";
            pathGroupComponentsDefault = $"/{VIEWS_DIRECTORY}/CMSPagina/GrupoComponentes/";
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
        public ActionResult Index()
        {

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "apariencia vistas edicion no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Apariencia;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Apariencia_Vistas;

            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "APARIENCIA");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONVISTAS", "VISTAS");

            // Controlar si es o no del ecosistema            
            bool isInEcosistemaPlatform = !string.IsNullOrEmpty(RequestParams("ecosistema")) ? (bool.Parse(RequestParams("ecosistema"))) : false;
            if (isInEcosistemaPlatform)
            {
                ViewBag.isInEcosistemaPlatform = "true";
            }

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            CargarDatos();

            return View(paginaModel);
        }

        /// <summary>
        /// Método para gestionar las vistas de la WEB
        /// </summary>
        /// <param name="PaginasPersonalizables">Nombre de la página</param>
        /// <param name="FormulariosSemanticos">Nombre del formularios semántico</param>
        /// <param name="Fichero">Fichero con la vista</param>
        /// <param name="Accion">Acción a relizar</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
		public ActionResult Web(string PaginasPersonalizables, string FormulariosSemanticos, IFormFile Fichero, ManageViewsViewModel.Action Accion)
        {
            CargarDatos();
            string error = "";
            if (string.IsNullOrEmpty(PaginasPersonalizables) && string.IsNullOrEmpty(FormulariosSemanticos))
            {
                error = "Tienes que seleccionar una opción en el combo";
            }
            else if (!string.IsNullOrEmpty(PaginasPersonalizables) && !string.IsNullOrEmpty(FormulariosSemanticos))
            {
                error = "Sólamente puedes seleccionar una opción, no dos";
            }

            if (string.IsNullOrEmpty(error))
            {
                string pagina = PaginasPersonalizables;
                bool esRdfType = false;
                if (string.IsNullOrEmpty(pagina))
                {
                    pagina = FormulariosSemanticos;
                    esRdfType = true;
                }
                if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Download)
                {
                    string lineaSeguridad = $"@*[security|||{pagina.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower()}|||{ProyectoSeleccionado.NombreCorto.ToLower()}]*@";

                    string htmlPagina = DescargarPagina(pagina, false, esRdfType);

                    if (!string.IsNullOrEmpty(htmlPagina))
                    {
                        htmlPagina = $"{lineaSeguridad}\n{htmlPagina}";

                        MemoryStream stream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(stream);
                        writer.Write(htmlPagina);
                        writer.Flush();
                        stream.Position = 0;

                        return File(writer.BaseStream, "application/text", pagina);
                    }
                    else
                    {
                        error = "No hay una plantilla personalizada para esta vista";
                    }
                }
                else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.DownloadOriginal)
                {
                    string fileName = pagina;

                    string htmlPagina = DescargarPagina(pagina, true, esRdfType);
                    if (!string.IsNullOrEmpty(htmlPagina))
                    {
                        MemoryStream stream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(stream);
                        writer.Write(htmlPagina);
                        writer.Flush();
                        stream.Position = 0;

                        return File(writer.BaseStream, "application/text", fileName);
                    }
                    else
                    {
                        error = "No hay una plantilla personalizada para esta vista";
                    }
                }
                else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Upload)
                {
                    if (Fichero != null)
                    {
                        StreamReader sr = new StreamReader(Fichero.OpenReadStream());
                        string security = sr.ReadLine().ToLower();
                        string texto = sr.ReadToEnd();
                        sr.Close();

                        string lineaSeguridad = $"@*[security|||{pagina.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower()}|||{ProyectoSeleccionado.NombreCorto.ToLower()}]*@";

                        if (security.StartsWith("@*[security|||") && security.EndsWith("]*@"))
                        {
                            string seguridadNombrePagina = security.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            string seguridadNombreProy = security.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[2].Replace("]*@", "");

                            if (seguridadNombrePagina == pagina.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower() && seguridadNombreProy == ProyectoSeleccionado.NombreCorto.ToLower() && lineaSeguridad == security)
                            {
                                string errorCompilando = CompilarVista(texto);
                                if (string.IsNullOrEmpty(errorCompilando))
                                {
                                    if (!GuardarPagina(pagina, texto, esRdfType))
                                    {
                                        error = "Error al guardar los datos, intentalo de nuevo";
                                    }
                                }
                                else
                                {
                                    error = errorCompilando;
                                }
                            }
                            else
                            {
                                error = $"La linea de seguridad no concuerda con lo esperado,<br />por la seguirdad de la plataforma, debes añadir la siguiente linea al inicio de la vista:<br />{lineaSeguridad}";
                            }
                        }
                        else
                        {
                            error = $"Por la seguiridad de la plataforma, debes añadir la siguiente linea al inicio de la vista:<br />{lineaSeguridad}";
                        }
                    }
                    else
                    {
                        error = "Tienes que seleccionar un fichero para subir";
                    }
                }
                else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Delete)
                {
                    if (!EliminarPagina(pagina, esRdfType))
                    {
                        error = "Error al eliminar la vista personalizada";
                    }
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }
            else
            {
                CargarDatos();
                ViewBag.Personalizacion = string.Empty;
                ViewBag.PersonalizacionLayout = "";
                paginaModel.OKMessage = "Guardado correctamente";
                return GnossResultHtml("_Web", paginaModel);
            }
        }

        /// <summary>
        /// Método para gestionar las vistas del servicio de resultados
        /// </summary>
        /// <param name="PaginasPersonalizables">Nombre de la página</param>
        /// <param name="Fichero">Fichero con la vista</param>
        /// <param name="Accion">Acción a relizar</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
		[TypeFilter(typeof(AccesoIntegracionAttribute))]
		public ActionResult Resultados(string PaginasPersonalizables, IFormFile Fichero, ManageViewsViewModel.Action Accion)
        {
            string PaginaResultados = PaginasPersonalizables;
            CargarDatos();
            string error = "";

            if (string.IsNullOrEmpty(PaginaResultados))
            {
                error = "Tienes que seleccionar una opción en el combo";
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Download)
            {
                string lineaSeguridad = $"@*[security|||{PaginaResultados.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower()}|||{ProyectoSeleccionado.NombreCorto.ToLower()}]*@";

                string htmlPagina = DescargarPagina(PaginaResultados, false, false);

                if (!htmlPagina.StartsWith(lineaSeguridad))
                {
                    htmlPagina = $"{lineaSeguridad}\n{htmlPagina}";
                }

                if (!string.IsNullOrEmpty(htmlPagina))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(htmlPagina);
                    writer.Flush();
                    stream.Position = 0;

                    return File(writer.BaseStream, "application/text", PaginaResultados);
                }
                else
                {
                    error = "No hay una plantilla personalizada para esta vista";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.DownloadOriginal)
            {
                string fileName = PaginaResultados;
                string htmlPagina = DescargarPagina(PaginaResultados, true, false);
                if (!string.IsNullOrEmpty(htmlPagina))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(htmlPagina);
                    writer.Flush();
                    stream.Position = 0;

                    return File(writer.BaseStream, "application/text", fileName);
                }
                else
                {
                    error = "No hay una plantilla personalizada para esta vista";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Upload)
            {

                if (Fichero != null)
                {
                    StreamReader sr = new StreamReader(Fichero.OpenReadStream());
                    string security = sr.ReadLine().ToLower();
                    string texto = sr.ReadToEnd();
                    sr.Close();


                    string lineaSeguridad = $"@*[security|||{PaginaResultados.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower()}|||{ProyectoSeleccionado.NombreCorto.ToLower()}]*@";

                    if (security.StartsWith("@*[security|||") && security.EndsWith("]*@"))
                    {
                        string seguridadNombrePagina = security.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        string seguridadNombreProy = security.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[2].Replace("]*@", "");

                        if (seguridadNombrePagina == PaginaResultados.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower() && seguridadNombreProy == ProyectoSeleccionado.NombreCorto.ToLower() && lineaSeguridad == security)
                        {
                            string errorCompilando = CompilarVista(texto);
                            if (string.IsNullOrEmpty(errorCompilando))
                            {
                                if (GuardarPagina(PaginaResultados, texto, false))
                                {
                                    string urlsResultados = mConfigService.ObtenerUrlServicioResultados();
                                    string[] urlsServicios = urlsResultados.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (string url in urlsServicios)
                                    {
                                        CargadorResultados cargadorResultados = new CargadorResultados();
                                        cargadorResultados.Url = url;
                                        cargadorResultados.InvalidarVistas(UsuarioActual.IdentidadID);
                                    }
                                }
                                else
                                {
                                    error = "Error al guardar los datos, intentalo de nuevo";
                                }
                            }
                            else
                            {
                                error = errorCompilando;
                            }
                        }
                        else
                        {
                            error = $"La linea de seguridad no concuerda con lo esperado,<br />por la seguirdad de la plataforma, debes añadir la siguiente linea al inicio de la vista:<br />{lineaSeguridad}";
                        }
                    }
                    else
                    {
                        error = $"Por la seguirdad de la plataforma, debes añadir la siguiente linea al inicio de la vista:<br />{lineaSeguridad}";
                    }
                }
                else
                {
                    error = "Tienes que seleccionar un fichero para subir";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.DownloadOriginal)
            {
                string lineaSeguridad = $"@*[security|||{PaginaResultados.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower()}|||{ProyectoSeleccionado.NombreCorto.ToLower()}]*@";

                string htmlPagina = DescargarPagina(PaginaResultados, true, false);

                if (!htmlPagina.StartsWith(lineaSeguridad))
                {
                    htmlPagina = $"{lineaSeguridad}\n{htmlPagina}";
                }

                if (!string.IsNullOrEmpty(htmlPagina))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(htmlPagina);
                    writer.Flush();
                    stream.Position = 0;

                    return File(writer.BaseStream, "application/text", PaginaResultados);
                }
                else
                {
                    error = "No hay una plantilla personalizada para esta vista";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Delete)
            {
                if (EliminarPagina(PaginaResultados, false))
                {
                    CargadorResultados cargadorResultados = new CargadorResultados();
                    cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();
                    cargadorResultados.InvalidarVistas(UsuarioActual.IdentidadID);
                }
                else
                {
                    error = "Error al eliminar la vista personalizada";
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }
            else
            {
                CargarDatos();
                ViewBag.Personalizacion = string.Empty;
                ViewBag.PersonalizacionLayout = "";
                paginaModel.OKMessage = "Guardado correctamente";
                return GnossResultHtml("_Resultados", paginaModel);
            }
        }


        /// <summary>
        /// Método para gestionar las vistas del servicio de facetas
        /// </summary>
        /// <param name="PaginasPersonalizables">Nombre de la página</param>
        /// <param name="Fichero">Fichero con la vista</param>
        /// <param name="Accion">Acción a relizar</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
		[TypeFilter(typeof(AccesoIntegracionAttribute))]
		public ActionResult Facetas(string PaginasPersonalizables, IFormFile Fichero, ManageViewsViewModel.Action Accion)
        {
            string PaginaFacetas = PaginasPersonalizables;

            CargarDatos();
            string error = "";
            if (string.IsNullOrEmpty(PaginaFacetas))
            {
                error = "Tienes que seleccionar una opción en el combo";
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Download)
            {
                string htmlPagina = DescargarPagina(PaginaFacetas, false, false);
                if (!string.IsNullOrEmpty(htmlPagina))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(htmlPagina);
                    writer.Flush();
                    stream.Position = 0;

                    return File(writer.BaseStream, "application/text", PaginaFacetas);
                }
                else
                {
                    error = "No hay una plantilla personalizada para esta vista";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.DownloadOriginal)
            {
                string fileName = PaginaFacetas;

                string lineaSeguridad = $"@*[security|||{PaginaFacetas.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower()}|||{ProyectoSeleccionado.NombreCorto.ToLower()}]*@";
                string htmlPagina = DescargarPagina(PaginaFacetas, true, false);
                if (!string.IsNullOrEmpty(htmlPagina))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(htmlPagina);
                    writer.Flush();
                    stream.Position = 0;

                    return File(writer.BaseStream, "application/text", fileName);
                }
                else
                {
                    error = "No hay una plantilla personalizada para esta vista";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Upload)
            {
                if (Fichero != null)
                {
                    StreamReader sr = new StreamReader(Fichero.OpenReadStream());
                    string security = sr.ReadLine().ToLower();
                    string texto = sr.ReadToEnd();
                    sr.Close();

                    string lineaSeguridad = $"@*[security|||{PaginasPersonalizables.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower()}|||{ProyectoSeleccionado.NombreCorto.ToLower()}]*@";

                    if (security.StartsWith("@*[security|||") && security.EndsWith("]*@"))
                    {
                        string seguridadNombrePagina = security.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        string seguridadNombreProy = security.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[2].Replace("]*@", "");

                        if (seguridadNombrePagina == PaginasPersonalizables.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower() && seguridadNombreProy == ProyectoSeleccionado.NombreCorto.ToLower() && lineaSeguridad == security)
                        {
                            string errorCompilando = CompilarVista(texto);
                            if (string.IsNullOrEmpty(errorCompilando))
                            {
                                if (GuardarPagina(PaginaFacetas, texto, false))
                                {
                                    CargadorFacetas cargadorFacetas = new CargadorFacetas();
                                    cargadorFacetas.Url = mConfigService.ObtenerUrlServicioFacetas();
                                    cargadorFacetas.InvalidarVistas(UsuarioActual.IdentidadID);
                                }
                                else
                                {
                                    error = "Error al guardar los datos, intentalo de nuevo";
                                }
                            }
                            else
                            {
                                error = errorCompilando;
                            }
                        }
                        else
                        {
                            error = $"La linea de seguridad no concuerda con lo esperado,<br />por la seguirdad de la plataforma, debes añadir la siguiente linea al inicio de la vista:<br />{lineaSeguridad}";
                        }
                    }
                    else
                    {
                        error = $"Por la seguirdad de la plataforma, debes añadir la siguiente linea al inicio de la vista:<br />{lineaSeguridad}";
                    }
                }
                else
                {
                    error = "Tienes que seleccionar un fichero para subir";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.DownloadOriginal)
            {
                string htmlPagina = DescargarPagina(PaginaFacetas, true, false);
                if (!string.IsNullOrEmpty(htmlPagina))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(htmlPagina);
                    writer.Flush();
                    stream.Position = 0;

                    return File(writer.BaseStream, "application/text", PaginaFacetas);
                }
                else
                {
                    error = "No hay una plantilla personalizada para esta vista";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Delete)
            {
                if (EliminarPagina(PaginaFacetas, false))
                {
                    CargadorFacetas cargadorFacetas = new CargadorFacetas();
                    cargadorFacetas.Url = mConfigService.ObtenerUrlServicioFacetas();
                    cargadorFacetas.InvalidarVistas(UsuarioActual.IdentidadID);
                }
                else
                {
                    error = "Error al eliminar la vista personalizada";
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }
            else
            {
                CargarDatos();
                ViewBag.Personalizacion = string.Empty;
                ViewBag.PersonalizacionLayout = "";
                paginaModel.OKMessage = "Guardado correctamente";
                return GnossResultHtml("_Facetas", paginaModel);
            }
        }

        /// <summary>
        /// Método para gestionar las vistas del CMS
        /// </summary>
        /// <param name="ComponentePersonalizable">Nombre de la página</param>
        /// <param name="idPersonalizacion">ID de la personalización</param>
        /// <param name="Nombre">Nombre de la vista</param>
        /// <param name="Fichero">Fichero con la vista</param>
        /// <param name="Accion">Acción a relizar</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
		[TypeFilter(typeof(AccesoIntegracionAttribute))]
		public ActionResult CMS(string ComponentePersonalizable, Guid idPersonalizacion, string Nombre, IFormFile Fichero, ManageViewsViewModel.Action Accion)
        {
            CargarDatos();
            string error = "";
            if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Download)
            {
                string fileName = ComponentePersonalizable;
                string htmlPagina = DescargarComponenteCMS(ComponentePersonalizable, false, idPersonalizacion);
                if (!string.IsNullOrEmpty(htmlPagina))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(htmlPagina);
                    writer.Flush();
                    stream.Position = 0;

                    return File(writer.BaseStream, "application/text", fileName);
                }
                else
                {
                    error = "No hay una plantilla personalizada para esta vista";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.DownloadOriginal)
            {
                string fileName = ComponentePersonalizable;
                string htmlPagina = DescargarComponenteCMS(ComponentePersonalizable, true, Guid.Empty);
                if (!string.IsNullOrEmpty(htmlPagina))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(htmlPagina);
                    writer.Flush();
                    stream.Position = 0;

                    return File(writer.BaseStream, "application/text", fileName);
                }
                else
                {
                    error = "No hay una plantilla personalizada para esta vista";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Upload)
            {
                if (Fichero != null)
                {
                    if (string.IsNullOrEmpty(Nombre.Trim()))
                    {
                        error = "Hay que especificar un nombre en el componente";
                    }
                    else
                    {
                        StreamReader sr = new StreamReader(Fichero.OpenReadStream());
                        string texto = sr.ReadToEnd();
                        sr.Close();

                        if (idPersonalizacion == Guid.Empty)
                        {
                            idPersonalizacion = Guid.NewGuid();
                        }

                        string errorCompilando = CompilarVista(texto);
                        if (string.IsNullOrEmpty(errorCompilando))
                        {
                            if (ComponentePersonalizable == pathResourceDefault || ComponentePersonalizable == pathListResourcesDefault || ComponentePersonalizable == pathGroupComponentsDefault)
                            {
                                ComponentePersonalizable = $"{ComponentePersonalizable}_{Guid.NewGuid()}.cshtml";
                            }

                            List<VistaVirtualCMS> vistaVirtualAnterior = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.PersonalizacionComponenteID.Equals(idPersonalizacion)).ToList();
                            string datosExtra = "";
                            if (vistaVirtualAnterior.Count == 1 && vistaVirtualAnterior[0].DatosExtra != null)
                            {
                                datosExtra = vistaVirtualAnterior[0].DatosExtra;
                            }

                            if (!GuardarComponenteCMS(ComponentePersonalizable, texto, Nombre, datosExtra, idPersonalizacion))
                            {
                                error = "Error al guardar los datos, intentalo de nuevo";
                            }
                        }
                        else
                        {
                            error = errorCompilando;
                        }
                    }
                }
                else
                {
                    error = "Tienes que seleccionar un fichero para subir";
                }
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Delete)
            {
                if (!EliminarComponenteCMS(ComponentePersonalizable, idPersonalizacion))
                {
                    error = "Error al eliminar la vista personalizada";
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }
            else
            {
                CargarDatos();
                ViewBag.Personalizacion = string.Empty;
                ViewBag.PersonalizacionLayout = "";
                paginaModel.OKMessage = "Guardado correctamente";
                return GnossResultHtml("_CMS", paginaModel);
            }
        }


        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
		[TypeFilter(typeof(AccesoIntegracionAttribute))]
		public ActionResult CMSExtra(string ComponentePersonalizable, Guid idPersonalizacion, bool ResourcesExtra, bool Identities, bool IdentitiesExtra, ManageViewsViewModel.Action Accion, IFormFile Fichero, string Nombre)
        {
            CargarDatos();
            string error = "";
            if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Save)
            {
                string DatosExtra = "";
                if (ResourcesExtra)
                {
                    DatosExtra = $"{DatosExtra}{DatosExtraVistas.DatosExtraRecursos.ToString()}";
                }
                DatosExtra = $"{DatosExtra}&";
                if (Identities)
                {
                    DatosExtra = $"{DatosExtra}{DatosExtraVistas.Identidades.ToString()}";
                }
                DatosExtra = $"{DatosExtra}&";
                if (IdentitiesExtra)
                {
                    DatosExtra = $"{DatosExtra}{DatosExtraVistas.DatosExtraIdentidades.ToString()}";
                }

                if (Fichero != null)
                {
                    if (string.IsNullOrEmpty(Nombre.Trim()))
                    {
                        error = "Hay que especificar un nombre en el componente";
                    }
                    else
                    {
                        StreamReader sr = new StreamReader(Fichero.OpenReadStream());
                        string texto = sr.ReadToEnd();
                        sr.Close();

                        if (idPersonalizacion == Guid.Empty)
                        {
                            idPersonalizacion = Guid.NewGuid();
                        }

                        string errorCompilando = CompilarVista(texto);
                        if (string.IsNullOrEmpty(errorCompilando))
                        {
                            if (ComponentePersonalizable == pathResourceDefault || ComponentePersonalizable == pathListResourcesDefault || ComponentePersonalizable == pathGroupComponentsDefault)
                            {
                                ComponentePersonalizable = $"{ComponentePersonalizable}_{Guid.NewGuid()}.cshtml";
                            }


                            if (!GuardarComponenteCMS(ComponentePersonalizable, texto, Nombre, DatosExtra, idPersonalizacion))
                            {
                                error = "Error al guardar los datos, intentalo de nuevo";
                            }
                        }
                        else
                        {
                            error = errorCompilando;
                        }
                    }
                }
                else
                {
                    List<VistaVirtualCMS> vistaVirtualAnterior = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.PersonalizacionComponenteID.Equals(idPersonalizacion)).ToList();
                    if (vistaVirtualAnterior.Count > 0)
                    {
                        string HTML = vistaVirtualAnterior[0].HTML;
                        string NombreVista = vistaVirtualAnterior[0].Nombre;

                        if (!GuardarComponenteCMS(ComponentePersonalizable, HTML, NombreVista, DatosExtra, idPersonalizacion))
                        {
                            error = "Error al guardar los datos, intentalo de nuevo";
                        }
                    }
                    else
                    {
                        error = $"El componente '{ComponentePersonalizable}' con id='{idPersonalizacion}' no existe";
                    }
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }
            else
            {
                CargarDatos();
                ViewBag.Personalizacion = string.Empty;
                ViewBag.PersonalizacionLayout = "";
                paginaModel.OKMessage = "Guardado correctamente";
                return GnossResultHtml("_CMS", paginaModel);
            }
        }

        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
		public ActionResult InvalidarVistas()
        {
            LimpiarCacheVistaEntera();

            CargarDatos();
            ViewBag.Personalizacion = string.Empty;
            ViewBag.PersonalizacionLayout = "";
            paginaModel.OKMessage = "Caches invalidadas correctamente";
            return GnossResultOK("Caches invalidadas correctamente");
        }

        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
		public ActionResult CompartirVistasEnDominio(string pDominio)
        {
            string error = string.Empty;

            bool yaCompartido = ComprobarDominioYaCompartido(pDominio.ToLower());
            if (!yaCompartido)
            {
                error = CompartirPersonalizacionEnDominio(pDominio.ToLower());
            }
            else
            {
                error = $"La personalización de este proyecto ya está compartida en el dominio: {pDominio}";
            }

            CargarDatos();
            ViewBag.Personalizacion = string.Empty;
            ViewBag.PersonalizacionLayout = "";
            paginaModel.OKMessage = "Vistas compartidas correctamente";

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }
            else
            {
                return GnossResultOK("Vistas compartidas correctamente");
            }
        }

        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarVistas } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarVistasEcosistema } })]
		public ActionResult DejarDeCompartirVistasEnDominio(string pDominios)
        {
            string error = string.Empty;

            if (!string.IsNullOrEmpty(pDominios))
            {
                string[] dominios = pDominios.Split(',');

                foreach (string dominio in dominios)
                {
                    bool estaCompartido = ComprobarDominioYaCompartido(dominio.ToLower());

                    if (estaCompartido)
                    {
                        error = DejarDeCompartirPersonalizacionEnDominio(dominio.ToLower());
                    }
                    else
                    {
                        error = $"El dominio {dominio} no tiene la personalización compartida";
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        break;
                    }
                }
            }
            else
            {
                error = $"No se ha seleccionado ningún dominio.";
            }

            CargarDatos();
            ViewBag.Personalizacion = string.Empty;
            ViewBag.PersonalizacionLayout = "";
            paginaModel.OKMessage = "La personalización se ha dejado compartir correctamente en los dominios.";

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }
            else
            {
                return GnossResultOK("La personalización se ha dejado compartir correctamente en los dominios seleccionados.");
            }
        }

        private string CompilarVista(string pHtml)
        {
            Guid testID = Guid.NewGuid();
            string vistaTemporal = $"/{VIEWS_DIRECTORY}/TESTvistaTEST/{testID}$$${testID}.cshtml";

            string errorCompilando = null;

            try
            {
                ViewRenderer viewRender = new ViewRenderer(mViewEngine);
                viewRender.RenderView("~" + vistaTemporal, null, ControllerContext, pHtml, TempData);
            }
            catch (Exception ex)
            {
                //La vista no compila: 
                errorCompilando = ex.Message;
            }

            if (string.IsNullOrEmpty(errorCompilando))
            {
                try
                {
                    errorCompilando = BuscarTextoTraducir(pHtml);

                    ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCL>(), mLoggerFactory);
                    paramGeneralCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
                }
                catch (Exception ex)
                {
                    errorCompilando = ex.Message;
                }
            }

            return errorCompilando;
        }

        /// <summary>
        /// Método para añadir a la base de datos el texto a traducir dentro del metodo Html.Translate("....")
        /// </summary>
        /// <param name="pHtml">Codigo html a traducir</param>
        /// <returns>String: codigo traducido</returns>
        private string BuscarTextoTraducir(string pHtml)
        {
            Guid personalizacionID = Guid.Empty;
            GestorParametroGeneral gestorParametroGeneral = new GestorParametroGeneral();
            ParametroGeneralGBD parametroGeneralGBD = new ParametroGeneralGBD(mEntityContext);
            ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCN>(), mLoggerFactory);

            if (EsAdministracionEcosistema)
            {
                personalizacionID = mControladorBase.PersonalizacionEcosistemaID;
                gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = parametroGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(personalizacionID);
            }
            else
            {
                VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCL>(), mLoggerFactory);
                DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave, mControladorBase.PersonalizacionEcosistemaID, mControladorBase.ComunidadExcluidaPersonalizacionEcosistema);
                if (vistaVirtualDW.ListaVistaVirtualProyecto.Count > 0)
                {
                    personalizacionID = (vistaVirtualDW.ListaVistaVirtualProyecto.Where(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault()).PersonalizacionID;
                    gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = parametroGeneralCN.ObtenerTextosPersonalizacionProyecto(ProyectoSeleccionado.Clave);
                }
            }

            if (personalizacionID != Guid.Empty)
            {
                int indice = pHtml.IndexOf(comandoTraduccion, 0);
                int aux = 0;
                string texto = string.Empty;
                string idioma = "es";

                while (indice > -1)
                {
                    try
                    {
                        aux = indice;
                        indice = pHtml.IndexOf("\")", aux);
                        texto = pHtml.Substring(aux, indice - aux);
                        texto = texto.Replace(comandoTraduccion, "");

                        int indiceTexto = 0;
                        bool encontrado = false;
                        while (indiceTexto > -1 && !encontrado)
                        {
                            indiceTexto++;
                            indiceTexto = texto.IndexOf("\"", indiceTexto);
                            if (indiceTexto > -1 && texto[indiceTexto - 1] != Path.DirectorySeparatorChar)
                            {
                                encontrado = true;
                            }
                        }
                        if (indiceTexto > -1)
                        {
                            texto = texto.Substring(0, indiceTexto);
                        }
                        if (texto.Length > 100)
                        {
                            return "ID del texto demasiado largo. En la sección de traducciones (*aquí su comunidad*/administrar-traducciones) cree una traducción con un ID más corto que podrá colocar en la vista, este será remplazado por el texto adecuado al idioma";
                        }
                    }
                    catch
                    {
                        return "Ha ocurrido un error al buscar las claves de traducción. Revise que el todas las traducciones están correctamente escritas y no hay '\"' sin cerrar";
                    }

                    TextosPersonalizadosPersonalizacion traduccion = gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Find(textoPersonalizado => textoPersonalizado.PersonalizacionID.Equals(personalizacionID) && textoPersonalizado.TextoID.Equals(texto, StringComparison.CurrentCultureIgnoreCase) && textoPersonalizado.Language.Equals(idioma));
                    if (traduccion == null)
                    {
                        if (EsAdministracionEcosistema)
                        {
                            TextosPersonalizadosPersonalizacion filaTraduccion = new TextosPersonalizadosPersonalizacion();
                            filaTraduccion.PersonalizacionID = personalizacionID;
                            filaTraduccion.TextoID = texto;
                            filaTraduccion.Texto = texto;
                            filaTraduccion.Language = idioma;
                            parametroGeneralGBD.AddTextosPersonalizadosPersonalizacion(filaTraduccion);
                            gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Add(filaTraduccion);
                        }
                        else
                        {

                            List<TextosPersonalizadosPersonalizacion> listaTextosPersonalizadosPersonalizacionEcosistema = parametroGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
                            traduccion = listaTextosPersonalizadosPersonalizacionEcosistema.Find(textoPersonalizado => textoPersonalizado.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID) && textoPersonalizado.TextoID.Equals(texto, StringComparison.CurrentCultureIgnoreCase) && textoPersonalizado.Language.Equals(idioma));
                            if (traduccion == null)
                            {
                                TextosPersonalizadosPersonalizacion filaTraduccion = new TextosPersonalizadosPersonalizacion();
                                filaTraduccion.PersonalizacionID = personalizacionID;
                                filaTraduccion.TextoID = texto;
                                filaTraduccion.Texto = texto;
                                filaTraduccion.Language = idioma;
                                parametroGeneralGBD.AddTextosPersonalizadosPersonalizacion(filaTraduccion);
                                gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Add(filaTraduccion);
                            }
                        }
                    }
                    indice = pHtml.IndexOf(comandoTraduccion, indice);
                }
                try
                {
                    parametroGeneralGBD.SaveChanges();
                }
                catch (Exception ex)
                {

                    string pattern = @"\((.*?), (.*?), (.*?)\)";
                    MatchCollection matches = Regex.Matches(ex.InnerException.ToString(), pattern);
                    string traduccionVista = matches.FirstOrDefault().Groups[2].Value;

                    if (!string.IsNullOrEmpty(traduccionVista))
                    {
                        string traduccionBD = gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Find(textoBD => textoBD.TextoID.Contains(traduccionVista)).TextoID;

                        return $"Hay un problema con la traduccion \"{traduccionVista}\" de la vista. Revisa que el texto esté correctamente escrito, ya que en base de datos es: \"{traduccionBD}\"";
                    }

                    return $"Revise las traducciones de la vista, hay algunas que no coinciden con las de base de datos.";
                }

            }
            else
            {
                return "No esta habilitada la personalización de vistas. Revise el campo \"Permitir administración de vistas\" en las Opciones de MetaAdministración";
            }
            return null;
        }

        private bool ComprobarDominioYaCompartido(string pDominio)
        {
            VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
            return vistaVirtualCN.ComprobarPersonalizacionCompartidaEnDominio(pDominio, ProyectoSeleccionado.Clave);
        }

        private string CompartirPersonalizacionEnDominio(string pDominio)
        {
            string error = string.Empty;

            try
            {
                VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
                vistaVirtualCN.CompartirPersonalizacionEnDominio(pDominio, ProyectoSeleccionado.Clave);
            }
            catch (Exception ex)
            {
                error = $"Ha habido un error a la hora de compartir la personalización en el dominio: {pDominio}";
                mLoggingService.GuardarLogError($"{error}. ERROR: {ex.Message}", mlogger);
            }

            return error;
        }

        private string DejarDeCompartirPersonalizacionEnDominio(string pDominio)
        {
            string error = string.Empty;

            try
            {
                VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
                vistaVirtualCN.DejarDeCompartirPersonalizacionEnDominio(pDominio, ProyectoSeleccionado.Clave);
            }
            catch (Exception ex)
            {
                error = $"Ha habido un error al intentar dejar de compartir la personalización en el dominio: {pDominio}";
                mLoggingService.GuardarLogError($"{error}. ERROR: {ex.Message}", mlogger);
            }

            return error;
        }

        private void CargarDatos()
        {
            mVistaVirtualDW = null;
            CargarVistaVirtual(VistaVirtualDW);
            CargarVistaRecursos(VistaVirtualDW);
            CargarVistaResultados(VistaVirtualDW);
            CargarVistaFacetas(VistaVirtualDW);
            CargarVistaCMS();
            CargarDominiosCompartidos();

            string urlPagina = UtilIdiomas.GetText("URLSEM", "ADMINISTRARVISTAS");

            if (EsAdministracionEcosistema)
            {
                urlPagina = UtilIdiomas.GetText("URLSEM", "ADMINISTRARVISTASECOSISTEMA");

                if (!ParametrosAplicacionDS.Any(item => item.Parametro.Equals(TiposParametrosAplicacion.PersonalizacionEcosistemaID)))
                {
                    Guid personalizacionID = Guid.NewGuid();

                    mEntityContext.ParametroAplicacion.Add(new ParametroAplicacion() { Parametro = TiposParametrosAplicacion.PersonalizacionEcosistemaID, Valor = personalizacionID.ToString() });
                    mEntityContext.VistaVirtualPersonalizacion.Add(new VistaVirtualPersonalizacion() { PersonalizacionID = personalizacionID });
                    mEntityContext.SaveChanges();

                    ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                    parametroAplicacionCL.InvalidarCacheParametrosAplicacion();

                    mGnossCache.VersionarCacheLocal(ProyectoAD.MetaProyecto);

                    ViewBag.PersonalizacionAdministracionEcosistema = personalizacionID;
                }
            }

            paginaModel.UrlActionWeb = $"{urlPagina}/web";
            paginaModel.UrlActionResults = $"{urlPagina}/results";
            paginaModel.UrlActionFacets = $"{urlPagina}/facets";
            paginaModel.UrlActionCMS = $"{urlPagina}/cms";
            paginaModel.UrlActionCMSExtra = $"{urlPagina}/cms-extra";
            paginaModel.UrlActionInvalidateViews = $"{urlPagina}/invalidateviews";
            paginaModel.UrlActionShareViews = $"{urlPagina}/compartir-vistas-en-dominio";
            paginaModel.UrlActionStopSharing = $"{urlPagina}/dejar-de-compartir";
        }

        private void CargarVistaVirtual(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            string rootPath = mEnv.ContentRootPath;
            if (BaseURL.Contains("depuracion.net"))
            {
                rootPath = rootPath + ".Open";
            }
            List<string> listaVistas = ObtenerFicherosDeDirectorio(Path.Combine(rootPath, mViews));

            paginaModel.ListEditedViews = new List<ManageViewsViewModel.EditedView>();
            paginaModel.ListEditedViews.Insert(0, new ManageViewsViewModel.EditedView { Name=""});
            paginaModel.ListOriginalViews = new List<string>();
            paginaModel.ListOriginalViews.Insert(0, "");
            //Insertamos las vistas que no corresponden con componentes del CMS
            foreach (string nombreVista in listaVistas)
            {
                bool esVistaAdministracion = nombreVista.StartsWith($"/{VIEWS_DIRECTORY}/Administracion");
                bool esVistaCMS = nombreVista.StartsWith($"/{VIEWS_DIRECTORY}/CMSPagina") && nombreVista.LongCount(letra => letra.ToString() == "/") > 3;

                if (!esVistaAdministracion && !esVistaCMS && !nombreVista.StartsWith($"/Views/CargadorFacetas") && !nombreVista.StartsWith($"/Views/CargadorResultados") && !nombreVista.StartsWith($"/Views/Shared/_ResultadoMensaje.cshtml") && !nombreVista.StartsWith($"/Views/CargadorContextoMensajes/CargarContextoMensajes.cshtml"))
                {
                    if (pVistaVirtualDW.ListaVistaVirtual.Any(item => item.TipoPagina.Equals(nombreVista)))
                    {
                        VistaVirtual vistaVirtual = pVistaVirtualDW.ListaVistaVirtual.First(item => item.TipoPagina.Equals(nombreVista));
                        ManageViewsViewModel.EditedView vistaPersonalizada = new ManageViewsViewModel.EditedView();
                        vistaPersonalizada.Name = nombreVista;
                        vistaPersonalizada.FechaCreacion = vistaVirtual.FechaCreacion;
                        vistaPersonalizada.FechaModificacion = vistaVirtual.FechaModificacion;
                        paginaModel.ListEditedViews.Add(vistaPersonalizada);
                    }
                    paginaModel.ListOriginalViews.Add(nombreVista);
                }
            }

            foreach (VistaVirtual filaVistaVirtual in pVistaVirtualDW.ListaVistaVirtual)
            {
                if (!paginaModel.ListEditedViews.Any(item => item.Name.Contains(filaVistaVirtual.TipoPagina)) && !filaVistaVirtual.TipoPagina.StartsWith($"/Views/CargadorFacetas") && !filaVistaVirtual.TipoPagina.StartsWith($"/Views/CargadorResultados"))
                {
                    ManageViewsViewModel.EditedView vistaPersonalizada = new();
                    vistaPersonalizada.Name = filaVistaVirtual.TipoPagina;
                    vistaPersonalizada.FechaCreacion = filaVistaVirtual.FechaCreacion;
                    vistaPersonalizada.FechaModificacion = filaVistaVirtual.FechaModificacion;
                    paginaModel.ListEditedViews.Add(vistaPersonalizada);
                }
            }
        }

        private void CargarDominiosCompartidos()
        {
            VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
            paginaModel.ListDomainsShared = vistaVirtualCN.ObtenerDominiosEstaCompartidaPersonalizacion(ProyectoSeleccionado.Clave);
        }

        private void CargarVistaRecursos(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            if (!EsAdministracionEcosistema)
            {
                List<string> listaFormularios = ObtenerFormulariosSemanticos();
                paginaModel.ListEditedFormsViews = new List<ManageViewsViewModel.EditedView>();
				paginaModel.ListEditedViews.Insert(0, new ManageViewsViewModel.EditedView { Name = "" });
				paginaModel.ListOriginalFormsViews = new List<string>();
                paginaModel.ListOriginalFormsViews.Insert(0, "");

                //Insertamos las vistas que no corresponden con componentes del CMS
                foreach (string nombreVista in listaFormularios)
                {
                    if (pVistaVirtualDW.ListaVistaVirtualRecursos.Any(item => item.RdfType.ToLower().Equals(nombreVista.ToLower())))
                    {
                        VistaVirtualRecursos vista = pVistaVirtualDW.ListaVistaVirtualRecursos.First(item => item.RdfType.ToLower().Equals(nombreVista.ToLower()));
                        ManageViewsViewModel.EditedView vistaPersonalizada = new ManageViewsViewModel.EditedView();
                        vistaPersonalizada.Name = nombreVista;
                        vistaPersonalizada.FechaCreacion = vista.FechaCreacion;
                        vistaPersonalizada.FechaModificacion = vista.FechaModificacion;
						paginaModel.ListEditedFormsViews.Add(vistaPersonalizada);
                    }
                    paginaModel.ListOriginalFormsViews.Add(nombreVista);
                }
            }
        }

        private void CargarVistaResultados(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            List<string> vistasResultados = ObtenerVistasDeResultados();
            paginaModel.ListEditedResultsServiceViews = new List<ManageViewsViewModel.EditedView>();
			paginaModel.ListEditedViews.Insert(0, new ManageViewsViewModel.EditedView { Name = "" });
			paginaModel.ListOriginalResultsServiceViews = new List<string>();
            paginaModel.ListOriginalResultsServiceViews.Insert(0, "");

            foreach (string nombreVista in vistasResultados)
            {
                if (pVistaVirtualDW.ListaVistaVirtual.Any(item => item.TipoPagina.Equals(nombreVista)))
                {
					VistaVirtual vistaVirtual = pVistaVirtualDW.ListaVistaVirtual.First(item => item.TipoPagina.Equals(nombreVista));
					ManageViewsViewModel.EditedView vistaPersonalizada = new ManageViewsViewModel.EditedView();
					vistaPersonalizada.Name = nombreVista;
					vistaPersonalizada.FechaCreacion = vistaVirtual.FechaCreacion;
                    vistaPersonalizada.FechaModificacion = vistaVirtual.FechaModificacion;
					paginaModel.ListEditedResultsServiceViews.Add(vistaPersonalizada);
                }
                paginaModel.ListOriginalResultsServiceViews.Add(nombreVista);
            }

            foreach (var elemento in pVistaVirtualDW.ListaVistaVirtual.Where(item => item.TipoPagina.StartsWith("/Views/CargadorResultados") && !vistasResultados.Contains(item.TipoPagina)))
            {
				ManageViewsViewModel.EditedView vistaPersonalizada = new ManageViewsViewModel.EditedView();
                vistaPersonalizada.Name = elemento.TipoPagina;
				vistaPersonalizada.FechaCreacion = elemento.FechaCreacion;
				vistaPersonalizada.FechaModificacion = elemento.FechaModificacion;
				paginaModel.ListEditedResultsServiceViews.Add(vistaPersonalizada);
			}
        }

        private void CargarVistaFacetas(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            List<string> vistasFacetas = ObtenerVistasDeFacetas();
            paginaModel.ListEditedFacetedServiceViews = new List<ManageViewsViewModel.EditedView>();
			paginaModel.ListEditedViews.Insert(0, new ManageViewsViewModel.EditedView { Name = "" }); 
            paginaModel.ListOriginalFacetedServiceViews = new List<string>();
            paginaModel.ListOriginalFacetedServiceViews.Insert(0, "");

            foreach (string nombreVista in vistasFacetas)
            {
                if (pVistaVirtualDW.ListaVistaVirtual.Any(item => item.TipoPagina.Equals(nombreVista)))
                {
					VistaVirtual vistaVirtual = pVistaVirtualDW.ListaVistaVirtual.First(item => item.TipoPagina.Equals(nombreVista));
					ManageViewsViewModel.EditedView vistaPersonalizada = new ManageViewsViewModel.EditedView();
					vistaPersonalizada.Name = nombreVista;
					vistaPersonalizada.FechaCreacion = vistaVirtual.FechaCreacion;
					vistaPersonalizada.FechaModificacion = vistaVirtual.FechaModificacion;
					paginaModel.ListEditedFacetedServiceViews.Add(vistaPersonalizada);
                }
                paginaModel.ListOriginalFacetedServiceViews.Add(nombreVista);
            }

			foreach (var elemento in pVistaVirtualDW.ListaVistaVirtual.Where(item => item.TipoPagina.StartsWith("/Views/CargadorFacetas") && !vistasFacetas.Contains(item.TipoPagina)))
			{
				ManageViewsViewModel.EditedView vistaPersonalizada = new ManageViewsViewModel.EditedView();
				vistaPersonalizada.Name = elemento.TipoPagina;
				vistaPersonalizada.FechaCreacion = elemento.FechaCreacion;
				vistaPersonalizada.FechaModificacion = elemento.FechaModificacion;
				paginaModel.ListEditedFacetedServiceViews.Add(vistaPersonalizada);
			}
		}

        private void CargarVistaCMS()
        {
            string rootPath = mEnv.ContentRootPath;
            if (BaseURL.Contains("depuracion.net"))
            {
                rootPath = rootPath + ".Open";
            }

            //Almacenamos en listaComponentesDisponibles los componentes disponibles de la comunidad que tienen vistas base
            List<TipoComponenteCMS> listaComponentesDisponibles = new List<TipoComponenteCMS>();
            foreach (TipoComponenteCMS tipoComponenteCMS in UtilComponentes.ListaComponentesPublicos)
            {
                listaComponentesDisponibles.Add(tipoComponenteCMS);
            }

            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
            GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
            cmsCN.Dispose();
            foreach (TipoComponenteCMS tipoComponenteCMS in gestorCMS.ListaComponentesPrivadosProyecto)
            {
                listaComponentesDisponibles.Add(tipoComponenteCMS);
            }

            List<string> listaVistas = ObtenerFicherosDeDirectorio(Path.Combine(rootPath, $"{mViews}", "CMSPagina"));
            mLoggingService.GuardarLogError($"La ruta obtenida es: {Path.Combine(rootPath, $"{mViews}", "CMSPagina")}", mlogger);
            List<TipoComponenteCMS> listaComponentesDisponiblesAux = new List<TipoComponenteCMS>(listaComponentesDisponibles);
            foreach (TipoComponenteCMS componente in listaComponentesDisponiblesAux)
            {
                if (!listaVistas.Contains($"/{mViews}/CMSPagina/{componente}/_{componente}.cshtml"))
                {
                    listaComponentesDisponibles.Remove(componente);
                }
            }

            #region Componentes
            paginaModel.ListCMSComponents = new List<ManageViewsViewModel.CMSComponentViewModel>();
            foreach (TipoComponenteCMS componente in Enum.GetValues(typeof(TipoComponenteCMS)))
            {
                if (listaComponentesDisponibles.Contains(componente))
                {
                    string vistaComponente = $"/{VIEWS_DIRECTORY}/CMSPagina/{componente}/_{componente}.cshtml";

                    ManageViewsViewModel.CMSComponentViewModel componenteActual = new ManageViewsViewModel.CMSComponentViewModel();
                    componenteActual.PathName = vistaComponente;
                    componenteActual.Name = UtilIdiomas.GetText("COMADMINCMS", $"COMPONENTE_{componente.ToString().ToUpper()}");
                    // Diccionario con los componentes CMS personalizados
                    componenteActual.CustomizationName = new Dictionary<Guid, ManageViewsViewModel.EditedView>();

                    foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.Equals(vistaComponente)))
                    {
                        ManageViewsViewModel.EditedView vistaPersonalizada = new ManageViewsViewModel.EditedView();
                        vistaPersonalizada.Name = filaVistaVirtualCMS.Nombre;
                        vistaPersonalizada.FechaCreacion = filaVistaVirtualCMS.FechaCreacion;
                        vistaPersonalizada.FechaModificacion = filaVistaVirtualCMS.FechaModificacion;
                        componenteActual.CustomizationName.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, vistaPersonalizada);
                    }

                    paginaModel.ListCMSComponents.Add(componenteActual);
                }
            }
            #endregion

            #region Recursos
            paginaModel.PathNameResourceDefault = pathResourceDefault;
            paginaModel.ListCMSResources = new List<ManageViewsViewModel.CMSResourceViewModel>();
            string vistaComponenteRecursos = pathResourceDefault;
            List<string> presentacionesRecursosCargadas = new List<string>();
            // Seleccionamos las vistas por defecto
            foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
            {
                //Presentaciones genéricas
                ManageViewsViewModel.CMSResourceViewModel presentacionRecursoActual = new ManageViewsViewModel.CMSResourceViewModel();
                presentacionRecursoActual.PathName = $"{vistaComponenteRecursos}_{tipoPresentacion}.cshtml";
                List<VistaVirtualCMS> filasRecursos = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.Equals(presentacionRecursoActual.PathName)).ToList();
                presentacionRecursoActual.Generic = true;
                presentacionRecursoActual.ExtraInformation = new Dictionary<ManageViewsViewModel.ExtraInformation, bool>();
                if (filasRecursos.Count > 0)
                {
                    presentacionRecursoActual.Name = filasRecursos[0].Nombre;
                    presentacionRecursoActual.CustomizationID = filasRecursos[0].PersonalizacionComponenteID;

                    string datosExtra = filasRecursos[0].DatosExtra;
                    if (datosExtra == null)
                    {
                        datosExtra = "";
                    }
                    string[] datosExtraArray = datosExtra.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> datosExtraList = datosExtraArray.OfType<string>().ToList();
                    if (string.IsNullOrEmpty(datosExtra) || datosExtraList.Contains(DatosExtraVistas.DatosExtraRecursos.ToString()))
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.ResourcesExtra, true);
                    }
                    else
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.ResourcesExtra, false);
                    }
                    if (string.IsNullOrEmpty(datosExtra) || datosExtraList.Contains(DatosExtraVistas.Identidades.ToString()))
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.Identities, true);
                    }
                    else
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.Identities, false);
                    }
                    if (!string.IsNullOrEmpty(datosExtra) && datosExtraList.Contains(DatosExtraVistas.DatosExtraIdentidades.ToString()))
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.IdentitiesExtra, true);
                    }
                    else
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.IdentitiesExtra, false);
                    }
                }
                else
                {
                    presentacionRecursoActual.CustomizationID = Guid.Empty;
                    presentacionRecursoActual.Name = UtilIdiomas.GetText("COMADMINCMS", $"PRESENTACION_{tipoPresentacion.ToString().ToUpper()}");
                    presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.ResourcesExtra, true);
                    presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.Identities, true);
                    presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.IdentitiesExtra, false);
                }
                paginaModel.ListCMSResources.Add(presentacionRecursoActual);
                presentacionesRecursosCargadas.Add(presentacionRecursoActual.PathName);
            }
            foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistaComponenteRecursos)))
            {
                if (!presentacionesRecursosCargadas.Contains(filaVistaVirtualCMS.TipoComponente))
                {
					//Presentaciones nuevas
					ManageViewsViewModel.CMSResourceViewModel presentacionRecursoActual = new ManageViewsViewModel.CMSResourceViewModel();
                    presentacionRecursoActual.PathName = filaVistaVirtualCMS.TipoComponente;
                    presentacionRecursoActual.Name = filaVistaVirtualCMS.Nombre;
                    presentacionRecursoActual.Generic = false;
                    presentacionRecursoActual.CustomizationID = filaVistaVirtualCMS.PersonalizacionComponenteID;
                    presentacionRecursoActual.ExtraInformation = new Dictionary<ManageViewsViewModel.ExtraInformation, bool>();
                    presentacionRecursoActual.FechaCreacion = filaVistaVirtualCMS.FechaCreacion;
                    presentacionRecursoActual.FechaModificacion = filaVistaVirtualCMS.FechaModificacion;

                    List<string> datosExtraList = new List<string>();
                    if (filaVistaVirtualCMS.DatosExtra != null)
                    {
                        string[] datosExtraArray = filaVistaVirtualCMS.DatosExtra.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                        datosExtraList = datosExtraArray.OfType<string>().ToList();
                    }

                    if (string.IsNullOrEmpty(filaVistaVirtualCMS.DatosExtra) || datosExtraList.Contains(DatosExtraVistas.DatosExtraRecursos.ToString()))
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.ResourcesExtra, true);
                    }
                    else
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.ResourcesExtra, false);
                    }
                    if (string.IsNullOrEmpty(filaVistaVirtualCMS.DatosExtra) || datosExtraList.Contains(DatosExtraVistas.Identidades.ToString()))
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.Identities, true);
                    }
                    else
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.Identities, false);
                    }
                    if (!string.IsNullOrEmpty(filaVistaVirtualCMS.DatosExtra) && datosExtraList.Contains(DatosExtraVistas.DatosExtraIdentidades.ToString()))
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.IdentitiesExtra, true);
                    }
                    else
                    {
                        presentacionRecursoActual.ExtraInformation.Add(ManageViewsViewModel.ExtraInformation.IdentitiesExtra, false);
                    }

                    paginaModel.ListCMSResources.Add(presentacionRecursoActual);
                    presentacionesRecursosCargadas.Add(presentacionRecursoActual.PathName);
                }
            }
            #endregion

            #region Listado recursos
            paginaModel.PathNameListResourcesDefault = pathListResourcesDefault;
            paginaModel.ListCMSListResources = new List<ManageViewsViewModel.CMSListResourceViewModel>();
            string vistaComponenteListadoRecursos = pathListResourcesDefault;
            List<string> presentacionesListadoRecursosCargadas = new List<string>();
            foreach (TipoPresentacionListadoRecursosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoRecursosCMS)))
            {
				//Presentaciones genéricas
				ManageViewsViewModel.CMSListResourceViewModel presentacionListadoRecursoActual = new ManageViewsViewModel.CMSListResourceViewModel();
                presentacionListadoRecursoActual.PathName = $"{vistaComponenteListadoRecursos}_{tipoPresentacion}.cshtml";
                List<VistaVirtualCMS> filasRecursos = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.Equals(presentacionListadoRecursoActual.PathName)).ToList();
                presentacionListadoRecursoActual.Generic = true;
                if (filasRecursos.Count > 0)
                {
                    presentacionListadoRecursoActual.Name = filasRecursos[0].Nombre;
                    presentacionListadoRecursoActual.CustomizationID = filasRecursos[0].PersonalizacionComponenteID;
                }
                else
                {
                    presentacionListadoRecursoActual.CustomizationID = Guid.Empty;
                    presentacionListadoRecursoActual.Name = UtilIdiomas.GetText("COMADMINCMS", $"PRESENTACIONLISTADO_{tipoPresentacion.ToString().ToUpper()}");
                }
                paginaModel.ListCMSListResources.Add(presentacionListadoRecursoActual);
                presentacionesListadoRecursosCargadas.Add(presentacionListadoRecursoActual.PathName);
            }
            foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistaComponenteListadoRecursos) && !item.TipoComponente.StartsWith(vistaComponenteRecursos)))
            {
                if (!presentacionesListadoRecursosCargadas.Contains(filaVistaVirtualCMS.TipoComponente))
                {
					//Presentaciones nuevas
					ManageViewsViewModel.CMSListResourceViewModel presentacionListadoRecursosActual = new ManageViewsViewModel.CMSListResourceViewModel();
                    presentacionListadoRecursosActual.PathName = filaVistaVirtualCMS.TipoComponente;
                    presentacionListadoRecursosActual.Name = filaVistaVirtualCMS.Nombre;
                    presentacionListadoRecursosActual.Generic = false;
                    presentacionListadoRecursosActual.CustomizationID = filaVistaVirtualCMS.PersonalizacionComponenteID;
                    presentacionListadoRecursosActual.FechaCreacion = filaVistaVirtualCMS.FechaCreacion;
                    presentacionListadoRecursosActual.FechaModificacion = filaVistaVirtualCMS.FechaModificacion;
                    paginaModel.ListCMSListResources.Add(presentacionListadoRecursosActual);
                    presentacionesListadoRecursosCargadas.Add(presentacionListadoRecursosActual.PathName);
                }
            }

            #endregion

            #region Grupos componentes

            paginaModel.PathNameGroupComponentsDefault = pathGroupComponentsDefault;
            paginaModel.ListCMSGroupComponents = new List<ManageViewsViewModel.CMSGroupComponentViewModel>();
            string vistaComponenteGrupoComponentes = pathGroupComponentsDefault;
            List<string> presentacionesGrupoComponentesCargadas = new List<string>();
            foreach (TipoPresentacionGrupoComponentesCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionGrupoComponentesCMS)))
            {
				//Presentaciones genéricas
				ManageViewsViewModel.CMSGroupComponentViewModel presentacionGrupoComponentesActual = new ManageViewsViewModel.CMSGroupComponentViewModel();
                presentacionGrupoComponentesActual.PathName = $"{vistaComponenteGrupoComponentes}_{tipoPresentacion}.cshtml";
                List<VistaVirtualCMS> filasGruposComponentes = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.Equals(presentacionGrupoComponentesActual.PathName)).ToList();
                presentacionGrupoComponentesActual.Generic = true;
                if (filasGruposComponentes.Count > 0)
                {
                    presentacionGrupoComponentesActual.Name = filasGruposComponentes[0].Nombre;
                    presentacionGrupoComponentesActual.CustomizationID = filasGruposComponentes[0].PersonalizacionComponenteID;
                }
                else
                {
                    presentacionGrupoComponentesActual.CustomizationID = Guid.Empty;
                    presentacionGrupoComponentesActual.Name = UtilIdiomas.GetText("COMADMINCMS", $"PRESENTACIONGRUPO_{tipoPresentacion.ToString().ToUpper()}");
                }
                paginaModel.ListCMSGroupComponents.Add(presentacionGrupoComponentesActual);
                presentacionesGrupoComponentesCargadas.Add(presentacionGrupoComponentesActual.PathName);
            }
            foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistaComponenteGrupoComponentes)))
            {
                if (!presentacionesGrupoComponentesCargadas.Contains(filaVistaVirtualCMS.TipoComponente))
                {
					//Presentaciones nuevas
					ManageViewsViewModel.CMSGroupComponentViewModel presentacionGrupoComponentesActual = new ManageViewsViewModel.CMSGroupComponentViewModel();
                    presentacionGrupoComponentesActual.PathName = filaVistaVirtualCMS.TipoComponente;
                    presentacionGrupoComponentesActual.Name = filaVistaVirtualCMS.Nombre;
                    presentacionGrupoComponentesActual.Generic = false;
                    presentacionGrupoComponentesActual.CustomizationID = filaVistaVirtualCMS.PersonalizacionComponenteID;
                    presentacionGrupoComponentesActual.FechaCreacion = filaVistaVirtualCMS.FechaCreacion;
                    presentacionGrupoComponentesActual.FechaModificacion = filaVistaVirtualCMS.FechaModificacion;
                    paginaModel.ListCMSGroupComponents.Add(presentacionGrupoComponentesActual);
                    presentacionesGrupoComponentesCargadas.Add(presentacionGrupoComponentesActual.PathName);
                }
            }

            #endregion
        }

        private bool GuardarPagina(string pagina, string pHtml, bool pEsRdfType)
        {
            try
            {
                bool iniciado = false;
                try
                {
                    iniciado = HayIntegracionContinua;
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                    throw;
                }

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);

                    if (EsAdministracionEcosistema)
                    {
                        vistaVirtualCN.GuardarHtmlParaVistaDeEcosistema(mControladorBase.PersonalizacionEcosistemaID, pagina, pHtml, pEsRdfType);
                    }
                    else
                    {
                        vistaVirtualCN.GuardarHtmlParaVistaDeProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, null, pagina, pHtml, pEsRdfType);
                    }

                    if (iniciado)
                    {
                        string nombre = pagina;

                        if (pEsRdfType)
                        {
                            nombre = $"/Recursos/{pagina}.cshtml";
                        }

                        HttpResponseMessage resultado = InformarCambioAdministracionVistas("Vistas", nombre, pHtml);

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new ExcepcionWeb("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                    LimpiarCacheVista(pagina, pEsRdfType);
                }
                catch
                {
                    if (transaccionIniciada)
                    {
                        proyAD.TerminarTransaccion(false);
                    }
                    throw;
                }

                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error en el guardado de vistas personalizadas. pagina: {pagina} ", mlogger);
                return false;
            }
        }

        private bool GuardarComponenteCMS(string pagina, string pHtml, string pNombre, string pDatosExtra, Guid pPersonalizacionComponenteID)
        {
            try
            {
                bool iniciado = false;
                try
                {
                    iniciado = HayIntegracionContinua;
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                    throw;
                }

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);

                    if (EsAdministracionEcosistema)
                    {
                        vistaVirtualCN.GuardarHtmlParaVistaDeComponenteCMSdeEcosistema(mControladorBase.PersonalizacionEcosistemaID, pagina, pHtml, pPersonalizacionComponenteID, pNombre, pDatosExtra);
                    }
                    else
                    {
                        vistaVirtualCN.GuardarHtmlParaVistaDeComponenteCMSdeProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, null, pagina, pHtml, pPersonalizacionComponenteID, pNombre, pDatosExtra);
                    }

                    if (iniciado)
                    {
                        AdministrarVistasCMS modelo = new AdministrarVistasCMS();
                        modelo.HTML = pHtml;
                        modelo.Nombre = pNombre;
                        modelo.DatosExtra = pDatosExtra;
                        modelo.PersonalizacionComponenteID = pPersonalizacionComponenteID;
                        pagina = $"CMS/{pagina.Replace($"/{VIEWS_DIRECTORY}/CMSPagina", "").TrimStart('/').Replace(".cshtml", $"_{modelo.Nombre}$$${modelo.PersonalizacionComponenteID}.cshtml")}";
                        modelo.Ruta = pagina;

                        HttpResponseMessage resultado = InformarCambioAdministracion("VistasCMS", JsonConvert.SerializeObject(modelo));
                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new ExcepcionWeb("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    LimpiarCacheComponenteCMS(pPersonalizacionComponenteID);

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch
                {
                    if (transaccionIniciada)
                    {
                        proyAD.TerminarTransaccion(false);
                    }

                    throw;
                }

                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error en el guardado de vistas personalizadas. pagina: {pagina}", mlogger);
                return false;
            }
        }


        private bool EliminarPagina(string pagina, bool pEsRdfType)
        {
            try
            {
                bool iniciado = false;
                try
                {
                    iniciado = HayIntegracionContinua;
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                    throw;
                }

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    Guid personalizacionID;

                    if (pEsRdfType)
                    {
                        personalizacionID = VistaVirtualDW.ListaVistaVirtualRecursos.FirstOrDefault().PersonalizacionID;
                    }
                    else
                    {
                        personalizacionID = VistaVirtualDW.ListaVistaVirtual.FirstOrDefault().PersonalizacionID;
                    }

                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
                    vistaVirtualCN.EliminarHtmlParaVistaDeProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, personalizacionID, pagina, pEsRdfType);

                    if (iniciado)
                    {
                        string nombre = pagina;

                        if (pEsRdfType)
                        {
                            nombre = $"/Recursos/{pagina}.cshtml";
                        }

                        HttpResponseMessage resultado = InformarCambioAdministracion("Vistas", JsonConvert.SerializeObject(new KeyValuePair<string, string>(nombre, "")));
                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new ExcepcionWeb("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    LimpiarCacheVista(pagina, pEsRdfType);

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch
                {
                    if (transaccionIniciada)
                    {
                        proyAD.TerminarTransaccion(false);
                    }

                    throw;
                }
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error en el eliminado de vistas personalizadas. pagina: {pagina} ", mlogger);
                return false;
            }
        }

        private bool EliminarComponenteCMS(string pVista, Guid pPersonalizacionComponenteID)
        {
            try
            {
                bool iniciado = false;
                try
                {
                    iniciado = HayIntegracionContinua;
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                    throw;
                }

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
                    vistaVirtualCN.EliminarHtmlParaVistaDeComponenteCMSdeProyecto(VistaVirtualDW.ListaVistaVirtualCMS[0].PersonalizacionID, pPersonalizacionComponenteID, pVista);

                    if (iniciado)
                    {
                        string nombre = pVista;

                        HttpResponseMessage resultado = InformarCambioAdministracion("VistasCMS", JsonConvert.SerializeObject(new KeyValuePair<string, string>(nombre, "")));

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new ExcepcionWeb("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    LimpiarCacheComponenteCMS(pPersonalizacionComponenteID);

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch
                {
                    if (transaccionIniciada)
                    {
                        proyAD.TerminarTransaccion(false);
                    }

                    throw;
                }

                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error en el eliminado de vistas personalizadas de componentes CMS. pagina: {pVista}", mlogger);
                return false;
            }
        }

        private string DescargarPagina(string pagina, bool pOriginal, bool pEsRdfType)
        {
            if (pOriginal)
            {
                return ObtenerFicheroDeRuta(pagina);
            }
            else
            {
                if (pEsRdfType)
                {
                    List<VistaVirtualRecursos> filas = VistaVirtualDW.ListaVistaVirtualRecursos.Where(item => item.RdfType.Equals(pagina)).ToList();
                    string cshtml = null;
                    if (filas.Count > 0)
                    {
                        cshtml = filas.FirstOrDefault().HTML.ToString();
                    }
                    return cshtml;
                }
                else
                {
                    List<VistaVirtual> filas = VistaVirtualDW.ListaVistaVirtual.Where(item => item.TipoPagina.Equals(pagina)).ToList();
                    string cshtml = null;
                    if (filas.Count > 0)
                    {
                        cshtml = filas.FirstOrDefault().HTML.ToString();
                    }
                    return cshtml;
                }
            }
        }

        private string DescargarComponenteCMS(string pagina, bool pOriginal, Guid pIdPersonalizacion)
        {
            if (pOriginal)
            {
                return ObtenerFicheroDeRuta(pagina);
            }
            else
            {
                List<VistaVirtualCMS> listaVistaVirtualCMS = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.PersonalizacionComponenteID.Equals(pIdPersonalizacion)).ToList();
                string cshtml = null;
                if (listaVistaVirtualCMS.Count > 0)
                {
                    cshtml = listaVistaVirtualCMS[0].HTML;
                }
                return cshtml;
            }
        }

        private void LimpiarCacheVistaEntera()
        {
            ControladorAdministrarVistas.LimpiarCacheVistasRedis(EsAdministracionEcosistema, null, UrlIntragnoss);
            BDVirtualPath.LimpiarListasRutasVirtuales();

            string urlsResultados = mConfigService.ObtenerUrlServicioResultados();
            string[] urlsServicios = urlsResultados.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string url in urlsServicios)
            {
                CargadorResultados cargadorResultados = new CargadorResultados();
                cargadorResultados.Url = url;
                cargadorResultados.InvalidarVistas(UsuarioActual.IdentidadID);
            }

            CargadorFacetas cargadorFacetas = new CargadorFacetas();
            cargadorFacetas.Url = mConfigService.ObtenerUrlServicioFacetas();
            cargadorFacetas.InvalidarVistas(UsuarioActual.IdentidadID);
        }

        private void LimpiarCacheVista(string pVista, bool pEsRdfType)
        {
            VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCL>(), mLoggerFactory);

            if (EsAdministracionEcosistema)
            {
                vistaVirtualCL.InvalidarVistasVirtualesEcosistema();
            }
            else
            {
                vistaVirtualCL.InvalidarVistasVirtuales(ProyectoSeleccionado.Clave);
            }
            vistaVirtualCL.Dispose();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            if (EsAdministracionEcosistema)
            {
                proyCL.InvalidarTodasComunidadesMVC();
                mGnossCache.VersionarCacheLocal(ProyectoAD.MetaProyecto);
            }
            else
            {
                proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
                proyCL.InvalidarComunidadMVC(ProyectoAD.MetaProyecto);
                mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
            }
            proyCL.Dispose();

            if (pEsRdfType)
            {
                pVista = $"/{VIEWS_DIRECTORY}/FichaRecurso/{pVista}.cshtml";
            }

            ControladorAdministrarVistas.LimpiarCacheVistasRedis(EsAdministracionEcosistema, "", UrlIntragnoss, pVista);
        }

        private void LimpiarCacheComponenteCMS(Guid pPersonalizacionComponenteID)
        {
            VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCL>(), mLoggerFactory);

            if (EsAdministracionEcosistema)
            {
                vistaVirtualCL.InvalidarVistasVirtualesEcosistema();
            }
            else
            {
                vistaVirtualCL.InvalidarVistasVirtuales(ProyectoSeleccionado.Clave);
            }
            vistaVirtualCL.Dispose();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            if (EsAdministracionEcosistema)
            {
                proyCL.InvalidarTodasComunidadesMVC();
            }
            else
            {
                proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
                proyCL.InvalidarComunidadMVC(ProyectoAD.MetaProyecto);
            }
            proyCL.Dispose();

            ControladorAdministrarVistas.LimpiarCacheVistasRedis(EsAdministracionEcosistema, $"/{VIEWS_DIRECTORY}/CMSPagina/{pPersonalizacionComponenteID}.cshtml", UrlIntragnoss);
        }

        private List<string> ObtenerFicherosDeDirectorio(string pDirectorio)
        {
            List<string> listaPaginasPersonalizables = new List<string>();

            if (!string.IsNullOrEmpty(pDirectorio) && Directory.Exists(pDirectorio))
            {
                List<string> listaFicherosDirectorio = new List<string>(Directory.GetFiles(pDirectorio));

                foreach (string fichero in listaFicherosDirectorio.Where(fich => fich.EndsWith(".cshtml")))
                {
                    if (!fichero.EndsWith("_ViewStart.cshtml"))
                    {
                        string pagina = fichero;
                        string views = $"{Path.DirectorySeparatorChar}{mViews}{Path.DirectorySeparatorChar}";
                        int indiceViews = pagina.IndexOf(views);

                        pagina = pagina.Substring(indiceViews);
                        pagina = pagina.Replace("\\", "/");
                        listaPaginasPersonalizables.Add(pagina);
                    }
                }

                listaFicherosDirectorio.Clear();

                foreach (string directorio in Directory.GetDirectories(pDirectorio))
                {
                    if (!directorio.EndsWith("ElPrado"))
                    {
                        List<string> listaFicherosSubDirectorios = ObtenerFicherosDeDirectorio(directorio);

                        listaPaginasPersonalizables.AddRange(listaFicherosSubDirectorios);

                        listaFicherosSubDirectorios.Clear();
                    }
                }
            }

            return listaPaginasPersonalizables;
        }

        private string ObtenerFicheroDeRuta(string pRuta)
        {
            string fichero = "";
            string rootPath = mEnv.ContentRootPath;
            if (BaseURL.Contains("depuracion.net"))
            {
                rootPath = rootPath + ".Open";
            }
            using (StreamReader sr = new StreamReader(rootPath + pRuta))
            {
                fichero = sr.ReadToEnd();
            }
            return fichero;
        }

        private List<string> ObtenerFormulariosSemanticos()
        {
            FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
            List<OntologiaProyecto> listaOntologias = facetaCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, false, false);
            Dictionary<string, List<string>> ontologiasProyecto = FacetadoAD.ObtenerInformacionOntologias(listaOntologias);

            List<string> listaFormuariosSemanticos = ontologiasProyecto.Keys.ToList();

            return listaFormuariosSemanticos;
        }

        public static List<string> ObtenerVistasDeResultados()
        {
            if (listaVistasResultados == null)
            {
                listaVistasResultados = new List<string>();
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoBlog.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoComentario.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoComentarioContribuciones.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoComunidad.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoContacto.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoGrupo.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoInvitacion.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoMensaje.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoPerfil.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoRecurso.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoRecursoContribuciones.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoRecursoMisRecursos.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_ResultadoPaginaCMS.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/CargarResultados.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/CargarResultadosGadget.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_partial-views/_list-actions.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_partial-views/_list-votes.cshtml");
                listaVistasResultados.Add($"/Views/CargadorResultados/_partial-views/_results-pagination.cshtml");
                listaVistasResultados.Add($"/Views/CargadorContextoMensajes/CargarContextoMensajes.cshtml");
                listaVistasResultados.Add($"/Views/Shared/_ResultadoMensaje.cshtml");
            }
            return listaVistasResultados;
        }

        public static List<string> ObtenerVistasDeFacetas()
        {
            if (listaVistasFacetas == null)
            {
                listaVistasFacetas = new List<string>();
                listaVistasFacetas.Add($"/Views/CargadorFacetas/_CajaBusqueda.cshtml");
                listaVistasFacetas.Add($"/Views/CargadorFacetas/_CajaBusquedaSimple.cshtml");
                listaVistasFacetas.Add($"/Views/CargadorFacetas/_Faceta.cshtml");
                listaVistasFacetas.Add($"/Views/CargadorFacetas/_ItemFaceta.cshtml");
                listaVistasFacetas.Add($"/Views/CargadorFacetas/CargarFacetas.cshtml");
            }
            return listaVistasFacetas;
        }

        private DataWrapperVistaVirtual VistaVirtualDW
        {
            get
            {
                if (mVistaVirtualDW == null)
                {
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);

                    if (EsAdministracionEcosistema)
                    {
                        mVistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorEcosistemaID(mControladorBase.PersonalizacionEcosistemaID);
                    }
                    else
                    {
                        mVistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave);
                    }

                    vistaVirtualCN.Dispose();
                }
                return mVistaVirtualDW;
            }
        }
    }
}
