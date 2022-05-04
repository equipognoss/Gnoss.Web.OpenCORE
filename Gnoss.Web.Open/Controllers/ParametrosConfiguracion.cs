using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using Es.Riam.InterfacesOpen;

/// <summary>
/// Descripción breve de ParametrosConfiguracion
/// </summary>
[System.ComponentModel.ToolboxItem(false)]
// Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
public class ParametrosConfiguracionController : ControllerBaseWeb
{

    public ParametrosConfiguracionController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
    : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
    {
    }

    [HttpGet]
    public string UrlServicioResultados()
    {
        return mConfigService.ObtenerUrlServicioResultados();
    }

    [HttpGet]
    public string UrlPropiaProyecto(Guid pProyectoID)
    {
        ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
        string urlPropia = proyectoCN.ObtenerURLPropiaProyecto(pProyectoID);
        proyectoCN.Dispose();
        return urlPropia;
    }

    [HttpGet]
    public Guid ProyectoIDPorNombreCorto(string pNombreCorto)
    {
        Guid proyectoID = Guid.Empty;

        ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
        List<Guid> listaproyID = proyectoCN.ObtenerProyectoIDOrganizacionIDPorNombreCorto(pNombreCorto);
        if (listaproyID != null && listaproyID.Any())
        {
            proyectoID = listaproyID[1];
        }
        else
        {
            //Comprobamos si pNombreCorto es el dominio actual
            string dominioActual = UtilDominios.ObtenerDominioUrl(Request.Path, true);
            dominioActual = dominioActual.Substring(dominioActual.LastIndexOf("/") + 1);

            if (dominioActual == pNombreCorto)
            {
                listaproyID = proyectoCN.ObtenerProyectoIDOrganizacionIDPorNombreCorto(new RouteConfig(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD).NombreProyectoSinNombreCorto);
                if (listaproyID != null && listaproyID.Any())
                {
                    proyectoID = listaproyID[1];
                }
            }
        }
        proyectoCN.Dispose();

        if (proyectoID == Guid.Empty)
        {
            throw new ExcepcionGeneral("No existe un proyecto para " + pNombreCorto);
        }
        return proyectoID;
    }

}
