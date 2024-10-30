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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using static Es.Riam.Web.Util.UtilMatomo;
using Es.Riam.Util;
using System.IO;
using System.Web;
using System.Net;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{


    /// <summary>
    /// 
    /// </summary>
    public class AdministrarMatomoController : ControllerBaseWeb
    {
        public AdministrarMatomoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
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
            ViewBag.BodyClassPestanya = "configuracionGeneral edicion edicionPaginas listado no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_Matomo;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTADISTICASDELACOMUNIDAD");

            return View();
        }





        /// <summary>
        /// Se devuelve elo JSON de la estadística de Matomo correspondiente en el html.
        /// </summary>
        /// <param name="pModel">Gráfico que queremos leer</param>
        /// <returns>-</returns>
        [HttpPost]
        public ActionResult Graphic(MatomoGraphicsViewModel pModel)
        {
            UtilMatomo utilMatomo = new UtilMatomo(mConfigService.ObtenerOAuthMatomo(), mConfigService.ObtenerUrlMatomo());
            string json = utilMatomo.GetGraphic(pModel);
            return Content(json, "application/json");
        }

        /// <summary>
        /// Se devuelve elo JSON de la estadística de Matomo correspondiente en el html.
        /// </summary>
        /// <param name="pModel">Gráfico que queremos leer</param>
        /// <returns>-</returns>
        [HttpPost]
        public ActionResult TopUsersGraphic(MatomoGraphicsViewModel pModel)
        {
            UtilMatomo utilMatomo = new UtilMatomo(mConfigService.ObtenerOAuthMatomo(), mConfigService.ObtenerUrlMatomo());
            string json = utilMatomo.GetGraphic(pModel);

            var usersMatomo = UsersMatomo.FromJson(json);

            List<Guid> usuariosID = new List<Guid>();
            // Obtener lista de usuarios del objeto
            foreach (var dias in usersMatomo)
            {
                if (dias.Value.Count != 0)
                {
                    foreach (var u in dias.Value)
                    {
                        string[] id = u.Segment.Split("==");
                        Guid guidUsuario = new Guid();
                        if (Guid.TryParse(id[1], out guidUsuario) && !usuariosID.Contains(guidUsuario))
                        {
                            usuariosID.Add(guidUsuario);
                        }
                    }
                }
            }

            PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<string, string> nombresPersonas = persCN.ObtenerNombreCortoYNombresPersonasDeUsuariosID(usuariosID);
            Dictionary<string, string> nombreAsociadoAId = new Dictionary<string, string>();
            List<string> namesUsers = nombresPersonas.Keys.ToList();
            for (int i = 0; i < nombresPersonas.Count; i++)
            {
                nombreAsociadoAId.Add(usuariosID[i].ToString(), namesUsers[i]);
            }
            foreach (var dias in usersMatomo)
            {
                if (dias.Value.Count != 0)
                {
                    foreach (var u in dias.Value)
                    {
                        string[] id = u.Segment.Split("==");
                        if (nombreAsociadoAId.ContainsKey(id[1]))
                        {
                            //foreach (string nombreCorto in nombresPersonas.Keys)
                            string nombreCorto = nombreAsociadoAId[id[1]];
                            string urlPerfil = GnossUrlsSemanticas.GetURLPerfilPersonaOOrgEnProyecto(BaseURL, UtilIdiomas, UrlPerfil, ProyectoSeleccionado.NombreCorto, nombreCorto, null);
                            string nombre = nombresPersonas[nombreCorto];
                            u.nombrePerfil = nombre;
                            u.urlPerfil = urlPerfil;
                            u.idPerfil = id[1];
                        }
                    }
                }
            }


            var jsonString = JsonConvert.SerializeObject(usersMatomo);
            return Content(jsonString, "application/json");
        }


        /// <summary>
        /// Se devuelve elo JSON de la estadística de Matomo correspondiente en el html.
        /// </summary>
        /// <param name="pModel">Gráfico que queremos leer</param>
        /// <returns>-</returns>
        [HttpPost]
        public ActionResult TopDownloadsGraphic(MatomoGraphicsViewModel pModel)
        {
            UtilMatomo utilMatomo = new UtilMatomo(mConfigService.ObtenerOAuthMatomo(), mConfigService.ObtenerUrlMatomo());
            string json = utilMatomo.GetGraphic(pModel);
            List<RootMatomo> myDeserializedClass = JsonConvert.DeserializeObject<List<RootMatomo>>(json);

            List<Guid> documentos = new List<Guid>();

            foreach (RootMatomo day in myDeserializedClass)
            {
                if (day.actionDetails.Count != 0)
                {
                    foreach (var a in day.actionDetails)
                    {
                        if (!string.IsNullOrEmpty(a.type) && a.type.Equals("download"))
                        {
                            Uri urlRecurso = new Uri(a.url);
                            string param1 = HttpUtility.ParseQueryString(urlRecurso.Query).Get("doc");
                            a.ResourceID = param1;
                            Guid guidRecurso = new Guid();
                            if (Guid.TryParse(param1, out guidRecurso) && !documentos.Contains(guidRecurso))
                            {
                                documentos.Add(guidRecurso);
                            }
                        }

                        if (!string.IsNullOrEmpty(a.eventName) && a.eventName.Equals("Me gusta"))
                        {
                            if (!string.IsNullOrEmpty(a.dimension4))
                            {
                                string param1 = a.dimension4.Split('/').Last();
                                a.ResourceID = param1;
                                Guid guidRecurso = new Guid();
                                if (Guid.TryParse(param1, out guidRecurso))
                                {
                                    if (!documentos.Contains(guidRecurso))
                                    {
                                        documentos.Add(guidRecurso);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(a.eventName) && a.eventName.Equals("Comentario"))
                        {
                            string urlWithoutQuery = a.url.Split('?')[0];
                            string param1 = urlWithoutQuery.Split('/').Last();
                            a.ResourceID = param1;
                            Guid guidRecurso = Guid.Empty;
                            if (Guid.TryParse(param1, out guidRecurso) && !documentos.Contains(guidRecurso))
                            {
                                documentos.Add(guidRecurso);
                            }

                        }
                    }
                }
            }

            var titulos = mEntityContext.Documento.Where(item => documentos.Contains(item.DocumentoID)).Select(item => new { item.DocumentoID, item.ElementoVinculadoID, item.Titulo });

            Dictionary<string, string> recursosUrlYNombre = titulos.ToDictionary(x => x.DocumentoID.ToString(), x => x.Titulo);


            foreach (var day in myDeserializedClass)
            {
                if (day.actionDetails.Count != 0)
                {
                    foreach (var act in day.actionDetails)
                    {
                        if (act.type.Equals("download") && (act.ResourceID != null))
                        {
                            act.ResourceName = recursosUrlYNombre[act.ResourceID];
                            string url = GnossUrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, recursosUrlYNombre[act.ResourceID], new Guid(act.ResourceID), Guid.Empty, false);
                            act.ResourceURL = url;
                        }

                        if (!string.IsNullOrEmpty(act.eventName) && act.eventName.Equals("Me gusta") && act.ResourceID != null && recursosUrlYNombre.ContainsKey(act.ResourceID))
                        {
                            act.ResourceName = recursosUrlYNombre[act.ResourceID];
                            string url = GnossUrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, recursosUrlYNombre[act.ResourceID], new Guid(act.ResourceID), Guid.Empty, false);
                            act.ResourceURL = url;
                        }

                        if (!string.IsNullOrEmpty(act.eventName) && act.eventName.Equals("Comentario") && act.ResourceID != null && recursosUrlYNombre.ContainsKey(act.ResourceID))
                        {
                            act.ResourceName = recursosUrlYNombre[act.ResourceID];
                            string url = GnossUrlsSemanticas.GetURLBaseRecursosFichaConIDs(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, recursosUrlYNombre[act.ResourceID], new Guid(act.ResourceID), Guid.Empty, false);
                            act.ResourceURL = url;
                        }
                    }
                }
            }

            var jsonString = JsonConvert.SerializeObject(myDeserializedClass);
            return Content(jsonString, "application/json");
        }


        /// <summary>
        /// Se devuelve el src del widget de la estadística de Matomo correspondiente .
        /// </summary>
        /// <param name="pModel">Widget que queremos leer</param>
        /// <returns>-</returns>

        public ActionResult Widget(MatomoWidgetViewModel pModel)
        {
            UtilMatomo utilMatomo = new UtilMatomo(mConfigService.ObtenerOAuthMatomo(), mConfigService.ObtenerUrlMatomo());
            string content = utilMatomo.GetWidget(pModel);
            content = content.Replace("index.php", $"{UtilIdiomas.GetText("URLSEM", "ADMINISTRARMATOMO")}/matomorequest/index.php");

            return Content(content, "text/html");
        }


        /// <summary>
        /// Se devuelve el src del widget de la estadística de Matomo correspondiente .
        /// </summary>
        /// <param name="pModel">Widget que queremos leer</param>
        /// <returns>-</returns>
        public ActionResult MatomoRequest(MatomoWidgetViewModel pModel)
        {
            UtilMatomo utilMatomo = new UtilMatomo(mConfigService.ObtenerOAuthMatomo(), mConfigService.ObtenerUrlMatomo());
            WebResponse response = utilMatomo.MatomoRequest(RequestParams("matomopage"), Request.QueryString.Value);

            string content = null;
            using (var stream = response.GetResponseStream())
            {
                content = new StreamReader(stream).ReadToEnd();
            }

            return Content(content, response.ContentType);
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
                    return GnossResultERROR("La contraseña debe de ser la misma que la utilizada para loguearte en el proyecto");
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
            GuardarLogAuditoria();
            if (!string.IsNullOrEmpty(mConfigService.ObtenerUrlMatomo()))
            {
                string url = $"http://matomo_hash_generator/HashGenerator.php?password={pPassword}";
                StreamReader streamReader = new StreamReader(UtilWeb.HacerPeticionGetDevolviendoWebResponse(url).GetResponseStream());

                string hashNewPassword = streamReader.ReadToEnd();

                MySqlConnection connection = new MySqlConnection(mConfigService.ObtenerCadenaConexionMatomo());
                connection.Open();

                string query = $"UPDATE matomo.matomo_user SET password = '{hashNewPassword}' WHERE login = 'matomo'";
                MySqlCommand mySqlCommand = new MySqlCommand(query, connection);
                mySqlCommand.ExecuteNonQuery();

                return GnossResultOK();
            }
            else
            {
                return GnossResultERROR("No esta Matomo configurado en el proyecto. Ponte en contacto con el administrador para darlo de alta.");
            }
        }
    }


    #region classes

    public class ActionDetailMatomo
    {
        public string type { get; set; }
        public string url { get; set; }
        public string pageTitle { get; set; }
        public int? pageIdAction { get; set; }
        public string idpageview { get; set; }
        public string serverTimePretty { get; set; }
        public int pageId { get; set; }
        public int timeSpent { get; set; }
        public string timeSpentPretty { get; set; }
        public int? pageviewPosition { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string icon { get; set; }
        public string iconSVG { get; set; }
        public int timestamp { get; set; }
        public string dimension2 { get; set; }
        public string dimension4 { get; set; }
        public string pageLoadTime { get; set; }
        public int? pageLoadTimeMilliseconds { get; set; }
        public string siteSearchKeyword { get; set; }
        public string siteSearchCategory { get; set; }
        public object siteSearchCount { get; set; }
        public string eventCategory { get; set; }
        public string eventAction { get; set; }
        public string eventName { get; set; }
        public string ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string ResourceURL { get; set; }
        public int? eventValue { get; set; }
    }

    public class PluginsIconMatomo
    {
        public string pluginIcon { get; set; }
        public string pluginName { get; set; }
    }

    public class RootMatomo
    {
        public int idSite { get; set; }
        public int idVisit { get; set; }
        public string visitIp { get; set; }
        public string visitorId { get; set; }
        public string fingerprint { get; set; }
        public List<ActionDetailMatomo> actionDetails { get; set; }
        public int goalConversions { get; set; }
        public string siteCurrency { get; set; }
        public string siteCurrencySymbol { get; set; }
        public string serverDate { get; set; }
        public string visitServerHour { get; set; }
        public int lastActionTimestamp { get; set; }
        public string lastActionDateTime { get; set; }
        public string siteName { get; set; }
        public int serverTimestamp { get; set; }
        public int firstActionTimestamp { get; set; }
        public string serverTimePretty { get; set; }
        public string serverDatePretty { get; set; }
        public string serverDatePrettyFirstAction { get; set; }
        public string serverTimePrettyFirstAction { get; set; }
        public string userId { get; set; }
        public string visitorType { get; set; }
        public string visitorTypeIcon { get; set; }
        public int visitConverted { get; set; }
        public object visitConvertedIcon { get; set; }
        public int visitCount { get; set; }
        public string visitEcommerceStatus { get; set; }
        public object visitEcommerceStatusIcon { get; set; }
        public int daysSinceFirstVisit { get; set; }
        public int secondsSinceFirstVisit { get; set; }
        public int daysSinceLastEcommerceOrder { get; set; }
        public object secondsSinceLastEcommerceOrder { get; set; }
        public int visitDuration { get; set; }
        public string visitDurationPretty { get; set; }
        public int searches { get; set; }
        public int actions { get; set; }
        public int interactions { get; set; }
        public string referrerType { get; set; }
        public string referrerTypeName { get; set; }
        public string referrerName { get; set; }
        public string referrerKeyword { get; set; }
        public object referrerKeywordPosition { get; set; }
        public string referrerUrl { get; set; }
        public object referrerSearchEngineUrl { get; set; }
        public object referrerSearchEngineIcon { get; set; }
        public object referrerSocialNetworkUrl { get; set; }
        public object referrerSocialNetworkIcon { get; set; }
        public string languageCode { get; set; }
        public string language { get; set; }
        public string deviceType { get; set; }
        public string deviceTypeIcon { get; set; }
        public string deviceBrand { get; set; }
        public string deviceModel { get; set; }
        public string operatingSystem { get; set; }
        public string operatingSystemName { get; set; }
        public string operatingSystemIcon { get; set; }
        public string operatingSystemCode { get; set; }
        public string operatingSystemVersion { get; set; }
        public string browserFamily { get; set; }
        public string browserFamilyDescription { get; set; }
        public string browser { get; set; }
        public string browserName { get; set; }
        public string browserIcon { get; set; }
        public string browserCode { get; set; }
        public string browserVersion { get; set; }
        public int events { get; set; }
        public string continent { get; set; }
        public string continentCode { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string countryFlag { get; set; }
        public object region { get; set; }
        public object regionCode { get; set; }
        public object city { get; set; }
        public string location { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public string visitLocalTime { get; set; }
        public string visitLocalHour { get; set; }
        public int daysSinceLastVisit { get; set; }
        public int secondsSinceLastVisit { get; set; }
        public string resolution { get; set; }
        public string plugins { get; set; }
        public List<PluginsIconMatomo> pluginsIcons { get; set; }
        public string dimension1 { get; set; }
        public string dimension3 { get; set; }
    }




    public partial class UsersMatomo
    {
        [JsonProperty("nb_uniq_visitors")]
        public long NbUniqVisitors { get; set; }

        [JsonProperty("nb_visits")]
        public long NbVisits { get; set; }

        [JsonProperty("nb_actions")]
        public long NbActions { get; set; }

        [JsonProperty("nb_users")]
        public long NbUsers { get; set; }

        [JsonProperty("max_actions")]
        public long MaxActions { get; set; }

        [JsonProperty("sum_visit_length")]
        public long SumVisitLength { get; set; }

        [JsonProperty("bounce_count")]
        public long BounceCount { get; set; }

        [JsonProperty("nb_visits_converted")]
        public long NbVisitsConverted { get; set; }

        [JsonProperty("idvisitor")]
        public string Idvisitor { get; set; }

        [JsonProperty("segment")]
        public string Segment { get; set; }

        public string urlPerfil { get; set; }

        public string nombrePerfil { get; set; }

        public string idPerfil { get; set; }
    }

    public partial class UsersMatomo
    {
        public static Dictionary<string, List<UsersMatomo>> FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, List<UsersMatomo>>>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Dictionary<string, List<UsersMatomo>> self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    #endregion
}

