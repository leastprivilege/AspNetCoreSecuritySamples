// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
        [ValidateAntiForgeryToken]
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

        public IActionResult Google(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                return Redirect("/");
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),

                Items =
                {
                    { "uru", returnUrl },
                    { "scheme", "Google" }
                }
            };

            return Challenge(props, "Google");
        }

        public async Task<IActionResult> Callback()
        {
            var result = await HttpContext.AuthenticateAsync("external");
            if (!result.Succeeded)
            {
                throw new Exception("error");
            }

            // get sub and issuer to check if external user is known
            var sub = result.Principal.FindFirst("sub");
            var issuer = result.Properties.Items["scheme"];

            // do your customm provisioning logic

            // sign in user
            var claims = new List<Claim>
            {
                new Claim("sub", "123456789"),
                new Claim("name", "Dominick"),
                new Claim("role", "Geek"),
                new Claim("email", result.Principal.FindFirst("email").Value)
            };

            var ci = new ClaimsIdentity(claims, issuer, "name", "role");
            var p = new ClaimsPrincipal(ci);

            await HttpContext.SignInAsync(p);

            return Redirect(result.Properties.Items["uru"]);
        }

        public IActionResult AccessDenied() => View();

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}