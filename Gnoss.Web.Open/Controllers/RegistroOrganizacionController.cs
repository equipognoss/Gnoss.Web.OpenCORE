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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gnoss.Web.Controllers
{
    public class RegistroOrganizacionController : ControllerBaseWeb
    {
        public RegistroOrganizacionController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
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
            return controladorDeSolicitudes.GuardarRegistroOrganizacion(pNombreCorto, pNombre, UsuarioActual.UsuarioID);
        }

        [HttpPost]
        public IActionResult CrearNuevaCuenta(string nombre_corto, string nombre)
        {
            RegistrarOrgViewModel registrarOrg = new RegistrarOrgViewModel();
            try
            {
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
    }
}
