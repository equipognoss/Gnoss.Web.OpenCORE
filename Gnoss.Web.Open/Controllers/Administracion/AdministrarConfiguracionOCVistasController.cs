using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOCVistas;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Gnoss.Web.Open.Controllers.ControllerBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Gnoss.Web.Open.Controllers.Administracion
{
    public class AdministrarConfiguracionOCVistasController : AdministrarOCControllerBase
    {
        protected string path = @"C:\TFG-Andra\DirectorioOntologia";
        private XmlSerializer serializerXML = new XmlSerializer(typeof(Config));
        private XmlSerializer serializerOWL = new XmlSerializer(typeof(RDF));
        private Config mConfig;
        private RDF mRDF;
        public string EntidadPrincipal;
        private string NombreOC;
        


        public AdministrarConfiguracionOCVistasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }
        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult Index(Guid ontologiaID)
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "estructura edicion edicionPaginas listado no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.GrafoConocimiento;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.GrafoConocimiento_ObjetosConocimiento_Ontologias;

            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "GRAFODECONOCIMIENTO");
            ViewBag.HeaderTitle = "";

            ObtenerOntologia(ontologiaID);

            //Recogemos la lista de entidades
            List<Entidad> listaEntidades = mConfig.EspefEntidad.Entidad;

            ObtenerEntidadPrincipal(ontologiaID);

            //Recogemos las propiedades de cada entidad
            Dictionary<string, List<PropiedadXML>> propiedades = ObtenerPropiedades(mConfig);

            //Creamos el modelo
            VistasModel modelo = ObtenerModelo(listaEntidades, propiedades);
            modelo.OntologiaID = ontologiaID.ToString();
            ViewBag.VistasModel = modelo;

			return View();
        }

        /// <summary>
        /// Obtiene el fichero XML y OWL de la ontología cuyo ID es pOntologiaID
        /// </summary>
        /// <param name="pOntologiaID"></param>
        private void ObtenerOntologia(Guid pOntologiaID)
        {
            using (XmlReader xmlReader = ObtenerXmlReaderXML(pOntologiaID))
            {
                mConfig = (Config)serializerXML.Deserialize(xmlReader);
            }

            using (XmlReader xmlReader = ObtenerXmlReaderOntologia(pOntologiaID))
            {
                mRDF = (RDF)serializerOWL.Deserialize(xmlReader);
            }
        }

        /// <summary>
        /// Devuelve una lista con las propiedades del grupo
        /// </summary>
        /// <param name="grupo">Grupo del cual se quiere mostrar las propiedades</param>
        private List<NameProp> BusquedaProfundidad(object[] pContenido, List<NameProp> pLista)
        {
            List<object> elementos = pContenido.ToList<object>();
            foreach (object e in elementos)
            {
                if (e is Grupo)
                {
                    Grupo group = (Grupo)e;
                    if (group.Tipo != "titulo" && group.Tipo != "subtitulo" && group.Contenido.Length != 0)
                    {
                        BusquedaProfundidad(group.Contenido, pLista);
                    }
                }
                else if (e is NameProp)
                {
                    NameProp nameProp = (NameProp)e;
                    if (!nameProp.Valor.Contains("@@@")) 
                    {
                        pLista.Add(nameProp);
                    }
                }
            }
            return pLista;
        }

        /// <summary>
        /// Obtiene las entidades junto con sus las propiedades del bloque EspefPropiedad
        /// </summary>
        private Dictionary<string, List<PropiedadXML>> ObtenerPropiedades(Config pConfig)
        {
            Dictionary<string, List<PropiedadXML>> modelo = new Dictionary<string, List<PropiedadXML>>();
            List<PropiedadXML> propiedades = pConfig.EspefPropiedad.Propiedad;
            foreach (PropiedadXML propiedad in propiedades)
            {
                if (!modelo.ContainsKey(propiedad.EntidadID))
                {
                    modelo.Add(propiedad.EntidadID, new List<PropiedadXML>() { propiedad });
                }
                else
                {
                    modelo[propiedad.EntidadID].Add(propiedad);
                }
            }
            return modelo;
        }

        /// <summary>
        /// Crea el modelo recibiendo como parámetro una lista de entidades y un diccionario con las propiedades de cada una
        /// </summary>
        /// <param name="pListaEntidades"></param>
        /// <param name="pPropiedades"></param>
        /// <returns></returns>
        private VistasModel ObtenerModelo(List<Entidad> pListaEntidades, Dictionary<string, List<PropiedadXML>> pPropiedades) 
        {
            VistasModel vistasModel = new VistasModel();
            vistasModel.EntidadPrincipal = EntidadPrincipal;
            vistasModel.NombreOC = NombreOC;
            vistasModel.Entidades = new List<EntidadModel>();

            foreach (string entidad in pPropiedades.Keys)  
            {
                List<PropiedadModel> propiedadModels = new List<PropiedadModel>();
                //Introducimos las propiedades para el panel lateral
                foreach (PropiedadXML propiedad in pPropiedades[entidad])
                {
                    propiedadModels.Add(new PropiedadModel() { ID = AcortarNombre(propiedad.ID), Nombre = propiedad.AtrNombre.Text, Tipo = ObtenerTipoPropiedad(propiedad.ID, entidad) });
                }
                propiedadModels = propiedadModels.OrderBy(propiedad => propiedad.ID).ToList();

                Entidad e = pListaEntidades.FirstOrDefault(e => e.ID == entidad);
                if (e != null)
                {
                    List<PropiedadModel> propiedadesOrdenEntidad = new List<PropiedadModel>();
                    List<PropiedadModel> propiedadesOrdenEntidadLectura = new List<PropiedadModel>();

                    //Propiedades de la vista de edición
                    if (e.OrdenEntidad != null) 
                    { 
                        object[] grupo = e.OrdenEntidad.Contenido;
                        List<NameProp> nameProps = BusquedaProfundidad(grupo, new List<NameProp>());
                        foreach (NameProp nameProp in nameProps)
                        {
                            propiedadesOrdenEntidad.Add(new PropiedadModel() { ID = AcortarNombre(nameProp.Valor), Tipo = ObtenerTipoPropiedad(nameProp.Valor, e.ID) });
                        }
                    }

                    //Propiedades de la vista de visualizacion
                    if (e.OrdenEntidadLectura != null) 
                    {
                        object[] grupoVisualizacion = e.OrdenEntidadLectura.Contenido;
                        List<NameProp> namePropsVisualizacion = BusquedaProfundidad(grupoVisualizacion, new List<NameProp>());
                        foreach (NameProp nameProp in namePropsVisualizacion)
                        {
                            propiedadesOrdenEntidadLectura.Add(new PropiedadModel() { ID = AcortarNombre(nameProp.Valor), Tipo = ObtenerTipoPropiedad(nameProp.Valor, e.ID), Atributos = ObtenerAtributosPropiedad(nameProp) });
                        }
                    }

                    //Representantes
                    List<Representante> representantes = null;
                    if (e.Representantes != null)
                    {
                        e.Representantes.Representante.ForEach(r => r.Valor = AcortarNombre(r.Valor));
                        representantes = e.Representantes.Representante;
                    }

                    //AtrNombres
                    List<AtrNombre> atrNombres = null;
                    if (e.AtrNombre != null)
                    {
                        atrNombres = e.AtrNombre.FindAll(e => e.Text != null);
                    }

                    //AtrNombreLecturas
                    List<AtrNombreLectura> atrNombreLecturas = null;
                    if (e.AtrNombreLectura != null)
                    {
                        atrNombreLecturas = e.AtrNombreLectura.FindAll(e => e.Text != null);
                    }

                    vistasModel.Entidades.Add(new EntidadModel()
                    {
                        ID = AcortarNombre(e.ID),
                        Propiedades = propiedadModels,
                        PropiedadesOrdenEntidad = propiedadesOrdenEntidad,
                        PropiedadesOrdenEntidadLectura = propiedadesOrdenEntidadLectura,
                        Representantes = representantes,
                        ClaseCssPanel = e.ClaseCssPanel,
                        ClaseCssTitulo = e.ClaseCssTitulo,
                        TagNameTituloEdicion = e.TagNameTituloEdicion,
                        TagNameTituloLectura = e.TagNameTituloLectura,
                        AtrNombres = atrNombres,
                        AtrNombreLecturas = atrNombreLecturas,
                        Microdatos = e.Microdatos,
                        CampoOrden = AcortarNombre(e.CampoOrden),
                        CampoRepresentanteOrden = AcortarNombre(e.CampoRepresentanteOrden)
                    });
                }
                else 
                {
                    vistasModel.Entidades.Add(new EntidadModel()
                    {
                        ID = AcortarNombre(entidad),
                        Propiedades = propiedadModels
                    });
                }
            }

            //Obtenemos los idiomas
            IdiomasOnto idiomas = mConfig.ConfiguracionGeneral.IdiomasOnto;
            List<string> idiomasDisponibles = new List<string>();
            foreach (string idioma in idiomas.IdiomaOnto) 
            {
                idiomasDisponibles.Add(idioma);
            }
            vistasModel.IdiomasDisponibles = idiomasDisponibles;

            //Obtenemos la configuración general
            List<string> configuracionGeneral = CargarConfiguracionGeneral();

            vistasModel.ConfiguracionGeneral = configuracionGeneral;

            return vistasModel;
        }

        /// <summary>
        /// Devuelve el tipo de la propiedad cuyo id es introducido
        /// </summary>
        /// <param name="pIdPropiedad"></param>
        /// <returns></returns>
        private string ObtenerTipoPropiedad(string pIdPropiedad, string pIdEntidad) 
        {
            List<ObjectProperty> objecttypes = mRDF.ObjectProperty;
            List<DatatypeProperty> datatypes = mRDF.DatatypeProperty;
            List<FunctionalProperty> funcionalProperties = mRDF.FunctionalProperty;

            PropiedadXML propiedad = mConfig.EspefPropiedad.Propiedad.Find(propiedad => propiedad.ID == pIdPropiedad && propiedad.EntidadID == pIdEntidad);
            if (propiedad == null)
            {
                return "Propiedad inexistente en el XML";
            }

            ObjectProperty objectProperty = objecttypes.Find(e => e.About == pIdPropiedad && objecttypes.Contains(e));
            if (objectProperty != null)
            {
                if (objectProperty.Range == null) //si el rango es vacío puede ser recíproca o externa
                {
                    if (propiedad != null && propiedad.SeleccionEntidad != null)
                    {
                        return "Externa";
                    }
                    else //una propiedad de rango vacío se debe configurar con un selector de entidad 
                    {
                        return "Falta configurar";
                    }
                }
                else 
                {
                    return "Objeto"; //temporal
                }
            }
            DatatypeProperty data = datatypes.Find(e => e.About == pIdPropiedad);
            if (data != null && data.Range!= null) 
            {
                return TipoDataType(propiedad, data.Range.Resource);
            }

            FunctionalProperty functional = funcionalProperties.Find(e => e.About == pIdPropiedad);
            if (functional != null) 
            {
                if (IsDataType(functional)) //functional de tipo datatype
                {
					return TipoDataType(propiedad, functional.Range.Resource);
                }
                else //functional de tipo objecttype
                {
                    if (functional.Range == null) //si el rango es vacío puede ser recíproca o externa
                    {
                        if (propiedad != null && propiedad.SeleccionEntidad != null)
                        {
                            return "Externa";
                        }
                        else //una propiedad de rango vacío se debe configurar con un selector de entidad 
                        {
                            return "Falta configurar";
                        }
                    }
                    else
                    {
                        return "Objeto"; 
                    }
                }
            }

            return "Propiedad inexistente en el OWL";
        }

        /// <summary>
        /// Indica si una propiedad de tipo Functional es DataType
        /// </summary>
        /// <returns></returns>
        private bool IsDataType(FunctionalProperty pFunctional) 
        {
            if (pFunctional.Range == null) 
            {
                return false;
            }
            if (pFunctional.Range.Resource == "http://www.w3.org/2001/XMLSchema#string" ||
                pFunctional.Range.Resource == "http://www.w3.org/2001/XMLSchema#int" ||
                pFunctional.Range.Resource == "http://www.w3.org/2001/XMLSchema#float" ||
                pFunctional.Range.Resource == "http://www.w3.org/2001/XMLSchema#date" ||
                pFunctional.Range.Resource == "http://www.w3.org/2001/XMLSchema#boolean")
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        private string TipoDataType(PropiedadXML pPropiedad, string pRangeResource) 
        {
            if (pPropiedad.TipoCampo != null) 
            {
				if (pPropiedad.TipoCampo == "Archivo" || pPropiedad.TipoCampo == "Video")
				{
					return pPropiedad.TipoCampo;
				}
				else if (pPropiedad.TipoCampo.Contains("Imagen"))
				{
					return "Imagen";
				}
				else if (pPropiedad.TipoCampo.Contains("Link"))
				{
					return "Link";
				}
			}
            return pRangeResource;
		}

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult GuardarVistas(Guid ontologiaID, List<EntidadModel> listaEntidades, List<string> configuracionGeneral)
        {
            try
            {
                ObtenerOntologia(ontologiaID);

                GuardarConfiguracionGeneral(configuracionGeneral);
                mConfig.EspefEntidad.Entidad = ConstruirEntidades(listaEntidades);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, IndentChars = ("\t"), OmitXmlDeclaration = true }))
                    {
                        serializerXML.Serialize(xmlWriter, mConfig, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
                    }

                    byte[] buffer = ms.ToArray();
                    XmlDocument xmlDocument = new XmlDocument();
                    string xml = Encoding.UTF8.GetString(buffer);

                    //Gestionar el BOM
                    string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (xml.StartsWith(_byteOrderMarkUtf8))
                    {
                        xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
                        //Los tres primeros bytes son el BOM (0xEF, 0xBB, 0xBF)
                        buffer = buffer[3..];
                    }
                    xmlDocument.LoadXml(xml);

                    if (Es.Riam.Gnoss.Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE.ContainsKey(ontologiaID))
                    {
                        Es.Riam.Gnoss.Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE[ontologiaID] = buffer;
                    }
                    else
                    {
                        Es.Riam.Gnoss.Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE.Add(ontologiaID, buffer);
                    }
                    Dictionary<string, List<string>> selectores = new Dictionary<string, List<string>>();
                    selectores = ObtenerSelectores(ontologiaID, xmlDocument);
                    foreach (string grafo in selectores.Keys)
                    {
                        foreach (string selector in selectores[grafo])
                        {
                            FacetaEntidadesExternas facetaEntidad = new FacetaEntidadesExternas();
                            facetaEntidad.Grafo = grafo;
                            facetaEntidad.BuscarConRecursividad = true;
                            facetaEntidad.EsEntidadSecundaria = false;
                            facetaEntidad.EntidadID = $"{UrlIntragnoss}items/{selector}";
                            facetaEntidad.OrganizacionID = UsuarioActual.OrganizacionID;
                            facetaEntidad.ProyectoID = UsuarioActual.ProyectoID;
                            AniadirFacetaEntidad(facetaEntidad);
                        }
                    }

                    CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
                    fileService.GuardarXmlOntologia(buffer, ontologiaID);

                    GuardarCambios(ontologiaID);
                }
            }
            catch(Exception ex)
            {
                GuardarLogError(ex);
				return GnossResultERROR("Contacte con el administrador del Proyecto, no es posible atender la petición.");
			}
            return Ok();
        }

        //private HttpResponseMessage AdministrarIntegracionContinua(EditOntologyViewModel Ontologia, Guid documentoID)
        //{
        //    HttpResponseMessage resultado = new HttpResponseMessage(HttpStatusCode.BadRequest);
        //    if (HayIntegracionContinua)
        //    {
        //        //Para la IC 
        //        Documento doc = GestorDocumental.ListaDocumentos[documentoID];
        //        string nombreOnto = doc.Enlace.Replace(".owl", "");
        //        CallFileService fileService = new CallFileService(mConfigService);
        //        if (Ontologia.FicheroOWL == null)
        //        {

        //            Ontologia.FicheroOWL = fileService.ObtenerOntologia(documentoID, true);

        //        }
        //        if (Ontologia.FicheroCSS == null)
        //        {
        //            Ontologia.FicheroCSS = fileService.DescargarCSSOntologia(documentoID, ".css", true);
        //        }
        //        if (Ontologia.FicheroIMG == null)
        //        {
        //            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
        //            servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");
        //            byte[] buffer = servicioImagenes.ObtenerImagenDeDirectorioOntologia($"Archivos{Path.DirectorySeparatorChar}{documentoID.ToString().Substring(0, 3)}", $"{documentoID}_240", ".jpg");
        //            if (buffer != null && buffer.Any())
        //            {
        //                Ontologia.FicheroIMG = Convert.ToBase64String(buffer);
        //            }
        //        }
        //        if (Ontologia.FicheroJS == null)
        //        {

        //            Ontologia.FicheroJS = Ontologia.FicheroCSS = fileService.DescargarCSSOntologia(documentoID, ".js", true);

        //        }
        //        if (Ontologia.FicheroXML == null)
        //        {
        //            Ontologia.FicheroXML = fileService.ObtenerXmlOntologia(documentoID, true);

        //        }
        //        //Esto es para notificar las ontologias en la IC correctamente.
        //        Ontologia.NameOWL = nombreOnto;
        //        return InformarCambioAdministracion("Ontologias", JsonConvert.SerializeObject(Ontologia, Newtonsoft.Json.Formatting.Indented));
        //    }
        //    else
        //    {
        //        return new HttpResponseMessage(HttpStatusCode.OK);
        //    }
        //}

        
        /// <summary>
        /// Devuelve la estructura básica de un grupo con su lista de propiedades para la ficha de edición.
        /// </summary>
        /// <returns></returns>
        private Grupo ObtenerGrupoPropiedades(List<PropiedadModel> pPropiedades) {

            Grupo grupo = new Grupo { Class = "formtheme02" };
            grupo.Contenido = new object[0];
            grupo.TipoArray = new ElementosOrden[] { ElementosOrden.Grupo };

            Grupo grupofieldset = new Grupo { Class = "fieldset" };
            grupofieldset.Contenido = new object[0];
            grupofieldset.TipoArray = new ElementosOrden[] { ElementosOrden.Grupo };

            Grupo grupopropiedades = new Grupo { Tipo = "fieldset", Class = "mediumLabels" };
            grupopropiedades.Contenido = new object[0];
            grupopropiedades.TipoArray = new ElementosOrden[0];

            foreach (PropiedadModel propiedad in pPropiedades) 
            {
                NameProp nameProp = new NameProp { Valor = CompletarNombre(propiedad.ID) };

                grupopropiedades.Contenido = grupopropiedades.Contenido.Append(nameProp).ToArray();
                grupopropiedades.TipoArray = grupopropiedades.TipoArray.Append(ElementosOrden.NameProp).ToArray();
            }

            grupofieldset.Contenido = grupofieldset.Contenido.Append(grupopropiedades).ToArray();
            grupo.Contenido = grupo.Contenido.Append(grupofieldset).ToArray();

            return grupo;
        }

        /// <summary>
        /// Devuelve la estructura básica de un grupo con su lista de propiedades para la ficha del recurso.
        /// </summary>
        /// <param name="pPropiedades"></param>
        /// <returns></returns>
        private Grupo ObtenerGrupoLecturaPropiedades(List<PropiedadModel> pPropiedades) 
        {
            Grupo grupo = new Grupo { ClassLectura = "group content semanticView" };
            grupo.Contenido = new object[0];
            grupo.TipoArray = new ElementosOrden[] { ElementosOrden.Grupo };

            Grupo grupointerno = new Grupo { ClassLectura = "group group_info" };
            grupointerno.Contenido = new object[0];
            grupointerno.TipoArray = new ElementosOrden[] { ElementosOrden.Grupo };

            Grupo grupopropiedades = new Grupo { ClassLectura = "contentGroup" };
            grupopropiedades.Contenido = new object[0];
            grupopropiedades.TipoArray = new ElementosOrden[0];

            foreach (PropiedadModel propiedad in pPropiedades)
            {
                //Cogemos los atributos
                Dictionary<string, string> atributos = propiedad.Atributos;

                string PropDeEntHija = null;
                string SoloPrimerValor = null;
                string SinTitulo = null;
                string Tipo = null;
                string link = null;
                string target = null;

                if (atributos != null)
                {
                    string value = null;

                    //PropDeEntHija
                    bool hasValue = atributos.TryGetValue("PropDeEntHija", out value);
                    if (hasValue)
                    {
                        PropDeEntHija = value;
                    }

                    //SoloPrimerValor
                    hasValue = atributos.TryGetValue("SoloPrimerValor", out value);
                    if (hasValue)
                    {
                        SoloPrimerValor = value;
                    }

                    //SinTitul
                    hasValue = atributos.TryGetValue("SinTitulo", out value);
                    if (hasValue)
                    {
                        SinTitulo = value;
                    }

                    //Tipo
                    hasValue = atributos.TryGetValue("Tipo", out value);
                    if (hasValue)
                    {
                        Tipo = value;
                    }

                    //link
                    hasValue = atributos.TryGetValue("link", out value);
                    if (hasValue)
                    {
                        link = value;
                    }

                    //target
                    hasValue = atributos.TryGetValue("target", out value);
                    if (hasValue)
                    {
                        target = value;
                    }
                }

                NameProp nameProp = new NameProp
                {
                    Valor = CompletarNombre(propiedad.ID),
                    PropDeEntHija = PropDeEntHija,
                    SoloPrimerValor = SoloPrimerValor,
                    SinTitulo = SinTitulo,
                    Tipo = Tipo,
                    Link = link,
                    Target = target,
                };

                grupopropiedades.Contenido = grupopropiedades.Contenido.Append(nameProp).ToArray();
                grupopropiedades.TipoArray = grupopropiedades.TipoArray.Append(ElementosOrden.NameProp).ToArray();
            }

            grupointerno.Contenido = grupointerno.Contenido.Append(grupopropiedades).ToArray();
            grupo.Contenido = grupo.Contenido.Append(grupointerno).ToArray();

            return grupo;
        }

        /// <summary>
        /// Transforma el modelo recibido desde el XML Builder a un modelo compatible con el XML
        /// </summary>
        /// <param name="pListaEntidades"></param>
        /// <returns></returns>
        private List<Entidad> ConstruirEntidades(List<EntidadModel> pListaEntidades) 
        {
            List<Entidad> entities = new List<Entidad>();
            foreach (EntidadModel entidadJSON in pListaEntidades) {
                if (!string.IsNullOrEmpty(entidadJSON.ID)) 
                {
                    Entidad entidad = new Entidad();
                    //Añadimos el ID
                    entidad.ID = CompletarNombre(entidadJSON.ID);

                    //Añadimos el bloque de orden de entidad (ficha de edición)
                    if (entidadJSON.PropiedadesOrdenEntidad != null) 
                    {
                        entidad.OrdenEntidad = new OrdenEntidad { Contenido = new object[0] };
                        entidad.OrdenEntidad.Contenido = entidad.OrdenEntidad.Contenido.Append(ObtenerGrupoPropiedades(entidadJSON.PropiedadesOrdenEntidad)).ToArray();
                        entidad.OrdenEntidad.TipoArray = new ElementosOrden[] { ElementosOrden.Grupo };
                    }
                    
                    //Añadimos el bloque de orden entidad lectura (ficha de visualización)
                    if (entidadJSON.PropiedadesOrdenEntidadLectura != null)
                    {
                        entidad.OrdenEntidadLectura = new OrdenEntidadLectura { Contenido = new object[0] };
                        entidad.OrdenEntidadLectura.Contenido = entidad.OrdenEntidadLectura.Contenido.Append(ObtenerGrupoLecturaPropiedades(entidadJSON.PropiedadesOrdenEntidadLectura)).ToArray();
                        entidad.OrdenEntidadLectura.TipoArray = new ElementosOrden[] { ElementosOrden.Grupo };
                    }

                    //Añadimos los representantes
                    List<Representante> representantes = entidadJSON.Representantes;
                    if (representantes != null && representantes.Count != 0) 
                    {
                        Representantes reps = new Representantes();
                        reps.Representante = new List<Representante>();
                        foreach (Representante representante in representantes) 
                        {
                            reps.Representante.Add(new Representante() { Tipo = representante.Tipo, NumCaracteres = representante.NumCaracteres, Valor = CompletarNombre(representante.Valor) });
                        }
                        entidad.Representantes = reps;
                    }

                    //Añadimos las clases css
                    entidad.ClaseCssPanel = entidadJSON.ClaseCssPanel;
                    entidad.ClaseCssTitulo = entidadJSON.ClaseCssTitulo;

                    //Añadimos las etiquetas HTML
                    entidad.TagNameTituloEdicion = entidadJSON.TagNameTituloEdicion;
                    entidad.TagNameTituloLectura = entidadJSON.TagNameTituloLectura;

                    //Añadimos los títulos
                    //AtrNombre (Edición)
                    List<AtrNombre> atrNombres = entidadJSON.AtrNombres;
                    if (atrNombres != null && atrNombres.Count != 0)
                    {
                        atrNombres.ForEach(e => e.Text = HttpUtility.HtmlDecode(e.Text));
                        entidad.AtrNombre = atrNombres;
                    }

                    //AtrNombreLectura (Visualización)
                    List<AtrNombreLectura> atrNombreLecturas = entidadJSON.AtrNombreLecturas;
                    if (atrNombreLecturas != null && atrNombreLecturas.Count != 0)
                    {
                        atrNombreLecturas.ForEach(e => e.Text = HttpUtility.HtmlDecode(e.Text));
                        entidad.AtrNombreLectura = atrNombreLecturas;
                    }

                    //Microdatos
                    entidad.Microdatos = entidadJSON.Microdatos;

                    //Campo Orden
                    entidad.CampoOrden = CompletarNombre(entidadJSON.CampoOrden);

                    //Campo Representante Orden
                    entidad.CampoRepresentanteOrden = CompletarNombre(entidadJSON.CampoRepresentanteOrden);

                    entities.Add(entidad);
                }
            }
            return entities;
        }

        /// <summary>
        /// Devuelve el diccionario de atributos de la propiedad cuyo id pasamos como parámetro
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Dictionary<string, string> ObtenerAtributosPropiedad(NameProp pNameProp) {
            Dictionary<string, string> atributos = new Dictionary<string, string>();

            if (pNameProp.PropDeEntHija != null) 
            {
                atributos.Add("PropDeEntHija", pNameProp.PropDeEntHija);
            }

            if (pNameProp.SoloPrimerValor != null)
            {
                atributos.Add("SoloPrimerValor", pNameProp.SoloPrimerValor);
            }

            if (pNameProp.SinTitulo != null)
            {
                atributos.Add("SinTitulo", pNameProp.SinTitulo);
            }

            if (pNameProp.Tipo != null)
            {
                atributos.Add("Tipo", pNameProp.Tipo);
            }

            if (pNameProp.Link != null)
            {
                atributos.Add("link", pNameProp.Link);
            }

            if (pNameProp.Target != null)
            {
                atributos.Add("target", pNameProp.Target);
            }

            return atributos;
        }

        /// <summary>
        /// Guarda la configuración general recibida desde la vista
        /// </summary>
        /// <param name="pConfiguracionGeneral"></param>
        private void GuardarConfiguracionGeneral(List<string> pConfiguracionGeneral) 
        {
            //Menú
            if (pConfiguracionGeneral.Contains("menu-abajo"))
            {
                mConfig.ConfiguracionGeneral.MenuDocumentoAbajo = true;
            }
            else 
            {
                mConfig.ConfiguracionGeneral.MenuDocumentoAbajo = false;
            }

            //Secciones
            if (pConfiguracionGeneral.Contains("ocultar-titulo-descp"))
            {
                mConfig.ConfiguracionGeneral.OcultarTituloDescpImgDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarTituloDescpImgDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-fecha"))
            {
                mConfig.ConfiguracionGeneral.OcultarFechaRec = false;
            }
            else 
            {
                mConfig.ConfiguracionGeneral.OcultarFechaRec = true;
            }

            if (pConfiguracionGeneral.Contains("ocultar-autoria"))
            {
                mConfig.ConfiguracionGeneral.OcultarAutoriaEdicion = new object();
            }
            else 
            {
                mConfig.ConfiguracionGeneral.OcultarAutoriaEdicion = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-publicador"))
            {
                mConfig.ConfiguracionGeneral.OcultarPublicadorDoc = new object();
            }
            else 
            {
                mConfig.ConfiguracionGeneral.OcultarPublicadorDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-categorias"))
            {
                mConfig.ConfiguracionGeneral.OcultarCategoriasDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarCategoriasDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-etiquetas"))
            {
                mConfig.ConfiguracionGeneral.OcultarEtiquetasDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarEtiquetasDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-editores"))
            {
                mConfig.ConfiguracionGeneral.OcultarEditoresDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarEditoresDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-visitas"))
            {
                mConfig.ConfiguracionGeneral.OcultarVisitasDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarVisitasDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-votos"))
            {
                mConfig.ConfiguracionGeneral.OcultarVotosDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarVotosDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-visitas-votos"))
            {
                mConfig.ConfiguracionGeneral.OcultarUtilsDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarUtilsDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-compartido"))
            {
                mConfig.ConfiguracionGeneral.OcultarCompartidoDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarCompartidoDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-compartido-en"))
            {
                mConfig.ConfiguracionGeneral.OcultarCompartidoEnDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarCompartidoEnDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-licencia"))
            {
                mConfig.ConfiguracionGeneral.OcultarLicenciaDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarLicenciaDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-version"))
            {
                mConfig.ConfiguracionGeneral.OcultarVersionDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarVersionDoc = null;
            }

            //Botones
            if (pConfiguracionGeneral.Contains("ocultar-acciones"))
            {
                mConfig.ConfiguracionGeneral.OcultarAccionesDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarAccionesDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-editar"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonEditarDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonEditarDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-crear-versiones"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonCrearVersionDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonCrearVersionDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-enviar-enlace"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonEnviarEnlaceDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonEnviarEnlaceDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-vincular"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonVincularDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonVincularDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-eliminar"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonEliminarDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonEliminarDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-restaurar-version"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonRestaurarVersionDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonRestaurarVersionDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-agregar-categoria"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonAgregarCategoriaDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonAgregarCategoriaDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-agregar-etiqueta"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonAgregarEtiquetasDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonAgregarEtiquetasDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-historial"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonHistorialDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonHistorialDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-bloquear-comentarios"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonBloquearComentariosDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonBloquearComentariosDoc = null;
            }

            if (pConfiguracionGeneral.Contains("ocultar-boton-certificar"))
            {
                mConfig.ConfiguracionGeneral.OcultarBotonCertificarDoc = new object();
            }
            else
            {
                mConfig.ConfiguracionGeneral.OcultarBotonCertificarDoc = null;
            }
        }

        /// <summary>
        /// Devuelve una lista con las opciones de la configuración general del XML
        /// </summary>
        /// <returns></returns>
        private List<string> CargarConfiguracionGeneral() 
        {
            List<string> opciones = new List<string>();

            //Menú
            if (mConfig.ConfiguracionGeneral.MenuDocumentoAbajo == true)
            {
                opciones.Add("menu-abajo");
            }

            //Secciones
            if (mConfig.ConfiguracionGeneral.OcultarTituloDescpImgDoc != null)
            {
                opciones.Add("ocultar-titulo-descp");
            }

            if (mConfig.ConfiguracionGeneral.OcultarFechaRec == false)
            {
                opciones.Add("ocultar-titulo-descp");
            }

            if (mConfig.ConfiguracionGeneral.OcultarAutoriaEdicion != null)
            {
                opciones.Add("ocultar-autoria");
            }

            if (mConfig.ConfiguracionGeneral.OcultarPublicadorDoc != null)
            {
                opciones.Add("ocultar-publicador");
            }

            if (mConfig.ConfiguracionGeneral.OcultarCategoriasDoc != null) 
            {
                opciones.Add("ocultar-categorias");
            }

            if (mConfig.ConfiguracionGeneral.OcultarEditoresDoc != null)
            {
                opciones.Add("ocultar-editores");
            }

            if (mConfig.ConfiguracionGeneral.OcultarVisitasDoc != null)
            {
                opciones.Add("ocultar-visitas");
            }

            if (mConfig.ConfiguracionGeneral.OcultarVotosDoc != null)
            {
                opciones.Add("ocultar-votos");
            }

            if (mConfig.ConfiguracionGeneral.OcultarUtilsDoc != null)
            {
                opciones.Add("ocultar-visitas-votos");
            }

            if (mConfig.ConfiguracionGeneral.OcultarCompartidoDoc != null)
            {
                opciones.Add("ocultar-compartido");
            }

            if (mConfig.ConfiguracionGeneral.OcultarCompartidoEnDoc != null)
            {
                opciones.Add("ocultar-compartido-en");
            }

            if (mConfig.ConfiguracionGeneral.OcultarLicenciaDoc != null)
            {
                opciones.Add("ocultar-licencia");
            }

            if (mConfig.ConfiguracionGeneral.OcultarVersionDoc != null)
            {
                opciones.Add("ocultar-version");
            }

            //Botones
            if (mConfig.ConfiguracionGeneral.OcultarAccionesDoc != null)
            {
                opciones.Add("ocultar-acciones");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonEditarDoc != null)
            {
                opciones.Add("ocultar-boton-editar");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonCrearVersionDoc != null)
            {
                opciones.Add("ocultar-boton-crear-versiones");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonEnviarEnlaceDoc != null)
            {
                opciones.Add("ocultar-boton-enviar-enlace");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonVincularDoc != null)
            {
                opciones.Add("ocultar-boton-vincular");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonEliminarDoc != null)
            {
                opciones.Add("ocultar-boton-eliminar");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonRestaurarVersionDoc != null)
            {
                opciones.Add("ocultar-boton-restaurar-version");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonAgregarCategoriaDoc != null)
            {
                opciones.Add("ocultar-boton-agregar-categoria");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonAgregarEtiquetasDoc != null)
            {
                opciones.Add("ocultar-boton-agregar-etiqueta");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonHistorialDoc != null)
            {
                opciones.Add("ocultar-boton-historial");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonBloquearComentariosDoc != null)
            {
                opciones.Add("ocultar-boton-bloquear-comentarios");
            }

            if (mConfig.ConfiguracionGeneral.OcultarBotonCertificarDoc != null)
            {
                opciones.Add("ocultar-boton-certificar");
            }

            return opciones;
        }

        /// <summary>
        /// Obtiene la entidad principal del Objeto de Conocimiento
        /// </summary>
        /// <param name="pOntologiaID"></param>
        private void ObtenerEntidadPrincipal(Guid pOntologiaID) 
        {
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication); 
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion(); 
            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, true, false); 
            foreach (Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.Documento documento in dataWrapperDocumentacion.ListaDocumento) 
            { 
                byte[] arrayOnto = null;
                Dictionary<string, List<Es.Riam.Semantica.Plantillas.EstiloPlantilla>> listaEstilos;
                arrayOnto = ControladorDocumentacion.ObtenerOntologia(documento.DocumentoID, out listaEstilos, ProyectoSeleccionado.Clave, null, null, false);
                Ontologia ontologia = new Ontologia(arrayOnto, true); 
                ontologia.LeerOntologia();
                ElementoOntologia entidadPrincipal = ontologia.ObtenerEntidadPrincipal();
                if (documento.DocumentoID == pOntologiaID) 
                {
                    EntidadPrincipal = AcortarNombre(entidadPrincipal.Descripcion);
                    NombreOC = documento.Titulo;
                    return;
                }
            }
        }

        

        /// <summary>
        /// Transforma la URI en la notación corta namespace:nombre
        /// </summary>
        /// <param name="pNombreID"></param>
        /// <returns></returns>
        private string AcortarNombre(string pNombreID) 
        {
            if (pNombreID != null) 
            {
				foreach (KeyValuePair<string, string> nspace in Namespaces)
				{
					if (pNombreID.StartsWith(nspace.Value))
					{
						pNombreID = pNombreID.Replace(nspace.Value, nspace.Key + ":");
						return pNombreID;
					}
				}
			}
            return pNombreID;
        }

        /// <summary>
        /// Transforma la notación corta namespace:nombre en la URI
        /// </summary>
        /// <param name="pNombre"></param>
        /// <returns></returns>
        private string CompletarNombre(string pNombre)
        {
            if (!string.IsNullOrEmpty(pNombre)) 
            {
                foreach (KeyValuePair<string, string> nspace in Namespaces)
                {
                    if (pNombre.StartsWith(nspace.Key))
                    {
                        pNombre = pNombre.Replace(nspace.Key + ":", nspace.Value);
                        return pNombre;
                    }
                }
            }
            return pNombre;
        }
    }
}
