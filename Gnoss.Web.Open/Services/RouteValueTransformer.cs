using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Routes;
using Gnoss.Web.Open.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Gnoss.Web.Services
{
    public class RouteValueTransformer : DynamicRouteValueTransformer
    {
        private IServiceScopeFactory mScopedFactory;
        private ConfigService mConfigService;
        private static Dictionary<string, List<PestanyaRouteModel>> DICTIONARY_PROJECT_TABS = new Dictionary<string, List<PestanyaRouteModel>>();
        private static string PROYECTO_DEFECTO = "";
        private static string IDIOMA_DEFECTO = "";
        public RouteValueTransformer(IServiceScopeFactory scopedFactory, ConfigService configService)
        {
            mScopedFactory = scopedFactory;
            mConfigService = configService;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            string[] segmentos = httpContext.Request.Path.ToUriComponent().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segmentos.Length > 0)
            {
                bool urlCorta = true;
                bool urlConIdioma = false;
                values = new RouteValueDictionary();
                string rutaPedida = "";
                string idioma = "";

                string comunidadTxt = "";

                string nombreCortoComunidad = "";
                if (segmentos[0].Length == 2)
                {
                    if (mConfigService.ObtenerListaIdiomasDictionary().ContainsKey(segmentos[0]))
                    {
                        idioma = segmentos[0];
                        urlConIdioma = true;
                    }

                    UtilIdiomas utilIdiomas = new UtilIdiomas(idioma, null, null, mConfigService);
                    comunidadTxt = utilIdiomas.GetText("COMMON", "COMUNIDAD").ToLower();

                    if (segmentos.Length > 2 && !string.IsNullOrEmpty(segmentos[2]))
                    {
                        string comunidadSegmento = segmentos[1].ToLower();
                        if (comunidadSegmento.Equals(comunidadTxt))
                        {
                            nombreCortoComunidad = segmentos[2];
                            urlCorta = false;
                        }
                    }

                }
                else if (segmentos.Length > 1 && !string.IsNullOrEmpty(segmentos[1]))
                {
                    idioma = mConfigService.ObtenerIdiomaDefecto();
                    if (string.IsNullOrEmpty(idioma))
                    {
                        idioma = "es";
                    }
                    IDIOMA_DEFECTO = idioma;

                    UtilIdiomas utilIdiomas = new UtilIdiomas(idioma, null, null, mConfigService);
                    comunidadTxt = utilIdiomas.GetText("COMMON", "COMUNIDAD").ToLower();

                    string comunidadSegmento = segmentos[0].ToLower();
                    if (comunidadSegmento.Equals(comunidadTxt))
                    {
                        nombreCortoComunidad = segmentos[1];
                        urlCorta = false;
                    }
                }

                if (string.IsNullOrEmpty(nombreCortoComunidad) && string.IsNullOrEmpty(PROYECTO_DEFECTO))
                {
                    using (var scope = mScopedFactory.CreateScope())
                    {
                        Guid proyectoDefectoID = Guid.Empty;
                        if (mConfigService.ObtenerProyectoConexion().HasValue)
                        {
                            proyectoDefectoID = mConfigService.ObtenerProyectoConexion().Value;
                        }

                        if (proyectoDefectoID != Guid.Empty)
                        {
                            EntityContext entityContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
                            string valor = entityContext.ParametroProyecto.Where(item => item.ProyectoID.Equals(proyectoDefectoID) && item.Parametro.Equals("ProyectoSinNombreCortoEnURL")).Select(item => item.Valor).FirstOrDefault();

                            if (!string.IsNullOrEmpty(valor) && valor.Equals("1"))
                            {
                                PROYECTO_DEFECTO = entityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(proyectoDefectoID)).Select(item => item.NombreCorto).FirstOrDefault();
                                nombreCortoComunidad = PROYECTO_DEFECTO;
                            }
                        }
                    }
                }
                else if (string.IsNullOrEmpty(nombreCortoComunidad) && !string.IsNullOrEmpty(PROYECTO_DEFECTO))
                {
                    nombreCortoComunidad = PROYECTO_DEFECTO;
                }

                values.Add("NombreCortoComunidad", nombreCortoComunidad);
                if (string.IsNullOrEmpty(idioma) && string.IsNullOrEmpty(IDIOMA_DEFECTO))
                {
                    using (var scope = mScopedFactory.CreateScope())
                    {
                        EntityContext entityContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
                        try
                        {
                            idioma = mConfigService.ObtenerIdiomaDefecto();
                        }
                        catch (Exception) { }
                        if (string.IsNullOrEmpty(idioma))
                        {
                            string idiomasDefecto = entityContext.ParametroAplicacion.Where(item => item.Parametro.Equals("Idiomas")).Select(item => item.Valor).FirstOrDefault();
                            if (!string.IsNullOrEmpty(idiomasDefecto))
                            {
                                idioma = idiomasDefecto.Split("&&&")[0].Split("|")[0];
                            }
                            else
                            {
                                idioma = "es";
                            }
                        }
                    }
                    IDIOMA_DEFECTO = idioma;
                }
                else if (string.IsNullOrEmpty(idioma) && !string.IsNullOrEmpty(IDIOMA_DEFECTO))
                {
                    idioma = IDIOMA_DEFECTO;
                }

                values.Add("lang", idioma);

                rutaPedida = ObtenerRutaPedida(segmentos, urlConIdioma, urlCorta);

                if (rutaPedida.Contains('?'))
                {
                    rutaPedida = rutaPedida.Split('?')[0];
                }
                lock (DICTIONARY_PROJECT_TABS)
                {
                    if (!DICTIONARY_PROJECT_TABS.ContainsKey(nombreCortoComunidad))
                    {
                        CargarRutasProyecto(nombreCortoComunidad, mConfigService, mScopedFactory);
                    }
                }

                PestanyaRouteModel pestanyaRouteModel = DICTIONARY_PROJECT_TABS[nombreCortoComunidad].FirstOrDefault(item => item.RutaPestanya.Equals(rutaPedida));
                if (pestanyaRouteModel != null)
                {
                    switch (pestanyaRouteModel.TipoPestanya)
                    {
                        case (short)TipoPestanyaMenu.BusquedaSemantica:
                            values.Add("controller", "Busqueda");
                            values.Add("action", "Index");
                            values.Add("semanticos", "true");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                        case (short)TipoPestanyaMenu.Recursos:
                            values.Add("controller", "Busqueda");
                            values.Add("action", "Index");
                            values.Add("Recursos", "true");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                        case (short)TipoPestanyaMenu.Preguntas:
                            values.Add("controller", "Busqueda");
                            values.Add("action", "Index");
                            values.Add("preguntas", "true");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                        case (short)TipoPestanyaMenu.Debates:
                            values.Add("controller", "Busqueda");
                            values.Add("action", "Index");
                            values.Add("debates", "true");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                        case (short)TipoPestanyaMenu.Encuestas:
                            values.Add("controller", "Busqueda");
                            values.Add("action", "Index");
                            values.Add("encuestas", "true");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                        case (short)TipoPestanyaMenu.PersonasYOrganizaciones:
                            values.Add("controller", "Busqueda");
                            values.Add("action", "Index");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            values.Add("perorg", "true");
                            break;
                        case (short)TipoPestanyaMenu.AcercaDe:
                            values.Add("controller", "AcercaDeComunidad");
                            values.Add("action", "Index");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                        case (short)TipoPestanyaMenu.BusquedaAvanzada:
                            values.Add("controller", "Busqueda");
                            values.Add("action", "Index");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            values.Add("Meta", "true");
                            break;
                        case (short)TipoPestanyaMenu.Comunidades:
                            values.Add("controller", "Busqueda");
                            values.Add("action", "Index");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            values.Add("comunidades", "true");
                            break;
                        case (short)TipoPestanyaMenu.CMS:
                            values.Add("controller", "CMSPagina");
                            values.Add("action", "Index");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                        case (short)TipoPestanyaMenu.Home:
                            values.Add("controller", "HomeComunidad");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                        case (short)TipoPestanyaMenu.Indice:
                            values.Add("controller", "Indice");
                            values.Add("PestanyaID", pestanyaRouteModel.PestanyaID.ToString());
                            break;
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// SANTI
        /// </summary>
        /// <param name="pSegmentos"></param>
        /// <param name="pEsRutaConIdioma"></param>
        /// <returns></returns>
        private string ObtenerRutaPedida(string[] pSegmentos, bool pEsRutaConIdioma, bool pEsUrlCorta)
        {
            string rutaPedida = "";
            int indiceRutaPedida = 0;
            if (!pEsRutaConIdioma && (string.IsNullOrEmpty(PROYECTO_DEFECTO) || !pEsUrlCorta))
            {
                //Es ruta sin idioma y larga. Ej --> testing.gnoss.com/comunidad/testing/recursos
                indiceRutaPedida = 2;
            }
            else if (!pEsRutaConIdioma && !string.IsNullOrEmpty(PROYECTO_DEFECTO))
            {
                //Es ruta sin idioma y con proyecto por defecto. Ej --> testing.gnoss.com/recursos
                indiceRutaPedida = 0;
            }
            else if ((string.IsNullOrEmpty(PROYECTO_DEFECTO) || !pEsUrlCorta) && pEsRutaConIdioma)
            {
                //Es ruta con idioma y larga. Ej --> testing.gnoss.com/es/comunidad/testing/recursos
                indiceRutaPedida = 3;
            }
            else
            {
                //Es ruta con idioma y con proyecto por defecto. Ej --> testing.gnoss.com/es/recursos
                indiceRutaPedida = 1;
            }

            while (indiceRutaPedida < pSegmentos.Length)
            {
                rutaPedida = $"{rutaPedida}/{pSegmentos[indiceRutaPedida]}";
                indiceRutaPedida++;
            }

            rutaPedida = rutaPedida.TrimStart('/');

            return rutaPedida;
        }

        public static void EliminarRutasProyecto(string pNombreCorto)
        {
            lock (DICTIONARY_PROJECT_TABS)
            {
                if (DICTIONARY_PROJECT_TABS.ContainsKey(pNombreCorto))
                {
                    DICTIONARY_PROJECT_TABS.Remove(pNombreCorto);
                }
            }
        }

        public static void RecalcularRutasProyecto(string pNombreCorto, ConfigService pConfigService, IServiceScopeFactory pServiceScopeFactory)
        {
            lock (DICTIONARY_PROJECT_TABS)
            {
                if (DICTIONARY_PROJECT_TABS.ContainsKey(pNombreCorto))
                {
                    DICTIONARY_PROJECT_TABS.Remove(pNombreCorto);
                }
                CargarRutasProyecto(pNombreCorto, pConfigService, pServiceScopeFactory);
            }
        }

        private static void CargarRutasProyecto(string pNombreCortoProyecto, ConfigService pConfigService, IServiceScopeFactory pServiceScopeFactory)
        {

            lock (DICTIONARY_PROJECT_TABS)
            {
                using (var scope = pServiceScopeFactory.CreateScope())
                {
                    List<PestanyaRouteModel> listTabs = new List<PestanyaRouteModel>();
                    if (!string.IsNullOrEmpty(pNombreCortoProyecto))
                    {
                        EntityContext entityContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
                        LoggingService loggingService = scope.ServiceProvider.GetRequiredService<LoggingService>();
                        RedisCacheWrapper redisCacheWrapper = scope.ServiceProvider.GetRequiredService<RedisCacheWrapper>();
                        VirtuosoAD virtuosoAD = scope.ServiceProvider.GetRequiredService<VirtuosoAD>();

                        ProyectoCL proyectoCL = new ProyectoCL(entityContext, loggingService, redisCacheWrapper, pConfigService, virtuosoAD, null);
                        List<Guid> proyectoYProyectoSuperiorID = new List<Guid>();

                        proyectoYProyectoSuperiorID = proyectoCL.ObtenerProyectoYProyectoSuperiorIDs(pNombreCortoProyecto);

                        var listTabsDB = entityContext.ProyectoPestanyaMenu.Where(item => proyectoYProyectoSuperiorID.Contains(item.ProyectoID) && !string.IsNullOrEmpty(item.Ruta) && !item.TipoPestanya.Equals((short)TipoPestanyaMenu.EnlaceInterno) && !item.TipoPestanya.Equals((short)TipoPestanyaMenu.EnlaceExterno)).Select(item => new { item.PestanyaID, item.TipoPestanya, item.Ruta }).ToList();


                        foreach (var tab in listTabsDB)
                        {
                            string[] rutas = tab.Ruta.Split("|||");
                            foreach (string ruta in rutas)
                            {
                                if (!string.IsNullOrEmpty(ruta))
                                {
                                    PestanyaRouteModel pestanyaRouteModel = new PestanyaRouteModel();
                                    if (ruta.Contains("@"))
                                    {
                                        string[] ruta_idioma = ruta.Split("@");
                                        pestanyaRouteModel.Idioma = ruta_idioma[1];
                                        pestanyaRouteModel.PestanyaID = tab.PestanyaID;
                                        pestanyaRouteModel.RutaPestanya = ruta_idioma[0];
                                        pestanyaRouteModel.TipoPestanya = tab.TipoPestanya;
                                    }
                                    else
                                    {
                                        pestanyaRouteModel.Idioma = "";
                                        pestanyaRouteModel.PestanyaID = tab.PestanyaID;
                                        pestanyaRouteModel.RutaPestanya = ruta;
                                        pestanyaRouteModel.TipoPestanya = tab.TipoPestanya;
                                    }
                                    listTabs.Add(pestanyaRouteModel);
                                }
                            }
                        }
                    }
                    DICTIONARY_PROJECT_TABS.Add(pNombreCortoProyecto, listTabs);
                }
            }
        }

        private void LeerRutas()
        {
            XmlDocument xmlRoute = new XmlDocument();
            xmlRoute.Load(new StringReader(Resources.routemap));

            //Lista de urls del metaproyecto, con identidad personal y de organización
            Dictionary<string, string> listaUrlsMetaProyecto = new Dictionary<string, string>();
            ObtenerListaRutas(xmlRoute, listaUrlsMetaProyecto, "metaproyecto");

            Dictionary<string, string> listaUrlsMetaAdministrador = new Dictionary<string, string>();
            ObtenerListaRutas(xmlRoute, listaUrlsMetaAdministrador, "metaAdministrador");

            //Lista de urls del metaproyecto, solo con identidad de organización
            Dictionary<string, string> listaUrlsSoloOrg = new Dictionary<string, string>();
            ObtenerListaRutas(xmlRoute, listaUrlsSoloOrg, "organizacion");

            //Lista de urls comunes a la comunidad y al metaproyecto, con identidad personal y de organización
            Dictionary<string, string> listaUrls = new Dictionary<string, string>();
            ObtenerListaRutas(xmlRoute, listaUrls, "comun");

            //Lista de urls de una comunidad
            Dictionary<string, string> listaUrlsCom = new Dictionary<string, string>();
            ObtenerListaRutas(xmlRoute, listaUrlsCom, "comunidad");

            //Lista de urls de ficha de recurso de una comunidad
            Dictionary<string, string> listaUrlsFichaRecursoCom = new Dictionary<string, string>();
            ObtenerListaRutas(xmlRoute, listaUrlsFichaRecursoCom, "fichaRecurso");
        }

        /// <summary>
        /// Obtiene la lista de rutas de una seccion del fichero RouteMap.xml
        /// </summary>
        /// <param name="xmlRoute">Fichero RouteMap.xml</param>
        /// <param name="listaUrls">Lista en la que vamos a guardar las urls leídas</param>
        /// <param name="seccion">Sección de la que vamos a leer las rutas</param>
        private static void ObtenerListaRutas(XmlDocument xmlRoute, Dictionary<string, string> listaUrls, string seccion)
        {
            XmlNodeList listaNodos = xmlRoute.SelectNodes("routeMap/seccion[@name='" + seccion + "']/pagina");
            foreach (XmlNode nodo in listaNodos)
            {
                string urlMapeo = nodo["key"].InnerText.Replace("@#@@", "@#@$").Replace("@@#@", "$@#@");

                listaUrls.Add(urlMapeo, nodo["value"].InnerText);
            }
        }
    }
}
