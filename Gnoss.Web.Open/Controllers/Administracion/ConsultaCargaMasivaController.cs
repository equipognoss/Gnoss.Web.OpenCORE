using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Carga;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using System.Linq;


namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Consulta de las cargas masivas
    /// </summary>
    public class ConsultaCargaMasivaController : ControllerBaseWeb
    {
        public ConsultaCargaMasivaController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        private const int NUMERO_CARGAS_MOSTRADAS = 15;
        private ConsultaCargaMasivaViewModel mPaginaModel = null;
        private const short CERRADA = 1;
        private const short ABIERTA = 0;
        public const string CARGAABIERTA = "Abierta";
        public const string CARGACERRADA = "Cerrada en curso";
        public const string CARGACOMPLETADA = "Cerrada Completada";

        #endregion

        #region Métodos de evento
        /// <summary>
        /// Carga index de la pagina
        /// </summary>
        /// <returns>Actionresult</returns>
        [HttpGet]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            mPaginaModel = CargarModelo();
            return View(mPaginaModel);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Carga el modelo de datos
        /// </summary>
        /// <returns>Modelo de consulta de la carga masiva</returns>
        private ConsultaCargaMasivaViewModel CargarModelo()
        {
            ConsultaCargaMasivaViewModel consulta = new ConsultaCargaMasivaViewModel();
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Carga> listaCargas = proyCN.ObtenerCargasMasivasPorIdentidadID(IdentidadActual.Clave);
            consulta.ListaInformacion = new List<InformacionMostrar>();

            foreach(Carga c in listaCargas.Take(NUMERO_CARGAS_MOSTRADAS))
            {
                InformacionMostrar info = new InformacionMostrar();
                info.NombreCarga = c.Nombre;
                info.FechaCarga = c.FechaAlta.ToString();

                List<CargaPaquete> listaPaquetes = new List<CargaPaquete>();
                listaPaquetes = proyCN.ObtenerPaquetesPorIDCarga(c.CargaID);
                info.NumPaquetes = listaPaquetes.Count;

                List<CargaPaquete> paquetesProcesados = listaPaquetes.Where(x => x.Estado == (short)EstadoPaquete.Correcto).ToList();
                info.NumParquetesProcesados = paquetesProcesados.Count;

                info.IDCarga = c.CargaID.ToString();

                if (c.Estado == ABIERTA)
                {
                    info.EstadoCarga = CARGAABIERTA;
                }
                else if (c.Estado == CERRADA)
                {
                    info.EstadoCarga = CARGACERRADA;
                    if (info.NumPaquetes == info.NumParquetesProcesados)
                    {
                        info.EstadoCarga = CARGACOMPLETADA;
                    }
                }

                if (listaPaquetes.Any(x => !string.IsNullOrEmpty(x.Error)))
                {
                    info.EstadoCarga = "ERROR en la carga";
                    List<CargaPaquete> listaPaquetesConError = listaPaquetes.Where(x => !string.IsNullOrEmpty(x.Error)).ToList();
                    info.NumPaquetesConError = listaPaquetesConError.Count;
                    info.ErrorCarga = $"Carga con error: {listaPaquetesConError.FirstOrDefault().Error}";
                }

                consulta.ListaInformacion.Add(info);
            }

            return consulta;
        }

        #endregion
    }
}