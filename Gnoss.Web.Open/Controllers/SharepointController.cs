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

namespace Gnoss.Web.Controllers
{
    public class SharepointController : ControllerBaseWeb
    {
        private string tenantID = "";
        private string clientID = "";
        private string clientSecret = "";
        private string baseUrl = "https://graph.microsoft.com/v1.0/";
        private string mToken = "";
        private bool mEsEnlaceSharepoint;
        private string urlFichero;
        private Guid documentoID;

        public string Token
        {
            get { return mToken; }
            set { mToken = value; }
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
            string base64Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url));
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

        public void GuardarArchivoDesdeAPI(string url)
        {
            string urlCodificada = CodificarUrl(url);
            string nombreFichero = ExtraerNombreFichero(urlCodificada);
            string peticion = $"{baseUrl}shares/{urlCodificada}/root/content";
            HttpWebRequest webRequest = null;

            webRequest = System.Net.WebRequest.Create(peticion) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 600000;
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("Authorization", "Bearer " + mToken);

            FileInfo archivoInfo = new FileInfo(nombreFichero);
            string extensionArchivo = System.IO.Path.GetExtension(archivoInfo.Name).ToLower();
            GestionDocumental gd = new GestionDocumental(mLoggingService);
            gd.Url = UrlServicioWebDocumentacion;
            Stream streamFichero = webRequest.GetResponse().GetResponseStream();
            byte[] buffer1;

            using (var memoryStream = new MemoryStream())
            {
                streamFichero.CopyTo(memoryStream);
                buffer1 = memoryStream.ToArray();
            }

            string idAuxGestorDocumental = gd.AdjuntarDocumento(buffer1, TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.ProyectoID, documentoID, extensionArchivo);
        }

        public bool ComprobarSiEstaAlineadoConSharepoint(string url)
        {
            string urlCodificada = CodificarUrl(url);
            string peticion = $"{baseUrl}shares/{urlCodificada}/root";
            string response = UtilGeneral.WebRequest("GET", peticion, mToken, null);
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
