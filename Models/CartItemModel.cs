namespace Ecommerce_Website_MVC.Models
{
    public class CartItemModel
    {
        public int     ProductId     { get; set; }
        public string  BrandName     { get; set; }
        public string  Title         { get; set; }
        public decimal SellingPrice  { get; set; }
        public decimal MRP           { get; set; }
        public int     Quantity      { get; set; }
        public string  ImagePath     { get; set; }
        public int     StockQuantity { get; set; }
        public decimal LineTotal => SellingPrice * Quantity;
    }
}

