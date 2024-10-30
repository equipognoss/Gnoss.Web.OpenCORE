using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Filters;
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

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class AdministrarComunidadCategoriasMetaproyectoController : ControllerBaseWeb
    {
        private AdministrarComunidadGeneralModel mPaginaModel;
        private GestionTesauro mGestorTesauroMetaproyecto;

        public AdministrarComunidadCategoriasMetaproyectoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

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
            return View("~/Views/Administracion/AdministrarComunidadGeneral/AdministrarComunidadCategoriasMetaproyecto.cshtml", PaginaModel);
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
            GuardarLogAuditoria();
            string error = ComprobarErrores(Options);

            if (!string.IsNullOrEmpty(error))
            {
                return GnossResultERROR(error);
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionProyecto gestProy = new GestionProyecto(proyCN.ObtenerProyectoPorID(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
            Elementos.ServiciosGenerales.Proyecto Proy = gestProy.ListaProyectos[ProyectoSeleccionado.Clave];

            GuardarCategorizacion(Proy, Options.SelectedCategories);

            if (!string.IsNullOrEmpty(RequestParams("new-community-wizard")) && Proy.Estado.Equals((short)EstadoProyecto.Definicion))
            {
                Proy.Estado = (short)EstadoProyecto.Abierto;
            }
            
            try
            {
                proyCN.GuardarProyectos();

                if (Proy.Estado.Equals((short)EstadoProyecto.Abierto))
                {
                    ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    controladorProyecto.ActualizarModeloBase(Proy.Clave, PrioridadBase.Alta);
                }
                
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarFilaProyecto(Proy.Clave);
            }
            catch (Exception)
            {
                GnossResultERROR();
            }

            string redirect = ObtenerUrlSiguientePasoAsistenteNuevaComunidad();

            if (string.IsNullOrEmpty(redirect))
            {
                return GnossResultOK();
            }
            else
            {
                return GnossResultUrl(redirect);
                //return Redirect(redirect);
            }
        }

        private string ComprobarErrores(AdministrarComunidadGeneralModel options)
        {
            foreach (var propiedad in GestorTesauroMetaproyecto.FilasPropiedadesPorCategoria.Values.Where(prop => prop.Obligatoria.Equals(true)))
            {
                if (!EstaCategoriaObligatoriaSeleccionada(propiedad.CategoriaTesauroID, options))
                {
                    return $"Debe seleccionar alguna categoría de \"{GestorTesauroMetaproyecto.ListaCategoriasTesauro[propiedad.CategoriaTesauroID].Nombre[UtilIdiomas.LanguageCode]}\"";
                }
            }

            return null;
        }

        private bool EstaCategoriaObligatoriaSeleccionada(Guid pCategoriaTesauroID, AdministrarComunidadGeneralModel options)
        {
            foreach (Guid categoriaSeleccionada in options.SelectedCategories)
            {
                if (GestorTesauroMetaproyecto.ListaCategoriasTesauro[pCategoriaTesauroID].SubCategorias.Exists(cat => cat.Clave.Equals(categoriaSeleccionada)))
                {
                    return true;
                }
            }

            // Ninguna categoría de primer nivel estaba seleciconada, busco en las hijas
            foreach (CategoriaTesauro categoriaHija in GestorTesauroMetaproyecto.ListaCategoriasTesauro[pCategoriaTesauroID].SubCategorias.Where(cat => cat.SubCategorias.Count > 0))
            {
                if (EstaCategoriaObligatoriaSeleccionada(categoriaHija.Clave, options))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyecto"></param>
        /// <param name="pCategoriasSeleccionadas"></param>
        private void GuardarCategorizacion(Elementos.ServiciosGenerales.Proyecto pProyecto, List<Guid> pCategoriasSeleccionadas)
        {
            pProyecto.GestorProyectos.GestionTesauro = GestorTesauroMetaproyecto;
            Guid tesauroID = (Guid)pProyecto.GestorProyectos.GestionTesauro.TesauroDW.ListaTesauroProyecto.Where(item => item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).FirstOrDefault().TesauroID;
            List<Guid> listaCatsAnteriores = new List<Guid>();

            List<ProyectoAgCatTesauro> listaProyectoAgCatTesauro = pProyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoAgCatTesauro.ToList();
            foreach (ProyectoAgCatTesauro filaProyectoAgCatTesauro in listaProyectoAgCatTesauro)
            {
                listaCatsAnteriores.Add(filaProyectoAgCatTesauro.CategoriaTesauroID);
                if (!pCategoriasSeleccionadas.Contains(filaProyectoAgCatTesauro.CategoriaTesauroID))
                {
                    pProyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoAgCatTesauro.Remove(filaProyectoAgCatTesauro);
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

                        if (! (mEntityContext.ProyectoAgCatTesauro.Any(proy=>proy.OrganizacionID.Equals(proyectoAgCatTesauro.OrganizacionID) && proy.ProyectoID.Equals(proyectoAgCatTesauro.ProyectoID) && proy.TesauroID.Equals(proyectoAgCatTesauro.TesauroID) && proy.CategoriaTesauroID.Equals(proyectoAgCatTesauro.CategoriaTesauroID))))
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
        public AdministrarComunidadGeneralModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarComunidadGeneralModel();

                    mPaginaModel.Name = ProyectoSeleccionado.Nombre;
                    mPaginaModel.Desciption = ProyectoSeleccionado.FilaProyecto.Descripcion;
                    mPaginaModel.Tags = ProyectoSeleccionado.FilaProyecto.Tags;

                    mPaginaModel.EcosistemaCategories = CargarTesauro(ProyectoAD.MetaProyecto);
                    mPaginaModel.SelectedCategories = new List<Guid>();
                    foreach (ProyectoAgCatTesauro filaProyectoAgCatTesauro in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoAgCatTesauro)
                    {
                        mPaginaModel.SelectedCategories.Add(filaProyectoAgCatTesauro.CategoriaTesauroID);
                    }
                }
                return mPaginaModel;
            }
        }

        public GestionTesauro GestorTesauroMetaproyecto
        {
            get
            {
                if (mGestorTesauroMetaproyecto == null)
                {
                    TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperTesauro tesauroDW = tesauroCN.ObtenerTesauroDeProyecto(ProyectoAD.MetaProyecto);
                    tesauroCN.Dispose();

                    mGestorTesauroMetaproyecto = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);
                }
                return mGestorTesauroMetaproyecto;
            }
        }
    }
}