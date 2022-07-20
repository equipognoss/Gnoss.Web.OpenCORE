using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using OntologiaAClase;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class AdministrarPlantillasOntologicasController : ControllerBaseWeb
    {
        private IMassiveOntologyToClass mMassiveOntologyToClass;

        public AdministrarPlantillasOntologicasController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IMassiveOntologyToClass massiveOntologyToClass, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
            mMassiveOntologyToClass = massiveOntologyToClass;
        }

        /// <summary>
        /// 
        /// </summary>
        private AdministrarPlantillasOntologicasViewModel mPaginaModel = null;
        /// <summary>
        /// DataSet de proyecto con la configuración semántica.
        /// </summary>
        private DataWrapperProyecto mProyectoConfigSemDataWrapperProyecto;
        /// <summary>
        /// 
        /// </summary>
        private GestorDocumental mGestorDocumental;

        /// <summary>
        /// Lista con los grafos simples para autocompletar configurados en el XML.
        /// </summary>
        private List<string> mGrafosSimplesAutocompletarConfig;

        /// <summary>
        /// Diccionario con las propiedades que puede tener la ontología
        /// </summary>
        private Dictionary<string, string> mPropiedadesOntologia;

        /// <summary>
        /// Lista de documentos con imagenes
        /// </summary>
        private Dictionary<Guid, bool> mListaDocumentosConRecursos;


        private XmlDocument doc;
        private Guid proyID;
        private string nombreCortoProy;


        #region Métodos Web

        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            return View(PaginaModel);
        }

        [HttpPost]
        [TypeFilter(typeof(AccesoIntegracionAttribute))]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Guardar(EditOntologyViewModel Ontologia)
        {
            //En algun momento se tendrá que repasar este controlador.
            //LLega a un punto que no se entiende muy bien los pasos.
            //habría que dividir la acción guardar en dos acciones
            //Una que sea editar y otra que sea crear.
            //Los metodos compartidos se pasan a privados.

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

            Guid documentoID = OntologiaID;
            bool editandoPlantilla = true;
            Ontologia.Description = HttpUtility.UrlDecode(Ontologia.Description);
            if (documentoID.Equals(Guid.Empty))
            {
                documentoID = Guid.NewGuid();
                editandoPlantilla = false;
            }

            if (editandoPlantilla)
            {
                Ontologia.Principal = GestorDocumental.ListaDocumentos[documentoID].TipoDocumentacion.Equals(TiposDocumentacion.Ontologia);
            }

            bool AgregadoCSS = false;
            bool AgregadoIMG = false;
            bool AgregadoJS = false;

            string error = "";

            ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool transaccionIniciada = false;

            if (ValidarAgregacionEdicionOnto(Ontologia, !editandoPlantilla))
            {
                try
                {
                    ControladorIdentidades controladorIdentidades = new ControladorIdentidades(IdentidadActual.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

                    string rutaFichero = null;

                    if (editandoPlantilla || (Ontologia.OWL != null &&
                        (Ontologia.XML != null || Ontologia.GenericXML || Ontologia.NoUseXML) &&
                        (Ontologia.CSS != null || Ontologia.GenericCSS) &&
                        (Ontologia.IMG != null || Ontologia.GenericIMG) &&
                        (Ontologia.JS != null || Ontologia.GenericJS)))
                    {
                        rutaFichero = GuardarArchivos(Ontologia, documentoID, ref AgregadoCSS, ref AgregadoIMG, ref AgregadoJS, ref error);

                        if (!string.IsNullOrEmpty(error))
                        {
                            return GnossResultERROR(error);
                        }

                        DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                        if (rutaFichero != null && docCN.ExisteOtraOntologiaEnProyecto(ProyectoSeleccionado.Clave, rutaFichero, documentoID))
                        {
                            docCN.Dispose();
                            return GnossResultERROR(UtilIdiomas.GetText("COMADMIN", "ERRORONTOUSADA", rutaFichero));
                        }

                        docCN.Dispose();
                    }

                    string nombreOnto = Ontologia.Name.Trim();
                    string descripcionOnto = HttpUtility.UrlDecode(Ontologia.Description);

                    if (rutaFichero != null && !editandoPlantilla)
                    {
                        GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
                        controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                        TiposDocumentacion tipoDoc = TiposDocumentacion.Ontologia;

                        if (!Ontologia.Principal)
                        {
                            tipoDoc = TiposDocumentacion.OntologiaSecundaria;

                            if (!Ontologia.NoUseXML)//Hemos subido XML a la entidad secundaria, generico o no.
                            {
                                AD.EntityModel.Models.ProyectoDS.ProyectoConfigExtraSem filaConfigSem = new AD.EntityModel.Models.ProyectoDS.ProyectoConfigExtraSem();
                                filaConfigSem.ProyectoID = ProyectoSeleccionado.Clave;
                                filaConfigSem.UrlOntologia = rutaFichero;
                                filaConfigSem.SourceTesSem = documentoID.ToString();
                                filaConfigSem.Tipo = (short)TipoConfigExtraSemantica.EntidadSecundaria;
                                filaConfigSem.Nombre = nombreOnto;
                                filaConfigSem.Editable = true;
                                ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Add(filaConfigSem);
                                if (!mEntityContext.ProyectoConfigExtraSem.Any(proy => proy.ProyectoID.Equals(filaConfigSem.ProyectoID) && proy.UrlOntologia.Equals(filaConfigSem.UrlOntologia) && proy.SourceTesSem.Equals(filaConfigSem.SourceTesSem)))
                                {
                                    mEntityContext.ProyectoConfigExtraSem.Add(filaConfigSem);
                                }

                            }
                        }

                        if (mGrafosSimplesAutocompletarConfig != null)
                        {
                            foreach (string grafoSimple in mGrafosSimplesAutocompletarConfig)
                            {
                                AD.EntityModel.Models.ProyectoDS.ProyectoConfigExtraSem filaConfigSem = new AD.EntityModel.Models.ProyectoDS.ProyectoConfigExtraSem();
                                filaConfigSem.ProyectoID = ProyectoSeleccionado.Clave;
                                filaConfigSem.UrlOntologia = rutaFichero;
                                filaConfigSem.SourceTesSem = grafoSimple;
                                filaConfigSem.Tipo = (short)TipoConfigExtraSemantica.GrafoSimple;
                                filaConfigSem.Nombre = nombreOnto;
                                filaConfigSem.Editable = true;
                                ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Add(filaConfigSem);
                                if (!mEntityContext.ProyectoConfigExtraSem.Any(proy => proy.ProyectoID.Equals(filaConfigSem.ProyectoID) && proy.UrlOntologia.Equals(filaConfigSem.UrlOntologia) && proy.SourceTesSem.Equals(filaConfigSem.SourceTesSem)))
                                {
                                    mEntityContext.ProyectoConfigExtraSem.Add(filaConfigSem);
                                }
                            }
                        }

                        Documento doc = GestorDocumental.AgregarDocumento(rutaFichero, nombreOnto, descripcionOnto, null, tipoDoc, TipoEntidadVinculadaDocumento.Web, Guid.Empty, true, false, false, null, false, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.IdentidadID);

                        //Cambiar identificador del documento
                        if (GestorDocumental.ListaDocumentos.ContainsKey(doc.Clave))
                        {
                            GestorDocumental.ListaDocumentos.Remove(doc.Clave);
                            GestorDocumental.ListaDocumentos.Add(documentoID, doc);
                        }
                        else
                        {
                            GestorDocumental.ListaDocumentos.Add(documentoID, doc);
                        }
                        doc.FilaDocumento.DocumentoID = documentoID;

                        //Agrego la comunidad a la que pertenece el documento:
                        doc.FilaDocumento.ProyectoID = mControladorBase.UsuarioActual.ProyectoID;

                        //Pongo la ultima versión del documento a false para que no aparezca en las listas:
                        doc.FilaDocumento.UltimaVersion = false;

                        GestorDocumental.GestorIdentidades = IdentidadActual.GestorIdentidades;
                        controladorIdentidades.CompletarCargaIdentidad(mControladorBase.UsuarioActual.IdentidadID);

                        GestorDocumental.VincularDocumentoABaseRecursos(doc, GestorDocumental.BaseRecursosIDActual, TipoPublicacion.Compartido, mControladorBase.UsuarioActual.IdentidadID, false);

                        if (AgregadoCSS || AgregadoIMG || AgregadoJS)
                        {
                            if (AgregadoIMG)
                            {
                                doc.FilaDocumento.NombreCategoriaDoc = "240," + UtilArchivos.ContentOntologias + "/Archivos/" + documentoID.ToString().Substring(0, 3) + "/" + documentoID + ".jpg";
                            }
                            doc.FilaDocumento.VersionFotoDocumento = 1;
                        }

                        if (mPropiedadesOntologia != null)
                        {
                            foreach (string propiedad in mPropiedadesOntologia.Keys)
                            {
                                doc.FilaDocumento.NombreElementoVinculado += propiedad + "=" + mPropiedadesOntologia[propiedad] + "|||";
                            }
                        }
                        Ontologia.Protected = doc.FilaDocumento.Protegido;
                        Ontologia.OntologyProperties = doc.FilaDocumento.NombreElementoVinculado;

                        try
                        {
                            mEntityContext.NoConfirmarTransacciones = true;
                            transaccionIniciada = proyAD.IniciarTransaccion(true);
                            GuardarCambios();

                            if (iniciado)
                            {
                                HttpResponseMessage resultado = AdministrarIntegracionContinua(Ontologia, documentoID);

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
                            GuardarLogError(ex.ToString());
                            return GnossResultERROR(ex.Message);
                        }
                        return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLAdministrarPlantillasComunidad(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto));
                    }
                    else if (editandoPlantilla)
                    {
                        GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Titulo = nombreOnto;
                        GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Descripcion = descripcionOnto;

                        //if (rutaFichero != null)
                        //{
                        //    GestorDocumental.ListaDocumentos[DocumentoID].FilaDocumento.Enlace = rutaFichero;
                        //}

                        string rutaActualizarDocsOnto = null;

                        if (Ontologia.EditIMG)
                        {
                            if (AgregadoIMG)
                            {
                                GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc = "240," + UtilArchivos.ContentOntologias + "/Archivos/" + documentoID.ToString().Substring(0, 3) + "/" + documentoID + ".jpg"; ;
                                rutaActualizarDocsOnto = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc;

                                if (!GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento.HasValue)
                                {
                                    GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento = 1;
                                }
                                else
                                {
                                    GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento = Math.Abs(GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento.Value) + 1;
                                }
                            }
                            else if (GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc != null)
                            {
                                rutaActualizarDocsOnto = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc;
                                GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreCategoriaDoc = null;
                            }
                        }

                        if ((AgregadoCSS || AgregadoJS) && !AgregadoIMG)
                        {
                            if (!GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento.HasValue)
                            {
                                GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento = 1;
                            }
                            else
                            {
                                GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento = Math.Abs(GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.VersionFotoDocumento.Value) + 1;
                            }
                        }

                        if (Ontologia.Principal || (!Ontologia.Principal && Ontologia.EditXML))
                        {
                            //("UrlOntologia='" + GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace + "' AND Tipo=" + (short)TipoConfigExtraSemantica.EntidadSecundaria)
                            List<ProyectoConfigExtraSem> listaProyectoConfigExtraSemBorrar2 = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace) && proy.Tipo.Equals((short)TipoConfigExtraSemantica.EntidadSecundaria)).ToList();
                            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoConfigExtraSem fila in listaProyectoConfigExtraSemBorrar2)
                            {
                                mEntityContext.EliminarElemento(fila);

                                ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Remove(fila);
                            }
                        }

                        if (!Ontologia.Principal && Ontologia.EditXML) //Se ha pulsado el botón para interacturar con el archivo de configuración.
                        {
                            if (!Ontologia.NoUseXML)//Hemos subido XML a la entidad secundaria, generico o no.
                            {
                                ProyectoConfigExtraSem filaConfigSem = new ProyectoConfigExtraSem();
                                filaConfigSem.ProyectoID = ProyectoSeleccionado.Clave;
                                filaConfigSem.UrlOntologia = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace;
                                filaConfigSem.SourceTesSem = documentoID.ToString();
                                filaConfigSem.Tipo = (short)TipoConfigExtraSemantica.EntidadSecundaria;
                                filaConfigSem.Nombre = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Titulo;
                                filaConfigSem.Editable = true;
                                ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Add(filaConfigSem);
                                mEntityContext.ProyectoConfigExtraSem.Add(filaConfigSem);
                            }
                        }
                        List<ProyectoConfigExtraSem> listaProyectoConfigExtraSemBorrar = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace) && proy.Tipo.Equals((short)TipoConfigExtraSemantica.GrafoSimple)).ToList();
                        foreach (ProyectoConfigExtraSem fila in listaProyectoConfigExtraSemBorrar)
                        {
                            mEntityContext.EliminarElemento(fila);
                            ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Remove(fila);
                        }

                        if (mGrafosSimplesAutocompletarConfig != null)
                        {
                            foreach (string grafoSimple in mGrafosSimplesAutocompletarConfig)
                            {
                                ProyectoConfigExtraSem filaConfigSem = new ProyectoConfigExtraSem();
                                filaConfigSem.ProyectoID = ProyectoSeleccionado.Clave;
                                filaConfigSem.UrlOntologia = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Enlace;
                                filaConfigSem.SourceTesSem = grafoSimple;
                                filaConfigSem.Tipo = (short)TipoConfigExtraSemantica.GrafoSimple;
                                filaConfigSem.Nombre = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Titulo;
                                filaConfigSem.Editable = true;
                                ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Add(filaConfigSem);
                                if (!(mEntityContext.ProyectoConfigExtraSem.Any(proy => proy.ProyectoID.Equals(filaConfigSem.ProyectoID) && proy.UrlOntologia.Equals(filaConfigSem.UrlOntologia) && proy.SourceTesSem.Equals(filaConfigSem.SourceTesSem))))
                                {
                                    mEntityContext.ProyectoConfigExtraSem.Add(filaConfigSem);
                                }
                            }
                        }

                        if (mPropiedadesOntologia != null)
                        {
                            List<string> listaPropsOnto = null;
                            Dictionary<string, string> dicPropsOnto = new Dictionary<string, string>();
                            string propsOnto = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreElementoVinculado;

                            if (!string.IsNullOrEmpty(propsOnto))
                            {
                                listaPropsOnto = new List<string>(propsOnto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries));
                                foreach (string prop in listaPropsOnto)
                                {
                                    if (prop.Contains("="))
                                    {
                                        string[] valores = prop.Split(new char[] { '=' });
                                        dicPropsOnto.Add(valores[0], valores[1]);
                                    }
                                }

                                //quitamos las propiedades de la Ontología que ya no están configuradas
                                //mPropiedadesOntologia contiene las configuradas en el Xml y dicPropsOnto son las obtenidas en BD
                                if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.urlservicio.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.urlservicio.ToString()))
                                {
                                    dicPropsOnto.Remove(PropiedadesOntologia.urlservicio.ToString());
                                }
                                if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.urlserviciocomplementario.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.urlserviciocomplementario.ToString()))
                                {
                                    dicPropsOnto.Remove(PropiedadesOntologia.urlserviciocomplementario.ToString());
                                }
                                if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString()))
                                {
                                    dicPropsOnto.Remove(PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString());
                                }
                                if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.urlservicioElim.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.urlservicioElim.ToString()))
                                {
                                    dicPropsOnto.Remove(PropiedadesOntologia.urlservicioElim.ToString());
                                }
                                if (!mPropiedadesOntologia.ContainsKey(PropiedadesOntologia.enviarRdfAntiguo.ToString()) && dicPropsOnto.ContainsKey(PropiedadesOntologia.enviarRdfAntiguo.ToString()))
                                {
                                    dicPropsOnto.Remove(PropiedadesOntologia.enviarRdfAntiguo.ToString());
                                }
                            }

                            //guardo las propiedades configuradas en el Xml
                            foreach (string propiedad in mPropiedadesOntologia.Keys)
                            {
                                if (!dicPropsOnto.ContainsKey(propiedad))
                                {
                                    dicPropsOnto.Add(propiedad, mPropiedadesOntologia[propiedad]);
                                }
                                else
                                {
                                    dicPropsOnto[propiedad] = mPropiedadesOntologia[propiedad];
                                }
                            }

                            GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreElementoVinculado = "";
                            foreach (string propiedadOnto in dicPropsOnto.Keys)
                            {
                                GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreElementoVinculado += propiedadOnto + "=" + dicPropsOnto[propiedadOnto] + "|||";
                            }
                        }

                        Ontologia.Protected = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.Protegido;
                        Ontologia.OntologyProperties = GestorDocumental.ListaDocumentos[documentoID].FilaDocumento.NombreElementoVinculado;

                        try
                        {
                            mEntityContext.NoConfirmarTransacciones = true;
                            transaccionIniciada = proyAD.IniciarTransaccion(true);

                            GuardarCambios();

                            if (iniciado)
                            {
                                HttpResponseMessage resultado = AdministrarIntegracionContinua(Ontologia, documentoID);
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
                            GuardarLogError(ex.ToString());
                            return GnossResultERROR(ex.Message);
                        }

                        if (rutaActualizarDocsOnto != null)
                        {
                            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            docCN.ActualizarFotoDocumentosDeOntologia(documentoID, rutaActualizarDocsOnto, !AgregadoIMG);
                            docCN.Dispose();
                        }

                        DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        docCL.GuardarOntologia(documentoID, null);
                        docCL.GuardarIDXmlOntologia(documentoID, Guid.NewGuid());
                        docCL.Dispose();

                        mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);

                        return GnossResultUrl(mControladorBase.UrlsSemanticas.GetURLAdministrarPlantillasComunidad(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto));
                    }
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex.ToString());
                    return GnossResultERROR(UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC"));
                }
            }

            return GnossResultOK();
        }

        private HttpResponseMessage AdministrarIntegracionContinua(EditOntologyViewModel Ontologia, Guid documentoID)
        {
            HttpResponseMessage resultado = new HttpResponseMessage(HttpStatusCode.BadRequest);
            if (HayIntegracionContinua)
            {
                //Para la IC 
                Documento doc = GestorDocumental.ListaDocumentos[documentoID];
                string nombreOnto = doc.Enlace.Replace(".owl", "");
                CallFileService fileService = new CallFileService(mConfigService);
                if (Ontologia.FicheroOWL == null)
                {

                    Ontologia.FicheroOWL = fileService.ObtenerOntologia(documentoID, true);

                }
                if (Ontologia.FicheroCSS == null)
                {
                    Ontologia.FicheroCSS = fileService.DescargarCSSOntologia(documentoID, ".css", true);
                }
                if (Ontologia.FicheroIMG == null)
                {
                    ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService);
                    servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");
                    byte[] buffer = servicioImagenes.ObtenerImagenDeDirectorioOntologia($"Archivos{Path.DirectorySeparatorChar}" + documentoID.ToString().Substring(0, 3), documentoID + "_240", ".jpg");
                    if (buffer != null && buffer.Any())
                    {
                        Ontologia.FicheroIMG = Convert.ToBase64String(buffer);
                    }
                }
                if (Ontologia.FicheroJS == null)
                {

                    Ontologia.FicheroJS = Ontologia.FicheroCSS = fileService.DescargarCSSOntologia(documentoID, ".js", true);

                }
                if (Ontologia.FicheroXML == null)
                {
                    Ontologia.FicheroXML = fileService.ObtenerXmlOntologia(documentoID, true);

                }
                //Esto es para notificar las ontologias en la IC correctamente.
                Ontologia.NameOWL = nombreOnto;
                return InformarCambioAdministracion("Ontologias", JsonConvert.SerializeObject(Ontologia, Newtonsoft.Json.Formatting.Indented));
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }


        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult DownloadClasses()
        {
            Dictionary<string, string> dicPref = new Dictionary<string, string>();
            Dictionary<string, KeyValuePair<Ontologia, byte[]>> diccionarioOntologias = new Dictionary<string, KeyValuePair<Ontologia, byte[]>>();
            List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();
            foreach (var ontologiaPrimaria in PaginaModel.Templates)
            {
                if (!ontologiaPrimaria.OntologyName.Equals("dbpedia"))
                {
                    byte[] bytesOntologias = ObtenerOntologia(ontologiaPrimaria.OntologyID);
                    byte[] bytesXmlOntologia = ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);
                    Ontologia ontologia = new Ontologia(bytesOntologias);
                    ontologia.LeerOntologia();

                    foreach (string key in ontologia.NamespacesDefinidos.Keys)
                    {
                        if (!dicPref.ContainsKey(key))
                        {
                            dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
                        }
                    }

                    diccionarioOntologias.Add(ontologiaPrimaria.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
                }
            }
            foreach (var secondaryOntology in PaginaModel.SecondaryTemplates)
            {
                byte[] bytesOntologias = ObtenerOntologia(secondaryOntology.OntologyID);
                byte[] bytesXmlOntologia = ObtenerXmlOntologia(secondaryOntology.OntologyID);
                Ontologia ontologia = new Ontologia(bytesOntologias);
                ontologia.LeerOntologia();
                foreach (string key in ontologia.NamespacesDefinidos.Keys)
                {
                    if (!dicPref.ContainsKey(key))
                    {
                        dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
                    }
                }

                diccionarioOntologias.Add(secondaryOntology.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
            }

            try
            {
                string directorio = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(directorio);
                List<string> nombresOntologias = PaginaModel.Templates.Select(p => p.OntologyName).Union(PaginaModel.SecondaryTemplates.Select(p => p.OntologyName)).ToList();
                 GenerarClaseYVista claseYvista = new GenerarClaseYVista(directorio, ProyectoSeleccionado.NombreCorto, ProyectoSeleccionado.Clave, nombresOntologias, dicPref, diccionarioOntologias, false, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mMassiveOntologyToClass);
                claseYvista.CrearObjetos(diccionarioOntologias);
                claseYvista.CrearRelaciones();
                
                foreach (var ontologiaPrimaria in PaginaModel.Templates)
                {
                    try
                    {
                        byte[] bytesOntologia = ObtenerOntologia(ontologiaPrimaria.OntologyID);
                        byte[] bytesXmlOntologia = ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);

                        Ontologia ontologia = new Ontologia(bytesOntologia);


                        OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaPrimaria.OntologyName, ontologia, bytesXmlOntologia, true, listaIdiomas, directorio);

                        claseYvista.CrearClases(contenedorOntologia);
                        claseYvista.CrearVistas(contenedorOntologia, ProyectoSeleccionado.NombreCorto);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, $"error en la ontología {ontologiaPrimaria.OntologyName}");
                        throw new Exception(ex.Message);
                    }
                }
                foreach (var ontologiaSecundaria in PaginaModel.SecondaryTemplates)
                {
                    byte[] bytesOntologia = ObtenerOntologia(ontologiaSecundaria.OntologyID);
                    byte[] bytesXmlOntologia = null;
                    if (ontologiaSecundaria.HasXmlFile)
                    {
                        bytesXmlOntologia = ObtenerXmlOntologia(ontologiaSecundaria.OntologyID);
                    }

                    Ontologia ontologia = new Ontologia(bytesOntologia);

                    OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaSecundaria.OntologyName, ontologia, bytesXmlOntologia, false, listaIdiomas, directorio);
                    claseYvista.CrearClases(contenedorOntologia);
                }
                //claseYvista.ObtenerDLL(ProyectoSeleccionado.NombreCorto);
                claseYvista.GenerarPaqueteCSPROJ(ProyectoSeleccionado.NombreCorto);
                //Generar clases 
                //Comprimirlo

                DirectoryInfo directoryPrincipal = new DirectoryInfo(directorio);
                DirectoryInfo[] directories = directoryPrincipal.GetDirectories();

                string nombrefichero = "";
                foreach (DirectoryInfo dir in directories)
                {
                    if (dir.Name.Contains("ClasesYVistas_"))
                    {
                        nombrefichero = dir.Name;
                    }
                }
                string pZipPath = Path.Combine(directorio, "comprimido.zip");

                string folderPath = Path.Combine(directorio, nombrefichero);
                ZipFile.CreateFromDirectory(folderPath, pZipPath);

                byte[] bytes = System.IO.File.ReadAllBytes(pZipPath);
                Thread.Sleep(1000);

                try
                {
                    Directory.Delete(directorio, true);
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        Directory.Delete(directorio, true);
                    }
                    catch
                    {
                        GuardarLogError("Fallo al intentar borrra el fichero temporar de la carpeta: " + directorio);
                    }
                }

                return File(bytes, "application/zip", $"{ProyectoSeleccionado.NombreCorto}_ClassesAndViews.zip");
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                return GnossResultERROR("Error al generar las clases");
            }
        }

        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult DownloadClassesJava()
        {
            Dictionary<string, string> dicPref = new Dictionary<string, string>();
            Dictionary<string, KeyValuePair<Ontologia, byte[]>> diccionarioOntologias = new Dictionary<string, KeyValuePair<Ontologia, byte[]>>();
            List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();
            foreach (var ontologiaPrimaria in PaginaModel.Templates)
            {
                if (!ontologiaPrimaria.OntologyName.Equals("dbpedia"))
                {
                    byte[] bytesOntologias = ObtenerOntologia(ontologiaPrimaria.OntologyID);
                    byte[] bytesXmlOntologia = ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);
                    Ontologia ontologia = new Ontologia(bytesOntologias);
                    ontologia.LeerOntologia();

                    foreach (string key in ontologia.NamespacesDefinidos.Keys)
                    {
                        if (!dicPref.ContainsKey(key))
                        {
                            dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
                        }
                    }

                    diccionarioOntologias.Add(ontologiaPrimaria.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
                }
            }
            foreach (var secondaryOntology in PaginaModel.SecondaryTemplates)
            {
                byte[] bytesOntologias = ObtenerOntologia(secondaryOntology.OntologyID);
                byte[] bytesXmlOntologia = ObtenerXmlOntologia(secondaryOntology.OntologyID);
                Ontologia ontologia = new Ontologia(bytesOntologias);
                ontologia.LeerOntologia();
                foreach(string key in ontologia.NamespacesDefinidos.Keys)
                {
                    if (!dicPref.ContainsKey(key))
                    {
                        dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
                    }
                }

                diccionarioOntologias.Add(secondaryOntology.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
            }

            try
            {
                string directorio = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "ClasesYVistas");
                Directory.CreateDirectory(directorio);
                List<string> nombresOntologias = PaginaModel.Templates.Select(p => p.OntologyName).Union(PaginaModel.SecondaryTemplates.Select(p => p.OntologyName)).ToList();

                GenerarClaseYVista claseYvista = new GenerarClaseYVista(directorio, ProyectoSeleccionado.NombreCorto, ProyectoSeleccionado.Clave, nombresOntologias, dicPref, diccionarioOntologias, true, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mMassiveOntologyToClass);
                claseYvista.CrearObjetos(diccionarioOntologias);
                claseYvista.CrearRelaciones();

                string ruta = Path.Combine(directorio, "src", "main", "java");
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                foreach (var ontologiaPrimaria in PaginaModel.Templates)
                {
                    try
                    {
                        byte[] bytesOntologia = ObtenerOntologia(ontologiaPrimaria.OntologyID);
                        byte[] bytesXmlOntologia = ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);

                        Ontologia ontologia = new Ontologia(bytesOntologia);
                        OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaPrimaria.OntologyName, ontologia, bytesXmlOntologia, true, listaIdiomas, ruta);

                        claseYvista.CrearClasesJava(contenedorOntologia);
                        claseYvista.CrearVistas(contenedorOntologia, ProyectoSeleccionado.NombreCorto);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, $"error en la ontología {ontologiaPrimaria.OntologyName}");
                        throw new Exception(ex.Message);
                    }
                }
                foreach (var ontologiaSecundaria in PaginaModel.SecondaryTemplates)
                {
                    byte[] bytesOntologia = ObtenerOntologia(ontologiaSecundaria.OntologyID);
                    byte[] bytesXmlOntologia = null;
                    if (ontologiaSecundaria.HasXmlFile)
                    {
                        bytesXmlOntologia = ObtenerXmlOntologia(ontologiaSecundaria.OntologyID);
                    }

                    Ontologia ontologia = new Ontologia(bytesOntologia);
                    OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaSecundaria.OntologyName, ontologia, bytesXmlOntologia, false, listaIdiomas, directorio);
                    claseYvista.CrearClasesJava(contenedorOntologia);
                }

                CrearPomXml(directorio, ProyectoSeleccionado.NombreCorto);

                DirectoryInfo directoryPrincipal = new DirectoryInfo(directorio);
                DirectoryInfo[] directories = directoryPrincipal.GetDirectories();

                string pZipPath = Path.Combine(directorio.Replace("ClasesYVistas", ""), "comprimido.zip");
                ZipFile.CreateFromDirectory(directorio, pZipPath);

                byte[] bytes = System.IO.File.ReadAllBytes(pZipPath);
                Thread.Sleep(1000);

                try
                {
                    Directory.Delete(directorio, true);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        Directory.Delete(directorio, true);
                    }
                    catch
                    {
                        GuardarLogError("Fallo al intentar borrra el fichero temporar de la carpeta: " + directorio);
                    }
                }

                //TODO Jorge: comprobar mime type zip
                return File(bytes, "application/zip", $"{ProyectoSeleccionado.NombreCorto}_ClassesAndViews.zip");
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                return GnossResultERROR("Error al generar las clases");
            }
        }

        private void CrearPomXml(string pDirectorio, string pNombreCortoProyecto)
        {
            string nombreFichero = Path.Combine(pDirectorio, "pom.xml");
            if (!System.IO.File.Exists(nombreFichero))
            {
                System.IO.File.WriteAllText(nombreFichero, EscribirPomXml(pNombreCortoProyecto));
            }
        }

        private string EscribirPomXml(string pNombreCortoProyecto)
        {
            StringBuilder clase = new StringBuilder();
            string versionApiWrapperJava = "0.0.2";

            clase.AppendLine("<project xmlns=\"http://maven.apache.org/POM/4.0.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://maven.apache.org/POM/4.0.0 https://maven.apache.org/xsd/maven-4.0.0.xsd\">");
            clase.AppendLine($"\t<modelVersion>4.0.0</modelVersion>  <!-- Por defecto mantener así -->");
            clase.AppendLine($"\t<groupId>com.arquitecturaJavaGnoss</groupId>  <!-- Introducir grupo al que pertenece el proyecto maven. Lo has indicado al crear el proyecto -->");
            clase.AppendLine($"\t<artifactId>Clases{pNombreCortoProyecto}</artifactId>  <!-- Introducir nombre que le has dado al proyecto. Lo has indicado al crear el proyecto -->");
            clase.AppendLine($"\t<version>1</version>  <!-- Versión inicial -->");
            clase.AppendLine();
            clase.AppendLine($"\t<properties>");
            clase.AppendLine();
            clase.AppendLine($"\t<project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>");
            clase.AppendLine($"\t\t<maven.compiler.source>1.8</maven.compiler.source>");
            clase.AppendLine($"\t\t<maven.compiler.target>1.8</maven.compiler.target>");
            clase.AppendLine($"\t</properties>");
            clase.AppendLine();
            clase.AppendLine($"\t<dependencies>");
            clase.AppendLine($"\t\t<dependency>");
            clase.AppendLine($"\t\t\t<groupId>io.github.equipognoss</groupId>");
            clase.AppendLine($"\t\t\t<artifactId>Gnoss-ApiWrapper-Java</artifactId>");
            clase.AppendLine($"\t\t\t<version>{versionApiWrapperJava}</version>");
            clase.AppendLine($"\t\t</dependency>");
            clase.AppendLine("\t\t<dependency>");
            clase.AppendLine("\t\t\t<groupId>com.microsoft.azure</groupId>");
            clase.AppendLine("\t\t\t<artifactId>applicationinsights-web-auto</artifactId>");
            clase.AppendLine("\t\t\t<!-- or applicationinsights-web for manual web filter registration -->");
            clase.AppendLine("\t\t\t<!-- or applicationinsights-core for bare API -->");
            clase.AppendLine("\t\t\t<version>2.6.2</version>");
            clase.AppendLine("\t\t</dependency>");
            clase.AppendLine($"\t</dependencies>");
            clase.AppendLine("</project>");

            return clase.ToString();
        }

        private byte[] ObtenerXmlOntologia(Guid pOntologiaID)
        {
            CallFileService fileService = new CallFileService(mConfigService);

            byte[] bytesArchivo = fileService.ObtenerXmlOntologiaBytes(pOntologiaID);

            return bytesArchivo;
        }

        [HttpGet]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult DownloadVersion(Guid? ontologyID, string copyName, string ontologyName)
        {
            if (!string.IsNullOrEmpty(copyName) && ontologyID.HasValue)
            {
                try
                {
                    return DescargarArchivo(ontologyID.Value, copyName, ontologyName);
                }
                catch (Exception ex)
                {
                    GuardarLogError(ex);
                    return Content("ERROR: Ha habido un error al obtener el historial");
                }
            }

            return Content("Parámetros de descarga incorrectos");
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Eliminar(Guid ontoID)
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

            if (!ListaDocumentosConRecursos.ContainsKey(ontoID))
            {
                try
                {
                    mEntityContext.NoConfirmarTransacciones = true;
                    transaccionIniciada = proyAD.IniciarTransaccion(true);

                    List<ProyectoConfigExtraSem> filasConfig = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(GestorDocumental.ListaDocumentos[ontoID].Enlace)).ToList();

                    if (filasConfig.Count > 0)
                    {
                        ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Remove(filasConfig.First());
                        mEntityContext.EliminarElemento(filasConfig.First());
                    }

                    GestorDocumental.ListaDocumentos[ontoID].FilaDocumentoWebVinBR.Eliminado = true;
                    if (GestorDocumental.ListaDocumentos[ontoID].ProyectoID.Equals(ProyectoSeleccionado.Clave))
                    {
                        GestorDocumental.ListaDocumentos[ontoID].FilaDocumento.Eliminado = true;
                    }
                    GuardarCambios();

                    EditOntologyViewModel ontologyBorrar = new EditOntologyViewModel();
                    using (DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                    using (GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentoPorID(ontoID), mLoggingService, mEntityContext))
                    {
                        Documento doc = gestorDoc.ListaDocumentos[ontoID];
                        ontologyBorrar.Name = doc.Nombre;
                        ontologyBorrar.Deleted = true;
                    }
                    if (iniciado)
                    {
                        HttpResponseMessage resultado = InformarCambioAdministracion("Ontologias", JsonConvert.SerializeObject(ontologyBorrar, Newtonsoft.Json.Formatting.Indented));

                        if (!resultado.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            throw new Exception("Contacte con el administrador del Proyecto, no es posible atender la petición.");
                        }
                    }
                    if (transaccionIniciada)
                    {
                        mEntityContext.TerminarTransaccionesPendientes(true);
                    }

                    return GnossResultOK();
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
                return GnossResultERROR("No se puede eliminar la ontología porque tiene recursos asociados");
            }

        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Protect(Guid ontoID, bool Protect)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentoPorID(ontoID), mLoggingService, mEntityContext);
            Documento doc = gestorDoc.ListaDocumentos[ontoID];

            doc.FilaDocumento.Protegido = Protect;

            docCN.ActualizarDocumentacion();
            docCN.Dispose();


            return GnossResultOK();
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult AllowMasiveUpload(Guid ontoID, bool AllowMasiveUpload)
        {

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentoPorID(ontoID), mLoggingService, mEntityContext);

            Documento doc = gestorDoc.ListaDocumentos[ontoID];

            Dictionary<string, string> propiedades = UtilCadenas.ObtenerPropiedadesDeTexto(doc.NombreEntidadVinculada);

            if (propiedades.ContainsKey(PropiedadesOntologia.cargasmultiples.ToString()))
            {
                propiedades.Remove(PropiedadesOntologia.cargasmultiples.ToString());
            }

            if (AllowMasiveUpload)
            {
                propiedades.Add(PropiedadesOntologia.cargasmultiples.ToString(), "true");
            }

            doc.NombreEntidadVinculada = UtilCadenas.ObtenerTextoDePropiedades(propiedades);

            docCN.ActualizarDocumentacion();
            docCN.Dispose();


            return GnossResultOK();
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { TipoPaginaAdministracion.Semantica, "AdministracionSemanticaPermitido" })]
        public ActionResult Historial(Guid ontoID)
        {
            OntologicalTemplatesAdministrationViewModel viewModel = new OntologicalTemplatesAdministrationViewModel() { OntologyID = ontoID };

            try
            {
                List<string> listadoCopias = CargarHistorial(ontoID);

                if (listadoCopias == null || listadoCopias.Count == 0)
                {
                    return GnossResultERROR("No hay versiones de esta ontologia");
                }

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                viewModel.OntologyName = docCN.ObtenerEnlaceDocumentoPorDocumentoID(ontoID);
                if (viewModel.OntologyName.Contains("."))
                {
                    viewModel.OntologyName = viewModel.OntologyName.Substring(0, viewModel.OntologyName.LastIndexOf("."));
                }
                docCN.Dispose();

                viewModel.VersionList = listadoCopias;
            }
            catch (Exception ex)
            {
                GuardarLogError(ex);
                return GnossResultERROR("Ha habido un error al obtener el historial");
            }

            return PartialView("_Historial", viewModel);
        }

        #endregion

        #region Métodos

        private ActionResult DescargarArchivo(Guid pOntologiaID, string pVersion, string pOntologyName)
        {
            if (!string.IsNullOrEmpty(pVersion))
            {
                byte[] bytesArchivo = ObtenerOntologia(pOntologiaID, pVersion);

                return File(bytesArchivo, "application/text", pVersion.Replace(pOntologiaID.ToString(), pOntologyName));
            }

            return Content("No se ha encontrado el archivo");
        }

        private byte[] ObtenerOntologia(Guid pOntologiaID, string pVersion = null)
        {
            CallFileService fileService = new CallFileService(mConfigService);
            byte[] bytesArchivo = null;
            if (string.IsNullOrEmpty(pVersion))
            {
                bytesArchivo = fileService.ObtenerOntologiaBytes(pOntologiaID);
            }
            else
            {
                bytesArchivo = fileService.DescargarVersionBytes(pOntologiaID, pVersion);
            }
            return bytesArchivo;
        }

        /// <summary>
        /// Valida si los datos de documento son correctos o no.
        /// </summary>
        /// <param name="Ontologia"></param>
        /// <param name="pComprobarFileUpdates">Indica si se debe comprobar o no los fileUpdates</param>
        /// <returns>True si los datos son correctos, false en caso contario.</returns>
        private bool ValidarAgregacionEdicionOnto(EditOntologyViewModel Ontologia, bool pComprobarFileUpdates)
        {
            if (string.IsNullOrEmpty(Ontologia.Name))
            {
                return false;
            }
            if (string.IsNullOrEmpty(Ontologia.Description))
            {
                return false;
            }

            if (!pComprobarFileUpdates)
            {
                return true;
            }

            if (Ontologia.OWL == null)
            {
                return false;
            }
            if (Ontologia.XML == null && !Ontologia.GenericXML)
            {
                if (Ontologia.Principal || !Ontologia.NoUseXML)
                {
                    return false;
                }
            }
            if (Ontologia.CSS == null && !Ontologia.GenericCSS)
            {
                return false;
            }
            if (Ontologia.IMG == null && !Ontologia.GenericIMG)
            {
                return false;
            }
            if (Ontologia.JS == null && !Ontologia.GenericJS)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Agrega el archivo de plantilla y el de su estilo a Gnoss.
        /// </summary>
        /// <returns>El nombre del archivo ontológico reemplazador</returns>
        protected string GuardarArchivos(EditOntologyViewModel Ontologia, Guid DocumentoID, ref bool AgregadoCSS, ref bool AgregadoIMG, ref bool AgregadoJS, ref string Error)
        {
            Stopwatch sw = null;
            try
            {
                //GestionDocumental gd = null;
                FileInfo archivoInfo1 = null;

                if (Ontologia.OWL != null)
                {
                    archivoInfo1 = new FileInfo(Ontologia.OWL.FileName);
                }

                FileInfo archivoInfo2 = null;
                string extensionArchivo2 = null;

                if (!Ontologia.GenericCSS)
                {
                    if (Ontologia.CSS != null)
                    {
                        archivoInfo2 = new FileInfo(Ontologia.CSS.FileName);
                    }
                }
                else if (Ontologia.EditCSS)
                {
                    extensionArchivo2 = ".css";
                }

                FileInfo archivoInfo3 = null;
                byte[] buffer3 = null;
                string extensionArchivo3 = null;
                bool generarXMLDef = false;

                if (!Ontologia.GenericXML && !Ontologia.NoUseXML)
                {
                    if (Ontologia.XML != null)
                    {
                        archivoInfo3 = new FileInfo(Ontologia.XML.FileName);
                    }
                }
                else if (Ontologia.EditXML && Ontologia.GenericXML && !Ontologia.NoUseXML)
                {
                    generarXMLDef = true;
                    extensionArchivo3 = ".xml";
                }

                FileInfo archivoInfo4 = null;
                byte[] buffer4 = null;
                string extensionArchivo4 = null;

                if (!Ontologia.GenericIMG)
                {
                    if (Ontologia.IMG != null)
                    {
                        archivoInfo4 = new FileInfo(Ontologia.IMG.FileName);
                    }
                }
                else if (Ontologia.EditIMG)
                {
                    extensionArchivo4 = ".jpg";
                }

                FileInfo archivoInfo5 = null;
                string extensionArchivo5 = null;

                if (!Ontologia.GenericJS)
                {
                    if (Ontologia.JS != null)
                    {
                        archivoInfo5 = new FileInfo(Ontologia.JS.FileName);
                    }
                }
                else if (Ontologia.EditJS)
                {
                    extensionArchivo5 = ".js";
                }

                string extensionArchivo1 = null;

                if (archivoInfo1 != null)
                {
                    extensionArchivo1 = Path.GetExtension(archivoInfo1.Name).ToLower();
                }

                if (archivoInfo2 != null)
                {
                    extensionArchivo2 = Path.GetExtension(archivoInfo2.Name).ToLower();
                }

                if (archivoInfo3 != null)
                {
                    extensionArchivo3 = Path.GetExtension(archivoInfo3.Name).ToLower();
                }

                if (archivoInfo4 != null)
                {
                    extensionArchivo4 = Path.GetExtension(archivoInfo4.Name).ToLower();
                }

                if (archivoInfo5 != null)
                {
                    extensionArchivo5 = Path.GetExtension(archivoInfo5.Name).ToLower();
                }

                //Obtengo lo primero el buffer del archivo para no perderlo luego mediante un cierre de Stream.
                byte[] buffer1 = null;
                if (archivoInfo1 != null)
                {
                    using (BinaryReader reader1 = new BinaryReader(Ontologia.OWL.OpenReadStream()))
                    {
                        buffer1 = reader1.ReadBytes((int)Ontologia.OWL.Length);
                        Ontologia.FicheroOWL = Convert.ToBase64String(buffer1);
                    }
                }

                if (generarXMLDef)
                {
                    string nombreOntologia = null;
                    if (archivoInfo1 != null)
                    {
                        nombreOntologia = archivoInfo1.Name;
                    }
                    else
                    {
                        //Si no hemos reemplazo el archivo ontológico, devolvemos el enlace que ya poseia el doc:
                        nombreOntologia = GestorDocumental.ListaDocumentos[DocumentoID].Enlace;
                    }

                    nombreOntologia = nombreOntologia.Substring(0, nombreOntologia.LastIndexOf("."));

                    //Agrego namespaces y urls:
                    string urlOntologia = BaseURLFormulariosSem + "/Ontologia/" + nombreOntologia + "#";

                    GestionOWL gestorOWL = new GestionOWL();
                    gestorOWL.UrlOntologia = urlOntologia;
                    gestorOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_GNOSS;

                    //Obtengo la ontología:
                    byte[] arrayOntologia = null;

                    if (archivoInfo1 != null)
                    {
                        arrayOntologia = buffer1;
                    }
                    else
                    {
                        arrayOntologia = ControladorDocumentacion.ObtenerOntologia(DocumentoID, ProyectoSeleccionado.Clave);
                    }

                    //Leo la ontología:
                    Ontologia ontologia = new Ontologia(arrayOntologia, true);
                    ontologia.LeerOntologia();
                    ontologia.IdiomaUsuario = IdiomaUsuario;

                    buffer3 = ControladorDocumentacion.GenerarArchivoConfiguracionPlantillaOntologiaGenerico(nombreOntologia, ontologia, !Ontologia.Principal);
                }


                bool archivoOWLValido = (extensionArchivo1 == null || (extensionArchivo1.ToLower() == ".owl" && ControladorDocumentacion.ComprobarBuenFormatoPlantillaOWL(buffer1)));
                bool archivoXMLValido = (extensionArchivo3 == null || extensionArchivo3.ToLower() == ".xml");
                bool archivoCSSValido = (extensionArchivo2 == null || extensionArchivo2.ToLower() == ".css");
                bool archivoIMGValido = (extensionArchivo4 == null || extensionArchivo4.ToLower() == ".jpg" || extensionArchivo4.ToLower() == ".png" || extensionArchivo4.ToLower() == ".gif" || extensionArchivo4.ToLower() == ".jpeg");
                bool archivoJSValido = (extensionArchivo5 == null || extensionArchivo5.ToLower() == ".js");

                if (archivoOWLValido && archivoCSSValido && archivoXMLValido && archivoIMGValido && archivoJSValido)
                {
                    byte[] buffer2 = null;

                    if (archivoInfo2 != null)
                    {
                        if (!Ontologia.GenericCSS)
                        {
                            using (BinaryReader reader2 = new BinaryReader(Ontologia.CSS.OpenReadStream()))
                            {
                                buffer2 = reader2.ReadBytes((int)Ontologia.CSS.Length);
                                Ontologia.FicheroCSS = Convert.ToBase64String(buffer2);
                                reader2.Close();
                            }
                        }
                        else
                        {
                            extensionArchivo2 = ".css";
                        }

                        AgregadoCSS = true;
                    }

                    if (archivoInfo3 != null)
                    {
                        if (!Ontologia.GenericXML)
                        {
                            using (BinaryReader reader3 = new BinaryReader(Ontologia.XML.OpenReadStream()))
                            {
                                buffer3 = reader3.ReadBytes((int)Ontologia.XML.Length);
                                Ontologia.FicheroXML = Convert.ToBase64String(buffer3);
                                reader3.Close();
                                if (!ComprobarXMLConfiguracion(buffer3, ref Error))
                                {
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            using (BinaryReader reader3 = new BinaryReader((new System.IO.StreamReader(archivoInfo3.FullName)).BaseStream))
                            {
                                buffer3 = reader3.ReadBytes((int)archivoInfo3.Length);
                                Ontologia.FicheroXML = Convert.ToBase64String(buffer3);
                                reader3.Close();
                                if (!ComprobarXMLConfiguracion(buffer3, ref Error))
                                {
                                    return null;
                                }
                            }
                        }
                    }

                    if (archivoInfo4 != null)
                    {
                        if (!Ontologia.GenericIMG)
                        {
                            using (BinaryReader reader4 = new BinaryReader(Ontologia.IMG.OpenReadStream()))
                            {
                                buffer4 = reader4.ReadBytes((int)Ontologia.IMG.Length);
                                Ontologia.FicheroIMG = Convert.ToBase64String(buffer4);
                                reader4.Close();
                            }

                            SixLabors.ImageSharp.Image imagen = Image.Load(new MemoryStream(buffer4));

                            if (extensionArchivo4 != ".jpg")
                            {
                                MemoryStream ms = new MemoryStream();
                                imagen.SaveAsJpeg(ms);
                                buffer4 = ms.ToArray();
                                extensionArchivo4 = ".jpg";
                            }

                            if (imagen.Height > 240 || imagen.Width > 240)
                            {
                                SixLabors.ImageSharp.Image imagenPeque = UtilImages.AjustarImagen(imagen, 240, 240);
                                MemoryStream ms = new MemoryStream();
                                imagenPeque.SaveAsJpeg(ms);
                                buffer4 = ms.ToArray();
                                imagenPeque.Dispose();
                            }

                            imagen.Dispose();
                            AgregadoIMG = true;
                        }
                        else
                        {
                            extensionArchivo4 = ".jpg";
                        }
                    }

                    byte[] buffer5 = null;

                    if (archivoInfo5 != null)
                    {
                        if (!Ontologia.GenericJS)
                        {
                            using (BinaryReader reader5 = new BinaryReader(Ontologia.JS.OpenReadStream()))
                            {
                                buffer5 = reader5.ReadBytes((int)Ontologia.JS.Length);
                                Ontologia.FicheroJS = Convert.ToBase64String(buffer5);
                                reader5.Close();
                            }
                        }
                        else
                        {
                            extensionArchivo5 = ".js";
                        }

                        AgregadoJS = true;
                    }

                    #region Guardado archivos

                    int resultado = 0;

                    if (archivoInfo1 != null || extensionArchivo2 != null || buffer3 != null || buffer4 != null || extensionArchivo5 != null)
                    {

                        if (archivoInfo1 != null)
                        {

                            sw = LoggingService.IniciarRelojTelemetria();
                            //Además lo guardamos en la Web:
                            CallFileService fileService = new CallFileService(mConfigService);
                            fileService.GuardarOntologia(buffer1, DocumentoID);

                            resultado = 1;
                            mLoggingService.AgregarEntradaDependencia("Subir ontologia", false, "GuardarArchivos", sw, true);
                        }

                        if (buffer3 != null)//Se comprueba que el fichero xml no sea nulo
                        {
                            doc = new XmlDocument();
                            MemoryStream ms = new MemoryStream(buffer3);
                            doc.Load(ms);
                            proyID = mControladorBase.UsuarioActual.ProyectoID;
                            nombreCortoProy = ProyectoSeleccionado.NombreCorto;
                            if (Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE.ContainsKey(OntologiaID))
                            {
                                Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE[OntologiaID] = buffer3;
                            }
                            else
                            {
                                Servicios.UtilidadesVirtuoso.ONTOLOGIA_XML_CACHE.Add(OntologiaID, buffer3);
                            }
                            Dictionary<string, List<string>> selectores = new Dictionary<string, List<string>>();
                            selectores = ObtenerSelectores(buffer1);
                            foreach (string grafo in selectores.Keys)
                            {
                                foreach (string selector in selectores[grafo])
                                {
                                    FacetaEntidadesExternas facetaEntidad = new FacetaEntidadesExternas();
                                    facetaEntidad.Grafo = grafo;
                                    facetaEntidad.BuscarConRecursividad = true;
                                    facetaEntidad.EsEntidadSecundaria = false;
                                    facetaEntidad.EntidadID = UrlIntragnoss + "items/" + selector;
                                    facetaEntidad.OrganizacionID = UsuarioActual.OrganizacionID;
                                    facetaEntidad.ProyectoID = UsuarioActual.ProyectoID;
                                    AniadirFacetaEntidad(facetaEntidad);
                                }
                            }

                        }
                        if (archivoInfo1 == null || resultado == 1)
                        {
                            if (archivoInfo2 != null)
                            {
                                CallFileService fileService = new CallFileService(mConfigService);
                                fileService.GuardarCSSOntologia(buffer2, DocumentoID, $"{Path.DirectorySeparatorChar}Archivos{Path.DirectorySeparatorChar}{DocumentoID.ToString().Substring(0, 3)}{Path.DirectorySeparatorChar}", extensionArchivo2);
                            }

                            if (buffer3 != null)
                            {
                                CallFileService fileService = new CallFileService(mConfigService);
                                fileService.GuardarXmlOntologia(buffer3, DocumentoID);
                            }

                            if (buffer4 != null)
                            {
                                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService);
                                servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");

                                servicioImagenes.AgregarImagenADirectorioOntologia(buffer4, $"Archivos{Path.DirectorySeparatorChar}{DocumentoID.ToString().Substring(0, 3)}", DocumentoID + "_240", extensionArchivo4);
                            }

                            if (archivoInfo5 != null)
                            {
                                CallFileService fileService = new CallFileService(mConfigService);
                                fileService.GuardarCSSOntologia(buffer5, DocumentoID, $"{Path.DirectorySeparatorChar}Archivos{Path.DirectorySeparatorChar}{DocumentoID.ToString().Substring(0, 3)}{Path.DirectorySeparatorChar}", extensionArchivo5);

                            }

                            resultado = 1;
                        }
                    }
                    else
                    {
                        //Para que no entre por el switch y devuelva null:
                        resultado = 1000;
                    }

                    switch (resultado)
                    {
                        case 0:
                            {
                                Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOC");
                                break;
                            }
                        case 1:
                            {
                                string nuevoEnlace = null;

                                if (archivoInfo1 != null)
                                {
                                    nuevoEnlace = archivoInfo1.Name.ToLower();
                                }
                                else
                                {
                                    //Si no hemos reemplazo el archivo ontológico, devolvemos el enlace que ya poseia el doc:
                                    nuevoEnlace = GestorDocumental.ListaDocumentos[DocumentoID].Enlace;
                                }
                                return nuevoEnlace;
                            }
                        case 2:
                            {
                                Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSEGURIDAD");
                                break;
                            }
                        case 3:
                            {
                                Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCSIZE");
                                break;
                            }
                    }

                    #endregion
                }
                else if ((extensionArchivo1 == null || extensionArchivo1.ToLower() == ".owl") && (extensionArchivo2 == null || extensionArchivo2.ToLower() == ".css") && (extensionArchivo3 == null || extensionArchivo3.ToLower() == ".xml") && (extensionArchivo5 == null || extensionArchivo5.ToLower() == ".js"))
                {
                    //Lo que ha fallado es la comprobación del buen formato del OWL
                    Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCFORMATOARCHIVOOWL", NombreProyectoEcosistema);
                }
                else
                {
                    Error = UtilIdiomas.GetText("PERFILBASESUBIR", "ERRORSUBIRDOCFORMATO");
                    //return null;
                }
            }
            catch (ExcepcionGeneral ex)
            {
                //La ontología está mal construida 
                Error = ex.Message;
                throw;
            }
            catch (Exception)
            {
                //Se han producido errores al guardar el documento en el servidor
                Error = "Se produjo un error al subir el documento, inténtelo de nuevo más tarde.";
                throw;
            }
            return null;
        }
        private void AniadirFacetaEntidad(FacetaEntidadesExternas facetaEntidad)
        {
            if (!mEntityContext.FacetaEntidadesExternas.Any(facetaEntidadExterna => facetaEntidadExterna.OrganizacionID.Equals(facetaEntidad.OrganizacionID) && facetaEntidadExterna.ProyectoID.Equals(facetaEntidad.ProyectoID) && facetaEntidadExterna.EntidadID.Equals(facetaEntidad.EntidadID)) && !mEntityContext.FacetaEntidadesExternas.Local.Any(facetaEntidadExterna => facetaEntidadExterna.OrganizacionID.Equals(facetaEntidad.OrganizacionID) && facetaEntidadExterna.ProyectoID.Equals(facetaEntidad.ProyectoID) && facetaEntidadExterna.EntidadID.Equals(facetaEntidad.EntidadID)))
            {
                mEntityContext.FacetaEntidadesExternas.Add(facetaEntidad);
            }
        }

        /// <summary>
        /// Comprueba la correción del XML de configuración.
        /// </summary>
        /// <param name="pArray">Array del XML</param>
        /// <returns>TRUE si todo va bien, FALSE si no</returns>
        private bool ComprobarXMLConfiguracion(byte[] pArray, ref string Error)
        {
            Error = "";

            try
            {
                LectorXmlConfig lector = new LectorXmlConfig(Guid.Empty, ProyectoSeleccionado.Clave, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD);
                Dictionary<string, List<EstiloPlantilla>> listaEstilos = lector.ObtenerConfiguracionXml(pArray, ProyectoSeleccionado.Clave);

                mGrafosSimplesAutocompletarConfig = ((EstiloPlantillaConfigGen)listaEstilos["[ConfiguracionGeneral]"][0]).GrafosSimplesAutocompletar;
                mPropiedadesOntologia = ((EstiloPlantillaConfigGen)listaEstilos["[ConfiguracionGeneral]"][0]).PropiedadesOntologia;
                return true;
            }
            catch (Exception ex)
            {
                Error = UtilIdiomas.GetText("COMADMIN", "XMLCONFIGURACIONINCORRECTO") + ": " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Devuelve los selectores del archivo XML
        /// </summary> 
        /// <returns>La clave es el grafo y el valor la lista de selectores del grafo</returns>
        private Dictionary<string, List<string>> ObtenerSelectores(byte[] pOntologia)
        {
            string clase = string.Empty;
            Dictionary<string, List<string>> dicSelectores = new Dictionary<string, List<string>>();
            List<string> vSelectores = new List<string>();
            if (EspefEntidad != null)
            {

                XmlNodeList listaSeleccionEntidad = EspefPropiedad.SelectNodes($"Propiedad/SeleccionEntidad");
                foreach (XmlNode seleccionEntidad in listaSeleccionEntidad)
                {
                    XmlNode nodoGrafo = seleccionEntidad.SelectSingleNode("Grafo");
                    if (nodoGrafo != null)
                    {
                        string nombreGrafo = nodoGrafo.InnerText;

                        List<string> listaSelectoresNombre = new List<string>();
                        XmlNode selector = seleccionEntidad.SelectSingleNode("UrlTipoEntSolicitada");
                        if (selector == null)
                        {
                            Ontologia ontologia = null;
                            if (pOntologia == null)
                            {
                                DocumentacionCN documentacionCn = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                Guid ontologiaID = documentacionCn.ObtenerOntologiaAPartirNombre(ProyectoSeleccionado.Clave, nombreGrafo);
                                byte[] ontologiArrray = ControladorDocumentacion.ObtenerOntologia(ontologiaID, ProyectoSeleccionado.Clave);
                                ontologia = new Ontologia(ontologiArrray, true);
                            }
                            else
                            {
                                ontologia = new Ontologia(pOntologia, true);
                            }

                            ontologia.LeerOntologia();
                            List<ElementoOntologia> entidadesPrincipal = GestionOWL.ObtenerElementosContenedorSuperior(ontologia.Entidades);
                            ElementoOntologia entidadPrincipal = null;
                            if (entidadesPrincipal != null && entidadesPrincipal.Count > 0)
                            {
                                entidadPrincipal = entidadesPrincipal.First();
                                string tipoEntidad = entidadPrincipal.TipoEntidadRelativo;
                                if (!listaSelectoresNombre.Contains(tipoEntidad))
                                {
                                    listaSelectoresNombre.Add(tipoEntidad);
                                }
                            }
                        }
                        else if (selector != null)
                        {
                            string[] sel = selector.InnerText.Split(',');
                            foreach (string selNombre in sel)
                            {
                                string nombre = "";
                                if (selNombre != null && selNombre.Contains("#"))
                                {
                                    nombre = selNombre.Substring(selNombre.LastIndexOf("#") + 1);
                                }
                                else if (selNombre != null && selNombre.Contains("/"))
                                {
                                    nombre = selNombre.Substring(selNombre.LastIndexOf("/") + 1);
                                }
                                else
                                {
                                    nombre = selNombre;
                                }

                                if (!listaSelectoresNombre.Contains(nombre))
                                {
                                    listaSelectoresNombre.Add(nombre);
                                }
                            }
                        }

                        if (dicSelectores.ContainsKey(nombreGrafo))
                        {
                            vSelectores = dicSelectores[nombreGrafo];
                            vSelectores = vSelectores.Union(listaSelectoresNombre).ToList();
                            dicSelectores[nombreGrafo] = vSelectores;
                        }
                        else
                        {
                            dicSelectores.Add(nombreGrafo, listaSelectoresNombre);
                        }
                    }
                }
            }

            return dicSelectores;
        }

        private List<string> CargarHistorial(Guid pOntologiaID)
        {
            List<string> listadoCopias = null;

            CallFileService fileService = new CallFileService(mConfigService);

            string[] archivos = fileService.ObtenerHistorialOntologia(pOntologiaID);

            if (archivos != null && archivos.Length > 0)
            {
                listadoCopias = new List<string>(archivos);
            }


            return listadoCopias;
        }

        private PlantillaOntologicaViewModel ObtenerTemplate(Documento pDocumento)
        {
            PlantillaOntologicaViewModel template = new PlantillaOntologicaViewModel();
            template.OntologyID = pDocumento.Clave;
            template.IsSecondaryOntology = pDocumento.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria;

            template.Name = pDocumento.Titulo;
            template.OntologyName = pDocumento.Enlace.Replace(".owl", "");
            template.Description = pDocumento.Descripcion;
            if (!string.IsNullOrEmpty(pDocumento.FilaDocumento.NombreCategoriaDoc) && pDocumento.FilaDocumento.NombreCategoriaDoc.Contains(".jpg"))
            {
                template.Image = pDocumento.FilaDocumento.NombreCategoriaDoc.Split(',')[1].Replace(".jpg", "_" + pDocumento.FilaDocumento.NombreCategoriaDoc.Split(',')[0] + ".jpg");
            }
            template.Protected = pDocumento.FilaDocumento.Protegido;
            if (ListaDocumentosConRecursos.ContainsKey(pDocumento.Clave))
            {
                template.HasResources = true;
            }


            bool cargasMultiplesDisponibles = false;
            Dictionary<string, string> propiedades = UtilCadenas.ObtenerPropiedadesDeTexto(pDocumento.NombreEntidadVinculada);
            if (propiedades.ContainsKey(PropiedadesOntologia.cargasmultiples.ToString()) && propiedades[PropiedadesOntologia.cargasmultiples.ToString()] == "true")
            {
                cargasMultiplesDisponibles = true;
            }
            template.AllowMasiveUpload = cargasMultiplesDisponibles;

            template.HasXmlFile = true;

            if (pDocumento.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria)
            {
                List<ProyectoConfigExtraSem> filas = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(pDocumento.Enlace) && proy.Tipo.Equals((short)TipoConfigExtraSemantica.EntidadSecundaria)).ToList();
                if (filas == null || filas.Count == 0)
                {
                    template.HasXmlFile = false;

                }
            }

            return template;
        }

        /// <summary>
        /// Guarda el documento en la base de datos.
        /// </summary>
        private void GuardarCambios()
        {
            mEntityContext.SaveChanges();

            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCL.InvalidarOntologiasProyecto(ProyectoSeleccionado.Clave);

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarOntologiasEcosistema();
        }

        #endregion

        #region Propiedades
        private AdministrarPlantillasOntologicasViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarPlantillasOntologicasViewModel();
                    mPaginaModel.Templates = new List<PlantillaOntologicaViewModel>();
                    mPaginaModel.SecondaryTemplates = new List<PlantillaOntologicaViewModel>();

                    foreach (Documento doc in GestorDocumental.ListaDocumentos.Values)
                    {
                        if (doc.TipoDocumentacion == TiposDocumentacion.Ontologia)
                        {
                            mPaginaModel.Templates.Add(ObtenerTemplate(doc));
                        }
                        else if (doc.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria)
                        {
                            mPaginaModel.SecondaryTemplates.Add(ObtenerTemplate(doc));
                        }
                    }

                    mPaginaModel.SelectedOntology = new PlantillaOntologicaViewModel();

                    Guid ontologiaSeleccionada = OntologiaID;

                    if (!ontologiaSeleccionada.Equals(Guid.Empty) && GestorDocumental.ListaDocumentos.ContainsKey(ontologiaSeleccionada))
                    {
                        mPaginaModel.SelectedOntology = ObtenerTemplate(GestorDocumental.ListaDocumentos[ontologiaSeleccionada]);
                    }

                }
                return mPaginaModel;
            }

        }

        private Guid OntologiaID
        {
            get
            {
                Guid ontologiaSeleccionada = Guid.Empty;
                if (!string.IsNullOrEmpty(RequestParams("ontoID")))
                {
                    Guid.TryParse(RequestParams("ontoID"), out ontologiaSeleccionada);
                }

                return ontologiaSeleccionada;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private GestorDocumental GestorDocumental
        {
            get
            {
                if (mGestorDocumental == null)
                {
                    mGestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext);

                    DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    docCL.ObtenerBaseRecursosProyecto(mGestorDocumental.DataWrapperDocumentacion, mControladorBase.UsuarioActual.ProyectoID, mControladorBase.UsuarioActual.OrganizacionID, mControladorBase.UsuarioActual.UsuarioID);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    docCN.ObtenerOntologiasProyecto(mControladorBase.UsuarioActual.ProyectoID, mGestorDocumental.DataWrapperDocumentacion, true, true, false, true);
                    docCN.Dispose();

                    mGestorDocumental.CargarDocumentos();
                }
                return mGestorDocumental;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private DataWrapperProyecto ProyectoConfigSemDataWrapperProyecto
        {
            get
            {
                if (mProyectoConfigSemDataWrapperProyecto == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mProyectoConfigSemDataWrapperProyecto = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(ProyectoSeleccionado.Clave);
                    proyCN.Dispose();
                }
                return mProyectoConfigSemDataWrapperProyecto;
            }

        }
        private Dictionary<Guid, bool> ListaDocumentosConRecursos
        {
            get
            {
                if (mListaDocumentosConRecursos == null)
                {
                    List<Guid> listaDocs = new List<Guid>(GestorDocumental.ListaDocumentos.Keys);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mListaDocumentosConRecursos = docCN.ComprobarSiOntologiaTieneRecursos(listaDocs);
                    docCN.Dispose();
                }
                return mListaDocumentosConRecursos;
            }
        }
        private XmlNode EspefEntidad
        {
            get
            {
                XmlNodeList listaespef = null;
                if (doc.DocumentElement != null)
                {
                    listaespef = doc.DocumentElement.SelectNodes("EspefEntidad");
                    if (listaespef != null)
                    {
                        if (listaespef.Count > 1)
                        {
                            foreach (XmlNode espef in listaespef)
                            {
                                if (espef.Attributes.Count > 0)
                                {
                                    if (espef.Attributes[0].InnerText.Equals(proyID))
                                    {
                                        return espef;
                                    }
                                    else if (espef.Attributes[0].InnerText.Equals(nombreCortoProy))
                                    {
                                        return espef;
                                    }
                                }
                            }
                            foreach (XmlNode espef in listaespef)
                            {
                                if (espef.Attributes.Count < 1)
                                {
                                    return espef;
                                }
                            }
                        }
                    }
                }
                if (listaespef != null)
                {
                    return listaespef.Item(0);
                }
                else { return null; }
            }
        }
        private XmlNode EspefPropiedad
        {
            get
            {
                XmlNodeList listaEspefPropiedad = null;
                if (doc.DocumentElement != null)
                {
                    listaEspefPropiedad = doc.DocumentElement.SelectNodes("EspefPropiedad");
                    if (listaEspefPropiedad != null)
                    {
                        if (listaEspefPropiedad.Count > 1)
                        {
                            foreach (XmlNode espef in listaEspefPropiedad)
                            {

                                if (espef.Attributes.Count > 0)
                                {
                                    if (espef.Attributes[0].InnerText.Equals(proyID))
                                    {
                                        return espef;
                                    }
                                    else if (espef.Attributes[0].InnerText.Equals(nombreCortoProy))
                                    {
                                        return espef;
                                    }
                                }
                            }
                            foreach (XmlNode espef in listaEspefPropiedad)
                            {
                                if (espef.Attributes.Count < 1)
                                {
                                    return espef;
                                }
                            }
                            //Devolver el que coincida con el proyecto

                            //Devolver el que coincida con el nombrecorto

                            //Devolver el que no tenga attributos
                        }
                    }
                }
                if (listaEspefPropiedad != null)
                {
                    return listaEspefPropiedad.Item(0);
                }
                else { return null; }
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class AdministrarPlantillasOntologicasViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public List<PlantillaOntologicaViewModel> Templates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PlantillaOntologicaViewModel> SecondaryTemplates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PlantillaOntologicaViewModel SelectedOntology { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PlantillaOntologicaViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSecondaryOntology { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OntologyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Protected { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool HasResources { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AllowMasiveUpload { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool HasXmlFile { get; set; }

    }


    public class OntologicalTemplatesAdministrationViewModel
    {
        public Guid OntologyID { get; set; }
        public List<string> VersionList { get; set; }
        public string OntologyName { get; set; }
    }

    public class HttpPostedFileConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var stream = (Stream)value;
            using (var sr = new BinaryReader(stream))
            {
                var buffer = sr.ReadBytes((int)stream.Length);
                writer.WriteValue(Convert.ToBase64String(buffer));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(Stream));
        }
    }
}
