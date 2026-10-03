using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Others
{
    public partial class contactUs : System.Web.UI.Page
    {
        private string connStr => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDynamicContactSettingsInformation();
            }
        }

        private void LoadDynamicContactSettingsInformation()
        {
            string query = "SELECT TOP 1 Email, PhoneNumber FROM [dbo].[SiteSettings] WHERE SiteSettingId = 1";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 1. Safe Mail Extraction & Navigation Setup
                                object emailVal = reader["Email"];
                                if (emailVal != DBNull.Value && !string.IsNullOrEmpty(emailVal.ToString().Trim()))
                                {
                                    string emailStr = emailVal.ToString().Trim();
                                    hlEmail.Text = emailStr;
                                    hlEmail.NavigateUrl = "mailto:" + emailStr;
                                }
                                else
                                {
                                    hlEmail.Text = "vanshmodi268@gmail.com";
                                    hlEmail.NavigateUrl = "mailto:vanshmodi268@gmail.com";
                                }

                                // 2. Safe Telephone Extraction & Navigation Setup
                                object phoneVal = reader["PhoneNumber"];
                                if (phoneVal != DBNull.Value && !string.IsNullOrEmpty(phoneVal.ToString().Trim()))
                                {
                                    string phoneStr = phoneVal.ToString().Trim();
                                    hlPhone.Text = "+91 " + phoneStr;
                                    hlPhone.NavigateUrl = "tel:" + phoneStr;
                                }
                                else
                                {
                                    hlPhone.Text = "+91 99986 77425";
                                    hlPhone.NavigateUrl = "tel:9998677425";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Safe layout backup parameters fallback protection
                        lblError.Text = "Settings initialization fallback details: " + ex.Message;
                        hlEmail.Text = "vanshmodi268@gmail.com";
                        hlEmail.NavigateUrl = "mailto:vanshmodi268@gmail.com";
                        hlPhone.Text = "+91 99986 77425";
                        hlPhone.NavigateUrl = "tel:9998677425";
                    }
                }
            }
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            lblStatusAlert.Visible = false;

            string name = txtContactName.Text.Trim();
            string email = txtContactEmail.Text.Trim();
            string message = txtMessage.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                lblStatusAlert.Text = "⚠️ All fields are required.";
                lblStatusAlert.Style["color"] = "#ff3333";
                lblStatusAlert.Visible = true;
                return;
            }

            string query = "INSERT INTO [dbo].[contactUs] (name, email, message) VALUES (@name, @email, @message)";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@message", message);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        lblStatusAlert.Text = "✓ Message sent successfully! We will get back to you shortly.";
                        lblStatusAlert.Style["color"] = "#2ebd59";
                        lblStatusAlert.Visible = true;

                        txtContactName.Text = string.Empty;
                        txtContactEmail.Text = string.Empty;
                        txtMessage.Text = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = "Database Error: " + ex.Message;
                    }
                }
            }
        }
    }
}