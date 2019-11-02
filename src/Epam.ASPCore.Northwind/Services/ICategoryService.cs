using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.ASPCore.Northwind.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public interface ICategoryService
    {
        List<SelectListItem> GetCategoriesSelectedList(int? selectedItemId = null);

        List<CategoriesModel> GetCategories();

        CategoryImageModel GetCategoryImage(int categoryId);

        Task<bool> UploadCategoryPicture(IFormFile image, int categoryId);

        CategoriesModel GetCategory(int id);
    }
}
