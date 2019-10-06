using System.Linq;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Epam.ASPCore.Northwind.WebUI.Models;
using Epam.ASPCore.Northwind.WebUI.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly INorthwindRepository<Products> _productsRepository;
        private readonly INorthwindRepository<Categories> _categoriesRepository;
        private readonly INorthwindRepository<Suppliers> _supplierRepository;
        private readonly ProductsSettings _productsSettings;

        public ProductsController(INorthwindRepository<Products> productsRepository, INorthwindRepository<Categories> categoriesRepository, INorthwindRepository<Suppliers> supplierRepository, IOptions<ProductsSettings> productsSettings)
        {
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
            _supplierRepository = supplierRepository;
            _productsSettings = productsSettings.Value;
        }

        public IActionResult Index()
        {
            var categories = _categoriesRepository.Get();
            var suppliers = _supplierRepository.Get();
            var products = _productsRepository.Get();

            var productsModel = products.Select(p => new ProductsModel
            {
                Discontinued = p.Discontinued,
                ReorderLevel = p.ReorderLevel,
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                QuantityPerUnit = p.QuantityPerUnit,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                UnitsOnOrder = p.UnitsOnOrder,
                Category = categories.Where(c => c.CategoryId == p.CategoryId).Select(nc => new CategoriesModel()
                {
                    CategoryId = nc.CategoryId,
                    CategoryName = nc.CategoryName
                }).FirstOrDefault(),
                Supplier = suppliers.Where(s => s.SupplierId == p.SupplierId).Select(ns => new SuppliersModel()
                {
                    SupplierId = ns.SupplierId,
                    CompanyName = ns.CompanyName
                }).FirstOrDefault()
            }).Take(_productsSettings.Maximum == 0 ? products.ToList().Count : _productsSettings.Maximum);

            return View(productsModel);
        }
    }
}