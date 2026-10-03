using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Ecommerce_Website_MVC.Models;

namespace Ecommerce_Website_MVC.Controllers
{
    public class BlogController : Controller
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        // GET: /Blog
        public ActionResult Index()
        {
            var blogs = new List<BlogModel>();

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT BlogId, BlogTitle, Excerpt, BlogImage, PublishDate, Author FROM [dbo].[Blogs] ORDER BY PublishDate DESC", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        blogs.Add(new BlogModel
                        {
                            BlogId      = Convert.ToInt32(rdr["BlogId"]),
                            BlogTitle   = rdr["BlogTitle"].ToString(),
                            Excerpt     = rdr["Excerpt"].ToString(),
                            BlogImage   = rdr["BlogImage"].ToString(),
                            PublishDate = Convert.ToDateTime(rdr["PublishDate"]),
                            
                        });
                    }
                }
            }
            return View(blogs);
        }

        // GET: /Blog/Details/5
        public ActionResult Details(int id)
        {
            BlogModel blog = null;

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT * FROM [dbo].[Blogs] WHERE BlogId = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        blog = new BlogModel
                        {
                            BlogId      = Convert.ToInt32(rdr["BlogId"]),
                            BlogTitle   = rdr["BlogTitle"].ToString(),
                            Excerpt     = rdr["Excerpt"].ToString(),
                            Content     = rdr["Content"].ToString(),
                            BlogImage   = rdr["BlogImage"].ToString(),
                            PublishDate = Convert.ToDateTime(rdr["PublishDate"]),
                            
                        };
                    }
                }
            }

            if (blog == null) return HttpNotFound();
            return View(blog);
        }
    }
}


