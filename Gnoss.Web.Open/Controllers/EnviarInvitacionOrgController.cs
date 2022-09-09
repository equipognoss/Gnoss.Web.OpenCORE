using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Amigos;
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
using System.Data;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    /// <summary>
    /// Enumeración para representar el estado del envio de invitaciones
    /// </summary>
    public enum SendStatus
    {
        Ok, Ko, EnviadasAUsuariosYaAgnadidos
    }

    /// <summary>
    /// View model de la pagina de "Enviar invitación a la comunidad" 
    /// </summary>
    [Serializable]
    public class SendInvitationOrgViewModel
    {
        /// <summary>
        /// Indica si permite enviar invitaciones a tus contactos
        /// </summary>
        public bool AllowInviteContacts { get; set; }
        /// <summary>
        /// Indica si permite enviar invitaciones por email
        /// </summary>
        public bool AllowInviteEmail { get; set; }
        /// <summary>
        /// Indica si se puede personalizar el mensaje de invitación a la comunidad
        /// </summary>
        public bool AllowPersonlizeMessage { get; set; }
        /// <summary>
        /// Mensaje de invitación a la comunidad
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Identificadores y emails de los invitados, separados por comas
        /// </summary>
        public string Guests { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EnviarInvitacionOrgController : ControllerBaseWeb
    {
        #region Miembros

        /// <summary>
        /// Obtiene si hay que mostrar la pestanya de importar contactos por el email.
        /// </summary>
        private bool? mPestanyaImportarContactosCorreo = null;

        /// <summary>
        /// Devuelve si hay que mostrar el panel con el mensaje que se va a enviar a la hora de importar contactos.
        /// </summary>
        private bool? mPanelMensajeImportarContactos = null;

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestorIdentidades;
        #endregion

        public EnviarInvitacionOrgController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if ((mControladorBase.UsuarioActual.EsIdentidadInvitada) || (IdentidadActual.TrabajaConOrganizacion && !EsIdentidadActualAdministradorOrganizacion) || (IdentidadActual.TrabajaConClase && !IdentidadActual.EstaAdministrandoClase))
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            EliminarPersonalizacionVistas();
            CargarModelo();
            return View(PaginaModel);
        }
     
        /// <summary>
        /// Método que cargará la vista (anteriormente Index) en un modal dinámico mediante la llamada "load-modal" 
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadModal()
        {
            if ((mControladorBase.UsuarioActual.EsIdentidadInvitada) || (IdentidadActual.TrabajaConOrganizacion && !EsIdentidadActualAdministradorOrganizacion) || (IdentidadActual.TrabajaConClase && !IdentidadActual.EstaAdministrandoClase))
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            EliminarPersonalizacionVistas();
            CargarModelo();            
            return GnossResultHtml("../UsuariosOrganizacion/_modal-views/_invite-organization", PaginaModel);
        }



        [HttpPost]
        public ActionResult GuardarCambios(SendInvitationOrgViewModel pEnviarInvitacionModel)
        {
            if ((mControladorBase.UsuarioActual.EsIdentidadInvitada) || (IdentidadActual.TrabajaConOrganizacion && !EsIdentidadActualAdministradorOrganizacion) || (IdentidadActual.TrabajaConClase && !IdentidadActual.EstaAdministrandoClase))
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            string notas = UtilIdiomas.GetText("INVITACIONES", "TEXTODEFECTOINVITACIONCOMUNIDAD", ProyectoSeleccionado.FilaProyecto.Nombre, UtilCadenas.EliminarEnlacesDeHtml(UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto)), NombreProyectoEcosistema);

            if (pEnviarInvitacionModel.Guests == null)
            {
                return GnossResultERROR(UtilIdiomas.GetText("INVITACIONES", "SINDESTINATARIOS"));
            }

            string[] correos = pEnviarInvitacionModel.Guests.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            if (pEnviarInvitacionModel.Message != null)
            {
                notas = HttpUtility.UrlDecode(pEnviarInvitacionModel.Message);
            }

            SendStatus status = EnviarInvitaciones(correos, notas);

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
                return GnossResultOK(mensaje);
            }
        }

        #region Métodos privados

        private void CargarModelo()
        {
            PaginaModel = new SendInvitationOrgViewModel();

            if (!IdentidadActual.EstaAdministrandoClase)
            {
                PaginaModel.Message = UtilCadenas.EliminarEnlacesDeHtml(UtilIdiomas.GetText("INVITACIONES", "TEXTODEFECTOINVITACIONORGANIZACION", IdentidadActual.OrganizacionPerfil.Nombre, NombreProyectoEcosistema));
            }
            else
            {
                PaginaModel.Message = UtilCadenas.EliminarEnlacesDeHtml(UtilIdiomas.GetText("INVITACIONES", "TEXTODEFECTOINVITACIONCLASE", NombreProyectoEcosistema, "", IdentidadActual.OrganizacionPerfil.Nombre, IdentidadActual.Nombre(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave))));
            }

            PaginaModel.AllowInviteContacts = true;
            PaginaModel.AllowInviteEmail = true;
            PaginaModel.AllowPersonlizeMessage = true;
            if (!ParametrosGeneralesRow.InvitacionesPorContactoDisponibles)
            {
                PaginaModel.AllowInviteContacts = false;
            }
            if (!PestanyaImportarContactosCorreo)
            {
                PaginaModel.AllowInviteEmail = false;
            }
            if (!PanelMensajeImportarContactos)
            {
                PaginaModel.AllowPersonlizeMessage = false;
            }

            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (!amigosCL.EstanCargadosAmigos(IdentidadActual.Clave, false, false, false, false, false))
            {
                ControladorAmigos contAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                contAmigos.CargarAmigos(IdentidadActual, false);
            }
        }

        /// <summary>
        /// Se produce al pulsar el botón de enviar las invitaciones
        /// </summary>
        private SendStatus EnviarInvitaciones(string[] pInvitados, string pMensaje)
        {
            SendStatus status;

            Dictionary<Guid, Guid> listaInvitacionesPorIdentidad = null;

            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
            GestionNotificaciones GestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrg = orgCN.ObtenerOrganizacionPorID(IdentidadActual.OrganizacionID.Value).ListaOrganizacion.FirstOrDefault();

            List<Identidad> listaIdentidades = new List<Identidad>();
            List<string> listaEmails = new List<string>();
            bool correosYaAgnadidos = false;
            List<Guid> listaIdentContactos = new List<Guid>();

            foreach (string invitado in pInvitados)
            {
                string inv = invitado.Trim();

                if (inv.Contains("@"))
                {
                    if (ComprobarCorreoEsValido(inv, filaOrg.NombreCorto))
                    {
                        listaEmails.Add(inv);
                    }
                    else
                    {
                        correosYaAgnadidos = true;
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
                    if (ComprobarCorreoEsValido(gestorIdentidadesContactos.ListaIdentidades[cor].Email, filaOrg.NombreCorto))
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
                listaInvitacionesPorIdentidad = GestorNotificaciones.AgregarNotificacionInvitacionUsuarioAOrg(IdentidadActual.OrganizacionID.Value, filaOrg.Nombre, pMensaje, IdentidadActual, DateTime.Now, listaIdentidades, this.BaseURLIdioma, true, filaOrg.ModoPersonal, true, UtilIdiomas.LanguageCode);
            }

            if (listaEmails.Count > 0)
            {
                string urlEnlace = GnossUrlsSemanticas.GetURLAceptarInvitacion(BaseURLIdioma, UtilIdiomas);
                GestorNotificaciones.AgregarNotificacionInvitacionExternoAOrg(IdentidadActual.OrganizacionID.Value, filaOrg.Nombre, pMensaje, IdentidadActual.Nombre(IdentidadUsuarioActualEnProyecto(ProyectoSeleccionado.Clave)), DateTime.Now, listaEmails, urlEnlace, filaOrg.ModoPersonal, UtilIdiomas.LanguageCode);
            }

            status = SendStatus.Ok;
            if (listaEmails.Count > 0 || listaIdentidades.Count > 0)
            {
                notificacionCN.ActualizarNotificacion();
                notificacionCN.Dispose();

                List<Guid> listaPerfilesInvitados = new List<Guid>();
                foreach (Identidad ident in listaIdentidades)
                {
                    listaPerfilesInvitados.Add(ident.PerfilID);
                }

                ControladorAmigos.AgregarNotificacionInvitacionNuevaAPerfiles(listaPerfilesInvitados);

                if (listaEmails.Count > 0)
                {
                    PeticionCN petCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    petCN.ActualizarBD();
                    petCN.Dispose();
                }
            }
            else
            {
                status = SendStatus.Ko;
            }

            if (correosYaAgnadidos)
            {
                status = SendStatus.EnviadasAUsuariosYaAgnadidos;
            }

            if (listaInvitacionesPorIdentidad != null && listaInvitacionesPorIdentidad.Count > 0)
            {
                ControladorAmigos.AgregarInvitacionModeloBase(listaInvitacionesPorIdentidad, PrioridadBase.Alta);
            }

            return status;
        }

        /// <summary>
        /// Carga de entre mis contactos a aquellos a los que puedo invitar a formar parte de la organización (los que todavía no son miembros)
        /// </summary>
        private void CargarContactosPuedoInvitar()
        {
            IdentidadCN IdenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperIdentidad idenDW = IdenCN.ObtenerIdentidadesAmigosPuedoInvitarOrganizacion(IdentidadActual.OrganizacionID.Value, IdentidadActual.IdentidadMyGNOSS.Clave, " ");
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            DataWrapperOrganizacion orgDW = new DataWrapperOrganizacion();
            ObtenerPersonasYOrgDeIdentidades(idenDW, dataWrapperPersona, orgDW);
            GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
            GestionOrganizaciones gestorOrg = new GestionOrganizaciones(orgDW, mLoggingService, mEntityContext);
            mGestorIdentidades = new GestionIdentidades(idenDW, gestorPersonas, gestorOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Comprueba que el correo no pertenece ya a la organización
        /// </summary>
        /// <param name="pEmail"></param>
        /// <param name="pNombreCortoOrg"></param>
        /// <returns></returns>
        private bool ComprobarCorreoEsValido(string pEmail, string pNombreCortoOrg)
        {
            string[] emails = new string[] { pEmail };
            PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<string, List<Guid>> dic = persCN.ObtenerUsuarioIDPerfilIDPorEmailYOrganizacion(pNombreCortoOrg, emails);
            persCN.Dispose();

            return (dic == null || dic.Keys.Count == 0);
        }

        #endregion

        #region Propiedades

        SendInvitationOrgViewModel PaginaModel { get; set; }

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

        /// <summary>
        /// Obtiene o establece el gestor de identidades de mis contactos
        /// </summary>
        public GestionIdentidades GestorIdentidadesContactos
        {
            get
            {
                if (mGestorIdentidades == null)
                {
                    CargarContactosPuedoInvitar();
                }
                return mGestorIdentidades;
            }
            set
            {
                mGestorIdentidades = value;
            }
        }

        
        #endregion
    }
}