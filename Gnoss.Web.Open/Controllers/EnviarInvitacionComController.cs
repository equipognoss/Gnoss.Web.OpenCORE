using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class EnviarInvitacionComController : ControllerBaseWeb
    {

        /// <summary>
        /// Obtiene si hay que mostrar la pestanya de importar contactos por el email.
        /// </summary>
        private bool? mPestanyaImportarContactosCorreo = null;

        /// <summary>
        /// Devuelve si hay que mostrar el panel con el mensaje que se va a enviar a la hora de importar contactos.
        /// </summary>
        private bool? mPanelMensajeImportarContactos = null;


        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        /// <summary>
        /// Enumeración para representar el estado del envio de invitaciones
        /// </summary>
        public enum SendStatus
        {
            Ok, Ko, EnviadasAUsuariosYaAgnadidos
        }

        public EnviarInvitacionComController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<EnviarInvitacionComController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EsPaginaEdicion = true;
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            if (!ParametrosGeneralesRow.InvitacionesDisponibles || (ProyectoSeleccionado.Estado != (short)EstadoProyecto.Abierto))
            {
                if (!mControladorBase.UsuarioActual.EstaAutorizadoEnProyecto((ulong)Capacidad.Proyecto.CapacidadesAdministrador.AdministrarProyecto))
                {
                    return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
            }

            SendInvitationComViewModel paginaModel = new SendInvitationComViewModel();

            if (!ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionExpandida))
            {
                paginaModel.Message = UtilIdiomas.GetText("INVITACIONES", "TEXTODEFECTOINVITACIONCOMUNIDAD", ProyectoSeleccionado.FilaProyecto.Nombre, UtilCadenas.EliminarEnlacesDeHtml(UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto)), NombreProyectoEcosistema);
            }
            else
            {
                paginaModel.Message = UtilIdiomas.GetText("INVITACIONES", "TEXTODEFECTOINVITACIONCLASE", IdentidadActual.Persona.NombreConApellidos, ProyectoSeleccionado.FilaProyecto.Nombre, NombreProyectoEcosistema);
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGruposDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            paginaModel.AllowGroupsInvitations = ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) && gestorIdentidades.ListaGrupos.Count > 0;

            paginaModel.AllowInviteContacts = true;
            paginaModel.AllowInviteEmail = true;
            paginaModel.AllowPersonlizeMessage = true;
            if (!ParametrosGeneralesRow.InvitacionesPorContactoDisponibles)
            {
                paginaModel.AllowInviteContacts = false;
            }
            if (!PestanyaImportarContactosCorreo)
            {
                paginaModel.AllowInviteEmail = false;
            }
            if (!PanelMensajeImportarContactos)
            {
                paginaModel.AllowPersonlizeMessage = false;
            }

            if (!string.IsNullOrEmpty(RequestParams("new-community-wizard")))
            {
                // Está llegando a esta página desde el asistente de creación de comunidades, se la muestro como una página de administración
                EliminarPersonalizacionVistas();
            }

            return View(paginaModel);
        }

        /// <summary>
        /// Cargar la vista de invitación a comunidad en una vista modal
        /// </summary>
        public ActionResult LoadModal() {

            ActionResult partialView = View();

            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            if (!ParametrosGeneralesRow.InvitacionesDisponibles || (ProyectoSeleccionado.Estado != (short)EstadoProyecto.Abierto))
            {
                if (!mControladorBase.UsuarioActual.EstaAutorizadoEnProyecto((ulong)Capacidad.Proyecto.CapacidadesAdministrador.AdministrarProyecto))
                {
                    return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
            }

            SendInvitationComViewModel paginaModel = new SendInvitationComViewModel();

            if (!ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionExpandida))
            {
                paginaModel.Message = UtilIdiomas.GetText("INVITACIONES", "TEXTODEFECTOINVITACIONCOMUNIDAD", ProyectoSeleccionado.FilaProyecto.Nombre, UtilCadenas.EliminarEnlacesDeHtml(UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto)), NombreProyectoEcosistema);
            }
            else
            {
                paginaModel.Message = UtilIdiomas.GetText("INVITACIONES", "TEXTODEFECTOINVITACIONCLASE", IdentidadActual.Persona.NombreConApellidos, ProyectoSeleccionado.FilaProyecto.Nombre, NombreProyectoEcosistema);
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGruposDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            paginaModel.AllowGroupsInvitations = ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) && gestorIdentidades.ListaGrupos.Count > 0;

            paginaModel.AllowInviteContacts = true;
            paginaModel.AllowInviteEmail = true;
            paginaModel.AllowPersonlizeMessage = true;
            if (!ParametrosGeneralesRow.InvitacionesPorContactoDisponibles)
            {
                paginaModel.AllowInviteContacts = false;
            }
            if (!PestanyaImportarContactosCorreo)
            {
                paginaModel.AllowInviteEmail = false;
            }
            if (!PanelMensajeImportarContactos)
            {
                paginaModel.AllowPersonlizeMessage = false;
            }

            if (!string.IsNullOrEmpty(RequestParams("new-community-wizard")))
            {
                // Está llegando a esta página desde el asistente de creación de comunidades, se la muestro como una página de administración
                EliminarPersonalizacionVistas();
            }
            // Construir el modelo y devolver el modal
            partialView = GnossResultHtml("../Shared/_cabecera/_modal-views/_invite-community", paginaModel);
            // Devolver la vista modal
            return partialView;       
        }


        [HttpPost]
        public ActionResult GuardarCambios(SendInvitationComViewModel pEnviarInvitacionModel)
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            if (string.IsNullOrEmpty(pEnviarInvitacionModel.Guests))
            {
                return GnossResultERROR(UtilIdiomas.GetText("INVITACIONES", "SINDESTINATARIOS"));
            }

            if (!ParametrosGeneralesRow.InvitacionesDisponibles || (ProyectoSeleccionado.Estado != (short)EstadoProyecto.Abierto))
            {
                if (!mControladorBase.UsuarioActual.EstaAutorizadoEnProyecto((ulong)Capacidad.Proyecto.CapacidadesAdministrador.AdministrarProyecto))
                {
                    return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
            }

            string notas = UtilIdiomas.GetText("INVITACIONES", "TEXTODEFECTOINVITACIONCOMUNIDAD", ProyectoSeleccionado.FilaProyecto.Nombre, UtilCadenas.EliminarEnlacesDeHtml(UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto)), NombreProyectoEcosistema);

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGruposDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            bool allowGroupsInvitations = ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) && gestorIdentidades.ListaGrupos.Count > 0;
            if (pEnviarInvitacionModel.Guests == null)
            {
                return GnossResultERROR(UtilIdiomas.GetText("INVITACIONES", "SINDESTINATARIOS"));
            }

            string[] correos = pEnviarInvitacionModel.Guests.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            string[] grupos = { };

            if (allowGroupsInvitations)
            {
                if (pEnviarInvitacionModel.Groups != null)
                {
                    grupos = pEnviarInvitacionModel.Groups.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            if (pEnviarInvitacionModel.Message != null)
            {
                notas = HttpUtility.UrlDecode(pEnviarInvitacionModel.Message);
            }

            SendStatus status = EnviarInvitaciones(correos, notas, grupos);

            if (status == SendStatus.Ko)
            {
                return GnossResultERROR(UtilIdiomas.GetText("INVITACIONES", "SINDESTINATARIOS"));
            }
            else
            {
                string mensaje = "";
                if (status == SendStatus.Ok)
                {
                    mensaje = UtilIdiomas.GetText("INVITACIONES", "INVITACIONOK");
                }
                else
                {
                    mensaje = UtilIdiomas.GetText("INVITACIONES", "INVITACIONOKCONAGNADIDOS");
                }

                string redirect = ObtenerUrlSiguientePasoAsistenteNuevaComunidad();

                if (string.IsNullOrEmpty(redirect))
                {
                    return GnossResultOK(mensaje);
                }
                else
                {
                    return GnossResultUrl(redirect);
                }
            }
        }

        /// <summary>
        /// Se produce al pulsar el botón de enviar las invitaciones
        /// </summary>
        private SendStatus EnviarInvitaciones(string[] pInvitados, string pMensaje, string[] pGrupos)
        {
            SendStatus status;

            List<Guid> listaGrupos = new List<Guid>();
            foreach (string grupo in pGrupos)
            {
                listaGrupos.Add(new Guid(grupo));
            }

            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
            GestionNotificaciones GestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);

            List<Identidad> listaIdentidades = new List<Identidad>();
            List<string> listaEmails = new List<string>();

            List<Guid> listaIdentContactos = new List<Guid>();
            Dictionary<Guid, Guid> listaInvitacionesPorIdentidad = null;

            bool correosYaAgnadidos = false;

            //Añade los emails y las identidades a las que se enviará la invitación
            foreach (string invitado in pInvitados)
            {
                string inv = invitado.Trim();

                if (inv.Contains("@"))
                {
                    if (!listaEmails.Contains(invitado))
                    {
                        if (ComprobarCorreoEsValido(inv))
                        {
                            listaEmails.Add(inv);
                        }
                        else
                        {
                            correosYaAgnadidos = true;
                        }
                    }
                }
                else
                {
                    listaIdentContactos.Add(new Guid(invitado));
                }
            }

            if (listaIdentContactos.Count > 0)
            {
                DataWrapperIdentidad identDW = new DataWrapperIdentidad();
                DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
                DataWrapperOrganizacion orgDW = new DataWrapperOrganizacion();
                ObtenerIdentidadesPorID(listaIdentContactos, identDW, dataWrapperPersona, orgDW);
                GestionIdentidades gestorIdentidadesContactos = new GestionIdentidades(identDW, new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext), new GestionOrganizaciones(orgDW, mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                foreach (Guid cor in listaIdentContactos)
                {
                    if (ComprobarCorreoEsValido(gestorIdentidadesContactos.ListaIdentidades[cor].Email))
                    {
                        listaIdentidades.Add(gestorIdentidadesContactos.ListaIdentidades[cor]);
                    }
                    else
                    {
                        correosYaAgnadidos = true;
                    }
                }
            }

            if (listaIdentidades.Count > 0)
            {
                GestorNotificaciones.AgregarNotificacionInvitacionUsuarioACom(ProyectoSeleccionado, UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto), pMensaje, IdentidadActual, DateTime.Now, listaIdentidades, BaseURL, true, true, listaGrupos, UtilIdiomas.LanguageCode);
            }

            if (listaEmails.Count > 0)
            {
                Guid mOrganizacaionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;

                string urlEnlace = mControladorBase.UrlsSemanticas.GetURLAceptarInvitacionEnProyecto(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto);
                string nombreRemitente = IdentidadActual.Nombre(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave));

                if (IdentidadActual.ModoParticipacion.Equals(TiposIdentidad.ProfesionalCorporativo))
                {
                    nombreRemitente = IdentidadActual.NombreOrganizacion;
                }
                GestorNotificaciones.AgregarNotificacionInvitacionExternoACom(ProyectoSeleccionado, UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto), pMensaje, nombreRemitente, DateTime.Now, listaEmails, urlEnlace, UtilIdiomas.LanguageCode, listaGrupos);
            }

            status = SendStatus.Ok;

            if (listaEmails.Count > 0 || listaIdentidades.Count > 0)
            {
                notificacionCN.ActualizarNotificacion(mAvailableServices);
                notificacionCN.Dispose();

                PeticionCN petCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PeticionCN>(), mLoggerFactory);
                petCN.ActualizarBD();
                petCN.Dispose();

                List<Guid> listaPerfilesInvitados = new List<Guid>();
                foreach (Identidad ident in listaIdentidades)
                {
                    listaPerfilesInvitados.Add(ident.PerfilID);
                }

                ControladorAmigos.AgregarNotificacionInvitacionNuevaAPerfiles(listaPerfilesInvitados);
            }
            else
            {
                status = SendStatus.Ko;
            }

            if (correosYaAgnadidos)
            {
                status = SendStatus.EnviadasAUsuariosYaAgnadidos;
            }

            //switch (mStatus)
            //{
            //    case SendStatus.Ok:
            //        divOk.Visible = true;
            //        divKo.Visible = false;
            //        break;
            //    case SendStatus.EnviadasAUsuariosYaAgnadidos:
            //        divOk.Visible = true;
            //        divKo.Visible = false;
            //        break;
            //    case SendStatus.Ko:
            //        divOk.Visible = false;
            //        divKo.Visible = true;
            //        break;
            //}

            if (listaIdentidades.Count > 0)
            {
                listaInvitacionesPorIdentidad = new Dictionary<Guid, Guid>();
                foreach (Identidad identidad in listaIdentidades)
                {
                    List<AD.EntityModel.Models.Notificacion.Invitacion> filas = notificacionDW.ListaInvitacion.Where(item => item.IdentidadDestinoID.Equals(identidad.Clave)).ToList();
                    AD.EntityModel.Models.Notificacion.Invitacion fila = filas.FirstOrDefault();

                    listaInvitacionesPorIdentidad.Add(fila.InvitacionID, fila.IdentidadDestinoID);
                }
            }

            if (listaInvitacionesPorIdentidad != null && listaInvitacionesPorIdentidad.Count > 0)
            {
                ControladorAmigos.AgregarInvitacionModeloBase(listaInvitacionesPorIdentidad, AD.BASE_BD.PrioridadBase.Alta);
            }

            //this.txtHackInvitados.Text = "";
            //MarcarCorreosNoValidos();

            //pInvitados = new string[0];

            return status;
        }

        /// <summary>
        /// Comprueba si el correo pasado es válido
        /// </summary>
        /// <param name="pCorreo">Correo a comprobar</param>
        /// <returns>True si el correo es válido, False si no lo es.</returns>
        private bool ComprobarCorreoEsValido(string pEmail)
        {
            PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            bool correoPerteneceAProyecto = persCN.ObtenerSiCorreoPerteneceAProyecto(ProyectoSeleccionado.Clave, pEmail);
            persCN.Dispose();

            //Si el correo no pertence al proyecto, entonces es valido.
            return !correoPerteneceAProyecto;
        }



        /// <summary>
        /// Obtiene si hay que mostrar la pestanya de importar contactos por el email.
        /// </summary>
        public bool PestanyaImportarContactosCorreo
        {
            get
            {
                if (!mPestanyaImportarContactosCorreo.HasValue)
                {
                    mPestanyaImportarContactosCorreo = true;
                    List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.PestanyaImportarContactosCorreo.ToString())).ToList();
                    if (busqueda.Count > 0)
                    {
                        mPestanyaImportarContactosCorreo = bool.Parse(busqueda.First().Valor);
                    }
                }
                return mPestanyaImportarContactosCorreo.Value;
            }
        }

        /// <summary>
        /// Devuelve si hay que mostrar el panel con el mensaje que se va a enviar a la hora de importar contactos.
        /// </summary>
        public bool PanelMensajeImportarContactos
        {
            get
            {
                if (!mPanelMensajeImportarContactos.HasValue)
                {
                    mPanelMensajeImportarContactos = true;
                    List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.PanelMensajeImportarContactos.ToString())).ToList();
                    if (busqueda.Count > 0)
                    {
                        mPanelMensajeImportarContactos = bool.Parse(busqueda.First().Valor);
                    }
                }
                return mPanelMensajeImportarContactos.Value;
            }
        }
    }
}
