<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="privacy.aspx.cs" Inherits="ShowsGarage.Web_Files.Others.privacy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Show's Garage | Privacy</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
    .policy-box { background: #121212; color: #aec6c9; padding: 50px; border-radius: 15px; max-width: 900px; margin: 20px auto; line-height: 1.8; }
    .policy-box h1 { color: #FAEAB1; font-size: 2.5rem; margin-bottom: 5px; }
    .policy-box h3 { color: #fff; margin-top: 30px; border-left: 3px solid #FAEAB1; padding-left: 15px; }
    .highlight { color: #FAEAB1; }
</style>

<div class="policy-box">
    <h1>Privacy Policy</h1>
    <p>Last Updated: May 13, 2026</p>
    <p>At <span class="highlight">Show's Garage</span>, we prioritize the security of our members. This policy outlines how we handle your information.</p>

    <h3>1. Data Collection</h3>
    <p>We collect essential information required for account creation and service delivery, including your name, verified email address, and contact number.</p>

    <h3>2. Authentication & OTP</h3>
    <p>To prevent unauthorized access, we use <span class="highlight">One-Time Passwords (OTP)</span> for registration and sensitive actions. These codes are temporary and encrypted during transmission.</p>

    <h3>3. Password Security</h3>
    <p>Your privacy is paramount. We enforce strict password requirements (8-20 characters, symbols, and capitals) and store them using industry-standard hashing to ensure they are never visible to anyone, including our staff.</p>

    <h3>4. Contact</h3>
    <p>For any privacy-related inquiries, please reach out via our Contact page or at privacy@showsgarage.com.</p>
</div>
</asp:Content>
