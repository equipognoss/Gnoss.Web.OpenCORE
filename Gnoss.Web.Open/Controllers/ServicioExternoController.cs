using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class ServicioExternoController : ControllerBaseWeb
    {
        public ServicioExternoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }


        public ActionResult Index(string pNombreServicio, string pNombreAccion)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            string urlServicio = proyCN.ObtenerUrlServicioExterno(ProyectoSeleccionado.Clave, pNombreServicio);
            proyCN.Dispose();
            if (!string.IsNullOrEmpty(urlServicio))
            {


                urlServicio = urlServicio + "/" + pNombreAccion;
                Dictionary<string, string> parametros = new Dictionary<string, string>();
                if (Request != null && Request.Query != null && Request.Query.Count > 0)
                {
                    foreach (string claveParametro in Request.Query.Keys)
                    {
                        parametros.Add(claveParametro, Request.Query[claveParametro]);
                    }
                }

                //pIdentidadID          (identidad del usuario, en caso de que tenga )
                //pOrganizacionID       (id de la organizacion de usuario, en caso de que tenga)
                //userID                (id del usuario, en caso de que tenga)
                //pPerfilID             (id del perfil del usuario, en caso de que tenga)
                //pEmail                (email del usuario, en caso de que tenga)
                //pProyectoID           (id del proyecto, siemrpe)
                //communityShortName    (nomrbecorto de la comunidad, siemrpe)
                //pAsincrono            (booleano que marca la petición como asíncrona)
                //UserLanguaje          (idioma en el que el usuario está navegando)

                if (IdentidadActual != null)
                {
                    parametros.Add("pIdentidadID", IdentidadActual.Clave.ToString());
                    if (IdentidadActual.OrganizacionID.HasValue)
                    {
                        parametros.Add("pOrganizacionID", IdentidadActual.OrganizacionID.Value.ToString());
                    }
                    if (mControladorBase.UsuarioActual != null)
                    {
                        parametros.Add("userID", mControladorBase.UsuarioActual.UsuarioID.ToString());
                    }
                    parametros.Add("pPerfilID", IdentidadActual.PerfilID.ToString());
                    parametros.Add("pEmail", IdentidadActual.Persona.FilaPersona.Email.ToString());
                }
                parametros.Add("pProyectoID", ProyectoSeleccionado.Clave.ToString());
                parametros.Add("communityShortName", ProyectoSeleccionado.NombreCorto);
                parametros.Add("UserLanguaje", UtilIdiomas.LanguageCode);

                if (Request.Form.Files.Count > 0)
                {
                    foreach (IFormFile fichero in Request.Form.Files)
                    {
                        MemoryStream target = new MemoryStream();
                        fichero.OpenReadStream().CopyTo(target);
                        byte[] data = target.ToArray();

                        parametros.Add(fichero.FileName, Convert.ToBase64String(data));
                    }
                }

                if (parametros != null && parametros.ContainsKey("pAsincrono") && parametros["pAsincrono"].ToLower().Equals("true"))
                {
                    try
                    {
                        //lanzo la ejecución en un nuevo hilo                                
                        Task.Factory.StartNew(() => UtilWeb.HacerPeticionPostDevolviendoWebResponse(urlServicio, parametros));
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex);
                    }

                    return new EmptyResult();
                }
                else
                {
                    try
                    {
                        WebResponse respuesta = UtilWeb.HacerPeticionPostDevolviendoWebResponse(urlServicio, parametros);
                        Stream stream = respuesta.GetResponseStream();

                        Dictionary<string, string> cabecera = new Dictionary<string, string>();
                        //Preparamos cabecera          
                        for (int i = 0; i < respuesta.Headers.Count; ++i)
                        {
                            cabecera.Add(respuesta.Headers.Keys[i], respuesta.Headers[i]);
                        }

                        return new SimpleResult(cabecera, stream, respuesta.ContentType);
                    }
                    catch(WebException ex)
                    {
                        Response.StatusCode = 502;
                        return Content($"Error in request {urlServicio}: {ex.Message}");
                    }
                }
            }
            else
            {
                Response.StatusCode = 502;
                return Content($"Error in request {urlServicio}: not found");
            }
        }
    }
}
