using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Es.Riam.Gnoss.Web.MVC.Models.HeaderModel.SearchHeaderModel;

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
        protected LoggingService mLoggingService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ControllerCabeceraBase(ControllerBaseWeb pControllerBase, IHttpContextAccessor httpContextAccessor, EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, ILogger<ControllerCabeceraBase> logger, ILoggerFactory loggerFactory)
        {
            mControllerBase = pControllerBase;
            mHttpContextAccessor = httpContextAccessor;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            ViewBag = pControllerBase.ViewBag;
            mConfigService = configService;
            IdentidadActual = mControllerBase.IdentidadActual;
            ProyectoSeleccionado = mControllerBase.ProyectoSeleccionado;
            ProyectoVirtual = mControllerBase.ProyectoVirtual;            
            UtilIdiomas = mControllerBase.UtilIdiomas;
            BaseURLIdioma = mControllerBase.BaseURLIdioma;
            UrlPerfil = mControllerBase.UrlPerfil;
            Request = mControllerBase.Request;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null, mLoggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);
        }

        private string RequestParams(string pParametro)
        {
            return mControllerBase.RequestParams(pParametro);
        }

        public void CargarDatosCabecera()
        {
            HeaderModel cabecera = new HeaderModel();
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

			cabecera.UrlAdvancedSearch = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, mControllerBase.BaseURLIdioma, ProyectoVirtual.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");

            cabecera.Languajes = paramCL.ObtenerListaIdiomasDictionary();

            cabecera.Searcher = CargarMetabuscador();

            cabecera.HomeUserAvailable = !mControllerBase.EsEcosistemaSinMetaProyecto;

            cabecera.SubscriptionsAvailable = !mControllerBase.EsEcosistemaSinBandejaSuscripciones;

            cabecera.ContactsAvailable = !mControllerBase.EsEcosistemaSinContactos;

            cabecera.ChangePasswordVisible = !(mControllerBase.ParametroProyecto.ContainsKey(ParametroAD.OcultarCambiarPassword) && mControllerBase.ParametroProyecto[ParametroAD.OcultarCambiarPassword].ToLower().Equals("true"));

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
                AgregarOpcionesBusquedaPeticion(cabecera.Searcher.ListSelectCombo);

                SeleccionarItemBusquedaActivo(cabecera.Searcher.ListSelectCombo);
            }
                       
            ViewBag.Cabecera = cabecera;
        }

        /// <summary>
        /// De la lista de opciones para los buscadores, marcamos como seleccionado el que nos interesa en función de la petición recibida
        /// </summary>
        /// <param name="pListaOpciones">Lista de opciones a la que se añadirán las necesarias</param>
        private void SeleccionarItemBusquedaActivo(List<SearchSelectComboModel> pListaOpciones)
        {
            string pagActual = "";
            if (Request != null)
            {
                pagActual = Request.Path.ToString().Trim('/').ToLower();
            }
            if (pagActual.Equals("admin"))
            {
                string path = Request.Path.ToString().Remove(Request.Path.ToString().LastIndexOf("/"));//quito el /admin
                pagActual = path.Substring(path.LastIndexOf("/") + 1);
            }

            SearchSelectComboModel elementoSeleccionado = pListaOpciones.Where(item => pagActual.ToLower().Contains(item.Name.ToLower())).FirstOrDefault();
            if (elementoSeleccionado != null)
            {
                elementoSeleccionado.Selected = true;
            }
        }

        /// <summary>
        /// Agrega las opciones de búsqueda a la lista en función del origen de la petición recibida
        /// </summary>
        /// <param name="pListaOpciones">Lista de opciones a la que se añadirán las necesarias</param>
        private void AgregarOpcionesBusquedaPeticion(List<SearchSelectComboModel> pListaOpciones)
        {
            string search = "?search=";
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
                pListaOpciones.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES, textoBusquedaEspacioPersonal, BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MISRECURSOS") + search, true, FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES));
            }
            else if (RequestParams("perfilorg") != null && RequestParams("perfilorg").Equals("true"))
            {
                pListaOpciones.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES, UtilIdiomas.GetText("COMENTARIOS", "ESPACIOCORPORATIVODE", IdentidadActual.OrganizacionPerfil.Nombre), BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MISRECURSOS") + search, true, FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES));
            }
            else if (RequestParams("mensajes") != null && RequestParams("mensajes").Equals("true"))
            {
                pListaOpciones.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_MENSAJES, UtilIdiomas.GetText("MENU", "MENSAJES"), BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MENSAJES") + search, true, FacetadoAD.BUSQUEDA_MENSAJES));
            }
            if (RequestParams("contribuciones") != null && RequestParams("contribuciones").Equals("true"))
            {
                string textoBusqueda = UtilIdiomas.GetText("PERFIL", "MISCONTRIBUCIONES");
                if (IdentidadActual.OrganizacionID.HasValue)
                {
                    //Administrador organizacion
                    textoBusqueda = UtilIdiomas.GetText("PERFIL", "CONTRIBUCIONESDE", IdentidadActual.OrganizacionPerfil.Nombre);
                }

                pListaOpciones.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS, textoBusqueda, BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MISCONTRIBUCIONES") + search, true, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS));
            }
            if (RequestParams("contactos") != null && RequestParams("contactos").Equals("true"))
            {
                string urlContactos = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "CONTACTOS") + search;
                if (RequestParams("organizacion") != null && IdentidadActual.IdentidadOrganizacion != null)
                {
                    urlContactos = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "CONTACTOSORG") + search;
                }

                pListaOpciones.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_CONTACTOS, UtilIdiomas.GetText("COMMON", "CONTACTOS"), urlContactos, true, FacetadoAD.BUSQUEDA_CONTACTOS));
            }

            if(RequestParams("mis-recursos") != null && RequestParams("mis-recursos").Equals("true"))
            {
                pListaOpciones.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_MIS_RECURSOS, UtilIdiomas.GetText("URLSEM", "MISRECURSOS"), $"{BaseURLIdioma}/{UtilIdiomas.GetText("URLSEM", "MISRECURSOS")}{search}", true, FacetadoAD.BUSQUEDA_MIS_RECURSOS));
            }
        }

        private string AgregarEventoComunidad(string pUrlServicioLogin)
        {
            if (!string.IsNullOrEmpty(RequestParams("eventoComID")))
            {
                pUrlServicioLogin += $"&eventoComID={RequestParams("eventoComID")}";
            }

            return pUrlServicioLogin;
        }

        /// <summary>
        /// Carga un control lista con las categorías de primer nivel
        /// </summary>
        public HeaderModel.SearchHeaderModel CargarMetabuscador()
        {
            HeaderModel.SearchHeaderModel buscador = new HeaderModel.SearchHeaderModel();
            buscador.ListSelectCombo = new List<SearchSelectComboModel>();
           
            GestorParametroGeneral gestorParametroGeneral = new GestorParametroGeneral();
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
            gestorParametroGeneral = gestorController.ObtenerParametrosGeneralesDeProyecto(gestorParametroGeneral, ProyectoVirtual.Clave);
            bool metaBusquedaActiva = false;
            List<ConfiguracionAmbitoBusquedaProyecto> listaConfiguracionAmbitoBusqueda = gestorParametroGeneral.ListaConfiguracionAmbitoBusquedaProyecto.Where(configuracionAmbitoBusquedaProyecto => configuracionAmbitoBusquedaProyecto.ProyectoID.Equals(ProyectoVirtual.Clave)).ToList();
            if (((ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto) || mControllerBase.ParametrosGeneralesVirtualRow.ComunidadGNOSS) && !mControllerBase.EsEcosistemaSinMetaProyecto))
            {
                if (listaConfiguracionAmbitoBusqueda.Count > 0 && listaConfiguracionAmbitoBusqueda[0].TodoGnoss)
                {
                    metaBusquedaActiva = true;
                }
                else
                {
                    //Si no hay filas lo pintamos todo
                    metaBusquedaActiva = true;
                }
            }
            if (metaBusquedaActiva)
            {
                //Metabúsqueda
                buscador.ListSelectCombo.Add(GenerarSearchComboModel($"MyGNOSS{FacetadoAD.BUSQUEDA_AVANZADA}", UtilIdiomas.GetText("BUSCADORFACETADO", "TODOGNOSS", mControllerBase.NombreProyectoEcosistema), UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA"), ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto)));
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
                foreach (SearchSelectComboModel combo in buscador.ListSelectCombo)
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

            return buscador;
        }

        /// <summary>
        /// Cargo los posibles buscadores del proyecto. Se marcará el seleccionado a posteriori en función de la página en la que te encuentres.
        /// </summary>
        /// <param name="pListaSelectCombo">Lista de buscadores donde se añadirán más</param>
        public void CargarMetabuscadorMetaProyecto(List<SearchSelectComboModel> pListaSelectCombo)
        {
            //Blogs            
            pListaSelectCombo.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_BLOGS, UtilIdiomas.GetText("COMMON", "BLOGS"), UtilIdiomas.GetText("URLSEM", "BLOGS"), false));

            //Comentarios
            pListaSelectCombo.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_COMENTARIOS, UtilIdiomas.GetText("COMMON", "COMENTARIOS"), UtilIdiomas.GetText("URLSEM", "COMENTARIOS"), false));

            //Comunidades            
            pListaSelectCombo.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_COMUNIDADES, UtilIdiomas.GetText("COMMON", "COMUNIDADES"), UtilIdiomas.GetText("URLSEM", "COMUNIDADES"), false));

            //Personas
            string urlPersonasYOrganizaciones = UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES");
            if (mControllerBase.AdministradorQuiereVerTodasLasPersonas)
            {
                urlPersonasYOrganizaciones += "/admin";
            }

            pListaSelectCombo.Add(GenerarSearchComboModel(FacetadoAD.BUSQUEDA_PERSONASYORG, UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES"), urlPersonasYOrganizaciones, false, AD.BASE_BD.OrigenAutocompletar.PersyOrg));
        }

        public void CargarMetabuscadorComunidad(List<SearchSelectComboModel> pListaSelectCombo, GestorParametroGeneral gestorParametroGeneral, HeaderModel.SearchHeaderModel pBuscador)
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
                string searchPersonalizado = "search";

                if (pestanyaBusquedaAvanzada != null && pestanyaBusquedaAvanzada.Activa && (pestanyaBusquedaAvanzada.FilaProyectoPestanyaBusqueda == null || pestanyaBusquedaAvanzada.FilaProyectoPestanyaBusqueda.MostrarEnComboBusqueda))
                {
                    string rutaPestanya = pestanyaBusquedaAvanzada.Ruta;
                    string nombrePestanya = pestanyaBusquedaAvanzada.Nombre;
                    if (string.IsNullOrEmpty(rutaPestanya))
                    {
                        rutaPestanya = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    }

                    if (string.IsNullOrEmpty(nombrePestanya))
                    {
                        nombrePestanya = UtilIdiomas.GetText("BUSCADORFACETADO", "TODALACOMUNIDAD");
                    }
                    

                    if (pestanyaBusquedaAvanzada.FilaProyectoPestanyaBusqueda != null)
                    {
                        searchPersonalizado = pestanyaBusquedaAvanzada.FilaProyectoPestanyaBusqueda.SearchPersonalizado;
                    }

                    pListaSelectCombo.Add(GenerarSearchComboModel(pestanyaBusquedaAvanzada.Clave.ToString(), nombrePestanya, rutaPestanya, !pestanyaDefecto.HasValue,"","",-1, searchPersonalizado));
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
       
        private void CargarPestanyaMetabuscadorComunidad(List<SearchSelectComboModel> pListaSelectCombo, GestorParametroGeneral pParamDS, HeaderModel.SearchHeaderModel pBuscador, ProyectoPestanyaMenu pPestanyaMenu, Guid? pPestanyaDefecto)
        {
            string nombrePestanya = pPestanyaMenu.Nombre;
            string rutaPestanya = pPestanyaMenu.Ruta;
            string facetasAutocompletar = "";
            string searchPersonalizado = "search";
            if (pPestanyaMenu.Activa && (pPestanyaMenu.FilaProyectoPestanyaBusqueda == null || pPestanyaMenu.FilaProyectoPestanyaBusqueda.MostrarEnComboBusqueda))
            {
                short tipoAutocompletar = -1;
				if (pPestanyaMenu.FilaProyectoPestanyaBusqueda != null)
				{
					tipoAutocompletar = (short)pPestanyaMenu.FilaProyectoPestanyaBusqueda.TipoAutocompletar;
                    searchPersonalizado = pPestanyaMenu.FilaProyectoPestanyaBusqueda.SearchPersonalizado;
                }                
                switch (pPestanyaMenu.TipoPestanya)
                {
                    case TipoPestanyaMenu.Recursos:
                        #region Recursos
                        
                        if (string.IsNullOrEmpty(nombrePestanya))
                        {
                            nombrePestanya = UtilIdiomas.GetText("COMMON", "BASERECURSOS");
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "RECURSOS");
                        }                                              
                        pListaSelectCombo.Add(GenerarSearchComboModel(pPestanyaMenu.Clave.ToString(), nombrePestanya, rutaPestanya, pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave, AD.BASE_BD.OrigenAutocompletar.Recursos, facetasAutocompletar, tipoAutocompletar, searchPersonalizado));                        
                        #endregion
                        break;
                    case TipoPestanyaMenu.BusquedaSemantica:
                        #region Busqueda semántica
                        //Añadimos las busquedas de los recursos semánticos configurados
                        if (pPestanyaMenu.FilaProyectoPestanyaBusqueda != null)
                        {
                            string extraJsProyOrigen = "";
                            string filtroPest = pPestanyaMenu.FilaProyectoPestanyaBusqueda.CampoFiltro.Replace("rdf:type=", "");
                           
                           
                            pListaSelectCombo.Add(GenerarSearchComboModel(pPestanyaMenu.Clave.ToString(), nombrePestanya, rutaPestanya, pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave, pPestanyaMenu.FilaProyectoPestanyaMenu.Ruta, facetasAutocompletar, tipoAutocompletar, searchPersonalizado));

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

                        if (string.IsNullOrEmpty(nombrePestanya))
                        {
                            nombrePestanya = UtilIdiomas.GetText("COMMON", "PREGUNTAS");
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "PREGUNTAS");
                        }
                        
                        pListaSelectCombo.Add(GenerarSearchComboModel(pPestanyaMenu.Clave.ToString(), nombrePestanya, rutaPestanya, pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave, AD.BASE_BD.OrigenAutocompletar.Preguntas, facetasAutocompletar, tipoAutocompletar));
                        
                        #endregion
                        break;
                    case TipoPestanyaMenu.Encuestas:
                        #region Encuestas

                        if (string.IsNullOrEmpty(nombrePestanya))
                        {
                            nombrePestanya = UtilIdiomas.GetText("COMMON", "ENCUESTAS");
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "ENCUESTAS");
                        }

                        pListaSelectCombo.Add(GenerarSearchComboModel(pPestanyaMenu.Clave.ToString(), nombrePestanya, rutaPestanya, pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave, AD.BASE_BD.OrigenAutocompletar.Encuestas, facetasAutocompletar, tipoAutocompletar));
                        
                        #endregion
                        break;
                    case TipoPestanyaMenu.Debates:
                        #region Debates

                        if (string.IsNullOrEmpty(nombrePestanya))
                        {
                            nombrePestanya = UtilIdiomas.GetText("COMMON", "DEBATES");
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "DEBATES");
                        }

                        pListaSelectCombo.Add(GenerarSearchComboModel(pPestanyaMenu.Clave.ToString(), nombrePestanya, rutaPestanya, pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave, AD.BASE_BD.OrigenAutocompletar.Debates, facetasAutocompletar, tipoAutocompletar));

                        #endregion
                        break;
                    case TipoPestanyaMenu.PersonasYOrganizaciones:
                        #region PersonasYOrganizaciones
                       
                        if (string.IsNullOrEmpty(nombrePestanya))
                        {
                            if (ProyectoVirtual.EsProyectoEduca)
                            {
                                nombrePestanya = UtilIdiomas.GetText("COMMON", "PROFESORESYALUMNOS");
                            }
                            else
                            {
                                nombrePestanya = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES");
                            }
                        }
                        if (string.IsNullOrEmpty(rutaPestanya))
                        {
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES");
                        }
                        
                        pListaSelectCombo.Add(GenerarSearchComboModel(pPestanyaMenu.Clave.ToString(), nombrePestanya, rutaPestanya, pPestanyaDefecto.HasValue && pPestanyaDefecto.Value == pPestanyaMenu.Clave, AD.BASE_BD.OrigenAutocompletar.PersyOrg, facetasAutocompletar, tipoAutocompletar));

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
            List<SearchSelectComboModel> combosEliminar = new List<SearchSelectComboModel>();
            bool marcarSleccionada = false;
            foreach (SearchSelectComboModel combo in pSearchHeaderModel.ListSelectCombo)
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

            foreach (SearchSelectComboModel comboEliminar in combosEliminar)
            {
                pSearchHeaderModel.ListSelectCombo.Remove(comboEliminar);
            }
        }

        private void ProcesarCombosBusquedas(List<SearchSelectComboModel> pListaCombos, UtilIdiomas pUtilIdiomas, string pIdiomaDefecto)
        {
            string urlComunidad = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(pUtilIdiomas, mControllerBase.BaseURLIdioma, ProyectoVirtual.NombreCorto)}/";
            if (ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                urlComunidad = BaseURLIdioma + UrlPerfil;
            }
            foreach (SearchSelectComboModel combo in pListaCombos)
            {
                combo.Name = UtilCadenas.ObtenerTextoDeIdioma(combo.Name, pUtilIdiomas.LanguageCode, pIdiomaDefecto);
                combo.Url = $"{urlComunidad}{UtilCadenas.ObtenerTextoDeIdioma(combo.Url, pUtilIdiomas.LanguageCode, pIdiomaDefecto)}?{combo.SearchPersonalizado}=";
            }
        }

        /// <summary>
        /// Crea una opción para los buscadores del proyecto
        /// </summary>
        /// <param name="pId">Id del elemento</param>
        /// <param name="pName">Nombre del elemento</param>
        /// <param name="pUrl">Url de la página en la que se realizará la búesqueda</param>
        /// <param name="pSelected">Si es o no el elemento seleccionado del combo</param>
        /// <param name="pAutocomplete">Nombre utilizado para el autocompletar del elemento. Puede ser nulo</param>
        /// <param name="tipoAutocompletar">Indica el tipo de autocompletar que se va a usar en la pestaña</param>
        /// <returns></returns>
        private SearchSelectComboModel GenerarSearchComboModel(string pId, string pName, string pUrl, bool pSelected, string pAutocomplete = "", string pFacetAutocomplete = "", short pTipoAutocompletar = -1, string pSearchPersonalizado = "search")
        {
            return new SearchSelectComboModel() { ID = pId, Name = pName, Url = pUrl, Selected = pSelected, Autocomplete = pAutocomplete, FacetAutocomplete = pFacetAutocomplete, TipoAutocompletar = pTipoAutocompletar , SearchPersonalizado=pSearchPersonalizado };
        }
    }
}
