using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.AdministrarTraducciones;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// Vista que permite ver todas las traducciones.
    /// </summary>
    public class AdministrarTraduccionesController : ControllerBaseWeb
    {
        public AdministrarTraduccionesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        /// <summary>
        /// Permite ver todas las traducciones.
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Texto, "AdministracionVistasPermitido" })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            AdministrarTraduccionesViewModel modelo = new AdministrarTraduccionesViewModel();

            string baseUrl = $"{BaseURLIdioma}/{UtilIdiomas.GetText("URLSEM", "ADMINISTRARTRADUCCIONESECOSISTEMA")}/";
            if (!EsAdministracionEcosistema)
            {
                baseUrl = $"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto)}/{UtilIdiomas.GetText("URLSEM", "ADMINISTRARTRADUCCIONES")}/";
            }

            modelo.URLActionCrearTexto = $"{baseUrl}crear";
            modelo.URLActionEditarTexto = $"{baseUrl}editar";
            modelo.URLActionCrearEntradas = $"{baseUrl}crearentradas";
            modelo.URLActionEliminarEntradas = $"{baseUrl}eliminarentradas";

            ParametroGeneralCN paramGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<string, string> idiomas = mConfigService.ObtenerListaIdiomasDictionary();
            List<TextoTraducidoModel> listaTextos = new List<TextoTraducidoModel>();

            GestorParametroGeneral textosPersonalizadosDS = new GestorParametroGeneral();
            if (EsAdministracionEcosistema)
            {
                ViewBag.PersonalizacionAdministracionEcosistema = mControladorBase.PersonalizacionEcosistemaID;
                textosPersonalizadosDS.ListaTextosPersonalizadosPersonalizacion = paramGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
            }
            else
            {
                textosPersonalizadosDS.ListaTextosPersonalizadosPersonalizacion = paramGeneralCN.ObtenerTextosPersonalizacionProyecto(ProyectoSeleccionado.Clave);
            }

            paramGeneralCN.Dispose();

            var textosAgrupados = textosPersonalizadosDS.ListaTextosPersonalizadosPersonalizacion.GroupBy(fila => fila.TextoID);
            foreach (var grupo in textosAgrupados)
            {
                TextoTraducidoModel modeloTraduccion = new TextoTraducidoModel(idiomas);
                modeloTraduccion.TextoID = grupo.Key;
                foreach (TextosPersonalizadosPersonalizacion filaTextoPersonalizado in grupo)
                {
                    modeloTraduccion.AgregarTraduccion(filaTextoPersonalizado.Language, filaTextoPersonalizado.Texto);
                }
                listaTextos.Add(modeloTraduccion);
            }
            modelo.Textos = listaTextos;
            return View(modelo);
        }

        /// <summary>
        /// Permite introducir los datos para crear una nueva traducción
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Texto, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Crear()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            Dictionary<string, string> idiomas = mConfigService.ObtenerListaIdiomasDictionary();
            TextoTraducidoModel modelo = new TextoTraducidoModel(idiomas);
            return View("Editar", modelo);
        }

        /// <summary>
        /// Guarda los datos de una nueva traducción.
        /// </summary>
        /// <param name="pModelo">Modelo con los datos de la nueva traduccion</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Texto, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Crear(TextoTraducidoModel pModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    GuardarTraduccion(pModelo, false);
                }
                catch (Exception ex)
                {
                    GuardarLogError(mLoggingService.DevolverCadenaError(ex, ""));
                    pModelo.Idiomas = mConfigService.ObtenerListaIdiomasDictionary();
                    if (ex.Message.Equals("Contacte con el administrador del Proyecto, no es posible atender la petición."))
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error al guardar la traducción, inténtelo de nuevo");
                    }

                    EliminarPersonalizacionVistas();
                    CargarPermisosAdministracionComunidadEnViewBag();
                    return View("Editar", pModelo);
                }
                string urlRedireccion = Request.Path.ToString().ToLower();
                urlRedireccion = urlRedireccion.Substring(0, urlRedireccion.LastIndexOf("/crear"));
                return Redirect(urlRedireccion);
            }
            pModelo.Idiomas = mConfigService.ObtenerListaIdiomasDictionary();
            return View("Editar", pModelo);
        }

        /// <summary>
        /// Permite editar los datos de una traducción.
        /// </summary>
        /// <param name="pTextoId">Id del texto a editar</param>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Texto, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Editar(string pTextoId)
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            Dictionary<string, string> idiomas = mConfigService.ObtenerListaIdiomasDictionary();
            TextoTraducidoModel modelo = new TextoTraducidoModel(idiomas);
            var data = Convert.FromBase64String(pTextoId);
            pTextoId = TextoTraducidoModel.GetString(data);
           
            ParametroGeneralCN paramGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestorParametroGeneral gestorParametoGeneral = new GestorParametroGeneral();
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);

            if (EsAdministracionEcosistema)
            {
                gestorParametoGeneral.ListaTextosPersonalizadosPersonalizacion = paramGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
            }
            else
            {
                gestorParametoGeneral.ListaTextosPersonalizadosPersonalizacion = paramGeneralCN.ObtenerTextosPersonalizacionProyecto(ProyectoSeleccionado.Clave);
            }

            var textosAgrupados = gestorParametoGeneral.ListaTextosPersonalizadosPersonalizacion.Where(fila => fila.TextoID == pTextoId).GroupBy(fila => fila.TextoID);
            modelo.TextoID = pTextoId;
            foreach (var grupo in textosAgrupados)
            {
                foreach (TextosPersonalizadosPersonalizacion filaTextoPersonalizado in grupo)
                {
                    modelo.AgregarTraduccion(filaTextoPersonalizado.Language, filaTextoPersonalizado.Texto);
                }
            }

            return View(modelo);
        }

        /// <summary>
        /// Guarda los datos de la traducción editada.
        /// </summary>
        /// <param name="pModelo">Datos de la traducción editada</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Texto, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult Editar(TextoTraducidoModel pModelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    GuardarTraduccion(pModelo, true);
                }
                catch (Exception ex)
                {
                    GuardarLogError(mLoggingService.DevolverCadenaError(ex, ""));
                    pModelo.Idiomas = mConfigService.ObtenerListaIdiomasDictionary();
                    ModelState.AddModelError(string.Empty, "Error al guardar la traducción, inténtelo de nuevo");
                    return View("Editar", pModelo);
                }
                string urlRedireccion = Request.Path.ToString().ToLower();
                urlRedireccion = urlRedireccion.Substring(0, urlRedireccion.LastIndexOf("/editar"));
                return Redirect(urlRedireccion);
            }
            pModelo.Idiomas = mConfigService.ObtenerListaIdiomasDictionary();
            return View("Editar", pModelo);
        }


        /// <summary>
        /// Crea las entradas que están en las vistas pero no existen en la BBDD
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Texto, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult CrearEntradas()
        {
            //Recorremos todas las vistas en BBDD 
            VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperVistaVirtual vistaVirtualDW = new DataWrapperVistaVirtual();
            if (EsAdministracionEcosistema)
            {
                //Estamos en un ecosistema
                vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorEcosistemaID(mControladorBase.PersonalizacionEcosistemaID);
            }
            else
            {
                //Estamos en un proyecto
                vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave);
            }
            vistaVirtualCN.Dispose();

            List<string> listaEntradasTexto = new List<string>();

            foreach (VistaVirtual vistaVirtual in vistaVirtualDW.ListaVistaVirtual)
            {
                List<string> listaObtenida = BuscarTextoTraducir(vistaVirtual.HTML);
                foreach (string entradaObtenida in listaObtenida)
                {
                    if (!listaEntradasTexto.Contains(entradaObtenida))
                    {
                        listaEntradasTexto.Add(entradaObtenida);
                    }
                }
            }

            foreach (VistaVirtualRecursos vistaVirtual in vistaVirtualDW.ListaVistaVirtualRecursos)
            {
                List<string> listaObtenida = BuscarTextoTraducir(vistaVirtual.HTML);
                foreach (string entradaObtenida in listaObtenida)
                {
                    if (!listaEntradasTexto.Contains(entradaObtenida))
                    {
                        listaEntradasTexto.Add(entradaObtenida);
                    }
                }
            }

            foreach (VistaVirtualCMS vistaVirtual in vistaVirtualDW.ListaVistaVirtualCMS)
            {
                List<string> listaObtenida = BuscarTextoTraducir(vistaVirtual.HTML);
                foreach (string entradaObtenida in listaObtenida)
                {
                    if (!listaEntradasTexto.Contains(entradaObtenida))
                    {
                        listaEntradasTexto.Add(entradaObtenida);
                    }
                }
            }

            foreach (VistaVirtualGadgetRecursos vistaVirtual in vistaVirtualDW.ListaVistaVirtualGadgetRecursos)
            {
                List<string> listaObtenida = BuscarTextoTraducir(vistaVirtual.HTML);
                foreach (string entradaObtenida in listaObtenida)
                {
                    if (!listaEntradasTexto.Contains(entradaObtenida))
                    {
                        listaEntradasTexto.Add(entradaObtenida);
                    }
                }
            }

            if (EsAdministracionEcosistema)
            {
                ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestorParametroGeneral gestorParametroGeneral = new GestorParametroGeneral();
                gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = paramCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
                ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                foreach (string entradaObtenida in listaEntradasTexto)
                {
                    if (gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Where(textoPersonalizadoPers => textoPersonalizadoPers.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID) && textoPersonalizadoPers.TextoID.Equals(entradaObtenida)).ToList().Count == 0)
                    {
                        TextosPersonalizadosPersonalizacion textoPersonalizadoPersonalizacion = new TextosPersonalizadosPersonalizacion(mControladorBase.PersonalizacionEcosistemaID, entradaObtenida, "es", entradaObtenida);
                        gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Add(textoPersonalizadoPersonalizacion);
                        gestorController.AddTextosPersonalizadosPersonalizacion(textoPersonalizadoPersonalizacion);
                    }
                }

                paramCN.ActualizarParametrosGenerales();
                paramCN.Dispose();


                ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramGeneralCL.InvalidarCacheTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
                paramGeneralCL.Dispose();
            }
            else
            {
                if (vistaVirtualDW.ListaVistaVirtualProyecto.Count > 0)
                {
                    ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestorParametroGeneral gestorParametroGeneral = new GestorParametroGeneral();
                    gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = paramCN.ObtenerTextosPersonalizacionProyecto(mControladorBase.PersonalizacionEcosistemaID);
                    ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                    Guid personalizacionID = ((VistaVirtualProyecto)vistaVirtualDW.ListaVistaVirtualProyecto.Where(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault()).PersonalizacionID;

                    foreach (string entradaObtenida in listaEntradasTexto)
                    {
                        if (gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Where(textoPersonalizadoPersonalizacion => textoPersonalizadoPersonalizacion.PersonalizacionID.Equals(personalizacionID) && textoPersonalizadoPersonalizacion.TextoID.Equals(entradaObtenida)).ToList().Count == 0)
                        {
                            TextosPersonalizadosPersonalizacion textoPersonalizadoPersonalizacion = new TextosPersonalizadosPersonalizacion(personalizacionID, entradaObtenida, "es", entradaObtenida);
                            gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Add(textoPersonalizadoPersonalizacion);
                            gestorController.AddTextosPersonalizadosPersonalizacion(textoPersonalizadoPersonalizacion);
                        }
                    }

                    paramCN.ActualizarParametrosGenerales();
                    paramCN.Dispose();

                    ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paramGeneralCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
                    paramGeneralCL.Dispose();
                }
            }

            return new RedirectResult($"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto)}/{UtilIdiomas.GetText("URLSEM", "ADMINISTRARTRADUCCIONES")}");
        }

        /// <summary>
        /// Elimina las entradas que están en la  BBDD pero no en la vista
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Texto, "AdministracionVistasPermitido" })]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        public ActionResult EliminarEntradas()
        {
            bool iniciado = false;
            try
            {
                iniciado = HayIntegracionContinua;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
                return GnossResultERROR("Contacte con el administrador del Proyecto, no es posible atender la petición.");
            }

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            try
            {
                mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);

                //Recorremos todas las vistas en BBDD 
                VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperVistaVirtual vistaVirtualDW = new DataWrapperVistaVirtual();
                if (EsAdministracionEcosistema)
                {
                    //Estamos en un ecosistema
                    vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorEcosistemaID(mControladorBase.PersonalizacionEcosistemaID);
                }
                else
                {
                    //Estamos en un proyecto
                    vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave);
                }
                vistaVirtualCN.Dispose();

                List<string> listaEntradasTexto = new List<string>();

                foreach (VistaVirtual vistaVirtual in vistaVirtualDW.ListaVistaVirtual)
                {
                    List<string> listaObtenida = BuscarTextoTraducir(vistaVirtual.HTML);
                    foreach (string entradaObtenida in listaObtenida)
                    {
                        if (!listaEntradasTexto.Contains(entradaObtenida))
                        {
                            listaEntradasTexto.Add(entradaObtenida);
                        }
                    }
                }

                foreach (VistaVirtualRecursos vistaVirtual in vistaVirtualDW.ListaVistaVirtualRecursos)
                {
                    List<string> listaObtenida = BuscarTextoTraducir(vistaVirtual.HTML);
                    foreach (string entradaObtenida in listaObtenida)
                    {
                        if (!listaEntradasTexto.Contains(entradaObtenida))
                        {
                            listaEntradasTexto.Add(entradaObtenida);
                        }
                    }
                }

                foreach (VistaVirtualCMS vistaVirtual in vistaVirtualDW.ListaVistaVirtualCMS)
                {
                    List<string> listaObtenida = BuscarTextoTraducir(vistaVirtual.HTML);
                    foreach (string entradaObtenida in listaObtenida)
                    {
                        if (!listaEntradasTexto.Contains(entradaObtenida))
                        {
                            listaEntradasTexto.Add(entradaObtenida);
                        }
                    }
                }

                foreach (VistaVirtualGadgetRecursos vistaVirtual in vistaVirtualDW.ListaVistaVirtualGadgetRecursos)
                {
                    List<string> listaObtenida = BuscarTextoTraducir(vistaVirtual.HTML);
                    foreach (string entradaObtenida in listaObtenida)
                    {
                        if (!listaEntradasTexto.Contains(entradaObtenida))
                        {
                            listaEntradasTexto.Add(entradaObtenida);
                        }
                    }
                }

                List<TranslatorModel> listaTraducciones = new List<TranslatorModel>();

                if (EsAdministracionEcosistema)
                {
                    ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestorParametroGeneral gestorParametroGeneral = new GestorParametroGeneral();
                    ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                    gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = paramCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
                    
                    foreach (TextosPersonalizadosPersonalizacion filaTextosPersonalizadosPersonalizacion in gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Where(textoPersonalizado => textoPersonalizado.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID)))
                    {
                        if (!listaEntradasTexto.Contains(filaTextosPersonalizadosPersonalizacion.TextoID))
                        {
                            TranslatorModel modeloTraduccion = new TranslatorModel();
                            modeloTraduccion.TextID = filaTextosPersonalizadosPersonalizacion.TextoID;
                            modeloTraduccion.Deleted = true;
                            listaTraducciones.Add(modeloTraduccion);
                            gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Remove(filaTextosPersonalizadosPersonalizacion);
                            gestorController.DeleteTextoPersonalizadoPersonalizacion(filaTextosPersonalizadosPersonalizacion);
                        }
                    }

                    paramCN.ActualizarParametrosGenerales();
                    paramCN.Dispose();


                    ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paramGeneralCL.InvalidarCacheTextosPersonalizadosPersonalizacionEcosistema(mControladorBase.PersonalizacionEcosistemaID);
                    paramGeneralCL.Dispose();
                }
                else
                {
                    if (vistaVirtualDW.ListaVistaVirtualProyecto.Count > 0)
                    {
                        ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                        GestorParametroGeneral gestorParametroGeneral = new GestorParametroGeneral();
                        gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion = paramCN.ObtenerTextosPersonalizacionProyecto(ProyectoSeleccionado.Clave);
                        Guid personalizacionID = (vistaVirtualDW.ListaVistaVirtualProyecto.Where(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault()).PersonalizacionID;

                        foreach (TextosPersonalizadosPersonalizacion filaTextosPersonalizadosPersonalizacion in gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Where(textoPersonalizado => textoPersonalizado.PersonalizacionID.Equals(personalizacionID)).ToList())
                        {
                            if (!listaEntradasTexto.Contains(filaTextosPersonalizadosPersonalizacion.TextoID))
                            {
                                TranslatorModel modeloTraduccion = new TranslatorModel();
                                modeloTraduccion.TextID = filaTextosPersonalizadosPersonalizacion.TextoID;
                                modeloTraduccion.Deleted = true;
                                listaTraducciones.Add(modeloTraduccion);
                                gestorController.DeleteTextoPersonalizadoPersonalizacion(filaTextosPersonalizadosPersonalizacion);
                                gestorParametroGeneral.ListaTextosPersonalizadosPersonalizacion.Remove(filaTextosPersonalizadosPersonalizacion);
                            }
                        }

                        paramCN.ActualizarParametrosGenerales();
                        paramCN.Dispose();

                        ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        paramGeneralCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
                        paramGeneralCL.Dispose();
                    }
                }

                if (iniciado && listaTraducciones.Count > 0)
                {
                    HttpResponseMessage resultado = InformarCambioAdministracion("Traducciones", JsonConvert.SerializeObject(listaTraducciones, Formatting.Indented));
                    if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                    }
                }

                if (transaccionIniciada)
                {
                    mEntityContext.TerminarTransaccionesPendientes(true);
                }
            }
            catch (Exception ex)
            {
                if (transaccionIniciada)
                {
                    proyAD.TerminarTransaccion(false);
                }

                return GnossResultERROR(ex.Message);
            }

            return new RedirectResult($"{mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto)}/{UtilIdiomas.GetText("URLSEM", "ADMINISTRARTRADUCCIONES")}");
        }

        /// <summary>
        /// Obtiene las entradas de texto
        /// </summary>
        /// <param name="pHtml"></param>
        /// <returns></returns>
        private List<string> BuscarTextoTraducir(string pHtml)
        {
            List<string> listaEntradas = new List<string>();
            string comandoTraduccion = "Html.Translate(\"";
            int indice = pHtml.IndexOf(comandoTraduccion, 0);
            int aux = 0;
            string texto = string.Empty;

            while (indice > -1)
            {
                aux = indice;
                indice = pHtml.IndexOf("\")", aux);
                texto = pHtml.Substring(aux, indice - aux);
                texto = texto.Replace(comandoTraduccion, "");

                int indiceTexto = 0;
                bool encontrado = false;
                while (indiceTexto > -1 && !encontrado)
                {
                    indiceTexto++;
                    indiceTexto = texto.IndexOf("\"", indiceTexto);
                    if (indiceTexto > -1)
                    {
                        if (texto[indiceTexto - 1] != '\\')
                        {
                            encontrado = true;
                        }
                    }
                }
                if (indiceTexto > -1)
                {
                    texto = texto.Substring(0, indiceTexto);
                }

                if (!listaEntradas.Contains(texto))
                {
                    listaEntradas.Add(texto);
                }

                indice = pHtml.IndexOf(comandoTraduccion, indice);
            }
            return listaEntradas;
        }

        #region Guardar en BD

        /// <summary>
        /// Guarda en BD la nueva traducción 
        /// </summary>
        /// <param name="pModelo">Datos de la nueva traducción</param>
        /// <param name="pEsEdicion">Indica si es edición o creación</param>
        private void GuardarTraduccion(TextoTraducidoModel pModelo, bool pEsEdicion)
        {

            bool iniciado = false;
            try
            {
                iniciado = HayIntegracionContinua;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Se ha comprobado que tiene la integración continua configurada y no puede acceder al API de Integración Continua.");
            }

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            try
            {
                mEntityContext.NoConfirmarTransacciones = true;
                transaccionIniciada = proyAD.IniciarTransaccion(true);

                GestorParametroGeneral textosPersonalizadosDSGuardado = new GestorParametroGeneral();
                Guid personalizacionID = Guid.Empty;
                bool vistaVirtualProyectoRowsMayorCero = false;
                try
                {
                    ParametroGeneralCN paramGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    if (EsAdministracionEcosistema)
                    {
                        personalizacionID = mControladorBase.PersonalizacionEcosistemaID;
                        textosPersonalizadosDSGuardado.ListaTextosPersonalizadosPersonalizacion = paramGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(personalizacionID);
                    }
                    else
                    {
                        using (VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication))

                        {
                            DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave, mControladorBase.PersonalizacionEcosistemaID, mControladorBase.ComunidadExcluidaPersonalizacionEcosistema);

                            vistaVirtualProyectoRowsMayorCero = vistaVirtualDW.ListaVistaVirtualProyecto.Count > 0;
                            if (vistaVirtualProyectoRowsMayorCero)
                            {
                                personalizacionID = (vistaVirtualDW.ListaVistaVirtualProyecto.Where(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault()).PersonalizacionID;
                                textosPersonalizadosDSGuardado.ListaTextosPersonalizadosPersonalizacion = paramGeneralCN.ObtenerTextosPersonalizacionProyecto(ProyectoSeleccionado.Clave);
                            }
                        }
                    }

                    if ((EsAdministracionEcosistema) || (!EsAdministracionEcosistema && vistaVirtualProyectoRowsMayorCero))
                    {
                        foreach (TraduccionModel traduccion in pModelo.Traducciones)
                        {
                            try
                            {
                                ControladorTraducciones.AlmacenarDatosTextosPersonalizados(pModelo.TextoID, pEsEdicion, textosPersonalizadosDSGuardado, personalizacionID, traduccion.Idioma, traduccion.Texto);
                            }
                            catch (ErrorEdicionNoValida)
                            {
                                ModelState.AddModelError(string.Empty, "El TextID ya existe, introduzca uno distinto.");
                                throw new Exception("El TextId que se intenta crear ya existe");
                            }
                        }
                        paramGeneralCN.ActualizarParametrosGenerales();

                        if (iniciado)
                        {
                            TranslatorModel modeloTraduccion = new TranslatorModel();
                            modeloTraduccion.Deleted = false;
                            modeloTraduccion.TextID = pModelo.TextoID;
                            modeloTraduccion.LanguagesText = new Dictionary<string, string>();
                            foreach (TraduccionModel traduccion in pModelo.Traducciones)
                            {
                                if (traduccion.Texto != null)
                                {
                                    modeloTraduccion.LanguagesText.Add(traduccion.Idioma, traduccion.Texto);
                                }
                            }

                            HttpResponseMessage resultado = InformarCambioAdministracion("Traducciones", JsonConvert.SerializeObject(new List<TranslatorModel>() { modeloTraduccion }, Formatting.Indented));

                            if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                            {
                                throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                            }
                        }
                        ControladorTraducciones.LimpiarCaches(personalizacionID, EsAdministracionEcosistema);
                    }

                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                if (transaccionIniciada)
                {
                    proyAD.TerminarTransaccion(false);
                }
                GuardarLogError(ex, "Error en Guardar Traduccion");
                throw;
            }
        }

        #endregion

    }
}
