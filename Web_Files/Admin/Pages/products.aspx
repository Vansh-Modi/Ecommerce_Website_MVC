<%@ Page Title="Manage Inventory | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="products.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Product Inventory | Admin Hub</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/products.css?v=3" rel="stylesheet" type="text/css" />
    <style>
        .category-management-link-wrapper {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .btn-mode-toggle {
            font-size: 11px;
            color: #0076df;
            text-decoration: none;
            font-weight: 700;
            text-transform: uppercase;
            cursor: pointer;
        }

            .btn-mode-toggle:hover {
                text-decoration: underline;
                color: #0096ff;
            }

        .category-bullet-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background-color: #0d0d0d;
            border: 1px solid #1c1c1c;
            padding: 10px 14px;
            border-radius: 4px;
            margin-bottom: 10px;
        }

        .cat-title-text {
            color: #ffffff;
            font-size: 13px;
            font-weight: 600;
        }

        .cat-action-links a, .cat-action-links input[type="submit"] {
            font-size: 11px;
            color: #666;
            text-decoration: none;
            margin-left: 6px;
            cursor: pointer;
            background: none;
            border: none;
            padding: 0;
        }

        .cat-action-links .cat-edit-link:hover {
            color: #0076df;
            text-decoration: underline;
        }

        .cat-action-links .cat-delete-link:hover {
            color: #ff3333;
            text-decoration: underline;
        }

        /* Multiline Sub-Gallery Photo Control CSS Core */
        .admin-gallery-manager-panel {
            margin-top: 15px;
            padding-top: 15px;
            border-top: 1px dashed #222;
        }

        .admin-gallery-grid-wrapper {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 10px;
        }

        .admin-gallery-thumb-card {
            position: relative;
            width: 80px;
            height: 80px;
            border: 1px solid #222;
            border-radius: 4px;
            overflow: hidden;
            background-color: #000;
        }

            .admin-gallery-thumb-card img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }

            .admin-gallery-thumb-card .btn-drop-gallery-img {
                position: absolute;
                top: 2px;
                right: 2px;
                background: rgba(255, 51, 51, 0.9);
                color: #fff;
                border: none;
                border-radius: 3px;
                font-size: 10px;
                padding: 2px 4px;
                cursor: pointer;
                line-height: 1;
                font-weight: bold;
            }

                .admin-gallery-thumb-card .btn-drop-gallery-img:hover {
                    background: #ff0000;
                }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container wide-fluid-layout">

            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">
                    <asp:Literal ID="litPageMainHeading" runat="server" Text="Garage Inventory Matrix"></asp:Literal>
                </h1>
                <span class="admin-pill-badge">
                    <asp:Literal ID="litPageBadgeText" runat="server" Text="Stock Control Panel"></asp:Literal>
                </span>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="admin-alert-banner" Visible="false"></asp:Label>

            <asp:HiddenField ID="hfActiveProductID" runat="server" Value="" />
            <asp:HiddenField ID="hfActiveCategoryID" runat="server" Value="" />

            <div class="products-split-grid">

                <asp:Panel ID="pnlProductFormLayout" runat="server" CssClass="product-form-panel-column">
                    <div class="settings-card-panel">
                        <h2 class="settings-card-title">
                            <asp:Literal ID="litFormTitle" runat="server" Text="Add New Scale Model"></asp:Literal></h2>

                        <div class="settings-form-grid">
                            <div class="form-group full-width">
                                <label class="settings-label">Model Replica Title</label>
                                <asp:TextBox ID="txtTitle" runat="server" CssClass="settings-input" placeholder="e.g., Nissan Skyline GT-R R34 V-Spec II"></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <label class="settings-label">Brand Label / Manufacturer</label>
                                <asp:TextBox ID="txtBrandName" runat="server" CssClass="settings-input" placeholder="e.g., Autoart, Solido, Maisto"></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <label class="settings-label">Scale Ratio Proportion</label>
                                <asp:TextBox ID="txtScale" runat="server" CssClass="settings-input" placeholder="e.g., 1:18, 1:43, 1:64"></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <div class="category-management-link-wrapper">
                                    <label class="settings-label">Assigned Category Link</label>
                                    <asp:LinkButton ID="lnkSwitchToCategoryMode" runat="server" CssClass="btn-mode-toggle" OnClick="lnkSwitchToCategoryMode_Click" CausesValidation="false">Manage Categories ⚙️</asp:LinkButton>
                                </div>
                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="settings-input select-arrow-override"></asp:DropDownList>
                            </div>

                            <div class="form-group half-width">
                                <label class="settings-label">Initial Stock Quantity Level</label>
                                <asp:TextBox ID="txtStockQuantity" runat="server" CssClass="settings-input" TextMode="Number" placeholder="0"></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <label class="settings-label">Sourcing Cost Price (Rs.)</label>
                                <asp:TextBox ID="txtCostPrice" runat="server" CssClass="settings-input numeric-cost" placeholder="0.00"></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <label class="settings-label">Factory Print MRP (Rs.)</label>
                                <asp:TextBox ID="txtMRP" runat="server" CssClass="settings-input numeric-mrp" placeholder="0.00"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Retail Selling Price (Rs.)</label>
                                <asp:TextBox ID="txtSellingPrice" runat="server" CssClass="settings-input numeric-sell" placeholder="0.00"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Upload Primary Thumbnail (.png / .jpg)</label>
                                <asp:FileUpload ID="fileProductImg" runat="server" CssClass="settings-input file-picker" />
                                <asp:TextBox ID="txtCurrentImgPath" runat="server" CssClass="settings-input path-fallback-label" Enabled="false" Visible="false"></asp:TextBox>
                            </div>

                            <div class="form-group full-width" style="margin-top: 5px;">
                                <label class="settings-label">Upload Additional Gallery Photos (Multiple Allowed)</label>
                                <asp:FileUpload ID="fileProductGallery" runat="server" CssClass="settings-input file-picker" AllowMultiple="true" />
                            </div>

                            <asp:Panel ID="pnlActiveGalleryManagementBlock" runat="server" CssClass="form-group full-width admin-gallery-manager-panel" Visible="false">
                                <label class="settings-label" style="color: #0076df;">Active Multi-Image Gallery Assets</label>
                                <div class="admin-gallery-grid-wrapper">
                                    <asp:Repeater ID="rptEditProductGallery" runat="server" OnItemCommand="rptEditProductGallery_ItemCommand">
                                        <ItemTemplate>
                                            <div class="admin-gallery-thumb-card">
                                                <img src='<%# ResolveUrl("~" + Container.DataItem.ToString().Replace("~","")) %>' alt="Gallery asset" />
                                                <asp:Button ID="btnDeleteGalleryItem" runat="server" Text="✕" CssClass="btn-drop-gallery-img" CommandName="DropGalleryImage" CommandArgument='<%# Container.DataItem %>' OnClientClick="return confirm('Remove this specific image from product sub-gallery registers?');" CausesValidation="false" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </asp:Panel>

                            <div class="form-group full-width" style="display: flex; align-items: center; gap: 8px; margin: 10px 0;">
                                <asp:CheckBox ID="chkIsNewArrival" runat="server" Checked="true" Style="margin: 0; cursor: pointer;" />
                                <label for="<%= chkIsNewArrival.ClientID %>" class="checkbox-custom-label" style="margin: 0; cursor: pointer; font-size: 13px; color: #ccc; line-height: 1;">
                                    Flag as New Arrival Display Item
                                </label>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Technical Description & Highlights</label>
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="settings-input text-area" TextMode="MultiLine" Rows="4" placeholder="Enter specifications..."></asp:TextBox>
                            </div>

                            <div class="summary-actions-block full-width processing-buttons-row">
                                <asp:Button ID="btnSaveProduct" runat="server" Text="Save Scale Model" CssClass="btn-settings-save" OnClick="btnSaveProduct_Click" />
                                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn-settings-cancel" OnClick="btnCancelEdit_Click" Visible="false" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlCategoryFormLayout" runat="server" CssClass="product-form-panel-column" Visible="false">
                    <div class="settings-card-panel">
                        <div class="category-management-link-wrapper" style="border-bottom: 1px solid #222; padding-bottom: 10px; margin-bottom: 20px;">
                            <h2 class="settings-card-title" style="margin: 0; padding: 0; border: none;">
                                <asp:Literal ID="litCategoryFormTitle" runat="server" Text="Create New Store Category"></asp:Literal></h2>
                            <asp:LinkButton ID="lnkReturnToProductMode" runat="server" CssClass="btn-mode-toggle" OnClick="lnkReturnToProductMode_Click" Style="color: #ff5722;" CausesValidation="false">← Back to Products</asp:LinkButton>
                        </div>

                        <div class="settings-form-grid">
                            <div class="form-group full-width" style="margin-bottom: 10px;">
                                <label class="settings-label label-highlight">Category Classification Name</label>
                                <asp:TextBox ID="txtCategoryName" runat="server" CssClass="settings-input" placeholder="e.g., Diecast 1:18 Premium, JDM Special Edition"></asp:TextBox>
                            </div>

                            <div class="summary-actions-block full-width processing-buttons-row" style="margin-bottom: 20px;">
                                <asp:Button ID="btnSaveCategory" runat="server" Text="Save Category" CssClass="btn-settings-save" OnClick="btnSaveCategory_Click" Style="background-color: #0076df;" />
                                <asp:Button ID="btnCancelCategoryEdit" runat="server" Text="Cancel" CssClass="btn-settings-cancel" OnClick="btnCancelCategoryEdit_Click" Visible="false" />
                            </div>
                        </div>

                        <h3 class="settings-label" style="margin-bottom: 12px; display: block;">Active Shell Categories Ledger</h3>
                        <div class="category-live-ledger-stack">
                            <asp:Repeater ID="rptCategoriesList" runat="server" OnItemCommand="rptCategoriesList_ItemCommand">
                                <ItemTemplate>
                                    <div class="category-bullet-row">
                                        <span class="cat-title-text"><%# Eval("CategoryName") %></span>
                                        <div class="cat-action-links">
                                            <asp:LinkButton ID="btnEditCat" runat="server" Text="Edit ✎" CssClass="cat-edit-link" CommandName="EditCategory" CommandArgument='<%# Eval("CategoryID") %>' CausesValidation="false"></asp:LinkButton>
                                            <span style="color: #222;">|</span>
                                            <asp:LinkButton ID="btnDeleteCat" runat="server" Text="Delete 🗑" CssClass="cat-delete-link" CommandName="DeleteCategory" CommandArgument='<%# Eval("CategoryID") %>' OnClientClick="return confirm('Warning: Deleting this category will detach it from all assigned scale model catalog products. Proceed?');" CausesValidation="false"></asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </asp:Panel>

                <div class="product-grid-display-column">
                    <div class="settings-card-panel premium-border">
                        <h2 class="settings-card-title">Live Garage Inventory Index</h2>

                        <div class="inventory-table-responsive-wrapper">
                            <asp:Repeater ID="rptInventoryMatrix" runat="server" OnItemCommand="rptInventoryMatrix_ItemCommand">
                                <HeaderTemplate>
                                    <table class="admin-inventory-table">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center; width: 70px;">Model</th>
                                                <th>Item Specifications</th>
                                                <th>Category</th>
                                                <th style="text-align: right;">Financials</th>
                                                <th style="text-align: center; width: 60px;">Stock</th>
                                                <th style="text-align: center; width: 110px;">Actions</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td style="text-align: center; vertical-align: middle;">
                                            <img src='<%# ResolveUrl(string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? "~/Assets/images/default-model.png" : "~" + Eval("ImagePath").ToString().Replace("~","")) %>' class="inventory-thumbnail" alt="Replica Preview" />
                                        </td>
                                        <td>
                                            <span class="inv-item-title"><%# Eval("Title") %></span>
                                            <div class="inv-item-sub-meta">
                                                Label: <strong><%# Eval("BrandName") %></strong> | Prop: <strong style="color: #0076df;"><%# Eval("Scale") %></strong>
                                                <%# Convert.ToBoolean(Eval("IsNewArrival")) ? "<span class='new-arrival-inline-pill'>New</span>" : "" %>
                                            </div>
                                        </td>
                                        <td style="vertical-align: middle;"><span class="inv-category-tag"><%# Eval("CategoryName") %></span></td>
                                        <td class="inv-financials-cell">
                                            <span class="sell-cost-display" style="color: #25D366;">S: Rs.<%# string.Format("{0:N0}", Eval("SellingPrice")) %></span><span class="mrp-cost-display" style="color: #ffcc00; display: block; font-size: 11px;">M: Rs.<%# string.Format("{0:N0}", Eval("MRP")) %></span><span class="base-cost-display">C: Rs.<%# string.Format("{0:N0}", Eval("CostPrice")) %></span></td>
                                        <td style="text-align: center; vertical-align: middle;">
                                            <span class='<%# Convert.ToInt32(Eval("StockQuantity")) <= 0 ? "stock-badge stock-empty" : "stock-badge stock-healthy" %>'>
                                                <%# Eval("StockQuantity") %>
                                            </span>
                                        </td>
                                        <td class="inv-actions-cell" style="text-align: center; vertical-align: middle;">
                                            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="inv-btn-link edit-trigger" CommandName="EditProduct" CommandArgument='<%# Eval("ProductID") %>' CausesValidation="false">Edit</asp:LinkButton>
                                            <span style="color: #222; margin: 0 2px;">|</span>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="inv-btn-link delete-trigger" CommandName="DeleteProduct" CommandArgument='<%# Eval("ProductID") %>' OnClientClick="return confirm('Are you sure you want to delete this scale model from your store listing completely?');" CausesValidation="false">Delete</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>

            </div>
        </main>
    </div>
</asp:Content>
