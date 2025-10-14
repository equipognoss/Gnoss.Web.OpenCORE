using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.AbstractsOpen;
using Es.Riam.InterfacesOpen;
using Gnoss.Web.Open.Filters;
using Microsoft.Extensions.Hosting;
using static Es.Riam.Gnoss.Web.MVC.Models.ViewModels.MyCommunitiesViewModel;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.Logica.MVC;
using Es.Riam.Gnoss.Recursos;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class MisComunidadesController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        public MisComunidadesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<MisComunidadesController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Miembros

        private MyCommunitiesViewModel mPaginaModel;

        #endregion

        #region Propiedades

        private MyCommunitiesViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = CargarModelo();
                }
                return mPaginaModel;
            }
        }

        #endregion
        
        #region Metdos Web
        [HttpGet]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            return View(PaginaModel);
        }
        #endregion

        #region Metodos privados

        private MyCommunitiesViewModel CargarModelo()
        {
            MyCommunitiesViewModel modelo = new MyCommunitiesViewModel();

            // Obtenemos los proyectos de los que es miembro el usuario
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto dataWrapperProyecto = proyectoCN.ObtenerProyectosParticipaPerfilUsuario(IdentidadActual.PerfilUsuario.Clave, true, UsuarioActual.UsuarioID);
            proyectoCN.Dispose();

            //Obtenemos todos los proyectos hijos a los que pertenece el usuario.
            List<Guid> hijos = new List<Guid>();
            foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in dataWrapperProyecto.ListaProyecto.Where(proyecto => proyecto.ProyectoSuperiorID.HasValue).OrderBy(proy => proy.Nombre))
            {
                hijos.Add(filaProy.ProyectoID);
            }

            //Obtenemos todos los proyectos padre de los proyectos hijos
            proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto padresNoHabilitados = new DataWrapperProyecto();
            if (hijos.Count != 0)
            {
                padresNoHabilitados = proyectoCN.ObtenerProyectosPadresDeProyectos(hijos);
            }
            proyectoCN.Dispose();

            //Mezclamos las listas, obteniendo todos los proyectos afectados (tanto a los que pertenece como a los que no)
            DataWrapperProyecto todos = new DataWrapperProyecto();
            todos.Merge(dataWrapperProyecto);
            todos.Merge(padresNoHabilitados);

            //Creamos un gestor de proyectos con todos los proyectos
            GestionProyecto gestProy = new GestionProyecto(todos, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);

            if (gestProy.ListaProyectos.Count > 0)
            {
                modelo.Communities = new List<MyCommunityModel>();
                foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in gestProy.DataWrapperProyectos.ListaProyecto.Where(item => !item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).OrderBy(item => item.Nombre))
                {
                    MyCommunityModel comunidad = new MyCommunityModel();
                    comunidad.Key = filaProy.ProyectoID;
                    comunidad.ParentKey = Guid.Empty;
                    if (filaProy.ProyectoSuperiorID.HasValue)
                    {
                        comunidad.ParentKey = filaProy.ProyectoSuperiorID.Value;
                    }

                    comunidad.Name = CadenaMultiIdioma(filaProy.Nombre);
                    comunidad.ProyectType = (CommunityModel.TypeProyect)filaProy.TipoProyecto;
                    comunidad.CreationDate = (DateTime)filaProy.FechaInicio;
                    comunidad.Description = CadenaMultiIdioma(filaProy.Descripcion);

                    var filaParametroGeneral = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyecto>(), mLoggerFactory).ObtenerFilaParametrosGeneralesDeProyecto(filaProy.ProyectoID);

                    string nombreImagenSmall = filaParametroGeneral?.NombreImagenPeque;
                    string urlFoto = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesProyectos}/{nombreImagenSmall}";
                    comunidad.Logo = urlFoto;

                    if (string.IsNullOrEmpty(nombreImagenSmall) || nombreImagenSmall.Equals("peque"))
                    {
                        urlFoto = $"{BaseURLStatic}/img/{UtilArchivos.ContentImgIconos}/{UtilArchivos.ContentImagenesProyectos}/anonimo_peque.png";
                        comunidad.Logo = mControladorBase.CargarImagenSup(filaProy.ProyectoID);

                        if (string.IsNullOrEmpty(comunidad.Logo))
                        {
                            comunidad.Logo = urlFoto;
                        }
                    }

                    comunidad.Url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, filaProy.NombreCorto);
                    comunidad.AccessType = (CommunityModel.TypeAccessProject)filaProy.TipoAcceso;
                    comunidad.Persons = CargarNumeroPersonas(filaProy.OrganizacionID, filaProy.ProyectoID);
                    comunidad.Organizations = CargarNumeroOrganizaciones(filaProy.ProyectoID);
                    comunidad.Resources = CargarNumeroRecursos(filaProy.ProyectoID);

                    string tags = "";
                    if (filaProy.Tags != null)
                    {
                        tags = filaProy.Tags;
                    }
                    comunidad.Tags = new List<string>(tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    modelo.Communities.Add(comunidad);
                }
            }

            CargarCategoriasComunidad(modelo.Communities.Select(item => item.Key).ToList(), modelo);
            CargarListaFiltrosOrden(modelo);
            CargarListaVistas(modelo);

            return modelo;
        }

        private void CargarListaFiltrosOrden(MyCommunitiesViewModel pModel)
        {
            Dictionary<string, string> listaFiltrosOrden = new Dictionary<string, string>();

            listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "NOMBRE"), "foaf:firstName");
            listaFiltrosOrden.Add(UtilIdiomas.GetText("OBJETOGNOSS", "NUMRECURSOS"), "gnoss:hasnumerorecursos");
            listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILBASERECURSOEDITAR", "PARTICIPANTES"), "gnoss:hasparticipanteIdentidadID");
            listaFiltrosOrden.Add(UtilIdiomas.GetText("COMMON", "FECHACREACION"), "gnoss:hasfechaAlta");

            pModel.FilterOrderList = listaFiltrosOrden;
            pModel.FilterOrderSelected = listaFiltrosOrden.ElementAt(0).Key;
            pModel.Communities = pModel.Communities.OrderBy(item => item.Name).ToList();
        }

        private void CargarListaVistas(MyCommunitiesViewModel pModel)
        {
            pModel.ViewList = new List<ViewTypeModel>();

            ViewTypeModel vistaMosaico = new ViewTypeModel();
            vistaMosaico.Active = true;
            vistaMosaico.ViewType = ViewTypeModel.ViewTypeSearch.Grid;
            pModel.ViewList.Add(vistaMosaico);

            ViewTypeModel vistaListado = new ViewTypeModel();
            vistaListado.ViewType = ViewTypeModel.ViewTypeSearch.List;
            vistaListado.Active = false;
            pModel.ViewList.Add(vistaListado);
        }

        private string CadenaMultiIdioma(string pCadena)
        {
            string resultado = pCadena;
            if (resultado.Contains("|||"))
            {
                List<string> cadenasIdioma = resultado.Split(new string[] { "|||" }, StringSplitOptions.None).ToList();
                Dictionary<string, string> cadenasIdiomaDictionary = new Dictionary<string, string>();
                foreach (string cadena in cadenasIdioma)
                {
                    if (cadena.Contains("@"))
                    {
                        string[] idiomas = cadena.Split('@');
                        cadenasIdiomaDictionary.Add(idiomas[1], idiomas[0]);
                    }
                }

                if (cadenasIdiomaDictionary.ContainsKey(UtilIdiomas.LanguageCode))
                {
                    resultado = cadenasIdiomaDictionary[UtilIdiomas.LanguageCode];
                }
                else
                {
                    resultado = cadenasIdiomaDictionary.Values.First();
                }
            }
            return resultado;
        }

        private int CargarNumeroPersonas(Guid pOrganizacionID, Guid pProyectoID)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            int resultado = personaCN.ObtenerPersonasDeProyecto(pOrganizacionID, pProyectoID).Count();
            personaCN.Dispose();
            return resultado;
        }

        private int CargarNumeroOrganizaciones(Guid pProyectoID)
        {
            OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
            int resultado = organizacionCN.ObtenerOrganizacionesParticipanEnProyecto(pProyectoID).ListaOrganizacion.Count();
            organizacionCN.Dispose();
            return resultado;
        }

        private int CargarNumeroRecursos(Guid pProyectoID)
        {
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            int resultado = documentacionCN.ObtenerDocumentosIDDeBaseRecursosProyecto(pProyectoID).Count();
            documentacionCN.Dispose();
            return resultado;
        }

        private void CargarCategoriasComunidad(List<Guid> pListaComunidadesID, MyCommunitiesViewModel pModel)
        {
            MVCCN controladorMVCCN = new MVCCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<MVCCN>(), mLoggerFactory);
            List<ObtenerTesauroProyectoMVC> listaTesauroProyectoMVC = controladorMVCCN.ObtenerCategoriasComunidadesPorID(pListaComunidadesID);
            if (listaTesauroProyectoMVC != null)
            {
                foreach (ObtenerTesauroProyectoMVC tesauroProyectoMVC in listaTesauroProyectoMVC)
                {
                    Guid claveProyecto = tesauroProyectoMVC.ProyectoID;
                    Guid claveCategoria = tesauroProyectoMVC.CategoriaTesauroID;
                    string NombreCategoria = tesauroProyectoMVC.Nombre;

                    MyCommunityModel comunidad = pModel.Communities.Where(item => item.Key == claveProyecto).First();
                    if (comunidad.Categories == null)
                    {
                        comunidad.Categories = new List<CategoryModel>();
                    }
                    CategoryModel categoria = new CategoryModel();
                    categoria.Key = claveCategoria;
                    categoria.Name = NombreCategoria;
                    categoria.LanguageName = NombreCategoria;
                    categoria.Lang = UtilIdiomas.LanguageCode;
                    comunidad.Categories.Add(categoria);
                }
            }
        }

        protected override void CargarTituloPagina()
        {
            TituloPagina = UtilIdiomas.GetText("MISCOMUNIDADES", "TITULOPAGINA");

            base.CargarTituloPagina();
        }
        #endregion
    }
}
