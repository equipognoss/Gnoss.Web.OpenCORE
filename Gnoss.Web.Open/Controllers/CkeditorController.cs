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
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
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


        public CkeditorController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(httpContextAccessor, entityContext, loggingService, configService, redisCacheWrapper, virtuosoAD, gnossCache, viewEngine, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication)
        {
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
            if (Session.Get("Usuario") == null || Session.Get<GnossIdentity>("Usuario").EsUsuarioInvitado)
            {
                throw new Exception("No tienes permiso");
            }
            //JsonResult json = new JsonResult();

            string nombreFichero = "";

            try
            {
                mUsuarioID = Session.Get<GnossIdentity>("Usuario").UsuarioID;
                mEspecialID = Guid.NewGuid();

                IFormFile fileUpload = Request.Form.Files["fuCKEditor"];
                if (fileUpload == null)
                {
                    fileUpload = Request.Form.Files["upload"];
                }

                byte[] buffer1;
                mExtension = System.IO.Path.GetExtension(new FileInfo(fileUpload.FileName).Name).ToLower();
                nombreFichero = fileUpload.FileName;

                BinaryReader reader = new BinaryReader(fileUpload.OpenReadStream());
                buffer1 = reader.ReadBytes((int)fileUpload.Length);
                //reader.Close(); NO SE CIERRA, SI NO NOS CARGAMOS EL ARCHIVO

                bool correcto = false;

                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
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
            objetoJsonDevolver.url = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesUsuarios + "/" + "ImagenesCKEditor/" + mUsuarioID.ToString().ToLower() + "/" + mEspecialID + mExtension;
            return Json(objetoJsonDevolver);
        }

    }


}