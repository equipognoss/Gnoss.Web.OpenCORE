using AspNetCoreGeneratedDocument;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gnoss.Web.Open.Controllers.Administracion
{
    public class CMSAdminComponenteHistorialController : ControllerAdministrationWeb
    {
        #region Miembros

        private IAvailableServices mAvailableServices;

        #endregion

        #region Constructor

        public CMSAdminComponenteHistorialController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<CMSAdminComponenteHistorialController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mAvailableServices = availableServices;
        }

        #endregion

        [HttpPost]
        public ActionResult LoadHistory(string idComponente)
        {
            CMSAdminComponenteHistorialViewModel modelo = CargarComponentesHistorial(Guid.Parse(idComponente));

            return PartialView("_modal-views/_history", modelo);
        }

        public ActionResult Compare(string documentosComparar)
        {
            string[] param = documentosComparar.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            CMSAdminComparadorComponentesViewModel modelo = CargarComparadorComponetes(Guid.Parse(param[0]), Guid.Parse(param[1]), Guid.Parse(RequestParams("idComponente")));

            return PartialView("_modal-views/_comparator", modelo);
        }

        #region Métodos privados

        public CMSAdminComponenteHistorialViewModel CargarComponentesHistorial(Guid pComponenteID)
        {
            CMSAdminComponenteHistorialViewModel componentes = new CMSAdminComponenteHistorialViewModel();

            using (CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
            {
                List<CMSComponenteVersion> listaVersiones = CMSCN.ObtenerVersionesComponenteCMSSinModelo(pComponenteID);
                using (IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory))
                {
                    foreach (CMSComponenteVersion version in listaVersiones)
                    {
                        CMSAdminComponenteVersionViewModel componenteVersion = new CMSAdminComponenteVersionViewModel();
                        componenteVersion.ComponenteID = version.ComponenteID;
                        componenteVersion.VersionID = version.VersionID;
                        componenteVersion.Fecha = version.Fecha;
                        componenteVersion.Comentario = version.Comentario;
                        componenteVersion.Autor = identidadCN.ObtenerNombreDeIdentidad(version.IdentidadID);
                        componentes.Componentes.Add(componenteVersion);
                    }
                }
            }

            return componentes;
        }

        public CMSAdminComparadorComponentesViewModel CargarComparadorComponetes(Guid pVersion1, Guid pVersion2, Guid pComponenteID)
        {
            CMSAdminComparadorComponentesViewModel modelo = new CMSAdminComparadorComponentesViewModel();
            modelo.Restaurando = RequestParams("estaRestaurando") != null && RequestParams("estaRestaurando").Equals("true");

            using (CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
            {
                var versionesAux = CMSCN.ObtenerVersionesComponenteCMS(pComponenteID).OrderBy(item => item.Fecha).ToList();
                int version = 1;
                foreach (var versionAux in versionesAux)
                {
                    if (versionAux.VersionID.Equals(pVersion1))
                    {
                        modelo.ComponenteVersion1.ComponenteID = versionAux.ComponenteID;
                        modelo.ComponenteVersion1.VersionID = versionAux.VersionID;
                        modelo.ComponenteVersion1.Version = version;
                        modelo.ComponenteVersion1.VersionActual = versionesAux.Last().VersionID.Equals(pVersion1);
                        modelo.ComponenteVersion1.Fecha = versionAux.Fecha;
                        modelo.ComponenteVersion1.Propiedades = JsonConvert.DeserializeObject<CMSAdminComponenteEditarViewModel>(versionAux.ModeloJSON);
                    }
                    else if (versionAux.VersionID.Equals(pVersion2))
                    {
                        modelo.ComponenteVersion2.ComponenteID = versionAux.ComponenteID;
                        modelo.ComponenteVersion2.VersionID = versionAux.VersionID;
                        modelo.ComponenteVersion2.Version = version;
                        modelo.ComponenteVersion2.VersionActual = versionesAux.Last().VersionID.Equals(pVersion2);
                        modelo.ComponenteVersion2.Fecha = versionAux.Fecha;
                        modelo.ComponenteVersion2.Propiedades = JsonConvert.DeserializeObject<CMSAdminComponenteEditarViewModel>(versionAux.ModeloJSON);
                    }
                    version++;
                }

            }

            modelo.DiferenciasPropiedadesCMS = CompararPropiedadesCMS(modelo.ComponenteVersion1.Propiedades, modelo.ComponenteVersion2.Propiedades);
            modelo.DiferenciasCamposComunesCMS = ComprobarCamposComunesCMS(modelo.ComponenteVersion1.Propiedades, modelo.ComponenteVersion2.Propiedades);

            return modelo;
        }

        public Dictionary<TipoPropiedadCMS, Dictionary<string, bool>> CompararPropiedadesCMS(CMSAdminComponenteEditarViewModel pModelo1, CMSAdminComponenteEditarViewModel pModelo2)
        {
            Dictionary<TipoPropiedadCMS, Dictionary<string, bool>> resultado = new Dictionary<TipoPropiedadCMS, Dictionary<string, bool>>();

            foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in pModelo1.Properties)
            {
                CMSAdminComponenteEditarViewModel.PropiedadComponente propiedadAux = pModelo2.Properties.Where(item => item.TipoPropiedadCMS == propiedad.TipoPropiedadCMS).FirstOrDefault();
                if (pModelo1.ListaIdiomas.Count > 1)
                {
                    Dictionary<string, bool> diccAux = new Dictionary<string, bool>();
                    if (propiedad.MultiLang)
                    {
                        if (propiedad.TipoPropiedadCMS.Equals(TipoPropiedadCMS.ListaOpcionesMenu) || propiedad.TipoPropiedadCMS.Equals(TipoPropiedadCMS.ListaCamposEnvioCorreo))
                        {
                            List<string> options = propiedad.Value.Split("###").ToList();
                            List<string> optionsAux = propiedadAux.Value.Split("###").ToList();
                            if (options.Count != optionsAux.Count)
                            {
                                foreach (string lang in pModelo1.ListaIdiomas.Keys)
                                {
                                    diccAux.Add(lang, true);
                                }
                            }
                            else
                            {
                                int cont = 0;
                                foreach (string option in options)
                                {
                                    string[] data = option.Split("&&&");
                                    string[] dataAux = optionsAux[cont].Split("&&&");
                                    ComprobarCamposListas(data, dataAux, diccAux, propiedad.TipoPropiedadCMS, pModelo1.ListaIdiomas);
                                    cont++;
                                }
                            }
                        }
                        else
                        {
                            foreach (string lang in pModelo1.ListaIdiomas.Keys)
                            {
                                string val = UtilCadenas.ObtenerTextoDeIdioma(propiedad.Value, lang, null, true) ?? "";
                                string valAux = UtilCadenas.ObtenerTextoDeIdioma(propiedadAux.Value, lang, null, true) ?? "";
                                bool result = !val.Equals(valAux);
                                diccAux.Add(lang, result);
                            }
                        }
                        resultado.Add(propiedad.TipoPropiedadCMS, diccAux);
                    }
                    else
                    {
                        diccAux.Add("", !propiedad.Value.Equals(propiedadAux.Value));
                        resultado.Add(propiedad.TipoPropiedadCMS, diccAux);
                    }
                }
                else
                {
                    resultado.Add(propiedad.TipoPropiedadCMS, new Dictionary<string, bool> { { pModelo1.IdiomaPorDefecto, !propiedad.Value.Equals(propiedadAux.Value) } });
                }
            }

            return resultado;
        }

        private Dictionary<string, bool> ComprobarCamposComunesCMS(CMSAdminComponenteEditarViewModel pModelo1, CMSAdminComponenteEditarViewModel pModelo2)
        {
            Dictionary<string, bool> resultado = new Dictionary<string, bool>();

            resultado.Add("privacy", pModelo1.Private != pModelo2.Private);
            resultado.Add("name", !pModelo1.Name.Equals(pModelo2.Name));
            resultado.Add("shortName", !pModelo1.ShortName.Equals(pModelo2.ShortName));
            resultado.Add("active", pModelo1.Active != pModelo2.Active);
            resultado.Add("accessType", pModelo1.AccesoPublicoComponente != pModelo2.AccesoPublicoComponente);
            resultado.Add("caducidades", pModelo1.Caducidades.Where(item => item.Value).Select(item => item.Key).FirstOrDefault() != pModelo2.Caducidades.Where(item => item.Value).Select(item => item.Key).FirstOrDefault());
            resultado.Add("styles", !pModelo1.Styles.Equals(pModelo2.Styles));

            return resultado;
        }

        private void ComprobarCamposListas(string[] pDataModel1, string[] pDataModel2, Dictionary<string, bool> pDicc, TipoPropiedadCMS pTipoPropiedad, Dictionary<string, string> pIdiomas)
        {
            foreach (string lang in pIdiomas.Keys)
            {
                if (pTipoPropiedad.Equals(TipoPropiedadCMS.ListaCamposEnvioCorreo))
                {
                    string nombre1 = UtilCadenas.ObtenerTextoDeIdioma(pDataModel1[0], lang, null, true) ?? "";
                    bool obligatorio1 = bool.Parse(pDataModel1[1]);
                    string tipoCampo1 = pDataModel1[2];

                    string nombre2 = UtilCadenas.ObtenerTextoDeIdioma(pDataModel2[0], lang, null, true) ?? "";
                    bool obligatorio2 = bool.Parse(pDataModel2[1]);
                    string tipoCampo2 = pDataModel2[2];

                    bool result = !nombre1.Equals(nombre2) || obligatorio1 != obligatorio2 || !tipoCampo1.Equals(tipoCampo2);
                    pDicc.Add(lang, result);
                }
                else if (pTipoPropiedad.Equals(TipoPropiedadCMS.ListaOpcionesMenu))
                {
                    string nombre1 = UtilCadenas.ObtenerTextoDeIdioma(pDataModel1[0], lang, null, true) ?? "";
                    string enlace1 = UtilCadenas.ObtenerTextoDeIdioma(pDataModel1[1], lang, null, true) ?? "";
                    string nombre2 = UtilCadenas.ObtenerTextoDeIdioma(pDataModel2[0], lang, null, true) ?? "";
                    string enlace2 = UtilCadenas.ObtenerTextoDeIdioma(pDataModel2[1], lang, null, true) ?? "";

                    bool result = !nombre1.Equals(nombre2) || !enlace1.Equals(enlace2);
                    pDicc.Add(lang, result);
                }
            }

        }

        #endregion
    }
}
