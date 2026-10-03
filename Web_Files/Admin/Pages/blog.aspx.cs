using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_blogs : System.Web.UI.Page
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
                LoadAllPublishedArticles();
            }
        }

        private void LoadAllPublishedArticles()
        {
            string query = "SELECT BlogId, BlogTitle, Excerpt, BlogImage, PublishDate FROM [dbo].[Blogs] ORDER BY PublishDate DESC";
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
                        rptBlogsLedger.DataSource = dt;
                        rptBlogsLedger.DataBind();
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusFeedback("❌ Failed to index live news wire entries: " + ex.Message, false);
                    }
                }
            }
        }

        protected void btnSaveBlog_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            string title = txtTitle.Text.Trim();
            string excerpt = txtExcerpt.Text.Trim();
            string content = txtContent.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                DisplayStatusFeedback("⚠️ An explicit Headline Title and Main Article Content are required parameters.", false);
                return;
            }

            bool isEditing = !string.IsNullOrEmpty(hfActiveBlogID.Value);
            string imageRelativePath = isEditing ? txtCurrentImgPath.Text : "/Assets/uploads/blogs/default-blog.png";

            if (fileBlogImg.HasFile)
            {
                string ext = Path.GetExtension(fileBlogImg.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp")
                {
                    try
                    {
                        string targetDir = Server.MapPath("~/Assets/uploads/blogs/");
                        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                        string cleanFileName = "Blog_" + DateTime.Now.Ticks + ext;
                        fileBlogImg.SaveAs(Path.Combine(targetDir, cleanFileName));
                        imageRelativePath = "~/Assets/uploads/blogs/" + cleanFileName;
                    }
                    catch (Exception fileEx)
                    {
                        DisplayStatusFeedback("❌ Media Storage Write Exception: " + fileEx.Message, false);
                        return;
                    }
                }
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string sql = isEditing ?
                    @"UPDATE [dbo].[Blogs] SET BlogTitle = @Title, Excerpt = @Excerpt, BlogContent = @Content, BlogImage = @Img WHERE BlogId = @BlogId" :
                    @"INSERT INTO [dbo].[Blogs] (BlogTitle, Excerpt, BlogContent, BlogImage, PublishDate) VALUES (@Title, @Excerpt, @Content, @Img, @Date)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Excerpt", string.IsNullOrEmpty(excerpt) ? (object)DBNull.Value : excerpt);
                    cmd.Parameters.AddWithValue("@Content", content);
                    cmd.Parameters.AddWithValue("@Img", imageRelativePath);

                    if (isEditing)
                    {
                        cmd.Parameters.AddWithValue("@BlogId", Convert.ToInt32(hfActiveBlogID.Value));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                    }

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        DisplayStatusFeedback(isEditing ? "✓ Article updated successfully." : "✓ Brand new article entry broadcast onto live store wire feed.", true);
                        ResetAuthoringSuitePanel();
                        LoadAllPublishedArticles();
                    }
                    catch (Exception sqlEx)
                    {
                        DisplayStatusFeedback("❌ Editorial Transaction Database Exception: " + sqlEx.Message, false);
                    }
                }
            }
        }

        protected void rptBlogsLedger_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int blogId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditBlog")
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Blogs] WHERE BlogId = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", blogId);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfActiveBlogID.Value = blogId.ToString();
                                txtTitle.Text = reader["BlogTitle"].ToString();
                                txtExcerpt.Text = reader["Excerpt"].ToString();
                                txtContent.Text = reader["BlogContent"].ToString();
                                txtCurrentImgPath.Text = reader["BlogImage"].ToString();

                                litFormTitle.Text = "Modify Published Article #" + blogId;
                                btnSaveBlog.Text = "Update Broadcast";
                                btnCancelEdit.Visible = true;
                            }
                        }
                    }
                    catch (Exception ex) { DisplayStatusFeedback("❌ Error loading editing layout properties: " + ex.Message, false); }
                }
            }
            else if (e.CommandName == "DeleteBlog")
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Blogs] WHERE BlogId = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", blogId);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        DisplayStatusFeedback("✓ Article permanently dropped from catalog news feeds.", true);
                        if (hfActiveBlogID.Value == blogId.ToString()) ResetAuthoringSuitePanel();
                        LoadAllPublishedArticles();
                    }
                    catch (Exception ex) { DisplayStatusFeedback("❌ Content deletion error instance: " + ex.Message, false); }
                }
            }
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetAuthoringSuitePanel();
        }

        private void ResetAuthoringSuitePanel()
        {
            hfActiveBlogID.Value = "";
            txtTitle.Text = "";
            txtExcerpt.Text = "";
            txtContent.Text = "";
            txtCurrentImgPath.Text = "";
            litFormTitle.Text = "Compose New Article";
            btnSaveBlog.Text = "Publish Article";
            btnCancelEdit.Visible = false;
        }

        private void DisplayStatusFeedback(string text, bool isSuccess)
        {
            lblMessage.Text = text;
            lblMessage.CssClass = isSuccess ? "admin-alert-banner alert-success" : "admin-alert-banner alert-error";
            lblMessage.Visible = true;
        }
    }
}