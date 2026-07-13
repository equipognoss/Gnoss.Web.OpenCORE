using Es.Riam.Gnoss.Web.MVC.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace Gnoss.Web.Open.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CabecerasCspAttribute : ActionFilterAttribute
    {
        private const string HEADER_CSP = "Content-Security-Policy";
        private readonly string _queryParam;
        private readonly string _queryValue;

        public CabecerasCspAttribute(string queryParam = "", string queryValue = "")
        {
            _queryParam = queryParam;
            _queryValue = queryValue;
        }

        /// <summary>
        /// Añade las siguientes cabeceras a la respuesta de una peticion para que no renderice la vista en un IFrame:
        /// Content-Security-Policy; frame-ancestors 'none'
        /// X-Frame-Options DENY
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

            headers.TryAdd("X-Frame-Options", "DENY");

            if (headers.TryGetValue(HEADER_CSP, out var csp))
            {
                headers[HEADER_CSP] = $"{csp}; frame-ancestors 'none'";
            }
            else
            {
                headers.Append(HEADER_CSP, "frame-ancestors 'none'");
            }

            base.OnResultExecuting(context);
        }
    }
}
