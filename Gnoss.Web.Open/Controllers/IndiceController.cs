using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Facetas;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
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
using System.Data;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class IndiceController : ControllerPestanyaBase
    {

        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public IndiceController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<IndiceController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;

        }

        [TypeFilter(typeof(AccesoPestanyaAttribute))]
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(RequestParams("callback")))
            {
                string action = RequestParams("callback");
                if (action.ToLower() == "Categorias|SolicitarCategoria".ToLower() && !mControladorBase.UsuarioActual.EsIdentidadInvitada)
                {
                    try
                    {
                        string nombreCategoria = RequestParams("nombreCategoria");

                        if (!string.IsNullOrEmpty(nombreCategoria))
                        {
                            Guid categoriaSupID = new Guid(RequestParams("categoriaSupID"));

                            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
                            GestionTesauro gestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                            gestorTesauro.AgregarSugerenciaCategoria(categoriaSupID, gestorTesauro.TesauroActualID, nombreCategoria, IdentidadActual.Clave);

                            tesauroCN.ActualizarTesauro();
                            tesauroCN.Dispose();

                            //Envio un correo al administrador avisandole de la solicitud:
                            GestionNotificaciones GestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);

                            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                            List<string> listaCorreos = personaCN.ObtenerCorreoDeAdministradoresDeProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                            personaCN.Dispose();

                            if (listaCorreos.Count > 0)
                            {
                                GestorNotificaciones.AgregarNotificacionCategoriaSugeridaEnComunidad(listaCorreos, ProyectoSeleccionado.Nombre, mControladorBase.UrlsSemanticas.GetURLAdministrarCategoriasComunidad(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, true), UtilIdiomas.LanguageCode);
                            }

                            if (listaCorreos.Count > 0)
                            {
                                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                                notificacionCN.ActualizarNotificacion(mAvailableServices);
                                notificacionCN.Dispose();
                            }

                            GestorNotificaciones.Dispose();

                            return Content("OK");
                        }
                    }
                    catch{ 
                    }

                    return Content("KO");
                } 
                return Content("KO");
            }

            IndexViewModel paginaModel = new IndexViewModel();
            
            string url = BaseURL + "/" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                url = urlComunidad;
            }

            paginaModel.UrlBaseCategories = url + "/" + UtilIdiomas.GetText("URLSEM", "CATEGORIA") + "/";

            Comunidad.Categories = CargarTesauroProyecto();

            List<CategoryModel> categoriasTesauro = Comunidad.Categories;

            Dictionary<string, List<string>> listaFiltros = new Dictionary<string, List<string>>();
            TipoBusqueda tipoBusqueda = TipoBusqueda.BusquedaAvanzada;
            DataWrapperFacetas tConfiguracionOntologia = null;

            List<string> mListaItemsBusquedaExtra = ControladorFacetas.ObtenerListaItemsBusquedaExtra(listaFiltros, tipoBusqueda, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
            Dictionary<string, string> mInformacionOntologias = ControladorFacetas.ObtenerInformacionOntologias(tipoBusqueda, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, ref tConfiguracionOntologia);
            List<string> mFormulariosSemanticos = ControladorFacetas.ObtenerFormulariosSemanticos(tipoBusqueda, ProyectoSeleccionado.Clave);

            FacetadoDS facetadoDS = new FacetadoDS();
            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, true, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            facetadoCN.ListaItemsBusquedaExtra = mListaItemsBusquedaExtra;
            facetadoCN.ObtenerFaceta(ProyectoSeleccionado.Clave.ToString(), facetadoDS, "skos:ConceptID", listaFiltros, mListaItemsBusquedaExtra, false, !mControladorBase.UsuarioActual.EsIdentidadInvitada, mControladorBase.UsuarioActual.EsUsuarioInvitado, IdentidadActual.Clave.ToString(), TipoDisenio.ListaOrdCantidad, 0, -1, mFormulariosSemanticos, null, ProyectoSeleccionado.TipoProyecto, false, null, true, false, false, ParametrosGeneralesRow.PermitirRecursosPrivados, true, 0, TipoPropiedadFaceta.NULL,null,false);
            facetadoCN.Dispose();
            
            Dictionary<Guid, int> NumElementosCategoria = new Dictionary<Guid, int>();

            foreach (DataRow fila in facetadoDS.Tables["skos:ConceptID"].Rows)
            {
                Guid categoriaID = new Guid(((string)fila[0]).Substring(((string)fila[0]).LastIndexOf("/") + 1));
                int numRecursos = int.Parse((string)fila[1]);

                if (NumElementosCategoria.ContainsKey(categoriaID))
                {
                    NumElementosCategoria[categoriaID] = NumElementosCategoria[categoriaID] + numRecursos;
                }
                else
                {
                    NumElementosCategoria.Add(categoriaID, numRecursos);
                }
            }

            facetadoDS.Dispose();

            foreach (CategoryModel cat in categoriasTesauro)
            {
                if (NumElementosCategoria.ContainsKey(cat.Key))
                {
                    cat.NumResources = NumElementosCategoria[cat.Key];
                }
                else
                { 
                    cat.NumResources = 0; 
                }
            }

            paginaModel.Categories = categoriasTesauro;

            return View(paginaModel);
        }
    }
}
