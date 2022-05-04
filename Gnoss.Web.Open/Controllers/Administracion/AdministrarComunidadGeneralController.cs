using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
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
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Modelo de la página de administrar comunidad general
    /// </summary>
    public class AdministrarComunidadGeneralModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ListaIdiomas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ListaIdiomasPlataforma { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IdiomaPorDefecto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StateCommunity State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Desciption { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ImageCoordenadas ImageHead { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ImageCoordenadas ImageLogo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CategoryModel> EcosistemaCategories { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Guid> SelectedCategories { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool MultiLanguage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool HasMultiLanguageObjects { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public class StateCommunity
        {
            /// <summary>
            /// 
            /// </summary>
            public short State { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public DateTime ReOpenDate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string CauseOfClose { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int DaysOfGrace { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool CanBeOpened { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class ImageCoordenadas
        {
            /// <summary>
            /// 
            /// </summary>
            public string Ruta { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Pos_X_0 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Pos_Y_0 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Pos_X_1 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Pos_Y_1 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Alto { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Ancho { get; set; }
        }
    }

    /// <summary>
    /// Controller de administrar comunidad general
    /// </summary>
    public partial class AdministrarComunidadGeneralController : ControllerBaseWeb
    {
        public AdministrarComunidadGeneralController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        /// <summary>
        /// Ancho minimo de la imagen a subir
        /// </summary>
        const int minAnchoImg = 60;
        /// <summary>
        /// Ancho maximo de la imagen a subir
        /// </summary>
        const int maxAnchoImg = 850;
        /// <summary>
        /// Alto minimo de la imagen a subir
        /// </summary>
        const int minAltoImg = 20;
        /// <summary>
        /// Alto maximo de la imagen a subir
        /// </summary>
        const int maxAltoImg = 400;

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarComunidadGeneralModel mPaginaModel = null;
        /// <summary>
        /// 
        /// </summary>
        //private ParametroGeneralDS mParametrosGeneralesDS;
        private GestorParametroGeneral mParametrosGeneralesDS;
        /// <summary>
        /// 
        /// </summary>
        private ParametroGeneral mFilaParametrosGenerales = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            return View(PaginaModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult AbrirComunidad()
        {
            EliminarPersonalizacionVistas();
            if (ProyectoSeleccionado.Estado.Equals((short)EstadoProyecto.Definicion))
            {
                if (Comunidad.Categories.Count > 0)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestionProyecto gestProy = new GestionProyecto(proyCN.ObtenerProyectoPorID(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    Elementos.ServiciosGenerales.Proyecto Proy = gestProy.ListaProyectos[ProyectoSeleccionado.Clave];

                    Proy.Estado = (short)EstadoProyecto.Abierto;

                    proyCN.GuardarProyectos();

                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    proyCL.InvalidarFilaProyecto(Proy.Clave);
                    proyCL.InvalidarCabeceraMVC(Proy.Clave);
                    proyCL.InvalidarComunidadMVC(Proy.Clave);

                    AdministrarComunidadGeneralModel.StateCommunity State = new AdministrarComunidadGeneralModel.StateCommunity();
                    State.State = (short)EstadoProyecto.Abierto;
                    State.DaysOfGrace = 15;

                    return PartialView("EstadoComunidad", State);
                }
                return GnossResultERROR("La comunidad no tiene un tesauro de categorias");
            }

            return GnossResultERROR("La comunidad no esta en definicion y no se ha podido abrir");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(AdministrarComunidadGeneralModel Options)
        {
            string error = ComprobarErrores(Options);

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }

            bool cambioNombreProy = false;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionProyecto gestProy = new GestionProyecto(proyCN.ObtenerProyectoPorID(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
            Elementos.ServiciosGenerales.Proyecto Proy = gestProy.ListaProyectos[ProyectoSeleccionado.Clave];

            EstadoProyecto estadoAnterior = (EstadoProyecto)Proy.Estado;

            FilaParametrosGenerales.IdiomaDefecto = Options.IdiomaPorDefecto;

            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService);
            servicioImagenes.Url = UrlIntragnossServicios;

            GuardarImagenHead(servicioImagenes, Options.ImageHead);
            GuardarImagenLogo(servicioImagenes, Options.ImageLogo);

            if (!Proy.Nombre.Equals(Options.Name))
            {
                cambioNombreProy = true;
                Proy.Nombre = Options.Name;
            }

            bool HasMultiLanguageObjects = TieneObjetosMultiIdioma();

            FilaParametrosGenerales.IdiomasDisponibles = HasMultiLanguageObjects || Options.MultiLanguage;

            if (!FilaParametrosGenerales.IdiomasDisponibles)
            {
                Proy.FilaProyecto.Descripcion = UtilCadenas.ObtenerTextoDeIdioma(Options.Desciption, UtilIdiomas.LanguageCode, null);
            }
            else
            {
                Proy.FilaProyecto.Descripcion = Options.Desciption;
            }

            Proy.Tags = Options.Tags;

            GuardarCategorizacion(Proy, Options.SelectedCategories);

            bool? recalcularVisibilidadDocumentosRestringiendola = GuardarEstadoProy(Proy, Options.State);

            try
            {
                mEntityContext.SaveChanges();

                if (Options.State.State.Equals(EstadoProyecto.Abierto) && (estadoAnterior == EstadoProyecto.Cerrado || estadoAnterior == EstadoProyecto.Definicion))
                {
                    if (estadoAnterior == EstadoProyecto.Definicion)
                    {
                        //Notifico que se ha abierto el proyecto
                        LiveUsuariosCN liveUsuariosCN = new LiveUsuariosCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        liveUsuariosCN.InsertarFilaEnColaUsuarios(ProyectoAD.MetaProyecto, Proy.Clave, (int)AccionLive.ComunidadAbierta, (int)TipoLive.Comunidad, null);
                    }

                    //Agregamos el evento a la cola del live
                    LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    LiveDS liveDS = new LiveDS();

                    List<Guid> listusuariosAdminID = new List<Guid>();
                    foreach (AdministradorProyecto filaAdmin in Proy.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto)
                    {
                        if (filaAdmin.Tipo == (short)TipoRolUsuario.Administrador)
                        {
                            listusuariosAdminID.Add(filaAdmin.UsuarioID);
                        }
                    }

                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> identidadesAdmin = identCN.ObtenerIdentidadesIDDeusuariosEnProyecto(Proy.Clave, listusuariosAdminID);
                    ControladorBase controladorBase = mControladorBase;

                    foreach (Guid idAdmin in identidadesAdmin)
                    {
                        try
                        {
                            controladorBase.InsertarFilaEnColaRabbitMQ(Proy.Clave, idAdmin, (int)AccionLive.ComunidadAbierta, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                            liveDS.Cola.AddColaRow(Proy.Clave, idAdmin, (int)AccionLive.ComunidadAbierta, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta, null);
                        }

                    }

                    liveCN.ActualizarBD(liveDS);
                    liveCN.Dispose();
                    liveDS.Dispose();
                }

                if (Options.State.State.Equals((short)EstadoProyecto.Abierto))
                {
                    ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    controladorProyecto.ActualizarModeloBase(Proy.Clave, PrioridadBase.Alta);
                }

                if (recalcularVisibilidadDocumentosRestringiendola != null)
                {
                    ControladorDocumentacion.EstablecePrivacidadTodosRecursosComunidadEnMetaBuscador(Proy.Clave, IdentidadActual, recalcularVisibilidadDocumentosRestringiendola.Value);
                }

                ParametroGeneralCL paramGenCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramGenCL.InvalidarCacheParametrosGeneralesDeProyecto(Proy.Clave);

                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarFilaProyecto(Proy.Clave);
                proyCL.InvalidarCabeceraMVC(Proy.Clave);
                proyCL.InvalidarComunidadMVC(Proy.Clave);

                if (cambioNombreProy)
                {
                    //Elimino la caché de todos los miembros para que se actualice su menú de comunidades
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> perfilesComunidadID = identidadCN.ObtenerPerfilesIDDeProyecto(ProyectoSeleccionado.Clave);

                    IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCL.EliminarPerfilesMVC(perfilesComunidadID);

                    identidadCN.Dispose();
                    identidadCL.Dispose();
                }
            }
            catch (Exception)
            {
                GnossResultERROR();
            }

            return GnossResultOK();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool TieneObjetosMultiIdioma()
        {
            bool HasMultiLanguageObjects = false;

            foreach (CategoryModel cat in Comunidad.Categories)
            {
                if (cat.LanguageName.Contains("@"))
                {
                    HasMultiLanguageObjects = true;
                    break;
                }
            }

            if (!HasMultiLanguageObjects)
            {
                foreach (ProyectoGadget fila in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoGadget)
                {
                    if (fila.MultiIdioma)
                    {
                        HasMultiLanguageObjects = true;
                        break;
                    }
                }
            }
            return HasMultiLanguageObjects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyecto"></param>
        /// <param name="pEstado"></param>
        /// <returns></returns>
        private bool? GuardarEstadoProy(Elementos.ServiciosGenerales.Proyecto pProyecto, AdministrarComunidadGeneralModel.StateCommunity pEstado)
        {
            bool? recalcularVisibilidadDocumentosRestringiendola = null;

            if (pProyecto.Estado != pEstado.State)
            {
                if ((pProyecto.Estado != (short)EstadoProyecto.Cerrado || pProyecto.Estado != (short)EstadoProyecto.CerradoTemporalmente) && (pEstado.State == (short)EstadoProyecto.Cerrado || pEstado.State == (short)EstadoProyecto.CerradoTemporalmente))
                {
                    //Hay que recalcular la visibilidad de todos los documentos de la comunidad para restringirla.
                    recalcularVisibilidadDocumentosRestringiendola = true;
                }
                else if ((pProyecto.Estado == (short)EstadoProyecto.Cerrado || pProyecto.Estado == (short)EstadoProyecto.CerradoTemporalmente) && (pEstado.State != (short)EstadoProyecto.Cerrado || pEstado.State != (short)EstadoProyecto.CerradoTemporalmente))
                {
                    //Hay que devolver la visibilidad original (recalculada) de todos los documentos de la comunidad.
                    recalcularVisibilidadDocumentosRestringiendola = false;
                }

                if (pEstado.State.Equals((short)EstadoProyecto.CerradoTemporalmente))
                {
                    ProyectoCerradoTmp filasProyCerradoTmp = pProyecto.FilaProyecto.ProyectoCerradoTmp;
                    if (filasProyCerradoTmp != null)
                    {
                        filasProyCerradoTmp.FechaReapertura = pEstado.ReOpenDate;
                    }
                    else
                    {
                        ProyectoCerradoTmp proyectoCerradoTmp = new ProyectoCerradoTmp();
                        proyectoCerradoTmp.OrganizacionID = pProyecto.FilaProyecto.OrganizacionID;
                        proyectoCerradoTmp.ProyectoID = pProyecto.Clave;
                        proyectoCerradoTmp.Motivo = pEstado.CauseOfClose;
                        proyectoCerradoTmp.FechaCierre = DateTime.Now;
                        proyectoCerradoTmp.FechaReapertura = pEstado.ReOpenDate;
                        pProyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoCerradoTmp.Add(proyectoCerradoTmp);
                        if (!(mEntityContext.ProyectoCerradoTmp.Any(proy => proy.OrganizacionID.Equals(proyectoCerradoTmp.OrganizacionID) && proy.ProyectoID.Equals(proyectoCerradoTmp.ProyectoID))))
                        {
                            mEntityContext.ProyectoCerradoTmp.Add(proyectoCerradoTmp);
                        }
                    }
                }
                else if (pEstado.State.Equals((short)EstadoProyecto.Cerrandose))
                {
                    ProyectoCerrandose filasProyCerrandose = pProyecto.FilaProyecto.ProyectoCerrandose;
                    if (filasProyCerrandose != null)
                    {
                        filasProyCerrandose.PeriodoDeGracia = pEstado.DaysOfGrace;
                    }
                    else
                    {
                        ProyectoCerrandose proyectoCerrandose = new ProyectoCerrandose();
                        proyectoCerrandose.OrganizacionID = pProyecto.FilaProyecto.OrganizacionID;
                        proyectoCerrandose.ProyectoID = pProyecto.Clave;
                        proyectoCerrandose.FechaCierre = DateTime.Now;
                        proyectoCerrandose.PeriodoDeGracia = pEstado.DaysOfGrace;
                        pProyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoCerrandose.Add(proyectoCerrandose);
                        if (!(mEntityContext.ProyectoCerrandose.Any(proy => proy.OrganizacionID.Equals(proyectoCerrandose.OrganizacionID) && proy.ProyectoID.Equals(proyectoCerrandose.ProyectoID))))
                        {
                            mEntityContext.ProyectoCerrandose.Add(proyectoCerrandose);
                        }
                    }
                }

                if (!pEstado.State.Equals((short)EstadoProyecto.CerradoTemporalmente))
                {
                    ProyectoCerradoTmp filasProyCerradoTmp = pProyecto.FilaProyecto.ProyectoCerradoTmp;
                    if (filasProyCerradoTmp != null)
                    {
                        pProyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoCerradoTmp.Remove(pProyecto.FilaProyecto.ProyectoCerradoTmp);
                        mEntityContext.EliminarElemento(pProyecto.FilaProyecto.ProyectoCerradoTmp);
                        pProyecto.FilaProyecto.ProyectoCerradoTmp = null;
                        //filasProyCerradoTmp.Delete();
                    }
                }

                if (pEstado.State.Equals((short)EstadoProyecto.Cerrandose))
                {
                    ProyectoCerrandose filasProyCerrandose = pProyecto.FilaProyecto.ProyectoCerrandose;
                    if (filasProyCerrandose != null)
                    {
                        pProyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoCerrandose.Remove(pProyecto.FilaProyecto.ProyectoCerrandose);
                        mEntityContext.EliminarElemento(pProyecto.FilaProyecto.ProyectoCerrandose);
                        pProyecto.FilaProyecto.ProyectoCerrandose = null;
                        //filaProyCerrandose.Delete();
                    }
                }

                pProyecto.Estado = pEstado.State;
            }

            return recalcularVisibilidadDocumentosRestringiendola;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyecto"></param>
        /// <param name="pCategoriasSeleccionadas"></param>
        private void GuardarCategorizacion(Elementos.ServiciosGenerales.Proyecto pProyecto, List<Guid> pCategoriasSeleccionadas)
        {
            pProyecto.GestorProyectos.GestionTesauro = new GestionTesauro(new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerTesauroDeProyecto(ProyectoAD.MetaProyecto), mLoggingService, mEntityContext);
            Guid tesauroID = (Guid)pProyecto.GestorProyectos.GestionTesauro.TesauroDW.ListaTesauroProyecto.Where(item => item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).FirstOrDefault().TesauroID;
            List<Guid> listaCatsAnteriores = new List<Guid>();
            foreach (ProyectoAgCatTesauro filaProyectoAgCatTesauro in pProyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoAgCatTesauro)
            {
                listaCatsAnteriores.Add(filaProyectoAgCatTesauro.CategoriaTesauroID);
                if (!pCategoriasSeleccionadas.Contains(filaProyectoAgCatTesauro.CategoriaTesauroID))
                {
                    //pProyecto.GestorProyectos.DataWrapperProyecto.ListaProyectoAgCatTesauro.Remove(filaProyectoAgCatTesauro);
                    mEntityContext.EliminarElemento(filaProyectoAgCatTesauro);
                }
            }
            foreach (Guid CategoriaID in pCategoriasSeleccionadas)
            {
                if (!listaCatsAnteriores.Contains(CategoriaID))
                {
                    if (pProyecto.GestorProyectos.GestionTesauro.TesauroDW.ListaCategoriaTesauro.Any(item => item.TesauroID.Equals(tesauroID) && item.CategoriaTesauroID.Equals(CategoriaID)))
                    {
                        ProyectoAgCatTesauro proyectoAgCatTesauro = new ProyectoAgCatTesauro();
                        proyectoAgCatTesauro.OrganizacionID = pProyecto.FilaProyecto.OrganizacionID;
                        proyectoAgCatTesauro.ProyectoID = pProyecto.Clave;
                        proyectoAgCatTesauro.TesauroID = tesauroID;
                        proyectoAgCatTesauro.CategoriaTesauroID = CategoriaID;
                        pProyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoAgCatTesauro.Add(proyectoAgCatTesauro);
                        if (!(mEntityContext.ProyectoAgCatTesauro.Any(proy => proy.OrganizacionID.Equals(proyectoAgCatTesauro.OrganizacionID) && proy.ProyectoID.Equals(proyectoAgCatTesauro.ProyectoID) && proy.TesauroID.Equals(proyectoAgCatTesauro.TesauroID) && proy.CategoriaTesauroID.Equals(proyectoAgCatTesauro.CategoriaTesauroID))))
                        {
                            mEntityContext.ProyectoAgCatTesauro.Add(proyectoAgCatTesauro);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pServicioImagenes"></param>
        /// <param name="pImagenCabecera"></param>
        private void GuardarImagenHead(ServicioImagenes pServicioImagenes, AdministrarComunidadGeneralModel.ImageCoordenadas pImagenCabecera)
        {
            string ruta = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower();
            string rutaTemp = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "_sup_grande_temp";
            string rutaAux = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "_sup_grande_temp2";

            string rutaCompletaAux = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + rutaAux + ".png";

            if (rutaCompletaAux.Equals(pImagenCabecera.Ruta))
            {
                //Guardar imagen temporal nueva si ha cambiado
                byte[] bytesFichero = pServicioImagenes.ObtenerImagen(rutaAux, ".png");
                if (bytesFichero != null)
                {
                    pServicioImagenes.AgregarImagen(bytesFichero, rutaTemp, ".png");

                    pServicioImagenes.BorrarImagen(rutaAux + ".png");
                }
            }

            if (!string.IsNullOrEmpty(pImagenCabecera.Ruta))
            {
                AdministrarComunidadGeneralModel.ImageCoordenadas ImageHead = CargarImagenSup();

                bool imagenCambiada = !ImageHead.Ruta.Equals(pImagenCabecera.Ruta);
                bool tamañoCambiado = (!ImageHead.Ancho.Equals(pImagenCabecera.Ancho) || !ImageHead.Alto.Equals(pImagenCabecera.Alto));
                bool coordenadasCambiadas = (!ImageHead.Pos_X_0.Equals(pImagenCabecera.Pos_X_0) || !ImageHead.Pos_X_1.Equals(pImagenCabecera.Pos_X_1) || !ImageHead.Pos_Y_0.Equals(pImagenCabecera.Pos_Y_0) || !ImageHead.Pos_Y_1.Equals(pImagenCabecera.Pos_Y_1));

                if (imagenCambiada || tamañoCambiado || coordenadasCambiadas)
                {
                    // Guardar imagen redimensionada
                    byte[] bytesFicheroTemp = pServicioImagenes.ObtenerImagen(rutaTemp, ".png");

                    SixLabors.ImageSharp.Image imagenCortada = UtilImages.CropImage(bytesFicheroTemp, pImagenCabecera.Pos_X_1, pImagenCabecera.Pos_Y_1, pImagenCabecera.Pos_X_0, pImagenCabecera.Pos_Y_0);

                    float anchura = pImagenCabecera.Ancho;
                    float altura = pImagenCabecera.Alto;

                    imagenCortada = UtilImages.AjustarImagen(imagenCortada, anchura, altura, false);

                    MemoryStream ms = new MemoryStream();
                    imagenCortada.SaveAsPng(ms);

                    //Guardo la miniatura en el servicio de imagen
                    pServicioImagenes.AgregarImagen(ms.ToArray(), ruta, ".png");

                    // Guardar coordenadas
                    FilaParametrosGenerales.CoordenadasSup = $"[ { pImagenCabecera.Pos_X_0 }, { pImagenCabecera.Pos_Y_0 }, { pImagenCabecera.Pos_X_1 }, { pImagenCabecera.Pos_Y_1 } ]";

                    if (FilaParametrosGenerales.VersionFotoImagenSupGrande == null)
                    {
                        FilaParametrosGenerales.VersionFotoImagenSupGrande = 1;
                    }
                    else
                    {
                        FilaParametrosGenerales.VersionFotoImagenSupGrande++;
                    }
                }
            }
            else
            {
                pServicioImagenes.BorrarImagen(rutaTemp + ".png");

                FilaParametrosGenerales.CoordenadasSup = null;
                FilaParametrosGenerales.VersionFotoImagenSupGrande = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pServicioImagenes"></param>
        /// <param name="pImagenLogo"></param>
        private void GuardarImagenLogo(ServicioImagenes pServicioImagenes, AdministrarComunidadGeneralModel.ImageCoordenadas pImagenLogo)
        {
            string ruta = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "_peque";
            string rutaTemp = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "_mosaico_grande_temp";
            string rutaAux = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "_mosaico_grande_temp2";

            string rutaCompletaAux = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + rutaAux + ".png";

            if (rutaCompletaAux.Equals(pImagenLogo.Ruta))
            {
                //Guardar imagen temporal nueva si ha cambiado
                byte[] bytesFichero = pServicioImagenes.ObtenerImagen(rutaAux, ".png");
                if (bytesFichero != null)
                {
                    pServicioImagenes.AgregarImagen(bytesFichero, rutaTemp, ".png");

                    pServicioImagenes.BorrarImagen(rutaAux + ".png");
                }
            }

            if (!string.IsNullOrEmpty(pImagenLogo.Ruta))
            {
                AdministrarComunidadGeneralModel.ImageCoordenadas ImageLogo = CargarImagenLogo();

                bool imagenCambiada = !ImageLogo.Ruta.Equals(pImagenLogo.Ruta);

                bool coordenadasCambiadas = (!ImageLogo.Pos_X_0.Equals(pImagenLogo.Pos_X_0) || !ImageLogo.Pos_X_1.Equals(pImagenLogo.Pos_X_1) || !ImageLogo.Pos_Y_0.Equals(pImagenLogo.Pos_Y_0) || !ImageLogo.Pos_Y_1.Equals(pImagenLogo.Pos_Y_1));

                if (imagenCambiada || coordenadasCambiadas)
                {
                    // Guardar imagen redimensionada
                    byte[] bytesFicheroTemp = pServicioImagenes.ObtenerImagen(rutaTemp, ".png");

                    SixLabors.ImageSharp.Image imagenCortada = UtilImages.CropImage(bytesFicheroTemp, pImagenLogo.Pos_X_1, pImagenLogo.Pos_Y_1, pImagenLogo.Pos_X_0, pImagenLogo.Pos_Y_0);

                    imagenCortada = UtilImages.AjustarImagen(imagenCortada, 120, 120, false);

                    MemoryStream ms = new MemoryStream();
                    imagenCortada.SaveAsPng(ms);

                    //Guardo la miniatura en el servicio de imagen
                    pServicioImagenes.AgregarImagen(ms.ToArray(), ruta, ".png");

                    // Guardar coordenadas
                    FilaParametrosGenerales.CoordenadasMosaico = $"[ { pImagenLogo.Pos_X_0 }, { pImagenLogo.Pos_Y_0 }, { pImagenLogo.Pos_X_1}, {pImagenLogo.Pos_Y_1} ]";

                    if (FilaParametrosGenerales.VersionFotoImagenMosaicoGrande == null)
                    {
                        FilaParametrosGenerales.VersionFotoImagenMosaicoGrande = 1;
                    }
                    else
                    {
                        FilaParametrosGenerales.VersionFotoImagenMosaicoGrande++;
                    }
                    FilaParametrosGenerales.NombreImagenPeque = $"{ FilaParametrosGenerales.ProyectoID.ToString() }_peque.png?v={ FilaParametrosGenerales.VersionFotoImagenMosaicoGrande }";
                }
            }
            else
            {
                pServicioImagenes.BorrarImagen(rutaTemp + ".png");

                FilaParametrosGenerales.CoordenadasMosaico = null;
                FilaParametrosGenerales.VersionFotoImagenMosaicoGrande = null;
                FilaParametrosGenerales.NombreImagenPeque = "peque";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        private string ComprobarErrores(AdministrarComunidadGeneralModel Options)
        {
            if (!mConfigService.ObtenerListaIdiomasDictionary().ContainsKey(Options.IdiomaPorDefecto))
            {
                return "El idioma seleccionado no existe";
            }
            
            Options.Desciption = HttpUtility.UrlDecode(Options.Desciption);

            if (!string.IsNullOrEmpty(Options.ImageHead.Ruta) && Options.ImageHead.Ruta.IndexOf("?") > 0)
            {
                Options.ImageHead.Ruta = Options.ImageHead.Ruta.Substring(0, Options.ImageHead.Ruta.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(Options.ImageLogo.Ruta) && Options.ImageLogo.Ruta.IndexOf("?") > 0)
            {
                Options.ImageLogo.Ruta = Options.ImageLogo.Ruta.Substring(0, Options.ImageLogo.Ruta.IndexOf("?"));
            }

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        public ActionResult SubirImagenCabecera(IFormFile fileUpload)
        {
            AdministrarComunidadGeneralModel.ImageCoordenadas ImageHead = new AdministrarComunidadGeneralModel.ImageCoordenadas();

            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService);
            servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");

            string ruta = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "_sup_grande_temp2";

            string urlFicheroImagen = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + ruta + ".png";

            servicioImagenes.BorrarImagen(ruta + ".png");

            string error = string.Empty;

            //Límite de 10 MB
            if (fileUpload.Length <= 10 * 1024 * 1024)
            {
                byte[] bytesFichero = new byte[fileUpload.Length];
                ((Stream)fileUpload.OpenReadStream()).Read(bytesFichero, 0, (int)fileUpload.Length);

                Image imagePerfilOriginal = UtilImages.ConvertirArrayBytesEnImagen(bytesFichero);

                if (imagePerfilOriginal.Height >= minAltoImg && imagePerfilOriginal.Width >= minAnchoImg && imagePerfilOriginal.Height <= maxAltoImg && imagePerfilOriginal.Width <= maxAnchoImg)
                {
                    servicioImagenes.AgregarImagen(bytesFichero, ruta, ".png");

                    ImageHead.Pos_X_0 = 0;
                    ImageHead.Pos_Y_0 = 0;
                    ImageHead.Pos_X_1 = imagePerfilOriginal.Width;
                    ImageHead.Pos_Y_1 = imagePerfilOriginal.Height;

                    ImageHead.Ruta = urlFicheroImagen;

                    ImageHead.Ancho = imagePerfilOriginal.Width;
                    ImageHead.Alto = imagePerfilOriginal.Height;
                }
                else
                {
                    error = "La dimensión mínima permitida para la imagen es de 60 x 20 px y la máxima 850 x 400 px. El tamaño máximo permitido es de 10 Mb.";
                }
            }
            else
            {
                error = UtilIdiomas.GetText("PERFIL", "ERRORTAMAÑOIMAGEN");
            }

            if (string.IsNullOrEmpty(error))
            {

                return PartialView("ImagenCabecera", ImageHead);
            }
            else
            {
                return GnossResultERROR(error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        public ActionResult SubirImagenLogo(IFormFile fileUpload)
        {
            AdministrarComunidadGeneralModel.ImageCoordenadas ImageHead = new AdministrarComunidadGeneralModel.ImageCoordenadas();

            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService);
            servicioImagenes.Url = UrlIntragnossServicios;

            string ruta = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "_mosaico_grande_temp2";

            string urlFicheroImagen = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + ruta + ".png";

            servicioImagenes.BorrarImagen(ruta + ".png");

            string error = string.Empty;

            //Límite de 10 MB
            if (fileUpload.Length <= 10 * 1024 * 1024)
            {
                byte[] bytesFichero = new byte[fileUpload.Length];
                ((Stream)fileUpload.OpenReadStream()).Read(bytesFichero, 0, (int)fileUpload.Length);

                Image imagePerfilOriginal = UtilImages.ConvertirArrayBytesEnImagen(bytesFichero);

                if (imagePerfilOriginal.Height >= minAltoImg && imagePerfilOriginal.Width >= minAnchoImg && imagePerfilOriginal.Height <= maxAltoImg && imagePerfilOriginal.Width <= maxAnchoImg)
                {
                    servicioImagenes.AgregarImagen(bytesFichero, ruta, ".png");

                    ImageHead.Pos_X_0 = 0;
                    ImageHead.Pos_Y_0 = 0;
                    ImageHead.Pos_X_1 = imagePerfilOriginal.Width;
                    ImageHead.Pos_Y_1 = imagePerfilOriginal.Height;

                    ImageHead.Ruta = urlFicheroImagen;
                }
                else
                {
                    error = "La dimensión mínima permitida para la imagen es de 60 x 20 px y la máxima 850 x 400 px. El tamaño máximo permitido es de 10 Mb.";
                }
            }
            else
            {
                error = UtilIdiomas.GetText("PERFIL", "ERRORTAMAÑOIMAGEN");
            }

            if (string.IsNullOrEmpty(error))
            {

                return PartialView("ImagenLogo", ImageHead);
            }
            else
            {
                return GnossResultERROR(error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private AdministrarComunidadGeneralModel.ImageCoordenadas CargarImagenSup()
        {
            AdministrarComunidadGeneralModel.ImageCoordenadas ImageHead = new AdministrarComunidadGeneralModel.ImageCoordenadas();
            ImageHead.Ruta = "";

            if (!string.IsNullOrEmpty(ParametrosGeneralesVirtualRow.CoordenadasSup))
            {
                string[] coordenadas = ParametrosGeneralesVirtualRow.CoordenadasSup.Trim().TrimStart('[').TrimEnd(']').Split(',');

                ImageHead.Pos_X_0 = int.Parse(coordenadas[0]);
                ImageHead.Pos_Y_0 = int.Parse(coordenadas[1]);
                ImageHead.Pos_X_1 = int.Parse(coordenadas[2]);
                ImageHead.Pos_Y_1 = int.Parse(coordenadas[3]);

                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService);
                servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");
                string ruta = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower();
                byte[] imagenBytes = servicioImagenes.ObtenerImagen(ruta, ".png");
                if (imagenBytes != null)
                {
                    Image imagen = UtilImages.ConvertirArrayBytesEnImagen(imagenBytes);

                    ImageHead.Alto = imagen.Height;
                    ImageHead.Ancho = imagen.Width;
                }
                string rutaOriginal = ruta + "_sup_grande_temp";
                byte[] imagenOriginalBytes = servicioImagenes.ObtenerImagen(rutaOriginal, ".png");
                if (imagenOriginalBytes != null)
                {
                    ImageHead.Ruta = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + rutaOriginal + ".png";
                }
            }

            return ImageHead;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private AdministrarComunidadGeneralModel.ImageCoordenadas CargarImagenLogo()
        {
            AdministrarComunidadGeneralModel.ImageCoordenadas ImageLogo = new AdministrarComunidadGeneralModel.ImageCoordenadas();
            ImageLogo.Ruta = "";

            if (!string.IsNullOrEmpty(ParametrosGeneralesVirtualRow.CoordenadasMosaico))
            {
                string[] coordenadas = ParametrosGeneralesVirtualRow.CoordenadasMosaico.Trim().TrimStart('[').TrimEnd(']').Split(',');

                if (coordenadas.Length == 4)
                {
                    ImageLogo.Pos_X_0 = int.Parse(coordenadas[0]);
                    ImageLogo.Pos_Y_0 = int.Parse(coordenadas[1]);
                    ImageLogo.Pos_X_1 = int.Parse(coordenadas[2]);
                    ImageLogo.Pos_Y_1 = int.Parse(coordenadas[3]);

                    ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService);
                    servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");

                    string rutaOriginal = UtilArchivos.ContentImagenesProyectos + "/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "_mosaico_grande_temp";
                    byte[] imagenOriginalBytes = servicioImagenes.ObtenerImagen(rutaOriginal, ".png");
                    if (imagenOriginalBytes != null)
                    {
                        ImageLogo.Ruta = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + rutaOriginal + ".png";
                    }
                }
            }

            return ImageLogo;
        }

        /// <summary>
        /// 
        /// </summary>
        private ParametroGeneral FilaParametrosGenerales
        {
            get
            {
                if (mFilaParametrosGenerales == null)
                {
                    //mFilaParametrosGenerales = ParametrosGeneralesDS.ParametroGeneral.FindByOrganizacionIDProyectoID(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
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
                    // ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService);
                    //mParametrosGeneralesDS = paramCN.ObtenerParametrosGeneralesDeProyecto(ProyectoVirtual.Clave);
                    //paramCN.Dispose();
                    ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                    mParametrosGeneralesDS = new GestorParametroGeneral();
                    mParametrosGeneralesDS = gestorController.ObtenerParametrosGeneralesDeProyecto(mParametrosGeneralesDS, ProyectoVirtual.Clave);
                }
                return mParametrosGeneralesDS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AdministrarComunidadGeneralModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarComunidadGeneralModel();

                    if (ParametrosGeneralesRow.IdiomasDisponibles)
                    {
                        mPaginaModel.ListaIdiomas = mConfigService.ObtenerListaIdiomasDictionary();
                    }
                    else
                    {
                        mPaginaModel.ListaIdiomas = new Dictionary<string, string>();
                        mPaginaModel.ListaIdiomas.Add(IdiomaPorDefecto, mConfigService.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
                    }

                    mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;
                    mPaginaModel.ListaIdiomasPlataforma = mConfigService.ObtenerListaIdiomasDictionary();

                    mPaginaModel.State = new AdministrarComunidadGeneralModel.StateCommunity();
                    mPaginaModel.State.State = ProyectoSeleccionado.Estado;
                    mPaginaModel.State.DaysOfGrace = 15;
                    mPaginaModel.State.CanBeOpened = Comunidad.Categories.Count > 0;

                    if (ProyectoSeleccionado.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente))
                    {
                        mPaginaModel.State.ReOpenDate = ProyectoSeleccionado.FilaProyecto.ProyectoCerradoTmp.FechaReapertura;
                        mPaginaModel.State.CauseOfClose = HttpUtility.UrlDecode(ProyectoSeleccionado.FilaProyecto.ProyectoCerradoTmp.Motivo);
                    }
                    else if (ProyectoSeleccionado.Estado.Equals((short)EstadoProyecto.Cerrandose))
                    {
                        ProyectoCerrandose filaProyectoCerrandose = ProyectoSeleccionado.FilaProyecto.ProyectoCerrandose;
                        TimeSpan diferencia = filaProyectoCerrandose.FechaCierre.Date.AddDays(filaProyectoCerrandose.PeriodoDeGracia).Subtract(DateTime.Now.Date);
                        mPaginaModel.State.DaysOfGrace = diferencia.Days;
                    }

                    mPaginaModel.Name = ProyectoSeleccionado.Nombre;
                    mPaginaModel.Desciption = ProyectoSeleccionado.FilaProyecto.Descripcion;
                    mPaginaModel.Tags = ProyectoSeleccionado.FilaProyecto.Tags;

                    mPaginaModel.ImageHead = CargarImagenSup();
                    mPaginaModel.ImageLogo = CargarImagenLogo();


                    mPaginaModel.EcosistemaCategories = CargarTesauro(ProyectoAD.MetaProyecto);
                    mPaginaModel.SelectedCategories = new List<Guid>();
                    foreach (ProyectoAgCatTesauro filaProyectoAgCatTesauro in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoAgCatTesauro)
                    {
                        mPaginaModel.SelectedCategories.Add(filaProyectoAgCatTesauro.CategoriaTesauroID);
                    }

                    mPaginaModel.HasMultiLanguageObjects = TieneObjetosMultiIdioma();

                    mPaginaModel.MultiLanguage = ParametrosGeneralesRow.IdiomasDisponibles || mPaginaModel.HasMultiLanguageObjects;

                }
                return mPaginaModel;
            }
        }
    }
}