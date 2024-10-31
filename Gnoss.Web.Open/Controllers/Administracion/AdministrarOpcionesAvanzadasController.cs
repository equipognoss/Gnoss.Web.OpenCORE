using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
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
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Xml;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para administrar las opciones avanzadas de la comunidad
    /// </summary>
    public class AdministrarOpcionesAvanzadasController : ControllerBaseWeb
    {
        public AdministrarOpcionesAvanzadasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
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
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View(PaginaModel);
        }

        /// <summary>
        /// Index para mostrar/cargar la información relativa a la Interacción social de usuarios en la comunidad. Se realiza desde "Comunidad -> Integración social
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
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
        //[TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
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
            bool isInEcosistemaPlatform = !string.IsNullOrEmpty(RequestParams("ecosistema")) ? (bool.Parse(RequestParams("ecosistema"))) : false;
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

            ControladorOpcionesAvanzadas contrOpcionesAvanzadas = new ControladorOpcionesAvanzadas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

            //Options.CodigoGoogleAnalytics = HttpUtility.UrlDecode(Options.CodigoGoogleAnalytics);
            //Options.ScriptGoogleAnalytics = HttpUtility.UrlDecode(Options.ScriptGoogleAnalytics);

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
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

                    //if (Options.RobotsBusqueda == "") { Options.RobotsBusqueda = null; }

                    HttpResponseMessage resultado = InformarCambioAdministracion("OpcionesAvanzadas", JsonConvert.SerializeObject(Options, Newtonsoft.Json.Formatting.Indented));

                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
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
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
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
            ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

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

            
            if(parametroProyecto != null)
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
                ControladorOpcionesAvanzadas contrOpcionesAvanzadas = new ControladorOpcionesAvanzadas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                contrOpcionesAvanzadas.CargarBuzonCorreo(Options);
                HttpResponseMessage resultado = InformarCambioAdministracion("OpcionesAvanzadas", JsonConvert.SerializeObject(Options, Newtonsoft.Json.Formatting.Indented));
                if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                {
                    throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                }
            }

            return GnossResultOK();
        }

		/// <summary>
		/// Validar el correo electrónico del usuario
		/// </summary>
		/// <returns>ActionResult</returns>
		[HttpPost]
		[TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
		public ActionResult ValidarCorreo(AdministrarOpcionesAvanzadasViewModel Options)
		{
           
			try
			{

				if (Options.ConfiguracionCorreo.Email == null || Options.ConfiguracionCorreo.SMTP == null || Options.ConfiguracionCorreo.User == null || Options.ConfiguracionCorreo.Destinatario == null)//validar todos los campos
				{
					string error = UtilIdiomas.GetText("COMADMININFOGENERAL", "GADGETSINRELLENAR");
					return GnossResultERROR(error);
				}
				else
				{
                    //Verificar que el correo configurado funciona
                    UtilCorreo gestorCorreo = new UtilCorreo(Options.ConfiguracionCorreo.SMTP, Options.ConfiguracionCorreo.Port, Options.ConfiguracionCorreo.User, Options.ConfiguracionCorreo.Password, Options.ConfiguracionCorreo.SSL);
                    Guid notifId = new Guid(new byte[16]);
                    gestorCorreo.EnviarCorreo(Options.ConfiguracionCorreo.Destinatario, Options.ConfiguracionCorreo.Email, null, null, null,"Prueba","El correo electrónico configurado funciona",false, notifId);

                    //guardar los datos del buzon
                    GuardarBuzonCorreo(Options);
					return GnossResultOK("El correo electrónico se ha configurado con éxito");
				}
			}
			catch (Exception ex)
			{
				return GnossResultERROR(ex.Message);
			}
		}

			/// <summary>
			/// Guardar solo la información relativa a la configuráción del buzón del usuario. Se realiza desde "Configuración -> Buzón de correo
			/// </summary>
			/// <returns>ActionResult</returns>
			[HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        //[TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult GuardarBuzonCorreo(AdministrarOpcionesAvanzadasViewModel Options)
        {
            GuardarLogAuditoria();
            GuardarDatosConfiguracionCorreo(Options);

            return GnossResultOK();
        }

        private void CorreoIntegracion(AdministrarOpcionesAvanzadasViewModel options)
        {
            if(string.IsNullOrEmpty(options.ConfiguracionCorreo.Email) && string.IsNullOrEmpty(options.ConfiguracionCorreo.SMTP) && string.IsNullOrEmpty(options.ConfiguracionCorreo.User) && string.IsNullOrEmpty(options.ConfiguracionCorreo.Password))
            {
                options.ConfiguracionCorreo = null;
            }
        }

        private void GuardarDatosConfiguracionCorreo(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null);
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

                    ControladorOpcionesAvanzadas contrOpcionesAvanzadas = new ControladorOpcionesAvanzadas(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    mPaginaModel = contrOpcionesAvanzadas.CargarOpcionesAvanzadas(EsAdministracionEcosistema);

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
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
