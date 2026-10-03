using System;

namespace Ecommerce_Website_MVC.Models
{
    public class ProductModel
    {
        public int     ProductId     { get; set; }
        public string  Title         { get; set; }
        public string  BrandName     { get; set; }
        public int     CategoryId    { get; set; }
        public string  CategoryName  { get; set; }
        public string  Scale         { get; set; }
        public string  Description   { get; set; }
        public decimal SellingPrice  { get; set; }
        public decimal CostPrice     { get; set; }
        public decimal MRP           { get; set; }
        public int     StockQuantity { get; set; }
        public string  ImagePath     { get; set; }
        public bool    IsNewArrival  { get; set; }
        public DateTime CreatedAt    { get; set; }
    }
}

