<%@ Page Title="Manage Orders | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="orders.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Orders | Admin Dashboard</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/orders.css?v=3" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container universal-wide-layout">
            
            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Incoming Order Pipeline</h1>
                <span class="admin-pill-badge">Live Store Operations</span>
            </div>

            <div class="admin-filter-strip">
                <div class="filter-controls-group">
                    <span class="filter-strip-label">Pipeline State Filter:</span>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged" CssClass="admin-filter-dropdown select-arrow-fix">
                        <asp:ListItem Value="ALL">-- Display Entire Order Log History --</asp:ListItem>
                        <asp:ListItem Value="Awaiting Verification">Awaiting Verification Only</asp:ListItem>
                        <asp:ListItem Value="Approved">Approved / Processing</asp:ListItem>
                        <asp:ListItem Value="Shipped">Shipped / Dispatched Logs</asp:ListItem>
                        <asp:ListItem Value="Delivered">Delivered Logs</asp:ListItem>
                        <asp:ListItem Value="Cancelled">Cancelled Logs</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-strip-hint">
                    Serialized logs mapping based on state constraints.
                </div>
            </div>

            <asp:Label ID="lblAdminStatus" runat="server" CssClass="admin-alert-banner" Visible="false"></asp:Label>

            <div class="admin-orders-vertical-stack">
                <asp:Repeater ID="rptAdminOrders" runat="server" OnItemDataBound="rptAdminOrders_ItemDataBound" OnItemCommand="rptAdminOrders_ItemCommand">
                    <ItemTemplate>
                        <div class="admin-order-master-card">
                            
                            <div class="admin-order-card-header">
                                <div class="header-meta-group">
                                    <span class="meta-label">Date Logged</span>
                                    <span class="meta-value"><%# Eval("OrderDate", "{0:dd MMM yyyy HH:mm}") %></span>
                                </div>
                                <div class="header-meta-group">
                                    <span class="meta-label">Total Invoice Value</span>
                                    <span class="meta-value text-green">Rs.<%# string.Format("{0:N2}", Eval("TotalAmount")) %></span></div>
                                <div class="header-meta-group">
                                    <span class="meta-label">Customer ID</span>
                                    <span class="meta-value">User #<%# Eval("UserID") %></span></div>
                                <div class="header-meta-group id-right-align">
                                    <span class="meta-label">Order ID Token</span>
                                    <span class="meta-value id-code-highlight">#<%# Eval("OrderID") %></span></div>
                                <div class="header-status-badge-container">
                                    <asp:Label ID="lblStatusBadge" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                </div>
                            </div>

                            <div class="admin-order-card-body">
                                <div class="admin-shipping-logistics-summary">
                                    <strong>Fulfillment Logistics Destination:</strong> <%# Eval("ShippingAddress") %>
                                    
                                    <div class="payment-meta-footer-row" style="margin-bottom: 10px;">
                                        <span>
                                            Payment Mode: <strong><%# Eval("PaymentMethod") %></strong> | 
                                            Ref/UTR: <strong class="monospace-white"><%# string.IsNullOrEmpty(Eval("TransactionReference").ToString()) ? "N/A" : Eval("TransactionReference") %></strong>
                                        </span>
                                    </div>

                                    <div class="tracking-meta-summary" style="font-size:12px; color:#aaa; background:#0c0c0c; border:1px dashed #222; padding:8px 12px; border-radius:4px; margin-bottom:15px;">
                                        Courier: <strong style="color:#fff;"><%# string.IsNullOrEmpty(Eval("TrackingPartner").ToString()) ? "Unassigned" : Eval("TrackingPartner") %></strong> | 
                                        Waybill: <strong style="color:#0076df;"><%# string.IsNullOrEmpty(Eval("TrackingNumber").ToString()) ? "N/A" : Eval("TrackingNumber") %></strong> | 
                                        Est. Arrival: <strong style="color:#25D366;"><%# Eval("EstimatedDeliveryDate") == DBNull.Value ? "Pending" : Eval("EstimatedDeliveryDate", "{0:dd MMM yyyy}") %></strong>
                                        <asp:LinkButton ID="lnkTogglePaymentEdit" runat="server" CssClass="btn-toggle-edit" Style="float:right; color:#0076df; font-weight:bold; text-decoration:none;" CommandName="TogglePaymentEdit" CommandArgument='<%# Container.ItemIndex %>'>Update Logistics & Status ✎</asp:LinkButton>
                                        <div style="clear:both;"></div>
                                    </div>

                                    <asp:Panel ID="pnlInlinePaymentEdit" runat="server" CssClass="payment-inline-edit-box" Visible="false" Style="background:#111; padding:15px; border-radius:6px; border:1px solid #222; margin-bottom:15px;">
                                        <div class="edit-fields-row" style="display:grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap:15px; margin-bottom:15px;">
                                            <div class="inline-input-group">
                                                <label style="display:block; font-size:11px; color:#888; margin-bottom:4px;">Transaction Reference ID / UTR</label>
                                                <asp:TextBox ID="txtEditTxnRef" runat="server" CssClass="inline-text-box" Style="width:100%; padding:6px; background:#000; border:1px solid #333; color:#fff;" Text='<%# Eval("TransactionReference") %>'></asp:TextBox>
                                            </div>
                                            <div class="inline-input-group">
                                                <label style="display:block; font-size:11px; color:#888; margin-bottom:4px;">Active Pipeline Status</label>
                                                <asp:DropDownList ID="ddlEditStatus" runat="server" CssClass="inline-text-box select-arrow-fix" Style="width:100%; padding:6px; background:#000; border:1px solid #333; color:#0076df; font-weight:bold;">
                                                    <asp:ListItem Value="Awaiting Verification">Awaiting Verification</asp:ListItem>
                                                    <asp:ListItem Value="Approved">Approved</asp:ListItem>
                                                    <asp:ListItem Value="Dispatched">Dispatched</asp:ListItem>
                                                    <asp:ListItem Value="Delivered">Delivered</asp:ListItem>
                                                    <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="inline-input-group">
                                                <label style="display:block; font-size:11px; color:#888; margin-bottom:4px;">Courier Partner Name</label>
                                                <asp:TextBox ID="txtCourierPartner" runat="server" CssClass="inline-text-box" Style="width:100%; padding:6px; background:#000; border:1px solid #333; color:#fff;" placeholder="e.g. Delhivery, BlueDart" Text='<%# Eval("TrackingPartner") %>'></asp:TextBox>
                                            </div>
                                            <div class="inline-input-group">
                                                <label style="display:block; font-size:11px; color:#888; margin-bottom:4px;">Waybill Tracking Number</label>
                                                <asp:TextBox ID="txtTrackingNumber" runat="server" CssClass="inline-text-box" Style="width:100%; padding:6px; background:#000; border:1px solid #333; color:#fff;" placeholder="e.g. 1234567890" Text='<%# Eval("TrackingNumber") %>'></asp:TextBox>
                                            </div>
                                            <div class="inline-input-group">
                                                <label style="display:block; font-size:11px; color:#888; margin-bottom:4px;">Est. Delivery Arrival Date</label>
                                                <asp:TextBox ID="txtEstDeliveryDate" runat="server" CssClass="inline-text-box" TextMode="Date" Style="width:100%; padding:5px; background:#000; border:1px solid #333; color:#fff;" Text='<%# Eval("EstimatedDeliveryDate") == DBNull.Value ? "" : Convert.ToDateTime(Eval("EstimatedDeliveryDate")).ToString("yyyy-MM-dd") %>'></asp:TextBox>
                                            </div>
                                        </div>
                                        <asp:Button ID="btnSaveInlinePayment" runat="server" Text="Update Logistics Parameters" CssClass="btn-inline-save" Style="background:#0076df; color:#fff; border:none; padding:8px 16px; border-radius:4px; font-weight:bold; cursor:pointer;" CommandName="SaveInlinePayment" CommandArgument='<%# Eval("OrderID") %>' />
                                    </asp:Panel>
                                </div>

                                <div class="admin-purchased-items-list">
                                    <h4 class="items-list-section-title">Purchased Manifest Line Items</h4>
                                    <asp:Repeater ID="rptAdminOrderItems" runat="server">
                                        <ItemTemplate>
                                            <div class="admin-item-manifest-row">
                                                <div class="manifest-item-details">
                                                    <span class="manifest-item-title"><%# Eval("Title") %></span>
                                                    <span class="manifest-item-specs">Scale Ratio: <%# Eval("Scale") %> | Brand Label: <%# Eval("BrandName") %></span>
                                                </div>
                                                <div class="manifest-item-pricing">
                                                    <span class="manifest-item-qty">Quantity: <%# Eval("Quantity") %></span>
                                                    <span class="manifest-item-price">Rs.<%# string.Format("{0:N2}", Eval("UnitPrice")) %></span></div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                                <asp:PlaceHolder ID="phVerificationBlock" runat="server" Visible='<%# Eval("Status").ToString().Equals("Awaiting Verification", StringComparison.OrdinalIgnoreCase) %>'>
                                    <div class="admin-receipt-verification-panel">
                                        <div class="verification-details-side">
                                            <a href='<%# Eval("PaymentScreenshotPath") %>' target="_blank" title="Click to view file attachment in safe terminal screen">
                                                <img src='<%# string.IsNullOrEmpty(Eval("PaymentScreenshotPath").ToString()) ? "/Assets/images/no-image.png" : Eval("PaymentScreenshotPath") %>' class="admin-receipt-thumbnail-preview" alt="Customer Deposit Receipt" onerror="this.src='/Assets/images/no-image.png';" />
                                            </a>
                                            <div class="reference-string-group">
                                                <span class="reference-string-label">Customer Declared Reference / UTR Number</span>
                                                <strong class="reference-string-value"><%# Eval("TransactionReference") %></strong>
                                                <p class="verification-instructions-hint">Cross-examine this value metric against your current live merchant UPI / Bank ledger entries before approving delivery clearance.</p>
                                            </div>
                                        </div>
                                        
                                        <div class="verification-actions-side">
                                            <asp:LinkButton ID="lnkApprove" runat="server" CssClass="admin-btn-action btn-action-approve" CommandName="ApprovePayment" CommandArgument='<%# Eval("OrderID") %>'>Approve Transaction</asp:LinkButton>
                                            <asp:LinkButton ID="lnkReject" runat="server" CssClass="admin-btn-action btn-action-reject" CommandName="RejectPayment" CommandArgument='<%# Eval("OrderID") %>' OnClientClick="return confirm('Rejecting this order will return items to stock. Remember to process the manual cash transfer back to the buyer\'s account.');">Reject / Cancel Order</asp:LinkButton>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>

                            </div>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </main>
    </div>
</asp:Content>