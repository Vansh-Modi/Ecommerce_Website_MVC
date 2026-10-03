using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class shop : Page
    {
        private const string BasePillStyle = "btn-filter-pill-isolated";
        private const string ActivePillStyle = "btn-filter-pill-isolated active-pill-isolated";

        private string connStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategories();

                if (Request.QueryString["search"] != null)
                {
                    string searchKeyword = Request.QueryString["search"].ToString().Trim();
                    BindProductsFilteredBySearch(searchKeyword);
                }
                else
                {
                    BindProducts(0);
                }
            }
        }

        private void BindCategories()
        {
            const string query = "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName ASC";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        try
                        {
                            con.Open();
                            sda.Fill(dt);
                            rptCategories.DataSource = dt;
                            rptCategories.DataBind();
                        }
                        catch { /* Quiet fallback catch engine */ }
                    }
                }
            }
        }

        private void BindProducts(int categoryId)
        {
            string query = @"SELECT ProductID, Title, BrandName, MRP, SellingPrice, ImagePath, 
                                   ISNULL(StockQuantity, 0) AS StockQuantity 
                            FROM Products";

            if (categoryId > 0)
            {
                query += " WHERE CategoryID = @catID";
            }
            query += " ORDER BY ProductID DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (categoryId > 0)
                    {
                        cmd.Parameters.AddWithValue("@catID", categoryId);
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        try
                        {
                            con.Open();
                            sda.Fill(dt);
                            rptProducts.DataSource = dt;
                            rptProducts.DataBind();
                        }
                        catch { /* Quiet fallback catch engine */ }
                    }
                }
            }
        }

        private void BindProductsFilteredBySearch(string keyword)
        {
            const string query = @"SELECT ProductID, Title, BrandName, MRP, SellingPrice, ImagePath, 
                                   ISNULL(StockQuantity, 0) AS StockQuantity 
                            FROM Products 
                            WHERE Title LIKE @search 
                               OR BrandName LIKE @search 
                            ORDER BY ProductID DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + keyword + "%");

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        try
                        {
                            con.Open();
                            sda.Fill(dt);
                            rptProducts.DataSource = dt;
                            rptProducts.DataBind();
                        }
                        catch { /* Quiet fallback catch engine */ }
                    }
                }
            }
        }

        protected void CategoryFilter_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn != null)
            {
                int categoryId = Convert.ToInt32(btn.CommandArgument);
                BindProducts(categoryId);

                lnkAll.CssClass = BasePillStyle;

                foreach (RepeaterItem item in rptCategories.Items)
                {
                    LinkButton lb = (LinkButton)item.FindControl("lnkCat");
                    if (lb != null)
                    {
                        lb.CssClass = BasePillStyle;
                    }
                }

                btn.CssClass = ActivePillStyle;
            }
        }
    }
}