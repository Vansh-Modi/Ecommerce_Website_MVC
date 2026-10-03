using System;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class order_success : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Security Identity Gate Check synced to match global validation frameworks
            if (Session["UserRole"] == null || Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Retrieve the active tracking Order ID from request parameters
                if (Request.QueryString["id"] != null)
                {
                    lblOrderIdDisplay.Text = Server.HtmlEncode(Request.QueryString["id"]);
                }
                else
                {
                    // Fallback baseline layout placeholder if redirected directly
                    lblOrderIdDisplay.Text = "TRACKING-LOGGED";
                }
            }
        }
    }
}