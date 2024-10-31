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
using System.Linq;
using Gnoss.Web.Open.Filters;
using System.Buffers;
using System.Net.Http;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Microsoft.Extensions.Hosting;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class ServicioExternoController : ControllerBaseWeb
    {
        public ServicioExternoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }


        public ActionResult Index(string pNombreServicio, string pNombreAccion)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            string urlServicio = proyCN.ObtenerUrlServicioExterno(ProyectoSeleccionado.Clave, pNombreServicio);
            proyCN.Dispose();
            if (!string.IsNullOrEmpty(urlServicio))
            {
                if (!urlServicio.EndsWith("/"))
                {
                    urlServicio = $"{urlServicio}/";
                }

                urlServicio = urlServicio + pNombreAccion;
                Dictionary<string, string> parametros = new Dictionary<string, string>();
                if (Request != null && Request.Query != null && Request.Query.Count > 0)
                {
                    foreach (string claveParametro in Request.Query.Keys)
                    {
                        parametros.Add(claveParametro, Request.Query[claveParametro]);
                    }
                }

                if(Request != null && !Request.Method.Equals("GET") && Request.Form != null && Request.Form.Keys.Count > 0)
                {
                    foreach (string claveParametro in Request.Form.Keys)
                    {
                        parametros.Add(claveParametro, Request.Form[claveParametro]);
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

                if (Request.Method != "GET" && Request.Form != null && Request.Form.Files.Count > 0)
                {
                    foreach (IFormFile fichero in Request.Form.Files)
                    {
                        MemoryStream target = new MemoryStream();
                        fichero.OpenReadStream().CopyTo(target);
                        byte[] data = target.ToArray();

                        parametros.Add(fichero.Name, Convert.ToBase64String(data));
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

                        return GenerarRespuesta(respuesta);
                    }
                    catch(WebException ex)
                    {
                        //Response.StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                        //Response.ContentType = ((HttpWebResponse)ex.Response).ContentType;
                        string response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        string error = $"Error {Response.StatusCode} al enviar una petición al servicio {urlServicio} con los parámetros: {string.Join("\n", parametros.Select(item => $"{item.Key} -> {item.Value}"))} Error: {response}";
                        GuardarLogError(ex, error);

                        return new ContentResult() { Content = response, ContentType = ((HttpWebResponse)ex.Response).ContentType, StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode };
                        //return GnossResultERROR(mensajeRespuesta);
                    }
                }
            }
            else
            {
                Response.StatusCode = 502;
                return Content($"Error in request {urlServicio}: not found");
            }
        }

        public ActionResult GenerarRespuesta(WebResponse pRespuesta)
        {
            Stream stream = pRespuesta.GetResponseStream();

            Response.ContentType = pRespuesta.ContentType;
            string body = "";

            if (!pRespuesta.ContentType.StartsWith("application") || pRespuesta.ContentType.Equals("application/json"))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    body = sr.ReadToEnd();
                }

                return Content(body, pRespuesta.ContentType);
            }
            else
            {
                if (pRespuesta.Headers["Content-Disposition"]!= null)
                {
                    Response.Headers.ContentDisposition = pRespuesta.Headers["Content-Disposition"];
				}
				using (BinaryReader sr = new BinaryReader(stream))
				{
					byte[] buffer = new byte[4098];
                    while(buffer.Length > 0 )
                    {
						buffer = sr.ReadBytes(4098);
                        Response.Body.WriteAsync(buffer);
					}
                    Response.Body.FlushAsync();
				}

                return new EmptyResult();
			}
        }
    }
}
