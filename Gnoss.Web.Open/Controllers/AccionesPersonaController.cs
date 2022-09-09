using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Models;
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
    public class AccionesPersonaController : ControllerBaseWeb
    {

        public AccionesPersonaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {

        }

        // GET: AccionesPersona
        public ActionResult Index()
        {
            if (mControladorBase.ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
            {
                // Es administrador del proyecto, le mostramos las acciones de las personas
                List<Guid> listaIdentidades = ObtenerPersonasDeRequest();

                if (listaIdentidades.Count > 0)
                {
                    MultiViewResult result = new MultiViewResult(this, mViewEngine);

                    //Cargamos el modelo
                    ObtenerModelo(result, listaIdentidades);

                    return result;
                }
            }

            return new EmptyResult();
        }

        private List<Guid> ObtenerPersonasDeRequest()
        {
            string personas = RequestParams("listaRecursos");
            List<Guid> listaIdentidades = new List<Guid>();

            if (!string.IsNullOrEmpty(personas))
            {
                string[] listaPersonas = personas.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (listaPersonas.Length > 0)
                {
                    foreach (string control in listaPersonas)
                    {
                        string[] args = control.Split('_');

                        Guid identidadId = new Guid(args[1]);

                        if (!listaIdentidades.Contains(identidadId))
                        {
                            listaIdentidades.Add(identidadId);
                        }
                    }
                }
            }

            return listaIdentidades;
        }

        private void ObtenerModelo(MultiViewResult pResult, List<Guid> pListaIdentidades)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            GestionPersonas gestorPersonas = new GestionPersonas(personaCN.ObtenerPersonasPorIdentidad(pListaIdentidades), mLoggingService, mEntityContext);

            List<Guid> listaUsuariosID = gestorPersonas.ListaPersonas.Values.Select(persona => persona.UsuarioID).ToList();

            gestorPersonas.GestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerUsuariosCompletosPorID(listaUsuariosID), mLoggingService, mEntityContext, mConfigService);

            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesPorID(pListaIdentidades, false), gestorPersonas, gestorPersonas.GestorUsuarios, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<Guid> listaOrganizaciones = gestorIdentidades.ListaIdentidades.Values.Where(identidad => identidad.EsOrganizacion).Select(identidad => identidad.OrganizacionID.Value).ToList();

            if (listaOrganizaciones.Count > 0)
            {
                OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorIdentidades.GestorOrganizaciones = new GestionOrganizaciones(orgCN.ObtenerOrganizacionesDeIdentidadesCargadas(gestorIdentidades.DataWrapperIdentidad), mLoggingService, mEntityContext);
            }

            FichaPerfilController fichaPerfilController = new FichaPerfilController(mLoggingService, mConfigService, mEntityContext, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mViewEngine, mEntityContextBASE, mEnv, mActionContextAccessor, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mOAuth);
            foreach (Guid identidadID in gestorIdentidades.ListaIdentidades.Keys)
            {
                ProfileModel perfil = fichaPerfilController.ObtenerFichaIdentidadModelDeIdentidad(gestorIdentidades.ListaIdentidades[identidadID]);

                pResult.AddView("ControlesMVC/_AccionesPerfil", "AccionesRecursoListado_" + identidadID + "_" + ProyectoSeleccionado.Clave, perfil);
            }
        }
    }
}