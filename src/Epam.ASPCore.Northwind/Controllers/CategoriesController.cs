using System.IO;
using Epam.ASPCore.Northwind.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_categoryService.GetCategories());
        }

        [HttpGet]
        public IActionResult GetImage(int id)
        {
            Stream categoryImageStream = _categoryService.GetCategoryImageStream(id);

            if (categoryImageStream.Length == 0)
                return NotFound();

            return File(categoryImageStream, "application/octet-stream");
        }
    }
}