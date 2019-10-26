using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.ASPCore.Northwind.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Epam.ASPCore.Northwind.WebUI.ViewComponents
{
    public class BreadcrumbsViewComponent : ViewComponent
    {
        public Task<ViewViewComponentResult> InvokeAsync(List<BreadcrumbModel> listModels)
        {
            return Task.FromResult(View(listModels));
        }
    }
}
