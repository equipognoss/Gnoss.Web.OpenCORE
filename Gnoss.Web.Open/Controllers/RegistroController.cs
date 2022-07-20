using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.Live;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Usuarios;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class RegistroController : ControllerBaseWeb
    {
        public RegistroController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        private bool? mEsInvitacionAClase = null;

        private DataWrapperSolicitud mSolicitudDW = null;

        private bool? mEsInvitacionAClasePrimaria = null;

        private bool? mEsInvitacionAOrg = null;

        private AutenticationModel registro = new AutenticationModel();

        private List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> mClausulasRegistro = null;
        private DataWrapperProyecto mDatosExtraProyectoDataWrapperProyecto = null;

        private ProyectoEvento mInvitacionAEventoComunidad = null;
        private Guid? InvitacionID = null;
        private Peticion Invitacion = null;

        private SolicitudNuevoUsuario mSolicitud = null;
        private SolicitudNuevoUsuario mSolicitudPreActivacionRegistro = null;

        private List<ProfileModel> mListaPerfilesAceptarInvitacion = new List<ProfileModel>();

        private bool? mSaltarPasosRegistro;
        private string mUrlRedireccionTrasRegistro;

        public ActionResult Index()
        {
            CargarInvitacion();

            if (Invitacion != null && Invitacion.FilaPeticion.Estado != (short)EstadoPeticion.Pendiente)
            {
                registro.TypePage = AutenticationModel.TypeAutenticationPage.InvitacionUsada;
                return View(registro);
            }
            else if ((Invitacion != null && Invitacion.FilaPeticion.Estado == (short)EstadoPeticion.Pendiente) || (!mControladorBase.UsuarioActual.EsUsuarioInvitado /* && mControladorBase.UsuarioActual.EsIdentidadInvitada*/  && IdentidadActual.GestorIdentidades.ListaPerfiles.Values.Count > 0))
            {
                registro.InvitationRegistre = true;

                if (Invitacion != null && Invitacion.GestorPeticiones != null && Invitacion.GestorPeticiones.PeticionDW.ListaPeticionOrgInvitaPers.Count > 0)
                {
                    registro.AskPosition = true;
                }
                else
                {
                    //Si no es una invitacion a una organizanizacion
                    CargarSelectorPerfiles();
                }
            }

            if ((ProyectoSeleccionado.Estado == (short)EstadoProyecto.Definicion && string.IsNullOrEmpty(RequestParams("mostrarLogin")) && Invitacion == null) || (!mControladorBase.UsuarioActual.EsUsuarioInvitado && ((UserIdentityModel)ViewBag.IdentidadActual).IsExpelled))
            {
                return GnossResultUrl(Comunidad.Url);
            }

            if (!string.IsNullOrEmpty(RequestParams("registroRedesSociales")))
            {
                UsuarioCL usuarioCL = new UsuarioCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                usuarioCL.UsarClienteEscritura = true;
                Dictionary<string, object> listaDatosUsuario = usuarioCL.ObtenerDatosRedSocial(Request.Query["loginid"]);

                string emailUsuario = "";
                if (listaDatosUsuario != null && listaDatosUsuario.ContainsKey("correo"))
                {
                    emailUsuario = (string)listaDatosUsuario["correo"];

                    if (!string.IsNullOrEmpty(emailUsuario) && !ComprobarDominioEmailsPermitidos(emailUsuario))
                    {
                        //string dominiosPermitidos = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.DominiosEmailLoginRedesSociales).Valor;
                        string dominiosPermitidos = ParametrosAplicacionDS.Find(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.DominiosEmailLoginRedesSociales)).Valor;
                        return View("ErrorDominioEmailLoginRedSocial", UtilIdiomas.GetText("LOGIN", "ERRORDOMINIONOPERMITIDO", dominiosPermitidos));
                    }
                }

                if (!string.IsNullOrEmpty(Request.Query["loginid"]) && !string.IsNullOrEmpty(Request.Query["proyID"]))
                {
                    //Si es la primera redirección cierro popup y redirijo a condiciones
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    string UrlComunidad = mControladorBase.UrlsSemanticas.GetURLHacerseMiembroComunidad(BaseURLIdioma, UtilIdiomas, proyCN.ObtenerNombreCortoProyecto(new Guid(Request.Query["proyID"])), true) + "/reload?loginid=" + Request.Query["loginid"];

                    if (new Guid(Request.Query["proyID"]) == ProyectoAD.MetaProyecto)
                    {
                        UrlComunidad = BaseURLIdioma + "/" + UtilIdiomas.GetText("URLSEM", "LOGIN") + "/reload?loginid=" + Request.Query["loginid"];
                    }

                    if (InvitacionAEventoComunidad != null)
                    {
                        UrlComunidad += "&eventoComID=" + InvitacionAEventoComunidad.EventoID;
                    }


                    if (!string.IsNullOrEmpty(Request.Query["invitacionID"]))
                    {
                        UrlComunidad += "&invitacionID=" + Request.Query["invitacionID"];
                    }
                    string closePopUp = "window.opener.location.href='" + UrlComunidad + "';self.close();";


                    return PartialView("_ClosePopUpRedesSociales", closePopUp);
                }
                else if (string.IsNullOrEmpty(Request.Query["loginid"]) && string.IsNullOrEmpty(Request.Query["proyID"]))
                {
                    string urlRegitro = "";

                    if (!string.IsNullOrEmpty(Request.Query["urlRegistro"]))
                    {
                        urlRegitro = Request.Query["urlRegistro"];
                    }

                    string closePopUp = "";

                    if (!string.IsNullOrEmpty(urlRegitro))
                    {
                        closePopUp = "window.opener.document.location.href='" + urlRegitro + "';self.close();";
                    }
                    else
                    {
                        closePopUp = "window.opener.document.location.reload();self.close();";
                    }

                    return PartialView("_ClosePopUpRedesSociales", closePopUp);
                }
            }

            if (!mControladorBase.UsuarioActual.EsIdentidadInvitada && InvitacionAEventoComunidad != null && InvitacionAEventoComunidad.TipoEvento.Equals((short)TipoEventoProyecto.SinRestriccion))
            {
                ControladorIdentidades.AgregarParticipanteEvento(IdentidadActual.Persona, ProyectoSeleccionado, InvitacionAEventoComunidad, UrlIntragnoss, UtilIdiomas.LanguageCode);
            }

            //Se debe hacer cuando el usuario tenga Email de Tutor.
            if (!ValidadoClausulasTutor)
            {
                if (HaySolicitudPrevia && SolicitudPrevia != null && SolicitudPrevia.EmailTutor != null && !ControladorIdentidades.ComprobarPersonaEsMayorAnios(SolicitudPrevia.FechaNacimiento.Value, 14))
                {
                    return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "ACTIVARCUENTATUTOR") + "/" + SolicitudPrevia.SolicitudID);
                }
            }

            if ((!mControladorBase.UsuarioActual.EsUsuarioInvitado && Invitacion is PeticionInvOrganizacion) || // Invitación a organización
                (!mControladorBase.UsuarioActual.EsIdentidadInvitada || (ProyectoSeleccionado.EsPublico && !mControladorBase.UsuarioActual.EsUsuarioInvitado && !string.IsNullOrEmpty(RequestParams("mostrarLogin")))))
            {
                if (mListaPerfilesAceptarInvitacion.Count == 0)
                {
                    if (Invitacion != null)
                    {
                        AceptarInvitacion(false);

                        if (Invitacion.GestorPeticiones != null && Invitacion.GestorPeticiones.PeticionDW.ListaPeticionOrgInvitaPers.Count > 0)
                        {
                            Guid organizacionID = Invitacion.GestorPeticiones.PeticionDW.ListaPeticionOrgInvitaPers.FirstOrDefault().OrganizacionID;

                            string nombreCortoOrg = IdentidadActual.GestorIdentidades.GestorOrganizaciones.ListaOrganizaciones[organizacionID].NombreCorto;

                            return GnossResultUrl(GnossUrlsSemanticas.ObtenerUrlBaseIdentidad(BaseURLIdioma, UtilIdiomas, nombreCortoOrg) + UtilIdiomas.GetText("URLSEM", "REGISTROORGANIZACIONCOMPLETO"));
                        }
                        else
                        {
                            string urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                            if (!ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionExpandida) && !ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionPrimaria))
                            {
                                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                //Insertar en la tabla UsuarioRedirect el valor del redirect
                                urlRedirect = urlRedirect + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/1";
                                usuarioCN.InsertarUrlRedirect(mControladorBase.UsuarioActual.UsuarioID, urlRedirect);
                            }
                            return GnossResultUrl(urlRedirect);

                        }
                    }
                    else if (InvitacionAEventoComunidad != null && !string.IsNullOrEmpty(InvitacionAEventoComunidad.UrlRedirect))
                    {
                        return GnossResultUrl(InvitacionAEventoComunidad.UrlRedirect);
                    }
                    else
                    {
                        string urlRedirect = Comunidad.Url;

                        if (!string.IsNullOrEmpty(RequestParams("redirect")))
                        {
                            urlRedirect = BaseURLIdioma + "/" + RequestParams("redirect");
                        }

                        string urlRedirectHome = ObtenerUrlHomeConectado();
                        if (!string.IsNullOrEmpty(urlRedirectHome))
                        {
                            urlRedirect = urlRedirectHome;
                        }

                        if (Request.Query.Count > 0)
                        {
                            urlRedirect += "?" + Request.QueryString.ToString();
                        }

                        return GnossResultUrl(urlRedirect);
                    }
                }
            }

            if (!string.IsNullOrEmpty(RequestParams("callback")))
            {
                string callback = RequestParams("callback");
                if (callback.ToLower() == "cargarFormLogin".ToLower())
                {
                    registro.TypePage = AutenticationModel.TypeAutenticationPage.Login;
                    return PartialView("_FormularioLogin");
                }
                else if (callback.ToLower() == "comprobarEmail".ToLower())
                {
                    if (ExisteEmail(RequestParams("email")))
                    {
                        return Content("KO");
                    }
                    return Content("OK");
                }

                return new EmptyResult();
            }

            if (!string.IsNullOrEmpty(RequestParams("mostrarLogin")))
            {
                if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
                {
                    if (!IdentidadActual.EsIdentidadInvitada || ProyectoSeleccionado.EsPublico || RegistroAbiertoEnComunidadPrivada)
                    {
                        return GnossResultUrl(Comunidad.Url);
                    }

                    registro.TypePage = AutenticationModel.TypeAutenticationPage.NoPermiso;
                }
                else
                {
                    EstablecerPaginaOrigen();

                    registro.TypePage = AutenticationModel.TypeAutenticationPage.Login;
                }
            }
            else if (!string.IsNullOrEmpty(RequestParams("SelectorPerfilInvitacion")))
            {
                //CallBack
                string args = RequestParams("SelectorPerfilInvitacion");
                GnossIdentity usuario = mControladorBase.UsuarioActual;
                Guid perfilID = Guid.Empty;
                if (args.Length > 0)
                {
                    //Recuperamos el objeto gnossidentity del perfil
                    perfilID = new Guid(args);
                }

                if (IdentidadActual.GestorIdentidades.ListaPerfiles.ContainsKey(perfilID))
                {
                    AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.FirstOrDefault(identidad => identidad.PerfilID.Equals(perfilID) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto));
                    Identidad identidadPerfil = new Identidad(filaIdentidad, IdentidadActual.GestorIdentidades.ListaPerfiles[perfilID], mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    bool invitacionAceptadaOtraIdentidad = false;
                    string urlRedirect = RegistrarUsuarioEnComunidad(identidadPerfil, out invitacionAceptadaOtraIdentidad);
                    if (Invitacion != null)
                    {
                        AceptarInvitacion(invitacionAceptadaOtraIdentidad);
                    }
                    return GnossResultUrl(urlRedirect);
                }
            }
            else if (EstaVerificandoEmail)
            {
                if (SolicitudPrevia != null)
                {
                    if (Solicitud.Solicitud.Estado == 2)
                    {
                        return Redirect(ValidarUsuario(SolicitudPrevia.UsuarioID));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(SolicitudPrevia.Email))
                        {
                            return Redirect(RegistrarUsuario(SolicitudPrevia.EmailTutor, null, null, SolicitudPrevia));
                        }
                        else
                        {
                            return Redirect(RegistrarUsuario(SolicitudPrevia.Email, null, null, SolicitudPrevia));
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Solicitud.Email))
                    {
                        return Redirect(RegistrarUsuario(Solicitud.EmailTutor, null, null, Solicitud));
                    }
                    else
                    {
                        return Redirect(RegistrarUsuario(Solicitud.Email, null, null, Solicitud));
                    }
                }


            }
            else
            {
                registro.TypePage = AutenticationModel.TypeAutenticationPage.Registro;
                registro.AskBornDate = ParametrosGeneralesRow.FechaNacimientoObligatoria;

                if (InvitacionAEventoComunidad != null)
                {
                    registro.EventID = InvitacionAEventoComunidad.EventoID;
                    registro.EventName = InvitacionAEventoComunidad.Nombre;
                }

                if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
                {
                    if (Invitacion == null && !ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        if (!ProyectoSeleccionado.EsPublico && !RegistroAbiertoEnComunidadPrivada)
                        {
                            return GnossResultUrl(Comunidad.Url);
                        }
                        if (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido)
                        {
                            EnviarSolicitudComunidad();
                            return GnossResultUrl(UrlOrigen);
                        }
                    }

                    int numClausulas = CargarClausulasRegistro(registro);

                    bool cumpleClausualas = false;

                    if (numClausulas > 0)
                    {
                        if (!string.IsNullOrEmpty(Request.Form["clausulasSelecc"]))
                        {
                            cumpleClausualas = ComprobarErroresRegistroClausulas().Count == 0;
                        }
                    }

                    if (numClausulas == 0 || cumpleClausualas)
                    {
                        //Si es una identidad invitada y tiene un perfil disponible para aceptar la invitacion 
                        //Si no tiene perfiles disponibles para aceptar la invitacion 
                        if (mListaPerfilesAceptarInvitacion.Count == 0 || (ViewBag.SoloIdentidadPersonal != null && ViewBag.SoloIdentidadPersonal))
                        {
                            bool invitacionAceptadaOtraIdentidad = false;
                            //si es de acceso publico y no tiene condiciones le hace miembro directamente
                            string urlRedirect = RegistrarUsuarioEnComunidad(IdentidadActual, out invitacionAceptadaOtraIdentidad);
                            if (Invitacion != null)
                            {
                                AceptarInvitacion(invitacionAceptadaOtraIdentidad);
                            }
                            return GnossResultUrl(urlRedirect);
                        }
                    }
                }
                else
                {
                    if (Invitacion == null && !HaySolicitudPrevia)
                    {
                        if (!ProyectoSeleccionado.EsPublico && !RegistroAbiertoEnComunidadPrivada)
                        {
                            return GnossResultUrl(Comunidad.Url);
                        }
                    }

                    CargarCamposRegistroProyecto(registro);
                    CargarCamposExtraRegistro(registro);
                    CargarEdadMinimaRegistro(registro);

                    if (Request.HasFormContentType && Request.Form.Count > 0)
                    {
                        string emailRedSodial = Request.Form["txtEmail"];
                        string email = Request.Form["txtEmail"];
                        string emailTutor = Request.Form["txtEmailTutor"];
                        TipoRedSocialLogin? tipoRedSocial = null;
                        string idRedSocial = null;
                        bool registroRedesSociales;

                        if (Request.Form.ContainsKey("urlOrigen"))
                        {
                            ViewBag.UrlOrigen = Request.Form["urlOrigen"].ToString().Trim();
                        }

                        if (string.IsNullOrEmpty(emailRedSodial))
                        {
                            emailRedSodial = Request.Form["txtEmailTutor"];
                        }

                        if (!string.IsNullOrEmpty(RequestParams("registroRedesSociales")) && bool.TryParse(RequestParams("registroRedesSociales"), out registroRedesSociales) && registroRedesSociales)
                        {
                            registro.TypePage = AutenticationModel.TypeAutenticationPage.RegistroConRedesSociales;

                            UsuarioCL usuarioCL = new UsuarioCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                            usuarioCL.UsarClienteEscritura = true;
                            Dictionary<string, object> listaDatosUsuario = usuarioCL.ObtenerDatosRedSocial(Request.Query["loginid"]);

                            // Twitter no tiene email en su lista de datos
                            string correo = (string)listaDatosUsuario["correo"];
                            if (!string.IsNullOrEmpty(correo))
                            {
                                emailRedSodial = correo;
                            }

                            tipoRedSocial = (TipoRedSocialLogin)(short)listaDatosUsuario["tipored"];
                            idRedSocial = (string)listaDatosUsuario["id"];

                            Dictionary<string, List<string>> listaGrupos = (Dictionary<string, List<string>>)listaDatosUsuario["grupos"];
                            foreach (string proy in listaGrupos.Keys)
                            {
                                registro.NetGroups += "@@@" + proy;
                                foreach (string grupo in listaGrupos[proy])
                                {
                                    registro.NetGroups += "|" + grupo;
                                }
                            }

                            registro.NetToken = (string)listaDatosUsuario["token"];
                        }

                        bool hayErrores = ComprobarErroresRegistro(email, emailTutor);

                        if (!hayErrores)
                        {
                            string urlRedirect = RegistrarUsuario(emailRedSodial, tipoRedSocial, idRedSocial, Solicitud);
                            if (Invitacion != null && !mControladorBase.UsuarioActual.EsUsuarioInvitado)
                            {
                                AceptarInvitacion(false);
                            }
                            if (urlRedirect != string.Empty)
                            {
                                return GnossResultUrl(urlRedirect);
                            }
                        }
                    }
                    else
                    {
                        EstablecerPaginaOrigen();
                        EstablecerSeguridad(registro, Session, Response);
                        bool registroRedesSociales;

                        if (!string.IsNullOrEmpty(RequestParams("registroRedesSociales")) && bool.TryParse(RequestParams("registroRedesSociales"), out registroRedesSociales) && registroRedesSociales)
                        {
                            registro.TypePage = AutenticationModel.TypeAutenticationPage.RegistroConRedesSociales;

                            UsuarioCL usuarioCL = new UsuarioCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                            usuarioCL.UsarClienteEscritura = true;
                            Dictionary<string, object> listaDatosUsuario = usuarioCL.ObtenerDatosRedSocial(Request.Query["loginid"]);

                            registro.Name = (string)listaDatosUsuario["nombre"];
                            registro.LastName = (string)listaDatosUsuario["apellidos"];
                            registro.Email = (string)listaDatosUsuario["correo"];

                            bool? hombre = (bool?)listaDatosUsuario["hombre"];
                            DateTime? nacimiento = (DateTime?)listaDatosUsuario["nacimiento"];
                            if (hombre.HasValue)
                            {
                                registro.Gender = (hombre.Value ? "H" : "M");
                            }
                            if (nacimiento.HasValue)
                            {
                                registro.BornDate = nacimiento.Value.ToString("dd/MM/yyyy");
                            }

                            Dictionary<string, List<string>> listaGrupos = (Dictionary<string, List<string>>)listaDatosUsuario["grupos"];
                            foreach (string proy in listaGrupos.Keys)
                            {
                                registro.NetGroups += "@@@" + proy;
                                foreach (string grupo in listaGrupos[proy])
                                {
                                    registro.NetGroups += "|" + grupo;
                                }
                            }

                            registro.NetToken = (string)listaDatosUsuario["token"];
                        }
                        //si es un registro preactivado los datos ya se han traído desde la solicitud
                        else if (EsRegistroPreActivado)
                        {
                            if (SolicitudPrevia != null)
                            {
                                InsertarRegistroSolicitud(registro, SolicitudPrevia);
                            }
                            else
                            {
                                InsertarRegistroSolicitud(registro, Solicitud);
                            }
                            /*
                            registro.Name = SolicitudPrevia.Nombre;
                            registro.LastName = SolicitudPrevia.Apellidos;
                            registro.Email = SolicitudPrevia.Email;
                            registro.Gender = SolicitudPrevia.Sexo;
                            registro.CountryID = SolicitudPrevia.PaisID;
                            if (!SolicitudPrevia.IsProvinciaIDNull())
                            {
                                registro.RegionID = SolicitudPrevia.ProvinciaID;
                            }
                            registro.Region = SolicitudPrevia.Provincia;
                            registro.Location = SolicitudPrevia.Poblacion;
                            registro.BornDate = SolicitudPrevia.FechaNacimiento.ToString("dd/MM/yyyy");
                            */
                        }
                        else
                        {
                            registro.Name = RequestParams("nombre");
                            registro.LastName = RequestParams("apellidos");
                            registro.Email = RequestParams("email");

                            if (Request.Headers.ContainsKey("Referer") && mControladorBase.DominoAplicacion.Equals(UtilDominios.ObtenerDominioUrl(Request.Headers["Referer"], false)))
                            {
                                Response.Cookies.Append("reg-redirect", Request.Headers["Referer"].ToString(), new CookieOptions
                                {
                                    Expires = DateTime.Now.AddHours(1),
                                    Domain = mControladorBase.DominoAplicacion
                                });
                            }
                        }
                    }

                    CargarClausulasRegistro(registro);
                }
            }

            if (!string.IsNullOrEmpty(RequestParams("token")))
            {
                string urlDesconexion = UrlEncode($"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoConexion)}/{UtilIdiomas.GetText("URLSEM", "ELIMINARSESION")}?token={RequestParams("token")}");
                string query = "?dominio=" + BaseURL + $"/&eliminar=true&redirect={urlDesconexion}";
                string urlRedirect = mControladorBase.UrlServicioLogin + "/eliminarCookie" + query;
                registro.URLDesconexion = urlRedirect;
            }

            if (Request.HasFormContentType && Request.Form != null && Request.Form.Count > 0 && Request.Form["peticionAJAX"] == "true")
            {
                string errores = "";
                if (registro.Errors != null)
                {
                    foreach (string error in registro.Errors)
                    {
                        errores += "||" + error;
                    }
                }
                if (!string.IsNullOrEmpty(registro.ImageCaptcha))
                {
                    errores += "||captcha=" + registro.ImageCaptcha;
                }
                if (!registro.SecurityID.Equals(Guid.Empty))
                {
                    errores += "||securityID=" + registro.SecurityID;
                }
                return GnossResultERROR(errores);
            }

            return View(registro);
        }

        public ActionResult RegistroCompleto()
        {
            return View("RegistroCompleto", IdentidadActual.PerfilUsuario.NombreOrganizacion);
        }

        public ActionResult ValidaEmail()
        {
            if (EsTutorRegistro)
            {
                return View("ValidarEmail", true);
            }
            else
            {
                return View("ValidarEmail", false);
            }
        }

        public ActionResult ClausulasTutor()
        {
            //Obtener la url del registro.
            string urlRegistro = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
            urlRegistro += "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/" + UtilIdiomas.GetText("URLSEM", "ACTIVACIONGNOSS") + "/" + SolicitudPrevia.SolicitudID;
            return View("ClausulasTutor", urlRegistro);
        }

        public ActionResult RegistroTutor()
        {
            string url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
            Guid personaID = mControladorBase.UsuarioActual.PersonaID;
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionPersonas gestorPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorID(personaID), mLoggingService, mEntityContext);
            Persona persona = gestorPersonas.ListaPersonas[personaID];
            if (!mControladorBase.UsuarioActual.EsUsuarioInvitado && !ControladorIdentidades.ComprobarPersonaEsMayorAnios(persona.Fecha, 14))
            {
                return Redirect(url);
            }
            return View("RegistroTutor");
        }

        public ActionResult ReenviarEmail()
        {
            AutenticationModel registro = new AutenticationModel();
            if (!string.IsNullOrEmpty(RequestParams("callback")))
            {
                string callback = RequestParams("callback");
                if (callback.ToLower() == "comprobarEmail".ToLower())
                {
                    if (ExisteEmail(RequestParams("email")))
                    {
                        return Content("KO");
                    }
                    return Content("OK");
                }

                return new EmptyResult();
            }

            Guid solicitudCorreo = Guid.Empty;
            if (RequestParams("preActivacionID") != null)
            {
                solicitudCorreo = new Guid(RequestParams("preActivacionID"));
            }
            estaAceptada(registro);


            return View("ReenviarEmail", registro);
        }

        public ActionResult CambiarCorreo(string Url)
        {
            Guid solicitudCorreo = Guid.Empty;
            if (RequestParams("preActivacionID") != null)
            {
                solicitudCorreo = new Guid(RequestParams("preActivacionID"));
            }
            CambiarPersonaCorreo(Url);

            EnviarCorreo(solicitudCorreo, Url);

            return new GnossResult(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "REGISTROCOMPLETO"), mViewEngine);
        }

        private void CambiarPersonaCorreo(string correo)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperPersona dataWrapperPersona = personaCN.ObtenerPersonaPorUsuario(Solicitud.UsuarioID);
            Guid personaID = personaCN.ObtenerPersonaIDPorUsuarioID(Solicitud.UsuarioID).Value;

            GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
            gestorPersonas.CargarGestor();
            Persona persona = null;
            if (gestorPersonas.ListaPersonas.ContainsKey(personaID))
            {
                persona = gestorPersonas.ListaPersonas[personaID];
            }
            if(persona != null)
            {
                AD.EntityModel.Models.PersonaDS.Persona filaPersona = persona.FilaPersona;
                filaPersona.Email = correo;
                personaCN.ActualizarPersonas();
            }
        }

        private void estaAceptada(AutenticationModel registro)
        {
            registro.Reenviar = false;
            if (SolicitudPrevia == null)
            {
                registro.Reenviar = true;
            }
        }

        public ActionResult EnviarEmail()
        {
            //Mandar Correo

            Guid solicitudCorreo = Guid.Empty;
            if (RequestParams("preActivacionID") != null)
            {
                solicitudCorreo = new Guid(RequestParams("preActivacionID"));
            }
            EnviarCorreo(solicitudCorreo);

            return new GnossResult(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "REGISTROCOMPLETO"), mViewEngine);
            //return View("ValidarEmail");
        }


        public void EnviarCorreo(Guid SolicitudID, string cambioEmail = null)
        {
            DataWrapperNotificacion notifDW = new DataWrapperNotificacion();
            GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notifDW, IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            //Enviamos un correo de congirmación
            SolicitudNuevoUsuario filaSolicitudNuevoUsuario2 = SolicitudDW.ListaSolicitudNuevoUsuario[0];

            string urlEnlace2 = BaseURLIdioma;
            if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MyGnoss))
            {
                urlEnlace2 = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
            }

            urlEnlace2 += "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/" + UtilIdiomas.GetText("URLSEM", "ACTIVACIONGNOSS") + "/" + filaSolicitudNuevoUsuario2.SolicitudID;

            string correo = filaSolicitudNuevoUsuario2.Email;
            if (cambioEmail != null)
            {
                correo = cambioEmail;
            }

            gestorNotificaciones.AgregarNotificacionSolicitudEntradaGNOSS(filaSolicitudNuevoUsuario2.SolicitudID, filaSolicitudNuevoUsuario2.Nombre, null, TiposNotificacion.SolicitudNuevoUsuario, correo, null, this.BaseURLIdioma, filaSolicitudNuevoUsuario2.Idioma, urlEnlace2, "", ProyectoSeleccionado.Clave);

            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            notificacionCN.ActualizarNotificacion();

        }

        /// <summary>
        /// Se cargan los perfiles que se van a mostrar en el desplegable
        /// </summary>
        private void CargarSelectorPerfiles()
        {
            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperOrganizacion orgDW = orgCN.CargarOrganizacionesAdministraUsuario(mControladorBase.UsuarioActual.UsuarioID);

            foreach (Elementos.Identidad.Perfil perfil in IdentidadActual.GestorIdentidades.ListaPerfiles.Values)
            {
                string nombreAMostrar = perfil.NombrePersonaEnOrganizacion;
                bool agregarPerfilAlDesplegable = true;
                if (perfil.OrganizacionID.HasValue)
                {
                    nombreAMostrar = perfil.NombrePersonaEnOrganizacion + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + perfil.NombreOrganizacion;

                    if (!perfil.PersonaID.HasValue)
                    {
                        if (IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(identidad => identidad.PerfilID.Equals(perfil.Clave) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && identidad.Tipo == 3))
                        {//"PerfilID = '" + perfil.Clave + "' AND ProyectoID = '" + ProyectoSeleccionado.Clave + "' AND Tipo = 3 AND FechaBaja IS NULL AND FechaExpulsion IS NULL"
                            bool esAdminOrg = orgDW.ListaAdministradorOrganizacion.Any(item => item.OrganizacionID.Equals(perfil.OrganizacionID));
                            bool organizacionPertenceComunidad = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(identidad => identidad.PerfilID.Equals(perfil.Clave) && identidad.ProyectoID.Equals(ProyectoSeleccionado.Clave) && identidad.Tipo == 3 && !identidad.FechaBaja.HasValue && !identidad.FechaExpulsion.HasValue);

                            if (!esAdminOrg || organizacionPertenceComunidad)
                            {
                                agregarPerfilAlDesplegable = false;
                            }
                        }
                    }
                    else
                    {
                        agregarPerfilAlDesplegable = false;
                    }
                }
                //"ProyectoID = '" + ProyectoSeleccionado.Clave + "' AND IdentidadID <> '" + UsuarioAD.Invitado + "' AND Tipo <> 3"
                if ((IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(identidad => identidad.ProyectoID.Equals(ProyectoSeleccionado.Clave) && !identidad.IdentidadID.Equals(UsuarioAD.Invitado) && identidad.Tipo != 3) && perfil.PersonaID.HasValue) || IdentidadActual.GestorIdentidades.ListaPerfiles.Count == 1)
                {
                    //Es el perfil personal, comprobar si ya existe alguna identidad dentro del proyecto, si así es, no cargar el personal.
                    //Comprobar si el perfil pertenece ya a la comunidad con alguna identidad
                    //Ó sólo tiene un perfil
                    agregarPerfilAlDesplegable = false;
                }

                if (agregarPerfilAlDesplegable)
                {
                    ProfileModel pm = new ProfileModel();
                    pm.NamePerson = nombreAMostrar;
                    pm.Key = perfil.Clave;

                    mListaPerfilesAceptarInvitacion.Add(pm);
                }
            }

            orgCN.Dispose();

            ViewBag.PerfilesAceptarinvitacion = mListaPerfilesAceptarInvitacion.OrderBy(perfil => perfil.NamePerson).ToList();

            bool soloIdentidadPersonal = false;
            if (mListaPerfilesAceptarInvitacion.Count == 1)
            {
                if (mListaPerfilesAceptarInvitacion[0].Key.Equals(IdentidadActual.IdentidadMyGNOSS.PerfilID))
                {
                    soloIdentidadPersonal = true;
                }
            }
            ViewBag.SoloIdentidadPersonal = soloIdentidadPersonal;
        }


        private void InsertarRegistroSolicitud(AutenticationModel registro, SolicitudNuevoUsuario pSolicitud)
        {
            registro.Name = pSolicitud.Nombre;
            registro.LastName = pSolicitud.Apellidos;
            registro.Email = pSolicitud.Email;
            registro.Gender = pSolicitud.Sexo;
            registro.CountryID = pSolicitud.PaisID.Value;
            if (pSolicitud.ProvinciaID.HasValue)
            {
                registro.RegionID = pSolicitud.ProvinciaID.Value;
            }
            registro.Region = pSolicitud.Provincia;
            registro.Location = pSolicitud.Poblacion;
            registro.BornDate = pSolicitud.FechaNacimiento.Value.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// 
        /// </summary>
        private void EnviarSolicitudComunidad()
        {
            SolicitudCN solCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperSolicitud solDW = new DataWrapperSolicitud();

            Solicitud filaSolicitud = new Solicitud();
            filaSolicitud.SolicitudID = Guid.NewGuid();
            filaSolicitud.Estado = (short)EstadoSolicitud.Espera;
            filaSolicitud.FechaSolicitud = DateTime.Now;
            filaSolicitud.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaSolicitud.ProyectoID = ProyectoSeleccionado.Clave;
            solDW.ListaSolicitud.Add(filaSolicitud);
            mEntityContext.Solicitud.Add(filaSolicitud);


            SolicitudUsuario filaSolUsu = new SolicitudUsuario();
            filaSolUsu.Solicitud = filaSolicitud;
            filaSolUsu.PersonaID = IdentidadActual.PersonaID.Value;
            filaSolUsu.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
            filaSolUsu.PerfilID = IdentidadActual.PerfilID;
            solDW.ListaSolicitudUsuario.Add(filaSolUsu);
            mEntityContext.SolicitudUsuario.Add(filaSolUsu);


            GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestorNotificaciones.AgregarNotificacionSolicitud(filaSolicitud.SolicitudID, DateTime.Now, ProyectoSeleccionado.Nombre, mControladorBase.UsuarioActual.Login, "", TiposNotificacion.SolicitudPendiente, IdentidadActual.Persona.FilaPersona.Email, IdentidadActual.Persona, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, UtilIdiomas.LanguageCode);

            mEntityContext.SaveChanges();
            solCN.Dispose();
        }

        private void AceptarInvitacion(bool pInvitacionAceptadaOtraIdentidad)
        {
            if (Invitacion.GestorPeticiones.PeticionDW.ListaPeticionInvitacionComunidad.Count > 0)
            {
                AD.EntityModel.Models.Peticion.PeticionInvitacionComunidad filaPeticionInvitacionCom = Invitacion.GestorPeticiones.PeticionDW.ListaPeticionInvitacionComunidad.FirstOrDefault();
                if (!filaPeticionInvitacionCom.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !pInvitacionAceptadaOtraIdentidad)
                {
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                    DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
                    dataWrapperUsuario.ListaUsuario.Add(usuarioCN.ObtenerUsuarioPorID(mControladorBase.UsuarioActual.UsuarioID));

                    //Se tiene que acceptar aquí o sino no se ha creado el usuario todavía...
                    ControladorIdentidades.AceptarInvitacionComunidadConInvitacion(filaPeticionInvitacionCom.ProyectoID, filaPeticionInvitacionCom.OrganizacionID, IdentidadActual, dataWrapperUsuario, UsuarioActual.UsuarioID);
                }
            }
            if (Invitacion.GestorPeticiones.PeticionDW.ListaPeticionInvitacionGrupo.Count > 0)
            {
                AD.EntityModel.Models.Peticion.PeticionInvitacionGrupo filaPeticionGrupo = Invitacion.GestorPeticiones.PeticionDW.ListaPeticionInvitacionGrupo.FirstOrDefault();
                string[] grupos = filaPeticionGrupo.GruposID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<Guid> listaGrupos = new List<Guid>();

                foreach (string grupo in grupos)
                {
                    Guid grupoID = new Guid(grupo);
                    if (!listaGrupos.Contains(grupoID))
                    {
                        listaGrupos.Add(grupoID);
                    }
                }

                DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();
                BasePerOrgComunidadDS basePerOrgComDS = new BasePerOrgComunidadDS();
                IdentidadCN identidadCN2 = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionIdentidades gestorIden = new GestionIdentidades(identidadCN2.ObtenerIdentidadDePersonaEnProyecto(filaPeticionGrupo.ProyectoID, IdentidadActual.PersonaID.Value), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                List<Guid> listaGruposPertenece = identidadCN2.ObtenerIDGruposDeIdentidad(gestorIden.ListaIdentidades.Keys[0]);

                List<Guid> listaGruposEsxisten = identidadCN2.ComprobarSiIDGruposExisten(listaGrupos);

                foreach (Guid grupoID in listaGrupos)
                {
                    if (!listaGruposPertenece.Contains(grupoID) && listaGruposEsxisten.Contains(grupoID))
                    {
                        AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();

                        filaGrupoIdentidadesParticipacion.GrupoID = grupoID;
                        filaGrupoIdentidadesParticipacion.IdentidadID = gestorIden.ListaIdentidades.Keys[0];
                        filaGrupoIdentidadesParticipacion.FechaAlta = DateTime.Now;

                        identidadDW.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
                        mEntityContext.GrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
                        basePerOrgComDS = InsertarFilaBaseGrupo(grupoID);
                    }
                }


                identidadCN2.ActualizaIdentidades();
                //ActualizarBaseGrupo
                BaseComunidadCN basePerOrgComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
                basePerOrgComCN.InsertarFilasEnRabbit("ColaTagsCom_Per_Org", basePerOrgComDS);
            }
            else if (Invitacion.GestorPeticiones.PeticionDW.ListaPeticionInvitacionContacto.Count > 0)
            {
                AD.EntityModel.Models.Peticion.PeticionInvitaContacto filapeticionInvitacionContacto = Invitacion.GestorPeticiones.PeticionDW.ListaPeticionInvitacionContacto.FirstOrDefault();

                ControladorIdentidades.AceptarInvitacionContacto(filapeticionInvitacionContacto.IdentidadID, IdentidadActual, UrlIntragnoss, BaseURL, BaseURLIdioma, ProyectoVirtual, Invitacion, UtilIdiomas.LanguageCode, UsuarioActual);
            }
            else if (Invitacion.GestorPeticiones.PeticionDW.ListaPeticionOrgInvitaPers.Count > 0)
            {
                string cargo = "";
                //if (Request.Form.ContainsKey("txtCargo"))
                // Comprobar que hay formulario en la petición para evitar fallo en peticiones sin Form
                if(Request.HasFormContentType && Request.Form != null && Request.Form.Count > 0)
                {
                    cargo = Request.Form["txtCargo"].ToString().Trim();
                }

                PeticionInvOrganizacion invitacion = (PeticionInvOrganizacion)Invitacion;
                invitacion.FilaInvitacionOrganizacion.Cargo = cargo;

                ControladorIdentidades.AceptarInvitacionOrganizacion(invitacion, IdentidadActual, UrlIntragnoss, UsuarioActual, ProyectoSeleccionado.Clave);
            }

            Invitacion.FilaPeticion.FechaProcesado = DateTime.Now;
            Invitacion.FilaPeticion.Estado = (short)EstadoPeticion.Aceptada;
            Invitacion.FilaPeticion.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;

            PeticionCN petCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            petCN.ActualizarBD();
            petCN.Dispose();
        }

        private BasePerOrgComunidadDS InsertarFilaBaseGrupo(Guid pGrupoID)
        {
            BasePerOrgComunidadDS basePerOrgComDS = new BasePerOrgComunidadDS();
            BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org_Add = basePerOrgComDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
            filaColaTagsCom_Per_Org_Add.TablaBaseProyectoID = ProyectoSeleccionado.FilaProyecto.TablaBaseProyectoID;
            filaColaTagsCom_Per_Org_Add.Tags = Constantes.PERS_U_ORG + "g" + Constantes.PERS_U_ORG + ", " + Constantes.ID_TAG_PER + pGrupoID + Constantes.ID_TAG_PER;
            filaColaTagsCom_Per_Org_Add.Tipo = (short)TiposElementosEnCola.Agregado;
            filaColaTagsCom_Per_Org_Add.Estado = 0;
            filaColaTagsCom_Per_Org_Add.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsCom_Per_Org_Add.Prioridad = (short)PrioridadBase.Alta;
            basePerOrgComDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org_Add);
            return basePerOrgComDS;
        }

        private void EstablecerPaginaOrigen()
        {
            string urlOrigen = "";
            bool proyectoSinURL = ParametroProyecto.ContainsKey(ParametroAD.ProyectoSinNombreCortoEnURL) && ParametroProyecto[ParametroAD.ProyectoSinNombreCortoEnURL] == "1";

            if (Session.Get("paginaOriginal") != null)
            {
                urlOrigen = Session.Get("paginaOriginal").ToString();
                Session.Remove("paginaOriginal");
            }
            else if (Request.Headers.ContainsKey("Referer")
               && !Request.Headers["Referer"].ToString().Contains("?redirect=")
               && Request.Headers["Referer"].ToString().StartsWith(ProyectoSeleccionado.UrlPropia(IdiomaUsuario))
               && (proyectoSinURL || Request.Headers["Referer"].ToString().Contains(ProyectoSeleccionado.NombreCorto))
               && new Uri(Request.Headers["Referer"].ToString()).Host.Replace("www.", "").Equals(mControladorBase.DominoAplicacion.Substring(1))
               && Request.Headers["Referer"].ToString() != mControladorBase.UrlsSemanticas.GetURLHacerseMiembroComunidad(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto)
               && Request.Headers["Referer"].ToString() != mControladorBase.UrlsSemanticas.GetURLHacerseMiembroComunidad(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, true))
            {
                string urlRedirectHome = ObtenerUrlHomeConectado();

                urlOrigen = Request.Headers["Referer"].ToString();

                string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);

                if (!string.IsNullOrEmpty(urlRedirectHome) && urlOrigen.TrimEnd('/').Equals(urlComunidad))
                {
                    urlOrigen = urlRedirectHome;
                }
            }

            if (Request.Headers.ContainsKey("Referer") && !string.IsNullOrEmpty(new Uri(Request.Headers["Referer"]).Query) && !string.IsNullOrEmpty(System.Web.HttpUtility.ParseQueryString(new Uri(Request.Headers["Referer"]).Query)["urlOrigen"]))
            {
                urlOrigen = System.Web.HttpUtility.ParseQueryString(new Uri(Request.Headers["Referer"]).Query)["urlOrigen"];
            }

            string cadenaRedirect = "/redirect/";

            if (string.IsNullOrEmpty(urlOrigen) && Request.Path.ToString().Contains(cadenaRedirect))
            {
                urlOrigen = Request.Path.ToString();
            }


            if (urlOrigen != null && urlOrigen.Contains(cadenaRedirect))
            {
                //Se está redireccionando así mismo, extraigo la url de redirect:
                string url = urlOrigen;
                url = url.Substring(url.IndexOf(cadenaRedirect) + cadenaRedirect.Length);

                if ((!url.StartsWith("http://")) && (!url.StartsWith("https://")))
                {
                    string dominioComunidad = ProyectoSeleccionado.UrlPropia(IdiomaUsuario);
                    if (!url.StartsWith("/"))
                    {
                        url = "/" + url;
                    }
                    url = dominioComunidad + url;
                }

                urlOrigen = url;
            }

            ViewBag.UrlOrigen = urlOrigen;

            if (ViewBag.Cabecera is HeaderModel)
            {
                HeaderModel cabecera = ViewBag.Cabecera;

                if (cabecera.SocialNetworkRegister != null)
                {
                    cabecera.SocialNetworkRegister = new Dictionary<string, string>();
                    if (ObtenerParametrosLoginExterno(TipoRedSocialLogin.Facebook, ParametroProyecto, ParametrosAplicacionDS).Count > 0)
                    {
                        string urlServicioLoginFacebook = mControladorBase.UrlServicioLogin + "/loginfacebook.aspx?token=" + System.Net.WebUtility.UrlEncode(TokenLoginUsuario) + "&proyectoID=" + ProyectoSeleccionado.Clave.ToString() + "&urlOrigen=" + System.Net.WebUtility.UrlEncode(urlOrigen);

                        urlServicioLoginFacebook = AgregarEventoComunidad(urlServicioLoginFacebook);

                        cabecera.SocialNetworkRegister.Add("Facebook", urlServicioLoginFacebook);
                    }

                    if (ObtenerParametrosLoginExterno(TipoRedSocialLogin.Google, ParametroProyecto, ParametrosAplicacionDS).Count > 0)
                    {
                        string urlServicioLoginGoogle = mControladorBase.UrlServicioLogin + "/logingoogle.aspx?token=" + System.Net.WebUtility.UrlEncode(TokenLoginUsuario) + "&proyectoID=" + ProyectoSeleccionado.Clave.ToString() + "&urlOrigen=" + System.Net.WebUtility.UrlEncode(urlOrigen);

                        urlServicioLoginGoogle = AgregarEventoComunidad(urlServicioLoginGoogle);

                        cabecera.SocialNetworkRegister.Add("Google", urlServicioLoginGoogle);
                    }

                    if (ObtenerParametrosLoginExterno(TipoRedSocialLogin.Twitter, ParametroProyecto, ParametrosAplicacionDS).Count > 0)
                    {
                        string urlServicioLoginTwitter = mControladorBase.UrlServicioLogin + "/logintwitter.aspx?token=" + System.Net.WebUtility.UrlEncode(TokenLoginUsuario) + "&proyectoID=" + ProyectoSeleccionado.Clave.ToString() + "&urlOrigen=" + System.Net.WebUtility.UrlEncode(urlOrigen);

                        urlServicioLoginTwitter = AgregarEventoComunidad(urlServicioLoginTwitter);

                        cabecera.SocialNetworkRegister.Add("Twitter", urlServicioLoginTwitter);
                    }
                    if (ObtenerParametrosLoginExterno(TipoRedSocialLogin.Santillana, ParametroProyecto, ParametrosAplicacionDS).Count > 0)
                    {
                        string urlServicioLoginSantillana = mControladorBase.UrlServicioLogin + "/loginSantillana.aspx?token=" + System.Net.WebUtility.UrlEncode(TokenLoginUsuario) + "&proyectoID=" + ProyectoSeleccionado.Clave.ToString() + "&urlOrigen=" + System.Net.WebUtility.UrlEncode(UrlOrigen);

                        urlServicioLoginSantillana = AgregarEventoComunidad(urlServicioLoginSantillana);

                        cabecera.SocialNetworkRegister.Add("Santillana", urlServicioLoginSantillana);
                    }
                }
            }
        }

        private string AgregarEventoComunidad(string pUrlServicioLogin)
        {
            if (!string.IsNullOrEmpty(RequestParams("eventoComID")))
            {
                pUrlServicioLogin += "&eventoComID=" + RequestParams("eventoComID");
            }

            return pUrlServicioLogin;
        }

        public void CargarCamposRegistroProyecto(AutenticationModel pRegistro)
        {
            if (DatosExtraProyectoDataWrapperProyecto.ListaCamposRegistroProyectoGenericos.Count > 0)
            {
                foreach (CamposRegistroProyectoGenericos fila in DatosExtraProyectoDataWrapperProyecto.ListaCamposRegistroProyectoGenericos)
                {
                    switch (fila.Tipo)
                    {
                        case (short)TipoCampoGenericoRegistro.Pais:
                            pRegistro.AskCountry = true;
                            if (ParametroProyecto.ContainsKey(ParametroAD.IdiomaRegistroDefecto))
                            {
                                Guid id;
                                if (Guid.TryParse(ParametroProyecto[ParametroAD.IdiomaRegistroDefecto], out id))
                                {
                                    pRegistro.CountryID = id;
                                }
                            }
                            if (pRegistro.CountryList == null || pRegistro.CountryList.Count == 0)
                            {
                                pRegistro.CountryList = new Dictionary<Guid, string>();
                                foreach (AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
                                {
                                    pRegistro.CountryList.Add(filaPais.PaisID, filaPais.Nombre);

                                    if ((pRegistro.CountryID == null || pRegistro.CountryID.Equals(Guid.Empty)) && filaPais.Nombre.Equals("España"))
                                    {
                                        pRegistro.CountryID = filaPais.PaisID;
                                    }
                                }
                            }
                            break;

                        case (short)TipoCampoGenericoRegistro.Provincia:
                            CargarDatosProvincia(pRegistro.CountryID);
                            pRegistro.AskRegion = true;
                            break;

                        case (short)TipoCampoGenericoRegistro.Localidad:
                            pRegistro.AskLocation = true;
                            break;
                        case (short)TipoCampoGenericoRegistro.Sexo:
                            pRegistro.AskGender = true;
                            break;
                    }
                }
            }
        }

        private void CargarDatosProvincia(Guid pPaisID)
        {
            registro.RegionList = new Dictionary<Guid, string>();
            foreach (AD.EntityModel.Models.Pais.Provincia fila in PaisesDW.ListaProvincia.Where(provincia => provincia.PaisID == pPaisID))
            {
                registro.RegionList.Add(fila.ProvinciaID, fila.Nombre);
            }
        }

        public void CargarCamposExtraRegistro(AutenticationModel pRegistro)
        {
            pRegistro.AdditionalFields = new List<AdditionalFieldAutentication>();

            if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.Count > 0 || DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.Count > 0)
            {
                foreach (DatoExtraEcosistema fila in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.Where(dato => dato.Paso1Registro == true).OrderBy(dato => dato.Orden))
                {
                    Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();
                    foreach (DatoExtraEcosistemaOpcion filaDatoExtraEcosistemaOpcion in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaOpcion.Where(dato => dato.DatoExtraID.Equals(fila.DatoExtraID)).OrderBy(dato => dato.Orden))
                    {
                        Guid opcionID = filaDatoExtraEcosistemaOpcion.OpcionID;
                        string opcion = UtilCadenas.ObtenerTextoDeIdioma(filaDatoExtraEcosistemaOpcion.Opcion, UtilIdiomas.LanguageCode, "es");

                        listaOpciones.Add(opcionID, opcion);
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, "es");
                    campoExtra.FieldName = fila.DatoExtraID.ToString();
                    campoExtra.Required = fila.Obligatorio;
                    campoExtra.Options = listaOpciones;

                    pRegistro.AdditionalFields.Add(campoExtra);
                }

                foreach (DatoExtraProyecto fila in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.Where(dato => dato.Paso1Registro == true).OrderBy(dato => dato.Orden))
                {
                    Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();
                    foreach (DatoExtraProyectoOpcion filaDatoExtraProyectoOpcion in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoOpcion.Where(dato => dato.DatoExtraID.Equals(fila.DatoExtraID)).OrderBy(dato => dato.Orden))
                    {
                        Guid opcionID = filaDatoExtraProyectoOpcion.OpcionID;
                        string opcion = UtilCadenas.ObtenerTextoDeIdioma(filaDatoExtraProyectoOpcion.Opcion, UtilIdiomas.LanguageCode, "es");

                        listaOpciones.Add(opcionID, opcion);
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, "es");
                    campoExtra.FieldName = fila.DatoExtraID.ToString();
                    campoExtra.Required = fila.Obligatorio;
                    campoExtra.Options = listaOpciones;

                    pRegistro.AdditionalFields.Add(campoExtra);
                }
            }

            if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Count > 0 || DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Count > 0)
            {
                foreach (DatoExtraEcosistemaVirtuoso fila in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Where(dato => dato.Paso1Registro == true).OrderBy(dato => dato.Orden))
                {

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, "es");
                    campoExtra.FieldName = fila.InputID;
                    campoExtra.Required = fila.Obligatorio;
                    if (fila.InputID.Equals("ddlPais"))
                    {
                        Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();
                        foreach (AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
                        {

                            listaOpciones.Add(filaPais.PaisID, filaPais.Nombre);
                        }
                        campoExtra.Options = listaOpciones;
                    }
                    else if (!string.IsNullOrEmpty(fila.QueryVirtuoso))
                    {
                        campoExtra.DependencyFields = fila.InputsSuperiores;
                        campoExtra.AutoCompleted = true;
                    }

                    pRegistro.AdditionalFields.Add(campoExtra);
                }

                foreach (DatoExtraProyectoVirtuoso fila in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Where(dato => dato.Paso1Registro == true).OrderBy(dato => dato.Orden))
                {
                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, "es");
                    campoExtra.FieldName = fila.InputID;
                    campoExtra.Required = fila.Obligatorio;
                    if (!string.IsNullOrEmpty(fila.QueryVirtuoso))
                    {
                        campoExtra.DependencyFields = fila.InputsSuperiores;
                        campoExtra.AutoCompleted = true;
                    }
                    pRegistro.AdditionalFields.Add(campoExtra);
                }
            }
        }

        private string RegistrarUsuarioEnComunidad(Identidad pIdentidadPerfil, out bool pInvitacionAceptadaOtraIdentidad)
        {
            pInvitacionAceptadaOtraIdentidad = false;

            //Comprobar si es un perfil de tipo personal o uno de tipo organización o de un tipo organización
            if (pIdentidadPerfil.PersonaID.HasValue && (pIdentidadPerfil.Tipo.Equals(TiposIdentidad.Personal) || pIdentidadPerfil.Tipo.Equals(TiposIdentidad.Profesor)))
            {
                ControladorIdentidades.RegistrarPerfilPersonalEnProyecto(mControladorBase.UsuarioActual.UsuarioID, pIdentidadPerfil, ProyectoSeleccionado);

                ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                if (ProyectoSeleccionado.ListaTipoProyectoEventoAccion.ContainsKey(TipoProyectoEventoAccion.Registro))
                {
                    proyectoCL.AgregarEventosAccionProyectoPorProyectoYUsuarioID(ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, TipoProyectoEventoAccion.Registro);
                }
                proyectoCL.Dispose();
            }
            else if (!pIdentidadPerfil.PersonaID.HasValue && (pIdentidadPerfil.Tipo.Equals(TiposIdentidad.Organizacion)))
            {
                if (!IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(identidad => identidad.PerfilID.Equals(pIdentidadPerfil.PerfilID) && identidad.ProyectoID.Equals(ProyectoSeleccionado.Clave)))
                {
                    AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(pIdentidadPerfil.PerfilID));
                    AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion filaPerfilOrganizacion = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfilOrganizacion.FirstOrDefault(perfilOrg => perfilOrg.PerfilID.Equals(pIdentidadPerfil.PerfilID));
                    ControladorIdentidades.RegistrarOrganizacionEnProyecto(filaPerfil, filaPerfilOrganizacion, ProyectoSeleccionado);
                }

                if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
                {
                    Perfil perfilPersonaOrg = IdentidadActual.GestorIdentidades.ObtenerPerfilDePersonaEnOrganizacion(pIdentidadPerfil.OrganizacionID.Value, IdentidadActual.PersonaID.Value);
                    TiposIdentidad tipoIdentidadMyGnoss = IdentidadActual.GestorIdentidades.ObtenerPerfilDePersonaEnOrganizacion(pIdentidadPerfil.OrganizacionID.Value, IdentidadActual.PersonaID.Value).IdentidadMyGNOSS.Tipo;
                    ControladorIdentidades.RegistrarUsuarioEnProyecto(mControladorBase.UsuarioActual.UsuarioID, perfilPersonaOrg, ProyectoSeleccionado.Clave, tipoIdentidadMyGnoss, null);
                }

                pInvitacionAceptadaOtraIdentidad = true;
            }

            //Recargamos el gestor de identidades de la identidad actual
            //Obtener las identidades del usuario actual
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCL.EliminarCacheGestorTodasIdentidadesUsuario(mControladorBase.UsuarioActual.UsuarioID, mControladorBase.UsuarioActual.PersonaID);

            List<string> listaClavesInvalidar = new List<string>();

            string prefijoClave;

            if (!string.IsNullOrEmpty(identidadCL.Dominio))
            {
                prefijoClave = identidadCL.Dominio;
            }
            else
            {
                prefijoClave = IdentidadCL.DominioEstatico;
            }

            prefijoClave = prefijoClave + "_" + identidadCL.ClaveCache[0] + "_";
            prefijoClave = prefijoClave.ToLower();

            string rawKey = string.Concat("IdentidadActual_", IdentidadActual.PersonaID, "_", IdentidadActual.PerfilID);
            listaClavesInvalidar.Add(prefijoClave + rawKey.ToLower());
            string rawKey2 = "PerfilMVC_" + IdentidadActual.PerfilID;
            listaClavesInvalidar.Add(prefijoClave + rawKey2.ToLower());

            identidadCL.InvalidarCachesMultiples(listaClavesInvalidar);
            identidadCL.Dispose();

            //Limpiamos la cache de contactos para el proyecto
            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            amigosCL.InvalidarAmigosPertenecenProyecto(ProyectoSeleccionado.Clave);
            amigosCL.Dispose();

            if (Request.Method.Equals("POST"))
            {
                string clausulasSelecc = Request.Form["clausulasSelecc"];

                List<Guid> listaClausulas = new List<Guid>();
                if (!string.IsNullOrEmpty(clausulasSelecc))
                {
                    string[] clausulasTexto = clausulasSelecc.Split(',');

                    foreach (string clausula in clausulasTexto)
                    {
                        if (!string.IsNullOrEmpty(clausula))
                        {
                            listaClausulas.Add(new Guid(clausula));
                        }
                    }
                }
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionUsuarios gestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerClausulasRegitroProyectoYUsuario(ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID), mLoggingService, mEntityContext, mConfigService);

                //gestorUsuarios.UsuarioDS.Merge(new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD).ObtenerClausulasRegitroProyecto(ProyectoSeleccionado.Clave));
                //gestorUsuarios.UsuarioDS.Merge(new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD).ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto));

                //UsuarioCN usuarioCN2 = new UsuarioCN(mEntityContext, mLoggingService, mConfigService);
                //UsuarioDS usuDS = usuarioCN2.ObtenerClausulasRegitroPorID(listaClausulas);
                //usuarioCN2.Dispose();

                Dictionary<KeyValuePair<Guid, Guid>, List<Guid>> listaProyectoClausulas = new Dictionary<KeyValuePair<Guid, Guid>, List<Guid>>();
                foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in ClausulasRegistro)
                {
                    KeyValuePair<Guid, Guid> orgProy = new KeyValuePair<Guid, Guid>(filaClausula.OrganizacionID, filaClausula.ProyectoID);
                    if (listaProyectoClausulas.ContainsKey(orgProy))
                    {
                        listaProyectoClausulas[orgProy].Add(filaClausula.ClausulaID);
                    }
                    else
                    {
                        List<Guid> nuevaLista = new List<Guid>();
                        nuevaLista.Add(filaClausula.ClausulaID);
                        listaProyectoClausulas.Add(orgProy, nuevaLista);
                    }
                }

                foreach (KeyValuePair<Guid, Guid> proyectoClausulaID in listaProyectoClausulas.Keys)
                {
                    gestorUsuarios.AgregarClausulasAdicionalesRegistroProy(mControladorBase.UsuarioActual.UsuarioID, proyectoClausulaID.Key, proyectoClausulaID.Value, listaProyectoClausulas[proyectoClausulaID]);
                }
                usuarioCN.ActualizarUsuario(false);
            }

            if (InvitacionAEventoComunidad != null && InvitacionAEventoComunidad.TipoEvento.Equals((short)TipoEventoProyecto.SinRestriccion))
            {
                ControladorIdentidades.AceptarExtrasInvitacionAComunidad(IdentidadActual.Persona, InvitacionAComunidad, ProyectoSeleccionado, UrlIntragnoss, InvitacionAEventoComunidad, UtilIdiomas.LanguageCode);
            }

            Session.Set("paginaOriginal", UrlOrigen);

            //Insertar en la tabla UsuarioRedirect el valor del redirect
            UsuarioCN usuarioCN2 = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            string urlRedireccion = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/1";

            // Si es una organización, redirigir a la home de la comunidad
            if (pIdentidadPerfil.OrganizacionID.HasValue)
            {
                urlRedireccion = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
            }

            return urlRedireccion;
        }

        private string RegistrarUsuario(string pEmail, TipoRedSocialLogin? pTipoRedSocial, string pIdRedSocial, SolicitudNuevoUsuario pSolicitud)
        {

            string nombre = "";
            string apellidos = "";
            string nombreUsuario = "";
            string email = "";
            string password = "";
            string fechaNacimiento = "";
            string clausulasSelecc = "";
            string emailTutor = "";

            string gruposEnRedSocial = "";
            string tokenEnRedSocial = "";

            if (!HaySolicitudPrevia)
            {
                nombre = Request.Form["txtNombre"].ToString().Trim();
                apellidos = Request.Form["txtApellidos"].ToString().Trim();
                nombreUsuario = Request.Form["txtNombreUsuario"];
                email = Request.Form["txtEmail"];
                emailTutor = Request.Form["txtEmailTutor"];
                password = Request.Form["txtContrasenya"];
                fechaNacimiento = Request.Form["txtFechaNac"];
                clausulasSelecc = Request.Form["clausulasSelecc"];

                gruposEnRedSocial = Request.Form["txtGruposRed"];
                tokenEnRedSocial = Request.Form["txtTokenRed"];
            }


            if (!string.IsNullOrEmpty(RequestParams("registroRedesSociales")))
            {
                string caracteres = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@$?";
                Byte[] randomBytes = new Byte[12];
                char[] passw = new char[12];

                Random random = new Random(DateTime.Now.Millisecond);
                for (int i = 0; i < 12; i++)
                {
                    random.NextBytes(randomBytes);
                    passw[i] = caracteres[(int)randomBytes[i] % caracteres.Length];
                }
                password = new string(passw);
                email = pEmail;
            }

            string nombreCortoUsuario = "";
            DataWrapperUsuario dataWrapperUsuario = null;
            GestionUsuarios gestorUsuarios = null;
            UsuarioGnoss usuario = null;
            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = null;
            Solicitud filaSolicitud = null;
            SolicitudNuevoUsuario filaNuevoUsuario = null;

            if (HaySolicitudPrevia)
            {

                nombreCortoUsuario = pSolicitud.NombreCorto;
                UsuarioCN userCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                dataWrapperUsuario = new DataWrapperUsuario();
                dataWrapperUsuario.ListaUsuario.Add(userCN.ObtenerUsuarioPorID(pSolicitud.UsuarioID));
                userCN.Dispose();
                gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);
                gestorUsuarios.CargarGestor();

                if (gestorUsuarios.ListaUsuarios.ContainsKey(pSolicitud.UsuarioID))
                {
                    usuario = gestorUsuarios.ListaUsuarios[pSolicitud.UsuarioID];
                }

                if(usuario != null)
                {
                    filaUsuario = usuario.FilaUsuario;
                    filaSolicitud = pSolicitud.Solicitud;
                    filaSolicitud.SolicitudID = pSolicitud.SolicitudID;
                    filaSolicitud.FechaSolicitud = pSolicitud.Solicitud.FechaSolicitud;

                    filaNuevoUsuario = pSolicitud;
                }
                else
                {
                    mLoggingService.GuardarLogError("El usuario es nulo para la solicitud: " + pSolicitud.Solicitud);
                    throw new ExcepcionGeneral("El usuario es nulo para la solicitud: " + pSolicitud.Solicitud);
                }
                
            }
            else
            {
                if (!String.IsNullOrEmpty(nombreUsuario))
                {
                    nombreUsuario = nombreUsuario.Trim();
                }
                string loginUsuario = nombreUsuario;
                //Obtener el Login
                if (string.IsNullOrEmpty(loginUsuario))
                {
                    loginUsuario = UtilCadenas.LimpiarCaracteresNombreCortoRegistro(nombre) + '-' + UtilCadenas.LimpiarCaracteresNombreCortoRegistro(apellidos);

                    if (loginUsuario.Length > 12)
                    {
                        loginUsuario = loginUsuario.Substring(0, 12);
                    }

                    int hashNumUsu = 1;
                    while (ExisteUsuario(loginUsuario))
                    {
                        loginUsuario = loginUsuario.Substring(0, loginUsuario.Length - hashNumUsu.ToString().Length) + hashNumUsu.ToString();
                        hashNumUsu++;
                    }

                    nombreCortoUsuario = loginUsuario;
                    hashNumUsu = 1;
                    while (ExisteNombreCorto(nombreCortoUsuario))
                    {
                        nombreCortoUsuario = nombreCortoUsuario.Substring(0, nombreCortoUsuario.Length - hashNumUsu.ToString().Length) + hashNumUsu.ToString();
                        hashNumUsu++;
                    }
                }
                else
                {
                    nombreCortoUsuario = loginUsuario;
                }

                //Usuario
                dataWrapperUsuario = new DataWrapperUsuario();
                gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);
                usuario = gestorUsuarios.AgregarUsuario(loginUsuario, nombreCortoUsuario, HashHelper.CalcularHash(password, true), true, RegistroAutomaticoEcosistema);

                filaUsuario = usuario.FilaUsuario;
                filaUsuario.EstaBloqueado = false;

                //TODO inicializar mSolicitudDW
                filaSolicitud = new Solicitud();
                filaSolicitud.SolicitudID = Guid.NewGuid();
                filaSolicitud.FechaSolicitud = DateTime.Now;
                filaSolicitud.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaSolicitud.ProyectoID = ProyectoSeleccionado.Clave;

                if (!string.IsNullOrEmpty(Request.Form["txtProyRegistro"]))
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Guid proyRegistro = proyCN.ObtenerProyectoIDPorNombreCorto(Request.Form["txtProyRegistro"]);

                    if (proyRegistro != Guid.Empty)
                    {
                        filaSolicitud.ProyectoID = proyRegistro;
                    }
                }

                filaNuevoUsuario = new SolicitudNuevoUsuario();
            }

            if (!HaySolicitudPrevia && fechaNacimiento != null)
            {
                try
                {
                    string[] fecha = fechaNacimiento.Split('/');

                    int dia = int.Parse(fecha[0]);
                    int mes = int.Parse(fecha[1]);
                    int anio = int.Parse(fecha[2]);

                    filaNuevoUsuario.FechaNacimiento = new DateTime(anio, mes, dia);
                }
                catch 
                {
                    mLoggingService.GuardarLog("Fallo obtener la fecha de nacimiento del usuario: " + filaNuevoUsuario.UsuarioID);
                }
            }

            if (RegistroAutomaticoEcosistema || mControladorBase.RegistroAutomaticoEnComunidad || (InvitacionAOrganizacion != null || InvitacionAComunidad != null && filaNuevoUsuario.FechaNacimiento.HasValue && ControladorIdentidades.ComprobarPersonaEsMayorAnios(filaNuevoUsuario.FechaNacimiento.Value, 14)) || HaySolicitudPrevia)
            {
                filaSolicitud.Estado = (short)EstadoSolicitud.Aceptada;
            }
            else
            {
                filaSolicitud.Estado = (short)EstadoSolicitud.Espera;
            }

            filaSolicitud.FechaProcesado = filaSolicitud.FechaSolicitud;

            Dictionary<Guid, Guid> dicDatosExtraProyecto = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Guid> dicDatosExtraEcosistema = new Dictionary<Guid, Guid>();

            Dictionary<int, string> dicDatosExtraProyectoVirtuoso = new Dictionary<int, string>();
            Dictionary<int, string> dicDatosExtraEcosistemaVirtuoso = new Dictionary<int, string>();
            if (!HaySolicitudPrevia)
            {
                SolicitudDW.ListaSolicitud.Add(filaSolicitud);
                mEntityContext.Solicitud.Add(filaSolicitud);

                filaNuevoUsuario.SolicitudID = filaSolicitud.SolicitudID;
                filaNuevoUsuario.UsuarioID = filaUsuario.UsuarioID;
                filaNuevoUsuario.NombreCorto = nombreCortoUsuario;
                filaNuevoUsuario.Nombre = nombre;
                filaNuevoUsuario.Apellidos = apellidos;
                filaNuevoUsuario.Email = email;
                filaNuevoUsuario.EmailTutor = emailTutor;
                filaNuevoUsuario.EsBuscable = true;
                filaNuevoUsuario.EsBuscableExterno = false;

                if (SaltarPasosRegistro)
                {
                    Response.Cookies.Append("skp-stps", "true", new CookieOptions { Expires = DateTime.Now.AddHours(1), Domain = mControladorBase.DominoAplicacion });
                }

                if (!ParametrosGeneralesRow.PrivacidadObligatoria)
                {
                    filaNuevoUsuario.EsBuscable = false;
                    filaNuevoUsuario.EsBuscableExterno = false;
                }

                if (registro.AskCountry)
                {
                    filaNuevoUsuario.PaisID = new Guid(Request.Form["ddlPais"]);
                }

                filaNuevoUsuario.Provincia = "";
                if (registro.AskRegion)
                {
                    if (!string.IsNullOrEmpty(Request.Form["ddlProvincia"]) && registro.CountryList[new Guid(Request.Form["ddlPais"])].Equals("España"))
                    {
                        filaNuevoUsuario.ProvinciaID = new Guid(Request.Form["ddlProvincia"]);
                        filaNuevoUsuario.Provincia = registro.RegionList[filaNuevoUsuario.ProvinciaID.Value];
                    }
                    else if (!string.IsNullOrEmpty(Request.Form["txtProvincia"]))
                    {
                        filaNuevoUsuario.Provincia = Request.Form["txtProvincia"];
                    }
                }

                if (registro.AskLocation || !string.IsNullOrEmpty(Request.Form["txtLocalidad"]))
                {
                    filaNuevoUsuario.Poblacion = Request.Form["txtLocalidad"];
                }

                if (registro.AskGender)
                {
                    filaNuevoUsuario.Sexo = Request.Form["ddlSexo"];
                }

                filaNuevoUsuario.FaltanDatos = false;
                filaNuevoUsuario.Idioma = UtilIdiomas.LanguageCode;

                AdditionalFieldAutentication campoExtraPais = registro.AdditionalFields.Find(campoExtra => campoExtra.FieldName == "ddlPais");
                if (campoExtraPais != null && campoExtraPais.FieldValue != null && campoExtraPais.FieldValue != Guid.Empty.ToString())
                {
                    filaNuevoUsuario.PaisID = new Guid(campoExtraPais.FieldValue);
                }


                if (clausulasSelecc != "")
                {
                    string clausulasAdicionales = "";
                    foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in ClausulasRegistro)
                    {
                        if (filaClausula.Tipo < (short)TipoClausulaAdicional.MensajeLoguado && filaClausula.Tipo == (short)TipoClausulaAdicional.Opcional)
                        {
                            if (clausulasSelecc.Contains(filaClausula.ClausulaID.ToString()))
                            {
                                clausulasAdicionales += filaClausula.ClausulaID + ",";
                            }
                        }
                    }
                    filaNuevoUsuario.ClausulasAdicionales = clausulasAdicionales;
                }

                //Dictionary<Guid, Guid> dicDatosExtraProyecto = new Dictionary<Guid, Guid>();
                //Dictionary<Guid, Guid> dicDatosExtraEcosistema = new Dictionary<Guid, Guid>();

                //Dictionary<int, string> dicDatosExtraProyectoVirtuoso = new Dictionary<int, string>();
                //Dictionary<int, string> dicDatosExtraEcosistemaVirtuoso = new Dictionary<int, string>();

                foreach (AdditionalFieldAutentication campoExtra in registro.AdditionalFields)
                {
                    string nombreCampo = campoExtra.FieldName;
                    string valorCampo = RequestParams(nombreCampo);

                    Guid guidNombreCampo = Guid.Empty;
                    if (Guid.TryParse(nombreCampo, out guidNombreCampo))
                    {
                        Guid guidValorCampo = Guid.Empty;
                        if (Guid.TryParse(valorCampo, out guidValorCampo) && !guidValorCampo.Equals(Guid.Empty))
                        {
                            DatoExtraEcosistema filaDatoExtraEcosistema = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.FirstOrDefault(dato => dato.DatoExtraID.Equals(guidNombreCampo));
                            if (filaDatoExtraEcosistema != null)
                            {
                                dicDatosExtraEcosistema.Add(filaDatoExtraEcosistema.DatoExtraID, guidValorCampo);
                            }
                            else
                            {
                                DatoExtraProyecto filaDatoExtraProyecto = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.FirstOrDefault(dato => dato.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && dato.ProyectoID.Equals(ProyectoSeleccionado.Clave) && dato.DatoExtraID.Equals(guidNombreCampo));
                                if (filaDatoExtraProyecto != null)
                                {
                                    dicDatosExtraProyecto.Add(filaDatoExtraProyecto.DatoExtraID, guidValorCampo);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(valorCampo))
                        {
                            List<DatoExtraEcosistemaVirtuoso> filasDatoExtraEcosistemaVirtuoso = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Where(dato => dato.Paso1Registro == true && dato.InputID.Equals(nombreCampo)).ToList();
                            if (filasDatoExtraEcosistemaVirtuoso.Count > 0)
                            {
                                dicDatosExtraEcosistemaVirtuoso.Add(filasDatoExtraEcosistemaVirtuoso[0].Orden, valorCampo);

                            }
                            else
                            {
                                List<DatoExtraProyectoVirtuoso> filasDatoExtraProyectoVirtuoso = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Where(dato => dato.Paso1Registro == true && dato.InputID.Equals(nombreCampo)).ToList();
                                if (filasDatoExtraProyectoVirtuoso.Count > 0)
                                {
                                    dicDatosExtraProyectoVirtuoso.Add(filasDatoExtraProyectoVirtuoso[0].Orden, valorCampo);

                                }
                            }
                        }
                    }
                }
                filaNuevoUsuario.Solicitud = filaSolicitud;
                SolicitudDW.ListaSolicitudNuevoUsuario.Add(filaNuevoUsuario);
                mEntityContext.SolicitudNuevoUsuario.Add(filaNuevoUsuario);
                //Recogemos y agregamos los campos extra
                GuardarDatosExtra(SolicitudDW, filaSolicitud, dicDatosExtraEcosistema, dicDatosExtraProyecto);
                GuardarDatosExtraVirtuoso(SolicitudDW, filaSolicitud, dicDatosExtraEcosistemaVirtuoso, dicDatosExtraProyectoVirtuoso);
            }

            Session.Remove("SeguridadRegistro");
            Session.Remove("CaptchaImageText");

            dicDatosExtraProyecto = new Dictionary<Guid, Guid>();
            dicDatosExtraEcosistema = new Dictionary<Guid, Guid>();

            dicDatosExtraProyectoVirtuoso = new Dictionary<int, string>();
            dicDatosExtraEcosistemaVirtuoso = new Dictionary<int, string>();
            //Registro validado, recogemos los parámetros extra y los agregamos a BD.     
            if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.Count > 0 || DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.Count > 0)
            {
                foreach (DatoExtraEcosistemaOpcionSolicitud filaEcosistema in SolicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud.ToList())
                {
                    if (mEntityContext.Entry(filaEcosistema).State != EntityState.Deleted)
                    {
                        dicDatosExtraEcosistema.Add(filaEcosistema.DatoExtraID, filaEcosistema.OpcionID);
                    }
                }

                foreach (DatoExtraProyectoOpcionSolicitud filaProy in mSolicitudDW.ListaDatoExtraProyectoOpcionSolicitud.ToList())
                {
                    if (mEntityContext.Entry(filaProy).State != EntityState.Deleted)
                    {
                        dicDatosExtraProyecto.Add(filaProy.DatoExtraID, filaProy.OpcionID);
                    }
                }
            }

            if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Count > 0 || DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Count > 0)
            {
                foreach (DatoExtraEcosistemaVirtuosoSolicitud filaEcosistema in mSolicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud.ToList())
                {
                    if (mEntityContext.Entry(filaEcosistema).State != EntityState.Deleted)
                    {
                        dicDatosExtraEcosistemaVirtuoso.Add(DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.FirstOrDefault(dato => dato.DatoExtraID.Equals(filaEcosistema.DatoExtraID)).Orden, filaEcosistema.Opcion);
                    }
                }

                foreach (DatoExtraProyectoVirtuosoSolicitud filaProy in mSolicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud.ToList())
                {
                    if (mEntityContext.Entry(filaProy).State != EntityState.Deleted)
                    {
                        dicDatosExtraProyectoVirtuoso.Add((int)DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.FirstOrDefault(dato => dato.DatoExtraID.Equals(filaProy.DatoExtraID)).Orden, filaProy.Opcion);
                    }
                }
            }

            string emailAUtilizar = filaNuevoUsuario.Email;
            if (string.IsNullOrEmpty(emailAUtilizar))
            {
                emailAUtilizar = filaNuevoUsuario.EmailTutor;
            }

            JsonEstado jsonEstadoValidar = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.ValidacionRegistro, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, filaNuevoUsuario.Nombre, filaNuevoUsuario.Apellidos, emailAUtilizar, password, GestorParametroAplicacion, DatosExtraProyectoDataWrapperProyecto, dicDatosExtraEcosistemaVirtuoso, dicDatosExtraProyectoVirtuoso, null, filaNuevoUsuario.Idioma);

            if (jsonEstadoValidar != null && !jsonEstadoValidar.Correcto)
            {
                string error = jsonEstadoValidar.InfoExtra;
                //lblErroresRegistroExterno.Text = error;
                //divKoErroresRegistroExrterno.Visible = true;
                //CargarClausulasAdicionales(ProyectoSeleccionado.Clave);

                registro.Errors.Add(error);
                return string.Empty;
            }

            if (RegistroAutomaticoEcosistema || mControladorBase.RegistroAutomaticoEnComunidad || InvitacionAOrganizacion != null || (InvitacionAComunidad != null && filaNuevoUsuario.FechaNacimiento.HasValue && ControladorIdentidades.ComprobarPersonaEsMayorAnios(filaNuevoUsuario.FechaNacimiento.Value, 14)) || HaySolicitudPrevia)
            {
                JsonEstado jsonEstado = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.Registro, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, filaNuevoUsuario.Nombre, filaNuevoUsuario.Apellidos, emailAUtilizar, password, GestorParametroAplicacion, DatosExtraProyectoDataWrapperProyecto, dicDatosExtraEcosistemaVirtuoso, dicDatosExtraProyectoVirtuoso, null, filaNuevoUsuario.Idioma);

                if (jsonEstado != null && !jsonEstado.Correcto)
                {
                    string error = jsonEstado.InfoExtra;
                    //lblErroresRegistroExterno.Text = error;
                    //divKoErroresRegistroExrterno.Visible = true;
                    //CargarClausulasAdicionales(ProyectoSeleccionado.Clave);

                    registro.Errors.Add(error);
                    return string.Empty;
                }

                int? dia = null;
                int? mes = null;
                int? anio = null;
                if (!HaySolicitudPrevia)
                {
                    if (ParametrosGeneralesRow.FechaNacimientoObligatoria)
                    {
                        string[] fecha = fechaNacimiento.Split('/');

                        dia = int.Parse(fecha[0]);
                        mes = int.Parse(fecha[1]);
                        anio = int.Parse(fecha[2]);
                    }
                }

                if (SolicitudPrevia != null || !HaySolicitudPrevia)
                {
                    new ControladorIdentidades(null, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AceptarUsuario(IdentidadActual, filaSolicitud.SolicitudID, mSolicitudDW, dataWrapperUsuario, ParametrosGeneralesRow, ProyectoVirtual, anio, mes, dia, InvitacionAEventoComunidad, ParametroProyecto, BaseURL, InvitacionAComunidad, UrlIntragnoss, UrlIntragnossServicios, InvitacionAOrganizacion, pTipoRedSocial, pIdRedSocial, HaySolicitudPrevia);

                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    if (ProyectoSeleccionado.ListaTipoProyectoEventoAccion.ContainsKey(TipoProyectoEventoAccion.Registro))
                    {
                        proyectoCL.AgregarEventosAccionProyectoPorProyectoYUsuarioID(ProyectoSeleccionado.Clave, filaUsuario.UsuarioID, TipoProyectoEventoAccion.Registro);
                    }
                    proyectoCL.Dispose();

                    mControladorBase.ResetearIdentidadActual();
                    //mControladorBase.LimpiarSesion(new string[0]);

                    if (!string.IsNullOrEmpty(gruposEnRedSocial) && gruposEnRedSocial.Length > 0)
                    {
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();
                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        LiveUsuariosCL liveUsuariosCL = new LiveUsuariosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

                        GestionIdentidades gestIdent = new GestionIdentidades(dataWrapperIdentidad, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                        GestionUsuarios gestUsu = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);

                        string[] listaProyectos = gruposEnRedSocial.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string proyectoyGrupos in listaProyectos)
                        {
                            string[] listaGrupos = proyectoyGrupos.Split('|');
                            Guid proyectoID = proyCL.ObtenerProyectoIDPorNombreCorto(listaGrupos[0]);
                            if (!proyectoID.Equals(Guid.Empty))
                            {
                                Guid identidadID = IdentidadActual.GestorIdentidades.ObtenerIdentidadDeProyecto(proyectoID, mControladorBase.UsuarioActual.PersonaID);
                                if (identidadID.Equals(Guid.Empty))
                                {
                                    identidadID = identidadCN.ObtenerIdentidadUsuarioEnProyecto(gestUsu.DataWrapperUsuario.ListaUsuario.First().UsuarioID, proyectoID);
                                }

                                for (int i = 1; i < listaGrupos.Length; i++)
                                {
                                    string nombreGrupo = listaGrupos[i];

                                    Guid grupoID = identidadCN.ObtenerGrupoIDPorNombreYProyectoID(nombreGrupo, proyectoID);
                                    string nombrecortoGrupo = identidadCN.ObtenerNombreCortoGrupoPorID(grupoID);
                                    if (grupoID != Guid.Empty)
                                    {
                                        AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();
                                        filaGrupoIdentidadesParticipacion.GrupoID = grupoID;
                                        filaGrupoIdentidadesParticipacion.IdentidadID = identidadID;
                                        filaGrupoIdentidadesParticipacion.FechaAlta = DateTime.Now;
                                        gestIdent.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);

                                        //Asignar la caché del grupo al usuario
                                        liveUsuariosCL.ClonarLiveGrupoProyectoAUsu(usuario.Clave, proyectoID, grupoID);
                                        ActualizarGrupoBase(grupoID, proyectoID);
                                        identidadCL.InvalidarCacheGrupoPorNombreCortoYProyecto(nombrecortoGrupo, proyectoID);
                                    }
                                }

                                identidadCL.InvalidarCacheMiembrosComunidad(proyectoID);
                            }
                        }
                        identidadCN.Dispose();
                        identidadCL.Dispose();
                        liveUsuariosCL.Dispose();

                        mEntityContext.SaveChanges();
                    }

                    if (pTipoRedSocial == TipoRedSocialLogin.Santillana)
                    {
                        Dictionary<string, string> listaParametros = ObtenerParametrosLoginExterno(TipoRedSocialLogin.Santillana, ParametroProyecto, ParametrosAplicacionDS);
                        string url = listaParametros["url"];
                        //pIDenRedSocial

                        ////Si no tiene token "TokenAltaUsuarioApiLogin", lo creamos y se lo mandamos a su api
                        SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        Guid tokenAlta = solicitudCN.ObtenerTokenAltaUsuarioAPILogin(mControladorBase.UsuarioActual.UsuarioID);

                        if (tokenAlta == Guid.Empty)
                        {
                            Guid token = Guid.NewGuid();
                            solicitudCN.GuardarTokenAltaUsuarioAPILogin(mControladorBase.UsuarioActual.UsuarioID, token, ProyectoSeleccionado.FilaProyecto.NombreCorto);
                            solicitudCN.GuardarTokenAccesoAPILogin(token, mControladorBase.UsuarioActual.Login);
                            solicitudCN.Dispose();

                            string finalUrl = url + "/Autentication/ActualizarTokenUsuario?peticionid=" + pIdRedSocial + "&token_id=" + tokenEnRedSocial + "&login_gnoss=" + mControladorBase.UsuarioActual.Login + "&token_gnoss=" + token.ToString();
                            mLoggingService.AgregarEntrada("CrearUsuario Santillana - '" + finalUrl + "'");
                            mLoggingService.GuardarLogError(new Exception("CrearUsuario Santillana - '" + finalUrl + "'"));
                            string datosActualidados = UtilWeb.WebRequest(UtilWeb.Metodo.GET, finalUrl, "");
                        }

                    }

                    TipoProyectoEventoAccion tipoAccion = TipoProyectoEventoAccion.Registro;
                    switch (pTipoRedSocial)
                    {
                        case TipoRedSocialLogin.Facebook:
                            tipoAccion = TipoProyectoEventoAccion.RegistroFacebook;
                            break;
                        case TipoRedSocialLogin.Google:
                            tipoAccion = TipoProyectoEventoAccion.RegistroGoogle;
                            break;
                        case TipoRedSocialLogin.Twitter:
                            tipoAccion = TipoProyectoEventoAccion.RegistroTwitter;
                            break;
                        case TipoRedSocialLogin.Santillana:
                            tipoAccion = TipoProyectoEventoAccion.RegistroSantillana;
                            break;
                    }

                    if (ProyectoSeleccionado.ListaTipoProyectoEventoAccion.ContainsKey(tipoAccion))
                    {
                        proyectoCL.AgregarEventosAccionProyectoPorProyectoYUsuarioID(ProyectoSeleccionado.Clave, filaUsuario.UsuarioID, tipoAccion);
                    }
                    proyectoCL.Dispose();

                    UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    if (HaySolicitudPrevia && (SolicitudPrevia == null || SolicitudPrevia.FechaNacimiento.HasValue && ControladorIdentidades.ComprobarPersonaEsMayorAnios(SolicitudPrevia.FechaNacimiento.Value, 14)))
                    {
                        GnossIdentity identity = null;
                        if (usuCN.ObtenerUsuarioPorID(filaUsuario.UsuarioID).EstaBloqueado.HasValue && !(usuCN.ObtenerUsuarioPorID(filaUsuario.UsuarioID).EstaBloqueado.Value))
                        {
                            identity = mControladorBase.ValidarUsuario(usuCN.ObtenerUsuarioPorID(filaUsuario.UsuarioID).Login);
                        }
                        if (identity != null)
                        {
                            this.Session.Clear();
                            mControladorBase.UsuarioActual.UsarMasterParaLectura = true;
                            Session.Set("Usuario", identity);
                            Session.Set("MantenerConectado", true);
                            Session.Set("CrearCookieEnServicioLogin", true);
                            Session.Remove("EnvioCookie");

                            mControladorBase.CrearCookieLogueado();
                        }
                    }
                    usuCN.Dispose();

                    //if (InvitacionAOrganizacion != null)
                    //{
                    //    AceptamosInvitacionOrganizacion();
                    //}

                    Session.Set("paginaOriginal", UrlOrigen);
                }
                else
                {
                    //Añadir el flag de Validado    
                    UsuarioCN usuaCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                    filaUsuario.Validado = (short)ValidacionUsuario.Verificado;
                    if (HaySolicitudPrevia && SolicitudPrevia.FechaNacimiento.HasValue && ControladorIdentidades.ComprobarPersonaEsMayorAnios(SolicitudPrevia.FechaNacimiento.Value, 14))
                    {
                        GnossIdentity identity = null;
                        if (usuaCN.ObtenerUsuarioPorID(filaUsuario.UsuarioID).EstaBloqueado.HasValue && !usuaCN.ObtenerUsuarioPorID(filaUsuario.UsuarioID).EstaBloqueado.Value)
                        {
                            identity = mControladorBase.ValidarUsuario(usuaCN.ObtenerUsuarioPorID(filaUsuario.UsuarioID).Login);
                        }

                        if (identity != null)
                        {
                            this.Session.Clear();
                            mControladorBase.UsuarioActual.UsarMasterParaLectura = true;
                            Session.Set("Usuario", identity);
                            Session.Set("MantenerConectado", true);
                            Session.Set("CrearCookieEnServicioLogin", true);
                            Session.Remove("EnvioCookie");

                            mControladorBase.CrearCookieLogueado();
                        }
                    }

                    usuaCN.ActualizarUsuario(false);
                    usuaCN.Dispose();
                }

            }
            else
            {
                JsonEstado jsonEstado = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.SolicitudRegistro, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, filaNuevoUsuario.Nombre, filaNuevoUsuario.Apellidos, emailAUtilizar, password, GestorParametroAplicacion, DatosExtraProyectoDataWrapperProyecto, dicDatosExtraEcosistemaVirtuoso, dicDatosExtraProyectoVirtuoso, null, filaNuevoUsuario.Idioma);

                if (jsonEstado != null && !jsonEstado.Correcto)
                {
                    string error = jsonEstado.InfoExtra;
                    //lblErroresRegistroExterno.Text = error;
                    //divKoErroresRegistroExrterno.Visible = true;
                    //CargarClausulasAdicionales(ProyectoSeleccionado.Clave);

                    registro.Errors.Add(error);
                    return string.Empty;
                }

                mEntityContext.SaveChanges();

                DataWrapperNotificacion notifDW = new DataWrapperNotificacion();
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notifDW, IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                //Enviamos un correo de congirmación
                SolicitudNuevoUsuario filaSolicitudNuevoUsuario2 = mSolicitudDW.ListaSolicitudNuevoUsuario.FirstOrDefault();

                string urlEnlace2 = BaseURLIdioma;
                if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MyGnoss))
                {
                    urlEnlace2 = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                }

                urlEnlace2 += "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/" + UtilIdiomas.GetText("URLSEM", "ACTIVACIONGNOSS") + "/" + filaSolicitudNuevoUsuario2.SolicitudID;

                if (InvitacionAComunidad != null && filaNuevoUsuario.FechaNacimiento.HasValue &&
                    ControladorIdentidades.ComprobarPersonaEsMayorAnios(filaNuevoUsuario.FechaNacimiento.Value, 14))
                {
                    urlEnlace2 += "/" + UtilIdiomas.GetText("URLSEM", "PETICIONGNOSS") + "/" + InvitacionAComunidad.Clave;
                }
                else if (InvitacionAOrganizacion != null)
                {
                    urlEnlace2 += "/" + UtilIdiomas.GetText("URLSEM", "PETICIONGNOSS") + "/" + InvitacionAOrganizacion.Clave;
                }
                else if (InvitacionAEventoComunidad != null)
                {
                    urlEnlace2 += "/" + UtilIdiomas.GetText("URLSEM", "PETICIONGNOSS") + "/" + InvitacionAEventoComunidad.EventoID;
                }

                //Diferenciar entre el email de tutor o de usuario normal.
                if (string.IsNullOrEmpty(filaNuevoUsuario.EmailTutor) || !NecesitaTutor(fechaNacimiento))
                {
                    gestorNotificaciones.AgregarNotificacionSolicitudEntradaGNOSS(filaSolicitudNuevoUsuario2.SolicitudID, filaSolicitudNuevoUsuario2.Nombre, null, TiposNotificacion.SolicitudNuevoUsuario, filaNuevoUsuario.Email, null, this.BaseURLIdioma, filaSolicitudNuevoUsuario2.Idioma, urlEnlace2, "", ProyectoSeleccionado.Clave);
                }
                else
                {
                    ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    Elementos.ServiciosGenerales.Proyecto proyecto = ProyectoSeleccionado;
                    if (!proyecto.Clave.Equals(filaSolicitud.ProyectoID))
                    {
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        proyecto = new GestionProyecto(proyCL.ObtenerProyectoPorID(filaSolicitud.ProyectoID), mLoggingService, mEntityContext).ListaProyectos.Values.First();
                    }
                    List<Identidad> ListaAdministradores = controladorProyecto.CargarAdministradoresProyecto(proyecto);
                    string nombreAdmin = ListaAdministradores.First().Nombre(IdentidadActual.Clave);
                    //Es necesario compilar el servicio de correo tambien.
                    gestorNotificaciones.AgregarNotificacionSolicitudEntradaGNOSS(filaSolicitudNuevoUsuario2.SolicitudID, $"{filaSolicitudNuevoUsuario2.Nombre} {filaSolicitudNuevoUsuario2.Apellidos}", nombreAdmin, TiposNotificacion.SolicitudNuevoUsuarioTutor, filaNuevoUsuario.EmailTutor, null, this.BaseURLIdioma, filaSolicitudNuevoUsuario2.Idioma, urlEnlace2, UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.Nombre, UtilIdiomas.LanguageCode, IdiomaPorDefecto), ProyectoSeleccionado.Clave);
                    /*gestorNotificaciones.AgregarNotificacionSolicitudEntradaGNOSS(filaSolicitudNuevoUsuario2.SolicitudID, filaSolicitudNuevoUsuario2.Nombre, null, TiposNotificacion.SolicitudNuevoUsuario, filaNuevoUsuario.EmailTutor, null, this.BaseURLIdioma, filaSolicitudNuevoUsuario2.Idioma, urlEnlace2, "", ProyectoSeleccionado.Clave);*/
                }

                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                notificacionCN.ActualizarNotificacion();
                notificacionCN.Dispose();
                gestorNotificaciones.Dispose();
            }

            string cookieSeguridadRegistro = Request.Cookies["SeguridadRegistro"];

            if (cookieSeguridadRegistro != null)
            {
                Response.Cookies.Append("SeguridadRegistro", cookieSeguridadRegistro, new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            }

            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            string urlRedirect = "";

            if (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido)
            {
                urlRedirect = UrlOrigen;
            }
            else if (!RegistroAutomaticoEcosistema && InvitacionAOrganizacion == null && (InvitacionAComunidad == null || filaNuevoUsuario.FechaNacimiento.HasValue && !ControladorIdentidades.ComprobarPersonaEsMayorAnios(filaNuevoUsuario.FechaNacimiento.Value, 14)))
            {
                if (HaySolicitudPrevia)
                {
                    if (mSolicitudPreActivacionRegistro != null)
                    {
                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        TipoProyecto tipoProyecto = proyCN.ObtenerTipoProyecto(SolicitudPrevia.Solicitud.ProyectoID);
                        ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        string nombreCorto = proyectoCL.ObtenerNombreCortoProyecto(SolicitudPrevia.Solicitud.ProyectoID);
                        //Insertar en la tabla UsuarioRedirect el valor del redirect
                        if (!SaltarPasosRegistro && ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Count > 0 && !tipoProyecto.Equals(TipoProyecto.EducacionExpandida) && !tipoProyecto.Equals(TipoProyecto.EducacionPrimaria))
                        {
                            List<ProyectoPasoRegistro> filasPasos = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.OrderBy(item => item.Orden).ToList();

                            ProyectoPasoRegistro filaActual = filasPasos.FirstOrDefault();
                            if (filaActual.PasoRegistro.Equals("Datos") || filaActual.PasoRegistro.Equals("Preferencias") || filaActual.PasoRegistro.Equals("Conecta"))
                            {
                                urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/1";
                            }
                            else
                            {
                                urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + filaActual.PasoRegistro;

                            }
                        }
                        else
                        {
                            urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, nombreCorto);
                            if (!SaltarPasosRegistro && !tipoProyecto.Equals(TipoProyecto.EducacionExpandida) && !tipoProyecto.Equals(TipoProyecto.EducacionPrimaria))
                            {
                                //Insertar en la tabla UsuarioRedirect el valor del redirect
                                urlRedirect = urlRedirect + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/1";
                            }
                            else if (SaltarPasosRegistro && !string.IsNullOrEmpty(UrlRedireccionTrasRegistro))
                            {
                                urlRedirect = UrlRedireccionTrasRegistro;
                            }
                        }

                        if (string.IsNullOrEmpty(filaNuevoUsuario.Email) || SolicitudPrevia.FechaNacimiento.HasValue && !ControladorIdentidades.ComprobarPersonaEsMayorAnios(SolicitudPrevia.FechaNacimiento.Value, 14))
                        {
                            //Redirección a una pantalla donde le muestre al tutor que ha terminado el registro   

                            urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, nombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROTUTOR");
                            return urlRedirect;
                        }
                        else if (!SaltarPasosRegistro && !tipoProyecto.Equals(TipoProyecto.EducacionExpandida) && !tipoProyecto.Equals(TipoProyecto.EducacionPrimaria))
                        {
                            usuarioCN.InsertarUrlRedirect(filaUsuario.UsuarioID, urlRedirect);
                        }
                    }
                    else
                    {
                        urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(email))
                    {
                        urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROCOMPLETO") + "/tutor";
                        this.Response.Redirect(urlRedirect);
                        this.Response.StatusCode = StatusCodes.Status200OK;
                    }
                    else
                    {
                        urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROCOMPLETO");
                        this.Response.Redirect(urlRedirect);
                        this.Response.StatusCode = StatusCodes.Status200OK;
                    }
                }
            }
            else if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                urlRedirect = BaseURL + "/home";
            }
            else if (ParametroProyecto.ContainsKey(ParametroAD.RegistroSinPasos) && ParametroProyecto[ParametroAD.RegistroSinPasos].Equals("1"))
            {
                urlRedirect = UrlOrigen;
            }
            else
            {
                if (!SaltarPasosRegistro && ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Count > 0 && !ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionExpandida) && !ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionPrimaria))
                {
                    List<ProyectoPasoRegistro> filasPasos = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.OrderBy(item => item.Orden).ToList();

                    ProyectoPasoRegistro filaActual = filasPasos.FirstOrDefault();
                    if (filaActual.PasoRegistro.Equals("Datos") || filaActual.PasoRegistro.Equals("Preferencias") || filaActual.PasoRegistro.Equals("Conecta"))
                    {
                        urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/1";
                    }
                    else
                    {
                        urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + filaActual.PasoRegistro;

                    }
                }
                else
                {
                    urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                    if (!SaltarPasosRegistro && !ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionExpandida) && !ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionPrimaria))
                    {
                        //Insertar en la tabla UsuarioRedirect el valor del redirect
                        urlRedirect = urlRedirect + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/1";
                    }
                    else if (SaltarPasosRegistro && !string.IsNullOrEmpty(UrlRedireccionTrasRegistro))
                    {
                        urlRedirect = UrlRedireccionTrasRegistro;
                    }
                }

                usuarioCN.InsertarUrlRedirect(filaUsuario.UsuarioID, urlRedirect);

                /*//Insertar en la tabla UsuarioRedirect el valor del redirect
                urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                if (!SaltarPasosRegistro && !ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionExpandida) && !ProyectoSeleccionado.TipoProyecto.Equals(TipoProyecto.EducacionPrimaria))
                {
                    //Insertar en la tabla UsuarioRedirect el valor del redirect
                    urlRedirect = urlRedirect + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/1";
                    usuarioCN.InsertarUrlRedirect(filaUsuario.UsuarioID, urlRedirect);
                }*/
            }


            string nombreCortoUsu = usuarioCN.ObtenerNombreCortoUsuarioPorID(mControladorBase.UsuarioActual.UsuarioID);
            usuarioCN.Dispose();

            string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();

            string query = "usuarioID=" + mControladorBase.UsuarioActual.UsuarioID + "&loginUsuario=" + mControladorBase.UsuarioActual.Login + "&idioma=" + IdiomaUsuario + "&personaID=" + mControladorBase.UsuarioActual.PersonaID + "&nombreCorto=" + nombreCortoUsu;
            query += "&token=" + TokenLoginUsuario + "&redirect=" + urlRedirect;
            string redirectEncode = UrlEncode($"{urlServicioLogin}/crearCookie?{query}");
            return urlServicioLogin + "/eliminarcookie?nuevoEnvio=true&usuarioID=" + mControladorBase.UsuarioActual.UsuarioID + "&dominio=" + BaseURL + "/&redirect=" + redirectEncode;
        }

        /// <summary>
        /// Cambia el estado del campo "Validado" de la tabla usuario a 1
        /// </summary>
        /// <param name="pUsuarioID">Id del usuario a validar</param>
        /// <returns></returns>
        private string ValidarUsuario(Guid pUsuarioID)
        {
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            usuarioCN.ValidarUsuario(pUsuarioID);

            usuarioCN.ActualizarUsuario(true);

            string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();

            string urlRedirect = UrlOrigen;

            string nombreCortoUsu = usuarioCN.ObtenerNombreCortoUsuarioPorID(mControladorBase.UsuarioActual.UsuarioID);
            usuarioCN.Dispose();

            string query = "usuarioID=" + mControladorBase.UsuarioActual.UsuarioID + "&loginUsuario=" + mControladorBase.UsuarioActual.Login + "&idioma=" + IdiomaUsuario + "&personaID=" + mControladorBase.UsuarioActual.PersonaID + "&nombreCorto=" + nombreCortoUsu;
            query += "&token=" + TokenLoginUsuario + "&redirect=" + urlRedirect;

            return urlServicioLogin + "/eliminarcookie?nuevoEnvio=true&usuarioID=" + mControladorBase.UsuarioActual.UsuarioID + "&dominio=" + BaseURL + "/&redirect=" + urlServicioLogin + "/crearCookie?" + query;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pGrupoID"></param>
        private void ActualizarGrupoBase(Guid pGrupoID, Guid pProyectoID)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            int tablaBaseProyecto = proyectoCN.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            proyectoCN.Dispose();

            BasePerOrgComunidadDS basePerOrgComDS = new BasePerOrgComunidadDS();

            BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org_Add = basePerOrgComDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();

            filaColaTagsCom_Per_Org_Add.TablaBaseProyectoID = tablaBaseProyecto;
            filaColaTagsCom_Per_Org_Add.Tags = Constantes.PERS_U_ORG + "g" + Constantes.PERS_U_ORG + ", " + Constantes.ID_TAG_PER + pGrupoID + Constantes.ID_TAG_PER;
            filaColaTagsCom_Per_Org_Add.Tipo = (short)TiposElementosEnCola.Agregado;
            filaColaTagsCom_Per_Org_Add.Estado = 0;
            filaColaTagsCom_Per_Org_Add.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsCom_Per_Org_Add.Prioridad = (short)PrioridadBase.Alta;

            basePerOrgComDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org_Add);

            BaseComunidadCN basePerOrgComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            basePerOrgComCN.InsertarFilasEnRabbit("ColaTagsCom_Per_Org", basePerOrgComDS);
            basePerOrgComCN.Dispose();
        }

        /// <summary>
        /// Guarda los datos extra
        /// </summary>
        private void GuardarDatosExtra(DataWrapperSolicitud pSolicitudDW, Solicitud pSolicitud, Dictionary<Guid, Guid> pDicDatosExtraEcosistema, Dictionary<Guid, Guid> pDicDatosExtraProyecto)
        {
            if (!HaySolicitudPrevia)
            {
                foreach (DatoExtraEcosistemaOpcionSolicitud filaDEEcosistemaOS in pSolicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud.ToList())
                {
                    pSolicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud.Remove(filaDEEcosistemaOS);
                    mEntityContext.EliminarElemento(filaDEEcosistemaOS);
                }

                foreach (DatoExtraProyectoOpcionSolicitud filaDEProyectoOS in pSolicitudDW.ListaDatoExtraProyectoOpcionSolicitud.ToList())
                {
                    pSolicitudDW.ListaDatoExtraProyectoOpcionSolicitud.Remove(filaDEProyectoOS);
                    mEntityContext.EliminarElemento(filaDEProyectoOS);
                }
            }

            //Guardamos los datos en DatoExtraSolicitudProyecto, DatoExtraSolicitudEcosistema
            foreach (Guid datoExtra in pDicDatosExtraEcosistema.Keys)
            {
                if (pDicDatosExtraEcosistema[datoExtra] != Guid.Empty)
                {
                    //Ecosistema
                    DatoExtraEcosistema filaDatoExtraEcosistema = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.Where(dato => dato.DatoExtraID.Equals(datoExtra)).First();

                    Guid valor = pDicDatosExtraEcosistema[datoExtra];
                    DatoExtraEcosistemaOpcionSolicitud datoExtraEco = new DatoExtraEcosistemaOpcionSolicitud { DatoExtraID = filaDatoExtraEcosistema.DatoExtraID, OpcionID = valor, SolicitudID = pSolicitud.SolicitudID };

                    pSolicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud.Add(datoExtraEco);
                    mEntityContext.DatoExtraEcosistemaOpcionSolicitud.Add(datoExtraEco);
                }
            }

            foreach (Guid datoExtra in pDicDatosExtraProyecto.Keys)
            {
                if (pDicDatosExtraProyecto[datoExtra] != Guid.Empty)
                {
                    //ProyectoVirtuoso
                    DatoExtraProyecto filaDatoExtraProyecto = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.Where(dato => dato.DatoExtraID.Equals(datoExtra)).First();

                    Guid valor = pDicDatosExtraProyecto[datoExtra];
                    DatoExtraProyectoOpcionSolicitud datoExtraProy = new DatoExtraProyectoOpcionSolicitud { OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoID = ProyectoSeleccionado.Clave, DatoExtraID = filaDatoExtraProyecto.DatoExtraID, OpcionID = valor, SolicitudID = pSolicitud.SolicitudID };
                    pSolicitudDW.ListaDatoExtraProyectoOpcionSolicitud.Add(datoExtraProy);
                    mEntityContext.DatoExtraProyectoOpcionSolicitud.Add(datoExtraProy);
                }
            }


            Session.Remove("DicDatosExtraEcosistema");
            Session.Remove("DicDatosExtraProyecto");
        }

        /// <summary>
        /// Guarda los datos extra tanto de virtuoso como del ecosistema.
        /// </summary>
        private void GuardarDatosExtraVirtuoso(DataWrapperSolicitud pSolicitudDW, Solicitud pSolicitud, Dictionary<int, string> pDicDatosExtraEcosistemaVirtuoso, Dictionary<int, string> pDicDatosExtraProyectoVirtuoso)
        {
            if (!HaySolicitudPrevia)
            {
                foreach (DatoExtraEcosistemaVirtuosoSolicitud filaDEEcosistemaVS in pSolicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud.ToList())
                {
                    pSolicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud.Remove(filaDEEcosistemaVS);
                    mEntityContext.DatoExtraEcosistemaVirtuosoSolicitud.Remove(filaDEEcosistemaVS);
                }

                foreach (DatoExtraProyectoVirtuosoSolicitud filaDEProyectoVS in pSolicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud.ToList())
                {
                    pSolicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud.Remove(filaDEProyectoVS);
                    mEntityContext.DatoExtraProyectoVirtuosoSolicitud.Remove(filaDEProyectoVS);
                }
            }

            //Guardamos los datos en DatoExtraSolicitudProyectoVirtuoso, DatoExtraSolicitudEcosistemaVirtuoso
            foreach (int orden in pDicDatosExtraEcosistemaVirtuoso.Keys)
            {
                if (!string.IsNullOrEmpty(pDicDatosExtraEcosistemaVirtuoso[orden].Trim()) && pDicDatosExtraEcosistemaVirtuoso[orden].Trim() != "|")
                {
                    //EcosistemaVirtuoso
                    DatoExtraEcosistemaVirtuoso depvr = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Where(dato => dato.Orden.Equals(orden)).FirstOrDefault();

                    string valor = pDicDatosExtraEcosistemaVirtuoso[orden];
                    if (valor.EndsWith("|"))
                    {
                        valor = valor.Substring(0, valor.Length - 1);
                    }

                    valor = ObtenerPais(valor);

                    pSolicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud.Add(new DatoExtraEcosistemaVirtuosoSolicitud { DatoExtraID = depvr.DatoExtraID, SolicitudID = pSolicitud.SolicitudID, Opcion = valor });
                }
            }

            foreach (int orden in pDicDatosExtraProyectoVirtuoso.Keys)
            {
                if (!string.IsNullOrEmpty(pDicDatosExtraProyectoVirtuoso[orden].Trim()) && pDicDatosExtraProyectoVirtuoso[orden].Trim() != "|")
                {
                    //ProyectoVirtuoso
                    DatoExtraProyectoVirtuoso depvr = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Where(dato => dato.Orden.Equals(orden)).FirstOrDefault();

                    string valor = pDicDatosExtraProyectoVirtuoso[orden];
                    if (valor.EndsWith("|"))
                    {
                        valor = valor.Substring(0, valor.Length - 1);
                    }

                    valor = ObtenerPais(valor);

                    List<DatoExtraProyectoVirtuosoSolicitud> filasDatoExtraSolicitud = pSolicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud.Where(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.DatoExtraID.Equals(depvr.DatoExtraID) && item.SolicitudID.Equals(pSolicitud.SolicitudID)).ToList();
                    if (filasDatoExtraSolicitud != null && filasDatoExtraSolicitud.Count > 0)
                    {
                        filasDatoExtraSolicitud[0].Opcion = valor;
                    }
                    else
                    {
                        pSolicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud.Add(new DatoExtraProyectoVirtuosoSolicitud { OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoID = ProyectoSeleccionado.Clave, DatoExtraID = depvr.DatoExtraID, SolicitudID = pSolicitud.SolicitudID, Opcion = valor });
                    }
                }
            }

            Session.Remove("DicDatosExtraEcosistemaVirtuoso");
            Session.Remove("DicDatosExtraProyectoVirtuoso");
        }

        /// <summary>
        /// Seguridad para que no se registren bots, ya que hemos quitado el captcha
        /// </summary>
        public void EstablecerSeguridad(AutenticationModel pRegistro, ISession pSession, HttpResponse pResponse)
        {
            //Este guid se lo mandaremos a la pagina, para comprobar que antes de cada registro se pide esta pagina
            Guid guidSeguridad = Guid.NewGuid();
            //Este guid se lo mandamos a la cache, si no hay cache, no se podran registrar (Los bots no suelen tener cache)
            Guid guidSeguridadCookie = Guid.NewGuid();
            //Guardamos la fecha actual para comprobar que tarda mas de 5 segundos, si tarda menos es un bot.
            DateTime fechaActual = DateTime.Now;

            //En cada registro, el guid de seguridad (El que mandamos a la pagina) y el Guid de la cookie, son un par unico
            Dictionary<string, object> seguridadRegistro = new Dictionary<string, object>();
            seguridadRegistro.Add("guidSeguridad", guidSeguridad);
            seguridadRegistro.Add("guidSeguridadCookie", guidSeguridadCookie);
            seguridadRegistro.Add("fechaActual", fechaActual);


            pSession.Set("SeguridadRegistro", seguridadRegistro);

            pResponse.Cookies.Append("SeguridadRegistro", guidSeguridad + "|" + guidSeguridadCookie, new CookieOptions { Expires = DateTime.Today.AddDays(1).AddSeconds(-1), Domain = mControladorBase.DominoAplicacion });

            pRegistro.SecurityID = guidSeguridad;
        }

        private List<string> ComprobarErroresRegistroClausulas()
        {
            List<string> listaErroresClausulas = new List<string>();

            string clausulasSelecc = Request.Form["clausulasSelecc"];

            foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in ClausulasRegistro)
            {
                if (filaClausula.Tipo < (short)TipoClausulaAdicional.MensajeLoguado && filaClausula.Tipo != (short)TipoClausulaAdicional.Opcional)
                {
                    if (!clausulasSelecc.Contains(filaClausula.ClausulaID.ToString()))
                    {
                        listaErroresClausulas.Add("clausulaObligatoria_" + filaClausula.ClausulaID);
                    }
                }
            }

            return listaErroresClausulas;
        }

        private bool ComprobarErroresRegistro(string pEmail, string pEmailTutor = null)
        {
            string mensaje = "";
            try
            {
                mensaje += System.Environment.NewLine + "comienzo comprobaciones";
                List<string> errores = new List<string>();

                string nombre = Request.Form["txtNombre"].ToString().Trim();
                string apellidos = Request.Form["txtApellidos"].ToString().Trim();
                string nombreUsuario = Request.Form["txtNombreUsuario"];
                string email = pEmail;
                string emailTutor = pEmailTutor;
                string password = Request.Form["txtContrasenya"];
                string fechaNacimiento = Request.Form["txtFechaNac"];
                string fechaNacimientoChecked = Request.Form["cbMayorEdad"];
                string guidSeguridad = Request.Form["txtSeguridad"];
                string textocaptcha = Request.Form["captcha"];
                Guid paisID = Guid.Empty;
                Guid provinciaID = Guid.Empty;
                string provincia = "";
                string localidad = "";
                string sexo = "";

                mensaje += System.Environment.NewLine + "comprobaciones request hechas";
                mensaje += System.Environment.NewLine + "registro null: " + registro == null;

                if (registro.AskCountry)
                {
                    mensaje += System.Environment.NewLine + "entro if country";
                    //está vacío o sin definir
                    if (string.IsNullOrEmpty(Request.Form["ddlPais"]) || Request.Form["ddlPais"].Equals(Guid.Empty))
                    {
                        mensaje += System.Environment.NewLine + "errores";
                        errores.Add("camposVacios");
                    }
                    else
                    {
                        mensaje += System.Environment.NewLine + "request";
                        paisID = new Guid(Request.Form["ddlPais"]);
                    }
                }
                mensaje += System.Environment.NewLine + "país comprobado";
                if (registro.AskRegion)
                {
                    mensaje += System.Environment.NewLine + "entro if region";
                    //si el país seleccionado no es España, hay que comprobar txtProvincia
                    if (!registro.CountryList[new Guid(Request.Form["ddlPais"])].Equals("España"))
                    {
                        mensaje += System.Environment.NewLine + "CoutnryList";
                        if (string.IsNullOrEmpty(Request.Form["txtProvincia"]))
                        {
                            mensaje += System.Environment.NewLine + "errores";
                            errores.Add("camposVacios");
                        }
                        else
                        {
                            mensaje += System.Environment.NewLine + "request";
                            provincia = Request.Form["txtProvincia"];
                        }
                    }
                    else
                    {
                        mensaje += System.Environment.NewLine + "ddlProvincia";
                        provinciaID = new Guid(Request.Form["ddlProvincia"]);
                    }
                }

                mensaje += System.Environment.NewLine + "región comprobada";

                if (registro.AskLocation)
                {
                    mensaje += System.Environment.NewLine + "if location";
                    if (string.IsNullOrEmpty(Request.Form["txtLocalidad"]))
                    {
                        mensaje += System.Environment.NewLine + "if errores";
                        errores.Add("camposVacios");
                    }
                    else
                    {
                        mensaje += System.Environment.NewLine + "request";
                        localidad = Request.Form["txtLocalidad"];
                    }
                }

                mensaje += System.Environment.NewLine + "location comprobada";

                if (registro.AskGender)
                {
                    mensaje += System.Environment.NewLine + "if gender";
                    //está vacío o sin definir
                    if (string.IsNullOrEmpty(Request.Form["ddlSexo"]) || Request.Form["ddlSexo"].Equals("0"))
                    {
                        mensaje += System.Environment.NewLine + "errores";
                        errores.Add("camposVacios");
                    }
                    else
                    {
                        mensaje += System.Environment.NewLine + "request";
                        sexo = Request.Form["ddlSexo"];
                    }
                }
                mensaje += System.Environment.NewLine + "genero comprobado";
                bool mostrarCaptcha = false;
                bool mostrarErrorCaptcha = false;
                bool mostrarErrorMaximoIntentos = false;

                string expRegCaracteresRepetidos = "(.)\\1{2,}";
                string expRegNombres = "^([a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ]+[a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ\\s-]*[a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ]+)$";
                string expRegPassword = "(?!^[0-9]*$)(?!^[a-zA-ZñÑüÜ]*$)^([a-zA-ZñÑüÜ0-9#_$*]{6,12})$";

                if (!string.IsNullOrEmpty(RequestParams("registroRedesSociales")))
                {
                    mensaje += System.Environment.NewLine + "registroredessociales";
                    if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) /*|| string.IsNullOrEmpty(email)*/ || (registro.AskBornDate && string.IsNullOrEmpty(fechaNacimiento)) || (!registro.AskBornDate && (string.IsNullOrEmpty(fechaNacimientoChecked) || fechaNacimientoChecked.Equals("off"))))
                    {
                        errores.Add("camposVacios");
                    }
                    mensaje += System.Environment.NewLine + "registroredessociales ok";
                }
                else
                {
                    mensaje += System.Environment.NewLine + "campos vacíos";
                    if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) /*|| string.IsNullOrEmpty(email)*/ || string.IsNullOrEmpty(password) || (registro.AskBornDate && string.IsNullOrEmpty(fechaNacimiento)) || (!registro.AskBornDate && (string.IsNullOrEmpty(fechaNacimientoChecked) || fechaNacimientoChecked.Equals("off"))))
                    {
                        errores.Add("camposVacios");
                    }
                    mensaje += System.Environment.NewLine + "campos vacios ok";
                }

                mensaje += System.Environment.NewLine + "expresion regular nombre";
                if (Regex.Match(nombre, expRegNombres).Length == 0 || Regex.Match(nombre, expRegCaracteresRepetidos).Length != 0)
                {
                    errores.Add("formatoNombre");
                }
                mensaje += System.Environment.NewLine + "expresion regular apellidos";

                if (Regex.Match(apellidos, expRegNombres).Length == 0 || Regex.Match(apellidos, expRegCaracteresRepetidos).Length != 0)
                {
                    errores.Add("formatoApellidos");
                }
                if (!string.IsNullOrEmpty(email))
                {
                    mensaje += System.Environment.NewLine + "validar email";
                    if (!UtilCadenas.ValidarEmail(email))
                    {
                        mensaje += System.Environment.NewLine + "no valida email";
                        errores.Add("formatoEmail");
                    }
                    else if (ExisteEmail(email))
                    {
                        mensaje += System.Environment.NewLine + "existe email";
                        errores.Add("emailrepetido");
                    }
                }

                if (string.IsNullOrEmpty(RequestParams("registroRedesSociales")))
                {
                    mensaje += System.Environment.NewLine + "registro redes sociales x2";
                    if (Regex.Match(password, expRegPassword).Length == 0)
                    {
                        errores.Add("formatoPassword");
                    }
                }
                mensaje += System.Environment.NewLine + "comprobar registro clausulas";
                errores.AddRange(ComprobarErroresRegistroClausulas());

                mensaje += System.Environment.NewLine + "additionalFields";
                foreach (AdditionalFieldAutentication campoExtra in registro.AdditionalFields)
                {
                    mensaje += System.Environment.NewLine + "additional field " + campoExtra.FieldName;
                    string nombreCampo = campoExtra.FieldName;
                    string valorCampo = RequestParams(nombreCampo);

                    campoExtra.FieldValue = valorCampo;

                    if (campoExtra.Required && string.IsNullOrEmpty(valorCampo))
                    {
                        if (!errores.Contains("camposVacios"))
                        {
                            errores.Add("camposVacios");
                        }
                    }
                }

                if (string.IsNullOrEmpty(emailTutor) && NecesitaTutor(fechaNacimiento))
                {
                    mensaje += System.Environment.NewLine + "Mayor 14 años ";
                    errores.Add("emailtutor");
                }

                if (!string.IsNullOrEmpty(nombreUsuario))
                {
                    if (nombreUsuario.Length > 12)
                    {
                        mensaje += System.Environment.NewLine + "El nombre de Usuario debe tener como maximo 12 caracteres";
                        errores.Add("caracteresusuario");
                    }
                    if (ExisteUsuario(nombreUsuario) || ExisteNombreCorto(nombreUsuario))
                    {
                        mensaje += System.Environment.NewLine + "El nombre de Usuario ya existe";
                        errores.Add("yaexisteusuario");
                    }
                }
                //Dictionary<string, object> seguridadRegistro;
                //mensaje += System.Environment.NewLine + "seguridad registro";

                //if (Session["SeguridadRegistro"] == null)
                //{
                //    mensaje += System.Environment.NewLine + "no hay sesión";
                //    seguridadRegistro = new Dictionary<string, object>();
                //    mostrarCaptcha = true;
                //}
                //else
                //{
                //    mensaje += System.Environment.NewLine + "hay sesión";
                //    seguridadRegistro = (Dictionary<string, object>)Session["SeguridadRegistro"];

                //    if (seguridadRegistro.ContainsKey("fechaActual"))
                //    {
                //        mensaje += System.Environment.NewLine + "fecha actual";
                //        TimeSpan diffFechas = DateTime.Now - (DateTime)seguridadRegistro["fechaActual"];
                //        if (diffFechas < new TimeSpan(0, 0, 5))
                //        {
                //            mostrarCaptcha = true;
                //        }
                //    }

                //    mensaje += System.Environment.NewLine + "cookies seguridad registro";
                //    HttpCookie cookieSeguridadRegistro = Request.Cookies.Get("SeguridadRegistro");
                //    mensaje += System.Environment.NewLine + "cookies seguridad registro ok";
                //    if (cookieSeguridadRegistro == null)
                //    {
                //        mostrarCaptcha = true;
                //    }
                //    mensaje += System.Environment.NewLine + "apellidos captcha";
                //    //textocaptcha
                //    if (apellidos.StartsWith(nombre) && !seguridadRegistro.ContainsKey("captchaImageText"))
                //    {
                //        mostrarCaptcha = true;
                //    }
                //    mensaje += System.Environment.NewLine + "captcha image text";
                //    if ((seguridadRegistro.ContainsKey("captchaImageText") && (textocaptcha == null || seguridadRegistro["captchaImageText"].ToString() != textocaptcha)))
                //    {
                //        mostrarCaptcha = true;
                //        mostrarErrorCaptcha = true;
                //    }
                //    mensaje += System.Environment.NewLine + "guid seguridad";
                //    if (!seguridadRegistro["guidSeguridad"].ToString().Equals(guidSeguridad) || !cookieSeguridadRegistro.Value.Equals(seguridadRegistro["guidSeguridad"].ToString() + "|" + seguridadRegistro["guidSeguridadCookie"].ToString()))
                //    {
                //        mostrarCaptcha = true;
                //    }
                //}

                //mensaje += System.Environment.NewLine + "num intentos";
                //if (!seguridadRegistro.ContainsKey("numIntentos"))
                //{
                //    seguridadRegistro.Add("numIntentos", 1);
                //}
                //else
                //{
                //    mensaje += System.Environment.NewLine + "num intentos else";
                //    seguridadRegistro["numIntentos"] = (int)seguridadRegistro["numIntentos"] + 1;
                //    mensaje += System.Environment.NewLine + "if num intentos >= 10";
                //    if ((int)seguridadRegistro["numIntentos"] >= 10)
                //    {
                //        mostrarCaptcha = true;
                //        mostrarErrorMaximoIntentos = true;
                //    }
                //}

                //if (mostrarCaptcha)
                //{
                //    mensaje += System.Environment.NewLine + "mostrar captcha";
                //    int numIntentos = (int)seguridadRegistro["numIntentos"];
                //    mensaje += System.Environment.NewLine + "establecer seguridad";
                //    EstablecerSeguridad(registro, Session, Response);
                //    mensaje += System.Environment.NewLine + "obtengo seguridad de sesión";
                //    seguridadRegistro = (Dictionary<string, object>)Session["SeguridadRegistro"];
                //    mensaje += System.Environment.NewLine + "obtenido";
                //    seguridadRegistro.Add("numIntentos", numIntentos);
                //    mensaje += System.Environment.NewLine + "num intentos";
                //    //Mostrar captcha
                //    Session["CaptchaImageText"] = UtilCaptchaImagen.GenerateRandomCode();
                //    mensaje += System.Environment.NewLine + "generaterandomcode";
                //    registro.ImageCaptcha = BaseURL + "/RegistroCaptchaImagen.aspx?v" + seguridadRegistro["numIntentos"];
                //    mensaje += System.Environment.NewLine + "image captcha";
                //    if (mostrarErrorCaptcha)
                //    {
                //        errores.Add("errorCaptcha");
                //    }
                //    if (mostrarErrorMaximoIntentos)
                //    {
                //        errores.Add("errorCaptchaNumIntentos");
                //    }

                //    mensaje += System.Environment.NewLine + "if captcha image text";
                //    if (seguridadRegistro.ContainsKey("captchaImageText"))
                //    {
                //        seguridadRegistro["captchaImageText"] = Session["CaptchaImageText"];
                //    }
                //    else
                //    {
                //        mensaje += System.Environment.NewLine + "else captcha image text";
                //        seguridadRegistro.Add("captchaImageText", Session["CaptchaImageText"]);
                //    }

                //    mensaje += System.Environment.NewLine + "seguridad registro a sesión";
                //    this.Session["SeguridadRegistro"] = seguridadRegistro;
                //}

                // TODO: Miguel, req8222 - ICE. Nuevas funciones web

                ////si el CMS está activo y el evento tiene componente del CMS cargamos su html
                //if (ParametrosGeneralesRow.CMSDisponible && InvitacionAEventoComunidad != null && !InvitacionAEventoComunidad.IsComponenteIDNull())
                //{
                //    Guid idComponente = InvitacionAEventoComunidad.ComponenteID;
                //    CMSCL cms2CL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService);
                //    string htmlComponente = cms2CL.ObtenerComponentePorIDEnProyecto(ProyectoSeleccionado.Clave, idComponente, UtilIdiomas.LanguageCode);
                //    cms2CL.Dispose();

                //    if (!string.IsNullOrEmpty(htmlComponente))
                //    {
                //        phComponenteEvento.Controls.Add(new LiteralControl(htmlComponente));
                //    }
                //    else
                //    {
                //        Controles_Componentes_ComponenteBase ctlComponente = (Controles_Componentes_ComponenteBase)LoadControl("~/Controles/Componentes/ComponenteBase.ascx");
                //        CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService);
                //        GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerComponentePorID(idComponente));
                //        cmsCN.Dispose();
                //        CMSComponente componente = gestorCMS.ListaComponentes[idComponente];
                //        ctlComponente.Componente = componente;
                //        ctlComponente.CargarControl();
                //        phComponenteEvento.Controls.Add(ctlComponente);
                //    }
                //}

                mensaje += System.Environment.NewLine + "if final";
                if (mostrarCaptcha || errores.Count > 0)
                {
                    mensaje += System.Environment.NewLine + "entro if";
                    registro.Name = nombre;
                    mensaje += System.Environment.NewLine + "nombre";
                    registro.LastName = apellidos;
                    mensaje += System.Environment.NewLine + "apellidos";
                    registro.Email = email;
                    mensaje += System.Environment.NewLine + "email";
                    registro.EmailTutor = emailTutor;
                    mensaje += System.Environment.NewLine + "emailTutor";
                    registro.BornDate = fechaNacimiento;
                    mensaje += System.Environment.NewLine + "fecha nacimiento";
                    registro.Gender = sexo;
                    mensaje += System.Environment.NewLine + "sexo";
                    registro.CountryID = paisID;
                    mensaje += System.Environment.NewLine + "paisid";
                    registro.RegionID = provinciaID;
                    mensaje += System.Environment.NewLine + "provincia";
                    registro.Region = provincia;
                    mensaje += System.Environment.NewLine + "localidad";
                    registro.Location = localidad;
                    mensaje += System.Environment.NewLine + "errores";
                    registro.Errors = errores;
                    mensaje += System.Environment.NewLine + "securityid";
                    //registro.SecurityID = (Guid)seguridadRegistro["guidSeguridad"];
                    mensaje += System.Environment.NewLine + "return true";
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje += System.Environment.NewLine + "Error: " + ex.Message;
                mensaje += System.Environment.NewLine + "Pila: " + ex.StackTrace;
                GuardarLogError(mensaje);
                registro.Errors = new List<string>();
                registro.Errors.Add("Ha habido un error al guardar los datos, inténtelo de nuevo por favor. ");
                return true;
            }
            return false;
        }

        private bool NecesitaTutor(string fechaNacimiento)
        {
            bool necesitaTutor = false;
            if (!string.IsNullOrEmpty(fechaNacimiento))
            {
                necesitaTutor = true;
                string[] fecha = fechaNacimiento.Split('/');

                int dia = int.Parse(fecha[0]);
                int mes = int.Parse(fecha[1]);
                int anio = int.Parse(fecha[2]);

                DateTime fechaNac = new DateTime(anio, mes, dia);

                int dias = DateTime.Now.Date.Subtract(fechaNac).Days;
                int años = dias / 365;

                if (años > 14)
                {
                    necesitaTutor = false;
                }
            }

            return necesitaTutor;
        }
        /// <summary>
        /// Verdad si existe el email
        /// </summary>
        /// <param name="pEmail">Email a validar</param>
        /// <returns></returns>
        private bool ExisteEmail(string pEmail)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool existe = false;
            if (HaySolicitudPrevia)
            {
                existe = personaCN.ExisteEmailExceptoEnSolicitud(pEmail, SolicitudPrevia.SolicitudID);
            }
            else
            {
                existe = personaCN.ExisteEmail(pEmail);
            }
            personaCN.Dispose();

            return existe;
        }

        /// <summary>
        /// Verdad si existe el usuario
        /// </summary>
        /// <returns></returns>
        private bool ExisteUsuario(string pLogin)
        {
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool existe = usuarioCN.ExisteUsuarioEnBD(pLogin);
            usuarioCN.Dispose();

            return existe;
        }

        /// <summary>
        /// Verdad si existe el usuario
        /// </summary>
        /// <returns></returns>
        private bool ExisteNombreCorto(string pNombreCorto)
        {
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool existe = usuarioCN.ExisteNombreCortoEnBD(pNombreCorto);
            usuarioCN.Dispose();

            return existe;
        }

        public void CargarEdadMinimaRegistro(AutenticationModel pRegistro)
        {
            pRegistro.MinAgeRegistre = 15;
            List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("EdadLimiteRegistroEcosistema")).ToList();
            //if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'EdadLimiteRegistroEcosistema'").Length > 0)
            if (busqueda.Count > 0)
            {
                //pRegistro.MinAgeRegistre = int.Parse(((ParametroAplicacionDS.ParametroAplicacionRow)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'EdadLimiteRegistroEcosistema'")[0]).Valor);
                pRegistro.MinAgeRegistre = int.Parse(busqueda.First().Valor);
            }

            if (EsInvitacionAClase)
            {
                pRegistro.MinAgeRegistre = 14;
            }

            if (EsInvitacionAClasePrimaria)
            {
                pRegistro.MinAgeRegistre = 7;
            }
        }

        private string ObtenerPais(string pValor)
        {
            Guid paisID = new Guid();

            if (Guid.TryParse(pValor, out paisID))
            {
                foreach (AD.EntityModel.Models.Pais.Pais fila in PaisesDW.ListaPais)
                {
                    if (fila.PaisID == paisID)
                    {
                        pValor = fila.Nombre;
                    }
                }
            }

            return pValor;
        }

        public int CargarClausulasRegistro(AutenticationModel pRegistro)
        {
            int numClausulas = 0;

            Dictionary<Guid, KeyValuePair<string, bool>> listaClausulas = new Dictionary<Guid, KeyValuePair<string, bool>>();
            Dictionary<string, KeyValuePair<string, string>> listaCondiciones = new Dictionary<string, KeyValuePair<string, string>>();

            foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in ClausulasRegistro.OrderBy(item => item.Tipo).ToList())
            {
                if (filaClausula.Tipo < (short)TipoClausulaAdicional.MensajeLoguado)
                {
                    string textoClausula = UtilCadenas.ObtenerTextoDeIdioma(filaClausula.Texto, UtilIdiomas.LanguageCode, null);

                    listaClausulas.Add(filaClausula.ClausulaID, new KeyValuePair<string, bool>(textoClausula, filaClausula.Tipo == (short)TipoClausulaAdicional.Opcional));

                    numClausulas++;
                }
                else if (filaClausula.Tipo == (short)TipoClausulaAdicional.CondicionesUso || filaClausula.Tipo == (short)TipoClausulaAdicional.ClausulasTexo)
                {
                    string titulo = "";
                    string id = "";

                    if (filaClausula.Tipo == (short)TipoClausulaAdicional.CondicionesUso)
                    {
                        if (filaClausula.ProyectoID == ProyectoAD.MetaProyecto)
                        {
                            id = "condicionesUsoGenericas";
                        }
                        else
                        {
                            id = "condicionesUsoCom";
                        }
                        titulo = UtilCadenas.ObtenerTextoDeIdioma((ClausulasRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.TituloCondicionesUso) && item.ProyectoID.Equals(filaClausula.ProyectoID)).FirstOrDefault()).Texto, UtilIdiomas.LanguageCode, null);
                    }
                    else
                    {
                        if (filaClausula.ProyectoID == ProyectoAD.MetaProyecto)
                        {
                            id = "clausulasTextoGenericas";
                        }
                        else
                        {
                            id = "clausulasTextoCom";
                        }
                        titulo = UtilCadenas.ObtenerTextoDeIdioma((ClausulasRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.TituloClausulasTexo) && item.ProyectoID.Equals(filaClausula.ProyectoID)).FirstOrDefault()).Texto, UtilIdiomas.LanguageCode, null);
                    }

                    string descripcion = UtilCadenas.ObtenerTextoDeIdioma(filaClausula.Texto, UtilIdiomas.LanguageCode, null);
                    listaCondiciones.Add(id, new KeyValuePair<string, string>(titulo, descripcion));
                }
            }

            pRegistro.Clauses = listaClausulas;
            pRegistro.Terms = listaCondiciones;

            return numClausulas;
        }

        private void ComprobarPeticionOrgID()
        {
            if (RequestParams("peticionOrgID") != null)
            {
                Guid peticionID = new Guid(RequestParams("peticionOrgID"));
                PeticionCN petCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionPeticiones gestorPeticiones = new GestionPeticiones(petCN.ObtenerPeticionInvitacionOrganizacionPorID(peticionID), mLoggingService, mEntityContext);
                petCN.Dispose();

                if (gestorPeticiones.ListaPeticiones.Count > 0)
                {
                    PeticionInvOrganizacion peticionOrg = (PeticionInvOrganizacion)gestorPeticiones.ListaPeticiones[peticionID];

                    Guid organizacionID = peticionOrg.FilaInvitacionOrganizacion.OrganizacionID;

                    OrganizacionCL orgCL = new OrganizacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mEsInvitacionAClase = orgCL.ComprobarOrganizacionEsClase(organizacionID);
                    mEsInvitacionAClasePrimaria = orgCL.ComprobarOrganizacionEsClasePrimaria(organizacionID);
                    mEsInvitacionAOrg = !orgCL.ComprobarOrganizacionEsClase(organizacionID);
                    orgCL.Dispose();
                }
            }
        }

        /// <summary>
        /// Comprueba si el dominio del email con el que el usuario ha hecho login en la red social está permitido en la aplicación
        /// </summary>
        /// <param name="pEmail">Email del usuario con el que ha hecho login en la red social</param>
        /// <returns>Bool que indica si ese dominio está permitido</returns>
        private bool ComprobarDominioEmailsPermitidos(string pEmail)
        {
            bool permitido = true;
            //ParametroAplicacionDS.ParametroAplicacionRow filaParametro = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.DominiosEmailLoginRedesSociales);
            AD.EntityModel.ParametroAplicacion filaParametro = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.DominiosEmailLoginRedesSociales));
            if (filaParametro != null && !string.IsNullOrEmpty(filaParametro.Valor))
            {
                string dominioEmailUsuario = pEmail.Substring(pEmail.IndexOf("@"));
                List<string> lista = new List<string>(filaParametro.Valor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                if (!lista.Contains(dominioEmailUsuario))
                {
                    permitido = false;
                }
            }

            return permitido;
        }

        #region Propiedades

        public PeticionInvComunidad InvitacionAComunidad
        {
            get
            {
                if (Invitacion != null && Invitacion is PeticionInvComunidad)
                {
                    return (PeticionInvComunidad)Invitacion;
                }
                return null;
            }
        }

        public PeticionInvOrganizacion InvitacionAOrganizacion
        {
            get
            {
                if (Invitacion != null && Invitacion is PeticionInvOrganizacion)
                {
                    return (PeticionInvOrganizacion)Invitacion;
                }
                return null;
            }
        }

        public ProyectoEvento InvitacionAEventoComunidad
        {
            get
            {
                if (mInvitacionAEventoComunidad == null)
                {
                    if (!string.IsNullOrEmpty(RequestParams("eventoComID")))
                    {
                        Guid eventoID = new Guid(RequestParams("eventoComID"));

                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        mInvitacionAEventoComunidad = proyCN.ObtenerEventoProyectoPorEventoID(eventoID).ListaProyectoEvento[0];
                        proyCN.Dispose();

                        if (mInvitacionAEventoComunidad == null || !mInvitacionAEventoComunidad.Activo)
                        {
                            Response.Redirect(mControladorBase.UrlsSemanticas.GetURLHacerseMiembroComunidad(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto));
                        }
                    }
                    else if (EsRegistroEnComunidad)
                    {
                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperProyecto proyDataWrapperProyecto = proyCN.ObtenerEventosProyectoPorProyectoID(ProyectoSeleccionado.Clave);
                        proyCN.Dispose();
                        List<ProyectoEvento> filasEvento = proyDataWrapperProyecto.ListaProyectoEvento.Where(proy => proy.Interno == true && proy.Activo == true).ToList();

                        //COMPROBAMOS SI EXISTE UN EVENTO INTERNO ACTIVO
                        if (filasEvento != null && filasEvento.Count > 0)
                        {
                            //sólo debería existir un evento interno
                            mInvitacionAEventoComunidad = filasEvento[0];
                        }
                    }
                }

                return mInvitacionAEventoComunidad;
            }
        }

        /// <summary>
        /// Obtiene la fila de solicitud de preactivación de nuevo usuario
        /// </summary>
        public SolicitudNuevoUsuario SolicitudPrevia
        {
            get
            {
                if (mSolicitudPreActivacionRegistro == null)
                {
                    if (mSolicitud != null)
                    {
                        if (mSolicitud.Solicitud.Estado == 0 || (!string.IsNullOrEmpty(RequestParams("activacionMyGnossID")) && Solicitud != null))
                        {
                            mSolicitudPreActivacionRegistro = mSolicitud;
                        }
                    }
                    else
                    {
                        if (Solicitud != null && Solicitud.Solicitud.Estado == 0 || (!string.IsNullOrEmpty(RequestParams("activacionMyGnossID")) && Solicitud != null))
                        {
                            mSolicitudPreActivacionRegistro = mSolicitud;
                        }
                    }

                }
                return mSolicitudPreActivacionRegistro;
            }
        }


        /// <summary>
        /// Obtiene la fila de solicitud de preactivación de nuevo usuario
        /// </summary>
        public SolicitudNuevoUsuario Solicitud
        {
            get
            {
                if (mSolicitud == null)
                {
                    Guid preactivacionID = Guid.Empty;

                    if (!Guid.TryParse(RequestParams("preActivacionID"), out preactivacionID))
                    {
                        Guid.TryParse(RequestParams("activacionMyGnossID"), out preactivacionID);
                    }

                    if (preactivacionID != Guid.Empty)
                    {
                        List<SolicitudNuevoUsuario> filasNuevoUsuario = SolicitudDW.ListaSolicitudNuevoUsuario.Where(item => item.SolicitudID.Equals(preactivacionID)).ToList();
                        if (filasNuevoUsuario != null && filasNuevoUsuario.Count > 0)
                        {
                            mSolicitud = filasNuevoUsuario[0];
                        }
                    }
                }

                return mSolicitud;
            }
        }

        /// <summary>
        /// Obtiene la fila de solicitud de preactivación de nuevo usuario
        /// </summary>
        public DataWrapperSolicitud SolicitudDW
        {
            get
            {
                if (mSolicitudDW == null)
                {
                    Guid preactivacionID = Guid.Empty;

                    if (!Guid.TryParse(RequestParams("preActivacionID"), out preactivacionID))
                    {
                        Guid.TryParse(RequestParams("activacionMyGnossID"), out preactivacionID);
                    }

                    if (preactivacionID != Guid.Empty)
                    {
                        SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        mSolicitudDW = solicitudCN.ObtenerSolicitudPorID(preactivacionID);

                        solicitudCN.Dispose();
                    }
                    else
                    {
                        mSolicitudDW = new DataWrapperSolicitud();
                    }
                }

                return mSolicitudDW;
            }
        }

        /// <summary>
        /// Indica si se va a terminar el registro de una cuenta preactivada
        /// </summary>
        public bool HaySolicitudPrevia
        {
            get
            {
                return EsRegistroPreActivado || EstaVerificandoEmail;
            }
        }

        /// <summary>
        /// Indica si se va a terminar el registro de una cuenta preactivada
        /// </summary>
        public bool EsRegistroPreActivado
        {
            get
            {
                return (!string.IsNullOrEmpty(RequestParams("preActivacionID")) && SolicitudPrevia != null);
            }
        }

        /// <summary>
        /// Indica si se va a verificar el correo tras haberse registrado
        /// </summary>
        public bool EstaVerificandoEmail
        {
            get
            {
                return (!string.IsNullOrEmpty(RequestParams("activacionMyGnossID")) && (SolicitudPrevia != null || Solicitud != null));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EsInvitacionAEventoDeComunidad
        {
            get
            {
                return InvitacionAEventoComunidad != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EsRegistroEnComunidad
        {
            get
            {
                return !string.IsNullOrEmpty(RequestParams("esregistro"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EsTutorRegistro
        {
            get
            {
                return !string.IsNullOrEmpty(RequestParams("tutor"));
            }
        }

        public bool ValidadoClausulasTutor
        {
            get
            {
                return !string.IsNullOrEmpty(RequestParams("clausulas"));
            }
        }


        private void CargarInvitacion()
        {
            if (!string.IsNullOrEmpty(RequestParams("peticionComID")))
            {
                InvitacionID = new Guid(RequestParams("peticionComID"));
            }
            else if (!string.IsNullOrEmpty(RequestParams("peticionOrgID")))
            {
                InvitacionID = new Guid(RequestParams("peticionOrgID"));
            }

            if (InvitacionID.HasValue)
            {
                PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionPeticiones gestionPeticion = new GestionPeticiones(peticionCN.ObtenerPeticionPorID(InvitacionID.Value), mLoggingService, mEntityContext);
                Invitacion = gestionPeticion.ListaPeticiones[InvitacionID.Value];
                peticionCN.Dispose();
            }
        }

        private List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> ClausulasRegistro
        {
            get
            {
                if (mClausulasRegistro == null)
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    DataWrapperUsuario usuarioDW = proyCL.ObtenerClausulasRegitroProyecto(ProyectoSeleccionado.Clave);

                    if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && mControladorBase.UsuarioActual.EsUsuarioInvitado)
                    {
                        foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in usuarioDW.ListaClausulaRegistro.ToList())
                        {
                            filaClausula.Orden += 100;
                        }
                        usuarioDW.Merge(proyCL.ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto));
                    }
                    mClausulasRegistro = usuarioDW.ListaClausulaRegistro.ToList();
                }
                return mClausulasRegistro;
            }
        }

        /// <summary>
        /// Datos extra del proyecto
        /// </summary>
        public DataWrapperProyecto DatosExtraProyectoDataWrapperProyecto
        {
            get
            {
                if (mDatosExtraProyectoDataWrapperProyecto == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mDatosExtraProyectoDataWrapperProyecto = proyectoCN.ObtenerDatosExtraProyectoPorID(ProyectoSeleccionado.Clave);
                    proyectoCN.Dispose();
                }
                return mDatosExtraProyectoDataWrapperProyecto;
            }
        }


        public string UrlOrigen
        {
            get
            {
                if (InvitacionAEventoComunidad != null && !InvitacionAEventoComunidad.Interno && string.IsNullOrEmpty(RequestParams("referer")))
                {
                    return mControladorBase.UrlsSemanticas.GetURLAceptarInvitacionEventoEnProyecto(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, InvitacionAEventoComunidad.EventoID);
                }
                else
                {
                    if (string.IsNullOrEmpty(RequestParams("urlOrigen")))
                    {
                        string urlRedirectHome = ObtenerUrlHomeConectado();

                        string urlComunidad = Comunidad.Url;

                        if (RequestParams("redirect") != null)
                        {
                            string urlRedi = RequestParams("redirect");
                            string trozoCom = UtilIdiomas.GetText("URLSEM", "COMUNIDAD") + "/" + ProyectoSeleccionado.NombreCorto;

                            if (urlRedi.Contains(trozoCom))
                            {
                                urlRedi = urlRedi.Substring(urlRedi.IndexOf(trozoCom) + trozoCom.Length);
                            }

                            urlComunidad += urlRedi;
                        }
                        else if (!string.IsNullOrEmpty(urlRedirectHome))
                        {
                            urlComunidad = urlRedirectHome;
                        }
                        return urlComunidad;
                    }
                    return RequestParams("urlOrigen");
                }
            }
        }


        private bool EsInvitacionAClase
        {
            get
            {
                if (mEsInvitacionAClase == null)
                {
                    mEsInvitacionAClase = false;

                    ComprobarPeticionOrgID();
                }
                return mEsInvitacionAClase.Value;
            }
        }

        private bool EsInvitacionAClasePrimaria
        {
            get
            {
                if (mEsInvitacionAClasePrimaria == null)
                {
                    mEsInvitacionAClasePrimaria = false;

                    ComprobarPeticionOrgID();
                }
                return mEsInvitacionAClasePrimaria.Value;
            }
        }

        /// <summary>
        /// Si el proyecto es público o privado pero con registro abierto, las páginas con registro son accesibles para el usuario desconectado
        /// </summary>
        public override bool PaginaVisibleEnPrivada
        {
            get
            {
                ComprobarPeticionOrgID();
                return ProyectoSeleccionado.EsPublico || RegistroAbiertoEnComunidadPrivada || HaySolicitudPrevia || (mEsInvitacionAOrg.HasValue && mEsInvitacionAOrg.Value) || !string.IsNullOrEmpty(RequestParams("mostrarLogin"));
            }
        }

        private bool RegistroAbiertoEnComunidadPrivada
        {
            get
            {
                return (ParametroProyecto.ContainsKey(ParametroAD.RegistroAbierto) && ParametroProyecto[ParametroAD.RegistroAbierto].Equals("1"));
            }
        }
        public bool SaltarPasosRegistro
        {
            get
            {
                if (!mSaltarPasosRegistro.HasValue)
                {
                    mSaltarPasosRegistro = false;
                    //skp-stps, acrónimo de skip-steps
                    string cookieValue = Request.Cookies["skp-stps"];
                    if (cookieValue != null && cookieValue.Equals("true"))
                    {
                        mSaltarPasosRegistro = true;
                        //cookie.Domain = mControladorBase.DominoAplicacion;
                        mControladorBase.ExpirarCookie("skp-stps");
                    }
                    else if (!string.IsNullOrEmpty(Request.Headers["skp-stps"]) && Request.Headers["skp-stps"].Equals("true"))
                    {
                        mSaltarPasosRegistro = true;
                    }
                }

                return mSaltarPasosRegistro.Value;
            }
        }

        public string UrlRedireccionTrasRegistro
        {
            get
            {
                if (mUrlRedireccionTrasRegistro == null)
                {
                    string cookieValue = Request.Cookies["reg-redirect"];
                    //skp-stps, acrónimo de skip-steps
                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        mUrlRedireccionTrasRegistro = cookieValue;
                        //cookie.Domain = mControladorBase.DominoAplicacion;
                        mControladorBase.ExpirarCookie("reg-redirect");
                    }
                }
                return mUrlRedireccionTrasRegistro;
            }
        }
        #endregion

    }
}
