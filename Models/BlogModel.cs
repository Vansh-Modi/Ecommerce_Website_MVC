using System;

namespace Ecommerce_Website_MVC.Models
{
    public class BlogModel
    {
        public int      BlogId      { get; set; }
        public string   BlogTitle   { get; set; }
        public string   Excerpt     { get; set; }
        public string   Content     { get; set; }
        public string   BlogImage   { get; set; }
        public DateTime PublishDate { get; set; }
        public string   Author      { get; set; }
    }
}

