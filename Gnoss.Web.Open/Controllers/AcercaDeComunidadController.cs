using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
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
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class AcercaDeComunidadController : ControllerPestanyaBase
    {

        public AcercaDeComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        [TypeFilter(typeof(AccesoPestanyaAttribute))]
        public ActionResult Index()
        {
            AboutTheCommunityViewModel paginaModel = new AboutTheCommunityViewModel();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            int? numRecursos = proyCL.ObtenerContadorComunidad(ProyectoSeleccionado.Clave, TipoBusqueda.Recursos);
            if (!numRecursos.HasValue) { numRecursos = ContadoresProyecto.NumeroRecursos; }

            int? numPerYOrg = proyCL.ObtenerContadorComunidad(ProyectoSeleccionado.Clave, TipoBusqueda.PersonasYOrganizaciones);
            if (!numPerYOrg.HasValue) { numPerYOrg = ContadoresProyecto.NumeroMiembros + ContadoresProyecto.NumeroOrgRegistradas; }

            paginaModel.NumberOfResources = numRecursos.Value;
            paginaModel.NumberOfPersonOrganizations = numPerYOrg.Value;

            paginaModel.Description = UtilCadenas.ObtenerTextoDeIdioma(ProyectoSeleccionado.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            if (ProyectoSeleccionado.FilaProyecto.FechaInicio.HasValue)
            {
                paginaModel.FoundationDate = ProyectoSeleccionado.FilaProyecto.FechaInicio.Value;
            }

            if (ProyectoSeleccionado.GestorProyectos.GestionTesauro == null)
            {
                TesauroCL tesCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                ProyectoSeleccionado.GestorProyectos.GestionTesauro = new GestionTesauro(tesCL.ObtenerTesauroDeProyectoMyGnoss(), mLoggingService, mEntityContext);
                tesCL.Dispose();
            }

            paginaModel.Categories = new List<CategoryModel>();

            foreach (CategoriaTesauro catTes in ProyectoSeleccionado.CategoriasTesauro.Values)
            {
                CategoryModel categoriaTesauro = CargarCategoria(catTes);
                paginaModel.Categories.Add(categoriaTesauro);
            }

            paginaModel.Tags = ProyectoSeleccionado.ListaTagsSoloLectura;

            if (ParametrosGeneralesRow.PermitirCertificacionRec)
            {
                ParametroGeneralCN paramGCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paginaModel.CertificationPolicy = paramGCN.ObtenerPoliticaCertificacionDeProyecto(ProyectoSeleccionado.Clave);
                paramGCN.Dispose();

                paginaModel.CertificationLevels = Comunidad.CertificationLevels;
            }

            paginaModel.InvitationsAvailable = ParametrosGeneralesRow.InvitacionesDisponibles;
            paginaModel.RatingsAvailable = ParametrosGeneralesRow.VotacionesDisponibles;
            paginaModel.CommentsAvailable = ParametrosGeneralesRow.ComentariosDisponibles;

            ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

            List<Identidad> ListaAdministradores = controladorProyecto.CargarAdministradoresProyecto(ProyectoSeleccionado);
            List<Guid> listaIds = new List<Guid>();
            foreach (Identidad identidad in ListaAdministradores)
            {
                if (!listaIds.Contains(identidad.Clave))
                {
                    listaIds.Add(identidad.Clave);
                }
            }

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> listaFotosIds = identCN.ObtenerSiListaIdentidadesTienenFoto(listaIds);
            identCN.Dispose();

            paginaModel.Administrators = new List<ProfileModel>();
            foreach (Identidad identidad in ListaAdministradores)
            {
                ProfileModel usuario = new ProfileModel();
                usuario.NamePerson = identidad.Nombre();
                usuario.UrlPerson = mControladorBase.UrlsSemanticas.GetURLPerfilPersonaOOrgEnProyecto(BaseURLIdioma, UtilIdiomas, UrlPerfil, identidad, ProyectoSeleccionado.NombreCorto);
                usuario.UrlFoto = obtenerImagen(listaFotosIds[identidad.Clave]);
                usuario.TypeProfile = (ProfileType)identidad.FilaIdentidad.Tipo;

                paginaModel.Administrators.Add(usuario);
            }

            return View(paginaModel);
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

    }
}
