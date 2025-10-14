using AngleSharp.Common;
using Microsoft.AspNetCore;
using DotNetOpenAuth.Messaging;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OntologiaAClase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Universal.Common;
using Universal.Common.Extensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Gnoss.Web.Open.Filters;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    // ----------------------- Clases auxiliares ----------------------------------
    [Serializable]
    public class OntologiaCompleta
    {
        public string Enlace { get; set; }
        public Ontologia Ontologia { get; set; }
        public Guid DocumentoID { get; set; }
        public Dictionary<string, List<EstiloPlantilla>> XML { get; set; }
    }
    [Serializable]
    public class CaminoHastaPrincipal
    {
        public string Path { get; set; }
        public Propiedad Propiedad { get; set; }
    }
    // -------------------------------------------------------------------------

    public class AdministrarFacetasController : ControllerAdministrationWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarFacetasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarFacetasController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros
        private static List<string> mNombrePropiedadesOntologiasBasicas = new List<string>() { "rdf:type" };
        private static List<string> mPrefijosExcluidos = new List<string>() { "gnoss:" };
        private static List<string> mPrefijosEspeciales = new List<string>() { "vcard:", "skos:", "rdf:", "sioc-t:", "foaf:" };

        private AdministrarFacetasViewModel mPaginaModel = null;
        private Dictionary<string, string> mListaOntologias = null;
        private List<OntologiaCompleta> mListaOntologiasCompletas = null;
        private readonly List<FacetaModel> mListaFacetasPropuestas = new List<FacetaModel>();


        #endregion

        #region Metodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.VerFaceta, (ulong)PermisoContenidos.CrearFaceta, (ulong)PermisoContenidos.EliminarFaceta, (ulong)PermisoContenidos.ModificarFaceta } })]
        public ActionResult Index()
        {

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "estructura edicion edicionPaginas no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.DescubrimientoAnalisis;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.DescubrimientoAnalisis_Facetas;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "DESCUBRIMIENTOYANALISIS");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONSEMANTICA", "FACETAS");

            // Establecer en el ViewBag el idioma por defecto
            ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;			

			EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
			CargarPermisosFacetasEnViewBag();

			return View(PaginaModel);
        }

        /// <summary>
        /// Nueva Faceta
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.CrearFaceta } })]
		public ActionResult NuevaFaceta(TipoFaceta TipoFaceta, Guid? pestanyaPropuestaID = null)
        {
            EliminarPersonalizacionVistas();
            CargarPermisosFacetasEnViewBag();
            FacetaModel faceta = null;
            if (!pestanyaPropuestaID.HasValue || (ListaFacetasPropuestasCache != null && !ListaFacetasPropuestasCache.ContainsKey(pestanyaPropuestaID.Value)) || ListaFacetasPropuestasCache == null)
            {
                faceta = CargarFacetaNueva(TipoFaceta);
            }
            else
            {
                //El diccionario se carga en el index al cargar la pagina, cuando se calcula el PaginaModel
                faceta = ListaFacetasPropuestasCache[pestanyaPropuestaID.Value];
                char[] separators = new char[2];
                separators[0] = '-';
                separators[1] = '|';
                string[] ObjetoConocimientoAndNames = faceta.Name.Split(separators);
                string name = "";
                foreach (string text in ObjetoConocimientoAndNames.Where(item => item.Contains('@')))
                {
                    name = $"{name}{text}|||";
                }
                faceta.Name = name;
            }

            CargarOntologias();

            // Cargar el idioma por defecto cuando se cree una nueva faceta
            ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
            return PartialView("_EdicionFaceta", faceta);
        }

        /// <summary>
        /// Nueva Pestaña
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.CrearFaceta, (ulong)PermisoContenidos.ModificarFaceta } })]
		public ActionResult NuevoFiltro()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();


            FacetaModel.FiltrosFacetas filtro = new FacetaModel.FiltrosFacetas();
            string filtroID = Guid.NewGuid().ToString();
            filtro.Key = filtroID;
            //Le pongo el valor deleted true para que se sepa que es un filtro nuevo en la vista.
            filtro.Deleted = true;

            return PartialView("_FichaFiltro", filtro);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.CrearFaceta, (ulong)PermisoContenidos.ModificarFaceta, (ulong)PermisoContenidos.EliminarFaceta } })]
		[TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Guardar(List<FacetaModel> ListaFacetas)
        {
            GuardarLogAuditoria();
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
            ControladorFacetas contrFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorFacetas>(), mLoggerFactory);
            string errores = contrFacetas.ComprobarErrores(ListaFacetas);
            if (string.IsNullOrEmpty(errores))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = false;

                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);
                    List<FacetaModel> facetaModel = PaginaModel.ListaFacetas;

                    contrFacetas.GuardarFacetas(ListaFacetas);
                    if (iniciado)
                    {
                        ListaFacetas = ModificarOrdenFacetas(facetaModel, ListaFacetas);
                        HttpResponseMessage resultado = InformarCambioAdministracion("Facetas", JsonConvert.SerializeObject(ListaFacetas, Formatting.Indented));

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new ExcepcionWeb("Contacte con el administrador del Proyecto, no es posible atender la petición.");
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


        /// <summary>
        /// Dado un string q devuelve las posibles autocompleciones en base a las ontologías pasadas como parámetro, o en base a todas las ontologías de la comunidad si la lista está vacía
        /// </summary>
        /// <param name="q">filtro de la faceta a autocompletar</param>
        /// <param name="pOntologiasSeleccionadas">ontologías a considerar para la autocompleción. Se utilizarán propiedades que pertenezcan a todas las ontologías seleccionadas para sugerir el autocompletado. En caso de que la lista esté vacía, se considerarán todas las propiedades del proyecto. </param>
        /// <returns>IActionResult con la lista de posibles autocompleciones en el cuerpo de la respuesta.</returns>
        [HttpPost]
        public IActionResult AutocompletarOntologias([FromForm] string q, [FromForm(Name = "ontologias")] IEnumerable<string> pOntologiasSeleccionadas)
        {
            StringBuilder sb = new StringBuilder();
            List<Propiedad> opcionesAutocompletado = new List<Propiedad>();
            if (ListaPropiedadesCache == null) // Si la cache ha sido borrada se recuperan la lista de propiedades
            {
                CargarPropiedadesCache();
            }
            string[] caminoPropiedades = q.Split("@@@");
            string propertyName = caminoPropiedades[caminoPropiedades.Length - 1];
            string prefix = string.Empty;

            opcionesAutocompletado = ListaPropiedadesCache.Where(prop => prop.NombreConNamespace.ToLower().Contains(propertyName)).ToList();

            if (caminoPropiedades.Count() > 1) // Si el filtro contiene un camino entre cadenas se sugieren las propiedades de la ontología destino
            {
                string nombrePropiedadOrigen = caminoPropiedades[caminoPropiedades.Length - 2];
                List<Propiedad> candidatasPropiedadOrigen = ListaPropiedadesCache.Where(prop => prop.NombreConNamespace == nombrePropiedadOrigen).ToList();

                bool coincidencia = false;
                foreach (Propiedad candidata in candidatasPropiedadOrigen.Where(item => item.EspecifPropiedad.SelectorEntidad != null))
                {
                    string nombreOntologiaDestino = candidata.EspecifPropiedad.SelectorEntidad.Grafo;
                    nombreOntologiaDestino = nombreOntologiaDestino.Remove(nombreOntologiaDestino.Length - 4);

                    opcionesAutocompletado = opcionesAutocompletado.Where(prop => prop.Ontologia.ConfiguracionPlantilla.Namespace == nombreOntologiaDestino).ToList();
                    prefix = q.Remove(q.Length - propertyName.Length);
                    coincidencia = true;
                }

                if (!coincidencia) // Si ninguna de las candidatas apunta a una ontologiaOrigen, se sugiere volver al paso anterior
                {
                    opcionesAutocompletado.Clear();
                    sb.Append(q.Remove(q.Length - propertyName.Length - 3));
                    sb.Append(Environment.NewLine);
                }
            }
            else if (pOntologiasSeleccionadas.Any()) // Si hay alguna ontologiaOrigen seleccionada, se cogen solo las propiedades que pertenecen a todas ellas. 
            {
                if (pOntologiasSeleccionadas.Count() > 1)
                {
                    opcionesAutocompletado = opcionesAutocompletado.Where(prop => opcionesAutocompletado.Count(prop2 => prop2.NombreConNamespace == prop.NombreConNamespace) == pOntologiasSeleccionadas.Count()).ToList(); // Si una propiedad está en todas las ontologías estará repetida tantas veces como ontologías haya
                }

                opcionesAutocompletado = opcionesAutocompletado.Where(prop => pOntologiasSeleccionadas.Contains(prop.Ontologia.ConfiguracionPlantilla.Namespace)).ToList();
                opcionesAutocompletado = opcionesAutocompletado.GroupBy(prop => prop.NombreConNamespace).Select(prop => prop.First()).ToList(); // Eliminar duplicados

            } // Si no hay ontologias seleccionadas, se consideran todas las ontologias del proyecto

            foreach (Propiedad prop in opcionesAutocompletado)
            {
                sb.Append(prefix);
                sb.Append(prop.NombreConNamespace);
                sb.Append(Environment.NewLine);
            }

            if (!pOntologiasSeleccionadas.Any() && caminoPropiedades.Count() == 1)
            {
                foreach (string s in mNombrePropiedadesOntologiasBasicas.Where(item => item.Contains(propertyName)))
                {
                    sb.Append(s);
                    sb.Append(Environment.NewLine);
                }
            }

            return Ok(sb.ToString());
        }

        /// <summary>
        /// Comprueba que un filtro es válido para las ontologías seleccionadas, i.e., que la primera propiedad pertenece a todas las ontologías y el camino entre propiedades es correcto
        /// </summary>
        /// <param name="pFiltro">Filtro de faceta validar</param>
        /// <param name="pOntologiasSeleccionadas">Ontologías a las que se va a aplicar la faceta.</param>
        /// <returns>Si la validación falla, el método devuelve un GnossResultERROR con la causa del error. En caso contrario, se deuvelve un GnossResultOK</returns>
        [HttpPost]
        public IActionResult ValidarFaceta([FromForm] string pFiltro, [FromForm] IEnumerable<string> pOntologiasSeleccionadas)
        {
            string[] caminoPropiedades = pFiltro.Split("@@@");
            string nombrePropiedadOrigen = caminoPropiedades.FirstOrDefault();
            if (ListaPropiedadesCache == null)
            {
                CargarPropiedadesCache();
            }

            // Los prefijos excluidos son casos excepcionales que se dan siempre por válidos
            if (mPrefijosExcluidos.Exists(item => nombrePropiedadOrigen.StartsWith(item)))
            {
                return GnossResultOK();
            }

            OntologiaCompleta ontologiaOrigen;
            string nombreOntologiaOrigen;

            foreach (string nombreOntologia in pOntologiasSeleccionadas)  // Validar que la primera propiedad pertenezca a todas las ontologías seleccionadas
            {
                ontologiaOrigen = ListaOntologiasCompletas.Find(onto => onto.Ontologia.ConfiguracionPlantilla.Namespace == nombreOntologia);
                if (ontologiaOrigen != null) // Para el caso de ontologías como Recursos, Preguntas, Debates, etc que no están en backend se da siempre por válido
                {
                    for (int i = 0; i < caminoPropiedades.Length; i++) // Validar que cada propiedad del camino está en la ontología correspondiente y apunta a la siguiente, salvo la última
                    {
                        nombrePropiedadOrigen = caminoPropiedades[i];
                        Propiedad propiedadOrigen = ontologiaOrigen.Ontologia.Propiedades.Find(prop => prop.NombreConNamespace == nombrePropiedadOrigen);

                        if (propiedadOrigen == null) // Si la propiedad no pertenece a la ontolgía, se comprueba si es una de las especiales y se da error en caso contrario
                        {
                            if (mPrefijosEspeciales.Exists(item => nombrePropiedadOrigen.StartsWith(item)))
                            {
                                if (caminoPropiedades.Length > 1)
                                {
                                    return GnossResultERROR($"La propiedad especial {nombrePropiedadOrigen} no puede estar en un camino entre ontologías.");
                                }

                                return GnossResultOK();
                            }

                            return GnossResultERROR($"La propiedad {nombrePropiedadOrigen} no está en la ontología {ontologiaOrigen.Ontologia.ConfiguracionPlantilla.Namespace}.");
                        }

                        EstiloPlantillaEspecifProp plantillaOrigen = (EstiloPlantillaEspecifProp)ontologiaOrigen.XML.FirstOrDefault(prop => prop.Key == propiedadOrigen.Nombre).Value.FirstOrDefault();

                        // Si la propiedad actual no es la última, se comprueba si la propiedad es de tipo objeto
                        if (i < caminoPropiedades.Length - 1 && plantillaOrigen != null)
                        {
                            if (plantillaOrigen.SelectorEntidad == null)
                            {
                                return GnossResultERROR($"La propiedad {nombrePropiedadOrigen} no apunta a otra ontología.");
                            }

                            nombreOntologiaOrigen = plantillaOrigen.SelectorEntidad.Grafo; // Se avanza a la siguiente ontología
                            nombreOntologiaOrigen = nombreOntologiaOrigen.Remove(nombreOntologiaOrigen.Length - 4);
                            ontologiaOrigen = ListaOntologiasCompletas.Find(onto => onto.Ontologia.ConfiguracionPlantilla.Namespace == nombreOntologiaOrigen);
                        }
                    }
                }
            }

            return GnossResultOK();
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
            if(pTipoFaceta == TipoFaceta.Siglo)
            {
                faceta.AlgoritmoTransformacion = 19;
            }
            else
            {
                faceta.AlgoritmoTransformacion = 0;

            }
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

        private List<FacetaModel> ModificarOrdenFacetas(List<FacetaModel> pFacetaModel, List<FacetaModel> pListaFacetas)
        {
            List<FacetaModel> listaFacetasDevolver = new List<FacetaModel>();
            foreach (FacetaModel faceta in pListaFacetas)
            {
                FacetaModel facetaNueva = faceta;
                if (!faceta.Deleted && !faceta.Modified)
                {
                    facetaNueva = pFacetaModel.Find(facet => facet.AgrupacionID.Equals(faceta.AgrupacionID));
                    
                    if (facetaNueva != null && facetaNueva.Orden != faceta.Orden)
                    {
                        //Solo modificar las que se han cambiado de orden 
                        facetaNueva.Orden = faceta.Orden;
                        facetaNueva.Modified = true;
                        listaFacetasDevolver.Add(facetaNueva);
                    }
                }
                else
                {
                    listaFacetasDevolver.Add(facetaNueva);
                }
            }
            return listaFacetasDevolver;
        }

        private void CargarOntologias()
        {
            ViewBag.ListaOntologias = ListaOntologias;
        }


        /// <summary>
        /// Carga las propiedades del proyecto en la memoria cache
        /// </summary>
        private void CargarPropiedadesCache()
        {
            ListaPropiedadesCache = new List<Propiedad>();
            foreach (OntologiaCompleta ontologiaCompleta in ListaOntologiasCompletas)
            {
                ElementoOntologia entidadPrincipal = ontologiaCompleta.Ontologia.ObtenerEntidadPrincipal();
                foreach (Propiedad propiedad in entidadPrincipal.Propiedades)
                {
                    ListaPropiedadesCache.Add(propiedad);
                }
            }
        }


        /// <summary>
        /// Método para cargar las facetas propuestas cuando se inicia la página
        /// </summary>
        private void CargarFacetasPropuestas()
        {
            foreach (OntologiaCompleta ontologiaCompleta in ListaOntologiasCompletas)
            {
                if (ontologiaCompleta.XML != null)
                {
                    EstiloPlantillaConfigGen configGen = (EstiloPlantillaConfigGen)ontologiaCompleta.XML["[ConfiguracionGeneral]"][0];
                    configGen.Ontologia = ontologiaCompleta.Ontologia;
                    ElementoOntologia entidadPrincipal = ontologiaCompleta.Ontologia.ObtenerEntidadPrincipal();
                    string propiedadDescripcion = configGen.PropiedadDescripcion.Key;
                    string propiedadImagen = configGen.PropiedadImagenRepre.Key;
                    string propiedadTitulo = configGen.PropiedadTitulo.Key;
                    if (propiedadDescripcion == null) { propiedadDescripcion = ""; }
                    if (propiedadTitulo == null) { propiedadTitulo = ""; }
                    if (propiedadImagen == null) { propiedadImagen = ""; }

                    var selectoresEntidad = ontologiaCompleta.XML.Where(item => item.Value.Count > 0 && item.Value[0] is EstiloPlantillaEspecifProp && ((EstiloPlantillaEspecifProp)item.Value[0]).SelectorEntidad != null).Select(item => item.Value[0]).ToList();

                    foreach (Propiedad propiedad in entidadPrincipal.Propiedades)
                    {
                        var element = ontologiaCompleta.XML.FirstOrDefault(item => item.Key.Equals(propiedad.Nombre));
                        EstiloPlantillaEspecifProp xmlProp = null;
                        if (element.Value != null)
                        {
                            xmlProp = (EstiloPlantillaEspecifProp)element.Value[0];
                            selectoresEntidad.Remove(xmlProp);
                        }
                        if (propiedad.Rango.EndsWith("#string") && !propiedad.RangoEsFecha || (propiedad.Rango.EndsWith("#string") && xmlProp != null && xmlProp.SelectorEntidad != null) || propiedad.Rango.Equals(""))
                        {
                            if (xmlProp != null && xmlProp.ImagenMini == null && !xmlProp.FechaConHora && !xmlProp.FechaLibre && !xmlProp.FechaMesAnio)
                            {
                                if ((!propiedadDescripcion.Equals(propiedad.Nombre) || propiedadDescripcion.Equals(propiedadTitulo)) && !propiedadImagen.Equals(propiedad.Nombre))
                                {
                                    if ((xmlProp.SelectorEntidad == null && !mPaginaModel.ListaFacetas.Exists(faceta => faceta.ClaveFaceta.Equals(propiedad.NombreConNamespace) && faceta.ObjetosConocimiento.Contains(propiedad.Ontologia.ConfiguracionPlantilla.Namespace) && string.IsNullOrEmpty(faceta.ClaveFacetaYReprocidad))) || (xmlProp.SelectorEntidad != null && string.IsNullOrEmpty(xmlProp.SelectorEntidad.ConsultaReciproca)))
                                    {
                                        FacetaModel facetaPropuesta = CrearFacetaPropuesta(propiedad, xmlProp, ontologiaCompleta);
                                        AniadirFacetaPropuestaListas(facetaPropuesta, xmlProp);
                                    }
                                }
                            }
                        }
                    }

                    foreach (EstiloPlantillaEspecifProp element in selectoresEntidad)
                    {
                        ElementoOntologia entidad = ontologiaCompleta.Ontologia.Entidades.Find(item => item.TipoEntidad.Equals(element.NombreEntidad));
                        if (!entidadPrincipal.TipoEntidad.Equals(entidad.TipoEntidad))
                        {
                            Propiedad propiedad = ontologiaCompleta.Ontologia.Propiedades.Find(item => item.Nombre.Equals(element.NombreRealPropiedad));

                            string relacionHastaPrincipal = ObtenerCaminoHastaPrincipal(ontologiaCompleta.Ontologia, entidad.TipoEntidad, entidadPrincipal.TipoEntidad, new List<string>());
                            if (!string.IsNullOrEmpty(relacionHastaPrincipal))
                            {
                                var ontologiaDestinoCompl = ListaOntologiasCompletas.Find(item => item.Enlace.Equals(element.SelectorEntidad.Grafo));
                                if (ontologiaDestinoCompl != null && propiedad != null)
                                {
                                    FacetaModel facetaPropuesta = CrearFacetaPropuesta(propiedad, element, ontologiaDestinoCompl);
                                    facetaPropuesta.ClaveFaceta = $"{relacionHastaPrincipal}{facetaPropuesta.ClaveFaceta}";
                                    AniadirFacetaPropuestaListas(facetaPropuesta, element);
                                }
                            }
                        }
                    }
                }
            }
            mPaginaModel.ListaFacetasPropuestas = mListaFacetasPropuestas;
        }

        public void AniadirFacetaPropuestaListas(FacetaModel pFacetaPropuesta, EstiloPlantillaEspecifProp pXmlProp)
        {
            bool aniadir = true;
            if (pXmlProp.SelectorEntidad != null)
            {
                aniadir = !mPaginaModel.ListaFacetas.Exists(faceta => faceta.ClaveFaceta.Equals(pFacetaPropuesta.ClaveFaceta) && faceta.ObjetosConocimiento.Contains(pFacetaPropuesta.ObjetosConocimiento[0]));

            }
            if (aniadir)
            {
                mListaFacetasPropuestas.Add(pFacetaPropuesta);
                ListaFacetasPropuestasCache.Add(pFacetaPropuesta.SuggestedID, pFacetaPropuesta);
            }
        }

        private string ObtenerCaminoHastaPrincipal(Ontologia pOntologia, string pEntidadDestinoID, string pEntidadPrincipalID, List<string> pEntidadesExaminadas)
        {
            if (pEntidadesExaminadas.Contains(pEntidadDestinoID))
            {
                return null;
            }
            else
            {
                pEntidadesExaminadas.Add(pEntidadDestinoID);
            }
            string relacionHastaPrincipal = null;

            var propiedadRelacionPrincipal = pOntologia.Propiedades.Find(item => item.Rango.Equals(pEntidadDestinoID) && item.Dominio.Contains(pEntidadPrincipalID));

            if (propiedadRelacionPrincipal != null)
            {
                //Terminado
                relacionHastaPrincipal = propiedadRelacionPrincipal.NombreConNamespace + "@@@";
            }
            else
            {
                foreach (var propiedad in pOntologia.Propiedades.Where(item => item.Rango.Equals(pEntidadDestinoID)))
                {
                    foreach (string entidad in propiedad.Dominio)
                    {
                        relacionHastaPrincipal = ObtenerCaminoHastaPrincipal(pOntologia, entidad, pEntidadPrincipalID, pEntidadesExaminadas);

                        if (!string.IsNullOrEmpty(relacionHastaPrincipal))
                        {
                            return $"{relacionHastaPrincipal}{propiedad.NombreConNamespace}@@@";
                        }
                    }
                }
            }
            return relacionHastaPrincipal;
        }

        public string ObtenerClaveFacetaPropuesta(Propiedad pPropiedad, EstiloPlantillaEspecifProp pXmlProp, OntologiaCompleta pOntologiaCompleta)
        {
            string claveFaceta = $"{pPropiedad.NombreConNamespace}";
            if (pXmlProp != null && pXmlProp.SelectorEntidad != null)
            {
                string propLink = "";
                if (pXmlProp.SelectorEntidad.PropLinkARecurso != null && pXmlProp.SelectorEntidad.PropLinkARecurso.Count > 0)
                {
                    propLink = pXmlProp.SelectorEntidad.PropLinkARecurso[0];

                }
                else if (pXmlProp.SelectorEntidad.PropiedadesLectura != null && pXmlProp.SelectorEntidad.PropiedadesLectura.Count > 0)
                {
                    propLink = pXmlProp.SelectorEntidad.PropiedadesLectura[0].NombreRealPropiedad;
                }
                string ontologiaRef = pXmlProp.SelectorEntidad.Grafo;
                OntologiaCompleta ontologiaCompletaRef = ListaOntologiasCompletas.Find(item => item.Enlace.Equals(ontologiaRef));
                Propiedad propRef = pOntologiaCompleta.Ontologia.Propiedades.Find(item => item.Nombre.Equals(propLink));
                var element = pOntologiaCompleta.XML.FirstOrDefault(item => item.Key.Equals(propLink));
                EstiloPlantillaEspecifProp xmlPropRef = null;
                if (element.Value != null)
                {
                    xmlPropRef = (EstiloPlantillaEspecifProp)element.Value[0];
                    claveFaceta = $"{claveFaceta}@@@{ObtenerClaveFacetaPropuesta(propRef, xmlPropRef, ontologiaCompletaRef)}";
                }
            }
            return claveFaceta;
        }
        private FacetaModel CrearFacetaPropuesta(Propiedad pPropiedad, EstiloPlantillaEspecifProp pXmlProp, OntologiaCompleta pOntologiaCompleta)
        {
            Guid suggestID = Guid.NewGuid();
            FacetaModel facetaPropuesta = new FacetaModel();
            facetaPropuesta.SuggestedID = suggestID;
            facetaPropuesta.Type = TipoFaceta.Texto;
            facetaPropuesta.Comportamiento = 0;
            facetaPropuesta.ObjetosConocimiento = new List<string> { pPropiedad.Ontologia.ConfiguracionPlantilla.Namespace };
            facetaPropuesta.EsSemantica = true;
            facetaPropuesta.PrivacidadGrupos = new Dictionary<Guid, string>();
            facetaPropuesta.ListaFiltrosFacetas = new List<FacetaModel.FiltrosFacetas>();
            facetaPropuesta.ListFiltro = new List<string>();
            facetaPropuesta.Condicion = "";
            facetaPropuesta.Filtros = "";
            facetaPropuesta.NumElementosVisibles = 10;
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

            if (((pOntologiaCompleta.Ontologia.ConfiguracionPlantilla.ListaIdiomas != null && pOntologiaCompleta.Ontologia.ConfiguracionPlantilla.ListaIdiomas.Count > 1) || pOntologiaCompleta.Ontologia.ConfiguracionPlantilla.MultiIdioma) && !pXmlProp.NoMultiIdioma)
            {
                facetaPropuesta.AlgoritmoTransformacion = (short)TiposAlgoritmoTransformacion.MultiIdioma;
            }

            foreach (string idioma in paramCL.ObtenerListaIdiomas())
            {
                string nombre = UtilCadenas.SplitCamelCase(pPropiedad.NombreGeneracionClases);
                nombre = UtilCadenas.PrimerCaracterAMayuscula(nombre);
                facetaPropuesta.Name = $"{facetaPropuesta.Name}{pPropiedad.Ontologia.ConfiguracionPlantilla.Namespace} - {nombre}@{idioma}|||";
            }
            facetaPropuesta.ClaveFaceta = ObtenerClaveFacetaPropuesta(pPropiedad, pXmlProp, pOntologiaCompleta);
            return facetaPropuesta;
        }

        private void CargarPermisosFacetasEnViewBag()
        {
			UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
			ViewBag.CrearFacetaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.CrearFaceta, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.EliminarFacetaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarFaceta, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.ModificarFacetaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.ModificarFaceta, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
			ViewBag.VerFacetaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.VerFaceta, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
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

        private Dictionary<Guid, FacetaModel> ListaFacetasPropuestasCache
        {
            get
            {
                return mGnossCache.ObtenerDeCacheLocal($"{ProyectoSeleccionado.Clave}_facetas_propuestas") as Dictionary<Guid, FacetaModel>;
            }
            set
            {
                mGnossCache.AgregarObjetoCacheLocal(ProyectoSeleccionado.Clave, $"{ProyectoSeleccionado.Clave}_facetas_propuestas", value);
            }
        }

        private List<Propiedad> ListaPropiedadesCache
        {
            get
            {
                return mGnossCache.ObtenerDeCacheLocal($"{ProyectoSeleccionado.Clave}_lista_propiedades") as List<Propiedad>;
            }
            set
            {
                mGnossCache.AgregarObjetoCacheLocal(ProyectoSeleccionado.Clave, $"{ProyectoSeleccionado.Clave}_lista_propiedades", value);
            }
        }

        private List<OntologiaCompleta> ListaOntologiasCompletas
        {
            get
            {
                if (mListaOntologiasCompletas == null || mListaOntologiasCompletas.Count == 0)
                {
                    mListaOntologiasCompletas = new List<OntologiaCompleta>();
                    DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                    DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    
                    if (ParametroProyecto.ContainsKey("ProyectoIDPatronOntologias"))
                    {
                        Guid proyectoIDPatronOntologias;
                        Guid.TryParse(ParametroProyecto["ProyectoIDPatronOntologias"], out proyectoIDPatronOntologias);
                        documentacionCN.ObtenerOntologiasProyecto(proyectoIDPatronOntologias, dataWrapperDocumentacion, false, true, true);
                    }

                    documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, true, true);
                    documentacionCN.Dispose();

                    byte[] arrayOnto = null;
                    foreach (Documento documento in dataWrapperDocumentacion.ListaDocumento)
                    {
                        Dictionary<string, List<EstiloPlantilla>> listaEstilos;
                        arrayOnto = ControladorDocumentacion.ObtenerOntologia(documento.DocumentoID, out listaEstilos, ProyectoSeleccionado.Clave, null, null, false);
                        LectorXmlConfig lectorXmlConfig = new LectorXmlConfig(documento.DocumentoID, ProyectoSeleccionado.Clave, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mLoggerFactory.CreateLogger<LectorXmlConfig>(), mLoggerFactory);

                        Dictionary<string, List<EstiloPlantilla>> xml = lectorXmlConfig.ObtenerConfiguracionXml();

                        Ontologia ontologia = new Ontologia(arrayOnto, true);
                        ontologia.LeerOntologia();
                        ontologia.EstilosPlantilla = listaEstilos;
                        ontologia.IdiomaUsuario = UtilIdiomas.LanguageCode;
                        ontologia.OntologiaID = documento.DocumentoID;
                        OntologiaCompleta ontologiaCompleta = new OntologiaCompleta
                        {
                            DocumentoID = documento.DocumentoID,
                            Enlace = documento.Enlace,
                            Ontologia = ontologia,
                            XML = xml
                        };

                        mListaOntologiasCompletas.Add(ontologiaCompleta);
                    }
                }

                return mListaOntologiasCompletas;
            }
        }

        private AdministrarFacetasViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    ListaFacetasPropuestasCache = new Dictionary<Guid, FacetaModel>();

                    mPaginaModel = new AdministrarFacetasViewModel();
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

                    mPaginaModel.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();

                    mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;

                    CargarOntologias();

                    ControladorFacetas contrFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorFacetas>(), mLoggerFactory);

                    mPaginaModel.ListaFacetas = contrFacetas.CargarListadoFacetas();
                    try
                    {
                        CargarFacetasPropuestas();
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Error al crear facetas propuestas",mlogger);
                    }
                }
                return mPaginaModel;
            }
        }

        #endregion
    }
}