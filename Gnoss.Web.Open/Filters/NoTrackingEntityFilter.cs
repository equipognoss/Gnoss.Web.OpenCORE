using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.Web.Open.Filters
{
    public class NoTrackingEntityFilter : BaseActionFilterAttribute
    {
        private EntityContext _entityContext;
        public NoTrackingEntityFilter(EntityContext entityContext)
        {
            _entityContext = entityContext;
        }
        protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
        {
            //var doc = _entityContext.Documento.First();
            //var state = _entityContext.Entry(doc).State;
            _entityContext.SetTrackingFalse();
            //var doc1 = _entityContext.Documento.First();
            //var state1 = _entityContext.Entry(doc1).State;
        }
    }
}
