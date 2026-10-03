using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class order_details : Page
    {
        private string ConnectionString => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Core Security Validation Gate matching system authorization architecture
            if (Session["UserRole"] == null || Session["UserID"] == null)
            {
                string currentUrlWithQueryString = Request.Url.PathAndQuery;
                Response.Redirect("/Web_Files/Master_Pages/Pages/login.aspx?ReturnUrl=" + HttpUtility.UrlEncode(currentUrlWithQueryString));
                return;
            }

            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblErrorMessage.Text = "⚠️ Missing parameters record routing exception. Return to history index panel.";
                    lblErrorMessage.Visible = true;
                    pnlTrackingInfo.Visible = false;
                    return;
                }

                if (int.TryParse(Request.QueryString["id"].Trim(), out int orderId))
                {
                    LoadTrackingMatrixData(orderId);
                }
                else
                {
                    lblErrorMessage.Text = "❌ Invalid routing parameters context specified.";
                    lblErrorMessage.Visible = true;
                    pnlTrackingInfo.Visible = false;
                }
            }
        }

        private void LoadTrackingMatrixData(int orderId)
        {
            int currentUserId = Convert.ToInt32(Session["UserID"]);

            const string query = @"
                SELECT OrderID, Status, ShippingAddress, TrackingPartner, TrackingNumber, EstimatedDeliveryDate 
                FROM [dbo].[Orders] 
                WHERE OrderID = @OrderID AND UserID = @UserID";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.Parameters.AddWithValue("@UserID", currentUserId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pnlTrackingInfo.Visible = true;
                                lblErrorMessage.Visible = false;

                                lblOrderID.Text = reader["OrderID"].ToString();
                                lblStatus.Text = reader["Status"].ToString();
                                lblDeliveryAddress.Text = reader["ShippingAddress"].ToString();

                                string partner = reader["TrackingPartner"].ToString();
                                string trackNo = reader["TrackingNumber"].ToString();
                                object estDate = reader["EstimatedDeliveryDate"];

                                lblCourierName.Text = !string.IsNullOrEmpty(partner) ? partner : "Processing";
                                lblTrackingNo.Text = !string.IsNullOrEmpty(trackNo) ? trackNo : "Not Available Yet";

                                if (estDate != DBNull.Value && estDate != null)
                                {
                                    lblEstDelivery.Text = Convert.ToDateTime(estDate).ToString("dd MMM yyyy (dddd)");
                                }
                                else
                                {
                                    lblEstDelivery.Text = "Calculating schedule...";
                                }
                            }
                            else
                            {
                                lblErrorMessage.Text = "❌ Access Boundary Violation: Record resource not found or missing structural security authorization tokens.";
                                lblErrorMessage.Visible = true;
                                pnlTrackingInfo.Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = $"❌ Processing Framework Failure: {ex.Message}";
                lblErrorMessage.Visible = true;
                pnlTrackingInfo.Visible = false;
            }
        }
    }
}