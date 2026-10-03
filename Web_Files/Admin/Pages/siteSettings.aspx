<%@ Page Title="Global Site Settings | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="admin-settings.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Site Settings | Admin Panel</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container">

            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Global Site Configurations</h1>
                <span class="admin-pill-badge">System Root Settings</span>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="admin-alert-banner" Visible="false"></asp:Label>

            <div class="settings-layout-grid">

                <div class="settings-main-column">

                    <div class="settings-card-panel">
                        <h2 class="settings-card-title">Shop Identity & Branding</h2>
                        <div class="settings-form-grid">
                            <div class="form-group half-width">
                                <label class="settings-label">Logo Text / Title</label>
                                <asp:TextBox ID="txtLogoTitle" runat="server" CssClass="settings-input" placeholder="e.g. Show's Garage"></asp:TextBox>
                            </div>
                            <div class="form-group half-width">
                                <label class="settings-label">Upload Custom Logo Image (.png / .jpg)</label>
                                <asp:FileUpload ID="fileLogo" runat="server" CssClass="settings-input file-picker" />
                            </div>
                            <div class="form-group half-width">
                                <label class="settings-label">Support Email Address</label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="settings-input" TextMode="Email" placeholder="contact@showsgarage.com"></asp:TextBox>
                            </div>
                            <div class="form-group half-width">
                                <label class="settings-label">Business Phone Number</label>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="settings-input" placeholder="10-digit mobile" MaxLength="10"></asp:TextBox>
                            </div>
                            <div class="form-group full-width">
                                <label class="settings-label">Footer Copyright Text Statement</label>
                                <asp:TextBox ID="txtCopyright" runat="server" CssClass="settings-input" placeholder="© 2026 Show's Garage. All Rights Reserved."></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="settings-card-panel">
                        <h2 class="settings-card-title">Homepage Hero Spotlight Elements</h2>
                        <div class="settings-form-grid">
                            <div class="form-group full-width">
                                <label class="settings-label">Main Hero Welcome Heading Text</label>
                                <asp:TextBox ID="txtHeroTitle" runat="server" CssClass="settings-input" placeholder="Premium Collectibles Hub"></asp:TextBox>
                            </div>
                            <div class="form-group full-width">
                                <label class="settings-label">Sub-hero Narrative/Subtitle Paragraph</label>
                                <asp:TextBox ID="txtHeroSubtitle" runat="server" CssClass="settings-input text-area" TextMode="MultiLine" Rows="2" placeholder="Discover detailed diecast scale replicas..."></asp:TextBox>
                            </div>
                            <div class="form-group half-width">
                                <label class="settings-label">Upload New Hero Showcase Banner</label>
                                <asp:FileUpload ID="fileHeroImg" runat="server" CssClass="settings-input file-picker" />
                            </div>
                            <div class="form-group half-width">
                                <label class="settings-label">Current Live Banner Reference Path</label>
                                <asp:TextBox ID="txtCurrentHeroPath" runat="server" CssClass="settings-input path-display" Enabled="false"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="settings-sidebar-column">

                    <div class="settings-card-panel premium-border">
                        <h2 class="settings-card-title text-orange">Payment Gateway & Logistics</h2>
                        <div class="settings-form-grid">

                            <div class="form-group full-width">
                                <label class="settings-label label-highlight">Flat Logistics/Shipping Charges (Rs.)</label>
                                <asp:TextBox ID="txtShippingCharges" runat="server" CssClass="settings-input numeric-input" placeholder="0.00"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label label-highlight">Flat App Platform Convenience Fee (Rs.)</label>
                                <asp:TextBox ID="txtPlatformFee" runat="server" CssClass="settings-input numeric-input" placeholder="0.00"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label label-highlight">Merchant UPI Handle Address</label>
                                <asp:TextBox ID="txtUpiID" runat="server" CssClass="settings-input upi-input" placeholder="username@bank"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Bank Settlement Credentials (Text Layout Block)</label>
                                <asp:TextBox ID="txtBankDetails" runat="server" CssClass="settings-input text-area" TextMode="MultiLine" Rows="4" placeholder="Bank: HDFC Bank&#10;Acc: 50100XXXXXXXXX&#10;IFSC: HDFC0001234"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Upload Active Store Account Payment QR Code Image</label>
                                <asp:FileUpload ID="fileQrCode" runat="server" CssClass="settings-input file-picker" />
                                <div class="current-qr-preview-box" id="divQrContainer" runat="server" visible="false">
                                    <span style="font-size: 10px; color: #666; display: block; margin-bottom: 5px; text-transform: uppercase;">Active QR Preview</span>
                                    <asp:Image ID="imgQrPreview" runat="server" CssClass="qr-img-thumb" />
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="action-submit-panel">
                        <asp:Button ID="btnSaveSettings" runat="server" Text="Save Configuration Parameters" CssClass="btn-settings-save" OnClick="btnSaveSettings_Click" />
                    </div>

                </div>

            </div>
        </main>
    </div>
</asp:Content>