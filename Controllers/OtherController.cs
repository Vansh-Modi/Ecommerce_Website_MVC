using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Ecommerce_Website_MVC.Controllers
{
    public class OtherController : Controller
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        // GET: /Other/Contact
        public ActionResult Contact()
        {
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("SELECT TOP 1 Email, PhoneNumber FROM [dbo].[SiteSettings] WHERE SiteSettingId=1", conn))
                {
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            ViewBag.Email = rdr["Email"].ToString();
                            ViewBag.Phone = rdr["PhoneNumber"].ToString();
                        }
                    }
                }
            }
            catch { ViewBag.Email = ""; ViewBag.Phone = ""; }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(string name, string email, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Error = "âš ï¸ All fields are required.";
                return View();
            }

            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(
                    "INSERT INTO [dbo].[contactUs] (name, email, message) VALUES (@n, @e, @m)", conn))
                {
                    cmd.Parameters.AddWithValue("@n", name.Trim());
                    cmd.Parameters.AddWithValue("@e", email.Trim());
                    cmd.Parameters.AddWithValue("@m", message.Trim());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["Success"] = "âœ“ Message sent successfully! We will get back to you shortly.";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database Error: " + ex.Message;
                return View();
            }
        }

        // GET: /Other/Privacy
        public ActionResult Privacy() => View();

        // GET: /Other/Shipping
        public ActionResult Shipping() => View();
    }
}

