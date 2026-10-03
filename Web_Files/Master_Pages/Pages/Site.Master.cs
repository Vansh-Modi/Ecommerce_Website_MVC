using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Master_Pages.Pages
{
    public partial class Site : MasterPage
    {
        private string ConnectionString => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGlobalDynamicBrandingParameters();
                EvaluateUserRoleNavigationDeck();
            }
        }

        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            string queryStr = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(queryStr))
            {
                Response.Redirect("/Web_Files/Client/Pages/shop.aspx?search=" + HttpUtility.UrlEncode(queryStr));
            }
        }

        private void LoadGlobalDynamicBrandingParameters()
        {
            const string query = "SELECT TOP 1 Logo, LogoTitle, Copyright, Email, PhoneNumber FROM [dbo].[SiteSettings] WHERE [SiteSettingId] = 1";

            var conn = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read()) return;

                object dbLogo = reader["Logo"];
                object dbLogoTitle = reader["LogoTitle"];

                ibLogo.ImageUrl = (dbLogo == DBNull.Value || string.IsNullOrEmpty(dbLogo.ToString().Trim()))
                    ? "~/Web_Files/Images/icons/Show's Garage Logo BG.png"
                    : dbLogo.ToString().Trim();

                string companyTitle = (dbLogoTitle == DBNull.Value || string.IsNullOrEmpty(dbLogoTitle.ToString().Trim()))
                    ? "Show's Garage"
                    : dbLogoTitle.ToString().Trim();

                ibLogo.AlternateText = companyTitle;
                litFooterBrandTitle.Text = companyTitle.ToUpper();

                object dbCopyright = reader["Copyright"];
                if (dbCopyright != DBNull.Value && !string.IsNullOrEmpty(dbCopyright.ToString()))
                {
                    string copyrightText = dbCopyright.ToString();
                    if (copyrightText.Contains("{year}"))
                    {
                        copyrightText = copyrightText.Replace("{year}", DateTime.Now.Year.ToString());
                    }
                    litCopyrightDisplay.Text = copyrightText;
                }
                else
                {
                    litCopyrightDisplay.Text = $"&copy; {DateTime.Now.Year} Shows Garage";
                }

                object dbEmail = reader["Email"];
                if (dbEmail != DBNull.Value && !string.IsNullOrEmpty(dbEmail.ToString().Trim()))
                {
                    lnkFooterEmail.HRef = "mailto:" + dbEmail.ToString().Trim();
                    lnkFooterEmail.InnerText = dbEmail.ToString().Trim();
                    lnkFooterEmail.Visible = true;
                }
                else
                {
                    lnkFooterEmail.Visible = false;
                }

                object dbPhone = reader["PhoneNumber"];
                if (dbPhone != DBNull.Value && !string.IsNullOrEmpty(dbPhone.ToString().Trim()))
                {
                    lnkFooterPhone.HRef = "tel:" + dbPhone.ToString().Trim();
                    lnkFooterPhone.InnerText = "Call: " + dbPhone.ToString().Trim();
                    lnkFooterPhone.Visible = true;
                }
                else
                {
                    lnkFooterPhone.Visible = false;
                }
            }
            catch
            {
                ibLogo.ImageUrl = "~/Web_Files/Images/icons/Show's Garage Logo BG.png";
                litFooterBrandTitle.Text = "SHOWS GARAGE";
                litCopyrightDisplay.Text = $"&copy; {DateTime.Now.Year} Shows Garage";
                lnkFooterEmail.Visible = false;
                lnkFooterPhone.Visible = false;
            }
        }

        private void EvaluateUserRoleNavigationDeck()
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
            {
                pnlAdminNav.Visible = true;
                pnlUserNav.Visible = false;
                Panel2.Visible = true;
                Panel1.Visible = false;
            }
            else
            {
                pnlAdminNav.Visible = false;
                pnlUserNav.Visible = true;
                Panel2.Visible = false;
                Panel1.Visible = true;
            }
        }

        protected void ibUserLogout_Click(object sender, ImageClickEventArgs e) => ExecuteGlobalLogoutSequence();

        protected void ibAdminLogout_Click(object sender, ImageClickEventArgs e) => ExecuteGlobalLogoutSequence();

        private void ExecuteGlobalLogoutSequence()
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/homePage.aspx");
        }

        protected void btnFooterJoin_Click(object sender, EventArgs e) => txtFooterEmail.Text = string.Empty;

        protected void ibLogo_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
            {
                Response.Redirect("~/Web_Files/Admin/Pages/dashboard.aspx");
            }
            else
            {
                Response.Redirect("~/homePage.aspx");
            }
        }
    }
}


