using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
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
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class CrearGrupoController : ControllerBaseWeb
    {
        private GrupoIdentidades mGrupo = null;
        private bool mEsGrupoOrganizacion = false;

        public CrearGrupoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(RequestParams("grupoOrg")) && RequestParams("grupoOrg") == "true")
            {
                EliminarPersonalizacionVistas();

                mEsGrupoOrganizacion = true;

                if (!IdentidadActual.TrabajaConOrganizacion || !EsIdentidadActualAdministradorOrganizacion)
                {
                    return GnossResultUrl(BaseURLIdioma + UrlPerfil + "home");
                }
            }
            else
            {
                if (!ProyectoSeleccionado.EsAdministradorUsuario(IdentidadActual.Persona.UsuarioID) && !(ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))
                {
                    return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "", ""));
                }
            }

            GroupEditViewModel paginaModel = new GroupEditViewModel();

            paginaModel.EsGrupoDeOrganizacion = mEsGrupoOrganizacion;

            if(Grupo != null)
            {
                paginaModel.Group = new GroupCardModel();
                paginaModel.Group.Name = Grupo.Nombre;
                paginaModel.Group.Description = Grupo.Descripcion;
                paginaModel.Group.ShortName = Grupo.NombreCorto;
                paginaModel.Group.Tags = Grupo.ListaTagsSoloLectura;

                if (EsGrupoOrganizacion)
                {
                    paginaModel.Group.UrlGroup = GnossUrlsSemanticas.GetUrlAdministrarOrg(BaseURLIdioma, UtilIdiomas, UrlPerfil, UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + Grupo.NombreCorto);
                }
                else
                {
                    paginaModel.Group.UrlGroup = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + Grupo.NombreCorto;
                }

                paginaModel.Participants = new Dictionary<Guid, string>();

                foreach (Identidad identidad in Grupo.Participantes.Values)
                {
                    paginaModel.Participants.Add(identidad.PerfilID, identidad.NombreCompuesto());
                }
            }
            else if (!string.IsNullOrEmpty(RequestParams("grupo")))
            {
                return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "", ""));
            }

            paginaModel.UrlSaveGroup = Request.Path.ToString() + "/SaveGroup";

            return View(paginaModel);
        }


        public ActionResult SaveGroup(string Titulo, string Descripcion, string Tags, string Participantes)
        {
            if (!string.IsNullOrEmpty(RequestParams("grupoOrg")) && RequestParams("grupoOrg") == "true")
            {
                mEsGrupoOrganizacion = true;

                if (!IdentidadActual.TrabajaConOrganizacion || !EsIdentidadActualAdministradorOrganizacion)
                {
                    return GnossResultUrl(BaseURLIdioma + UrlPerfil + "home");
                }
            }
            else
            {
                if (!ProyectoSeleccionado.EsAdministradorUsuario(IdentidadActual.Persona.UsuarioID) && !(ParametrosGeneralesRow.SupervisoresAdminGrupos && EsIdentidadActualSupervisorProyecto))
                {
                    return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "", ""));
                }
            }


            // Es posible que la lista de participantes en el momento de la creación del grupo sea null porque no se deseen añadir en primera instancia. Tenerlo en cuenta para evitar errores            
            string[] listaParticipantes = Array.Empty<string>();
            if (Participantes != null)
            {
                listaParticipantes = Participantes.Split(new string[] { "[-|-]" }, StringSplitOptions.RemoveEmptyEntries);
            }


            //existe y diferente q editamos
            Guid idGrupo = Guid.Empty;
            if (Grupo != null)
            {
                idGrupo = Grupo.Clave;
            }
            if (ExisteNombreGrupo(Titulo, idGrupo) && (Grupo == null || Grupo.Nombre != Titulo))
            {
                //jsonText += "document.getElementById('" + txtTitulo.ClientID + "').attributes['class'].value += ' error'; OcultarUpdateProgress()";
                return GnossResultERROR("ERROR Titulo");
            }

            List<Guid> listaPerfiles = new List<Guid>();
            foreach (string participante in listaParticipantes)
            {
                Guid perfilID = Guid.Empty;
                if (Guid.TryParse(participante, out perfilID))
                {
                    listaPerfiles.Add(perfilID);
                }
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Guid> listaIdentidades = identidadCN.ObtenerIdentidadesIDDePerfilEnProyecto(ProyectoSeleccionado.Clave, listaPerfiles);

            string descripcionDescodificada = HttpUtility.UrlDecode(Descripcion);

            if (Grupo != null)
            {
                EditarGrupo(Titulo.Trim(), descripcionDescodificada, Tags, listaIdentidades);
            }
            else if (string.IsNullOrEmpty(RequestParams("grupo")))
            {
                CrearGrupo(Titulo.Trim(), descripcionDescodificada, Tags, listaIdentidades);
            }
            else
            {
                return GnossResultUrl(mControladorBase.UrlsSemanticas.ObtenerURLPersonasYOrganizaciones(BaseURLIdioma, UtilIdiomas, ProyectoSeleccionado.NombreCorto, "", ""));
            }

            return GnossResultUrl(UrlFichaGrupo());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTitulo"></param>
        /// <param name="pDescripcion"></param>
        /// <param name="pTags"></param>
        /// <param name="pEsPublico"></param>
        /// <param name="pListaParticipantes"></param>
        private void CrearGrupo(string pTitulo, string pDescripcion, string pTags, List<Guid> pListaParticipantes)
        {
            string nombreCortoGrupo = UtilCadenas.EliminarCaracteresUrlSem(pTitulo);

            Guid grupoID = Guid.NewGuid();

            if (nombreCortoGrupo.Length > 12)
            {
                nombreCortoGrupo = nombreCortoGrupo.Substring(0, 12);
            }

            int hashNumUsu = 1;
            while (ExisteNombreCortoGrupo(nombreCortoGrupo))
            {
                nombreCortoGrupo = nombreCortoGrupo.Substring(0, nombreCortoGrupo.Length - hashNumUsu.ToString().Length) + hashNumUsu.ToString();
                hashNumUsu++;
            }

            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();

            AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupoIdentidades = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidades();
            filaGrupoIdentidades.GrupoID = grupoID;
            filaGrupoIdentidades.Nombre = pTitulo;
            filaGrupoIdentidades.NombreCorto = nombreCortoGrupo;
            filaGrupoIdentidades.Descripcion = pDescripcion;
            filaGrupoIdentidades.Tags = pTags;
            filaGrupoIdentidades.Publico = true;
            filaGrupoIdentidades.FechaAlta = DateTime.Now;
            filaGrupoIdentidades.PermitirEnviarMensajes = true;

            identidadDW.ListaGrupoIdentidades.Add(filaGrupoIdentidades);
            mEntityContext.GrupoIdentidades.Add(filaGrupoIdentidades);
            if (EsGrupoOrganizacion)
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesOrganizacion filaGrupoIdentidadesOrganizacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesOrganizacion();
                filaGrupoIdentidadesOrganizacion.GrupoID = grupoID;
                filaGrupoIdentidadesOrganizacion.OrganizacionID = IdentidadActual.OrganizacionID.Value;

                identidadDW.ListaGrupoIdentidadesOrganizacion.Add(filaGrupoIdentidadesOrganizacion);
                filaGrupoIdentidades.GrupoIdentidadesOrganizacion.Add(filaGrupoIdentidadesOrganizacion);
                filaGrupoIdentidadesOrganizacion.GrupoIdentidades = filaGrupoIdentidades;
                mEntityContext.GrupoIdentidadesOrganizacion.Add(filaGrupoIdentidadesOrganizacion);
            }
            else
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesProyecto filaGrupoIdentidadesProyecto = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesProyecto();
                filaGrupoIdentidadesProyecto.GrupoID = grupoID;
                filaGrupoIdentidadesProyecto.ProyectoID = ProyectoSeleccionado.Clave;
                filaGrupoIdentidadesProyecto.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;

                identidadDW.ListaGrupoIdentidadesProyecto.Add(filaGrupoIdentidadesProyecto);
                filaGrupoIdentidadesProyecto.GrupoIdentidades = filaGrupoIdentidades;
                mEntityContext.GrupoIdentidadesProyecto.Add(filaGrupoIdentidadesProyecto);
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> listaNombresParticipantes = identidadCN.ObtenerNombresDeIdentidades(pListaParticipantes);
            
            foreach (Guid identidad in pListaParticipantes)
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();

                filaGrupoIdentidadesParticipacion.GrupoID = grupoID;
                filaGrupoIdentidadesParticipacion.IdentidadID = identidad;
                filaGrupoIdentidadesParticipacion.FechaAlta = filaGrupoIdentidades.FechaAlta;

                identidadDW.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
                filaGrupoIdentidades.GrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
                filaGrupoIdentidadesParticipacion.GrupoIdentidades = filaGrupoIdentidades;
                mEntityContext.GrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
            }

            identidadCN.ActualizaIdentidades();
            identidadCN.Dispose();

            EnviarMensajeMiembros(pListaParticipantes, pTitulo, nombreCortoGrupo, false);

            mGrupo = new GrupoIdentidades(filaGrupoIdentidades, IdentidadActual.GestorIdentidades, mLoggingService);

            ControladorGrupos.ActualizarBase(ProyectoSeleccionado.Clave, Grupo.Clave);

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (EsGrupoOrganizacion)
            {
                identidadCL.InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(IdentidadActual.OrganizacionID.Value);
                identidadCL.InvalidarCacheGrupoPorNombreCortoYOrganizacion(Grupo.NombreCorto, IdentidadActual.OrganizacionID.Value);
            }
            else
            {
                identidadCL.InvalidarCacheMiembrosComunidad(ProyectoSeleccionado.Clave);

                identidadCL.InvalidarCacheGrupoPorNombreCortoYProyecto(Grupo.NombreCorto, ProyectoSeleccionado.Clave);
            }
            identidadCL.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTitulo"></param>
        /// <param name="pDescripcion"></param>
        /// <param name="pTags"></param>
        /// <param name="pEsPublico"></param>
        /// <param name="pListaParticipantes"></param>
        private void EditarGrupo(string pTitulo, string pDescripcion, string pTags, List<Guid> pListaParticipantes)
        {
            string nombreViejoGrupo = Grupo.FilaGrupoIdentidades.Nombre;
            Grupo.FilaGrupoIdentidades.Nombre = pTitulo;
            Grupo.FilaGrupoIdentidades.Descripcion = pDescripcion;
            Grupo.FilaGrupoIdentidades.Tags = pTags;
            Grupo.FilaGrupoIdentidades.Publico = true;

            List<Guid> listaParticipantesEliminados = new List<Guid>();
            
            foreach (Guid identidad in Grupo.Participantes.Keys)
            {
                if (!pListaParticipantes.Contains(identidad))
                {
                    AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion grupoIdentidadesParticipacion = Grupo.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Where(item => item.GrupoID.Equals(Grupo.Clave) && item.IdentidadID.Equals(identidad)).FirstOrDefault();

                    mEntityContext.EliminarElemento(grupoIdentidadesParticipacion);
                    Grupo.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Remove(grupoIdentidadesParticipacion);

                    listaParticipantesEliminados.Add(identidad);
                }
            }

            List<Guid> listaParticipantesNuevos = new List<Guid>();

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> listaNombresParticipantes = identidadCN.ObtenerNombresDeIdentidades(pListaParticipantes);
            
            foreach (Guid identidad in pListaParticipantes)
            {
                if (!Grupo.Participantes.ContainsKey(identidad))
                {
                    AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();

                    filaGrupoIdentidadesParticipacion.GrupoID = Grupo.Clave;
                    filaGrupoIdentidadesParticipacion.IdentidadID = identidad;
                    filaGrupoIdentidadesParticipacion.FechaAlta = DateTime.Now;

                    Grupo.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
                    mEntityContext.GrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);

                    listaParticipantesNuevos.Add(identidad);
                }
            }

            identidadCN.ActualizaIdentidades();
            identidadCN.Dispose();

            EnviarMensajeMiembros(listaParticipantesEliminados, pTitulo, Grupo.NombreCorto, true);
            EnviarMensajeMiembros(listaParticipantesNuevos, pTitulo, Grupo.NombreCorto, false);

            ControladorGrupos.ActualizarBase(ProyectoSeleccionado.Clave, Grupo.Clave, nombreViejoGrupo);
            
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (EsGrupoOrganizacion)
            {
                identidadCL.InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(IdentidadActual.OrganizacionID.Value);
                identidadCL.InvalidarCacheGrupoPorNombreCortoYOrganizacion(Grupo.NombreCorto, IdentidadActual.OrganizacionID.Value);
            }
            else
            {
                identidadCL.InvalidarCacheMiembrosComunidad(ProyectoSeleccionado.Clave);

                identidadCL.InvalidarCacheGrupoPorNombreCortoYProyecto(Grupo.NombreCorto, ProyectoSeleccionado.Clave);
            }
            identidadCL.Dispose();
        }

        /// <summary>
        /// Envia un mensaje a todos los participantes del grupo que se han añadido
        /// </summary>
        public void EnviarMensajeMiembros(List<Guid> pListaIdentidades, string pNombreGrupo, string pNombreCortoGrupo, bool pEsMensajeEliminacion)
        {
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();

            ObtenerIdentidadesPorID(pListaIdentidades, dataWrapperIdentidad, dataWrapperPersona, null);

            dataWrapperIdentidad.Merge(IdentidadActual.GestorIdentidades.DataWrapperIdentidad);

            GestionPersonas gestPers = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
            GestionIdentidades gestIdent = new GestionIdentidades(dataWrapperIdentidad, gestPers, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            GestionCorreo gestionCorreo = new GestionCorreo(new CorreoDS(), gestPers, gestIdent, IdentidadActual.GestorAmigos, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestionCorreo.GestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<Guid> listaTodosDestinatarios = new List<Guid>();

            foreach (Guid identidadID in pListaIdentidades)
            {
                Identidad ident = gestIdent.ListaIdentidades[identidadID];

                if (ident.Clave != IdentidadActual.Clave)
                {
                    UtilIdiomas utilIdiomasPersona = new UtilIdiomas(ident.Persona.FilaPersona.Idioma, ProyectoSeleccionado.Clave, Guid.Empty, Guid.Empty, mLoggingService, mEntityContext, mConfigService);

                    string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);

                    string urlGrupo = urlComunidad + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + pNombreCortoGrupo;

                    string asunto = "";
                    string mensaje = "";

                    if (pEsMensajeEliminacion)
                    {
                        asunto = utilIdiomasPersona.GetText("GRUPO", "ASUNTOELIMINACIONGRUPO", pNombreGrupo);
                        if (EsGrupoOrganizacion)
                        {
                            mensaje = utilIdiomasPersona.GetText("GRUPO", "MENSAJEELIMINACIONGRUPOORGANIZACION", ident.Persona.Nombre, pNombreGrupo, urlGrupo, IdentidadActual.OrganizacionPerfil.Nombre, urlComunidad);
                        }
                        else
                        {
                            mensaje = utilIdiomasPersona.GetText("GRUPO", "MENSAJEELIMINACIONGRUPOCOMUNIDAD", ident.Persona.Nombre, pNombreGrupo, urlGrupo, ProyectoSeleccionado.Nombre, urlComunidad);
                        }
                    }
                    else
                    {
                        asunto = utilIdiomasPersona.GetText("GRUPO", "ASUNTOASIGNACIONGRUPO", pNombreGrupo);

                        if (EsGrupoOrganizacion)
                        {
                            mensaje = utilIdiomasPersona.GetText("GRUPO", "MENSAJEASIGNACIONGRUPOORGANIZACION", ident.Persona.Nombre, pNombreGrupo, urlGrupo, IdentidadActual.OrganizacionPerfil.Nombre, urlComunidad);
                        }
                        else
                        {
                            mensaje = utilIdiomasPersona.GetText("GRUPO", "MENSAJEASIGNACIONGRUPOCOMUNIDAD", ident.Persona.Nombre, pNombreGrupo, urlGrupo, ProyectoSeleccionado.Nombre, urlComunidad);
                        }

                    }
                    //1 -> NombrePersona 2 -> Nombre del grupo 3 -> url del Grupo 4 -> Nombre de la comunidad 5 -> url de la comunidad

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                    List<Guid> lista = new List<Guid>();
                    lista.Add(identidadCN.ObtenerIdentidadIDDeMyGNOSSPorIdentidad(ident.Clave));
                    identidadCN.Dispose();

                    foreach (Guid destinatario in lista)
                    {
                        if (!listaTodosDestinatarios.Contains(destinatario))
                        {
                            listaTodosDestinatarios.Add(destinatario);
                        }
                    }

                    if (!EsEcosistemaSinMetaProyecto)
                    {
                        gestionCorreo.AgregarCorreo(IdentidadActual.Clave, lista, asunto, mensaje, BaseURL, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                    }
                    else
                    {
                        gestionCorreo.AgregarCorreo(IdentidadActual.Clave, lista, asunto, mensaje, BaseURL, ProyectoVirtual, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                    }

                    ////Pongo el correo como eliminado para que no le aparezca a la persona que envia el correo en su bandeja de salida:
                    //correo.FilaCorreo.Eliminado = true;
                    //correo.FilaCorreo.EnPapelera = true;
                }
            }
            CorreoCN actualizarCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            actualizarCN.ActualizarCorreo(gestionCorreo.CorreoDS);

            //ControladorCorreo.AgregarNotificacionCorreoNuevoAIdentidades(listaTodosDestinatarios);
        }

        private string UrlFichaGrupo()
        {
            string urlRedirect = "";
            if (EsGrupoOrganizacion == true)
            {
                urlRedirect = GnossUrlsSemanticas.GetUrlAdministrarOrg(BaseURLIdioma, UtilIdiomas, UrlPerfil, UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + Grupo.NombreCorto);
            }
            else
            {
                urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + Grupo.NombreCorto;
            }

            return urlRedirect;
        }

        /// <summary>
        /// Verdad si existe el Nombre del grupo
        /// </summary>
        /// <param name="pNombreGrupo">Nombre del nuevo grupo</param>
        /// <param name="pGrupoID">ID del grupo que se está modificando</param>
        /// <returns></returns>
        private bool ExisteNombreGrupo(string pNombreGrupo, Guid pGrupoID)
        {
            bool existe = true;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (EsGrupoOrganizacion)
            {
                existe = identidadCN.ExisteGrupoEnOrganizacionPorNombre(pNombreGrupo, pGrupoID, IdentidadActual.OrganizacionID.Value);
            }
            else
            {
                existe = identidadCN.ExisteGrupoEnProyectoPorNombre(pNombreGrupo, pGrupoID, ProyectoSeleccionado.Clave);
            }
            identidadCN.Dispose();

            return existe;
        }

        /// <summary>
        /// Verdad si existe el NombreCorto del grupo
        /// </summary>
        /// <returns></returns>
        private bool ExisteNombreCortoGrupo(string pNombreCortoGrupo)
        {
            bool existe = true;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (EsGrupoOrganizacion)
            {
                existe = identidadCN.ExisteGrupoEnOrganizacionPorNombreCorto(pNombreCortoGrupo, IdentidadActual.OrganizacionID.Value);
            }
            else
            {
                existe = identidadCN.ExisteGrupoEnProyectoPorNombreCorto(pNombreCortoGrupo, ProyectoSeleccionado.Clave);
            }
            identidadCN.Dispose();

            return existe;
        }


        public bool EsGrupoOrganizacion
        {
            get { return mEsGrupoOrganizacion; }
            set { mEsGrupoOrganizacion = value; }
        }

        public GrupoIdentidades Grupo
        {
            get
            {
                if (mGrupo == null)
                {
                    if (!string.IsNullOrEmpty(RequestParams("grupo")))
                    {
                        string nombreCortoGrupo = HttpUtility.UrlDecode(RequestParams("grupo"));

                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        GestionIdentidades gestorIdentidades = null;

                        if (EsGrupoOrganizacion)
                        {
                            gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGrupoPorNombreCortoYOrganizacion(nombreCortoGrupo, IdentidadActual.OrganizacionID.Value), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                        }
                        else
                        {
                            gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGrupoPorNombreCortoYProyecto(nombreCortoGrupo, ProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                        }
                        identidadCN.Dispose();

                        if (gestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades.Count > 0)
                        {
                            Guid grupoID = gestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades[0].GrupoID;
                            mGrupo = gestorIdentidades.ListaGrupos[grupoID];
                        }
                        else
                        {
                            //RedirigirFueraPagina();
                        }
                    }
                }
                return mGrupo;
            }
        }
    }
}
