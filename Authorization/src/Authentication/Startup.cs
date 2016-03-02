using Authentication.Policies;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Authentication
{
    public class Startup
    {
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

                options.AddPolicy("Over18", policy =>
                {
                    policy.RequireDelegate((context, requirement) =>
                    {
                        var age = context.User.FindFirst("age")?.Value ?? "0";
                        if (int.Parse(age) >= 18)
                        {
                            context.Succeed(requirement);
                        }
                    });
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
            app.UseIISPlatformHandler();
            app.UseDeveloperExceptionPage();
            
            app.UseCookieAuthentication(options =>
            {
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/forbidden";

                options.AuthenticationScheme = "Cookies";
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
            });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}