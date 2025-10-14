using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class CondicionesUsoController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public CondicionesUsoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<CondicionesUsoController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        //
        // GET: /CondicionesUso/
        public ActionResult Index()
        {
            string textoCondiciones = GetCondicionesUsoContentPage();       
            return View("Index", textoCondiciones);
        }

        // Acción para devolver la vista que contendrá las Condiciones de uso en un modal
        [HttpPost]
        public ActionResult LoadModal()
        {
            string textoCondiciones = GetCondicionesUsoContentPage();
            return GnossResultHtml("_pie/_modal-views/_usage-conditions", textoCondiciones);
        }

        private string GetCondicionesUsoContentPage() {
            string textoCondiciones = "";
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            DataWrapperUsuario usuarioDW = proyCL.ObtenerClausulasRegitroProyecto(ProyectoVirtual.Clave);

            if (!usuarioDW.ListaClausulaRegistro.Any(item => item.Tipo.Equals((short)TipoClausulaAdicional.CondicionesUso)))
            {
                usuarioDW = proyCL.ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto);
            }

            var condicionesUso = usuarioDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.CondicionesUso)).OrderBy(item => item.Orden).FirstOrDefault();
            if (condicionesUso != null)
            {
                textoCondiciones = UtilCadenas.ObtenerTextoDeIdioma(condicionesUso.Texto, IdiomaUsuario, null);
            }

            ViewBag.NombreProyClausulas = proyCL.ObtenerNombreDeProyectoID(ProyectoAD.MetaProyecto);

            return textoCondiciones;
        }
    }
}
