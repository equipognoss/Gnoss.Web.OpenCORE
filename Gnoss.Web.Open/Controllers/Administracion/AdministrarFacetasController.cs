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
using Universal.Common;
using Universal.Common.Extensions;

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
		public List<FacetaModel> ListaFacetasPropuestas { get; set; }
	}

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
        public string Path { get; set;}
        public Propiedad Propieadad { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AdministrarFacetasController : ControllerBaseWeb
    {

        public AdministrarFacetasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarFacetasViewModel mPaginaModel = null;

        private DataWrapperFacetas mFacetaDW = null;

        private Dictionary<string, string> mListaOntologias = null;
		private List<OntologiaCompleta> mListaOntologiasCompletas = null;
		List<FacetaModel> mListaFacetasPropuestas = new List<FacetaModel>();
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

            return View(PaginaModel);
        }

        /// <summary>
        /// Nueva Pestaña
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult NuevaFaceta(TipoFaceta TipoFaceta, Guid? pestanyaPropuestaID = null)
        {
            EliminarPersonalizacionVistas();

            FacetaModel faceta = null;
            if (!pestanyaPropuestaID.HasValue || (pestanyaPropuestaID.HasValue && ListaFacetasPropuestasCache != null && !ListaFacetasPropuestasCache.ContainsKey(pestanyaPropuestaID.Value)) || ListaFacetasPropuestasCache == null)
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
                foreach (string text in ObjetoConocimientoAndNames)
                {
                    if (text.Contains("@"))
                    {
                        name = $"{name}{text}|||";
                    }
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
        public ActionResult NuevoFiltro()
        {
            //EliminarPersonalizacionVistas();
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
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
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
            ControladorFacetas contrFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            string errores = contrFacetas.ComprobarErrores(ListaFacetas);
            if (string.IsNullOrEmpty(errores))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
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

        private List<FacetaModel> ModificarOrdenFacetas(List<FacetaModel> pFacetaModel, List<FacetaModel> pListaFacetas)
        {
            List<FacetaModel> listaFacetasDevolver = new List<FacetaModel>();
            foreach (FacetaModel faceta in pListaFacetas)
            {
                FacetaModel facetaNueva = faceta;
                if (!faceta.Deleted && !faceta.Modified)
                {
                    facetaNueva = pFacetaModel.Where(facet => facet.AgrupacionID.Equals(faceta.AgrupacionID)).FirstOrDefault();
                    if (facetaNueva != null)
                    {
                        //Solo modificar las que se han cambiado de orden 
                        if (facetaNueva.Orden != faceta.Orden)
                        {
                            facetaNueva.Orden = faceta.Orden;
                            facetaNueva.Modified = true;
                            listaFacetasDevolver.Add(facetaNueva);
                        }
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

        private void CargarFacetasPropuestas()
        {			
			foreach (OntologiaCompleta ontologiaCompleta in ListaOntologiasCompletas)
            {
                if (ontologiaCompleta.XML != null)
                {
                    EstiloPlantillaConfigGen configGen = (EstiloPlantillaConfigGen)ontologiaCompleta.XML["[ConfiguracionGeneral]"].First();
                    configGen.Ontologia = ontologiaCompleta.Ontologia;
                    ElementoOntologia entidadPrincipal = ontologiaCompleta.Ontologia.ObtenerEntidadPrincipal();
                    string propiedadDescripcion = configGen.PropiedadDescripcion.Key;
                    string propiedadImagen = configGen.PropiedadImagenRepre.Key;
                    string propiedadTitulo = configGen.PropiedadTitulo.Key;
                    if (propiedadDescripcion == null) { propiedadDescripcion = ""; }
                    if (propiedadTitulo == null) { propiedadTitulo = ""; }
                    if (propiedadImagen == null) { propiedadImagen = ""; }

                    var selectoresEntidad = ontologiaCompleta.XML.Where(item => item.Value.Count > 0 && item.Value.First() is EstiloPlantillaEspecifProp && ((EstiloPlantillaEspecifProp)item.Value.First()).SelectorEntidad != null).Select(item => item.Value.First()).ToList();

                    foreach (Propiedad propiedad in entidadPrincipal.Propiedades)
                    {
                        var element = ontologiaCompleta.XML.FirstOrDefault(item => item.Key.Equals(propiedad.Nombre));
                        EstiloPlantillaEspecifProp xmlProp = null;
                        if (element.Value != null)
                        {
                            xmlProp = (EstiloPlantillaEspecifProp)element.Value.First();
                            selectoresEntidad.Remove(xmlProp);
                        }
                        if (propiedad.Rango.EndsWith("#string") && !propiedad.RangoEsFecha || (propiedad.Rango.EndsWith("#string") && xmlProp.SelectorEntidad != null) || propiedad.Rango.Equals(""))
                        {

                            if (xmlProp != null && xmlProp.ImagenMini == null && !xmlProp.FechaConHora && !xmlProp.FechaLibre && !xmlProp.FechaMesAnio)
                            {
                                if (((!propiedadDescripcion.Equals(propiedad.Nombre) || propiedadDescripcion.Equals(propiedadTitulo)) && !propiedadImagen.Equals(propiedad.Nombre)))
                                {
                                    if ((xmlProp.SelectorEntidad == null && !mPaginaModel.ListaFacetas.Any(faceta => faceta.ClaveFaceta.Equals(propiedad.NombreConNamespace) && faceta.ObjetosConocimiento.Contains(propiedad.Ontologia.ConfiguracionPlantilla.Namespace) && string.IsNullOrEmpty(faceta.ClaveFacetaYReprocidad))) || (xmlProp.SelectorEntidad != null && string.IsNullOrEmpty(xmlProp.SelectorEntidad.ConsultaReciproca)))
                                    {
                                        FacetaModel facetaPropuesta = CrearFacetaPropuesta(propiedad, xmlProp, ontologiaCompleta);
                                        aniadirFacetaPropuestaListas(facetaPropuesta, xmlProp);
                                    }
                                }
                            }
                        }
                    }

                    foreach (EstiloPlantillaEspecifProp element in selectoresEntidad)
                    {
                        ElementoOntologia entidad = ontologiaCompleta.Ontologia.Entidades.FirstOrDefault(item => item.TipoEntidad.Equals(element.NombreEntidad));
                        if (!entidadPrincipal.TipoEntidad.Equals(entidad.TipoEntidad))
                        {
                            Propiedad propiedad = ontologiaCompleta.Ontologia.Propiedades.FirstOrDefault(item => item.Nombre.Equals(element.NombreRealPropiedad));

                            string relacionHastaPrincipal = ObtenerCaminoHastaPrincipal(ontologiaCompleta.Ontologia, entidad.TipoEntidad, entidadPrincipal.TipoEntidad, new List<string>());
                            if (!string.IsNullOrEmpty(relacionHastaPrincipal))
                            {
                                var ontologiaDestinoCompl = ListaOntologiasCompletas.FirstOrDefault(item => item.Enlace.Equals(element.SelectorEntidad.Grafo));
                                if (ontologiaDestinoCompl != null && propiedad != null)
                                {
                                    FacetaModel facetaPropuesta = CrearFacetaPropuesta(propiedad, element, ontologiaDestinoCompl);
                                    facetaPropuesta.ClaveFaceta = $"{relacionHastaPrincipal}{facetaPropuesta.ClaveFaceta}";
                                    aniadirFacetaPropuestaListas(facetaPropuesta, element);
								}
                            }
                        }
                    }
                }
            }
            mPaginaModel.ListaFacetasPropuestas = mListaFacetasPropuestas;
		}

        public void aniadirFacetaPropuestaListas(FacetaModel pFacetaPropuesta, EstiloPlantillaEspecifProp pXmlProp)
        {
			bool aniadir = true;
			if (pXmlProp.SelectorEntidad != null)
			{
				aniadir = !mPaginaModel.ListaFacetas.Any(faceta => faceta.ClaveFaceta.Equals(pFacetaPropuesta.ClaveFaceta) && faceta.ObjetosConocimiento.Contains(pFacetaPropuesta.ObjetosConocimiento.First()));

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

			var propiedadRelacionPrincipal = pOntologia.Propiedades.FirstOrDefault(item => item.Rango.Equals(pEntidadDestinoID) && item.Dominio.Contains(pEntidadPrincipalID));

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
			if (pXmlProp!= null && pXmlProp.SelectorEntidad != null)
			{
                string propLink = "";
                if (pXmlProp.SelectorEntidad.PropLinkARecurso != null && pXmlProp.SelectorEntidad.PropLinkARecurso.Count > 0)
                {
                    propLink = pXmlProp.SelectorEntidad.PropLinkARecurso[0];

				}
                else if(pXmlProp.SelectorEntidad.PropiedadesLectura != null && pXmlProp.SelectorEntidad.PropiedadesLectura.Count > 0)
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
					xmlPropRef = (EstiloPlantillaEspecifProp)element.Value.First();
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
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

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

        private List<OntologiaCompleta> ListaOntologiasCompletas
        {
            get
            {
                if (mListaOntologiasCompletas == null)
                {
                    mListaOntologiasCompletas = new List<OntologiaCompleta>();
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

                    byte[] arrayOnto = null;
                    foreach (Documento documento in dataWrapperDocumentacion.ListaDocumento) 
                    {
						Dictionary<string, List<EstiloPlantilla>> listaEstilos = new Dictionary<string, List<EstiloPlantilla>>();
						arrayOnto = ControladorDocumentacion.ObtenerOntologia(documento.DocumentoID, out listaEstilos, ProyectoSeleccionado.Clave, null, null, false);
						LectorXmlConfig lectorXmlConfig = new LectorXmlConfig(documento.DocumentoID, ProyectoSeleccionado.Clave, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD);
                       
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
                            Ontologia= ontologia,
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
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

					mPaginaModel.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();

                    mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;

                    CargarOntologias();

                    ControladorFacetas contrFacetas = new ControladorFacetas(ProyectoSeleccionado, ParametroProyecto, ListaOntologias, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                    mPaginaModel.ListaFacetas = contrFacetas.CargarListadoFacetas();
                    try
                    {
                        CargarFacetasPropuestas();
                    }
                    catch(Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Error al crear facetas propuestas");
                    }
				}
                return mPaginaModel;
            }
        }

        #endregion
    }
}
