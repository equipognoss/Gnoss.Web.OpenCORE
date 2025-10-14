using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.IO;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{

    public class CkeditorController : ControllerBaseGnoss
    {
        /// <summary>
        /// ID del usuario.
        /// </summary>
        private Guid mUsuarioID;

        /// <summary>
        /// ID para el archivo.
        /// </summary>
        private Guid mEspecialID;

        /// <summary>
        /// Extensión del archivo.
        /// </summary>
        private string mExtension;

        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public CkeditorController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IHostingEnvironment env, EntityContextBASE entityContextBASE, IAvailableServices availableServices, ILogger<CkeditorController> logger, ILoggerFactory loggerFactory)
            : base(httpContextAccessor, entityContext, loggingService, configService, redisCacheWrapper, virtuosoAD, gnossCache, viewEngine, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, env, entityContextBASE, availableServices,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }


        public class ObjetoJson
        {
            public string fileName { get; set; }
            public int uploaded { get; set; }
            public string url { get; set; }
        }

        [HttpPost]
        public JsonResult Index()
        {
            if (Session.Get<GnossIdentity>("Usuario") == null || Session.Get<GnossIdentity>("Usuario").EsUsuarioInvitado)
            {
                throw new Exception("No tienes permiso");
            }

            string nombreFichero = "";

            try
            {
                mUsuarioID = Session.Get<GnossIdentity>("Usuario").UsuarioID;
                mEspecialID = Guid.NewGuid();

                IFormFile fileUpload = Request.Form.Files[0];
                if (fileUpload == null)
                {
                    fileUpload = Request.Form.Files["upload"];
                }

                byte[] buffer1;
                mExtension = System.IO.Path.GetExtension(new FileInfo(fileUpload.FileName).Name).ToLower();
                nombreFichero = fileUpload.FileName;

                BinaryReader reader = new BinaryReader(fileUpload.OpenReadStream());
                buffer1 = reader.ReadBytes((int)fileUpload.Length);

                bool correcto = false;

                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                servicioImagenes.Url = UrlIntragnossServicios.ToString();

                correcto = servicioImagenes.AgregarImagenADirectorio(buffer1, Path.Combine(UtilArchivos.ContentImagenesUsuarios, "ImagenesCKEditor",  mUsuarioID.ToString()), mEspecialID.ToString(), mExtension);

                if (!correcto)
                {
                    throw new Exception("Archivo no subido");
                }

            }
            catch (Exception ex)
            {
                GuardarLogError(ex.Message + "\n" + ex.StackTrace);
                throw;
            }

            ObjetoJson objetoJsonDevolver = new ObjetoJson();
            objetoJsonDevolver.fileName = nombreFichero;
            objetoJsonDevolver.uploaded = 1;
            objetoJsonDevolver.url = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesUsuarios}/ImagenesCKEditor/{mUsuarioID.ToString().ToLower()}/{mEspecialID}{mExtension}";
            return Json(objetoJsonDevolver);
        }

    }


}