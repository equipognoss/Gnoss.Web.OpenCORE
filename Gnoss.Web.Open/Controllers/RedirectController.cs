using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class RedirectController : ControllerBaseWeb
    {

        public RedirectController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.DisplayName == "Recurso")
            {
                base.OnActionExecuting(filterContext);
            }
        }

        public ActionResult Home()
        {
            mLoggingService.AgregarEntrada("Entra por RedirectController");
            string sPage = "";

            if (!string.IsNullOrEmpty(UrlPresentacion))
            {
                mLoggingService.AgregarEntrada("Tiene url de Presentacion");

                if (!(UrlPresentacion.StartsWith("http://") || UrlPresentacion.StartsWith("https://")))
                {
                    string parametros = "";
                    if (ProyectoConexionID != null)
                    {
                        parametros = "?proyectoID=" + ProyectoConexionID + "&comunidad=true";
                    }

                    sPage = BaseURL + "/" + UrlPresentacion + parametros;
                }
                else
                {
                    sPage = UrlPresentacion;
                }
            }
            else
            {
                CargarUtilIdiomasDesdeIdiomaNavegador();

                mLoggingService.AgregarEntrada("No tiene url de Presentacion");

                if (ProyectoConexionID.Value.Equals(ProyectoAD.MetaProyecto))
                {
                    mLoggingService.AgregarEntrada("No tiene ProyectoConexionID");
                    if (UtilIdiomas.LanguageCode != "es")
                    {
                        sPage = "/" + UtilIdiomas.LanguageCode;
                    }
                    sPage = sPage + "/" + UtilIdiomas.GetText("URLSEM", "HOME");
                }
                else
                {
                    mLoggingService.AgregarEntrada("Tiene ProyectoConexionID");

                    if (UtilIdiomas.LanguageCode != "es")
                    {
                        sPage = "/" + UtilIdiomas.LanguageCode;
                    }
                    sPage = sPage + "/" + UtilIdiomas.GetText("URLSEM", "COMUNIDAD") + "/" + NombreCortoProyectoConexion;
                }
            }

            mLoggingService.AgregarEntrada("Url de redirección permanente" + sPage);

            return RedirectPermanent(sPage);
        }

        private void CargarUtilIdiomasDesdeIdiomaNavegador()
        {
            Dictionary<string, string> listaIdiomas = mConfigService.ObtenerListaIdiomasDictionary();

            string idioma = UtilIdiomas.LanguageCode;

            if (!listaIdiomas.ContainsKey(idioma))
            {
                idioma = listaIdiomas.First().Key;
            }

            if (!EsBot && !Request.Headers.ContainsKey("Referer") && Request.Headers.ContainsKey("accept-language"))
            {
                var languages = Request.Headers["accept-language"].ToString().Split(',')
                    .Select(System.Net.Http.Headers.StringWithQualityHeaderValue.Parse)
                    .OrderByDescending(s => s.Quality.GetValueOrDefault(1));

                foreach (var userLanguaje in languages)
                {
                    string idiomaNavegador = userLanguaje.Value.Split('-')[0];
                    if (listaIdiomas.ContainsKey(idiomaNavegador))
                    {
                        idioma = idiomaNavegador;
                        break;
                    }
                }
            }

            if (UtilIdiomas.LanguageCode != idioma)
            {
                UtilIdiomas = new UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService);
            }
        }

        public ActionResult Recurso()
        {
            Guid recursoID = new Guid(RequestParams("RecursoID"));

            bool redirect301 = !string.IsNullOrEmpty(RequestParams("redirect301"));

            //TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService);

            //lnkInicio.Attributes["href"] = BaseURLIdioma + UrlPerfil + UtilIdiomas.GetText("URLSEM", "HOME");


            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Guid> listaRecursos = new List<Guid>();
            listaRecursos.Add(recursoID);
            GestorDocumental gestionDocumentacion = new GestorDocumental(documentacionCN.ObtenerDocumentosPorID(listaRecursos, true), mLoggingService, mEntityContext);
            gestionDocumentacion.CargarDocumentos(false);
            if (gestionDocumentacion.DataWrapperDocumentacion.ListaDocumento.Count > 0)
            {
                DocumentoWeb documentoWeb = new DocumentoWeb(gestionDocumentacion.DataWrapperDocumentacion.ListaDocumento.FirstOrDefault(), gestionDocumentacion, mLoggingService);
                //mTitulo = documentoWeb.Titulo;


                bool permitirProyPrivados = true;//(Request.Query["redireccionarDocumento"] == "false");

                #region Pre-cargas

                List<Guid> listaProyectosDocumento = new List<Guid>();
                if (gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Count > 0)
                {
                    foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProy in gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosProyecto)
                    {
                        listaProyectosDocumento.Add(filaBRProy.ProyectoID);
                    }
                }

                if (!listaProyectosDocumento.Contains(documentoWeb.ProyectoID))
                {
                    listaProyectosDocumento.Add(documentoWeb.ProyectoID);
                }

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperProyecto proyDataWrapperProyecto = proyCN.ObtenerProyectosPorIDsCargaLigera(listaProyectosDocumento);
                proyCN.Dispose();

                GestionProyecto gestorProyectos = new GestionProyecto(proyDataWrapperProyecto, mLoggingService, mEntityContext);

                #endregion

                //1º Miramos el sitio donde se publicó el recurso:
                #region 1º comprobacion
                if (documentoWeb.ProyectoID == ProyectoAD.MetaProyecto)
                {
                    bool hayQueLlevarAMyGnoss = false;
                    if (gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count > 0)
                    {
                        AD.EntityModel.Models.PersonaDS.Persona filaPersona = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerPersonaPorIdentidadCargaLigera(documentoWeb.CreadorID);

                        if (filaPersona.PersonaID == mControladorBase.UsuarioActual.PersonaID /*&& IdentidadActual.Tipo.Equals(TiposIdentidad.Personal)*/)
                        {
                            hayQueLlevarAMyGnoss = true;
                        }
                        else
                        {
                            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            gestionDocumentacion.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroUsuario(filaPersona.UsuarioID.Value), mLoggingService, mEntityContext);
                            tesauroCN.Dispose();

                            Guid clavePublica = (gestionDocumentacion.GestorTesauro.TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroPublicoID.Value;
                            bool documentoEnCatPublica = false;
                            foreach (CategoriaTesauro categoria in documentoWeb.Categorias.Values)
                            {
                                if (categoria.PadreNivelRaiz.Clave == clavePublica)
                                {
                                    documentoEnCatPublica = true;
                                    break;
                                }
                            }

                            if (documentoEnCatPublica)
                            {
                                Guid identidadLink = Guid.Empty;
                                if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto)
                                {
                                    identidadLink = mControladorBase.UsuarioActual.IdentidadID;
                                }
                                else
                                {
                                    if (ParticipaUsuarioActualEnProyecto(ProyectoAD.MetaProyecto))
                                    {
                                        identidadLink = IdentidadUsuarioActualEnProyecto(ProyectoAD.MetaProyecto);
                                    }
                                }
                                if (identidadLink != Guid.Empty)
                                {
                                    //Response.Redirect("perfilRecursosCompartidosFicha.aspx?ID=" + identidadLink + "&docID=" + documentoWeb.Clave.ToString() + "&identidad=" + documentoWeb.CreadorID);
                                    if (!mControladorBase.UsuarioActual.EsUsuarioInvitado)
                                    {
                                        hayQueLlevarAMyGnoss = true;
                                    }
                                }
                            }
                        }
                    }

                    if (gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count > 0)
                    {
                        AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR = gestionDocumentacion.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Find(doc => doc.IdentidadPublicacionID.Equals(documentoWeb.CreadorID) && doc.TipoPublicacion.Equals((short)TipoPublicacion.Publicado));

                        AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion filaBROrg = gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Where(item => item.BaseRecursosID.Equals(filaDocVinBR.BaseRecursosID)).FirstOrDefault();

                        TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        gestionDocumentacion.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroOrganizacion(filaBROrg.OrganizacionID), mLoggingService, mEntityContext);
                        tesauroCN.Dispose();

                        Guid clavePublica = gestionDocumentacion.GestorTesauro.TesauroDW.ListaTesauroOrganizacion.FirstOrDefault().CategoriaTesauroPublicoID.Value;
                        bool documentoEnCatPublica = false;
                        documentoWeb.RecargarCategoriasTesauro();
                        foreach (CategoriaTesauro categoria in documentoWeb.Categorias.Values)
                        {
                            if (categoria.PadreNivelRaiz.Clave == clavePublica)
                            {
                                documentoEnCatPublica = true;
                                break;
                            }
                        }

                        if (documentoEnCatPublica)
                        {
                            Guid identidadLink = Guid.Empty;
                            if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto)
                            {
                                identidadLink = mControladorBase.UsuarioActual.IdentidadID;
                            }
                            else
                            {
                                if (ParticipaUsuarioActualEnProyecto(ProyectoAD.MetaProyecto))
                                {
                                    identidadLink = IdentidadUsuarioActualEnProyecto(ProyectoAD.MetaProyecto);
                                }
                            }
                            if (identidadLink != Guid.Empty)
                            {
                                //Response.Redirect("perfilRecursosCompartidosFicha.aspx?ID=" + identidadLink + "&docID=" + documentoWeb.Clave.ToString() + "&identidad=" + documentoWeb.CreadorID);
                                hayQueLlevarAMyGnoss = true;
                            }
                        }
                    }

                    //Si hay identidad habrá que llevar con esa:
                    if (hayQueLlevarAMyGnoss)
                    {
                        GestionIdentidades gestorIdentAux = new GestionIdentidades(new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerIdentidadPorID(documentoWeb.CreadorID, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                        string urlRedirect = "";

                        if (IdentidadActual.GestorIdentidades.ListaIdentidades.ContainsKey(documentoWeb.CreadorID))
                        {
                            //Si la identidad a la que llevamos es una identidad mía:
                            urlRedirect = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, "", UrlPerfil, documentoWeb, false);
                        }
                        else
                        {
                            urlRedirect = GnossUrlsSemanticas.GetURLBaseRecursosFichaMyGnoss(BaseURLIdioma, UtilIdiomas, UrlPerfil, gestorIdentAux.ListaIdentidades[documentoWeb.CreadorID], documentoWeb);
                        }
                        gestorIdentAux.Dispose();
                        gestorProyectos.Dispose();

                        return Redirect(urlRedirect);
                    }
                }
                else
                {
                    if (permitirProyPrivados || (gestorProyectos.ListaProyectos[documentoWeb.ProyectoID].FilaProyecto.TipoAcceso != (short)TipoAcceso.Privado && gestorProyectos.ListaProyectos[documentoWeb.ProyectoID].FilaProyecto.TipoAcceso != (short)TipoAcceso.Reservado))
                    {
                        Guid identidadLink = Guid.Empty;
                        if (ParticipaUsuarioActualEnProyecto(documentoWeb.ProyectoID))
                        {
                            identidadLink = IdentidadUsuarioActualEnProyecto(documentoWeb.ProyectoID);
                        }

                        if (!identidadLink.Equals(Guid.Empty))
                        {
                            //Response.Redirect("perfilBaseRecursosFicha.aspx?ID=" + identidadLink + "&docID=" + documentoWeb.Clave.ToString() + "&comunidad=true");
                            string parametrosExtra = "";
                            if (identidadLink != mControladorBase.UsuarioActual.IdentidadID)
                            {
                                parametrosExtra += "?IDCambioIdentidda=" + identidadLink.ToString();
                            }
                            string urlRedirect = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, gestorProyectos.ListaProyectos[documentoWeb.ProyectoID].NombreCorto, UrlPerfil, documentoWeb, false);
                            gestorProyectos.Dispose();

                            return Redirect(urlRedirect);
                        }
                    }
                }

                #endregion

                //2º Mirar en la primera comunidad que tengo acceso por orden de compartición

                #region 2º comprobación

                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocBR in gestionDocumentacion.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.Eliminado == false).OrderBy(doc => doc.FechaPublicacion.Value).ToList())
                {
                    Guid baseRecursosID = filaDocBR.BaseRecursosID;

                    List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBRProy = gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRec => baseRec.BaseRecursosID.Equals(baseRecursosID)).ToList();
                    if (filasBRProy.Count > 0)
                    {
                        bool proyectoPrivado = (gestorProyectos.ListaProyectos[filasBRProy[0].ProyectoID].FilaProyecto.TipoAcceso == (short)TipoAcceso.Privado || gestorProyectos.ListaProyectos[filasBRProy[0].ProyectoID].FilaProyecto.TipoAcceso == (short)TipoAcceso.Reservado);
                        if (permitirProyPrivados || !proyectoPrivado)
                        {
                            Guid identidadLink = Guid.Empty;
                            if (ParticipaUsuarioActualEnProyecto(filasBRProy[0].ProyectoID))
                            {
                                identidadLink = IdentidadUsuarioActualEnProyecto(filasBRProy[0].ProyectoID);
                            }

                            if (!identidadLink.Equals(Guid.Empty) || !proyectoPrivado)
                            {
                                //Response.Redirect("perfilBaseRecursosFicha.aspx?ID=" + identidadLink + "&docID=" + documentoWeb.Clave.ToString() + "&comunidad=true");
                                string parametrosExtra = "";
                                if (identidadLink != mControladorBase.UsuarioActual.IdentidadID && !identidadLink.Equals(Guid.Empty))
                                {
                                    parametrosExtra += "?IDCambioIdentidda=" + identidadLink.ToString();
                                }
                                string urlRedirect = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, gestorProyectos.ListaProyectos[filasBRProy[0].ProyectoID].NombreCorto, UrlPerfil, documentoWeb, false);
                                gestorProyectos.Dispose();

                                if (redirect301)
                                {
                                    return RedirectPermanent(urlRedirect);
                                }
                                else
                                {
                                    return Redirect(urlRedirect);
                                }
                                //break;
                            }
                        }
                    }
                }

                #endregion

                //3º Mirar a la primera persona que lo ha guardado en una carpeta pública por orden de compartición

                #region 3º comprobación

                List<Guid> listaUsuarioRecurso = new List<Guid>();
                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosUsuario filaBRusu in gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosUsuario)
                {
                    listaUsuarioRecurso.Add(filaBRusu.UsuarioID);
                }

                List<Guid> listaOrgaRecurso = new List<Guid>();
                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion filaBROrg in gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion)
                {
                    listaOrgaRecurso.Add(filaBROrg.OrganizacionID);
                }

                TesauroCN tesauroCN2 = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperTesauro tesauroDW = tesauroCN2.ObtenerTesaurosDeListaUsuarios(listaUsuarioRecurso);
                tesauroDW.Merge(tesauroCN2.ObtenerTesaurosDeListaOrganizaciones(listaOrgaRecurso));
                gestionDocumentacion.GestorTesauro = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);
                tesauroCN2.Dispose();

                documentoWeb.RecargarCategoriasTesauro();
                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocBR in gestionDocumentacion.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => !doc.Eliminado && doc.FechaPublicacion.HasValue).OrderBy(doc => doc.FechaPublicacion.Value).ToList())
                {
                    Guid baseRecursosID = filaDocBR.BaseRecursosID;

                    List<AD.EntityModel.Models.Documentacion.BaseRecursosUsuario> filasBRUsur = gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Where(item => item.BaseRecursosID.Equals(baseRecursosID)).ToList();

                    if (filasBRUsur.Count > 0)
                    {
                        Guid categoriaPublica = gestionDocumentacion.GestorTesauro.TesauroDW.ListaTesauroUsuario.Where(item => item.UsuarioID.Equals(filasBRUsur[0])).FirstOrDefault().CategoriaTesauroPublicoID.Value;
                        List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasDocVinCat = gestionDocumentacion.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.BaseRecursosID.Equals(baseRecursosID)).ToList();
                        foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocVinCat in filasDocVinCat)
                        {
                            if (gestionDocumentacion.GestorTesauro.ListaCategoriasTesauro[filaDocVinCat.CategoriaTesauroID].PadreNivelRaiz.Clave == categoriaPublica)
                            {
                                Guid identidadLink = Guid.Empty;
                                if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto)
                                {
                                    identidadLink = mControladorBase.UsuarioActual.IdentidadID;
                                }
                                else
                                {
                                    if (ParticipaUsuarioActualEnProyecto(ProyectoAD.MetaProyecto))
                                    {
                                        identidadLink = IdentidadUsuarioActualEnProyecto(ProyectoAD.MetaProyecto);
                                    }
                                }
                                if ((identidadLink != Guid.Empty) && (!mControladorBase.UsuarioActual.EsUsuarioInvitado))
                                {
                                    //Response.Redirect("perfilRecursosCompartidosFicha.aspx?ID=" + identidadLink + "&docID=" + documentoWeb.Clave.ToString() + "&identidad=" + filaDocBR.IdentidadPublicacionID);
                                    GestionIdentidades gestorIdentAux = new GestionIdentidades(new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerIdentidadPorID(filaDocBR.IdentidadPublicacionID.Value, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    string urlRedirect = GnossUrlsSemanticas.GetURLBaseRecursosFichaMyGnoss(BaseURLIdioma, UtilIdiomas, UrlPerfil, gestorIdentAux.ListaIdentidades[filaDocBR.IdentidadPublicacionID.Value], documentoWeb);
                                    gestorIdentAux.Dispose();
                                    gestorProyectos.Dispose();

                                    if (redirect301)
                                    {
                                        return RedirectPermanent(urlRedirect);
                                    }
                                    else
                                    {
                                        return Redirect(urlRedirect);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        List<AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion> filasBROrg = gestionDocumentacion.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Where(item => item.BaseRecursosID.Equals(baseRecursosID)).ToList();
                        if (filasBROrg.Count > 0)
                        {
                            Guid categoriaPublica = gestionDocumentacion.GestorTesauro.TesauroDW.ListaTesauroOrganizacion.Where(item => item.OrganizacionID.Equals(filasBROrg.FirstOrDefault().OrganizacionID)).FirstOrDefault().CategoriaTesauroPublicoID.Value;
                            List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasDocVinCat = gestionDocumentacion.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(baseRec => baseRec.BaseRecursosID.Equals(baseRecursosID)).ToList();
                            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocVinCat in filasDocVinCat)
                            {
                                if (gestionDocumentacion.GestorTesauro.ListaCategoriasTesauro[filaDocVinCat.CategoriaTesauroID].PadreNivelRaiz.Clave == categoriaPublica)
                                {
                                    Guid identidadLink = Guid.Empty;
                                    if (mControladorBase.UsuarioActual.ProyectoID == ProyectoAD.MetaProyecto)
                                    {
                                        identidadLink = mControladorBase.UsuarioActual.IdentidadID;
                                    }
                                    else
                                    {
                                        if (ParticipaUsuarioActualEnProyecto(ProyectoAD.MetaProyecto))
                                        {
                                            identidadLink = IdentidadUsuarioActualEnProyecto(ProyectoAD.MetaProyecto);
                                        }
                                    }
                                    if (identidadLink != Guid.Empty)
                                    {
                                        //Response.Redirect("perfilRecursosCompartidosFicha.aspx?ID=" + identidadLink + "&docID=" + documentoWeb.Clave.ToString() + "&identidad=" + filaDocBR.IdentidadPublicacionID);
                                        GestionIdentidades gestorIdentAux = new GestionIdentidades(new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerIdentidadPorID(filaDocBR.IdentidadPublicacionID.Value, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                                        string urlRedirect = GnossUrlsSemanticas.GetURLBaseRecursosFichaMyGnoss(BaseURLIdioma, UtilIdiomas, UrlPerfil, gestorIdentAux.ListaIdentidades[filaDocBR.IdentidadPublicacionID.Value], documentoWeb);
                                        gestorIdentAux.Dispose();
                                        gestorProyectos.Dispose();

                                        if (redirect301)
                                        {
                                            return RedirectPermanent(urlRedirect);
                                        }
                                        else
                                        {
                                            return Redirect(urlRedirect);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion

                //// 4º Si no tienes acceso a ninguna de las comunidades en las que esta compartido o publicado
                //// Muestra una lista de las comunidades publicas o de acceso restringido en las que esta publicado o compartido
                //#region 4º comprobacion

                //this.lblComunidadesPrivadas.Text = "";
                //string concatenador = "";
                //foreach (Proyecto proy in gestorProyectos.ListaProyectos.Values)
                //{
                //    if (!(proy.FilaProyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) || proy.FilaProyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado)))
                //        this.lblComunidadesPrivadas.Text += concatenador + "<a href='" + mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proy.NombreCorto) + "'>" + proy.Nombre + "</a>";
                //    concatenador = ", ";
                //}

                //#endregion



                //gestionDocumentacion.GestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(ProyectoAD.MetaProyecto));
                //documentacionCN.Dispose();
                //Documento mDocumento = gestionDocumentacion.ListaDocumentos[recursoID];
                ////AgregarRecurso(mDocumento);

                //if ((ProyectoSeleccionado != null) && (ProyectoSeleccionado.Clave.Equals(mDocumento.ProyectoID)))
                //{
                //    mGestorProyecto = ProyectoSeleccionado.GestorProyectos;
                //}
                //else
                //{
                //    mGestorProyecto = new GestionProyecto(new ProyectoCN(mEntityContext, mLoggingService, mConfigService).ObtenerProyectoPorID(mDocumento.ProyectoID));
                //}
                //mProyectoSeleccionado = mGestorProyecto.ListaProyectos[mDocumento.ProyectoID];

                //ComprobarProyecto(gestionDocumentacion.ListaDocumentos[recursoID]);
            }

            return Redirect(BaseURLSinHTTP + "/404.aspx?gone=410");
        }

        /// <summary>
        /// Acción encargada de redireccionar las urls registradas con el nuevo modelo EF
        /// </summary>
        /// <param name="redireccionID">Identificador de la redirección</param>
        /// <returns></returns>
        public ActionResult RedireccionamientoModeloEF(Guid redireccionID)
        {
            string filtrosOriginales = HttpContext.Request.QueryString.ToString();
            string urlRedireccion = string.Empty;

            mLoggingService.AgregarEntrada("RedirectController RedireccionamientoModeloEF. Antes de ObtenerRedireccionRegistroRutaPorDominio. ");

            //obtenerlas de cache??
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            RedireccionRegistroRuta filaRedireccion = proyCN.ObtenerRedireccionRegistroRutaPorRedireccionID(redireccionID);
            proyCN.Dispose();

            mLoggingService.AgregarEntrada("RedirectController RedireccionamientoModeloEF. Después de ObtenerRedireccionRegistroRutaPorRedireccionID. redireccionID: " + redireccionID);

            if (filaRedireccion != null)
            {
                mLoggingService.AgregarEntrada("RedirectController TablaRedireccionamiento. Antes Obtener UrlRedireccion");

                urlRedireccion = ObtenerUrlRedireccion(filaRedireccion);

                mLoggingService.AgregarEntrada("RedirectController TablaRedireccionamiento. Tras Obtener UrlRedireccion: " + urlRedireccion);
            }

            if (!string.IsNullOrEmpty(urlRedireccion))
            {
                if (!urlRedireccion.StartsWith("/") && !urlRedireccion.StartsWith("http://") && !urlRedireccion.StartsWith("https://"))
                {
                    urlRedireccion = "/" + urlRedireccion;
                }

                return RedirectPermanent(urlRedireccion);
            }
            else if (!string.IsNullOrEmpty(NombreCortoProyectoConexion))
            {
                return Redirect("/" + UtilIdiomas.GetText("URLSEM", "COMUNIDAD") + "/" + NombreCortoProyectoConexion);
            }
            else
            {
                return RedireccionarAPaginaNoEncontrada();
            }
        }

        /// <summary>
        /// Acción para redirigir todos los enlaces que vengan con /it/comunità a /it/comunita
        /// </summary>
        /// <returns></returns>
        public ActionResult RedireccionarComunitaItaliano()
        {
            return RedirectPermanent(Request.Path.ToString().Replace("/it/comunità/", "/it/comunita/"));
        }

        private string ObtenerUrlRedireccion(RedireccionRegistroRuta pFilaRedireccion)
        {
            string urlRedireccion = string.Empty;
            Dictionary<string, List<string>> dicUrlYlistaParametros = ObtenerUrlRedireccionYListaParametrosRedireccion(pFilaRedireccion);
            if (dicUrlYlistaParametros != null && dicUrlYlistaParametros.Count > 0)
            {
                urlRedireccion = dicUrlYlistaParametros.Keys.First();

                //si la url contiene parametros, reemplazar sus valores
                if (dicUrlYlistaParametros[urlRedireccion] != null)
                {
                    foreach (string parametro in dicUrlYlistaParametros[urlRedireccion])
                    {
                        string valorParametro = RequestParams(parametro);
                        urlRedireccion = urlRedireccion.Replace(parametro, valorParametro);
                    }
                }
            }
            return urlRedireccion;
        }

        private Dictionary<string, List<string>> ObtenerUrlRedireccionYListaParametrosRedireccion(RedireccionRegistroRuta pFilaRedireccion)
        {
            Dictionary<string, List<string>> dicUrlYlistaParametros = null;
            List<string> listaParametros = null;
            string urlRedireccion = null;

            //comprobar si es directa o parametrizada
            if (string.IsNullOrEmpty(pFilaRedireccion.NombreParametro))
            {
                //es directa.
                RedireccionValorParametro filaValorRedirec = pFilaRedireccion.RedireccionValorParametro.FirstOrDefault(r => r.RedireccionID.Equals(pFilaRedireccion.RedireccionID));

                if (!string.IsNullOrEmpty(filaValorRedirec.UrlRedireccion))
                {
                    listaParametros = ExtraerParametrosUrlRedireccion(filaValorRedirec.UrlRedireccion);
                    urlRedireccion = filaValorRedirec.UrlRedireccion;
                }
            }
            else
            {
                //es parametrizada.
                string valorParametro = RequestParams(pFilaRedireccion.NombreParametro);
                if (!string.IsNullOrEmpty(valorParametro))
                {
                    RedireccionValorParametro filaValorRedirec = pFilaRedireccion.RedireccionValorParametro.FirstOrDefault(r => r.RedireccionID.Equals(pFilaRedireccion.RedireccionID) && r.ValorParametro.Equals(valorParametro));

                    if (!string.IsNullOrEmpty(filaValorRedirec.UrlRedireccion))
                    {
                        listaParametros = ExtraerParametrosUrlRedireccion(filaValorRedirec.UrlRedireccion);
                        urlRedireccion = filaValorRedirec.UrlRedireccion;
                    }
                }
                //listaParametros.Add(pFilaRedireccion.NombreParametro);
            }

            if (!string.IsNullOrEmpty(urlRedireccion))
            {
                dicUrlYlistaParametros = new Dictionary<string, List<string>>();
                dicUrlYlistaParametros.Add(urlRedireccion, listaParametros);
            }

            return dicUrlYlistaParametros;
        }

        private List<string> ExtraerParametrosUrlRedireccion(string pUrlRedireccion)
        {
            List<string> listaParams = null;
            string[] partes = pUrlRedireccion.Split(new char[] { '{' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes != null && partes.Any())
            {
                foreach (string parte in partes)
                {
                    if (parte.Contains("}"))
                    {
                        if (listaParams == null)
                        {
                            listaParams = new List<string>();
                        }

                        string param = parte.Substring(0, parte.IndexOf("}") + 1); //conservo las llaves {nomParam}
                        if (!listaParams.Contains(param))
                        {
                            listaParams.Add(param);
                        }
                    }
                }
            }

            return listaParams;
        }
    }
}
