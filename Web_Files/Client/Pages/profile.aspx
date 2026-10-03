<%@ Page Title="Profile | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/profile.css?v=2" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="profile-wrapper">
        <main class="profile-card">

            <header class="profile-header">
                <h1 class="profile-title">Your Profile</h1>
                <p class="profile-subtitle">Manage your personal information and account settings.</p>
            </header>

            <div class="status-message-container">
                <asp:Label ID="lblStatus" runat="server" CssClass="status-label-alert"></asp:Label>
            </div>

            <div class="welcome-container">
                <h2 class="welcome-heading">Welcome, <asp:Literal ID="litWelcomeName" runat="server"></asp:Literal></h2>
            </div>

            <div class="profile-form">

                <div class="top-action-group">
                    <button type="button" class="btn btn-action-toggle" onclick="togglePasswordSection()">Change Password</button>
                </div>

                <div class="form-group-clean">
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-input-clean" Placeholder="Enter your full name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvName" runat="server"
                        ControlToValidate="txtName" ErrorMessage="Name is required"
                        CssClass="validation-error" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="form-group-clean">
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input-clean disabled-input" Placeholder="Email Address"></asp:TextBox>
                </div>

                <div class="form-group-clean">
                    <asp:TextBox ID="txtNumber" runat="server" CssClass="form-input-clean" Placeholder="Enter your phone number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPhone" runat="server"
                        ControlToValidate="txtNumber" ErrorMessage="Phone number is required"
                        CssClass="validation-error" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revPhone" runat="server"
                        ControlToValidate="txtNumber" ValidationExpression="^[0-9]{10}$"
                        ErrorMessage="Please enter a valid 10-digit phone number"
                        CssClass="validation-error" Display="Dynamic">
                    </asp:RegularExpressionValidator>
                </div>

                <!-- Sliding Security Credentials Reset Drawer -->
                <section id="passwordDrawer" class="password-sliding-drawer hidden-drawer">
                    <h3 class="drawer-title">Update Security Credentials</h3>
                    
                    <asp:PlaceHolder ID="phStandardReset" runat="server" Visible="true">
                        <div class="form-group-clean">
                            <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" PointChar="•" CssClass="form-input-clean" placeholder="Current Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCurrentPass" runat="server" 
                                ControlToValidate="txtCurrentPassword" ErrorMessage="Current password required." 
                                CssClass="validation-error" Display="Dynamic" ValidationGroup="PasswordGroup">
                            </asp:RequiredFieldValidator>
                            <div class="forgot-current-link-wrapper">
                                <asp:LinkButton ID="lnkForgotCurrent" runat="server" OnClick="lnkForgotCurrent_Click" CssClass="inline-forgot-trigger" CausesValidation="False">Forgot Current Password?</asp:LinkButton>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phOtpVerification" runat="server" Visible="false">
                        <div class="form-group-clean verification-otp-alert-card">
                            <label class="otp-alert-label">* Recovery Mode Active</label>
                            <asp:TextBox ID="txtProfileOTP" runat="server" CssClass="form-input-clean" placeholder="Enter 6-Digit Email OTP" MaxLength="6"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvProfileOTP" runat="server" 
                                ControlToValidate="txtProfileOTP" ErrorMessage="Verification OTP code required." 
                                CssClass="validation-error" Display="Dynamic" ValidationGroup="PasswordGroup">
                            </asp:RequiredFieldValidator>
                            <div style="margin-top: 5px;">
                                <asp:LinkButton ID="lnkCancelRecovery" runat="server" OnClick="lnkCancelRecovery_Click" CssClass="inline-cancel-trigger" CausesValidation="False">Cancel OTP Recovery</asp:LinkButton>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="form-group-clean">
                        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" PointChar="•" CssClass="form-input-clean" placeholder="New Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNewPass" runat="server" 
                            ControlToValidate="txtNewPassword" ErrorMessage="New password required." 
                            CssClass="validation-error" Display="Dynamic" ValidationGroup="PasswordGroup">
                        </asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group-clean">
                        <asp:TextBox ID="txtConfirmNewPassword" runat="server" TextMode="Password" PointChar="•" CssClass="form-input-clean" placeholder="Confirm New Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvConfirmNewPass" runat="server" 
                            ControlToValidate="txtConfirmNewPassword" ErrorMessage="Please confirm your new password." 
                            CssClass="validation-error" Display="Dynamic" ValidationGroup="PasswordGroup">
                        </asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvPasswordMatch" runat="server" 
                            ControlToValidate="txtConfirmNewPassword" ControlToCompare="txtNewPassword" 
                            ErrorMessage="Passwords do not match." CssClass="validation-error" 
                            Display="Dynamic" ValidationGroup="PasswordGroup">
                        </asp:CompareValidator>
                    </div>

                    <div class="form-actions-row">
                        <asp:Button ID="btnUpdatePassword" runat="server" Text="Confirm New Password" 
                            CssClass="btn btn-save" ValidationGroup="PasswordGroup" OnClick="btnUpdatePassword_Click" />
                    </div>
                </section>

                <div class="form-actions-row bottom-actions">
                    <asp:Button ID="btnUpdate" runat="server" Text="Save" CssClass="btn btn-save" OnClick="btnUpdate_Click" />
                    <a href="../../../homePage.aspx" class="btn btn-cancel">Cancel</a>
                    <button type="button" class="btn btn-reset" onclick="resetFormFields()">Reset</button>
                </div>

            </div>
        </main>
    </div>

    <script type="text/javascript">
        function togglePasswordSection() {
            var drawer = document.getElementById('passwordDrawer');
            if (drawer) drawer.classList.toggle('hidden-drawer');
        }

        function resetFormFields() {
            var nameField = document.getElementById('<%= txtName.ClientID %>');
            var phoneField = document.getElementById('<%= txtNumber.ClientID %>');

            if (nameField) nameField.value = nameField.defaultValue;
            if (phoneField) phoneField.value = phoneField.defaultValue;

            var currentPass = document.getElementById('<%= txtCurrentPassword.ClientID %>');
            var newPass = document.getElementById('<%= txtNewPassword.ClientID %>');
            var confirmPass = document.getElementById('<%= txtConfirmNewPassword.ClientID %>');
            var otpField = document.getElementById('<%= txtProfileOTP.ClientID %>');

            if (currentPass) currentPass.value = '';
            if (newPass) newPass.value = '';
            if (confirmPass) confirmPass.value = '';
            if (otpField) otpField.value = '';
        }
    </script>
</asp:Content>