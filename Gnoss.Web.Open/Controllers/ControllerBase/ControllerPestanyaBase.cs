using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Exportaciones;
using Es.Riam.Gnoss.Web.Controles.Facetas;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public abstract class ControllerPestanyaBase : ControllerBaseWeb
    {
        /// <summary>
        /// Gestor de tesauro
        /// </summary>
        protected GestionTesauro mGestorTesauroProyectoActual;
        private UtilSemCms mUtilSemCms;
        private SemCmsController mSemCmsController;
        private UtilExportaciones mUtilExportaciones;
        private UtilServiciosFacetas mUtilServiciosFacetas;
        private ControladorFacetas mControladorFacetas;

        protected ControllerPestanyaBase(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext, bool pOmitirRedireccionAPrimeraPagina)
        {
            if (ProyectoSeleccionado == null)
            {
                filterContext.Result = Redirect("/error?errorCode=404&lang=" + RequestParams("lang") + "&urlAcceso=" + "/comunidad/" + RequestParams("nombreProy"));
                return;
            }

            if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto) && ProyectoPestanyaActual == null && !pOmitirRedireccionAPrimeraPagina)
            {
                filterContext.Result = RedirigirAPrimeraPagina();
                return;
            }

            base.OnActionExecuting(filterContext);

            if (filterContext.Result != null)
            {
                return;
            }

            CargarComboBusqueda();

            if (ProyectoPestanyaActual != null)
            {
                ViewBag.BodyClassPestanya = ProyectoPestanyaActual.CSSBodyClassPestanya;                
                ViewBag.MetaDescriptionPestanya = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.MetaDescription, IdiomaUsuario, "es");
            }
        }

        protected UtilServiciosFacetas UtilServiciosFacetas
        {
            get
            {
                if(mUtilServiciosFacetas == null)
                {
                    mUtilServiciosFacetas = new UtilServiciosFacetas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
                return mUtilServiciosFacetas;
            }
        }

        protected UtilExportaciones UtilExportaciones
        {
            get
            {
                if(mUtilExportaciones == null)
                {
                    mUtilExportaciones = new UtilExportaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mServicesUtilVirtuosoAndReplication);
                }
                return mUtilExportaciones;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            OnActionExecuting(filterContext, false);
        }

        protected ControladorFacetas ControladorFacetas
        {
            get
            {
                if(mControladorFacetas == null)
                {
                    mControladorFacetas = new ControladorFacetas(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService);
                }
                return mControladorFacetas;
            }
        }

        protected UtilSemCms UtilSemCms
        {
            get
            {
                if(mUtilSemCms == null)
                {
                    mUtilSemCms = new UtilSemCms(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
                return mUtilSemCms;
            }
        }

        private ActionResult RedirigirAPrimeraPagina()
        {
            //if (ParametrosGeneralesRow.PestanyaRecursosVisible)
            //{
            //    Response.Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "RECURSOS"));
            //}
            if (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
            {
                return new RedirectResult(BaseURLIdioma);
            }
            //Where("PestanyaPadreID IS NULL", "Orden ASC");
            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> filasPestanyas = ProyectoVirtual.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proy => !proy.PestanyaPadreID.HasValue).OrderBy(proy => proy.Orden).ToList();
            if (filasPestanyas.Count > 0)
            {
                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu fila in filasPestanyas)
                {
                    AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = fila;
                    if (!filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.EnlaceExterno))
                    {
                        string rutaPestanya = "";
                        switch (filaPestanya.TipoPestanya)
                        {
                            case (short)TipoPestanyaMenu.Indice:
                                rutaPestanya = UtilIdiomas.GetText("URLSEM", "INDICE");
                                break;
                            case (short)TipoPestanyaMenu.Recursos:
                                rutaPestanya = UtilIdiomas.GetText("URLSEM", "RECURSOS");
                                break;
                            case (short)TipoPestanyaMenu.Preguntas:
                                rutaPestanya = UtilIdiomas.GetText("URLSEM", "PREGUNTAS");
                                break;
                            case (short)TipoPestanyaMenu.Debates:
                                rutaPestanya = UtilIdiomas.GetText("URLSEM", "DEBATES");
                                break;
                            case (short)TipoPestanyaMenu.Encuestas:
                                rutaPestanya = UtilIdiomas.GetText("URLSEM", "ENCUESTAS");
                                break;
                            case (short)TipoPestanyaMenu.PersonasYOrganizaciones:
                                rutaPestanya = UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES");
                                break;
                            case (short)TipoPestanyaMenu.AcercaDe:
                                rutaPestanya = UtilIdiomas.GetText("URLSEM", "ACERCADE");
                                break;
                            case (short)TipoPestanyaMenu.BusquedaAvanzada:
                                rutaPestanya = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                                break;
                        }
                        if (!string.IsNullOrEmpty(filaPestanya.Ruta))
                        {
                            rutaPestanya = UtilCadenas.ObtenerTextoDeIdioma(filaPestanya.Ruta, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                        }
                        return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + rutaPestanya);
                    }
                }
            }
            else
            {
                if (!Request.Host.Value.Contains("depuracion") && !new Uri(ProyectoSeleccionado.UrlPropia(IdiomaUsuario)).Host.Equals(Request.Host))
                {
                    return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
            }
            return RedireccionarAPaginaNoEncontrada();
        }

        /// <summary>
        /// Carga los enlaces multiidiomas de la pestaña actual
        /// </summary>
        protected override void CargarEnlacesMultiIdioma()
        {
            if (ProyectoPestanyaActual != null)
            {
				ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
				Dictionary<string, string> listaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
                Dictionary<string, KeyValuePair<bool, string>> listaEnlacesMultiIdioma = new Dictionary<string, KeyValuePair<bool, string>>();
                foreach (string idioma in listaIdiomas.Keys)
                {
                    Recursos.UtilIdiomas utilIdiomasActual = new Recursos.UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper);
                    string rutaComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(utilIdiomasActual, BaseURLIdioma, ProyectoVirtual.NombreCorto);
                    string rutaPestanya = "";
                    switch (ProyectoPestanyaActual.TipoPestanya)
                    {
                        case TipoPestanyaMenu.Indice:
                            rutaPestanya = utilIdiomasActual.GetText("URLSEM", "INDICE");
                            break;
                        case TipoPestanyaMenu.Recursos:
                            rutaPestanya = utilIdiomasActual.GetText("URLSEM", "RECURSOS");
                            break;
                        case TipoPestanyaMenu.Preguntas:
                            rutaPestanya = utilIdiomasActual.GetText("URLSEM", "PREGUNTAS");
                            break;
                        case TipoPestanyaMenu.Debates:
                            rutaPestanya = utilIdiomasActual.GetText("URLSEM", "DEBATES");
                            break;
                        case TipoPestanyaMenu.Encuestas:
                            rutaPestanya = utilIdiomasActual.GetText("URLSEM", "ENCUESTAS");
                            break;
                        case TipoPestanyaMenu.PersonasYOrganizaciones:
                            rutaPestanya = utilIdiomasActual.GetText("URLSEM", "PERSONASYORGANIZACIONES");
                            break;
                        case TipoPestanyaMenu.AcercaDe:
                            rutaPestanya = utilIdiomasActual.GetText("URLSEM", "ACERCADE");
                            break;
                        case TipoPestanyaMenu.BusquedaAvanzada:
                            rutaPestanya = utilIdiomasActual.GetText("URLSEM", "BUSQUEDAAVANZADA");
                            break;
                        case TipoPestanyaMenu.Comunidades:
                        case TipoPestanyaMenu.Borradores:
                        case TipoPestanyaMenu.Contribuciones:
                            base.CargarEnlacesMultiIdioma();
                            return;
                    }
                    if (!string.IsNullOrEmpty(ProyectoPestanyaActual.Ruta))
                    {
                        rutaPestanya = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Ruta, utilIdiomasActual.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    }
                    if (!string.IsNullOrEmpty(rutaPestanya))
                    {
                        rutaComunidad += "/" + rutaPestanya;
                    }
                    bool disponible = true;
                    if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma))
                    {
                        if (!ProyectoPestanyaActual.ListaIdiomasDisponibles(listaIdiomas).Contains(idioma))
                        {
                            disponible = false;
                        }
                    }
                    listaEnlacesMultiIdioma.Add(idioma, new KeyValuePair<bool, string>(disponible, rutaComunidad));
                }
                ListaEnlacesMultiIdioma = listaEnlacesMultiIdioma;
            }
            else
            {
                ListaEnlacesMultiIdioma = new Dictionary<string, KeyValuePair<bool, string>>();
                if (RequestParams("Comunidades") != null && RequestParams("Comunidades").Equals("true"))
                {
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
					List<string> listaIdiomas = paramCL.ObtenerListaIdiomas();
                    foreach (string idioma in listaIdiomas)
                    {
                        Recursos.UtilIdiomas utilIdiomasActual = new Recursos.UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper);
                        ListaEnlacesMultiIdioma.Add(idioma, new KeyValuePair<bool, string>(true, $"{BaseURL}/{utilIdiomasActual.GetText("URLSEM", "COMUNIDADES")}"));
                    }
                }

            }
        }

        protected override void CargarTituloPagina()
        {
            string tituloPagina = "";
            string metaDescripcion = "";
            ProyectoPestanyaMenu i;

            if (ProyectoPestanyaActual != null)
            {
                if (!string.IsNullOrEmpty(ProyectoPestanyaActual.MetaDescription))
                {
                    metaDescripcion = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.MetaDescription, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }

                if (!string.IsNullOrEmpty(ProyectoPestanyaActual.Titulo))
                {
                    tituloPagina = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }
                else if (!string.IsNullOrEmpty(ProyectoPestanyaActual.Nombre))
                {
                    tituloPagina = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                }
                else
                {
                    switch (ProyectoPestanyaActual.TipoPestanya)
                    {
                        case TipoPestanyaMenu.CMS:
                            if (ProyectoPestanyaActual.FilaProyectoPestanyaCMS.Ubicacion.Equals((short)TipoUbicacionCMS.HomeProyecto) || ProyectoPestanyaActual.FilaProyectoPestanyaCMS.Ubicacion.Equals((short)TipoUbicacionCMS.HomeProyectoMiembro) || ProyectoPestanyaActual.FilaProyectoPestanyaCMS.Ubicacion.Equals((short)TipoUbicacionCMS.HomeProyectoNoMiembro))
                            {
                                metaDescripcion = UtilCadenas.HtmlDecode(UtilCadenas.EliminarHtmlDeTexto(UtilCadenas.ObtenerTextoDeIdioma(this.ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto)));
                            }
                            break;
                        case TipoPestanyaMenu.Home:
                            metaDescripcion = UtilCadenas.HtmlDecode(UtilCadenas.EliminarHtmlDeTexto(UtilCadenas.ObtenerTextoDeIdioma(this.ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto)));
                            break;
                        case TipoPestanyaMenu.Indice:
                            tituloPagina = UtilIdiomas.GetText("COMMON", "INDICE");
                            metaDescripcion = UtilIdiomas.GetText("METADESCRIPTION", "INDICE", ProyectoSeleccionado.Nombre, ProyectoSeleccionado.NumRecusosTotales.ToString());
                            break;
                        case TipoPestanyaMenu.AcercaDe:
                            tituloPagina = UtilIdiomas.GetText("COMMON", "ACERCADE");
                            break;
                    }
                }

                this.TituloPagina = tituloPagina;

                base.CargarTituloPagina();

                if (!string.IsNullOrEmpty(metaDescripcion))
                {
                    if (metaDescripcion.Length > 160)
                    {
                        metaDescripcion = metaDescripcion.Substring(0, 160) + "...";
                    }

                    ViewBag.ListaMetas.Add(new KeyValuePair<string, string>("description", metaDescripcion));
                }
            }
        }

        /// <summary>
        /// Selecciona del combo de búsqueda la página de búsqueda actual
        /// </summary>
        protected void CargarComboBusqueda()
        {
            if (ProyectoPestanyaActual != null && ProyectoPestanyaActual.EsPestanyaBusqueda)
            {
                foreach (HeaderModel.SearchHeaderModel.SearchSelectComboModel combo in ((HeaderModel)ViewBag.Cabecera).Searcher.ListSelectCombo)
                {
                    if (combo.ID.ToLower() == ProyectoPestanyaActual.Clave.ToString().ToLower())
                    {
                        combo.Selected = true;
                    }
                    else
                    {
                        combo.Selected = false;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el proyecto seleccionado
        /// </summary>
        public ProyectoPestanyaMenu ProyectoPestanyaActual
        {
            get
            {
                return mControladorBase.ProyectoPestanyaActual;
            }
        }



        public GestionTesauro GestorTesauroProyectoActual
        {
            get
            {
                if (mGestorTesauroProyectoActual == null)
                {
                    mGestorTesauroProyectoActual = new GestionTesauro(new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                }
                return mGestorTesauroProyectoActual;
            }
        }

        /// <summary>
        /// Obtiene o establece si el usuario invitado puede ver esta página
        /// </summary>
        public override bool PaginaVisibleEnPrivada
        {
            get
            {
                return (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado) && !(ProyectoPestanyaActual == null || ProyectoPestanyaActual.Privacidad != TipoPrivacidadPagina.Especial);
            }
        }
    }
}
