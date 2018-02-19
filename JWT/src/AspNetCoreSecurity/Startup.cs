// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSecurity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                options.Filters.Add(new AuthorizeFilter(
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build()));
            })
                .AddJsonFormatters()
                .AddAuthorization();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://demo.identityserver.io";
                    options.Audience = "api";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}