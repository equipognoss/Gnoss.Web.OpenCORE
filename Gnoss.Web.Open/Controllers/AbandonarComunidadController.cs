using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class AbandonarComunidadController : ControllerBaseWeb
    {

        public AbandonarComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {

        }


        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada || IdentidadActual.TrabajaConOrganizacion)
            {
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            return View(AbandonandoPlataforma);
        }

        [HttpPost]
        public ActionResult Confirmar()
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada || IdentidadActual.TrabajaConOrganizacion)
            {
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            bool error = false;

            if (AbandonandoPlataforma)
            {
                string pass = RequestParams("password");

                bool contrasenyaCorrecta;
                JsonEstado jsonEstado = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.LoginConRegistro, ProyectoAD.MetaProyecto, mControladorBase.UsuarioActual.UsuarioID, null, null, IdentidadActual.Persona.FilaPersona.Email, pass, GestorParametroAplicacion, null, null, null, null);

                if (jsonEstado != null)
                {
                    contrasenyaCorrecta = jsonEstado.Correcto;
                }
                else if (string.IsNullOrEmpty(pass))
                {
                    contrasenyaCorrecta = false;
                    error = true;
                }
                else
                {
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    contrasenyaCorrecta = usuarioCN.ValidarPasswordUsuario(usuarioCN.ObtenerUsuarioPorID(UsuarioActual.UsuarioID), pass);
                    usuarioCN.Dispose();
                }

                if (contrasenyaCorrecta)
                {
                    JsonEstado jsonEstadoInvalidar = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.InvalidarUsuario, ProyectoAD.MetaProyecto, mControladorBase.UsuarioActual.UsuarioID, null, null, IdentidadActual.Persona.FilaPersona.Email, pass, GestorParametroAplicacion, null, null, null, null);

                    if (jsonEstado == null)
                    {
                        error = AbandonarPlataforma();
                    }
                    else
                    {
                        if (jsonEstado.Correcto)
                        {
                            error = AbandonarPlataforma();
                        }
                        else
                        {
                            error = true;
                        }
                    }
                }
            }
            else
            {
                error = ControladorIdentidades.AbandonarComunidad(mControladorBase.UsuarioActual.ProyectoID, mControladorBase.UsuarioActual.UsuarioID, mControladorBase.UsuarioActual.IdentidadID, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.PerfilID);
            }

            if (!error)
            {
                string sesionIdentidadID = string.Format("IdentidadIDdePersonaEnProyecto_{0}_{1}", ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.PersonaID);
                HttpContext.Session.Remove(sesionIdentidadID);

                if (AbandonandoPlataforma)
                {
                    //return new RedirectResult(DominoAplicacionIdioma.Replace("https://", "http://") + "/" + UtilIdiomas.GetText("URLSEM", "DESCONECTAR"));
                    return GnossResultOK(DominoAplicacionIdioma.Replace("https://", "http://") + "/" + UtilIdiomas.GetText("URLSEM", "DESCONECTAR"));
                }
                else
                {
                    return GnossResultOK(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
            }
            UtilIdiomas.GetText("COMUNIDADES", "ABANDONARCOMUNIDADERROR");
            return GnossResultERROR("Ha ocurrido un error, comprube que no sea el administrador de la comunidad");
        }

        private bool AbandonarPlataforma()
        {
            //Obtengo usuario:
            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperUsuario dataWrapperUsuario = usuCN.ObtenerUsuarioCompletoPorID(IdentidadActual.Persona.UsuarioID);
            usuCN.Dispose();

            //Boqueo Usuario:
            dataWrapperUsuario.ListaUsuario.FirstOrDefault().EstaBloqueado = true;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            //Borro ProyectoUsuarioIdentidad:
            List<AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad> listaProyectoUsuarioIdentidadBorrar = dataWrapperUsuario.ListaProyectoUsuarioIdentidad.ToList();
            foreach (AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad filaProyUsuIdent in listaProyectoUsuarioIdentidadBorrar)
            {
                if (proyCN.EsUsuarioAdministradorProyecto(filaProyUsuIdent.UsuarioID, filaProyUsuIdent.ProyectoID))
                {
                    return true;
                }
                mEntityContext.EliminarElemento(filaProyUsuIdent);
            }

            //Bloqueo ProyectoRolUsuario:
            foreach (AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyRolUsu in dataWrapperUsuario.ListaProyectoRolUsuario)
            {
                filaProyRolUsu.EstaBloqueado = true;
            }

            //Elimino Persona:
            IdentidadActual.Persona.FilaPersona.Eliminado = true;

            //Borro perfiles:
            List<Guid> perfilesEliminados = new List<Guid>();
            foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.PersonaID.Equals(IdentidadActual.PersonaID)).ToList())
            {
                perfilesEliminados.Add(filaPerfil.PerfilID);
                filaPerfil.Eliminado = true;
            }

            //Pongo como expulsadas las identidades:
            List<Guid> listaProyectosEliminados = new List<Guid>();
            foreach (Guid perfilID in perfilesEliminados)
            {
                List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(perfilID)).ToList();
                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in filasIdent)
                {
                    listaProyectosEliminados.Add(filaIdent.ProyectoID);
                    filaIdent.FechaExpulsion = DateTime.Now;
                    filaIdent.FechaBaja = DateTime.Now;
                }
            }

            #region Elimino Contactos

            ControladorAmigos contrAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            contrAmigos.CargarAmigos(IdentidadActual, false);

            List<Guid> listaContactosEliminados = new List<Guid>();

            foreach (Identidad amigo in IdentidadActual.GestorAmigos.ListaContactos.Values)
            {
                listaContactosEliminados.Add(amigo.Clave);
                IdentidadActual.GestorAmigos.EliminarAmigos(IdentidadActual.IdentidadMyGNOSS, amigo.IdentidadMyGNOSS);
            }

            #endregion

            //Guardo:
            mEntityContext.SaveChanges();

            //Actualizo modelo base:
            ControladorPersonas controPer = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            foreach (Guid proyectoID in listaProyectosEliminados)
            {
                controPer.ActualizarEliminacionModeloBaseSimple(IdentidadActual.PersonaID.Value, proyectoID, PrioridadBase.Alta);
            }

            //Limpio Caches:
            try
            {
                foreach (Guid perfilID in perfilesEliminados)
                {
                    //Invalido la cache de Mis comunidades
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    proyCL.InvalidarMisProyectos(perfilID);
                    proyCL.Dispose();
                }

                foreach (Guid proyectoID in listaProyectosEliminados)
                {
                    //Invalidamos la cache de amigos en la comunidad
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    amigosCL.InvalidarAmigosPertenecenProyecto(proyectoID);
                    amigosCL.Dispose();
                }

                foreach (Guid identidadID in listaContactosEliminados)
                {
                    //Limpiamos la cache de los contactos
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    amigosCL.InvalidarAmigos(identidadID);
                    amigosCL.Dispose();
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            //string claveEstaEnProy = "UsuarioEstaEnProy_" + ProyectoSeleccionado.Clave;
            //string claveEstaBloqEnProy = "UsuarioEstaBloqEnProy_" + ProyectoSeleccionado.Clave;
            //Session.Remove(claveEstaEnProy);
            //Session.Remove(claveEstaBloqEnProy);

            return false;
        }

        //private bool AbandonarComunidad()
        //{
        //    Guid proyectoID = mControladorBase.UsuarioActual.ProyectoID;
        //    Guid usuarioID = mControladorBase.UsuarioActual.UsuarioID;
        //    Guid identidadID = mControladorBase.UsuarioActual.IdentidadID;
        //    Guid organizacionID = mControladorBase.UsuarioActual.OrganizacionID;

        //    //string error = null;
        //    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService);

        //    if (proyCN.EsUsuarioAdministradorProyecto(usuarioID, proyectoID))
        //    {
        //        proyCN.Dispose();
        //        return true;
        //    }
        //    else
        //    {
        //        UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService);
        //        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService);
        //        GestionProyecto gestorProyectos = new GestionProyecto(new ProyectoDS());
        //        GestionUsuarios gestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerUsuarioCompletoPorID(usuarioID));
        //        GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(identidadID, false));
        //        gestorProyectos.GestionUsuarios = gestorUsuarios;
        //        gestorProyectos.GestionUsuarios.GestorIdentidades = gestorIdentidades;
        //        gestorUsuarios.GestorSuscripciones = new GestionSuscripcion(new SuscripcionDS());
        //        gestorUsuarios.GestorSuscripciones.GestorNotificaciones = new GestionNotificaciones(new NotificacionDS());

        //        gestorProyectos.EliminarUsuarioDeProyecto(usuarioID, proyectoID, organizacionID, identidadID, gestorUsuarios, gestorIdentidades);

        //        //Invalido la cache de Mis comunidades
        //        IdentidadDS idenDS = identidadCN.ObtenerIdentidadPorID(identidadID, true);
        //        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD);
        //        proyCL.InvalidarMisProyectos(((IdentidadDS.IdentidadRow)idenDS.Identidad.Rows[0]).PerfilID);

        //        proyCL.InvalidarCacheListaProyectosPerfil(((IdentidadDS.IdentidadRow)idenDS.Identidad.Rows[0]).PerfilID);
        //        proyCL.InvalidarCacheListaProyectosUsuario(usuarioID);
        //        proyCL.Dispose();


        //        ControladorPersonas controladorPersonas = new ControladorPersonas(new RiamPage());
        //        controladorPersonas.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[identidadID], proyectoID, false, true, PrioridadBase.Alta);

        //        #region Eliminación de las Suscripciones de la identidad que abandona el proyecto

        //        #region Eliminamos todas las suscripciones de esta identidad (No se incluyen las suscripciones a identidades)

        //        SuscripcionCN suscripCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService);
        //        SuscripcionDS suscripcionDS = suscripCN.ObtenerSuscripcionesDeIdentidad(identidadID, true);
        //        if (suscripcionDS.Suscripcion.Rows.Count > 0)
        //        {
        //            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService);
        //            List<Guid> listaSuscripciones = new List<Guid>();
        //            gestorUsuarios.GestorSuscripciones.SuscripcionDS.Merge(suscripcionDS, true);
        //            foreach (SuscripcionDS.SuscripcionRow filaSuscripcion in suscripcionDS.Suscripcion.Rows)
        //            {
        //                if (!listaSuscripciones.Contains(filaSuscripcion.SuscripcionID))
        //                {
        //                    listaSuscripciones.Add(filaSuscripcion.SuscripcionID);
        //                }
        //            }

        //            gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDS.Merge(notificacionCN.ObtenerNotificacionesDeSolicitudes(listaSuscripciones), true);

        //            gestorUsuarios.GestorSuscripciones.EliminarSuscripciones(identidadID);
        //            notificacionCN.Dispose();
        //            listaSuscripciones.Clear();
        //        }
        //        suscripCN.Dispose();
        //        suscripcionDS.Dispose();

        //        #endregion

        //        #region Eliminamos todas las suscripciones a identidades en este proyecto

        //        SuscripcionCN suscripCN2 = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService);
        //        SuscripcionDS suscripcionDS2 = suscripCN2.ObtenerSuscripcionesDeIdentidad(IdentidadActual.IdentidadMyGNOSS.Clave, true);
        //        if (suscripcionDS2.Suscripcion.Rows.Count > 0)
        //        {
        //            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService);
        //            List<Guid> listaSuscripciones = new List<Guid>();
        //            gestorUsuarios.GestorSuscripciones.SuscripcionDS.Merge(suscripcionDS2, true);
        //            foreach (SuscripcionDS.SuscripcionRow filaSuscripcion in suscripcionDS2.Suscripcion.Rows)
        //            {
        //                if (!listaSuscripciones.Contains(filaSuscripcion.SuscripcionID))
        //                {
        //                    listaSuscripciones.Add(filaSuscripcion.SuscripcionID);
        //                }
        //            }

        //            gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDS.Merge(notificacionCN.ObtenerNotificacionesDeSolicitudes(listaSuscripciones), true);

        //            gestorUsuarios.GestorSuscripciones.EliminarSuscripcionesDeIdentidadEnProyecto(IdentidadActual.IdentidadMyGNOSS.Clave, ProyectoSeleccionado.Clave);
        //            notificacionCN.Dispose();
        //            listaSuscripciones.Clear();
        //        }
        //        suscripCN2.Dispose();
        //        suscripcionDS2.Dispose();


        //        #endregion

        //        #endregion

        //        #region Eliminamos la configuración de datos extra del proyecto

        //        gestorIdentidades.IdentidadesDS.Merge(identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave));
        //        foreach (IdentidadDS.DatoExtraProyectoOpcionIdentidadRow fila in gestorIdentidades.IdentidadesDS.DatoExtraProyectoOpcionIdentidad)
        //        {
        //            fila.Delete();
        //        }

        //        #endregion

        //        //if (proyectoID == new Guid("8df3cd8a-7034-4727-96d1-5609369a38ef") || proyectoID == new Guid("2bde630b-a1ff-4286-9975-9b953a3449b1")) //Inevery
        //        //{
        //        //    ControladorIdentidades.DarBajaUsuarioSantillanaAsincrono(IdentidadActual.Persona.FilaPersona.Email, new RiamPage());
        //        //}

        //        ActualizarConjuntoCN actualizarCN = new ActualizarConjuntoCN();
        //        actualizarCN.GuardarDatosUsuariosWeb(null, gestorProyectos.ProyectoDS, gestorUsuarios.UsuarioDS, gestorIdentidades.IdentidadesDS, gestorIdentidades.GestorPersonas.PersonaDS, gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDS, null, gestorUsuarios.GestorSuscripciones.SuscripcionDS, null);

        //        usuarioCN.Dispose();
        //        identidadCN.Dispose();
        //        gestorProyectos.GestionUsuarios.GestorSuscripciones.Dispose();
        //        gestorProyectos.GestionUsuarios.GestorSuscripciones.GestorNotificaciones.Dispose();
        //        gestorProyectos.GestionUsuarios.GestorSuscripciones.GestorNotificaciones = null;
        //        gestorProyectos.GestionUsuarios.GestorSuscripciones = null;
        //        gestorProyectos.GestionUsuarios.GestorIdentidades.Dispose();
        //        gestorProyectos.GestionUsuarios.GestorIdentidades = null;
        //        gestorProyectos.GestionUsuarios.Dispose();
        //        gestorProyectos.GestionUsuarios = null;
        //        gestorProyectos.Dispose();
        //        gestorProyectos = null;

        //        //Eliminamos la cache de contactos en la comunidad
        //        AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService);
        //        amigosCL.InvalidarAmigosPertenecenProyecto(proyectoID);
        //        amigosCL.Dispose();

        //        #region Actualizar cola GnossLIVE

        //        ControladorDocumentacion.ActualizarGnossLive(IdentidadActual.FilaIdentidad.ProyectoID, IdentidadActual.FilaIdentidad.PerfilID, AccionLive.Eliminado, (int)TipoLive.Miembro, false, PrioridadLive.Alta);

        //        #endregion

        //        string claveEstaEnProy = "UsuarioEstaEnProy_" + ProyectoSeleccionado.Clave;
        //        string claveEstaBloqEnProy = "UsuarioEstaBloqEnProy_" + ProyectoSeleccionado.Clave;
        //        Session.Remove(claveEstaEnProy);
        //        Session.Remove(claveEstaBloqEnProy);
        //    }
        //    proyCN.Dispose();

        //    return false;
        //}

        #region propiedades

        private bool AbandonandoPlataforma
        {
            get { return ProyectoPrincipalUnico == ProyectoSeleccionado.Clave; }
        }

        /// <summary>
        /// Obtiene el domino de la aplicación más el idioma (http://didactalia.net/en)
        /// </summary>
        public string DominoAplicacionIdioma
        {
            get
            {
                return mControladorBase.DominoAplicacionConHTTP + IdiomaUsuarioDistintoPorDefecto;
            }
        }

        #endregion

    }

}
