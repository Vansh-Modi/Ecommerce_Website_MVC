using System;

namespace Ecommerce_Website_MVC.Models
{
    public class UserModel
    {
        public int    UserId       { get; set; }
        public string FullName     { get; set; }
        public string Phone        { get; set; }
        public string Email        { get; set; }
        public string PasswordHash { get; set; }
        public string Role         { get; set; }
        public bool   IsVerified   { get; set; }
        public DateTime CreatedAt  { get; set; }
    }
}

