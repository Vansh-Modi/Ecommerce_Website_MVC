using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Ecommerce_Website_MVC.Models;

namespace Ecommerce_Website_MVC.Controllers
{
    public class HomeController : Controller
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        // GET: /  or  /Home/Index
        public ActionResult Index()
        {
            var vm = new HomeViewModel();

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                using (var cmd = new SqlCommand(
                    "SELECT HeroImg, HeroTitle, HeroSubtitle FROM [dbo].[SiteSettings] WHERE SiteSettingId = 1", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        vm.HeroImg      = rdr["HeroImg"].ToString();
                        vm.HeroTitle    = rdr["HeroTitle"].ToString();
                        vm.HeroSubtitle = rdr["HeroSubtitle"].ToString();
                    }
                }

                using (var cmd = new SqlCommand(
                    "SELECT ProductID, ImagePath, BrandName, Title, MRP, SellingPrice FROM [dbo].[Products] ORDER BY CreatedAt DESC", conn))
                {
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    vm.Products = MapProducts(dt);
                }

                using (var cmd = new SqlCommand(
                    "SELECT TOP 3 BlogId, BlogTitle, Excerpt, BlogImage, PublishDate FROM [dbo].[Blogs] ORDER BY PublishDate DESC", conn))
                {
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    vm.LatestBlogs = MapBlogs(dt);
                }
            }

            return View(vm);
        }


        private List<ProductModel> MapProducts(DataTable dt)
        {
            var list = new List<ProductModel>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ProductModel
                {
                    ProductId    = Convert.ToInt32(r["ProductID"]),
                    ImagePath    = r["ImagePath"].ToString(),
                    BrandName    = r["BrandName"].ToString(),
                    Title        = r["Title"].ToString(),
                    MRP          = Convert.ToDecimal(r["MRP"]),
                    SellingPrice = Convert.ToDecimal(r["SellingPrice"])
                });
            }
            return list;
        }

        private List<BlogModel> MapBlogs(DataTable dt)
        {
            var list = new List<BlogModel>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BlogModel
                {
                    BlogId      = Convert.ToInt32(r["BlogId"]),
                    BlogTitle   = r["BlogTitle"].ToString(),
                    Excerpt     = r["Excerpt"].ToString(),
                    BlogImage   = r["BlogImage"].ToString(),
                    PublishDate = Convert.ToDateTime(r["PublishDate"])
                });
            }
            return list;
        }
    }
    public class HomeViewModel
    {
        public string HeroImg      { get; set; }
        public string HeroTitle    { get; set; }
        public string HeroSubtitle { get; set; }
        public List<ProductModel> Products    { get; set; } = new List<ProductModel>();
        public List<BlogModel>    LatestBlogs { get; set; } = new List<BlogModel>();
    }
}

