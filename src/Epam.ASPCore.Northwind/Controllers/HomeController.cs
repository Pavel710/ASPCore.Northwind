using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epam.ASPCore.Northwind.WebUI.Models;
using Epam.ASPCore.Northwind.WebUI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Serilog;
using Constants = Epam.ASPCore.Northwind.WebUI.Infrastructure.Constants;

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ITokenAcquisition tokenAcquisition, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            //var a = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
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
