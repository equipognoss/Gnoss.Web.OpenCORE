using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Cookie;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using Es.Riam.Gnoss.Recursos;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Modelo de la página de administrar cookies
    /// </summary>
    [Serializable]
    public class AdministrarCookiesViewModel
    {
        /// <summary>
        /// Lista de idiomas de la plataforma
        /// </summary>
        public Dictionary<string, string> ListaIdiomas { get; set; }

        /// <summary>
        /// Indica si es contenido multiidioma
        /// </summary>
        public bool ContenidoMultiIdioma { get; set; }

        /// <summary>
        /// Idioma por defecto de la comunidad
        /// </summary>
        public string IdiomaPorDefecto { get; set; }

        /// <summary>
        /// Lista de cookies del proyecto
        /// </summary>
        public List<CookieModel> ListaProyectoCookie { get; set; }

        /// <summary>
        /// Lista de categorias de las cookies del proyecto
        /// </summary>
        public List<CategoriaCookieModel> ListaCategoriaProyectoCookie { get; set; }
    }

    /// <summary>
    /// Modelo de cookies para utilizar en la página de administrar cookies
    /// </summary>
    [Serializable]
    public class CookieModel
    {
        /// <summary>
        /// Cookie del proyecto
        /// </summary>
        public ProyectoCookie ProyectoCookie { get; set; }

        /// <summary>
        /// Lista de categorías de la cookie
        /// </summary>
        public List<CategoriaProyectoCookie> ListaCategoriaProyectoCookie { get; set; }

        /// <summary>
        /// Indica se se ha modificado o es nueva
        /// </summary>
        public bool Modificada { get; set; }
    }

    /// <summary>
    /// Modelo de las categorías para la página de edición
    /// </summary>
    [Serializable]
    public class CategoryEditModel
    {
        /// <summary>
        /// Id de la categoría
        /// </summary>
        public string CategoriaID { get; set; }

        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Nombre corto de la categoría (se utiliza para mostrar y crear las cookies a mostrar en la página de manage-cookies)
        /// </summary>
        public string NombreCorto { get; set; }

        /// <summary>
        /// Descripción de la categoría
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Si se ha eliminado o no la categoría
        /// </summary>
        public string Deleted { get; set; }

        /// <summary>
        /// Si se ha modificado la categoría
        /// </summary>
        public string EsModificada { get; set; }
    }

    /// <summary>
    /// Modelo de la categoría de cookies
    /// </summary>
    [Serializable]
    public class CategoriaCookieModel
    {
        /// <summary>
        /// Categoría de la cookie
        /// </summary>
        public CategoriaProyectoCookie CategoriaProyectoCookie { get; set; }

        /// <summary>
        /// Si ha sido o no moficada la categoría
        /// </summary>
        public bool Modificada { get; set; }
        
        /// <summary>
        /// Si la categoría tiene o no alguna cookie vinculada
        /// </summary>
        public bool CookiesVinculadas { get; set; }
    }

    /// <summary>
    /// Controlador de administrar cookies
    /// </summary>
    public class AdministrarCookiesController : ControllerBaseWeb
    {
        private AdministrarCookiesViewModel mPaginaModel = null;
        private UtilIdiomas utilIdiomasEspanyol = null;
        private UtilIdiomas utilIdiomasCatalan = null;
        private UtilIdiomas utilIdiomasIngles = null;
        private UtilIdiomas utilIdiomasAleman = null;
        private UtilIdiomas utilIdiomasItaliano = null;
        private UtilIdiomas utilIdiomasFrances = null;
        private UtilIdiomas utilIdiomasGallego = null;
        private UtilIdiomas utilIdiomasEuskera = null;
        private UtilIdiomas utilIdiomasPortugues = null;

        public AdministrarCookiesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
             : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
            utilIdiomasEspanyol = new UtilIdiomas("es", mLoggingService, mEntityContext, mConfigService);
            utilIdiomasCatalan = new UtilIdiomas("ca", mLoggingService, mEntityContext, mConfigService);
            utilIdiomasIngles = new UtilIdiomas("en", mLoggingService, mEntityContext, mConfigService);
            utilIdiomasAleman = new UtilIdiomas("de", mLoggingService, mEntityContext, mConfigService);
            utilIdiomasItaliano = new UtilIdiomas("it", mLoggingService, mEntityContext, mConfigService);
            utilIdiomasFrances = new UtilIdiomas("fr", mLoggingService, mEntityContext, mConfigService);
            utilIdiomasGallego = new UtilIdiomas("gl", mLoggingService, mEntityContext, mConfigService);
            utilIdiomasEuskera = new UtilIdiomas("eu", mLoggingService, mEntityContext, mConfigService);
            utilIdiomasPortugues = new UtilIdiomas("pt", mLoggingService, mEntityContext, mConfigService);
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            return View(PaginaModel);
        }

        /// <summary>
        /// Crea las cookies iniciales del metaproyecto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public bool CrearCookiesTecnicasDelProyecto()
        {
            try
            {
                CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                cookieCN.CrearCookiesInicialesProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.Organizacion.Clave);

                EliminarPersonalizacionVistas();
                CargarPermisosAdministracionComunidadEnViewBag();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Se encarga de pintar las cookies para el metaproyecto
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PintarCookiesMetaproyecto()
        {
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<ProyectoCookie> listaCookiesProyecto = cookieCN.ObtenerCookiesDeProyecto(new Guid("11111111-1111-1111-1111-111111111111"));
            List<CategoriaProyectoCookie> listaCategoriaCookiesMetaproyecto = cookieCN.ObtenerCategoriasProyectoCookie(new Guid("11111111-1111-1111-1111-111111111111"));

            List<CookieModel> listaCookieModel = new List<CookieModel>();
            foreach (ProyectoCookie proyectoCookie in listaCookiesProyecto)
            {
                listaCookieModel.Add(new CookieModel { ProyectoCookie = proyectoCookie, ListaCategoriaProyectoCookie = listaCategoriaCookiesMetaproyecto, Modificada = false });
            }

            return PartialView("_CookiesProyecto", listaCookieModel);
        }

        /// <summary>
        /// Se encarga de pintar las categorías de las cookies para el metaproyecto
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PintarCategoriasCookiesMetaproyecto()
        {
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<CategoriaProyectoCookie> listaCategoriaCookiesMetaproyecto = cookieCN.ObtenerCategoriasProyectoCookie(new Guid("11111111-1111-1111-1111-111111111111"));

            List<CategoriaCookieModel> listaCategoriaCookieModel = new List<CategoriaCookieModel>();

            foreach (CategoriaProyectoCookie categoriaCookie in listaCategoriaCookiesMetaproyecto)
            {
                listaCategoriaCookieModel.Add(new CategoriaCookieModel { CategoriaProyectoCookie = categoriaCookie, Modificada = false });
            }

            return PartialView("_CategoriasCookies", listaCategoriaCookieModel);
        }

        /// <summary>
        /// Método para añadir una cookie a la vista
        /// </summary>
        /// <param name="Categoria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCookie(string Categoria)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ProyectoCookie cookie = new ProyectoCookie();
            string[] categoria = Categoria.Split('_');
            cookie.CategoriaID = new Guid(categoria[0]);
            cookie.CookieID = Guid.NewGuid();
            cookie.Nombre = "Nombre";
            cookie.Descripcion = "Descripcion";
            cookie.EsEditable = true;
            cookie.NombreCorto = "";
            cookie.Tipo = (short)TipoCookies.Persistent;
            cookie.ProyectoID = ProyectoSeleccionado.Clave;
            cookie.OrganizacionID = proyectoCN.ObtenerOrganizacionIDAPartirDeProyectoID(cookie.ProyectoID);

            CookieModel cookieModel = new CookieModel();
            cookieModel.Modificada = true;
            cookieModel.ProyectoCookie = cookie;            
            cookieModel.ListaCategoriaProyectoCookie = new List<CategoriaProyectoCookie>();
            cookieModel.ListaCategoriaProyectoCookie.Add(new CategoriaProyectoCookie() { Nombre = categoria[1], CategoriaID = cookie.CategoriaID, Descripcion = "", ProyectoID = cookie.ProyectoID });

            return PartialView("_EdicionCookieProyecto", cookieModel);
        }

        /// <summary>
        /// Se encarga de pintar las cookies analíticas
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PintarCookiesAnaliticas()
        {
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (!cookieCN.ExistenCookiesAnaliticas())
            {
                CategoriaProyectoCookie categoriaGoogleAnalytics = cookieCN.ObtenerCategoriaPorNombreCorto("Analiticas", ProyectoSeleccionado.Clave);
                if (categoriaGoogleAnalytics == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    categoriaGoogleAnalytics = new CategoriaProyectoCookie();
                    categoriaGoogleAnalytics.ProyectoID = ProyectoSeleccionado.Clave;
                    categoriaGoogleAnalytics.OrganizacionID = proyectoCN.ObtenerOrganizacionIDAPartirDeProyectoID(categoriaGoogleAnalytics.ProyectoID);
                    categoriaGoogleAnalytics.CategoriaID = Guid.NewGuid();
                    categoriaGoogleAnalytics.NombreCorto = "Analiticas";
                    categoriaGoogleAnalytics.Nombre = $"{utilIdiomasEspanyol.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@es|||{utilIdiomasIngles.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@en|||{utilIdiomasPortugues.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@pt|||{utilIdiomasCatalan.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@ca|||{utilIdiomasEuskera.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@eu|||{utilIdiomasGallego.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@gl|||{utilIdiomasFrances.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@fr|||{utilIdiomasAleman.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@de|||{utilIdiomasItaliano.GetText("MENSAJECOOKIES", "NOMBRECOOKIESANALITICAS")}@it";
                    categoriaGoogleAnalytics.Descripcion = $"{utilIdiomasEspanyol.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@es|||{utilIdiomasIngles.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@en|||{utilIdiomasPortugues.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@pt|||{utilIdiomasCatalan.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@ca|||{utilIdiomasEuskera.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@eu|||{utilIdiomasGallego.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@gl|||{utilIdiomasFrances.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@fr|||{utilIdiomasAleman.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@de|||{utilIdiomasItaliano.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESANALITICAS")}@it";
                    mEntityContext.CategoriaProyectoCookie.Add(categoriaGoogleAnalytics);
                    mEntityContext.SaveChanges();
                }
                List<CookieModel> listaCookiesAnaliticas = GenerarCookiesAnaliticas(categoriaGoogleAnalytics.CategoriaID);

                return PartialView("_ListaEdicionCookieProyecto", listaCookiesAnaliticas);
            }
            else
            {
                GuardarLogError("Ya existen las cookies analíticas");
                throw new Exception("Ya existen las cookies analíticas");
            }
        }

        /// <summary>
        /// Se encarga de pintar las cookies de youtube (redes sociales)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PintarCookiesYoutube()
        {
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (!cookieCN.ExistenCookiesYoutube())
            {
                CategoriaProyectoCookie categoriaRedesSociales = cookieCN.ObtenerCategoriaPorNombreCorto("Redes sociales", ProyectoSeleccionado.Clave);
                if (categoriaRedesSociales == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    categoriaRedesSociales = new CategoriaProyectoCookie();
                    categoriaRedesSociales.ProyectoID = ProyectoSeleccionado.Clave;
                    categoriaRedesSociales.OrganizacionID = proyectoCN.ObtenerOrganizacionIDAPartirDeProyectoID(categoriaRedesSociales.ProyectoID);
                    categoriaRedesSociales.CategoriaID = Guid.NewGuid();
                    categoriaRedesSociales.NombreCorto = "Redes sociales";
                    categoriaRedesSociales.Nombre = $"{utilIdiomasEspanyol.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@es|||{utilIdiomasIngles.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@en|||{utilIdiomasPortugues.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@pt|||{utilIdiomasCatalan.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@ca|||{utilIdiomasEuskera.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@eu|||{utilIdiomasGallego.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@gl|||{utilIdiomasFrances.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@fr|||{utilIdiomasAleman.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@de|||{utilIdiomasItaliano.GetText("MENSAJECOOKIES", "NOMBRECOOKIESREDESSOCIALES")}@it";
                    categoriaRedesSociales.Descripcion = $"{utilIdiomasEspanyol.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@es|||{utilIdiomasIngles.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@en|||{utilIdiomasPortugues.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@pt|||{utilIdiomasCatalan.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@ca|||{utilIdiomasEuskera.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@eu|||{utilIdiomasGallego.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@gl|||{utilIdiomasFrances.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@fr|||{utilIdiomasAleman.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@de|||{utilIdiomasItaliano.GetText("MENSAJECOOKIES", "DESCRIPCIONCOOKIESREDESSOCIALES")}@it";

                    mEntityContext.CategoriaProyectoCookie.Add(categoriaRedesSociales);
                    mEntityContext.SaveChanges();
                }
                List<CookieModel> listaCookiesAnaliticas = GenerarCookiesYoutube(categoriaRedesSociales.CategoriaID);

                return PartialView("_ListaEdicionCookieProyecto", listaCookiesAnaliticas);
            }
            else
            {
                GuardarLogError("Ya existen las cookies de Youtube");
                throw new Exception("Ya existen las cookies de Youtube");
            }
        }

        /// <summary>
        /// Método que sirve para guardar las cookies
        /// </summary>
        /// <param name="ListaCookies"></param>
        [HttpPost]
        public void SaveCookies(CookieEditModel[] ListaCookies)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<CookiesModel> lista = new List<CookiesModel>();

            foreach (CookieEditModel cookieProyecto in ListaCookies)
            {
                CookiesModel cookieModel = new CookiesModel();

                bool esEliminada = false;
                if (cookieProyecto.Deleted != null && bool.Parse(cookieProyecto.Deleted))
                {
                    esEliminada = bool.Parse(cookieProyecto.Deleted);
                }

                bool esEditada = false;
                if (cookieProyecto.EsModificada != null && bool.Parse(cookieProyecto.EsModificada))
                {
                    esEditada = bool.Parse(cookieProyecto.EsModificada);
                }

                cookieModel.EsModificada = esEditada;
                cookieModel.Deleted = esEliminada;
                ProyectoCookie proyectoCookie = cookieCN.ObtenerCookiePorId(new Guid(cookieProyecto.CookieID), ProyectoSeleccionado.Clave);

                if (proyectoCookie == null)
                {
                    CategoriaProyectoCookie categoriaProyectoCookie = ObtenerCategoriaDeCookie(new Guid(cookieProyecto.Categoria));
                    proyectoCookie = new ProyectoCookie();
                    proyectoCookie.CategoriaID = categoriaProyectoCookie.CategoriaID;
                    proyectoCookie.CookieID = Guid.NewGuid();
                    proyectoCookie.Descripcion = cookieProyecto.Descripcion;
                    proyectoCookie.Nombre = cookieProyecto.Nombre;
                    proyectoCookie.ProyectoID = ProyectoSeleccionado.Clave;
                    proyectoCookie.EsEditable = !EsCookieTecnica(cookieProyecto.Nombre);
                    proyectoCookie.OrganizacionID = proyectoCN.ObtenerOrganizacionIDAPartirDeProyectoID(proyectoCookie.ProyectoID);
                    proyectoCookie.Tipo = ObtenerTipoPorNombre(cookieProyecto.Tipo);
                    mEntityContext.ProyectoCookie.Add(proyectoCookie);
                }
                else if (esEditada)
                {
                    proyectoCookie.CategoriaID = new Guid(cookieProyecto.Categoria);
                    proyectoCookie.Descripcion = cookieProyecto.Descripcion;
                    proyectoCookie.Nombre = cookieProyecto.Nombre;
                }
                else if (esEliminada)
                {
                    mEntityContext.ProyectoCookie.Remove(proyectoCookie);
                }

                if (proyectoCookie != null)
                {
                    CategoriaProyectoCookie categoriaProyectoCookie = ObtenerCategoriaDeCookie(proyectoCookie.CategoriaID);
                    CategoriaCookiesModel categoria = new CategoriaCookiesModel();
                    categoria.CategoriaID = categoriaProyectoCookie.CategoriaID;
                    categoria.Descripcion = categoriaProyectoCookie.Descripcion;
                    categoria.Nombre = categoriaProyectoCookie.Nombre;
                    categoria.NombreCorto = categoriaProyectoCookie.NombreCorto;

                    cookieModel.Categoria = categoria;
                    cookieModel.CookieID = proyectoCookie.CookieID;
                    cookieModel.Descripcion = proyectoCookie.Descripcion;
                    cookieModel.Nombre = proyectoCookie.Nombre;
                    cookieModel.Tipo = proyectoCookie.Tipo;
                    lista.Add(cookieModel);
                }
            }
            mEntityContext.SaveChanges();

            /*Incluir cookies en integración continua*/
            bool iniciado = false;
            try
            {
                iniciado = HayIntegracionContinua;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
            }
            if (iniciado)
            {
                List<CookiesModel> listaCookiesSinTecnicas = OmitirCookiesTecnicas(lista);
                HttpResponseMessage resultado = InformarCambioAdministracion("Cookies", JsonConvert.SerializeObject(listaCookiesSinTecnicas, Formatting.Indented));
                if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                {
                    throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                }
            }
        }

        /// <summary>
        /// Método que se utiliza para guardar las categorías en base de datos
        /// </summary>
        /// <param name="ListaCategorias">Lista de categorías a guardar en base de datos</param>
        [HttpPost]
        public void SaveCategories(CategoryEditModel[] ListaCategorias)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            foreach (CategoryEditModel categoriaCookie in ListaCategorias)
            {
                bool esEliminada = false;
                if (categoriaCookie.Deleted != null && bool.Parse(categoriaCookie.Deleted))
                {
                    esEliminada = bool.Parse(categoriaCookie.Deleted);
                }

                bool esEditada = false;
                if (categoriaCookie.EsModificada != null && bool.Parse(categoriaCookie.EsModificada))
                {
                    esEditada = bool.Parse(categoriaCookie.EsModificada);
                }

                CategoriaProyectoCookie categoriaProyectoCookie = cookieCN.ObtenerCategoriaPorId(new Guid(categoriaCookie.CategoriaID), ProyectoSeleccionado.Clave);
                if (categoriaProyectoCookie == null)
                {
                    categoriaProyectoCookie = new CategoriaProyectoCookie();
                    categoriaProyectoCookie.Descripcion = categoriaCookie.Descripcion;
                    categoriaProyectoCookie.Nombre = categoriaCookie.Nombre;
                    categoriaProyectoCookie.NombreCorto = categoriaCookie.NombreCorto;
                    categoriaProyectoCookie.CategoriaID = Guid.NewGuid();
                    categoriaProyectoCookie.EsCategoriaTecnica = false;
                    categoriaProyectoCookie.ProyectoID = ProyectoSeleccionado.Clave;
                    categoriaProyectoCookie.EsCategoriaTecnica = categoriaProyectoCookie.NombreCorto.Equals("Tecnica");
                    categoriaProyectoCookie.OrganizacionID = proyectoCN.ObtenerOrganizacionIDAPartirDeProyectoID(categoriaProyectoCookie.ProyectoID);
                    mEntityContext.CategoriaProyectoCookie.Add(categoriaProyectoCookie);
                }
                else if (esEditada)
                {
                    categoriaProyectoCookie.Descripcion = categoriaCookie.Descripcion;
                    categoriaProyectoCookie.Nombre = categoriaCookie.Nombre;
                    categoriaProyectoCookie.NombreCorto = categoriaCookie.NombreCorto;
                }
                else if (esEliminada)
                {
                    if (!cookieCN.HayCookiesVinculadas(categoriaProyectoCookie.CategoriaID))
                    {
                        mEntityContext.CategoriaProyectoCookie.Remove(categoriaProyectoCookie);
                    }
                    else
                    {
                        string mensajeError = $"No se puede eliminar una categoría que tiene cookies vinculadas\n\tCategoriaID:{categoriaProyectoCookie.CategoriaID}";
                        GuardarLogError(mensajeError);
                        throw new Exception(mensajeError);
                    }
                }
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Añade una categoría para las cookies a la vista
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCategory()
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            CategoriaProyectoCookie categoriaProyectoCookie = new CategoriaProyectoCookie();
            categoriaProyectoCookie.CategoriaID = Guid.NewGuid();
            categoriaProyectoCookie.Descripcion = "Descripcion";
            categoriaProyectoCookie.EsCategoriaTecnica = false;
            categoriaProyectoCookie.Nombre = "Nombre";
            categoriaProyectoCookie.NombreCorto = "";
            categoriaProyectoCookie.ProyectoID = ProyectoSeleccionado.Clave;
            categoriaProyectoCookie.OrganizacionID = proyectoCN.ObtenerOrganizacionIDAPartirDeProyectoID(categoriaProyectoCookie.ProyectoID);

            CategoriaCookieModel categoriaCookieModel = new CategoriaCookieModel();
            categoriaCookieModel.CategoriaProyectoCookie = categoriaProyectoCookie;
            categoriaCookieModel.Modificada = true;

            return PartialView("_EdicionCategoriaCookie", categoriaCookieModel);
        }

        /// <summary>
        /// Genera las cookies analíticas 
        /// </summary>
        /// <param name="pCategoriaID">Ídentificador de la categoría a la que serán vinculadas las cookies generadas</param>
        /// <returns></returns>
        private List<CookieModel> GenerarCookiesAnaliticas(Guid pCategoriaID)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Guid organizacionID = proyectoCN.ObtenerOrganizacionIDAPartirDeProyectoID(ProyectoSeleccionado.Clave);
            List<CategoriaProyectoCookie> listaCategoriasCookie = cookieCN.ObtenerCategoriasProyectoCookie(ProyectoSeleccionado.Clave);

            List<CookieModel> listaCookiesAnaliticas = new List<CookieModel>();

            ProyectoCookie cookieGa = new ProyectoCookie() { Nombre = "_ga", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Distinguir a los usuarios para análisis de uso: Habilita la función de control de visitas únicas. La primera vez que un usuario entre en la web se instalará esta cookie. Cuando este usuario vuelva a entrar con el mismo navegador la cookie considerará que es el mismo usuario.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelGa = new CookieModel() { ProyectoCookie = cookieGa, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesAnaliticas.Add(cookieModelGa);

            ProyectoCookie cookieUtma = new ProyectoCookie() { Nombre = "__utma", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Distinguir a los usuarios para análisis de uso: Habilita la función de control de visitas únicas. La primera vez que un usuario entre en la web se instalará esta cookie. Cuando este usuario vuelva a entrar con el mismo navegador la cookie considerará que es el mismo usuario.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelUtma = new CookieModel() { ProyectoCookie = cookieUtma, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesAnaliticas.Add(cookieModelUtma);

            ProyectoCookie cookieUtmt = new ProyectoCookie() { Nombre = "__utmt", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Se usa para limitar el porcentaje de solicitudes.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelUtmt = new CookieModel() { ProyectoCookie = cookieUtmt, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesAnaliticas.Add(cookieModelUtmt);

            ProyectoCookie cookieUtmb = new ProyectoCookie() { Nombre = "__utmb", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Se usa para determinar nuevas sesiones o visitas. La cookie se crea cuando se ejecuta la biblioteca de JavaScript y no hay ninguna cookie __utmb. La cookie se actualiza cada vez que se envían datos a Google Analytics.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelUtmb = new CookieModel() { ProyectoCookie = cookieUtmb, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesAnaliticas.Add(cookieModelUtmb);

            ProyectoCookie cookieUtmc = new ProyectoCookie() { Nombre = "__utmc", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Se usa para conocer el tiempo de navegación. Comprueba si se debe mantener la sesión abierta o se debe crear una sesión nueva. Se elimina automáticamente al cambiar de web o al cerrar el navegador.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelUtmc = new CookieModel() { ProyectoCookie = cookieUtmc, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesAnaliticas.Add(cookieModelUtmc);

            ProyectoCookie cookieUtmv = new ProyectoCookie() { Nombre = "__utmv", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Se usa para almacenar datos de variables personalizadas a nivel de visitante.  La cookie se actualiza cada vez que se envían datos a Google Analytics.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelUtmv = new CookieModel() { ProyectoCookie = cookieUtmv, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesAnaliticas.Add(cookieModelUtmv);

            ProyectoCookie cookieUtmz = new ProyectoCookie() { Nombre = "__utmz", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Almacena la fuente de tráfico o la campaña que explica cómo ha llegado el usuario al sitio web. La cookie se crea cuando se ejecuta la biblioteca de JavaScript y se actualiza cada vez que se envían datos a Google Analytics.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelUtmz = new CookieModel() { ProyectoCookie = cookieUtmz, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesAnaliticas.Add(cookieModelUtmz);

            return listaCookiesAnaliticas;
        }

        /// <summary>
        /// Genera las cookies de youtube (pertenecen a redes sociales)
        /// </summary>
        /// <param name="pCategoriaID">Identificador de la categoría de redes sociales a la que se vinculará</param>
        /// <returns></returns>
        private List<CookieModel> GenerarCookiesYoutube(Guid pCategoriaID)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Guid organizacionID = proyectoCN.ObtenerOrganizacionIDAPartirDeProyectoID(ProyectoSeleccionado.Clave);
            List<CategoriaProyectoCookie> listaCategoriasCookie = cookieCN.ObtenerCategoriasProyectoCookie(ProyectoSeleccionado.Clave);

            List<CookieModel> listaCookiesYoutube = new List<CookieModel>();

            ProyectoCookie cookieNid = new ProyectoCookie() { Nombre = "NID", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Esta cookie contiene un ID único que Google utiliza para recordar tus preferencias y otra información, como tu idioma preferido, el número de resultados de búsqueda que quieres que se muestren por página (por ejemplo, 10 o 20) y si quieres que el filtro Búsqueda Segura de Google esté activado o desactivado. La cookie \"NID\" caduca 6 meses después de su último uso.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelNid = new CookieModel() { ProyectoCookie = cookieNid, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesYoutube.Add(cookieModelNid);

            ProyectoCookie cookieVisitor = new ProyectoCookie() { Nombre = "VISITOR_INFO1_LIVE", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Se utiliza para detectar y resolver problemas con el servicio. Habilita las recomendaciones personalizadas en YouTube, que están basadas en las visualizaciones y búsquedas que haya hecho el usuario.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelVisitor = new CookieModel() { ProyectoCookie = cookieVisitor, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesYoutube.Add(cookieModelVisitor);

            ProyectoCookie cookiePref = new ProyectoCookie() { Nombre = "PREF", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Para almacenar información como la configuración de página o las preferencias de reproducción del usuario (por ejemplo, la reproducción automática, la reproducción aleatoria de contenido y el tamaño del reproductor). En YouTube Music, estas preferencias incluyen el volumen, el modo de repetición y la reproducción automática. Esta cookie caduca 8 meses después de su último uso.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelPref = new CookieModel() { ProyectoCookie = cookiePref, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesYoutube.Add(cookieModelPref);

            ProyectoCookie cookieYsc = new ProyectoCookie() { Nombre = "YSC", NombreCorto = "", CategoriaID = pCategoriaID, CookieID = Guid.NewGuid(), Descripcion = "Se utiliza en YouTube para recordar lo que introducen los usuarios y asociar sus acciones. Esta cookie dura mientras los usuarios mantienen el navegador abierto.", EsEditable = true, ProyectoID = ProyectoSeleccionado.Clave, OrganizacionID = organizacionID, Tipo = (short)TipoCookies.Third_Party };
            CookieModel cookieModelYsc = new CookieModel() { ProyectoCookie = cookieYsc, Modificada = true, ListaCategoriaProyectoCookie = listaCategoriasCookie };
            listaCookiesYoutube.Add(cookieModelYsc);

            return listaCookiesYoutube;
        }

        /// <summary>
        /// Nos indica si la cookie pasada por parámetro es una cookie Técnica
        /// </summary>
        /// <param name="pNombreCookie">Nombre de la cookie</param>
        /// <returns></returns>
        private bool EsCookieTecnica(string pNombreCookie)
        {
            string[] listaCookiesTecnicas = new string[] { "IdiomaActual", "cookieAviso.gnoss.com", "ASP.NET_SessionId", "UsuarioLogueado", "SesionUsuarioActiva" };

            return listaCookiesTecnicas.Contains(pNombreCookie);
        }

        /// <summary>
        /// Obtriene el tipo de cookie en función del nombre
        /// </summary>
        /// <param name="pNombreTipo">Nombre del tipo</param>
        /// <returns></returns>
        private short ObtenerTipoPorNombre(string pNombreTipo)
        {
            switch (pNombreTipo)
            {
                case "Persistent":
                    return 0;
                case "Session":
                    return 1;
                case "Third party":
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Obtiene la categoría completa de base de datos con su identificador
        /// </summary>
        /// <param name="pCategoriaID">Identificador de la categoría a obtener</param>
        /// <returns></returns>
        private CategoriaProyectoCookie ObtenerCategoriaDeCookie(Guid pCategoriaID)
        {
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            CategoriaProyectoCookie categoria = cookieCN.ObtenerCategoriaPorId(pCategoriaID, ProyectoSeleccionado.Clave);
            if (categoria == null)
            {
                categoria = cookieCN.ObtenerCategoriaPorId(pCategoriaID, ProyectoAD.MetaProyecto);

                categoria = cookieCN.ObtenerCategoriaPorNombreCorto(categoria.NombreCorto, ProyectoSeleccionado.Clave);
            }

            return categoria;
        }

        /// <summary>
        /// Devuelve la lista obtenida por parámetro eliminando las cookies técnicas
        /// </summary>
        /// <param name="pListaCookies">Lista de la cual queremos eliminar las cookies técnicas</param>
        /// <returns></returns>
        private List<CookiesModel> OmitirCookiesTecnicas(List<CookiesModel> pListaCookies)
        {
            List<CookiesModel> listaCookiesSinTecnicas = new List<CookiesModel>();
            foreach (CookiesModel cookie in pListaCookies)
            {
                if (!EsCookieTecnica(cookie.Nombre))
                {
                    listaCookiesSinTecnicas.Add(cookie);
                }
            }

            return listaCookiesSinTecnicas;
        }

        /// <summary>
        /// Modelo utilizado en la vista de la página de administración de cookies
        /// </summary>
        private AdministrarCookiesViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarCookiesViewModel();

                    if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma))
                    {
                        mPaginaModel.ContenidoMultiIdioma = true;
                    }

                    if (ParametrosGeneralesRow.IdiomasDisponibles)
                    {
                        mPaginaModel.ListaIdiomas = mConfigService.ObtenerListaIdiomasDictionary();
                    }
                    else
                    {
                        mPaginaModel.ListaIdiomas = new Dictionary<string, string>();
                        mPaginaModel.ListaIdiomas.Add(IdiomaPorDefecto, mConfigService.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
                    }

                    mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;
                    mPaginaModel.ListaProyectoCookie = new List<CookieModel>();
                    mPaginaModel.ListaCategoriaProyectoCookie = new List<CategoriaCookieModel>();

                    CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<CategoriaProyectoCookieViewModel> listaCategoriaProyectoCookieViewModel = new List<CategoriaProyectoCookieViewModel>();
                    List<CategoriaProyectoCookie> listaCategoriaProyectoCookie = cookieCN.ObtenerCategoriasProyectoCookie(ProyectoSeleccionado.Clave);
                    List<ProyectoCookie> listaProyectoCookie = cookieCN.ObtenerCookiesDeProyecto(ProyectoSeleccionado.Clave);
                    foreach (ProyectoCookie proyectoCookie in listaProyectoCookie)
                    {
                        mPaginaModel.ListaProyectoCookie.Add(new CookieModel { ProyectoCookie = proyectoCookie, ListaCategoriaProyectoCookie = listaCategoriaProyectoCookie, Modificada = false });
                    }
                    foreach (CategoriaProyectoCookie categoriaProyectoCookie in listaCategoriaProyectoCookie)
                    {
                        bool tieneCookiesVinculadas = cookieCN.TieneCategoriaCookiesVinculadas(categoriaProyectoCookie.CategoriaID);
                        mPaginaModel.ListaCategoriaProyectoCookie.Add(new CategoriaCookieModel { CategoriaProyectoCookie = categoriaProyectoCookie, Modificada = false, CookiesVinculadas = tieneCookiesVinculadas });
                    }
                }
                return mPaginaModel;
            }
        }
    }
}