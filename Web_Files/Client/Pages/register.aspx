<%@ Page Title="Create Account | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/register.css?v=3" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="auth-theme-wrapper">
        <main class="auth-centered-container">
            <div class="auth-card-width">

                <!-- Roomy Header Section with Fixed Contrast Variables -->
                <header class="auth-header-block">
                    <h1 class="auth-main-heading">Join The Garage</h1>
                    <p class="auth-subtitle-text">Create an account to start managing your diecast collection log.</p>
                </header>

                <div class="profile-form">
                    <!-- Full Name Row -->
                    <div class="input-group">
                        <label>Full Name</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="auth-input" placeholder="Enter full name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Name is required" CssClass="auth-error-message" Display="Dynamic" ValidationGroup="RegisterVG"></asp:RequiredFieldValidator>
                    </div>

                    <!-- Contact Number Row -->
                    <div class="input-group">
                        <label>Phone Number</label>
                        <asp:TextBox ID="txtNumber" runat="server" CssClass="auth-input" placeholder="10-Digit Contact Number" MaxLength="10"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtNumber" ErrorMessage="Phone number is required" CssClass="auth-error-message" Display="Dynamic" ValidationGroup="RegisterVG"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revPhone" runat="server" ControlToValidate="txtNumber" ValidationExpression="^[0-9]{10}$" ErrorMessage="Please enter a valid 10-digit phone number" CssClass="auth-error-message" Display="Dynamic" ValidationGroup="RegisterVG"></asp:RegularExpressionValidator>
                    </div>

                    <!-- Email Address and Get OTP Row -->
                    <div class="input-group">
                        <label>Email Address</label>
                        <div class="auth-inline-input-row">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="auth-input auth-flex-input" placeholder="name@email.com" TextMode="Email"></asp:TextBox>
                            <asp:Button ID="btnGetOTP" runat="server" Text="Get OTP" CssClass="btn-inline-action" OnClick="btnGetOTP_Click" ValidationGroup="RegisterVG" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email address is required" CssClass="auth-error-message" Display="Dynamic" ValidationGroup="RegisterVG"></asp:RequiredFieldValidator>
                    </div>

                    <!-- Verification OTP Entry Row -->
                    <div class="input-group">
                        <label>Enter Verification OTP</label>
                        <asp:TextBox ID="txtOTP" runat="server" CssClass="auth-input" placeholder="6-Digit Code" MaxLength="6"></asp:TextBox>
                        <div class="auth-resend-link-box">
                            <asp:LinkButton ID="btnResendOTP" runat="server" OnClick="btnGetOTP_Click" CssClass="auth-resend-btn" CausesValidation="False">Resend OTP Code</asp:LinkButton>
                        </div>
                    </div>

                    <!-- Password Configuration Rows -->
                    <div class="input-group">
                        <label>Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required" CssClass="auth-error-message" Display="Dynamic" ValidationGroup="RegisterVG"></asp:RequiredFieldValidator>
                    </div>

                    <div class="input-group">
                        <label>Confirm Password</label>
                        <asp:TextBox ID="txtConfirmPass" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvConfirmPass" runat="server" ControlToValidate="txtConfirmPass" ErrorMessage="Please confirm your password" CssClass="auth-error-message" Display="Dynamic" ValidationGroup="RegisterVG"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvPasswordMatch" runat="server" ControlToValidate="txtConfirmPass" ControlToCompare="txtPassword" ErrorMessage="Passwords do not match." CssClass="auth-error-message" Display="Dynamic" ValidationGroup="RegisterVG"></asp:CompareValidator>
                    </div>

                    <!-- Registration Submit Action -->
                    <asp:Button ID="btnRegister" runat="server" Text="Complete Registration" CssClass="auth-primary-submit-btn" OnClick="btnRegister_Click" ValidationGroup="RegisterVG" />

                    <!-- Generous Footer Segment Layout -->
                    <footer class="auth-footer-routing-box">
                        <div class="auth-footer-text">
                            Already part of the garage? <a href="/Web_Files/Master_Pages/Pages/login.aspx" class="auth-redirect-link">Log In Instead</a>
                        </div>
                        <asp:Label ID="lblError" runat="server" CssClass="auth-system-alert-text"></asp:Label>
                    </footer>
                </div>

            </div>
        </main>
    </div>
</asp:Content>