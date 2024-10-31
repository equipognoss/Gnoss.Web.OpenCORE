using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.Solicitudes;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{

	/// <summary>
	/// Controlador para administrar las opciones avanzadas de la comunidad por el metaadministrador de la plataforma
	/// </summary>
	public class AdministrarOpcionesMetaAdministradorController : ControllerBaseWeb
	{
		public AdministrarOpcionesMetaAdministradorController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
			: base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
		{
			_appLifetime = appLifetime;
		}

		#region Miembros

		private AdministrarOpcionesMetaadministradorViewModel mPaginaModel = null;
		//private ParametroGeneralDS mParametrosGeneralesDS;
		private GestorParametroGeneral mParametrosGeneralesDS;
		private List<ParametroAplicacion> mParametroAplicacion;
		private ParametroGeneral mFilaParametrosGenerales = null;
		private List<CMSComponentePrivadoProyecto> mFilaPermisosCMS = null;

		private bool mProyectoSinNombreCortoEnUrlActivado = false;
		private bool mProyectoSinUrlPropiaActivada = false;
		private IHostApplicationLifetime _appLifetime;

		#endregion

		#region Metodos de eventos

		/// <summary>
		/// Index
		/// </summary>
		/// <returns>ActionResult</returns>
		[HttpGet]
		[TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
		public ActionResult Index()
		{
			// Añadir clases para el body del Layout
			ViewBag.BodyClassPestanya = "meta-administrador";
			ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
			ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_OpcionesMetaAdministrador;
			// Establecer el título para el header de DevTools
			ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
			ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "OPCIONESDELMETAADMINISTRADOR");



			EliminarPersonalizacionVistas();
			CargarPermisosAdministracionComunidadEnViewBag();

			return View(PaginaModel);
		}

		/// <summary>
		/// Guardar
		/// </summary>
		/// <returns>ActionResult</returns>
		[HttpPost]
		[TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
		public ActionResult Guardar(AdministrarOpcionesMetaadministradorViewModel Options)
		{
			GuardarLogAuditoria();
			try
			{
				bool eliminarTerceraPeticionFacetas = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "TerceraPeticionFacetasPlegadas") != Options.TerceraPeticionFacetasPlegadas;

				bool esNecesarioReiniciar = GuardarOpciones(Options);

				InvalidarCaches(eliminarTerceraPeticionFacetas);

				if (esNecesarioReiniciar)
				{
					return GnossResultOK("shutdown");
				}
				else
				{
					return GnossResultOK();
				}
			}
			catch (ExcepcionGeneral ex)
			{
				return GnossResultERROR(ex.Message);
			}
			catch (Exception ex)
			{
				GuardarLogError(ex);
				return GnossResultERROR(ex.Message);
			}

		}

		[HttpPost]
		[TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
		public void Shutdown()
		{
			ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
			Guid clave = Guid.NewGuid();
			proyectoCL.AgregarObjetoCache(NombresCL.CLAVEREINICIO, clave, 3600);
			_appLifetime.StopApplication();
		}

		#endregion

		#region Metodos

		private bool GuardarOpciones(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			bool esNecesarioReiniciar;

			DataWrapperVistaVirtual vistaVirtualDW = PasarDatosDeModeloADataSet(pOptions, out esNecesarioReiniciar);

			ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

			mEntityContext.NoConfirmarTransacciones = true;
			try
			{
				paramCN.ActualizarParametrosGenerales();
				paramCN.Dispose();

				if (vistaVirtualDW != null)
				{
					vistaVirtualCN.ActualizarVistaVirtual(vistaVirtualDW);
				}

				mEntityContext.TerminarTransaccionesPendientes(true);
			}
			catch
			{
				mEntityContext.TerminarTransaccionesPendientes(false);
				throw;
			}

			ParametroAplicacionCN ParametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			ParametroAplicacionCN.ActualizarConfiguracionGnoss();
			ParametroAplicacionCN.Dispose();

			return esNecesarioReiniciar;
		}

		private DataWrapperVistaVirtual PasarDatosVistasADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			DataWrapperVistaVirtual vistaVirtualDW = null;

			if (pOptions.VistasActivadas)
			{
				if (pOptions.PersonalizacionIDVistas == Guid.Empty)
				{
					vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave);

					if (vistaVirtualDW.ListaVistaVirtualPersonalizacion.Count > 0)
					{
						pOptions.PersonalizacionIDVistas = vistaVirtualDW.ListaVistaVirtualPersonalizacion.First().PersonalizacionID;
					}
					else
					{
						pOptions.PersonalizacionIDVistas = Guid.NewGuid();
					}
				}
				else
				{
					vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorPersonalizacionID(pOptions.PersonalizacionIDVistas);
				}

				if (vistaVirtualDW.ListaVistaVirtualPersonalizacion.Where(item => item.PersonalizacionID.Equals(pOptions.PersonalizacionIDVistas)).ToList().FirstOrDefault() == null)
				{
					VistaVirtualPersonalizacion filaVistaVirtualPersonalizacion = new VistaVirtualPersonalizacion();
					filaVistaVirtualPersonalizacion.PersonalizacionID = pOptions.PersonalizacionIDVistas;
					vistaVirtualDW.ListaVistaVirtualPersonalizacion.Add(filaVistaVirtualPersonalizacion);
					mEntityContext.VistaVirtualPersonalizacion.Add(filaVistaVirtualPersonalizacion);
				}
				if (vistaVirtualDW.ListaVistaVirtualProyecto.Where(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.PersonalizacionID.Equals(pOptions.PersonalizacionIDVistas)).ToList().FirstOrDefault() == null)
				{
					VistaVirtualProyecto filaVistaVirtualProyecto = new VistaVirtualProyecto();
					filaVistaVirtualProyecto.PersonalizacionID = pOptions.PersonalizacionIDVistas;
					filaVistaVirtualProyecto.ProyectoID = ProyectoSeleccionado.Clave;
					filaVistaVirtualProyecto.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
					vistaVirtualDW.ListaVistaVirtualProyecto.Add(filaVistaVirtualProyecto);
					mEntityContext.VistaVirtualProyecto.Add(filaVistaVirtualProyecto);
				}
			}
			else
			{
				vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave);

				if (vistaVirtualDW.ListaVistaVirtualProyecto.Count > 0)
				{
					mEntityContext.EliminarElemento(vistaVirtualDW.ListaVistaVirtualProyecto.First());
					vistaVirtualDW.ListaVistaVirtualProyecto.Remove(vistaVirtualDW.ListaVistaVirtualProyecto.First());
				}
			}

			if (ObtenerParametroAplicacionPersonalizacionEcosistemaID() != "")
			{
				if (!mControladorBase.ComunidadExcluidaPersonalizacionEcosistema && pOptions.NoUsarVistasDelEcosistema)
				{
					AgregarExcepcionProyectoPersonalizacionEcosistemaParametroAplicacion();
				}
				else if (mControladorBase.ComunidadExcluidaPersonalizacionEcosistema && !pOptions.NoUsarVistasDelEcosistema)
				{
					EliminarExcepcionProyectoPersonalizacionEcosistemaParametroAplicacion();
				}
			}

			return vistaVirtualDW;
		}

		private void PasarDatosCapturaImgADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			if (pOptions.CapturasImgSizeAlto > 0 || pOptions.CapturasImgSizeAncho > 0)
			{
				// Si alguno de los dos es 0, le porngo el valor del otro
				if (pOptions.CapturasImgSizeAlto == 0)
				{
					pOptions.CapturasImgSizeAlto = pOptions.CapturasImgSizeAncho;
				}
				else if (pOptions.CapturasImgSizeAncho == 0)
				{
					pOptions.CapturasImgSizeAncho = pOptions.CapturasImgSizeAlto;
				}

				ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "CaputurasImgSize", $"{pOptions.CapturasImgSizeAncho},{pOptions.CapturasImgSizeAlto}");
			}
			else
			{
				ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "CaputurasImgSize", null);
			}
		}

		private void PasarDatosProyectoSinRegistroObligatorio(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			Dictionary<string, bool> lista = new Dictionary<string, bool>();
			if (!string.IsNullOrEmpty(pOptions.ProyectosView))
			{
				string[] substrings = pOptions.ProyectosView.Split(',');

				foreach (string cadena in substrings)
				{
					string[] proyecto = cadena.Split(':');
					bool booleano = false;
					if (proyecto[1].Equals("true"))
					{
						booleano = true;
					}
					lista.Add(proyecto[0], booleano);
				}

				ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
				foreach (string proyecto in lista.Keys)
				{
					Guid proyID = proyCL.ObtenerProyectoIDPorNombreCorto(proyecto);
					EntityContext context = mEntityContext;
					//var fila = ParamAplicacionDS.ProyectoSinRegistroObligatorio.FindByOrganizacionIDProyectoIDOrganizacionSinRegistroIDProyectoSinRegistroID(ProyectoAD.MetaOrganizacion, ProyectoSeleccionado.Clave, ProyectoAD.MetaOrganizacion, proyID);
					ProyectoSinRegistroObligatorio fila = context.ProyectoSinRegistroObligatorio.Where(proyectoSinRegistro => proyectoSinRegistro.ProyectoRegistroObligatorio.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && proyectoSinRegistro.ProyectoRegistroObligatorio.ProyectoID.Equals(ProyectoSeleccionado.Clave) && proyectoSinRegistro.OrganizacionSinRegistroID.Equals(ProyectoAD.MetaOrganizacion) && proyectoSinRegistro.ProyectoSinRegistroID.Equals(proyID)).FirstOrDefault();

					if (fila != null && !lista[proyecto])
					{
						context.EliminarElemento(fila);
						// fila.Delete();
					}
					else if (fila == null && lista[proyecto])
					{
						context.ProyectoSinRegistroObligatorio.Add(new ProyectoSinRegistroObligatorio(ProyectoAD.MetaOrganizacion, ProyectoSeleccionado.Clave, ProyectoAD.MetaOrganizacion, proyID));
						//ParamAplicacionDS.ProyectoSinRegistroObligatorio.AddProyectoSinRegistroObligatorioRow(ProyectoAD.MetaOrganizacion, ProyectoSeleccionado.Clave, ProyectoAD.MetaOrganizacion, proyID);
					}
				}
			}
		}

		private void PasarDatosVersionCssYJsADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			if (pOptions.VersionCSS > 0 || pOptions.VersionJS > 0)
			{
				if (pOptions.VersionCSS > 0)
				{
					FilaParametrosGenerales.VersionCSS = pOptions.VersionCSS;
				}
				else if (!(FilaParametrosGenerales.VersionCSS == null))
				{
					FilaParametrosGenerales.VersionCSS = null;
				}

				if (pOptions.VersionCSS > 0)
				{
					FilaParametrosGenerales.VersionJS = pOptions.VersionJS;
				}
				else if (!(FilaParametrosGenerales.VersionJS == null))
				{
					FilaParametrosGenerales.VersionJS = null;
				}

				if (!FilaParametrosGenerales.OcultarPersonalizacion)
				{
					FilaParametrosGenerales.OcultarPersonalizacion = true;
				}
			}
			else if (FilaParametrosGenerales.OcultarPersonalizacion)
			{
				FilaParametrosGenerales.OcultarPersonalizacion = false;
			}
		}
		private void PasarDatosConsultaSparqlADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			Guid proyectoID = ProyectoVirtual.Clave;
			Guid organizacionID = ProyectoVirtual.FilaProyecto.OrganizacionID;
			bool existe22 = mEntityContext.CMSComponentePrivadoProyecto.Any(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.BuscadorSPARQL));
			bool existe26 = mEntityContext.CMSComponentePrivadoProyecto.Any(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.ConsultaSPARQL));

			if (pOptions.ConsultaSparql == true)
			{
				if (!existe22)
				{
					CMSComponentePrivadoProyecto cmscomponentePrivadoProyecto2 = new CMSComponentePrivadoProyecto();
					cmscomponentePrivadoProyecto2.OrganizacionID = organizacionID;
					cmscomponentePrivadoProyecto2.ProyectoID = proyectoID;
					cmscomponentePrivadoProyecto2.TipoComponente = (short)TipoComponenteCMS.BuscadorSPARQL;

					mEntityContext.CMSComponentePrivadoProyecto.Add(cmscomponentePrivadoProyecto2);
				}
				if (!existe26)
				{
					CMSComponentePrivadoProyecto cmscomponentePrivadoProyecto = new CMSComponentePrivadoProyecto();
					cmscomponentePrivadoProyecto.OrganizacionID = organizacionID;
					cmscomponentePrivadoProyecto.ProyectoID = proyectoID;
					cmscomponentePrivadoProyecto.TipoComponente = (short)TipoComponenteCMS.ConsultaSPARQL;

					mEntityContext.CMSComponentePrivadoProyecto.Add(cmscomponentePrivadoProyecto);
				}
			}
			else
			{
				if (existe22)
				{
					CMSComponentePrivadoProyecto cmsComponentePrivadoProyecto = mEntityContext.CMSComponentePrivadoProyecto.Where(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.BuscadorSPARQL)).FirstOrDefault();
					mEntityContext.EliminarElemento(cmsComponentePrivadoProyecto);
				}
				if (existe26)
				{
					CMSComponentePrivadoProyecto cmsComponentePrivadoProyecto = mEntityContext.CMSComponentePrivadoProyecto.Where(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.ConsultaSPARQL)).FirstOrDefault();
					mEntityContext.EliminarElemento(cmsComponentePrivadoProyecto);
				}
			}
		}
		private void PasarDatosPreguntaTICADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			Guid proyectoID = ProyectoVirtual.Clave;
			Guid organizacionID = ProyectoVirtual.FilaProyecto.OrganizacionID;
			bool existe19 = mEntityContext.CMSComponentePrivadoProyecto.Any(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.PreguntaTIC));
			if (pOptions.PreguntaTIC == true)
			{
				if (!existe19)
				{
					CMSComponentePrivadoProyecto cmscomponentePrivadoProyecto = new CMSComponentePrivadoProyecto();
					cmscomponentePrivadoProyecto.OrganizacionID = organizacionID;
					cmscomponentePrivadoProyecto.ProyectoID = proyectoID;
					cmscomponentePrivadoProyecto.TipoComponente = (short)TipoComponenteCMS.PreguntaTIC;

					mEntityContext.CMSComponentePrivadoProyecto.Add(cmscomponentePrivadoProyecto);
				}
			}
			else
			{
				if (existe19)
				{
					CMSComponentePrivadoProyecto cmsComponentePrivadoProyecto = mEntityContext.CMSComponentePrivadoProyecto.Where(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.PreguntaTIC)).FirstOrDefault();
					mEntityContext.EliminarElemento(cmsComponentePrivadoProyecto);
				}
			}

		}
		private void PasarDatosBuscadorSPARQLADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			Guid proyectoID = ProyectoVirtual.Clave;
			Guid organizacionID = ProyectoVirtual.FilaProyecto.OrganizacionID;
			bool existe22 = mEntityContext.CMSComponentePrivadoProyecto.Any(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.BuscadorSPARQL));
			if (pOptions.BuscadorSPARQL == true && pOptions.ConsultaSparql == false)
			{
				if (!existe22)
				{
					CMSComponentePrivadoProyecto cmscomponentePrivadoProyecto = new CMSComponentePrivadoProyecto();
					cmscomponentePrivadoProyecto.OrganizacionID = organizacionID;
					cmscomponentePrivadoProyecto.ProyectoID = proyectoID;
					cmscomponentePrivadoProyecto.TipoComponente = (short)TipoComponenteCMS.BuscadorSPARQL;

					mEntityContext.CMSComponentePrivadoProyecto.Add(cmscomponentePrivadoProyecto);
				}
			}
			else if (pOptions.BuscadorSPARQL == false && pOptions.ConsultaSparql == false)
			{
				if (existe22)
				{
					CMSComponentePrivadoProyecto cmsComponentePrivadoProyecto = mEntityContext.CMSComponentePrivadoProyecto.Where(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.BuscadorSPARQL)).FirstOrDefault();
					mEntityContext.EliminarElemento(cmsComponentePrivadoProyecto);
				}
			}
		}
		private void PasarDatosFichaDescripcionDocumentoADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			Guid proyectoID = ProyectoVirtual.Clave;
			Guid organizacionID = ProyectoVirtual.FilaProyecto.OrganizacionID;
			bool existe24 = mEntityContext.CMSComponentePrivadoProyecto.Any(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.FichaDescripcionDocumento));
			if (pOptions.FichaDescripcionDocumento == true)
			{
				if (!existe24)
				{
					CMSComponentePrivadoProyecto cmscomponentePrivadoProyecto = new CMSComponentePrivadoProyecto();
					cmscomponentePrivadoProyecto.OrganizacionID = organizacionID;
					cmscomponentePrivadoProyecto.ProyectoID = proyectoID;
					cmscomponentePrivadoProyecto.TipoComponente = (short)TipoComponenteCMS.FichaDescripcionDocumento;

					mEntityContext.CMSComponentePrivadoProyecto.Add(cmscomponentePrivadoProyecto);
				}
			}
			else
			{
				if (existe24)
				{
					CMSComponentePrivadoProyecto cmsComponentePrivadoProyecto = mEntityContext.CMSComponentePrivadoProyecto.Where(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.FichaDescripcionDocumento)).FirstOrDefault();
					mEntityContext.EliminarElemento(cmsComponentePrivadoProyecto);
				}
			}
		}
		private void PasarDatosConsultaSQLServerADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions)
		{
			Guid proyectoID = ProyectoVirtual.Clave;
			Guid organizacionID = ProyectoVirtual.FilaProyecto.OrganizacionID;
			bool existe27 = mEntityContext.CMSComponentePrivadoProyecto.Any(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.ConsultaSQLSERVER));
			if (pOptions.ConsultaSQLServer == true)
			{
				if (!existe27)
				{
					CMSComponentePrivadoProyecto cmscomponentePrivadoProyecto = new CMSComponentePrivadoProyecto();
					cmscomponentePrivadoProyecto.OrganizacionID = organizacionID;
					cmscomponentePrivadoProyecto.ProyectoID = proyectoID;
					cmscomponentePrivadoProyecto.TipoComponente = (short)TipoComponenteCMS.ConsultaSQLSERVER;

					mEntityContext.CMSComponentePrivadoProyecto.Add(cmscomponentePrivadoProyecto);
				}
			}
			else
			{
				if (existe27)
				{
					CMSComponentePrivadoProyecto cmsComponentePrivadoProyecto = mEntityContext.CMSComponentePrivadoProyecto.Where(componentePrivada => componentePrivada.ProyectoID.Equals(proyectoID) && componentePrivada.TipoComponente.Equals((short)TipoComponenteCMS.ConsultaSQLSERVER)).FirstOrDefault();
					mEntityContext.EliminarElemento(cmsComponentePrivadoProyecto);
				}
			}
		}

		private DataWrapperVistaVirtual PasarDatosDeModeloADataSet(AdministrarOpcionesMetaadministradorViewModel pOptions, out bool pReiniciarAplicacion)
		{
			ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			//Compruebo si puedo activar la url corta en este proyecto
			mProyectoSinNombreCortoEnUrlActivado = !ParametrosGeneralesDS.ListaParametroProyecto.Exists(parametro => parametro.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametro.ProyectoID.Equals(ProyectoSeleccionado.Clave) && parametro.Parametro.Equals("ProyectoSinNombreCortoEnURL"));

			pReiniciarAplicacion = false;

			if (pOptions.ProyectoSinNombreCortoEnURL)
			{
				//¿Puedo habilitar la url corta para este proyecto?
				if (mProyectoSinNombreCortoEnUrlActivado)
				{
					string proyectoConfiguradoDominio = mEntityContext.ParametroAplicacion.Where(item => item.Parametro.Equals(ProyectoSeleccionado.UrlPropia(IdiomaUsuario))).Select(item => item.Valor).FirstOrDefault();

					//¿Hay algun proyecto configurado previamente?
					if (string.IsNullOrEmpty(proyectoConfiguradoDominio))
					{
						paramCN.ActualizarParametroAplicacion(ProyectoSeleccionado.UrlPropia(IdiomaUsuario), ProyectoSeleccionado.Clave.ToString());

						// ¿Se quiere reiniciar ahora la aplicacion?
						if (pOptions.ReiniciarAplicacion)
						{
							pReiniciarAplicacion = true;
						}
					}
					else if (!proyectoConfiguradoDominio.Equals(ProyectoSeleccionado.Clave.ToString()))
					{
						throw new Exception($"El dominio {ProyectoSeleccionado.UrlPropia(IdiomaUsuario)} ya esta asociada al proyecto {proyectoConfiguradoDominio}. Solo puede haber un proyecto asociado a un dominio con la url acortada.");
					}
				}

				ConfigurarRegistroAutomatico(pOptions.RegistroAutomaticoCheckbox);

			}
			else
			{
				// ¿Esta configurada la url corta?
				if (mEntityContext.ParametroAplicacion.Any(item => item.Parametro.Equals(ProyectoSeleccionado.UrlPropia(IdiomaUsuario)) && item.Valor.Equals(ProyectoSeleccionado.Clave.ToString())))
				{
					paramCN.ActualizarParametroAplicacion(ProyectoSeleccionado.UrlPropia(IdiomaUsuario), string.Empty);

					// ¿Se quiere reiniciar ahora la aplicacion?
					if (pOptions.ReiniciarAplicacion)
					{
						pReiniciarAplicacion = true;
					}

					ConfigurarRegistroAutomatico(pOptions.RegistroAutomaticoCheckbox);
				}
			}

			// Parámetros booleanos
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "AdministracionSemanticaPermitido", pOptions.AdministracionSemanticaPermitido);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "AdministracionPaginasPermitido", pOptions.AdministracionPaginasPermitido);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "AdministracionVistasPermitido", pOptions.AdministracionVistasPermitido);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "AdministracionDesarrolladoresPermitido", pOptions.AdministracionDesarrolladoresPermitido);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "PintarEnlacesLODEtiquetasEnProyecto", pOptions.EtiquetasConLOD, true);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "RecibirNewsletterDefecto", pOptions.RecibirNewsletterDefecto, true);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "GoogleDrive", pOptions.GoogleDrive);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple", pOptions.OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "TerceraPeticionFacetasPlegadas", pOptions.TerceraPeticionFacetasPlegadas);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "TieneGrafoDbPedia", pOptions.TieneGrafoDbPedia);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "ProyectoSinNombreCortoEnURL", pOptions.ProyectoSinNombreCortoEnURL);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "RegistroAutomaticoCheckbox", pOptions.RegistroAutomaticoCheckbox);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "PermitirDescargaIdentidadInvitada", pOptions.PermitirDescargaIdentidadInvitada);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "CargarEditoresLectoresEnBusqueda", pOptions.CargarEditoresLectoresEnBusqueda);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "TextoInvariableTesauroSemantico", pOptions.TextoInvariableTesauroSemantico);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "RegistroAbiertoEnComunidad", pOptions.RegistroAbiertoEnComunidad);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "PermitirMayusculas", pOptions.PermitirMayusculas);
			ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "PropiedadCMSMultiIdioma", pOptions.PropiedadCMSMultiIdioma);
			if (mProyectoSinNombreCortoEnUrlActivado)
			{
				Gnoss.Logica.ServiciosGenerales.ProyectoCN proyCN = new Gnoss.Logica.ServiciosGenerales.ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
				List<string> listaUrls = proyCN.ObtenerUrlsComunidadCajaBusqueda(ProyectoSeleccionado.Clave);

				foreach (string url in listaUrls)
				{
					if (url.Contains(UtilIdiomas.GetText("URLSEM", "COMUNIDAD")))
					{
						proyCN.QuitarUrlComunidadCajaBusqueda(ProyectoSeleccionado.Clave, url);
					}
				}
			}

			bool resultado = pOptions.Replicacion;
			string resultadoNumerico = "0";
			if (resultado)
			{
				resultadoNumerico = "1";
			}


			paramCN.ActualizarParametroAplicacion("Replicacion", resultadoNumerico);
			paramCN.Actualizar();

			// Estos parámetros son booleanos pero se guardan con true y false
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "EnviarNotificacionesDeSuscripciones", pOptions.EnviarNotificacionesDeSuscripciones ? "true" : null);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "SuscribirATodaComunidad", pOptions.SuscribirATodaComunidad ? "true" : null);

			// Parametros numéricos
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "DiaEnvioSuscripcion", (pOptions.DiaEnvioSuscripcion > 0 && pOptions.DiaEnvioSuscripcion < 8) ? pOptions.DiaEnvioSuscripcion.ToString() : null);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "SegundosDormirNewsletterPorCorreo", pOptions.SegundosDormirNewsletterPorCorreo > 0 ? pOptions.SegundosDormirNewsletterPorCorreo.ToString() : null);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "NumeroFacetasPrimeraPeticion", pOptions.NumeroFacetasPrimeraPeticion > 0 ? pOptions.NumeroFacetasPrimeraPeticion.ToString() : null);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "NumeroFacetasSegundaPeticion", pOptions.NumeroFacetasSegundaPeticion > 0 ? pOptions.NumeroFacetasSegundaPeticion.ToString() : null);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "FilasPorPagina", pOptions.FilasPorPagina > 0 ? pOptions.FilasPorPagina.ToString() : null);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "ServiceBusSegundos", pOptions.ServiceBusSegundos > 0 ? pOptions.ServiceBusSegundos.ToString() : null);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "ServiceBusReintentos", pOptions.ServiceBusReintentos > 0 ? pOptions.ServiceBusReintentos.ToString() : null);

			// Parámetros string
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "loginFacebook", pOptions.LoginFacebook);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "loginGoogle", pOptions.LoginGoogle);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "loginTwitter", pOptions.LoginTwitter);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "FacetasCostosasTerceraPeticion", pOptions.FacetasCostosasTerceraPeticion);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "PropiedadContenidoMultiIdioma", pOptions.PropiedadContenidoMultiIdioma);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "PropiedadesConEnlacesDbpedia", pOptions.PropiedadesConEnlacesDbpedia);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "ExcepcionBusquedaMovil", pOptions.ExcepcionBusquedaMovil);
			ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "RutaEstilos", pOptions.RutaEstilos);

			FilaParametrosGenerales.CMSDisponible = pOptions.CMSActivado;
			FilaParametrosGenerales.ComunidadGNOSS = !pOptions.CabeceraSimple;
			FilaParametrosGenerales.TieneSitemapComunidad = pOptions.SiteMapActivado;
			FilaParametrosGenerales.NumeroRecursosRelacionados = (short)pOptions.NumeroRecursosRelacionados;
			FilaParametrosGenerales.BiosCortas = pOptions.BiosCortas;
			FilaParametrosGenerales.RdfDisponibles = pOptions.RdfDisponibles;
			//FilaParametrosGenerales.CargasMasivasDisponibles = pOptions.CargasMasivasDisponibles;
			FilaParametrosGenerales.FechaNacimientoObligatoria = pOptions.FechaNacimientoObligatoria;
			FilaParametrosGenerales.PrivacidadObligatoria = pOptions.PrivacidadObligatoria;
			FilaParametrosGenerales.EventosDisponibles = pOptions.EventosDisponibles;
			FilaParametrosGenerales.AvisoCookie = pOptions.AvisoCookie;

			if (!ProyectoSeleccionado.EsPublico)
			{
				// Estos parámetros sólo se usan en comunidades privadas
				ControladorProyecto.GuardarParametroBooleano(ParametrosGeneralesDS, "RegistroAbierto", pOptions.RegistroAbierto);
				FilaParametrosGenerales.SolicitarCoockieLogin = pOptions.SolicitarCookieLogin;
			}

			FilaParametrosGenerales.InvitacionesPorContactoDisponibles = pOptions.InvitacionesPorContactoDisponibles;

			if (!string.IsNullOrEmpty(pOptions.Copyright))
			{
				FilaParametrosGenerales.Copyright = pOptions.Copyright;
			}
			else if (!(FilaParametrosGenerales.Copyright == null))
			{
				FilaParametrosGenerales.Copyright = null;
			}

			if (!string.IsNullOrEmpty(pOptions.EnlaceContactoPiePagina))
			{
				FilaParametrosGenerales.EnlaceContactoPiePagina = pOptions.EnlaceContactoPiePagina;
			}
			else if (!(FilaParametrosGenerales.EnlaceContactoPiePagina == null))
			{
				FilaParametrosGenerales.EnlaceContactoPiePagina = null;
			}

			if (!string.IsNullOrEmpty(pOptions.AlgoritmoPersonasRecomendadas))
			{
				FilaParametrosGenerales.AlgoritmoPersonasRecomendadas = pOptions.AlgoritmoPersonasRecomendadas;
			}
			else if (!(FilaParametrosGenerales.AlgoritmoPersonasRecomendadas == null))
			{
				FilaParametrosGenerales.AlgoritmoPersonasRecomendadas = null;
			}
			PasarDatosVersionCssYJsADataSet(pOptions);

			PasarDatosConsultaSparqlADataSet(pOptions);
			PasarDatosPreguntaTICADataSet(pOptions);
			PasarDatosBuscadorSPARQLADataSet(pOptions);
			PasarDatosFichaDescripcionDocumentoADataSet(pOptions);
			PasarDatosConsultaSQLServerADataSet(pOptions);

			PasarDatosCapturaImgADataSet(pOptions);
			PasarDatosProyectoSinRegistroObligatorio(pOptions);

			return PasarDatosVistasADataSet(pOptions);
		}

		private void InvalidarCaches(bool pEliminarTerceraPeticionFacetas)
		{
			ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			paramCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);

			VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			vistaVirtualCL.InvalidarVistasVirtuales(ProyectoSeleccionado.Clave);

			ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
			proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
			proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);
			proyCL.InvalidarParametrosProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID);
			proyCL.Dispose();

			ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
			parametroAplicacionCL.InvalidarCacheParametrosAplicacion();

			mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);

			if (pEliminarTerceraPeticionFacetas)
			{
				FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
				facetadoCL.InvalidarCacheQueContengaCadena($"facetado_{ProyectoSeleccionado.Clave.ToString()}_*_facetas_3_");
				facetadoCL.Dispose();
			}
		}

		private Dictionary<Guid, KeyValuePair<string, Guid>> CargarListaProyectosConVistas()
		{
			VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			Dictionary<Guid, KeyValuePair<string, Guid>> listaProyectosVistas = vistaVirtualCN.ObtenerProyectosConVistas();
			vistaVirtualCN.Dispose();

			return listaProyectosVistas;
		}

		private Dictionary<string, bool> CargarProyectosRegistroObligatorio(Guid id)
		{
			//Pasar a ParametroAplicacion estos metodos
			ParametroAplicacionCN parametroaplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			Dictionary<string, bool> listaFinal = new Dictionary<string, bool>();
			Dictionary<Guid, string> listaProyectos = parametroaplicacionCN.ObtenerProyectosRegistroObligatorio();
			List<Guid> listaSinRegistro = parametroaplicacionCN.ObtenerProyectosSinRegistroObligatorio(id);
			parametroaplicacionCN.Dispose();

			foreach (KeyValuePair<Guid, string> par in listaProyectos)
			{
				listaFinal.Add(par.Value, listaSinRegistro.Contains(par.Key));
			}

			return listaFinal;
		}

		private string ObtenerParametroAplicacionPersonalizacionEcosistemaID()
		{
			string valor = "";

			//ParametroAplicacionDS.ParametroAplicacionRow filaPersonalizacion = ParamAplicacionDS.ParametroAplicacion.FindByParametro("PersonalizacionEcosistemaID");
			AD.EntityModel.ParametroAplicacion filaPersonalizacion = ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("PersonalizacionEcosistemaID"));
			if (filaPersonalizacion != null && !string.IsNullOrEmpty(filaPersonalizacion.Valor))
			{
				valor = filaPersonalizacion.Valor;
			}
			return valor;
		}

		private void AgregarExcepcionProyectoPersonalizacionEcosistemaParametroAplicacion()
		{
			AD.EntityModel.ParametroAplicacion filaExcepcion = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("PersonalizacionEcosistemaExcepciones"));
			if (filaExcepcion != null)
			{
				if (!filaExcepcion.Valor.Contains(ProyectoSeleccionado.Clave.ToString()))
				{
					if (!string.IsNullOrEmpty(filaExcepcion.Valor))
					{
						filaExcepcion.Valor = filaExcepcion.Valor + ',';
					}
					filaExcepcion.Valor = filaExcepcion.Valor + ProyectoSeleccionado.Clave.ToString();
				}
			}
			else
			{
				filaExcepcion = new AD.EntityModel.ParametroAplicacion("PersonalizacionEcosistemaExcepciones", ProyectoSeleccionado.Clave.ToString());
				mEntityContext.ParametroAplicacion.Add(filaExcepcion);
				//context.SaveChanges();
			}

		}

		private void EliminarExcepcionProyectoPersonalizacionEcosistemaParametroAplicacion()
		{
			//ParametroAplicacionDS.ParametroAplicacionRow filaExcepcion = ParamAplicacionDS.ParametroAplicacion.FindByParametro("PersonalizacionEcosistemaExcepciones");
			AD.EntityModel.ParametroAplicacion filaExcepcion = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("PersonalizacionEcosistemaExcepciones"));
			if (filaExcepcion != null && !string.IsNullOrEmpty(filaExcepcion.Valor))
			{
				filaExcepcion.Valor = filaExcepcion.Valor.Replace(ProyectoSeleccionado.Clave.ToString(), "");
				filaExcepcion.Valor = filaExcepcion.Valor.Replace(",,", ",").Trim(',');
			}
		}

		private void CargarModelo()
		{
			// Parámetros booleanos
			mPaginaModel = new AdministrarOpcionesMetaadministradorViewModel();
			mPaginaModel.CMSActivado = FilaParametrosGenerales.CMSDisponible;
			mPaginaModel.TieneVistasEcosistema = ObtenerParametroAplicacionPersonalizacionEcosistemaID() != "";
			mPaginaModel.NoUsarVistasDelEcosistema = mControladorBase.ComunidadExcluidaPersonalizacionEcosistema;

			mPaginaModel.CabeceraSimple = !FilaParametrosGenerales.ComunidadGNOSS;
			mPaginaModel.SiteMapActivado = FilaParametrosGenerales.TieneSitemapComunidad;
			mPaginaModel.NumeroRecursosRelacionados = FilaParametrosGenerales.NumeroRecursosRelacionados;
			mPaginaModel.BiosCortas = FilaParametrosGenerales.BiosCortas;
			mPaginaModel.RdfDisponibles = FilaParametrosGenerales.RdfDisponibles;
			//mPaginaModel.CargasMasivasDisponibles = FilaParametrosGenerales.CargasMasivasDisponibles;
			mPaginaModel.FechaNacimientoObligatoria = FilaParametrosGenerales.FechaNacimientoObligatoria;
			mPaginaModel.PrivacidadObligatoria = FilaParametrosGenerales.PrivacidadObligatoria;
			mPaginaModel.EventosDisponibles = FilaParametrosGenerales.EventosDisponibles;
			mPaginaModel.SolicitarCookieLogin = FilaParametrosGenerales.SolicitarCoockieLogin;
			mPaginaModel.InvitacionesPorContactoDisponibles = FilaParametrosGenerales.InvitacionesPorContactoDisponibles;
			mPaginaModel.AvisoCookie = FilaParametrosGenerales.AvisoCookie;

			mPaginaModel.ConsultaSparql = FilaPermisosCMS.Any(componentePrivado => componentePrivado.TipoComponente.Equals((short)TipoComponenteCMS.ConsultaSPARQL));
			mPaginaModel.PreguntaTIC = FilaPermisosCMS.Any(componentePrivado => componentePrivado.TipoComponente.Equals((short)TipoComponenteCMS.PreguntaTIC));
			mPaginaModel.BuscadorSPARQL = FilaPermisosCMS.Any(componentePrivado => componentePrivado.TipoComponente.Equals((short)TipoComponenteCMS.BuscadorSPARQL));
			mPaginaModel.FichaDescripcionDocumento = FilaPermisosCMS.Any(componentePrivado => componentePrivado.TipoComponente.Equals((short)TipoComponenteCMS.FichaDescripcionDocumento));
			mPaginaModel.ConsultaSQLServer = FilaPermisosCMS.Any(componentePrivado => componentePrivado.TipoComponente.Equals((short)TipoComponenteCMS.ConsultaSQLSERVER));

			// if (!FilaParametrosGenerales.IsVersionCSSNull())
			if (!(FilaParametrosGenerales.VersionCSS == null))
			{
				// mPaginaModel.VersionCSS = FilaParametrosGenerales.VersionCSS;
				mPaginaModel.VersionCSS = (int)FilaParametrosGenerales.VersionCSS;
			}
			if (!(FilaParametrosGenerales.VersionJS == null))
			{
				mPaginaModel.VersionJS = (int)FilaParametrosGenerales.VersionJS;
			}
			if (!(FilaParametrosGenerales.Copyright == null))
			{
				mPaginaModel.Copyright = FilaParametrosGenerales.Copyright;
			}
			if (!(FilaParametrosGenerales.EnlaceContactoPiePagina == null))
			{
				mPaginaModel.EnlaceContactoPiePagina = FilaParametrosGenerales.EnlaceContactoPiePagina;
			}
			if (!(FilaParametrosGenerales.AlgoritmoPersonasRecomendadas == null))
			{
				mPaginaModel.AlgoritmoPersonasRecomendadas = FilaParametrosGenerales.AlgoritmoPersonasRecomendadas;
			}



			// Vistas
			mPaginaModel.ListaProyectosVistas = CargarListaProyectosConVistas();
			mPaginaModel.PersonalizacionIDVistas = Guid.Empty;
			mPaginaModel.VistasActivadas = false;
			if (mPaginaModel.ListaProyectosVistas.ContainsKey(ProyectoSeleccionado.Clave))
			{
				mPaginaModel.VistasActivadas = true;
				mPaginaModel.PersonalizacionIDVistas = mPaginaModel.ListaProyectosVistas[ProyectoSeleccionado.Clave].Value;
			}

			mPaginaModel.AdministracionSemanticaPermitido = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "AdministracionSemanticaPermitido");
			mPaginaModel.AdministracionPaginasPermitido = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "AdministracionPaginasPermitido");
			mPaginaModel.AdministracionVistasPermitido = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "AdministracionVistasPermitido");
			mPaginaModel.AdministracionDesarrolladoresPermitido = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "AdministracionDesarrolladoresPermitido");
			mPaginaModel.EtiquetasConLOD = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "PintarEnlacesLODEtiquetasEnProyecto", true);
			//mPaginaModel.AgruparEventosNuevosUsuarios = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "AgruparRegistrosUsuariosEnProyecto");
			mPaginaModel.RecibirNewsletterDefecto = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "RecibirNewsletterDefecto", true);
			mPaginaModel.EnviarNotificacionesDeSuscripciones = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "EnviarNotificacionesDeSuscripciones");
			mPaginaModel.SuscribirATodaComunidad = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "SuscribirATodaComunidad");
			mPaginaModel.GoogleDrive = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "GoogleDrive");
			mPaginaModel.OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple");
			mPaginaModel.TerceraPeticionFacetasPlegadas = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "TerceraPeticionFacetasPlegadas");
			mPaginaModel.TieneGrafoDbPedia = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "TieneGrafoDbPedia");
			mPaginaModel.PermitirDescargaIdentidadInvitada = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "PermitirDescargaIdentidadInvitada");
			mPaginaModel.ProyectoSinNombreCortoEnURL = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "ProyectoSinNombreCortoEnURL");
			mPaginaModel.RegistroAutomaticoCheckbox = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "RegistroAutomaticoCheckbox");
			mPaginaModel.RegistroAbierto = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "RegistroAbierto");
			mPaginaModel.CargarEditoresLectoresEnBusqueda = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "CargarEditoresLectoresEnBusqueda");
			mPaginaModel.TextoInvariableTesauroSemantico = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "TextoInvariableTesauroSemantico");

			mPaginaModel.OcultarCambiarPassword = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "OcultarCambiarPassword");
			mPaginaModel.DuplicarRecursosDisponible = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "DuplicarRecursosDisponible");
			mPaginaModel.CacheFacetas = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "CacheFacetas");
			string valor = string.Empty;
			foreach (ParametroAplicacion param in ParametroAplicacion)
			{
				if (param.Parametro.Equals("Replicacion"))
				{
					valor = param.Valor;
				}
			}
			bool esCierto = true;
			if (valor.Equals("1") || valor.Equals(""))
			{
				esCierto = true;
			}
			else if (valor.Equals("0"))
			{
				esCierto = false;
			}
			mPaginaModel.Replicacion = esCierto;
			mPaginaModel.RegistroAbiertoEnComunidad = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "RegistroAbiertoEnComunidad");
			mPaginaModel.PermitirMayusculas = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "PermitirMayusculas");
			mPaginaModel.PropiedadCMSMultiIdioma = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "PropiedadCMSMultiIdioma");

			//Parámetros numéricos
			mPaginaModel.DiaEnvioSuscripcion = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "DiaEnvioSuscripcion");
			mPaginaModel.SegundosDormirNewsletterPorCorreo = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "SegundosDormirNewsletterPorCorreo");
			mPaginaModel.NumeroFacetasPrimeraPeticion = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "NumeroFacetasPrimeraPeticion");
			mPaginaModel.NumeroFacetasSegundaPeticion = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "NumeroFacetasSegundaPeticion");

			mPaginaModel.TipoEnviarMensajeBienvenida = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "TipoEnviarMensajeBienvenida");
			mPaginaModel.CaducidadPassword = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "CaducidadPassword");
			mPaginaModel.PeriodicidadSuscripcion = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "PeriodicidadSuscripcion");
			mPaginaModel.FilasPorPagina = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "FilasPorPagina");
			mPaginaModel.ServiceBusSegundos = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "ServiceBusSegundos");
			mPaginaModel.ServiceBusReintentos = ControladorProyecto.ObtenerParametroInt(ParametroProyecto, "ServiceBusReintentos");


			// Parametros string
			mPaginaModel.LoginFacebook = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "loginFacebook");
			mPaginaModel.LoginGoogle = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "loginGoogle");
			mPaginaModel.LoginTwitter = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "loginTwitter");
			mPaginaModel.FacetasCostosasTerceraPeticion = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "FacetasCostosasTerceraPeticion");
			mPaginaModel.PropiedadContenidoMultiIdioma = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "PropiedadContenidoMultiIdioma");
			mPaginaModel.PropiedadesConEnlacesDbpedia = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "PropiedadesConEnlacesDbpedia");
			mPaginaModel.ExcepcionBusquedaMovil = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "ExcepcionBusquedaMovil");

			mPaginaModel.NombrePoliticaCookies = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "NombrePoliticaCookies");


			mPaginaModel.ProyectosRegistroObligatorio = CargarProyectosRegistroObligatorio(ProyectoSeleccionado.Clave);
			mPaginaModel.RutaEstilos = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "RutaEstilos");
			ObtenerParametroCapturasImgSize();

		}

		private void ObtenerParametroCapturasImgSize()
		{
			string captura = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "CaputurasImgSize");
			if (!string.IsNullOrEmpty(captura) && captura.Contains(','))
			{
				int capturaAncho = 0;
				int capturaAlto = 0;

				int.TryParse(captura.Substring(0, captura.IndexOf(',')), out capturaAncho);
				int.TryParse(captura.Substring(captura.IndexOf(',') + 1), out capturaAlto);

				mPaginaModel.CapturasImgSizeAncho = capturaAncho;
				mPaginaModel.CapturasImgSizeAlto = capturaAlto;
			}
		}

		/// <summary>
		/// Metodo que elimina o añade a base de datos el registro automatico
		/// </summary>
		/// <param name="registroAutomatico">True o false segun se quiera añadir el registro automático o no</param>
		private void ConfigurarRegistroAutomatico(bool registroAutomatico)
		{
			ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			Guid organizacionID;

			if(ProyectoSeleccionado.Organizacion != null)
			{
				organizacionID = ProyectoSeleccionado.Organizacion.Clave;
			}
			else
			{
				organizacionID = proyectoCN.ObtenerOrganizacionIDProyecto(ProyectoSeleccionado.Clave);
			}
			
			ParametroProyecto paramPro = paramCN.ObtenerParametroDeProyecto("RegistroAutomatico", ProyectoSeleccionado.Clave);

			if (registroAutomatico && paramPro == null)
			{
				// Si se ha marcado el registro automático y no existe ya en base de datos lo añade
				paramCN.ActualizarParametroEnProyecto(ProyectoSeleccionado.Clave, organizacionID, "RegistroAutomatico", ProyectoSeleccionado.UrlPropia(IdiomaUsuario));
			}
			else if (!registroAutomatico && paramPro != null)
			{
				//Si no se ha marcado el registro automático y existe en base de datos lo elimina
				paramCN.BorrarParametroDeProyecto(ProyectoSeleccionado.Clave, organizacionID, "RegistroAutomatico");
			}
		}

		#endregion

		#region Propiedades

		private AdministrarOpcionesMetaadministradorViewModel PaginaModel
		{
			get
			{
				if (mPaginaModel == null)
				{
					CargarModelo();
				}
				return mPaginaModel;
			}
		}

		private ParametroGeneral FilaParametrosGenerales
		{
			get
			{
				if (mFilaParametrosGenerales == null)
				{
					//mFilaParametrosGenerales = ParametrosGeneralesDS.ParametroGeneral.FindByOrganizacionIDProyectoID(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
					mFilaParametrosGenerales = ParametrosGeneralesDS.ListaParametroGeneral.Find(parametroGeneral => parametroGeneral.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametroGeneral.ProyectoID.Equals(ProyectoSeleccionado.Clave));
				}
				return mFilaParametrosGenerales;
			}
		}

		private List<CMSComponentePrivadoProyecto> FilaPermisosCMS
		{
			get
			{
				if (mFilaPermisosCMS == null)
				{
					mFilaPermisosCMS = new List<CMSComponentePrivadoProyecto>();
					ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
					mFilaPermisosCMS = gestorController.ObtenerCMSComponentePrivadoProyecto(mParametrosGeneralesDS, ProyectoVirtual.Clave);

				}
				return mFilaPermisosCMS;
			}
		}

		/// <summary>
		/// Obtiene el dataset de parámetros generales
		/// </summary>
		private GestorParametroGeneral ParametrosGeneralesDS
		{
			get
			{
				if (mParametrosGeneralesDS == null)
				{
					mParametrosGeneralesDS = new GestorParametroGeneral();
					ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
					mParametrosGeneralesDS = gestorController.ObtenerParametrosGeneralesDeProyecto(mParametrosGeneralesDS, ProyectoVirtual.Clave);
					mParametrosGeneralesDS.ListaParametroProyecto = gestorController.ObtenerParametroProyecto(ProyectoVirtual.Clave);
				}
				return mParametrosGeneralesDS;
			}
		}

		private List<AD.EntityModel.ParametroAplicacion> ParametroAplicacion
		{
			get
			{
				if (mParametroAplicacion == null)
				{
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
					mParametroAplicacion = paramCL.ObtenerParametrosAplicacionPorContext();
				}
				return mParametroAplicacion;
			}
		}

		/// <summary>
		/// Obtiene o establece si el usuario invitado puede ver esta página
		/// </summary>
		public override bool PaginaVisibleEnPrivada
		{
			get
			{
				return true;
			}
		}

		#endregion
	}
}
