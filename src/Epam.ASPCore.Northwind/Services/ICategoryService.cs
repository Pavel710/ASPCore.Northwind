using System.Collections.Generic;
using Epam.ASPCore.Northwind.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public interface ICategoryService
    {
        List<SelectListItem> GetCategoriesSelectedList(int? selectedItemId = null);
    }
}
