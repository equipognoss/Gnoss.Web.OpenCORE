using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class AdministrarNewsletterController : ControllerBaseWeb
    {

        public AdministrarNewsletterController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {

        }

        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }
            
            return View(IdentidadActual.FilaIdentidad.RecibirNewsLetter);
        }

        [HttpPost]
        public ActionResult GuardarCambios(bool RecibirNewsletter)
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }
            bool error = false;
            try
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Identidad identidad = mEntityContext.Identidad.FirstOrDefault(ident => ident.IdentidadID.Equals(IdentidadActual.Clave));

                if(identidad != null)
                {
                    if (RecibirNewsletter)
                    {
                        identidad.RecibirNewsLetter = true;
                    }
                    else
                    {
                        identidad.RecibirNewsLetter = false;
                    }
                    mEntityContext.SaveChanges();
                    identidadCN.Dispose();

                    IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Clave, IdentidadActual.PersonaID.Value);
                    identidadCL.Dispose();
                    IdentidadActual = null;

                    ControladorIdentidades.AccionEnServicioExternoProyecto(TipoAccionExterna.Edicion, IdentidadActual.Persona, ProyectoSeleccionado.Clave, IdentidadActual.Clave, "", "", null, null);
                }
            }
            catch (Exception)
            {
                error = true;
            }

            if (error)
            {
                return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }
            else
            {
                return GnossResultOK(UtilIdiomas.GetText("COMMON", "CAMBIOSCORRECTOS"));
            } 
        }
    }
}
