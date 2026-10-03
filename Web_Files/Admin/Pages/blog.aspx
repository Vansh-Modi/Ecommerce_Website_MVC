<%@ Page Title="Manage Blogs | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="blogs.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_blogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Blogs | Admin Hub</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/blogs.css?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container wide-fluid-layout">
            
            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Garage News & Blog Editor</h1>
                <span class="admin-pill-badge">Content Management</span>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="admin-alert-banner" Visible="false"></asp:Label>
            <asp:HiddenField ID="hfActiveBlogID" runat="server" Value="" />

            <div class="blogs-split-grid">
                
                <div class="blog-form-column">
                    <div class="settings-card-panel">
                        <h2 class="settings-card-title"><asp:Literal ID="litFormTitle" runat="server" Text="Compose New Article"></asp:Literal></h2>
                        
                        <div class="settings-form-grid">
                            <div class="form-group full-width">
                                <label class="settings-label">Article Title</label>
                                <asp:TextBox ID="txtTitle" runat="server" CssClass="settings-input" placeholder="e.g., Rare 1:18 Nissan GTR Release Explored"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Short Summary / Excerpt</label>
                                <asp:TextBox ID="txtExcerpt" runat="server" CssClass="settings-input" placeholder="A brief hook text displayed on the news feed list cards..."></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Upload Showcase Cover Image</label>
                                <asp:FileUpload ID="fileBlogImg" runat="server" CssClass="settings-input file-picker" />
                                <asp:TextBox ID="txtCurrentImgPath" runat="server" CssClass="settings-input" Enabled="false" Visible="false"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="settings-label">Main Article Narrative Body Content</label>
                                <asp:TextBox ID="txtContent" runat="server" CssClass="settings-input text-area blog-rich-text" TextMode="MultiLine" Rows="10" placeholder="Type out your full blog news statement content here..."></asp:TextBox>
                            </div>

                            <div class="summary-actions-block full-width processing-buttons-row">
                                <asp:Button ID="btnSaveBlog" runat="server" Text="Publish Article" CssClass="btn-settings-save" OnClick="btnSaveBlog_Click" />
                                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn-settings-cancel" OnClick="btnCancelEdit_Click" Visible="false" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="blog-list-column">
                    <div class="settings-card-panel premium-border">
                        <h2 class="settings-card-title">Live Editorial Articles</h2>
                        
                        <div class="blog-grid-responsive-scroll">
                            <asp:Repeater ID="rptBlogsLedger" runat="server" OnItemCommand="rptBlogsLedger_ItemCommand">
                                <HeaderTemplate>
                                    <table class="admin-blog-table">
                                        <thead>
                                            <tr>
                                                <th style="text-align: center; width: 70px;">Cover</th>
                                                <th>Article Metadata</th>
                                                <th style="text-align: center; width: 110px;">Actions</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td style="text-align: center; vertical-align: middle;">
                                            <img src='<%# ResolveUrl(string.IsNullOrEmpty(Eval("BlogImage").ToString()) ? "~/Assets/images/default-blog.png" : "~" + Eval("BlogImage").ToString().Replace("~","")) %>' class="blog-row-thumbnail" alt="Cover" />
                                        </td>
                                        <td>
                                            <span class="blog-row-headline"><%# Eval("BlogTitle") %></span>
                                            <span class="blog-row-date">Published: <%# Eval("PublishDate", "{0:dd MMM yyyy}") %></span>
                                            <p class="blog-row-excerpt-clip"><%# Eval("Excerpt") %></p>
                                        </td>
                                        <td style="text-align: center; vertical-align: middle;">
                                            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="blog-trigger-link edit-mode-color" CommandName="EditBlog" CommandArgument='<%# Eval("BlogId") %>'>Edit</asp:LinkButton>
                                            <span style="color:#222; margin:0 3px;">|</span>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="blog-trigger-link delete-mode-color" CommandName="DeleteBlog" CommandArgument='<%# Eval("BlogId") %>' OnClientClick="return confirm('Are you sure you want to drop this article entry permanently from the news wire?');">Delete</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>

            </div>
        </main>
    </div>
</asp:Content>