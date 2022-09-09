using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.Controles.Suscripcion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class EditarSuscripcionComunidadController : ControllerBaseWeb
    {
        private Suscripcion mSuscripcion;
        private GestionTesauro mGestorTesauro;

        public EditarSuscripcionComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        public ActionResult Index()
        {
            EditSuscriptionViewModel paginaModel = new EditSuscriptionViewModel();

            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            List<CategoryModel> categoriasTesauro = CargarTesauroProyecto(UtilIdiomas.LanguageCode);

            bool notificacionesActivas = false;
            List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones)).ToList();
            if (ParametroProyecto.ContainsKey(ParametroAD.EnviarNotificacionesDeSuscripciones))
            {
                notificacionesActivas = ParametroProyecto[ParametroAD.EnviarNotificacionesDeSuscripciones] == "true";
            }
            //else if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro='" + TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones + "'").Length > 0)

            else if (busqueda.Count > 0)
            {
                notificacionesActivas = busqueda.First().Valor.Equals("true");
            }

            paginaModel.ActiveNotifications = notificacionesActivas;

            if (notificacionesActivas)
            {
                paginaModel.ReceiveSubscription = false;
                paginaModel.DailySubscription = false;
                paginaModel.WeeklySubscription = false;
            }

            if (Suscripcion != null && Suscripcion.FilasCategoriasVinculadas != null)
            {
                foreach (CategoryModel cat in categoriasTesauro)
                {
                    cat.Selected = Suscripcion.GestorSuscripcion.SuscripcionDW.ListaCategoriaTesVinSuscrip.Any(filaCat => filaCat.CategoriaTesauroID == cat.Key);
                }

                if (notificacionesActivas)
                {
                    if (Suscripcion.FilaSuscripcion.Periodicidad > 0)
                    {
                        paginaModel.ReceiveSubscription = true;
                        paginaModel.DailySubscription = Suscripcion.FilaSuscripcion.Periodicidad == 1;
                        paginaModel.WeeklySubscription = Suscripcion.FilaSuscripcion.Periodicidad == 7;
                    }
                }
            }

            paginaModel.Categories = categoriasTesauro;

            return View(paginaModel);
        }

        public ActionResult Modal()
        {
            EditSuscriptionViewModel paginaModel = new EditSuscriptionViewModel();

            if (UsuarioActual.EsIdentidadInvitada)
            {
                return new RedirectResult(GnossUrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));

            }

            List<CategoryModel> categoriasTesauro = CargarTesauroProyecto(UtilIdiomas.LanguageCode);

            bool notificacionesActivas = false;
            List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones)).ToList();
            if (ParametroProyecto.ContainsKey(ParametroAD.EnviarNotificacionesDeSuscripciones))
            {
                notificacionesActivas = ParametroProyecto[ParametroAD.EnviarNotificacionesDeSuscripciones] == "true";
            }
            //else if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro='" + TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones + "'").Length > 0)

            else if (busqueda.Count > 0)
            {
                notificacionesActivas = busqueda.First().Valor.Equals("true");
            }

            paginaModel.ActiveNotifications = notificacionesActivas;

            if (notificacionesActivas)
            {
                paginaModel.ReceiveSubscription = false;
                paginaModel.DailySubscription = false;
                paginaModel.WeeklySubscription = false;
            }

            if (Suscripcion != null && Suscripcion.FilasCategoriasVinculadas != null)
            {
                foreach (CategoryModel cat in categoriasTesauro)
                {
                    cat.Selected = Suscripcion.GestorSuscripcion.SuscripcionDW.ListaCategoriaTesVinSuscrip.Any(filaCat => filaCat.CategoriaTesauroID == cat.Key);
                }

                if (notificacionesActivas && Suscripcion.FilaSuscripcion.Periodicidad > 0)
                {
                    paginaModel.ReceiveSubscription = true;
                    paginaModel.DailySubscription = Suscripcion.FilaSuscripcion.Periodicidad == 1;
                    paginaModel.WeeklySubscription = Suscripcion.FilaSuscripcion.Periodicidad == 7;
                }
            }

            paginaModel.Categories = categoriasTesauro;

            return PartialView("_subscribe-community-categories", paginaModel);
        }


        /// <summary>
        /// Se produce al pulsar el enlace 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CrearSuscripcion()
        {
            SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ControladorSuscripciones.CrearSuscripcionTesauroProyecto(IdentidadActual.GestorIdentidades.GestorSuscripciones, IdentidadActual.Clave, GestorTesauro.TesauroActualID, IdentidadActual.FilaIdentidad.OrganizacionID, ProyectoSeleccionado.Clave, PeriodicidadSuscripcion.Diaria);
            mSuscripcion = IdentidadActual.GestorIdentidades.GestorSuscripciones.ObtenerSuscripcionAProyecto(ProyectoSeleccionado.Clave);

            //TODO Alberto: Incorporo esta línea porque al suscribirse a las novedades de una comunidad, en numerosas ocasiones esta línea venía vacía... =S
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperPersona dataWrapperPersona = personaCN.ObtenerPersonaPorID(IdentidadActual.PersonaID.Value);
            GestionPersonas gestorPersona = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
            if (!dataWrapperPersona.ListaConfigGnossPersona.Any())
            {
                gestorPersona.AgregarConfiguracionGnossPersona(IdentidadActual.PersonaID.Value);
            }
            //IdentidadActual.GestorIdentidades.GestorPersonas.AgregarConfiguracionGnossPersona(IdentidadActual.PersonaID.Value);

            suscCN.ActualizarSuscripcion();
            suscCN.Dispose();
        }

        [HttpPost]
        public ActionResult GuardarCambios(EditSuscriptionViewModel editarSuscripcion)
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }
            bool error = false;
            try
            {
                string categoriasSeleccionadas = "";
                if (!string.IsNullOrEmpty(editarSuscripcion.SelectedCategories))
                {
                    categoriasSeleccionadas = editarSuscripcion.SelectedCategories;
                }

                string[] categorias = categoriasSeleccionadas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<Guid> listacategorias = new List<Guid>();
                foreach (string cat in categorias)
                {
                    listacategorias.Add(new Guid(cat));
                }

                if (Suscripcion == null)
                {
                    CrearSuscripcion();
                }

                if (Suscripcion.FilasCategoriasVinculadas != null)
                {
                    foreach (AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip filaCat in mSuscripcion.FilasCategoriasVinculadas.ToList())
                    {
                        if (!listacategorias.Contains(filaCat.CategoriaTesauroID))
                        {
                            //Si tengo una categoría que no está en el selector la quito del DW
                            mSuscripcion.FilasCategoriasVinculadas.Remove(filaCat);
                            mEntityContext.EliminarElemento(filaCat);
                        }
                        else
                        {
                            if (!listacategorias.Contains(filaCat.CategoriaTesauroID))
                            {
                                //Si la categoría está en el selector, en el DS y no está seleccionada, la elimino de la lista para dejar sólo las añadidas
                                listacategorias.Remove(filaCat.CategoriaTesauroID);
                            }
                        }
                    }
                }

                foreach (Guid catID in listacategorias)
                {
                    //Ya sólo quedan categorías añadidas, así que añado las filas
                    Suscripcion.GestorSuscripcion.VincularCategoria(Suscripcion, GestorTesauro.ListaCategoriasTesauro[catID]);
                }

                if (editarSuscripcion.ReceiveSubscription)
                {
                    if (editarSuscripcion.DailySubscription)
                    {
                        mSuscripcion.FilaSuscripcion.Periodicidad = (short)PeriodicidadSuscripcion.Diaria;
                    }
                    else if (editarSuscripcion.WeeklySubscription)
                    {
                        mSuscripcion.FilaSuscripcion.Periodicidad = (short)PeriodicidadSuscripcion.Semanal;
                    }
                }
                else
                {
                    mSuscripcion.FilaSuscripcion.Periodicidad = (short)PeriodicidadSuscripcion.NoEnviar;
                }

                if (listacategorias.Count == 0)
                {
                    mSuscripcion.GestorSuscripcion.EliminarSuscripcion(mSuscripcion.Clave);
                }

                SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                suscCN.ActualizarSuscripcion();
                suscCN.Dispose();

                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                controladorPersonas.ActivoEnComunidad(IdentidadActual);

                ControladorIdentidades.AccionEnServicioExternoProyecto(TipoAccionExterna.Edicion, IdentidadActual.Persona, ProyectoSeleccionado.Clave, IdentidadActual.Clave, "", "", IdentidadActual.FilaIdentidad.FechaAlta, null);
            }
            catch (Exception)
            {
                error = true;
            }

            if (error)
            {
                return GnossResultERROR(UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }
            else
            {
                return GnossResultOK(UtilIdiomas.GetText("COMMON", "CAMBIOSCORRECTOS"));
            }
        }

        public Suscripcion Suscripcion
        {
            get
            {
                if (mSuscripcion == null)
                {
                    SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperSuscripcion suscDW = suscCN.ObtenerSuscripcionesDePerfil(IdentidadActual.FilaIdentidad.PerfilID, false);
                    IdentidadActual.GestorIdentidades.GestorSuscripciones = new GestionSuscripcion(suscDW, mLoggingService, mEntityContext);
                    suscCN.Dispose();
                    mSuscripcion = IdentidadActual.GestorIdentidades.GestorSuscripciones.ObtenerSuscripcionAProyecto(ProyectoSeleccionado.Clave);
                }
                return mSuscripcion;
            }
        }

        GestionTesauro GestorTesauro
        {
            get
            {
                if (mGestorTesauro == null)
                {
                    TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperTesauro tesDW = tesauroCL.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave);
                    tesauroCL.Dispose();
                    mGestorTesauro = new GestionTesauro(tesDW, mLoggingService, mEntityContext);
                }
                return mGestorTesauro;
            }
        }
    }
}
