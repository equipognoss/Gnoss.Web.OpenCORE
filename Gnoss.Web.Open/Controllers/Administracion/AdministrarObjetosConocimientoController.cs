using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AdministrarObjetosConocimientoViewModel
    {
        public Dictionary<string, string> ListaIdiomas { get; set; }

        public string IdiomaPorDefecto { get; set; }

        public Dictionary<string, string> ListaOntologiasCom { get; set; }

        public List<ObjetoConocimientoModel> ListaObjetosConocimiento { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarObjetosConocimientoController : ControllerBaseWeb
    {
        public AdministrarObjetosConocimientoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarObjetosConocimientoViewModel mPaginaModel = null;

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
        /// Nuevo Contexto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevoObjetoConocimento(string ontology)
        {
            EliminarPersonalizacionVistas();

            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, false, true);
            documentacionCN.Dispose();

            

            AD.EntityModel.Models.Documentacion.Documento filaDoc = dataWrapperDocumentacion.ListaDocumento.FirstOrDefault(doc => doc.Tipo == (short)TiposDocumentacion.Ontologia && doc.Enlace.ToLower().Equals(ontology.ToLower() + ".owl"));
            Dictionary<string, List<EstiloPlantilla>> listaEstilos = new Dictionary<string, List<EstiloPlantilla>>();
            byte[] arrayOnto = ControladorDocumentacion.ObtenerOntologia(filaDoc.DocumentoID, out listaEstilos, filaDoc.ProyectoID.Value, null, null, false);
            Ontologia ontologia = new Ontologia(arrayOnto, true);
            ontologia.LeerOntologia();

            if (filaDoc != null)
            {
                ObjetoConocimientoModel objetoConocimiento = new ObjetoConocimientoModel();
                objetoConocimiento.Ontologia = ontology;
                objetoConocimiento.Name = filaDoc.Titulo;
                objetoConocimiento.Subtipos = new Dictionary<string, string>();
                objetoConocimiento.PresentacionListado = new ObjetoConocimientoModel.PresentacionModel();
                objetoConocimiento.PresentacionMosaico = new ObjetoConocimientoModel.PresentacionModel();
                objetoConocimiento.PresentacionMapa = new ObjetoConocimientoModel.PresentacionModel();
                objetoConocimiento.PresentacionRelacionados = new ObjetoConocimientoModel.PresentacionModel();

                // TODO Fernando
                Dictionary<string, string> namespaces = ontologia.NamespacesDefinidos;
                string valor;
                string namespaceExtra = "";
                string namespacesFinal ="";
                foreach(string clave in namespaces.Keys)
                {
                    namespaces.TryGetValue(clave, out valor);
                    if(!valor.Equals("rdf") && !valor.Equals("xsd") && !valor.Equals("rdfs") && !valor.Equals("owl") && !valor.Equals("base") && !string.IsNullOrEmpty(valor))
                    {
                        namespaceExtra = valor + ":" + clave;
                        if (string.IsNullOrEmpty(namespacesFinal))
                        {
                            namespacesFinal = $"{namespaceExtra}";
                        }
                        else
                        {
                            namespacesFinal = $"{namespacesFinal}|{namespaceExtra}";
                        }
                    }
                }
                objetoConocimiento.NamespaceExtra = namespacesFinal;

                return PartialView("_EdicionObjetoConocimiento", objetoConocimiento);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Nuevo Contexto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevoSubTipo()
        {
            EliminarPersonalizacionVistas();

            return PartialView("_FichaSubTipo", new KeyValuePair<string, string>("", "Nuevo Sub-Tipo"));
        }

        /// <summary>
        /// Nuevo Contexto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaPropiedad()
        {
            EliminarPersonalizacionVistas();

            ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();
            propiedad.Propiedad = "Nueva propiedad";

            return PartialView("_FichaPropiedad", propiedad);
        }

        /// <summary>
        /// Nuevo Contexto
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaPropiedadPersonalizada()
        {
            EliminarPersonalizacionVistas();
            ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel propiedad = new ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel();
            propiedad.Identificador = "Identificador";
            propiedad.Select = "select distinct ?s ?rdftype"; 
            propiedad.Where = "where\n{\n  ?s a ?rdftype.\n  <FILTER_RESOURCE_IDS>\n}";
            return PartialView("_FichaPropiedadPersonalizado", propiedad);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Guardar(List<ObjetoConocimientoModel> ListaObjetosConocimiento)
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
            ControladorObjetosConocimiento contrObjetosConocim = new ControladorObjetosConocimiento(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            string errores = contrObjetosConocim.ComprobarErrores(ListaObjetosConocimiento);

            if (string.IsNullOrEmpty(errores))
            {
                GuardarXmlCambiosAdministracion();
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;

                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    contrObjetosConocim.GuardarObjetosConocimiento(ListaObjetosConocimiento);
                    if (iniciado)
                    {
                        HttpResponseMessage resultado = InformarCambioAdministracion("ObjetosConocimiento", JsonConvert.SerializeObject(ListaObjetosConocimiento, Formatting.Indented));

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

                contrObjetosConocim.InvalidarCaches(UrlIntragnoss);

                return GnossResultOK();
            }
            else {
                return GnossResultERROR(errores);
            }
        }

        #endregion

        #region Propiedades

        private AdministrarObjetosConocimientoViewModel PaginaModel
        {
            get
            {
                if(mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarObjetosConocimientoViewModel();

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

                    mPaginaModel.ListaOntologiasCom = new Dictionary<string, string>();

                    DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                    DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Guid proyectoIDPatronOntologias = Guid.Empty;
                    if (ParametroProyecto.ContainsKey("ProyectoIDPatronOntologias"))
                    {
                        Guid.TryParse(ParametroProyecto["ProyectoIDPatronOntologias"], out proyectoIDPatronOntologias);
                        documentacionCN.ObtenerOntologiasProyecto(proyectoIDPatronOntologias, dataWrapperDocumentacion, false, false, true);
                    }
                    documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, false, true);
                    documentacionCN.Dispose();

                    List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = dataWrapperDocumentacion.ListaDocumento.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList();

                    mPaginaModel.ListaObjetosConocimiento = new List<ObjetoConocimientoModel>();

                    ControladorObjetosConocimiento contrObjetosConocim = new ControladorObjetosConocimiento(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                    foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in filasDoc)
                    {
                        var enlace = filaDoc.Enlace.Replace(".owl", "");
                        if (!mPaginaModel.ListaOntologiasCom.Any(x=>string.Compare(x.Key,enlace,true)==0))
                        {
                            mPaginaModel.ListaOntologiasCom.Add(enlace, filaDoc.Titulo);
                        }
                        ObjetoConocimientoModel objetoConocimiento = contrObjetosConocim.CargarObjetoConocimiento(filaDoc);

                        if (objetoConocimiento != null && !mPaginaModel.ListaObjetosConocimiento.Any(o => o.Ontologia == objetoConocimiento.Ontologia))
                        {
                            mPaginaModel.ListaObjetosConocimiento.Add(objetoConocimiento);
                        }
                    }
                }
                return mPaginaModel;
            }
        }

        #endregion
    }
}
