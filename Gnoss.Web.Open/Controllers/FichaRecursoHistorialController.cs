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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using static Es.Riam.Gnoss.Web.MVC.Models.ResourceModel.PollModel;



namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class FichaRecursoHistorialController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public FichaRecursoHistorialController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<FichaRecursoHistorialController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
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

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
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
            
            Dictionary<Guid, ResourceModel> listaRecursos = ControladorProyectoMVC.ObtenerRecursosPorID(listaVersionDocumentos, "", null, false, pEsFichaRecurso: true);

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            Dictionary<Guid, int> listaVersionDocumentosAux = docCN.ObtenerVersionesDocumentoIDPorID(listaRecursos.Values.First().OriginalKey);
            Guid ultimaVersion = listaVersionDocumentosAux.OrderByDescending(item => item.Value).FirstOrDefault().Key;


            foreach (ResourceModel recurso in listaRecursos.Values.ToList())
            {
                recurso.LastVersion = ultimaVersion;
                recurso.Version = listaVersionDocumentosAux[recurso.Key];
            }
            ResourceModel modelo1 = listaRecursos.Values.First();
            ResourceModel modelo2 = listaRecursos.Values.Last();
            modelo1.PropertiesDifferences = CompararPropiedades(modelo1, modelo2);
            modelo2.PropertiesDifferences = modelo1.PropertiesDifferences;
            
            return PartialView("_comparator", listaRecursos.Values.ToList());
        }

        private Dictionary<string, bool> CompararPropiedades(ResourceModel pModelo1, ResourceModel pModelo2)
        {
            Dictionary<string, bool> resultado = new Dictionary<string, bool>();
            pModelo1.Link = pModelo1.Link ?? "";
            pModelo2.Link = pModelo2.Link ?? "";
            resultado.Add("title", !pModelo1.Title.Equals(pModelo2.Title));
            resultado.Add("description", !pModelo1.Description.Equals(pModelo2.Description));
            resultado.Add("tags", !pModelo1.Tags.OrderBy(i => i).SequenceEqual(pModelo2.Tags.OrderBy(i => i)));
            resultado.Add("categories", !pModelo1.Categories.Select(i => i.Key).SequenceEqual(pModelo2.Categories.Select(i => i.Key)));
            resultado.Add("image", !pModelo1.UrlPreview.Equals(pModelo2.UrlPreview));
            resultado.Add("private", pModelo1.Private != pModelo2.Private);
            resultado.Add("link", !pModelo1.Link.Equals(pModelo2.Link));
            if(pModelo1.TypeDocument == ResourceModel.DocumentType.Encuesta)
            {
                List<PollOptionsModel> pModelo1Opciones = pModelo1.Poll.PollOptions;
                List<PollOptionsModel> pModelo2Opciones = pModelo2.Poll.PollOptions;
                resultado.Add("pollOptions", !pModelo1Opciones.Select(i => i.Name).SequenceEqual(pModelo2Opciones.Select(i => i.Name)));
            }
            return resultado;
        }
    }
}
