using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
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
    public class BandejaSuscripcionesController : ControllerBaseWeb
    {

        public BandejaSuscripcionesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EsPaginaEdicion = true;
            base.OnActionExecuting(filterContext);
        }

        protected override void CargarTituloPagina()
        {
            TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "SUSCRIPCIONES");

            base.CargarTituloPagina();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            if (!string.IsNullOrEmpty(RequestParams("callback")))
            {
                if (RequestParams("callback").ToLower() == "ActividadReciente|MostrarMas".ToLower())
                {
                    int numPeticion = int.Parse(RequestParams("numPeticion"));

                    ActividadReciente actividadRecienteController = new ActividadReciente(true, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
                    return PartialView("ControlesMVC/_ActividadReciente", actividadRecienteController.ObtenerActividadReciente(numPeticion, 10, TipoActividadReciente.Suscripcion, null, false));
                }
                return new EmptyResult();
            }
            ActividadReciente actividadReciente = new ActividadReciente(true, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
            RecentActivity actividad = null;

            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            liveCN.ResetearContadorNuevasSuscripciones(IdentidadActual.PerfilID);
            liveCN.Dispose();

            //https://riamgnoss.atlassian.net/browse/CORE-3414 No hay un devmyunikemia
            actividad = actividadReciente.ObtenerActividadReciente(1, 10, TipoActividadReciente.Suscripcion, null, false);
            //if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            //{
            //    actividad = actividadReciente.ObtenerActividadReciente(1, 10, TipoActividadReciente.Suscripcion, null, false);
            //}
            //else
            //{
            //    actividad = actividadReciente.ObtenerActividadReciente(1, 10, TipoActividadReciente.SuscripcionProyecto, null, false);
            //}


            return View(actividad);
        }

        [HttpGet]
        public ActionResult Manage()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            List<Guid> listaIdsComunidades = new List<Guid>();
            List<Guid> listaUsuarios = new List<Guid>();
            List<Guid> listaIdentidades = new List<Guid>();
            List<Guid> listaIdsIdentidadesOrg = new List<Guid>();
            List<Guid> listaIdsBlogs = new List<Guid>();

            SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperSuscripcion suscDW = suscCN.ObtenerSuscripcionesDePerfil(IdentidadActual.FilaIdentidad.PerfilID, false);
            GestionSuscripcion gestorSuscripciones = new GestionSuscripcion(suscDW, mLoggingService, mEntityContext);
            suscCN.Dispose();

            foreach (Suscripcion susc in gestorSuscripciones.ListaSuscripciones.Values)
            {
                switch (susc.Tipo)
                {
                    case AD.Suscripcion.TipoSuscripciones.Comunidades:
                        listaIdsComunidades.Add(((AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto)susc.FilaRelacion).ProyectoID);
                        break;
                    case AD.Suscripcion.TipoSuscripciones.Personas:
                        Guid IdentidadID = Guid.Empty;
                        Guid UsuarioID = Guid.Empty;
                        if (susc.FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario)
                        {
                            UsuarioID = ((AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario)susc.FilaRelacion).UsuarioID;
                        }
                        else if (susc.FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto)
                        {
                            Guid identidadID = ((AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto)susc.FilaRelacion).IdentidadID;
                            listaIdentidades.Add(identidadID);
                        }
                        listaUsuarios.Add(UsuarioID);
                        
                        //listaUsuarios.Lista.Add(susc);
                        break;
                    case AD.Suscripcion.TipoSuscripciones.Organizacion:
                        Guid organizacionID = ((AD.EntityModel.Models.Suscripcion.SuscripcionTesauroOrganizacion)susc.FilaRelacion).OrganizacionID;

                        if (!listaIdsIdentidadesOrg.Contains(organizacionID))
                        {
                            listaIdsIdentidadesOrg.Add(organizacionID);
                        }
                        break;
                }
            }

            string urlBase = BaseURLIdioma + UrlPerfil;

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Guid> identidadesMyGnoss = identCN.ObtenerIdentidadesIDPorUsuariosIDEnProyecto(listaUsuarios, ProyectoAD.MetaProyecto).Values.ToList();
            identidadesMyGnoss.AddRange(identCN.ObtenerIdentidadesIDDeMyGNOSSPorIdentidades(listaIdentidades));
            ManageSuscriptionsViewModel paginaModel = new ManageSuscriptionsViewModel();

            paginaModel.Communities = ControladorProyectoMVC.ObtenerComunidadesPorID(listaIdsComunidades, urlBase + UtilIdiomas.GetText("URLSEM", "COMUNIDADES"));
            paginaModel.Profiles = ControladorProyectoMVC.ObtenerIdentidadesPorID(identidadesMyGnoss);

            paginaModel.Organizations = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdsIdentidadesOrg);

            return View(paginaModel);
        }
    }
}
