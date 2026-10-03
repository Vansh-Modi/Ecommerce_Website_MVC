using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Master_Pages.Pages
{
    public partial class login : Page
    {
        private string connStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] != null)
            {
                Response.Redirect("/homePage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblError.Text = string.Empty;
                lblForgotError.Text = string.Empty;
                lblResetError.Text = string.Empty;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                const string query = "SELECT UserID, Role, FullName, Email FROM Users WHERE Email=@email AND PasswordHash=@pass";

                var con = new SqlConnection(connStr);
                var cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@pass", txtPassword.Text.Trim());

                con.Open();
                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    Session["UserID"] = dr["UserID"].ToString();
                    Session["UserRole"] = dr["Role"].ToString();
                    Session["UserName"] = dr["FullName"].ToString();
                    Session["UserEmail"] = dr["Email"].ToString();
                    string userRole = dr["Role"].ToString();

                    if (userRole == "Admin")
                    {
                        Response.Redirect("/Web_Files/Admin/Pages/dashboard.aspx", endResponse: false);
                    }
                    else
                    {
                        if (Request.QueryString["ReturnUrl"] != null)
                        {
                            string targetPage = HttpUtility.UrlDecode(Request.QueryString["ReturnUrl"]);
                            Response.Redirect(targetPage, endResponse: false);
                        }
                        else
                        {
                            Response.Redirect("/homePage.aspx", endResponse: false);
                        }
                    }
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    lblError.Text = "Invalid email or password. Please try again.";
                    lblError.ForeColor = Color.FromArgb(229, 9, 20); // Accurate error red tint mapping
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"System Error: {ex.Message}";
                lblError.ForeColor = Color.OrangeRed;
            }
        }

        protected void lnkGoToForgot_Click(object sender, EventArgs e)
        {
            lblForgotError.Text = string.Empty;
            txtForgotEmail.Text = string.Empty;
            mvAuth.SetActiveView(vForgotPassword);
        }

        protected void lnkBackToLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            mvAuth.SetActiveView(vLogin);
        }

        protected void btnSendRecovery_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string emailInput = txtForgotEmail.Text.Trim();
            try
            {
                const string checkQuery = "SELECT COUNT(1) FROM Users WHERE Email = @email";

                var con = new SqlConnection(connStr);
                var cmd = new SqlCommand(checkQuery, con);

                cmd.Parameters.AddWithValue("@email", emailInput);
                con.Open();
                int userExists = Convert.ToInt32(cmd.ExecuteScalar());

                if (userExists > 0)
                {
                    string recoveryOTP = new Random().Next(100000, 999999).ToString();
                    Session["RecoveryOTP"] = recoveryOTP;
                    Session["RecoveryTargetEmail"] = emailInput;

                    using (var mail = new MailMessage())
                    {
                        mail.To.Add(emailInput);
                        mail.From = new MailAddress("showsgarage@gmail.com", "Show's Garage");
                        mail.Subject = "Security Recovery Account Code - Show's Garage";
                        mail.Body = $@"
                            <div style='background-color: #141414; padding: 30px; font-family: Arial, sans-serif; text-align: center; color: #ffffff; border-radius: 8px;'>
                                <h2 style='color: #e50914; margin-bottom: 20px;'>Show's Garage</h2>
                                <p style='font-size: 16px; color: #ccc;'>Use the verification code below to reset your password:</p>
                                <div style='background-color: #222; display: inline-block; padding: 15px 35px; font-size: 28px; font-weight: bold; letter-spacing: 5px; color: #fff; margin: 20px 0; border: 1px solid #444; border-radius: 4px;'>{recoveryOTP}</div>
                            </div>";
                        mail.IsBodyHtml = true;

                        var smtp = new SmtpClient("smtp.gmail.com", 587);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("showsgarage@gmail.com", "gikedyayowpsbhjq");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }

                    lblResetError.Text = "Security OTP dispatched cleanly to your email address.";
                    lblResetError.ForeColor = Color.LightGreen;
                    txtForgotOTP.Text = string.Empty;
                    txtNewPass.Text = string.Empty;
                    mvAuth.SetActiveView(vResetPassword);
                }
                else
                {
                    lblForgotError.Text = "This email address is not registered.";
                    lblForgotError.ForeColor = Color.FromArgb(229, 9, 20);
                }
            }
            catch (Exception ex)
            {
                lblForgotError.Text = $"SMTP Transport Error: {ex.Message}";
                lblForgotError.ForeColor = Color.OrangeRed;
            }
        }

        protected void btnResetSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (Session["RecoveryOTP"] != null && txtForgotOTP.Text.Trim() == Session["RecoveryOTP"].ToString())
            {
                string targetEmail = Session["RecoveryTargetEmail"].ToString();
                string newPassword = txtNewPass.Text.Trim();
                try
                {
                    var con = new SqlConnection(connStr);
                    con.Open();

                    const string checkOldPassQuery = "SELECT PasswordHash FROM Users WHERE Email = @email";
                    string currentPasswordInDb = string.Empty;

                    using (var checkCmd = new SqlCommand(checkOldPassQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@email", targetEmail);
                        var result = checkCmd.ExecuteScalar();
                        if (result != null)
                        {
                            currentPasswordInDb = result.ToString();
                        }
                    }

                    if (newPassword == currentPasswordInDb)
                    {
                        lblResetError.Text = "You cannot reuse your current password. Please choose a new one.";
                        lblResetError.ForeColor = Color.FromArgb(229, 9, 20);
                        return;
                    }

                    const string updateQuery = "UPDATE Users SET PasswordHash = @newPass WHERE Email = @email";
                    using (var updateCmd = new SqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@newPass", newPassword);
                        updateCmd.Parameters.AddWithValue("@email", targetEmail);
                        updateCmd.ExecuteNonQuery();
                    }

                    Session["RecoveryOTP"] = null;
                    Session["RecoveryTargetEmail"] = null;
                    lblError.Text = "Password updated! You can now log in.";
                    lblError.ForeColor = Color.LightGreen;
                    mvAuth.SetActiveView(vLogin);
                    return;
                }
                catch (Exception ex)
                {
                    lblResetError.Text = $"Database Save Failure: {ex.Message}";
                    lblResetError.ForeColor = Color.OrangeRed;
                    return;
                }
            }
            lblResetError.Text = "The verification OTP code is incorrect.";
            lblResetError.ForeColor = Color.FromArgb(229, 9, 20);
        }
    }
}