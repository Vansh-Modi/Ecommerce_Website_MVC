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
    public class AdminController : Controller
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        private bool IsAdmin => Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (!IsAdmin)
            {
                ctx.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(ctx);
        }

        public ActionResult Dashboard()
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                const string countsSql = @"
                    SELECT
                        (SELECT COUNT(OrderID) FROM [dbo].[Orders] WHERE Status = 'Awaiting Verification') AS AwaitingVerify,
                        (SELECT COUNT(ProductID) FROM [dbo].[Products]) AS TotalProducts,
                        (SELECT COUNT(UserID) FROM [dbo].[Users]) AS TotalUsers,
                        (SELECT COUNT(contactId) FROM [dbo].[contactUs]) AS TotalSupportTickets";

                using (var cmd = new SqlCommand(countsSql, conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        ViewBag.AwaitingVerify    = rdr["AwaitingVerify"].ToString();
                        ViewBag.TotalProducts     = rdr["TotalProducts"].ToString();
                        ViewBag.TotalUsers        = rdr["TotalUsers"].ToString();
                        ViewBag.TotalSupportTickets = rdr["TotalSupportTickets"].ToString();
                    }
                }

                const string mtdSql = @"
                    SELECT ISNULL(SUM(TotalAmount),0)
                    FROM [dbo].[Orders]
                    WHERE Status = 'Approved'
                    AND OrderDate >= DATEADD(month, DATEDIFF(month,0,GETDATE()),0)";

                using (var cmd = new SqlCommand(mtdSql, conn))
                    ViewBag.MtdRevenue = string.Format("{0:N2}", Convert.ToDecimal(cmd.ExecuteScalar()));

                var recentOrders = new List<OrderModel>();
                using (var cmd = new SqlCommand(
                    "SELECT TOP 5 OrderID, OrderDate, TotalAmount, Status, ShippingAddress FROM [dbo].[Orders] ORDER BY OrderDate DESC", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        recentOrders.Add(new OrderModel
                        {
                            OrderId         = Convert.ToInt32(rdr["OrderID"]),
                            OrderDate       = Convert.ToDateTime(rdr["OrderDate"]),
                            TotalAmount     = Convert.ToDecimal(rdr["TotalAmount"]),
                            Status          = rdr["Status"].ToString(),
                            ShippingAddress = rdr["ShippingAddress"].ToString()
                        });
                }
                ViewBag.RecentOrders = recentOrders;

                var lowStock = new List<ProductModel>();
                using (var cmd = new SqlCommand(
                    "SELECT TOP 6 Title, Scale, BrandName, StockQuantity FROM [dbo].[Products] WHERE StockQuantity<=2 ORDER BY StockQuantity ASC, Title ASC", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        lowStock.Add(new ProductModel
                        {
                            Title         = rdr["Title"].ToString(),
                            Scale         = rdr["Scale"].ToString(),
                            BrandName     = rdr["BrandName"].ToString(),
                            StockQuantity = Convert.ToInt32(rdr["StockQuantity"])
                        });
                }
                ViewBag.LowStock = lowStock;
            }
            return View();
        }


        public ActionResult Orders(string filter = "ALL")
        {
            var orders = new List<OrderModel>();
            string q = @"
                SELECT OrderID, UserID, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod,
                       ISNULL(PaymentScreenshotPath,'') AS PaymentScreenshotPath,
                       ISNULL(TransactionReference,'') AS TransactionReference,
                       ISNULL(TrackingPartner,'') AS TrackingPartner,
                       ISNULL(TrackingNumber,'') AS TrackingNumber, EstimatedDeliveryDate
                FROM [dbo].[Orders]";

            q += filter == "ALL"
                ? " WHERE Status <> 'Cancelled' ORDER BY CASE WHEN Status='Awaiting Verification' THEN 1 WHEN Status='Approved' THEN 2 WHEN Status='Shipped' THEN 3 ELSE 4 END, OrderDate DESC"
                : " WHERE Status = @fs ORDER BY OrderDate DESC";

            using (var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = new SqlCommand(q, conn))
                {
                    if (filter != "ALL") cmd.Parameters.AddWithValue("@fs", filter);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var o = new OrderModel
                            {
                                OrderId               = Convert.ToInt32(rdr["OrderID"]),
                                UserId                = Convert.ToInt32(rdr["UserID"]),
                                OrderDate             = Convert.ToDateTime(rdr["OrderDate"]),
                                TotalAmount           = Convert.ToDecimal(rdr["TotalAmount"]),
                                Status                = rdr["Status"].ToString(),
                                ShippingAddress       = rdr["ShippingAddress"].ToString(),
                                PaymentMethod         = rdr["PaymentMethod"].ToString(),
                                PaymentScreenshotPath = rdr["PaymentScreenshotPath"].ToString(),
                                TransactionReference  = rdr["TransactionReference"].ToString(),
                                TrackingPartner       = rdr["TrackingPartner"].ToString(),
                                TrackingNumber        = rdr["TrackingNumber"].ToString(),
                                EstimatedDeliveryDate = rdr["EstimatedDeliveryDate"] == DBNull.Value
                                                        ? (DateTime?)null
                                                        : Convert.ToDateTime(rdr["EstimatedDeliveryDate"])
                            };
                            orders.Add(o);
                        }
                    }
                }

                foreach (var o in orders)
                {
                    using (var cmd = new SqlCommand(@"
                        SELECT od.Quantity, od.UnitPrice, p.Title, p.Scale, p.BrandName
                        FROM [dbo].[OrderDetails] od
                        INNER JOIN [dbo].[Products] p ON od.ProductID = p.ProductID
                        WHERE od.OrderID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", o.OrderId);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                                o.Items.Add(new OrderDetailModel
                                {
                                    Title     = rdr["Title"].ToString(),
                                    BrandName = rdr["BrandName"].ToString(),
                                    Scale     = rdr["Scale"].ToString(),
                                    Quantity  = Convert.ToInt32(rdr["Quantity"]),
                                    UnitPrice = Convert.ToDecimal(rdr["UnitPrice"])
                                });
                        }
                    }
                }
            }

            ViewBag.Filter = filter;
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrder(int orderId, string txnRef, string status, string trackPartner, string trackNum, string estDate)
        {
            string currentStatus = "";
            using (var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT Status FROM [dbo].[Orders] WHERE OrderID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    conn.Open();
                    currentStatus = cmd.ExecuteScalar()?.ToString();
                }
            }

            if (status == "Cancelled" && !currentStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                RollbackOrderStock(orderId);

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                UPDATE [dbo].[Orders]
                SET TransactionReference=@txn, Status=@st, TrackingPartner=@tp, TrackingNumber=@tn, EstimatedDeliveryDate=@ed
                WHERE OrderID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@txn", txnRef ?? "");
                cmd.Parameters.AddWithValue("@st",  status);
                cmd.Parameters.AddWithValue("@tp",  string.IsNullOrEmpty(trackPartner) ? (object)DBNull.Value : trackPartner);
                cmd.Parameters.AddWithValue("@tn",  string.IsNullOrEmpty(trackNum)     ? (object)DBNull.Value : trackNum);
                cmd.Parameters.AddWithValue("@ed",  string.IsNullOrEmpty(estDate)      ? (object)DBNull.Value : (object)Convert.ToDateTime(estDate));
                cmd.Parameters.AddWithValue("@id",  orderId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = $"âœ“ Order #{orderId} updated successfully.";
            return RedirectToAction("Orders");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectOrder(int orderId)
        {
            RollbackOrderStock(orderId);
            TempData["Success"] = $"âš ï¸ Order #{orderId} rejected. Stock reverted.";
            return RedirectToAction("Orders", new { filter = "Cancelled" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveOrder(int orderId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("UPDATE [dbo].[Orders] SET Status='Approved' WHERE OrderID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", orderId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = $"âœ“ Order #{orderId} approved.";
            return RedirectToAction("Orders");
        }

        private void RollbackOrderStock(int orderId)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        decimal totalAmount = 0;
                        using (var cmd = new SqlCommand("SELECT TotalAmount, Status FROM [dbo].[Orders] WHERE OrderID=@id", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            using (var rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    if (rdr["Status"].ToString().Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                                    { trans.Rollback(); return; }
                                    totalAmount = Convert.ToDecimal(rdr["TotalAmount"]);
                                }
                            }
                        }

                        using (var cmd = new SqlCommand("UPDATE [dbo].[Orders] SET Status='Cancelled' WHERE OrderID=@id", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        var lines = new DataTable();
                        using (var cmd = new SqlCommand("SELECT ProductID, Quantity FROM [dbo].[OrderDetails] WHERE OrderID=@id", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            new SqlDataAdapter(cmd).Fill(lines);
                        }

                        foreach (DataRow row in lines.Rows)
                        {
                            using (var cmd = new SqlCommand("UPDATE [dbo].[Products] SET StockQuantity=StockQuantity+@q WHERE ProductID=@p", conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@q", Convert.ToInt32(row["Quantity"]));
                                cmd.Parameters.AddWithValue("@p", Convert.ToInt32(row["ProductID"]));
                                cmd.ExecuteNonQuery();
                            }
                        }

                        using (var cmd = new SqlCommand(@"
                            INSERT INTO [dbo].[Expenses](Title,Amount,ExpenseType,ExpenseDate,OrderID,Remarks)
                            VALUES(@t,@a,@et,@ed,@oid,@rem)", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@t",   $"Customer Refund for Cancelled Order #{orderId}");
                            cmd.Parameters.AddWithValue("@a",   totalAmount);
                            cmd.Parameters.AddWithValue("@et",  "Damaged Return");
                            cmd.Parameters.AddWithValue("@ed",  DateTime.Now);
                            cmd.Parameters.AddWithValue("@oid", orderId);
                            cmd.Parameters.AddWithValue("@rem", $"PENDING MANUAL TRANSFER: Rejected on {DateTime.Now:dd MMM yyyy}.");
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch { trans.Rollback(); throw; }
                }
            }
        }


        public ActionResult Products()
        {
            ViewBag.Products   = GetAllProducts();
            ViewBag.Categories = GetAllCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveProduct(int? productId, string title, string brandName, string scale, int categoryId,
            string description, decimal costPrice, decimal sellingPrice, decimal mrp, int stockQuantity,
            bool isNewArrival, HttpPostedFileBase productImg, string currentImgPath)
        {
            bool isEditing      = productId.HasValue && productId.Value > 0;
            string finalImgPath = isEditing ? currentImgPath : "/Assets/images/default-model.png";

            if (productImg != null && productImg.ContentLength > 0)
            {
                string ext = Path.GetExtension(productImg.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp")
                {
                    string dir = Server.MapPath("~/Assets/uploads/products/");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    string fname = "Model_" + DateTime.Now.Ticks + ext;
                    productImg.SaveAs(Path.Combine(dir, fname));
                    finalImgPath = "/Assets/uploads/products/" + fname;
                }
            }

            int activeId = 0;
            using (var conn = new SqlConnection(ConnStr))
            {
                string sql = isEditing
                    ? @"UPDATE [dbo].[Products]
                        SET Title=@t, BrandName=@b, CategoryID=@c, Scale=@sc, Description=@d,
                            SellingPrice=@sp, CostPrice=@cp, MRP=@mrp, StockQuantity=@sq, ImagePath=@img, IsNewArrival=@na
                        WHERE ProductID=@pid"
                    : @"INSERT INTO [dbo].[Products](Title,BrandName,CategoryID,Scale,Description,SellingPrice,CostPrice,MRP,StockQuantity,ImagePath,IsNewArrival,CreatedAt)
                        VALUES(@t,@b,@c,@sc,@d,@sp,@cp,@mrp,@sq,@img,@na,@ca); SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@t",   title);
                    cmd.Parameters.AddWithValue("@b",   string.IsNullOrEmpty(brandName) ? (object)DBNull.Value : brandName);
                    cmd.Parameters.AddWithValue("@c",   categoryId);
                    cmd.Parameters.AddWithValue("@sc",  string.IsNullOrEmpty(scale) ? (object)DBNull.Value : scale);
                    cmd.Parameters.AddWithValue("@d",   string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                    cmd.Parameters.AddWithValue("@sp",  sellingPrice);
                    cmd.Parameters.AddWithValue("@cp",  costPrice);
                    cmd.Parameters.AddWithValue("@mrp", mrp);
                    cmd.Parameters.AddWithValue("@sq",  stockQuantity);
                    cmd.Parameters.AddWithValue("@img", finalImgPath);
                    cmd.Parameters.AddWithValue("@na",  isNewArrival);
                    if (isEditing) cmd.Parameters.AddWithValue("@pid", productId.Value);
                    else           cmd.Parameters.AddWithValue("@ca", DateTime.Now);

                    conn.Open();
                    activeId = isEditing ? productId.Value : Convert.ToInt32(cmd.ExecuteScalar());
                    if (isEditing) cmd.ExecuteNonQuery();
                }
            }

            TempData["Success"] = isEditing ? "âœ“ Product updated." : "âœ“ Product added.";
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int productId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("DELETE FROM [dbo].[Products] WHERE ProductID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = "âœ“ Product deleted.";
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCategory(int? categoryId, string categoryName)
        {
            bool isEditing = categoryId.HasValue && categoryId.Value > 0;
            string sql = isEditing
                ? "UPDATE [dbo].[Categories] SET CategoryName=@n WHERE CategoryID=@id"
                : "INSERT INTO [dbo].[Categories](CategoryName) VALUES(@n)";

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@n", categoryName.Trim());
                if (isEditing) cmd.Parameters.AddWithValue("@id", categoryId.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = isEditing ? "âœ“ Category updated." : "âœ“ Category added.";
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int categoryId)
        {
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("DELETE FROM [dbo].[Categories] WHERE CategoryID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", categoryId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["Success"] = "âœ“ Category deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "âŒ Cannot delete: " + ex.Message;
            }
            return RedirectToAction("Products");
        }


        public ActionResult Users()
        {
            var users = new List<UserModel>();
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT UserID, FullName, Phone, Email, Role, IsVerified FROM [dbo].[Users] ORDER BY UserID DESC", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        users.Add(new UserModel
                        {
                            UserId     = Convert.ToInt32(rdr["UserID"]),
                            FullName   = rdr["FullName"].ToString(),
                            Phone      = rdr["Phone"].ToString(),
                            Email      = rdr["Email"].ToString(),
                            Role       = rdr["Role"].ToString(),
                            IsVerified = Convert.ToBoolean(rdr["IsVerified"])
                        });
            }
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(int userId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("DELETE FROM [dbo].[Users] WHERE UserID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = "âœ“ User deleted.";
            return RedirectToAction("Users");
        }


        public ActionResult Blog()
        {
            var blogs = new List<BlogModel>();
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT * FROM [dbo].[Blogs] ORDER BY PublishDate DESC", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        blogs.Add(new BlogModel
                        {
                            BlogId      = Convert.ToInt32(rdr["BlogId"]),
                            BlogTitle   = rdr["BlogTitle"].ToString(),
                            Excerpt     = rdr["Excerpt"].ToString(),
                            BlogImage   = rdr["BlogImage"].ToString(),
                            PublishDate = Convert.ToDateTime(rdr["PublishDate"]),
                            
                        });
            }
            return View(blogs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBlog(int? blogId, string blogTitle, string excerpt, string content,
            string author, HttpPostedFileBase blogImage, string currentBlogImage)
        {
            bool isEditing  = blogId.HasValue && blogId.Value > 0;
            string imgPath  = isEditing ? currentBlogImage : "";

            if (blogImage != null && blogImage.ContentLength > 0)
            {
                string dir = Server.MapPath("~/Assets/uploads/blog/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string ext   = Path.GetExtension(blogImage.FileName).ToLower();
                string fname = "Blog_" + DateTime.Now.Ticks + ext;
                blogImage.SaveAs(Path.Combine(dir, fname));
                imgPath = "/Assets/uploads/blog/" + fname;
            }

            string sql = isEditing
                ? "UPDATE [dbo].[Blogs] SET BlogTitle=@t, Excerpt=@e, Content=@c, Author=@a, BlogImage=@img WHERE BlogId=@id"
                : "INSERT INTO [dbo].[Blogs](BlogTitle,Excerpt,Content,Author,BlogImage,PublishDate) VALUES(@t,@e,@c,@a,@img,@pub)";

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@t",   blogTitle);
                cmd.Parameters.AddWithValue("@e",   excerpt);
                cmd.Parameters.AddWithValue("@c",   content);
                cmd.Parameters.AddWithValue("@a",   author ?? "Admin");
                cmd.Parameters.AddWithValue("@img", imgPath);
                if (isEditing) cmd.Parameters.AddWithValue("@id",  blogId.Value);
                else           cmd.Parameters.AddWithValue("@pub", DateTime.Now);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = isEditing ? "âœ“ Blog updated." : "âœ“ Blog published.";
            return RedirectToAction("Blog");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBlog(int blogId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("DELETE FROM [dbo].[Blogs] WHERE BlogId=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", blogId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = "âœ“ Blog deleted.";
            return RedirectToAction("Blog");
        }


        public ActionResult Expenses()
        {
            var expenses = new List<ExpenseModel>();
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT * FROM [dbo].[Expenses] ORDER BY ExpenseDate DESC", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        expenses.Add(new ExpenseModel
                        {
                            ExpenseId   = Convert.ToInt32(rdr["ExpenseId"]),
                            Title       = rdr["Title"].ToString(),
                            Amount      = Convert.ToDecimal(rdr["Amount"]),
                            ExpenseType = rdr["ExpenseType"].ToString(),
                            ExpenseDate = Convert.ToDateTime(rdr["ExpenseDate"]),
                            Remarks     = rdr["Remarks"].ToString()
                        });
            }
            return View(expenses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveExpense(int? expenseId, string title, decimal amount, string expenseType, string expenseDate, string remarks)
        {
            bool isEditing = expenseId.HasValue && expenseId.Value > 0;
            string sql = isEditing
                ? "UPDATE [dbo].[Expenses] SET Title=@t, Amount=@a, ExpenseType=@et, ExpenseDate=@ed, Remarks=@r WHERE ExpenseId=@id"
                : "INSERT INTO [dbo].[Expenses](Title,Amount,ExpenseType,ExpenseDate,Remarks) VALUES(@t,@a,@et,@ed,@r)";

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@t",  title);
                cmd.Parameters.AddWithValue("@a",  amount);
                cmd.Parameters.AddWithValue("@et", expenseType);
                cmd.Parameters.AddWithValue("@ed", Convert.ToDateTime(expenseDate));
                cmd.Parameters.AddWithValue("@r",  remarks ?? "");
                if (isEditing) cmd.Parameters.AddWithValue("@id", expenseId.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = isEditing ? "âœ“ Expense updated." : "âœ“ Expense added.";
            return RedirectToAction("Expenses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteExpense(int expenseId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("DELETE FROM [dbo].[Expenses] WHERE ExpenseId=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", expenseId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = "âœ“ Expense deleted.";
            return RedirectToAction("Expenses");
        }


        public ActionResult Reports()
        {
            var report = new ReportModel();
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT
                        (SELECT ISNULL(SUM(TotalAmount),0) FROM [dbo].[Orders] WHERE Status='Approved') AS TotalRevenue,
                        (SELECT ISNULL(SUM(Amount),0) FROM [dbo].[Expenses]) AS TotalExpenses,
                        (SELECT COUNT(*) FROM [dbo].[Orders]) AS TotalOrders,
                        (SELECT COUNT(*) FROM [dbo].[Orders] WHERE Status='Approved') AS ApprovedOrders,
                        (SELECT COUNT(*) FROM [dbo].[Orders] WHERE Status='Cancelled') AS CancelledOrders,
                        (SELECT ISNULL(SUM(TotalAmount),0) FROM [dbo].[Orders] WHERE Status='Approved' AND OrderDate >= DATEADD(month,DATEDIFF(month,0,GETDATE()),0)) AS MtdRevenue", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        report.TotalRevenue    = Convert.ToDecimal(rdr["TotalRevenue"]);
                        report.TotalExpenses   = Convert.ToDecimal(rdr["TotalExpenses"]);
                        report.NetProfit       = report.TotalRevenue - report.TotalExpenses;
                        report.TotalOrdersCount= Convert.ToInt32(rdr["TotalOrders"]);
                        report.ApprovedOrders  = Convert.ToInt32(rdr["ApprovedOrders"]);
                        report.CancelledOrders = Convert.ToInt32(rdr["CancelledOrders"]);
                        report.MtdRevenue      = Convert.ToDecimal(rdr["MtdRevenue"]);
                    }
                }
            }
            return View(report);
        }


        public ActionResult SiteSettings()
        {
            SiteSettingsModel settings = null;
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT TOP 1 * FROM [dbo].[SiteSettings] WHERE SiteSettingId=1", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        settings = new SiteSettingsModel
                        {
                            SiteSettingId     = 1,
                            Logo              = rdr["Logo"].ToString(),
                            LogoTitle         = rdr["LogoTitle"].ToString(),
                            HeroImg           = rdr["HeroImg"].ToString(),
                            HeroTitle         = rdr["HeroTitle"].ToString(),
                            HeroSubtitle      = rdr["HeroSubtitle"].ToString(),
                            Copyright         = rdr["Copyright"].ToString(),
                            Email             = rdr["Email"].ToString(),
                            PhoneNumber       = rdr["PhoneNumber"].ToString(),
                            ShippingCharges   = Convert.ToDecimal(rdr["ShippingCharges"]),
                            QrCodePath        = rdr["QrCodePath"].ToString(),
                            UpiId             = rdr["UpiID"].ToString(),
                            BankAccountDetails= rdr["BankAccountDetails"].ToString()
                        };
                }
            }
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSiteSettings(SiteSettingsModel model, HttpPostedFileBase logoFile, HttpPostedFileBase heroFile, HttpPostedFileBase qrFile)
        {
            if (logoFile  != null && logoFile.ContentLength  > 0) model.Logo       = SaveUpload(logoFile,  "logos");
            if (heroFile  != null && heroFile.ContentLength  > 0) model.HeroImg     = SaveUpload(heroFile,  "hero");
            if (qrFile    != null && qrFile.ContentLength    > 0) model.QrCodePath  = SaveUpload(qrFile,   "qr");

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                UPDATE [dbo].[SiteSettings]
                SET Logo=@logo, LogoTitle=@lt, HeroImg=@hi, HeroTitle=@ht, HeroSubtitle=@hs,
                    Copyright=@copy, Email=@email, PhoneNumber=@phone, ShippingCharges=@ship,
                    QrCodePath=@qr, UpiID=@upi, BankAccountDetails=@bank
                WHERE SiteSettingId=1", conn))
            {
                cmd.Parameters.AddWithValue("@logo",  model.Logo ?? "");
                cmd.Parameters.AddWithValue("@lt",    model.LogoTitle ?? "");
                cmd.Parameters.AddWithValue("@hi",    model.HeroImg ?? "");
                cmd.Parameters.AddWithValue("@ht",    model.HeroTitle ?? "");
                cmd.Parameters.AddWithValue("@hs",    model.HeroSubtitle ?? "");
                cmd.Parameters.AddWithValue("@copy",  model.Copyright ?? "");
                cmd.Parameters.AddWithValue("@email", model.Email ?? "");
                cmd.Parameters.AddWithValue("@phone", model.PhoneNumber ?? "");
                cmd.Parameters.AddWithValue("@ship",  model.ShippingCharges);
                cmd.Parameters.AddWithValue("@qr",    model.QrCodePath ?? "");
                cmd.Parameters.AddWithValue("@upi",   model.UpiId ?? "");
                cmd.Parameters.AddWithValue("@bank",  model.BankAccountDetails ?? "");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = "âœ“ Site settings saved.";
            return RedirectToAction("SiteSettings");
        }


        private string SaveUpload(HttpPostedFileBase file, string subfolder)
        {
            string dir = Server.MapPath($"~/Assets/uploads/{subfolder}/");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string ext  = Path.GetExtension(file.FileName).ToLower();
            string name = subfolder + "_" + DateTime.Now.Ticks + ext;
            file.SaveAs(Path.Combine(dir, name));
            return $"/Assets/uploads/{subfolder}/{name}";
        }

        private List<ProductModel> GetAllProducts()
        {
            var list = new List<ProductModel>();
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT p.*, c.CategoryName FROM [dbo].[Products] p
                INNER JOIN [dbo].[Categories] c ON p.CategoryID = c.CategoryID
                ORDER BY p.ProductID DESC", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        list.Add(new ProductModel
                        {
                            ProductId    = Convert.ToInt32(rdr["ProductID"]),
                            Title        = rdr["Title"].ToString(),
                            BrandName    = rdr["BrandName"].ToString(),
                            CategoryId   = Convert.ToInt32(rdr["CategoryID"]),
                            CategoryName = rdr["CategoryName"].ToString(),
                            Scale        = rdr["Scale"].ToString(),
                            SellingPrice = Convert.ToDecimal(rdr["SellingPrice"]),
                            CostPrice    = Convert.ToDecimal(rdr["CostPrice"]),
                            MRP          = Convert.ToDecimal(rdr["MRP"]),
                            StockQuantity= Convert.ToInt32(rdr["StockQuantity"]),
                            ImagePath    = rdr["ImagePath"].ToString(),
                            IsNewArrival = Convert.ToBoolean(rdr["IsNewArrival"]),
                            Description  = rdr["Description"].ToString()
                        });
            }
            return list;
        }

        private List<CategoryModel> GetAllCategories()
        {
            var list = new List<CategoryModel>();
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT CategoryID, CategoryName FROM [dbo].[Categories] ORDER BY CategoryName ASC", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        list.Add(new CategoryModel
                        {
                            CategoryId   = Convert.ToInt32(rdr["CategoryID"]),
                            CategoryName = rdr["CategoryName"].ToString()
                        });
            }
            return list;
        }
    }
}



