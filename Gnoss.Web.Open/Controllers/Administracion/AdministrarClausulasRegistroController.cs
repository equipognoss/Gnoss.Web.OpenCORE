using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
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
using System.Web;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Microsoft.Extensions.Hosting;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarClausulasRegistroController : ControllerBaseWeb
    {

        public AdministrarClausulasRegistroController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth, IHostApplicationLifetime appLifetime)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth, appLifetime)
        {
        }

        #region Miembros

        private ManageRegisterClausesViewModel mPaginaModel = null;

        private DataWrapperUsuario mUsuarioDR = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionPaginasPermitido" })]
        public ActionResult Index()
        {

            // Añadir clase para el body del Layout
            ViewBag.BodyClassPestanya = "configuracion edicionClausulas edicion no-max-width-container";
            ViewBag.ActiveSection = AdministracionSeccionesDevTools.SeccionesDevTools.Configuracion;
            ViewBag.ActiveSubSection = AdministracionSeccionesDevTools.SubSeccionesDevTools.Configuracion_ClausulasLegales;
            // Establecer el título para el header de DevTools
            ViewBag.HeaderParentTitle = UtilIdiomas.GetText("DEVTOOLS", "CONFIGURACION");
            ViewBag.HeaderTitle = UtilIdiomas.GetText("DEVTOOLS", "CLAUSULASDEREGISTRO");

            // Establecer en el ViewBag el idioma por defecto
            ViewBag.IdiomaPorDefecto = IdiomaPorDefecto;

            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();
            return View(PaginaModel);
        }

        /// <summary>
        /// Nueva Clausula
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionPaginasPermitido" })]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult NuevaClausula(ManageRegisterClausesViewModel.ClauseType TipoClausula)
        {
            GuardarLogAuditoria();
            EliminarPersonalizacionVistas();

            ManageRegisterClausesViewModel.RegisterClauseModel clause = new ManageRegisterClausesViewModel.RegisterClauseModel();
            clause.Key = Guid.NewGuid();
            clause.Title = "";
            clause.Text1 = "";
            clause.Text2 = "";
            clause.Order = 0;
            clause.Type = TipoClausula;
            clause.CookieName = "";

            switch (TipoClausula)
            {
                case ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina:
                    #region carga a partir de las cláusulas del ecosistema
                    ////cargar la cláusula de política de cookies con los textos del ecositema
                    //ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD);
                    //UsuarioDS usuarioDS = proyCL.ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto);
                    //proyCL.Dispose();
                    //List<UsuarioDS.ClausulaRegistroRow> filasCabecera = usuarioDS.ClausulaRegistro.Where(c => c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesCabecera)).ToList();
                    //List<UsuarioDS.ClausulaRegistroRow> filasTexto = usuarioDS.ClausulaRegistro.Where(c => c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina)).ToList();

                    //if (filasCabecera.Count > 0)
                    //{
                    //    clause.Text1 = filasCabecera[0].Texto;
                    //}
                    //if (filasTexto.Count > 0)
                    //{
                    //    clause.Text2 = filasTexto[0].Texto;
                    //}
                    #endregion

                    string enlacePoliticaCookies = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "POLITICACOOKIES");
                    string textoCabecera = UtilIdiomas.GetText("COMADMINCLAUSULASREGISTRO", "TEXTODEFECTOCABECERAPOLCOOKIES", enlacePoliticaCookies);
                    string textoPagina = UtilIdiomas.GetText("COMADMINCLAUSULASREGISTRO", "TEXTODEFECTOPAGINAPOLCOOKIES");
                    clause.Text1 = textoCabecera;
                    clause.Text2 = textoPagina;
                    //clause.CookieName = UtilIdiomas.GetText("COMADMINCLAUSULASREGISTRO", "TEXTODEFECTONOMBRECOOKIEPOLITICACOOKIES");
                    clause.Title = UtilIdiomas.GetText("POLITICACOOKIES", "TITULO");
                    break;

                case ManageRegisterClausesViewModel.ClauseType.CondicionesUso:
                    clause.Title = UtilIdiomas.GetText("CONDICIONESUSO", "TITULO");
                    break;

                case ManageRegisterClausesViewModel.ClauseType.ClausulasTexo:
                    clause.Title = UtilIdiomas.GetText("POLITICAPRIVACIDAD", "TITULOPAGINA");
                    break;
            }

            return PartialView("_EdicionClausulaRegistro", clause);
        }

        /// <summary>
        /// Guardar
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Diseño, "AdministracionPaginasPermitido" })]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(List<ManageRegisterClausesViewModel.RegisterClauseModel> ListaClausulas)
        {
            GuardarLogAuditoria();
            GuardarClausulas(ListaClausulas);

            InvalidarCaches();

            return GnossResultOK();


        }

        #region Métodos Privados

        private void GuardarClausulas(List<ManageRegisterClausesViewModel.RegisterClauseModel> ListaClausulas)
        {
            List<Guid> listaClausulasNuevas = new List<Guid>();
            //bool tieneClausulaObligatoria = false;

            //Añadir los nuevos
            foreach (ManageRegisterClausesViewModel.RegisterClauseModel clausula in ListaClausulas)
            {
                if (!clausula.Deleted)
                {
                    List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasClausulas = UsuarioDW.ListaClausulaRegistro.Where(item => item.ClausulaID.Equals(clausula.Key)).ToList();

                    //cuando se ponga la unicidad en el desplegable de tipos para la Obligatoria no debería ser necesaria la comprobación de que sólo ha añadido una cláusula de este tipo -> ahora mismo no son editables
                    if (filasClausulas.Count == 0 /*&& (!clausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.Obligatoria) || !tieneClausulaObligatoria)*/)
                    {
                        listaClausulasNuevas.Add(clausula.Key);
                        AgregarClausulaNueva(clausula);
                        //if (clausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.Obligatoria))
                        //{
                        //    tieneClausulaObligatoria = true;
                        //}
                    }
                }
            }

            //Modificar los que tienen cambios
            foreach (ManageRegisterClausesViewModel.RegisterClauseModel clausula in ListaClausulas)
            {
                if (!clausula.Deleted && !listaClausulasNuevas.Contains(clausula.Key))
                {
                    List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasClausulas = UsuarioDW.ListaClausulaRegistro.Where(item => item.ClausulaID.Equals(clausula.Key)).ToList();

                    if (filasClausulas.Count > 0)
                    {
                        EditarClausula(filasClausulas.FirstOrDefault(), clausula);
                    }
                }
            }

            //Eliminar los eliminados
            foreach (ManageRegisterClausesViewModel.RegisterClauseModel clausula in ListaClausulas)
            {
                if (clausula.Deleted && !listaClausulasNuevas.Contains(clausula.Key))
                {
                    AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausulas = UsuarioDW.ListaClausulaRegistro.FirstOrDefault(item => item.ClausulaID.Equals(clausula.Key));

                    if (filaClausulas != null)
                    {
                        EliminarClausula(filaClausulas);

                        if (clausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina))
                        {
                            ParametroProyecto.Remove(ParametroAD.NombrePoliticaCookies);
                        }
                    }
                }
            }

            bool tieneClausulas = ListaClausulas.Find(clausula => !clausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesCabecera) && !clausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina)) != null;
            bool actualizarParamGral = false;

            if (tieneClausulas && !ParametrosGeneralesRow.ClausulasRegistro)
            {
                ParametrosGeneralesRow.ClausulasRegistro = true;
                actualizarParamGral = true;
            }
            else if (ParametrosGeneralesRow.ClausulasRegistro && !tieneClausulas)
            {
                ParametrosGeneralesRow.ClausulasRegistro = false;
                actualizarParamGral = true;
            }

            ParametroAD parametroAD = new ParametroAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
            {
                parametroAD.ActualizarParametroEnProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ParametroAD.NombrePoliticaCookies, ParametroProyecto[ParametroAD.NombrePoliticaCookies]);
            }
            else
            {
                parametroAD.BorrarParametroDeProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ParametroAD.NombrePoliticaCookies);
            }

            if (actualizarParamGral)
            {
                ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.ActualizarParametrosGenerales();
                paramCN.Dispose();
            }

            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            usuCN.ActualizarUsuario(false);
            usuCN.Dispose();
        }

        private void EliminarEntrada(object pEntrada)
        {
            mEntityContext.EliminarElemento(pEntrada);
        }

        private void AgregarClausulaNueva(ManageRegisterClausesViewModel.RegisterClauseModel pClausula)
        {
            AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaNuevaClausula = new AD.EntityModel.Models.UsuarioDS.ClausulaRegistro();
            filaNuevaClausula.ClausulaID = pClausula.Key;
            filaNuevaClausula.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaNuevaClausula.ProyectoID = ProyectoSeleccionado.Clave;
            filaNuevaClausula.Orden = pClausula.Order;
            filaNuevaClausula.Tipo = (short)pClausula.Type;
            filaNuevaClausula.Texto = HttpUtility.UrlDecode(pClausula.Text1);

            if (pClausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina))
            {
                AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaNuevaClausulaCabecera = new AD.EntityModel.Models.UsuarioDS.ClausulaRegistro();
                filaNuevaClausulaCabecera.ClausulaID = Guid.NewGuid();
                filaNuevaClausulaCabecera.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaNuevaClausulaCabecera.ProyectoID = ProyectoSeleccionado.Clave;
                filaNuevaClausulaCabecera.Orden = pClausula.Order;
                filaNuevaClausulaCabecera.Tipo = (short)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesCabecera;
                filaNuevaClausulaCabecera.Texto = HttpUtility.UrlDecode(pClausula.Text1);
                //+1 al orden de la cláusula PoliticaCookiesUrlPagina
                filaNuevaClausula.Orden = pClausula.Order + 1;
                filaNuevaClausula.Texto = HttpUtility.UrlDecode(pClausula.Text2);

                UsuarioDW.ListaClausulaRegistro.Add(filaNuevaClausulaCabecera);
                mEntityContext.ClausulaRegistro.Add(filaNuevaClausulaCabecera);

                if (!string.IsNullOrEmpty(pClausula.CookieName))
                {
                    if (!ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
                    {
                        ParametroProyecto.Add(ParametroAD.NombrePoliticaCookies, "");
                    }
                    ParametroProyecto[ParametroAD.NombrePoliticaCookies] = pClausula.CookieName;
                }
            }
            else if (pClausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.CondicionesUso))
            {
                AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaNuevaClausulaTituloCond = new AD.EntityModel.Models.UsuarioDS.ClausulaRegistro();
                filaNuevaClausulaTituloCond.ClausulaID = Guid.NewGuid();
                filaNuevaClausulaTituloCond.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaNuevaClausulaTituloCond.ProyectoID = ProyectoSeleccionado.Clave;
                filaNuevaClausulaTituloCond.Orden = pClausula.Order;
                filaNuevaClausulaTituloCond.Tipo = (short)ManageRegisterClausesViewModel.ClauseType.TituloCondicionesUso;
                filaNuevaClausulaTituloCond.Texto = pClausula.Title;
                //+1 al orden de la cláusula PoliticaCookiesUrlPagina
                filaNuevaClausula.Orden = pClausula.Order + 1;

                UsuarioDW.ListaClausulaRegistro.Add(filaNuevaClausulaTituloCond);
                mEntityContext.ClausulaRegistro.Add(filaNuevaClausulaTituloCond);

                List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasClausulaObligatoria = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.Obligatoria)).ToList();

                if (filasClausulaObligatoria.Count > 0)
                {
                    if (string.IsNullOrEmpty(filasClausulaObligatoria[0].Texto) || filasClausulaObligatoria[0].Texto.Contains("##POLITICA_PRIVACIDAD_COM##"))
                    {
                        filasClausulaObligatoria[0].Texto += "##CONDICIONES_USO_COM##";
                    }
                }
                else
                {
                    AñadirFilaClausulaObligatoria(pClausula, "##CONDICIONES_USO_COM##");
                }
            }
            else if (pClausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.ClausulasTexo))
            {
                AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaNuevaClausulaTituloPrivacidad = new AD.EntityModel.Models.UsuarioDS.ClausulaRegistro();
                filaNuevaClausulaTituloPrivacidad.ClausulaID = Guid.NewGuid();
                filaNuevaClausulaTituloPrivacidad.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaNuevaClausulaTituloPrivacidad.ProyectoID = ProyectoSeleccionado.Clave;
                filaNuevaClausulaTituloPrivacidad.Orden = pClausula.Order;
                filaNuevaClausulaTituloPrivacidad.Tipo = (short)ManageRegisterClausesViewModel.ClauseType.TituloClausulasTexo;
                filaNuevaClausulaTituloPrivacidad.Texto = pClausula.Title;
                //+1 al orden de la cláusula PoliticaCookiesUrlPagina
                filaNuevaClausula.Orden = pClausula.Order + 1;

                UsuarioDW.ListaClausulaRegistro.Add(filaNuevaClausulaTituloPrivacidad);
                mEntityContext.ClausulaRegistro.Add(filaNuevaClausulaTituloPrivacidad);

                List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasClausulaObligatoria = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.Obligatoria)).ToList();

                if (filasClausulaObligatoria.Count > 0)
                {
                    if (string.IsNullOrEmpty(filasClausulaObligatoria[0].Texto) || filasClausulaObligatoria[0].Texto.Contains("##CONDICIONES_USO_COM##"))
                    {
                        filasClausulaObligatoria[0].Texto += "##POLITICA_PRIVACIDAD_COM##";
                    }
                }
                else
                {
                    AñadirFilaClausulaObligatoria(pClausula, "##POLITICA_PRIVACIDAD_COM##");
                }
            }

            UsuarioDW.ListaClausulaRegistro.Add(filaNuevaClausula);
            mEntityContext.ClausulaRegistro.Add(filaNuevaClausula);
        }

        private void AñadirFilaClausulaObligatoria(ManageRegisterClausesViewModel.RegisterClauseModel pClausula, string pMascara)
        {
            AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaNuevaClausulaObligatoria = new AD.EntityModel.Models.UsuarioDS.ClausulaRegistro();
            filaNuevaClausulaObligatoria.ClausulaID = Guid.NewGuid();
            filaNuevaClausulaObligatoria.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaNuevaClausulaObligatoria.ProyectoID = ProyectoSeleccionado.Clave;
            filaNuevaClausulaObligatoria.Orden = pClausula.Order;
            filaNuevaClausulaObligatoria.Tipo = (short)ManageRegisterClausesViewModel.ClauseType.Obligatoria;
            filaNuevaClausulaObligatoria.Texto = pMascara;
            //+2 al orden de la cláusula Obligatoria
            filaNuevaClausulaObligatoria.Orden = pClausula.Order + 2;

            UsuarioDW.ListaClausulaRegistro.Add(filaNuevaClausulaObligatoria);
            mEntityContext.ClausulaRegistro.Add(filaNuevaClausulaObligatoria);
        }

        private void EditarClausula(AD.EntityModel.Models.UsuarioDS.ClausulaRegistro pFilaClausula, ManageRegisterClausesViewModel.RegisterClauseModel pClausula)
        {
            pFilaClausula.Orden = pClausula.Order;
            pFilaClausula.Texto = HttpUtility.UrlDecode(pClausula.Text1);

            if (pClausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina))
            {
                pFilaClausula.Texto = HttpUtility.UrlDecode(pClausula.Text2);
                List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasCabecera = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesCabecera)).ToList();

                if (filasCabecera.Count > 0)
                {
                    filasCabecera[0].Texto = HttpUtility.UrlDecode(pClausula.Text1);
                    filasCabecera[0].Orden = pClausula.Order;
                }
                //+1 al orden de la cláusula PoliticaCookiesUrlPagina respecto de PoliticaCookiesCabecera
                pFilaClausula.Orden = pClausula.Order + 1;

                //si no existe se añade, si existe y no es vacía se edita y si existe y es vacía se elimina
                if (!string.IsNullOrEmpty(pClausula.CookieName))
                {
                    if (!ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
                    {
                        ParametroProyecto.Add(ParametroAD.NombrePoliticaCookies, "");
                    }
                    ParametroProyecto[ParametroAD.NombrePoliticaCookies] = pClausula.CookieName;
                }
                else
                {
                    if (ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
                    {
                        ParametroProyecto.Remove(ParametroAD.NombrePoliticaCookies);
                    }
                }
            }
            else if (pClausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.CondicionesUso))
            {
                List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasTituloCondUso = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.TituloCondicionesUso)).ToList();

                if (filasTituloCondUso.Count > 0)
                {
                    filasTituloCondUso[0].Texto = HttpUtility.UrlDecode(pClausula.Title);
                    filasTituloCondUso[0].Orden = pClausula.Order;
                }
                //+1 al orden de la cláusula CondicionesUso respecto de TituloCondicionesUso
                pFilaClausula.Orden = pClausula.Order + 1;
            }
            else if (pClausula.Type.Equals(ManageRegisterClausesViewModel.ClauseType.ClausulasTexo))
            {
                List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasTituloPolPriv = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.TituloClausulasTexo)).ToList();

                if (filasTituloPolPriv.Count > 0)
                {
                    filasTituloPolPriv[0].Texto = HttpUtility.UrlDecode(pClausula.Title);
                }
                //+1 al orden de la cláusula ClausulasTexo respecto de TituloClausulasTexo
                pFilaClausula.Orden = pClausula.Order + 1;
            }
        }

        private void EliminarClausula(AD.EntityModel.Models.UsuarioDS.ClausulaRegistro pFilaClausula)
        {
            string textoClausula = string.Empty;

            if (pFilaClausula.Tipo.Equals((int)ManageRegisterClausesViewModel.ClauseType.CondicionesUso))
            {
                AD.EntityModel.Models.UsuarioDS.ClausulaRegistro tituloCondUso = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.TituloCondicionesUso)).FirstOrDefault();

                EliminarEntrada(tituloCondUso);

                AD.EntityModel.Models.UsuarioDS.ClausulaRegistro clausulaObligatoria = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.Obligatoria)).FirstOrDefault();

                textoClausula = clausulaObligatoria.Texto;

                if (textoClausula.Contains("##CONDICIONES_USO_COM##"))
                {
                    textoClausula = textoClausula.Replace("##CONDICIONES_USO_COM##", string.Empty);
                }

                if (string.IsNullOrEmpty(textoClausula))
                {
                    EliminarEntrada(clausulaObligatoria);
                }
                else
                {
                    clausulaObligatoria.Texto = textoClausula;
                }
            }

            if (pFilaClausula.Tipo.Equals((int)ManageRegisterClausesViewModel.ClauseType.ClausulasTexo))
            {
                AD.EntityModel.Models.UsuarioDS.ClausulaRegistro tituloPolPriv = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.TituloClausulasTexo)).FirstOrDefault();

                EliminarEntrada(tituloPolPriv);

                AD.EntityModel.Models.UsuarioDS.ClausulaRegistro clausulaObligatoria = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.Obligatoria)).FirstOrDefault();

                textoClausula = clausulaObligatoria.Texto;

                if (textoClausula.Contains("##POLITICA_PRIVACIDAD_COM##"))
                {
                    textoClausula = textoClausula.Replace("##POLITICA_PRIVACIDAD_COM##", "");
                }

                if (string.IsNullOrEmpty(textoClausula))
                {
                    EliminarEntrada(clausulaObligatoria);
                }
                else
                {
                    clausulaObligatoria.Texto = textoClausula;
                }
            }

            if (pFilaClausula.Tipo.Equals((int)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina))
            {
                AD.EntityModel.Models.UsuarioDS.ClausulaRegistro cabecera = UsuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesCabecera)).FirstOrDefault();

                EliminarEntrada(cabecera);

                if (ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
                {
                    ParametroProyecto.Remove(ParametroAD.NombrePoliticaCookies);
                }
            }
            mEntityContext.EliminarElemento(pFilaClausula);
            UsuarioDW.ListaClausulaRegistro.Remove(pFilaClausula);
        }

        private void InvalidarCaches()
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarCachePoliticaCookiesProyecto(ProyectoSeleccionado.Clave);
            proyCL.InvalidarCacheClausulasRegitroProyecto(ProyectoSeleccionado.Clave);
            proyCL.InvalidarParametrosProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID);
            proyCL.Dispose();
        }

        #endregion

        #region Propiedades

        private ManageRegisterClausesViewModel PaginaModel
        {

            get
            {
                if (mPaginaModel == null)
                {
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
					mPaginaModel = new ManageRegisterClausesViewModel();
                    mPaginaModel.LanguagesList = paramCL.ObtenerListaIdiomasDictionary();
                    mPaginaModel.DefaultLanguage = !(ParametrosGeneralesRow.IdiomaDefecto == null) && mPaginaModel.LanguagesList.ContainsKey(ParametrosGeneralesRow.IdiomaDefecto) ? ParametrosGeneralesRow.IdiomaDefecto : mPaginaModel.LanguagesList.Keys.First();
                    mPaginaModel.ClausesList = new List<ManageRegisterClausesViewModel.RegisterClauseModel>();

                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    DataWrapperUsuario usuarioDW = proyCL.ObtenerClausulasRegitroProyecto(ProyectoSeleccionado.Clave);
                    if (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                    {
                        usuarioDW.Merge(proyCL.ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto));
                    }
                    proyCL.Dispose();

                    //recorremos únicamente las cláusulas del proyecto
                    foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in usuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave)).OrderBy(c => c.Orden))
                    {
                        //ahora mismo las obligatorias no son editables por si mismas -> dependen de CondicionesUso y ClausulasTexo(Pol. Privacidad)
                        if (filaClausula.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.CondicionesUso) || filaClausula.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.ClausulasTexo) || filaClausula.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.Opcional) || filaClausula.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina) /*|| filaClausula.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.Obligatoria)*/)
                        {
                            ManageRegisterClausesViewModel.RegisterClauseModel clause = new ManageRegisterClausesViewModel.RegisterClauseModel();
                            clause.Key = filaClausula.ClausulaID;
                            clause.Order = filaClausula.Orden;
                            clause.Type = (ManageRegisterClausesViewModel.ClauseType)filaClausula.Tipo;
                            clause.Text1 = filaClausula.Texto;
                            clause.Text2 = "";

                            switch (clause.Type)
                            {
                                //case ManageRegisterClausesViewModel.ClauseType.Obligatoria:
                                //    //en esta cláusula el título es irrelevante y no se va a editar
                                //    clause.Title = UtilIdiomas.GetText("COMADMINCLAUSULASREGISTRO", "CLAUSULAOBLIGATORIATITULO");
                                //    break;
                                case ManageRegisterClausesViewModel.ClauseType.Opcional:
                                    //en esta cláusula el título es irrelevante y no se va a editar
                                    clause.Title = UtilIdiomas.GetText("COMADMINCLAUSULASREGISTRO", "CLAUSULAOPCIONALTITULO");
                                    break;
                                case ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesUrlPagina:
                                    //en esta cláusula el título es irrelevante y no se va a editar
                                    //tiene dos textos porque son dos cláusulas en una y ambas son de texto, no una de título como en CondicionesUso o ClausulasTexto(Política de privacidad)
                                    clause.Title = UtilIdiomas.GetText("POLITICACOOKIES", "TITULO");
                                    clause.Text1 = "";
                                    clause.Text2 = filaClausula.Texto;
                                    //buscamos la cláusula de cabecera asociada a la cláusula de PoliticaCookiesUrlPagina
                                    List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasCabecera = usuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesCabecera)).ToList();

                                    string nombreCookie = string.Empty;
                                    if (ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
                                    {
                                        clause.CookieName = ParametroProyecto[ParametroAD.NombrePoliticaCookies];
                                    }

                                    if (filasCabecera.Count > 0)
                                    {
                                        clause.Text1 = filasCabecera[0].Texto;
                                    }
                                    else
                                    {
                                        //si el proyecto no la tiene personalizada, buscamos la cláusula de cabecera asociada a la cláusula de PoliticaCookiesUrlPagina en el ecosistema
                                        List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasCabeceraEcosistema = usuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoAD.MetaProyecto) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.PoliticaCookiesCabecera)).ToList();
                                        if (filasCabeceraEcosistema.Count > 0)
                                        {
                                            clause.Text1 = filasCabeceraEcosistema[0].Texto;
                                        }
                                    }
                                    break;
                                case ManageRegisterClausesViewModel.ClauseType.CondicionesUso:
                                    clause.Title = UtilIdiomas.GetText("CONDICIONESUSO", "TITULO");
                                    clause.Text1 = filaClausula.Texto;
                                    clause.Text2 = "";
                                    //buscamos la cláusula de título asociada a la cláusula de CondicionesUso
                                    List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasTituloCondUso = usuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.TituloCondicionesUso)).ToList();

                                    if (filasTituloCondUso.Count > 0)
                                    {
                                        clause.Title = filasTituloCondUso[0].Texto;
                                    }
                                    break;
                                case ManageRegisterClausesViewModel.ClauseType.ClausulasTexo:
                                    clause.Title = UtilIdiomas.GetText("POLITICAPRIVACIDAD", "TITULOPAGINA");
                                    clause.Text1 = filaClausula.Texto;
                                    clause.Text2 = "";
                                    //buscamos la cláusula de título asociada a la cláusula de ClausulasTexo(Política de privacidad)
                                    List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> filasTituloPolPriv = usuarioDW.ListaClausulaRegistro.Where(c => c.ProyectoID.Equals(ProyectoSeleccionado.Clave) && c.Tipo.Equals((short)ManageRegisterClausesViewModel.ClauseType.TituloClausulasTexo)).ToList();

                                    if (filasTituloPolPriv.Count > 0)
                                    {
                                        clause.Title = filasTituloPolPriv[0].Texto;
                                    }
                                    break;
                            }

                            mPaginaModel.ClausesList.Add(clause);
                        }
                    }
                }

                return mPaginaModel;
            }
        }

        private DataWrapperUsuario UsuarioDW
        {
            get
            {
                if (mUsuarioDR == null)
                {
                    mUsuarioDR = new DataWrapperUsuario();
                    UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mUsuarioDR = usuCN.ObtenerClausulasRegitroProyecto(ProyectoSeleccionado.Clave);
                    usuCN.Dispose();
                }
                return mUsuarioDR;
            }
        }

        #endregion
    }
}
