using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class ControllerAdministrationWeb : ControllerBaseWeb
    {
        protected ILogger mlogger;
        protected ILoggerFactory mLoggerFactory;
        public ControllerAdministrationWeb(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<ControllerAdministrationWeb> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        protected void CargarPermisosAdministracionComunidadEnViewBag()
        {
            if (string.IsNullOrEmpty(RequestParams("new-community-wizard")))
            {
                ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
            }

            if (HayIntegracionContinua)
            {
                Comunidad.IntegracionContinuaActivada = true;
                Comunidad.RamaActualGit = RamaEnUsoGit;
                Comunidad.VersionRamaRelease = VersionRama;

                if (string.IsNullOrEmpty(Comunidad.RamaActualGit) || Comunidad.RamaActualGit == "develop")
                {
                    if (!EntornoActualEsPreproduccion && !EntornoActualEsPruebas)
                    {
                        Comunidad.IntContActivadaSinRamaEnUso = false;
                    }
                    else
                    {
                        Comunidad.IntContActivadaSinRamaEnUso = true;
                    }
                }

                if (!UsuarioEstaDadoDeAltaEnIntCont)
                {
                    Comunidad.UsuarioDadoAltaIntCont = false;
                }
                else
                {
                    Comunidad.UsuarioDadoAltaIntCont = true;
                }

                if (EntornoActualEsPreproduccion)
                {
                    Comunidad.EntornoEsPre = true;
                }

                if (!Comunidad.EntornoEsPre && !EntornoActualEsPruebas)
                {
                    Comunidad.EntornoEsPro = true;
                }

                if (EntornoActualBloqueado)
                {
                    Comunidad.EntornoBloqueado = true;
                }

                Comunidad.DeshabilitarGuardarAdministracionIC = Comunidad.IntContActivadaSinRamaEnUso || !Comunidad.UsuarioDadoAltaIntCont || Comunidad.EntornoBloqueado || Comunidad.EntornoEsPre;

                ObtenerInformacionBannerAvisoIC();
            }
            else
            {
                if (mEnv.IsProduction())
                {
                    Comunidad.EntornoEsPro = true;
                }
                else if (mEnv.IsStaging())
                {
                    Comunidad.EntornoEsPre = true;
                }
            }
        }

        public bool EsAdministracionEcosistema
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("ecosistema")) && RequestParams("ecosistema").Equals("true"))
                {
                    ViewBag.PersonalizacionAdministracionEcosistema = mControladorBase.PersonalizacionEcosistemaID;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Obtiene el mensaje para mostrar en el banner que indica el estado de la integración continua y las variables
        /// que indican a la vista que botones pintar en el banner
        /// </summary>
        private void ObtenerInformacionBannerAvisoIC()
        {

            if (HayRamaOVersionActiva(Comunidad.RamaActualGit))
            {
                if (Comunidad.EntornoEsPre)
                {
                    Comunidad.MensajeAvisoAdministracionIC = UtilIdiomas.GetText("DEVTOOLS", "ESTASENPRERAMAACTIVANOPUEDESHACERCAMBIOS", Comunidad.RamaActualGit);
                }
                else if (EsVersion(Comunidad.RamaActualGit))
                {
                    Comunidad.MensajeAvisoAdministracionIC = UtilIdiomas.GetText("DEVTOOLS", "ESTASENLARAMARELEASE(VERSION@1@)PUEDESHACERCAMBIOSENLAADMIN", Comunidad.VersionRamaRelease);
                    Comunidad.MostrarBotonAdministrarDespliegues = true;
                }
                else
                {
                    //Es Rama
                    Comunidad.MensajeAvisoAdministracionIC = UtilIdiomas.GetText("DEVTOOLS", "ESTASENLA@1@,SOLOSETEPERMITEHACERCAMBIOSENADMINISTRACIONYCER", Comunidad.RamaActualGit);
                    Comunidad.MostrarBotonAdministrarRamas = true;
                }
            }
            else
            {
                if (Comunidad.EntornoBloqueado)
                {
                    Comunidad.MensajeAvisoAdministracionIC = UtilIdiomas.GetText("DEVTOOLS", "ELENTORNOACTUALESTABLOQUEADOPORQUESEHAVALIDADOUNAVERSIONDEB");
                }
                else
                {
                    Comunidad.MensajeAvisoAdministracionIC = UtilIdiomas.GetText("DEVTOOLS", "NOTIENESUNARAMAACTIVA,DEBESCREARUNARAMAPARAPODERSUBIRLOSCAMB");
                    Comunidad.MostrarBotonAdministrarDespliegues = true;
                    Comunidad.MostrarBotonAdministrarRamas = true;
                }
            }
        }

        private bool HayRamaOVersionActiva(string ramaActualGit)
        {
            return !string.Equals(ramaActualGit, string.Empty) && !string.Equals(ramaActualGit, "develop") && !ramaActualGit.StartsWith("release_");
        }
        
        private bool EsVersion(string ramaActualGit)
        {
            return ramaActualGit.StartsWith("release_");
        }
    }
}
