using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class my_orders : Page
    {
        private string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Secure boundary identity gate check matching universal global requirements
            if (Session["UserRole"] == null || Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserOrderHistory();
            }
        }

        private void LoadUserOrderHistory()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            const string query = @"
                SELECT OrderID, OrderDate, TotalAmount, Status, ShippingAddress, 
                       ISNULL(PaymentScreenshotPath, '') as PaymentScreenShot, 
                       ISNULL(TransactionReference, '') as TransactionReference 
                FROM [dbo].[Orders] 
                WHERE UserID = @UserID 
                ORDER BY OrderDate DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dtOrders = new DataTable();
                        try
                        {
                            conn.Open();
                            da.Fill(dtOrders);

                            if (dtOrders.Rows.Count == 0)
                            {
                                pnlNoOrders.Visible = true;
                            }
                            else
                            {
                                rptOrders.DataSource = dtOrders;
                                rptOrders.DataBind();
                            }
                        }
                        catch (Exception ex)
                        {
                            lblStatusMessage.Text = "❌ Failed to load order records: " + ex.Message;
                            lblStatusMessage.Visible = true;
                        }
                    }
                }
            }
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rowView = (DataRowView)e.Item.DataItem;
                int orderId = Convert.ToInt32(rowView["OrderID"]);
                string status = rowView["Status"].ToString();

                // 1. Color-code the main status badge dynamically via standard styles
                Label lblStatusBadge = (Label)e.Item.FindControl("lblStatusBadge");
                if (lblStatusBadge != null)
                {
                    if (status.Equals("Awaiting Payment", StringComparison.OrdinalIgnoreCase))
                        lblStatusBadge.CssClass = "badge-status status-awaiting-pay";
                    else if (status.Equals("Awaiting Verification", StringComparison.OrdinalIgnoreCase))
                        lblStatusBadge.CssClass = "badge-status status-awaiting-verify";
                    else if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase) || status.Equals("Payment Verified", StringComparison.OrdinalIgnoreCase))
                        lblStatusBadge.CssClass = "badge-status status-approved";
                    else if (status.Equals("Shipped", StringComparison.OrdinalIgnoreCase))
                        lblStatusBadge.CssClass = "badge-status status-shipped";
                    else if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                        lblStatusBadge.CssClass = "badge-status status-approved";
                    else
                        lblStatusBadge.CssClass = "badge-status status-default";
                }

                // 2. Compute timeline process node classes based on verified statuses
                HtmlGenericControl stepPlaced = (HtmlGenericControl)e.Item.FindControl("stepPlaced");
                HtmlGenericControl stepVerified = (HtmlGenericControl)e.Item.FindControl("stepVerified");
                HtmlGenericControl stepShipped = (HtmlGenericControl)e.Item.FindControl("stepShipped");

                if (stepPlaced != null && stepVerified != null && stepShipped != null)
                {
                    if (status.Equals("Awaiting Payment", StringComparison.OrdinalIgnoreCase) || status.Equals("Awaiting Verification", StringComparison.OrdinalIgnoreCase))
                    {
                        stepPlaced.Attributes["class"] = "timeline-step active-step";
                        stepVerified.Attributes["class"] = "timeline-step";
                        stepShipped.Attributes["class"] = "timeline-step";
                    }
                    else if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase) || status.Equals("Payment Verified", StringComparison.OrdinalIgnoreCase))
                    {
                        stepPlaced.Attributes["class"] = "timeline-step completed";
                        stepVerified.Attributes["class"] = "timeline-step active-step";
                        stepShipped.Attributes["class"] = "timeline-step";
                    }
                    else if (status.Equals("Shipped", StringComparison.OrdinalIgnoreCase))
                    {
                        stepPlaced.Attributes["class"] = "timeline-step completed";
                        stepVerified.Attributes["class"] = "timeline-step completed";
                        stepShipped.Attributes["class"] = "timeline-step active-step";
                    }
                    else if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                    {
                        stepPlaced.Attributes["class"] = "timeline-step completed";
                        stepVerified.Attributes["class"] = "timeline-step completed";
                        stepShipped.Attributes["class"] = "timeline-step completed active-step";
                    }
                }

                // 3. Child iteration retrieval executed via standard nested block declarations
                Repeater rptOrderItems = (Repeater)e.Item.FindControl("rptOrderItems");
                if (rptOrderItems != null)
                {
                    const string itemsQuery = @"
                        SELECT od.Quantity, od.UnitPrice, p.Title, p.Scale, p.BrandName 
                        FROM [dbo].[OrderDetails] od
                        INNER JOIN [dbo].[Products] p ON od.ProductID = p.ProductID
                        WHERE od.OrderID = @OrderID";

                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(itemsQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                DataTable dtItems = new DataTable();
                                try
                                {
                                    da.Fill(dtItems);
                                    rptOrderItems.DataSource = dtItems;
                                    rptOrderItems.DataBind();
                                }
                                catch { /* Graceful fallback catch */ }
                            }
                        }
                    }
                }
            }
        }
    }
}