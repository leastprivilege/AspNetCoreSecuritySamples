using AspNetCoreAuthentication.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
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
        public Startup(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                // only allow authenticated users
                var defaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(defaultPolicy));
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

        public void Configure(IApplicationBuilder app)
        {
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

            app.UseMvcWithDefaultRoute();
        }
    }
}