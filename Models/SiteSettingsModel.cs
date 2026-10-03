namespace Ecommerce_Website_MVC.Models
{
    public class SiteSettingsModel
    {
        public int    SiteSettingId     { get; set; }
        public string Logo              { get; set; }
        public string LogoTitle         { get; set; }
        public string HeroImg           { get; set; }
        public string HeroTitle         { get; set; }
        public string HeroSubtitle      { get; set; }
        public string Copyright         { get; set; }
        public string Email             { get; set; }
        public string PhoneNumber       { get; set; }
        public decimal ShippingCharges  { get; set; }
        public string QrCodePath        { get; set; }
        public string UpiId             { get; set; }
        public string BankAccountDetails { get; set; }
    }
}

