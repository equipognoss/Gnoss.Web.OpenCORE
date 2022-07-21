using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
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
using Gnoss.Web.Services.VirtualPathProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    public class AdministrarVistasController : ControllerBaseWeb
    {
        private string pathResourceDefault;
        private string pathListResourcesDefault;
        private string pathGroupComponentsDefault;

        private const string comandoTraduccion = "Html.Translate(\"";
        private ManageViewsViewModel paginaModel = new ManageViewsViewModel();
        private DataWrapperVistaVirtual mVistaVirtualDW;

        private static List<string> listaVistasResultados;
        private static List<string> listaVistasFacetas;
        private string mViews;
        private const string VIEWS_DIRECTORY = "Views";

        public AdministrarVistasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
            mViews = "Views";
            if (!BaseURL.Contains("depuracion.net"))
            {
                mViews = "ViewsAdministracion";
            }
            pathResourceDefault = $"/{VIEWS_DIRECTORY}/CMSPagina/ListadoRecursos/Vistas/";
            pathListResourcesDefault = $"/{VIEWS_DIRECTORY}/CMSPagina/ListadoRecursos/";
            pathGroupComponentsDefault = $"/{VIEWS_DIRECTORY}/CMSPagina/GrupoComponentes/";
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionVistasPermitido" })]

        public ActionResult Index()
        {
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
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
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
                string Pagina = PaginasPersonalizables;
                bool esRdfType = false;
                if (string.IsNullOrEmpty(Pagina))
                {
                    Pagina = FormulariosSemanticos;
                    esRdfType = true;
                }
                if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Download)
                {
                    string fileName = Pagina;

                    string lineaSeguridad = "@*[security|||" + Pagina.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower() + "|||" + ProyectoSeleccionado.NombreCorto.ToLower() + "]*@";

                    string htmlPagina = DescargarPagina(Pagina, false, esRdfType);

                    if (!string.IsNullOrEmpty(htmlPagina))
                    {
                        htmlPagina = lineaSeguridad + "\n" + htmlPagina;

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
                    string fileName = Pagina;
                    if (mConfigService.EstaDesplegadoEnDocker())
                    {
                        Pagina = Pagina.Replace("Views", "ViewsAdministracion");
                    }
                    string htmlPagina = DescargarPagina(Pagina, true, esRdfType);
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

                        string lineaSeguridad = "@*[security|||" + Pagina.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower() + "|||" + ProyectoSeleccionado.NombreCorto.ToLower() + "]*@";

                        if (security.StartsWith("@*[security|||") && security.EndsWith("]*@"))
                        {
                            string seguridadNombrePagina = security.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            string seguridadNombreProy = security.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[2].Replace("]*@", "");

                            if (seguridadNombrePagina == Pagina.Replace($"/{VIEWS_DIRECTORY}/", "").ToLower() && seguridadNombreProy == ProyectoSeleccionado.NombreCorto.ToLower() && lineaSeguridad == security)
                            {
                                string errorCompilando = CompilarVista(texto);
                                if (string.IsNullOrEmpty(errorCompilando))
                                {
                                    if (!GuardarPagina(Pagina, texto, esRdfType))
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
                                error = "La linea de seguridad no concuerda con lo esperado,<br />por la seguirdad de la plataforma, debes añadir la siguiente linea al inicio de la vista:<br />" + lineaSeguridad;
                            }
                        }
                        else
                        {
                            error = "Por la seguirdad de la plataforma, debes añadir la siguiente linea al inicio de la vista:<br />" + lineaSeguridad;
                        }
                    }
                    else
                    {
                        error = "Tienes que seleccionar un fichero para subir";
                    }
                }
                else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Delete)
                {
                    if (!EliminarPagina(Pagina, esRdfType))
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
        /// <param name="PaginaResultados">Nombre de la página</param>
        /// <param name="Fichero">Fichero con la vista</param>
        /// <param name="Accion">Acción a relizar</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Resultados(string PaginaResultados, IFormFile Fichero, ManageViewsViewModel.Action Accion)
        {
            CargarDatos();
            string error = "";

            if (string.IsNullOrEmpty(PaginaResultados))
            {
                error = "Tienes que seleccionar una opción en el combo";
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Download)
            {
                string fileName = PaginaResultados;
                string htmlPagina = DescargarPagina(PaginaResultados, false, false);
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
                string fileName = PaginaResultados;
                if (mConfigService.EstaDesplegadoEnDocker())
                {
                    PaginaResultados = PaginaResultados.Replace("Views", "ViewsAdministracion");
                }
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
                    string texto = sr.ReadToEnd();
                    sr.Close();

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
                    error = "Tienes que seleccionar un fichero para subir";
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
        /// <param name="PaginaFacetas">Nombre de la página</param>
        /// <param name="Fichero">Fichero con la vista</param>
        /// <param name="Accion">Acción a relizar</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Facetas(string PaginaFacetas, IFormFile Fichero, ManageViewsViewModel.Action Accion)
        {
            CargarDatos();
            string error = "";
            if (string.IsNullOrEmpty(PaginaFacetas))
            {
                error = "Tienes que seleccionar una opción en el combo";
            }
            else if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Download)
            {
                string fileName = PaginaFacetas;
                string htmlPagina = DescargarPagina(PaginaFacetas, false, false);
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
                string fileName = PaginaFacetas;
                if (mConfigService.EstaDesplegadoEnDocker())
                {
                    PaginaFacetas = PaginaFacetas.Replace("Views", "ViewsAdministracion");
                }
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
                    string texto = sr.ReadToEnd();
                    sr.Close();

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
                    error = "Tienes que seleccionar un fichero para subir";
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
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionVistasPermitido" })]
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
                if (mConfigService.EstaDesplegadoEnDocker())
                {
                    ComponentePersonalizable = ComponentePersonalizable.Replace("Views", "ViewsAdministracion");
                }
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
                                ComponentePersonalizable = ComponentePersonalizable + "_" + Guid.NewGuid() + ".cshtml"; ;
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
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult CMSExtra(string ComponentePersonalizable, Guid idPersonalizacion, bool ResourcesExtra, bool Identities, bool IdentitiesExtra, ManageViewsViewModel.Action Accion)
        {
            CargarDatos();
            string error = "";
            if (Request.Method.Equals("POST") && Accion == ManageViewsViewModel.Action.Save)
            {
                List<VistaVirtualCMS> vistaVirtualAnterior = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.PersonalizacionComponenteID.Equals(idPersonalizacion)).ToList();
                if (vistaVirtualAnterior.Count == 1)
                {
                    string HTML = vistaVirtualAnterior[0].HTML;
                    string Nombre = vistaVirtualAnterior[0].Nombre;
                    string DatosExtra = "";
                    if (ResourcesExtra)
                    {
                        DatosExtra += DatosExtraVistas.DatosExtraRecursos.ToString();
                    }
                    DatosExtra += "&";
                    if (Identities)
                    {
                        DatosExtra += DatosExtraVistas.Identidades.ToString();
                    }
                    DatosExtra += "&";
                    if (IdentitiesExtra)
                    {
                        DatosExtra += DatosExtraVistas.DatosExtraIdentidades.ToString();
                    }
                    if (!GuardarComponenteCMS(ComponentePersonalizable, HTML, Nombre, DatosExtra, idPersonalizacion))
                    {
                        error = "Error al guardar los datos, intentalo de nuevo";
                    }
                }
                else
                {
                    error = "El componente '" + ComponentePersonalizable + "' con id='" + idPersonalizacion + "' no existe";
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
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionVistasPermitido" })]
        public ActionResult InvalidarVistas()
        {
            LimpiarCacheVistaEntera();

            CargarDatos();
            ViewBag.Personalizacion = string.Empty;
            ViewBag.PersonalizacionLayout = "";
            paginaModel.OKMessage = "Caches invalidadas correctamente";
            return GnossResultHtml("Index", paginaModel);
        }

        private string CompilarVista(string pHtml)
        {
            Guid testID = Guid.NewGuid();
            string vistaTemporal = $"/{VIEWS_DIRECTORY}/TESTvistaTEST/" + testID + "$$$" + testID + ".cshtml";

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

                    ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paramGeneralCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
                }
                catch (Exception ex)
                {
                    //Error al añadir las nuevas traducciones
                    errorCompilando = ex.Message;
                }
            }

            /*TODO Javier
            if (MyVirtualPathProvider.ListaHtmlsTemporales.ContainsKey(testID))
            {
                MyVirtualPathProvider.ListaHtmlsTemporales.Remove(testID);
            }*/

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
            // ParametroGeneralDS textosPersonalizadosDS = null;
            GestorParametroGeneral gestorParametroGeneral = new GestorParametroGeneral();
            ParametroGeneralGBD parametroGeneralGBD = new ParametroGeneralGBD(mEntityContext);
            //GestorParametroAplicacionController gestorController = new GestorParametroAplicacionController();
            ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            //List<TextosPersonalizadosPersonalizacion> listaTextos = new List<TextosPersonalizadosPersonalizacion>();

            if (EsAdministracionEcosistema)
            {
                personalizacionID = mControladorBase.PersonalizacionEcosistemaID;
                gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = parametroGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(personalizacionID);
                //listaTextos = gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion;
                //listaTextos = parametroGeneralGBD.ObtenerTextosPersonalizadosPersonalizacion(personalizacionID);
            }
            else
            {
                VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave, mControladorBase.PersonalizacionEcosistemaID, mControladorBase.ComunidadExcluidaPersonalizacionEcosistema);
                if (vistaVirtualDW.ListaVistaVirtualProyecto.Count > 0)
                {
                    personalizacionID = ((VistaVirtualProyecto)vistaVirtualDW.ListaVistaVirtualProyecto.Where(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault()).PersonalizacionID;
                    gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = parametroGeneralCN.ObtenerTextosPersonalizacionProyecto(ProyectoSeleccionado.Clave);
                    //listaTextos = gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion;
                    //listaTextos = parametroGeneralGBD.ObtenerTextosPersonalizadosPersonalizacion(ProyectoSeleccionado.Clave);
                }
            }

            if (personalizacionID != Guid.Empty)
            {
                int indice = pHtml.IndexOf(comandoTraduccion, 0);
                int aux = 0;
                string texto = string.Empty;
                string idioma = "es";

                //ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService);
                //ParametroGeneralDS parametroGeneralDS = parametroGeneralCN.ObtenerParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);

                while (indice > -1)
                {
                    aux = indice;
                    indice = pHtml.IndexOf("\")", aux);
                    texto = pHtml.Substring(aux, indice - aux);
                    texto = texto.Replace(comandoTraduccion, "");

                    int indiceTexto = 0;
                    Boolean encontrado = false;
                    while (indiceTexto > -1 && !encontrado)
                    {
                        indiceTexto++;
                        indiceTexto = texto.IndexOf("\"", indiceTexto);
                        if (indiceTexto > -1)
                        {
                            if (texto[indiceTexto - 1] != Path.DirectorySeparatorChar)
                            {
                                encontrado = true;
                            }
                        }
                    }
                    if (indiceTexto > -1)
                    {
                        //idioma = texto.Substring(indiceTexto).Replace(",", "").Replace("\"", "").Trim();
                        texto = texto.Substring(0, indiceTexto);
                    }
                    if (texto.Length > 100)
                    {
                        return "ID del texto demasiado largo. En la sección de traducciones (*aquí su comunidad*/administrar-traducciones) cree una traducción con un ID más corto que podrá colocar en la vista, este será remplazado por el texto adecuado al idioma";
                    }
                    //ParametroGeneralDS.TextosPersonalizadosPersonalizacionRow traduccion = gestorParametroGeneral.TextosPersonalizadosPersonalizacion.FindByPersonalizacionIDTextoIDLanguage(personalizacionID, texto, idioma);
                    //TextosPersonalizadosPersonalizacion traduccion = gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Find(textoPersonalizado=>textoPersonalizado.PersonalizacionID.Equals(personalizacionID) &&textoPersonalizado.Texto.Equals(texto) &&textoPersonalizado.Language.Equals(idioma));
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
                            //Recorremos la la lista del gestor por si esta introducido el mismo valor que nos viene.
                            //TextosPersonalizadosPersonalizacion filaTraduccion = parametroGeneralGBD.ObtenerTextosPersonalizadosPersonalizacion(personalizacionID).Find(textoPersonalizado => textoPersonalizado.Texto.Equals(texto) && textoPersonalizado.Language.Equals(idioma));
                            //filaTraduccion = new TextosPersonalizadosPersonalizacion(personalizacionID, texto, idioma, texto);
                            parametroGeneralGBD.AddTextosPersonalizadosPersonalizacion(filaTraduccion);
                            gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Add(filaTraduccion);
                            //gestorParametroGeneral.TextosPersonalizadosPersonalizacion.AddTextosPersonalizadosPersonalizacionRow(filaTraduccion);
                            //gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Add(filaTraduccion);           
                            //mEntityContext.TextosPersonalizadosPersonalizacion.Add(filaTraduccion);
                        }

                        else
                        {

                            List<TextosPersonalizadosPersonalizacion> listaTextosPersonalizadosPersonalizacionEcosistema = parametroGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
                            //gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = parametroGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
                            traduccion = listaTextosPersonalizadosPersonalizacionEcosistema.Find(textoPersonalizado => textoPersonalizado.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID) && textoPersonalizado.TextoID.Equals(texto, StringComparison.CurrentCultureIgnoreCase) && textoPersonalizado.Language.Equals(idioma));
                            if (traduccion == null)
                            {
                                TextosPersonalizadosPersonalizacion filaTraduccion = new TextosPersonalizadosPersonalizacion();
                                filaTraduccion.PersonalizacionID = personalizacionID;
                                filaTraduccion.TextoID = texto;
                                filaTraduccion.Texto = texto;
                                filaTraduccion.Language = idioma;
                                //Recorremos la la lista del gestor por si esta introducido el mismo valor que nos viene.
                                //TextosPersonalizadosPersonalizacion filaTraduccion = parametroGeneralGBD.ObtenerTextosPersonalizadosPersonalizacion(personalizacionID).Find(textoPersonalizado => textoPersonalizado.Texto.Equals(texto) && textoPersonalizado.Language.Equals(idioma));
                                //filaTraduccion = new TextosPersonalizadosPersonalizacion(personalizacionID, texto, idioma, texto);
                                parametroGeneralGBD.AddTextosPersonalizadosPersonalizacion(filaTraduccion);
                                gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Add(filaTraduccion);
                                //gestorParametroGeneral.TextosPersonalizadosPersonalizacion.AddTextosPersonalizadosPersonalizacionRow(filaTraduccion);
                                //gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Add(filaTraduccion);           
                                //mEntityContext.TextosPersonalizadosPersonalizacion.Add(filaTraduccion);
                            }
                        }
                    }
                    indice = pHtml.IndexOf(comandoTraduccion, indice); //-----------
                }
                ParametroGeneralCN paramGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                parametroGeneralGBD.saveChanges();
                //mEntityContext.SaveChanges();
                //paramGeneralCN.ActualizarParametrosGenerales(gestorParametroGeneral, false);
            }
            else
            {
                return "Ocurrió un error al obtener el PersonalizacionID de su comunidad";
            }
            return null;
        }

        private void CargarDatos()
        {
            mVistaVirtualDW = null;
            CargarVistaVirtual(VistaVirtualDW);
            CargarVistaRecursos(VistaVirtualDW);
            CargarVistaResultados(VistaVirtualDW);
            CargarVistaFacetas(VistaVirtualDW);
            CargarVistaCMS(VistaVirtualDW);

            string urlPagina = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/";

            if (EsAdministracionEcosistema)
            {
                urlPagina = UtilIdiomas.GetText("URLSEM", "ADMINISTRARVISTASECOSISTEMA");
            }
            else
            {
                urlPagina = UtilIdiomas.GetText("URLSEM", "ADMINISTRARVISTAS");
            }

            paginaModel.UrlActionWeb = urlPagina + "/web";
            paginaModel.UrlActionResults = urlPagina + "/results";
            paginaModel.UrlActionFacets = urlPagina + "/facets";
            paginaModel.UrlActionCMS = urlPagina + "/cms";
            paginaModel.UrlActionCMSExtra = urlPagina + "/cms-extra";
            paginaModel.UrlActionInvalidateViews = urlPagina + "/invalidateviews";
        }

        private void CargarVistaVirtual(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            List<string> listaVistas = ObtenerFicherosDeDirectorio(Path.Combine(mEnv.ContentRootPath, mViews));

            paginaModel.ListEditedViews = new List<string>();
            paginaModel.ListEditedViews.Insert(0, "");
            paginaModel.ListOriginalViews = new List<string>();
            paginaModel.ListOriginalViews.Insert(0, "");
            //Insertamos las vistas que no corresponden con componentes del CMS
            foreach (string nombreVista in listaVistas)
            {
                bool esVistaEdicionRecurso = nombreVista.StartsWith($"/{VIEWS_DIRECTORY}/EditarRecurso/_");
                bool esVistaAdministracion = nombreVista.StartsWith($"/{VIEWS_DIRECTORY}/Administracion");
                bool esVistaCMS = nombreVista.StartsWith($"/{VIEWS_DIRECTORY}/CMSPagina") && nombreVista.LongCount(letra => letra.ToString() == "/") > 3;

                if (!esVistaEdicionRecurso && !esVistaAdministracion && !esVistaCMS)
                {
                    if (pVistaVirtualDW.ListaVistaVirtual.Any(item => item.TipoPagina.Equals(nombreVista)))
                    {
                        paginaModel.ListEditedViews.Add(nombreVista);
                    }
                    paginaModel.ListOriginalViews.Add(nombreVista);
                }
            }

            foreach (VistaVirtual filaVistaVirtual in pVistaVirtualDW.ListaVistaVirtual)
            {
                if (!paginaModel.ListEditedViews.Contains(filaVistaVirtual.TipoPagina) && !filaVistaVirtual.TipoPagina.StartsWith($"/Views/CargadorFacetas") && !filaVistaVirtual.TipoPagina.StartsWith($"/Views/CargadorResultados"))
                {
                    paginaModel.ListEditedViews.Add(filaVistaVirtual.TipoPagina);
                }
            }
        }

        private void CargarVistaRecursos(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            if (!EsAdministracionEcosistema)
            {
                List<string> listaFormularios = ObtenerFormulariosSemanticos();
                paginaModel.ListEditedFormsViews = new List<string>();
                paginaModel.ListEditedFormsViews.Insert(0, "");
                paginaModel.ListOriginalFormsViews = new List<string>();
                paginaModel.ListOriginalFormsViews.Insert(0, "");

                //Insertamos las vistas que no corresponden con componentes del CMS
                foreach (string nombreVista in listaFormularios)
                {
                    if (pVistaVirtualDW.ListaVistaVirtualRecursos.Any(item => item.RdfType.Equals(nombreVista)))
                    {
                        paginaModel.ListEditedFormsViews.Add(nombreVista);
                    }
                    paginaModel.ListOriginalFormsViews.Add(nombreVista);
                }
            }
        }

        private void CargarVistaResultados(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            List<string> listaVistasResultados = ObtenerVistasDeResultados();
            paginaModel.ListEditedResultsServiceViews = new List<string>();
            paginaModel.ListEditedResultsServiceViews.Insert(0, "");
            paginaModel.ListOriginalResultsServiceViews = new List<string>();
            paginaModel.ListOriginalResultsServiceViews.Insert(0, "");

            foreach (string nombreVista in listaVistasResultados)
            {
                if (pVistaVirtualDW.ListaVistaVirtual.Any(item => item.TipoPagina.Equals(nombreVista)))
                {
                    paginaModel.ListEditedResultsServiceViews.Add(nombreVista);
                }
                paginaModel.ListOriginalResultsServiceViews.Add(nombreVista);
            }

        }

        private void CargarVistaFacetas(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            List<string> listaVistasFacetas = ObtenerVistasDeFacetas();
            paginaModel.ListEditedFacetedServiceViews = new List<string>();
            paginaModel.ListEditedFacetedServiceViews.Insert(0, "");
            paginaModel.ListOriginalFacetedServiceViews = new List<string>();
            paginaModel.ListOriginalFacetedServiceViews.Insert(0, "");

            foreach (string nombreVista in listaVistasFacetas)
            {
                if (pVistaVirtualDW.ListaVistaVirtual.Any(item => item.TipoPagina.Equals(nombreVista)))
                {
                    paginaModel.ListEditedFacetedServiceViews.Add(nombreVista);
                }
                paginaModel.ListOriginalFacetedServiceViews.Add(nombreVista);
            }
        }

        private void CargarVistaCMS(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            //Almacenamos en listaComponentesDisponibles los componentes disponibles de la comunidad que tienen vistas base
            List<TipoComponenteCMS> listaComponentesDisponibles = new List<TipoComponenteCMS>();
            foreach (TipoComponenteCMS tipoComponenteCMS in UtilComponentes.ListaComponentesPublicos)
            {
                listaComponentesDisponibles.Add(tipoComponenteCMS);
            }
            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
            cmsCN.Dispose();
            foreach (TipoComponenteCMS tipoComponenteCMS in gestorCMS.ListaComponentesPrivadosProyecto)
            {
                listaComponentesDisponibles.Add(tipoComponenteCMS);
            }
            List<string> listaVistas = ObtenerFicherosDeDirectorio(Path.Combine(mEnv.ContentRootPath, $"{mViews}", "CMSPagina"));
            mLoggingService.GuardarLogError("La ruta obtenida es: " + Path.Combine(mEnv.ContentRootPath, $"{mViews}", "CMSPagina"));
            List<TipoComponenteCMS> listaComponentesDisponiblesAux = new List<TipoComponenteCMS>(listaComponentesDisponibles);
            foreach (TipoComponenteCMS componente in listaComponentesDisponiblesAux)
            {
                if (!listaVistas.Contains($"/{mViews}/CMSPagina/" + componente.ToString() + "/_" + componente.ToString() + ".cshtml"))
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
                    string vistaComponente = $"/{VIEWS_DIRECTORY}/CMSPagina/" + componente.ToString() + "/_" + componente.ToString() + ".cshtml";

                    ManageViewsViewModel.CMSComponentViewModel componenteActual = new ManageViewsViewModel.CMSComponentViewModel();
                    componenteActual.PathName = vistaComponente;
                    componenteActual.Name = UtilIdiomas.GetText("COMADMINCMS", "COMPONENTE_" + componente.ToString().ToUpper());
                    componenteActual.CustomizationName = new Dictionary<Guid, string>();

                    foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.Equals(vistaComponente)))
                    {
                        componenteActual.CustomizationName.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, filaVistaVirtualCMS.Nombre);
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
            foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
            {
                //Presentaciones genéricas
                ManageViewsViewModel.CMSResourceViewModel presentacionRecursoActual = new ManageViewsViewModel.CMSResourceViewModel();
                presentacionRecursoActual.PathName = vistaComponenteRecursos + "_" + tipoPresentacion.ToString() + ".cshtml";
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
                    presentacionRecursoActual.Name = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACION_" + tipoPresentacion.ToString().ToUpper());
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
                presentacionListadoRecursoActual.PathName = vistaComponenteListadoRecursos + "_" + tipoPresentacion.ToString() + ".cshtml";
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
                    presentacionListadoRecursoActual.Name = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONLISTADO_" + tipoPresentacion.ToString().ToUpper());
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
                presentacionGrupoComponentesActual.PathName = vistaComponenteGrupoComponentes + "_" + tipoPresentacion.ToString() + ".cshtml";
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
                    presentacionGrupoComponentesActual.Name = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONGRUPO_" + tipoPresentacion.ToString().ToUpper());
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

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

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
                            nombre = "/Recursos/" + pagina + ".cshtml";
                        }

                        HttpResponseMessage resultado = InformarCambioAdministracionVistas("Vistas", nombre, pHtml);

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    LimpiarCacheVista(pagina, pEsRdfType);

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch (Exception ex)
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
                mLoggingService.GuardarLogError(ex, "Error en el guardado de vistas personalizadas. pagina:" + pagina);
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

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

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
                        pagina = "CMS/" + pagina.Replace($"/{VIEWS_DIRECTORY}/CMSPagina", "").TrimStart('/').Replace(".cshtml", "_" + modelo.Nombre + "$$$" + modelo.PersonalizacionComponenteID + ".cshtml");
                        modelo.Ruta = pagina;

                        HttpResponseMessage resultado = InformarCambioAdministracion("VistasCMS", JsonConvert.SerializeObject(modelo));
                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    LimpiarCacheComponenteCMS(pPersonalizacionComponenteID);

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch (Exception ex)
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
                mLoggingService.GuardarLogError(ex, "Error en el guardado de vistas personalizadas. pagina:" + pagina);
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

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    Guid personalizacionID = Guid.Empty;

                    if (pEsRdfType)
                    {
                        personalizacionID = VistaVirtualDW.ListaVistaVirtualRecursos.FirstOrDefault().PersonalizacionID;
                    }
                    else
                    {
                        personalizacionID = VistaVirtualDW.ListaVistaVirtual.FirstOrDefault().PersonalizacionID;
                    }

                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    vistaVirtualCN.EliminarHtmlParaVistaDeProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, personalizacionID, pagina, pEsRdfType);

                    if (iniciado)
                    {
                        string nombre = pagina;

                        if (pEsRdfType)
                        {
                            nombre = "/Recursos/" + pagina + ".cshtml";
                        }

                        HttpResponseMessage resultado = InformarCambioAdministracion("Vistas", JsonConvert.SerializeObject(new KeyValuePair<string, string>(nombre, "")));
                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    LimpiarCacheVista(pagina, pEsRdfType);

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch (Exception ex)
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
                mLoggingService.GuardarLogError(ex, "Error en el eliminado de vistas personalizadas. pagina:" + pagina);
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

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    vistaVirtualCN.EliminarHtmlParaVistaDeComponenteCMSdeProyecto(VistaVirtualDW.ListaVistaVirtualCMS[0].PersonalizacionID, pPersonalizacionComponenteID, pVista);

                    if (iniciado)
                    {
                        string nombre = pVista;

                        HttpResponseMessage resultado = InformarCambioAdministracion("VistasCMS", JsonConvert.SerializeObject(new KeyValuePair<string, string>(nombre, "")));

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    LimpiarCacheComponenteCMS(pPersonalizacionComponenteID);

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch (Exception ex)
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
                mLoggingService.GuardarLogError(ex, "Error en el eliminado de vistas personalizadas de componentes CMS. pagina:" + pVista);
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

                //VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService);
                //return vistaVirtualCN.ObtenerHtmlParaVistaDeProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, pagina, pEsRdfType);
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
            VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (EsAdministracionEcosistema)
            {
                vistaVirtualCL.InvalidarVistasVirtualesEcosistema();
            }
            else
            {
                vistaVirtualCL.InvalidarVistasVirtuales(ProyectoSeleccionado.Clave);
            }
            vistaVirtualCL.Dispose();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
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

            if (pEsRdfType)
            {
                pVista = $"/{VIEWS_DIRECTORY}/FichaRecurso/{pVista}.cshtml";
            }

            ControladorAdministrarVistas.LimpiarCacheVistasRedis(EsAdministracionEcosistema, "", UrlIntragnoss, pVista);
        }

        private void LimpiarCacheComponenteCMS(Guid pPersonalizacionComponenteID)
        {
            VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (EsAdministracionEcosistema)
            {
                vistaVirtualCL.InvalidarVistasVirtualesEcosistema();
            }
            else
            {
                vistaVirtualCL.InvalidarVistasVirtuales(ProyectoSeleccionado.Clave);
            }
            vistaVirtualCL.Dispose();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
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

            ControladorAdministrarVistas.LimpiarCacheVistasRedis(EsAdministracionEcosistema, "", UrlIntragnoss);
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

                        pagina = pagina.Substring(indiceViews).Replace("/ViewsAdministracion/", "/Views/");
                        //pagina = pagina.Substring(0, pagina.LastIndexOf('.'));

                        //int indiceUltimaContrabarra = fichero.LastIndexOf('\\');

                        //fichero = fichero.Remove(indiceUltimaContrabarra, 1);
                        //fichero = fichero.Insert(indiceUltimaContrabarra, "_");
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
            using (StreamReader sr = new StreamReader(mEnv.ContentRootPath + pRuta))
            {
                fichero = sr.ReadToEnd();
            }
            return fichero;
        }

        private List<string> ObtenerFormulariosSemanticos()
        {
            List<string> listaFormuariosSemanticos = new List<string>();

            FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<OntologiaProyecto> listaOntologias = facetaCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, false, false);
            Dictionary<string, List<string>> ontologiasProyecto = FacetadoAD.ObtenerInformacionOntologias(listaOntologias);

            listaFormuariosSemanticos = ontologiasProyecto.Keys.ToList();

            return listaFormuariosSemanticos;
        }

        public List<string> ObtenerVistasDeResultados()
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
                listaVistasResultados.Add($"/Views/CargadorContextoMensajes/CargarContextoMensajes.cshtml");
                listaVistasResultados.Add($"/Views/Shared/_ResultadoMensaje.cshtml");
            }
            return listaVistasResultados;
        }

        public List<string> ObtenerVistasDeFacetas()
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
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

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
