﻿using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
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
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{

    public class AdministrarUtilidadesController : ControllerBaseWeb
    {
        public AdministrarUtilidadesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarComunidadUtilidades mPaginaModel = null;

        #endregion

        // GET: AdministrarUtilidades
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clases para el body del Layout
            ViewBag.BodyClassPestanya = "permisos-comunidad comunidad no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_TiposDeContenidoPermisos;
            // Establecer el título para el header de DevTools            
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("ADMINISTRACIONSEMANTICA", "COMUNIDAD");            
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "TIPOSDECONTENIDO/PERMISOS");

            // Activar la visualización del icono de la documentación de la sección
            ViewBag.showDocumentationByDefault = "true";
            // Indica si sólo se desea visualizar la documentación por secciones (Ej: Mostrar la documentación en el modal con el contenido desplegado/plegado)
            ViewBag.showDocumentationSection = "true";

            return View(PaginaModel);
        }

        /// <summary>
        /// Cargar la página index para gestionar los certificados de la comunidad  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IndexGestionCertificados()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clases para el body del Layout
            ViewBag.BodyClassPestanya = "edicionCertificados comunidad max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_NivelesDeCertificacion;
            // Establecer el título para el header de DevTools            
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("ADMINISTRACIONSEMANTICA", "COMUNIDAD");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "NIVELESDECERTIFICACION");

            // Activar la visualización del icono de la documentación de la sección
            ViewBag.showDocumentationByDefault = "true";
            // Indica si sólo se desea visualizar la documentación por secciones (Ej: Mostrar la documentación en el modal con el contenido desplegado/plegado)
            ViewBag.showDocumentationSection = "true";

            // Devolver la página para la gestión de certificados
            return View("../AdministrarUtilidades/Index_NivelesCertificacion", PaginaModel);                   
        }



        /// <summary>
        /// Devolver la vista modal para ser insertada en un modal de bootstrap. Contiene las opciones para poder añadir Grupos para asignación de permisos
        /// sobre tipo de recursos.        
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadAddGroup()
        {
            ActionResult partialView = View();
            partialView = GnossResultHtml("../AdministrarUtilidades/_modal-views/_add-groups", null);
                    
            // Devolver la sección de la home correspondiente
            return partialView;
        }


        /// <summary>
        /// Devolver la vista modal para editar o crear una nueva certificación        
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadCreateEditCertification(AdministrarUtilidadesCertificationModel pModel)
        {
            ActionResult partialView = View();

            // Construir el modelo y comprobar

            partialView = GnossResultHtml("../AdministrarUtilidades/_modal-views/_add-edit-certifications", pModel);

            // Devolver la vista modal
            return partialView;
        }

        /// <summary>
        /// Devolver la vista modal para confirmación de la eliminación de un certificado en concreto.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadConfirmDeleteCertification(AdministrarUtilidadesCertificationModel pModel)
        {
            ActionResult partialView = View();

            // Construir el modelo y comprobar
            partialView = GnossResultHtml("../AdministrarUtilidades/_modal-views/_confirm-delete-certification", pModel);

            // Devolver la vista modal
            return partialView;
        }



        // POST: Guardar AdministrarUtilidades
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Guardar(AdministrarComunidadUtilidades DatosGuardado)
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

            ControladorUtilidades contrUtilidades = new ControladorUtilidades(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mEntityContextBASE, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            string errores = ComprobarErrores(DatosGuardado);

            if (string.IsNullOrEmpty(errores))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;

                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    contrUtilidades.GuardarUtilidades(DatosGuardado);
                    mControladorBase.LimpiarListaOntologiasPermitidasPorIdentidad();
                    if (iniciado)
                    {
                        contrUtilidades.CargarNivelesCertificacion(DatosGuardado, ParametrosGeneralesRow.PoliticaCertificacion, this.IdentidadActual, ProyectoVirtual.TipoProyecto, UtilIdiomas);
                        HttpResponseMessage resultado = InformarCambioAdministracion("Utilidades", JsonConvert.SerializeObject(DatosGuardado, Formatting.Indented));

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


                contrUtilidades.InvalidarCache();
                return GnossResultOK();
            }

            return GnossResultERROR(errores);
        }


        // POST: Guardar exclusivamente los Certificados de la comunidad.
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult SaveCertifications(AdministrarComunidadUtilidades DatosGuardado)
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

            ControladorUtilidades contrUtilidades = new ControladorUtilidades(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mEntityContextBASE, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            
            // Comprobar errores 
            string errores = ComprobarErrores(DatosGuardado);

            if (string.IsNullOrEmpty(errores))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool transaccionIniciada = false;

                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    // Guardar datos exclusivamente de certificados
                    contrUtilidades.GuardarUtilidadesNivelesCertificacion(DatosGuardado);                    

                    if (iniciado)
                    {

                        contrUtilidades.CargarTiposYPermisos(DatosGuardado);
                        HttpResponseMessage resultado = InformarCambioAdministracion("Utilidades", JsonConvert.SerializeObject(DatosGuardado, Formatting.Indented));

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


                contrUtilidades.InvalidarCache();
                return GnossResultOK();
            }

            return GnossResultERROR(errores);
        }


        private string ComprobarErrores(AdministrarComunidadUtilidades DatosGuardado)
        {
            string error = "";

            if (DatosGuardado.NivelesCertificacion != null)
            {
                foreach (AdministrarComunidadUtilidades.NivelCertificacion nivelCertificacion in DatosGuardado.NivelesCertificacion)
                {
                    if (string.IsNullOrEmpty(nivelCertificacion.Nombre))
                    {
                        error = "ERROR_NOMBRE_CERTIFICACION_VACIO";
                    }
                }
            }
            return error;
        }


        private AdministrarComunidadUtilidades PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    ControladorUtilidades contrUtilidades = new ControladorUtilidades(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mEntityContextBASE, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                    mPaginaModel = contrUtilidades.CargarUtilidades();

                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mPaginaModel.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
                    mPaginaModel.IdiomaDefecto = IdiomaPorDefecto;

                    //Si no hay videos de brightcove disponibles, deshabilito los checks de brightcove
                    if (ParametrosGeneralesRow.PlataformaVideoDisponible == (short)PlataformaVideoDisponible.Brightcove)
                    {
                        ViewBag.BrightcoveDisponible = true;
                    }
                    //Si no hay videos de TOP disponibles, deshabilito los checks de TOP
                    else if (ParametrosGeneralesRow.PlataformaVideoDisponible == (short)PlataformaVideoDisponible.TOP)
                    {
                        ViewBag.TOPDisponible = true;
                    }

                    //Añade la política de certificación de la comunidad
                    if (string.IsNullOrEmpty(ParametrosGeneralesRow.PoliticaCertificacion) || string.IsNullOrWhiteSpace(ParametrosGeneralesRow.PoliticaCertificacion))
                    {
                        //TipoProyecto.EducacionExpandida
                        if (ProyectoVirtual.TipoProyecto != AD.ServiciosGenerales.TipoProyecto.EducacionExpandida)
                        {
                            mPaginaModel.PoliticaCertificacion = UtilIdiomas.GetText("COMADMIN", "ELABORANDOPOLITICA", this.IdentidadActual.Nombre(), "administrador");
                        }
                        else
                        {
                            mPaginaModel.PoliticaCertificacion = UtilIdiomas.GetText("COMADMIN", "POLITICACERTIFICACION", this.IdentidadActual.Nombre());
                        }

                    }
                }
                return mPaginaModel;
            }
        }


        // Modelo de datos usado para editar un determinado recurso existente a través del modal de Crear/Editar certificado desde Administración
        public class AdministrarUtilidadesCertificationModel
        {
            // Id del certificado
            public string id { get; set; }
            // Nombre del certificado
            public string name { get; set; }
            // Posición en la que estará el certificado
            public string position { get; set; }                      
        }
    }
}