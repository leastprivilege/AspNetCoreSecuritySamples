using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Http.Authentication;

namespace AspNetCoreAuthentication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secure()
        {
            ViewBag.IdentityToken = await HttpContext.Authentication.GetIdentityTokenAsync();
            ViewBag.AccessToken = await HttpContext.Authentication.GetAccessTokenAsync();
            
            return View();
        }
    }

    internal static class AuthenticateContextExtensions
    {
        public static async Task<string> GetIdentityTokenAsync(this AuthenticationManager manager)
        {
            var context = new AuthenticateContext("Cookies");
            await manager.AuthenticateAsync(context);

            return context.Properties[".Token.id_token"];
        }

        public static async Task<string> GetAccessTokenAsync(this AuthenticationManager manager)
        {
            var context = new AuthenticateContext("Cookies");
            await manager.AuthenticateAsync(context);

            return context.Properties[".Token.access_token"];
        }
    }
}