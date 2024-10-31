using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class CMSAdminDisenioController : ControllerBaseWeb
    {
        public CMSAdminDisenioController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Miembros

        private ManageXMLViewModel paginaModel = new ManageXMLViewModel();
        string mUrlServicioWebDocumentacion = null;

        #endregion

        /// <summary>
        /// Método principañ
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {           
            if (!ParametrosGeneralesRow.CMSDisponible)
            {
                //Si no tenemos acceso redirigimos a la home
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            CargarModeloPagina("", "", false);

            return View(paginaModel);
        }

        /// <summary>
        /// Método para subir un XML de configuración del CMS de la comunidad
        /// </summary>
        /// <param name="Fichero">Fichero XML con la configuración</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult UploadFile(IFormFile Fichero)
        {
            GuardarLogAuditoria();
            string mensajeOK = "";
            string mensajeKO = "";
            Stopwatch sw = null;

            if (Request.Method.Equals("POST") && Fichero != null && Fichero.ContentType.Equals("text/xml"))
            {
                //Obtengo lo primero el buffer del archivo para no perderlo luego mediante un cierre de Stream.
                byte[] buffer = null;
                using (BinaryReader reader = new BinaryReader(Fichero.OpenReadStream()))
                {
                    buffer = reader.ReadBytes((int)Fichero.Length);
                    reader.Close();
                }

                if (buffer != null)
                {
                    try
                    {
                        MemoryStream ms = new MemoryStream(buffer);
                        StreamReader sr = new StreamReader(ms);
                        string texto = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();

                        //proceso el XML
                        ControladorProyectoCMS controladorEstructuraXML = new ControladorProyectoCMS(ProyectoSeleccionado.Clave, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                        controladorEstructuraXML.ConfigurarCMSComunidadConXML(texto);
                        #region Guardo el XML en el historial
                        GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService);
                        gd.Url = UrlServicioWebDocumentacion;
                        
                        sw = LoggingService.IniciarRelojTelemetria();
                        gd.AdjuntarDocumentoADirectorio(buffer, "ConfiguracionCMS/" + ProyectoSeleccionado.Clave, ProyectoSeleccionado.Clave.ToString(), ".xml");
                        gd.AdjuntarDocumentoADirectorio(buffer, "ConfiguracionCMS/" + ProyectoSeleccionado.Clave, ProyectoSeleccionado.Clave.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".xml");
                        mLoggingService.AgregarEntradaDependencia("Subir archivo de configuración del CMS al gestor documental", false, "UploadFile", sw, true);

                        #endregion

                        mensajeOK = UtilIdiomas.GetText("COMADMINDISENIO", "XMLSUBIDO");
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.AgregarEntradaDependencia("Subir archivo de configuración del CMS al gestor documental", false, "UploadFile", sw, false);
                        mensajeKO = ex.Message;
                        GuardarLogError(ex.ToString());
                    }
                }
                else
                {
                    mensajeKO = UtilIdiomas.GetText("COMADMINDISENIO", "XMLVACIO");
                }
            }
            else
            {
                mensajeKO = UtilIdiomas.GetText("COMADMINDISENIO", "XMLVACIO");
            }
            CargarModeloPagina(mensajeOK, mensajeKO, false);
            return View("Index", paginaModel);
        }

        /// <summary>
        /// Método para ver el historial de versiones
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult History()
        {
            CargarModeloPagina("", "", true);
            return View("Index", paginaModel);
        }

        /// <summary>
        /// Método para descargar el XML dinámico con la configuración del XML del CMS de la comunidad
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult DownloadDynamicFile()
        {
            //obtengo el xml con la configuración almacenada para ese proyecto
            ControladorProyectoCMS controladorEstructuraXML = new ControladorProyectoCMS(ProyectoSeleccionado.Clave, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            XmlDocument xml = controladorEstructuraXML.PintarEstructuraXMLCMS();

            MemoryStream memoryStream = new MemoryStream();
            xml.Save(memoryStream);

            //create new Bite Array
            byte[] biteArray = new byte[memoryStream.Length];
            //Set pointer to the beginning of the stream
            memoryStream.Position = 0;
            //Read the entire stream
            memoryStream.Read(biteArray, 0, (int)memoryStream.Length);

            //Genero el XML dinámico
            return File(biteArray, "application/text", "plantilla_" + ProyectoSeleccionado.NombreCorto + "_CMS.xml");
        }

        /// <summary>
        /// Método para descargar un XML del historial de la configuración del XML del CMS de la comunidad
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult DownloadFile(string fileName)
        {
            GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService);
            gd.Url = UrlServicioWebDocumentacion;
            byte[] buffer = gd.ObtenerDocumentoDeDirectorio("ConfiguracionCMS/" + ProyectoSeleccionado.Clave, fileName, ".xml");
            return File(buffer, "application/text", "plantilla_" + ProyectoSeleccionado.NombreCorto + fileName.Substring(fileName.IndexOf("_") + 1) + "_CMS.xml");
        }

        /// <summary>
        /// Carga el modelo para la vista de la página
        /// </summary>
        /// <param name="pMensajeOK">Mensaje del OK</param>
        /// <param name="pMensajeKO">Mensaje del KO</param>
        /// <param name="pCargarHistorial">True si se quiere cargar el historial de versines de los XML del CMS</param>
        private void CargarModeloPagina(string pMensajeOK, string pMensajeKO, bool pCargarHistorial)
        {
            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "configuracion administrar-disenio edicion no-max-width-container ";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Cookies;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONDESARROLLADORES", "COOKIES");

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
           
            paginaModel.UrlActionFile = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARDISENIOCMS") + "/UploadFile";
            paginaModel.UrlActionHistory = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARDISENIOCMS") + "/History";
            paginaModel.UrlActionDownloadDynamicFile = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARDISENIOCMS") + "/DownloadDynamicFile";
            paginaModel.UrlActionDownloadFile = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARDISENIOCMS") + "/DownloadFile";
            paginaModel.OKMessage = pMensajeOK;
            paginaModel.KOMessage = pMensajeKO;
            paginaModel.ListXMLVersion = new SortedDictionary<DateTime, string>();
            if (pCargarHistorial)
            { 
                GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService);
                gd.Url = UrlServicioWebDocumentacion;

                string[] listado = gd.ObtenerListadoDeDocumentosDeDirectorio("ConfiguracionCMS/" + ProyectoSeleccionado.Clave);

                foreach (string nombreFichero in listado)
                {
                    if (nombreFichero.Contains("_"))
                    {
                        int indiceBarraBaja = nombreFichero.IndexOf("_") + 1;
                        int indicePunto = nombreFichero.LastIndexOf(".");

                        string fecha = nombreFichero.Substring(indiceBarraBaja, indicePunto - indiceBarraBaja);
                        int año = int.Parse(fecha.Substring(0, 4));
                        int mes = int.Parse(fecha.Substring(4, 2));
                        int dia = int.Parse(fecha.Substring(6, 2));
                        int hora = int.Parse(fecha.Substring(9, 2));
                        int min = int.Parse(fecha.Substring(11, 2));
                        int seg = int.Parse(fecha.Substring(13, 2));
                        DateTime fechaFichero = new DateTime(año, mes, dia, hora, min, seg, 0);
                        paginaModel.ListXMLVersion.Add(fechaFichero, nombreFichero.Substring(0, nombreFichero.IndexOf(".")));
                    }
                }
            }
        }
    }
}
