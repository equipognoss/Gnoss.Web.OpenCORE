using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class CondicionesUsoController : ControllerBaseWeb
    {

        public CondicionesUsoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
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
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
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
            return textoCondiciones;
        }
    }
}
