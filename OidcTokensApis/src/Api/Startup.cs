using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        public Startup()
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            // add MVC (without views)
            services.AddControllers();

            services.AddAuthentication("token")
                .AddJwtBearer("token", options =>
                {
                    options.Authority = "https://demo.identityserver.io";
                    options.Audience = "api";
                });
                
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization();
            });
        }
    }
}