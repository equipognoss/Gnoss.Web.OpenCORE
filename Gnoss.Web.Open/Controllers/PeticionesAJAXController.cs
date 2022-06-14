using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Web.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WebPush;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class PeticionesAJAXController : ControllerBaseWeb
    {
        public PeticionesAJAXController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ObtenerCookieSesionUsuarioActiva();
        }

        /// <summary>
        /// Carga el numero de mensajes comentarios, invitaciones y suscripciones nuevos para el usuario.
        /// </summary>
        /// <param name="pPerfilUsuarioID">Identificador del perfil del usuario</param>
        /// <param name="pPerfilOrganizacionID">Identificador del perfil de la organizacion, si la tiene</param>
        /// <param name="pBandejaDeOrganizacion">Booleano que indica si la bandeja es de organizacion</param>
        /// <param name="pOtrasIdent">Cadena con otras identidades</param>
        /// <returns>Devuelve el numero de mensajes, comentarios, invitaciones y suscripciones separados por "|"</returns>   
        public ActionResult CargarNumElementosNuevos(Guid pPerfilUsuarioID, Guid? pPerfilOrganizacionID, string pBandejaDeOrganizacion, string pOtrasIdent)
        {
            string urlRedirect = "";
            try
            {
                if (Session.Get("Usuario") != null)
                {
                    Guid UsuarioID = Session.Get<GnossIdentity>("Usuario").UsuarioID;

                    if (!Session.Get<GnossIdentity>("Usuario").PersonaID.Equals(UsuarioAD.Invitado))
                    {
                        //Comprobar la cookie para ver si este usuario esta logeado.
                        AD.EntityModel.ParametroAplicacion busquedaLoginUnicoPorUsuario = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.LoginUnicoPorUsuario));
                        bool loginUnicoPorUsuario = busquedaLoginUnicoPorUsuario != null && busquedaLoginUnicoPorUsuario.Valor.Equals("1");
                        AD.EntityModel.ParametroAplicacion busquedaDesconexionUsuarios = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.DesconexionUsuarios));
                        bool desconexionUsuarios = busquedaDesconexionUsuarios != null && busquedaDesconexionUsuarios.Valor.Equals("1");
                        AD.EntityModel.ParametroAplicacion filaExcluidos = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos));
                        List<string> sessionesPermitidasUsuario = null;

                        if (loginUnicoPorUsuario && desconexionUsuarios && (filaExcluidos == null || !filaExcluidos.Valor.ToLower().Contains(UsuarioID.ToString())))
                        {
                            // La plataforma está configurada para que los usuarios sólo puedan estar logueados en un sólo navegador al mismo tiempo
                            sessionesPermitidasUsuario = mGnossCache.ObtenerDeCache($"{ControladorBase.SESION_UNICA_POR_USUARIO}_List_{UsuarioID.ToString()}", true) as List<string>;

                            //Si no tiene nada en la cache, implica que no se ha hecho login o que le han desconectado.
                            if (sessionesPermitidasUsuario == null)
                            {
                                //Desconectar
                                urlRedirect = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoConexion)}/{UtilIdiomas.GetText("URLSEM", "DESCONECTAR")}?redirect={UrlEncode($"{UtilIdiomas.GetText("URLSEM", "LOGIN")}?desconectado=true")}";
                                // El sessionID actual es distinto al sessionID con el que inició sesión previamente. Le redirijo
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(urlRedirect))
                    {
                        return Content(urlRedirect);
                    }

                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            if (pBandejaDeOrganizacion == null)
            {
                pBandejaDeOrganizacion = "";
            }

            int numMensajes = 0;
            int numMensajesOrg = 0;
            int numInvitaciones = 0;
            int numInvitacionesOrg = 0;
            int numSuscripciones = 0;
            int numComentarios = 0;

            int numMensajesSinLeer = 0;
            int numMensajesSinLeerOrg = 0;
            int numInvitacionesSinLeer = 0;
            int numInvitacionesSinLeerOrg = 0;
            int numSuscripcionesSinLeer = 0;
            int numComentariosSinLeer = 0;

            string numElemNuevosOtrasIdent = "";

            bool bandejaOrg = bool.Parse(pBandejaDeOrganizacion);

            try
            {
                Guid perfilBuscarID = pPerfilUsuarioID;

                List<Guid> identidades = new List<Guid>();
                identidades.Add(perfilBuscarID);

                if (bandejaOrg)
                {
                    identidades.Add(pPerfilOrganizacionID.Value);
                }

                Dictionary<Guid, Guid> dicPerfiles_Organizaciones = new Dictionary<Guid, Guid>();
                string[] perfilesOtrasIdent = pOtrasIdent != null ? pOtrasIdent.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();

                foreach (string idPerfil in perfilesOtrasIdent)
                {
                    if (idPerfil.Contains("_"))
                    {
                        //Perfil personal corporativo (Miembro de la organización)
                        identidades.Add(new Guid(idPerfil.Substring(0, idPerfil.IndexOf("_"))));

                        //Perfil corporativo (organización)
                        identidades.Add(new Guid(idPerfil.Substring(idPerfil.IndexOf("_") + 1)));

                        dicPerfiles_Organizaciones.Add(new Guid(idPerfil.Substring(0, idPerfil.IndexOf("_"))), new Guid(idPerfil.Substring(idPerfil.IndexOf("_") + 1)));
                    }
                    else
                    {
                        identidades.Add(new Guid(idPerfil));
                    }
                }

                //Cargamos todos los contadores de todos los perfiles a la vez
                LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<AD.EntityModel.Models.IdentidadDS.ContadorPerfil> listaContadorPerfil = liveCN.ObtenerContadoresPerfiles(identidades);
                AD.EntityModel.Models.IdentidadDS.ContadorPerfil contadorPerfilBuscar = listaContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(perfilBuscarID));
                if (contadorPerfilBuscar != null)
                {
                    numMensajes = contadorPerfilBuscar.NumNuevosMensajes;
                    numComentarios = contadorPerfilBuscar.NuevosComentarios;
                    numInvitaciones = contadorPerfilBuscar.NuevasInvitaciones;
                    numSuscripciones = contadorPerfilBuscar.NuevasSuscripciones;

                    numComentariosSinLeer = contadorPerfilBuscar.NumComentariosSinLeer;
                    numMensajesSinLeer = contadorPerfilBuscar.NumMensajesSinLeer;
                    numInvitacionesSinLeer = contadorPerfilBuscar.NumInvitacionesSinLeer;
                    numSuscripcionesSinLeer = contadorPerfilBuscar.NumSuscripcionesSinLeer;
                    if (pPerfilOrganizacionID.HasValue) 
                    {
                        AD.EntityModel.Models.IdentidadDS.ContadorPerfil contadorPerfilBuscarOrganizacion = listaContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilOrganizacionID.Value));
                        if (bandejaOrg && contadorPerfilBuscarOrganizacion != null)
                        {
                            numMensajesOrg = contadorPerfilBuscarOrganizacion.NumNuevosMensajes;
                            numInvitacionesOrg = contadorPerfilBuscarOrganizacion.NuevasInvitaciones;

                            numMensajesSinLeerOrg = contadorPerfilBuscarOrganizacion.NumMensajesSinLeer;
                            numInvitacionesSinLeerOrg = contadorPerfilBuscarOrganizacion.NumInvitacionesSinLeer;
                        }
                    }
                }

                foreach (AD.EntityModel.Models.IdentidadDS.ContadorPerfil contadorPerfil in listaContadorPerfil.Where(item => !item.PerfilID.Equals(perfilBuscarID) && !item.PerfilID.Equals(pPerfilOrganizacionID.Value)).ToList())
                {
                    int numNovTotal = 0;

                    numNovTotal += contadorPerfil.NumNuevosMensajes + contadorPerfil.NuevosComentarios + contadorPerfil.NuevasInvitaciones + contadorPerfil.NuevasSuscripciones;

                    //Si es una organización, debemos sumar al número total de novedades del perfil personal corporativo, el de la organización.
                    if (dicPerfiles_Organizaciones.ContainsKey(contadorPerfil.PerfilID))
                    {
                        AD.EntityModel.Models.IdentidadDS.ContadorPerfil contadorPerfilOrg = listaContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(contadorPerfil.PerfilID));
                        if (contadorPerfilOrg != null)
                        {
                            numNovTotal += contadorPerfilOrg.NumNuevosMensajes + contadorPerfilOrg.NuevasInvitaciones;
                        }
                    }

                    //Si es una organización no debemos pintar el número de elementos de ese perfil.
                    if (!dicPerfiles_Organizaciones.ContainsValue(contadorPerfil.PerfilID))
                    {
                        numElemNuevosOtrasIdent += contadorPerfil.PerfilID + ":" + numNovTotal + "&";
                    }
                }

                liveCN.Dispose();

                Dictionary<string, string> cookieRewrite = mControladorBase.CookieRewrite;
                if (cookieRewrite != null)
                {
                    Response.Cookies.Append("rewrite" + mControladorBase.DominoAplicacion, UtilCookies.ToLegacyCookieString(cookieRewrite, mEntityContext), new CookieOptions { Expires = DateTime.Now.AddDays(1), Domain = mControladorBase.DominoAplicacion });
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }


            string elementos = numMensajes + "|" + numInvitaciones + "|" + numSuscripciones + "|" + numComentarios + "|" + numMensajesSinLeer + "|" + numInvitacionesSinLeer + "|" + numSuscripcionesSinLeer + "|" + numComentariosSinLeer + "|" + numMensajesOrg + "|" + numMensajesSinLeerOrg + "|" + numInvitacionesOrg + "|" + numInvitacionesSinLeerOrg + "|" + numElemNuevosOtrasIdent;

            bool hayMensajesNuevos = ComprobarSiHayNotificacionesNuevas(elementos);

            if (hayMensajesNuevos)
            {
                string subject = mConfigService.ObtenerVapidSubject();
                string publicKey = mConfigService.ObtenerVapidPublicKey();
                string privateKey = mConfigService.ObtenerVapidPrivateKey();
                VapidDetails vapidDetails = new VapidDetails(subject, publicKey, privateKey);
                PushSubscription subscription = new PushSubscription(HttpContext.Request.Cookies["endpoint"], HttpContext.Request.Cookies["p256dh"], HttpContext.Request.Cookies["auth"]);
                if (subscription == null)
                {

                }
                WebPushClient webPushClient = new WebPushClient();
                try
                {
                    webPushClient.SendNotification(subscription, "GNOSS - Mensaje nuevo", vapidDetails);
                }
                catch (Exception exception)
                {
                    HttpContext.Response.Cookies.Append("endpoint", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
                    HttpContext.Response.Cookies.Append("p256dh", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
                    HttpContext.Response.Cookies.Append("auth", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
                }
            }
            HttpContext.Response.Cookies.Append("elementosAntes", elementos, new CookieOptions { Expires = DateTime.Now.AddMinutes(30) });

            return Content(elementos);
        }


        public void SuscribirseNotificacionesPush(string pEndpoint, string pP256dh, string pAuth)
        {
            CookieOptions opts = new CookieOptions();
            opts.Expires = DateTime.MaxValue;
            HttpContext.Response.Cookies.Append("endpoint", pEndpoint, opts);
            HttpContext.Response.Cookies.Append("p256dh", pP256dh, opts);
            HttpContext.Response.Cookies.Append("auth", pAuth, opts);
        }

        public void DesuscribirseNotificacionesPush()
        {
            HttpContext.Response.Cookies.Append("endpoint", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            HttpContext.Response.Cookies.Append("p256dh", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            HttpContext.Response.Cookies.Append("auth", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
        }

        public bool ComprobarSiHayNotificacionesNuevas(string pElementos)
        {
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["endpoint"]) || string.IsNullOrEmpty(HttpContext.Request.Cookies["p256dh"]) || string.IsNullOrEmpty(HttpContext.Request.Cookies["auth"]))
            {
                return false;
            }
            string elementosAntiguos = HttpContext.Request.Cookies["elementosAntes"];
            if (string.IsNullOrEmpty(elementosAntiguos) || string.IsNullOrEmpty(pElementos))
            {
                return false;
            }
            int notificacionesAntes = int.Parse(elementosAntiguos.Substring(0, 1));
            int notificacionesAhora = int.Parse(pElementos.Substring(0, 1));

            return (notificacionesAhora > notificacionesAntes);
        }


        public ActionResult ObtenerDatosUsuarioActual()
        {
            try
            {
                if (Session.Get("Usuario") != null)
                {
                    Guid personaID = Session.Get<GnossIdentity>("Usuario").PersonaID;

                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestionPersonas gestorPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorID(personaID), mLoggingService, mEntityContext);
                    personaCN.Dispose();

                    Persona persona = gestorPersonas.ListaPersonas[personaID];

                    JsonDatosUsuario datosUsuario = new JsonDatosUsuario();
                    datosUsuario.UsuarioID = persona.UsuarioID.ToString();
                    datosUsuario.Nombre = persona.Nombre;
                    datosUsuario.Apellidos = persona.Apellidos;
                    datosUsuario.Email = persona.FilaPersona.Email;
                    datosUsuario.Ciudad = persona.Localidad;

                    if (!Session.Get<GnossIdentity>("Usuario").PersonaID.Equals(UsuarioAD.Invitado))
                    {
                        PaisCL paisCL = new PaisCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperPais paisDW = paisCL.ObtenerPaisesProvincias();
                        paisCL.Dispose();
                        foreach (AD.EntityModel.Models.Pais.Pais fila in paisDW.ListaPais)
                        {
                            if (fila.PaisID == persona.PaisID)
                            {
                                datosUsuario.Pais = fila.Nombre;
                            }
                        }
                    }

                    return Content(JsonConvert.SerializeObject(datosUsuario));
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            return Content(null);
        }

        public class JsonDatosUsuario
        {
            public string UsuarioID { get; set; }
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Email { get; set; }
            public string Ciudad { get; set; }
            public string Pais { get; set; }
        }
    }

}
