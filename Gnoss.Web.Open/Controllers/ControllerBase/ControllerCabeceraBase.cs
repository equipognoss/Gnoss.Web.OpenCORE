using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Es.Riam.Web.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public partial class ControllerCabeceraBase
    {
        private ControllerBaseWeb mControllerBase;

        private dynamic ViewBag;

        private Identidad IdentidadActual;
        private Proyecto ProyectoSeleccionado;
        private Proyecto ProyectoVirtual;

        private UtilIdiomas UtilIdiomas;
        private string BaseURLIdioma;
        private string UrlPerfil;
        private HttpRequest Request;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        protected ControladorBase mControladorBase;
        protected EntityContext mEntityContext;

        public ControllerCabeceraBase(ControllerBaseWeb pControllerBase, IHttpContextAccessor httpContextAccessor, EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache)
        {
            mControllerBase = pControllerBase;
            mHttpContextAccessor = httpContextAccessor;

            ViewBag = pControllerBase.ViewBag;
            mConfigService = configService;
            IdentidadActual = mControllerBase.IdentidadActual;
            ProyectoSeleccionado = mControllerBase.ProyectoSeleccionado;
            ProyectoVirtual = mControllerBase.ProyectoVirtual;
            //UtilIdiomasCache = mControllerBase.UtilIdiomasCache;
            UtilIdiomas = mControllerBase.UtilIdiomas;
            BaseURLIdioma = mControllerBase.BaseURLIdioma;
            UrlPerfil = mControllerBase.UrlPerfil;
            Request = mControllerBase.Request;

            mEntityContext = entityContext;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null);
        }

        private string RequestParams(string pParametro)
        {
            return mControllerBase.RequestParams(pParametro);
        }

        public void CargarDatosCabecera()
        {
            //ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD);

            //HeaderModel cabecera = (HeaderModel)proyCL.ObtenerCabeceraMVC(ProyectoVirtual.Clave);

            //if (cabecera == null)
            //{
            HeaderModel cabecera = new HeaderModel();

            cabecera.UrlAdvancedSearch = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, mControllerBase.BaseURLIdioma, ProyectoVirtual.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");

            cabecera.Languajes = mConfigService.ObtenerListaIdiomasDictionary();

            cabecera.Searcher = CargarMetabuscador();

            cabecera.HomeUserAvailable = !mControllerBase.EsEcosistemaSinMetaProyecto;

            cabecera.SubscriptionsAvailable = !mControllerBase.EsEcosistemaSinBandejaSuscripciones;

            cabecera.ContactsAvailable = !mControllerBase.EsEcosistemaSinContactos;

            cabecera.ChangePasswordVisible = !(mControllerBase.ParametroProyecto.ContainsKey(ParametroAD.OcultarCambiarPassword) && mControllerBase.ParametroProyecto[ParametroAD.OcultarCambiarPassword].ToLower().Equals("true"));
            //    proyCL.AgregarCabeceraMVC(ProyectoVirtual.Clave, cabecera);
            //}

            ProcesarPrivacidadPestanyasMetabuscadorComunidad(cabecera.Searcher);

            cabecera.SocialNetworkRegister = new Dictionary<string, string>();
            if (mControllerBase.ObtenerParametrosLoginExterno(TipoRedSocialLogin.Facebook, mControllerBase.ParametroProyecto, mControllerBase.ParametrosAplicacionDS).Count > 0)
            {
                string urlServicioLoginFacebook = mControladorBase.UrlServicioLogin + "/loginfacebook?token=" + System.Net.WebUtility.UrlEncode(mControllerBase.TokenLoginUsuario) + "&proyectoID=" + ProyectoSeleccionado.Clave.ToString() + "&urlOrigen=" + System.Net.WebUtility.UrlEncode(mControllerBase.BaseURL + mHttpContextAccessor.HttpContext.Request.Path);

                urlServicioLoginFacebook = AgregarEventoComunidad(urlServicioLoginFacebook);

                cabecera.SocialNetworkRegister.Add("Facebook", urlServicioLoginFacebook);
            }

            if (mControllerBase.ObtenerParametrosLoginExterno(TipoRedSocialLogin.Google, mControllerBase.ParametroProyecto, mControllerBase.ParametrosAplicacionDS).Count > 0)
            {
                string urlServicioLoginGoogle = mControladorBase.UrlServicioLogin + "/logingoogle?token=" + System.Net.WebUtility.UrlEncode(mControllerBase.TokenLoginUsuario) + "&proyectoID=" + ProyectoSeleccionado.Clave.ToString() + "&urlOrigen=" + System.Net.WebUtility.UrlEncode(mControllerBase.BaseURL + mHttpContextAccessor.HttpContext.Request.Path);

                urlServicioLoginGoogle = AgregarEventoComunidad(urlServicioLoginGoogle);

                cabecera.SocialNetworkRegister.Add("Google", urlServicioLoginGoogle);
            }

            if (mControllerBase.ObtenerParametrosLoginExterno(TipoRedSocialLogin.Twitter, mControllerBase.ParametroProyecto, mControllerBase.ParametrosAplicacionDS).Count > 0)
            {
                string urlServicioLoginTwitter = mControladorBase.UrlServicioLogin + "/logintwitter.aspx?token=" + System.Net.WebUtility.UrlEncode(mControllerBase.TokenLoginUsuario) + "&proyectoID=" + ProyectoSeleccionado.Clave.ToString() + "&urlOrigen=" + System.Net.WebUtility.UrlEncode(mControllerBase.BaseURL + mHttpContextAccessor.HttpContext.Request.Path);

                urlServicioLoginTwitter = AgregarEventoComunidad(urlServicioLoginTwitter);

                cabecera.SocialNetworkRegister.Add("Twitter", urlServicioLoginTwitter);
            }
            if (mControllerBase.ObtenerParametrosLoginExterno(TipoRedSocialLogin.Santillana, mControllerBase.ParametroProyecto, mControllerBase.ParametrosAplicacionDS).Count > 0)
            {
                string urlServicioLoginSantillana = mControladorBase.UrlServicioLogin + "/loginSantillana.aspx?token=" + System.Net.WebUtility.UrlEncode(mControllerBase.TokenLoginUsuario) + "&proyectoID=" + ProyectoSeleccionado.Clave.ToString() + "&urlOrigen=" + System.Net.WebUtility.UrlEncode(mControllerBase.BaseURL + mHttpContextAccessor.HttpContext.Request.Path);

                urlServicioLoginSantillana = AgregarEventoComunidad(urlServicioLoginSantillana);

                cabecera.SocialNetworkRegister.Add("Santillana", urlServicioLoginSantillana);
            }

            ProcesarCombosBusquedas(cabecera.Searcher.ListSelectCombo, UtilIdiomas, mControllerBase.ParametrosGeneralesRow.IdiomaDefecto);

            if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                #region Agregamos al ddl de busqueda opciones en funcion de la peticion
                string pSearch = "?search=";
                string textoBusquedaEspacioPersonal = UtilIdiomas.GetText("COMENTARIOS", "MIESPACIOPERSONAL");
                if (RequestParams("perfilorg") != null && RequestParams("perfilorg").Equals("true"))
                {
                    if (!string.IsNullOrEmpty(RequestParams("organizacion")) && IdentidadActual.IdentidadOrganizacion != null)
                    {
                        //administrador organizacion
                        textoBusquedaEspacioPersonal = UtilIdiomas.GetText("PERFIL", "ESPACIOCORPORATIVODE", IdentidadActual.OrganizacionPerfil.Nombre);
                    }
                    else
                    {
                        textoBusquedaEspacioPersonal = UtilCadenas.ObtenerTextoDeIdioma(mControllerBase.EspacioPersonal, UtilIdiomas.LanguageCode, null);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(mControllerBase.EspacioPersonal))
                    {
                        textoBusquedaEspacioPersonal = UtilCadenas.ObtenerTextoDeIdioma(mControllerBase.EspacioPersonal, UtilIdiomas.LanguageCode, null);
                    }
                }

                if (RequestParams("EditarRecursosPerfil") != null && RequestParams("EditarRecursosPerfil").Equals("true"))
                {
                    HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModel = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                    selectComboModel.Name = textoBusquedaEspacioPersonal;
                    selectComboModel.ID = FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES;
                    selectComboModel.Url = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MISRECURSOS") + pSearch;
                    selectComboModel.Autocomplete = FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES;
                    selectComboModel.Selected = true;
                    cabecera.Searcher.ListSelectCombo.Add(selectComboModel);
                }
                else if (RequestParams("perfilorg") != null && RequestParams("perfilorg").Equals("true"))
                {
                    HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModel = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                    selectComboModel.Name = UtilIdiomas.GetText("COMENTARIOS", "ESPACIOCORPORATIVODE", IdentidadActual.OrganizacionPerfil.Nombre);
                    selectComboModel.ID = FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES;
                    selectComboModel.Url = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MISRECURSOS") + pSearch;
                    selectComboModel.Autocomplete = FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES;
                    selectComboModel.Selected = true;
                    cabecera.Searcher.ListSelectCombo.Add(selectComboModel);
                }
                else if (RequestParams("mensajes") != null && RequestParams("mensajes").Equals("true"))
                {
                    HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModel = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                    selectComboModel.ID = FacetadoAD.BUSQUEDA_MENSAJES;
                    selectComboModel.Name = UtilIdiomas.GetText("MENU", "MENSAJES");
                    selectComboModel.Url = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MENSAJES") + pSearch;
                    selectComboModel.Autocomplete = FacetadoAD.BUSQUEDA_MENSAJES;
                    selectComboModel.Selected = true;
                    cabecera.Searcher.ListSelectCombo.Add(selectComboModel);
                }
                if (RequestParams("contribuciones") != null && RequestParams("contribuciones").Equals("true"))
                {
                    string textoBusqueda = UtilIdiomas.GetText("PERFIL", "MISCONTRIBUCIONES");
                    if (IdentidadActual.OrganizacionID.HasValue)
                    {
                        //administrador organizacion
                        textoBusqueda = UtilIdiomas.GetText("PERFIL", "CONTRIBUCIONESDE", IdentidadActual.OrganizacionPerfil.Nombre);
                    }
                    HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModel = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                    selectComboModel.ID = FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS;
                    selectComboModel.Name = textoBusqueda;
                    selectComboModel.Url = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MISCONTRIBUCIONES") + pSearch;
                    selectComboModel.Autocomplete = FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS;
                    selectComboModel.Selected = true;
                    cabecera.Searcher.ListSelectCombo.Add(selectComboModel);
                }
                if (RequestParams("contactos") != null && RequestParams("contactos").Equals("true"))
                {
                    HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModel = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                    selectComboModel.ID = FacetadoAD.BUSQUEDA_CONTACTOS;
                    selectComboModel.Name = UtilIdiomas.GetText("COMMON", "CONTACTOS");
                    selectComboModel.Autocomplete = FacetadoAD.BUSQUEDA_CONTACTOS;
                    selectComboModel.Selected = true;
                    if (RequestParams("organizacion") != null && IdentidadActual.IdentidadOrganizacion != null)
                    {
                        selectComboModel.Url = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "CONTACTOSORG") + pSearch;
                    }
                    else
                    {
                        selectComboModel.Url = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "CONTACTOS") + pSearch;
                    }
                    cabecera.Searcher.ListSelectCombo.Add(selectComboModel);
                }
                #endregion

                #region Seleccionamos el item del ddl seleccionado

                string pagActual = "";
                if (Request != null)
                {
                    pagActual = Request.Path.ToString().Substring(Request.Path.ToString().LastIndexOf("/") + 1);
                }
                if (pagActual.Equals("admin"))
                {
                    string path = Request.Path.ToString().Remove(Request.Path.ToString().LastIndexOf("/"));//quito el /admin
                    pagActual = path.Substring(path.LastIndexOf("/") + 1);
                }


                foreach (HeaderModel.SearchHeaderModel.SearchSelectComboModel combo in cabecera.Searcher.ListSelectCombo)
                {
                    if (pagActual == UtilIdiomas.GetText("URLSEM", "BLOGS") && combo.Autocomplete == FacetadoAD.BUSQUEDA_BLOGS)
                    {
                        combo.Selected = true;
                        break;
                    }
                    if (pagActual == UtilIdiomas.GetText("URLSEM", "COMUNIDADES") && combo.Autocomplete == FacetadoAD.BUSQUEDA_COMUNIDADES)
                    {
                        combo.Selected = true;
                        break;
                    }
                    if (pagActual == UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES") && combo.Autocomplete == FacetadoAD.BUSQUEDA_PERSONASYORG)
                    {
                        combo.Selected = true;
                        break;
                    }
                }
                #endregion
            }

            Dictionary<string, string> dicParametrosAplicacion = new Dictionary<string, string>();
            // foreach(ParametroAplicacionDS.ParametroAplicacionRow filaParam in mControllerBase.ParametrosAplicacionDS.ParametroAplicacion.Rows)
            foreach (AD.EntityModel.ParametroAplicacion filaParam in mControllerBase.ParametrosAplicacionDS)
            {
                if (!dicParametrosAplicacion.ContainsKey(filaParam.Parametro))
                {
                    dicParametrosAplicacion.Add(filaParam.Parametro, filaParam.Valor);
                }
            }

            ViewBag.ParametrosAplicacion = dicParametrosAplicacion;

            //proyCL.Dispose();
            ViewBag.Cabecera = cabecera;
        }

        private string AgregarEventoComunidad(string pUrlServicioLogin)
        {
            if (!string.IsNullOrEmpty(RequestParams("eventoComID")))
            {
                pUrlServicioLogin += "&eventoComID=" + RequestParams("eventoComID");
            }

            return pUrlServicioLogin;
        }

        /// <summary>
        /// Carga un control lista con las categorías de primer nivel
        /// </summary>
        /// <param name="pListaCategorias">Control lista que vamos a cargar</param>
        public HeaderModel.SearchHeaderModel CargarMetabuscador()
        {
            HeaderModel.SearchHeaderModel buscador = new HeaderModel.SearchHeaderModel();
            buscador.ListSelectCombo = new List<HeaderModel.SearchHeaderModel.SearchSelectComboModel>();

            //ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService);
            //ParametroGeneralDS paramDS = paramCN.ObtenerParametrosGeneralesDeProyecto(ProyectoVirtual.Clave);
            //paramCN.Dispose();
            GestorParametroGeneral gestorParametroGeneral = new GestorParametroGeneral();
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
            gestorParametroGeneral = gestorController.ObtenerParametrosGeneralesDeProyecto(gestorParametroGeneral, ProyectoVirtual.Clave);
            bool metaBusquedaActiva = false;
            List<ConfiguracionAmbitoBusquedaProyecto> listaConfiguracionAmbitoBusqueda = gestorParametroGeneral.ListaConfiguracionAmbitoBusquedaProyecto.Where(configuracionAmbitoBusquedaProyecto => configuracionAmbitoBusquedaProyecto.ProyectoID.Equals(ProyectoVirtual.Clave)).ToList();
            if (((ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto) || mControllerBase.ParametrosGeneralesVirtualRow.ComunidadGNOSS) && !mControllerBase.EsEcosistemaSinMetaProyecto))
            {
                if (listaConfiguracionAmbitoBusqueda.Count > 0 && listaConfiguracionAmbitoBusqueda[0].TodoGnoss)
                {
                    //if (((ParametroGeneralDS.ConfiguracionAmbitoBusquedaProyectoRow[])gestorParametroGeneral.ConfiguracionAmbitoBusquedaProyecto.Select("ProyectoID = '" + ProyectoVirtual.Clave + "'"))[0].TodoGnoss)
                    // {
                    metaBusquedaActiva = true;
                    // }
                }
                else
                {
                    //Si no hay filas lo pintamos todo
                    metaBusquedaActiva = true;
                }
            }
            if (metaBusquedaActiva)
            {
                HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModel = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                selectComboModel.ID = "MyGNOSS" + FacetadoAD.BUSQUEDA_AVANZADA;
                selectComboModel.Name = UtilIdiomas.GetText("BUSCADORFACETADO", "TODOGNOSS", mControllerBase.NombreProyectoEcosistema);
                selectComboModel.Url = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                selectComboModel.Selected = ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto);
                buscador.ListSelectCombo.Add(selectComboModel);
            }

            if (ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                CargarMetabuscadorMetaProyecto(buscador.ListSelectCombo);
            }
            else
            {
                int numCombosCargados = buscador.ListSelectCombo.Count;
                CargarMetabuscadorComunidad(buscador.ListSelectCombo, gestorParametroGeneral, buscador);
                bool comboseleccionado = false;
                foreach (HeaderModel.SearchHeaderModel.SearchSelectComboModel combo in buscador.ListSelectCombo)
                {
                    if (combo.Selected)
                    {
                        comboseleccionado = true;
                        break;
                    }
                }
                if (!comboseleccionado && buscador.ListSelectCombo.Count > numCombosCargados)
                {
                    buscador.ListSelectCombo[numCombosCargados].Selected = true;
                }
            }

            //  paramDS.Dispose();


            return buscador;
        }

        public void CargarMetabuscadorMetaProyecto(List<HeaderModel.SearchHeaderModel.SearchSelectComboModel> pListaSelectCombo)
        {
            //Agrego blogs, comunidad y personas y organizaciones
            //Blogs
            HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelBlogs = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
            selectComboModelBlogs.ID = FacetadoAD.BUSQUEDA_BLOGS;
            selectComboModelBlogs.Name = UtilIdiomas.GetText("COMMON", "BLOGS");
            selectComboModelBlogs.Url = UtilIdiomas.GetText("URLSEM", "BLOGS");
            selectComboModelBlogs.Selected = false;
            pListaSelectCombo.Add(selectComboModelBlogs);

            //Comunidades
            HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelComunidades = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
            selectComboModelComunidades.ID = FacetadoAD.BUSQUEDA_COMUNIDADES;
            selectComboModelComunidades.Name = UtilIdiomas.GetText("COMMON", "COMUNIDADES");
            selectComboModelComunidades.Url = UtilIdiomas.GetText("URLSEM", "COMUNIDADES");
            selectComboModelComunidades.Selected = false;
            pListaSelectCombo.Add(selectComboModelComunidades);

            //Personas
            HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelPersonas = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
            selectComboModelPersonas.ID = FacetadoAD.BUSQUEDA_PERSONASYORG;
            selectComboModelPersonas.Autocomplete = Es.Riam.Gnoss.AD.BASE_BD.OrigenAutocompletar.PersyOrg;
            selectComboModelPersonas.Name = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES");
            string admin = "";
            if (mControllerBase.AdministradorQuiereVerTodasLasPersonas)
            {
                admin = "/admin";
            }
            selectComboModelPersonas.Url = UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES") + admin;
            selectComboModelPersonas.Selected = false;
            pListaSelectCombo.Add(selectComboModelPersonas);
        }

        public void CargarMetabuscadorComunidad(List<HeaderModel.SearchHeaderModel.SearchSelectComboModel> pListaSelectCombo, GestorParametroGeneral gestorParametroGeneral, HeaderModel.SearchHeaderModel pBuscador)
        {
            bool montarMetaBusquedaComunidad = false;
            Guid? pestanyaDefecto = null;
            List<ConfiguracionAmbitoBusquedaProyecto> confAmbitoBusquedaProy = gestorParametroGeneral.ListaConfiguracionAmbitoBusquedaProyecto.Where(configAmbitoBusquedaProy => configAmbitoBusquedaProy.ProyectoID.Equals(ProyectoVirtual.Clave)).ToList();
            if (confAmbitoBusquedaProy.Count > 0)
            {
                if (confAmbitoBusquedaProy[0].Metabusqueda)
                {
                    montarMetaBusquedaComunidad = true;
                }
                if (!(confAmbitoBusquedaProy[0].PestanyaDefectoID == null))
                {
                    pestanyaDefecto = confAmbitoBusquedaProy[0].PestanyaDefectoID;
                }
            }
            else
            {
                montarMetaBusquedaComunidad = true;
            }
            if (montarMetaBusquedaComunidad)
            {
                ProyectoPestanyaMenu pestanyaBusquedaAvanzada = ProyectoVirtual.ListaPestanyasMenu.Values.FirstOrDefault(p => p.TipoPestanya.Equals(TipoPestanyaMenu.BusquedaAvanzada));

                if (pestanyaBusquedaAvanzada != null && pestanyaBusquedaAvanzada.Activa && (pestanyaBusquedaAvanzada.FilaProyectoPestanyaBusqueda == null || pestanyaBusquedaAvanzada.FilaProyectoPestanyaBusqueda.MostrarEnComboBusqueda))
                {
                    string rutaPestanya = pestanyaBusquedaAvanzada.Ruta;
                    string nombrePestanya = pestanyaBusquedaAvanzada.Nombre;
                    if (string.IsNullOrEmpty(rutaPestanya))
                    {
                        rutaPestanya = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    }

                    HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModel = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                    selectComboModel.ID = pestanyaBusquedaAvanzada.Clave.ToString();
                    selectComboModel.Selected = !pestanyaDefecto.HasValue;
                    selectComboModel.Name = nombrePestanya;
                    if (string.IsNullOrEmpty(nombrePestanya))
                    {
                        selectComboModel.Name = UtilIdiomas.GetText("BUSCADORFACETADO", "TODALACOMUNIDAD");
                    }
                    selectComboModel.Url = rutaPestanya;

                    pListaSelectCombo.Add(selectComboModel);
                }
            }


            foreach (ProyectoPestanyaMenu pestanyaMenu in ProyectoVirtual.ListaPestanyasMenu.Values)
            {
                if (!pestanyaMenu.FilaProyectoPestanyaMenu.PestanyaPadreID.HasValue)
                {
                    CargarPestanyaMetabuscadorComunidad(pListaSelectCombo, gestorParametroGeneral, pBuscador, pestanyaMenu, pestanyaDefecto);
                }
            }
        }

        // private void CargarPestanyaMetabuscadorComunidad(List<HeaderModel.SearchHeaderModel.SearchSelectComboModel> pListaSelectCombo, ParametroGeneralDS pParamDS,  HeaderModel.SearchHeaderModel pBuscador, ProyectoPestanyaMenu pPestanyaMenu, Guid? pPestanyaDefecto)
        private void CargarPestanyaMetabuscadorComunidad(List<HeaderModel.SearchHeaderModel.SearchSelectComboModel> pListaSelectCombo, GestorParametroGeneral pParamDS, HeaderModel.SearchHeaderModel pBuscador, ProyectoPestanyaMenu pPestanyaMenu, Guid? pPestanyaDefecto)
        {
            string nombrePestanya = pPestanyaMenu.Nombre;
            string rutaPestanya = pPestanyaMenu.Ruta;
            string facetasAutocompletar = "";
            if (pPestanyaMenu.Activa && (pPestanyaMenu.FilaProyectoPestanyaBusqueda == null || pPestanyaMenu.FilaProyectoPestanyaBusqueda.MostrarEnComboBusqueda))
            {
                switch (pPestanyaMenu.TipoPestanya)
                {
                    case TipoPestanyaMenu.Recursos:
                        #region Recursos
                        HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelRecursos = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                        selectComboModelRecursos.ID = pPestanyaMenu.Clave.ToString();
                        selectComboModelRecursos.Autocomplete = Es.Riam.Gnoss.AD.BASE_BD.OrigenAutocompletar.Recursos;
                        selectComboModelRecursos.FacetAutocomplete = facetasAutocompletar;
                        selectComboModelRecursos.Name = nombrePestanya;
                        selectComboModelRecursos.Selected = pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave;
                        if (string.IsNullOrEmpty(selectComboModelRecursos.Name))
                        {
                            selectComboModelRecursos.Name = UtilIdiomas.GetText("COMMON", "BASERECURSOS");
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "RECURSOS");
                        }
                        selectComboModelRecursos.Url = rutaPestanya;
                        pListaSelectCombo.Add(selectComboModelRecursos);
                        #endregion
                        break;
                    case TipoPestanyaMenu.BusquedaSemantica:
                        #region Búsqueda
                        //Añadimos las busquedas de los recursos semánticos configurados
                        if (pPestanyaMenu.FilaProyectoPestanyaBusqueda != null)
                        {
                            string extraJsProyOrigen = "";
                            string filtroPest = pPestanyaMenu.FilaProyectoPestanyaBusqueda.CampoFiltro.Replace("rdf:type=", "");

                            HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelBusqueda = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                            selectComboModelBusqueda.ID = pPestanyaMenu.Clave.ToString();
                            selectComboModelBusqueda.Autocomplete = pPestanyaMenu.FilaProyectoPestanyaMenu.Ruta;
                            selectComboModelBusqueda.FacetAutocomplete = facetasAutocompletar;
                            selectComboModelBusqueda.Name = nombrePestanya;
                            selectComboModelBusqueda.Url = rutaPestanya;
                            selectComboModelBusqueda.Selected = pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave;
                            pListaSelectCombo.Add(selectComboModelBusqueda);

                            if (mControllerBase.ProyectoOrigenBusquedaID != Guid.Empty)
                            {
                                extraJsProyOrigen += filtroPest + "[|]" + pPestanyaMenu.FilaProyectoPestanyaBusqueda.CampoFiltro + "[||]";
                                pBuscador.JSExtra = "hackBusquedaPestProyOrigen = '" + extraJsProyOrigen + "';";
                            }
                        }
                        #endregion
                        break;
                    case TipoPestanyaMenu.Preguntas:
                        #region Preguntas
                        HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelPreguntas = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                        selectComboModelPreguntas.ID = pPestanyaMenu.Clave.ToString();
                        selectComboModelPreguntas.Autocomplete = Es.Riam.Gnoss.AD.BASE_BD.OrigenAutocompletar.Preguntas;
                        selectComboModelPreguntas.FacetAutocomplete = facetasAutocompletar;
                        selectComboModelPreguntas.Name = nombrePestanya;
                        selectComboModelPreguntas.Selected = pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave;
                        if (string.IsNullOrEmpty(selectComboModelPreguntas.Name))
                        {
                            selectComboModelPreguntas.Name = UtilIdiomas.GetText("COMMON", "PREGUNTAS");
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "PREGUNTAS");
                        }
                        selectComboModelPreguntas.Url = rutaPestanya;
                        pListaSelectCombo.Add(selectComboModelPreguntas);
                        #endregion
                        break;
                    case TipoPestanyaMenu.Encuestas:
                        #region Encuestas
                        HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelEncuestas = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                        selectComboModelEncuestas.ID = pPestanyaMenu.Clave.ToString();
                        selectComboModelEncuestas.Autocomplete = Es.Riam.Gnoss.AD.BASE_BD.OrigenAutocompletar.Encuestas;
                        selectComboModelEncuestas.FacetAutocomplete = facetasAutocompletar;
                        selectComboModelEncuestas.Name = nombrePestanya;
                        selectComboModelEncuestas.Selected = pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave;
                        if (string.IsNullOrEmpty(selectComboModelEncuestas.Name))
                        {
                            selectComboModelEncuestas.Name = UtilIdiomas.GetText("COMMON", "ENCUESTAS");
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "ENCUESTAS");
                        }
                        selectComboModelEncuestas.Url = rutaPestanya;
                        pListaSelectCombo.Add(selectComboModelEncuestas);
                        #endregion
                        break;
                    case TipoPestanyaMenu.Debates:
                        #region Debates
                        HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelDebates = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                        selectComboModelDebates.ID = pPestanyaMenu.Clave.ToString();
                        selectComboModelDebates.Autocomplete = Es.Riam.Gnoss.AD.BASE_BD.OrigenAutocompletar.Debates;
                        selectComboModelDebates.FacetAutocomplete = facetasAutocompletar;
                        selectComboModelDebates.Name = nombrePestanya;
                        selectComboModelDebates.Selected = pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave;
                        if (string.IsNullOrEmpty(selectComboModelDebates.Name))
                        {
                            selectComboModelDebates.Name = UtilIdiomas.GetText("COMMON", "DEBATES");
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "DEBATES");
                        }
                        selectComboModelDebates.Url = rutaPestanya;
                        pListaSelectCombo.Add(selectComboModelDebates);
                        #endregion
                        break;
                    case TipoPestanyaMenu.PersonasYOrganizaciones:
                        #region PersonasYOrganizaciones
                        HeaderModel.SearchHeaderModel.SearchSelectComboModel selectComboModelPersonas = new HeaderModel.SearchHeaderModel.SearchSelectComboModel();
                        selectComboModelPersonas.ID = pPestanyaMenu.Clave.ToString();
                        selectComboModelPersonas.Autocomplete = Es.Riam.Gnoss.AD.BASE_BD.OrigenAutocompletar.PersyOrg;
                        selectComboModelPersonas.FacetAutocomplete = facetasAutocompletar;
                        selectComboModelPersonas.Name = nombrePestanya;
                        selectComboModelPersonas.Selected = pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave;
                        if (string.IsNullOrEmpty(selectComboModelPersonas.Name))
                        {
                            if (ProyectoVirtual.EsProyectoEduca)
                            {
                                selectComboModelPersonas.Name = UtilIdiomas.GetText("COMMON", "PROFESORESYALUMNOS");
                            }
                            else
                            {
                                selectComboModelPersonas.Name = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES");
                            }
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES");
                        }
                        selectComboModelPersonas.Url = rutaPestanya;
                        pListaSelectCombo.Add(selectComboModelPersonas);
                        #endregion
                        break;
                }
            }

            foreach (ProyectoPestanyaMenu pestanyaMenu in ProyectoVirtual.ListaPestanyasMenu.Values)
            {
                if (pestanyaMenu.FilaProyectoPestanyaMenu.PestanyaPadreID.HasValue && pestanyaMenu.FilaProyectoPestanyaMenu.PestanyaPadreID == pPestanyaMenu.FilaProyectoPestanyaMenu.PestanyaID)
                {
                    CargarPestanyaMetabuscadorComunidad(pListaSelectCombo, pParamDS, pBuscador, pestanyaMenu, pPestanyaDefecto);
                }
            }
        }

        private void ProcesarPrivacidadPestanyasMetabuscadorComunidad(HeaderModel.SearchHeaderModel pSearchHeaderModel)
        {
            List<HeaderModel.SearchHeaderModel.SearchSelectComboModel> combosEliminar = new List<HeaderModel.SearchHeaderModel.SearchSelectComboModel>();
            bool marcarSleccionada = false;
            foreach (HeaderModel.SearchHeaderModel.SearchSelectComboModel combo in pSearchHeaderModel.ListSelectCombo)
            {
                if (marcarSleccionada)
                {
                    combo.Selected = true;
                    marcarSleccionada = false;
                }

                Guid idPestanya = Guid.Empty;
                if (Guid.TryParse(combo.ID, out idPestanya) && ProyectoVirtual.ListaPestanyasMenu.ContainsKey(idPestanya))
                {
                    ProyectoPestanyaMenu pestanya = ProyectoVirtual.ListaPestanyasMenu[idPestanya];
                    if (!pestanya.VisibleSinAcceso && pestanya.Privacidad != TipoPrivacidadPagina.Normal)
                    {
                        if (pestanya.Privacidad == TipoPrivacidadPagina.Lectores)
                        {
                            if (IdentidadActual == null || mControladorBase.UsuarioActual.EsIdentidadInvitada)
                            {
                                if (combo.Selected)
                                {
                                    marcarSleccionada = true;
                                }
                                combosEliminar.Add(combo);
                            }
                            else
                            {
                                bool perfilConPermiso = pestanya.ListaRolIdentidad.ContainsKey(IdentidadActual.PerfilID);
                                if (!perfilConPermiso)
                                {
                                    bool perfilEnGrupoConPermiso = false;
                                    foreach (Guid idGrupoIdentidad in mControllerBase.ListaGruposPerfilEnProyectoVirtual)
                                    {
                                        if (pestanya.ListaRolGrupoIdentidades.ContainsKey(idGrupoIdentidad))
                                        {
                                            perfilEnGrupoConPermiso = true;
                                            break;
                                        }
                                    }
                                    if (!perfilEnGrupoConPermiso)
                                    {
                                        if (combo.Selected)
                                        {
                                            marcarSleccionada = true;
                                        }
                                        combosEliminar.Add(combo);
                                    }
                                }
                            }
                        }
                        else if (pestanya.Privacidad == TipoPrivacidadPagina.Especial)
                        {
                            if (mControladorBase.UsuarioActual.EsIdentidadInvitada && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido))
                            {
                                if (combo.Selected)
                                {
                                    marcarSleccionada = true;
                                }
                                combosEliminar.Add(combo);
                            }
                        }
                    }
                }
            }

            foreach (HeaderModel.SearchHeaderModel.SearchSelectComboModel comboEliminar in combosEliminar)
            {
                pSearchHeaderModel.ListSelectCombo.Remove(comboEliminar);
            }
        }

        private void ProcesarCombosBusquedas(List<HeaderModel.SearchHeaderModel.SearchSelectComboModel> pListaCombos, UtilIdiomas pUtilIdiomas, string pIdiomaDefecto)
        {
            string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(pUtilIdiomas, mControllerBase.BaseURLIdioma, ProyectoVirtual.NombreCorto) + "/";
            if (ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                urlComunidad = BaseURLIdioma + UrlPerfil;
            }
            foreach (HeaderModel.SearchHeaderModel.SearchSelectComboModel combo in pListaCombos)
            {
                combo.Name = UtilCadenas.ObtenerTextoDeIdioma(combo.Name, pUtilIdiomas.LanguageCode, pIdiomaDefecto);
                combo.Url = urlComunidad + UtilCadenas.ObtenerTextoDeIdioma(combo.Url, pUtilIdiomas.LanguageCode, pIdiomaDefecto) + "?search=";
            }
        }
    }
}
