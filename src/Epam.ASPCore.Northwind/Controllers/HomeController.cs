using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epam.ASPCore.Northwind.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(IConfiguration config, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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
                var userExist = await _userManager.FindByEmailAsync(userPreferredName);

                if (userExist == null)
                {
                    var newAzureUser = new IdentityUser { UserName = userPreferredName, Email = userPreferredName };
                    var resultCreateUser = await _userManager.CreateAsync(newAzureUser);
                    if (resultCreateUser.Succeeded)
                    {
                        Log.Information("User created a new account.");
                        if (_config.GetSection("AdminUserEmail").Value == userPreferredName)
                        {
                            var currentUser = await _userManager.FindByEmailAsync(newAzureUser.Email);
                            await _userManager.AddToRoleAsync(currentUser, "Administrator");
                        }
                        await _signInManager.SignInAsync(newAzureUser, isPersistent: false);
                    }
                }
                else
                {
                    if (_config.GetSection("AdminUserEmail").Value == userPreferredName)
                    {
                        var userAzure = await _userManager.FindByEmailAsync(userPreferredName);
                        await _signInManager.SignInAsync(userAzure, isPersistent: false);
                    }
                }

                return RedirectToAction("Index", "Categories");
            }
            return LocalRedirect("/Home/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult UsersTable()
        {
            return View(_userManager.Users.ToList());
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
