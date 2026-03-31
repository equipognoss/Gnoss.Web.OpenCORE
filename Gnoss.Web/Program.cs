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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Gnoss.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CargarClaveReinicioCache(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 1000000000); // Maximo tamaño de subida ~ 1Gb
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                });

        private static void CargarClaveReinicioCache(IHost pHost)
        {
            using(var scope = pHost.Services.CreateScope())
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
                catch (Exception ex) { 
                    loggingService.GuardarLogError($"Ha habido un error al cargar la clave reinicio. ERROR: {ex}", loggerFactory.CreateLogger<Program>());
                    throw;
                }
            }

        }
    }
}
