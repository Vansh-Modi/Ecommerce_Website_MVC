using System;
using System.Collections.Generic;

namespace Ecommerce_Website_MVC.Models
{
    public class OrderModel
    {
        public int      OrderId                  { get; set; }
        public int      UserId                   { get; set; }
        public DateTime OrderDate                { get; set; }
        public decimal  TotalAmount              { get; set; }
        public string   Status                   { get; set; }
        public string   ShippingAddress          { get; set; }
        public string   Pincode                  { get; set; }
        public string   PaymentMethod            { get; set; }
        public string   PaymentScreenshotPath    { get; set; }
        public string   TransactionReference     { get; set; }
        public string   TrackingPartner          { get; set; }
        public string   TrackingNumber           { get; set; }
        public DateTime? EstimatedDeliveryDate   { get; set; }
        public List<OrderDetailModel> Items { get; set; } = new List<OrderDetailModel>();
    }

    public class OrderDetailModel
    {
        public int     OrderDetailId { get; set; }
        public int     OrderId       { get; set; }
        public int     ProductId     { get; set; }
        public string  Title         { get; set; }
        public string  BrandName     { get; set; }
        public string  Scale         { get; set; }
        public int     Quantity      { get; set; }
        public decimal UnitPrice     { get; set; }
        public decimal LineTotal     => UnitPrice * Quantity;
    }
}

