using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Filters;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;


namespace Es.Riam.Gnoss.Web.MVC.Controllers.Administracion
{

    /// <summary>
    /// ViewModel de la página de administrar pestañas
    /// </summary>
    [Serializable]
    public class AdministrarEventosViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, string> ListaComponentesCMS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<short, string> ListaTiposEventos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EventModel SelectedEvent { get; set; }

        /// <summary>
        /// Lista de pestañas de la comunidad
        /// </summary>
        public List<EventModel> ListaEventos { get; set; }

        /// <summary>
        /// Modelo de pestañas de una comunidad
        /// </summary>
        [Serializable]
        public partial class EventModel
        {
            /// <summary>
            /// 
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool Interno { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Desciption { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Enlace { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public short Type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int NumMembers { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string InfoExtra { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Guid ComponenteCMS { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool Active { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Group { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string UrlRedirect { get; set; }
        }
    }

    public class AdministrarEventosController : ControllerBaseWeb
    {
        public AdministrarEventosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private AdministrarEventosViewModel mPaginaModel = null;

        #endregion

        #region Metodos de evento

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Index()
        {
            EliminarPersonalizacionVistas();
            CargarPermisosAdministracionComunidadEnViewBag();

            if(!ParametrosGeneralesRow.EventosDisponibles)
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
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult Guardar(AdministrarEventosViewModel.EventModel evento)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto dataWrapperProyecto;

            Guid eventoID = evento.Key;

            if(eventoID.Equals(Guid.Empty)) {
                dataWrapperProyecto = new DataWrapperProyecto();
                eventoID = Guid.NewGuid();
            }
            else
            {
                dataWrapperProyecto = proyCN.ObtenerEventoProyectoPorEventoID(eventoID);
            }

            ProyectoEvento filaEvento = null;
            if (dataWrapperProyecto.ListaProyectoEvento.Count == 1)
            {
                filaEvento = dataWrapperProyecto.ListaProyectoEvento.First();
            }
            else
            {
                ProyectoEvento proyectoEvento = new ProyectoEvento();
                proyectoEvento.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                proyectoEvento.ProyectoID = ProyectoSeleccionado.Clave;
                proyectoEvento.EventoID = eventoID;
                proyectoEvento.Nombre = "";
                proyectoEvento.Descripcion = "";
                proyectoEvento.TipoEvento = (short)TipoEventoProyecto.SinRestriccion;
                proyectoEvento.Activo = true;
                proyectoEvento.InfoExtra = null;
                proyectoEvento.Interno = false;
                proyectoEvento.ComponenteID = Guid.Empty;
                proyectoEvento.UrlRedirect = "";
                proyectoEvento.Grupo = "";
                filaEvento = proyectoEvento;
                dataWrapperProyecto.ListaProyectoEvento.Add(filaEvento);
                if (!(mEntityContext.ProyectoEvento.Any(proy=>proy.OrganizacionID.Equals(proyectoEvento.OrganizacionID) && proy.EventoID.Equals(proyectoEvento.EventoID))))
                {
                    mEntityContext.ProyectoEvento.Add(filaEvento);
                }
            }

            filaEvento.Nombre = evento.Name;
            filaEvento.Descripcion = HttpUtility.UrlDecode(evento.Desciption);
            filaEvento.TipoEvento = evento.Type;
            filaEvento.InfoExtra = evento.InfoExtra;
            filaEvento.Grupo = evento.Group;
            filaEvento.UrlRedirect = evento.UrlRedirect;

            //asignamos el componente si está disponible el CMS en la comunidad, y en los argumentos del callback llega el guid del componente
            if (evento.ComponenteCMS.Equals(Guid.Empty))
            {
                filaEvento.ComponenteID = null;
            }
            else { 
                filaEvento.ComponenteID = evento.ComponenteCMS;

                //Comprobar si exsite el componente
                CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerComponentePorID(evento.ComponenteCMS, ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                cmsCN.Dispose();
                if (!gestorCMS.ListaComponentes.ContainsKey(evento.ComponenteCMS))
                {
                    return GnossResultERROR("No se puede vincular un evento a un componente que no existe");
                }
            }

            proyCN.ActualizarProyectos();
            proyCN.Dispose();

            return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "COMADMINCOMEVENTOS"));
        }

        public ActionResult Delete(Guid eventoID)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerEventoProyectoPorEventoID(eventoID);
            ProyectoEvento filaEvento = dataWrapperProyecto.ListaProyectoEvento.First();
            mEntityContext.EliminarElemento(filaEvento);
            proyCN.ActualizarProyectos();
            proyCN.Dispose();

            return GnossResultOK();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult SelectInterno(Guid eventoID)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerEventosProyectoPorProyectoID(ProyectoSeleccionado.Clave);

            foreach (ProyectoEvento filaEvento in dataWrapperProyecto.ListaProyectoEvento)
            {
                if (filaEvento.EventoID.Equals(eventoID))
                {
                    filaEvento.Interno = true;
                }
                else
                {
                    filaEvento.Interno = false;
                }
            }

            proyCN.ActualizarProyectos();
            proyCN.Dispose();

            return GnossResultOK();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult ChangeActive(Guid eventoID, bool Activar)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerEventoProyectoPorEventoID(eventoID);
            ProyectoEvento filaEvento = dataWrapperProyecto.ListaProyectoEvento.First();
            filaEvento.Activo = Activar;

            proyCN.ActualizarProyectos();
            proyCN.Dispose();

            return GnossResultOK();
        }

        /// <summary>
        /// Filtra y monta el documento CSV que se le devuelve al usuario.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(UsuarioLogueadoAttribute), Arguments = new object[] { RolesUsuario.AdministradorComunidad })]
        public ActionResult DownloadCSV(Guid eventoID)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerEventoProyectoPorEventoID(eventoID);
            dataWrapperProyecto.Merge(proyCN.ObtenerEventoProyectoParticipantesPorEventoID(eventoID));
            ProyectoEvento pFilaEvento = dataWrapperProyecto.ListaProyectoEvento.First();

            DataSet dataSetMiembros = proyCN.ObtenerEmailsMiembrosDeEventoDeProyecto(eventoID);

            Dictionary<Guid, KeyValuePair<string, string>> ListaIdentidadNombreEmail = new Dictionary<Guid, KeyValuePair<string, string>>();
            foreach (DataRow fila in dataSetMiembros.Tables[0].Rows)
            {
                ListaIdentidadNombreEmail.Add((Guid)fila["identidadID"], new KeyValuePair<string, string>((string)fila["Nombre"] + " " + (string)fila["Apellidos"], (string)fila["Email"]));
            }
            proyCN.Dispose();

            MemoryStream escritor = new MemoryStream();
            byte[] byteArray = Array.Empty<byte>();
            StreamWriter sw = new StreamWriter(escritor, Encoding.Default);
            sw.Write("Nombre");
            sw.Write(";");
            sw.Write("Email");
            sw.Write(";");
            sw.Write("Fecha");
            sw.Write(";");
            sw.WriteLine();


            Dictionary<Guid, DateTime> listaIdentidadFecha = new Dictionary<Guid, DateTime>();
            foreach (ProyectoEventoParticipante filaParticipante in dataWrapperProyecto.ListaProyectoEventoParticipante)
            {
                listaIdentidadFecha.Add(filaParticipante.IdentidadID, filaParticipante.Fecha);
            }

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add("miembros");
            dataSet.Tables["miembros"].Columns.Add("nombre", "".GetType());
            dataSet.Tables["miembros"].Columns.Add("email", "".GetType());
            dataSet.Tables["miembros"].Columns.Add("fecha", new DateTime().GetType());
            foreach (Guid identidadID in ListaIdentidadNombreEmail.Keys)
            {
                string nombre = ListaIdentidadNombreEmail[identidadID].Key;
                string email = ListaIdentidadNombreEmail[identidadID].Value;
                DateTime fecha = DateTime.Now;
                if (listaIdentidadFecha.ContainsKey(identidadID))
                {
                    fecha = listaIdentidadFecha[identidadID];
                }
                dataSet.Tables["miembros"].Rows.Add(nombre, email, fecha);
            }

            foreach (DataRow fila in dataSet.Tables["miembros"].Select("", " fecha desc"))
            {
                sw.Write(fila["nombre"]);
                sw.Write(";");
                sw.Write(fila["email"]);
                sw.Write(";");
                sw.Write(fila["fecha"]);
                sw.Write(";");
                //Finalizamos la linea del miembro:
                sw.WriteLine();
            }

            //Vaciamos el buffer
            sw.Flush();

            //Se lo devolvemos al usuario:
            string nombreFichero = pFilaEvento.Nombre.Replace(" ", "_") + ".csv";
            string contentType = "text/comma-separated-values";

            return File(escritor.ToArray(), contentType, nombreFichero);            
        }

        private AdministrarEventosViewModel.EventModel CargarEvento(ProyectoEvento filaEvento, ref bool hayInterno, GestionCMS gestorCMSConFiltros, Dictionary<Guid, int> eventoMiembros)
        {
            AdministrarEventosViewModel.EventModel evento = new AdministrarEventosViewModel.EventModel();

            evento.Key = filaEvento.EventoID;
            evento.Interno = false;
            if (filaEvento.Interno && !hayInterno)
            {
                evento.Interno = true;
                hayInterno = true;
            }

            evento.Name = UtilCadenas.ObtenerTextoDeIdioma(filaEvento.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            evento.Desciption = filaEvento.Descripcion;

            if (!filaEvento.Interno)
            {
                evento.Enlace = mControladorBase.UrlsSemanticas.GetURLAceptarInvitacionEventoEnProyecto(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, filaEvento.EventoID);
            }
            else
            {
                evento.Enlace = mControladorBase.UrlsSemanticas.GetURLHacerseMiembroComunidad(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto);
            }

            evento.Type = filaEvento.TipoEvento;

            string urlDescarga = mControladorBase.UrlsSemanticas.ObtenerURLAdministracionComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto, "COMADMINCOMEVENTOS") + "?eventoid=" + filaEvento.EventoID.ToString();
            evento.NumMembers = 0;
            if (eventoMiembros.ContainsKey(filaEvento.EventoID))
            {
                evento.NumMembers = eventoMiembros[filaEvento.EventoID];
            }

            evento.InfoExtra = string.Empty;

            if (!string.IsNullOrEmpty(filaEvento.InfoExtra))
            {
                evento.InfoExtra = filaEvento.InfoExtra;
            }

            evento.ComponenteCMS = Guid.Empty;

            if (!filaEvento.ComponenteID.HasValue && gestorCMSConFiltros.ListaComponentes.ContainsKey(filaEvento.ComponenteID.Value))
            {
                evento.ComponenteCMS = filaEvento.ComponenteID.Value;
            }

            evento.Active = filaEvento.Activo;
            evento.Group = filaEvento.Grupo;
            evento.UrlRedirect = filaEvento.UrlRedirect;

            return evento;
        }

        #endregion

        #region Propiedades

        private AdministrarEventosViewModel PaginaModel
        {
            get
            {
                if (mPaginaModel == null)
                {
                    mPaginaModel = new AdministrarEventosViewModel();

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerEventosProyectoPorProyectoID(ProyectoSeleccionado.Clave);
                    Dictionary<Guid, int> eventoMiembros = proyCN.ObtenerNumeroParticipantesEventosPorProyectoID(ProyectoSeleccionado.Clave);
                    proyCN.Dispose();

                    mPaginaModel.ListaComponentesCMS = new Dictionary<Guid, string>();

                    CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestionCMS gestorCMSConFiltros = new GestionCMS(CMSCN.ObtenerComponentesCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);

                    foreach (CMSComponente componente in gestorCMSConFiltros.ListaComponentes.Values)
                    {
                        mPaginaModel.ListaComponentesCMS.Add(componente.Clave, componente.Nombre);
                    }

                    mPaginaModel.ListaTiposEventos = new Dictionary<short, string>();
                    mPaginaModel.ListaTiposEventos.Add((short)TipoEventoProyecto.SinRestriccion, "Sin restricción");
                    mPaginaModel.ListaTiposEventos.Add((short)TipoEventoProyecto.NuevoEnComunidad, "Nuevos miembros en comunidad");
                    mPaginaModel.ListaTiposEventos.Add((short)TipoEventoProyecto.NuevoEnEcosistema, "Nuevos miembros en ecosistema");

                    bool hayInterno = false;

                    if (string.IsNullOrEmpty(RequestParams("eventoID")))
                    {
                        mPaginaModel.SelectedEvent = new AdministrarEventosViewModel.EventModel();

                        mPaginaModel.ListaEventos = new List<AdministrarEventosViewModel.EventModel>();

                        foreach (ProyectoEvento filaEvento in dataWrapperProyecto.ListaProyectoEvento)
                        {
                            AdministrarEventosViewModel.EventModel evento = CargarEvento(filaEvento, ref hayInterno, gestorCMSConFiltros, eventoMiembros);
                            mPaginaModel.ListaEventos.Add(evento);
                        }
                    }
                    else
                    {
                        ProyectoEvento filaEvento = dataWrapperProyecto.ListaProyectoEvento.FirstOrDefault(proy=>proy.EventoID.Equals(new Guid(RequestParams("eventoID"))));
                        mPaginaModel.SelectedEvent = CargarEvento(filaEvento, ref hayInterno, gestorCMSConFiltros, eventoMiembros);
                    }
                }
                return mPaginaModel;
            }
        }

        #endregion
    }
}