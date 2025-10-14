using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Gnoss.Web.Open.Controllers.ControllerBase
{
	public class AdministrarOCControllerBase : ControllerAdministrationWeb
	{
		protected Dictionary<string, string> Namespaces { get; private set; }
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarOCControllerBase(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarOCControllerBase> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
		{
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

		/// <summary>
		/// Obtiene el fichero XML y OWL de la ontología cuyo ID es pOntologiaID
		/// </summary>
		/// <param name="pOntologiaID"></param>
		protected XmlReader ObtenerXmlReaderXML(Guid pOntologiaID)
		{
			CallFileService servicioArc = new CallFileService(mConfigService, mLoggingService);

			byte[] byteArrayXML = servicioArc.ObtenerXmlOntologiaBytes(pOntologiaID);
			MemoryStream stream = new MemoryStream(byteArrayXML);

			return XmlReader.Create(stream);

		}

		protected XmlReader ObtenerXmlReaderOntologia(Guid pOntologiaID)
		{
			CallFileService servicioArc = new CallFileService(mConfigService, mLoggingService);
			byte[] byteArrayRDF = servicioArc.ObtenerOntologiaBytes(pOntologiaID);
			ObtenerNamespaces(byteArrayRDF);
			MemoryStream stream = new MemoryStream(byteArrayRDF);

			return XmlReader.Create(stream);
				
		}


		/// <summary>
		/// Recoge los namespaces que encuentra en el OWL de la ontología
		/// </summary>
		/// <param name="pOntologiaBytes"></param>
		private void ObtenerNamespaces(byte[] pOntologiaBytes)
		{
			string xml = Encoding.Default.GetString(pOntologiaBytes);
			Namespaces = XDocument.Parse(xml).Root.Attributes().ToDictionary(x => x.Name.LocalName, x => x.Value);
		}

		/// <summary>
		/// Añade a la base de datos la faceta entidad externa pasada por parámetro
		/// </summary>
		/// <param name="facetaEntidad">Faceta entidad externa a añadir a la web</param>
		protected void AniadirFacetaEntidad(FacetaEntidadesExternas facetaEntidad)
		{
			if (!mEntityContext.FacetaEntidadesExternas.Any(facetaEntidadExterna => facetaEntidadExterna.OrganizacionID.Equals(facetaEntidad.OrganizacionID) && facetaEntidadExterna.ProyectoID.Equals(facetaEntidad.ProyectoID) && facetaEntidadExterna.EntidadID.Equals(facetaEntidad.EntidadID)) && !mEntityContext.FacetaEntidadesExternas.Local.Any(facetaEntidadExterna => facetaEntidadExterna.OrganizacionID.Equals(facetaEntidad.OrganizacionID) && facetaEntidadExterna.ProyectoID.Equals(facetaEntidad.ProyectoID) && facetaEntidadExterna.EntidadID.Equals(facetaEntidad.EntidadID)))
			{
				mEntityContext.FacetaEntidadesExternas.Add(facetaEntidad);
			}
		}

		/// <summary>
		/// Devuelve los selectores del archivo XML
		/// </summary> 
		/// <returns>La clave es el grafo y el valor la lista de selectores del grafo</returns>
		protected Dictionary<string, List<string>> ObtenerSelectores(Guid pOntologiaID, XmlDocument pXmlDocument)
		{
			string clase = string.Empty;
			Dictionary<string, List<string>> dicSelectores = new Dictionary<string, List<string>>();
			List<string> vSelectores = new List<string>();
			if (EspefEntidad(pXmlDocument) != null)
			{
				XmlNodeList listaSeleccionEntidad = EspefPropiedad(pXmlDocument).SelectNodes($"Propiedad/SeleccionEntidad");
				foreach (XmlNode seleccionEntidad in listaSeleccionEntidad)
				{
					XmlNode nodoGrafo = seleccionEntidad.SelectSingleNode("Grafo");
					if (nodoGrafo != null)
					{
						string nombreGrafo = nodoGrafo.InnerText;

						List<string> listaSelectoresNombre = new List<string>();
						XmlNode selector = seleccionEntidad.SelectSingleNode("UrlTipoEntSolicitada");
						if (selector == null)
						{
							byte[] ontologiArrray = ControladorDocumentacion.ObtenerOntologia(pOntologiaID, ProyectoSeleccionado.Clave);
							Ontologia ontologia = new Ontologia(ontologiArrray, true);

							ontologia.LeerOntologia();
							List<ElementoOntologia> entidadesPrincipal = GestionOWL.ObtenerElementosContenedorSuperior(ontologia.Entidades);
							ElementoOntologia entidadPrincipal = null;
							if (entidadesPrincipal != null && entidadesPrincipal.Count > 0)
							{
								entidadPrincipal = entidadesPrincipal.First();
								string tipoEntidad = entidadPrincipal.TipoEntidadRelativo;
								if (!listaSelectoresNombre.Contains(tipoEntidad))
								{
									listaSelectoresNombre.Add(tipoEntidad);
								}
							}
						}
						else if (selector != null)
						{
							string[] sel = selector.InnerText.Split(',');
							foreach (string selNombre in sel)
							{
								string nombre = "";
								if (selNombre != null && selNombre.Contains("#"))
								{
									nombre = selNombre.Substring(selNombre.LastIndexOf("#") + 1);
								}
								else if (selNombre != null && selNombre.Contains("/"))
								{
									nombre = selNombre.Substring(selNombre.LastIndexOf("/") + 1);
								}
								else
								{
									nombre = selNombre;
								}

								if (!listaSelectoresNombre.Contains(nombre))
								{
									listaSelectoresNombre.Add(nombre);
								}
							}
						}

						if (dicSelectores.ContainsKey(nombreGrafo))
						{
							vSelectores = dicSelectores[nombreGrafo];
							vSelectores = vSelectores.Union(listaSelectoresNombre).ToList();
							dicSelectores[nombreGrafo] = vSelectores;
						}
						else
						{
							dicSelectores.Add(nombreGrafo, listaSelectoresNombre);
						}
					}
				}
			}
			return dicSelectores;
		}

		/// <summary>
		/// Espef propiedad definida como etiqueta en el archivo de configuración de la ontología
		/// </summary>
		private XmlNode EspefPropiedad(XmlDocument pXmlDocument)
		{
			XmlNodeList listaEspefPropiedad = null;
			if (pXmlDocument.DocumentElement != null)
			{
				listaEspefPropiedad = pXmlDocument.DocumentElement.SelectNodes("EspefPropiedad");
				if (listaEspefPropiedad != null)
				{
					if (listaEspefPropiedad.Count > 1)
					{
						foreach (XmlNode espef in listaEspefPropiedad)
						{

							if (espef.Attributes.Count > 0)
							{
								if (espef.Attributes[0].InnerText.Equals(ProyectoSeleccionado.Clave))
								{
									return espef;
								}
								else if (espef.Attributes[0].InnerText.Equals(ProyectoSeleccionado.NombreCorto))
								{
									return espef;
								}
							}
						}
						foreach (XmlNode espef in listaEspefPropiedad)
						{
							if (espef.Attributes.Count < 1)
							{
								return espef;
							}
						}
					}
				}
			}
			if (listaEspefPropiedad != null)
			{
				return listaEspefPropiedad.Item(0);
			}
			else { return null; }
		}

		/// <summary>
		/// Espef entidad definida como etiqueta en el archivo de configuración de la ontología
		/// </summary>
		private XmlNode EspefEntidad(XmlDocument pXmlDocument)
		{
			XmlNodeList listaespef = null;
			if (pXmlDocument.DocumentElement != null)
			{
				listaespef = pXmlDocument.DocumentElement.SelectNodes("EspefEntidad");
				if (listaespef != null)
				{
					if (listaespef.Count > 1)
					{
						foreach (XmlNode espef in listaespef)
						{
							if (espef.Attributes.Count > 0)
							{
								if (espef.Attributes[0].InnerText.Equals(ProyectoSeleccionado.Clave))
								{
									return espef;
								}
								else if (espef.Attributes[0].InnerText.Equals(ProyectoSeleccionado.NombreCorto))
								{
									return espef;
								}
							}
						}
						foreach (XmlNode espef in listaespef)
						{
							if (espef.Attributes.Count < 1)
							{
								return espef;
							}
						}
					}
				}
			}
			if (listaespef != null)
			{
				return listaespef.Item(0);
			}
			else { return null; }
		}


		/// <summary>
		/// Guarda el documento en la base de datos.
		/// </summary>
		protected void GuardarCambios(Guid pOntologiaID)
		{
			mEntityContext.SaveChanges();

			DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
			docCL.InvalidarOntologiasProyecto(ProyectoSeleccionado.Clave);

			ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
			proyCL.InvalidarOntologiasEcosistema();

			bool iniciado = false;
			try
			{
				iniciado = HayIntegracionContinua;
			}
			catch (Exception ex)
			{
				GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
				throw;
			}

			//if (iniciado)
			//{
			//    HttpResponseMessage resultado = AdministrarIntegracionContinua(mRDF.Ontology, pOntologiaID);
			//    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
			//    {
			//        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
			//    }
			//}

			docCL.GuardarOntologia(pOntologiaID, null);
			docCL.GuardarIDXmlOntologia(pOntologiaID, Guid.NewGuid());
			docCL.Dispose();

			mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
		}

	}
}
