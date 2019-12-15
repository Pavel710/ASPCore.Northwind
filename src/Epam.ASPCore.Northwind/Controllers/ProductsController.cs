using Epam.ASPCore.Northwind.WebUI.Models;
using Epam.ASPCore.Northwind.WebUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;

        public ProductsController(ICategoryService categoryService,
            IProductService productService,
            ISupplierService supplierService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _supplierService = supplierService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_productService.GetProducts());
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Add()
        {
            ViewBag.Mod = "Add";
            ViewBag.Categories = _categoryService.GetCategoriesSelectedList();
            ViewBag.Suppliers = _supplierService.GetSuppliersSelectedList();
            return View("ProductForm");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Add(ProductsModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Add");
            }

            _productService.SaveProduct(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.Mod = "Update";
            var model = _productService.GetProduct(id);
            ViewBag.Categories = _categoryService.GetCategoriesSelectedList(model.Category?.CategoryId);
            ViewBag.Suppliers = _supplierService.GetSuppliersSelectedList(model.Supplier?.SupplierId);
            return View("ProductForm", model);
        }

        [HttpPost]
        public IActionResult Update(ProductsModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Update");
            }

            _productService.UpdateProduct(model);
            return RedirectToAction("Index");
        }
    }
}