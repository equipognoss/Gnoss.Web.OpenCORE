using ConsoleApplication1.OAuth;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC;
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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Universal.Common.Extensions;
using VDS.RDF.Configuration;
using VDS.RDF.Writing;
using static Org.BouncyCastle.Math.EC.ECCurve;
using XMLModel = Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC.XMLModel;

namespace Gnoss.Web.Open.Controllers.Administracion
{
    

    public class AdministrarConfiguracionOCController : AdministrarOCControllerBase
    {
        private Dictionary<string, Dictionary<string, List<string>>> listaOntologiasPropiedades;
        public Dictionary<string, Dictionary<string, List<string>>> ListaOntologiasPropiedades { 
            get 
            {
               return mGnossCache.ObtenerDeCacheLocal("ListaOntologiasPropiedadesOC") as Dictionary<string, Dictionary<string, List<string>>>;
            } set
            {
                mGnossCache.AgregarObjetoCacheLocal(ProyectoSeleccionado.Clave, "ListaOntologiasPropiedadesOC", value);
            }
        }
        public AdministrarConfiguracionOCController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
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
			ViewBag.HeaderParentTitle = "Objeto de conocimiento";
			ViewBag.HeaderTitle = UtilIdiomas.GetText("ADMINISTRACIONPAGINAS", "PAGINAS");


			XmlReader xmlReaderOntologia= ObtenerXmlReaderOntologia(ontologiaID);
			XmlReader xmlReaderXML = ObtenerXmlReaderXML(ontologiaID);

			ConfiguracionOCModel modelo = new ConfiguracionOCModel(xmlReaderXML, xmlReaderOntologia);
            ViewBag.HeaderTitle = "Configuración de " + modelo.XmlFile.ConfiguracionGeneral.Namespace;

			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			ViewBag.IdiomasComunidad = paramCL.ObtenerListaIdiomasDictionary();

            ListaOntologiasPropiedades = new Dictionary<string, Dictionary<string, List<string>>>();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, true, false);
            

            foreach (Documento documento in dataWrapperDocumentacion.ListaDocumento)
            {
                byte[] arrayOnto = null;
                Dictionary<string, List<Es.Riam.Semantica.Plantillas.EstiloPlantilla>> listaEstilos;
                Dictionary<string, List<string>> entidadesPropiedades = new Dictionary<string, List<string>>();
                modelo.ListaOntologias.Add(documento.Enlace.ToLower());
                
                arrayOnto = ControladorDocumentacion.ObtenerOntologia(documento.DocumentoID, out listaEstilos, ProyectoSeleccionado.Clave, null, null, false); 

                Ontologia ontologia = new Ontologia(arrayOnto, true); 
                ontologia.LeerOntologia();

                foreach (KeyValuePair<string, string> pair in ontologia.NamespacesDefinidos)
                {
                    if (!modelo.Prefixes.ContainsKey(pair.Key))
                    {
                        modelo.Prefixes.Add(pair.Key, pair.Value);
                    }
                }
                ElementoOntologia entidadPrincipal = ontologia.ObtenerEntidadPrincipal();
				foreach (ElementoOntologia entidad in ontologia.Entidades)
                {
                    string nombreEntidad = entidadPrincipal.Descripcion.Equals(entidad.Descripcion) ? entidad.Descripcion + " (principal)" : entidad.Descripcion;
                    List<string> listaPropiedades = new List<string>();
                    foreach (Propiedad propiedad in entidad.Propiedades) { 
                        listaPropiedades.Add(propiedad.NombreFormatoUri); 
                    }
                    entidadesPropiedades.Add(nombreEntidad, listaPropiedades);
                }
                modelo.ListaOntologiasPropiedades.Add(documento.Enlace.ToLower(), entidadesPropiedades);
                ListaOntologiasPropiedades.Add(documento.Enlace.ToLower(), entidadesPropiedades);                
            }
            
            ViewBag.ListaOntologiasPropiedades = modelo.ListaOntologiasPropiedades;
            ViewBag.ListaOntologias = modelo.ListaOntologias;
            ViewBag.ListaEntidades = modelo.XmlFile.EspefEntidad.Entidad.Select(x => x.ID).ToList();
            ViewBag.ListaPropiedades = modelo.Properties;

            ViewBag.Multidioma = modelo.XmlFile.ConfiguracionGeneral.MultiIdioma;

