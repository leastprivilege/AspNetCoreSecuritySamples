using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace AspNetCoreAuthentication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // never persist key store to local directory, just for demo purposes
            services.AddDataProtection()
                .SetApplicationName("DataProtectionDemo")
                .ProtectKeysWithDpapi()
                .PersistKeysToFileSystem(new DirectoryInfo(Directory.GetCurrentDirectory()));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}