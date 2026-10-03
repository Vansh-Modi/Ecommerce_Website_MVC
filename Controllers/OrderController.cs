using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Ecommerce_Website_MVC.Models;

namespace Ecommerce_Website_MVC.Controllers
{
    public class OrderController : Controller
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        private bool IsLoggedIn => Session["UserID"] != null;

        // GET: /Order/MyOrders
        public ActionResult MyOrders()
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });

            int userId = Convert.ToInt32(Session["UserID"]);
            var orders = new List<OrderModel>();

            const string q = @"
                SELECT OrderID, OrderDate, TotalAmount, Status, PaymentMethod, TrackingPartner, TrackingNumber, EstimatedDeliveryDate
                FROM [dbo].[Orders]
                WHERE UserID = @uid
                ORDER BY OrderDate DESC";

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(q, conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        orders.Add(new OrderModel
                        {
                            OrderId                = Convert.ToInt32(rdr["OrderID"]),
                            OrderDate              = Convert.ToDateTime(rdr["OrderDate"]),
                            TotalAmount            = Convert.ToDecimal(rdr["TotalAmount"]),
                            Status                 = rdr["Status"].ToString(),
                            PaymentMethod          = rdr["PaymentMethod"].ToString(),
                            TrackingPartner        = rdr["TrackingPartner"].ToString(),
                            TrackingNumber         = rdr["TrackingNumber"].ToString(),
                            EstimatedDeliveryDate  = rdr["EstimatedDeliveryDate"] == DBNull.Value
                                                     ? (DateTime?)null
                                                     : Convert.ToDateTime(rdr["EstimatedDeliveryDate"])
                        });
                    }
                }
            }
            return View(orders);
        }

        // GET: /Order/Details/5
        public ActionResult Details(int id)
        {
            if (!IsLoggedIn) return RedirectToAction("Login", "Account");

            OrderModel order = null;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                using (var cmd = new SqlCommand(
                    "SELECT * FROM [dbo].[Orders] WHERE OrderID=@id AND UserID=@uid", conn))
                {
                    cmd.Parameters.AddWithValue("@id",  id);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            order = new OrderModel
                            {
                                OrderId               = Convert.ToInt32(rdr["OrderID"]),
                                OrderDate             = Convert.ToDateTime(rdr["OrderDate"]),
                                TotalAmount           = Convert.ToDecimal(rdr["TotalAmount"]),
                                Status                = rdr["Status"].ToString(),
                                ShippingAddress       = rdr["ShippingAddress"].ToString(),
                                Pincode               = rdr["Pincode"].ToString(),
                                PaymentMethod         = rdr["PaymentMethod"].ToString(),
                                TransactionReference  = rdr["TransactionReference"].ToString(),
                                TrackingPartner       = rdr["TrackingPartner"].ToString(),
                                TrackingNumber        = rdr["TrackingNumber"].ToString(),
                                EstimatedDeliveryDate = rdr["EstimatedDeliveryDate"] == DBNull.Value
                                                        ? (DateTime?)null
                                                        : Convert.ToDateTime(rdr["EstimatedDeliveryDate"])
                            };
                        }
                    }
                }

                if (order == null) return HttpNotFound();

                using (var cmd = new SqlCommand(@"
                    SELECT od.Quantity, od.UnitPrice, p.Title, p.Scale, p.BrandName, p.ImagePath
                    FROM [dbo].[OrderDetails] od
                    INNER JOIN [dbo].[Products] p ON od.ProductID = p.ProductID
                    WHERE od.OrderID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            order.Items.Add(new OrderDetailModel
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
            return View(order);
        }
    }
}

