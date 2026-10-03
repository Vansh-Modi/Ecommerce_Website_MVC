<%@ Page Title="Manage Users | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="users.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Users | Admin Dashboard</title>
    <link href="/Web_Files/Admin/Styles/settings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/users.css?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container wide-fluid-layout">
            
            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Registered Accounts Registry</h1>
                <span class="admin-pill-badge">User Access Security</span>
            </div>

            <asp:Label ID="lblAdminStatus" runat="server" CssClass="admin-alert-banner" Visible="false"></asp:Label>

            <div class="users-table-responsive-wrapper">
                <asp:Repeater ID="rptUsersRegistry" runat="server" OnItemDataBound="rptUsersRegistry_ItemDataBound" OnItemCommand="rptUsersRegistry_ItemCommand">
                    <HeaderTemplate>
                        <table class="admin-users-table">
                            <thead>
                                <tr>
                                    <th style="width: 60px; text-align: center;">ID</th>
                                    <th>Profile Identity Details</th>
                                    <th>Contact Info</th>
                                    <th style="text-align: center;">Security State</th>
                                    <th style="text-align: center; width: 140px;">System Role Mapping</th>
                                    <th>Activity Logs</th>
                                    <th style="text-align: center; width: 100px;">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center; font-family: monospace; font-weight: 700; color: #555;">
                                #<%# Eval("UserID") %>
                            </td>
                            
                            <td>
                                <span class="usr-profile-name"><%# Eval("FullName") %></span>
                                <span class="usr-meta-date">Joined: <%# Eval("CreatedAt", "{0:dd MMM yyyy}") %></span>
                            </td>
                            
                            <td>
                                <span class="usr-contact-line email-highlight"><%# Eval("Email") %></span>
                                <span class="usr-contact-line"><%# Eval("Phone") %></span>
                            </td>
                            
                            <td style="text-align: center; vertical-align: middle;">
                                <span class='<%# Convert.ToBoolean(Eval("IsVerified")) ? "verify-badge state-active" : "verify-badge state-pending" %>'>
                                    <%# Convert.ToBoolean(Eval("IsVerified")) ? "Verified" : "Pending" %>
                                </span>
                            </td>
                            
                            <td style="vertical-align: middle; text-align: center;">
                                <asp:DropDownList ID="ddlUserRole" runat="server" CssClass="usr-inline-dropdown select-arrow-badge">
                                    <asp:ListItem Value="Customer">Customer</asp:ListItem>
                                    <asp:ListItem Value="Admin">Admin</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            
                            <td>
                                <span class="usr-meta-log">Last Authentication Attempt:</span>
                                <span class="usr-timestamp-value">
                                    <%# Eval("LastLogin") != DBNull.Value ? Convert.ToDateTime(Eval("LastLogin")).ToString("dd MMM yyyy HH:mm") : "Never Logged In" %>
                                </span>
                            </td>
                            
                            <td class="usr-actions-cell" style="text-align: center; vertical-align: middle;">
                                <asp:LinkButton ID="lnkUpdateUser" runat="server" CssClass="usr-btn-link update-trigger" CommandName="UpdateUserRole" CommandArgument='<%# Eval("UserID") %>'>Save 💾</asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

        </main>
    </div>
</asp:Content>