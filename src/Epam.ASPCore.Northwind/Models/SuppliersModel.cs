using System.Collections.Generic;

namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class SuppliersModel
    {
        public SuppliersModel()
        {
            
        }

        public SuppliersModel(int supplierId)
        {
            SupplierId = supplierId;
        }

        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string HomePage { get; set; }

        public ICollection<ProductsModel> Products { get; set; }
    }
}
