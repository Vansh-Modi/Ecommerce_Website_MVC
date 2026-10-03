<%@ Page Title="Login & Recovery | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="ShowsGarage.Web_Files.Master_Pages.Pages.login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="/Web_Files/Master_Pages/Styles/login.css?v=2" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="auth-wrapper">
        <div class="auth-card">
            <asp:MultiView ID="mvAuth" runat="server" ActiveViewIndex="0">
                
                <!-- View 1: Standard Login Screen Panel -->
                <asp:View ID="vLogin" runat="server">
                    <div class="auth-header">
                        <h2>Welcome Back</h2>
                        <p>Login to your garage account</p>
                    </div>
                    <div class="auth-form">
                        <div class="input-group">
                            <label>Email Address 
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="*(Required)" ForeColor="Red" ValidationGroup="LoginVG"></asp:RequiredFieldValidator>
                            </label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="auth-input" placeholder="Enter your email" TextMode="Email"></asp:TextBox>
                        </div>
                        <div class="input-group">
                            <label>Password 
                                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" Display="Dynamic" ErrorMessage="*(Required)" ForeColor="Red" ValidationGroup="LoginVG"></asp:RequiredFieldValidator>
                            </label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnLogin" runat="server" Text="LOGIN" CssClass="auth-btn" OnClick="btnLogin_Click" ValidationGroup="LoginVG" />
                        
                        <div class="auth-footer">
                            <p style="margin-bottom: 8px;">
                                <asp:LinkButton ID="lnkGoToForgot" runat="server" OnClick="lnkGoToForgot_Click" ForeColor="#bbb" Font-Size="13px" CausesValidation="False">Forgot Password?</asp:LinkButton>
                            </p>
                            <p>New to the garage? <a href="/Web_Files/Client/Pages/register.aspx">Create Account</a></p>
                            <div style="margin-top: 10px;">
                                <asp:Label ID="lblError" runat="server" Font-Size="13px"></asp:Label>
                            </div>
                        </div>
                    </div>
                </asp:View>

                <!-- View 2: Forgot Password Initiation Screen Panel -->
                <asp:View ID="vForgotPassword" runat="server">
                    <div class="auth-header">
                        <h2>Recover Password</h2>
                        <p>Enter your email to receive a recovery code</p>
                    </div>
                    <div class="auth-form">
                        <div class="input-group">
                            <label>Registered Email Address
                                <asp:RequiredFieldValidator ID="rfvForgotEmail" runat="server" ControlToValidate="txtForgotEmail" Display="Dynamic" ErrorMessage="*(Required)" ForeColor="Red" ValidationGroup="ForgotVG"></asp:RequiredFieldValidator>
                            </label>
                            <asp:TextBox ID="txtForgotEmail" runat="server" CssClass="auth-input" placeholder="name@email.com" TextMode="Email"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnSendRecovery" runat="server" Text="SEND OTP" CssClass="auth-btn" OnClick="btnSendRecovery_Click" ValidationGroup="ForgotVG" />
                        
                        <div class="auth-footer">
                            <p><asp:LinkButton ID="lnkBackToLogin1" runat="server" OnClick="lnkBackToLogin_Click" ForeColor="#bbb" CausesValidation="False">Back to Login</asp:LinkButton></p>
                            <div style="margin-top: 10px;">
                                <asp:Label ID="lblForgotError" runat="server" Font-Size="13px"></asp:Label>
                            </div>
                        </div>
                    </div>
                </asp:View>

                <!-- View 3: OTP Verification and Reset Core Update Screen Panel -->
                <asp:View ID="vResetPassword" runat="server">
                    <div class="auth-header">
                        <h2>Set New Password</h2>
                        <p>Enter the OTP received in your inbox and update credentials</p>
                    </div>
                    <div class="auth-form">
                        <div class="input-group">
                            <label>6-Digit Verification Code
                                <asp:RequiredFieldValidator ID="rfvForgotOTP" runat="server" ControlToValidate="txtForgotOTP" Display="Dynamic" ErrorMessage="*(Required)" ForeColor="Red" ValidationGroup="ResetVG"></asp:RequiredFieldValidator>
                            </label>
                            <asp:TextBox ID="txtForgotOTP" runat="server" CssClass="auth-input" placeholder="Ex: 123456" MaxLength="6"></asp:TextBox>
                        </div>
                        <div class="input-group">
                            <label>New Password
                                <asp:RequiredFieldValidator ID="rfvNewPass" runat="server" ControlToValidate="txtNewPass" Display="Dynamic" ErrorMessage="*(Required)" ForeColor="Red" ValidationGroup="ResetVG"></asp:RequiredFieldValidator>
                            </label>
                            <asp:TextBox ID="txtNewPass" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnResetSubmit" runat="server" Text="UPDATE PASSWORD" CssClass="auth-btn" OnClick="btnResetSubmit_Click" ValidationGroup="ResetVG" />
                        
                        <div class="auth-footer">
                            <p><asp:LinkButton ID="lnkBackToLogin2" runat="server" OnClick="lnkBackToLogin_Click" ForeColor="#bbb" CausesValidation="False">Cancel and Back</asp:LinkButton></p>
                            <div style="margin-top: 10px;">
                                <asp:Label ID="lblResetError" runat="server" Font-Size="13px"></asp:Label>
                            </div>
                        </div>
                    </div>
                </asp:View>

            </asp:MultiView>
        </div>
    </div>
</asp:Content>