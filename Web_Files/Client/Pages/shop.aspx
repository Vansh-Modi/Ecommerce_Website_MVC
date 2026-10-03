<%@ Page Title="Shop | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="shop.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.shop" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/shop.css?v=3" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Our Featured Collection</h1>
            </div>

            <!-- Horizontal Category Navigation Scroll UI Component -->
            <div class="final-scroll-box">
                <div class="final-scroll-content">
                    
                    <asp:LinkButton ID="lnkAll" runat="server" 
                        CssClass="btn-filter-pill-isolated active-pill-isolated" 
                        OnClick="CategoryFilter_Click" 
                        CommandArgument="0">All Collections</asp:LinkButton>
                    
                    <asp:Repeater ID="rptCategories" runat="server">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkCat" runat="server" 
                                CssClass="btn-filter-pill-isolated" 
                                OnClick="CategoryFilter_Click" 
                                CommandArgument='<%# Eval("CategoryID") %>'><%# Eval("CategoryName") %></asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>

            <!-- Core Structural Products Presentation Matrix Grid -->
            <div class="products-gallery-grid">
                <asp:Repeater ID="rptProducts" runat="server">
                    <ItemTemplate>
                        <div class='<%# Convert.ToInt32(Eval("StockQuantity")) <= 0 ? "product-gallery-card sold-out" : "product-gallery-card" %>'>
                            <a href='productDetails.aspx?id=<%# Eval("ProductID") %>' class="card-clickable-area">
                                <div class="product-image-frame">
                                    <img src='<%# ResolveUrl(Eval("ImagePath") != DBNull.Value && !string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? Eval("ImagePath").ToString() : "/Assets/images/no-image.png") %>' alt='<%# Eval("Title") %>' class="product-gallery-img" />
                                </div>
                                <div class="product-details-block">
                                    <h3><%# Eval("BrandName") %></h3>
                                    <p class="product-subtitle-text"><%# Eval("Title") %></p>
                                    <span class="product-price-tag">Rs.<%# string.Format("{0:N0}", Eval("SellingPrice")) %></span>
                                </div>
                            </a>
                            <div class="card-action-block">
                                <%# Convert.ToInt32(Eval("StockQuantity")) <= 0 ? 
                                    "<span class='btn-sold-out'>Out of Stock</span>" : 
                                    "<a href='productDetails.aspx?id=" + Eval("ProductID") + "' class='btn btn-add-to-cart'>View Details</a>" 
                                %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </main>
    </div>
</asp:Content>