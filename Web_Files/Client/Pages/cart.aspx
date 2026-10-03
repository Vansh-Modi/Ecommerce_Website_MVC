<%@ Page Title="Your Cart | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="cart.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.cart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Your Cart | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/cart.css?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Your Shopping Cart</h1>
                <a href="shop.aspx" class="btn btn-filter-pill">← Continue Shopping</a>
            </div>

            <!-- Main Cart Structural Split Container -->
            <div class="cart-layout-container">
                
                <!-- Left Panel: List of Cart Items -->
                <div class="cart-items-column">
                    <asp:Label ID="lblEmptyMessage" runat="server" CssClass="cart-empty-text" Visible="false" Text="Your cart is currently empty."></asp:Label>
                    
                    <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
                        <ItemTemplate>
                            <div class="cart-item-row">
                                <div class="cart-item-image-frame">
                                    <img src='<%# ResolveUrl(Eval("ImagePath") != DBNull.Value && !string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? Eval("ImagePath").ToString() : "/Assets/images/no-image.png") %>' alt='<%# Eval("Title") %>' />
                                </div>
                                
                                <div class="cart-item-details">
                                    <span class="cart-item-brand"><%# Eval("BrandName") %></span>
                                    <h3 class="cart-item-title"><%# Eval("Title") %></h3>
                                    <span class="cart-item-price">Rs.<%# string.Format("{0:N0}", Eval("SellingPrice")) %></span>
                                </div>

                                <div class="cart-item-actions">
                                    <div class="quantity-control-wrapper">
                                        <asp:LinkButton ID="btnMinus" runat="server" CommandName="UpdateQty" CommandArgument='<%# Eval("ProductID") + "|minus" %>' CssClass="qty-btn">-</asp:LinkButton>
                                        <span class="qty-display-text"><%# Eval("Quantity") %></span>
                                        <asp:LinkButton ID="btnPlus" runat="server" CommandName="UpdateQty" CommandArgument='<%# Eval("ProductID") + "|plus" %>' CssClass="qty-btn">+</asp:LinkButton>
                                    </div>
                                    
                                    <div class="item-subtotal-block">
                                        <span class="subtotal-label">Total:</span>
                                        <span class="subtotal-amount">Rs.<%# string.Format("{0:N0}", Convert.ToDecimal(Eval("SellingPrice")) * Convert.ToInt32(Eval("Quantity"))) %></span>
                                    </div>

                                    <asp:LinkButton ID="btnRemove" runat="server" CommandName="RemoveItem" CommandArgument='<%# Eval("ProductID") %>' CssClass="cart-remove-btn">
                                        <span>Remove</span>
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <!-- Right Panel: Sticky Summary Block -->
                <div class="cart-summary-column" id="divSummary" runat="server">
                    <div class="summary-card">
                        <h2 class="summary-card-title">Order Summary</h2>
                        
                        <div class="summary-data-row">
                            <span>Items Count:</span>
                            <asp:Label ID="lblTotalItems" runat="server" Font-Bold="true" Text="0"></asp:Label>
                        </div>
                        <div class="summary-data-row total-highlight-row">
                            <span>Estimated Total:</span>
                            <span class="total-green-price">Rs.<asp:Label ID="lblCartTotal" runat="server" Text="0"></asp:Label></span>
                        </div>

                        <div class="summary-actions-block">
                            <asp:Button ID="btnCheckout" runat="server" Text="Proceed to Checkout" CssClass="btn btn-checkout-large" OnClick="btnCheckout_Click" />
                        </div>
                    </div>
                </div>

            </div>
        </main>
    </div>
</asp:Content>