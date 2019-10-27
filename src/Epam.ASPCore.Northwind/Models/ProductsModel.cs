using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class ProductsModel
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "The product name field is required.")]
        [StringLength(100)]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "The quantity per unit field is required.")]
        public string QuantityPerUnit { get; set; }
        [Range(0, 9999.99)]
        public decimal? UnitPrice { get; set; }
        [Range(0, 200)]
        public short? UnitsInStock { get; set; }
        [Range(0, 200)]
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public CategoriesModel Category { get; set; }
        public SuppliersModel Supplier { get; set; }
        public ICollection<OrderDetailsModel> OrderDetails { get; set; }
    }
}
