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
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
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

            /// <summary>
            /// Fecha de creación de la redirección
            /// </summary>
            public DateTime FechaCreacion { get; set; }

            /// <summary>
            /// Indicará si la redirección es de reciente creación o por el contrario ya existía.
            /// </summary>
            public bool EsRecienteCreacion { get; set; }

            /// <summary>
            /// Indicará si la redirección está siendo editada/creada de 0.
            /// </summary>
            public bool Edited { get; set; }
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

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "estructura edicionRedirecciones edicion no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_Redirecciones;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTRUCTURA");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONPAGINAS", "REDIRECCIONES");
            

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
            // Se está creando una nueva redirección
            redireccion.EsRecienteCreacion = true;

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
            if (ListaRedirecciones == null || ListaRedirecciones.Count == 0)
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
                try
                {
					GuardarRedirecciones(ListaRedirecciones);
				}
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    return GnossResultERROR(UtilIdiomas.GetText("COMADMINREDIRECCIONES", "ERRORGUARDARBD"));
                }
                finally
                {

                }

                return GnossResultOK();
            }
            else
            {
                return GnossResultERROR(errores);
            }
        }

        
        /// <summary>
        /// Devolver la vista modal para confirmar la eliminación de un item de tipo Redirección.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CargarEliminarRedireccionItem()
        {
            ActionResult partialView = View();

            // Construir la vista que se devolverá
            partialView = GnossResultHtml("_modal-views/_delete-redirection-item", null);

            // Devolver la vista modal
            return partialView;
        }



        #endregion

        #region Propiedades

        private ManageRedirectionsViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new ManageRedirectionsViewModel();
                    mPaginaModel.RedirectionsList = new List<ManageRedirectionsViewModel.RedirectionModel>();

                    string dominio = ProyectoSeleccionado.UrlPropia(IdiomaUsuario).Replace("http://", "").Replace("https://", "");
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<RedireccionRegistroRuta> listaRedirecciones = proyCN.ObtenerRedireccionRegistroRutaPorDominio(dominio, false);

                    if (listaRedirecciones != null && listaRedirecciones.Count > 0)
                    {
                        foreach (RedireccionRegistroRuta redireccion in listaRedirecciones.OrderByDescending(r => r.FechaCreacion))
                        {
                            ManageRedirectionsViewModel.RedirectionModel modeloRed = new ManageRedirectionsViewModel.RedirectionModel();
                            modeloRed.Key = redireccion.RedireccionID;
                            modeloRed.OriginalUrl = redireccion.UrlOrigen;
                            modeloRed.ParameterName = redireccion.NombreParametro;
                            // Indicar que el filtro ya existe
                            modeloRed.EsRecienteCreacion = false;
                            modeloRed.FechaCreacion = redireccion.FechaCreacion;

                            if (string.IsNullOrEmpty(redireccion.NombreParametro) && redireccion.RedireccionValorParametro != null && redireccion.RedireccionValorParametro.Count > 0)
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

        private void GuardarRedirecciones(List<ManageRedirectionsViewModel.RedirectionModel> pListaRedireccionesVista)
        {           
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            try
            {
                string dominio = ProyectoSeleccionado.UrlPropia(IdiomaUsuario).Replace("http://", "").Replace("https://", "");
                List<RedireccionRegistroRuta> listaRedireccionesBD = proyCN.ObtenerRedireccionRegistroRutaPorDominio(dominio, true);

                foreach (ManageRedirectionsViewModel.RedirectionModel redireccionVista in pListaRedireccionesVista)
                {
                    RedireccionRegistroRuta redireccionRegistroRuta = listaRedireccionesBD.Where(item => item.RedireccionID.Equals(redireccionVista.Key)).FirstOrDefault();
                    if (redireccionVista.DeleteRedirection)
                    {
                        proyCN.BorrarRedireccionRegistroRuta(redireccionVista.Key);
                    }
                    else
                    {
                        bool esNueva = false;
                        if (redireccionRegistroRuta == null)
                        {
                            redireccionRegistroRuta = new RedireccionRegistroRuta();
                            redireccionRegistroRuta.FechaCreacion = DateTime.Now;
                            redireccionRegistroRuta.RedireccionID = Guid.NewGuid();
                            redireccionVista.Key = redireccionRegistroRuta.RedireccionID;
                            esNueva = true;
                        }

                        redireccionRegistroRuta.UrlOrigen = redireccionVista.OriginalUrl;
                        redireccionRegistroRuta.Dominio = dominio;
                        if(redireccionVista.ParameterValues == null || redireccionVista.ParameterValues.Count == 0)
                        {
                            redireccionRegistroRuta.NombreParametro = string.Empty;
                        }
                        else
                        {
                            redireccionRegistroRuta.NombreParametro = redireccionVista.ParameterName;
                        }

                        if (redireccionVista.Edited)
                        {
                            redireccionRegistroRuta.FechaCreacion = DateTime.Now;
                        }
                        
                        if (esNueva)
                        {
                            proyCN.AniadirRedireccionRegistroRuta(redireccionRegistroRuta);
                        }

                        ActualizarFilasValorParametro(redireccionVista, proyCN);
                    }
                }

                proyCN.Actualizar();
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

        private void ActualizarFilasValorParametro(ManageRedirectionsViewModel.RedirectionModel pRedireccionRegistroRuta, ProyectoCN pProyectoCN)
        {
            //Obtengo todas las RedireccionesValorParametro para esta RedirecciónID
            List<RedireccionValorParametro> listaRedireccionesValorParametroBD = pProyectoCN.ObtenerRedireccionValorParametroPorRedireccionID(pRedireccionRegistroRuta.Key);

            if (pRedireccionRegistroRuta.ParameterValues != null && pRedireccionRegistroRuta.ParameterValues.Count > 0)
            {
                //Recorro todas las redireccionesValorParametro indicadas en la vista para esta redirección
                foreach (ManageRedirectionsViewModel.ParameterValue parametroValorVista in pRedireccionRegistroRuta.ParameterValues)
                {
                    bool esNueva = false;
                    RedireccionValorParametro redireccionValorParametroBD = listaRedireccionesValorParametroBD.Where(item => item.ValorParametro.Equals(parametroValorVista.Value)).FirstOrDefault();

                    if (redireccionValorParametroBD == null)
                    {
                        redireccionValorParametroBD = new RedireccionValorParametro();
                        redireccionValorParametroBD.RedireccionID = pRedireccionRegistroRuta.Key;
                        redireccionValorParametroBD.ValorParametro = parametroValorVista.Value;
                        esNueva = true;
                    }

                    redireccionValorParametroBD.UrlRedireccion = parametroValorVista.DestinationUrl;
                    redireccionValorParametroBD.MantenerFiltros = pRedireccionRegistroRuta.PreserveFilters;
                    redireccionValorParametroBD.OrdenPresentacion = (short)parametroValorVista.Order;

                    if (esNueva)
                    {
                        //Si es nueva la añado a la base de datos
                        pProyectoCN.AniadirRedireccionValorParametro(redireccionValorParametroBD);
                    }
                    else
                    {
                        //Si no es nueva la quito de la lista de todas las existentes, pues ya la he modificado (No se esta eliminando de BD, solo de la lista)
                        listaRedireccionesValorParametroBD.Remove(redireccionValorParametroBD);
                    }
                }
            }
            else
            {
                RedireccionValorParametro redireccionDirecta = listaRedireccionesValorParametroBD.Where(item => string.IsNullOrEmpty(item.ValorParametro)).FirstOrDefault();
                if (redireccionDirecta != null)
                {
                    //Si existe no se marca para eliminar posteriormente
                    redireccionDirecta.UrlRedireccion = pRedireccionRegistroRuta.DestinationUrl;
                    listaRedireccionesValorParametroBD.Remove(redireccionDirecta);
                }
                else
                {
                    //Si no existe se crea
                    redireccionDirecta = new RedireccionValorParametro();
                    redireccionDirecta.ValorParametro = string.Empty;
                    redireccionDirecta.OrdenPresentacion = 0;
                    redireccionDirecta.MantenerFiltros = false;
                    redireccionDirecta.RedireccionID = pRedireccionRegistroRuta.Key;
                    redireccionDirecta.UrlRedireccion = pRedireccionRegistroRuta.DestinationUrl;
                    pProyectoCN.AniadirRedireccionValorParametro(redireccionDirecta);
                }
            }

            //Elimino de BD las que quedan en la lista, pues son las que ni son nuevas ni han llegado de la vista (Se han eliminado)
            pProyectoCN.BorrarFilasRedireccionValorParametro(listaRedireccionesValorParametroBD, true);
        }

        /// <summary>
        /// Añade y modifica las FilasValorParametro. Retorna una lista con las filas que hay que borrar 
        /// </summary>
        /// <param name="pRedireccionBD"></param>
        /// <param name="pRedireccionVista"></param>
        /// <returns></returns>
        private List<RedireccionValorParametro> ComprobarFilasValorParametro(RedireccionRegistroRuta pRedireccionBD, ManageRedirectionsViewModel.RedirectionModel pRedireccionVista)
        {
            List<RedireccionValorParametro> listaBorrar = new List<RedireccionValorParametro>();

            if (pRedireccionBD.RedireccionValorParametro == null)
            {
                pRedireccionBD.RedireccionValorParametro = new List<RedireccionValorParametro>();
            }

            //es redirección directa
            if (!string.IsNullOrEmpty(pRedireccionVista.DestinationUrl))
            {
                RedireccionValorParametro filaValorParam = null;
                if (pRedireccionBD.RedireccionValorParametro != null && pRedireccionBD.RedireccionValorParametro.Count > 0 && pRedireccionBD.RedireccionValorParametro.FirstOrDefault(r => r.RedireccionID.Equals(pRedireccionBD.RedireccionID) && r.ValorParametro.Equals(string.Empty)) != null)
                {
                    filaValorParam = pRedireccionBD.RedireccionValorParametro.First(r => r.RedireccionID.Equals(pRedireccionBD.RedireccionID));
                }
                else
                {
                    filaValorParam = mEntityContext.RedireccionValorParametro.FirstOrDefault(item => item.RedireccionID.Equals(pRedireccionBD.RedireccionID));
                    if (filaValorParam == null)
                    {
                        filaValorParam = new RedireccionValorParametro();
                    }
                }

                filaValorParam.RedireccionRegistroRuta = pRedireccionBD;
                filaValorParam.RedireccionID = pRedireccionVista.Key;
                filaValorParam.ValorParametro = string.Empty;
                filaValorParam.MantenerFiltros = pRedireccionVista.PreserveFilters;
                filaValorParam.UrlRedireccion = pRedireccionVista.DestinationUrl;
                filaValorParam.OrdenPresentacion = 0;
                //pFilaRedireccion.RedireccionValorParametro.Add(filaValorParam);

                //hay que borrar las hijas que han cambiado de tipo
                foreach (RedireccionValorParametro filaValorBD in pRedireccionBD.RedireccionValorParametro.Where(r => !r.ValorParametro.Equals(string.Empty)))
                {
                    listaBorrar.Add(filaValorBD);
                }
            }
            else //es redirección parametrizada
            {
                pRedireccionBD.NombreParametro = pRedireccionVista.ParameterName;

                if (pRedireccionVista.ParameterValues != null && pRedireccionVista.ParameterValues.Count > 0)
                {
                    short orden = 0;
                    foreach (ManageRedirectionsViewModel.ParameterValue parametroValor in pRedireccionVista.ParameterValues)
                    {
                        RedireccionValorParametro filaValorParam = null;
                        if (pRedireccionBD.RedireccionValorParametro != null && pRedireccionBD.RedireccionValorParametro.Count > 0 && pRedireccionBD.RedireccionValorParametro.Any(r => r.RedireccionID.Equals(pRedireccionBD.RedireccionID) && r.ValorParametro != null && r.ValorParametro.Equals(parametroValor.Value)))
                        {
                            filaValorParam = pRedireccionBD.RedireccionValorParametro.First(r => r.RedireccionID.Equals(pRedireccionBD.RedireccionID) && r.ValorParametro.Equals(parametroValor.Value));
                        }

                        if (filaValorParam == null)
                        {
                            filaValorParam = new RedireccionValorParametro();
                        }
                        filaValorParam.RedireccionRegistroRuta = pRedireccionBD;
                        filaValorParam.RedireccionID = pRedireccionVista.Key;
                        filaValorParam.ValorParametro = parametroValor.Value;
                        filaValorParam.UrlRedireccion = parametroValor.DestinationUrl;
                        filaValorParam.MantenerFiltros = false;
                        filaValorParam.OrdenPresentacion = orden;
                        pRedireccionBD.RedireccionValorParametro.Add(filaValorParam);

                        orden++;
                    }

                    //hay que borrar las hijas que han cambiado de tipo
                    foreach (RedireccionValorParametro filaValorBD in pRedireccionBD.RedireccionValorParametro.Where(r => r.ValorParametro.Equals(string.Empty)))
                    {
                        listaBorrar.Add(filaValorBD);
                    }
                }

                if (pRedireccionBD.RedireccionValorParametro != null)
                {
                    foreach (RedireccionValorParametro filaValorBD in pRedireccionBD.RedireccionValorParametro)
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