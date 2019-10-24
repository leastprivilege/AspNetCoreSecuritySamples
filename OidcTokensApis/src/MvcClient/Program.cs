using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace MvcCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog((context, config) =>
                     {
                         config
                             .MinimumLevel.Warning()
                             .MinimumLevel.Override("IdentityModel", LogEventLevel.Debug)
                             .MinimumLevel.Override("System.Net.Http", LogEventLevel.Information)
                             .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                             .Enrich.FromLogContext()
                             .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");
                     });
                });

        private static void UseSerilog(Func<object, object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
