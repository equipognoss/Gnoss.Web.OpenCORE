using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios.Model;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Usuarios;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.Controles.Solicitudes;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AdministrarUsuariosOrganizacionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public enum TipoPagina
        {
            /// <summary>
            /// 
            /// </summary>
            Usuarios = 0,
            /// <summary>
            /// 
            /// </summary>
            Comunidades = 1,
            /// <summary>
            /// 
            /// </summary>
            Grupos = 2
        }
        /// <summary>
        /// 
        /// </summary>
        public TipoPagina PageType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SearchFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short TypeFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool OrderAsc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumPage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumElementosFiltradosPage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UrlFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OrganizationModel Organization { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<UserOrganizationModel> Usuarios { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CommunityModel> Proyectos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GroupCardModel> Grupos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public class OrganizationModel
        {
            public Guid Key { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public string Foto { get; set; }
            public int NumUsersOrg { get; set; }
            public int NumUsersOrgTypeAdmin { get; set; }
            public int NumUsersOrgTypeSuper { get; set; }
            public int NumUsersOrgTypeUser { get; set; }
            public int NumCommunitiesOrg { get; set; }
            public int NumCommunitiesAdminOrg { get; set; }
            public int NumGroupsOrg { get; set; }
        }

        public class UserOrganizationModel
        {
            public Guid User_Key { get; set; }
            public Guid Person_Key { get; set; }
            public string Name { get; set; }
            public TipoAdministradoresOrganizacion Type { get; set; }
            public string Foto { get; set; }
            public string Url { get; set; }
        }
    }

    public class ProyectosUsuViewModel
    {
        public Guid Key { get; set; }
        public string Nombre { get; set; }
        public bool Participa { get; set; }
        public short TipoParticipacion { get; set; }
        public bool Administra { get; set; }
    }

    public class UsuariosOrganizacionController : ControllerBaseWeb
    {
        public UsuariosOrganizacionController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarUsuariosOrganizacionViewModel mPaginaModel = null;

        /// <summary>
        /// DataSet con todos los usuarios de la organizacion para realizar filtros
        /// </summary>
        private DataWrapperUsuario mDataWrapperUsuario;

        private DataWrapperProyecto mDataWrapperProyecto;

        /// <summary>
        /// DataSet con los grupos de la organizacion para realizar filtros
        /// </summary>
        private DataWrapperIdentidad mDataWrapperIdentidad;

        private string FiltroSearch = "";
        private int NumPagina = 1;
        private short TipoPestanya = -1;
        private bool OrdenAscendente = true;

        private bool Filtrando = false;

        #endregion

        #region Metodos de evento

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();

            return View(PaginaModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult LoadAsignCommunity(Guid personaID)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<UsuarioAdministradorComunidad> lista = proyectoCN.CargarProyectosParticipaPersonaOrg(IdentidadActual.OrganizacionID.Value, personaID);

            List<ProyectosUsuViewModel> proyectosParticipante = new List<ProyectosUsuViewModel>();

            foreach (UsuarioAdministradorComunidad filaProyecto in lista)
            {
                bool estaEnProyecto = filaProyecto.Tipo != null;
                bool esAdmin = false;
                short tipoParticipacion = -1;
                if (estaEnProyecto)
                {
                    tipoParticipacion = (short)filaProyecto.Tipo;
                    esAdmin = ((int)filaProyecto.Administrador).Equals(1);
                }

                ProyectosUsuViewModel proyParticipa = new ProyectosUsuViewModel();
                proyParticipa.Key = filaProyecto.ProyectoID;
                proyParticipa.Nombre = UtilCadenas.ObtenerTextoDeIdioma(filaProyecto.Nombre, IdiomaUsuario, IdiomaPorDefecto);
                proyParticipa.Participa = estaEnProyecto;
                proyParticipa.TipoParticipacion = tipoParticipacion;
                proyParticipa.Administra = esAdmin;

                proyectosParticipante.Add(proyParticipa);
            }


            return PartialView("_AccionesUsuOrg", proyectosParticipante);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult AsignCommunity(Guid personaID, List<ProyectosUsuViewModel> ProyectosAsignados)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<UsuarioAdministradorComunidad> lista = proyectoCN.CargarProyectosParticipaPersonaOrg(IdentidadActual.OrganizacionID.Value, personaID);

            List<Guid> proyectosAdministra = new List<Guid>();
            Dictionary<Guid, short> proyectosParticipa = new Dictionary<Guid, short>();

            foreach (UsuarioAdministradorComunidad filaProyecto in lista)
            {
                Guid proyectoID = filaProyecto.ProyectoID;
                bool estaEnProyecto = filaProyecto.Tipo != null;
                bool esAdmin = false;
                short tipoParticipacion = -1;
                if (estaEnProyecto)
                {
                    tipoParticipacion = (short)filaProyecto.Tipo;
                    esAdmin = ((int)filaProyecto.Administrador).Equals(1);

                    proyectosParticipa.Add(proyectoID, tipoParticipacion);

                    if (esAdmin)
                    {
                        proyectosAdministra.Add(proyectoID);
                    }
                }
            }


            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionPersonas gestPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorID(personaID), mLoggingService, mEntityContext);
            personaCN.Dispose();
            Guid usuarioPersona = gestPersonas.ListaPersonas[personaID].UsuarioID;

            OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionOrganizaciones gestOrganizaciones = new GestionOrganizaciones(organizacionCN.ObtenerOrganizacionesDePersona(personaID), mLoggingService, mEntityContext);
            organizacionCN.Dispose();

            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionUsuarios gestUsuarios = new GestionUsuarios(usuCN.ObtenerUsuarioCompletoPorID(usuarioPersona), mLoggingService, mEntityContext, mConfigService);
            usuCN.Dispose();

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionIdentidades gestIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadDePersonaEnOrganizacion(personaID, IdentidadActual.OrganizacionID.Value), gestPersonas, gestUsuarios, gestOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            GestionProyecto gestProyectos = new GestionProyecto(proyectoCN.ObtenerAdministradorProyectoDePersona(personaID), mLoggingService, mEntityContext);

            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyectoCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();
            proyectoCN.Dispose();

            Guid perfilID = gestIdentidades.ListaIdentidades.Single(ident => ident.Value.FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto).Value.PerfilID;

            foreach (ProyectosUsuViewModel proyAsignado in ProyectosAsignados)
            {
                Guid comunidadID = proyAsignado.Key;

                if (proyAsignado.TipoParticipacion != 1 && proyAsignado.TipoParticipacion != 2)
                {
                    proyAsignado.TipoParticipacion = 1;
                }

                if (proyectosParticipa.ContainsKey(comunidadID))
                {
                    if (!proyectosAdministra.Contains(comunidadID) && !proyAsignado.Participa)
                    {
                        EliminarUsuarioComunidad(comunidadID, perfilID, usuarioPersona, gestIdentidades, gestProyectos);
                    }
                    else if (proyectosParticipa[comunidadID] != proyAsignado.TipoParticipacion)
                    {
                        CambiarModoParticipaComunidad(comunidadID, perfilID, (TiposIdentidad)proyAsignado.TipoParticipacion, gestIdentidades);
                    }
                }
                else if (proyAsignado.Participa)
                {
                    AgregarUsuarioComunidad(comunidadID, perfilID, usuarioPersona, (TiposIdentidad)proyAsignado.TipoParticipacion, gestIdentidades, recibirNewsletterDefectoProyectos);
                }
            }

            mEntityContext.SaveChanges();

            EliminarCacheAsignarComunidad(perfilID, IdentidadActual.Clave);

            return GnossResultOK();
        }

        /// <summary>
        /// Se encarga de eliminar la caché necesaria al asignar a un usuario de una organización a una comunidad o cambiar su modo de participación en ella.
        /// </summary>
        /// <param name="pPerfilID">Perfil del usuario a modificar</param>
        /// <param name="pIdentidadID">Identidad del usuario a modificar</param>
        private void EliminarCacheAsignarComunidad(Guid pPerfilID, Guid pIdentidadID)
        {
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCL.InvalidarFichaIdentidadMVC(pIdentidadID);
            identidadCL.Dispose();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarMisProyectos(pPerfilID);
            proyCL.Dispose();
        }

        private void CambiarModoParticipaComunidad(Guid comunidadID, Guid perfilID, TiposIdentidad modoIdentidad, GestionIdentidades gestIdentidades)
        {
            Elementos.Identidad.Identidad identidadProyecto = gestIdentidades.ListaIdentidades.Single(ident => ident.Value.PerfilID == perfilID && ident.Value.FilaIdentidad.ProyectoID == comunidadID && !ident.Value.FilaIdentidad.FechaBaja.HasValue).Value;

            Elementos.Identidad.Identidad identidadMetaProyecto = gestIdentidades.ListaIdentidades.Single(ident => ident.Value.PerfilID == perfilID && ident.Value.FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto).Value;

            Elementos.Identidad.Identidad identidadOrganizacionMetaProyecto = gestIdentidades.ListaIdentidades.Values[0];
            foreach (Guid identidadID in gestIdentidades.ListaIdentidades.Keys)
            {
                if (gestIdentidades.ListaIdentidades[identidadID].FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto)
                {
                    identidadOrganizacionMetaProyecto = gestIdentidades.ListaIdentidades[identidadID];
                    break;
                }
            }

            if (gestIdentidades.CambiarModoParticipacionIdentidad(identidadProyecto, identidadMetaProyecto, identidadOrganizacionMetaProyecto, modoIdentidad))
            {
                LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                LiveDS liveDS = new LiveDS();
                try
                {
                    mControladorBase.InsertarFilaEnColaRabbitMQ(comunidadID, perfilID, (int)AccionLive.EstadoCambiado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                    liveDS.Cola.AddColaRow(comunidadID, perfilID, (int)AccionLive.PerfilEditado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0, null);
                }
                try
                {
                    mControladorBase.InsertarFilaEnColaRabbitMQ(comunidadID, perfilID, (int)AccionLive.EstadoCambiado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                    liveDS.Cola.AddColaRow(comunidadID, perfilID, (int)AccionLive.EstadoCambiado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0, null);
                }

                liveCN.ActualizarBD(liveDS);
                liveCN.Dispose();
                liveDS.Dispose();

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                controladorPersonas.ActualizarModeloBASE(identidadProyecto, comunidadID, (modoIdentidad == TiposIdentidad.ProfesionalPersonal), true, PrioridadBase.Alta);

                // Reprocesar los recursos que haya creado la identidad
                ControladorDocumentacion controlador = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                controlador.ReprocesarRecursosIdentidadCambiarModoParticipacion(identidadProyecto);

                //Limpiamos la cache de contactos para el proyecto
                AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                amigosCL.InvalidarAmigosPertenecenProyecto(comunidadID);
                amigosCL.Dispose();
            }
        }

        private void EliminarUsuarioComunidad(Guid comunidadID, Guid perfilID, Guid usuarioID, GestionIdentidades gestIdentidades, GestionProyecto gestProyectos)
        {
            Elementos.Identidad.Identidad identidad = gestIdentidades.ListaIdentidades.Single(ident => ident.Value.PerfilID == perfilID && ident.Value.FilaIdentidad.ProyectoID == comunidadID).Value;

            gestProyectos.EliminarUsuarioDeProyecto(usuarioID, comunidadID, ProyectoAD.MetaOrganizacion, identidad.Clave, gestIdentidades.GestorUsuarios, gestIdentidades);

            if (identidad.ModoPersonal)
            {
                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                controladorPersonas.ActualizarModeloBASE(identidad, comunidadID, false, true, PrioridadBase.Alta);
            }

            LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            LiveDS liveDS = new LiveDS();
            try
            {
                mControladorBase.InsertarFilaEnColaRabbitMQ(comunidadID, identidad.PerfilID, (int)AccionLive.Eliminado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                liveDS.Cola.AddColaRow(comunidadID, identidad.PerfilID, (int)AccionLive.Eliminado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0, null);
            }

            liveCN.ActualizarBD(liveDS);
            liveCN.Dispose();
            liveDS.Dispose();
        }

        private void AgregarUsuarioComunidad(Guid comunidadID, Guid perfilID, Guid usuarioID, TiposIdentidad modoIdentidad, GestionIdentidades gestIdentidades, Dictionary<Guid, bool> recibirNewsletterDefectoProyectos)
        {
            Usuario filaUsuario = (Usuario)gestIdentidades.GestorUsuarios.ListaUsuarios[usuarioID].FilaElementoEntity;

            Elementos.Identidad.Identidad identidad = gestIdentidades.ListaIdentidades.Single(ident => ident.Value.PerfilID == perfilID && ident.Value.FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto).Value;

            //Limpiamos la cache de contactos para el proyecto
            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            amigosCL.InvalidarAmigosPertenecenProyecto(comunidadID);
            amigosCL.Dispose();

            //Elementos.Identidad.Identidad ObjetoIdentidadProy = ControladorIdentidades.AgregarIdentidadPerfilYUsuarioAProyecto(gestIdentidades, gestIdentidades.GestorUsuarios, ProyectoAD.MetaOrganizacion, comunidadID, filaUsuario, identidad.PerfilUsuario, recibirNewsletterDefectoProyectos);
            //gestIdentidades.RecargarHijos();
            ProyectoAD proyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            Guid OrganizacionID = proyectoAD.ObtenerOrganizacionIDProyecto(comunidadID);
            proyectoAD.Dispose();
            if (OrganizacionID.Equals(Guid.Empty))
            {
                throw new Exception("No se Agregar al proyecto debido a que no existe organizacion ID");
            }
            else
            {
                Elementos.Identidad.Identidad ObjetoIdentidadProy = ControladorIdentidades.AgregarIdentidadPerfilYUsuarioAProyecto(gestIdentidades, gestIdentidades.GestorUsuarios, OrganizacionID, comunidadID, filaUsuario, identidad.PerfilUsuario, recibirNewsletterDefectoProyectos);
                gestIdentidades.RecargarHijos();

				ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
				controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(identidad.PerfilUsuario, filaUsuario, gestIdentidades.GestorUsuarios, gestIdentidades);

				//asigno el modo de participacion
				List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidadProyecto = gestIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(identidad.PerfilUsuario.Clave) && ident.ProyectoID.Equals(comunidadID) && !ident.FechaBaja.HasValue).ToList();
                List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidadMetaProyecto = gestIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(identidad.PerfilUsuario.Clave) && ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.FechaBaja.HasValue).ToList();

                Elementos.Identidad.Identidad identidadProyecto = gestIdentidades.ListaIdentidades[filasIdentidadProyecto[0].IdentidadID];
                Elementos.Identidad.Identidad identidadMetaProyecto = gestIdentidades.ListaIdentidades[filasIdentidadMetaProyecto[0].IdentidadID];
                Elementos.Identidad.Identidad identidadOrganizacionMetaProyecto = gestIdentidades.ListaIdentidades.Values[0];

                identidadOrganizacionMetaProyecto = gestIdentidades.ListaIdentidades.First(identidades => identidades.Value.FilaIdentidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Value;
                //foreach (Guid identidadID in gestIdentidades.ListaIdentidades.Keys)
                //{
                //    if (gestIdentidades.ListaIdentidades[identidadID].FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto)
                //    {
                //        identidadOrganizacionMetaProyecto = gestIdentidades.ListaIdentidades[identidadID];
                //        break;
                //    }
                //}

                gestIdentidades.CambiarModoParticipacionIdentidad(identidadProyecto, identidadMetaProyecto, identidadOrganizacionMetaProyecto, modoIdentidad);

                if (identidadProyecto.ModoPersonal)
                {
                    ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    controladorPersonas.ActualizarModeloBASE(identidadProyecto, comunidadID, identidadProyecto.ModoPersonal, false, null, false, PrioridadBase.Alta);
                }

                LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                LiveDS liveDS = new LiveDS();
                try
                {
                    mControladorBase.InsertarFilaEnColaRabbitMQ(comunidadID, identidadProyecto.PerfilUsuario.Clave, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                    liveDS.Cola.AddColaRow(comunidadID, identidadProyecto.PerfilUsuario.Clave, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0, null);
                }

                liveCN.ActualizarBD(liveDS);
                liveCN.Dispose();
                liveDS.Dispose();

                // Reprocesar los recursos que haya creado la identidad
                ControladorDocumentacion controlador = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                controlador.ReprocesarRecursosIdentidadCambiarModoParticipacion(identidadProyecto);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comunidadID"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult LoadAsignUsers(Guid comunidadID)
        {
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<IdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto> listaIdentidadesDeUsuariosDeOrganizacion = identCN.ObtenerTodasIdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto(IdentidadActual.OrganizacionID.Value, comunidadID);

            List<ProyectosUsuViewModel> usuariosParticipante = new List<ProyectosUsuViewModel>();
            List<Guid> personasParticipanConOtraIdentidad = new List<Guid>();

            foreach (IdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto filaIdentidad in listaIdentidadesDeUsuariosDeOrganizacion)
            {
                Guid personaID = Guid.Empty;
                if (filaIdentidad.PersonaID.HasValue)
                    personaID = filaIdentidad.PersonaID.Value;

                if (personasParticipanConOtraIdentidad.Contains(personaID))
                {
                    continue;
                }

                ProyectosUsuViewModel usuParticipa = usuariosParticipante.FirstOrDefault(persona => persona.Key == personaID);

                bool estaEnProyecto = filaIdentidad.ProyectoID.Equals(comunidadID);

                bool estaConIdentidadOrg = false;
                if (filaIdentidad.OrganizacionID.HasValue)
                {
                    estaConIdentidadOrg = filaIdentidad.OrganizacionID.Value.Equals(IdentidadActual.OrganizacionID.Value);
                }

                if (estaEnProyecto && !estaConIdentidadOrg)
                {
                    personasParticipanConOtraIdentidad.Add(personaID);
                    if (usuParticipa != null)
                    {
                        usuariosParticipante.Remove(usuParticipa);
                    }
                    continue;
                }

                if (usuParticipa == null)
                {
                    usuParticipa = new ProyectosUsuViewModel();
                    if (filaIdentidad.PersonaID.HasValue)
                    {
                        usuParticipa.Key = filaIdentidad.PersonaID.Value;
                    }
                    usuParticipa.Nombre = filaIdentidad.NombrePerfil;

                    usuParticipa.Participa = false;
                    usuParticipa.TipoParticipacion = -1;
                    usuParticipa.Administra = false;
                    usuariosParticipante.Add(usuParticipa);
                }

                if (estaEnProyecto)
                {
                    bool esAdmin = false;
                    short tipoParticipacion = -1;
                    if (estaEnProyecto)
                    {
                        tipoParticipacion = filaIdentidad.TipoParticipacion;
                        esAdmin = filaIdentidad.TipoAdministracion.Equals(0);
                    }

                    usuParticipa.Participa = estaEnProyecto;
                    usuParticipa.TipoParticipacion = tipoParticipacion;
                    usuParticipa.Administra = esAdmin;
                }

            }

            return PartialView("_AccionesProyOrg", usuariosParticipante);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult AsignUsers(Guid comunidadID, List<ProyectosUsuViewModel> UsuariosAsignados)
        {
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<AD.EntityModel.Models.IdentidadDS.IdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto> listaIdentidadesDeUsuariosDeOrganizacion = identCN.ObtenerTodasIdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto(IdentidadActual.OrganizacionID.Value, comunidadID);

            List<Guid> personasAdministradores = new List<Guid>();
            Dictionary<Guid, short> personasParticipa = new Dictionary<Guid, short>();
            List<Guid> personasParticipanConOtraIdentidad = new List<Guid>();

            foreach (AD.EntityModel.Models.IdentidadDS.IdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto filaIdentidad in listaIdentidadesDeUsuariosDeOrganizacion)
            {
                Guid personaID = Guid.Empty;
                if (filaIdentidad.PersonaID.HasValue)
                {
                    personaID = filaIdentidad.PersonaID.Value;
                }

                if (personasParticipanConOtraIdentidad.Contains(personaID))
                {
                    continue;
                }

                bool estaEnProyecto = filaIdentidad.ProyectoID.Equals(comunidadID);
                bool estaConIdentidadOrg = false;
                if (filaIdentidad.OrganizacionID.HasValue && IdentidadActual.OrganizacionID.HasValue)
                {
                    estaConIdentidadOrg = filaIdentidad.OrganizacionID.Value.Equals(IdentidadActual.OrganizacionID.Value);
                }

                if (estaEnProyecto && !estaConIdentidadOrg)
                {
                    personasParticipanConOtraIdentidad.Add(personaID);
                }
                else if (estaEnProyecto)
                {
                    bool esAdmin = false;
                    short tipoParticipacion = filaIdentidad.TipoParticipacion;
                    esAdmin = filaIdentidad.TipoAdministracion.Equals(0);
                    personasParticipa.Add(personaID, tipoParticipacion);
                    if (esAdmin)
                    {
                        personasAdministradores.Add(personaID);
                    }
                }
            }

            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionPersonas gestPersonas = new GestionPersonas(personaCN.ObtenerPersonasDeOrganizacionCargaLigera(IdentidadActual.OrganizacionID.Value), mLoggingService, mEntityContext);
            personaCN.Dispose();

            OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionOrganizaciones gestOrganizaciones = new GestionOrganizaciones(organizacionCN.ObtenerOrganizacionPorIDCargaLigera(IdentidadActual.OrganizacionID.Value), mLoggingService, mEntityContext);
            organizacionCN.Dispose();

            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionUsuarios gestUsuarios = new GestionUsuarios(usuCN.CargarUsuariosDeOrganizacion(IdentidadActual.OrganizacionID.Value), mLoggingService, mEntityContext, mConfigService);
            usuCN.Dispose();

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperIdentidad dataWrapper = identidadCN.ObtenerIdentidadesDePersonasConOrganizacionEnProyecto(IdentidadActual.OrganizacionID.Value, ProyectoAD.MetaOrganizacion, comunidadID);
            dataWrapper.Merge(identidadCN.ObtenerIdentidadesDePersonasConOrganizacionEnProyecto(IdentidadActual.OrganizacionID.Value, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto));

            GestionIdentidades gestIdentidades = new GestionIdentidades(dataWrapper, gestPersonas, gestUsuarios, gestOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionProyecto gestProyectos = new GestionProyecto(proyectoCN.ObtenerAdministradorProyectoDeProyecto(comunidadID), mLoggingService, mEntityContext);

            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyectoCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();
            proyectoCN.Dispose();

            foreach (ProyectosUsuViewModel usuAsignado in UsuariosAsignados)
            {
                Guid personaID = usuAsignado.Key;
                Guid usuarioPersona = gestPersonas.ListaPersonas[personaID].UsuarioID;
                Guid perfilID = gestIdentidades.ListaIdentidades.Single(ident => ident.Value.PersonaID == personaID && ident.Value.FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto).Value.PerfilID;

                if (personasParticipanConOtraIdentidad.Contains(personaID))
                {
                    continue;
                }

                if (usuAsignado.TipoParticipacion != 1 && usuAsignado.TipoParticipacion != 2)
                {
                    usuAsignado.TipoParticipacion = 1;
                }

                if (personasParticipa.ContainsKey(personaID))
                {
                    if (!personasAdministradores.Contains(personaID) && !usuAsignado.Participa)
                    {
                        EliminarUsuarioComunidad(comunidadID, perfilID, usuarioPersona, gestIdentidades, gestProyectos);
                    }
                    else if (personasParticipa[personaID] != usuAsignado.TipoParticipacion)
                    {
                        CambiarModoParticipaComunidad(comunidadID, perfilID, (TiposIdentidad)usuAsignado.TipoParticipacion, gestIdentidades);
                    }
                }
                else if (usuAsignado.Participa)
                {
                    AgregarUsuarioComunidad(comunidadID, perfilID, usuarioPersona, (TiposIdentidad)usuAsignado.TipoParticipacion, gestIdentidades, recibirNewsletterDefectoProyectos);
                }
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarMisProyectos(perfilID);
                proyCL.Dispose();
            }

            mEntityContext.SaveChanges();
            return GnossResultOK();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comunidadID"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult LeaveCommunity(Guid comunidadID)
        {
            Guid organizacionID = Guid.Empty;
            if (IdentidadActual.PerfilUsuario.FilaPerfil.OrganizacionID.HasValue)
            {
                organizacionID = IdentidadActual.PerfilUsuario.FilaPerfil.OrganizacionID.Value;
            }
            Guid organizacionIDdelProyecto = ProyectoAD.MetaProyecto;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (!proyCN.EsAlguienDeLAOrganizacionAdministradorProyecto(organizacionID, comunidadID))
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionProyecto gestorProyectos = new GestionProyecto(new DataWrapperProyecto(), mLoggingService, mEntityContext);
                GestionUsuarios gestorUsuarios = new GestionUsuarios(new DataWrapperUsuario(), mLoggingService, mEntityContext, mConfigService);
                GestionIdentidades gestorIdentidades = new GestionIdentidades(new DataWrapperIdentidad(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorProyectos.GestionUsuarios = gestorUsuarios;
                gestorProyectos.GestionUsuarios.GestorIdentidades = gestorIdentidades;
                gestorUsuarios.GestorSuscripciones = new GestionSuscripcion(new DataWrapperSuscripcion(), mLoggingService, mEntityContext);
                gestorUsuarios.GestorSuscripciones.GestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                gestorProyectos.GestionUsuarios.DataWrapperUsuario.Merge(usuarioCN.CargarUsuariosDeOrganizacionYsusIdentidadesYPermisosEnProyectosConPerfilDeDichaOrg(organizacionID));

                gestorProyectos.GestionUsuarios.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesDeOrganizacionYEmpleados(organizacionID));

                gestorProyectos.GestionUsuarios.GestorIdentidades.GestorOrganizaciones = new GestionOrganizaciones(organizacionCN.ObtenerOrganizacionesPorSusIdentidadesDeProyecto(comunidadID), mLoggingService, mEntityContext);

                foreach (Usuario filaUsuario in gestorProyectos.GestionUsuarios.DataWrapperUsuario.ListaUsuario)
                {
                    Guid usuarioID = filaUsuario.UsuarioID;
                    ProyectoUsuarioIdentidad proyectoUsuarioIdentidad = gestorProyectos.GestionUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.FirstOrDefault(proyUserIde => proyUserIde.UsuarioID.Equals(usuarioID) && proyUserIde.ProyectoID.Equals(comunidadID));
                    if (proyectoUsuarioIdentidad != null)
                    {
                        Guid identidadID = proyectoUsuarioIdentidad.IdentidadID;

                        gestorProyectos.EliminarUsuarioDeProyecto(usuarioID, comunidadID, organizacionIDdelProyecto, identidadID, gestorUsuarios, gestorIdentidades);

                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        Guid perfilID = gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.IdentidadID.Equals(identidadID)).Select(identi => identi.PerfilID).FirstOrDefault();
                        proyCL.InvalidarMisProyectos(perfilID);

                        proyCL.InvalidarCacheListaProyectosPerfil(perfilID);
                        proyCL.Dispose();

                        ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                        controladorPersonas.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[identidadID], comunidadID, false, true, PrioridadBase.Alta);

                        SuscripcionCN suscripCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperSuscripcion suscripcionDW = suscripCN.ObtenerSuscripcionesDeIdentidad(identidadID, true);
                        if (suscripcionDW.ListaSuscripcion.Count > 0)
                        {
                            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            List<Guid> listaSuscripciones = new List<Guid>();
                            gestorUsuarios.GestorSuscripciones.SuscripcionDW.Merge(suscripcionDW);
                            foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in suscripcionDW.ListaSuscripcion)
                            {
                                if (!listaSuscripciones.Contains(filaSuscripcion.SuscripcionID))
                                {
                                    listaSuscripciones.Add(filaSuscripcion.SuscripcionID);
                                }
                            }

                            gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDW.Merge(notificacionCN.ObtenerNotificacionesDeSolicitudes(listaSuscripciones));

                            gestorUsuarios.GestorSuscripciones.EliminarSuscripciones(identidadID);
                            notificacionCN.Dispose();
                            listaSuscripciones.Clear();
                        }
                        suscripCN.Dispose();

                        ControladorDocumentacion.ActualizarGnossLive(comunidadID, gestorIdentidades.ListaIdentidades[identidadID].PerfilID, AccionLive.Eliminado, (int)TipoLive.Miembro, false, PrioridadLive.Alta);
                    }
                }
                //Eliminamos la cache de contactos en la comunidad
                AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                amigosCL.InvalidarAmigosPertenecenProyecto(comunidadID);
                amigosCL.Dispose();

                //Elimino de OrganizacionParticipaProy
                gestorProyectos.GestionUsuarios.GestorIdentidades.GestorOrganizaciones.EliminarOrganizacionDeProyecto(organizacionID, organizacionIDdelProyecto, comunidadID);

                Guid PerfilID = gestorProyectos.GestionUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilOrganizacion.Where(perfilOrg => perfilOrg.OrganizacionID.Equals(organizacionID)).Select(perfilOrg => perfilOrg.PerfilID).FirstOrDefault();

                Guid IdentidadOrganizacionID = gestorProyectos.GestionUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(PerfilID) && ident.ProyectoID.Equals(comunidadID)).Select(ident => ident.IdentidadID).FirstOrDefault();

                ControladorOrganizaciones controladorOrg = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                controladorOrg.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[IdentidadOrganizacionID], comunidadID, false, true, PrioridadBase.Alta);

                //Elimino Identidad
                gestorProyectos.GestionUsuarios.GestorIdentidades.EliminarIdentidad(IdentidadOrganizacionID);

                //Guardo
                mEntityContext.SaveChanges();

                usuarioCN.Dispose();
                identidadCN.Dispose();
                organizacionCN.Dispose();

                ControladorDocumentacion.ActualizarGnossLive(comunidadID, PerfilID, AccionLive.Eliminado, (int)TipoLive.Miembro, false, PrioridadLive.Alta);
            }
            else
            {
                return GnossResultERROR();
            }

            proyCN.Dispose();

            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyectoCL.InvalidarCacheProyectosOrgCargaLigeraParaFiltros(IdentidadActual.OrganizacionID.Value);
            proyectoCL.Dispose();

            return GnossResultOK();
        }
        /// <summary>
        /// Activa el registro automático en la comunidad
        /// </summary>
        /// <param name="comunidadID"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult ActivarRegAuto(Guid comunidadID)
        {
            try
            {
                Guid organizacionId = Guid.Empty;
                if (IdentidadActual.PerfilUsuario.FilaPerfil.OrganizacionID.HasValue)
                {
                    organizacionId = IdentidadActual.PerfilUsuario.FilaPerfil.OrganizacionID.Value;
                }
                else
                {
                    organizacionId = IdentidadActual.OrganizacionID.Value;
                }
                OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                List<Guid> listaPersonasOrganizacion = organizacionCN.ObetenerPersonasDeLaOrganizacion(organizacionId);

                List<Guid> listaUsuariosOrganizacion = new List<Guid>();
                foreach (Guid persona in listaPersonasOrganizacion)
                {
                    GestionPersonas gestPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorID(persona), mLoggingService, mEntityContext);
                    Guid usuarioPersona = gestPersonas.ListaPersonas[persona].UsuarioID;

                    listaUsuariosOrganizacion.Add(usuarioPersona);
                }

                List<Guid> listaUsuarios = proyCN.ObtenerUsuariosNoParticipanEnComunidad(listaUsuariosOrganizacion, comunidadID);
                Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();

                List<Guid> listaPerfiles = new List<Guid>();
                foreach (Guid usuario in listaUsuarios)
                {
                    Guid personaID = mEntityContext.Persona.Where(item => item.UsuarioID.Value.Equals(usuario)).Select(item => item.PersonaID).FirstOrDefault();

                    GestionPersonas gestPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorID(personaID), mLoggingService, mEntityContext);

                    Guid usuarioPersona = usuario;

                    GestionOrganizaciones gestOrganizaciones = new GestionOrganizaciones(organizacionCN.ObtenerOrganizacionesDePersona(personaID), mLoggingService, mEntityContext);

                    GestionUsuarios gestUsuarios = new GestionUsuarios(usuCN.ObtenerUsuarioCompletoPorID(usuarioPersona), mLoggingService, mEntityContext, mConfigService);

                    GestionIdentidades gestIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadDePersonaEnOrganizacion(personaID, IdentidadActual.OrganizacionID.Value), gestPersonas, gestUsuarios, gestOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    GestionProyecto gestProyectos = new GestionProyecto(proyCN.ObtenerAdministradorProyectoDePersona(personaID), mLoggingService, mEntityContext);

                    Guid perfilID = gestIdentidades.ListaIdentidades.Single(ident => ident.Value.FilaIdentidad.ProyectoID == ProyectoAD.MetaProyecto).Value.PerfilID;

                    AgregarUsuarioComunidad(comunidadID, perfilID, usuarioPersona, TiposIdentidad.ProfesionalPersonal, gestIdentidades, recibirNewsletterDefectoProyectos);

                    listaPerfiles.Add(perfilID);
                }
                mEntityContext.SaveChanges();
                proyCL.InvalidarMisProyectosListaPerfiles(listaPerfiles);
                organizacionCN.ActualizarRegAuto(IdentidadActual.OrganizacionID.Value, comunidadID);
                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }
        }
        /// <summary>
        /// Desactiva el registro automático de la comunidad
        /// </summary>
        /// <param name="comunidadID"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult DesactivarRegAuto(Guid comunidadID)
        {
            try
            {
                OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                orgCN.ActualizarRegAuto(IdentidadActual.OrganizacionID.Value, comunidadID);
                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult ChangeRol(Guid personaID, short rol)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionPersonas gestPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorID(personaID), mLoggingService, mEntityContext);
            personaCN.Dispose();
            Guid usuarioPersona = gestPersonas.ListaPersonas[personaID].UsuarioID;

            List<Guid> listausuarios = new List<Guid>();
            listausuarios.Add(usuarioPersona);

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionIdentidades gestIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadDePersonaEnOrganizacion(personaID, IdentidadActual.OrganizacionID.Value), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            PerfilPersonaOrg filasIdentidad = gestIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(item => item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value) && item.PersonaID.Equals(personaID)).FirstOrDefault();
            Guid? perfilID = null;
            perfilID = filasIdentidad.PerfilID;

            identidadCN.Dispose();

            OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionOrganizaciones gestOrganizaciones = new GestionOrganizaciones(organizacionCN.ObtenerOrganizacionesDePersona(personaID), mLoggingService, mEntityContext);
            gestOrganizaciones.OrganizacionDW.Merge(organizacionCN.CargarOrganizacionesAdministraUsuario(usuarioPersona));
            organizacionCN.Dispose();

            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionUsuarios gestUsuarios = new GestionUsuarios(usuCN.CargarUsuariosDeOrganizacion(IdentidadActual.OrganizacionID.Value), mLoggingService, mEntityContext, mConfigService);
            usuCN.Dispose();

            List<AdministradorOrganizacion> filasAdministradorOrganizacion = gestOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(IdentidadActual.OrganizacionID) && item.UsuarioID.Equals(usuarioPersona)).ToList();

            if (filasAdministradorOrganizacion.Any())
            {
                mEntityContext.EliminarElemento(filasAdministradorOrganizacion.FirstOrDefault());
                gestOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion.Remove(filasAdministradorOrganizacion.FirstOrDefault());
            }

            List<OrganizacionRolUsuario> filasOrganizacionRolUsuario = gestUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.OrganizacionID.Equals(IdentidadActual.OrganizacionID) && item.UsuarioID.Equals(usuarioPersona)).ToList();            

            if (rol == (short)TipoAdministradoresOrganizacion.Editor)
            {
                gestOrganizaciones.AgregarEditorDeOrganizacion(usuarioPersona, IdentidadActual.OrganizacionID.Value, gestUsuarios);
            }
            else if (rol == (short)TipoAdministradoresOrganizacion.Administrador)
            {
                gestOrganizaciones.AgregarAdministradorDeOrganizacion(usuarioPersona, IdentidadActual.OrganizacionID.Value, gestUsuarios);
            }
            
            mEntityContext.SaveChanges();

            UsuarioCL usuarioCL = new UsuarioCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            usuarioCL.EliminarCacheUsuariosCargaLigeraParaFiltros(IdentidadActual.OrganizacionID.Value);

            IdentidadCL identCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identCL.AgregarPermisosUsuarioEnOrg(usuarioPersona, IdentidadActual.OrganizacionID.Value, rol);
            identCL.InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(IdentidadActual.OrganizacionID.Value);
            if (perfilID.HasValue)
            {
                identCL.EliminarPerfilMVC(perfilID.Value);
            }
            identCL.Dispose();

            return GnossResultOK();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult Delete(Guid personaID)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionPersonas gestPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorID(personaID), mLoggingService, mEntityContext);

            Guid usuarioPersona = gestPersonas.ListaPersonas[personaID].UsuarioID;

            Guid organizacionID = IdentidadActual.OrganizacionID.Value;

            //Eliminamos de la caché su referencia:
            IdentidadCL identCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identCL.EliminarPermisosUsuarioEnOrg(usuarioPersona, organizacionID);
            identCL.Dispose();

            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionUsuarios gestUsuarios = new GestionUsuarios(usuarioCN.ObtenerUsuarioCompletoPorID(usuarioPersona), mLoggingService, mEntityContext, mConfigService);
            gestUsuarios.DataWrapperUsuario.Merge(usuarioCN.ObtenerOrganizacionRolUsuario(usuarioPersona, organizacionID));
            usuarioCN.Dispose();

            List<Guid> listausuarios = new List<Guid>();
            listausuarios.Add(usuarioPersona);

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionIdentidades gestIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadDePersonaEnOrganizacion(personaID, organizacionID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            Persona elementoPersona = gestPersonas.ListaPersonas[personaID];

            OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionOrganizaciones gestOrganizaciones = new GestionOrganizaciones(organizacionCN.ObtenerOrganizacionesDePersona(personaID), mLoggingService, mEntityContext);
            organizacionCN.Dispose();

            Guid perfilID = gestIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(item => item.OrganizacionID.Equals(organizacionID) && item.PersonaID.Equals(personaID)).FirstOrDefault().PerfilID;

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionProyecto gestProyectos = new GestionProyecto(proyectoCN.ObtenerAdministradorProyectoDePersona(personaID), mLoggingService, mEntityContext);
            proyectoCN.Dispose();

            List<ProyectoUsuarioIdentidad> listafilaUsuarioIdentidad = new List<ProyectoUsuarioIdentidad>();

            foreach (ProyectoUsuarioIdentidad filaProyectoUsuarioIdentidad in gestUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Where(proyUsIden => proyUsIden.UsuarioID.Equals(usuarioPersona)))
            {
                if (gestIdentidades.DataWrapperIdentidad.ListaIdentidad.Count(ident => ident.IdentidadID.Equals(filaProyectoUsuarioIdentidad.IdentidadID) && ident.PerfilID.Equals(perfilID)) > 0)
                {
                    listafilaUsuarioIdentidad.Add(filaProyectoUsuarioIdentidad);
                }
            }

            //Variable que controla si el usuario es el administrador de alguna de sus comunidades
            bool esAdministrador = false;
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            foreach (AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad fila in listafilaUsuarioIdentidad)
            {
                if (fila.ProyectoID != ProyectoAD.MetaProyecto)
                {
                    esAdministrador = proyCN.EsUsuarioAdministradorProyecto(usuarioPersona, fila.ProyectoID);

                    if (esAdministrador)
                    {
                        break;
                    }
                }
            }

            if (!esAdministrador)
            {
                // Desvincular la persona de la organización
                var listaPersonaVinculoOrg = gestOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(personaID) && item.OrganizacionID.Equals(organizacionID));//.FindByPersonaIDOrganizacionID(personaID, organizacionID).Delete();

                foreach (PersonaVinculoOrganizacion personaVinculoOrganizacion in listaPersonaVinculoOrg.ToList())
                {
                    mEntityContext.EliminarElemento(personaVinculoOrganizacion);
                    gestOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Remove(personaVinculoOrganizacion);
                }


                //Elimino de "AdministradorOrganizacion"
                List<AdministradorOrganizacion> filasAdministradorOrganizacion = gestOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(usuarioPersona) && item.OrganizacionID.Equals(organizacionID)).ToList();

                foreach (AdministradorOrganizacion filaAdministradorOrganizacion in filasAdministradorOrganizacion)
                {
                    mEntityContext.EliminarElemento(filaAdministradorOrganizacion);
                    gestOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion.Remove(filaAdministradorOrganizacion);
                }
                //Elimino de "PersonaVisibleEnOrg"
                List<PersonaVisibleEnOrg> filasPersonasVisibleEnOrg = gestOrganizaciones.OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(personaID) && item.OrganizacionID.Equals(organizacionID)).ToList();//.Select("PersonaID = '" + personaID + "' AND OrganizacionID ='" + organizacionID + "'");

                foreach (PersonaVisibleEnOrg filasPersonaVisibleEnOrg in filasPersonasVisibleEnOrg)
                {
                    mEntityContext.EliminarElemento(filasPersonasVisibleEnOrg);
                    gestOrganizaciones.OrganizacionDW.ListaPersonaVisibleEnOrg.Remove(filasPersonaVisibleEnOrg);
                }

                OrganizacionRolUsuario filasOrganizacionRolUsuario = gestUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(usuarioPersona) && item.OrganizacionID.Equals(organizacionID)).FirstOrDefault();
                if (filasOrganizacionRolUsuario != null)
                {
                    mEntityContext.EliminarElemento(filasOrganizacionRolUsuario);
                    DataWrapperUsuario.ListaOrganizacionRolUsuario.Remove(filasOrganizacionRolUsuario);
                }

                //Modifico su perfil (persona+organizacion) para ponerlo de baja o eliminarlo si era nuevo(estaba en memoria sin guardar)
                gestIdentidades.EliminarPerfilPersonaOrganizacion(personaID, organizacionID);

                foreach (AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad fila in listafilaUsuarioIdentidad)
                {
                    if (fila.ProyectoID != ProyectoAD.MetaProyecto)
                    {
                        Guid identidadID = fila.IdentidadID;
                        Guid proyectoID = fila.ProyectoID;

                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);//"IdentidadID = '" + identidadID.ToString() + "'"
                        proyCL.InvalidarMisProyectos(gestIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.IdentidadID.Equals(identidadID)).Select(ident => ident.PerfilID).FirstOrDefault());
                        proyCL.Dispose();

                        //GestorProyectos.EliminarUsuarioDeProyecto(usuarioID, fila.ProyectoID, fila.OrganizacionGnossID, fila.IdentidadID, GestorUsuarios, GestorIdentidades);

                        if (gestIdentidades.ListaIdentidades[identidadID].ModoPersonal)
                        {
                            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                            controladorPersonas.ActualizarModeloBASE(gestIdentidades.ListaIdentidades[identidadID], proyectoID, false, true, PrioridadBase.Alta);
                        }
                        gestProyectos.EliminarUsuarioDeProyecto(usuarioPersona, fila.ProyectoID, fila.OrganizacionGnossID, fila.IdentidadID, gestUsuarios, gestIdentidades);
                    }
                    else
                    {
                        gestProyectos.EliminarUsuarioDeMyGnoss(usuarioPersona, fila.ProyectoID, fila.OrganizacionGnossID, fila.IdentidadID, gestUsuarios, gestIdentidades);
                    }
                }


                listafilaUsuarioIdentidad.Clear();

                mEntityContext.SaveChanges();
            }
            else
            {
                return GnossResultERROR(UtilIdiomas.GetText("USUARIOS", "ERRORESADMIN"));
            }

            UsuarioCL usuarioCL = new UsuarioCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            usuarioCL.EliminarCacheUsuariosCargaLigeraParaFiltros(IdentidadActual.OrganizacionID.Value);

            return GnossResultOK();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorOrganizacion })]
        public ActionResult Filter(string Search, int NumPage, string Order, short Type = -1)
        {
            Filtrando = true;

            FiltroSearch = Search;
            NumPagina = NumPage;
            TipoPestanya = Type;
            OrdenAscendente = Order != "DESC";

            if (TipoPaginaUsuOrg.Equals(AdministrarUsuariosOrganizacionViewModel.TipoPagina.Usuarios))
            {
                return PartialView("_Usuarios", PaginaModel);
            }
            else if (TipoPaginaUsuOrg.Equals(AdministrarUsuariosOrganizacionViewModel.TipoPagina.Comunidades))
            {
                return PartialView("_Comunidades", PaginaModel);
            }
            else if (TipoPaginaUsuOrg.Equals(AdministrarUsuariosOrganizacionViewModel.TipoPagina.Grupos))
            {
                return PartialView("_Grupos", PaginaModel);

            }

            return GnossResultERROR();
        }

        #endregion

        private List<AdministrarUsuariosOrganizacionViewModel.UserOrganizationModel> CargarListaUsuarios()
        {
            List<UsuarioIdentidadPersona> listaUsuarioIdentidadPersona = new List<UsuarioIdentidadPersona>();
            string filtro = "";
            bool filtroIniciado = false;
            if (!string.IsNullOrEmpty(FiltroSearch))
            {
                filtroIniciado = true;
            }

            string orden = "";
            int inicio = (NumPagina - 1) * 10;
            int final = inicio + 10;
            if (TipoPestanya > -1)
            {
                if (filtroIniciado)
                {
                    listaUsuarioIdentidadPersona = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Where(usuario => usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower()) && usuario.Tipo.Equals(TipoPestanya)).ToList();
                }
                else
                {
                    listaUsuarioIdentidadPersona = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Where(usuario => usuario.Tipo.Equals(TipoPestanya)).ToList();
                }
                filtro += " tipo = " + TipoPestanya.ToString();
            }
            else if (filtroIniciado)
            {
                listaUsuarioIdentidadPersona = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Where(usuario => usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower())).ToList();
            }
            if (listaUsuarioIdentidadPersona.Count > 0)
            {
                if (OrdenAscendente)
                {
                    listaUsuarioIdentidadPersona = listaUsuarioIdentidadPersona.OrderBy(usuario => usuario.Nombre).ToList();
                }
                else
                {
                    listaUsuarioIdentidadPersona = listaUsuarioIdentidadPersona.OrderByDescending(usuario => usuario.Nombre).ToList();
                }
            }
            else
            {
                if (OrdenAscendente)
                {
                    listaUsuarioIdentidadPersona = DataWrapperUsuario.ListaUsuarioIdentidadPersona.OrderBy(usuario => usuario.Nombre).ToList();
                }
                else
                {
                    listaUsuarioIdentidadPersona = DataWrapperUsuario.ListaUsuarioIdentidadPersona.OrderByDescending(usuario => usuario.Nombre).ToList();
                }
            }




            List<AdministrarUsuariosOrganizacionViewModel.UserOrganizationModel> usuariosFiltro = new List<AdministrarUsuariosOrganizacionViewModel.UserOrganizationModel>();

            int cont = 0;
            //mUsuariosParaPintar = new Dictionary<Guid, Identidad>();    Select(filtro, orden)
            foreach (UsuarioIdentidadPersona filaUsuario in listaUsuarioIdentidadPersona)
            {
                if (cont < inicio)
                {
                    cont++;
                    continue;
                }

                usuariosFiltro.Add(CargarUsuarioOrg(filaUsuario));
                cont++;

                if (cont >= final)
                {
                    break;
                }
            }

            return usuariosFiltro;
        }

        private List<CommunityModel> CargarListaProyectos()
        {
            string filtro = "";
            List<ProyectoConAdministrado> proyectoConAdministrado = new List<ProyectoConAdministrado>();
            proyectoConAdministrado = DataWrapperProyecto.ListaProyectoConAdministrado;
            if (!string.IsNullOrEmpty(FiltroSearch))
            {
                filtro = " (Nombre like '" + FiltroSearch + "%' OR Nombre like '% " + FiltroSearch + "%') ";
                proyectoConAdministrado = DataWrapperProyecto.ListaProyectoConAdministrado.Where(proyecto => proyecto.Nombre.ToLower().StartsWith(FiltroSearch.ToLower()) || proyecto.Nombre.ToLower().Contains(" " + FiltroSearch.ToLower())).ToList();
            }

            string orden = "";
            int inicio = (NumPagina - 1) * 10;
            int final = inicio + 10;

            if (OrdenAscendente)
            {
                orden += "Nombre ASC";
                proyectoConAdministrado = proyectoConAdministrado.OrderBy(proyecto => proyecto.Nombre).ToList();
            }
            else
            {
                orden += "Nombre DESC";
                proyectoConAdministrado = proyectoConAdministrado.OrderByDescending(proyecto => proyecto.Nombre).ToList();
            }
            if (TipoPestanya > -1)
            {
                //if (filtro.Length > 0)
                //{
                //    filtro += " AND ";
                //}
                filtro += " administrado = " + TipoPestanya.ToString();
                proyectoConAdministrado = proyectoConAdministrado.Where(proyecto => proyecto.Administrado.Equals(TipoPestanya)).ToList();
            }



            List<Guid> listaComunidades = new List<Guid>();
            int cont = 0;

            foreach (ProyectoConAdministrado filaProyecto in proyectoConAdministrado)
            {
                if (cont < inicio)
                {
                    cont++;
                    continue;
                }

                if (!listaComunidades.Contains(filaProyecto.ProyectoID))
                {
                    cont++;
                    listaComunidades.Add(filaProyecto.ProyectoID);
                }

                if (cont >= final)
                {
                    break;
                }
            }

            ControladorProyectoMVC controladorProyecto = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLContent, BaseURLStatic, ProyectoSeleccionado, ParametrosGeneralesRow, IdentidadActual, false, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);

            var comunidades = controladorProyecto.ObtenerComunidadesPorID(listaComunidades, BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "COMUNIDADES")).Values.ToList();

            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (IdentidadActual.OrganizacionID.HasValue)
            {
                Dictionary<Guid, bool> dicRegAuto = orgCN.ObtenerParametroRegistroautomatico(listaComunidades, IdentidadActual.OrganizacionID.Value);
                foreach (var comunidad in comunidades)
                {
                    comunidad.RegistroAutomatico = dicRegAuto[comunidad.Key];
                }
            }
            else
            {
                throw new Exception($"El campo IdentidadActual.OrganizacionID no puede ser NULL. IdentidadID {IdentidadActual.Clave}");
            }

            return comunidades;
        }

        private List<GroupCardModel> CargarListaGrupos()
        {
            string filtro = "";
            List<AD.EntityModel.Models.IdentidadDS.GrupoIdentidades> listaGrupoIdentidades = DataWrapperIdentidad.ListaGrupoIdentidades.ToList();
            if (!string.IsNullOrEmpty(FiltroSearch))
            {
                filtro = " (Nombre like '" + FiltroSearch + "%' OR Nombre like '% " + FiltroSearch + "%') ";
                listaGrupoIdentidades = DataWrapperIdentidad.ListaGrupoIdentidades.Where(iden => iden.Nombre.ToLower().Contains(FiltroSearch.ToLower())).ToList();
            }

            string orden = "";
            int inicio = (NumPagina - 1) * 10;
            int final = inicio + 10;

            if (OrdenAscendente)
            {
                orden += "Nombre ASC";
                listaGrupoIdentidades = listaGrupoIdentidades.OrderBy(item => item.Nombre).ToList();
            }
            else
            {
                orden += "Nombre DESC";
                listaGrupoIdentidades = listaGrupoIdentidades.OrderByDescending(item => item.Nombre).ToList();
            }

            if (TipoPestanya > -1)
            {
                if (filtro.Length > 0)
                {
                    filtro += " AND ";
                }
                //administrado?
                //filtro += " administrado = " + TipoPestanya.ToString();
                //listaGrupoIdentidades = listaGrupoIdentidades.Where(item => item.)
            }

            List<Guid> listaGrupos = new List<Guid>();
            int cont = 0;
            foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupo in listaGrupoIdentidades)
            {
                if (cont < inicio)
                {
                    cont++;
                    continue;
                }

                if (!listaGrupos.Contains(filaGrupo.GrupoID))
                {
                    cont++;
                    listaGrupos.Add(filaGrupo.GrupoID);
                }

                if (cont >= final)
                {
                    break;
                }
            }

            ControladorProyectoMVC controladorProyecto = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLContent, BaseURLStatic, ProyectoSeleccionado, ParametrosGeneralesRow, IdentidadActual, false, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);

            return controladorProyecto.ObtenerGruposPorID(listaGrupos).Values.ToList();
        }

        private AdministrarUsuariosOrganizacionViewModel.UserOrganizationModel CargarUsuarioOrg(UsuarioIdentidadPersona filaUsuario)
        {
            AdministrarUsuariosOrganizacionViewModel.UserOrganizationModel user = new AdministrarUsuariosOrganizacionViewModel.UserOrganizationModel();
            user.Person_Key = filaUsuario.PersonaID;
            user.User_Key = filaUsuario.UsuarioID;
            user.Name = $"{filaUsuario.Nombre} {filaUsuario.Apellidos}";

            user.Type = (TipoAdministradoresOrganizacion)filaUsuario.Tipo;

            string nombreCortoUsu = filaUsuario.NombreCortoUsu;
            string urlFoto = filaUsuario.Foto;

            if (!string.IsNullOrEmpty(urlFoto) && !urlFoto.Equals("sinfoto"))
            {
                user.Foto = "/" + UtilArchivos.ContentImagenes + urlFoto;
            }

            user.Url = UtilIdiomas.GetText("URLSEM", "ORGANIZACION") + "/" + IdentidadActual.OrganizacionPerfil.NombreCorto + "/" + UtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + nombreCortoUsu;

            return user;
        }
        #region Propiedades

        private AdministrarUsuariosOrganizacionViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarUsuariosOrganizacionViewModel();

                    mPaginaModel.PageType = TipoPaginaUsuOrg;

                    if (RequestParams("tipo") != null)
                    {
                        short.TryParse(RequestParams("tipo"), out TipoPestanya);
                    }
                    mPaginaModel.SearchFilter = FiltroSearch;
                    mPaginaModel.TypeFilter = TipoPestanya;
                    mPaginaModel.OrderAsc = OrdenAscendente;
                    mPaginaModel.NumPage = NumPagina;


                    mPaginaModel.Organization = new AdministrarUsuariosOrganizacionViewModel.OrganizationModel();

                    mPaginaModel.Organization.Key = IdentidadActual.OrganizacionID.Value;
                    mPaginaModel.Organization.Name = IdentidadActual.OrganizacionPerfil.Nombre;

                    string urlFoto = IdentidadActual.IdentidadOrganizacion.UrlImagenGrande;

                    if (!string.IsNullOrEmpty(urlFoto) && !urlFoto.Equals("sinfoto"))
                    {
                        mPaginaModel.Organization.Foto = "/" + UtilArchivos.ContentImagenes + urlFoto;
                    }

                    mPaginaModel.Organization.Url = UtilIdiomas.GetText("URLSEM", "ORGANIZACION") + "/" + IdentidadActual.OrganizacionPerfil.NombreCorto;
                    
                    // Filtro introducido
                    string filtro = mPaginaModel.SearchFilter;

                    mPaginaModel.UrlFilter = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "ADMINISTRACION") + "/";

                    #region Usuarios

                    if (TipoPaginaUsuOrg.Equals(AdministrarUsuariosOrganizacionViewModel.TipoPagina.Usuarios))
                    {
                        mPaginaModel.UrlFilter += UtilIdiomas.GetText("URLSEM", "USUARIOS");
                        if (!string.IsNullOrEmpty(FiltroSearch))
                        {
                            //filtro = " AND (Nombre like '" + FiltroSearch + "%' OR Nombre like '% " + FiltroSearch + "%') ";
                            mPaginaModel.Organization.NumUsersOrgTypeAdmin = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower()) && usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador));
                            mPaginaModel.Organization.NumUsersOrgTypeSuper = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower()) && usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Editor));
                            mPaginaModel.Organization.NumUsersOrgTypeUser = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower()) && usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Comentarista));
                        }
                        else
                        {
                            mPaginaModel.Organization.NumUsersOrgTypeAdmin = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador));
                            mPaginaModel.Organization.NumUsersOrgTypeSuper = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Editor));
                            mPaginaModel.Organization.NumUsersOrgTypeUser = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Comentarista));
                        }


                        string filtroNumResultados = "";
                        if (TipoPestanya > -1)
                        {
                            if (!string.IsNullOrEmpty(FiltroSearch))
                            {
                                mPaginaModel.NumElementosFiltradosPage = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => (usuario.Nombre.ToLower().StartsWith(FiltroSearch.ToLower()) || usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower())) && usuario.Tipo.Equals(TipoPestanya));
                            }
                            else
                            {
                                mPaginaModel.NumElementosFiltradosPage = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Tipo.Equals(TipoPestanya));
                            }

                        }
                        else if (!string.IsNullOrEmpty(filtro))
                        {
                            mPaginaModel.NumElementosFiltradosPage = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => (usuario.Nombre.ToLower().StartsWith(FiltroSearch.ToLower()) || usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower())));
                        }
                        else
                        {
                            mPaginaModel.NumElementosFiltradosPage = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count;
                        }

                        mPaginaModel.Usuarios = CargarListaUsuarios();
                    }
                    else if (!Filtrando)
                    {
                        if (!string.IsNullOrEmpty(FiltroSearch))
                        {
                            //filtro = " AND (Nombre like '" + FiltroSearch + "%' OR Nombre like '% " + FiltroSearch + "%') ";
                            mPaginaModel.Organization.NumUsersOrgTypeAdmin = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower()) && usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador));
                            mPaginaModel.Organization.NumUsersOrgTypeSuper = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower()) && usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Editor));
                            mPaginaModel.Organization.NumUsersOrgTypeUser = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Nombre.ToLower().Contains(FiltroSearch.ToLower()) && usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Comentarista));
                        }
                        else
                        {
                            mPaginaModel.Organization.NumUsersOrgTypeAdmin = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador));
                            mPaginaModel.Organization.NumUsersOrgTypeSuper = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Editor));
                            mPaginaModel.Organization.NumUsersOrgTypeUser = DataWrapperUsuario.ListaUsuarioIdentidadPersona.Count(usuario => usuario.Tipo.Equals((short)TipoAdministradoresOrganizacion.Comentarista));
                        }
                    }

                    mPaginaModel.Organization.NumUsersOrg = mPaginaModel.Organization.NumUsersOrgTypeAdmin + mPaginaModel.Organization.NumUsersOrgTypeSuper + mPaginaModel.Organization.NumUsersOrgTypeUser;

                    #endregion

                    #region Comunidades

                    if (TipoPaginaUsuOrg.Equals(AdministrarUsuariosOrganizacionViewModel.TipoPagina.Comunidades))
                    {
                        mPaginaModel.UrlFilter += UtilIdiomas.GetText("URLSEM", "COMUNIDADES");

                        if (string.IsNullOrEmpty(FiltroSearch))
                        {
                            //mPaginaModel.Organization.NumCommunitiesOrg = DataWrapperProyecto.Tables["Proyecto"].Rows.Count;
                            mPaginaModel.Organization.NumCommunitiesOrg = DataWrapperProyecto.ListaProyectoConAdministrado.Count;
                        }
                        else
                        {
                            mPaginaModel.Organization.NumCommunitiesOrg = DataWrapperProyecto.ListaProyectoConAdministrado.Where(proyecto => proyecto.Nombre.ToLower().StartsWith(FiltroSearch.ToLower()) || proyecto.Nombre.ToLower().Contains(" " + FiltroSearch.ToLower())).ToList().Count;//(filtro.Substring(4)).Length;
                        }

                        if (string.IsNullOrEmpty(FiltroSearch))
                        {
                            mPaginaModel.Organization.NumCommunitiesAdminOrg = DataWrapperProyecto.ListaProyectoConAdministrado.Where(proyecto => proyecto.Administrado == 0).ToList().Count;
                        }
                        else
                        {
                            //mPaginaModel.Organization.NumCommunitiesAdminOrg = DataWrapperProyecto.ListaProyecto.Select("administrado = 0" + filtro).Length;
                            mPaginaModel.Organization.NumCommunitiesAdminOrg = DataWrapperProyecto.ListaProyectoConAdministrado.Where(proyecto => proyecto.Administrado == 0 && (proyecto.Nombre.ToLower().StartsWith(FiltroSearch.ToLower()) || proyecto.Nombre.ToLower().Contains(" " + FiltroSearch.ToLower()))).ToList().Count;
                        }


                        string filtroNumResultados = "";
                        if (TipoPestanya > -1)
                        {
                            if (!string.IsNullOrEmpty(FiltroSearch))
                            {
                                //filtroNumResultados += " administrado = " + TipoPestanya.ToString() + filtro;
                                mPaginaModel.NumElementosFiltradosPage = DataWrapperProyecto.ListaProyectoConAdministrado.Where(proyecto => proyecto.Administrado == TipoPestanya && (proyecto.Nombre.ToLower().StartsWith(FiltroSearch.ToLower()) || proyecto.Nombre.ToLower().Contains(" " + FiltroSearch.ToLower()))).ToList().Count;
                            }
                            else
                            {
                                mPaginaModel.NumElementosFiltradosPage = DataWrapperProyecto.ListaProyectoConAdministrado.Where(proyecto => proyecto.Administrado == TipoPestanya).ToList().Count;
                            }

                        }
                        else if (!string.IsNullOrEmpty(FiltroSearch))
                        {

                            mPaginaModel.NumElementosFiltradosPage = DataWrapperProyecto.ListaProyectoConAdministrado.Where(proyecto => proyecto.Nombre.ToLower().StartsWith(FiltroSearch.ToLower()) || proyecto.Nombre.ToLower().Contains(" " + FiltroSearch.ToLower())).ToList().Count;
                        }
                        else
                        {
                            mPaginaModel.NumElementosFiltradosPage = DataWrapperProyecto.ListaProyectoConAdministrado.Count;
                        }

                        mPaginaModel.Proyectos = CargarListaProyectos();
                    }
                    else if (!Filtrando)
                    {
                        mPaginaModel.Organization.NumCommunitiesOrg = DataWrapperProyecto.ListaProyectoConAdministrado.Count;
                    }

                    #endregion


                    if (TipoPaginaUsuOrg.Equals(AdministrarUsuariosOrganizacionViewModel.TipoPagina.Grupos))
                    {
                        mPaginaModel.UrlFilter += UtilIdiomas.GetText("URLSEM", "GRUPOS");

                        if (string.IsNullOrEmpty(filtro))
                        {
                            mPaginaModel.Organization.NumGroupsOrg = DataWrapperIdentidad.ListaGrupoIdentidadesOrganizacion.Count;
                        }
                        else
                        {
                            mPaginaModel.Organization.NumGroupsOrg = DataWrapperIdentidad.ListaGrupoIdentidades.Count(item => item.Nombre.ToLower().Contains(FiltroSearch.ToLower()));
                        }
                        mPaginaModel.NumElementosFiltradosPage = mPaginaModel.Organization.NumGroupsOrg;

                        mPaginaModel.Grupos = CargarListaGrupos();

                    }
                    else if (!Filtrando)
                    {
                        mPaginaModel.Organization.NumGroupsOrg = DataWrapperIdentidad.ListaGrupoIdentidadesOrganizacion.Count;
                    }

                }
                return mPaginaModel;
            }
        }

        private AdministrarUsuariosOrganizacionViewModel.TipoPagina TipoPaginaUsuOrg
        {
            get
            {
                return (AdministrarUsuariosOrganizacionViewModel.TipoPagina)short.Parse(RequestParams("pestanya"));
            }
        }

        /// <summary>
        /// Obtiene el DataSet de usuarios
        /// </summary>
        private DataWrapperUsuario DataWrapperUsuario
        {
            get
            {
                if (mDataWrapperUsuario == null)
                {
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mDataWrapperUsuario = usuarioCN.CargarUsuariosDeOrganizacionCargaLigeraParaFiltros(IdentidadActual.OrganizacionID.Value);
                }
                return mDataWrapperUsuario;
            }
        }

        /// <summary>
        /// Obtiene el DataSet de proyectos
        /// </summary>
        private DataWrapperProyecto DataWrapperProyecto
        {
            get
            {
                if (mDataWrapperProyecto == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mDataWrapperProyecto = proyectoCN.CargarProyectosDeOrganizacionCargaLigeraParaFiltros(IdentidadActual.OrganizacionID.Value);
                }
                return mDataWrapperProyecto;
            }
        }

        /// <summary>
        /// Obtiene el dataWrapper de identidades
        /// </summary>
        private DataWrapperIdentidad DataWrapperIdentidad
        {
            get
            {
                if (mDataWrapperIdentidad == null)
                {
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mDataWrapperIdentidad = identidadCN.ObtenerGruposDeOrganizacion(IdentidadActual.OrganizacionID.Value);
                }
                return mDataWrapperIdentidad;
            }
        }

        #endregion
    }
}