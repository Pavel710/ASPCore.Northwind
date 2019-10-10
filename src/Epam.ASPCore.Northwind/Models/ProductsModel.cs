using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class ProductsModel
    {
        public int ProductId { get; set; }
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }
        [Required]
        public string QuantityPerUnit { get; set; }
        [Required]
        [Range(0, 9999.99)]
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public CategoriesModel Category { get; set; }
        public SuppliersModel Supplier { get; set; }
        public ICollection<OrderDetailsModel> OrderDetails { get; set; }
    }
}
