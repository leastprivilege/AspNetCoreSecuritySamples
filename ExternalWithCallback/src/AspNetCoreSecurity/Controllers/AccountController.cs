// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using System;

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

        public IActionResult Google(string returnUrl = null)
        {
            if (!Url.IsLocalUrl(returnUrl)) returnUrl = "/";

            var props = new AuthenticationProperties
            {
                RedirectUri = "/account/callback",
                Items =
                {
                    { "returnUrl", returnUrl }
                }
            };
            
            return Challenge(props, "Google");
        }

        public async Task<IActionResult> Callback()
        {
            var result = await HttpContext.AuthenticateAsync("Temp");
            if (!result.Succeeded) throw new Exception("no external authentication going on right now...");

            var extUser = result.Principal;
            var extUserId = extUser.FindFirst(ClaimTypes.NameIdentifier);
            var issuer = extUserId.Issuer;

            // provisioning logic happens here...

            var claims = new List<Claim>
            {
                new Claim("sub", "123456789"),
                new Claim("name", "Dominick"),
                new Claim("email", extUser.FindFirst(ClaimTypes.Email).Value),
                new Claim("role", "Geek")
            };

            var ci = new ClaimsIdentity(claims, "password", "name", "role");
            var p = new ClaimsPrincipal(ci);

            await HttpContext.SignInAsync(p);
            await HttpContext.SignOutAsync("Temp");

            return Redirect(result.Properties.Items["returnUrl"]);
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