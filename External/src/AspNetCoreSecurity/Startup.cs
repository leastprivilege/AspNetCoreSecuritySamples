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
                })
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
                    options.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
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
