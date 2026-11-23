using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;

namespace Gnoss.Web.Services.VirtualPathProvider
{
    /// <summary>
    /// Comprueba si la página ha sido cambiada para reemplazarla por la versión de caché
    /// </summary>
    public class BDChangeToken : IChangeToken
    {
        readonly private string _viewPath;
        readonly private IHttpContextAccessor _httpContextAccessor;

        public BDChangeToken(string viewPath, IHttpContextAccessor httpContextAccessor)
        {
            _viewPath = viewPath;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool ActiveChangeCallbacks => false;

        public bool HasChanged
        {
            get
            {
                if (_viewPath.Contains("$$$") && !BDVirtualPath.ListaRutasVirtuales.ContainsKey(_viewPath))
                {
                    // Razor descartará la vista compilada y la volverá a pedir y compilar
                    if (LoggingService.TrazaHabilitada)
                    {
                        _httpContextAccessor?.HttpContext?.RequestServices?.GetService<LoggingService>()?.AgregarEntrada($"BDChangeToken: vista cambiada, se invalidará la vista compilada {_viewPath}");
                    }
                    return true;
                }
                return false;
            }
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return EmptyDisposable.Instance;
        }
    }

    internal class EmptyDisposable : IDisposable
    {
        public static EmptyDisposable Instance { get; } = new EmptyDisposable();
        private EmptyDisposable() { }
        public void Dispose()
        {
            // No hace nada.
        }
    }
}
