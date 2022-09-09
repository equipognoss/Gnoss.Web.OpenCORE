using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
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
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Gnoss.Web.Controllers
{
    public class AdministrarSolicitudesNuevasComunidadesController : ControllerBaseWeb
    {
        public AdministrarSolicitudesNuevasComunidadesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
           : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
        public IActionResult Index(int? pagina)
        {
            if (!pagina.HasValue)
            {
                pagina = 1;
            }
            PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperPeticion peticionDW = peticionCN.ObtenerPeticionComunidadesPendientesDeAceptarPaginacion(pagina.Value - 1, 5);
            AdministrarSolicitudesNuevasComunidades administrarSolicitudesNuevasComunidades = new AdministrarSolicitudesNuevasComunidades();
            List<SolicitudNuevaComunidadModel> solicitudesNuevasComunidades = new List<SolicitudNuevaComunidadModel>();
            foreach (var peticionNuevoProyecto in peticionDW.ListaPeticionNuevoProyecto)
            {
                SolicitudNuevaComunidadModel solicitudNuevaComunidadModel = new SolicitudNuevaComunidadModel()
                {
                    ComunidadPrivadaPadreID = peticionNuevoProyecto.ComunidadPrivadaPadreID,
                    Descripcion = peticionNuevoProyecto.Descripcion,
                    Nombre = peticionNuevoProyecto.Nombre,
                    NombreCorto = peticionNuevoProyecto.NombreCorto,
                    PeticionID = peticionNuevoProyecto.PeticionID,
                    IsPrivate = peticionNuevoProyecto.Tipo != (short)TipoAcceso.Publico
                };
                solicitudesNuevasComunidades.Add(solicitudNuevaComunidadModel);
            }
            administrarSolicitudesNuevasComunidades.SolicitudesNuevasComunidades = solicitudesNuevasComunidades;
            administrarSolicitudesNuevasComunidades.NumeroPaginaActual = pagina.Value;
            administrarSolicitudesNuevasComunidades.NumeroResultadosTotal = peticionCN.ObtenerPeticionComunidadesPendientesDeAceptarCount();
            administrarSolicitudesNuevasComunidades.PageName = UtilIdiomas.GetText("COMMON", "ADMINISTRARNUEVASCOMUNIDADES"); 
            administrarSolicitudesNuevasComunidades.UrlBusqueda = Request.Headers["Referer"].ToString();
            if (ProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
            {
                administrarSolicitudesNuevasComunidades.ComunidadUrl = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto);
            }
            else
            {
                administrarSolicitudesNuevasComunidades.ComunidadUrl = BaseURLIdioma;
            }
            return View(administrarSolicitudesNuevasComunidades);
        }

        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
        public IActionResult aceptar_peticion(Guid peticion_id)
        {
            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
            servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");
            Dictionary<string, byte[]> logosProyectos = new Dictionary<string, byte[]>();
            string ruta = $"proyectos/{peticion_id}";
            byte[] imagenLogo = servicioImagenes.ObtenerImagen(ruta + "_temp", ".png");
            logosProyectos.Add(peticion_id.ToString(), imagenLogo);
            //Obtenemos la peticion del nuevo proy
            PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperPeticion peticionDW = peticionCN.ObtenerPeticionComunidadesPendientesDeAceptar();
            PeticionNuevoProyecto peticion = peticionDW.ListaPeticionNuevoProyecto.FirstOrDefault(item => item.PeticionID.Equals(peticion_id));
            
            if (peticion != null)
            {
                Guid organizacionID = ProyectoAD.MetaOrganizacion;
                Guid idPadre = Guid.Empty;
                if (peticion.ComunidadPrivadaPadreID.HasValue)
                {
                    ProyectoCN proyeCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    organizacionID = proyeCN.ObtenerProyectoCargaLigeraPorID(peticion.ComunidadPrivadaPadreID.Value).ListaProyecto.First().OrganizacionID;
                    idPadre = peticion.ComunidadPrivadaPadreID.Value;
                }
                ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                DataWrapperOrganizacion orgDW = null;
                DataWrapperProyecto dataWrapperProyecto = null;
                GestorParametroGeneral paramDS = new GestorParametroGeneral();
                DataWrapperTesauro tesauroDW = null;
                DataWrapperDocumentacion dataWrapperDocumentacion = null;
                DataWrapperUsuario dataWrapperUsuario = null;
                DataWrapperIdentidad identidadadDW = null;

                Es.Riam.Gnoss.Elementos.ServiciosGenerales.Proyecto proyecto = controladorProyecto.CrearNuevoProyecto(peticion.Nombre, peticion.NombreCorto, peticion.Descripcion, null, peticion.Tipo, 1, peticion.Peticion.UsuarioID.Value, peticion.PerfilCreadorID, organizacionID, idPadre, true, true, true, true, false, imagenLogo, out orgDW, out dataWrapperProyecto, out paramDS, out tesauroDW, out dataWrapperDocumentacion, out dataWrapperUsuario, out identidadadDW);
                peticion.Peticion.Estado = (short)EstadoPeticion.Aceptada;
                peticion.Peticion.FechaProcesado = DateTime.Now;
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<PersonaIdentidad> listaPersonaIdentidad = personaCN.ObtenerEmaileIdentidadATravesDePerfil(peticion.PerfilCreadorID);
                DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                if (listaPersonaIdentidad.Count == 1)
                {
                    GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    string nombre = listaPersonaIdentidad.First().Nombre;
                    Guid personaID = (Guid)listaPersonaIdentidad.First().PersonaID;
                    string correo = listaPersonaIdentidad.First().Email;
                    string enlaceAComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proyecto.NombreCorto);
                    gestorNotificaciones.AgregarNotificacionSolicitudAceptadaNuevaComunidad(nombre, personaID, proyecto.Clave, proyecto.FilaProyecto.OrganizacionID, proyecto.Nombre, correo, enlaceAComunidad, UtilIdiomas.LanguageCode);
                }
                mEntityContext.SaveChanges();
                //Actualizo el modelo base:
                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                controladorPersonas.ActualizarModeloBASE(proyecto.IdentidadCreadoraProyecto, proyecto.Clave, true, true, PrioridadBase.Alta);

                if (imagenLogo != null)
                {
                    //borramos la imagen temporal y dejamos la definitiva
                    servicioImagenes.BorrarImagen(ruta + "_temp.png");
                    servicioImagenes.AgregarImagen(imagenLogo, "proyectos/" + proyecto.Clave.ToString(), ".png");
                }

                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarCacheListaProyectosPerfil(peticion.PerfilCreadorID);
                string versionDocConfiguracion = string.Empty;
                string urlDocConfigComunidad = string.Empty;
                byte[] buffer = null;
                string xml = string.Empty;

                List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VersionDocConfigDefectoComunidad")).ToList();
                if (busqueda.Count > 0)
                {
                    versionDocConfiguracion = busqueda.First().Valor;
                    urlDocConfigComunidad = BaseURLContent + "/Documentacion/Configuracion/ConfiguracionComunidadDefecto.xml?v=" + versionDocConfiguracion;
                    using (WebClient webClient = new WebClient())
                    {
                        xml = webClient.DownloadString(urlDocConfigComunidad);
                    }
                }


                if (!string.IsNullOrEmpty(xml))
                {
                    controladorProyecto.ConfigurarComunidadConXML(xml, organizacionID, proyecto.Clave);

                    //Subimos el fichero al servidor
                    Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                    GestionDocumental gd = new GestionDocumental(mLoggingService, mConfigService);
                    gd.Url = UrlServicioWebDocumentacion.Replace("https://", "http://");
                    gd.AdjuntarDocumentoADirectorio(buffer, "Configuracion/" + proyecto.Clave, proyecto.Clave.ToString(), ".xml");
                    gd.AdjuntarDocumentoADirectorio(buffer, "Configuracion/" + proyecto.Clave, proyecto.Clave.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".xml");
                    mLoggingService.AgregarEntradaDependencia("Subir archivo de configuración por defecto al gestor documental", false, "AceptarSolicitudNuevaComunidad", sw, true);
                }
                return Ok();
            }
            else
            {
                return BadRequest($"No se ha encontrado la petición con el id {peticion_id}");
            }
        }
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
        public IActionResult rechazar_peticion(Guid peticion_id)
        {
            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
            servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");

            GeneralCN generalCN = new GeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
            GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperPeticion peticionDW = peticionCN.ObtenerPeticionComunidadesPendientesDeAceptar();
            string nombreProy = peticionDW.ListaPeticionNuevoProyecto.FirstOrDefault(item => item.PeticionID.Equals(peticion_id)).Nombre;
            Guid perfilCreadorID = peticionDW.ListaPeticionNuevoProyecto.FirstOrDefault(item => item.PeticionID.Equals(peticion_id)).PerfilCreadorID;

            PeticionNuevoProyecto peticionNuevoProyecto = peticionDW.ListaPeticionNuevoProyecto.FirstOrDefault(item => item.PeticionID.Equals(peticion_id));
            peticionDW.ListaPeticionNuevoProyecto.Remove(peticionNuevoProyecto);
            mEntityContext.EliminarElemento(peticionNuevoProyecto);
            Peticion fila = peticionDW.ListaPeticion.FirstOrDefault(item => item.PeticionID.Equals(peticion_id));
            if (fila != null)
            {
                fila.Estado = (short)EstadoPeticion.Rechazada;
                fila.FechaProcesado = DateTime.Now;
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<PersonaIdentidad> listaPersonaIdentidad = personaCN.ObtenerEmaileIdentidadATravesDePerfil(perfilCreadorID);
                if (listaPersonaIdentidad.Count == 1)
                {
                    string nombre = listaPersonaIdentidad.First().Nombre;
                    Guid personaID = listaPersonaIdentidad.First().PersonaID;
                    string correo = listaPersonaIdentidad.First().Email;
                    gestorNotificaciones.AgregarNotificacionSolicitudRechazadaNuevaComunidad(nombre, personaID, nombreProy, correo, UtilIdiomas.LanguageCode);
                }
                mEntityContext.SaveChanges();
                String ruta = "proyectos/" + peticion_id;
                servicioImagenes.BorrarImagen(ruta + "_temp.png");
                return Ok();
            }
            else
            {
                return BadRequest($"No se ha encontrado la petición con el id {peticion_id}");
            }
        }
    }
}
