<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="shipping.aspx.cs" Inherits="ShowsGarage.Web_Files.Others.shipping" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Show's Garage | Shipping</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
    .shipping-box { background: #121212; color: #aec6c9; padding: 50px; border-radius: 15px; max-width: 900px; margin: 20px auto; }
    .shipping-box h1 { color: #FAEAB1; }
    .shipping-box h3 { color: #fff; margin-top: 25px; }
    .status-badge { display: inline-block; padding: 5px 15px; background: #1e1e1e; border: 1px solid #FAEAB1; color: #FAEAB1; border-radius: 20px; font-size: 12px; margin-bottom: 20px; }
</style>

<div class="shipping-box">
    <div class="status-badge">Domestic Shipping Available</div>
    <h1>Shipping & Delivery</h1>
    
    <h3>1. Shipping Locations</h3>
    <p>Show's Garage currently ships high-performance automotive parts and accessories across <span style="color:#fff;">India</span>, with express logistics available for the <span style="color:#fff;">Surat and Gujarat</span> regions.</p>

    <h3>2. Delivery Timelines</h3>
    <ul>
        <li><strong>Local (Surat):</strong> 24-48 Hours</li>
        <li><strong>Gujarat Region:</strong> 2-3 Business Days</li>
        <li><strong>Rest of India:</strong> 5-7 Business Days</li>
    </ul>

    <h3>3. Order Tracking</h3>
    <p>Once your order is processed, a tracking ID will be sent to your verified email. You can monitor your shipment directly through our integrated logistics partners.</p>

    <h3>4. Packaging</h3>
    <p>All parts are shipped in heavy-duty, tamper-evident packaging to ensure they arrive at your garage in pristine condition.</p>
</div>
</asp:Content>
