using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.RDF;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
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
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para el borrado de recursos de la ontología
    /// </summary>
    public class AdministrarBorradoMasivoController : ControllerAdministrationWeb
	{
        #region Miembros

        private BorradoMasivoViewModel mBorradoMasivo = null;

        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        public AdministrarBorradoMasivoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarBorradoMasivoController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;

        }

        #region Metodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.EjecutarBorradoMasivo } })]
		public ActionResult Index()
        {
            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "grafo-de-conocimiento borrado-grafo edicion no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_BorradoMasivo;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "GRAFODECONOCIMIENTO");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONDESARROLLADORES", "BORRADOMASIVO");

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            BorradoMasivoViewModel mBorradoMasivo = ObtenerOntologias();
            return View(mBorradoMasivo);
        }

        /// <summary>
        /// Eliminar
        /// </summary>
        /// <param name="pOptions">Modelo de Borrado masivo</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.EjecutarBorradoMasivo } })]
		public ActionResult Eliminar(BorradoMasivoViewModel pOptions)
        {
            GuardarLogAuditoria();
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            FacetadoCN facetaCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            mServicesUtilVirtuosoAndReplication.UsarClienteTradicional = true;
            facetaCN.FacetadoAD.TimeOutVirtuoso = 12000;
            pOptions.OntologiaSeleccionada = HayOntologiaSeleccionada(pOptions);
            List<Guid> listaIdsRecursosEliminar = new List<Guid>();
            try
            {
                foreach (Guid guid in pOptions.OntologiaSeleccionada)
                {
                    string url = docCN.OntologiaSeleccionada(guid).Enlace.ToLower();
                    // Borro el grafo de la ontología
                    string query = $"SPARQL clear graph <{UrlIntragnoss}{url}>";

                    facetaCN.ActualizarVirtuoso(query, url, true, 0);
                    
                    // Borro el grafo de búsqueda
                    EliminarTriplesGrafoBusqueda(url);

                    List<Guid> listaIdsRecursosAux = docCN.ObtenerRecursosDeOntologia(guid);
                    if (listaIdsRecursosAux != null && listaIdsRecursosAux.Count > 0)
                    {
                        listaIdsRecursosEliminar.AddRange(listaIdsRecursosAux);
                    }  
                }

                // Borro las tablas ACID
                docCN.BorradoMasivoOntologias(pOptions.OntologiaSeleccionada, ProyectoSeleccionado.Clave);

                // Borro la base de datos RDF
                RdfCN rdfCN = new RdfCN("rdf", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<RdfCN>(), mLoggerFactory);
                rdfCN.VaciarTablasRdf();

                FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCL>(), mLoggerFactory);
                facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "");

                foreach (Guid recursoID in listaIdsRecursosEliminar)
                {
                    ControladorDocumentacion.InsertarEnColaProcesarFicherosRecursosModificadosOEliminados(recursoID, Models.FicherosRecursos.TipoEventoProcesarFicherosRecursos.BorradoPersistente, mAvailableServices);
                }

                return GnossResultOK();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return GnossResultERROR(ex.Message);
                throw;
            }
        }

        #endregion

        #region Metodos privados

        private BorradoMasivoViewModel ObtenerOntologias()
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            Dictionary<Guid, string> ontologias = docCN.ObtenerOntologiasParaBorrado(ProyectoSeleccionado.Clave);
            BorradoMasivoViewModel bMasivo = new BorradoMasivoViewModel();
            bMasivo.FechaCreacionOntologias = ObtenerFechasCreacion(ontologias);
			bMasivo.FechaModificacionOntologias = ObtenerFechasModificacion(ontologias);
            bMasivo.CantidadRecursosDeOntologia = ObtenerCantidadRecursosDeOntologia(ontologias);
            bMasivo.OntologiaABorrar = ontologias;
            return bMasivo;
        }

        private Dictionary<Guid,long> ObtenerFechasCreacion(Dictionary<Guid, string> ontologias)
        {
            Dictionary<Guid,long> ontologiasFechas = new Dictionary<Guid,long>();
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            foreach (Guid guid in ontologias.Keys)
            {
                ontologiasFechas.Add(guid, docCN.ObtenerFechaCreacionDocumento(guid));
            }
            return ontologiasFechas;
        }
		private Dictionary<Guid, long> ObtenerFechasModificacion(Dictionary<Guid, string> ontologias)
		{
			Dictionary<Guid, long> ontologiasFechas = new Dictionary<Guid, long>();
			DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
			foreach (Guid guid in ontologias.Keys)
			{
				ontologiasFechas.Add(guid, docCN.ObtenerFechaModificacionDocumento(guid));
			}
			return ontologiasFechas;
		}
        private Dictionary<Guid, int> ObtenerCantidadRecursosDeOntologia(Dictionary<Guid, string> ontologias)
        {
            Dictionary<Guid, int> recursosOntologias = new Dictionary<Guid, int>();
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            foreach (Guid guid in ontologias.Keys)
            {
                recursosOntologias.Add(guid, docCN.ObtenerCantidadRecursosDeOntologia(guid));
            }
            return recursosOntologias;
        }
        private List<Guid> HayOntologiaSeleccionada(BorradoMasivoViewModel pOptions)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            List<Guid> listaOnto = new List<Guid>();
            foreach (Guid guid in pOptions.OntologiaSeleccionada)
            {
                Documento ontologia = docCN.OntologiaSeleccionada(guid);
                listaOnto.Add(ontologia.DocumentoID);
            }
            return listaOnto;
        }

        private void EliminarTriplesGrafoBusqueda(string pOntologia)
        {
            FacetadoAD facetadoAD = new FacetadoAD(UrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoAD>(), mLoggerFactory);
            FacetadoCN facetaCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            facetaCN.FacetadoAD.TimeOutVirtuoso = 1200;
            facetadoAD.UsarClienteTradicional = true;
            string nombreArchivos = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            int posPunto = pOntologia.IndexOf(".");
            string ontologiaSinExtension = pOntologia.Substring(0, posPunto);

            string nombreOnt = mEntityContext.OntologiaProyecto.Where(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.OntologiaProyecto1.ToLower().Equals(ontologiaSinExtension.ToLower())).Select(item => item.OntologiaProyecto1.ToLower()).FirstOrDefault();

            string queryCopiaGrafo = $"dump_one_graph_distinto_de_tipo('{UrlIntragnoss}{ProyectoSeleccionado.Clave}','{nombreArchivos}',1000000,'{nombreOnt}')";
            string queryBorradoGrafo = $"sparql clear graph <{UrlIntragnoss}{ProyectoSeleccionado.Clave}>";
            string queryDeleteLoadList = "delete from DB.DBA.load_list";
            string querySetLoadList = $"ld_dir ('./dumps/', '{nombreArchivos}*.gz', '{UrlIntragnoss}{ProyectoSeleccionado.Clave}')";
            string queryLoaderRun = "rdf_loader_run()";
            string queryEnableCheckpoint = "checkpoint_interval(60)";
            string queryEnableRegenVirtuosoIndex = "DB.DBA.VT_BATCH_UPDATE('DB.DBA.RDF_OBJ', 'ON', 1)";
            string queryEnableScheduler = "scheduler_interval(1)";

            try
            {
                facetaCN.ActualizarVirtuoso(queryCopiaGrafo, ProyectoSeleccionado.Clave.ToString(), true, 0);
            }
            catch
            {
                throw new ExcepcionWeb("Faltan permisos para acceder a la carpeta dumps o no existe, contacta con el administrador");
            }

            GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {queryCopiaGrafo}");
            facetaCN.ActualizarVirtuoso(queryBorradoGrafo, ProyectoSeleccionado.Clave.ToString(), true, 0);
            GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {queryBorradoGrafo}");
            facetaCN.ActualizarVirtuoso(queryDeleteLoadList, ProyectoSeleccionado.Clave.ToString(), true, 0);
            GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {queryDeleteLoadList}");

            try
            {
                facetaCN.ActualizarVirtuoso(querySetLoadList, ProyectoSeleccionado.Clave.ToString(), true, 0);
            }
            catch (Exception ex)
            {
                try
                {
                    GuardarLogError(ex, "Error al ejecutar ld_dir en la ruta ./dumps/ Pruebo en /opt/virtuoso/var/lib/virtuoso/db/dumps/.");
                    querySetLoadList = $"ld_dir ('/opt/virtuoso/var/lib/virtuoso/db/dumps/', '{nombreArchivos}*.gz', '{UrlIntragnoss}{ProyectoSeleccionado.Clave}')";
                    facetaCN.ActualizarVirtuoso(querySetLoadList, ProyectoSeleccionado.Clave.ToString(), true, 0);
                    GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {querySetLoadList}");
                }
                catch (Exception exception)
                {
                    try
                    {
                        GuardarLogError(exception, "Error al ejecutar ld_dir en la ruta /opt/virtuoso/var/lib/virtuoso/db/dumps/. Pruebo en /opt/virtuoso/database/dumps/");

                        querySetLoadList = $"ld_dir ('/opt/virtuoso/database/dumps/', '{nombreArchivos}*.gz', '{UrlIntragnoss}{ProyectoSeleccionado.Clave}')";
                        facetaCN.ActualizarVirtuoso(querySetLoadList, ProyectoSeleccionado.Clave.ToString(), true, 0);
                        GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {querySetLoadList}");
                    }
                    catch (Exception exception2)
                    {
                        GuardarLogError(exception2, "Error al ejecutar ld_dir en la ruta /opt/virtuoso/database/dumps/. Pruebo en /opt/virtuoso-opensource/database/dumps/");

                        querySetLoadList = $"ld_dir ('/opt/virtuoso-opensource/database/dumps/', '{nombreArchivos}*.gz', '{UrlIntragnoss}{ProyectoSeleccionado.Clave}')";
                        facetaCN.ActualizarVirtuoso(querySetLoadList, ProyectoSeleccionado.Clave.ToString(), true, 0);
                        GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {querySetLoadList}");
                    }
                }
            }
            finally
            {
                facetaCN.ActualizarVirtuoso(queryLoaderRun, ProyectoSeleccionado.Clave.ToString(), true, 0);
                GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {queryLoaderRun}");
                facetaCN.ActualizarVirtuoso(queryEnableCheckpoint, ProyectoSeleccionado.Clave.ToString(), true, 0);
                GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {queryEnableCheckpoint}");
                facetaCN.ActualizarVirtuoso(queryEnableRegenVirtuosoIndex, ProyectoSeleccionado.Clave.ToString(), true, 0);
                facetaCN.ActualizarVirtuoso(queryEnableScheduler, ProyectoSeleccionado.Clave.ToString(), true, 0);
                GuardarLogError($"INFO: EJECUTADA INSTRUCCIÓN {queryEnableRegenVirtuosoIndex}");
                facetadoAD.UsarClienteTradicional = false;
            }
        }

        #endregion
    }
}