using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.CMS;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para administrar una página del CMS
    /// </summary>
    public class AdministrarPaginasCMSController : ControllerBaseWeb
    {
        public AdministrarPaginasCMSController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarPaginasCMSViewModel mPaginaModel = null;
        /// <summary>
        /// 
        /// </summary>
        private GestionCMS mGestorCMS;
        /// <summary>
        /// 
        /// </summary>
        private Guid? mPestanyaID;
        /// <summary>
        /// 
        /// </summary>
        private static AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu mPestanya;
        #endregion

        private static int NUM_COMPONENTES_LAYOUT_CARGAR = 10;

        #region Metodos de eventos

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult Index()
        {
            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "edicion page-builder comunidad no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Estructura;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Estructura_Paginas_CMSBuilder;

            // Establecer el título para el header de DevTools                        
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "ESTRUCTURA");
            ViewBag.HeaderTitle = "Page Builder";

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            if (!ParametrosGeneralesRow.CMSDisponible)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADGENERAL"));
            }

            if (GestorCMSPaginaActual.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave) || GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave].ContainsKey((short)TipoUbicacionCMSPaginaActual))
            {
                return View(PaginaModel);
            }
            else
            {
                //Esta página ya no existe, le redirigimos a la administracion de páginas
                return Redirect(Comunidad.Url + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARPESTANYASCOM"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] {"", TipoPaginaAdministracion.Pagina })]
        public ActionResult Descartar()
        {
            GestionCMS gestorCMSDescartar = new GestionCMS(new DataWrapperCMS(), mLoggingService, mEntityContext);

            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, Guid> listaGuidViejoGuidNuevo = new Dictionary<Guid, Guid>();
            foreach (CMSBloque bloqueCMS in GestorCMSPaginaActual.ListaBloques.Values)
            {
                if (!bloqueCMS.Borrador)
                {
                    listaGuidViejoGuidNuevo.Add(bloqueCMS.Clave, Guid.NewGuid());
                }
            }

            foreach (CMSBloque bloqueCMS in GestorCMSPaginaActual.ListaBloques.Values)
            {
                if (!bloqueCMS.Borrador)
                {
                    Guid? bloquePadreID = null;
                    if (bloqueCMS.BloquePadreID.HasValue)
                    {
                        bloquePadreID = listaGuidViejoGuidNuevo[bloqueCMS.BloquePadreID.Value];
                    }
                    gestorCMSDescartar.AgregarNuevoBloque(listaGuidViejoGuidNuevo[bloqueCMS.Clave], GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)TipoUbicacionCMSPaginaActual], bloquePadreID, bloqueCMS.Orden, bloqueCMS.FilaBloque.Estilos, true);

                    foreach (CMSComponente componente in bloqueCMS.Componentes.Values)
                    {
                        gestorCMSDescartar.AgregarComponenteABloque(ProyectoSeleccionado, listaGuidViejoGuidNuevo[bloqueCMS.Clave], componente.Clave);
                    }
                }
            }

            cmsCN.ActualizarCMSEliminandoBloquesDePaginaDeProyecto(gestorCMSDescartar.CMSDW, ProyectoSeleccionado.Clave, TipoUbicacionCMSPaginaActual, true);
            cmsCN.Dispose();

            CMSCL cmsCL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            cmsCL.InvalidarCacheConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave);
            cmsCL.InvalidarCacheCMSDeUbicacionDeProyecto(TipoUbicacionCMSPaginaActual, ProyectoSeleccionado.Clave);
            cmsCL.Dispose();

            return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARPAGINA") + "/" + ((short)TipoUbicacionCMSPaginaActual).ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult GuardarBorrador(string Estructura, string OpcionesPropiedades, bool MostrarSoloCuerpo, DateTime FechaModificacion)
        {
            GuardarLogAuditoria();
            ControladorPaginasCMS contrPaginasCMS = new ControladorPaginasCMS(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            if (mPestanya != null)
            {
				string error = contrPaginasCMS.ComprobarErrorConcurrencia(FechaModificacion, mPestanya.FechaModificacion);
				if (!string.IsNullOrEmpty(error))
				{
					return GnossResultERROR(error);
				}
			}
			

			Guardar(Estructura, OpcionesPropiedades, MostrarSoloCuerpo);

            return GnossResultOK();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult Publicar(string Estructura, string OpcionesPropiedades, bool MostrarSoloCuerpo, DateTime FechaModificacion)
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
            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;
            try
            {
				ControladorPaginasCMS contrPaginasCMS = new ControladorPaginasCMS(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                if (mPestanya != null)
                {
					string error = contrPaginasCMS.ComprobarErrorConcurrencia(FechaModificacion, mPestanya.FechaModificacion);
					if (!string.IsNullOrEmpty(error))
					{
						return GnossResultERROR(error);
					}
				}

				mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);
               
                contrPaginasCMS.GuardarWeb(TipoUbicacionCMSPaginaActual, Estructura, OpcionesPropiedades, MostrarSoloCuerpo, false, mPestanya);

                if (iniciado)
                {
                    HttpResponseMessage resultado = InformarCambioAdministracion("PaginasCMS", JsonConvert.SerializeObject(PaginaModel, Formatting.Indented));

                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("No se ha podido guardar, ha habido un error al intentar registrar el cambio con la integración continua. Contacte con el administrador.");
                    }
                }

                contrPaginasCMS.InvalidarCache(TipoUbicacionCMSPaginaActual, false);
                if (transaccionIniciada)
                {
                    mEntityContext.TerminarTransaccionesPendientes(true);
                }
                ControladorCMS controlador = new ControladorCMS(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD);
                controlador.ActualizarModeloBaseSimple(mPestanya.PestanyaID, ProyectoSeleccionado.Clave, AD.BASE_BD.PrioridadBase.Alta, false);

            }
            catch (Exception ex)
            {
                if (transaccionIniciada)
                {
                    proyAD.TerminarTransaccion(false);
                }

                return GnossResultERROR(ex.Message);
            }

            return GnossResultOK();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult VistaPrevia(string Estructura, string OpcionesPropiedades, bool MostrarSoloCuerpo, DateTime FechaModificacion)
        {
			ControladorPaginasCMS contrPaginasCMS = new ControladorPaginasCMS(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            if (mPestanya != null)
            {
				string error = contrPaginasCMS.ComprobarErrorConcurrencia(FechaModificacion, mPestanya.FechaModificacion);
				if (!string.IsNullOrEmpty(error))
				{
					return GnossResultERROR(error);
				}
			}
			

			Guardar(Estructura, OpcionesPropiedades, MostrarSoloCuerpo);

            string enlaceVistaPrevia = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/CMSPagina?PaginaCMS=" + TipoUbicacionCMSPaginaActual.ToString() + "&preView=true";
            return GnossResultUrl(enlaceVistaPrevia);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult CrearComponente(string Estructura, string OpcionesPropiedades, bool MostrarSoloCuerpo, string TipoComponente, string IDContenedor)
        {
            Guardar(Estructura, OpcionesPropiedades, MostrarSoloCuerpo);

            string enlaceEdicion = mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSNUEVOCOMPONENTE") + "/" + TipoComponente + "/" + IDContenedor;
            return GnossResultUrl(enlaceEdicion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult SeleccionarComponente(string Estructura, string OpcionesPropiedades, bool MostrarSoloCuerpo, string TipoComponente, string IDContenedor)
        {
            Guardar(Estructura, OpcionesPropiedades, MostrarSoloCuerpo);

            string enlaceEdicion = $"{mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSLISTADOCOMPONENTES")}/{TipoComponente}/{IDContenedor}";
            return GnossResultUrl(enlaceEdicion);
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult BuscarComponente(string search)
        {
            ControladorPaginasCMS contrPaginasCMS = new ControladorPaginasCMS(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            List<AdministrarPaginasCMSViewModel.CMSComponentModel> listaComponentesBuscados = contrPaginasCMS.CargarComponentesComunidad(UtilIdiomas, NUM_COMPONENTES_LAYOUT_CARGAR, search);

            return PartialView("../Shared/Layout/_partial-views/_layout-left-cms-dinamic-components-admin", listaComponentesBuscados);
        }

        private void Guardar(string estructura, string propiedadComponente, bool MostrarSoloCuerpo, bool borrador = true)
        {
            ControladorPaginasCMS contrPaginasCMS = new ControladorPaginasCMS(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            contrPaginasCMS.GuardarWeb(TipoUbicacionCMSPaginaActual, estructura, propiedadComponente, MostrarSoloCuerpo, borrador, mPestanya);

            contrPaginasCMS.InvalidarCache(TipoUbicacionCMSPaginaActual, borrador);

        }

        #endregion

        #region Propiedades


        private AdministrarPaginasCMSViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    ControladorPaginasCMS contrPaginasCMS = new ControladorPaginasCMS(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    mPaginaModel = contrPaginasCMS.CargarPagina(TipoUbicacionCMSPaginaActual, GestorCMSPaginaActual);
                    mPaginaModel.ListaComponenteComunidad = contrPaginasCMS.CargarComponentesComunidad(UtilIdiomas, NUM_COMPONENTES_LAYOUT_CARGAR);
                    mPaginaModel.ContieneMultiplesComponentes = contrPaginasCMS.BuscarScomponentesConPeticionAjax(UtilIdiomas, NUM_COMPONENTES_LAYOUT_CARGAR);
                    
                    if (PestanyaID.HasValue)
                    {
                        mPaginaModel.Key = PestanyaID.Value;
                        mPestanya = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.FirstOrDefault(pest => pest.PestanyaID.Equals(mPaginaModel.Key));
						mPaginaModel.FechaModificacion = mPestanya.FechaModificacion;
					}

                    CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestionCMS gestorCMSSinFiltros = new GestionCMS(CMSCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);

                    mPaginaModel.ListaComponentesPrivados = gestorCMSSinFiltros.ListaComponentesPrivadosProyecto;
                }

                return mPaginaModel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public GestionCMS GestorCMSPaginaActual
        {
            get
            {
                if (mGestorCMS == null)
                {
                    CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mGestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeUbicacionDeProyecto(TipoUbicacionCMSPaginaActual, ProyectoSeleccionado.Clave, 0, false), mLoggingService, mEntityContext);
                    cmsCN.Dispose();
                }
                return mGestorCMS;
            }
        }

        private short? mTipoUbicacionCMSPaginaActual = null;

        /// <summary>
        /// 
        /// </summary>
        public short TipoUbicacionCMSPaginaActual
        {
            get
            {
                if (!mTipoUbicacionCMSPaginaActual.HasValue)
                {
                    short tipoPaginaShort;
                    Guid tipoPaginaGuid;
                    if (short.TryParse(RequestParams("tipoPagina"), out tipoPaginaShort))
                    {
                        mTipoUbicacionCMSPaginaActual = tipoPaginaShort;
                        return tipoPaginaShort;
                    }
                    else if (Guid.TryParse(RequestParams("tipoPagina"), out tipoPaginaGuid) && PestanyaID.HasValue)
                    {
                        var filaPestanya = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaCMS.FirstOrDefault(pest => pest.PestanyaID == PestanyaID.Value);
                        if (filaPestanya != null)
                        {
                            mTipoUbicacionCMSPaginaActual = filaPestanya.Ubicacion;
                            return filaPestanya.Ubicacion;
                        }
                    }

                    RedireccionarAPaginaNoEncontrada();
                }

                return mTipoUbicacionCMSPaginaActual.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid? PestanyaID
        {
            get
            {
                if (!mPestanyaID.HasValue)
                {
                    Guid idPestanya = Guid.Empty;
                    if (Guid.TryParse(RequestParams("tipoPagina"), out idPestanya))
                    {
                        mPestanyaID = idPestanya;
                    }
                    else
                    {
                        foreach (ProyectoPestanyaCMS filaPestanya in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaCMS)
                        {
                            if (filaPestanya.Ubicacion == TipoUbicacionCMSPaginaActual)
                            {
                                mPestanyaID = filaPestanya.PestanyaID;
                                break;
                            }
                        }
                        if (!mPestanyaID.HasValue)
                        {
                            mPestanyaID = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(pest => pest.TipoPestanya == (short)TipoPestanyaMenu.Home).FirstOrDefault().PestanyaID;
                        }
                    }
                }
                return mPestanyaID;
            }
        }

        #endregion
    }
}