using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class ContactosController : ControllerBaseWeb
    {
        private IAvailableServices mAvailableServices;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ContactosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<ContactosController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mAvailableServices = availableServices;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EsPaginaEdicion = true;
            base.OnActionExecuting(filterContext);
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            string grafo = mControladorBase.UsuarioActual.UsuarioID.ToString();
            Guid identidadBusqueda = Guid.Empty;

            if (RequestParams("organizacion") == "true")
            {
                grafo = IdentidadActual.OrganizacionID.Value.ToString();
                identidadBusqueda = IdentidadActual.IdentidadOrganizacion.Clave;
            }

            string javascriptAdicional = "primeraCargaDeFacetas = false;";

            insertarScriptBuscador("divFac", "divRel", "divNumResultadosBusqueda", "panListadoFiltrosPulgarcito", "divRel", "divFiltros", grafo, "navegadorBusqueda", "", "", null, "", "", "", javascriptAdicional, identidadBusqueda, ProyectoSeleccionado.Clave, null, (short)TipoBusqueda.Contactos, null, null);

            NotificacionCN notCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
            int numInvitaciones = notCN.ObtenerNumeroInvitacionesPendientesDeMyGnoss(mControladorBase.UsuarioActual.PersonaID, IdentidadActual.OrganizacionID, RequestParams("organizacion") == "true");
            notCN.Dispose();

            return View(numInvitaciones);
        }

        [HttpPost]
        public ActionResult CreateGroup(string GroupName)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            bool existe = false;
            GrupoAmigos grupo = null;
            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
            /*TODO Javier migrar
            AmigosDS amigosDS = amigosCN.ObtenerAmigosDeIdentidad(mControladorBase.UsuarioActual.IdentidadID);
            amigosCN.Dispose();

            GestionAmigos pGestionAmigos = new GestionAmigos(amigosDS, mLoggingService, mEntityContext, mConfigService);

            foreach (GrupoAmigos ga in pGestionAmigos.ListaGrupoAmigos.Values)
            {
                if (ga.Nombre == GroupName)
                {
                    existe = true;
                }
            }

            if (!existe && GroupName != "")
            {
                Guid identidad = IdentidadActual.IdentidadMyGNOSS.Clave;
                FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD);

                if (RequestParams("organizacion") == "true")
                {
                    grupo = pGestionAmigos.AgregarGrupoAmigos(IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS.Clave, GroupName);
                    identidad = IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
                }
                else
                {
                    grupo = pGestionAmigos.AgregarGrupoAmigos(IdentidadActual.IdentidadMyGNOSS.Clave, GroupName);
                }

                mControladorBase.UsuarioActual.UsarMasterParaLectura = true;

                facetadoCN.InsertarNuevoGrupoContactos(identidad, grupo.Clave, GroupName);
                facetadoCN.Dispose();

                ControladorAmigos.GuardarAmigos(amigosDS, IdentidadActual, RequestParams("organizacion") == "true", EsAdministrador(IdentidadActual));

                return GnossResultOK(UtilIdiomas.GetText("CONTACTOS", "CREARGRUPOOK"));
            }*/
            return GnossResultERROR(UtilIdiomas.GetText("CONTACTOS", "CREARGRUPOERR"));
        }

        [HttpPost]
        public ActionResult DeleteContact(Guid ContactID)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            /*TODO Javier migrar
            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService);
            AmigosDS amigosDS = amigosCN.ObtenerAmigosDeIdentidad(IdentidadActual.IdentidadMyGNOSS.Clave);
            GestionAmigos pGestionAmigos = new GestionAmigos(amigosDS, mLoggingService, mEntityContext, mConfigService);

            //Eliminamos de Virtuoso
            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD);
            facetadoCN.BorrarContacto(IdentidadActual.IdentidadMyGNOSS.Clave, ContactID);
            facetadoCN.Dispose();

            //Eliminamos del Acido
            //Elimino a ÉL de MIS contactos                
            pGestionAmigos.EliminarContacto(ContactID);
            mEntityContext.SaveChanges();

            //Me elimino de SUS contactos
            amigosDS = amigosCN.ObtenerAmigosDeIdentidad(ContactID);
            pGestionAmigos = new GestionAmigos(amigosDS, mLoggingService, mEntityContext, mConfigService);
            pGestionAmigos.EliminarContacto(IdentidadActual.Clave);
            mEntityContext.SaveChanges();

            //Agrego una fila al base
            ControladorContactos contrContactos = new ControladorContactos(mLoggingService, mEntityContext, mConfigService, mEntityContextBASE, mRedisCacheWrapper, mVirtuosoAD);
            contrContactos.ActualizarEliminacionModeloBaseSimple(IdentidadActual.Clave, ContactID);
            */
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteGroup(Guid GroupID)
        {
            /*TODO Javier
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }


            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService);
            AmigosDS amigosDS = amigosCN.ObtenerGrupoYMiembros(GroupID);

            //Eliminar el contacto de la lista de usuarios con permisos.
            ControladorContactos contrContactos = new ControladorContactos(mLoggingService, mEntityContext, mConfigService, mEntityContextBASE, mRedisCacheWrapper, mVirtuosoAD);
            GestionAmigos pGestionAmigos = new GestionAmigos(amigosDS, IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService);

            //Eliminamos del Acido
            GrupoAmigos grupoEliminar = pGestionAmigos.ListaGrupoAmigos[GroupID];
            string nombreGrupo = grupoEliminar.Nombre;
            pGestionAmigos.EliminarGrupoAmigos(grupoEliminar);
            ControladorAmigos.GuardarAmigos(pGestionAmigos.AmigosDS, IdentidadActual, IdentidadActual.EsOrganizacion, EsIdentidadActualAdministradorOrganizacion);

            //Eliminamos de Virtuoso
            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD);
            facetadoCN.BorrarGrupoContactos(IdentidadActual.IdentidadMyGNOSS.Clave, GroupID, nombreGrupo);
            facetadoCN.Dispose();

            //Insertamos una fila al BASE
            contrContactos.ActualizarEliminacionModeloBaseSimple(IdentidadActual.Clave, GroupID);
            */
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteMemberGroup(Guid ContactID, Guid GroupID)
        {
            /* TODO Javier migrar
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }


            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService);
            AmigosDS amigosDS = amigosCN.ObtenerGrupoYMiembros(GroupID);

            //Eliminar el contacto de la lista de usuarios con permisos.
            ControladorContactos contrContactos = new ControladorContactos(mLoggingService, mEntityContext, mConfigService, mEntityContextBASE, mRedisCacheWrapper, mVirtuosoAD);
            
            GestionAmigos pGestionAmigos = new GestionAmigos(amigosDS, IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService);

            AmigosDS.GrupoAmigosRow pFilaGrupoAmigos = (AmigosDS.GrupoAmigosRow)amigosDS.GrupoAmigos.Rows.Find(GroupID);
            GrupoAmigos pGrupoAmigos = new GrupoAmigos(pFilaGrupoAmigos, pGestionAmigos, mLoggingService);

            pGestionAmigos.EliminarAmigoDeGrupo(pGrupoAmigos, ContactID);
            mEntityContext.SaveChanges();
            */
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ChangeGroupName(Guid GroupID, string GroupName)
        {
            /*TODO Javier migrar
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }


            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService);
            bool existe = false;

            AmigosDS amigosDS = amigosCN.ObtenerAmigosDeIdentidad(IdentidadActual.Clave);
            GestionAmigos pGestionAmigos = new GestionAmigos(amigosDS, IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService);

            foreach (GrupoAmigos ga in pGestionAmigos.ListaGrupoAmigos.Values)
            {
                if (ga.Nombre == GroupName)
                {
                    existe = true;
                }
            }

            if (!existe)
            {
                GrupoAmigos grupo = pGestionAmigos.ListaGrupoAmigos[GroupID];
                grupo.Nombre = GroupName;

                mEntityContext.SaveChanges();

                FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD);
                facetadoCN.ModificarGrupoContactos(grupo.Clave.ToString(), grupo.Nombre);
                facetadoCN.Dispose();
            }
            else
            {
                return GnossResultERROR(UtilIdiomas.GetText("CONTACTOS", "CREARGRUPOERR"));
            }
            */
            return GnossResultOK();
        }

        [HttpPost]
        public ActionResult SendMessage(Guid ContactID, string Subject, string Body)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }


            Body = HttpUtility.HtmlDecode(Body);

            Guid autor = IdentidadActual.IdentidadMyGNOSS.Clave;

            GestionCorreo gestionCorreo = new GestionCorreo(new CorreoDS(), null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<GestionCorreo>(), mLoggerFactory);
            gestionCorreo.IdentidadActual = IdentidadActual;
            gestionCorreo.GestorIdentidades = IdentidadActual.GestorIdentidades;

            List<Guid> listaDestinatarios = new List<Guid>();
            listaDestinatarios.Add(ContactID);

            Guid correoID;
            if (!EsEcosistemaSinMetaProyecto)
            {
                correoID = gestionCorreo.AgregarCorreo(autor, listaDestinatarios, Subject, Body, BaseURL, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
            }
            else
            {
                correoID = gestionCorreo.AgregarCorreo(autor, listaDestinatarios, Subject, Body, BaseURL, ProyectoVirtual, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
            }

            CorreoCN actualizarCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);
            actualizarCN.ActualizarCorreo(gestionCorreo.CorreoDS);

            string destinatarios = "";
            foreach (Guid destinatario in listaDestinatarios)
            {
                destinatarios += "|" + destinatario.ToString();
            }

            if (destinatarios.StartsWith("|")) destinatarios = destinatarios.Substring(1);

            ControladorDocumentacion.AgregarMensajeFacModeloBaseSimple(correoID, autor, ProyectoSeleccionado.Clave, "base", destinatarios, null, PrioridadBase.Alta, mAvailableServices);

            return GnossResultOK();
        }

        [HttpPost]
        public ActionResult SendMessageGroup(Guid ContactID, string Subject, string Body)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }


            Body = HttpUtility.HtmlDecode(Body);

            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
            /*TODO Javier migrar
            GestionAmigos gestorAmigos = new GestionAmigos(amigosCN.ObtenerAmigosDeIdentidad(IdentidadActual.Clave), IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService);

            ControladorAmigos contrAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor);
            contrAmigos.CargarAmigos(IdentidadActual, EsIdentidadActualAdministradorOrganizacion);

            Guid autor = IdentidadActual.IdentidadMyGNOSS.Clave;

            GestionCorreo gestionCorreo = new GestionCorreo(new CorreoDS(), null, null, null, mLoggingService, mEntityContext, mConfigService);
            gestionCorreo.IdentidadActual = IdentidadActual;
            gestionCorreo.GestorIdentidades = IdentidadActual.GestorIdentidades;

            List<Guid> listaDestinatarios = new List<Guid>();


            GrupoAmigos grupoAmigo = null;
            if (IdentidadActual.Tipo.Equals(TiposIdentidad.Profesor))
            {
                //Si la identidad es la de un profesor, debemos obtener el grupoAmigos de la clase a la que se quiera enviar un mensaje.
                if (!gestorAmigos.ListaGrupoAmigos.ContainsKey(ContactID))
                {
                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService);

                    AmigosDS amigosDS = amigosCN.ObtenerGrupoAmigosProfesor(IdentidadActual.Clave);
                    foreach (AmigosDS.GrupoAmigosRow dr in amigosDS.GrupoAmigos.Select("GrupoID = '" + ContactID + "'"))
                    {
                        grupoAmigo = new GrupoAmigos(dr, gestorAmigos, mLoggingService);
                    }
                }
                else
                {
                    grupoAmigo = gestorAmigos.ListaGrupoAmigos[ContactID];
                }
            }
            else
            {
                grupoAmigo = gestorAmigos.ListaGrupoAmigos[ContactID];
            }
           
            if (grupoAmigo.Clave.Equals(ContactID))
            {
                foreach (Identidad idAmigo in grupoAmigo.ListaAmigos.Values)
                {
                    if (!idAmigo.EsOrganizacion)
                    {
                        if (gestorAmigos.EsAmigoRealDeIdentidad(IdentidadActual, idAmigo, false, EsIdentidadActualAdministradorOrganizacion))
                            listaDestinatarios.Add(idAmigo.IdentidadMyGNOSS.Clave);
                    }
                    else
                    {
                        if (gestorAmigos.EsAmigoRealDeIdentidad(IdentidadActual, idAmigo, true, EsIdentidadActualAdministradorOrganizacion))
                            listaDestinatarios.Add(idAmigo.IdentidadMyGNOSS.Clave);
                    }
                }
            }

            if (listaDestinatarios.Count > 0)
            {
                Guid correoID;
                if (!EsEcosistemaSinMetaProyecto)
                {
                    correoID = gestionCorreo.AgregarCorreo(autor, listaDestinatarios, Subject, Body, BaseURL, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                }
                else
                {
                    correoID = gestionCorreo.AgregarCorreo(autor, listaDestinatarios, Subject, Body, BaseURL, ProyectoVirtual, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                }

                CorreoCN actualizarCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService);
                actualizarCN.ActualizarCorreo(gestionCorreo.CorreoDS);

                foreach (Guid destinatario in listaDestinatarios)
                {
                    ControladorDocumentacion.AgregarMensajeFacModeloBaseSimple(correoID, autor, ProyectoSeleccionado.Clave, "base", destinatario.ToString(), null, PrioridadBase.Alta);
                }
            }
             */
            return GnossResultOK();
        }

        [HttpPost]
        public ActionResult AddContactsGroup(Guid GroupID, string ContactsList)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            string[] listaContactos = ContactsList.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
            /* TODO Javier migrar
            AmigosDS amigosDS = amigosCN.ObtenerGrupoYMiembros(GroupID);
            GestionAmigos gestionAmigos = new GestionAmigos(amigosDS, mLoggingService, mEntityContext, mConfigService);

            GrupoAmigos grupoAmigos = gestionAmigos.ListaGrupoAmigos[GroupID];

            foreach (string contacto in listaContactos)
            {
                Guid idContacto = Guid.Empty;

                if (Guid.TryParse(contacto, out idContacto))
                {
                    gestionAmigos.AgregarAmigoAGrupo(grupoAmigos, IdentidadActual.Clave, idContacto, false);
                }
            }

            mEntityContext.SaveChanges();
            */
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult AddGroupsContact(Guid ContactID, string GroupsList)
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }


            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);

            string[] listaGrupos = GroupsList.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string grupo in listaGrupos)
            {
                Guid idGrupo = Guid.Empty;

                if (Guid.TryParse(grupo, out idGrupo))
                {
                    /*TODO Javier migrar AmigosDS amigosDS = amigosCN.ObtenerGrupoYMiembros(idGrupo);
                     GestionAmigos pGestionAmigos = new GestionAmigos(amigosDS, IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService);
                     AmigosDS.GrupoAmigosRow pFilaGrupoAmigos = (AmigosDS.GrupoAmigosRow)amigosDS.GrupoAmigos.Rows.Find(idGrupo);
                     GrupoAmigos pGrupoAmigos = new GrupoAmigos(pFilaGrupoAmigos, pGestionAmigos, mLoggingService);

                     pGestionAmigos.AgregarAmigoAGrupo(pGrupoAmigos, IdentidadActual.Clave, ContactID, false);
                    */
                    mEntityContext.SaveChanges();
                }
            }

            return new EmptyResult();
        }
    }
}
