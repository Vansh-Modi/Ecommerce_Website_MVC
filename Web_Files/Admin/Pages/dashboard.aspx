<%@ Page Title="Admin Dashboard | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Admin Command Dashboard | Show's Garage</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/dashboard.css?v=2" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container wide-fluid-layout">
            
            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Control Tower Dashboard</h1>
                <span class="admin-pill-badge status-live">System Online</span>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="admin-alert-banner alert-error" Visible="false"></asp:Label>

            <div class="dashboard-quick-actions-bar">
                <a href="orders.aspx" class="quick-action-link">Process Orders 📦</a>
                <a href="products.aspx" class="quick-action-link">Add Products 🏎️</a>
                <a href="expenses.aspx" class="quick-action-link">Log Expenses 💸</a>
                <a href="contactUs.aspx" class="quick-action-link">Customer Inquiries 📬</a>
                <a href="reports.aspx" class="quick-action-link">P/L Analysis 📊</a>
            </div>

            <div class="dash-summary-cards-grid">
                
                <div class="dash-stat-card card-action-alert">
                    <span class="dash-stat-label">Action Required</span>
                    <div class="dash-stat-value text-blue">
                        <asp:Literal ID="litAwaitingVerifyCount" runat="server" Text="0"></asp:Literal>
                    </div>
                    <p class="dash-stat-desc">Orders awaiting manual payment verification lookup.</p>
                </div>

                <div class="dash-stat-card card-support-alert">
                    <span class="dash-stat-label">Support Mailbox</span>
                    <div class="dash-stat-value text-orange">
                        <asp:Literal ID="litSupportTicketsCount" runat="server" Text="0"></asp:Literal>
                    </div>
                    <p class="dash-stat-desc">Open tickets inside your contactUs inquiry data table.</p>
                </div>

                <div class="dash-stat-card">
                    <span class="dash-stat-label">Total Scale Models</span>
                    <div class="dash-stat-value">
                        <asp:Literal ID="litTotalProductsCount" runat="server" Text="0"></asp:Literal>
                    </div>
                    <p class="dash-stat-desc">Active dynamic replica items inside catalog listings.</p>
                </div>

                <div class="dash-stat-card">
                    <span class="dash-stat-label">Registered Members</span>
                    <div class="dash-stat-value">
                        <asp:Literal ID="litTotalUsersCount" runat="server" Text="0"></asp:Literal>
                    </div>
                    <p class="dash-stat-desc">Total scale model enthusiast customer accounts.</p>
                </div>

                <div class="dash-stat-card">
                    <span class="dash-stat-label">Gross Revenue (MTD)</span>
                    <div class="dash-stat-value text-green">
                        Rs.<asp:Literal ID="litMtdRevenue" runat="server" Text="0.00"></asp:Literal>
                    </div>
                    <p class="dash-stat-desc">Total approved collections logged within current month cycle.</p>
                </div>

            </div>

            <div class="dash-activity-split-grid">
                
                <div class="settings-card-panel premium-border activity-table-card">
                    <h2 class="settings-card-title">Recent Order Pipeline Activity</h2>
                    <div class="dash-table-responsive-wrapper">
                        <asp:Repeater ID="rptRecentOrders" runat="server" OnItemDataBound="rptRecentOrders_ItemDataBound">
                            <HeaderTemplate>
                                <table class="admin-dash-table">
                                    <thead>
                                        <tr>
                                            <th>Order ID</th>
                                            <th>Timestamp</th>
                                            <th>Address Summary</th>
                                            <th style="text-align: right;">Amount</th>
                                            <th style="text-align: center; width: 130px;">Fulfillment State</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="cell-id-code">#<%# Eval("OrderID") %></td>
                                    <td class="cell-timestamp"><%# Eval("OrderDate", "{0:dd MMM HH:mm}") %></td>
                                    <td class="cell-shipping-clip" title='<%# Eval("ShippingAddress") %>'>
                                        <%# Eval("ShippingAddress").ToString().Length > 45 ? Eval("ShippingAddress").ToString().Substring(0, 42) + "..." : Eval("ShippingAddress") %>
                                    </td>
                                    <td class="cell-total-amount">Rs.<%# string.Format("{0:N2}", Eval("TotalAmount")) %></td>
                                    <td style="text-align: center; vertical-align: middle;">
                                        <asp:Label ID="lblRowStatusBadge" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
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

                <div class="settings-card-panel critical-stock-card">
                    <h2 class="settings-card-title text-orange-label">🚨 Low Stock Watchlist</h2>
                    <div class="dash-stock-watchlist-container">
                        <asp:Repeater ID="rptLowStockWatch" runat="server">
                            <ItemTemplate>
                                <div class="stock-watchlist-row">
                                    <div class="watchlist-item-details">
                                        <span class="watchlist-item-title"><%# Eval("Title") %></span>
                                        <span class="watchlist-item-meta">Ratio: <%# Eval("Scale") %> | Brand: <%# Eval("BrandName") %></span>
                                    </div>
                                    <span class='<%# Convert.ToInt32(Eval("StockQuantity")) == 0 ? "watchlist-badge badge-critical" : "watchlist-badge badge-warning" %>'>
                                        <%# Eval("StockQuantity") %> Left
                                    </span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlHealthyStockPlaceholder" runat="server" Visible="false" Style="text-align:center; padding:40px 10px; color:#555;">
                            <div style="font-size:32px; margin-bottom:10px;">🛡️</div>
                            <span style="font-size:12px; font-weight:700; text-transform:uppercase; letter-spacing:0.5px;">All Stock Levels Healthy</span>
                        </asp:Panel>
                    </div>
                </div>

            </div>

        </main>
    </div>
</asp:Content>