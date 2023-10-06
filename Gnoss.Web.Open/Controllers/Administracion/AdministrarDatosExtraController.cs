using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.AdministrarTraducciones;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System.IO.Pipelines;

namespace Gnoss.Web.Open.Controllers.Administracion
{
    public class AdministrarDatosExtraController : ControllerBaseWeb
    {
        public AdministrarDatosExtraController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

		private AdministrarDatosExtraViewModel mPaginaModel = null;

        private DataWrapperProyecto mDatosExtraProyectoDataWrapperProyecto = null;

		public IActionResult Index()
        {
			EliminarPersonalizacionVistas();
			CargarPermisosAdministracionComunidadEnViewBag();

			// Añadir clase para el body del Layout
			ViewBag.BodyClassPestanya = "configuracion edicionTraducciones edicion no-max-width-container";
			ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
			ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_DatosExtra;
			// Establecer el título para el header de DevTools
			ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
			ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ADMINISTRARDATOSEXTRA");

			// Establecer en el ViewBag el idioma por defecto
			ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
			ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";
			ViewBag.UrlActionSaveExtraData = "";

			return View(PaginaModel);
        }

		public IActionResult Guardar(string pNombre, string pTipo, string pOpciones, bool pObligatorio, int pOrden, string pPredicadoRDF)
		{
			string error = ComprobarErrores(pNombre, pTipo, pOpciones, EsAdministracionEcosistema);

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

            if (string.IsNullOrEmpty(pPredicadoRDF))
			{
				pPredicadoRDF = "";
			}

			if (!string.IsNullOrEmpty(error))
			{
				return GnossResultERROR(error);
			}

			try
			{
				TipoDatoExtra tipo;
				bool tipoCorrecto = Enum.TryParse(pTipo, out tipo);
				ControladorDatosExtra controladorDE = new ControladorDatosExtra(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mEntityContextBASE, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

				//Tipo
				if (string.IsNullOrEmpty(pTipo) || !tipoCorrecto)
				{
					return GnossResultERROR("El tipo del dato no es correcto");
				}

				Guid datoExtraID = Guid.NewGuid();

				if (EsAdministracionEcosistema)
				{
					if (tipo.Equals(TipoDatoExtra.Opcion))
					{
						string[] opciones = pOpciones.Split(',');

						controladorDE.GuardarNuevoDatoExtraEcosistema(datoExtraID, pNombre, pObligatorio, pOpciones, pOrden, pPredicadoRDF);
					}
					else
					{
						controladorDE.GuardarNuevoDatoExtraVirtuosoEcosistema(datoExtraID, pNombre, pObligatorio, pOrden, pPredicadoRDF);
					}
				}
				else
				{
					if (tipo.Equals(TipoDatoExtra.Opcion))
					{
						string[] opciones = pOpciones.Split(',');

						controladorDE.GuardarNuevoDatoExtraProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, datoExtraID, pNombre, pObligatorio, pOpciones, pOrden, pPredicadoRDF);
					}
					else
					{
						controladorDE.GuardarNuevoDatoExtraVirtuosoProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, datoExtraID, pNombre, pObligatorio, pOrden, pPredicadoRDF);
					}
				}

				controladorDE.GuardarCambios();

                if (iniciado)
                {
					DatoExtraModel datoExtraModel = new DatoExtraModel();
					
					datoExtraModel.DatoExtraID = datoExtraID;
					datoExtraModel.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
					datoExtraModel.Opciones = ObtenerOpciones(datoExtraID, EsAdministracionEcosistema, tipo);
					datoExtraModel.Orden = pOrden;
					datoExtraModel.Deleted = false;
					datoExtraModel.Obligatorio = pObligatorio;
					datoExtraModel.Tipo = tipo;
					datoExtraModel.PredicadoRDF = pPredicadoRDF;
                    datoExtraModel.Nombre = pNombre;
                    datoExtraModel.ProyectoID = ProyectoSeleccionado.Clave;

                    HttpResponseMessage resultado = InformarCambioAdministracion("DatosExtra", JsonConvert.SerializeObject(datoExtraModel, Formatting.Indented));
                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }
            }
			catch(Exception ex)
			{
				mLoggingService.GuardarLogError(ex.Message);
				return GnossResultERROR("Ha habido un error al guardar los datos");
			}			

