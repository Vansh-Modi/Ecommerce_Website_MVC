using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class checkout : Page
    {
        private decimal dynamicShippingFee;

        private string ConnectionString => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserID"] == null || Session["UserEmail"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            LoadShippingFeesFromSettings();

            if (!IsPostBack)
            {
                DataTable dtCart = Session["Cart"] as DataTable;
                if (dtCart == null || dtCart.Rows.Count == 0)
                {
                    Response.Redirect("cart.aspx");
                    return;
                }

                BindCheckoutReview(dtCart);
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

        private void BindCheckoutReview(DataTable dtCart)
        {
            decimal itemsSubtotal = 0;
            int itemQuantityCounter = 0;

            foreach (DataRow row in dtCart.Rows)
            {
                itemsSubtotal += (Convert.ToDecimal(row["SellingPrice"]) * Convert.ToInt32(row["Quantity"]));
                itemQuantityCounter += Convert.ToInt32(row["Quantity"]);
            }

            decimal appliedShippingFee = (ddlPaymentMode.SelectedValue == "COD") ? 0.00m : dynamicShippingFee;
            decimal grandTotal = itemsSubtotal + appliedShippingFee;

            Repeater1.DataSource = dtCart;
            Repeater1.DataBind();

            Label1.Text = itemQuantityCounter.ToString();
            Label2.Text = string.Format("{0:N0}", itemsSubtotal);
            Label3.Text = string.Format("{0:N0}", appliedShippingFee);
            Label4.Text = string.Format("{0:N0}", grandTotal);
        }

        protected void txtCity_TextChanged(object sender, EventArgs e)
        {
            string cityInput = txtCity.Text.Trim().ToLower();
            lblPaymentWarning.Visible = false;

            if (cityInput != "surat" && !string.IsNullOrEmpty(cityInput))
            {
                ddlPaymentMode.SelectedValue = "ONLINE";
                ListItem codItem = ddlPaymentMode.Items.FindByValue("COD");
                if (codItem != null)
                {
                    ddlPaymentMode.Items.Remove(codItem);
                }

                lblPaymentWarning.Text = "⚠️ Cash-on-Delivery is only available within Surat city limits.";
                lblPaymentWarning.Visible = true;
            }
            else
            {
                if (ddlPaymentMode.Items.FindByValue("COD") == null)
                {
                    ddlPaymentMode.Items.Add(new ListItem("Cash-on-Delivery", "COD"));
                }
            }

            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart != null)
            {
                BindCheckoutReview(dtCart);
            }
            updMainCheckoutLayout.Update();
        }

        protected void ddlPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart != null)
            {
                BindCheckoutReview(dtCart);
            }
            updMainCheckoutLayout.Update();
        }

        protected void btnPageNavigation_Click(object sender, EventArgs e)
        {
            lblStatusMessage.Visible = false;

            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string city = txtCity.Text.Trim();
            string pincode = txtPincode.Text.Trim();
            string selectedPaymentMode = ddlPaymentMode.SelectedValue;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(city) ||
                string.IsNullOrWhiteSpace(pincode))
            {
                lblStatusMessage.Text = "⚠️ Please fill in all required delivery information fields (Name, Phone, Address, City, and Pincode) before submitting.";
                lblStatusMessage.Visible = true;
                return;
            }

            if (city.ToLower() != "surat" && selectedPaymentMode == "COD")
            {
                lblStatusMessage.Text = "❌ Validation Breach: COD services are strictly closed outside Surat city limits.";
                lblStatusMessage.Visible = true;
                return;
            }

            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart == null || dtCart.Rows.Count == 0)
            {
                Response.Redirect("cart.aspx");
                return;
            }

            Session["Checkout_FullName"] = fullName;
            Session["Checkout_Phone"] = phone;
            Session["Checkout_Address"] = address;
            Session["Checkout_City"] = city;
            Session["Checkout_Pincode"] = pincode;
            Session["Checkout_PaymentMethod"] = selectedPaymentMode;

            Response.Redirect("payment.aspx");
        }
    }
}