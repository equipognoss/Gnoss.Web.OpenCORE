using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class CMSPaginaController : ControllerPestanyaBase
    {
        #region Miembros

        /// <summary>
        /// Ubicación del CMS
        /// </summary>
        public short? mTipoUbicacionCMSPaginaActual = null;

        /// <summary>
        /// Componente del CMS
        /// </summary>
        public Guid? mComponenteCMSPaginaActual = null;

        /// <summary>
        /// True = Debe refrescar el componente; False solo pintarlo;
        /// </summary>
        private bool mRefrescar;

        private ControladorCMS mControladorCMS;

        #endregion

        public CMSPaginaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TipoComponenteCMS? tipoComponenteCMSActual = null;

            mControladorCMS = new ControladorCMS(this, ComponenteCMSPaginaActual, TipoUbicacionCMSPaginaActual, null, mHttpContextAccessor, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);

            if (ComponenteCMSPaginaActual.HasValue && mControladorCMS.GestorCMSActual.ListaComponentes.ContainsKey(ComponenteCMSPaginaActual.Value))
            {
                tipoComponenteCMSActual = mControladorCMS.GestorCMSActual.ListaComponentes[ComponenteCMSPaginaActual.Value].TipoComponenteCMS;
            }

            if (TipoUbicacionCMSPaginaActual.HasValue || tipoComponenteCMSActual.HasValue || ComponenteCMSPaginaActual.HasValue)
            {
                base.OnActionExecuting(filterContext, true);
            }

            if (filterContext.Result != null)
            {
                return;
            }

            if (ProyectoSeleccionado.EsAdministradorUsuario(IdentidadActual.Persona.UsuarioID) && RequestParams("preView") != null && RequestParams("preView") == "true")
            {
                mControladorCMS.VistaPrevia = true;
            }

            mControladorCMS.Comunidad = Comunidad;
        }

        [TypeFilter(typeof(AccesoPestanyaAttribute))]
        public ActionResult Index()
        {
            mLoggingService.AgregarEntrada("Entra en CMSpagina");
            if (ComponenteCMSPaginaActual.HasValue)
            {
                mLoggingService.AgregarEntrada("Tiene valor componenteCMSPaginaActual");
                if (!mControladorCMS.TieneUsuarioAccesoAComponente(mControladorBase.UsuarioActual.EsIdentidadInvitada))
                {
                    return new EmptyResult();
                }
                // comunidad publica o comunidad privada y soy miembro
                bool tieneacceso = ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Publico) || ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Restringido) || !mControladorBase.UsuarioActual.EsIdentidadInvitada;
                mLoggingService.AgregarEntrada("Tiene Acceso" + tieneacceso.ToString());
                if (!tieneacceso)
                {
                    mLoggingService.AgregarEntrada("Entra por la parte que no tiene acceso");
                    bool rangoAdmitido = false;
                    string IPRecargarComponentes = "";
                    string ipPeticion = mHttpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
                    if (Request.Headers != null && Request.Headers.ContainsKey("X-Forwarded-For"))
                    {
                        ipPeticion = Request.Headers["X-Forwarded-For"];
                    }
                    List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.IPRecargarComponentes.ToString())).ToList();
                    if (busqueda.Count > 0)
                    {
                        IPRecargarComponentes = busqueda.First().Valor;
                    }
                    rangoAdmitido = ipPeticion == "127.0.0.1" || (!string.IsNullOrEmpty(IPRecargarComponentes) && !string.IsNullOrEmpty(ipPeticion) && ipPeticion.StartsWith(IPRecargarComponentes));

                    mLoggingService.AgregarEntrada("Rango admitido: " + rangoAdmitido.ToString());
                    if (!rangoAdmitido)
                    {
                        return new EmptyResult();
                    }
                }

                #region Componente CMS
                /*Parámetros:
                     * componenteID(obligatorio): ID del componente
                     * idioma: idioma en el que se pide (si no se pide se hace en todos los idiomas del ecosistema)
                     * pintar: indica si se pinta o no (si no se especifica, false y refrescar=true. Si no hay idioma pintar=false)
                     * refrescar: indica si se refresca (si no se especifica, false, pero si no se especifica pintar refrescar=true)                 * 
                     */
                //Single viene ComponentName se pinta tal cual en el idioma de la petición

                string idiomaPedido = RequestParams("idioma");
                //Hay que pintar el HTML (bool pintar y string idioma)
                bool pintar = false;
                bool.TryParse(RequestParams("pintar"), out pintar);

                Dictionary<string, string> listaIdiomas = mConfigService.ObtenerListaIdiomasDictionary();
                if (string.IsNullOrEmpty(idiomaPedido) || !listaIdiomas.ContainsKey(idiomaPedido))
                {
                    //Si no se esoecifica idioma no se pinta
                    pintar = false;
                }

                //Hay que generar el HTML (bool refrescar)
                bool.TryParse(RequestParams("refrescar"), out mRefrescar);
                if (!pintar)
                {
                    mRefrescar = true;
                }

                string componentName = RequestParams("ComponentName");
                if (!string.IsNullOrEmpty(componentName))
                {
                    idiomaPedido = UtilIdiomas.LanguageCode;
                    pintar = true;
                    mRefrescar = false;
                }

                try
                {
                    if(idiomaPedido != null)
                    {
                        mLoggingService.AgregarEntrada("Va a por la ficha del componente con los parametros pinta: " + pintar.ToString() + ", mRefrescar: " + mRefrescar.ToString() + " y idiomaPedido: " + idiomaPedido.ToString());
                    }
                    else
                    {
                        mLoggingService.AgregarEntrada("Va a por la ficha del componente con los parametros pinta: " + pintar.ToString() + ", mRefrescar: " + mRefrescar.ToString() + " y idiomaPedido: " + UtilIdiomas.LanguageCode);
                    }

                    CMSComponent fichaComponente = mControladorCMS.CargarComponente();
                    mLoggingService.AgregarEntrada("Comprueba que la ficha del componente es distinto de nulo y que pintar es true: " + pintar.ToString());
                    if (fichaComponente != null && mControladorCMS.PintarComponente)
                    {
                        mLoggingService.AgregarEntrada("Pinta el componente y es distinto de nulo la ficha. Devuelve la ficha del componente");
                        return PintarComponenteCMS(fichaComponente);
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError("Error producido en el componente con ID='" + ComponenteCMSPaginaActual + "' en la comunidad " + ProyectoSeleccionado.Nombre + " \n " + mLoggingService.DevolverCadenaError(ex, Version));
                    return Content("");
                }
            }
            else if (TipoUbicacionCMSPaginaActual.HasValue)
            {
                mLoggingService.AgregarEntrada("Tiene valor TipoUbicacionCMSPaginaActual");
                List<CMSBlock> listaBloques = mControladorCMS.CargarBloquesPaginaCMS();
                if (listaBloques != null)
                {
                    mLoggingService.AgregarEntrada("Devuelve la lista de bloques");
                    return View(listaBloques);
                }
            }

            return new EmptyResult();
        }

        #region Propiedades

        /// <summary>
        /// Ubicación del CMS
        /// </summary>
        public short? TipoUbicacionCMSPaginaActual
        {
            get
            {
                if (!mTipoUbicacionCMSPaginaActual.HasValue)
                {
                    if (RequestParams("PaginaCMS") != null)
                    {
                        mTipoUbicacionCMSPaginaActual = short.Parse(RequestParams("PaginaCMS"));
                    }
                    else if (!mTipoUbicacionCMSPaginaActual.HasValue && ProyectoPestanyaActual != null)
                    {
                        mTipoUbicacionCMSPaginaActual = ProyectoPestanyaActual.FilaProyectoPestanyaCMS.Ubicacion;
                    }
                }
                return mTipoUbicacionCMSPaginaActual;
            }
        }

        /// <summary>
        /// Componente del CMS
        /// </summary>
        public Guid? ComponenteCMSPaginaActual
        {
            get
            {
                if (!mComponenteCMSPaginaActual.HasValue)
                {
                    Guid eventoID;
                    if (!string.IsNullOrEmpty(RequestParams("componenteID")))
                    {
                        mComponenteCMSPaginaActual = new Guid(RequestParams("componenteID"));
                    }
                    else if (!string.IsNullOrEmpty(RequestParams("ComponentKey")))
                    {
                        mComponenteCMSPaginaActual = new Guid(RequestParams("ComponentKey"));
                    }
                    else if (!string.IsNullOrEmpty(RequestParams("ComponentName")))
                    {
                        CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                        Guid proyectoID = ProyectoSeleccionado.Clave;
                        
                        mComponenteCMSPaginaActual = cmsCN.ObtenerIDComponentePorNombreEnProyecto(RequestParams("ComponentName"), proyectoID);

                        if (!mComponenteCMSPaginaActual.HasValue || mComponenteCMSPaginaActual.Value.Equals(Guid.Empty))
                        {
                            if (ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.HasValue)
                            {
                                proyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoSuperiorID.Value;
                                mComponenteCMSPaginaActual = cmsCN.ObtenerIDComponentePorNombreEnProyecto(RequestParams("ComponentName"), proyectoID);
                            }
                            
                        }
                        
                        cmsCN.Dispose();
                    }
                    else if (!string.IsNullOrEmpty(RequestParams("eventoID")) && Guid.TryParse(RequestParams("eventoID"), out eventoID))
                    {
                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerEventoProyectoPorEventoID(eventoID);
                        List<ProyectoEvento> filasEvento = dataWrapperProyecto.ListaProyectoEvento.Where(proy=>proy.EventoID.Equals(eventoID)).ToList();

                        if (filasEvento.Count > 0)
                        {
                            ProyectoEvento filaEvento = filasEvento.First();
                            mComponenteCMSPaginaActual = filaEvento.ComponenteID;
                        }

                        proyCN.Dispose();
                    }
                }
                return mComponenteCMSPaginaActual;
            }
        }

        #endregion

        /// <summary>
        /// Si el proyecto es público o privado pero con registro abierto, las páginas con registro son accesibles para el usuario desconectado
        /// </summary>
        public override bool PaginaVisibleEnPrivada
        {
            get
            {
                /*
                bool pestanyaVisibleEnPrivada = base.PaginaVisibleEnPrivada;
                bool rangoAdmitido = false;
                string IPRecargarComponentes = "";
                string ipPeticion = Request.UserHostAddress;
                if (!pestanyaVisibleEnPrivada && ComponenteCMSPaginaActual.HasValue)
                {    
                    if (Request.Headers != null && Request.Headers["X-Forwarded-For"] != null)
                    {
                        ipPeticion = Request.Headers["X-Forwarded-For"];
                    }

                    if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.IPRecargarComponentes.ToString() + "'").Length > 0)
                    {
                        IPRecargarComponentes = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.IPRecargarComponentes.ToString() + "'")[0]["Valor"].ToString();
                    }

                    rangoAdmitido = ipPeticion == "127.0.0.1" || (!string.IsNullOrEmpty(IPRecargarComponentes) && ipPeticion.StartsWith(IPRecargarComponentes));
                }

                if (!rangoAdmitido)
                {
                    //GuardarLogError(string.Format("Petición desde IP no permitida a recargar componente. IP: {0}, UserHostAdress: {1}, IPs permitidas, las que comiencen por: {2}", ipPeticion, Request.UserHostAddress, IPRecargarComponentes));
                }

                return pestanyaVisibleEnPrivada || rangoAdmitido;*/

                return base.PaginaVisibleEnPrivada || ComponenteCMSPaginaActual.HasValue;
            }
        }

        public ActionResult LoadMoreActivity(int NumPeticion, Guid ComponentKey)
        {
            #region Actividad reciente
            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerComponentePorID(ComponentKey,ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
            cmsCN.Dispose();

            CMSComponenteActividadReciente componente = (CMSComponenteActividadReciente)(gestorCMS.ListaComponentes[ComponentKey]);

            CMSComponent fichaComponenteActividadReciente = mControladorCMS.ObtenerFichaActividadReciente(componente, UtilIdiomas.LanguageCode, NumPeticion);
            mControladorCMS.ObtenerNombresVistas(componente, ref fichaComponenteActividadReciente);
            string vista = fichaComponenteActividadReciente.ViewName;
            return PartialView(vista, fichaComponenteActividadReciente);
            #endregion
        }

        public ActionResult SendForm(Guid ComponentKey)
        {
            #region Enviar correo

            string mensaje = "";
            string mensajeError = "";

            CMSComponenteEnvioCorreo componente = (CMSComponenteEnvioCorreo)(mControladorCMS.GestorCMSActual.ListaComponentes[ComponentKey]);
            foreach (short orden in componente.ListaCamposEnvioCorreo.Keys)
            {
                string valorCampo = Uri.UnescapeDataString(RequestParams(ComponentKey.ToString() + "_" + orden));
                if (bool.Parse(componente.ListaCamposEnvioCorreo[orden][TipoPropiedadEnvioCorreo.Obligatorio]) && string.IsNullOrEmpty(valorCampo))
                {
                    mensajeError += UtilIdiomas.GetText("COMADMINCMS", "ERRORCAMPOVACIO", UtilCadenas.ObtenerTextoDeIdioma(componente.ListaCamposEnvioCorreo[orden][TipoPropiedadEnvioCorreo.Nombre], UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto));
                }
                else
                {
                    mensaje += "<p>" + UtilCadenas.ObtenerTextoDeIdioma(componente.ListaCamposEnvioCorreo[orden][TipoPropiedadEnvioCorreo.Nombre], UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto) + ": " + valorCampo + "</p>";
                }
            }

            if (string.IsNullOrEmpty(mensajeError))
            {
                List<string> listaCorreos = new List<string>();
                listaCorreos.Add(componente.DestinatarioCorreo);

                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorNotificaciones.AgregarNotificacionGenerica(listaCorreos, mensaje, ProyectoSeleccionado.Nombre + "-" + componente.Titulo, UtilIdiomas.LanguageCode, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                notificacionCN.ActualizarNotificacion();
                notificacionCN.Dispose();


                string mensajeOK = UtilCadenas.ObtenerTextoDeIdioma(componente.TextoMensajeOK, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

                if (!string.IsNullOrEmpty(mensajeOK))
                {
                    mensajeOK = "<p>" + mensajeOK + "</p>";
                }
                else
                {
                    mensajeOK = "<p>" + UtilIdiomas.GetText("COMADMINCMS", "CORREOENVIADOOK") + "</p>";
                }

                return GnossResultOK(mensajeOK);
            }
            else
            {
                return GnossResultERROR(mensajeError);
            }
            #endregion
        }

        public ActionResult SearcherCMS(Guid ComponentKey, string filtro, int paginaBusqueda=1)
        {
            CMSComponente componente = mControladorCMS.GestorCMSActual.ListaComponentes[ComponentKey];

            if (componente is CMSComponenteBuscador)
            {
                #region Buscador CMS
                CMSComponenteBuscador componenteBuscador = (CMSComponenteBuscador)componente;

                CMSComponent fichaComponenteBuscador = mControladorCMS.ObtenerFichaBuscador(componenteBuscador, filtro, paginaBusqueda, UtilIdiomas.LanguageCode);
                mControladorCMS.ObtenerNombresVistas(componente, ref fichaComponenteBuscador);
                string vista = fichaComponenteBuscador.ViewName;
                return PartialView(vista, fichaComponenteBuscador);
                #endregion
            }
            else if (componente is CMSComponenteBuscadorSPARQL)
            {
                #region Buscador CMS SPARQL
                CMSComponenteBuscadorSPARQL componenteBuscadorSPARQL = (CMSComponenteBuscadorSPARQL)componente;

                CMSComponent fichaComponenteBuscadorSPARQL = mControladorCMS.ObtenerFichaBuscadorSPARQL(componenteBuscadorSPARQL, UtilIdiomas.LanguageCode, paginaBusqueda, filtro);
                mControladorCMS.ObtenerNombresVistas(componente, ref fichaComponenteBuscadorSPARQL);
                string vista = fichaComponenteBuscadorSPARQL.ViewName;

                return PartialView(vista, fichaComponenteBuscadorSPARQL);
                #endregion
            }else if (componente is CMSComponenteFaceta)
            {
                #region Buscador CMS
                CMSComponenteFaceta componenteFaceta = (CMSComponenteFaceta)componente;

                CMSComponent fichaComponenteFaceta = mControladorCMS.ObtenerFichaFaceta(componenteFaceta, filtro, UtilIdiomas.LanguageCode);
                mControladorCMS.ObtenerNombresVistas(componente, ref fichaComponenteFaceta);
                string vista = fichaComponenteFaceta.ViewName;
                return PartialView(vista, fichaComponenteFaceta);
                #endregion
            }
            return null;
        }
    }
}
#endregion