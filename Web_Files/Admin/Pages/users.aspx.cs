using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_users : System.Web.UI.Page
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
                LoadRegisteredUsersDirectory();
            }
        }

        private void LoadRegisteredUsersDirectory()
        {
            // Selects entire profile collection sorted chronologically by registration date milestones
            string query = "SELECT UserID, FullName, Email, Phone, IsVerified, Role, CreatedAt, LastLogin FROM [dbo].[Users] ORDER BY CreatedAt DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtUsers = new DataTable();
                    try
                    {
                        conn.Open();
                        da.Fill(dtUsers);
                        rptUsersRegistry.DataSource = dtUsers;
                        rptUsersRegistry.DataBind();
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusNotification("❌ Error fetching accounts index schema layers: " + ex.Message, false);
                    }
                }
            }
        }

        protected void rptUsersRegistry_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rowView = (DataRowView)e.Item.DataItem;
                string assignedRole = rowView["Role"].ToString();

                // Align the current system role assignment variable directly with the item level dropdown selection
                DropDownList ddlUserRole = (DropDownList)e.Item.FindControl("ddlUserRole");
                if (ddlUserRole != null)
                {
                    ListItem matchingItem = ddlUserRole.Items.FindByValue(assignedRole);
                    if (matchingItem != null)
                    {
                        ddlUserRole.SelectedValue = assignedRole;
                    }
                    else
                    {
                        // Fallback baseline execution path handling rule context
                        ddlUserRole.SelectedIndex = 0;
                    }
                }
            }
        }

        protected void rptUsersRegistry_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblAdminStatus.Visible = false;
            int targetUserId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "UpdateUserRole")
            {
                // Extract selected role drop down value metric mapping string from target item row components index
                DropDownList ddlRole = (DropDownList)e.Item.FindControl("ddlUserRole");
                if (ddlRole != null)
                {
                    string selectedRole = ddlRole.SelectedValue;
                    ExecuteAccountPrivilegeUpdate(targetUserId, selectedRole);
                }
            }
        }

        private void ExecuteAccountPrivilegeUpdate(int userId, string targetRole)
        {
            // Safeguard against accidentally locked systems loops state
            int currentAdminSessionId = Convert.ToInt32(Session["UserID"]);
            if (userId == currentAdminSessionId && targetRole != "Admin")
            {
                DisplayStatusNotification("⚠️ Operations Halted: Security parameters protect you from stripping your own active Admin access credentials privileges.", false);
                return;
            }

            string sql = "UPDATE [dbo].[Users] SET Role = @Role WHERE UserID = @UserID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Role", targetRole);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        DisplayStatusNotification($"✓ Access authorization guidelines for Profile User #{userId} updated successfully to '{targetRole}'.", true);
                        LoadRegisteredUsersDirectory(); // Re-index active layout grid mapping parameters
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusNotification("❌ Database Access Overwrite Rejected: " + ex.Message, false);
                    }
                }
            }
        }

        private void DisplayStatusNotification(string text, bool isSuccess)
        {
            lblAdminStatus.Text = text;
            lblAdminStatus.CssClass = isSuccess ? "admin-alert-banner alert-success" : "admin-alert-banner alert-error";
            lblAdminStatus.Visible = true;
        }
    }
}