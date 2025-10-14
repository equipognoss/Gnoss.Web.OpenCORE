using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gnoss.Web.App_Start
{

    public class CachedRouter<TPrimaryKey> : IRouter
    {
        private readonly IRouter _innerRouter;
        public static bool RECALCULAR_RUTAS = false;
        private IApplicationBuilder _applicationBuilder;
        private ConfigService _configService;
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public CachedRouter(IRouter innerRouter, IApplicationBuilder applicationBuilder, ConfigService configService, ILogger logger, ILoggerFactory loggerFactory)
        {
            _applicationBuilder = applicationBuilder;
            if (innerRouter == null)
                throw new ArgumentNullException("innerRouter");
            _innerRouter = innerRouter;
            _configService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }


        public async Task RouteAsync(RouteContext context)
        {
            if (RECALCULAR_RUTAS)
            {
                var requestPath = context.HttpContext.Request.Path.Value;
                string[] argsUrlDestino = context.HttpContext.Request.Path.Value.Split('?');
                if (requestPath.Trim() == String.Empty)
                    return;

                string[] argsPagina = argsUrlDestino[0].Split('/', StringSplitOptions.RemoveEmptyEntries);
                string idioma = RouteConfig.IdiomaPrincipalDominio;
                string proyectoIDString = "";
                Guid proyectoID = Guid.Empty;
                
                string rutaPestanya = argsPagina[argsPagina.Length - 1];
                ProyectoPestanyaMenu filaPestanya = null;
                using (var serviceScope = _applicationBuilder.ApplicationServices.CreateScope())
                {
                    EntityContext entityContext = serviceScope.ServiceProvider.GetRequiredService<EntityContext>();
                    LoggingService loggingService = serviceScope.ServiceProvider.GetRequiredService<LoggingService>();
					RedisCacheWrapper redisCacheWrapper = serviceScope.ServiceProvider.GetRequiredService<RedisCacheWrapper>();
					UtilIdiomasFactory utilIdiomasFactory = new UtilIdiomasFactory(loggingService, entityContext, _configService, redisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomasFactory>(), mLoggerFactory);
                    UtilIdiomas utilIdiomas = utilIdiomasFactory.ObtenerUtilIdiomas(idioma);
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, null, _configService, null, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                    if (paramCL.ObtenerListaIdiomas().Contains(argsPagina[0]))
                    {
                        idioma = argsPagina[0];
                        rutaPestanya = string.Join("",argsPagina.Skip(1));

                        utilIdiomas = utilIdiomasFactory.ObtenerUtilIdiomas(idioma);
                        if(utilIdiomas.GetText("URLSEM", "COMUNIDAD").Equals(argsPagina[1]))
                        {
                            rutaPestanya = string.Join("", argsPagina.Skip(3));
                            proyectoID = entityContext.Proyecto.Where(item => item.NombreCorto.Equals(argsPagina[2])).Select(item => item.ProyectoID).FirstOrDefault();
                        }
                    }
                    else if (utilIdiomas.GetText("URLSEM", "COMUNIDAD").Equals(argsPagina[0]))
                    {
                        rutaPestanya = string.Join("", argsPagina.Skip(2));
                        proyectoID = entityContext.Proyecto.Where(item => item.NombreCorto.Equals(argsPagina[1])).Select(item => item.ProyectoID).FirstOrDefault();
                    }
                    else
                    {
                        rutaPestanya = string.Join("", argsPagina);
                        if (!string.IsNullOrEmpty(_configService.ObtenerProyectoID()))
                        {
                            proyectoID = new Guid(_configService.ObtenerProyectoID());
                        }
                    }
                    if (!proyectoID.Equals(Guid.Empty))
                    {
                        proyectoIDString = proyectoID.ToString();
                    }
                    filaPestanya = entityContext.ProyectoPestanyaMenu.Where(item => item.Ruta.ToUpper().Equals(rutaPestanya.ToUpper()) && item.ProyectoID.Equals(proyectoID)).FirstOrDefault();
                }

                if (filaPestanya != null)
                {
                    TipoPestanyaMenu tipoPestanya = (TipoPestanyaMenu)filaPestanya.TipoPestanya;
                    string argControler = "";
                    string argAction = "";

                    //Invoke MVC controller/action
                    var oldRouteData = context.RouteData;
                    var newRouteData = new RouteData(oldRouteData);
                    bool esPaginaBusqueda = false;
                    if (filaPestanya.Activa)
                    {
                        switch (tipoPestanya)
                        {
                            case TipoPestanyaMenu.Home:
                                newRouteData.Values["controller"] = "HomeComunidad";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                
                                
                                break;
                            case TipoPestanyaMenu.Indice:
                                newRouteData.Values["controller"] = "Indice";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                break;
                            case TipoPestanyaMenu.Recursos:
                                newRouteData.Values["controller"] = "Busqueda";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                newRouteData.Values["Recursos"] = "true";
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.Preguntas:
                                newRouteData.Values["controller"] = "Busqueda";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                newRouteData.Values["preguntas"] = "true";
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.Debates:
                                newRouteData.Values["controller"] = "Busqueda";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                newRouteData.Values["debates"] = "true";
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.Encuestas:
                                newRouteData.Values["controller"] = "Busqueda";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                newRouteData.Values["encuestas"] = "true";
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.PersonasYOrganizaciones:
                                newRouteData.Values["controller"] = "Busqueda";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                newRouteData.Values["perorg"] = "true";
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.AcercaDe:
                                newRouteData.Values["controller"] = "AcercaDeComunidad";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                break;
                            case TipoPestanyaMenu.CMS:
                                newRouteData.Values["controller"] = "CMSPagina";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                break;
                            case TipoPestanyaMenu.BusquedaSemantica:
                                newRouteData.Values["controller"] = "Busqueda";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                newRouteData.Values["semanticos"] = "true";
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.BusquedaAvanzada:
                                newRouteData.Values["controller"] = "Busqueda";
                                newRouteData.Values["PestanyaID"] = filaPestanya.PestanyaID.ToString();
                                newRouteData.Values["Meta"] = "true";
                                esPaginaBusqueda = true;
                                break;
                            case TipoPestanyaMenu.EnlaceInterno:
                                break;
                            case TipoPestanyaMenu.EnlaceExterno:
                                break;
                        }
                        if (!string.IsNullOrEmpty(proyectoIDString))
                        {
                            newRouteData.Values["proyectoID"] = proyectoIDString;
                        }
                        newRouteData.Values["lang"] = idioma;
                        newRouteData.Values["action"] = "Index";
                        newRouteData.Routers.Add(_innerRouter);
                        context.RouteData = newRouteData;
                    }
                }
            }
            try
            {
                await this._innerRouter.RouteAsync(context);
            }
            finally
            {
                // Restore the original values to prevent polluting the route data.
                //if (!context.IsHandled)
                //{
                //    context.RouteData = oldRouteData;
                //}
            }


            
        }
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            VirtualPathData result = null;

            var values = context.Values;
            var controller = Convert.ToString(values["controller"]);
            var action = Convert.ToString(values["action"]);
            var id = Convert.ToString(values["id"]);

            if ("Item".Equals(controller) && "View".Equals(action))
            {
                result = new VirtualPathData(this, "abcd?id=" + id);
                //context.IsBound = true;
            }

            // IMPORTANT: Always return null if there is no match.
            // This tells .NET routing to check the next route that is registered.
            return result;
        }

    }
}
