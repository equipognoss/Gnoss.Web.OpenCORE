using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
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


namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PermisosPaginaViewModel
    {
        public TipoPaginaAdministracion TipoPagina { get; set; }

        public List<PerfilPermisoModel> Perfiles { get; set; }

        [Serializable]
        public partial class PerfilPermisoModel
        {
            /// <summary>
            /// 
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarPermisosPaginasController : ControllerAdministrationWeb
	{
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public AdministrarPermisosPaginasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<AdministrarPermisosPaginasController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        private List<PermisosPaginaViewModel> mPaginaModel = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View(PaginaModel);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(List<PermisosPaginaViewModel> Permisos)
        {
            GuardarLogAuditoria();
            string errores = ComprobarErrores(Permisos);

            if (string.IsNullOrEmpty(errores))
            {
                GuardarPaginasPermisos(Permisos);
                return GnossResultOK();
            }
            else
            {
                return GnossResultERROR(errores);
            }
        }

        #region Métodos Privados

        private void GuardarPaginasPermisos(List<PermisosPaginaViewModel> pPermisos)
        {
            Dictionary<Guid, Guid> listaPerfilesUsuarios = ObtenerUsuariosDePerfiles(pPermisos);


            List<PermisosPaginasUsuarios> filasPermisoProyecto = mEntityContext.PermisosPaginasUsuarios.Where(fila => fila.ProyectoID.Equals(ProyectoSeleccionado.Clave)).ToList();

            //Añadir las nuevas
            foreach (PermisosPaginaViewModel permisosPagina in pPermisos)
            {
                TipoPaginaAdministracion pagina = permisosPagina.TipoPagina;

                List<Guid> usuariosAñadidos = new List<Guid>();

                List<PermisosPaginasUsuarios> filasPermisoProyectoPagina = filasPermisoProyecto.Where(fila => fila.Pagina.Equals((short)pagina)).ToList();

                if (permisosPagina.Perfiles != null)
                {
                    foreach (PermisosPaginaViewModel.PerfilPermisoModel perfil in permisosPagina.Perfiles)
                    {
                        Guid usuarioID = listaPerfilesUsuarios[perfil.Key];
                        usuariosAñadidos.Add(usuarioID);

                        List<PermisosPaginasUsuarios> filasUsuario = filasPermisoProyectoPagina.Where(fila => fila.UsuarioID.Equals(usuarioID)).ToList();
                        List<Guid> idsUsuariosBD = filasUsuario.Select(fila => fila.UsuarioID).ToList();

                        if (!usuarioID.Equals(Guid.Empty) && !idsUsuariosBD.Contains(usuarioID))
                        {
                            PermisosPaginasUsuarios filaNuevoUsuario = CrearFilaPermisosPaginasUsuarios(pagina, usuarioID, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                            mEntityContext.PermisosPaginasUsuarios.Add(filaNuevoUsuario);
                        }
                    }
                }

                //Eliminar las eliminadas
                List<Guid> idsUsuariosEliminar = filasPermisoProyectoPagina.Select(fila => fila.UsuarioID).ToList().Except(usuariosAñadidos).ToList();

                foreach (Guid usuarioIDEliminar in idsUsuariosEliminar)
                {
                    if (!usuarioIDEliminar.Equals(Guid.Empty))
                    {
                        PermisosPaginasUsuarios filaUsuarioEliminar = filasPermisoProyectoPagina.Find(fila => fila.UsuarioID == usuarioIDEliminar);
                        mEntityContext.PermisosPaginasUsuarios.Remove(filaUsuarioEliminar);
                    }
                }
            }

            mEntityContext.SaveChanges();
        }

        private PermisosPaginasUsuarios CrearFilaPermisosPaginasUsuarios(TipoPaginaAdministracion pPagina, Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID)
        {
            PermisosPaginasUsuarios filaUsuario = new PermisosPaginasUsuarios();
            filaUsuario.Pagina = (short)pPagina;
            filaUsuario.UsuarioID = pUsuarioID;
            filaUsuario.OrganizacionID = pOrganizacionID;
            filaUsuario.ProyectoID = pProyectoID;
            return filaUsuario;
        }

        private Dictionary<Guid, Guid> ObtenerUsuariosDePerfiles(List<PermisosPaginaViewModel> pPermisos)
        {
            List<Guid> listaPerfilesID = new List<Guid>();

            foreach (PermisosPaginaViewModel permisosPagina in pPermisos)
            {
                if (permisosPagina.Perfiles != null)
                {
                    TipoPaginaAdministracion pagina = permisosPagina.TipoPagina;

                    foreach (PermisosPaginaViewModel.PerfilPermisoModel perfil in permisosPagina.Perfiles)
                    {
                        if (!listaPerfilesID.Contains(perfil.Key))
                        {
                            listaPerfilesID.Add(perfil.Key);
                        }
                    }
                }
            }

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

            Dictionary<Guid, Guid> dicPerfilesIDUsuariosID = identCN.ObtenerUsuariosIDPorPerfilID(listaPerfilesID);

            identCN.Dispose();

            return dicPerfilesIDUsuariosID;
        }

        private List<PermisosPaginaViewModel> PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new List<PermisosPaginaViewModel>();

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    List<PermisosPaginasUsuarios> filasPermisos = proyCN.ObtenerPermisosPaginasProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                    proyCN.Dispose();

                    if (filasPermisos.Count > 0)
                    {
                        List<Guid> idsUsuarios = filasPermisos.Select(fila => fila.UsuarioID).ToList();

                        IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        Dictionary<Guid, Guid> usuariosIDPerfilesID = identCN.ObtenerPerfilesIDPorUsuariosIDEnProyecto(idsUsuarios, ProyectoSeleccionado.Clave);

                        foreach (PermisosPaginasUsuarios filaPermiso in filasPermisos)
                        {
                            TipoPaginaAdministracion tipoPaginaAdmin = (TipoPaginaAdministracion)filaPermiso.Pagina;

                            PermisosPaginaViewModel permiso = mPaginaModel.Find(permisoPagina => permisoPagina.TipoPagina == tipoPaginaAdmin);

                            if (permiso == null)
                            {
                                permiso = new PermisosPaginaViewModel();
                                permiso.TipoPagina = tipoPaginaAdmin;
                                permiso.Perfiles = new List<PermisosPaginaViewModel.PerfilPermisoModel>();

                                mPaginaModel.Add(permiso);
                            }

                            PermisosPaginaViewModel.PerfilPermisoModel perfilPermiso = new PermisosPaginaViewModel.PerfilPermisoModel();
                            perfilPermiso.Key = usuariosIDPerfilesID[filaPermiso.UsuarioID];
                            perfilPermiso.Name = identCN.ObtenerNombredePerfilID(perfilPermiso.Key);

                            permiso.Perfiles.Add(perfilPermiso);
                        }

                        identCN.Dispose();
                    }
                }
                return mPaginaModel;
            }
        }

        private string ComprobarErrores(List<PermisosPaginaViewModel> pPermisos)
        {
            string error = "";
            error = ComprobarPerfilesRepetidos(pPermisos);
            return error;
        }

        private string ComprobarPerfilesRepetidos(List<PermisosPaginaViewModel> pPermisos)
        {
            List<Guid> listaIDs = new List<Guid>();

            foreach (PermisosPaginaViewModel permisosPagina in pPermisos)
            {
                if (permisosPagina.Perfiles != null)
                {
                    TipoPaginaAdministracion pagina = permisosPagina.TipoPagina;

                    foreach (PermisosPaginaViewModel.PerfilPermisoModel perfil in permisosPagina.Perfiles)
                    {
                        if (listaIDs.Contains(perfil.Key))
                        {
                            return "PERFIL REPETIDO|||" + (short)pagina;
                        }
                    }
                }
            }
            return "";
        }

        #endregion
    }
}
