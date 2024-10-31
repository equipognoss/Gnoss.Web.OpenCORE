using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class ActividadRecienteController : ControllerBaseWeb
    {
        public ActividadRecienteController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        public ActionResult Index(int TypeActivity, int NumPeticion, Guid ComponentKey, Guid? ProfileKey)
        {
            int numItems = 10;
            if (ParametroProyecto.ContainsKey("FilasPorPagina"))
            {
                Int32.TryParse(ParametroProyecto["FilasPorPagina"], out numItems);
            }
            else if(ParametrosAplicacionDS.Any(parametro => parametro.Parametro.Equals("FilasPorPagina")))
            {
                Int32.TryParse(ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("FilasPorPagina")).First().Valor, out numItems);
            }

            if (ComponentKey != Guid.Empty)
            {
                CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerComponentePorID(ComponentKey,ProyectoSeleccionado.Clave, true), mLoggingService, mEntityContext);
                cmsCN.Dispose();

                if (gestorCMS.ListaComponentes.ContainsKey(ComponentKey) && gestorCMS.ListaComponentes[ComponentKey] is CMSComponenteActividadReciente)
                {
                    CMSComponenteActividadReciente Componente = (CMSComponenteActividadReciente)gestorCMS.ListaComponentes[ComponentKey];
                    numItems = Componente.NumeroItems;
                }
            }

            string partialView = "ControlesMVC/_ActividadReciente";
            if ((TipoActividadReciente)TypeActivity == TipoActividadReciente.PerfilProyecto)
            {
                partialView = "ControlesMVC/_ActividadRecientePerfil";
            }

            ActividadReciente actividadRecienteController = new ActividadReciente(mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mEnv);
            return PartialView(partialView, actividadRecienteController.ObtenerActividadReciente(NumPeticion, numItems, (TipoActividadReciente)TypeActivity, ProfileKey, false, ComponentKey));
        }
    }
}
