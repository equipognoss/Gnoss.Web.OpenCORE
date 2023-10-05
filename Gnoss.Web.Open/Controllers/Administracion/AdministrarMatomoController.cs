using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using MySql.Data.MySqlClient;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Web.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Data.SqlClient;

using static Es.Riam.Web.Util.UtilMatomo;
using Es.Riam.Util;
using System.IO;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    public class AdministrarMatomoController : ControllerBaseWeb
    {
        private AdministrarSeoGoogleViewModel mPaginaModel = null;
        private bool mConfigDeParametroProyecto = false;
        private bool mConfigGoogleDeParametroProyecto = false;
        private bool mScriptDeParametroGeneral = false;
        private string mMetaRobots = "all";

        public AdministrarMatomoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

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

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Matomo;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = "Administrar Matomo";

            //return View(PaginaModel);
            return View();
        }

        /// <summary>
        /// Se restablece el usuario para acceder a matomo. Si no existe se crea.
        /// </summary>
        /// <param name="pPassword">Contraseña actual del usuario</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult RestoreCurrentUser(string pPassword)
        {
            if (!string.IsNullOrEmpty(mConfigService.ObtenerUrlMatomo()))
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                Usuario usuario = usuarioCN.ObtenerUsuarioPorID(UsuarioActual.UsuarioID);

                if (!usuarioCN.ValidarPasswordUsuario(usuario, pPassword))
                {
                    return GnossResultERROR("La constraseña debe de ser la misma que la utilizada para loguearte en el proyecto");
                }

                UtilMatomo utilMatomo = new UtilMatomo(mConfigService.ObtenerOAuthMatomo(), mConfigService.ObtenerUrlMatomo());
                MatomoUserModel usuarioMatomo = utilMatomo.GetUser(usuario.Login);

                if (usuarioMatomo == null)
                {
                    string email = personaCN.ObtenerEmailPersonalPorUsuario(usuario.UsuarioID);
                    bool userAdded = utilMatomo.AddUser(usuario.Login, pPassword, email);

                    if (!userAdded)
                    {
                        GnossResultERROR("El usuario no ha podido añadirse correctamente.");
                    }
                }
                return GnossResultOK();
            }
            else
            {
                return GnossResultERROR("No esta Matomo configurado en el proyecto. Ponte en contacto con el administrador para darlo de alta.");
            }
        }

        /// <summary>
        /// Restablece la contraseña del administrador de matomo con la contraseña indicada
        /// </summary>
        /// <param name="pPassword">Contraseña a establecer para el adminsitrador de matomo</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult RestoreAdminMatomoPassword(string pPassword)
        {
            if (!string.IsNullOrEmpty(mConfigService.ObtenerUrlMatomo()))
            {
                string url = $"http://matomo_hash_generator/HashGenerator.php?password={pPassword}";
                StreamReader streamReader = new StreamReader(UtilWeb.HacerPeticionGetDevolviendoWebResponse(url).GetResponseStream());

                string hashNewPassword = streamReader.ReadToEnd();

                MySqlConnection connection = new MySqlConnection(mConfigService.ObtenerCadenaConexionMatomo());
                connection.Open();

                string query = $"UPDATE matomo.matomo_user SET password = '{hashNewPassword}' WHERE login = 'matomo'";
                MySqlCommand mySqlCommand = new MySqlCommand(query, connection);
                string response = mySqlCommand.ExecuteScalar().ToString();

                return GnossResultOK();
            }
            else
            {
                return GnossResultERROR("No esta Matomo configurado en el proyecto. Ponte en contacto con el administrador para darlo de alta.");
            }
        }
    }
}