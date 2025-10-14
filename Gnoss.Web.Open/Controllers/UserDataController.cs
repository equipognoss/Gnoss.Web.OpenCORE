using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class UserDataController : Controller
    {

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public UserDataController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, ILogger<UserDataController> logger, ILoggerFactory loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        //
        // GET: /UserData/
        [HttpPost]
        public ActionResult Email()
        {
            string email = "";

            try
            {
                Guid usuarioID = ComprobarPermisosOauth();
                if (!usuarioID.Equals(Guid.Empty))
                {
                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                    email = personaCN.ObtenerEmailPersonalPorUsuario(usuarioID);
                    personaCN.Dispose();
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }

            return Content(email);
        }


        /// <summary>
        /// Si la petición es Oauth, comprueba si el dueño del token enviado tiene permisos para interactuar con el recurso.
        /// </summary>
        /// <returns>UsuarioID de la peticion Oauth</returns>
        public Guid ComprobarPermisosOauth()
        {
            Guid usuarioID = Guid.Empty;
            try
            {

                string urlPeticionOauth = Es.Riam.Util.UtilOAuth.ObtenerUrlGetDePeticionOAuth(Request);
                
                CallOauthService servicioOauth = new CallOauthService(mConfigService);

                usuarioID = servicioOauth.ObtenerUsuarioAPartirDeUrl(urlPeticionOauth, Request.Method);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }

            return usuarioID;
        }


    }
}
