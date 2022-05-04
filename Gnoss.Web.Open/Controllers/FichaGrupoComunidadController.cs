using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{

    public class FichaGrupoComunidadController : ControllerBaseWeb
    {
        #region Miembros

        GestionIdentidades gestorIdentidades = null;

        private GrupoIdentidades mGrupo = null;

        private bool? mEsGrupoOrganizacion = null;

        private List<Guid> mListaIdentidadesGrupo = new List<Guid>();

        private string UrlGrupo;

        #endregion

        public FichaGrupoComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Métodos

        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index()
        {

            if (!string.IsNullOrEmpty(RequestParams("callback")))
            {
                if (mControladorBase.UsuarioActual.EsIdentidadInvitada || Grupo == null)
                {
                    return new EmptyResult();
                }
                else if (RequestParams("callback").ToLower() == "Accion_EnviarMensaje".ToLower())
                {
                    string asunto = RequestParams("asunto");
                    string mensaje = HttpUtility.UrlDecode(RequestParams("mensaje"));
                    EnviarMensajeGrupo(asunto, mensaje);
                    try
                    {
                        return GnossResultOK();
                    }
                    catch (Exception ex)
                    {
                        // Guardar Log
                        GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);

                        return GnossResultERROR();
                    }
                }
                else if (RequestParams("callback").ToLower() == "Accion_EnviarMensajeTutor".ToLower())
                {
                    string asunto = RequestParams("asunto");
                    string mensaje = HttpUtility.UrlDecode(RequestParams("mensaje"));
                    string devolver = EnviarMensajeGrupoTutor(asunto, mensaje);
                    try
                    {
                        if (!string.IsNullOrEmpty(devolver))
                        {
                            return GnossResultERROR(devolver);
                        }
                        else
                        {
                            return GnossResultOK();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Guardar Log
                        GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);

                        return GnossResultERROR();
                    }
                }
            }

            if(Grupo == null)
            {
                Response.Redirect(mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "", ""));
            }

            GroupPageViewModel modeloPagina = new GroupPageViewModel();
            
            modeloPagina.Group = new GroupCardModel();
            modeloPagina.Group.Clave = Grupo.Clave;
            if (EsGrupoOrganizacion)
            {
                modeloPagina.Group.GroupType = GroupCardModel.GroupTypes.Organization;

                EliminarPersonalizacionVistas();
            }
            else
            {
                modeloPagina.Group.GroupType = GroupCardModel.GroupTypes.Community;
            }
            modeloPagina.Group.Name = Grupo.Nombre;
            modeloPagina.Group.Description = Grupo.Descripcion;
            modeloPagina.Group.ShortName = Grupo.NombreCorto;
            modeloPagina.Group.UrlGroup = UrlGrupo;
            modeloPagina.Group.UrlSearch = mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, "");
            modeloPagina.Group.Tags = Grupo.ListaTagsSoloLectura;

            int numMembers = 0;
            modeloPagina.ListMembers = CargarParticipantes(1, "", out numMembers);
            modeloPagina.NumMembers = numMembers;
            modeloPagina.NumPage = 1;

            modeloPagina.Actions = new GroupPageViewModel.ActionsModel();
            modeloPagina.Actions.Edit = false;
            modeloPagina.Actions.Delete = false;
            modeloPagina.Actions.RemoveMember = false;
            modeloPagina.Actions.RequestAccess = false;
            modeloPagina.Actions.LeaveGroup = false;
            modeloPagina.Actions.AsignarAGrupo = false;
            modeloPagina.Actions.AddMember = false;

            if (!mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                if ((Grupo.FilaGrupoOrganizacion != null && EsIdentidadActualAdministradorOrganizacion) || (Grupo.EsGrupoDeProyecto && (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) || (ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))))
                {
                    modeloPagina.Actions.AsignarAGrupo = true;
                    modeloPagina.Actions.AddMember = true;
                    modeloPagina.Actions.Edit = true;
                    modeloPagina.Actions.Delete = true;
                    modeloPagina.Actions.RemoveMember = true;
                }
                else if (Grupo.EsGrupoDeProyecto)
                {
                    if (ListaIdentidadesGrupo.Contains(IdentidadActual.Clave))
                    {
                        modeloPagina.Actions.LeaveGroup = true;
                    }
                    else
                    {
                        SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        if (!solicitudCN.TieneSolicitudPendienteDeIdentidadEnGrupo(IdentidadActual.Clave, Grupo.Clave))
                        {
                            modeloPagina.Actions.RequestAccess = true;
                        }
                        solicitudCN.Dispose();
                    }
                }

                if (((Grupo.FilaGrupoProyecto != null && (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) || (ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto) || ListaIdentidadesGrupo.Contains(IdentidadActual.Clave))) || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado)
                    && ParametrosGeneralesRow.ComunidadGNOSS)
                {
                    if (Grupo.PermitirEnviarMensajes)
                    {
                        modeloPagina.Group.AllowSendMessage = true;
                    }
                }
            }

            modeloPagina.UrlLoadMore = UrlGrupo + "/load-more";
            modeloPagina.UrlRequestAccess = UrlGrupo + "/request-access";
            modeloPagina.UrlLeaveGroup = UrlGrupo + "/leave-group";
            modeloPagina.UrlEditGroup = UrlGrupo + "/" + UtilIdiomas.GetText("URLSEM", "EDITARGRUPO");
            modeloPagina.UrlDeleteGroup = UrlGrupo + "/delete";
            modeloPagina.UrlAddMember = UrlGrupo + "/add-members";
            modeloPagina.UrlRemoveMember = UrlGrupo + "/remove-member";

            return View(modeloPagina);
        }

        public ActionResult LeaveGroup()
        {
            if (Grupo != null)
            {
                if (ListaIdentidadesGrupo.Contains(IdentidadActual.Clave))
                {
                    EliminarIdentidadDeGrupo(IdentidadActual.Clave);
                    string urlRedirect = "";
                    if (EsGrupoOrganizacion)
                    {
                        urlRedirect = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "ADMINISTRACION") + "/" + UtilIdiomas.GetText("URLSEM", "GRUPOS");
                    }
                    else
                    {
                        if (Grupo.EsPublico)
                        {
                            urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + Grupo.NombreCorto;
                        }
                        else
                        {
                            urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "", "");
                        }
                    }
                    
                    StringBuilder sb = new StringBuilder();
                    sb.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + Grupo.Clave.ToString().ToUpper() + ">", "<http://gnoss/hasparticipanteID>", "<http://gnoss/" + IdentidadActual.Clave.ToString().ToUpper() + ">"));
                    sb.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + Grupo.Clave.ToString().ToUpper() + ">", "<http://gnoss/hasparticipante>", "\"" + IdentidadActual.NombreCompuesto(IdentidadActual.Clave).ToLower() + "\""));

                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCN.BorrarGrupoTripletas(ProyectoSeleccionado.Clave.ToString(), sb.ToString(), false);
                    facetadoCN.Dispose();

                    FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCL.InvalidarCacheQueContengaCadena(NombresCL.PRIMEROSRECURSOS + "_" + ProyectoSeleccionado.Clave.ToString() + "_" + IdentidadActual.PerfilID);
                    facetadoCL.Dispose();

                    return GnossResultUrl(urlRedirect);
                }
            }
            return GnossResultERROR();
        }

        public ActionResult RequestAccessGroup()
        {
            if (Grupo != null)
            {
                if (!ListaIdentidadesGrupo.Contains(IdentidadActual.Clave))
                {
                    SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperSolicitud solicitudDW = solicitudCN.ObtenerSolicitudDeIdentidadEnGrupo(IdentidadActual.Clave, Grupo.Clave);

                    if (solicitudDW.ListaSolicitud.Count > 0)
                    {
                        solicitudDW.ListaSolicitud[0].Estado = (short)EstadoSolicitud.Espera;
                        solicitudDW.ListaSolicitud[0].FechaSolicitud = DateTime.Now;
                    }
                    else
                    {
                        Solicitud filaSolicitud = new Solicitud();
                        filaSolicitud.SolicitudID = Guid.NewGuid();
                        filaSolicitud.Estado = (short)EstadoSolicitud.Espera;
                        filaSolicitud.FechaSolicitud = DateTime.Now;
                        filaSolicitud.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        filaSolicitud.ProyectoID = ProyectoSeleccionado.Clave;
                        solicitudDW.ListaSolicitud.Add(filaSolicitud);
                        mEntityContext.Solicitud.Add(filaSolicitud);

                        SolicitudGrupo filaSolicitudGrupo = new SolicitudGrupo();
                        filaSolicitudGrupo.SolicitudID = filaSolicitud.SolicitudID;
                        filaSolicitudGrupo.GrupoID = Grupo.Clave;
                        filaSolicitudGrupo.IdentidadID = IdentidadActual.Clave;
                        solicitudDW.ListaSolicitudGrupo.Add(filaSolicitudGrupo);
                        mEntityContext.SolicitudGrupo.Add(filaSolicitudGrupo);
                    }

                    solicitudCN.ActualizarBD();
                    solicitudCN.Dispose();

                    return GnossResultOK();
                }
            }

            return GnossResultERROR();
        }

        public ActionResult LoadMoreMembers(string Search, int NumPage)
        {
            if (Grupo != null)
            {
                GroupPageViewModel modeloPagina = new GroupPageViewModel();

                modeloPagina.Group = new GroupCardModel();
                modeloPagina.Group.Clave = Grupo.Clave;
                if (EsGrupoOrganizacion)
                {
                    modeloPagina.Group.GroupType = GroupCardModel.GroupTypes.Organization;
                }
                else
                {
                    modeloPagina.Group.GroupType = GroupCardModel.GroupTypes.Community;
                }
                modeloPagina.Group.Name = Grupo.Nombre;
                modeloPagina.Group.UrlGroup = UrlGrupo;
                modeloPagina.Group.UrlSearch = mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, "");
                modeloPagina.Group.Tags = Grupo.ListaTagsSoloLectura;
                
                int numMembers = 0;
                modeloPagina.ListMembers = CargarParticipantes(NumPage, Search, out numMembers);
                modeloPagina.NumMembers = numMembers;
                modeloPagina.NumPage = NumPage;

                modeloPagina.UrlLoadMore = UrlGrupo + "/load-more";
                modeloPagina.UrlRequestAccess = UrlGrupo + "/request-access";
                modeloPagina.UrlLeaveGroup = UrlGrupo + "/leave-group";
                modeloPagina.UrlEditGroup = UrlGrupo + "/" + UtilIdiomas.GetText("URLSEM", "EDITARGRUPO");
                modeloPagina.UrlDeleteGroup = UrlGrupo + "/delete";
                modeloPagina.UrlAddMember = UrlGrupo + "/add-members";
                modeloPagina.UrlRemoveMember = UrlGrupo + "/remove-member";

                //return PartialView("_ContenedorPerfiles", modeloPagina);                
                return PartialView("_ContenedorPerfiles", modeloPagina);

            }

            return new EmptyResult();
        }

        public ActionResult DeleteGroup()
        {
            if (Grupo != null)
            {
                if ((Grupo.FilaGrupoOrganizacion != null && EsIdentidadActualAdministradorOrganizacion) || (Grupo.EsGrupoDeProyecto && (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) || (ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))))
                {
                    EliminarGrupo();

                    string urlRedirect = "";

                    if (EsGrupoOrganizacion)
                    {
                        urlRedirect = GnossUrlsSemanticas.GetUrlAdministrarOrg(BaseURLIdioma, UtilIdiomas, UrlPerfil, UtilIdiomas.GetText("URLSEM", "GRUPOS"));
                    }
                    else
                    {
                        urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "", "");
                    }

                    return GnossResultUrl(urlRedirect);
                }
            }
            return GnossResultERROR();
        }

        public ActionResult AddMembers(string[] Members)
        {
            if (Grupo != null)
            {
                bool puedeAgregarUsuarioAGrupo = false;

                if (Grupo.FilaGrupoProyecto != null)
                {
                    puedeAgregarUsuarioAGrupo = ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) || (ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto);
                }
                else if (Grupo.FilaGrupoOrganizacion != null)
                {
                    puedeAgregarUsuarioAGrupo = EsIdentidadActualAdministradorOrganizacion || (ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto);
                }

                //if (EsIdentidadActualAdministradorOrganizacionProyectoActual || (ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))
                if (puedeAgregarUsuarioAGrupo)
                {

                    List<Guid> listaPerfiles = new List<Guid>();
                    foreach (string participante in Members)
                    {
                        Guid perfilID = Guid.Empty;
                        if (Guid.TryParse(participante, out perfilID))
                        {
                            listaPerfiles.Add(perfilID);
                        }
                    }

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> listaIdentidades = identidadCN.ObtenerIdentidadesIDDePerfilEnProyecto(ProyectoSeleccionado.Clave, listaPerfiles);

                    if (listaIdentidades.Count > 0)
                    {
                        AgregarParticipantes(listaIdentidades);
                    }
                    
                    Dictionary<Guid, ProfileModel> listaIdentidadesPublicadores = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdentidades);

                    MultiViewResult result = new MultiViewResult(this, mViewEngine);

                    int i = 0;
                    foreach (ProfileModel fichaPerfil in listaIdentidadesPublicadores.Values)
                    {
                        result.AddView("_FichaPerfilGrupo", "fichaPerfil_" + i, fichaPerfil);
                        i++;
                    }

                    return result;
                }
            }
            return GnossResultERROR();
        }

        public ActionResult RemoveMember(Guid IdentidadID)
        {
            if (Grupo != null)
            {
                if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) || (ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))
                {
                    EliminarIdentidadDeGrupo(IdentidadID);

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestionIdentidades gestIdenti = new GestionIdentidades(identidadCN.ObtenerIdentidadPorIDCargaLigeraTablas(IdentidadID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCN.Dispose();

                    Identidad identidad = gestIdenti.ListaIdentidades[IdentidadID];

                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCN.BorrarParticipanteDeGrupo(Grupo.Clave, identidad.Clave, identidad.NombreCompuesto(IdentidadID), ProyectoSeleccionado.Clave);
                    facetadoCN.Dispose();

                    FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCL.InvalidarCacheQueContengaCadena(NombresCL.PRIMEROSRECURSOS + "_" + ProyectoSeleccionado.Clave.ToString() + "_" + identidad.PerfilID);
                    facetadoCL.Dispose();

                    return GnossResultOK();
                }
            }
            return GnossResultERROR();
        }

        private void AgregarParticipantes(List<Guid> pListaParticipantes)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> listaNombresParticipantes = identidadCN.ObtenerNombresDeIdentidades(pListaParticipantes);

            Dictionary<Guid, Guid> listaPerfilesIDporIdentidadID = identidadCN.ObtenerPerfilesIDDeIdentidadesID(pListaParticipantes);

            StringBuilder sb = new StringBuilder();

            List <Guid> listaNuevasIdentidades = new List<Guid>();
            foreach (Guid identidad in pListaParticipantes)
            {
                if (!Grupo.Participantes.ContainsKey(identidad))
                {
                    AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();

                    filaGrupoIdentidadesParticipacion.GrupoID = Grupo.Clave;
                    filaGrupoIdentidadesParticipacion.IdentidadID = identidad;
                    filaGrupoIdentidadesParticipacion.FechaAlta = DateTime.Now;

                    Grupo.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
                    mEntityContext.GrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
                    sb.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + Grupo.Clave.ToString().ToUpper() + ">", "<http://gnoss/hasparticipanteID>", "<http://gnoss/" + identidad.ToString().ToUpper() + ">"));
                    //sb.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + Grupo.Clave.ToString().ToUpper() + ">", "<http://gnoss/hasparticipante>", "\"" + listaNombresParticipantes[identidad].ToLower() + "\""));

                    listaNuevasIdentidades.Add(identidad);
                }
            }

            identidadCN.ActualizaIdentidades();

            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCN.InsertaTripletas(ProyectoSeleccionado.Clave.ToString(), sb.ToString(), 0, false);
            facetadoCN.Dispose();

            //Notificamos a los usuarios de que han sido agregados al grupo.
            EnviarMensajeMiembros(listaNuevasIdentidades, Grupo.Nombre, Grupo.NombreCorto, false);

            List<Guid> perfilesDeIdentidadesNuevas = identidadCN.ObtenerPerfilesDeIdentidades(listaNuevasIdentidades);

            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            foreach (Guid perfilID in perfilesDeIdentidadesNuevas)
            {
                facetadoCL.InvalidarCacheQueContengaCadena(NombresCL.PRIMEROSRECURSOS + "_" + ProyectoSeleccionado.Clave.ToString() + "_" + perfilID);
            }

            identidadCN.Dispose();

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (EsGrupoOrganizacion)
            {
                identidadCL.InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(IdentidadActual.OrganizacionID.Value);
                identidadCL.InvalidarCacheGrupoPorNombreCortoYOrganizacion(Grupo.NombreCorto, IdentidadActual.OrganizacionID.Value);
            }
            else
            {
                identidadCL.InvalidarCacheMiembrosComunidad(ProyectoSeleccionado.Clave);

                identidadCL.InvalidarCacheGrupoPorNombreCortoYProyecto(Grupo.NombreCorto, ProyectoSeleccionado.Clave);

                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                docCL.InvalidarPerfilesConRecursosPrivados(ProyectoSeleccionado.Clave);
                facetadoCL.InvalidarCachePerfilTieneGrupos(ProyectoSeleccionado.Clave, IdentidadActual.PerfilID);

                foreach (Guid identidadID in listaPerfilesIDporIdentidadID.Keys)
                {
                    facetadoCL.InvalidarCachePerfilTieneGrupos(ProyectoSeleccionado.Clave, listaPerfilesIDporIdentidadID[identidadID]);
                }

                BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (ParametrosGeneralesRow.PreguntasDisponibles)
                {
                    try
                    {
                        baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Preguntas, null);
                    }
                    catch(Exception ex)
                    {
                        GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                        baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Preguntas);
                    }                    
                }
                if (ParametrosGeneralesRow.DebatesDisponibles)
                {
                    try
                    {
                        baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Debates, null);
                    }
                    catch(Exception ex)
                    {
                        GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                        baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Debates);
                    }
                }
                if (ParametrosGeneralesRow.EncuestasDisponibles)
                {
                    try
                    {
                        baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Encuestas, null);
                    }
                    catch(Exception ex)
                    {
                        GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                        baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Encuestas);
                    }
                }

                try
                {
                    baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, null);
                }
                catch(Exception ex)
                {
                    GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                    baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos);
                }
                

                foreach (List<string> ontologias in InformacionOntologias.Values)
                {
                    foreach (string ns in ontologias)
                    {
                        try
                        {
                            baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, $"rdf:type={ns}");
                        }
                        catch(Exception ex)
                        {
                            GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                            baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, $"rdf:type={ns}");
                        }
                        
                    }
                }

                baseComunidadCN.Dispose();
            }
            identidadCL.Dispose();

            mGrupo = null;
        }

        private List<ProfileModel> CargarParticipantes(int pNumPeticion, string pNombreFiltro, out int numElementosFiltrados)
        {
            int numElementosMostrar = 10;
            int numItem = 1;

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> dicNombresIdentidades = identCN.ObtenerNombresDeIdentidades(ListaIdentidadesGrupo);
            identCN.Dispose();
            
            List<Guid> listaIdentidadesCargar = new List<Guid>();
            if(pNombreFiltro == null)
            {
                pNombreFiltro = "";
            }
            else
            {
                pNombreFiltro = LimpiarTexto(pNombreFiltro);
            }
            // Buscar con tildes y sin tildes
            var dicNombresIdentidadesAux = dicNombresIdentidades.Where(identidad => identidad.Value.ToLower().Contains(pNombreFiltro.ToLower()));
            numElementosFiltrados = dicNombresIdentidadesAux.Count();

            foreach (KeyValuePair<Guid, string> ident in dicNombresIdentidadesAux.OrderBy(identidad => identidad.Value).ToList())
            {
                if (numItem > ((pNumPeticion - 1) * numElementosMostrar))
                {
                    if (listaIdentidadesCargar.Count == numElementosMostrar) { break; }
                    listaIdentidadesCargar.Add(ident.Key);
                }
                numItem++;
            }
            
            Dictionary<Guid, ProfileModel> listaIdentidadesPublicadores = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdentidadesCargar);

            return listaIdentidadesPublicadores.Values.ToList();
        }
        private string LimpiarTexto(string pCadena)
        {
            byte[] tempBytes;
            tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(pCadena);
            return Encoding.UTF8.GetString(tempBytes).ToLower();
        }

        private void EnviarMensajeGrupo(string pAsunto, string pMensaje)
        {
            Guid autor = IdentidadActual.IdentidadMyGNOSS.Clave;

            GestionCorreo gestionCorreo = new GestionCorreo(new CorreoDS(), null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestionCorreo.IdentidadActual = IdentidadActual;
            gestionCorreo.GestorIdentidades = IdentidadActual.GestorIdentidades;

            Dictionary<Guid, bool> listaDestinatarios = new Dictionary<Guid, bool>();
            listaDestinatarios.Add(Grupo.Clave, true);

            // Cargamos las identidades del grupo
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestionCorreo.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesPorID(listaDestinatarios.Keys.ToList(), true));
            identCN.Dispose();

            Guid nuevoCorreo;
            if (!EsEcosistemaSinMetaProyecto)
            {
                nuevoCorreo = gestionCorreo.AgregarCorreo(autor, listaDestinatarios, pAsunto, pMensaje, BaseURLIdioma, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode, true);
            }
            else
            {
                nuevoCorreo = gestionCorreo.AgregarCorreo(autor, listaDestinatarios, pAsunto, pMensaje, BaseURLIdioma, ProyectoVirtual, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode, true);
            }

            string destinatarios = "";
            foreach (Guid destinatario in listaDestinatarios.Keys)
            {
                if (listaDestinatarios[destinatario])
                {
                    destinatarios += "|" + "g_" + destinatario.ToString();
                }
                else
                {
                    destinatarios += "|" + destinatario.ToString();
                }
            }
            if (destinatarios.StartsWith("|"))
            {
                destinatarios = destinatarios.Substring(1);
            }

            //Añadimos una línea al base para que la procese:
            ControladorDocumentacion.AgregarMensajeFacModeloBaseSimple(nuevoCorreo, autor, ProyectoAD.MyGnoss, "base", destinatarios, null, PrioridadBase.Alta);

            //ControladorCorreo.AgregarNotificacionCorreoNuevoAIdentidades(listaDestinatarios);
        }

        private string EnviarMensajeGrupoTutor(string pAsunto, string pMensaje)
        {
            Guid autor = IdentidadActual.IdentidadMyGNOSS.Clave;

            GestionCorreo gestionCorreo = new GestionCorreo(new CorreoDS(), null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestionCorreo.IdentidadActual = IdentidadActual;
            gestionCorreo.GestorIdentidades = IdentidadActual.GestorIdentidades;

            Dictionary<Guid, bool> listaDestinatarios = new Dictionary<Guid, bool>();
            foreach (Guid identidadID in ListaIdentidadesGrupo)
            {
                if (!identidadID.Equals(IdentidadActual.FilaIdentidad.PerfilID))
                {
                    listaDestinatarios.Add(identidadID, false);
                }
            }


            // Cargamos las identidades del grupo
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestionCorreo.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesPorID(listaDestinatarios.Keys.ToList(), true));

            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();
            DataWrapperPersona personaDS = new DataWrapperPersona();
            ObtenerIdentidadesPorID(ListaIdentidadesGrupo, identidadDW, personaDS, null);

            gestionCorreo.GestorIdentidades.GestorPersonas.DataWrapperPersonas.Merge(personaDS);
            gestionCorreo.GestorIdentidades.RecargarHijos();
            gestionCorreo.GestorIdentidades.GestorPersonas.RecargarPersonas();



            identCN.Dispose();

            Guid nuevoCorreo;
            if (!EsEcosistemaSinMetaProyecto)
            {
                nuevoCorreo = gestionCorreo.AgregarCorreoTutorGrupos(autor, listaDestinatarios, pAsunto, pMensaje, BaseURLIdioma, ProyectoSeleccionado, TiposNotificacion.MensajeTutor, UtilIdiomas.LanguageCode, true);
            }
            else
            {
                nuevoCorreo = gestionCorreo.AgregarCorreoTutorGrupos(autor, listaDestinatarios, pAsunto, pMensaje, BaseURLIdioma, ProyectoVirtual, TiposNotificacion.MensajeTutor, UtilIdiomas.LanguageCode, true);
            }

            
            return GetListaPersonasSinTutor(gestionCorreo, listaDestinatarios);
        }

        private string GetListaPersonasSinTutor(GestionCorreo gestionCorreo, Dictionary<Guid, bool> listaDestinatarios)
        {
            string devolver = "";
            foreach (Guid PersonaID in listaDestinatarios.Keys)
            {
                if (gestionCorreo.GestorIdentidades.ListaIdentidades.ContainsKey(PersonaID))
                {
                    Identidad identidadDestinatario = gestionCorreo.GestorIdentidades.ListaIdentidades[PersonaID];

                    if (string.IsNullOrEmpty(identidadDestinatario.Persona.FilaPersona.EmailTutor))
                    {
                        devolver = $"{devolver} No es posible enviar un mensaje a padres del usuario {identidadDestinatario.Persona.FilaPersona.Nombre}  {identidadDestinatario.Persona.FilaPersona.Apellidos} con correo {identidadDestinatario.Persona.FilaPersona.Email} porque no ha completado dicho campo.\n";
                    }
                }
            }
            return devolver;
        }

        private void EliminarGrupo()
        {
            ControladorGrupos.EliminarGrupoComunidad(Grupo.NombreCorto, ProyectoSeleccionado.Clave, Grupo.GestorIdentidades, ParametrosGeneralesRow, UrlIntragnoss, EsGrupoOrganizacion, InformacionOntologias, IdentidadActual.OrganizacionID);
        }

        private void EliminarIdentidadDeGrupo(Guid pIdentidadID)
        {
            if (Grupo.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Where(item => item.GrupoID.Equals(Grupo.Clave) && item.IdentidadID.Equals(pIdentidadID)).Any())
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion grupoIdentidadesParticipacion = Grupo.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Where(item => item.GrupoID.Equals(Grupo.Clave) && item.IdentidadID.Equals(pIdentidadID)).FirstOrDefault();

                mEntityContext.EliminarElemento(grupoIdentidadesParticipacion);
                Grupo.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Remove(grupoIdentidadesParticipacion);

                List<Guid> identidadEliminada = new List<Guid>();
                identidadEliminada.Add(pIdentidadID);

                //Notificamos a los usuarios de que han sido eliminados del grupo.
                EnviarMensajeMiembros(identidadEliminada, Grupo.Nombre, Grupo.NombreCorto, true);

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.ActualizaIdentidades();
                identidadCN.Dispose();

                StringBuilder sb = new StringBuilder();
                sb.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + Grupo.Clave.ToString().ToUpper() + ">", "<http://gnoss/hasparticipanteID>", "<http://gnoss/" + pIdentidadID.ToString().ToUpper() + ">"));
                //sb.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + Grupo.Clave.ToString().ToUpper() + ">", "<http://gnoss/hasparticipante>", "\"" + listaNombresParticipantes[identidad].ToLower() + "\""));

                FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                facetadoCN.BorrarGrupoTripletas(ProyectoSeleccionado.Clave.ToString(), sb.ToString(), false);
                facetadoCN.Dispose();

                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                if (EsGrupoOrganizacion)
                {
                    identidadCL.InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(IdentidadActual.OrganizacionID.Value);
                    identidadCL.InvalidarCacheGrupoPorNombreCortoYOrganizacion(Grupo.NombreCorto, IdentidadActual.OrganizacionID.Value);
                }
                else
                {
                    identidadCL.InvalidarCacheMiembrosComunidad(ProyectoSeleccionado.Clave);

                    identidadCL.InvalidarCacheGrupoPorNombreCortoYProyecto(Grupo.NombreCorto, ProyectoSeleccionado.Clave);

                    BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);

                    if (ParametrosGeneralesRow.PreguntasDisponibles)
                    {
                        try
                        {
                            baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Preguntas, null);
                        }
                        catch(Exception ex)
                        {
                            GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                            baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Preguntas);
                        }
                        
                    }
                    if (ParametrosGeneralesRow.DebatesDisponibles)
                    {
                        try
                        {
                            baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Debates, null);
                        }
                        catch(Exception ex)
                        {
                            GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                            baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Debates);
                        }
                    }
                    if (ParametrosGeneralesRow.EncuestasDisponibles)
                    {
                        try
                        {
                            baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Encuestas, null);
                        }
                        catch (Exception ex)
                        {
                            GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                            baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Encuestas);
                        }
                        
                    }

                    try
                    {
                        baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, null);
                    }
                    catch(Exception ex)
                    {
                        GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                        baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos);
                    }

                    foreach (List<string> ontologias in InformacionOntologias.Values)
                    {
                        foreach (string ns in ontologias)
                        {
                            try
                            {
                                baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, $"rdf:type={ns}");
                            }
                            catch(Exception ex)
                            {
                                GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                                baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoSeleccionado.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, $"rdf:type={ns}");
                            }
                            
                        }
                    }

                    baseComunidadCN.Dispose();
                }
                identidadCL.Dispose();
            }
        }

        /// <summary>
        /// Envia un mensaje a todos los participantes del grupo que se han añadido
        /// </summary>
        private void EnviarMensajeMiembros(List<Guid> pListaIdentidades, string pNombreGrupo, string pNombreCortoGrupo, bool pEsMensajeEliminacion)
        {
            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();

            ObtenerIdentidadesPorID(pListaIdentidades, identidadDW, dataWrapperPersona, null);

            identidadDW.Merge(IdentidadActual.GestorIdentidades.DataWrapperIdentidad);

            GestionPersonas gestPers = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
            GestionIdentidades gestIdent = new GestionIdentidades(identidadDW, gestPers, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            GestionCorreo gestionCorreo = new GestionCorreo(new CorreoDS(), gestPers, gestIdent, IdentidadActual.GestorAmigos, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestionCorreo.GestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<Guid> listaTodosDestinatarios = new List<Guid>();

            foreach (Guid identidadID in pListaIdentidades)
            {
                Identidad ident = gestIdent.ListaIdentidades[identidadID];

                if (ident.Clave != IdentidadActual.Clave)
                {
                    UtilIdiomas utilIdiomasPersona = new UtilIdiomas(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.DirectorySeparatorChar + "languages", Array.Empty<string>(), ident.Persona.FilaPersona.Idioma, ProyectoSeleccionado.Clave, Guid.Empty, Guid.Empty, mLoggingService, mEntityContext, mConfigService);

                    string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);

                    string urlGrupo = urlComunidad + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + pNombreCortoGrupo;
                    if (EsGrupoOrganizacion)
                    {
                        urlGrupo = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "ADMINISTRACION") + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + pNombreCortoGrupo;
                    }

                    string asunto = "";
                    string mensaje = "";

                    if (pEsMensajeEliminacion)
                    {
                        asunto = utilIdiomasPersona.GetText("GRUPO", "ASUNTOELIMINACIONGRUPO", pNombreGrupo);
                        if (EsGrupoOrganizacion)
                        {
                            mensaje = utilIdiomasPersona.GetText("GRUPO", "MENSAJEELIMINACIONGRUPOORGANIZACION", ident.Persona.Nombre, pNombreGrupo, urlGrupo, IdentidadActual.OrganizacionPerfil.Nombre, urlComunidad);
                        }
                        else
                        {
                            mensaje = utilIdiomasPersona.GetText("GRUPO", "MENSAJEELIMINACIONGRUPOCOMUNIDAD", ident.Persona.Nombre, pNombreGrupo, urlGrupo, ProyectoSeleccionado.Nombre, urlComunidad);
                        }
                    }
                    else
                    {
                        asunto = utilIdiomasPersona.GetText("GRUPO", "ASUNTOASIGNACIONGRUPO", pNombreGrupo);

                        if (EsGrupoOrganizacion)
                        {
                            mensaje = utilIdiomasPersona.GetText("GRUPO", "MENSAJEASIGNACIONGRUPOORGANIZACION", ident.Persona.Nombre, pNombreGrupo, urlGrupo, IdentidadActual.OrganizacionPerfil.Nombre, urlComunidad);
                        }
                        else
                        {
                            mensaje = utilIdiomasPersona.GetText("GRUPO", "MENSAJEASIGNACIONGRUPOCOMUNIDAD", ident.Persona.Nombre, pNombreGrupo, urlGrupo, ProyectoSeleccionado.Nombre, urlComunidad);
                        }

                    }
                    //1 -> NombrePersona 2 -> Nombre del grupo 3 -> url del Grupo 4 -> Nombre de la comunidad 5 -> url de la comunidad

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                    List<Guid> lista = new List<Guid>();
                    lista.Add(identidadCN.ObtenerIdentidadIDDeMyGNOSSPorIdentidad(ident.Clave));
                    identidadCN.Dispose();

                    foreach (Guid destinatario in lista)
                    {
                        if (!listaTodosDestinatarios.Contains(destinatario))
                        {
                            listaTodosDestinatarios.Add(destinatario);
                        }
                    }

                    if (!EsEcosistemaSinMetaProyecto)
                    {
                        gestionCorreo.AgregarCorreo(IdentidadActual.Clave, lista, asunto, mensaje, BaseURL, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                    }
                    else
                    {
                        gestionCorreo.AgregarCorreo(IdentidadActual.Clave, lista, asunto, mensaje, BaseURL, ProyectoVirtual, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                    }
                }
            }
        }
        
        #endregion

        #region Propiedades

        private GrupoIdentidades Grupo
        {
            get 
            {
                if (mGrupo == null)
                {
                    if (!string.IsNullOrEmpty(RequestParams("grupo")))
                    {
                        string nombreCortoGrupo = HttpUtility.UrlDecode(RequestParams("grupo"));

                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                        if (EsGrupoOrganizacion)
                        {
                            Guid organizacionID = new Guid();
                            if (IdentidadActual.OrganizacionID.HasValue)
                            {
                                organizacionID = IdentidadActual.OrganizacionID.Value;
                                gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGrupoPorNombreCortoYOrganizacion(nombreCortoGrupo, organizacionID, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                            }
                            else if (RequestParams("nombreOrgRewrite") != null)
                            {
                                if (IdentidadActual.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaOrganizacion.Any(item => item.NombreCorto.Equals(RequestParams("nombreOrgRewrite"))))
                                {
                                    AD.EntityModel.Models.OrganizacionDS.Organizacion organizacion = IdentidadActual.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaOrganizacion.FirstOrDefault(item => item.NombreCorto.Equals(RequestParams("nombreOrgRewrite")));
                                    organizacionID = organizacion.OrganizacionID;
                                    gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGrupoPorNombreCortoYOrganizacion(nombreCortoGrupo, organizacionID, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                                }
                                else
                                {
                                    // Obtenemos el grupo a partir de su perfilID
                                    gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGrupoPorNombreCortoYPerfilID(nombreCortoGrupo, IdentidadActual.PerfilID, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                                }
                            }
                        }
                        else
                        {
                            gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGrupoPorNombreCortoYProyecto(nombreCortoGrupo, ProyectoSeleccionado.Clave, false, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                        }

                        if (gestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades.Count > 0)
                        {
                            Guid grupoID = gestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades[0].GrupoID;
                            mGrupo = gestorIdentidades.ListaGrupos[grupoID];

                            if (EsGrupoOrganizacion)
                            {
                                UrlGrupo = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "ADMINISTRACION") + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + mGrupo.NombreCorto;
                            }
                            else
                            {
                                UrlGrupo = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + mGrupo.NombreCorto;
                            }

                            foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion fila in gestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion)
                            {
                                mListaIdentidadesGrupo.Add(fila.IdentidadID);
                            }
                        }

                        identidadCN.Dispose();
                    }
                }
                return mGrupo;
            }
        }

        private bool EsGrupoOrganizacion
        {
            get {
                if (!mEsGrupoOrganizacion.HasValue)
                {
                    mEsGrupoOrganizacion = false;
                    if (!string.IsNullOrEmpty(RequestParams("grupoOrg")) && RequestParams("grupoOrg") == "true")
                    {
                        mEsGrupoOrganizacion = true;
                    }
                }
                return mEsGrupoOrganizacion.Value;
            }
        }

        private List<Guid> ListaIdentidadesGrupo
        {
            get
            {
                return mListaIdentidadesGrupo;
            }
        }
        

        #endregion

    }
}