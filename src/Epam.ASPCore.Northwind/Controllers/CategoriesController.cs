using System.IO;
using System.Threading.Tasks;
using Epam.ASPCore.Northwind.WebUI.Services;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("/CategoryImages/{id}")]
        public IActionResult GetImage(int id)
        {
            var categoryImage = _categoryService.GetCategoryImage(id);
            Stream categoryImageStream = categoryImage.ImageStream;

            if (categoryImageStream.Length == 0)
                return NotFound();

            return File(categoryImageStream, $"image/{categoryImage.ImageFormat}");
        }

        [HttpGet]
        public IActionResult UploadImage(int categoryId)
        {
            return View(_categoryService.GetCategory(categoryId));
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile uploadedImage, int categoryId)
        {
            await _categoryService.UploadCategoryPicture(uploadedImage, categoryId);
            return RedirectToAction("Index");
        }
    }
}