<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="DeveloperProfile.aspx.cs" Inherits="ShowsGarage.Web_Files.Master_Pages.Pages.DeveloperProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Show's Garage | Developer Profile</title>
    <style>
        .profile-wrapper {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 50vh;
}

.profile-card {
    background-color: #1a1a1a;
    padding: 40px;
    border-radius: 15px;
    border: 2px solid #D4AF37;
    text-align: center;
    color: #fff;
    width: 100%;
    max-width: 400px;
    box-shadow: 0 10px 30px rgba(0,0,0,0.5);
}

.gold-line { border: 0; border-top: 1px solid #D4AF37; margin: 20px 0; }

.contact-item { margin: 15px 0; font-size: 16px; }

.contact-item a { color: #FAEAB1; text-decoration: none; }

.back-btn {
    display: inline-block;
    margin-top: 20px;
    padding: 10px 20px;
    background: #D4AF37;
    color: #1a1a1a;
    text-decoration: none;
    font-weight: bold;
    border-radius: 5px;
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="profile-wrapper">
        <div class="profile-card">
            <h2 class="gold-text">Vansh Modi</h2>
            <p class="profile-title">Lead Developer & Designer</p>
            <hr class="gold-line" />
            
            <div class="contact-item">
                <strong>Email:</strong> <a href="mailto:vanshmodi268@gmail.com">vanshmodi268@gmail.com</a>
            </div>
            <div class="contact-item">
                <strong>LinkedIn:</strong> <a href="https://www.linkedin.com/feed/">linkedin.com/in/VanshModi</a>
            </div>
            <div class="contact-item">
                <strong>GitHub:</strong> <a href="https://github.com/Vansh-Modi">github.com/VanshModi</a>
            </div>
            
            <br />
            <a href="/../../../homePage.aspx" class="back-btn">Back to Garage</a>
        </div>
    </div>
</asp:Content>
