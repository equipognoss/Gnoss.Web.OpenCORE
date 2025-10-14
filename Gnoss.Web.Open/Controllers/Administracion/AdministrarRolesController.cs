using BeetleX.Redis.Commands;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.MVC;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gnoss.Web.Open.Controllers.Administracion
{
    public partial class DiccionarioDePermisos
    {
        public bool CrearRecursoSemantico { get; set; }
        public bool EditarRecursoSemantico { get; set; }
        public bool EliminarRecursoSemantico { get; set; }
        public bool RestaurarVersionRecursoSemantico { get; set; }
        public bool EliminarVersionRecursoSemantico { get; set; }
    }

    public class AdministrarRolesController : ControllerAdministrationWeb
    {
        private AdministrarRolesViewModel mPaginaModel;

        private GestorParametroGeneral mParametrosGeneralesDS;
        private ParametroGeneral mFilaParametrosGenerales = null;
        private ILogger mLogger;
        private ILoggerFactory mLoggerFactory;

        public AdministrarRolesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarRolesController> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mLogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Metodos web

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult Index()
        {
            ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";
            ViewBag.BodyClassPestanya = "admin-roles edicion listado no-max-width-container";
            if (EsAdministracionEcosistema)
            {
                ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
                ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_RolesEcosistema;
            }
            else
            {
                ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
                ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_Roles;
            }

            ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;
            if (ParametrosGeneralesRow.IdiomasDisponibles)
            {
                ViewBag.MultiIdioma = true;
            }
            else
            {
                ViewBag.MultiIdioma = false;
            }

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View(PaginaModel);
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult CrearNuevoRol(string pNombre, string pDescripcion, short pAmbito, string pPermisos, string pPermisosRecursos, string pPermisosEcosistema, string pPermisosContenidos, string pPermisosRecursosSemanticos)
        {
            return GuardarRol(Guid.NewGuid(), pNombre, pDescripcion, pAmbito, pPermisos, pPermisosRecursos, pPermisosEcosistema, pPermisosContenidos, pPermisosRecursosSemanticos);
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult GuardarRol(Guid pRolID, string pNombre, string pDescripcion, short pAmbito, string pPermisos, string pPermisosRecursos, string pPermisosEcosistema, string pPermisosContenidos, string pPermisosRecursosSemanticos)
        {
            try
            {
                if (pRolID.Equals(ProyectoAD.RolAdministrador) || pRolID.Equals(ProyectoAD.RolAdministradorEcosistema))
                {
                    return GnossResultERROR($"El rol de Administrador no puede ser modificado");
                }

                if (!string.IsNullOrEmpty(pDescripcion))
                {
                    pDescripcion = UtilCadenas.EliminarHtmlDeTexto(HttpUtility.UrlDecode(pDescripcion));
                }

                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                if (EsAdministracionEcosistema)
                {
                    RolEcosistema rolEcosistema = new RolEcosistema();
                    rolEcosistema.RolID = pRolID;
                    rolEcosistema.Nombre = pNombre;
                    rolEcosistema.Descripcion = pDescripcion;
                    rolEcosistema.Permisos = ObtenerSumaRolesDeBinario(pPermisosEcosistema, TipoDePermiso.Ecosistema);
                    rolEcosistema.FechaModificacion = DateTime.Now;

                    proyectoCN.GuardarRolEcosistema(rolEcosistema);
                }
                else
                {
                    Rol rol = new Rol();
                    if (pAmbito.Equals((short)AmbitoRol.Ecosistema))
                    {
                        rol.ProyectoID = ProyectoAD.MetaProyecto;
                    }
                    else
                    {
                        rol.ProyectoID = ProyectoSeleccionado.Clave;
                    }
                    rol.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                    rol.RolID = pRolID;
                    rol.Nombre = pNombre;
                    rol.Descripcion = pDescripcion;
                    rol.Tipo = pAmbito;
                    rol.FechaModificacion = DateTime.Now;
                    rol.PermisosAdministracion = ObtenerSumaRolesDeBinario(pPermisos, TipoDePermiso.Comunidad);
                    rol.PermisosRecursos = ObtenerSumaRolesDeBinario(pPermisosRecursos, TipoDePermiso.Recursos);
                    rol.PermisosContenidos = ObtenerSumaRolesDeBinario(pPermisosContenidos, TipoDePermiso.Contenidos);
                    rol.EsRolUsuario = ComprobarSiEsRolDeUsuario(pRolID);

                    Dictionary<Guid, DiccionarioDePermisos> dicPermisosRecursosSemanticos = new Dictionary<Guid, DiccionarioDePermisos>();
                    if (!string.IsNullOrEmpty(pPermisosRecursosSemanticos))
                    {
                        dicPermisosRecursosSemanticos = JsonConvert.DeserializeObject<Dictionary<Guid, DiccionarioDePermisos>>(pPermisosRecursosSemanticos);
                        foreach (Guid ontologiaID in dicPermisosRecursosSemanticos.Keys)
                        {
                            ulong permisos = 0;
                            if (dicPermisosRecursosSemanticos[ontologiaID].CrearRecursoSemantico)
                            {
                                permisos += (ulong)TipoPermisoRecursosSemanticos.Crear;
                            }
                            if (dicPermisosRecursosSemanticos[ontologiaID].EditarRecursoSemantico)
                            {
                                permisos += (ulong)TipoPermisoRecursosSemanticos.Modificar;
                            }
                            if (dicPermisosRecursosSemanticos[ontologiaID].EliminarRecursoSemantico)
                            {
                                permisos += (ulong)TipoPermisoRecursosSemanticos.Eliminar;
                            }
                            if (dicPermisosRecursosSemanticos[ontologiaID].RestaurarVersionRecursoSemantico)
                            {
                                permisos += (ulong)TipoPermisoRecursosSemanticos.RestaurarVersion;
                            }
                            if (dicPermisosRecursosSemanticos[ontologiaID].EliminarVersionRecursoSemantico)
                            {
                                permisos += (ulong)TipoPermisoRecursosSemanticos.EliminarVersion;
                            }

                            if (pAmbito.Equals((short)AmbitoRol.Ecosistema) && permisos > 0)
                            {
                                return GnossResultERROR($"No se puede crear un rol con ámbito de Ecosistema si tiene permisos relacionados con los recursos semánticos");
                            }

                            RolOntologiaPermiso permisoRolOntologia = mEntityContext.RolOntologiaPermiso.Where(x => x.DocumentoID.Equals(ontologiaID) && x.RolID.Equals(pRolID)).FirstOrDefault();
                            if (permisoRolOntologia != null)
                            {
                                permisoRolOntologia.Permisos = permisos;
                            }
                            else
                            {
                                permisoRolOntologia = new RolOntologiaPermiso();
                                permisoRolOntologia.DocumentoID = ontologiaID;
                                permisoRolOntologia.RolID = pRolID;
                                permisoRolOntologia.Permisos = permisos;
                                mEntityContext.RolOntologiaPermiso.Add(permisoRolOntologia);
                            }
                        }
                    }

                    proyectoCN.GuardarRolProyecto(rol);
                }

                proyectoCN.Dispose();

                return GnossResultOK("Los cambios se han guardado correctamente");
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Ha habido un error al guardar los datos del rol", mlogger);
                return GnossResultERROR($"Ha habido un error al guardar los cambios");
            }
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult EliminarRol(Guid pRolID)
        {
            try
            {
                if (pRolID.Equals(ProyectoAD.RolAdministrador) || pRolID.Equals(ProyectoAD.RolAdministradorEcosistema))
                {
                    return GnossResultERROR($"El rol de Administrador no puede ser eliminado");
                }                

                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

                if (EsAdministracionEcosistema)
                {
                    proyectoCN.EliminarRolDeAdministracionEcosistema(pRolID);
                }
                else
                {
                    if (!ComprobarSiEsRolDeUsuario(pRolID))
                    {
						proyectoCN.EliminarRolDeProyecto(pRolID);
					}					
                }

                proyectoCN.Dispose();

                return GnossResultOK($"El rol ha sido eliminado correctamente");
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error al eliminar el rol {pRolID}", mlogger);
                return GnossResultERROR($"Error al intentar eliminar el rol {pRolID}");
            }
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult CargarModalNuevoRol()
        {
            ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";

            PaginaModel.ListaPermisos = CargarListaDePermisosDeRol(Guid.Empty);

            return PartialView("_modal-views/_new-rol-form", PaginaModel);
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult CargarModalVerRol(Guid pRolID)
        {
            ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";
            RolModel rol = CargarInformacionRol(pRolID);
            return PartialView("_modal-views/_see-rol-form", rol);
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult CargarModalEditarRol(Guid pRolID)
        {
            ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";
            RolModel rol = CargarInformacionRol(pRolID);
            return PartialView("_modal-views/_edit-rol-form", rol);
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult CargarModalEliminarRol(Guid pRolID)
        {
            ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";

            return PartialView("_modal-views/_confirm-delete", pRolID);
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult GuardarDescargarFicheroUsuarioInvitado(bool pPermitirDescargarDocUsuInvitado)
        {
            try
            {
                ViewBag.isInEcosistemaPlatform = EsAdministracionEcosistema ? "true" : "false";

                if (!EsAdministracionEcosistema)
                {
                    FilaParametrosGenerales.PermitirUsuNoLoginDescargDoc = pPermitirDescargarDocUsuInvitado;

                    mEntityContext.SaveChanges();

                    return GnossResultOK(UtilIdiomas.GetText("COMADMINPESTANYAS", "GUARDAROK"));
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }

            return GnossResultERROR("Error al guardar la configuración");
        }

        [TypeFilter(typeof(PermisosAdministracion), Arguments = new object[] { new ulong[] { (ulong)PermisoComunidad.GestionarRolesYPermisos } })]
        [TypeFilter(typeof(PermisosAdministracionEcosistema), Arguments = new object[] { new ulong[] { (ulong)PermisoEcosistema.GestionarRolesYPermisosEcosistema } })]
        public ActionResult EliminarMiembroDeRol(Guid pRolID, Guid pMiembroID, bool pEsGrupo)
        {
            try
            {
                string mensaje = string.Empty;
                if (pEsGrupo)
                {
                    RolGrupoIdentidades rolGrupoIdentidades = mEntityContext.RolGrupoIdentidades.Find(pRolID, pMiembroID);
                    if (rolGrupoIdentidades != null)
                    {
                        mEntityContext.RolGrupoIdentidades.Remove(rolGrupoIdentidades);
                        mEntityContext.SaveChanges();
                        mensaje = "El grupo ha sido desvinculado del rol";
                    }
                    else
                    {                       
                        mensaje = "El grupo ya había sido desvinculado del rol";
                    }
                }
                else
                {
                    if (EsAdministracionEcosistema)
                    {
                        RolEcosistemaUsuario rolEcosistemaUsuario = mEntityContext.RolEcosistemaUsuario.Find(pRolID, pMiembroID);
                        if(rolEcosistemaUsuario != null)
                        {
                            if (pRolID.Equals(ProyectoAD.RolAdministradorEcosistema))
                            {
                                int numeroAdministradores = mEntityContext.RolEcosistemaUsuario.Where(x => x.RolID.Equals(ProyectoAD.RolAdministradorEcosistema)).ToList().Count;
                                if (numeroAdministradores == 1)
                                {
                                    return GnossResultERROR("Debe haber al menos un usuario administrador");
								}
                            }
                            mEntityContext.Remove(rolEcosistemaUsuario);
                            mEntityContext.SaveChanges();
                            mensaje = "El usuario ha sido desvinculado del rol";
                        }
                        else
                        {
                            mensaje = "El usuario ya había sido desvinculado del rol";
                        }

                    }
                    else
                    {
                        RolIdentidad rolIdentidad = mEntityContext.RolIdentidad.Find(pRolID, pMiembroID);
                        if (rolIdentidad != null)
                        {
							if (pRolID.Equals(ProyectoAD.RolAdministrador))
							{
								ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
								int numeroAdministradores = proyectoCN.ObtenerIdentidadesAdministradorasDeProyecto(ProyectoSeleccionado.Clave).Count;
								if (numeroAdministradores == 1)
								{
									return GnossResultERROR("Debe haber al menos un usuario administrador");
								}
							}
							mEntityContext.Remove(rolIdentidad);
                            mEntityContext.SaveChanges();
                            mensaje = "El usuario ya había sido desvinculado del rol";
                        }
                        else
                        {
                            mensaje = "El usuario ha sido desvinculado del rol";
                        }
                    }
                }
                return GnossResultOK(mensaje);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
            return GnossResultERROR("Error al eliminar al miembro");

        }

        #endregion

        #region Metodos privados
        private RolModel CargarInformacionRol(Guid pRolID)
        {
            RolModel rol = PaginaModel.ListaRoles.Where(x => x.RolID.Equals(pRolID)).FirstOrDefault();

            ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLContent, BaseURLStatic, ProyectoVirtual, ParametrosGeneralesVirtualRow, IdentidadActual, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);

            rol.ListaMiembros = new List<MiembroDeRol>();
            if (EsAdministracionEcosistema)
            {
                List<RolEcosistemaUsuario> rolesUsuario = mEntityContext.RolEcosistemaUsuario.Where(x => x.RolID.Equals(pRolID)).ToList();
                foreach (RolEcosistemaUsuario rolUsuario in rolesUsuario)
                {
                    MiembroDeRol usuario = new MiembroDeRol();
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    Guid identidadID = identidadCN.ObtenerIdentidadUsuarioEnProyecto(rolUsuario.UsuarioID, ProyectoSeleccionado.Clave);
                    Es.Riam.Gnoss.Elementos.Identidad.Identidad identidad = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(identidadID, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication).ListaIdentidades[identidadID];
                    usuario.ID = rolUsuario.UsuarioID;
                    usuario.Nombre = identidad.NombreCompuesto();
                    usuario.Foto = identidad.UrlImagen;
                    usuario.Url = UrlsSemanticas.GetURLPerfilDeIdentidad(BaseURLIdioma, ProyectoSeleccionado.NombreCorto, UtilIdiomas, identidad);
                    rol.ListaMiembros.Add(usuario);
                }
            }
            else
            {
				ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                List<Guid> identidadesRol = proyectoCN.ObtenerIdentidadesDeRol(pRolID, ProyectoSeleccionado.Clave);

                Dictionary<Guid, ProfileModel> identidades = controladorMVC.ObtenerIdentidadesPorID(identidadesRol);
                foreach (ProfileModel identidadRol in identidades.Values)
                {
                    MiembroDeRol usuario = new MiembroDeRol();
                    usuario.ID = identidadRol.Key;
                    usuario.Nombre = identidadRol.NamePerson;
                    if (!string.IsNullOrEmpty(identidadRol.NameOrganization))
                    {
                        usuario.NombreCompleto = identidadRol.NamePerson + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + identidadRol.NameOrganization;
                    }
                    usuario.Foto = identidadRol.UrlFoto;
                    usuario.Url = identidadRol.UrlPerson;
                    rol.ListaMiembros.Add(usuario);
                }

                List<Guid> gruposRol = mEntityContext.RolGrupoIdentidades.Join(mEntityContext.GrupoIdentidades, rolGrupoIdentidad => rolGrupoIdentidad.GrupoID, grupoIdentidades => grupoIdentidades.GrupoID, (rolGrupoIdentidad, grupoIdentidades) => grupoIdentidades).Where(x => x.RolGrupoIdentidades.Any(y => y.RolID == pRolID) && x.GrupoIdentidadesProyecto.Any(y => y.ProyectoID == ProyectoSeleccionado.Clave)).Select(x => x.GrupoID).ToList();


                Dictionary<Guid, GroupCardModel> grupos = controladorMVC.ObtenerGruposPorID(gruposRol);

                foreach (GroupCardModel grupoRol in grupos.Values)
                {
                    MiembroDeRol grupo = new MiembroDeRol();
                    grupo.ID = grupoRol.Clave;
                    grupo.Nombre = grupoRol.Name;
                    grupo.NombreCompleto = grupoRol.CompleteName;
                    grupo.Url = grupoRol.UrlGroup;
                    grupo.EsGrupo = true;
                    rol.ListaMiembros.Add(grupo);
                }
            }
            return rol;
        }

        private bool ComprobarSiEsRolDeUsuario(Guid pRolID)
        {
			bool esRolUsuario = false;

			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);			
			Rol rolUsuario = proyectoCN.ObtenerRolUsuario(ProyectoSeleccionado.Clave);
			if (rolUsuario != null && rolUsuario.RolID.Equals(pRolID))
			{
				esRolUsuario = true;
			}
            proyectoCN.Dispose();

            return esRolUsuario;
		}

        private ulong ObtenerSumaRolesDeBinario(string pCadenaBinaria, TipoDePermiso pTipoPermiso)
        {
            ulong resultado = 0;
            ulong[] valoresPermisos = null;
            switch (pTipoPermiso)
            {
                case TipoDePermiso.Comunidad:
                    valoresPermisos = (ulong[])Enum.GetValues(typeof(PermisoComunidad));
                    break;
                case TipoDePermiso.Contenidos:
                    valoresPermisos = (ulong[])Enum.GetValues(typeof(PermisoContenidos));
                    break;
                case TipoDePermiso.Recursos:
                    valoresPermisos = (ulong[])Enum.GetValues(typeof(PermisoRecursos));
                    break;
                case TipoDePermiso.Ecosistema:
                    valoresPermisos = (ulong[])Enum.GetValues(typeof(PermisoEcosistema));
                    break;
            }

            for (int i = 0; i < pCadenaBinaria.Length; i++)
            {
                if (pCadenaBinaria[i].Equals('1'))
                {
                    resultado += valoresPermisos[i];
                }
            }

            return resultado;
        }

        private List<PermisoModel> CargarListaDePermisosDeRol(Guid pRolID)
        {
            List<PermisoModel> listaPermisos = new List<PermisoModel>();

            if (EsAdministracionEcosistema)
            {
                RolEcosistema rol = mEntityContext.RolEcosistema.FirstOrDefault(x => x.RolID.Equals(pRolID));
                if (rol != null || pRolID.Equals(Guid.Empty))
                {
                    foreach (PermisoEcosistema permisoEcosistema in Enum.GetValues(typeof(PermisoEcosistema)))
                    {
                        PermisoModel permiso = new PermisoModel();
                        permiso.Nombre = permisoEcosistema.ToString();
                        if (!pRolID.Equals(Guid.Empty))
                        {
                            permiso.Concedido = UtilPermisos.TienePermiso(rol.Permisos, (ulong)permisoEcosistema);
                        }
                        permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoEcosistema).Nombre;

                        listaPermisos.Add(permiso);
                    }
                }
            }
            else
            {
                Rol rol = mEntityContext.Rol.FirstOrDefault(x => x.RolID.Equals(pRolID));
                if (rol != null || pRolID.Equals(Guid.Empty))
                {
                    foreach (PermisoComunidad permisoComunidad in Enum.GetValues(typeof(PermisoComunidad)))
                    {
                        PermisoModel permiso = new PermisoModel();
                        permiso.Nombre = permisoComunidad.ToString();
                        if (!pRolID.Equals(Guid.Empty))
                        {
                            permiso.Concedido = UtilPermisos.TienePermiso(rol.PermisosAdministracion, (ulong)permisoComunidad);
                        }
                        permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoComunidad).Nombre;

                        listaPermisos.Add(permiso);
                    }
                    foreach (PermisoContenidos permisoContenidos in Enum.GetValues(typeof(PermisoContenidos)))
                    {
                        PermisoModel permiso = new PermisoModel();
                        permiso.Nombre = permisoContenidos.ToString();
                        if (!pRolID.Equals(Guid.Empty))
                        {
                            permiso.Concedido = UtilPermisos.TienePermiso(rol.PermisosContenidos, (ulong)permisoContenidos);
                        }
                        permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoContenidos).Nombre;

                        listaPermisos.Add(permiso);
                    }
                    foreach (PermisoRecursos permisoRecurso in Enum.GetValues(typeof(PermisoRecursos)))
                    {
                        PermisoModel permiso = new PermisoModel();
                        permiso.Nombre = permisoRecurso.ToString();
                        if (!pRolID.Equals(Guid.Empty))
                        {
                            permiso.Concedido = UtilPermisos.TienePermiso(rol.PermisosRecursos, (ulong)permisoRecurso);
                        }
                        permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoRecurso).Nombre;

                        listaPermisos.Add(permiso);
                    }
                }
            }

            return listaPermisos;
        }

        private List<RolModel> CargarListaDeRoles(Dictionary<Guid, string> pOntologias)
        {
            List<RolModel> listaRoles = new List<RolModel>();
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

            if (EsAdministracionEcosistema)
            {
                List<RolEcosistema> rolesEcosistema = proyectoCN.ObtenerRolesAdministracionEcosistema();
                foreach (RolEcosistema rolEcosistema in rolesEcosistema)
                {
                    RolModel rolModel = new RolModel();
                    rolModel.RolID = rolEcosistema.RolID;
                    rolModel.Nombre = HttpUtility.HtmlDecode(rolEcosistema.Nombre);
                    rolModel.FechaModificacion = rolEcosistema.FechaModificacion.ToString("dd/MM/yyyy");
                    rolModel.Descripcion = HttpUtility.HtmlDecode(rolEcosistema.Descripcion);
                    rolModel.PermisosAdministracionEcosistema = rolEcosistema.Permisos;
                    rolModel.Editable = rolEcosistema.RolID != ProyectoAD.RolAdministradorEcosistema;
                    rolModel.Borrable = rolEcosistema.RolID != ProyectoAD.RolAdministradorEcosistema;
                    rolModel.ListaPermisos = CargarListaDePermisosDeRol(rolEcosistema.RolID);

                    listaRoles.Add(rolModel);
                }
            }
            else
            {
                List<Rol> roles = proyectoCN.ObtenerRolesDeProyecto(ProyectoSeleccionado.Clave);
                foreach (Rol rol in roles)
                {
                    RolModel rolModel = new RolModel();
                    rolModel.RolID = rol.RolID;
                    rolModel.Nombre = HttpUtility.HtmlDecode(rol.Nombre);
                    rolModel.Descripcion = HttpUtility.HtmlDecode(rol.Descripcion);
                    rolModel.FechaModificacion = rol.FechaModificacion.ToString("dd/MM/yyyy");
                    rolModel.Tipo = rol.Tipo;
                    rolModel.ProyectoID = rol.ProyectoID;
                    rolModel.PermisosAdministracion = rol.PermisosAdministracion;
                    rolModel.PermisosContenidos = rol.PermisosContenidos;
                    rolModel.PermisosRecursos = rol.PermisosRecursos;
                    rolModel.ListaPermisosRecursosSemanticos = new List<PermisoRecursoSemantico>();
                    rolModel.Editable = rol.RolID != ProyectoAD.RolAdministrador;
                    rolModel.Borrable = rol.RolID != ProyectoAD.RolAdministrador && !rol.EsRolUsuario;
                    rolModel.EsRolUsuario = rol.EsRolUsuario;

					foreach (Guid ontologia in pOntologias.Keys)
					{
						RolOntologiaPermiso permiso = mEntityContext.RolOntologiaPermiso.Where(x => x.RolID.Equals(rol.RolID) && x.DocumentoID.Equals(ontologia)).FirstOrDefault();
						PermisoRecursoSemantico permisoModel = new PermisoRecursoSemantico();
						bool crear = false;
						bool editar = false;                        
                        bool eliminar = false;
                        bool restaurar = false;
                        bool eliminarVersion = false;
						if (permiso != null)
                        {
							crear = UtilPermisos.TienePermiso(permiso.Permisos, (ulong)TipoPermisoRecursosSemanticos.Crear);
							editar = UtilPermisos.TienePermiso(permiso.Permisos, (ulong)TipoPermisoRecursosSemanticos.Modificar);
							eliminar = UtilPermisos.TienePermiso(permiso.Permisos, (ulong)TipoPermisoRecursosSemanticos.Eliminar);
							restaurar = UtilPermisos.TienePermiso(permiso.Permisos, (ulong)TipoPermisoRecursosSemanticos.RestaurarVersion);
							eliminarVersion = UtilPermisos.TienePermiso(permiso.Permisos, (ulong)TipoPermisoRecursosSemanticos.EliminarVersion);
						}
						
						permisoModel.OntologiaID = ontologia;
						permisoModel.Nombre = UtilCadenas.ObtenerTextoDeIdioma(docCN.ObtenerTituloDocumentoPorID(ontologia), UtilIdiomas.LanguageCode, IdiomaPorDefecto);
                        permisoModel.PermisoCrear = crear;
						permisoModel.PermisoEditar = editar;
						permisoModel.PermisoEliminar = eliminar;
						permisoModel.PermisoRestaurarVersion = restaurar;
						permisoModel.PermisoEliminarVersion = eliminarVersion;

						rolModel.ListaPermisosRecursosSemanticos.Add(permisoModel);
					}
                    
                    rolModel.ListaPermisos = CargarListaDePermisosDeRol(rol.RolID);

                    listaRoles.Add(rolModel);
                }
            }

            proyectoCN.Dispose();
            docCN.Dispose();

            return listaRoles;
        }

        #endregion

        private ParametroGeneral FilaParametrosGenerales
        {
            get
            {
                if (mFilaParametrosGenerales == null)
                {
                    mFilaParametrosGenerales = new ParametroGeneral();
                    mFilaParametrosGenerales = ParametrosGeneralesDS.ListaParametroGeneral.Find(parametroGeneral => parametroGeneral.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametroGeneral.ProyectoID.Equals(ProyectoSeleccionado.Clave));
                }
                return mFilaParametrosGenerales;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales
        /// </summary>
        private GestorParametroGeneral ParametrosGeneralesDS
        {
            get
            {
                if (mParametrosGeneralesDS == null)
                {
                    ParametroGeneralGBD controllerParametrosGeneral = new ParametroGeneralGBD(mEntityContext);
                    mParametrosGeneralesDS = new GestorParametroGeneral();
                    mParametrosGeneralesDS = controllerParametrosGeneral.ObtenerParametrosGeneralesDeProyecto(mParametrosGeneralesDS, ProyectoSeleccionado.Clave);
                }
                return mParametrosGeneralesDS;
            }
        }

        private AdministrarRolesViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    try
                    {
                        mPaginaModel = new AdministrarRolesViewModel();

                        ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                        if (ParametrosGeneralesRow.IdiomasDisponibles)
                        {
                            mPaginaModel.ListaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
                        }
                        else
                        {
                            mPaginaModel.ListaIdiomas = new Dictionary<string, string>();
                            mPaginaModel.ListaIdiomas.Add(IdiomaPorDefecto, paramCL.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
                        }
                        mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;

                        mPaginaModel.PermitirDescargarDocUsuInvitado = FilaParametrosGenerales.PermitirUsuNoLoginDescargDoc;

						DataWrapperDocumentacion dwDocumentacion = new DataWrapperDocumentacion();
						DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
						docCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dwDocumentacion, true, false, false);
                        Dictionary<Guid, string> diccionarioOntologias = dwDocumentacion.ListaDocumento.ToDictionary(k => k.DocumentoID, k => k.Titulo);
                        mPaginaModel.DiccionarioOntologias = diccionarioOntologias;
						docCN.Dispose();
						mPaginaModel.ListaRoles = CargarListaDeRoles(diccionarioOntologias);  
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, $"Error al cargar la página de roles", mlogger);
                    }
                }

                return mPaginaModel;
            }
        }
    }
}
