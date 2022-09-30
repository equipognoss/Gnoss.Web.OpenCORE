using Es.Riam.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.CL;
using Microsoft.AspNetCore.Hosting;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Http;
using Es.Riam.Gnoss.AD.Virtuoso;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.AbstractsOpen;
using Es.Riam.InterfacesOpen;
using System.Linq;

namespace Gnoss.Web.Controllers
{
    public class SharepointController : ControllerBaseWeb
    {
        private string tenantID = "";
        private string clientID = "";
        private string clientSecret = "";
        private string baseUrl = "https://graph.microsoft.com/v1.0/";
        private string microsoftLoginEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        private string mToken = "";
        private string mTokenCreadorRecurso = "";
        private bool mEsEnlaceSharepoint;
        private string urlFichero;
        private Guid documentoID;

        public string Token
        {
            get { return mToken; }
            set { mToken = value; }
        }

        public string TokenCreadorRecurso
        {
            get { return mTokenCreadorRecurso; }
            set { mTokenCreadorRecurso = value; }
        }

        public SharepointController(string documentoID, LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
            ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, servicesUtilVirtuosoAndReplication);

            tenantID = parametroAplicacionCN.ObtenerParametroAplicacion("SharepointTenantID");
            clientSecret = parametroAplicacionCN.ObtenerParametroAplicacion("SharepointClientSecret");
            clientID = parametroAplicacionCN.ObtenerParametroAplicacion("SharepointClientID");
            
            if (!string.IsNullOrEmpty(documentoID))
            {
                this.documentoID = new Guid(documentoID);
            }
            else
            {
                this.documentoID = new Guid();
            }                      
        }

        public string RenovarToken(string pRefreshToken, Guid pUsuarioID)
        {
            try
            {
                string peticion = $"{microsoftLoginEndpoint}";
                string requestParameters = $"client_id={clientID}&grant_type=refresh_token&refresh_token={pRefreshToken}&client_secret={clientSecret}";
                byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
                string response = UtilGeneral.WebRequest("POST", peticion, byteData);
                dynamic respuestaObj = JsonConvert.DeserializeObject(response);
                //Almacenamos los nuevos tokens en BD (los tokens de refresco duran 90 dias)
                string token = respuestaObj.access_token;
                string refreshTokenNuevo = respuestaObj.refresh_token;

                ActualizarTokensUsuario(pUsuarioID, token, refreshTokenNuevo);

                return token;
            }
            catch (Exception ex)
            {
                GuardarLogError($"Se ha producido un error al renovar el Token de SharePoint: {ex.Message}");
                return "";
            }
            
        }

        public void ActualizarTokensUsuario(Guid pUsuarioID, string pToken, string pTokenRefresco)
        {
            UsuarioVinculadoLoginRedesSociales usuarioSP = mEntityContext.UsuarioVinculadoLoginRedesSociales.Where(parametroApp => parametroApp.UsuarioID.Equals(pUsuarioID) && parametroApp.TipoRedSocial == 6).FirstOrDefault();
            UsuarioVinculadoLoginRedesSociales usuarioRefresco = mEntityContext.UsuarioVinculadoLoginRedesSociales.Where(parametroApp => parametroApp.UsuarioID.Equals(pUsuarioID) && parametroApp.TipoRedSocial == 7).FirstOrDefault();

            usuarioSP.IDenRedSocial = pToken;
            usuarioRefresco.IDenRedSocial = pTokenRefresco;
            mEntityContext.UsuarioVinculadoLoginRedesSociales.Update(usuarioSP);
            mEntityContext.UsuarioVinculadoLoginRedesSociales.Update(usuarioRefresco);
            mEntityContext.SaveChanges();
        }

        public bool ComprobarValidezToken(string token)
        {
            string peticionComprobarToken = $"{baseUrl}me";
            string response = UtilGeneral.WebRequest("GET", peticionComprobarToken, token, null);

            if (response != "")
            {
                return true;
            }
            
            return false;
        }

        public string CodificarUrl(string url)
        {
            string[] enlaceConNombre = url.Split("|||");
            string enlace = url;

            if (enlaceConNombre.Length > 1)
            {
                enlace = enlaceConNombre[0];
            }

            string base64Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(enlace));
            string encodedUrl = "u!" + base64Value.TrimEnd('=').Replace('/', '_').Replace('+', '-');

