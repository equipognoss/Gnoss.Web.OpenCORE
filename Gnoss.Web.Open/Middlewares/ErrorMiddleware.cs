using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Routes;
using Gnoss.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Newtonsoft.Json;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Util;

namespace Gnoss.Web.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _env;
        private ConfigService _configService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ErrorMiddleware(RequestDelegate next, IHostingEnvironment env, ConfigService configService, ILogger<ErrorMiddleware> logger, ILoggerFactory loggerFactory)
        { 
            _next = next;
            _env = env;
            _configService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context, LoggingService loggingService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper)
        {
            try
            {
                await _next(context);
                if(context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    string originalPath = context.Request.Path.Value;
                    context.Items["originalPath"] = originalPath;
                    context.Request.Path = ComprobarRutaPestanya(context, entityContext, loggingService, redisCacheWrapper);
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(context, ex, loggingService, entityContext);
                await _next(context);
            }
        }

        private void HandleExceptionAsync(HttpContext context, Exception ex, LoggingService loggingService, EntityContext entityContext)
        {
            var code = HttpStatusCode.InternalServerError;
            context.Response.StatusCode = (int)code;                        
            string mensajeError = $" ERROR:  {ex.Message}\r\nStackTrace: {ex.StackTrace}";

            loggingService.GuardarLogError(ex, mlogger); //Enviar Excepción

            if (ex.StackTrace.Contains(" _Page_Views") && !ex.StackTrace.Contains(" _Page_Views_Shared_Layout") && !ex.StackTrace.Contains(" ASP._Page_Views_Error") && !ex.StackTrace.Contains(" _Page_Views_Shared_Head__HojasDeEstilo"))
            {
                string directorioLogErroresVistas = Path.Combine(_env.ContentRootPath, "ErroresVistas");
                // Es posible que haya un error en las vistas, guardo el log e intento invalidarlas
                if (Directory.Exists(directorioLogErroresVistas))
                {
                    loggingService.GuardarLogError(loggingService.DevolverCadenaError(ex, "1.0.0.0"), mlogger, $"{directorioLogErroresVistas}\\errorvista_{DateTime.Now.ToString("yyyyMMdd")}.log");
                }

                // Si la última vez que se invalidaron vistas por error fué hace menos de 30 minutos, no vuelvo a invalidar las vistas. El error probablemente no estará en la invalidación, hay otro tipo de error. 
                //if (!UltimaInvalidacionVistasPorError.HasValue || DateTime.Now.Subtract(UltimaInvalidacionVistasPorError.Value).TotalMinutes > 30)
                //{
                //    UltimaInvalidacionVistasPorError = DateTime.Now;

                    //InvalidarVistasLocales();
                    List<ParametroAplicacion> listaParametroAplicacion = entityContext.ParametroAplicacion.Where(parametro => parametro.Parametro == "CorreoErroresVistas").ToList();
                    if (listaParametroAplicacion.Count > 0)
                    {
                        GestionNotificaciones.EnviarCorreoError(mensajeError, $"en vista", listaParametroAplicacion);
                    }
                //}
            }
            context.Response.StatusCode = (int)code;
            
            context.Request.Path = "/error";
            context.Request.QueryString = context.Request.QueryString.Add("errorcode", "500");
            Dictionary<string, string> excep = new Dictionary<string, string>();
            excep.Add("Error", ex.Message);
            excep.Add("ErrorTrace", ex.StackTrace);
            var stream = new MemoryStream();
            string json = JsonConvert.SerializeObject(excep);
            var writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
            writer.Write(json);
            writer.Flush();
            stream.Position = 0;
            context.Request.Body = stream;
        }

        public string ComprobarRutaPestanya(HttpContext context, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper)
        {
            string[] segmentos = context.Items["originalPath"].ToString().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segmentos.Length > 0)
            {
                string rutaPedida = "";
                string nombreCortoComunidad = "";
                ProyectoCN proyectoCN = new ProyectoCN(entityContext, loggingService, _configService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                string dominio = _configService.ObtenerDominio();
                string idioma = proyectoCN.ObtenerIdiomaPrincipalDominio(dominio);
                UtilIdiomas utilIdiomas = new UtilIdiomas(idioma, loggingService, entityContext, _configService, redisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);
                string comunidadTxt = utilIdiomas.GetText("COMMON", "COMUNIDAD").ToLower();
                ParametroAplicacionCL paramCL = new ParametroAplicacionCL(entityContext, loggingService, redisCacheWrapper, _configService, null, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                Dictionary<string, string> listaIdiomasPlataforma = paramCL.ObtenerListaIdiomasDictionary();
                paramCL.Dispose();

                if (UtilCadenas.RegexPrefijoIdioma.IsMatch(segmentos[0]))
				{
                    if (listaIdiomasPlataforma.ContainsKey(segmentos[0]))
                    {
                        idioma = segmentos[0];
                    }
                    utilIdiomas = new UtilIdiomas(idioma, loggingService, entityContext, _configService, redisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);
                    comunidadTxt = utilIdiomas.GetText("COMMON", "COMUNIDAD").ToLower();
                    if (segmentos.Length > 2 && !string.IsNullOrEmpty(segmentos[2]))
                    {
                        string comunidadSegmento = segmentos[1].ToLower();
                        if (comunidadSegmento.Equals(comunidadTxt))
                        {
                            nombreCortoComunidad = segmentos[2];
                        }

                        for (int i = 3; i < segmentos.Length; i++)
                        {
                            rutaPedida = $"{rutaPedida}/{segmentos[i]}";
                        }
                        rutaPedida = rutaPedida.TrimStart('/');
                    }

                }
                else if (segmentos.Length > 1 && !string.IsNullOrEmpty(segmentos[1]))
                {
                    string comunidadSegmento = segmentos[0].ToLower();
                    if (comunidadSegmento.Equals(comunidadTxt))
                    {
                        nombreCortoComunidad = segmentos[1];
                    }
                    for (int i = 2; i < segmentos.Length; i++)
                    {
                        rutaPedida = $"{rutaPedida}/{segmentos[i]}";
                    }
                    rutaPedida = rutaPedida.TrimStart('/');
                }

                if (string.IsNullOrEmpty(nombreCortoComunidad))
                {
                    string proyectoDefectoIDstring = entityContext.ParametroAplicacion.Where(item => item.Parametro.Equals("ComunidadPrincipalID")).Select(item => item.Valor).FirstOrDefault();
                    if (!string.IsNullOrEmpty(proyectoDefectoIDstring))
                    {
                        Guid proyectoDefectoID = new Guid(proyectoDefectoIDstring);
                        nombreCortoComunidad = entityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(proyectoDefectoID)).First().NombreCorto;
                    }

                }

                if (rutaPedida.Contains('?'))
                {
                    rutaPedida = rutaPedida.Split('?')[0];
                }

                if (!string.IsNullOrEmpty(nombreCortoComunidad))
                {
                    Guid proyectoID = entityContext.Proyecto.Where(proyecto => proyecto.NombreCorto.Equals(nombreCortoComunidad)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();
                    
                    bool idiomasDisponibles = proyectoID == ProyectoAD.MetaProyecto ? entityContext.ParametroAplicacion.Where(item => item.Parametro.Equals("Idiomas")).Select(item => item.Valor).First().Split("&&&").Count() != 1 : entityContext.ParametroGeneral.Where(item => item.ProyectoID == proyectoID).Select(item => item.IdiomasDisponibles).First();
                    if (!proyectoID.Equals(Guid.Empty))
                    {
                        var listTabsDB = entityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(proyectoID) && !string.IsNullOrEmpty(item.Ruta) && !item.TipoPestanya.Equals((short)TipoPestanyaMenu.EnlaceInterno) && !item.TipoPestanya.Equals((short)TipoPestanyaMenu.EnlaceExterno)).Select(item => new { item.PestanyaID, item.TipoPestanya, item.Ruta }).ToList();
                        List<PestanyaRouteModel> listTabs = new List<PestanyaRouteModel>();
                        foreach (var tab in listTabsDB)
                        {
                            string[] rutas = tab.Ruta.Split("|||");
                            foreach (string ruta in rutas)
                            {
                                if (!string.IsNullOrEmpty(ruta))
                                {
                                    PestanyaRouteModel pestanyaRouteModel = new PestanyaRouteModel();
                                    if (ruta.Contains("@"))
                                    {
                                        string[] ruta_idioma = ruta.Split("@");
                                        pestanyaRouteModel.Idioma = ruta_idioma[1];
                                        pestanyaRouteModel.PestanyaID = tab.PestanyaID;
                                        pestanyaRouteModel.RutaPestanya = ruta_idioma[0];
                                        pestanyaRouteModel.TipoPestanya = tab.TipoPestanya;
                                    }
                                    else
                                    {
                                        pestanyaRouteModel.Idioma = "";
                                        pestanyaRouteModel.PestanyaID = tab.PestanyaID;
                                        pestanyaRouteModel.RutaPestanya = ruta;
                                        pestanyaRouteModel.TipoPestanya = tab.TipoPestanya;
                                    }
                                    listTabs.Add(pestanyaRouteModel);
                                }
                            }
                        }
                        if ((listTabs.Exists(item => item.RutaPestanya.Equals(rutaPedida) && item.Idioma.Equals(idioma)) && idiomasDisponibles) || (listTabs.Exists(item => item.RutaPestanya.Equals(rutaPedida)) && !idiomasDisponibles))
                        {
                            RouteValueTransformer.EliminarRutasProyecto(nombreCortoComunidad);
                            string ruta = "http://";
                            if (_configService.PeticionHttps())
                            {
                                ruta = "https://";
                            }
                            ruta = $"{ruta}{context.Request.Host}{context.Items["originalPath"]}";
                            //RedirectToRoute(mHttpContextAccessor.HttpContext.Items["originalPath"]);
                            return context.Items["originalPath"].ToString();
                        }
                        else
                        {
                            context.Items["statusCode"] = 404;
                            if (UtilCadenas.RegexPrefijoIdioma.IsMatch(segmentos[0])  && listaIdiomasPlataforma.ContainsKey(segmentos[0]))
                            {
                                return $"/{idioma}/{comunidadTxt}/{nombreCortoComunidad}/error";
                            }

                            return $"/{comunidadTxt}/{nombreCortoComunidad}/error";
                        }
                    }
                }
            }
            string rutaEjecucionWeb = _configService.ObtenerRutaEjecucionWeb();
            if (!string.IsNullOrEmpty(rutaEjecucionWeb))
            {
                //return $"/{rutaEjecucionWeb}{context.Items["originalPath"].ToString().Trim('/')}";
            }

            context.Items["statusCode"] = 404;
            return "/error";
        }

    }

}
