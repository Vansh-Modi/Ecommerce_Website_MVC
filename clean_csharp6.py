import os
import re

directory = r"D:\.Net Projects\MScIT\Ecommerce_Website_MVC\Views"

for root, _, files in os.walk(directory):
    for f in files:
        if f.endswith('.cshtml'):
            path = os.path.join(root, f)
            with open(path, 'r', encoding='utf-8') as file:
                content = file.read()
            
            orig = content
            
            # 1. String Interpolation
            content = content.replace('$"&copy; {DateTime.Now.Year} Shows Garage"', '"&copy; " + DateTime.Now.Year + " Shows Garage"')
            content = content.replace('$"({item.Scale})"', '"(" + item.Scale + ")"')
            
            # 2. Viewbag?.ToString() pattern
            content = re.sub(r'!string\.IsNullOrEmpty\(ViewBag\.(\w+)\?\.\s*ToString\(\)\)', r'ViewBag.\1 != null && !string.IsNullOrEmpty(ViewBag.\1.ToString())', content)
            content = re.sub(r'!string\.IsNullOrEmpty\(Model\?\.\s*(\w+)\)', r'Model != null && !string.IsNullOrEmpty(Model.\1)', content)
            
            # 3. Specific Null Conditionals
            content = content.replace('ViewBag.Filter?.ToString() ?? "ALL"', 'ViewBag.Filter != null ? ViewBag.Filter.ToString() : "ALL"')
            content = content.replace('ViewBag.PaymentMethod?.ToString() ?? "Online"', 'ViewBag.PaymentMethod != null ? ViewBag.PaymentMethod.ToString() : "Online"')
            content = content.replace('ViewBag.SiteTitle?.ToString()?.ToUpper() ?? "SHOW\'S GARAGE"', 'ViewBag.SiteTitle != null ? ViewBag.SiteTitle.ToString().ToUpper() : "SHOW\'S GARAGE"')
            content = content.replace('o.EstimatedDeliveryDate?.ToString("yyyy-MM-dd")', '(o.EstimatedDeliveryDate.HasValue ? o.EstimatedDeliveryDate.Value.ToString("yyyy-MM-dd") : "")')
            content = content.replace('b.Excerpt?.Length>80?b.Excerpt.Substring(0,80)+"...":b.Excerpt', '!string.IsNullOrEmpty(b.Excerpt) && b.Excerpt.Length > 80 ? b.Excerpt.Substring(0,80) + "..." : b.Excerpt')
            content = content.replace('b.Author?.Replace', '(b.Author != null ? b.Author.Replace') # handled carefully if needed
            content = content.replace('cartItems?.Sum(i => i.LineTotal) ?? 0', 'cartItems != null ? cartItems.Sum(i => i.LineTotal) : 0')
            
            # SiteSettings Model null checks
            for prop in ["LogoTitle", "Copyright", "HeroTitle", "HeroSubtitle", "Email", "PhoneNumber", "ShippingCharges", "UpiId", "BankAccountDetails"]:
                content = content.replace(f"Model?.{prop}", f"(Model != null ? Model.{prop} : null)")
                
            # Shop Index CategoryName
            content = content.replace('Model.Categories.Find(c => c.CategoryId == Model.CategoryId)?.CategoryName ?? "Products"', '(Model.Categories.Find(c => c.CategoryId == Model.CategoryId) != null ? Model.Categories.Find(c => c.CategoryId == Model.CategoryId).CategoryName : "Products")')

            if content != orig:
                with open(path, 'w', encoding='utf-8') as file:
                    file.write(content)
                print(f"Cleaned C# 6 syntax in {f}")
