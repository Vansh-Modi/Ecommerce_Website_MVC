using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class cart : Page
    {
        private string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Synced Security Gate Check explicitly matching universal framework guidelines
            if (Session["UserRole"] == null || Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!IsPostBack)
            {
                LoadCartFromDatabase();
                CalculateAndBindCart();
            }
        }

        private void LoadCartFromDatabase()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            var dtCart = new DataTable();

            const string query = @"
                SELECT c.ProductID, p.BrandName, p.Title, p.SellingPrice, p.MRP, c.Quantity, p.ImagePath, p.StockQuantity
                FROM Cart c 
                INNER JOIN Products p ON c.ProductID = p.ProductID 
                WHERE c.UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtCart);
                    }
                }
            }

            bool initialStockAdjusted = false;
            foreach (DataRow row in dtCart.Rows)
            {
                int currentQty = Convert.ToInt32(row["Quantity"]);
                int maxAvailable = Convert.ToInt32(row["StockQuantity"]);

                if (currentQty > maxAvailable)
                {
                    initialStockAdjusted = true;
                    if (maxAvailable > 0)
                    {
                        row["Quantity"] = maxAvailable;
                        UpdateQuantityInDatabase(userId, Convert.ToInt32(row["ProductID"]), maxAvailable);
                    }
                    else
                    {
                        RemoveItemFromDatabase(userId, Convert.ToInt32(row["ProductID"]));
                        row.Delete();
                    }
                }
            }

            if (initialStockAdjusted)
            {
                dtCart.AcceptChanges();
            }

            Session["Cart"] = dtCart;
        }

        private void CalculateAndBindCart()
        {
            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart == null || dtCart.Rows.Count == 0)
            {
                lblEmptyMessage.Visible = true;
                divSummary.Visible = false;
                rptCartItems.DataSource = null;
                rptCartItems.DataBind();
                return;
            }

            lblEmptyMessage.Visible = false;
            divSummary.Visible = true;

            decimal cartTotal = 0;
            int totalItemsCount = 0;

            foreach (DataRow row in dtCart.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                decimal price = Convert.ToDecimal(row["SellingPrice"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                cartTotal += (price * qty);
                totalItemsCount += qty;
            }

            rptCartItems.DataSource = dtCart;
            rptCartItems.DataBind();

            lblTotalItems.Text = totalItemsCount.ToString();
            lblCartTotal.Text = string.Format("{0:N0}", cartTotal);
        }

        protected void rptCartItems_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart == null) return;
            int userId = Convert.ToInt32(Session["UserID"]);

            if (e.CommandName == "UpdateQty")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int targetProductId = Convert.ToInt32(args[0]);
                string operationalAction = args[1];

                foreach (DataRow row in dtCart.Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == targetProductId)
                    {
                        int currentQty = Convert.ToInt32(row["Quantity"]);
                        int newQty = currentQty;

                        if (operationalAction == "plus")
                        {
                            int availableStock = GetAvailableStock(targetProductId);
                            if (currentQty >= availableStock)
                            {
                                string msg = $"Cannot add more items. Only {availableStock} unit(s) available in stock.";
                                ScriptManager.RegisterStartupScript(this, GetType(), "StockAlert", $"alert('{msg}');", true);
                                return;
                            }
                            newQty = currentQty + 1;
                        }
                        else if (operationalAction == "minus" && currentQty > 1)
                        {
                            newQty = currentQty - 1;
                        }

                        if (newQty != currentQty)
                        {
                            UpdateQuantityInDatabase(userId, targetProductId, newQty);
                            row["Quantity"] = newQty;
                        }
                        break;
                    }
                }
            }
            else if (e.CommandName == "RemoveItem")
            {
                int targetProductId = Convert.ToInt32(e.CommandArgument);
                RemoveItemFromDatabase(userId, targetProductId);

                for (int i = dtCart.Rows.Count - 1; i >= 0; i--)
                {
                    if (Convert.ToInt32(dtCart.Rows[i]["ProductID"]) == targetProductId)
                    {
                        dtCart.Rows[i].Delete();
                        break;
                    }
                }
                dtCart.AcceptChanges();
            }

            Session["Cart"] = dtCart;
            CalculateAndBindCart();
        }

        private int GetAvailableStock(int productId)
        {
            const string query = "SELECT StockQuantity FROM Products WHERE ProductID = @ProductID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private void UpdateQuantityInDatabase(int userId, int productId, int newQuantity)
        {
            const string query = "UPDATE Cart SET Quantity = @Quantity WHERE UserID = @UserID AND ProductID = @ProductID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Quantity", newQuantity);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void RemoveItemFromDatabase(int userId, int productId)
        {
            const string query = "DELETE FROM Cart WHERE UserID = @UserID AND ProductID = @ProductID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart == null || dtCart.Rows.Count == 0) return;

            int userId = Convert.ToInt32(Session["UserID"]);
            bool stockIssueFound = false;
            string alertMessage = "Some items in your cart are no longer available in the requested quantity:\\n";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                foreach (DataRow row in dtCart.Rows)
                {
                    if (row.RowState == DataRowState.Deleted) continue;

                    int productId = Convert.ToInt32(row["ProductID"]);
                    int requestedQty = Convert.ToInt32(row["Quantity"]);
                    string productTitle = row["Title"].ToString();

                    const string query = "SELECT StockQuantity FROM Products WHERE ProductID = @ProductID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        object result = cmd.ExecuteScalar();
                        int currentStock = result != null ? Convert.ToInt32(result) : 0;

                        if (requestedQty > currentStock)
                        {
                            stockIssueFound = true;
                            alertMessage += $"- {productTitle} (Available: {currentStock}, in your cart: {requestedQty})\\n";

                            if (currentStock > 0)
                            {
                                UpdateQuantityInDatabase(userId, productId, currentStock);
                                row["Quantity"] = currentStock;
                            }
                            else
                            {
                                RemoveItemFromDatabase(userId, productId);
                                row.Delete();
                            }
                        }
                    }
                }
            }

            if (stockIssueFound)
            {
                dtCart.AcceptChanges();
                Session["Cart"] = dtCart;
                CalculateAndBindCart();
                ScriptManager.RegisterStartupScript(this, GetType(), "CheckoutStockError", $"alert('{alertMessage}');", true);
            }
            else
            {
                Response.Redirect("checkout.aspx");
            }
        }
    }
}