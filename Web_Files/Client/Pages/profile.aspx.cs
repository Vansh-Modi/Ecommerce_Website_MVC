using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class profile : Page
    {
        private string connStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Restored the original authorization check and ReturnUrl tracking
            if (Session["UserRole"] == null)
            {
                string currentUrlWithQueryString = Request.Url.PathAndQuery;
                Response.Redirect("/Web_Files/Master_Pages/Pages/login.aspx?ReturnUrl=" + HttpUtility.UrlEncode(currentUrlWithQueryString));
                return;
            }

            if (!IsPostBack)
            {
                LoadUserProfile();
            }
        }

        private void LoadUserProfile()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    const string query = "SELECT FullName, Phone, Email FROM Users WHERE Email = @email";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@email", Session["UserEmail"].ToString());
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fullName = reader["FullName"].ToString();
                                txtName.Text = fullName;
                                txtNumber.Text = reader["Phone"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                litWelcomeName.Text = fullName.ToLower();
                                txtEmail.Enabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading profile data: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(229, 9, 20); // Aligned to crimson validation red
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    const string query = "UPDATE Users SET FullName = @name, Phone = @phone WHERE Email = @email";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", txtNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", Session["UserEmail"].ToString());

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblStatus.Text = "Profile text updates saved successfully!";
                            lblStatus.ForeColor = Color.LightGreen;
                            litWelcomeName.Text = txtName.Text.Trim().ToLower();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Profile Save Error: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(229, 9, 20);
            }
        }

        protected void lnkForgotCurrent_Click(object sender, EventArgs e)
        {
            string emailTarget = Session["UserEmail"].ToString();
            try
            {
                string profileOTP = new Random().Next(100000, 999999).ToString();
                Session["ProfileRecoveryOTP"] = profileOTP;

                using (MailMessage mail = new MailMessage())
                {
                    mail.To.Add(emailTarget);
                    mail.From = new MailAddress("showsgarage@gmail.com", "Show's Garage");
                    mail.Subject = "Profile Security Credentials Token - Show's Garage";
                    mail.Body = $@"
                        <div style='background-color: #141414; padding: 30px; font-family: Arial, sans-serif; text-align: center; color: #ffffff; border-radius: 8px;'>
                            <h2 style='color: #e50914; margin-bottom: 20px;'>Show's Garage</h2>
                            <p style='font-size: 16px; color: #ccc;'>You initiated a credentials recovery bypass from your active profile portal. Use the verification token code below:</p>
                            <div style='background-color: #222; display: inline-block; padding: 15px 35px; font-size: 28px; font-weight: bold; letter-spacing: 5px; color: #fff; margin: 20px 0; border: 1px solid #444; border-radius: 4px;'>{profileOTP}</div>
                        </div>";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("showsgarage@gmail.com", "gikedyayowpsbhjq");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                lblStatus.Text = "A security validation token has been dispatched cleanly to your inbox.";
                lblStatus.ForeColor = Color.LightGreen;
                phStandardReset.Visible = false;
                phOtpVerification.Visible = true;
                rfvCurrentPass.Enabled = false;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "SMTP Connection Error: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(229, 9, 20);
            }
        }

        protected void lnkCancelRecovery_Click(object sender, EventArgs e)
        {
            Session["ProfileRecoveryOTP"] = null;
            phStandardReset.Visible = true;
            phOtpVerification.Visible = false;
            rfvCurrentPass.Enabled = true;
            lblStatus.Text = "Recovery mode aborted.";
            lblStatus.ForeColor = Color.FromArgb(255, 204, 0); // Vivid Yellow warn accent indicator
        }

        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string targetEmail = Session["UserEmail"].ToString();
            string newPassword = txtNewPassword.Text.Trim();

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    const string checkQuery = "SELECT PasswordHash FROM Users WHERE Email = @email";
                    string currentDbPassword = string.Empty;

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@email", targetEmail);
                        object result = checkCmd.ExecuteScalar();
                        if (result != null)
                        {
                            currentDbPassword = result.ToString();
                        }
                    }

                    // Handle verification route mapping conditionally based on active drawer views
                    if (phOtpVerification.Visible)
                    {
                        if (Session["ProfileRecoveryOTP"] == null || txtProfileOTP.Text.Trim() != Session["ProfileRecoveryOTP"].ToString())
                        {
                            lblStatus.Text = "The verification profile OTP code is incorrect.";
                            lblStatus.ForeColor = Color.FromArgb(229, 9, 20);
                            return;
                        }
                    }
                    else if (txtCurrentPassword.Text != currentDbPassword)
                    {
                        lblStatus.Text = "Incorrect current password. Identity authentication check failed.";
                        lblStatus.ForeColor = Color.FromArgb(229, 9, 20);
                        return;
                    }

                    if (newPassword == currentDbPassword)
                    {
                        lblStatus.Text = "You cannot reuse your current password layout. Please select a distinct one.";
                        lblStatus.ForeColor = Color.FromArgb(229, 9, 20);
                        return;
                    }

                    const string updateQuery = "UPDATE Users SET PasswordHash = @newPassword WHERE Email = @email";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@newPassword", newPassword);
                        updateCmd.Parameters.AddWithValue("@email", targetEmail);
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblStatus.Text = "Security credentials updated securely and successfully!";
                            lblStatus.ForeColor = Color.LightGreen;

                            txtCurrentPassword.Text = string.Empty;
                            txtNewPassword.Text = string.Empty;
                            txtConfirmNewPassword.Text = string.Empty;
                            txtProfileOTP.Text = string.Empty;
                            Session["ProfileRecoveryOTP"] = null;

                            phStandardReset.Visible = true;
                            phOtpVerification.Visible = false;
                            rfvCurrentPass.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Credentials Update Failure: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(229, 9, 20);
            }
        }
    }
}