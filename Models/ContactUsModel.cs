using System;

namespace Ecommerce_Website_MVC.Models
{
    public class ContactUsModel
    {
        public int    ContactId { get; set; }
        public string Name      { get; set; }
        public string Email     { get; set; }
        public string Message   { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}

