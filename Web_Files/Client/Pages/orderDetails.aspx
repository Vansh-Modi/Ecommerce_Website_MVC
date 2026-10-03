<%@ Page Title="Order Tracking | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="order-details.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.order_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/myOrders.css?v=2" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container narrow-tracking-wrapper">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Shipment Tracking Hub</h1>
                <a href="myOrders.aspx" class="btn btn-filter-pill">← Back to History</a>
            </div>

            <asp:Label ID="lblErrorMessage" runat="server" CssClass="status-msg-error" Visible="false"></asp:Label>

            <!-- Main Order Tracking Details Structural Panel -->
            <asp:Panel ID="pnlTrackingInfo" runat="server" CssClass="tracking-details-card">
                <div class="tracking-header-block">
                    <div class="tracking-header-text">
                        <h2>Order Tracking Data Matrix</h2>
                        <span>System Reference Mapping Sequence</span>
                    </div>
                    <span class="live-status-container">
                        Status: <asp:Label ID="lblStatus" runat="server" />
                    </span>
                </div>

                <div class="tracking-grid">
                    <div class="tracking-data-group">
                        <label>Order Reference ID</label>
                        <span class="highlight-brand-color">#<asp:Label ID="lblOrderID" runat="server" /></span>
                    </div>
                    <div class="tracking-data-group">
                        <label>Logistics Courier Partner</label>
                        <span><asp:Label ID="lblCourierName" runat="server" Text="Awaiting Dispatch Allocation" /></span>
                    </div>
                    <div class="tracking-data-group">
                        <label>Docket Tracking Waybill Number</label>
                        <span><asp:Label ID="lblTrackingNo" runat="server" Text="Pending Dispatch Assignment" /></span>
                    </div>
                    <div class="tracking-data-group">
                        <label>Estimated Delivery Arrival</label>
                        <span><asp:Label ID="lblEstDelivery" runat="server" Text="Awaiting Sourcing Clearance" /></span>
                    </div>
                </div>

                <div class="shipping-details-summary spec-tracking-summary">
                    <label>Delivery Dispatch Address Link</label>
                    <p><asp:Label ID="lblDeliveryAddress" runat="server" /></p>
                </div>
            </asp:Panel>

        </main>
    </div>
</asp:Content>