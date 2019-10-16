using System.Collections.Generic;
using System.IO;
using Epam.ASPCore.Northwind.WebUI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public interface ICategoryService
    {
        List<SelectListItem> GetCategoriesSelectedList(int? selectedItemId = null);
        List<CategoriesModel> GetCategories();
        Stream GetCategoryImageStream(int categoryId);
    }
}
