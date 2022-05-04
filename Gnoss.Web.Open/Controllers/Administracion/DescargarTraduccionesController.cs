using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Traducciones;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{
    public class DescargarTraduccionesController : ControllerBaseWeb
    {
        public DescargarTraduccionesController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        /// <summary>
        /// Muestra la pagina de administrar categorías
        /// </summary>
        /// <returns>ActionResult</returns>
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Diseño })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            DescargarTraduccionesViewModel descargarTraducciones = new DescargarTraduccionesViewModel();


            //var ontologiasPrincipalesF = mEntityContext.Documento.Where(item => item.ProyectoID.HasValue && !item.Eliminado && item.ProyectoID.Value.Equals(ProyectoSeleccionado.Clave) && item.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList();

            var ontologiasPrincipalesConsulta = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRec => baseRec.BaseRecursosID, (item, baseRec) => new
            {
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRec
            }).Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && !item.Documento.Eliminado && item.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia)).Select(item => item.Documento);
            var ontologiasPrincipales = ontologiasPrincipalesConsulta.ToList();
            descargarTraducciones.mListaOntologiasPrincipales = new Dictionary<string, string>();

            foreach (var ontologiaPrincipal in ontologiasPrincipales)
            {
                string enlace = ontologiaPrincipal.Enlace.ToLower();

                if (enlace != null)
                {
                    string ontologia = enlace.Substring(0, enlace.IndexOf(".owl"));
                    string ontologiaTitulo = ontologiaPrincipal.Titulo;

                    descargarTraducciones.mListaOntologiasPrincipales.Add(ontologia, ontologiaTitulo);
                }
            }


            var ontologiasSecundarias = mEntityContext.Documento.Where(item => item.ProyectoID.HasValue && !item.Eliminado && item.ProyectoID.Value.Equals(ProyectoSeleccionado.Clave) && item.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)).ToList();

            descargarTraducciones.mListaOntologiasSecundarias = new Dictionary<string, string>();
            descargarTraducciones.mListaOntologiasSecundariasTaxonomias = new Dictionary<string, string>();

            foreach (var ontologiaSecundaria in ontologiasSecundarias)
            {
                string enlace = ontologiaSecundaria.Enlace.ToLower();

                if (enlace.EndsWith(".owl"))
                {
                    string ontologia = enlace.Substring(0, enlace.IndexOf(".owl"));
                    string ontologiaTitulo = ontologiaSecundaria.Titulo;

                    if (!enlace.Equals("taxonomy.owl"))
                    {
                        descargarTraducciones.mListaOntologiasSecundarias.Add(ontologia, ontologiaTitulo);
                    }

                    if (enlace.Equals("taxonomy.owl"))
                    {
                        descargarTraducciones.mListaOntologiasSecundariasTaxonomias.Add(ontologia, ontologiaTitulo);
                    }
                }
            }

            return View(descargarTraducciones);
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Diseño })]
        public ActionResult subirFicheros(IFormFile file, bool validar)
        {
            if (!validar)
            {
                Dictionary<string, MemoryStream> listMStream = new Dictionary<string, MemoryStream>();
                Stream ms = new MemoryStream();

                if (file != null)
                {
                    file.OpenReadStream().CopyTo(ms);
                }

                DataSet ds = UtilFicheros.LeerExcelDeRutaADataSet(ms);

                int contadorTablas = ds.Tables.Count;

                for (int i = 0; i < contadorTablas; i++)
                {
                    DataTable tabla = ds.Tables[i];
                    string nombreTabla = tabla.TableName;


                    if (nombreTabla.Equals("JavaScript"))
                    {
                        TraduccionJavaScript js = new TraduccionJavaScript();
                        js.ExcelToJavascript(tabla, listMStream);
                    }
                    else if (nombreTabla.Equals("MensajesCore"))
                    {
                        TraduccionXmlMensajes mensajes = new TraduccionXmlMensajes();
                        mensajes.ExcelMensajesToXml(tabla, listMStream);
                    }
                    else if (nombreTabla.Equals("Core"))
                    {
                        TraduccionXmlCore core = new TraduccionXmlCore();
                        core.ExcelCoreToXml(tabla, listMStream);
                    }
                    else
                    {
                        BaseDeDatos bd = new BaseDeDatos(ProyectoSeleccionado, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mRedisCacheWrapper, mHttpContextAccessor, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);
                        if (nombreTabla.Contains("tabla_"))
                        {
                            switch (nombreTabla)
                            {
                                case "tabla_TextosPersonalizados":
                                    bd.ExcelToTextosPersonalizados(ProyectoSeleccionado.PersonalizacionID, tabla);
                                    break;
                                case "tabla_TextosPestañas":
                                    bd.ExcelToProyectoPestanyaMenu(ProyectoSeleccionado.Clave, tabla);
                                    break;
                                case "tabla_TextosComponentesCMS":
                                    bd.ExcelToCmsPropiedadComponente(ProyectoSeleccionado.Clave, tabla);
                                    break;
                                case "tabla_TextosFacetas":
                                    bd.ExcelToFacetaObjetoConocimientoProyecto(ProyectoSeleccionado.Clave, tabla);
                                    break;
                                case "tabla_CategoriasTesauro":
                                    bd.ExcelToCategoriaTesauro(ProyectoSeleccionado.Clave, tabla);
                                    break;
                                case "tabla_ClausulasRegistro":
                                    bd.ExcelToClausulaRegistro(ProyectoSeleccionado.Clave, tabla);
                                    break;
                                case "tabla_ComponentesRecursos":
                                    bd.ExcelToProyectoGadget(ProyectoSeleccionado.Clave, tabla);
                                    break;
                                case "tabla_ConfiguracionOntologias":
                                    bd.ExcelToOntologiaProyectoSql(ProyectoSeleccionado.Clave, tabla);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (nombreTabla.Contains("PrimaryOnto"))
                        {
                            bd.ExcelToVirtuoso(tabla, UrlIntragnoss, listMStream, ProyectoSeleccionado.Clave);
                        }
                        else if (nombreTabla.Contains("SecondaryOnto"))
                        {
                            bd.ExcelToVirtuoso(tabla, UrlIntragnoss, listMStream, ProyectoSeleccionado.Clave);
                        }
                        else if (nombreTabla.Contains("taxonomy"))
                        {
                            bd.ExcelToVirtuoso(tabla, UrlIntragnoss, listMStream, ProyectoSeleccionado.Clave);
                        }
                    }
                }

                byte[] bytesZip = null;

                using (var msZip = new MemoryStream())
                {
                    using (ZipArchive zipArchive = new ZipArchive(msZip, ZipArchiveMode.Create, true))
                    {
                        foreach (string nombreFichero in listMStream.Keys)
                        {
                            MemoryStream fichero = listMStream[nombreFichero];

                            var entry = zipArchive.CreateEntry(nombreFichero);
                            using (Stream entryStream = entry.Open())
                            {
                                fichero.CopyTo(entryStream);
                            }
                        }
                    }
                    msZip.Flush();
                    bytesZip = msZip.ToArray();
                }

                if (bytesZip.Length > 0)
                {
                    return File(bytesZip, "application/octet-stream", $"Traducciones_{ProyectoSeleccionado.NombreCorto}.zip");
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return validarHtml(file);
            }
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Diseño })]
        public ActionResult validarHtml(IFormFile file)
        {
            Stream ms = new MemoryStream();

            if (file != null)
            {
                file.OpenReadStream().CopyTo(ms);
            }

            DataSet ds = UtilFicheros.LeerExcelDeRutaADataSet(ms);
            return obtenerErroresValidacionHtml(ds);
        }

        private FileContentResult obtenerErroresValidacionHtml(DataSet ds)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            foreach (DataTable hojaExcel in ds.Tables)
            {
                bool escribirNombreTabla = true;

                for (int j = 0; j < hojaExcel.Rows.Count; j++)
                {
                    bool escribirFila = true;
                    for (int i = 2; i < hojaExcel.Columns.Count; i++)
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(hojaExcel.Rows[j][i].ToString());

                        if (doc.ParseErrors.Any())
                        {
                            if (escribirNombreTabla)
                            {
                                writer.Write(hojaExcel.TableName);
                                escribirNombreTabla = false;
                            }
                            if (escribirFila)
                            {
                                writer.Write($"\r\n\tFila: {j + 2}");
                                escribirFila = false;
                            }

                            writer.Write($"\r\n\t\tIdioma: {hojaExcel.Columns[i]}");

                            foreach (var error in doc.ParseErrors)
                            {
                                writer.Write($"\r\n\t\t\tError: {error.Reason}");
                            }
                        }
                    }
                }
                if (!escribirNombreTabla)
                {
                    writer.Write(Environment.NewLine);
                }
            }
            writer.Flush();

            if (stream.Length > 0)
            {
                stream.Position = 0;
                return File(stream.ToArray(), "text/plain", $"Errores.txt");
            }
            return null;
        }

        [HttpPost]
        [TypeFilter(typeof(PermisosPaginasUsuariosAttribute), Arguments = new object[] { "", TipoPaginaAdministracion.Diseño })]
        public FileResult getDescargasFicheros(DescargarTraduccionesViewModel OpcionesDescarga)
        {
            //comprobar errores

            BaseDeDatos bd = new BaseDeDatos(ProyectoSeleccionado, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mRedisCacheWrapper, mHttpContextAccessor, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);
            ExcelPackage excel = new ExcelPackage();
            TraduccionXmlOntologias ontologiaXML = new TraduccionXmlOntologias();
            List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();

            //descargar ficheros segun sean seleccionados

            //XML
            if (OpcionesDescarga.DescargarXMLCore) //descargar excel de core de XML
            {
                Dictionary<string, Stream> diccionarioXMLCore = new Dictionary<string, Stream>();

                foreach (string idioma in listaIdiomas)
                {
                    diccionarioXMLCore.Add(idioma, Es.Riam.Gnoss.Recursos.UtilIdiomas.ObtenerDocumentoTextoIdioma(idioma));
                }

                TraduccionXmlCore xml = new TraduccionXmlCore();
                xml.XmlToExcelCore(diccionarioXMLCore, excel);
            }

            if (OpcionesDescarga.DescargarXMLMensajesCore) //descargar excel de mensajes de XML
            {
                Dictionary<string, Stream> diccionarioXMLMensajesCore = new Dictionary<string, Stream>();

                foreach (string idioma in listaIdiomas)
                {
                    string contenidoXML = GestionNotificaciones.ObtenerContenidoTextosMensajes(idioma);

                    if (!string.IsNullOrEmpty(contenidoXML))
                    {
                        var stream = new MemoryStream();
                        var writer = new StreamWriter(stream);
                        writer.Write(contenidoXML);
                        writer.Flush();
                        stream.Position = 0;

                        diccionarioXMLMensajesCore.Add(idioma, stream);
                    }
                }

                TraduccionXmlMensajes xml = new TraduccionXmlMensajes();
                xml.XmlToExcelMensajes(diccionarioXMLMensajesCore, excel);
            }

            //Ontologias principales    

            if (OpcionesDescarga.mListaOntologiasPrincipalesXML != null && OpcionesDescarga.mListaOntologiasPrincipalesXML.Count > 0)
            {
                foreach (string ontologiaPrincipal in OpcionesDescarga.mListaOntologiasPrincipalesXML.Keys)
                {
                    string nombreHoja = $"XML_Ontologia_{ontologiaPrincipal}";
                    Guid? ontologiasPrincipales = mEntityContext.Documento.Where(item => item.Enlace.Equals(ontologiaPrincipal + ".owl")).Select(item => item.DocumentoID).FirstOrDefault();

                    if (!ontologiasPrincipales.Equals(Guid.Empty))
                    {
                        byte[] array = ObtenerXmlOntologia(ontologiasPrincipales.Value);

                        if (array != null)
                        {
                            Stream stream = new MemoryStream(array);

                            ontologiaXML.XmlOntologiaToExcel(stream, excel, nombreHoja);
                        }
                    }
                }
            }


            //Base de Datos        
            if (OpcionesDescarga.DescargarTextosPersonalizados) //TextosPersonalizados 
            {
                bd.TextosPersonalizadosToExcel(ProyectoSeleccionado.Clave, excel);
            }

            if (OpcionesDescarga.DescargarTextoProyectoPestanyaMenu) //ProyectoPestanyaMenu
            {
                bd.ProyectoPestanyaMenuToExcel(ProyectoSeleccionado.Clave, excel);
            }

            if (OpcionesDescarga.DescargarCmsPropiedadComponente) //CmsPropiedadComponente
            {
                bd.CmsPropiedadComponenteToExcel(ProyectoSeleccionado.Clave, excel);
            }

            if (OpcionesDescarga.DescargarFacetaObjetoConocimientoProyecto) //FacetaObjetoConocimientoProyecto
            {
                bd.FacetaObjetoConocimientoProyectoToExcel(ProyectoSeleccionado.Clave, excel);
            }

            if (OpcionesDescarga.DescargarCategoriaTesauro) //CategoriaTesauro
            {
                bd.CategoriaTesauroToExcel(ProyectoSeleccionado.Clave, excel);
            }

            if (OpcionesDescarga.DescargarClausulaRegistro) //ClausulaRegistro
            {
                bd.ClausulaRegistroToExcel(ProyectoSeleccionado.Clave, excel);
            }

            if (OpcionesDescarga.DescargarProyectoGadget) //ProyectoGadget
            {
                bd.ProyectoGadgetToExcel(ProyectoSeleccionado.Clave, excel);
            }

            if (OpcionesDescarga.DescargarOntologiaProyectoSql) //OntologiaProyectoSql
            {
                bd.OntologiaProyectoToExcelSql(ProyectoSeleccionado.Clave, excel);
            }


            //descargar ontologias seleccionadas principales
            if (OpcionesDescarga.mListaOntologiasPrincipales != null && OpcionesDescarga.mListaOntologiasPrincipales.Count > 0)
            {
                foreach (string ontologiaPrincipal in OpcionesDescarga.mListaOntologiasPrincipales.Keys)
                {

                    bd.OntologiaProyectoToExcelVirtuosoPorFecha(ProyectoSeleccionado.Clave, ontologiaPrincipal + ".owl", OpcionesDescarga.fechaFin, OpcionesDescarga.fechaInicio, excel, UrlIntragnoss);
                }
            }

            //descargar ontologias seleccionadas secundarias

            if (OpcionesDescarga.mListaOntologiasSecundarias != null && OpcionesDescarga.mListaOntologiasSecundarias.Count > 0)
            {
                if (!OpcionesDescarga.seleccionarTodosOntoSecundarias)
                {
                    foreach (string ontologiaSecundaria in OpcionesDescarga.mListaOntologiasSecundarias.Keys)
                    {
                        bd.OntologiaSecundariaToExcelSeleccionados(ProyectoSeleccionado.Clave, ontologiaSecundaria + ".owl", excel, UrlIntragnoss);
                    }
                }
                else //selecciona seleccionar todos
                {
                    bd.OntologiasSecundariosToExcelVirtuoso(ProyectoSeleccionado.Clave, excel, UrlIntragnoss);
                }
            }


            //descargar taxonomias seleccionadas

            if (OpcionesDescarga.mListaOntologiasSecundariasTaxonomias != null && OpcionesDescarga.mListaOntologiasSecundariasTaxonomias.Count > 0)
            {
                if (!OpcionesDescarga.seleccionarTodosTaxonomias)
                {
                    foreach (string ontologiaTaxonomy in OpcionesDescarga.mListaOntologiasSecundariasTaxonomias.Keys)
                    {
                        bd.OntologiasSecundariasToExcelVirtuosoTaxonomy(ProyectoSeleccionado.Clave, excel, UrlIntragnoss);
                    }
                }
            }

            //JavaScript

            if (OpcionesDescarga.DescargarJS)
            {

                Dictionary<string, Stream> diccionarioJavaScript = new Dictionary<string, Stream>();

                foreach (string idioma in listaIdiomas)
                {
                    string urlJS = $"{BaseURLStatic}/lang/{idioma}/text.js";

                    try
                    {
                        WebRequest request = WebRequest.Create(urlJS);
                        request.Credentials = CredentialCache.DefaultCredentials;
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string contenidoJS = reader.ReadToEnd();
                        reader.Close();
                        dataStream.Close();
                        response.Close();

                        if (!string.IsNullOrEmpty(contenidoJS))
                        {
                            var stream = new MemoryStream();
                            var writer = new StreamWriter(stream);
                            writer.Write(contenidoJS);
                            writer.Flush();
                            stream.Position = 0;

                            diccionarioJavaScript.Add(idioma, stream);
                        }
                    }
                    catch
                    {
                        //Controlamos que da un error al obtener el fichero javascript
                    }
                }

                TraduccionJavaScript js = new TraduccionJavaScript();
                js.JavascriptToExcel(excel, diccionarioJavaScript);
            }

            if (excel.Workbook.Worksheets.Count > 0)
            {
                return File(excel.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Traducciones_{ProyectoSeleccionado.NombreCorto}.xlsx");
            }
            else
            {
                return null;
            }

        }

        private byte[] ObtenerXmlOntologia(Guid pOntologiaID)
        {
            CallFileService fileService = new CallFileService(mConfigService);
            byte[] bytesArchivo = fileService.ObtenerXmlOntologiaBytes(pOntologiaID);

            return bytesArchivo;
        }

    }

    public class DescargarTraduccionesViewModel
    {

        //Listas ontologias
        public Dictionary<string, string> mListaOntologiasPrincipalesXML { get; set; }
        public Dictionary<string, string> mListaOntologiasPrincipales { get; set; }
        public Dictionary<string, string> mListaOntologiasSecundarias { get; set; }
        public Dictionary<string, string> mListaOntologiasSecundariasTaxonomias { get; set; }

        //seleccionarTodos
        public bool seleccionarTodosOntoPrincipales { get; set; }
        public bool seleccionarTodosOntoSecundarias { get; set; }
        public bool seleccionarTodosTaxonomias { get; set; }

        //xml
        public bool DescargarXMLCore { get; set; }
        public bool DescargarXMLMensajesCore { get; set; }

        //JS
        public bool DescargarJS { get; set; }

        //BaseDeDatos
        public bool DescargarTextosPersonalizados { get; set; }
        public bool DescargarTextoProyectoPestanyaMenu { get; set; }
        public bool DescargarCmsPropiedadComponente { get; set; }
        public bool DescargarFacetaObjetoConocimientoProyecto { get; set; }
        public bool DescargarCategoriaTesauro { get; set; }
        public bool DescargarClausulaRegistro { get; set; }
        public bool DescargarProyectoGadget { get; set; }
        public bool DescargarOntologiaProyectoSql { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }

    }
}