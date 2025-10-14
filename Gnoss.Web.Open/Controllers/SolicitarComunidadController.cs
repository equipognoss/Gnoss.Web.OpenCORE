using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class SolicitarComunidadController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public SolicitarComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<SolicitarComunidadController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {

            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        private Peticion mSolicitudComunidad;
        private GestionPeticiones mGestorPeticiones;

        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.Logueado })]
        public ActionResult Index()
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Dictionary<Guid, string> listaComunidadesAdministro = proyectoCN.ObtenerNombresProyectosPrivadosAdministradosPorUsuario(mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.PerfilUsuario.Clave);
            proyectoCN.Dispose();

            SolicitarComunidadViewModel model = new SolicitarComunidadViewModel();
            model.ComunidadesPrivadasAdministraUsuario = listaComunidadesAdministro;
            model.AceptacionAutomaticaDeComunidad = AceptarComunidadAutomaticamente;
            ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext,mLoggingService,mRedisCacheWrapper,mConfigService,mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
            model.ListaIdiomasPlataforma = parametroAplicacionCL.ObtenerListaIdiomasDictionary();
            model.IdiomaPorDefecto = IdiomaPorDefecto;

            return View(model);
        }

        /// <summary>
        /// Se ejecuta al pulsar el botón de envío de solicitud
        /// </summary>
        /// <param name="sender">Objeto que produce el evento</param>
        /// <param name="e">Argumentos del evento</param>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.Logueado })]
        public ActionResult Send(string Name, string ShortName, string Description, short Type, string Language, Guid CommunityParent)
        {
            Description = HttpUtility.HtmlDecode(Description);
            Description = HttpUtility.UrlDecode(Description);
            
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            bool esUsuarioMetaadministrador = proyectoCN.EsUsuarioAdministradorProyecto(mControladorBase.UsuarioActual.UsuarioID, ProyectoAD.MetaProyecto);

            if (Name != string.Empty && ShortName != string.Empty && Description != string.Empty)
            {
                string error = GenerarSolicitud(Name, ShortName, Description, Type, Language, CommunityParent);
                if (!string.IsNullOrEmpty(error))
                {
                    return GnossResultERROR(UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "ERRORENVIO"));
                }
                else if (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(RequestParams("automaticAccept")) && AceptarComunidadAutomaticamente && esUsuarioMetaadministrador)
                {
                    AceptarComunidad();

                    string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ShortName);
                    ParametroAplicacion filaParametroPasos = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.PasosAsistenteCreacionComunidad));

                    if (filaParametroPasos == null || string.IsNullOrEmpty(filaParametroPasos.Valor))
                    {
                        return GnossResultUrl(urlComunidad);
                    }
                    else
                    {
                        string[] pasos = filaParametroPasos.Valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        return GnossResultUrl($"{urlComunidad}/{UtilIdiomas.GetText("URLSEM", pasos[0])}?new-community-wizard=1");
                    }
                }
            }
            else
            {
                return GnossResultERROR(UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "CAMPOSINCOMPLETOS"));
            }

            return GnossResultOK(UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "ENVIOCORRECTO"));
        }

        private string GenerarSolicitud(string Name, string ShortName, string Description, short Type, string Language, Guid CommunityParent, bool enviarEmailConfirmacion = true)
        {
            // Comprobar que el nombre corto y el nombre de la comunidad son únicos
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PeticionCN>(), mLoggerFactory);
            string error = "";

            if (proyectoCN.ExisteNombreCortoEnBD(ShortName) || peticionCN.ExistePeticionProyectoMismoNombreCorto(ShortName))
            {
                error = UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRECORTOREPETIDO", NombreProyectoEcosistema);
            }            
            else if ((TipoAcceso)Type == TipoAcceso.Reservado && CommunityParent == Guid.Empty)
            {
                error = UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "COMRESERVADASINCOMPRIVADA");
            }

            proyectoCN.Dispose();
            peticionCN.Dispose();
            if (error != "")
            {
                return error;
            }

            string mensaje = $"<p>{UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRE")} {Name}</p>";
            mensaje += $"<p>{UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRECORTO")} {ShortName}</p>";
            mensaje += $"<p>{UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "DESCRIPCION")} {Description}</p>";
            mensaje += $"<p>{UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "TIPO")}: {(TipoAcceso)Type}</p>";

            try
            {
                DataWrapperPeticion peticionDW = new DataWrapperPeticion();
                mGestorPeticiones = new GestionPeticiones(peticionDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionPeticiones>(), mLoggerFactory);

                if ((TipoAcceso)Type == TipoAcceso.Reservado)
                {
                    proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    Dictionary<Guid, string> listaComunidadesAdministro = proyectoCN.ObtenerNombresProyectosPrivadosAdministradosPorUsuario(mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.PerfilUsuario.Clave);
                    proyectoCN.Dispose();

                    string nombreProyectoPadre = listaComunidadesAdministro[CommunityParent];
                    mensaje += $"<p>{UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "SUBCOMUNIDADDE", nombreProyectoPadre)}</p>";

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    Guid proyectoPadreID = proyCN.ObtenerProyectoIDPorNombreLargo(nombreProyectoPadre);
                    proyCN.Dispose();
                    mSolicitudComunidad = mGestorPeticiones.AgregarPeticionDeNuevoProyecto(Name, ShortName, Description, Language, Type, mControladorBase.UsuarioActual.UsuarioID, proyectoPadreID, IdentidadActual.PerfilUsuario.Clave);
                }
                else
                {
                    mSolicitudComunidad = mGestorPeticiones.AgregarPeticionDeNuevoProyecto(Name, ShortName, Description, Language, Type, mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.PerfilUsuario.Clave);
                }
                DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                GestionNotificaciones GestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);

                List<ParametroAplicacion> filas = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.CorreoSolicitudes)).ToList();
                
                if (enviarEmailConfirmacion && filas.Any())
                {                    
                    GestorNotificaciones.AgregarNotificacionSolicitudCrearComunidad(IdentidadActual, BaseURL, $"SOLICITUD DE COMUNIDAD{UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRE")}", mensaje, filas[0].Valor, UtilIdiomas.LanguageCode);
                }

                mEntityContext.SaveChanges();
                GestorNotificaciones.Dispose();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                return UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "ERRORENVIO");
            }

            return null;
        }

        private void AceptarComunidad()
        {
            //Obtenemos la peticion del nuevo proy
            AD.EntityModel.Models.Peticion.PeticionNuevoProyecto peticion = ((PeticionNuevoProyecto)mSolicitudComunidad).FilaNuevoProyecto;
            Guid organizacionID = ProyectoAD.MetaOrganizacion;
            Guid idPadre = Guid.Empty;

            if (peticion.ComunidadPrivadaPadreID.HasValue)
            {
                ProyectoCN proyeCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                organizacionID = proyeCN.ObtenerProyectoCargaLigeraPorID(peticion.ComunidadPrivadaPadreID.Value).ListaProyecto[0].OrganizacionID;
                proyeCN.Dispose();
                idPadre = peticion.ComunidadPrivadaPadreID.Value;
            }
            ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyecto>(), mLoggerFactory);

            DataWrapperOrganizacion orgDW = null;
            DataWrapperProyecto proyDWP = null;
            GestorParametroGeneral paramDS;
            DataWrapperTesauro tesauroDW = null;
            DataWrapperDocumentacion documentacionDW = null;
            DataWrapperUsuario dataWrapperUsuario = null;
            DataWrapperIdentidad dataWrapperIdentidad = null;

            Proyecto proyecto = controladorProyecto.CrearNuevoProyecto(peticion.Nombre, peticion.NombreCorto, peticion.Descripcion, null, peticion.Tipo, 1, peticion.IdiomaDefecto, peticion.Peticion.UsuarioID.Value, peticion.PerfilCreadorID, organizacionID, idPadre, true, true, true, true, false, null, out orgDW, out proyDWP, out paramDS, out tesauroDW, out documentacionDW, out dataWrapperUsuario, out dataWrapperIdentidad, mAvailableServices, null, DominioConfigurado);
            peticion.Peticion.Estado = (short)EstadoPeticion.Aceptada;
            peticion.Peticion.FechaProcesado = DateTime.Now;

            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            List<PersonaIdentidad> listaPersonaIdentidad = personaCN.ObtenerEmaileIdentidadATravesDePerfil(peticion.PerfilCreadorID);
            personaCN.Dispose();

            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            if (listaPersonaIdentidad.Count == 1)
            {
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);
                string nombre = listaPersonaIdentidad[0].Nombre;
                Guid personaID = listaPersonaIdentidad[0].PersonaID;
                string correo = listaPersonaIdentidad[0].Email;
                string enlaceAComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proyecto.NombreCorto);
                gestorNotificaciones.AgregarNotificacionSolicitudAceptadaNuevaComunidad(nombre, personaID, proyecto.Clave, proyecto.FilaProyecto.OrganizacionID, proyecto.Nombre, correo, enlaceAComunidad, UtilIdiomas.LanguageCode);
            }
            mEntityContext.SaveChanges();

            //Actualizo el modelo base:
            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            controladorPersonas.ActualizarModeloBASE(proyecto.IdentidadCreadoraProyecto, proyecto.Clave, true, true, PrioridadBase.Alta, mAvailableServices);

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            proyCL.InvalidarCacheListaProyectosPerfil(peticion.PerfilCreadorID);
            proyCL.Dispose();

            //lectura de la configuración desde el xml por defecto via petición web
            string versionDocConfiguracion = string.Empty;
            string urlDocConfigComunidad = string.Empty;
            byte[] buffer = null;
            string xml = string.Empty;
            string xmlTesauro = string.Empty;

            List<ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VersionDocConfigDefectoComunidad")).ToList();
            if (busqueda.Count > 0)
            {
                versionDocConfiguracion = busqueda[0].Valor;
                urlDocConfigComunidad = $"{BaseURLContent}/Documentacion/Configuracion/ConfiguracionComunidadDefecto.xml?v={versionDocConfiguracion}";
                string urlDocTesauroComunidad = $"{BaseURLContent}/Documentacion/Configuracion/TesauroDefecto.xml?v={versionDocConfiguracion}";
                WebClient webClient = new WebClient();

                try
                {
                    xml = webClient.DownloadString(urlDocConfigComunidad);
                }
                catch 
                {
                    mLoggingService.GuardarLog($"Problema al obtener el xml de la comunidad por defecto. Ruta: {urlDocConfigComunidad}", mlogger);
                }

                try
                {
                    xmlTesauro = webClient.DownloadString(urlDocTesauroComunidad);
                }
                catch
                {
                    mLoggingService.GuardarLog($"Problema al obtener el xml del tesauro por defecto. Ruta: {urlDocTesauroComunidad}", mlogger);
                }
                webClient.Dispose();
            }

            //Insertar la Vista Virtual para poder administrar las vistas
            VistaVirtualPersonalizacion filaVistaVirtualPersonalizacion = new VistaVirtualPersonalizacion();
            filaVistaVirtualPersonalizacion.PersonalizacionID = Guid.NewGuid();
            mEntityContext.VistaVirtualPersonalizacion.Add(filaVistaVirtualPersonalizacion);

            VistaVirtualProyecto filaVistaVirtualProyecto = new VistaVirtualProyecto();
            filaVistaVirtualProyecto.PersonalizacionID = filaVistaVirtualPersonalizacion.PersonalizacionID;
            filaVistaVirtualProyecto.ProyectoID = proyecto.Clave;
            filaVistaVirtualProyecto.OrganizacionID = ProyectoAD.MetaOrganizacion;
            mEntityContext.VistaVirtualProyecto.Add(filaVistaVirtualProyecto);

            if (!string.IsNullOrEmpty(xml))
            {
                controladorProyecto.ConfigurarComunidadConXML(xml, organizacionID, proyecto.Clave, mAvailableServices);

                //Subimos el fichero al servidor
                Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<GestionDocumental>(), mLoggerFactory);
                gd.Url = UrlServicioWebDocumentacion.Replace("https://", "http://");
                gd.AdjuntarDocumentoADirectorio(buffer, $"Configuracion/{proyecto.Clave}", proyecto.Clave.ToString(), ".xml");
                gd.AdjuntarDocumentoADirectorio(buffer, $"Configuracion/{proyecto.Clave}", $"{proyecto.Clave.ToString()}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}", ".xml");
                mLoggingService.AgregarEntradaDependencia("Subir archivo de configuración por defecto al gestor documental", false, "AceptarSolicitudNuevaComunidad", sw, true);
            }

            controladorProyecto.InsertarParametrosAdministracion(proyecto.Clave, organizacionID);
            if (!string.IsNullOrEmpty(xmlTesauro) && xmlTesauro.Contains("Thesaurus"))
            {
                // configurar tesauro por defecto
                ControladorProyecto.ConfigurarTesauroComunidadConXML(proyecto.Clave, xmlTesauro);
            }
        }

        private bool AceptarComunidadAutomaticamente
        {
            get
            {                
                ParametroAplicacion filaParametroAceptarAutomaticamente = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.AceptacionComunidadesAutomatica));
                return (filaParametroAceptarAutomaticamente == null || !filaParametroAceptarAutomaticamente.Valor.Equals("0"));
            }
        }
    }
}