            return encodedUrl;
        }

        public string ExtraerNombreFichero(string urlCodificada)
        {
            string peticion = $"{baseUrl}shares/{urlCodificada}";
            string response = UtilGeneral.WebRequest("GET", peticion, mToken, null);
            dynamic respuestaObj = JsonConvert.DeserializeObject(response);
            string nombre = respuestaObj.name;

            return nombre;
        }

        public string ObtenerNombreFichero(string url)
        {
            string urlCodificada = CodificarUrl(url);
            string peticion = $"{baseUrl}shares/{urlCodificada}";
            string response = UtilGeneral.WebRequest("GET", peticion, mToken, null);
            dynamic respuestaObj = JsonConvert.DeserializeObject(response);
            string nombre = respuestaObj.name;

            return nombre;
        }

        public string ExtraerExtensionFicheroAPI(string url)
        {
            string urlCodificada = CodificarUrl(url);
            string nombreFichero = ExtraerNombreFichero(urlCodificada);
            int ultimoPunto = nombreFichero.LastIndexOf(".");
            string extension = nombreFichero.Substring(ultimoPunto);

            return extension;

        }
        public string ExtraerNombreSinExtension(string url)
        {
            string urlCodificada = CodificarUrl(url);
            string peticion = $"{baseUrl}shares/{urlCodificada}";
            string response = UtilGeneral.WebRequest("GET", peticion, mToken, null);
            dynamic respuestaObj = JsonConvert.DeserializeObject(response);
            string nombreCompleto = respuestaObj.name;
            int punto = nombreCompleto.LastIndexOf(".");
            string nombre = nombreCompleto.Substring(0, punto);

            return nombre;
        }

        public string GuardarArchivoDesdeAPI(string url)
        {
            string urlCodificada = CodificarUrl(url);
            string nombreFichero;

            if (url.Contains("|||") && !string.IsNullOrEmpty(url.Split("|||")[1]))
            {
                nombreFichero = url.Split("|||")[1];
            }
            else
            {
                nombreFichero = ExtraerNombreFichero(urlCodificada);
            }

            string peticion = $"{baseUrl}shares/{urlCodificada}/root/content";
            HttpWebRequest webRequest = null;

            webRequest = System.Net.WebRequest.Create(peticion) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 600000;
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("Authorization", "Bearer " + mToken);

            FileInfo archivoInfo = new FileInfo(nombreFichero);
            string extensionArchivo = Path.GetExtension(archivoInfo.Name).ToLower();
            GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService);
            gd.Url = UrlServicioWebDocumentacion;
            Stream streamFichero = webRequest.GetResponse().GetResponseStream();
            byte[] buffer1;

            using (var memoryStream = new MemoryStream())
            {
                streamFichero.CopyTo(memoryStream);
                buffer1 = memoryStream.ToArray();
            }

            gd.AdjuntarDocumento(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.ProyectoID, documentoID, extensionArchivo);

            return nombreFichero;
        }

        public bool ComprobarSiEstaAlineadoConSharepoint(string url)
        {
            try
            {
                return PeticionAlineadoConSharepoint(url);
            }
            catch
            {
                if(!string.IsNullOrEmpty(mTokenCreadorRecurso) && !mTokenCreadorRecurso.Equals(mToken))
                {
                    try { PeticionAlineadoConSharepoint(url, true); }
                    catch { return true; }
                }
                return true;
            } 
        }

        private bool PeticionAlineadoConSharepoint(string url, bool pEsCreadorRecurso = false)
        {
            string token = !pEsCreadorRecurso ? mToken : mTokenCreadorRecurso;

            string urlCodificada = CodificarUrl(url);
            string peticion = $"{baseUrl}shares/{urlCodificada}/root";
            string response = UtilGeneral.WebRequest("GET", peticion, token, null);
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }
            dynamic respuestaObj = JsonConvert.DeserializeObject(response);

            string fechaModificacionApi = respuestaObj.lastModifiedDateTime;
            DateTime fechaModificacionSP = DateTime.Parse(fechaModificacionApi, System.Globalization.CultureInfo.InvariantCulture);
            long fechaModificacionSharepoint = fechaModificacionSP.Ticks;

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            long fechaCreacionVersion = docCN.ObtenerFechaCreacionDocumento(documentoID);
            return !(fechaModificacionSharepoint > fechaCreacionVersion);
        }

    }

}
