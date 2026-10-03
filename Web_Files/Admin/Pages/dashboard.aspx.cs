using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_dashboard : System.Web.UI.Page
    {
        private string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // Security Identity Authorization Scope Guard Check
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            // Guarantee execution happens solely on authentic baseline page load ticks
            if (!IsPostBack)
            {
                CompileDashboardTelemetryMetrics();
            }
        }

        /// <summary>
        /// Central orchestration routine that aggregates data from Orders, Products, Users, 
        /// and contactUs tables to feed the dashboard UI components.
        /// </summary>
        private void CompileDashboardTelemetryMetrics()
        {
            lblMessage.Visible = false;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    // Query A: Compile general core telemetry aggregates counts via optimized sub-queries
                    string countsSql = @"
                        SELECT 
                            (SELECT COUNT(OrderID) FROM [dbo].[Orders] WHERE Status = 'Awaiting Verification') AS AwaitingVerify,
                            (SELECT COUNT(ProductID) FROM [dbo].[Products]) AS TotalProducts,
                            (SELECT COUNT(UserID) FROM [dbo].[Users]) AS TotalUsers,
                            (SELECT COUNT(contactId) FROM [dbo].[contactUs]) AS TotalSupportTickets";

                    using (SqlCommand cmdCounts = new SqlCommand(countsSql, conn))
                    {
                        using (SqlDataReader reader = cmdCounts.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litAwaitingVerifyCount.Text = reader["AwaitingVerify"].ToString();
                                litTotalProductsCount.Text = reader["TotalProducts"].ToString();
                                litTotalUsersCount.Text = reader["TotalUsers"].ToString();
                                litSupportTicketsCount.Text = reader["TotalSupportTickets"].ToString();
                            }
                        }
                    }

                    // Query B: Calculate Month-to-Date (MTD) approved sales turnover revenue metrics performance
                    string mtdRevenueSql = @"
                        SELECT ISNULL(SUM(TotalAmount), 0) 
                        FROM [dbo].[Orders] 
                        WHERE Status = 'Approved' 
                        AND OrderDate >= DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)";

                    using (SqlCommand cmdMtd = new SqlCommand(mtdRevenueSql, conn))
                    {
                        decimal mtdRev = Convert.ToDecimal(cmdMtd.ExecuteScalar());
                        litMtdRevenue.Text = string.Format("{0:N2}", mtdRev);
                    }

                    // Query C: Hydrate recent active order streaming data repeater lists
                    string recentOrdersSql = @"
                        SELECT TOP 5 OrderID, OrderDate, TotalAmount, Status, ShippingAddress 
                        FROM [dbo].[Orders] 
                        ORDER BY OrderDate DESC";

                    using (SqlCommand cmdRecent = new SqlCommand(recentOrdersSql, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmdRecent);
                        DataTable dtRecent = new DataTable();
                        da.Fill(dtRecent);

                        rptRecentOrders.DataSource = dtRecent;
                        rptRecentOrders.DataBind();
                    }

                    // Query D: Extract critical low stock counts (Items with 2 or fewer units left)
                    string lowStockSql = @"
                        SELECT TOP 6 Title, Scale, BrandName, StockQuantity 
                        FROM [dbo].[Products] 
                        WHERE StockQuantity <= 2 
                        ORDER BY StockQuantity ASC, Title ASC";

                    using (SqlCommand cmdStock = new SqlCommand(lowStockSql, conn))
                    {
                        SqlDataAdapter daStock = new SqlDataAdapter(cmdStock);
                        DataTable dtStock = new DataTable();
                        daStock.Fill(dtStock);

                        if (dtStock.Rows.Count == 0)
                        {
                            pnlHealthyStockPlaceholder.Visible = true;
                            rptLowStockWatch.Visible = false;
                        }
                        else
                        {
                            pnlHealthyStockPlaceholder.Visible = false;
                            rptLowStockWatch.Visible = true;
                            rptLowStockWatch.DataSource = dtStock;
                            rptLowStockWatch.DataBind();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Catch anomalies and bubble up descriptive contextual warning layout metrics
                    lblMessage.Text = "❌ System Operational Telemetry Load Failure: " + ex.Message;
                    lblMessage.Visible = true;
                }
            }
        }

        /// <summary>
        /// Row level layout mapping event binder. Injects dynamic color badging modifiers based on status strings.
        /// </summary>
        protected void rptRecentOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rowView = (DataRowView)e.Item.DataItem;
                string status = rowView["Status"].ToString();

                Label lblBadge = (Label)e.Item.FindControl("lblRowStatusBadge");
                if (lblBadge != null)
                {
                    lblBadge.CssClass = "dash-row-badge " + GetCssBadgeStyleModifier(status);
                }
            }
        }

        /// <summary>
        /// Transforms raw pipeline string states into clear, decoupled theme-compliant CSS rule overrides classes.
        /// </summary>
        private string GetCssBadgeStyleModifier(string status)
        {
            switch (status.ToLower())
            {
                case "awaiting payment":
                    return "badge-mod-yellow";
                case "awaiting verification":
                    return "badge-mod-blue";
                case "approved":
                    return "badge-mod-green";
                case "cancelled":
                    return "badge-mod-gray";
                default:
                    return "badge-mod-gray";
            }
        }
    }
}