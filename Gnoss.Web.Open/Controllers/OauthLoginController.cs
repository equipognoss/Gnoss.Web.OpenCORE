using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.Messages;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.UtilOAuth;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class OauthLoginController : Controller
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ControladorBase mControladorBase;
        private EntityContextOauth mEntityContextOauth;
        protected Proyecto mProyecto;
        protected string mIdiomaUsuario;
        protected IHttpContextAccessor mHttpContextAccessor;


        public OauthLoginController(IHttpContextAccessor httpContextAccessor, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, EntityContextOauth entityContextOauth)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAD;
            mEntityContextOauth = entityContextOauth;
            mHttpContextAccessor = httpContextAccessor;

            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null);
        }

        public Proyecto ProyectoSeleccionado
        {
            get
            {
                if (mProyecto == null)
                {
                    string nombreCortoProyecto = null;
                    Guid proyectoID = Guid.Empty;

                    if (RequestParams("proyectoID") != null)
                    {
                        proyectoID = new Guid(RequestParams("proyectoID"));
                    }
                    else if (RequestParams("nombreProy") != null)
                    {
                        nombreCortoProyecto = RequestParams("nombreProy");
                    }
                    else if (RequestParams("NombreCortoComunidad") != null)
                    {
                        nombreCortoProyecto = RequestParams("NombreCortoComunidad");
                    }
                    else if (RequestParams("proy") != null)
                    {
                        proyectoID = new Guid(RequestParams("proy"));
                    }
                    else if (RequestParams("pProyectoID") != null)
                    {
                        Guid.TryParse(RequestParams("pProyectoID").Replace("\"", ""), out proyectoID);
                    }
                    else
                    {
                        proyectoID = ProyectoAD.MetaProyecto;
                    }

                    if (!string.IsNullOrEmpty(RequestParams("ecosistema")) && RequestParams("ecosistema").Equals("true"))
                    {
                        proyectoID = ProyectoAD.MetaProyecto;
                        nombreCortoProyecto = "mygnoss";
                    }

                    if (mProyecto == null || (nombreCortoProyecto != mProyecto.NombreCorto && mProyecto.Clave != proyectoID))
                    {
                        ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                        if (!string.IsNullOrEmpty(nombreCortoProyecto))
                        {
                            proyectoID = proyectoCL.ObtenerProyectoIDPorNombreCorto(nombreCortoProyecto);
                        }

                        GestionProyecto gestorProyecto = new GestionProyecto(proyectoCL.ObtenerProyectoPorID(proyectoID), mLoggingService, mEntityContext);

                        if (gestorProyecto.ListaProyectos.Count > 0 && gestorProyecto.ListaProyectos.ContainsKey(proyectoID))
                        {
                            mProyecto = gestorProyecto.ListaProyectos[proyectoID];
                        }

                        if (mProyecto == null)
                        {
                            return null;
                        }

                    }
                    return mProyecto;
                }
                return mProyecto;
            }
            set
            {
                mProyecto = value;
            }
        }

        public string RequestParams(string pParametro)
        {
            string valorParametro = null;

            try
            {
                if (mHttpContextAccessor.HttpContext.Request.Query.ContainsKey(pParametro))
                {
                    valorParametro = mHttpContextAccessor.HttpContext.Request.Query[pParametro];
                }
                else if (mHttpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(pParametro))
                {
                    valorParametro = mHttpContextAccessor.HttpContext.Request.RouteValues[pParametro] as string;

                }
                else if (mHttpContextAccessor.HttpContext.Request.HasFormContentType && mHttpContextAccessor.HttpContext.Request.Form != null && mHttpContextAccessor.HttpContext.Request.Form.ContainsKey(pParametro))
                {
                    valorParametro = mHttpContextAccessor.HttpContext.Request.Form[pParametro];
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex.Message);
            }

            return valorParametro;
        }

        /// <summary>
        /// Obtiene el idioma del usuario
        /// </summary>
        public string IdiomaUsuario
        {
            get
            {
                if (mIdiomaUsuario == null)
                {
                    mIdiomaUsuario = RequestParams("lang");
                }
                return mIdiomaUsuario;
            }
            set
            {
                mIdiomaUsuario = value;
            }
        }

        //
        // GET: /OauthLogin/

        private ServiceProvider mServiceProvider = null;

        public OauthLoginController()
        {
            if (mServiceProvider == null)
            {
                mServiceProvider = new ServiceProvider(Consumidor.AutoDescripcion("/access_token", "/request_token", "/authorize"), TokenManager, new CustomOAuthMessageFactory(TokenManager));
            }
        }

        [HttpPost]
        public ActionResult RequestToken()
        {
            try
            {
                IProtocolMessage request = mServiceProvider.ReadRequest();

                RequestScopedTokenMessage requestToken;

                if ((requestToken = request as RequestScopedTokenMessage) != null)
                {
                    mLoggingService.AgregarEntrada("Peticion request token");

                    //Ha llegado una petición de request token, se genera un request token y se le envía 
                    var response = mServiceProvider.PrepareUnauthorizedTokenMessage(requestToken);
                    mServiceProvider.Channel.Send(response);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return new UnauthorizedResult();
        }

        public ActionResult Authorize()
        {
            try
            {
                IProtocolMessage request = mServiceProvider.ReadRequest();
                UserAuthorizationRequest requestAuth;

                if ((requestAuth = request as UserAuthorizationRequest) != null)
                {
                    mLoggingService.AgregarEntrada("Peticion autorización de un request token");

                    //Ha llegado una petición de autorización de un request token, se le redirige al usuario a autorizar.aspx para que dé su consentimiento
                    PendingOAuthAuthorization = requestAuth;

                    mLoggingService.AgregarEntrada("Peticion redirijida a Autorizar.aspx");

                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Guid? proyectoID = mConfigService.ObtenerProyectoConexion();
                    if (proyectoID == null || proyectoID.Equals(Guid.Empty))
                    {
                        if (ProyectoSeleccionado != null)
                        {
                            string valor = paramCN.ObtenerParametroAplicacion(ProyectoSeleccionado.UrlPropia(IdiomaUsuario));

                            if (!string.IsNullOrEmpty(valor))
                            {
                                proyectoID = new Guid(valor);
                            }
                        }
                    }

                    Guid? organizacionID = mConfigService.ObtenerOrganizacionConexion();

                    string url = UtilDominios.ObtenerDominioUrl(Request.Path, true);

                    if (proyectoID.HasValue && !proyectoID.Value.Equals(ProyectoAD.MetaProyecto))
                    {
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
                        string nombreCortoProyecto = proyCL.ObtenerNombreCortoProyecto(proyectoID.Value);
                        url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(new UtilIdiomas("es", mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper), "", nombreCortoProyecto);

                    }

                    return Redirect(string.Concat(url, "/login/redirect/authorized"));
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            //return new HttpUnauthorizedResult();
            return Content("401: Unauthorized");
        }

        public ActionResult Authorized()
        {
            try
            {
                var pending = PendingOAuthAuthorization;
                ITokenContainingMessage tokenMessage = pending;

                if (HttpContext.Session.TryGetValue("usuario", out _))
                {
                    GnossIdentity usuario = HttpContext.Session.Get<GnossIdentity>("usuario");

                    TokenManager.ComprobarUsuarioEnOAuthBD(usuario.Login, usuario.UsuarioID.ToString());

                    TokenManager.AutorizarRequestToken(tokenMessage.Token, usuario.UsuarioID);
                    PendingOAuthAuthorization = null;
                    TokenManager.ActualizarBaseDeDatos();

                    var response = mServiceProvider.PrepareAuthorizationResponse(pending);

                    if (response != null)
                    {
                        mServiceProvider.Channel.Send(response);
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            return new UnauthorizedResult();
        }

        [HttpPost]
        public ActionResult AccessToken()
        {
            try
            {
                IProtocolMessage request = mServiceProvider.ReadRequest();
                AuthorizedTokenRequest requestAccessToken;

                if ((requestAccessToken = request as AuthorizedTokenRequest) != null)
                {
                    mLoggingService.AgregarEntrada("Peticion de access token");

                    //Llega una petición de access token, se invalida el request token de la petición y se envía un access token
                    var response = mServiceProvider.PrepareAccessTokenMessage(requestAccessToken);
                    mServiceProvider.Channel.Send(response);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }

            //return new HttpUnauthorizedResult();
            return Content("401: Unauthorized");
        }

        private void GuardarLogError(Exception ex)
        {
            mLoggingService.GuardarLogError(ex);
        }


        /// <summary>
        /// Obtiene el controlador de tokens
        /// </summary>
        public ControladorTokens TokenManager
        {
            get
            {
                ControladorTokens tokenManager = null;
                if ((HttpContext.Session != null) && HttpContext.Session.TryGetValue("tokenManager", out _))
                {
                    tokenManager = HttpContext.Session.Get<ControladorTokens>("tokenManager");
                }

                if (tokenManager == null)
                {
                    tokenManager = new ControladorTokens(mEntityContextOauth, mLoggingService, mEntityContext, mConfigService, null);

                    if (HttpContext.Session != null)
                    {
                        HttpContext.Session.Set("tokenManager", tokenManager);
                    }
                }
                return tokenManager;
            }
        }

        /// <summary>
        /// Obtiene el token pendiente de autorizar
        /// </summary>
        public UserAuthorizationRequest PendingOAuthAuthorization
        {
            get { return HttpContext.Session.Get<UserAuthorizationRequest>("authrequest"); }
            set { HttpContext.Session.Set("authrequest", value); }
        }

    }
}
