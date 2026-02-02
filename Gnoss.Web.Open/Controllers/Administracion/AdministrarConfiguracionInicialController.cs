using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.Logica.Peticion;
using System.Net;
using System.Diagnostics;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Modelo de la página de administrar comunidad Home
    /// </summary>
    public class AdministrarConfiguracionInicialModel
    { }

    /// <summary>
    /// Controller de administrar configuración inicial del proyecto.
    /// </summary>
    public partial class AdministrarConfiguracionInicialController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarConfiguracionInicialController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarConfiguracionInicialController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;

        }

        /// <summary>
        /// Método index que devuelve la vista de configuración inicial de la comunidad/proyecto
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.ConfiguracionInicial;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.ConfiguracionInicial_DatosAcceso_Urls;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACIONINICIAL");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "DATOSDEACCESOYURLS");

            ViewBag.isInitialConfiguration = true;

            ComunidadInicialModel model = new ComunidadInicialModel();
            model.UrlProyectosPublicos = ProyectoSeleccionado.UrlPropia(IdiomaUsuario);
            model.UrlIntragnoss = $"http://gnoss/";

            return View(model);
        }

        /// <summary>
        /// Método para crear una comunidad y modificar los datos del usuario administrador por defecto
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Guardar(string UserName, string UserEmail, string UserPassword, string UrlProyectosPublicos, string UrlProyectosPrivados, bool UseSameUrlForPrivateProjects, string UrlIntraGnoss, string Name, string ShortName, short Type)
        {
            GuardarLogAuditoria();
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(ShortName) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(UserEmail) && !string.IsNullOrEmpty(UserPassword) && !string.IsNullOrEmpty(UrlProyectosPublicos))
            {
                string error = ComprobarErroresConfiguracionInicial(UserName, ShortName);
                if (!string.IsNullOrEmpty(error))
                {
                    return GnossResultERROR(error);
                }

                #region Actualizar los parámetros de UrlIntragnoss y UrlsPropiasProyecto
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);

                parametroAplicacionCN.ActualizarParametroAplicacion("UrlIntragnoss", UrlIntraGnoss);

                string urlPropia = string.Empty;
                string urlPropiasProyecto = $"0={UrlProyectosPublicos}|2={UrlProyectosPublicos}";
                if (UseSameUrlForPrivateProjects)
                {
                    urlPropiasProyecto = $"{urlPropiasProyecto}|1={UrlProyectosPublicos}|3={UrlProyectosPublicos}";
                    urlPropia = UrlProyectosPublicos;
                }
                else
                {
                    urlPropiasProyecto = $"{urlPropiasProyecto}|1={UrlProyectosPrivados}|3={UrlProyectosPrivados}";
                    urlPropia = UrlProyectosPrivados;
                }

                parametroAplicacionCN.ActualizarParametroAplicacion("UrlsPropiasProyecto", urlPropiasProyecto);
                #endregion

                #region Crear la comunidad
                
                CrearComunidad(Name, ShortName, Type, urlPropia);
                
                #endregion

                #region Modificar los datos del administrador por defecto
                EstablecerUsuarioAdministrador(UserName, UserEmail, UserPassword);
                #endregion
            }
            else
            {
                return GnossResultERROR(UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "CAMPOSINCOMPLETOS"));
            }

            return GnossResultOK();
        }

        private void EstablecerUsuarioAdministrador(string pNombreUsuario, string pEmail, string pPassword)
        {
            string error = string.Empty;
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);

            try
            {
                Guid usuarioID = UsuarioActual.UsuarioID;

                usuarioCN.EstablecerLoginUsuario(usuarioID, pNombreUsuario);
                EstablecerEmailUsuario(pEmail, usuarioID);
                usuarioCN.EstablecerPasswordUsuario(usuarioID, pPassword);

                mEntityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                GuardarLogError($"Ha habido un error al intentar establecer el usuario administrar del proyecto. {ex.Message}");
            }
        }

        private void EstablecerEmailUsuario(string pEmail, Guid pUsuarioID)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            DataWrapperPersona dataWrapperPersona = personaCN.ObtenerPersonaPorUsuario(pUsuarioID);
            AD.EntityModel.Models.PersonaDS.Persona filaPersona = dataWrapperPersona?.ListaPersona.Find(persona => persona.UsuarioID.Equals(pUsuarioID));

            if (UtilCadenas.ValidarEmail(pEmail))
            {
                personaCN.ModificarCorreoPersona(filaPersona.PersonaID, pEmail);
            }
            else
            {
                throw new BadHttpRequestException($"El email introducido no es válido: {pEmail}");
            }

            personaCN.Dispose();
        }

        private void CrearComunidad(string pNombre, string pNombreCorto, short pTipo, string pUrlsPropias)
        {
            ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyecto>(), mLoggerFactory);
            DataWrapperOrganizacion orgDW = null;
            DataWrapperProyecto proyDWP = null;
            GestorParametroGeneral paramDS = new GestorParametroGeneral();
            DataWrapperTesauro tesauroDW = null;
            DataWrapperDocumentacion documentacionDW = null;
            DataWrapperUsuario dataWrapperUsuario = null;
            DataWrapperIdentidad dataWrapperIdentidad = null;
            
            Elementos.ServiciosGenerales.Proyecto proyecto = controladorProyecto.CrearNuevoProyecto(pNombre, pNombreCorto, "", null, pTipo, 1, IdiomaPorDefecto ,mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.PerfilUsuario.Clave, ProyectoAD.MetaOrganizacion, Guid.Empty, true, true, true, true, false, null, out orgDW, out proyDWP, out paramDS, out tesauroDW, out documentacionDW, out dataWrapperUsuario, out dataWrapperIdentidad,mAvailableServices, pUrlsPropias);

            mEntityContext.SaveChanges();
 
            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            controladorPersonas.ActualizarModeloBASE(proyecto.IdentidadCreadoraProyecto, proyecto.Clave, true, true, PrioridadBase.Alta, mAvailableServices);

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            proyCL.InvalidarCacheListaProyectosPerfil(IdentidadActual.PerfilUsuario.Clave);
            proyCL.Dispose();

            string versionDocConfiguracion = string.Empty;
            string urlDocConfigComunidad = string.Empty;
            byte[] buffer = null;
            string xml = string.Empty;
            string xmlTesauro = string.Empty;

            List<ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VersionDocConfigDefectoComunidad")).ToList();
            if (busqueda.Count > 0)
            {
                versionDocConfiguracion = busqueda.First().Valor;
                urlDocConfigComunidad = BaseURLContent + "/Documentacion/Configuracion/ConfiguracionComunidadDefecto.xml?v=" + versionDocConfiguracion;
                string urlDocTesauroComunidad = BaseURLContent + "/Documentacion/Configuracion/TesauroDefecto.xml?v=" + versionDocConfiguracion;
                WebClient webClient = new WebClient();

                try
                {
                    xml = webClient.DownloadString(urlDocConfigComunidad);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLog("Problema al obtener el xml de la comunidad por defecto. Ruta: " + urlDocConfigComunidad, mlogger);
                }

                try
                {
                    xmlTesauro = webClient.DownloadString(urlDocTesauroComunidad);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLog("Problema al obtener el xml del tesauro por defecto. Ruta: " + urlDocTesauroComunidad, mlogger);
                }
                webClient.Dispose();
            }


            if (!string.IsNullOrEmpty(xml))
            {
                controladorProyecto.ConfigurarComunidadConXML(xml, ProyectoAD.MetaOrganizacion, proyecto.Clave, mAvailableServices);

                //Subimos el fichero al servidor
                Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                gd.Url = UrlServicioWebDocumentacion.Replace("https://", "http://");
                gd.AdjuntarDocumentoADirectorio(buffer, "Configuracion/" + proyecto.Clave, proyecto.Clave.ToString(), ".xml");
                gd.AdjuntarDocumentoADirectorio(buffer, "Configuracion/" + proyecto.Clave, proyecto.Clave.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".xml");
                mLoggingService.AgregarEntradaDependencia("Subir archivo de configuración por defecto al gestor documental", false, "AceptarSolicitudNuevaComunidad", sw, true);
            }

            if (!string.IsNullOrEmpty(xmlTesauro) && xmlTesauro.Contains("Thesaurus"))
            {
                // configurar tesauro por defecto
                ControladorProyecto.ConfigurarTesauroComunidadConXML(proyecto.Clave, xmlTesauro);
            }
        }

        /// <summary>
        /// Comprueba que los datos introducidos para la configuración inicial son válidos
        /// </summary>
        /// <param name="pUserShortName">Nombre corto del usuario administrador (Login)</param>
        /// <param name="pCommunityShortName">Nombre corto de la comunidad a crear</param>
        /// <returns>Devuelve el error en caso de haberlo, si no lo hay devuelve una cadena vacía</returns>
        private string ComprobarErroresConfiguracionInicial(string pUserShortName, string pCommunityShortName)
        {            
            if(pUserShortName.Length < 3 || pUserShortName.Length > 12)
            {
                return UtilIdiomas.GetText("DEVTOOLS", "CARACTERESMINIMOSMAXIMOSNOMBRECORTOUSUARIO");
            }

            using (ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory))
            using (PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PeticionCN>(), mLoggerFactory))
            {
                if (proyectoCN.ExisteNombreCortoEnBD(pCommunityShortName) || peticionCN.ExistePeticionProyectoMismoNombreCorto(pCommunityShortName))
                {
                    return UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRECORTOREPETIDO", NombreProyectoEcosistema);
                }
            }

            return string.Empty;
        }
    }
}