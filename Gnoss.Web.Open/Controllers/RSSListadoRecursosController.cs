using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    /// <summary>
    /// ViewModel de la página de administrar pestañas
    /// </summary>
    [Serializable]
    public class RSSListadoRecursosViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, string> ListaFuentesRSS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumResultados { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumPage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FichaRecursoRSS> ListaRecursos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public class FichaRecursoRSS
        {
            public Guid Key { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public string EnlaceEditar { get; set; }
            public Dictionary<Guid, string> Categorias { get; set; }
            public List<string> Tags { get; set; }
            public bool EstaCompleto { get; set; }
        }
    }

    public class RSSListadoRecursosController : ControllerBaseWeb
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public RSSListadoRecursosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<RSSListadoRecursosController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {

            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros

        private short mNumPagina = 1;

        private RSSListadoRecursosViewModel mPaginaModel = null;

        private bool? mEsAdmin = null;

		#endregion

		#region Metodos de evento

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
        {
            return View(PaginaModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Filtrar(short NumPagina, Guid Fuente, bool SoloCompletos)
        {
            mNumPagina = NumPagina;

            return PartialView("_ResultadosRecursosRSS", PaginaModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Publicar(Guid Fuente, bool SoloCompletos, string RecursosID)
        {
            List<Guid> listaID = ObtenerIDsRecursos(RecursosID);

            short numPublicados = PublicarRecursos(listaID);

            if (numPublicados > -1)
            {
                return PartialView("_ResultadosRecursosRSS", PaginaModel);
            }
            else
            {
                return GnossResultERROR(UtilIdiomas.GetText("COMADMINFUENTES", "ERRORPUBLICAR"));
            }
        }

        /// <summary>
        /// Publica los recursos de la lista pasada como parámetro
        /// </summary>
        /// <param name="pListaIDRecursos">Lista de identificadores de recursos</param>
        private short PublicarRecursos(List<Guid> pListaIDRecursos)
        {
            short numPublicados = 0;
            try
            {
                GestorDocumental gestDocumental = ControladorDocumentacion.CargaIncialDeBaseRecursos(new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory), null, true, UsuarioActual.ProyectoID, UsuarioActual.UsuarioID, UsuarioActual.OrganizacionID);
                gestDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
                ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorIdentidades>(), mLoggerFactory);
                controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                Dictionary<Documento, List<Proyecto>> listaDocsParaTwitter = new Dictionary<Documento, List<Proyecto>>();
                List<Proyecto> listaProyTwitter = new List<Proyecto>();
                listaProyTwitter.Add(ProyectoSeleccionado);

                if (ProyectoSeleccionado.FilaProyecto.TieneTwitter)
                {
                    ControladorDocumentacion.EnviarEnlaceATwitterDeComunidad(IdentidadActual, listaDocsParaTwitter, BaseURLIdioma, UtilIdiomas, mAvailableServices);
                }

            }
            catch (Exception)
            {
                numPublicados = -1;
            }
            return numPublicados;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(Guid Fuente, bool SoloCompletos, string RecursosID)
        {
            return PartialView("_ResultadosRecursosRSS", PaginaModel);
        }

        private List<Guid> ObtenerIDsRecursos(string RecursosID)
        {

            string[] listaRecursos = RecursosID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> listaID = new List<Guid>();
            foreach (string id in listaRecursos)
            {
                listaID.Add(new Guid(id));
            }
            return listaID;
        }


        private Dictionary<Guid, string> CargarListaFuentesRSS()
        {
            Dictionary<Guid, string> listaFuentesRSS = new Dictionary<Guid, string>();

            //Es administrador y puede administrar todas
            if (EsAdmin)
            {
                listaFuentesRSS.Add(Guid.Empty, UtilIdiomas.GetText("COMADMINFUENTES", "TODAS"));
            }

            return listaFuentesRSS;
        }

        #endregion

        #region Propiedades

        private bool EsAdmin
        {
            get
            {
                if (!mEsAdmin.HasValue)
                {
                    mEsAdmin = ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID);
                }
                return mEsAdmin.Value;
            }
        }

        private RSSListadoRecursosViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new RSSListadoRecursosViewModel();

                    mPaginaModel.ListaFuentesRSS = CargarListaFuentesRSS();

                    int numResultados = 0;
                    mPaginaModel.NumResultados = numResultados;
                    mPaginaModel.NumPage = mNumPagina;
                }
                return mPaginaModel;
            }
        }
        #endregion
    }
}