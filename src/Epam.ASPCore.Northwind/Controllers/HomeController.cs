using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private ITokenAcquisition _tokenAcquisition;
        private readonly WebOptions _webOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ITokenAcquisition tokenAcquisition, IOptions<WebOptions> webOptions, 
            IHttpContextAccessor httpContextAccessor, SignInManager<IdentityUser> signInManager)
        {
            _tokenAcquisition = tokenAcquisition;
            _webOptions = webOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            //var a = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }

        [AllowAnonymous]
        public IActionResult RedirectToAzure()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("RedirectToAzureCallback") };
            return new ChallengeResult("AzureAD", properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RedirectToAzureCallback()
        {
            var result = await HttpContext.AuthenticateAsync(AzureADDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, "Evgen")
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(id));
                return LocalRedirect("/Categories/Index");
            }
            return LocalRedirect("/Home/Index");
        }


        [Authorize(AuthenticationSchemes = "AzureAD")]
        public async Task<IActionResult> Profile()
        {
            // Initialize the GraphServiceClient. 
            GraphServiceClient graphClient = GetGraphServiceClient(new[] { Infrastructure.Constants.ScopeUserRead });

            var me = await graphClient.Me.Request().GetAsync();
            ViewBag.Me = me;

            try
            {
                // Get user photo
                using (var photoStream = await graphClient.Me.Photo.Content.Request().GetAsync())
                {
                    byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                    ViewBag.Photo = Convert.ToBase64String(photoByte);
                }
            }
            catch (Exception)
            {
                ViewBag.Photo = null;
            }

            return View();
        }

        private GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await _tokenAcquisition.GetAccessTokenOnBehalfOfUserAsync(scopes);
                return result;
            }, _webOptions.GraphApiUrl);
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
