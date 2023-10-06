using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using static Es.Riam.Gnoss.Web.MVC.Models.ViewModels.EditProfileViewModel;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class EditarPerfilComunidadController : ControllerBaseWeb
    {
        #region Miembrios
        private EditProfileViewModel paginaModel = new EditProfileViewModel();
        private Suscripcion mSuscripcion;
        private DataWrapperProyecto mDatosExtraProyectoDS = null;

        /// <summary>
        /// Ontologia
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// Clausulas adicionales del registro
        /// </summary>
        private List<EditProfileViewModel.AdditionalClause> mClausulasRegistro = null;

        #endregion

        public EditarPerfilComunidadController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Metodos Acciones

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EsPaginaEdicion = true;
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Método inicial.
        /// </summary>
        /// <returns>Acción resultante</returns>
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.MiembroComunidad })]
        public ActionResult Index()
        {
            paginaModel.UrlActionSaveImage = mControladorBase.UrlsSemanticas.GetURLEditarPerfil(BaseURLIdioma, UrlPerfil, UtilIdiomas, ProyectoSeleccionado.NombreCorto) + "/" + "save-image";

            paginaModel.UrlActionSaveProfile = mControladorBase.UrlsSemanticas.GetURLEditarPerfil(BaseURLIdioma, UrlPerfil, UtilIdiomas, ProyectoSeleccionado.NombreCorto) + "/" + "save-profile";
            List<CategoryModel> categoriasTesauro = CargarTesauroProyecto(UtilIdiomas.LanguageCode);

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }

                paginaModel.ProfileOrganization = new EditProfileViewModel.ProfileOrganizationViewModel();
                CargarDatosPerfilOrganizacion();

                paginaModel.UrlActionSaveImage = mControladorBase.UrlsSemanticas.GetURLEditarPerfilOrg(BaseURLIdioma, UrlPerfil, UtilIdiomas, ProyectoSeleccionado.NombreCorto) + "/" + "save-image";

                paginaModel.UrlActionSaveProfile = mControladorBase.UrlsSemanticas.GetURLEditarPerfilOrg(BaseURLIdioma, UrlPerfil, UtilIdiomas, ProyectoSeleccionado.NombreCorto) + "/" + "save-profile";
            }
            else if (IdentidadActual.TrabajaConOrganizacion)
            {
                paginaModel.ProfileProfesional = new EditProfileViewModel.ProfileProfesionalViewModel();
                CargarDatosPerfilProfesional();
            }
            else if (IdentidadActual.EsIdentidadProfesor)
            {
                paginaModel.ProfileTeacher = new EditProfileViewModel.ProfileTeacherViewModel();
                CargarDatosPerfilProfesor();
            }
            else if (IdentidadActual.ModoPersonal)
            {
                paginaModel.ProfilePersonal = new EditProfileViewModel.ProfilePersonalViewModel();
                CargarDatosPerfilPersonal();
            }

            if (paginaModel.ProfileOrganization == null)
            {
                CargarCurriculum();
            }

            CargarRedesSociales();

            paginaModel.UrlActionSaveEmail = mControladorBase.UrlsSemanticas.GetURLEditarPerfil(BaseURLIdioma, UrlPerfil, UtilIdiomas, ProyectoSeleccionado.NombreCorto) + "/" + "save-email";

            paginaModel.UrlActionSaveBio = mControladorBase.UrlsSemanticas.GetURLEditarPerfil(BaseURLIdioma, UrlPerfil, UtilIdiomas, ProyectoSeleccionado.NombreCorto) + "/" + "save-bio";

            paginaModel.UrlSaveUserDataForm = mControladorBase.UrlsSemanticas.GetURLEditarPerfil(BaseURLIdioma, UrlPerfil, UtilIdiomas, ProyectoSeleccionado.NombreCorto) + "/" + "save-data";

            paginaModel.UrlActionDeleteBio = mControladorBase.UrlsSemanticas.GetURLEditarPerfil(BaseURLIdioma, UrlPerfil, UtilIdiomas, ProyectoSeleccionado.NombreCorto) + "/" + "delete-bio";

            if (Suscripcion != null && Suscripcion.FilasCategoriasVinculadas != null)
            {
                foreach (CategoryModel cat in categoriasTesauro)
                {
                    cat.Selected = Suscripcion.GestorSuscripcion.SuscripcionDW.ListaCategoriaTesVinSuscrip.Any(filaCat => filaCat.CategoriaTesauroID == cat.Key);
                }
            }

            paginaModel.Categories = categoriasTesauro;
            return View(paginaModel);
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

        [HttpPost]
        public ActionResult Index(EditProfileViewModel pPaginaModel)
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            if (RequestParams("callback") != null)
            {
                if (RequestParams("callback").ToLower() == "EliminarImagen".ToLower())
                {
                    BorrarImagenRegistro();
                    return new EmptyResult();
                }
                else if (RequestParams("callback").ToLower() == "UsarFotoPersonal".ToLower())
                {
                    UsarImagenPersonal(bool.Parse(RequestParams("UsarFotoPersonal")));
                    IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Clave, IdentidadActual.PersonaID.Value);
                    identidadCL.Dispose();
                    return new EmptyResult();
                }
                else if (RequestParams("callback").ToLower() == "CambiarPais".ToLower())
                {
                    Guid paisID = new Guid(RequestParams("pais"));
                    CargarDatosProvincia(paisID);
                    return PartialView("_Provincia", paginaModel);

                    //return GnossResultHtml("_Provincia", paginaModel);
                }
                else if (RequestParams("callback").ToLower() == "comprobarEmail".ToLower())
                {
                    if (ExisteEmail(RequestParams("email")))
                    {
                        return Content("KO");
                    }
                    return Content("OK");
                }
                else if (RequestParams("callback").ToLower() == "AnyadirRedSocial".ToLower())
                {
                    string nombreRedSocial = "";

                    try
                    {
                        string url = RequestParams("url");
                        Uri urlTest = null;
                        if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                        {
                            url = "http://" + url;
                        }
                        if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out urlTest))
                        {
                            nombreRedSocial = AnyadirRedSocial(url);
                            return Content("OK:" + nombreRedSocial);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex);
                        return Content("ERROR:" + UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
                    }

                    return Content("ERROR:" + UtilIdiomas.GetText("COMMON", "URLNOVALIDA"));
                }
                else if (RequestParams("callback").ToLower() == "EliminarRedSocial".ToLower())
                {
                    EliminarRedSocial(RequestParams("nombreRed"));
                }
                else if (RequestParams("callback").ToLower() == "EditarRedSocial".ToLower())
                {
                    EditarRedSocial(RequestParams("url"), RequestParams("nombreRed"));
                }
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GuardarBio(QuickCurriculum pCurriculum)
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            // Permitir guardar Biografía sin información -> Eliminación de biografía o Tags por parte del usuario                       
            if (string.IsNullOrEmpty(pCurriculum.Description))
            {
                pCurriculum.Description = "";
            }
            if (string.IsNullOrEmpty(pCurriculum.Tags))
            {
                pCurriculum.Tags = "";
            }
            GuardarCVRapido(pCurriculum.Description, pCurriculum.Tags, true);
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Clave, IdentidadActual.PersonaID.Value);
            identidadCL.Dispose();
            return GnossResultOK(UtilIdiomas.GetText("COMADMINCMS", "COMPONENTEGUARDADOOK"));

            //return GnossResultERROR();
        }

        [HttpPost]
        public ActionResult EliminarBio()
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            //EliminarCVRapido();
            return GnossResultOK(UtilIdiomas.GetText("COMADMINCMS", "COMPONENTEGUARDADOOK"));
        }

        [HttpPost]
        public ActionResult GuardarPerfil(EditProfileViewModel pPaginaModel)
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            string urlRedirect = mControladorBase.UrlsSemanticas.GetURLEditarPerfil(BaseURLIdioma.Replace("http://", "https://"), "/", UtilIdiomas, ProyectoSeleccionado.NombreCorto);

            Dictionary<string, string> errores = new Dictionary<string, string>();

            paginaModel = pPaginaModel;

            if (paginaModel.ProfilePersonal != null)
            {
                UtilIdiomas utilIdiomasProfilePersonal = new Recursos.UtilIdiomas(paginaModel.ProfilePersonal.Lang, mLoggingService, mEntityContext, mConfigService);
                urlRedirect = mControladorBase.UrlsSemanticas.GetURLEditarPerfil(BaseURLIdioma.Replace("http://", "https://"), "/", utilIdiomasProfilePersonal, ProyectoSeleccionado.NombreCorto);


                CargarCamposExtraRegistro();

                foreach (AdditionalFieldAutentication campoExtra in paginaModel.ProfilePersonal.AdditionalFields)
                {
                    string nombreCampo = campoExtra.FieldName;
                    string valorCampo = RequestParams(nombreCampo);//campoExtra.FieldValue;

                    campoExtra.FieldValue = valorCampo;

                    if (campoExtra.Required && (string.IsNullOrEmpty(valorCampo) || valorCampo.Equals(Guid.Empty.ToString())))
                    {
                        if (!errores.ContainsKey(campoExtra.Title))
                        {
                            errores.Add(campoExtra.Title, UtilIdiomas.GetText("COMADMINCMS", "ERRORCAMPOVACIO", campoExtra.Title));
                        }
                    }
                }

                if (errores.Count == 0)
                {
                    errores = GuardarPerfilPersonal(paginaModel.ProfilePersonal);
                }
            }
            else if (paginaModel.ProfileProfesional != null)
            {
                errores = GuardarPerfilProfesional(paginaModel.ProfileProfesional);
            }
            else if (paginaModel.ProfileOrganization != null)
            {
                urlRedirect = mControladorBase.UrlsSemanticas.GetURLEditarPerfilOrg(BaseURLIdioma.Replace("http://", "https://"), "/", UtilIdiomas, ProyectoSeleccionado.NombreCorto);

                errores = GuardarPerfilOrganizacion(paginaModel.ProfileOrganization);
            }
            else if (paginaModel.ProfileTeacher != null)
            {
                errores = GuardarPerfilProfesor(paginaModel.ProfileTeacher);
            }
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Clave, IdentidadActual.PersonaID.Value);
            identidadCL.Dispose();

            if (errores.Count > 0)
            {
                string erroresString = "";
                string separador = "";
                foreach (string error in errores.Keys)
                {
                    erroresString += separador + error + "&&" + errores[error];
                    separador = "||";
                }

                return GnossResultERROR(erroresString);
            }
            else
            {
                return GnossResultOK(UtilIdiomas.GetText("COMADMINCMS", "COMPONENTEGUARDADOOK"));
            }
        }



        [HttpPost]
        public ActionResult GuardarDatosUsuarioFormulario(EditProfileViewModel.DataEditProfile dataEditProfile)
        {

            //GuardarPerfilPersonalNulo(EditProfileViewModel.ProfilePersonalViewModel pPerfilPersonal)
            IFormFile pFicheroImagen = dataEditProfile.FicheroImagen;
            string imagen = Request.Form["FicheroImagen"];
            EditProfileViewModel.ProfilePersonalViewModel perfilPersonal = dataEditProfile.ProfilePersonal;
            Dictionary<string, string> errores = new Dictionary<string, string>();
            string description = dataEditProfile.Description;
            string tags = dataEditProfile.Tags;
            string[] listaRedesSociales = null;
            if (dataEditProfile.RedesSociales != null && dataEditProfile.RedesSociales.Contains(","))
            {
                listaRedesSociales = dataEditProfile.RedesSociales.Split(',');
            }
            else if (dataEditProfile.RedesSociales != null && !dataEditProfile.RedesSociales.Equals("undefined"))
            {
                listaRedesSociales = new string[] { dataEditProfile.RedesSociales };
            }

            if (dataEditProfile.RedesSociales != null && listaRedesSociales != null)
            {
                Identidad IdentidadUsuario = IdentidadActual;
                List<AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales> filasRedesSociales = IdentidadUsuario.GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Where(item => item.PerfilID.Equals(IdentidadUsuario.FilaIdentidad.PerfilID)).ToList();
                foreach (AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales filaEliminar in filasRedesSociales)
                {
                    IdentidadUsuario.GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Remove(filaEliminar);
                    mEntityContext.EliminarElemento(filaEliminar);
                }

                foreach (string redSocial in listaRedesSociales)
                {
                    AnyadirRedSocial(redSocial);
                }
            }
            else if (dataEditProfile.RedesSociales != null && dataEditProfile.RedesSociales.Equals("undefined"))
            {
                if (dataEditProfile.RedesSociales.Equals("undefined") && listaRedesSociales == null)
                {
                    EliminarRedesSociales();
                }
            }

            if (perfilPersonal != null)
            {
                paginaModel = new EditProfileViewModel();
                paginaModel.ProfilePersonal = dataEditProfile.ProfilePersonal;

                CargarCamposExtraRegistro();

                errores = GuardarPerfilPersonalNulo(perfilPersonal);
            }
            else
            {
                paginaModel.ProfilePersonal = new EditProfileViewModel.ProfilePersonalViewModel();
                CargarDatosPerfilPersonal();

                perfilPersonal = paginaModel.ProfilePersonal;
                //CargarCamposExtraRegistro();
                //Dictionary<int, string> dicDatosExtraProyectoVirtuoso = new Dictionary<int, string>();
                //Dictionary<int, string> dicDatosExtraEcosistemaVirtuoso = new Dictionary<int, string>();
                //GuardarDatosExtraNulos(perfilPersonal, dicDatosExtraProyectoVirtuoso, dicDatosExtraEcosistemaVirtuoso);
                errores = GuardarPerfilPersonalNulo(perfilPersonal);
            }

            if (!string.IsNullOrEmpty(description) || !string.IsNullOrEmpty(tags))
            {
                GuardarCVRapido(description, tags, true);
                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Clave, IdentidadActual.PersonaID.Value);
                identidadCL.Dispose();
            }

            if (pFicheroImagen != null)
            {
                GnossResult result = (GnossResult)GuardarImagen(pFicheroImagen);
                if (result.result.Status.Equals(GnossResult.GnossStatus.Error))
                {
                    errores.Add("imagen", result.result.Message);
                }
            }
            if (!string.IsNullOrEmpty(imagen) && imagen.Equals("undefined"))
            {
                BorrarImagenRegistro();
            }
            if (errores.Count == 0)
            {
                return GnossResultOK();
            }
            else
            {
                string erroresString = "";
                string separador = "";
                foreach (string error in errores.Keys)
                {
                    erroresString += separador + error + "&&" + errores[error];
                    separador = "||";
                }
                return GnossResultERROR(erroresString);
            }



        }


        [HttpPost]
        public ActionResult GuardarEmail(string pEmail)
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            string error = "";
            if (string.IsNullOrEmpty(pEmail))
            {
                error = "email&&" + UtilIdiomas.GetText("COMADMINCMS", "ERRORCAMPOVACIO", UtilIdiomas.GetText("REGISTROUSUARIOORGANIZACION", "EMAIL").Replace(" *", ""));
            }
            else if (personaCN.ExisteEmail(pEmail, IdentidadActual.Persona.Clave))
            {
                error = "email&&" + UtilIdiomas.GetText("PERFIL", "ERRORMAIL");
            }
            else if (!UtilCadenas.ValidarEmail(pEmail))
            {
                error = "formatoEmail&&" + UtilIdiomas.GetText("REGISTRO", "ERRORFORMATOEMAIL");
            }

            if (string.IsNullOrEmpty(error))
            {
                //Cambiamos el email            
                IdentidadActual.Persona.FilaPersona.Email = pEmail;
                personaCN.ActualizarPersonas();
                personaCN.Dispose();


                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Clave, IdentidadActual.PersonaID.Value);
                identidadCL.Dispose();

                ControladorIdentidades.NotificarEdicionPerfilEnProyectos(TipoAccionExterna.Edicion, IdentidadActual.Persona.Clave, "", "", ProyectoSeleccionado.Clave);

                return GnossResultOK(UtilIdiomas.GetText("COMADMINCMS", "COMPONENTEGUARDADOOK"));
            }
            else
            {
                return GnossResultERROR(error);
            }
        }

        [HttpPost]
        public ActionResult GuardarImagen(IFormFile FicheroImagen)
        {
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (!EsIdentidadActualAdministradorOrganizacion)
                {
                    return RedireccionarAPaginaNoEncontrada();
                }
            }

            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            int versionFoto = 0;

            string urlFicheroImagen = "";

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                urlFicheroImagen = UtilArchivos.ContentImagenesOrganizaciones + "/" + IdentidadActual.OrganizacionID.ToString();

                if (IdentidadActual.OrganizacionPerfil.FilaOrganizacion.VersionLogo != null)
                {
                    versionFoto = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.VersionLogo.Value;
                }
            }
            else if (IdentidadActual.TrabajaConOrganizacion)
            {
                urlFicheroImagen = UtilArchivos.ContentImagenesPersona_Organizacion + "/" + IdentidadActual.OrganizacionID.ToString() + "/" + mControladorBase.UsuarioActual.PersonaID.ToString();

                PersonaVinculoOrganizacion filaPersona = IdentidadActual.OrganizacionPerfil.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();
                if (filaPersona.VersionFoto != null)
                {
                    versionFoto = filaPersona.VersionFoto.Value;
                }
            }
            else if (IdentidadActual.EsIdentidadProfesor)
            {
                return GnossResultERROR("Not Autorized");
            }
            else if (IdentidadActual.ModoPersonal)
            {
                urlFicheroImagen = UtilArchivos.ContentImagenesPersonas + "/" + mControladorBase.UsuarioActual.PersonaID.ToString();

                if (IdentidadActual.Persona.FilaPersona.VersionFoto.HasValue)
                {
                    versionFoto = IdentidadActual.Persona.FilaPersona.VersionFoto.Value;
                }
            }

            string error = string.Empty;

            int tamanoMini = 60;
            int tamanoMaxi = 240;

            int minSize = 240;
            int maxSize = 450;

            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
            servicioImagenes.Url = UrlIntragnossServicios;

            //Si se sube un fichero nuevo se borra la foto temporal
            servicioImagenes.BorrarImagen(urlFicheroImagen + "_temp" + ".png");
            servicioImagenes.BorrarImagen(urlFicheroImagen + "_temp2" + ".png");

            //Límite de 10 MB
            if (FicheroImagen.Length <= 10 * 1024 * 1024)
            {
                byte[] bytesFichero = new byte[FicheroImagen.Length];
                ((System.IO.Stream)FicheroImagen.OpenReadStream()).Read(bytesFichero, 0, (int)FicheroImagen.Length);

                Image imagePerfilOriginal = UtilImages.ConvertirArrayBytesEnImagen(bytesFichero);

                float proporcion = 0;
                if (imagePerfilOriginal.Height > imagePerfilOriginal.Width)
                {
                    proporcion = (float)imagePerfilOriginal.Height / imagePerfilOriginal.Width;
                }
                else
                {
                    proporcion = (float)imagePerfilOriginal.Width / imagePerfilOriginal.Height;
                }

                if (proporcion < 1.8)
                {
                    if (imagePerfilOriginal.Height >= minSize && imagePerfilOriginal.Width >= minSize)
                    {
                        //la redimensionamos
                        SizeF tamañoProporcional = UtilImages.CalcularTamanioProporcionado(imagePerfilOriginal, maxSize, maxSize);
                        imagePerfilOriginal = UtilImages.AjustarImagen(imagePerfilOriginal, tamañoProporcional.Width, tamañoProporcional.Height);

                        //Imagen Original
                        MemoryStream ms = new MemoryStream();
                        imagePerfilOriginal.SaveAsPng(ms);
                        servicioImagenes.AgregarImagen(ms.ToArray(), urlFicheroImagen, ".png");

                        int w = 0;
                        int h = 0;
                        int x = 0;
                        int y = 0;

                        if (imagePerfilOriginal.Height > imagePerfilOriginal.Width)
                        {
                            w = imagePerfilOriginal.Width;
                            h = imagePerfilOriginal.Width;
                            y = imagePerfilOriginal.Height / 2 - imagePerfilOriginal.Width / 2;
                        }
                        else if (imagePerfilOriginal.Height < imagePerfilOriginal.Width)
                        {
                            w = imagePerfilOriginal.Height;
                            h = imagePerfilOriginal.Height;
                            x = imagePerfilOriginal.Width / 2 - imagePerfilOriginal.Height / 2;
                        }
                        else
                        {
                            w = imagePerfilOriginal.Width;
                            h = imagePerfilOriginal.Height;
                        }

                        byte[] bytesImagenCortada = UtilImages.CropImageFile(ms.ToArray(), w, h, x, y);
                        Image imagenCortada = UtilImages.ConvertirArrayBytesEnImagen(bytesImagenCortada);

                        Image imagenCortadaGrande = UtilImages.AjustarImagen(imagenCortada, tamanoMaxi, tamanoMaxi, false);

                        //convertimos la imagen a png
                        MemoryStream msGrande = new MemoryStream();
                        imagenCortadaGrande.SaveAsPng(msGrande);

                        servicioImagenes.AgregarImagen(msGrande.ToArray(), urlFicheroImagen + "_grande", ".png");

                        Image imagenCortadaMini = UtilImages.AjustarImagen(imagenCortada, tamanoMini, tamanoMini, false);

                        //convertimos la imagen a png
                        MemoryStream msMini = new MemoryStream();
                        imagenCortadaMini.SaveAsPng(msMini);

                        servicioImagenes.AgregarImagen(msMini.ToArray(), urlFicheroImagen + "_peque", ".png");

                        string coordenadasFoto = "[ " + x.ToString() + ", " + y.ToString() + ", " + (x + w).ToString() + ", " + (y + h).ToString() + " ]";
                        versionFoto = versionFoto + 1;

                        if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
                        {
                            if (mEntityContext.Entry(IdentidadActual.OrganizacionPerfil.FilaOrganizacion).State.Equals(EntityState.Detached))
                            {
                                IdentidadActual.OrganizacionPerfil.FilaElementoEntity = mEntityContext.Organizacion.FirstOrDefault(orga => orga.OrganizacionID.Equals(IdentidadActual.OrganizacionPerfil.FilaOrganizacion.OrganizacionID));
                            }
                            IdentidadActual.OrganizacionPerfil.FilaOrganizacion.VersionLogo = versionFoto;
                            IdentidadActual.OrganizacionPerfil.FilaOrganizacion.CoordenadasLogo = coordenadasFoto;

                            mEntityContext.SaveChanges();

                            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            //identidadCN.ActualizarFotoIdentidadesPersona(IdentidadActual.PersonaID.Value, false);
                            identidadCN.ActualizarFotoIdentidadesOrganizacion(IdentidadActual.OrganizacionID.Value, false);
                            identidadCN.Dispose();
                        }
                        else if (IdentidadActual.TrabajaConOrganizacion)
                        {
                            PersonaVinculoOrganizacion filaPersona = IdentidadActual.OrganizacionPerfil.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();
                            if (mEntityContext.Entry(filaPersona).State.Equals(EntityState.Detached))
                            {
                                filaPersona = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();
                            }
                            filaPersona.VersionFoto = versionFoto;
                            filaPersona.CoordenadasFoto = coordenadasFoto;
                            filaPersona.FechaAnadidaFoto = DateTime.Now;
                            filaPersona.UsarFotoPersonal = false;
                            mEntityContext.SaveChanges();

                            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            identidadCN.ActualizarFotoIdentidadesDePersonaDeOrganizacion(IdentidadActual.PersonaID.Value, filaPersona.OrganizacionID, false, false);
                            //identidadCN.ActualizarFotoIdentidadesPersona(IdentidadActual.PersonaID.Value, false);
                            identidadCN.Dispose();
                        }
                        else if (IdentidadActual.ModoPersonal)
                        {
                            if (mEntityContext.Entry(IdentidadActual.Persona.FilaPersona).State.Equals(EntityState.Detached))
                            {
                                IdentidadActual.Persona.FilaPersona = mEntityContext.Persona.FirstOrDefault(pers => pers.PersonaID.Equals(IdentidadActual.Persona.FilaPersona.PersonaID));
                            }
                            IdentidadActual.Persona.FilaPersona.VersionFoto = versionFoto;
                            IdentidadActual.Persona.FilaPersona.CoordenadasFoto = coordenadasFoto;
                            IdentidadActual.Persona.FilaPersona.FechaAnadidaFoto = DateTime.Now;
                            mEntityContext.SaveChanges();

                            IdentidadCN identidadCN = new IdentidadCN(mEntityContext,mLoggingService,mConfigService, mServicesUtilVirtuosoAndReplication);
                            identidadCN.ActualizarFotoIdentidadesPersona(IdentidadActual.PersonaID.Value, false);
                            identidadCN.Dispose();
                        }
                    }
                    else
                    {
                        error = UtilIdiomas.GetText("PERFIL", "ERRORIMAGENPEQUEÑA", minSize + " px.");
                    }
                }
                else
                {
                    error = UtilIdiomas.GetText("PERFIL", "ERRORIMAGENCUADRADA");
                }
            }
            else
            {
                error = UtilIdiomas.GetText("PERFIL", "ERRORTAMAÑOIMAGEN");
            }

            if (error.Equals(string.Empty))
            {
                EliminarCaches();

                //return Content(UtilArchivos.ContentImagenes + "/" + urlFicheroImagen.ToLower() + "_grande.png?" + Guid.NewGuid().ToString());

                return GnossResultOK(UtilArchivos.ContentImagenes + "/" + urlFicheroImagen/*.ToLower()*/ + "_grande.png?" + Guid.NewGuid().ToString());
            }
            else
            {
                //return new StatusCodeResult(400, error);
                return GnossResultERROR(error);
            }
        }

        #endregion

        private void EliminarRedSocial(string nombreRedSocial)
        {
            Identidad IdentidadUsuario = IdentidadActual;
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                IdentidadUsuario = IdentidadActual.IdentidadOrganizacion;
            }

            AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales perfil = IdentidadUsuario.GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Where(item => item.PerfilID.Equals(IdentidadUsuario.FilaIdentidad.PerfilID) && item.NombreRedSocial.Equals(nombreRedSocial)).FirstOrDefault();

            IdentidadUsuario.GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Remove(perfil);
            mEntityContext.EliminarElemento(perfil);

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.ActualizaIdentidades();
            identidadCN.Dispose();

            EliminarCaches();
        }

        private void EliminarRedesSociales()
        {
            Identidad IdentidadUsuario = IdentidadActual;
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                IdentidadUsuario = IdentidadActual.IdentidadOrganizacion;
            }

            List<AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales> filasRedesSociales = IdentidadUsuario.GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Where(item => item.Equals(IdentidadUsuario.FilaIdentidad.PerfilID)).ToList();
            foreach (AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales filaEliminar in filasRedesSociales)
            {
                IdentidadUsuario.GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Remove(filaEliminar);
                mEntityContext.EliminarElemento(filaEliminar);
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.ActualizaIdentidades();
            identidadCN.Dispose();

            EliminarCaches();
        }

        private void EditarRedSocial(string urlRedSocial, string nombreRedSocial)
        {
            Identidad IdentidadUsuario = IdentidadActual;
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                IdentidadUsuario = IdentidadActual.IdentidadOrganizacion;
            }

            AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales filaPerfilRedesSoc = IdentidadUsuario.GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Where(item => item.PerfilID.Equals(IdentidadUsuario.FilaIdentidad.PerfilID) && item.NombreRedSocial.Equals(nombreRedSocial)).FirstOrDefault();
            if (mEntityContext.Entry(filaPerfilRedesSoc).State.Equals(EntityState.Detached))
            {
                filaPerfilRedesSoc = mEntityContext.PerfilRedesSociales.Where(item => item.PerfilID.Equals(IdentidadUsuario.FilaIdentidad.PerfilID) && item.NombreRedSocial.Equals(nombreRedSocial)).FirstOrDefault();
            }
            while (true)
            {
                if (urlRedSocial.EndsWith("/"))
                {
                    urlRedSocial = urlRedSocial.Substring(0, urlRedSocial.Length - 1);
                }
                else
                {
                    break;
                }
            }
            filaPerfilRedesSoc.urlUsuario = urlRedSocial;
            filaPerfilRedesSoc.Usuario = urlRedSocial.Substring(urlRedSocial.LastIndexOf('/') + 1);

            if (urlRedSocial.StartsWith("http://"))
            {
                urlRedSocial = urlRedSocial.Substring(7);
            }
            if (urlRedSocial.StartsWith("https://"))
            {
                urlRedSocial = urlRedSocial.Substring(8);
            }
            if (urlRedSocial.StartsWith("www."))
            {
                urlRedSocial = urlRedSocial.Substring(4);
            }
            filaPerfilRedesSoc.NombreRedSocial = urlRedSocial.Substring(0, 1).ToUpper() + urlRedSocial.Substring(1, urlRedSocial.IndexOf('.') - 1).ToLower();

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.ActualizaIdentidades();
            identidadCN.Dispose();

            EliminarCaches();
        }

        private string AnyadirRedSocial(string urlRedSocial)
        {
            Identidad IdentidadUsuario = IdentidadActual;
            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                IdentidadUsuario = IdentidadActual.IdentidadOrganizacion;
            }

            AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales filaPerfilRedesSoc = new AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales();

            filaPerfilRedesSoc.PerfilID = IdentidadUsuario.FilaIdentidad.PerfilID;

            while (true)
            {
                if (urlRedSocial.EndsWith("/"))
                {
                    urlRedSocial = urlRedSocial.Substring(0, urlRedSocial.Length - 1);
                }
                else
                {
                    break;
                }
            }
            filaPerfilRedesSoc.urlUsuario = urlRedSocial;
            int fin = urlRedSocial.Length;
            if (urlRedSocial.IndexOf('?') > 0)
            {
                fin = urlRedSocial.IndexOf('?');
            }

            int inicio = urlRedSocial.Substring(0, fin).LastIndexOf('/') + 1;
            string usuario = "";
            if (inicio > 0)
            {
                usuario = urlRedSocial.Substring(inicio, fin - inicio);
            }
            if (usuario.Length > 15)
            {
                filaPerfilRedesSoc.Usuario = usuario.Substring(0, 15);
            }
            else
            {
                filaPerfilRedesSoc.Usuario = usuario;
            }

            if (urlRedSocial.StartsWith("http://"))
            {
                urlRedSocial = urlRedSocial.Substring(7);
            }
            if (urlRedSocial.StartsWith("https://"))
            {
                urlRedSocial = urlRedSocial.Substring(8);
            }
            if (urlRedSocial.StartsWith("www."))
            {
                urlRedSocial = urlRedSocial.Substring(4);
            }
            if (urlRedSocial.IndexOf('/') > urlRedSocial.IndexOf('.'))
            {
                urlRedSocial = urlRedSocial.Substring(0, urlRedSocial.IndexOf('/'));
            }

            string nombreRedSocial = urlRedSocial.Substring(0, 1).ToUpper() + urlRedSocial.Substring(1, urlRedSocial.LastIndexOf('.') - 1).ToLower();

            if (nombreRedSocial.Length > 20)
            {
                filaPerfilRedesSoc.NombreRedSocial = nombreRedSocial.Substring(0, 20);
            }
            else
            {
                filaPerfilRedesSoc.NombreRedSocial = nombreRedSocial;
            }
            IdentidadUsuario.GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Add(filaPerfilRedesSoc);
            mEntityContext.PerfilRedesSociales.Add(filaPerfilRedesSoc);

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.ActualizaIdentidades();
            identidadCN.Dispose();

            EliminarCaches();

            return filaPerfilRedesSoc.NombreRedSocial;
        }

        private Dictionary<string, string> GuardarPerfilOrganizacion(EditProfileViewModel.ProfileOrganizationViewModel pPerfilOrganizacion)
        {
            Dictionary<string, string> listaErrores = new Dictionary<string, string>();

            try
            {
                //OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService);
                //IdentidadActual.OrganizacionPerfil.GestorOrganizaciones.OrganizacionDS.Merge(organizacionCN.ObtenerOrganizacionPorID(IdentidadActual.OrganizacionPerfil.Clave));
                //organizacionCN.Dispose();

                Elementos.ServiciosGenerales.Organizacion organizacion = ObtenerOrganizacionEditar(pPerfilOrganizacion.NameOrganization);

                bool cambiadoNombre = (organizacion.FilaOrganizacion.Nombre != pPerfilOrganizacion.NameOrganization);
                bool cambiadoAlias = (organizacion.FilaOrganizacion.Alias != pPerfilOrganizacion.Alias);

                organizacion.FilaOrganizacion.CP = pPerfilOrganizacion.PostalCode;
                organizacion.FilaOrganizacion.Direccion = pPerfilOrganizacion.Address;
                organizacion.FilaOrganizacion.Localidad = pPerfilOrganizacion.Location;
                organizacion.FilaOrganizacion.Provincia = "";

                Guid provinciaID = Guid.Empty;
                if (Guid.TryParse(pPerfilOrganizacion.Region, out provinciaID) && provinciaID != Guid.Empty)
                {
                    organizacion.FilaOrganizacion.ProvinciaID = provinciaID;
                }
                else
                {
                    organizacion.FilaOrganizacion.Provincia = pPerfilOrganizacion.Region;
                    organizacion.FilaOrganizacion.ProvinciaID = null;
                }
                organizacion.FilaOrganizacion.Nombre = pPerfilOrganizacion.NameOrganization;
                organizacion.FilaOrganizacion.Web = pPerfilOrganizacion.WebSite;
                organizacion.FilaOrganizacion.Alias = pPerfilOrganizacion.Alias;

                AmigosDS amigosDS = null;

                bool organizacionEsClase = organizacion is OrganizacionClase;
                paginaModel.ProfileOrganization.IsClass = organizacionEsClase;

                if (!organizacionEsClase)
                {
                    if (cambiadoAlias)
                    {
                        /*TODO Javier migrar
                        AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService);
                        amigosDS = amigosCN.ObtenerGrupoAmigosAutomatico(organizacion.Clave);
                        AmigosDS.GrupoAmigosRow filaGrupoAmigos = (AmigosDS.GrupoAmigosRow)amigosDS.GrupoAmigos.Rows[0];
                        filaGrupoAmigos.Nombre = UtilIdiomas.GetText("CONTACTOS", "GRUPOMIEMBROSORGANIZACION", organizacion.FilaOrganizacion.Alias);*/
                    }
                }
                else
                {
                    OrganizacionClase clase = (OrganizacionClase)organizacion;

                    clase.Centro = pPerfilOrganizacion.Centre;
                    clase.Asignatura = pPerfilOrganizacion.Subject;
                    clase.Curso = pPerfilOrganizacion.Course;
                    clase.Grupo = pPerfilOrganizacion.Group;

                    if (cambiadoAlias)
                    {
                        /*TODO Javier migrar
                        AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService);
                        amigosDS = amigosCN.ObtenerGrupoAmigosAutomatico(organizacion.Clave);
                        AmigosDS.GrupoAmigosRow filaGrupoAmigos = (AmigosDS.GrupoAmigosRow)amigosDS.GrupoAmigos.Rows[0];
                        filaGrupoAmigos.Nombre = UtilIdiomas.GetText("CONTACTOS", "GRUPOMIEMBROSCLASE", organizacion.FilaOrganizacion.Alias);
                    */
                    }
                }

                organizacion.FilaOrganizacion.PaisID = pPerfilOrganizacion.Country;

                mEntityContext.SaveChanges();

                ControladorOrganizaciones contrOrg = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<Guid> listaProyectos = proyCN.ObtenerListaProyectoIDDeOrganizacion(organizacion.Clave, false);
                if (cambiadoNombre)
                {
                    List<Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS.Perfil> listaPerfiles = identidadCN.ObtenerPerfilesDeUnaOrganizacion(organizacion.FilaOrganizacion.OrganizacionID);
                    foreach(AD.EntityModel.Models.IdentidadDS.Perfil perfil in listaPerfiles)
                    {
                        perfil.NombreOrganizacion = organizacion.FilaOrganizacion.Nombre;
                    }
                    mEntityContext.SaveChanges();
                }
                contrOrg.ActualizarModeloBaseSimpleMultiple(IdentidadActual.Persona.Clave, listaProyectos);

                EliminarCaches();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                listaErrores.Add("error", UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }

            return listaErrores;
        }

        private Es.Riam.Gnoss.Elementos.ServiciosGenerales.Organizacion ObtenerOrganizacionEditar(string pNombreOrganizacion)
        {
            Es.Riam.Gnoss.Elementos.ServiciosGenerales.Organizacion org = IdentidadActual.OrganizacionPerfil;
            if (org == null && IdentidadActual.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaOrganizacion.Any(item => item.Nombre.Equals(pNombreOrganizacion)))
            {
                Guid organizacionID = (IdentidadActual.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaOrganizacion.FirstOrDefault(item => item.Nombre.Equals(pNombreOrganizacion))).OrganizacionID;

                OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperOrganizacion orgDW = orgCN.ObtenerOrganizacionesPorID(new List<Guid>() { organizacionID });
                orgCN.Dispose();

                GestionOrganizaciones gestor = new GestionOrganizaciones(orgDW, mLoggingService, mEntityContext);
                org = gestor.ListaOrganizaciones[organizacionID];
            }

            if (org != null)
            {
                org.FilaElementoEntity = mEntityContext.Organizacion.FirstOrDefault(x => x.OrganizacionID.Equals(org.FilaOrganizacion.OrganizacionID));
            }
            return org;
        }

        private Dictionary<string, string> GuardarPerfilPersonalNulo(EditProfileViewModel.ProfilePersonalViewModel pPerfilPersonal)
        {
            bool existenErrores = false;

            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            Dictionary<string, string> listaErrores = new Dictionary<string, string>();

            try
            {
                if (!string.IsNullOrEmpty(pPerfilPersonal.Email))
                {
                    if (personaCN.ExisteEmail(pPerfilPersonal.Email, IdentidadActual.Persona.Clave))
                    {
                        listaErrores.Add("email", UtilIdiomas.GetText("PERFIL", "ERRORMAIL"));
                        existenErrores = true;
                    }
                    else if (!UtilCadenas.ValidarEmail(pPerfilPersonal.Email))
                    {
                        listaErrores.Add("formatoEmail", UtilIdiomas.GetText("REGISTRO", "ERRORFORMATOEMAIL"));
                        existenErrores = true;
                    }
                }

                if (!existenErrores)
                {
                    if (mEntityContext.Entry(IdentidadActual.Persona.FilaPersona).State == EntityState.Detached)
                    {
                        IdentidadActual.Persona.FilaPersona = personaCN.ObtenerFilaPersonaPorID(IdentidadActual.Persona.FilaPersona.PersonaID);
                    }
                    bool CambiadoNombre = false;
                    bool CambiadoApellidos = false;
                    if (!string.IsNullOrEmpty(pPerfilPersonal.Name))
                    {
                        CambiadoNombre = (IdentidadActual.Persona.FilaPersona.Nombre != pPerfilPersonal.Name);
                    }
                    if (!string.IsNullOrEmpty(pPerfilPersonal.LastName))
                    {
                        CambiadoApellidos = (IdentidadActual.Persona.FilaPersona.Apellidos != pPerfilPersonal.LastName);
                    }


                    if ((IdentidadActual.Persona.FilaPersona.EstadoCorreccion == (short)EstadoCorreccion.NotificadoNoCambiado || IdentidadActual.Persona.FilaPersona.EstadoCorreccion == (short)EstadoCorreccion.NotificadoNoCambiado3Dias) && (CambiadoNombre || CambiadoApellidos))
                    {
                        IdentidadActual.Persona.FilaPersona.EstadoCorreccion = (short)EstadoCorreccion.NotificadoCambiado;
                    }

                    if (!string.IsNullOrEmpty(pPerfilPersonal.Name))
                    {
                        IdentidadActual.Persona.FilaPersona.Nombre = pPerfilPersonal.Name;
                    }

                    if (!string.IsNullOrEmpty(pPerfilPersonal.LastName))
                    {
                        IdentidadActual.Persona.FilaPersona.Apellidos = pPerfilPersonal.LastName;
                    }
                    if (!string.IsNullOrEmpty(pPerfilPersonal.Location))
                    {
                        IdentidadActual.Persona.FilaPersona.LocalidadPersonal = pPerfilPersonal.Location;
                    }
                    string antiguoEmail = IdentidadActual.Persona.FilaPersona.Email;
                    if (!string.IsNullOrEmpty(pPerfilPersonal.Email))
                    {
                        IdentidadActual.Persona.FilaPersona.Email = pPerfilPersonal.Email;
                    }
                    if (!string.IsNullOrEmpty(pPerfilPersonal.Lang))
                    {
                        IdentidadActual.Persona.FilaPersona.Idioma = pPerfilPersonal.Lang;
                    }
                    if (!string.IsNullOrEmpty(pPerfilPersonal.Region))
                    {
                        IdentidadActual.Persona.FilaPersona.ProvinciaPersonal = "";

                        Guid provinciaID = Guid.Empty;
                        if (Guid.TryParse(pPerfilPersonal.Region, out provinciaID) && provinciaID != Guid.Empty)
                        {
                            IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID = provinciaID;
                        }
                        else
                        {
                            IdentidadActual.Persona.FilaPersona.ProvinciaPersonal = pPerfilPersonal.Region;
                            IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID = null;
                        }
                    }

                    if (!string.IsNullOrEmpty(pPerfilPersonal.PostalCode))
                    {
                        IdentidadActual.Persona.FilaPersona.CPPersonal = pPerfilPersonal.PostalCode;
                    }
                    if (pPerfilPersonal.Country != Guid.Empty)
                    {
                        IdentidadActual.Persona.FilaPersona.PaisPersonalID = pPerfilPersonal.Country;
                    }



                    int? dia = null;
                    int? mes = null;
                    int? anio = null;

                    if (!string.IsNullOrEmpty(pPerfilPersonal.BornDate))
                    {
                        string[] fecha = pPerfilPersonal.BornDate.Split('/');

                        dia = int.Parse(fecha[0]);
                        mes = int.Parse(fecha[1]);
                        anio = int.Parse(fecha[2]);

                        IdentidadActual.Persona.FilaPersona.FechaNacimiento = new DateTime(anio.Value, mes.Value, dia.Value);
                    }

                    if (!string.IsNullOrEmpty(pPerfilPersonal.Sex))
                    {
                        IdentidadActual.Persona.FilaPersona.Sexo = pPerfilPersonal.Sex;
                    }

                    // Modificar los tags de nombre de persona
                    if (CambiadoNombre || CambiadoApellidos)
                    {
                        string profesorSexo = "";
                        if (IdentidadActual.Persona.Sexo.Equals("H"))
                        {
                            profesorSexo = UtilIdiomas.GetText("SOLICITUDESNUEVOSPROFESORES", "PROFESOR") + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " ";
                        }
                        else
                        {
                            profesorSexo = UtilIdiomas.GetText("SOLICITUDESNUEVOSPROFESORES", "PROFESORA") + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " ";
                        }

                        foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.PersonaID.Equals(mControladorBase.UsuarioActual.PersonaID)).ToList())
                        {
                            string profesor = "";
                            if (IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaProfesor.Any(item => item.PerfilID.Equals(filaPerfil.PerfilID)))
                            {
                                profesor = profesorSexo;
                            }
                            filaPerfil.NombrePerfil = profesor + IdentidadActual.Persona.NombreConApellidos;

                            if (CambiadoNombre)
                            {
                                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in filaPerfil.Identidad)
                                {
                                    filaIdentidad.NombreCortoIdentidad = profesor + IdentidadActual.Persona.Nombre;
                                }
                            }
                        }
                    }

                    if (ParametrosGeneralesRow.PrivacidadObligatoria && ProyectoPrincipalUnico != ProyectoAD.MetaProyecto)
                    {
                        bool comprobarPerfil = true;
                        bool comprobarPerfilExterno = true;
                        List<ParametroAplicacion> listaParametroApp = ParametrosAplicacionDS.Where(parametroApp => parametroApp.Parametro.Equals("VisibilidadPerfil")).ToList();
                        if (listaParametroApp.Count == 1)
                        {
                            string visibilidadPerfil = listaParametroApp[0].Valor;

                            comprobarPerfil = visibilidadPerfil[0].ToString() == "1";
                            comprobarPerfilExterno = visibilidadPerfil[2].ToString() == "1";
                            IdentidadActual.Persona.FilaPersona.EsBuscable = visibilidadPerfil[1].ToString() == "1";
                            IdentidadActual.Persona.FilaPersona.EsBuscableExternos = visibilidadPerfil[3].ToString() == "1";
                        }

                        if (comprobarPerfil && pPerfilPersonal.IsSearched.HasValue)
                        {
                            IdentidadActual.Persona.FilaPersona.EsBuscable = pPerfilPersonal.IsSearched.Value;
                        }
                        if (comprobarPerfilExterno && pPerfilPersonal.IsExternalSearched.HasValue)
                        {
                            IdentidadActual.Persona.FilaPersona.EsBuscableExternos = pPerfilPersonal.IsExternalSearched.Value;

                            //si está marcado visible en todo internet implica que será visible en la comunidad
                            if (pPerfilPersonal.IsExternalSearched.HasValue && pPerfilPersonal.IsExternalSearched.Value)
                            {
                                IdentidadActual.Persona.FilaPersona.EsBuscable = true;
                            }
                        }
                    }

                    if (!MostrarDatosDemograficosPerfil)
                    {
                        IdentidadActual.Persona.PaisID = Guid.Empty;
                        IdentidadActual.Persona.Provincia = "";
                        IdentidadActual.Persona.Localidad = "";
                        IdentidadActual.Persona.CodPostal = "";
                    }

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);
                    IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(dataWrapperIdentidad);
                    identidadCN.Dispose();

                    Dictionary<int, string> dicDatosExtraProyectoVirtuoso = new Dictionary<int, string>();
                    Dictionary<int, string> dicDatosExtraEcosistemaVirtuoso = new Dictionary<int, string>();
                    GuardarDatosExtraNulos(pPerfilPersonal, dicDatosExtraProyectoVirtuoso, dicDatosExtraEcosistemaVirtuoso);


                    GestorParametroAplicacion gestorApp = new GestorParametroAplicacion();
                    gestorApp.ParametroAplicacion = ParametrosAplicacionDS;
                    JsonEstado jsonEstado = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.Edicion, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.Persona.Nombre, IdentidadActual.Persona.Apellidos, IdentidadActual.Persona.FilaPersona.Email, "", gestorApp, DatosExtraProyectoDS, dicDatosExtraEcosistemaVirtuoso, dicDatosExtraProyectoVirtuoso, antiguoEmail);
                    if (jsonEstado != null && !jsonEstado.Correcto)
                    {
                        //Revierto los cambios y notifico del error                    
                        string error = UtilIdiomas.GetText("LOGIN", "AUTENTICACIONEXTERNA");
                        if (!string.IsNullOrEmpty(jsonEstado.InfoExtra))
                        {
                            error = jsonEstado.InfoExtra;
                        }
                        Session.SetString("errorServicioExterno", error);
                    }
                    else
                    {
                        mEntityContext.SaveChanges();
                        IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        identidadCL.InvalidarFichasIdentidadesMVC(IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Select(item => item.IdentidadID).ToList());
                    }

                    ControladorIdentidades.NotificarEdicionPerfilEnProyectos(TipoAccionExterna.Edicion, IdentidadActual.Persona.Clave, "", antiguoEmail, ProyectoSeleccionado.Clave);

                    if (personaCN != null)
                    {
                        personaCN.Dispose();
                        personaCN = null;
                    }

                    List<Guid> listaProyectos = new List<Guid>();
                    foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad)
                    {
                        if (!listaProyectos.Contains(filaIdentidad.ProyectoID))
                        {
                            listaProyectos.Add(filaIdentidad.ProyectoID);
                        }
                    }
                    if (!listaProyectos.Contains(ProyectoAD.MyGnoss))
                    {
                        listaProyectos.Add(ProyectoAD.MyGnoss);
                    }

                    ControladorPersonas contrPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    contrPers.ActualizarModeloBaseSimpleMultiple(IdentidadActual.Persona.Clave, listaProyectos);
                    EliminarCaches();
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                listaErrores.Add("error", UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }

            return listaErrores;
        }


        private Dictionary<string, string> GuardarPerfilPersonal(ProfilePersonalViewModel pPerfilPersonal)
        {
            bool existenErrores = false;

            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            Dictionary<string, string> listaErrores = new Dictionary<string, string>();

            try
            {
                //Comprobaciones campos vacios perfil
                if (string.IsNullOrEmpty(pPerfilPersonal.Name))
                {
                    listaErrores.Add("nombre", UtilIdiomas.GetText("COMADMINCMS", "ERRORCAMPOVACIO", UtilIdiomas.GetText("REGISTRO", "NOMBRE").Replace(" *", "")));
                    existenErrores = true;
                }
                if (string.IsNullOrEmpty(pPerfilPersonal.LastName))
                {
                    listaErrores.Add("nombre", UtilIdiomas.GetText("COMADMINCMS", "ERRORCAMPOVACIO", UtilIdiomas.GetText("REGISTRO", "APELLIDOS").Replace(" *", "")));
                    existenErrores = true;
                }
                if (string.IsNullOrEmpty(pPerfilPersonal.Location))
                {
                    listaErrores.Add("nombre", UtilIdiomas.GetText("COMADMINCMS", "ERRORCAMPOVACIO", UtilIdiomas.GetText("REGISTRO", "POBLACION").Replace(" *", "")));
                    existenErrores = true;
                }
                //if (string.IsNullOrEmpty(pPerfilPersonal.Email))
                //{
                //    listaErrores.Add("email", UtilIdiomas.GetText("COMADMINCMS", "ERRORCAMPOVACIO", UtilIdiomas.GetText("REGISTROUSUARIOORGANIZACION", "EMAIL").Replace(" *", "")));
                //    existenErrores = true;
                //}
                //else if (personaCN.ExisteEmail(pPerfilPersonal.Email, IdentidadActual.Persona.Clave))
                //{
                //    listaErrores.Add("email", UtilIdiomas.GetText("PERFIL", "ERRORMAIL"));
                //    existenErrores = true;
                //}
                //else if (!UtilCadenas.ValidarEmail(pPerfilPersonal.Email))
                //{
                //    listaErrores.Add("formatoEmail", UtilIdiomas.GetText("REGISTRO", "ERRORFORMATOEMAIL"));
                //    existenErrores = true;
                //}

                //Para los Emails de Tutores
                if (!string.IsNullOrEmpty(pPerfilPersonal.Email))
                {
                    if (personaCN.ExisteEmail(pPerfilPersonal.Email, IdentidadActual.Persona.Clave))
                    {
                        listaErrores.Add("email", UtilIdiomas.GetText("PERFIL", "ERRORMAIL"));
                        existenErrores = true;
                    }
                    else if (!UtilCadenas.ValidarEmail(pPerfilPersonal.Email))
                    {
                        listaErrores.Add("formatoEmail", UtilIdiomas.GetText("REGISTRO", "ERRORFORMATOEMAIL"));
                        existenErrores = true;
                    }
                }

                if (string.IsNullOrEmpty(pPerfilPersonal.BornDate))
                {
                    listaErrores.Add("fechaNacimiento", UtilIdiomas.GetText("COMADMINCMS", "ERRORCAMPOVACIO", UtilIdiomas.GetText("REGISTRO", "FECHANACIMIENTO").Replace(" *", "")));
                    existenErrores = true;
                }

                if (!existenErrores)
                {
                    if (mEntityContext.Entry(IdentidadActual.Persona.FilaPersona).State == EntityState.Detached)
                    {
                        IdentidadActual.Persona.FilaPersona = personaCN.ObtenerFilaPersonaPorID(IdentidadActual.Persona.FilaPersona.PersonaID);
                    }

                    bool CambiadoNombre = (IdentidadActual.Persona.FilaPersona.Nombre != pPerfilPersonal.Name);
                    bool CambiadoApellidos = (IdentidadActual.Persona.FilaPersona.Apellidos != pPerfilPersonal.LastName);

                    if ((IdentidadActual.Persona.FilaPersona.EstadoCorreccion == (short)EstadoCorreccion.NotificadoNoCambiado || IdentidadActual.Persona.FilaPersona.EstadoCorreccion == (short)EstadoCorreccion.NotificadoNoCambiado3Dias) && (CambiadoNombre || CambiadoApellidos))
                    {
                        IdentidadActual.Persona.FilaPersona.EstadoCorreccion = (short)EstadoCorreccion.NotificadoCambiado;
                    }

                    IdentidadActual.Persona.FilaPersona.Nombre = pPerfilPersonal.Name.Trim();
                    IdentidadActual.Persona.FilaPersona.Apellidos = pPerfilPersonal.LastName.Trim();
                    IdentidadActual.Persona.FilaPersona.LocalidadPersonal = pPerfilPersonal.Location;

                    string antiguoEmail = IdentidadActual.Persona.FilaPersona.Email;
                    if (!string.IsNullOrEmpty(pPerfilPersonal.Email))
                    {
                        IdentidadActual.Persona.FilaPersona.Email = pPerfilPersonal.Email;
                    }

                    if (!string.IsNullOrEmpty(pPerfilPersonal.EmailTutor))
                    {
                        IdentidadActual.Persona.FilaPersona.EmailTutor = pPerfilPersonal.EmailTutor;
                    }

                    IdentidadActual.Persona.FilaPersona.Idioma = pPerfilPersonal.Lang;
                    IdentidadActual.Persona.FilaPersona.ProvinciaPersonal = "";

                    Guid provinciaID = Guid.Empty;
                    if (Guid.TryParse(pPerfilPersonal.Region, out provinciaID) && provinciaID != Guid.Empty)
                    {
                        IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID = provinciaID;
                    }
                    else
                    {
                        IdentidadActual.Persona.FilaPersona.ProvinciaPersonal = pPerfilPersonal.Region;
                        IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID = null;
                    }
                    IdentidadActual.Persona.FilaPersona.CPPersonal = pPerfilPersonal.PostalCode;

                    IdentidadActual.Persona.FilaPersona.PaisPersonalID = pPerfilPersonal.Country;

                    int? dia = null;
                    int? mes = null;
                    int? anio = null;

                    if (!string.IsNullOrEmpty(pPerfilPersonal.BornDate))
                    {
                        string[] fecha = pPerfilPersonal.BornDate.Split('/');

                        dia = int.Parse(fecha[0]);
                        mes = int.Parse(fecha[1]);
                        anio = int.Parse(fecha[2]);

                        IdentidadActual.Persona.FilaPersona.FechaNacimiento = new DateTime(anio.Value, mes.Value, dia.Value);
                    }

                    if (pPerfilPersonal.Sex != null)
                    {
                        IdentidadActual.Persona.FilaPersona.Sexo = pPerfilPersonal.Sex;
                    }

                    // Modificar los tags de nombre de persona
                    if (CambiadoNombre || CambiadoApellidos)
                    {
                        string profesorSexo = "";
                        if (IdentidadActual.Persona.Sexo.Equals("H"))
                        {
                            profesorSexo = UtilIdiomas.GetText("SOLICITUDESNUEVOSPROFESORES", "PROFESOR") + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " ";
                        }
                        else
                        {
                            profesorSexo = UtilIdiomas.GetText("SOLICITUDESNUEVOSPROFESORES", "PROFESORA") + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " ";
                        }

                        foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.PersonaID.Equals(mControladorBase.UsuarioActual.PersonaID)).ToList())
                        {
                            IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            string profesor = "";
                            if (IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaProfesor.Any(teacher => teacher.PerfilID.Equals(filaPerfil.PerfilID)))
                            {
                                profesor = profesorSexo;
                            }
                            filaPerfil.NombrePerfil = profesor + IdentidadActual.Persona.NombreConApellidos;

                            if (CambiadoNombre)
                            {
                                List<AD.EntityModel.Models.IdentidadDS.Identidad> listaIdentidadesEntity = idenCN.ObtenerListaIdentidadesPorPerfilID(filaPerfil.PerfilID);

                                foreach (AD.EntityModel.Models.IdentidadDS.Identidad identidadEntity in listaIdentidadesEntity)
                                {
                                    AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(item => item.IdentidadID.Equals(identidadEntity.IdentidadID)).FirstOrDefault();

                                    identidadEntity.NombreCortoIdentidad = profesor + IdentidadActual.Persona.Nombre;
                                    if(filaIdentidad != null)
                                    {
                                        filaIdentidad.NombreCortoIdentidad = profesor + IdentidadActual.Persona.Nombre;
                                    }
                                }
                            }
                        }
                    }

                    if (ParametrosGeneralesRow.PrivacidadObligatoria && ProyectoPrincipalUnico != ProyectoAD.MetaProyecto)
                    {
                        bool comprobarPerfil = true;
                        bool comprobarPerfilExterno = true;

                        // if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'VisibilidadPerfil'").Length == 1)
                        if (ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VisibilidadPerfil")).ToList().Count == 1)
                        {
                            //string visibilidadPerfil = (string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'VisibilidadPerfil'")[0]["Valor"];
                            string visibilidadPerfil = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VisibilidadPerfil")).ToList().First().Valor;

                            comprobarPerfil = visibilidadPerfil[0].ToString() == "1";
                            comprobarPerfilExterno = visibilidadPerfil[2].ToString() == "1";
                            IdentidadActual.Persona.FilaPersona.EsBuscable = visibilidadPerfil[1].ToString() == "1";
                            IdentidadActual.Persona.FilaPersona.EsBuscableExternos = visibilidadPerfil[3].ToString() == "1";
                        }

                        if (comprobarPerfil && pPerfilPersonal.IsSearched.HasValue)
                        {
                            IdentidadActual.Persona.FilaPersona.EsBuscable = pPerfilPersonal.IsSearched.Value;
                        }
                        if (comprobarPerfilExterno && pPerfilPersonal.IsExternalSearched.HasValue)
                        {
                            IdentidadActual.Persona.FilaPersona.EsBuscableExternos = pPerfilPersonal.IsExternalSearched.Value;

                            //si está marcado visible en todo internet implica que será visible en la comunidad
                            if (pPerfilPersonal.IsExternalSearched.HasValue && pPerfilPersonal.IsExternalSearched.Value)
                            {
                                IdentidadActual.Persona.FilaPersona.EsBuscable = true;
                            }
                        }
                    }

                    if (!MostrarDatosDemograficosPerfil)
                    {
                        IdentidadActual.Persona.PaisID = Guid.Empty;
                        IdentidadActual.Persona.Provincia = "";
                        IdentidadActual.Persona.Localidad = "";
                        IdentidadActual.Persona.CodPostal = "";
                    }
                    personaCN.Actualizar();

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);
                    IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(dataWrapperIdentidad);
                    identidadCN.Dispose();

                    Dictionary<int, string> dicDatosExtraProyectoVirtuoso = new Dictionary<int, string>();
                    Dictionary<int, string> dicDatosExtraEcosistemaVirtuoso = new Dictionary<int, string>();

                    GuardarDatosExtra(pPerfilPersonal, dicDatosExtraProyectoVirtuoso, dicDatosExtraEcosistemaVirtuoso);

                    //JsonEstado jsonEstado = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.Edicion, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.Persona.Nombre, IdentidadActual.Persona.Apellidos, IdentidadActual.Persona.FilaPersona.Email, "", ParametrosAplicacionDS, DatosExtraProyectoDS, dicDatosExtraEcosistemaVirtuoso, dicDatosExtraProyectoVirtuoso, antiguoEmail);
                    JsonEstado jsonEstado = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.Edicion, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.Persona.Nombre, IdentidadActual.Persona.Apellidos, IdentidadActual.Persona.FilaPersona.Email, "", GestorParametroAplicacion, DatosExtraProyectoDS, dicDatosExtraEcosistemaVirtuoso, dicDatosExtraProyectoVirtuoso, antiguoEmail);
                    if (jsonEstado != null && !jsonEstado.Correcto)
                    {
                        //Revierto los cambios y notifico del error                    
                        string error = UtilIdiomas.GetText("LOGIN", "AUTENTICACIONEXTERNA");
                        if (!string.IsNullOrEmpty(jsonEstado.InfoExtra))
                        {
                            error = jsonEstado.InfoExtra;
                        }
                        Session.SetString("errorServicioExterno", error);
                    }
                    else
                    {
                        mEntityContext.SaveChanges();
                    }

                    ControladorIdentidades.NotificarEdicionPerfilEnProyectos(TipoAccionExterna.Edicion, IdentidadActual.Persona.Clave, "", antiguoEmail, ProyectoSeleccionado.Clave);

                    if (personaCN != null)
                    {
                        personaCN.Dispose();
                        personaCN = null;
                    }

                    List<Guid> listaProyectos = new List<Guid>();
                    foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad)
                    {
                        if (!listaProyectos.Contains(filaIdentidad.ProyectoID))
                        {
                            listaProyectos.Add(filaIdentidad.ProyectoID);
                        }
                    }
                    if (!listaProyectos.Contains(ProyectoAD.MyGnoss))
                    {
                        listaProyectos.Add(ProyectoAD.MyGnoss);
                    }

                    ControladorPersonas contrPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    EliminarCaches();
                    contrPers.ActualizarModeloBaseSimpleMultiple(IdentidadActual.Persona.Clave, listaProyectos);

                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                listaErrores.Add("error", UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }

            return listaErrores;
        }

        private Dictionary<string, string> GuardarPerfilProfesional(EditProfileViewModel.ProfileProfesionalViewModel pPerfilProfesional)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            Dictionary<string, string> listaErrores = new Dictionary<string, string>();

            try
            {
                if (personaCN.ExisteEmail(pPerfilProfesional.Email, IdentidadActual.Persona.Clave))
                {
                    listaErrores.Add("email", UtilIdiomas.GetText("PERFIL", "ERRORMAIL"));
                }
                personaCN.Dispose();

                /*PersonaVinculoOrganizacion filaPersona = IdentidadActual.OrganizacionPerfil.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();
                filaPersona.EmailTrabajo = pPerfilProfesional.Email;
                */
                mEntityContext.PersonaVinculoOrganizacion.FirstOrDefault(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).EmailTrabajo = pPerfilProfesional.Email;

                mEntityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                listaErrores.Add("error", UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }

            return listaErrores;
        }

        private Dictionary<string, string> GuardarPerfilProfesor(EditProfileViewModel.ProfileTeacherViewModel pPerfilProfesor)
        {
            Dictionary<string, string> listaErrores = new Dictionary<string, string>();

            try
            {
                AD.EntityModel.Models.IdentidadDS.Profesor filaProfesor = IdentidadActual.PerfilUsuario.FilaPerfil.Profesor.First();

                filaProfesor.Email = pPerfilProfesor.Email;
                if (!string.IsNullOrEmpty(pPerfilProfesor.StudiesCentre))
                {
                    filaProfesor.CentroEstudios = pPerfilProfesor.StudiesCentre;
                }

                if (!string.IsNullOrEmpty(pPerfilProfesor.Departament))
                {
                    filaProfesor.AreaEstudios = pPerfilProfesor.Departament;
                }

                mEntityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                listaErrores.Add("error", UtilIdiomas.GetText("COMMON", "CAMBIOSINCORRECTOS"));
            }

            return listaErrores;
        }

        private void GuardarDatosExtra(EditProfileViewModel.ProfilePersonalViewModel pPerfilPersonal, Dictionary<int, string> pDicDatosExtraProyectoVirtuoso, Dictionary<int, string> pDicDatosExtraEcosistemaVirtuoso)
        {
            DataWrapperIdentidad dataWrapperIdentidad = IdentidadActual.GestorIdentidades.DataWrapperIdentidad;
            //cargar el gestor
            if (IdentidadActual.GestorIdentidades.GestorUsuarios == null)
            {
                CargarClausulasRegistrosOpcionales();
            }
            DataWrapperUsuario usuarioDW = IdentidadActual.GestorIdentidades.GestorUsuarios.DataWrapperUsuario;

            List<DatoExtraProyectoOpcionIdentidad> listaDatoExtraProyectoOpcionIdentidad = dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.ToList();
            foreach (DatoExtraProyectoOpcionIdentidad filaDatoExtra in listaDatoExtraProyectoOpcionIdentidad)
            {
                if (filaDatoExtra.IdentidadID == IdentidadActual.Clave)
                {
                    mEntityContext.EliminarElemento(filaDatoExtra);
                    dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.Remove(filaDatoExtra);
                }
            }

            List<AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad> listaDatoExtraProyectoVirtuosoIdentidadBorrar = dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad.ToList();
            foreach (AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad filaDatoExtraVirtuoso in listaDatoExtraProyectoVirtuosoIdentidadBorrar)
            {
                if (filaDatoExtraVirtuoso.IdentidadID == IdentidadActual.Clave)
                {
                    mEntityContext.EliminarElemento(filaDatoExtraVirtuoso);
                    dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad.Remove(filaDatoExtraVirtuoso);
                }
            }

            List<AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil> listaDatoExtraEcosistemaOpcionPerfilBorrar = dataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil.ToList();
            foreach (AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil filaDatoExtraEcosistema in listaDatoExtraEcosistemaOpcionPerfilBorrar)
            {
                if (filaDatoExtraEcosistema.PerfilID == IdentidadActual.PerfilID)
                {
                    mEntityContext.EliminarElemento(filaDatoExtraEcosistema);
                    dataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil.Remove(filaDatoExtraEcosistema);
                }
            }

            List<AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil> listaDatoExtraEcosistemaVirtuosoPerfilBorrar = dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.ToList();
            foreach (AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil filaDatoExtraEcosistemaVirtuoso in listaDatoExtraEcosistemaVirtuosoPerfilBorrar)
            {
                if (filaDatoExtraEcosistemaVirtuoso.PerfilID == IdentidadActual.PerfilID)
                {
                    mEntityContext.EliminarElemento(filaDatoExtraEcosistemaVirtuoso);
                    dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.Remove(filaDatoExtraEcosistemaVirtuoso);
                }
            }

            Dictionary<Guid, Guid> dicDatosExtraProyecto = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Guid> dicDatosExtraEcosistema = new Dictionary<Guid, Guid>();
            List<Guid> listaClausulasRegistro = new List<Guid>();

            //if(paginaModel.ProfilePersonal != null)
            // { 
            if (paginaModel.ProfilePersonal != null)
            {

                foreach (EditProfileViewModel.AdditionalClause clausulaAdicional in paginaModel.ProfilePersonal.AdditionalClauses)
                {
                    AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg fila = usuarioDW.ListaProyRolUsuClausulaReg.Where(item => item.ClausulaID.Equals(clausulaAdicional.Id) && item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.OrganizacionGnossID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.UsuarioID.Equals(mControladorBase.UsuarioActual.UsuarioID)).FirstOrDefault();

                    bool valor = clausulaAdicional.Checked;
                    bool.TryParse(RequestParams(clausulaAdicional.Id.ToString()), out valor);

                    if (fila != null)
                    {
                        fila.Valor = valor;
                    }
                    else
                    {
                        AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg proyRolUsuClausulaReg = new AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg();
                        proyRolUsuClausulaReg.ClausulaID = clausulaAdicional.Id;
                        proyRolUsuClausulaReg.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        proyRolUsuClausulaReg.ProyectoID = ProyectoSeleccionado.Clave;
                        proyRolUsuClausulaReg.OrganizacionGnossID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        proyRolUsuClausulaReg.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                        proyRolUsuClausulaReg.Valor = valor;

                        usuarioDW.ListaProyRolUsuClausulaReg.Add(proyRolUsuClausulaReg);
                        mEntityContext.ProyRolUsuClausulaReg.Add(proyRolUsuClausulaReg);
                    }
                }
            }
            if (pPerfilPersonal.AdditionalFields != null)
            {
                foreach (AdditionalFieldAutentication campoExtra in pPerfilPersonal.AdditionalFields)
                {
                    string nombreCampo = campoExtra.FieldName;
                    string valorCampo = RequestParams(nombreCampo);// || campoExtra.FieldValue;

                    Guid guidNombreCampo = Guid.Empty;
                    if (Guid.TryParse(nombreCampo, out guidNombreCampo))
                    {
                        DatoExtraEcosistema filaDatoExtraEcosistema = DatosExtraProyectoDS.ListaDatoExtraEcosistema.FirstOrDefault(dato => dato.DatoExtraID.Equals(guidNombreCampo));
                        if (filaDatoExtraEcosistema != null && !valorCampo.Equals(Guid.Empty.ToString()))
                        {
                            dicDatosExtraEcosistema.Add(filaDatoExtraEcosistema.DatoExtraID, new Guid(valorCampo));
                        }
                        else
                        {
                            //FindByOrganizacionIDProyectoIDDatoExtraID(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, guidNombreCampo);
                            DatoExtraProyecto filaDatoExtraProyecto = DatosExtraProyectoDS.ListaDatoExtraProyecto.FirstOrDefault();
                            if (filaDatoExtraProyecto != null && !valorCampo.Equals(Guid.Empty.ToString()))
                            {
                                dicDatosExtraProyecto.Add(filaDatoExtraProyecto.DatoExtraID, new Guid(valorCampo));
                            }
                        }
                    }
                    else
                    {
                        List<DatoExtraEcosistemaVirtuoso> filasDatoExtraEcosistemaVirtuoso = DatosExtraProyectoDS.ListaDatoExtraEcosistemaVirtuoso.Where(dato => dato.InputID.Equals(nombreCampo)).ToList();
                        if (filasDatoExtraEcosistemaVirtuoso.Count > 0 && !string.IsNullOrEmpty(valorCampo))
                        {
                            pDicDatosExtraEcosistemaVirtuoso.Add(filasDatoExtraEcosistemaVirtuoso[0].Orden, valorCampo);
                        }
                        else
                        {
                            List<DatoExtraProyectoVirtuoso> filasDatoExtraProyectoVirtuoso = DatosExtraProyectoDS.ListaDatoExtraProyectoVirtuoso.Where(dato => dato.InputID.Equals(nombreCampo)).ToList();
                            if (filasDatoExtraProyectoVirtuoso.Count > 0 && !string.IsNullOrEmpty(valorCampo))
                            {
                                pDicDatosExtraProyectoVirtuoso.Add(filasDatoExtraProyectoVirtuoso[0].Orden, valorCampo);
                            }
                        }
                    }
                }
            }
            foreach (Guid datoExtra in dicDatosExtraProyecto.Keys)
            {
                DatoExtraProyectoOpcionIdentidad datoExtraProyectoOpcionIdentidad = new DatoExtraProyectoOpcionIdentidad();
                datoExtraProyectoOpcionIdentidad.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                datoExtraProyectoOpcionIdentidad.ProyectoID = ProyectoSeleccionado.Clave;
                datoExtraProyectoOpcionIdentidad.DatoExtraID = datoExtra;
                datoExtraProyectoOpcionIdentidad.OpcionID = dicDatosExtraProyecto[datoExtra];
                datoExtraProyectoOpcionIdentidad.IdentidadID = IdentidadActual.Clave;
				//dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.AddDatoExtraProyectoOpcionIdentidadRow(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, datoExtra, dicDatosExtraProyecto[datoExtra], IdentidadActual.FilaIdentidad);
				dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.Add(datoExtraProyectoOpcionIdentidad);
                mEntityContext.DatoExtraProyectoOpcionIdentidad.Add(datoExtraProyectoOpcionIdentidad);
            }

            foreach (int orden in pDicDatosExtraProyectoVirtuoso.Keys)
            {
                if (!string.IsNullOrEmpty(pDicDatosExtraProyectoVirtuoso[orden].Trim()) && pDicDatosExtraProyectoVirtuoso[orden].Trim() != "|")
                {
                    string valor = pDicDatosExtraProyectoVirtuoso[orden].Trim();
                    if (valor.EndsWith("|"))
                    {
                        valor = valor.Substring(0, valor.Length - 1);
                    }

                    valor = IntentoObtenerElPais(valor);

                    AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad datoExtraProyectoVirtuosoIdentidad = new AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad();
                    datoExtraProyectoVirtuosoIdentidad.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                    datoExtraProyectoVirtuosoIdentidad.ProyectoID = ProyectoSeleccionado.Clave;
                    datoExtraProyectoVirtuosoIdentidad.DatoExtraID = DatosExtraProyectoDS.ListaDatoExtraProyectoVirtuoso.FirstOrDefault(dato => dato.Orden.Equals(orden)).DatoExtraID;
                    datoExtraProyectoVirtuosoIdentidad.Opcion = valor;
                    datoExtraProyectoVirtuosoIdentidad.IdentidadID = IdentidadActual.FilaIdentidad.IdentidadID;
                    dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad.Add(datoExtraProyectoVirtuosoIdentidad);
                    mEntityContext.DatoExtraProyectoVirtuosoIdentidad.Add(datoExtraProyectoVirtuosoIdentidad);
                }
            }

            foreach (Guid datoExtra in dicDatosExtraEcosistema.Keys)
            {
                AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil datoExtraEcosistemaOpcionPerfil = new AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil();
                datoExtraEcosistemaOpcionPerfil.DatoExtraID = datoExtra;
                datoExtraEcosistemaOpcionPerfil.OpcionID = dicDatosExtraEcosistema[datoExtra];
                datoExtraEcosistemaOpcionPerfil.PerfilID = IdentidadActual.PerfilUsuario.FilaPerfil.PerfilID;
                dataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil.Add(datoExtraEcosistemaOpcionPerfil);
                mEntityContext.DatoExtraEcosistemaOpcionPerfil.Add(datoExtraEcosistemaOpcionPerfil);
            }

            foreach (int orden in pDicDatosExtraEcosistemaVirtuoso.Keys)
            {
                if (!string.IsNullOrEmpty(pDicDatosExtraEcosistemaVirtuoso[orden].Trim()) && pDicDatosExtraEcosistemaVirtuoso[orden].Trim() != "|")
                {
                    string valor = pDicDatosExtraEcosistemaVirtuoso[orden].Trim();
                    if (valor.EndsWith("|"))
                    {
                        valor = valor.Substring(0, valor.Length - 1);
                    }

                    valor = IntentoObtenerElPais(valor);

                    AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil datoExtraEcosistemaVirtuosoPerfil = new AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil();
                    datoExtraEcosistemaVirtuosoPerfil.DatoExtraID = DatosExtraProyectoDS.ListaDatoExtraEcosistemaVirtuoso.First(dato => dato.Orden.Equals(orden)).DatoExtraID;
                    datoExtraEcosistemaVirtuosoPerfil.PerfilID = IdentidadActual.PerfilUsuario.FilaPerfil.PerfilID;
                    datoExtraEcosistemaVirtuosoPerfil.Opcion = valor;
                    dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.Add(datoExtraEcosistemaVirtuosoPerfil);
                    mEntityContext.DatoExtraEcosistemaVirtuosoPerfil.Add(datoExtraEcosistemaVirtuosoPerfil);
                }
            }
        }


        private void GuardarDatosExtraNulos(EditProfileViewModel.ProfilePersonalViewModel pPerfilPersonal, Dictionary<int, string> pDicDatosExtraProyectoVirtuoso, Dictionary<int, string> pDicDatosExtraEcosistemaVirtuoso)
        {

            DataWrapperIdentidad identidadDW = IdentidadActual.GestorIdentidades.DataWrapperIdentidad;
            //cargar el gestor
            if (IdentidadActual.GestorIdentidades.GestorUsuarios == null)
            {
                CargarClausulasRegistrosOpcionales();
            }
            DataWrapperUsuario usuarioDW = IdentidadActual.GestorIdentidades.GestorUsuarios.DataWrapperUsuario;
            List<DatoExtraProyectoOpcionIdentidad> listaDatoExtraProyectoOpcionIdentidad = identidadDW.ListaDatoExtraProyectoOpcionIdentidad;
            foreach (DatoExtraProyectoOpcionIdentidad filaDatoExtra in listaDatoExtraProyectoOpcionIdentidad)
            {
                if (filaDatoExtra.IdentidadID == IdentidadActual.Clave)
                {
                    string nombreCampo = filaDatoExtra.DatoExtraID.ToString();
                    string valorCampo = RequestParams(nombreCampo);
                    if (!string.IsNullOrEmpty(valorCampo))
                    {
                        mEntityContext.EliminarElemento(filaDatoExtra);
                        identidadDW.ListaDatoExtraProyectoOpcionIdentidad.Remove(filaDatoExtra);
                    }
                }
            }
            List<AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad> listaDatoExtraProyectoVirtuosoIdentidad = identidadDW.ListaDatoExtraProyectoVirtuosoIdentidad;
            foreach (AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad filaDatoExtraVirtuoso in listaDatoExtraProyectoVirtuosoIdentidad)
            {
                if (filaDatoExtraVirtuoso.IdentidadID == IdentidadActual.Clave)
                {
                    string nombreCampo = filaDatoExtraVirtuoso.DatoExtraID.ToString();
                    string valorCampo = RequestParams(nombreCampo);
                    if (!string.IsNullOrEmpty(valorCampo))
                    {
                        mEntityContext.EliminarElemento(filaDatoExtraVirtuoso);
                        identidadDW.ListaDatoExtraProyectoVirtuosoIdentidad.Remove(filaDatoExtraVirtuoso);
                    }
                }
            }


            List<AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil> listaDatoExtraEcosistemaOpcionPerfil = identidadDW.ListaDatoExtraEcosistemaOpcionPerfil;
            foreach (AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil filaDatoExtraEcosistema in listaDatoExtraEcosistemaOpcionPerfil)
            {
                if (filaDatoExtraEcosistema.PerfilID == IdentidadActual.PerfilID)
                {
                    string nombreCampo = filaDatoExtraEcosistema.DatoExtraID.ToString();
                    string valorCampo = RequestParams(nombreCampo);
                    if (!string.IsNullOrEmpty(valorCampo))
                    {
                        identidadDW.ListaDatoExtraEcosistemaOpcionPerfil.Remove(filaDatoExtraEcosistema);
                        mEntityContext.EliminarElemento(filaDatoExtraEcosistema);
                    }
                }
            }

            List<AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil> listaDatoExtraEcosistemaVirtuosoPerfil = identidadDW.ListaDatoExtraEcosistemaVirtuosoPerfil;
            foreach (AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil filaDatoExtraEcosistemaVirtuoso in identidadDW.ListaDatoExtraEcosistemaVirtuosoPerfil)
            {
                if (filaDatoExtraEcosistemaVirtuoso.PerfilID == IdentidadActual.PerfilID)
                {
                    string nombreCampo = filaDatoExtraEcosistemaVirtuoso.DatoExtraID.ToString();
                    string valorCampo = RequestParams(nombreCampo);
                    if (!string.IsNullOrEmpty(valorCampo))
                    {
                        identidadDW.ListaDatoExtraEcosistemaVirtuosoPerfil.Remove(filaDatoExtraEcosistemaVirtuoso);
                        mEntityContext.EliminarElemento(filaDatoExtraEcosistemaVirtuoso);
                    }
                }
            }

            Dictionary<Guid, Guid> dicDatosExtraProyecto = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Guid> dicDatosExtraEcosistema = new Dictionary<Guid, Guid>();
            List<Guid> listaClausulasRegistro = new List<Guid>();

            if (paginaModel.ProfilePersonal != null)
            {
                foreach (EditProfileViewModel.AdditionalClause clausulaAdicional in paginaModel.ProfilePersonal.AdditionalClauses)
                {
                    AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg fila = usuarioDW.ListaProyRolUsuClausulaReg.Where(item => item.ClausulaID.Equals(clausulaAdicional.Id) && item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.OrganizacionGnossID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.UsuarioID.Equals(mControladorBase.UsuarioActual.UsuarioID)).FirstOrDefault();

                    bool valor = clausulaAdicional.Checked;
                    bool.TryParse(RequestParams(clausulaAdicional.Id.ToString()), out valor);

                    if (fila != null)
                    {
                        fila.Valor = valor;
                    }
                    else
                    {
                        AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg proyRolUsuClausulaReg = new AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg();
                        proyRolUsuClausulaReg.ClausulaID = clausulaAdicional.Id;
                        proyRolUsuClausulaReg.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        proyRolUsuClausulaReg.ProyectoID = ProyectoSeleccionado.Clave;
                        proyRolUsuClausulaReg.OrganizacionGnossID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        proyRolUsuClausulaReg.UsuarioID = mControladorBase.UsuarioActual.UsuarioID;
                        proyRolUsuClausulaReg.Valor = valor;

                        mEntityContext.ProyRolUsuClausulaReg.Add(proyRolUsuClausulaReg);
                        usuarioDW.ListaProyRolUsuClausulaReg.Add(proyRolUsuClausulaReg);
                    }
                }
            }
            if (pPerfilPersonal.AdditionalFields != null)
            {
                foreach (AdditionalFieldAutentication campoExtra in pPerfilPersonal.AdditionalFields)
                {
                    string nombreCampo = campoExtra.FieldName;
                    string valorCampo = RequestParams(nombreCampo);// || campoExtra.FieldValue;
                    if (string.IsNullOrEmpty(valorCampo))
                    {
                        valorCampo = campoExtra.FieldValue;

                    }
                    Guid guidNombreCampo = Guid.Empty;
                    if (Guid.TryParse(nombreCampo, out guidNombreCampo) && !string.IsNullOrEmpty(valorCampo))
                    {
                        DatoExtraEcosistema filaDatoExtraEcosistema = DatosExtraProyectoDS.ListaDatoExtraEcosistema.FirstOrDefault(dato => dato.DatoExtraID.Equals(guidNombreCampo));
                        if (filaDatoExtraEcosistema != null && !valorCampo.Equals(Guid.Empty.ToString()))
                        {
                            dicDatosExtraEcosistema.Add(filaDatoExtraEcosistema.DatoExtraID, new Guid(valorCampo));
                        }
                        else
                        {
                            DatoExtraProyecto filaDatoExtraProyecto = DatosExtraProyectoDS.ListaDatoExtraProyecto.FirstOrDefault(dato => dato.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && dato.ProyectoID.Equals(ProyectoSeleccionado.Clave) && dato.DatoExtraID.Equals(guidNombreCampo));
                            if (filaDatoExtraProyecto != null && !valorCampo.Equals(Guid.Empty.ToString()))
                            {
                                dicDatosExtraProyecto.Add(filaDatoExtraProyecto.DatoExtraID, new Guid(valorCampo));
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(valorCampo))
                    {
                        List<AD.EntityModel.Models.ProyectoDS.DatoExtraEcosistemaVirtuoso> filasDatoExtraEcosistemaVirtuoso = DatosExtraProyectoDS.ListaDatoExtraEcosistemaVirtuoso.Where(dato => dato.InputID.Equals(nombreCampo)).ToList();
                        if (filasDatoExtraEcosistemaVirtuoso.Count > 0 && !string.IsNullOrEmpty(valorCampo))
                        {
                            pDicDatosExtraEcosistemaVirtuoso.Add(filasDatoExtraEcosistemaVirtuoso[0].Orden, valorCampo);
                        }
                        else
                        {
                            List<AD.EntityModel.Models.ProyectoDS.DatoExtraProyectoVirtuoso> filasDatoExtraProyectoVirtuoso = DatosExtraProyectoDS.ListaDatoExtraProyectoVirtuoso.Where(dato => dato.InputID.Equals(nombreCampo)).ToList();
                            if (filasDatoExtraProyectoVirtuoso.Count > 0 && !string.IsNullOrEmpty(valorCampo))
                            {
                                pDicDatosExtraProyectoVirtuoso.Add(filasDatoExtraProyectoVirtuoso[0].Orden, valorCampo);
                            }
                        }
                    }
                }
            }
            foreach (Guid datoExtra in dicDatosExtraProyecto.Keys)
            {
                DatoExtraProyectoOpcionIdentidad datoExtraProyectoOpcionIdentidad = new DatoExtraProyectoOpcionIdentidad();
                datoExtraProyectoOpcionIdentidad.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                datoExtraProyectoOpcionIdentidad.ProyectoID = ProyectoSeleccionado.Clave;
                datoExtraProyectoOpcionIdentidad.DatoExtraID = datoExtra;
                datoExtraProyectoOpcionIdentidad.IdentidadID = IdentidadActual.FilaIdentidad.PerfilID;
                datoExtraProyectoOpcionIdentidad.OpcionID = dicDatosExtraProyecto[datoExtra];

                identidadDW.ListaDatoExtraProyectoOpcionIdentidad.Add(datoExtraProyectoOpcionIdentidad);
            }


            foreach (int orden in pDicDatosExtraProyectoVirtuoso.Keys)
            {
                if (!string.IsNullOrEmpty(pDicDatosExtraProyectoVirtuoso[orden].Trim()) && pDicDatosExtraProyectoVirtuoso[orden].Trim() != "|")
                {
                    string valor = pDicDatosExtraProyectoVirtuoso[orden].Trim();
                    if (valor.EndsWith("|"))
                    {
                        valor = valor.Substring(0, valor.Length - 1);
                    }

                    valor = IntentoObtenerElPais(valor);

                    AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad proyectoVirtuosoIdentidad = new AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad();
                    proyectoVirtuosoIdentidad.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                    proyectoVirtuosoIdentidad.ProyectoID = ProyectoSeleccionado.Clave;
                    proyectoVirtuosoIdentidad.DatoExtraID = DatosExtraProyectoDS.ListaDatoExtraProyectoVirtuoso.FirstOrDefault(dato => dato.Orden.Equals(orden)).DatoExtraID;
                    proyectoVirtuosoIdentidad.IdentidadID = IdentidadActual.FilaIdentidad.PerfilID;
                    proyectoVirtuosoIdentidad.Opcion = valor;

                    identidadDW.ListaDatoExtraProyectoVirtuosoIdentidad.Add(proyectoVirtuosoIdentidad);
                }
            }

            foreach (Guid datoExtra in dicDatosExtraEcosistema.Keys)
            {
                AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil datoExtraEcosistemaOpcionPerfil = new AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil();
                datoExtraEcosistemaOpcionPerfil.DatoExtraID = datoExtra;
                datoExtraEcosistemaOpcionPerfil.PerfilID = IdentidadActual.PerfilUsuario.FilaPerfil.PerfilID;
                datoExtraEcosistemaOpcionPerfil.OpcionID = dicDatosExtraEcosistema[datoExtra];

                identidadDW.ListaDatoExtraEcosistemaOpcionPerfil.Add(datoExtraEcosistemaOpcionPerfil);
            }

            foreach (int orden in pDicDatosExtraEcosistemaVirtuoso.Keys)
            {
                if (!string.IsNullOrEmpty(pDicDatosExtraEcosistemaVirtuoso[orden].Trim()) && pDicDatosExtraEcosistemaVirtuoso[orden].Trim() != "|")
                {
                    string valor = pDicDatosExtraEcosistemaVirtuoso[orden].Trim();
                    if (valor.EndsWith("|"))
                    {
                        valor = valor.Substring(0, valor.Length - 1);
                    }

                    valor = IntentoObtenerElPais(valor);
                    AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil datoExtraEcosistemaVirtuosoPerfil = new AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil();
                    datoExtraEcosistemaVirtuosoPerfil.DatoExtraID = DatosExtraProyectoDS.ListaDatoExtraEcosistemaVirtuoso.FirstOrDefault(dato => dato.Orden.Equals(orden)).DatoExtraID;
                    datoExtraEcosistemaVirtuosoPerfil.PerfilID = IdentidadActual.PerfilUsuario.FilaPerfil.PerfilID;
                    datoExtraEcosistemaVirtuosoPerfil.Opcion = valor;

                    identidadDW.ListaDatoExtraEcosistemaVirtuosoPerfil.Add(datoExtraEcosistemaVirtuosoPerfil);
                }
            }
        }

        /// <summary>
        /// Agrega el archivo a la BD.
        /// </summary>
        /// <param name="pArg">Argumentos</param>
        /// <returns>Código de devolución callback</returns>
        private void GuardarCVRapido(string pDescription, string pTags, bool permitirNulos)
        {
            string descp = null;
            string tags = null;
            CargarCurriculum();
            if (paginaModel.Curriculum != null)
            {
                if (paginaModel.Curriculum.Description == null)
                {
                    paginaModel.Curriculum.Description = "";
                }
                if (paginaModel.Curriculum.Tags == null)
                {
                    paginaModel.Curriculum.Tags = "";
                }
                descp = paginaModel.Curriculum.Description.Trim();
                tags = paginaModel.Curriculum.Tags.Trim();
            }

            if (!string.IsNullOrEmpty(pDescription))
            {
                descp = pDescription.Trim();
            }
            if (permitirNulos && !string.IsNullOrEmpty(pTags))
            {
                tags = pTags.Trim();
            }
            else if (!permitirNulos)
            {
                tags = pTags.Trim();
            }
            //DocumentoID y IDCV pra crear uno nuevo
            Guid DocumentoID = Guid.NewGuid();
            Guid IDCV = Guid.NewGuid();

            bool nuevoCurriculum = true;

            Guid curriculumID = Guid.Empty;

            if (CVUnicoPorPerfil)
            {
                if (IdentidadActual.PerfilUsuario.FilaPerfil.CurriculumID.HasValue)
                {
                    curriculumID = IdentidadActual.PerfilUsuario.FilaPerfil.CurriculumID.Value;
                }
            }
            else
            {
                if (IdentidadActual.FilaIdentidad.CurriculumID.HasValue)
                {
                    curriculumID = IdentidadActual.FilaIdentidad.CurriculumID.Value;
                }
            }

            if (!curriculumID.Equals(Guid.Empty))
            {
                nuevoCurriculum = false;
                IDCV = curriculumID;
            }

            List<string> listaTags = new List<string>();
            if (permitirNulos && !string.IsNullOrEmpty(tags))
            {
                listaTags = UtilCadenas.SepararTexto(tags);
            }
            else if (!permitirNulos)
            {
                listaTags = UtilCadenas.SepararTexto(tags);
            }
            string descripcion = descp;

            if ((string.IsNullOrEmpty(descp) || descp == "<p></p>") && !permitirNulos)
            {
                //En el rdf antiguo no había tags. Lanzar excepción y controlarla para mostrar un mensaje en el que se le pidan tags.
                throw new ArgumentNullException("No se ha agregado una descripción.");
            }
            else if (listaTags.Count == 0 && !permitirNulos)
            {
                //En el rdf antiguo no había tags. Lanzar excepción y controlarla para mostrar un mensaje en el que se le pidan tags.
                throw new ArgumentNullException("No se han agregado tags.");
            }
            if (paginaModel.Curriculum != null)
            {
                Curriculum curriculum = mEntityContext.Curriculum.FirstOrDefault(cv => cv.CurriculumID.Equals(IDCV));
                if(curriculum != null)
                {
                    curriculum.Tags = tags;
                    curriculum.Description = descp;
                }
                else
                {
                    curriculum = new Curriculum()
                    {
                        CurriculumID = IDCV,
                        Description = descp,
                        Tags = tags,
                        FechaPublicacion = DateTime.Now,
                    };
                    mEntityContext.Curriculum.Add(curriculum);
                }
            }
            GuardarDatosCVRapido(IDCV);

        }


        /// <summary>
        /// Guarda en BD el curriculum.
        /// </summary>
        private void GuardarDatosCVRapido(Guid pCVSeleccionado)
        {
            #region Actualizamos el ácido de la identidad

            List<AD.EntityModel.Models.IdentidadDS.Identidad> filas = null;
            if (CVUnicoPorPerfil)
            {
                IdentidadActual.PerfilUsuario.FilaPerfil.CurriculumID = pCVSeleccionado;

                filas = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(IdentidadActual.PerfilID)).ToList();

                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in filas)
                {
                    if (mEntityContext.Entry(filaIdent).State.Equals(EntityState.Detached))
                    {
                        AD.EntityModel.Models.IdentidadDS.Identidad identBD = mEntityContext.Identidad.FirstOrDefault(item => item.IdentidadID.Equals(filaIdent.IdentidadID));
                        identBD.CurriculumID = pCVSeleccionado;
                    }
                    filaIdent.CurriculumID = pCVSeleccionado;
                }
            }
            else
            {
                IdentidadActual.FilaIdentidad.CurriculumID = pCVSeleccionado;
                filas = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.CurriculumID.Equals(pCVSeleccionado)).ToList();
                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in filas)
                {
                    if (mEntityContext.Entry(filaIdent).State.Equals(EntityState.Detached))
                    {
                        AD.EntityModel.Models.IdentidadDS.Identidad identBD = mEntityContext.Identidad.FirstOrDefault(item => item.IdentidadID.Equals(filaIdent.IdentidadID));
                        identBD.CurriculumID = pCVSeleccionado;
                    }
                }
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.ActualizaIdentidades();
            identidadCN.Dispose();
            #endregion

            #region Actualizamos base de las identidades que tienen este curriculum

            if (filas != null)
            {
                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in filas)
                {
                    if (filaIdent.Tipo == 2 || filaIdent.Tipo == 3)
                    {
                        //Genero los nuevos
                        ControladorOrganizaciones controladorOrganizaciones2 = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        controladorOrganizaciones2.ActualizarModeloBASE(IdentidadActual.GestorIdentidades.ListaIdentidades[filaIdent.IdentidadID], filaIdent.ProyectoID, true, false, PrioridadBase.Alta);
                    }
                    else
                    {
                        //Genero los nuevos
                        ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                        controladorPersonas.ActualizarModeloBASE(IdentidadActual.GestorIdentidades.ListaIdentidades[filaIdent.IdentidadID], filaIdent.ProyectoID, true, false, PrioridadBase.Alta);

                    }
                }
            }
            #endregion

            #region Borramos caché

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCL.EliminarCacheGestorIdentidadActual(UsuarioActual.UsuarioID, UsuarioActual.IdentidadID, UsuarioActual.PersonaID);
            identidadCL.Dispose();

            #endregion
        }

        /// <summary>
        /// Guarda los datos del documento en la BD en formato RDF.
        /// <returns>TRUE si se ha podido guardar el formulario RDF, FALSE en caso contrario</returns>
        /// </summary>
        //private bool GuardarFormularioRDF(string pRDF, Guid pDocumentoID, Guid pIDCv, bool pEsNuevo)
        //{
        //    try
        //    {
        //        //string nombreTemporal = Path.GetRandomFileName() + ".rdf";
        //        //string ruta = Path.GetTempPath() + nombreTemporal;

        //        GestionOWL GestionOWL = new GestionOWL();
        //        GestionOWL.UrlOntologia = UrlOntologiaCurriculum;
        //        GestionOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_HR_XML;

        //        GeneradorPantillaOWL GeneradorOWL = new GeneradorPantillaOWL(Ontologia, true, new Dictionary<Propiedad, CampoOntologia>(), new Dictionary<ElementoOntologia, HtmlGenericControl>(), pDocumentoID, false, false, pIDCv);
        //        List<ElementoOntologia> entidadesPrincipales = GeneradorOWL.RecogerValoresRdf(pRDF);
        //        Stream stream = GestionOWL.PasarOWL(null, Ontologia, entidadesPrincipales, null, null);
        //        stream.Position = 0;

        //        string contenidoFichero = new StreamReader(stream).ReadToEnd();

        //        stream.Flush();
        //        //string contenidoFichero = System.IO.File.ReadAllText(ruta);
        //        //System.IO.File.Delete(ruta);

        //        if (!pEsNuevo)
        //        {
        //            //El RDF ya existia por lo que hay que remplazarlo:  
        //            ControladorDocumentacion.BorrarRDFDeVirtuoso(pDocumentoID, "Curriculum.owl", UrlIntragnoss, false, UsuarioActual.ProyectoID);
        //        }

        //        #region SemWeb

        //        RdfDS RdfDS = null;
        //        if (!pEsNuevo)
        //        {
        //            RdfDS = ControladorDocumentacion.ObtenerRDFDeBDRDF(pDocumentoID, ProyectoAD.MetaProyecto);
        //        }

        //        ControladorDocumentacion.GuardarRDFEnVirtuoso(entidadesPrincipales, "Curriculum.owl", UrlIntragnoss, null, mControladorBase.UsuarioActual.ProyectoID, pDocumentoID.ToString(), false, "", false, false, (short)PrioridadBase.Alta);
        //        ControladorDocumentacion.GuardarRDFEnBDRDF(contenidoFichero, pDocumentoID, ProyectoAD.MetaProyecto, RdfDS);

        //        #endregion

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        GuardarLogErrorAJAX(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
        //    }

        //    return false;
        //}

        private void UsarImagenPersonal(bool pUsarFotoPerfil)
        {
            if (IdentidadActual.TrabajaConOrganizacion)
            {
                PersonaVinculoOrganizacion filaPersona = IdentidadActual.OrganizacionPerfil.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();
                if (mEntityContext.Entry(filaPersona).State.Equals(EntityState.Detached))
                {
                    filaPersona = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();
                }
                filaPersona.UsarFotoPersonal = pUsarFotoPerfil;

                mEntityContext.SaveChanges();
            }
        }

        private void BorrarImagenRegistro()
        {
            string urlFicheroImagen = "";

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                urlFicheroImagen = UtilArchivos.ContentImagenesOrganizaciones + "/" + IdentidadActual.OrganizacionID.ToString();
            }
            else if (IdentidadActual.TrabajaConOrganizacion)
            {
                urlFicheroImagen = UtilArchivos.ContentImagenesPersona_Organizacion + "/" + IdentidadActual.OrganizacionID.ToString() + "/" + mControladorBase.UsuarioActual.PersonaID.ToString();
            }
            else if (IdentidadActual.EsIdentidadProfesor)
            {
                return;
            }
            else if (IdentidadActual.ModoPersonal)
            {
                urlFicheroImagen = UtilArchivos.ContentImagenesPersonas + "/" + mControladorBase.UsuarioActual.PersonaID.ToString();
            }

            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
            servicioImagenes.Url = UrlIntragnossServicios;

            //Si se sube un fichero nuevo se borra la foto temporal
            servicioImagenes.BorrarImagen(urlFicheroImagen + "_temp" + ".png");
            servicioImagenes.BorrarImagen(urlFicheroImagen + "_temp2" + ".png");
            servicioImagenes.BorrarImagen(urlFicheroImagen + ".png");
            servicioImagenes.BorrarImagen(urlFicheroImagen + "_peque" + ".png");
            servicioImagenes.BorrarImagen(urlFicheroImagen + "_grande" + ".png");

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                if (mEntityContext.Entry(IdentidadActual.OrganizacionPerfil.FilaOrganizacion).State.Equals(EntityState.Detached))
                {
                    IdentidadActual.OrganizacionPerfil.FilaElementoEntity = mEntityContext.Organizacion.FirstOrDefault(orga => orga.OrganizacionID.Equals(IdentidadActual.OrganizacionPerfil.FilaOrganizacion.OrganizacionID));
                }
                IdentidadActual.OrganizacionPerfil.FilaOrganizacion.Logotipo = null;
                IdentidadActual.OrganizacionPerfil.FilaOrganizacion.CoordenadasLogo = null;
                mEntityContext.SaveChanges();
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.ActualizarFotoIdentidadesPersona(IdentidadActual.PersonaID.Value, true);
                identidadCN.Dispose();
            }
            else if (IdentidadActual.TrabajaConOrganizacion)
            {
                PersonaVinculoOrganizacion filaPersona = IdentidadActual.OrganizacionPerfil.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();
                if (mEntityContext.Entry(filaPersona).State.Equals(EntityState.Detached))
                {
                    filaPersona = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();
                }
                filaPersona.Foto = null;
                filaPersona.CoordenadasFoto = null;
                filaPersona.FechaAnadidaFoto = null;
                mEntityContext.SaveChanges();
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.ActualizarFotoIdentidadesPersona(IdentidadActual.PersonaID.Value, true);
                identidadCN.Dispose();
            }
            else if (IdentidadActual.ModoPersonal)
            {
                if (mEntityContext.Entry(IdentidadActual.Persona.FilaPersona).State.Equals(EntityState.Detached))
                {
                    IdentidadActual.Persona.FilaPersona = mEntityContext.Persona.FirstOrDefault(pers => pers.PersonaID.Equals(IdentidadActual.Persona.FilaPersona.PersonaID));
                }
                IdentidadActual.Persona.FilaPersona.Foto = null;
                IdentidadActual.Persona.FilaPersona.CoordenadasFoto = null;
                IdentidadActual.Persona.FilaPersona.FechaAnadidaFoto = null;

                mEntityContext.SaveChanges();
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.ActualizarFotoIdentidadesPersona(IdentidadActual.PersonaID.Value, true);
                identidadCN.Dispose();
            }

            EliminarCaches();
        }

        private void EliminarCaches()
        {
            //TODO : Si se cambia la foto de una organización, hay que pedirle al servicio de refresco de cache que borre las caches de los miembros de la organización
            //TODO : Si se cambia algun dato de la organizacion, hay que pedirle al servicio de refresco de cache que borre las caches de los miembros de la organización

            List<string> listaClavesInvalidar = new List<string>();

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

            string prefijoClave;

            if (!string.IsNullOrEmpty(identidadCL.Dominio))
            {
                prefijoClave = identidadCL.Dominio;
            }
            else
            {
                prefijoClave = IdentidadCL.DominioEstatico;
            }


            prefijoClave = prefijoClave + "_" + identidadCL.ClaveCache[0]  + "_5.0.0.0_";
            prefijoClave = prefijoClave.ToLower();

            

            foreach (Guid perfilID in IdentidadActual.GestorIdentidades.ListaPerfiles.Keys)
            {
                string rawKey = string.Concat("IdentidadActual_", IdentidadActual.PersonaID, "_", perfilID);
                string rawKeyCache = identidadCL.ObtenerClaveCache(string.Concat("IdentidadActual_", IdentidadActual.PersonaID, "_", perfilID));
                string rawKey2Cache = identidadCL.ObtenerClaveCache("PerfilMVC_" + perfilID);
                string rawKey2 = "PerfilMVC_" + perfilID;
                listaClavesInvalidar.Add(rawKeyCache.ToLower());
                listaClavesInvalidar.Add(rawKey2Cache.ToLower());
            }
            identidadCL.InvalidarCachesMultiples(listaClavesInvalidar);

            List<Guid> listaIdentidadesInvalidar = new List<Guid>();
            foreach (Guid identidadID in IdentidadActual.GestorIdentidades.ListaIdentidades.Keys)
            {
                listaIdentidadesInvalidar.Add(identidadID);
            }
            identidadCL.InvalidarFichasIdentidadesMVC(listaIdentidadesInvalidar);


            identidadCL.Dispose();
        }

        private void CargarDatosProvincia(Guid pPaisID)
        {
            paginaModel.RegionList = new Dictionary<Guid, string>();
            foreach (AD.EntityModel.Models.Pais.Provincia fila in PaisesDW.ListaProvincia.Where(provincia => provincia.PaisID == pPaisID))
            {
                paginaModel.RegionList.Add(fila.ProvinciaID, fila.Nombre);
            }
        }

        private void CargarDatosPerfilPersonal()
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (mEntityContext.Entry(IdentidadActual.Persona.FilaPersona).State == EntityState.Detached)
            {
                IdentidadActual.Persona.FilaPersona = personaCN.ObtenerFilaPersonaPorID(IdentidadActual.Persona.FilaPersona.PersonaID);
            }
            personaCN.Dispose();
            paginaModel.ProfilePersonal.Name = IdentidadActual.Persona.Nombre;
            paginaModel.ProfilePersonal.LastName = IdentidadActual.Persona.Apellidos;

            string foto = UtilArchivos.ContentImagenes + IdentidadActual.UrlImagenGrande;

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                foto = UtilArchivos.ContentImagenes + IdentidadActual.IdentidadOrganizacion.UrlImagenGrande;
            }

            if (!string.IsNullOrEmpty(foto) && !foto.Contains("sinfoto") && !foto.Contains("anonimo"))
            {
                paginaModel.UrlFoto = foto;
            }

            if (MostrarDatosDemograficosPerfil)
            {
                if (IdentidadActual.Persona.FilaPersona.PaisPersonalID.HasValue)
                {
                    paginaModel.ProfilePersonal.Country = IdentidadActual.Persona.FilaPersona.PaisPersonalID.Value;
                }

                if (IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID.HasValue)
                {
                    paginaModel.ProfilePersonal.Region = IdentidadActual.Persona.FilaPersona.ProvinciaPersonalID.Value.ToString();
                }
                else
                {
                    paginaModel.ProfilePersonal.Region = IdentidadActual.Persona.FilaPersona.ProvinciaPersonal;
                }
                paginaModel.ProfilePersonal.PostalCode = IdentidadActual.Persona.FilaPersona.CPPersonal;
                paginaModel.ProfilePersonal.Location = IdentidadActual.Persona.FilaPersona.LocalidadPersonal;
                paginaModel.ProfilePersonal.Email = IdentidadActual.Persona.FilaPersona.Email;
                if (!string.IsNullOrEmpty(IdentidadActual.Persona.FilaPersona.EmailTutor))
                {
                    paginaModel.ProfilePersonal.EmailTutor = IdentidadActual.Persona.FilaPersona.EmailTutor;
                }
                paginaModel.ProfilePersonal.Sex = IdentidadActual.Persona.FilaPersona.Sexo;
                if (IdentidadActual.Persona.FilaPersona.FechaNacimiento.HasValue)
                {
                    paginaModel.ProfilePersonal.BornDate = IdentidadActual.Persona.FilaPersona.FechaNacimiento.Value.ToString("dd/MM/yyyy");
                }
                paginaModel.ProfilePersonal.Lang = IdentidadActual.Persona.FilaPersona.Idioma;

                paginaModel.CountryList = new Dictionary<Guid, string>();
                foreach (AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
                {
                    paginaModel.CountryList.Add(filaPais.PaisID, filaPais.Nombre);
                }

                CargarDatosProvincia(IdentidadActual.Persona.PaisID);
            }

            if (ParametrosGeneralesRow.PrivacidadObligatoria && ProyectoPrincipalUnico != ProyectoAD.MetaProyecto)
            {
                paginaModel.ProfilePersonal.IsSearched = IdentidadActual.Persona.FilaPersona.EsBuscable;
                paginaModel.ProfilePersonal.IsExternalSearched = IdentidadActual.Persona.FilaPersona.EsBuscableExternos;

                // if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'VisibilidadPerfil'").Length == 1)
                List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VisibilidadPerfil")).ToList();
                if (busqueda.Count == 1)
                {
                    //cadena de 4 digitos, los dos primeros configuran visibilidad en la comunidad, los dos últimos visibilidad en todo internet
                    //de cada pareja de digitos, el primero indica si el check es visible y el segundo si está marcado
                    //ej.: 0111 -> es visible únicamente el check de "visible en todo internet" pero ambos están marcados
                    //string visibilidadPerfil = (string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'VisibilidadPerfil'")[0]["Valor"];
                    string visibilidadPerfil = busqueda.First().Valor;

                    if (visibilidadPerfil[0].ToString() == "0")
                    {
                        paginaModel.ProfilePersonal.IsSearched = null;
                    }
                    if (visibilidadPerfil[2].ToString() == "0")
                    {
                        paginaModel.ProfilePersonal.IsExternalSearched = null;
                    }
                }
            }

            CargarCamposExtraRegistro();

            CargarEdadMinimaRegistro();
        }

        private void CargarDatosPerfilProfesor()
        {
            string foto = UtilArchivos.ContentImagenes + IdentidadActual.UrlImagenGrande;
            if (!string.IsNullOrEmpty(foto) && !foto.Contains("sinfoto") && !foto.Contains("anonimo"))
            {
                paginaModel.UrlFoto = foto;
            }

            AD.EntityModel.Models.IdentidadDS.Profesor filaProfe = IdentidadActual.PerfilUsuario.FilaPerfil.Profesor.FirstOrDefault();
            paginaModel.ProfileTeacher.StudiesCentre = filaProfe.CentroEstudios;
            paginaModel.ProfileTeacher.Departament = filaProfe.AreaEstudios;
            paginaModel.ProfileTeacher.Email = filaProfe.Email;
        }

        private void CargarDatosPerfilProfesional()
        {
            PersonaVinculoOrganizacion filaPersona = IdentidadActual.OrganizacionPerfil.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(IdentidadActual.Persona.Clave) && item.OrganizacionID.Equals(IdentidadActual.OrganizacionID.Value)).ToList().FirstOrDefault();

            paginaModel.ProfileProfesional.Email = filaPersona.EmailTrabajo;

            paginaModel.UsarFotoPersonal = filaPersona.UsarFotoPersonal;

            string foto = UtilArchivos.ContentImagenes + IdentidadActual.UrlImagenGrande;
            if (!string.IsNullOrEmpty(foto) && !foto.Contains("sinfoto") && !foto.Contains("anonimo"))
            {
                paginaModel.UrlFoto = foto;
            }

        }

        private void CargarDatosPerfilOrganizacion()
        {
            string foto = UtilArchivos.ContentImagenes + IdentidadActual.IdentidadOrganizacion.UrlImagenGrande;
            if (!string.IsNullOrEmpty(foto) && !foto.Contains("sinfoto") && !foto.Contains("anonimo"))
            {
                paginaModel.UrlFoto = foto;
            }

            paginaModel.ProfileOrganization.Country = new Guid("98d604b4-3141-4499-bde1-c320f09ef45c");
            if (IdentidadActual.OrganizacionPerfil.FilaOrganizacion.PaisID != null)
            {
                paginaModel.ProfileOrganization.Country = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.PaisID.Value;
            }
            if (IdentidadActual.OrganizacionPerfil.FilaOrganizacion.ProvinciaID != null)
            {
                paginaModel.ProfileOrganization.Region = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.ProvinciaID.ToString();
            }
            else
            {
                paginaModel.ProfileOrganization.Region = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.Provincia;
            }

            paginaModel.CountryList = new Dictionary<Guid, string>();
            foreach (AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
            {
                paginaModel.CountryList.Add(filaPais.PaisID, filaPais.Nombre);
            }

            CargarDatosProvincia(paginaModel.ProfileOrganization.Country);

            paginaModel.ProfileOrganization.PostalCode = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.CP;
            paginaModel.ProfileOrganization.Address = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.Direccion;
            paginaModel.ProfileOrganization.Location = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.Localidad;
            paginaModel.ProfileOrganization.NameOrganization = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.Nombre;
            paginaModel.ProfileOrganization.WebSite = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.Web;

            if (IdentidadActual.OrganizacionPerfil.FilaOrganizacion.Alias != null)
            {
                paginaModel.ProfileOrganization.Alias = IdentidadActual.OrganizacionPerfil.FilaOrganizacion.Alias;
            }

            bool organizacionEsClase = IdentidadActual.OrganizacionPerfil is OrganizacionClase;
            paginaModel.ProfileOrganization.IsClass = organizacionEsClase;

            if (!organizacionEsClase)
            {

                Dictionary<TiposOrganizacion, string> listadoTiposOrg = OrganizacionCN.ObtenerListadoTiposOrganizacion();
                paginaModel.ProfileOrganization.ListTypesOrganization = new Dictionary<short, string>();
                foreach (TiposOrganizacion tipoOrg in listadoTiposOrg.Keys)
                {
                    if (!tipoOrg.Equals(TiposOrganizacion.Otros))
                    {
                        paginaModel.ProfileOrganization.ListTypesOrganization.Add((short)tipoOrg, listadoTiposOrg[tipoOrg]);
                    }
                }
                paginaModel.ProfileOrganization.ListTypesOrganization.Add((short)TiposOrganizacion.Otros, listadoTiposOrg[TiposOrganizacion.Otros]);

                Dictionary<SectoresOrganizacion, string> listadoSectores = OrganizacionCN.ObtenerListadoSectoresOrganizacion();
                paginaModel.ProfileOrganization.ListTypesSectors = new Dictionary<short, string>();
                foreach (SectoresOrganizacion sector in listadoSectores.Keys)
                {
                    if (!sector.Equals(SectoresOrganizacion.Otros))
                    {
                        paginaModel.ProfileOrganization.ListTypesSectors.Add((short)sector, listadoSectores[sector]);
                    }
                }
                paginaModel.ProfileOrganization.ListTypesSectors.Add((short)SectoresOrganizacion.Otros, listadoSectores[SectoresOrganizacion.Otros]);
            }
            else
            {
                OrganizacionClase clase = (OrganizacionClase)IdentidadActual.OrganizacionPerfil;
                paginaModel.ProfileOrganization.Centre = clase.Centro;
                paginaModel.ProfileOrganization.Subject = clase.Asignatura;
                paginaModel.ProfileOrganization.Course = clase.Curso;
                paginaModel.ProfileOrganization.Group = clase.Grupo;
            }
        }

        private void CargarCamposExtraRegistro()
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();
            if (IdentidadActual.TrabajaConOrganizacion || IdentidadActual.TrabajaPersonaConOrganizacion || IdentidadActual.OrganizacionID.HasValue)
            {
                DataWrapperIdentidad identidadesPersonas = identidadCN.ObtenerPerfilesDePersona(IdentidadActual.PersonaID.Value, true, IdentidadActual.Clave);
                if (identidadesPersonas.ListaIdentidad.Any(identidad => identidad.Tipo == 0))
                {
                    dataWrapperIdentidad = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(identidadesPersonas.ListaIdentidad.FirstOrDefault(Identidad => Identidad.Tipo == 0).IdentidadID);
                }
                else
                {
                    dataWrapperIdentidad = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);
                }
            }
            else
            {
                dataWrapperIdentidad = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);
            }

            identidadCN.Dispose();

            List<AdditionalFieldAutentication> CamposExtra = new List<AdditionalFieldAutentication>();

            if (DatosExtraProyectoDS.ListaDatoExtraEcosistema.Count > 0 || DatosExtraProyectoDS.ListaDatoExtraProyecto.Count > 0)
            {
                foreach (DatoExtraEcosistema fila in DatosExtraProyectoDS.ListaDatoExtraEcosistema.OrderBy(dato => dato.Orden))
                {
                    Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();
                    foreach (DatoExtraEcosistemaOpcion filaDatoExtraEcosistemaOpcion in DatosExtraProyectoDS.ListaDatoExtraEcosistemaOpcion.Where(dato => dato.DatoExtraID.Equals(fila.DatoExtraID)).OrderBy(dato => dato.Orden))
                    {
                        Guid opcionID = filaDatoExtraEcosistemaOpcion.OpcionID;
                        string opcion = filaDatoExtraEcosistemaOpcion.Opcion;

                        listaOpciones.Add(opcionID, opcion);
                    }

                    Guid opcionSeleccionada = Guid.Empty;
                    foreach (AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil filaDatoExtraEcosistemaOpcionPerfil in dataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil)
                    {
                        if (filaDatoExtraEcosistemaOpcionPerfil.DatoExtraID == fila.DatoExtraID)
                        {
                            opcionSeleccionada = filaDatoExtraEcosistemaOpcionPerfil.OpcionID;
                            break;
                        }
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, "es");
                    campoExtra.FieldName = fila.DatoExtraID.ToString();
                    campoExtra.Required = fila.Obligatorio;
                    campoExtra.Options = listaOpciones;
                    campoExtra.FieldValue = opcionSeleccionada.ToString();

                    CamposExtra.Add(campoExtra);
                }

                foreach (DatoExtraProyecto fila in DatosExtraProyectoDS.ListaDatoExtraProyecto.OrderBy(dato => dato.Orden))
                {
                    Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();
                    foreach (DatoExtraProyectoOpcion filaDatoExtraProyectoOpcion in DatosExtraProyectoDS.ListaDatoExtraProyectoOpcion.Where(dato => dato.DatoExtraID.Equals(fila.DatoExtraID)).OrderBy(dato => dato.Orden))
                    {
                        Guid opcionID = filaDatoExtraProyectoOpcion.OpcionID;
                        string opcion = filaDatoExtraProyectoOpcion.Opcion;

                        listaOpciones.Add(opcionID, opcion);
                    }

                    Guid opcionSeleccionada = Guid.Empty;
                    foreach (DatoExtraProyectoOpcionIdentidad filaDatoExtraOpcionIdentidad in dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad)
                    {
                        if (filaDatoExtraOpcionIdentidad.ProyectoID == fila.ProyectoID && filaDatoExtraOpcionIdentidad.DatoExtraID == fila.DatoExtraID)
                        {
                            opcionSeleccionada = filaDatoExtraOpcionIdentidad.OpcionID;
                        }
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, "es");
                    campoExtra.FieldName = fila.DatoExtraID.ToString();
                    campoExtra.Required = fila.Obligatorio;
                    campoExtra.Options = listaOpciones;
                    campoExtra.FieldValue = opcionSeleccionada.ToString();

                    CamposExtra.Add(campoExtra);
                }
            }

            if (DatosExtraProyectoDS.ListaDatoExtraEcosistemaVirtuoso.Count > 0 || DatosExtraProyectoDS.ListaDatoExtraProyectoVirtuoso.Count > 0)
            {
                foreach (DatoExtraEcosistemaVirtuoso fila in DatosExtraProyectoDS.ListaDatoExtraEcosistemaVirtuoso.OrderBy(dato => dato.Orden))
                {
                    string opcionSeleccionada = "";
                    AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil datoExtraEcosistemaVirtuosoPerfil = dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.FirstOrDefault(datoExtra => datoExtra.DatoExtraID.Equals(fila.DatoExtraID));
                    if (datoExtraEcosistemaVirtuosoPerfil != null)
                    {
                        opcionSeleccionada = datoExtraEcosistemaVirtuosoPerfil.Opcion;
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, "es");
                    campoExtra.FieldName = fila.InputID;
                    campoExtra.Required = fila.Obligatorio;
                    if (fila.InputID.Equals("ddlPais"))
                    {
                        Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();
                        foreach (Es.Riam.Gnoss.AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
                        {

                            listaOpciones.Add(filaPais.PaisID, filaPais.Nombre);
                        }
                        campoExtra.Options = listaOpciones;
                    }
                    else if (!string.IsNullOrEmpty(fila.QueryVirtuoso))
                    {
                        campoExtra.DependencyFields = fila.InputsSuperiores;
                        campoExtra.AutoCompleted = true;
                    }
                    campoExtra.FieldValue = opcionSeleccionada;

                    CamposExtra.Add(campoExtra);
                }

                foreach (DatoExtraProyectoVirtuoso fila in DatosExtraProyectoDS.ListaDatoExtraProyectoVirtuoso.OrderBy(dato => dato.Orden))
                {
                    string opcionSeleccionada = "";
                    AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil datoExtraEcosistemaVirtuosoPerfil = dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.FirstOrDefault(datoExtra => datoExtra.DatoExtraID.Equals(fila.DatoExtraID));
                    if (datoExtraEcosistemaVirtuosoPerfil != null)
                    {
                        opcionSeleccionada = datoExtraEcosistemaVirtuosoPerfil.Opcion;
                    }
                    else
                    {
                        AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad datoExtraIdentidad = dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad.FirstOrDefault(datoExtra => datoExtra.DatoExtraID.Equals(fila.DatoExtraID));
                        if (datoExtraIdentidad != null)
                        {
                            opcionSeleccionada = datoExtraIdentidad.Opcion;
                        }
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, "es");
                    campoExtra.FieldName = fila.InputID;
                    campoExtra.Required = fila.Obligatorio;
                    if (!string.IsNullOrEmpty(fila.QueryVirtuoso))
                    {
                        campoExtra.DependencyFields = fila.InputsSuperiores;
                        campoExtra.AutoCompleted = true;
                    }
                    campoExtra.FieldValue = opcionSeleccionada;

                    CamposExtra.Add(campoExtra);
                }
            }

            paginaModel.ProfilePersonal.AdditionalFields = CamposExtra;
            paginaModel.ProfilePersonal.AdditionalClauses = ClausulasRegistroOpcionales;
        }

        private void CargarCurriculum()
        {
            paginaModel.Curriculum = new EditProfileViewModel.QuickCurriculum();

            Guid curriculumID = Guid.Empty;

            if (CVUnicoPorPerfil)
            {
                if (IdentidadActual.PerfilUsuario.FilaPerfil.CurriculumID.HasValue)
                {
                    curriculumID = IdentidadActual.PerfilUsuario.FilaPerfil.CurriculumID.Value;
                }
            }
            else
            {
                if (IdentidadActual.FilaIdentidad.CurriculumID.HasValue)
                {
                    curriculumID = IdentidadActual.FilaIdentidad.CurriculumID.Value;
                }
            }

            Curriculum curriculum = mEntityContext.Curriculum.FirstOrDefault(cv => cv.CurriculumID.Equals(curriculumID));
            if (curriculum != null) 
            {
                paginaModel.Curriculum.Description = curriculum.Description;
                paginaModel.Curriculum.Tags = curriculum.Tags;
            }
        }

        private void CargarRedesSociales()
        {
            paginaModel.SocialNetworks = new List<EditProfileViewModel.SocialNetwork>();

            Dictionary<string, AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales> ListaRedesSociales = IdentidadActual.ListaRedesSociales;

            if (RequestParams("organizacion") != null && RequestParams("organizacion") == "true")
            {
                ListaRedesSociales = IdentidadActual.IdentidadOrganizacion.ListaRedesSocialesOrganizacion;
            }

            foreach (AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales filaPerfilRedesSoc in ListaRedesSociales.Values)
            {
                string dominio = filaPerfilRedesSoc.urlUsuario.ToLower();

                if (dominio.IndexOf("http://") == 0)
                {
                    dominio = dominio.Substring(7);
                }

                if (dominio.IndexOf("https://") == 0)
                {
                    dominio = dominio.Substring(8);
                }

                if (dominio.IndexOf("www.") == 0)
                {
                    dominio = dominio.Substring(4);
                }

                if (dominio.IndexOf('/') >= 0)
                {
                    dominio = dominio.Substring(0, dominio.IndexOf('/'));
                }

                EditProfileViewModel.SocialNetwork redSocial = new EditProfileViewModel.SocialNetwork();
                redSocial.Domain = dominio;
                redSocial.Name = filaPerfilRedesSoc.NombreRedSocial;
                redSocial.Url = filaPerfilRedesSoc.urlUsuario;

                paginaModel.SocialNetworks.Add(redSocial);
            }
        }

        /// <summary>
        /// Obtiene el RDF del documento de la BD, y devuele las entidades pricipales del mismo.
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <returns>Lista con las entidades pricipales leidas del archivo RDF leido de la BD y generado</returns>
        private List<ElementoOntologia> ObtenerRdfDeDocumento(GestorDocumental pGestorDoc, Guid pDocumentoID)
        {
            GestionOWL gestorOWL = new GestionOWL();
            gestorOWL.UrlOntologia = UrlOntologiaCurriculum;
            gestorOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_HR_XML;

            pGestorDoc.RdfDS = ControladorDocumentacion.ObtenerRDFDeBDRDF(pDocumentoID, ProyectoAD.MetaProyecto);

            string rdfTexto = "";


            if (pGestorDoc.RdfDS.RdfDocumento.Count > 0)
            {
                rdfTexto = pGestorDoc.ListaDocumentos[pDocumentoID].RdfSemantico;
            }
            else
            {
                pGestorDoc.RdfDS = null;

                byte[] buffer = ControladorDocumentacion.ObtenerRDFDeVirtuoso(pDocumentoID, "Curriculum.owl", UrlIntragnoss, UrlOntologiaCurriculum, GestionOWL.NAMESPACE_ONTO_HR_XML, Ontologia);

                if (buffer != null)
                {
                    MemoryStream ms = new MemoryStream();
                    StreamReader reader = new StreamReader(ms);
                    rdfTexto = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }

            List<ElementoOntologia> instanciasPrincipales = null;

            if (!string.IsNullOrEmpty(rdfTexto))
            {
                try
                {
                    instanciasPrincipales = gestorOWL.LeerFicheroRDF(Ontologia, rdfTexto, true);
                }
                catch { }
            }

            return instanciasPrincipales;
        }

        /// <summary>
        /// Agrega la entidad al txt de valores rdf.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pDominioFuncional">Indica si el dominio de la propiedad superior es funcional (o cardinalidad 1)
        /// o no hay dominio</param>
        private string AgregarValoresRDF(ElementoOntologia pEntidad, bool pDominioFuncional)
        {
            string texto = "";

            texto += "<" + pEntidad.TipoEntidad + ">";

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.ElementoOntologia == null)
                {
                    propiedad.ElementoOntologia = pEntidad;
                }

                if (propiedad.Tipo == TipoPropiedad.DatatypeProperty || propiedad.EspecifPropiedad.SelectorEntidad != null)
                {
                    if (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno || propiedad.EspecifPropiedad.ValoresSepComas)
                    {
                        string valorProp = "";

                        if (propiedad.UnicoValor.Key != null)
                        {
                            valorProp = propiedad.UnicoValor.Key;
                        }
                        else if (propiedad.ListaValores != null)
                        {
                            string coma = "";
                            foreach (string valor in propiedad.ListaValores.Keys)
                            {
                                valorProp += coma + valor;
                                coma = ",";
                            }
                        }

                        texto += "<" + propiedad.Nombre + ">" + valorProp.Replace("<", "[--C]").Replace(">", "[C--]") + "</" + propiedad.Nombre + ">";
                    }
                    else if (propiedad.ListaValores != null)
                    {
                        foreach (string valor in propiedad.ListaValores.Keys)
                        {
                            texto += "<" + propiedad.Nombre + ">" + valor.Replace("<", "[--C]").Replace(">", "[C--]") + "</" + propiedad.Nombre + ">";
                        }
                    }
                }
                else
                {//Es ObjectPropert

                    if (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno)
                    {//Solo puede tomar un valor.
                        ElementoOntologia entidad = null;

                        if (propiedad.UnicoValor.Key != null)
                        {
                            entidad = propiedad.UnicoValor.Value;
                        }
                        else if (propiedad.ListaValores != null && propiedad.ListaValores.Any())
                        {
                            entidad = propiedad.ListaValores.First().Value;
                        }

                        if (entidad == null)//Creamos una nueva:
                        {
                            entidad = Ontologia.GetEntidadTipo(propiedad.Rango, false);
                        }

                        texto += "<" + propiedad.Nombre + ">";
                        texto += AgregarValoresRDF(entidad, pDominioFuncional);
                        texto += "</" + propiedad.Nombre + ">";
                    }
                    else
                    {//Puede tomar varios valores.
                        if (propiedad.ListaValores.Count > 0)
                        {
                            foreach (ElementoOntologia entidadProp in propiedad.ListaValores.Values)
                            {
                                texto += "<" + propiedad.Nombre + ">";
                                texto += AgregarValoresRDF(entidadProp, false);
                                texto += "</" + propiedad.Nombre + ">";
                            }
                        }
                    }
                }
            }

            texto += "</" + pEntidad.TipoEntidad + ">";

            return texto;
        }

        /// <summary>
        /// Verdad si existe el email
        /// </summary>
        /// <param name="pEmail">Email a validar</param>
        /// <returns></returns>
        private bool ExisteEmail(string pEmail)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool existe = personaCN.ExisteEmail(pEmail);
            personaCN.Dispose();

            return existe;
        }

        private string IntentoObtenerElPais(string pValor)
        {
            PaisCL paisCL = new PaisCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperPais paisDS = paisCL.ObtenerPaisesProvincias();

            foreach (AD.EntityModel.Models.Pais.Pais fila in paisDS.ListaPais)
            {
                if (fila.PaisID.ToString() == pValor)
                {
                    pValor = fila.Nombre;
                }
            }

            paisCL.Dispose();

            return pValor;
        }

        private void CargarEdadMinimaRegistro()
        {
            ViewBag.EdadMinimaRegistro = 15;
            List<ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("EdadLimiteRegistroEcosistema")).ToList();
            if (busqueda.Count > 0)
            {
                ViewBag.EdadMinimaRegistro = int.Parse(busqueda.First().Valor);
            }
        }


        private void CargarClausulasRegistrosOpcionales()
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            DataWrapperUsuario usuClauProyDS = proyCL.ObtenerClausulasRegitroProyecto(ProyectoSeleccionado.Clave);
            proyCL.Dispose();

            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            usuClauProyDS.Merge(usuCN.ObtenerProyClausulasUsuPorUsuarioID(mControladorBase.UsuarioActual.UsuarioID));
            usuCN.Dispose();

            //cargar el gestor de usuario con las cláusulas del registro
            IdentidadActual.GestorIdentidades.GestorUsuarios = new GestionUsuarios(usuClauProyDS, mLoggingService, mEntityContext, mConfigService);
        }

        private List<EditProfileViewModel.AdditionalClause> ClausulasRegistroOpcionales
        {
            get
            {
                if (mClausulasRegistro == null)
                {
                    mClausulasRegistro = new List<EditProfileViewModel.AdditionalClause>();
                    try
                    {
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        DataWrapperUsuario usuClauProyDS = proyCL.ObtenerClausulasRegitroProyecto(ProyectoSeleccionado.Clave);
                        proyCL.Dispose();

                        UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        usuClauProyDS.Merge(usuCN.ObtenerProyClausulasUsuPorUsuarioID(mControladorBase.UsuarioActual.UsuarioID)); //Migrar EF  Original
                        usuCN.Dispose();

                        //cargar el gestor de usuario con las cláusulas del registro
                        IdentidadActual.GestorIdentidades.GestorUsuarios = new GestionUsuarios(usuClauProyDS, mLoggingService, mEntityContext, mConfigService);
                        //IdentidadActual.GestorIdentidades.GestorUsuarios.UsuarioDS = usuCN.ObtenerProyClausulasUsuPorUsuarioID(mControladorBase.UsuarioActual.UsuarioID);
                        foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in usuClauProyDS.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.Opcional)))
                        {
                            AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg filasProyRolClau = usuClauProyDS.ListaProyRolUsuClausulaReg.Where(item => item.UsuarioID.Equals(mControladorBase.UsuarioActual.UsuarioID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ClausulaID.Equals(filaClausula.ClausulaID)).FirstOrDefault();

                            EditProfileViewModel.AdditionalClause clausula = new EditProfileViewModel.AdditionalClause();
                            clausula.Id = filaClausula.ClausulaID;
                            clausula.Text = filaClausula.Texto;
                            clausula.Order = filaClausula.Orden;
                            clausula.Checked = false;

                            clausula.Checked = filasProyRolClau.Valor;

                            mClausulasRegistro.Add(clausula);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex);
                    }
                }
                return mClausulasRegistro;
            }
        }

        private DataWrapperProyecto DatosExtraProyectoDS
        {
            get
            {
                if (mDatosExtraProyectoDS == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mDatosExtraProyectoDS = proyectoCN.ObtenerDatosExtraProyectoPorID(ProyectoSeleccionado.Clave);
                    proyectoCN.Dispose();
                }
                return mDatosExtraProyectoDS;
            }
        }

        /// <summary>
        /// Obtiene la ontologia
        /// </summary>
        private Ontologia Ontologia
        {
            get
            {
                if (mOntologia == null)
                {
                    LectorXmlConfig lectorXML = new LectorXmlConfig(Guid.Empty, ProyectoAD.MetaProyecto, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD);

                    Dictionary<string, List<EstiloPlantilla>> listaEstilos = lectorXML.ObtenerConfiguracionXml(Es.Riam.Gnoss.Recursos.Properties.Resources.Curriculum, ProyectoAD.MetaProyecto);

                    string rutaOntologia = Path.Combine($"{BaseURLSinHTTP}/{UtilArchivos.ContentOntologias}/Curriculum.owl");//Server.MapPath(BaseURLSinHTTP + "/" + UtilArchivos.ContentOntologias + "/Curriculum.owl");

                    mOntologia = new Ontologia(rutaOntologia, true);
                    mOntologia.LeerOntologia();
                    mOntologia.EstilosPlantilla = listaEstilos;
                    mOntologia.IdiomaUsuario = IdentidadActual.Persona.FilaPersona.Idioma;
                }
                return mOntologia;
            }
        }
    }
}
