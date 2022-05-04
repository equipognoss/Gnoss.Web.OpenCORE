using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class OntologiaController : ControllerBaseWeb
    {
        public OntologiaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        //
        // GET: /Ontologia/

        public ActionResult DescargarOntologia()
        {
            bool existeOntologia = false;

            string nombreOntologia = RequestParams("ontologia");

            if (!string.IsNullOrEmpty(nombreOntologia))
            {
                byte[] ontologia = null;

                try
                {
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Guid ontologiaID = docCN.ObtenerOntologiaAPartirNombre(Guid.Empty, nombreOntologia);

                    if (!ontologiaID.Equals(Guid.Empty))
                    {
                        CallFileService servicioArchivos = new CallFileService(mConfigService);

                        ontologia = servicioArchivos.ObtenerOntologiaBytes(ontologiaID);

                        if (ontologia != null && ontologia.Length > 0)
                        {
                            existeOntologia = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                }

                if (existeOntologia && ontologia != null)
                {
                    //DescargarArchivo(nombreOntologia, ontologia);
                    return new FileContentResult(ontologia, "application/octet-stream");
                }
            }

            return RedireccionarAPaginaNoEncontrada();
        }

    }
}
