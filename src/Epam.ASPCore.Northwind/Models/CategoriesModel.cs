using System.Collections.Generic;

namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class CategoriesModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }

        public virtual ICollection<ProductsModel> Products { get; set; }
    }
}
