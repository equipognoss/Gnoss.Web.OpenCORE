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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class ActividadRecienteController : ControllerBaseWeb
    {
        public ActividadRecienteController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
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
            //int numeroPeticion = int.Parse(NumPeticion);
            //int tipoActividad = int.Parse(TypeActivity);

            if (ComponentKey != Guid.Empty)
            {
                CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionCMS gestorCMS=new GestionCMS(cmsCN.ObtenerComponentePorID(ComponentKey,ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                cmsCN.Dispose();

                //.NumeroItems
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

            ActividadReciente actividadRecienteController = new ActividadReciente(mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
            return PartialView(partialView, actividadRecienteController.ObtenerActividadReciente(NumPeticion, numItems, (TipoActividadReciente)TypeActivity, ProfileKey, false, ComponentKey));
        }
    }
}
