using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Models;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class CrearCookieController : ControllerBaseWeb
    {
        public CrearCookieController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;
        }

        // GET: CrearCookie
        public ActionResult Index(CrearCookieLoginModel crearCookieLoginModel)
        {
            string redirect = "";

            bool redirigirAEntrando = false;
            string redirectValido = ComprobarRedirectValido(crearCookieLoginModel.redirect);
            if (!crearCookieLoginModel.redirect.Equals(redirectValido))
            {
                crearCookieLoginModel.redirect = ProyectoSeleccionado.UrlPropia(IdiomaUsuario);
            }
            try
            {
                //Cabeceras para poder recibir cookies de terceros
                HttpContext.Response.Headers.Add("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");

                string dominio = "*";

                if (Request.Headers.ContainsKey("Referer"))
                {
                    dominio = UtilDominios.ObtenerDominioUrl(Request.Headers["Referer"], true);
                }

                Response.Headers.Add("Access-Control-Allow-Origin", dominio);
                Response.Headers.Add("Access-Control-Allow-Credentials", "true");


                if (!string.IsNullOrEmpty(crearCookieLoginModel.redirect))
                {
                    redirect = HttpUtility.UrlDecode(crearCookieLoginModel.redirect);

                    if (redirect.EndsWith("/" + UtilIdiomas.GetText("URLSEM", "DESCONECTAR")) || redirect.Contains("/" + UtilIdiomas.GetText("URLSEM", "DESCONECTAR") + "?") || redirect.Contains("/" + UtilIdiomas.GetText("URLSEM", "DESCONECTAR") + "/"))
                    {
                        redirect = redirect.Substring(0, redirect.IndexOf("/" + UtilIdiomas.GetText("URLSEM", "DESCONECTAR")));
                    }
                }

                if (!string.IsNullOrEmpty(crearCookieLoginModel.token))
                {
                    string tokenRecibido = crearCookieLoginModel.token;

                    bool tokenValido = false;
                    if (Session.Get("tokenCookie") != null && tokenRecibido.Equals(Session.Get<string>("tokenCookie")))
                    {
                        tokenValido = true;
                    }
                    else
                    {
                        Guid tokenRecibidoID = Guid.Empty;
                        if (Guid.TryParse(tokenRecibido, out tokenRecibidoID))
                        {
                            SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            string login = solicitudCN.ObtenerLoginAPILoginDeToken(new Guid(tokenRecibido), true);
                            solicitudCN.Dispose();

                            if (!string.IsNullOrEmpty(login))
                            {
                                tokenValido = true;
                            }
                        }
                    }

                    if (tokenValido)
                    {
                        string redirigirLogin = LoguearUsuario(crearCookieLoginModel);

                        Session.Remove("tokenCookie");

                        if (!string.IsNullOrEmpty(redirigirLogin))
                        {
                            return Redirect(redirigirLogin);
                        }

                        mControladorBase.CrearCookieLogueado();

                        if (crearCookieLoginModel.entrando.HasValue && crearCookieLoginModel.entrando.Value && (Request.Scheme.Equals("https")))
                        {
                            // Hay que redireccionar a entrando, ya que estamos en un sitio seguro en el que 
                            // el usuario acaba de loguearse y el usuario había entrado en dominios que no funcionan 
                            // con https, hay que eliminar la cookie en ellos desde una url sin https
                            redirigirAEntrando = true;
                        }
                    }
                }

                Session.Set("cookieSolicitada", true);

                if (crearCookieLoginModel.eliminarCookie.HasValue && crearCookieLoginModel.eliminarCookie.Value.Equals(false))
                {
                    redirigirAEntrando = false;
                    HttpContext.Session.Set("EnvioCookie", DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex.Message + "\r\n\r\nPila: " + ex.StackTrace);

                if (string.IsNullOrEmpty(redirect))
                {
                    if (!ProyectoConexionID.Value.Equals(ProyectoAD.MetaProyecto))
                    {
                        redirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoConexion);
                    }
                    else
                    {
                        redirect = mConfigService.ObtenerUrlBase();
                    }
                }
            }

            string urlRedirect = redirect;

            if (redirigirAEntrando)
            {
                urlRedirect = mControladorBase.DominoAplicacionConHTTP.Replace("https://", "http://") + "/loading?redirect=" + System.Net.WebUtility.UrlEncode(redirect);
            }

            return Redirect(urlRedirect);
        }

        private string LoguearUsuario(CrearCookieLoginModel crearCookieLoginModel)
        {
            Guid usuID = Guid.Empty;

            if (crearCookieLoginModel.usuarioID.HasValue && (!crearCookieLoginModel.usuarioID.Value.Equals(UsuarioAD.Invitado.ToString())))
            {
                //Obtengo el usuarioID a partir del parametro GET
                string usuarioID = crearCookieLoginModel.usuarioID.Value.ToString();
                string idioma = crearCookieLoginModel.idioma;
                string login = "";

                usuID = new Guid(usuarioID);


                //Compruebo que si el usuario estaba logueado y ahora viene con la identidad invitado porque se ha perdido la sesión, no de error
                if (usuID.Equals(UsuarioAD.Invitado))
                {
                    return null;
                }

                //Login del usuario
                if (!string.IsNullOrEmpty(crearCookieLoginModel.loginUsuario))
                {
                    login = crearCookieLoginModel.loginUsuario;
                }

                string[] salvar = { "ClausulasRegistro", "paginaOriginal" };
                mControladorBase.LimpiarSesion(salvar);


                AD.EntityModel.ParametroAplicacion busquedaLoginUnicoPorUsuario = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.LoginUnicoPorUsuario));
                //bool loginUnicoPorUsuario = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.LoginUnicoPorUsuario) != null && ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.LoginUnicoPorUsuario).Valor.Equals("1");
                bool loginUnicoPorUsuario = busquedaLoginUnicoPorUsuario != null && busquedaLoginUnicoPorUsuario.Valor.Equals("1");
                //ParametroAplicacionDS.ParametroAplicacionRow filaExcluidos = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos);
                AD.EntityModel.ParametroAplicacion filaExcluidos = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos));


                List<string> sessionesPermitidasUsuario = null;

                if (loginUnicoPorUsuario && (filaExcluidos == null || !filaExcluidos.Valor.ToLower().Contains(usuID.ToString())))
                {
                    // La plataforma está configurada para que los usuarios sólo puedan estar logueados en un sólo navegador al mismo tiempo
                    sessionesPermitidasUsuario = mGnossCache.ObtenerDeCache($"{ControladorBase.SESION_UNICA_POR_USUARIO}_List_{usuID}", true) as List<string>;

                    //
                    //Borrar la cache esta GnossCache.ObtenerDeCache($"{mControladorBase.SESION_UNICA_POR_USUARIO}_List_{usuID}", true)
                    string sessionIdAntigua = mGnossCache.ObtenerDeCache($"{ControladorBase.SESION_UNICA_POR_USUARIO}_List_{usuID}", true) as string;
                    if (!string.IsNullOrEmpty(sessionIdAntigua))
                    {
                        if (sessionesPermitidasUsuario == null)
                        {
                            sessionesPermitidasUsuario = new List<string>();
                        }
                        if (!sessionesPermitidasUsuario.Contains(sessionIdAntigua))
                        {
                            sessionesPermitidasUsuario.Add(sessionIdAntigua);
                        }
                    }

                    if (sessionesPermitidasUsuario != null && (string.IsNullOrEmpty(Session.Id) || !sessionesPermitidasUsuario.Contains(Session.Id)))
                    {
                        //Generar un token para mandar
                        Guid token = Guid.NewGuid();
                        Session.Set(token.ToString(), usuID);
                        GuardarLogError($"Un usuario está intentando loguearse dos veces. Usuario {usuID}, sessión {Session.Id}, Lista de sesiones permitidas: {string.Join(",", sessionesPermitidasUsuario)}");
                        // El sessionID actual es distinto al sessionID con el que inició sesión previamente. No le permito loguearse

                        //Metodo en el logout.
                        return $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, NombreCortoProyectoConexion)}/{UtilIdiomas.GetText("URLSEM", "LOGIN")}?logintwice=true&token={token}";
                    }
                }

                GnossIdentity identity = mControladorBase.ValidarUsuario(login);
                if (identity != null)
                {
                    Session.Set("Usuario", identity);

                    if (loginUnicoPorUsuario)
                    {
                        if (sessionesPermitidasUsuario == null)
                        {
                            sessionesPermitidasUsuario = new List<string>();
                        }
                        sessionesPermitidasUsuario.Add(Session.Id);

                        mGnossCache.AgregarObjetoCache($"{ControladorBase.SESION_UNICA_POR_USUARIO}_List_{usuID}", sessionesPermitidasUsuario/*TODO Javier sacar del config services, Session.Timeout * 60*/);
                    }
                }
                Session.Set("CookieCreada", true);
            }
            return null;
        }

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

    }
}