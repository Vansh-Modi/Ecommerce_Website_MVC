using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class payment_upload : Page
    {
        private decimal dynamicShippingFee = 120.00m;
        private string ConnectionString => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Checkout_PaymentMethod"] == null || Session["Cart"] == null)
            {
                Response.Redirect("cart.aspx");
                return;
            }

            LoadShippingFeesFromSettings();

            if (!IsPostBack)
            {
                string paymentMethod = Session["Checkout_PaymentMethod"].ToString();
                decimal totalDue = CalculateTotalDue(paymentMethod);
                litPaymentDue.Text = $"{totalDue:N2}";

                if (paymentMethod == "COD")
                {
                    pnlOnlinePaymentDetails.Visible = false;
                    pnlOnlineUploadForm.Visible = false;
                    pnlCodConfirmation.Visible = true;
                    btnSubmitProof.Text = "Confirm Cash on Delivery Order ✔";
                }
                else
                {
                    pnlOnlinePaymentDetails.Visible = true;
                    pnlOnlineUploadForm.Visible = true;
                    pnlCodConfirmation.Visible = false;
                    btnSubmitProof.Text = "Submit Reference & Complete Order";
                    LoadMerchantDetailsFromSettings();
                }
            }
        }

        private void LoadShippingFeesFromSettings()
        {
            const string query = "SELECT TOP 1 ISNULL(ShippingCharges, 120.00) FROM [dbo].[SiteSettings]";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        object res = cmd.ExecuteScalar();
                        if (res != null)
                        {
                            dynamicShippingFee = Convert.ToDecimal(res);
                        }
                    }
                    catch
                    {
                        dynamicShippingFee = 120.00m;
                    }
                }
            }
        }

        private decimal CalculateTotalDue(string paymentMethod)
        {
            decimal total = 0.00m;

            if (Session["Cart"] is DataTable dtCart)
            {
                foreach (DataRow row in dtCart.Rows)
                {
                    total += Convert.ToDecimal(row["SellingPrice"]) * Convert.ToInt32(row["Quantity"]);
                }
            }

            if (paymentMethod != "COD")
            {
                total += dynamicShippingFee;
            }

            return total;
        }

        private void LoadMerchantDetailsFromSettings()
        {
            const string query = "SELECT TOP 1 QrCodePath, UpiID, BankAccountDetails FROM [dbo].[SiteSettings]";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string qrPath = reader["QrCodePath"].ToString();
                                string upiId = reader["UpiID"].ToString();
                                string bankDetails = reader["BankAccountDetails"].ToString();

                                imgQrCode.ImageUrl = !string.IsNullOrEmpty(qrPath) ? qrPath : "/Assets/images/default-qr.png";
                                litUpiId.Text = !string.IsNullOrEmpty(upiId) ? upiId : "Not Available";
                                litBankDetails.Text = !string.IsNullOrEmpty(bankDetails) ? bankDetails : "Contact Support for Details";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = $"❌ Configuration Read Failure: {ex.Message}";
                        lblStatus.Visible = true;
                    }
                }
            }
        }

        protected void btnSubmitProof_Click(object sender, EventArgs e)
        {
            lblStatus.Visible = false;
            int userId = Convert.ToInt32(Session["UserID"]);

            if (!(Session["Cart"] is DataTable dtCart) || dtCart.Rows.Count == 0)
            {
                lblStatus.Text = "❌ Process Error: Session cart data has expired.";
                lblStatus.Visible = true;
                return;
            }

            string paymentMethod = Session["Checkout_PaymentMethod"].ToString();
            string fullName = Session["Checkout_FullName"].ToString();
            string phone = Session["Checkout_Phone"].ToString();
            string address = Session["Checkout_Address"].ToString();
            string city = Session["Checkout_City"].ToString();
            string pincode = Session["Checkout_Pincode"].ToString();

            string txnRef = "COD-ORDER";
            string relativeDbStringPath = "COD";
            string targetOrderStatus = "Awaiting Verification";

            if (paymentMethod != "COD")
            {
                txnRef = txtTxnReference.Text.Trim();
                if (string.IsNullOrEmpty(txnRef))
                {
                    lblStatus.Text = "⚠️ Please enter your transaction UTR or reference number.";
                    lblStatus.Visible = true;
                    return;
                }

                if (!fileScreenshot.HasFile)
                {
                    lblStatus.Text = "⚠️ Please upload an image screenshot copy of your payment receipt.";
                    lblStatus.Visible = true;
                    return;
                }

                const int maxAllowedBytes = 2097152; // 2MB Upload Cap Sequence Limit
                if (fileScreenshot.PostedFile.ContentLength > maxAllowedBytes)
                {
                    lblStatus.Text = "⚠️ Screenshot size exceeds the maximum limit of 2MB.";
                    lblStatus.Visible = true;
                    return;
                }

                string mimeType = fileScreenshot.PostedFile.ContentType.ToLower();
                if (mimeType != "image/jpeg" && mimeType != "image/jpg" && mimeType != "image/png")
                {
                    lblStatus.Text = "❌ Invalid file type. Only standard graphics (.jpg, .jpeg, .png) are supported.";
                    lblStatus.Visible = true;
                    return;
                }

                try
                {
                    string extension = Path.GetExtension(fileScreenshot.FileName).ToLower();
                    string folderMapPath = Server.MapPath("~/Assets/uploads/receipts/");

                    if (!Directory.Exists(folderMapPath))
                    {
                        Directory.CreateDirectory(folderMapPath);
                    }

                    string cleanFileName = $"Order_Pending_{userId}_{DateTime.Now.Ticks}{extension}";
                    string fullServerSavePath = Path.Combine(folderMapPath, cleanFileName);

                    fileScreenshot.SaveAs(fullServerSavePath);
                    relativeDbStringPath = $"/Assets/uploads/receipts/{cleanFileName}";
                }
                catch (Exception ex)
                {
                    lblStatus.Text = $"❌ File Save Error: {ex.Message}";
                    lblStatus.Visible = true;
                    return;
                }
            }

            try
            {
                decimal totalOrderAmount = CalculateTotalDue(paymentMethod);
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    try
                    {
                        const string insertOrderSql = @"
                            INSERT INTO [dbo].[Orders] 
                            (UserID, OrderDate, TotalAmount, Status, ShippingAddress, Pincode, PaymentMethod, PaymentScreenshotPath, TransactionReference) 
                            OUTPUT INSERTED.OrderID
                            VALUES 
                            (@UserID, @OrderDate, @TotalAmount, @Status, @ShippingAddress, @Pincode, @PaymentMethod, @ImgPath, @TxnRef)";

                        int generatedOrderId;
                        using (SqlCommand cmdOrder = new SqlCommand(insertOrderSql, conn, trans))
                        {
                            cmdOrder.Parameters.AddWithValue("@UserID", userId);
                            cmdOrder.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                            cmdOrder.Parameters.AddWithValue("@TotalAmount", totalOrderAmount);
                            cmdOrder.Parameters.AddWithValue("@Status", targetOrderStatus);
                            cmdOrder.Parameters.AddWithValue("@ShippingAddress", $"{address}, {city} (Phone: {phone}, Name: {fullName})");
                            cmdOrder.Parameters.AddWithValue("@Pincode", pincode);
                            cmdOrder.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                            cmdOrder.Parameters.AddWithValue("@ImgPath", relativeDbStringPath);
                            cmdOrder.Parameters.AddWithValue("@TxnRef", txnRef);

                            generatedOrderId = Convert.ToInt32(cmdOrder.ExecuteScalar());
                        }

                        const string insertItemsQuery = @"
                            INSERT INTO [dbo].[OrderDetails] (OrderID, ProductID, Quantity, UnitPrice) 
                            VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice)";

                        const string deductStockSql = @"
                            UPDATE [dbo].[Products] 
                            SET StockQuantity = StockQuantity - @Quantity 
                            WHERE ProductID = @ProductID";

                        foreach (DataRow row in dtCart.Rows)
                        {
                            using (SqlCommand cmdItem = new SqlCommand(insertItemsQuery, conn, trans))
                            {
                                cmdItem.Parameters.AddWithValue("@OrderID", generatedOrderId);
                                cmdItem.Parameters.AddWithValue("@ProductID", Convert.ToInt32(row["ProductID"]));
                                cmdItem.Parameters.AddWithValue("@Quantity", Convert.ToInt32(row["Quantity"]));
                                cmdItem.Parameters.AddWithValue("@UnitPrice", Convert.ToDecimal(row["SellingPrice"]));
                                cmdItem.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdStock = new SqlCommand(deductStockSql, conn, trans))
                            {
                                cmdStock.Parameters.AddWithValue("@Quantity", Convert.ToInt32(row["Quantity"]));
                                cmdStock.Parameters.AddWithValue("@ProductID", Convert.ToInt32(row["ProductID"]));
                                cmdStock.ExecuteNonQuery();
                            }
                        }

                        const string deleteCartSql = "DELETE FROM Cart WHERE UserID = @UserID";
                        using (SqlCommand cmdCart = new SqlCommand(deleteCartSql, conn, trans))
                        {
                            cmdCart.Parameters.AddWithValue("@UserID", userId);
                            cmdCart.ExecuteNonQuery();
                        }

                        trans.Commit();

                        // Flush complete transaction checkout environments 
                        Session["Cart"] = null;
                        Session["Checkout_FullName"] = null;
                        Session["Checkout_Phone"] = null;
                        Session["Checkout_Address"] = null;
                        Session["Checkout_City"] = null;
                        Session["Checkout_Pincode"] = null;
                        Session["Checkout_PaymentMethod"] = null;

                        Response.Redirect($"confirmOrder.aspx?id={generatedOrderId}");
                    }
                    catch (Exception ex2)
                    {
                        trans.Rollback();
                        throw new Exception($"Inventory verification rollback. Details: {ex2.Message}", ex2);
                    }
                }
            }
            catch (Exception ex3)
            {
                lblStatus.Text = $"❌ Process Error: {ex3.Message}";
                lblStatus.Visible = true;
            }
        }
    }
}