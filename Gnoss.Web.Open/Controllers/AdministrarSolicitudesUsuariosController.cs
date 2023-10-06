using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.Controles.Solicitudes;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Gnoss.Web.Controllers
{
    public class AdministrarSolicitudesUsuariosController : ControllerBaseWeb
    {
        public AdministrarSolicitudesUsuariosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
           : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
            
            //Textos y enlaces
            Guid idMyGNOSS = IdentidadActual.PerfilUsuario.IdentidadMyGNOSS.Clave;
            

            if (ProyectoSeleccionado.Estado != (short)EstadoProyecto.Abierto)
            {
                Response.Redirect(GnossUrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }
        }
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad})]
        public IActionResult Index(int? pagina)
        {
            if (!pagina.HasValue)
            {
                pagina = 1;
            }
            SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            
            var lista = solicitudCN.ObtenerModelUsuarioSolicitudesAccesoProyectoPorProyecto(UsuarioActual.OrganizacionID, UsuarioActual.ProyectoID, pagina.Value - 1, 10);
            AdministrarSolicitudesUsuarioModel administrarSolicitudesUsuarioModel = new AdministrarSolicitudesUsuarioModel();
            administrarSolicitudesUsuarioModel.SolicitudesNuevoUsuario = lista;
            administrarSolicitudesUsuarioModel.NumeroPaginaActual = pagina.Value;
            administrarSolicitudesUsuarioModel.NumeroResultadosTotal = solicitudCN.ObtenerModelUsuarioSolicitudesAccesoProyectoPorProyectoCount(UsuarioActual.OrganizacionID, UsuarioActual.ProyectoID);
            administrarSolicitudesUsuarioModel.UrlBusqueda = Request.Headers["Referer"].ToString();
            administrarSolicitudesUsuarioModel.PageName = UtilIdiomas.GetText("COMMON", "ADMINIISTRARUSUARIOSCOMUNIDAD");
            if (ProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
            {
                administrarSolicitudesUsuarioModel.ComunidadUrl = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto);
            }
            else
            {
                administrarSolicitudesUsuarioModel.ComunidadUrl = BaseURLIdioma;
            }
            return View(administrarSolicitudesUsuarioModel);
        }
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public IActionResult aceptar_solicitud(Guid solicitud_id)
        {
            SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperSolicitud solicitudDW = solicitudCN.ObtenerSolicitudesAccesoProyectoPorProyecto(UsuarioActual.OrganizacionID, UsuarioActual.ProyectoID);
            Solicitud solicitud = solicitudDW.ListaSolicitud.FirstOrDefault(item => item.SolicitudID.Equals(solicitud_id));
            SolicitudUsuario solicitudUsuario = solicitudDW.ListaSolicitudUsuario.FirstOrDefault(item => item.SolicitudID.Equals(solicitud_id));
            DataWrapperUsuario dataWrapperUsuario = new UsuarioAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerUsuarioCompletoPorID(solicitudUsuario.UsuarioID);
            Usuario usuario = dataWrapperUsuario.ListaUsuario.FirstOrDefault();
            if (solicitud != null)
            {
                solicitud.FechaProcesado = DateTime.Now;
                solicitud.Estado = (short)EstadoSolicitud.Aceptada;
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid identidadIDProyecto = identCN.ObtenerIdentidadIDDePersonaEnProyecto(ProyectoSeleccionado.Clave, solicitudUsuario.PersonaID)[0];
                DataWrapperIdentidad dataWrapperIdentidad = identCN.ObtenerPerfilesDePersona(solicitudUsuario.PersonaID, false, identidadIDProyecto);
                GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionUsuarios gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorIdentidades.GestorPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorIDCargaLigera(solicitudUsuario.PersonaID), mLoggingService, mEntityContext);
                Es.Riam.Gnoss.Elementos.Identidad.Perfil perfilUsuario = gestorIdentidades.ListaPerfiles[solicitudUsuario.PerfilID];

                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyectoCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();

                //Compruebo si hay que retomar perfil
                Guid identidadID = Guid.Empty;
                Guid perfilID = Guid.Empty;
                Es.Riam.Gnoss.Elementos.Identidad.Identidad ObjetoIdentidadProy = ControladorIdentidades.AgregarIdentidadPerfilYUsuarioAProyecto(gestorIdentidades, gestorUsuarios, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, usuario, perfilUsuario, recibirNewsletterDefectoProyectos);
                gestorIdentidades.RecargarHijos();
                //Invalido la cache de Mis comunidades
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarMisProyectos(ObjetoIdentidadProy.PerfilID);
                proyCL.Dispose();

                gestorIdentidades.RecargarHijos();

                dataWrapperIdentidad = gestorIdentidades.DataWrapperIdentidad;

                identidadID = ObjetoIdentidadProy.Clave;
                perfilID = ObjetoIdentidadProy.FilaIdentidad.PerfilID;
                Persona persona = gestorIdentidades.GestorPersonas.ListaPersonas[solicitudUsuario.PersonaID];

                #region Agrego cláusulas adicionales

                if (!string.IsNullOrEmpty(solicitudUsuario.ClausulasAdicionales))
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    gestorUsuarios.DataWrapperUsuario.Merge(proyectoCL.ObtenerClausulasRegitroProyecto(solicitud.ProyectoID));
                    gestorUsuarios.DataWrapperUsuario.Merge(proyectoCL.ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto));

                    List<Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg> listaAuxiliarProyRolUsuClausulaReg = gestorUsuarios.DataWrapperUsuario.ListaProyRolUsuClausulaReg.Where(item => item.UsuarioID.Equals(usuario.UsuarioID) && item.ProyectoID.Equals(solicitud.ProyectoID) && item.OrganizacionID.Equals(solicitud.OrganizacionID)).ToList();
                    foreach (Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg filaProyRolClau in listaAuxiliarProyRolUsuClausulaReg)
                    {
                        mEntityContext.EliminarElemento(filaProyRolClau);
                        gestorUsuarios.DataWrapperUsuario.ListaProyRolUsuClausulaReg.Remove(filaProyRolClau);
                    }

                    List<Guid> clausulasTrue = new List<Guid>();
                    string[] clausulasTexto = solicitudUsuario.ClausulasAdicionales.Split(',');

                    foreach (string clausula in clausulasTexto)
                    {
                        if (!string.IsNullOrEmpty(clausula))
                        {
                            clausulasTrue.Add(new Guid(clausula));
                        }
                    }

                    UsuarioCN usuarioCN2 = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperUsuario usuDW = usuarioCN2.ObtenerClausulasRegitroPorID(clausulasTrue);
                    usuarioCN2.Dispose();

                    Dictionary<KeyValuePair<Guid, Guid>, List<Guid>> listaProyectoClausulas = new Dictionary<KeyValuePair<Guid, Guid>, List<Guid>>();
                    foreach (Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in usuDW.ListaClausulaRegistro)
                    {
                        KeyValuePair<Guid, Guid> orgProy = new KeyValuePair<Guid, Guid>(filaClausula.OrganizacionID, filaClausula.ProyectoID);
                        if (listaProyectoClausulas.ContainsKey(orgProy))
                        {
                            listaProyectoClausulas[orgProy].Add(filaClausula.ClausulaID);
                        }
                        else
                        {
                            List<Guid> nuevaLista = new List<Guid>();
                            nuevaLista.Add(filaClausula.ClausulaID);
                            listaProyectoClausulas.Add(orgProy, nuevaLista);
                        }
                    }

                    foreach (KeyValuePair<Guid, Guid> proyectoClausulaID in listaProyectoClausulas.Keys)
                    {
                        gestorUsuarios.AgregarClausulasAdicionalesRegistroProy(usuario.UsuarioID, proyectoClausulaID.Key, proyectoClausulaID.Value, listaProyectoClausulas[proyectoClausulaID]);
                    }
                }

                #endregion

                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorNotificaciones.AgregarNotificacionSolicitud(solicitudUsuario.SolicitudID, DateTime.Now, ProyectoSeleccionado.Nombre, persona.FilaPersona.Nombre, "", TiposNotificacion.SolicitudAceptadaAccesoProyecto, persona.FilaPersona.Email, persona, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, UtilIdiomas.LanguageCode);

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                controladorPersonas.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[identidadID], ProyectoSeleccionado.Clave, true, true, PrioridadBase.Alta);

                #region Actualizar cola GnossLIVE

                ControladorDocumentacion.ActualizarGnossLive(ProyectoSeleccionado.Clave, perfilID, Es.Riam.Gnoss.AD.Live.AccionLive.Agregado, (int)Es.Riam.Gnoss.AD.Live.TipoLive.Miembro, false, PrioridadLive.Alta);

                #endregion

                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCL.EliminarCacheGestorTodasIdentidadesUsuario(solicitudUsuario.UsuarioID, solicitudUsuario.PersonaID);

                List<string> listaClavesInvalidar = new List<string>();

                string prefijoClave;

                if (!string.IsNullOrEmpty(identidadCL.Dominio))
                {
                    prefijoClave = identidadCL.Dominio;
                }
                else
                {
                    prefijoClave = IdentidadCL.DominioEstatico;
                }

                prefijoClave = prefijoClave + "_" + identidadCL.ClaveCache[0] + "_";
                prefijoClave = prefijoClave.ToLower();
                string rawKey = string.Concat("IdentidadActual_", solicitudUsuario.PersonaID, "_", solicitudUsuario.PerfilID);
                listaClavesInvalidar.Add(prefijoClave + rawKey.ToLower());
                string rawKey2 = "PerfilMVC_" + solicitudUsuario.PerfilID;
                listaClavesInvalidar.Add(prefijoClave + rawKey2.ToLower());

                identidadCL.InvalidarCachesMultiples(listaClavesInvalidar);

                //Limpiamos la cache de contactos para el proyecto
                AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                amigosCL.InvalidarAmigosPertenecenProyecto(ProyectoSeleccionado.Clave);


                //return UtilAJAX.RedirigirCallBackAUrl(GnossUrlsSemanticas.GetURLAdministrarSolicitudesAccesoProyecto(BaseURLIdioma, UtilIdiomas, IdentidadActual, ProyectoSeleccionado.NombreCorto, null));
                mEntityContext.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest($"No se ha encontrado la solicitud con id {solicitud_id}");
            }
           
        }

        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public IActionResult rechazar_solicitud(Guid solicitud_id)
        {

            SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool exito = solicitudCN.RechazarSolicitud(solicitud_id);
            if (exito)
            {
                return Ok();
            }
            else
            {
                return BadRequest($"No se ha encontrado la solicitud con id {solicitud_id}");
            }
            
            //return UtilAJAX.RedirigirCallBackAUrl(GnossUrlsSemanticas.GetURLAdministrarSolicitudesAccesoProyecto(BaseURLIdioma, UtilIdiomas, IdentidadActual, ProyectoSeleccionado.NombreCorto, null));
        }
    }
}
