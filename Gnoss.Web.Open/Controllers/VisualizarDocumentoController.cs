using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    [TypeFilter(typeof(NoTrackingEntityFilter))]
    public class VisualizarDocumentoController : ControllerBaseWeb
    {
        public const string ID_GOOGLE = "##idgoogle##";
        protected Guid? mUsuarioOauth;

        public VisualizarDocumentoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
        : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Métodos de eventos

        private string UrlReferrer
        {
            get
            {
                return Request.Headers["Referer"].ToString();
            }
        }
        [HttpGet]
        public IActionResult Index()
        {   
            mLoggingService.AgregarEntrada("Entra page load VisualizarDocumento");
            bool OmitirRedireccionFlash = false;
            string mNombreDocumento = string.Empty;
            string mGoogleID = string.Empty;

            #region Presentaciones Flash
            //Las presentaciones flash se le debe mostrar al usuario invitado(excepto en privadas)
            if (Request.Query["ext"] == ".swf" && Request.Query.ContainsKey("doc") && Request.Query.ContainsKey("ID") && Request.Query.ContainsKey("proy"))
            {
                Guid docID = new Guid(Request.Query["doc"]);
                Guid identidadID = new Guid(Request.Query["ID"]);
                Guid proyectoID = new Guid(Request.Query["proy"]);

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                //Comprobamos si realmente es un swf  
                DataWrapperDocumentacion dataWrapperDocumentacion = docCN.ObtenerDocumentoPorID(docID);
                dataWrapperDocumentacion.Merge(docCN.ObtenerEditoresDocumento(docID));

                Documento doc = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext).ListaDocumentos[docID];
                Identidad identidad = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(identidadID, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication).ListaIdentidades[identidadID];

                bool esSWF = Request.Headers.ContainsKey("flashDocSem") || doc.Enlace.ToLower().EndsWith(".swf");
                bool tieneAcceso = true;
                if (doc.FilaDocumentoWebVinBR != null && doc.FilaDocumentoWebVinBR.PrivadoEditores)
                {
                    tieneAcceso = false;
                    foreach (EditorRecurso editor in doc.ListaPerfilesEditores.Values)
                    {
                        if (editor.FilaEditor.PerfilID == identidad.PerfilID)
                        {
                            tieneAcceso = true;
                            break;
                        }
                    }

                    if (doc.CreadorID == identidadID)
                    {
                        tieneAcceso = true;
                    }
                }

                //Comprobamos si se lo debemos mostrar
                //bool puedeVerRecurso=identidadCN.ParticipaIdentidadEnComunidad(identidadID, proyectoID)|| (ProyectoSeleccionado.TipoAcceso!=TipoAcceso.Privado && ProyectoSeleccionado.TipoAcceso!=TipoAcceso.Reservado); 
                bool puedeVerRecurso = identidadCN.ParticipaPerfilEnComunidad(identidad.PerfilID, proyectoID) || (ProyectoSeleccionado.TipoAcceso != TipoAcceso.Privado && ProyectoSeleccionado.TipoAcceso != TipoAcceso.Reservado);

                docCN.Dispose();
                identidadCN.Dispose();

                if (esSWF && tieneAcceso && puedeVerRecurso)
                {
                    OmitirRedireccionFlash = true;
                }
            }
            #endregion

            if (Request.Query.ContainsKey("ontologia"))
            {
                mLoggingService.AgregarEntrada("Entra if ontologia");
                DevolverOntologia();
                return null;
            }

            if (!OmitirRedireccionFlash && EsPeticionOAuth)
            {
                OmitirRedireccionFlash = ComprobarPermisosOauth(false);
            }

            string tipoEntidad = "", ext = "", nombre = "", archivoAdjuntoSem = "", idiomaFichero = "";
            Guid organizacion = Guid.Empty, proyecto = Guid.Empty, ficheroID = Guid.Empty, personaID = Guid.Empty, ontologiaAdjuntoSem = Guid.Empty;
            List<AD.EntityModel.Models.IdentidadDS.Identidad> listaFilasIdentidad = null;

            if (!OmitirRedireccionFlash)
            {
                if (ParametroProyecto.ContainsKey(ParametroAD.PermitirDescargaIdentidadInvitada))
                {
                    PaginaVisibleEnPrivada = ParametroProyecto[ParametroAD.PermitirDescargaIdentidadInvitada].Equals("1");
                }

                listaFilasIdentidad = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID == proyecto && identidad.Tipo != 3).ToList();
                mLoggingService.AgregarEntrada("parametros recogidos");

                if (listaFilasIdentidad.Any())
                {
                    IdentidadActual = IdentidadActual.GestorIdentidades.ListaIdentidades[listaFilasIdentidad.First().IdentidadID];
                }

                if (!mControladorBase.UsuarioActual.EsUsuarioInvitado && mControladorBase.UsuarioActual.EsIdentidadInvitada && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado) && !PaginaVisibleEnPrivada)
                {
                    mLoggingService.AgregarEntrada("PageLoad: !OmitirRedireccionFlash, !EsUsuarioInvitado y EsIdentidadInvitada y (proyecto con acceso privado ó reservado)");
                    Response.Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
            }

            bool descargaDesdeRdf = !string.IsNullOrEmpty(Request.Query["dscr"]) && Request.Query["dscr"].Equals("true");

            //Obtiene los parámetros de la petición
            if (Request.Query.ContainsKey("tipo"))
            {
                tipoEntidad = Request.Query["tipo"];
            }
            if (Request.Query.ContainsKey("org"))
            {
                organizacion = new Guid(Request.Query["org"]);
            }
            if (Request.Query.ContainsKey("proy"))
            {
                proyecto = new Guid(Request.Query["proy"]);
            }
            if (Request.Query.ContainsKey("doc"))
            {
                ficheroID = new Guid(Request.Query["doc"]);
            }
            if (Request.Query.ContainsKey("ext"))
            {
                ext = Request.Query["ext"];
            }
            if (Request.Query.ContainsKey("nombre"))
            {
                nombre = Request.Query["nombre"];
            }
            if (Request.Query.ContainsKey("personaID"))
            {
                personaID = new Guid(Request.Query["personaID"]);
            }
            if (Request.Query.ContainsKey("archivoAdjuntoSem"))
            {
                archivoAdjuntoSem = Request.Query["archivoAdjuntoSem"];
            }
            if (Request.Query.ContainsKey("ontologiaAdjuntoSem"))
            {
                ontologiaAdjuntoSem = new Guid(Request.Query["ontologiaAdjuntoSem"]);
            }
            if (Request.Query.ContainsKey("idiomaFichero"))
            {
                idiomaFichero = Request.Query["idiomaFichero"];
            }
            if (Request.Query.ContainsKey("archivo"))
            {
                string archivo = Request.Query["archivo"];
                archivoAdjuntoSem = archivo.Substring(0, archivo.LastIndexOf('.'));
                ext = archivo.Substring(archivo.LastIndexOf('.'));
            }
            bool descargaIframe = !string.IsNullOrEmpty(Request.Query["iframe"]) && Request.Query["iframe"].Equals("true");

            DocumentacionCN docsCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            //Obtengo el documento
            GestorDocumental gestorDocs = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext);

            mLoggingService.AgregarEntrada("compruebo gestor documental");
            
            //Carga la base de recursos del usuario:
            gestorDocs.DataWrapperDocumentacion.ListaBaseRecursos = new List<AD.EntityModel.Models.Documentacion.BaseRecursos>();
            if (gestorDocs.DataWrapperDocumentacion != null)
            {
                gestorDocs.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Clear();
                gestorDocs.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Clear();
                gestorDocs.DataWrapperDocumentacion.ListaDocumentoComentario.Clear();
            }

            gestorDocs.DataWrapperDocumentacion.ListaBaseRecursosUsuario = new List<AD.EntityModel.Models.Documentacion.BaseRecursosUsuario>();
            gestorDocs.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion = new List<AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion>();

            if (!proyecto.Equals(Guid.Empty))
            {
                docsCN.ObtenerBaseRecursosProyecto(gestorDocs.DataWrapperDocumentacion, proyecto);
            }

            Guid baseRecursosActual = Guid.Empty;
            if (gestorDocs.DataWrapperDocumentacion.ListaBaseRecursos.Count > 0)
            {
                baseRecursosActual = gestorDocs.BaseRecursosIDActual;
            }

            docsCN.ObtenerDocumentoPorIDCargarTotal(ficheroID, gestorDocs.DataWrapperDocumentacion, true, true, baseRecursosActual);

            gestorDocs.DataWrapperDocumentacion.Merge(docsCN.ObtenerVersionesDocumentoPorID(ficheroID));
            gestorDocs.CargarDocumentos();

            //Comprueba si se está accediendo al recurso desde una comunidad diferente a la que se creó el recurso
            ComprobarAccesoDesdeOtroProyecto(proyecto, ficheroID);
                        
            mLoggingService.AgregarEntrada("compruebo OmitirRedireccionFlash " + OmitirRedireccionFlash);

            if (!OmitirRedireccionFlash)
            {
                if (gestorDocs.ListaDocumentos.ContainsKey(ficheroID))
                {
                    Documento doc = gestorDocs.ListaDocumentos[ficheroID];

                    if (mControladorBase.UsuarioActual != null && mControladorBase.UsuarioActual.EsUsuarioInvitado)
                    {
                        ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        //ParametroGeneralDS paramDS = paramCL.ObtenerParametrosGeneralesDeProyecto(proyecto);
                        GestorParametroGeneral gestorParametroGeneral = paramCL.ObtenerParametrosGeneralesDeProyecto(proyecto);
                        paramCL.Dispose();
                        string permitirDescargaParam = "PermitirDescargaIdentidadInvitada";
                        bool permitirVisualizarDescargarInvitado = false;
                        if (ParametroProyecto.ContainsKey(permitirDescargaParam))
                        {
                            if (ParametroProyecto[permitirDescargaParam] == "1" || ParametroProyecto[permitirDescargaParam] == "true")
                            {
                                permitirVisualizarDescargarInvitado = true;
                            }
                            else if (ParametroProyecto[permitirDescargaParam] == "0" || ParametroProyecto[permitirDescargaParam] == "false")
                            {
                                permitirVisualizarDescargarInvitado = false;
                            }
                        }
                        if (!permitirVisualizarDescargarInvitado)
                        {
                            if (!proyecto.Equals(ProyectoAD.MetaProyecto))
                            {
                                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                string nombreCortoProyecto = proyCN.ObtenerNombreCortoProyecto(proyecto);
                                string urlProyecto = proyCN.ObtenerURLPropiaProyecto(proyecto);
                                proyCN.Dispose();

                                string url = "";
                                if (UrlReferrer != null)
                                {
                                    url = new Uri(UrlReferrer).AbsolutePath.Trim('/');
                                }
                                else
                                {
                                    string idioma = "";
                                    if (!UtilIdiomas.LanguageCode.Equals("es"))
                                    {
                                        idioma += "/" + UtilIdiomas.LanguageCode;
                                    }

                                    url = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(idioma, UtilIdiomas, nombreCortoProyecto, "", doc, false);
                                }

                                mLoggingService.AgregarEntrada("PageLoad: !OmitirRedireccionFlash, ListaDocumentosContieneficheroID " + ficheroID + ", EsUsuarioInvitado, !PermitirUsuNoLoginDescargDoc, !MetaProyecto");

                                Response.Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, nombreCortoProyecto) + "/login/redirect/" + url);
                            }
                            else
                            {
                                mLoggingService.AgregarEntrada("PageLoad: !OmitirRedireccionFlash, ListaDocumentosContieneficheroID " + ficheroID + ", EsUsuarioInvitado, !PermitirUsuNoLoginDescargDoc, MetaProyecto");

                                mControladorBase.RedirigirUsuarioInvitadoARegistrarse(BaseURL, BaseURLIdioma, UtilIdiomas);
                            }
                        }
                    }

                    mLoggingService.AgregarEntrada("compruebo descargarDesdeRDF " + descargaDesdeRdf + " " + UrlReferrer);

                    //Compruebo que viene de la ficha del recurso o del historial o con una cadena oauth válida, si no le redirecciono a la ficha
                    bool redireccionarFicha = true;

                    if (descargaDesdeRdf || (!string.IsNullOrEmpty(UrlReferrer)))
                    {
                        //docsCN.ObtenerDocumentoPorIDCargarTotal(ficheroID, gestorDocs.DocumentacionDS, true, false, null);
                        //gestorDocs.CargarNuevosHijos();*

                        //Cargo los grupos de los editores
                        List<Guid> listaGrupos = new List<Guid>();
                        foreach (GrupoEditorRecurso grupoEditor in doc.ListaGruposEditores.Values)
                        {
                            if (!listaGrupos.Contains(grupoEditor.Clave))
                            {
                                listaGrupos.Add(grupoEditor.Clave);
                            }
                        }
                        if (listaGrupos.Count > 0)
                        {
                            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            DataWrapperIdentidad identDW = identidadCN.ObtenerGruposPorIDGrupo(listaGrupos);

                            GestionIdentidades gestorIdent = new GestionIdentidades(identDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                            identidadCN.Dispose();

                            if (gestorDocs.GestorIdentidades == null)
                            {
                                gestorDocs.GestorIdentidades = gestorIdent;
                            }
                            else
                            {
                                gestorDocs.GestorIdentidades.DataWrapperIdentidad.Merge(gestorIdent.DataWrapperIdentidad);
                                gestorDocs.GestorIdentidades.RecargarHijos();
                            }
                        }
                        if (ProyectoSeleccionado.EsAdministradorUsuario(mControladorBase.UsuarioActual.UsuarioID))
                        {
                            redireccionarFicha = false;
                        }
                        else if (doc.EsEditoraOLectoraIdentidad(IdentidadActual))
                        {

                            if (descargaDesdeRdf || (UrlReferrer.ToLower().Contains($"/{ficheroID.ToString().ToLower()}")))
                            {
                                redireccionarFicha = false;
                            }
                            else if (!doc.FilaDocumento.UltimaVersion)
                            {
                                if (descargaDesdeRdf || doc.UltimaVersion == null || UrlReferrer.EndsWith($"/{doc.UltimaVersion.NombreSem(UtilIdiomas.LanguageCode)}/{doc.UltimaVersion.Clave}"))
                                {
                                    redireccionarFicha = false;
                                }
                            }
                        }
                        else if (IdentidadActual != null && (descargaDesdeRdf || (UrlReferrer.ToLower().Contains($"/{ficheroID.ToString().ToLower()}")))) //Si el recurso es publico en alguna otra comunidad a la que pertenece el usuario, le dejamos descargarlo
                        {
                            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVin in doc.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos)
                            {
                                if (!filaDocVin.PrivadoEditores)
                                {
                                    List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBRProy = doc.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRec => baseRec.BaseRecursosID.Equals(filaDocVin.BaseRecursosID)).ToList();

                                    if (filasBRProy.Count > 0)
                                    {
                                        Guid proyectoID = filasBRProy.First().ProyectoID;
                                        List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(proyectoID) && !ident.FechaBaja.HasValue && !ident.FechaExpulsion.HasValue).ToList();

                                        if (filasIdent.Count > 0)
                                        {
                                            redireccionarFicha = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (doc.FilaDocumentoWebVinBR != null && doc.FilaDocumentoWebVinBR.PrivadoEditores && !doc.FilaDocumento.UltimaVersion && doc.UltimaVersionID != Guid.Empty)//Hay que mirar los editores de la última versión.
                        {
                            //Carga la base de recursos del usuario:
                            gestorDocs.DataWrapperDocumentacion.ListaBaseRecursos = new List<AD.EntityModel.Models.Documentacion.BaseRecursos>();
                            gestorDocs.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Clear();
                            gestorDocs.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Clear();
                            gestorDocs.DataWrapperDocumentacion.ListaDocumentoComentario.Clear();
                            gestorDocs.DataWrapperDocumentacion.ListaBaseRecursosUsuario = new List<AD.EntityModel.Models.Documentacion.BaseRecursosUsuario>();
                            gestorDocs.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion = new List<AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion>();

                            if (proyecto != Guid.Empty)
                            {
                                docsCN.ObtenerBaseRecursosProyecto(gestorDocs.DataWrapperDocumentacion, proyecto);
                            }

                            docsCN.ObtenerDocumentoPorIDCargarTotal(doc.UltimaVersionID, doc.GestorDocumental.DataWrapperDocumentacion, true, true, gestorDocs.BaseRecursosIDActual);
                            gestorDocs.CargarDocumentos(false);

                            if (gestorDocs.ListaDocumentos[doc.UltimaVersionID].ListaPerfilesEditores.ContainsKey(IdentidadActual.FilaIdentidad.PerfilID))
                            {
                                redireccionarFicha = false;
                            }
                        }

                        Documento docUlt = doc.UltimaVersion;
                    }
                    docsCN.Dispose();

                    if (redireccionarFicha && EsPeticionOAuth)
                    {
                        redireccionarFicha = ComprobarPermisosOauth(false);
                    }

                    if (redireccionarFicha)
                    {
                        RedireccionarAFicha(organizacion, proyecto, doc);
                    }
                }
            }

            //Llamada al servicio web para obtener el contenido del fichero.
            GestionDocumental gestorDocumental = new GestionDocumental(mLoggingService, mConfigService);
            gestorDocumental.Url = UrlServicioWebDocumentacion;

            byte[] byteArray = null;

            mLoggingService.AgregarEntrada("Obtengo archivo del gestor documental");
            string[] separadores = { ID_GOOGLE };

            mLoggingService.AgregarEntrada("obtengo el recurso ");

            if (nombre != string.Empty)
            {
                HttpRequest request = Request;
                string[] nombreDoc = nombre.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                mNombreDocumento = nombreDoc[0];

                if (nombreDoc.Length > 1)
                {
                    mGoogleID = nombreDoc[1];

                    FileInfo informacionNombre = new FileInfo(mGoogleID);
                    mNombreDocumento += informacionNombre.Extension;
                    mGoogleID = ExtraerNombreFicheroSinExtension(informacionNombre.Name);
                }
            }
            if (archivoAdjuntoSem != "")
            {
                if (TieneGoogleDriveConfigurado)
                {
                    try
                    {
                        /* TODO Javier migrar
                        OAuthGoogleDrive gd = new OAuthGoogleDrive();
                        string[] googleID = archivoAdjuntoSem.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                        byteArray = gd.DescargarDocumento(googleID[1], ext.Substring(1, ext.Length - 1));
                        nombre = googleID[0] + ext;*/
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLog("Problema al descargar fichero '" + archivoAdjuntoSem + "' de Google Drive:" + ex.ToString());
                    }
                }
                else
                {
                    string directorio = Path.Combine(UtilArchivos.ContentDocumentosSemAntiguo, ontologiaAdjuntoSem.ToString().Substring(0, 3), ontologiaAdjuntoSem.ToString());
                    if (!string.IsNullOrEmpty(idiomaFichero))
                    {
                        directorio = Path.Combine(directorio, idiomaFichero);
                    }

                    byteArray = gestorDocumental.ObtenerDocumentoDeDirectorio(directorio, archivoAdjuntoSem, ext);

                    if (byteArray == null || byteArray.Count() == 0)//Miramos si el documento está en el nuevo directorio:
                    {
                        directorio = Path.Combine(UtilArchivos.ContentDocumentosSem, UtilArchivos.DirectorioDocumento(ficheroID));
                        if (!string.IsNullOrEmpty(idiomaFichero))
                        {
                            directorio = Path.Combine(directorio, idiomaFichero);
                        }

                        byteArray = gestorDocumental.ObtenerDocumentoDeDirectorio(directorio, archivoAdjuntoSem, ext);
                    }

                    if (archivoAdjuntoSem.Contains("_"))
                    {
                        string guidValor = archivoAdjuntoSem.Substring(archivoAdjuntoSem.LastIndexOf("_") + 1);
                        Guid aux = Guid.Empty;

                        if (Guid.TryParse(guidValor, out aux))
                        {
                            archivoAdjuntoSem = archivoAdjuntoSem.Substring(0, archivoAdjuntoSem.LastIndexOf("_"));
                        }
                    }

                    nombre = archivoAdjuntoSem + ext;
                }
            }
            else if (tipoEntidad == "Ontologia")
            {
                try
                {
                    CallFileService servicioArch = new CallFileService(mConfigService, mLoggingService);

                    if (ext == ".xml")
                    {
                        byteArray = servicioArch.ObtenerXmlOntologiaBytes(ficheroID);
                    }
                    else
                    {
                        byteArray = servicioArch.ObtenerOntologiaBytes(ficheroID);
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex.ToString());
                }
            }
            else if (!personaID.Equals(Guid.Empty))
            {
                if (TieneGoogleDriveConfigurado)
                {
                    if (nombre != string.Empty)
                    {
                        //TODO Javier migrar Preguntar 
                        //OAuthGoogleDrive gd = new OAuthGoogleDrive();
                        //byteArray = gd.DescargarDocumento(mGoogleID, ext.Substring(1, ext.Length - 1));
                    }
                }
                else
                {
                    byteArray = gestorDocumental.ObtenerDocumentoDeBaseRecursosUsuario(tipoEntidad, personaID, ficheroID, ext);
                }
            }
            else if (!proyecto.Equals(Guid.Empty))
            {
                if (TieneGoogleDriveConfigurado)
                {
                    if (nombre != string.Empty)
                    {
                        //TODO Javier migrar
                        //OAuthGoogleDrive gd = new OAuthGoogleDrive();
                        //byteArray = gd.DescargarDocumento(mGoogleID, ext.Substring(1, ext.Length - 1));
                    }
                }
                else
                {
                    byteArray = gestorDocumental.ObtenerDocumento(tipoEntidad, organizacion, proyecto, ficheroID, ext);
                }
            }
            else
            {
                if (TieneGoogleDriveConfigurado)
                {
                    if (nombre != string.Empty)
                    {
                        //TODO Javier migrar
                        //OAuthGoogleDrive gd = new OAuthGoogleDrive();
                        //byteArray = gd.DescargarDocumento(mGoogleID, ext.Substring(1, ext.Length - 1));
                    }
                }
                else
                {
                    byteArray = gestorDocumental.ObtenerDocumentoDeBaseRecursosOrganizacion(tipoEntidad, organizacion, ficheroID, ext);
                }
            }

            mLoggingService.AgregarEntrada("Obtenido");

            string contentType = "application/octet-stream";
            string nombreCarpetaContenedora = "";
            string parametroArchivoGnoss = "";
            bool agregarCabecera = true;

            if (Request.Headers.ContainsKey("pdf"))
            {
                parametroArchivoGnoss = "pdf";
                contentType = "application/pdf";
                nombreCarpetaContenedora = "PDF";
                nombre = nombre.Replace(".gnoss", ".pdf");
            }
            else if (Request.Headers.ContainsKey("rdf"))
            {
                parametroArchivoGnoss = "rdf";
                nombre = nombre.Replace(".gnoss", ".owl");
                nombreCarpetaContenedora = "OWL";

                if (Request.Headers["User-Agent"].ToString() == "Firefox")
                {
                    //Le añado el tipo de contenido
                    contentType = "application/rdf+xml";
                }
            }

            if (ext.ToLower() == ".swf")
            {
                //Le añado el tipo de contenido
                contentType = "application/x-shockwave-flash";
                agregarCabecera = false;
            }

            if ((!string.IsNullOrEmpty(parametroArchivoGnoss)) && (Request.Headers[parametroArchivoGnoss].Equals("true")))
            {
                string rutaTemporales = Path.GetTempPath();
                string rutaArchivoGnoss = Path.Combine(rutaTemporales, ficheroID.ToString() + ".gnoss");

                if ((Request.Headers["User-Agent"].ToString() == "Firefox") || (!parametroArchivoGnoss.Equals("rdf")))
                {
                    agregarCabecera = false;
                }

                if (!System.IO.File.Exists(rutaArchivoGnoss))
                {
                    //Creo archivo zip en la carpeta de temporales
                    FileStream ficheroGnoss = System.IO.File.Create(rutaArchivoGnoss);
                    ficheroGnoss.Write(byteArray, 0, byteArray.Length);
                    ficheroGnoss.Flush();
                    ficheroGnoss.Close();
                }

                BinaryReader lectorPDF = new BinaryReader(UtilZip.DescomprimirFichero(rutaArchivoGnoss, Path.Combine(rutaTemporales, ficheroID.ToString(), nombreCarpetaContenedora), nombreCarpetaContenedora + "/" + nombre));
                byteArray = lectorPDF.ReadBytes((int)lectorPDF.BaseStream.Length);
                lectorPDF.Close();

                nombre = nombre.Replace(".owl", ".rdf");
            }

            mLoggingService.AgregarEntrada("escribo respuesta ");

            if (byteArray != null && byteArray.Length > 0)
            {
                if (mControladorBase.UsuarioActual != null && !mControladorBase.UsuarioActual.EsIdentidadInvitada)
                {
                    // Actualizo el contador del usuario
                    // TODO: Mover al servicio de sockets offline
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCN.ActualizarContadorIdentidad(IdentidadActual.Clave, IdentidadAD.CONTADOR_NUMERO_DESCARGAS);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    docCN.ActualizarNumeroDescargasDocumento(ficheroID, gestorDocs.BaseRecursosIDActual);
                }

                mLoggingService.GuardarLog($"7. {HttpContext.Response.StatusCode} Descargar iframe -> descargarIframe: {descargaIframe}");
                
                if (descargaIframe)
                {
                    agregarCabecera = false;
                    //Añadimos la cabecera http al navegador                    
                    Response.Headers.Add("Content-Disposition", $"inline; filename=\"{HttpUtility.UrlEncode(nombre).Replace("+", " ")}\"");
                    ConcurrentDictionary<string, string> mimetypes = Conexion.ObtenerMimeType();
                    string mimetype = "";
                    if (mimetypes.TryGetValue(ext, out mimetype))
                    {
                        Response.Headers.Add("Content-Type", mimetype);
                        contentType = mimetype;
                    }
                    else
                    {
                        Response.Headers.Add("Content-Type", "application/octet-stream");
                    }
                    return File(byteArray, contentType);
                }
                else if (agregarCabecera)
                {
                    //Le añado el tipo de contenido
                    Response.ContentType = contentType;
                    //Añadimos la cabecera http al navegador
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{HttpUtility.UrlEncode(nombre)}\"");
                }

                //Enviamos todo al explorador
                //Response.Flush();
                //Escribimos el contenido del fichero en la página
                return File(byteArray, contentType, nombre);
            }
            else
            {
                return NotFound("No se encontr&oacute; el documento");
            }
        }

        private bool? mPermisoOauth;
        /// <summary>
        /// Si la petición es Oauth, comprueba si el dueño del token enviado tiene permisos para interactuar con el recurso.
        /// </summary>
        /// <returns>False si TRUE si la URL es incorrecta (redirecciona antes de devolverlo así que imposible que lo devuelva)</returns>
        public bool ComprobarPermisosOauth(bool pRedireccionar)
        {
            if (EsPeticionOAuth)
            {
                if (!mPermisoOauth.HasValue)
                {
                    mPermisoOauth = false;
                    try
                    {
                        //string urlPeticionOauth = UtilOAuth.ObtenerUrlGetDePeticionOAuth(Request);
                        //mLoggingService.AgregarEntrada("ComprobarPermisosOauth: Inicio '" + urlPeticionOauth + "'");

                        /* TODO Javier migrar
                        ServicioOauth servicioOauth = new ServicioOauth();
                        string urlServicioOauth = Conexion.ObtenerUrlServicioOauth() + "/ServicioOauth.asmx";
                        if (urlServicioOauth.StartsWith("https://"))
                        {
                            urlServicioOauth = urlServicioOauth.Replace("https://", "http://");
                        }
                        servicioOauth.Url = urlServicioOauth;

                        mUsuarioOauth = servicioOauth.ObtenerUsuarioAPartirDeUrl(urlPeticionOauth, Request.Method);
                        servicioOauth.Dispose();*/

                        mLoggingService.AgregarEntrada("ComprobarPermisosOauth: UsuarioID Obtenido '" + mUsuarioOauth + "'");

                        if (mUsuarioOauth.HasValue && !mUsuarioOauth.Value.Equals(Guid.Empty))
                        {
                            mPermisoOauth = !ComprobarUsuarioOauthTienePermiso(mUsuarioOauth.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.AgregarEntrada("ComprobarPermisosOauth: Error al obtener el usuarioID del servicio OAuth.");
                        GuardarLogError(" ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                        mPermisoOauth = true;
                    }

                    if (!mPermisoOauth.Value && pRedireccionar) //Devolver página error oauth:
                    {
                        Response.Redirect(mControladorBase.DominoAplicacionConHTTP + "/" + "Error.aspx?errorOAuth=" + System.Net.WebUtility.UrlEncode(mControladorBase.UtilIdiomas.GetText("COMMON", "ERRORURLOAUTH")));
                    }
                }

                return mPermisoOauth.Value;
            }
            //else if (Page.Request["rdf"] != null && Usuario.UsuarioActual.EsIdentidadInvitada && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado)) //Se pide un RDF de una comunidad privada sin usar OAuth -> Error
            //{
            //    Response.Redirect(UtilUsuario.DominoAplicacionConHTTP + "/" + "Error.aspx?errorOAuth=" + System.Net.WebUtility.UrlEncode(GetText("COMMON", "ERRORNOUSAOAUTH")));
            //}

            return true;
        }

        /// <summary>
        /// Comprueba si un usuriaro extraido de OAuth tiene permiso sobre la página demandada.
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        protected bool ComprobarUsuarioOauthTienePermiso(Guid pUsuarioID)
        {
            if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
            {
                return false;
            }
            else
            {
                if (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido)
                {
                    return true;
                }
                else
                {
                    List<Guid> listaUsu = new List<Guid>();
                    listaUsu.Add(pUsuarioID);
                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> listaIdent = identCN.ObtenerIdentidadesIDDeusuariosEnProyecto(ProyectoSeleccionado.Clave, listaUsu, true);
                    identCN.Dispose();

                    return (listaIdent.Count > 0);//Usuario está en la comunidad.
                }
            }
        }

        private void ComprobarAccesoDesdeOtroProyecto(Guid pProyectoID, Guid pDocumentoID)
        {   
            if (ProyectoSeleccionado.Clave != pProyectoID)
            {                
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                bool comunidadPermiteDescargaAInvitados = false;

                bool esIdentidadInvitada = true;
                Guid? perfilID = null;
                Guid? identidadID = null;
                Guid? identidadMygnossID = null;

                if (mControladorBase.UsuarioActual != null)
                {
                    esIdentidadInvitada = mControladorBase.UsuarioActual.EsIdentidadInvitada;

                    if (!esIdentidadInvitada)
                    {
                        perfilID = IdentidadActual.PerfilID;
                        identidadID = IdentidadActual.Clave;
                        identidadMygnossID = IdentidadActual.IdentidadMyGNOSS.Clave;
                    }
                }
                else if (mUsuarioOauth.HasValue)
                {
                    List<Guid> listaUsu = new List<Guid>();
                    listaUsu.Add(mUsuarioOauth.Value);
                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> listaIdent = identCN.ObtenerIdentidadesIDDeusuariosEnProyecto(ProyectoSeleccionado.Clave, listaUsu, true);

                    if (listaIdent.Count > 0)
                    {
                        identidadID = listaIdent[0];
                        esIdentidadInvitada = false;
                        perfilID = identCN.ObtenerPerfilIDDeIdentidadID(identidadID.Value);
                        identidadMygnossID = identCN.ObtenerIdentidadIDDeMyGNOSSPorIdentidad(identidadID.Value);
                    }

                    identCN.Dispose();
                }

                if (ParametrosGeneralesRow != null)
                {
                    comunidadPermiteDescargaAInvitados = esIdentidadInvitada && ParametrosGeneralesRow.PermitirUsuNoLoginDescargDoc;
                }

                bool usuarioTieneAccesoProyecto = !esIdentidadInvitada || ProyectoSeleccionado.EsPublico;

                bool usuarioTieneAccesoRecurso = docCN.TieneUsuarioAccesoADocumentoEnProyecto(ProyectoSeleccionado.Clave, pDocumentoID, perfilID, identidadID, identidadMygnossID, false, !esIdentidadInvitada);
                                
                // Si el usuario no tiene acceso al recurso, se le redirige a la home de la comunidad
                // Si el usuario tiene acceso al recurso, pero no tiene acceso a la comunidad o esta no permite la descarga para el usuario invitado, se le redirige a la home de la comunidad
                if (!usuarioTieneAccesoRecurso || (!comunidadPermiteDescargaAInvitados && !usuarioTieneAccesoProyecto))
                {
                    mLoggingService.AgregarEntrada("PageLoad: comunidadPermiteDescargaAInvitados, usuarioTieneAccesoProyecto y usuarioTieneAccesoRecurso");
                    Response.Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                }
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Redirecciona al usuario a la ficha del recurso, ya que no ha llegado a esta página desde la ficha
        /// </summary>
        private void RedireccionarAFicha(Guid pOrganizacionID, Guid pProyectoID, Documento pDocumento)
        {
            try
            {
                string nombreProy = "";
                string urlPerfil = "";

                //Redirecciono a la ficha
                if (!pProyectoID.Equals(Guid.Empty))
                {
                    mLoggingService.AgregarEntrada("RedireccionarAFicha: ProyectoID = " + pProyectoID);

                    //Es un recurso de una base de recursos de comunidad
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    nombreProy = proyCN.ObtenerNombreCortoProyecto(pProyectoID);
                    proyCN.Dispose();
                }
                else
                {
                    mLoggingService.AgregarEntrada("RedireccionarAFicha: ProyectoID vacio");

                    //Es un recurso de una base de recursos personal
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(pDocumento.CreadorID, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCN.Dispose();

                    Identidad identidad = gestorIdentidades.ListaIdentidades[pDocumento.CreadorID];

                    if (identidad.FilaIdentidad.Tipo.Equals((short)TiposIdentidad.Personal))
                    {
                        urlPerfil = "/" + mControladorBase.UtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + identidad.PerfilUsuario.FilaPerfil.NombreCortoUsu + "/";

                        mLoggingService.AgregarEntrada("RedireccionarAFicha: identidad personal");
                    }
                    else
                    {
                        urlPerfil = "/" + mControladorBase.UrlsSemanticas.ObtenerURLOrganizacionOClase(UtilIdiomas, identidad.OrganizacionID.Value) + "/" + identidad.PerfilUsuario.FilaPerfil.NombreCortoOrg + "/";

                        mLoggingService.AgregarEntrada("RedireccionarAFicha: identidad organiazacion");
                    }
                }

                string url = mControladorBase.UrlsSemanticas.GetURLBaseRecursosFicha(BaseURLIdioma, UtilIdiomas, nombreProy, urlPerfil, pDocumento, false);

                if (mControladorBase.UsuarioActual.EsUsuarioInvitado && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado))
                {
                    mLoggingService.AgregarEntrada("RedireccionarAFicha: esInvitado y (proyecto de acceso privado ó reservado)");

                    //Redirigir A Login para ir a ficha recurso:
                    Response.Redirect(mControladorBase.ObtenerUrlRedireccionLogin(url));
                }
                else
                {
                    mLoggingService.AgregarEntrada("RedireccionarAFicha: no esInvitado ó el no es proyecto de acceso privado ni reservado");

                    Response.Redirect(url);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            mLoggingService.AgregarEntrada("RedireccionarAFicha: no ha entrado por ninguna redireccion y lo lleva a la home");

            Response.Redirect(BaseURLIdioma + "/home");
        }

        /// <summary>
        /// Devuelve una ontología solicitada.
        /// </summary>
        private void DevolverOntologia()
        {
            string nombreOnto = Request.Query["ontologia"];
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Guid documentoID = docCN.ObtenerOntologiaAPartirNombre(Guid.Empty, nombreOnto);
            docCN.Dispose();


            string contentType = "application/octet-stream";
            nombreOnto = nombreOnto.Replace("#", "");

            if (Request.Headers["User-Agent"].ToString() == "Firefox")
            {
                //Le añado el tipo de contenido
                contentType = "application/text/xml";
            }

            if (documentoID != Guid.Empty)
            {
                try
                {
                    FileStream stream = new FileStream($"{Path.Combine(mEnv.WebRootPath, Request.Path, UtilArchivos.ContentOntologias, "Archivos")}/{documentoID.ToString().Substring(0, 3)}/{documentoID}.owl", FileMode.Open); //new FileStream(Server.MapPath(Request.ApplicationPath + "/" + UtilArchivos.ContentOntologias + "/Archivos/") + documentoID.ToString().Substring(0, 3) + "/" + documentoID + ".owl", FileMode.Open);
                    byte[] byteArray = new byte[(int)stream.Length];
                    stream.Read(byteArray, 0, (int)stream.Length);
                    stream.Flush();
                    stream.Close();
                    stream.Dispose();

                    char[] charArray = byteArray.ToString().ToCharArray();

                    //Enviamos el fichero a la página
                    //Response.Write(charArray, 0, charArray.Length);
                    //Limpiamos la página de salida
                    Response.Clear();

                    //Le añado el tipo de contenido
                    Response.ContentType = contentType;

                    //Añadimos la cabecera http al navegador
                    Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + nombreOnto + "\"");//+ HttpUtility.ParseQueryString(nombre));
                                                                                                               //Enviamos todo al explorador
                                                                                                               //Response.Flush();
                                                                                                               //Escribimos el contenido del fichero en la página
                    Response.WriteAsync(Convert.ToBase64String(byteArray));
                }
                catch (Exception)
                {
                    documentoID = Guid.Empty;
                }
            }

            if (documentoID == Guid.Empty)
            {
                Response.WriteAsync("No se encontr&oacute; el documento");
            }
        }

        /// <summary>
        /// devuelve el nombre del documento sin la extensión
        /// </summary>
        /// <param name="pNombreFichero">nombre fichero</param>
        /// <returns>string con nombre fichero</returns>
        private string ExtraerNombreFicheroSinExtension(string pNombreFichero)
        {
            return pNombreFichero.Substring(0, pNombreFichero.LastIndexOf('.'));
        }

        #endregion

    }
}