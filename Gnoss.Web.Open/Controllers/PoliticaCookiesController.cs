using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Cookie;
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class PoliticaCookiesController : ControllerBaseWeb
    {

        private List<CategoryCookieModel> ListCategoryCookieModel;

        public PoliticaCookiesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        protected override void CargarTituloPagina()
        {
            TituloPagina = UtilIdiomas.GetText("POLITICACOOKIES", "TITULO");

            base.CargarTituloPagina();
        }

        public ActionResult Index()
        {
            string descripcionPoliticaCookies = "";

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            DataWrapperUsuario usuarioDW = proyCL.ObtenerPoliticaCookiesProyecto(ProyectoSeleccionado.Clave);
            usuarioDW.Merge(proyCL.ObtenerPoliticaCookiesProyecto(ProyectoAD.MetaProyecto));
            proyCL.Dispose();

            if (usuarioDW.ListaClausulaRegistro.Count == 0)
            {
                //No hay clausulas
                return RedireccionarAPaginaNoEncontrada();
            }
            else
            {
                string texto = usuarioDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesUrlPagina) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave)).Select(item => item.Texto).FirstOrDefault();

                if (string.IsNullOrEmpty(texto))
                {
                    texto = usuarioDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesUrlPagina) && item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Texto).FirstOrDefault();
                }

                if (Uri.IsWellFormedUriString(texto, UriKind.Absolute))
                {
                    //Se trata de una URL
                    return RedireccionarAPaginaNoEncontrada();
                }
                else
                {
                    descripcionPoliticaCookies = UtilCadenas.ObtenerTextoDeIdioma(texto, IdiomaUsuario, null);
                }

                CargarCategoriasYCookies();

                ViewBag.ListCategoryCookieModel = ListCategoryCookieModel;
            }

            return View("Index", descripcionPoliticaCookies);
        }

        private void CargarCategoriasYCookies()
        {
            ListCategoryCookieModel = new List<CategoryCookieModel>();

            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<CategoriaProyectoCookie> listaCategorias = cookieCN.ObtenerCategoriasProyectoCookie(ProyectoSeleccionado.Clave);

            foreach (CategoriaProyectoCookie categoriaCookie in listaCategorias)
            {
                CategoryCookieModel categoryCookieModel = new CategoryCookieModel();
                categoryCookieModel.CategoriaProyectoCookie = categoriaCookie;
                categoryCookieModel.ListaCookies = cookieCN.ObtenerCookiesDeCategoria(categoriaCookie.CategoriaID);
            }

        }
        // Acción para devolver la vista que contendrá la Política de Cookies en un modal
        [HttpPost]
        public ActionResult LoadModal()
        {
            string descripcionPoliticaCookies = "";

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            DataWrapperUsuario usuarioDW = proyCL.ObtenerPoliticaCookiesProyecto(ProyectoSeleccionado.Clave);
            usuarioDW.Merge(proyCL.ObtenerPoliticaCookiesProyecto(ProyectoAD.MetaProyecto));

            proyCL.Dispose();

            if (usuarioDW.ListaClausulaRegistro.Count == 0)
            {
                //No hay clausulas
                return RedireccionarAPaginaNoEncontrada();
            }
            else
            {
                string texto = usuarioDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesUrlPagina) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave)).Select(item => item.Texto).FirstOrDefault();

                if (string.IsNullOrEmpty(texto))
                {
                    texto = usuarioDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesUrlPagina) && item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Texto).FirstOrDefault();
                }

                if (Uri.IsWellFormedUriString(texto, UriKind.Absolute))
                {
                    //Se trata de una URL
                    return RedireccionarAPaginaNoEncontrada();
                }
                else
                {
                    descripcionPoliticaCookies = UtilCadenas.ObtenerTextoDeIdioma(texto, IdiomaUsuario, null);
                }

                CargarCategoriasYCookies();

                ViewBag.ListCategoryCookieModel = ListCategoryCookieModel;
            }
            return GnossResultHtml("_pie/_modal-views/_cookies-policy", descripcionPoliticaCookies);
        }
    }
}
