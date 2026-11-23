using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gnoss.Web.Services.Diagnostico
{
    /// <summary>
    /// Escucha los eventos de diagnóstico que emite ASP.NET Core MVC al ejecutar resultados de acción y
    /// al renderizar cada página Razor (vista, layout y vistas parciales) y los escribe en la traza de la petición.
    /// Solo se activa cuando la traza está habilitada, por lo que no tiene coste en funcionamiento normal.
    /// </summary>
    public class TrazaRenderizadoVistasObserver : IObserver<KeyValuePair<string, object?>>
    {
        private const string EVENTO_INICIO_RESULTADO = "Microsoft.AspNetCore.Mvc.BeforeActionResult";
        private const string EVENTO_FIN_RESULTADO = "Microsoft.AspNetCore.Mvc.AfterActionResult";
        private const string EVENTO_INICIO_VISTA = "Microsoft.AspNetCore.Mvc.Razor.BeforeViewPage";
        private const string EVENTO_FIN_VISTA = "Microsoft.AspNetCore.Mvc.Razor.AfterViewPage";

        private const string CLAVE_INICIOS_VISTAS = "TrazaRenderizadoVistas_Inicios";
        private const string CLAVE_INICIO_RESULTADO = "TrazaRenderizadoVistas_InicioResultado";

        /// <summary>
        /// Predicado para DiagnosticListener.Subscribe: MVC solo construye y envía el evento si devuelve true.
        /// </summary>
        public static bool EstaHabilitado(string pNombreEvento)
        {
            if (!LoggingService.TrazaHabilitada)
            {
                return false;
            }

            return pNombreEvento == EVENTO_INICIO_RESULTADO || pNombreEvento == EVENTO_FIN_RESULTADO || pNombreEvento == EVENTO_INICIO_VISTA || pNombreEvento == EVENTO_FIN_VISTA;
        }

        public void OnNext(KeyValuePair<string, object?> pEvento)
        {
            try
            {
                switch (pEvento.Value)
                {
                    case BeforeActionResultEventData inicioResultado:
                        {
                            HttpContext httpContext = inicioResultado.ActionContext.HttpContext;
                            httpContext.Items[CLAVE_INICIO_RESULTADO] = Stopwatch.GetTimestamp();
                            AgregarEntrada(httpContext, $"Inicio ejecución resultado {inicioResultado.Result?.GetType().Name}");
                            break;
                        }
                    case AfterActionResultEventData finResultado:
                        {
                            HttpContext httpContext = finResultado.ActionContext.HttpContext;
                            AgregarEntrada(httpContext, $"Fin ejecución resultado {finResultado.Result?.GetType().Name} ({ObtenerMilisegundos(httpContext.Items[CLAVE_INICIO_RESULTADO] as long?)} ms)");
                            break;
                        }
                    case BeforeViewPageEventData inicioVista:
                        {
                            ObtenerIniciosVistas(inicioVista.HttpContext)[inicioVista.Page] = Stopwatch.GetTimestamp();
                            AgregarEntrada(inicioVista.HttpContext, $"Inicio vista {inicioVista.Page?.Path}");
                            break;
                        }
                    case AfterViewPageEventData finVista:
                        {
                            Dictionary<IRazorPage, long> inicios = ObtenerIniciosVistas(finVista.HttpContext);
                            long? inicio = null;
                            if (finVista.Page != null && inicios.Remove(finVista.Page, out long valor))
                            {
                                inicio = valor;
                            }
                            AgregarEntrada(finVista.HttpContext, $"Fin vista {finVista.Page?.Path} ({ObtenerMilisegundos(inicio)} ms)");
                            break;
                        }
                }
            }
            catch
            {
                // La traza nunca debe romper el renderizado
            }
        }

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        private static void AgregarEntrada(HttpContext pHttpContext, string pMensaje)
        {
            LoggingService loggingService = pHttpContext?.RequestServices?.GetService<LoggingService>();
            loggingService?.AgregarEntrada(pMensaje);
        }

        private static Dictionary<IRazorPage, long> ObtenerIniciosVistas(HttpContext pHttpContext)
        {
            if (pHttpContext.Items[CLAVE_INICIOS_VISTAS] is not Dictionary<IRazorPage, long> inicios)
            {
                inicios = new Dictionary<IRazorPage, long>();
                pHttpContext.Items[CLAVE_INICIOS_VISTAS] = inicios;
            }
            return inicios;
        }

        private static string ObtenerMilisegundos(long? pInicio)
        {
            if (!pInicio.HasValue)
            {
                return "?";
            }
            return ((int)Stopwatch.GetElapsedTime(pInicio.Value).TotalMilliseconds).ToString();
        }
    }
}
