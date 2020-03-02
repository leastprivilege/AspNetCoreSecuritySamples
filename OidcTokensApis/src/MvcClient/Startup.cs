using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace MvcCode
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("cookie", options =>
                {
                    options.Cookie.Name = "mvccode";

                    options.Events.OnSigningOut = async e =>
                    {
                        await e.HttpContext.RevokeUserRefreshTokenAsync();
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://demo.identityserver.io";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "interactive.confidential.short";
                    options.ClientSecret = "secret";

                    // code flow + PKCE (PKCE is turned on by default)
                    options.ResponseType = "code";
                    options.UsePkce = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("api");

                    // not mapped by default
                    options.ClaimActions.MapJsonKey("website", "website");

                    // keeps id_token smaller
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                    
                    options.ClaimActions.DeleteClaim("s_hash");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });

            // adds global authorization policy to require authenticated users
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = options.DefaultPolicy;
            });

            // adds user and client access token management
            services.AddAccessTokenManagement(options =>
                {
                    // client config is inferred from OpenID Connect settings
                    // if you want to specify scopes explicitly, do it here, otherwise the scope parameter will not be sent
                    options.Client.Scope = "api";
                })
                .ConfigureBackchannelHttpClient()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            // registers HTTP client that uses the managed user access token
            services.AddUserAccessTokenClient("user_client", client =>
            {
                client.BaseAddress = new Uri(" https://localhost:44311/");
            });

            // registers HTTP client that uses the managed client access token
            services.AddClientAccessTokenClient("client", configureClient: client =>
            {
                client.BaseAddress = new Uri(" https://localhost:44311/");
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
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