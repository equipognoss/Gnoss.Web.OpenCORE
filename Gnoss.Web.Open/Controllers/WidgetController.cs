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
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class WidgetController : ControllerBaseWeb
    {
        public WidgetController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Comunidad = base.Comunidad;

            ViewBag.BaseUrl = BaseURL;
            ViewBag.BaseUrlIdioma = BaseURLIdioma;
            ViewBag.BaseUrlStatic = BaseURLStatic;
            ViewBag.BaseUrlContent = BaseURLContent;
            ViewBag.BaseUrlPersonalizacion = BaseURLPersonalizacion;
            ViewBag.BaseUrlPersonalizacionEcosistema = BaseURLPersonalizacionEcosistema;
            ViewBag.EsEcosistemaSinMetaProyecto = EsEcosistemaSinMetaProyecto;

            ViewBag.UtilIdiomas = UtilIdiomas;
        }

        public ActionResult Comunidad()
        {
            //Número de recursos que se mostrarán en la página
            int numeroRecursos = 10;

            if (Request.Headers.ContainsKey("numRecursos") && Request.Headers["numRecursos"] != "")
            {
                numeroRecursos = int.Parse(Request.Headers["numRecursos"]);
                if (numeroRecursos > 100)
                {
                    numeroRecursos = 100;
                }
            }

            string urlImagen = "";

            if (!string.IsNullOrEmpty(Request.Headers["logoCabecera"]))
            {
                int versionFotoImagenSupGrande = 0;
                if (!(ParametrosGeneralesRow.VersionFotoImagenSupGrande==null))
                {
                    versionFotoImagenSupGrande = (int)ParametrosGeneralesRow.VersionFotoImagenSupGrande;
                }

                urlImagen = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + ".png?" + versionFotoImagenSupGrande;
            }
            else
            {
                string nombreImagenPeque = ParametrosGeneralesRow.NombreImagenPeque;

                urlImagen = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesProyectos + "/" + nombreImagenPeque;

                if (nombreImagenPeque.Equals("peque"))
                {
                    urlImagen = BaseURLStatic + "/img" + "/" + UtilArchivos.ContentImgIconos + "/" + UtilArchivos.ContentImagenesProyectos + "/" + "anonimo_peque.png";
                }
                else if (!(ParametrosGeneralesRow.VersionFotoImagenMosaicoGrande==null))
                {
                    urlImagen += "?" + ParametrosGeneralesRow.VersionFotoImagenMosaicoGrande;
                }
            }

            string ontologia = null;

            if (!string.IsNullOrEmpty(Request.Headers["ontologia"]))
            {
                ontologia = Request.Headers["ontologia"];
            }

            ((CommunityModel)ViewBag.Comunidad).Logo = urlImagen;

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Guid> listaDocumentos = docCN.ObtenerUltimosRecursosIDPublicados(ProyectoSeleccionado.Clave, numeroRecursos, ontologia);
            docCN.Dispose();

            Dictionary<Guid, ResourceModel> listaRecursos = ControladorProyectoMVC.ObtenerRecursosPorID(listaDocumentos, "", null, false);

            return View("Comunidad", listaRecursos.Values.ToList());
        }

    }
}
