<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="blogDetails.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.blogDetials" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="blog-container">
        <!-- Labels that will change based on what was clicked -->
        <h1 id="lblBlogTitle" runat="server"></h1>
        <p class="blog-meta">Published on: <span id="lblBlogDate" runat="server"></span></p>
        <hr />
        <div id="divBlogContent" runat="server" class="blog-content">
            <!-- Full blog text will go here -->
        </div>
    </div>
</asp:Content>