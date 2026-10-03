using System;

namespace Ecommerce_Website_MVC.Models
{
    public class ExpenseModel
    {
        public int      ExpenseId   { get; set; }
        public string   Title       { get; set; }
        public decimal  Amount      { get; set; }
        public string   ExpenseType { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int?     OrderId     { get; set; }
        public string   Remarks     { get; set; }
    }
}

