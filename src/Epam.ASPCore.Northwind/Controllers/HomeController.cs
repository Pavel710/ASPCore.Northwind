using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epam.ASPCore.Northwind.WebUI.Models;
using Epam.ASPCore.Northwind.WebUI.Services;
using Microsoft.AspNetCore.Diagnostics;
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

        public HomeController(ITokenAcquisition tokenAcquisition, IOptions<WebOptions> webOptions)
        {
            _tokenAcquisition = tokenAcquisition;
            _webOptions = webOptions.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeForScopes(Scopes = new[] { Infrastructure.Constants.ScopeUserRead })]
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
        public IActionResult Error()
        {
            var error = this.HttpContext.Features.Get<IExceptionHandlerFeature>().Error;
            Log.Error("Application error: Request Id: " + (Activity.Current?.Id ?? HttpContext.TraceIdentifier) + "; " + "Source: " + error.Source + "; " + "Message: " + error.Message + "; " + Environment.NewLine + "StackTrace" + error.StackTrace);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
