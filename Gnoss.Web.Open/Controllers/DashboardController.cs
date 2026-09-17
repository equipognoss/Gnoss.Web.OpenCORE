using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class DashboardController : ControllerPestanyaBase
    {
        #region Miembros

        private Identidad mIdentidadPaginaContribuciones;

        /// <summary>
        /// Indica si la indentidad que se está buscando existe
        /// </summary>
        private bool? mExisteIdentidadPaginaContribuciones;

        private List<ProyectoPestanyaDashboardAsistente> mFilasPestanyaDashboardAsistente;

        private bool mEsVistaMapa = false;

        private int mContRedirecciones = 0;

        private DataWrapperProyecto mProyectoDW = null;
        /// <summary>
        /// Filtro de orden configurados.
        /// </summary>
        private Dictionary<string, string> mFiltrosOrdenConfig;

        protected List<string> mListaItems;
        /// <summary>
        /// Lista de filtros de búsqueda
        /// </summary>
        protected Dictionary<string, List<string>> mListaFiltrosFacetas = new Dictionary<string, List<string>>();

        public const string SEPARADOR_MULTIVALOR = "@|@";

        private UtilWeb mUtilWeb;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        public DashboardController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IWebHostEnvironment env, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<DashboardController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env,utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices,logger,loggerFactory)
        {
            mUtilWeb = new UtilWeb(httpContextAccessor);
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Metodos

        [TypeFilter(typeof(AccesoPestanyaAttribute))]
        public ActionResult Index()
        {
        
            DashboardViewModel paginaModel = new DashboardViewModel();

            ObtenerDatosPaginaDashboard(paginaModel);

            paginaModel.TipoPagina = TipoPagina.ToString();

            return View(paginaModel);
        }

        private void ObtenerDatosPaginaDashboard(DashboardViewModel pModelo)
        {
            string nombrePestaña = ObtenerNombrePestañaDeURL(mUtilWeb.RequestUrl());

            List<ProyectoPestanyaDashboardAsistente> filasAsistente = ProyectoDW.ListaProyectoPestanyaDashboardAsistente.Where(item => item.PestanyaID.Equals(ProyectoPestanyaActual.Clave)).OrderBy(item => item.Orden).ToList();

            //if (filasAsistente.Count > 0)
            //{
            //    if (pModelo.ListAsistente == null)
            //    {
            //        pModelo.ListAsistente = new List<DashboardViewModel.AsistenteModel>();
            //    }
            //    foreach (ProyectoPestanyaDashboardAsistente filaAsistente in filasAsistente)
            //    {
            //        DashboardViewModel.AsistenteModel asis = new DashboardViewModel.AsistenteModel();

            //        CargadorResultados cargadorResultados = new CargadorResultados();
            //        cargadorResultados.Url = mConfigService.ObtenerUrlServicioResultados();

            //        string res=cargadorResultados.CargarResultados(new Guid("b20aca73-c0bf-4ffd-a4f1-0379b0b32830"), new Guid("2ADB7944-2506-4E60-A3AB-9335D83A17FC"), false, this.Request.GetDisplayUrl(), false, false, TipoBusqueda.Recursos, "b20aca73-c0bf-4ffd-a4f1-0379b0b32830", "busquedaTipoChart%3D1c9c3117-1d87-40e3-b243-d1e98c4375c9%7CPestanyaActualID%3Da803c21a-b53f-40b2-b19e-38a3f8900864%7Crdf%3Atype%3Dpelicula%7CordenarPor%3Dschema%3AaggregateRating", "", false, "es", 1, "", new Guid("525420e3-b810-9195-fd5d-f7dcb401aeb1"), Request);

            //        string[] datos = res.Split("|||");
            //        asis.Labels = new string[datos.Length - 1];
            //        for (int i = 1; i < datos.Length - 1; i++)
            //        {
            //            asis.Labels[i - 1] = datos[i].Split("@@@")[0];
            //        }
            //        //asis.Labels = new string[100];
            //        //for (int i = 1; i < 100 - 1; i++)
            //        //{
            //        //    asis.Labels[i - 1] = datos[i].Split("@@@")[0];
            //        //}

            //        asis.Key = filaAsistente.AsisID;
            //        asis.AsistenteName = filaAsistente.Nombre;
            //        asis.Horizontal = filaAsistente.Horizontal;
                    
            //        asis.Tamanyo = filaAsistente.Tamanyo;
            //        asis.Tipo = filaAsistente.Tipo;
            //        asis.Titulo = filaAsistente.Titulo;
            //        asis.ListDataset = new List<DashboardViewModel.AsistenteModel.DatasetModel>();
            //        List<ProyectoPestanyaDashboardAsistenteDataset> filasDataset = filaAsistente.ProyectoPestanyaDashboardAsistenteDataset.ToList();
            //        int dataset = 1;
            //        foreach (ProyectoPestanyaDashboardAsistenteDataset filaDataset in filasDataset)
            //        {
            //            DashboardViewModel.AsistenteModel.DatasetModel dat = new DashboardViewModel.AsistenteModel.DatasetModel();

            //            dat.Datos = new string[datos.Length - 1];
            //            for (int i = 1; i < datos.Length - 1; i++)
            //            {
            //                dat.Datos[i - 1] = datos[i].Split("@@@")[dataset+1];
            //            }
            //            //dat.Datos = new string[100];
            //            //for (int i = 1; i < 100 - 1; i++)
            //            //{
            //            //    dat.Datos[i - 1] = datos[i].Split("@@@")[dataset];
            //            //}

            //            dat.Color = filaDataset.Color;
            //            dat.Nombre = filaDataset.Nombre;
            //            dat.Key = filaDataset.DatasetID;

            //            asis.ListDataset.Add(dat);
            //            dataset++;
            //        }
            //        pModelo.ListAsistente.Add(asis);
            //    }
            //}

            //pModelo.PageName = ProyectoDW.ListaProyectoPestanyaMenu.FirstOrDefault(item => item.PestanyaID.Equals(ProyectoPestanyaActual.Clave)).Nombre.Split('@')[0];

            //TipoPagina = TiposPagina.Dashboard;

            //if (ProyectoPestanyaActual != null && !string.IsNullOrEmpty(ProyectoPestanyaActual.Titulo))
            //{
            //    pModelo.PageTittle = UtilCadenas.ObtenerTextoDeIdioma(ProyectoPestanyaActual.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            //}
        }

        private string ObtenerNombrePestañaDeURL(string pUrl)
        {
            mLoggingService.AgregarEntrada("MVC-ObtenerNombrePestañaDeURL-purl = " + pUrl);

            string nombrePestaña = "Dashboard Analítico";

            nombrePestaña = pUrl.Substring(pUrl.IndexOf('/' + ProyectoSeleccionado.NombreCorto + '/') + ProyectoSeleccionado.NombreCorto.Length + 2);

            mLoggingService.AgregarEntrada("MVC-ObtenerNombrePestañaDeURL-nombrePestaña = " + nombrePestaña);

            if (nombrePestaña.IndexOf('/') > 0)
            {
                nombrePestaña = nombrePestaña.Substring(0, nombrePestaña.IndexOf('/'));
                mLoggingService.AgregarEntrada("MVC-ObtenerNombrePestañaDeURL-nombrePestaña = " + nombrePestaña);
            }

            if (nombrePestaña.IndexOf('?') > 0)
            {
                nombrePestaña = nombrePestaña.Substring(0, nombrePestaña.IndexOf('?'));
                mLoggingService.AgregarEntrada("MVC-ObtenerNombrePestañaDeURL-nombrePestaña = " + nombrePestaña);
            }

            return nombrePestaña;
        }

        #endregion

        #region Propiedades

        private DataWrapperProyecto ProyectoDW
        {
            get
            {
                if (mProyectoDW == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mProyectoDW = proyCN.ObtenerProyectoDashboardPorID(ProyectoSeleccionado.Clave);
                    proyCN.Dispose();
                }
                return mProyectoDW;
            }
        }
        #endregion

    }
}
