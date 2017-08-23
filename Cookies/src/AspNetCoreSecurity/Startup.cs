using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSecurity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/account/login";
                    options.AccessDeniedPath = "/account/denied";
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}