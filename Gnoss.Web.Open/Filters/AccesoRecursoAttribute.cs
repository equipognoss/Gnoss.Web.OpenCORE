using DocumentFormat.OpenXml.Office2010.Word;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Logica.MVC;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.MVC.Controllers.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Gnoss.Web.Open.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Filters
{

    public enum RolesAccesoRecurso
    {
        Lector = 0,
        Editor = 1
    }

    public class AccesoRecursoAttribute : BaseActionFilterAttribute
    {
        private ControladorBase mControladorBase;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private RedisCacheWrapper mRedisCacheWrapper;
        private VirtuosoAD mVirtuosoAD;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        private static PermisoRecursos[] mPermisosRecurso = { PermisoRecursos.EditarDebate, PermisoRecursos.EditarEncuesta,PermisoRecursos.EditarNota, PermisoRecursos.EditarPregunta, PermisoRecursos.EditarRecursoTipoAdjunto, PermisoRecursos.EditarRecursoTipoEnlace, PermisoRecursos.EditarRecursoTipoReferenciaADocumentoFisico, PermisoRecursos.EliminarDebate, PermisoRecursos.EliminarEncuesta,PermisoRecursos.EliminarNota, PermisoRecursos.EliminarPregunta, PermisoRecursos.EliminarRecursoTipoAdjunto, PermisoRecursos.EliminarRecursoTipoEnlace, PermisoRecursos.EliminarRecursoTipoReferenciaADocumentoFisico, PermisoRecursos.EliminarVersionDebate, PermisoRecursos.EliminarVersionEncuesta,PermisoRecursos.EliminarVersionNota, PermisoRecursos.EliminarVersionPregunta, PermisoRecursos.EliminarVersionAdjunto, PermisoRecursos.EliminarVersionEnlace, PermisoRecursos.EliminarVersionReferencia, PermisoRecursos.RestaurarVersionDebate, PermisoRecursos.RestaurarVersionEncuesta,PermisoRecursos.RestaurarVersionNota, PermisoRecursos.RestaurarVersionPregunta, PermisoRecursos.RestaurarVersionAdjunto, PermisoRecursos.RestaurarVersionEnlace, PermisoRecursos.RestaurarVersionReferencia, PermisoRecursos.CertificarRecurso};
        private static TipoPermisoRecursosSemanticos[] mPermisosRecursoSemantico = { TipoPermisoRecursosSemanticos.Modificar, TipoPermisoRecursosSemanticos.Eliminar, TipoPermisoRecursosSemanticos.EliminarVersion, TipoPermisoRecursosSemanticos.RestaurarVersion };


        public AccesoRecursoAttribute(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ILogger<AccesoRecursoAttribute> logger, ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAD;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, null, mLoggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);

            Rol = RolesAccesoRecurso.Lector;
        }

        public RolesAccesoRecurso Rol
        {
            get;
            set;
        }

        protected override void RealizarComprobaciones(ActionExecutingContext pFilterContext)
        {
            string parametroDocumentoID = mControladorBase.RequestParams("docID");
            string parametroDocumentoVersionID = mControladorBase.RequestParams("versionDocID");
            Guid documentoID;
            Guid documentoVersionID;
            Guid.TryParse(parametroDocumentoVersionID, out documentoVersionID);


            string urlRedirect = null;

            

            if (!string.IsNullOrEmpty(parametroDocumentoID) && (Guid.TryParse(parametroDocumentoID, out documentoID)))
            {
                DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                // Si no viene la version en la peticion es que estamos en la ficha de la ultima version
                documentoVersionID = documentoVersionID == Guid.Empty ? documentacionCN.ObtenerUltimaVersionDeDocumento(documentoID) : documentoVersionID;

                documentoID = documentoVersionID != Guid.Empty ? documentoVersionID : documentoID;
                //TODO JUAN. Hay que meter aquí la comprobación de MyGNOSS o se hace en la ficha?

                //Verificar si el usuario tiene permisos por algun rol
                if (TienePermisosGlobales(documentoID))
                {
                    return;
                }

                //Verifico si el recurso está compartido en la comunidad y no está eliminado
                if (mControladorBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto && !documentacionCN.EstaDocumentoCompartidoEnProyecto(documentoID, mControladorBase.ProyectoSeleccionado.Clave) && (Controlador(pFilterContext).ProyectoOrigenBusquedaID.Equals(Guid.Empty) || !documentacionCN.EstaDocumentoCompartidoEnProyecto(documentoID, Controlador(pFilterContext).ProyectoOrigenBusquedaID)))
                {
                    //Si el recurso esta descompartido, pero existe en otra comunidad, le llevamos a esa comunidad
                    Guid proyectoOriginalID = documentacionCN.ObtenerProyectoIDPorDocumentoID(documentoID);

                    if (proyectoOriginalID.Equals(Guid.Empty) || proyectoOriginalID.Equals(mControladorBase.ProyectoSeleccionado.Clave))
                    {
                        pFilterContext.Result = Controlador(pFilterContext).RedireccionarAPaginaNoEncontrada();
                    }
                    else
                    {
                        if (!proyectoOriginalID.Equals(ProyectoAD.MetaProyecto))
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

                            string nombreCortoProy = proyCN.ObtenerNombreCortoProyecto(proyectoOriginalID);

                            pFilterContext.Result = new RedirectResult(mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(Controlador(pFilterContext).BaseURLIdioma, Controlador(pFilterContext).UtilIdiomas, nombreCortoProy, "", mControladorBase.RequestParams("nombreRecurso"), documentoID, null, false));
                            return;
                        }
                    }
                }

                Guid? estadoID = documentacionCN.ObtenerEstadoIDDeDocumento(documentoID);
                if (estadoID.HasValue && !estadoID.Value.Equals(Guid.Empty))
                {
                    FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);
                    bool publico = flujosCN.ComprobarEstadoEsPublico(estadoID.Value);
                    if (!publico)
                    {
                        if (Rol.Equals(RolesAccesoRecurso.Editor))
                        {
                            if (!flujosCN.ComprobarIdentidadTienePermisoEdicionEnEstado(estadoID.Value, mControladorBase.IdentidadActual.Clave, documentoID))
                            {
                                pFilterContext.Result = Controlador(pFilterContext).RedireccionarAPaginaNoEncontrada();
                                return;
                            }
                        }
                        else
                        {
                            if (!flujosCN.ComprobarIdentidadTienePermisoLecturaEnEstado(estadoID.Value, mControladorBase.IdentidadActual.Clave, documentoID))
                            {
                                pFilterContext.Result = Controlador(pFilterContext).RedireccionarAPaginaNoEncontrada();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (Rol.Equals(RolesAccesoRecurso.Editor))
                    {
                        urlRedirect = ComprobarPermisosEdicionRecurso(documentoID);
                    }
                    else
                    {
                        urlRedirect = ComprobarPermisosLecturaRecurso(documentoID, pFilterContext);
                    }
                }
            }
            else
            {
                //El ID está mal formado, redirijo a la página 404
                pFilterContext.Result = Controlador(pFilterContext).RedireccionarAPaginaNoEncontrada();
                return;
            }

            if (!string.IsNullOrEmpty(urlRedirect))
            {
                Redireccionar(urlRedirect, pFilterContext);
            }
        }

        private bool TienePermisosGlobales(Guid pDocumentoId)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            TiposDocumentacion tipoDocumento = docCN.ObtenerTipoDocumentoPorDocumentoID(pDocumentoId);

            switch (tipoDocumento)
            {
                case TiposDocumentacion.Hipervinculo:
                case TiposDocumentacion.ReferenciaADoc:
                case TiposDocumentacion.FicheroServidor:
                case TiposDocumentacion.Nota:
                case TiposDocumentacion.Pregunta:
                case TiposDocumentacion.Debate:
                case TiposDocumentacion.Encuesta:
                    return TienePermisosGlobalesRecursoSemanticaDebil();
                case TiposDocumentacion.Semantico:
                    return TienePermisosGlobalesRecursoSemantico(docCN, pDocumentoId);
                default:
                    return false;
            }
        }
        private bool TienePermisosGlobalesRecursoSemanticaDebil()
        {
            if (mPermisosRecurso?.Length > 0)
            {
                UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
                
                Guid identidadId = mControladorBase.IdentidadActual.Clave;
                Guid identidadMyGnoss = mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave;

                foreach (ulong permisoRequerido in mPermisosRecurso)
                {
                    bool tienePermisoPagina = utilPermisos.IdentidadTienePermiso(permisoRequerido, identidadId, identidadMyGnoss, TipoDePermiso.Recursos);
                    if (tienePermisoPagina)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TienePermisosGlobalesRecursoSemantico(DocumentacionCN pDocCN, Guid pDocumentoId)
        {
            if (mPermisosRecursoSemantico?.Length > 0)
            {
                UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);

                Guid identidadId = mControladorBase.IdentidadActual.Clave;
                Guid identidadMyGnoss = mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave;

                foreach (TipoPermisoRecursosSemanticos permisoRequerido in mPermisosRecursoSemantico)
                {
                    Dictionary<Guid, Guid> ontologiaId = pDocCN.ObtenerElementoVinculadoIDPorDocumentoID([pDocumentoId]);
                    bool tienePermisoPagina = utilPermisos.IdentidadTienePermisoRecursoSemantico(identidadId, identidadId, permisoRequerido, ontologiaId[pDocumentoId]);
                    if (tienePermisoPagina)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string ComprobarPermisosLecturaRecurso(Guid pDocumentoID, ActionExecutingContext pFilterContext)
        {
            string urlRedirect = null;

            if (mControladorBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                // Si el usuario no pertenece a la comunidad: 
                // Chequear si el recurso es público

                // Si el usuario pertenece a la comunidad: 
                // Chequear si el recurso es público o público para usuarios registrados

                // Si el usuario pertenece a la comunidad: 
                // Chequear si el recurso es público o público para usuarios registrados o el usuario es lector

                //Si el documento no existe, o está eliminado, o no está compartido en esta comunidad, o es borrador, dar error 404

                DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                List<Guid> listaProyectos = new List<Guid>();
                listaProyectos.Add(mControladorBase.ProyectoSeleccionado.Clave);

                if (!Controlador(pFilterContext).ProyectoOrigenBusquedaID.Equals(Guid.Empty) || documentacionCN.EstaDocumentoCompartidoEnProyecto(pDocumentoID, Controlador(pFilterContext).ProyectoOrigenBusquedaID))
                {
                    listaProyectos.Add(Controlador(pFilterContext).ProyectoOrigenBusquedaID);
                }


                if (!documentacionCN.TieneUsuarioAccesoADocumentoEnProyecto(listaProyectos, pDocumentoID, mControladorBase.IdentidadActual.PerfilID, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, false, !mControladorBase.UsuarioActual.EsIdentidadInvitada))
                {
                    if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
                    {
                        string urlRedirectBase = mControladorBase.BaseURLIdioma + mControladorBase.UrlPerfil;
                        if (mControladorBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
                        {
                            urlRedirectBase = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(mControladorBase.UtilIdiomas, mControladorBase.BaseURLIdioma, mControladorBase.ProyectoSeleccionado.NombreCorto) + "/";
                        }
                        string urlRedireccion = urlRedirectBase + mControladorBase.UtilIdiomas.GetText("URLSEM", "LOGIN") + mControladorBase.UrlParaLoginConRedirect;
                        urlRedirect = urlRedireccion;
                    }
                    else
                    {
                        //Saco la página 403
                        pFilterContext.Result = Controlador(pFilterContext).RedireccionarAPaginaNoEncontrada("403");
                    }

                }

                documentacionCN.Dispose();
            }
            else
            {
                FichaRecursoController controladorRec = (FichaRecursoController)Controlador(pFilterContext);
                bool accesoCorrecto = false;

                if (controladorRec.Documento.FilaDocumentoWebVinBR.BaseRecursosID == controladorRec.Documento.GestorDocumental.BaseRecursosIDActual && !controladorRec.Documento.FilaDocumentoWebVinBR.Eliminado)
                {
                    if (controladorRec.UsuarioVeRecursoOtroPerfilID != Guid.Empty)
                    {//Usuario está viendo el perfil de otro. Hay que comprobar que el recurso está categorizado publico en la BR de ese usuario:
                        Guid catPublicaUsuario = controladorRec.Documento.GestorDocumental.GestorTesauro.CategoriaPublicaID;

                        foreach (CategoriaTesauro categoria in controladorRec.Documento.CategoriasTesauro.Values)
                        {
                            if (categoria.PadreNivelRaiz.Clave == catPublicaUsuario)
                            {
                                accesoCorrecto = true;
                                break;
                            }
                        }
                    }
                    else if (controladorRec.EsIdentidadBROrg)
                    {
                        List<AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion> filasBROrg = controladorRec.Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Where(item => item.BaseRecursosID.Equals(controladorRec.Documento.GestorDocumental.BaseRecursosIDActual)).ToList();

                        if (filasBROrg.Count > 0)
                        {
                            Guid orgID = filasBROrg[0].OrganizacionID;

                            if (mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion, orgID) || mControladorBase.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, orgID))
                            {
                                accesoCorrecto = true;
                            }
                        }
                    }
                    else if (controladorRec.Documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Where(item => item.BaseRecursosID.Equals(controladorRec.Documento.GestorDocumental.BaseRecursosIDActual) && item.UsuarioID.Equals(mControladorBase.UsuarioActual.UsuarioID)).Count() > 0)
                    {
                        accesoCorrecto = true;
                    }
                }

                if (!accesoCorrecto)
                {
                    //Saco la página 403
                    pFilterContext.Result = Controlador(pFilterContext).RedireccionarAPaginaNoEncontrada("403");
                }
            }

            return urlRedirect;
        }

        private string ComprobarPermisosEdicionRecurso(Guid pDocumentoID)
        {
            string urlRedirect = null;

            // Si se está comprobando el permiso de edición: 
            // Chequear si el usuario es editor

            if (mControladorBase.UsuarioActual.EsUsuarioInvitado || mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                //El usuario no pertenece a la comunidad, redirigir a la ficha del recurso
                urlRedirect = ObtenerUrlRutaRecurso(pDocumentoID);
            }
            else
            {
                DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                //Compruebo si el usuario es editor del recurso
                if (!documentacionCN.TieneUsuarioAccesoADocumentoEnProyecto(mControladorBase.ProyectoSeleccionado.Clave, pDocumentoID, mControladorBase.IdentidadActual.PerfilID, mControladorBase.IdentidadActual.Clave, mControladorBase.IdentidadActual.IdentidadMyGNOSS.Clave, true, true))
                {
                    // No es editor del recurso, redirige a la ficha del recurso
                    urlRedirect = ObtenerUrlRutaRecurso(pDocumentoID);
                }
            }

            return urlRedirect;
        }

        private string ObtenerUrlRutaRecurso(Guid pDocumentoID)
        {
            //obtener datos recuros

            string titulo;
            Guid? elementoVinculadoID;
            short tipo;


            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            docCN.ObtenerTituloElementoVinculadoIDTipoDeRecurso(pDocumentoID, out titulo, out elementoVinculadoID, out tipo);

            return mControladorBase.UrlsSemanticas.GetURLBaseRecursosFichaConIDs(mControladorBase.BaseURLIdioma, mControladorBase.UtilIdiomas, mControladorBase.ProyectoSeleccionado.NombreCorto, mControladorBase.UrlPerfil, titulo, pDocumentoID, elementoVinculadoID, (TiposDocumentacion)tipo, mControladorBase.IdentidadActual.TrabajaPersonaConOrganizacion);
        }
    }
}