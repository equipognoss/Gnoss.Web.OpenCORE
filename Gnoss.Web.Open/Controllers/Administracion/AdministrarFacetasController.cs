using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AdministrarFacetasViewModel
    {
        public Dictionary<string, string> ListaIdiomas { get; set; }

        public string IdiomaPorDefecto { get; set; }

        public List<FacetaModel> ListaFacetas { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarFacetasController : ControllerBaseWeb
    {

        public AdministrarFacetasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarFacetasViewModel mPaginaModel = null;

        private DataWrapperFacetas mFacetaDW = null;

        private Dictionary<string, string> mListaOntologias = null;

        #endregion

        #region Metodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View(PaginaModel);
        }

        /// <summary>
        /// Nueva Pestaña
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaFaceta(TipoFaceta TipoFaceta)
        {
            EliminarPersonalizacionVistas();

            FacetaModel faceta = CargarFacetaNueva(TipoFaceta);

            CargarOntologias();

            return PartialView("_EdicionFaceta", faceta);
        }

        /// <summary>
        /// Nueva Pestaña
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevoFiltro()
        {
            //EliminarPersonalizacionVistas();
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();


            FacetaModel.FiltrosFacetas filtro = new FacetaModel.FiltrosFacetas();
            string filtroID = Guid.NewGuid().ToString();
            filtro.Nombre = filtroID;
            //LE pongo el valor deleted true para que se sepa que es un filtro nuevo en la vista.
            filtro.Deleted = true;

            return PartialView("_FichaFiltro", filtro);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Guardar(List<FacetaModel> ListaFacetas)
        {
            bool iniciado = false;
            try
            {
                iniciado = HayIntegracionContinua;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                return GnossResultERROR("Contacte con el administrador del Proyecto, no es posible atender la petición.");
            }
            ControladorFacetas contrFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD);
            string errores = contrFacetas.ComprobarErrores(ListaFacetas);
            if (string.IsNullOrEmpty(errores))
            {
                GuardarXmlCambiosAdministracion();

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;

                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    contrFacetas.GuardarFacetas(ListaFacetas);
                    if (iniciado)
                    {
                        HttpResponseMessage resultado = InformarCambioAdministracion("Facetas", JsonConvert.SerializeObject(ListaFacetas, Formatting.Indented));

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch (Exception ex)
                {
                    if (transaccionIniciada)
                    {
                        proyAD.TerminarTransaccion(false);
                    }

                    return GnossResultERROR(ex.Message);
                }

                contrFacetas.InvalidarCaches(UrlIntragnoss);

                contrFacetas.CrearFilasPropiedadesIntegracionContinua(ListaFacetas);

                if (EntornoActualEsPruebas && iniciado)
                {
                    //contrFacetas.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaFacetas, UrlApiDesplieguesEntornoSiguiente);
                    contrFacetas.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaFacetas, UrlApiEntornoSeleccionado("pre"), UsuarioActual.UsuarioID);
                    contrFacetas.ModificarFilasIntegracionContinuaEntornoSiguiente(ListaFacetas, UrlApiEntornoSeleccionado("pro"), UsuarioActual.UsuarioID);
                }

                return GnossResultOK();
            }
            else
            {
                return GnossResultERROR(errores);
            }
        }
        #endregion

        #region Metodos

        private FacetaModel CargarFacetaNueva(TipoFaceta pTipoFaceta)
        {
            FacetaModel faceta = new FacetaModel();
            faceta.Name = UtilIdiomas.GetText("COMADMINFACETAS", "NUEVAFACETA");
            faceta.Type = pTipoFaceta;
            faceta.ClaveFaceta = "";
            faceta.Reciprocidad = "";
            faceta.ObjetosConocimiento = new List<string>();

            faceta.Presentacion = 0;
            faceta.Disenyo = 0;
            faceta.AlgoritmoTransformacion = 0;
            faceta.NumElementosVisibles = 5;

            faceta.EsSemantica = true;
            faceta.Autocompletar = false;

            faceta.Comportamiento = 0;
            faceta.Excluyente = false;

            faceta.OcultaEnFacetas = false;
            faceta.OcultaEnFiltros = false;
            faceta.PrivacidadGrupos = new Dictionary<Guid, string>();
            faceta.Condicion = "";
            faceta.PriorizarOrdenResultados = false;

            faceta.ListaFiltrosFacetas = new List<FacetaModel.FiltrosFacetas>();
            faceta.Orden = 0;
            return faceta;
        }



        private void CargarOntologias()
        {
            ViewBag.ListaOntologias = ListaOntologias;
        }

        #endregion

        #region Propiedades

        private Dictionary<string, string> ListaOntologias
        {
            get
            {
                if (mListaOntologias == null)
                {
                    mListaOntologias = new Dictionary<string, string>();

                    foreach (OntologiaProyecto filaOntologia in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaOntologiaProyecto)
                    {
                        mListaOntologias.Add(filaOntologia.OntologiaProyecto1.ToLower(), UtilCadenas.ObtenerTextoDeIdioma(filaOntologia.NombreOnt, UtilIdiomas.LanguageCode, IdiomaPorDefecto));
                    }
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_RECURSOS.ToLower(), UtilIdiomas.GetText("COMMON", "RECURSOS"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_PREGUNTAS.ToLower(), UtilIdiomas.GetText("COMMON", "PREGUNTAS"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_DEBATES.ToLower(), UtilIdiomas.GetText("COMMON", "DEBATES"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_ENCUESTAS.ToLower(), UtilIdiomas.GetText("COMMON", "ENCUESTAS"));
                    if (!mListaOntologias.ContainsKey(FacetadoAD.BUSQUEDA_PERSONA.ToLower()))
                    {
                        mListaOntologias.Add(FacetadoAD.BUSQUEDA_PERSONA.ToLower(), UtilIdiomas.GetText("COMMON", "PERSONAS"));
                    }
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_ORGANIZACION.ToLower(), UtilIdiomas.GetText("COMMON", "ORGANIZACIONES"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_CLASE.ToLower(), UtilIdiomas.GetText("CONFIGURACIONFACETADO", "CLASE"));
                    mListaOntologias.Add(FacetadoAD.BUSQUEDA_GRUPO.ToLower(), UtilIdiomas.GetText("GRUPO", "GRUPOS"));
                }
                return mListaOntologias;
            }
        }

        private AdministrarFacetasViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarFacetasViewModel();

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

                    CargarOntologias();

                    ControladorFacetas contrFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD);

                    mPaginaModel.ListaFacetas = contrFacetas.CargarListadoFacetas();


                }
                return mPaginaModel;
            }
        }

        #endregion
    }
}
