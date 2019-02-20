// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityModel.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
            services.AddHttpClient();

            services.AddMvc(options =>
            {
                var global = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();

                options.Filters.Add(new AuthorizeFilter(global));

            }).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("cookies", options =>
                {
                    options.AccessDeniedPath = "/account/denied";
                })
                .AddAutomaticTokenManagement()
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://demo.identityserver.io";
                    options.ClientId = "native.code";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("api");

                    options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash", "at_hash");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };

                    options.Events.OnRedirectToIdentityProvider = context =>
                    {
                        // only modify requests to the authorization endpoint
                        if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                        {
                            // generate code_verifier
                            var codeVerifier = CryptoRandom.CreateUniqueId(32);

                            // store codeVerifier for later use
                            context.Properties.Items.Add(OidcConstants.TokenRequest.CodeVerifier, codeVerifier);

                            // create code_challenge
                            string codeChallenge;
                            using (var sha256 = SHA256.Create())
                            {
                                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                                codeChallenge = Base64Url.Encode(challengeBytes);
                            }

                            // add code_challenge and code_challenge_method to request
                            context.ProtocolMessage.Parameters.Add(OidcConstants.AuthorizeRequest.CodeChallenge, codeChallenge);
                            context.ProtocolMessage.Parameters.Add(OidcConstants.AuthorizeRequest.CodeChallengeMethod, OidcConstants.CodeChallengeMethods.Sha256);
                        }

                        return Task.CompletedTask;
                    };

                    options.Events.OnAuthorizationCodeReceived = context =>
                    {
                        // only when authorization code is being swapped for tokens
                        if (context.TokenEndpointRequest?.GrantType == OpenIdConnectGrantTypes.AuthorizationCode)
                        {
                            // get stored code_verifier
                            if (context.Properties.Items.TryGetValue(OidcConstants.TokenRequest.CodeVerifier, out var codeVerifier))
                            {
                                // add code_verifier to token request
                                context.TokenEndpointRequest.Parameters.Add(OidcConstants.TokenRequest.CodeVerifier, codeVerifier);
                            }
                        }

                        return Task.CompletedTask;
                    };
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}