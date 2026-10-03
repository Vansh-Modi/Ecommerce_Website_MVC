using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_products : System.Web.UI.Page
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
                LoadCategoriesDropdownSelector();
                LoadInventoryItemsMatrixGrid();
                LoadCategoriesManagementLedger();
            }
        }

        // ==========================================
        // SECTION 1: INVENTORY SELECTION CORE BINDERS
        // ==========================================

        private void LoadCategoriesDropdownSelector()
        {
            string query = "SELECT CategoryID, CategoryName FROM [dbo].[Categories] ORDER BY CategoryName ASC";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        ddlCategory.DataSource = cmd.ExecuteReader();
                        ddlCategory.DataTextField = "CategoryName";
                        ddlCategory.DataValueField = "CategoryID";
                        ddlCategory.DataBind();
                    }
                    catch { }
                }
            }
            ddlCategory.Items.Insert(0, new ListItem("-- Select Category Shell --", ""));
        }

        private void LoadInventoryItemsMatrixGrid()
        {
            string query = @"
                SELECT p.*, c.CategoryName 
                FROM [dbo].[Products] p
                INNER JOIN [dbo].[Categories] c ON p.CategoryID = c.CategoryID
                ORDER BY p.ProductID DESC";

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
                        rptInventoryMatrix.DataSource = dt;
                        rptInventoryMatrix.DataBind();
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusFeedback("❌ Failed to bind stock metrics register: " + ex.Message, false);
                    }
                }
            }
        }

        private void LoadCategoriesManagementLedger()
        {
            string query = "SELECT CategoryID, CategoryName FROM [dbo].[Categories] ORDER BY CategoryName ASC";
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
                        rptCategoriesList.DataSource = dt;
                        rptCategoriesList.DataBind();
                    }
                    catch { }
                }
            }
        }

        private void LoadProductGalleryAdministrationGrid(int productId)
        {
            List<string> imagePaths = new List<string>();
            string query = "SELECT ImagePath FROM [dbo].[ProductGallery] WHERE ProductID = @ProductID ORDER BY GalleryID ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                imagePaths.Add(rdr["ImagePath"].ToString());
                            }
                        }
                    }
                    catch { }
                }
            }

            if (imagePaths.Count > 0)
            {
                rptEditProductGallery.DataSource = imagePaths;
                rptEditProductGallery.DataBind();
                pnlActiveGalleryManagementBlock.Visible = true;
            }
            else
            {
                pnlActiveGalleryManagementBlock.Visible = false;
            }
        }

        // ==========================================
        // SECTION 2: WORKSPACE TOGGLE ROUTINES
        // ==========================================

        protected void lnkSwitchToCategoryMode_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            pnlProductFormLayout.Visible = false;
            pnlCategoryFormLayout.Visible = true;

            litPageMainHeading.Text = "Categories Configuration Master";
            litPageBadgeText.Text = "Classification Settings";
            ResetCategoryFormPanel();
        }

        protected void lnkReturnToProductMode_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            pnlProductFormLayout.Visible = true;
            pnlCategoryFormLayout.Visible = false;

            litPageMainHeading.Text = "Garage Inventory Matrix";
            litPageBadgeText.Text = "Stock Control Panel";
            ResetProductFormPanel();
        }

        // ==========================================
        // SECTION 3: PRODUCTS OPERATIONAL ACTIONS
        // ==========================================

        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            string title = txtTitle.Text.Trim();
            string brand = txtBrandName.Text.Trim();
            string scale = txtScale.Text.Trim();
            string categoryVal = ddlCategory.SelectedValue;
            string description = txtDescription.Text.Trim();
            bool isNewArrival = chkIsNewArrival.Checked;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(categoryVal) || string.IsNullOrEmpty(txtSellingPrice.Text.Trim()) || string.IsNullOrEmpty(txtCostPrice.Text.Trim()) || string.IsNullOrEmpty(txtMRP.Text.Trim()))
            {
                DisplayStatusFeedback("⚠️ Title, Category configuration, MRP field, and both financial valuation properties are strictly required.", false);
                return;
            }

            int stock = 0;
            int.TryParse(txtStockQuantity.Text.Trim(), out stock);

            decimal costPrice = 0, sellingPrice = 0, mrpValue = 0;
            decimal.TryParse(txtCostPrice.Text.Trim(), out costPrice);
            decimal.TryParse(txtSellingPrice.Text.Trim(), out sellingPrice);
            decimal.TryParse(txtMRP.Text.Trim(), out mrpValue);

            bool isEditing = !string.IsNullOrEmpty(hfActiveProductID.Value);
            string finalImgRelativePath = isEditing ? txtCurrentImgPath.Text : "/Assets/images/default-model.png";

            // A. Execute Single Main Thumbnail Management Pipeline
            if (fileProductImg.HasFile)
            {
                string ext = Path.GetExtension(fileProductImg.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp")
                {
                    try
                    {
                        string targetDir = Server.MapPath("~/Assets/uploads/products/");
                        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                        string customUniqueFileName = "Model_" + DateTime.Now.Ticks + ext;
                        fileProductImg.SaveAs(Path.Combine(targetDir, customUniqueFileName));
                        finalImgRelativePath = "/Assets/uploads/products/" + customUniqueFileName;
                    }
                    catch (Exception fileEx)
                    {
                        DisplayStatusFeedback("❌ Thumbnail Save Error: " + fileEx.Message, false);
                        return;
                    }
                }
                else
                {
                    DisplayStatusFeedback("❌ Format Error: Main thumbnail configurations must use standard images (.jpg, .jpeg, .png, .webp).", false);
                    return;
                }
            }

            int activeWorkingProductId = 0;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string sql = isEditing ?
                    @"UPDATE [dbo].[Products] 
                      SET Title = @Title, BrandName = @BrandName, CategoryID = @CategoryID, Scale = @Scale, 
                          Description = @Description, SellingPrice = @SellingPrice, CostPrice = @CostPrice, MRP = @MRP,
                          StockQuantity = @StockQuantity, ImagePath = @ImagePath, IsNewArrival = @IsNewArrival
                      WHERE ProductID = @ProductID" :
                    @"INSERT INTO [dbo].[Products] (Title, BrandName, CategoryID, Scale, Description, SellingPrice, CostPrice, MRP, StockQuantity, ImagePath, IsNewArrival, CreatedAt)
                      VALUES (@Title, @BrandName, @CategoryID, @Scale, @Description, @SellingPrice, @CostPrice, @MRP, @StockQuantity, @ImagePath, @IsNewArrival, @CreatedAt);
                      SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@BrandName", string.IsNullOrEmpty(brand) ? (object)DBNull.Value : brand);
                    cmd.Parameters.AddWithValue("@CategoryID", Convert.ToInt32(categoryVal));
                    cmd.Parameters.AddWithValue("@Scale", string.IsNullOrEmpty(scale) ? (object)DBNull.Value : scale);
                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                    cmd.Parameters.AddWithValue("@SellingPrice", sellingPrice);
                    cmd.Parameters.AddWithValue("@CostPrice", costPrice);
                    cmd.Parameters.AddWithValue("@MRP", mrpValue);
                    cmd.Parameters.AddWithValue("@StockQuantity", stock);
                    cmd.Parameters.AddWithValue("@ImagePath", finalImgRelativePath);
                    cmd.Parameters.AddWithValue("@IsNewArrival", isNewArrival);

                    if (isEditing)
                    {
                        activeWorkingProductId = Convert.ToInt32(hfActiveProductID.Value);
                        cmd.Parameters.AddWithValue("@ProductID", activeWorkingProductId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    }

                    try
                    {
                        conn.Open();
                        if (isEditing)
                        {
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            activeWorkingProductId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                    catch (Exception sqlEx)
                    {
                        DisplayStatusFeedback("❌ Data Query Execution Failure: " + sqlEx.Message, false);
                        return;
                    }
                }
            }

            // B. Multi-File Processing Loop for Dynamic Secondary ProductGallery Collections
            if (fileProductGallery.HasFiles)
            {
                string targetGalleryDir = Server.MapPath("~/Assets/uploads/gallery/");
                if (!Directory.Exists(targetGalleryDir)) Directory.CreateDirectory(targetGalleryDir);

                foreach (var currentFile in fileProductGallery.PostedFiles)
                {
                    string fileExtension = Path.GetExtension(currentFile.FileName).ToLower();
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".webp")
                    {
                        try
                        {
                            string internalUniqueName = "Gallery_" + DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString().Substring(0, 5) + fileExtension;
                            currentFile.SaveAs(Path.Combine(targetGalleryDir, internalUniqueName));
                            string galleryRelativePathString = "/Assets/uploads/gallery/" + internalUniqueName;

                            // Insert matching mapping row entries back to database structures layout
                            string galleryInsertQuery = "INSERT INTO [dbo].[ProductGallery] (ProductID, ImagePath) VALUES (@ProductID, @ImagePath)";
                            using (SqlConnection galleryConn = new SqlConnection(ConnectionString))
                            {
                                using (SqlCommand galleryCmd = new SqlCommand(galleryInsertQuery, galleryConn))
                                {
                                    galleryCmd.Parameters.AddWithValue("@ProductID", activeWorkingProductId);
                                    galleryCmd.Parameters.AddWithValue("@ImagePath", galleryRelativePathString);
                                    galleryConn.Open();
                                    galleryCmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception galleryEx)
                        {
                            DisplayStatusFeedback("⚠️ Main parameters saved, but sub-gallery items failed to process completely: " + galleryEx.Message, false);
                        }
                    }
                }
            }

            DisplayStatusFeedback(isEditing ? "✓ Inventory configuration matrix parameters updated successfully." : "✓ Brand new item listing safely injected inside garage catalog data.", true);
            ResetProductFormPanel();
            LoadInventoryItemsMatrixGrid();
        }

        protected void rptInventoryMatrix_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditProduct")
                PopulateFormForProductEditing(productId);
            else if (e.CommandName == "DeleteProduct")
                ExecutePermanentProductDeletion(productId);
        }

        protected void rptEditProductGallery_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DropGalleryImage")
            {
                string targetDbPathToDelete = e.CommandArgument.ToString();
                int currentActiveProdId = Convert.ToInt32(hfActiveProductID.Value);

                string query = "DELETE FROM [dbo].[ProductGallery] WHERE ProductID = @ProductID AND ImagePath = @ImagePath";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", currentActiveProdId);
                        cmd.Parameters.AddWithValue("@ImagePath", targetDbPathToDelete);

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();

                            // Attempt physical cleaning logic routines from server discs structure
                            try
                            {
                                string completePhysicalDiskPath = Server.MapPath("~" + targetDbPathToDelete.Replace("~", ""));
                                if (File.Exists(completePhysicalDiskPath))
                                {
                                    File.Delete(completePhysicalDiskPath);
                                }
                            }
                            catch { }

                            DisplayStatusFeedback("✓ Sub-gallery item removed cleanly.", true);
                            LoadProductGalleryAdministrationGrid(currentActiveProdId);
                        }
                        catch (Exception ex)
                        {
                            DisplayStatusFeedback("❌ Failed to clear sub-image item row allocation parameters: " + ex.Message, false);
                        }
                    }
                }
            }
        }

        private void PopulateFormForProductEditing(int productId)
        {
            string query = "SELECT * FROM [dbo].[Products] WHERE ProductID = @ProductID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hfActiveProductID.Value = productId.ToString();
                                txtTitle.Text = reader["Title"].ToString();
                                txtBrandName.Text = reader["BrandName"].ToString();
                                txtScale.Text = reader["Scale"].ToString();
                                ddlCategory.SelectedValue = reader["CategoryID"].ToString();
                                txtStockQuantity.Text = reader["StockQuantity"].ToString();
                                txtCostPrice.Text = string.Format("{0:F2}", reader["CostPrice"]);
                                txtSellingPrice.Text = string.Format("{0:F2}", reader["SellingPrice"]);
                                txtMRP.Text = string.Format("{0:F2}", reader["MRP"]);
                                txtCurrentImgPath.Text = reader["ImagePath"].ToString();
                                chkIsNewArrival.Checked = Convert.ToBoolean(reader["IsNewArrival"]);
                                txtDescription.Text = reader["Description"].ToString();

                                litFormTitle.Text = "Modify Scale Model #" + productId;
                                btnSaveProduct.Text = "Update Listing";
                                btnCancelEdit.Visible = true;
                                lblMessage.Visible = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusFeedback("❌ Edit State Fetch Failure: " + ex.Message, false);
                    }
                }
            }
            // Bind supplementary gallery items for editing workflows
            LoadProductGalleryAdministrationGrid(productId);
        }

        private void ExecutePermanentProductDeletion(int productId)
        {
            // Note: Cascade Delete on foreign keys handles ProductGallery cleanup in the DB.
            string query = "DELETE FROM [dbo].[Products] WHERE ProductID = @ProductID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        DisplayStatusFeedback("✓ Selected model product profile successfully excised from active database catalog registers.", true);

                        if (hfActiveProductID.Value == productId.ToString()) ResetProductFormPanel();
                        LoadInventoryItemsMatrixGrid();
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusFeedback("❌ Operations Restriction: Cannot excise model item. It might be linked to past customer order logs tracking tables. " + ex.Message, false);
                    }
                }
            }
        }

        // ==========================================
        // SECTION 4: CATEGORY OPERATIONAL ACTIONS
        // ==========================================

        protected void btnSaveCategory_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            string categoryName = txtCategoryName.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                DisplayStatusFeedback("⚠️ Please specify a category classification title name string.", false);
                return;
            }

            bool isEditingCategory = !string.IsNullOrEmpty(hfActiveCategoryID.Value);
            string sql = isEditingCategory ?
                "UPDATE [dbo].[Categories] SET CategoryName = @Name WHERE CategoryID = @CategoryID" :
                "INSERT INTO [dbo].[Categories] (CategoryName) VALUES (@Name)";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", categoryName);
                    if (isEditingCategory)
                        cmd.Parameters.AddWithValue("@CategoryID", Convert.ToInt32(hfActiveCategoryID.Value));

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        DisplayStatusFeedback(isEditingCategory ? "✓ Category entry title renamed successfully." : "✓ Brand new categories structure classification row committed.", true);
                        ResetCategoryFormPanel();
                        LoadCategoriesDropdownSelector();
                        LoadCategoriesManagementLedger();
                        LoadInventoryItemsMatrixGrid();
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusFeedback("❌ Query Processing Failure: " + ex.Message, false);
                    }
                }
            }
        }

        protected void rptCategoriesList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int categoryId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditCategory")
            {
                string query = "SELECT CategoryName FROM [dbo].[Categories] WHERE CategoryID = @ID";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", categoryId);
                        try
                        {
                            conn.Open();
                            object nameResult = cmd.ExecuteScalar();
                            if (nameResult != null)
                            {
                                hfActiveCategoryID.Value = categoryId.ToString();
                                txtCategoryName.Text = nameResult.ToString();
                                litCategoryFormTitle.Text = "Edit Category Details #" + categoryId;
                                btnSaveCategory.Text = "Update Category";
                                btnCancelCategoryEdit.Visible = true;
                                lblMessage.Visible = false;
                            }
                        }
                        catch { }
                    }
                }
            }
            else if (e.CommandName == "DeleteCategory")
            {
                string query = "DELETE FROM [dbo].[Categories] WHERE CategoryID = @ID";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", categoryId);
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();

                            DisplayStatusFeedback("✓ Category definition completely dropped from database schemas.", true);
                            if (hfActiveCategoryID.Value == categoryId.ToString()) ResetCategoryFormPanel();
                            LoadCategoriesDropdownSelector();
                            LoadCategoriesManagementLedger();
                            LoadInventoryItemsMatrixGrid();
                        }
                        catch (Exception ex)
                        {
                            DisplayStatusFeedback("❌ Restriction Error: Cannot drop shell category. Ensure no products are currently linked to this grouping. Error: " + ex.Message, false);
                        }
                    }
                }
            }
        }

        protected void btnCancelCategoryEdit_Click(object sender, EventArgs e)
        {
            ResetCategoryFormPanel();
        }

        // ==========================================
        // SECTION 5: CLEAN FLUSH RESET ROUTINES
        // ==========================================

        private void ResetProductFormPanel()
        {
            hfActiveProductID.Value = "";
            txtTitle.Text = "";
            txtBrandName.Text = "";
            txtScale.Text = "";
            ddlCategory.SelectedIndex = 0;
            txtStockQuantity.Text = "";
            txtCostPrice.Text = "";
            txtSellingPrice.Text = "";
            txtMRP.Text = "";
            txtCurrentImgPath.Text = "";
            txtDescription.Text = "";
            chkIsNewArrival.Checked = true;

            litFormTitle.Text = "Add New Scale Model";
            btnSaveProduct.Text = "Save Scale Model";
            btnCancelEdit.Visible = false;
            pnlActiveGalleryManagementBlock.Visible = false;
        }

        private void ResetCategoryFormPanel()
        {
            hfActiveCategoryID.Value = "";
            txtCategoryName.Text = "";
            litCategoryFormTitle.Text = "Create New Store Category";
            btnSaveCategory.Text = "Save Category";
            btnCancelCategoryEdit.Visible = false;
        }

        private void DisplayStatusFeedback(string text, bool isSuccess)
        {
            lblMessage.Text = text;
            lblMessage.CssClass = isSuccess ? "admin-alert-banner alert-success" : "admin-alert-banner alert-error";
            lblMessage.Visible = true;
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetProductFormPanel();
        }
    }
}