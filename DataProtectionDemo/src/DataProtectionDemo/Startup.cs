using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace DataProtectionDemo
{
    public class Startup
    {
        private readonly IApplicationEnvironment _app;

        public Startup(IApplicationEnvironment app)
        {
            _app = app;
            
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.ConfigureDataProtection(config =>
            {
                config.SetApplicationName("DataProtectionDemo");

                config.PersistKeysToFileSystem(new DirectoryInfo(_app.ApplicationBasePath));
                config.ProtectKeysWithDpapi();                
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}