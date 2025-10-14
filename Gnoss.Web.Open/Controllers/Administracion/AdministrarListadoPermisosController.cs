using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.MVC;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Gnoss.Web.Open.Controllers.Administracion
{
	public partial class ListadoPermisosViewModel
	{
		public List<PermisoViewModel> ListaPermisos { get; set; }
		public List<string> ListaRoles { get; set; }
	}

	public partial class PermisoViewModel
	{
		public string Nombre { get; set; }
		public string Seccion { get; set; }
		public string Descripcion { get; set; }
		public Dictionary<Guid, string> RolesAsignados { get; set; }
	}

	public class AdministrarListadoPermisosController : ControllerAdministrationWeb
	{
		private ListadoPermisosViewModel mPaginaModel;
		private List<RolEcosistema> mRolesEcosistema;
		private List<Rol> mRoles;

		public AdministrarListadoPermisosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarListadoPermisosController> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
		{
		}

		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
		public ActionResult Index()
		{
			ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";
			ViewBag.BodyClassPestanya = "admin-permisos listado no-max-width-container";
			if (EsAdministracionEcosistema)
			{
				ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
				ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_PermisosEcosistema;
			}
			else
			{
				ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
				ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_Permisos;
			}

			ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
			if (ParametrosGeneralesRow.IdiomasDisponibles)
			{
				ViewBag.MultiIdioma = true;
			}
			else
			{
				ViewBag.MultiIdioma = false;
			}

			EliminarPersonalizacionVistas();
			CargarPermisosAdministracionComunidadEnViewBag();

			return View(PaginaModel);
		}

		public Dictionary<Guid, string> CargarRolesAsignadosDePermiso(ulong pPermiso, TipoDePermiso pTipoPermiso)
		{
			Dictionary<Guid, string> rolesAsignados = new Dictionary<Guid, string>();

			try
			{
				ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";
				
				ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
				string tienePermiso = UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "NO");
				if (EsAdministracionEcosistema)
				{
					if (mRolesEcosistema == null)
					{
						mRolesEcosistema = proyectoCN.ObtenerRolesAdministracionEcosistema().OrderBy(x => x.Nombre).ToList();
					}

					foreach (RolEcosistema rol in mRolesEcosistema)
					{
						tienePermiso = UtilPermisos.TienePermiso(rol.Permisos, pPermiso) ? UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "SI") : UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "NO");
						rolesAsignados.Add(rol.RolID, tienePermiso);
					}
				}
				else
				{
					if (mRoles == null)
					{
						mRoles = proyectoCN.ObtenerRolesDeProyecto(ProyectoSeleccionado.Clave).OrderBy(x => x.Nombre).ToList();
					}
					
					foreach (Rol rol in mRoles)
					{
						switch (pTipoPermiso)
						{
							case TipoDePermiso.Comunidad:
								tienePermiso = UtilPermisos.TienePermiso(rol.PermisosAdministracion, pPermiso) ? UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "SI") : UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "NO");
								break;
							case TipoDePermiso.Contenidos:
								tienePermiso = UtilPermisos.TienePermiso(rol.PermisosContenidos, pPermiso) ? UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "SI") : UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "NO");
								break;
							case TipoDePermiso.Recursos:
								tienePermiso = UtilPermisos.TienePermiso(rol.PermisosRecursos, pPermiso) ? UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "SI") : UtilIdiomas.ObtenerTexto("ROLESYPERMISOS", "NO");
								break;
						}

						rolesAsignados.Add(rol.RolID, tienePermiso);
					}
				}				
			}
			catch (Exception ex)
			{
				mLoggingService.GuardarLogError(ex, mlogger);
			}

			return rolesAsignados;
		}

		private List<PermisoViewModel> ObtenerPermisos()
		{
			List<PermisoViewModel> listaPermisos = new List<PermisoViewModel>();

			if (EsAdministracionEcosistema)
			{
				foreach (PermisoEcosistema permisoEcosistema in Enum.GetValues(typeof(PermisoEcosistema)))
				{
					PermisoViewModel permiso = new PermisoViewModel();
					permiso.Nombre = UtilIdiomas.GetText("ROLESYPERMISOS", permisoEcosistema.ToString().ToUpper());
					permiso.Seccion = "ECOSISTEMA";
					permiso.Descripcion = UtilIdiomas.GetText("ROLESYPERMISOS", UtilCadenas.ObtenerAtributoEnum<DescriptionAttribute>(permisoEcosistema).Description.ToUpper());
					permiso.RolesAsignados = CargarRolesAsignadosDePermiso((ulong)permisoEcosistema, TipoDePermiso.Ecosistema);

					listaPermisos.Add(permiso);
				}
			}
			else
			{
				foreach (PermisoComunidad permisoComunidad in Enum.GetValues(typeof(PermisoComunidad)))
				{
					PermisoViewModel permiso = new PermisoViewModel();
					permiso.Nombre = UtilIdiomas.GetText("ROLESYPERMISOS", permisoComunidad.ToString().ToUpper());
					permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoComunidad).Nombre;
					permiso.Descripcion = UtilIdiomas.GetText("ROLESYPERMISOS", UtilCadenas.ObtenerAtributoEnum<DescriptionAttribute>(permisoComunidad).Description.ToUpper());
					permiso.RolesAsignados = CargarRolesAsignadosDePermiso((ulong)permisoComunidad, TipoDePermiso.Comunidad);

					listaPermisos.Add(permiso);
				}
				foreach (PermisoContenidos permisoContenidos in Enum.GetValues(typeof(PermisoContenidos)))
				{
					PermisoViewModel permiso = new PermisoViewModel();
					permiso.Nombre = UtilIdiomas.GetText("ROLESYPERMISOS", permisoContenidos.ToString().ToUpper());
					permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoContenidos).Nombre;
					permiso.Descripcion = UtilIdiomas.GetText("ROLESYPERMISOS", UtilCadenas.ObtenerAtributoEnum<DescriptionAttribute>(permisoContenidos).Description.ToUpper());
					permiso.RolesAsignados = CargarRolesAsignadosDePermiso((ulong)permisoContenidos, TipoDePermiso.Contenidos);

					listaPermisos.Add(permiso);
				}
				foreach (PermisoRecursos permisoRecurso in Enum.GetValues(typeof(PermisoRecursos)))
				{
					if (!permisoRecurso.Equals(PermisoRecursos.CrearRecursoSemantico) && !permisoRecurso.Equals(PermisoRecursos.EditarRecursoSemantico) && !permisoRecurso.Equals(PermisoRecursos.EliminarRecursoSemantico))
					{
						PermisoViewModel permiso = new PermisoViewModel();
						permiso.Nombre = UtilIdiomas.GetText("ROLESYPERMISOS", permisoRecurso.ToString().ToUpper());
						permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoRecurso).Nombre;
						permiso.Descripcion = UtilIdiomas.GetText("ROLESYPERMISOS", UtilCadenas.ObtenerAtributoEnum<DescriptionAttribute>(permisoRecurso).Description.ToUpper());
						permiso.RolesAsignados = CargarRolesAsignadosDePermiso((ulong)permisoRecurso, TipoDePermiso.Recursos);

						listaPermisos.Add(permiso);
					}					
				}				
			}				

			return listaPermisos;
		}

		private ListadoPermisosViewModel PaginaModel
		{
			get
			{
				if (mPaginaModel == null)
				{
					try
					{
						mPaginaModel = new ListadoPermisosViewModel();
						mPaginaModel.ListaPermisos = ObtenerPermisos();
						ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
						if (EsAdministracionEcosistema)
						{
							mPaginaModel.ListaRoles = proyectoCN.ObtenerRolesAdministracionEcosistema().Select(x => UtilCadenas.ObtenerTextoDeIdioma(x.Nombre, UtilIdiomas.LanguageCode, IdiomaPorDefecto)).ToList();
						}
						else
						{
							mPaginaModel.ListaRoles = proyectoCN.ObtenerRolesDeProyecto(ProyectoSeleccionado.Clave).Select(x => UtilCadenas.ObtenerTextoDeIdioma(x.Nombre, UtilIdiomas.LanguageCode, IdiomaPorDefecto)).ToList();
						}
						 
					}
					catch (Exception ex)
					{
						mLoggingService.GuardarLogError(ex, mlogger);
					}					
				}

				return mPaginaModel;
			}
		}
	}
}
