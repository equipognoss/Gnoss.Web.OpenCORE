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
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class PoliticaPrivacidadController : ControllerBaseWeb
    {
        public PoliticaPrivacidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        protected override void CargarTituloPagina()
        {
            TituloPagina = UtilIdiomas.GetText("POLITICAPRIVACIDAD", "TITULOPAGINA");

            base.CargarTituloPagina();
        }

        public ActionResult Index()
        {
            string textoPoliticaPrivacidad = GetPoliticaPrivacidadContentPage();
            return View("Index", textoPoliticaPrivacidad);
        }

        // Acción para devolver la vista que contendrá la Política de Privacidad en un modal
        [HttpPost]
        public ActionResult LoadModal()
        {
            string textoPoliticaPrivacidad = GetPoliticaPrivacidadContentPage();
            return GnossResultHtml("_pie/_modal-views/_privacy-policy", textoPoliticaPrivacidad);
        }

        private string GetPoliticaPrivacidadContentPage()
        {
            string textoPoliticaPrivacidad = "";

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            DataWrapperUsuario usuarioDW = proyCL.ObtenerClausulasRegitroProyecto(ProyectoVirtual.Clave);

            if (usuarioDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.ClausulasTexo)).Select(item => item.Orden).ToList().Count == 0)
            {
                usuarioDW = proyCL.ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto);
            }

            AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula = usuarioDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.ClausulasTexo)).OrderBy(item => item.Orden).FirstOrDefault();

            if (filaClausula != null)
            {
                textoPoliticaPrivacidad = UtilCadenas.ObtenerTextoDeIdioma(filaClausula.Texto, IdiomaUsuario, null);
            }
            return textoPoliticaPrivacidad;
        }
    }
}
