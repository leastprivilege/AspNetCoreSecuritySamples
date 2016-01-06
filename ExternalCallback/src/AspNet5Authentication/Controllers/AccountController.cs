using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using Microsoft.AspNet.Http.Authentication;

namespace AspNet5Authentication.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!string.IsNullOrWhiteSpace(userName) &&
                userName == password)
            {
                var claims = new List<Claim>
                {
                    new Claim("sub", userName),
                    new Claim("given_name", "Dominick"),
                    new Claim("role", "Geek")
                };

                var id = new ClaimsIdentity(claims, "Password");

                await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));

                return new LocalRedirectResult(returnUrl);
            }

            return View();
        }

        public IActionResult Google()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "/account/callback"
            };

            return new ChallengeResult("Google", props);
        }

        public async Task<IActionResult> Callback()
        {
            var external = await HttpContext.Authentication.AuthenticateAsync("External");

            var claims = new List<Claim>
            {
                new Claim("sub", "123223"),
                new Claim("name", external.FindFirst(ClaimTypes.Name).Value),
                new Claim("role", "Geek")
            };

            var id = new ClaimsIdentity(claims, "Password");

            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));
            await HttpContext.Authentication.SignOutAsync("External");

            return Redirect("/home/secure");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return Redirect("~/");
        }
    }
}