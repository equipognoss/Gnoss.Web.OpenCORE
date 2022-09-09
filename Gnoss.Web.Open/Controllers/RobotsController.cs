using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class RobotsController : ControllerBaseWeb
    {
        private EntityContext mEntityContext;

        public RobotsController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
            mEntityContext = entityContext;
        }

        // GET: Robot
        public ActionResult Index()
        {
            string dominio = UtilDominios.ObtenerDominioUrl(RequestUrl, true);
            var sitemapIndex = mEntityContext.SitemapsIndex.FirstOrDefault(item => item.Dominio.Equals(dominio));

            if (sitemapIndex != null)
            {
                return Content(sitemapIndex.Robots);
            }
            else
            {
                //AppDomain.CurrentDomain.BaseDirectory
                using (StreamReader streamReader = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}/robots.txt"))
                {
                    return Content(streamReader.ReadToEnd());
                }
            }
            return Content("");
        }
    }
}