namespace Ecommerce_Website_MVC.Models
{
    public class ReportModel
    {
        public decimal TotalRevenue       { get; set; }
        public decimal TotalExpenses      { get; set; }
        public decimal NetProfit          { get; set; }
        public int     TotalOrdersCount   { get; set; }
        public int     ApprovedOrders     { get; set; }
        public int     CancelledOrders    { get; set; }
        public decimal MtdRevenue         { get; set; }
        public decimal TotalCogs          { get; set; }
    }
}