			return GnossResultOK("El dato extra ha sido creado correctamente");
		}

		public IActionResult Editar(Guid pDatoExtraID, string pNombre, string pTipo, string pOpciones, bool pObligatorio, int pOrden, string pPredicadoRDF)
		{
            string error = ComprobarErrores(pNombre, pTipo, pOpciones, EsAdministracionEcosistema);

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

            if (string.IsNullOrEmpty(pPredicadoRDF))
			{
				pPredicadoRDF = "";
			}

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }

            try
            {
                TipoDatoExtra tipo;
                bool tipoCorrecto = Enum.TryParse(pTipo, out tipo);
                ControladorDatosExtra controladorDE = new ControladorDatosExtra(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mEntityContextBASE, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                //Tipo
                if (string.IsNullOrEmpty(pTipo) || !tipoCorrecto)
                {
                    return GnossResultERROR("El tipo del dato no es correcto");
                }
				// se cambia el tipo
				DatoExtraModel datoExtra = PaginaModel.ListaDatosExtraProyecto.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();

				if (datoExtra != null)
				{
					if (!datoExtra.Tipo.Equals(tipo))
					{
						Eliminar(pDatoExtraID);
						Guardar(pNombre, pTipo, pOpciones, pObligatorio, pOrden, pPredicadoRDF);

                        return GnossResultOK("El dato extra ha sido modificado correctamente");
                    }
				}
				else
				{
					DatoExtraVirtuosoModel datoExtraVirtuoso = PaginaModel.ListaDatosExtraVirtuoso.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();
					if (datoExtraVirtuoso != null)
					{
						if (!datoExtraVirtuoso.Tipo.Equals(tipo))
						{
							Eliminar(pDatoExtraID);
							Guardar(pNombre, pTipo, pOpciones, pObligatorio, pOrden, pPredicadoRDF);

                            return GnossResultOK("El dato extra ha sido modificado correctamente");
                        }
					}				
				}
				
                if (EsAdministracionEcosistema)
                {
                    if (tipo.Equals(TipoDatoExtra.Opcion))
                    {
                        string[] opciones = pOpciones.Split(',');

                        controladorDE.ModificarDatoExtraEcosistema(pDatoExtraID, pNombre, pObligatorio, pOpciones, pOrden, pPredicadoRDF);
                    }
                    else
                    {
                        controladorDE.ModificarDatoExtraVirtuosoEcosistema(pDatoExtraID, pNombre, pObligatorio, pOrden, pPredicadoRDF);
                    }
                }
                else
                {
                    if (tipo.Equals(TipoDatoExtra.Opcion))
                    {
                        string[] opciones = pOpciones.Split(',');

                        controladorDE.ModificarDatoExtraProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, pDatoExtraID, pNombre, pObligatorio, pOpciones, pOrden, pPredicadoRDF);
                    }
                    else
                    {
                        controladorDE.ModificarDatoExtraVirtuosoProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, pDatoExtraID, pNombre, pObligatorio, pOrden, pPredicadoRDF);
                    }
                }

                controladorDE.GuardarCambios();

