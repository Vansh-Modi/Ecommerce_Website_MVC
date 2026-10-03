using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Ecommerce_Website_MVC.Models;

namespace Ecommerce_Website_MVC.Controllers
{
    public class CartController : Controller
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        private bool IsLoggedIn => Session["UserID"] != null && Session["UserRole"] != null;

        public ActionResult Index()
        {
            if (!IsLoggedIn)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            var items = LoadCartFromDb();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int productId, string action)
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserID"]);

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                int stock = 0;
                using (var cmd = new SqlCommand("SELECT StockQuantity FROM Products WHERE ProductID=@p", conn))
                {
                    cmd.Parameters.AddWithValue("@p", productId);
                    stock = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int currentQty = 0;
                using (var cmd = new SqlCommand("SELECT Quantity FROM Cart WHERE UserID=@u AND ProductID=@p", conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@p", productId);
                    currentQty = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int newQty = currentQty;
                if (action == "plus" && currentQty < stock) newQty = currentQty + 1;
                else if (action == "minus" && currentQty > 1) newQty = currentQty - 1;

                if (newQty != currentQty)
                {
                    using (var cmd = new SqlCommand("UPDATE Cart SET Quantity=@q WHERE UserID=@u AND ProductID=@p", conn))
                    {
                        cmd.Parameters.AddWithValue("@q", newQty);
                        cmd.Parameters.AddWithValue("@u", userId);
                        cmd.Parameters.AddWithValue("@p", productId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else if (action == "plus" && currentQty >= stock)
                {
                    TempData["CartError"] = $"Only {stock} unit(s) available in stock.";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItem(int productId)
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserID"]);

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("DELETE FROM Cart WHERE UserID=@u AND ProductID=@p", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@p", productId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");

            var items = LoadCartFromDb();
            if (items.Count == 0) return RedirectToAction("Index");

            decimal shippingFee = GetShippingFee();
            ViewBag.ShippingFee = shippingFee;
            ViewBag.CartItems   = items;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string fullName, string phone, string address, string city, string pincode, string paymentMode)
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");

            var items = LoadCartFromDb();
            if (items.Count == 0) return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address)  || string.IsNullOrWhiteSpace(city)  ||
                string.IsNullOrWhiteSpace(pincode))
            {
                ViewBag.Error       = "âš ï¸ Please fill in all required delivery fields.";
                ViewBag.ShippingFee = GetShippingFee();
                ViewBag.CartItems   = items;
                return View();
            }

            if (city.ToLower() != "surat" && paymentMode == "COD")
            {
                ViewBag.Error       = "âŒ COD is only available within Surat city limits.";
                ViewBag.ShippingFee = GetShippingFee();
                ViewBag.CartItems   = items;
                return View();
            }

            Session["Checkout_FullName"]      = fullName;
            Session["Checkout_Phone"]         = phone;
            Session["Checkout_Address"]       = address;
            Session["Checkout_City"]          = city;
            Session["Checkout_Pincode"]       = pincode;
            Session["Checkout_PaymentMethod"] = paymentMode;

            return RedirectToAction("Payment");
        }


        public ActionResult Payment()
        {
            if (!IsLoggedIn || Session["Checkout_PaymentMethod"] == null)
                return RedirectToAction("Index");

            var items          = LoadCartFromDb();
            string method      = Session["Checkout_PaymentMethod"].ToString();
            decimal shippingFee = method == "COD" ? 0 : GetShippingFee();
            decimal subtotal   = 0;
            items.ForEach(i => subtotal += i.LineTotal);

            ViewBag.PaymentMethod = method;
            ViewBag.Subtotal      = subtotal;
            ViewBag.ShippingFee   = shippingFee;
            ViewBag.TotalDue      = subtotal + shippingFee;
            ViewBag.CartItems     = items;

            if (method != "COD")
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("SELECT TOP 1 QrCodePath, UpiID, BankAccountDetails FROM [dbo].[SiteSettings]", conn))
                {
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            ViewBag.QrCodePath       = rdr["QrCodePath"].ToString();
                            ViewBag.UpiId            = rdr["UpiID"].ToString();
                            ViewBag.BankDetails      = rdr["BankAccountDetails"].ToString();
                        }
                    }
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitPayment(string txnRef, HttpPostedFileBase screenshot)
        {
            if (!IsLoggedIn || Session["Checkout_PaymentMethod"] == null)
                return RedirectToAction("Index");

            int    userId      = Convert.ToInt32(Session["UserID"]);
            string method      = Session["Checkout_PaymentMethod"].ToString();
            var    items       = LoadCartFromDb();
            decimal shippingFee = method == "COD" ? 0 : GetShippingFee();
            decimal subtotal   = 0;
            items.ForEach(i => subtotal += i.LineTotal);
            decimal total = subtotal + shippingFee;

            string relPath  = "COD";
            string txnStore = "COD-ORDER";
            string status   = "Awaiting Verification";

            if (method != "COD")
            {
                if (string.IsNullOrEmpty(txnRef))
                {
                    ViewBag.Error = "âš ï¸ Please enter your transaction UTR / reference number.";
                    return PaymentViewWithData(items, method, subtotal, shippingFee, total);
                }
                if (screenshot == null || screenshot.ContentLength == 0)
                {
                    ViewBag.Error = "âš ï¸ Please upload a screenshot of your payment.";
                    return PaymentViewWithData(items, method, subtotal, shippingFee, total);
                }
                if (screenshot.ContentLength > 2097152)
                {
                    ViewBag.Error = "âš ï¸ Screenshot must be under 2 MB.";
                    return PaymentViewWithData(items, method, subtotal, shippingFee, total);
                }

                string mime = screenshot.ContentType.ToLower();
                if (mime != "image/jpeg" && mime != "image/jpg" && mime != "image/png")
                {
                    ViewBag.Error = "âŒ Only .jpg / .jpeg / .png files are accepted.";
                    return PaymentViewWithData(items, method, subtotal, shippingFee, total);
                }

                try
                {
                    string folder = Server.MapPath("~/Assets/uploads/receipts/");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    string ext  = Path.GetExtension(screenshot.FileName).ToLower();
                    string name = $"Order_Pending_{userId}_{DateTime.Now.Ticks}{ext}";
                    screenshot.SaveAs(Path.Combine(folder, name));
                    relPath  = $"/Assets/uploads/receipts/{name}";
                    txnStore = txnRef.Trim();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "âŒ File Save Error: " + ex.Message;
                    return PaymentViewWithData(items, method, subtotal, shippingFee, total);
                }
            }

            try
            {
                string fullName = Session["Checkout_FullName"].ToString();
                string phone    = Session["Checkout_Phone"].ToString();
                string address  = Session["Checkout_Address"].ToString();
                string city     = Session["Checkout_City"].ToString();
                string pincode  = Session["Checkout_Pincode"].ToString();

                int generatedOrderId;
                using (var conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    try
                    {
                        const string insertOrder = @"
                            INSERT INTO [dbo].[Orders]
                            (UserID, OrderDate, TotalAmount, Status, ShippingAddress, Pincode, PaymentMethod, PaymentScreenshotPath, TransactionReference)
                            OUTPUT INSERTED.OrderID
                            VALUES (@uid, @date, @total, @status, @addr, @pin, @method, @img, @txn)";

                        using (var cmd = new SqlCommand(insertOrder, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@uid",    userId);
                            cmd.Parameters.AddWithValue("@date",   DateTime.Now);
                            cmd.Parameters.AddWithValue("@total",  total);
                            cmd.Parameters.AddWithValue("@status", status);
                            cmd.Parameters.AddWithValue("@addr",   $"{address}, {city} (Phone: {phone}, Name: {fullName})");
                            cmd.Parameters.AddWithValue("@pin",    pincode);
                            cmd.Parameters.AddWithValue("@method", method);
                            cmd.Parameters.AddWithValue("@img",    relPath);
                            cmd.Parameters.AddWithValue("@txn",    txnStore);
                            generatedOrderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (var item in items)
                        {
                            using (var cmd = new SqlCommand(
                                "INSERT INTO [dbo].[OrderDetails](OrderID,ProductID,Quantity,UnitPrice) VALUES(@o,@p,@q,@u)", conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@o", generatedOrderId);
                                cmd.Parameters.AddWithValue("@p", item.ProductId);
                                cmd.Parameters.AddWithValue("@q", item.Quantity);
                                cmd.Parameters.AddWithValue("@u", item.SellingPrice);
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new SqlCommand(
                                "UPDATE [dbo].[Products] SET StockQuantity=StockQuantity-@q WHERE ProductID=@p", conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@q", item.Quantity);
                                cmd.Parameters.AddWithValue("@p", item.ProductId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        using (var cmd = new SqlCommand("DELETE FROM Cart WHERE UserID=@u", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@u", userId);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                    }
                    catch (Exception ex2)
                    {
                        trans.Rollback();
                        throw new Exception("Order submission failed: " + ex2.Message, ex2);
                    }
                }

                Session["Checkout_FullName"] = Session["Checkout_Phone"] = Session["Checkout_Address"] =
                Session["Checkout_City"]     = Session["Checkout_Pincode"] = Session["Checkout_PaymentMethod"] = null;

                return RedirectToAction("Confirm", new { id = generatedOrderId });
            }
            catch (Exception ex3)
            {
                ViewBag.Error = "âŒ Process Error: " + ex3.Message;
                return PaymentViewWithData(items, method, subtotal, shippingFee, total);
            }
        }

        public ActionResult Confirm(int id)
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");
            ViewBag.OrderId = id;
            return View();
        }


        private List<CartItemModel> LoadCartFromDb()
        {
            var list = new List<CartItemModel>();
            if (Session["UserID"] == null) return list;
            int userId = Convert.ToInt32(Session["UserID"]);

            const string q = @"
                SELECT c.ProductID, p.BrandName, p.Title, p.SellingPrice, p.MRP, c.Quantity, p.ImagePath, p.StockQuantity
                FROM Cart c INNER JOIN Products p ON c.ProductID = p.ProductID
                WHERE c.UserID = @uid";

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(q, conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int qty   = Convert.ToInt32(rdr["Quantity"]);
                        int stock = Convert.ToInt32(rdr["StockQuantity"]);
                        if (qty > stock) qty = stock;

                        list.Add(new CartItemModel
                        {
                            ProductId    = Convert.ToInt32(rdr["ProductID"]),
                            BrandName    = rdr["BrandName"].ToString(),
                            Title        = rdr["Title"].ToString(),
                            SellingPrice = Convert.ToDecimal(rdr["SellingPrice"]),
                            MRP          = Convert.ToDecimal(rdr["MRP"]),
                            Quantity     = qty,
                            ImagePath    = rdr["ImagePath"].ToString(),
                            StockQuantity= stock
                        });
                    }
                }
            }
            return list;
        }

        private decimal GetShippingFee()
        {
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("SELECT TOP 1 ISNULL(ShippingCharges, 120.00) FROM [dbo].[SiteSettings]", conn))
                {
                    conn.Open();
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch { return 120.00m; }
        }

        private ActionResult PaymentViewWithData(List<CartItemModel> items, string method, decimal subtotal, decimal shipping, decimal total)
        {
            ViewBag.PaymentMethod = method;
            ViewBag.CartItems     = items;
            ViewBag.Subtotal      = subtotal;
            ViewBag.ShippingFee   = shipping;
            ViewBag.TotalDue      = total;
            return View("Payment");
        }
    }
}

