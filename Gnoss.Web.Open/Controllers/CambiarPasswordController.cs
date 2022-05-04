using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class CambiarPasswordController : ControllerBaseWeb
    {

        public CambiarPasswordController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmedPassword)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            string error = "";

            if (OldPassword.Length == 0)
            {
                error = UtilIdiomas.GetText("CAMBIARPASSWORD", "INTRODUCEPASSANTERIOR");
            }
            else if (NewPassword != ConfirmedPassword)
            {
                error = UtilIdiomas.GetText("CAMBIARPASSWORD", "NUEVASPASSIGUALES");
            }
            else if (OldPassword == NewPassword)
            {
                error = UtilIdiomas.GetText("CAMBIARPASSWORD", "NUEVAPASSIGUALANTERIOR");
            }
            else
            {
                UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = usuCN.ObtenerFilaUsuarioPorLoginOEmail(UsuarioActual.Login);

                JsonEstado jsonEstadoEditarPass = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.EditarPasswordConLogin, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, "", "", IdentidadActual.Persona.FilaPersona.Email, OldPassword, GestorParametroAplicacion, null, null, null, NewPassword);

                bool contrasenyCorrecta = false;

                if (jsonEstadoEditarPass != null)
                {
                    if (jsonEstadoEditarPass.Correcto)
                    {
                        contrasenyCorrecta = true;
                    }
                }
                else
                {
                    contrasenyCorrecta = usuCN.ValidarPasswordUsuario(filaUsuario, OldPassword);
                }

                if (contrasenyCorrecta)
                {
                    try
                    {
                        usuCN.EstablecerPasswordPropioUsuario(filaUsuario, NewPassword);
                        mControladorBase.UsuarioActual.FechaCambioPassword = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        GuardarLogError(mLoggingService.DevolverCadenaError(ex, "2.0.0.0"));
                        error = ex.Message;
                    }
                }
                else
                {
                    error = UtilIdiomas.GetText("CAMBIARPASSWORD", "PASSANTERIORNOCHUNGA");
                }
            }
            if (error == "")
            {
                return GnossResultOK(UtilIdiomas.GetText("CAMBIARPASSWORD", "PASSCAMBIADOEXITO"));
            }
            else
            {
                return GnossResultERROR(error);
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
