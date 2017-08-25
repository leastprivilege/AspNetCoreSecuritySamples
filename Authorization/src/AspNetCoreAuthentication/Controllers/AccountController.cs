using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCoreAuthentication.Controllers
{
    [AllowAnonymous]
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

            if (!string.IsNullOrEmpty(userName) &&
                userName == password)
            {
                List<Claim> claims;

                if (userName == "alice")
                {
                    claims = new List<Claim>
                    {
                        new Claim("sub", "1"),
                        new Claim("name", "Alice"),
                        new Claim("email", "alice@smith.com"),
                        new Claim("status", "senior"),
                        new Claim("department", "sales"),
                        new Claim("region", "south")
                    };
                }
                else if (userName == "bob")
                {
                    claims = new List<Claim>
                    {
                        new Claim("sub", "2"),
                        new Claim("name", "Bob"),
                        new Claim("email", "bob@smith.com"),
                        new Claim("status", "junior"),
                        new Claim("department", "sales"),
                        new Claim("region", "north"),
                        new Claim("role", "supervisor")
                    };
                }
                else
                {
                    claims = new List<Claim>
                    {
                        new Claim("sub", "3"),
                        new Claim("name", userName),
                        new Claim("email", userName + "@smith.com"),
                        new Claim("status", "intern"),
                        new Claim("department", "development")
                    };
                }

                var id = new ClaimsIdentity(claims, "local", "name", "role");
                await HttpContext.SignInAsync(new ClaimsPrincipal(id));

                return LocalRedirect(returnUrl);
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}