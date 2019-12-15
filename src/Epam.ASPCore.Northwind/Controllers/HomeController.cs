using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epam.ASPCore.Northwind.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost, AllowAnonymous]
        public IActionResult Logout()
        {
            foreach (var schema in new List<string>(){ AzureADDefaults.AuthenticationScheme, IdentityConstants.ApplicationScheme })
            {
                HttpContext.SignOutAsync(schema);
            }

            return LocalRedirect("~/");
        }

        [AllowAnonymous]
        public IActionResult RedirectToAzure()
        {
            var properties = new AuthenticationProperties { RedirectUri =  Url.Action("RedirectToAzureCallback")};
            return Challenge(properties, AzureADDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RedirectToAzureCallback()
        {
            var result = await HttpContext.AuthenticateAsync(AzureADDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var userPreferredName = User.FindFirstValue("preferred_username");

                if (_config.GetSection("AdminUserEmail").Value == userPreferredName)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userPreferredName),
                        new Claim(ClaimTypes.Email, userPreferredName),
                        new Claim(ClaimTypes.Role, "Administrator"),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        AzureADDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                }

                return RedirectToAction("Index", "Categories");
            }
            return LocalRedirect("/Home/Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var error = this.HttpContext.Features.Get<IExceptionHandlerFeature>().Error;
            Log.Error("Application error: Request Id: " + (Activity.Current?.Id ?? HttpContext.TraceIdentifier) + "; " + "Source: " + error.Source + "; " + "Message: " + error.Message + "; " + Environment.NewLine + "StackTrace" + error.StackTrace);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
