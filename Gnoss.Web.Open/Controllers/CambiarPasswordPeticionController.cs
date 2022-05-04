using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    /// <summary>
    /// View model de las pagina de busqueda
    /// </summary>
    [Serializable]
    public class CambiarPasswordPeticionViewModel
    {
        /// <summary>
        /// Nombre de la pagina de busqueda
        /// </summary>
        public string UrlAceptar { get; set; }
        /// <summary>
        /// Html de los resultados de la busqueda
        /// </summary>
        public string UrlRechazar { get; set; }

    }

    public class CambiarPasswordPeticionController : ControllerBaseWeb
    {

        public CambiarPasswordPeticionController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }


        [HttpGet]
        public ActionResult Index()
        {
            string urlBase = BaseURLIdioma + UrlPerfil;
            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto || EsEcosistemaSinMetaProyecto)
            {
                urlBase = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto) + "/";
            }

            if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(urlBase);
            }
            else if( RequestParams("usuarioID") == null)
            {
                return View("NoUsuario");
            }
            else if (RequestParams("peticionID") == null)
            {
                return View("NoPeticion");
            }
            else
            {
                Guid UsuarioID = new Guid(RequestParams("usuarioID"));
                Guid PeticionID = new Guid(RequestParams("peticionID"));

                PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionPeticiones gestionPeticion = new GestionPeticiones(peticionCN.ObtenerPeticionPorUsuarioIDyTipo(UsuarioID, TipoPeticion.CambioPassword, true), mLoggingService, mEntityContext);
                peticionCN.Dispose();

                UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperUsuario dataWrapperUsuario = usuCN.ObtenerUsuarioCompletoPorID(UsuarioID);
                usuCN.Dispose();
                GestionUsuarios gestorUsuario = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);

                if (!gestionPeticion.ListaPeticiones.ContainsKey(PeticionID) || gestorUsuario.DataWrapperUsuario.ListaUsuario.Count == 0)
                {
                    return View("NoPeticion");
                }
            }

            urlBase += UtilIdiomas.GetText("URLSEM", "CAMBIARPASSWORD") + "/" + UtilIdiomas.GetText("URLSEM", "PETICION") + "/" + RequestParams("peticionID") + "/" + UtilIdiomas.GetText("URLSEM", "USUARIO") + "/" + RequestParams("usuarioID");

            CambiarPasswordPeticionViewModel paginaModel = new CambiarPasswordPeticionViewModel();
            paginaModel.UrlAceptar = urlBase + "/accept";
            paginaModel.UrlRechazar = urlBase + "/reject";

            return View(paginaModel);
            
        }

        [HttpPost]
        public ActionResult ChangePassword(string User, string Password, string PasswordConfirmed)
        {
            string error = string.Empty;

            if (!string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Password) && Password.Equals(PasswordConfirmed))
            {
                if ((RequestParams("usuarioID") != null) && (RequestParams("peticionID") != null))
                {
                    Guid UsuarioID = new Guid(RequestParams("usuarioID"));
                    Guid PeticionID = new Guid(RequestParams("peticionID"));

                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    string email = personaCN.ObtenerEmailPersonalPorUsuario(UsuarioID);
                    //Comprobar si es menor
                    if (!string.IsNullOrEmpty(email))
                    {
                        AD.EntityModel.Models.UsuarioDS.Usuario usuarioRow = usuarioCN.ObtenerFilaUsuarioPorLoginOEmail(email);
                        usuarioCN.Dispose();

                        // Comprobación de que el User es el Login o el Email del usuario y de que existe una persona con ese usuarioID y ese Email
                        if (usuarioRow.Login.ToLower().Equals(User.ToLower()) || email.ToLower().Equals(User.ToLower()))
                        {
                            if (!personaCN.ComprobarEmailUsuario(email, UsuarioID))
                            {
                                error = UtilIdiomas.GetText("CONFIRMACIONCAMBIOPASSWORD", "TITULONOUSUARIO");
                            }
                        }
                        else
                        {
                            error = UtilIdiomas.GetText("CONFIRMACIONCAMBIOPASSWORD", "TITULONOUSUARIO");
                        }
                    }
                    else
                    {
                        AD.EntityModel.Models.UsuarioDS.Usuario usuarioRow = usuarioCN.ObtenerFilaUsuarioPorLoginOEmail(User);
                        usuarioCN.Dispose();

                        if (usuarioRow == null)
                        {
                            error = UtilIdiomas.GetText("CONFIRMACIONCAMBIOPASSWORD", "TITULONOUSUARIO");
                        }
                    }

                    if (string.IsNullOrEmpty(error))
                    {
                        try
                        {
                            UsuarioCN.ValidarFormatoPassword(Password);
                        }
                        catch (ErrorDatoNoValido)
                        {
                            error = UtilIdiomas.GetText("CONFIRMACIONCAMBIOPASSWORD", "FORMATONOVALIDO");
                        }
                    }
                    personaCN.Dispose();

                    if(string.IsNullOrEmpty(error))
                    {

                        PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        GestionPeticiones gestionPeticion = new GestionPeticiones(peticionCN.ObtenerPeticionPorUsuarioIDyTipo(UsuarioID, TipoPeticion.CambioPassword, true), mLoggingService, mEntityContext);
                        peticionCN.Dispose();

                        UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperUsuario dataWrapperUsuario = usuCN.ObtenerUsuarioCompletoPorID(UsuarioID);
                        usuCN.Dispose();
                        GestionUsuarios gestorUsuario = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);

                        if (gestionPeticion.ListaPeticiones.ContainsKey(PeticionID) && gestorUsuario.DataWrapperUsuario.ListaUsuario.Count > 0)
                        {
                            Peticion peticion = gestionPeticion.ListaPeticiones[PeticionID];
                            peticion.FilaPeticion.FechaProcesado = DateTime.Now;
                            peticion.FilaPeticion.Estado = (short)EstadoPeticion.Aceptada;

                            try
                            {
                                JsonEstado jsonEstadoEditarPass = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.EditarPassword, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, "", "", User, Password, GestorParametroAplicacion, null, null, null, null);

                                mEntityContext.SaveChanges();

                                return GnossResultOK();

                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
            else
            { 
                if(string.IsNullOrEmpty(User))
                {
                    error = UtilIdiomas.GetText("CONFIRMACIONCAMBIOPASSWORD", "NOUSUARIO");
                }
                else if(string.IsNullOrEmpty(Password))
                {
                    error = UtilIdiomas.GetText("CONFIRMACIONCAMBIOPASSWORD", "NOPASSWORD");
                }
                else if (!Password.Equals(PasswordConfirmed))
                {
                    error = UtilIdiomas.GetText("CONFIRMACIONCAMBIOPASSWORD", "PASSWORDNOIGUALES");
                }
            }

            return GnossResultERROR(error);
        }

        [HttpPost]
        public ActionResult Reject()
        {
            if ((RequestParams("usuarioID") != null) && (RequestParams("peticionID") != null))
            {
                bool error = false;

                Guid UsuarioID = new Guid(RequestParams("usuarioID"));
                Guid PeticionID = new Guid(RequestParams("peticionID"));

                PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionPeticiones gestionPeticion = new GestionPeticiones(peticionCN.ObtenerPeticionPorUsuarioIDyTipo(UsuarioID, TipoPeticion.CambioPassword, true), mLoggingService, mEntityContext);

                UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperUsuario usuDW = usuCN.ObtenerUsuarioCompletoPorID(UsuarioID);
                usuCN.Dispose();
                GestionUsuarios gestorUsuario = new GestionUsuarios(usuDW, mLoggingService, mEntityContext, mConfigService);

                Peticion peticion = gestionPeticion.ListaPeticiones[PeticionID];
                peticion.FilaPeticion.FechaProcesado = DateTime.Now;
                peticion.FilaPeticion.Estado = (short)EstadoPeticion.Rechazada;

                try
                {
                    peticionCN.ActualizarBD();

                    //Response.Redirect(BaseURLIdioma + "/" + UtilIdiomas.GetText("URLSEM", "CAMBIARPASSWORD") + "/" + UtilIdiomas.GetText("URLSEM", "RECHAZADO"));
                    
                }
                catch (Exception)
                {
                    error = true;
                }
                peticionCN.Dispose();

                if (!error)
                {
                    return GnossResultOK();
                }
            }
            return GnossResultERROR();
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
