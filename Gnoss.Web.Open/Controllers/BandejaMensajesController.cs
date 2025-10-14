using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.Organizador.Correo;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class BandejaMensajesController : ControllerBaseWeb
    {
        #region Miembros

        private MessageModel mMensaje = null;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        public BandejaMensajesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<BandejaMensajesController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mAvailableServices = availableServices;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Metodos

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EsPaginaEdicion = true;
            base.OnActionExecuting(filterContext);
        }

        protected override void CargarTituloPagina()
        {
            string url = Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString;
            if (Mensaje != null)
            {
                TituloPagina = Mensaje.Subject;
                if (Mensaje.Subject.Length > 100)
                {
                    TituloPagina = Mensaje.Subject.Substring(0, 100) + "...";
                }

                if (url.Contains("&responder&") || url.Contains("&responderATodos&") || url.Contains("&reenviar&"))
                {
                    if (url.Contains("&responder&"))
                    {
                        TituloPagina = UtilIdiomas.GetText("CORREO", "RESPONDERMENSAJESINTEXTOMENSAJE") + " - " + TituloPagina;
                    }
                    else if (url.Contains("&responderATodos&"))
                    {
                        TituloPagina = UtilIdiomas.GetText("CORREO", "REENVIARMENSAJEATODOS") + " - " + TituloPagina;
                    }
                    else if (url.Contains("&reenviar&"))
                    {
                        TituloPagina = UtilIdiomas.GetText("CORREO", "REENVIARMENSAJESINTEXTOMENSAJE") + " - " + TituloPagina;
                    }
                }
                else
                {
                    if (Mensaje.Deleted)
                    {
                        TituloPagina += " - " + UtilIdiomas.GetText("BANDEJAENTRADA", "MENSAJESELIMINADOS");
                    }
                    else if (Mensaje.Received)
                    {
                        TituloPagina += " - " + UtilIdiomas.GetText("BANDEJAENTRADA", "MENSAJESRECIBIDOS");
                    }
                    else if (Mensaje.Sent)
                    {
                        TituloPagina += " - " + UtilIdiomas.GetText("BANDEJAENTRADA", "MENSAJESENVIADOS");
                    }
                }
            }
            else
            {
                if (url.Contains("&nuevo&") || RequestParams("nuevo") != null)
                {
                    TituloPagina = UtilIdiomas.GetText("CORREO", "ESCRIBIRNUEVO");
                }
                else if (url.Contains("&eliminados&"))
                {
                    TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "MENSAJESELIMINADOS");
                }
                else if (url.Contains("&recibidos&"))
                {
                    TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "MENSAJESRECIBIDOS");
                }
                else if (url.Contains("&enviados&"))
                {
                    TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "MENSAJESENVIADOS");
                }
                else {
                    TituloPagina = UtilIdiomas.GetText("BANDEJAENTRADA", "BANDEJAENTRADA");
                }
            }

            base.CargarTituloPagina();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }
            //ViewBag.applicationServerKey = mConfigService.ObtenerVapidPublicKey();
            ControladorCorreo.ResetearContadorNuevosMensajes(IdentidadActual.PerfilID);
            if (IdentidadActual.IdentidadOrganizacion != null && EsIdentidadActualAdministradorOrganizacion)
            {
                ControladorCorreo.ResetearContadorNuevosMensajes(IdentidadActual.IdentidadOrganizacion.PerfilID);
            }

            string grafo = mControladorBase.UsuarioActual.UsuarioID.ToString();
            string parametrosAdicionales = null;
            Guid identidadBusqueda = mControladorBase.UsuarioActual.IdentidadID;

            if (RequestParams("organizacion") == "true")
            {
                grafo = IdentidadActual.OrganizacionID.Value.ToString();
                identidadBusqueda = IdentidadActual.IdentidadOrganizacion.Clave;
            }

            if (EsEcosistemaSinMetaProyecto && ProyectoVirtual != null)
            {
                parametrosAdicionales = "proyectoVirtualID=" + ProyectoVirtual.Clave + "|" + parametrosAdicionales;
            }

            string variableLeerMaster = "primeraCargaDeFacetas = false;";
            if (mControladorBase.UsuarioActual.UsarMasterParaLectura)
            {
                variableLeerMaster += " bool_usarMasterParaLectura = true;";
            }
            insertarScriptBuscador("divFac", "divRel", "divNumResultadosBusqueda", "panListadoFiltrosPulgarcito", "divRel", "divFiltros", grafo, "navegadorBusqueda", "", "", parametrosAdicionales, "", "", "", variableLeerMaster, identidadBusqueda, ProyectoSeleccionado.Clave, null, (short)TipoBusqueda.Mensajes, null, null);

            if (!string.IsNullOrEmpty(RequestParams("mensaje")))
            {
                /* Cargar ViewBag para indicar que se está en la vista de Mensaje */
                ViewBag.TituloTipoPagina = "mensaje";
                // Url actual del mensaje
                string urlMensaje = Request.Path + Request.QueryString;
                string urlVuelta = "";
                string tipoBandeja = "recibidos";
                // Obtener el mensaje visualizado
                Guid mensajeID = new Guid(RequestParams("mensaje"));
                MessageModel mensaje = ControladorProyectoMVC.ObtenerMensajePorID(mensajeID, "", IdentidadPagina);

                /* Controlar la bandeja de mensajes */
                if (mensaje.Deleted)
                {
                    tipoBandeja = "eliminados";
                }
                else if (!mensaje.Received)
                {
                    tipoBandeja = "enviados";
                }


                /* URL de vuelta a Mensajes */
                if (urlMensaje.Contains("&"))
                {
                    urlVuelta += "?" + urlMensaje.Substring(urlMensaje.IndexOf("&") + 1).Replace("|", "&");
                }
                else if (Request.Headers.ContainsKey("Referer") && Request.Headers["Referer"].ToString().Contains("&"))
                {
                    urlVuelta += Request.Headers["Referer"].ToString().Substring(Request.Headers["Referer"].ToString().IndexOf("&"));
                }
                else
                {
                    urlVuelta += "?" + tipoBandeja;
                }

                // Construcción de la URL para volver a bandeja original
                ViewBag.UrlVuelta = urlVuelta;

                if (Mensaje.Received && !Mensaje.Readed)
                {
                    MarcarCorreoLeido(Mensaje);
                }

                string url = "&" + Request.QueryString.Value.Replace("?", "");
                if (url.Contains("&responder") || url.Contains("&responderATodos") || url.Contains("&reenviar"))
                {
                    ControladorAmigos controlador = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory);
                    controlador.CargarAmigos(IdentidadActual, EsIdentidadActualAdministradorOrganizacion, mAvailableServices, pSoloComprobar: true);
                    return View("EdicionMensaje", Mensaje);
                }

                return View("Mensaje", Mensaje);
            }
            else if (("&" + Request.Query.ToString()).Contains("&nuevo&") || RequestParams("nuevo") != null)
            {
                ControladorAmigos controlador = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory);
                controlador.CargarAmigos(IdentidadActual, EsIdentidadActualAdministradorOrganizacion, mAvailableServices, pSoloComprobar: true);
                return View("EdicionMensaje");
            }

            PersonaCL personaCL = new PersonaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCL>(), mLoggerFactory);
            string vista = personaCL.ObtenerVistaMenuMensajes(mControladorBase.UsuarioActual.PersonaID);
            bool compactView = !string.IsNullOrEmpty(vista) && (vista.Equals("compact") || vista.Equals("compactada"));

            // Comprobar en qué bandeja de mensajes se encuentra el usuario, por defecto Recibidos
            ListadoMensajesViewModel listadoMensajes = new ListadoMensajesViewModel();
            // Asignar si la vista tiene que ser o no compacta según preferencias del usuario
            listadoMensajes.CompactView = compactView;
            // Asignar la bandeja en la que se encuentra el usuario            
            if (Request.QueryString.Value.Length == 0)
            {
                // Detectar que se está en bandeja de "Recibidos"
                listadoMensajes.TipoBandejaActual = TipoBandejaMensajes.Recibidos;
            }
            else if (Request.QueryString.Value.Contains("?enviados"))
            {
                // Detectar que se está en bandeja de "Enviados"
                listadoMensajes.TipoBandejaActual = TipoBandejaMensajes.Enviados;
            }
            else if (Request.QueryString.Value.Contains("?eliminados"))
            {
                listadoMensajes.TipoBandejaActual = TipoBandejaMensajes.Eliminados;
            }

            if (Request.Headers.ContainsKey("Accept") && Request.Headers["Accept"].Equals("application/json") && !Comunidad.EntornoEsPro && !Comunidad.EntornoEsPre)
            {
                CargadorResultados cargadorResultados = new CargadorResultados();
                string parametroadicional = "";
                cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();
                ResultadoModel listaResultados = cargadorResultados.CargarResultadosJSON(ProyectoAD.MyGnoss, mControladorBase.UsuarioActual.IdentidadID, mControladorBase.UsuarioActual.EsUsuarioInvitado, mControladorBase.DominoAplicacionConHTTP + this.Request.Path, mControladorBase.UsuarioActual.UsarMasterParaLectura, AdministradorQuiereVerTodasLasPersonas, VariableTipoBusqueda, grafo, parametroadicional, "", true, UtilIdiomas.LanguageCode, -1, "");
                //List<Correo> listaResultados = GestionCorreo.ListaCorreosEnviados;
                MemoryStream memStream = new MemoryStream();

                System.Text.Json.JsonSerializer.Serialize(memStream, listaResultados);

                string respuesta = string.Empty;

                memStream.Seek(0, SeekOrigin.Begin);

                using (MemoryStream output = new MemoryStream())
                using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(output, System.IO.Compression.CompressionMode.Compress))
                {
                    memStream.CopyTo(deflateStream);
                    deflateStream.Close();

                    respuesta = Convert.ToBase64String(output.ToArray());
                }
                respuesta = SerializeViewData(respuesta);
                return Content(respuesta);
            }
            insertarScriptBuscador("divFac", "divRel", "divNumResultadosBusqueda", "panListadoFiltrosPulgarcito", "divRel", "divFiltros", grafo, "navegadorBusqueda", "", "", parametrosAdicionales, "", "", "", variableLeerMaster, identidadBusqueda, ProyectoSeleccionado.Clave, null, (short)TipoBusqueda.Mensajes, null, null);

            return View("Listado", listadoMensajes);
        }
        private string SerializeViewData(string json)
        {
            ViewBag.ControladorProyectoMVC = null;
            UtilIdiomasSerializable aux = ViewBag.UtilIdiomas.GetUtilIdiomas();
            ViewBag.UtilIdiomas = aux;

            JsonSerializerSettings jsonSerializerSettingsVB = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full
            };
            Dictionary<string, object> dic = ViewData.Where(k => !k.Key.Equals("LoggingService")).ToDictionary(k => k.Key, v => v.Value);
            string jsonViewData = JsonConvert.SerializeObject(dic, jsonSerializerSettingsVB);

            return json + "{ComienzoJsonViewData}" + jsonViewData;
        }
        private void MarcarCorreoLeido(MessageModel pMessage)
        {
            //obtenemos el correo actual para marcarlo como parte de una conversacion
            CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);

            if (IdentidadActual.IdentidadOrganizacion != null && IdentidadActual.IdentidadOrganizacion.Clave != null)
            {
                GestionCorreo.CorreoDS.Merge(correoCN.ObtenerCorreoPorID(pMessage.Key, IdentidadActual.Clave, IdentidadActual.IdentidadOrganizacion.Clave));
            }
            else
            {
                GestionCorreo.CorreoDS.Merge(correoCN.ObtenerCorreoPorID(pMessage.Key, IdentidadActual.Clave, null));
            }

            correoCN.ActualizarCorreoLeido(GestionCorreo.CorreoDS, pMessage.Key, pMessage.ReceiverKey);
            correoCN.Dispose();


            FacetadoCN facetadoCN = new FacetadoCN("acidHome", UrlIntragnoss, mControladorBase.UsuarioActual.ProyectoID.ToString(), ReplicacionAD.COLA_REPLICACION_MASTER_HOME, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            facetadoCN.ModificarPendienteLeerALeido(Grafo.ToString(), pMessage.Key.ToString());
            facetadoCN.Dispose();

            if (pMessage.ReceiverKey != IdentidadActual.Clave && IdentidadActual.IdentidadOrganizacion != null && pMessage.ReceiverKey == IdentidadActual.IdentidadOrganizacion.Clave)
            {
                ControladorCorreo.AgregarNotificacionCorreoLeidoAPerfil(IdentidadActual.IdentidadOrganizacion.PerfilID);
            }
            else
            {
                ControladorCorreo.AgregarNotificacionCorreoLeidoAPerfil(IdentidadActual.PerfilID);
            }
        }

        [HttpPost]
        public ActionResult SelectView(string defaultView)
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            PersonaCL personaCL = new PersonaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCL>(), mLoggerFactory);
            personaCL.ModificarVistaMenuMensajes(mControladorBase.UsuarioActual.PersonaID, defaultView);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteMessages(string listMessages, string profileBox)
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            List<Guid> listaCorreosSeleccionados = new List<Guid>();

            if (listMessages != "")
            {
                char[] separador = { ',' };
                foreach (string correoSeleccionado in listMessages.Split(separador, StringSplitOptions.RemoveEmptyEntries))
                {
                    listaCorreosSeleccionados.Add(new Guid(correoSeleccionado));
                }
            }

            string js = "";
            //Agregamos la lectura desde la Master para el usuario, pra que lo vea como leído la siguiente lectura que haga.
            mControladorBase.UsuarioActual.UsarMasterParaLectura = true;
            js += "bool_usarMasterParaLectura = true;";

            if (profileBox == "eliminados")
            {
                js += EliminarDefinitivamenteCorreo(listaCorreosSeleccionados);
            }
            else
            {
                EliminarCorreo(listaCorreosSeleccionados, profileBox);
            }


            //if (function == "EliminarCorreos")
            //{
            //    return UtilAJAX.AgregarJavaScripADevolucionCallBack("", js + "FiltrarPorFacetas(ObtenerHash2());");
            //}
            //else
            //{
            string urlRedirect = "";

            if (!BandejaOrganizacion)
            {
                urlRedirect = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MENSAJES") + "?" + profileBox;
            }
            else
            {
                urlRedirect = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MENSAJESORG") + "?" + profileBox;
            }
            //}

            return GnossResultUrl(urlRedirect);
        }

        [HttpPost]
        public ActionResult SendMessage(string Subject, string Body, string Receivers, bool OrgIsSender, string UrlAction)
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return new RedirectResult(BaseURLIdioma + UrlPerfil + "home");
            }
            Guid? correoOrigenID = null;
            if ((UrlAction.Contains("responder")) || (UrlAction.Contains("responderATodos")) || (UrlAction.Contains("reenviar")))
            {
                string mensajeID = UrlAction.Substring(UrlAction.IndexOf('=') + 1);
                if (mensajeID.Contains("&"))
                {
                    mensajeID = mensajeID.Substring(0, mensajeID.IndexOf('&'));
                }
                //Creo un Guid a partir del mensajeID para pasarlo al guardar mensaje
                correoOrigenID = new Guid(mensajeID);
            }
            bool responder = false;
            if ((UrlAction.Contains("responder")) || (UrlAction.Contains("responderATodos")))
            {
                responder = true;
            }

            //Variables normales
            Body = UtilCadenas.LimpiarInyeccionCodigo(Body);
            string asunto = UtilCadenas.LimpiarInyeccionCodigo(Subject);
            string cuerpo = HttpUtility.UrlDecode(Body);

            bool EnvioCuentaOrg = OrgIsSender;
            string destinatarios = Receivers;

            string error = string.Empty;

            if (asunto.Length > 255)
            {
                error = UtilIdiomas.GetText("CORREO", "ERRORLONGITUDASUNTO");
            }
            else if (asunto == "")
            {
                asunto = "(Sin asunto)";
            }

            if (error == string.Empty)
            {
                Dictionary<Guid, bool> ListaDestinatarios = AceptarCorreo(destinatarios, EnvioCuentaOrg, responder, ref error);

                if (error == string.Empty)
                {
                    Guid autor = IdentidadActual.IdentidadMyGNOSS.Clave;

                    if (EnvioCuentaOrg)
                    {
                        autor = IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
                    }

                    Guid nuevoCorreo = GestionCorreo.AgregarCorreo(autor, ListaDestinatarios, asunto, cuerpo, this.BaseURLIdioma, ProyectoVirtual, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);

                    //si existe correo de origen, marcamos todos los mensajes como conversacion, ConversacionID=MensajeID
                    if (correoOrigenID.HasValue)
                    {
                        MarcarCorreosConversacion(correoOrigenID.Value, nuevoCorreo);
                    }


                    foreach (var notificacion in GestionCorreo.GestorNotificaciones.NotificacionDW.ListaNotificacionCorreoPersona.Where(item => !item.EnviadoRabbit))
                    {
                        notificacion.EnviadoRabbit = true;
                    }

                    Guardar(nuevoCorreo, autor, correoOrigenID, ListaDestinatarios);

                    GestionCorreo.ListaCorreos.Remove(nuevoCorreo);

                    List<Correo> listaCorreosRecibidos = new List<Correo>();
                    foreach (Correo correorec in GestionCorreo.ListaCorreosRecibidos)
                    {
                        listaCorreosRecibidos.Add(correorec);
                    }

                    foreach (Correo correorecibido in listaCorreosRecibidos)
                    {
                        if (correorecibido.Clave == nuevoCorreo)
                        {
                            GestionCorreo.ListaCorreosRecibidos.Remove(correorecibido);
                        }
                    }

                    List<Correo> ListaCorreosEnviados = new List<Correo>();
                    foreach (Correo correoen in GestionCorreo.ListaCorreosEnviados)
                    {
                        ListaCorreosEnviados.Add(correoen);
                    }

                    foreach (Correo correoenviado in ListaCorreosEnviados)
                    {
                        if (correoenviado.Clave == nuevoCorreo)
                        {
                            GestionCorreo.ListaCorreosEnviados.Remove(correoenviado);
                        }
                    }

                    return GnossResultUrl(BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "MENSAJES") + "?recibidos");

                }
            }

            return GnossResultERROR(error);
        }

        /// <summary>
        /// marca los correos de respuesta como pertenecientes a una conversacion
        /// </summary>
        /// <param name="correoOrigenID">identificador del mensaje original</param>
        /// <param name="nuevoCorreo">identificador del nuevo mensaje</param>
        private void MarcarCorreosConversacion(Guid correoOrigenID, Guid nuevoCorreo)
        {
            //obtenemos el correo actual para marcarlo como parte de una conversacion
            CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);

            if (IdentidadActual.IdentidadOrganizacion != null && IdentidadActual.IdentidadOrganizacion.Clave != null)
            {
                GestionCorreo.CorreoDS.Merge(correoCN.ObtenerCorreoPorID(correoOrigenID, IdentidadActual.Clave, IdentidadActual.IdentidadOrganizacion.Clave));
            }
            else
            {
                GestionCorreo.CorreoDS.Merge(correoCN.ObtenerCorreoPorID(correoOrigenID, IdentidadActual.Clave, null));
            }

            GestionCorreo.ResetearListasCorreos();

            if (GestionCorreo.ListaCorreos != null && GestionCorreo.ListaCorreos.ContainsKey(correoOrigenID))
            {
                Correo correoRecibido = GestionCorreo.ListaCorreos[correoOrigenID];

                string asuntoOrigen = correoRecibido.Asunto;
                string asuntoNuevo = GestionCorreo.ListaCorreos[nuevoCorreo].Asunto;
                string asuntoOrigenLimpio = "";
                string asuntoNuevoLimpio = "";

                if (correoOrigenID != null)
                {
                    //limpio los asuntos(RE: FWD:)
                    asuntoOrigenLimpio = UtilCadenas.LimpiarAsunto(asuntoOrigen);
                    asuntoNuevoLimpio = UtilCadenas.LimpiarAsunto(asuntoNuevo);

                    //comparo los asuntos limpios sin espacios
                    if (asuntoOrigenLimpio.Trim().Equals(asuntoNuevoLimpio.Trim()))
                    {
                        //asigno el mismo ConversacionID para todos los correos de la lista de recibidos relacionados con el mensaje original
                        foreach (Correo correo in GestionCorreo.ListaCorreosRecibidos)
                        {
                            if ((correo != correoRecibido) && (correo.FilaCorreo.RowState.Equals(DataRowState.Added)))
                            {
                                correo.FilaCorreo.ConversacionID = correoRecibido.FilaCorreo.ConversacionID;
                                //pongo un unico RE: y Fwd: en los correos recibidos
                                correo.FilaCorreo.Asunto = UnicoReFwd(asuntoNuevo);
                            }
                        }
                        //asigno el mismo ConversacionID para todos los correos de la lista de enviados relacionados con el mensaje original
                        //cuando es un reenvio la lista contiene dos correos
                        foreach (Correo correoEnviado in GestionCorreo.ListaCorreosEnviados)
                        {
                            if (correoEnviado.FilaCorreo.RowState.Equals(DataRowState.Added))
                            {
                                correoEnviado.FilaCorreo.ConversacionID = correoRecibido.FilaCorreo.ConversacionID;
                                //pongo un unico RE: y Fwd: en los correos recibidos
                                correoEnviado.FilaCorreo.Asunto = UnicoReFwd(asuntoNuevo);
                            }
                        }
                    }
                    //else
                    //{
                    //    //si el asunto ha cambiado, inicio una nueva conversacion
                    //    IniciarConversacion(nuevoCorreo);
                    //}
                }
            }
        }

        /// <summary>
        /// deja el asunto con una única respuesta o un único reenvío, dependiendo del asunto de entrada
        /// </summary>
        /// <param name="asuntoNuevo">cadena con el asunto del mensaje</param>
        /// <returns>devuelve una cadena con el asunto del mensaje conteniendo una única respuesta o un único reenvío</returns>
        private string UnicoReFwd(string asuntoNuevo)
        {
            string salida = asuntoNuevo;
            //si contiene ambos a la vez
            if (asuntoNuevo.Contains("RE: ") && (asuntoNuevo.Contains("Fwd: ")))
            {
                if (asuntoNuevo.IndexOf("RE: ") < asuntoNuevo.IndexOf("Fwd: "))
                {
                    asuntoNuevo = asuntoNuevo.Replace("RE: ", "");
                    asuntoNuevo = asuntoNuevo.Replace("Fwd: ", "");
                    salida = "RE: " + "Fwd: " + asuntoNuevo.Replace("RE: ", "");
                }
                else
                {
                    asuntoNuevo = asuntoNuevo.Replace("RE: ", "");
                    asuntoNuevo = asuntoNuevo.Replace("Fwd: ", "");
                    salida = "Fwd: " + "RE: " + asuntoNuevo;
                }
            }
            else
            {
                //solo contiene uno de los dos
                if (asuntoNuevo.Contains("RE: "))
                {
                    asuntoNuevo = asuntoNuevo.Replace("RE: ", "");
                    salida = "RE: " + asuntoNuevo;
                }
                if (asuntoNuevo.Contains("Fwd: "))
                {
                    asuntoNuevo = asuntoNuevo.Replace("Fwd: ", "");
                    salida = "Fwd: " + asuntoNuevo;
                }
            }

            return salida;
        }

        /// <summary>
        /// Guarda los datos de la página
        /// </summary>
        private void Guardar(Guid pNuevoCorreo, Guid pDestinatarioCorreo, Guid? pMensajeOrigenID, Dictionary<Guid, bool> pListaDestinatarios)
        {
            CorreoCN actualizarCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);
            actualizarCN.ActualizarCorreo(GestionCorreo.CorreoDS);

            string destinatarios = "";
            foreach (Guid destinatario in pListaDestinatarios.Keys)
            {
                if (pListaDestinatarios[destinatario])
                {
                    destinatarios += "|" + "g_" + destinatario.ToString();
                }
                else
                {
                    destinatarios += "|" + destinatario.ToString();
                }
            }
            if (destinatarios.StartsWith("|"))
            {
                destinatarios = destinatarios.Substring(1);
            }

            //Añadimos una línea al base para que la procese:
            ControladorDocumentacion.AgregarMensajeFacModeloBaseSimple(pNuevoCorreo, pDestinatarioCorreo, ProyectoSeleccionado.Clave, "base", destinatarios, pMensajeOrigenID, PrioridadBase.Alta, mAvailableServices);
            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
            notificacionCN.ActualizarNotificacion(mAvailableServices);
        }

        /// <summary>
        /// Acepta el envío del correo y devuelve la cadena del error en caso de haberlo
        /// </summary>
        /// <returns>Cadena de texto con el error si lo hay</returns>
        private Dictionary<Guid, bool> AceptarCorreo(string pDestinatarios, bool pEsCuentaOrg, bool pEsRespuesta, ref string pError)
        {

            Dictionary<Guid, bool> ListaDestinatarios = new Dictionary<Guid, bool>();

            CargarIdentidadesDeDestinatarios(pDestinatarios, pEsCuentaOrg);

            //this.txtAsunto.Text = asunto;
            List<string> listaDescartados = new List<string>();

            List<Guid> listaPreferencias = new List<Guid>();

            if (RequestParams("nombreCortoPerfil") == null && RequestParams("nombreCortoOrganizacion") == null)
            {
                if (ValidarDestinatarios(ref listaDescartados, listaPreferencias, pDestinatarios, pEsRespuesta, ref ListaDestinatarios))
                {
                    if (pDestinatarios == "")
                    {
                        pError = UtilIdiomas.GetText("CORREO", "SELECCIONARDESTINATARIO");
                    }
                }
                else
                {
                    pError = UtilIdiomas.GetText("CORREO", "ERRORENVIODESTINATARIOS");

                    foreach (string destinatario in listaDescartados)
                    {
                        pError += destinatario + "<br/>";
                    }
                }
            }
            else
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

                Guid destinatarioID = Guid.Empty;

                if (RequestParams("nombreCortoPerfil") != null)
                {
                    string nombreCortoOrg = "";
                    if (RequestParams("nombreCortoOrganizacion") != null)
                    {
                        nombreCortoOrg = RequestParams("nombreCortoOrganizacion");
                    }
                    destinatarioID = identidadCN.ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(RequestParams("nombreCortoPerfil"), ProyectoSeleccionado.Clave, nombreCortoOrg, false)[0];
                }
                else if (RequestParams("nombreCortoOrganizacion") != null)
                {
                    destinatarioID = identidadCN.ObtenerIdentidadIDDeOrganizacionEnProyecto(RequestParams("nombreCortoOrganizacion"), ProyectoSeleccionado.Clave);
                }
                identidadCN.Dispose();
            }

            return ListaDestinatarios;
        }

        private void CargarIdentidadesDeDestinatarios(string pDestinatarios, bool pRdbCuentaOrg)
        {
            pDestinatarios = pDestinatarios.Replace("/n", "").Replace(", ", ",").Replace(" , ", ",").Replace(" ,", ",");

            string[] arrayDestinatarios = pDestinatarios.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();


            //LoggingService.AgregarEntrada("CargaIdentidades");
            if (GestionCorreo.GestorIdentidades != null && GestionCorreo.GestorIdentidades.DataWrapperIdentidad != null)
            {
                dataWrapperIdentidad = GestionCorreo.GestorIdentidades.DataWrapperIdentidad;
            }
            if (arrayDestinatarios.Length > 0)
            {
                dataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesPorNombreYProyecto(arrayDestinatarios.Where(x => !x.Contains("|||")).Select(x => x.Trim()), ProyectoAD.MetaProyecto));

                if (arrayDestinatarios.Any(x => x.Contains("|||")))
                {
                    List<Guid> destinatarios = arrayDestinatarios.Where(x => x.Contains("|||")).Select(destinatario => destinatario.Substring(destinatario.IndexOf("|||") + 3).Trim()).ToList().ConvertAll(Guid.Parse);
                    dataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesPorID(destinatarios, true));
                }
            }

            dataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadPorID(IdentidadPagina.IdentidadMyGNOSS.Clave, true));
            identidadCN.Dispose();
            //LoggingService.AgregarEntrada("fin CargaIdentidades");
            List<Guid> listaIdentidadesCorreos = new List<Guid>();
            foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in dataWrapperIdentidad.ListaIdentidad)
            {
                if (!listaIdentidadesCorreos.Contains(filaIdentidad.IdentidadID))
                {
                    listaIdentidadesCorreos.Add(filaIdentidad.IdentidadID);
                }
            }
            GestionCorreo.GestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);


            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);
            DataWrapperAmigos amigosDW = new DataWrapperAmigos();
            DataWrapperIdentidad DataWrapperIdentidad2 = new DataWrapperIdentidad();
            if (!pRdbCuentaOrg)
            {
                amigosCL.ObtenerAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave, DataWrapperIdentidad2, null, null, amigosDW, false, false);

                GestionCorreo.GestorAmigos = new GestionAmigos(amigosDW, new GestionIdentidades(DataWrapperIdentidad2, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                amigosCL.ObtenerAmigos(IdentidadPagina.IdentidadMyGNOSS.Clave, DataWrapperIdentidad2, null, null, amigosDW, false, false);

                GestionCorreo.GestorAmigos = new GestionAmigos(amigosDW, new GestionIdentidades(DataWrapperIdentidad2, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                //Cargo la identidad de la organización porque es el autor del mensaje y si no no se manda email. 
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                GestionCorreo.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadDeOrganizacion(IdentidadActual.OrganizacionID.Value, ProyectoAD.MetaProyecto, true));
                identCN.Dispose();

                GestionCorreo.GestorIdentidades.RecargarHijos();
            }
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            var perfiles = GestionCorreo.GestorIdentidades.ListaPerfiles.Where(item => item.Value.PersonaID.HasValue);
            if (perfiles.Any())
            {
                GestionCorreo.GestorIdentidades.GestorPersonas = new Elementos.ServiciosGenerales.GestionPersonas(personaCN.ObtenerPersonasPorID(perfiles.Select(item => item.Value.PersonaID.Value).ToList()), mLoggingService, mEntityContext);
            }
        }

        /// <summary>
        /// Valida los destinatarios del correo
        /// </summary>
        /// <param name="pDestinatarios">Cadena de texto con los destinatarios del correo</param>
        /// <param name="pListaDescartados">Lista de destinatarios descartados</param>
        /// <param name="pListaPreferencias">Lista de preferencias</param>
        /// <returns>TRUE si no hay destinatarios descartados, FALSE en caso contrario</returns>
        private bool ValidarDestinatarios(ref List<string> pListaDescartados, List<Guid> pListaPreferencias, string pDestinatarios, bool pEsRespuesta, ref Dictionary<Guid, bool> pListaDestinatarios)
        {
            List<string> listaProf = new List<string>();
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
			Dictionary<string, string> idiomas = paramCL.ObtenerListaIdiomasDictionary();

            foreach (string claveIdioma in idiomas.Keys)
            {
                UtilIdiomas utilIdiomas = new UtilIdiomas(claveIdioma, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);
                //UtilIdiomas utilIdiomas = new UtilIdiomas(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.DirectorySeparatorChar + "languages", new string[0], claveIdioma, ParametrosGeneralesRow, ParametrosAplicacionDS);
                listaProf.Add(utilIdiomas.GetText("SOLICITUDESNUEVOSPROFESORES", "PROFESOR") + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " ");
                listaProf.Add(utilIdiomas.GetText("SOLICITUDESNUEVOSPROFESORES", "PROFESORA") + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " ");
            }

            Dictionary<Guid, bool> listaTemp = GestionCorreo.SepararDestinatarios(pDestinatarios, ref pListaDescartados, pListaPreferencias, listaProf, pEsRespuesta);
            foreach (Guid identidad in listaTemp.Keys)
            {
                if (!pListaDestinatarios.ContainsKey(identidad))
                {
                    pListaDestinatarios.Add(identidad, listaTemp[identidad]);
                }
            }


            List<Guid> listaTempAmigosGrupo = GestionCorreo.SepararDestinatariosDeGrupos(ref pListaDescartados);

            foreach (Guid identidadDeGrupo in listaTempAmigosGrupo)
            {
                if (!pListaDestinatarios.ContainsKey(identidadDeGrupo))
                {
                    pListaDestinatarios.Add(identidadDeGrupo, false);
                }
            }
            return pListaDescartados.Count == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaCorreosSeleccionados"></param>
        private string EliminarDefinitivamenteCorreo(List<Guid> pListaCorreosSeleccionados)
        {
            string javascriptDevolucion = "";
            int numSeleccionados = pListaCorreosSeleccionados.Count;

            CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);
            Guid? identidadOrg = null;

            if (IdentidadActual.IdentidadOrganizacion != null && EsIdentidadActualAdministradorOrganizacion)
            {
                identidadOrg = IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
            }

            CorreoDS correoDS = new CorreoDS();
            correoCN.ObtenerCorreoPorListaIDs(pListaCorreosSeleccionados, IdentidadActual.IdentidadMyGNOSS.Clave, identidadOrg, ref correoDS);

            GestionCorreo gestorCorreo = new GestionCorreo(correoDS, null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<GestionCorreo>(), mLoggerFactory);

            //Caché de mensajes
            FacetadoCL facCL = new FacetadoCL("acid", "bandeja", UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCL>(), mLoggerFactory);
            List<string> filasAInsertar = new List<string>();
            
            BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<BaseComunidadCN>(), mLoggerFactory);
            foreach (Correo correo in gestorCorreo.ListaCorreosEliminados)
            {
                correoCN.EliminarCorreoDefinitivamente(correo.Clave, correo.FilaCorreo.Autor, correo.FilaCorreo.Destinatario);

                if (correo.Recibido && !correo.Leido)
                {
                    if (BandejaOrganizacion)
                    {
                        ControladorCorreo.AgregarNotificacionCorreoLeidoAPerfil(IdentidadActual.IdentidadOrganizacion.PerfilID);
                    }
                    else
                    {
                        ControladorCorreo.AgregarNotificacionCorreoLeidoAPerfil(IdentidadActual.PerfilID);
                    }

                    javascriptDevolucion += "DisminuirContadorMensajeNoLeido(" + BandejaOrganizacion.ToString().ToLower() + ");";
                }

                //Clave de caché a borrar:
                string clavecache = ObtenerClaveCacheBandejaMensajes(facCL, "eliminados");

                FacetadoCN facetadoCN = null;
                try
                {
                    facetadoCN = new FacetadoCN("acidHome_Master", UrlIntragnoss, "", ReplicacionAD.COLA_REPLICACION_MASTER_HOME, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    facetadoCN.BorrarRecurso(Grafo.ToString(), correo.Clave, 0);
                }
                catch (Exception ex) { mLoggingService.GuardarLogError(ex, mlogger); }
                finally { if (facetadoCN != null) facetadoCN.Dispose(); }

                FacetadoCN facetadoCN2 = new FacetadoCN("acidHome", UrlIntragnoss, "", ReplicacionAD.COLA_REPLICACION_MASTER_HOME, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                facetadoCN2.BorrarRecurso(Grafo.ToString(), correo.Clave, 0);
                facetadoCN2.Dispose();

                facCL.InvalidarCache(clavecache);
                mControladorBase.UsuarioActual.UsarMasterParaLectura = true;

                //Enviamos una fila al servicio windows encargado del refresco de la caché para que muestre el mensaje que se ha recibido.
                if (identidadOrg.HasValue)
                {
                    filasAInsertar.Add(baseComunidadCN.PreprarFilaColaRefrescoCacheRabbitMQ(ProyectoAD.MetaProyecto, TiposEventosRefrescoCache.CambiosBandejaDeMensajes, TipoBusqueda.Mensajes, identidadOrg.Value.ToString()));
                }
                else
                {
                    filasAInsertar.Add(baseComunidadCN.PreprarFilaColaRefrescoCacheRabbitMQ(ProyectoAD.MetaProyecto, TiposEventosRefrescoCache.CambiosBandejaDeMensajes, TipoBusqueda.Mensajes, IdentidadActual.Clave.ToString()));
                }
            }

            baseComunidadCN.InsertarFilasColaRefrescoCacheEnRabbitMQ(filasAInsertar, TiposEventosRefrescoCache.CambiosBandejaDeMensajes);
            baseComunidadCN.Dispose();

            facCL.Dispose();
            correoCN.Dispose();

            return javascriptDevolucion;
        }

        /// <summary>
        /// Elimina un correo y realiza la recarga de la paginación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarCorreo(List<Guid> pListaCorreosSeleccionados, string pTipoBandeja)
        {
            int numSeleccionados = pListaCorreosSeleccionados.Count;

            CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);
            Guid? identidadOrg = null;

            if (IdentidadActual.IdentidadOrganizacion != null && EsIdentidadActualAdministradorOrganizacion)
            {
                identidadOrg = IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
            }

            CorreoDS correoDS = new CorreoDS();
            correoCN.ObtenerCorreoPorListaIDs(pListaCorreosSeleccionados, IdentidadActual.IdentidadMyGNOSS.Clave, identidadOrg, ref correoDS);
            GestionCorreo gestorCorreo = null;

            //Caché de mensajes
            FacetadoCL facCL = new FacetadoCL("acid", "bandeja", UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCL>(), mLoggerFactory);

            if (pTipoBandeja == "recibidos")
            {
                gestorCorreo = new GestionCorreo(correoDS, null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<GestionCorreo>(), mLoggerFactory);
                foreach (Correo correo in gestorCorreo.ListaCorreosRecibidos)
                {
                    EliminarCorreoYLimpiarCache(correoCN, correo, facCL, pTipoBandeja, identidadOrg);
                }
            }
            else if (pTipoBandeja == "enviados")
            {
                gestorCorreo = new GestionCorreo(correoDS, null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<GestionCorreo>(), mLoggerFactory);
                foreach (Correo correo in gestorCorreo.ListaCorreosEnviados)
                {
                    EliminarCorreoYLimpiarCache(correoCN, correo, facCL, pTipoBandeja, identidadOrg);
                }
            }

            facCL.Dispose();
            correoCN.Dispose();

        }

        private void EliminarCorreoYLimpiarCache(CorreoCN pCorreoCN, Correo pCorreo, FacetadoCL pFacCL, string pTipoBandeja, Guid? pIdentidadOrg)
        {
            pCorreoCN.EliminarCorreo(pCorreo.Clave, pCorreo.FilaCorreo.Autor, pCorreo.FilaCorreo.Destinatario);

            // Clave de caché a borrar:
            string clavecache = ObtenerClaveCacheBandejaMensajes(pFacCL, pTipoBandeja);

            FacetadoCN facetadoCN = null;
            try
            {
                facetadoCN = new FacetadoCN("acidHome_Master", UrlIntragnoss, "", ReplicacionAD.COLA_REPLICACION_MASTER_HOME, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                //Intentamos eliminarlo directamente de virtuosomaster
                facetadoCN.ModificarMensajeAEliminado(Grafo.ToString(), pCorreo.Clave.ToString());
            }
            catch (Exception ex) { mLoggingService.GuardarLogError(ex, mlogger); }
            finally { if (facetadoCN != null) facetadoCN.Dispose(); }

            FacetadoCN facetadoCN2 = new FacetadoCN("acidHome", UrlIntragnoss, "", ReplicacionAD.COLA_REPLICACION_MASTER_HOME, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            facetadoCN2.ModificarMensajeAEliminado(Grafo.ToString(), pCorreo.Clave.ToString());
            facetadoCN2.Dispose();

            pFacCL.InvalidarCache(clavecache);

            // Eliminamos la caché de la bandeja de eliminados
            clavecache = ObtenerClaveCacheBandejaMensajes(pFacCL, "eliminados");
            pFacCL.InvalidarCache(clavecache);

            mControladorBase.UsuarioActual.UsarMasterParaLectura = true;

            //Enviamos una fila al servicio windows encargado del refresco de la caché para que muestre el mensaje que se ha recibido.
            BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<BaseComunidadCN>(), mLoggerFactory);
            if (pIdentidadOrg.HasValue && pCorreo.FilaCorreo.Destinatario.Equals(pIdentidadOrg.Value))
            {
                try
                {
                    baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoAD.MetaProyecto, TiposEventosRefrescoCache.CambiosBandejaDeMensajes, TipoBusqueda.Mensajes, pIdentidadOrg.Value.ToString(), mAvailableServices);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                    baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoAD.MetaProyecto, TiposEventosRefrescoCache.CambiosBandejaDeMensajes, TipoBusqueda.Mensajes, pIdentidadOrg.Value.ToString());
                }

            }
            else
            {
                try
                {
                    baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoAD.MetaProyecto, TiposEventosRefrescoCache.CambiosBandejaDeMensajes, TipoBusqueda.Mensajes, IdentidadActual.Clave.ToString(), mAvailableServices);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                    baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoAD.MetaProyecto, TiposEventosRefrescoCache.CambiosBandejaDeMensajes, TipoBusqueda.Mensajes, IdentidadActual.Clave.ToString());
                }

            }
            baseComunidadCN.Dispose();
        }

        private string ObtenerClaveCacheBandejaMensajes(FacetadoCL pFacCL, string pTipoBandeja)
        {
            // gnoss.com_facetado_facetado_11111111-1111-1111-1111-111111111111_mensaje_mvc_6c57d6cc-b750-4022-966f-f1d48d663143_1_pTipoBandeja

            string clavecache = string.Empty;
            clavecache = pFacCL.ObtenerKeyBusquedaFacetadoMVC(ProyectoAD.MetaProyecto, FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes), false);
            clavecache += "_1_" + pTipoBandeja;

            return clavecache;
        }

        private bool BandejaOrganizacion
        {
            get
            {
                return RequestParams("organizacion") == "true";
            }
        }

        private Guid Grafo
        {
            get
            {
                Guid mGrafo = mControladorBase.UsuarioActual.UsuarioID;
                if (BandejaOrganizacion)
                {
                    mGrafo = IdentidadActual.OrganizacionID.Value;
                }
                return mGrafo;
            }
        }


        private MessageModel ObtenerMensaje()
        {
            Guid mensajeID = new Guid(RequestParams("mensaje"));

            MessageModel mensaje = ControladorProyectoMVC.ObtenerMensajePorID(mensajeID, "", IdentidadPagina);

            CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);
            Guid? identidadOrganizacion = null;
            if (IdentidadActual.IdentidadOrganizacion != null)
            {
                identidadOrganizacion = IdentidadActual.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
            }

            TiposListadoCorreo tipoListado = TiposListadoCorreo.Recibido;
            if (mensaje.Deleted)
            {
                tipoListado = TiposListadoCorreo.Eliminado;
            }
            else if (!mensaje.Received)
            {
                tipoListado = TiposListadoCorreo.Enviado;
            }

            mensaje.PreviousMessageKey = correoCN.ObtenerCorreoIDAnteriorDeCorreoPorID(mensajeID, IdentidadActual.Clave, identidadOrganizacion, tipoListado);
            mensaje.NextMessageKey = correoCN.ObtenerCorreoIDSiguienteDeCorreoPorID(mensajeID, IdentidadActual.Clave, identidadOrganizacion, tipoListado);

            correoCN.Dispose();

            return mensaje;
        }

        private GestionCorreo mGestionCorreo;


        /// <summary>
        /// Devuelve la identidad actual de el usuario o la de la organización en caso de serlo
        /// </summary>
        private Identidad IdentidadPagina
        {
            get
            {
                if (BandejaOrganizacion && EsIdentidadActualAdministradorOrganizacion)
                {
                    return IdentidadActual.IdentidadOrganizacion;
                }
                else
                {
                    return IdentidadActual;
                }
            }
        }

        /// <summary>
        /// Obtiene el gestor de correo utilizado en la pantalla
        /// </summary>
        private GestionCorreo GestionCorreo
        {
            get
            {
                if (mGestionCorreo == null || mGestionCorreo.IdentidadActual != IdentidadPagina)
                {
                    mGestionCorreo = new GestionCorreo(new CorreoDS(), null, null, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<GestionCorreo>(), mLoggerFactory);
                    mGestionCorreo.IdentidadActual = IdentidadPagina;
                }
                return mGestionCorreo;
            }
        }

        #endregion

        #region Propiedades

        private MessageModel Mensaje
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("mensaje")))
                {
                    if (mMensaje == null)
                    {
                        mMensaje = ObtenerMensaje();
                    }
                    return mMensaje;
                }
                return null;
            }
        }

        #endregion
    }
}
