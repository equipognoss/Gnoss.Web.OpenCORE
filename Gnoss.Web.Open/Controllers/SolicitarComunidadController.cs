using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
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
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class SolicitarComunidadController : ControllerBaseWeb
    {
        public SolicitarComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        private Peticion mSolicitudComunidad;
        private GestionPeticiones mGestorPeticiones;

        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.Logueado })]
        public ActionResult Index()
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> listaComunidadesAdministro = proyectoCN.ObtenerNombresProyectosPrivadosAdministradosPorUsuario(mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.PerfilUsuario.Clave);
            proyectoCN.Dispose();

            SolicitarComunidadViewModel model = new SolicitarComunidadViewModel();
            model.ComunidadesPrivadasAdministraUsuario = listaComunidadesAdministro;
            model.AceptacionAutomaticaDeComunidad = AceptarComunidadAutomaticamente;

            return View(model);
        }

        /// <summary>
        /// Se ejecuta al pulsar el botón de envío de solicitud
        /// </summary>
        /// <param name="sender">Objeto que produce el evento</param>
        /// <param name="e">Argumentos del evento</param>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.Logueado })]
        public ActionResult Send(string Name, string ShortName, string Description, short Type, Guid CommunityParent)
        {
            Description = HttpUtility.HtmlDecode(Description);
            Description = HttpUtility.UrlDecode(Description);

            if (Name != "" && ShortName != "" && Description != "")
            {
                string error = GenerarSolicitud(Name, ShortName, Description, Type, CommunityParent);
                if (!string.IsNullOrEmpty(error))
                {
                    return GnossResultERROR(UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "ERRORENVIO"));
                }
                else if (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(RequestParams("automaticAccept")) && AceptarComunidadAutomaticamente)
                {
                    AceptarComunidad();

                    string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ShortName);
                    //ParametroAplicacionDS.ParametroAplicacionRow filaParametroPasos = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.PasosAsistenteCreacionComunidad);
                    AD.EntityModel.ParametroAplicacion filaParametroPasos = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.PasosAsistenteCreacionComunidad));

                    if (filaParametroPasos == null || string.IsNullOrEmpty(filaParametroPasos.Valor))
                    {
                        return GnossResultUrl(urlComunidad);
                    }
                    else
                    {
                        string[] pasos = filaParametroPasos.Valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        return GnossResultUrl(urlComunidad + "/" + UtilIdiomas.GetText("URLSEM", pasos[0]) + "?new-community-wizard=1");
                    }
                }
            }
            else
            {
                return GnossResultERROR(UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "CAMPOSINCOMPLETOS"));
            }

            return GnossResultOK(UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "ENVIOCORRECTO"));
        }

        private string GenerarSolicitud(string Name, string ShortName, string Description, short Type, Guid CommunityParent, bool enviarEmailConfirmacion = true)
        {
            // Comprobar que el nombre corto y el nombre de la comunidad son únicos
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            string error = "";

            if (proyectoCN.ExisteNombreCortoEnBD(ShortName) || peticionCN.ExistePeticionProyectoMismoNombreCorto(ShortName))
            {
                error = UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRECORTOREPETIDO", NombreProyectoEcosistema);
            }
            //else if (proyectoCN.ExisteNombreEnBD(Name) || peticionCN.ExistePeticionProyectoMismoNombre(Name))
            //{
            //    error = UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBREREPETIDO", NombreProyectoEcosistema);
            //}
            // David: Comprobar que se ha elegido un proyecto padre en caso de ser una comunidad reservada
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

            string mensaje = "<p>" + UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRE") + " " + Name + "</p>";
            mensaje += "<p>" + UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRECORTO") + " " + ShortName + "</p>";
            mensaje += "<p>" + UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "DESCRIPCION") + " " + Description + "</p>";
            mensaje += "<p>" + UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "TIPO") + ":" + " " + ((TipoAcceso)Type).ToString() + "</p>";

            try
            {
                DataWrapperPeticion peticionDW = new DataWrapperPeticion();
                mGestorPeticiones = new GestionPeticiones(peticionDW, mLoggingService, mEntityContext);

                if ((TipoAcceso)Type == TipoAcceso.Reservado)
                {
                    proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Dictionary<Guid, string> listaComunidadesAdministro = proyectoCN.ObtenerNombresProyectosPrivadosAdministradosPorUsuario(mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.PerfilUsuario.Clave);
                    proyectoCN.Dispose();

                    string nombreProyectoPadre = listaComunidadesAdministro[CommunityParent];
                    mensaje += "<p>" + UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "SUBCOMUNIDADDE", nombreProyectoPadre) + "</p>";

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Guid proyectoPadreID = proyCN.ObtenerProyectoIDPorNombreLargo(nombreProyectoPadre);
                    proyCN.Dispose();
                    mSolicitudComunidad = mGestorPeticiones.AgregarPeticionDeNuevoProyecto(Name, ShortName, Description, Type, mControladorBase.UsuarioActual.UsuarioID, proyectoPadreID, IdentidadActual.PerfilUsuario.Clave);
                }
                else
                {
                    mSolicitudComunidad = mGestorPeticiones.AgregarPeticionDeNuevoProyecto(Name, ShortName, Description, Type, mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.PerfilUsuario.Clave);
                }
                DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                GestionNotificaciones GestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                //ParametroAplicacionDS.ParametroAplicacionRow[] filas = (ParametroAplicacionDS.ParametroAplicacionRow[])ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro='" + TiposParametrosAplicacion.CorreoSolicitudes + "'");
                List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> filas = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.CorreoSolicitudes)).ToList();

                if (enviarEmailConfirmacion)
                {
                    GestorNotificaciones.AgregarNotificacionSolicitudCrearComunidad(IdentidadActual, BaseURL, "SOLICITUD DE COMUNIDAD" + UtilIdiomas.GetText("SOLICITARCOMUNIDAD", "NOMBRE"), mensaje, filas[0].Valor, UtilIdiomas.LanguageCode);
                }

                mEntityContext.SaveChanges();
                GestorNotificaciones.Dispose();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex.Message);
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
                ProyectoCN proyeCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                organizacionID = proyeCN.ObtenerProyectoCargaLigeraPorID(peticion.ComunidadPrivadaPadreID.Value).ListaProyecto.First().OrganizacionID;
                proyeCN.Dispose();
                idPadre = peticion.ComunidadPrivadaPadreID.Value;
            }
            ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

            DataWrapperOrganizacion orgDW = null;
            DataWrapperProyecto proyDWP = null;
            GestorParametroGeneral paramDS = new GestorParametroGeneral();
            DataWrapperTesauro tesauroDW = null;
            DataWrapperDocumentacion documentacionDW = null;
            DataWrapperUsuario dataWrapperUsuario = null;
            DataWrapperIdentidad dataWrapperIdentidad = null;

            Proyecto proyecto = controladorProyecto.CrearNuevoProyecto(peticion.Nombre, peticion.NombreCorto, peticion.Descripcion, null, peticion.Tipo, 1, peticion.Peticion.UsuarioID.Value, peticion.PerfilCreadorID, organizacionID, idPadre, true, true, true, true, false, null, out orgDW, out proyDWP, out paramDS, out tesauroDW, out documentacionDW, out dataWrapperUsuario, out dataWrapperIdentidad);
            peticion.Peticion.Estado = (short)EstadoPeticion.Aceptada;
            peticion.Peticion.FechaProcesado = DateTime.Now;

            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<PersonaIdentidad> listaPersonaIdentidad = personaCN.ObtenerEmaileIdentidadATravesDePerfil(peticion.PerfilCreadorID);
            personaCN.Dispose();

            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            if (listaPersonaIdentidad.Count == 1)
            {
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                string nombre = listaPersonaIdentidad.First().Nombre;
                Guid personaID = listaPersonaIdentidad.First().PersonaID;
                string correo = listaPersonaIdentidad.First().Email;
                string enlaceAComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proyecto.NombreCorto);
                gestorNotificaciones.AgregarNotificacionSolicitudAceptadaNuevaComunidad(nombre, personaID, proyecto.Clave, proyecto.FilaProyecto.OrganizacionID, proyecto.Nombre, correo, enlaceAComunidad, UtilIdiomas.LanguageCode);
            }
            mEntityContext.SaveChanges();

            //Actualizo el modelo base:
            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            controladorPersonas.ActualizarModeloBASE(proyecto.IdentidadCreadoraProyecto, proyecto.Clave, true, true, PrioridadBase.Alta);

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarCacheListaProyectosPerfil(peticion.PerfilCreadorID);
            proyCL.Dispose();

            //lectura de la configuración desde el xml por defecto via petición web
            string versionDocConfiguracion = string.Empty;
            string urlDocConfigComunidad = string.Empty;
            byte[] buffer = null;
            string xml = string.Empty;
            string xmlTesauro = string.Empty;

            List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VersionDocConfigDefectoComunidad")).ToList();
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
                catch(Exception ex) 
                {
                    mLoggingService.GuardarLog("Problema al obtener el xml de la comunidad por defecto. Ruta: " + urlDocConfigComunidad);
                }

                try
                {
                    xmlTesauro = webClient.DownloadString(urlDocTesauroComunidad);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLog("Problema al obtener el xml del tesauro por defecto. Ruta: " + urlDocTesauroComunidad);
                }
                webClient.Dispose();
            }


            if (!string.IsNullOrEmpty(xml))
            {
                controladorProyecto.ConfigurarComunidadConXML(xml, organizacionID, proyecto.Clave);

                //Subimos el fichero al servidor
                Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                GestionDocumental gd = new GestionDocumental(mLoggingService);
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

        private bool AceptarComunidadAutomaticamente
        {
            get
            {
                //ParametroAplicacionDS.ParametroAplicacionRow filaParametroAceptarAutomaticamente = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.AceptacionComunidadesAutomatica);
                AD.EntityModel.ParametroAplicacion filaParametroAceptarAutomaticamente = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.AceptacionComunidadesAutomatica));
                return (filaParametroAceptarAutomaticamente == null || !filaParametroAceptarAutomaticamente.Valor.Equals("0"));
            }
        }
    }

}