                if (iniciado)
                {
                    DatoExtraModel datoExtraModel = new DatoExtraModel();

                    datoExtraModel.DatoExtraID = pDatoExtraID;
                    datoExtraModel.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                    datoExtraModel.Opciones = ObtenerOpciones(pDatoExtraID, EsAdministracionEcosistema, tipo);
                    datoExtraModel.Orden = pOrden;
					datoExtraModel.PredicadoRDF = pPredicadoRDF;
                    datoExtraModel.Deleted = false;
                    datoExtraModel.Obligatorio = pObligatorio;
                    datoExtraModel.Tipo = tipo;
					datoExtraModel.ProyectoID = ProyectoSeleccionado.Clave;
					datoExtraModel.Nombre = pNombre;

                    HttpResponseMessage resultado = InformarCambioAdministracion("DatosExtra", JsonConvert.SerializeObject(datoExtraModel, Formatting.Indented));
                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex.Message);
                return GnossResultERROR("Ha habido un error al guardar los datos");
            }

            return GnossResultOK("El dato extra ha sido modificado correctamente");
		}

		public IActionResult Eliminar(Guid pDatoExtraID)
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

            if (pDatoExtraID.Equals(Guid.Empty))
			{
				return GnossResultERROR("Error al eliminar el dato extra, el ID no es correcto");
			}
			try
			{
				ControladorDatosExtra controladorDE = new ControladorDatosExtra(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mEntityContextBASE, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

				controladorDE.EliminarDatoExtra(pDatoExtraID, EsAdministracionEcosistema);
				controladorDE.GuardarCambios();

                if (iniciado)
                {
                    DatoExtraModel datoExtraModel = new DatoExtraModel();

                    datoExtraModel.DatoExtraID = pDatoExtraID;
                    datoExtraModel.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
					datoExtraModel.PredicadoRDF = string.Empty;
					datoExtraModel.Opciones = null;
                    datoExtraModel.Orden = 0;
                    datoExtraModel.Deleted = true;
                    datoExtraModel.Obligatorio = false;
                    datoExtraModel.Tipo = TipoDatoExtra.TextoLibre;

                    HttpResponseMessage resultado = InformarCambioAdministracion("DatosExtra", JsonConvert.SerializeObject(datoExtraModel, Formatting.Indented));
                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }
            }
			catch(Exception ex)
			{
				mLoggingService.GuardarLogError(ex.Message);
				return GnossResultERROR("Error al eliminar el dato extra");
			}			

			return GnossResultOK();
		}

		[TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Texto, "AdministracionVistasPermitido" })]
		public IActionResult CargarModalEdicion(Guid pDatoID)
		{
			DatoExtraEditModel model = new DatoExtraEditModel();
			DatoExtraModel datoExtraOpcion = PaginaModel.ListaDatosExtraProyecto.Where(item => item.DatoExtraID.Equals(pDatoID)).FirstOrDefault();
			DatoExtraVirtuosoModel datoExtraLibre = null;

			if (datoExtraOpcion == null)
			{
				datoExtraLibre = PaginaModel.ListaDatosExtraVirtuoso.Where(item => item.DatoExtraID.Equals(pDatoID)).FirstOrDefault();
				
				if (datoExtraLibre == null)
				{
					return GnossResultERROR();
				}

				model.Nombre = datoExtraLibre.Nombre;
				model.Obligatorio = datoExtraLibre.Obligatorio;
				model.DatoExtraID = pDatoID;
				model.Opciones = new List<DatoExtraOpcionModel>();
				model.Tipo = datoExtraLibre.Tipo;
				model.Orden = datoExtraLibre.Orden;
				model.PredicadoRDF = datoExtraLibre.PredicadoRDF;
			}
			else
			{
                model.Nombre = datoExtraOpcion.Nombre;
                model.Obligatorio = datoExtraOpcion.Obligatorio;
                model.DatoExtraID = pDatoID;
				model.Opciones = datoExtraOpcion.Opciones;
                model.Tipo = datoExtraOpcion.Tipo;
				model.Orden = datoExtraOpcion.Orden;
				model.PredicadoRDF = datoExtraOpcion.PredicadoRDF;
            }			

			return GnossResultHtml("../AdministrarDatosExtra/_modal-views/_edit-extra-data", model);
		}

		private List<DatoExtraOpcionModel> ObtenerOpciones(Guid pDatoExtraID, bool pEcosistema, TipoDatoExtra pTipo)
		{
			List<DatoExtraOpcionModel> listaOpciones = new List<DatoExtraOpcionModel>();

			if (pTipo.Equals(TipoDatoExtra.Opcion))
			{
                ControladorDatosExtra controladorDE = new ControladorDatosExtra(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mEntityContextBASE, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

				listaOpciones = controladorDE.ObtenerOpciones(pDatoExtraID, pEcosistema);
            }

			return listaOpciones;
        }
		private string ComprobarErrores(string pNombre, string pTipo, string pOpciones, bool pEcosistema)
		{
			TipoDatoExtra tipo;
			bool tipoCorrecto = Enum.TryParse(pTipo, out tipo);
			
			//Tipo
			if (string.IsNullOrEmpty(pTipo) || !tipoCorrecto)
			{
				return "El dato extra debe tener un tipo";
			}

			//Opciones
			if (tipo.Equals(TipoDatoExtra.Opcion) && string.IsNullOrEmpty(pOpciones))
			{
				return "El dato extra de tipo OPCION debe tener al menos una opción";
			}

			//Nombre
			if (string.IsNullOrEmpty(pNombre))
			{
				return "El dato extra debe tener un nombre";
			}

			return string.Empty;
		}

        public AdministrarDatosExtraViewModel PaginaModel
        {
            get 
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarDatosExtraViewModel();
                    mPaginaModel.ListaDatosExtraVirtuoso = new List<DatoExtraVirtuosoModel>();
                    mPaginaModel.ListaDatosExtraProyecto = new List<DatoExtraModel>();

                    if (EsAdministracionEcosistema)
                    {
                        // DatoExtraEcosistema -> opciones
                        foreach (DatoExtraEcosistema datoExtraEcosistema in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema)
                        {
                            DatoExtraModel datoExtraModel = new DatoExtraModel();

                            datoExtraModel.DatoExtraID = datoExtraEcosistema.DatoExtraID;
                            datoExtraModel.Obligatorio = datoExtraEcosistema.Obligatorio;
                            datoExtraModel.Nombre = datoExtraEcosistema.Titulo;
                            datoExtraModel.PredicadoRDF = datoExtraEcosistema.PredicadoRDF;
                            datoExtraModel.Orden = datoExtraEcosistema.Orden;
							datoExtraModel.Tipo = TipoDatoExtra.Opcion;
							datoExtraModel.Opciones = new List<DatoExtraOpcionModel>();

							foreach (DatoExtraEcosistemaOpcion opcion in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaOpcion.Where(x => x.DatoExtraID.Equals(datoExtraModel.DatoExtraID)).ToList())
                            {
                                DatoExtraOpcionModel opcionModel = new DatoExtraOpcionModel();

                                opcionModel.DatoExtraID = opcion.DatoExtraID;
                                opcionModel.OpcionID = opcion.OpcionID;
                                opcionModel.Orden = opcion.Orden;
                                opcionModel.Nombre = opcion.Opcion;

                                datoExtraModel.Opciones.Add(opcionModel);
                            }
                            
                            mPaginaModel.ListaDatosExtraProyecto.Add(datoExtraModel);
                        }

						// DatoExtraEcosistemaVirtuoso -> texto libre
						foreach (DatoExtraEcosistemaVirtuoso datoExtraEcosistemaVirtuoso in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso)
						{
							DatoExtraVirtuosoModel datoExtraModel = new DatoExtraVirtuosoModel();

							datoExtraModel.DatoExtraID = datoExtraEcosistemaVirtuoso.DatoExtraID;
							datoExtraModel.Obligatorio = datoExtraEcosistemaVirtuoso.Obligatorio;
							datoExtraModel.Nombre = datoExtraEcosistemaVirtuoso.Titulo;
							datoExtraModel.PredicadoRDF = datoExtraEcosistemaVirtuoso.PredicadoRDF;
							datoExtraModel.Orden = datoExtraEcosistemaVirtuoso.Orden;
                            datoExtraModel.NombreInput = datoExtraEcosistemaVirtuoso.InputID;
                            datoExtraModel.QueryVirtuoso = datoExtraEcosistemaVirtuoso.QueryVirtuoso;
							datoExtraModel.Tipo = TipoDatoExtra.TextoLibre;

							mPaginaModel.ListaDatosExtraVirtuoso.Add(datoExtraModel);
						}
					}
                    else
                    {
						// DatoExtraProyecto -> opciones
						foreach (DatoExtraProyecto datoExtraProyecto in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto)
						{
							DatoExtraModel datoExtraModel = new DatoExtraModel();

							datoExtraModel.DatoExtraID = datoExtraProyecto.DatoExtraID;
							datoExtraModel.Obligatorio = datoExtraProyecto.Obligatorio;
							datoExtraModel.Nombre = datoExtraProyecto.Titulo;
							datoExtraModel.PredicadoRDF = datoExtraProyecto.PredicadoRDF;
							datoExtraModel.Orden = datoExtraProyecto.Orden;
							datoExtraModel.OrganizacionID = datoExtraProyecto.OrganizacionID;
							datoExtraModel.ProyectoID = datoExtraProyecto.ProyectoID;
							datoExtraModel.Tipo = TipoDatoExtra.Opcion;
							datoExtraModel.Opciones = new List<DatoExtraOpcionModel>();

							foreach (DatoExtraProyectoOpcion opcion in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoOpcion.Where(x => x.DatoExtraID.Equals(datoExtraModel.DatoExtraID)).ToList())
							{
								DatoExtraOpcionModel opcionModel = new DatoExtraOpcionModel();

								opcionModel.DatoExtraID = opcion.DatoExtraID;
								opcionModel.OpcionID = opcion.OpcionID;
								opcionModel.Orden = opcion.Orden;
								opcionModel.Nombre = opcion.Opcion;
								opcionModel.ProyectoID = opcion.ProyectoID;
								opcionModel.OrganizacionID = opcion.OrganizacionID;

								datoExtraModel.Opciones.Add(opcionModel);
							}

							mPaginaModel.ListaDatosExtraProyecto.Add(datoExtraModel);
						}

						// DatoExtraProyectoVirtuoso
						foreach (DatoExtraProyectoVirtuoso datoExtraProyectoVirtuoso in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso)
						{
							DatoExtraVirtuosoModel datoExtraModel = new DatoExtraVirtuosoModel();

							datoExtraModel.DatoExtraID = datoExtraProyectoVirtuoso.DatoExtraID;
							datoExtraModel.Obligatorio = datoExtraProyectoVirtuoso.Obligatorio;
							datoExtraModel.Nombre = datoExtraProyectoVirtuoso.Titulo;
							datoExtraModel.PredicadoRDF = datoExtraProyectoVirtuoso.PredicadoRDF;
							datoExtraModel.Orden = datoExtraProyectoVirtuoso.Orden;
							datoExtraModel.NombreInput = datoExtraProyectoVirtuoso.InputID;
							datoExtraModel.QueryVirtuoso = datoExtraProyectoVirtuoso.QueryVirtuoso;
							datoExtraModel.Tipo = TipoDatoExtra.TextoLibre;

							mPaginaModel.ListaDatosExtraVirtuoso.Add(datoExtraModel);
						}
					}
				}

                return mPaginaModel;
            }
        }

		public DataWrapperProyecto DatosExtraProyectoDataWrapperProyecto
		{
			get
			{
				if (mDatosExtraProyectoDataWrapperProyecto == null)
				{
					ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
					mDatosExtraProyectoDataWrapperProyecto = proyectoCN.ObtenerDatosExtraProyectoPorID(ProyectoSeleccionado.Clave);
					proyectoCN.Dispose();
				}

				return mDatosExtraProyectoDataWrapperProyecto;
			}
		}
	}
}
