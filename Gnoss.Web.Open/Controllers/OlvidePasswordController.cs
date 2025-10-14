using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class OlvidePasswordController : ControllerBaseWeb
    {

        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public OlvidePasswordController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<OlvidePasswordController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;

        }

        [HttpGet]
        public ActionResult Index()
        {
            if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                {
                    return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
                }
                else
                {
                    return new RedirectResult(ProyectoSeleccionado.UrlPropia(UtilIdiomas.LanguageCode));
                }
            }

            if (!string.IsNullOrEmpty(RequestParams("confirmado")) && RequestParams("confirmado") == "true")
            {
                return View("Confirmado");
            }

            OlvidePasswordViewModel paginaModel = new OlvidePasswordViewModel();

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {

                paginaModel.Url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "OLVIDEPASSWORD") + "/change-request";
            }
            else
            {
                paginaModel.Url = BaseURLIdioma + "/" + UtilIdiomas.GetText("URLSEM", "OLVIDEPASSWORD") + "/change-request";
            }

            paginaModel.Login = "";
            if (!string.IsNullOrEmpty(RequestParams("login")))
            {
                paginaModel.Login = RequestParams("login");
            }

            return View(paginaModel);
        }

        [HttpPost]
        public ActionResult ChangeRequest(string User)
        {
            if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            string login = User;

            if (!login.Trim().Equals(string.Empty))
            {
                try
                {
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    DataWrapperUsuario dataWrapperUsuario = usuarioCN.ObtenerUsuarioPorLoginOEmail(login, false);

                    string urlBase = $"{BaseURLIdioma}/";

                    if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto || EsEcosistemaSinMetaProyecto)
                    {
                        urlBase = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto)}/";
                    }

                    if (dataWrapperUsuario.ListaUsuario.Count > 0)
                    {
                        AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = dataWrapperUsuario.ListaUsuario.First();
                        DataWrapperUsuario userDW = usuarioCN.ObtenerFilaUsuarioVincRedSocialPorUsuarioID(filaUsuario.UsuarioID);

                        if (userDW.ListaUsuarioVinculadoLoginRedesSociales.Count(us => us.TipoRedSocial.Equals((short)TipoRedSocialLogin.Santillana)) > 0 )
                        {
                            //Usuario que pertenece a santillana connect y estamos en una comunidad en la que se ha registrado con el Login de santillana connect
                            return GnossResultERROR(UtilIdiomas.GetText("CAMBIOPASS", "SANTILLANACONNECT"));
                        }
                        else
                        {
                            DataWrapperPeticion peticionDW = new DataWrapperPeticion();
                            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
                            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
                            filaPeticion.FechaPeticion = DateTime.Now;
                            filaPeticion.PeticionID = Guid.NewGuid();
                            filaPeticion.UsuarioID = filaUsuario.UsuarioID;
                            filaPeticion.Tipo = (int)TipoPeticion.CambioPassword;

                            peticionDW.ListaPeticion.Add(filaPeticion);
                            mEntityContext.Peticion.Add(filaPeticion);

                            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                            DataWrapperPersona dataWrapperPersona = personaCN.ObtenerPersonaPorUsuario(filaUsuario.UsuarioID);

                            if (dataWrapperPersona.ListaPersona.Count > 0)
                            {
                                GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
                                AD.EntityModel.Models.PersonaDS.Persona filaPersona = dataWrapperPersona.ListaPersona.FirstOrDefault();

                                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);

                                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);


                                string urlCambioPassword = $"{urlBase}{UtilIdiomas.GetText("URLSEM", "CAMBIARPASSWORD")}/{UtilIdiomas.GetText("URLSEM", "PETICION")}/{filaPeticion.PeticionID}/{UtilIdiomas.GetText("URLSEM", "USUARIO")}/{filaUsuario.UsuarioID}";

                                gestorNotificaciones.AgregarNotificacionPeticionCambioPassword(gestorPersonas.ListaPersonas[filaPersona.PersonaID], urlCambioPassword, ProyectoSeleccionado, UtilIdiomas.LanguageCode);

                                //mEntityContext.SaveChanges();
                                notificacionCN.ActualizarNotificacion(mAvailableServices);

                                return GnossResultUrl($"{urlBase}{UtilIdiomas.GetText("URLSEM", "OLVIDEPASSWORD")}/{UtilIdiomas.GetText("URLSEM", "CONFIRMADO")}");
                            }
                            else
                            {
                                return GnossResultERROR(UtilIdiomas.GetText("SOLICITARCAMBIOPASSWORD", "NOPOSIBLECAMBIO", NombreProyectoEcosistema));
                            }
                        }
                    }
                    else //No hay que revelar que el usaurio no existe
                    {
                        return GnossResultUrl($"{urlBase}{UtilIdiomas.GetText("URLSEM", "OLVIDEPASSWORD")}/{UtilIdiomas.GetText("URLSEM", "CONFIRMADO")}");
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(mLoggingService.DevolverCadenaError(ex, "1.0.0.0"));
                    return GnossResultERROR(UtilIdiomas.GetText("SOLICITARCAMBIOPASSWORD", "NOPOSIBLECAMBIO", NombreProyectoEcosistema));
                }
            }
            else
            {
                return GnossResultERROR(UtilIdiomas.GetText("SOLICITARCAMBIOPASSWORD", "INTRODUCENOMBREUSU"));
            }
        }

        #region Propiedades

        /// <summary>
        /// Obtiene o establece si el usuario invitado puede ver esta página
        /// </summary>
        public override bool PaginaVisibleEnPrivada
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}
