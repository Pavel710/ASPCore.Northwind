using System.Collections.Generic;
using Epam.ASPCore.Northwind.WebUI.Models;
using Epam.ASPCore.Northwind.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    [Route("api/NorthwindAPI")]
    [ApiController]
    public class NorthwindAPIController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public NorthwindAPIController(
            IProductService productService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet("categories")]
        public ActionResult<List<CategoriesModel>> GetCategoriesCollection()
        {
            return _categoryService.GetCategories();
        }

        [HttpGet("products")]
        public ActionResult<List<ProductsModel>> GetProductsCollection()
        {
            return _productService.GetProducts();
        }
    }
}