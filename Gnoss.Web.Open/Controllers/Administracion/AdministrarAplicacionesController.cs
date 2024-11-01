﻿using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.LogicaOAuth.OAuth;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.OAuthAD.OAuth;
using Es.Riam.Gnoss.OAuthAD.OAuth.EncapsuladoDatos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ParametroAplicacionGBD;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.OAuth;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.Extensions.Hosting;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Permite al usuario administrar sus aplicaciones y tokens de acceso
    /// </summary>
    public class AdministrarAplicacionesController : ControllerBaseWeb
    {
        private OAuthCN mOAuthCN;
        private EntityContextOauth mEntityContextOauth;

        public AdministrarAplicacionesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, EntityContextOauth entityContextOauth, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
            mEntityContextOauth = entityContextOauth;
            mOAuthCN = new OAuthCN(mEntityContext, mLoggingService, mConfigService, entityContextOauth, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Pagina principal donde se muestran todas las aplicaciones del usuario.
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "configuracion oauth-configuracion max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_OAuth;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "OAUTH");

            OAuthModel mOAuthModel = ObtenerOAuth();

            return View(mOAuthModel);
        }

        public OAuthModel ObtenerOAuth()
        {
			UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			ConsumerModel modelo;
			ConsumerData consumerData;
			Gnoss.Logica.Documentacion.DocumentacionCN documentoCN = new Gnoss.Logica.Documentacion.DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
			DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
			OAuthToken tokens;
			OAuthModel oauthModel = new OAuthModel();

			AdministrarAplicacionesViewModel mAplicaciones = mOAuthCN.ObtenerAplicacionesPorUsuarioIDyProyectoID(mControladorBase.UsuarioActual.UsuarioID, ProyectoSeleccionado.Clave);
			if (mAplicaciones.Aplicaciones == null || mAplicaciones.Aplicaciones.Count == 0)
			{
				//Se genera un consumer model donde guardar los datos y se almacena en BD
				modelo = new ConsumerModel(0, "Automatico", "", "", "", "");

				try
				{
					oauthModel = CrearTokens(modelo.Name);
				}
				catch (Exception ex)
				{
					GuardarLogError(mLoggingService.DevolverCadenaError(ex, ""));
				}
			}
			else
			{
				modelo = mAplicaciones.Aplicaciones.FirstOrDefault();
				tokens = mOAuthCN.ConsultarTokensPorUsuarioIDyConsumerId(mControladorBase.UsuarioActual.UsuarioID, modelo.ConsumerId);

				oauthModel.ConsumerKey = modelo.Key;
				oauthModel.ConsumerSecret = modelo.Secret;
				oauthModel.TokenKey = tokens.Token;
				oauthModel.TokenSecret = tokens.TokenSecret;
			}

			consumerData = mOAuthCN.ObtenerConsumerPorConsumerId(modelo.ConsumerId).ConsumerData.FirstOrDefault();
			documentoCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, false, false);

			oauthModel.API = mConfigService.ObtenerUrlApi();
			oauthModel.DevEmail = usuarioCN.ObtenerEmailPorUsuarioID(mControladorBase.UsuarioActual.UsuarioID);
			oauthModel.OntologyName = dataWrapperDocumentacion.ListaDocumento.FirstOrDefault()?.Enlace.Replace(".owl","");
			oauthModel.ShortName = ProyectoSeleccionado.NombreCorto;
			oauthModel.LogPath = $"{Path.DirectorySeparatorChar}log";
			oauthModel.LogFileName = "trace.log";
			oauthModel.LogLevel = "DEBUG";

			if (oauthModel.OntologyName == null)
			{
				oauthModel.OntologyName = "";
			}

            return oauthModel;
		}

		/// <summary>
		/// Crea y descarga un XML con los datos de config
		/// </summary>
		/// <returns>Descarga XML</returns>
		[TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
        public ActionResult DescargarXML()
        {
            OAuthModel model = ObtenerOAuth();
			//Se genera el XML con los datos aportados
			byte[] bytes = CrearXML(model);
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"oAuth_{model.ShortName}.config\"");
            //Content-Disposition: attachment; filename="filename.jpg"
            return File(bytes, "text/xml");
        }

        private OAuthModel CrearTokens(string pNombre)
        {
            OAuthModel oAuthModel = new OAuthModel(pNombre);

            OAuthConsumer filaOAuthConsumer = new OAuthConsumer();
            filaOAuthConsumer.ConsumerKey = GenerarTokens();
            filaOAuthConsumer.ConsumerSecret = GenerarTokens();
            filaOAuthConsumer.VerificationCodeFormat = 1;
            filaOAuthConsumer.VerificationCodeLength = 1;

            mEntityContextOauth.OAuthConsumer.Add(filaOAuthConsumer);

            mOAuthCN.ActualizarBD();

            ConsumerData filaConsumer = new ConsumerData();
            filaConsumer.ConsumerId = filaOAuthConsumer.ConsumerId;
            filaConsumer.Nombre = pNombre;
            filaConsumer.UrlOrigen = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            filaConsumer.FechaAlta = DateTime.Now;

            mEntityContextOauth.ConsumerData.Add(filaConsumer);

            mOAuthCN.ActualizarBD();

            if (!mEntityContextOauth.Usuario.Any(item => item.UsuarioID.Equals(mControladorBase.UsuarioActual.UsuarioID)))
            {
                Usuario filaUsuario = new Usuario();
                filaUsuario.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                filaUsuario.Login = mControladorBase.UsuarioActual.Login;

                mEntityContextOauth.Usuario.Add(filaUsuario);
                mOAuthCN.ActualizarBD();
            }

            UsuarioConsumer filaUsuarioConsumer = new UsuarioConsumer();
            filaUsuarioConsumer.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
            filaUsuarioConsumer.ConsumerId = filaOAuthConsumer.ConsumerId;
            filaUsuarioConsumer.ProyectoID = ProyectoSeleccionado.Clave;

            mEntityContextOauth.UsuarioConsumer.Add(filaUsuarioConsumer);

            mOAuthCN.ActualizarBD();

            OAuthToken filaToken = new OAuthToken();
            filaToken.Token = GenerarTokens();
            filaToken.TokenSecret = GenerarTokens();
            filaToken.State = 2;
            filaToken.IssueDate = DateTime.Now;
            filaToken.ConsumerId = filaOAuthConsumer.ConsumerId;
            filaToken.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
            filaToken.Scope = null;
            filaToken.RequestTokenVerifier = null;
            filaToken.ConsumerVersion = "1.0.1";

            mEntityContextOauth.OAuthToken.Add(filaToken);

            //mEntityContextOauth.PinToken.Add(filaPinToken);

            mOAuthCN.ActualizarBD();

            oAuthModel.ConsumerKey = filaOAuthConsumer.ConsumerKey;
            oAuthModel.ConsumerSecret = filaOAuthConsumer.ConsumerSecret;
            oAuthModel.TokenKey = filaToken.Token;
            oAuthModel.TokenSecret = filaToken.TokenSecret;

            return oAuthModel;
        }

        /// <summary>
        /// Crea el XML de config con todos los datos
        /// </summary>
        /// <param name="URLAPI">URLAPI</param>
        /// <param name="CShortName">nombre corto del community</param>
        /// <param name="ontologyName">nombre de la ontologia</param>
        /// <param name="devEmail">email del developer</param>
        /// <param name="tokenKey">token</param>
        /// <param name="tokenSecret">tokenSecret</param>
        /// <param name="consumerKey">consumerKey</param>
        /// <param name="consumerSecret">consumerSecret</param>
        /// <returns>Devuelve el XML en forma de byte[] para poder ser enviado</returns>
        private byte[] CrearXML(OAuthModel xmlModel)
        {
            //Creacion del XML y su encabezado
            XDeclaration declaracionXML = new XDeclaration("1.0", "UTF-8", null);
            XDocument documentoXML = new XDocument(declaracionXML);

            //Creacion del nodo raiz
            XElement raiz = new XElement("config");
            documentoXML.Add(raiz);

            XElement nodoApi = new XElement("apiEndpoint");
            nodoApi.SetValue(xmlModel.API);
            raiz.Add(nodoApi);

            XElement nodoShortName = new XElement("communityShortName");
            nodoShortName.SetValue(xmlModel.ShortName);
            raiz.Add(nodoShortName);

            XElement nodoOntology = new XElement("ontologyName");
            nodoOntology.SetValue(xmlModel.OntologyName);
            raiz.Add(nodoOntology);

            XElement nodoDevEmail = new XElement("developerEmail");
            nodoDevEmail.SetValue(xmlModel.DevEmail);
            raiz.Add(nodoDevEmail);

            XElement nodoTokens = new XElement("token");
            raiz.Add(nodoTokens);
            XElement nodoKey = new XElement("tokenKey", xmlModel.TokenKey);
            nodoTokens.Add(nodoKey);
            XElement nodoSecret = new XElement("tokenSecret", xmlModel.TokenSecret);
            nodoTokens.Add(nodoSecret);

            XElement nodoConsumer = new XElement("consumer");
            raiz.Add(nodoConsumer);
            XElement nodoConsumerKey = new XElement("consumerKey", xmlModel.ConsumerKey);
            nodoConsumer.Add(nodoConsumerKey);
            XElement nodoConsumerSecret = new XElement("consumerSecret", xmlModel.ConsumerSecret);
            nodoConsumer.Add(nodoConsumerSecret);

            XElement nodoLog = new XElement("log");
            raiz.Add(nodoLog);
            XElement nodoLogPath = new XElement("logPath", xmlModel.LogPath);
            nodoLog.Add(nodoLogPath);
            XElement nodoLogFileName = new XElement("logFileName", xmlModel.LogFileName);
            nodoLog.Add(nodoLogFileName);
            XElement nodoLogLevel = new XElement("logLevel", xmlModel.LogLevel);
            nodoLog.Add(nodoLogLevel);

            //documentoXML.Save("RUTA");
            byte[] bytes = Encoding.UTF8.GetBytes(documentoXML.ToString());

            return bytes;
        }


        /// <summary>
        /// Muestra un formulario para crear una nueva aplicación.
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
        public ActionResult Crear()
        {
            GuardarLogAuditoria();
            return View("Editar", new ConsumerModel(0, "", "", "", "", ""));
        }

        /// <summary>
        /// Guarda en la BD los datos de la nueva aplicación creada.
        /// </summary>
        /// <param name="pConsumerId">ConsumerModel con los datos de la aplicación</param>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
        [HttpPost]
        public void Crear(ConsumerModel pModelo)
        {
            Boolean insertarUsuario = false;
            //Creamos los Consumers para la aplicación
            DateTime time = DateTime.Now;
            pModelo.Key = GenerarTokens();
            pModelo.Secret = GenerarTokens();

            //Guardar  en BD
            if (ModelState.IsValid)
            {
                try
                {
                    //Comprobamos si el usuario está ya en la lista de usuarios de Oauth
                    Usuario usuario = mOAuthCN.ObtenerUsuarioPorUsuarioID(mControladorBase.UsuarioActual.UsuarioID);
                    if (usuario == null)
                    {
                        insertarUsuario = true;
                    }

                    OAuthConsumer filaOAuthConsumer = new OAuthConsumer();
                    filaOAuthConsumer.ConsumerKey = pModelo.Key;
                    filaOAuthConsumer.ConsumerSecret = pModelo.Secret;
                    filaOAuthConsumer.VerificationCodeFormat = 1;
                    filaOAuthConsumer.VerificationCodeLength = 1;

                    mEntityContextOauth.OAuthConsumer.Add(filaOAuthConsumer);

                    ConsumerData filaConsumer = new ConsumerData();
                    filaConsumer.ConsumerId = filaOAuthConsumer.ConsumerId;
                    filaConsumer.Nombre = pModelo.Name;
                    filaConsumer.UrlOrigen = this.Request.Path.ToString();
                    filaConsumer.FechaAlta = time;

                    mEntityContextOauth.ConsumerData.Add(filaConsumer);

                    UsuarioConsumer filaUsuarioConsumer = new UsuarioConsumer();
                    filaUsuarioConsumer.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                    filaUsuarioConsumer.ConsumerId = filaOAuthConsumer.ConsumerId;
                    filaUsuarioConsumer.ProyectoID = ProyectoSeleccionado.Clave;

                    mEntityContextOauth.UsuarioConsumer.Add(filaUsuarioConsumer);

                    //Insertamos el usuario si no estay
                    if (insertarUsuario)
                    {
                        Usuario filaUsuario = new Usuario();
                        filaUsuario.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                        filaUsuario.Login = mControladorBase.UsuarioActual.Login;
                        mEntityContextOauth.Usuario.Add(filaUsuario);
                    }

                    mOAuthCN.ActualizarBD();

                    OAuthConsumer cosumer = mOAuthCN.ObtenerConsumerPorConsumerKey(pModelo.Key);
                    pModelo.ConsumerId = cosumer.ConsumerId;
                    Dispose();

                }
                catch (Exception ex)
                {
                    GuardarLogError(mLoggingService.DevolverCadenaError(ex, ""));
                    ModelState.AddModelError(string.Empty, "Error al guardar la aplicación, inténtelo de nuevo");
                    //return View("Editar", pModelo);
                }
                string urlBase = this.Request.Path.ToString().ToLower();
                urlBase = urlBase.Substring(0, urlBase.LastIndexOf("/crear"));
                //return Redirect(string.Format("{0}/informe/{1}", urlBase, pModelo.ConsumerId));
            }
            else
            {
               // return View("Editar", pModelo);
            }
        }

        /// <summary>
        /// Muestra un formulario para editar algunos datos de la aplicación.
        /// </summary>
        /// <param name="id">ConsumerId de la aplicación</param>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
        public ActionResult Editar(int id)
        {
            GuardarLogAuditoria();
            ConsumerModel modelo = mOAuthCN.ObtenerAplicacionPorConsumerId(id);
            OAuthToken tokens = mOAuthCN.ConsultarTokensPorUsuarioIDyConsumerId(mControladorBase.UsuarioActual.UsuarioID, id);
            if (tokens != null)
            {
                modelo.Token = tokens.Token;
                modelo.SecretToken = tokens.TokenSecret;
            }
            return View(modelo);
        }

        /// <summary>
        /// Guarda los datos de la aplicación editada.
        /// </summary>
        /// <param name="pModelo">ConsumerModel con los datos de la aplicación</param>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
        [HttpPost]
        public ActionResult Editar(ConsumerModel pModelo)
        {
            GuardarLogAuditoria();
            if (ModelState.IsValid)
            {
                //Guardar en BD
                try
                {
                    DataWrapperOAuth datawrapperOAuth = mOAuthCN.ObtenerConsumerPorConsumerId(pModelo.ConsumerId);

                    OAuthConsumer oAuthConsumer = datawrapperOAuth.OAuthConsumer.FirstOrDefault(oauthConsumer => oauthConsumer.ConsumerId.Equals(pModelo.ConsumerId));
                    if (oAuthConsumer != null)
                    {
                        ConsumerData consumerData = datawrapperOAuth.ConsumerData.FirstOrDefault(consumerData2 => consumerData2.ConsumerId.Equals(pModelo.ConsumerId));
                        consumerData.Nombre = pModelo.Name;

                        mOAuthCN.ActualizarBD();
                    }
                    else
                    {
                        throw new Exception($"El consumer ID {pModelo.ConsumerId} no existe en la base de datos");
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(mLoggingService.DevolverCadenaError(ex, ""));
                    ModelState.AddModelError(string.Empty, "Error al guardar los cambios, inténtelo de nuevo");
                    return View("Editar", pModelo);
                }

                string urlBase = this.Request.Path.ToString().ToLower();
                urlBase = urlBase.Substring(0, urlBase.LastIndexOf("/editar"));
                return Redirect(string.Format("{0}/informe/{1}", urlBase, pModelo.ConsumerId));
            }
            else
            {
                return View("Editar", pModelo);
            }
        }

        /// <summary>
        /// Muestra la información de la aplicación.
        /// </summary>
        /// <param name="id">ConsumerId de la aplicación</param>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
        public ActionResult Informe(int id)
        {
            ConsumerModel modelo = mOAuthCN.ObtenerAplicacionPorConsumerId(id);
            OAuthToken tokens = mOAuthCN.ConsultarTokensPorUsuarioIDyConsumerId(mControladorBase.UsuarioActual.UsuarioID, id);
            if (tokens != null)
            {
                modelo.Token = tokens.Token;
                modelo.SecretToken = tokens.TokenSecret;
            }
            return View(modelo);
        }

        /// <summary>
        /// Genera el token, el tokenSecret y los almacena en la BD.
        /// </summary>
        /// <param name="id">ConsumerId de la aplicación</param>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionDesarrolladoresPermitido" })]
        public void Tokens(int id)
        {
            ConsumerModel modelo = mOAuthCN.ObtenerAplicacionPorConsumerId(id);

            try
            {
                //Guardar en BD
                //oauthDS = new OAuthDS();
                //OAuthDS.OAuthTokenRow filaToken = oauthDS.OAuthToken.NewOAuthTokenRow();
                OAuthToken filaToken = new OAuthToken();

                filaToken.Token = GenerarTokens();
                filaToken.TokenSecret = GenerarTokens();
                filaToken.State = 2;
                filaToken.IssueDate = DateTime.Now;
                filaToken.ConsumerId = modelo.ConsumerId;
                filaToken.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                filaToken.Scope = null;
                filaToken.RequestTokenVerifier = null;
                filaToken.ConsumerVersion = "1.0.1";

                mEntityContextOauth.OAuthToken.Add(filaToken);
                mOAuthCN.ActualizarBD();
            }
            catch (Exception ex)
            {
                GuardarLogError(mLoggingService.DevolverCadenaError(ex, ""));
            }

            //return Redirect(this.Request.Path.ToString().Substring(0, this.Request.Path.ToString().LastIndexOf('/')));
        }


        /// <summary>
        /// Genera un nuevo token
        /// </summary>
        /// <returns>String con el token</returns>
        private string GenerarTokens()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());

            return token;
        }
    }
}
