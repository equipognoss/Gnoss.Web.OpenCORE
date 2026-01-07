using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.RelatedVirtuoso;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.FirstDataLoad;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC;
using Es.Riam.Gnoss.Web.MVC.Controllers;
using Es.Riam.Gnoss.Web.OAuthAD;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.InterfacesOpen;
using Es.Riam.InterfacesOpenArchivos;
using Es.Riam.Open;
using Es.Riam.OpenArchivos;
using Es.Riam.OpenReplication;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using Gnoss.Web.App_Start;
using Gnoss.Web.Middlewares;
using Gnoss.Web.Services;
using Gnoss.Web.Services.VirtualPathProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Gnoss.Web
{
    public class Startup
    {
        private const int _minKestrelMaxRequestLineSize = 8192;
        private const int _maxKestrelMaxRequestLineSize = 24576;

        private RouteConfig mRouteConfig;

        private string IdiomaPrincipalDominio = "es";
        private IPublishEvents mIPublishEvents;

        private static string mDominio = null;
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            mDominio = environment.ApplicationName;
            Configuration = configuration;
            mEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment mEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            bool cargado = false;
			ILoggerFactory loggerFactory =
			LoggerFactory.Create(builder =>
			{
				builder.AddConfiguration(Configuration.GetSection("Logging"));
				builder.AddSimpleConsole(options =>
				{
					options.IncludeScopes = true;
					options.SingleLine = true;
					options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
					options.UseUtcTimestamp = true;
				});
			});

			services.AddSingleton(loggerFactory);

            var assemblyOpen = Assembly.Load("Gnoss.Web.Open");
            var externalControllerOpen = new AssemblyPart(assemblyOpen);
            // ApplicationPartManager
            services.AddControllers()
                    .ConfigureApplicationPartManager(apm =>
                    {
                        apm.ApplicationParts.Add(externalControllerOpen);
                    });
			services.Configure<FormOptions>(x =>
			{
				x.ValueLengthLimit = 524288000;
				x.MultipartBodyLengthLimit = 524288000; // In case of multipart
			});
			services.AddControllers();
            services.AddScoped(typeof(UtilTelemetry));
            services.AddScoped(typeof(Usuario));
            services.AddScoped(typeof(UtilPeticion));
            services.AddScoped(typeof(Conexion));
            services.AddScoped(typeof(UtilGeneral));
            services.AddScoped(typeof(LoggingService));
            if (Environment.GetEnvironmentVariables().Contains("redisCacheSingleton"))
            {
                services.AddSingleton(typeof(RedisCacheWrapper));
            }
            else
            {
                services.AddScoped(typeof(RedisCacheWrapper));
            }
            services.AddScoped(typeof(Configuracion));
            services.AddScoped(typeof(GnossCache));
            services.AddScoped(typeof(VirtuosoAD));
            services.AddScoped(typeof(UtilServicios));
            services.AddScoped(typeof(RouteConfig));
            services.AddScoped(typeof(BDVirtualPath));
            services.AddScoped(typeof(RelatedVirtuosoCL));
            services.AddScoped<IUtilServicioIntegracionContinua, UtilServicioIntegracionContinuaOpen>();
            services.AddScoped<IServicesUtilVirtuosoAndReplication, ServicesVirtuosoAndBidirectionalReplicationOpen>();
            services.AddScoped<IMassiveOntologyToClass, MassiveOntologyToClassOpen>();
            services.AddScoped<IUtilArchivos, UtilArchivosOpen>();
            services.AddScoped<IPublishEvents, PublishEventsOpen>();
            services.AddScoped<IAvailableServices, AvailableServicesOpen>();
            services.AddScoped<IRDFSearch, RDFSearchOpen>();
            services.AddScoped<IOAuth, OAuthOpen>();
            string bdType = "";
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            if (environmentVariables.Contains("connectionType"))
            {
                bdType = environmentVariables["connectionType"] as string;
            }
            else
            {
                bdType = Configuration.GetConnectionString("connectionType");
            }
            if (bdType.Equals("2") || bdType.Equals("1"))
            {
                services.AddScoped(typeof(DbContextOptions<EntityContext>));
                services.AddScoped(typeof(DbContextOptions<EntityContextBASE>));
                services.AddScoped(typeof(DbContextOptions<EntityContextOauth>));
            }
            services.AddSingleton(typeof(ConfigService));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<RouteValueTransformer>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(60); // Tiempo de expiración   
                                                                //options.Cookie.Name = "AppTest";
                                                                //options.Cookie.HttpOnly = true; // correct initialization

            });
            services.AddMvc(mvc => mvc.EnableEndpointRouting = false);
            services.Configure<CookieTempDataProviderOptions>(options => {
                options.Cookie.IsEssential = true;
            });

            services.Configure<FormOptions>(options => { options.ValueCountLimit = 4096; });
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

            string acid = "";
            if (environmentVariables.Contains("acid"))
            {
                acid = environmentVariables["acid"] as string;
            }
            else
            {
                acid = Configuration.GetConnectionString("acid");
            }

            string baseConnection = "";
            if (environmentVariables.Contains("base"))
            {
                baseConnection = environmentVariables["base"] as string;
            }
            else
            {
                baseConnection = Configuration.GetConnectionString("base");
            }
            string oauthConnection = "";
            if (environmentVariables.Contains("oauth"))
            {
                oauthConnection = environmentVariables["oauth"] as string;
            }
            else
            {
                oauthConnection = Configuration.GetConnectionString("oauth");
            }
			var sp = services.BuildServiceProvider();
			var loggingService = sp.GetService<LoggingService>();
            AgregarEntityContextService(bdType, services);
            sp = services.BuildServiceProvider();
            // Resolve the services from the service provider
            var virtualProvider = sp.GetService<BDVirtualPath>();
            var servicesUtilVirtuosoAndReplication = sp.GetService<IServicesUtilVirtuosoAndReplication>();
            while (!cargado)
            {
                try
                {
                    services.AddRazorPages().AddRazorRuntimeCompilation();
                    services.AddControllersWithViews().AddRazorRuntimeCompilation();
                    services.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
                    {

                        opts.FileProviders.Add(
                            new BDFileProvider(loggingService, virtualProvider));
                    });
                    cargado = true;
                }
                catch (Exception)
                {
                    cargado = false;
                }
            }
            var configService = sp.GetService<ConfigService>();
            string redisConnection = configService.ObtenerConexionRedisIPMaster("redis");
            configService.ObtenerProcesarStringGrafo();
            int workerThreads = configService.ObtenerWorkerThreads();
            int completionPortThreads = configService.ObtenerPortThreads();
            if (completionPortThreads > 0 || workerThreads > 0)
            {
                System.Threading.ThreadPool.SetMinThreads(workerThreads, completionPortThreads);
            }
            var options = ConfigurationOptions.Parse(redisConnection);
            options.ClientName = $"{AppDomain.CurrentDomain.FriendlyName}_DataProtectionKeys_{Environment.GetEnvironmentVariable("COMPOSE_PROJECT_NAME")}_{Guid.NewGuid()}"; ;
            options.SocketManager = SocketManager.ThreadPool;
            var redis = ConnectionMultiplexer.Connect(options);
            services.AddDataProtection().PersistKeysToStackExchangeRedis(redis, "DataProtectionKeys");
            if (!redisConnection.Contains("name"))
            {
                redisConnection += $",name={AppDomain.CurrentDomain.FriendlyName}_Session_{Environment.GetEnvironmentVariable("COMPOSE_PROJECT_NAME")}_{Guid.NewGuid()}";
            }
            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = redisConnection;
            });
            ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.ViewLocationFormats.Add("/Views/Administracion/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/Views/CMSPagina/{0}" + RazorViewEngine.ViewExtension);
            });

            int maxRequestLineSize = 0;
            string maxRequestLineSizeString = string.Empty;
            if (environmentVariables.Contains("Kestrel__maxRequestLineSize"))
            {
                maxRequestLineSizeString = environmentVariables["Kestrel__maxRequestLineSize"] as string;
            }
            else
            {
                maxRequestLineSizeString = Configuration.GetSection("Kestrel")["maxRequestLineSize"];
            }

            if (!string.IsNullOrWhiteSpace(maxRequestLineSizeString))
            {
                if (int.TryParse(maxRequestLineSizeString, out maxRequestLineSize))
                {
                    if (maxRequestLineSize >= _minKestrelMaxRequestLineSize && maxRequestLineSize <= _maxKestrelMaxRequestLineSize)
                    {
                        services.Configure<KestrelServerOptions>(options =>
                        {
                            options.Limits.MaxRequestLineSize = maxRequestLineSize;
                        }
                        );
                    }
                    else
                    {
                        loggingService.GuardarLogError($"La variable 'Kestrel__maxRequestLineSize' solo acepta valores entre {_minKestrelMaxRequestLineSize} y {_maxKestrelMaxRequestLineSize}.");
                    }
                }
                else
                {
                    loggingService.GuardarLogError("La variable 'Kestrel__maxRequestLineSize' se encuentra configurada pero su valor no es valido.");
                }
            }

            UtilTelemetry utilTelemetry = sp.GetService<UtilTelemetry>();
            loggingService.AgregarEntrada("INICIO Application_Start");
            LoggingService.RUTA_DIRECTORIO_ERROR = Path.Combine(mEnvironment.ContentRootPath, "logs");
            loggingService.GuardarLogError("Application_Start", loggerFactory.CreateLogger<Startup>());
            loggingService.GuardarLogError("Application_Start");
            // Resolve the services from the service provider

            var entity = sp.GetService<EntityContext>();
            var entityBASE = sp.GetService<EntityContextBASE>();
            var entityOauth = sp.GetService<EntityContextOauth>();
			var redisCacheWrapper = sp.GetService<RedisCacheWrapper>();
			try
			{
				entity.Migrate();
				entityBASE.Migrate();
				entityOauth.Migrate();
			}
			catch (Exception ex)
			{
				if (ex is AggregateException)
				{
					foreach (Exception aggregateException in ((AggregateException)ex).InnerExceptions)
					{
						loggingService.GuardarLogError(aggregateException);
					}
				}
				loggingService.GuardarLogError(ex);
				loggingService.GuardarTraza(ObtenerRutaTraza());
				throw;
			}

			string configLogStash = configService.ObtenerLogStashConnection();
            if (!string.IsNullOrEmpty(configLogStash))
            {
                LoggingService.InicializarLogstash(configLogStash);
            }
            FirstDataLoad firstDataLoad = new FirstDataLoad(entity, configService, entityOauth, loggingService, loggerFactory.CreateLogger<FirstDataLoad>(), loggerFactory);
            firstDataLoad.InsertDataIfPossible();
            firstDataLoad.InsertRoles();
            mIPublishEvents = sp.GetService<IPublishEvents>();
            CrearColaEventosInternos();
            BaseCL.UsarCacheLocal = UsoCacheLocal.SiElServicioLoUsanPocosProyectos;
            string rutaVersionCacheLocal = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/config/versionCacheLocal/";
            if (!Directory.Exists(rutaVersionCacheLocal)) { Directory.CreateDirectory(rutaVersionCacheLocal); }
            EstablecerDominioCache(entity);

            CargarDominio(configService);
            RabbitMQClient.ClientName = $"WEB_{mDominio}";

            CargarIdiomasPlataforma(entity, loggingService, configService, servicesUtilVirtuosoAndReplication, redisCacheWrapper, loggerFactory);

            CargarTextosPersonalizadosDominio(entity, loggingService, configService, redisCacheWrapper, loggerFactory);

            ConfigurarApplicationInsights(configService, entity, utilTelemetry);

            CargarConfiguracionReplicado(entity);

            Conexion.CargarMimeType();

            // Resolve the services from the service provider
            mRouteConfig = sp.GetService<RouteConfig>();
            RouteConfig.IdiomaPrincipalDominio = IdiomaPrincipalDominio;
            GnossUrlsSemanticas.IdiomaPrincipalDominio = IdiomaPrincipalDominio;

            ProyectoAD.MetaOrganizacion = EstablecerOrganizacionGnoss(configService);
            ProyectoAD.MetaProyecto = EstablecerMetaProyecto(configService);
            loggingService.AgregarEntrada("FIN Application_Start");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ConfigService configService, IApplicationBuilder applicationBuilder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseReferrerPolicy(opts => opts.NoReferrerWhenDowngrade());
            app.UseXContentTypeOptions();
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseErrorGnossMidleware();
            string rutaEjecucionWeb = configService.ObtenerRutaEjecucionWeb();
            if (!string.IsNullOrEmpty(rutaEjecucionWeb))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = $"/{rutaEjecucionWeb.Trim('/')}"
                });
            }
            else
            {
                app.UseStaticFiles();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseGnossMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{*slug}");
                endpoints.MapRazorPages();
            });
            app.UseMvc(route =>
            {
                MapearRutas(route, configService, applicationBuilder);
            });

        }
        /// <summary>
        /// Establece el dominio de la cache.
        /// </summary>
        private void EstablecerDominioCache(EntityContext entity)
        {
            string dominio = entity.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals("UrlIntragnoss")).FirstOrDefault().Valor;
            GestionOWL.URLIntragnoss = dominio;

            dominio = dominio.Replace("http://", "").Replace("https://", "").Replace("www.", "");

            if (dominio[dominio.Length - 1] == '/')
            {
                dominio = dominio.Substring(0, dominio.Length - 1);
            }

            BaseCL.DominioEstatico = dominio;
        }
        public static string Dominio
        {
            get
            {
                return mDominio;
            }
        }
		protected string ObtenerRutaTraza()
		{
			string ruta = Path.Combine(mEnvironment.ContentRootPath, "trazas");

			if (!Directory.Exists(ruta))
			{
				Directory.CreateDirectory(ruta);
			}

			ruta = Path.Combine(ruta, $"traza_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");

			return ruta;
		}
        private void CargarIdiomasPlataforma(Es.Riam.Gnoss.AD.EntityModel.EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, RedisCacheWrapper redisCacheWrapper, ILoggerFactory loggerFactory)
        {
            Dictionary<string, string> listaIdiomas = new Dictionary<string, string>();
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<ParametroAplicacionCL>(), loggerFactory);
            listaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
            if (listaIdiomas.Count == 1 || string.IsNullOrEmpty(Dominio))
            {
                IdiomaPrincipalDominio = listaIdiomas.First().Key;
            }
            else if (listaIdiomas.Count > 1)
            {
                ProyectoCN proyCN = new ProyectoCN(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<ProyectoCN>(), loggerFactory);
                IdiomaPrincipalDominio = proyCN.ObtenerIdiomaPrincipalDominio(Dominio);//select proyectoid, URLPropia from Proyecto where URLPropia like '%pruebasiphoneen.gnoss.net@%'

                if (!listaIdiomas.ContainsKey(IdiomaPrincipalDominio))
                {
                    IdiomaPrincipalDominio = listaIdiomas.First().Key;
                }
            }
        }

		private void CargarDominio(ConfigService configService)
        {
            string dominioConfig = configService.ObtenerDominio();
            if (!string.IsNullOrEmpty(dominioConfig))
            {
                mDominio = dominioConfig;
            }

            if (mDominio.Contains("depuracion.net"))
            {
                mDominio = "";
            }

        }

        private Guid EstablecerMetaProyecto(ConfigService configService)
        {
            Guid? proyectoID = configService.ObtenerProyectoGnoss();

            if (!proyectoID.HasValue)
            {
                return ProyectoAD.MyGnoss;
            }
            return proyectoID.Value;
        }

        private Guid EstablecerOrganizacionGnoss(ConfigService configService)
        {
            Guid? organizacionID = configService.ObtenerOrganizacionGnoss();

            if (!organizacionID.HasValue)
            {
                return ProyectoAD.MyGnoss;
            }
            return organizacionID.Value;
        }


        private void ConfigurarApplicationInsights(ConfigService configService, EntityContext entityContext, UtilTelemetry utilTelemetry)
        {
            string dominio = mEnvironment.ApplicationName;

            string dominioConfig = configService.ObtenerDominio();
            if (!string.IsNullOrEmpty(dominioConfig))
            {
                dominio = dominioConfig;
            }

            UtilTelemetry.ModoDepuracion = false;

            if (!string.IsNullOrEmpty(dominio) && dominio.Equals("depuracion.net"))
            {
                UtilTelemetry.ModoDepuracion = true;
            }

            ConfigApplicationInsightsDominio filaConfigAppInsightsDominio = null;
            //ParametroAplicacionDS.ParametroAplicacionRow filaConfigAppInsights = null;
            ParametroAplicacion filaConfigAppInsights = null;
            List<ConfigApplicationInsightsDominio> busquedaInsight = entityContext.ConfigApplicationInsightsDominio.Where(configAppIns => configAppIns.Dominio.Equals(dominio)).ToList();

            if (busquedaInsight.Count > 0)
            {
                filaConfigAppInsightsDominio = busquedaInsight[0];

                if (filaConfigAppInsightsDominio.ImplementationKey != null && !filaConfigAppInsightsDominio.ImplementationKey.Equals(Guid.Empty))
                {
                    Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = filaConfigAppInsightsDominio.ImplementationKey.ToString().ToLower();
                    utilTelemetry.Telemetry.InstrumentationKey = filaConfigAppInsightsDominio.ImplementationKey.ToString().ToLower();
                }

                if (UtilTelemetry.EstaConfiguradaTelemetria)
                {
                    //Configuración de los logs
                    if (Enum.IsDefined(typeof(UtilTelemetry.UbicacionLogsYTrazas), filaConfigAppInsightsDominio.UbicacionLogs))
                    {
                        LoggingService.UBICACIONLOGS = (UtilTelemetry.UbicacionLogsYTrazas)filaConfigAppInsightsDominio.UbicacionLogs;
                    }

                    //Configuración de las trazas
                    if (Enum.IsDefined(typeof(UtilTelemetry.UbicacionLogsYTrazas), filaConfigAppInsightsDominio.UbicacionTrazas))
                    {
                        LoggingService.UBICACIONTRAZA = (UtilTelemetry.UbicacionLogsYTrazas)filaConfigAppInsightsDominio.UbicacionTrazas;
                    }
                }

            }
            string valor = configService.ObtenerImplementationKeyWeb();

            if (!string.IsNullOrEmpty(valor))
            {
                Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = valor.ToLower();
            }

            if (UtilTelemetry.EstaConfiguradaTelemetria)
            {
                //Configuración de los logs

                string ubicacionLogs = configService.ObtenerUbicacionLogsResultados();

                int valorInt = 0;
                if (int.TryParse(ubicacionLogs, out valorInt))
                {
                    if (Enum.IsDefined(typeof(UtilTelemetry.UbicacionLogsYTrazas), valorInt))
                    {
                        LoggingService.UBICACIONLOGS = (UtilTelemetry.UbicacionLogsYTrazas)valorInt;
                    }
                }


                //Configuración de las trazas

                string ubicacionTrazas = configService.ObtenerUbicacionTrazasWeb();

                int valorInt2 = 0;
                if (int.TryParse(ubicacionTrazas, out valorInt2))
                {
                    if (Enum.IsDefined(typeof(UtilTelemetry.UbicacionLogsYTrazas), valorInt2))
                    {
                        LoggingService.UBICACIONTRAZA = (UtilTelemetry.UbicacionLogsYTrazas)valorInt2;
                    }
                }

            }

        }


        private void CargarTextosPersonalizadosDominio(EntityContext context, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, ILoggerFactory loggerFactory)
        {
            string dominio = configService.ObtenerDominio();
            Guid personalizacionEcosistemaID = Guid.Empty;
            List<ParametroAplicacion> parametrosAplicacionPers = context.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals(TiposParametrosAplicacion.PersonalizacionEcosistemaID.ToString())).ToList();
            if (parametrosAplicacionPers.Count > 0)
            {
                personalizacionEcosistemaID = new Guid(parametrosAplicacionPers[0].Valor.ToString());
            }
            UtilIdiomas utilIdiomas = new UtilIdiomas("", loggingService, context, configService, redisCacheWrapper, loggerFactory.CreateLogger<UtilIdiomas>(), loggerFactory);
            utilIdiomas.CargarTextosPersonalizadosDominio(dominio, personalizacionEcosistemaID);
        }

        private void CargarConfiguracionReplicado(EntityContext entityContext)
        {
            ParametroAplicacion filaReplicacion = entityContext.ParametroAplicacion.FirstOrDefault(x => x.Parametro.Equals("Replicacion"));
            if (filaReplicacion != null)
            {
                if (filaReplicacion.Valor == "1")
                {
                    FacetadoCN.Replicacion = true;
                }
                else
                {
                    FacetadoCN.Replicacion = false;
                }

            }
        }

        public void MapearRutas(IRouteBuilder routes, ConfigService configService, IApplicationBuilder applicationBuilder)
        {
            RouteConfig.IdiomaPrincipalDominio = IdiomaPrincipalDominio;
            mRouteConfig.RegistrarRutasRedireccionamiento(routes);
            mRouteConfig.RegisterRoutes(routes);
            //routes.Routes.Add(new CachedRouter<Guid>(routes.DefaultHandler, applicationBuilder, configService));
        }
        public void CrearColaEventosInternos()
        {
            mIPublishEvents.CrearCola();
        }

        private void AgregarEntityContextService(string bdType, IServiceCollection pServices)
        {
            if (bdType.Equals("0"))
            {
                pServices.AddDbContext<EntityContext>();
                pServices.AddDbContext<EntityContextBASE>();
                pServices.AddDbContext<EntityContextOauth>();
            }
            else if (bdType.Equals("1"))
            {
                pServices.AddDbContext<EntityContext, EntityContextOracle>();
                pServices.AddDbContext<EntityContextBASE, EntityContextBASEOracle>();
                pServices.AddDbContext<EntityContextOauth, EntityContextOauthOracle>();
            }
            else if (bdType.Equals("2"))
            {
                pServices.AddDbContext<EntityContext, EntityContextPostgres>();
                pServices.AddDbContext<EntityContextBASE, EntityContextBASEPostgres>();
                pServices.AddDbContext<EntityContextOauth, EntityContextOauthPostgres>();
            }
        }
    }
}
