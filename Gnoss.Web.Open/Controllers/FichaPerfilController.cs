using DocumentFormat.OpenXml.Office2010.Excel;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Suscripcion;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.Organizador.Correo;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.Controles.Suscripcion;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Universal.Common.Extensions;
using static Es.Riam.Gnoss.Util.Seguridad.Capacidad;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class FichaPerfilController : ControllerBaseWeb
    {
        #region Miembros

        private Elementos.Identidad.Identidad mIdentidadPagina;

        /// <summary>
        /// Montar los recusos cuando es MyGnoss
        /// </summary>
        bool? mMostrarRecursos;

        private Suscripcion mSuscripcion;
        /// <summary>
        /// Ontologia
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// Información extra del Perfil
        /// </summary>
        private DataSet mInformacionExtraPerfil = null;

        /// <summary>
        /// Indica si la indentidad que se está buscando existe
        /// </summary>
        private bool? mExisteIdentidad;

        private readonly ProfilePageViewModel paginaModel = new ProfilePageViewModel();


        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        public FichaPerfilController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<FichaPerfilController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {

            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Métodos

        public ActionResult Index()
        {
            //Si intentas entrar en tu perfil pero estas desconectado, redirigimos a la home, funcionalidad obligatoria para IneveryCrea
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada && RequestParams("MyProfilePage") == "true")
            {
                return new RedirectResult(Comunidad.Url);
            }

            if (RequestParams("nombreCortoPerfil") != null && (IdentidadPagina == null || IdentidadPagina.Tipo == TiposIdentidad.ProfesionalCorporativo))
            {
                if (!string.IsNullOrEmpty(RequestParams("nombreCortoOrganizacion")))
                {
                    string nombreCortoOrg = RequestParams("nombreCortoOrganizacion");

                    string url = Request.Path.ToString();
                    int inicioPerfil = url.IndexOf(UtilIdiomas.GetText("URLSEM", "ORGANIZACION") + "/" + nombreCortoOrg) + 1;
                    url = url.Substring(0, inicioPerfil + (UtilIdiomas.GetText("URLSEM", "ORGANIZACION") + "/" + nombreCortoOrg).Length);
                    return new RedirectResult(url);
                }
            }

            if (EsEcosistemaSinMetaProyecto && ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                string nombreCortoProy = ProyectoVirtual.NombreCorto;
                if (!IdentidadPagina.ListaProyectosPerfilActual.ContainsKey(ProyectoVirtual.Clave))
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    nombreCortoProy = proyCN.ObtenerNombreCortoProyecto(IdentidadPagina.ListaProyectosPerfilActual.First(p => p.Key != ProyectoAD.MetaProyecto).Key);
                    proyCN.Dispose();
                }

                return Redirect(mControladorBase.UrlsSemanticas.GetURLPerfilDeIdentidad(BaseURL, nombreCortoProy, UtilIdiomas, IdentidadPagina));
            }

            if (!string.IsNullOrEmpty(RequestParams("callback")))
            {
                if (mControladorBase.UsuarioActual.EsIdentidadInvitada || IdentidadPagina == null)
                {
                    return new EmptyResult();
                }

                paginaModel.Profile = ControladorProyectoMVC.ObtenerIdentidadPorID(IdentidadPagina.Clave);

                if (RequestParams("callback").ToLower() == "ActividadReciente|MostrarMas".ToLower() || RequestParams("callback").ToLower() == "inicio".ToLower())
                {
                    int numPeticion = 0;
                    if (RequestParams("callback").ToLower() != "inicio".ToLower())
                    {
                        numPeticion = int.Parse(RequestParams("numPeticion"));
                    }

                    CargarActividadReciente(numPeticion);
                }
                else if (RequestParams("callback").ToLower() == "grupos".ToLower())
                {
                    CargarGrupos();
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Grups;
                }
                else if (RequestParams("callback").ToLower() == "recursos".ToLower())
                {
                    if (MostrarRecursos)
                    {
                        CargarPaginaRecursos();
                    }
                    else
                    {
                        return new EmptyResult();
                    }
                }
                else if (RequestParams("callback").ToLower() == "personas".ToLower())
                {
                    CargarPersonasPerfil();
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.People;
                }
                else if (RequestParams("callback").ToLower() == "contactos".ToLower())
                {
                    CargarContactosPerfil();
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Contacts;
                }
                else if (RequestParams("callback").ToLower() == "seguidores".ToLower())
                {
                    int numPeticion = 1;
                    int.TryParse(RequestParams("numPeticion"), out numPeticion);
                    CargarSeguidoresPerfil(numPeticion);
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Followers;

                    MultiViewResult result = new MultiViewResult(this, mViewEngine);

                    int i = 0;
                    foreach (ProfileModel fichaPerfil_ in paginaModel.ProfileFollowers)
                    {
                        result.AddView("ControlesMVC/_FichaPerfilMini", "fichaPerfil_" + i, fichaPerfil_);
                        i++;
                    }

                    return result;
                }
                else if (RequestParams("callback").ToLower() == "sigue-a".ToLower())
                {
                    int numPeticion = 1;
                    int.TryParse(RequestParams("numPeticion"), out numPeticion);
                    CargarSeguidosPerfil(numPeticion);
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Followed;

                    MultiViewResult result = new MultiViewResult(this, mViewEngine);

                    int i = 0;
                    foreach (ProfileModel fichaPerfil_ in paginaModel.ProfileFollowed)
                    {
                        result.AddView("ControlesMVC/_FichaPerfilMini", "fichaPerfil_" + i, fichaPerfil_);
                        i++;
                    }

                    return result;
                }
                else if (RequestParams("callback").ToLower() == "Accion_EnviarMensaje".ToLower())
                {
                    return AccionEnviarMensaje();
                }
                else if (RequestParams("callback").ToLower() == "Accion_EnviarMensajeTutor".ToLower())
                {
                    return AccionEnviarMensajeTutor();
                }
                else if (RequestParams("callback").ToLower() == "Accion_EliminarContacto".ToLower())
                {
                    EliminarContacto();
                }
                else if (RequestParams("callback").ToLower() == "Accion_AgregarContactoOrg".ToLower())
                {
                    AgregarContactoOrg();
                }
                else if (RequestParams("callback").ToLower() == "Accion_EliminarContactoOrg".ToLower())
                {
                    EliminarContactoOrg();
                }
                else if (RequestParams("callback").ToLower() == "Accion_EnviarCorreccion".ToLower())
                {
                    if (string.IsNullOrEmpty(RequestParams("EnviarCorreccionDefinitiva")))
                    {
                        EnviarMensajeDeCorreccionDeIdentidad();
                    }
                    else
                    {
                        EnviarMensajeDeCorreccionDeIdentidadDefinitivo();
                    }
                }
                else if (RequestParams("callback").ToLower() == "Accion_ValidarCorreccion".ToLower())
                {
                    ValidarCorreccionDeIdentidad();
                }
                else if (RequestParams("callback").ToLower() == "Accion_EliminarPersona".ToLower())
                {
                    EliminarPersona();
                }
                else if (RequestParams("callback").ToLower() == "CargarListaAccionesRecursos".ToLower())
                {
                    return new EmptyResult();
                }
                else if (RequestParams("callback").ToLower() == "accion_expulsar")
                {
                    ExpulsarUsuarioDeComunidad();
                }
                else if (RequestParams("callback").ToLower() == "accion_readmitir")
                {
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        ReadmitirUsuarioEnComunidad();
                    }
                    else
                    {
                        return new UnauthorizedResult();
                    }
                }
                else if (RequestParams("callback").ToLower() == "accion_bloquear")
                {
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        ModificarBloqueoUsuarioEnComunidad(true);
                    }
                    else
                    {
                        return new UnauthorizedResult();
                    }
                }
                else if (RequestParams("callback").ToLower() == "accion_desbloquear")
                {
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        ModificarBloqueoUsuarioEnComunidad(false);
                    }
                    else
                    {
                        return new UnauthorizedResult();
                    }
                }
                else if (RequestParams("callback").ToLower() == "accion_enviarnewsletter")
                {
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        ModificarEnviarNewsletterAUsuario(true);
                    }
                    else
                    {
                        return new UnauthorizedResult();
                    }
                }
                else if (RequestParams("callback").ToLower() == "accion_noenviarnewsletter")
                {
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        ModificarEnviarNewsletterAUsuario(false);
                    }
                    else
                    {
                        return new UnauthorizedResult();
                    }
                }
                else if (RequestParams("callback").ToLower() == "accion_cambiarrol")
                {
                    if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        if (!IdentidadPagina.Clave.Equals(IdentidadActual.Clave) || ProyectoSeleccionado.ListaAdministradoresIDs.Count > 1)
                        {
                            CambiarRolUsuario();
                        }
                        else
                        {
                            return new UnauthorizedResult();
                        }
                    }
                    else
                    {
                        return new UnauthorizedResult();
                    }
                }

                return PartialView("_ContenidoPagina", paginaModel);
            }

            ViewBag.BodyClass = " layout02  comGnoss";

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            bool perfilEliminado = identidadCN.EstaPerfilEliminado(IdentidadPagina.PerfilID);
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada && !perfilEliminado)
            {
                if (IdentidadPagina == null)
                {
                    paginaModel.Profile = new ProfileModel();

                    string nombrePersona = RequestParams("nombreCortoPerfil");
                    string nombreOrg = RequestParams("nombreCortoOrganizacion");

                    if (!string.IsNullOrEmpty(nombrePersona))
                    {
                        paginaModel.Profile.NamePerson = nombrePersona;
                    }
                    else if (!string.IsNullOrEmpty(nombreOrg))
                    {
                        paginaModel.Profile.NamePerson = nombreOrg;
                    }

                    paginaModel.Profile.NamePerson = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(paginaModel.Profile.NamePerson.Replace('-', ' ').ToLower());
                }
                else
                {
                    paginaModel.Profile = ControladorProyectoMVC.ObtenerIdentidadPorID(IdentidadPagina.Clave, true);
                }

                return View("_FichaInvitado", paginaModel);
            }

            if (IdentidadPagina == null || perfilEliminado)
            {
                return RedireccionarAPaginaNoEncontrada();
            }

            paginaModel.Profile = ControladorProyectoMVC.ObtenerIdentidadPorID(IdentidadPagina.Clave, true);
            paginaModel.MyProfilePage = !string.IsNullOrEmpty(RequestParams("MyProfilePage")) && RequestParams("MyProfilePage") == "true";

            if (IdentidadPagina.Persona != null)
            {
                paginaModel.UserKey = IdentidadPagina.Persona.UsuarioID;
            }

            paginaModel.ShowResources = MostrarRecursos;

            //ObtenerFichaIdentidadModelDeIdentidadPagina();
            if (ProyectoSeleccionado.Equals(ProyectoAD.MetaProyecto))
            {
                paginaModel.HasGrups = identidadCN.TieneIdentidadGrupos(ProyectoSeleccionado.Clave, IdentidadPagina.PerfilID);
            }
            else
            {
                paginaModel.HasGrups = identidadCN.TieneIdentidadGrupos(IdentidadPagina.Clave);
            }

            CargarRedesSocialesIdentidadPagina(paginaModel.Profile);

            CargarAccionesIdentidadPagina(paginaModel.Profile);
            //paginaModel.Profile.Actions.SendMessage = true;

            Guid identMensaje = IdentidadPagina.Clave;

            if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.ProfesionalCorporativo))
            {
                if (!IdentidadActual.OrganizacionID.HasValue || !IdentidadPagina.OrganizacionID.Equals(IdentidadActual.OrganizacionID))
                {
                    if (IdentidadPagina.IdentidadOrganizacion != null)
                    {
                        identMensaje = IdentidadPagina.IdentidadOrganizacion.Clave;
                    }
                    else
                    {
                        IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        identMensaje = identCN.ObtenerIdentidadIDDeOrganizacionIDEnProyecto(IdentidadPagina.OrganizacionID.Value, ProyectoSeleccionado.Clave);
                        identCN.Dispose();
                    }
                }
            }
            ViewBag.IdentidadMensajeID = identMensaje;

            if (MostrarRecursos)
            {
                Guid grafoID = Guid.Empty;
                string parametrosAdiccionales = "";

                if (IdentidadPagina.OrganizacionID.HasValue)
                {
                    grafoID = IdentidadPagina.OrganizacionID.Value;
                }
                else
                {
                    grafoID = IdentidadPagina.FilaIdentidad.PerfilID;
                }

                if (IdentidadPagina.Persona != null)
                {
                    parametrosAdiccionales = "|gnoss:haspublicadorIdentidadID=gnoss:" + IdentidadPagina.Clave.ToString().ToUpper();
                }

                string hash = "var hash = ObtenerHash2(); if(hash.indexOf('recursos')>=0){hash = hash.substring(8);}";

                insertarScriptBuscador("", "panResultados", "panNumResultados", "panFiltros", "panResultados", "divFiltros", grafoID.ToString(), "panNavegadorPaginas", hash, UtilIdiomas.GetText("URLSEM", "CONTRIBUCIONES"), parametrosAdiccionales, "", "", "", "", Guid.Empty, ProyectoSeleccionado.Clave, null, (short)TipoBusqueda.Contribuciones, null, null);
            }

            if (!mControladorBase.UsuarioActual.EsIdentidadInvitada && IdentidadPagina != null)
            {
                CargarActividadReciente(1);

                CargarPersonasPerfil();

                if (!EsEcosistemaSinContactos)
                {
                    CargarContactosPerfil();
                }

                string[] parametrosPeticion = Request.QueryString.ToString().Trim('?').Split('&');

                CargarSeguidosPerfil(1);

                CargarSeguidoresPerfil(1);

                CargarCurriculumModelDeIdentidadPagina();

                CargarInfoExtraDeIdentidadPagina(IdentidadPagina.Clave, paginaModel.Profile);

                CargarGrupos();

                if (parametrosPeticion.Contains("recursos") && MostrarRecursos)
                {
                    CargarPaginaRecursos();
                }
                else if (parametrosPeticion.Contains("contactos"))
                {
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Contacts;
                }
                else if (parametrosPeticion.Contains("seguidores"))
                {
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Followers;
                }
                else if (parametrosPeticion.Contains("sigue-a"))
                {
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Followed;
                }
                else if (parametrosPeticion.Contains("grupos"))
                {
                    paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Grups;
                }
                else
                {
                    ViewBag.BodyClass = " layout01 homePerfil  comGnoss";
                }
                GadgetController gadgetController = new GadgetController(this, mHttpContextAccessor, mLoggingService, mGnossCache, mConfigService, mVirtuosoAD, mEntityContext, mRedisCacheWrapper, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mEnv, mAvailableServices, mLoggerFactory.CreateLogger<GadgetController>(), mLoggerFactory);
                paginaModel.Gadgets = gadgetController.CargarListaGadgetsFichaPerfil(IdentidadPagina.PerfilID);

            }

            CargarPersonasInteres(4);

            List<CategoryModel> categoriasTesauro = CargarTesauroProyecto(UtilIdiomas.LanguageCode);
            List<CategoryModel> categoriasSuscritas = new List<CategoryModel>();
            if (Suscripcion != null && Suscripcion.FilasCategoriasVinculadas != null)
            {
                foreach (CategoryModel cat in categoriasTesauro)
                {
                    bool selected = Suscripcion.GestorSuscripcion.SuscripcionDW.ListaCategoriaTesVinSuscrip.Any(filaCat => filaCat.CategoriaTesauroID == cat.Key);
                    if (selected)
                    {
                        cat.Selected = true;
                        categoriasSuscritas.Add(cat);
                    }
                }
            }

            paginaModel.Categories = categoriasSuscritas;
            return View(paginaModel);
        }

        public Suscripcion Suscripcion
        {
            get
            {
                if (mSuscripcion == null)
                {
                    SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
                    DataWrapperSuscripcion suscDW = suscCN.ObtenerSuscripcionesDePerfil(IdentidadActual.FilaIdentidad.PerfilID, false);
                    IdentidadActual.GestorIdentidades.GestorSuscripciones = new GestionSuscripcion(suscDW, mLoggingService, mEntityContext);
                    suscCN.Dispose();
                    mSuscripcion = IdentidadActual.GestorIdentidades.GestorSuscripciones.ObtenerSuscripcionAProyecto(ProyectoSeleccionado.Clave);
                }

                return mSuscripcion;
            }
        }

		public ActionResult CargarModalCambiarRolUsuario(Guid pIdentidadID)
		{
            try 
            {
				CambiarRolUsuarioViewModel model = new CambiarRolUsuarioViewModel();
				model.IdentidadID = pIdentidadID;
				model.Roles = new List<RolModel>();
				model.RolesYaTiene = new List<Guid>();
                model.RolesHeredados = new List<RolHeredado>();
				ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                bool esEcosistema = false;
                
                List<GrupoUsuarioConRoles> grupos = usuarioCN.ObtenerGruposDeUsuario(pIdentidadID);
                List<RolHeredado> heredados = new List<RolHeredado>();

                foreach (var grupo in grupos)
                {
                    foreach (var rol in grupo.RolesGrupo)
                    {
                        heredados.Add(new RolHeredado
                        {
                            Nombre = UtilCadenas.ObtenerTextoDeIdioma(rol.Nombre, UtilIdiomas.LanguageCode, null),
                            Descripcion = HttpUtility.HtmlDecode(UtilCadenas.ObtenerTextoDeIdioma(rol.Descripcion, UtilIdiomas.LanguageCode, null)),
                            GrupoOrigen = grupo.NombreGrupo + " (Grupo de origen)"
                        });
                    }
                }
                model.RolesHeredados = heredados;
                if (!string.IsNullOrEmpty(RequestParams("ecosistema")) && RequestParams("ecosistema").Equals("true"))
				{
                    esEcosistema = true;
				}
                if (esEcosistema)
                {
                    
					Guid usuarioID = usuarioCN.ObtenerGuidUsuarioIDporIdentidadID(pIdentidadID);
					List<RolEcosistema> rolesEcosistema = proyectoCN.ObtenerRolesAdministracionEcosistema();
                    List<RolEcosistema> rolesUsuario = proyectoCN.ObtenerRolesAdministracionEcosistemaDeUsuario(usuarioID);

                    foreach (RolEcosistema rol in rolesEcosistema)
                    {
						RolModel rolModel = new RolModel();
						rolModel.Nombre = UtilCadenas.ObtenerTextoDeIdioma(rol.Nombre, UtilIdiomas.LanguageCode, null);
						rolModel.RolID = rol.RolID;
						rolModel.Descripcion = HttpUtility.HtmlDecode(UtilCadenas.ObtenerTextoDeIdioma(rol.Descripcion, UtilIdiomas.LanguageCode, null));
                        rolModel.Tipo = (short)AmbitoRol.Ecosistema;
						model.Roles.Add(rolModel);

                        if (rolesUsuario.Contains(rol))
                        {
                            model.RolesYaTiene.Add(rol.RolID);
                        }
					}
                }
                else
                {
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    List<Rol> rolesIdentidad = identidadCN.ObtenerRolesDeIdentidad(pIdentidadID);
					List<Rol> rolesProyecto = proyectoCN.ObtenerRolesDeProyecto(ProyectoSeleccionado.Clave);

					foreach (Rol rol in rolesProyecto)
					{
						RolModel rolModel = new RolModel();
						rolModel.Nombre = UtilCadenas.ObtenerTextoDeIdioma(rol.Nombre, UtilIdiomas.LanguageCode, null);
						rolModel.RolID = rol.RolID;
						rolModel.Descripcion = HttpUtility.HtmlDecode(UtilCadenas.ObtenerTextoDeIdioma(rol.Descripcion, UtilIdiomas.LanguageCode, null));
                        rolModel.Tipo = rol.Tipo;

						model.Roles.Add(rolModel);

						if (rolesIdentidad.Contains(rol))
						{
							model.RolesYaTiene.Add(rol.RolID);
						}
					}

					identidadCN.Dispose();					
				}

				proyectoCN.Dispose();
                usuarioCN.Dispose();

				return PartialView("_modal-views/_change-rol", model);
		    }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return GnossResultERROR();
            }
		}

		public ActionResult CargarModalCambiarRolGrupo(Guid pGrupoID)
		{
            try
            {
				ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
				IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication,  mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
				List<Rol> rolesGrupo = proyectoCN.ObtenerRolesDeGrupo(pGrupoID);
				DataWrapperIdentidad dwIdentidad = identidadCN.ObtenerGruposPorIDGrupo(new List<Guid>() { pGrupoID }, false);
				List<Rol> rolesProyecto = proyectoCN.ObtenerRolesDeProyecto(ProyectoSeleccionado.Clave);

				CambiarRolGrupoViewModel model = new CambiarRolGrupoViewModel();
				model.NombreGrupo = dwIdentidad.ListaGrupoIdentidades.Where(grupo => grupo.GrupoID.Equals(pGrupoID)).Select(x => x.Nombre).FirstOrDefault();
				model.GrupoID = pGrupoID;
				model.Roles = new List<RolModel>();
				model.RolesYaTiene = new List<Guid>();

				foreach (Rol rol in rolesProyecto)
				{
					RolModel rolModel = new RolModel();
					rolModel.Nombre = UtilCadenas.ObtenerTextoDeIdioma(rol.Nombre, UtilIdiomas.LanguageCode, null);
					rolModel.RolID = rol.RolID;
					rolModel.Descripcion = HttpUtility.HtmlDecode(UtilCadenas.ObtenerTextoDeIdioma(rol.Descripcion, UtilIdiomas.LanguageCode, null));
                    rolModel.Tipo = rol.Tipo;

					model.Roles.Add(rolModel);

                    if (rolesGrupo.Contains(rol))
                    {
                        model.RolesYaTiene.Add(rol.RolID);
                    }
				}

                proyectoCN.Dispose();
                identidadCN.Dispose();

				return PartialView("_modal-views/_change-rol-group", model);
			}
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return GnossResultERROR();
            }
		}

        public ActionResult CambiarRolesGrupo(Guid pGrupoID, string pListaRoles)
        {
			try
            {
				if (pListaRoles == null)
				{
					pListaRoles = "";
				}
				if (!pGrupoID.Equals(Guid.Empty))
				{
					string[] rolesAux = pListaRoles.Split("|||");
					List<Guid> roles = new List<Guid>();
					foreach (string rol in rolesAux)
					{
						if (Guid.TryParse(rol, out Guid rolID))
						{
							roles.Add(rolID);
						}
					}
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    List<Rol> rolesGrupo = proyectoCN.ObtenerRolesDeGrupo(pGrupoID);
                    // Eliminamos los roles que ya no tiene el grupo
                    foreach (Rol rolGrupo in rolesGrupo)
                    {
                        if (!roles.Contains(rolGrupo.RolID))
                        {
                            RolGrupoIdentidades filaGrupoIdentidades = mEntityContext.RolGrupoIdentidades.Where(x => x.GrupoID.Equals(pGrupoID) && x.RolID.Equals(rolGrupo.RolID)).FirstOrDefault();
                            if (filaGrupoIdentidades != null)
                            {
								mEntityContext.EliminarElemento(filaGrupoIdentidades);
							}                            
                        }
                    }
                    // Añadimos los roles nuevos
                    foreach (Guid rolID in roles)
                    {
                        Rol rolGrupo = rolesGrupo.Where(x => x.RolID.Equals(rolID)).FirstOrDefault();
                        if (rolGrupo == null)
                        {
                            RolGrupoIdentidades filaRolGrupoIdentidades = new RolGrupoIdentidades();
                            filaRolGrupoIdentidades.RolID = rolID;
                            filaRolGrupoIdentidades.GrupoID = pGrupoID;

                            mEntityContext.RolGrupoIdentidades.Add(filaRolGrupoIdentidades);
                        }
                    }

                    mEntityContext.SaveChanges();
				}

				return GnossResultOK();
			}
            catch (Exception ex)
            {
				mLoggingService.GuardarLogError(ex, $"Error al guardar los roles del grupo", mlogger);
				return GnossResultERROR("Error al guardar los roles del grupo");
			}
        }

		public ActionResult CambiarRolesUsuario(Guid pIdentidadID, string pListaRoles)
		{
			try
			{
                if (pListaRoles == null)
                {
                    pListaRoles = "";
                }
                if (!pIdentidadID.Equals(Guid.Empty))
                {
					UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
					bool esEcosistema = false;
					if (!string.IsNullOrEmpty(RequestParams("ecosistema")) && RequestParams("ecosistema").Equals("true"))
					{
						esEcosistema = true;
					}
					string[] rolesAux = pListaRoles.Split("|||");
					List<Guid> rolesNuevos = new List<Guid>();
					foreach (string rol in rolesAux)
					{
						if (Guid.TryParse(rol, out Guid rolID))
						{
							rolesNuevos.Add(rolID);
						}
					}
                    if (esEcosistema)
                    {
						Guid usuarioID = usuCN.ObtenerGuidUsuarioIDporIdentidadID(pIdentidadID);
						ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
						List<RolEcosistema> rolesEcosistema = proyectoCN.ObtenerRolesAdministracionEcosistema();
						List<RolEcosistema> rolesUsuario = proyectoCN.ObtenerRolesAdministracionEcosistemaDeUsuario(usuarioID);
                        
						// Eliminamos los roles que ya no tiene
						foreach (RolEcosistema rol in rolesUsuario)
						{
							if (!rolesNuevos.Contains(rol.RolID))
							{                                
								RolEcosistemaUsuario filaRol = mEntityContext.RolEcosistemaUsuario.Where(x => x.UsuarioID.Equals(usuarioID) && x.RolID.Equals(rol.RolID)).FirstOrDefault();
								if (filaRol != null)
								{
                                    if (filaRol.RolID.Equals(ProyectoAD.RolAdministradorEcosistema))
                                    {
										int numeroAdministradores = mEntityContext.RolEcosistemaUsuario.Where(x => x.RolID.Equals(ProyectoAD.RolAdministradorEcosistema)).ToList().Count;
										if (numeroAdministradores == 1)
                                        {
											return GnossResultERROR("No puede ser eliminado porque es el único administrador del proyecto");
										}
                                    }
									mEntityContext.EliminarElemento(filaRol);
								}
							}
						}
						// Añadimos los nuevos
						foreach (Guid rolID in rolesNuevos)
						{
							RolEcosistema rolDeIdentidad = rolesUsuario.Where(x => x.RolID.Equals(rolID)).FirstOrDefault();
							if (rolDeIdentidad == null)
							{
								RolEcosistemaUsuario filaRol = new RolEcosistemaUsuario();
								filaRol.UsuarioID = usuarioID;
								filaRol.RolID = rolID;

								mEntityContext.RolEcosistemaUsuario.Add(filaRol);
							}
						}
					}
                    else
                    {
						IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
						List<Rol> rolesIdentidad = identidadCN.ObtenerRolesDeIdentidad(pIdentidadID);

						// Eliminamos los roles que ya no tiene
						foreach (Rol rol in rolesIdentidad)
						{
							if (!rolesNuevos.Contains(rol.RolID))
							{
								RolIdentidad filaRolIdentidad = mEntityContext.RolIdentidad.Where(x => x.IdentidadID.Equals(pIdentidadID) && x.RolID.Equals(rol.RolID)).FirstOrDefault();
								if (filaRolIdentidad != null)
								{
                                    if (filaRolIdentidad.RolID.Equals(ProyectoAD.RolAdministrador))
                                    {
										ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
										int identidadesAdministradoras = proyectoCN.ObtenerIdentidadesAdministradorasDeProyecto(ProyectoSeleccionado.Clave).Count;
										if (identidadesAdministradoras == 1)
                                        {
											return GnossResultERROR("No puede ser eliminado porque es el único administrador del proyecto");
										}
                                    }
									mEntityContext.EliminarElemento(filaRolIdentidad);
								}
							}
						}
						// Añadimos los nuevos
						foreach (Guid rolID in rolesNuevos)
						{
							Rol rolDeIdentidad = rolesIdentidad.Where(x => x.RolID.Equals(rolID)).FirstOrDefault();
							if (rolDeIdentidad == null)
							{
								RolIdentidad filaRolIdentidad = new RolIdentidad();
								filaRolIdentidad.IdentidadID = pIdentidadID;
								filaRolIdentidad.RolID = rolID;

								mEntityContext.RolIdentidad.Add(filaRolIdentidad);
							}
						}
					}                    

                    mEntityContext.SaveChanges();
				}

				return GnossResultOK();
			}
			catch (Exception ex)
			{
				mLoggingService.GuardarLogError(ex, $"Error al guardar los roles del usuario", mlogger);
				return GnossResultERROR("Error al guardar los roles del usuario");
			}
		}

		private void CambiarRolUsuario()
        {
            Guid usuarioID = IdentidadPagina.Persona.UsuarioID;
            short rol;

            if (short.TryParse(RequestParams("rol"), out rol))
            {
                bool rolCambiado = ControladorIdentidades.CambiarRolUsuarioEnProyecto(IdentidadPagina, rol);

                if (rolCambiado)
                {
                    ControladorPersonas controladorPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                    controladorPers.ActualizarModeloBASE(IdentidadPagina, ProyectoSeleccionado.Clave, true, false, PrioridadBase.Alta, mAvailableServices);

                    if (usuarioID.Equals(mControladorBase.UsuarioActual.UsuarioID))
                    {
                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                        mControladorBase.UsuarioActual.RolPermitidoProyecto = proyCN.CalcularRolFinalUsuarioEnProyecto(usuarioID, mControladorBase.UsuarioActual.Login, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);

                        Response.Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                    }
                }
            }
        }

        private void ModificarEnviarNewsletterAUsuario(bool pEnviarNewsletter)
        {
            if (!IdentidadPagina.EsOrganizacion)
            {
                IdentidadPagina.FilaIdentidad.RecibirNewsLetter = pEnviarNewsletter;

                mEntityContext.SaveChanges();
            }
        }

        private void DesbloquearUsuarioEnComunidad()
        {
            throw new NotImplementedException();
        }

        private void ModificarBloqueoUsuarioEnComunidad(bool pBloquear)
        {            
            ControladorIdentidades.BloquearDesbloquearUsuario(IdentidadPagina, pBloquear);
            ControladorPersonas controladorPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            controladorPers.ActualizarModeloBASE(IdentidadPagina, ProyectoSeleccionado.Clave, true, false, PrioridadBase.Alta, mAvailableServices);
        }

        private void ReadmitirUsuarioEnComunidad()
        {
            GestionIdentidades gestorIdentidades = IdentidadPagina.GestorIdentidades;

            if (!IdentidadPagina.EsOrganizacion)
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                gestorIdentidades.GestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerUsuarioCompletoPorID(IdentidadPagina.Persona.FilaPersona.UsuarioID.Value), mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
                usuarioCN.Dispose();

                gestorIdentidades.GestorUsuarios.GestorIdentidades = gestorIdentidades;
            }

            if (IdentidadPagina.FilaIdentidad.Tipo == (short)TiposIdentidad.Personal)
            {
                gestorIdentidades.GestorUsuarios.RetomarUsuarioEnProyecto(IdentidadPagina.Usuario.FilaUsuario, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, IdentidadPagina.Clave);
                gestorIdentidades.RecargarHijos();

                mEntityContext.SaveChanges();
            }
            else if (IdentidadPagina.FilaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo || IdentidadPagina.FilaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal)
            {
                gestorIdentidades.GestorUsuarios.RetomarUsuarioEnProyecto(IdentidadPagina.Usuario.FilaUsuario, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, IdentidadPagina.Clave);
                gestorIdentidades.RecargarHijos();

                mEntityContext.SaveChanges();
            }
            else if (IdentidadPagina.FilaIdentidad.Tipo == (short)TiposIdentidad.Organizacion)
            {
                GestionOrganizaciones gestorOrg = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);

                //Añado  OrganizacionParticipaProy
                gestorOrg.AgregarOrganizacionAProyecto((Guid)IdentidadPagina.OrganizacionID, IdentidadPagina.FilaIdentidad.OrganizacionID, IdentidadPagina.FilaIdentidad.ProyectoID, IdentidadPagina.Clave);

                //Pongo a "Fecha de Baja null" la IdentidadPagina , la pongo Activa
                gestorIdentidades.RetomarIdentidadPerfil(IdentidadPagina.Clave);

                //Guardo
                mEntityContext.SaveChanges();

                gestorOrg.Dispose();
                gestorOrg = null;
            }

            //Invalidamos la cache de amigos en la comunidad
            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService,mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
            amigosCL.InvalidarAmigosPertenecenProyecto(ProyectoSeleccionado.Clave);
            amigosCL.Dispose();

            ControladorPersonas controladorPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            // Lo paso a true para marcarla como privada, así sólo se le muestra al administrador para que pueda readmitirlo
            controladorPers.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[IdentidadPagina.Clave], ProyectoSeleccionado.Clave, true, false, PrioridadBase.Alta, mAvailableServices);
        }


        /// <summary>
        /// Cambia la contraseña de acceso de un miembro desde el admin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
		[TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarMiembros } })]
		[TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.AdministrarMiembrosEcosistema } })]
		public ActionResult ResetPassord(PeticionContrasenya pModelo)
        {
            try
            {
                if (pModelo.pass == null)
                {
                    string error = UtilIdiomas.GetText("COMADMININFOGENERAL", "GADGETSINRELLENAR");
                    return GnossResultERROR(error);
                }
                else
                {

                    string contrasenya = pModelo.pass;

                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    Guid? usuID = usuarioCN.ObtenerUsuarioIDPorNombreCorto(pModelo.nombreCortoUsu);

                    usuarioCN.EstablecerPasswordUsuario(usuID.Value, contrasenya);

                    return GnossResultOK();
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return GnossResultERROR(ex.Message);
            }
        }

        private void ExpulsarUsuarioDeComunidad()
        {
            string motivo = $"<p>{HttpUtility.HtmlDecode(RequestParams("motivo")).Trim('\n').Replace("\n", "</p><p>")}</p>";

            ControladorProyecto.ExpulsarUsuarioComunidad(IdentidadPagina, ProyectoSeleccionado, motivo, UtilIdiomas.LanguageCode, mAvailableServices);
            ControladorPersonas controladorPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            controladorPers.ActualizarModeloBASE(IdentidadPagina, ProyectoSeleccionado.Clave, true, false, PrioridadBase.Alta, mAvailableServices);
        }

        private void CargarInfoExtraDeIdentidadPagina(Guid pIdentidadID, ProfileModel pPerfilModel)
        {
            Dictionary<Guid, ProfileModel> dicIdentidadPagina = new Dictionary<Guid, ProfileModel>();
            dicIdentidadPagina.Add(pIdentidadID, pPerfilModel);
            ControladorProyectoMVC.ObtenerInfoExtraIdentidadesPorID(dicIdentidadPagina);
            string grafo = ProyectoSeleccionado.Clave.ToString();
            if (IdentidadActual.TrabajaConOrganizacion)
            {
                grafo = IdentidadActual.OrganizacionID.Value.ToString();
            }
            else
            {
                grafo = IdentidadActual.PerfilID.ToString();
            }
            string urlPaginaContribuciones = $"{BaseURLIdioma}{UrlPerfil}{UtilIdiomas.GetText("URLSEM", "MISCONTRIBUCIONES")}";
            ViewBag.UrlPaginaMisContribuciones = urlPaginaContribuciones;
            pPerfilModel.ExtraInfo.IdentityResourceCounter.ContributionsCounter = ObtenerContadorContribuciones(ProyectoSeleccionado.Clave, true, !UsuarioActual.EsIdentidadInvitada, UsuarioActual.EsUsuarioInvitado, UsuarioActual.IdentidadID, "", true, false, TipoBusqueda.Contribuciones, urlPaginaContribuciones, grafo, "PestanyaActualID=00000000-0000-0000-0000-000000000000|gnoss:hasEstado=Publicado%20/%20Compartido|", false, "MyGnoss", UtilIdiomas.LanguageCode, UsuarioActual.UsarMasterParaLectura, Guid.NewGuid());
        }

        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Follow()
        {
            if (mControladorBase.UsuarioActual != null && !mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                if (IdentidadPagina != null)
                {
                    SuscripcionCL suscripcionCL = new SuscripcionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCL>(), mLoggerFactory);

                    if (IdentidadPagina.Persona != null)
                    {
                        ControladorSuscripciones.SuscribirmePerfil(IdentidadActual, ProyectoSeleccionado, BaseURL, UrlIntragnoss, IdentidadPagina, true, null, null, UtilIdiomas.LanguageCode, mAvailableServices);

                        AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
                        FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                        GestionAmigos gestAmigos = new GestionAmigos(amigosCN.CargarAmigosCompleto(IdentidadActual.IdentidadMyGNOSS.Clave), IdentidadPagina.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        List<Guid> listaSeguidores = identidadCN.ObtenerListaIdentidadesIDSusucritasAPerfilEnProyecto(Perfil.Key, ProyectoSeleccionado.Clave);
                        identidadCN.Dispose();

                        // Si en mi lista de seguidores está presente la identidad que voy a seguir, entonces es un amigo.
                        if (listaSeguidores.Contains(IdentidadPagina.Clave))
                        {
                            // Comprobamos que no estén añadidos en la tabla
                            if (!amigosCN.EsAmigoDeIdentidad(IdentidadActual.IdentidadMyGNOSS.Clave, IdentidadPagina.IdentidadMyGNOSS.Clave) && !amigosCN.EsAmigoDeIdentidad(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave))
                            {
                                gestAmigos.CrearFilaAmigo(IdentidadActual.IdentidadMyGNOSS.Clave, IdentidadPagina.IdentidadMyGNOSS.Clave);
                                gestAmigos.CrearFilaAmigo(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);

                                facetadoCN.InsertarNuevoContacto(IdentidadActual.IdentidadMyGNOSS.Clave.ToString(), IdentidadPagina.IdentidadMyGNOSS.Clave.ToString());
                                facetadoCN.InsertarNuevoContacto(IdentidadPagina.IdentidadMyGNOSS.Clave.ToString(), IdentidadActual.IdentidadMyGNOSS.Clave.ToString());

                                mEntityContext.SaveChanges();

                                // Agregamos una fila al BASE
                                ControladorContactos contrContactos = new ControladorContactos(mLoggingService, mEntityContext, mConfigService, mEntityContextBASE, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorContactos>(), mLoggerFactory);
                                contrContactos.ActualizarModeloBaseSimple(IdentidadActual.IdentidadMyGNOSS.Clave, IdentidadPagina.IdentidadMyGNOSS.Clave);
                                contrContactos.ActualizarModeloBaseSimple(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);

                                //Limpiamos la cache de los contactos y la regeneramos
                                AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
                                amigosCL.InvalidarAmigos(IdentidadActual.IdentidadMyGNOSS.Clave);
                                amigosCL.InvalidarAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave);
                                amigosCL.AgregarCacheAutocompletarInvalidar(Guid.NewGuid());
                                amigosCL.RefrescarCacheAmigos(IdentidadActual.IdentidadMyGNOSS.Clave, mEntityContextBASE, mAvailableServices, EsIdentidadActualAdministradorOrganizacion);
                                amigosCL.RefrescarCacheAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave, mEntityContextBASE, mAvailableServices, EsIdentidadActualAdministradorOrganizacion);
                                amigosCL.Dispose();
                            }
                        }

                        amigosCN.Dispose();
                        facetadoCN.Dispose();
                        gestAmigos.Dispose();

                    }
                    else
                    {
                        //Me suscribo a lo que publican todos los miembros de la organización (corporativo y profesional)
                        ControladorSuscripciones.SuscribirmeOrganizacion(IdentidadActual, ProyectoSeleccionado, UrlIntragnoss, IdentidadPagina, true, 0, mAvailableServices);
                    }

                    //Invalido las cachés de identidades suscritas de los dos usuarios. 
                    suscripcionCL.InvalidarListaIdentidadesSuscritasPerfil(IdentidadPagina.PerfilID);
                    suscripcionCL.InvalidarListaIdentidadesSuscritasPerfil(IdentidadActual.PerfilID);

                    return GnossResultOK();
                }
            }
            return GnossResultERROR();
        }


        [HttpPost, TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Unfollow()
        {
            if (mControladorBase.UsuarioActual != null && !mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                if (IdentidadPagina != null)
                {
                    if (IdentidadPagina.Persona != null)
                    {
                        ControladorSuscripciones.SuscribirmePerfil(IdentidadActual, ProyectoVirtual, BaseURL, UrlIntragnoss, IdentidadPagina, false, null, null, UtilIdiomas.LanguageCode, mAvailableServices);
                        AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
                        GestionAmigos gestAmigos = new GestionAmigos(amigosCN.CargarAmigosCompleto(IdentidadActual.IdentidadMyGNOSS.Clave), IdentidadPagina.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                        FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);

                        if (amigosCN.EsAmigoDeIdentidad(IdentidadActual.IdentidadMyGNOSS.Clave, IdentidadPagina.IdentidadMyGNOSS.Clave) && amigosCN.EsAmigoDeIdentidad(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave))
                        {
                            gestAmigos.EliminarAmigos(IdentidadActual.IdentidadMyGNOSS, IdentidadPagina.IdentidadMyGNOSS);
                            gestAmigos.EliminarAmigos(IdentidadPagina.IdentidadMyGNOSS, IdentidadActual.IdentidadMyGNOSS);

                            facetadoCN.BorrarContacto(IdentidadActual.IdentidadMyGNOSS.Clave, IdentidadPagina.IdentidadMyGNOSS.Clave);
                            facetadoCN.BorrarContacto(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);

                            mEntityContext.SaveChanges();

                            //Agregamos una fila al BASE
                            ControladorContactos contrContactos = new ControladorContactos(mLoggingService, mEntityContext, mConfigService, mEntityContextBASE, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorContactos>(), mLoggerFactory);
                            contrContactos.ActualizarEliminacionModeloBaseSimple(IdentidadActual.IdentidadMyGNOSS.Clave, IdentidadPagina.IdentidadMyGNOSS.Clave);
                            contrContactos.ActualizarEliminacionModeloBaseSimple(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);

                            //Limpiamos la cache de los contactos y lo regeneramos
                            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
                            amigosCL.InvalidarAmigos(IdentidadActual.IdentidadMyGNOSS.Clave);
                            amigosCL.InvalidarAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave);
                            amigosCL.AgregarCacheAutocompletarInvalidar(Guid.NewGuid());
                            amigosCL.RefrescarCacheAmigos(IdentidadActual.IdentidadMyGNOSS.Clave, mEntityContextBASE, mAvailableServices, EsIdentidadActualAdministradorOrganizacion);
                            amigosCL.RefrescarCacheAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave, mEntityContextBASE, mAvailableServices, EsIdentidadActualAdministradorOrganizacion);

                            amigosCL.Dispose();
                        }

                        amigosCN.Dispose();
                        gestAmigos.Dispose();
                        facetadoCN.Dispose();
                    }
                    else
                    {
                        //Me suscribo a lo que publican todos los miembros de la organización (corporativo y profesional)
                        ControladorSuscripciones.SuscribirmeOrganizacion(IdentidadActual, ProyectoVirtual, UrlIntragnoss, IdentidadPagina, false, 0, mAvailableServices);
                    }
                    SuscripcionCL suscripcionCL = new SuscripcionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCL>(), mLoggerFactory);

                    suscripcionCL.InvalidarListaIdentidadesSuscritasPerfil(IdentidadPagina.PerfilID);
                    suscripcionCL.InvalidarListaIdentidadesSuscritasPerfil(IdentidadActual.PerfilID);

                    return GnossResultOK();
                }
            }
            return GnossResultERROR();
        }
        private int ObtenerContadorContribuciones(Guid pProyectoID, bool pEsMyGnoss, bool pEstaEnProyecto, bool EsUsuarioInvitado, Guid pIdentidadID, string pArgumentos, bool pEsPrimeraCarga, bool pAdminQuiereVerTodasPersonas, TipoBusqueda pTipoBusqueda, string pUrlPaginaBusqueda, string pGrafo, string pParametroAdicional, bool pHayParametrosBusqueda, string pUbicacionBusqueda, string pLanguageCode, bool pUsarMasterParaLectura, Guid pTokenAfinidad)
        {
            int numResultados = 0;
            try
            {
                mLoggingService.AgregarEntrada("Llamada a los servicios de resultados y facetas");
                CargadorResultados cargadorResultados = new CargadorResultados();
                cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();
                string jsonResultados = cargadorResultados.CargarResultados(pProyectoID, pIdentidadID, EsUsuarioInvitado, pUrlPaginaBusqueda, pUsarMasterParaLectura, pAdminQuiereVerTodasPersonas, pTipoBusqueda, pGrafo, pParametroAdicional, pArgumentos, pEsPrimeraCarga, pLanguageCode, -1, "", pTokenAfinidad, Request);
                KeyValuePair<int, string> respuestaResultados = JsonConvert.DeserializeObject<KeyValuePair<int, string>>(jsonResultados);

                numResultados = respuestaResultados.Key;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error al obtener el número de contribuciones del perfil", mlogger);
            }

            mLoggingService.AgregarEntrada("Fin llamada a los servicios de resultados y facetas");

            return numResultados;
        }

        private void CargarPersonasInteres(int pNumPersonasCarga)
        {
            string localidad = "";
            string provincia = "";
            string pais = "";

            Dictionary<string, string> listaDatosExtra = new Dictionary<string, string>();
            List<Guid> listaCategoriasSuscritas = new List<Guid>();

            PaisCL paisCL = new PaisCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PaisCL>(), mLoggerFactory);
            DataWrapperPais paisDW = paisCL.ObtenerPaisesProvincias();

            //Obtengo el país, provincia y localidad:
            if (IdentidadActual.Persona.FilaPersona.LocalidadPersonal != null)
            {
                localidad = IdentidadActual.Persona.FilaPersona.LocalidadPersonal;
            }
            if (IdentidadActual.Persona.FilaPersona.PaisPersonalID.HasValue && IdentidadActual.Persona.FilaPersona.PaisPersonalID != Guid.Empty)
            {
                pais = paisDW.ListaPais.Where(item => item.PaisID.Equals(IdentidadActual.Persona.FilaPersona.PaisPersonalID.Value)).FirstOrDefault().Nombre;
            }
            if (IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID.HasValue)
            {
                if (IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID != Guid.Empty)
                {
                    provincia = paisDW.ListaProvincia.Where(item => item.ProvinciaID.Equals(IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID.Value)).FirstOrDefault().Nombre;
                }
            }
            else if (IdentidadActual.Persona.FilaPersona.ProvinciaPersonal != null)
            {
                provincia = IdentidadActual.Persona.FilaPersona.LocalidadPersonal;
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

            //Obtengo los datos extra de registro
            DataWrapperDatoExtra dataWrapperDatoExtra = identidadCN.ObtenerIdentidadDatoExtraRegistroDeProyecto(ProyectoSeleccionado.Clave, IdentidadActual.Clave);

            foreach (Triples fila in dataWrapperDatoExtra.ListaTriples)
            {
                string predicado = fila.PredicadorRDF;
                if (!listaDatosExtra.ContainsKey(predicado))
                {
                    listaDatosExtra.Add(predicado, fila.Opcion);
                }
            }

            //Obtengo las categorías de tesauro a las que está suscrito un usuario en un proyecto
            SuscripcionCN suscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
            dataWrapperDatoExtra = suscripcionCN.ObtenerCategoriasTesauroDeProyectoSuscritaIdentidad(ProyectoSeleccionado.Clave, IdentidadActual.Clave);

            foreach (Triples fila in dataWrapperDatoExtra.ListaTriples)
            {
                listaCategoriasSuscritas.Add(new Guid(fila.Opcion));
            }

            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            facetadoCN.InformacionOntologias = InformacionOntologias;

            FacetadoDS facetadoDS = new FacetadoDS();
            bool recomendacionescargadas = false;
            if (!mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                try
                {
                    if (!string.IsNullOrEmpty(ParametrosGeneralesRow.AlgoritmoPersonasRecomendadas) && ParametrosGeneralesRow.AlgoritmoPersonasRecomendadas.ToLower() != "true")
                    {

                        facetadoDS = new FacetadoDS();
                        string props = ParametrosGeneralesRow.AlgoritmoPersonasRecomendadas;

                        string[] propiedades = props.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                        Dictionary<string, float> listaPropiedadesPeso = new Dictionary<string, float>();
                        foreach (string propiedad in propiedades)
                        {
                            string[] propiedadPeso = propiedad.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            string prop = propiedadPeso[0].Trim();
                            float peso = float.Parse(propiedadPeso[1].Trim());
                            if (!listaPropiedadesPeso.ContainsKey(prop))
                            {
                                listaPropiedadesPeso.Add(prop, peso);
                            }
                        }

                        #region Obtenemos datos de usuario

                        Dictionary<string, Object> valorPropiedades = new Dictionary<string, object>();

                        foreach (string propiedad in listaPropiedadesPeso.Keys)
                        {
                            switch (propiedad)
                            {
                                case "http://www.w3.org/2006/vcard/ns#locality":
                                    valorPropiedades.Add(propiedad, localidad);
                                    break;
                                case "http://d.opencalais.com/1/type/er/Geo/ProvinceOrState":
                                    valorPropiedades.Add(propiedad, provincia);
                                    break;
                                case "http://www.w3.org/2006/vcard/ns#country-name":
                                    valorPropiedades.Add(propiedad, pais);
                                    break;
                                case "http://xmlns.com/foaf/0.1/interest":
                                    valorPropiedades.Add(propiedad, listaCategoriasSuscritas);
                                    break;
                                case "http://gnoss/hasPopularidad":
                                    valorPropiedades.Add(propiedad, null);
                                    break;
                                default:
                                    if (listaDatosExtra.ContainsKey(propiedad))
                                    {
                                        valorPropiedades.Add(propiedad, listaDatosExtra[propiedad]);
                                    }
                                    break;
                            }
                        }


                        #endregion

                        if (valorPropiedades.Count > 0)
                        {
                            facetadoDS = facetadoCN.ObtenerPersonasRecomendadas(ProyectoSeleccionado.Clave, IdentidadActual.Clave, listaPropiedadesPeso, valorPropiedades, pNumPersonasCarga + 20);
                        }
                        recomendacionescargadas = true;
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex.ToString());
                }

                if (!recomendacionescargadas)
                {
                    facetadoDS = facetadoCN.ObtenerPersonasRecomendadas(ProyectoSeleccionado.Clave, IdentidadActual.Clave, localidad, provincia, pais, listaDatosExtra, listaCategoriasSuscritas, pNumPersonasCarga + 20);
                }
            }

            List<Guid> listaIdentidades = new List<Guid>();
            if (facetadoDS.Tables.Count > 0)
            {
                foreach (DataRow fila in facetadoDS.Tables[0].Rows)
                {
                    string id = (string)fila[0];
                    listaIdentidades.Add(new Guid(id.Substring(id.LastIndexOf('/') + 1)));
                }
            }

            if (listaIdentidades.Count > 0)
            {
                Dictionary<Guid, ProfileModel> identidadesModel = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdentidades);

                List<ProfileModel> listaPerfiles = new List<ProfileModel>();
                foreach (ProfileModel perfil in identidadesModel.Values)
                {
                    if (perfil.Key != IdentidadActual.Clave && (perfil.TypeProfile == ProfileType.Personal || perfil.TypeProfile == ProfileType.ProfessionalPersonal))
                    {
                        listaPerfiles.Add(perfil);
                    }
                    if (listaPerfiles.Count >= pNumPersonasCarga)
                    {
                        break;
                    }
                }

                paginaModel.PeopleOfInterest = listaPerfiles;
            }
        }

        private void AgregarContactoOrg()
        {
            if (EsIdentidadActualAdministradorOrganizacion && IdentidadActual.IdentidadOrganizacion != null && mControladorBase.UsuarioActual != null && (IdentidadPagina.Persona == null || !IdentidadPagina.Persona.Clave.Equals(IdentidadActual.Persona.Clave)))
            {
                if (!IdentidadActual.IdentidadOrganizacion.ListaPerfilesAmigos.Contains(IdentidadPagina.FilaIdentidad.PerfilID))
                {

                    NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                    DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

                    if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.Organizacion))
                    {
                        notificacionDW.Merge(notificacionCN.ObtenerInvitacionesPendientesDeMyGnoss(null, null, IdentidadPagina.OrganizacionID, true));
                    }
                    else
                    {
                        notificacionDW.Merge(notificacionCN.ObtenerInvitacionesPendientesDeMyGnoss(IdentidadPagina.FilaIdentidad.PerfilID, IdentidadPagina.Persona.Clave, null, false));
                    }

                    //Limpiamos la cache de los contactos
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
                    amigosCL.InvalidarAmigos(IdentidadActual.IdentidadMyGNOSS.Clave);
                    amigosCL.InvalidarAmigos(IdentidadActual.IdentidadOrganizacion.Clave);
                    amigosCL.InvalidarAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave);
                    amigosCL.Dispose();
                }
            }
        }

        private void EliminarContacto()
        {
            if (IdentidadActual.ModoPersonal && !EsIdentidadActualAdministradorOrganizacion && mControladorBase.UsuarioActual != null && (IdentidadPagina.Persona == null || !IdentidadPagina.Persona.Clave.Equals(IdentidadActual.Persona.Clave)))
            {

                if (IdentidadActual.ListaPerfilesAmigos.Contains(IdentidadPagina.FilaIdentidad.PerfilID))
                {
                    DataWrapperNotificacion notificacionDW = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory).ObtenerInvitacionesPendientesDeMyGnoss(IdentidadActual.FilaIdentidad.PerfilID, IdentidadActual.Persona.Clave, null, false);

                    if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.Organizacion))
                    {
                        notificacionDW.Merge(new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory).ObtenerInvitacionesPendientesDeMyGnoss(null, null, IdentidadPagina.OrganizacionID, true));
                    }
                    else
                    {
                        notificacionDW.Merge(new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory).ObtenerInvitacionesPendientesDeMyGnoss(IdentidadPagina.FilaIdentidad.PerfilID, IdentidadPagina.Persona.Clave, null, false));
                    }
                    GestionNotificaciones mGestionNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);
                    mGestionNotificaciones.EliminarNotificacionContacto(IdentidadActual.IdentidadMyGNOSS, IdentidadPagina.IdentidadMyGNOSS, TiposNotificacion.EliminarContacto, ProyectoSeleccionado, UtilIdiomas.LanguageCode);

                    if (IdentidadActual.GestorAmigos.EsAmigoRealDeIdentidad(IdentidadActual, IdentidadPagina, true, EsIdentidadActualAdministradorOrganizacion))
                    {
                        IdentidadActual.GestorAmigos.EliminarAmigos(IdentidadActual.IdentidadMyGNOSS, IdentidadPagina.IdentidadMyGNOSS);
                    }
                    else
                    {
                        IdentidadActual.GestorAmigos.CambiarContactoDeIdentidad(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS.Clave);
                    }

                    mEntityContext.SaveChanges();

                    //eliminar contacto en virtuoso
                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, false, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    facetadoCN.BorrarContacto(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);
                    facetadoCN.Dispose();

                    //Limpiamos la cache de los contactos
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
                    amigosCL.InvalidarAmigos(IdentidadActual.IdentidadMyGNOSS.Clave);
                    amigosCL.InvalidarAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave);
                    amigosCL.Dispose();
                }
            }
        }

        private void EliminarContactoOrg()
        {
            if (EsIdentidadActualAdministradorOrganizacion && IdentidadActual.IdentidadOrganizacion != null && mControladorBase.UsuarioActual != null && (IdentidadPagina.Persona == null || !IdentidadPagina.Persona.Clave.Equals(IdentidadActual.Persona.Clave)))
            {
                if (IdentidadActual.IdentidadOrganizacion.ListaPerfilesAmigos.Contains(IdentidadPagina.FilaIdentidad.PerfilID))
                {
                    NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                    DataWrapperNotificacion notificacionDW = notificacionCN.ObtenerInvitacionesPendientesDeMyGnoss(IdentidadActual.FilaIdentidad.PerfilID, IdentidadActual.Persona.Clave, IdentidadActual.OrganizacionID, EsIdentidadActualAdministradorOrganizacion);

                    if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.Organizacion))
                    {
                        notificacionDW.Merge(notificacionCN.ObtenerInvitacionesPendientesDeMyGnoss(null, null, IdentidadPagina.OrganizacionID, true));
                    }
                    else
                    {
                        notificacionDW.Merge(notificacionCN.ObtenerInvitacionesPendientesDeMyGnoss(IdentidadPagina.FilaIdentidad.PerfilID, IdentidadPagina.Persona.Clave, null, false));
                    }
                    GestionNotificaciones mGestionNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);
                    mGestionNotificaciones.EliminarNotificacionContacto(IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS, IdentidadPagina.IdentidadMyGNOSS, TiposNotificacion.EliminarContacto, ProyectoSeleccionado, UtilIdiomas.LanguageCode);

                    IdentidadActual.IdentidadOrganizacion.GestorAmigos.EliminarAmigos(IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS, IdentidadPagina.IdentidadMyGNOSS);

                    mEntityContext.SaveChanges();
                    notificacionCN.Dispose();

                    //eliminar contacto en virtuoso
                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, false, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    facetadoCN.BorrarContacto(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);
                    facetadoCN.Dispose();

                    //Limpiamos la cache de los contactos
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
                    amigosCL.InvalidarAmigos(IdentidadActual.IdentidadMyGNOSS.Clave);
                    amigosCL.InvalidarAmigos(IdentidadActual.IdentidadOrganizacion.Clave);
                    amigosCL.InvalidarAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave);
                    amigosCL.Dispose();
                }
            }
        }

        private ActionResult AccionEnviarMensaje()
        {
            bool error = false;
            if (mControladorBase.UsuarioActual != null && !mControladorBase.UsuarioActual.EsIdentidadInvitada && (IdentidadPagina.Persona != null || (IdentidadPagina.EsOrganizacion && IdentidadPagina.OrganizacionPerfil != null)))
            {
                try
                {
                    string asunto = HttpUtility.HtmlDecode(RequestParams("asunto"));
                    string mensaje = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(RequestParams("mensaje")));

                    Guid autor = IdentidadActual.IdentidadMyGNOSS.Clave;

                    GestionCorreo gestionCorreo = new GestionCorreo(new CorreoDS(), null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<GestionCorreo>(), mLoggerFactory);
                    gestionCorreo.IdentidadActual = IdentidadActual;
                    gestionCorreo.GestorIdentidades = IdentidadActual.GestorIdentidades;

                    List<Guid> listaDestinatarios = new List<Guid>();
                    listaDestinatarios.Add(IdentidadPagina.IdentidadMyGNOSS.Clave);

                    gestionCorreo.GestorIdentidades.DataWrapperIdentidad.Merge(IdentidadPagina.GestorIdentidades.DataWrapperIdentidad);
                    gestionCorreo.GestorIdentidades.GestorPersonas.DataWrapperPersonas.Merge(IdentidadPagina.GestorIdentidades.GestorPersonas.DataWrapperPersonas);
                    gestionCorreo.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(IdentidadPagina.GestorIdentidades.GestorOrganizaciones.OrganizacionDW);

                    gestionCorreo.GestorIdentidades.RecargarHijos();
                    gestionCorreo.GestorIdentidades.GestorPersonas.RecargarPersonas();
                    gestionCorreo.GestorIdentidades.GestorOrganizaciones.RecargarOrganizciones();

                    Guid idCorreo = gestionCorreo.AgregarCorreo(autor, listaDestinatarios, asunto, mensaje, BaseURL, ProyectoVirtual, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);

                    CorreoCN actualizarCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);
                    actualizarCN.ActualizarCorreo(gestionCorreo.CorreoDS);

                    AgregarMensajeABase(idCorreo, autor, listaDestinatarios, PrioridadBase.Alta);

                    //Enviamos la notificación a ColaNotificacion para enviar el mail.
                    NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                    notificacionCN.ActualizarNotificacion(mAvailableServices);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    error = true;
                }
            }
            else
            {
                error = true;
            }

            if (error)
            {
                return Content("KO|" + UtilIdiomas.GetText("BANDEJAENTRADA", "MENSAJENOENVIADO"));
            }
            else
            {
                return Content("OK");
            }
        }

        private ActionResult AccionEnviarMensajeTutor()
        {
            bool error = false;
            if (mControladorBase.UsuarioActual != null && !mControladorBase.UsuarioActual.EsIdentidadInvitada && (IdentidadPagina.Persona != null || (IdentidadPagina.EsOrganizacion && IdentidadPagina.OrganizacionPerfil != null)))
            {
                try
                {
                    string asunto = HttpUtility.HtmlDecode(RequestParams("asunto"));
                    string mensaje = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(RequestParams("mensaje")));

                    Guid autor = IdentidadActual.IdentidadMyGNOSS.Clave;

                    GestionCorreo gestionCorreo = new GestionCorreo(new CorreoDS(), null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<GestionCorreo>(), mLoggerFactory);
                    gestionCorreo.IdentidadActual = IdentidadActual;
                    gestionCorreo.GestorIdentidades = IdentidadActual.GestorIdentidades;

                    List<Guid> listaDestinatarios = new List<Guid>();
                    listaDestinatarios.Add(IdentidadPagina.IdentidadMyGNOSS.Clave);

                    gestionCorreo.GestorIdentidades.DataWrapperIdentidad.Merge(IdentidadPagina.GestorIdentidades.DataWrapperIdentidad);
                    gestionCorreo.GestorIdentidades.GestorPersonas.DataWrapperPersonas.Merge(IdentidadPagina.GestorIdentidades.GestorPersonas.DataWrapperPersonas);
                    gestionCorreo.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(IdentidadPagina.GestorIdentidades.GestorOrganizaciones.OrganizacionDW);

                    gestionCorreo.GestorIdentidades.RecargarHijos();
                    gestionCorreo.GestorIdentidades.GestorPersonas.RecargarPersonas();
                    gestionCorreo.GestorIdentidades.GestorOrganizaciones.RecargarOrganizciones();

                    Guid idCorreo = gestionCorreo.AgregarCorreoTutor(autor, listaDestinatarios, asunto, mensaje, BaseURL, ProyectoVirtual, TiposNotificacion.MensajeTutor, UtilIdiomas.LanguageCode);
                    //AgregarCorreoTutor

                    CorreoCN actualizarCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);
                    actualizarCN.ActualizarCorreo(gestionCorreo.CorreoDS);

                    AgregarMensajeABase(idCorreo, autor, listaDestinatarios, PrioridadBase.Alta);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    error = true;
                }
            }
            else
            {
                error = true;
            }

            if (error)
            {
                return Content("KO|" + UtilIdiomas.GetText("BANDEJAENTRADA", "MENSAJENOENVIADO"));
            }
            else
            {
                return Content("OK");
            }
        }

        private void EnviarMensajeDeCorreccionDeIdentidad()
        {
            if (AdministradorQuiereVerTodasLasPersonas && IdentidadPagina.Persona.FilaPersona.EstadoCorreccion != (short)EstadoCorreccion.NotificadoNoCambiado3Dias)
            {
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);
                gestorNotificaciones.AgregarNotificacionCorreccionDeIdentidad(IdentidadPagina.Persona, false, UtilIdiomas.LanguageCode);

                IdentidadPagina.Persona.FilaPersona.FechaNotificacionCorreccion = DateTime.Now;
                IdentidadPagina.Persona.FilaPersona.EstadoCorreccion = (short)EstadoCorreccion.NotificadoNoCambiado;

                mEntityContext.SaveChanges();

                ControladorPersonas contrPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                contrPers.ActualizarModeloBaseSimple(IdentidadPagina, ProyectoAD.MyGnoss, UrlIntragnoss);
            }
        }

        private void EnviarMensajeDeCorreccionDeIdentidadDefinitivo()
        {
            if (AdministradorQuiereVerTodasLasPersonas && IdentidadPagina.Persona.FilaPersona.EstadoCorreccion == (short)EstadoCorreccion.NotificadoNoCambiado3Dias)
            {
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);
                gestorNotificaciones.AgregarNotificacionCorreccionDeIdentidad(IdentidadPagina.Persona, true, UtilIdiomas.LanguageCode);

                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                notificacionCN.ActualizarNotificacion(mAvailableServices);
            }
        }

        private void ValidarCorreccionDeIdentidad()
        {
            if (AdministradorQuiereVerTodasLasPersonas && IdentidadPagina.Persona.FilaPersona.EstadoCorreccion != (short)EstadoCorreccion.NoCorreccion)
            {
                IdentidadPagina.Persona.FilaPersona.FechaNotificacionCorreccion = null;
                IdentidadPagina.Persona.FilaPersona.EstadoCorreccion = (short)EstadoCorreccion.NoCorreccion;
                PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                persCN.ActualizarPersonas();

                ControladorPersonas contrPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                contrPers.ActualizarModeloBaseSimple(IdentidadPagina, ProyectoAD.MyGnoss, UrlIntragnoss);
            }
        }

        private void EliminarPersona()
        {
            //Obtengo usuario:
            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            DataWrapperUsuario dataWrapperUsuario = usuCN.ObtenerUsuarioCompletoPorID(IdentidadPagina.Persona.UsuarioID);
            usuCN.Dispose();

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            IdentidadPagina.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerPerfilesDePersona(IdentidadPagina.PersonaID.Value, false, IdentidadPagina.FilaIdentidad.IdentidadID));
            identCN.Dispose();

            //Boqueo Usuario:
            dataWrapperUsuario.ListaUsuario.First().EstaBloqueado = true;

            //Borro ProyectoUsuarioIdentidad:
            List<AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad> listaProyectoUsuarioIdentidad = dataWrapperUsuario.ListaProyectoUsuarioIdentidad.ToList();
            foreach (AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad filaProyUsuIdent in listaProyectoUsuarioIdentidad)
            {
                dataWrapperUsuario.ListaProyectoUsuarioIdentidad.Remove(filaProyUsuIdent);
                mEntityContext.EliminarElemento(filaProyUsuIdent);
            }

            //Bloqueo ProyectoRolUsuario:
            foreach (AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyRolUsu in dataWrapperUsuario.ListaProyectoRolUsuario)
            {
                filaProyRolUsu.EstaBloqueado = true;
            }

            //Elimino Persona:
            IdentidadPagina.Persona.FilaPersona.Eliminado = true;

            //Borro perfiles:
            List<Guid> perfilesEliminados = new List<Guid>();
            foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in IdentidadPagina.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.PersonaID.Equals(IdentidadPagina.PersonaID)).ToList())
            {
                perfilesEliminados.Add(filaPerfil.PerfilID);
                filaPerfil.Eliminado = true;
            }

            //Pongo como expulsadas las identidades:
            List<Guid> listaProyectosEliminados = new List<Guid>();
            foreach (Guid perfilID in perfilesEliminados)
            {
                List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = IdentidadPagina.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(perfilID)).ToList();
                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in filasIdent)
                {
                    listaProyectosEliminados.Add(filaIdent.ProyectoID);
                    filaIdent.FechaExpulsion = DateTime.Now;
                    filaIdent.FechaBaja = DateTime.Now;
                }
            }

            #region Elimino Contactos

            List<Guid> listaContactosEliminados = new List<Guid>();

            foreach (Elementos.Identidad.Identidad amigo in IdentidadPagina.GestorAmigos.ListaContactos.Values)
            {
                listaContactosEliminados.Add(amigo.Clave);
                IdentidadPagina.GestorAmigos.EliminarAmigos(IdentidadPagina.IdentidadMyGNOSS, amigo.IdentidadMyGNOSS);
            }

            #endregion

            GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);
            gestorNotificaciones.AgregarNotificacionEliminacionDeUsuario(IdentidadPagina.Persona, UtilIdiomas.LanguageCode);

            //Guardo:
            mEntityContext.SaveChanges();

            //Actualizo Live:
            foreach (Guid proyectoID in listaProyectosEliminados)
            {
                ControladorDocumentacion.ActualizarGnossLive(proyectoID, IdentidadPagina.FilaIdentidad.PerfilID, AccionLive.Eliminado, (int)TipoLive.Miembro, false, PrioridadLive.Alta, mAvailableServices);
            }

            //Actualizo modelo base:
            ControladorPersonas controPer = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            foreach (Guid proyectoID in listaProyectosEliminados)
            {
                controPer.ActualizarEliminacionModeloBaseSimple(IdentidadPagina.PersonaID.Value, proyectoID, PrioridadBase.Alta, mAvailableServices);
            }

            //Limpio Caches:
            try
            {
                foreach (Guid perfilID in perfilesEliminados)
                {
                    //Invalido la cache de Mis comunidades
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    proyCL.InvalidarMisProyectos(perfilID);
                    proyCL.Dispose();
                }

                foreach (Guid proyectoID in listaProyectosEliminados)
                {
                    //Invalidamos la cache de amigos en la comunidad
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
                    amigosCL.InvalidarAmigosPertenecenProyecto(proyectoID);
                    amigosCL.Dispose();
                }

                foreach (Guid identidadID in listaContactosEliminados)
                {
                    //Limpiamos la cache de los contactos
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
                    amigosCL.InvalidarAmigos(identidadID);
                    amigosCL.Dispose();
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
        }

        private void CargarAccionesIdentidadPagina(ProfileModel pPerfilModel)
        {
            pPerfilModel.Actions.AddContactOrg = false;
            pPerfilModel.Actions.AddContact = false;

            if ((EsIdentidadActualAdministradorOrganizacion || IdentidadActual.ModoPersonal) && mControladorBase.UsuarioActual != null && (IdentidadPagina.Persona == null || !IdentidadPagina.Persona.Clave.Equals(IdentidadActual.Persona.Clave)))
            {
                if (!EsGnossOrganiza)
                {
                    if (EsIdentidadActualAdministradorOrganizacion && IdentidadActual.IdentidadOrganizacion != null)
                    {
                        pPerfilModel.Actions.AddContactOrg = true;

                        if (IdentidadActual.IdentidadOrganizacion.ListaPerfilesAmigos.Contains(IdentidadPagina.FilaIdentidad.PerfilID))
                        {
                            pPerfilModel.Actions.StatusContactOrg = ProfileModel.ActionsModel.StatusContact.Contact;
                        }
                        else
                        {
                            pPerfilModel.Actions.StatusContactOrg = ProfileModel.ActionsModel.StatusContact.NoContact;
                        }
                    }
                    if (IdentidadActual.ModoPersonal && !EsIdentidadActualAdministradorOrganizacion)
                    {
                        if (!(IdentidadActual.PerfilUsuario.OrganizacionID.HasValue && IdentidadActual.PerfilUsuario.PersonaID.HasValue && IdentidadPagina.PerfilUsuario.OrganizacionID.HasValue && IdentidadPagina.PerfilUsuario.PersonaID.HasValue))
                        {
                            pPerfilModel.Actions.AddContact = true;

                            if (IdentidadActual.ListaPerfilesAmigos.Contains(IdentidadPagina.FilaIdentidad.PerfilID))
                            {
                                pPerfilModel.Actions.StatusContactProfile = ProfileModel.ActionsModel.StatusContact.Contact;
                            }
                            else
                            {
                                pPerfilModel.Actions.StatusContactProfile = ProfileModel.ActionsModel.StatusContact.NoContact;
                            }
                        }
                    }
                }
            }

            Guid identMensaje = IdentidadPagina.Clave;

            if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.ProfesionalCorporativo))
            {
                if (!IdentidadActual.OrganizacionID.HasValue || !IdentidadPagina.OrganizacionID.Equals(IdentidadActual.OrganizacionID))
                {
                    if (IdentidadPagina.IdentidadOrganizacion != null)
                    {
                        identMensaje = IdentidadPagina.IdentidadOrganizacion.Clave;
                    }
                    else
                    {
                        IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        identMensaje = identCN.ObtenerIdentidadIDDeOrganizacionIDEnProyecto(IdentidadPagina.OrganizacionID.Value, ProyectoSeleccionado.Clave);
                        identCN.Dispose();
                    }
                }
            }

            pPerfilModel.Actions.SendMessage = true;
            ViewBag.IdentidadMensajeID = identMensaje;

            if (mControladorBase.UsuarioActual != null && IdentidadPagina.Persona != null && !IdentidadPagina.Persona.Clave.Equals(IdentidadActual.Persona.Clave))
            {
                bool verRecursos = false;

                AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona filasConfigPer = IdentidadPagina.Persona.FilaPersona.ConfiguracionGnossPersona;
                if (filasConfigPer != null)
                {
                    verRecursos = filasConfigPer.VerRecursos;
                }
                else
                {
                    if (IdentidadPagina.Tipo == TiposIdentidad.Personal || IdentidadPagina.Tipo == TiposIdentidad.Profesor)
                    {
                        PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                        verRecursos = persCN.PersonaPermiteVerRecursosAUsusariosGNOSS(IdentidadPagina.Persona.Clave);
                        persCN.Dispose();
                    }
                }

                if (ProyectoSeleccionado.Clave != ProyectoAD.MyGnoss)
                {
                    verRecursos = true;
                }

                if ((IdentidadPagina.Tipo == TiposIdentidad.Personal || IdentidadPagina.Tipo == TiposIdentidad.Profesor || IdentidadPagina.Tipo == TiposIdentidad.ProfesionalPersonal) && verRecursos && !EsGnossOrganiza)
                {
                    pPerfilModel.Actions.Follow = true;

                    bool siguiendo = false;
                    if (ProyectoVirtual.Clave != ProyectoAD.MyGnoss)
                    {
                        if (pPerfilModel.Actions.FollowingProfile)
                        {
                            siguiendo = true;
                        }
                        else
                        {
                            ControladorSuscripciones controladorSuscripciones = new ControladorSuscripciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorSuscripciones>(), mLoggerFactory);
                            controladorSuscripciones.CargarIdentidadesSuscritasEnProyecto(IdentidadActual, ProyectoVirtual.Clave);

                            if (IdentidadActual.ListaPerfilesSuscritos.Contains(IdentidadPagina.FilaIdentidad.PerfilID))
                            {
                                siguiendo = true;
                            }
                        }
                    }
                    else
                    {
                        List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.SeguirEnTodaLaActividad)).ToList();
                        bool SeguirEnTodaLaActividad = busqueda.Count > 0 && bool.Parse(busqueda.First().Valor);

                        pPerfilModel.Actions.Follow = SeguirEnTodaLaActividad;
                    }

                    if (pPerfilModel.Actions.Follow)
                    {
                        pPerfilModel.Actions.FollowingProfile = siguiendo;
                    }
                }
            }

            if (AdministradorQuiereVerTodasLasPersonas)
            {
                pPerfilModel.Actions.DeletePerson = true;

                if (IdentidadPagina.Persona.FilaPersona.EstadoCorreccion == (short)EstadoCorreccion.NotificadoNoCambiado3Dias)
                {
                    pPerfilModel.Actions.ReportDefinitiveCorrection = true;
                }
                else
                {
                    pPerfilModel.Actions.ReportCorrection = true;
                }

                if (IdentidadPagina.Persona.FilaPersona.EstadoCorreccion != (short)EstadoCorreccion.NoCorreccion)
                {
                    pPerfilModel.Actions.ValidateCorrection = true;
                }
            }
        }

        private void CargarActividadReciente(int pNumPagina)
        {
            ActividadReciente actividadReciente = new ActividadReciente(false, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mEnv, mAvailableServices, mLoggerFactory.CreateLogger<ActividadReciente>(), mLoggerFactory);

            Guid perfilIDPagina = IdentidadPagina.PerfilID;
            if (IdentidadPagina.EsOrganizacion)
            {
                perfilIDPagina = IdentidadPagina.OrganizacionID.Value;
            }
            else
            {
                perfilIDPagina = IdentidadPagina.PerfilID;
            }

            int filasPorPagina = 10;
            if (ParametroProyecto.ContainsKey("FilasPorPagina"))
            {
                Int32.TryParse(ParametroProyecto["FilasPorPagina"], out filasPorPagina);
            }
            else if (ParametrosAplicacionDS.Any(parametro => parametro.Parametro.Equals("FilasPorPagina")))
            {
                Int32.TryParse(ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("FilasPorPagina")).ToList().First().Valor, out filasPorPagina);
            }
            paginaModel.RecentActivity = actividadReciente.ObtenerActividadReciente(pNumPagina, filasPorPagina, TipoActividadReciente.PerfilProyecto, perfilIDPagina, IdentidadPagina.EsOrganizacion);
            paginaModel.PageType = ProfilePageViewModel.ProfilePageType.RecentActivity;
        }

        private void CargarPaginaRecursos()
        {
            paginaModel.PageType = ProfilePageViewModel.ProfilePageType.Resources;
        }

        /// <summary>
        /// Obtiene el RDF del documento de la BD, y devuele las entidades pricipales del mismo.
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <returns>Lista con las entidades pricipales leidas del archivo RDF leido de la BD y generado</returns>
        private List<ElementoOntologia> ObtenerRdfDeDocumento(GestorDocumental pGestorDoc, Guid pDocumentoID)
        {
            GestionOWL gestorOWL = new GestionOWL();
            gestorOWL.UrlOntologia = UrlOntologiaCurriculum;
            gestorOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_HR_XML;

            pGestorDoc.RdfDS = ControladorDocumentacion.ObtenerRDFDeBDRDF(pDocumentoID, ProyectoAD.MetaProyecto);

            string rdfTexto = string.Empty;

            if (pGestorDoc.RdfDS.RdfDocumento.Count > 0)
            {
                rdfTexto = pGestorDoc.ListaDocumentos[pDocumentoID].RdfSemantico;
            }
            else
            {
                pGestorDoc.RdfDS = null;
                byte[] bytesRdf = ControladorDocumentacion.ObtenerRDFDeVirtuoso(pDocumentoID, "Curriculum.owl", UrlIntragnoss, UrlOntologiaCurriculum, GestionOWL.NAMESPACE_ONTO_HR_XML, Ontologia);

                if (bytesRdf != null)
                {
                    MemoryStream buffer = new MemoryStream(bytesRdf);
                    StreamReader reader = new StreamReader(buffer);
                    rdfTexto = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }

            List<ElementoOntologia> instanciasPrincipales = null;

            if (!string.IsNullOrEmpty(rdfTexto))
            {
                instanciasPrincipales = gestorOWL.LeerFicheroRDF(Ontologia, rdfTexto, true);
            }

            return instanciasPrincipales;
        }

        /// <summary>
        /// Obtiene la ontologia
        /// </summary>
        public Ontologia Ontologia
        {
            get
            {
                if (mOntologia == null)
                {
                    Dictionary<string, List<EstiloPlantilla>> listaEstilos = new Dictionary<string, List<EstiloPlantilla>>();

                    string rutaOntologia = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UtilArchivos.ContentOntologias, "Curriculum.owl");
                    byte[] arrayOntologia = System.IO.File.ReadAllBytes(rutaOntologia);

                    mOntologia = new Ontologia(arrayOntologia, true);
                    mOntologia.LeerOntologia();
                    mOntologia.EstilosPlantilla = listaEstilos;
                    mOntologia.IdiomaUsuario = IdentidadActual.Persona.FilaPersona.Idioma;
                }
                return mOntologia;
            }
        }

        private void CargarGrupos()
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestionIdentidad = new GestionIdentidades(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadPagina.Clave, IdentidadActual.Clave == IdentidadPagina.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            List<GroupCardModel> listaGrupos = new List<GroupCardModel>();
            foreach (Elementos.Identidad.GrupoIdentidades grupo in gestionIdentidad.ListaGrupos.Values)
            {
                GroupCardModel fichaGrupo = new GroupCardModel();
                fichaGrupo.Clave = grupo.Clave;
                fichaGrupo.Name = grupo.Nombre;
                fichaGrupo.ShortName = grupo.NombreCorto;
                string urlGrupo = "";
                if (grupo.FilaGrupoProyecto != null)
                {
                    urlGrupo = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + grupo.NombreCorto;

                    if ((ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID) || (ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto) || grupo.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Any(item => item.IdentidadID.Equals(IdentidadActual.Clave) && item.GrupoID.Equals(grupo.Clave)))
                        && ParametrosGeneralesRow.ComunidadGNOSS)
                    {
                        fichaGrupo.AllowSendMessage = true;
                    }
                    if (IdentidadActual.Clave == IdentidadPagina.Clave)
                    {
                        fichaGrupo.AllowLeaveGroup = true;
                    }
                }
                else
                {
                    urlGrupo = GnossUrlsSemanticas.GetUrlAdministrarOrg(BaseURLIdioma, UtilIdiomas, UrlPerfil, UtilIdiomas.GetText("URLSEM", "GRUPO")) + "/" + grupo.NombreCorto;
                    fichaGrupo.AllowSendMessage = ParametrosGeneralesRow.ComunidadGNOSS;
                    fichaGrupo.AllowLeaveGroup = true;
                }
                fichaGrupo.UrlGroup = urlGrupo;

                listaGrupos.Add(fichaGrupo);
            }
            paginaModel.ProfileGroups = listaGrupos;
        }

        private void ObtenerFichaIdentidadModelDeIdentidadPagina()
        {
            ProfileModel fichaIdentidadPagina = new ProfileModel();
            if (IdentidadPagina == null)
            {
                fichaIdentidadPagina.Key = Guid.Empty;
                if (RequestParams("nombreCortoPerfil") != null)
                {
                    fichaIdentidadPagina.TypeProfile = ProfileType.Personal;
                    fichaIdentidadPagina.NamePerson = RequestParams("nombreCortoPerfil");
                }
                if (RequestParams("nombreCortoOrganizacion") != null)
                {
                    fichaIdentidadPagina.TypeProfile = ProfileType.Organization;
                    fichaIdentidadPagina.NamePerson = RequestParams("nombreCortoOrganizacion");
                }
            }
            else
            {
                fichaIdentidadPagina.Key = IdentidadPagina.Clave;

                fichaIdentidadPagina.UrlFoto = "/" + Es.Riam.Util.UtilArchivos.ContentImagenes + IdentidadPagina.FilaIdentidad.Foto.Replace("peque", "grande");
                if (string.IsNullOrEmpty(IdentidadPagina.FilaIdentidad.Foto) || IdentidadPagina.FilaIdentidad.Foto == "sinfoto")
                {
                    fichaIdentidadPagina.UrlFoto = "";
                }

                TiposIdentidad tipoIdentidadPublicador = IdentidadPagina.Tipo;

                fichaIdentidadPagina.TypeProfile = (ProfileType)(short)tipoIdentidadPublicador;

                string linkAPerfilPublicador = "";

                if (!string.IsNullOrEmpty(ProyectoSeleccionado.NombreCorto) && (ProyectoSeleccionado.Clave != ProyectoPrincipalUnico || !PerfilGlobalEnComunidadPrincipal))
                {
                    linkAPerfilPublicador = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/";
                }
                else if (UrlPerfil != null)
                {
                    linkAPerfilPublicador = BaseURLIdioma + UrlPerfil;
                }

                string linkOrg = "";

                if (tipoIdentidadPublicador == TiposIdentidad.ProfesionalPersonal || tipoIdentidadPublicador == TiposIdentidad.ProfesionalCorporativo || tipoIdentidadPublicador == TiposIdentidad.Organizacion)
                {
                    linkOrg = mControladorBase.UrlsSemanticas.ObtenerURLOrganizacionOClase(UtilIdiomas, IdentidadPagina.OrganizacionID.Value) + "/" + IdentidadPagina.OrganizacionPerfil.NombreCorto;

                    fichaIdentidadPagina.NameOrganization = IdentidadPagina.PerfilUsuario.NombreOrganizacion;
                    fichaIdentidadPagina.UrlOrganization = linkAPerfilPublicador + linkOrg;

                    if (tipoIdentidadPublicador == TiposIdentidad.ProfesionalCorporativo)
                    {
                        if ((IdentidadActual != null) && (IdentidadActual.OrganizacionID.HasValue && IdentidadPagina.OrganizacionID.Value == IdentidadActual.OrganizacionID.Value))
                        {
                            fichaIdentidadPagina.NamePerson = IdentidadPagina.PerfilUsuario.NombrePersonaEnOrganizacion;
                        }
                    }
                }
                if (tipoIdentidadPublicador == TiposIdentidad.Personal || tipoIdentidadPublicador == TiposIdentidad.Profesor || tipoIdentidadPublicador == TiposIdentidad.ProfesionalPersonal)
                {
                    string linkPers = UtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + IdentidadPagina.PerfilUsuario.NombreCortoUsu;

                    fichaIdentidadPagina.NamePerson = IdentidadPagina.PerfilUsuario.NombrePersonaEnOrganizacion;
                    if (tipoIdentidadPublicador == TiposIdentidad.ProfesionalPersonal)
                    {
                        fichaIdentidadPagina.UrlPerson = linkAPerfilPublicador + linkOrg + "/" + linkPers;
                    }
                    else
                    {
                        fichaIdentidadPagina.UrlPerson = linkAPerfilPublicador + linkPers;
                    }
                }
                if (tipoIdentidadPublicador == TiposIdentidad.Organizacion)
                {
                    fichaIdentidadPagina.IsClass = false;
                }

                if (string.IsNullOrEmpty(RequestParams("callback")))
                {
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    paginaModel.HasGrups = identidadCN.TieneIdentidadGrupos(ProyectoSeleccionado.Clave, IdentidadPagina.PerfilID);

                    CargarRedesSocialesIdentidadPagina(fichaIdentidadPagina);

                    CargarAccionesIdentidadPagina(fichaIdentidadPagina);
                }
            }

            paginaModel.ShowResources = MostrarRecursos;

            paginaModel.Profile = fichaIdentidadPagina;
            paginaModel.Profile.ListActions = new ProfileModel.UrlActions();
            paginaModel.Profile.ListActions.UrlFollow = paginaModel.Profile.UrlPerson + "/follow";
            paginaModel.Profile.ListActions.UrlUnfollow = paginaModel.Profile.UrlPerson + "/unfollow";

            if (IdentidadPagina.Tipo == TiposIdentidad.Organizacion || IdentidadPagina.Tipo == TiposIdentidad.ProfesionalCorporativo)
            {
                paginaModel.Profile.ListActions.UrlFollow = paginaModel.Profile.UrlOrganization + "/follow";
                paginaModel.Profile.ListActions.UrlUnfollow = paginaModel.Profile.UrlOrganization + "/unfollow";
            }
        }

        private void CargarRedesSocialesIdentidadPagina(ProfileModel pPerfilModel)
        {
            if (IdentidadPagina.ListaRedesSociales.Count > 0)
            {
                List<ProfileModel.SocialNetworkProfileModel> redesSocialesPerfilPagina = new List<ProfileModel.SocialNetworkProfileModel>();

                foreach (AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales filaRedSocial in IdentidadPagina.ListaRedesSociales.Values)
                {
                    ProfileModel.SocialNetworkProfileModel redSocialPerfil = new ProfileModel.SocialNetworkProfileModel();
                    string urlRedSocial = filaRedSocial.urlUsuario;
                    if (urlRedSocial.StartsWith("http://"))
                    {
                        urlRedSocial = urlRedSocial.Substring(7);
                    }
                    if (urlRedSocial.StartsWith("https://"))
                    {
                        urlRedSocial = urlRedSocial.Substring(8);
                    }
                    if (urlRedSocial.IndexOf('/') >= 0)
                    {
                        urlRedSocial = urlRedSocial.Substring(0, urlRedSocial.IndexOf('/'));
                    }

                    string urlFavicon = "http://" + urlRedSocial + "/favicon.ico";
                    if (filaRedSocial.NombreRedSocial == "Tuenti")
                    {
                        urlFavicon = "http://estaticosak1.tuenti.com/layout/web2-Zero/images/3_favicon.54771.png";
                    }
                    if (!UtilWeb.ExisteUrl(urlFavicon))
                    {
                        urlFavicon = BaseURLStatic + "/img/Logos_Redes_Sociales/logo_generico.gif";
                    }
                    string urlUsuario = filaRedSocial.urlUsuario;
                    if (!urlUsuario.StartsWith("http://") && !urlUsuario.StartsWith("https://"))
                    {
                        urlUsuario = "http://" + urlRedSocial;
                    }

                    redSocialPerfil.Name = filaRedSocial.NombreRedSocial;
                    redSocialPerfil.Url = urlRedSocial;
                    redSocialPerfil.UrlProfile = filaRedSocial.urlUsuario;
                    redSocialPerfil.UrlFavicon = urlFavicon;

                    redesSocialesPerfilPagina.Add(redSocialPerfil);
                }

                pPerfilModel.SocialNetworks = redesSocialesPerfilPagina;
            }
        }

        private void CargarRedesSocialesPerfil(ProfileModel pPerfilModel)
        {
            //Obtengo el perfil de la identidad
            Guid perfilID = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pPerfilModel.Key)).Select(item => item.PerfilID).FirstOrDefault();

            List<PerfilRedesSociales> listaRedesSocialesPerfil = mEntityContext.PerfilRedesSociales.Where(item => item.PerfilID.Equals(perfilID)).ToList();

            if (listaRedesSocialesPerfil.Count > 0)
            {
                List<ProfileModel.SocialNetworkProfileModel> redesSocialesPerfilPagina = new List<ProfileModel.SocialNetworkProfileModel>();

                foreach (PerfilRedesSociales filaRedSocial in listaRedesSocialesPerfil)
                {
                    ProfileModel.SocialNetworkProfileModel redSocialPerfil = new ProfileModel.SocialNetworkProfileModel();
                    string urlRedSocial = filaRedSocial.urlUsuario;
                    if (urlRedSocial.StartsWith("http://"))
                    {
                        urlRedSocial = urlRedSocial.Substring(7);
                    }
                    if (urlRedSocial.StartsWith("https://"))
                    {
                        urlRedSocial = urlRedSocial.Substring(8);
                    }
                    if (urlRedSocial.IndexOf('/') >= 0)
                    {
                        urlRedSocial = urlRedSocial.Substring(0, urlRedSocial.IndexOf('/'));
                    }

                    string urlFavicon = "http://" + urlRedSocial + "/favicon.ico";
                    if (filaRedSocial.NombreRedSocial == "Tuenti")
                    {
                        urlFavicon = "http://estaticosak1.tuenti.com/layout/web2-Zero/images/3_favicon.54771.png";
                    }
                    if (!UtilWeb.ExisteUrl(urlFavicon))
                    {
                        urlFavicon = BaseURLStatic + "/img/Logos_Redes_Sociales/logo_generico.gif";
                    }
                    string urlUsuario = filaRedSocial.urlUsuario;
                    if (!urlUsuario.StartsWith("http://") && !urlUsuario.StartsWith("https://"))
                    {
                        urlUsuario = "http://" + urlRedSocial;
                    }

                    redSocialPerfil.Name = filaRedSocial.NombreRedSocial;
                    redSocialPerfil.Url = urlRedSocial;
                    redSocialPerfil.UrlProfile = filaRedSocial.urlUsuario;
                    redSocialPerfil.UrlFavicon = urlFavicon;

                    redesSocialesPerfilPagina.Add(redSocialPerfil);
                }

                pPerfilModel.SocialNetworks = redesSocialesPerfilPagina;
            }
        }

        private void CargarPersonasPerfil()
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

            if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.Organizacion))
            {
                if (IdentidadPagina.OrganizacionPerfil.FilaOrganizacion.EsBuscable || IdentidadPagina.OrganizacionPerfil.FilaOrganizacion.EsBuscableExternos || EsIdentidadActualAdministradorOrganizacion)
                {
                    OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                    DataWrapperOrganizacion organizacionDW = organizacionCN.ObtenerOrganizacionPorID((Guid)IdentidadPagina.OrganizacionID);
                    organizacionCN.Dispose();

                    DataWrapperIdentidad dataWrapper = identidadCN.ObtenerIdentidadesDePersonasEnOrganizacionVisiblesEnProyecto((Guid)IdentidadPagina.OrganizacionID, IdentidadPagina.FilaIdentidad.ProyectoID);

                    GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapper, null, new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    List<ProfileModel> listaPersonasOrganizacion = new List<ProfileModel>();
                    if (IdentidadPagina.OrganizacionPerfil.FilaOrganizacion.EsBuscable || IdentidadPagina.OrganizacionPerfil.FilaOrganizacion.EsBuscableExternos)
                    {
                        foreach (Elementos.Identidad.Identidad ident in gestorIdentidades.ListaIdentidades.Values)
                        {
                            listaPersonasOrganizacion.Add(ObtenerFichaIdentidadModelDeIdentidad(ident));
                        }
                    }

                    paginaModel.PeopleInOrganization = listaPersonasOrganizacion;
                }
            }
        }

        private void CargarContactosPerfil()
        {
            bool visibilidadContactos = VisibilidadContactosDisponible();
            List<ProfileModel> listaContactosPerfil = new List<ProfileModel>();

            try
            {
                if (visibilidadContactos && IdentidadPagina.GestorAmigos.ListaContactos.Count > 0)
                {
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
                    List<Guid> listaIdentidades = amigosCL.ObtenerListaIdentidadesAmigosPertenecenProyecto(IdentidadPagina.IdentidadMyGNOSS.Clave, ProyectoSeleccionado.Clave);
                    amigosCL.Dispose();

                    foreach (Elementos.Identidad.Identidad contacto in IdentidadPagina.GestorAmigos.ListaContactos.Values)
                    {
                        if ((short)contacto.Tipo != 1 && (short)contacto.Tipo != 2 && listaIdentidades.Contains(contacto.Clave))
                        {
                            listaContactosPerfil.Add(ObtenerFichaIdentidadModelDeIdentidad(contacto));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (IdentidadPagina == null)
                {
                    GuardarLogError("IdentidadPagina es nulo");
                }
                else if (IdentidadPagina.GestorAmigos == null)
                {
                    GuardarLogError("IdentidadPagina.GestorAmigos es nulo");
                }
                else if (IdentidadPagina.GestorAmigos.ListaContactos == null)
                {
                    GuardarLogError("IdentidadPagina.GestorAmigos.ListaContactos. es nulo");
                }
            }

            paginaModel.ProfileContacts = listaContactosPerfil;
        }

        private void CargarSeguidoresPerfil(int numPeticion)
        {
            int numElementosMostrar = 20;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

            List<Guid> listaIdentidades = new List<Guid>();

            if (ProyectoSeleccionado.Clave == ProyectoAD.MyGnoss)
            {
                listaIdentidades = identidadCN.ObtenerListaIdentidadesSusucritasAPerfil(IdentidadPagina.PerfilID);
            }
            else
            {
                listaIdentidades = identidadCN.ObtenerListaIdentidadesIDSusucritasAPerfilEnProyecto(IdentidadPagina.PerfilID, ProyectoSeleccionado.Clave);
            }

            int numItem = 1;
            List<Guid> listaIdentidadesCargar = new List<Guid>();
            foreach (Guid ident in listaIdentidades)
            {
                if (numItem > ((numPeticion - 1) * numElementosMostrar))
                {
                    if (listaIdentidadesCargar.Count == numElementosMostrar) { break; }
                    listaIdentidadesCargar.Add(ident);
                }
                numItem++;
            }

            Dictionary<Guid, ProfileModel> listaSeguidoresPerfil = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdentidadesCargar);

            foreach (Guid seguidorID in listaSeguidoresPerfil.Keys)
            {
                CargarRedesSocialesPerfil(listaSeguidoresPerfil[seguidorID]);
                listaSeguidoresPerfil[seguidorID].Actions.Follow = true;
            }

            paginaModel.ProfileFollowers = listaSeguidoresPerfil.Values.ToList();
            paginaModel.NumProfileFollowers = listaIdentidades.Count;

            ControladorProyectoMVC.ObtenerInfoExtraIdentidadesPorID(listaSeguidoresPerfil);
        }

        private void CargarSeguidosPerfil(int numPeticion)
        {
            int numElementosMostrar = 20;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            List<Guid> listaIdentidades = new List<Guid>();

            if (ProyectoSeleccionado.Clave == ProyectoAD.MyGnoss)
            {
                listaIdentidades = identidadCN.ObtenerListaIdentidadesSusucritasPorPerfil(IdentidadPagina.PerfilID);
            }
            else
            {
                listaIdentidades = identidadCN.ObtenerListaIdentidadesSusucritasPorPerfilEnProyecto(IdentidadPagina.PerfilID, ProyectoSeleccionado.Clave);
            }

            int numItem = 1;
            List<Guid> listaIdentidadesCargar = new List<Guid>();
            foreach (Guid ident in listaIdentidades)
            {
                if (numItem > ((numPeticion - 1) * numElementosMostrar))
                {
                    if (listaIdentidadesCargar.Count == numElementosMostrar) { break; }
                    listaIdentidadesCargar.Add(ident);
                }
                numItem++;
            }

            Dictionary<Guid, ProfileModel> listaSeguidosPerfil = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdentidadesCargar);

            foreach (Guid seguidoID in listaSeguidosPerfil.Keys)
            {
                CargarRedesSocialesPerfil(listaSeguidosPerfil[seguidoID]);
                listaSeguidosPerfil[seguidoID].Actions.Follow = true;
            }

            paginaModel.ProfileFollowed = listaSeguidosPerfil.Values.ToList();
            paginaModel.NumProfileFollowed = listaIdentidades.Count;

            ControladorProyectoMVC.ObtenerInfoExtraIdentidadesPorID(listaSeguidosPerfil);
        }

        private bool VisibilidadContactosDisponible()
        {
            TipoVisibilidadContactosOrganizacion visibilidad = TipoVisibilidadContactosOrganizacion.Nadie;

            if (IdentidadPagina.Tipo == TiposIdentidad.Organizacion)
            {
                visibilidad = (TipoVisibilidadContactosOrganizacion)(IdentidadPagina.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaConfiguracionGnossOrg.Where(item => item.OrganizacionID.Equals(IdentidadPagina.OrganizacionID.Value)).Select(item => item.VisibilidadContactos).FirstOrDefault());
            }
            else if (!IdentidadPagina.TrabajaConOrganizacion)
            {
                AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona filaConf = IdentidadPagina.GestorIdentidades.GestorPersonas.DataWrapperPersonas.ListaConfigGnossPersona.FirstOrDefault(config => IdentidadPagina.PersonaID.HasValue && config.PersonaID.Equals(IdentidadPagina.PersonaID.Value));

                if (filaConf != null && filaConf.VerAmigos)
                {
                    visibilidad = TipoVisibilidadContactosOrganizacion.Contactos;

                    if (filaConf.VerAmigosExterno)
                    {
                        visibilidad = TipoVisibilidadContactosOrganizacion.ContactosDeContactos;
                    }
                }
            }

            bool contactosVisibles = false;
            if (visibilidad == TipoVisibilidadContactosOrganizacion.Contactos)
            {
                contactosVisibles = IdentidadPagina.ListaContactos.ContainsKey(IdentidadActual.IdentidadMyGNOSS.Clave) || (IdentidadPagina.IdentidadMyGNOSS.Clave == IdentidadActual.IdentidadMyGNOSS.Clave);
            }
            else if (visibilidad == TipoVisibilidadContactosOrganizacion.ContactosDeContactos)
            {
                AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
                contactosVisibles = amigosCN.ComprobarIdentidadAmigosDeAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave, IdentidadActual.IdentidadMyGNOSS.Clave) || (IdentidadPagina.IdentidadMyGNOSS.Clave == IdentidadActual.IdentidadMyGNOSS.Clave);
                amigosCN.Dispose();
            }

            return contactosVisibles;
        }

        private void CargarCurriculumModelDeIdentidadPagina()
        {
            CurriculumModel curriculum = new CurriculumModel();

            if (InformacionExtraPerfil != null)
            {
                DataRow[] filasInfoExtra = InformacionExtraPerfil.Tables["DatosPersonas"].Select("s='http://gnoss/" + IdentidadPagina.Clave + "'");

                if (filasInfoExtra.Length > 0)
                {
                    bool paisCargado = false;
                    bool provinciaCargada = false;

                    foreach (DataRow filaInfo in filasInfoExtra)
                    {
                        //Cargo el nombre del pais
                        if (!filaInfo.IsNull("countryname") && !string.IsNullOrEmpty(filaInfo["countryname"].ToString()) && !paisCargado)
                        {
                            string pais = filaInfo["countryname"].ToString();
                            curriculum.Countryname = pais;
                            paisCargado = true;
                        }

                        //Cargo el nombre de la provincia o estado
                        if (!filaInfo.IsNull("ProvinceOrState") && !string.IsNullOrEmpty(filaInfo["ProvinceOrState"].ToString()) && !provinciaCargada)
                        {
                            string provincia = filaInfo["ProvinceOrState"].ToString();
                            curriculum.ProvinceOrState = provincia;
                            provinciaCargada = true;
                        }

                        //Cargo los Puestos Actuales
                        if (!filaInfo.IsNull("PositionTitleEmpresaActual") && !string.IsNullOrEmpty(filaInfo["PositionTitleEmpresaActual"].ToString()))
                        {
                            curriculum.PositionTitleEmpresaActual = filaInfo["PositionTitleEmpresaActual"].ToString();
                            //if (!listaTrabajosActuales.Contains(puesto))
                            //{
                            //    listaTrabajosActuales.Add(puesto);
                            //}
                        }

                        //Cargo los Puestos Actuales
                        if (!filaInfo.IsNull("OrganizationNameEmpresaActual") && !string.IsNullOrEmpty(filaInfo["OrganizationNameEmpresaActual"].ToString()))
                        {
                            curriculum.CurrentOrganizationName = filaInfo["OrganizationNameEmpresaActual"].ToString();
                        }

                        ////Cargo los Tags de el CV de esta identidad
                        if (!filaInfo.IsNull("Tag") && !string.IsNullOrEmpty(filaInfo["Tag"].ToString()))
                        {
                            string tag = filaInfo["Tag"].ToString();

                            if (curriculum.ListTags == null)
                            {
                                curriculum.ListTags = new List<string>();
                            }

                            if (!curriculum.ListTags.Contains(tag))
                            {
                                curriculum.ListTags.Add(tag);
                            }
                        }
                    }
                }
            }

            if (Curriculum != null)
            {
                curriculum.Description = Curriculum.Description;
                curriculum.ListTags = UtilCadenas.SepararTexto(Curriculum.Tags);

                paginaModel.Curriculum = curriculum;
            }

            paginaModel.ShowDemographicsDataProfile = MostrarDatosDemograficosPerfil;
        }


        private Guid? CurriculumID
        {
            get
            {
                Guid? mCurriculumID = null;
                if (CVUnicoPorPerfil)
                {
                    if (IdentidadPagina.PerfilUsuario.FilaPerfil.CurriculumID.HasValue)
                    {
                        mCurriculumID = IdentidadPagina.PerfilUsuario.FilaPerfil.CurriculumID;
                    }
                }
                else
                {
                    if (IdentidadPagina.FilaIdentidad.CurriculumID.HasValue)
                    {
                        mCurriculumID = IdentidadPagina.FilaIdentidad.CurriculumID;
                    }
                }
                return mCurriculumID;
            }
        }

        private Curriculum Curriculum
        {
            get
            {
                if (CurriculumID.HasValue)
                {
                    return mEntityContext.Curriculum.FirstOrDefault(item => item.CurriculumID.Equals(CurriculumID.Value));
                }
                return null;
            }
        }

        /// <summary>
        /// Devuelve un perfil model con los datos necesarios a partir de una identidad.
        /// </summary>
        /// <param name="pIdentidad">Identidad de la que se van a obtener los datos.</param>
        /// <returns>PerfilModel con los datos necesarios.</returns>
        public ProfileModel ObtenerFichaIdentidadModelDeIdentidad(Elementos.Identidad.Identidad pIdentidad)
        {
            ProfileModel perfil = new ProfileModel();

            perfil.Key = pIdentidad.Clave;

            perfil.UrlFoto = "/" + Es.Riam.Util.UtilArchivos.ContentImagenes + pIdentidad.FilaIdentidad.Foto;
            if (string.IsNullOrEmpty(pIdentidad.FilaIdentidad.Foto) || pIdentidad.FilaIdentidad.Foto == "sinfoto")
            {
                perfil.UrlFoto = "";
            }

            TiposIdentidad tipoIdentidadPublicador = pIdentidad.Tipo;

            perfil.TypeProfile = (ProfileType)(short)tipoIdentidadPublicador;

            string linkAPerfilPublicador = "";

            if (!string.IsNullOrEmpty(mControladorBase.ProyectoSeleccionado.NombreCorto) && (mControladorBase.ProyectoSeleccionado.Clave != mControladorBase.ProyectoPrincipalUnico || !mControladorBase.PerfilGlobalEnComunidadPrincipal))
            {
                linkAPerfilPublicador = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto) + "/";
            }
            else if (mControladorBase.UrlPerfil != null)
            {
                linkAPerfilPublicador = mControladorBase.BaseURLIdioma + mControladorBase.UrlPerfil;
            }

            string linkOrg = "";

            if (tipoIdentidadPublicador == TiposIdentidad.ProfesionalPersonal || tipoIdentidadPublicador == TiposIdentidad.ProfesionalCorporativo || tipoIdentidadPublicador == TiposIdentidad.Organizacion)
            {
                linkOrg = mControladorBase.UrlsSemanticas.ObtenerURLOrganizacionOClase(mControladorBase.UtilIdiomas, pIdentidad.OrganizacionID.Value) + "/" + pIdentidad.PerfilUsuario.NombreCortoOrg;

                perfil.NameOrganization = pIdentidad.PerfilUsuario.NombreOrganizacion;
                perfil.UrlOrganization = linkAPerfilPublicador + linkOrg;

                if (tipoIdentidadPublicador == TiposIdentidad.ProfesionalCorporativo)
                {
                    if ((mControladorBase.IdentidadActual != null) && (mControladorBase.IdentidadActual.OrganizacionID.HasValue && pIdentidad.OrganizacionID.Value == IdentidadActual.OrganizacionID.Value))
                    {
                        perfil.NamePerson = pIdentidad.PerfilUsuario.NombrePersonaEnOrganizacion;
                    }
                }
            }
            if (tipoIdentidadPublicador == TiposIdentidad.Personal || tipoIdentidadPublicador == TiposIdentidad.Profesor || tipoIdentidadPublicador == TiposIdentidad.ProfesionalPersonal)
            {
                string linkPers = mControladorBase.UtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + pIdentidad.PerfilUsuario.NombreCortoUsu;

                perfil.NamePerson = pIdentidad.PerfilUsuario.NombrePersonaEnOrganizacion;
                if (tipoIdentidadPublicador == TiposIdentidad.ProfesionalPersonal)
                {
                    perfil.UrlPerson = linkAPerfilPublicador + linkOrg + "/" + linkPers;
                }
                else
                {
                    perfil.UrlPerson = linkAPerfilPublicador + linkPers;
                }
            }

            perfil.Rol = UserRol.User;

			IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
			perfil.Roles = new List<string>();
			List<Rol> rolesIdentidad = identidadCN.ObtenerRolesDeIdentidad(pIdentidad.Clave);
			if (rolesIdentidad != null && rolesIdentidad.Count > 0)
			{
				foreach (Rol rol in rolesIdentidad)
				{
					perfil.Roles.Add(UtilCadenas.ObtenerTextoDeIdioma(rol.Nombre, UtilIdiomas.LanguageCode, null));
				}
			}

			identidadCN.Dispose();

			if (!pIdentidad.EsOrganizacion && pIdentidad.Persona != null)
            {
                var filaAdmin = ProyectoSeleccionado.FilaProyecto.AdministradorProyecto.FirstOrDefault(f => f.UsuarioID.Equals(pIdentidad.Persona.UsuarioID));

                if (filaAdmin != null)
                {
                    perfil.Rol = (UserRol)filaAdmin.Tipo;
                }
            }

			UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
			bool tienePermiso = false;
			if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
			{
				utilPermisos.UsuarioTienePermisoAdministracionEcosistema((ulong)PermisoEcosistema.AdministrarMiembrosEcosistema, mControladorBase.UsuarioActual.UsuarioID);
			}
			else
			{
				tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoComunidad.GestionarMiembros, mControladorBase.IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Comunidad);
			}

			if (ProyectoSeleccionado.EsAdministradorUsuario(IdentidadActual.Persona.UsuarioID) || tienePermiso)
            {
                perfil.Actions = new ProfileModel.ActionsModel();

                // Si la identidad es de una organización que no sea con la que yo participo
                // o si no es el propio usuario
                if ((pIdentidad.EsOrganizacion && (!IdentidadActual.TrabajaPersonaConOrganizacion || !IdentidadActual.OrganizacionID.Equals(pIdentidad.OrganizacionID))) || (pIdentidad.Usuario != null && !ProyectoSeleccionado.EsAdministradorUsuario(pIdentidad.Usuario.Clave)))
                {
                    if (!pIdentidad.FilaIdentidad.FechaExpulsion.HasValue)
                    {
                        perfil.Actions.BlockUser = true;
                        perfil.Actions.ExpelMember = true;
                        if ((pIdentidad.EsOrganizacion && pIdentidad.OrganizacionPerfil?.FilaOrganizacion.OrganizacionParticipaProy.Count(filaProy => filaProy.ProyectoID == ProyectoSeleccionado.Clave) > 0 && pIdentidad.OrganizacionPerfil?.FilaOrganizacion.OrganizacionParticipaProy.First(filaProy => filaProy.ProyectoID == ProyectoSeleccionado.Clave)?.EstaBloqueada == true) ||
                            (!pIdentidad.EsOrganizacion && pIdentidad.Usuario?.FilaUsuario.ProyectoRolUsuario.Count(filaProy => filaProy.ProyectoID == ProyectoSeleccionado.Clave) > 0 && pIdentidad.Usuario?.FilaUsuario.ProyectoRolUsuario.First(filaProy => filaProy.ProyectoID == ProyectoSeleccionado.Clave)?.EstaBloqueado == true))
                        {
                            perfil.Actions.BlockUser = false;
                            perfil.Actions.UnblockUser = true;
                        }
                    }
                    else
                    {
                        perfil.Actions.ReadmitMember = true;
                    }
                }

                if (!pIdentidad.EsOrganizacion && !pIdentidad.FilaIdentidad.FechaExpulsion.HasValue)
                {
                    perfil.Actions.NotSendNewsletter = true;
                    perfil.Actions.ResetPassword = true;
                    if (!pIdentidad.FilaIdentidad.RecibirNewsLetter)
                    {
                        perfil.Actions.NotSendNewsletter = false;
                        perfil.Actions.SendNewsletter = true;
                    }

                    if (!pIdentidad.Clave.Equals(IdentidadActual.Clave) || ProyectoSeleccionado.ListaAdministradoresIDs.Count > 1)
                    {
                        perfil.Actions.ChangeRol = true;
                    }
                }
            }

            return perfil;
        }

        /// <summary>
        /// Carga la identidad que se va a pintar en la pagina
        /// </summary>
        private void CargarIdentidadPagina()
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            Guid identidadID = IdentidadActual.Clave;
            mExisteIdentidad = true;
            bool obtenerEliminados = ProyectoSeleccionado.EsAdministradorUsuario(IdentidadActual.Persona.UsuarioID);

            if (Request.Query.ContainsKey("com") && Request.Query["com"] != Guid.Empty.ToString() && string.IsNullOrEmpty(RequestParams("organizacion")))
            {
                //Si viene el parametro "com" estoy viendo mi perfil en una comunidad
                identidadID = identidadCN.ObtenerIdentidadIDDePersonaEnProyecto(new Guid(HttpUtility.UrlDecode(Request.Query["com"])), mControladorBase.UsuarioActual.PersonaID)[0];
            }
            else
            {
                Guid destinatarioID = Guid.Empty;

                if (RequestParams("nombreCortoPerfil") != null)
                {
                    string nombreCortoOrg = "";
                    if (RequestParams("nombreCortoOrganizacion") != null)
                    {
                        nombreCortoOrg = RequestParams("nombreCortoOrganizacion");
                    }

                    Guid[] listaIDs = identidadCN.ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(RequestParams("nombreCortoPerfil"), ProyectoSeleccionado.Clave, nombreCortoOrg, obtenerEliminados);

                    if (listaIDs != null && listaIDs[0] != null)
                    {
                        identidadID = listaIDs[0];
                    }
                    else
                    {
                        mExisteIdentidad = false;
                    }
                }
                else if (RequestParams("nombreCortoOrganizacion") != null)
                {
                    identidadID = identidadCN.ObtenerIdentidadIDDeOrganizacionEnProyecto(RequestParams("nombreCortoOrganizacion"), ProyectoSeleccionado.Clave);
                }
                else if (!string.IsNullOrEmpty(RequestParams("organizacion")))
                {
                    identidadID = IdentidadActual.IdentidadOrganizacion.Clave;
                }
            }

            if (mExisteIdentidad.Value)
            {

                DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerIdentidadPorID(identidadID, !obtenerEliminados);
                dataWrapperIdentidad.Merge(identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(identidadID));
                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = dataWrapperIdentidad.ListaIdentidad.FirstOrDefault(identidad => identidad.IdentidadID.Equals(identidadID));

                //Si no hay fila, hay que consultar BD Master por si acaso no está en una réplica:
                if (filaIdentidad == null)
                {
                    dataWrapperIdentidad = identidadCN.ObtenerIdentidadPorID(identidadID, !obtenerEliminados);
                    filaIdentidad = dataWrapperIdentidad.ListaIdentidad.FirstOrDefault(identidad => identidad.IdentidadID.Equals(identidadID));
                }

                if (filaIdentidad != null)
                {
                    dataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesDePerfil(filaIdentidad.PerfilID));

                    DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
                    //Si no es una organización ni un perfil corporativo, cargamos la persona.
                    if (filaIdentidad.Tipo != (short)TiposIdentidad.Organizacion && filaIdentidad.Tipo != (short)TiposIdentidad.ProfesionalCorporativo)
                    {
                        PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);

                        dataWrapperPersona = personaCN.ObtenerPersonasPorIdentidad(identidadID);
                        personaCN.Dispose();
                    }
                    GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);

                    DataWrapperOrganizacion organizacionDW = new DataWrapperOrganizacion();
                    //Si es una organización o un perfil profesional, cargamos también los datos de la organización
                    if (filaIdentidad.Tipo != (short)TiposIdentidad.Personal && filaIdentidad.Tipo != (short)TiposIdentidad.Profesor)
                    {
                        OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                        organizacionDW = organizacionCN.ObtenerOrganizacionesPorIdentidad(identidadID);
                        organizacionCN.Dispose();

                        dataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadDeOrganizacion(filaIdentidad.Perfil.OrganizacionID.Value, ProyectoSeleccionado.Clave, true));
                    }
                    GestionOrganizaciones gestorOrganizaciones = new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext);

                    GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, gestorPersonas, gestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    IdentidadPagina = gestorIdentidades.ListaIdentidades[identidadID];

                    if (!filaIdentidad.FechaBaja.HasValue && !filaIdentidad.FechaExpulsion.HasValue)
                    {
                        ControladorAmigos controladorAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory);
                        controladorAmigos.CargarAmigos(IdentidadActual, EsIdentidadActualAdministradorOrganizacion, mAvailableServices);

                        FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mControladorBase.UsuarioActual.ProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);

                        if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.Organizacion))
                        {
                            InformacionExtraPerfil = facetadoCN.ObtieneInformacionPersonas(IdentidadPagina.FilaIdentidad.ProyectoID, IdentidadPagina.OrganizacionID.Value);
                            controladorAmigos.CargarAmigos(IdentidadPagina.IdentidadOrganizacion, EsIdentidadActualAdministradorOrganizacion, mAvailableServices);
                        }
                        else
                        {
                            InformacionExtraPerfil = facetadoCN.ObtieneInformacionPersonas(IdentidadPagina.FilaIdentidad.ProyectoID, IdentidadPagina.Clave);
                            controladorAmigos.CargarAmigos(IdentidadPagina, EsIdentidadActualAdministradorOrganizacion, mAvailableServices);
                        }
                        InformacionExtraPerfil = facetadoCN.ObtieneInformacionPersonas(IdentidadPagina.FilaIdentidad.ProyectoID, IdentidadPagina.Clave);

                        facetadoCN.Dispose();
                    }
                }
            }
        }

        private void AgregarMensajeABase(Guid pCorreoID, Guid pIdentidadRemitenteID, List<Guid> pListaDestinatarios, PrioridadBase pPrioridadBase)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(ProyectoAD.MetaProyecto);
            proyCL.Dispose();

            string destinatarios = "";
            foreach (Guid destinatario in pListaDestinatarios)
            {
                destinatarios += $"|{destinatario.ToString()}";
            }

            if (destinatarios.StartsWith("|")) destinatarios = destinatarios.Substring(1);

            //Agregamos peticiones a la cola
            ControladorCorreo.AgregarElementoARabbitMQ(id, pCorreoID, destinatarios, pIdentidadRemitenteID, pPrioridadBase, mAvailableServices);
        }

        #endregion

        #region Propiedades

        private Elementos.Identidad.Identidad IdentidadPagina
        {
            get
            {
                if (mIdentidadPagina == null && !mExisteIdentidad.HasValue)
                {
                    CargarIdentidadPagina();
                }
                return mIdentidadPagina;
            }
            set
            {
                mIdentidadPagina = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la Información extra del Perfil
        /// </summary>
        public DataSet InformacionExtraPerfil
        {
            get
            {
                return mInformacionExtraPerfil;
            }
            set
            {
                mInformacionExtraPerfil = value;
            }
        }

        public string UrlPerfilPagina
        {
            get
            {
                string urlPerfil = "/";

                try
                {
                    if (IdentidadPagina != null && (IdentidadPagina.TrabajaConOrganizacion || IdentidadPagina.EsIdentidadProfesor))
                    {
                        string nombreCorto = IdentidadPagina.PerfilUsuario.NombreCortoOrg;

                        if (IdentidadPagina.EsIdentidadProfesor)
                        {
                            nombreCorto = IdentidadPagina.PerfilUsuario.NombreCortoUsu;
                        }

                        urlPerfil += UtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + nombreCorto + "/";
                    }
                    else if (IdentidadPagina != null)
                    {
                        string nombreCorto = IdentidadPagina.PerfilUsuario.NombreCortoUsu;
                        urlPerfil += UtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + nombreCorto + "/";
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                }
                return urlPerfil;
            }
        }

        private bool MostrarRecursos
        {
            get
            {
                if (!mMostrarRecursos.HasValue)
                {
                    mMostrarRecursos = true;

                    if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                    {
                        if (/*EsEcosistemaSinMetaProyecto || */IdentidadPagina == null)
                        {
                            mMostrarRecursos = false;
                        }
                        else if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
                        {
                            #region Comprobamos privacidad recursos para invitado
                            if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.Organizacion))
                            {
                                if (IdentidadPagina.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaConfiguracionGnossOrg.Any(item => item.OrganizacionID.Equals(IdentidadPagina.OrganizacionID.Value)))
                                {
                                    mMostrarRecursos = (IdentidadPagina.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaConfiguracionGnossOrg.FirstOrDefault(item => item.OrganizacionID.Equals(IdentidadPagina.OrganizacionID.Value))).VerRecursosExterno;
                                }
                            }
                            else
                            {
                                AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona confGnossPersona = IdentidadPagina.GestorIdentidades.GestorPersonas.DataWrapperPersonas.ListaConfigGnossPersona.FirstOrDefault(persona => persona.PersonaID.Equals(IdentidadPagina.PersonaID));
                                if (confGnossPersona != null)
                                {
                                    mMostrarRecursos = confGnossPersona.VerRecursosExterno;
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Comprobamos privacidad recursos para usuario
                            if (IdentidadPagina.ModoParticipacion.Equals(TiposIdentidad.Organizacion))
                            {
                                if (IdentidadPagina.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaConfiguracionGnossOrg.Any(item => item.OrganizacionID.Equals(IdentidadPagina.OrganizacionID.Value)))
                                {
                                    mMostrarRecursos = (IdentidadPagina.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaConfiguracionGnossOrg.FirstOrDefault(item => item.OrganizacionID.Equals(IdentidadPagina.OrganizacionID.Value))).VerRecursos;
                                }
                            }
                            else
                            {
                                AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona configGnossPersona = IdentidadPagina.GestorIdentidades.GestorPersonas.DataWrapperPersonas.ListaConfigGnossPersona.FirstOrDefault(persona => persona.PersonaID.Equals(IdentidadPagina.PersonaID));
                                if (configGnossPersona != null)
                                {
                                    mMostrarRecursos = configGnossPersona.VerRecursos;
                                }
                            }

                            #endregion
                        }
                    }

                    if (mMostrarRecursos.Value && ListaPermisosDocumentos.Contains(TiposDocumentacion.Debate))
                    {
                        IdentidadCN identiCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        int numDebates = 0;
                        if (IdentidadPagina.EsOrganizacion && IdentidadPagina.IdentidadOrganizacion.OrganizacionID.HasValue)
                        {
                            numDebates = identiCN.ObtenerNumDebatesDeIdentidadesDeOrgEnProyecto(IdentidadPagina.IdentidadOrganizacion.OrganizacionID.Value, ProyectoSeleccionado.Clave);
                        }
                        else
                        {
                            numDebates = identiCN.ObtenerNumDebatesDeIdentidadEnProyecto(IdentidadPagina.Clave, ProyectoSeleccionado.Clave);
                        }
                        ViewBag.MostrarDebates = numDebates > 0;
                    }

                }
                return mMostrarRecursos.Value;
            }
        }

        #endregion

    }
}
