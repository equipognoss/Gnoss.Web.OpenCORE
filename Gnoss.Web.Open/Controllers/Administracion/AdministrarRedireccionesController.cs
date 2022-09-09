using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Gnoss.Web.MVC;
using Es.Riam.Interfaces.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using Es.Riam.InterfacesOpen;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Redirections Registry Administration View Model
    /// </summary>
    public class ManageRedirectionsViewModel
    {
        /// <summary>
        /// List of Redirection Model
        /// </summary>
        public List<RedirectionModel> RedirectionsList { get; set; }
        /// <summary>
        /// Types of Redirections
        /// </summary>
        public enum RedirectionType
        {
            /// <summary>
            /// Directa.
            /// </summary>
            Direct = 0,
            /// <summary>
            /// Parametrizada.
            /// </summary>
            Parameterised = 1
        }
        /// <summary>
        /// Redirection Model
        /// </summary>
        [Serializable]
        public partial class RedirectionModel
        {
            /// <summary>
            /// 
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string OriginalUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string DestinationUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string ParameterName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<ParameterValue> ParameterValues { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool PreserveFilters { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool DeleteRedirection { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public RedirectionType RedirectionType { get; set; }
        }

        /// <summary>
        /// Parameter Value Model
        /// </summary>
        [Serializable]
        public partial class ParameterValue
        {
            /// <summary>
            /// 
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string DestinationUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Order { get; set; }
        }
    }

    /// <summary>
    /// Handles the registration of redirections 
    /// </summary>
    public class AdministrarRedireccionesController : ControllerBaseWeb
    {
        public AdministrarRedireccionesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        private ManageRedirectionsViewModel mPaginaModel = null;

        #endregion

        #region Métodos de evento
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            return View(PaginaModel);
        }

        /// <summary>
        /// Nueva Redireccion
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult NuevaRedireccion()
        {
            EliminarPersonalizacionVistas();

            ManageRedirectionsViewModel.RedirectionModel redireccion = new ManageRedirectionsViewModel.RedirectionModel();
            redireccion.Key = Guid.NewGuid();
            redireccion.OriginalUrl = "";
            redireccion.DestinationUrl = "";
            redireccion.PreserveFilters = false;
            redireccion.ParameterValues = new List<ManageRedirectionsViewModel.ParameterValue>();

            return PartialView("_EdicionRedireccion", redireccion);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult Guardar(List<ManageRedirectionsViewModel.RedirectionModel> ListaRedirecciones)
        {
            string errores = "";
            //controlar errores
            if(ListaRedirecciones == null || ListaRedirecciones.Count == 0)
            {
                errores = "LISTA VACIA";
            }

            if (string.IsNullOrEmpty(errores))
            {
                foreach (ManageRedirectionsViewModel.RedirectionModel redVista in ListaRedirecciones)
                {
                    if (string.IsNullOrEmpty(redVista.OriginalUrl))
                    {
                        errores = "URLORIGEN VACIO";
                    }
                    else if (redVista.RedirectionType.Equals(ManageRedirectionsViewModel.RedirectionType.Direct))
                    {
                        if (string.IsNullOrEmpty(redVista.DestinationUrl))
                        {
                            errores = "URLDESTINO VACIO";
                        }
                    }
                    else if (redVista.RedirectionType.Equals(ManageRedirectionsViewModel.RedirectionType.Parameterised))
                    {
                        if (string.IsNullOrEmpty(redVista.ParameterName))
                        {
                            errores = "NOMPARAMETRO VACIO";
                        }
                        else
                        {
                            if (redVista.ParameterValues != null)
                            {
                                foreach (ManageRedirectionsViewModel.ParameterValue valoresParam in redVista.ParameterValues)
                                {
                                    if (string.IsNullOrEmpty(valoresParam.Value) || string.IsNullOrEmpty(valoresParam.DestinationUrl))
                                    {
                                        errores = "CAMPOPARAMETROS VACIO";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                errores = "SIN PARAMETROS";
                            }
                        }
                    }
                    else
                    {
                        errores = "TIPOREDIRECCION INCORRECTA";
                    }

                    if (!string.IsNullOrEmpty(errores))
                    {
                        errores += "|||" + redVista.Key;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(errores))
            {
                GuardarRedirecciones(ListaRedirecciones);

                try
                {
                    //MvcApplication.RecalculandoRutas = true;
                    new RouteConfig(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD).RecalcularTablaRutas();

                    mGnossCache.RecalcularRedirecciones();
                }
                catch(Exception ex)
                {
                    GuardarLogError(ex);
                    return GnossResultERROR(UtilIdiomas.GetText("COMADMINREDIRECCIONES", "ERRORRECALCULANDORUTAS"));
                }
                finally
                {
                    // TODO
                    //MvcApplication.RecalculandoRutas = false;
                }

                return GnossResultOK();
            }
            else {
                return GnossResultERROR(errores);
            }
        }

        #endregion

        #region Propiedades

        private ManageRedirectionsViewModel PaginaModel
        {
            get
            {
                if(mPaginaModel == null)
                {
                    mPaginaModel = new ManageRedirectionsViewModel();
                    mPaginaModel.RedirectionsList = new List<ManageRedirectionsViewModel.RedirectionModel>();

                    string dominio = ProyectoSeleccionado.UrlPropia(IdiomaUsuario).Replace("http://", "").Replace("https://", "");
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<RedireccionRegistroRuta> listaRedirecciones = proyCN.ObtenerRedireccionRegistroRutaPorDominio(dominio, false);
                    
                    if(listaRedirecciones != null && listaRedirecciones.Count > 0)
                    {
                        foreach (RedireccionRegistroRuta redireccion in listaRedirecciones.OrderByDescending(r => r.FechaCreacion))
                        {
                            ManageRedirectionsViewModel.RedirectionModel modeloRed = new ManageRedirectionsViewModel.RedirectionModel();
                            modeloRed.Key = redireccion.RedireccionID;
                            modeloRed.OriginalUrl = redireccion.UrlOrigen;
                            modeloRed.ParameterName = redireccion.NombreParametro;
                            
                            if (string.IsNullOrEmpty(redireccion.NombreParametro))
                            {
                                RedireccionValorParametro filaValor = redireccion.RedireccionValorParametro.First();
                                modeloRed.PreserveFilters = filaValor.MantenerFiltros;
                                modeloRed.DestinationUrl = filaValor.UrlRedireccion;
                                modeloRed.RedirectionType = ManageRedirectionsViewModel.RedirectionType.Direct;
                            }
                            else
                            {
                                modeloRed.PreserveFilters = false;
                                modeloRed.RedirectionType = ManageRedirectionsViewModel.RedirectionType.Parameterised;
                                modeloRed.ParameterValues = new List<ManageRedirectionsViewModel.ParameterValue>();
                                foreach (RedireccionValorParametro filaValor in redireccion.RedireccionValorParametro.OrderBy(r => r.OrdenPresentacion))
                                {
                                    ManageRedirectionsViewModel.ParameterValue modeloValor = new ManageRedirectionsViewModel.ParameterValue();
                                    modeloValor.Value = filaValor.ValorParametro;
                                    modeloValor.DestinationUrl = filaValor.UrlRedireccion;
                                    modeloValor.Order = filaValor.OrdenPresentacion;
                                    modeloRed.ParameterValues.Add(modeloValor);
                                }                                
                            }

                            mPaginaModel.RedirectionsList.Add(modeloRed);
                        }
                    }

                    proyCN.Dispose();
                }

                return mPaginaModel;
            }
        }

        #endregion

        #region Métodos privados

        private void GuardarRedirecciones(List<ManageRedirectionsViewModel.RedirectionModel> ListaRedirecciones)
        {
            List<Guid> listaRedireccionesBorrar = null;
            Dictionary<RedireccionRegistroRuta, bool> dicRedireccionesEditar = null;
            bool borrar = false;
            bool editar = false;
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            try {

                string dominio = ProyectoSeleccionado.UrlPropia(IdiomaUsuario).Replace("http://", "").Replace("https://", "");
                List<RedireccionValorParametro> listaValoresBorrar = null;
                List <RedireccionRegistroRuta> filasRedirecciones = proyCN.ObtenerRedireccionRegistroRutaPorDominio(dominio, true);

                if (filasRedirecciones != null && filasRedirecciones.Count > 0)
                {
                    //invierto el orden de la lista para que tras guardar se sigan mostrando en la vista en el orden en que el usuario las introdujo
                    ListaRedirecciones.Reverse();

                    //las eliminadas
                    foreach (Guid clave in ListaRedirecciones.Where(r => r.DeleteRedirection.Equals(true)).Select(x => x.Key))
                    {
                        borrar = true;
                        if (listaRedireccionesBorrar == null)
                        {
                            listaRedireccionesBorrar = new List<Guid>();
                        }

                        if (!listaRedireccionesBorrar.Contains(clave))
                        {
                            listaRedireccionesBorrar.Add(clave);
                        }
                    }
                }

                //las añadidas/editadas
                foreach (ManageRedirectionsViewModel.RedirectionModel redireccionVista in ListaRedirecciones.Where(r => r.DeleteRedirection.Equals(false)))
                {
                    editar = true;
                    if (dicRedireccionesEditar == null)
                    {
                        dicRedireccionesEditar = new Dictionary<RedireccionRegistroRuta, bool>();
                    }

                    bool esNueva = false;
                    RedireccionRegistroRuta filaRedireccion = null;
                    if (filasRedirecciones != null && filasRedirecciones.Count > 0)
                    {
                        filaRedireccion = filasRedirecciones.Find(r => r.RedireccionID.Equals(redireccionVista.Key));
                    }
                    if(filaRedireccion == null)
                    {
                        esNueva = true;
                        filaRedireccion = new RedireccionRegistroRuta();
                        filaRedireccion.FechaCreacion = DateTime.Now;
                    }
                    
                    filaRedireccion.RedireccionID = redireccionVista.Key;
                    filaRedireccion.UrlOrigen = redireccionVista.OriginalUrl;
                    filaRedireccion.Dominio = ProyectoSeleccionado.UrlPropia(IdiomaUsuario).Replace("http://", "").Replace("https://", "");
                    filaRedireccion.NombreParametro = string.Empty;

                    listaValoresBorrar = ComprobarFilasValorParametro(filaRedireccion, redireccionVista);
                    dicRedireccionesEditar.Add(filaRedireccion, esNueva);
                }

                if (borrar)
                {
                    proyCN.BorrarFilaRedireccionRegistroRuta(listaRedireccionesBorrar, editar);
                }
                if (editar)
                {
                    if (listaValoresBorrar != null && listaValoresBorrar.Count > 0)
                    {
                        proyCN.BorrarFilasRedireccionValorParametro(listaValoresBorrar, true);
                    }

                    proyCN.GuardarFilaRedireccionRegistroRuta(dicRedireccionesEditar, false);
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
            }
            finally
            {
                proyCN.Dispose();
            }
        }

        /// <summary>
        /// Añade y modifica las FilasValorParametro. Retorna una lista con las filas que hay que borrar 
        /// </summary>
        /// <param name="pFilaRedireccion"></param>
        /// <param name="pRedireccionVista"></param>
        /// <returns></returns>
        private List<RedireccionValorParametro> ComprobarFilasValorParametro(RedireccionRegistroRuta pFilaRedireccion, ManageRedirectionsViewModel.RedirectionModel pRedireccionVista)
        {
            List<RedireccionValorParametro> listaBorrar = new List<RedireccionValorParametro>();

            if (pFilaRedireccion.RedireccionValorParametro == null)
            {
                pFilaRedireccion.RedireccionValorParametro = new List<RedireccionValorParametro>();
            }

            //es redirección directa
            if (!string.IsNullOrEmpty(pRedireccionVista.DestinationUrl))
            {
                RedireccionValorParametro filaValorParam = null;
                if (pFilaRedireccion.RedireccionValorParametro != null && pFilaRedireccion.RedireccionValorParametro.Count > 0 && pFilaRedireccion.RedireccionValorParametro.FirstOrDefault(r => r.RedireccionID.Equals(pFilaRedireccion.RedireccionID) && r.ValorParametro.Equals(string.Empty)) != null)
                {
                    filaValorParam = pFilaRedireccion.RedireccionValorParametro.First(r => r.RedireccionID.Equals(pFilaRedireccion.RedireccionID));
                }
                else
                {
                    filaValorParam = mEntityContext.RedireccionValorParametro.FirstOrDefault(item => item.RedireccionID.Equals(pFilaRedireccion.RedireccionID));
                    if (filaValorParam == null)
                    {
                        filaValorParam = new RedireccionValorParametro();
                    }
                }
                    
                filaValorParam.RedireccionRegistroRuta = pFilaRedireccion;
                filaValorParam.RedireccionID = pRedireccionVista.Key;
                filaValorParam.ValorParametro = string.Empty;
                filaValorParam.MantenerFiltros = pRedireccionVista.PreserveFilters;
                filaValorParam.UrlRedireccion = pRedireccionVista.DestinationUrl;
                filaValorParam.OrdenPresentacion = 0;
                //pFilaRedireccion.RedireccionValorParametro.Add(filaValorParam);

                //hay que borrar las hijas que han cambiado de tipo
                foreach (RedireccionValorParametro filaValorBD in pFilaRedireccion.RedireccionValorParametro.Where(r => !r.ValorParametro.Equals(string.Empty)))
                {
                    listaBorrar.Add(filaValorBD);
                }
            }
            else //es redirección parametrizada
            {
                pFilaRedireccion.NombreParametro = pRedireccionVista.ParameterName;

                if (pRedireccionVista.ParameterValues != null && pRedireccionVista.ParameterValues.Count > 0)
                {
                    short orden = 0;
                    foreach (ManageRedirectionsViewModel.ParameterValue parametroValor in pRedireccionVista.ParameterValues)
                    {
                        RedireccionValorParametro filaValorParam = null;
                        if (pFilaRedireccion.RedireccionValorParametro != null && pFilaRedireccion.RedireccionValorParametro.Count > 0 && pFilaRedireccion.RedireccionValorParametro.Any(r => r.RedireccionID.Equals(pFilaRedireccion.RedireccionID) && r.ValorParametro != null && r.ValorParametro.Equals(parametroValor.Value)))
                        {
                            filaValorParam = pFilaRedireccion.RedireccionValorParametro.First(r => r.RedireccionID.Equals(pFilaRedireccion.RedireccionID) && r.ValorParametro.Equals(parametroValor.Value));
                        }

                        if (filaValorParam == null)
                        {
                            filaValorParam = new RedireccionValorParametro();
                        }
                        filaValorParam.RedireccionRegistroRuta = pFilaRedireccion;
                        filaValorParam.RedireccionID = pRedireccionVista.Key;
                        filaValorParam.ValorParametro = parametroValor.Value;
                        filaValorParam.UrlRedireccion = parametroValor.DestinationUrl;
                        filaValorParam.MantenerFiltros = false;
                        filaValorParam.OrdenPresentacion = orden;
                        pFilaRedireccion.RedireccionValorParametro.Add(filaValorParam);

                        orden++;
                    }

                    //hay que borrar las hijas que han cambiado de tipo
                    foreach (RedireccionValorParametro filaValorBD in pFilaRedireccion.RedireccionValorParametro.Where(r => r.ValorParametro.Equals(string.Empty)))
                    {
                        listaBorrar.Add(filaValorBD);
                    }
                }

                if (pFilaRedireccion.RedireccionValorParametro != null)
                {
                    foreach (RedireccionValorParametro filaValorBD in pFilaRedireccion.RedireccionValorParametro)
                    {
                        if (pRedireccionVista.ParameterValues != null && pRedireccionVista.ParameterValues.Find(r => r.Value != null && r.Value.Equals(filaValorBD.ValorParametro)) == null)
                        {
                            listaBorrar.Add(filaValorBD);
                        }
                    }
                }
            }

            return listaBorrar;
        }

        #endregion
    }
}