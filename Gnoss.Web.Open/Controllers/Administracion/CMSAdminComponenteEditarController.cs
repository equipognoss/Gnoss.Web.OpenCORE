using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Flujos;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Flujos;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    public class CMSAdminComponenteEditarController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public CMSAdminComponenteEditarController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<CMSAdminComponenteEditarController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;

        }

        #region Miembros

        private CMSAdminComponenteEditarViewModel mPaginaModel = null;

        private CMSComponente mCMSComponente = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.VerComponenteCMS, (ulong)PermisoContenidos.EditarComponenteCMS } })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            CargarPermisosCMSViewBag();

            if (!ParametrosGeneralesRow.CMSDisponible)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADGENERAL"));
            }

            return View(PaginaModel);
        }

        /// <summary>
        /// Método que cargará la información del componente CMS y lo insertará en una vista modal para su edición/revisión.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CargarModal()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            CargarPermisosCMSViewBag();

            // Construir el modelo
            ActionResult partialView = GnossResultHtml("_modal-views/_index", PaginaModel);

            // Devolver la vista modal
            return partialView;
        }

        /// <summary>
        /// Devolver la vista modal para confirmar la eliminación de un Componente CMS.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CargarEliminarComponenteItem()
        {
			CargarPermisosCMSViewBag();

            // Construir la vista que se devolverá
            ActionResult partialView = GnossResultHtml("_modal-views/_delete-component-item", null);

            // Devolver la vista modal
            return partialView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.EditarComponenteCMS, (ulong)PermisoContenidos.CrearComponenteCMS } })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Guardar(CMSAdminComponenteEditarViewModel Componente)
        {
            return GuardarComponente(Componente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ComprobarLista(string[] listaIDs)
        {
            CargarPermisosCMSViewBag();
            List<CMSAdminComponenteEditarCheckListViewModel> resultadoComprobacion = new List<CMSAdminComponenteEditarCheckListViewModel>();

            string errorNoExiste = "";
            if (TipoComponenteCMSActual == TipoComponenteCMS.GrupoComponentes)
            {
                errorNoExiste = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOEXISTECOMPONENTE");
            }
            else if (TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
            {
                errorNoExiste = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOEXISTEPROYECTO");
            }
            else
            {
                errorNoExiste = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOEXISTERECURSO");
            }

            List<Guid> listaIdentificadores = new List<Guid>();

            foreach (string id in listaIDs)
            {
                Guid idRecurso = Guid.Empty;
                string error = string.Empty;

                if (string.IsNullOrEmpty(id))
                {
                    error = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOVACIO");
                }
                else
                {
                    try
                    {
                        if (TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

                            if (Guid.TryParse(id, out idRecurso))
                            {
                                if (!proyCN.ExisteProyectoConID(idRecurso))
                                {
                                    idRecurso = Guid.Empty;
                                }
                            }
                            else
                            {
                                idRecurso = proyCN.ObtenerProyectoIDPorNombre(id);
                            }

                            proyCN.Dispose();

                            if (idRecurso.Equals(Guid.Empty))
                            {
                                mLoggingService.GuardarLogError($"El proyecto {id} no existe.", mlogger);
                                throw new ExcepcionWeb($"El proyecto {id} no existe.");
                            }
                        }
                        else
                        {
                            idRecurso = new Guid(id);
                        }

                        if (listaIdentificadores.Contains(idRecurso))
                        {
                            error = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOREPETIDO");
                        }
                        else
                        {
                            listaIdentificadores.Add(idRecurso);
                        }
                    }
                    catch (Exception)
                    {
                        error = errorNoExiste;
                        mLoggingService.GuardarLogError($"{error}. ID: {id}", mlogger);
                    }
                }

                CMSAdminComponenteEditarCheckListViewModel datosResultado = new CMSAdminComponenteEditarCheckListViewModel();
                datosResultado.Orden = resultadoComprobacion.Count;
                datosResultado.Identificador = idRecurso;
                datosResultado.Error = error;
                resultadoComprobacion.Add(datosResultado);
            }

            if (TipoComponenteCMSActual == TipoComponenteCMS.GrupoComponentes)
            {
                bool cargadoResultado = CargarResultadosComponentesComponenteCMS(listaIdentificadores, resultadoComprobacion);
                
                if(!cargadoResultado)
                {
                    CargarResultadosRecursosComponenteCMS(listaIdentificadores, resultadoComprobacion);
                }
            }
            else if (TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
            {
                CargarResultadosProyectosComponenteCMS(listaIdentificadores, resultadoComprobacion);
            }
            else
            {
                CargarResultadosRecursosComponenteCMS(listaIdentificadores, resultadoComprobacion);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
            };
            var resultado = Json(resultadoComprobacion, options);
            return resultado;
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idComponente"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.EliminarComponenteCMS } })]
        public ActionResult Delete(string idComponente)
        {
			CargarPermisosCMSViewBag();
			GuardarLogAuditoria();
            Guid ComponenteID;

            if (Guid.TryParse(idComponente, out ComponenteID) && !ComponenteID.Equals(Guid.Empty))
            {
                try
                {
                    using (CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
                    using (GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerComponentePorID(ComponenteID, ProyectoSeleccionado.Clave, false), mLoggingService, mEntityContext))
                    {
                        ControladorComponenteCMS contrCMS = new ControladorComponenteCMS(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorComponenteCMS>(), mLoggerFactory);
                        if (!cmsCN.ComponenteCMSPerteneceGrupo(ComponenteID))
                        {
                            contrCMS.BorrarComponenteCrearFilasIntegracionContinua(ComponenteID, cmsCN, gestorCMS);
                            CMSAdminComponenteEditarViewModel componente = null;
                            InformarCambioAdministracion("ComponentesCMS", JsonConvert.SerializeObject(new KeyValuePair<Guid, CMSAdminComponenteEditarViewModel>(ComponenteID, componente), Formatting.Indented));
                            return GnossResultOK();
                        }
                        else
                        {
                            throw new ErrorComponentePerteneceGrupoComponentes(UtilIdiomas.GetText("COMADMINCMS", "ELIMINARCOMPONENTEPERTENECEGRUPOCOMPONENTES"));
                        }
                    }
                }
                catch (ErrorComponenteVinculadoPagina ex)
                {
                    return GnossResultERROR(ex.Message);
                }
                catch (ErrorComponentePerteneceGrupoComponentes ex)
                {
                    return GnossResultERROR(ex.Message);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    return GnossResultERROR();
                }
            }

            return GnossResultERROR();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idComponente"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Select(string idComponente)
        {
            CargarPermisosCMSViewBag();

			string idBloqueContenedor = RequestParams("idBloqueContenedor");

            if (!string.IsNullOrEmpty(idBloqueContenedor) && !string.IsNullOrEmpty(idComponente))
            {
                Guid bloqueID;
                Guid ComponenteID;

                if (Guid.TryParse(idBloqueContenedor, out bloqueID) && !bloqueID.Equals(Guid.Empty) && Guid.TryParse(idComponente, out ComponenteID) && !ComponenteID.Equals(Guid.Empty))
                {
                    CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
                    GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    gestorCMS.AgregarComponenteABloque(ProyectoSeleccionado, bloqueID, ComponenteID);
                    cmsCN.ActualizarCMS(gestorCMS.CMSDW);
                    cmsCN.Dispose();

                    return GnossResultUrl($"{mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARPAGINA")}/{gestorCMS.ListaBloques[bloqueID].TipoUbicacion}");
                }
            }

            return GnossResultERROR();
        }

        [HttpPost]
		[TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.RestaurarVersionCMS, (ulong)PermisoContenidos.EliminarVersionCMS } })]
		public ActionResult Restore(Guid idComponente, Guid versionIdComponente)
        {
            CMSAdminComponenteEditarViewModel componenteRestaurar;

            using (CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
            {
                componenteRestaurar = JsonConvert.DeserializeObject<CMSAdminComponenteEditarViewModel>(CMSCN.ObtenerVersionComponenteCMS(idComponente, versionIdComponente).ModeloJSON);
                componenteRestaurar.FechaModificacion = DateTime.Now;
            }

            return GuardarComponente(componenteRestaurar, RequestParams("comentario"));
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.RestaurarVersionCMS, (ulong)PermisoContenidos.EliminarVersionCMS } })]
        public ActionResult DeleteVersion(Guid idComponente, Guid versionIdComponente)
        {
            try
            {
                ControladorComponenteCMS controladorComponenteCMS = new ControladorComponenteCMS(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorComponenteCMS>(), mLoggerFactory);

                controladorComponenteCMS.EliminarVersionComponente(idComponente, versionIdComponente);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"ERROR: ${ex.Message}\r\nStackTrace: {ex.StackTrace}", mlogger);
                return GnossResultERROR(UtilIdiomas.GetText("DEVTOOLS", "ERRORELIMINARVERSIONCMS"));
            }

            return GnossResultOK(UtilIdiomas.GetText("DEVTOOLS", "VERSIONCMSELIMINADO"));
        }

        [HttpPost]
		[TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.RestaurarVersionCMS, (ulong)PermisoContenidos.EditarComponenteCMS } })]
		public ActionResult AddCommentVersion(Guid pComponenteID, Guid pVersionID)
        {
            CMSAdminComponenteVersionViewModel modelo = new CMSAdminComponenteVersionViewModel();
            modelo.ComponenteID = pComponenteID;
            modelo.VersionID = pVersionID;
            modelo.Autor = IdentidadActual.Nombre();
            return GnossResultHtml("_modal-views/_add-version-comment", modelo);
        }

		public ActionResult ObtenerHistorialTransiciones(Guid pComponenteID)
        {
            try
            {
				UtilFlujos utilFlujos = new UtilFlujos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilFlujos>(), mLoggerFactory);
				CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
				CMSAdminHistorialTransicionesViewModel modelo = new CMSAdminHistorialTransicionesViewModel();

				modelo.ListaTransiciones = utilFlujos.ObtenerHistorialDeTransiciones(pComponenteID, TipoContenidoFlujo.ComponenteCMS, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                modelo.ComponenteID = pComponenteID;
                modelo.NombreComponente = UtilCadenas.ObtenerTextoDeIdioma(cmsCN.ObtenerNombreComponentePorIDComponenteEnProyecto(pComponenteID, ProyectoSeleccionado.Clave), UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                modelo.Fecha = cmsCN.ObtenerFechaModificacionDeComponenteEnProyecto(pComponenteID, ProyectoSeleccionado.Clave);
                cmsCN.Dispose();
                
				return PartialView("_modal-views/_transition-history", modelo);
            }
            catch(Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "HISTORIALERROR"));
            }
        }

		public ActionResult CargarModalRealizarTransicion(Guid pTransicionID, Guid pComponenteID)
        {
            try
            {
                CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
                CMSAdminCambiarEstadoViewModel model = new CMSAdminCambiarEstadoViewModel();
                model.TransicionID = pTransicionID;
                model.ComponenteID = pComponenteID;
                model.Nombre = UtilCadenas.ObtenerTextoDeIdioma(cmsCN.ObtenerNombreComponentePorIDComponenteEnProyecto(pComponenteID, ProyectoSeleccionado.Clave), UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                cmsCN.Dispose();

				return PartialView("_modal-views/_change-state", model);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return GnossResultERROR();
            }
		}

		public ActionResult RealizarTransicion(Guid pContenidoID, Guid pTransicionID, string pComentario)
        {
            try
            {
                UtilFlujos utilFlujos = new UtilFlujos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilFlujos>(), mLoggerFactory);
                EstadoModel estadoDocumento = utilFlujos.ObtenerEstadoDeContenido(CMSComponente.Estado.Value, IdentidadActual.Clave, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
				bool tienePermiso = estadoDocumento.Transiciones.Any(t => t.TransicionID.Equals(pTransicionID));

				if (tienePermiso)
				{
					FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
					FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, ProyectoSeleccionado.Clave.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
					NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
					DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
					GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);

					pComentario = pComentario ?? "";

					// Cambiar estado en base de datos y añadir al historial
					Guid estadoDestino = flujosCN.ObtenerEstadoDestinoTransicion(pTransicionID);
					flujosCN.CambiarEstadoContenido(pContenidoID, estadoDestino, TipoContenidoFlujo.ComponenteCMS);
					flujosCN.GuardarHistorialTransicionComponenteCMS(pContenidoID, pTransicionID, pComentario, IdentidadActual.Clave);

					// Cambiar estado en Virtuoso                    
					facetadoCN.ModificarEstadoDeContenido(ProyectoSeleccionado.Clave, estadoDocumento.EstadoID, estadoDestino, pContenidoID);

					// Enviar correo de aviso a lectores y editores de los estados de origen y destino					
					string urlComponente = $"{mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSLISTADOCOMPONENTES")}";
                    gestorNotificaciones.EnviarCorreoAvisoCambioDeEstado(pTransicionID, ProyectoSeleccionado.Clave, pComentario, urlComponente, UtilCadenas.ObtenerTextoDeIdioma(CMSComponente.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto), IdentidadActual.Nombre());
					notificacionCN.ActualizarNotificacion(mAvailableServices);

					notificacionCN.Dispose();
					facetadoCN.Dispose();
					flujosCN.Dispose();
				}

				return GnossResultOK(UtilIdiomas.GetText("FLUJOS", "TRANSICIONREALIZADA"));
			}
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
				return GnossResultERROR(UtilIdiomas.GetText("FLUJOS", "TRANSICIONERROR"));
			}
        }

        private ActionResult GuardarComponente(CMSAdminComponenteEditarViewModel pComponente, string pComentario = null)
        {
            GuardarLogAuditoria();
            string error = string.Empty;

            try
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

                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = false;

                GestionCMS gestorCMS;
                CMSComponente componenteEdicion = CMSComponente;

                if (esEdicion)
                {
                    gestorCMS = CMSComponente.GestorCMS;

                    componenteEdicion.Nombre = pComponente.Name;
                    componenteEdicion.Estilos = pComponente.Styles;
                    componenteEdicion.Activo = pComponente.Active;
                    componenteEdicion.TipoCaducidadComponenteCMS = pComponente.CaducidadSeleccionada;
                }
                else
                {
                    using (CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
                    {
                        gestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    }

                    componenteEdicion = gestorCMS.AgregarNuevoComponente(ProyectoSeleccionado, pComponente.Name, pComponente.Styles, pComponente.Active, (short)TipoComponenteCMSActual, (short)pComponente.CaducidadSeleccionada, new Dictionary<TipoPropiedadCMS, string>(), false);
					
                    Guid? estadoInicial = null;
					using (FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory))
					{
						estadoInicial = flujosCN.ObtenerEstadoInicialDeTipoContenido(ProyectoSeleccionado.Clave, TiposContenidos.ComponenteCMS);
						if (estadoInicial.HasValue)
						{
							componenteEdicion.FilaComponente.EstadoID = estadoInicial.Value;
						}
					}

					using (FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory))
					{
						if (estadoInicial.HasValue)
						{
							facetadoCN.AnyadirEstadoDeContenido(ProyectoSeleccionado.Clave, estadoInicial.Value, componenteEdicion.Clave);
						}
						facetadoCN.InsertarTripleRdfTypeDeContenido(ProyectoSeleccionado.Clave, componenteEdicion.Clave, "ComponenteCMS");
					}
				}

                pComponente.Type = TipoComponenteCMSActual;
                // Para IC Notificacion al repositorio.
                Dictionary<TipoPropiedadCMS, bool> propiedadesComponente = UtilComponentes.PropiedadesDisponiblesPorTipoComponente[TipoComponenteCMSActual];
                if (pComponente.Properties.Count > 0)
                {
                    foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in pComponente.Properties)
                    {
                        propiedad.TypeComponent = TipoComponenteCMSActual;
                        CMSAdminComponenteEditarViewModel.PropiedadComponente propiedadBIS = ObtenerPropiedad(propiedad.TipoPropiedadCMS, propiedadesComponente);
                        propiedad.Required = propiedadBIS.Required;
                        propiedad.MultiLang = propiedadBIS.MultiLang;
                    }
                }

                pComponente.Caducidades = new Dictionary<TipoCaducidadComponenteCMS, bool>();
                if (UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponenteCMSActual].Count > 1)
                {
                    foreach (TipoCaducidadComponenteCMS caducidad in UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponenteCMSActual])
                    {
                        bool selected = false;

                        if (CMSComponente != null && CMSComponente.TipoCaducidadComponenteCMS == caducidad)
                        {
                            selected = true;
                            pComponente.CaducidadSeleccionada = caducidad;
                        }

                        pComponente.Caducidades.Add(caducidad, selected);
                    }
                }

                if (pComponente.Styles == null)
                {
                    pComponente.Styles = string.Empty;
                }

                if (string.IsNullOrEmpty(pComponente.ShortName) || pComponente.ShortName.Trim().Equals(string.Empty))
                {
                    pComponente.ShortName = componenteEdicion.Clave.ToString();
                }

                if (gestorCMS.CMSDW.ListaCMSComponente.FirstOrDefault(filaComponente => filaComponente.NombreCortoComponente == pComponente.ShortName && filaComponente.ComponenteID != componenteEdicion.Clave) != null)
                {
                    // Si hay otro componente con el mismo nombre corto
                    error = $"<p>{UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOMBRECORTOREPETIDO")}</p>";
                    return GnossResultERROR(error);
                }

                ControladorComponenteCMS contrComponente = new ControladorComponenteCMS(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorComponenteCMS>(), mLoggerFactory, HayIntegracionContinua);

                contrComponente.AgregarPropiedadesComponente(componenteEdicion, pComponente, gestorCMS, UrlIntragnossServicios, BaseURLContent);

                if (esEdicion)
                {
                    error = contrComponente.ComprobarErrorConcurrencia(pComponente, componenteEdicion);
                }

                if (string.IsNullOrEmpty(error))
                {
                    Guid componenteIDparaRefresecar = componenteEdicion.Clave;

                    if (bloqueContenedor.HasValue)
                    {
                        gestorCMS.AgregarComponenteABloque(ProyectoSeleccionado, bloqueContenedor.Value, componenteIDparaRefresecar);
                    }

                    try
                    {
                        mEntityContext.NoConfirmarTransacciones = true;
                        transaccionIniciada = proyAD.IniciarTransaccion(true);

                        contrComponente.GuardarComponente(componenteEdicion, pComponente, mAvailableServices);

                        if (iniciado)
                        {
                            foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in pComponente.Properties.Where(x => x.TipoPropiedadCMS.Equals(TipoPropiedadCMS.Imagen)))
                            {
                                string valorPropiedad = HttpUtility.UrlDecode(propiedad.Value);
                                ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                                foreach (string idioma in paramCL.ObtenerListaIdiomasDictionary().Keys)
                                {
                                    string imagenIdioma = UtilCadenas.ObtenerTextoDeIdioma(valorPropiedad, idioma, null, true);

                                    if (!string.IsNullOrEmpty(imagenIdioma))
                                    {
                                        string comienzoFile = "File:";
                                        if (imagenIdioma.StartsWith(comienzoFile))
                                        {
                                            string cadenaControl = ";Data:";

                                            string[] fichero = imagenIdioma.Split(new string[] { cadenaControl, comienzoFile }, StringSplitOptions.RemoveEmptyEntries);

                                            string nombreFichero = UtilCadenas.RemoveAccentsWithRegEx(fichero[0]);
                                            string base64Image = fichero[1];

                                            List<string> listaExtensiones = new List<string>() { "jpg", "jpeg", "png", "gif" };

                                            if (listaExtensiones.Contains(nombreFichero.Split('.').Last().ToLower()))
                                            {
                                                byte[] byteImage = Convert.FromBase64String(base64Image);
                                                HttpResponseMessage resultadoImagen = InformarCambioAdministracionCMS("ObjetosMultimedia", Convert.ToBase64String(byteImage), nombreFichero);

                                                if (!resultadoImagen.StatusCode.Equals(HttpStatusCode.OK))
                                                {
                                                    throw new ExcepcionWeb("Ha ocurrido un error al registrar los cambios con la integración continua.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            HttpResponseMessage resultado = InformarCambioAdministracion("ComponentesCMS", JsonConvert.SerializeObject(new KeyValuePair<Guid, CMSAdminComponenteEditarViewModel>(componenteEdicion.Clave, pComponente), Formatting.Indented));
                            if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                            {
                                throw new ExcepcionWeb("Ha ocurrido un error al registrar los cambios con la integración continua.");
                            }
                        }

                        contrComponente.InvalidarCache(componenteEdicion.Clave);

                        contrComponente.CrearFilasPropiedadesIntegracionContinua(pComponente);

                        if (EntornoActualEsPruebas && iniciado)
                        {
                            contrComponente.ModificarFilasIntegracionContinuaEntornoSiguiente(pComponente, UrlApiEntornoSeleccionado("pre"), UsuarioActual.UsuarioID);
                            contrComponente.ModificarFilasIntegracionContinuaEntornoSiguiente(pComponente, UrlApiEntornoSeleccionado("pro"), UsuarioActual.UsuarioID);
                        }
                        CMSAdminComponenteEditarViewModel versionado = CargarModelo(contrComponente.CargarComponente(componenteEdicion.Clave));

                        contrComponente.GuardarVersionComponente(versionado, componenteEdicion.Clave, pComentario);

                        if (transaccionIniciada)
                        {
                            mEntityContext.TerminarTransaccionesPendientes(true);
                        }

                        if (bloqueContenedor.HasValue)
                        {
                            return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARPAGINA") + "/" + ((short)gestorCMS.ListaBloques[bloqueContenedor.Value].TipoUbicacion).ToString());
                        }
                        else
                        {
                            if (!esEdicion)
                            {
                                string idSavedComponent = componenteEdicion.Clave.ToString();
                                return GnossResultOK(idSavedComponent);
                            }
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
                }
                else
                {
                    return GnossResultERROR(error);
                }

                string rutasImagenes = "";
                if (componenteEdicion is CMSComponenteDestacado)
                {
                    rutasImagenes = ((CMSComponenteDestacado)componenteEdicion).Imagen;
                }
                return GnossResultOK(rutasImagenes);
            }
            catch
            {
                return GnossResultERROR("Ha habido un error al guardar el componente.");
            }
        }
        /// <summary>
        /// Carga los datos referentes a los proyectos basado en los identificadores pasados por parámetros
        /// </summary>
        /// <param name="pListaIdentificadores">Lista de identificadores de proyectos</param>
        /// <param name="pResultadoComprobacion">Lista con los modelos a cargar</param>
        private void CargarResultadosProyectosComponenteCMS(List<Guid> pListaIdentificadores, List<CMSAdminComponenteEditarCheckListViewModel> pResultadoComprobacion)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            GestionProyecto gestorProy = new GestionProyecto(proyCN.ObtenerProyectosPorID(pListaIdentificadores), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
            proyCN.Dispose();

            foreach (Guid proyID in gestorProy.ListaProyectos.Keys)
            {
                Proyecto proy = gestorProy.ListaProyectos[proyID];

                var resultados = pResultadoComprobacion.Where(resultado => resultado.Identificador == proyID);
                if (resultados.Any())
                {
                    string enlace = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proy.NombreCorto);
                    resultados.First().UrlEnlace = enlace;
                    resultados.First().TextoEnlace = proy.Nombre;
                    resultados.First().Error = string.Empty;
                }
            }
        }

        /// <summary>
        /// Carga los datos referentes a los componentes basado en los identificadores pasados por parámetros
        /// </summary>
        /// <param name="pListaIdentificadores">Lista de identificadores de componentes</param>
        /// <param name="pResultadoComprobacion">Lista con los modelos a cargar</param>
        /// <returns>True o false en función de si se ha cargado algo a partir de los datos proporcionados</returns>
        private bool CargarResultadosComponentesComponenteCMS(List<Guid> pListaIdentificadores, List<CMSAdminComponenteEditarCheckListViewModel> pResultadoComprobacion)
        {
            bool cargado = false;
            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
            GestionCMS gestionCMS = new GestionCMS(cmsCN.ObtenerComponentePorListaID(pListaIdentificadores, ProyectoSeleccionado.Clave, false), mLoggingService, mEntityContext);
            cmsCN.Dispose();

            foreach (Guid componenteID in gestionCMS.ListaComponentes.Keys)
            {
                CMSComponente componente = gestionCMS.ListaComponentes[componenteID];

                var resultados = pResultadoComprobacion.Where(resultado => resultado.Identificador == componenteID);
                if (resultados.Any())
                {
                    cargado = true;
                    string enlace = $"{mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARCOMPONENTE")}/{componente.Clave}";
                    resultados.First().UrlEnlace = enlace;
                    resultados.First().TextoEnlace = componente.Nombre;
                    resultados.First().Error = string.Empty;
                }
            }

            return cargado;
        }


        /// <summary>
        /// Carga los datos referentes a los recursos basado en los identificadores pasados por parámetros
        /// </summary>
        /// <param name="pListaIdentificadores">Lista de identificadores de recursos</param>
        /// <param name="pResultadoComprobacion">Lista con los modelos a cargar</param>
        private void CargarResultadosRecursosComponenteCMS(List<Guid> pListaIdentificadores, List<CMSAdminComponenteEditarCheckListViewModel> pResultadoComprobacion)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentosPorID(pListaIdentificadores, true), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
            docCN.ObtenerBaseRecursosProyecto(gestorDoc.DataWrapperDocumentacion, ProyectoSeleccionado.Clave);
            docCN.Dispose();

            foreach (Guid docID in gestorDoc.ListaDocumentos.Keys)
            {
                Documento doc = gestorDoc.ListaDocumentos[docID];

                var resultados = pResultadoComprobacion.Where(resultado => resultado.Identificador == docID);
                if (resultados.Any())
                {
                    string enlace = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, doc, false);
                    resultados.First().UrlEnlace = enlace;
                    resultados.First().TextoEnlace = UtilCadenas.ObtenerTextoDeIdioma(doc.Titulo, UtilIdiomas.LanguageCode, null);
                    resultados.First().Error = string.Empty;
                }
            }
        }

        #region Propiedades

        public CMSAdminComponenteEditarViewModel.PropiedadComponente ObtenerPropiedad(TipoPropiedadCMS tipoPropiedad, Dictionary<TipoPropiedadCMS, bool> propiedadesComponente)
        {
            CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad = new CMSAdminComponenteEditarViewModel.PropiedadComponente();
            propiedad.TipoPropiedadCMS = tipoPropiedad;
            propiedad.Required = propiedadesComponente[tipoPropiedad];
            if (CMSComponente != null && CMSComponente.PropiedadesComponente.ContainsKey(tipoPropiedad))
            {
                if (tipoPropiedad.Equals(TipoPropiedadCMS.ListaIDs) && TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
                {
                    string[] listaIDs = CMSComponente.PropiedadesComponente[tipoPropiedad].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    List<Guid> listaProyectos = new List<Guid>();
                    foreach (string elemento in listaIDs)
                    {
                        listaProyectos.Add(new Guid(elemento));
                    }

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    Dictionary<Guid, string> listaNombresCortos = proyCN.ObtenerNombresCortosProyectos(listaProyectos);
                    proyCN.Dispose();
                    //Recorremos esta lista para que no nos cambie el orden de los proyectos.
                    foreach (Guid proyectoID in listaProyectos)
                    {
                        propiedad.Value += listaNombresCortos[proyectoID] + ",";
                    }
                }
                else
                {
                    propiedad.Value = CMSComponente.PropiedadesComponente[tipoPropiedad];
                }
            }
            propiedad.TypeComponent = TipoComponenteCMSActual;

            propiedad.MultiLang = EsPropiedadMultiIdioma(tipoPropiedad);

            return propiedad;
        }

        private static bool EsPropiedadMultiIdioma(TipoPropiedadCMS pTipoPropiedad)
        {
            return UtilComponentes.ListaPropiedadesMultiIdioma.Contains(pTipoPropiedad);
        }

        private void CargarPermisosCMSViewBag()
        {
			UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
			ViewBag.VerComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.VerComponenteCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.CrearComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.CrearComponenteCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.EliminarComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarComponenteCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.ModificarComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EditarComponenteCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.RestaurarVersionComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.RestaurarVersionCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.EliminarVersionComponenteCMSPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarVersionCMS, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
		}

        private CMSAdminComponenteEditarViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    ControladorComponenteCMS contrComponente = new ControladorComponenteCMS(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<ControladorComponenteCMS>(), mLoggerFactory);

                    mPaginaModel = contrComponente.CargarComponente(TipoComponenteCMSActual, CMSComponente);

                    mPaginaModel = CargarModelo(mPaginaModel);
                }
                return mPaginaModel;
            }
        }

        private CMSAdminComponenteEditarViewModel CargarModelo(CMSAdminComponenteEditarViewModel pComponente)
        {
            pComponente.Personalizaciones = new Dictionary<Guid, string>();
            pComponente.PersonalizacionSeleccionada = Guid.Empty;

            if (CMSComponente != null)
            {
                pComponente.FechaModificacion = (DateTime)CMSComponente.FilaComponente.FechaUltimaActualizacion;
            }
            else
            {
                pComponente.FechaModificacion = DateTime.Now;
            }


            VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCL>(), mLoggerFactory);
            DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave, mControladorBase.PersonalizacionEcosistemaID, mControladorBase.ComunidadExcluidaPersonalizacionEcosistema);
            vistaVirtualCL.Dispose();

            string tipoComponente = TipoComponenteCMSActual.ToString();
            string vistaComponente = $"/Views/CMSPagina/{tipoComponente}/_{tipoComponente}";

            List<VistaVirtualCMS> listaVistaVirtualCMS = vistaVirtualDW.ListaVistaVirtualCMS.Where(fila => fila.TipoComponente.StartsWith(vistaComponente)).ToList();

            if (listaVistaVirtualCMS.Any())
            {
                foreach (VistaVirtualCMS filaVistaVirtualCMS in listaVistaVirtualCMS)
                {
                    string nombre = filaVistaVirtualCMS.Nombre;
                    if (string.IsNullOrEmpty(nombre))
                    {
                        nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                    }
                    pComponente.Personalizaciones.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);

                    if (CMSComponente != null && CMSComponente.PropiedadesComponente.ContainsKey(TipoPropiedadCMS.Personalizacion) && CMSComponente.PropiedadesComponente[TipoPropiedadCMS.Personalizacion] == filaVistaVirtualCMS.PersonalizacionComponenteID.ToString())
                    {
                        pComponente.PersonalizacionSeleccionada = filaVistaVirtualCMS.PersonalizacionComponenteID;
                    }
                }
            }
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
            if (ParametrosGeneralesRow.IdiomasDisponibles)
            {
                pComponente.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
            }
            else
            {
                pComponente.ListaIdiomas = new Dictionary<string, string>();
                pComponente.ListaIdiomas.Add(IdiomaPorDefecto, paramCL.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
            }

            if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma) || (ParametroProyecto.ContainsKey(ParametroAD.PropiedadCMSMultiIdioma) && ParametroProyecto[ParametroAD.PropiedadCMSMultiIdioma] == "1"))
            {
                pComponente.ContenidoMultiIdioma = true;
                pComponente.ListaIdiomasDisponibles = new List<string>();

                if (!string.IsNullOrEmpty(CMSComponente?.FilaComponente?.IdiomasDisponibles))
                {
                    string[] idiomasDisponibles = CMSComponente.FilaComponente.IdiomasDisponibles.Split("|||", StringSplitOptions.RemoveEmptyEntries);
                    foreach (string idioma in idiomasDisponibles)
                    {
                        string[] configuracionIdioma = idioma.Split("@");
                        string estadoIdioma = configuracionIdioma[0];
                        string claveIdioma = configuracionIdioma[1];
                        if (estadoIdioma?.ToLower() == "true" && !pComponente.ListaIdiomasDisponibles.Contains(claveIdioma))
                        {
                            pComponente.ListaIdiomasDisponibles.Add(claveIdioma);
                        }
                    }

                }
            }

            pComponente.IdiomaPorDefecto = IdiomaPorDefecto;
            pComponente.EsEdicion = esEdicion;

            Uri urlVolver = null;
            if (!string.IsNullOrEmpty(Request.Headers["Referer"]))
            {
                urlVolver = new Uri(Request.Headers["Referer"]);
            }

            if (bloqueContenedor.HasValue)
            {
                CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
                GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                cmsCN.Dispose();

                urlVolver = new Uri(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARPAGINA") + "/" + ((short)gestorCMS.ListaBloques[bloqueContenedor.Value].TipoUbicacion).ToString());
            }
            if (urlVolver != null)
            {
                pComponente.UrlVuelta = urlVolver.ToString();
            }

            return pComponente;
        }

        /// <summary>
        /// Componente editado
        /// </summary>
        private CMSComponente CMSComponente
        {
            get
            {
                if (mCMSComponente == null && esEdicion)
                {
                    Guid idComponente = Guid.Empty;
					if (!string.IsNullOrEmpty(RequestParams("idComponente")))
					{
						idComponente = new Guid(RequestParams("idComponente"));
					}
                    else if (!string.IsNullOrEmpty(RequestParams("pContenidoID")))
                    {
						idComponente = new Guid(RequestParams("pContenidoID"));
					}
                    GestionCMS gestorCMS = null;
                    using (CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
                    {
                        gestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    }
                    mCMSComponente = gestorCMS.ListaComponentes[idComponente];
                }
                return mCMSComponente;
            }
        }

        /// <summary>
        /// Indica si estamos editando o creando el componente
        /// </summary>
        private bool esEdicion
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("idComponente")) || !string.IsNullOrEmpty(RequestParams("pContenidoID")))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Tipo de componente actual
        /// </summary>
        private TipoComponenteCMS TipoComponenteCMSActual
        {
            get
            {
                if (esEdicion)
                {
                    return CMSComponente.TipoComponenteCMS;
                }
                else
                {
                    return (TipoComponenteCMS)short.Parse(RequestParams("tipoComponente"));
                }
            }
        }

        /// <summary>
        /// Indica el bloque que contendra al componente
        /// </summary>
        private Guid? bloqueContenedor
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("idBloqueContenedor")))
                {
                    return new Guid(RequestParams("idBloqueContenedor"));
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion
    }
}