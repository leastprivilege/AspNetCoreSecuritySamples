// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreSecurity
{
    public class Startup
    {
        public Startup()
        {
            // lame
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies", options =>
                {
                    options.AccessDeniedPath = "/account/denied";
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = "https://demo.identityserver.io";
                    options.ClientId = "server.hybrid";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("api");

                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.MapUniqueJsonKey("website", "website");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name", 
                        RoleClaimType = "role"
                    };
                });

            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
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