            return View(modelo);
        }

        [HttpPost]
        public ActionResult CargarEntidades(string pOntologia)
        {
            List<string> entidades = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> fila in ListaOntologiasPropiedades)
            {
                if (fila.Key.Equals(pOntologia))
                {
                    foreach (KeyValuePair<string, List<string>> par in fila.Value)
                    {
                        entidades.Add(par.Key);
                    }

                }
            }
            return Json(entidades);
        }

        [HttpPost]
        public ActionResult CargarPropiedades(string pOntologia, string pEntidad)
        {
            List<string> propiedades = new List<string>();
            if (string.IsNullOrEmpty(pOntologia) && string.IsNullOrEmpty(pEntidad))
            {
                return Json(propiedades);
            }
            if (pOntologia == null)
            {
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> fila in ListaOntologiasPropiedades)
                {
                    foreach (KeyValuePair<string, List<string>> par in fila.Value)
                    {
                        if (par.Key.StartsWith(pEntidad))
                        {
                            foreach (string propiedad in par.Value)
                            {
                                propiedades.Add(propiedad);
                            }
                        }

                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> fila in ListaOntologiasPropiedades)
                {
                    if (fila.Key.Equals(pOntologia))
                    {
                        foreach (KeyValuePair<string, List<string>> par in fila.Value)
                        {
                            if (par.Key.Equals(pEntidad))
                            {
                                foreach (string propiedad in par.Value)
                                {
                                    propiedades.Add(propiedad);
                                }
                            }

                        }

                    }
                }
            }
            
            return Json(propiedades);
        }
        [HttpPost]
        public ActionResult Save(Guid ontologiaID)
        {
            GuardarLogAuditoria();
            XmlReader xmlReaderXML = ObtenerXmlReaderXML(ontologiaID);
			XmlSerializer serializer = new XmlSerializer(typeof(XMLModel));
            XMLModel config = new XMLModel();
            using (XmlReader xmlReader = xmlReaderXML)
            {
                config = (XMLModel)serializer.Deserialize(xmlReader);
            }
            //Almacenamos la informacion en los archivos correspondientes
            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, IndentChars = ("\t"), OmitXmlDeclaration = true }))
                {
                    parseConfiguracionGeneral(Request, config);
                    parseEspefPropiedad(Request, config);
                    serializer.Serialize(xmlWriter, config);
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

            return Redirect(ProyectoSeleccionado.UrlPropia(UtilIdiomas.LanguageCode) + $"/comunidad/" + Comunidad.ShortName + $"/{UtilIdiomas.GetText("URLSEM", "ADMINISTRARCONFIGURACIONOC")}" + "/" + ontologiaID);

            //TODO: PONER BIEN LA REDIRECCION EN FUNCION DE LOS PARAMETROS Y NO FIJA
            //return Redirect("http://depuracion.net/comunidad/testing3/administrar-configuracion-oc/ontologiaID");
        }

        private void limpiarListas(XMLModel config)
        {
            if (config.ConfiguracionGeneral.IdiomasOnto.IdiomaOnto != null)
            {
                config.ConfiguracionGeneral.IdiomasOnto.IdiomaOnto = new List<string>();
            }           
            
            

        }
        private object parsearBool(string value, object whenTrue, object whenFalse)
        {
            if (value.Equals("True"))
            {
                return whenTrue;
            }
            else
            {
                return whenFalse;
            }
        }

        private object parsearCheck(string value, object whenTrue, object whenFalse)
        {
            if (value.Equals("on"))
            {
                return whenTrue;
            }
            else
            {
                return whenFalse;
            }
        }
        private void parseConfiguracionGeneral(HttpRequest request, XMLModel config)
        {
            limpiarListas(config);
            config.ConfiguracionGeneral.HtmlNuevo = true;
            config.ConfiguracionGeneral.TituloDoc.Text = request.Form["titulo"];
            if (string.IsNullOrEmpty(request.Form["descripcion"]))
            {
                //El titulo es obligatorio pero la descripcion se puede dejar en blanco. En ese caso, en el XML se guarda como el titulo
                config.ConfiguracionGeneral.DescripcionDoc.Text = request.Form["titulo"];
            }
            else
            {
                config.ConfiguracionGeneral.DescripcionDoc.Text = request.Form["descripcion"];
            }
            config.ConfiguracionGeneral.Namespace = request.Form["namespace"];
            config.ConfiguracionGeneral.CategorizacionTesauroGnossObligatoria = (bool)parsearBool(request.Form["categorizacion"], true, false);
            KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> imagenPrincipal = request.Form.Where(x => x.Key.Contains("imagenPrincipal")).ToList().FirstOrDefault();
            if (!string.IsNullOrEmpty(imagenPrincipal.Key))
            {
                config.ConfiguracionGeneral.ImagenDoc = new ImagenDoc();
                string[] propiedadesImagenPrincipal = imagenPrincipal.Key.Split('@');
                string entidadID = propiedadesImagenPrincipal[0];
                string propiedadImagenPrincipal = propiedadesImagenPrincipal[1];
                config.ConfiguracionGeneral.ImagenDoc.EntidadID = entidadID;
                config.ConfiguracionGeneral.ImagenDoc.Text = propiedadImagenPrincipal;
            }
            else
            {
                config.ConfiguracionGeneral.ImagenDoc = null;
            }
            config.ConfiguracionGeneral.MultiIdioma = parsearCheck(request.Form["multidioma"], string.Empty, null);
            List<string> idiomas = request.Form["idiomas"].ToString().Split(',').ToList();
            foreach (string idioma in idiomas)
            {
                config.ConfiguracionGeneral.IdiomasOnto.IdiomaOnto.Add(idioma);
            }

            if (config.ConfiguracionGeneral.PropiedadesOntologia == null)
            {
                config.ConfiguracionGeneral.PropiedadesOntologia = new PropiedadesOntologia();
            }
            if (!string.IsNullOrEmpty(request.Form["servicioCreacion"]))
            {
                config.ConfiguracionGeneral.PropiedadesOntologia.UrlServicioCreacionRecurso = request.Form["servicioCreacion"];
            }
            else config.ConfiguracionGeneral.PropiedadesOntologia.UrlServicioCreacionRecurso = null;
            if (!string.IsNullOrEmpty(request.Form["servicioComplementarioCreacion"]))
            {
                config.ConfiguracionGeneral.PropiedadesOntologia.UrlServicioComplementarioCreacionRecurso = request.Form["servicioComplementarioCreacion"];
            }
            else config.ConfiguracionGeneral.PropiedadesOntologia.UrlServicioComplementarioCreacionRecurso = null;
            if (!string.IsNullOrEmpty(request.Form["servicioComplementarioCreacionSincrono"]))
            {
                config.ConfiguracionGeneral.PropiedadesOntologia.UrlServicioComplementarioCreacionRecursoSincrono = request.Form["servicioComplementarioCreacionSincrono"];
            }
            else
                config.ConfiguracionGeneral.PropiedadesOntologia.UrlServicioComplementarioCreacionRecursoSincrono = null;
            if (!string.IsNullOrEmpty(request.Form["servicioEliminacion"]))
            {
                config.ConfiguracionGeneral.PropiedadesOntologia.UrlServicioEliminacionRecurso = request.Form["servicioEliminacion"];
            }
            else
                config.ConfiguracionGeneral.PropiedadesOntologia.UrlServicioEliminacionRecurso = null;
        }

        private string getCampo(List<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> camposParsear, string nombreCampo)
        {
            return camposParsear.Where(x => x.Key.Split("@")[2].Equals(nombreCampo)).Select(x => x.Value).ToList().FirstOrDefault().ToString();
        }

        private List<string> getCampos (List<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> camposParsear, string nombreCampo)
        {
            return camposParsear.Where(x => x.Key.Split("@")[2].Equals(nombreCampo)).Select(x => x.Value.ToString()).ToList();
        }
    

        private void parseEspefPropiedad(HttpRequest request, XMLModel config)
        {
            foreach(PropiedadXML p in config.EspefPropiedad.Propiedad)
            {
                List<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> camposParsear = request.Form.Where(x => x.Key.StartsWith(p.EntidadID+"@"+p.ID)).ToList();
                p.AtrNombre.Text = getCampo(camposParsear, "Nombre");
                p.AtrNombreLectura = new List<AtrNombreLectura>();
                p.AtrNombreLectura.Add(new AtrNombreLectura { Text = getCampo(camposParsear, "NombreLectura") });
                string tipoPropiedad = getCampo(camposParsear, "TipoPropiedad");
                parsePropiedad(tipoPropiedad, request, camposParsear, p);
            }

        }
        
        private void parsePropiedad(string tipoPropiedad, HttpRequest request, List<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> camposParsear, PropiedadXML p) 
        {
            switch(tipoPropiedad)
            {
                case "auxiliar":
                    break;
                case "external":
                    string abrirNuevaPestanya = getCampo(camposParsear, "nuevaPestanya");
                    string tipoSeleccion = getCampo(camposParsear, "Tipo");
                    string grafoReferenciado = getCampo(camposParsear, "grafoReferenciado");
                    string entidadReferenciada = getCampo(camposParsear, "entidadReferenciada").Replace(" (principal)", "");
                    string propiedadReferenciada = getCampo(camposParsear, "propiedadReferenciada");
                    List<string> propiedadesMostrar = getCampo(camposParsear, "propiedadesSeleccionadas").Split(",").ToList();

                    //Sólo se cambia la información del XML si se han elegido estos campos. Así evitamos que se vacíen selectores de entidad cuando no se abren los modales de configuracion
                    if(!string.IsNullOrEmpty(grafoReferenciado) && !string.IsNullOrEmpty(entidadReferenciada) && !string.IsNullOrEmpty(propiedadReferenciada))
                    {
                        SeleccionEntidad se = new SeleccionEntidad();
                        se.NuevaPestanya = (bool)parsearCheck(abrirNuevaPestanya, true, false);
                        se.TipoSeleccion = tipoSeleccion;
                        se.Grafo = grafoReferenciado;
                        se.UrlTipoEntSolicitada = entidadReferenciada;
                        se.PropsEntEdicion = new PropsEntEdicion();
                        se.PropsEntEdicion.NameProp = propiedadReferenciada;

                        se.PropsEntLectura = new PropsEntLectura();
                        se.PropsEntLectura.Propiedad = new List<PropiedadXML>();
                        foreach (string propiedadMostrar in propiedadesMostrar.Where(x => !string.IsNullOrEmpty(x)).ToList())
                        {
                            PropiedadXML pxml = new PropiedadXML();
                            pxml.ID = propiedadMostrar;
                            pxml.EntidadID = entidadReferenciada;
                            pxml.AtrNombreLectura = new List<AtrNombreLectura> { new AtrNombreLectura { Text = propiedadMostrar } };
                            se.PropsEntLectura.Propiedad.Add(pxml);
                        }
                        se.LinkARecurso = new LinkARecurso { IrAComunidad = true };
                        se.Reciproca = (bool)parsearCheck(getCampo(camposParsear, "reciproca"), true, false) ? new Reciproca() : null;
                        p.SeleccionEntidad = se;

                    }
                    break;
                case "numerical":
                    break;
                case "string":
                    #region tipoCampo
                    string tipoCampo = getCampo(camposParsear, "tipoString");
                    if (!tipoCampo.Equals("String"))
                    {
                        p.TipoCampo = tipoCampo;
                    }
                    else p.TipoCampo = null;
                    #endregion
                    #region valoresCombo
                    

                    string listadoValores = getCampo(camposParsear, "listadoValores");
                    if (!string.IsNullOrEmpty(listadoValores))
                    {
                        p.valoresCombo = new ValoresCombo();
                        string valorDefecto = getCampo(camposParsear, "valorDefecto");
                        if (!string.IsNullOrEmpty(valorDefecto))
                        {
                            p.valoresCombo.valorDefecto = valorDefecto;
                        }
                        p.valoresCombo.valorCombo = new List<string>();
                        foreach (string valor in listadoValores.Split(","))
                        {
                            if (!valor.Equals(""))
                            {
								p.valoresCombo.valorCombo.Add(valor);
							}
                            
                        }
                    }
                    #endregion
                    #region miniaturas y opensd
                    List<string> miniaturas = getCampos(camposParsear, "miniatura");
                    if (miniaturas.Count > 0)
                    {
                        if(p.ImgMiniVP == null)
                        {
                            p.ImgMiniVP = new ImgMiniVP();
                        }
                        p.ImgMiniVP.Size = new List<Size>();
                        for (int i = 0; i < miniaturas.Count; i += 3)
                        {
                            string tipo = miniaturas[i];
                            string ancho = miniaturas[i + 1];
                            string alto = miniaturas[i + 2];

                            Size nuevaMin = new Size { Alto = alto.ToInt32(), Ancho = ancho.ToInt32(), Tipo = tipo};
                            p.ImgMiniVP.Size.Add(nuevaMin);
                        }
                    }

                    string opensdAncho = getCampo(camposParsear, "opensdAncho");
                    string opensdAlto = getCampo(camposParsear, "opensdAlto");
                    p.UsarOpenSeaDragon = new UsarOpenSeaDragon();
                    if (string.IsNullOrEmpty(opensdAncho) && string.IsNullOrEmpty(opensdAlto))
                    {
                        p.UsarOpenSeaDragon = null;
                    }
                    if (!string.IsNullOrEmpty(opensdAncho))
                    {
                        p.UsarOpenSeaDragon.PropiedadAnchoID = opensdAncho;
                    }
                    if (!string.IsNullOrEmpty(opensdAlto))
                    {
                        p.UsarOpenSeaDragon.PropiedadAltoID = opensdAlto;
                    }
                    #endregion
                    #region multidioma
                    string multiIdiomaGeneral = request.Form["multidioma"];
                    string multiIdiomaPropiedad = getCampo(camposParsear, "multidioma");
                    if(multiIdiomaGeneral.Equals("on") && multiIdiomaPropiedad.Equals("off"))
                    {
                        p.MultiIdioma = "false";
                    }
                    else
                    {
                        p.MultiIdioma = null;
                    }
                    #endregion

                    break;
                case "date":
                    p.GuardarFechaComoEntero = "";
                    break;
                case "boolean":
                    break;
                default:
                    break;
            }
        }
    }
}
