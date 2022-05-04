using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class AdministrarDisenioController : ControllerBaseWeb
    {
        public AdministrarDisenioController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        /// <summary>
        /// Método principal
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View(CargarModeloPagina());
        }

        /// <summary>
        /// Método para subir un XML de configuración de la comunidad
        /// </summary>
        /// <param name="Fichero">Fichero XML con la configuración</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult UploadFile(IFormFile Fichero)
        {
            Stopwatch sw = null;

            if (Fichero != null && Fichero.ContentType.Equals("text/xml"))
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
                        ControladorProyecto controlador = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                        controlador.ConfigurarComunidadConXML(texto, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);

                        VersionarCaches(controlador);

                        #region Guardo el XML en el historial
                        GestionDocumental gd = new GestionDocumental(mLoggingService);
                        gd.Url = UrlServicioWebDocumentacion;

                        sw = LoggingService.IniciarRelojTelemetria();
                        gd.AdjuntarDocumentoADirectorio(buffer, "Configuracion/" + ProyectoSeleccionado.Clave, ProyectoSeleccionado.Clave.ToString(), ".xml");
                        gd.AdjuntarDocumentoADirectorio(buffer, "Configuracion/" + ProyectoSeleccionado.Clave, ProyectoSeleccionado.Clave.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".xml");
                        mLoggingService.AgregarEntradaDependencia("Subir archivo de configuración de la comunidad al gestor documental", false, "UploadFile", sw, true);
                        #endregion

                        return GnossResultOK(UtilIdiomas.GetText("COMADMINDISENIO", "XMLSUBIDO"));
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.AgregarEntradaDependencia("Subir archivo de configuración de la comunidad al gestor documental", false, "UploadFile", sw, false);
                        GuardarLogError(ex.ToString());

                        return GnossResultERROR(ex.Message);
                    }
                }
            }

            return GnossResultERROR(UtilIdiomas.GetText("COMADMINDISENIO", "XMLVACIO"));
        }

        /// <summary>
        /// Método para ver el historial de versiones
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult History()
        {
            return PartialView("_Historial", CargarHistorial());
        }

        /// <summary>
        /// Método para descargar el XML dinámico con la configuración del XML de la comunidad
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult DownloadDynamicFile()
        {
            //obtengo el xml con la configuración almacenada para ese proyecto
            ControladorProyectoPintarEstructuraXML controladorEstructuraXML = new ControladorProyectoPintarEstructuraXML(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.FilaProyecto.ProyectoID, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);
            XmlDocument xml = controladorEstructuraXML.PintarEstructuraXML();

            MemoryStream memoryStream = new MemoryStream();
            xml.Save(memoryStream);

            //create new Bite Array
            byte[] biteArray = new byte[memoryStream.Length];
            //Set pointer to the beginning of the stream
            memoryStream.Position = 0;
            //Read the entire stream
            memoryStream.Read(biteArray, 0, (int)memoryStream.Length);

            //Genero el XML dinámico
            return File(biteArray, "text/xml", "plantilla_" + ProyectoSeleccionado.NombreCorto + ".xml");
        }

        /// <summary>
        /// Método para descargar un XML del historial de la configuración del XML de la comunidad
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult DownloadFile(string fileName)
        {
            GestionDocumental gd = new GestionDocumental(mLoggingService);
            gd.Url = UrlServicioWebDocumentacion;
            byte[] buffer = gd.ObtenerDocumentoDeDirectorio("Configuracion/" + ProyectoSeleccionado.Clave, fileName, ".xml");
            return File(buffer, "text/xml", "plantilla_" + ProyectoSeleccionado.NombreCorto + fileName.Substring(fileName.IndexOf("_") + 1) + ".xml");
        }

        /// <summary>
        /// Carga el modelo para la vista de la página
        /// </summary>
        private ManageXMLViewModel CargarModeloPagina()
        {
            ManageXMLViewModel paginaModel = new ManageXMLViewModel();

            paginaModel.UrlActionFile = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARDISENIO") + "/UploadFile";
            paginaModel.UrlActionHistory = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARDISENIO") + "/History";
            paginaModel.UrlActionDownloadDynamicFile = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARDISENIO") + "/DownloadDynamicFile";

            return paginaModel;
        }

        private List<HistoryFile> CargarHistorial()
        {
            string urlActionDownloadFile = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARDISENIO") + "/DownloadFile";

            List<HistoryFile> listXMLVersion = new List<HistoryFile>();

            GestionDocumental gd = new GestionDocumental(mLoggingService);
            gd.Url = UrlServicioWebDocumentacion;

            string[] listado = gd.ObtenerListadoDeDocumentosDeDirectorio("Configuracion/" + ProyectoSeleccionado.Clave);

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

                    HistoryFile fichero = new HistoryFile();
                    fichero.Fecha = fechaFichero;
                    fichero.Name = nombreFichero;
                    fichero.UrlDownload = urlActionDownloadFile + "/" + nombreFichero.Substring(0, nombreFichero.IndexOf("."));
                    listXMLVersion.Add(fichero);
                }
            }

            return listXMLVersion;
        }

        /// <summary>
        /// Ivalida y versiona las cachés de las rutas del proyecto
        /// </summary>
        /// <param name="controlador"></param>
        private void VersionarCaches(ControladorProyecto controlador)
        {
            mGnossCache.RecalcularRutasProyecto(ProyectoSeleccionado.Clave);

            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            List<string> listaRutasPestanyasInvalidarDeCache = null;
            if (controlador.RutasPestanyasInvalidar.Count > 0)
            {
                listaRutasPestanyasInvalidarDeCache = (List<string>)proyectoCL.ObtenerObjetoDeCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_INVALIDAR + ProyectoSeleccionado.Clave, true);

                if (listaRutasPestanyasInvalidarDeCache != null)
                {
                    foreach (string rutaInvalidar in controlador.RutasPestanyasInvalidar)
                    {
                        if (!listaRutasPestanyasInvalidarDeCache.Contains(rutaInvalidar))
                        {
                            listaRutasPestanyasInvalidarDeCache.Add(rutaInvalidar);
                        }
                    }
                }
                else
                {
                    listaRutasPestanyasInvalidarDeCache = new List<string>();
                    listaRutasPestanyasInvalidarDeCache.AddRange(controlador.RutasPestanyasInvalidar);
                }
            }

            List<string> listaRutasPestanyasRegistrarDeCache = null;
            if (controlador.RutasPestanyasRegistrar.Count > 0)
            {
                listaRutasPestanyasRegistrarDeCache = (List<string>)proyectoCL.ObtenerObjetoDeCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_REGISTRAR + ProyectoSeleccionado.Clave, true);

                if (listaRutasPestanyasRegistrarDeCache != null)
                {
                    foreach (string rutaRegistrar in controlador.RutasPestanyasRegistrar)
                    {
                        if (!listaRutasPestanyasRegistrarDeCache.Contains(rutaRegistrar))
                        {
                            listaRutasPestanyasRegistrarDeCache.Add(rutaRegistrar);
                        }
                    }
                }
                else
                {
                    listaRutasPestanyasRegistrarDeCache = new List<string>();
                    listaRutasPestanyasRegistrarDeCache.AddRange(controlador.RutasPestanyasRegistrar);
                }
            }

            //agregar a caché las rutas de las pestañas a invalidar y a registrar con una duración de 18 horas -> 64800 segundos
            if (listaRutasPestanyasInvalidarDeCache != null && listaRutasPestanyasInvalidarDeCache.Count > 0)
            {
                //GnossCache.AgregarObjetoCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_INVALIDAR + ProyectoSeleccionado.Clave, listaRutasPestanyasInvalidarDeCache, 64800);
                proyectoCL.AgregarObjetoCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_INVALIDAR + ProyectoSeleccionado.Clave, listaRutasPestanyasInvalidarDeCache, 64800);
            }
            if (listaRutasPestanyasRegistrarDeCache != null && listaRutasPestanyasRegistrarDeCache.Count > 0)
            {
                //GnossCache.AgregarObjetoCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_REGISTRAR + ProyectoSeleccionado.Clave, listaRutasPestanyasRegistrarDeCache, 64800);
                proyectoCL.AgregarObjetoCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_REGISTRAR + ProyectoSeleccionado.Clave, listaRutasPestanyasRegistrarDeCache, 64800);
            }

            proyectoCL.Dispose();
        }
    }
}
