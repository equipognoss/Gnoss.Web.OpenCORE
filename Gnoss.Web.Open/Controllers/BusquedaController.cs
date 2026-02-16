using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.MetaBuscadorAD;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Comentario;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Comentario;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.MetaBuscador;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Logica.Voto;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Exportaciones;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Gnoss.Web.RSS.RSS20;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using static Es.Riam.Gnoss.Web.Controles.ControladorBase;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class BusquedaController : ControllerPestanyaBase
    {
        #region Miembros

        private Identidad mIdentidadPaginaContribuciones;

        /// <summary>
        /// Indica si la indentidad que se está buscando existe
        /// </summary>
        private bool? mExisteIdentidadPaginaContribuciones;

        private ProyectoPestanyaBusqueda mFilaPestanyaBusqueda;

        private bool mEsVistaMapa = false;

        private int mContRedirecciones = 0;

        private DataWrapperProyecto mProyectoDW = null;
        /// <summary>
        /// Filtro de orden configurados.
        /// </summary>
        private Dictionary<string, string> mFiltrosOrdenConfig;

        protected List<string> mListaItems;
        /// <summary>
        /// Lista de filtros de búsqueda
        /// </summary>
        protected Dictionary<string, List<string>> mListaFiltrosFacetas = new Dictionary<string, List<string>>();

        public const string SEPARADOR_MULTIVALOR = "@|@";

        private UtilWeb mUtilWeb;
        private IRDFSearch mRDFSearch;

        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        public BusquedaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IRDFSearch rDFSearch, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<BusquedaController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mUtilWeb = new UtilWeb(httpContextAccessor);
            mRDFSearch = rDFSearch;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Metodos

        private ActionResult CargarRSS(ResultadoModel pListaResultados)
        {
            Uri URLRss = new Uri(Request.Path.ToString());

            //Creamos el Canal con: URL, descripcion, título y imagen.
            string tituloCanalRss = $"{ProyectoSeleccionado.Nombre} - {NombreProyectoEcosistema} - {UtilIdiomas.GetText("COMBASE", "TITULOPAGINA")}";

            Regex regex = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
            string descripcionRSS = regex.Replace(UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto), string.Empty);
            RSSCanal miCanal = new RSSCanal(tituloCanalRss, URLRss, descripcionRSS);
            miCanal.Image = new RSSCanalImage(new Uri($"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesProyectos}/{ProyectoSeleccionado.Clave.ToString().ToLower()}.png"), tituloCanalRss, URLRss);

            //Creamos los items que representaran recursos de la Comunidad con: Título,URL,descripcion,fecha de publicación,link a comentarios, Guid, categorías
            foreach (ObjetoBuscadorModel resultado in pListaResultados.ListaResultados)
            {
                if (resultado is ResourceModel)
                {
                    ResourceModel resultadoRecurso = (ResourceModel)resultado;

                    RSSItem item = new RSSItem();
                    item.Title = resultadoRecurso.Title;
                    item.Link = new Uri(resultadoRecurso.CompletCardLink);
                    item.Description = $"{RSSUtil.crearHtmlImagenDocumento(resultadoRecurso.Title, resultadoRecurso.UrlPreview)}{resultadoRecurso.Description}{RSSUtil.crearHtmlEtiquetasDocumento(resultadoRecurso.Tags, resultadoRecurso.UrlSearch)}";
                    item.PubDate = resultadoRecurso.PublishDate;
                    item.Comments = new Uri($"{item.Link.ToString()}#comments");
                    item.Guid = new RSSItemGuid(resultadoRecurso.Key.ToString());

                    foreach (CategoryModel catTes in resultadoRecurso.Categories)
                    {
                        RSSCategory categoria = new RSSCategory(catTes.Name);
                        item.Category.Add(categoria);
                    }

                    if (resultadoRecurso.Authors != null && resultadoRecurso.Authors.Count > 0)
                    {
                        int cont = 0;
                        item.Author = "";
                        foreach (string autor in resultadoRecurso.Authors.Keys)
                        {
                            if (cont > 0)
                            {
                                item.Author += ", ";
                            }
                            item.Author += autor;
                        }
                    }
                    miCanal.Items.Add(item);
                }
            }

            RSSWriter writer = new RSSWriter(miCanal);
            string textoRSS = writer.pintarRSS();
            return Content(textoRSS, "text/xml");
        }

        
        /// <summary>
        /// Carga los enlaces multiidiomas de la pestaña actual
        /// </summary>
        protected override void CargarEnlacesMultiIdioma()
        {
            base.CargarEnlacesMultiIdioma();

            string[] filtros = { };
            if (!string.IsNullOrEmpty(RequestParams("searchInfo")))
            {
                filtros = RequestParams("searchInfo").Split('/');
            }
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
            List<string> listaIdiomas = paramCL.ObtenerListaIdiomas();
            foreach (string idioma in listaIdiomas)
            {
                if (!ParametrosGeneralesRow.IdiomasDisponibles && idioma != IdiomaPorDefecto)
                {
                    continue;
                }
                string enlace = ListaEnlacesMultiIdioma[idioma].Value;
                Recursos.UtilIdiomas utilIdiomasActual = new Recursos.UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<Recursos.UtilIdiomas>(), mLoggerFactory);
                if (filtros != null && filtros.Length > 1 && filtros[0].ToLower() == UtilIdiomas.GetText("URLSEM", "TAG").ToLower())
                {
                    string filtroTag = filtros[1].ToLower();
                    enlace += $"/{utilIdiomasActual.GetText("URLSEM", "TAG")}/{filtroTag}";
                }
                else if (filtros != null && ((filtros.Length > 2 && filtros[0] == UtilIdiomas.GetText("URLSEM", "CATEGORIA")) || RequestParams("categoria") != null))
                {
                    string categoriaIDString = string.Empty;

                    if (filtros.Length > 2 && filtros[0] == UtilIdiomas.GetText("URLSEM", "CATEGORIA"))
                    {
                        categoriaIDString = filtros[2].ToLower();
                    }
                    else
                    {
                        categoriaIDString = RequestParams("categoria").ToLower();
                    }

                    Guid categoriaID;
                    if (Guid.TryParse(categoriaIDString, out categoriaID))
                    {
                        string nombreCat = "";

                        if (GestorTesauroProyectoActual.ListaCategoriasTesauro[categoriaID].Nombre.ContainsKey(idioma))
                        {
                            nombreCat = GestorTesauroProyectoActual.ListaCategoriasTesauro[categoriaID].Nombre[idioma];
                        }
                        else if (GestorTesauroProyectoActual.ListaCategoriasTesauro[categoriaID].Nombre.Count > 0)
                        {
                            nombreCat = GestorTesauroProyectoActual.ListaCategoriasTesauro[categoriaID].Nombre.First().Value;
                        }
                        else
                        {
                            nombreCat = GestorTesauroProyectoActual.ListaCategoriasTesauro[categoriaID].FilaCategoria.Nombre;
                        }

                        nombreCat = UtilCadenas.EliminarCaracteresUrlSem(nombreCat);
                        enlace = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(utilIdiomasActual, BaseURLIdioma, ProyectoSeleccionado.NombreCorto)}/{utilIdiomasActual.GetText("URLSEM", "CATEGORIA")}/{nombreCat}/{categoriaIDString}";
                    }
                }

                string parametros = string.Empty;
                if (new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}").OriginalString.Contains("?"))
                {
                    parametros = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}").OriginalString.Substring(new Uri(this.Request.Path).OriginalString.IndexOf("?"));
                }

                enlace += parametros;

                ListaEnlacesMultiIdioma[idioma] = new KeyValuePair<bool, string>(ListaEnlacesMultiIdioma[idioma].Key, enlace);
            }
        }

        private GestionOrganizaciones mGestionOrganizaciones;
        private GestionPersonas mGestionPersonas;
        private GestionComentarios mGestionComentarios;
        private GestorDocumental mGestionDocumentacion;
        private GestionIdentidades mGestorIdentidades;
        private GestionProyecto mGestorProyecto;

        /// <summary>
        /// Obtiene la busqueda según los filtros
        /// </summary>
        private List<ElementoGnoss> Buscar(ResultadoModel pListaResultados)
        {
            List<Guid> listaMisIdentidades = new List<Guid>();
            listaMisIdentidades.Add(IdentidadActual.Clave);
            bool esComunidadPrivadaAccRestrOMyGnoss = (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado);

            bool puedeBuscar = true;

            if ((mControladorBase.UsuarioActual.EsUsuarioInvitado) && (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto)) && (!ProyectoPrincipalUnico.Equals(ProyectoAD.MetaProyecto)))
            {
                puedeBuscar = (!ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_ORGANIZACION) && !ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_PERSONA));
            }
            MetaBuscadorCN metaBuscadorCN = new MetaBuscadorCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<MetaBuscadorCN>(), mLoggerFactory);

            if (puedeBuscar)
            {
                Dictionary<string, TiposResultadosMetaBuscador> dicTemp = new Dictionary<string, TiposResultadosMetaBuscador>();
                foreach (ObjetoBuscadorModel resultado in pListaResultados.ListaResultados)
                {
                    if (resultado is ResourceModel)
                    {
                        dicTemp.Add(((ResourceModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Documento);
                    }
                    else if (resultado is ProfileModel)
                    {
                        ProfileModel profile = (ProfileModel)resultado;
                        if (profile.KeyPerson.Equals(Guid.Empty))
                        {
                            dicTemp.Add(((ProfileModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Organizacion);
                        }
                        else
                        {
                            dicTemp.Add(((ProfileModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Persona);
                        }
                    }
                    else if (resultado is BlogModel)
                    {
                        dicTemp.Add(((BlogModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Blog);
                    }
                    else if (resultado is CommunityModel)
                    {
                        dicTemp.Add(((CommunityModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Comunidad);
                    }
                }

                ////Método para cambiar la lista de elementos de Guid a String por cambios de Javi (Alberto)
                metaBuscadorCN.ListaOrdenadaElementos = dicTemp;
                metaBuscadorCN.NumResultados = pListaResultados.NumeroResultadosTotal;

                metaBuscadorCN.BuscarContenidos(mControladorBase.UsuarioActual.ProyectoID, IdentidadActual.PerfilID, mControladorBase.UsuarioActual.EsIdentidadInvitada);
            }

            TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);

            if (IdentidadActual.PerfilUsuario.OrganizacionID != null && metaBuscadorCN.OrganizacionDW != null && metaBuscadorCN.OrganizacionDW.ListaOrganizacion.Any(item => item.OrganizacionID.Equals(IdentidadActual.PerfilUsuario.OrganizacionID.Value)))
            {
                //Obtengo la organización del usuario actual para que pinte a todas las personas que son de mi organización:
                OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                DataWrapperOrganizacion dataWrapperOrganizacionAux = new DataWrapperOrganizacion();
                dataWrapperOrganizacionAux.ListaOrganizacion.Add(orgCN.ObtenerNombreOrganizacionPorID(IdentidadActual.PerfilUsuario.OrganizacionID.Value));
                metaBuscadorCN.OrganizacionDW.Merge(dataWrapperOrganizacionAux);
                orgCN.Dispose();
            }
            mGestionOrganizaciones = new GestionOrganizaciones(metaBuscadorCN.OrganizacionDW, mLoggingService, mEntityContext);
            mGestionPersonas = new GestionPersonas(metaBuscadorCN.DataWrapperPersona, mLoggingService, mEntityContext);
            if (metaBuscadorCN.ComentarioDW != null)
            {
                mGestionComentarios = new GestionComentarios(metaBuscadorCN.ComentarioDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionComentarios>(), mLoggerFactory);
            }
            if (metaBuscadorCN.DocumentacionDW != null)
            {
                mGestionDocumentacion = new GestorDocumental(metaBuscadorCN.DocumentacionDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

                mGestionDocumentacion.GestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(mControladorBase.UsuarioActual.ProyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            }

            if (metaBuscadorCN.DataWrapperIdentidad != null)
            {
                mGestionPersonas = new GestionPersonas(metaBuscadorCN.DataWrapperPersona, mLoggingService, mEntityContext);
                mGestionOrganizaciones = new GestionOrganizaciones(metaBuscadorCN.OrganizacionDW, mLoggingService, mEntityContext);

                if (metaBuscadorCN.DataWrapperPersona == null)
                {
                    mGestionPersonas = new GestionPersonas(new DataWrapperPersona(), mLoggingService, mEntityContext);
                }

                if (metaBuscadorCN.OrganizacionDW == null)
                {
                    mGestionOrganizaciones = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);
                }
                mGestorIdentidades = new GestionIdentidades(metaBuscadorCN.DataWrapperIdentidad, mGestionPersonas, mGestionOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            if (metaBuscadorCN.DataWrapperProyecto != null)
            {
                mGestorProyecto = new GestionProyecto(metaBuscadorCN.DataWrapperProyecto, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                mGestorProyecto.GestionTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyectoMyGnoss(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                List<Guid> listaProy = new List<Guid>();
                foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in metaBuscadorCN.DataWrapperProyecto.ListaProyecto)
                {
                    if (!listaProy.Contains(filaProy.ProyectoID))
                    {
                        listaProy.Add(filaProy.ProyectoID);
                    }
                }
            }

            List<ElementoGnoss> mListadoGenerico = new List<ElementoGnoss>();

            foreach (string ID in metaBuscadorCN.ListaOrdenadaElementos.Keys)
            {
                ElementoGnoss elemento;

                switch (metaBuscadorCN.ListaOrdenadaElementos[ID])
                {
                    case TiposResultadosMetaBuscador.IdentidadPersona:
                        if (mGestorIdentidades.ListaIdentidades.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestorIdentidades.ListaIdentidades[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                    case TiposResultadosMetaBuscador.IdentidadOrganizacion:
                        if (mGestorIdentidades.ListaIdentidades.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestorIdentidades.ListaIdentidades[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                    case TiposResultadosMetaBuscador.Documento:
                    case TiposResultadosMetaBuscador.Pregunta:
                    case TiposResultadosMetaBuscador.Debate:
                    case TiposResultadosMetaBuscador.Encuesta:
                        if (mGestionDocumentacion.ListaDocumentos.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestionDocumentacion.ListaDocumentos[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                    case TiposResultadosMetaBuscador.Comentario:
                        if (mGestionComentarios.ListaComentarios.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestionComentarios.ListaComentarios[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                    case TiposResultadosMetaBuscador.Comunidad:
                        if (mGestorProyecto.ListaProyectos.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestorProyecto.ListaProyectos[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                }
            }

            if (mGestionDocumentacion != null)
            {
                List<Guid> listaIdentidadesURLSem = new List<Guid>();

                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR in mGestionDocumentacion.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos)
                {
                    if (filaDocVinBR.IdentidadPublicacionID.HasValue && !listaIdentidadesURLSem.Contains(filaDocVinBR.IdentidadPublicacionID.Value))
                    {
                        listaIdentidadesURLSem.Add(filaDocVinBR.IdentidadPublicacionID.Value);
                    }
                }
                GestionPersonas gestorPersonas = new GestionPersonas(new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory).ObtenerPersonasPorIdentidadesCargaLigera(listaIdentidadesURLSem), mLoggingService, mEntityContext);
                GestionOrganizaciones gestorOrg = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                mGestionDocumentacion.GestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesPorID(listaIdentidadesURLSem, false), gestorPersonas, gestorOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();
            }

            metaBuscadorCN.Dispose();

            return mListadoGenerico;
        }

        [TypeFilter(typeof(AccesoPestanyaAttribute))]
        public ActionResult Index()
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true" && !EsIdentidadActualAdministradorOrganizacion)
            {
                return RedireccionarAPaginaNoEncontrada();
            }

            SearchViewModel paginaModel = new SearchViewModel();
            Stopwatch sw = null;

            ObtenerDatosPaginaBusqueda(paginaModel);

            CargarListaFiltrosOrden(paginaModel);
            CargarListaVistas(paginaModel);           
            if (mFilaPestanyaBusqueda != null)
            {
                paginaModel.SearchPersonalizadoSelected = mFilaPestanyaBusqueda.SearchPersonalizado;
                paginaModel.FacetedVisible = mFilaPestanyaBusqueda.MostrarFacetas;
                paginaModel.SearchBoxVisible = mFilaPestanyaBusqueda.MostrarCajaBusqueda;
                paginaModel.TextSearchBox = UtilCadenas.ObtenerTextoDeIdioma(mFilaPestanyaBusqueda.TextoDefectoBuscador, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }
            else
            {
                paginaModel.FacetedVisible = true;
                paginaModel.SearchBoxVisible = true;
                paginaModel.TextSearchBox = "";
            }

            string grafo = ProyectoSeleccionado.Clave.ToString();
            Guid proyectoID = ProyectoSeleccionado.Clave;
            if (TipoPagina == TiposPagina.Contribuciones || TipoPagina == TiposPagina.Borradores)
            {
                proyectoID = ProyectoAD.MetaProyecto;

                // Permitir añadir recursos desde página contribuciones                    
                if (TipoPagina == TiposPagina.Contribuciones)
                {
                    ViewBag.Comunidad.Permissions.CreateResource = true;
                    ViewBag.EsPaginaContribuciones = true;
                }

                if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
                {
                    string urlRedireccion = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "LOGIN") + mControladorBase.UrlParaLoginConRedirect;

                    return new RedirectResult(urlRedireccion);
                }
                if (TipoPagina == TiposPagina.Contribuciones && IdentidadPaginaContribuciones != null)
                {
                    if (IdentidadPaginaContribuciones.TrabajaConOrganizacion)
                    {
                        grafo = IdentidadPaginaContribuciones.OrganizacionID.Value.ToString();
                    }
                    else
                    {
                        grafo = IdentidadPaginaContribuciones.PerfilID.ToString();
                    }
                }
                else
                {
                    if (IdentidadActual.TrabajaConOrganizacion)
                    {
                        grafo = IdentidadActual.OrganizacionID.Value.ToString();
                    }
                    else
                    {
                        grafo = IdentidadActual.PerfilID.ToString();
                    }
                }
            }

            Guid tokenAfinidad = Guid.NewGuid();

            paginaModel.TipoPagina = TipoPagina.ToString();
            if (RequestParams("contribuciones") != null && RequestParams("contribuciones").Equals("true") && IdentidadPaginaContribuciones != null)
            {
                paginaModel.TipoPagina += "OtroPerfil";

                ViewBag.UrlPerfilContribuciones = mControladorBase.UrlsSemanticas.GetURLPerfilDeIdentidad(BaseURLIdioma, ProyectoSeleccionado.NombreCorto, UtilIdiomas, IdentidadPaginaContribuciones);
                ViewBag.UrlFotoPerfilContribuciones = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}{IdentidadPaginaContribuciones.UrlImagen}";
            }

            bool hayParametrosBusqueda = false;
            string argumentos = "";
            string rawUrl = Request.GetDisplayUrl();
            string separadorArgumentos = "";
            string parametroadicional = "";

            string ordenPorDefecto = "gnoss:hasfechapublicacion";
            string ordenEnSearch = "gnoss:relevancia";

            foreach (string param in Request.Query.Keys)
            {
                if (param != null && param.StartsWith("fac_"))
                {
                    hayParametrosBusqueda = true;

                    string filtro = $"{param.Substring(4)}={HttpUtility.UrlDecode(Request.Query[param])}";
                    argumentos += separadorArgumentos + filtro;
                    separadorArgumentos = "|";
                }
            }

            string ordenAplicarDefecto = string.Empty;
            if(paginaModel.FilterOrderList != null && paginaModel.FilterOrderList.Values.Count > 0)
            {
                ordenAplicarDefecto = paginaModel.FilterOrderList.Values.FirstOrDefault();
            }

            parametroadicional = ObtenerParametroAdicionalBusqueda(ordenAplicarDefecto);

            if (VariableTipoBusqueda.Equals(TipoBusqueda.PersonasYOrganizaciones))
            {
                ordenPorDefecto = "gnoss:hasPopularidad";
                ordenEnSearch = ordenPorDefecto;

                ViewBag.UrlLoadResourceActions += "/person";
            }

            if (FiltrosOrdenConfig.Count > 0 && RequestParams("filtroContexto") == null)
            {
                string filtro = new List<string>(FiltrosOrdenConfig.Keys)[0];
                string filtroLimpio = filtro;

                if (filtroLimpio.Contains("|"))
                {
                    filtroLimpio = filtro.Split('|')[0];
                }

                ordenPorDefecto = filtroLimpio;
            }

            if (!string.IsNullOrEmpty(Request.GetDisplayUrl()))
            {
                rawUrl = Request.GetDisplayUrl();

                //bug7595
                rawUrl = rawUrl.Replace("+", "%2B").Replace("%26", "---AMPERSAND---");
                rawUrl = rawUrl.Replace("%7C", "---TUBERIA---");
                rawUrl = rawUrl.Replace("%7c", "---TUBERIA---");
                rawUrl = HttpUtility.UrlDecode(rawUrl);
                rawUrl = rawUrl.Replace("---TUBERIA---", "%257C");
                rawUrl = rawUrl.Replace("---AMPERSAND---", "%26");

                //Detectado que puede estar doblemente codificado al haber más de un filtro, visto en InternetExplorer 8.
                if (UtilCadenas.PasarAUtf8(UtilCadenas.PasarAANSI(rawUrl)) == rawUrl)
                {
                    rawUrl = UtilCadenas.PasarAANSI(rawUrl);
                }
            }

            if (!string.IsNullOrEmpty(rawUrl) && rawUrl.Contains("?"))
            {
                Uri uri = new Uri(rawUrl);
                string[] partesUrl = rawUrl.Split("?");

                string urlLimpio = $"{UtilCadenas.LimpiarInyeccionCodigo(partesUrl[0])}?";
                string parametrosUrl = partesUrl[1];

                foreach (string filtro in parametrosUrl.Split("&"))
                {
                    urlLimpio += $"{UtilCadenas.LimpiarInyeccionCodigo(filtro)}&";

                    argumentos += separadorArgumentos + UtilCadenas.LimpiarInyeccionCodigo(filtro);

                    separadorArgumentos = "|";
                    hayParametrosBusqueda = true;
                }

                //Cuando se desea hace una busqueda por texto dentro de una faceta (usando el buscador de la faceta) se envia el filtro como =>>
                //con la limpieza de inyecciones esto se esta convirtiendo en =&gt;&gt; lo que hace que el servicio de resultados lo ignore al entender que se le inyecta html
                //para solucionarlo remplazamos =&gt;&gt; por =>>
                argumentos = argumentos.Replace("=&gt;&gt;", "=>>");

                rawUrl = urlLimpio.Trim('&');
            }

            string filtroPag = string.Empty;
            string urlReal = HttpUtility.UrlDecode(new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}").AbsoluteUri).ToLower();
            string urlNueva = ProcesarUrlParaRedireccion(urlReal, "");
            if (!urlNueva.Replace("https", Request.Scheme).ToLower().Equals(urlReal))
            {
                return RedirigirAPaginaCon301(urlNueva);
            }

            Dictionary<string, string> listafiltrosselecionados = ProcesarUrlObtenerFiltros(urlNueva, ref argumentos, ref filtroPag, ref separadorArgumentos);
            paginaModel.PageFiltersList = new List<string>();
            string tituloPaginaBusqueda = "";

            if (!string.IsNullOrEmpty(ProyectoPestanyaActual.Titulo))
            {
                tituloPaginaBusqueda = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }
            else if (!string.IsNullOrEmpty(ProyectoPestanyaActual.Nombre))
            {
                tituloPaginaBusqueda = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }
            else
            {
                switch (ProyectoPestanyaActual.TipoPestanya)
                {
                    case TipoPestanyaMenu.Recursos:
                        tituloPaginaBusqueda = UtilIdiomas.GetText("COMMON", "BASERECURSOS");
                        break;
                    case TipoPestanyaMenu.Preguntas:
                        tituloPaginaBusqueda = UtilIdiomas.GetText("COMMON", "PREGUNTAS");
                        break;
                    case TipoPestanyaMenu.Debates:
                        tituloPaginaBusqueda = UtilIdiomas.GetText("COMMON", "DEBATES");
                        break;
                    case TipoPestanyaMenu.Encuestas:
                        tituloPaginaBusqueda = UtilIdiomas.GetText("COMMON", "ENCUESTAS");
                        break;
                    case TipoPestanyaMenu.PersonasYOrganizaciones:
                        tituloPaginaBusqueda = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES");
                        break;
                    case TipoPestanyaMenu.BusquedaAvanzada:
                        break;
                }
            }

            if (listafiltrosselecionados.Count > 0)
            {
                if (listafiltrosselecionados.First().Key != "")
                {
                    if (tituloPaginaBusqueda != "")
                    {
                        tituloPaginaBusqueda = $"{tituloPaginaBusqueda} > ";
                    }
                    tituloPaginaBusqueda = tituloPaginaBusqueda + UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculas(listafiltrosselecionados.First().Key);
                }
                tituloPaginaBusqueda = $"{tituloPaginaBusqueda} > {listafiltrosselecionados.First().Value}";
            }

            if (!string.IsNullOrEmpty(tituloPaginaBusqueda))
            {
                tituloPaginaBusqueda = $"{tituloPaginaBusqueda} - ";
            }
            tituloPaginaBusqueda = tituloPaginaBusqueda + UtilCadenas.ObtenerTextoDeIdioma(ProyectoVirtual.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

            if (BaseURL.Contains("gnoss."))
            {
                tituloPaginaBusqueda = $"{tituloPaginaBusqueda} - GNOSS";
            }

            TituloPagina = tituloPaginaBusqueda;
            ViewBag.TituloPagina = TituloPagina;
            paginaModel.PageFiltersList = new List<string>(listafiltrosselecionados.Values);

            if (!filtroPag.Equals(string.Empty))
            {
                hayParametrosBusqueda = true;
            }

            string instruccionesExtra = "var filtroDePag = ''; var filtrosDeInicio = ''; filtrosPeticionActual = '';";

            if (!string.IsNullOrEmpty(argumentos))
            {
                instruccionesExtra = $"var textoCategoria='{UtilIdiomas.GetText("URLSEM", "CATEGORIA")}'; var textoBusqAvaz='{UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA")}'; var textoComunidad='{UtilIdiomas.GetText("URLSEM", "COMUNIDAD")}'; var filtroDePag = '{filtroPag.Replace("'", "\\'")}'; var filtrosDeInicio = decodeURIComponent('{UrlEncode(argumentos.Replace("%257C", "%7C"))}'); filtrosPeticionActual = filtrosDeInicio; ";
            }

            string filtroContexto = "";
            if (RequestParams("filtroContexto") != null)
            {
                filtroContexto = RequestParams("filtroContexto");
            }

            if (mFilaPestanyaBusqueda == null || !mFilaPestanyaBusqueda.OmitirCargaInicialFacetasResultados)
            {
                instruccionesExtra += $"$(document).ready(function () {{FinalizarMontarResultados(decodeURIComponent('{UrlEncode(parametroadicional)}'), '', 1, '#' + panResultados); enlazarJavascriptFacetas = true; FinalizarMontarFacetas(); }});";
            }
            insertarScriptBuscador("panFacetas", "panResultados", "panNumResultados", "panListadoFiltros", "panResultados", "panFiltros", grafo, "panNavegador", instruccionesExtra, "", parametroadicional, ordenPorDefecto, ordenEnSearch, filtroContexto, "", Guid.Empty, ProyectoSeleccionado.Clave, UbicacionBusqueda, (short)VariableTipoBusqueda, mFilaPestanyaBusqueda, null, mEsVistaMapa, tokenAfinidad);

            string argumentosResultados = argumentos;
            short tipoBusqueda = -1;
            if (!string.IsNullOrEmpty(RequestParams("pag")) || !string.IsNullOrEmpty(RequestParams("pagina")))
            {
                string parametroPagina = RequestParams("pag");
                if (RequestParams("pag") == null && RequestParams("pagina") != null)
                {
                    parametroPagina = RequestParams("pagina");
                }
                argumentosResultados += separadorArgumentos + "pagina=" + parametroPagina;

                AsignarMetaRobots("NOINDEX, FOLLOW");
                if (parametroPagina.Equals("1"))
                {
                    tipoBusqueda = (short)TipoPaginaMetaRobots.BusquedaPrimeraPagina;
                }
                else
                {
                    tipoBusqueda = (short)TipoPaginaMetaRobots.BusquedaPaginada;
                }
            }
            else
            {
                tipoBusqueda = (short)TipoPaginaMetaRobots.BusquedaPrimeraPagina;
            }

            if (tipoBusqueda != -1 && ProyectoMetaRobotsRows != null && ProyectoMetaRobotsRows.Any(filaMetaRobots => filaMetaRobots.Tipo.Equals(tipoBusqueda)))
            {
                AsignarMetaRobots(ProyectoMetaRobotsRows.First(filaMetaRobots => filaMetaRobots.Tipo.Equals(tipoBusqueda)).Content);
            }

            bool primeraCarga = string.IsNullOrEmpty(argumentosResultados);

            if (argumentos.EndsWith("|rss") || argumentos.EndsWith("?rss") || argumentos.EndsWith("|rdf") || argumentos.EndsWith("?rdf"))
            {
                argumentos = argumentos.Substring(0, argumentos.Length - 4);
            }
            else if (argumentos.StartsWith("rss|") || argumentos.StartsWith("rdf|"))
            {
                argumentos = argumentos.Substring(4);
            }

            if (EsPeticionRDF || EsPeticionRSS)
            {
                CargadorResultados cargadorResultados = new CargadorResultados();
                cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();

                sw = LoggingService.IniciarRelojTelemetria();
                ResultadoModel listaResultados = cargadorResultados.CargarResultadosJSON(proyectoID, mControladorBase.UsuarioActual.IdentidadID, mControladorBase.UsuarioActual.EsUsuarioInvitado, rawUrl, mControladorBase.UsuarioActual.UsarMasterParaLectura, AdministradorQuiereVerTodasLasPersonas, VariableTipoBusqueda, grafo, parametroadicional, argumentos, primeraCarga, UtilIdiomas.LanguageCode, -1, "", Request);
                mLoggingService.AgregarEntradaDependencia("Llamada al servicio de resultados. EsPeticionRDF || EsPeticionRSS", false, "BusquedaController.Index", sw, true);

                if (EsPeticionRDF)
                {
                    mRDFSearch.mUrlIntragnoss = UrlIntragnoss;
                    return File(mRDFSearch.CargarRDFListaResultados(listaResultados, ProyectoSeleccionado, OntologiaGnoss, IdiomaUsuario, UtilSemCms, UtilIdiomas, BaseURLIdioma, BaseURLContent, UrlPerfil, UrlPagina, UrlsSemanticas, ProyectoPestanyaActual.Clave, BaseURLFormulariosSem, IdentidadActual, UsuarioActual, ProyectoPrincipalUnico, ListaItemsBusqueda), "application/rdf+xml");
                }
                else if (EsPeticionRSS)
                {
                    return CargarRSS(listaResultados);
                }
            }
            if (!string.IsNullOrEmpty(RequestParams("ExportarDatosUsuario")))
            {
                return ExportacionDatosUsuario(proyectoID, rawUrl, grafo, RequestParams("ExportarDatosUsuario").Split("|", StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            else if (!string.IsNullOrEmpty(RequestParams("ParametrosExportacion")))
            {
                string pParametrosExportacion = RequestParams("ParametrosExportacion");
                string[] parametros = pParametrosExportacion.Split('|');
                Guid exportacionID = new Guid(parametros[0]);
                string nombreExportacion = parametros[1];
                string formato = parametros[2];

                if (!exportacionID.Equals(Guid.Empty))
                {
                    return ExportacionBusquedaComunidad(exportacionID, formato, parametroadicional, proyectoID, rawUrl, grafo, argumentos, primeraCarga, nombreExportacion);
                }
            }

            string filtrosPeticion = string.Empty;
            if (Request.Query.Count > 0)
            {
                filtrosPeticion = $"&{Request.QueryString.ToString()}";
                var data = Encoding.UTF8.GetBytes(filtrosPeticion);
                filtrosPeticion = Convert.ToBase64String(data);
            }

            if (ParametrosGeneralesRow.RssDisponibles)
            {
                string filtros = string.Empty;
                if (Request.Query.Count > 0)
                {
                    filtros = $"&{Request.QueryString.ToString()}";
                    var dataFiltros = Encoding.UTF8.GetBytes(filtros);
                    filtros = Convert.ToBase64String(dataFiltros);
                }
                ViewBag.URLRSS = $"{ViewBag.UrlPagina}?rss{filtros}";
            }

            if (ParametrosGeneralesRow.RdfDisponibles)
            {
                string filtros = string.Empty;
                if (Request.Query.Count > 0)
                {
                    filtros = $"&{Request.QueryString.ToString()}";
                    var dataFilters = Encoding.UTF8.GetBytes(filtros);
                    filtros = Convert.ToBase64String(dataFilters);
                }

                ViewBag.URLRDF = $"{ViewBag.UrlPagina}?rdf{filtros}";
            }

            if (!mEsVistaMapa)
            {
                Guid idetidadID = mControladorBase.UsuarioActual.IdentidadID;
                bool verTodasPersonas = AdministradorQuiereVerTodasLasPersonas;
                if (RequestParams("organizacion") != null)
                {
                    idetidadID = IdentidadActual.IdentidadOrganizacion.Clave;
                    verTodasPersonas = !string.IsNullOrEmpty(RequestParams("organizacion")) && VariableTipoBusqueda == TipoBusqueda.Contribuciones && EsIdentidadActualAdministradorOrganizacion;
                }

                // AQUI
                int numResultados = CargarResultadosYFacetas(paginaModel, proyectoID, ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto), !mControladorBase.UsuarioActual.EsIdentidadInvitada, mControladorBase.UsuarioActual.EsUsuarioInvitado, idetidadID, argumentos.Replace("\"", "%2522"), primeraCarga, verTodasPersonas, VariableTipoBusqueda, rawUrl, grafo, parametroadicional, hayParametrosBusqueda, UbicacionBusqueda, UtilIdiomas.LanguageCode, mControladorBase.UsuarioActual.UsarMasterParaLectura, tokenAfinidad);

                string tituloPaginaBusquedaDescripcion = "";
                string metaTagDescription = "";
                string nombreProyecto = UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

                if (!string.IsNullOrEmpty(ProyectoPestanyaActual.Titulo))
                {
                    tituloPaginaBusquedaDescripcion = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }
                else if (!string.IsNullOrEmpty(ProyectoPestanyaActual.Nombre))
                {
                    tituloPaginaBusquedaDescripcion = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }
                else
                {
                    switch (ProyectoPestanyaActual.TipoPestanya)
                    {
                        case TipoPestanyaMenu.Recursos:
                            tituloPaginaBusquedaDescripcion = UtilIdiomas.GetText("COMMON", "BASERECURSOS");
                            break;
                        case TipoPestanyaMenu.Preguntas:
                            tituloPaginaBusquedaDescripcion = UtilIdiomas.GetText("COMMON", "PREGUNTAS");
                            break;
                        case TipoPestanyaMenu.Debates:
                            tituloPaginaBusquedaDescripcion = UtilIdiomas.GetText("COMMON", "DEBATES");
                            break;
                        case TipoPestanyaMenu.Encuestas:
                            tituloPaginaBusquedaDescripcion = UtilIdiomas.GetText("COMMON", "ENCUESTAS");
                            break;
                        case TipoPestanyaMenu.PersonasYOrganizaciones:
                            tituloPaginaBusquedaDescripcion = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES");
                            break;
                    }
                }

                string terminacionTextosFemenino = "";

                if (ProyectoPestanyaActual != null && !string.IsNullOrEmpty(ProyectoPestanyaActual.MetaDescription))
                {
                    metaTagDescription = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.MetaDescription, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }
                else if (listafiltrosselecionados.Count > 0)
                {
                    string tipoFiltro = listafiltrosselecionados.First().Key;
                    string valorFiltro = listafiltrosselecionados.First().Value;

                    if (tipoFiltro == UtilIdiomas.GetText("URLSEM", "CATEGORIA"))
                    {
                        if (ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.BusquedaAvanzada)
                        {
                            metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", "CATBUSQUEDA", valorFiltro, numResultados.ToString(), nombreProyecto);
                        }
                        else
                        {
                            metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", $"CAT{terminacionTextosFemenino}", tituloPaginaBusquedaDescripcion, valorFiltro, numResultados.ToString(), nombreProyecto);
                        }
                    }
                    else if (tipoFiltro == UtilIdiomas.GetText("URLSEM", "TAG"))
                    {
                        if (ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.BusquedaAvanzada)
                        {
                            metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", "TAGBUSQUEDA", valorFiltro, numResultados.ToString(), nombreProyecto);
                        }
                        else
                        {
                            metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", $"TAG{terminacionTextosFemenino}", tituloPaginaBusquedaDescripcion, valorFiltro, numResultados.ToString(), nombreProyecto);
                        }
                    }
                    else if (!string.IsNullOrEmpty(tipoFiltro))
                    {
                        if (ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.BusquedaAvanzada)
                        {
                            metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", "TAGBUSQUEDA", valorFiltro, numResultados.ToString(), nombreProyecto);
                        }
                        else
                        {
                            metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", $"TAG{terminacionTextosFemenino}", tituloPaginaBusquedaDescripcion, valorFiltro, numResultados.ToString(), nombreProyecto);
                        }
                    }
                    else
                    {
                        if (ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.BusquedaAvanzada)
                        {
                            metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", "SEARCHBUSQUEDA", valorFiltro, numResultados.ToString(), nombreProyecto);
                        }
                        else
                        {
                            metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", $"SEARCH{terminacionTextosFemenino}", tituloPaginaBusquedaDescripcion, valorFiltro, numResultados.ToString(), nombreProyecto);
                        }
                    }
                }
                else
                {
                    if (ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.BusquedaAvanzada)
                    {
                        metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", "SINFILTROBUSQUEDA", numResultados.ToString(), nombreProyecto);
                    }
                    else
                    {
                        metaTagDescription = UtilIdiomas.GetText("METADESCRIPTION", $"SINFILTRO{terminacionTextosFemenino}", numResultados.ToString(), nombreProyecto, tituloPaginaBusquedaDescripcion);
                    }
                }

                if (metaTagDescription.Length > 150)
                {
                    metaTagDescription = metaTagDescription.Substring(0, 150) + "...";
                }

                ViewBag.ListaMetas.Add(new ViewMetaData { AttributeName = "name", AttributeValue = "description", ContentAttributeValue = metaTagDescription });
            }

            // Mostrar la vista Administrar Miembros desde Administración"
            if (RequestParams("admin") != null && RequestParams("admin").Equals("true"))
            {
                UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
                bool tienePermiso = false;
                if (RequestParams("ecosistema") != null && RequestParams("ecosistema").Equals("true"))
                {
                    tienePermiso = utilPermisos.UsuarioTienePermisoAdministracionEcosistema((ulong)PermisoEcosistema.AdministrarMiembrosEcosistema, mControladorBase.UsuarioActual.UsuarioID);
                }
                else
                {
					tienePermiso = utilPermisos.IdentidadTienePermiso((ulong)PermisoComunidad.GestionarMiembros, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Comunidad);
				}
                if (!tienePermiso)
                {
                    return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
                CargarPermisosAdministracionComunidadEnViewBag();
                EliminarPersonalizacionVistas();
                // Añadir clase para el body del Layout
                ViewBag.BodyClassPestanya = "miembros-comunidad no-max-width-container listado";
                ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Comunidad;
                ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Comunidad_Miembros;
                // Establecer el título para el header de DevTools
                ViewBag.HeaderParentTitle = UtilIdiomas.GetText("ADMINISTRACIONSEMANTICA", "COMUNIDAD");
                ViewBag.HeaderTitle = UtilIdiomas.GetText("COMADMIN", "MIEMBROS");
                ViewBag.PermisosPaginas = PermisosPaginasAdministracion;
                // Construyo la vista a devolver junto con el modelo para miembros de la comunidad
                ActionResult gestionMiembrosView = View();
                // Activar la visualización del icono de la documentación de la sección
                ViewBag.showDocumentationByDefault = "true";
                // Indica si sólo se desea visualizar la documentación por secciones (Ej: Mostrar la documentación en el modal con el contenido desplegado/plegado)
                ViewBag.showDocumentationSection = "true";
				bool isInEcosistemaPlatform = !string.IsNullOrEmpty(RequestParams("ecosistema")) ? (bool.Parse(RequestParams("ecosistema"))) : false;
				if (isInEcosistemaPlatform)
				{
					ViewBag.isInEcosistemaPlatform = "true";
				}
				gestionMiembrosView = GnossResultHtml("../Busqueda/Index_AdministrarMiembros", paginaModel);

                return gestionMiembrosView;
            }
            return View(paginaModel);
        }


        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult LoadModalUserPagePermissions(Guid identidadID)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            Guid usuarioID = identidadCN.ObtenerUsuarioIDConIdentidadID(identidadID);

            // Obtener una lista con los permisos de los que dispone el usuario (true/false)
            List<PermisosPaginasUsuarios> listaPermisosPaginas = ObtenerModeloPagePermissions(usuarioID);

            PermisosPaginaUsuarioModel permisosPaginaUsuario = new PermisosPaginaUsuarioModel();
            permisosPaginaUsuario.IdentidadID = identidadID;
            if (listaPermisosPaginas.Count > 0)
            {
                permisosPaginaUsuario.Disenyo = listaPermisosPaginas.Any(item => item.Pagina == (short)TipoPaginaAdministracion.Diseño);
                permisosPaginaUsuario.Pagina = listaPermisosPaginas.Any(item => item.Pagina == (short)TipoPaginaAdministracion.Pagina);
                permisosPaginaUsuario.Semantica = listaPermisosPaginas.Any(item => item.Pagina == (short)TipoPaginaAdministracion.Semantica);
                permisosPaginaUsuario.Tesauro = listaPermisosPaginas.Any(item => item.Pagina == (short)TipoPaginaAdministracion.Tesauro);
                permisosPaginaUsuario.Texto = listaPermisosPaginas.Any(item => item.Pagina == (short)TipoPaginaAdministracion.Texto);
            }

            // Devolver la vista con el modelo
            return PartialView("~/Views/Administracion/AdministrarPermisosPaginas/_modal-views/_config-page-permissions.cshtml", permisosPaginaUsuario);
        }

        /// <summary>
        /// Método para guardar los permisos que dispondrá un usuario sobre las páginas de la comunidad./ al hacer click en "Administrar permisos de páginas"
        /// </summary>
        /// <param name="permisosPaginaUsuario">Permisos asignados a las páginas adminsitración de un usuario</param>         
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult SaveUserPagePermissions(PermisosPaginaUsuarioModel permisosPaginaUsuario)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            Guid usuarioID = identidadCN.ObtenerUsuarioIDConIdentidadID(permisosPaginaUsuario.IdentidadID);

            List<PermisosPaginasUsuarios> listaPermisosPaginas = ObtenerModeloPagePermissions(usuarioID);

            try
            {
                if (listaPermisosPaginas.Count > 0)
                {
                    EliminarPermisosAntiguos(listaPermisosPaginas);
                }

                AgregarPermisosNuevos(permisosPaginaUsuario, usuarioID);
            }
            catch (Exception ex)
            {
                return GnossResultERROR(ex.Message);
            }

            return GnossResultOK();
        }

        /// <summary>
        /// Elimina todos los permisos antiguos para el usuario indicado
        /// </summary>
        /// <param name="pListaPermisosPaginasAntiguos">Lista de permisos a eliminar</param>
        private void EliminarPermisosAntiguos(List<PermisosPaginasUsuarios> pListaPermisosPaginasAntiguos)
        {
            foreach (PermisosPaginasUsuarios permisosPaginasUsuario in pListaPermisosPaginasAntiguos)
            {
                mEntityContext.PermisosPaginasUsuarios.Remove(permisosPaginasUsuario);
            }
        }

        /// <summary>
        /// Agrega los permisos nuevos indicados para el usuario indicado
        /// </summary>
        /// <param name="pPermisosPaginaUsuarioModel">Modelo con el usuario y los permisos seleccionados</param>
        private void AgregarPermisosNuevos(PermisosPaginaUsuarioModel pPermisosPaginaUsuarioModel, Guid pUsuarioID)
        {
            if (pPermisosPaginaUsuarioModel.Disenyo)
            {
                AgregarPermisoNuevo(TipoPaginaAdministracion.Diseño, pUsuarioID);
            }
            if (pPermisosPaginaUsuarioModel.Pagina)
            {
                AgregarPermisoNuevo(TipoPaginaAdministracion.Pagina, pUsuarioID);
            }
            if (pPermisosPaginaUsuarioModel.Semantica)
            {
                AgregarPermisoNuevo(TipoPaginaAdministracion.Semantica, pUsuarioID);
            }
            if (pPermisosPaginaUsuarioModel.Tesauro)
            {
                AgregarPermisoNuevo(TipoPaginaAdministracion.Tesauro, pUsuarioID);
            }
            if (pPermisosPaginaUsuarioModel.Texto)
            {
                AgregarPermisoNuevo(TipoPaginaAdministracion.Texto, pUsuarioID);
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Agrega el permiso indicado a base de datos
        /// </summary>
        /// <param name="pTipoPagina">Tipo de página a agregar el permiso</param>
        /// <param name="pUsuarioID">Usuario al cual se le agregará el permiso</param>
        private void AgregarPermisoNuevo(TipoPaginaAdministracion pTipoPagina, Guid pUsuarioID)
        {
            PermisosPaginasUsuarios permisoPaginaUsuario = new PermisosPaginasUsuarios();
            permisoPaginaUsuario.Pagina = (short)pTipoPagina;
            permisoPaginaUsuario.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
            permisoPaginaUsuario.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            permisoPaginaUsuario.UsuarioID = pUsuarioID;

            mEntityContext.PermisosPaginasUsuarios.Add(permisoPaginaUsuario);
        }

        /// <summary>
        /// Método para construir el modelo que corresponde con los permisos de los que dispone un usuario en las páginas de Administración.
        /// Este método construye el modelo para la vista modal _config-page-permissions que se utiliza desde "Administrar Miembros" de la Administración 
        /// al hacer click en "Administrar permisos de páginas"
        /// </summary>
        /// <param name="pUserID">Identificación del usuario del que se desean obtener las páginas sobre las que tiene permiso</param>        
        private List<PermisosPaginasUsuarios> ObtenerModeloPagePermissions(Guid pUserID)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            return proyectoCN.ObtenerPermisosPaginasProyectoUsuarioID(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.FilaProyecto.ProyectoID, pUserID);
        }

        private ActionResult ExportacionDatosUsuario(Guid pProyectoID, string pRawUrl, string pGrafo, List<string> pParametros)
        {
            mLoggingService.AgregarEntrada("Inicio Exportacion");

            CargadorResultados cargadorResultados = new CargadorResultados();
            cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();

            Stopwatch sw = LoggingService.IniciarRelojTelemetria();
            string parametros_adicionales = $"|busquedaSoloIDs=true|numeroResultados=-1";

            string resultado = cargadorResultados.CargarResultados(pProyectoID, mControladorBase.UsuarioActual.IdentidadID, mControladorBase.UsuarioActual.EsUsuarioInvitado, pRawUrl, mControladorBase.UsuarioActual.UsarMasterParaLectura, AdministradorQuiereVerTodasLasPersonas, VariableTipoBusqueda, pGrafo, parametros_adicionales, "", true, UtilIdiomas.LanguageCode, -1, "");

            mLoggingService.AgregarEntradaDependencia("Llamar servicio resultados", false, "ExportacionDatosUsuario", sw, true);

            return MontarCSVExcelUsuarios(pProyectoID, resultado, pParametros, VariableTipoBusqueda);
        }

        private ActionResult ExportacionBusquedaComunidad(Guid pExportacionID, string pFormato, string pParametroadicional, Guid pProyectoID, string pRawUrl, string pGrafo, string pArgumentos, bool pPrimeraCarga, string pNombreExportacion)
        {
            mLoggingService.AgregarEntrada("Inicio Exportacion");

            CargadorResultados cargadorResultados = new CargadorResultados();
            cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();

            Stopwatch sw = LoggingService.IniciarRelojTelemetria();
            string parametros_adicionales = $"{pParametroadicional}|busquedaSoloIDs=true|numeroResultados=-1";

            string resultados = cargadorResultados.CargarResultados(pProyectoID, mControladorBase.UsuarioActual.IdentidadID, mControladorBase.UsuarioActual.EsUsuarioInvitado, pRawUrl, mControladorBase.UsuarioActual.UsarMasterParaLectura, AdministradorQuiereVerTodasLasPersonas, VariableTipoBusqueda, pGrafo, parametros_adicionales, pArgumentos, pPrimeraCarga, UtilIdiomas.LanguageCode, -1, "");

            mLoggingService.AgregarEntradaDependencia("Llamar servicio resultados", false, "ExportacionBusquedaComunidad", sw, true);

            return MontarCSVExcelComunidad(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, pExportacionID, resultados, ListaItemsBusqueda, VariableTipoBusqueda, $"{pParametroadicional}|{pArgumentos}", pFormato, pNombreExportacion);
        }

        public void ObtenerFiltrosBusqueda(List<string> LItems)
        {
            List<string> comunidades = new List<string>();
            if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MyGnoss))
            {
                comunidades.Add($"gnoss:{mControladorBase.UsuarioActual.ProyectoID.ToString().ToUpper()}");
            }

            if (ListaFiltrosFacetas.ContainsKey("sioc:has_space"))
            {
                ListaFiltrosFacetas["sioc:has_space"].AddRange(comunidades);
            }

            List<string> tipoitem = new List<string>();
            foreach (string item in LItems)
            {
                if (!tipoitem.Contains(item))
                {
                    tipoitem.Add(item);
                }
            }

            if (ListaFiltrosFacetas.ContainsKey("rdf:type"))
            {
                foreach (string tipo in tipoitem)
                {
                    if (!ListaFiltrosFacetas["rdf:type"].Contains(tipo))
                    {
                        ListaFiltrosFacetas["rdf:type"].Add(tipo);
                    }
                }
            }
            else
            {
                ListaFiltrosFacetas.Add("rdf:type", tipoitem);
            }

            //TipoBusqueda
            if ((VariableTipoBusqueda == TipoBusqueda.Contribuciones) && (IdentidadActual.OrganizacionID.HasValue) && string.IsNullOrEmpty(RequestParams("organizacion")))
            {
                List<string> L = new List<string>();
                L.Add($"gnoss:{IdentidadActual.Clave.ToString().ToUpper()}");
                ListaFiltrosFacetas.Add("gnoss:haspublicadorIdentidadID", L);
            }
        }

        public Dictionary<Guid, TiposResultadosMetaBuscador> ObtenerListaID(FacetadoDS facetadoDS)
        {
            Dictionary<Guid, TiposResultadosMetaBuscador> listaIdsResultado = new Dictionary<Guid, TiposResultadosMetaBuscador>();

            if (facetadoDS.Tables.Contains("RecursosBusqueda"))
            {
                foreach (DataRow myrow in facetadoDS.Tables["RecursosBusqueda"].Rows)
                {
                    try
                    {
                        Guid id = obtenerIDDesdeURI((string)myrow[0]);
                        if (!listaIdsResultado.ContainsKey(id))
                        {
                            listaIdsResultado.Add(id, TipoResultadoToTiposResultadoMetaBuscador((string)myrow["rdftype"]));
                        }

                        if (listaIdsResultado.Count == 2000)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, mlogger);
                    }
                }
            }

            return listaIdsResultado;
        }

        public Guid obtenerIDDesdeURI(string URIID)
        {
            Guid idGuid = Guid.Empty;

            string id = "";
            if (URIID.Contains(":"))
            {
                id = URIID.Substring(URIID.IndexOf("gnoss") + 6);
            }
            else
            {
                id = URIID.ToUpper();
            }

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    idGuid = new Guid(id);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }

            return idGuid;
        }

        private TiposResultadosMetaBuscador TipoResultadoToTiposResultadoMetaBuscador(string pTipo)
        {
            TiposResultadosMetaBuscador tipo = TiposResultadosMetaBuscador.Documento;
            switch (pTipo)
            {
                case FacetadoAD.BUSQUEDA_RECURSOS:
                case FacetadoAD.BUSQUEDA_DEBATES:
                case FacetadoAD.BUSQUEDA_PREGUNTAS:
                case FacetadoAD.BUSQUEDA_ENCUESTAS:
                case FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES:
                    tipo = TiposResultadosMetaBuscador.Documento;
                    break;
                case FacetadoAD.BUSQUEDA_DAFOS:
                    tipo = TiposResultadosMetaBuscador.Dafo;
                    break;
                case FacetadoAD.BUSQUEDA_PERSONA:
                case FacetadoAD.BUSQUEDA_PROFESOR:
                case FacetadoAD.BUSQUEDA_ALUMNO:
                    tipo = TiposResultadosMetaBuscador.IdentidadPersona;
                    break;
                case FacetadoAD.BUSQUEDA_ORGANIZACION:
                case FacetadoAD.BUSQUEDA_CLASE:
                case FacetadoAD.BUSQUEDA_CLASE_UNIVERSIDAD:
                case FacetadoAD.BUSQUEDA_CLASE_SECUNDARIA:
                    tipo = TiposResultadosMetaBuscador.IdentidadOrganizacion;
                    break;
                case FacetadoAD.BUSQUEDA_COMUNIDADES:
                case FacetadoAD.BUSQUEDA_COMUNIDAD_EDUCATIVA:
                case FacetadoAD.BUSQUEDA_COMUNIDAD_NO_EDUCATIVA:
                    tipo = TiposResultadosMetaBuscador.Comunidad;
                    break;
                case FacetadoAD.BUSQUEDA_BLOGS:
                    tipo = TiposResultadosMetaBuscador.Blog;
                    break;

                case FacetadoAD.BUSQUEDA_ARTICULOSBLOG:
                    tipo = TiposResultadosMetaBuscador.EntradaBlog;
                    break;

                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENTARIOS:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMARTICULOBLOG:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMDEBATES:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENCUESTAS:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMFACTORDAFO:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMPREGUNTAS:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMRECURSOS:
                    tipo = TiposResultadosMetaBuscador.Comentario;
                    break;
            }

            return tipo;
        }
        private ActionResult MontarCSVExcelUsuarios(Guid pProyectoID, string pRespuestaServicioResultados, List<string> pParametros, TipoBusqueda pTipoBusqueda)
        {
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            List<ElementoGnoss> listaDefinitiva = UtilExportaciones.ObtenerListaDocumentosGnossConLaRespuestaDelServicioDeResultados(pRespuestaServicioResultados, ProyectoSeleccionado.Clave);

            mLoggingService.AgregarEntradaDependencia("Llamar construcción fichero", false, "MontarCSVExcelComunidad", sw, true);

            DataSet dsExportacion = ObtenerDatosExportacionMiembrosComunidad(pProyectoID, listaDefinitiva, pTipoBusqueda, pParametros);

            string fileName = $"{UtilIdiomas.GetText("DEVTOOLS", "EXPORTACIONDATOSCOMUNIDAD")}_{ProyectoSeleccionado.NombreCorto}";

            return File(UtilExportaciones.EscribirDataTableEnCSV(dsExportacion.Tables["Exportacion"]).ToArray(), "text/csv", $"{fileName}.csv");
        }
        /// <summary>
        /// Monta el CSV del excel pedido desde una comunidad.
        /// </summary>
        private ActionResult MontarCSVExcelComunidad(Guid pProyectoID, Guid pOrgIDProyBusquedaExcelCom, Guid pExportacionID, string pRespuestaServicioResultados, List<string> pListaItemsBusqueda, TipoBusqueda pTipoBusqueda, string pArgumentos, string pFormato, string pNombreExportacion)
        {
            ExportacionBusquedaCL exporBusCL = new ExportacionBusquedaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ExportacionBusquedaCL>(), mLoggerFactory);
            DataWrapperExportacionBusqueda exporBusDW = exporBusCL.ObtenerExportacionesProyecto(pProyectoID);
            exporBusCL.Dispose();

            pFormato = pFormato.ToLower();
            Stopwatch sw = null;


            if (exporBusDW.ListaProyectoPestanyaBusquedaExportacionExterna.Any(item => item.ExportacionID.Equals(pExportacionID)))
            {
                sw = LoggingService.IniciarRelojTelemetria();

                Dictionary<string, string> parametros = ObtenerDiccionarioDeParametrosParaServicioExterno(pProyectoID, pOrgIDProyBusquedaExcelCom, pExportacionID, pRespuestaServicioResultados, pListaItemsBusqueda, pTipoBusqueda, pFormato);
                string urlServicioxterno = exporBusDW.ListaProyectoPestanyaBusquedaExportacionExterna.FirstOrDefault(item => item.ExportacionID.Equals(pExportacionID)).UrlServicioExterno;
                WebResponse respuesta = UtilWeb.HacerPeticionPostDevolviendoWebResponse(urlServicioxterno, parametros);

                mLoggingService.AgregarEntradaDependencia("Llamar servicio externo", false, "MontarCSVExcelComunidad", sw, true);

                Stream stream = respuesta.GetResponseStream();

                string tempFileName = UtilCadenas.ObtenerTextoDeIdioma(pNombreExportacion, UtilIdiomas.LanguageCode, null);

                string contentType;
                if (pFormato.Equals(FormatosExportancion.EXCEL))
                {
                    tempFileName += ".xlsx";
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }
                else
                {
                    tempFileName += ".csv";
                    contentType = "text/csv";
                }

                return File(stream, contentType, tempFileName);
            }
            else
            {
                sw = LoggingService.IniciarRelojTelemetria();

                List<ElementoGnoss> listaDefinitiva = UtilExportaciones.ObtenerListaDocumentosGnossConLaRespuestaDelServicioDeResultados(pRespuestaServicioResultados, ProyectoSeleccionado.Clave);

                mLoggingService.AgregarEntradaDependencia("Llamar construcción fichero", false, "MontarCSVExcelComunidad", sw, true);

                DataSet dsExportacion = ObtenerDatosExportacionComunidad(exporBusDW, pProyectoID, pOrgIDProyBusquedaExcelCom, pExportacionID, listaDefinitiva, pListaItemsBusqueda, pTipoBusqueda, pArgumentos);

                //TODO: Implementar la exportación en excel
                //if (pFormato.Equals(FormatosExportancion.CSV))
                //{
                    return File(UtilExportaciones.EscribirDataTableEnCSV(dsExportacion.Tables["Exportacion"]).ToArray(), "text/csv", pNombreExportacion + ".csv");
                //}
                //else
                //{
                //    return File(UtilExportaciones.EscribirDataTableEnExcel(dsExportacion.Tables["Exportacion"], "Exportacion").ToArray(), "application///vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{UtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "EXPORTARBUSQUEDA")}.xlsx");
                //}
            }
        }

        private Dictionary<string, string> ObtenerDiccionarioDeParametrosParaServicioExterno(Guid pProyectoID, Guid pOrgIDProyBusquedaExcelCom, Guid pExportacionID, string pRespuestaServicioResultados, List<string> pListaItemsBusqueda, TipoBusqueda pTipoBusqueda, string pFormato)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            parametros.Add("pUsuarioID", UsuarioActual.UsuarioID.ToString());
            parametros.Add("pProyectoID", pProyectoID.ToString());
            parametros.Add("pOrgIDProyBusquedaExcelCom", pOrgIDProyBusquedaExcelCom.ToString());
            parametros.Add("pExportacionID", pExportacionID.ToString());

            parametros.Add("pRespuestaServicioResultados", pRespuestaServicioResultados);
            parametros.Add("pListaItemsBusqueda", JsonConvert.SerializeObject(pListaItemsBusqueda));

            parametros.Add("pTipoBusqueda", pTipoBusqueda.ToString());
            parametros.Add("pFormato", pFormato);

            parametros.Add("UserLanguaje", UtilIdiomas.LanguageCode);

            return parametros;
        }

        private string DevolverStringSiesFecha(string valorpropiedad)
        {
            string valorFinal = valorpropiedad;
            try
            {
                if (valorpropiedad.Length == 14)
                {

                    DateTime fecha;
                    if (DateTime.TryParseExact(valorpropiedad, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                    {
                        if (valorpropiedad.EndsWith("000000"))
                        {
                            valorFinal = fecha.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            valorFinal = fecha.ToString("dd/MM/yyyy HH:dd:ss");
                        }
                    }
                }
            }
            catch (Exception)
            { }

            return valorFinal;
        }

        private DataSet ObtenerDatosExportacionMiembrosComunidad(Guid pProyectoID, List<ElementoGnoss> pListaDefinitiva, TipoBusqueda pTipoBusqueda, List<string> pColumnas)
        {
            DataSet dsExportacion = new DataSet();
            DataTable dtContenidos = new DataTable();
            dtContenidos.TableName = "Exportacion";
            dsExportacion.Tables.Add(dtContenidos);


            foreach (string columna in pColumnas)
            {
                string page = "";
                switch (columna)
                {
                    case "NOMBRE":
                        page = "COMMON";
                        break;
                    case "APELLIDOS":
                        page = "INVITACIONES";
                        break;
                    case "EMAIL":
                    case "FECHANACIMIENTOSIN*":
                        page = "REGISTRO";
                        break;
                    default:
                        page = "DEVTOOLS";
                        break;
                }
                dsExportacion.Tables["Exportacion"].Columns.Add(UtilIdiomas.GetText(page, columna));
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto dataWrapperProyectoDatosExtra = proyCN.ObtenerDatosExtraProyectoPorID(ProyectoSeleccionado.Clave);
            proyCN.Dispose();

            List<Guid> listaIdentidades = new List<Guid>();
            foreach (ElementoGnoss elemento in pListaDefinitiva)
            {
                if (elemento is Identidad)
                {
                    listaIdentidades.Add(elemento.Clave);
                }
            }

            Dictionary<Guid, Dictionary<string, int>> dicContadoresIdentidad = null;
            DataWrapperIdentidad identDWDatosExtra = new DataWrapperIdentidad();
            if (listaIdentidades.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                identDWDatosExtra = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(listaIdentidades);
                dicContadoresIdentidad = identidadCN.ObtenerContadoresDeIdentidad(listaIdentidades);
                identidadCN.Dispose();
            }

            PaisCL paisCL = new PaisCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PaisCL>(), mLoggerFactory);
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            DataWrapperPais paisDW = null;

            foreach (ElementoGnoss elemento in pListaDefinitiva)
            {
                Dictionary<string, List<string>> dicCSVfiles = new Dictionary<string, List<string>>();

                StringBuilder datosExtraPropiedad_FormatoCSV = new StringBuilder();
                List<string> fila = new List<string>(); // Fila de la tabala

                Identidad identidad = (Identidad)elemento;

                foreach (string columna in pColumnas)
                {
                    string aux = columna;
                    if (columna.Equals("FECHANACIMIENTOSIN*"))
                    {
                        aux = "FECHANACIMIENTO";
                    }
                    fila.Add(ObtenerDatosPersonasYOrganizacionesPorPropiedad(aux.ToLower(), identidad, dicContadoresIdentidad, paisDW, paisCL, usuarioCN, dataWrapperProyectoDatosExtra, identDWDatosExtra));

                    // TODO -> Añadir metodo de identidadMVC para añadir datos tipo recive newsletter, esta bloqueado, esta expulsado y ya.
                }

                if (dicCSVfiles.Keys.Count == 0)
                {
                    dsExportacion.Tables["Exportacion"].Rows.Add(fila.ToArray());
                }

                if (dicCSVfiles.Keys.Count > 0)
                {
                    List<string> listaFilasCSVDefinitivas = ObtenerFilasCSVRemplazandoValores(dicCSVfiles, null, identidad.Clave, datosExtraPropiedad_FormatoCSV.ToString(), string.Empty, false);

                    foreach (string filaCSVDefinitiva in listaFilasCSVDefinitivas)
                    {
                        dsExportacion.Tables["Exportacion"].Rows.Add(filaCSVDefinitiva.Substring(0, filaCSVDefinitiva.Length - 1).Split(';'));
                    }
                }
            }
            return dsExportacion;
        }

        private DataSet ObtenerDatosExportacionComunidad(DataWrapperExportacionBusqueda pExporBusDW, Guid pProyectoID, Guid pOrgIDProyBusquedaExcelCom, Guid pExportacionID, List<ElementoGnoss> pListaDefinitiva, List<string> pListaItemsBusqueda, TipoBusqueda pTipoBusqueda, string pArgumentos)
        {
            DataSet dsExportacion = new DataSet();
            DataTable dtContenidos = new DataTable();
            dtContenidos.TableName = "Exportacion";
            dsExportacion.Tables.Add(dtContenidos);

            foreach (ProyectoPestanyaBusquedaExportacionPropiedad filaPropExport in pExporBusDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Where(item => item.ExportacionID.Equals(pExportacionID)).OrderBy(item => item.Orden))
            {
                string nombreColumna = nombreColumna = UtilCadenas.ObtenerTextoDeIdioma(filaPropExport.NombrePropiedad, mControladorBase.IdiomaUsuario, null);
                dsExportacion.Tables["Exportacion"].Columns.Add(nombreColumna);
            }

            #region TratamientoDocumentos

            FacetadoDS facDSPresentacion = null;
            Dictionary<string, List<string>> informacionOntologias = null;

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            DataWrapperFacetas dataWrapperFacetas = facetaCL.ObtenerFacetasDeProyecto(pListaItemsBusqueda, pProyectoID, false);
            GestionFacetas gestorFacetas = new GestionFacetas(dataWrapperFacetas);
            facetaCL.Dispose();


            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto dataWrapperProyectoDatosExtra = proyCN.ObtenerDatosExtraProyectoPorID(ProyectoSeleccionado.Clave);
            proyCN.Dispose();

            List<Guid> listaIdentidades = new List<Guid>();
            foreach (ElementoGnoss elemento in pListaDefinitiva)
            {
                if (elemento is Identidad)
                {
                    listaIdentidades.Add(elemento.Clave);
                }
            }

            Dictionary<Guid, Dictionary<string, int>> dicContadoresIdentidad = null;
            DataWrapperIdentidad identDWDatosExtra = new DataWrapperIdentidad();
            if (listaIdentidades.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                identDWDatosExtra = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(listaIdentidades);
                dicContadoresIdentidad = identidadCN.ObtenerContadoresDeIdentidad(listaIdentidades);
                identidadCN.Dispose();
            }

            PaisCL paisCL = new PaisCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PaisCL>(), mLoggerFactory);
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            DataWrapperPais paisDW = null;

            foreach (ElementoGnoss elemento in pListaDefinitiva)
            {
                Dictionary<string, List<string>> dicCSVfiles = new Dictionary<string, List<string>>();

                // Recoge las propiedades del campo DatosExtraPropiedad de la tabla ProyectoPestanyaBusquedaExportacionPropiedad. Mantener con el formatoCSV para identificar que usa ; aunque posteriormente lo pasa al DataSet
                StringBuilder datosExtraPropiedad_FormatoCSV = new StringBuilder();
                List<string> fila = new List<string>(); // Fila de la tabala

                #region Documento

                if (elemento is Documento)
                {
                    DocumentoWeb documentoWeb = null;

                    if (elemento is DocumentoWeb)
                    {
                        documentoWeb = (DocumentoWeb)elemento;
                    }
                    else
                    {
                        Documento documento = (Documento)elemento;
                        documentoWeb = new DocumentoWeb(documento.FilaDocumento, documento.GestorDocumental);
                    }

                    string propiedadTransitiva = string.Empty;
                    bool ultimoValorRequerido = false;
                    string predicadoProp = string.Empty;
                    foreach (ProyectoPestanyaBusquedaExportacionPropiedad filaPropExport in pExporBusDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Where(item => item.ExportacionID.Equals(pExportacionID)).OrderBy(item => item.Orden))
                    {
                        if (filaPropExport.OntologiaID.HasValue && (documentoWeb.TipoDocumentacion != TiposDocumentacion.Semantico || filaPropExport.OntologiaID != documentoWeb.ElementoVinculadoID))
                        {
                            //thisCSVFile.Append(";");
                            // GuardarValorSegunFormato(";", thisCSVFile, fila, pFormato);
                        }
                        else if (filaPropExport.OntologiaID.HasValue && !string.IsNullOrEmpty(filaPropExport.Ontologia))
                        {
                            if (facDSPresentacion == null)
                            {
                                informacionOntologias = UtilServiciosFacetas.ObtenerInformacionOntologias(pOrgIDProyBusquedaExcelCom, pProyectoID);

                                facDSPresentacion = ObtenerPresentacionFacetado(pExporBusDW, pListaDefinitiva, pProyectoID, informacionOntologias, pExportacionID, pArgumentos);
                            }

                            predicadoProp = string.Empty;
                            string valorProp = PintarPropiedadesSemDeVista(filaPropExport, informacionOntologias, facDSPresentacion, documentoWeb, pProyectoID, gestorFacetas, out predicadoProp);

                            if (filaPropExport.DatosExtraPropiedad != null && !string.IsNullOrEmpty(filaPropExport.DatosExtraPropiedad) && filaPropExport.DatosExtraPropiedad.Contains(TipoDatosExtraPropiedadExportacion.SepararValoresPorFila))
                            {
                                if (!string.IsNullOrEmpty(valorProp))
                                {
                                    List<string> listaValoresSinAgrupar = new List<string>();
                                    string[] delimiter = { "," };
                                    foreach (string valor in valorProp.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        listaValoresSinAgrupar.Add(valor.Trim());
                                    }

                                    if (predicadoProp.EndsWith("|"))
                                    {
                                        predicadoProp = predicadoProp.Substring(0, predicadoProp.Length - 1);
                                    }

                                    string clave = SEPARADOR_MULTIVALOR + predicadoProp + SEPARADOR_MULTIVALOR;
                                    dicCSVfiles.Add(clave, listaValoresSinAgrupar);

                                    // Añadimos siempre al csv para procesar despues sacando las relaciones
                                    datosExtraPropiedad_FormatoCSV.Append(TratarTextoParaCSV(clave));
                                }
                                datosExtraPropiedad_FormatoCSV.Append(";");
                            }
                            else
                            {
                                fila.Add(TratarTextoParaCSV(valorProp));

                            }
                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Tipo)
                        {
                            string tipoDocumento = "";

                            if (documentoWeb.TipoDocumentacion == TiposDocumentacion.Semantico)
                            {
                                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                                DataWrapperDocumentacion docDW = docCN.ObtenerOntologiasDeDocumentos(new List<Guid> { documentoWeb.Clave });
                                if (docDW.ListaDocumento.FirstOrDefault() != null)
                                {
                                    tipoDocumento = docDW.ListaDocumento.FirstOrDefault().Titulo;
                                }
                            }
                            else
                            {
                                tipoDocumento = MontarCSVObtenerTipoDocumento(documentoWeb.TipoDocumentacion, documentoWeb.Extension);
                            }

                            fila.Add(TratarTextoParaCSV(tipoDocumento));

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Titulo)
                        {
                            #region Titulo del documento
                            fila.Add(TratarTextoParaCSV(UtilCadenas.ObtenerTextoDeIdioma(documentoWeb.Titulo, IdiomaUsuario, null)));
                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.FechaPublicacion)
                        {
                            #region Fecha
                            if (documentoWeb.Compartido)
                            {
                                fila.Add(TratarTextoParaCSV(documentoWeb.Fecha.ToString("dd.MM.yy")));
                            }
                            else
                            {
                                fila.Add(TratarTextoParaCSV(documentoWeb.CompartidoIdentidaFechaEnProyectoActual(pProyectoID).Value.ToString("dd.MM.yy")));
                            }
                            #endregion
                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Publicador)
                        {
                            #region Publicador del documento 

                            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> listaDocumentoWebVinRecursos = documentoWeb.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.Eliminado == false && !doc.TipoPublicacion.Equals((short)TipoPublicacion.Publicado) && doc.DocumentoID.Equals(documentoWeb.Clave) && doc.BaseRecursosID.Equals(mControladorBase.BaseRecursosProyectoSeleccionado)).ToList();

                            if (listaDocumentoWebVinRecursos.Count > 0)
                            {
                                List<Guid> publicadoresID = new List<Guid>();

                                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocBR in listaDocumentoWebVinRecursos)
                                {
                                    if (filaDocBR.IdentidadPublicacionID.HasValue && !publicadoresID.Contains(filaDocBR.IdentidadPublicacionID.Value))
                                    {
                                        publicadoresID.Add(filaDocBR.IdentidadPublicacionID.Value);
                                    }
                                }
                                List<string> publicadores = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory).ObtenerNombresDeIdentidades(publicadoresID).Values.ToList();
                                if (filaPropExport.DatosExtraPropiedad != null && !string.IsNullOrEmpty(filaPropExport.DatosExtraPropiedad) && filaPropExport.DatosExtraPropiedad.Contains(TipoDatosExtraPropiedadExportacion.SepararValoresPorFila))
                                {
                                    string clave = SEPARADOR_MULTIVALOR + "ClavePublicadoresCompartidoresDocumento" + SEPARADOR_MULTIVALOR;
                                    dicCSVfiles.Add(clave, publicadores);
                                    fila.Add(TratarTextoParaCSV(clave));
                                }
                                else
                                {
                                    string usuario = "";

                                    for (int ii = 0; ii < publicadores.Count; ii++)
                                    {
                                        usuario += publicadores[ii] + ", ";
                                    }
                                    fila.Add(TratarTextoParaCSV(usuario.Substring(0, usuario.Length - 2)));
                                }
                            }
                            else
                            {
                                string creador = "";
                                if (documentoWeb.FilaDocumentoWebVinBR != null && documentoWeb.FilaDocumentoWebVinBR.IdentidadPublicacionID.HasValue)
                                {
                                    creador = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory).ObtenerNombreDeIdentidad(documentoWeb.FilaDocumentoWebVinBR.IdentidadPublicacionID.Value);
                                }

                                fila.Add(TratarTextoParaCSV(creador));
                            }
                            #endregion
                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Publicadores)
                        {
                            #region Publicador del documento y personas que lo compartieron en la comunidad.

                            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> listaDocumentoWebVinRecursos = documentoWeb.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.Eliminado == false && !doc.TipoPublicacion.Equals((short)TipoPublicacion.Publicado) && doc.DocumentoID.Equals(documentoWeb.Clave)).ToList();
                            if (listaDocumentoWebVinRecursos.Count > 0)
                            {
                                List<Guid> publicadoresID = new List<Guid>();

                                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocBR in listaDocumentoWebVinRecursos)
                                {
                                    if (!(filaDocBR.IdentidadPublicacionID.HasValue) && !publicadoresID.Contains(filaDocBR.IdentidadPublicacionID.Value))
                                    {
                                        publicadoresID.Add(filaDocBR.IdentidadPublicacionID.Value);
                                    }
                                }
                                List<string> publicadores = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory).ObtenerNombresDeIdentidades(publicadoresID).Values.ToList();

                                if (filaPropExport.DatosExtraPropiedad != null && !string.IsNullOrEmpty(filaPropExport.DatosExtraPropiedad) && filaPropExport.DatosExtraPropiedad.Contains(TipoDatosExtraPropiedadExportacion.SepararValoresPorFila))
                                {
                                    string clave = SEPARADOR_MULTIVALOR + "ClavePublicadoresCompartidoresDocumento" + SEPARADOR_MULTIVALOR;
                                    dicCSVfiles.Add(clave, publicadores);
                                    fila.Add(TratarTextoParaCSV(clave));
                                }
                                else
                                {
                                    string usuario = "";

                                    for (int ii = 0; ii < publicadores.Count; ii++)
                                    {
                                        usuario += publicadores[ii] + ", ";
                                    }
                                    fila.Add(TratarTextoParaCSV(usuario.Substring(0, usuario.Length - 2)));
                                }
                            }
                            else
                            {
                                string creador = "";
                                if (documentoWeb.FilaDocumentoWebVinBR != null && documentoWeb.FilaDocumentoWebVinBR.IdentidadPublicacionID.HasValue)
                                {
                                    creador = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory).ObtenerNombreDeIdentidad(documentoWeb.FilaDocumentoWebVinBR.IdentidadPublicacionID.Value);
                                }

                                fila.Add(TratarTextoParaCSV(creador));
                            }
                            #endregion
                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Descripcion)
                        {
                            #region Descripcion del documento
                            fila.Add(TratarTextoParaCSV(UtilCadenas.ObtenerTextoDeIdioma(documentoWeb.Descripcion, IdiomaUsuario, null)));
                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.NumeroVotos)
                        {
                            #region NumeroVotos del documento
                            VotoCN votoCN = new VotoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
                            DataWrapperVoto votoDS = new DataWrapperVoto();
                            string votos = votoCN.ObtenerNumeroVotos(documentoWeb.Clave, pProyectoID);
                            fila.Add(votos.ToString());

                            #endregion
                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.NumVisitas)
                        {
                            #region Numero de Visitas
                            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                            string numeroVisitas = documentacionCN.ObtenerNumeroVisitas(documentoWeb.Clave, mControladorBase.BaseRecursosProyectoSeleccionado);

                            fila.Add(numeroVisitas);
                            #endregion
                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.NumeroConsultas)
                        {
                            #region NumeroConsultas del documento
                            fila.Add(documentoWeb.FilaDocumento.NumeroTotalConsultas.ToString());
                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.NumeroDescargas)
                        {
                            #region NumeroDescargas del documento
                            fila.Add(documentoWeb.FilaDocumento.NumeroTotalDescargas.ToString());
                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Categorias)
                        {

                            #region Categorias del documento
                            StringBuilder nomCategorias = new StringBuilder();
                            foreach (Guid cateogiraID in documentoWeb.Categorias.Keys)
                            {
                                nomCategorias.Append(documentoWeb.Categorias[cateogiraID].Nombre[UtilIdiomas.LanguageCode]);
                                nomCategorias.Append(", ");
                            }

                            string finalCategorias = nomCategorias.ToString();
                            if (finalCategorias.EndsWith(", "))
                            {
                                finalCategorias = finalCategorias.Substring(0, finalCategorias.LastIndexOf(", "));
                            }

                            fila.Add(finalCategorias);
                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.CategoriaFiltrada)
                        {

                            #region Categorias del documento
                            StringBuilder nomCategorias = new StringBuilder();
                            if (filaPropExport.DatosExtraPropiedad != null)
                            {
                                string[] delimiter = { "=" };
                                List<string> lista = filaPropExport.DatosExtraPropiedad.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (lista.Count == 2)
                                {
                                    if (lista[0].ToLower().Equals(TipoDatosExtraPropiedadExportacion.FiltroCategoria.ToLower()))
                                    {
                                        Guid categoriaFiltradaID = Guid.Empty;
                                        if (Guid.TryParse(lista[1], out categoriaFiltradaID))
                                        {
                                            if (!categoriaFiltradaID.Equals(Guid.Empty))
                                            {
                                                foreach (Guid cateogiraID in documentoWeb.Categorias.Keys)
                                                {
                                                    if (GestorTesauroProyectoActual.ListaCategoriasTesauro[categoriaFiltradaID].Hijos.Contains(GestorTesauroProyectoActual.ListaCategoriasTesauro[cateogiraID]))
                                                    {
                                                        nomCategorias.Append(documentoWeb.Categorias[cateogiraID].Nombre[UtilIdiomas.LanguageCode]);
                                                        nomCategorias.Append(", ");
                                                    }
                                                }

                                                string finalCategorias = nomCategorias.ToString();
                                                if (finalCategorias.EndsWith(", "))
                                                {
                                                    finalCategorias = finalCategorias.Substring(0, finalCategorias.LastIndexOf(", "));
                                                }

                                                fila.Add(finalCategorias);
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Etiquetas)
                        {

                            #region Etiquetas del documento
                            fila.Add(documentoWeb.Tags);
                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Url)
                        {

                            #region Url del documento
                            fila.Add(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, documentoWeb, false));
                            #endregion

                        }
                        //Editores DocumentoRolIdentidad nos devuelve los perfiles, y buscar los nombres y apellidos.
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Editores)
                        {
                            #region Editores del documento
                            if (documentoWeb.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Any(doc => doc.DocumentoID.Equals(documentoWeb.Clave)))
                            {
                                //Puede haber mas de una persona que haya compartdo el recurso, recorremos el array por cada persona que lo haya compartido y los mostramos todos en el csv.

                                List<Guid> editoresID = new List<Guid>();

                                foreach (Guid perfilID in documentoWeb.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(documentoWeb.Clave)).Select(x => x.PerfilID))
                                {
                                    if (!editoresID.Contains(perfilID))
                                    {
                                        editoresID.Add(perfilID);
                                    }
                                }
                                List<string> editoresConRepetidos = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory).ObtenerNombresDePerfilesUOrganizacion(editoresID).Values.ToList();

                                List<string> editores = new List<string>();
                                foreach (string editor in editoresConRepetidos)
                                {
                                    if (!editores.Contains(editor))
                                    {
                                        editores.Add(editor);
                                    }

                                }
                                if (filaPropExport.DatosExtraPropiedad != null && !string.IsNullOrEmpty(filaPropExport.DatosExtraPropiedad) && filaPropExport.DatosExtraPropiedad.Contains(TipoDatosExtraPropiedadExportacion.SepararValoresPorFila))
                                {
                                    string clave = SEPARADOR_MULTIVALOR + "ClavePublicadoresCompartidoresDocumento" + SEPARADOR_MULTIVALOR;
                                    dicCSVfiles.Add(clave, editores);

                                    fila.Add(TratarTextoParaCSV(clave));
                                }
                                else
                                {
                                    string usuario = "";

                                    for (int ii = 0; ii < editores.Count; ii++)
                                    {
                                        usuario += editores[ii] + ", ";
                                    }

                                    fila.Add(TratarTextoParaCSV(usuario.Substring(0, usuario.Length - 2)));
                                }
                            }
                            else
                            {

                                string creador = "";
                                if (documentoWeb.FilaDocumentoWebVinBR != null && documentoWeb.FilaDocumentoWebVinBR.IdentidadPublicacionID.HasValue)
                                {
                                    creador = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory).ObtenerNombreDeIdentidad(documentoWeb.FilaDocumentoWebVinBR.IdentidadPublicacionID.Value);
                                }

                                fila.Add(TratarTextoParaCSV(creador));


                            }
                            #endregion


                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.NumComentarios)
                        {

                            #region Numero de Comentarios
                            ComentarioCN comentarioCN = new ComentarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ComentarioCN>(), mLoggerFactory);
                            DataWrapperComentario comentarioDW = new DataWrapperComentario();
                            int comentarios = comentarioCN.ObtenerComentariosDeDocumento(comentarioDW, documentoWeb.Clave, ProyectoSeleccionado.Clave, 1, 100, true);
                            fila.Add(comentarios.ToString());
                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Comentadores)
                        {

                            #region Comentadores del documento

                            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                            List<string> listaComentadores = documentacionCN.ObtenerComentadores(documentoWeb.Clave, ProyectoSeleccionado.Clave);

                            if (listaComentadores.Count > 0)
                            {
                                string usuario = "";

                                for (int ii = 0; ii < listaComentadores.Count; ii++)
                                {
                                    usuario += listaComentadores[ii] + ", ";
                                }
                                fila.Add(TratarTextoParaCSV(usuario.Substring(0, usuario.Length - 2)));
                            }
                            else
                            {
                                fila.Add("");
                            }
                            #endregion

                        }
                        else if (filaPropExport.Propiedad == TipoPropExportacion.Calificacion)
                        {
                            #region Editores del documento
                            if (documentoWeb.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Any(doc => doc.DocumentoID.Equals(documentoWeb.Clave)))
                            {
                                Guid nivelCertificacionDoc = new Guid();

                                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocBR in documentoWeb.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(documentoWeb.Clave)).ToList())
                                {
                                    if (filaDocBR.NivelCertificacionID.HasValue)
                                    {
                                        nivelCertificacionDoc = filaDocBR.NivelCertificacionID.Value;
                                    }
                                }


                                List<string> certificacion = new List<string>();

                                foreach (NivelCertificacion nivelCertificacionRow in ProyectoDW.ListaNivelCertificacion.Where(doc => doc.NivelCertificacionID.Equals(nivelCertificacionDoc)).ToList())
                                {
                                    AdministrarComunidadUtilidades.NivelCertificacion nivelCertificacion = new AdministrarComunidadUtilidades.NivelCertificacion();
                                    nivelCertificacion.CertificacionID = nivelCertificacionRow.NivelCertificacionID;
                                    nivelCertificacion.Nombre = nivelCertificacionRow.Descripcion;
                                    nivelCertificacion.Orden = nivelCertificacionRow.Orden;

                                    certificacion.Add(nivelCertificacion.Nombre);
                                }

                                if (certificacion.Count > 0)
                                {
                                    string usuario = "";

                                    for (int ii = 0; ii < certificacion.Count; ii++)
                                    {
                                        usuario += certificacion[ii] + ", ";
                                    }
                                    fila.Add(TratarTextoParaCSV(usuario.Substring(0, usuario.Length - 2)));
                                }
                                else
                                {
                                    fila.Add("");
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            fila.Add("");
                        }

                        string cadenaBusqueda = TipoDatosExtraPropiedadExportacion.PropiedadTransitiva + "='";
                        if (filaPropExport.DatosExtraPropiedad != null && filaPropExport.DatosExtraPropiedad.Contains(TipoDatosExtraPropiedadExportacion.PropiedadTransitiva) && filaPropExport.DatosExtraPropiedad.Contains(cadenaBusqueda))
                        {
                            propiedadTransitiva = filaPropExport.DatosExtraPropiedad.Substring(filaPropExport.DatosExtraPropiedad.IndexOf(cadenaBusqueda) + cadenaBusqueda.Length);
                            propiedadTransitiva = propiedadTransitiva.Substring(0, propiedadTransitiva.IndexOf("'"));
                        }

                        if (filaPropExport.DatosExtraPropiedad != null && filaPropExport.DatosExtraPropiedad.Contains(TipoDatosExtraPropiedadExportacion.UltimoValorRequerido))
                        {
                            ultimoValorRequerido = true;
                        }
                    }

                    if (dicCSVfiles.Keys.Count == 0)
                    {
                        dsExportacion.Tables["Exportacion"].Rows.Add(fila.ToArray());
                    }

                    if (dicCSVfiles.Keys.Count > 0)
                    {
                        List<string> listaFilasCSVDefinitivas = ObtenerFilasCSVRemplazandoValores(dicCSVfiles, facDSPresentacion, documentoWeb.Clave, datosExtraPropiedad_FormatoCSV.ToString(), propiedadTransitiva, ultimoValorRequerido);

                        foreach (string filaCSVDefinitiva in listaFilasCSVDefinitivas)
                        {
                            dsExportacion.Tables["Exportacion"].Rows.Add(filaCSVDefinitiva.Substring(0, filaCSVDefinitiva.Length - 1).Split(';'));
                        }
                    }
                }

                #endregion

                #region Personas y Organizaciones
                if (elemento is Identidad)
                {
                    Identidad identidad = (Identidad)elemento;

                    foreach (ProyectoPestanyaBusquedaExportacionPropiedad filaPropExport in pExporBusDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Where(item => item.ExportacionID.Equals(pExportacionID)).OrderBy(item => item.Orden))
                    {
                        fila.Add(ObtenerDatosPersonasYOrganizacionesPorPropiedad(filaPropExport.Propiedad.ToLower(), identidad, dicContadoresIdentidad, paisDW, paisCL, usuarioCN, dataWrapperProyectoDatosExtra, identDWDatosExtra));
                    }

                    if (dicCSVfiles.Keys.Count == 0)
                    {
                        dsExportacion.Tables["Exportacion"].Rows.Add(fila.ToArray());
                    }

                    if (dicCSVfiles.Keys.Count > 0)
                    {
                        List<string> listaFilasCSVDefinitivas = ObtenerFilasCSVRemplazandoValores(dicCSVfiles, null, identidad.Clave, datosExtraPropiedad_FormatoCSV.ToString(), string.Empty, false);

                        foreach (string filaCSVDefinitiva in listaFilasCSVDefinitivas)
                        {
                            dsExportacion.Tables["Exportacion"].Rows.Add(filaCSVDefinitiva.Substring(0, filaCSVDefinitiva.Length - 1).Split(';'));
                        }
                    }
                }
                #endregion
            }

            usuarioCN.Dispose();
            paisCL.Dispose();

            #endregion

            return dsExportacion;
        }

        private string ObtenerDatosPersonasYOrganizacionesPorPropiedad(string pPropiedad, Identidad identidad, Dictionary<Guid, Dictionary<string, int>> pDicContadoresIdentidad, DataWrapperPais pPaisDW, PaisCL pPaisCL, UsuarioCN pUsuCN, DataWrapperProyecto pDataWrapperProyectoDatosExtra, DataWrapperIdentidad pIdentDWDatosExtra)
        {
            if (pPropiedad.Equals(TipoPropExportacionPersonas.FechaAlta))
            {
                return UtilCadenas.ObtenerTextoDeIdioma(identidad.FilaIdentidad.FechaAlta.ToString(), IdiomaUsuario, null);
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.Nombre))
            {
                return UtilCadenas.ObtenerTextoDeIdioma(identidad.Persona.Nombre, IdiomaUsuario, null);
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.Apellidos))
            {
                return UtilCadenas.ObtenerTextoDeIdioma(identidad.Persona.Apellidos, IdiomaUsuario, null);
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.Email))
            {
                return UtilCadenas.ObtenerTextoDeIdioma(identidad.Persona.Mail, IdiomaUsuario, null);
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.FechaNacimiento))
            {
                return UtilCadenas.ObtenerTextoDeIdioma(identidad.Persona.Fecha.ToShortDateString(), IdiomaUsuario, null);
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.NumeroAccesos))
            {
                int numAccesos = identidad.FilaIdentidad.NumConnexiones;
                return numAccesos.ToString();
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.NumeroVisitas))
            {
                if (pDicContadoresIdentidad != null && pDicContadoresIdentidad.Count > 0 && pDicContadoresIdentidad.ContainsKey(identidad.Clave))
                {
                    return pDicContadoresIdentidad[identidad.Clave][IdentidadAD.CONTADOR_NUMERO_VISITAS].ToString();
                }
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.NumeroDescargas))
            {
                if (pDicContadoresIdentidad != null && pDicContadoresIdentidad.Count > 0 && pDicContadoresIdentidad.ContainsKey(identidad.Clave))
                {
                    return pDicContadoresIdentidad[identidad.Clave][IdentidadAD.CONTADOR_NUMERO_DESCARGAS].ToString();
                }
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.Pais) && identidad.Persona.PaisID != Guid.Empty)
            {
                if (pPaisDW == null)
                {
                    pPaisDW = pPaisCL.ObtenerPaisesProvincias();
                }

                return pPaisDW.ListaPais.Where(item => item.PaisID.Equals(identidad.Persona.PaisID)).FirstOrDefault().Nombre;
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.Provincia))
            {
                return identidad.Persona.Provincia;
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.Localidad))
            {
                return identidad.Persona.Localidad;
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.RedesSociales))
            {
                if (identidad.GestorIdentidades.GestorUsuarios.ListaUsuarios.ContainsKey(identidad.Persona.UsuarioID))
                {
                    string redSocial = "";
                    List<AD.EntityModel.Models.UsuarioDS.UsuarioVinculadoLoginRedesSociales> filasUsuRedesSociales = identidad.GestorIdentidades.GestorUsuarios.ListaUsuarios[identidad.Persona.UsuarioID].FilaUsuario.UsuarioVinculadoLoginRedesSociales.ToList();

                    if (filasUsuRedesSociales == null || !filasUsuRedesSociales.Any())
                    {
                        redSocial = "Gnoss";
                    }
                    else
                    {
                        string coma = "";
                        foreach (AD.EntityModel.Models.UsuarioDS.UsuarioVinculadoLoginRedesSociales filaRedSocial in filasUsuRedesSociales)
                        {
                            redSocial += coma + (TipoRedSocialLogin)filaRedSocial.TipoRedSocial;
                            coma = ",";
                        }
                    }

                    return redSocial;
                }
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.NumRecursosPublicados))
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                int numRecursos = docCN.ObtenerNumeroRecursosPublicados(identidad.FilaIdentidad.IdentidadID);
                return numRecursos.ToString();
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.NumRecursosComentados))
            {
                ComentarioCN comentarioCN = new ComentarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ComentarioCN>(), mLoggerFactory);
                DataWrapperComentario comentarioDW = comentarioCN.ObtenerTodosComentarios();

                int contadorComentarios = 0;
                contadorComentarios = comentarioDW.ListaComentario.Where(com => com.IdentidadID.Equals(identidad.FilaIdentidad.IdentidadID)).Count();
                return contadorComentarios.ToString();
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.FechaUltimoAcceso))
            {
                DateTime? fechaUltimoAcceso = pUsuCN.ObtenerFechaUltimoAccesoDeUsuario(identidad.Persona.UsuarioID);
                string fecha = "";
                if (fechaUltimoAcceso.HasValue)
                {
                    fecha = fechaUltimoAcceso.Value.ToString();
                }
                return fecha;
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.Rol))
            {
                string rol = "USUARIO";
                if (!identidad.Tipo.Equals(TiposIdentidad.Organizacion) && identidad.PersonaID.HasValue)
                {
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    Guid usuarioID = identidadCN.ObtenerUsuarioIDConIdentidadID(identidad.Clave);
                    var filaAdmin = ProyectoSeleccionado.FilaProyecto.AdministradorProyecto.FirstOrDefault(f => f.UsuarioID.Equals(usuarioID));
                    if (filaAdmin != null)
                    {
                        if (((UserRol)filaAdmin.Tipo).Equals(UserRol.Administrator))
                        {
                            rol = "ADMINISTRADOR";
                        }
                        else if (((UserRol)filaAdmin.Tipo).Equals(UserRol.Supervisor))
                        {
                            rol = "SUPERVISOR";
                        }
                    }
                }
                return UtilIdiomas.GetText("COMADMIN", rol);
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.EstaSuscritoBoletin))
            {
                return identidad.FilaIdentidad.RecibirNewsLetter ? UtilIdiomas.GetText("COMMON", "SI") : UtilIdiomas.GetText("COMMON", "NO");
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.EstaExpulsado))
            {
                return identidad.FilaIdentidad.FechaExpulsion.HasValue ? UtilIdiomas.GetText("COMMON", "SI") : UtilIdiomas.GetText("COMMON", "NO");
            }
            else if (pPropiedad.Equals(TipoPropExportacionPersonas.EstaBloqueado))
            {
                return identidad.FilaIdentidad.FechaBaja.HasValue ? UtilIdiomas.GetText("COMMON", "SI") : UtilIdiomas.GetText("COMMON", "NO");
            }
            else
            {
                List<DatoExtraProyectoVirtuoso> filasDatosExtraProyecto = pDataWrapperProyectoDatosExtra.ListaDatoExtraProyectoVirtuoso.Where(dato => dato.NombreCampo.Equals(pPropiedad)).ToList();

                if (filasDatosExtraProyecto.Count > 0)
                {
                    Guid datoExtraID = filasDatosExtraProyecto[0].DatoExtraID;
                    List<AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad> filasDatoExtraVirIdent = pIdentDWDatosExtra.ListaDatoExtraProyectoVirtuosoIdentidad.Where(datoExtra => datoExtra.DatoExtraID.Equals(datoExtraID) && datoExtra.IdentidadID.Equals(identidad.Clave)).ToList();

                    if (filasDatoExtraVirIdent.Count > 0)
                    {
                        return UtilCadenas.ObtenerTextoDeIdioma(filasDatoExtraVirIdent.First().Opcion, IdiomaUsuario, null);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            return "";
        }

        /// <summary>
        /// Añade el valor al formato de salida indicado
        /// <param name="pValor">Dato a añadir</param>
        /// <param name="pCsvFile">CSV al que añadir el valor</param>
        /// <param name="pFila">Fila del excel donde se añade el valor</param>
        /// <param name="pFormato">Formato de salida
        ///     "CSV" - Fichero csv
        ///     "EXCEL" - Archivo de Excel
        /// </param>
        /// </summary>
        private void GuardarValorSegunFormato(string pValor, StringBuilder pCsvFile, List<string> pFila, string pFormato = "CSV")
        {
            if (pFormato.Equals(FormatosExportancion.CSV))
            {
                if (pValor.Equals(";"))
                {
                    pCsvFile.Append(pValor);
                }
                else
                {
                    pCsvFile.Append(TratarTextoParaCSV(pValor));
                }
            }
            else if (pFormato.Equals(FormatosExportancion.EXCEL))
            {
                if (!pValor.Equals(";"))
                {
                    pFila.Add(pValor);
                }
            }
        }

        private List<string> ObtenerFilasCSVRemplazandoValores(Dictionary<string, List<string>> pDicCSVFiles, FacetadoDS pFacDSPresentacion, Guid pElementoID, string pDatosExtraPropiedad_FormatoCSV, string pPropiedadTransitiva, bool pUltimoValorRequerido)
        {
            List<string> filasDefinitivas = new List<string>();

            if (pFacDSPresentacion != null)
            {
                Dictionary<string, List<string>> diccionarioRelacionValores = new Dictionary<string, List<string>>();
                if (!string.IsNullOrEmpty(pPropiedadTransitiva))
                {
                    // Agregar todos los valores de la propiedad de padres e hijos
                    // listaRelacionValores.Add
                    foreach (DataRow dr in pFacDSPresentacion.Tables["SelectPropEnt"].Select("p like '%" + pPropiedadTransitiva + "%' AND s like '%" + pElementoID + "%'"))
                    {
                        string objeto = dr[2].ToString();
                        string relacion = dr[3].ToString();

                        if (relacion.Contains("@@@"))
                        {
                            relacion = relacion.Substring(0, relacion.LastIndexOf("@@@"));
                        }

                        if (diccionarioRelacionValores.ContainsKey(relacion))
                        {
                            diccionarioRelacionValores[relacion].Add(objeto);
                        }
                        else
                        {
                            diccionarioRelacionValores.Add(relacion, new List<string>());
                            diccionarioRelacionValores[relacion].Add(objeto);
                        }
                    }
                }

                // Recorrer el objeto y completar las filas del CSV
                foreach (DataRow dr in pFacDSPresentacion.Tables["SelectPropEnt"].Select("relacion like '%" + pElementoID + "%'"))
                {
                    string predicado = dr[1].ToString();
                    string objeto = dr[2].ToString();
                    string relacion = dr[3].ToString();

                    if (!relacion.Contains("@"))
                    {
                        string cadenaTemporal = pDatosExtraPropiedad_FormatoCSV.Replace(SEPARADOR_MULTIVALOR + predicado + SEPARADOR_MULTIVALOR, objeto);

                        // Llamada recursiva para remplazar todos los casos de herencia
                        RemplazarValoresFila_Recursivo(pFacDSPresentacion, predicado, objeto, relacion, cadenaTemporal, pPropiedadTransitiva, diccionarioRelacionValores, pUltimoValorRequerido, ref filasDefinitivas);
                    }
                }
            }
            else
            {
                Dictionary<int, List<string>> dicIteracionFilas = new Dictionary<int, List<string>>();
                for (int i = 0; i < pDicCSVFiles.Keys.Count; i++)
                {
                    dicIteracionFilas.Add(i, new List<string>());

                    if (i == 0)
                    {
                        // Primera iteracción, se añaden los valores al diccionario.
                        foreach (string clave in pDicCSVFiles.Keys)
                        {
                            foreach (string valor in pDicCSVFiles[clave])
                            {
                                dicIteracionFilas[i].Add(pDatosExtraPropiedad_FormatoCSV.ToString().Replace(clave, valor));
                            }
                        }
                    }
                    else
                    {
                        // Iteracciones posteriores, se reprocesan las filas anteriores y se agregan al diccionario de la iteracción actual.
                        foreach (string cadena in dicIteracionFilas[i - 1])
                        {
                            string cadenaTemporal = cadena;
                            foreach (string clave in pDicCSVFiles.Keys)
                            {
                                foreach (string valor in pDicCSVFiles[clave])
                                {
                                    cadenaTemporal = cadenaTemporal.ToString().Replace(clave, valor);
                                }
                            }

                            if (!dicIteracionFilas[i].Contains(cadenaTemporal))
                            {
                                dicIteracionFilas[i].Add(cadenaTemporal);
                            }
                        }
                    }
                }

                foreach (string filaDefinitiva in dicIteracionFilas[dicIteracionFilas.Keys.Count - 1])
                {
                    if (!filasDefinitivas.Contains(filaDefinitiva))
                    {
                        string temporalFilaDefinitiva = filaDefinitiva;
                        if (temporalFilaDefinitiva.Contains(SEPARADOR_MULTIVALOR))
                        {
                            string[] delimiter = { SEPARADOR_MULTIVALOR };
                            string[] trozos = temporalFilaDefinitiva.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);


                            temporalFilaDefinitiva = string.Empty;
                            for (int i = 0; i < trozos.Length; i++)
                            {
                                if (i % 2 == 1)
                                {
                                    temporalFilaDefinitiva += trozos[i];
                                }
                            }
                        }

                        filasDefinitivas.Add(temporalFilaDefinitiva);
                    }
                }
            }

            return filasDefinitivas;
        }

        private void RemplazarValoresFila_Recursivo(FacetadoDS pFacDSPresentacion, string pPredicado, string pObjeto, string pRelacion, string pCadenaTemporal, string pPropiedadTransitiva, Dictionary<string, List<string>> pDicRelacionValoresTransitiva, bool pUltimoValorRequerido, ref List<string> pListaCadenas)
        {
            DataRow[] filas = pFacDSPresentacion.Tables["SelectPropEnt"].Select("relacion like '%" + pRelacion + "%' AND relacion <> '" + pRelacion + "'", "level ASC");
            if (filas.Length > 0)
            {
                foreach (DataRow dr in filas)
                {
                    string sujeto = dr[0].ToString();
                    string predicado = dr[1].ToString();
                    string objeto = dr[2].ToString();
                    string relacion = dr[3].ToString();

                    // Quitamos las relaciones que empiezan con la relación pRelación pero que pertencen a hijos de hijos...
                    // OK: Padre    Hijo    SubHijo
                    // KO: Padre    ""  SubHijo
                    if (!relacion.Replace(pRelacion + "@@@", "").Contains("@@@"))
                    {
                        string cadenaTemporal = pCadenaTemporal.Replace(SEPARADOR_MULTIVALOR + predicado + SEPARADOR_MULTIVALOR, objeto);

                        RemplazarValoresFila_Recursivo(pFacDSPresentacion, predicado, objeto, relacion, cadenaTemporal, pPropiedadTransitiva, pDicRelacionValoresTransitiva, pUltimoValorRequerido, ref pListaCadenas);
                    }
                }
            }
            else
            {
                string filaLimpia = string.Empty;
                int posicionPropiedadTransitiva = ObtenerPosicionCadenaPropiedadTransitiva(pCadenaTemporal, pPropiedadTransitiva);

                if (!pCadenaTemporal.Contains(SEPARADOR_MULTIVALOR + pPredicado + SEPARADOR_MULTIVALOR) && pCadenaTemporal.Contains(pPredicado))
                {
                    // Eliminar lo que haya entre @|@ .* pPredicado .* @|@

                    string inicio = pCadenaTemporal.Substring(0, pCadenaTemporal.IndexOf(pPredicado));
                    string fin = pCadenaTemporal.Substring(pCadenaTemporal.IndexOf(pPredicado) + pPredicado.Length);

                    // Separar la cadena por el pPredicado
                    inicio = inicio.Substring(0, inicio.LastIndexOf(SEPARADOR_MULTIVALOR));
                    fin = fin.Substring(fin.IndexOf(SEPARADOR_MULTIVALOR) + SEPARADOR_MULTIVALOR.Length);

                    string filaTemporalValoresRemplazo = inicio + pObjeto + fin;
                    filaLimpia = EliminarSeparadoresMultivalor(filaTemporalValoresRemplazo);
                    if (!pListaCadenas.Contains(filaLimpia))
                    {
                        if (pUltimoValorRequerido && !filaLimpia.EndsWith(";;"))
                        {
                            pListaCadenas.Add(filaLimpia);
                        }
                        else if (!filaLimpia.EndsWith(";;"))
                        {
                            pListaCadenas.Add(filaLimpia);
                        }
                    }
                }
                else if (pCadenaTemporal.Contains(SEPARADOR_MULTIVALOR + pPredicado + SEPARADOR_MULTIVALOR))
                {
                    filaLimpia = pCadenaTemporal.Replace(SEPARADOR_MULTIVALOR + pPredicado + SEPARADOR_MULTIVALOR, pObjeto);
                    if (!pListaCadenas.Contains(filaLimpia))
                    {
                        if (pUltimoValorRequerido && !filaLimpia.EndsWith(";;"))
                        {
                            pListaCadenas.Add(filaLimpia);
                        }
                        else if (!filaLimpia.EndsWith(";;"))
                        {
                            pListaCadenas.Add(filaLimpia);
                        }
                    }
                }
                else if (pCadenaTemporal.Contains(pObjeto))
                {
                    filaLimpia = EliminarSeparadoresMultivalor(pCadenaTemporal);
                    if (!pListaCadenas.Contains(filaLimpia))
                    {
                        if (pUltimoValorRequerido && !filaLimpia.EndsWith(";;"))
                        {
                            pListaCadenas.Add(filaLimpia);
                        }
                        else if (!filaLimpia.EndsWith(";;"))
                        {
                            pListaCadenas.Add(filaLimpia);
                        }
                    }
                }

                while (pRelacion.Contains("@@@"))
                {
                    AgregarValoresTransitivosLista(filaLimpia, pDicRelacionValoresTransitiva, pRelacion, pCadenaTemporal, posicionPropiedadTransitiva, pListaCadenas, pUltimoValorRequerido);
                    pRelacion = pRelacion.Substring(0, pRelacion.LastIndexOf("@@@"));
                }

                AgregarValoresTransitivosLista(filaLimpia, pDicRelacionValoresTransitiva, pRelacion, pCadenaTemporal, posicionPropiedadTransitiva, pListaCadenas, pUltimoValorRequerido);
            }
        }

        private int ObtenerPosicionCadenaPropiedadTransitiva(string pCadenaTemporal, string pPropiedadTransitiva)
        {
            int posicionPropiedadTransitiva = -1;
            if (pCadenaTemporal.Contains(pPropiedadTransitiva))
            {
                string[] delimiter = { ";" };
                string[] trozos = pCadenaTemporal.Split(delimiter, StringSplitOptions.None);
                for (int i = 0; i < trozos.Length; i++)
                {
                    if (trozos[i].Contains(pPropiedadTransitiva))
                    {
                        posicionPropiedadTransitiva = i;
                    }
                }
            }

            return posicionPropiedadTransitiva;
        }

        private void AgregarValoresTransitivosLista(string pFilaLimpia, Dictionary<string, List<string>> pDicRelacionValoresTransitiva, string pRelacion, string pCadenaTemporal, int pPosicionPropiedadTransitiva, List<string> pListaCadenas, bool pUltimoValorRequerido)
        {
            if (!string.IsNullOrEmpty(pFilaLimpia) && pDicRelacionValoresTransitiva.ContainsKey(pRelacion))
            {
                foreach (string objetoTransitivo in pDicRelacionValoresTransitiva[pRelacion])
                {
                    string filaValoresTransitivos = string.Empty;
                    string[] delimiter = { ";" };
                    string[] trozos = pCadenaTemporal.Split(delimiter, StringSplitOptions.None);
                    for (int i = 0; i < trozos.Length - 1; i++)
                    {
                        if (i == pPosicionPropiedadTransitiva)
                        {
                            filaValoresTransitivos += objetoTransitivo + ";";
                        }
                        else
                        {
                            filaValoresTransitivos += trozos[i] + ";";
                        }
                    }

                    filaValoresTransitivos = EliminarSeparadoresMultivalor(filaValoresTransitivos);

                    if (!pListaCadenas.Contains(filaValoresTransitivos))
                    {
                        if (pUltimoValorRequerido && !filaValoresTransitivos.EndsWith(";;"))
                        {
                            pListaCadenas.Add(filaValoresTransitivos);
                        }
                        else if (!filaValoresTransitivos.EndsWith(";;"))
                        {
                            pListaCadenas.Add(filaValoresTransitivos);
                        }
                    }
                }
            }
        }

        private string EliminarSeparadoresMultivalor(string pFilaConSeparadores)
        {
            if (pFilaConSeparadores.Contains(SEPARADOR_MULTIVALOR))
            {
                string[] delimiter = { SEPARADOR_MULTIVALOR };
                string[] trozos = pFilaConSeparadores.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);


                pFilaConSeparadores = string.Empty;
                for (int i = 0; i < trozos.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        pFilaConSeparadores += trozos[i];
                    }
                }
            }

            return pFilaConSeparadores;
        }

        private string MontarCSVObtenerTipoDocumento(TiposDocumentacion pDocumentoWebTipo, string pDocumentoWebExtension)
        {
            string tipoDocumento = string.Empty;
            if (pDocumentoWebTipo == TiposDocumentacion.ReferenciaADoc)
            {
                tipoDocumento = "---";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.Nota)
            {
                tipoDocumento = "NOTA";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.VideoBrightcove || pDocumentoWebTipo == TiposDocumentacion.Video)
            {
                tipoDocumento = "VIDEO";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.Debate)
            {
                tipoDocumento = "DEB";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.Pregunta)
            {
                tipoDocumento = "PREG";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.Newsletter)
            {
                tipoDocumento = "NEWS";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.Hipervinculo)
            {
                tipoDocumento = "LINK";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.EntradaBlog)
            {
                tipoDocumento = "BLOG";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.Wiki)
            {
                tipoDocumento = "WIKI";
            }
            else if (pDocumentoWebTipo == TiposDocumentacion.Encuesta)
            {
                tipoDocumento = "Encuesta";
            }
            else
            {
                if (!string.IsNullOrEmpty(pDocumentoWebExtension))
                {
                    tipoDocumento = pDocumentoWebExtension.Substring(1).ToUpper();
                }
                else
                {
                    tipoDocumento = "---";
                }
            }

            return tipoDocumento;
        }

        /// <summary>
        /// Pinta las propiedades semánticas de un tipo de vista.
        /// </summary>
        /// <param name="pDiv">Div del tipo de vista</param>
        /// <param name="pPropPintar">Propiedad a pintar</param>
        /// <param name="pPropiedades">Lista de propiedades</param>
        public string PintarPropiedadesSemDeVista(ProyectoPestanyaBusquedaExportacionPropiedad pFilaPropExport, Dictionary<string, List<string>> pInformacionOntologias, FacetadoDS pFacDSPresentacion, DocumentoWeb pDocumento, Guid pProyectoID, GestionFacetas pGestorFacetas, out string pPredicadoProp)
        {
            string[] propiedades = pFilaPropExport.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            string objetoFinal = string.Empty;
            pPredicadoProp = string.Empty;

            int i = 0;
            foreach (string propint in propiedades)
            {
                string propiedadAux = propint;
                string idioma = "";
                if (propiedadAux.Contains("[MultiIdioma]"))
                {
                    idioma = "@" + UtilIdiomas.LanguageCode;
                    propiedadAux = propiedadAux.Replace("[MultiIdioma]", "");
                }

                FacetaMayuscula tipoMayusculas = FacetaMayuscula.Nada;

                if (pGestorFacetas.ListaFacetasPorClave.ContainsKey(propiedadAux))
                {
                    tipoMayusculas = pGestorFacetas.ListaFacetasPorClave[propiedadAux].Mayusculas;
                }

                string propiedadCadena = propiedadAux;

                List<string> prefijos = new List<string>();
                string[] delimiters = new string[] { "@@@", "RRR" };
                string[] props = propiedadAux.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string prop in props)
                {
                    if (!prop.StartsWith("http:") && prop.Contains(":"))
                    {
                        prefijos.Add(prop.Substring(0, prop.IndexOf(":")));
                    }
                }

                prefijos = prefijos.Distinct().ToList();

                Dictionary<string, List<string>> dicNombreAbreviatura = UtilServiciosFacetas.ObtenerInformacionOntologiasSinArroba(UrlIntragnoss, pInformacionOntologias);

                if (prefijos.Count > 0)
                {
                    foreach (string Nombre in dicNombreAbreviatura.Keys)
                    {
                        foreach (string ns in dicNombreAbreviatura[Nombre])
                        {
                            if (prefijos.Contains(ns))
                            {
                                propiedadCadena = propiedadCadena.Replace($"@{ns}:", $"@{Nombre}");
                                propiedadCadena = propiedadCadena.Replace($"RRR{ns}:", $"@@@{Nombre}");
                                if (propiedadCadena.StartsWith($"{ns}:"))
                                {
                                    propiedadCadena = Nombre + propiedadCadena.Substring(ns.Length + 1);
                                }
                                prefijos.Remove(ns);
                                if (prefijos.Count == 0)
                                {
                                    break;
                                }
                            }
                        }
                        if (prefijos.Count == 0)
                        {
                            break;
                        }
                    }

                    if (prefijos.Count > 0)
                    {
                        List<KeyValuePair<string, string>> prefijosGnoss = FacetadoAD.ListaNamespacesBasicos.Where(item => prefijos.Contains(item.Key)).ToList();

                        foreach (KeyValuePair<string, string> prefijo in prefijosGnoss.ToList())
                        {
                            propiedadCadena = propiedadCadena.Replace($"@{prefijo.Key}:", $"@{prefijo.Value}");
                            propiedadCadena = propiedadCadena.Replace($"RRR{prefijo.Key}:", $"@@@{prefijo.Value}");
                            if (propiedadCadena.StartsWith($"{prefijo.Key}:"))
                            {
                                propiedadCadena = $"{prefijo.Value}{propiedadCadena.Substring(prefijo.Key.Length + 1)}";
                            }
                            prefijos.Remove(prefijo.Key);

                            if (prefijos.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                    // Problemas con el orden de las cadenas, no coincide con las relaciones de virtuoso, diferente orden... =(
                    pPredicadoProp += propiedadCadena + "|";
                }

                //Si tiene esa propiedad, la pinta
                DataRow[] filasFacPresent = null;
                if (pFacDSPresentacion != null && pFacDSPresentacion.Tables["SelectPropEnt"] != null)
                {
                    filasFacPresent = pFacDSPresentacion.Tables["SelectPropEnt"].Select("p='" + propiedadCadena + "' AND s = 'http://gnoss/" + pDocumento.Clave.ToString().ToUpper() + "'");
                }

                if (filasFacPresent != null && filasFacPresent.Length > 0)
                {
                    string objeto = "";
                    foreach (DataRow fila in filasFacPresent)
                    {
                        string objetoTemp = fila["o"].ToString();

                        switch (tipoMayusculas)
                        {
                            case FacetaMayuscula.MayusculasTodasPalabras:
                                objetoTemp = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculas(objetoTemp);
                                break;
                            case FacetaMayuscula.MayusculasTodoMenosArticulos:
                                objetoTemp = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(objetoTemp);
                                break;
                            case FacetaMayuscula.MayusculasPrimeraPalabra:
                                objetoTemp = UtilCadenas.ConvertirPrimeraLetraDeFraseAMayúsculas(objetoTemp);
                                break;
                            case FacetaMayuscula.MayusculasTodasLetras:
                                objetoTemp = UtilCadenas.ConvertirAMayúsculas(objetoTemp);
                                break;
                        }

                        if (!objeto.Contains(objetoTemp))
                        {
                            objeto += DevolverStringSiesFecha(objetoTemp) + ", ";
                        }
                    }
                    if (objeto != "")
                    {
                        objeto = objeto.Substring(0, objeto.Length - 2);
                    }

                    objetoFinal += objeto + ", ";
                }

                i++;
            }

            if (!string.IsNullOrEmpty(objetoFinal))
            {
                objetoFinal = objetoFinal.Substring(0, objetoFinal.Length - 2);
            }

            return objetoFinal;
        }
        private string CleanParameterPollution(string pParametrosGet)
        {
            List<string> listaParametros = pParametrosGet.Split('&').ToList();
            List<string> listaParametrosEliminar = new List<string>();
            Dictionary<string, string> parametrosDictionary = new Dictionary<string, string>();
            foreach (string parametro in listaParametros)
            {
                if (parametro.Contains("="))
                {
                    string[] valores = parametro.Split('=');
                    string clave = valores[0];
                    string valor = valores[1];
                    if (!parametrosDictionary.ContainsKey(clave))
                    {
                        parametrosDictionary.Add(clave, valor);
                    }
                    else if (clave.Contains("search") & !clave.Contains(":"))
                    {
                        listaParametrosEliminar.Add(parametro);
                    }
                    else if (parametrosDictionary[clave].Contains(valor))
                    {
                        listaParametrosEliminar.Add(parametro);
                    }
                    else
                    {
                        parametrosDictionary[clave] = $"{parametrosDictionary[clave]},{valor}";
                    }
                }
                else
                {
                    listaParametrosEliminar.Add(parametro);
                }
            }
            foreach (string eliminar in listaParametrosEliminar)
            {
                listaParametros.Remove(eliminar);
            }
            pParametrosGet = "";
            foreach (string parametro in listaParametros)
            {
                pParametrosGet = $"{pParametrosGet}&{parametro}";
            }
            pParametrosGet = pParametrosGet.TrimStart('&');
            return pParametrosGet;
        }

        /// <summary>
        /// Modifica la url con las redirecciones necesarias
        /// </summary>
        /// <param name="pUrl">Url a comprobar redirecciones</param>
        /// <returns>Url a que va a redireccionar</returns>
        private string ProcesarUrlParaRedireccion(string pUrl, string pParametrosGet)
        {
            if (pUrl.Contains("?"))
            {
                string baseUrl = pUrl.Substring(0, pUrl.IndexOf("?"));
                string parametrosGet = pUrl.Substring(pUrl.IndexOf("?") + 1);
                return ProcesarUrlParaRedireccion(baseUrl, parametrosGet);
            }
            else
            {
                string pUrlOriginal = pUrl;

                //si pUrl acaba en /pagina/1 purl=purl sin /pagina/1 y + ?pagina=1
                int numPagina = 0;
                string valorPagina = pUrl.Substring(pUrl.LastIndexOf("/") + 1);

                if (int.TryParse(valorPagina, out numPagina) && pUrl.EndsWith("/pagina/" + numPagina))
                {
                    pUrl = pUrl.Replace("/pagina/" + numPagina, "");
                    pParametrosGet = "pagina=" + numPagina + "&" + pParametrosGet;
                }
                //HTTP Parameter Pollution
                pParametrosGet = CleanParameterPollution(pParametrosGet);
                //FIN HTTP Parameter Pollution
                //se obtienen los RequestParams de la url pasada. Como la url va cambiando no podemos usar RequestParams("categoria") porque la QueryString original
                //permanece invariable y no nos sirve en este caso
                string RequestParamsSearchInfo = "";
                string RequestParamsCategoria = "";

                string pUrlMinus = pUrl.ToLower();

                string urlCat = UtilIdiomas.LanguageCode + "/";

                if (pUrlMinus.Contains((UtilIdiomas.GetText("URLSEM", "CATEGORIA") + "/" + ProyectoSeleccionado.NombreCorto + "/").ToLower()))
                {
                    urlCat += (UtilIdiomas.GetText("URLSEM", "CATEGORIA") + "/" + ProyectoSeleccionado.NombreCorto + "/").ToLower() + "/";
                }

                urlCat += (UtilIdiomas.GetText("URLSEM", "CATEGORIA") + "/").ToLower();
                mContRedirecciones++;

                string aux = pUrlMinus.Substring(pUrlMinus.IndexOf(urlCat) + urlCat.Length);
                if (pUrlMinus.Contains(urlCat) && aux.Contains("/"))
                {
                    if (aux.Contains("/"))
                    {
                        string aux2 = aux.Substring(aux.IndexOf("/") + 1);
                        if (aux2.Contains("/"))
                        {
                            RequestParamsCategoria = aux2.Substring(0, aux2.IndexOf("/"));
                        }
                        else
                        {
                            RequestParamsCategoria = aux2;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(RequestParamsCategoria))
                {
                    //"materaialeducativo/categoria/nomcat/GUID/XXXXXXXXXXXXXXXXXXXXXXXXX/XXXXXXXXXXXXXX"
                    if (!string.IsNullOrEmpty(pUrlMinus.Substring(pUrlMinus.IndexOf(RequestParamsCategoria) + RequestParamsCategoria.Length)))
                    {
                        RequestParamsSearchInfo = pUrlMinus.Substring(pUrlMinus.IndexOf(RequestParamsCategoria) + RequestParamsCategoria.Length + 1);
                    }
                }
                else
                {
                    //"materaialeducativo/busqueda/XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    //"materaialeducativo/recursos/XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    int indiceNombreProy = pUrlMinus.IndexOf((ProyectoSeleccionado.NombreCorto + "/").ToLower()) + (ProyectoSeleccionado.NombreCorto + "/").Length;
                    string searchInfo = pUrlMinus.Substring(indiceNombreProy);
                    RequestParamsSearchInfo = searchInfo.Substring(searchInfo.IndexOf("/") + 1);
                }

                if (pUrlMinus.Contains((ProyectoSeleccionado.NombreCorto + "/" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA2")).ToLower()))
                {
                    //reemplazamos busqueda-avanzada por busqueda
                    string textoBusquedaAvanzada = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA2");
                    string textoBusqueda = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    int indiceBusquedaAvanzada = pUrlMinus.IndexOf(textoBusquedaAvanzada);
                    pUrl = pUrl.Substring(0, indiceBusquedaAvanzada) + textoBusqueda + pUrl.Substring(indiceBusquedaAvanzada + textoBusquedaAvanzada.Length);
                    pUrlMinus = pUrl.ToLower();
                }

                if (!string.IsNullOrEmpty(RequestParamsCategoria))
                {
                    string categoriaID = RequestParamsCategoria;
                    Guid catID = Guid.Empty;

                    if (!Guid.TryParse(categoriaID, out catID) || !GestorTesauroProyectoActual.ListaCategoriasTesauro.ContainsKey(catID))
                    {
                        pUrl = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, null) + "/" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    }
                    else if (!string.IsNullOrEmpty(RequestParamsSearchInfo))
                    {
                        int inicioCategoria = pUrlMinus.IndexOf(RequestParamsCategoria);
                        pUrl = pUrl.Substring(0, inicioCategoria + RequestParamsCategoria.Length);
                    }
                }
                else if (!string.IsNullOrEmpty(RequestParamsSearchInfo))
                {
                    string urlBusquedaPaginaActual = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto);
                    string rutaPestanya = "";
                    switch (ProyectoPestanyaActual.TipoPestanya)
                    {
                        case TipoPestanyaMenu.Indice:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "INDICE");
                            break;
                        case TipoPestanyaMenu.Recursos:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "RECURSOS");
                            break;
                        case TipoPestanyaMenu.Preguntas:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "PREGUNTAS");
                            break;
                        case TipoPestanyaMenu.Debates:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "DEBATES");
                            break;
                        case TipoPestanyaMenu.Encuestas:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "ENCUESTAS");
                            break;
                        case TipoPestanyaMenu.PersonasYOrganizaciones:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES");
                            break;
                        case TipoPestanyaMenu.AcercaDe:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "ACERCADE");
                            break;
                        case TipoPestanyaMenu.BusquedaAvanzada:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                            break;
                        case TipoPestanyaMenu.Borradores:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "BORRADORES");
                            urlBusquedaPaginaActual = BaseURLIdioma + UrlPerfil.TrimEnd('/');
                            break;
                        case TipoPestanyaMenu.Contribuciones:
                            rutaPestanya = UtilIdiomas.GetText("URLSEM", "MISCONTRIBUCIONES");
                            urlBusquedaPaginaActual = BaseURLIdioma + UrlPerfil.TrimEnd('/');
                            break;
                    }
                    if (!string.IsNullOrEmpty(ProyectoPestanyaActual.Ruta))
                    {
                        rutaPestanya = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Ruta, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    }
                    if (!string.IsNullOrEmpty(rutaPestanya))
                    {
                        urlBusquedaPaginaActual += $"/{rutaPestanya}";
                    }

                    string[] filtro = RequestParamsSearchInfo.Split('/');
                    Guid aux2 = Guid.Empty;
                    if (filtro.Length > 2 && filtro[filtro.Length - 2] == UtilIdiomas.GetText("URLSEM", "TAG"))
                    {
                        pUrl = $"{urlBusquedaPaginaActual}/{UtilIdiomas.GetText("URLSEM", "TAG")}/{filtro[filtro.Length - 1]}";
                    }
                    else if (filtro.Length > 2 && filtro[filtro.Length - 2] == UtilIdiomas.GetText("URLSEM", "CATEGORIA") && Guid.TryParse(filtro[filtro.Length - 1], out aux2))
                    {
                        pUrl = $"{urlBusquedaPaginaActual}/{UtilIdiomas.GetText("URLSEM", "CATEGORIA")}/{filtro[filtro.Length - 1]}";
                    }
                    else if (filtro.Length > 3 && filtro[filtro.Length - 3] == UtilIdiomas.GetText("URLSEM", "CATEGORIA") && Guid.TryParse(filtro[filtro.Length - 1], out aux2))
                    {
                        pUrl = $"{urlBusquedaPaginaActual}/{UtilIdiomas.GetText("URLSEM", "CATEGORIA")}/{filtro[filtro.Length - 2]}/{filtro[filtro.Length - 1]}";
                    }
                    else if (filtro[0] == UtilIdiomas.GetText("URLSEM", "CATEGORIA"))
                    {
                        Guid idCategoria = Guid.Empty;
                        if (Guid.TryParse(filtro[1], out idCategoria))
                        {
                            pUrl = pUrl.ToLower();
                            pUrl = pUrl.Replace($"/{filtro[0]}/", $"/{filtro[0]}/nomcat/");

                        }
                        else if (filtro.Length == 2 || (filtro.Length > 2 && !Guid.TryParse(filtro[2], out idCategoria)) || !GestorTesauroProyectoActual.ListaCategoriasTesauro.ContainsKey(idCategoria))
                        {
                            pUrl = pUrl.Substring(0, pUrlMinus.IndexOf(filtro[0].ToLower()) - 1);
                        }
                        else
                        {
                            bool esBusquedaCategoria = pUrl.Contains($"/{UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA")}/{UtilIdiomas.GetText("URLSEM", "CATEGORIA")}/");
                            if (esBusquedaCategoria)
                            {
                                //Quitamos el texto /busqueda de /busqueda/categoria
                                string textoBusqueda = $"/{UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA")}";
                                int indiceBusqueda = pUrlMinus.IndexOf(textoBusqueda);
                                pUrl = pUrl.Substring(0, indiceBusqueda) + pUrl.Substring(indiceBusqueda + textoBusqueda.Length);
                                pUrlMinus = pUrl.ToLower();
                            }
                            if (filtro.Length > 3)
                            {
                                int inicioCategoria = pUrlMinus.IndexOf(filtro[2].ToLower());
                                pUrl = pUrl.Substring(0, inicioCategoria + filtro[2].Length);
                            }
                        }
                    }
                }

                if (pUrlOriginal != pUrl && mContRedirecciones < 10)
                {
                    return ProcesarUrlParaRedireccion(pUrl, pParametrosGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(pParametrosGet))
                    {
                        return $"{pUrl}?{pParametrosGet}";
                    }
                    else
                    {
                        return pUrl;
                    }
                }
            }
        }

        private Dictionary<string, string> ProcesarUrlObtenerFiltros(string pUrl, ref string argumentos, ref string filtroPag, ref string separadorArgumentos)
        {
            Dictionary<string, string> listaFiltrosURL = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(RequestParams("categoria")))
            {
                GestionTesauro gestorTesauro = CargarGestorTesauroProyectoActual();

                string categoria = RequestParams("categoria");
                Guid categoriaID;

                if (Guid.TryParse(categoria, out categoriaID) && gestorTesauro.ListaCategoriasTesauro.ContainsKey(categoriaID))
                {
                    filtroPag = $"skos:ConceptID=gnoss:{HttpUtility.UrlDecode(categoria).ToUpper()}";
                    argumentos += separadorArgumentos + filtroPag;
                    separadorArgumentos = "|";

                    string nomCategoriaReal = gestorTesauro.ListaCategoriasTesauro[categoriaID].Nombre[UtilIdiomas.LanguageCode];

                    listaFiltrosURL.Add(UtilIdiomas.GetText("URLSEM", "CATEGORIA"), UtilCadenas.LimpiarInyeccionCodigo(nomCategoriaReal));
                }
            }
            else if (!string.IsNullOrEmpty(RequestParams("searchInfo")))
            {
                string[] filtro = RequestParams("searchInfo").Split('/');

                if (filtro[0].ToLower() == UtilIdiomas.GetText("URLSEM", "CATEGORIA").ToLower())
                {
                    GestionTesauro gestorTesauro = CargarGestorTesauroProyectoActual();

                    filtroPag = $"skos:ConceptID=gnoss:{HttpUtility.UrlDecode(filtro[2]).ToUpper()}";
                    argumentos += separadorArgumentos + filtroPag;
                    separadorArgumentos = "|";

                    string nomCategoriaReal = gestorTesauro.ListaCategoriasTesauro[new Guid(filtro[2])].Nombre[UtilIdiomas.LanguageCode];

                    listaFiltrosURL.Add(filtro[0], UtilCadenas.LimpiarInyeccionCodigo(nomCategoriaReal));
                }
                else if (filtro[0].ToLower() == UtilIdiomas.GetText("URLSEM", "TAG").ToLower())
                {
                    filtroPag = $"sioc_t:Tag={HttpUtility.UrlDecode(filtro[1])}";
                    argumentos += separadorArgumentos + filtroPag;
                    separadorArgumentos = "|";

                    listaFiltrosURL.Add(filtro[0], UtilCadenas.LimpiarInyeccionCodigo(filtro[1]));
                }
                else
                {
                    string nombreRealFaceta;
                    if (filtro[0].Equals("autor"))
                    {
                        nombreRealFaceta = "gnoss:hasautor";
                    }
                    else
                    {
                        FacetaCN tablasCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                        nombreRealFaceta = tablasCN.ObtenerNombreFacetaRedireccion(filtro[0]);
                    }

                    if (nombreRealFaceta != null && nombreRealFaceta != "")
                    {
                        if (filtro[0].Equals("autor"))
                        {
                            filtroPag = $"{nombreRealFaceta}={HttpUtility.UrlDecode(filtro[1])}";
                            listaFiltrosURL.Add("autor", UtilCadenas.LimpiarInyeccionCodigo(filtro[1]));
                        }
                        else
                        {
                            filtroPag = $"{nombreRealFaceta}={HttpUtility.UrlDecode(filtro[0])}";
                            listaFiltrosURL.Add(filtro[0], UtilCadenas.LimpiarInyeccionCodigo(filtro[0]));
                        }

                        argumentos += separadorArgumentos + filtroPag;
                        separadorArgumentos = "|";
                    }
                }
            }
            else if (!string.IsNullOrEmpty(RequestParams("search")))
            {
                listaFiltrosURL.Add("", UtilCadenas.LimpiarInyeccionCodigo(RequestParams("search")));
            }

            if (listaFiltrosURL.Count == 0)
            {
                Uri urlBusqueda = new Uri(pUrl);
                if (urlBusqueda.Query.StartsWith("?skos:ConceptID=gnoss:") && urlBusqueda.Query.Substring(1).Split('=').Length == 2)
                {
                    string filtroBusqueda = urlBusqueda.Query.Substring(1);

                    if (filtroBusqueda.Contains('&'))
                    {
                        filtroBusqueda = filtroBusqueda.Substring(0, filtroBusqueda.IndexOf('&'));
                    }
                    string categoriaBusqueda = filtroBusqueda.Replace("skos:ConceptID=gnoss:", "");
                    Guid categoriaID;

                    GestionTesauro gestorTesauro = CargarGestorTesauroProyectoActual();

                    if (Guid.TryParse(categoriaBusqueda, out categoriaID) && gestorTesauro.ListaCategoriasTesauro.ContainsKey(categoriaID))
                    {
                        string nomCategoriaReal = gestorTesauro.ListaCategoriasTesauro[categoriaID].Nombre[UtilIdiomas.LanguageCode];
                        listaFiltrosURL.Add(UtilIdiomas.GetText("URLSEM", "CATEGORIA").ToLower(), UtilCadenas.LimpiarInyeccionCodigo(nomCategoriaReal));
                    }
                }
            }
            return listaFiltrosURL;
        }

        /// <summary>
        /// Obtiene el dataSet con la configuración de la presentación de los resultados.
        /// </summary>
        /// <param name="pElementos">Elementos</param>
        /// <returns>DataSet con la configuración de la presentación de los resultados</returns>
        private FacetadoDS ObtenerPresentacionFacetado(DataWrapperExportacionBusqueda pExportacionBusquedaDW, List<ElementoGnoss> pElementos, Guid pProyectoSeleccionado, Dictionary<string, List<string>> pInformacionOntologias, Guid pExportacionID, string pParametros)
        {
            mLoggingService.AgregarEntrada("Inicio ObtenerPresentacionFacetado");

            #region Obtener propiedades de documentos semanticos para listado
            FacetadoDS facDSPresentacion = null;
            List<string> listaPropiedades = new List<string>();
            List<string> listaGrafos = new List<string>();
            List<ProyectoPestanyaBusquedaExportacionPropiedad> filasExporProp = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Where(item => item.ExportacionID.Equals(pExportacionID)).ToList();

            foreach (ElementoGnoss elementoGnoss in pElementos)
            {
                if (elementoGnoss is Documento && ((Documento)elementoGnoss).TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    #region Cargamos las propiedades que queremos mostrar del listado

                    foreach (ProyectoPestanyaBusquedaExportacionPropiedad filaExpProp in filasExporProp)
                    {
                        if (filaExpProp.OntologiaID.HasValue && !string.IsNullOrEmpty(filaExpProp.Ontologia) && ((Documento)elementoGnoss).ElementoVinculadoID == filaExpProp.OntologiaID)
                        {
                            string[] propiedades = filaExpProp.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string propiedad in propiedades)
                            {
                                if (!listaPropiedades.Contains(propiedad))
                                {
                                    if (propiedad.Contains("|"))
                                    {
                                        string[] delimiter = { "|" };
                                        foreach (string propiedadIndividual in propiedad.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            if (!listaPropiedades.Contains(propiedadIndividual))
                                            {
                                                listaPropiedades.Add(propiedadIndividual);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!listaPropiedades.Contains(propiedad))
                                        {
                                            listaPropiedades.Add(propiedad);
                                        }
                                    }
                                }

                                int posicionBarra = filaExpProp.Ontologia.LastIndexOf("/");
                                string grafo = filaExpProp.Ontologia.Substring(posicionBarra + 1);
                                if (grafo.Contains("#"))
                                {
                                    grafo = grafo.Substring(0, grafo.IndexOf("#"));
                                }

                                if (!listaGrafos.Contains(grafo))
                                {
                                    listaGrafos.Add(grafo);
                                }
                            }
                        }
                    }

                    #endregion
                }
            }

            if (listaPropiedades.Count > 0 && listaGrafos.Count > 0)
            {
                CargarDataSetPresentacionFacetado(ref facDSPresentacion, listaGrafos, pParametros, pProyectoSeleccionado, listaPropiedades, pInformacionOntologias);
            }

            #endregion

            mLoggingService.AgregarEntrada("Fin ObtenerPresentacionFacetado");

            return facDSPresentacion;
        }

        private void CargarDataSetPresentacionFacetado(ref FacetadoDS pFacDSPresentacion, List<string> pListaGrafos, string pParametros, Guid pProyectoSeleccionado, List<string> pListaPropiedades, Dictionary<string, List<string>> pInformacionOntologias)
        {
            string filtroContextoWhere = string.Empty;
            bool excluirPersonas = VariableTipoBusqueda != TipoBusqueda.PersonasYOrganizaciones;

            bool omitirPalabrasNoRelevantesSearch = true;
            bool esMiembroComunidad = IdentidadActual.Clave != UsuarioAD.Invitado;
            bool estaEnMyGnoss = ProyectoSeleccionado.Equals(ProyectoAD.MetaProyecto);
            bool esInvitado = IdentidadActual.Clave != UsuarioAD.Invitado;

            string identidadID = IdentidadActual.Clave.ToString();
            string proyectoID = ProyectoSeleccionado.Clave.ToString();

            Dictionary<string, List<string>> listaFiltros = new Dictionary<string, List<string>>();
            Dictionary<string, Tuple<string, string, string, bool>> filtrosSearchPersonalizados = new Dictionary<string, Tuple<string, string, string, bool>>();

            int paginaActual = 1;
            string filtroOrdenadoPor = string.Empty;
            bool filtroOrdenDescendente = true;

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            FacetadoCN facCN = new FacetadoCN(UrlIntragnoss, ProyectoSeleccionado.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            facCN.InformacionOntologias = pInformacionOntologias;
            facCN.FacetaDW = facetaCL.ObtenerTodasFacetasDeProyecto(null, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, false);

            //Nuevo Revisar Juan
            filtrosSearchPersonalizados = UtilServiciosFacetas.ObtenerListaFiltrosSearchPersonalizados(pProyectoSeleccionado);


            Dictionary<string, List<string>> listaFiltrosFacetasUsuario = new Dictionary<string, List<string>>();
            UtilServiciosFacetas.ExtraerParametros(facCN.FacetaDW, pProyectoSeleccionado, pParametros, new List<string>(), ref filtroOrdenDescendente, ref paginaActual, ref filtroOrdenadoPor, listaFiltros, listaFiltrosFacetasUsuario, IdentidadActual.Clave, filtrosSearchPersonalizados);

            List<string> listaFiltrosExtra = UtilServiciosFacetas.ObtenerListaItemsBusquedaExtra(listaFiltrosFacetasUsuario, VariableTipoBusqueda, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
            List<string> semanticos = UtilServiciosFacetas.ObtenerFormulariosSemanticos(VariableTipoBusqueda, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);

            FacetadoCL facetadoCL = new FacetadoCL(UtilServicios.UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCL>(), mLoggerFactory);

            bool tieneRecursosPrivados = false;

            if (ParametrosGeneralesRow.PermitirRecursosPrivados)
            {
                //Si el usuario no tiene 
                tieneRecursosPrivados = UtilServiciosFacetas.ChequearUsuarioTieneRecursosPrivados(false, IdentidadActual.PerfilID, VariableTipoBusqueda, ProyectoSeleccionado.Clave, facetadoCL);
            }

            facCN.FacetadoAD.UsuarioTieneRecursosPrivados = tieneRecursosPrivados;

            foreach (string grafo in pListaGrafos)
            {
                // Dividir las propiedades en bloques de numPropiedadesConsulta y hacer merge de los dataset
                List<string> listaPropiedadesTemp = new List<string>(pListaPropiedades);

                int numPropiedadesConsulta = 5;
                while (listaPropiedadesTemp.Count > 0)
                {
                    List<string> partialListaPropiedadesTemp = listaPropiedadesTemp.Take(numPropiedadesConsulta).ToList();

                    try
                    {
                        if (pFacDSPresentacion == null)
                        {
                            pFacDSPresentacion = facCN.ObtenerValoresPropiedadesEntidadesPorDocumentoID(pProyectoSeleccionado.ToString(), new List<Guid>(), partialListaPropiedadesTemp, mControladorBase.IdiomaUsuario, true, listaFiltros, listaFiltrosExtra, esMiembroComunidad, proyectoID, semanticos, filtroContextoWhere, ProyectoSeleccionado.TipoProyecto, excluirPersonas, omitirPalabrasNoRelevantesSearch, filtrosSearchPersonalizados, estaEnMyGnoss, esInvitado, identidadID, true);
                        }
                        else
                        {
                            pFacDSPresentacion.Merge(facCN.ObtenerValoresPropiedadesEntidadesPorDocumentoID(pProyectoSeleccionado.ToString(), new List<Guid>(), partialListaPropiedadesTemp, mControladorBase.IdiomaUsuario, true, listaFiltros, listaFiltrosExtra, esMiembroComunidad, proyectoID, semanticos, filtroContextoWhere, ProyectoSeleccionado.TipoProyecto, excluirPersonas, omitirPalabrasNoRelevantesSearch, filtrosSearchPersonalizados, estaEnMyGnoss, esInvitado, identidadID, true));
                        }
                    }
                    catch (Exception)
                    {
                        UtilidadesVirtuoso utilidadesVirtuoso = new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UtilidadesVirtuoso>(), mLoggerFactory);
                        while (!utilidadesVirtuoso.ServidorOperativo("acid", UrlIntragnoss))
                        {
                            //Dormimos 5 segundos
                            Thread.Sleep(5 * 1000);
                        }

                        if (pFacDSPresentacion == null)
                        {
                            pFacDSPresentacion = facCN.ObtenerValoresPropiedadesEntidadesPorDocumentoID(pProyectoSeleccionado.ToString(), new List<Guid>(), partialListaPropiedadesTemp, mControladorBase.IdiomaUsuario, true, listaFiltros, listaFiltrosExtra, esMiembroComunidad, proyectoID, semanticos, filtroContextoWhere, ProyectoSeleccionado.TipoProyecto, excluirPersonas, omitirPalabrasNoRelevantesSearch, filtrosSearchPersonalizados, estaEnMyGnoss, esInvitado, identidadID, true);
                        }
                        else
                        {
                            pFacDSPresentacion.Merge(facCN.ObtenerValoresPropiedadesEntidadesPorDocumentoID(pProyectoSeleccionado.ToString(), new List<Guid>(), partialListaPropiedadesTemp, mControladorBase.IdiomaUsuario, true, listaFiltros, listaFiltrosExtra, esMiembroComunidad, proyectoID, semanticos, filtroContextoWhere, ProyectoSeleccionado.TipoProyecto, excluirPersonas, omitirPalabrasNoRelevantesSearch, filtrosSearchPersonalizados, estaEnMyGnoss, esInvitado, identidadID, true));
                        }
                    }

                    if (listaPropiedadesTemp.Count >= numPropiedadesConsulta)
                    {
                        foreach (string resultado in partialListaPropiedadesTemp)
                        {
                            listaPropiedadesTemp.Remove(resultado);
                        }
                    }
                    else
                    {
                        listaPropiedadesTemp = new List<string>();
                    }
                }
            }

            facCN.Dispose();
        }

        /// <summary>
        /// Trata el texto para que sea correcto en un CSV.
        /// </summary>
        /// <param name="pTexto">Texto</param>
        /// <returns>Texto para que sea correcto en un CSV</returns>
        private string TratarTextoParaCSV(string pTexto)
        {
            return pTexto.Replace(";", ",").Replace("\n", "  ").Replace("\r", "").Replace("\r\n", " ");
        }

        private GestionTesauro CargarGestorTesauroProyectoActual()
        {
            TesauroCL tesCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);
            GestionTesauro gestorTesauro = new GestionTesauro(tesCL.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            return gestorTesauro;
        }

        private ActionResult RedirigirAPaginaCon301(string pUrl)
        {
            return new RedirectResult(pUrl, true);
        }

        private void CargarListaFiltrosOrden(SearchViewModel pModelo)
        {
            List<string> filtrosSearchPersonalizados = new List<string>();
            if (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoSearchPersonalizado != null)
            {
                foreach (ProyectoSearchPersonalizado fila in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoSearchPersonalizado)
                {

                    filtrosSearchPersonalizados.Add(fila.NombreFiltro);
                }
            }

            Dictionary<string, string> listaFiltrosOrden = new Dictionary<string, string>();

            if (!VariableTipoBusqueda.Equals(TipoBusqueda.BusquedaAvanzada) || FiltrosOrdenConfig.Count > 0)
            {
                bool contieneFiltroSearchPersonalizado = false;
                foreach (string searchPersonalizado in filtrosSearchPersonalizados)
                {
                    if (!string.IsNullOrEmpty(RequestParams(searchPersonalizado)))
                    {
                        contieneFiltroSearchPersonalizado = true;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(RequestParams("search")) || contieneFiltroSearchPersonalizado)
                {
                    //Si es una búsqueda search, la primera vez se busca por relevancia...
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("METABUSCADOR", "RELEVANCIA"), "gnoss:relevancia");
                }
                if (FiltrosOrdenConfig.Count > 0)
                {
                    foreach (string filtro in FiltrosOrdenConfig.Keys)
                    {
                        string filtroLimpio = filtro;

                        listaFiltrosOrden.Add(FiltrosOrdenConfig[filtro], filtroLimpio);
                    }
                }
                else if (ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_PERSONA) || ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_ORGANIZACION))
                {
                    //listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "POPULARIDAD"), "gnoss:hasPopularidad");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("INVITACIONES", "APELLIDOS"), "foaf:familyName");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "NOMBRE"), "foaf:firstName");
                }
                else if (ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_RECURSOS) || ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_DEBATES) || ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_DAFOS) || ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_PREGUNTAS) || ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_ENCUESTAS))
                {
                    if (ProyectoSeleccionado.EsCatalogo && FiltrosOrdenConfig.Count == 0)
                    {
                        return;
                    }
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "FECHA"), "gnoss:hasfechapublicacion");
                    //listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "POPULARIDAD"), "gnoss:hasPopularidad");
                }
                else if (ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS) || ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENTARIOS))
                {
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "FECHA"), "gnoss:hasfechapublicacion");
                    //listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "POPULARIDAD"), "gnoss:hasPopularidad");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "NOMBRE"), "foaf:firstName");
                }

                if (ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_BLOGS))
                {
                    //listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "POPULARIDAD"), "gnoss:hasPopularidad");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "FECHA"), "gnoss:hasfechapublicacion");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "NOMBRE"), "foaf:firstName");
                }
                if (ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_ARTICULOSBLOG))
                {
                    //listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "POPULARIDAD"), "gnoss:hasPopularidad");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "FECHA"), "gnoss:hasfechapublicacion");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "NOMBRE"), "foaf:firstName");
                }
                if (ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_COMUNIDADES))
                {
                    //listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "POPULARIDAD"), "gnoss:hasPopularidad");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOS", "NOMBRE"), "foaf:firstName");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("OBJETOGNOSS", "NUMRECURSOS"), "gnoss:hasnumerorecursos");
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("COMMON", "FECHACREACION"), "gnoss:hasfechaAlta");
                }

                if (listaFiltrosOrden.Count == 0)
                {
                    //Si es una búsqueda search, la primera vez se busca por relevancia...
                    listaFiltrosOrden.Add(UtilIdiomas.GetText("METABUSCADOR", "RELEVANCIA"), "gnoss:relevancia");
                }

                pModelo.FilterOrderList = listaFiltrosOrden;

                pModelo.FilterOrderSelected = listaFiltrosOrden.ElementAt(0).Key;
            }
        }



        private void CargarListaVistas(SearchViewModel pModelo)
        {
            pModelo.ViewList = new List<SearchViewModel.ViewTypeModel>();
            if (ProyectoPestanyaActual != null && ProyectoPestanyaActual.FilaProyectoPestanyaBusqueda != null && !string.IsNullOrEmpty(ProyectoPestanyaActual.FilaProyectoPestanyaBusqueda.VistaDisponible))
            {
                string vistas = ProyectoPestanyaActual.FilaProyectoPestanyaBusqueda.VistaDisponible;

                int numvistasDisponibles = 0;
                int vistaPorDefecto = 0;
                int valorMayor = 0;

                int listadoPermitido = 1;
                int mosaicosPermitido = 0;
                int mapaPermitido = 0;
                int chartPermitido = 0;

                int.TryParse(vistas[0].ToString(), out listadoPermitido);
                if (listadoPermitido > 0) { numvistasDisponibles++; }
                if (listadoPermitido > valorMayor) { vistaPorDefecto = 0; valorMayor = listadoPermitido; }

                int.TryParse(vistas[1].ToString(), out mosaicosPermitido);
                if (mosaicosPermitido > 0) { numvistasDisponibles++; }
                if (mosaicosPermitido > valorMayor) { vistaPorDefecto = 1; valorMayor = mosaicosPermitido; }

                if (vistas.Length > 2)
                {
                    int.TryParse(vistas[2].ToString(), out mapaPermitido);
                    if (mapaPermitido > 0) { numvistasDisponibles++; }
                    if (mapaPermitido > valorMayor) { vistaPorDefecto = 2; valorMayor = mapaPermitido; }
                }

                if (vistas.Length > 3)
                {
                    int.TryParse(vistas[3].ToString(), out chartPermitido);
                    if (chartPermitido > 0) { numvistasDisponibles++; }
                    if (chartPermitido > valorMayor) { vistaPorDefecto = 3; }
                }

                if (EsBot)
                {
                    mapaPermitido = 0;
                    chartPermitido = 0;
                }

                if (listadoPermitido > 0)
                {
                    SearchViewModel.ViewTypeModel vistaListado = new SearchViewModel.ViewTypeModel();
                    vistaListado.ViewType = SearchViewModel.ViewTypeModel.ViewTypeSearch.List;
                    vistaListado.Active = vistaPorDefecto == 0;
                    pModelo.ViewList.Add(vistaListado);
                }
                if (mosaicosPermitido > 0)
                {
                    SearchViewModel.ViewTypeModel vistaMosaico = new SearchViewModel.ViewTypeModel();
                    vistaMosaico.ViewType = SearchViewModel.ViewTypeModel.ViewTypeSearch.Grid;
                    vistaMosaico.Active = vistaPorDefecto == 1;
                    pModelo.ViewList.Add(vistaMosaico);
                }
                if (mapaPermitido > 0)
                {
                    SearchViewModel.ViewTypeModel vistaMapa = new SearchViewModel.ViewTypeModel();
                    vistaMapa.ViewType = SearchViewModel.ViewTypeModel.ViewTypeSearch.Map;
                    vistaMapa.Active = vistaPorDefecto == 2;
                    mEsVistaMapa = vistaPorDefecto == 2;
                    pModelo.ViewList.Add(vistaMapa);
                    AgregarMapaALaPagina();
                }
                if (chartPermitido > 0)
                {
                    SearchViewModel.ViewTypeModel vistaGrafico = new SearchViewModel.ViewTypeModel();
                    vistaGrafico.ViewType = SearchViewModel.ViewTypeModel.ViewTypeSearch.Chart;
                    vistaGrafico.Active = vistaPorDefecto == 3;
                    pModelo.ViewList.Add(vistaGrafico);

                    if (mapaPermitido == 0)
                    {
                        ViewBag.ListaJS.Add("//www.google.com/jsapi?key=" /* TODO Javier + Conexion.ObtenerParametro("Config/services.config", "config/claveApiGoogle", false)*/);
                    }

                    FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
                    DataWrapperFacetas facetaDS = facetaCL.ObtenerDatosChartProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                    facetaCL.Dispose();

                    ViewBag.JSGraficos = "";

                    pModelo.ChartList = new List<SearchViewModel.ChartViewModel>();

                    foreach (FacetaConfigProyChart filaConfig in facetaDS.ListaFacetaConfigProyChart.OrderBy(item => item.Orden))
                    {
                        ViewBag.JSGraficos += filaConfig.JSBase;

                        bool pintarChart = true;

                        if (filaConfig.Ontologias != null && filaConfig.Ontologias != "")
                        {
                            pintarChart = false;

                            if (mFilaPestanyaBusqueda != null)
                            {
                                foreach (string ontologia in filaConfig.Ontologias.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (mFilaPestanyaBusqueda.CampoFiltro.Contains("=" + ontologia))
                                    {
                                        pintarChart = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (pintarChart)
                        {
                            SearchViewModel.ChartViewModel grafico = new SearchViewModel.ChartViewModel();
                            grafico.Key = filaConfig.ChartID;
                            grafico.Name = UtilCadenas.ObtenerTextoDeIdioma(filaConfig.Nombre, IdiomaUsuario, ParametrosGeneralesRow.IdiomaDefecto);
                            grafico.JS = filaConfig.JSBusqueda;
                            pModelo.ChartList.Add(grafico);
                        }
                    }
                }
                List<ProyectoPestanyaDashboardAsistente> asistentes = ProyectoPestanyaActual.FilasProyectoPestanyaDashboardAsistente;
                if (asistentes != null)
                {
                    DashboardViewModel dashboardViewModel = new DashboardViewModel();
                    List<DashboardViewModel.AsistenteModel> listAsistentes = new List<DashboardViewModel.AsistenteModel>();
                    foreach (ProyectoPestanyaDashboardAsistente asis in asistentes)
                    {
                        DashboardViewModel.AsistenteModel asistenteModel = new DashboardViewModel.AsistenteModel();
                        asistenteModel.Key = asis.AsisID;
                        asistenteModel.AsistenteName = asis.Nombre;
                        asistenteModel.PropExtra = asis.PropExtra;

                        asistenteModel.Tamanyo = asis.Tamanyo;
                        asistenteModel.Tipo = asis.Tipo;
                        asistenteModel.Titulo = asis.Titulo;
                        asistenteModel.ListDataset = new List<DashboardViewModel.AsistenteModel.DatasetModel>();
                        List<string> etiquetas = new List<string>();
                        if (asis.Labels != null)
                        {
                            etiquetas.Add(asis.Labels);
                        }

                        List<ProyectoPestanyaDashboardAsistenteDataset> filasDataset = asis.ProyectoPestanyaDashboardAsistenteDataset.OrderBy(x => x.Orden).ToList();
                        foreach (ProyectoPestanyaDashboardAsistenteDataset filaDataset in filasDataset)
                        {
                            DashboardViewModel.AsistenteModel.DatasetModel dat = new DashboardViewModel.AsistenteModel.DatasetModel();

                            dat.Color = filaDataset.Color;
                            dat.Nombre = filaDataset.Nombre;
                            dat.Key = filaDataset.DatasetID;

                            etiquetas.Add(filaDataset.Datos);

                            asistenteModel.ListDataset.Add(dat);
                        }

                        //ORDENES
                        //List<string> etiquetasAux = etiquetas.Select(x => (string)x.Clone()).ToList();
                        string select = asis.Select;
                        int[] ordenes = new int[etiquetas.Count];

                        List<string> etiquetasOrdenadas = etiquetas.OrderBy(x => select.IndexOf(x)).ToList();
                        int i = 0;
                        foreach (string etiqueta in etiquetas)
                        {
                            ordenes[i] = etiquetasOrdenadas.IndexOf(etiqueta);
                            i++;
                        }
                        asistenteModel.Ordenes = ordenes;
                        asistenteModel.Orden = asis.Orden;

                        listAsistentes.Add(asistenteModel);
                    }
                    dashboardViewModel.ListAsistente = listAsistentes;
                    pModelo.Dashboard = dashboardViewModel;
                }
            }
            else
            {
                SearchViewModel.ViewTypeModel vistaListado = new SearchViewModel.ViewTypeModel();
                vistaListado.ViewType = SearchViewModel.ViewTypeModel.ViewTypeSearch.List;
                vistaListado.Active = true;
                pModelo.ViewList.Add(vistaListado);
                if (TipoPagina.Equals(TiposPagina.Comunidades))
                {
                    SearchViewModel.ViewTypeModel vistaMosaico = new SearchViewModel.ViewTypeModel();
                    vistaMosaico.ViewType = SearchViewModel.ViewTypeModel.ViewTypeSearch.Grid;
                    pModelo.ViewList.Add(vistaMosaico);
                }
            }
        }

        private string ObtenerNombrePestanyaDeURL(string pUrl)
        {
            mLoggingService.AgregarEntrada($"MVC-ObtenerNombrePestañaDeURL-purl = {pUrl}");

            string nombrePestaña = pUrl.Substring(pUrl.IndexOf('/' + ProyectoSeleccionado.NombreCorto + '/') + ProyectoSeleccionado.NombreCorto.Length + 2);

            mLoggingService.AgregarEntrada($"MVC-ObtenerNombrePestañaDeURL-nombrePestaña = {nombrePestaña}");

            if (nombrePestaña.IndexOf('/') > 0)
            {
                nombrePestaña = nombrePestaña.Substring(0, nombrePestaña.IndexOf('/'));
                mLoggingService.AgregarEntrada($"MVC-ObtenerNombrePestañaDeURL-nombrePestaña = {nombrePestaña}");
            }

            if (nombrePestaña.IndexOf('?') > 0)
            {
                nombrePestaña = nombrePestaña.Substring(0, nombrePestaña.IndexOf('?'));
                mLoggingService.AgregarEntrada($"MVC-ObtenerNombrePestañaDeURL-nombrePestaña = {nombrePestaña}");
            }

            return nombrePestaña;
        }

        private void ObtenerDatosPaginaBusqueda(SearchViewModel pModelo)
        {
            string nombrePestaña = ObtenerNombrePestanyaDeURL(mUtilWeb.RequestUrl());

            List<ProyectoPestanyaBusquedaExportacion> filasExportacion = ExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Where(item => item.PestanyaID.Equals(ProyectoPestanyaActual.Clave)).OrderBy(item => item.Orden).ToList();

            if (filasExportacion.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperIdentidad dataWrapperIdentidad = null;
                if (IdentidadActual.TrabajaConOrganizacion && IdentidadActual.OrganizacionID.HasValue)
                {
                    dataWrapperIdentidad = identidadCN.ObtenerGruposDeOrganizacion(IdentidadActual.OrganizacionID.Value, false);
                }

                DataWrapperIdentidad identidadComDW = identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.Clave, true);

                foreach (ProyectoPestanyaBusquedaExportacion filaExportacion in filasExportacion)
                {
                    bool mostrarExportacion = true;
                    //obtiene los grupos exportadores
                    if (filaExportacion.GruposExportadores != null)
                    {
                        mostrarExportacion = false;
                        string[] grupos = filaExportacion.GruposExportadores.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string grupo in grupos)
                        {
                            Guid grupoID = Guid.Empty;
                            if (Guid.TryParse(grupo, out grupoID))
                            {
                                if (dataWrapperIdentidad != null && dataWrapperIdentidad.ListaGrupoIdentidades != null && dataWrapperIdentidad.ListaGrupoIdentidades.Find(grup => grup.GrupoID.Equals(grupoID)) != null)
                                {
                                    mostrarExportacion = true;
                                    break;
                                }

                                if (identidadComDW != null && identidadComDW.ListaGrupoIdentidades != null && identidadComDW.ListaGrupoIdentidades.FirstOrDefault(grupoIden => grupoIden.GrupoID.Equals(grupoID)) != null)
                                {
                                    mostrarExportacion = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (mostrarExportacion)
                    {
                        if (pModelo.ListExportation == null)
                        {
                            pModelo.ListExportation = new List<SearchViewModel.ExportationModel>();
                        }

                        SearchViewModel.ExportationModel exportacion = new SearchViewModel.ExportationModel();
                        exportacion.Key = filaExportacion.ExportacionID;
                        exportacion.ExportationName = filaExportacion.NombreExportacion;
                        exportacion.ExportationFormats = new List<string>();
                        string[] formatos = filaExportacion.FormatosExportacion.Split(',');
                        for (int i = 0; i < formatos.Length; i++)
                        {
                            exportacion.ExportationFormats.Add(formatos[i]);
                        }
                        pModelo.ListExportation.Add(exportacion);
                    }
                }

                identidadCN.Dispose();
            }

            if (RequestParams("contribuciones") != null && RequestParams("contribuciones").Equals("true"))
            {
                TipoPagina = TiposPagina.Contribuciones;
                VariableTipoBusqueda = TipoBusqueda.Contribuciones;

                if (IdentidadPaginaContribuciones != null)
                {
                    //administrador organizacion
                    pModelo.PageName = UtilIdiomas.GetText("PERFIL", "CONTRIBUCIONESDE", IdentidadPaginaContribuciones.NombreCompuesto());
                }
                else if (!string.IsNullOrEmpty(RequestParams("organizacion")))
                {
                    //administrador organizacion
                    pModelo.PageName = UtilIdiomas.GetText("PERFIL", "CONTRIBUCIONESDE", IdentidadActual.OrganizacionPerfil.Nombre);
                }
                else
                {
                    pModelo.PageName = UtilIdiomas.GetText("COMMON", "MISCONTRIBUCIONES");
                }
            }
            else if (RequestParams("borradores") != null && RequestParams("borradores").Equals("true"))
            {
                TipoPagina = TiposPagina.Borradores;
                VariableTipoBusqueda = TipoBusqueda.Contribuciones;

                pModelo.PageName = UtilIdiomas.GetText("BLOGS", "BORRADORES");
            }
            else if (RequestParams("Recursos") != null && RequestParams("Recursos").Equals("true"))
            {
                TipoPagina = TiposPagina.BaseRecursos;
                VariableTipoBusqueda = TipoBusqueda.Recursos;
                pModelo.PageName = UtilIdiomas.GetText("COMMON", "BASERECURSOS");
            }
            else if ((RequestParams("perorg") != null && RequestParams("perorg").Equals("true")) || RequestParams("PersonasOrganizaciones") != null && RequestParams("PersonasOrganizaciones").Equals("true"))
            {
                if (RequestParams("admin") != null && RequestParams("admin").Equals("true") && ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    string vistasV1 = ParametrosAplicacionDS.Where(item => item.Parametro.Equals("VistasV1")).Select(item => item.Valor).FirstOrDefault();
                    if (!string.IsNullOrEmpty(vistasV1) && (vistasV1.Equals("true") || vistasV1.Equals("True")))
                    {
                        EliminarPersonalizacionVistas();
                        CargarPermisosAdministracionComunidadEnViewBag();
                    }
                }

                TipoPagina = TiposPagina.PersonasYOrganizaciones;
                VariableTipoBusqueda = TipoBusqueda.PersonasYOrganizaciones;

                if (ProyectoSeleccionado.EsProyectoEduca)
                {
                    pModelo.PageName = UtilIdiomas.GetText("COMMON", "PROFESORESYALUMNOS");
                }
                else
                {
                    if (ProyectoSeleccionado.Clave.Equals(ProyectoPrincipalUnico) && mControladorBase.UsuarioActual.EsIdentidadInvitada)
                    {
                        pModelo.PageName = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONESINVITADO");
                    }
                    else
                    {
                        pModelo.PageName = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES");
                    }
                }
            }
            else if (RequestParams("preguntas") != null && RequestParams("preguntas").Equals("true"))
            {
                TipoPagina = TiposPagina.Preguntas;
                VariableTipoBusqueda = TipoBusqueda.Preguntas;
                pModelo.PageName = UtilIdiomas.GetText("COMMON", "PREGUNTAS");
            }
            else if (RequestParams("encuestas") != null && RequestParams("encuestas").Equals("true"))
            {
                TipoPagina = TiposPagina.Encuestas;
                VariableTipoBusqueda = TipoBusqueda.Encuestas;
                pModelo.PageName = UtilIdiomas.GetText("COMMON", "ENCUESTAS");
            }
            else if (RequestParams("debates") != null && RequestParams("debates").Equals("true"))
            {
                TipoPagina = TiposPagina.Debates;
                VariableTipoBusqueda = TipoBusqueda.Debates;
                pModelo.PageName = UtilIdiomas.GetText("COMMON", "DEBATES");
            }
            else if (RequestParams("dafos") != null && RequestParams("dafos").Equals("true"))
            {
                TipoPagina = TiposPagina.Dafos;
                VariableTipoBusqueda = TipoBusqueda.Dafos;
                pModelo.PageName = UtilIdiomas.GetText("COMMON", "ENCUESTAS");
            }
            else if (RequestParams("blogs") != null && RequestParams("blogs").Equals("true"))
            {
                TipoPagina = TiposPagina.Blogs;
                VariableTipoBusqueda = TipoBusqueda.Blogs;
                pModelo.PageName = UtilIdiomas.GetText("COMMON", "BLOGS");
            }
            else if (RequestParams("verblog") != null && RequestParams("verblog").Equals("true"))
            {
                TipoPagina = TiposPagina.Blogs;
                VariableTipoBusqueda = TipoBusqueda.ArticuloBlogs;
                pModelo.PageName = UtilIdiomas.GetText("COMMON", "BUSCARARTICULOS");
            }
            else if (RequestParams("comunidades") != null && RequestParams("comunidades").Equals("true"))
            {
                TipoPagina = TiposPagina.Comunidades;
                VariableTipoBusqueda = TipoBusqueda.Comunidades;
                pModelo.PageName = UtilIdiomas.GetText("COMMON", "COMUNIDADES");
            }
            else if (ProyectoPestanyaActual != null && ProyectoPestanyaActual.FilaProyectoPestanyaBusqueda != null)
            {
                TipoPagina = TiposPagina.Semanticos;
                VariableTipoBusqueda = TipoBusqueda.Recursos;
            }

            if (TipoPagina.Equals(TiposPagina.Home))
            {
                if (nombrePestaña == "busqueda")
                {
                    VariableTipoBusqueda = TipoBusqueda.BusquedaAvanzada;
                }
                else
                {
                    VariableTipoBusqueda = TipoBusqueda.Recursos;
                }
            }

            if (ProyectoPestanyaActual != null && !string.IsNullOrEmpty(ProyectoPestanyaActual.Nombre))
            {
                pModelo.PageName = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }
            if (ProyectoPestanyaActual != null && ProyectoPestanyaActual.FilaProyectoPestanyaBusqueda != null)
            {
                mFilaPestanyaBusqueda = ProyectoPestanyaActual.FilaProyectoPestanyaBusqueda;
            }
            if (ProyectoPestanyaActual != null && !string.IsNullOrEmpty(ProyectoPestanyaActual.Titulo))
            {
                pModelo.PageTittle = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }

            UbicacionBusqueda = "Particular";

            if ((ProyectoSeleccionado.Clave.Equals(ProyectoAD.MyGnoss))
                && !(RequestParams("comunidades") != null && RequestParams("comunidades").Equals("true"))
                && !(RequestParams("blogs") != null && RequestParams("blogs").Equals("true"))
                && !(RequestParams("perorg") != null && RequestParams("perorg").Equals("true"))
                && !(RequestParams("verblog") != null && RequestParams("verblog").Equals("true")))
            {
                UbicacionBusqueda = "MyGnoss";
            }
            else if ((RequestParams("perorg") != null && RequestParams("perorg").Equals("true")) || RequestParams("PersonasOrganizaciones") != null && RequestParams("PersonasOrganizaciones").Equals("true"))
            {
                UbicacionBusqueda = "PersonaInicial";
            }
            else if ((RequestParams("Recursos") == null)
                && (RequestParams("PersonasOrganizaciones") == null)
                && (RequestParams("preguntas") == null)
                && (RequestParams("debates") == null)
                && (RequestParams("encuestas") == null)
                && (RequestParams("dafos") == null))
            {
                UbicacionBusqueda = "Meta";
            }
        }

        /// <summary>
        /// Genera los parámetros adicionales de la búsqueda según la pestaña actual y los filtros
        /// ya aplicados
        /// </summary>
        /// <param name="pOrder">Orden por defecto a aplicar según la página o búsqueda</param>
        /// <returns>Devuelve los parametros adicionales a enviar al servicio resultados y facetas</returns>
        private string ObtenerParametroAdicionalBusqueda(string pOrder)
        {
            string parametroadicional = "";

            if (mFilaPestanyaBusqueda != null)
            {
                parametroadicional = mFilaPestanyaBusqueda.CampoFiltro;
            }

            if (VariableTipoBusqueda.Equals(TipoBusqueda.Contribuciones))
            {
                if (TipoPagina.Equals(TiposPagina.Borradores))
                {
                    parametroadicional = $"gnoss:hasEstado=Borrador|{parametroadicional}";
                }
                else
                {
                    parametroadicional = $"gnoss:hasEstado=Publicado%20/%20Compartido|{parametroadicional}";
                }
            }

            if (EsEcosistemaSinMetaProyecto && ProyectoVirtual != null)
            {
                parametroadicional = $"proyectoVirtualID={ProyectoVirtual.Clave}|{parametroadicional}";
            }

            if (mFilaPestanyaBusqueda != null)
            {
                parametroadicional = $"PestanyaActualID={mFilaPestanyaBusqueda.PestanyaID}|{parametroadicional}";
            }
            else if (ProyectoPestanyaActual != null)
            {
                parametroadicional = $"PestanyaActualID={ProyectoPestanyaActual.Clave}|{parametroadicional}";
            }

            if (mFilaPestanyaBusqueda != null && mFilaPestanyaBusqueda.ProyectoOrigenID.HasValue)
            {
                parametroadicional = $"proyectoOrigenID={mFilaPestanyaBusqueda.ProyectoOrigenID.ToString().ToLower()}|{parametroadicional}";
            }
            else if (VariableTipoBusqueda == TipoBusqueda.BusquedaAvanzada && ProyectoOrigenBusquedaID != Guid.Empty)
            {
                parametroadicional = ObtenerParametrosExtraBusquedaConProyOrigen();
                parametroadicional = $"proyectoOrigenID={ProyectoOrigenBusquedaID.ToString().ToLower()}|{parametroadicional}";
            }

            #region Configuracion Orden

            if (!string.IsNullOrEmpty(pOrder) && RequestParams("filtroContexto") == null)
            {
                string cleanOrder = pOrder;

                if (cleanOrder.Contains("|"))
                {
                    cleanOrder = pOrder.Split('|')[0];

                    if (pOrder.Split('|')[1].ToLower() == "asc")
                    {
                        parametroadicional += "|orden=asc";
                    }
                }

                parametroadicional += $"|ordenarPor={cleanOrder}";
            }

            #endregion

            return parametroadicional;
        }

        private int CargarResultadosYFacetas(SearchViewModel pModelo, Guid pProyectoID, bool pEsMyGnoss, bool pEstaEnProyecto, bool EsUsuarioInvitado, Guid pIdentidadID, string pArgumentos, bool pEsPrimeraCarga, bool pAdminQuiereVerTodasPersonas, TipoBusqueda pTipoBusqueda, string pUrlPaginaBusqueda, string pGrafo, string pParametroAdicional, bool pHayParametrosBusqueda, string pUbicacionBusqueda, string pLanguageCode, bool pUsarMasterParaLectura, Guid pTokenAfinidad)
        {
            int numResultados = 0;
            bool llamarServicioFacetasYResultados = true;

            if ((RequestParams("perorg") != null && RequestParams("perorg").Equals("true")) || (RequestParams("PersonasOrganizaciones") != null && RequestParams("PersonasOrganizaciones").Equals("true")))
            {
                if (EsUsuarioInvitado)
                {
                    llamarServicioFacetasYResultados = false;
                }
            }

            if (mFilaPestanyaBusqueda != null && mFilaPestanyaBusqueda.OmitirCargaInicialFacetasResultados)
            {
                llamarServicioFacetasYResultados = false;
            }

            if (llamarServicioFacetasYResultados)
            {
                Stopwatch sw = null;
                mLoggingService.AgregarEntrada("Llamada a los servicios de resultados y facetas");

                CargadorResultados cargadorResultados = new CargadorResultados();
                cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();

                CargadorFacetas cargadorFacetas = new CargadorFacetas();
                cargadorFacetas.Url = mConfigService.ObtenerUrlServicioFacetas();

                int numeroFacetas = 1;
                if (Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json") && (Comunidad.EntornoEsPro || Comunidad.EntornoEsPre))
                {
                    numeroFacetas = 0;
                }

                Elementos.ServiciosGenerales.ProyectoPestanyaMenu pestanyaActual = null;
                if (mFilaPestanyaBusqueda != null)
                {
                    pestanyaActual = new Elementos.ServiciosGenerales.ProyectoPestanyaMenu(mFilaPestanyaBusqueda.ProyectoPestanyaMenu, ProyectoSeleccionado.GestorProyectos, mLoggingService, mEntityContext);
                }
                if (pestanyaActual != null && !TieneAccesoAPestanya(pestanyaActual))
                {
                    if (!string.IsNullOrEmpty(pestanyaActual.HTMLAlternativo))
                    {
                        pModelo.HTMLResourceList = pestanyaActual.HTMLAlternativo;
                    }
                    else
                    {
                        RedireccionarAPaginaNoEncontrada();
                    }
                }
                else
                {
                    Thread hiloResultados = new Thread(() =>
                    {
                        try
                        {
                            sw = LoggingService.IniciarRelojTelemetria();
                            string jsonResultados = cargadorResultados.CargarResultados(pProyectoID, pIdentidadID, EsUsuarioInvitado, pUrlPaginaBusqueda, pUsarMasterParaLectura, pAdminQuiereVerTodasPersonas, pTipoBusqueda, pGrafo, pParametroAdicional, pArgumentos, pEsPrimeraCarga, pLanguageCode, -1, "", pTokenAfinidad, Request);
                            mLoggingService.AgregarEntradaDependencia("Llamar CargarResultados", false, "BusquedaController.CargarResultadosYFacetas", sw, true);

                            try
                            {
                                bool jsonRequest = Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json");
                                if (jsonRequest)
                                {
                                    string[] divideJson = jsonResultados.Split(new string[] { "{ComienzoJsonViewData}" }, StringSplitOptions.None);
                                    string jsonModel = divideJson[0];
                                    string jsonViewData = divideJson[1];

                                    ResultadoModel resultadoModel = new ResultadoModel();
                                    using (MemoryStream input = new MemoryStream(Convert.FromBase64String(jsonModel)))
                                    using (DeflateStream deflateStream = new DeflateStream(input, CompressionMode.Decompress))
                                    using (MemoryStream output = new MemoryStream())
                                    {
                                        deflateStream.CopyTo(output);
                                        deflateStream.Close();
                                        output.Seek(0, SeekOrigin.Begin);
                                        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                                        {
                                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                                            TypeNameHandling = TypeNameHandling.All
                                        };

                                        resultadoModel = JsonConvert.DeserializeObject<ResultadoModel>((string)System.Text.Json.JsonSerializer.Deserialize(output, typeof(string)), jsonSerializerSettings);
                                    }

                                    // Deserializamos ViewData
                                    JsonSerializerSettings jsonSerializerSettingsVB = new JsonSerializerSettings
                                    {
                                        TypeNameHandling = TypeNameHandling.All,
                                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                                    };
                                    Dictionary<string, object> ViewDataDeserializado = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonViewData, jsonSerializerSettingsVB);
                                    foreach (string item in ViewDataDeserializado.Keys)
                                    {
                                        if (ViewData[item] == null)
                                        {
                                            ViewData.Add(item, ViewDataDeserializado[item]);
                                        }

                                    }

                                    pModelo.JSONResourceList = resultadoModel;
                                    numResultados = resultadoModel.NumeroResultadosTotal;
                                }
                                else
                                {
                                    KeyValuePair<int, string> respuestaResultados = JsonConvert.DeserializeObject<KeyValuePair<int, string>>(jsonResultados);

                                    numResultados = respuestaResultados.Key;

                                    pModelo.HTMLResourceList = respuestaResultados.Value;
                                }
                            }
                            catch
                            {
                                pModelo.HTMLResourceList = jsonResultados;
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                mLoggingService.AgregarEntradaDependencia("Llamar CargarResultados", false, "BusquedaController.CargarResultadosYFacetas", sw, false);
                                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                            }
                            catch (Exception) { }
                        }
                    });

                    Thread hiloFacetas = new Thread(() =>
                    {
                        try
                        {
                            sw = LoggingService.IniciarRelojTelemetria();

                            string resultado = cargadorFacetas.CargarFacetas(pProyectoID, pEstaEnProyecto, EsUsuarioInvitado, pIdentidadID, pArgumentos, pUbicacionBusqueda, (!pHayParametrosBusqueda), pLanguageCode, pAdminQuiereVerTodasPersonas, pTipoBusqueda, numeroFacetas, null, pGrafo, pParametroAdicional, "", pUrlPaginaBusqueda, pUsarMasterParaLectura, pTokenAfinidad, Request);

                            bool jsonRequest = Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json") && !Comunidad.EntornoEsPro && !Comunidad.EntornoEsPre;
                            if (jsonRequest)
                            {
                                string[] divideJson = resultado.Split(new string[] { "{ComienzoJsonViewData}" }, StringSplitOptions.None);
                                string jsonFaceted = divideJson[0];
                                string jsonViewData = divideJson[1];
                                //Deserializamos las Facetas con la configuracion adecuada
                                FacetedModel facetedModel = new FacetedModel();
                                using (MemoryStream input = new MemoryStream(Convert.FromBase64String(jsonFaceted)))
                                using (DeflateStream deflateStream = new DeflateStream(input, CompressionMode.Decompress))
                                using (MemoryStream output = new MemoryStream())
                                {
                                    deflateStream.CopyTo(output);
                                    deflateStream.Close();
                                    output.Seek(0, SeekOrigin.Begin);


                                    facetedModel = System.Text.Json.JsonSerializer.Deserialize<FacetedModel>(output);
                                    pModelo.JSONFaceted = facetedModel;
                                }

                                // Deserializamos ViewData
                                JsonSerializerSettings jsonSerializerSettingsVB = new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All,
                                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                                };
                                Dictionary<string, object> ViewDataDeserializado = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonViewData, jsonSerializerSettingsVB);
                                foreach (string item in ViewDataDeserializado.Keys)
                                {
                                    if (ViewData[item] == null)
                                    {
                                        ViewData.Add(item, ViewDataDeserializado[item]);
                                    }
                                }
                            }
                            else
                            {
                                pModelo.HTMLFaceted = resultado;
                            }
                            mLoggingService.AgregarEntradaDependencia("Llamar CargarFacetas", false, "BusquedaController.CargarResultadosYFacetas", sw, true);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                mLoggingService.AgregarEntradaDependencia("Llamar CargarFacetas", false, "BusquedaController.CargarResultadosYFacetas", sw, false);
                                GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);

                            }
                            catch (Exception) { }
                        }
                    });

                    //Lanzamos y esperamos a las facetas y los resultados
                    if (hiloResultados != null)
                    {
                        hiloResultados.Start();
                    }
                    hiloFacetas.Start();
                    if (hiloResultados != null)
                    {
                        hiloResultados.Join();
                    }
                    hiloFacetas.Join();
                }

                mLoggingService.AgregarEntrada("Fin llamada a los servicios de resultados y facetas");
            }

            return numResultados;
        }

        /// <summary>
        /// Agrega el mapa a la página.
        /// </summary>
        private void AgregarMapaALaPagina()
        {
            StringBuilder sb = new StringBuilder();

            string apiKey = mConfigService.ObtenerClaveApiGoogle();

            if (!string.IsNullOrEmpty(apiKey))
            {
                apiKey = $"key={apiKey}";
            }

            ViewBag.ListaJS.Add($"//www.google.com/jsapi?{apiKey}");
            ViewBag.ListaJS.Add("defer/" + BaseURLStatic + "/jsNuevo/markerclusterer.js");

            sb.AppendLine($"google.load(\"maps\", \"3\",  {{other_params:\"sensor=false&{apiKey}\"}});");


            if (!string.IsNullOrEmpty(mFilaPestanyaBusqueda.PosicionCentralMapa))
            {
                //Provincia, region, ID Google Fusion Tables, Where
                //La Rioja|ES|1txq6kRDLPz4iy4XVax58F-xHbuZuKapoRtGNuzby|Provincia = 'La Rioja'
                //string[] parametrosPosicion = "La Rioja|ES|1txq6kRDLPz4iy4XVax58F-xHbuZuKapoRtGNuzby|Provincia = \'La Rioja\'".Split('|');
                string[] parametrosPosicion = mFilaPestanyaBusqueda.PosicionCentralMapa.Split('|');
                string address = null;
                string region = "ES";
                string IDMap = null;
                string filtroWhere = null;

                if (parametrosPosicion[0] != "")
                {
                    address = parametrosPosicion[0];
                }
                if (parametrosPosicion[1] != "")
                {
                    region = parametrosPosicion[1];
                }
                if (parametrosPosicion.Length > 3)
                {
                    if (parametrosPosicion[2] != "")
                    {
                        IDMap = parametrosPosicion[2];
                    }
                    if (parametrosPosicion[3] != "")
                    {
                        filtroWhere = parametrosPosicion[3].Replace("'", "\\'");
                    }
                }
                sb.AppendLine($"utilMapas.EstablecerParametrosMapa('{address}', '{region}', '{IDMap}', '{filtroWhere}');");
            }

            FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
            DataWrapperFacetas facetaDW = facetaCN.ObtenerPropsMapaPerYOrgProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, VariableTipoBusqueda);
            facetaCN.Dispose();

            if (facetaDW.ListaFacetaConfigProyMapa.Count == 1)
            {
                string filtroCoordMapaLat = facetaDW.ListaFacetaConfigProyMapa.FirstOrDefault().PropLatitud;
                string filtroCoordMapaLong = facetaDW.ListaFacetaConfigProyMapa.FirstOrDefault().PropLongitud;

                foreach (string key in InformacionOntologias.Keys)
                {
                    if (key.StartsWith("@"))
                    {
                        filtroCoordMapaLat = filtroCoordMapaLat.Replace(key.Substring(1), InformacionOntologias[key][0] + ":");
                        filtroCoordMapaLong = filtroCoordMapaLong.Replace(key.Substring(1), InformacionOntologias[key][0] + ":");
                    }
                }
                sb.AppendLine("utilMapas.EstablecerFiltroCoordMapa('" + filtroCoordMapaLat + "','" + filtroCoordMapaLong + "');");
            }

            ViewBag.JSMapa += sb.ToString();
        }

        public bool TieneAccesoAPestanya(Elementos.ServiciosGenerales.ProyectoPestanyaMenu pPestanyaActual)
        {
            bool usuarioSinAcceso = false;
            switch (pPestanyaActual.Privacidad)
            {
                case TipoPrivacidadPagina.Especial:
                    if (!(ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado))
                    {
                        //publicas                    
                        if (mControladorBase.UsuarioActual == null || mControladorBase.UsuarioActual.EsIdentidadInvitada)
                        {
                            usuarioSinAcceso = true;
                        }
                    }
                    break;
                case TipoPrivacidadPagina.Lectores:
                    bool identidadActualEsPerfilEditor = false;
                    foreach (Guid perfilID in pPestanyaActual.ListaRolIdentidad.Keys)
                    {
                        if (IdentidadActual != null && IdentidadActual.PerfilID == perfilID)
                        {
                            identidadActualEsPerfilEditor = true;
                        }
                    }

                    if (!identidadActualEsPerfilEditor)
                    {
                        bool identidadActualPerteneceAAlgunGrupo = false;
                        if (IdentidadActual != null)
                        {
                            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                            IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.Clave, true));
                            if (IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalCorporativo || IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalPersonal)
                            {
                                IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.IdentidadMyGNOSS.Clave, true));
                            }
                            IdentidadActual.GestorIdentidades.CargarGrupos();
                            identidadCN.Dispose();

                            foreach (Guid grupoID in pPestanyaActual.ListaRolGrupoIdentidades.Keys)
                            {
                                if (IdentidadActual.GestorIdentidades.ListaGrupos.ContainsKey(grupoID))
                                {
                                    identidadActualPerteneceAAlgunGrupo = true;
                                }
                            }
                        }

                        if (!identidadActualPerteneceAAlgunGrupo)
                        {
                            usuarioSinAcceso = true;
                        }
                    }
                    break;
            }

            return !usuarioSinAcceso;
        }

        /// <summary>
        /// Carga la identidad que se va a pintar en la pagina
        /// </summary>
        private void CargarIdentidadPagina(bool pSoloActivas)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            Guid identidadID = IdentidadActual.Clave;
            mExisteIdentidadPaginaContribuciones = false;

            Guid destinatarioID = Guid.Empty;

            if (RequestParams("nombreCortoPerfil") != null)
            {
                mExisteIdentidadPaginaContribuciones = true;
                string nombreCortoOrg = "";
                if (RequestParams("nombreCortoOrganizacion") != null)
                {
                    nombreCortoOrg = RequestParams("nombreCortoOrganizacion");
                }

                Guid[] listaIDs = identidadCN.ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(RequestParams("nombreCortoPerfil"), ProyectoSeleccionado.Clave, nombreCortoOrg, false);

                if (listaIDs != null && !listaIDs[0].Equals(Guid.Empty))
                {
                    identidadID = listaIDs[0];
                }
                else
                {
                    mExisteIdentidadPaginaContribuciones = false;
                }
            }
            else if (RequestParams("nombreCortoOrganizacion") != null)
            {
                mExisteIdentidadPaginaContribuciones = true;
                identidadID = identidadCN.ObtenerIdentidadIDDeOrganizacionEnProyecto(RequestParams("nombreCortoOrganizacion"), ProyectoSeleccionado.Clave);
            }
            else if (!string.IsNullOrEmpty(RequestParams("organizacion")) && IdentidadActual.IdentidadOrganizacion != null)
            {
                mExisteIdentidadPaginaContribuciones = true;
                identidadID = identidadCN.ObtenerIdentidadIDDeOrganizacionEnProyecto(IdentidadActual.IdentidadOrganizacion.NombreCorto, ProyectoSeleccionado.Clave);
            }

            if (mExisteIdentidadPaginaContribuciones.Value)
            {
                DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerIdentidadPorID(identidadID, pSoloActivas);
                dataWrapperIdentidad.Merge(identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(identidadID));
                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = dataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(identidadID));

                //Si no hay fila, hay que consultar BD Master por si acaso no está en una réplica:
                if (filaIdentidad == null)
                {
                    dataWrapperIdentidad = identidadCN.ObtenerIdentidadPorID(identidadID, pSoloActivas);
                    filaIdentidad = dataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(identidadID));
                }

                if (filaIdentidad != null)
                {
                    DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
                    //Si no es una organización ni un perfil corporativo, cargamos la persona.
                    if (filaIdentidad.Tipo != (short)TiposIdentidad.Organizacion && filaIdentidad.Tipo != (short)TiposIdentidad.ProfesionalCorporativo)
                    {
                        PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                        dataWrapperPersona = personaCN.ObtenerPersonasPorIdentidad(identidadID);
                        personaCN.Dispose();
                    }
                    GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);

                    DataWrapperOrganizacion organizacionDW = new DataWrapperOrganizacion();
                    //Si es una organización o un perfil profesional, cargamos también los datos de la organización
                    if (filaIdentidad.Tipo != (short)TiposIdentidad.Personal && filaIdentidad.Tipo != (short)TiposIdentidad.Profesor)
                    {
                        OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                        organizacionDW = organizacionCN.ObtenerOrganizacionesPorIdentidad(identidadID);
                        organizacionCN.Dispose();
                        if (filaIdentidad.Perfil.OrganizacionID.HasValue)
                        {
                            dataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadDeOrganizacion(filaIdentidad.Perfil.OrganizacionID.Value, ProyectoSeleccionado.Clave, true));
                        }

                        GestionOrganizaciones gestorOrganizaciones = new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext);
                        GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, gestorPersonas, gestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                        IdentidadPaginaContribuciones = gestorIdentidades.ListaIdentidades[identidadID];
                    }
                }
            }
        }

        #endregion

        #region Propiedades

        private Identidad IdentidadPaginaContribuciones
        {
            get
            {
                if (mIdentidadPaginaContribuciones == null && !mExisteIdentidadPaginaContribuciones.HasValue)
                {
                    CargarIdentidadPagina(true);
                }
                return mIdentidadPaginaContribuciones;
            }
            set
            {
                mIdentidadPaginaContribuciones = value;
            }
        }

        private string UbicacionBusqueda { get; set; }

        /// <summary>
        /// Obtiene la lista de items que se obtendrá de la búsqueda (recursos, personas, debates...)
        /// </summary>
        protected List<string> ListaItemsBusqueda
        {
            get
            {
                if (mListaItems == null)
                {
                    mListaItems = new List<string>();
                    if ((RequestParams("Recursos") != null && RequestParams("Recursos").Equals("true")) || (mFilaPestanyaBusqueda != null))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_RECURSOS);

                        if (mFilaPestanyaBusqueda != null)
                        {
                            string semanticos = mFilaPestanyaBusqueda.CampoFiltro;
                            char[] separadores = { '|' };
                            foreach (string tipo in semanticos.Split(separadores, StringSplitOptions.RemoveEmptyEntries))
                            {
                                mListaItems.Add(tipo.Replace("rdf:type=", ""));
                            }
                        }
                    }
                    else if (RequestParams("RecursosPerfil") != null && RequestParams("RecursosPerfil").Equals("true"))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_RECURSOS);
                    }
                    else if ((RequestParams("perorg") != null && RequestParams("perorg").Equals("true")) || RequestParams("PersonasOrganizaciones") != null && RequestParams("PersonasOrganizaciones").Equals("true"))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_ORGANIZACION);
                        mListaItems.Add(FacetadoAD.BUSQUEDA_CLASE);
                        mListaItems.Add(FacetadoAD.BUSQUEDA_PERSONA);
                    }
                    else if (RequestParams("preguntas") != null && RequestParams("preguntas").Equals("true"))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_PREGUNTAS);
                    }
                    else if (RequestParams("encuestas") != null && RequestParams("encuestas").Equals("true"))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_ENCUESTAS);
                    }
                    else if (RequestParams("debates") != null && RequestParams("debates").Equals("true"))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_DEBATES);
                    }
                    else if (RequestParams("dafos") != null && RequestParams("dafos").Equals("true"))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_DAFOS);
                    }
                    else if (RequestParams("blogs") != null && RequestParams("blogs").Equals("true"))
                    {
                        //si esta en blogs en mygnoss
                        mListaItems.Add(FacetadoAD.BUSQUEDA_BLOGS);
                    }
                    else if (RequestParams("verblog") != null && RequestParams("verblog").Equals("true"))
                    {
                        //si esta en blogs en mygnoss
                        mListaItems.Add("ArticuloBlog");
                    }
                    else if (RequestParams("comunidades") != null && RequestParams("comunidades").Equals("true"))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_COMUNIDADES);
                    }

                    else if (RequestParams("contribuciones") != null && RequestParams("contribuciones").Equals("true"))
                    {
                        mListaItems.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_PUBLICADO);
                        mListaItems.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMPARTIDO);
                        mListaItems.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS);
                        mListaItems.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_PREGUNTA);
                        mListaItems.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_DEBATE);
                        mListaItems.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_FACTORDAFO);
                        mListaItems.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_ENCUESTA);
                    }
                    else
                    {
                        //if estoy en metabuscador 
                        if (ListaFiltrosFacetas.ContainsKey("rdf:type") && ListaFiltrosFacetas["rdf:type"] != null && ListaFiltrosFacetas["rdf:type"].Count > 0)
                        {
                            foreach (string s in ListaFiltrosFacetas["rdf:type"]) { mListaItems.Add(s); }
                        }

                        if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MyGnoss))
                        {
                            //si esta en mygnoss
                            mListaItems.Add("MyGnoss");
                        }
                        else //esta en comunidad
                        {
                            mListaItems.Add(FacetadoAD.BUSQUEDA_AVANZADA);
                        }
                    }
                }
                return mListaItems;
            }
        }

        /// <summary>
        /// Filtro de orden configurados.
        /// </summary>
        public Dictionary<string, string> FiltrosOrdenConfig
        {
            get
            {
                if (mFiltrosOrdenConfig == null)
                {
                    mFiltrosOrdenConfig = new Dictionary<string, string>();
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    DataWrapperProyecto dataWrapperProyecto = proyCL.ObtenerFiltrosOrdenesDeProyecto(ProyectoSeleccionado.Clave);
                    proyCL.Dispose();

                    if (mFilaPestanyaBusqueda != null)
                    {
                        foreach (ProyectoPestanyaFiltroOrdenRecursos filaFiltroOrd in dataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos.Where(proy => proy.PestanyaID.Equals(mFilaPestanyaBusqueda.PestanyaID)).OrderBy(proy => proy.Orden).ToList())
                        {
                            mFiltrosOrdenConfig.Add(filaFiltroOrd.FiltroOrden, UtilCadenas.ObtenerTextoDeIdioma(filaFiltroOrd.NombreFiltro, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto));
                        }
                    }
                }

                return mFiltrosOrdenConfig;
            }
        }

        /// <summary>
        /// Lista de filtros establecidos desde las facetas (solo en los lugares en los que aparezcan facetas)
        /// </summary>
        public virtual Dictionary<string, List<string>> ListaFiltrosFacetas
        {
            get
            {
                if (mListaFiltrosFacetas.Count == 0)
                {
                    if (RequestParams("tag") != null)
                    {
                        List<string> listaTags = new List<string>();
                        listaTags.Add(GnossUrlsSemanticas.ObtenerTagBusqueda(RequestParams("tag"), false));

                        mListaFiltrosFacetas.Add("sioc_t:Tag", listaTags);
                    }
                    else if (RequestParams("sioc_t:tag") != null)
                    {
                        List<string> listaTags = new List<string>();
                        string tags = RequestParams("sioc_t:tag");
                        string[] delimiter = { "," };
                        string[] etiquetas = tags.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string etiqueta in etiquetas)
                        {
                            if (!listaTags.Contains(etiqueta))
                            {
                                listaTags.Add(etiqueta);
                            }
                        }

                        mListaFiltrosFacetas.Add("sioc_t:Tag", listaTags);
                    }
                    else if (RequestParams("skos:ConceptID") != null)
                    {
                        List<string> listaTags = new List<string>();
                        string categorias = RequestParams("skos:ConceptID");
                        string[] delimiter = { "," };
                        string[] cats = categorias.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string cat in cats)
                        {
                            if (!listaTags.Contains(cat))
                            {
                                listaTags.Add(cat);
                            }
                        }

                        mListaFiltrosFacetas.Add("skos:ConceptID", listaTags);
                    }
                    else if (RequestParams("categoria") != null)
                    {
                        List<string> listaTags = new List<string>();
                        listaTags.Add("gnoss:" + RequestParams("categoria"));
                        mListaFiltrosFacetas.Add("skos:ConceptID", listaTags);
                    }
                    else if (!string.IsNullOrEmpty(RequestParams("searchInfo")))
                    {
                        string[] delimiter = { "/" };
                        string[] parametros = RequestParams("searchInfo").Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (parametros.Length > 1 && parametros[0] == UtilIdiomas.GetText("URLSEM", "TAG"))
                        {
                            List<string> listaTags = new List<string>();
                            listaTags.Add(parametros[1]);
                            mListaFiltrosFacetas.Add("sioc_t:Tag", listaTags);
                        }
                    }
                    else if (RequestParams("search") != null)
                    {
                        List<string> listaTags = new List<string>();
                        if (RequestParams("search").Contains("<script"))
                        {
                            listaTags.Add(UtilCadenas.HtmlEncode(RequestParams("search")));
                        }
                        else
                        {
                            listaTags.Add(RequestParams("search"));
                        }

                        mListaFiltrosFacetas.Add("search", listaTags);
                    }

                    if (!string.IsNullOrEmpty(RequestParams("tag")))
                    {
                        List<string> listaTags = new List<string>();
                        listaTags.Add(GnossUrlsSemanticas.ObtenerTagBusqueda(RequestParams("tag"), false));

                        mListaFiltrosFacetas.Add("search", listaTags);
                    }
                }
                return mListaFiltrosFacetas;
            }
        }

        private DataWrapperProyecto ProyectoDW
        {
            get
            {
                if (mProyectoDW == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mProyectoDW = proyCN.ObtenerProyectoPorIDConNiveles(ProyectoSeleccionado.Clave);
                    proyCN.Dispose();
                }
                return mProyectoDW;
            }
        }

        #endregion

    }
}
