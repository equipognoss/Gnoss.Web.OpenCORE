using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Comentario;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Comentario;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Voto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL;
using Es.Riam.Gnoss.Web.MVC.Controles;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.ControlesMVC
{
    public class AccionesRecurso
    {
        private ControllerBaseWeb ControllerBase;

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private VirtuosoAD mVirtuosoAD;
        private IHttpContextAccessor mHttpContextAccessor;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;
        private ControladorBase mControladorBase;
        private ControladorDocumentacion mControladorDocumentacion;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pController">Controller</param>
        public AccionesRecurso(ControllerBaseWeb pController, EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, GnossCache gnossCache, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AccionesRecurso> logger, ILoggerFactory loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            ControllerBase = pController;
            mVirtuosoAD = virtuosoAD;
            mHttpContextAccessor = httpContextAccessor;
            mGnossCache = gnossCache;
            mEntityContextBASE = entityContextBASE;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mLoggerFactory = loggerFactory;
            mlogger = logger;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);
        }

        protected ControladorDocumentacion ControladorDocumentacion
        {
            get
            {
                if(mControladorDocumentacion == null)
                {
                    mControladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
                }
                return mControladorDocumentacion;
            }
        }

        public void CargarAccionesRecursos(MultiViewResult result, Dictionary<Guid, List<Guid>> pListaDocumentosProyecto)
        {
            List<Guid> listaProyectos = new List<Guid>();
            foreach (Guid proyectoID in pListaDocumentosProyecto.Keys)
            {
                listaProyectos.Add(proyectoID);
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataSet datosProyectoDesplegarAcciones = proyCN.ObtenerDatosProyectosDesplegarAcciones(listaProyectos);
            proyCN.Dispose();

            Dictionary<Guid, bool> listaVotacionesDisponiblesProyectos = new Dictionary<Guid, bool>();
            //Dictionary<Guid, bool> listaCompartirPermitidoProyectos = new Dictionary<Guid, bool>();

            //Cargamos el gestor documental con los datos necesarios de los recursos
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //cargamos la base de recursos del proyecto actual
            docCN.ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, ControllerBase.ProyectoSeleccionado.Clave);

            foreach (Guid proyectoID in pListaDocumentosProyecto.Keys)
            {
                dataWrapperDocumentacion.Merge(docCN.ObtenerDocumentosPorIDParaListadoDeAcciones(pListaDocumentosProyecto[proyectoID], proyectoID, ControllerBase.IdentidadActual.Persona.UsuarioID));

                DataRow filaDatosDesplegarAcciones = datosProyectoDesplegarAcciones.Tables["DatosDesplegarAcciones"].Select("ProyectoID = '" + proyectoID + "'")[0];

                listaVotacionesDisponiblesProyectos.Add(proyectoID, (bool)filaDatosDesplegarAcciones["VotacionesDisponibles"]);
            }

            docCN.Dispose();
            GestorDocumental gestorDoc = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);

            if (ControllerBase != null && ControllerBase.IdentidadActual != null)
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                ControllerBase.IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerGruposParticipaIdentidad(ControllerBase.IdentidadActual.Clave, false));
                ControllerBase.IdentidadActual.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerGruposParticipaIdentidad(ControllerBase.IdentidadActual.IdentidadMyGNOSS.Clave, false));

                gestorDoc.GestorIdentidades = ControllerBase.IdentidadActual.GestorIdentidades;
            }

            List<Guid> listaDocumentosVotaciones = new List<Guid>();

            foreach (Guid proyectoID in pListaDocumentosProyecto.Keys)
            {
                if (listaVotacionesDisponiblesProyectos[proyectoID])
                {
                    listaDocumentosVotaciones.AddRange(pListaDocumentosProyecto[proyectoID]);
                }
            }

            //Cargamos el gestor de votos
            if (listaDocumentosVotaciones.Count > 0)
            {
                VotoCN votoCN = new VotoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
                gestorDoc.GestorVotos = new GestionVotosDocumento(votoCN.ObtenerVotosDocumentosPorID(listaDocumentosVotaciones), gestorDoc, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionVotosDocumento>(), mLoggerFactory);
                votoCN.Dispose();
                gestorDoc.GestorVotos.RecargarVotos();
            }

            Guid idBaseRecursosUsuario = Guid.Empty;

            if (gestorDoc.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count > 0)
            {
                idBaseRecursosUsuario = gestorDoc.DataWrapperDocumentacion.ListaBaseRecursosUsuario[0].BaseRecursosID;
            }

            Dictionary<Guid, Guid> listaBasesRecursoProyecto = new Dictionary<Guid, Guid>();

            SuscripcionCN suscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
            List<Guid> suscripcionPorIdentidad = new List<Guid>();

            foreach (Guid proyectoID in pListaDocumentosProyecto.Keys)
            {
                List<Guid> listaIdentidadesAutores = new List<Guid>();
                List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBaseRecursosProyecto = gestorDoc.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRec => baseRec.ProyectoID.Equals(proyectoID)).ToList();
                Guid idBaseRecursosProyecto = idBaseRecursosUsuario;

                if (filasBaseRecursosProyecto.Count > 0)
                {
                    idBaseRecursosProyecto = filasBaseRecursosProyecto[0].BaseRecursosID;
                }

                listaBasesRecursoProyecto.Add(proyectoID, idBaseRecursosProyecto);

                foreach (Guid documento in pListaDocumentosProyecto[proyectoID])
                {
                    if (gestorDoc.ListaDocumentos.ContainsKey(documento))
                    {
                        Documento doc = gestorDoc.ListaDocumentos[documento];
                        if (doc.IdentidadCompardidoPorBaseRecursos.ContainsKey(idBaseRecursosProyecto))
                        {
                            Guid identidadAutor = doc.IdentidadCompardidoPorBaseRecursos[idBaseRecursosProyecto].IdentidadPublicacionID.Value;
                            if (!listaIdentidadesAutores.Contains(identidadAutor))
                            {
                                listaIdentidadesAutores.Add(identidadAutor);
                            }
                        }
                    }
                }

                if (!ControllerBase.UsuarioActual.EsIdentidadInvitada && listaIdentidadesAutores.Count > 0)
                {
                    suscripcionPorIdentidad.AddRange(suscripcionCN.ObtenerListaIdentidadesSuscritasPorProyecto(ControllerBase.IdentidadActual.IdentidadMyGNOSS.Clave, listaIdentidadesAutores, proyectoID));
                }
            }
            suscripcionCN.Dispose();


            //Diccionario con clave Proyecto y valor las ontologías del proyecto con un booleano para indicar si tienen los votos activos
            Dictionary<Guid, Dictionary<Guid, bool>> dicProyOntVotos = new Dictionary<Guid, Dictionary<Guid, bool>>();

            Dictionary<Guid, Proyecto> listaProyectosDoc = new Dictionary<Guid, Proyecto>();
            Dictionary<Guid, ParametroGeneral> listaParametrosGeneralesDoc = new Dictionary<Guid, ParametroGeneral>();

            foreach (Guid proyectoID in pListaDocumentosProyecto.Keys)
            {
                Proyecto proy = ControllerBase.ProyectoVirtual;
                ParametroGeneral parametrosGenerales = ControllerBase.ParametrosGeneralesVirtualRow;
                if (!ControllerBase.ProyectoVirtual.Clave.Equals(proyectoID))
                {
                    if (listaProyectosDoc.ContainsKey(proyectoID))
                    {
                        proy = listaProyectosDoc[proyectoID];
                        parametrosGenerales = listaParametrosGeneralesDoc[proyectoID];
                    }
                    else
                    {
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                        GestionProyecto gestProy = new GestionProyecto(proyCL.ObtenerProyectoPorID(proyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                        proy = gestProy.ListaProyectos[proyectoID];
                        listaProyectosDoc.Add(proyectoID, proy);

                        ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCL>(), mLoggerFactory);
                        listaParametrosGeneralesDoc.Add(proyectoID, paramCL.ObtenerParametrosGeneralesDeProyecto(proyectoID).ListaParametroGeneral[0]);
                    }
                }

                Identidad identidadEnProy = ControllerBase.IdentidadActual;
                Guid identidadID = ControllerBase.IdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad(proyectoID);
                if (identidadID != Guid.Empty && ControllerBase.IdentidadActual.GestorIdentidades.ListaIdentidades.ContainsKey(identidadID))
                {
                    identidadEnProy = ControllerBase.IdentidadActual.GestorIdentidades.ListaIdentidades[ControllerBase.IdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad(proyectoID)];
                }

                ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(ControllerBase.UtilIdiomas, ControllerBase.BaseURL, ControllerBase.BaseURLContent, ControllerBase.BaseURLStatic, proy, parametrosGenerales, identidadEnProy, ControllerBase.EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);

                List<Guid> listaIdentidades = gestorDoc.ListaDocumentos.Values.Where(doc => pListaDocumentosProyecto[proyectoID].Contains(doc.Clave) && doc.IdentidadCompardido != null && doc.IdentidadCompardido.IdentidadPublicacionID.HasValue).Select(doc2 =>  doc2.IdentidadCompardido.IdentidadPublicacionID.Value).Distinct().ToList();
                
                Dictionary<Guid, ProfileModel> identidadesPublicadores = controladorMVC.ObtenerIdentidadesPorID(listaIdentidades, false);

                foreach (Guid documentoID in pListaDocumentosProyecto[proyectoID])
                {
                    if (gestorDoc.ListaDocumentos.ContainsKey(documentoID))
                    {
                        Documento docActual = gestorDoc.ListaDocumentos[documentoID];

                        if(docActual.IdentidadCompardido == null) { continue; }

                        ResourceModel fichaRecurso = new ResourceModel();
                        fichaRecurso.Key = documentoID;

                        Identidad identidadAccion = ControllerBase.IdentidadActual;

                        if (proyectoID != ControllerBase.ProyectoSeleccionado.Clave)
                        {
                            List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidades = identidadAccion.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(proyectoID)).ToList();
                            if (filasIdentidades.Count > 0)
                            {
                                identidadAccion = identidadAccion.GestorIdentidades.ListaIdentidades[filasIdentidades.First().IdentidadID];
                            }
                        }

                        Guid idBaseRecursosProyecto = Guid.Empty;
                        if (listaBasesRecursoProyecto.ContainsKey(proyectoID))
                        {
                            idBaseRecursosProyecto = listaBasesRecursoProyecto[proyectoID];
                        }

                        DataRow filaDatosDesplegarAcciones = datosProyectoDesplegarAcciones.Tables["DatosDesplegarAcciones"].Select("ProyectoID = '" + proyectoID + "'")[0];
                        string nombreCorto = filaDatosDesplegarAcciones["NombreCorto"].ToString();
                        bool votacionesDisponibles = (bool)filaDatosDesplegarAcciones["VotacionesDisponibles"];
                        bool permitirVotacionesNegativas = (bool)filaDatosDesplegarAcciones["PermitirVotacionesNegativas"];
                        bool comentariosDisponibles = (bool)filaDatosDesplegarAcciones["ComentariosDisponibles"];
                        bool compartirRecursosPermitido = (bool)filaDatosDesplegarAcciones["CompartirRecursosPermitido"];

                        fichaRecurso.CompletCardLink = ControllerBase.UrlsSemanticas.GetURLBaseRecursosFicha(ControllerBase.BaseURLIdioma, ControllerBase.UtilIdiomas, ControllerBase.ProyectoSeleccionado.NombreCorto, ControllerBase.UrlPerfil, docActual, false);

                        //establecemos las urls de las acciones del recurso
                        ControllerBase.ControladorProyectoMVC.EstablecerUrlAccionesReecurso(fichaRecurso, ControllerBase.ProyectoSeleccionado.NombreCorto);

                        if (docActual.IdentidadCompardidoPorBaseRecursos.ContainsKey(idBaseRecursosProyecto) && (ControllerBase.UsuarioActual.EsIdentidadInvitada || !suscripcionPorIdentidad.Contains(docActual.IdentidadCompardidoPorBaseRecursos[idBaseRecursosProyecto].IdentidadPublicacionID.Value)) && docActual.IdentidadCompardidoPorBaseRecursos[idBaseRecursosProyecto].IdentidadPublicacionID != identidadAccion.Clave && docActual.IdentidadCompardidoPorBaseRecursos[idBaseRecursosProyecto].IdentidadPublicacionID.HasValue)
                        {
                            fichaRecurso.Publisher = identidadesPublicadores[docActual.IdentidadCompardidoPorBaseRecursos[idBaseRecursosProyecto].IdentidadPublicacionID.Value];
                            
                            if (fichaRecurso.Publisher.ListActions == null)
                            {
                                fichaRecurso.Publisher.ListActions = new ProfileModel.UrlActions();
                            }
                            fichaRecurso.Publisher.ListActions.UrlFollow = fichaRecurso.Publisher.UrlPerson + "/follow";
                            fichaRecurso.Publisher.ListActions.UrlUnfollow = fichaRecurso.Publisher.UrlPerson + "/unfollow";
                        }


                        Documento doc = gestorDoc.ListaDocumentos[documentoID];
                        bool votacionesDisponiblesOntologia = true;
                        if (!doc.ElementoVinculadoID.Equals(Guid.Empty))
                        {
                            if (dicProyOntVotos.ContainsKey(proyectoID) && dicProyOntVotos[proyectoID].ContainsKey(doc.ElementoVinculadoID))
                            {
                                votacionesDisponiblesOntologia = dicProyOntVotos[proyectoID][doc.ElementoVinculadoID];
                            }
                            else
                            {
                                if (!dicProyOntVotos.ContainsKey(proyectoID))
                                {
                                    dicProyOntVotos.Add(proyectoID, new Dictionary<Guid, bool>());
                                }
                                //Comprobamos si las votaciones están disponibles para la ontología                        
                                Dictionary<string, List<EstiloPlantilla>> listaEstilos = null;
                                ControllerBase.ControladorDocumentacion.CargarEstilosOntologia(doc.ElementoVinculadoID, out listaEstilos, proyectoID);
                                votacionesDisponiblesOntologia = !((EstiloPlantillaConfigGen)listaEstilos["[" + LectorXmlConfig.NodoConfigGen + "]"][0]).OcultarVotosDoc;
                                dicProyOntVotos[proyectoID].Add(doc.ElementoVinculadoID, votacionesDisponiblesOntologia);
                            }
                        }

                        if (listaVotacionesDisponiblesProyectos[proyectoID] && votacionesDisponibles && votacionesDisponiblesOntologia)
                        {
                            bool heVotadoPositivo = false;
                            bool heVotadoNegativo = false;

                            int numVotosPositivos = 0;
                            int numVotosNegativos = 0;

                            foreach (AD.EntityModel.Models.Voto.Voto votoFor in docActual.ListaVotosPorComunidad(proyectoID).Values)
                            {
                                if (votoFor.Voto1 > 0)
                                {
                                    numVotosPositivos++;
                                    if (identidadAccion.Clave == votoFor.IdentidadID)
                                    {
                                        //Ya ha votado el recurso positivamente                                
                                        heVotadoPositivo = true;
                                    }
                                }

                                if (votoFor.Voto1 < 0)
                                {
                                    numVotosNegativos++;
                                    if (identidadAccion.Clave == votoFor.IdentidadID)
                                    {
                                        //Ya ha votado el recurso negativamente
                                        heVotadoNegativo = true;
                                    }
                                }
                            }

                            bool esPropioAutor = false;
                            if (ControllerBase.IdentidadActual.ListaTodosIdentidadesDeIdentidad.Contains(docActual.CreadorID))
                            {
                                esPropioAutor = true;
                            }

                            VotesModel fichaVotos = new VotesModel();
                            fichaVotos.NumPositiveVotes = numVotosPositivos;
                            fichaVotos.NumNegativeVotes = numVotosNegativos;
                            fichaVotos.IsVotedPositive = heVotadoPositivo;
                            fichaVotos.IsVotedNegative = heVotadoNegativo;
                            fichaVotos.IsOwnedAuthor = esPropioAutor;
                            fichaVotos.AllowNegativeVotes = permitirVotacionesNegativas;

                            string urlDocumento = fichaRecurso.CompletCardLink;

                            fichaVotos.UrlVotePositive = urlDocumento + "/vote-positive";
                            fichaVotos.UrlVoteNegative = urlDocumento + "/vote-negative";
                            fichaVotos.UrlDeleteVote = urlDocumento + "/delete-vote";

                            fichaRecurso.Votes = fichaVotos;
                        }

                        fichaRecurso.AllowComments = false;
                        if (!docActual.EsBorrador && comentariosDisponibles)
                        {
                            fichaRecurso.AllowComments = true;
							ComentarioCN comentCN = new ComentarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ComentarioCN>(), mLoggerFactory);
							DataWrapperComentario comentarioDW = comentCN.ObtenerComentariosDeDocumento(docActual.VersionOriginalID, ControllerBase.ProyectoSeleccionado.Clave);
							comentCN.Dispose();
							fichaRecurso.NumComments = comentarioDW.ListaComentario.Count;
                        }

                        // Agregar a mi espacio personal

                        bool compartirPermitido = docActual.CompartirPermitido && !docActual.EsBorrador && compartirRecursosPermitido && docActual.FilaDocumento.UltimaVersion;

                        if (compartirPermitido && !docActual.IdentidadCompardidoPorBaseRecursos.ContainsKey(idBaseRecursosUsuario) && docActual.TipoDocumentacion != TiposDocumentacion.Debate && docActual.TipoDocumentacion != TiposDocumentacion.Pregunta && docActual.TipoDocumentacion != TiposDocumentacion.Newsletter && docActual.TipoDocumentacion != TiposDocumentacion.Encuesta)
                        {
                            fichaRecurso.Actions = new ResourceModel.ActionsModel();
                            fichaRecurso.Actions.AddToMyPersonalSpace = true;
                        }

                        ComprobarPermisosEdicion(docActual, fichaRecurso);

                        result.AddView("ControlesMVC/_AccionesRecurso", "AccionesRecursoListado_" + documentoID + "_" + proyectoID, fichaRecurso);

                    }
                }
            }
        }

        private void ComprobarPermisosEdicion(Documento pDocumento, ResourceModel pFichaRecurso)
        {
            SemCmsController genPlantillasOWL = null;

            if (pDocumento != null && pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico)
            {
                genPlantillasOWL = new SemCmsController(new SemanticResourceModel(), new Ontologia(), pDocumento, null, ControllerBase.ProyectoSeleccionado, ControllerBase.IdentidadActual, ControllerBase.UtilIdiomas, ControllerBase.BaseURL, ControllerBase.BaseURLIdioma, ControllerBase.BaseURLContent, ControllerBase.BaseURLStatic, ControllerBase.UrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SemCmsController>(), mLoggerFactory).ObtenerControladorSemCMS(pDocumento, ControllerBase.ProyectoSeleccionado, ControllerBase.IdentidadActual, ControllerBase.BaseURLFormulariosSem, ControllerBase.UtilIdiomas, ControllerBase.BaseURL, ControllerBase.BaseURLIdioma, ControllerBase.BaseURLContent, ControllerBase.BaseURLStatic, ControllerBase.UrlIntragnoss, false, ControllerBase.ParametroProyecto, ControllerBase.Request.Query["paramsemcms"]);
            }

            bool bloqueadoPorOtroUsuario = pDocumento.FilaDocumento.IdentidadProteccionID.HasValue && !pDocumento.FilaDocumento.IdentidadProteccionID.Equals(mControladorBase.UsuarioActual.IdentidadID);

            bool esIdentidadBrOrg = !string.IsNullOrEmpty(ControllerBase.RequestParams("organizacion"));
            Identidad identidadBrOrg = null;
            if (esIdentidadBrOrg)
            {
                identidadBrOrg = ControllerBase.IdentidadActual.IdentidadOrganizacion;
            }

            if(pFichaRecurso.Actions == null)
            {
                pFichaRecurso.Actions = new ResourceModel.ActionsModel();
            }

            pFichaRecurso.Actions.Edit = TienePermisosEditarDoc(pDocumento, ControllerBase.IdentidadActual, identidadBrOrg, ControllerBase.ProyectoSeleccionado, genPlantillasOWL, bloqueadoPorOtroUsuario);

            pFichaRecurso.Actions.Delete = true;

            if (!(pDocumento.FilaDocumento.UltimaVersion && (!pDocumento.FilaDocumento.Protegido || !bloqueadoPorOtroUsuario || ControllerBase.EsIdentidadActualSupervisorProyecto) && (!pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki) || ControllerBase.EsIdentidadActualSupervisorProyecto) && (pDocumento.TienePermisosEdicionIdentidad(ControllerBase.IdentidadActual, identidadBrOrg, ControllerBase.ProyectoSeleccionado, mControladorBase.UsuarioActual.UsuarioID, ControllerBase.EsIdentidadActualAdministradorOrganizacion) || pDocumento.TienePermisosIdentidadEliminarRecursoEnBR(ControllerBase.IdentidadActual, pDocumento.GestorDocumental.BaseRecursosIDActual, ControllerBase.ProyectoSeleccionado, ControllerBase.UsuarioActual.UsuarioID, ControllerBase.UsuarioActual.UsuarioID, ControllerBase.EsIdentidadActualAdministradorOrganizacion, ControllerBase.EsIdentidadActualSupervisorProyecto) || ControladorDocumentacion.EsEditorPerfilDeDocumento(ControllerBase.IdentidadActual.PerfilID, pDocumento, true, mControladorBase.UsuarioActual.UsuarioID))))
            {
                pFichaRecurso.Actions.Delete = false;
            }

            pFichaRecurso.Actions.CreateVersion = true;

            if (pDocumento.EsBorrador || !pDocumento.FilaDocumento.UltimaVersion || !pDocumento.PermiteVersiones || !ControllerBase.UsuarioActual.ProyectoID.Equals(pDocumento.ProyectoID) || (pDocumento.TipoDocumentacion == TiposDocumentacion.Wiki && pDocumento.FilaDocumento.Protegido && !ControllerBase.EsIdentidadActualSupervisorProyecto) || pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Encuesta) || pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Pregunta) || pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Debate) || (!pDocumento.TienePermisosEdicionIdentidad(ControllerBase.IdentidadActual, identidadBrOrg, ControllerBase.ProyectoSeleccionado, mControladorBase.UsuarioActual.UsuarioID, ControllerBase.EsIdentidadActualAdministradorOrganizacion) && !ControladorDocumentacion.EsEditorPerfilDeDocumento(ControllerBase.IdentidadActual.PerfilID, pDocumento, true, mControladorBase.UsuarioActual.UsuarioID)))
            {
                pFichaRecurso.Actions.CreateVersion = false;
            }

            if (pFichaRecurso.Actions.Edit)
            {
                if (pDocumento.TipoDocumentacion != TiposDocumentacion.Semantico)
                {
                    pFichaRecurso.EditCardLink = ControllerBase.UrlsSemanticas.GetURLBaseRecursosEditarDocumento(ControllerBase.BaseURLIdioma, ControllerBase.UtilIdiomas, NombreProyEdicionRecurso(pDocumento), ControllerBase.UrlPerfil, pDocumento, 1, esIdentidadBrOrg);
                }
                else
                {
                    bool esDocVirtual = false;
                    if (pDocumento.GestorDocumental.ListaDocumentos.ContainsKey(pDocumento.ElementoVinculadoID))
                    {
                        Dictionary<string, string> listaPropiedades = UtilCadenas.ObtenerPropiedadesDeTexto(pDocumento.GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].NombreEntidadVinculada);
                        if (listaPropiedades.ContainsKey(PropiedadesOntologia.urlservicio.ToString()))
                        {
                            esDocVirtual = true;
                        }
                    }
                    pFichaRecurso.EditCardLink = ControllerBase.UrlsSemanticas.GetURLBaseRecursosVerDocumentoCreado(ControllerBase.BaseURLIdioma, ControllerBase.UtilIdiomas, NombreProyEdicionRecurso(pDocumento), ControllerBase.UrlPerfil, pDocumento.Clave, pDocumento.FilaDocumento.ElementoVinculadoID.ToString(), true, esIdentidadBrOrg, esDocVirtual);
                }
            }

            if (pDocumento.TipoDocumentacion != TiposDocumentacion.Semantico)
            {
                pFichaRecurso.UrlNewVersion = ControllerBase.UrlsSemanticas.GetURLBaseRecursosEditarDocumento(ControllerBase.BaseURLIdioma, ControllerBase.UtilIdiomas, NombreProyEdicionRecurso(pDocumento), ControllerBase.UrlPerfil, pDocumento, 1, esIdentidadBrOrg);
            }
        }

        public bool TienePermisosEditarDoc(Documento pDocumento, Identidad pIdentidadActual, Identidad IdentidadOrganizacionBROrg, Proyecto pProyectoActual, SemCmsController pGenPlantillasOwl, bool pBloqueadoPorOtroUsuario)
        {
            if (pBloqueadoPorOtroUsuario || !pDocumento.FilaDocumento.UltimaVersion || pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki) || ((!pDocumento.TienePermisosEdicionIdentidad(pIdentidadActual, IdentidadOrganizacionBROrg, pProyectoActual, mControladorBase.UsuarioActual.UsuarioID, ControllerBase.EsIdentidadActualAdministradorOrganizacion)) && !ControladorDocumentacion.EsEditorPerfilDeDocumento(pIdentidadActual.PerfilID, pDocumento, true, mControladorBase.UsuarioActual.UsuarioID)))
            {
                return false;
            }
            else if (!pDocumento.EsBorrador && pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Encuesta) && pDocumento.FilaDocumento.DocumentoRespuestaVoto.Count > 0)
            {
                return false;
            }
            else if (ControllerBase.UsuarioActual.EsIdentidadInvitada)
            {
                return false;
            }
            else if (pGenPlantillasOwl != null && pGenPlantillasOwl.Ontologia.ConfiguracionPlantilla.OcultarBotonEditarDoc)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Nombre del proyecto para la edición del recurso.
        /// </summary>
        public string NombreProyEdicionRecurso(Documento pDocumento)
        {
            string nomProyEditar = "";

            if (ControllerBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                nomProyEditar = ControllerBase.NombreProyBusquedaOActual;
            }

            if (ControllerBase.ProyectoOrigenBusquedaID != Guid.Empty && pDocumento.FilaDocumento.ProyectoID == ControllerBase.ProyectoSeleccionado.Clave)
            {
                nomProyEditar = ControllerBase.ProyectoSeleccionado.NombreCorto;
            }

            return nomProyEditar;
        }


    }
}
