<%@ Page Title="Secure Checkout | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="checkout.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/checkout.css?v=2" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <div class="shop-theme-wrapper">
        <main class="shop-content-container">

            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Secure Checkout</h1>
                <a href="cart.aspx" class="btn btn-filter-pill">← Return to Cart</a>
            </div>

            <asp:Label ID="lblStatusMessage" runat="server" CssClass="status-msg-error" Visible="false"></asp:Label>

            <div class="cart-layout-container">
                <asp:UpdatePanel ID="updMainCheckoutLayout" runat="server" UpdateMode="Conditional" RenderMode="Block" style="display: contents;">
                    <ContentTemplate>

                        <!-- Left Panel: Delivery & Billing Details Entry Form -->
                        <div class="cart-items-column">
                            <div class="checkout-form-card">
                                <h2 class="form-section-title">Shipping & Billing Information</h2>

                                <div class="form-grid">
                                    <div class="form-group full-width">
                                        <label class="form-label">Full Name</label>
                                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-input" placeholder="Enter Name"></asp:TextBox>
                                    </div>

                                    <div class="form-group phone-width">
                                        <label class="form-label">Phone Number</label>
                                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-input" placeholder="+91 xxxxx xxxxx" MaxLength="10"></asp:TextBox>
                                    </div>

                                    <div class="form-group full-width">
                                        <label class="form-label">Shipping Address</label>
                                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-input text-area" TextMode="MultiLine" Rows="3" placeholder="House/Apartment number, Street Name, Area..."></asp:TextBox>
                                    </div>

                                    <div class="form-group half-width">
                                        <label class="form-label">City</label>
                                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-input" placeholder="Surat / Mumbai / Bangalore" AutoPostBack="True" OnTextChanged="txtCity_TextChanged"></asp:TextBox>
                                    </div>

                                    <div class="form-group half-width">
                                        <label class="form-label">Pincode / Postal Code</label>
                                        <asp:TextBox ID="txtPincode" runat="server" CssClass="form-input" placeholder="39500X" MaxLength="6"></asp:TextBox>
                                    </div>

                                    <div class="form-group full-width">
                                        <label class="form-label">Payment Mode</label>
                                        <asp:DropDownList ID="ddlPaymentMode" runat="server" CssClass="form-input select-dropdown" AutoPostBack="True" OnSelectedIndexChanged="ddlPaymentMode_SelectedIndexChanged">
                                            <asp:ListItem Value="ONLINE" Selected="True">Online Transfer (UPI / QR / Bank)</asp:ListItem>
                                            <asp:ListItem Value="COD">Cash-on-Delivery</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblPaymentWarning" runat="server" CssClass="payment-warning-hint" Visible="false" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Right Panel: Sticky Order Summary & Verification -->
                        <div class="cart-summary-column">
                            <div class="summary-card">
                                <h2 class="summary-card-title">Review Your Order</h2>

                                <div class="checkout-items-review-list">
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <div class="checkout-mini-item">
                                                <div class="mini-item-info">
                                                    <span class="mini-item-title"><%# Eval("Title") %></span>
                                                    <span class="mini-item-meta">Qty: <%# Eval("Quantity") %></span>
                                                </div>
                                                <span class="mini-item-subtotal">Rs.<%# string.Format("{0:N0}", Convert.ToDecimal(Eval("SellingPrice")) * Convert.ToInt32(Eval("Quantity"))) %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                                <div class="summary-data-row pt-15-border">
                                    <span>Total Unique Items:</span>
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                </div>

                                <div class="summary-data-row muted-data-row">
                                    <span>Subtotal (Items Total):</span>
                                    <span>Rs.<asp:Label ID="Label2" runat="server" Text="0"></asp:Label></span>
                                </div>

                                <div class="summary-data-row">
                                    <span>Delivery Partner Fee:</span>
                                    <span class="shipping-cost-text">Rs.<asp:Label ID="Label3" runat="server" Text="0"></asp:Label></span>
                                </div>

                                <div class="summary-data-row total-highlight-row">
                                    <span>Grand Total / To Pay:</span>
                                    <span class="total-green-price">Rs.<asp:Label ID="Label4" runat="server" Text="0"></asp:Label></span>
                                </div>

                                <div class="mandate-box">
                                    <input type="checkbox" id="chkAgree" required />
                                    <label for="chkAgree">
                                        I authorize Show's Garage to act as my sourcing broker. I explicitly agree to pay the listed retail MRP alongside the specified brokerage commission for procurement.
                                    </label>
                                </div>

                                <div class="notice-alert-box">
                                    <span class="notice-alert-title">Important Notice</span>
                                    <p class="notice-alert-body">
                                        Payment is verified manually by our garage team. Your order will sit as <span class="highlight-inline">"Awaiting Verification"</span> until the transaction hits our statement ledger.
                                    </p>
                                </div>

                                <div class="summary-actions-block">
                                    <asp:Button ID="btnPageNavigation" runat="server" Text="Continue to Payment →" CssClass="btn btn-checkout-large" OnClick="btnPageNavigation_Click" />
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </main>
    </div>
</asp:Content>