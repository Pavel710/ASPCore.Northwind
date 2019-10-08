namespace Epam.ASPCore.Northwind.WebUI.Models
{
    public class OrderDetailsModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }

        public OrdersModel Order { get; set; }
        public ProductsModel Product { get; set; }
    }
}
