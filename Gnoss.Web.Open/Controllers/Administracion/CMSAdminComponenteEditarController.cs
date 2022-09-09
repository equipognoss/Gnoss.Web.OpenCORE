using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    public class CMSAdminComponenteEditarController : ControllerBaseWeb
    {
        public CMSAdminComponenteEditarController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        private CMSAdminComponenteEditarViewModel mPaginaModel = null;

        private CMSComponente mCMSComponente = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            if (!ParametrosGeneralesRow.CMSDisponible)
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADGENERAL"));
            }

            return View(PaginaModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Pagina, "AdministracionPaginasPermitido" })]
        public ActionResult Guardar(CMSAdminComponenteEditarViewModel Componente)
        {
            string error = "";

            try
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

                GestionCMS gestorCMS;
                CMSComponente componenteEdicion = CMSComponente;

                if (esEdicion)
                {
                    gestorCMS = CMSComponente.GestorCMS;

                    componenteEdicion.Nombre = Componente.Name;
                    componenteEdicion.Estilos = Componente.Styles;
                    componenteEdicion.Activo = Componente.Active;
                    componenteEdicion.TipoCaducidadComponenteCMS = Componente.CaducidadSeleccionada;
                }
                else
                {
                    using (CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                    {
                        gestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    }
                        
                    componenteEdicion = gestorCMS.AgregarNuevoComponente(ProyectoSeleccionado, Componente.Name, Componente.Styles, Componente.Active, (short)TipoComponenteCMSActual, (short)Componente.CaducidadSeleccionada, new Dictionary<TipoPropiedadCMS, string>(), false);
                }

                Componente.Type = TipoComponenteCMSActual;
                // Para IC Notificacion al repositorio.
                Dictionary<TipoPropiedadCMS, bool> propiedadesComponente = UtilComponentes.PropiedadesDisponiblesPorTipoComponente[TipoComponenteCMSActual];
                if (Componente.Properties.Count > 0)
                {
                    foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in Componente.Properties)
                    {
                        propiedad.TypeComponent = TipoComponenteCMSActual;
                        CMSAdminComponenteEditarViewModel.PropiedadComponente propiedadBIS = ObtenerPropiedad(propiedad.TipoPropiedadCMS, propiedadesComponente);
                        propiedad.Required = propiedadBIS.Required;
                        propiedad.MultiLang = propiedadBIS.MultiLang;
                    }
                }

                Componente.Caducidades = new Dictionary<TipoCaducidadComponenteCMS, bool>();
                if (UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponenteCMSActual].Count > 1)
                {
                    foreach (TipoCaducidadComponenteCMS caducidad in UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponenteCMSActual])
                    {
                        bool selected = false;

                        if (CMSComponente != null && CMSComponente.TipoCaducidadComponenteCMS == caducidad)
                        {
                            selected = true;
                            Componente.CaducidadSeleccionada = caducidad;
                        }

                        Componente.Caducidades.Add(caducidad, selected);
                    }
                }

                if (Componente.Styles == null)
                {
                    Componente.Styles = "";
                }
                



                if (string.IsNullOrEmpty(Componente.ShortName) || Componente.ShortName.Trim().Equals(string.Empty))
                {
                    Componente.ShortName = componenteEdicion.Clave.ToString();
                }

                if (gestorCMS.CMSDW.ListaCMSComponente.FirstOrDefault(filaComponente => filaComponente.NombreCortoComponente == Componente.ShortName && filaComponente.ComponenteID != componenteEdicion.Clave) != null)
                {
                    // Si hay otro componente con el mismo nombre corto
                    error = "<p>" + UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOMBRECORTOREPETIDO") + "</p>";
                    return GnossResultERROR(error);
                }

                ControladorComponenteCMS contrComponente = new ControladorComponenteCMS(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, HayIntegracionContinua);

                contrComponente.AgregarPropiedadesComponente(componenteEdicion, Componente, gestorCMS, UrlIntragnossServicios, BaseURLContent);
                


                if (string.IsNullOrEmpty(error))
                {
                    Guid componenteIDparaRefresecar = componenteEdicion.Clave;

                    if (bloqueContenedor.HasValue)
                    {
                        gestorCMS.AgregarComponenteABloque(ProyectoSeleccionado, bloqueContenedor.Value, componenteIDparaRefresecar);
                    }

                    try
                    {
                        mEntityContext.NoConfirmarTransacciones = true;
                        transaccionIniciada = proyAD.IniciarTransaccion(true);

                        contrComponente.GuardarComponente(componenteEdicion, Componente);

                        if (iniciado)
                        {
                            foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in Componente.Properties.Where(x => x.TipoPropiedadCMS.Equals(TipoPropiedadCMS.Imagen)))
                            {

                                string valorPropiedad = HttpUtility.UrlDecode(propiedad.Value);
                                foreach (string idioma in mConfigService.ObtenerListaIdiomasDictionary().Keys)
                                {
                                    string imagenIdioma = UtilCadenas.ObtenerTextoDeIdioma(valorPropiedad, idioma, null, true);

                                    if (!string.IsNullOrEmpty(imagenIdioma))
                                    {
                                        string comienzoFile = "File:";
                                        if (imagenIdioma.StartsWith(comienzoFile))
                                        {
                                            string cadenaControl = ";Data:";

                                            string[] fichero = imagenIdioma.Split(new string[] { cadenaControl, comienzoFile }, StringSplitOptions.RemoveEmptyEntries);

                                            string nombreFichero = UtilCadenas.RemoveAccentsWithRegEx(fichero[0]);
                                            string base64Image = fichero[1];

                                            List<string> listaExtensiones = new List<string>();
                                            listaExtensiones.Add("jpg");
                                            listaExtensiones.Add("jpeg");
                                            listaExtensiones.Add("png");
                                            listaExtensiones.Add("gif");

                                            if (listaExtensiones.Contains(nombreFichero.Split('.').Last().ToLower()))
                                            {
                                                byte[] byteImage = Convert.FromBase64String(base64Image);
                                                String ruta = UtilArchivos.ContentImagenesProyectos + "/personalizacion/" + ProyectoSeleccionado.Clave.ToString().ToLower() + "/cms/";
                                                HttpResponseMessage resultadoImagen = InformarCambioAdministracionCMS("ObjetosMultimedia", Convert.ToBase64String(byteImage), nombreFichero);
                                                if (!resultadoImagen.StatusCode.Equals(HttpStatusCode.OK))
                                                {
                                                    throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            HttpResponseMessage resultado = InformarCambioAdministracion("ComponentesCMS", JsonConvert.SerializeObject(new KeyValuePair<Guid, CMSAdminComponenteEditarViewModel>(componenteEdicion.Clave, Componente), Formatting.Indented));
                            if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                            {
                                throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                            }
                        }

                        contrComponente.InvalidarCache(componenteEdicion.Clave);

                        contrComponente.CrearFilasPropiedadesIntegracionContinua(Componente);

                        if (EntornoActualEsPruebas && iniciado)
                        {
                            //contrComponente.ModificarFilasIntegracionContinuaEntornoSiguiente(Componente, UrlApiDesplieguesEntornoSiguiente);

                            contrComponente.ModificarFilasIntegracionContinuaEntornoSiguiente(Componente, UrlApiEntornoSeleccionado("pre"), UsuarioActual.UsuarioID);
                            contrComponente.ModificarFilasIntegracionContinuaEntornoSiguiente(Componente, UrlApiEntornoSeleccionado("pro"), UsuarioActual.UsuarioID);
                        }

                        if (transaccionIniciada)
                        {
                            mEntityContext.TerminarTransaccionesPendientes(true);
                        }

                        if (bloqueContenedor.HasValue)
                        {
                            return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARPAGINA") + "/" + ((short)gestorCMS.ListaBloques[bloqueContenedor.Value].TipoUbicacion).ToString());
                        }
                        else
                        {
                            if (!esEdicion)
                            {
                                return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSLISTADOCOMPONENTES"));
                            }
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
                }
                else
                {
                    return GnossResultERROR(error);
                }

                string rutasImagenes = "";
                if (componenteEdicion is CMSComponenteDestacado)
                {
                    rutasImagenes = ((CMSComponenteDestacado)componenteEdicion).Imagen;
                }
                return GnossResultOK(rutasImagenes);
            }
            catch
            {
                return GnossResultERROR("Ha habido un error al guardar el componente.");
            }
        }


       

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult ComprobarLista(string[] listaIDs)
        {
            List<CMSAdminComponenteEditarCheckListViewModel> resultadoComprobacion = new List<CMSAdminComponenteEditarCheckListViewModel>();

            string errorNoExiste = "";
            if (TipoComponenteCMSActual == TipoComponenteCMS.GrupoComponentes)
            {
                errorNoExiste = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOEXISTECOMPONENTE");
            }
            else if (TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
            {
                errorNoExiste = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOEXISTEPROYECTO");
            }
            else
            {
                errorNoExiste = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOEXISTERECURSO");
            }

            List<Guid> listaIdentificadores = new List<Guid>();

            foreach (string id in listaIDs)
            {
                Guid idRecurso = Guid.Empty;
                string error = errorNoExiste;

                if (string.IsNullOrEmpty(id))
                {
                    error = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOVACIO");
                }
                else
                {
                    try
                    {
                        if (TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            idRecurso = proyCN.ObtenerProyectoIDPorNombre(id);
                            proyCN.Dispose();

                            if (idRecurso.Equals(Guid.Empty))
                            {
                                throw new Exception();
                            }
                        }
                        else
                        {
                            idRecurso = new Guid(id);
                        }

                        if (listaIdentificadores.Contains(idRecurso))
                        {
                            error = UtilIdiomas.GetText("COMADMINCMS", "ERRORLISTADO_NOREPETIDO");
                        }
                        else
                        {
                            listaIdentificadores.Add(idRecurso);
                        }

                    }
                    catch (Exception)
                    {
                        error = errorNoExiste;
                    }
                }

                CMSAdminComponenteEditarCheckListViewModel datosResultado = new CMSAdminComponenteEditarCheckListViewModel();
                datosResultado.Orden = resultadoComprobacion.Count;
                datosResultado.Identificador = idRecurso;
                datosResultado.Error = error;
                resultadoComprobacion.Add(datosResultado);

            }

            if (TipoComponenteCMSActual == TipoComponenteCMS.GrupoComponentes)
            {
                CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionCMS gestionCMS = new GestionCMS(cmsCN.ObtenerComponentePorListaID(listaIdentificadores, ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                cmsCN.Dispose();

                foreach (Guid componenteID in gestionCMS.ListaComponentes.Keys)
                {
                    CMSComponente componente = gestionCMS.ListaComponentes[componenteID];

                    var resultados = resultadoComprobacion.Where(resultado => resultado.Identificador == componenteID);
                    if(resultados.Any())
                    {
                        string enlace = mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARCOMPONENTE") + "/" + componente.Clave.ToString();
                        resultados.First().UrlEnlace = enlace;
                        resultados.First().TextoEnlace = componente.Nombre;
                        resultados.First().Error = "";
                    }
                }
            }
            else if (TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionProyecto gestorProy = new GestionProyecto(proyCN.ObtenerProyectosPorID(listaIdentificadores), mLoggingService, mEntityContext);
                proyCN.Dispose();

                foreach (Guid proyID in gestorProy.ListaProyectos.Keys)
                {
                    Proyecto proy = gestorProy.ListaProyectos[proyID];

                    var resultados = resultadoComprobacion.Where(resultado => resultado.Identificador == proyID);
                    if (resultados.Any())
                    {
                        string enlace = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proy.NombreCorto);
                        resultados.First().UrlEnlace = enlace;
                        resultados.First().TextoEnlace = proy.Nombre;
                        resultados.First().Error = "";
                    }
                }
            }
            else
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentosPorID(listaIdentificadores, true), mLoggingService, mEntityContext);
                docCN.ObtenerBaseRecursosProyecto(gestorDoc.DataWrapperDocumentacion, ProyectoSeleccionado.Clave);
                docCN.Dispose();

                foreach (Guid docID in gestorDoc.ListaDocumentos.Keys)
                {
                    Documento doc = gestorDoc.ListaDocumentos[docID];

                    var resultados = resultadoComprobacion.Where(resultado => resultado.Identificador == docID);
                    if (resultados.Any())
                    {
                        string enlace = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURL, UtilIdiomas, ProyectoSeleccionado.NombreCorto, UrlPerfil, doc, false);
                        resultados.First().UrlEnlace = enlace;
                        resultados.First().TextoEnlace = UtilCadenas.ObtenerTextoDeIdioma(doc.Titulo, UtilIdiomas.LanguageCode, null);
                        resultados.First().Error = "";
                    }
                }
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
            };
            var resultado = Json(resultadoComprobacion, options);            
            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idComponente"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult Delete(string idComponente)
        {
            //string idComponente = RequestParams("idComponente");
            Guid ComponenteID = Guid.Empty;

            if (Guid.TryParse(idComponente, out ComponenteID) && !ComponenteID.Equals(Guid.Empty))
            {
                using (CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                using(GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerComponentePorID(ComponenteID, ProyectoSeleccionado.Clave), mLoggingService, mEntityContext))
                {
                    ControladorComponenteCMS contrCMS = new ControladorComponenteCMS(ProyectoSeleccionado,ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    contrCMS.BorrarComponenteCrearFilasIntegracionContinua(ComponenteID, cmsCN, gestorCMS);
                    CMSAdminComponenteEditarViewModel componente = null;
                    HttpResponseMessage resultado = InformarCambioAdministracion("ComponentesCMS", JsonConvert.SerializeObject(new KeyValuePair<Guid, CMSAdminComponenteEditarViewModel>(ComponenteID, componente), Formatting.Indented));
                    return GnossResultOK();
                }
            }

            return GnossResultERROR();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idComponente"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Pagina })]
        public ActionResult Select(string idComponente)
        {
            string idBloqueContenedor = RequestParams("idBloqueContenedor");
            //string idComponente = RequestParams("idComponente");

            if (!string.IsNullOrEmpty(idBloqueContenedor) && !string.IsNullOrEmpty(idComponente))
            {
                Guid bloqueID = Guid.Empty;
                Guid ComponenteID = Guid.Empty;

                if (Guid.TryParse(idBloqueContenedor, out bloqueID) && !bloqueID.Equals(Guid.Empty) && Guid.TryParse(idComponente, out ComponenteID) && !ComponenteID.Equals(Guid.Empty))
                {
                    CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    gestorCMS.AgregarComponenteABloque(ProyectoSeleccionado, bloqueID, ComponenteID);
                    cmsCN.ActualizarCMS(gestorCMS.CMSDW);
                    cmsCN.Dispose();

                    return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARPAGINA") + "/" + ((short)gestorCMS.ListaBloques[bloqueID].TipoUbicacion).ToString());
                }
            }

            return GnossResultERROR();
        }


        #region Propiedades


        public CMSAdminComponenteEditarViewModel.PropiedadComponente ObtenerPropiedad(TipoPropiedadCMS tipoPropiedad, Dictionary<TipoPropiedadCMS, bool> propiedadesComponente)
        {
            CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad = new CMSAdminComponenteEditarViewModel.PropiedadComponente();
            propiedad.TipoPropiedadCMS = tipoPropiedad;
            propiedad.Required = propiedadesComponente[tipoPropiedad];
            if (CMSComponente != null && CMSComponente.PropiedadesComponente.ContainsKey(tipoPropiedad))
            {
                if (tipoPropiedad.Equals(TipoPropiedadCMS.ListaIDs) && TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
                {
                    string[] listaIDs = CMSComponente.PropiedadesComponente[tipoPropiedad].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    List<Guid> listaProyectos = new List<Guid>();
                    foreach (string elemento in listaIDs)
                    {
                        listaProyectos.Add(new Guid(elemento));
                    }

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Dictionary<Guid, string> listaNombresCortos = proyCN.ObtenerNombresCortosProyectos(listaProyectos);
                    proyCN.Dispose();
                    //Recorremos esta lista para que no nos cambie el orden de los proyectos.
                    foreach (Guid proyectoID in listaProyectos)
                    {
                        propiedad.Value += listaNombresCortos[proyectoID] + ",";
                    }
                }
                else
                {
                    propiedad.Value = CMSComponente.PropiedadesComponente[tipoPropiedad];
                }
            }
            propiedad.TypeComponent = TipoComponenteCMSActual;

            propiedad.MultiLang = EsPropiedadMultiIdioma(tipoPropiedad);

            return propiedad;
        }

        private bool EsPropiedadMultiIdioma(TipoPropiedadCMS pTipoPropiedad)
        {
            return UtilComponentes.ListaPropiedadesMultiIdioma.Contains(pTipoPropiedad);
        }

        private CMSAdminComponenteEditarViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    ControladorComponenteCMS contrComponente = new ControladorComponenteCMS(ProyectoSeleccionado, ParametroProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mGnossCache, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                    mPaginaModel = contrComponente.CargarComponente(TipoComponenteCMSActual, CMSComponente);

                    mPaginaModel.Personalizaciones = new Dictionary<Guid, string>();
                    mPaginaModel.PersonalizacionSeleccionada = Guid.Empty;

                    VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave, mControladorBase.PersonalizacionEcosistemaID, mControladorBase.ComunidadExcluidaPersonalizacionEcosistema);
                    vistaVirtualCL.Dispose();

                    string tipoComponente = TipoComponenteCMSActual.ToString();
                    string vistaComponente = $"/Views/CMSPagina/{tipoComponente}/_{tipoComponente}";

                    List<VistaVirtualCMS> listaVistaVirtualCMS = vistaVirtualDW.ListaVistaVirtualCMS.Where(fila => fila.TipoComponente.StartsWith(vistaComponente)).ToList();

                    if (listaVistaVirtualCMS.Any())
                    {
                        foreach (VistaVirtualCMS filaVistaVirtualCMS in listaVistaVirtualCMS)
                        {
                            string nombre = filaVistaVirtualCMS.Nombre;
                            if (string.IsNullOrEmpty(nombre))
                            {
                                nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                            }
                            mPaginaModel.Personalizaciones.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);

                            if (CMSComponente != null && CMSComponente.PropiedadesComponente.ContainsKey(TipoPropiedadCMS.Personalizacion) && CMSComponente.PropiedadesComponente[TipoPropiedadCMS.Personalizacion] == filaVistaVirtualCMS.PersonalizacionComponenteID.ToString())
                            {
                                mPaginaModel.PersonalizacionSeleccionada = filaVistaVirtualCMS.PersonalizacionComponenteID;
                            }
                        }
                    }

                    if (ParametrosGeneralesRow.IdiomasDisponibles)
                    {
                        mPaginaModel.ListaIdiomas = mConfigService.ObtenerListaIdiomasDictionary();
                    }
                    else
                    {
                        mPaginaModel.ListaIdiomas = new Dictionary<string, string>();
                        mPaginaModel.ListaIdiomas.Add(IdiomaPorDefecto, mConfigService.ObtenerListaIdiomasDictionary()[IdiomaPorDefecto]);
                    }

                    if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma) || (ParametroProyecto.ContainsKey(ParametroAD.PropiedadCMSMultiIdioma) && ParametroProyecto[ParametroAD.PropiedadCMSMultiIdioma] == "1"))
                    {
                        mPaginaModel.ContenidoMultiIdioma = true;
                        mPaginaModel.ListaIdiomasDisponibles = new List<string>();

                        if (CMSComponente != null && !string.IsNullOrEmpty(CMSComponente.FilaComponente.IdiomasDisponibles))
                        {
                            mPaginaModel.ListaIdiomasDisponibles = mConfigService.ObtenerListaIdiomas();
                        }
                    }

                    mPaginaModel.IdiomaPorDefecto = IdiomaPorDefecto;
                    mPaginaModel.EsEdicion = esEdicion;

                    Uri urlVolver = null;
                    if (!string.IsNullOrEmpty(Request.Headers["Referer"]))
                    {
                        urlVolver = new Uri(Request.Headers["Referer"]);
                    }

                    if (bloqueContenedor.HasValue)
                    {
                        CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                        cmsCN.Dispose();

                        urlVolver = new Uri(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "ADMINISTRARCOMUNIDADCMSEDITARPAGINA") + "/" + ((short)gestorCMS.ListaBloques[bloqueContenedor.Value].TipoUbicacion).ToString());
                    }
                    if (urlVolver != null)
                    {
                        mPaginaModel.UrlVuelta = urlVolver.ToString();
                    }
                }
                return mPaginaModel;
            }
        }

        /// <summary>
        /// Componente editado
        /// </summary>
        private CMSComponente CMSComponente
        {
            get
            {
                if (mCMSComponente == null && esEdicion)
                {
                    Guid idComponente = new Guid(RequestParams("idComponente"));
                    GestionCMS gestorCMS = null;
                    using (CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                    {
                        gestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    }
                    mCMSComponente = gestorCMS.ListaComponentes[idComponente];
                }
                return mCMSComponente;
            }
        }

        /// <summary>
        /// Indica si estamos editando o creando el componente
        /// </summary>
        private bool esEdicion
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("idComponente")))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Tipo de componente actual
        /// </summary>
        private TipoComponenteCMS TipoComponenteCMSActual
        {
            get
            {
                if (esEdicion)
                {
                    return CMSComponente.TipoComponenteCMS;
                }
                else
                {
                    return (TipoComponenteCMS)short.Parse(RequestParams("tipoComponente"));
                }
            }
        }

        /// <summary>
        /// Indica el bloque que contendra al componente
        /// </summary>
        private Guid? bloqueContenedor
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("idBloqueContenedor")))
                {
                    return new Guid(RequestParams("idBloqueContenedor"));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Indica el tipo de componetne que contendra el contendor
        /// </summary>
        private TipoComponenteCMS? tipoComponenteContenedor
        {
            get
            {
                if (!string.IsNullOrEmpty(RequestParams("tipoComponente")))
                {
                    return (TipoComponenteCMS)short.Parse(RequestParams("tipoComponente"));
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion
    }
}