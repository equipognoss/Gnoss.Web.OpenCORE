using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.CMS;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class HomeComunidadController : ControllerPestanyaBase
    {

        public HomeComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        //public ActionResult ShowMoreActivity(int TypeActivity, int NumPeticion)
        //{
        //    ActividadRecienteController actividadRecienteController = new ActividadRecienteController(this);
        //    return PartialView("ControlesMVC/_ActividadReciente", actividadRecienteController.ObtenerActividadReciente(NumPeticion, 10, (TipoActividadReciente)TypeActivity, null, false));
        //}


        [TypeFilter(typeof(AccesoPestanyaAttribute))]
        public ActionResult Index()
        {
            if (ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Reservado))
            {
                AsignarMetaRobots("NOINDEX, NOFOLLOW");
            }

            if (!string.IsNullOrEmpty(RequestParams("callback")))
            {
                if (RequestParams("callback").ToLower() == "ActividadReciente|MostrarMas".ToLower())
                {
                    throw new Exception("Esto ya no se hace aqui. LLamar a 'load-more-activity'");
                    
                }
                else if (RequestParams("callback").ToLower() == "CargarListaAccionesRecursos".ToLower())
                {
                    throw new Exception("Esto ya no se hace aqui. LLamar a 'load-resource-actions'");
                }
                if (RequestParams("callback").ToLower() == "CargarGadgets".ToLower())
                {
                    //Seguridad : Dentro
                    return Index_Gadgets();
                }
                else if (RequestParams("callback").ToLower() == "paginadorgadget")
                {
                    //Seguridad : Dentro
                    Guid gadgetID = new Guid(RequestParams("gadgetid"));
                    int numPagina = int.Parse(RequestParams("numPagina"));

                    GadgetController gadgetControllerPaginado = new GadgetController(this, IdentidadActual, mHttpContextAccessor, mLoggingService, mGnossCache, mConfigService, mVirtuosoAD, mEntityContext, mRedisCacheWrapper, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
                    return gadgetControllerPaginado.CargarGadgetHome(gadgetID, numPagina);
                }

                return new EmptyResult();
            }

            bool redirigirAIdioma = false;

            if (!string.IsNullOrEmpty(RouteConfig.NombreProyectoSinNombreCorto))
            {
                Dictionary<string, string> listaIdiomas = mConfigService.ObtenerListaIdiomasDictionary();

                string idioma = UtilIdiomas.LanguageCode;
                bool emptyPath = string.IsNullOrEmpty(Request.Path) || Request.Path.Equals("/");

                if (!EsBot && !Request.Headers.ContainsKey("Referer") && Request.Headers.ContainsKey("accept-language") && emptyPath)
                {
                    var languages = Request.Headers["accept-language"].ToString().Split(',')
                        .Select(System.Net.Http.Headers.StringWithQualityHeaderValue.Parse)
                        .OrderByDescending(s => s.Quality.GetValueOrDefault(1));

                    foreach (var userLanguaje in languages)
                    {
                        string idiomaNavegador = userLanguaje.Value.Split('-')[0];
                        if (listaIdiomas.ContainsKey(idiomaNavegador))
                        {
                            idioma = idiomaNavegador;
                            if (UtilIdiomas.LanguageCode != idioma)
                            {
                                UtilIdiomas = new UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService);
                                redirigirAIdioma = true;
                            }
                            break;
                        }
                    }
                }

            }

            AD.EntityModel.ParametroAplicacion filaParametroRedireccion = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("UrlHome"));

            if (filaParametroRedireccion != null)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + filaParametroRedireccion.Valor);
            }

            if (redirigirAIdioma)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            if (ParametrosGeneralesRow.CMSDisponible)
            {
                ActionResult respuesta = Index_HomeCMS();
                if (respuesta != null)
                {
                    return respuesta;
                }
            }

            string parametroadicional = string.Empty;
            if (ProyectoOrigenBusquedaID != Guid.Empty)
            {
                parametroadicional = "proyectoOrigenID=" + ProyectoOrigenBusquedaID.ToString().ToLower() + "|";
            }

            insertarScriptBuscador("", "", "", "", "", "", ProyectoSeleccionado.Clave.ToString(), "", "MontarFacetas('', true, 8, '#panFacetas', null);", "", parametroadicional, "", "", "", "primeraCargaDeFacetas = false;", Guid.Empty, ProyectoSeleccionado.Clave, "homeCatalogoParticular", 0, null, null);

            if (ProyectoSeleccionado.EsCatalogo)
            {
                return Index_HomeCatalogo();
            }
            else
            {
                return Index_Home();
            }            
        }

        private ActionResult Index_HomeCMS()
        {
            CMSCL cmsCL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionCMS gestorCMS = new GestionCMS(cmsCL.ObtenerConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
            cmsCL.Dispose();

            bool mostrarCMS = false;

            TipoUbicacionCMS tipoUbicacion = TipoUbicacionCMS.HomeProyecto;

            if (!mostrarCMS)
            {
                tipoUbicacion = TipoUbicacionCMS.HomeProyecto;
                if (gestorCMS.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave) && gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave].ContainsKey((short)tipoUbicacion))
                {
                    mostrarCMS = gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)tipoUbicacion].Activa;
                }

                if (!mostrarCMS)
                {
                    tipoUbicacion = TipoUbicacionCMS.HomeProyectoNoMiembro;
                    if (!mControladorBase.UsuarioActual.EsIdentidadInvitada)
                    {
                        tipoUbicacion = TipoUbicacionCMS.HomeProyectoMiembro;
                    }
                    if (gestorCMS.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave) && gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave].ContainsKey((short)tipoUbicacion))
                    {
                        mostrarCMS = gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)tipoUbicacion].Activa;
                    }
                }
            }

            if (mostrarCMS)
            {
                ViewBag.CargandoPaginaCMS = true;
                ControladorCMS controladorCMS = new ControladorCMS(this, null, (short)tipoUbicacion, Comunidad, mHttpContextAccessor, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
                List<CMSBlock> listaBloquesHome = controladorCMS.CargarBloquesPaginaCMS();
                if (listaBloquesHome != null)
                {
                    return ViewCMSPagina(listaBloquesHome);
                }

                //CMSComponent fichaComponente = null;

                //try
                //{
                //    fichaComponente = controladorCMS.CargarComponente(true, false, IdiomaUsuario);
                //    PintarComponenteCMS(fichaComponente, IdiomaUsuario);
                //}
                //catch (Exception ex)
                //{
                //    string idComponente = "HOME";
                //    if (fichaComponente != null)
                //    {
                //        idComponente = fichaComponente.Key.ToString();
                //    }

                //    GuardarLogError("Error producido en el componente con ID='" + idComponente + "' en la comunidad " + ProyectoSeleccionado.Nombre + " \n " + mLoggingService.DevolverCadenaError(ex, Version));
                //}

            }

            return null;
        }

        private ActionResult Index_HomeCatalogo()
        {
            HomeCatalogoViewModel paginaModel = new HomeCatalogoViewModel();
            StringBuilder mensaje = new StringBuilder();

            try
            {
                mensaje.AppendLine("Entro en Index HomeCatalogo");
                paginaModel.Sections = new List<HomeCatalogoViewModel.SectionHome>();

                //FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService);
                //facetaCL.ObtenerOntologiasProyecto(configuracionDS, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);

                //Dictionary<string, string> informacionOntologias = new Dictionary<string, string>();
                //List<string> listaFormSemanticos = new List<string>();
                //mensaje.AppendLine("Creo listaFormSemanticos");
                //foreach (FacetaDS.OntologiaProyectoRow myrow in configuracionDS.OntologiaProyecto.Rows)
                //{
                //    listaFormSemanticos.Add(myrow.OntologiaProyecto);

                //    if (!string.IsNullOrEmpty(myrow.OntologiaProyecto) && !string.IsNullOrEmpty(myrow.Namespace))
                //        informacionOntologias.Add(myrow.OntologiaProyecto, myrow.Namespace);
                //}


                Dictionary<string, List<string>> informacionOntologias = UtilServiciosFacetas.ObtenerInformacionOntologias(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                List<string> listaFormSemanticos = informacionOntologias.Keys.ToList(); 

                mensaje.AppendLine("Obtengo SeccionesHomeCatalogoDeProyecto");
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                DataWrapperProyecto dataWrapperProyeco = proyCL.ObtenerSeccionesHomeCatalogoDeProyecto(ProyectoSeleccionado.Clave);
                mensaje.AppendLine("Obtenidas");
                if (dataWrapperProyeco != null && dataWrapperProyeco.ListaSeccionProyCatalogo.Count > 0)
                {
                    mensaje.AppendLine("Hay SeccionProyCatalogo");
                    //Select("OrganizacionID='" + ProyectoSeleccionado.FilaProyecto.OrganizacionID + "' AND ProyectoID='" + ProyectoSeleccionado.Clave + "'", "Orden Asc");
                    List<SeccionProyCatalogo> filasSeccion = dataWrapperProyeco.ListaSeccionProyCatalogo.Where(seccion=>seccion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && seccion.ProyectoID.Equals(ProyectoSeleccionado.Clave)).OrderBy(seccion=>seccion.Orden).ToList();

                    Dictionary<Guid, List<Guid>> listaRecursos = new Dictionary<Guid, List<Guid>>();
                    Dictionary<int, KeyValuePair<string, List<Guid>>> listaRecursosSeccion = new Dictionary<int, KeyValuePair<string, List<Guid>>>();
                    Dictionary<Guid, List<ResourceModel>> listaRecursosModel = new Dictionary<Guid, List<ResourceModel>>();

                    mensaje.AppendLine("Empiezo foreach secciones");

                    foreach (SeccionProyCatalogo filaSeccion in filasSeccion)
                    {
                        mensaje.AppendLine("Cargo Seccion Home");
                        KeyValuePair<string, List<Guid>> resultadosSeccion = CargarSeccionHome(filaSeccion, informacionOntologias, listaFormSemanticos);

                        mensaje.AppendLine("resultadosSeccion obtenidos");

                        listaRecursosSeccion.Add(filaSeccion.Orden, resultadosSeccion);
                        mensaje.AppendLine("antes if containskey");
                        if (!listaRecursos.ContainsKey(filaSeccion.ProyectoBusquedaID))
                        {
                            mensaje.AppendLine("entro if containskey");
                            listaRecursos.Add(filaSeccion.ProyectoBusquedaID, new List<Guid>());
                        }
                        mensaje.AppendLine("antes addrange");
                        listaRecursos[filaSeccion.ProyectoBusquedaID].AddRange(resultadosSeccion.Value);
                        mensaje.AppendLine("addrange hecho");
                    }
                    mensaje.AppendLine("empiezo foreach listarecursos");
                    foreach (Guid proyectoID in listaRecursos.Keys)
                    {
                        Stopwatch sw = null;
                        try
                        {
                            CargadorResultados cargadorResultadosContexto = new CargadorResultados();
                            cargadorResultadosContexto.Url = mConfigService.ObtenerUrlServicioResultados();
                            string parametros = "listadoRecursosEstatico:";
                            foreach (Guid id in listaRecursos[proyectoID])
                            {
                                parametros += id + ",";
                            }

                            Guid identidadActualID = UsuarioAD.Invitado;
                            bool esUsuarioInvitado = false;
                            sw = LoggingService.IniciarRelojTelemetria();
                            listaRecursosModel.Add(proyectoID, cargadorResultadosContexto.CargarResultadosContexto(proyectoID, parametros, false, UtilIdiomas.LanguageCode, TipoBusqueda.Recursos, 0, proyectoID.ToString(), "", "", EsBot, false, "", "", "", false, "", identidadActualID, esUsuarioInvitado));
                            mLoggingService.AgregarEntradaDependencia("Llamar al servicio de resultados", false, "Index_HomeCatalogo", sw, true);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.AgregarEntradaDependencia($"Error al obtener contextos para el proyecto {proyectoID}", false, "Index_HomeCatalogo", sw, true);
                            GuardarLogError(ex, string.Format("Error al obtener contextos para el proyecto {0}", proyectoID));
                        }
                    }
                    mensaje.AppendLine("empiezo foreach filasseccion");
                    foreach (SeccionProyCatalogo filaSeccion in filasSeccion)
                    {
                        try
                        {
                            HomeCatalogoViewModel.SectionHome seccion = new HomeCatalogoViewModel.SectionHome();
                            seccion.Resources = new List<ResourceModel>();

                            KeyValuePair<string, List<Guid>> resultadosSeccion = listaRecursosSeccion[filaSeccion.Orden];

                            seccion.Title = filaSeccion.Nombre;
                            if (!string.IsNullOrEmpty(resultadosSeccion.Key))
                            {
                                seccion.Title += resultadosSeccion.Key;
                            }

                            seccion.ViewType = (HomeCatalogoViewModel.SectionHome.ResourceViewType)filaSeccion.Tipo;

                            foreach (Guid resultadoID in resultadosSeccion.Value)
                            {
                                seccion.Resources.Add(listaRecursosModel[filaSeccion.ProyectoBusquedaID].Find(resource => resource.Key == resultadoID));
                            }

                            paginaModel.Sections.Add(seccion);
                        }
                        catch (Exception ex)
                        {
                            GuardarLogError(ex, string.Format("Error al intentar generar el modelo para la seccion de orden {0}. ", filaSeccion.Orden));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, mensaje.ToString());
            }

            return View("Catalogo", paginaModel);
        }

        private KeyValuePair<string, List<Guid>> CargarSeccionHome(SeccionProyCatalogo pFilaSeccion, Dictionary<string, List<string>> pInformacionOntologias, List<string> pListaFormulariosSem)
        {
            string tituloFiltro = "";

            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InformacionOntologias = pInformacionOntologias;
            FacetadoDS facetadoDS = null;

            //facetadoCL.FacetaDS = CargarFacetasProyecto();

            string filtro = "";
            if (string.IsNullOrEmpty(pFilaSeccion.Filtro) || (pFilaSeccion.Filtro.Contains("=") && !pFilaSeccion.Filtro.Contains(pFilaSeccion.Faceta)))
            {
                //Si no hay filtro, cargo la faceta completa y elijo un valor aleatoriamente. 
                facetadoDS = facetadoCL.ObtenerFaceta(pFilaSeccion.Faceta, pFilaSeccion.ProyectoBusquedaID, pListaFormulariosSem, true, false, mControladorBase.UsuarioActual.EsIdentidadInvitada, mControladorBase.UsuarioActual.IdentidadID, mControladorBase.UsuarioActual.EsUsuarioInvitado);

                if (facetadoDS.Tables[pFilaSeccion.Faceta].Rows.Count > 0)
                {
                    Random random = new Random((int)DateTime.Now.Ticks);
                    int indice = random.Next(facetadoDS.Tables[pFilaSeccion.Faceta].Rows.Count);
                    filtro = (string)facetadoDS.Tables[pFilaSeccion.Faceta].Rows[indice][0];
                    filtro = filtro.Replace("http://gnoss/", "gnoss:");
                    tituloFiltro = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(filtro);
                }

                if (!string.IsNullOrEmpty(pFilaSeccion.Filtro))
                {
                    filtro = pFilaSeccion.Faceta + "=" + filtro + "|" + pFilaSeccion.Filtro;
                }
            }
            else
            {
                filtro = pFilaSeccion.Filtro;
            }
                    
            List<Guid> listaElementos = new List<Guid>();

            if (!string.IsNullOrEmpty(filtro))
            {
                //Busco los recursos que hay que mostrar
                facetadoDS = facetadoCL.ObtenerRecursosDeFiltroPorFaceta(pFilaSeccion.ProyectoBusquedaID, pFilaSeccion.Faceta, filtro, pFilaSeccion.NumeroResultados, pListaFormulariosSem, IdentidadActual.Clave, IdentidadActual.EsIdentidadInvitada, UsuarioActual.EsUsuarioInvitado);

                if (facetadoDS != null && facetadoDS.Tables["RecursosBusqueda"].Rows.Count > 0)
                {
                    foreach (DataRow filaResultado in facetadoDS.Tables["RecursosBusqueda"].Rows)
                    {
                        Guid id;
                        
                        if(Guid.TryParse(((string)filaResultado[0]).Replace("http://gnoss/", ""), out id))
                        {
                            if (!listaElementos.Contains(id))
                            {
                                listaElementos.Add(id);
                            }
                        }
                    }
                }
            }

            return new KeyValuePair<string, List<Guid>>(tituloFiltro, listaElementos);
        }

        /// <summary>
        /// Cargamos las facetas del proyecto, necesario para saber si la configuración es multiidioma.
        /// </summary>
        /// <returns>Devuelve un DS con todoas las facetas del proyecto.</returns>
        private DataWrapperFacetas CargarFacetasProyecto()
        {
            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperFacetas facetaDW = new DataWrapperFacetas();

            facetaDW = facetaCL.ObtenerFacetasDeProyecto(null, ProyectoSeleccionado.Clave, true);

            facetaCL.Dispose();

            return facetaDW;
        }

        private ActionResult Index_Home()
        {
            HomeViewModel paginaModel = new HomeViewModel();

            //ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD);

            //int? numRecursos = proyCL.ObtenerContadorComunidad(ProyectoSeleccionado.Clave, TipoBusqueda.Recursos);
            //if (!numRecursos.HasValue) { numRecursos = ContadoresProyecto.NumeroRecursos; }

            //int? numPerYOrg = proyCL.ObtenerContadorComunidad(ProyectoSeleccionado.Clave, TipoBusqueda.PersonasYOrganizaciones);
            //if (!numPerYOrg.HasValue) { numPerYOrg = ContadoresProyecto.NumeroMiembros + ContadoresProyecto.NumeroOrgRegistradas; }

            //paginaModel.NumberOfResources = numRecursos.Value;
            //paginaModel.NumberOfPersonOrganizations = numPerYOrg.Value;

            ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

            List<Identidad> ListaAdministradores = controladorProyecto.CargarAdministradoresProyecto(ProyectoSeleccionado);

            Dictionary<Identidad, int> ListaIdentidadesMasActivas = controladorProyecto.CargarPersonasProyecto(ProyectoSeleccionado, 16, false);

            Dictionary<Identidad, int> ListaUtimasIdentidades = controladorProyecto.CargarPersonasProyecto(ProyectoSeleccionado, 4, true);

            List<Guid> listaIds = new List<Guid>();
            foreach (Identidad identidad in ListaIdentidadesMasActivas.Keys)
            {
                if (!listaIds.Contains(identidad.Clave))
                {
                    listaIds.Add(identidad.Clave);
                }
            }
            foreach (Identidad identidad in ListaUtimasIdentidades.Keys)
            {
                if (!listaIds.Contains(identidad.Clave))
                {
                    listaIds.Add(identidad.Clave);
                }
            }

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> listaFotosIds = identCN.ObtenerSiListaIdentidadesTienenFoto(listaIds);
            identCN.Dispose();

            paginaModel.MostActiveUsers = new List<ProfileModel>();
            foreach (Identidad identidad in ListaIdentidadesMasActivas.Keys)
            {
                ProfileModel usuario = new ProfileModel();
                usuario.NamePerson = identidad.Nombre(IdentidadActual.Clave);
                usuario.UrlPerson = mControladorBase.UrlsSemanticas.GetURLPerfilPersonaOOrgEnProyecto(BaseURLIdioma, UtilIdiomas, UrlPerfil, identidad, ProyectoSeleccionado.NombreCorto);
                usuario.UrlFoto = obtenerImagen(listaFotosIds[identidad.Clave]);
                usuario.TypeProfile = (ProfileType)identidad.FilaIdentidad.Tipo;

                paginaModel.MostActiveUsers.Add(usuario);
            }

            paginaModel.LastUsers = new List<ProfileModel>();
            foreach (Identidad identidad in ListaUtimasIdentidades.Keys)
            {
                ProfileModel usuario = new ProfileModel();
                usuario.NamePerson = identidad.Nombre(IdentidadActual.Clave);
                usuario.UrlPerson = mControladorBase.UrlsSemanticas.GetURLPerfilPersonaOOrgEnProyecto(BaseURLIdioma, UtilIdiomas, UrlPerfil, identidad, ProyectoSeleccionado.NombreCorto);
                usuario.UrlFoto = obtenerImagen(listaFotosIds[identidad.Clave]);
                usuario.TypeProfile = (ProfileType)identidad.FilaIdentidad.Tipo;

                paginaModel.LastUsers.Add(usuario);
            }

            ActividadReciente actividadReciente = new ActividadReciente(mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
            paginaModel.RecentActivity = actividadReciente.ObtenerActividadReciente(1, 10, TipoActividadReciente.HomeProyecto, null, false);

            GadgetController gadgetController = new GadgetController(this, IdentidadActual, mHttpContextAccessor, mLoggingService, mGnossCache, mConfigService, mVirtuosoAD, mEntityContext, mRedisCacheWrapper, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
            paginaModel.Gadgets = gadgetController.CargarListaGadgetsHome(true, null);

            CommunityModel.TabModel pestanyaActiva = Comunidad.Tabs.Find(pestanya => pestanya.Url.ToLower().EndsWith((UtilIdiomas.GetText("URLSEM", "COMUNIDAD") + "/" + ProyectoSeleccionado.NombreCorto).ToLower()));

            if (pestanyaActiva != null)
            {
                pestanyaActiva.Active = true;
            }

            return View(paginaModel);
        }

        private ActionResult Index_Gadgets()
        {
            MultiViewResult result = new MultiViewResult(this, mViewEngine);

            if (RequestParams("gadgetsID") != null)
            {
                List<Guid> listaGadgetsSinCargar = new List<Guid>();
                List<GadgetModel> listaGadgets = new List<GadgetModel>();

                try
                {
                    string[] gadgets = RequestParams("gadgetsID").Split(',');
                    foreach (string gadget in gadgets)
                    {
                        if (gadget != "")
                        {
                            listaGadgetsSinCargar.Add(new Guid(gadget));
                        }
                        if (listaGadgetsSinCargar.Count >= 3)
                        {
                            break;
                        }
                    }

                    GadgetController gadgetController = new GadgetController(this, IdentidadActual, mHttpContextAccessor, mLoggingService, mGnossCache, mConfigService, mVirtuosoAD, mEntityContext, mRedisCacheWrapper, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
                    listaGadgets = gadgetController.CargarListaGadgetsHome(false, listaGadgetsSinCargar);
                }
                catch
                { }
                finally
                {
                    foreach (Guid gadgetID in listaGadgetsSinCargar)
                    {
                        bool tieneContenido = false;
                        GadgetModel gadgetModel = listaGadgets.Find(gadget => gadget.Key == gadgetID);

                        if (gadgetModel != null)
                        {
                            if (gadgetModel is GadgetResourceListModel && ((GadgetResourceListModel)gadgetModel).Resources.Count > 0)
                            {
                                tieneContenido = true;
                                result.AddView("ControlesMVC/_FichaGadget", "FichaGadget_" + gadgetID, gadgetModel);
                            }
                        }

                        if (!tieneContenido)
                        {
                            result.AddContent("", "FichaGadget_" + gadgetID);
                        }
                    }
                }
            }

            return result;
        }

        private string obtenerImagen(string pUrlFoto)
        {
            string urlImagen = "";

            if (!string.IsNullOrEmpty(pUrlFoto) && pUrlFoto.ToLower() != "sinfoto" && !pUrlFoto.ToLower().Contains("anonimo35_peque"))
            {
                if (!pUrlFoto.ToLower().StartsWith("/" + UtilArchivos.ContentImagenes))
                {
                    pUrlFoto = "/" + UtilArchivos.ContentImagenes + pUrlFoto;
                }
                urlImagen = pUrlFoto;
            }
            return urlImagen;
        }

        #region Propiedades

        /// <summary>
        /// Obtiene o establece si el usuario invitado puede ver esta página
        /// </summary>
        public override bool PaginaVisibleEnPrivada
        {
            get 
            {
                //La HOME es visible si el proyecto es público o el usuario participa en la comunidad
                return ProyectoSeleccionado.EsPublico || !mControladorBase.UsuarioActual.EsIdentidadInvitada;
            }
        }

        #endregion
    }
}
