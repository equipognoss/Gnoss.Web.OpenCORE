using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class EnviarEnlaceController : ControllerBaseWeb
    {
        public EnviarEnlaceController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        public ActionResult Index()
        {
            string rutaEnlace = ViewBag.UrlPagina;
            rutaEnlace = rutaEnlace.Substring(0, rutaEnlace.LastIndexOf("/" + UtilIdiomas.GetText("URLSEM", "ENVIARPORCORREO")));

            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                if (EsBot)
                {
                    return RedirectPermanent(rutaEnlace);
                }
                else
                {
                    return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "LOGIN") + "/redirect/" + new Uri((string)ViewBag.UrlPagina).AbsolutePath);
                }
            }

            ControladorAmigos contrAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            contrAmigos.CargarAmigos(IdentidadActual, true);

            SendLinkViewModel paginaModel = new SendLinkViewModel();

            string nombreEnlace = RequestParams("nombreElem");

            if (!string.IsNullOrEmpty(RequestParams("tipo")) && RequestParams("tipo").Equals("recurso") && !string.IsNullOrEmpty(RequestParams("docID")))
            {
                Guid recursoID = Guid.Empty;
                if (Guid.TryParse(RequestParams("docID"), out recursoID))
                {
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    nombreEnlace = UtilCadenas.ObtenerTextoDeIdioma(docCN.ObtenerTituloDocumentoPorID(recursoID), IdiomaUsuario, null);
                }
            }

            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            Dictionary<TipoProyectoEventoAccion, string> listaEventos = ProyectoSeleccionado.ListaTipoProyectoEventoAccion;
            proyectoCL.Dispose();
            string accionEnvioEnlace = string.Empty;
            if (listaEventos != null && listaEventos.Count > 0)
            {
                //comprobación existencia evento EnviarEnlace
                if (listaEventos.ContainsKey(TipoProyectoEventoAccion.EnviarEnlace))
                {
                    accionEnvioEnlace = listaEventos[TipoProyectoEventoAccion.EnviarEnlace];
                }
            }

            //if (IdentidadActual.IdentidadOrganizacion != null)
            //{
            //    ViewBag.IdentidadOrgID = IdentidadActual.IdentidadOrganizacion.Clave;
            //}
            paginaModel.ExtraActionSent = accionEnvioEnlace.Replace("ctl00_ctl00_CPH1_CPHContenido_", "");
            paginaModel.LinkUrl = rutaEnlace;
            paginaModel.LinkName = nombreEnlace;

            return View(paginaModel);
        }

        [HttpPost]
        public ActionResult GuardarCambios(SendLinkViewModel pEnviarLinkModel)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "LOGIN") + "/redirect/" + new Uri((string)ViewBag.UrlPagina).AbsolutePath);
            }

            string nombreEnlace = RequestParams("nombreElem");

            if (!string.IsNullOrEmpty(RequestParams("tipo")) && RequestParams("tipo").Equals("recurso") && !string.IsNullOrEmpty(RequestParams("docID")))
            {
                Guid recursoID = Guid.Empty;
                if (Guid.TryParse(RequestParams("docID"), out recursoID))
                {
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    nombreEnlace = UtilCadenas.ObtenerTextoDeIdioma(docCN.ObtenerTituloDocumentoPorID(recursoID), IdiomaUsuario, null);
                }
            }

            string rutaEnlace = ViewBag.UrlPagina;
            rutaEnlace = rutaEnlace.Substring(0, rutaEnlace.LastIndexOf("/" + UtilIdiomas.GetText("URLSEM", "ENVIARPORCORREO")));
            rutaEnlace = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + rutaEnlace;

            if (!string.IsNullOrEmpty(pEnviarLinkModel.Receivers))
            {
                string[] correos = pEnviarLinkModel.Receivers.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                string notas = "";
                if(!string.IsNullOrEmpty(pEnviarLinkModel.Message))
                {
                    notas =  HttpUtility.UrlDecode(pEnviarLinkModel.Message.Replace("\n", "<br>"));
                }
                string idioma = pEnviarLinkModel.Lang;

                EnviarCorreos(correos, notas, idioma, rutaEnlace, nombreEnlace);
            }

            return GnossResultUrl(rutaEnlace);
        }

        /// <summary>
        /// Enviar correos
        /// </summary>
        /// <param name="sender">Objeto que produce el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void EnviarCorreos(string[] pCorreos, string pNotas, string pIdioma, string pRutaEnlace, string pNombreEnlace)
        {
            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
            GestionNotificaciones GestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<Identidad> listaIdentidadesPersonasGnoss = new List<Identidad>();
            List<string> listaEmails = new List<string>();

            bool correoParaIdentidadesEnElMetaEspacio = false;
            List<Guid> listaPerfilesDestinatarios = new List<Guid>();

            ControladorAmigos contrAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            contrAmigos.CargarAmigos(IdentidadActual, EsAdministrador(IdentidadActual));

            GestionCorreo gestionCorreo = new GestionCorreo(null, null, IdentidadActual.GestorIdentidades, IdentidadActual.GestorAmigos, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestionCorreo.GestorNotificaciones = GestorNotificaciones;
            List<Guid> listaIdentidadesEnvioConjunto = new List<Guid>();

            List<Guid> listaIdentContactosYGrupos = new List<Guid>();

            foreach (string correo in pCorreos)
            {
                string cor = correo.Trim();

                if (cor.Contains("@"))
                {
                    listaEmails.Add(cor);
                }
                else
                {
                    listaIdentContactosYGrupos.Add(new Guid(cor));
                }
            }

            if (listaIdentContactosYGrupos.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<Guid> listaIdentiadades = identidadCN.ObtenerIdentidadesDeMyGnossDeParticipantesDeGrupos(listaIdentContactosYGrupos);
                DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerIdentidadesPorID(listaIdentContactosYGrupos, true);
                identidadCN.Dispose();

                GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                foreach (Guid identidadID in gestorIdentidades.ListaIdentidades.Keys)
                {
                    if (!listaIdentiadades.Contains(identidadID))
                    {
                        listaIdentiadades.Add(identidadID);
                    }
                }

                if (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado)
                {
                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    listaIdentiadades = identCN.ObtenerIdentidadesIDDeMyGNOSSPorIdentidades(listaIdentiadades);
                    identCN.Dispose();
                }

                DataWrapperIdentidad identDW = new DataWrapperIdentidad();
                DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
                DataWrapperOrganizacion orgDW = new DataWrapperOrganizacion();
                ObtenerIdentidadesPorID(listaIdentiadades, identDW, dataWrapperPersona, orgDW);
                GestionIdentidades GestorIdentidadesContactos = new GestionIdentidades(identDW, new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext), new GestionOrganizaciones(orgDW, mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                foreach (Guid cor in listaIdentiadades)
                {
                    Identidad identidad = GestorIdentidadesContactos.ListaIdentidades[cor];

                    if (!listaPerfilesDestinatarios.Contains(identidad.PerfilID))
                    {
                        listaPerfilesDestinatarios.Add(identidad.PerfilID);
                    }

                    if (identidad != null)
                    {
                        listaIdentidadesPersonasGnoss.Add(identidad);
                    }

                    if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                    {
                        if (!listaIdentidadesEnvioConjunto.Contains(identidad.Clave))
                        {
                            listaIdentidadesEnvioConjunto.Add(identidad.Clave);
                        }
                    }
                    else
                    {
                        List<Guid> listaIdentidadesTemporal = new List<Guid>();
                        listaIdentidadesTemporal.Add(identidad.Clave);
                        EnviarEnlaceCorreoDestinaratarios(pRutaEnlace, pNombreEnlace, gestionCorreo, pNotas, listaIdentidadesTemporal);

                        correoParaIdentidadesEnElMetaEspacio = true;
                    }
                }
            }

            Guid correoID = Guid.Empty;
            if (listaIdentidadesEnvioConjunto.Count > 0)
            {
                correoID = EnviarEnlaceCorreoDestinaratarios(pRutaEnlace, pNombreEnlace, gestionCorreo, pNotas, listaIdentidadesEnvioConjunto);
            }

            CorreoCN actualizarCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            actualizarCN.ActualizarCorreo(gestionCorreo.CorreoDS);

            if (correoID != Guid.Empty)
            {
                string destinatarios = "";
                foreach (Guid destinatario in listaIdentidadesEnvioConjunto)
                {
                    destinatarios += "|" + destinatario.ToString();
                }
                if (destinatarios.StartsWith("|"))
                {
                    destinatarios = destinatarios.Substring(1);
                }

                //Añadimos una línea al base para que la procese:
                ControladorDocumentacion.AgregarMensajeFacModeloBaseSimple(correoID, IdentidadActual.IdentidadMyGNOSS.Clave, ProyectoSeleccionado.Clave, "base", destinatarios, null, Es.Riam.Gnoss.AD.BASE_BD.PrioridadBase.Alta);

            }

            if (listaEmails.Count > 0 || listaIdentidadesPersonasGnoss.Count > 0)
            {
                string enlaceRuta = "<a href='" + pRutaEnlace + "'>" + pNombreEnlace + "</a>";

                //Si se envía desde identidad de organización en modo corporativo sólo se muestra el nombre de la organización.
                //Si es organización en modo personal o identidad personal se verá el nombre completo de la identidad.
                string nombreIdentidad = "";

                if (IdentidadActual.TrabajaConOrganizacion)
                {
                    nombreIdentidad = IdentidadActual.PerfilUsuario.NombrePersonaEnOrganizacion + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + IdentidadActual.PerfilUsuario.NombreOrganizacion;
                }
                else
                {
                    nombreIdentidad = IdentidadActual.Nombre(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave));
                }

                if (!IdentidadActual.ModoPersonal)
                {
                    nombreIdentidad = IdentidadActual.NombreOrganizacion;
                }

                if (listaEmails.Count > 0)
                {
                    GestorNotificaciones.AgregarNotificacionEnlaceExterno(nombreIdentidad, listaEmails, pNotas, enlaceRuta, pNombreEnlace, pIdioma, ProyectoSeleccionado);
                }

                if (listaIdentidadesPersonasGnoss.Count > 0)
                {
                    GestorNotificaciones.AgregarNotificacionEnlace(nombreIdentidad, listaIdentidadesPersonasGnoss, pNotas, enlaceRuta, pNombreEnlace, pIdioma, ProyectoSeleccionado);
                }
            }

            if (listaEmails.Count > 0 || listaIdentidadesPersonasGnoss.Count > 0 || correoParaIdentidadesEnElMetaEspacio)
            {
                notificacionCN.ActualizarNotificacion();
                notificacionCN.Dispose();
            }
        }

        private Guid EnviarEnlaceCorreoDestinaratarios(string pRutaEnlace, string pNombreEnlace, GestionCorreo pGestionCorreo, string pNotas, List<Guid> pListaIdentidadesEnvioConjunto)
        {
            Guid correoID = Guid.Empty;

            string enlaceRuta = "<a href='" + pRutaEnlace + "'>" + pNombreEnlace + "</a>";

            string nombreEnlaceCorto = pNombreEnlace;

            //Límite máximo en 60 caracteres
            int logintudNombreEnlaceCortoEn = 90 - UtilIdiomas.GetText("ENVIOENLACE", "ENVIADOMENSAJE").Length;
            if (logintudNombreEnlaceCortoEn > 5 && nombreEnlaceCorto.Length > logintudNombreEnlaceCortoEn)
            {
                nombreEnlaceCorto = nombreEnlaceCorto.Substring(0, logintudNombreEnlaceCortoEn) + "...";
            }

            if (!EsEcosistemaSinMetaProyecto)
            {
                correoID = pGestionCorreo.AgregarCorreo(IdentidadActual.IdentidadMyGNOSS.Clave, pListaIdentidadesEnvioConjunto, UtilIdiomas.GetText("ENVIOENLACE", "ENVIADOMENSAJE") + nombreEnlaceCorto, pNotas + "<br/>" + enlaceRuta, this.BaseURL, TipoEnvioCorreoBienvenida.CorreoInterno, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
            }
            else
            {
                correoID = pGestionCorreo.AgregarCorreo(IdentidadActual.IdentidadMyGNOSS.Clave, pListaIdentidadesEnvioConjunto, UtilIdiomas.GetText("ENVIOENLACE", "ENVIADOMENSAJE") + nombreEnlaceCorto, pNotas + "<br/>" + enlaceRuta, this.BaseURL, TipoEnvioCorreoBienvenida.CorreoInterno, ProyectoVirtual, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
            }

            return correoID;
        }

    }
}
