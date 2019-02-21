using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace ConsulTestService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();
            var consulService= new ConsulService();
            consulService.RegisterDiscoveryService();
            CreateWebHostBuilder(args).Build().Run();
            consulService.DeregisterDiscoveryService();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://+:{ServiceConfigs.ServicePort}/")
                .UseStartup<Startup>();

        public static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.With(
                    new ThreadIdEnricher())
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
                .WriteTo.File(Path.Combine(RootDirectory, "..\\Logs\\ois-test-service.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] (Td={ThreadId}) {Message}{NewLine}{Exception}")
                .CreateLogger();
        }

        private static string RootDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    }
}
