using System.IO;

namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class CategoryImageModel
    {
        public MemoryStream ImageStream { get; set; }
        public string ImageFormat { get; set; }
    }
}
