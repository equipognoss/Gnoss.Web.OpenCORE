using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
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
using System.Collections.Generic;
using System.Linq;



namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class FichaRecursoHistorialController : ControllerBaseWeb
    {

        public FichaRecursoHistorialController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        protected override void CargarTituloPagina()
        {
            TituloPagina = UtilIdiomas.GetText("HISTORIALVERSIONES", "TITULOPAGINA");

            base.CargarTituloPagina();
        }

        public ActionResult Index()
        {
            if(string.IsNullOrEmpty(RequestParams("docID")))
            {
                return Redirect(Comunidad.Url);
            }

            Guid docID = new Guid(RequestParams("docID"));

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, int> listaVersionDocumentos = docCN.ObtenerVersionesDocumentoIDPorID(docID);
            docCN.Dispose();

            Dictionary<Guid, ResourceModel> listaRecursos = ControladorProyectoMVC.ObtenerRecursosPorID(listaVersionDocumentos.Keys.ToList(), "", null, false);

            foreach(ResourceModel recurso in listaRecursos.Values.ToList())
            {
                recurso.Version = listaVersionDocumentos[recurso.Key];
            }

            return View(listaRecursos.Values.ToList());
        }

        public ActionResult Compare()
        {
            string[] param = RequestParams("documentosComparar").Split(new char[]{'&'}, StringSplitOptions.RemoveEmptyEntries);
            Guid DocumentoID = new Guid(param[0]);
            Guid DocumentoAntID = new Guid(param[1]);

            List<Guid> listaVersionDocumentos = new List<Guid>();
            listaVersionDocumentos.Add(DocumentoID);
            listaVersionDocumentos.Add(DocumentoAntID);

            Dictionary<Guid, ResourceModel> listaRecursos = ControladorProyectoMVC.ObtenerRecursosPorID(listaVersionDocumentos, "", null, false);

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, int> listaVersionDocumentosAux = docCN.ObtenerVersionesDocumentoIDPorID(DocumentoID);
            docCN.Dispose();

            foreach (ResourceModel recurso in listaRecursos.Values.ToList())
            {
                recurso.Version = listaVersionDocumentosAux[recurso.Key];
            }

            return PartialView("_Comparador", listaRecursos.Values.ToList());
        }
    }
}
