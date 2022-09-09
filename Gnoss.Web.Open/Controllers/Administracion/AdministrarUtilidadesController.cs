using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
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
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{

    public class AdministrarUtilidadesController : ControllerBaseWeb
    {
        public AdministrarUtilidadesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
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
            return View(PaginaModel);
        }

        // POST: Guardar AdministrarUtilidades
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Guardar(AdministrarComunidadUtilidades DatosGuardado)
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

            ControladorUtilidades contrUtilidades = new ControladorUtilidades(ProyectoSeleccionado, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mEntityContextBASE, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            string errores = ComprobarErrores(DatosGuardado);

            if (string.IsNullOrEmpty(errores))
            {
                GuardarXmlCambiosAdministracion();
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

        
    }
}