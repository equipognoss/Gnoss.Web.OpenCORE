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
using Microsoft.AspNetCore.Hosting;
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

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class MisComunidadesController : ControllerBaseWeb
    {
        public MisComunidadesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            MyCommunitiesViewModel paginaModel = new MyCommunitiesViewModel();

            //Obtenemos los proyectos de los que es miembro el usuario.
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerProyectosParticipaPerfilUsuario(IdentidadActual.PerfilUsuario.Clave, true, UsuarioActual.UsuarioID);
            proyCN.Dispose();

            //Obtenemos todos los proyectos hijos a los que pertenece el usuario.
            List<Guid> hijos = new List<Guid>();
            foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in dataWrapperProyecto.ListaProyecto.Where(proyecto=>proyecto.ProyectoSuperiorID.HasValue).OrderBy(proy=>proy.Nombre))
            {
                hijos.Add(filaProy.ProyectoID);
            }

            //Obtenemos todos los proyectos padre de los proyectos hijos
            ProyectoCN proyCN2 = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto padresNoHabilitados = new DataWrapperProyecto();
            if (hijos.Count != 0)
            {
                padresNoHabilitados = proyCN2.ObtenerProyectosPadresDeProyectos(hijos);
            }
            proyCN2.Dispose();

            //Mezclamos las listas, obteniendo todos los proyectos afectados (tanto a los que pertenece como a los que no)
            DataWrapperProyecto todos = new DataWrapperProyecto();
            todos.Merge(dataWrapperProyecto);
            todos.Merge(padresNoHabilitados);

            //Creamos un gestor de proyectos con todos los proyectos
            GestionProyecto gestProy = new GestionProyecto(todos, mLoggingService, mEntityContext);

            if (gestProy.ListaProyectos.Count > 0)
            {
                paginaModel.Communities = new List<MyCommunitiesViewModel.MyCommunityModel>();

                foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in gestProy.DataWrapperProyectos.ListaProyecto.Where(item => !item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).OrderBy(proy => proy.Nombre))
                {
                    MyCommunitiesViewModel.MyCommunityModel comunidad = new MyCommunitiesViewModel.MyCommunityModel();
                    comunidad.Key = filaProy.ProyectoID;

                    comunidad.ParentKey = Guid.Empty;
                    if (filaProy.ProyectoSuperiorID.HasValue)
                    {
                        comunidad.ParentKey = filaProy.ProyectoSuperiorID.Value;
                    }

                    string nombreProyecto = filaProy.Nombre;

                    if (filaProy.Nombre.Contains("|||"))
                    {
                        List<string> nombresIdioma = nombreProyecto.Split(new string[] { "|||" }, StringSplitOptions.None).ToList();
                        Dictionary<string, string> nombreIdiomaDictionary = new Dictionary<string, string>();
                        foreach (string nombreCode in nombresIdioma)
                        {
                            if (nombreCode.Contains("@"))
                            {
                                string[] idiomas = nombreCode.Split('@');
                                nombreIdiomaDictionary.Add(idiomas[1], idiomas[0]);
                            }

                        }

                        if (nombreIdiomaDictionary.ContainsKey(UtilIdiomas.LanguageCode))
                        {
                            nombreProyecto = nombreIdiomaDictionary[UtilIdiomas.LanguageCode];
                        }
                        else
                        {
                            nombreProyecto = nombreIdiomaDictionary.Values.First();
                        }
                    }
                    comunidad.Name = nombreProyecto;


                    string nombreImagenePeque = ControladorProyecto.ObtenerFilaParametrosGeneralesDeProyecto(filaProy.ProyectoID).NombreImagenPeque;
                    string urlLogo = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesProyectos + "/" + nombreImagenePeque;

                    if (nombreImagenePeque == "peque")
                    {
                        urlLogo = BaseURLStatic + "/img" + "/" + UtilArchivos.ContentImgIconos + "/" + UtilArchivos.ContentImagenesProyectos + "/" + "anonimo_peque.png";
                    }
                    comunidad.Logo = urlLogo;
                    
                    comunidad.Url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, filaProy.NombreCorto);

                    comunidad.AccessType = (CommunityModel.TypeAccessProject)filaProy.TipoAcceso;

                    paginaModel.Communities.Add(comunidad);
                }
            }
            return View(paginaModel);
        }
    }
}
