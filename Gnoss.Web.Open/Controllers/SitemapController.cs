﻿using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
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
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Text.RegularExpressions;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class SitemapController : ControllerBaseWeb
    {
        private EntityContext mEntityContext;

        public SitemapController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
            mEntityContext = entityContext;
        }

        // GET: Sitemap
        public ActionResult Index()
        {
            string dominio = UtilDominios.ObtenerDominioUrl(RequestUrl, true);
            string nombreSitemap = RequestUrl.Substring(dominio.Length + 1);

            if (nombreSitemap.Equals("sitemap.xml"))
            {
                var sitemapIndex = mEntityContext.SitemapsIndex.FirstOrDefault(item => item.Dominio.Equals(dominio));
                if (sitemapIndex != null)
                {
                    return Content(sitemapIndex.Sitemap);
                }
            }

            if (Regex.Match(nombreSitemap, "^sitemap_\\d+.xml$").Success)
            {
                var sitemap = mEntityContext.Sitemaps.FirstOrDefault(item => item.Dominio.Equals(dominio) && item.SitemapIndexName.Equals(nombreSitemap));
                if (sitemap != null)
                {
                    return Content(sitemap.SitemapContent);
                }
            }

            return Content("");
        }
    }
}