using System;
using System.Text;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Newtonsoft.Json;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.AD.Virtuoso;
using Microsoft.AspNetCore.Http;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Interfaces.InterfacesOpen;

namespace Es.Riam.Gnoss.Web.MVC.Filters
{
    public enum FiltroAcciones
    {
        Todo,
        SoloIntegracion,
        SoloDespliegues
    }

    public class AccesoIntegracionAttribute : BaseActionFilterAttribute
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private VirtuosoAD mVirtuosoAD;
        private IHttpContextAccessor mHttpContextAccessor;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;
        private ControladorBase mControladorBase;
        private IUtilServicioIntegracionContinua mUtilIntegracionContinua;


        public AccesoIntegracionAttribute(EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, GnossCache gnossCache, EntityContextBASE entityContextBASE, IUtilServicioIntegracionContinua utilIntegracionContinua)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mVirtuosoAD = virtuosoAD;
            mHttpContextAccessor = httpContextAccessor;
            mGnossCache = gnossCache;
            mEntityContextBASE = entityContextBASE;
            mRedisCacheWrapper = redisCacheWrapper;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null);

            Filtro = FiltroAcciones.Todo;

            mUtilIntegracionContinua = utilIntegracionContinua;
        }

        public FiltroAcciones Filtro
        {
            get;
            set;
        }

        protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
        {
            try
            {
                string EntornoIntegracionContinua, UrlApiIntegracionContinua;
                Guid proyectoSeleccionado = mControladorBase.ProyectoSeleccionado.Clave;
                ObtenerParametrosControllerBaseGnoss(out UrlApiIntegracionContinua, out EntornoIntegracionContinua);
                //Si existe integracion continua y es administrador
                if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && mControladorBase.ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                {
                    string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto);
                    if (Filtro != FiltroAcciones.SoloIntegracion)
                    {
                        ConfigurarDespliegues(pFilterContext, EntornoIntegracionContinua, UrlApiIntegracionContinua, proyectoSeleccionado, urlComunidad);
                    }
                    bool esPreproduccion = mUtilIntegracionContinua.EsPreproduccion(EntornoIntegracionContinua, UrlApiIntegracionContinua);
                    if (esPreproduccion)
                    {
                        pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRARCOMUNIDADGENERAL"));
                    }

                    bool entornoBloqueado = mUtilIntegracionContinua.EstaEntornoBloqueado(proyectoSeleccionado, EntornoIntegracionContinua, UrlApiIntegracionContinua);
                    if (pFilterContext.Result == null && entornoBloqueado)
                    {
                        pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRARCOMUNIDADGENERAL"));
                    }

                    bool tieneHotfix = mUtilIntegracionContinua.EsHotfix(proyectoSeleccionado, EntornoIntegracionContinua, UrlApiIntegracionContinua);
                    if (pFilterContext.Result == null && tieneHotfix)
                    {
                        pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRARCOMUNIDADGENERAL"));
                    }

                    if (pFilterContext.Result == null)
                    {
                        string urlRedirectLogin = urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRACREDENCIALES");
                        string urlRedirectramas = urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRAINTEGRACIONCONTINUA");
                        string urlConflictos = urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRARLISTACONFLICTOS");

                        bool estaProcesando = mUtilIntegracionContinua.EstaProcesando(mControladorBase.UsuarioActual.UsuarioID, proyectoSeleccionado, EntornoIntegracionContinua, UrlApiIntegracionContinua);
                        if (estaProcesando)
                        {
                            pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "PROCESANDOTAREAS")+ "?redirect=ADMINISTRARCOMUNIDADGENERAL");
                        }
                        else
                        {
                            PeticionnesIntegracionContinua(pFilterContext, EntornoIntegracionContinua, UrlApiIntegracionContinua, proyectoSeleccionado, urlRedirectLogin, urlRedirectramas, urlConflictos);
                        }
                        
                    }

                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }
        }

        private void ConfigurarDespliegues(ActionExecutingContext pFilterContext, string EntornoIntegracionContinua, string UrlApiIntegracionContinua, Guid proyectoSeleccionado, string urlComunidad)
        {
            //Si existe una version actual en el repositorio
            if (mUtilIntegracionContinua.ExistCurrentVersion(proyectoSeleccionado, mControladorBase.UsuarioActual.UsuarioID, EntornoIntegracionContinua, UrlApiIntegracionContinua))
            {
                //Si la version del repositorio y de la web no coinciden
                if (!mUtilIntegracionContinua.ComprobarVersionRepositorioWebCoinciden(proyectoSeleccionado, mControladorBase.UsuarioActual.UsuarioID, EntornoIntegracionContinua, UrlApiIntegracionContinua))
                {
                    //Obligamos a actualizar la web a la version del repositorio
                    pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRARVERSIONANTIGUA"));
                }
            }
            /*else
            {
                //Si no existe la crea
                pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRARDESPLIEGUES"));
            }*/

        }

        private void PeticionnesIntegracionContinua(ActionExecutingContext pFilterContext, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pProyectoSeleccionado, string pUrlRedirectLogin, string pUrlRedirectramas, string pUrlRedirectConflictos)
        {
            TipoRepositorio resultado;
            
            bool iniciado = false;
            iniciado = mUtilIntegracionContinua.EstaEnBD(mControladorBase.ProyectoSeleccionado.FilaProyecto.NombreCorto, pUrlApiIntegracionContinua);
            string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.FilaProyecto.NombreCorto);
            //Comprobar si esta almacenado el proyecto de I.C en la BBDD
            if (iniciado)
            {        
                //Comprobar si se ha hecho login con el admin del repositorio.
                bool usuarioRepositorio = false;
                usuarioRepositorio = mUtilIntegracionContinua.EstaLogeado(mControladorBase.ProyectoSeleccionado.FilaProyecto.NombreCorto, mControladorBase.UsuarioActual.UsuarioID, pUrlApiIntegracionContinua);
                //Esta logeado
                if (usuarioRepositorio)
                {
                    //Comprobar si esta iniciado el repositorio.

                    short estainiciado = mUtilIntegracionContinua.EstaIniciadoRepositorio(mControladorBase.ProyectoSeleccionado.Clave, pEntornoIntegracionContinua, pUrlApiIntegracionContinua);

                    if (estainiciado == 0)
                    {
                        pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "CONFIGURARINTEGRACIONCONTINUAREPOSITORIO"));
                    }
                    else
                    {
                        //Comprobamos el tipo de repositorio que tiene la integracion continua
                        string repositorio = mUtilIntegracionContinua.CheckearRepositorio(mControladorBase.UsuarioActual.UsuarioID, pProyectoSeleccionado, pEntornoIntegracionContinua, pUrlApiIntegracionContinua);

                        if (Enum.TryParse(repositorio, out resultado))
                        {
                            //Si el repositorio del ecosistema al que pertecene la comunidad es Bitbucket Server
                            if (resultado == TipoRepositorio.BitbucketServer)
                            {
                                AdministrarCredencialesViewModel modeloCredenciales = new AdministrarCredencialesViewModel();
                                modeloCredenciales = mUtilIntegracionContinua.PeticionApiUsuarioExiste(modeloCredenciales, pProyectoSeleccionado, mControladorBase.UsuarioActual.UsuarioID, pEntornoIntegracionContinua, pUrlApiIntegracionContinua);
                                if (string.IsNullOrEmpty(modeloCredenciales.Error))
                                {
                                    //Si el usuario no esta registrado
                                    if (!modeloCredenciales.EstaRegistrado)
                                    {
                                        //Le redirigimos al Login
                                        pFilterContext.Result = new RedirectResult(pUrlRedirectLogin);
                                        return;
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException($"Error en la peticion User={mControladorBase.UsuarioActual.UsuarioID}&Project={pProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}, el error es {modeloCredenciales.Error}");
                                }
                            }
                            //Si el repositorio del ecosistema al que pertecene la comunidad es BitbucketCloud o GitHub
                            else if (resultado == TipoRepositorio.BitbucketCloud || resultado == TipoRepositorio.GitHub)
                            {
                                PeticionLoginGithubBitbucketCloud(pFilterContext, pEntornoIntegracionContinua, pUrlApiIntegracionContinua, pProyectoSeleccionado);
                            }
                            //obtenemos la informacion sobre las ramas activa para este entorno
                            AdministrarRamasViewModel modelRamas = mUtilIntegracionContinua.ObtenerEstadoIntegracion(pProyectoSeleccionado, mControladorBase.UsuarioActual.UsuarioID, pEntornoIntegracionContinua, pUrlApiIntegracionContinua);
                            if (string.IsNullOrEmpty(modelRamas.Error))
                            {
                                //Comprobamos si nos viene de la parte de fusion
                                string request = pFilterContext.HttpContext.Request.Path;
                                if (!request.Contains("configurar-propiedades-fusion"))
                                {
                                    //si no existe ninguna rama activa y ademas es fusionable(el entorno de develop por ahora)
                                    //MODIFICACIONIC: Modificación para cuando vaya a despliegues y no este la rama creada que no le redirija a crearla.
                                    if (Filtro != FiltroAcciones.SoloDespliegues && modelRamas.EsFusionable)
                                    {
                                        if (string.IsNullOrEmpty(modelRamas.Nombre)) { 
                                            //Redigirimos a la pagina de creacion de ramas
                                            pFilterContext.Result = new RedirectResult(pUrlRedirectramas);
                                            return;
                                        }
                                    }
                                }
                                bool conflictos = mUtilIntegracionContinua.HayConlictos(pProyectoSeleccionado, mControladorBase.UsuarioActual.UsuarioID, pEntornoIntegracionContinua, pUrlApiIntegracionContinua);
                                if (conflictos)
                                {
                                    pFilterContext.Result = new RedirectResult(pUrlRedirectConflictos);
                                    return;
                                }

                                //Si un entorno no es fusionable --> Que tiene que tener creadas por defecto dos ramas hijas activas (tendrán el mismo nombre que el entorno al que pertenezca)
                            }
                            else
                            {
                                throw new ArgumentException($"Error en la peticion User={mControladorBase.UsuarioActual.UsuarioID}&Project={pProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}, el error es {modelRamas.Error}");
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"Error al castear el TipoRepositorio en la peticion al API User={mControladorBase.UsuarioActual.UsuarioID}&Project={pProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}");
                        }
                    }
                   
                }
                else
                {
                    pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "CONFIGURARINTEGRACIONCONTINUAINCIAL"));
                }             
            }
            else
            {
                pFilterContext.Result = new RedirectResult(urlComunidad + "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "ADMINISTRARINTEGRACIONCONTINUA")); 
            }
        }

        private void PeticionLoginGithubBitbucketCloud(ActionExecutingContext pFilterContext, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, Guid pProyectoSeleccionado)
        {
            //Comprobamos si el usuario esta logeado o si el token necesita refresco
            string respuesta = mUtilIntegracionContinua.ComprobarAutorizacionGitBit(mControladorBase.UsuarioActual.UsuarioID, pProyectoSeleccionado, pEntornoIntegracionContinua, pFilterContext.HttpContext.Request.Path.ToString(), UrlApiIntegracionContinua);
            if (respuesta.StartsWith("http://") || respuesta.StartsWith("https://"))
            {
                //En este caso el usuario necesita hacer login--> este redirect va a GitHub o a Bitbucket
                pFilterContext.HttpContext.Response.Redirect(respuesta);
            }
        }

        private void ObtenerParametrosControllerBaseGnoss(out string pUrlApiIntegracionContinua, out string pEntornoIntegracionContinua)
        {
            pUrlApiIntegracionContinua = mConfigService.ObtenerUrlApiIntegracionContinua();
            if (!string.IsNullOrEmpty(pUrlApiIntegracionContinua))
            {
                // pEntornoIntegracionContinua = pmControladorBase.ParametrosAplicacionDS.ParametroAplicacion.FindByParametro("EntornoIntegracionContinua").Valor;
                pEntornoIntegracionContinua = mControladorBase.ListaParametrosAplicacion.Find(parametro => parametro.Parametro.Equals("EntornoIntegracionContinua")).Valor;
            }
            else
            {
                pEntornoIntegracionContinua = null;
            }
        }
    }
}
