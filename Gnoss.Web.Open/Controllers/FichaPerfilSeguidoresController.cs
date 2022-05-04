using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles;
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
using System;
using System.Collections.Generic;
using System.Linq;


namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class FichaPerfilSeguidoresController : ControllerBaseWeb
    {
        public FichaPerfilSeguidoresController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        ProfilePageFollowersViewModel paginaModel = new ProfilePageFollowersViewModel();

        #endregion

        #region Métodos


        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index()
        {

            if (!string.IsNullOrEmpty(RequestParams("MeSiguen")) && RequestParams("MeSiguen").Equals("true"))
            {
                paginaModel.PageType = ProfilePageFollowersViewModel.ProfilePageType.Followers;
            }
            else
            {
                paginaModel.PageType = ProfilePageFollowersViewModel.ProfilePageType.Followed;

            }
            CargarSeguidoresPerfil(1);
            CargarSeguidosPerfil(1);

            return View(paginaModel);
        }

        private void CargarSeguidoresPerfil(int numPeticion)
        {
            int numElementosMostrar = 10000;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            //Sigue a...
            List<Guid> listaIdentidades = new List<Guid>();

            if (ProyectoSeleccionado.Clave == ProyectoAD.MyGnoss)
            {
                listaIdentidades = identidadCN.ObtenerListaIdentidadesSusucritasAPerfil(IdentidadActual.PerfilID);
            }
            else
            {
                listaIdentidades = identidadCN.ObtenerListaIdentidadesIDSusucritasAPerfilEnProyecto(IdentidadActual.PerfilID, ProyectoSeleccionado.Clave);
            }

            int numItem = 1;
            List<Guid> listaIdentidadesCargar = new List<Guid>();
            foreach (Guid ident in listaIdentidades)
            {
                //if (numItem > ((numPeticion - 1) * numElementosMostrar))
                //{
                //    if (listaIdentidadesCargar.Count == numElementosMostrar) { break; }
                    listaIdentidadesCargar.Add(ident);
                //}
                //numItem++;
            }

            Dictionary<Guid, ProfileModel> listaSeguidoresPerfil = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdentidadesCargar);

            paginaModel.ProfileFollowers = listaSeguidoresPerfil.Values.ToList();
            paginaModel.NumProfileFollowers = listaIdentidades.Count;

            //if (pCargarInfoExtra)
            {
                ControladorProyectoMVC.ObtenerInfoExtraIdentidadesPorID(listaSeguidoresPerfil);
            }

        }

        private void CargarSeguidosPerfil(int numPeticion)
        {
            int numElementosMostrar = 10000;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            //Le siguen...
            List<Guid> listaIdentidades = new List<Guid>();

            if (ProyectoSeleccionado.Clave == ProyectoAD.MyGnoss)
            {
                listaIdentidades = identidadCN.ObtenerListaIdentidadesSusucritasPorPerfil(IdentidadActual.PerfilID);
            }
            else
            {
                listaIdentidades = identidadCN.ObtenerListaIdentidadesSusucritasPorPerfilEnProyecto(IdentidadActual.PerfilID, ProyectoSeleccionado.Clave);
            }

            int numItem = 1;
            List<Guid> listaIdentidadesCargar = new List<Guid>();
            foreach (Guid ident in listaIdentidades)
            {
                //if (numItem > ((numPeticion - 1) * numElementosMostrar))
                //{
                //    if (listaIdentidadesCargar.Count == numElementosMostrar) { break; }
                    listaIdentidadesCargar.Add(ident);
                //}
                //numItem++;
            }

            Dictionary<Guid, ProfileModel> listaSeguidosPerfil = ControladorProyectoMVC.ObtenerIdentidadesPorID(listaIdentidadesCargar);

            paginaModel.ProfileFollowed = listaSeguidosPerfil.Values.ToList();
            paginaModel.NumProfileFollowed = listaIdentidades.Count;

            //if (pCargarInfoExtra)
            {
                ControladorProyectoMVC.ObtenerInfoExtraIdentidadesPorID(listaSeguidosPerfil);
            }
        }

        #endregion

    }
}
