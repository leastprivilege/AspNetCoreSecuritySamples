using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCoreSecurity.Controllers
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
                    new Claim("sub", "123456789"),
                    new Claim("name", "Dominick"),
                    new Claim("role", "Geek")
                };

                var ci = new ClaimsIdentity(claims, "password", "name", "role");
                var p = new ClaimsPrincipal(ci);

                await HttpContext.SignInAsync(p);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("/");
                }
            }

            return View();
        }

        public IActionResult Denied(string returnUrl = null)
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}