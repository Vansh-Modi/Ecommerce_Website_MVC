using System;
using System.IO;
using System.Data.SqlClient;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_settings : System.Web.UI.Page
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
            // Security Identity Authorization Check
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                EnsureBaselineSettingsRecordExists();
                LoadCurrentStoreConfigurations();
            }
        }

        private void EnsureBaselineSettingsRecordExists()
        {
            // Verifies that a row with SiteSettingId = 1 exists so that the site doesn't throw null errors
            string countCheck = "SELECT COUNT(*) FROM [dbo].[SiteSettings] WHERE [SiteSettingId] = 1";

            // Baseline seed handles setting initial defaults including the PlatformFee column
            string insertSeed = "INSERT INTO [dbo].[SiteSettings] (LogoTitle, ShippingCharges, PlatformFees, UpiID, BankAccountDetails) VALUES ('Show''s Garage', 0.00, 0.00, 'garage@upi', 'Enter Bank Wire Layout Details');";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmdCheck = new SqlCommand(countCheck, conn))
                {
                    conn.Open();
                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    if (count == 0)
                    {
                        using (SqlCommand cmdSeed = new SqlCommand(insertSeed, conn))
                        {
                            cmdSeed.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void LoadCurrentStoreConfigurations()
        {
            string query = "SELECT TOP 1 * FROM [dbo].[SiteSettings] WHERE [SiteSettingId] = 1";

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
                                txtLogoTitle.Text = reader["LogoTitle"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                txtPhone.Text = reader["PhoneNumber"].ToString();
                                txtCopyright.Text = reader["Copyright"].ToString();
                                txtHeroTitle.Text = reader["HeroTitle"].ToString();
                                txtHeroSubtitle.Text = reader["HeroSubtitle"].ToString();
                                txtCurrentHeroPath.Text = reader["HeroImg"].ToString();
                                txtUpiID.Text = reader["UpiID"].ToString();
                                txtBankDetails.Text = reader["BankAccountDetails"].ToString();
                                txtShippingCharges.Text = string.Format("{0:F2}", reader["ShippingCharges"]);

                                // Fetch and present the customized Platform Fee field
                                txtPlatformFee.Text = string.Format("{0:F2}", reader["PlatformFees"]);

                                string qrPath = reader["QrCodePath"].ToString();
                                if (!string.IsNullOrEmpty(qrPath))
                                {
                                    imgQrPreview.ImageUrl = qrPath;
                                    divQrContainer.Visible = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("❌ Configuration Data Fetch Failure: " + ex.Message, false);
                    }
                }
            }
        }

        protected void btnSaveSettings_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;

            try
            {
                decimal shippingCharges = 0;
                if (!string.IsNullOrEmpty(txtShippingCharges.Text.Trim()))
                {
                    decimal.TryParse(txtShippingCharges.Text.Trim(), out shippingCharges);
                }

                decimal platformFee = 15.00m;
                if (!string.IsNullOrEmpty(txtPlatformFee.Text.Trim()))
                {
                    decimal.TryParse(txtPlatformFee.Text.Trim(), out platformFee);
                }

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    // 1. Fetch current paths from DB to fall back on if no new files are uploaded
                    string currentLogoPath = "";
                    string currentHeroPath = txtCurrentHeroPath.Text.Trim();
                    string currentQrPath = imgQrPreview.ImageUrl;

                    string fetchPathsSql = "SELECT TOP 1 Logo, HeroImg, QrCodePath FROM [dbo].[SiteSettings] WHERE [SiteSettingId] = 1";
                    using (SqlCommand cmdFetch = new SqlCommand(fetchPathsSql, conn))
                    {
                        using (SqlDataReader reader = cmdFetch.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentLogoPath = reader["Logo"].ToString();
                                currentHeroPath = string.IsNullOrEmpty(currentHeroPath) ? reader["HeroImg"].ToString() : currentHeroPath;
                                currentQrPath = string.IsNullOrEmpty(currentQrPath) ? reader["QrCodePath"].ToString() : currentQrPath;
                            }
                        }
                    }

                    // 2. Process Logo Image Asset Upload
                    if (fileLogo.HasFile)
                    {
                        string ext = Path.GetExtension(fileLogo.FileName).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".svg")
                        {
                            string dir = Server.MapPath("~/Assets/images/branding/");
                            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                            string name = "StoreLogo" + ext;
                            fileLogo.SaveAs(Path.Combine(dir, name));
                            currentLogoPath = "/Assets/images/branding/" + name;
                        }
                    }

                    // 3. Process Hero Banner Asset Upload
                    if (fileHeroImg.HasFile)
                    {
                        string ext = Path.GetExtension(fileHeroImg.FileName).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                        {
                            string dir = Server.MapPath("~/Assets/images/branding/");
                            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                            string name = "HeroBanner" + ext;
                            fileHeroImg.SaveAs(Path.Combine(dir, name));
                            currentHeroPath = "/Assets/images/branding/" + name;
                        }
                    }

                    // 4. Process QR Code Asset Upload
                    if (fileQrCode.HasFile)
                    {
                        string ext = Path.GetExtension(fileQrCode.FileName).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                        {
                            string dir = Server.MapPath("~/Assets/images/branding/");
                            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                            string name = "ActiveStoreQR" + ext;
                            fileQrCode.SaveAs(Path.Combine(dir, name));
                            currentQrPath = "/Assets/images/branding/" + name;
                        }
                    }

                    // 5. Run the complete update statement including PlatformFee
                    string updateSql = @"
                        UPDATE [dbo].[SiteSettings]
                        SET Logo = @Logo,
                            LogoTitle = @LogoTitle,
                            Email = @Email,
                            PhoneNumber = @Phone,
                            Copyright = @Copyright,
                            HeroTitle = @HeroTitle,
                            HeroSubtitle = @HeroSubtitle,
                            HeroImg = @HeroImg,
                            UpiID = @UpiID,
                            BankAccountDetails = @BankDetails,
                            ShippingCharges = @ShippingCharges,
                            PlatformFees = @PlatformFee,
                            QrCodePath = @QrCodePath
                        WHERE [SiteSettingId] = 1";

                    using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Logo", currentLogoPath);
                        cmd.Parameters.AddWithValue("@LogoTitle", txtLogoTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@Copyright", txtCopyright.Text.Trim());
                        cmd.Parameters.AddWithValue("@HeroTitle", txtHeroTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@HeroSubtitle", txtHeroSubtitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@HeroImg", currentHeroPath);
                        cmd.Parameters.AddWithValue("@UpiID", txtUpiID.Text.Trim());
                        cmd.Parameters.AddWithValue("@BankDetails", txtBankDetails.Text.Trim());
                        cmd.Parameters.AddWithValue("@ShippingCharges", shippingCharges);
                        cmd.Parameters.AddWithValue("@PlatformFee", platformFee);
                        cmd.Parameters.AddWithValue("@QrCodePath", currentQrPath);

                        cmd.ExecuteNonQuery();
                    }
                }

                DisplayAlert("✓ System parameters, metrics, branding logs, and settings records updated successfully.", true);
                LoadCurrentStoreConfigurations(); // Re-index updated database attributes
            }
            catch (Exception ex)
            {
                DisplayAlert("❌ Configuration Save Failure: " + ex.Message, false);
            }
        }

        private void DisplayAlert(string text, bool isSuccess)
        {
            lblMessage.Text = text;
            if (isSuccess)
            {
                lblMessage.CssClass = "admin-alert-banner alert-success";
            }
            else
            {
                lblMessage.CssClass = "admin-alert-banner alert-error";
            }
            lblMessage.Visible = true;
        }
    }
}