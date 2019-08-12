

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Controllers
{
    public class AuthController : Controller
    {
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        [Route("Auth/Login")]
        public async Task<IActionResult> Login(string returnUrl, string username, string password)
        {
            var environmentUsername = Environment.GetEnvironmentVariable("TAYKO_AUTH_USER") ?? "Admin";
            var environmentPassword = Environment.GetEnvironmentVariable("TAYKO_AUTH_PASS") ?? "Denver*123";

            if (username == environmentUsername && password == environmentPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, environmentUsername, ClaimValueTypes.String, "https://tayko.co")
                };
                var userIdentity = new ClaimsIdentity(claims, "SecureLogin");
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                        IsPersistent = false,
                        AllowRefresh = false
                    });

                return GoToReturnUrl(returnUrl);
            }

            return RedirectToAction("Denied");
        }
        
        private IActionResult GoToReturnUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Denied()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("Auth/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect(Request.Headers["Referer"]);
        }
    }
}