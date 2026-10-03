using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class productDetails : Page
    {
        private string connStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null)
            {
                string returnUrl = Request.Url.PathAndQuery;
                Response.Redirect("/Web_Files/Master_Pages/Pages/login.aspx?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl));
                return;
            }

            if (!IsPostBack)
            {
                string productIdStr = Request.QueryString["id"];
                if (string.IsNullOrEmpty(productIdStr))
                {
                    Response.Redirect("shop.aspx");
                    return;
                }

                LoadProductInformation(productIdStr);
            }
        }

        private void LoadProductInformation(string productId)
        {
            const string query = @"
                SELECT Title, Description, MRP, SellingPrice, BrandName, Scale, ImagePath, 
                       ISNULL(StockQuantity, 0) AS StockQuantity 
                FROM Products 
                WHERE ProductID = @prodID;
                
                SELECT ImagePath FROM ProductGallery WHERE ProductID = @prodID;";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@prodID", productId.Trim());
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblProductName.Text = reader["Title"].ToString();
                                lblManufacturer.Text = reader["BrandName"].ToString();

                                string scaleText = reader["Scale"].ToString();
                                lblScaleDisplay.Text = string.IsNullOrEmpty(scaleText) ? "N/A" : scaleText;

                                string descText = reader["Description"].ToString();
                                lblDescription.Text = string.IsNullOrEmpty(descText)
                                    ? "No technical description constraints provided for this diecast model replica."
                                    : descText;

                                decimal price = Convert.ToDecimal(reader["SellingPrice"]);
                                lblPrice.Text = string.Format("{0:N0}", price);

                                int currentStockCount = Convert.ToInt32(reader["StockQuantity"]);
                                if (currentStockCount <= 0)
                                {
                                    lblStockBadge.Text = "Out of Stock";
                                    lblStockBadge.Page.ClientScript.RegisterStartupScript(this.GetType(), "DisableBtn", "");
                                    lblStockBadge.CssClass = "details-stock-badge-indicator stock-out-badge";
                                    btnAddToCart.Text = "Sold Out";
                                    btnAddToCart.Enabled = false;
                                    btnAddToCart.CssClass = "btn-add-to-cart-large btn-add-disabled";
                                }
                                else
                                {
                                    lblStockBadge.Text = "In Stock (" + currentStockCount + " Units)";
                                    lblStockBadge.CssClass = "details-stock-badge-indicator stock-available-badge";
                                    btnAddToCart.Text = "Add to Cart";
                                    btnAddToCart.Enabled = true;
                                    btnAddToCart.CssClass = "btn-add-to-cart-large";
                                }

                                List<string> imageGalleryCollection = new List<string>();
                                string mainImageUrl = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";

                                if (!string.IsNullOrEmpty(mainImageUrl))
                                {
                                    imgProduct.ImageUrl = ResolveUrl(mainImageUrl);
                                    imageGalleryCollection.Add(mainImageUrl);
                                }
                                else
                                {
                                    imgProduct.ImageUrl = ResolveUrl("~/Assets/images/default-model.png");
                                }

                                if (reader.NextResult())
                                {
                                    while (reader.Read())
                                    {
                                        string galleryPath = reader["ImagePath"].ToString();
                                        if (!string.IsNullOrEmpty(galleryPath))
                                        {
                                            imageGalleryCollection.Add(galleryPath);
                                        }
                                    }
                                }

                                if (imageGalleryCollection.Count > 1)
                                {
                                    rptGallery.DataSource = imageGalleryCollection;
                                    rptGallery.DataBind();
                                }
                            }
                            else
                            {
                                Response.Redirect("shop.aspx");
                            }
                        }
                    }
                }
            }
            catch
            {
                Response.Redirect("shop.aspx");
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            lblDetailStatus.Visible = false;

            if (Session["UserID"] == null)
            {
                string targetUrl = Request.RawUrl;
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx?returnUrl=" + Server.UrlEncode(targetUrl));
                return;
            }

            int currentUserId = Convert.ToInt32(Session["UserID"]);
            string prodIdStr = Request.QueryString["id"];

            if (string.IsNullOrEmpty(prodIdStr) || !int.TryParse(prodIdStr.Trim(), out int currentProductId))
            {
                Response.Redirect("shop.aspx");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string verifyStockSql = "SELECT ISNULL(StockQuantity, 0) FROM Products WHERE ProductID = @productID";
                    using (SqlCommand cmdStock = new SqlCommand(verifyStockSql, con))
                    {
                        cmdStock.Parameters.AddWithValue("@productID", currentProductId);
                        if (Convert.ToInt32(cmdStock.ExecuteScalar()) <= 0)
                        {
                            lblDetailStatus.Text = "⚠️ This item has just sold out and cannot be requested for addition.";
                            lblDetailStatus.Visible = true;
                            btnAddToCart.Text = "Sold Out";
                            btnAddToCart.Enabled = false;
                            btnAddToCart.CssClass = "btn-add-to-cart-large btn-add-disabled";
                            return;
                        }
                    }

                    string checkQuery = "SELECT CartID, Quantity FROM Cart WHERE UserID = @userID AND ProductID = @productID";
                    int existingCartId = 0;
                    int currentQuantity = 0;

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@userID", currentUserId);
                        checkCmd.Parameters.AddWithValue("@productID", currentProductId);

                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                existingCartId = Convert.ToInt32(reader["CartID"]);
                                currentQuantity = Convert.ToInt32(reader["Quantity"]);
                            }
                        }
                    }

                    if (existingCartId > 0)
                    {
                        string updateQuery = "UPDATE Cart SET Quantity = @quantity WHERE CartID = @cartID";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@quantity", currentQuantity + 1);
                            updateCmd.Parameters.AddWithValue("@cartID", existingCartId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO Cart (UserID, ProductID, Quantity) VALUES (@userID, @productID, 1)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@userID", currentUserId);
                            insertCmd.Parameters.AddWithValue("@productID", currentProductId);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }

                Response.Redirect("cart.aspx");
            }
            catch
            {
                Response.Redirect("cart.aspx");
            }
        }
    }
}