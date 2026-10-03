<%@ Page Title="Customer Support Tickets | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="contactUs.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_support" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Customer Help Desk Tickets | Admin Panel</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/contactUs.css?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container standard-wide-layout" style="max-width: 950px;">
            
            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Customer Help Desk Mailbox</h1>
                <span class="admin-pill-badge badge-support">Active Communications</span>
            </div>

            <asp:Label ID="lblSupportStatus" runat="server" CssClass="admin-alert-banner alert-success" Visible="false"></asp:Label>

            <asp:Panel ID="pnlNoTickets" runat="server" Visible="false" CssClass="empty-mailbox-card">
                <div class="mailbox-icon">📬</div>
                <h3>Mailbox Empty</h3>
                <p>No pending consumer help inquiries logged right now.</p>
            </asp:Panel>

            <div class="support-tickets-stack">
                <asp:Repeater ID="rptSupportTickets" runat="server" OnItemCommand="rptSupportTickets_ItemCommand">
                    <ItemTemplate>
                        <div class="ticket-card-panel">
                            <div class="ticket-header-strip">
                                <div class="ticket-author-info">
                                    <span class="author-avatar"><%# Eval("name").ToString().Substring(0,1).ToUpper() %></span>
                                    <div class="author-meta">
                                        <strong class="author-name"><%# Eval("name") %></strong>
                                        <a href='mailto:<%# Eval("email") %>' class="author-email-link"><%# Eval("email") %></a>
                                    </div>
                                </div>
                                <div class="ticket-id-tag">Ticket Reference: #<%# Eval("contactId") %></div>
                            </div>
                            
                            <div class="ticket-body-content">
                                <p class="ticket-message-paragraph"><%# Eval("message") %></p>
                            </div>

                            <div class="ticket-footer-actions">
                                <a href='mailto:<%# Eval("email") %>?subject=Regarding Your Inquiry at Show-s Garage' class="btn-ticket-reply">Launch Email Client Reply ✉</a>
                                <asp:LinkButton ID="lnkArchive" runat="server" CssClass="btn-ticket-archive" CommandName="ArchiveTicket" CommandArgument='<%# Eval("contactId") %>' OnClientClick="return confirm('Archive this ticket from active view matrix?');">Archive & Resolve ✓</asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </main>
    </div>
</asp:Content>