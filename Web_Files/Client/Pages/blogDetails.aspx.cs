using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class blogDetials : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Grab the "?id=" value from the URL
                string blogId = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(blogId))
                {
                    LoadBlogContent(blogId);
                }
                else
                {
                    // Redirect back home if they try to access the template directly without an ID
                    Response.Redirect("homepage.aspx");
                }
            }
        }

        private void LoadBlogContent(string id)
        {
            // 1. Connect to your database here using the 'id' variable
            //    e.g., "SELECT Title, Body, Date FROM Blogs WHERE BlogID = @id"

            // 2. Mock example of pulling the data:
            if (id == "1")
            {
                lblBlogTitle.InnerText = "How to Detail Your Car";
                lblBlogDate.InnerText = "June 19, 2026";
                divBlogContent.InnerHtml = "<p>This is the full text about detailing your car...</p>";
            }
            else if (id == "2")
            {
                lblBlogTitle.InnerText = "Top 5 Engine Upgrades";
                lblBlogDate.InnerText = "June 10, 2026";
                divBlogContent.InnerHtml = "<p>This is the full text about engine upgrades...</p>";
            }
            else
            {
                lblBlogTitle.InnerText = "Blog Not Found";
                divBlogContent.InnerText = "The blog post you are looking for does not exist.";
            }
        }
    }
}