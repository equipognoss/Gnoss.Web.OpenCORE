using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text.Json;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para administrar las opciones avanzadas de la comunidad
    /// </summary>
    public class AdministrarOpcionesAvanzadasController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarOpcionesAvanzadasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IWebHostEnvironment env,IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarOpcionesAvanzadasController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env,utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros

        private AdministrarOpcionesAvanzadasViewModel mPaginaModel = null;

        #endregion

        #region Metodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clases para el body del Layout
            ViewBag.BodyClassPestanya = "edicionOpcionesAvanzadas comunidad";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_OpcionesAvanzadas;
            // Establecer el título para el header de DevTools            
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("ADMINISTRACIONSEMANTICA", "COMUNIDAD");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "OPCIONESAVANZADAS");

            return View(PaginaModel);
        }

        /// <summary>
        /// Index para mostrar/cargar la información relativa a la Interacción social de usuarios en la comunidad. Se realiza desde "Comunidad -> Integración social
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]        
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarInteraccionesSociales } })]
		public ActionResult IndexInteraccionSocial()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clases para el body del Layout
            ViewBag.BodyClassPestanya = "edicionInteraccionSocial comunidad";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_InteraccionSocial;
            // Establecer el título para el header de DevTools            
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("ADMINISTRACIONSEMANTICA", "COMUNIDAD");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "INTERACCIONSOCIAL");

            // Activar la visualización del icono de la documentación de la sección
            ViewBag.showDocumentationByDefault = "true";
            // Indica si sólo se desea visualizar la documentación por secciones (Ej: Mostrar la documentación en el modal con el contenido desplegado/plegado)
            ViewBag.showDocumentationSection = "true";

            // Devolver la página para la gestión de certificados
            return View("../AdministrarOpcionesAvanzadas/Index_InteraccionSocial", PaginaModel);
        }

        /// <summary>
        /// Index para mostrar/cargar la información relativa al buzón de correo de la comunidad. Se realiza desde "Configuración -> Buzón de correo
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarBuzonDeCorreo } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarBuzonDeCorreoEcosistema } })]
		public ActionResult IndexBuzonCorreo()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clases para el body del Layout
            ViewBag.BodyClassPestanya = "configuracion correo-configuracion";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_BuzonDeCorreo;
            // Establecer el título para el header de DevTools            
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "BUZONDECORREO");

            // Controlar si es o no del ecosistema            
            bool isInEcosistemaPlatform = !string.IsNullOrEmpty(RequestParams("ecosistema")) ? bool.Parse(RequestParams("ecosistema")) : false;
            if (isInEcosistemaPlatform)
            {
                ViewBag.isInEcosistemaPlatform = "true";
            }

            // Devolver la página
            return View("../AdministrarBuzonCorreo/Index", PaginaModel);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(LimitarPeticionesAdministracion), Arguments = new object[] { 30, 120, 15 })]
        public ActionResult Guardar(AdministrarOpcionesAvanzadasViewModel Options)
        {
            GuardarLogAuditoria();
            bool iniciado = false;
            try
            {
                iniciado = HayIntegracionContinua;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                return GnossResultERROR("Contacte con el administrador del Proyecto, no es posible atender la petición.");
            }

            ControladorOpcionesAvanzadas contrOpcionesAvanzadas = new ControladorOpcionesAvanzadas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorOpcionesAvanzadas>(), mLoggerFactory);

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
            bool transaccionIniciada = false;

            try
            {
                mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);

                contrOpcionesAvanzadas.GuardarOpcionesAvanzadas(Options);
                if (iniciado)
                {
                    CorreoIntegracion(Options);

                    if (Options.GruposVisibilidadAbierto == null) { Options.GruposVisibilidadAbierto = new Dictionary<Guid, string>(); }

                    if (Options.PestanyasSeleccionadas.Value.Equals(Guid.Empty)) { Options.PestanyasSeleccionadas = null; }

                    HttpResponseMessage resultado = InformarCambioAdministracion("OpcionesAvanzadas", JsonSerializer.Serialize(Options, new JsonSerializerOptions { WriteIndented = true }));

                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new ExcepcionWeb("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }

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

                return GnossResultERROR(ex.Message);
            }

            contrOpcionesAvanzadas.InvalidarCaches();

            return GnossResultOK();
        }

        /// <summary>
        /// Guardar solo la información relativa a la integración social del usuario. Se realiza desde "Comunidad -> Integración social
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarInteraccionesSociales } })]
		[TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(LimitarPeticionesAdministracion), Arguments = new object[] { 30, 120, 15 })]
        public ActionResult GuardarIntegracionSocial(AdministrarOpcionesAvanzadasViewModel Options)
        {
            GuardarLogAuditoria();
            bool iniciado = false;
            try
            {
                iniciado = HayIntegracionContinua;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                return GnossResultERROR("Contacte con el administrador del Proyecto, no es posible atender la petición.");
            }
            ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCN>(), mLoggerFactory);
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);

            ParametroGeneral parametroGeneral = parametroGeneralCN.ObtenerFilaParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
            ParametroProyecto parametroProyecto = parametroCN.ObtenerParametroDeProyecto(ParametroAD.NumeroCaracteresDescripcion, ProyectoSeleccionado.Clave);

            parametroGeneral.PermitirRecursosPrivados = Options.PermitirRecursosPrivados;
            parametroGeneral.CompartirRecursosPermitido = Options.CompartirRecursoPermitido;
            parametroGeneral.SupervisoresAdminGrupos = Options.SupervisoresPuedenAdministrarGrupos;
            parametroGeneral.ComentariosDisponibles = Options.ComentariosDisponibles;
            parametroGeneral.VerVotaciones = Options.MostrarVotaciones;
            parametroGeneral.PermitirVotacionesNegativas = Options.PermitirVotacionesNegativas;
            parametroGeneral.VotacionesDisponibles = Options.VotacionesDisponibles;
            parametroGeneral.InvitacionesDisponibles = Options.InvitacionesDisponibles;


            if (parametroProyecto != null)
            {
                parametroProyecto.Valor = Options.NumeroCaracteresDescripcionSuscripcion;
            }
            else if (!string.IsNullOrEmpty(Options.NumeroCaracteresDescripcionSuscripcion))
            {
                ParametroProyecto parametroProyectoNumCaracDescripSuscrip = new ParametroProyecto();
                parametroProyectoNumCaracDescripSuscrip.OrganizacionID = ProyectoSeleccionado.Organizacion.Clave;
                parametroProyectoNumCaracDescripSuscrip.ProyectoID = ProyectoSeleccionado.Clave;
                parametroProyectoNumCaracDescripSuscrip.Parametro = ParametroAD.NumeroCaracteresDescripcion;
                parametroProyectoNumCaracDescripSuscrip.Valor = Options.NumeroCaracteresDescripcionSuscripcion;
            }

            parametroGeneralCN.Actualizar();

            if (iniciado)
            {
                ControladorOpcionesAvanzadas contrOpcionesAvanzadas = new ControladorOpcionesAvanzadas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorOpcionesAvanzadas>(), mLoggerFactory);
                contrOpcionesAvanzadas.CargarBuzonCorreo(Options);
                HttpResponseMessage resultado = InformarCambioAdministracion("OpcionesAvanzadas", JsonSerializer.Serialize(Options, new JsonSerializerOptions { WriteIndented = true }));
                if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                {
                    throw new ExcepcionWeb("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                }
            }

            return GnossResultOK();
        }

		/// <summary>
		/// Comprueba la configuración del buzón de correo enviando un correo de prueba y, si el envío funciona, la guarda
		/// </summary>
		/// <returns>ActionResult</returns>
		[HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarBuzonDeCorreo } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarBuzonDeCorreoEcosistema } })]
		public ActionResult ValidarCorreo(AdministrarOpcionesAvanzadasViewModel Options)
		{
			ConfiguradorCorreo configuracionCorreo = Options?.ConfiguracionCorreo;

			// Comprobar que todos los campos del formulario llegan completos y con un valor válido
			string errorValidacion = ValidarDatosConfiguracionCorreo(configuracionCorreo);
			if (!string.IsNullOrEmpty(errorValidacion))
			{
				return GnossResultERROR(errorValidacion);
			}

			try
			{
				// Verificar que el correo configurado funciona enviando un correo de prueba al destinatario indicado
				UtilCorreo gestorCorreo = new UtilCorreo(configuracionCorreo.SMTP.Trim(), configuracionCorreo.Port, configuracionCorreo.User.Trim(), configuracionCorreo.Password, configuracionCorreo.SSL);
				gestorCorreo.EnviarCorreo(configuracionCorreo.Destinatario.Trim(), configuracionCorreo.Email.Trim(), null, null, null, UtilIdiomas.GetText("DEVTOOLS", "ASUNTOCORREOPRUEBA"), UtilIdiomas.GetText("DEVTOOLS", "CUERPOCORREOPRUEBA"), false, Guid.NewGuid());
			}
			catch (Exception ex)
			{
				GuardarLogError(ex, $"Error al enviar el correo de prueba del buzón del proyecto {ProyectoSeleccionado.Clave} a través del servidor {configuracionCorreo.SMTP}:{configuracionCorreo.Port}.");
				return GnossResultERROR(ObtenerMensajeErrorEnvioCorreo(ex, configuracionCorreo));
			}

			try
			{
				// El correo de prueba se ha enviado, guardar los datos del buzón
				GuardarLogAuditoria();
				GuardarDatosConfiguracionCorreo(Options);
			}
			catch (Exception ex)
			{
				GuardarLogError(ex, $"Error al guardar la configuración del buzón de correo del proyecto {ProyectoSeleccionado.Clave}.");
				return GnossResultERROR(UtilIdiomas.GetText("DEVTOOLS", "ERRORGUARDARCONFIGURACIONCORREO"));
			}

			return GnossResultOK(UtilIdiomas.GetText("DEVTOOLS", "CORREOCONFIGURADOCONEXITO"));
		}

		/// <summary>
		/// Comprueba que todos los campos de la configuración del buzón de correo están rellenos y son válidos
		/// </summary>
		/// <param name="pConfiguracionCorreo">Configuración del buzón recibida desde el formulario</param>
		/// <returns>El mensaje del primer error encontrado, o una cadena vacía si la configuración es válida</returns>
		private string ValidarDatosConfiguracionCorreo(ConfiguradorCorreo pConfiguracionCorreo)
		{
			if (pConfiguracionCorreo == null)
			{
				return UtilIdiomas.GetText("DEVTOOLS", "CORREOBUZONOBLIGATORIO");
			}

			// Correo electrónico de la comunidad, que actúa como remitente de los correos
			if (string.IsNullOrWhiteSpace(pConfiguracionCorreo.Email))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "CORREOBUZONOBLIGATORIO");
			}
			if (!EsDireccionCorreoValida(pConfiguracionCorreo.Email))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "CORREOBUZONNOVALIDO");
			}

			// Servidor SMTP
			if (string.IsNullOrWhiteSpace(pConfiguracionCorreo.SMTP))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "SMTPOBLIGATORIO");
			}

			// Puerto del servidor. Se almacena como short, así que un valor fuera de 1-32767 llega aquí como 0
			if (pConfiguracionCorreo.Port <= 0)
			{
				return UtilIdiomas.GetText("DEVTOOLS", "PUERTONOVALIDO");
			}

			// Usuario y contraseña de la cuenta. La contraseña es necesaria para poder enviar el correo de prueba
			if (string.IsNullOrWhiteSpace(pConfiguracionCorreo.User))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "USUARIOCORREOOBLIGATORIO");
			}
			if (string.IsNullOrEmpty(pConfiguracionCorreo.Password))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "CONTRASENYACORREOOBLIGATORIA");
			}

			// Tipo de servidor de envío
			if (string.IsNullOrWhiteSpace(pConfiguracionCorreo.Type))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "TIPOSERVIDOROBLIGATORIO");
			}

			// Email de sugerencias
			if (string.IsNullOrWhiteSpace(pConfiguracionCorreo.SuggestEmail))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "EMAILSUGERENCIASOBLIGATORIO");
			}
			if (!EsDireccionCorreoValida(pConfiguracionCorreo.SuggestEmail))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "EMAILSUGERENCIASNOVALIDO");
			}

			// Destinatario del correo de prueba, que se pide en el modal de validación
			if (string.IsNullOrWhiteSpace(pConfiguracionCorreo.Destinatario))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "EMAILPRUEBAOBLIGATORIO");
			}
			if (!EsDireccionCorreoValida(pConfiguracionCorreo.Destinatario))
			{
				return UtilIdiomas.GetText("DEVTOOLS", "EMAILPRUEBANOVALIDO");
			}

			return string.Empty;
		}

		/// <summary>
		/// Comprueba que una dirección de correo tiene un formato válido y que es una única dirección
		/// </summary>
		/// <param name="pDireccion">Dirección de correo a comprobar</param>
		/// <returns>TRUE si la dirección es válida</returns>
		private static bool EsDireccionCorreoValida(string pDireccion)
		{
			pDireccion = pDireccion.Trim();

			// MailAddress admite formatos como "Nombre <buzon@dominio.com>" o listas de direcciones, que aquí no son válidos
			if (pDireccion.Contains(',') || pDireccion.Contains(';') || pDireccion.Contains(' ') || pDireccion.Contains('<') || pDireccion.Contains('>'))
			{
				return false;
			}

			return MailAddress.TryCreate(pDireccion, out MailAddress direccion) && direccion.Host.Contains('.');
		}

		/// <summary>
		/// Traduce el error producido al enviar el correo de prueba a un mensaje que indique al administrador qué debe revisar.
		/// UtilCorreo envuelve el error real en una GnossSmtpException cuyo mensaje incluye el asunto y el cuerpo del correo,
		/// así que no se puede mostrar tal cual al usuario
		/// </summary>
		/// <param name="pExcepcion">Excepción producida durante el envío</param>
		/// <param name="pConfiguracionCorreo">Configuración del buzón que se estaba probando</param>
		/// <returns>Mensaje de error para el administrador</returns>
		private string ObtenerMensajeErrorEnvioCorreo(Exception pExcepcion, ConfiguradorCorreo pConfiguracionCorreo)
		{
			string smtp = pConfiguracionCorreo.SMTP.Trim();
			string puerto = pConfiguracionCorreo.Port.ToString();

			// Quedarse con el error original que envuelve UtilCorreo
			Exception excepcion = pExcepcion;
			if (excepcion is GnossSmtpException && excepcion.InnerException != null)
			{
				excepcion = excepcion.InnerException;
			}

			string detalle = excepcion.Message;

			// El servidor rechaza el destinatario del correo de prueba
			if (excepcion is SmtpFailedRecipientsException excepcionDestinatarios)
			{
				string destinatarioFallido = excepcionDestinatarios.InnerExceptions.Length > 0 ? excepcionDestinatarios.InnerExceptions[0].FailedRecipient : pConfiguracionCorreo.Destinatario;
				return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPDESTINATARIO"), destinatarioFallido, detalle);
			}
			if (excepcion is SmtpFailedRecipientException excepcionDestinatario)
			{
				return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPDESTINATARIO"), excepcionDestinatario.FailedRecipient ?? pConfiguracionCorreo.Destinatario, detalle);
			}

			if (excepcion is SmtpException excepcionSmtp)
			{
				// No se ha llegado a hablar con el servidor: el nombre, el puerto o la red no son correctos
				if (excepcionSmtp.InnerException is SocketException || excepcionSmtp.InnerException is TimeoutException)
				{
					return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPCONEXION"), smtp, puerto, excepcionSmtp.InnerException.Message);
				}

				switch (excepcionSmtp.StatusCode)
				{
					case SmtpStatusCode.MustIssueStartTlsFirst:
						// El servidor exige conexión segura y no se ha marcado "Usar servidor seguro"
						return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPSSL"), smtp, detalle);

					case SmtpStatusCode.ClientNotPermitted:
					case SmtpStatusCode.TransactionFailed:
						// Credenciales rechazadas o cuenta sin permiso para enviar
						return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPAUTENTICACION"), smtp, pConfiguracionCorreo.User.Trim(), detalle);

					case SmtpStatusCode.MailboxNameNotAllowed:
					case SmtpStatusCode.MailboxUnavailable:
					case SmtpStatusCode.MailboxBusy:
						// El servidor no acepta la dirección desde la que se envía
						return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPREMITENTE"), pConfiguracionCorreo.Email.Trim(), pConfiguracionCorreo.User.Trim(), detalle);

					case SmtpStatusCode.ServiceNotAvailable:
					case SmtpStatusCode.ServiceClosingTransmissionChannel:
						return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPSERVICIONODISPONIBLE"), smtp, detalle);

					case SmtpStatusCode.GeneralFailure:
						return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPCONEXION"), smtp, puerto, detalle);
				}

				return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORENVIOCORREOPRUEBA"), detalle);
			}

			// No se ha podido abrir la conexión con el servidor
			if (excepcion is SocketException || excepcion is TimeoutException)
			{
				return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPCONEXION"), smtp, puerto, detalle);
			}

			// El certificado del servidor no se ha podido validar, normalmente por usar SSL donde no corresponde
			if (excepcion is AuthenticationException)
			{
				return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPSSL"), smtp, detalle);
			}

			// MailAddress rechaza alguna de las direcciones indicadas
			if (excepcion is FormatException || excepcion is ArgumentException)
			{
				return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORSMTPDIRECCIONNOVALIDA"), detalle);
			}

			return ComponerTexto(UtilIdiomas.GetText("DEVTOOLS", "ERRORENVIOCORREOPRUEBA"), detalle);
		}

		/// <summary>
		/// Sustituye los parámetros (@1@, @2@...) de un texto de recursos.
		/// No se usa la sobrecarga de GetText con parámetros porque resuelve cada valor como un texto multiidioma,
		/// lo que altera valores como direcciones de correo o mensajes devueltos por el servidor
		/// </summary>
		/// <param name="pTexto">Texto de recursos con los parámetros sin sustituir</param>
		/// <param name="pParametros">Valores a sustituir, en orden</param>
		/// <returns>Texto con los parámetros sustituidos</returns>
		private static string ComponerTexto(string pTexto, params string[] pParametros)
		{
			if (string.IsNullOrEmpty(pTexto))
			{
				return pTexto;
			}

			for (int i = 0; i < pParametros.Length; i++)
			{
				pTexto = pTexto.Replace($"@{i + 1}@", pParametros[i]);
			}

			return pTexto;
		}

		/// <summary>
		/// Guardar solo la información relativa a la configuráción del buzón del usuario. Se realiza desde "Configuración -> Buzón de correo
		/// </summary>
		/// <returns>ActionResult</returns>
		[HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarBuzonDeCorreo } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarBuzonDeCorreoEcosistema } })]
		public ActionResult GuardarBuzonCorreo(AdministrarOpcionesAvanzadasViewModel Options)
        {
            GuardarLogAuditoria();
            GuardarDatosConfiguracionCorreo(Options);

            return GnossResultOK();
        }

        private static void CorreoIntegracion(AdministrarOpcionesAvanzadasViewModel options)
        {
            if (string.IsNullOrEmpty(options.ConfiguracionCorreo.Email) && string.IsNullOrEmpty(options.ConfiguracionCorreo.SMTP) && string.IsNullOrEmpty(options.ConfiguracionCorreo.User) && string.IsNullOrEmpty(options.ConfiguracionCorreo.Password))
            {
                options.ConfiguracionCorreo = null;
            }
        }

        private void GuardarDatosConfiguracionCorreo(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
            ConfiguracionEnvioCorreo filaConfiguracionEnvioCorreo = paramCN.ObtenerFilaConfiguracionEnvioCorreo(ProyectoSeleccionado.Clave);

            bool existeConfiguracionAnterior = filaConfiguracionEnvioCorreo != null;
            if (!existeConfiguracionAnterior)
            {
                filaConfiguracionEnvioCorreo = new ConfiguracionEnvioCorreo();
            }

            if (pOptions.ConfiguracionCorreo != null && !string.IsNullOrEmpty(pOptions.ConfiguracionCorreo.Email))
            {
                if (!existeConfiguracionAnterior || !string.IsNullOrEmpty(pOptions.ConfiguracionCorreo.Password))
                {
                    filaConfiguracionEnvioCorreo.clave = pOptions.ConfiguracionCorreo.Password;
                }

                filaConfiguracionEnvioCorreo.ProyectoID = ProyectoSeleccionado.Clave;
                filaConfiguracionEnvioCorreo.email = pOptions.ConfiguracionCorreo.Email;
                filaConfiguracionEnvioCorreo.smtp = pOptions.ConfiguracionCorreo.SMTP;
                filaConfiguracionEnvioCorreo.puerto = pOptions.ConfiguracionCorreo.Port;
                filaConfiguracionEnvioCorreo.usuario = pOptions.ConfiguracionCorreo.User;
                filaConfiguracionEnvioCorreo.tipo = pOptions.ConfiguracionCorreo.Type;
                filaConfiguracionEnvioCorreo.SSL = pOptions.ConfiguracionCorreo.SSL;
                filaConfiguracionEnvioCorreo.emailsugerencias = pOptions.ConfiguracionCorreo.SuggestEmail;

                paramCN.GuardarFilaConfiguracionEnvioCorreo(filaConfiguracionEnvioCorreo, !existeConfiguracionAnterior);
            }
            else if (existeConfiguracionAnterior)
            {
                paramCN.BorrarFilaConfiguracionEnvioCorreo(ProyectoSeleccionado.Clave);

            }
            paramCN.Dispose();
        }

        #endregion

        #region Propiedades

        private AdministrarOpcionesAvanzadasViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {

                    ControladorOpcionesAvanzadas contrOpcionesAvanzadas = new ControladorOpcionesAvanzadas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorOpcionesAvanzadas>(), mLoggerFactory);
                    mPaginaModel = contrOpcionesAvanzadas.CargarOpcionesAvanzadas(EsAdministracionEcosistema);

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mPaginaModel.PestanyasDeBusqueda = proyCN.ObtenerPestanyasProyectoNombre(ProyectoSeleccionado.Clave);
                    mPaginaModel.ProyectosConOntologias = proyCN.ObtenerProyectosConOntologiasAdministraUsuario(mControladorBase.UsuarioActual.UsuarioID);
                    proyCN.Dispose();
                }
                return mPaginaModel;
            }
        }

        #endregion
    }
}
