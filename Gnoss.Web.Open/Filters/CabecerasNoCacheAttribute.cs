using Es.Riam.Gnoss.Web.MVC.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Gnoss.Web.Open.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CabecerasNoCacheAttribute : ActionFilterAttribute
    {
        private readonly string _queryParam;
        private readonly string _queryValue;

        public CabecerasNoCacheAttribute(string queryParam = "", string queryValue = "")
        {
            _queryParam = queryParam;
            _queryValue = queryValue;
        }
        /// <summary>
        /// Añade las siguientes cabeceras a la respuesta de una petición para evitar que el navegador
        /// almacene en caché ni recupere del historial páginas con contenido autenticado/sensible:
        /// Cache-Control: no-store, no-cache, must-revalidate
        /// Pragma: no-cache
        /// Expires: 0
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!string.IsNullOrEmpty(_queryParam) && ((ControllerBaseWeb)context.Controller).RequestParams(_queryParam) != _queryValue)
            {
                base.OnResultExecuting(context);
                return;
            }

            var headers = context.HttpContext.Response.Headers;

            headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            headers["Pragma"] = "no-cache";
            headers["Expires"] = "0";

            base.OnResultExecuting(context);
        }
    }
}
