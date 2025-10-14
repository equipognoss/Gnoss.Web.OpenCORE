using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class AdminSolicitudGrupoController : ControllerBaseWeb
    {
        const int NUM_RESULTADOS_PAGINA = 10;
        private bool mEsGrupoOrganizacion = false;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        public AdminSolicitudGrupoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdminSolicitudGrupoController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarSolicitudesDeAccesoAGrupo } })]
		public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(RequestParams("grupoOrg")) && RequestParams("grupoOrg") == "true")
            {
                mEsGrupoOrganizacion = true;

                if (!IdentidadActual.TrabajaConOrganizacion || !EsIdentidadActualAdministradorOrganizacion)
                {
                    return GnossResultUrl(BaseURLIdioma + UrlPerfil + "home");
                }
            }
          

            SolicitudesGrupoViewModel paginaModel = CargarSolicitudes();

            return View(paginaModel);
        }

        private SolicitudesGrupoViewModel CargarSolicitudes()
        {
            SolicitudesGrupoViewModel paginaModel = new SolicitudesGrupoViewModel();

            SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SolicitudCN>(), mLoggerFactory);
            DataWrapperSolicitud solicitudDW = solicitudCN.ObtenerSolicitudesGrupoPorProyecto(ProyectoSeleccionado.Clave);
            solicitudCN.Dispose();

            List<SolicitudGrupo> filasSolicitudGrupo = solicitudDW.ListaSolicitudGrupo;

            if (GrupoFiltroID != Guid.Empty)
            {
                filasSolicitudGrupo = solicitudDW.ListaSolicitudGrupo.Where(item => item.GrupoID.Equals(GrupoFiltroID)).ToList();
            }
            
            List<Guid> listaIdentidades = new List<Guid>();
            List<Guid> listaGrupos = new List<Guid>();
            List<KeyValuePair<Guid, Guid>> listaIdentidadGrupo = new List<KeyValuePair<Guid, Guid>>();
            Dictionary<KeyValuePair<Guid, Guid>, DateTime> listaIdentidadFecha = new Dictionary<KeyValuePair<Guid, Guid>, DateTime>();

            int contador = 0;

            foreach (SolicitudGrupo fila in filasSolicitudGrupo)
            {
                if (!listaGrupos.Contains(fila.GrupoID))
                {
                    listaGrupos.Add(fila.GrupoID);
                }
            }

            foreach (SolicitudGrupo fila in filasSolicitudGrupo)
            {
                if (contador < ((Pagina - 1) * NUM_RESULTADOS_PAGINA))
                {
                    contador++;
                    continue;
                }
                if (listaIdentidadGrupo.Count == NUM_RESULTADOS_PAGINA)
                {
                    break;
                }

                if (!listaIdentidades.Contains(fila.IdentidadID))
                {
                    listaIdentidades.Add(fila.IdentidadID);
                }

                //Una identidad puede tener una solicitud a mas de un grupo
                KeyValuePair<Guid, Guid> identidadGrupo = new KeyValuePair<Guid, Guid>(fila.IdentidadID, fila.GrupoID);
                if (!listaIdentidadGrupo.Contains(identidadGrupo))
                {
                    listaIdentidadGrupo.Add(identidadGrupo);
                }

                if (!listaIdentidadFecha.ContainsKey(identidadGrupo))
                {
                    listaIdentidadFecha.Add(identidadGrupo, fila.Solicitud.FechaSolicitud);
                }
            }

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            Dictionary<Guid, string> urlFotos = identCN.ObtenerSiListaIdentidadesTienenFoto(listaIdentidades);
            Dictionary<Guid, string> nombres = identCN.ObtenerNombresDeIdentidades(listaIdentidades);
            Dictionary<Guid, string> grupos = identCN.ObtenerNombresDeGrupos(listaGrupos);
            identCN.Dispose();

            string urlPagina = Comunidad.Url + "/" + UtilIdiomas.GetText("URLSEM", "GRUPOS") + "/" + UtilIdiomas.GetText("URLSEM", "ADMINISTRARSOLICITUDES");

            paginaModel.UrlPagina = urlPagina;
            paginaModel.UrlAccept = urlPagina + "/accept-request";
            paginaModel.UrlReject = urlPagina + "/reject-request";
            paginaModel.Grupos = grupos;
            paginaModel.NumPaginas = (filasSolicitudGrupo.Count > 1 ? (filasSolicitudGrupo.Count - 1) / NUM_RESULTADOS_PAGINA : 0) + 1;
            paginaModel.PaginaActual = Pagina;
            paginaModel.GrupoFiltroID = GrupoFiltroID;

            List<SolicitudesGrupoViewModel.SolicitudModel> listaSolicitudes = new List<SolicitudesGrupoViewModel.SolicitudModel>();

            foreach (KeyValuePair<Guid, Guid> identidadGrupo in listaIdentidadGrupo)
            {
                SolicitudesGrupoViewModel.SolicitudModel solicitud = new SolicitudesGrupoViewModel.SolicitudModel();

                Guid identidadID = identidadGrupo.Key;
                Guid grupoID = identidadGrupo.Value;

                solicitud.KeyIdentity = identidadGrupo.Key;
                solicitud.KeyGroup = identidadGrupo.Value;
                solicitud.UrlFoto = urlFotos[identidadID];
                solicitud.NameIdentity = nombres[identidadID];
                solicitud.NameGroup = grupos[grupoID];
                solicitud.Date = listaIdentidadFecha[identidadGrupo];

                listaSolicitudes.Add(solicitud);
            }

            paginaModel.ListaSolicitudes = listaSolicitudes;
            return paginaModel;
        }

		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarSolicitudesDeAccesoAGrupo } })]
		public ActionResult AcceptRequest(Guid IdentidadID, Guid GrupoID)
        {
            if (!string.IsNullOrEmpty(RequestParams("grupoOrg")) && RequestParams("grupoOrg") == "true")
            {
                mEsGrupoOrganizacion = true;

                if (!IdentidadActual.TrabajaConOrganizacion || !EsIdentidadActualAdministradorOrganizacion)
                {
                    return GnossResultUrl(BaseURLIdioma + UrlPerfil + "home");
                }
            }
           

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();

            if (!identidadCN.ParticipaIdentidadEnGrupo(IdentidadID, GrupoID))
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();

                filaGrupoIdentidadesParticipacion.GrupoID = GrupoID;
                filaGrupoIdentidadesParticipacion.IdentidadID = IdentidadID;
                filaGrupoIdentidadesParticipacion.FechaAlta = DateTime.Now;

                identidadDW.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
                mEntityContext.GrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);

                identidadCN.ActualizaIdentidades();
            }

            SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SolicitudCN>(), mLoggerFactory);
            bool aceptada = solicitudCN.AceptarSolicitudDeIdentidadEnGrupoProyecto(IdentidadID, GrupoID);
            solicitudCN.Dispose();

            string nombrecorto = identidadCN.ObtenerNombreCortoGrupoPorID(GrupoID);
            identidadCN.Dispose();
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
            if (EsGrupoOrganizacion)
            {
                identidadCL.InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(IdentidadActual.OrganizacionID.Value);
                identidadCL.InvalidarCacheGrupoPorNombreCortoYOrganizacion(nombrecorto, IdentidadActual.OrganizacionID.Value);
            }
            else
            {
                identidadCL.InvalidarCacheMiembrosComunidad(ProyectoSeleccionado.Clave);

                identidadCL.InvalidarCacheGrupoPorNombreCortoYProyecto(nombrecorto, ProyectoSeleccionado.Clave);
            }
            identidadCL.Dispose();

            if (aceptada)
            {
                return GnossResultOK();
            }
            return GnossResultERROR();
        }

		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarSolicitudesDeAccesoAGrupo } })]
		public ActionResult RejectRequest(Guid IdentidadID, Guid GrupoID)
        {
            if (!string.IsNullOrEmpty(RequestParams("grupoOrg")) && RequestParams("grupoOrg") == "true")
            {
                mEsGrupoOrganizacion = true;

                if (!IdentidadActual.TrabajaConOrganizacion || !EsIdentidadActualAdministradorOrganizacion)
                {
                    return GnossResultUrl(BaseURLIdioma + UrlPerfil + "home");
                }
            }
            else
            {
                if (!ProyectoSeleccionado.EsAdministradorUsuario(IdentidadActual.Persona.UsuarioID) && !(ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))
                {
                    return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "", ""));
                }
            }

            SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SolicitudCN>(), mLoggerFactory);
            bool rechazada = solicitudCN.RechazarSolicitudDeIdentidadEnGrupoProyecto(IdentidadID, GrupoID);
            solicitudCN.Dispose();

            if (rechazada)
            {
                return GnossResultOK();
            }
            return GnossResultERROR();
        }

        private int Pagina
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("pagina")))
                {
                    return int.Parse(RequestParams("pagina"));
                }
                else
                {
                    return 1;
                }
            }
        }

        private Guid GrupoFiltroID
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("grupoID")))
                {
                    return Guid.Parse(RequestParams("grupoID"));
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        public bool EsGrupoOrganizacion
        {
            get { return mEsGrupoOrganizacion; }
            set { mEsGrupoOrganizacion = value; }
        }
    }
}
