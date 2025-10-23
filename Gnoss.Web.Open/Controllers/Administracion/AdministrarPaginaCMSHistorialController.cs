using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Gnoss.Web.Open.Filters;
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
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.AdministrarPaginasCMSViewModel;
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.AdministrarPaginasCMSViewModel.RowCMSModel;
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel;

namespace Gnoss.Web.Open.Controllers.Administracion
{
    public class AdministrarPaginaCMSHistorialController : ControllerAdministrationWeb
    {
        #region Miembros

        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        #region Constructores

        public AdministrarPaginaCMSHistorialController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime, IAvailableServices availableServices, ILogger<ControllerAdministrationWeb> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime, availableServices, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos Web

        [HttpPost]
		[TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.VerPagina } })]
		public ActionResult LoadHistory(Guid pPestanyaID)
        {
            UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
            ViewBag.RestaurarVersionPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.RestaurarVersionPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            ViewBag.EliminarVersionPaginaPermitido = utilPermisos.IdentidadTienePermiso((ulong)PermisoContenidos.EliminarVersionPagina, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, TipoDePermiso.Contenidos);
            AdministrarPaginaCMSHistorialViewModel modelo = CargarPaginaCMSHistorial(pPestanyaID);

            return PartialView("_history", modelo);
        }

		[TypeFilter(typeof(PermisosContenidos), Arguments = new object[] { new ulong[] { (ulong)PermisoContenidos.RestaurarVersionPagina, (ulong)PermisoContenidos.VerPagina } })]
		public ActionResult Compare(string documentosComparar, Guid pPestanyaID, bool pRestaurar = false)
        {
            string[] guids = documentosComparar.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            AdministrarPaginaCMSComparadorViewModel modelo = CargarComparadorEstructuraPaginaCMS(Guid.Parse(guids[0]), Guid.Parse(guids[1]), pPestanyaID, pRestaurar);
            modelo.Restaurando = pRestaurar;

            return PartialView("_comparator", modelo);
        }

        #endregion

        #region Metodos privados

        private AdministrarPaginaCMSHistorialViewModel CargarPaginaCMSHistorial(Guid pPestanyaID)
        {
            AdministrarPaginaCMSHistorialViewModel modelo = new AdministrarPaginaCMSHistorialViewModel();
            using (CMSCN CMSCN = new(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
            {
                List<ProyectoPestanyaVersionCMS> versionesPaginaCMS = CMSCN.ObtenerVersionesEstructuraPaginaCMSSinEstructura(pPestanyaID).OrderBy(item => item.Fecha).ToList();
                using (IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory))
                {
                    int contador = 1;
                    foreach (ProyectoPestanyaVersionCMS versionPaginaCMS in versionesPaginaCMS)
                    {
                        AdministrarPaginaCMSVersionViewModel versionPagina = new AdministrarPaginaCMSVersionViewModel(versionPaginaCMS.PestanyaID, versionPaginaCMS.VersionID, contador, versionesPaginaCMS.Last().VersionID.Equals(versionPaginaCMS.VersionID), versionPaginaCMS.Fecha, versionPaginaCMS.Comentario, identidadCN.ObtenerNombreDeIdentidad(versionPaginaCMS.IdentidadID));
                        modelo.PaginasVersiones.Add(versionPagina);
                        contador++;
                    }
                }
            }
            return modelo;
        }

        private AdministrarPaginaCMSComparadorViewModel CargarComparadorEstructuraPaginaCMS(Guid pVersion1, Guid pVersion2, Guid pComponenteID, bool pRestaurar)
        {
            AdministrarPaginaCMSComparadorViewModel modelo = new AdministrarPaginaCMSComparadorViewModel();

            using (CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
            {
                List<Guid> versionesPaginaCMS = CMSCN.ObtenerIdVersionesEstructuraPaginaCMS(pComponenteID);
                int contadorVersion = 1;
                foreach (Guid versionPaginaCMS in versionesPaginaCMS)
                {
                    if (versionPaginaCMS.Equals(pVersion1))
                    {
                        ProyectoPestanyaVersionCMS version1 = CMSCN.ObtenerVersionEstructuraPaginaCMS(pVersion1);
                        modelo.Modelo1 = new AdministrarPaginaCMSVersionViewModel(JsonConvert.DeserializeObject<AdministrarPaginasCMSViewModel>(version1.ModeloJSON), versionPaginaCMS, contadorVersion, versionesPaginaCMS.Last().Equals(pVersion1), version1.Fecha, version1.Comentario, "");
                    }
                    else if (versionPaginaCMS.Equals(pVersion2))
                    {
                        ProyectoPestanyaVersionCMS version2 = CMSCN.ObtenerVersionEstructuraPaginaCMS(pVersion2);
                        modelo.Modelo2 = new AdministrarPaginaCMSVersionViewModel(JsonConvert.DeserializeObject<AdministrarPaginasCMSViewModel>(version2.ModeloJSON), versionPaginaCMS, contadorVersion, versionesPaginaCMS.Last().Equals(pVersion2), version2.Fecha, version2.Comentario, "");
                    }
                    contadorVersion++;
                }
            }
            ComprobarNombresComponentes(modelo);
            CargarDiferencias(modelo);

            return modelo;
        }
        private void ComprobarNombresComponentes(AdministrarPaginaCMSComparadorViewModel pModelo)
        {
            pModelo.ComponentsDeleted = new Dictionary<Guid, string>();

            List<Guid> idsComponentesModelo1 = pModelo.Modelo1.PestanyaCMS.Rows.SelectMany(item => item.Cols.SelectMany(x => x.Components.Select(y => y.Key))).ToList();
            List<Guid> idsComponentesModelo2 = pModelo.Modelo2.PestanyaCMS.Rows.SelectMany(item => item.Cols.SelectMany(x => x.Components.Select(y => y.Key))).ToList();
            using (CMSCN cmscn = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory))
            {
                Dictionary<Guid, string> nombresActualesComponentes1 = cmscn.ObtenerNombreComponentesPorIDComponente(idsComponentesModelo1);
                foreach (ComponentCMSModel componente in pModelo.Modelo1.PestanyaCMS.Rows.SelectMany(item => item.Cols.SelectMany(x => x.Components)))
                {
                    if (nombresActualesComponentes1.ContainsKey(componente.Key))
                    {
                        componente.Name = nombresActualesComponentes1[componente.Key];
                    }
                    else
                    {
                        if (!pModelo.ComponentsDeleted.ContainsKey(componente.Key))
                        {
                            pModelo.ComponentsDeleted.Add(componente.Key, componente.Name);
                        }
                    }
                }

                Dictionary<Guid, string> nombresActualesComponentes2 = cmscn.ObtenerNombreComponentesPorIDComponente(idsComponentesModelo2);
                foreach (ComponentCMSModel componente in pModelo.Modelo2.PestanyaCMS.Rows.SelectMany(item => item.Cols.SelectMany(x => x.Components)))
                {
                    if (nombresActualesComponentes2.ContainsKey(componente.Key))
                    {
                        componente.Name = nombresActualesComponentes2[componente.Key];
                    }
                    else
                    {
                        if (!pModelo.ComponentsDeleted.ContainsKey(componente.Key))
                        {
                            pModelo.ComponentsDeleted.Add(componente.Key, componente.Name);
                        }
                    }
                }
            }
        }
        private void CargarDiferencias(AdministrarPaginaCMSComparadorViewModel pModeloComparador)
        {
            List<RowCMSModel> filaModelo1 = pModeloComparador.Modelo1.PestanyaCMS.Rows;
            List<RowCMSModel> filaModelo2 = pModeloComparador.Modelo2.PestanyaCMS.Rows;

            pModeloComparador.RowsDiff = new HashSet<Guid>();
            pModeloComparador.ColsDiff = new HashSet<Guid>();
            pModeloComparador.ComponentsDiff = new HashSet<Guid>();
            pModeloComparador.AttributesDiff = new Dictionary<Guid, List<string>>();

            List<RowCMSModel> modeloMasfilas;
            List<RowCMSModel> modeloMenosfilas;

            if (pModeloComparador.Modelo1.PestanyaCMS.Rows.Count >= pModeloComparador.Modelo2.PestanyaCMS.Rows.Count)
            {
                modeloMasfilas = pModeloComparador.Modelo1.PestanyaCMS.Rows;
                modeloMenosfilas = pModeloComparador.Modelo2.PestanyaCMS.Rows;
            }
            else
            {
                modeloMasfilas = pModeloComparador.Modelo2.PestanyaCMS.Rows;
                modeloMenosfilas = pModeloComparador.Modelo1.PestanyaCMS.Rows;
            }

            for (int i = 0; i < modeloMasfilas.Count; i++)
            {
                if (i < modeloMenosfilas.Count)
                {
                    CompararFilas(pModeloComparador, modeloMasfilas[i], modeloMenosfilas[i]);
                }
                else
                {
                    MarcarFilaCompletaDistinta(pModeloComparador, modeloMasfilas[i]);
                }
            }
        }

        private void CompararFilas(AdministrarPaginaCMSComparadorViewModel pModeloComparador, RowCMSModel pFilaA, RowCMSModel pFilaB)
        {
            CompararAtributos(pModeloComparador, pFilaA, pFilaB);
            CompararColumnas(pModeloComparador, pFilaA, pFilaB);
        }

        private void CompararAtributos(AdministrarPaginaCMSComparadorViewModel pModeloComparador, RowCMSModel pFilaA, RowCMSModel pFilaB)
        {
            if (pFilaA.Attributes != pFilaB.Attributes)
            {
                Dictionary<string, string> claveValorA = pFilaA.Attributes.Split("~~~", StringSplitOptions.RemoveEmptyEntries).ToDictionary(k => k.Split("---")[0], v => v.Split("---")[1]);
                Dictionary<string, string> claveValorB = pFilaB.Attributes.Split("~~~", StringSplitOptions.RemoveEmptyEntries).ToDictionary(k => k.Split("---")[0], v => v.Split("---")[1]);

                foreach (string clave in claveValorA.Keys)
                {
                    if (!claveValorB.ContainsKey(clave))
                    {
                        if (pModeloComparador.AttributesDiff.ContainsKey(pFilaA.Key))
                        {
                            pModeloComparador.AttributesDiff[pFilaA.Key].Add(clave);
                        }
                        else
                        {
                            pModeloComparador.AttributesDiff.Add(pFilaA.Key, [clave]);
                        }
                        pModeloComparador.RowsDiff.Add(pFilaA.Key);
                        pModeloComparador.RowsDiff.Add(pFilaB.Key);
                    }
                    else if (claveValorA[clave] != claveValorB[clave])
                    {
                        if (pModeloComparador.AttributesDiff.ContainsKey(pFilaA.Key))
                        {
                            pModeloComparador.AttributesDiff[pFilaA.Key].Add(clave);
                        }
                        else
                        {
                            pModeloComparador.AttributesDiff.Add(pFilaA.Key, [clave]);
                        }
                        pModeloComparador.RowsDiff.Add(pFilaA.Key);
                        pModeloComparador.RowsDiff.Add(pFilaB.Key);
                    }
                }

                foreach (string clave in claveValorB.Keys)
                {
                    if (!claveValorA.ContainsKey(clave))
                    {
                        if (pModeloComparador.AttributesDiff.ContainsKey(pFilaB.Key))
                        {
                            pModeloComparador.AttributesDiff[pFilaB.Key].Add(clave);
                        }
                        else
                        {
                            pModeloComparador.AttributesDiff.Add(pFilaB.Key, [clave]);
                        }
                        pModeloComparador.RowsDiff.Add(pFilaA.Key);
                        pModeloComparador.RowsDiff.Add(pFilaB.Key);
                    }
                    else if (claveValorB[clave] != claveValorA[clave])
                    {
                        if (pModeloComparador.AttributesDiff.ContainsKey(pFilaB.Key))
                        {
                            pModeloComparador.AttributesDiff[pFilaB.Key].Add(clave);
                        }
                        else
                        {
                            pModeloComparador.AttributesDiff.Add(pFilaB.Key, [clave]);
                        }
                        pModeloComparador.RowsDiff.Add(pFilaA.Key);
                        pModeloComparador.RowsDiff.Add(pFilaB.Key);
                    }
                }
            }
        }

        private void CompararColumnas(AdministrarPaginaCMSComparadorViewModel pModeloComparador, RowCMSModel pFilaA, RowCMSModel pFilaB)
        {
            RowCMSModel filaMasColumnas;
            RowCMSModel filaMenosColumnas;

            if (pFilaA.Cols.Count >= pFilaB.Cols.Count)
            {
                filaMasColumnas = pFilaA;
                filaMenosColumnas = pFilaB;
            }
            else
            {
                filaMasColumnas = pFilaB;
                filaMenosColumnas = pFilaA;
            }

            for (int i = 0; i < filaMasColumnas.Cols.Count; i++)
            {
                if (i < filaMenosColumnas.Cols.Count)
                {
                    CompararTamanno(pModeloComparador, filaMasColumnas.Key, filaMasColumnas.Cols[i], filaMenosColumnas.Key, filaMenosColumnas.Cols[i]);
                    CompararComponentes(pModeloComparador, filaMasColumnas.Key, filaMasColumnas.Cols[i], filaMenosColumnas.Key, filaMenosColumnas.Cols[i]);
                }
                else
                {
                    MarcarColumnaCompletaDistinta(pModeloComparador, filaMasColumnas.Key, filaMasColumnas.Cols[i]);
                }
            }
        }

        private void CompararTamanno(AdministrarPaginaCMSComparadorViewModel pModeloComparador, Guid pFilaA, ColCMSModel pColumnaA, Guid pFilaB, ColCMSModel pColumnaB)
        {
            if (pColumnaA.Class != pColumnaB.Class)
            {
                pModeloComparador.ColsDiff.Add(pColumnaA.Key);
                pModeloComparador.ColsDiff.Add(pColumnaB.Key);
                pModeloComparador.RowsDiff.Add(pFilaA);
                pModeloComparador.RowsDiff.Add(pFilaB);
            }
        }

        private void CompararComponentes(AdministrarPaginaCMSComparadorViewModel pModeloComparador, Guid pFilaA, ColCMSModel pColumnaA, Guid pFilaB, ColCMSModel pColumnaB)
        {
            List<ComponentCMSModel> columnaMasComponentes;
            List<ComponentCMSModel> columnaMenosComponentes;

            if (pColumnaA.Components.Count >= pColumnaB.Components.Count)
            {
                columnaMasComponentes = pColumnaA.Components;
                columnaMenosComponentes = pColumnaB.Components;
            }
            else
            {
                columnaMasComponentes = pColumnaB.Components;
                columnaMenosComponentes = pColumnaA.Components;
            }

            for (int i = 0; i < columnaMasComponentes.Count; i++)
            {
                if (i < columnaMenosComponentes.Count)
                {
                    if (columnaMasComponentes[i].Key != columnaMenosComponentes[i].Key)
                    {
                        pModeloComparador.ComponentsDiff.Add(columnaMasComponentes[i].Key);
                        pModeloComparador.ComponentsDiff.Add(columnaMenosComponentes[i].Key);
                        pModeloComparador.RowsDiff.Add(pFilaA);
                        pModeloComparador.RowsDiff.Add(pFilaB);
                    }
                }
                else
                {
                    pModeloComparador.ComponentsDiff.Add(columnaMasComponentes[i].Key);
                    pModeloComparador.RowsDiff.Add(pFilaA);
                    pModeloComparador.RowsDiff.Add(pFilaB);
                }
            }
        }

        private void MarcarFilaCompletaDistinta(AdministrarPaginaCMSComparadorViewModel pModeloComparador, RowCMSModel pFila)
        {
            pModeloComparador.RowsDiff.Add(pFila.Key);

            Dictionary<string, string> claveValor = pFila.Attributes.Split("~~~", StringSplitOptions.RemoveEmptyEntries).ToDictionary(k => k.Split("---")[0], v => v.Split("---")[1]);

            foreach (string clave in claveValor.Keys)
            {
                if (pModeloComparador.AttributesDiff.ContainsKey(pFila.Key))
                {
                    pModeloComparador.AttributesDiff[pFila.Key].Add(clave);
                }
                else
                {
                    pModeloComparador.AttributesDiff.Add(pFila.Key, [clave]);
                }
            }

            foreach (ColCMSModel columna in pFila.Cols)
            {
                MarcarColumnaCompletaDistinta(pModeloComparador, pFila.Key, columna);
            }
        }

        private void MarcarColumnaCompletaDistinta(AdministrarPaginaCMSComparadorViewModel pModeloComparador, Guid pFila, ColCMSModel pColumna)
        {
            pModeloComparador.RowsDiff.Add(pFila);
            pModeloComparador.ColsDiff.Add(pColumna.Key);
            foreach (ComponentCMSModel componente in pColumna.Components)
            {
                pModeloComparador.ComponentsDiff.Add(componente.Key);
            }
        }
        #endregion
    }
}
