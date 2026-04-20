using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace Gnoss.Web
{
    public class Program
    {
        private static Serilog.ILogger _startupLogger;
        public static void Main(string[] args)
        {
            _startupLogger = LoggingService.ConfigurarBasicStartupSerilog().CreateBootstrapLogger().ForContext<Program>();
            try
            {
                var host = CreateHostBuilder(args).Build();

                CargarClaveReinicioCache(host);

                host.Run();
            }
            catch (Exception ex)
            {
                _startupLogger.Fatal(ex, "Error fatal durante el arranque");
            }
            finally
            {
                (_startupLogger as IDisposable)?.Dispose();
                Log.CloseAndFlush(); // asegura que se escriben todos los logs pendientes
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    LoggingService.ConfigurarSeguimientoFicheros(hostContext, config, _startupLogger);
                })
                .UseSerilog((context, services, configuration) => LoggingService.ConfigurarSerilog(context.Configuration, services, configuration))
                .ConfigureServices((context, services) =>
                {
                    LoggingService.SuscribirCambios(context, _startupLogger);
                    _startupLogger.Information("Suscripción a cambios de configuración registrada");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 1000000000); // Maximo tamaño de subida ~ 1Gb
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                });

        private static void CargarClaveReinicioCache(IHost pHost)
        {
            using (var scope = pHost.Services.CreateScope())
            {
                var entity = scope.ServiceProvider.GetRequiredService<EntityContext>();
                var loggingService = scope.ServiceProvider.GetRequiredService<LoggingService>();
                var configService = scope.ServiceProvider.GetRequiredService<ConfigService>();
                var redisCacheWrapper = scope.ServiceProvider.GetRequiredService<RedisCacheWrapper>();
                var servicesUtilVirtuosoAndReplication = scope.ServiceProvider.GetRequiredService<IServicesUtilVirtuosoAndReplication>();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                try
                {
                    ProyectoCL proyectoCL = new ProyectoCL(entity, loggingService, redisCacheWrapper, configService, new VirtuosoAD(loggingService, entity, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<VirtuosoAD>(), loggerFactory), servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<ProyectoCL>(), loggerFactory);
                    proyectoCL.AgregarClaveReinicioAplicacion(ProyectoAD.MetaProyecto);
                }
                catch (Exception ex)
                {
                    loggingService.GuardarLogError($"Ha habido un error al cargar la clave reinicio. ERROR: {ex}", loggerFactory.CreateLogger<Program>());
                    throw;
                }
            }

        }
    }
}
