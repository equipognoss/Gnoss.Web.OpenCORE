using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
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
    public class EditarBioComunidadController : ControllerBaseWeb
    {
        public EditarBioComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        [HttpPost]
        public ActionResult Index(Guid CvSelected)
        {
            IdentidadActual.FilaIdentidad.CurriculumID = CvSelected;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.ActualizaIdentidades();
            identidadCN.Dispose();

            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            //controladorPersonas.ActualizarModeloBaseSimple(mControladorBase.UsuarioActual.PersonaID, ProyectoSeleccionado.Clave, PrioridadBase.Alta);
            controladorPersonas.ActualizarModeloBaseSimple(IdentidadActual, ProyectoSeleccionado.Clave, UrlIntragnoss);

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCL.EliminarCacheGestorIdentidadActual(UsuarioActual.UsuarioID, UsuarioActual.IdentidadID, UsuarioActual.PersonaID);
            identidadCL.Dispose();

            EditCvCommunityModel paginaModel = new EditCvCommunityModel();

            paginaModel.CvList = new Dictionary<Guid, string>();

            paginaModel.CvSelected = CvSelected;

            return View(paginaModel);
        }

        public ActionResult Index()
        {
            EditCvCommunityModel paginaModel = new EditCvCommunityModel();

            paginaModel.CvList = new Dictionary<Guid, string>();

            return View(paginaModel);
        }
    }
}
