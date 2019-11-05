using System.Collections.Generic;

namespace Epam.ASPCore.Northwind.Tests.API.Models
{
    public class ResponseModel
    {
        public bool Success { get; set; }
        public List<string> ProductName { get; set; }
        public List<string> QuantityPerUnit { get; set; }
        public List<string> UnitPrice { get; set; }
        public List<string> UnitsInStock { get; set; }
        public List<string> UnitsOnOrder { get; set; }
    }
}
