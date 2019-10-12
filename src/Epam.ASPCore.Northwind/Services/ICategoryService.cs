using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public interface ICategoryService
    {
        List<SelectListItem> GetCategoriesSelectedList(int? selectedItemId = null);
    }
}
