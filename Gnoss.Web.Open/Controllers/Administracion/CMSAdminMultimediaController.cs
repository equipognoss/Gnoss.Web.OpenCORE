using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Es.Riam.Web.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class CMSAdminMultimediaController : ControllerBaseWeb
    {
        public CMSAdminMultimediaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        private GestionCMS mGestorCMS = null;
        private List<string> mExtensionesImagenesPermitidas = null;
        private List<string> mExtensionesDocumentosPermitidos = null;

        #endregion

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Index(string pagina, string search, string extension, string numusos)
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            if (!ParametrosGeneralesRow.CMSDisponible)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADGENERAL"));
            }

            Comunidad.Tabs.Clear();
            CommunityModel.TabModel pestanyaEdicionPagina = new CommunityModel.TabModel();
            pestanyaEdicionPagina.Name = UtilIdiomas.GetText("COMADMINCMS", "EDICIONDEPAGINAS");
            pestanyaEdicionPagina.Url = UtilIdiomas.GetText("URLSEM", "ADMINISTRARCOMUNIDADCMS");
            Comunidad.Tabs.Add(pestanyaEdicionPagina);
            CommunityModel.TabModel pestanyaEdicionComponentes = new CommunityModel.TabModel();
            pestanyaEdicionComponentes.Name = UtilIdiomas.GetText("COMADMINCMS", "EDICIONDECOMPONENTES");
            pestanyaEdicionComponentes.Url = UtilIdiomas.GetText("URLSEM", "ADMINISTRARCOMUNIDADCMSLISTADOCOMPONENTES");
            Comunidad.Tabs.Add(pestanyaEdicionComponentes);
            CommunityModel.TabModel pestanyaEdicionMultimedia = new CommunityModel.TabModel();
            pestanyaEdicionMultimedia.Name = UtilIdiomas.GetText("COMADMINCMS", "EDICIONDEMULTIMEDIA");
            pestanyaEdicionMultimedia.Url = UtilIdiomas.GetText("URLSEM", "ADMINISTRARCOMUNIDADCMSMULTIMEDIA");
            pestanyaEdicionMultimedia.Active = true;
            Comunidad.Tabs.Add(pestanyaEdicionMultimedia);

            //Si es callback
            if (!string.IsNullOrEmpty(RequestParams("callback")))
            {
                return ActionCallback();
            }

            if (Request.Method.Equals("POST") && Request.Form.Files.Count > 0)
            {
                CargarFichero();
            }

            int paginaInt = 1;
            int.TryParse(pagina, out paginaInt);
            if (paginaInt < 1)
            {
                paginaInt = 1;
            }

            CargarModelo(paginaInt, search, extension, numusos);

            return View();
        }

        public void CargarFichero()
        {
            bool error = false;
            IFormFileCollection hfc = Request.Form.Files;
            for (int i = 0; i < hfc.Count; i++)
            {
                IFormFile fichero = hfc[i];

                if (fichero != null && !string.IsNullOrEmpty(fichero.FileName) && fichero.Length > 0)
                {
                    byte[] buffer1;
                    FileInfo file = new FileInfo(fichero.FileName);
                    string extensionArchivo = Path.GetExtension(file.Name).ToLower();

                    if ((ExtensionesImagenesPermitidas.Contains(extensionArchivo)) || (ExtensionesDocumentosPermitidos.Contains(extensionArchivo)))
                    {
                        string nombre = Path.GetFileNameWithoutExtension(file.Name).ToLower();
                        BinaryReader reader = new BinaryReader(fichero.OpenReadStream());
                        buffer1 = reader.ReadBytes((int)fichero.Length);
                        string ruta = UtilArchivos.ContentImagenesProyectos + "/personalizacion/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "/cms/";

                        if (ExtensionesImagenesPermitidas.Contains(extensionArchivo))
                        {
                            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                            servicioImagenes.Url = UrlIntragnossServicios;

                            string primeroDisponible = servicioImagenes.ObtenerNombreDisponible(ruta + nombre + extensionArchivo);
                            string NombrePrimeroDisponible = primeroDisponible.Substring(0, primeroDisponible.LastIndexOf("."));
                            string ExtensionPrimeroDisponible = primeroDisponible.Substring(primeroDisponible.LastIndexOf("."));

                            servicioImagenes.AgregarImagen(buffer1.ToArray(), ruta + NombrePrimeroDisponible, ExtensionPrimeroDisponible);

                            InformarCambioAdministracionCMS("ObjetosMultimedia", Convert.ToBase64String(buffer1), fichero.FileName);
                        }
                        else if (ExtensionesDocumentosPermitidos.Contains(extensionArchivo))
                        {
                            bool exito = false;
                            try
                            {
                                string rutaFichero = $"{UtilArchivos.ContentImagenes}/{ruta}";
                                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                                servicioImagenes.Url = UrlIntragnossServicios;
                                exito = servicioImagenes.AgregarFichero(buffer1.ToArray(), nombre, extensionArchivo, rutaFichero);
                                InformarCambioAdministracionCMS("ObjetosMultimedia", Convert.ToBase64String(buffer1), fichero.FileName);
                            }
                            catch (Exception ex)
                            {
                                GuardarLogError($"{ex.Message} \r\nPila: {ex.StackTrace}");
                            }

                            error = !exito;
                        }
                    }
                    else
                    {
                        error = true;
                    }
                }
            }
            if (error)
            {
                ViewBag.TextoSubida = "Las extensiones admitidas son .jpg .jpeg .png y .gif";
            }
            else
            {
                ViewBag.TextoSubida = "Subida correcta";
            }
        }

        public void CargarModelo(int pPagina, string pSearch, string pExtension, string pNumusos)
        {
            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
            servicioImagenes.Url = UrlIntragnossServicios;
            string ruta = UtilArchivos.ContentImagenesProyectos + "/personalizacion/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "/cms/";
            string[] nombres = servicioImagenes.ObtenerIDsImagenesPorNombreImagen(ruta, pSearch);

            SortedDictionary<string, string> listaResultados = new SortedDictionary<string, string>();
            if (nombres != null)
            {
                foreach (string nombre in nombres)
                {
                    if (string.IsNullOrEmpty(pExtension) || nombre.EndsWith(pExtension))
                    {
                        listaResultados.Add(nombre, BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + ruta + nombre);
                    }
                }
            }

            SortedDictionary<string, List<CMSComponente>> listaComponentesItem = new SortedDictionary<string, List<CMSComponente>>();
            var listaPropiedadesImagen = gestorCMS.CMSDW.ListaCMSPropiedadComponente.Where(item => item.TipoPropiedadComponente.Equals((short)TipoPropiedadCMS.Imagen));
            var listaPropiedadesHtml = gestorCMS.CMSDW.ListaCMSPropiedadComponente.Where(item => item.TipoPropiedadComponente.Equals((short)TipoPropiedadCMS.HTML));
            foreach (string nombre in listaResultados.Keys)
            {
                listaComponentesItem.Add(nombre, new List<CMSComponente>());
                var propiedadesDeImagen = listaPropiedadesImagen.Where(item => item.ValorPropiedad.Contains(listaResultados[nombre]));
                foreach (var propiedad in propiedadesDeImagen)
                {
                    var componente = gestorCMS.ListaComponentes[propiedad.ComponenteID];
                    if (!listaComponentesItem[nombre].Contains(componente))
                    {
                        listaComponentesItem[nombre].Add(componente);
                    }
                }

                var propiedadesHtml = listaPropiedadesHtml.Where(item => item.ValorPropiedad.Contains(listaResultados[nombre]));
                foreach (var propiedad in propiedadesHtml)
                {
                    var componente = gestorCMS.ListaComponentes[propiedad.ComponenteID];
                    if (!listaComponentesItem[nombre].Contains(componente))
                    {
                        listaComponentesItem[nombre].Add(componente);
                    }
                }
            }


            //SortedDictionary<string, List<CMSComponente>> listaComponentesItem = new SortedDictionary<string, List<CMSComponente>>();
            //Dictionary<string, string> dicIdiomas = Conexion.ObtenerListaIdiomas();
            //foreach (string nombre in listaResultados.Keys)
            //{
            //    listaComponentesItem.Add(nombre, new List<CMSComponente>());
            //    foreach (CMSComponente componente in gestorCMS.ListaComponentes.Values)
            //    {
            //        if (componente.PropiedadesComponente.ContainsKey(TipoPropiedadCMS.Imagen))
            //        {
            //            foreach (string idioma in dicIdiomas.Keys)
            //            {
            //                if (listaResultados[nombre].ToLower() == UtilCadenas.ObtenerTextoDeIdioma(componente.PropiedadesComponente[TipoPropiedadCMS.Imagen], idioma, null).ToLower())
            //                {
            //                    if (!listaComponentesItem[nombre].Contains(componente))
            //                    {
            //                        listaComponentesItem[nombre].Add(componente);
            //                    }
            //                }
            //            }
            //        }
            //        if (componente.PropiedadesComponente.ContainsKey(TipoPropiedadCMS.HTML))
            //        {
            //            if (componente.PropiedadesComponente[TipoPropiedadCMS.HTML].ToLower().Contains(listaResultados[nombre].ToLower()))
            //            {
            //                if (!listaComponentesItem[nombre].Contains(componente))
            //                {
            //                    listaComponentesItem[nombre].Add(componente);
            //                }
            //            }
            //        }
            //    }
            //}

            SortedDictionary<string, List<short>> listaPaginasItem = new SortedDictionary<string, List<short>>();
            foreach (string nombre in listaResultados.Keys)
            {
                listaPaginasItem.Add(nombre, new List<short>());

                foreach (CMSBloque bloque in gestorCMS.ListaBloques.Values)
                {
                    foreach (string atributo in bloque.Atributos.Keys)
                    {
                        if (bloque.Atributos[atributo].ToLower() == listaResultados[nombre].ToLower())
                        {
                            if (!listaPaginasItem[nombre].Contains(bloque.TipoUbicacion))
                            {
                                listaPaginasItem[nombre].Add(bloque.TipoUbicacion);
                            }
                            break;
                        }
                    }
                }
            }

            foreach (string item in listaComponentesItem.Keys)
            {
                if (!string.IsNullOrEmpty(pNumusos))
                {
                    if ((listaComponentesItem[item].Count + listaPaginasItem[item].Count).ToString() != pNumusos)
                    {
                        listaResultados.Remove(item);
                    }
                }
            }


            ResultadoModel resultado = CargarResultado(listaResultados, pPagina);
            List<FacetModel> facetas = CargarFacetas(listaResultados, pExtension, pNumusos, listaComponentesItem, listaPaginasItem);
            List<FacetItemModel> filtros = CargarFiltros(listaResultados, pSearch, pExtension, pNumusos);

            ViewBag.Resultado = resultado;
            ViewBag.Facetas = facetas;
            ViewBag.Filtros = filtros;
            ViewBag.ListaComponentesItem = listaComponentesItem;
            ViewBag.ListaPaginasItem = listaPaginasItem;
            ViewBag.ExtensionesImagenes = ExtensionesImagenesPermitidas;
        }

        private ResultadoModel CargarResultado(SortedDictionary<string, string> pListaResultados, int pPaginaFiltro)
        {
            ResultadoModel resultado = new ResultadoModel();
            resultado.ListaResultados = new List<ObjetoBuscadorModel>();

            int i = 1;
            foreach (string multimedia in pListaResultados.Keys)
            {
                if (i > (pPaginaFiltro - 1) * 10 && i <= (pPaginaFiltro) * 10)
                {
                    CMSMultimediaModel ficha = new CMSMultimediaModel();
                    ficha.Title = multimedia;
                    ficha.Link = pListaResultados[multimedia];
                    resultado.ListaResultados.Add(ficha);
                }
                i++;
            }

            resultado.NumeroPaginaActual = pPaginaFiltro;
            resultado.NumeroResultadosPagina = 10;
            resultado.NumeroResultadosTotal = pListaResultados.Count;
            return resultado;
        }

        private List<FacetModel> CargarFacetas(SortedDictionary<string, string> pListaResultados, string pExt, string pUsos, SortedDictionary<string, List<CMSComponente>> pListaComponentesItem, SortedDictionary<string, List<short>> pListaPaginasItem)
        {
            List<FacetModel> facetas = new List<FacetModel>();
            #region Extension
            FacetModel facetaExtension = new FacetModel();
            facetas.Add(facetaExtension);
            facetaExtension.Name = "Extension";
            facetaExtension.FacetItemList = new List<FacetItemModel>();
            Dictionary<string, int> extensiones = new Dictionary<string, int>();
            foreach (string nombre in pListaResultados.Keys)
            {
                string extension = nombre.Substring(nombre.LastIndexOf("."));
                if (extensiones.ContainsKey(extension))
                {
                    extensiones[extension]++;
                }
                else
                {
                    extensiones.Add(extension, 1);
                }
            }

            foreach (string extension in extensiones.Keys)
            {
                int numComponentes = extensiones[extension];

                FacetItemModel facetaItem = new FacetItemModel();
                facetaItem.Name = extension;
                facetaItem.Number = numComponentes;
                facetaItem.Filter = "extension=" + extension + "";

                if (pExt == extension)
                {
                    facetaItem.Selected = true;
                }
                facetaExtension.FacetItemList.Add(facetaItem);
            }
            #endregion

            #region usos

            FacetModel facetaUsos = new FacetModel();
            facetas.Add(facetaUsos);
            facetaUsos.Name = "Número de usos";
            facetaUsos.FacetItemList = new List<FacetItemModel>();

            Dictionary<int, int> numUsos = new Dictionary<int, int>();
            foreach (string item in pListaComponentesItem.Keys)
            {
                int numUso = pListaComponentesItem[item].Count + pListaPaginasItem[item].Count;
                if (!numUsos.ContainsKey(numUso))
                {
                    numUsos.Add(numUso, 0);
                }
                numUsos[numUso]++;
            }
            foreach (int usos in numUsos.Keys)
            {
                if (string.IsNullOrEmpty(pUsos) || pUsos == usos.ToString())
                {
                    int numComponentes = numUsos[usos];

                    FacetItemModel facetaItem = new FacetItemModel();
                    facetaItem.Name = usos.ToString();
                    facetaItem.Number = numComponentes;
                    facetaItem.Filter = "numusos=" + usos + "";
                    if (pUsos == usos.ToString())
                    {
                        facetaItem.Selected = true;
                    }
                    facetaUsos.FacetItemList.Add(facetaItem);
                }
            }

            #endregion

            return facetas;
        }

        private List<FacetItemModel> CargarFiltros(SortedDictionary<string, string> pListaResultados, string pSearch, string pExt, string pNumUsos)
        {
            List<FacetItemModel> filtros = new List<FacetItemModel>();

            if (!string.IsNullOrEmpty(pSearch))
            {
                FacetItemModel facetaItem = new FacetItemModel();
                facetaItem.Name = pSearch;
                facetaItem.Filter = "search=" + pSearch;
                filtros.Add(facetaItem);
            }
            //Extension
            if (!string.IsNullOrEmpty(pExt))
            {
                FacetItemModel facetaItem = new FacetItemModel();
                facetaItem.Name = pExt;
                facetaItem.Filter = "extension=" + pExt;
                filtros.Add(facetaItem);
            }
            //NumUsos
            if (!string.IsNullOrEmpty(pNumUsos))
            {
                FacetItemModel facetaItem = new FacetItemModel();
                facetaItem.Name = pNumUsos;
                facetaItem.Filter = "numusos=" + pNumUsos;
                filtros.Add(facetaItem);
            }

            return filtros;
        }

        #region Propiedades

        public GestionCMS gestorCMS
        {
            get
            {
                if (mGestorCMS == null)
                {
                    CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mGestorCMS = new GestionCMS(CMSCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    CMSCN.Dispose();
                }
                return mGestorCMS;
            }
        }

        public List<string> ExtensionesImagenesPermitidas
        {
            get
            {
                if (mExtensionesImagenesPermitidas == null)
                {
                    mExtensionesImagenesPermitidas = new List<string>();
                    ParametroAplicacionCL paramCL = null;
                    paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    //ParametroAplicacionDS paramDS = ((ParametroAplicacionDS)paramCL.ObtenerParametrosAplicacion());
                    List<AD.EntityModel.ParametroAplicacion> paramDS = paramCL.ObtenerParametrosAplicacionPorContext();
                    bool extensionesConfiguradas = false;
                    //DataRow[] filasExtensiones = paramDS.ParametroAplicacion.Select("Parametro = 'ExtensionesImagenesCMSMultimedia'");
                    List<AD.EntityModel.ParametroAplicacion> filasExtensiones = paramDS.Where(parametro => parametro.Parametro.Equals("ExtensionesImagenesCMSMultimedia")).ToList();

                    if (filasExtensiones.Count > 0)
                    {
                        string[] separador = new string[] { "&&&" };
                        string[] extensionesImagenes = filasExtensiones.First().Valor.ToString().Split(separador, StringSplitOptions.RemoveEmptyEntries);

                        if (extensionesImagenes != null && extensionesImagenes.Length > 0)
                        {
                            extensionesConfiguradas = true;
                            foreach (string extension in extensionesImagenes)
                            {
                                mExtensionesImagenesPermitidas.Add(extension);
                            }
                        }
                    }

                    if (!extensionesConfiguradas)
                    {
                        mExtensionesImagenesPermitidas.Add(".jpg");
                        mExtensionesImagenesPermitidas.Add(".jpeg");
                        mExtensionesImagenesPermitidas.Add(".png");
                        mExtensionesImagenesPermitidas.Add(".gif");
                    }
                }
                return mExtensionesImagenesPermitidas;
            }
        }

        public List<string> ExtensionesDocumentosPermitidos
        {
            get
            {
                if (mExtensionesDocumentosPermitidos == null)
                {
                    mExtensionesDocumentosPermitidos = new List<string>();
                    ParametroAplicacionCL paramCL = null;
                    paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    // ParametroAplicacionDS paramDS = ((ParametroAplicacionDS)paramCL.ObtenerParametrosAplicacion());
                    List<AD.EntityModel.ParametroAplicacion> paramDS = paramCL.ObtenerParametrosAplicacionPorContext();
                    bool extensionesConfiguradas = false;
                    // DataRow[] filasExtensiones = paramDS.ParametroAplicacion.Select("Parametro = 'ExtensionesDocumentosCMSMultimedia'");
                    List<AD.EntityModel.ParametroAplicacion> filasExtensiones = paramDS.Where(parametro => parametro.Parametro.Equals("ExtensionesDocumentosCMSMultimedia")).ToList();

                    if (filasExtensiones.Count > 0)
                    {
                        string[] separador = new string[] { "&&&" };
                        string[] extensionesDocumentos = filasExtensiones.First().Valor.ToString().Split(separador, StringSplitOptions.RemoveEmptyEntries);

                        if (extensionesDocumentos != null && extensionesDocumentos.Length > 0)
                        {
                            foreach (string extension in extensionesDocumentos)
                            {
                                mExtensionesDocumentosPermitidos.Add(extension);
                            }
                        }
                    }

                    if (!extensionesConfiguradas)
                    {
                        mExtensionesDocumentosPermitidos.Add(".pdf");
                        mExtensionesDocumentosPermitidos.Add(".txt");
                        mExtensionesDocumentosPermitidos.Add(".doc");
                        mExtensionesDocumentosPermitidos.Add(".docx");
                    }
                }
                return mExtensionesDocumentosPermitidos;
            }
        }

        #endregion

        #region Callback

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Diseño })]
        public ActionResult ActionCallback()
        {
            string action = RequestParams("callback");

            if (action.ToLower() == "eliminarcomponentemultimedia")
            {
                #region Eliminar componente multimedia
                string nombreComponente = RequestParams("nombreComponente");

                CargarModelo(1, "", "", "");

                if ((!ViewBag.ListaComponentesItem.ContainsKey(nombreComponente) || ViewBag.ListaComponentesItem[nombreComponente].Count == 0)
                    && (!ViewBag.ListaPaginasItem.ContainsKey(nombreComponente) || ViewBag.ListaPaginasItem[nombreComponente].Count == 0))
                {
                    ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                    servicioImagenes.Url = UrlIntragnossServicios;
                    String ruta = UtilArchivos.ContentImagenesProyectos + "/personalizacion/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "/cms/";
                    servicioImagenes.BorrarImagen(ruta + nombreComponente);
                    InformarCambioAdministracionCMS("ObjetosMultimedia", "null", nombreComponente);
                    string mensaje = "OK";
                    return Content(mensaje);
                }
                else
                {
                    string mensaje = "";
                    if (ViewBag.ListaComponentesItem.ContainsKey(nombreComponente) && ViewBag.ListaComponentesItem[nombreComponente].Count > 0)
                    {
                        mensaje = "Los siguientes componentes están utilizando este multimedia y no se puede borrar: ";
                        mensaje += "<ul>";
                        foreach (CMSComponente componente in ViewBag.ListaComponentesItem[nombreComponente])
                        {
                            string enlaceEdicion = mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARCOMPONENTE") + "/" + componente.Clave.ToString();
                            mensaje += "<li><a href=\"" + enlaceEdicion + "\">" + componente.Nombre.ToString() + "<a></li>";
                        }
                        mensaje += "</ul>";
                    }
                    if (ViewBag.ListaPaginasItem.ContainsKey(nombreComponente) && ViewBag.ListaPaginasItem[nombreComponente].Count > 0)
                    {
                        mensaje = "Las siguientes páginas están utilizando este multimedia y no se puede borrar: ";
                        mensaje += "<ul>";
                        foreach (short pagina in ViewBag.ListaPaginasItem[nombreComponente])
                        {
                            CMSPagina paginaCMS = gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave][pagina];
                            string enlaceEdicion = mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARPAGINA") + "/" + pagina.ToString();

                            string nombre = "";

                            if (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaCMS.Any(proy => proy.Ubicacion.Equals(pagina)))
                            {
                                AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaCMS filaPestanya = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaCMS.FirstOrDefault(proy => proy.Ubicacion.Equals(pagina));
                                nombre = UtilCadenas.ObtenerTextoDeIdioma(filaPestanya.ProyectoPestanyaMenu.Nombre, UtilIdiomas.LanguageCode, null);
                            }
                            else if (string.IsNullOrEmpty(paginaCMS.Nombre))
                            {
                                nombre = UtilIdiomas.GetText("COMADMINCMS", "PAGINA_" + paginaCMS.TipoUbicacion.ToString());
                            }
                            mensaje += "<li><a href=\"" + enlaceEdicion + "\">" + nombre + "<a></li>";
                        }
                        mensaje += "</ul>";
                    }
                    return Content(mensaje);
                }
                #endregion
            }
            return new EmptyResult();
        }

        #endregion

    }
}
