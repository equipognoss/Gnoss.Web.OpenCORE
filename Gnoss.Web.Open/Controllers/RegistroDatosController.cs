using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.Live;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
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
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Es.Riam.Gnoss.Web.MVC.Controllers
{
    public class RegistroDatosController : ControllerBaseWeb
    {
        public RegistroDatosController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine, EntityContextBASE entityContextBASE, IHostingEnvironment env, IActionContextAccessor actionContextAccessor, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IOAuth oAuth)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, viewEngine, entityContextBASE, env, actionContextAccessor, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication, oAuth)
        {
        }

        RegisterDataViewModel paginaModel = new RegisterDataViewModel();

        private List<PreferenciaProyecto> mFilasPreferenciaProyectos = null;
        //Datos extra del proyecto
        private DataWrapperProyecto mDatosExtraProyectoDataWrapperProyecto = null;
        private string mUrlRedireccionTrasRegistro;

        private GestionTesauro mGestorTesauro = null;

        private bool mTieneDatosRegistro = false;

        private bool? mPestanyaDatos = null;

        [AcceptVerbs]
        public ActionResult Index()
        {
            if (mControladorBase.UsuarioActual.EsIdentidadInvitada)
            {
                return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            if (Request.HasFormContentType && Request.Form.Files.Count > 0)
            {
                return GuardarImagenRegistro();
            }

            if (RequestParams("paginaOriginal") != null)
            {
                paginaModel.ReferrerPage = RequestParams("paginaOriginal");
            }

            if (Session.Get("paginaOriginal") != null)
            {
                paginaModel.ReferrerPage = JsonSerializer.Deserialize<string>(Session.GetString("paginaOriginal"));
                Session.Remove("paginaOriginal");
            }
            else if (Request.HasFormContentType && Request.Form.ContainsKey("prefer"))
            {
                //return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

            if (Request.HasFormContentType && Request.Form.ContainsKey("tienePaginaDatosRegistro") && Request.Form["tienePaginaDatosRegistro"] == "true")
            {
                mTieneDatosRegistro = true;
                Session.Set("tienePaginaDatosRegistro", mTieneDatosRegistro);
            }
            else if (Session.Get("tienePaginaDatosRegistro") != null)
            {
                mTieneDatosRegistro = Session.Get<bool>("tienePaginaDatosRegistro");
                Session.Remove("tienePaginaDatosRegistro");
            }

            int numPaso = int.Parse(RequestParams("paso"));

            paginaModel.TabPreferences = PestanyaPreferencias;
            paginaModel.TabData = PestanyaDatos;
            paginaModel.TabConect = PestanyaConecta;

            if (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Count > 0)
            {
                List<ProyectoPasoRegistro> filasPasos = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.OrderBy(proy=>proy.Orden).ToList();

                ProyectoPasoRegistro filaActual = filasPasos[numPaso - 1];

                PasosRegistro pasoRegistroActual;

                bool esPasoPredefinido = Enum.TryParse<PasosRegistro>(filaActual.PasoRegistro, out pasoRegistroActual);

                if (esPasoPredefinido)
                {
                    numPaso = (int)pasoRegistroActual + 1;
                }
                else
                {
                    return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + filaActual.PasoRegistro + "?referer=" + UrlOrigen);
                }
            }
            else
            {
                if (!PestanyaPreferencias) { numPaso++; }
                if (!PestanyaDatos && numPaso > 1) { numPaso++; }
                if (!PestanyaConecta && numPaso > 2) { numPaso++; }
            }

            paginaModel.TabActive = numPaso;

            bool errores = false;

            switch (numPaso)
            { 
                case 1:
                    if (Request.HasFormContentType && Request.Form.ContainsKey("paginaPreferencias") && Request.Form.ContainsKey("listaCategorias"))
                    {
                        GuardarPreferenciasRegistro();
                    }
                    else
                    {
                        CargarPreferenciasRegistro();
                    }
                    break;
                case 2:
                    if (Request.HasFormContentType && Request.Form.ContainsKey("paginaDatos"))
                    {
                        errores = !GuardarDatosRegistro();                        
                        // Si se produce algún error es necesario que el controlador construya los paises para evitar errores en la vista
                        if (errores == true)
                        {
                            CargarDatosRegistro();
                        }
                        // Si se produce algún error es necesario que el controlador construya los paises para evitar errores en la vista
                        if (errores == true)
                        {
                            CargarDatosRegistro();
                        }
                    }
                    else
                    {
                        CargarDatosRegistro();
                    }
                    break;
                case 3:
                    if (!Request.Query.ContainsKey("paginaConectate"))
                    {
                        bool hayResultados = CargarConectaRegistro();
                        if(!hayResultados)
                        {
                            if (!string.IsNullOrEmpty(UrlOrigen))
                            {
                                Session.Set("paginaOriginal", UrlOrigen);
                            }
                            return new RedirectResult(UrlSiguientePaso);
                        }
                    }
                    break;
                default:
                    //Esto no se si sera asi, debido a que si entra con un numero que no es ni 1, 2 o 3 quitara de BD este valor.
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    usuarioCN.EliminarUrlRedirect(mControladorBase.UsuarioActual.UsuarioID);
                    if (string.IsNullOrEmpty(UrlRedireccionTrasRegistro))
                    {
                        return new RedirectResult(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
                    }
                    else
                    {
                        return new RedirectResult(UrlRedireccionTrasRegistro);
                    }


            }

            if(Request.Method == "POST" && !errores)
            {
                if (!string.IsNullOrEmpty(UrlOrigen))
                {
                    Session.Set("paginaOriginal", UrlOrigen);
                }
                return new RedirectResult(UrlSiguientePaso);
            }

            return View(paginaModel);
        }

        private ActionResult GuardarImagenRegistro()
        {
            string error = string.Empty;

            int tamanoMini = 60;
            int tamanoMaxi = 240;

            int minSize = 240;
            int maxSize = 450;

            IFormFile ficheroImagen = Request.Form.Files["ImagenRegistroUsuario"];

            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
            servicioImagenes.Url = UrlIntragnossServicios;

            //Si se sube un fichero nuevo se borra la foto temporal
            servicioImagenes.BorrarImagen("Personas/" + mControladorBase.UsuarioActual.PersonaID.ToString() + "_temp" + ".png");
            servicioImagenes.BorrarImagen("Personas/" + mControladorBase.UsuarioActual.PersonaID.ToString() + "_temp2" + ".png");

            //Límite de 10 MB
            if (ficheroImagen.Length <= 10 * 1024 * 1024)
            {
                byte[] bytesFichero = new byte[ficheroImagen.Length];
                ((System.IO.Stream)ficheroImagen.OpenReadStream()).Read(bytesFichero, 0, (int)ficheroImagen.Length);

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
                        servicioImagenes.AgregarImagen(ms.ToArray(), "Personas/" + mControladorBase.UsuarioActual.PersonaID.ToString(), ".png");

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

                        servicioImagenes.AgregarImagen(msGrande.ToArray(), "Personas/" + mControladorBase.UsuarioActual.PersonaID.ToString() + "_grande", ".png");

                        Image imagenCortadaMini = UtilImages.AjustarImagen(imagenCortada, tamanoMini, tamanoMini, false);

                        //convertimos la imagen a png
                        MemoryStream msMini = new MemoryStream();
                        imagenCortadaMini.SaveAsPng(msMini);

                        servicioImagenes.AgregarImagen(msMini.ToArray(), "Personas/" + mControladorBase.UsuarioActual.PersonaID.ToString() + "_peque", ".png");

                        if (mEntityContext.Entry(IdentidadActual.Persona.FilaPersona).State.Equals(EntityState.Detached))
                        {
                            IdentidadActual.Persona.FilaPersona = mEntityContext.Persona.FirstOrDefault(pers => pers.PersonaID.Equals(IdentidadActual.Persona.FilaPersona.PersonaID));
                        }

                        if (!IdentidadActual.Persona.FilaPersona.VersionFoto.HasValue)
                        {
                            IdentidadActual.Persona.FilaPersona.VersionFoto = 1;
                        }
                        else
                        {
                            IdentidadActual.Persona.FilaPersona.VersionFoto++;
                        }

                        IdentidadActual.Persona.FilaPersona.CoordenadasFoto = "[ " + x.ToString() + ", " + y.ToString() + ", " + (x + w).ToString() + ", " + (y + h).ToString() + " ]";
                        IdentidadActual.Persona.FilaPersona.FechaAnadidaFoto = DateTime.Now;
                        mEntityContext.SaveChanges();
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        identidadCN.ActualizarFotoIdentidadesPersona(IdentidadActual.PersonaID.Value, false);
                        identidadCN.Dispose();

                        //Borramos Cache de la Identidad actual
                        IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Persona.Clave, IdentidadActual.PerfilID);
                        identidadCL.Dispose();

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
            EliminarCaches();

            if (error.Equals(string.Empty))
            {
                return Content(UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesPersonas + "/" + mControladorBase.UsuarioActual.PersonaID.ToString().ToLower() + "_grande.png?" + Guid.NewGuid().ToString());
            }
            else
            {
                return StatusCode(400, error);
            }
        }

        [HttpGet]
        public ActionResult Redirigir()
        {
            if (RequestParams("url") != null)
            {
                int numPaso = 0;
                List<ProyectoPasoRegistro> PasoURL = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Where(item => item.PasoRegistro.Equals(RequestParams("url"))).ToList();
                if (PasoURL.Count > 0)
                {
                    ProyectoPasoRegistro filaOrden = PasoURL.FirstOrDefault();
                    numPaso = filaOrden.Orden + 2;
                }
                string urlRedirect = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/" + (numPaso);
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                usuarioCN.ModificarUrlRedirect(mControladorBase.UsuarioActual.UsuarioID, urlRedirect);
                return Redirect(urlRedirect);
            }else
            {
                return Redirect(mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto));
            }

        }

        private void EliminarCaches()
        {
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


            prefijoClave = prefijoClave + "_" + identidadCL.ClaveCache[0] + "_4.0.0.0_";
            prefijoClave = prefijoClave.ToLower();

            foreach (Guid perfilID in IdentidadActual.GestorIdentidades.ListaPerfiles.Keys)
            {
                string rawKey = string.Concat("IdentidadActual_", IdentidadActual.PersonaID, "_", perfilID);
                string rawKey2 = "PerfilMVC_" + perfilID;
                string rawKeyCache = identidadCL.ObtenerClaveCache(string.Concat("IdentidadActual_", IdentidadActual.PersonaID, "_", perfilID));
                string rawKey2Cache = identidadCL.ObtenerClaveCache("PerfilMVC_" + perfilID);
                listaClavesInvalidar.Add(rawKey2Cache.ToLower());
                listaClavesInvalidar.Add(rawKeyCache.ToLower());
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

        private void GuardarPreferenciasRegistro()
        {
            List<Guid> listaCategoriasSeleccionadas = new List<Guid>();

            string[] categorias = Request.Form["listaCategorias"].ToString().Split('&');

            if (categorias.Length > 1)
            {
                for (int i = 1; i < categorias.Length; i++)
                {
                    listaCategoriasSeleccionadas.Add(new Guid(categorias[i]));
                }
            }

            if (listaCategoriasSeleccionadas.Count > 0)
            {
                GestionSuscripcion gestorSusc = new GestionSuscripcion(new DataWrapperSuscripcion(), mLoggingService, mEntityContext);

                SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Suscripcion susc = gestorSusc.CrearSuscripcion(IdentidadActual);
                susc.FilaSuscripcion.Periodicidad = (short)PeriodicidadSuscripcion.Diaria;

                AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto filaTesauro = new AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto();
                filaTesauro.SuscripcionID = susc.FilaSuscripcion.SuscripcionID;
                filaTesauro.ProyectoID = ProyectoSeleccionado.Clave;
                filaTesauro.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                Guid TesauroID = GestorTesauro.TesauroActualID;
                filaTesauro.TesauroID = TesauroID;
                gestorSusc.SuscripcionDW.ListaSuscripcionTesauroProyecto.Add(filaTesauro);
                mEntityContext.SuscripcionTesauroProyecto.Add(filaTesauro);

                foreach (Guid catID in listaCategoriasSeleccionadas)
                {
                    //Ya sólo quedan categorías añadidas, así que añado las filas
                    susc.GestorSuscripcion.VincularCategoria(susc, mGestorTesauro.ListaCategoriasTesauro[catID]);
                }

                suscCN.ActualizarSuscripcion();
                suscCN.Dispose();
            }

            //Le generamos suscripciones en redis
            Guid usuarioActualID = mControladorBase.UsuarioActual.UsuarioID;
            Guid perfilAcualID = IdentidadActual.PerfilID;
            Guid proyectoActualID = ProyectoSeleccionado.Clave;

            try
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<Guid> listaDocumentos = docCN.ObtenerDocumentosIDSuscripcionPerfilEnProyecto(perfilAcualID, proyectoActualID, 100);

                LiveUsuariosCL liveUsuariosCL = new LiveUsuariosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                int score = 1;
                for (int i = listaDocumentos.Count - 1; i >= 0; i--)
                {
                    string clave = "0_" + listaDocumentos[i].ToString() + "_" + ProyectoSeleccionado.Clave;
                    score = liveUsuariosCL.AgregarLiveProyectoUsuarioSuscripciones(usuarioActualID, proyectoActualID, clave, score) + 1;
                }
            }
            catch (Exception ex)
            {
                GuardarLogError(ex, "Error controlado, no se genera la actividad reciente pero la ejecución continúa");
            }

            ControladorIdentidades.AccionEnServicioExternoProyecto(TipoAccionExterna.Edicion, IdentidadActual.Persona, ProyectoSeleccionado.Clave, IdentidadActual.Clave, "", IdentidadActual.Persona.Email, IdentidadActual.FilaIdentidad.FechaAlta, null);
        }

        private bool HayErroresDatosRegistro()
        {
            List<string> errores = new List<string>();

            paginaModel.Name = IdentidadActual.Persona.Nombre;
            paginaModel.LastName = IdentidadActual.Persona.Apellidos;
            paginaModel.CountryID = IdentidadActual.Persona.PaisID;
            paginaModel.RegionID = IdentidadActual.Persona.ProvinciaID;
            paginaModel.Region = IdentidadActual.Persona.Provincia;
            paginaModel.Location = IdentidadActual.Persona.Localidad;
            paginaModel.Gender = IdentidadActual.Persona.Sexo;
            paginaModel.AskCountry = paginaModel.AskLocation = MostrarDatosDemograficosPerfil; //valor de inicio
            paginaModel.AskRegion = false;

            string foto = UtilArchivos.ContentImagenes + IdentidadActual.UrlImagenGrande;
            if (!foto.Contains("anonimo"))
            {
                paginaModel.Foto = UtilArchivos.ContentImagenes + IdentidadActual.UrlImagenGrande;
            }

            bool esPrimerRegistro = string.IsNullOrEmpty(IdentidadActual.Persona.Localidad);

            //.Equals("0") -> sin definir
            if (!string.IsNullOrEmpty(paginaModel.Gender) && paginaModel.Gender.Equals("0")) 
            {
                if (string.IsNullOrEmpty(RequestParams("ddlSexo")))
                {
                    if (!errores.Contains("camposVacios"))
                    {
                        errores.Add("camposVacios");
                    }
                }
                else
                {
                    paginaModel.Gender = RequestParams("ddlSexo");
                    IdentidadActual.Persona.Sexo = paginaModel.Gender;
                }
            }

            CargarDatosGenericosRegistro( );

            if (MostrarDatosDemograficosPerfil || paginaModel.AskCountry || paginaModel.AskRegion || paginaModel.AskLocation)
            {
                if ((paginaModel.AskCountry || MostrarDatosDemograficosPerfil) && paginaModel.CountryID == null || paginaModel.CountryID.Equals(Guid.Empty))
                {
                    //if (MostrarDatosDemograficosPerfil)
                    //{
                    //    paginaModel.AskCountry = true;
                    //    paginaModel.AskLocation = true;
                    //}

                    if (new Guid(RequestParams("ddlPais")) == Guid.Empty)
                    {
                        if (!errores.Contains("camposVacios"))
                        {
                            errores.Add("camposVacios");
                        }
                    }
                    else
                    {
                        paginaModel.CountryID = new Guid(RequestParams("ddlPais"));
                        IdentidadActual.Persona.PaisID = paginaModel.CountryID;
                    }
                }

                if (paginaModel.AskRegion && paginaModel.RegionID == null && string.IsNullOrEmpty(paginaModel.Region))
                {
                    if (new Guid(RequestParams("ddlProvincia")) == Guid.Empty && string.IsNullOrEmpty(RequestParams("txtProvincia")))
                    {
                        if (!errores.Contains("camposVacios"))
                        {
                            errores.Add("camposVacios");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(RequestParams("txtProvincia")))
                        {
                            paginaModel.Region = RequestParams("txtProvincia");
                            IdentidadActual.Persona.Provincia = paginaModel.Region;
                        }
                        else
                        {
                            paginaModel.RegionID = new Guid(RequestParams("ddlProvincia"));
                            IdentidadActual.Persona.ProvinciaID = paginaModel.RegionID;
                        }
                    }
                }

                if (paginaModel.AskLocation && string.IsNullOrEmpty(paginaModel.Location))
                {
                    if (string.IsNullOrEmpty(RequestParams("txtPoblacion")))
                    {
                        errores.Add("camposVacios");
                    }
                    else
                    {
                        paginaModel.Location = RequestParams("txtPoblacion");
                        IdentidadActual.Persona.Localidad = paginaModel.Location;
                    }
                }
            }
            

            if (ParametrosGeneralesRow.PrivacidadObligatoria && esPrimerRegistro)
            {
                List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VisibilidadPerfil")).ToList();
                if (busqueda.Count == 1)
                {
                    //string visibilidadPerfil = (string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'VisibilidadPerfil'")[0]["Valor"];
                    string visibilidadPerfil = busqueda.First().Valor;
                    if (visibilidadPerfil[0].ToString() == "1")
                    {
                        paginaModel.IsSearched = RequestParams("chkListarPerfil") != null;
                        IdentidadActual.Persona.FilaPersona.EsBuscable = RequestParams("chkListarPerfil") != null;
                    }
                    if (visibilidadPerfil[2].ToString() == "1")
                    {
                        paginaModel.IsExternalSearched = RequestParams("chkListarPerfilExterno") != null;
                        IdentidadActual.Persona.FilaPersona.EsBuscableExternos = RequestParams("chkListarPerfilExterno") != null;
                    }
                }
                else
                {
                    paginaModel.IsSearched = RequestParams("chkListarPerfil") != null;
                    paginaModel.IsExternalSearched = RequestParams("chkListarPerfil") != null;
                    IdentidadActual.Persona.FilaPersona.EsBuscable = RequestParams("chkListarPerfil") != null;        
                    IdentidadActual.Persona.FilaPersona.EsBuscableExternos = RequestParams("chkListarPerfilExterno") != null;
                }
            }

            foreach (AdditionalFieldAutentication campoExtra in paginaModel.AdditionalFields)
            {
                string nombreCampo = campoExtra.FieldName;
                string valorCampo = RequestParams(nombreCampo);

                campoExtra.FieldValue = valorCampo;

                if (campoExtra.Required && string.IsNullOrEmpty(valorCampo))
                {
                    if (!errores.Contains("camposVacios"))
                    {
                        errores.Add("camposVacios");
                    }
                }
            }

            if (errores.Count > 0)
            {
                if (paginaModel.CountryID != null)
                {
                    paginaModel.CountryList = new Dictionary<Guid, string>();
                    foreach (AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
                    {
                        paginaModel.CountryList.Add(filaPais.PaisID, filaPais.Nombre);
                        if (filaPais.Nombre.Equals("España"))
                        {
                            paginaModel.RegionList = new Dictionary<Guid, string>();
                            foreach (AD.EntityModel.Models.Pais.Provincia fila in PaisesDW.ListaProvincia.Where(provincia => provincia.PaisID == filaPais.PaisID))
                            {
                                paginaModel.RegionList.Add(fila.ProvinciaID, fila.Nombre);
                            }
                        }
                    }
                }

                paginaModel.Errors = errores;
                return true;
            }

            return false;
        }

        private bool GuardarDatosRegistro()
        {
            if (mEntityContext.Entry(IdentidadActual.Persona.FilaPersona).State.Equals(EntityState.Detached))
            {
                IdentidadActual.Persona.FilaPersona = mEntityContext.Persona.FirstOrDefault(pers => pers.PersonaID.Equals(IdentidadActual.Persona.FilaPersona.PersonaID));
            }
            CargarCamposExtraRegistro();

            if (!HayErroresDatosRegistro())
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);

                List<DatoExtraProyectoOpcionIdentidad> listaDatoExtraProyectoOpcionIdentidadBorar = dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.ToList();
                foreach (DatoExtraProyectoOpcionIdentidad filaDatoExtra in listaDatoExtraProyectoOpcionIdentidadBorar)
                {
                    if (filaDatoExtra.IdentidadID == IdentidadActual.Clave)
                    {
                        mEntityContext.EliminarElemento(filaDatoExtra);
                        dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.Remove(filaDatoExtra);
                    }
                }

                List<DatoExtraProyectoVirtuosoIdentidad> listaDatoExtraProyectoVirtuosoIdentidadBorrar = dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad.ToList();
                foreach (DatoExtraProyectoVirtuosoIdentidad filaDatoExtraVirtuoso in listaDatoExtraProyectoVirtuosoIdentidadBorrar)
                {
                    if (filaDatoExtraVirtuoso.IdentidadID == IdentidadActual.Clave)
                    {
                        mEntityContext.EliminarElemento(filaDatoExtraVirtuoso);
                        dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad.Remove(filaDatoExtraVirtuoso);
                    }
                }

                List<DatoExtraEcosistemaOpcionPerfil> listaDatoExtraEcosistemaOpcionPerfil = dataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil.ToList();
                foreach (DatoExtraEcosistemaOpcionPerfil filaDatoExtraEcosistema in listaDatoExtraEcosistemaOpcionPerfil)
                {
                    if (filaDatoExtraEcosistema.PerfilID == IdentidadActual.PerfilID)
                    {
                        mEntityContext.EliminarElemento(filaDatoExtraEcosistema);
                        dataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil.Remove(filaDatoExtraEcosistema);
                    }
                }

                List<DatoExtraEcosistemaVirtuosoPerfil> listaDatoExtraEcosistemaVirtuosoPerfilBorrar = dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.ToList();
                foreach (DatoExtraEcosistemaVirtuosoPerfil filaDatoExtraEcosistemaVirtuoso in listaDatoExtraEcosistemaVirtuosoPerfilBorrar)
                {
                    if (filaDatoExtraEcosistemaVirtuoso.PerfilID == IdentidadActual.PerfilID)
                    {
                        mEntityContext.EliminarElemento(filaDatoExtraEcosistemaVirtuoso);
                        dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.Remove(filaDatoExtraEcosistemaVirtuoso);
                    }
                }

                Dictionary<Guid, Guid> dicDatosExtraProyecto = new Dictionary<Guid, Guid>();
                Dictionary<Guid, Guid> dicDatosExtraEcosistema = new Dictionary<Guid, Guid>();

                Dictionary<int, string> dicDatosExtraProyectoVirtuoso = new Dictionary<int, string>();
                Dictionary<int, string> dicDatosExtraEcosistemaVirtuoso = new Dictionary<int, string>();

                foreach (AdditionalFieldAutentication campoExtra in paginaModel.AdditionalFields)
                {
                    string nombreCampo = campoExtra.FieldName;
                    string valorCampo = RequestParams(nombreCampo);

                    Guid guidNombreCampo = Guid.Empty;
                    if (Guid.TryParse(nombreCampo, out guidNombreCampo))
                    {
                        Guid guidValorCampo = Guid.Empty;
                        if (Guid.TryParse(valorCampo, out guidValorCampo) && !guidValorCampo.Equals(Guid.Empty))
                        {
                            DatoExtraEcosistema filaDatoExtraEcosistema = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.FirstOrDefault(dato=>dato.DatoExtraID.Equals(guidNombreCampo));
                            if (filaDatoExtraEcosistema != null)
                            {
                                dicDatosExtraEcosistema.Add(filaDatoExtraEcosistema.DatoExtraID, guidValorCampo);
                            }
                            else
                            {
                                DatoExtraProyecto filaDatoExtraProyecto = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.FirstOrDefault(dato=>dato.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && dato.ProyectoID.Equals(ProyectoSeleccionado.Clave) && dato.DatoExtraID.Equals(guidNombreCampo));
                                if (filaDatoExtraProyecto != null)
                                {
                                    dicDatosExtraProyecto.Add(filaDatoExtraProyecto.DatoExtraID, guidValorCampo);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(valorCampo))
                        {
                            List<DatoExtraEcosistemaVirtuoso> filasDatoExtraEcosistemaVirtuoso = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Where(dato=>dato.InputID.Equals(nombreCampo)).ToList();
                            if (filasDatoExtraEcosistemaVirtuoso.Count > 0)
                            {
                                dicDatosExtraEcosistemaVirtuoso.Add(filasDatoExtraEcosistemaVirtuoso.First().Orden, valorCampo);

                            }
                            else
                            {
                                List<DatoExtraProyectoVirtuoso> filasDatoExtraProyectoVirtuoso = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Where(dato => dato.InputID.Equals(nombreCampo)).ToList();
                                if (filasDatoExtraProyectoVirtuoso.Count > 0)
                                {
                                    dicDatosExtraProyectoVirtuoso.Add(filasDatoExtraProyectoVirtuoso[0].Orden, valorCampo);

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
                    datoExtraProyectoOpcionIdentidad.IdentidadID = IdentidadActual.FilaIdentidad.IdentidadID;
                    dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.Add(datoExtraProyectoOpcionIdentidad);
                    mEntityContext.DatoExtraProyectoOpcionIdentidad.Add(datoExtraProyectoOpcionIdentidad);
                }

                foreach (int orden in dicDatosExtraProyectoVirtuoso.Keys)
                {
                    if (!string.IsNullOrEmpty(dicDatosExtraProyectoVirtuoso[orden].Trim()) && dicDatosExtraProyectoVirtuoso[orden].Trim() != "|")
                    {
                        string valor = dicDatosExtraProyectoVirtuoso[orden].Trim();
                        if (valor.EndsWith("|"))
                        {
                            valor = valor.Substring(0, valor.Length - 1);
                        }

                        valor = IntentoObtenerElPais(valor);                  
                        DatoExtraProyectoVirtuosoIdentidad datoExtraProyectoVirtuosoIdentidad = new DatoExtraProyectoVirtuosoIdentidad();
                        datoExtraProyectoVirtuosoIdentidad.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        datoExtraProyectoVirtuosoIdentidad.ProyectoID = ProyectoSeleccionado.Clave;
                        datoExtraProyectoVirtuosoIdentidad.DatoExtraID = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Where(dato => dato.Orden.Equals(orden)).FirstOrDefault().DatoExtraID;
                        datoExtraProyectoVirtuosoIdentidad.Opcion = valor;
                        datoExtraProyectoVirtuosoIdentidad.IdentidadID = IdentidadActual.FilaIdentidad.IdentidadID;
                        dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad.Add(datoExtraProyectoVirtuosoIdentidad);
                        mEntityContext.DatoExtraProyectoVirtuosoIdentidad.Add(datoExtraProyectoVirtuosoIdentidad);
                    }
                }


                foreach (Guid datoExtra in dicDatosExtraEcosistema.Keys)
                {
                    
                    DatoExtraEcosistemaOpcionPerfil datoExtraEcosistemaOpcionPerfil = new DatoExtraEcosistemaOpcionPerfil();
                    datoExtraEcosistemaOpcionPerfil.DatoExtraID = datoExtra;
                    datoExtraEcosistemaOpcionPerfil.OpcionID = dicDatosExtraEcosistema[datoExtra];
                    datoExtraEcosistemaOpcionPerfil.PerfilID = IdentidadActual.PerfilUsuario.FilaPerfil.PerfilID;
                    dataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil.Add(datoExtraEcosistemaOpcionPerfil);
                    mEntityContext.DatoExtraEcosistemaOpcionPerfil.Add(datoExtraEcosistemaOpcionPerfil);
                }

                foreach (int orden in dicDatosExtraEcosistemaVirtuoso.Keys)
                {
                    if (!string.IsNullOrEmpty(dicDatosExtraEcosistemaVirtuoso[orden].Trim()) && dicDatosExtraEcosistemaVirtuoso[orden].Trim() != "|")
                    {
                        string valor = dicDatosExtraEcosistemaVirtuoso[orden].Trim();
                        if (valor.EndsWith("|"))
                        {
                            valor = valor.Substring(0, valor.Length - 1);
                        }

                        valor = IntentoObtenerElPais(valor);

                        
                        DatoExtraEcosistemaVirtuosoPerfil datoExtraEcosistemaVirtuosoPerfil = new DatoExtraEcosistemaVirtuosoPerfil();
                        datoExtraEcosistemaVirtuosoPerfil.DatoExtraID = DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Where(dato => dato.Orden.Equals(orden)).First().DatoExtraID;
                        datoExtraEcosistemaVirtuosoPerfil.PerfilID = IdentidadActual.PerfilUsuario.FilaPerfil.PerfilID;
                        datoExtraEcosistemaVirtuosoPerfil.Opcion = valor;
                        dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.Add(datoExtraEcosistemaVirtuosoPerfil);
                        mEntityContext.DatoExtraEcosistemaVirtuosoPerfil.Add(datoExtraEcosistemaVirtuosoPerfil);
                    }
                }

                JsonEstado jsonEstado = ControladorIdentidades.AccionEnServicioExternoEcosistema(TipoAccionExterna.Edicion, ProyectoSeleccionado.Clave, mControladorBase.UsuarioActual.UsuarioID, IdentidadActual.Persona.FilaPersona.Nombre, IdentidadActual.Persona.FilaPersona.Apellidos, IdentidadActual.Persona.FilaPersona.Email, "", GestorParametroAplicacion, DatosExtraProyectoDataWrapperProyecto, dicDatosExtraEcosistemaVirtuoso, dicDatosExtraProyectoVirtuoso, IdentidadActual.Persona.FilaPersona.Email, IdentidadActual.Persona.FilaPersona.Idioma);
                if (jsonEstado != null && !jsonEstado.Correcto)
                {
                    string error = jsonEstado.InfoExtra;

                    return false;
                }

                if (paginaModel.AskCountry && !string.IsNullOrEmpty(RequestParams("ddlPais")) && IdentidadActual.Persona.PaisID != new Guid(RequestParams("ddlPais")))
                {
                    IdentidadActual.Persona.PaisID = new Guid(RequestParams("ddlPais"));
                }

                if (paginaModel.AskRegion)
                {
                    if (!string.IsNullOrEmpty(RequestParams("ddlRegion")) && IdentidadActual.Persona.ProvinciaID != new Guid(RequestParams("ddlRegion")) && string.IsNullOrEmpty(IdentidadActual.Persona.Provincia))
                    {
                        IdentidadActual.Persona.ProvinciaID = new Guid(RequestParams("ddlRegion"));
                    }
                    else if (!string.IsNullOrEmpty(RequestParams("txtProvincia")) && IdentidadActual.Persona.Provincia != RequestParams("txtProvincia") && IdentidadActual.Persona.ProvinciaID.Equals(Guid.Empty))
                    {
                        IdentidadActual.Persona.Provincia = RequestParams("txtProvincia");
                    }
                }

                if (paginaModel.AskLocation && !string.IsNullOrEmpty(RequestParams("txtPoblacion")) && IdentidadActual.Persona.Localidad != RequestParams("txtPoblacion"))
                {
                    IdentidadActual.Persona.Localidad = RequestParams("txtPoblacion");
                }

                if (paginaModel.AskGender && !string.IsNullOrEmpty(RequestParams("ddlSexo")) && IdentidadActual.Persona.Sexo != RequestParams("ddlSexo"))
                {
                    IdentidadActual.Persona.Sexo = RequestParams("ddlSexo");
                }

                mEntityContext.SaveChanges();
                
                ControladorIdentidades.NotificarEdicionPerfilEnProyectos(TipoAccionExterna.Edicion, IdentidadActual.Persona.Clave, "", "");

                ControladorPersonas contrPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                //contrPers.ActualizarModeloBaseSimple(IdentidadActual, ProyectoAD.MetaProyecto, PrioridadBase.Alta);
                contrPers.ActualizarModeloBaseSimple(IdentidadActual, ProyectoAD.MetaProyecto, UrlIntragnoss);

                if (ParticipaUsuarioActualEnProyecto(ProyectoAD.ProyectoFAQ))
                {
                    //contrPers.ActualizarModeloBaseSimple(IdentidadActual.Persona.Clave, ProyectoAD.ProyectoFAQ, PrioridadBase.Alta);
                    contrPers.ActualizarModeloBaseSimple(IdentidadActual, ProyectoAD.ProyectoFAQ, UrlIntragnoss);
                }
                if (ParticipaUsuarioActualEnProyecto(ProyectoAD.ProyectoNoticias))
                {
                    //contrPers.ActualizarModeloBaseSimple(IdentidadActual.Persona.Clave, ProyectoAD.ProyectoNoticias, PrioridadBase.Alta);
                    contrPers.ActualizarModeloBaseSimple(IdentidadActual, ProyectoAD.ProyectoNoticias,UrlIntragnoss);
                }
                //contrPers.ActualizarModeloBaseSimple(IdentidadActual.Persona.Clave, ProyectoSeleccionado.Clave, PrioridadBase.Alta);
                contrPers.ActualizarModeloBaseSimple(IdentidadActual, ProyectoSeleccionado.Clave,UrlIntragnoss);


                //Actualizar los datos de las personas en la tabla Perfil.
                #region Actualizar cola GnossLIVE

                LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                LiveDS liveDS = new LiveDS();
                //"PersonaID = '" + mControladorBase.UsuarioActual.PersonaID + "'"
                foreach (Perfil filaPerfil in IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(mControladorBase.UsuarioActual.PersonaID)).ToList())
                { //Guardo una fila para cada perfil
                    try
                    {
                        mControladorBase.InsertarFilaEnColaRabbitMQ(IdentidadActual.FilaIdentidad.ProyectoID, filaPerfil.PerfilID, (int)AccionLive.PerfilEditado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta);
                    }
                    catch(Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                        liveDS.Cola.AddColaRow(IdentidadActual.FilaIdentidad.ProyectoID, filaPerfil.PerfilID, (int)AccionLive.PerfilEditado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta, null);
                    }
                    
                    //liveDS.ColaHomePerfil.AddColaHomePerfilRow(IdentidadActual.FilaIdentidad.ProyectoID, filaPerfil.PerfilID, (int)AccionLive.PerfilEditado, (int)TipoLive.Miembro, 0, DateTime.Now, (short)PrioridadLive.Alta);
                }

                liveCN.ActualizarBD(liveDS);

                liveCN.Dispose();
                liveDS.Dispose();

                #endregion

                List<string> clavesSesion = new List<string>();
                foreach (string key in Session.Keys)
                {
                    clavesSesion.Add(key);
                }
                foreach (string key in clavesSesion)
                {
                    if (key.StartsWith("FotoIdentidad_"))
                    {
                        Session.Remove(key);
                    }
                }

                //Borramos Cache de la Identidad actual
                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCL.EliminarCacheGestorIdentidad(IdentidadActual.Persona.Clave, IdentidadActual.PerfilID);
                identidadCL.Dispose();

                Session.Set("tieneDatosRegistro", true);

                return true;
            }
            else
            {
                return false;
            }
        }

        private void CargarPreferenciasRegistro()
        {
            Dictionary<Guid,KeyValuePair<string, Dictionary<Guid, string>>> listaPreferencias = new Dictionary<Guid,KeyValuePair<string, Dictionary<Guid, string>>>();

            foreach (PreferenciaProyecto filaPreferencia in FilasPreferenciaProyectos)
            {
                CategoriaTesauro categoriaPadre = GestorTesauro.ListaCategoriasTesauro[filaPreferencia.CategoriaTesauroID];

                string nombreCatPadre = categoriaPadre.Nombre[UtilIdiomas.LanguageCode];

                listaPreferencias.Add(filaPreferencia.CategoriaTesauroID, new KeyValuePair<string, Dictionary<Guid,string>>(nombreCatPadre, new Dictionary<Guid,string>()));

                foreach (CategoriaTesauro categoria in categoriaPadre.Hijos)
                { 
                    listaPreferencias[filaPreferencia.CategoriaTesauroID].Value.Add(categoria.Clave, categoria.Nombre[UtilIdiomas.LanguageCode]);
                }
            }

            paginaModel.ListPreferences = listaPreferencias;
        }

        private void CargarDatosRegistro()
        {
            paginaModel.Name = IdentidadActual.Persona.Nombre;
            paginaModel.LastName = IdentidadActual.Persona.Apellidos;

            paginaModel.AskCountry = paginaModel.AskLocation = MostrarDatosDemograficosPerfil; //valor de inicio
            paginaModel.AskRegion = false;

            string foto = UtilArchivos.ContentImagenes + IdentidadActual.UrlImagenGrande;
            if (!foto.Contains("anonimo"))
            {
                paginaModel.Foto = UtilArchivos.ContentImagenes + IdentidadActual.UrlImagenGrande;
            }

            if (MostrarDatosDemograficosPerfil)
            {
                paginaModel.Location = IdentidadActual.Persona.Localidad;

                if (IdentidadActual.Persona.ProvinciaID.Equals(Guid.Empty))
                {
                    paginaModel.Region = IdentidadActual.Persona.Provincia;
                }
                else
                {
                    paginaModel.RegionID = IdentidadActual.Persona.ProvinciaID;
                }
                
                paginaModel.CountryID = IdentidadActual.Persona.PaisID;

                if (paginaModel.CountryID.Equals(Guid.Empty))
                {
                    //Cargar el pais por defecto
                    paginaModel.CountryDefaultID = PaisesDW.ListaPais.Where(item => item.Nombre.Equals("España")).FirstOrDefault().PaisID;
                }

                paginaModel.CountryList = new Dictionary<Guid, string>();
                foreach (AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
                {
                    paginaModel.CountryList.Add(filaPais.PaisID, filaPais.Nombre);
                    //if (filaPais.Nombre.Equals("España"))
                    //{
                    //    paginaModel.RegionList = new Dictionary<Guid, string>();
                    //    foreach (PaisDS.ProvinciaRow fila in PaisesDS.Provincia.Where(provincia => provincia.PaisID == filaPais.PaisID))
                    //    {
                    //        paginaModel.RegionList.Add(fila.ProvinciaID, fila.Nombre);
                    //    }
                    //}
                }
            }

            CargarDatosGenericosRegistro();

            if (!string.IsNullOrEmpty(IdentidadActual.Persona.Sexo))
            {
                paginaModel.Gender = IdentidadActual.Persona.Sexo;
            }
            else
            {
                paginaModel.Gender = "0";
            }

            bool esPrimerRegistro = string.IsNullOrEmpty(IdentidadActual.Persona.Localidad);

            if (ParametrosGeneralesRow.PrivacidadObligatoria && esPrimerRegistro)
            {
                List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("VisibilidadPerfil")).ToList();
                // if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'VisibilidadPerfil'").Length == 1)
                if (busqueda.Count == 1)
                {
                    //cadena de 4 digitos, los dos primeros configuran visibilidad en la comunidad, los dos últimos visibilidad en todo internet
                    //de cada pareja de digitos, el primero indica si el check es visible y el segundo si está marcado
                    //ej.: 0111 -> es visible únicamente el check de "visible en todo internet" pero ambos están marcados
                    string visibilidadPerfil = busqueda.First().Valor;

                    if (visibilidadPerfil[0].ToString() == "1")
                    {
                        paginaModel.IsSearched = visibilidadPerfil[1].ToString() == "1";
                    }
                    if (visibilidadPerfil[2].ToString() == "1")
                    {
                        paginaModel.IsExternalSearched = visibilidadPerfil[3].ToString() == "1";
                    }
                }
                else
                {
                    paginaModel.IsSearched = IdentidadActual.Persona.FilaPersona.EsBuscable;
                    paginaModel.IsExternalSearched = IdentidadActual.Persona.FilaPersona.EsBuscableExternos;
                }
            }

            CargarCamposExtraRegistro();
        }

        private void CargarDatosGenericosRegistro()
        {
            foreach (CamposRegistroProyectoGenericos fila in DatosExtraProyectoDataWrapperProyecto.ListaCamposRegistroProyectoGenericos)
            {
                switch (fila.Tipo)
                {
                    case (short)TipoCampoGenericoRegistro.Pais:
                        //Si esta configurado que no hay datos demograficos. pero esta configurado el pais en la tabla 'CamposRegistroProyectoGenericos'
                        if (!MostrarDatosDemograficosPerfil)
                        {
                            paginaModel.AskCountry = true;
                            if (ParametroProyecto.ContainsKey(ParametroAD.IdiomaRegistroDefecto))
                            {
                                Guid id;
                                if (Guid.TryParse(ParametroProyecto[ParametroAD.IdiomaRegistroDefecto], out id))
                                {
                                    paginaModel.CountryID = id;
                                }
                            }
                            if (paginaModel.CountryList == null || paginaModel.CountryList.Count == 0)
                            {
                                paginaModel.CountryList = new Dictionary<Guid, string>();
                                foreach (AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
                                {
                                    paginaModel.CountryList.Add(filaPais.PaisID, filaPais.Nombre);

                                    if ((paginaModel.CountryID == null || paginaModel.CountryID.Equals(Guid.Empty)) && filaPais.Nombre.Equals("España"))
                                    {
                                        paginaModel.CountryID = filaPais.PaisID;
                                    }
                                }
                            }
                        }
                        break;

                    case (short)TipoCampoGenericoRegistro.Provincia:
                        CargarDatosProvincia(paginaModel.CountryID);
                        paginaModel.AskRegion = true;
                        break;

                    case (short)TipoCampoGenericoRegistro.Localidad:
                        paginaModel.AskLocation = true;
                        break;
                    case (short)TipoCampoGenericoRegistro.Sexo:
                        paginaModel.AskGender = true;
                        break;
                }
            }
        }

        private void CargarDatosProvincia(Guid pPaisID)
        {
            paginaModel.RegionList = new Dictionary<Guid, string>();
            foreach (AD.EntityModel.Models.Pais.Provincia fila in PaisesDW.ListaProvincia.Where(provincia => provincia.PaisID == pPaisID))
            {
                paginaModel.RegionList.Add(fila.ProvinciaID, fila.Nombre);
            }
        }

        private void CargarCamposExtraRegistro()
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();
            if (IdentidadActual.TrabajaConOrganizacion || IdentidadActual.TrabajaPersonaConOrganizacion || IdentidadActual.OrganizacionID.HasValue)
            {
                DataWrapperIdentidad identidadesPersonas = identidadCN.ObtenerPerfilesDePersona(IdentidadActual.PersonaID.Value, true);
                Identidad identidad = identidadesPersonas.ListaIdentidad.FirstOrDefault(ident => ident.Tipo == 0);
                if (identidad != null)
                {
                    identidadDW = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(identidad.IdentidadID);
                }
                else
                {
                    identidadDW = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);
                }
            }
            else
            {
                identidadDW = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);
            }

            identidadCN.Dispose();

            List<AdditionalFieldAutentication> CamposExtra = new List<AdditionalFieldAutentication>();

            if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.Count > 0 || DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.Count > 0)
            {
                foreach (DatoExtraEcosistema fila in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.OrderBy(dato=>dato.Orden))
                {
                    Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();
                    foreach (DatoExtraEcosistemaOpcion filaDatoExtraEcosistemaOpcion in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaOpcion.Where(dato=>dato.DatoExtraID.Equals(fila.DatoExtraID)).OrderBy(dato=>dato.Orden))
                    {
                        Guid opcionID = filaDatoExtraEcosistemaOpcion.OpcionID;
                        string opcion = filaDatoExtraEcosistemaOpcion.Opcion;

                        listaOpciones.Add(opcionID, opcion);
                    }

                    Guid opcionSeleccionada = Guid.Empty;
                    foreach (DatoExtraEcosistemaOpcionPerfil filaDatoExtraEcosistemaOpcionPerfil in identidadDW.ListaDatoExtraEcosistemaOpcionPerfil)
                    {
                        if (filaDatoExtraEcosistemaOpcionPerfil.DatoExtraID == fila.DatoExtraID)
                        {
                            opcionSeleccionada = filaDatoExtraEcosistemaOpcionPerfil.OpcionID;
                            break;
                        }
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    campoExtra.FieldName = fila.DatoExtraID.ToString();
                    campoExtra.Required = fila.Obligatorio;
                    campoExtra.Options = listaOpciones;
                    campoExtra.FieldValue = opcionSeleccionada.ToString();

                    CamposExtra.Add(campoExtra);
                }

                foreach (DatoExtraProyecto fila in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.OrderBy(dato=>dato.Orden))
                {
                    Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();//"DatoExtraID='" + fila.DatoExtraID.ToString() + "'", "orden asc"
                    foreach (DatoExtraProyectoOpcion filaDatoExtraProyectoOpcion in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoOpcion.Where(dato=>dato.DatoExtraID.Equals(fila.DatoExtraID)))
                    {
                        Guid opcionID = filaDatoExtraProyectoOpcion.OpcionID;
                        string opcion = UtilCadenas.ObtenerTextoDeIdioma(filaDatoExtraProyectoOpcion.Opcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

                        listaOpciones.Add(opcionID, opcion);
                    }

                    Guid opcionSeleccionada = Guid.Empty;
                    foreach (DatoExtraProyectoOpcionIdentidad filaDatoExtraOpcionIdentidad in identidadDW.ListaDatoExtraProyectoOpcionIdentidad)
                    {
                        if (filaDatoExtraOpcionIdentidad.ProyectoID == fila.ProyectoID && filaDatoExtraOpcionIdentidad.DatoExtraID == fila.DatoExtraID)
                        {
                            opcionSeleccionada = filaDatoExtraOpcionIdentidad.OpcionID;
                        }
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    campoExtra.FieldName = fila.DatoExtraID.ToString();
                    campoExtra.Required = fila.Obligatorio;
                    campoExtra.Options = listaOpciones;
                    campoExtra.FieldValue = opcionSeleccionada.ToString();

                    CamposExtra.Add(campoExtra);
                }
            }

            if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Count > 0 || DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Count > 0)
            {
                foreach (DatoExtraEcosistemaVirtuoso fila in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.OrderBy(dato=>dato.Orden))
                {
                    string opcionSeleccionada = "";
                    List<DatoExtraEcosistemaVirtuosoPerfil> listaDatoExtraEcosistemaVirtosoPerf = identidadDW.ListaDatoExtraEcosistemaVirtuosoPerfil.Where(item => item.DatoExtraID.Equals(fila.DatoExtraID)).ToList();
                    if (listaDatoExtraEcosistemaVirtosoPerf.Count > 0)
                    {
                        opcionSeleccionada = UtilCadenas.ObtenerTextoDeIdioma(listaDatoExtraEcosistemaVirtosoPerf.First().Opcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    campoExtra.FieldName = fila.InputID;
                    campoExtra.Required = fila.Obligatorio;
                    if (fila.InputID.Equals("ddlPais"))
                    {
                        Dictionary<Guid, string> listaOpciones = new Dictionary<Guid, string>();
                        foreach (AD.EntityModel.Models.Pais.Pais filaPais in PaisesDW.ListaPais)
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

                foreach (DatoExtraProyectoVirtuoso fila in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.OrderBy(dato=>dato.Orden))
                {
                    string opcionSeleccionada = "";
                    if (identidadDW.ListaDatoExtraProyectoVirtuosoIdentidad.Any(dato=>dato.DatoExtraID.Equals(fila.DatoExtraID)))
                    {
                        opcionSeleccionada = UtilCadenas.ObtenerTextoDeIdioma(identidadDW.ListaDatoExtraProyectoVirtuosoIdentidad.First(dato => dato.DatoExtraID.Equals(fila.DatoExtraID)).Opcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    }

                    AdditionalFieldAutentication campoExtra = new AdditionalFieldAutentication();
                    campoExtra.Title = UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
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

            paginaModel.AdditionalFields = CamposExtra;
        }

        private bool CargarConectaRegistro()
        {
            UsuariosRecomendadosController usuariosRecomendadosController = new UsuariosRecomendadosController(this, IdentidadActual, mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mHttpContextAccessor, mGnossCache, mServicesUtilVirtuosoAndReplication);
            List<ProfileModel> listaPerfiles = usuariosRecomendadosController.ObtenerUsuariosRecomendados(8);
            if (listaPerfiles.Count > 0)
            {
                paginaModel.ListProfiles = listaPerfiles;
                return true;
            }
            else
            {
                return false;
            }
        }

        private string IntentoObtenerElPais(string pValor)
        {
            Guid paisID = new Guid();

            if (Guid.TryParse(pValor, out paisID))
            {
                foreach (AD.EntityModel.Models.Pais.Pais fila in PaisesDW.ListaPais)
                {
                    if (fila.PaisID == paisID)
                    {
                        pValor = fila.Nombre;
                    }
                }
            }

            return pValor;
        }


        private GestionTesauro GestorTesauro
        {
            get
            {
                if (mGestorTesauro == null)
                {
                    TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mGestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                }
                return mGestorTesauro;
            }
        }

        private List<PreferenciaProyecto> FilasPreferenciaProyectos
        {
            get
            {
                if(mFilasPreferenciaProyectos == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperProyecto proyectoDS = proyectoCN.ObtenerPreferenciasProyectoPorID(ProyectoSeleccionado.Clave);

                    mFilasPreferenciaProyectos = proyectoDS.ListaPreferenciaProyecto;
                }
                return mFilasPreferenciaProyectos;
            }
        }

        public DataWrapperProyecto DatosExtraProyectoDataWrapperProyecto
        {
            get
            {
                if (mDatosExtraProyectoDataWrapperProyecto == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mDatosExtraProyectoDataWrapperProyecto = proyectoCN.ObtenerDatosExtraProyectoPorID(ProyectoSeleccionado.Clave);
                    proyectoCN.Dispose();
                }
                return mDatosExtraProyectoDataWrapperProyecto;
            }
        }

        private bool PestanyaPreferencias
        {
            get
            {
                //TRUE si hay preferencias y (o no hay nada en ProyectoPasoRegistro o existe una fila con Preferencias en ProyectoPasoRegistro )
                return FilasPreferenciaProyectos.Count > 0 && (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Count == 0 || ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Any(proy=>proy.PasoRegistro.Equals(PasosRegistro.Preferencias.ToString())));
            }
        }

        private bool PestanyaDatos
        {
            get
            {
                if (mPestanyaDatos == null)
                {
                    /*Existe alguna configuracion en ProyectoPasoRegistro*/
                    if(ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Count > 0)
                    {
                        /*Existe una fila con DATOS en ProyectoPasoRegistro*///"PasoRegistro='" + PasosRegistro.Datos + "'"
                        if (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Any(proy=>proy.PasoRegistro.Equals(PasosRegistro.Datos.ToString())))
                        {
                            mPestanyaDatos=true;
                        }
                        else
                        {
                            mPestanyaDatos=false;
                        }
                    }
                    else
                    {
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();
                        if (IdentidadActual.TrabajaConOrganizacion || IdentidadActual.TrabajaPersonaConOrganizacion || IdentidadActual.OrganizacionID.HasValue)
                        {
                            DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerPerfilesDePersona(IdentidadActual.PersonaID.Value, true);
                            if (dataWrapperIdentidad.ListaIdentidad.Any(ident => ident.Tipo==0))
                            {
                                identidadDW = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(dataWrapperIdentidad.ListaIdentidad.FirstOrDefault(ident => ident.Tipo == 0).IdentidadID);
                            }
                            else
                            {
                                identidadDW = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);
                            }

                        }
                        else
                        {
                            identidadDW = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(IdentidadActual.Clave);

                        }
                        identidadCN.Dispose();

                        if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyecto.Count > 0 || DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Count > 0)
                        {
                            //Si tiene datos Propios de comunidad la pestaña de datos estara siempre visible
                            return true;
                        }

                        if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.Count > 0)
                        {
                            #region Carga de los datos Extra del ecosistema
                            foreach (DatoExtraEcosistema filaDatoExtra in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistema.OrderBy(dato=>dato.Orden))
                            {
                                DatoExtraEcosistema filaDatoExtraEcosistema = filaDatoExtra;

                                Guid opcionSeleccionada = Guid.Empty;
                                foreach (DatoExtraEcosistemaOpcionPerfil filaDatoExtraEcosistemaOpcionPerfil in identidadDW.ListaDatoExtraEcosistemaOpcionPerfil)
                                {
                                    if (filaDatoExtraEcosistemaOpcionPerfil.DatoExtraID == filaDatoExtraEcosistema.DatoExtraID)
                                    {
                                        opcionSeleccionada = filaDatoExtraEcosistemaOpcionPerfil.OpcionID;
                                    }
                                }

                                if (filaDatoExtraEcosistema.Obligatorio && opcionSeleccionada == Guid.Empty)
                                {
                                    //Si hay datos del ecosistema obligatorios y no tiene una opcion seleccionada
                                    return true;
                                }
                            }
                            #endregion
                        }

                        if (DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Count > 0)
                        {
                            #region Carga de los datos extra del ecosistema en virtuoso

                            foreach (DatoExtraEcosistemaVirtuoso filaDatoExtraEcosistemaVirtuoso in DatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.OrderBy(dato=>dato.Orden))
                            {
                                string opcionSeleccionada = "";
                                if (identidadDW.ListaDatoExtraEcosistemaVirtuosoPerfil.Any(proyecto=>proyecto.DatoExtraID.Equals(filaDatoExtraEcosistemaVirtuoso.DatoExtraID)))
                                {
                                    opcionSeleccionada = identidadDW.ListaDatoExtraEcosistemaVirtuosoPerfil.First(proyecto => proyecto.DatoExtraID.Equals(filaDatoExtraEcosistemaVirtuoso.DatoExtraID)).Opcion;
                                }
                                if (filaDatoExtraEcosistemaVirtuoso.Obligatorio && string.IsNullOrEmpty(opcionSeleccionada))
                                {
                                    //Si hay datos del ecosistema obligatorios y no tiene una opcion seleccionada
                                    return true;
                                }
                            }
                            #endregion
                        }

                        bool tienePoblacion = !string.IsNullOrEmpty(IdentidadActual.Persona.Localidad);
                        bool tienePais = IdentidadActual.Persona.FilaPersona.PaisPersonalID.HasValue && IdentidadActual.Persona.FilaPersona.PaisPersonalID != Guid.Empty;
                        bool datosDemograficosCompletos = (!MostrarDatosDemograficosPerfil || tienePais) && (!MostrarDatosDemograficosPerfil || tienePoblacion);

                        mPestanyaDatos = (!datosDemograficosCompletos || mTieneDatosRegistro);
                    }
                }
                return mPestanyaDatos.Value;
            }
        }

        private bool PestanyaConecta
        {
            get{
                return ((FilasPreferenciaProyectos.Count > 0 && (!ProyectoSeleccionado.EsCatalogo || ParametrosGeneralesRow.MostrarPersonasEnCatalogo)) && (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Count == 0 || ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Any(proy=>proy.PasoRegistro.Equals(PasosRegistro.Conecta.ToString())))); 
            }
        }

        public string UrlOrigen
        {
            get
            {
                string urlOrigen = paginaModel.ReferrerPage;
                if (string.IsNullOrEmpty(urlOrigen))
                {
                    urlOrigen = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
                }

                if (urlOrigen.EndsWith("?inicio=1") || urlOrigen.EndsWith("&inicio=1"))
                {
                    if (urlOrigen.Contains("?"))
                    {
                        urlOrigen = urlOrigen.Replace("?inicio=1", "");
                    }
                    else
                    {
                        urlOrigen = urlOrigen.Replace("&inicio=1", "");
                    }
                }

                string urlRedirectHome = ObtenerUrlHomeConectado();
                
                string urlComunidad = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);

                // Si se ha aceptado una invitación, mandar a la página del perfil
                //bool urlOrigen_AceptarInvitacion = urlOrigen.Contains(UtilIdiomas.GetText("URLSEM", "ACEPTARINVITACION"));

                //if (!string.IsNullOrEmpty(urlRedirectHome) && (urlComunidad == urlOrigen || urlOrigen_AceptarInvitacion))
                if (!string.IsNullOrEmpty(urlRedirectHome) && urlComunidad == urlOrigen )
                {
                    urlOrigen = urlRedirectHome;
                }

                if (!urlOrigen.EndsWith("?inicio=1") && !urlOrigen.EndsWith("&inicio=1"))
                {
                    if (urlOrigen.Contains("?"))
                    {
                        urlOrigen += "&inicio=1";
                    }
                    else
                    {
                        urlOrigen += "?inicio=1";
                    }
                }
                return urlOrigen;
            }
        }

        public string UrlSiguientePaso
        {
            get
            {
                int numPaso = int.Parse(RequestParams("paso")) + 1;

                string urlSiguiente = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "REGISTROUSUARIO") + "/" + (numPaso);

                bool obligatorio = true;

                if (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.Count > 0)
                {
                    List<ProyectoPasoRegistro> filasPasos = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPasoRegistro.OrderBy(proy=>proy.Orden).ToList();

                    ProyectoPasoRegistro filaSiguiente = null;

                    if (filasPasos.Count > numPaso - 1)
                    {
                        filaSiguiente = filasPasos[numPaso -1];
                    }

                    if (filaSiguiente != null )
                    {
                        PasosRegistro pasoRegistroActual;

                        bool esPasoPredefinido = Enum.TryParse<PasosRegistro>(filaSiguiente.PasoRegistro, out pasoRegistroActual);

                        if (!esPasoPredefinido)
                        {
                            urlSiguiente = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto) + "/" + filaSiguiente.PasoRegistro + "?referer=" + UrlOrigen;
                        }
                        obligatorio = filaSiguiente.Obligatorio;
                    }
                    else
                    {
                        urlSiguiente = UrlOrigen;
                    }           
                }
                else {
                    if (!PestanyaPreferencias) { numPaso++; }
                    if (!PestanyaDatos && numPaso > 1) { numPaso++; }
                    if (!PestanyaConecta && numPaso > 2) { numPaso++; }
                    if (numPaso > 3)
                    {
                        Session.Remove("tienePaginaDatosRegistro");
                        urlSiguiente = UrlOrigen;
                    }
                }

                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                if(urlSiguiente == UrlOrigen || !obligatorio)
                {
                    usuarioCN.EliminarUrlRedirect(mControladorBase.UsuarioActual.UsuarioID);
                    if (!string.IsNullOrEmpty(UrlRedireccionTrasRegistro) && !UrlRedireccionTrasRegistro.Contains("/login"))
                    {
                        urlSiguiente = UrlRedireccionTrasRegistro;
                    }
                }
                else
                {
                    usuarioCN.ModificarUrlRedirect(mControladorBase.UsuarioActual.UsuarioID, urlSiguiente);
                }
                usuarioCN.Dispose();
                


                return urlSiguiente;
            }
        }


        public string UrlRedireccionTrasRegistro
        {
            get
            {
                if (mUrlRedireccionTrasRegistro == null)
                {
                    string cookieValue = Request.Cookies["reg-redirect"];
                    //skp-stps, acrónimo de skip-steps
                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        mUrlRedireccionTrasRegistro = cookieValue;
                        //cookie.Domain = mControladorBase.DominoAplicacion;
                        mControladorBase.ExpirarCookie("reg-redirect");
                    }
                }
                return mUrlRedireccionTrasRegistro;
            }
        }
    }
}
