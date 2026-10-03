<%@ Page Title="Contact Us | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="contactUs.aspx.cs" Inherits="ShowsGarage.Web_Files.Others.contactUs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Show's Garage | Contact Us </title>
    
    <style type="text/css">
        /* Base Container Styles (Your Exact Original CSS Layout Preserved) */
        a {
            color: #aec6c9;
            text-decoration: none;
        }
        
        .garage-content {
            background: #121212;
            color: #aec6c9;
            padding: 40px;
            border-radius: 15px;
            border: 1px solid #333;
            max-width: 900px;
            margin: 20px auto;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .garage-header {
            color: #FAEAB1;
            border-bottom: 2px solid #FAEAB1;
            padding-bottom: 10px;
            margin-bottom: 25px;
            text-transform: uppercase;
            letter-spacing: 2px;
            font-size: 1.8rem;
        }

        .contact-grid {
            display: grid;
            grid-template-columns: 1.5fr 1fr;
            gap: 30px;
        }

        .input-group {
            margin-bottom: 20px;
        }

        .input-group label {
            display: block;
            margin-bottom: 8px;
            color: #fff;
            font-weight: bold;
        }

        .garage-input {
            width: 100%;
            padding: 14px;
            background: #1e1e1e;
            border: 1px solid #444;
            color: #fff;
            border-radius: 8px;
            box-sizing: border-box;
            transition: 0.3s ease;
        }

        .garage-input:focus {
            border-color: #FAEAB1;
            outline: none;
            background: #252525;
        }

        .garage-btn {
            background: #FAEAB1;
            color: #121212;
            border: none;
            padding: 16px;
            font-weight: bold;
            cursor: pointer;
            border-radius: 8px;
            width: 100%;
            transition: 0.3s;
            font-size: 1rem;
            letter-spacing: 1px;
        }

        .garage-btn:hover {
            background: #e0d19d;
            transform: translateY(-2px);
        }

        .info-card {
            background: #1e1e1e;
            padding: 25px;
            border-radius: 12px;
            border-top: 4px solid #FAEAB1;
            height: fit-content;
        }

        /* --- MOBILE RESPONSIVENESS --- */
        @media (max-width: 768px) {
            .garage-content {
                margin: 10px;
                padding: 20px;
            }

            .contact-grid {
                grid-template-columns: 1fr;
                gap: 40px;
            }

            .garage-header {
                font-size: 1.4rem;
                text-align: center;
            }

            .info-card {
                order: 2;
                text-align: center;
            }

            .garage-btn {
                padding: 18px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="garage-content">
        <h2 class="garage-header">Get In Touch</h2>
        
        <asp:Label ID="lblStatusAlert" runat="server" CssClass="admin-alert-banner" Visible="false" Style="margin-bottom: 25px; display: block; font-weight: bold;"></asp:Label>

        <div class="contact-grid">
            <div class="form-section">
                <div class="input-group">
                    <label>Full Name 
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtContactName" Display="Dynamic" ErrorMessage="Enter Name" ForeColor="Red" ValidationGroup="ContactGroup"></asp:RequiredFieldValidator>
                    </label>
                    <asp:TextBox ID="txtContactName" runat="server" CssClass="garage-input" placeholder="Your Name"></asp:TextBox>
                </div>
                
                <div class="input-group">
                    <label>Email Address 
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtContactEmail" Display="Dynamic" ErrorMessage="Enter Email" ForeColor="Red" ValidationGroup="ContactGroup"></asp:RequiredFieldValidator>
                    </label>
                    <asp:TextBox ID="txtContactEmail" runat="server" CssClass="garage-input" placeholder="name@example.com" TextMode="Email"></asp:TextBox>
                </div>
                
                <div class="input-group">
                    <label>Message 
                        <asp:RequiredFieldValidator ID="rfvMessage" runat="server" ControlToValidate="txtMessage" Display="Dynamic" ErrorMessage="Enter Message" ForeColor="Red" ValidationGroup="ContactGroup"></asp:RequiredFieldValidator>
                    </label>
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="garage-input" TextMode="MultiLine" Rows="5" placeholder="Tell us about your car..."></asp:TextBox>
                </div>
                
                <asp:Button ID="btnSendMessage" runat="server" Text="SEND MESSAGE" CssClass="garage-btn" OnClick="btnSendMessage_Click" ValidationGroup="ContactGroup" />
            </div>

            <div class="info-card">
                <h4 style="color: #fff; margin-top: 0; font-size: 1.2rem;">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </h4>
                <h4 style="color: #fff; margin-top: 0; font-size: 1.2rem;">Garage Hub</h4>
                <p>📍 <a href="https://maps.google.com" target="_blank" style="color: #aec6c9;">Surat, Gujarat, India</a></p>
                
                <p>📧 <asp:HyperLink ID="hlEmail" runat="server" ForeColor="#AEC6C9"></asp:HyperLink></p>
                <p>📞 <asp:HyperLink ID="hlPhone" runat="server" ForeColor="#AEC6C9"></asp:HyperLink></p>
                
                <hr style="border: 0; border-top: 1px solid #444; margin: 20px 0;">
                <p style="font-size: 14px; color: #FAEAB1; font-style: italic; margin: 0;">
                    "Precision performance for the modern driver."
                </p>
            </div>
        </div>
    </div>
</asp:Content>