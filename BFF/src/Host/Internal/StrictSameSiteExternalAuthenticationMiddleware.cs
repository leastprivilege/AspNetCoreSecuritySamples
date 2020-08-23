using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace IdentityModel.AspNetCore
{
    // Fixes for Safari
    // https://brockallen.com/2019/01/11/same-site-cookies-asp-net-core-and-external-authentication-providers/
    public class StrictSameSiteExternalAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public StrictSameSiteExternalAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext ctx)
        {
            var schemes = ctx.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            var handlers = ctx.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();

            foreach (var scheme in await schemes.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(ctx, scheme.Name) as IAuthenticationRequestHandler;
                if (handler != null && await handler.HandleRequestAsync())
                {
                    // start same-site cookie special handling
                    string location = null;
                    if (ctx.Response.StatusCode == 302)
                    {
                        location = ctx.Response.Headers["location"];
                    }
                    else if (ctx.Request.Method == "GET" && !ctx.Request.Query["skip"].Any())
                    {
                        location = ctx.Request.Path + ctx.Request.QueryString + "&skip=1";
                    }

                    if (location != null)
                    {
                        ctx.Response.StatusCode = 200;
                        ctx.Response.ContentType = MediaTypeNames.Text.Html;

                        var html = $@"
                        <html><head>
                            <meta http-equiv='refresh' content='0;url={location}' />
                        </head></html>";
                        await ctx.Response.WriteAsync(html);
                    }
                    // end same-site cookie special handling

                    return;
                }
            }

            await _next(ctx);
        }
    }
}
