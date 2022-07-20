using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.CMS;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
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
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class HomeController : ControllerBaseWeb
    {

        public HomeController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        protected override void CargarTituloPagina()
        {
            TituloPagina = UtilIdiomas.GetText("COMMON", "HOME");

            base.CargarTituloPagina();
        }

        public ActionResult Index()
        {
            if (EsEcosistemaSinMetaProyecto)
            {
                Response.Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto));
            }

            if (ParametrosGeneralesRow.CMSDisponible)
            {
                bool tieneHomeCMS = Index_HomeCMS();
                if (tieneHomeCMS)
                {
                    return new EmptyResult();
                }
            }

            if (mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                return View("HomeDesconectado");
            }
            else
            {
                if (!string.IsNullOrEmpty(RequestParams("callback")))
                {
                    if (RequestParams("callback").ToLower() == "ActividadReciente|MostrarMas".ToLower())
                    {
                        int numPeticion = int.Parse(RequestParams("numPeticion"));

                        ActividadReciente actividadRecienteController = new ActividadReciente(mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
                        return PartialView("ControlesMVC/_ActividadReciente", actividadRecienteController.ObtenerActividadReciente(numPeticion, 10, TipoActividadReciente.HomeUsuario, null, false));
                    }
                    return new EmptyResult();
                }
                HomeConectedViewModel paginaModel = new HomeConectedViewModel();

                ActividadReciente actividadReciente = new ActividadReciente(mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication);
                paginaModel.RecentActivity = actividadReciente.ObtenerActividadReciente(1, 10, TipoActividadReciente.HomeUsuario, null, false);

                return View("HomeConectado", paginaModel);
            }
        }

        private bool Index_HomeCMS()
        {
            CMSCL cmsCL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionCMS gestorCMS = new GestionCMS(cmsCL.ObtenerConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
            cmsCL.Dispose();

            bool mostrarCMS = false;

            TipoUbicacionCMS tipoUbicacion = TipoUbicacionCMS.HomeProyecto;


            tipoUbicacion = TipoUbicacionCMS.HomeProyecto;
            if (gestorCMS.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave) && gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave].ContainsKey((short)tipoUbicacion))
            {
                mostrarCMS = gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)tipoUbicacion].Activa;
            }

            if (!mostrarCMS)
            {
                tipoUbicacion = TipoUbicacionCMS.HomeProyectoNoMiembro;
                if (!mControladorBase.UsuarioActual.EsIdentidadInvitada)
                {
                    tipoUbicacion = TipoUbicacionCMS.HomeProyectoMiembro;
                }
                if (gestorCMS.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave) && gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave].ContainsKey((short)tipoUbicacion))
                {
                    mostrarCMS = gestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)tipoUbicacion].Activa;
                }
            }

            if (mostrarCMS)
            {
                string urlTransfer = "/CMSPagina?PaginaCMS=" + (short)tipoUbicacion;// +"&PestanyaID=" + pestanyaID;
                if (!UtilIdiomas.LanguageCode.Equals("es"))
                {
                    urlTransfer = "/" + UtilIdiomas.LanguageCode + urlTransfer;
                }

                Response.Redirect(urlTransfer);

                return true;
            }

            return false;
        }

        public ActionResult SendGnossInvitation(string ListEmail)
        {
            string[] delimiter = { "," };
            string[] correos = ListEmail.Replace(';', ',').Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            bool errores = false;

            //Devuelves error
            if (correos.Length > 0)
            {
                string[] correosNoEnviados = InvitarEmail(correos);
                if (correosNoEnviados.Length > 0)
                {
                    //Ha habido errores....
                    string correosNoEnviadosComas = "";
                    foreach (string correo in correosNoEnviados)
                    {
                        correosNoEnviadosComas += ", " + correo;
                    }

                    if (correosNoEnviadosComas.Length > 0)
                    {
                        correosNoEnviadosComas = correosNoEnviadosComas.Substring(2);
                    }

                    errores = true;
                }
                else
                {
                    //Todo OK! =D
                    errores = false;

                }

            }
            else
            {
                //Pintar el divKo
                errores = true;
            }

            if (errores)
            {
                return GnossResultERROR();
            }
            else
            {
                return GnossResultOK();
            }
        }

        /// <summary>
        /// Establece que el usuario actual va a leer de la base de datos Master durante el siguiente minuto. 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetMaster()
        {
            if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
            {
                mControladorBase.UsuarioActual.UsarMasterParaLectura = true;
            }
            else
            {
                Response.StatusCode = 403;
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Método que envia correos a usuarios de Gnoss y externos
        /// </summary>
        /// <param name="pCorreos">Lista de correos a enviar</param>
        /// <returns>Lista con los correos que no se han enviado</returns>
        public string[] InvitarEmail(string[] pCorreos)
        {
            //Marcar los correos con errores para mostrarselos al usuario despúes y que pueda renviar el correo.
            //Si el correo ya pertenece a un usuario de Gnoss enviarle la invitación a gnoss no al correo...

            NotificacionCN notCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
            GestionNotificaciones GestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<string> listaEmailsMalos = new List<string>();

            bool invitacionesEnviadas = false;

            if (pCorreos.Length > 0)
            {
                //Separar por comas y comprobar los correos 1 a 1
                foreach (string pCorreo in pCorreos)
                {
                    if (UtilCadenas.ValidarEmail(pCorreo))
                    {
                        DataWrapperIdentidad identidadDW = null;
                        identidadDW = idenCN.ObtenerIdentidadesVisiblesPorEmail(pCorreo, mControladorBase.UsuarioActual.PersonaID);
                        GestionIdentidades gestorIden = new GestionIdentidades(identidadDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                        try
                        {
                            string nombreProyecto = mControladorBase.ObtenerFilaProyecto(mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.ProyectoID).Nombre;
                            AD.EntityModel.Models.ProyectoDS.Proyecto filaProy;
                            filaProy = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerProyectoPorID(mControladorBase.UsuarioActual.OrganizacionID).ListaProyecto.First();

                            string enviadasA = "";
                            string separador = "";

                            if (gestorIden.ListaIdentidades.Count > 0)
                            {
                                //Usuario GNOSS!
                                #region Usuario GNOSS

                                foreach (Guid destinatarioID in gestorIden.ListaIdentidades.Keys)
                                {
                                    //Guid destinatarioID = new Guid(identidad);

                                    DataWrapperIdentidad idenDW = idenCN.ObtenerIdentidadPorID(destinatarioID, true);
                                    GestionIdentidades gestorIdentidades = new GestionIdentidades(idenDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                                    Identidad identidadInvitado = gestorIdentidades.ListaIdentidades[destinatarioID];

                                    ControladorAmigos contrAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                                    contrAmigos.CargarAmigos(IdentidadActual, EsIdentidadActualAdministradorOrganizacion);

                                    gestorNotificaciones.AgregarNotificacionInvitacionContacto(IdentidadActual.IdentidadMyGNOSS, identidadInvitado.IdentidadMyGNOSS, TiposNotificacion.InvitacionContacto, BaseURL, ProyectoVirtual, UtilIdiomas.LanguageCode);

                                    //Limpiamos la cache de los contactos
                                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    amigosCL.InvalidarAmigos(IdentidadActual.IdentidadMyGNOSS.Clave);
                                    amigosCL.InvalidarAmigos(identidadInvitado.IdentidadMyGNOSS.Clave);
                                    amigosCL.Dispose();

                                    //PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService);
                                    //gestorIdentidades.GestorPersonas = new GestionPersonas(persCN.ObtenerPersonaPorID(identidadInvitado.PerfilUsuario.PersonaID.Value));
                                    //persCN.Dispose();

                                    enviadasA += separador + identidadInvitado.Nombre(UsuarioActual.UsuarioID);
                                    if (separador == "")
                                    {
                                        separador = ", ";
                                    }

                                    //gestorNotificaciones.AgregarNotificacionInvitacionContacto(IdentidadActual.IdentidadMyGNOSS, identidadInvitado.IdentidadMyGNOSS, TiposNotificacion.InvitacionContacto, this.BaseURLIdioma);
                                    //listaPefilesInvitados.Add(identidadInvitado.IdentidadMyGNOSS.PerfilID);
                                }

                                #endregion
                                invitacionesEnviadas = true;
                            }
                            else
                            {
                                //Usuario no GNOSS!
                                #region Usuario No Gnoss

                                Guid mOrganizacionID = ProyectoAD.MetaOrganizacion;

                                string urlEnlace = GnossUrlsSemanticas.GetURLAceptarInvitacion(BaseURLIdioma, UtilIdiomas);
                                string nombreRemitente = IdentidadActual.Nombre(UsuarioActual.UsuarioID);

                                if (!IdentidadActual.ModoPersonal)
                                {
                                    nombreRemitente = IdentidadActual.PerfilUsuario.NombreOrganizacion;
                                }

                                List<string> listaEmailsNoGnoss = new List<string>();
                                listaEmailsNoGnoss.Add(pCorreo);

                                enviadasA += separador + pCorreo;
                                if (separador == "")
                                {
                                    separador = ", ";
                                }

                                gestorNotificaciones.AgregarNotificacionInvitacionExternoAContacto(IdentidadActual.Clave, mOrganizacionID, ProyectoAD.MetaProyecto, "", nombreRemitente, DateTime.Now, listaEmailsNoGnoss, urlEnlace, IdiomaUsuario);

                                #endregion
                                invitacionesEnviadas = true;
                            }
                        }
                        catch (Exception)
                        {
                            listaEmailsMalos.Add(pCorreo);
                        }
                        finally
                        {
                            gestorIden.Dispose();
                        }
                    }
                    else
                    {
                        listaEmailsMalos.Add(pCorreo);
                    }
                }
            }

            if (invitacionesEnviadas)
            {
                //Insertamos las peticiones al final para controlar si se ha producido algún error al enviar los correo y que no se envie nada.
                if (gestorNotificaciones.GestorPeticiones != null)
                {
                    PeticionCN petCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    petCN.ActualizarBD();
                    petCN.Dispose();
                }

                if (IdentidadActual.GestorAmigos == null)
                {
                    notCN.ActualizarNotificacion();
                }
                else
                {
                    mEntityContext.SaveChanges();
                }

                //Agregar contactos que están ya en GNOSS
                //ControladorAmigos.AgregarNotificacionInvitacionNuevaAPerfiles(listaPefilesInvitados);
            }
            idenCN.Dispose();
            notCN.Dispose();
            gestorNotificaciones.Dispose();

            return listaEmailsMalos.ToArray();

        }


    }
}
