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
using Es.Riam.Gnoss.Elementos.Amigos;
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
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class AbandonarComunidadController : ControllerBaseWeb
    {
        private IAvailableServices mAvailableServices;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AbandonarComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AbandonarComunidadController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mAvailableServices = availableServices;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
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
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    contrasenyaCorrecta = usuarioCN.ValidarPasswordUsuario(usuarioCN.ObtenerUsuarioPorID(UsuarioActual.UsuarioID), pass);
                    usuarioCN.Dispose();
                }

                if (contrasenyaCorrecta)
                {
                    ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.InvalidarUsuario, ProyectoAD.MetaProyecto, mControladorBase.UsuarioActual.UsuarioID, null, null, IdentidadActual.Persona.FilaPersona.Email, pass, GestorParametroAplicacion, null, null, null, null);

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
                error = ControladorIdentidades.AbandonarComunidad(mControladorBase.UsuarioActual.ProyectoID, mControladorBase.UsuarioActual.UsuarioID, mControladorBase.UsuarioActual.IdentidadID, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.PerfilID, mAvailableServices);
            }

            if (!error)
            {
                string sesionIdentidadID = string.Format("IdentidadIDdePersonaEnProyecto_{0}_{1}", ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.PersonaID);
                HttpContext.Session.Remove(sesionIdentidadID);

                if (AbandonandoPlataforma)
                {                    
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
            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            DataWrapperUsuario dataWrapperUsuario = usuCN.ObtenerUsuarioCompletoPorID(IdentidadActual.Persona.UsuarioID);
            usuCN.Dispose();

            //Boqueo Usuario:
            dataWrapperUsuario.ListaUsuario.FirstOrDefault().EstaBloqueado = true;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

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

            ControladorAmigos contrAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory);
            contrAmigos.CargarAmigos(IdentidadActual, false, mAvailableServices);

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
            ControladorPersonas controPer = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            foreach (Guid proyectoID in listaProyectosEliminados)
            {
                controPer.ActualizarEliminacionModeloBaseSimple(IdentidadActual.PersonaID.Value, proyectoID, PrioridadBase.Alta, mAvailableServices);
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

            return false;
        }

        #region Propiedades

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
