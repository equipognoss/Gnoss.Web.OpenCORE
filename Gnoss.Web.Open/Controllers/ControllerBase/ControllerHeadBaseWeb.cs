using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using static Es.Riam.Gnoss.Web.MVC.Controllers.AdministrarUsuariosOrganizacionViewModel;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public partial class ControllerHeadBaseWeb : ControllerHeadBase
    {
        private ControllerBaseWeb ControllerBaseWeb
        {
            get
            {
                return (ControllerBaseWeb)base.ControllerBase;
            }
            set
            {
                base.ControllerBase = value;
            }
        }

        public ControllerHeadBaseWeb(ControllerBaseGnoss pControllerBase, LoggingService loggingService, ConfigService configService, EntityContext entityContext, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pControllerBase, loggingService, configService, entityContext, httpContextAccessor, redisCacheWrapper, gnossCache, virtuosoAD, entityContextBASE, servicesUtilVirtuosoAndReplication)
        {
        }

        protected override void ObtenerListaInputsHidden()
        {
            base.ObtenerListaInputsHidden();
            string inpt_FacetasProyAutoCompBuscadorCom = "";

            if (ControllerBaseWeb.FacetasProyAutoCompBuscadorCom != null)
            {
                inpt_FacetasProyAutoCompBuscadorCom = ControllerBaseWeb.FacetasProyAutoCompBuscadorCom;
            }
            else
            {
                inpt_FacetasProyAutoCompBuscadorCom = "sioc_t:Tag|gnoss:hasnombrecompleto|foaf:firstName";
            }

            if (ProyectoVirtual.TipoProyecto == TipoProyecto.Catalogo && !ControllerBaseWeb.ParametrosGeneralesVirtualRow.MostrarPersonasEnCatalogo)
            {
                inpt_FacetasProyAutoCompBuscadorCom = inpt_FacetasProyAutoCompBuscadorCom.Replace("gnoss:hasnombrecompleto", "").Replace("||", "|");
            }

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_FacetasProyAutoCompBuscadorCom", inpt_FacetasProyAutoCompBuscadorCom));

            //BuscadorPage
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_tipoBusquedaAutoCompl", ObtenerTipoBusquedaAutocompletar()));


            if (ControllerBaseWeb.EsPaginaEdicion)
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_refescarContadoresSinLimite", "1"));

                //HttpContext.Current.Response.Headers.Add("Cache-Control", "no-cache, must-revalidate");
                //HttpContext.Current.Response.Headers.Add("Pragma", "no-cache");
            }
        }

        private string ObtenerTipoBusquedaAutocompletar()
        {
            string tipo = "";
            //if (ControllerBaseWeb.TipoPagina == TiposPagina.Contribuciones)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS;
            //}
            //else if ((ControllerBaseWeb.TipoPagina == TipoPagina.BaseRecursos) && (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto)))
            //{
            //    tipo = FacetadoAD.BUSQUEDA_RECURSOS_PERFIL;
            //}
            if (ControllerBaseWeb.TipoPagina == TiposPagina.Comunidades)
            {
                tipo = FacetadoAD.BUSQUEDA_COMUNIDADES;
            }
            //else if (ControllerBaseWeb.TipoPagina == TipoPagina.Blogs)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_BLOGS;
            //}
            //else if (mControllerBase.TipoPagina == TiposPagina.BusquedaAvanzada)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_AVANZADA;
            //}
            //else if (mControllerBase.TipoPagina == TiposPagina.BaseRecursos)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_RECURSOS;
            //}
            //else if (mControllerBase.TipoPagina == TiposPagina.Debates)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_DEBATES;
            //}
            //else if (mControllerBase.TipoPagina == TiposPagina.Preguntas)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_PREGUNTAS;
            //}
            //else if (mControllerBase.TipoPagina == TiposPagina.Encuestas)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_ENCUESTAS;
            //}
            //else if (mControllerBase.TipoPagina == TiposPagina.Dafos)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_DAFOS;
            //}
            //else if (mControllerBase.TipoPagina == TiposPagina.PersonasYOrganizaciones)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_PERSONASYORG;
            //}
            //else if (mControllerBase.TipoPagina == TiposPagina.Contribuciones)
            //{
            //    tipo = FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS;
            //}

            if (ControllerBaseWeb is ControllerPestanyaBase && ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual != null && (
                ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.BusquedaSemantica
                || ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.Recursos
                || ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.Preguntas
                || ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.Debates
                || ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.Encuestas
                || ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.BusquedaAvanzada
                || ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual.TipoPestanya == TipoPestanyaMenu.PersonasYOrganizaciones))
            {
                tipo = ((ControllerPestanyaBase)ControllerBaseWeb).ProyectoPestanyaActual.FilaProyectoPestanyaMenu.PestanyaID.ToString();
            }

            return tipo;
        }
    }

}
