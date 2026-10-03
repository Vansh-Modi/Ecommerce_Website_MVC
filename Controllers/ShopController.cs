using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Ecommerce_Website_MVC.Models;

namespace Ecommerce_Website_MVC.Controllers
{
    public class ShopController : Controller
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        // GET: 
        public ActionResult Index(string search = null, int categoryId = 0)
        {
            var vm = new ShopViewModel();

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                using (var cmd = new SqlCommand("SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName ASC", conn))
                {
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                        vm.Categories.Add(new CategoryModel { CategoryId = Convert.ToInt32(r["CategoryID"]), CategoryName = r["CategoryName"].ToString() });
                }

                string q;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    q = @"SELECT ProductID, Title, BrandName, MRP, SellingPrice, ImagePath, ISNULL(StockQuantity,0) AS StockQuantity
                          FROM Products WHERE Title LIKE @s OR BrandName LIKE @s ORDER BY ProductID DESC";
                }
                else if (categoryId > 0)
                {
                    q = @"SELECT ProductID, Title, BrandName, MRP, SellingPrice, ImagePath, ISNULL(StockQuantity,0) AS StockQuantity
                          FROM Products WHERE CategoryID = @catID ORDER BY ProductID DESC";
                }
                else
                {
                    q = @"SELECT ProductID, Title, BrandName, MRP, SellingPrice, ImagePath, ISNULL(StockQuantity,0) AS StockQuantity
                          FROM Products ORDER BY ProductID DESC";
                }

                using (var cmd = new SqlCommand(q, conn))
                {
                    if (!string.IsNullOrWhiteSpace(search))
                        cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                    else if (categoryId > 0)
                        cmd.Parameters.AddWithValue("@catID", categoryId);

                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                    {
                        vm.Products.Add(new ProductModel
                        {
                            ProductId    = Convert.ToInt32(r["ProductID"]),
                            Title        = r["Title"].ToString(),
                            BrandName    = r["BrandName"].ToString(),
                            MRP          = Convert.ToDecimal(r["MRP"]),
                            SellingPrice = Convert.ToDecimal(r["SellingPrice"]),
                            ImagePath    = r["ImagePath"].ToString(),
                            StockQuantity= Convert.ToInt32(r["StockQuantity"])
                        });
                    }
                }
            }

            vm.Search     = search;
            vm.CategoryId = categoryId;
            return View(vm);
        }

        // GET: /Shop/Details/5
        public ActionResult Details(int id)
        {
            ProductModel product = null;
            var galleryImages    = new List<string>();

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                using (var cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            product = new ProductModel
                            {
                                ProductId    = Convert.ToInt32(rdr["ProductID"]),
                                Title        = rdr["Title"].ToString(),
                                BrandName    = rdr["BrandName"].ToString(),
                                CategoryId   = Convert.ToInt32(rdr["CategoryID"]),
                                Scale        = rdr["Scale"].ToString(),
                                Description  = rdr["Description"].ToString(),
                                SellingPrice = Convert.ToDecimal(rdr["SellingPrice"]),
                                MRP          = Convert.ToDecimal(rdr["MRP"]),
                                StockQuantity= Convert.ToInt32(rdr["StockQuantity"]),
                                ImagePath    = rdr["ImagePath"].ToString(),
                                IsNewArrival = Convert.ToBoolean(rdr["IsNewArrival"]),
                                CreatedAt    = Convert.ToDateTime(rdr["CreatedAt"])
                            };
                        }
                    }
                }

                if (product == null) return HttpNotFound();

                using (var cmd = new SqlCommand(
                    "SELECT ImagePath FROM [dbo].[ProductGallery] WHERE ProductID = @id ORDER BY GalleryID ASC", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            galleryImages.Add(rdr["ImagePath"].ToString());
                }
            }

            ViewBag.GalleryImages = galleryImages;
            return View(product);
        }

        // POST: /Shop/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", "Shop", new { id = productId }) });

            int userId = Convert.ToInt32(Session["UserID"]);

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Quantity FROM Cart WHERE UserID=@u AND ProductID=@p", conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@p", productId);
                    var existing = cmd.ExecuteScalar();
                    if (existing != null)
                    {
                        using (var upd = new SqlCommand("UPDATE Cart SET Quantity=Quantity+@q WHERE UserID=@u AND ProductID=@p", conn))
                        {
                            upd.Parameters.AddWithValue("@q", quantity);
                            upd.Parameters.AddWithValue("@u", userId);
                            upd.Parameters.AddWithValue("@p", productId);
                            upd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var ins = new SqlCommand("INSERT INTO Cart (UserID, ProductID, Quantity) VALUES (@u,@p,@q)", conn))
                        {
                            ins.Parameters.AddWithValue("@u", userId);
                            ins.Parameters.AddWithValue("@p", productId);
                            ins.Parameters.AddWithValue("@q", quantity);
                            ins.ExecuteNonQuery();
                        }
                    }
                }
            }

            TempData["CartSuccess"] = "Item added to cart!";
            return RedirectToAction("Index", "Cart");
        }
    }

    public class ShopViewModel
    {
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
        public List<ProductModel>  Products   { get; set; } = new List<ProductModel>();
        public string Search     { get; set; }
        public int    CategoryId { get; set; }
    }
}

