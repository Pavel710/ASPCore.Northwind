using System.Collections.Generic;
using Epam.ASPCore.Northwind.WebUI.Models;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public interface IProductService
    {
        List<ProductsModel> GetProductsModelList();
        void SaveProduct(ProductsModel model);
        ProductsModel GetProduct(int id);
        void UpdateProduct(ProductsModel model);
    }
}
