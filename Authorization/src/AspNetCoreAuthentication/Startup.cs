using AspNetCoreAuthentication.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreAuthentication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(source =>
                {
                    source.Path = "appsettings.json";
                    source.ReloadOnChange = true;
                })
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // only allow authenticated users
            var defaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            services.AddMvc(setup =>
            {
                setup.Filters.Add(new AuthorizeFilter(defaultPolicy));
            });

            services.AddAuthorization(options =>
            {
                // some examples
                options.AddPolicy("SalesOnly", policy =>
                {
                    policy.RequireClaim("department", "sales");
                });

                options.AddPolicy("SalesSenior", policy =>
                {
                    policy.RequireClaim("department", "sales");
                    policy.RequireClaim("status", "senior");
                });

                options.AddPolicy("DevInterns", policy =>
                {
                    policy.RequireClaim("department", "development");
                    policy.RequireClaim("status", "intern");
                });

                // custom policy
                options.AddPolicy("CxO", policy =>
                {
                    policy.RequireJobLevel(JobLevel.CxO);
                });
            });

            // register resource authorization handlers
            services.AddTransient<IAuthorizationHandler, CustomerAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, ProductAuthorizationHandler>();

            // register data access services
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,

                LoginPath = new PathString("/account/login")
            });

            app.UseClaimsTransformation(context =>
            {
                if (context.Principal.Identity.IsAuthenticated)
                {
                    context.Principal.Identities.First().AddClaim(new Claim("now", DateTime.Now.ToString()));
                }

                return Task.FromResult(context.Principal);
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}