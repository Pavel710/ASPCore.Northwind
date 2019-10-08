using System.Collections.Generic;
using System.Linq;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Epam.ASPCore.Northwind.WebUI.Models;
using Epam.ASPCore.Northwind.WebUI.Settings;
using Microsoft.Extensions.Options;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public class ProductService : IProductService
    {
        private readonly INorthwindRepository<Products> _productsRepository;
        private readonly INorthwindRepository<Categories> _categoriesRepository;
        private readonly INorthwindRepository<Suppliers> _supplierRepository;
        private readonly ProductsSettings _productsSettings;

        public ProductService(INorthwindRepository<Products> productsRepository,
            INorthwindRepository<Categories> categoriesRepository,
            INorthwindRepository<Suppliers> supplierRepository,
            IOptions<ProductsSettings> productsSettings)
        {
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
            _supplierRepository = supplierRepository;
            _productsSettings = productsSettings.Value;
        }

        public List<ProductsModel> GetProductsModelList()
        {
            var categories = _categoriesRepository.Get();
            var suppliers = _supplierRepository.Get();
            var products = _productsRepository.Get();

            return products.Select(p => new ProductsModel
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
            }).Take(_productsSettings.Maximum == 0 ? products.ToList().Count : _productsSettings.Maximum)
                .ToList();
        }

        public void SaveProduct(ProductsModel model)
        {
            var product = new Products
            {
                Discontinued = model.Discontinued,
                ProductName = model.ProductName,
                QuantityPerUnit = model.QuantityPerUnit,
                ReorderLevel = model.ReorderLevel,
                UnitPrice = model.UnitPrice,
                UnitsInStock = model.UnitsInStock,
                UnitsOnOrder = model.UnitsOnOrder,
                CategoryId = model.Category?.CategoryId,
                SupplierId = model.Supplier?.SupplierId
            };

            _productsRepository.Insert(product);
        }

        public ProductsModel GetProduct(int id)
        {
            var product = _productsRepository.GetByID(id);
            return new ProductsModel
            {
                Discontinued = product.Discontinued,
                ProductName = product.ProductName,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
                ProductId = product.ProductId,
                Category = product.CategoryId.HasValue ? new CategoriesModel(product.CategoryId.Value) : new CategoriesModel(),
                Supplier = product.SupplierId.HasValue ? new SuppliersModel(product.SupplierId.Value) : new SuppliersModel()
            };
        }

        public void UpdateProduct(ProductsModel model)
        {
            var product = _productsRepository.GetByID(model.ProductId);
            product.Discontinued = model.Discontinued;
            product.ProductName = model.ProductName;
            product.QuantityPerUnit = model.QuantityPerUnit;
            product.ReorderLevel = model.ReorderLevel;
            product.UnitPrice = model.UnitPrice;
            product.UnitsInStock = model.UnitsInStock;
            product.UnitsOnOrder = model.UnitsOnOrder;
            product.CategoryId = model.Category?.CategoryId;
            product.SupplierId = model.Supplier?.SupplierId;

            _productsRepository.Update(product);
        }
    }
}
