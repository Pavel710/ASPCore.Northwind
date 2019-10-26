namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class BreadcrumbModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string EntityName { get; set; }
        public bool IsCurrentPage { get; set; }
    }
}
