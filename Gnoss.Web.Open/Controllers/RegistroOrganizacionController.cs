using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.Solicitudes;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gnoss.Web.Controllers
{
    public class RegistroOrganizacionController : ControllerBaseWeb
    {
        public RegistroOrganizacionController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {

        }
        /// <summary>
        /// Verdad si el registro es de una clase
        /// </summary>
        private bool EsRegistroDeClase
        {
            get
            {
                return ((RequestParams("clase") != null) && (RequestParams("clase").Equals("true")));
            }
        }

        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorMetaProyecto })]
        public ActionResult Index()
        {
            RegistrarOrgViewModel registrarOrg = new RegistrarOrgViewModel();
            registrarOrg.PageName = "Registro de Organizacion";
            return View(registrarOrg);
        }

        private bool ValidarNombreCortoOrganizacion(string pNombreCorto)
        {
            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool existeNombreCorto = orgCN.ExisteNombreCortoEnBD(pNombreCorto);

            return !existeNombreCorto;
        }


        private bool ValidarOrganizacion(string pOrganizacion)
        {
            return !new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ExisteOrganizacion(pOrganizacion);
        }

        [NonAction]
        private Guid Guardar(string pNombreCorto, string pNombre)
        {
            ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            Guid idOrganizacion = controladorDeSolicitudes.GuardarRegistroOrganizacion(pNombreCorto, pNombre, UsuarioActual.UsuarioID);

            EliminarCacheGestorIdentidades();

            return idOrganizacion;
        }

        [HttpPost]
        public IActionResult CrearNuevaCuenta(string nombre_corto, string nombre)
        {
            RegistrarOrgViewModel registrarOrg = new RegistrarOrgViewModel();
            try
            {
                bool existeNombreCorto = ExisteNombreCortoOrganizacion(nombre_corto);
                if (existeNombreCorto)
                {
                    return GnossResultERROR($"Ya existe una organizacion llamada {nombre_corto}. Introduce otro nombre por favor.");
                }

                Guid idOrg = Guardar(nombre_corto, nombre);   
                registrarOrg.PageName = UtilIdiomas.GetText("COMMON", "SOLICITARORGANIZACION");
                registrarOrg.IDOrg = idOrg;
                registrarOrg.Success = true;
            }
            catch (Exception ex)
            {
                registrarOrg.Error = true;
            }
            return View("Index", registrarOrg); 
        }

        /// <summary>
        /// Guarda la foto del perfil
        /// </summary>
        /// <param name="pFila">Fila de la organización o de la solicitud</param>
        private void GuardarLogo(SolicitudNuevaOrganizacion pFila, IFormFile pLogo)
        {
            if (pLogo != null && pLogo.Length > 0)
            {
                byte[] fileBytes = null;

                using (var ms = new MemoryStream())
                {
                    pLogo.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                string url = UrlIntragnossServicios;
                servicioImagenes.Url = url;
                string idFoto;

                idFoto = $"{UtilArchivos.ContentImagenesSolicitudes}/{pFila.SolicitudID}";

                servicioImagenes.AgregarImagen(fileBytes, idFoto, ".png");
                Session.Remove("Logo");
            }
        }

        /// <summary>
        /// Comprueba si el nombre corto de la organización introducido existe ya en la base de datos
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto de la organización a comprobar</param>
        /// <returns>Si existe o no el nombre corto</returns>
        private bool ExisteNombreCortoOrganizacion(string pNombreCorto)
        {
            return mEntityContext.Organizacion.Any(item => item.NombreCorto.ToLower().Equals(pNombreCorto.ToLower()));
        }

        private void EliminarCacheGestorIdentidades()
        {
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Clave, IdentidadActual.PersonaID.Value);
        }
    }
}
