using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
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
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Controlador para administrar el registro por pasos
    /// </summary>
    public class AdministrarRegistroPorPasosController : ControllerBaseWeb
    {
        public AdministrarRegistroPorPasosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Miembros

        private AministrarRegistroPorPasosViewModel mPaginaModel = null;

        #endregion

        #region Métodos de evento

        /// <summary>
        /// Index
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            mPaginaModel = CargarModelo();
            return View(mPaginaModel);
        }
        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(List<PasoRegistroModel> ListaPestanyas)
        {
            GuardarLogAuditoria();
            try
            {              
                List<string> listaRegistros = new List<string>();
                List<bool> listaBooleanos = new List<bool>();
                foreach (PasoRegistroModel pasoRegistro in ListaPestanyas)
                {
                    listaRegistros.Add(pasoRegistro.NombrePasoRegistro);
                    listaBooleanos.Add(pasoRegistro.Obligatorio);
                } 
                               
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<ProyectoPestanyaMenu> listaProyectoPestanya = proyCN.ListaPestanyasMenuRegistro(ProyectoSeleccionado.Clave);
                
                for(int i = 0; i < ListaPestanyas.Count; i++)
                {
                    PasoRegistroModel paso = ListaPestanyas[i];
                    ProyectoPestanyaMenu menu = listaProyectoPestanya.FirstOrDefault(item => item.Nombre.Contains(paso.NombrePasoRegistro));
                    if (!paso.NombrePasoRegistro.Equals("Datos") && !paso.NombrePasoRegistro.Equals("Preferencias") && !paso.NombrePasoRegistro.Equals("Conecta"))
                    {                        
                        if(menu != null)
                        {
                            string rutaIdioma = UtilCadenas.ObtenerTextoDeIdioma(menu.Ruta, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                            paso.NombrePasoRegistro = rutaIdioma;
                        }
                        else
                        {
                            return GnossResultERROR("No es posible guardar un paso que no corresponda a una pestaña del menú");                            
                        }
                    }                 
                }          
                bool resultado = true;
                if(listaBooleanos.Count > 0 && !listaBooleanos[0])
                {
                    foreach(bool b in listaBooleanos)
                    {
                        if (b)
                        {
                            resultado = false;
                            break;
                        }
                    }
                }
                else if(listaBooleanos.Count > 0)
                {
                    for(int i = 0; i<listaBooleanos.Count && resultado; i++)
                    {
                        if (!listaBooleanos[i])
                        {
                            for (int j = i; j < listaBooleanos.Count && resultado; j++)
                            {
                                if (listaBooleanos[j])
                                {
                                    resultado = false;
                                }
                            }
                        }
                    }
                }
                if (resultado)
                {
                    proyCN.GuardarRegistroPorPasos(ProyectoSeleccionado.Clave, ListaPestanyas);
                }
                else
                {
                    throw new Exception("No se cumple la regla de obligatoriedad");
                }

                return GnossResultOK();
            }
            catch (Exception)
            {
                return GnossResultERROR("No es posible que haya un paso NO obligatorio por encima de uno obligatorio");
                throw;
            }
        }
        #endregion

        #region Métodos publicos

        /// <summary>
        /// Método que carga el modelo de la página
        /// </summary>
        /// <returns>El modelo para la carga de los datos</returns>
        public AministrarRegistroPorPasosViewModel CargarModelo()
        {
            AministrarRegistroPorPasosViewModel adRegistro = new AministrarRegistroPorPasosViewModel();
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<PasoRegistroModel> listaAPasar = new List<PasoRegistroModel>();
            List<string> listaNombres = proyCN.ObtenerProyectoPasoRegistro(ProyectoSeleccionado.Clave);
            List<bool> listaObligatoriedad = proyCN.ObtenerListaObligatoriedadRegistros(ProyectoSeleccionado.Clave);
            List<ProyectoPestanyaMenu> listaPestanyas = new List<ProyectoPestanyaMenu>();
            listaPestanyas = proyCN.ListaPestanyasMenuRegistro(ProyectoSeleccionado.Clave);

            for (int i = 0; i < listaNombres.Count; i++)
            {
                ProyectoPestanyaMenu pest = listaPestanyas.FirstOrDefault(item => item.Ruta.Equals(listaNombres[i]));
                if(pest != null)
                {
                    string nombre = UtilCadenas.ObtenerTextoDeIdioma(pest.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    listaAPasar.Add(new PasoRegistroModel(nombre, listaObligatoriedad[i]));
                }
                else
                {
                    listaAPasar.Add(new PasoRegistroModel(listaNombres[i], listaObligatoriedad[i]));
                }
            }

            adRegistro.ListaServicios = listaAPasar;
            
            List<string> listaNombresPestanyas = new List<string>();
            foreach(ProyectoPestanyaMenu pes in listaPestanyas)
            {
                string NombreIdioma = UtilCadenas.ObtenerTextoDeIdioma(pes.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                listaNombresPestanyas.Add(NombreIdioma);
            }
            adRegistro.ListaPestanyasMenu = listaNombresPestanyas;            
            return adRegistro;

        }

        #endregion
    }
}