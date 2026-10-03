using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_orders : System.Web.UI.Page
    {
        private string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSystemOrdersDashboard("ALL");
            }
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAdminStatus.Visible = false;
            LoadSystemOrdersDashboard(ddlStatusFilter.SelectedValue);
        }

        private void LoadSystemOrdersDashboard(string filterStatus)
        {
            // Base query fetching all columns
            string query = @"
                SELECT OrderID, UserID, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, 
                       ISNULL(PaymentScreenshotPath, '') as PaymentScreenshotPath, 
                       ISNULL(TransactionReference, '') as TransactionReference,
                       ISNULL(TrackingPartner, '') as TrackingPartner, 
                       ISNULL(TrackingNumber, '') as TrackingNumber, 
                       EstimatedDeliveryDate
                FROM [dbo].[Orders]";

            // Build conditional filtering logic
            if (filterStatus != "ALL")
            {
                // Specifically filters down to the chosen status (e.g., "Cancelled")
                query += " WHERE Status = @FilterStatus ORDER BY OrderDate DESC";
            }
            else
            {
                // 🔥 THE FIX: Exclude Cancelled logs entirely from the default "ALL" view
                query += " WHERE Status <> 'Cancelled' ORDER BY CASE WHEN Status = 'Awaiting Verification' THEN 1 WHEN Status = 'Approved' THEN 2 WHEN Status = 'Shipped' THEN 3 ELSE 4 END, OrderDate DESC";
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (filterStatus != "ALL")
                    {
                        cmd.Parameters.AddWithValue("@FilterStatus", filterStatus);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtOrders = new DataTable();
                    try
                    {
                        conn.Open();
                        da.Fill(dtOrders);
                        rptAdminOrders.DataSource = dtOrders;
                        rptAdminOrders.DataBind();
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusAlert("❌ Failed to bind master order logs tracking registry: " + ex.Message, false);
                    }
                }
            }
        }

        protected void rptAdminOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rowView = (DataRowView)e.Item.DataItem;
                int orderId = Convert.ToInt32(rowView["OrderID"]);
                string status = rowView["Status"].ToString();

                Label lblStatusBadge = (Label)e.Item.FindControl("lblStatusBadge");
                if (lblStatusBadge != null)
                {
                    lblStatusBadge.CssClass = "admin-status-badge " + GetCssBadgeClass(status);
                }

                DropDownList ddlEditStatus = (DropDownList)e.Item.FindControl("ddlEditStatus");
                if (ddlEditStatus != null)
                {
                    ddlEditStatus.SelectedValue = ddlEditStatus.Items.FindByValue(status) != null ? status : "Awaiting Verification";
                }

                Repeater rptAdminOrderItems = (Repeater)e.Item.FindControl("rptAdminOrderItems");
                if (rptAdminOrderItems != null)
                {
                    string subItemsQuery = @"
                        SELECT od.Quantity, od.UnitPrice, p.Title, p.Scale, p.BrandName 
                        FROM [dbo].[OrderDetails] od
                        INNER JOIN [dbo].[Products] p ON od.ProductID = p.ProductID
                        WHERE od.OrderID = @OrderID";

                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(subItemsQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dtItems = new DataTable();
                            da.Fill(dtItems);

                            rptAdminOrderItems.DataSource = dtItems;
                            rptAdminOrderItems.DataBind();
                        }
                    }
                }
            }
        }

        protected void rptAdminOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblAdminStatus.Visible = false;

            if (e.CommandName == "TogglePaymentEdit")
            {
                int itemIndex = Convert.ToInt32(e.CommandArgument);
                RepeaterItem rowItem = rptAdminOrders.Items[itemIndex];
                Panel pnlEdit = (Panel)rowItem.FindControl("pnlInlinePaymentEdit");
                if (pnlEdit != null) { pnlEdit.Visible = !pnlEdit.Visible; }
            }
            else if (e.CommandName == "SaveInlinePayment")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                TextBox txtRef = (TextBox)e.Item.FindControl("txtEditTxnRef");
                DropDownList ddlStat = (DropDownList)e.Item.FindControl("ddlEditStatus");
                TextBox txtPartner = (TextBox)e.Item.FindControl("txtCourierPartner");
                TextBox txtTrackNo = (TextBox)e.Item.FindControl("txtTrackingNumber");
                TextBox txtEstDate = (TextBox)e.Item.FindControl("txtEstDeliveryDate");

                if (txtRef != null && ddlStat != null && txtPartner != null && txtTrackNo != null && txtEstDate != null)
                {
                    ExecuteInlineLogisticsUpdate(orderId, txtRef.Text.Trim(), ddlStat.SelectedValue, txtPartner.Text.Trim(), txtTrackNo.Text.Trim(), txtEstDate.Text.Trim());
                    DisplayStatusAlert($"✓ Order tracking parameters and logs metadata for entry #{orderId} updated successfully inside registers.", true);

                    // If the active status filter is set to "ALL" and this order became "Cancelled", it disappears from "ALL".
                    // If the filter is something specific, let's keep it matching.
                    if (ddlStatusFilter.SelectedValue == "ALL" && ddlStat.SelectedValue == "Cancelled")
                    {
                        LoadSystemOrdersDashboard("ALL");
                    }
                    else
                    {
                        LoadSystemOrdersDashboard(ddlStatusFilter.SelectedValue);
                    }
                }
            }
            else if (e.CommandName == "ApprovePayment")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                ExecuteStatusTransition(orderId, "Approved");
                DisplayStatusAlert($"✓ Order reference line entry #{orderId} has been successfully verified.", true);
                LoadSystemOrdersDashboard(ddlStatusFilter.SelectedValue);
            }
            else if (e.CommandName == "RejectPayment")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                ExecuteOrderRejectionWithStockRollback(orderId);
                DisplayStatusAlert($"⚠️ Order reference line entry #{orderId} payment registration rejected. Stock logs reverted.", true);

                // Automatically switch the dropdown list to "Cancelled Logs" so the admin sees the order they just rejected
                ddlStatusFilter.SelectedValue = "Cancelled";
                LoadSystemOrdersDashboard("Cancelled");
            }
        }

        private void ExecuteInlineLogisticsUpdate(int orderId, string cleanTxnRef, string targetStatus, string partner, string trackNo, string estDateStr)
        {
            string currentStatus = "";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Status FROM [dbo].[Orders] WHERE OrderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                conn.Open();
                currentStatus = cmd.ExecuteScalar()?.ToString();
            }

            if (targetStatus == "Cancelled" && !currentStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                ExecuteOrderRejectionWithStockRollback(orderId);
            }

            string sql = @"
                UPDATE [dbo].[Orders] 
                SET TransactionReference = @TxnRef, 
                    Status = @Status, 
                    TrackingPartner = @TrackingPartner, 
                    TrackingNumber = @TrackingNumber, 
                    EstimatedDeliveryDate = @EstDate 
                WHERE OrderID = @OrderID";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TxnRef", cleanTxnRef);
                    cmd.Parameters.AddWithValue("@Status", targetStatus);
                    cmd.Parameters.AddWithValue("@TrackingPartner", string.IsNullOrEmpty(partner) ? (object)DBNull.Value : partner);
                    cmd.Parameters.AddWithValue("@TrackingNumber", string.IsNullOrEmpty(trackNo) ? (object)DBNull.Value : trackNo);

                    if (string.IsNullOrEmpty(estDateStr))
                        cmd.Parameters.AddWithValue("@EstDate", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@EstDate", Convert.ToDateTime(estDateStr));

                    cmd.Parameters.AddWithValue("@OrderID", orderId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ExecuteStatusTransition(int orderId, string targetStatus)
        {
            string sql = "UPDATE [dbo].[Orders] SET Status = @Status WHERE OrderID = @OrderID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", targetStatus);
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ExecuteOrderRejectionWithStockRollback(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        decimal totalInvoiceAmount = 0;
                        string orderDataSql = "SELECT TotalAmount, Status FROM [dbo].[Orders] WHERE OrderID = @OrderID";
                        using (SqlCommand cmdFetchOrder = new SqlCommand(orderDataSql, conn, trans))
                        {
                            cmdFetchOrder.Parameters.AddWithValue("@OrderID", orderId);
                            using (SqlDataReader orderReader = cmdFetchOrder.ExecuteReader())
                            {
                                if (orderReader.Read())
                                {
                                    if (orderReader["Status"].ToString().Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                                    {
                                        orderReader.Close();
                                        trans.Rollback();
                                        return;
                                    }
                                    totalInvoiceAmount = Convert.ToDecimal(orderReader["TotalAmount"]);
                                }
                                orderReader.Close();
                            }
                        }

                        string cancelOrderSql = "UPDATE [dbo].[Orders] SET Status = 'Cancelled' WHERE OrderID = @OrderID";
                        using (SqlCommand cmdCancel = new SqlCommand(cancelOrderSql, conn, trans))
                        {
                            cmdCancel.Parameters.AddWithValue("@OrderID", orderId);
                            cmdCancel.ExecuteNonQuery();
                        }

                        string getLineItemsSql = "SELECT ProductID, Quantity FROM [dbo].[OrderDetails] WHERE OrderID = @OrderID";
                        DataTable dtLines = new DataTable();
                        using (SqlCommand cmdFetch = new SqlCommand(getLineItemsSql, conn, trans))
                        {
                            cmdFetch.Parameters.AddWithValue("@OrderID", orderId);
                            SqlDataAdapter da = new SqlDataAdapter(cmdFetch);
                            da.Fill(dtLines);
                        }

                        string restoreInventorySql = "UPDATE [dbo].[Products] SET StockQuantity = StockQuantity + @Quantity WHERE ProductID = @ProductID";
                        foreach (DataRow line in dtLines.Rows)
                        {
                            using (SqlCommand cmdRestore = new SqlCommand(restoreInventorySql, conn, trans))
                            {
                                cmdRestore.Parameters.AddWithValue("@Quantity", Convert.ToInt32(line["Quantity"]));
                                cmdRestore.Parameters.AddWithValue("@ProductID", Convert.ToInt32(line["ProductID"]));
                                cmdRestore.ExecuteNonQuery();
                            }
                        }

                        string insertRefundExpenseSql = @"
                            INSERT INTO [dbo].[Expenses] (Title, Amount, ExpenseType, ExpenseDate, OrderID, Remarks)
                            VALUES (@Title, @Amount, @ExpenseType, @ExpenseDate, @OrderID, @Remarks)";

                        using (SqlCommand cmdExpense = new SqlCommand(insertRefundExpenseSql, conn, trans))
                        {
                            cmdExpense.Parameters.AddWithValue("@Title", $"Customer Refund Payout for Cancelled Order #{orderId}");
                            cmdExpense.Parameters.AddWithValue("@Amount", totalInvoiceAmount);
                            cmdExpense.Parameters.AddWithValue("@ExpenseType", "Damaged Return");
                            cmdExpense.Parameters.AddWithValue("@ExpenseDate", DateTime.Now);
                            cmdExpense.Parameters.AddWithValue("@OrderID", orderId);
                            cmdExpense.Parameters.AddWithValue("@Remarks", $"PENDING MANUAL TRANSFER: Order rejected on admin dashboard on {DateTime.Now:dd MMM yyyy}. Remit payment.");

                            cmdExpense.ExecuteNonQuery();
                        }

                        trans.Commit();
                    }
                    catch (Exception innerEx)
                    {
                        trans.Rollback();
                        throw new Exception("Operational transaction rejection sequence processing error context: " + innerEx.Message, innerEx);
                    }
                }
            }
        }

        private string GetCssBadgeClass(string status)
        {
            switch (status.ToLower())
            {
                case "awaiting payment": return "adm-badge-yellow";
                case "awaiting verification": return "adm-badge-blue";
                case "approved": return "adm-badge-green";
                case "dispatched": return "adm-badge-green";
                case "delivered": return "adm-badge-green";
                case "cancelled": return "adm-badge-gray";
                default: return "adm-badge-gray";
            }
        }

        private void DisplayStatusAlert(string message, bool isSuccess)
        {
            lblAdminStatus.Text = message;
            lblAdminStatus.CssClass = isSuccess ? "admin-alert-banner alert-success" : "admin-alert-banner alert-error";
            lblAdminStatus.Visible = true;
        }
    }
}