using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Controllers;
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
using System.Data.Entity;
using System.Linq;

namespace Gnoss.Web.Controllers.Administracion
{
    public class AdministrarSharepointController : ControllerBaseWeb
    {
        private AdministrarSharepointViewModel mPaginaModel = null;
        private GestorParametroAplicacion mGestorParametrosAplicacion = null;
        private string mDominioBase;
        private string mClientID;
        private string mTenantID;
        private string mClientSecret;
        private string mUrlAdminConsent;
        private string mUrlLoginSharepoint;
        private string mUrlObtenerTokenSharepoint;
        private string mUrlRedireccionSharepoint;
        private bool mPermisosConcedidos;
        private bool mPermitirVincularOneDrive;

        public AdministrarSharepointController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime) 
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "administrar-sharepoint";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_IntegracionSharePoint;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("ADMINISTRACIONSEMANTICA", "COMUNIDAD");            
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "ADMINISTRARSHAREPOINT");

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Activar la visualización del icono de la documentación de la sección
            ViewBag.showDocumentationByDefault = "true";
            // Indica si sólo se desea visualizar la documentación por secciones (Ej: Mostrar la documentación en el modal con el contenido desplegado/plegado)
            ViewBag.showDocumentationSection = "true";

            return View(PaginaModel);
        }

        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(AdministrarSharepointViewModel pModel)
        {
            GuardarLogAuditoria();
            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            try
            {               
                mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);
                GuardarConfiguracion(pModel);
                mEntityContext.SaveChanges();

                if (transaccionIniciada)
                {
                    mEntityContext.TerminarTransaccionesPendientes(true);
                }

                return GnossResultOK();

            } 
            catch (Exception ex)
            {
                if (transaccionIniciada)
                {
                    proyAD.TerminarTransaccion(false);
                }

                return GnossResultERROR("Error: " + ex.Message);
            }
        }

        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Manual()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View("ManualSP");
        }

        private AdministrarSharepointViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarSharepointViewModel();
                    mPaginaModel.DominioBase = DominioBase;
                    mPaginaModel.TenantID = TenantID;
                    mPaginaModel.ClientID = ClientID;
                    mPaginaModel.ClientSecret = ClientSecret;
                    mPaginaModel.UrlAdminConsent = UrlAdminConsent;
                    mPaginaModel.UrlLoginSharepoint = UrlLoginSharepoint;
                    mPaginaModel.UrlObtenerTokenSharepoint = UrlObtenerTokenSharepoint;
                    mPaginaModel.UrlRedireccionSharepoint = UrlRedireccionSharepoint;
                    mPaginaModel.PermisosConcedidos = PermisosConcedidos;
                    mPaginaModel.PermitirOneDrive = PermitirVincularOneDrive;
                }

                return mPaginaModel;
            }
        }

        private void GuardarConfiguracion(AdministrarSharepointViewModel pModel)
        {
            GuardarParametro(pModel.ClientID, ParametroAD.SharepointClientID);
            GuardarParametro(pModel.ClientSecret, ParametroAD.SharepointClientSecret);
            GuardarParametro(pModel.TenantID, ParametroAD.SharepointTenantID);
            GuardarParametro(pModel.PermitirOneDrive.ToString(), ParametroAD.PermitirEnlazarDocumentosOneDrive);
            InvalidarCaches();
        }

        private void GuardarParametro(string pModelParam, string pParam)
        {
            ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);           
            paramCN.ActualizarParametroAplicacion(pParam, pModelParam);
        }

        private void InvalidarCaches()
        {
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL("acid", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            paramCL.InvalidarCacheParametrosAplicacion();
        }

        public GestorParametroAplicacion GestorParametrosAplicacion
        {
            get
            {
                if (mGestorParametrosAplicacion == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

                    mGestorParametrosAplicacion = paramCL.ObtenerGestorParametros();
                }

                return mGestorParametrosAplicacion;
            }
        }

        private string ClientID
        {
            get
            {
                mClientID = "";
                ParametroAplicacion parametroAplicacion = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.SharepointClientID)).FirstOrDefault();
                
                if (parametroAplicacion != null)
                {
                    mClientID = parametroAplicacion.Valor;
                }     
                
                return mClientID;
            }
            set
            {
                mClientID = value;
            }
        }

        private string TenantID
        {
            get
            {
                mTenantID = "";
                ParametroAplicacion parametroAplicacion = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.SharepointTenantID)).FirstOrDefault();
                
                if (parametroAplicacion != null)
                {
                    mTenantID = parametroAplicacion.Valor;
                }

                return mTenantID;
            }
            set
            {
                mTenantID = value;
            }
        }

        private string ClientSecret
        {
            get
            {
                ClientSecret = "";
                ParametroAplicacion parametroAplicacion = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.SharepointClientSecret)).FirstOrDefault();
                
                if (parametroAplicacion != null)
                {
                    mClientSecret = parametroAplicacion.Valor;
                }

                return mClientSecret;
            }
            set
            {
                mClientSecret = value;
            }
        }

        private bool PermitirVincularOneDrive
        {
            get
            {
                PermitirVincularOneDrive = false;
                ParametroAplicacion parametroAplicacion = GestorParametroAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.PermitirEnlazarDocumentosOneDrive)).FirstOrDefault();

                if (parametroAplicacion != null)
                {
                    mPermitirVincularOneDrive = bool.Parse(parametroAplicacion.Valor);
                }

                return mPermitirVincularOneDrive;
            }
            set
            {
                mPermitirVincularOneDrive = value;
            }
        }

        private string UrlAdminConsent
        {
            get
            {
                mUrlAdminConsent = $"https://login.microsoftonline.com/{TenantID}/adminconsent?client_id={ClientID}";
                
                return mUrlAdminConsent;
            }
            set
            {
                mUrlAdminConsent = value;
            }
        }

        private bool PermisosConcedidos
        {
            get
            {
                mPermisosConcedidos = false;
                
                return mPermisosConcedidos;
            }
            set
            {
                mPermisosConcedidos = value;
            }
        }

        private string UrlLoginSharepoint
        {
            get
            {
                mUrlLoginSharepoint = "";
                string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();

                if (!string.IsNullOrEmpty(urlServicioLogin))
                {
                    mUrlLoginSharepoint = $"{urlServicioLogin}/LoginSharepoint";
                }

                return mUrlLoginSharepoint;
            }
            set
            {
                mUrlLoginSharepoint = value;
            }
        }

        private string UrlRedireccionSharepoint
        {
            get
            {
                mUrlRedireccionSharepoint = "";
                string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();

                if (!string.IsNullOrEmpty(urlServicioLogin))
                {
                    mUrlRedireccionSharepoint = $"{urlServicioLogin}/Redireccion";
                }

                return mUrlRedireccionSharepoint;
            }
            set
            {
                mUrlRedireccionSharepoint = value;
            }
        }

        private string UrlObtenerTokenSharepoint
        {
            get
            {
                mUrlObtenerTokenSharepoint = "";
                string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();

                if (!string.IsNullOrEmpty(urlServicioLogin))
                {
                    mUrlObtenerTokenSharepoint = $"{urlServicioLogin}/ObtenerTokenSharepoint";
                }

                return mUrlObtenerTokenSharepoint;
            }
            set
            {
                mUrlObtenerTokenSharepoint = value;
            }
        }

        private string DominioBase
        {
            get
            {
                mDominioBase = "";
                string dominio = mConfigService.ObtenerDominio();
                
                if (!string.IsNullOrEmpty(dominio))
                {
                    mDominioBase = dominio;
                }

                return mDominioBase;
            }
            set
            {
                mDominioBase = value;
            }
        }
    }
}
