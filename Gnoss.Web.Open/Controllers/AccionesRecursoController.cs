using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.ControlesMVC;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class AccionesRecursoController : ControllerBaseWeb
    {

        public AccionesRecursoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        public ActionResult Index()
        {
            if (ParametrosGeneralesRow.MostrarAccionesEnListados && !mControladorBase.EstaUsuarioBloqueadoProyecto)
            {
                string listaControlesActualizar = RequestParams("listaRecursos");

                string[] controles = listaControlesActualizar.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<Guid, List<Guid>> listaDocumentosProyecto = new Dictionary<Guid, List<Guid>>();
                List<Guid> listaProyectos = new List<Guid>();
                foreach (string control in controles)
                {
                    string[] args = control.Split('_');

                    Guid idDocumento = new Guid(args[1]);

                    Guid idProyecto = ProyectoSeleccionado.Clave;

                    if (args.Length > 4)
                    {
                        if (!Guid.TryParse(control.Split('_')[3], out idProyecto))
                        {
                            idProyecto = ProyectoSeleccionado.Clave;
                        }
                    }

                    if (!listaDocumentosProyecto.ContainsKey(idProyecto))
                    {
                        listaDocumentosProyecto.Add(idProyecto, new List<Guid>());
                        listaProyectos.Add(idProyecto);
                    }

                    if (!listaDocumentosProyecto[idProyecto].Contains(idDocumento))
                    {
                        listaDocumentosProyecto[idProyecto].Add(idDocumento);
                    }
                }

                MultiViewResult result = new MultiViewResult(this, mViewEngine);

                AccionesRecurso accionesRecurso = new AccionesRecurso(this, mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mHttpContextAccessor, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);
                accionesRecurso.CargarAccionesRecursos(result, listaDocumentosProyecto);

                return result;
            }
            return new EmptyResult();
        }
    }
}
