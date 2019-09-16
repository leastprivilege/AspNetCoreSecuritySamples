using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace AspNetCoreSecurity
{
    public class Startup
    {
        public Startup()
        {
            // turn off claim type mappings
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication("cookie")
                .AddCookie("cookie", options =>
                {
                    options.Cookie.Name = "demo";

                    options.LoginPath = "/account/login";
                    options.AccessDeniedPath = "/account/accessdenied";
                })
                .AddCookie("external", options =>
                {
                    options.Cookie.Name = "external";
                })
                .AddOpenIdConnect("Google", "Google", options =>
                {
                    options.SignInScheme = "external";
                    
                    options.Authority = "https://accounts.google.com/";
                    options.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";

                    options.CallbackPath = "/signin-google";
                    options.Scope.Add("email");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}