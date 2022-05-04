using Es.Riam.Gnoss.Web.MVC.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Es.Riam.Gnoss.Web.MVC.Filters
{
    public abstract class BaseActionFilterAttribute : ActionFilterAttribute
    {
        
        #region Métodos

        public override void OnActionExecuting(ActionExecutingContext pFilterContext)
        {
            RealizarComprobaciones(pFilterContext);
        }

        protected abstract void RealizarComprobaciones(ActionExecutingContext pFilterContext);

        protected void Redireccionar(string pUrl,ActionExecutingContext pFilterContext)
        {
            pFilterContext.Result = Controlador(pFilterContext).DevolverActionResult(pUrl);
            
        }

        #endregion

        #region Propiedades

        protected ControllerBaseWeb Controlador(ActionExecutingContext pFilterContext)
        {

            return (ControllerBaseWeb)pFilterContext.Controller;
          
        }

        #endregion

    }
}