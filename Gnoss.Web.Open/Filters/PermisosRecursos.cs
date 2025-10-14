using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.RDF;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Gnoss.Web.Open.Filters
{
	public class PermisosRecursos : PermisosPaginasUsuariosAttribute
	{
		public PermisosRecursos(ulong[] pPermisos, LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ILogger<PermisosRecursos> logger, ILoggerFactory loggerFactory, bool pComprobarPermisoUsuarioNoAdministrador = false) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, logger, loggerFactory, pComprobarPermisoUsuarioNoAdministrador)
		{
			mLoggingService = loggingService;
			mConfigService = configService;
			mEntityContext = entityContext;
			mRedisCacheWrapper = redisCacheWrapper;
			mVirtuosoAD = virtuosoAD;
			mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null, loggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);
			TipoDePermisoRequerido = TipoDePermiso.Recursos;
			if (pPermisos == null)
			{
				PermisosRequeridosParaAcceso = [];
			}
			else
			{
				PermisosRequeridosParaAcceso = pPermisos;
			}
		}

		protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
		{
			base.RealizarComprobaciones(pFilterContext);
		}
	}
}
