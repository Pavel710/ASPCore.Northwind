using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class ProductsModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public CategoriesModel Category { get; set; }
        public SuppliersModel Supplier { get; set; }
    }
}
