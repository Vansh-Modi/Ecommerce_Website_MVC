<%@ Page Title="Order Confirmed | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="order-success.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.order_success" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/confirmOrder.css?v=2" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper success-layout-center">
        <main class="shop-content-container success-narrow-container">
            
            <div class="success-card">
                
                <div class="success-checkmark-ring">
                    ✓
                </div>
                
                <h1 class="success-main-heading">Receipt Uploaded!</h1>
                
                <p class="success-meta-text">
                    Thank you for your business. Your order tracking identifier token is: 
                    <strong class="success-order-id">
                        #<asp:Label ID="lblOrderIdDisplay" runat="server" Text="00000"></asp:Label>
                    </strong>
                </p>

                <div class="success-info-panel">
                    <h3 class="info-panel-title">What Happens Next?</h3>
                    <ul class="info-panel-list">
                        <li>Our verification team will cross-check your submitted transaction reference number directly against our current ledger entries.</li>
                        <li>Once payment confirmation is successfully matched, your order configuration matrix updates to <strong class="text-green">"Approved"</strong>.</li>
                        <li>Your specialized items will be securely packed, dispatched from the garage hub, and tracking codes will be logged.</li>
                    </ul>
                </div>

                <p class="success-disclaimer">
                    An interactive historical statement tracking entry has been created in your profile log dashboard.
                </p>

                <div class="success-actions-block">
                    <a href="shop.aspx" class="btn-success-home">
                        Continue Shopping
                    </a>
                </div>

            </div>
        </main>
    </div>
</asp:Content>