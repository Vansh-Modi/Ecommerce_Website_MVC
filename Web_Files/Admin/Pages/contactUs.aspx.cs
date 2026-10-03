using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_support : System.Web.UI.Page
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
                LoadActiveSupportTicketsInbox();
            }
        }

        private void LoadActiveSupportTicketsInbox()
        {
            // Pull messages straight from your custom [contactUs] schema layout grid
            string query = "SELECT contactId, name, email, message FROM [dbo].[contactUs] ORDER BY contactId DESC";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    try
                    {
                        conn.Open();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            pnlNoTickets.Visible = true;
                            rptSupportTickets.Visible = false;
                        }
                        else
                        {
                            pnlNoTickets.Visible = false;
                            rptSupportTickets.Visible = true;
                            rptSupportTickets.DataSource = dt;
                            rptSupportTickets.DataBind();
                        }
                    }
                    catch (Exception ex)
                    {
                        lblSupportStatus.Text = "❌ Database Connection Failure: " + ex.Message;
                        lblSupportStatus.CssClass = "admin-alert-banner alert-error";
                        lblSupportStatus.Visible = true;
                    }
                }
            }
        }

        protected void rptSupportTickets_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblSupportStatus.Visible = false;

            if (e.CommandName == "ArchiveTicket")
            {
                int contactId = Convert.ToInt32(e.CommandArgument);
                string deleteSql = "DELETE FROM [dbo].[contactUs] WHERE contactId = @Id";

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(deleteSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", contactId);
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            lblSupportStatus.Text = $"✓ Support inquiry Ticket #{contactId} marked resolved and archived from ledger parameters.";
                            lblSupportStatus.CssClass = "admin-alert-banner alert-success";
                            lblSupportStatus.Visible = true;

                            LoadActiveSupportTicketsInbox();
                        }
                        catch (Exception ex)
                        {
                            lblSupportStatus.Text = "❌ Operational Query Refusal Error: " + ex.Message;
                            lblSupportStatus.CssClass = "admin-alert-banner alert-error";
                            lblSupportStatus.Visible = true;
                        }
                    }
                }
            }
        }
    }
}