using System.Collections.Generic;

namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class CategoriesModel
    {
        public CategoriesModel()
        {
            
        }

        public CategoriesModel(int categoryId)
        {
            CategoryId = categoryId;
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }

        public ICollection<ProductsModel> Products { get; set; }
    }
}
